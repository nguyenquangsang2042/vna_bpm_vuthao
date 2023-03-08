package com.vuthao.bpmop.base.custom;

import android.content.Context;
import android.util.AttributeSet;
import android.view.KeyEvent;
import android.widget.LinearLayout;

import androidx.annotation.Nullable;

public class CustomResizeLinearLayout extends LinearLayout {
    private boolean isKeyboardShown;
    private KeyboardStateChange listener;

    public interface KeyboardStateChange
    {
        void OnKeyboardShow();
        void OnKeyboardHide();
    }

    public CustomResizeLinearLayout(Context context) {
        super(context);
    }

    public CustomResizeLinearLayout(Context context, @Nullable AttributeSet attrs) {
        super(context, attrs);
    }

    public CustomResizeLinearLayout(Context context, @Nullable AttributeSet attrs, int defStyleAttr) {
        super(context, attrs, defStyleAttr);
    }

    public CustomResizeLinearLayout(Context context, AttributeSet attrs, int defStyleAttr, int defStyleRes) {
        super(context, attrs, defStyleAttr, defStyleRes);
    }

    public void setKeyboardStateListener(KeyboardStateChange listener)
    {
        this.listener = listener;
    }

    @Override
    public boolean dispatchKeyEventPreIme(KeyEvent event) {
        if (event.getKeyCode() == KeyEvent.KEYCODE_BACK) {
            if (isKeyboardShown) {
                isKeyboardShown = false;
                listener.OnKeyboardHide();
            }
        }
        return super.dispatchKeyEventPreIme(event);
    }

    @Override
    protected void onMeasure(int widthMeasureSpec, int heightMeasureSpec) {
        int proposedHeight = MeasureSpec.getSize(heightMeasureSpec);
        int actualHeight = this.getHeight();
        if (actualHeight > proposedHeight) {
            if (!isKeyboardShown) {
                isKeyboardShown = true;
                listener.OnKeyboardShow();
            }
        }
        super.onMeasure(widthMeasureSpec, heightMeasureSpec);
    }
}

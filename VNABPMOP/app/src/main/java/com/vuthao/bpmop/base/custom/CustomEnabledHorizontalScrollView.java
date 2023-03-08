package com.vuthao.bpmop.base.custom;

import android.content.Context;
import android.util.AttributeSet;
import android.view.MotionEvent;
import android.widget.HorizontalScrollView;

public class CustomEnabledHorizontalScrollView extends HorizontalScrollView {
    private boolean enableScrolling = true;

    public boolean isEnableScrolling()
    {
        return enableScrolling;
    }
    public void setEnableScrolling(boolean enableScrolling)
    {
        this.enableScrolling = enableScrolling;
    }

    public CustomEnabledHorizontalScrollView(Context context) {
        super(context);
    }

    public CustomEnabledHorizontalScrollView(Context context, AttributeSet attrs) {
        super(context, attrs);
    }

    public CustomEnabledHorizontalScrollView(Context context, AttributeSet attrs, int defStyleAttr) {
        super(context, attrs, defStyleAttr);
    }

    public CustomEnabledHorizontalScrollView(Context context, AttributeSet attrs, int defStyleAttr, int defStyleRes) {
        super(context, attrs, defStyleAttr, defStyleRes);
    }

    @Override
    public boolean onInterceptHoverEvent(MotionEvent event) {
        if (isEnableScrolling()) {
            return super.onInterceptHoverEvent(event);
        } else {
            return false;
        }
    }

    @Override
    public boolean onTouchEvent(MotionEvent ev) {
        if (isEnableScrolling()) {
            return super.onTouchEvent(ev);
        } else {
            return false;
        }
    }
}

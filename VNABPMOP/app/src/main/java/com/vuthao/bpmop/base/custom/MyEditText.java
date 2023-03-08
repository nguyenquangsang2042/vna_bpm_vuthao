package com.vuthao.bpmop.base.custom;

import android.annotation.SuppressLint;
import android.content.Context;
import android.graphics.Typeface;
import android.text.TextWatcher;
import android.util.AttributeSet;
import android.widget.EditText;

import androidx.core.content.res.ResourcesCompat;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.activity.BaseActivity;

@SuppressLint("AppCompatCustomView")
public class MyEditText extends EditText {
    public MyEditText(Context context) {
        super(context);
        init();
    }

    public MyEditText(Context context, AttributeSet attrs) {
        super(context, attrs);
        init();
    }

    public MyEditText(Context context, AttributeSet attrs, int defStyleAttr) {
        super(context, attrs, defStyleAttr);
        init();
    }

    public MyEditText(Context context, AttributeSet attrs, int defStyleAttr, int defStyleRes) {
        super(context, attrs, defStyleAttr, defStyleRes);
        init();
    }

    @Override
    public void addTextChangedListener(TextWatcher watcher) {
        super.addTextChangedListener(watcher);
    }



    private void init() {
        if (!isInEditMode()) {
            setTypeface(ResourcesCompat.getFont(BaseActivity.sBaseActivity, R.font.fontarial), Typeface.ITALIC);
        } else {
            setTypeface(ResourcesCompat.getFont(BaseActivity.sBaseActivity, R.font.fontarial), Typeface.NORMAL);
        }
    }
}

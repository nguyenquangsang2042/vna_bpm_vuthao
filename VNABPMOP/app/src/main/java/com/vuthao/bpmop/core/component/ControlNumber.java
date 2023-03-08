package com.vuthao.bpmop.core.component;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Intent;
import android.graphics.Typeface;
import android.util.TypedValue;
import android.view.View;
import android.widget.LinearLayout;

import androidx.core.content.ContextCompat;
import androidx.localbroadcastmanager.content.LocalBroadcastManager;

import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.MultipleClickGuard;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.detail.custom.DetailFunc;

public class ControlNumber extends ControlBase {
    private final ViewElement element;

    public ControlNumber(Activity mainAct, LinearLayout parentView, ViewElement element)
    {
        super(mainAct);
        this.mainAct = mainAct;
        this.element = element;
        initializeComponent();
    }

    @Override
    public void setValue(String value) {
        if (!Functions.isNullOrEmpty(value.trim()) && !value.equals("0")) {
            tvValue.setText(DetailFunc.share.getFormatControlDecimal(element));
        } else {
            if (element.isEnable()) {
                tvValue.setText(Functions.share.getTitle("TEXT_CHOOSE_CONTENT", "Chọn nội dung..."));
                tvValue.setTypeface(super.tvValue.getTypeface(), Typeface.ITALIC);
                tvValue.setTextSize(TypedValue.COMPLEX_UNIT_SP, 12);
                tvValue.setVisibility(View.VISIBLE);
            } else {
                tvValue.setVisibility(View.INVISIBLE);
            }
        }
    }

    @Override
    public String getValue() {
        return tvValue.getText().toString();
    }

    @Override
    public void initializeComponent() {
        super.initializeComponent();

        tvValue.setMaxLines(1);
        if (lnContent != null) {
            lnContent.setOnClickListener(new MultipleClickGuard(new View.OnClickListener() {
                @Override
                public void onClick(View view) {
                    handleClick();
                }
            }, 2000));
        }
    }

    public void handleClick() {
        Intent intent = new Intent();
        intent.setAction(VarsReceiver.FORMCLICK);
        intent.putExtra("element", new Gson().toJson(element));
        BroadcastUtility.send(mainAct, intent);
    }
    @Override
    public void initializeFrameView(LinearLayout frame) {
        if (element.isHidden()) return;

        super.initializeFrameView(frame);
    }

    @Override
    public void setProprety() {

    }

    @Override
    public void setEnable() {
        super.setEnable();
        if (element.isEnable()) {
            tvValue.setTextColor(ContextCompat.getColor(mainAct, R.color.clBlueEnable));
        }
    }

    @SuppressLint("SetTextI18n")
    @Override
    public void setTitle() {
        super.setTitle();

        tvTitle.setText(element.getTitle());
        if (element.isRequire() && element.isEnable()) {
            tvTitle.setText(tvTitle.getText() + " (*)");
            Functions.share.setTVHighlightControl(mainAct, tvTitle);
        }
    }

    @Override
    public void setValue() {
        super.setValue();
        setValue(element.getValue());
    }
}

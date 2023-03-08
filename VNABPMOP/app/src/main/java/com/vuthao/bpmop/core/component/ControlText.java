package com.vuthao.bpmop.core.component;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Intent;
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

public class ControlText extends ControlBase {
    private final ViewElement element;

    @Override
    public void setValue(String value) {
        tvValue.setText(value);
    }

    @Override
    public String getValue() {
        return tvValue.getText().toString();
    }

    public ControlText(Activity mainAct, LinearLayout parentView, ViewElement element) {
        super(mainAct);
        this.mainAct = mainAct;
        this.element = element;
        initializeComponent();
    }

    @Override
    public void initializeComponent() {
        super.initializeComponent();

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
        if (element.isHidden()) // Check xem có ẩn view hay không
            return;
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

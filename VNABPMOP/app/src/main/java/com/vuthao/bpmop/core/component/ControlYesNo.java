package com.vuthao.bpmop.core.component;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Intent;
import android.util.TypedValue;
import android.view.View;
import android.widget.CompoundButton;
import android.widget.LinearLayout;
import android.widget.Switch;

import androidx.localbroadcastmanager.content.LocalBroadcastManager;

import com.google.gson.Gson;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.vars.VarsReceiver;

import org.json.JSONObject;

public class ControlYesNo extends ControlBase implements CompoundButton.OnCheckedChangeListener {
    private final LinearLayout parentView;
    private final ViewElement element;
    private Switch switchYesNo;
    // Input grid Details
    private ViewElement elementParent; // Element của Control Grid
    private ViewElement elementChild; // Element child của Item dc click vào
    private int flagView;
    private JSONObject JObjectChild;

    public ControlYesNo(Activity mainAct, LinearLayout parentView, ViewElement element) {
        super(mainAct);
        this.parentView = parentView;
        this.element = element;
        initializeComponent();
    }

    /// <summary>
    /// Constructor của Input Grid Details
    /// </summary>
    public ControlYesNo(Activity mainAct, LinearLayout parentView, ViewElement elementParent, ViewElement elementChild, JSONObject JObjectChild, int flagView) {
        super(mainAct);
        // Để render theo base
        this.parentView = parentView;
        this.element = elementChild;

        // Input grid Details
        this.elementParent = elementParent;
        this.elementChild = elementChild;
        this.JObjectChild = JObjectChild;
        this.flagView = flagView;
        initializeComponent();
    }

    @Override
    public void setValue(String value) {
        if (element.isEnable()) {
            switchYesNo.setChecked(value.toLowerCase().equals("true"));
        } else {
            if (value.toLowerCase().equals("true")) {
                tvValue.setText(Functions.share.getTitle("TEXT_LABEL_YES", "Có"));
            } else {
                tvValue.setText(Functions.share.getTitle("TEXT_LABEL_NO", "Không"));
            }
        }
    }

    @Override
    public String getValue() {
        return switchYesNo.getText().toString();
    }

    @Override
    public void initializeComponent() {
        super.initializeComponent();

        switchYesNo = new Switch(mainAct);
    }

    @Override
    public void initializeFrameView(LinearLayout frame) {
        if (element.isHidden()) return;
        super.initializeFrameView(frame);

        if (element.isEnable()) {
            tvValue.setVisibility(View.GONE);

            int padding = (int) TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, 6, mainAct.getResources().getDisplayMetrics());
            LinearLayout.LayoutParams _switchParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WRAP_CONTENT, LinearLayout.LayoutParams.WRAP_CONTENT);
            switchYesNo.setPadding(0, padding, padding, padding);
            switchYesNo.setLayoutParams(_switchParams);

            switchYesNo.setOnCheckedChangeListener(this);

            lnContent.addView(switchYesNo);
        } else {
            tvValue.setVisibility(View.VISIBLE);
        }
    }

    @Override
    public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
        if (isChecked) {
            element.setValue("True");
        } else {
            element.setValue("False");
        }

        if (parentView != null) {
            Intent intent = new Intent();
            if (JObjectChild != null) {
                intent.setAction(VarsReceiver.CHILDACTION);
                intent.putExtra("elementParent", new Gson().toJson(elementParent));
                intent.putExtra("elementChild", new Gson().toJson(elementChild));
                intent.putExtra("json", JObjectChild.toString());
                intent.putExtra("flagview", flagView);
            } else {
                intent.setAction("FORMCLICK");
                intent.putExtra("element", new Gson().toJson(element));
            }
            BroadcastUtility.send(mainAct, intent);
        }
    }

    @Override
    public void setProprety() {
        super.setProprety();

    }

    @Override
    public void setEnable() {
        super.setEnable();

        if (element.isEnable()) {
            switchYesNo.setClickable(true);
            parentView.setClickable(true);
        } else {
            switchYesNo.setClickable(false);
            parentView.setClickable(false);
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

        if (element.isEnable()) {
            switchYesNo.setOnCheckedChangeListener(null);
        }
        setValue(element.getValue());

        if (element.isEnable()) {
            switchYesNo.setOnCheckedChangeListener(this);
        }
    }
}

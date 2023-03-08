package com.vuthao.bpmop.core.component;

import android.app.Activity;
import android.content.Intent;
import android.widget.LinearLayout;

import androidx.localbroadcastmanager.content.LocalBroadcastManager;

import com.google.gson.Gson;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.vars.VarsReceiver;

import org.json.JSONObject;

public class TemplateValueType_ControlNumber extends ControlNumber {
    private final ViewElement elementParent; // Element của Control Grid
    private final ViewElement elementChild; // Element child của Item dc click vào
    private final int flagView;
    private final JSONObject JObjectChild;
    private final LinearLayout parentView;

    public TemplateValueType_ControlNumber(Activity mainAct, LinearLayout parentView, ViewElement elementParent, ViewElement elementChild, JSONObject JObjectChild, int flagView) {
        super(mainAct, parentView, elementChild);
        this.elementParent = elementParent;
        this.elementChild = elementChild;
        this.JObjectChild = JObjectChild;
        this.parentView = parentView;
        this.flagView = flagView;
    }

    @Override
    public void setFrame(LinearLayout frame) {
        super.setFrame(frame);
    }

    @Override
    public LinearLayout getFrame() {
        return super.getFrame();
    }

    @Override
    public void initializeComponent() {
        super.initializeComponent();
    }

    @Override
    public void initializeFrameView(LinearLayout frame) {
        super.initializeFrameView(frame);
    }

    @Override
    public void setEnable() {
        super.setEnable();
    }

    @Override
    public void setProprety() {
        super.setProprety();
    }

    @Override
    public void setTitle() {
        super.setTitle();
    }

    @Override
    public void setValue() {
        super.setValue();
    }

    @Override
    public void handleClick() {
        if (parentView != null) {
            Intent intent = new Intent();
            intent.setAction(VarsReceiver.CHILDACTION);
            intent.putExtra("elementParent", new Gson().toJson(elementParent));
            intent.putExtra("elementChild", new Gson().toJson(elementChild));
            intent.putExtra("json", JObjectChild.toString());
            intent.putExtra("flagview", flagView);
            BroadcastUtility.send(mainAct, intent);
        }
    }
}

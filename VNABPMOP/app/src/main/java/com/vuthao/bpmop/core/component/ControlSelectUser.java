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
import com.google.gson.reflect.TypeToken;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.MultipleClickGuard;
import com.vuthao.bpmop.base.model.custom.UserAndGroup;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.vars.VarsReceiver;

import org.json.JSONObject;

import java.util.ArrayList;

public class ControlSelectUser extends ControlBase {
    private ViewElement element;
    // Input grid Details
    public ViewElement elementParent; // Element của Control Grid
    public ViewElement elementChild; // Element child của Item dc click vào
    public int flagView;
    public JSONObject JObjectChild;

    public ControlSelectUser(Activity mainAct) {
        super(mainAct);
    }

    @Override
    public void setValue(String value) {
        String data = value.trim();
        String result = "";

        ArrayList<UserAndGroup> userAndGroups = new Gson().fromJson(data, new TypeToken<ArrayList<UserAndGroup>>() {
        }.getType());

        if (userAndGroups != null && userAndGroups.size() > 0) {
            result = userAndGroups.get(0).getName();
            this.tvValue.setText(result);
        } else {
            if (element.isEnable()) {
                if (element.getDataType().equals("selectuser")) {
                    tvValue.setText(Functions.share.getTitle("TEXT_CONTROL_CHOOSE_USERS", "ChỌn người..."));
                } else {
                    tvValue.setText(Functions.share.getTitle("TEXT_CONTROL_CHOOSE_USERS", "ChỌn người hoặc nhóm..."));
                }

                tvValue.setTypeface(tvValue.getTypeface(), Typeface.ITALIC);
                tvValue.setTextSize(TypedValue.COMPLEX_UNIT_SP, 12);
            }
        }
    }

    @Override
    public String getValue() {
        return tvValue.getText().toString();
    }

    public ControlSelectUser(Activity mainAct, LinearLayout parentView, ViewElement element)
    {
        super(mainAct);
        this.mainAct = mainAct;
        this.element = element;
        initializeComponent();
    }

    public ControlSelectUser(Activity mainAct, LinearLayout parentView, ViewElement elementParent, ViewElement elementChild, JSONObject JObjectChild, int flagView)
    {
        super(mainAct);
        // Để render theo base
        this.element = elementChild;
        // Input grid Details
        this.elementParent = elementParent;
        this.elementChild = elementChild;
        this.JObjectChild = JObjectChild;
        this.flagView = flagView;
        initializeComponent();
    }

    @Override
    public void initializeComponent() {
        super.initializeComponent();
        if (lnContent != null) {
            lnContent.setOnClickListener(new MultipleClickGuard(new View.OnClickListener() {
                @Override
                public void onClick(View view) {
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
            }, 2000));
        }
    }

    @Override
    public void initializeFrameView(LinearLayout frame) {
        if (element.isHidden()) return;
        super.initializeFrameView(frame);
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

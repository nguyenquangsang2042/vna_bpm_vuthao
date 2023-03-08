package com.vuthao.bpmop.shareview;

import android.app.Activity;
import android.app.Dialog;
import android.content.Intent;
import android.text.Editable;
import android.text.InputFilter;
import android.text.InputType;
import android.text.TextWatcher;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.localbroadcastmanager.content.LocalBroadcastManager;

import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.keyboard.KeyboardManager;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.core.Vars;
import com.vuthao.bpmop.detail.custom.DetailFunc;

import org.json.JSONObject;

import java.text.DecimalFormat;

public class SharedView_PopupControlNumber extends SharedView_PopupControlBase implements TextWatcher, View.OnClickListener {
    private Dialog dialog;
    private View view;
    private ImageView imgBack;
    private TextView tvTitle;
    private ImageView imgDone;
    private ImageView imgDelete;
    private EditText edtContent;
    private ImageView imgClearText;

    public SharedView_PopupControlNumber(LayoutInflater inflater, Activity mainAct, String fragmentTag, View rootView) {
        super(inflater, mainAct, fragmentTag, rootView);
    }

    @Override
    public void initializeValue_Master(ViewElement viewElement) {
        super.initializeValue_Master(viewElement);
    }

    @Override
    public void initializeValue_InputGridDetail(ViewElement elementParent, ViewElement elementPopup, JSONObject JObjectChild) {
        super.initializeValue_InputGridDetail(elementParent, elementPopup, JObjectChild);
    }

    @Override
    public void initializeView() {
        super.initializeView();

        view = inflater.inflate(R.layout.control_popup_number, null);
        imgBack = view.findViewById(R.id.img_PopupControl_Number_Close);
        tvTitle = view.findViewById(R.id.tv_PopupControl_Number_Title);
        imgDone = view.findViewById(R.id.img_PopupControl_Number_Done);
        imgDelete = view.findViewById(R.id.img_PopupControl_Number_Delete);
        edtContent = view.findViewById(R.id.edt_PopupControl_Number);
        imgClearText = view.findViewById(R.id.img_PopupControl_Number_ClearText);

        imgDelete.setVisibility(View.GONE);
        edtContent.setInputType(InputType.TYPE_CLASS_NUMBER | InputType.TYPE_NUMBER_FLAG_DECIMAL);
        edtContent.setFilters(new InputFilter[]{new InputFilter.LengthFilter(255)});

        switch (flagView) {
            case Vars.FlagViewControlDynamic.DetailWorkflow:
            default: {
                tvTitle.setText(elementParent.getTitle());

                if (!Functions.isNullOrEmpty(elementParent.getValue())) {
                    long l = Long.parseLong(elementParent.getValue());
                    DecimalFormat df = new DecimalFormat("0");
                    df.setMaximumFractionDigits(Integer.MAX_VALUE);
                    edtContent.setText(df.format(l));
                } else {
                    edtContent.setText("");
                }

                break;
            }
            case Vars.FlagViewControlDynamic.DetailWorkflow_InputGridDetail: {
                tvTitle.setText(elementPopup.getTitle());

                if (!Functions.isNullOrEmpty(elementPopup.getValue())) {
                    long l = Long.parseLong(elementPopup.getValue());
                    DecimalFormat df = new DecimalFormat("0");
                    df.setMaximumFractionDigits(Integer.MAX_VALUE);
                    edtContent.setText(df.format(l));
                } else {
                    edtContent.setText("");
                }
                break;
            }
        }

        // region Dialog
        dialog = new Dialog(mainAct, R.style.Theme_Custom_BPMOP_Dialog_FullScreen);
        Window window = dialog.getWindow();
        dialog.requestWindowFeature(1);
        dialog.setCanceledOnTouchOutside(false);
        dialog.setCancelable(true);
        window.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_ADJUST_PAN);
        window.setGravity(Gravity.BOTTOM);
        dialog.setContentView(view);
        dialog.show();

        WindowManager.LayoutParams s = window.getAttributes();
        s.width = WindowManager.LayoutParams.MATCH_PARENT;
        s.height = WindowManager.LayoutParams.MATCH_PARENT;
        window.setAttributes(s);
        // endregion

        imgBack.setOnClickListener(this);
        edtContent.addTextChangedListener(this);
        imgClearText.setOnClickListener(this);
        imgDelete.setOnClickListener(this);
        imgDone.setOnClickListener(this);

        edtContent.setText(edtContent.getText().toString());
        edtContent.requestFocus();
        KeyboardManager.showKeyBoard(edtContent, mainAct);
    }

    private void done() {
        KeyboardManager.hideKeyboard(edtContent, mainAct);
        String result = "";

        switch (flagView) {
            case Vars.FlagViewControlDynamic.DetailWorkflow:
            default: {
                if (!Functions.isNullOrEmpty(edtContent.getText().toString())) {
                    double d = Double.parseDouble(edtContent.getText().toString().trim());
                    result = DetailFunc.share.getFormatControlDecimal(d, elementParent);
                }

                Intent intent = new Intent();
                intent.setAction(VarsReceiver.UPDATEFORM);
                intent.putExtra("element", new Gson().toJson(elementParent));
                intent.putExtra("newValue", result.replaceAll("\\,", ""));
                BroadcastUtility.send(mainAct, intent);
                break;
            }
            case Vars.FlagViewControlDynamic.DetailWorkflow_InputGridDetail: {
                if (!Functions.isNullOrEmpty(edtContent.getText().toString())) {
                    double d = Double.parseDouble(edtContent.getText().toString().trim());
                    result = DetailFunc.share.getFormatControlDecimal(d, elementPopup);
                }

                Intent intent = new Intent();
                intent.setAction(VarsReceiver.UPDATECHILDACTION);
                intent.putExtra("elementParent", new Gson().toJson(elementParent));
                intent.putExtra("elementChild", new Gson().toJson(elementPopup));
                intent.putExtra("json", JObjectChild.toString());
                intent.putExtra("newValue", result.trim().replaceAll("\\,", ""));
                BroadcastUtility.send(mainAct, intent);
                break;
            }
        }

        dialog.dismiss();
    }

    @Override
    public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {

    }

    @Override
    public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {

    }

    @Override
    public void afterTextChanged(Editable editable) {

    }

    @Override
    public void onClick(View view) {
        switch (view.getId()) {
            case R.id.img_PopupControl_Number_Close: {
                KeyboardManager.hideKeyboard(edtContent, mainAct);
                dialog.dismiss();
                break;
            }
            case R.id.img_PopupControl_Number_ClearText: {
                edtContent.setText("");
                break;
            }
            case R.id.img_PopupControl_Number_Delete: {
                edtContent.setText("");
                imgDone.performClick();
                break;
            }
            case R.id.img_PopupControl_Number_Done: {
                done();
                break;
            }
        }
    }
}

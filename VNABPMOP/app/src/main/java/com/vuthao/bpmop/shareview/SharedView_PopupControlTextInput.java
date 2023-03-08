package com.vuthao.bpmop.shareview;

import android.app.Activity;
import android.app.Dialog;
import android.content.Intent;
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

public class SharedView_PopupControlTextInput extends SharedView_PopupControlBase implements View.OnClickListener {
    private Dialog dialog;
    private View view;
    private ImageView imgBack;
    private TextView tvTitle;
    private ImageView imgDone;
    private ImageView imgDelete;
    private EditText edtContent;
    private ImageView imgClearText;

    public SharedView_PopupControlTextInput(LayoutInflater inflater, Activity mainAct, String fragmentTag, View rootView) {
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

        view = inflater.inflate(R.layout.popup_control_input_text, null);
        imgBack = view.findViewById(R.id.img_PopupControl_InputText_Close);
        tvTitle = view.findViewById(R.id.tv_PopupControl_InputText_Title);
        imgDone = view.findViewById(R.id.img_PopupControl_InputText_Done);
        imgDelete = view.findViewById(R.id.img_PopupControl_InputText_Delete);
        edtContent = view.findViewById(R.id.edt_PopupControl_InputText);
        imgClearText = view.findViewById(R.id.img_PopupControl_InputText_ClearText);

        imgDelete.setVisibility(View.GONE);

        switch (flagView) {
            case Vars.FlagViewControlDynamic.DetailWorkflow:
            default: {
                tvTitle.setText(elementParent.getTitle());
                if (!Functions.isNullOrEmpty(elementParent.getValue())) {
                    edtContent.setText(DetailFunc.share.formatHTMLToString(elementParent.getValue()));
                } else {
                    edtContent.setText("");
                }
                break;
            }
            case Vars.FlagViewControlDynamic.DetailWorkflow_InputGridDetail: {
                tvTitle.setText(elementPopup.getTitle());
                if (!Functions.isNullOrEmpty(elementPopup.getValue())) {
                    edtContent.setText(DetailFunc.share.formatHTMLToString(elementPopup.getValue()));
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
        imgClearText.setOnClickListener(this);
        imgDelete.setOnClickListener(this);
        imgDone.setOnClickListener(this);

        edtContent.requestFocus();
        KeyboardManager.showKeyBoard(edtContent, mainAct);
    }

    private void done() {
        KeyboardManager.hideKeyboard(edtContent, mainAct);
        switch (flagView) {
            case Vars.FlagViewControlDynamic.DetailWorkflow:
            default: {
                Intent intent = new Intent();
                intent.setAction(VarsReceiver.UPDATEFORM);
                intent.putExtra("element", new Gson().toJson(elementParent));
                intent.putExtra("newValue", edtContent.getText().toString().length() > 0 ? edtContent.getText().toString() : "");
                BroadcastUtility.send(mainAct, intent);
                break;
            }
            case Vars.FlagViewControlDynamic.DetailWorkflow_InputGridDetail: {
                Intent intent = new Intent();
                intent.setAction(VarsReceiver.UPDATECHILDACTION);
                intent.putExtra("elementParent", new Gson().toJson(elementParent));
                intent.putExtra("elementChild", new Gson().toJson(elementPopup));
                intent.putExtra("json", JObjectChild.toString());
                intent.putExtra("newValue", edtContent.getText().toString().length() > 0 ? edtContent.getText().toString() : "");
                BroadcastUtility.send(mainAct, intent);
                break;
            }
        }

        dialog.dismiss();
    }
    @Override
    public void onClick(View view) {
        switch (view.getId()) {
            case R.id.img_PopupControl_InputText_Close: {
                KeyboardManager.hideKeyboard(edtContent, mainAct);
                dialog.dismiss();
                break;
            }
            case R.id.img_PopupControl_InputText_ClearText: {
                edtContent.setText("");
                break;
            }
            case R.id.img_PopupControl_InputText_Delete: {
                edtContent.setText("");
                imgDone.performClick();
                break;
            }
            case R.id.img_PopupControl_InputText_Done: {
                done();
                break;
            }
        }
    }
}

package com.vuthao.bpmop.shareview;

import android.app.Activity;
import android.app.Dialog;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.keyboard.KeyboardManager;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.core.Vars;
import com.vuthao.bpmop.detail.custom.DetailFunc;

import org.json.JSONObject;

public class SharedView_PopupControlViewFullInfo extends SharedView_PopupControlBase {
    public SharedView_PopupControlViewFullInfo(LayoutInflater inflater, Activity mainAct, String fragmentTag, View rootView) {
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

        View view = inflater.inflate(R.layout.popup_control_input_text, null);
        ImageView imgBack = view.findViewById(R.id.img_PopupControl_InputText_Close);
        TextView tvTitle = view.findViewById(R.id.tv_PopupControl_InputText_Title);
        ImageView imgDone = view.findViewById(R.id.img_PopupControl_InputText_Done);
        ImageView imgDelete = view.findViewById(R.id.img_PopupControl_InputText_Delete);
        EditText edtContent = view.findViewById(R.id.edt_PopupControl_InputText);
        LinearLayout lnEdt = view.findViewById(R.id.ln_PopupControl_InputText);

        LinearLayout.LayoutParams paramsEdt = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.MATCH_PARENT);
        paramsEdt.setMargins(15,15,15,15);
        lnEdt.setLayoutParams(paramsEdt);

        imgDelete.setVisibility(View.GONE);
        imgDone.setVisibility(View.INVISIBLE);
        edtContent.setFocusable(false);

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
        Dialog dialog = new Dialog(mainAct, R.style.Theme_Custom_BPMOP_Dialog_FullScreen);
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

        imgBack.setOnClickListener(v -> {
            KeyboardManager.hideKeyboard(edtContent, mainAct);
            dialog.dismiss();
        });
    }
}

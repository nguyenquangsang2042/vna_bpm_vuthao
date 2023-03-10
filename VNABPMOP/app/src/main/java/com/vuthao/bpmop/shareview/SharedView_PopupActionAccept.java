package com.vuthao.bpmop.shareview;

import android.app.Activity;
import android.app.Dialog;
import android.content.Intent;
import android.graphics.Color;
import android.graphics.Typeface;
import android.graphics.drawable.ColorDrawable;
import android.text.Editable;
import android.text.TextWatcher;
import android.util.DisplayMetrics;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.core.content.res.ResourcesCompat;
import androidx.localbroadcastmanager.content.LocalBroadcastManager;

import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.keyboard.KeyboardManager;
import com.vuthao.bpmop.base.model.custom.ButtonAction;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.core.Vars;

public class SharedView_PopupActionAccept extends SharedView_PopupActionBase implements TextWatcher, View.OnClickListener {
    private Dialog dialog;
    private View view;
    private TextView tvTitle;
    private ImageView imgAction;
    private EditText edtComment;
    private TextView tvCancel;
    private TextView tvAccept;
    private TextView tvNote;

    public SharedView_PopupActionAccept(LayoutInflater inflater, Activity mainAct, String fragmentTag, View rootView) {
        super(inflater, mainAct, fragmentTag, rootView);
    }

    @Override
    public void initializeValue_DetailWorkflow(ButtonAction buttonAction) {
        super.initializeValue_DetailWorkflow(buttonAction);
    }

    @Override
    public void initializeView() {
        super.initializeView();

        view = inflater.inflate(R.layout.popup_action_accept, null);
        tvTitle = view.findViewById(R.id.tv_PopupAction_Accept_Title);
        imgAction = view.findViewById(R.id.img_PopupAction_Accept);
        edtComment = view.findViewById(R.id.edt_PopupAction_Accept_YKien);
        tvCancel = view.findViewById(R.id.tv_PopupAction_Accept_Huy);
        tvAccept = view.findViewById(R.id.tv_PopupAction_Accept_HoanTat);
        tvNote = view.findViewById(R.id.tv_PopupAction_Accept_Note);

        tvTitle.setText(buttonAction.getTitle());
        edtComment.setHint(Functions.share.getTitle("MESS_REQUIRE_COMMENT", "Vui l??ng nh???p ?? ki???n"));
        tvCancel.setText(Functions.share.getTitle("TEXT_EXIT", "Tho??t"));
        tvAccept.setText(buttonAction.getTitle());

        String imageName = "icon_bpm_Btn_action_" + buttonAction.getID();
        int resId = mainAct.getResources().getIdentifier(imageName.toLowerCase(), "drawable", mainAct.getPackageName());
        imgAction.setImageResource(resId);

        if (buttonAction.getNotes() != null && buttonAction.getNotes().size() > 0) {
            tvNote.setText(buttonAction.getNotes().get(0).getValue());
        }

        // region Dialog
        dialog = new Dialog(mainAct);
        Window window = dialog.getWindow();
        dialog.requestWindowFeature(1);
        dialog.setCancelable(true);
        dialog.setCanceledOnTouchOutside(false);
        window.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_ADJUST_PAN);
        window.setGravity(Gravity.CENTER);

        DisplayMetrics displayMetrics = mainAct.getResources().getDisplayMetrics();
        dialog.setContentView(view);
        dialog.show();

        WindowManager.LayoutParams s = window.getAttributes();
        s.width = displayMetrics.widthPixels;
        window.setAttributes(s);
        window.setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));
        // endregion

        tvCancel.setOnClickListener(this);
        edtComment.addTextChangedListener(this);
        tvAccept.setOnClickListener(this);
        edtComment.setText("");
    }

    @Override
    public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {

    }

    @Override
    public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {

    }

    @Override
    public void afterTextChanged(Editable s) {
        if (s.length() > 0) {
            edtComment.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.ITALIC);
        } else {
            edtComment.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.NORMAL);
        }
    }

    @Override
    public void onClick(View view) {
        switch (view.getId()) {
            case R.id.tv_PopupAction_Accept_Huy: {
                KeyboardManager.hideKeyboard(edtComment, mainAct);
                dialog.dismiss();
                break;
            }
            case R.id.tv_PopupAction_Accept_HoanTat: {
                Intent intent = new Intent();
                intent.setAction(VarsReceiver.SUBMITACTION);
                intent.putExtra("buttonAction", new Gson().toJson(buttonAction));
                intent.putExtra("comment", edtComment.getText().toString());
                BroadcastUtility.send(mainAct, intent);
                break;
            }
        }
    }
}

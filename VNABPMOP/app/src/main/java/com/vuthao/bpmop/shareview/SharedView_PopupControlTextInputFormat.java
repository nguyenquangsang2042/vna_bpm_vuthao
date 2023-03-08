package com.vuthao.bpmop.shareview;

import android.app.Activity;
import android.app.Dialog;
import android.content.Intent;
import android.graphics.Color;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.localbroadcastmanager.content.LocalBroadcastManager;

import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.custom.editor.RichEditor;
import com.vuthao.bpmop.base.keyboard.KeyboardManager;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.core.Vars;

import org.json.JSONObject;

public class SharedView_PopupControlTextInputFormat extends SharedView_PopupControlBase implements View.OnClickListener {
    private boolean isChange;
    private String result;

    private Dialog dialog;
    private View view;
    private LinearLayout lnContentClick;
    private ImageView imgBack;
    private TextView tvTitle;
    private ImageView imgDone;
    private ImageView imgDelete;
    private RichEditor mEditor;

    public SharedView_PopupControlTextInputFormat(LayoutInflater inflater, Activity mainAct, String fragmentTag, View rootView) {
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

        view = inflater.inflate(R.layout.popup_control_text_input_format, null);
        lnContentClick = view.findViewById(R.id.ln_PopupControl_InputTextFormat_ContentClick);
        imgBack = view.findViewById(R.id.img_PopupControl_InputTextFormat_Close);
        tvTitle = view.findViewById(R.id.tv_PopupControl_InputTextFormat_Title);
        imgDone = view.findViewById(R.id.img_PopupControl_InputTextFormat_Done);
        imgDelete = view.findViewById(R.id.img_PopupControl_InputTextFormat_Delete);
        mEditor = view.findViewById(R.id.editor_PopupControl_InputTextFormat);

        ImageView undo = view.findViewById(R.id.img_PopupControl_InputTextFormat_undo);
        ImageView redo = view.findViewById(R.id.img_PopupControl_InputTextFormat_redo);
        ImageView bold = view.findViewById(R.id.img_PopupControl_InputTextFormat_bold);
        ImageView italic = view.findViewById(R.id.img_PopupControl_InputTextFormat_italic);
        ImageView subscript = view.findViewById(R.id.img_PopupControl_InputTextFormat_subscript);
        ImageView superscript = view.findViewById(R.id.img_PopupControl_InputTextFormat_superscript);
        ImageView strikethrough = view.findViewById(R.id.img_PopupControl_InputTextFormat_strikethrough);
        ImageView underline = view.findViewById(R.id.img_PopupControl_InputTextFormat_underline);
        ImageView heading1 = view.findViewById(R.id.img_PopupControl_InputTextFormat_heading1);
        ImageView heading2 = view.findViewById(R.id.img_PopupControl_InputTextFormat_heading2);
        ImageView heading3 = view.findViewById(R.id.img_PopupControl_InputTextFormat_heading3);
        ImageView heading4 = view.findViewById(R.id.img_PopupControl_InputTextFormat_heading4);
        ImageView heading5 = view.findViewById(R.id.img_PopupControl_InputTextFormat_heading5);
        ImageView heading6 = view.findViewById(R.id.img_PopupControl_InputTextFormat_heading6);
        ImageView txt_color = view.findViewById(R.id.img_PopupControl_InputTextFormat_txt_color);
        ImageView bg_color = view.findViewById(R.id.img_PopupControl_InputTextFormat_bg_color);
        ImageView indent = view.findViewById(R.id.img_PopupControl_InputTextFormat_indent);
        ImageView outdent = view.findViewById(R.id.img_PopupControl_InputTextFormat_outdent);
        ImageView left = view.findViewById(R.id.img_PopupControl_InputTextFormat_align_left);
        ImageView center = view.findViewById(R.id.img_PopupControl_InputTextFormat_align_center);
        ImageView right = view.findViewById(R.id.img_PopupControl_InputTextFormat_align_right);
        ImageView blockquote = view.findViewById(R.id.img_PopupControl_InputTextFormat_blockquote);
        ImageView bullets = view.findViewById(R.id.img_PopupControl_InputTextFormat_insert_bullets);
        ImageView numbers = view.findViewById(R.id.img_PopupControl_InputTextFormat_insert_numbers);
        ImageView image = view.findViewById(R.id.img_PopupControl_InputTextFormat_insert_image);
        ImageView link = view.findViewById(R.id.img_PopupControl_InputTextFormat_insert_link);
        ImageView checkbox = view.findViewById(R.id.img_PopupControl_InputTextFormat_insert_checkbox);

        imgDelete.setVisibility(View.GONE);
        lnContentClick.setVisibility(View.GONE);

        switch (flagView) {
            case Vars.FlagViewControlDynamic.DetailWorkflow:
            default: {
                tvTitle.setText(elementParent.getTitle());
                if (!Functions.isNullOrEmpty(elementParent.getValue())) {
                    mEditor.setHtml(elementParent.getValue());
                }
                break;
            }
            case Vars.FlagViewControlDynamic.DetailWorkflow_InputGridDetail: {
                tvTitle.setText(elementPopup.getTitle());
                if (!Functions.isNullOrEmpty(elementPopup.getValue())) {
                    mEditor.setHtml(elementPopup.getValue());
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

        // region Editor
        undo.setOnClickListener(v -> mEditor.undo());
        underline.setOnClickListener(v -> mEditor.setUnderline());
        redo.setOnClickListener(v -> mEditor.redo());
        bold.setOnClickListener(v -> mEditor.setBold());
        italic.setOnClickListener(v -> mEditor.setItalic());
        subscript.setOnClickListener(v -> mEditor.setSubscript());
        strikethrough.setOnClickListener(v -> mEditor.setStrikeThrough());
        superscript.setOnClickListener(v -> mEditor.setSuperscript());
        heading1.setOnClickListener(v -> mEditor.setHeading(1));
        heading2.setOnClickListener(v -> mEditor.setHeading(2));
        heading3.setOnClickListener(v -> mEditor.setHeading(3));
        heading4.setOnClickListener(v -> mEditor.setHeading(4));
        heading5.setOnClickListener(v -> mEditor.setHeading(5));
        heading6.setOnClickListener(v -> mEditor.setHeading(6));
        txt_color.setOnClickListener(v -> {
            mEditor.setTextColor(isChange ? Color.BLACK : Color.RED);
            isChange = !isChange;
        });
        bg_color.setOnClickListener(v -> {
            mEditor.setBackgroundColor(isChange ? Color.TRANSPARENT : Color.YELLOW);
            isChange = !isChange;
        });
        indent.setOnClickListener(v -> mEditor.setIndent());
        outdent.setOnClickListener(v -> mEditor.setOutdent());
        left.setOnClickListener(v -> mEditor.setAlignLeft());
        center.setOnClickListener(v -> mEditor.setAlignCenter());
        right.setOnClickListener(v -> mEditor.setAlignRight());
        blockquote.setOnClickListener(v -> mEditor.setBlockquote());
        bullets.setOnClickListener(v -> mEditor.setBullets());
        numbers.setOnClickListener(v -> mEditor.setNumbers());
        image.setOnClickListener(v -> mEditor.insertImage("http://www.1honeywan.com/dachshund/image/7.21/7.21_3_thumb.JPG", "dachshund"));
        link.setOnClickListener(v -> mEditor.insertLink("https://github.com/wasabeef", "wasabeef"));
        checkbox.setOnClickListener(v -> mEditor.insertTodo());

        //endregion

        imgBack.setOnClickListener(this);
        imgDelete.setOnClickListener(this);
        imgDone.setOnClickListener(this);

        KeyboardManager.showKeyBoard(mEditor, mainAct);
    }

    private void done() {
        KeyboardManager.hideKeyboard(mainAct);
        if (!Functions.isNullOrEmpty(mEditor.getHtml())) {
            result = mEditor.getHtml();
        }

        switch (flagView) {
            case Vars.FlagViewControlDynamic.DetailWorkflow: {
                Intent intent = new Intent();
                intent.setAction(VarsReceiver.UPDATEFORM);
                intent.putExtra("element", new Gson().toJson(elementParent));
                intent.putExtra("newValue", result);
                BroadcastUtility.send(mainAct, intent);
                break;
            }
            case Vars.FlagViewControlDynamic.DetailWorkflow_InputGridDetail: {
                Intent intent = new Intent();
                intent.setAction(VarsReceiver.UPDATECHILDACTION);
                intent.putExtra("elementParent", new Gson().toJson(elementParent));
                intent.putExtra("elementChild", new Gson().toJson(elementPopup));
                intent.putExtra("json", JObjectChild.toString());
                intent.putExtra("newValue", result);
                BroadcastUtility.send(mainAct, intent);
                break;
            }
        }

        dialog.dismiss();
    }

    @Override
    public void onClick(View view) {
        switch (view.getId()) {
            case R.id.img_PopupControl_InputTextFormat_Close: {
                KeyboardManager.hideKeyboard(mEditor, mainAct);
                dialog.dismiss();
                break;
            }
            case R.id.img_PopupControl_InputTextFormat_Delete: {
                mEditor.setHtml("");
                break;
            }
            case R.id.img_PopupControl_InputTextFormat_Done: {
                done();
                break;
            }
        }
    }
}

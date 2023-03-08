package com.vuthao.bpmop.core.component;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Intent;
import android.graphics.Typeface;
import android.text.TextUtils;
import android.util.TypedValue;
import android.view.Gravity;
import android.view.View;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;
import android.widget.TextView;

import androidx.core.content.ContextCompat;
import androidx.core.content.res.ResourcesCompat;
import androidx.localbroadcastmanager.content.LocalBroadcastManager;

import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.MultipleClickGuard;
import com.vuthao.bpmop.base.custom.editor.RichEditor;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.detail.custom.DetailFunc;

public class ControlTextInputFormat extends ControlBase {
    private final ViewElement element;
    private final LinearLayout parentView;
    private RelativeLayout relaEditor;
    private LinearLayout lnEditor; // lớp đè lên rich Editor
    private TextView tvReadMore;
    private EditText edt;
    private RichEditor richEditor;

    @Override
    public void setValue(String value) {
        String data = value.trim();
        edt.setText(DetailFunc.share.formatHTMLToString(data));

        if (element.isEnable()) {
            if (Functions.isNullOrEmpty(data)) {
                tvValue.setVisibility(View.VISIBLE);
                tvReadMore.setVisibility(View.GONE);
                edt.setVisibility(View.GONE);

                tvValue.setTypeface(super.tvValue.getTypeface(), Typeface.ITALIC);
                tvValue.setTextSize(TypedValue.COMPLEX_UNIT_SP, 12);
                tvValue.setVisibility(View.VISIBLE);

                tvValue.setText(Functions.share.getTitle("TEXT_CHOOSE_CONTENT", "Chọn nội dung..."));
            } else {
                tvValue.setVisibility(View.GONE);
                edt.setVisibility(View.VISIBLE);

                tvReadMore.post(() -> {
                    int line = edt.getLineCount();
                    if (line > 5) {
                        tvReadMore.setVisibility(View.VISIBLE);
                    } else {
                        tvReadMore.setVisibility(View.GONE);
                    }
                });
            }

        } else {
            if (!Functions.isNullOrEmpty(data)) {
                tvValue.setVisibility(View.GONE);

                tvReadMore.post(() -> {
                    int line = edt.getLineCount();
                    if (line > 5) {
                        tvReadMore.setVisibility(View.VISIBLE);
                    } else {
                        tvReadMore.setVisibility(View.GONE);
                    }
                });
            }
        }
    }

    @Override
    public String getValue() {
        return richEditor.getHtml();
    }

    public ControlTextInputFormat(Activity mainAct, LinearLayout parentView, ViewElement element) {
        super(mainAct);
        this.mainAct = mainAct;
        this.parentView = parentView;
        this.element = element;
        initializeComponent();
    }

    @Override
    public void initializeComponent() {
        super.initializeComponent();

        edt = new EditText(mainAct);
        relaEditor = new RelativeLayout(mainAct);
        lnEditor = new LinearLayout(mainAct);
        richEditor = new RichEditor(mainAct);
        tvReadMore = new TextView(mainAct);

        edt.setMaxLines(3);
        edt.setEllipsize(TextUtils.TruncateAt.END);
        edt.setGravity(Gravity.TOP);
        edt.setTextColor(ContextCompat.getColor(mainAct, R.color.clBlack));
        edt.setBackgroundColor(ContextCompat.getColor(mainAct, R.color.transparent));
        edt.setFocusable(false);

        tvReadMore.setTextSize(TypedValue.COMPLEX_UNIT_SP, 12);
        tvReadMore.setTextColor(ContextCompat.getColor(mainAct, R.color.clOrangeFilter));
        tvReadMore.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.ITALIC);
        tvReadMore.setEllipsize(TextUtils.TruncateAt.END);
        tvReadMore.setGravity(Gravity.RIGHT);
        tvReadMore.setVisibility(View.GONE);
        tvReadMore.setText(Functions.share.getTitle("TEXT_CONTROL_READMORE", "Read more ..."));
        lnEditor.setBackgroundColor(ContextCompat.getColor(mainAct, R.color.transparent));
    }

    @Override
    public void initializeFrameView(LinearLayout frame) {
        if (element.isHidden()) return;
        super.initializeFrameView(frame);

        lnContent.removeView(lnLine);
        tvValue.setVisibility(View.GONE);

        int padding = (int) TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, 6, mainAct.getResources().getDisplayMetrics());
        RelativeLayout.LayoutParams paramsRelaEditor = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MATCH_PARENT, RelativeLayout.LayoutParams.WRAP_CONTENT);
        LinearLayout.LayoutParams paramsRichEditor = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, Functions.share.convertDpToPixel(100, frame.getContext()));
        LinearLayout.LayoutParams paramsEditText = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.MATCH_PARENT);

        edt.setLayoutParams(paramsEditText);
        tvReadMore.setLayoutParams(paramsEditText);

        relaEditor.setLayoutParams(paramsRelaEditor);
        lnEditor.setLayoutParams(paramsRichEditor);

        edt.setPadding(padding, 0, padding, 0);
        tvReadMore.setPadding(padding, 0, padding, 2 * padding);
        richEditor.setLayoutParams(paramsRichEditor);
        richEditor.setEnabled(false);

        if (parentView != null) {
            lnContent.setOnClickListener(view -> handleClicks());
            edt.setOnClickListener(view -> handleClicks());
            tvReadMore.setOnClickListener(view -> handleClicks());
            lnEditor.setOnClickListener(view -> handleClicks());
        }

        frame.addView(edt);
        frame.addView(tvReadMore);
    }

    private void handleClicks() {
        new MultipleClickGuard(view -> {
            Intent intent = new Intent();
            intent.setAction(VarsReceiver.FORMCLICK);
            intent.putExtra("element", new Gson().toJson(element));
            BroadcastUtility.send(mainAct, intent);
        }, 2000);
    }

    @Override
    public void setEnable() {
        super.setEnable();
        if (element.isEnable()) {
            tvValue.setTextColor(ContextCompat.getColor(mainAct, R.color.clBlueEnable));
        }
    }

    @Override
    public void setProprety() {
        super.setProprety();
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
        setValue(DetailFunc.share.formatHTMLToString(element.getValue()));
        if (!Functions.isNullOrEmpty(getValue())) {
            edt.setText(getValue());
            edt.setHeight(RelativeLayout.LayoutParams.WRAP_CONTENT);
        }
    }
}

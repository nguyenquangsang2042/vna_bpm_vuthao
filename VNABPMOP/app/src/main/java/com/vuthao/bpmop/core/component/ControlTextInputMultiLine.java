package com.vuthao.bpmop.core.component;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Intent;
import android.graphics.Typeface;
import android.text.Layout;
import android.text.TextUtils;
import android.util.TypedValue;
import android.view.Gravity;
import android.view.View;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.core.content.ContextCompat;
import androidx.core.content.res.ResourcesCompat;

import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.MultipleClickGuard;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.detail.custom.DetailFunc;

import org.json.JSONObject;

public class ControlTextInputMultiLine extends ControlBase {
    private final LinearLayout parentView;
    private final ViewElement element;
    private TextView tvReadMore;
    private ViewElement elementParent; // Element của Control Grid
    private ViewElement elementChild; // Element child của Item dc click vào
    private int flagView;
    private JSONObject JObjectChild;


    @Override
    public void setValue(String value) {
        if (!Functions.isNullOrEmpty(element.getValue())) {
            tvValue.setText(DetailFunc.share.formatHTMLToString(element.getValue()));
            tvReadMore.post(() -> {
                int lines = tvValue.getLineCount();
                if (lines >= 5) {
                    tvReadMore.setVisibility(View.VISIBLE);
                } else {
                    tvReadMore.setVisibility(View.GONE);
                }
            });
        } else {
            if (element.isEnable()) {
                tvValue.setText(Functions.share.getTitle("TEXT_CHOOSE_CONTENT", "Chọn nội dung..."));
                tvValue.setTypeface(super.tvValue.getTypeface(), Typeface.ITALIC);
                tvValue.setTextSize(TypedValue.COMPLEX_UNIT_SP, 12);
                tvReadMore.setVisibility(View.GONE);
            } else {
                tvValue.setVisibility(View.INVISIBLE);
                tvReadMore.setVisibility(View.GONE);
            }
        }
    }

    public ControlTextInputMultiLine(Activity mainAct, LinearLayout parentView, ViewElement element) {
        super(mainAct);
        this.mainAct = mainAct;
        this.parentView = parentView;
        this.element = element;
        initializeComponent();
    }

    /// <summary>
    /// Constructor của Input Grid Details
    /// </summary>
    public ControlTextInputMultiLine(Activity mainAct, LinearLayout parentView, ViewElement elementParent, ViewElement elementChild, JSONObject JObjectChild, int flagView) {
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
    public String getValue() {
        return tvValue.getText().toString();
    }

    @Override
    public void initializeComponent() {
        super.initializeComponent();

        tvReadMore = new TextView(mainAct);
        if (lnContent != null) {
            tvReadMore.setOnClickListener(new MultipleClickGuard(new View.OnClickListener() {
                @Override
                public void onClick(View view) {
                    handleClicks();
                }
            }, 2000));

            lnContent.setOnClickListener(new MultipleClickGuard(new View.OnClickListener() {
                @Override
                public void onClick(View view) {
                    handleClicks();
                }
            }, 2000));
        }

        if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
            tvReadMore.setText(Functions.share.getTitle("TEXT_CONTROL_READMORE", "Xem thêm..."));
        } else {
            tvReadMore.setText(Functions.share.getTitle("TEXT_CONTROL_READMORE", "Read more ..."));
        }

        tvReadMore.setTextSize(TypedValue.COMPLEX_UNIT_SP, 12);
        tvReadMore.setTextColor(ContextCompat.getColor(mainAct, R.color.clOrangeFilter));
        tvReadMore.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.ITALIC);
        tvReadMore.setEllipsize(TextUtils.TruncateAt.END);
        tvReadMore.setGravity(Gravity.RIGHT);
        tvReadMore.setVisibility(View.GONE);
    }

    private void handleClicks() {
        if (parentView != null) {
            Intent intent = new Intent();
            if (element.isEnable()) {
                if (JObjectChild != null) {
                    intent.setAction(VarsReceiver.CHILDACTION);
                    intent.putExtra("elementParent", new Gson().toJson(elementParent));
                    intent.putExtra("elementChild", new Gson().toJson(elementChild));
                    intent.putExtra("json", JObjectChild.toString());
                    intent.putExtra("flagview", flagView);
                } else {
                    intent.setAction(VarsReceiver.FORMCLICK);
                    intent.putExtra("element", new Gson().toJson(element));
                }
            } else {
                Layout layout = tvValue.getLayout();
                int lines = tvValue.getLineCount();

                int ellipsisCount = layout.getEllipsisCount(lines - 1);
                if (ellipsisCount > 0) {
                    if (JObjectChild != null) {
                        intent.setAction(VarsReceiver.CHILDACTION);
                        intent.putExtra("elementParent", new Gson().toJson(elementParent));
                        intent.putExtra("elementChild", new Gson().toJson(elementChild));
                        intent.putExtra("json", JObjectChild.toString());
                        intent.putExtra("flagview", flagView);
                    } else {
                        intent.setAction(VarsReceiver.FORMCLICK);
                        intent.putExtra("element", new Gson().toJson(element));
                    }
                }
            }
            BroadcastUtility.send(mainAct, intent);
        }
    }

    @Override
    public void initializeFrameView(LinearLayout frame) {
        if (element.isHidden()) return;
        super.initializeFrameView(frame);

        tvValue.setMaxLines(3);
        tvValue.setEllipsize(TextUtils.TruncateAt.END);
        tvReadMore.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT));

        lnContent.removeView(lnLine);
        frame.addView(tvReadMore);
        frame.addView(lnLine);
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

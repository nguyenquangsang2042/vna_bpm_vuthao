package com.vuthao.bpmop.core.component;

import android.app.Activity;
import android.graphics.Typeface;
import android.text.TextUtils;
import android.util.TypedValue;
import android.widget.Button;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.core.content.ContextCompat;
import androidx.core.content.res.ResourcesCompat;

import com.vuthao.bpmop.R;

public class ControlBase extends ComponentBase {
    protected LinearLayout lnContent, lnLine;
    protected TextView tvTitle, tvValue;
    protected Button btnAction;
    protected Activity mainAct;

    public ControlBase(Activity mainAct) {
        this.mainAct = mainAct;
    }

    @Override
    public void initializeCategory(int Category) {
        super.initializeCategory(Category);
    }

    @Override
    public void initializeComponent() {
        tvTitle = new TextView(mainAct);
        tvValue = new TextView(mainAct);
        lnLine = new LinearLayout(mainAct);
        lnContent = new LinearLayout(mainAct);
        btnAction = new Button(mainAct);

        tvTitle.setTextSize(TypedValue.COMPLEX_UNIT_SP, 12);
        tvTitle.setTextColor(ContextCompat.getColor(mainAct, R.color.clBottomDisable));
        tvTitle.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.NORMAL);
        tvTitle.setEllipsize(TextUtils.TruncateAt.END);

        tvValue.setTextSize(TypedValue.COMPLEX_UNIT_SP, 15);
        tvValue.setTextColor(ContextCompat.getColor(mainAct, R.color.clBlack));
        tvValue.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.NORMAL);
        tvValue.setEllipsize(TextUtils.TruncateAt.END);

        lnLine.setBackgroundColor(ContextCompat.getColor(mainAct,R.color.clControlGrayLight));

        lnContent.setOrientation(LinearLayout.VERTICAL);
        lnLine.setOrientation(LinearLayout.VERTICAL);
    }

    @Override
    public void initializeFrameView(LinearLayout frame) {
        super.initializeFrameView(frame);

        frame.setPadding(0,0,0,0);

        if (lnContent != null && lnLine != null && tvTitle != null && tvValue != null) {
            lnContent.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT));
            lnLine.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, (int)TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_PX, 0 , mainAct.getResources().getDisplayMetrics())));
            tvTitle.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT));
            tvValue.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT));

            int padding = (int)TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_PX, 3 , mainAct.getResources().getDisplayMetrics());

            lnContent.setPadding(padding, 2 * padding, padding, 2 * padding);
            tvTitle.setPadding(padding, 2 * padding, padding, padding);
            tvValue.setPadding(padding, padding / 2, padding, 2 * padding);
            lnLine.setPadding(padding, 2 * padding, padding, 2 * padding);

            lnContent.addView(tvTitle);
            lnContent.addView(tvValue);
            lnContent.addView(lnLine);

            frame.addView(lnContent);
        }
    }

    public interface ControlAttachmentHorizontalListener {
        void OnClick(int pos);
    }

    public interface ControlAttachmentVerticalListener {
        void OnClick(int pos);
    }

    public interface ControlLinkedWorkflowListener {
        void OnDeleteItemClick(int pos);
    }
}

package com.vuthao.bpmop.core.component;

import android.app.Activity;
import android.content.Intent;
import android.content.res.Resources;
import android.graphics.Typeface;
import android.text.TextUtils;
import android.util.TypedValue;
import android.view.Gravity;
import android.view.View;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.core.content.ContextCompat;
import androidx.core.content.res.ResourcesCompat;

import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;

public class ControlButtonBot extends ControlBase {
    private ViewElement element;
    private LinearLayout lnAction;
    private ImageView imgAction;
    private TextView tvAction;

    public ControlButtonBot(Activity mainAct) {
        super(mainAct);
    }

    public ControlButtonBot(Activity mainAct, LinearLayout parentView, ViewElement element, Resources resource) {
        super(mainAct);
        this.mainAct = mainAct;
        this.element = element;
        initializeComponent();
    }

    @Override
    public void initializeComponent() {
        super.initializeComponent();

        lnAction = new LinearLayout(mainAct);
        imgAction = new ImageView(mainAct);
        tvAction = new TextView(mainAct);

        imgAction.setScaleType(ImageView.ScaleType.CENTER);
        imgAction.setAdjustViewBounds(true);

        tvAction.setTextSize(TypedValue.COMPLEX_UNIT_SP, 14);
        tvAction.setTextColor(ContextCompat.getColor(mainAct, R.color.clBlack));
        tvAction.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.NORMAL);
        tvAction.setMaxLines(1);
        tvAction.setEllipsize(TextUtils.TruncateAt.END);
        tvAction.setGravity(Gravity.CENTER_VERTICAL);

        lnAction.setOrientation(LinearLayout.HORIZONTAL);

        if (lnContent != null) {
            lnContent.setOnClickListener(v -> {
                Intent intent = new Intent();
                intent.setAction(VarsReceiver.BOTTOM);
                intent.putExtra("element", new Gson().toJson(element));
                BroadcastUtility.send(mainAct, intent);
            });
        }
    }

    @Override
    public void initializeFrameView(LinearLayout frame) {
        if (element != null) {
            if (element.isHidden()) return;
        }

        super.initializeFrameView(frame);

        if (lnContent != null) {
            lnContent.setVisibility(View.GONE);
        }

        if (element != null) {
            int paddingLnAction = (int) TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, 3, mainAct.getResources().getDisplayMetrics());
            int paddingImage = (int) TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, 6, mainAct.getResources().getDisplayMetrics());

            LinearLayout.LayoutParams paramAction = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.MATCH_PARENT);
            paramAction.gravity = Gravity.CENTER;
            lnAction.setLayoutParams(paramAction);
            lnAction.setPadding(0, paddingLnAction, 0, paddingLnAction);
            lnAction.setGravity(Gravity.CENTER);

            if (!Functions.isNullOrEmpty(element.getTitle())) {
                LinearLayout.LayoutParams paramsImage = new LinearLayout.LayoutParams(Functions.share.convertDpToPixel(30, frame.getContext()), Functions.share.convertDpToPixel(30, frame.getContext()));
                paramsImage.gravity = Gravity.CENTER;
                imgAction.setLayoutParams(paramsImage);
                imgAction.setPadding(paddingImage, paddingImage, paddingImage, paddingImage);

                LinearLayout.LayoutParams paramtvAction = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.MATCH_PARENT);
                paramtvAction.setMargins(paddingImage, 0, 0, 0);
                paramtvAction.gravity = Gravity.CENTER;
                tvAction.setLayoutParams(paramtvAction);
                tvAction.setForegroundGravity(Gravity.CENTER);
                tvAction.setTextColor(ContextCompat.getColor(mainAct, R.color.clBlack));
                tvAction.setPadding(0, 0, 0, 0); // cho cách Imageview action ra
                //_tvAction.setTextAlignment(View.TEXT_ALIGNMENT_CENTER);

                lnAction.addView(imgAction);
                lnAction.addView(tvAction);

            } else {
                LinearLayout.LayoutParams paramsImage = new LinearLayout.LayoutParams(Functions.share.convertDpToPixel(30, frame.getContext()), Functions.share.convertDpToPixel(30, frame.getContext()));
                paramsImage.gravity = Gravity.CENTER;
                imgAction.setLayoutParams(paramsImage);
                imgAction.setPadding(paddingImage, paddingImage, paddingImage, paddingImage);
                imgAction.setColorFilter(ContextCompat.getColor(mainAct, R.color.clActionBlack));

                lnAction.addView(imgAction);
            }

            if (lnAction != null) {
                // attach clicks to details
                lnAction.setOnClickListener(v -> {
                    Intent intent = new Intent();
                    intent.setAction(VarsReceiver.BOTTOM);
                    intent.putExtra("element", new Gson().toJson(element));
                    BroadcastUtility.send(mainAct, intent);
                });
            }

            frame.addView(lnAction);
        }
    }

    @Override
    public void setProprety() {
        super.setProprety();
    }

    @Override
    public void setEnable() {
        super.setEnable();
    }

    @Override
    public void setTitle() {
        super.setTitle();

        if (element!= null) {
            if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
                if (!Functions.isNullOrEmpty(element.getTitle())) {
                    tvAction.setText(element.getTitle());
                }
            } else {
                if (!Functions.isNullOrEmpty(element.getValue())) {
                    tvAction.setText(element.getValue());
                }
            }
        }
    }

    @Override
    public void setValue() {
        if (element != null && !Functions.isNullOrEmpty(element.getValue())) {
            String imgName = "icon_bpm_btn_action_" + element.getID();
            int resId = mainAct.getResources().getIdentifier(imgName.toLowerCase(), "drawable", mainAct.getPackageName());
            imgAction.setImageResource(resId);
        }
    }
}

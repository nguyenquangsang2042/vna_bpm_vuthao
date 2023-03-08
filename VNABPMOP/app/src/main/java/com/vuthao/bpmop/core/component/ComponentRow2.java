package com.vuthao.bpmop.core.component;

import android.app.Activity;
import android.util.DisplayMetrics;
import android.util.TypedValue;
import android.widget.LinearLayout;

import androidx.core.content.ContextCompat;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.model.dynamic.ViewRow;

public class ComponentRow2 extends ComponentRow1 {
    private final ViewRow controlData;
    private ComponentRow1 row1, row2;

    public ComponentRow2(Activity mainAct, LinearLayout parentView, ViewRow controlData, int widthScreenTablet, boolean visibleItemLine, int flagView) {
        super();
        this.mainAct = mainAct;
        this.parentView = parentView;
        this.controlData = controlData;
        this.widthScreenTablet = widthScreenTablet;
        this.visibleItemLine = visibleItemLine;
        this.flagView = flagView;
        initializeComponent();
    }

    @Override
    public void initializeComponent() {
        row1 = new ComponentRow1(mainAct, parentView, controlData.getElements().get(0), (int) widthScreenTablet / 2, visibleItemLine, flagView);
        row2 = new ComponentRow1(mainAct, parentView, controlData.getElements().get(1), (int) widthScreenTablet / 2, visibleItemLine, flagView);
    }

    @Override
    public void initializeFrameView(LinearLayout frame) {
        super.initializeFrameView(frame);
        int widthRow;
        // ForPhone
        if (widthScreenTablet == -1) {
            DisplayMetrics dm = mainAct.getResources().getDisplayMetrics();
            widthRow = dm.widthPixels / 2;
        } else {
            // For Tablet
            widthRow = widthScreenTablet / 2;
        }

        LinearLayout lncontent = new LinearLayout(mainAct);
        lncontent.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT));
        lncontent.setOrientation(LinearLayout.HORIZONTAL);

        LinearLayout lnRow1 = new LinearLayout(mainAct);
        LinearLayout lnRow2 = new LinearLayout(mainAct);
        lnRow1.setLayoutParams(new LinearLayout.LayoutParams(widthRow, LinearLayout.LayoutParams.WRAP_CONTENT));
        lnRow2.setLayoutParams(new LinearLayout.LayoutParams(widthRow, LinearLayout.LayoutParams.WRAP_CONTENT));

        row1.initializeFrameView(lnRow1);
        row2.initializeFrameView(lnRow2);

        LinearLayout lnRowLine = new LinearLayout(mainAct);
        // Ẩn Line của Item -> Load Line của nguyên Row
        if (!visibleItemLine) {
            int padding = (int) TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, 6, mainAct.getResources().getDisplayMetrics());
            lnRowLine.setBackgroundColor(ContextCompat.getColor(mainAct, R.color.clControlGrayLight));
            lnRowLine.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, (int) TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_PX, 1, mainAct.getResources().getDisplayMetrics())));
            lnRowLine.setPadding(padding * 2, padding, padding * 2, padding);
        }

        lncontent.addView(lnRow1);
        lncontent.addView(lnRow2);
        frame.addView(lncontent);
    }

    @Override
    public void setTitle() {
        row1.setTitle();
        row2.setTitle();
    }

    @Override
    public void setValue() {
        row1.setValue();
        row2.setValue();
    }

    @Override
    public void setProprety() {
        row1.setProprety();
        row2.setProprety();
    }

    @Override
    public void setEnable() {
        super.setEnable();
        row1.setEnable();
        row2.setEnable();
    }
}

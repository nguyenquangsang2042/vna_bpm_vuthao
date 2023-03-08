package com.vuthao.bpmop.core.component;

import android.app.Activity;
import android.util.DisplayMetrics;
import android.util.TypedValue;
import android.widget.LinearLayout;

import androidx.core.content.ContextCompat;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.model.dynamic.ViewRow;

public class ComponentRow3 extends ComponentRow1 {
    private final ViewRow controlData;
    private ComponentRow1 row1, row2, row3;

    public ComponentRow3(Activity mainAct, LinearLayout parentView, ViewRow controlData, int widthScreenTablet, boolean visibleItemLine, int flagView) {
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
        row1 = new ComponentRow1(mainAct, parentView, controlData.getElements().get(0), (int) widthScreenTablet / 3, visibleItemLine, flagView);
        row2 = new ComponentRow1(mainAct, parentView, controlData.getElements().get(1), (int) widthScreenTablet / 3, visibleItemLine, flagView);
        row3 = new ComponentRow1(mainAct, parentView, controlData.getElements().get(2), (int) widthScreenTablet / 3, visibleItemLine, flagView);
    }

    @Override
    public void initializeFrameView(LinearLayout frame) {
        int _widthRow;

        // ForPhone
        if (widthScreenTablet == -1) {
            DisplayMetrics dm = mainAct.getResources().getDisplayMetrics();
            _widthRow = dm.widthPixels / 3;
        } else {
            // For Tablet
            _widthRow = widthScreenTablet / 3;
        }

        LinearLayout _lncontent = new LinearLayout(mainAct);
        _lncontent.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT));
        _lncontent.setOrientation(LinearLayout.HORIZONTAL);

        LinearLayout _lnRow1 = new LinearLayout(mainAct);
        LinearLayout _lnRow2 = new LinearLayout(mainAct);
        LinearLayout _lnRow3 = new LinearLayout(mainAct);

        _lnRow1.setLayoutParams(new LinearLayout.LayoutParams(_widthRow, LinearLayout.LayoutParams.WRAP_CONTENT));
        _lnRow2.setLayoutParams(new LinearLayout.LayoutParams(_widthRow, LinearLayout.LayoutParams.WRAP_CONTENT));
        _lnRow3.setLayoutParams(new LinearLayout.LayoutParams(_widthRow, LinearLayout.LayoutParams.WRAP_CONTENT));

        row1.initializeFrameView(_lnRow1);
        row2.initializeFrameView(_lnRow2);
        row3.initializeFrameView(_lnRow3);

        LinearLayout _lnRowLine = new LinearLayout(mainAct);

        if (!visibleItemLine) {
            int _padding = (int) TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, 6, mainAct.getResources().getDisplayMetrics());
            _lnRowLine.setBackgroundColor(ContextCompat.getColor(mainAct, R.color.clControlGrayLight));
            _lnRowLine.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, (int) TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_PX, 1, mainAct.getResources().getDisplayMetrics())));
            _lnRowLine.setPadding(_padding * 2, _padding, _padding * 2, _padding);
        }

        _lncontent.addView(_lnRow1);
        _lncontent.addView(_lnRow2);
        _lncontent.addView(_lnRow3);
        frame.addView(_lncontent);
    }

    @Override
    public void setTitle() {
        row1.setTitle();
        row2.setTitle();
        row3.setTitle();
    }

    @Override
    public void setValue() {
        super.setValue();

        row1.setValue();
        row2.setValue();
        row3.setValue();
    }

    @Override
    public void setProprety() {
        row1.setProprety();
        row2.setProprety();
        row3.setProprety();
    }

    @Override
    public void setEnable() {
        super.setEnable();

        row1.setEnable();
        row2.setEnable();
        row3.setEnable();
    }
}

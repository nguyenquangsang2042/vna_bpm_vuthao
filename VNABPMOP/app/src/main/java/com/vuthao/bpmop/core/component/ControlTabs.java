package com.vuthao.bpmop.core.component;

import android.app.Activity;
import android.util.Pair;
import android.util.TypedValue;
import android.view.View;
import android.widget.LinearLayout;

import androidx.recyclerview.widget.RecyclerView;
import androidx.recyclerview.widget.StaggeredGridLayoutManager;

import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.core.adapter.ControlTabsAdapter;

import java.util.ArrayList;

public class ControlTabs extends ControlBase implements ControlTabsAdapter.ControlTabsListener {
    private ViewElement element;
    private RecyclerView recyTabs;
    private ArrayList<Pair<String, String>> lstTabs = new ArrayList<Pair<String, String>>();
    private int selectedIndex = 0;
    private ControlTabsAdapter adapterTabs;
    private int widthScreenTablet = -1;

    public ControlTabs(Activity mainAct, LinearLayout parentView, ViewElement element, int widthScreenTablet) {
        super(mainAct);
        this.mainAct = mainAct;
        this.element = element;
        this.widthScreenTablet = widthScreenTablet;
        initializeComponent();
    }

    @Override
    public void initializeComponent() {
        super.initializeComponent();

        recyTabs = new RecyclerView(mainAct);
    }

    @Override
    public void initializeFrameView(LinearLayout frame) {
        if (element.isHidden()) return;
        super.initializeFrameView(frame);
        tvValue.setVisibility(View.GONE);
        lnLine.setVisibility(View.GONE);
        lnContent.removeView(lnLine);

        int padding = (int) TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, 12, mainAct.getResources().getDisplayMetrics());
        recyTabs.setPadding(padding, padding, padding, padding);
        frame.addView(recyTabs);
        frame.addView(lnLine);
    }

    @Override
    public void setProprety() {

    }

    @Override
    public void setEnable() {
        super.setEnable();

        recyTabs.setEnabled(element.isEnable());
    }

    @Override
    public void setTitle() {
        super.setTitle();

        tvTitle.setText(element.getTitle());
    }


    @Override
    public void setValue() {
        super.setValue();

        String selectedValue = element.getValue().split(";#")[1].toLowerCase();
        String[] arrayValue = element.getDataSource().split(";#");

        if (arrayValue.length > 2) {
            String[] arraySetValue = new String[arrayValue.length / 2]; // bo mấy dấu ";#" đi nên /2
            int index = 0;
            for (int i = 0; i < arrayValue.length; i += 2) {
                lstTabs.add(new Pair<>(arrayValue[i], arrayValue[i + 1]));
                arraySetValue[index] = arrayValue[i + 1];
                if (selectedValue.equals(arrayValue[i + 1].toLowerCase())) {
                    selectedIndex = index;
                }
                index++;
            }
        }

        StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayout.HORIZONTAL);
        adapterTabs = new ControlTabsAdapter(mainAct, lstTabs, selectedIndex, widthScreenTablet, this);
        recyTabs.setLayoutManager(staggeredGridLayoutManager);
        recyTabs.setAdapter(adapterTabs);
    }

    @Override
    public void OnClick(int pos) {
        selectedIndex = pos;
        StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayout.HORIZONTAL);
        adapterTabs = new ControlTabsAdapter(mainAct, lstTabs, selectedIndex, widthScreenTablet, this);
        recyTabs.setLayoutManager(staggeredGridLayoutManager);
        recyTabs.setAdapter(adapterTabs);
    }
}



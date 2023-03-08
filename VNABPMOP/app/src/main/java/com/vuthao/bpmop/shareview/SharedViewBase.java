package com.vuthao.bpmop.shareview;

import android.app.Activity;
import android.view.LayoutInflater;
import android.view.View;

public class SharedViewBase {
    protected Activity mainAct;
    protected LayoutInflater inflater;
    protected String fragmentTag;
    protected View rootView;

    public SharedViewBase() {
    }

    public SharedViewBase(LayoutInflater inflater, Activity mainAct, String fragmentTag, View rootView) {
        this.inflater = inflater;
        this.mainAct = mainAct;
        this.fragmentTag = fragmentTag;
        this.rootView = rootView;
    }

    public void initializeView() { }
}

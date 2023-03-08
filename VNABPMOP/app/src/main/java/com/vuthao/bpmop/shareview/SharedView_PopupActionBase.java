package com.vuthao.bpmop.shareview;

import android.app.Activity;
import android.view.LayoutInflater;
import android.view.View;

import com.vuthao.bpmop.base.model.custom.ButtonAction;
import com.vuthao.bpmop.core.Vars;

import java.util.ArrayList;

public class SharedView_PopupActionBase extends SharedViewBase {
    protected int flagView;
    protected ButtonAction buttonAction;
    protected ArrayList<ButtonAction> lstActionMore;

    public SharedView_PopupActionBase(LayoutInflater inflater, Activity mainAct, String fragmentTag, View rootView) {
        super(inflater, mainAct, fragmentTag, rootView);
    }

    public void initializeValue_DetailWorkflow(ButtonAction buttonAction) {
        flagView = Vars.FlagViewControlAction.DetailWorkflow;

        this.buttonAction = buttonAction;
    }

    public void initializeValue_DetailWorkflow_ActionMore(ArrayList<ButtonAction> lstActionMore) {
        flagView = Vars.FlagViewControlAction.DetailWorkflow;

        this.lstActionMore = lstActionMore;
    }
}

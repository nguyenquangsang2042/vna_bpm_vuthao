package com.vuthao.bpmop.shareview;

import android.app.Activity;
import android.view.LayoutInflater;
import android.view.View;

import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.core.Vars;

import org.json.JSONObject;

public class SharedView_PopupControlBase extends SharedViewBase{
    public int flagView;
    public ViewElement elementParent;
    public ViewElement elementPopup;  // Nếu là popup của control input grid detail mới xài cái này
    public JSONObject JObjectChild; // Nếu là popup của control input grid detail mới xài cái này

    public SharedView_PopupControlBase(LayoutInflater inflater, Activity mainAct, String fragmentTag, View rootView) {
        super(inflater, mainAct, fragmentTag, rootView);
    }

    public void initializeValue_Master(ViewElement viewElement) {
        flagView = Vars.FlagViewControlDynamic.DetailWorkflow;
        this.elementParent = viewElement;
    }

    public void initializeValue_InputGridDetail(ViewElement elementParent, ViewElement elementPopup, JSONObject JObjectChild) {
        flagView = Vars.FlagViewControlDynamic.DetailWorkflow_InputGridDetail;
        this.elementParent = elementParent;
        this.elementPopup = elementPopup;
        this.JObjectChild = JObjectChild;
    }

    public void initializeValue_Master_CreateTask(ViewElement elementParent) {
        flagView = Vars.FlagViewControlDynamic.DetailCreateTask;
        this.elementParent = elementParent;
    }
}

package com.vuthao.bpmop.base.model.custom;

import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.model.app.WorkflowStepDefine;

import java.util.ArrayList;

public class BoardKanBan {
    private WorkflowStepDefine itemStepDefine;
    private ArrayList<AppBase> lstAppBase;

    public BoardKanBan() {
    }

    public BoardKanBan(WorkflowStepDefine itemStepDefine, ArrayList<AppBase> lstAppBase) {
        this.itemStepDefine = itemStepDefine;
        this.lstAppBase = lstAppBase;
    }

    public WorkflowStepDefine getItemStepDefine() {
        return itemStepDefine;
    }

    public void setItemStepDefine(WorkflowStepDefine itemStepDefine) {
        this.itemStepDefine = itemStepDefine;
    }

    public ArrayList<AppBase> getLstAppBase() {
        return lstAppBase;
    }

    public void setLstAppBase(ArrayList<AppBase> lstAppBase) {
        this.lstAppBase = lstAppBase;
    }
}

package com.vuthao.bpmop.base.model.custom;

import java.util.ArrayList;

public class ExpandTask {
    private Task groupItem;
    private ArrayList<ExpandTask> lstChild;

    public ExpandTask() {
    }

    public ExpandTask(Task groupItem, ArrayList<ExpandTask> lstChild) {
        this.groupItem = groupItem;
        this.lstChild = lstChild;
    }

    public Task getGroupItem() {
        return groupItem;
    }

    public void setGroupItem(Task groupItem) {
        this.groupItem = groupItem;
    }

    public ArrayList<ExpandTask> getLstChild() {
        return lstChild;
    }

    public void setLstChild(ArrayList<ExpandTask> lstChild) {
        this.lstChild = lstChild;
    }
}

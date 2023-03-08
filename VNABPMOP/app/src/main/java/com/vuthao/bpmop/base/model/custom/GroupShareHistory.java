package com.vuthao.bpmop.base.model.custom;

import java.util.ArrayList;

public class GroupShareHistory {
    private ShareHistory parentItem;
    private ArrayList<ShareHistory> listChild;

    public GroupShareHistory() {
    }

    public GroupShareHistory(ShareHistory parentItem, ArrayList<ShareHistory> listChild) {
        this.parentItem = parentItem;
        this.listChild = listChild;
    }

    public ShareHistory getParentItem() {
        return parentItem;
    }

    public void setParentItem(ShareHistory parentItem) {
        this.parentItem = parentItem;
    }

    public ArrayList<ShareHistory> getListChild() {
        return listChild;
    }

    public void setListChild(ArrayList<ShareHistory> listChild) {
        this.listChild = listChild;
    }
}

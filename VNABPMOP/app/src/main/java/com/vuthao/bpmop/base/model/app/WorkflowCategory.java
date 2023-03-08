package com.vuthao.bpmop.base.model.app;

import io.realm.RealmObject;
import io.realm.annotations.Ignore;
import io.realm.annotations.PrimaryKey;

public class WorkflowCategory extends RealmObject {
    @PrimaryKey
    private int ID;
    private String Title;
    private int Order;
    @Ignore
    private boolean IsSelected;

    public WorkflowCategory(int ID, String title, int order, boolean isSelected) {
        this.ID = ID;
        Title = title;
        Order = order;
        IsSelected = isSelected;
    }

    public WorkflowCategory() {
    }

    public int getID() {
        return ID;
    }

    public void setID(int ID) {
        this.ID = ID;
    }

    public String getTitle() {
        return Title;
    }

    public void setTitle(String title) {
        Title = title;
    }

    public int getOrder() {
        return Order;
    }

    public void setOrder(int order) {
        Order = order;
    }

    public boolean isSelected() {
        return IsSelected;
    }

    public void setSelected(boolean selected) {
        IsSelected = selected;
    }
}

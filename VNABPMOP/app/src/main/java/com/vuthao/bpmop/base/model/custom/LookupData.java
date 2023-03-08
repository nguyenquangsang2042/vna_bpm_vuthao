package com.vuthao.bpmop.base.model.custom;

import io.realm.annotations.Ignore;

public class LookupData {
    private String ID;
    private String Title;
    private boolean IsSelected;

    public LookupData(String ID, String title, boolean isSelected) {
        this.ID = ID;
        Title = title;
        IsSelected = isSelected;
    }

    public LookupData() {
    }

    public String getID() {
        return ID;
    }

    public void setID(String ID) {
        this.ID = ID;
    }

    public String getTitle() {
        return Title;
    }

    public void setTitle(String title) {
        Title = title;
    }

    public boolean isSelected() {
        return IsSelected;
    }

    public void setSelected(boolean selected) {
        IsSelected = selected;
    }
}

package com.vuthao.bpmop.base.model.dynamic;

public class ViewBase {
    private String ID;
    private String Title;
    private String Value; // string
    private boolean Enable;
    private boolean IsRequire;

    public ViewBase() {
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

    public String getValue() {
        return Value;
    }

    public void setValue(String value) {
        Value = value;
    }

    public boolean isEnable() {
        return Enable;
    }

    public void setEnable(boolean enable) {
        Enable = enable;
    }

    public boolean isRequire() {
        return IsRequire;
    }

    public void setRequire(boolean require) {
        IsRequire = require;
    }
}

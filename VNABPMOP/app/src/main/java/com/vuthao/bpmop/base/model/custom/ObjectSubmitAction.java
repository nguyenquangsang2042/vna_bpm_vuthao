package com.vuthao.bpmop.base.model.custom;

import java.util.ArrayList;

public class ObjectSubmitAction {
    private String ID;
    private String Value;
    private String TypeSP;
    private String DataType;

    public ObjectSubmitAction() {
    }

    public ObjectSubmitAction(String ID, String value, String typeSP, String dataType) {
        this.ID = ID;
        Value = value;
        TypeSP = typeSP;
        DataType = dataType;
    }

    public String getID() {
        return ID;
    }

    public void setID(String ID) {
        this.ID = ID;
    }

    public String getValue() {
        return Value;
    }

    public void setValue(String value) {
        Value = value;
    }

    public String getTypeSP() {
        return TypeSP;
    }

    public void setTypeSP(String typeSP) {
        TypeSP = typeSP;
    }

    public String getDataType() {
        return DataType;
    }

    public void setDataType(String dataType) {
        DataType = dataType;
    }
}


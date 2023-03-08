package com.vuthao.bpmop.base.model.custom;

public class ObjectFilter {
    private String Key;
    private String LogicCon;
    private String Value;
    private String ValueType;
    private String ContentType;

    public ObjectFilter() {
    }

    public ObjectFilter(String key, String logicCon, String value, String valueType, String contentType) {
        Key = key;
        LogicCon = logicCon;
        Value = value;
        ValueType = valueType;
        ContentType = contentType;
    }

    public String getKey() {
        return Key;
    }

    public void setKey(String key) {
        Key = key;
    }

    public String getLogicCon() {
        return LogicCon;
    }

    public void setLogicCon(String logicCon) {
        LogicCon = logicCon;
    }

    public String getValue() {
        return Value;
    }

    public void setValue(String value) {
        Value = value;
    }

    public String getValueType() {
        return ValueType;
    }

    public void setValueType(String valueType) {
        ValueType = valueType;
    }

    public String getContentType() {
        return ContentType;
    }

    public void setContentType(String contentType) {
        ContentType = contentType;
    }
}

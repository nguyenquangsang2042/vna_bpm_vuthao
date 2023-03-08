package com.vuthao.bpmop.base.model.app;

import io.realm.RealmObject;
import io.realm.annotations.PrimaryKey;

public class DBVariable extends RealmObject {
    @PrimaryKey
    private String Id;
    private String Value;

    public DBVariable() {
    }

    public DBVariable(String id, String value) {
        Id = id;
        Value = value;
    }

    public String getId() {
        return Id;
    }

    public void setId(String id) {
        Id = id;
    }

    public String getValue() {
        return Value;
    }

    public void setValue(String value) {
        Value = value;
    }
}

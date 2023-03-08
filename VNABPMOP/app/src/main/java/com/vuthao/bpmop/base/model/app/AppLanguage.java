package com.vuthao.bpmop.base.model.app;

import io.realm.RealmObject;
import io.realm.annotations.PrimaryKey;

public class AppLanguage extends RealmObject {
    @PrimaryKey
    private String Key;
    private String ValueVN;
    private String ValueEN;

    public AppLanguage() {
    }

    public String getKey() {
        return Key;
    }

    public void setKey(String key) {
        Key = key;
    }

    public String getValueVN() {
        return ValueVN;
    }

    public void setValueVN(String valueVN) {
        ValueVN = valueVN;
    }

    public String getValueEN() {
        return ValueEN;
    }

    public void setValueEN(String valueEN) {
        ValueEN = valueEN;
    }
}

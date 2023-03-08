package com.vuthao.bpmop.base.model.custom;

import com.vuthao.bpmop.base.model.app.Status;

public class ObjectData extends Status {
    private String data;

    public ObjectData() {
    }

    public String getData() {
        return data;
    }

    public void setData(String data) {
        this.data = data;
    }
}

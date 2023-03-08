package com.vuthao.bpmop.base.model;

import com.vuthao.bpmop.base.model.app.Status;

public class ApiData extends Status {
    private String data;

    public ApiData(String data) {
        this.data = data;
    }

    public ApiData() {
    }

    public String getData() {
        return data;
    }

    public void setData(String data) {
        this.data = data;
    }
}

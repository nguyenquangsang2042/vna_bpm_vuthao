package com.vuthao.bpmop.base.model;

import com.vuthao.bpmop.base.model.app.Status;

public class ApiObject<T> extends Status {

    private T data;

    public ApiObject() {
    }

    public T getData() {
        return data;
    }

    public void setData(T data) {
        this.data = data;
    }
}

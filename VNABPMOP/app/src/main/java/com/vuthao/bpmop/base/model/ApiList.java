package com.vuthao.bpmop.base.model;

import com.vuthao.bpmop.base.model.app.Status;

import java.util.ArrayList;

public class ApiList<T> extends Status {
    private ArrayList<T> data;

    public ApiList() {
    }

    public ArrayList<T> getData() {
        return data;
    }

    public void setData(ArrayList<T> data) {
        this.data = data;
    }
}

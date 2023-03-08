package com.vuthao.bpmop.base.model.app;

import io.realm.RealmObject;
import io.realm.annotations.PrimaryKey;

public class TimeLanguage extends RealmObject {
    @PrimaryKey
    private int ID;
    private Integer Time;
    private int Type;
    private String Title;
    private String TitleEN;
    private int Index;
    private int Devices; // 0: dùng chung, 1: Mobile, 2: Web
    private String Modified;

    public TimeLanguage() {
    }

    public int getID() {
        return ID;
    }

    public void setID(int ID) {
        this.ID = ID;
    }

    public Integer getTime() {
        return Time;
    }

    public void setTime(Integer time) {
        Time = time;
    }

    public int getType() {
        return Type;
    }

    public void setType(int type) {
        Type = type;
    }

    public String getTitle() {
        return Title;
    }

    public void setTitle(String title) {
        Title = title;
    }

    public String getTitleEN() {
        return TitleEN;
    }

    public void setTitleEN(String titleEN) {
        TitleEN = titleEN;
    }

    public int getIndex() {
        return Index;
    }

    public void setIndex(int index) {
        Index = index;
    }

    public int getDevices() {
        return Devices;
    }

    public void setDevices(int devices) {
        Devices = devices;
    }

    public String getModified() {
        return Modified;
    }

    public void setModified(String modified) {
        Modified = modified;
    }
}

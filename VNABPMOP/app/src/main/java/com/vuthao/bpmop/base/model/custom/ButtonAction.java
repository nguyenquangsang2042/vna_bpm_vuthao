package com.vuthao.bpmop.base.model.custom;

import java.util.ArrayList;

public class ButtonAction {
    private int ID;
    private String Value;
    private String Title;
    private String TitleEN;
    private String Color;
    private ArrayList<ObjectElementNote> Notes;

    public ButtonAction() {
        Color = "0b61ae";
    }

    public int getID() {
        return ID;
    }

    public void setID(int ID) {
        this.ID = ID;
    }

    public String getValue() {
        return Value;
    }

    public void setValue(String value) {
        Value = value;
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

    public String getColor() {
        return Color;
    }

    public void setColor(String color) {
        Color = color;
    }

    public ArrayList<ObjectElementNote> getNotes() {
        return Notes;
    }

    public void setNotes(ArrayList<ObjectElementNote> notes) {
        Notes = notes;
    }
}

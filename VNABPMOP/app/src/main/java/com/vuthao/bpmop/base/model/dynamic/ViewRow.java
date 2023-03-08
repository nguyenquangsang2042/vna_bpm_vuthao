package com.vuthao.bpmop.base.model.dynamic;

import java.util.ArrayList;

public class ViewRow extends ViewBase {
    private int RowType;
    private ArrayList<ViewElement> Elements;

    public ViewRow() {
    }

    public ViewRow(int rowType, ArrayList<ViewElement> elements) {
        RowType = rowType;
        Elements = elements;
    }

    public int getRowType() {
        return RowType;
    }

    public void setRowType(int rowType) {
        RowType = rowType;
    }

    public ArrayList<ViewElement> getElements() {
        return Elements;
    }

    public void setElements(ArrayList<ViewElement> elements) {
        Elements = elements;
    }
}

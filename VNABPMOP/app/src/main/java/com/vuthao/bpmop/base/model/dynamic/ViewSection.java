package com.vuthao.bpmop.base.model.dynamic;

import java.util.ArrayList;

public class ViewSection extends ViewBase {
    private boolean ShowType;
    private boolean IsShowHint;
    private boolean IsFollow;
    private ArrayList<ViewRow> ViewRows;

    public ViewSection() {
    }

    public boolean isShowType() {
        return ShowType;
    }

    public void setShowType(boolean showType) {
        ShowType = showType;
    }

    public boolean isShowHint() {
        return IsShowHint;
    }

    public void setShowHint(boolean showHint) {
        IsShowHint = showHint;
    }

    public boolean isFollow() {
        return IsFollow;
    }

    public void setFollow(boolean follow) {
        IsFollow = follow;
    }

    public ArrayList<ViewRow> getViewRows() {
        return ViewRows;
    }

    public void setViewRows(ArrayList<ViewRow> viewRows) {
        ViewRows = viewRows;
    }
}

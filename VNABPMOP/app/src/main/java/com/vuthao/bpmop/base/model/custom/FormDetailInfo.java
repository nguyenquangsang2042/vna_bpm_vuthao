package com.vuthao.bpmop.base.model.custom;

import com.vuthao.bpmop.base.model.app.Status;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.model.dynamic.ViewRow;
import com.vuthao.bpmop.base.model.dynamic.ViewSection;

import java.util.ArrayList;

public class FormDetailInfo extends Status {
    private int ID;
    private String Title;
    private String Value;
    private boolean IsFollow;
    private ViewRow action;
    private ArrayList<ViewRow> ViewRows;
    private ArrayList<ViewSection> form;
    private MoreInfo moreInfo;

    public FormDetailInfo() {
    }

    public int getID() {
        return ID;
    }

    public void setID(int ID) {
        this.ID = ID;
    }

    public String getTitle() {
        return Title;
    }

    public void setTitle(String title) {
        Title = title;
    }

    public String getValue() {
        return Value;
    }

    public void setValue(String value) {
        Value = value;
    }

    public boolean isFollow() {
        return IsFollow;
    }

    public void setFollow(boolean follow) {
        IsFollow = follow;
    }

    public ViewRow getAction() {
        return action;
    }

    public void setAction(ViewRow action) {
        this.action = action;
    }

    public ArrayList<ViewRow> getViewRows() {
        return ViewRows;
    }

    public void setViewRows(ArrayList<ViewRow> viewRows) {
        ViewRows = viewRows;
    }

    public ArrayList<ViewSection> getForm() {
        return form;
    }

    public void setForm(ArrayList<ViewSection> form) {
        this.form = form;
    }

    public MoreInfo getMoreInfo() {
        return moreInfo;
    }

    public void setMoreInfo(MoreInfo moreInfo) {
        this.moreInfo = moreInfo;
    }

    public class MoreInfo {
        private String CommentChanged;
        private String OtherResourceId;

        public MoreInfo() {
        }

        public String getCommentChanged() {
            return CommentChanged;
        }

        public void setCommentChanged(String commentChanged) {
            CommentChanged = commentChanged;
        }

        public String getOtherResourceId() {
            return OtherResourceId;
        }

        public void setOtherResourceId(String otherResourceId) {
            OtherResourceId = otherResourceId;
        }
    }
}

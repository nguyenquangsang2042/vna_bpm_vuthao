package com.vuthao.bpmop.base.model.app;

import io.realm.RealmObject;
import io.realm.annotations.Ignore;
import io.realm.annotations.PrimaryKey;

public class Workflow extends RealmObject {
    @PrimaryKey
    private int WorkflowID;
    private String ListID;
    private String Code;
    private String Title;
    private String TitleEN;
    private String ImageURL;
    private int Favorite; // = True là có yêu thích và ngược lại
    private int IsPermission; // = True là hiện lên
    private String StatusName; // Deactive / Draft / Active
    private int WorkflowCategoryID;
    private String Modified;
    @Ignore
    private boolean isExpand;

    public Workflow() {
    }

    public int getWorkflowID() {
        return WorkflowID;
    }

    public void setWorkflowID(int workflowID) {
        WorkflowID = workflowID;
    }

    public String getListID() {
        return ListID;
    }

    public void setListID(String listID) {
        ListID = listID;
    }

    public String getCode() {
        return Code;
    }

    public void setCode(String code) {
        Code = code;
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

    public String getImageURL() {
        return ImageURL;
    }

    public void setImageURL(String imageURL) {
        ImageURL = imageURL;
    }

    public int isFavorite() {
        return Favorite;
    }

    public void setFavorite(int favorite) {
        Favorite = favorite;
    }

    public int isPermission() {
        return IsPermission;
    }

    public void setPermission(int permission) {
        IsPermission = permission;
    }

    public String getStatusName() {
        return StatusName;
    }

    public void setStatusName(String statusName) {
        StatusName = statusName;
    }

    public int getWorkflowCategoryID() {
        return WorkflowCategoryID;
    }

    public void setWorkflowCategoryID(int workflowCategoryID) {
        WorkflowCategoryID = workflowCategoryID;
    }

    public String getModified() {
        return Modified;
    }

    public void setModified(String modified) {
        Modified = modified;
    }

    public boolean isExpand() {
        return isExpand;
    }

    public void setExpand(boolean expand) {
        isExpand = expand;
    }
}

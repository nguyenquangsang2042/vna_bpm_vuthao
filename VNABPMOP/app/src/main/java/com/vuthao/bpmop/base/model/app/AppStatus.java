package com.vuthao.bpmop.base.model.app;

import io.realm.RealmObject;
import io.realm.annotations.Ignore;
import io.realm.annotations.PrimaryKey;

public class AppStatus extends RealmObject {
    @PrimaryKey
    private int ID;
    private String Title;
    private String TitleEN;
    private int Index;
    private String StatusDetails;
    private int ResourceCategoryIds;
    private String Modified;
    private String CreatedBy;
    private String ModifiedBy;
    private boolean IsShow;
    @Ignore
    private boolean IsSelected;

    public AppStatus() {
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

    public String getStatusDetails() {
        return StatusDetails;
    }

    public void setStatusDetails(String statusDetails) {
        StatusDetails = statusDetails;
    }

    public int getResourceCategoryIds() {
        return ResourceCategoryIds;
    }

    public void setResourceCategoryIds(int resourceCategoryIds) {
        ResourceCategoryIds = resourceCategoryIds;
    }

    public String getModified() {
        return Modified;
    }

    public void setModified(String modified) {
        Modified = modified;
    }

    public String getCreatedBy() {
        return CreatedBy;
    }

    public void setCreatedBy(String createdBy) {
        CreatedBy = createdBy;
    }

    public String getModifiedBy() {
        return ModifiedBy;
    }

    public void setModifiedBy(String modifiedBy) {
        ModifiedBy = modifiedBy;
    }

    public boolean isShow() {
        return IsShow;
    }

    public void setShow(boolean show) {
        IsShow = show;
    }

    public boolean isSelected() {
        return IsSelected;
    }

    public void setSelected(boolean selected) {
        IsSelected = selected;
    }
}

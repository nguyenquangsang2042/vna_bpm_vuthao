package com.vuthao.bpmop.base.model.app;

import io.realm.RealmObject;
import io.realm.annotations.PrimaryKey;

public class Group extends RealmObject {
    @PrimaryKey
    private String ID;
    private String Title;
    private String Description;
    private String Owner;
    private int WhoView;
    private int SPGroupId;
    private String Modified;
    private String ModifiedBy;
    private String Created;
    private String CreatedBy;
    private String Image;
    private int GroupType;
    private int Status;

    public Group() {
    }

    public String getID() {
        return ID;
    }

    public void setID(String ID) {
        this.ID = ID;
    }

    public String getTitle() {
        return Title;
    }

    public void setTitle(String title) {
        Title = title;
    }

    public String getDescription() {
        return Description;
    }

    public void setDescription(String description) {
        Description = description;
    }

    public String getOwner() {
        return Owner;
    }

    public void setOwner(String owner) {
        Owner = owner;
    }

    public int getWhoView() {
        return WhoView;
    }

    public void setWhoView(int whoView) {
        WhoView = whoView;
    }

    public int getSPGroupId() {
        return SPGroupId;
    }

    public void setSPGroupId(int SPGroupId) {
        this.SPGroupId = SPGroupId;
    }

    public String getModified() {
        return Modified;
    }

    public void setModified(String modified) {
        Modified = modified;
    }

    public String getModifiedBy() {
        return ModifiedBy;
    }

    public void setModifiedBy(String modifiedBy) {
        ModifiedBy = modifiedBy;
    }

    public String getCreated() {
        return Created;
    }

    public void setCreated(String created) {
        Created = created;
    }

    public String getCreatedBy() {
        return CreatedBy;
    }

    public void setCreatedBy(String createdBy) {
        CreatedBy = createdBy;
    }

    public String getImage() {
        return Image;
    }

    public void setImage(String image) {
        Image = image;
    }

    public int getGroupType() {
        return GroupType;
    }

    public void setGroupType(int groupType) {
        GroupType = groupType;
    }

    public int getStatus() {
        return Status;
    }

    public void setStatus(int status) {
        Status = status;
    }
}

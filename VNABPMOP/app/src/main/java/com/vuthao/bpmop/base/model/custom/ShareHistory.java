package com.vuthao.bpmop.base.model.custom;

import com.vuthao.bpmop.base.model.app.Status;

public class ShareHistory extends Status {
    private int ID;
    private String UserId; // ID Người Share
    private String UserName;
    private String UserPosition;
    private String UserImagePath;
    private String DateShared;
    private String Comment;
    private int ParentId; // Nếu null thì là người Share - còn lại là người được Share

    public ShareHistory() {
    }

    public int getID() {
        return ID;
    }

    public void setID(int ID) {
        this.ID = ID;
    }

    public String getUserId() {
        return UserId;
    }

    public void setUserId(String userId) {
        UserId = userId;
    }

    public String getUserName() {
        return UserName;
    }

    public void setUserName(String userName) {
        UserName = userName;
    }

    public String getUserPosition() {
        return UserPosition;
    }

    public void setUserPosition(String userPosition) {
        UserPosition = userPosition;
    }

    public String getUserImagePath() {
        return UserImagePath;
    }

    public void setUserImagePath(String userImagePath) {
        UserImagePath = userImagePath;
    }

    public String getDateShared() {
        return DateShared;
    }

    public void setDateShared(String dateShared) {
        DateShared = dateShared;
    }

    public String getComment() {
        return Comment;
    }

    public void setComment(String comment) {
        Comment = comment;
    }

    public int getParentId() {
        return ParentId;
    }

    public void setParentId(int parentId) {
        ParentId = parentId;
    }
}

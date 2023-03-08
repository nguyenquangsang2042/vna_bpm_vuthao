package com.vuthao.bpmop.base.model.custom;

import com.vuthao.bpmop.base.model.app.Status;

import java.util.ArrayList;

public class DetailTask extends Status {
    private Task parentItem;
    private ArrayList<UserAndGroup> assignTo;
    private ArrayList<AttachFile> attachment;
    private ArrayList<Task> childTask;
    private int userPermission;

    public DetailTask() {
    }

    public DetailTask(Task parentItem, ArrayList<UserAndGroup> assignTo, ArrayList<AttachFile> attachment, ArrayList<Task> childTask, int userPermission) {
        this.parentItem = parentItem;
        this.assignTo = assignTo;
        this.attachment = attachment;
        this.childTask = childTask;
        this.userPermission = userPermission;
    }

    public Task getParentItem() {
        return parentItem;
    }

    public void setParentItem(Task parentItem) {
        this.parentItem = parentItem;
    }

    public ArrayList<UserAndGroup> getAssignTo() {
        return assignTo;
    }

    public void setAssignTo(ArrayList<UserAndGroup> assignTo) {
        this.assignTo = assignTo;
    }

    public ArrayList<AttachFile> getAttachment() {
        return attachment;
    }

    public void setAttachment(ArrayList<AttachFile> attachment) {
        this.attachment = attachment;
    }

    public ArrayList<Task> getChildTask() {
        return childTask;
    }

    public void setChildTask(ArrayList<Task> childTask) {
        this.childTask = childTask;
    }

    public int getUserPermission() {
        return userPermission;
    }

    public void setUserPermission(int userPermission) {
        this.userPermission = userPermission;
    }
}

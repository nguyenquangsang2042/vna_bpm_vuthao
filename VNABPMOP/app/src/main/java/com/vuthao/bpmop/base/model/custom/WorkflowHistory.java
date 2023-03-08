package com.vuthao.bpmop.base.model.custom;

import java.util.ArrayList;

public class WorkflowHistory extends com.vuthao.bpmop.base.model.app.Status {
    private String ID;
    private int Step;
    private int WorkflowID;
    private String Title;
    private String TitleEN;
    private int SubmitActionId;
    private String SubmitAction;
    private String SubmitActionEN;
    private String Comment;
    private String Created;
    private String CompletedDate;
    private String AssignUserId;
    private String AssignUserName;
    private String AssignUserAvatar;
    private String AssignDepartmentTitle;
    private String AssignPositionTitle;
    private String FromUserId;
    private String FromUserName;
    private String FromUserAvatar;
    private String FromDepartmentTitle;
    private String FromPositionTitle;
    private int Count; // Để xác định xem là gửi lên lần thứ mấy
    private String ParentId;
    private int Status;
    private ArrayList<WorkflowHistory> ChildHistory;
    private boolean IsSublevel2;
    private String AssignedTo;

    public WorkflowHistory() {
    }

    public String getID() {
        return ID;
    }

    public void setID(String ID) {
        this.ID = ID;
    }

    public int getStep() {
        return Step;
    }

    public void setStep(int step) {
        Step = step;
    }

    public int getWorkflowID() {
        return WorkflowID;
    }

    public void setWorkflowID(int workflowID) {
        WorkflowID = workflowID;
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

    public int getSubmitActionId() {
        return SubmitActionId;
    }

    public void setSubmitActionId(int submitActionId) {
        SubmitActionId = submitActionId;
    }

    public String getSubmitAction() {
        return SubmitAction;
    }

    public void setSubmitAction(String submitAction) {
        SubmitAction = submitAction;
    }

    public String getSubmitActionEN() {
        return SubmitActionEN;
    }

    public void setSubmitActionEN(String submitActionEN) {
        SubmitActionEN = submitActionEN;
    }

    public String getComment() {
        return Comment;
    }

    public void setComment(String comment) {
        Comment = comment;
    }

    public String getCreated() {
        return Created;
    }

    public void setCreated(String created) {
        Created = created;
    }

    public String getCompletedDate() {
        return CompletedDate;
    }

    public void setCompletedDate(String completedDate) {
        CompletedDate = completedDate;
    }

    public String getAssignUserId() {
        return AssignUserId;
    }

    public void setAssignUserId(String assignUserId) {
        AssignUserId = assignUserId;
    }

    public String getAssignUserName() {
        return AssignUserName;
    }

    public void setAssignUserName(String assignUserName) {
        AssignUserName = assignUserName;
    }

    public String getAssignUserAvatar() {
        return AssignUserAvatar;
    }

    public void setAssignUserAvatar(String assignUserAvatar) {
        AssignUserAvatar = assignUserAvatar;
    }

    public String getAssignDepartmentTitle() {
        return AssignDepartmentTitle;
    }

    public void setAssignDepartmentTitle(String assignDepartmentTitle) {
        AssignDepartmentTitle = assignDepartmentTitle;
    }

    public String getAssignPositionTitle() {
        return AssignPositionTitle;
    }

    public void setAssignPositionTitle(String assignPositionTitle) {
        AssignPositionTitle = assignPositionTitle;
    }

    public String getFromUserId() {
        return FromUserId;
    }

    public void setFromUserId(String fromUserId) {
        FromUserId = fromUserId;
    }

    public String getFromUserName() {
        return FromUserName;
    }

    public void setFromUserName(String fromUserName) {
        FromUserName = fromUserName;
    }

    public String getFromUserAvatar() {
        return FromUserAvatar;
    }

    public void setFromUserAvatar(String fromUserAvatar) {
        FromUserAvatar = fromUserAvatar;
    }

    public String getFromDepartmentTitle() {
        return FromDepartmentTitle;
    }

    public void setFromDepartmentTitle(String fromDepartmentTitle) {
        FromDepartmentTitle = fromDepartmentTitle;
    }

    public String getFromPositionTitle() {
        return FromPositionTitle;
    }

    public void setFromPositionTitle(String fromPositionTitle) {
        FromPositionTitle = fromPositionTitle;
    }

    public int getCount() {
        return Count;
    }

    public void setCount(int count) {
        Count = count;
    }

    public String getParentId() {
        return ParentId;
    }

    public void setParentId(String parentId) {
        ParentId = parentId;
    }

    public int isStatus() {
        return Status;
    }

    public void setStatus(int status) {
        Status = status;
    }

    public ArrayList<WorkflowHistory> getChildHistory() {
        return ChildHistory;
    }

    public void setChildHistory(ArrayList<WorkflowHistory> childHistory) {
        ChildHistory = childHistory;
    }

    public boolean isSublevel2() {
        return IsSublevel2;
    }

    public void setSublevel2(boolean sublevel2) {
        IsSublevel2 = sublevel2;
    }

    public String getAssignedTo() {
        return AssignedTo;
    }

    public void setAssignedTo(String assignedTo) {
        AssignedTo = assignedTo;
    }
}

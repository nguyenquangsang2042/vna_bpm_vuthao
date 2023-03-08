package com.vuthao.bpmop.base.model.custom;

import java.util.ArrayList;

public class Task {
    private int ID;
    private int WorkflowId;
    private int SPItemId;
    private int Step;
    private String Title;
    private String Content;
    private String StartDate;
    private String DueDate;
    private String OpenDate;
    private String CompletedDate;
    private long Parent;
    private int Percent;
    private int Status;
    //private String AssignedBy; // Ko xài
    private String AssignedId;
    private String AssignedName; // Nếu trên 2 người trả theo dạng "User A, +1"
    private String AssignedImage;
    private String AssignedPosition;
    private String CreatedBy;
    private String ModifiedBy;
    private String Modified;
    private String Created;
    private boolean IsChange;
    private String CommentChanged; // Lần cuối phiếu cập nhật bình luận (null là lấy lần đầu)
    private String OtherResourceId;
    private ArrayList<Task> ChildTask;
    private boolean IsSublevel2;
    private boolean IsExpand;

    public Task() {
        IsExpand = true;
    }

    public int getID() {
        return ID;
    }

    public void setID(int ID) {
        this.ID = ID;
    }

    public int getWorkflowId() {
        return WorkflowId;
    }

    public void setWorkflowId(int workflowId) {
        WorkflowId = workflowId;
    }

    public int getSPItemId() {
        return SPItemId;
    }

    public void setSPItemId(int SPItemId) {
        this.SPItemId = SPItemId;
    }

    public int getStep() {
        return Step;
    }

    public void setStep(int step) {
        Step = step;
    }

    public String getTitle() {
        return Title;
    }

    public void setTitle(String title) {
        Title = title;
    }

    public String getContent() {
        return Content;
    }

    public void setContent(String content) {
        Content = content;
    }

    public String getStartDate() {
        return StartDate;
    }

    public void setStartDate(String startDate) {
        StartDate = startDate;
    }

    public String getDueDate() {
        return DueDate;
    }

    public void setDueDate(String dueDate) {
        DueDate = dueDate;
    }

    public String getOpenDate() {
        return OpenDate;
    }

    public void setOpenDate(String openDate) {
        OpenDate = openDate;
    }

    public String getCompletedDate() {
        return CompletedDate;
    }

    public void setCompletedDate(String completedDate) {
        CompletedDate = completedDate;
    }

    public long getParent() {
        return Parent;
    }

    public void setParent(long parent) {
        Parent = parent;
    }

    public int getPercent() {
        return Percent;
    }

    public void setPercent(int percent) {
        Percent = percent;
    }

    public int getStatus() {
        return Status;
    }

    public void setStatus(int status) {
        Status = status;
    }

    public String getAssignedId() {
        return AssignedId;
    }

    public void setAssignedId(String assignedId) {
        AssignedId = assignedId;
    }

    public String getAssignedName() {
        return AssignedName;
    }

    public void setAssignedName(String assignedName) {
        AssignedName = assignedName;
    }

    public String getAssignedImage() {
        return AssignedImage;
    }

    public void setAssignedImage(String assignedImage) {
        AssignedImage = assignedImage;
    }

    public String getAssignedPosition() {
        return AssignedPosition;
    }

    public void setAssignedPosition(String assignedPosition) {
        AssignedPosition = assignedPosition;
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

    public String getModified() {
        return Modified;
    }

    public void setModified(String modified) {
        Modified = modified;
    }

    public String getCreated() {
        return Created;
    }

    public void setCreated(String created) {
        Created = created;
    }

    public boolean isChange() {
        return IsChange;
    }

    public void setChange(boolean change) {
        IsChange = change;
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

    public ArrayList<Task> getChildTask() {
        return ChildTask;
    }

    public void setChildTask(ArrayList<Task> childTask) {
        ChildTask = childTask;
    }

    public boolean isSublevel2() {
        return IsSublevel2;
    }

    public void setSublevel2(boolean sublevel2) {
        IsSublevel2 = sublevel2;
    }

    public boolean isExpand() {
        return IsExpand;
    }

    public void setExpand(boolean expand) {
        IsExpand = expand;
    }
}

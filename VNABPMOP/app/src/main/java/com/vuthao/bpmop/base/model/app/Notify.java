package com.vuthao.bpmop.base.model.app;

import io.realm.RealmObject;
import io.realm.annotations.Ignore;
import io.realm.annotations.PrimaryKey;

public class Notify extends RealmObject {
    @PrimaryKey
    private String ID;
    private String Title;
    @Ignore
    private String URL;
    @Ignore
    private String RequestID;
    private int SPItemId;
    private int TaskID;
    private String Category;
    private boolean Type; //0: phối hợp, 1: cần xử lý
    private String SendUnit;
    private int Priority;
    private String EmailUpdate;
    private String AssignedBy;
    private int Status; // 0: chưa hoàn tất, 1: hoàn tất
    private int StatusGroup; // map qua bảng BeanAppStatus
    private String Action;
    private String DueDate;
    private String Content;
    private float Percent;
    private String Note;
    private String Created;
    private String Modified;
    private String StartDate;
    private boolean Notified;
    private boolean Reminder;
    private boolean Read;
    private String CompletedDate;
    private String ListName; // tên list chứa data
    private String ListNameEN; // tên list chứa data EN
    private int HasFile;
    private int Step;
    private String TaskCategory;
    private String TaskSubCategory;
    private String TitleEN;
    private String SubmitAction;
    private int SubmitActionId;
    private String SubmitActionEN;
    private String WFImageURL;//icon hiển thị
    @Ignore
    private boolean IsFollow;
    @Ignore
    private boolean IsSelected;

    public Notify() {
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

    public String getURL() {
        return URL;
    }

    public void setURL(String URL) {
        this.URL = URL;
    }

    public String getRequestID() {
        return RequestID;
    }

    public void setRequestID(String requestID) {
        RequestID = requestID;
    }

    public int getSPItemId() {
        return SPItemId;
    }

    public void setSPItemId(int SPItemId) {
        this.SPItemId = SPItemId;
    }

    public int getTaskID() {
        return TaskID;
    }

    public void setTaskID(int taskID) {
        TaskID = taskID;
    }

    public String getCategory() {
        return Category;
    }

    public void setCategory(String category) {
        Category = category;
    }

    public boolean isType() {
        return Type;
    }

    public void setType(boolean type) {
        Type = type;
    }

    public String getSendUnit() {
        return SendUnit;
    }

    public void setSendUnit(String sendUnit) {
        SendUnit = sendUnit;
    }

    public int getPriority() {
        return Priority;
    }

    public void setPriority(int priority) {
        Priority = priority;
    }

    public String getEmailUpdate() {
        return EmailUpdate;
    }

    public void setEmailUpdate(String emailUpdate) {
        EmailUpdate = emailUpdate;
    }

    public String getAssignedBy() {
        return AssignedBy;
    }

    public void setAssignedBy(String assignedBy) {
        AssignedBy = assignedBy;
    }

    public int getStatus() {
        return Status;
    }

    public void setStatus(int status) {
        Status = status;
    }

    public int getStatusGroup() {
        return StatusGroup;
    }

    public void setStatusGroup(int statusGroup) {
        StatusGroup = statusGroup;
    }

    public String getAction() {
        return Action;
    }

    public void setAction(String action) {
        Action = action;
    }

    public String getDueDate() {
        return DueDate;
    }

    public void setDueDate(String dueDate) {
        DueDate = dueDate;
    }

    public String getContent() {
        return Content;
    }

    public void setContent(String content) {
        Content = content;
    }

    public float getPercent() {
        return Percent;
    }

    public void setPercent(float percent) {
        Percent = percent;
    }

    public String getNote() {
        return Note;
    }

    public void setNote(String note) {
        Note = note;
    }

    public String getCreated() {
        return Created;
    }

    public void setCreated(String created) {
        Created = created;
    }

    public String getModified() {
        return Modified;
    }

    public void setModified(String modified) {
        Modified = modified;
    }

    public String getStartDate() {
        return StartDate;
    }

    public void setStartDate(String startDate) {
        StartDate = startDate;
    }

    public boolean isNotified() {
        return Notified;
    }

    public void setNotified(boolean notified) {
        Notified = notified;
    }

    public boolean isReminder() {
        return Reminder;
    }

    public void setReminder(boolean reminder) {
        Reminder = reminder;
    }

    public boolean isRead() {
        return Read;
    }

    public void setRead(boolean read) {
        Read = read;
    }

    public String getCompletedDate() {
        return CompletedDate;
    }

    public void setCompletedDate(String completedDate) {
        CompletedDate = completedDate;
    }

    public String getListName() {
        return ListName;
    }

    public void setListName(String listName) {
        ListName = listName;
    }

    public String getListNameEN() {
        return ListNameEN;
    }

    public void setListNameEN(String listNameEN) {
        ListNameEN = listNameEN;
    }

    public int isHasFile() {
        return HasFile;
    }

    public void setHasFile(int hasFile) {
        HasFile = hasFile;
    }

    public int getStep() {
        return Step;
    }

    public void setStep(int step) {
        Step = step;
    }

    public String getTaskCategory() {
        return TaskCategory;
    }

    public void setTaskCategory(String taskCategory) {
        TaskCategory = taskCategory;
    }

    public String getTaskSubCategory() {
        return TaskSubCategory;
    }

    public void setTaskSubCategory(String taskSubCategory) {
        TaskSubCategory = taskSubCategory;
    }

    public String getTitleEN() {
        return TitleEN;
    }

    public void setTitleEN(String titleEN) {
        TitleEN = titleEN;
    }

    public String getSubmitAction() {
        return SubmitAction;
    }

    public void setSubmitAction(String submitAction) {
        SubmitAction = submitAction;
    }

    public int getSubmitActionId() {
        return SubmitActionId;
    }

    public void setSubmitActionId(int submitActionId) {
        SubmitActionId = submitActionId;
    }

    public String getSubmitActionEN() {
        return SubmitActionEN;
    }

    public void setSubmitActionEN(String submitActionEN) {
        SubmitActionEN = submitActionEN;
    }

    public String getWFImageURL() {
        return WFImageURL;
    }

    public void setWFImageURL(String WFImageURL) {
        this.WFImageURL = WFImageURL;
    }

    public boolean isFollow() {
        return IsFollow;
    }

    public void setFollow(boolean follow) {
        IsFollow = follow;
    }

    public boolean isSelected() {
        return IsSelected;
    }

    public void setSelected(boolean selected) {
        IsSelected = selected;
    }
}

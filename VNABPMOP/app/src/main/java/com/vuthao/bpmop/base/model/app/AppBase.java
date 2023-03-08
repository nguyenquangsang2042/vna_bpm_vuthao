package com.vuthao.bpmop.base.model.app;

import com.vuthao.bpmop.base.model.custom.UserAndGroup;

import java.util.ArrayList;

import io.realm.RealmObject;
import io.realm.annotations.Ignore;
import io.realm.annotations.PrimaryKey;

public class AppBase extends RealmObject {
    @PrimaryKey
    private int ID;
    private String Title;
    private String TitleEN;
    private String Content;
    private String AssignedTo;
    private int Status;
    private int StatusGroup;
    private int ResourceCategoryId; // 16:Task || 8:WorkFlow
    private int ResourceSubCategoryId; // Định danh cho ResourceCategoryId (ResourceCategoryId = 16 và ResourceSubCategoryId = 0 : Task)
    private String CommentChanged;
    private String OtherResourceId;
    private String CreatedBy;
    private String ModifiedBy;
    private String Permission; // List ID những user có quyền
    private int WorkflowId;
    @Ignore
    private String NotifyId;
    @Ignore
    private AppStatus appStatus;
    @Ignore
    private UserAndGroup user;
    @Ignore
    private Workflow workflow;
    @Ignore
    private WorkflowStatus workflowStatus;
    @Ignore
    private int ApprovalStatus;
    @Ignore
    private int WorkflowID;
    private String NotifiedUsers;
    private int FileCount;
    private int CommentCount;
    private String ItemUrl;
    private int Step;           // xac dinh buoc cua phieu tren board
    private int AppFlg;
    private String AssignedBy;
    private String Created;
    private String DueDate;
    private String Modified;
    @Ignore
    private String StartDate;
    @Ignore
    private boolean Read;
    private int SPItemId;
    private String SubmitAction;
    private int SubmitActionId;
    private String SubmitActionEN;
    private String SendUnit;
    private String ListName;
    private String Attendees;
    private String WorkflowTitle;
    private boolean HasFile;
    private int Priority;
    private String NotifyCreated;
    @Ignore
    private int IsFollow;
    @Ignore
    private ArrayList<AppBase> Data;
    @Ignore
    private AppBase MoreInfo;
    @Ignore
    private int totalRecord;

    public AppBase() {
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

    public String getContent() {
        return Content;
    }

    public void setContent(String content) {
        Content = content;
    }

    public String getAssignedTo() {
        return AssignedTo;
    }

    public void setAssignedTo(String assignedTo) {
        AssignedTo = assignedTo;
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

    public int getResourceCategoryId() {
        return ResourceCategoryId;
    }

    public void setResourceCategoryId(int resourceCategoryId) {
        ResourceCategoryId = resourceCategoryId;
    }

    public int getResourceSubCategoryId() {
        return ResourceSubCategoryId;
    }

    public void setResourceSubCategoryId(int resourceSubCategoryId) {
        ResourceSubCategoryId = resourceSubCategoryId;
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

    public String getPermission() {
        return Permission;
    }

    public void setPermission(String permission) {
        Permission = permission;
    }

    public int getWorkflowId() {
        return WorkflowId;
    }

    public void setWorkflowId(int workflowId) {
        WorkflowId = workflowId;
    }

    public String getNotifiedUsers() {
        return NotifiedUsers;
    }

    public void setNotifiedUsers(String notifiedUsers) {
        NotifiedUsers = notifiedUsers;
    }

    public int getFileCount() {
        return FileCount;
    }

    public void setFileCount(int fileCount) {
        FileCount = fileCount;
    }

    public int getCommentCount() {
        return CommentCount;
    }

    public void setCommentCount(int commentCount) {
        CommentCount = commentCount;
    }

    public String getItemUrl() {
        return ItemUrl;
    }

    public void setItemUrl(String itemUrl) {
        ItemUrl = itemUrl;
    }

    public int getStep() {
        return Step;
    }

    public void setStep(int step) {
        Step = step;
    }

    public int getAppFlg() {
        return AppFlg;
    }

    public void setAppFlg(int appFlg) {
        AppFlg = appFlg;
    }

    public String getAssignedBy() {
        return AssignedBy;
    }

    public void setAssignedBy(String assignedBy) {
        AssignedBy = assignedBy;
    }

    public String getCreated() {
        return Created;
    }

    public void setCreated(String created) {
        Created = created;
    }

    public String getDueDate() {
        return DueDate;
    }

    public void setDueDate(String dueDate) {
        DueDate = dueDate;
    }

    public String getModified() {
        return Modified;
    }

    public void setModified(String modified) {
        Modified = modified;
    }

    public int isFollow() {
        return IsFollow;
    }

    public void setFollow(int follow) {
        IsFollow = follow;
    }

    public String getStartDate() {
        return StartDate;
    }

    public void setStartDate(String startDate) {
        StartDate = startDate;
    }

    public boolean isRead() {
        return Read;
    }

    public void setRead(boolean read) {
        Read = read;
    }

    public int getSPItemId() {
        return SPItemId;
    }

    public void setSPItemId(int SPItemId) {
        this.SPItemId = SPItemId;
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

    public String getSendUnit() {
        return SendUnit;
    }

    public void setSendUnit(String sendUnit) {
        SendUnit = sendUnit;
    }

    public String getListName() {
        return ListName;
    }

    public void setListName(String listName) {
        ListName = listName;
    }

    public boolean isHasFile() {
        return HasFile;
    }

    public void setHasFile(boolean hasFile) {
        HasFile = hasFile;
    }

    public int getPriority() {
        return Priority;
    }

    public void setPriority(int priority) {
        Priority = priority;
    }

    public String getNotifyCreated() {
        return NotifyCreated;
    }

    public void setNotifyCreated(String notifyCreated) {
        NotifyCreated = notifyCreated;
    }

    public ArrayList<AppBase> getData() {
        return Data;
    }

    public void setData(ArrayList<AppBase> data) {
        Data = data;
    }

    public AppBase getMoreInfo() {
        return MoreInfo;
    }

    public void setMoreInfo(AppBase moreInfo) {
        MoreInfo = moreInfo;
    }

    public int getTotalRecord() {
        return totalRecord;
    }

    public void setTotalRecord(int totalRecord) {
        this.totalRecord = totalRecord;
    }

    public String getAttendees() {
        return Attendees;
    }

    public void setAttendees(String attendees) {
        Attendees = attendees;
    }

    public int getIsFollow() {
        return IsFollow;
    }

    public void setIsFollow(int isFollow) {
        IsFollow = isFollow;
    }

    public String getNotifyId() {
        return NotifyId;
    }

    public void setNotifyId(String notifyId) {
        NotifyId = notifyId;
    }

    public AppStatus getAppStatus() {
        return appStatus;
    }

    public void setAppStatus(AppStatus appStatus) {
        this.appStatus = appStatus;
    }

    public UserAndGroup getUser() {
        return user;
    }

    public void setUser(UserAndGroup user) {
        this.user = user;
    }

    public Workflow getWorkflow() {
        return workflow;
    }

    public void setWorkflow(Workflow workflow) {
        this.workflow = workflow;
    }

    public String getWorkflowTitle() {
        return WorkflowTitle;
    }

    public void setWorkflowTitle(String workflowTitle) {
        WorkflowTitle = workflowTitle;
    }

    public WorkflowStatus getWorkflowStatus() {
        return workflowStatus;
    }

    public void setWorkflowStatus(WorkflowStatus workflowStatus) {
        this.workflowStatus = workflowStatus;
    }

    public int getApprovalStatus() {
        return ApprovalStatus;
    }

    public void setApprovalStatus(int approvalStatus) {
        ApprovalStatus = approvalStatus;
    }

    public int getWorkflowID() {
        return WorkflowID;
    }

    public void setWorkflowID(int workflowID) {
        WorkflowID = workflowID;
    }
}

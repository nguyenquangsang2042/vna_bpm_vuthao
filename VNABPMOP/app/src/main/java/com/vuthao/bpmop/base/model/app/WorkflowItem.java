package com.vuthao.bpmop.base.model.app;

import io.realm.RealmObject;
import io.realm.annotations.Ignore;
import io.realm.annotations.PrimaryKey;

public class WorkflowItem extends RealmObject {
    @PrimaryKey
    private String ID;
    private int ItemID;
    private int WorkflowID;
    private String WorkflowTitle;
    private String WorkflowTitleEN;
    private String Title;
    private String ListName;
    private String SiteUrl;
    private String IssueDate;
    private String Content;
    private String CreatedBy;
    private String CreatedByName;
    private String ApprovalDate;
    private String DonViNguoiNhanTien;
    private long Total;
    private String Status;
    private int StatusGroup;
    private int ActionStatusID;
    private String ActionStatus;
    private String ActionStatusEN;
    private String Approval;
    private String ApprovalName;
    private String Created;
    private String DueDate;
    private String ListId;
    private String AssignedToName; // Fullname, Fullname, Fullname
    private String AssignedTo; // ID, ID, ID
    private String Modified;
    private int Step;
    private String WorkflowStep;
    private boolean HasFile;
    private String TicketRequestDetails; //json ticket details
    private String WFImageURL;//icon hiển thị
    private boolean IsInfo; // đang chờ bổ sung thông tin
    private boolean IsConsul; // đang chờ tham vấn
    private boolean IsChange; // Phiếu có thay đổi giá trị form động hay không
    private String CommentChanged; // Lần cuối phiếu cập nhật bình luận (null là lấy lần đầu)

    @Ignore
    private boolean IsFollow;
    @Ignore
    private boolean Read;
    @Ignore
    private boolean IsSelected;
    private int FileCount;
    private int CommentCount;
    @Ignore
    private String GroupName;
    @Ignore
    private String OtherResourceId;

    public WorkflowItem() {
        // lần đầu lấy về là true
        setChange(true);
    }

    public WorkflowItem(boolean isChange) {
        IsChange = isChange;
    }

    public String getID() {
        return ID;
    }

    public void setID(String ID) {
        this.ID = ID;
    }

    public int getItemID() {
        return ItemID;
    }

    public void setItemID(int itemID) {
        ItemID = itemID;
    }

    public int getWorkflowID() {
        return WorkflowID;
    }

    public void setWorkflowID(int workflowID) {
        WorkflowID = workflowID;
    }

    public String getWorkflowTitle() {
        return WorkflowTitle;
    }

    public void setWorkflowTitle(String workflowTitle) {
        WorkflowTitle = workflowTitle;
    }

    public String getWorkflowTitleEN() {
        return WorkflowTitleEN;
    }

    public void setWorkflowTitleEN(String workflowTitleEN) {
        WorkflowTitleEN = workflowTitleEN;
    }

    public String getTitle() {
        return Title;
    }

    public void setTitle(String title) {
        Title = title;
    }

    public String getListName() {
        return ListName;
    }

    public void setListName(String listName) {
        ListName = listName;
    }

    public String getSiteUrl() {
        return SiteUrl;
    }

    public void setSiteUrl(String siteUrl) {
        SiteUrl = siteUrl;
    }

    public String getIssueDate() {
        return IssueDate;
    }

    public void setIssueDate(String issueDate) {
        IssueDate = issueDate;
    }

    public String getContent() {
        return Content;
    }

    public void setContent(String content) {
        Content = content;
    }

    public String getCreatedBy() {
        return CreatedBy;
    }

    public void setCreatedBy(String createdBy) {
        CreatedBy = createdBy;
    }

    public String getCreatedByName() {
        return CreatedByName;
    }

    public void setCreatedByName(String createdByName) {
        CreatedByName = createdByName;
    }

    public String getApprovalDate() {
        return ApprovalDate;
    }

    public void setApprovalDate(String approvalDate) {
        ApprovalDate = approvalDate;
    }

    public String getDonViNguoiNhanTien() {
        return DonViNguoiNhanTien;
    }

    public void setDonViNguoiNhanTien(String donViNguoiNhanTien) {
        DonViNguoiNhanTien = donViNguoiNhanTien;
    }

    public long getTotal() {
        return Total;
    }

    public void setTotal(long total) {
        Total = total;
    }

    public String getStatus() {
        return Status;
    }

    public void setStatus(String status) {
        Status = status;
    }

    public int getStatusGroup() {
        return StatusGroup;
    }

    public void setStatusGroup(int statusGroup) {
        StatusGroup = statusGroup;
    }

    public int getActionStatusID() {
        return ActionStatusID;
    }

    public void setActionStatusID(int actionStatusID) {
        ActionStatusID = actionStatusID;
    }

    public String getActionStatus() {
        return ActionStatus;
    }

    public void setActionStatus(String actionStatus) {
        ActionStatus = actionStatus;
    }

    public String getActionStatusEN() {
        return ActionStatusEN;
    }

    public void setActionStatusEN(String actionStatusEN) {
        ActionStatusEN = actionStatusEN;
    }

    public String getApproval() {
        return Approval;
    }

    public void setApproval(String approval) {
        Approval = approval;
    }

    public String getApprovalName() {
        return ApprovalName;
    }

    public void setApprovalName(String approvalName) {
        ApprovalName = approvalName;
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

    public String getListId() {
        return ListId;
    }

    public void setListId(String listId) {
        ListId = listId;
    }

    public String getAssignedToName() {
        return AssignedToName;
    }

    public void setAssignedToName(String assignedToName) {
        AssignedToName = assignedToName;
    }

    public String getAssignedTo() {
        return AssignedTo;
    }

    public void setAssignedTo(String assignedTo) {
        AssignedTo = assignedTo;
    }

    public String getModified() {
        return Modified;
    }

    public void setModified(String modified) {
        Modified = modified;
    }

    public int getStep() {
        return Step;
    }

    public void setStep(int step) {
        Step = step;
    }

    public String getWorkflowStep() {
        return WorkflowStep;
    }

    public void setWorkflowStep(String workflowStep) {
        WorkflowStep = workflowStep;
    }

    public boolean isHasFile() {
        return HasFile;
    }

    public void setHasFile(boolean hasFile) {
        HasFile = hasFile;
    }

    public String getTicketRequestDetails() {
        return TicketRequestDetails;
    }

    public void setTicketRequestDetails(String ticketRequestDetails) {
        TicketRequestDetails = ticketRequestDetails;
    }

    public String getWFImageURL() {
        return WFImageURL;
    }

    public void setWFImageURL(String WFImageURL) {
        this.WFImageURL = WFImageURL;
    }

    public boolean isInfo() {
        return IsInfo;
    }

    public void setInfo(boolean info) {
        IsInfo = info;
    }

    public boolean isConsul() {
        return IsConsul;
    }

    public void setConsul(boolean consul) {
        IsConsul = consul;
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

    public boolean isFollow() {
        return IsFollow;
    }

    public void setFollow(boolean follow) {
        IsFollow = follow;
    }

    public boolean isRead() {
        return Read;
    }

    public void setRead(boolean read) {
        Read = read;
    }

    public boolean isSelected() {
        return IsSelected;
    }

    public void setSelected(boolean selected) {
        IsSelected = selected;
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

    public String getGroupName() {
        return GroupName;
    }

    public void setGroupName(String groupName) {
        GroupName = groupName;
    }

    public String getOtherResourceId() {
        return OtherResourceId;
    }

    public void setOtherResourceId(String otherResourceId) {
        OtherResourceId = otherResourceId;
    }
}

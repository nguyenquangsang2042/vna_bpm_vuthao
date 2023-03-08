package com.vuthao.bpmop.base.model.custom;

import java.util.ArrayList;

import io.realm.RealmObject;
import io.realm.annotations.Ignore;
import io.realm.annotations.PrimaryKey;

public class Comment extends RealmObject {
    @PrimaryKey
    private String ID;
    private int CID;
    private String Content;
    private String Image;
    private String ResourceId;
    private int ResourceCategoryId;
    private int ResourceSubCategoryId;
    private String ParentCommentId;
    private int Status;        // 0: Đang chờ duyệt; 1: Đã phê duyệt; -1 là từ chối. (Chỉ riêng tin tức mới có)
    private int LikeCount;
    private int CommentCount;
    private String TagUsers;
    private boolean FlgChanged;
    private int IsLiked;// true la user đang like comment này
    private String Author;
    private String AttachFiles;
    private String Approver;   // Người phê duyệt. (Chỉ riêng tin tức mới có)
    private String Modified;
    private String Created;
    private String ApprovedDate; // Ngày phê duyệt. (Chỉ riêng tin tức mới có)
    @Ignore
    private String ApproverName;   // FullName người duyệt
    @Ignore
    private String ApproverPosition;
    @Ignore
    private String AccountID;
    @Ignore
    private String AccountName;
    @Ignore
    private String AuthorPosition;
    @Ignore
    private String FullName;
    @Ignore
    private String ImagePath;
    @Ignore
    private String ResourceTitle;
    @Ignore
    private String ResourceUrl;
    @Ignore
    private int ItemId;// ItemId trên site của Reosurce
    @Ignore
    private ArrayList<Comment> lstChildComments;

    public Comment() {
    }

    public String getID() {
        return ID;
    }

    public void setID(String ID) {
        this.ID = ID;
    }

    public int getCID() {
        return CID;
    }

    public void setCID(int CID) {
        this.CID = CID;
    }

    public String getContent() {
        return Content;
    }

    public void setContent(String content) {
        Content = content;
    }

    public String getImage() {
        return Image;
    }

    public void setImage(String image) {
        Image = image;
    }

    public String getResourceId() {
        return ResourceId;
    }

    public void setResourceId(String resourceId) {
        ResourceId = resourceId;
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

    public String getParentCommentId() {
        return ParentCommentId;
    }

    public void setParentCommentId(String parentCommentId) {
        ParentCommentId = parentCommentId;
    }

    public int getStatus() {
        return Status;
    }

    public void setStatus(int status) {
        Status = status;
    }

    public int getLikeCount() {
        return LikeCount;
    }

    public void setLikeCount(int likeCount) {
        LikeCount = likeCount;
    }

    public int getCommentCount() {
        return CommentCount;
    }

    public void setCommentCount(int commentCount) {
        CommentCount = commentCount;
    }

    public String getTagUsers() {
        return TagUsers;
    }

    public void setTagUsers(String tagUsers) {
        TagUsers = tagUsers;
    }

    public boolean isFlgChanged() {
        return FlgChanged;
    }

    public void setFlgChanged(boolean flgChanged) {
        FlgChanged = flgChanged;
    }

    public int isLiked() {
        return IsLiked;
    }

    public void setLiked(int liked) {
        IsLiked = liked;
    }

    public String getAuthor() {
        return Author;
    }

    public void setAuthor(String author) {
        Author = author;
    }

    public String getAttachFiles() {
        return AttachFiles;
    }

    public void setAttachFiles(String attachFiles) {
        AttachFiles = attachFiles;
    }

    public String getApprover() {
        return Approver;
    }

    public void setApprover(String approver) {
        Approver = approver;
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

    public String getApprovedDate() {
        return ApprovedDate;
    }

    public void setApprovedDate(String approvedDate) {
        ApprovedDate = approvedDate;
    }

    public String getApproverName() {
        return ApproverName;
    }

    public void setApproverName(String approverName) {
        ApproverName = approverName;
    }

    public String getApproverPosition() {
        return ApproverPosition;
    }

    public void setApproverPosition(String approverPosition) {
        ApproverPosition = approverPosition;
    }

    public String getAccountID() {
        return AccountID;
    }

    public void setAccountID(String accountID) {
        AccountID = accountID;
    }

    public String getAccountName() {
        return AccountName;
    }

    public void setAccountName(String accountName) {
        AccountName = accountName;
    }

    public String getAuthorPosition() {
        return AuthorPosition;
    }

    public void setAuthorPosition(String authorPosition) {
        AuthorPosition = authorPosition;
    }

    public String getFullName() {
        return FullName;
    }

    public void setFullName(String fullName) {
        FullName = fullName;
    }

    public String getImagePath() {
        return ImagePath;
    }

    public void setImagePath(String imagePath) {
        ImagePath = imagePath;
    }

    public String getResourceTitle() {
        return ResourceTitle;
    }

    public void setResourceTitle(String resourceTitle) {
        ResourceTitle = resourceTitle;
    }

    public String getResourceUrl() {
        return ResourceUrl;
    }

    public void setResourceUrl(String resourceUrl) {
        ResourceUrl = resourceUrl;
    }

    public int getItemId() {
        return ItemId;
    }

    public void setItemId(int itemId) {
        ItemId = itemId;
    }

    public ArrayList<Comment> getLstChildComments() {
        return lstChildComments;
    }

    public void setLstChildComments(ArrayList<Comment> lstChildComments) {
        this.lstChildComments = lstChildComments;
    }
}
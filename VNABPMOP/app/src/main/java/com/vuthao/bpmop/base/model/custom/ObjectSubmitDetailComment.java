package com.vuthao.bpmop.base.model.custom;

public class ObjectSubmitDetailComment {
    private String ID;
    private String DeviceName; // Android: "App Android" - IOS: "App IOS"
    private String CodeName; // giống DeviceName bỏ khoảng trắng: Android: "AppAndroid" - IOS: "AppIOS"
    private String AppName; // Ex: "Digital Process"
    private String Platform; // Tên hệ điều hành
    private String ResourceCategoryId; // 8 WFItem - 16 Task
    private String ResourceUrl; // Lấy trong BeanSettings - Task: TASK_URL - WFItem: WORKFLOW_URL
    private String ItemId; // WFItem ID phiếu - Task ID Task
    private String Author; // ID người tạo
    private String AuthorName;

    public ObjectSubmitDetailComment() {
    }

    public String getID() {
        return ID;
    }

    public void setID(String ID) {
        this.ID = ID;
    }

    public String getDeviceName() {
        return DeviceName;
    }

    public void setDeviceName(String deviceName) {
        DeviceName = deviceName;
    }

    public String getCodeName() {
        return CodeName;
    }

    public void setCodeName(String codeName) {
        CodeName = codeName;
    }

    public String getAppName() {
        return AppName;
    }

    public void setAppName(String appName) {
        AppName = appName;
    }

    public String getPlatform() {
        return Platform;
    }

    public void setPlatform(String platform) {
        Platform = platform;
    }

    public String getResourceCategoryId() {
        return ResourceCategoryId;
    }

    public void setResourceCategoryId(String resourceCategoryId) {
        ResourceCategoryId = resourceCategoryId;
    }

    public String getResourceUrl() {
        return ResourceUrl;
    }

    public void setResourceUrl(String resourceUrl) {
        ResourceUrl = resourceUrl;
    }

    public String getItemId() {
        return ItemId;
    }

    public void setItemId(String itemId) {
        ItemId = itemId;
    }

    public String getAuthor() {
        return Author;
    }

    public void setAuthor(String author) {
        Author = author;
    }

    public String getAuthorName() {
        return AuthorName;
    }

    public void setAuthorName(String authorName) {
        AuthorName = authorName;
    }
}

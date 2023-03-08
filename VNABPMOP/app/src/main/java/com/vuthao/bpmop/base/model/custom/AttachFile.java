package com.vuthao.bpmop.base.model.custom;

public class AttachFile {
    private String ID;
    private String Path;
    private String Title;
    private String Category;
    private String Type;
    private long Size;
    private String CreatedBy;
    private boolean IsAuthor;
    private int WorkflowId;
    private int WorkflowItemId;
    private String Name;
    private int AttachTypeId;
    private boolean IsImage; // để phân biệt xem là file ảnh hay file loại khác
    private String AttachTypeName;
    private String Extension;
    private String Url; // URL file trên server - xài trong trường hợp file comment
    private boolean QRCode;
    private boolean SignCA;
    private boolean GenerateBy;
    private double Index; //Do có sử dụng trên list sharepoint nữa, nên để double cho đồng bộ :3
    private String Modified;
    private String ModifiedBy;
    private String ModifiedPositon;
    private String ModifiedName;
    private String Created;
    private String CreatedPositon;
    private String CreatedName;

    public AttachFile() {
    }

    public String getID() {
        return ID;
    }

    public void setID(String ID) {
        this.ID = ID;
    }

    public String getPath() {
        return Path;
    }

    public void setPath(String path) {
        Path = path;
    }

    public String getTitle() {
        return Title;
    }

    public void setTitle(String title) {
        Title = title;
    }

    public String getCategory() {
        return Category;
    }

    public void setCategory(String category) {
        Category = category;
    }

    public String getType() {
        return Type;
    }

    public void setType(String type) {
        Type = type;
    }

    public long getSize() {
        return Size;
    }

    public void setSize(long size) {
        Size = size;
    }

    public String getCreatedBy() {
        return CreatedBy;
    }

    public void setCreatedBy(String createdBy) {
        CreatedBy = createdBy;
    }

    public boolean isAuthor() {
        return IsAuthor;
    }

    public void setAuthor(boolean author) {
        IsAuthor = author;
    }

    public int getWorkflowId() {
        return WorkflowId;
    }

    public void setWorkflowId(int workflowId) {
        WorkflowId = workflowId;
    }

    public int getWorkflowItemId() {
        return WorkflowItemId;
    }

    public void setWorkflowItemId(int workflowItemId) {
        WorkflowItemId = workflowItemId;
    }

    public String getName() {
        return Name;
    }

    public void setName(String name) {
        Name = name;
    }

    public int getAttachTypeId() {
        return AttachTypeId;
    }

    public void setAttachTypeId(int attachTypeId) {
        AttachTypeId = attachTypeId;
    }

    public boolean isImage() {
        return IsImage;
    }

    public void setImage(boolean image) {
        IsImage = image;
    }

    public String getAttachTypeName() {
        return AttachTypeName;
    }

    public void setAttachTypeName(String attachTypeName) {
        AttachTypeName = attachTypeName;
    }

    public String getExtension() {
        return Extension;
    }

    public void setExtension(String extension) {
        Extension = extension;
    }

    public String getUrl() {
        return Url;
    }

    public void setUrl(String url) {
        Url = url;
    }

    public boolean isQRCode() {
        return QRCode;
    }

    public void setQRCode(boolean QRCode) {
        this.QRCode = QRCode;
    }

    public boolean isSignCA() {
        return SignCA;
    }

    public void setSignCA(boolean signCA) {
        SignCA = signCA;
    }

    public boolean isGenerateBy() {
        return GenerateBy;
    }

    public void setGenerateBy(boolean generateBy) {
        GenerateBy = generateBy;
    }

    public double getIndex() {
        return Index;
    }

    public void setIndex(double index) {
        Index = index;
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

    public String getModifiedPositon() {
        return ModifiedPositon;
    }

    public void setModifiedPositon(String modifiedPositon) {
        ModifiedPositon = modifiedPositon;
    }

    public String getModifiedName() {
        return ModifiedName;
    }

    public void setModifiedName(String modifiedName) {
        ModifiedName = modifiedName;
    }

    public String getCreated() {
        return Created;
    }

    public void setCreated(String created) {
        Created = created;
    }

    public String getCreatedPositon() {
        return CreatedPositon;
    }

    public void setCreatedPositon(String createdPositon) {
        CreatedPositon = createdPositon;
    }

    public String getCreatedName() {
        return CreatedName;
    }

    public void setCreatedName(String createdName) {
        CreatedName = createdName;
    }
}

package com.vuthao.bpmop.base.model.custom;

public class OtherResource {
    private String Content;
    private String ResourceId; // OtherResourceID
    private int ResourceCategoryId; // 8 WFItem - 16 Task
    private int ResourceSubCategoryId; // 8 WFItem - 16 Task
    private String Image;
    private String ParentCommentId; // Parent: null - Child: parentID

    public OtherResource() {
    }

    public String getContent() {
        return Content;
    }

    public void setContent(String content) {
        Content = content;
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

    public String getImage() {
        return Image;
    }

    public void setImage(String image) {
        Image = image;
    }

    public String getParentCommentId() {
        return ParentCommentId;
    }

    public void setParentCommentId(String parentCommentId) {
        ParentCommentId = parentCommentId;
    }
}

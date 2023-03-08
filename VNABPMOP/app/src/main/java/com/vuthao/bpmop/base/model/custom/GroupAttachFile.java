package com.vuthao.bpmop.base.model.custom;

import java.util.ArrayList;

public class GroupAttachFile {
    private String Category;
    private ArrayList<AttachFile> AttachFiles;

    public GroupAttachFile(String category, ArrayList<AttachFile> attachFiles) {
        Category = category;
        AttachFiles = attachFiles;
    }

    public GroupAttachFile() {
    }

    public String getCategory() {
        return Category;
    }

    public void setCategory(String category) {
        Category = category;
    }

    public ArrayList<AttachFile> getAttachFiles() {
        return AttachFiles;
    }

    public void setAttachFiles(ArrayList<AttachFile> attachFiles) {
        AttachFiles = attachFiles;
    }
}

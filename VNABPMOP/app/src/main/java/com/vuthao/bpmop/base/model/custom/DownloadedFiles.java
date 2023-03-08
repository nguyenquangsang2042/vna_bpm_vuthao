package com.vuthao.bpmop.base.model.custom;

import io.realm.RealmObject;
import io.realm.annotations.PrimaryKey;

public class DownloadedFiles {
    private String ID;
    private String Title;
    private String Path;
    private long Size;
    private String DownloadedBy;
    private String Extension;
    private long Modified;
    private long Created;

    public DownloadedFiles() {
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

    public String getPath() {
        return Path;
    }

    public void setPath(String path) {
        Path = path;
    }

    public long getSize() {
        return Size;
    }

    public void setSize(long size) {
        Size = size;
    }

    public String getExtension() {
        return Extension;
    }

    public void setExtension(String extension) {
        Extension = extension;
    }

    public long getModified() {
        return Modified;
    }

    public void setModified(long modified) {
        Modified = modified;
    }

    public long getCreated() {
        return Created;
    }

    public void setCreated(long created) {
        Created = created;
    }

    public String getDownloadedBy() {
        return DownloadedBy;
    }

    public void setDownloadedBy(String downloadedBy) {
        DownloadedBy = downloadedBy;
    }
}

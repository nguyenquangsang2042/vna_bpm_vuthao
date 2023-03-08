package com.vuthao.bpmop.base.model.app;

import io.realm.RealmObject;
import io.realm.annotations.PrimaryKey;

public class WorkflowFollow extends RealmObject {
    @PrimaryKey
    private int WorkflowItemId;
    private String UserId;
    private int Status;
    private String Created;
    private String Modified;

    public WorkflowFollow() {
    }

    public int getWorkflowItemId() {
        return WorkflowItemId;
    }

    public void setWorkflowItemId(int workflowItemId) {
        WorkflowItemId = workflowItemId;
    }

    public String getUserId() {
        return UserId;
    }

    public void setUserId(String userId) {
        UserId = userId;
    }

    public int getStatus() {
        return Status;
    }

    public void setStatus(int status) {
        Status = status;
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
}

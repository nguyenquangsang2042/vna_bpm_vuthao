package com.vuthao.bpmop.base.model.custom;

import com.vuthao.bpmop.base.model.app.Workflow;
import com.vuthao.bpmop.base.model.app.WorkflowCategory;

import java.util.ArrayList;

public class BoardWorkflow {
    private WorkflowCategory workflowCategory;
    private ArrayList<Workflow> workflows;
    private boolean IsExpand;

    public BoardWorkflow(WorkflowCategory workflowCategory, ArrayList<Workflow> workflows) {
        this.workflowCategory = workflowCategory;
        this.workflows = workflows;
    }

    public BoardWorkflow() {
    }

    public WorkflowCategory getWorkflowCategory() {
        return workflowCategory;
    }

    public void setWorkflowCategory(WorkflowCategory workflowCategory) {
        this.workflowCategory = workflowCategory;
    }

    public ArrayList<Workflow> getWorkflows() {
        return workflows;
    }

    public void setWorkflows(ArrayList<Workflow> workflows) {
        this.workflows = workflows;
    }

    public boolean isExpand() {
        return IsExpand;
    }

    public void setExpand(boolean expand) {
        IsExpand = expand;
    }
}

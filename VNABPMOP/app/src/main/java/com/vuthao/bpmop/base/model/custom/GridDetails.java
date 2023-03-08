package com.vuthao.bpmop.base.model.custom;

import com.vuthao.bpmop.base.model.app.Status;
import com.vuthao.bpmop.base.model.dynamic.ViewRow;

import java.util.ArrayList;

public class GridDetails extends Status {
    private ArrayList<ViewRow> attachment;
    private ArrayList<ViewRow> details;
    private ArrayList<WorkFlowRelated> related;
    private ArrayList<Task> task;

    public GridDetails() {
    }

    public ArrayList<ViewRow> getAttachment() {
        return attachment;
    }

    public void setAttachment(ArrayList<ViewRow> attachment) {
        this.attachment = attachment;
    }

    public ArrayList<ViewRow> getDetails() {
        return details;
    }

    public void setDetails(ArrayList<ViewRow> details) {
        this.details = details;
    }

    public ArrayList<WorkFlowRelated> getRelated() {
        return related;
    }

    public void setRelated(ArrayList<WorkFlowRelated> related) {
        this.related = related;
    }

    public ArrayList<Task> getTask() {
        return task;
    }

    public void setTask(ArrayList<Task> task) {
        this.task = task;
    }
}

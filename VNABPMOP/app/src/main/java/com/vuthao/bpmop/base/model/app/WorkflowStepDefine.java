package com.vuthao.bpmop.base.model.app;

import io.realm.RealmObject;
import io.realm.annotations.PrimaryKey;

public class WorkflowStepDefine extends RealmObject {
    @PrimaryKey
    private int WorkflowStepDefineID;
    private int WorkflowID;
    private int SubWorkflowID;
    private int Step;
    private String Title;
    private int Duration;
    private String TimeUnit;
    private String EnterDay;

    public WorkflowStepDefine() {
    }

    public int getWorkflowStepDefineID() {
        return WorkflowStepDefineID;
    }

    public void setWorkflowStepDefineID(int workflowStepDefineID) {
        WorkflowStepDefineID = workflowStepDefineID;
    }

    public int getWorkflowID() {
        return WorkflowID;
    }

    public void setWorkflowID(int workflowID) {
        WorkflowID = workflowID;
    }

    public int getSubWorkflowID() {
        return SubWorkflowID;
    }

    public void setSubWorkflowID(int subWorkflowID) {
        SubWorkflowID = subWorkflowID;
    }

    public int getStep() {
        return Step;
    }

    public void setStep(int step) {
        Step = step;
    }

    public String getTitle() {
        return Title;
    }

    public void setTitle(String title) {
        Title = title;
    }

    public int getDuration() {
        return Duration;
    }

    public void setDuration(int duration) {
        Duration = duration;
    }

    public String getTimeUnit() {
        return TimeUnit;
    }

    public void setTimeUnit(String timeUnit) {
        TimeUnit = timeUnit;
    }

    public String getEnterDay() {
        return EnterDay;
    }

    public void setEnterDay(String enterDay) {
        EnterDay = enterDay;
    }
}

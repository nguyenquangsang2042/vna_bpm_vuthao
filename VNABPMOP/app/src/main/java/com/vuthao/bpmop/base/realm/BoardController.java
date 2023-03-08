package com.vuthao.bpmop.base.realm;

import com.vuthao.bpmop.base.model.app.Workflow;
import com.vuthao.bpmop.base.model.app.WorkflowCategory;

import java.util.ArrayList;

import io.realm.RealmResults;
import io.realm.Sort;

public class BoardController extends RealmController {

    public ArrayList<WorkflowCategory> getWorkflowCategory() {
        RealmResults<WorkflowCategory> results = realm.where(WorkflowCategory.class)
                .sort("Title", Sort.ASCENDING)
                .findAll();
        return new ArrayList<>(results);
    }

    public ArrayList<Workflow> getWorkflows(int id) {
        RealmResults<Workflow> results = realm.where(Workflow.class)
                .equalTo("WorkflowCategoryID", id)
                .and().equalTo("StatusName", "Active")
                .sort("WorkflowID", Sort.ASCENDING)
                .findAll();

        return new ArrayList<>(results);
    }

    public ArrayList<Workflow> getFavorites() {
        RealmResults<Workflow> results = realm.where(Workflow.class)
                .equalTo("Favorite", 1)
                .and()
                .equalTo("StatusName", "Active")
                .sort("Title", Sort.ASCENDING)
                .findAll();

        return new ArrayList<>(results);
    }
}

package com.vuthao.bpmop.follow.presenter;

import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.model.app.AppStatus;
import com.vuthao.bpmop.base.model.app.Group;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.model.app.Workflow;
import com.vuthao.bpmop.base.model.app.WorkflowFollow;
import com.vuthao.bpmop.base.model.custom.UserAndGroup;
import com.vuthao.bpmop.base.realm.RealmController;

import java.util.ArrayList;
import java.util.Date;
import java.util.stream.Collectors;

import io.realm.Realm;
import io.realm.RealmResults;

public class FollowPresenter {
    private final Realm realm;

    public FollowPresenter() {
        realm = new RealmController().getRealm();
    }

    public ArrayList<AppBase> getListFollow() {
        ArrayList<AppBase> appBases = new ArrayList<>();
        RealmResults<AppBase> results = realm.where(AppBase.class)
                .notEqualTo("StatusGroup", 1)
                .notEqualTo("ResourceCategoryId", 16)
                .findAll();

        for (AppBase ext : results) {
            String workflowItemId = Functions.share.getWorkflowItemIDByUrl(ext.getItemUrl());
            WorkflowFollow workflowFollow = realm.where(WorkflowFollow.class)
                    .equalTo("WorkflowItemId", Integer.parseInt(workflowItemId))
                    .findFirst();
            if (workflowFollow != null) {
                ext.setFollow(workflowFollow.getStatus());
            }

            if (ext.isFollow() == 1) {
                if (!Functions.isNullOrEmpty(ext.getAssignedTo())) {
                    UserAndGroup userAndGroup = new UserAndGroup();
                    User user = realm.where(User.class)
                            .equalTo("ID", ext.getAssignedTo().split(",")[0].toLowerCase())
                            .findFirst();
                    if (user != null) {
                        userAndGroup.setID(user.getID());
                        userAndGroup.setName(user.getFullName());
                        userAndGroup.setImagePath(user.getImagePath());
                        userAndGroup.setType("0");
                    } else {
                        Group group = realm.where(Group.class)
                                .equalTo("ID", ext.getAssignedTo().split(",")[0].toLowerCase())
                                .findFirst();
                        if (group != null) {
                            userAndGroup.setID(group.getID());
                            userAndGroup.setName(group.getTitle());
                            userAndGroup.setType("1");
                        }
                    }

                    ext.setUser(userAndGroup);
                }

                if (!Functions.isNullOrEmpty(ext.getAssignedTo())) {
                    Workflow workflow = realm.where(Workflow.class)
                            .equalTo("WorkflowID", ext.getWorkflowId())
                            .findFirst();
                    if (workflow != null) {
                        ext.setWorkflow(workflow);
                    }
                }

                if (ext.getStatusGroup() > 0) {
                    AppStatus s = realm.where(AppStatus.class)
                            .equalTo("ID", ext.getStatusGroup())
                            .findFirst();
                    if (s != null) {
                        ext.setAppStatus(s);
                    }
                }

                appBases.add(ext);
            }
        }

        appBases.sort((o1, o2) -> {
            Date t1Val = Functions.share.formatStringToDate(o1.getCreated());
            Date t2Val = Functions.share.formatStringToDate(o2.getCreated());
            return (t1Val.getTime() > t2Val.getTime() ? -1 : 1);     //descending
        });

        return appBases;
    }
}

package com.vuthao.bpmop.base.realm;

import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.Group;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.model.app.Workflow;
import com.vuthao.bpmop.base.model.app.WorkflowItem;
import com.vuthao.bpmop.base.model.custom.UserAndGroup;

import java.util.ArrayList;

import io.realm.Realm;
import io.realm.RealmResults;

public class WorkflowController extends RealmController {

    public Workflow getWorkflowById(int workdlowId) {
        if (realm.where(Workflow.class)
                .equalTo("WorkflowID", workdlowId)
                .findFirst() != null) {
            return realm.where(Workflow.class)
                    .equalTo("WorkflowID", workdlowId)
                    .findFirst();
        }
        return null;
    }

    public ArrayList<UserAndGroup> getUserAndGroup() {
        ArrayList<UserAndGroup> result = new ArrayList<>();
        RealmResults<User> users = realm.where(User.class)
                .isNotNull("Email").and()
                .not().equalTo("Email", CurrentUser.getInstance().getUser().getEmail())
                .findAll();
        RealmResults<Group> groups = realm.where(Group.class)
                .isNotNull("Description").and()
                .not().equalTo("Description", CurrentUser.getInstance().getUser().getEmail())
                .findAll();

        for (User user : users) {
            UserAndGroup userAndGroup = new UserAndGroup();
            userAndGroup.setID(user.getID());
            userAndGroup.setType("0");
            userAndGroup.setName(user.getFullName());
            userAndGroup.setAccountName(user.getAccountName());
            userAndGroup.setEmail(user.getEmail());
            userAndGroup.setImagePath(user.getImagePath());
            userAndGroup.setSearch(userAndGroup.getAccountName() + userAndGroup.getEmail());
            result.add(userAndGroup);
        }

        for (Group group : groups) {
            UserAndGroup userAndGroup = new UserAndGroup();
            userAndGroup.setID(group.getID());
            userAndGroup.setName(group.getTitle());
            userAndGroup.setImagePath(group.getImage());
            userAndGroup.setEmail(group.getDescription());
            userAndGroup.setType("1");
            userAndGroup.setAccountName(group.getTitle());
            userAndGroup.setSearch(userAndGroup.getAccountName() + userAndGroup.getEmail());
            result.add(userAndGroup);
        }

        return result;
    }

    public void updateFollow(String workflowId, boolean isFollow) {
        WorkflowItem workflowItem = realm.where(WorkflowItem.class).equalTo("ID", workflowId).findFirst();
        if (workflowItem != null) {
            realm.executeTransaction(realm -> {
                workflowItem.setFollow(isFollow);
                realm.copyToRealmOrUpdate(workflowItem);
            });
        }
    }
}

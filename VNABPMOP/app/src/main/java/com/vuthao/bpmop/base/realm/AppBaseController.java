package com.vuthao.bpmop.base.realm;

import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.model.app.AppStatus;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.Group;
import com.vuthao.bpmop.base.model.app.Notify;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.model.app.Workflow;
import com.vuthao.bpmop.base.model.app.WorkflowFollow;
import com.vuthao.bpmop.base.model.app.WorkflowItem;
import com.vuthao.bpmop.base.model.custom.UserAndGroup;

import java.util.ArrayList;
import java.util.Date;

import io.realm.RealmResults;

public class AppBaseController extends RealmController {

    public ArrayList<AppBase> getVDTItems(int status, int workflowId) {
        ArrayList<AppBase> items = new ArrayList<>();

        String[] app = Functions.share.getAppSettings(Constants.APPSTATUS_TOME_DANGXULY).split(",");
        Integer[] arrInt = new Integer[app.length];

        for (int i = 0; i < app.length; i++) {
            arrInt[i] = Integer.parseInt(app[i]);
        }

        RealmResults<AppBase> results = null;
        if (workflowId > 0) {
            results = realm.where(AppBase.class)
                    .beginGroup()
                    .contains("AssignedTo", CurrentUser.getInstance().getUser().getID().toUpperCase())
                    .or()
                    .contains("NotifiedUsers", CurrentUser.getInstance().getUser().getID().toUpperCase())
                    .or()
                    .contains("Attendees", CurrentUser.getInstance().getUser().getID().toUpperCase())
                    .endGroup()
                    .in("StatusGroup", arrInt)
                    .beginGroup()
                    .greaterThan("WorkflowId", 0)
                    .and()
                    .equalTo("WorkflowId", workflowId)
                    .endGroup()
                    .findAll();
        } else {
            results = realm.where(AppBase.class)
                    .beginGroup()
                    .contains("AssignedTo", CurrentUser.getInstance().getUser().getID().toUpperCase())
                    .or()
                    .contains("NotifiedUsers", CurrentUser.getInstance().getUser().getID().toUpperCase())
                    .or()
                    .contains("Attendees", CurrentUser.getInstance().getUser().getID().toUpperCase())
                    .endGroup()
                    .in("StatusGroup", arrInt)
                    .findAll();
        }


        for (AppBase ext : results) {
            // Task ko có follow
            if (ext.getResourceCategoryId() != 16) {
                String workflowItemID = Functions.share.getWorkflowItemIDByUrl(ext.getItemUrl());
                WorkflowFollow workflowFollow = realm.where(WorkflowFollow.class)
                        .equalTo("WorkflowItemId", Integer.parseInt(workflowItemID))
                        .findFirst();

                if (workflowFollow != null) {
                    ext.setFollow(workflowFollow.getStatus());
                }
            }

            Notify notify = realm.where(Notify.class)
                    .equalTo("SPItemId", ext.getID())
                    .equalTo("Status", status)
                    .equalTo("Type", true)
                    .findFirst();

            if (notify != null) {
                ext.setNotifyId(notify.getID());
                ext.setRead(notify.isRead());
                ext.setStartDate(notify.getStartDate());

                if (!Functions.isNullOrEmpty(notify.getAssignedBy())) {
                    User user = realm.where(User.class).equalTo("ID", ext.getAssignedBy().toLowerCase()).findFirst();
                    UserAndGroup userAndGroup = new UserAndGroup();
                    if (user != null) {
                        userAndGroup.setID(user.getID());
                        userAndGroup.setImagePath(user.getImagePath());
                        userAndGroup.setType("0");
                    } else {
                        Group group = realm.where(Group.class).equalTo("ID", ext.getAssignedBy().toLowerCase()).findFirst();
                        if (group != null) {
                            userAndGroup.setID(group.getID());
                            userAndGroup.setType("1");
                        }
                    }

                    ext.setUser(userAndGroup);
                }

                if (ext.getWorkflowId() > 0) {
                    Workflow workflow = realm.where(Workflow.class).equalTo("WorkflowID", ext.getWorkflowId()).findFirst();
                    if (workflow != null) {
                        ext.setWorkflow(workflow);
                    }
                }

                if (ext.getStatusGroup() > 0) {
                    AppStatus s = realm.where(AppStatus.class).equalTo("ID", ext.getStatusGroup()).findFirst();
                    if (s != null) {
                        ext.setAppStatus(s);
                    }
                }

                items.add(ext);
            }
        }

        /*items.sort((o1, o2) -> {
            Date t1Val = Functions.share.formatStringToDate(o1.getStartDate());
            Date t2Val = Functions.share.formatStringToDate(o2.getStartDate());
            return (t1Val.getTime() > t2Val.getTime() ? -1 : 1);     //descending
            //  return (d1.getTime() > d2.getTime() ? 1 : -1);     //ascending
        });*/

        return items;
    }

    public ArrayList<AppBase> modifiedFilters(String type, ArrayList<AppBase> filters) {
        if (type.equals("VDT")) {
            for (AppBase ext : filters) {
                // Task ko có follow
                if (ext.getResourceCategoryId() != 16) {
                    String workflowItemID = Functions.share.getWorkflowItemIDByUrl(ext.getItemUrl());
                    WorkflowFollow workflowFollow = realm.where(WorkflowFollow.class)
                            .equalTo("WorkflowItemId", Integer.parseInt(workflowItemID))
                            .findFirst();

                    if (workflowFollow != null) {
                        ext.setFollow(workflowFollow.getStatus());
                    }
                }

                if (!Functions.isNullOrEmpty(ext.getAssignedBy())) {
                    User user = realm.where(User.class).equalTo("ID", ext.getAssignedBy().toLowerCase()).findFirst();
                    UserAndGroup userAndGroup = new UserAndGroup();
                    if (user != null) {
                        userAndGroup.setID(user.getID());
                        userAndGroup.setImagePath(user.getImagePath());
                        userAndGroup.setType("0");
                    } else {
                        Group group = realm.where(Group.class).equalTo("ID", ext.getAssignedBy().toLowerCase()).findFirst();
                        if (group != null) {
                            userAndGroup.setID(group.getID());
                            userAndGroup.setType("1");
                        }
                    }

                    ext.setUser(userAndGroup);
                }

                if (ext.getWorkflowId() > 0) {
                    Workflow workflow = realm.where(Workflow.class).equalTo("WorkflowID", ext.getWorkflowId()).findFirst();
                    if (workflow != null) {
                        ext.setWorkflow(workflow);
                    }
                }

                if (ext.getStatusGroup() > 0) {
                    AppStatus s = realm.where(AppStatus.class).equalTo("ID", ext.getStatusGroup()).findFirst();
                    if (s != null) {
                        ext.setAppStatus(s);
                    }
                }
            }
        } else {
            for (AppBase ext : filters) {
                // Task không có follow
                if (ext.getResourceCategoryId() != 16) {
                    String workflowItemID = Functions.share.getWorkflowItemIDByUrl(ext.getItemUrl());
                    WorkflowFollow follow = realm.where(WorkflowFollow.class).equalTo("WorkflowItemId", Integer.parseInt(workflowItemID)).findFirst();
                    if (follow != null) {
                        ext.setFollow(follow.getStatus());
                    }
                }

                if (!Functions.isNullOrEmpty(ext.getAssignedTo())) {
                    UserAndGroup userAndGroup = new UserAndGroup();
                    User user = realm.where(User.class).equalTo("ID", ext.getAssignedTo().split(",")[0].toLowerCase()).findFirst();
                    if (user != null) {
                        userAndGroup.setID(user.getID());
                        userAndGroup.setName(user.getFullName());
                        userAndGroup.setImagePath(user.getImagePath());
                        userAndGroup.setType("0");
                    } else {
                        Group group = realm.where(Group.class).equalTo("ID", ext.getAssignedTo().split(",")[0].toLowerCase()).findFirst();
                        if (group != null) {
                            userAndGroup.setID(group.getID());
                            userAndGroup.setName(group.getTitle());
                            userAndGroup.setType("1");
                        }
                    }

                    ext.setUser(userAndGroup);
                }

                if (ext.getStatusGroup() > 0) {
                    AppStatus s = realm.where(AppStatus.class).equalTo("ID", ext.getStatusGroup()).findFirst();
                    if (s != null) {
                        ext.setAppStatus(s);
                    }
                }
            }
        }

        return filters;
    }

    public ArrayList<AppBase> getListVDTDaXuLy(int workflowId) {
        String[] app = Functions.share.getAppSettings(Constants.APPSTATUS_TOME_DAXULY).split(",");
        Integer[] arrInt = new Integer[app.length];

        for (int i = 0; i < app.length; i++) {
            arrInt[i] = Integer.parseInt(app[i]);
        }

        RealmResults<AppBase> results = null;
        if (workflowId > 0) {
            results = realm.where(AppBase.class)
                    .notEqualTo("StatusGroup", 256)
                    .and()
                    .contains("NotifiedUsers", CurrentUser.getInstance().getUser().getID().toUpperCase())
                    .and()
                    .beginGroup()
                    .notEqualTo("ResourceCategoryId", 16)
                    .or()
                    .notEqualTo("CreatedBy", CurrentUser.getInstance().getUser().getID().toUpperCase())
                    .endGroup()
                    .in("StatusGroup", arrInt)
                    .and()
                    .equalTo("WorkflowId", workflowId)
                    .findAll();
        } else {
            results = realm.where(AppBase.class)
                    .notEqualTo("StatusGroup", 256)
                    .and()
                    .contains("NotifiedUsers", CurrentUser.getInstance().getUser().getID().toUpperCase())
                    .and()
                    .beginGroup()
                    .notEqualTo("ResourceCategoryId", 16)
                    .or()
                    .notEqualTo("CreatedBy", CurrentUser.getInstance().getUser().getID().toUpperCase())
                    .endGroup()
                    .in("StatusGroup", arrInt)
                    .findAll();
        }

        ArrayList<AppBase> items = new ArrayList<>(results);

        for (AppBase ext : items) {
            Notify notify = realm.where(Notify.class)
                    .equalTo("SPItemId", ext.getID())
                    .equalTo("Status", 1)
                    .equalTo("Type", true)
                    .findFirst();
            if (notify != null) {
                ext.setNotifyId(notify.getID());
                ext.setRead(notify.isRead());
            }

            // Task ko có follow
            if (ext.getResourceCategoryId() != 16) {
                String workflowItemID = Functions.share.getWorkflowItemIDByUrl(ext.getItemUrl());
                WorkflowFollow workflowFollow = realm.where(WorkflowFollow.class)
                        .equalTo("WorkflowItemId", Integer.parseInt(workflowItemID))
                        .findFirst();

                if (workflowFollow != null) {
                    ext.setFollow(workflowFollow.getStatus());
                }
            }

            if (!Functions.isNullOrEmpty(ext.getAssignedBy())) {
                User user = realm.where(User.class).equalTo("ID", ext.getAssignedBy().toLowerCase()).findFirst();
                UserAndGroup userAndGroup = new UserAndGroup();
                if (user != null) {
                    userAndGroup.setID(user.getID());
                    userAndGroup.setImagePath(user.getImagePath());
                    userAndGroup.setType("0");
                } else {
                    Group group = realm.where(Group.class).equalTo("ID", ext.getAssignedBy().toLowerCase()).findFirst();
                    if (group != null) {
                        userAndGroup.setID(group.getID());
                        userAndGroup.setType("1");
                    }
                }

                ext.setUser(userAndGroup);
            }

            if (ext.getWorkflowId() > 0) {
                Workflow workflow = realm.where(Workflow.class).equalTo("WorkflowID", ext.getWorkflowId()).findFirst();
                if (workflow != null) {
                    ext.setWorkflow(workflow);
                }
            }

            if (ext.getStatusGroup() > 0) {
                AppStatus s = realm.where(AppStatus.class).equalTo("ID", ext.getStatusGroup()).findFirst();
                if (s != null) {
                    ext.setAppStatus(s);
                }
            }
        }

        items.sort((o1, o2) -> {
            Date t1Val = Functions.share.formatStringToDate(o1.getModified());
            Date t2Val = Functions.share.formatStringToDate(o2.getModified());
            //descending
            return (t1Val.getTime() > t2Val.getTime() ? -1 : 1);
        });

        return items;
    }

    public int getVTBDCounts(int workflowId) {
        Integer[] status = new Integer[]{256, 64, 16, 8};

        long results = 0;
        if (workflowId > 0) {
            results = realm.where(AppBase.class)
                    .equalTo("CreatedBy", CurrentUser.getInstance().getUser().getID())
                    .not().in("StatusGroup", status)
                    .equalTo("WorkflowId", workflowId)
                    .count();
        } else {
            results = realm.where(AppBase.class)
                    .equalTo("CreatedBy", CurrentUser.getInstance().getUser().getID())
                    .not().in("StatusGroup", status)
                    .count();
        }

        return Integer.parseInt(String.valueOf(results));
    }

    public ArrayList<AppBase> getVTBDItems(int workflowId) {
        String[] setting = Functions.share.getAppSettings(Constants.APPSTATUS_FROMME).split(",");
        Integer[] status = new Integer[setting.length];

        for (int i = 0; i < setting.length; i++) {
            status[i] = Integer.parseInt(setting[i]);
        }
        RealmResults<AppBase> results = null;
        if (workflowId > 0) {
            results = realm.where(AppBase.class)
                    .equalTo("CreatedBy", CurrentUser.getInstance().getUser().getID())
                    .in("StatusGroup", status)
                    .equalTo("WorkflowId", workflowId)
                    .sort("Created")
                    .findAll();
        } else {
            results = realm.where(AppBase.class)
                    .equalTo("CreatedBy", CurrentUser.getInstance().getUser().getID())
                    .in("StatusGroup", status)
                    .sort("Created")
                    .findAll();
        }

        ArrayList<AppBase> items = new ArrayList<>(results);

        for (int i = 0; i < items.size(); i++) {
            // Task không có follow
            if (items.get(i).getResourceCategoryId() != 16) {
                String workflowItemID = Functions.share.getWorkflowItemIDByUrl(items.get(i).getItemUrl());
                WorkflowFollow follow = realm.where(WorkflowFollow.class).equalTo("WorkflowItemId", Integer.parseInt(workflowItemID)).findFirst();
                if (follow != null) {
                    items.get(i).setFollow(follow.getStatus());
                }
            }

            if (!Functions.isNullOrEmpty(items.get(i).getAssignedTo())) {
                UserAndGroup userAndGroup = new UserAndGroup();
                User user = realm.where(User.class).equalTo("ID", items.get(i).getAssignedTo().split(",")[0].toLowerCase()).findFirst();
                if (user != null) {
                    userAndGroup.setID(user.getID());
                    userAndGroup.setName(user.getFullName());
                    userAndGroup.setImagePath(user.getImagePath());
                    userAndGroup.setType("0");
                } else {
                    Group group = realm.where(Group.class).equalTo("ID", items.get(i).getAssignedTo().split(",")[0].toLowerCase()).findFirst();
                    if (group != null) {
                        userAndGroup.setID(group.getID());
                        userAndGroup.setName(group.getTitle());
                        userAndGroup.setType("1");
                    }
                }

                items.get(i).setUser(userAndGroup);
            }

            if (items.get(i).getStatusGroup() > 0) {
                AppStatus s = realm.where(AppStatus.class).equalTo("ID", items.get(i).getStatusGroup()).findFirst();
                if (s != null) {
                    items.get(i).setAppStatus(s);
                }
            }
        }

        items.sort((o1, o2) -> {
            Date t1Val = Functions.share.formatStringToDate(o1.getCreated());
            Date t2Val = Functions.share.formatStringToDate(o2.getCreated());
            return (t1Val.getTime() > t2Val.getTime() ? -1 : 1);     //descending
            //  return (d1.getTime() > d2.getTime() ? 1 : -1);     //ascending
        });

        return items;
    }

    public WorkflowFollow getWorkflowfollow(int workflowId) {
        WorkflowFollow workflowFollow = realm.where(WorkflowFollow.class).equalTo("WorkflowItemId", workflowId).findFirst();
        return workflowFollow;
    }

    public ArrayList<AppStatus> getListAppStatusFilter() {
        ArrayList<AppStatus> items = new ArrayList<>();
        String config = Functions.share.getAppSettings(Constants.APPSTATUS); // 2,4,8,16,64,128

        if (!Functions.isNullOrEmpty(config)) {
            String[] numberStrs = config.split(",");
            Integer[] numbers = new Integer[numberStrs.length];

            for (int i = 0; i < config.split(",").length; i++) {
                numbers[i] = Integer.parseInt(numberStrs[i]);
            }

            RealmResults<AppStatus> results = realm.where(AppStatus.class)
                    .in("ID", numbers)
                    .findAll();
            items.addAll(results);
        }

        return items;
    }

    public ArrayList<AppStatus> getListAppStatusVDTDaXuLyFilter() {
        ArrayList<AppStatus> items = new ArrayList<>();
        Integer[] numbers = new Integer[] {64, 16, 8, 4};
        RealmResults<AppStatus> results = realm.where(AppStatus.class)
                .in("ID", numbers)
                .findAll();
        items.addAll(results);

        return items;
    }


    public String getValueSelected(String key) {
        return Functions.share.getAppSettings(key);
    }

    public void updateRead(String notifyId) {
        Notify notify = realm.where(Notify.class)
                .equalTo("ID", notifyId)
                .findFirst();
        if (notify != null) {
            realm.executeTransaction(realm -> {
                notify.setRead(true);
                realm.copyToRealmOrUpdate(notify);
            });
        }
    }

    public WorkflowItem getWorkflowItem(String Id) {
        return realm.where(WorkflowItem.class).equalTo("ID", Id).findFirst();
    }
}

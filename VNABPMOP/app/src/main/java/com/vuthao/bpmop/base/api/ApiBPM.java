package com.vuthao.bpmop.base.api;

import android.util.Log;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.ApiList;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.model.app.AppLanguage;
import com.vuthao.bpmop.base.model.app.AppStatus;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.DBVariable;
import com.vuthao.bpmop.base.model.app.Group;
import com.vuthao.bpmop.base.model.app.Notify;
import com.vuthao.bpmop.base.model.app.Position;
import com.vuthao.bpmop.base.model.app.ResourceView;
import com.vuthao.bpmop.base.model.app.Settings;
import com.vuthao.bpmop.base.model.app.Status;
import com.vuthao.bpmop.base.model.app.TimeLanguage;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.model.app.Workflow;
import com.vuthao.bpmop.base.model.app.WorkflowCategory;
import com.vuthao.bpmop.base.model.app.WorkflowFollow;
import com.vuthao.bpmop.base.model.app.WorkflowItem;
import com.vuthao.bpmop.base.model.app.WorkflowStatus;
import com.vuthao.bpmop.base.model.app.WorkflowStepDefine;
import com.vuthao.bpmop.base.realm.RealmController;
import com.vuthao.bpmop.base.realm.RealmHelper;
import com.vuthao.bpmop.base.vars.VarsTable;

import java.net.ConnectException;
import java.net.UnknownHostException;
import java.util.HashMap;

import io.realm.Realm;
import okhttp3.MultipartBody;
import okhttp3.RequestBody;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ApiBPM extends ApiController {
    private ApiAuthController apiAuth;
    private ApiBPMRefreshListener listener;
    private int taskCountComplete = 0;
    private int totalTask = 0;

    public ApiBPM(ApiBPMRefreshListener listener) {
        this.listener = listener;
        this.totalTask = Constants.asyncCallApi;
    }

    public ApiBPM(ApiBPMRefreshListener listener, int totalTask) {
        this.listener = listener;
        this.totalTask = totalTask;

        if (this.totalTask == 0) {
            this.totalTask = Constants.asyncCallApi;
        }
    }

    public ApiBPM() {
        apiAuth = new ApiAuthController();
    }

    public void updateWorkflowFollow(ApiBPMListener callback) {
        Call<ApiList<WorkflowFollow>> call = getApiRouteToken().getWorkflowFollows(getModified("WorkflowFollow", false), "0");
        call.enqueue(new Callback<ApiList<WorkflowFollow>>() {
            @Override
            public void onResponse(Call<ApiList<WorkflowFollow>> call, Response<ApiList<WorkflowFollow>> response) {
                ApiList<WorkflowFollow> setting = response.body();
                if (setting.getStatus().equals("SUCCESS")) {
                    if (setting.getData() != null && setting.getData().size() > 0)
                        new RealmHelper<WorkflowFollow>().addNewItems(setting.getData());
                    DBVariable db = new DBVariable("WorkflowFollow", setting.getDateNow());
                    new RealmHelper<DBVariable>().addNewItem(db);
                    callback.OnSuccess();
                }
            }

            @Override
            public void onFailure(Call<ApiList<WorkflowFollow>> call, Throwable t) {
            }
        });
    }

    public void sendControlDynamicAction(MultipartBody.Part[] files, HashMap<String, RequestBody> hashMap, String err, ApiBPMListener callback) {
        Call<Status> call = getApiRouteToken().sendControlDynamicAction(files, hashMap);
        call.enqueue(new Callback<Status>() {
            @Override
            public void onResponse(Call<Status> call, Response<Status> response) {
                if (response.isSuccessful()) {
                    assert response.body() != null;
                    if (response.body().getStatus().equals("SUCCESS")) {
                        callback.OnSuccess();
                    } else {
                        callback.OnErr(err);
                    }
                } else {
                    callback.OnErr(err);
                }
            }

            @Override
            public void onFailure(Call<Status> call, Throwable t) {
                if (t instanceof UnknownHostException || t instanceof ConnectException) {
                    callback.OnErr(Functions.share.getTitle("MESS_REQUIRE_NETWORK", "No network connection, please try again."));
                } else {
                    callback.OnErr(err);
                }
            }
        });
    }

    public void updateAllMasterData(boolean flgGetAll) {
        taskCountComplete = 0;
        getSettings(getModified(VarsTable.SETTINGS, flgGetAll),  flgGetAll ? "1" : "0");
        getAppLanguage(getModified(VarsTable.APPLANGUAGE, flgGetAll));
        getNotify(getModified(VarsTable.NOTIFY, flgGetAll),  flgGetAll ? "1" : "0");
        getAppBase(getModified(VarsTable.APPBASE, flgGetAll),  flgGetAll ? "1" : "0");
        getWorkflows(getModified(VarsTable.WORKFLOWS, flgGetAll),  flgGetAll ? "1" : "0");
        getTimeLanguage(getModified(VarsTable.TIMELANGUAGE, flgGetAll),  flgGetAll ? "1" : "0");
        getAppStatus(getModified(VarsTable.APPSTATUS, flgGetAll),  flgGetAll ? "1" : "0");
        getUsers(getModified(VarsTable.USERS, flgGetAll),  flgGetAll ? "1" : "0");
        getGroups(getModified(VarsTable.GROUPS, flgGetAll),  flgGetAll ? "1" : "0");
        getWorkflowFollow(getModified(VarsTable.WORKFLOWFOLLOW, flgGetAll),  flgGetAll ? "1" : "0");
        getWorkflowCategory(getModified(VarsTable.WORKFLOWCATEGORY, flgGetAll),  flgGetAll ? "1" : "0");
        getWorkflowItems(getModified(VarsTable.WORKFLOWITEM, flgGetAll),  flgGetAll ? "1" : "0");
        getPositions(getModified(VarsTable.POSITION, flgGetAll),  flgGetAll ? "1" : "0");
        getWorkflowStepDefine(getModified(VarsTable.WORKFLOWSTEPDEFINE, flgGetAll),  flgGetAll ? "1" : "0");
        getResourceView(getModified(VarsTable.RESOURCEVIEW, flgGetAll),  flgGetAll ? "1" : "0");
        getWorkflowStatus(getModified(VarsTable.WORKFLOWSTATUS, flgGetAll),  flgGetAll ? "1" : "0");
    }

    public void masterDataNotChanges(boolean flgGetAll) {
        getSettings(getModified(VarsTable.SETTINGS, flgGetAll),  flgGetAll ? "1" : "0");
        getWorkflowStatus(getModified(VarsTable.WORKFLOWSTATUS, flgGetAll),  flgGetAll ? "1" : "0");
        getAppStatus(getModified(VarsTable.APPSTATUS, flgGetAll),  flgGetAll ? "1" : "0");
    }

    public void updateDataSubmitAction(boolean flgGetAll) {
        taskCountComplete = 0;
        getNotify(getModified(VarsTable.NOTIFY, flgGetAll),  flgGetAll ? "1" : "0");
        getAppBase(getModified(VarsTable.APPBASE, flgGetAll),  flgGetAll ? "1" : "0");
        getWorkflowItems(getModified(VarsTable.WORKFLOWITEM, flgGetAll),  flgGetAll ? "1" : "0");
    }

    public void getSettings(String modified, String isFirst) {
        Call<ApiList<Settings>> call = getApiRoute().getSettings(modified, isFirst);
        call.enqueue(new Callback<ApiList<Settings>>() {
            @Override
            public void onResponse(Call<ApiList<Settings>> call, Response<ApiList<Settings>> response) {
                ApiList<Settings> setting = response.body();
                if (setting.getStatus().equals("SUCCESS")) {
                    Realm r = new RealmController().getRealm();
                    DBVariable db = new DBVariable(VarsTable.SETTINGS, setting.getDateNow());
                    r.executeTransaction(realm -> {
                        realm.copyToRealmOrUpdate(setting.getData());
                        realm.copyToRealmOrUpdate(db);
                    });
                }

                taskCountComplete++;
                if (taskCountComplete == totalTask)
                    listener.OnRefreshSuccess();
            }

            @Override
            public void onFailure(Call<ApiList<Settings>> call, Throwable ex) {
                taskCountComplete++;
                if (taskCountComplete == totalTask)
                    listener.OnRefreshErr();
                Log.d("Settings ERR", ex.getMessage());
            }
        });
    }

    public void getAppLanguage(String modified) {
        Call<ApiList<AppLanguage>> call = getApiRoute().getAppLanguage(CurrentUser.getInstance().getUser()==null?"1033":
                String.valueOf(CurrentUser.getInstance().getUser().getLanguage()),modified);
        call.enqueue(new Callback<ApiList<AppLanguage>>() {
            @Override
            public void onResponse(Call<ApiList<AppLanguage>> call, Response<ApiList<AppLanguage>> response) {
                ApiList<AppLanguage> setting = response.body();
                if (setting.getStatus().equals("SUCCESS")) {
                    Realm r = new RealmController().getRealm();
                    r.executeTransaction(realm -> {
                        realm.copyToRealmOrUpdate(setting.getData());
                        realm.copyToRealmOrUpdate(new DBVariable(VarsTable.APPLANGUAGE, setting.getDateNow()));
                    });
                }
                taskCountComplete++;
                if (taskCountComplete == totalTask)
                    listener.OnRefreshSuccess();
            }

            @Override
            public void onFailure(Call<ApiList<AppLanguage>> call, Throwable ex) {
                taskCountComplete++;
                if (taskCountComplete == totalTask)
                    listener.OnRefreshErr();
                Log.d("AppLanguage ERR", ex.getMessage());
            }
        });
    }

    public void getNotify(String modified, String isFirst) {
        Call<ApiList<Notify>> call = getApiRouteToken().getNotify(modified, isFirst);
        call.enqueue(new Callback<ApiList<Notify>>() {
            @Override
            public void onResponse(Call<ApiList<Notify>> call, Response<ApiList<Notify>> response) {
                ApiList<Notify> setting = response.body();
                if (setting.getStatus().equals("SUCCESS")) {
                    Realm r = new RealmController().getRealm();
                    r.executeTransaction(realm -> {
                        realm.copyToRealmOrUpdate(setting.getData());
                        realm.copyToRealmOrUpdate(new DBVariable(VarsTable.NOTIFY, setting.getDateNow()));
                    });
                }

                taskCountComplete++;
                if (taskCountComplete == totalTask)
                    listener.OnRefreshSuccess();
            }

            @Override
            public void onFailure(Call<ApiList<Notify>> call, Throwable ex) {
                taskCountComplete++;
                if (taskCountComplete == totalTask)
                    listener.OnRefreshErr();
                Log.d("getNotify ERR", ex.getMessage());
            }
        });
    }

    public void getAppBase(String modified, String isFirst) {
        Call<ApiList<AppBase>> call = getApiRouteToken().getAppBase(modified, isFirst);
        call.enqueue(new Callback<ApiList<AppBase>>() {
            @Override
            public void onResponse(Call<ApiList<AppBase>> call, Response<ApiList<AppBase>> response) {
                ApiList<AppBase> setting = response.body();
                if (setting.getStatus().equals("SUCCESS")) {
                    Realm r = new RealmController().getRealm();
                    r.executeTransaction(realm -> {
                        realm.copyToRealmOrUpdate(setting.getData());
                        realm.copyToRealmOrUpdate(new DBVariable(VarsTable.APPBASE, setting.getDateNow()));
                    });
                }

                taskCountComplete++;
                if (taskCountComplete == totalTask)
                    listener.OnRefreshSuccess();
            }

            @Override
            public void onFailure(Call<ApiList<AppBase>> call, Throwable t) {
                taskCountComplete++;
                if (taskCountComplete == totalTask)
                    listener.OnRefreshErr();
                Log.d("getAppBase ERR", t.getMessage());
            }
        });
    }

    public void getWorkflows(String modified, String isFirst) {
        Call<ApiList<Workflow>> call = getApiRouteToken().getWorkflows(modified, isFirst);
        call.enqueue(new Callback<ApiList<Workflow>>() {
            @Override
            public void onResponse(Call<ApiList<Workflow>> call, Response<ApiList<Workflow>> response) {
                ApiList<Workflow> setting = response.body();
                if (setting.getStatus().equals("SUCCESS")) {
                    Realm r = new RealmController().getRealm();
                    r.executeTransaction(realm -> {
                        realm.copyToRealmOrUpdate(setting.getData());
                        realm.copyToRealmOrUpdate(new DBVariable(VarsTable.WORKFLOWS, setting.getDateNow()));
                    });
                }

                taskCountComplete++;
                if (taskCountComplete == totalTask)
                    listener.OnRefreshSuccess();
            }

            @Override
            public void onFailure(Call<ApiList<Workflow>> call, Throwable t) {
                taskCountComplete++;
                if (taskCountComplete == totalTask)
                    listener.OnRefreshErr();
                Log.d("Workflow ERR", t.getMessage());
            }
        });
    }

    public void getTimeLanguage(String modified, String isFirst) {
        Call<ApiList<TimeLanguage>> call = getApiRoute().getTimeLanguage(modified, isFirst);
        call.enqueue(new Callback<ApiList<TimeLanguage>>() {
            @Override
            public void onResponse(Call<ApiList<TimeLanguage>> call, Response<ApiList<TimeLanguage>> response) {
                ApiList<TimeLanguage> setting = response.body();
                if (setting.getStatus().equals("SUCCESS")) {
                    Realm r = new RealmController().getRealm();
                    r.executeTransaction(realm -> {
                        realm.copyToRealmOrUpdate(setting.getData());
                        realm.copyToRealmOrUpdate(new DBVariable(VarsTable.TIMELANGUAGE, setting.getDateNow()));
                    });
                }

                taskCountComplete++;
                if (taskCountComplete == totalTask)
                    listener.OnRefreshSuccess();
            }

            @Override
            public void onFailure(Call<ApiList<TimeLanguage>> call, Throwable t) {
                taskCountComplete++;
                if (taskCountComplete == totalTask)
                    listener.OnRefreshErr();
                Log.d("TimeLanguage ERR", t.getMessage());
            }
        });
    }

    public void getAppStatus(String modified, String isFirst) {
        Call<ApiList<AppStatus>> call = getApiRoute().getAppStatus(modified, isFirst);
        call.enqueue(new Callback<ApiList<AppStatus>>() {
            @Override
            public void onResponse(Call<ApiList<AppStatus>> call, Response<ApiList<AppStatus>> response) {
                ApiList<AppStatus> setting = response.body();
                if (setting.getStatus().equals("SUCCESS")) {
                    Realm r = new RealmController().getRealm();
                    r.executeTransaction(realm -> {
                        realm.copyToRealmOrUpdate(setting.getData());
                        realm.copyToRealmOrUpdate(new DBVariable(VarsTable.APPSTATUS, setting.getDateNow()));
                    });
                }

                taskCountComplete++;
                if (taskCountComplete == totalTask)
                    listener.OnRefreshSuccess();
            }

            @Override
            public void onFailure(Call<ApiList<AppStatus>> call, Throwable t) {
                taskCountComplete++;
                if (taskCountComplete == totalTask)
                    listener.OnRefreshErr();
                Log.d("AppStatus ERR", t.getMessage());
            }
        });
    }

    public void getUsers(String modified, String isFirst) {
        Call<ApiList<User>> call = getApiRouteToken().getUsers(modified, isFirst);
        call.enqueue(new Callback<ApiList<User>>() {
            @Override
            public void onResponse(Call<ApiList<User>> call, Response<ApiList<User>> response) {
                ApiList<User> setting = response.body();
                if (setting.getStatus().equals("SUCCESS")) {
                    Realm r = new RealmController().getRealm();
                    r.executeTransaction(realm -> {
                        if (!modified.isEmpty()) {
                            // Khi update thi set lai CurrentUser de tranh mat cac thong tin quan trong vi
                            // User khong tra ve day du thong tin nhu login
                            for (User user : setting.getData()) {
                                if (user.getID().equals(CurrentUser.getInstance().getUser().getID())) {
                                    user.setLanguage(CurrentUser.getInstance().getUser().getLanguage());
                                    user.setPosition(CurrentUser.getInstance().getUser().getPositionTitle());
                                    user.setPositionTitle(CurrentUser.getInstance().getUser().getPositionTitle());
                                    break;
                                }
                            }
                        }

                        realm.copyToRealmOrUpdate(setting.getData());
                        realm.copyToRealmOrUpdate(new DBVariable(VarsTable.USERS, setting.getDateNow()));
                    });
                }

                taskCountComplete++;
                if (taskCountComplete == totalTask)
                    listener.OnRefreshSuccess();
            }

            @Override
            public void onFailure(Call<ApiList<User>> call, Throwable t) {
                taskCountComplete++;
                if (taskCountComplete == totalTask)
                    listener.OnRefreshErr();
                Log.d("User ERR", t.getMessage());
            }
        });
    }

    public void getGroups(String modified, String isFirst) {
        Call<ApiList<Group>> call = getApiRouteToken().getGroups(modified, isFirst);
        call.enqueue(new Callback<ApiList<Group>>() {
            @Override
            public void onResponse(Call<ApiList<Group>> call, Response<ApiList<Group>> response) {
                ApiList<Group> setting = response.body();
                if (setting.getStatus().equals("SUCCESS")) {
                    Realm r = new RealmController().getRealm();
                    r.executeTransaction(realm -> {
                        realm.copyToRealmOrUpdate(setting.getData());
                        realm.copyToRealmOrUpdate(new DBVariable(VarsTable.GROUPS, setting.getDateNow()));
                    });
                }

                taskCountComplete++;
                if (taskCountComplete == totalTask)
                    listener.OnRefreshSuccess();
            }

            @Override
            public void onFailure(Call<ApiList<Group>> call, Throwable t) {
                taskCountComplete++;
                if (taskCountComplete == totalTask)
                    listener.OnRefreshErr();
                Log.d("Group ERR", t.getMessage());
            }
        });
    }

    public void getWorkflowFollow(String modified, String isFirst) {
        Call<ApiList<WorkflowFollow>> call = getApiRouteToken().getWorkflowFollows(modified, isFirst);
        call.enqueue(new Callback<ApiList<WorkflowFollow>>() {
            @Override
            public void onResponse(Call<ApiList<WorkflowFollow>> call, Response<ApiList<WorkflowFollow>> response) {
                ApiList<WorkflowFollow> setting = response.body();
                if (setting.getStatus().equals("SUCCESS")) {
                    Realm r = new RealmController().getRealm();
                    r.executeTransaction(realm -> {
                        realm.copyToRealmOrUpdate(setting.getData());
                        realm.copyToRealmOrUpdate(new DBVariable(VarsTable.WORKFLOWFOLLOW, setting.getDateNow()));
                    });
                }

                taskCountComplete++;
                if (taskCountComplete == totalTask)
                    listener.OnRefreshSuccess();
            }

            @Override
            public void onFailure(Call<ApiList<WorkflowFollow>> call, Throwable t) {
                taskCountComplete++;
                if (taskCountComplete == totalTask)
                    listener.OnRefreshErr();
                Log.d("getWorkflowFollow ERR", t.getMessage());
            }
        });
    }

    public void getWorkflowCategory(String modified, String isFirst) {
        Call<ApiList<WorkflowCategory>> call = getApiRouteToken().getWorkflowCategory(modified, isFirst);
        call.enqueue(new Callback<ApiList<WorkflowCategory>>() {
            @Override
            public void onResponse(Call<ApiList<WorkflowCategory>> call, Response<ApiList<WorkflowCategory>> response) {
                ApiList<WorkflowCategory> setting = response.body();
                if (setting.getStatus().equals("SUCCESS")) {
                    Realm r = new RealmController().getRealm();
                    r.executeTransaction(realm -> {
                        realm.copyToRealmOrUpdate(setting.getData());
                        realm.copyToRealmOrUpdate(new DBVariable(VarsTable.WORKFLOWCATEGORY, setting.getDateNow()));
                    });
                }

                taskCountComplete++;
                if (taskCountComplete == totalTask)
                    listener.OnRefreshSuccess();
            }

            @Override
            public void onFailure(Call<ApiList<WorkflowCategory>> call, Throwable t) {
                taskCountComplete++;
                if (taskCountComplete == totalTask)
                    listener.OnRefreshErr();
                Log.d("WorkflowCategory ERR", t.getMessage());
            }
        });
    }

    public void getWorkflowItems(String modified, String isFirst) {
        Call<ApiList<WorkflowItem>> call = getApiRouteToken().getWorkflowItems(modified, isFirst);
        call.enqueue(new Callback<ApiList<WorkflowItem>>() {
            @Override
            public void onResponse(Call<ApiList<WorkflowItem>> call, Response<ApiList<WorkflowItem>> response) {
                ApiList<WorkflowItem> setting = response.body();
                if (setting.getStatus().equals("SUCCESS")) {
                    Realm r = new RealmController().getRealm();
                    r.executeTransaction(realm -> {
                        realm.copyToRealmOrUpdate(setting.getData());
                        realm.copyToRealmOrUpdate(new DBVariable(VarsTable.WORKFLOWITEM, setting.getDateNow()));
                    });
                }
                taskCountComplete++;
                if (taskCountComplete == totalTask)
                    listener.OnRefreshSuccess();
            }

            @Override
            public void onFailure(Call<ApiList<WorkflowItem>> call, Throwable t) {
                taskCountComplete++;
                if (taskCountComplete == totalTask)
                    listener.OnRefreshErr();
                Log.d("WorkflowItem ERR", t.getMessage());
            }
        });
    }

    public void getPositions(String modified, String isFirst) {
        Call<ApiList<Position>> call = getApiRouteToken().getPositions(modified, isFirst);
        call.enqueue(new Callback<ApiList<Position>>() {
            @Override
            public void onResponse(Call<ApiList<Position>> call, Response<ApiList<Position>> response) {
                ApiList<Position> setting = response.body();
                if (setting.getStatus().equals("SUCCESS")) {
                    Realm r = new RealmController().getRealm();
                    r.executeTransaction(realm -> {
                        realm.copyToRealmOrUpdate(setting.getData());
                        realm.copyToRealmOrUpdate(new DBVariable(VarsTable.POSITION, setting.getDateNow()));
                    });

                    taskCountComplete++;
                    if (taskCountComplete == totalTask)
                        listener.OnRefreshSuccess();
                }
            }

            @Override
            public void onFailure(Call<ApiList<Position>> call, Throwable t) {
                taskCountComplete++;
                if (taskCountComplete == totalTask)
                    listener.OnRefreshErr();
                Log.d("Position ERR", t.getMessage());
            }
        });
    }

    public void getWorkflowStepDefine(String modified, String isFirst) {
        Call<ApiList<WorkflowStepDefine>> call = getApiRouteToken().getWorkflowStepDefine(modified, isFirst);
        call.enqueue(new Callback<ApiList<WorkflowStepDefine>>() {
            @Override
            public void onResponse(Call<ApiList<WorkflowStepDefine>> call, Response<ApiList<WorkflowStepDefine>> response) {
                ApiList<WorkflowStepDefine> setting = response.body();
                if (setting.getStatus().equals("SUCCESS")) {
                    Realm r = new RealmController().getRealm();
                    r.executeTransaction(realm -> {
                        realm.copyToRealmOrUpdate(setting.getData());
                        realm.copyToRealmOrUpdate(new DBVariable(VarsTable.WORKFLOWSTEPDEFINE, setting.getDateNow()));
                    });
                }
            }

            @Override
            public void onFailure(Call<ApiList<WorkflowStepDefine>> call, Throwable t) {
                Log.d("WorkflowStepDefine ERR", t.getMessage());
            }
        });
    }

    public void getResourceView(String modified, String isFirst) {
        Call<ApiList<ResourceView>> call = getApiRouteToken().getResourceView(modified, isFirst);
        call.enqueue(new Callback<ApiList<ResourceView>>() {
            @Override
            public void onResponse(Call<ApiList<ResourceView>> call, Response<ApiList<ResourceView>> response) {
                ApiList<ResourceView> setting = response.body();
                if (setting.getStatus().equals("SUCCESS")) {
                    Realm r = new RealmController().getRealm();
                    r.executeTransaction(realm -> {
                        realm.copyToRealmOrUpdate(setting.getData());
                        realm.copyToRealmOrUpdate(new DBVariable(VarsTable.RESOURCEVIEW, setting.getDateNow()));
                    });
                }
            }

            @Override
            public void onFailure(Call<ApiList<ResourceView>> call, Throwable t) {
                Log.d("ResourceView ERR", t.getMessage());
            }
        });
    }

    public void getWorkflowStatus(String modified, String isFirst) {
        Call<ApiList<WorkflowStatus>> call = getApiRoute().getWorkflowStatus(modified, isFirst);
        call.enqueue(new Callback<ApiList<WorkflowStatus>>() {
            @Override
            public void onResponse(Call<ApiList<WorkflowStatus>> call, Response<ApiList<WorkflowStatus>> response) {
                ApiList<WorkflowStatus> setting = response.body();
                if (setting.getStatus().equals("SUCCESS")) {
                    Realm r = new RealmController().getRealm();
                    r.executeTransaction(realm -> {
                        realm.copyToRealmOrUpdate(setting.getData());
                        realm.copyToRealmOrUpdate(new DBVariable(VarsTable.WORKFLOWSTATUS, setting.getDateNow()));
                    });
                }
            }

            @Override
            public void onFailure(Call<ApiList<WorkflowStatus>> call, Throwable t) {
                Log.d("WorkflowStatus ERR", t.getMessage());
            }
        });
    }

    private String getModified(String table, boolean flgGetAll) {
        String modified = "";
        if (flgGetAll) {
            modified = Functions.share.getDateStringApi(-Constants.dataLimitDay);
        } else {
            DBVariable db = new RealmHelper<DBVariable>().getItemById(DBVariable.class, "Id", table);
            if (db != null) {
                modified = db.getValue();
            }
        }

        return modified;
    }

    public interface ApiBPMListener {
        void OnSuccess();
        void OnErr(String err);
    }

    public interface ApiBPMRefreshListener {
        void OnRefreshSuccess();
        void OnRefreshErr();
    }
}

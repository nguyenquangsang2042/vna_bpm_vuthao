package com.vuthao.bpmop.base.api;

import android.util.Log;

import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.ApiList;
import com.vuthao.bpmop.base.model.ApiObject;
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
import com.vuthao.bpmop.home.presenter.LoginPresenter;

import io.realm.Realm;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ApiAuthController extends ApiController {
    private LoginPresenter.LoginListener listener;
    private int taskCountComplete = 0;
    private final String ERRVN = "Phiên đăng nhập hết hạn. Vui lòng đăng nhập lại!";
    private final String ERREN = "Login session expired. Please log in again!";

    public ApiAuthController(LoginPresenter.LoginListener listener) {
        this.listener = listener;
    }

    public ApiAuthController() {
    }

    public void Login(String token, String deviceInfo, String loginType) {
        Call<ApiObject<User>> call = getApiRouteToken(token).Login(deviceInfo, loginType);
        call.enqueue(new Callback<ApiObject<User>>() {
            @Override
            public void onResponse(Call<ApiObject<User>> call, retrofit2.Response<ApiObject<User>> response) {
                if (response.isSuccessful()) {
                    ApiObject<User> data = response.body();
                    assert data != null;
                    if (data.getStatus().equals("SUCCESS")) {
                        listener.OnLoginSuccess(data.getData().getBeanUser());
                    } else {
                        // Yeu cau dang nhap lai -> het Cookie
                        if (data.getMess().getKey().equals("998")) {
                            listener.OnCookieExpire(ERREN);
                        } else {
                            listener.OnLoginErr(Functions.share.getTitle("TEXT_LOGINFAIL", "Login information is incorrect, please try again."));
                        }
                    }
                } else {
                    listener.OnLoginErr(Functions.share.getTitle("TEXT_LOGINFAIL", "Login information is incorrect, please try again."));
                }
            }

            @Override
            public void onFailure(Call<ApiObject<User>> call, Throwable t) {
                listener.OnLoginErr(Functions.share.getTitle("TEXT_LOGINFAIL", "Login information is incorrect, please try again."));
            }
        });
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
                if (taskCountComplete == Constants.asyncCallApi)
                    listener.OnGetDataSuccess();
            }

            @Override
            public void onFailure(Call<ApiList<Settings>> call, Throwable ex) {
                Log.d("Settings ERR", ex.getMessage());
            }
        });
    }

    public void getAppLanguage(String modified) {
        Call<ApiList<AppLanguage>> call = getApiRoute().getAppLanguage(CurrentUser.getInstance().getUser()==null?"1033":
                String.valueOf(CurrentUser.getInstance().getUser().getLanguage())
                ,modified);
        call.enqueue(new Callback<ApiList<AppLanguage>>() {
            @Override
            public void onResponse(Call<ApiList<AppLanguage>> call, Response<ApiList<AppLanguage>> response) {
                ApiList<AppLanguage> setting = response.body();
                assert setting != null;
                if (setting.getStatus().equals("SUCCESS")) {
                    Realm r = new RealmController().getRealm();
                    r.executeTransaction(realm -> {
                        realm.copyToRealmOrUpdate(setting.getData());
                        realm.copyToRealmOrUpdate(new DBVariable(VarsTable.APPLANGUAGE, setting.getDateNow()));
                    });
                }
                taskCountComplete++;
                if (taskCountComplete == Constants.asyncCallApi)
                    listener.OnGetDataSuccess();
            }

            @Override
            public void onFailure(Call<ApiList<AppLanguage>> call, Throwable ex) {
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
                if (taskCountComplete == Constants.asyncCallApi)
                    listener.OnGetDataSuccess();
            }

            @Override
            public void onFailure(Call<ApiList<Notify>> call, Throwable ex) {
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
                if (taskCountComplete == Constants.asyncCallApi)
                    listener.OnGetDataSuccess();
            }

            @Override
            public void onFailure(Call<ApiList<AppBase>> call, Throwable t) {
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
                if (taskCountComplete == Constants.asyncCallApi)
                    listener.OnGetDataSuccess();
            }

            @Override
            public void onFailure(Call<ApiList<Workflow>> call, Throwable t) {
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
                if (taskCountComplete == Constants.asyncCallApi)
                    listener.OnGetDataSuccess();
            }

            @Override
            public void onFailure(Call<ApiList<TimeLanguage>> call, Throwable t) {
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
                if (taskCountComplete == Constants.asyncCallApi)
                    listener.OnGetDataSuccess();
            }

            @Override
            public void onFailure(Call<ApiList<AppStatus>> call, Throwable t) {
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
                        realm.copyToRealmOrUpdate(setting.getData());
                        realm.copyToRealmOrUpdate(new DBVariable(VarsTable.USERS, setting.getDateNow()));
                    });
                }

                taskCountComplete++;
                if (taskCountComplete == Constants.asyncCallApi)
                    listener.OnGetDataSuccess();
            }

            @Override
            public void onFailure(Call<ApiList<User>> call, Throwable t) {
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
                if (taskCountComplete == Constants.asyncCallApi)
                    listener.OnGetDataSuccess();
            }

            @Override
            public void onFailure(Call<ApiList<Group>> call, Throwable t) {
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
                if (taskCountComplete == Constants.asyncCallApi)
                    listener.OnGetDataSuccess();
            }

            @Override
            public void onFailure(Call<ApiList<WorkflowFollow>> call, Throwable t) {
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
                if (taskCountComplete == Constants.asyncCallApi)
                    listener.OnGetDataSuccess();
            }

            @Override
            public void onFailure(Call<ApiList<WorkflowCategory>> call, Throwable t) {
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
                if (taskCountComplete == Constants.asyncCallApi)
                    listener.OnGetDataSuccess();
            }

            @Override
            public void onFailure(Call<ApiList<WorkflowItem>> call, Throwable t) {
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
                    if (taskCountComplete == Constants.asyncCallApi)
                        listener.OnGetDataSuccess();
                }
            }

            @Override
            public void onFailure(Call<ApiList<Position>> call, Throwable t) {
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
                Log.d("getWorkflowStepDefine ERR", t.getMessage());
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
        Call<ApiList<WorkflowStatus>> call = getApiRouteToken().getWorkflowStatus(modified, isFirst);
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
}

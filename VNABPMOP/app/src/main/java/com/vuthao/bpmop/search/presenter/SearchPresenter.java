package com.vuthao.bpmop.search.presenter;

import com.google.gson.Gson;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.api.ApiController;
import com.vuthao.bpmop.base.model.ApiList;
import com.vuthao.bpmop.base.model.ApiObject;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.model.app.AppStatus;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.Group;
import com.vuthao.bpmop.base.model.app.Notify;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.model.app.Workflow;
import com.vuthao.bpmop.base.model.app.WorkflowFollow;
import com.vuthao.bpmop.base.model.app.WorkflowStatus;
import com.vuthao.bpmop.base.model.custom.ObjectFilter;
import com.vuthao.bpmop.base.model.custom.ObjectPropertySearch;
import com.vuthao.bpmop.base.model.custom.UserAndGroup;
import com.vuthao.bpmop.base.realm.AppBaseController;
import com.vuthao.bpmop.base.realm.RealmController;

import java.util.ArrayList;
import java.util.Date;

import io.realm.Realm;
import io.realm.RealmResults;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class SearchPresenter {
    private Realm realm;
    private AppBaseController controller;
    private SearchListener listener;

    public interface SearchListener {
        void OnGetDataSuccess(ArrayList<AppBase> appBases, ObjectPropertySearch objectPropertySearch);
        void OnGetMoreDataSuccess(ArrayList<AppBase> appBases, ObjectPropertySearch objectPropertySearch);
        void OnGetDataErr();
    }
    public SearchPresenter(SearchListener listener) {
        this.listener = listener;
        realm = new RealmController().getRealm();
        controller = new AppBaseController();
    }

    public ArrayList<AppBase> getItems() {
        ArrayList<AppBase> items = new ArrayList<>();
        items.addAll(controller.getVDTItems(0, 0));

        String[] setting = Functions.share.getAppSettings(Constants.APPSTATUS_FROMME).split(",");
        Integer[] status = new Integer[setting.length];

        for (int i = 0; i < setting.length; i++) {
            status[i] = Integer.parseInt(setting[i]);
        }
        RealmResults<AppBase> results = realm.where(AppBase.class)
                .equalTo("CreatedBy", CurrentUser.getInstance().getUser().getID())
                .in("StatusGroup", status)
                .sort("Created")
                .findAll();

        items.addAll(results);

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

            if (items.get(i).getWorkflowId() > 0) {
                Workflow workflow = realm.where(Workflow.class).equalTo("WorkflowID", items.get(i).getWorkflowId()).findFirst();
                if (workflow != null) {
                    items.get(i).setWorkflow(workflow);
                }
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

    public void getFilters(ObjectPropertySearch objectPropertySearch) {
        if (objectPropertySearch == null) {
            objectPropertySearch = new ObjectPropertySearch();
            objectPropertySearch.setTotal(-1);
            objectPropertySearch.setOffset(0);
            objectPropertySearch.setLimit(Constants.mFilterLimit - 40);

            ArrayList<ObjectFilter> filters = new ArrayList<>();
            filters.add(new ObjectFilter("lcid", "eq", String.valueOf(CurrentUser.getInstance().getUser().getLanguage()), "", "text"));
            filters.add(new ObjectFilter("WorkflowId", "eq", "", "", "text"));
            filters.add(new ObjectFilter("FromDate", "gte", Functions.share.getToDay("yyyy-MM-dd", -30), "", "datetime"));
            filters.add(new ObjectFilter("lte", "eq", Functions.share.getToDay("yyyy-MM-dd"), "", "datetime"));
            filters.add(new ObjectFilter("Status", "eq", "", "", "text"));
            filters.add(new ObjectFilter("KeyWord", "contains", "", "", "text"));

            objectPropertySearch.setLstProSeach(new Gson().toJson(filters));
        }

        String data = new Gson().toJson(objectPropertySearch);
        Call<ApiObject<AppBase>> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getFilters(data);
        ObjectPropertySearch finalObjectPropertySearch = objectPropertySearch;
        call.enqueue(new Callback<ApiObject<AppBase>>() {
            @Override
            public void onResponse(Call<ApiObject<AppBase>> call, Response<ApiObject<AppBase>> response) {
                if (response.isSuccessful()) {
                    if (response.body() != null && response.body().getStatus().equals("SUCCESS")) {
                        ArrayList<AppBase> newList = modifiedDatas(response.body().getData().getData());
                        listener.OnGetDataSuccess(newList, finalObjectPropertySearch);
                    } else {
                        listener.OnGetDataErr();
                    }
                } else {
                    listener.OnGetDataErr();
                }
            }

            @Override
            public void onFailure(Call<ApiObject<AppBase>> call, Throwable t) {
                listener.OnGetDataErr();
            }
        });
    }

    public void getMoreFilters(ObjectPropertySearch objectPropertySearch) {
        String data = new Gson().toJson(objectPropertySearch);
        Call<ApiObject<AppBase>> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getFilters(data);
        ObjectPropertySearch finalObjectPropertySearch = objectPropertySearch;
        call.enqueue(new Callback<ApiObject<AppBase>>() {
            @Override
            public void onResponse(Call<ApiObject<AppBase>> call, Response<ApiObject<AppBase>> response) {
                if (response.isSuccessful()) {
                    if (response.body() != null && response.body().getStatus().equals("SUCCESS")) {
                        ArrayList<AppBase> newList = modifiedDatas(response.body().getData().getData());
                        listener.OnGetMoreDataSuccess(newList, finalObjectPropertySearch);
                    } else {
                        listener.OnGetDataErr();
                    }
                } else {
                    listener.OnGetDataErr();
                }
            }

            @Override
            public void onFailure(Call<ApiObject<AppBase>> call, Throwable t) {
                listener.OnGetDataErr();
            }
        });
    }

    public ArrayList<AppBase> modifiedDatas(ArrayList<AppBase> appBases) {
        for (AppBase base : appBases) {
            if (!Functions.isNullOrEmpty(base.getCreatedBy())) {
                User user = realm.where(User.class).equalTo("ID", base.getCreatedBy().toLowerCase()).findFirst();
                UserAndGroup userAndGroup = new UserAndGroup();
                if (user != null) {
                    userAndGroup.setID(user.getID());
                    userAndGroup.setImagePath(user.getImagePath());
                    userAndGroup.setType("0");
                } else {
                    Group group = realm.where(Group.class).equalTo("ID", base.getCreatedBy().toLowerCase()).findFirst();
                    if (group != null) {
                        userAndGroup.setID(group.getID());
                        userAndGroup.setType("1");
                    }
                }

                base.setUser(userAndGroup);
            }

            if (base.getWorkflowId() > 0) {
                Workflow workflow = realm.where(Workflow.class).equalTo("WorkflowID", base.getWorkflowId()).findFirst();
                if (workflow != null) {
                    base.setWorkflow(workflow);
                }
            }

            if (base.getApprovalStatus() != -1) {
                WorkflowStatus s = realm.where(WorkflowStatus.class).equalTo("ID", base.getApprovalStatus()).findFirst();
                if (s != null) {
                    base.setWorkflowStatus(s);
                }
            }
        }

        return appBases;
    }
}

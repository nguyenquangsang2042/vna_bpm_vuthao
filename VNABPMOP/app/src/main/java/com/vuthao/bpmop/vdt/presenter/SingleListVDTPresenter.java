package com.vuthao.bpmop.vdt.presenter;

import android.app.Activity;
import android.content.Intent;
import android.util.Log;

import com.google.gson.Gson;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.api.ApiController;
import com.vuthao.bpmop.base.model.ApiObject;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.model.app.Status;
import com.vuthao.bpmop.base.model.custom.ObjectFilter;
import com.vuthao.bpmop.base.model.custom.ObjectPropertySearch;
import com.vuthao.bpmop.base.realm.AppBaseController;
import com.vuthao.bpmop.detail.DetailWorkflowActivity;
import com.vuthao.bpmop.home.adapter.HomePageVDTAdapter;
import com.vuthao.bpmop.task.DetailCreateTaskActivity;

import java.util.ArrayList;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class SingleListVDTPresenter {
    private RefreshFilterListener refreshFilter;

    public SingleListVDTPresenter() {
    }

    public SingleListVDTPresenter(RefreshFilterListener refreshFilter) {
        this.refreshFilter = refreshFilter;
    }

    public interface RefreshFilterListener {
        void OnRefreshFilterSuccess(ArrayList<AppBase> appBases);
        void OnRefreshFilterErr();
    }

    public interface PagerSingleListVDTListener {
        void OnFilterCount(int count);
    }

    public void setReadNotify(String notifyId) {
        ArrayList<ObjectFilter> objectFilters = new ArrayList<>();
        ObjectFilter obj = new ObjectFilter();
        obj.setContentType("text");
        obj.setKey("NotifyId");
        obj.setLogicCon("eq");
        obj.setValue(notifyId);
        objectFilters.add(obj);
        String data = new Gson().toJson(objectFilters);

        new ApiController().getApiRouteToken(BaseActivity.getToken()).setReadNotify(data).enqueue(new Callback<Status>() {
            @Override
            public void onResponse(Call<Status> call, Response<Status> response) {
                Log.d("setReadNotify", "onResponse");
            }

            @Override
            public void onFailure(Call<Status> call, Throwable t) {
                Log.d("setReadNotify", "onFailure");
            }
        });
    }

    public void refreshFilterAfterSubmitAction(AppBaseController controller, ObjectPropertySearch propertySearch) {
        String data = new Gson().toJson(propertySearch);
        Call<ApiObject<AppBase>> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getListFilterMyTask(data);
        call.enqueue(new Callback<ApiObject<AppBase>>() {
            @Override
            public void onResponse(Call<ApiObject<AppBase>> call, Response<ApiObject<AppBase>> response) {
                if (response.isSuccessful()) {
                    if (response.body() != null && response.body().getData() != null) {
                        ArrayList<AppBase> apps = controller.modifiedFilters("VDT", response.body().getData().getData());
                        refreshFilter.OnRefreshFilterSuccess(apps);
                    } else {
                        refreshFilter.OnRefreshFilterErr();
                    }
                } else {
                    refreshFilter.OnRefreshFilterErr();
                }
            }

            @Override
            public void onFailure(Call<ApiObject<AppBase>> call, Throwable t) {
                refreshFilter.OnRefreshFilterErr();
            }
        });
    }

    public void handleClicks(Activity activity, AppBaseController controller, HomePageVDTAdapter adapter, AppBase appBase) {
        if (!appBase.isRead()) {
            controller.updateRead(appBase.getNotifyId());
            adapter.updateItemRead(appBase.getID());
        }

        //Tasks
        if (appBase.getResourceCategoryId() == 16) {
            Intent intent = new Intent(activity, DetailCreateTaskActivity.class);
            intent.putExtra("WorkflowItemId", Functions.share.getWorkflowItemIDByUrl(appBase.getItemUrl()));
            intent.putExtra("isClickFromAction", false);
            intent.putExtra("taskId", appBase.getID());
            activity.startActivity(intent);
        } else {
            Intent intent = new Intent(activity, DetailWorkflowActivity.class);
            intent.putExtra("WorkflowItemId", Functions.share.getWorkflowItemIDByUrl(appBase.getItemUrl()));
            activity.startActivity(intent);
        }
    }
}

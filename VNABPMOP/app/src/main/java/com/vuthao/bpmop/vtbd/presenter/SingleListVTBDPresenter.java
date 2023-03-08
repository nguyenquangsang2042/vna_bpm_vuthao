package com.vuthao.bpmop.vtbd.presenter;

import android.app.Activity;
import android.content.Intent;

import com.google.gson.Gson;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.api.ApiController;
import com.vuthao.bpmop.base.model.ApiObject;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.model.custom.ObjectPropertySearch;
import com.vuthao.bpmop.base.realm.AppBaseController;
import com.vuthao.bpmop.detail.DetailWorkflowActivity;
import com.vuthao.bpmop.task.DetailCreateTaskActivity;

import java.util.ArrayList;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class SingleListVTBDPresenter {
    private RefreshFilterListener refreshFilter;

    public interface RefreshFilterListener {
        void OnRefreshFilterSuccess(ArrayList<AppBase> appBases);
        void OnRefreshFilterErr();
    }

    public SingleListVTBDPresenter() {
    }

    public SingleListVTBDPresenter(RefreshFilterListener refreshFilter) {
        this.refreshFilter = refreshFilter;
    }

    public void refreshFilterAfterSubmitAction(AppBaseController controller, ObjectPropertySearch propertySearch) {
        String data = new Gson().toJson(propertySearch);
        Call<ApiObject<AppBase>> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getListFilterMyRequest(data);
        call.enqueue(new Callback<ApiObject<AppBase>>() {
            @Override
            public void onResponse(Call<ApiObject<AppBase>> call, Response<ApiObject<AppBase>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    if (response.body().getData() != null) {
                        ArrayList<AppBase> newData = controller.modifiedFilters("VTBD", response.body().getData().getData());
                        refreshFilter.OnRefreshFilterSuccess(newData);
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

    public void handleClicks(Activity activity, AppBase app) {
        Intent intent;
        if (app.getResourceCategoryId() == 16) {
            intent = new Intent(activity, DetailCreateTaskActivity.class);
            intent.putExtra("WorkflowItemId", Functions.share.getWorkflowItemIDByUrl(app.getItemUrl()));
            intent.putExtra("isClickFromAction", false);
            intent.putExtra("taskId", app.getID());
        } else {
            intent = new Intent(activity, DetailWorkflowActivity.class);
            intent.putExtra("WorkflowItemId", Functions.share.getWorkflowItemIDByUrl(app.getItemUrl()));
        }
        activity.startActivity(intent);
    }
}

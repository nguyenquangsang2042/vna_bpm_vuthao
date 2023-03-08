package com.vuthao.bpmop.home.presenter;

import android.app.Activity;
import android.content.Intent;
import android.util.Log;
import android.widget.TextView;

import com.google.gson.Gson;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.api.ApiController;
import com.vuthao.bpmop.base.api.ApiBPM;
import com.vuthao.bpmop.base.model.ApiObject;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.model.app.AppStatus;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.Status;
import com.vuthao.bpmop.base.model.custom.ObjectFilter;
import com.vuthao.bpmop.base.model.custom.ObjectPropertySearch;
import com.vuthao.bpmop.base.realm.AppBaseController;
import com.vuthao.bpmop.detail.DetailWorkflowActivity;
import com.vuthao.bpmop.home.adapter.HomePageVDTAdapter;
import com.vuthao.bpmop.home.adapter.HomePageVTBDAdapter;
import com.vuthao.bpmop.task.DetailCreateTaskActivity;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class HomePagePresenter {
    private HomePageListener listener;
    private AppBaseController appBaseController;
    private RefreshFilter refreshFilter;
    private ApiBPM apiBPM;

    public interface HomePageFilterListener {
        void OnFilterCount(int count);
    }

    public interface HomePageListener {
        void OnFilterSuccess(ArrayList<AppBase> apps, String type, String status, ObjectPropertySearch propertySearch);

        void OnFilterErr(String err);

        void OnDefaultFilter(String type);

        void OnFilterDissmiss();
    }

    public interface RefreshFilter {
        void OnRefreshActionFilterSuccess(ArrayList<AppBase> appBases);
        void OnRefreshActionFilterErr();
    }

    public HomePagePresenter() {
    }

    public HomePagePresenter(RefreshFilter refreshFilter) {
        this.refreshFilter = refreshFilter;
    }

    public HomePagePresenter(HomePageListener listener, AppBaseController appBaseController) {
        this.listener = listener;
        this.appBaseController = appBaseController;
        apiBPM = new ApiBPM();
    }

    public HomePagePresenter(HomePageListener listener, RefreshFilter refreshFilter, AppBaseController appBaseController) {
        this.listener = listener;
        this.refreshFilter = refreshFilter;
        this.appBaseController = appBaseController;
        apiBPM = new ApiBPM();
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

    public void refreshActionWithFilter(String type, AppBaseController controller, ObjectPropertySearch propertySearch) {
        String data = new Gson().toJson(propertySearch);
        Call<ApiObject<AppBase>> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getListFilterMyTask(data);
        call.enqueue(new Callback<ApiObject<AppBase>>() {
            @Override
            public void onResponse(Call<ApiObject<AppBase>> call, Response<ApiObject<AppBase>> response) {
                if (response.isSuccessful()) {
                    if (response.body() != null) {
                        if (response.body().getData() != null) {
                            ArrayList<AppBase> apps = controller.modifiedFilters(type, response.body().getData().getData());
                            refreshFilter.OnRefreshActionFilterSuccess(apps);
                        } else {
                            refreshFilter.OnRefreshActionFilterErr();
                        }
                    } else {
                        refreshFilter.OnRefreshActionFilterErr();
                    }
                }
            }

            @Override
            public void onFailure(Call<ApiObject<AppBase>> call, Throwable t) {
                refreshFilter.OnRefreshActionFilterErr();
            }
        });
    }

    public void bindTextTrangThai(TextView tvTrangThai, ArrayList<AppStatus> lstTrangThai) {
        if (lstTrangThai.get(0).isSelected()) {
            tvTrangThai.setText(Functions.share.getTitle("TEXT_ALL", "Tất cả"));
            return;
        }

        List<AppStatus> result = lstTrangThai.stream()
                .filter(AppStatus::isSelected)
                .collect(Collectors.toList());

        String title;
        if (CurrentUser.getInstance().getUser()
                .getLanguage() == Integer.parseInt(Constants.mLangVN)) {
            title = result.get(0).getTitle();
        } else {
            title = result.get(0).getTitleEN();
        }

        if (result.size() > 1) {
            tvTrangThai.setText(String.format("%s, (+%s)", title, result.size() - 1));
        } else {
            tvTrangThai.setText(title);
        }
    }

    public void filterVDTApi(HashMap<String, String> hashMap, int limit, int offset, int total, String type, String status) {
        ArrayList<ObjectFilter> objectFilters = handleObjectFilter(hashMap);
        ObjectPropertySearch propertySearch = new ObjectPropertySearch();
        propertySearch.setLstProSeach(new Gson().toJson(objectFilters));
        propertySearch.setLimit(limit);
        propertySearch.setOffset(offset);
        propertySearch.setTotal(total);

        String data = new Gson().toJson(propertySearch);

        Call<ApiObject<AppBase>> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getListFilterMyTask(data);
        call.enqueue(new Callback<ApiObject<AppBase>>() {
            @Override
            public void onResponse(Call<ApiObject<AppBase>> call, Response<ApiObject<AppBase>> response) {
                if (response.isSuccessful()) {
                    if (response.body() != null && response.body().getStatus().equals("SUCCESS")) {
                        listener.OnFilterSuccess(response.body().getData().getData(), type, status, propertySearch);
                    } else {
                        listener.OnFilterErr("Có lỗi xảy ra");
                    }
                }
            }

            @Override
            public void onFailure(Call<ApiObject<AppBase>> call, Throwable t) {
                listener.OnFilterErr("Có lỗi xảy ra");
            }
        });
    }

    public void refreshFilters(String type, ObjectPropertySearch propertySearch) {
        String data = new Gson().toJson(propertySearch);

        Call<ApiObject<AppBase>> call;
        if (type.equals("VDT")) {
            call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getListFilterMyTask(data);

        } else {
            call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getListFilterMyRequest(data);
        }

        call.enqueue(new Callback<ApiObject<AppBase>>() {
            @Override
            public void onResponse(Call<ApiObject<AppBase>> call, Response<ApiObject<AppBase>> response) {
                if (response.isSuccessful()) {
                    if (response.body() != null) {
                        if (response.body().getStatus().equals("SUCCESS")) {
                            listener.OnFilterSuccess(response.body().getData().getData(), type, "", propertySearch);
                        }
                    }
                }
            }

            @Override
            public void onFailure(Call<ApiObject<AppBase>> call, Throwable t) {
                Log.d("ERR getListFilterMyTask", t.getMessage());
            }
        });
    }

    public void filterVTDBApi(HashMap<String, String> hashMap, int limit, int offset, int total) {
        ArrayList<ObjectFilter> objectFilters = handleObjectFilter(hashMap);

        ObjectPropertySearch propertySearch = new ObjectPropertySearch();
        propertySearch.setLstProSeach(new Gson().toJson(objectFilters));
        propertySearch.setLimit(limit);
        propertySearch.setOffset(offset);
        propertySearch.setTotal(total);

        String data = new Gson().toJson(propertySearch);

        Call<ApiObject<AppBase>> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getListFilterMyRequest(data);
        call.enqueue(new Callback<ApiObject<AppBase>>() {
            @Override
            public void onResponse(Call<ApiObject<AppBase>> call, Response<ApiObject<AppBase>> response) {
                if (response.isSuccessful()) {
                    if (response.body().getStatus().equals("SUCCESS")) {
                        listener.OnFilterSuccess(response.body().getData().getData(), "VTBD", "", propertySearch);
                    }
                }
            }

            @Override
            public void onFailure(Call<ApiObject<AppBase>> call, Throwable t) {
                Log.d("ERR getListFilterMyTask", t.getMessage());
            }
        });
    }

    private ArrayList<ObjectFilter> handleObjectFilter(HashMap<String, String> hashMap) {
        ArrayList<ObjectFilter> _result = new ArrayList<ObjectFilter>();

        for (Map.Entry<String, String> entry : hashMap.entrySet()) {
            switch (entry.getKey()) {
                case "resourceviewid":
                    _result.add(new ObjectFilter("ResourceViewID", "eq", entry.getValue(), "", "text"));
                    break;
                case "viewtype":
                    _result.add(new ObjectFilter("ViewType", "eq", entry.getValue(), "", "text"));
                    break;
                case "statusgroup":
                    _result.add(new ObjectFilter("StatusGroup", "in", entry.getValue(), "", "text"));
                    break;
                case "duedate-gte":
                    _result.add(new ObjectFilter("DueDate", "gte", entry.getValue(), "", "datetime"));
                    break;
                case "duedate-lte":
                    _result.add(new ObjectFilter("DueDate", "lte", entry.getValue(), "", "datetime"));
                    break;
                case "created-gte":
                    _result.add(new ObjectFilter("Created", "gte", entry.getValue(), "", "date"));
                    break;
                case "created-lte":
                    _result.add(new ObjectFilter("Created", "lte", entry.getValue().replace("00:00", "23:59"), "", "date"));
                    break;
                case "lcid":
                    _result.add(new ObjectFilter("lcid", "eq", entry.getValue(), "", "text"));
                    break;
                case "workflowid":
                    _result.add(new ObjectFilter("WorkflowId", "eq", entry.getValue(), "", "text"));
                    break;
            }
        }

        return _result;
    }

    public void hanldeVDTClicks(Activity activity, AppBase appBase, AppBaseController controller, HomePageVDTAdapter adapter) {
        if (!appBase.isRead()) {
            controller.updateRead(appBase.getNotifyId());
            adapter.updateItemRead(appBase.getID());
        }

        if (appBase.getResourceCategoryId() == 16) {
            // Task
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

    public void hanldeVTBDClicks(Activity activity, AppBase appBase, AppBaseController controller, HomePageVTBDAdapter adapter) {
        if (!appBase.isRead()) {
            adapter.updateItemRead(appBase.getID());
        }

        if (appBase.getResourceCategoryId() == 16) {
            // Task
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

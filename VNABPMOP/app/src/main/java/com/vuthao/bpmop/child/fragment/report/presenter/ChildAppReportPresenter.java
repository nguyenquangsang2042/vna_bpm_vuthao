package com.vuthao.bpmop.child.fragment.report.presenter;

import android.app.Activity;
import android.content.Intent;
import android.util.Log;

import com.google.gson.Gson;
import com.google.gson.JsonObject;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.api.ApiController;
import com.vuthao.bpmop.base.model.ApiList;
import com.vuthao.bpmop.base.model.ApiObject;
import com.vuthao.bpmop.base.model.app.AppStatus;
import com.vuthao.bpmop.base.model.app.Group;
import com.vuthao.bpmop.base.model.app.ResourceView;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.model.custom.DetailList;
import com.vuthao.bpmop.base.model.custom.ObjectFilter;
import com.vuthao.bpmop.base.model.custom.ObjectPropertySearch;
import com.vuthao.bpmop.base.model.custom.UserAndGroup;
import com.vuthao.bpmop.base.realm.RealmController;
import com.vuthao.bpmop.detail.DetailWorkflowActivity;

import org.json.JSONArray;
import org.json.JSONObject;

import java.util.ArrayList;

import io.realm.Realm;
import io.realm.RealmResults;
import io.realm.Sort;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ChildAppReportPresenter {
    private Realm realm;
    private ChildAppReportListener listener;

    public ChildAppReportPresenter(ChildAppReportListener listener) {
        this.listener = listener;
        realm = new RealmController().getRealm();
    }

    public interface ChildAppReportListener {
        void OnGetDataSuccess(ArrayList<DetailList.Headers> headers, ArrayList<JSONObject> lstJObjectDynamic, ObjectPropertySearch objectPropertySearch);

        void OnGetMoreDataSuccess(ArrayList<DetailList.Headers> headers, ArrayList<JSONObject> lstJObjectDynamic, ObjectPropertySearch objectPropertySearch);

        void OnGetChartColumsSuccess(ArrayList<DetailList.Headers> columns);

        void OnGetChartColumsErr();

        void OnGetWorkflowChartSuccess(ArrayList<JSONObject> workflowCharts);

        void OnGetWorkflowChartErr();

        void OnGetDataErr();
    }

    public ArrayList<ResourceView> getReports(int workflowId) {
        RealmResults<ResourceView> results = realm.where(ResourceView.class)
                .equalTo("ResourceId", workflowId)
                .and()
                .equalTo("Status", 1)
                .and()
                .equalTo("TypeId", 1)
                .and()
                .equalTo("ViewType", 1)
                .sort(new String[]{"Index", "MenuId"}, new Sort[]{Sort.DESCENDING, Sort.ASCENDING})
                .findAll();

        return new ArrayList<>(results);
    }

    public void getDynamicMoreFormField(ObjectPropertySearch objectPropertySearch) {
        String data = new Gson().toJson(objectPropertySearch);
        Call<ApiObject<DetailList>> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getDynamicFormField(data);
        call.enqueue(new Callback<ApiObject<DetailList>>() {
            @Override
            public void onResponse(Call<ApiObject<DetailList>> call, Response<ApiObject<DetailList>> response) {
                if (response.isSuccessful()) {
                    getDynamicMoreWorkflowItem(response.body().getData().getData(), objectPropertySearch);
                } else {
                    listener.OnGetDataErr();
                }
            }

            @Override
            public void onFailure(Call<ApiObject<DetailList>> call, Throwable t) {
                listener.OnGetDataErr();
            }
        });
    }

    private void getDynamicMoreWorkflowItem(ArrayList<DetailList.Headers> headers, ObjectPropertySearch objectPropertySearch) {
        String data = new Gson().toJson(objectPropertySearch);
        Call<JsonObject> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getDynamicWorkflowItem(data);
        call.enqueue(new Callback<JsonObject>() {
            @Override
            public void onResponse(Call<JsonObject> call, Response<JsonObject> response) {
                if (response.isSuccessful()) {
                    try {
                        JSONObject jsonObject = new JSONObject(new Gson().toJson(response.body()));
                        JSONArray jsonArray = jsonObject.getJSONObject("data").getJSONArray("Data");
                        ArrayList<JSONObject> jsonObjects = new ArrayList<>();
                        for (int i = 0; i < jsonArray.length(); i++) {
                            jsonObjects.add(jsonArray.getJSONObject(i));
                        }

                        listener.OnGetMoreDataSuccess(headers, jsonObjects, objectPropertySearch);
                    } catch (Exception ex) {
                        Log.d("ERR", ex.getMessage());
                    }
                } else {
                    listener.OnGetDataErr();
                }
            }

            @Override
            public void onFailure(Call<JsonObject> call, Throwable t) {
                listener.OnGetDataErr();
            }
        });
    }

    public void getWorkflowChart(int resourceViewId) {
        ArrayList<ObjectFilter> lstProSearch = new ArrayList<>();
        lstProSearch.add(new ObjectFilter("ResourceViewID", "eq", String.valueOf(resourceViewId), "", ""));
        ObjectPropertySearch objectPropertySearch = new ObjectPropertySearch();
        objectPropertySearch.setLstProSeach(new Gson().toJson(lstProSearch));
        objectPropertySearch.setLimit(-1);

        String data = new Gson().toJson(objectPropertySearch);
        Call<JsonObject> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getWorkflowChart(data);
        call.enqueue(new Callback<JsonObject>() {
            @Override
            public void onResponse(Call<JsonObject> call, Response<JsonObject> response) {
                if (response.isSuccessful()) {
                    try {
                        JSONObject jsonObject = new JSONObject(new Gson().toJson(response.body()));
                        if (jsonObject.get("status").equals("SUCCESS")) {
                            JSONArray jsonArray = jsonObject.getJSONObject("data").getJSONArray("Data");
                            ArrayList<JSONObject> jsonObjects = new ArrayList<>();
                            for (int i = 0; i < jsonArray.length(); i++) {
                                jsonObjects.add(jsonArray.getJSONObject(i));
                            }

                            listener.OnGetWorkflowChartSuccess(jsonObjects);
                        } else {
                            listener.OnGetWorkflowChartErr();
                        }

                    } catch (Exception ex) {
                        Log.d("ERR", ex.getMessage());
                        listener.OnGetWorkflowChartErr();
                    }
                }
            }

            @Override
            public void onFailure(Call<JsonObject> call, Throwable t) {
                listener.OnGetWorkflowChartErr();
            }
        });
    }

    // Get Description for chart
    public String getDescription(DetailList.Headers header, JSONObject currentJObjectRow) {
        String result = "";
        try {
            Object object;
            if (!Functions.isNullOrEmpty(header.getFieldMapping())) {
                if (currentJObjectRow.has(header.getFieldMapping())) {
                    object = currentJObjectRow.get(header.getFieldMapping());
                } else {
                    object = "";
                }
            } else if (!Functions.isNullOrEmpty(header.getInternalName())) {
                if (currentJObjectRow.has(header.getInternalName())) {
                    object = currentJObjectRow.get(header.getInternalName());
                } else {
                    object = "";
                }
            } else {
                object = "";
            }

            result = object.toString();
        } catch (Exception ex) {
            Log.d("ERR getDescriptionForChart", ex.getMessage());
        }

        return result;
    }

    // Get value for chart
    public String getRawValue(DetailList.Headers header, JSONObject currentJObjectRow) {
        String result = "";
        try {
            Object object;
            if (!Functions.isNullOrEmpty(header.getFieldMapping())) {
                if (currentJObjectRow.has(header.getFieldMapping())) {
                    object = currentJObjectRow.get("Value");
                } else {
                    object = "";
                }
            } else if (!Functions.isNullOrEmpty(header.getInternalName())) {
                if (currentJObjectRow.has(header.getInternalName())) {
                    object = currentJObjectRow.get("Value");
                } else {
                    object = "";
                }
            } else {
                object = "";
            }

            result = object.toString();
        } catch (Exception ex) {
            Log.d("ERR getRawValueForChart", ex.getMessage());
        }

        return result;
    }

    public AppStatus getStatus(String id) {
        AppStatus status = new RealmController().getRealm().where(AppStatus.class)
                .equalTo("ID", Integer.parseInt(id))
                .findFirst();
        return status;
    }

    public UserAndGroup getUserChart(String id) {
        UserAndGroup userAndGroup = new UserAndGroup();
        User user = new RealmController().getRealm().where(User.class)
                .equalTo("ID", id.toLowerCase())
                .findFirst();
        if (user != null) {
            userAndGroup.setID(user.getID());
            userAndGroup.setName(user.getFullName());
            userAndGroup.setType("0");
        } else {
            Group group = new RealmController().getRealm().where(Group.class)
                    .equalTo("ID", id.toLowerCase())
                    .findFirst();
            if (group != null) {
                userAndGroup.setID(group.getID());
                userAndGroup.setName(group.getTitle());
                userAndGroup.setType("1");
            }
        }

        return userAndGroup;
    }

    public void getChartColunms(int resourceViewId) {
        ArrayList<ObjectFilter> lstProSearch = new ArrayList<>();
        lstProSearch.add(new ObjectFilter("ResourceViewID", "eq", String.valueOf(resourceViewId), "", ""));
        ObjectPropertySearch objectPropertySearch = new ObjectPropertySearch();
        objectPropertySearch.setLstProSeach(new Gson().toJson(lstProSearch));
        objectPropertySearch.setLimit(-1);

        String data = new Gson().toJson(objectPropertySearch);
        Call<ApiObject<DetailList>> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getChartColumn(data);
        call.enqueue(new Callback<ApiObject<DetailList>>() {
            @Override
            public void onResponse(Call<ApiObject<DetailList>> call, Response<ApiObject<DetailList>> response) {
                if (response.isSuccessful()) {
                    if (response.body() != null && response.body().getStatus().equals("SUCCESS")) {
                        listener.OnGetChartColumsSuccess(response.body().getData().getData());
                    } else {
                        listener.OnGetChartColumsErr();
                    }
                } else {
                    listener.OnGetChartColumsErr();
                }
            }

            @Override
            public void onFailure(Call<ApiObject<DetailList>> call, Throwable t) {
                listener.OnGetChartColumsErr();
            }
        });
    }

    public void getDynamicFormField(int resourceViewId) {
        ArrayList<ObjectFilter> lstProSearch = new ArrayList<>();
        lstProSearch.add(new ObjectFilter("ResourceViewID", "eq", String.valueOf(resourceViewId), "", ""));
        ObjectPropertySearch objectPropertySearch = new ObjectPropertySearch();
        objectPropertySearch.setLstProSeach(new Gson().toJson(lstProSearch));
        objectPropertySearch.setLimit(Constants.mFilterLimit - 40);
        objectPropertySearch.setOffset(0);
        objectPropertySearch.setTotal(-1);

        String data = new Gson().toJson(objectPropertySearch);
        Call<ApiObject<DetailList>> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getDynamicFormField(data);
        call.enqueue(new Callback<ApiObject<DetailList>>() {
            @Override
            public void onResponse(Call<ApiObject<DetailList>> call, Response<ApiObject<DetailList>> response) {
                if (response.isSuccessful()) {
                    if (response.body().getStatus().equals("SUCCESS")) {
                        getDynamicWorkflowItem(response.body().getData().getData(), objectPropertySearch);
                    } else {
                        listener.OnGetDataErr();
                    }
                } else {
                    listener.OnGetDataErr();
                }
            }

            @Override
            public void onFailure(Call<ApiObject<DetailList>> call, Throwable t) {
                listener.OnGetDataErr();
            }
        });
    }

    private void getDynamicWorkflowItem(ArrayList<DetailList.Headers> headers, ObjectPropertySearch objectPropertySearch) {
        String data = new Gson().toJson(objectPropertySearch);
        Call<JsonObject> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getDynamicWorkflowItem(data);
        call.enqueue(new Callback<JsonObject>() {
            @Override
            public void onResponse(Call<JsonObject> call, Response<JsonObject> response) {
                if (response.isSuccessful()) {
                    try {
                        JSONObject jsonObject = new JSONObject(new Gson().toJson(response.body()));
                        if (jsonObject.get("status").equals("SUCCESS")) {
                            JSONArray jsonArray = jsonObject.getJSONObject("data").getJSONArray("Data");
                            ArrayList<JSONObject> jsonObjects = new ArrayList<>();
                            for (int i = 0; i < jsonArray.length(); i++) {
                                jsonObjects.add(jsonArray.getJSONObject(i));
                            }

                            listener.OnGetDataSuccess(headers, jsonObjects, objectPropertySearch);
                        } else {
                            listener.OnGetDataErr();
                        }

                    } catch (Exception ex) {
                        Log.d("ERR", ex.getMessage());
                        listener.OnGetDataErr();
                    }
                } else {
                    listener.OnGetDataErr();
                }
            }

            @Override
            public void onFailure(Call<JsonObject> call, Throwable t) {
                listener.OnGetDataErr();
            }
        });
    }

    public void handleClicks(Activity activity, JSONObject object) {
        try {
            Intent intent = new Intent(activity, DetailWorkflowActivity.class);
            intent.putExtra("WorkflowItemId", object.get("ID").toString());
            activity.startActivity(intent);
        } catch (Exception ex) {
            Log.d("ERR handleClicks", ex.getMessage());
        }
    }

}

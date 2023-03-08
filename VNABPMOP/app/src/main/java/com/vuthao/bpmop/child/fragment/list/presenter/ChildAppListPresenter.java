package com.vuthao.bpmop.child.fragment.list.presenter;

import android.app.Activity;
import android.content.Intent;
import android.util.Log;

import com.google.gson.Gson;
import com.google.gson.JsonObject;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.api.ApiController;
import com.vuthao.bpmop.base.model.ApiObject;
import com.vuthao.bpmop.base.model.app.ResourceView;
import com.vuthao.bpmop.base.model.app.WorkflowItem;
import com.vuthao.bpmop.base.model.custom.DetailList;
import com.vuthao.bpmop.base.model.custom.ObjectFilter;
import com.vuthao.bpmop.base.model.custom.ObjectPropertySearch;
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

public class ChildAppListPresenter {
    private Realm realm;
    private ChildAppListListener listener;

    public ChildAppListPresenter(ChildAppListListener listener) {
        this.listener = listener;
        realm = new RealmController().getRealm();
    }

    public interface ChildAppListListener {
        void OnGetDataSuccess(ArrayList<DetailList.Headers> headers, ArrayList<JSONObject> lstJObjectDynamic, ObjectPropertySearch objectPropertySearch);

        void OnGetMoreDataSuccess(ArrayList<DetailList.Headers> headers, ArrayList<JSONObject> lstJObjectDynamic, ObjectPropertySearch objectPropertySearch);

        void OnGetDataErr();
    }

    public ChildAppListPresenter() {
        realm = new RealmController().getRealm();
    }

    public ArrayList<ResourceView> getListResource(int workflowId) {
        RealmResults<ResourceView> results = realm.where(ResourceView.class)
                .equalTo("ResourceId", workflowId)
                .and()
                .equalTo("Status", 1)
                .and()
                .equalTo("TypeId", 0)
                .and()
                .equalTo("ViewType", 1)
                .greaterThanOrEqualTo("Index", 0)
                .sort("MenuId", Sort.ASCENDING)
                //.sort(new String[]{"Index", "MenuId"}, new Sort[]{Sort.DESCENDING, Sort.ASCENDING})
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
                    if (response.body() != null && response.body().getData() != null) {
                        getDynamicMoreWorkflowItem(response.body().getData().getData(), objectPropertySearch);
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

    public void getDynamicFormField(int resourceViewId, ObjectPropertySearch objectPropertySearch) {
        if (objectPropertySearch == null) {
            ArrayList<ObjectFilter> lstProSearch = new ArrayList<>();
            lstProSearch.add(new ObjectFilter("ResourceViewID", "eq", String.valueOf(resourceViewId), "", ""));
            objectPropertySearch = new ObjectPropertySearch();
            objectPropertySearch.setLstProSeach(new Gson().toJson(lstProSearch));
            objectPropertySearch.setLimit(Constants.mFilterLimit - 40);
            objectPropertySearch.setOffset(0);
            objectPropertySearch.setTotal(-1);
        }

        String data = new Gson().toJson(objectPropertySearch);
        Call<ApiObject<DetailList>> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getDynamicFormField(data);
        ObjectPropertySearch finalObjectPropertySearch = objectPropertySearch;
        call.enqueue(new Callback<ApiObject<DetailList>>() {
            @Override
            public void onResponse(Call<ApiObject<DetailList>> call, Response<ApiObject<DetailList>> response) {
                if (response.isSuccessful()) {
                    if (response.body() != null && response.body().getStatus().equals("SUCCESS")) {
                        getDynamicWorkflowItem(response.body().getData().getData(), finalObjectPropertySearch);
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

    public void getDynamicWorkflowItem(ArrayList<DetailList.Headers> headers, ObjectPropertySearch objectPropertySearch) {
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
                            if (jsonArray.equals("[]")) {
                                listener.OnGetDataErr();
                            } else {
                                ArrayList<JSONObject> jsonObjects = new ArrayList<>();
                                for (int i = 0; i < jsonArray.length(); i++) {
                                    jsonObjects.add(jsonArray.getJSONObject(i));
                                }

                                listener.OnGetDataSuccess(headers, jsonObjects, objectPropertySearch);
                            }
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

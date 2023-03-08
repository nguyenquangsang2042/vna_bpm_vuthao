package com.vuthao.bpmop.board.presenter;

import android.app.Activity;
import android.app.Dialog;
import android.content.Intent;
import android.graphics.Color;
import android.graphics.drawable.ColorDrawable;
import android.util.Log;
import android.view.Gravity;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.api.ApiController;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.Status;
import com.vuthao.bpmop.base.model.app.Workflow;
import com.vuthao.bpmop.base.model.app.WorkflowCategory;
import com.vuthao.bpmop.base.model.custom.BoardWorkflow;
import com.vuthao.bpmop.base.realm.BoardController;
import com.vuthao.bpmop.base.realm.RealmController;
import com.vuthao.bpmop.board.adapter.BoardChooseCategoryAdapter;
import com.vuthao.bpmop.board.adapter.ExpandBoardMainGroupAdapter;
import com.vuthao.bpmop.child.activity.ChildTabsActivity;

import org.json.JSONObject;

import java.util.ArrayList;
import java.util.HashMap;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class BoardPresenter implements Callback {

    public void setFavorite(int workflowId, boolean flag) {
        try {
            JSONObject json = new JSONObject();
            json.put("Flag", flag ? 1 : 0);
            json.put("WorkflowId", workflowId);

            HashMap<String, Object> hashMap = new HashMap<>();
            hashMap.put("data", json.toString());
            Call<Status> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).setFavoriteWorkflow(hashMap);
            call.enqueue(this);
        } catch (Exception ex) {
            Log.d("ERR setFavorite", ex.getMessage());
        }
    }

    public void handleClicks(Activity activity, Workflow workflow) {
        Intent intent = new Intent(activity, ChildTabsActivity.class);
        intent.putExtra("workflow", new Gson().toJson(new RealmController().getRealm().copyFromRealm(workflow)));
        activity.startActivityForResult(intent, 100);
    }

    @Override
    public void onResponse(Call call, Response response) {
        Log.d("onResponse", response.toString());
    }

    @Override
    public void onFailure(Call call, Throwable t) {

    }
}

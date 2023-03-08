package com.vuthao.bpmop.qrcode.presenter;

import android.content.Intent;
import android.net.Uri;

import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.api.ApiController;
import com.vuthao.bpmop.base.model.ApiList;
import com.vuthao.bpmop.base.model.app.Workflow;
import com.vuthao.bpmop.base.model.app.WorkflowItem;
import com.vuthao.bpmop.base.realm.RealmController;
import com.vuthao.bpmop.detail.DetailWorkflowActivity;

import io.realm.Realm;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class QRCodePresenter {
    private QRCodeListener listener;

    public interface QRCodeListener {
        void GetWorkflowSuccess(WorkflowItem workflowItem);
        void GetWorkflowErr(String err);
    }

    public QRCodePresenter() {
    }

    public QRCodePresenter(QRCodeListener listener) {
        this.listener = listener;
    }

    public WorkflowItem getWorkflowItemByQRCode(String listId, String itemId) {
        Realm realm = new RealmController().getRealm();
        WorkflowItem workflowItem = realm.where(WorkflowItem.class)
                .equalTo("ItemID", Integer.parseInt(itemId))
                .equalTo("ListId", listId.toLowerCase())
                .findFirst();

        return workflowItem;
    }


    public void getWorkflowItemById(String rid) {
        Call<ApiList<WorkflowItem>> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getWorkflowItemById(rid);
        call.enqueue(new Callback<ApiList<WorkflowItem>>() {
            @Override
            public void onResponse(Call<ApiList<WorkflowItem>> call, Response<ApiList<WorkflowItem>> response) {
                if (response.isSuccessful()) {
                    if (response.body().getStatus().equals("SUCCESS")) {
                        listener.GetWorkflowSuccess(response.body().getData().get(0));
                    } else {
                        listener.GetWorkflowErr("Không tìm thấy thông tin phiếu");
                    }
                } else {
                    listener.GetWorkflowErr("Không tìm thấy thông tin phiếu");
                }
            }

            @Override
            public void onFailure(Call<ApiList<WorkflowItem>> call, Throwable t) {
                listener.GetWorkflowErr("Không tìm thấy thông tin phiếu");
            }
        });
    }
}

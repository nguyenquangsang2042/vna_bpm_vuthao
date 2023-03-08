package com.vuthao.bpmop.base.api;

import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.ApiList;
import com.vuthao.bpmop.base.model.ApiObject;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.Status;
import com.vuthao.bpmop.base.model.custom.FormDetailInfo;
import com.vuthao.bpmop.base.model.custom.WorkflowHistory;
import com.vuthao.bpmop.detail.custom.DetailFunc;

import java.net.ConnectException;
import java.net.UnknownHostException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

import okhttp3.MultipartBody;
import okhttp3.RequestBody;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ApiWorkflowController extends ApiController {

    public void getTicketRequestControlDynamicForm(String wId, DetailWorkflowListener callback) {

        Call<ApiObject<FormDetailInfo>> call = getApiRouteToken().getTicketRequestControlDynamicForm(wId, String.valueOf(CurrentUser.getInstance().getUser().getLanguage()));
        call.enqueue(new Callback<ApiObject<FormDetailInfo>>() {
            @Override
            public void onResponse(Call<ApiObject<FormDetailInfo>> call, Response<ApiObject<FormDetailInfo>> response) {
                if (response.isSuccessful()) {
                    if (response.body().getStatus().equals("SUCCESS")) {
                        assert response.body() != null;
                        callback.OnGetWorkflowDynamicSucess(response.body().getData());
                    } else {
                        callback.OnGetWorkflowErr(Functions.share.getTitle("MESS_WORKFLOW_NOTFOUND", "Không tìm thấy thông tin phiếu, vui lòng thử lại sau!"));
                    }
                } else {
                    callback.OnGetWorkflowErr(Functions.share.getTitle("MESS_WORKFLOW_NOTFOUND", "Không tìm thấy thông tin phiếu, vui lòng thử lại sau!"));
                }
            }

            @Override
            public void onFailure(Call<ApiObject<FormDetailInfo>> call, Throwable e) {
                if (e instanceof UnknownHostException || e instanceof ConnectException) {
                    callback.OnGetWorkflowErr(Functions.share.getTitle("TEXT_INTERNET", "Không có kết nối mạng!"));
                } else {
                    callback.OnGetWorkflowErr(Functions.share.getTitle("MESS_WORKFLOW_NOTFOUND", "Không tìm thấy thông tin phiếu, vui lòng thử lại sau!"));
                }
            }
        });
    }

    public void getListProcessHistory(String workflowId, WorkflowHistoryListener listener) {
        Call<ApiList<WorkflowHistory>> call = getApiRouteToken().getListProcessHistory(workflowId);
        call.enqueue(new Callback<ApiList<WorkflowHistory>>() {
            @Override
            public void onResponse(Call<ApiList<WorkflowHistory>> call, Response<ApiList<WorkflowHistory>> response) {
                if (response.isSuccessful()) {
                    assert response.body() != null;
                    if (response.body().getStatus().equals("SUCCESS")) {
                        listener.OnGetWorkflowHistorySuccess(response.body().getData());
                    }
                }
            }

            @Override
            public void onFailure(Call<ApiList<WorkflowHistory>> call, Throwable t) {
            }
        });
    }

    public void updateFollow(HashMap<String, String> hashMap, ApiWorkflowListener listener) {
        HashMap<String, RequestBody> map = new HashMap<>();
        for (Map.Entry<String, String> entry : hashMap.entrySet()) {
            map.put(entry.getKey(), DetailFunc.share.toRequestBody(entry.getValue()));
        }

        MultipartBody.Part[] filesComment = new MultipartBody.Part[1];
        filesComment[0] = MultipartBody.Part.createFormData("", "");

        Call<Status> call = getApiRouteToken().sendControlDynamicAction(filesComment, map);
        call.enqueue(new Callback<Status>() {
            @Override
            public void onResponse(Call<Status> call, Response<Status> response) {
                if (response.isSuccessful()) {
                    if (response.body().getStatus().equals("SUCCESS")) {
                        listener.OnGetDataSuccess();
                    }
                }
            }

            @Override
            public void onFailure(Call<Status> call, Throwable e) {
                if (e instanceof UnknownHostException || e instanceof ConnectException) {
                    listener.OnGetDataError(Functions.share.getTitle("MESS_REQUIRE_NETWORK", "No network connection, please try again."));
                } else {
                    listener.OnGetDataError(Functions.share.getTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"));
                }
            }
        });
    }

    public interface WorkflowHistoryListener {
        void OnGetWorkflowHistorySuccess(ArrayList<WorkflowHistory> histories);
    }

    public interface DetailWorkflowListener {
        void OnGetWorkflowDynamicSucess(FormDetailInfo formDetailInfo);

        void OnGetWorkflowErr(String err);
    }

    public interface ApiWorkflowListener {
        void OnGetDataSuccess();

        void OnGetDataError(String err);
    }
}

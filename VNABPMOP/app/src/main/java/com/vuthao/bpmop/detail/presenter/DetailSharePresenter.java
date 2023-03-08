package com.vuthao.bpmop.detail.presenter;

import android.content.Context;
import android.util.Log;

import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.api.ApiBPM;
import com.vuthao.bpmop.base.api.ApiController;
import com.vuthao.bpmop.base.model.ApiList;
import com.vuthao.bpmop.base.model.app.WorkflowItem;
import com.vuthao.bpmop.base.model.custom.GroupShareHistory;
import com.vuthao.bpmop.base.model.custom.ShareHistory;
import com.vuthao.bpmop.base.model.custom.UserAndGroup;
import com.vuthao.bpmop.detail.custom.DetailFunc;

import org.json.JSONObject;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;
import java.util.stream.Collectors;

import okhttp3.MultipartBody;
import okhttp3.RequestBody;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class DetailSharePresenter {
    private Context context;
    private DetailShareListener listener;
    private ApiBPM apiBPM;

    public interface DetailShareListener {
        void OnGetShareSuccess(ArrayList<ShareHistory> shareHistories);
        void OnShareSuccess();
        void OnShareErr(String err);
    }

    public DetailSharePresenter(Context context, DetailShareListener listener) {
        this.context = context;
        this.listener = listener;
        apiBPM = new ApiBPM();
    }

    public void getListShareHistory(String fid) {
        Call<ApiList<ShareHistory>> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getListShareHistory(fid);
        call.enqueue(new Callback<ApiList<ShareHistory>>() {
            @Override
            public void onResponse(Call<ApiList<ShareHistory>> call, Response<ApiList<ShareHistory>> response) {
                if (response.isSuccessful()) {
                    if (response.body().getStatus().equals("SUCCESS")) {
                        listener.OnGetShareSuccess(response.body().getData());
                    }
                }
            }

            @Override
            public void onFailure(Call<ApiList<ShareHistory>> call, Throwable t) {
                listener.OnGetShareSuccess(new ArrayList<>());
            }
        });
    }

    public ArrayList<GroupShareHistory> cloneListGroupShareHistory(ArrayList<ShareHistory> shareHistories) {
        ArrayList<GroupShareHistory> results = new ArrayList<>();
        ArrayList<ShareHistory> parents = (ArrayList<ShareHistory>) shareHistories.stream().filter(r -> r.getParentId() == 0).collect(Collectors.toList());
        ArrayList<ShareHistory> childs = (ArrayList<ShareHistory>) shareHistories.stream().filter(r -> r.getParentId() > 0).collect(Collectors.toList());

        for (ShareHistory share : parents) {
            results.add(new GroupShareHistory(share, new ArrayList<>()));
        }

        for (ShareHistory share : childs) {
            for (int i = 0; i < results.size(); i++) {
                if (results.get(i).getParentItem().getID() == share.getParentId()) {
                    results.get(i).getListChild().add(share);
                }
            }
        }
        return results;
    }


    public void share(WorkflowItem workflow, ArrayList<UserAndGroup> shares, String comment) {
        if (shares.isEmpty()) {
            listener.OnShareErr(Functions.share.getTitle("MESS_REQUIRE_USERGROUP", "Vui lòng chọn người để thực hiện."));
        } else {
            ArrayList<String> _lstAccountName = new ArrayList<>();
            for (int i = 0; i < shares.size(); i++) {
                if (shares.get(i).getType().equals("0")) {
                    _lstAccountName.add(shares.get(i).getAccountName());
                } else {
                    _lstAccountName.add(shares.get(i).getName());
                }
            }

            String _userValues = String.join(";", _lstAccountName);

            HashMap<String, RequestBody> hashMap = new HashMap<>();
            hashMap.put("userValues", DetailFunc.share.toRequestBody(_userValues));
            hashMap.put("func", DetailFunc.share.toRequestBody("Share"));
            hashMap.put("fid", DetailFunc.share.toRequestBody(workflow.getID()));
            hashMap.put("data", DetailFunc.share.toRequestBody(""));
            hashMap.put("lcid", DetailFunc.share.toRequestBody("1066"));
            if (!Functions.isNullOrEmpty(comment)) {
                hashMap.put("idea", DetailFunc.share.toRequestBody(comment));
            }

            MultipartBody.Part[] filesComment = new MultipartBody.Part[0];

            String err = Functions.share.getTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!");
            apiBPM.sendControlDynamicAction(filesComment, hashMap, err, new ApiBPM.ApiBPMListener() {
                @Override
                public void OnSuccess() {
                    listener.OnShareSuccess();
                }

                @Override
                public void OnErr(String err) {
                    listener.OnShareErr(err);
                }
            });
        }
    }
}

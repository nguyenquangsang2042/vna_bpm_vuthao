package com.vuthao.bpmop.task.presenter;

import android.app.Activity;
import android.content.Intent;
import android.graphics.Typeface;
import android.util.Log;
import android.view.View;
import android.widget.TextView;

import androidx.activity.result.ActivityResultLauncher;
import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.core.content.res.ResourcesCompat;

import com.google.gson.Gson;
import com.google.gson.JsonObject;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Crypter;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.api.ApiController;
import com.vuthao.bpmop.base.model.ApiList;
import com.vuthao.bpmop.base.model.ApiObject;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.Group;
import com.vuthao.bpmop.base.model.app.Status;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.model.app.WorkflowItem;
import com.vuthao.bpmop.base.model.custom.AttachFile;
import com.vuthao.bpmop.base.model.custom.Comment;
import com.vuthao.bpmop.base.model.custom.DetailTask;
import com.vuthao.bpmop.base.model.custom.FormDetailInfo;
import com.vuthao.bpmop.base.model.custom.LookupData;
import com.vuthao.bpmop.base.model.custom.ObjectFilter;
import com.vuthao.bpmop.base.model.custom.ObjectSubmitAction;
import com.vuthao.bpmop.base.model.custom.ObjectSubmitDetailComment;
import com.vuthao.bpmop.base.model.custom.Task;
import com.vuthao.bpmop.base.model.custom.UserAndGroup;
import com.vuthao.bpmop.base.realm.RealmController;
import com.vuthao.bpmop.detail.custom.DetailFunc;
import com.vuthao.bpmop.task.DetailCreateTaskActivity;
import com.vuthao.bpmop.task.FuncDetailCreateTask;

import org.json.JSONObject;

import java.io.File;
import java.net.ConnectException;
import java.net.UnknownHostException;
import java.util.ArrayList;
import java.util.HashMap;

import io.realm.Realm;
import okhttp3.MediaType;
import okhttp3.MultipartBody;
import okhttp3.RequestBody;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class DetailCreateTaskPresenter {
    private DetailCreateTaskListener listener;
    private DetailCreateCommentTaskListener commentTaskListener;
    private DeleleTaskListener deleleTaskListener;
    private Realm realm;

    public interface DetailCreateTaskListener {
        void OnGetTaskSuccess(DetailTask detailTask);
        void OnGetTaskErr(String err);
        void OnCreateTaskSuccess();
        void OnCreateTaskErr(String err);
    }

    public interface DeleleTaskListener {
        void OnDeleteSuccess();
        void OnDeleteErr(String err);
    }

    public interface DetailCreateCommentTaskListener {
        void OnGetCommentTaskSuccess(String otherResourceId, ArrayList<Comment> comments);
    }

    public DetailCreateTaskPresenter() {
        realm = new RealmController().getRealm();
    }

    public DetailCreateTaskPresenter(DetailCreateTaskListener listener, DetailCreateCommentTaskListener commentTaskListener
    , DeleleTaskListener deleleTaskListener) {
        this.listener = listener;
        this.commentTaskListener = commentTaskListener;
        this.deleleTaskListener = deleleTaskListener;
        realm = new RealmController().getRealm();
    }

    public DetailCreateTaskPresenter(DetailCreateTaskListener listener) {
        this.listener = listener;
        realm = new RealmController().getRealm();
    }

    public void setToolbarItem_Selected(Activity activity, TextView _tv, View _vw) {
        _tv.setTypeface(ResourcesCompat.getFont(activity, R.font.fontarial), Typeface.BOLD);
        _tv.setTextColor(ContextCompat.getColor(activity, R.color.clBlack));
        _vw.setVisibility(View.VISIBLE);
    }

    public void setToolbarItem_NotSelected(Activity activity, TextView _tv, View _vw) {
        _tv.setTypeface(ResourcesCompat.getFont(activity, R.font.fontarial), Typeface.NORMAL);
        _tv.setTextColor(ContextCompat.getColor(activity, R.color.clBottomDisable));
        _vw.setVisibility(View.INVISIBLE);
    }

    public void gotoTask(Activity activity,ActivityResultLauncher<Intent> launcher, Intent
            intent) {
        Intent i = new Intent(activity, DetailCreateTaskActivity.class);
        i.putExtra("WorkflowItemId", intent.getIntExtra("WorkflowItemId", 0));
        i.putExtra("isClickFromAction", false);
        i.putExtra("isChildTask", true);
        i.putExtra("taskId", intent.getIntExtra("taskId", 0));
        //launcher.launch(i);
        activity.startActivity(i);
    }

    //isCheckPermission - Kiểm tra quyền user
    public void checkWorkflowPermisson(WorkflowItem workflowItem, int taskId, boolean isCheckPermission) {
        if (isCheckPermission) {
            Call<ApiObject<FormDetailInfo>> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getTicketRequestControlDynamicForm(workflowItem.getID(), String.valueOf(CurrentUser.getInstance().getUser().getLanguage()));
            call.enqueue(new Callback<ApiObject<FormDetailInfo>>() {
                @Override
                public void onResponse(@NonNull Call<ApiObject<FormDetailInfo>> call, @NonNull Response<ApiObject<FormDetailInfo>> response) {
                    if (response.isSuccessful()) {
                        assert response.body() != null;
                        if (response.body().getStatus().equals("SUCCESS")) {
                            getDetailTaskForm(taskId);
                        } else {
                            listener.OnGetTaskErr(Functions.share.getTitle("MESS_TASK_NOTFOUND", "Không tìm thấy thông tin công việc, vui lòng thử lại sau!"));
                        }
                    } else {
                        listener.OnGetTaskErr(Functions.share.getTitle("MESS_TASK_NOTFOUND", "Không tìm thấy thông tin công việc, vui lòng thử lại sau!"));
                    }
                }

                @Override
                public void onFailure(@NonNull Call<ApiObject<FormDetailInfo>> call, @NonNull Throwable e) {
                    if (e instanceof UnknownHostException || e instanceof ConnectException) {
                        listener.OnGetTaskErr(Functions.share.getTitle("MESS_REQUIRE_NETWORK", "No network connection, please try again."));
                    } else {
                        listener.OnGetTaskErr(Functions.share.getTitle("MESS_TASK_NOTFOUND", "Không tìm thấy thông tin công việc, vui lòng thử lại sau!"));
                    }
                }
            });
        } else {
            getDetailTaskForm(taskId);
        }
    }

    private void getDetailTaskForm(int taskId) {
        Call<ApiObject<DetailTask>> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getDetailTaskForm(taskId);
        call.enqueue(new Callback<ApiObject<DetailTask>>() {
            @Override
            public void onResponse(@NonNull Call<ApiObject<DetailTask>> call, @NonNull Response<ApiObject<DetailTask>> response) {
                if (response.isSuccessful()) {
                    assert response.body() != null;
                    if (response.body().getStatus().equals("SUCCESS")) {
                        listener.OnGetTaskSuccess(response.body().getData());
                    } else {
                        listener.OnGetTaskErr(Functions.share.getTitle("MESS_TASK_NOTFOUND", "Không tìm thấy thông tin công việc, vui lòng thử lại sau!"));
                    }
                } else {
                    listener.OnGetTaskErr(Functions.share.getTitle("MESS_TASK_NOTFOUND", "Không tìm thấy thông tin công việc, vui lòng thử lại sau!"));
                }
            }

            @Override
            public void onFailure(@NonNull Call<ApiObject<DetailTask>> call, @NonNull Throwable t) {
                listener.OnGetTaskErr(Functions.share.getTitle("MESS_TASK_NOTFOUND", "Không tìm thấy thông tin công việc, vui lòng thử lại sau!"));
            }
        });
    }

    public void getDetailOtherResource(String otherResourceId, WorkflowItem workflowItem, String commentChange, String resourceCategoryId) {
        ObjectSubmitDetailComment objSubmitDetailComment = DetailFunc.share.initTrackingObjectSubmitDetail();
        objSubmitDetailComment.setID(otherResourceId);
        objSubmitDetailComment.setResourceCategoryId(resourceCategoryId);
        objSubmitDetailComment.setResourceUrl(String.format(DetailFunc.share.getURLSettingComment(Integer.parseInt(resourceCategoryId)), workflowItem.getID()));
        objSubmitDetailComment.setItemId(workflowItem.getID());
        objSubmitDetailComment.setAuthor(CurrentUser.getInstance().getUser().getID());
        objSubmitDetailComment.setAuthorName(CurrentUser.getInstance().getUser().getFullName());
        String data = new Gson().toJson(objSubmitDetailComment).replace("\\u003d", "=");

        Call<JsonObject> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getDetailOtherResource(data);
        call.enqueue(new Callback<JsonObject>() {
            @Override
            public void onResponse(@NonNull Call<JsonObject> call, @NonNull Response<JsonObject> response) {
                if (response.isSuccessful()) {
                    try {
                        JSONObject jsonObject = new JSONObject(new Gson().toJson(response.body()));
                        if (jsonObject.getString("status").equals("SUCCESS")) {
                            String Id = jsonObject.getJSONObject("data").getJSONObject("detail").getString("ID");
                            getListCommentsTask(Id, "", workflowItem);
                        }
                    } catch (Exception ex) {
                        Log.d("ERR getDetailOtherResource", ex.getMessage());
                        getListCommentsTask("", "", workflowItem);
                    }
                }
            }

            @Override
            public void onFailure(@NonNull Call<JsonObject> call, @NonNull Throwable ex) {
                Log.d("ERR getDetailOtherResource", ex.getMessage());
            }
        });
    }

    private void getListCommentsTask(String otherResourceId, String modified, WorkflowItem workflowItem) {
        ArrayList<ObjectFilter> lstFilter = new ArrayList<>();
        lstFilter.add(new ObjectFilter("FilterType", "eq", "COMMENT", "", ""));
        lstFilter.add(new ObjectFilter("ItemId", "eq", otherResourceId, "", ""));
        lstFilter.add(new ObjectFilter("Modified", "eq", modified, "", ""));

        String data =  new Crypter().encrypt(new Gson().toJson(lstFilter));
        Call<ApiList<Comment>> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getListComment(data);
        call.enqueue(new Callback<ApiList<Comment>>() {
            @Override
            public void onResponse(Call<ApiList<Comment>> call, Response<ApiList<Comment>> response) {
                if (response.isSuccessful()) {
                    assert response.body() != null;
                    if (response.body().getStatus().equals("SUCCESS")) {
                        commentTaskListener.OnGetCommentTaskSuccess(otherResourceId, response.body().getData());
                    }
                }
            }

            @Override
            public void onFailure(@NonNull Call<ApiList<Comment>> call, @NonNull Throwable t) {
                Log.d("ERR getListComments", t.getMessage());
            }
        });
    }

    public ArrayList<LookupData> getListLookupStatus() {
        ArrayList<LookupData> status = new ArrayList<>();
        status.add(new LookupData("1", FuncDetailCreateTask.getStatusNameByID(1), false));
        status.add(new LookupData("3", FuncDetailCreateTask.getStatusNameByID(3), false));
        status.add(new LookupData("2", FuncDetailCreateTask.getStatusNameByID(2), false));
        return status;
    }

    public void sendCreateTaskAction(Task task, ArrayList<UserAndGroup> lstAssignUser, ArrayList<ObjectSubmitAction> lstSubmitAction,
                                     ArrayList<AttachFile> files, int flag) {

        MultipartBody.Part[] attachs = new MultipartBody.Part[files.size()];

        for (int i = 0; i < files.size(); i++) {
            File f = new File(files.get(i).getPath());

            RequestBody requestBody = RequestBody.create(
                    MediaType.parse("image/*"),
                    f);

            // Add thêm request body vào trong builder
            attachs[i] = MultipartBody.Part.createFormData("file", f.getName(), requestBody);
        }

        String assignTo = getAccountName(lstAssignUser);

        HashMap<String, Object> hashMap = new HashMap();
        hashMap.put("itemTask", task);
        hashMap.put("Assign", assignTo);
        hashMap.put("Flag", flag);
        hashMap.put("JsonEdit", lstSubmitAction);
        RequestBody data = RequestBody.create(MediaType.parse("text/plain"), new Gson().toJson(hashMap));

        Call<Status> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).sendCreateTaskAction(attachs, data);
        call.enqueue(new Callback<Status>() {
            @Override
            public void onResponse(@NonNull Call<Status> call, @NonNull Response<Status> response) {
                if (response.isSuccessful()) {
                    assert response.body() != null;
                    if (response.body().getStatus().equals("SUCCESS")) {
                        listener.OnCreateTaskSuccess();
                    } else {
                        listener.OnCreateTaskErr(Functions.share.getTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"));
                    }
                } else {
                    listener.OnCreateTaskErr(Functions.share.getTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"));
                }
            }

            @Override
            public void onFailure(@NonNull Call<Status> call, @NonNull Throwable t) {
                if (t instanceof UnknownHostException || t instanceof ConnectException) {
                    listener.OnCreateTaskErr(Functions.share.getTitle("MESS_REQUIRE_NETWORK", "No network connection, please try again."));
                } else {
                    listener.OnCreateTaskErr(Functions.share.getTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"));
                }
            }
        });
    }

    private String getAccountName(ArrayList<UserAndGroup> users) {
        String accountName = "";
        ArrayList<String> lstAccountName = new ArrayList<>();
        for (UserAndGroup user : users) {
            if (user.getType().equals("0")) {
                User u = new RealmController().getRealm().where(User.class)
                        .equalTo("ID", user.getID())
                        .findFirst();
                if (u != null) {
                    lstAccountName.add(u.getAccountName());
                }
            } else {
                Group u = new RealmController().getRealm().where(Group.class)
                        .equalTo("ID", user.getID())
                        .findFirst();
                if (u != null) {
                    lstAccountName.add(u.getTitle());
                }
            }
        }

        accountName = String.join(";#", lstAccountName);
        return accountName;
    }

    public void deleteTask(int taskId) {
        Call<Status> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).deleteDetailTaskForm(String.valueOf(taskId));
        call.enqueue(new Callback<Status>() {
            @Override
            public void onResponse(@NonNull Call<Status> call, @NonNull Response<Status> response) {
                if (response.isSuccessful()) {
                    assert response.body() != null;
                    if (response.body().getStatus().equals("SUCCESS")) {
                        deleleTaskListener.OnDeleteSuccess();
                    } else {
                        deleleTaskListener.OnDeleteErr(Functions.share.getTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!"));
                    }
                } else {
                    deleleTaskListener.OnDeleteErr(Functions.share.getTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!"));
                }
            }

            @Override
            public void onFailure(@NonNull Call<Status> call, @NonNull Throwable e) {
                if (e instanceof UnknownHostException || e instanceof ConnectException) {
                    deleleTaskListener.OnDeleteErr(Functions.share.getTitle("MESS_REQUIRE_NETWORK", "No network connection, please try again."));
                } else {
                    deleleTaskListener.OnDeleteErr(Functions.share.getTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!"));
                }
            }
        });
    }

    public ArrayList<UserAndGroup> getAssigned(ArrayList<UserAndGroup> userAndGroups) {
        ArrayList<UserAndGroup> items = new ArrayList<>();
        UserAndGroup uag = new UserAndGroup();
        for (UserAndGroup item : userAndGroups) {
            // User
            if (item.getType().equals("0")) {
                User user = realm.where(User.class)
                        .equalTo("ID", item.getID())
                        .findFirst();
                if (user != null) {
                    uag.setID(user.getID());
                    uag.setType("0");
                    uag.setName(user.getFullName());
                    uag.setEmail(user.getEmail());
                    uag.setAccountName(user.getAccountName());
                    uag.setImagePath(user.getImagePath());
                    items.add(uag);
                }
            } else {
                Group group = realm.where(Group.class)
                        .equalTo("ID", item.getID())
                        .findFirst();
                if (group != null) {
                    uag.setID(group.getID());
                    uag.setType("1");
                    uag.setName(group.getTitle());
                    uag.setEmail(group.getDescription());
                    uag.setAccountName(group.getTitle());
                    uag.setImagePath(group.getImage());
                    items.add(uag);
                }
            }
        }
        return items;
    }
}

package com.vuthao.bpmop.detail.presenter;

import android.app.Activity;
import android.content.Intent;
import android.util.Log;
import android.view.View;

import androidx.activity.result.ActivityResultLauncher;
import androidx.annotation.NonNull;

import com.google.gson.Gson;
import com.google.gson.JsonObject;
import com.google.gson.reflect.TypeToken;
import com.vuthao.bpmop.base.Crypter;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.api.ApiController;
import com.vuthao.bpmop.base.api.ApiBPM;
import com.vuthao.bpmop.base.api.ApiWorkflowController;
import com.vuthao.bpmop.base.model.ApiList;
import com.vuthao.bpmop.base.model.ApiObject;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.Notify;
import com.vuthao.bpmop.base.model.app.Status;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.model.app.WorkflowFollow;
import com.vuthao.bpmop.base.model.app.WorkflowItem;
import com.vuthao.bpmop.base.model.custom.AttachFile;
import com.vuthao.bpmop.base.model.custom.ButtonAction;
import com.vuthao.bpmop.base.model.custom.Comment;
import com.vuthao.bpmop.base.model.custom.FormDetailInfo;
import com.vuthao.bpmop.base.model.custom.GridDetails;
import com.vuthao.bpmop.base.model.custom.ObjectFilter;
import com.vuthao.bpmop.base.model.custom.ObjectSubmitAction;
import com.vuthao.bpmop.base.model.custom.ObjectSubmitDetailComment;
import com.vuthao.bpmop.base.model.custom.OtherResource;
import com.vuthao.bpmop.base.model.custom.WorkFlowRelated;
import com.vuthao.bpmop.base.model.custom.WorkflowHistory;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.model.dynamic.ViewRow;
import com.vuthao.bpmop.base.model.dynamic.ViewSection;
import com.vuthao.bpmop.base.notification.NotificationsListenerService;
import com.vuthao.bpmop.base.realm.RealmController;
import com.vuthao.bpmop.base.realm.RealmHelper;
import com.vuthao.bpmop.base.vars.Variable;
import com.vuthao.bpmop.base.vars.VarsControl;
import com.vuthao.bpmop.core.Vars;
import com.vuthao.bpmop.core.adapter.TemplateValueTypeAdapter;
import com.vuthao.bpmop.core.component.ComponentButtonBot;
import com.vuthao.bpmop.detail.DetailWorkflowActivity;
import com.vuthao.bpmop.detail.custom.DetailFunc;
import com.vuthao.bpmop.shareview.SharedView_PopupActionAccept;
import com.vuthao.bpmop.shareview.SharedView_PopupActionForward;
import com.vuthao.bpmop.shareview.SharedView_PopupActionMore;
import com.vuthao.bpmop.shareview.SharedView_PopupActionReject;
import com.vuthao.bpmop.shareview.SharedView_PopupActionRequestInfo;
import com.vuthao.bpmop.shareview.SharedView_PopupControlDate;
import com.vuthao.bpmop.shareview.SharedView_PopupControlDateTime;
import com.vuthao.bpmop.shareview.SharedView_PopupControlMultiChoice;
import com.vuthao.bpmop.shareview.SharedView_PopupControlMultiLookup;
import com.vuthao.bpmop.shareview.SharedView_PopupControlNumber;
import com.vuthao.bpmop.shareview.SharedView_PopupControlSelectUserGroup;
import com.vuthao.bpmop.shareview.SharedView_PopupControlSelectUserGroupMulti;
import com.vuthao.bpmop.shareview.SharedView_PopupControlSingleChoice;
import com.vuthao.bpmop.shareview.SharedView_PopupControlSingleLookup;
import com.vuthao.bpmop.shareview.SharedView_PopupControlTextInput;
import com.vuthao.bpmop.shareview.SharedView_PopupControlTextInputFormat;
import com.vuthao.bpmop.shareview.SharedView_PopupControlViewFullInfo;
import com.vuthao.bpmop.task.DetailCreateTaskActivity;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.File;
import java.net.ConnectException;
import java.net.UnknownHostException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.Map;
import java.util.stream.Collectors;

import io.realm.Realm;
import io.realm.RealmResults;
import okhttp3.MediaType;
import okhttp3.MultipartBody;
import okhttp3.RequestBody;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class DetailWorkflowPresenter {
    private ApiWorkflowController.DetailWorkflowListener listener;
    private ApiWorkflowController.WorkflowHistoryListener historyListener;
    private FollowListener followListener;
    private ApiWorkflowController controller;
    private DetailWorkflowBottomActionListener actionListener;
    private CommentListener commentListener;
    private WorkflowControlDynamicListener workflowControlDynamicListener;
    private DetailGridsListener gridsListener;
    private WorkflowItemListener workflowItemListener;
    private ApiBPM apiBPM;
    private Realm realm;
    private int totalDetails = 0;
    private GridDetails gridDetails;
    private FormDetailInfo formDetailInfo;
    private PushNotificationListener pushNotificationListener;


    public DetailWorkflowPresenter() {
    }

    public interface WorkflowControlDynamicListener {
        void OnUpdateValueForElement(ViewElement element, boolean isNotifyDataSetChange);

        void OnSendAPI(ButtonAction action, String comment, HashMap<String, String> extension);
    }

    public interface PushNotificationListener {
        void OnGetItemSuccess(AppBase appBase);
        void OnGetItemError(String err);
    }

    public interface FollowListener {
        void OnFollowSuccess();

        void OnFollowErr(String err);
    }

    public interface CommentListener {
        void OnOtherResourceSuccess(String _OtherResourceId);

        void OnCommentSuccess();

        void OnLikeCommentSuccess();

        void OnLikeCommentErr(String err);

        void OnCommentErr(String err);
    }

    public interface DetailWorkflowBottomActionListener {
        void OnSubmitActionSuccess();

        void OnSubmitActionErr(String err);
    }

    public DetailWorkflowPresenter(CommentListener commentListener) {
        this.commentListener = commentListener;
        realm = new RealmController().getRealm();
    }

    public interface DetailGridsListener {
        void OnGetGridsSuccess(FormDetailInfo formDetailInfo, GridDetails gridDetails);
    }

    public interface WorkflowItemListener {
        void OnGetWorkflowItemSuccess(WorkflowItem workflowItem);

        void OnGetWorkflowItemErr(String err);
    }

    public DetailWorkflowPresenter(ApiWorkflowController.WorkflowHistoryListener historyListener) {
        this.historyListener = historyListener;
        controller = new ApiWorkflowController();
    }

    public DetailWorkflowPresenter(PushNotificationListener pushNotificationListener) {
        this.pushNotificationListener = pushNotificationListener;
    }

    public DetailWorkflowPresenter(ApiWorkflowController.DetailWorkflowListener listener, FollowListener followListener
            , CommentListener commentListener, DetailWorkflowBottomActionListener actionListener
            , WorkflowControlDynamicListener workflowControlDynamicListener
            , ApiWorkflowController.WorkflowHistoryListener historyListener
            , DetailGridsListener gridsListener, WorkflowItemListener workflowItemListener) {
        this.listener = listener;
        this.followListener = followListener;
        this.commentListener = commentListener;
        this.actionListener = actionListener;
        this.workflowControlDynamicListener = workflowControlDynamicListener;
        this.historyListener = historyListener;
        this.gridsListener = gridsListener;
        this.workflowItemListener = workflowItemListener;
        apiBPM = new ApiBPM();
        realm = new RealmController().getRealm();
        controller = new ApiWorkflowController();
    }

    public void getWorkflowItemById(String rid) {
        Call<ApiList<WorkflowItem>> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getWorkflowItemById(rid);
        call.enqueue(new Callback<ApiList<WorkflowItem>>() {
            @Override
            public void onResponse(Call<ApiList<WorkflowItem>> call, Response<ApiList<WorkflowItem>> response) {
                if (response.isSuccessful()) {
                    if (response.body().getStatus().equals("SUCCESS")) {
                        workflowItemListener.OnGetWorkflowItemSuccess(response.body().getData().get(0));
                    } else {
                        workflowItemListener.OnGetWorkflowItemErr(Functions.share.getTitle("MESS_WORKFLOW_NOTFOUND", "Không tìm thấy thông tin phiếu, vui lòng thử lại sau!"));
                    }
                } else {
                    workflowItemListener.OnGetWorkflowItemErr(Functions.share.getTitle("MESS_WORKFLOW_NOTFOUND", "Không tìm thấy thông tin phiếu, vui lòng thử lại sau!"));
                }
            }

            @Override
            public void onFailure(Call<ApiList<WorkflowItem>> call, Throwable t) {
                workflowItemListener.OnGetWorkflowItemErr(Functions.share.getTitle("MESS_WORKFLOW_NOTFOUND", "Không tìm thấy thông tin phiếu, vui lòng thử lại sau!"));
            }
        });
    }

    public void getTicketRequestControlDynamicForm(WorkflowItem workflowItem) {
        controller.getTicketRequestControlDynamicForm(workflowItem.getID(), new ApiWorkflowController.DetailWorkflowListener() {
            @Override
            public void OnGetWorkflowDynamicSucess(FormDetailInfo forms) {
                if (forms != null) {
                    getDetailOtherResource(forms.getMoreInfo().getOtherResourceId(), workflowItem, forms.getMoreInfo().getCommentChanged(), "8");
                    formDetailInfo = forms;
                    totalDetails++;
                    if (totalDetails == 2) {
                        totalDetails = 0;
                        gridsListener.OnGetGridsSuccess(formDetailInfo, gridDetails);
                    }
                } else {
                    listener.OnGetWorkflowErr(Functions.share.getTitle("MESS_WORKFLOW_NOTFOUND", "Không tìm thấy thông tin phiếu, vui lòng thử lại sau!"));
                }
            }

            @Override
            public void OnGetWorkflowErr(String err) {
                listener.OnGetWorkflowErr(err);
            }
        });
    }

    public void getGridsDetails(String workflowId) {
        Call<ApiObject<GridDetails>> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getGridsDetails(workflowId);
        call.enqueue(new Callback<ApiObject<GridDetails>>() {
            @Override
            public void onResponse(Call<ApiObject<GridDetails>> call, Response<ApiObject<GridDetails>> response) {
                assert response.body() != null;
                if (response.body().getData() != null) {
                    gridDetails = response.body().getData();
                    totalDetails++;
                    if (totalDetails == 2) {
                        totalDetails = 0;
                        gridsListener.OnGetGridsSuccess(formDetailInfo, gridDetails);
                    }
                } else {
                    listener.OnGetWorkflowErr(Functions.share.getTitle("MESS_WORKFLOW_NOTFOUND", "Không tìm thấy thông tin phiếu, vui lòng thử lại sau!"));
                }
            }

            @Override
            public void onFailure(Call<ApiObject<GridDetails>> call, Throwable t) {
                listener.OnGetWorkflowErr(Functions.share.getTitle("MESS_WORKFLOW_NOTFOUND", "Không tìm thấy thông tin phiếu, vui lòng thử lại sau!"));
            }
        });
    }

    public void getListProcessHistory(String workflowId) {
        controller.getListProcessHistory(workflowId, histories -> historyListener.OnGetWorkflowHistorySuccess(histories));
    }

    public ArrayList<AttachFile> getAttachFiles(String attach) {
        return new Gson().fromJson(attach, new TypeToken<ArrayList<AttachFile>>() {
        }.getType());
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

            }
        });
    }

    public ViewElement getElementAttachFromGrids(GridDetails gridDetails) {
        if (gridDetails.getAttachment() != null) {
            for (ViewRow r : gridDetails.getAttachment()) {
                for (ViewElement e : r.getElements()) {
                    return e;
                }
            }
        }

        return null;
    }

    public void updateOrInsertFollow(WorkflowItem workflowItem, boolean isFollow) {
        realm.executeTransaction(realm -> {
            WorkflowFollow follow = new WorkflowFollow();
            follow.setWorkflowItemId(Integer.parseInt(workflowItem.getID()));
            follow.setUserId(CurrentUser.getInstance().getUser().getUserId());
            follow.setStatus(isFollow ? 1 : 0);
            realm.copyToRealmOrUpdate(follow);
        });
    }

    public void updateFollow(WorkflowItem workflowItem) {
        try {
            String status = workflowItem.isFollow() ? "0" : "1";
            String langCode = String.valueOf(CurrentUser.getInstance().getUser().getLanguage());

            HashMap<String, String> hashMap = new HashMap<>();
            hashMap.put("func", "Follow");
            hashMap.put("fid", workflowItem.getID());
            hashMap.put("data", "");
            hashMap.put("lcid", langCode);
            hashMap.put("status", status);

            controller.updateFollow(hashMap, new ApiWorkflowController.ApiWorkflowListener() {
                @Override
                public void OnGetDataSuccess() {
                    apiBPM.updateWorkflowFollow(new ApiBPM.ApiBPMListener() {
                        @Override
                        public void OnSuccess() {
                        }

                        @Override
                        public void OnErr(String err) {
                            followListener.OnFollowErr(err);
                        }
                    });
                }

                @Override
                public void OnGetDataError(String err) {
                    followListener.OnFollowErr(err);
                }
            });
        } catch (Exception ex) {
            Log.d("ERR follow", ex.getMessage());
        }
    }

    public void getDetailOtherResource(String otherResourceId, WorkflowItem workflowItem, String commentChange, String resourceCategoryId) {
        ObjectSubmitDetailComment objSubmit = DetailFunc.share.initTrackingObjectSubmitDetail();
        objSubmit.setID(otherResourceId);
        objSubmit.setResourceCategoryId(resourceCategoryId);
        objSubmit.setResourceUrl(String.format(DetailFunc.share.getURLSettingComment(Integer.parseInt(resourceCategoryId)), workflowItem.getID()));
        objSubmit.setItemId(workflowItem.getID());
        objSubmit.setAuthor(CurrentUser.getInstance().getUser().getID());
        objSubmit.setAuthorName(CurrentUser.getInstance().getUser().getFullName());
        String data = new Gson().toJson(objSubmit).replace("\\u003d", "=");

        Call<JsonObject> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getDetailOtherResource(data);
        call.enqueue(new Callback<JsonObject>() {
            @Override
            public void onResponse(@NonNull Call<JsonObject> call, @NonNull Response<JsonObject> response) {
                if (response.isSuccessful()) {
                    String Id = "";
                    try {
                        JSONObject jsonObject = new JSONObject(new Gson().toJson(response.body()));
                        if (jsonObject.getString("status").equals("SUCCESS")) {
                            Id = jsonObject.getJSONObject("data").getJSONObject("detail").getString("ID");
                        }
                    } catch (Exception ex) {
                        Log.d("ERR getDetailOtherResource", ex.getMessage());
                    }

                    //auto lay comment online - khong co mang lam sao ma coi dc danh phieu ma lay offline ?
                    getListComments(Id, 8, "", workflowItem);

//                    if (Functions.isNullOrEmpty(workflowItem.getCommentChanged())) {
//                        getListComments(Id, 8, "", workflowItem);
//                    } else {
//                        long commentWofklow = Functions.share.formatStringToLongApi(workflowItem.getCommentChanged());
//                        long change = Functions.share.formatStringToLongApi(commentChange);
//
//                        if (commentWofklow < change) {
//                            new RealmController().getRealm().executeTransaction(realm -> {
//                                workflowItem.setChange(true);
//                                realm.copyToRealmOrUpdate(workflowItem);
//                            });
//                        }
//
//                        if (workflowItem.isChange()) {
//                            RealmResults<Comment> comments = new RealmController().getRealm().where(Comment.class).findAll();
//                            if (comments.size() > 0) {
//                                getListComments(Id, 8, DetailFunc.share.formatDateComment(workflowItem.getCommentChanged()), workflowItem);
//                            } else {
//                                getListComments(Id, 8, "", workflowItem);
//                            }
//                        } else {
//                            commentListener.OnOtherResourceSuccess(Id);
//                        }
//                    }
                }
            }

            @Override
            public void onFailure(@NonNull Call<JsonObject> call, @NonNull Throwable ex) {
                Log.d("ERR getDetailOtherResource", ex.getMessage());
            }
        });
    }

    public void getListComments(String OtherResourceID, int resourceCategoryID, String modified, WorkflowItem workflowItem) {
        ArrayList<ObjectFilter> lstFilter = new ArrayList<>();
        lstFilter.add(new ObjectFilter("FilterType", "eq", "COMMENT", "", ""));
        lstFilter.add(new ObjectFilter("ItemId", "eq", OtherResourceID, "", ""));
        lstFilter.add(new ObjectFilter("Modified", "eq", modified, "", ""));

        String data = new Crypter().encrypt(new Gson().toJson(lstFilter));
        Call<ApiList<Comment>> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getListComment(data);
        call.enqueue(new Callback<ApiList<Comment>>() {
            @Override
            public void onResponse(@NonNull Call<ApiList<Comment>> call, @NonNull Response<ApiList<Comment>> response) {
                if (response.isSuccessful()) {
                    assert response.body() != null;
                    if (response.body().getStatus().equals("SUCCESS")) {
                        if (response.body().getData() != null && response.body().getData().size() > 0) {
                            if (resourceCategoryID == 8) {
                                new RealmHelper<Comment>().addNewItems(response.body().getData());
                            }

                            String dateNow = response.body().getDateNow();
                            new RealmController().getRealm().executeTransaction(realm -> {
                                workflowItem.setCommentChanged(dateNow);
                                workflowItem.setChange(false);
                                realm.copyToRealmOrUpdate(workflowItem);
                            });

                        }
                    }
                    commentListener.OnOtherResourceSuccess(OtherResourceID);
                }
            }

            @Override
            public void onFailure(Call<ApiList<Comment>> call, Throwable t) {
                Log.d("ERR getListComments", t.getMessage());
            }
        });
    }

    public ArrayList<Comment> getComments(String resourceId) {
        RealmResults<Comment> realmResults = new RealmController().getRealm().where(Comment.class)
                .equalTo("ResourceId", resourceId)
                .findAll();
        return new ArrayList<>(realmResults);
    }

    public void sendComment(String comment, ArrayList<AttachFile> files, String OtherResourceId, String ResourceCategoryId, String parentCommentId) {
        OtherResource otherResource = new OtherResource();
        otherResource.setContent(comment.trim().replace("  ", " "));
        otherResource.setResourceId(OtherResourceId);
        otherResource.setResourceCategoryId(Integer.parseInt(ResourceCategoryId));
        otherResource.setResourceSubCategoryId(0);
        otherResource.setImage("");
        otherResource.setParentCommentId(parentCommentId);

        RequestBody data = RequestBody.create(MediaType.parse("text/plain"), new Gson().toJson(otherResource));

        MultipartBody.Part[] filesComment = new MultipartBody.Part[files.size()];

        for (int i = 0; i < files.size(); i++) {
            File f = new File(files.get(i).getPath());

            RequestBody requestBody = RequestBody.create(
                    MediaType.parse("image/*"),
                    f);

            filesComment[i] = MultipartBody.Part.createFormData(files.get(i).getTitle(), f.getName(), requestBody);
        }

        Call<Status> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).addComment(filesComment, data);
        call.enqueue(new Callback<Status>() {
            @Override
            public void onResponse(@NonNull Call<Status> call, @NonNull Response<Status> response) {
                if (response.isSuccessful()) {
                    assert response.body() != null;
                    if (response.body().getStatus().equals("SUCCESS")) {
                        commentListener.OnCommentSuccess();
                    } else {
                        commentListener.OnCommentErr(Functions.share.getTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!"));
                    }
                } else {
                    commentListener.OnCommentErr(Functions.share.getTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!"));
                }
            }

            @Override
            public void onFailure(@NonNull Call<Status> call, @NonNull Throwable t) {
                commentListener.OnCommentErr(Functions.share.getTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!"));
            }
        });
    }

    public void addLikeComment(Comment comment, String resourceId, boolean flag, int resourceCategoryId) {
        realm.executeTransaction(realm -> {
            comment.setLiked(comment.isLiked() == 1 ? 0 : 1);
            if (comment.isLiked() == 1) {
                comment.setLikeCount(comment.getLikeCount() + 1);
            } else {
                comment.setLikeCount(Math.max(comment.getLikeCount() - 1, 0));
            }

            realm.copyToRealmOrUpdate(comment);
        });

        commentListener.OnLikeCommentSuccess();

        HashMap<String, String> hashMap = new HashMap<>();
        hashMap.put("ResourceId", resourceId);
        hashMap.put("ResourceCategoryId", String.valueOf(resourceCategoryId));

        String func = flag ? "like" : "unlike";
        String data = new Gson().toJson(hashMap);

        Call<Status> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).addLikeComment(func, data);
        call.enqueue(new Callback<Status>() {
            @Override
            public void onResponse(@NonNull Call<Status> call, @NonNull Response<Status> response) {

            }

            @Override
            public void onFailure(@NonNull Call<Status> call, @NonNull Throwable t) {
            }
        });
    }

    public ArrayList<User> getListUserFromQTLC(WorkflowItem workflowItem, ArrayList<WorkflowHistory> lstQTLC) {
        if (workflowItem.getStep() > 0) {
            lstQTLC = (ArrayList<WorkflowHistory>) lstQTLC.stream().filter(r -> r.getStep() < workflowItem.getStep()).collect(Collectors.toList());
        }

        ArrayList<User> result = new ArrayList<>();

        if (lstQTLC != null && lstQTLC.size() > 0) {
            ArrayList<String> lstEmail = new ArrayList<>();
            for (WorkflowHistory history : lstQTLC) {
                if (lstEmail.stream().noneMatch(r -> r.equals(history.getAssignUserId()) && !history.getAssignUserId().equals(CurrentUser.getInstance().getUser().getID()))) {
                    lstEmail.add(history.getAssignUserId());
                }
            }
            String[] _strSearchUser = new String[lstEmail.size()];
            for (int i = 0; i < lstEmail.size(); i++) {
                _strSearchUser[i] = lstEmail.get(i);
            }

            if (_strSearchUser.length > 0) {
                RealmResults<User> results = new RealmController().getRealm().where(User.class)
                        .in("ID", _strSearchUser)
                        .findAll();
                result.addAll(results);
            }
        }

        return result;
    }

    public void submitAction(ArrayList<AttachFile> files, HashMap<String, String> hashMap) {
        HashMap<String, RequestBody> map = new HashMap<>();

        for (Map.Entry<String, String> entry : hashMap.entrySet()) {
            map.put(entry.getKey(), DetailFunc.share.toRequestBody(entry.getValue()));
        }

        MultipartBody.Part[] filesComment = new MultipartBody.Part[files.size()];
        if (!files.isEmpty()) {
            filesComment = new MultipartBody.Part[files.size()];
            for (int i = 0; i < files.size(); i++) {
                File f = new File(files.get(i).getPath());

                RequestBody requestBody = RequestBody.create(
                        MediaType.parse("image/*"), f);

                // Add thêm request body vào trong builder
                filesComment[i] = MultipartBody.Part.createFormData(files.get(i).getTitle(),
                        f.getName(), requestBody);
            }
        }

        apiBPM.sendControlDynamicAction(filesComment, map, Functions.share.getTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!"), new ApiBPM.ApiBPMListener() {
            @Override
            public void OnSuccess() {
                actionListener.OnSubmitActionSuccess();
            }

            @Override
            public void OnErr(String err) {
                actionListener.OnSubmitActionErr(err);
            }
        });
    }

    public ArrayList<AttachFile> getAttachmentsEdits(ArrayList<ViewElement> elementEdits) {
        ArrayList<AttachFile> files = new ArrayList<>();
        for (int i = 0; i < elementEdits.size(); i++) {
            if (elementEdits.get(i).getDataType().equals(VarsControl.INPUTATTACHMENTHORIZON)) {
                ArrayList<AttachFile> lstAttachmentLocal = new Gson().fromJson(elementEdits.get(i).getValue(), new TypeToken<ArrayList<AttachFile>>() {
                }.getType());

                files = (ArrayList<AttachFile>) lstAttachmentLocal.stream().filter(r -> r.getID().equals("")).collect(Collectors.toList());
            }
        }

        return files;
    }

    public ArrayList<ObjectSubmitAction> getObjectSubmitActions
            (ArrayList<ViewElement> elementEdits,
             ArrayList<AttachFile> attachFileDeletes,
             ArrayList<JSONObject> gridEdits) {
        ArrayList<ObjectSubmitAction> items = new ArrayList<>();
        if (elementEdits.size() > 0) {
            for (int i = 0; i < elementEdits.size(); i++) {
                if (elementEdits.get(i).getDataType().equals("inputattachmenthorizon")) {
                    ArrayList<AttachFile> lstAttachmentLocal = new Gson().fromJson(elementEdits.get(i).getValue(), new TypeToken<ArrayList<AttachFile>>() {
                    }.getType());

                    lstAttachmentLocal = (ArrayList<AttachFile>) lstAttachmentLocal.stream().filter(r -> r.getID().equals("")).collect(Collectors.toList());

                    ObjectSubmitAction beanSubmitCurrent = new ObjectSubmitAction();
                    beanSubmitCurrent.setID(elementEdits.get(i).getID());
                    beanSubmitCurrent.setValue(elementEdits.get(i).getValue());
                    beanSubmitCurrent.setTypeSP("Attachment");
                    beanSubmitCurrent.setDataType(elementEdits.get(i).getDataType());
                    items.add(beanSubmitCurrent);

                    // Nếu có xóa mới có list này
                    if (attachFileDeletes.size() > 0) {
                        attachFileDeletes = (ArrayList<AttachFile>) attachFileDeletes.stream().filter(r -> !r.getID().equals("")).collect(Collectors.toList());
                        ObjectSubmitAction beanSubmitDeleted = new ObjectSubmitAction();
                        beanSubmitDeleted.setID(elementEdits.get(i).getID());
                        beanSubmitDeleted.setValue(new Gson().toJson(attachFileDeletes));
                        beanSubmitDeleted.setTypeSP("RemoveAttachment");
                        beanSubmitDeleted.setDataType(elementEdits.get(i).getDataType());

                        items.add(beanSubmitDeleted);
                    }

                } else if (elementEdits.get(i).getDataType().equals("inputgriddetails")) {
                    // Nếu inputgriddetails -> tạo 2 Object: Object current List và Object Deleted
                    // Object của những Attach File còn lại trong List.
                    ObjectSubmitAction beanSubmitCurrent = new ObjectSubmitAction();
                    beanSubmitCurrent.setID(elementEdits.get(i).getID());
                    beanSubmitCurrent.setValue(elementEdits.get(i).getValue());
                    beanSubmitCurrent.setTypeSP("GridDetails");
                    beanSubmitCurrent.setDataType(elementEdits.get(i).getDataType());
                    items.add(beanSubmitCurrent);

                    if (gridEdits != null && gridEdits.size() > 0) {
                        // Add ID = 0 trong AdapterRecyTemplateValueType()
                        gridEdits = (ArrayList<JSONObject>) gridEdits.stream().filter(r -> {
                            try {
                                return Integer.parseInt(r.getString("ID")) != 0;
                            } catch (JSONException e) {
                                e.printStackTrace();
                            }
                            return false;
                        }).collect(Collectors.toList());

                        ObjectSubmitAction beanSubmitDeleted = new ObjectSubmitAction();
                        beanSubmitDeleted.setID(elementEdits.get(i).getID());
                        beanSubmitDeleted.setValue(new Gson().toJson(gridEdits));
                        beanSubmitDeleted.setTypeSP("RemoveGridDetails");
                        beanSubmitDeleted.setDataType(elementEdits.get(i).getDataType());
                        items.add(beanSubmitDeleted);
                    }
                } else {
                    // Cac control khác -> tạo bình thường
                    ObjectSubmitAction beanSubmitActionData = new ObjectSubmitAction();
                    beanSubmitActionData.setID(elementEdits.get(i).getID());
                    beanSubmitActionData.setValue(elementEdits.get(i).getValue());
                    beanSubmitActionData.setTypeSP(elementEdits.get(i).getTypeSP());
                    beanSubmitActionData.setDataType(elementEdits.get(i).getDataType());
                    items.add(beanSubmitActionData);
                }
            }
        }

        return items;
    }

    public void handleFormClicks(Activity activity, View lnAll, String strElement) {
        ViewElement element = new Gson().fromJson(strElement, ViewElement.class);

        switch (element.getDataType()) {
            case VarsControl.SELECTUSER: {
                if (!element.isEnable() && Functions.isNullOrEmpty(element.getValue())) {
                    return;
                }

                SharedView_PopupControlSelectUserGroup popup = new SharedView_PopupControlSelectUserGroup(activity.getLayoutInflater(), activity, "", null, false);
                popup.initializeValue_Master(element);
                popup.initializeView();
                break;
            }
            case VarsControl.SELECTUSERGROUP: {
                if (!element.isEnable() && Functions.isNullOrEmpty(element.getValue())) {
                    return;
                }

                SharedView_PopupControlSelectUserGroup popup = new SharedView_PopupControlSelectUserGroup(activity.getLayoutInflater(), activity, "", null, true);
                popup.initializeValue_Master(element);
                popup.initializeView();
                break;
            }
            case VarsControl.SELECTUSERMULTI: {
                if (!element.isEnable() && Functions.isNullOrEmpty(element.getValue())) {
                    return;
                }

                // Có trường hợp Enable + Disable
                SharedView_PopupControlSelectUserGroupMulti popup = new SharedView_PopupControlSelectUserGroupMulti(activity.getLayoutInflater(), activity, "DetailWorkflow", lnAll, false);
                popup.initializeValue_Master(element);
                popup.initializeView();
                break;
            }
            case VarsControl.SELECTUSERGROUPMULTI: {
                if (!element.isEnable() && Functions.isNullOrEmpty(element.getValue())) {
                    return;
                }
                // Có trường hợp Enable + Disable
                SharedView_PopupControlSelectUserGroupMulti popup = new SharedView_PopupControlSelectUserGroupMulti(activity.getLayoutInflater(), activity, "DetailWorkflow", lnAll, true);
                popup.initializeValue_Master(element);
                popup.initializeView();
                break;
            }
            case VarsControl.DATE: {
                if (!element.isEnable() && Functions.isNullOrEmpty(element.getValue())) {
                    return;
                }

                SharedView_PopupControlDate popup = new SharedView_PopupControlDate(activity.getLayoutInflater(), activity, "DetailWorkflow", lnAll);
                popup.initializeValue_Master(element);
                popup.initializeView();
                break;
            }
            case VarsControl.DATETIME: {
                if (!element.isEnable() && Functions.isNullOrEmpty(element.getValue())) {
                    return;
                }

                SharedView_PopupControlDateTime popup = new SharedView_PopupControlDateTime(activity.getLayoutInflater(), activity, "DetailWorkflow", lnAll);
                popup.initializeValue_Master(element);
                popup.initializeView();
                break;
            }
            case VarsControl.TIME: {
                break;
            }
            case VarsControl.SINGLECHOICE: {
                // Có trường hợp Enable + Disable
                SharedView_PopupControlSingleChoice popup = new SharedView_PopupControlSingleChoice(activity.getLayoutInflater(), activity, "DetailWorkflow", lnAll);
                popup.initializeValue_Master(element);
                popup.initializeView();
                break;
            }
            case VarsControl.SINGLELOOKUP: {
                if (element.isEnable()) {
                    SharedView_PopupControlSingleLookup popup = new SharedView_PopupControlSingleLookup(activity.getLayoutInflater(), activity, "DetailWorkflow", lnAll);
                    popup.initializeValue_Master(element);
                    popup.initializeView();
                }

                break;
            }
            case VarsControl.MULTIPLECHOICE: {
                if (!element.isEnable() && Functions.isNullOrEmpty(element.getValue())) {
                    return;
                }

                SharedView_PopupControlMultiChoice popup = new SharedView_PopupControlMultiChoice(activity.getLayoutInflater(), activity, "DetailWorkflow", lnAll);
                popup.initializeValue_Master(element);
                popup.initializeView();
                break;
            }
            case VarsControl.MULTIPLELOOKUP: {
                if (element.isEnable()) {
                    SharedView_PopupControlMultiLookup popup = new SharedView_PopupControlMultiLookup(activity.getLayoutInflater(), activity, "DetailWorkflow", lnAll);
                    popup.initializeValue_Master(element);
                    popup.initializeView();
                }
                break;
            }
            case VarsControl.NUMBER: {
                if (!element.isEnable() && Functions.isNullOrEmpty(element.getValue())) {
                    return;
                }

                if (element.isEnable()) {
                    SharedView_PopupControlNumber popup = new SharedView_PopupControlNumber(activity.getLayoutInflater(), activity, "DetailWorkflow", lnAll);
                    popup.initializeValue_Master(element);
                    popup.initializeView();
                } else {
                    SharedView_PopupControlViewFullInfo popup = new SharedView_PopupControlViewFullInfo(activity.getLayoutInflater(), activity, "DetailWorkflow", lnAll);
                    popup.initializeValue_Master(element);
                    popup.initializeView();
                }
                break;
            }
            case VarsControl.TABS: {
                break;
            }
            case VarsControl.ATTACHMENT: {
                break;
            }
            case VarsControl.ATTACHMENTVERTICAL: {
                break;
            }
            case VarsControl.YESNO: {
                if (element.isEnable()) {
                    workflowControlDynamicListener.OnUpdateValueForElement(element, false);
                }
                break;
            }
            case VarsControl.TREE: {
                break;
            }
            case VarsControl.ATTACHMENTVERTICALFORMFRAME: {
                break;
            }
            case VarsControl.TEXTINPUT:
            case VarsControl.TEXTINPUTMULTILINE: {
                if (element.isEnable()) {
                    SharedView_PopupControlTextInput popup = new SharedView_PopupControlTextInput(activity.getLayoutInflater(), activity, "DetailWorkflow", lnAll);
                    popup.initializeValue_Master(element);
                    popup.initializeView();
                } else {
                    SharedView_PopupControlViewFullInfo popup = new SharedView_PopupControlViewFullInfo(activity.getLayoutInflater(), activity, "DetailWorkflow", lnAll);
                    popup.initializeValue_Master(element);
                    popup.initializeView();
                }
                break;
            }
            // Text Editor
            case VarsControl.TEXTINPUTFORMAT: {
                if (element.isEnable()) {
                    SharedView_PopupControlTextInputFormat popup = new SharedView_PopupControlTextInputFormat(activity.getLayoutInflater(), activity, "DetailWorkflow", lnAll);
                    popup.initializeValue_Master(element);
                    popup.initializeView();
                }
                break;
            }
            case VarsControl.INPUTATTACHMENTHORIZON:
            case VarsControl.INPUTATTACHMENTVERTICAL: {
                break;
            }
            case VarsControl.INPUTWORKRELATED: {
                break;
            }
        }
    }

    public void bottomActionClick(Activity
                                          activity, ActivityResultLauncher<Intent> launcher,
                                  WorkflowItem workflowItem, ArrayList<WorkflowHistory> workflowHistories,
                                  View lnAll, ButtonAction buttonAction) {
        switch (buttonAction.getID()) {
            case Variable.WorkflowAction.Next:
            case Variable.WorkflowAction.Approve: {
                // Đang lưu - Đang thực hiện thì gửi API luôn, không cần hiển thị popup
                Integer[] actionArr = new Integer[]{1, 4};
                if (Arrays.stream(actionArr).anyMatch(r -> r == workflowItem.getStatusGroup())) {
                    workflowControlDynamicListener.OnSendAPI(buttonAction, "", new HashMap<>());
                } else {
                    SharedView_PopupActionAccept actionAccept = new SharedView_PopupActionAccept(activity.getLayoutInflater(), activity, "DetailWorkflow", lnAll);
                    actionAccept.initializeValue_DetailWorkflow(buttonAction);
                    actionAccept.initializeView();
                }
                break;
            }
            case Variable.WorkflowAction.Forward:
            case Variable.WorkflowAction.RequestIdea: {
                SharedView_PopupActionForward forward = new SharedView_PopupActionForward(activity.getLayoutInflater(), activity, "DetailWorkflow", lnAll, workflowItem, 0.9f);
                forward.initializeValue_DetailWorkflow(buttonAction);
                forward.initializeView();
                break;
            }
            case Variable.WorkflowAction.Return:
            case Variable.WorkflowAction.Reject:
            case Variable.WorkflowAction.Cancel:
            case Variable.WorkflowAction.Idea: {
                SharedView_PopupActionReject reject = new SharedView_PopupActionReject(activity.getLayoutInflater(), activity, "DetailWorkflow", lnAll, buttonAction);
                reject.initializeValue_DetailWorkflow(buttonAction);
                reject.initializeView();
                break;
            }
            case Variable.WorkflowAction.RequestInformation: {
                ArrayList<User> users = getListUserFromQTLC(workflowItem, workflowHistories);
                SharedView_PopupActionRequestInfo info = new SharedView_PopupActionRequestInfo(activity.getLayoutInflater(), activity, "DetailWorkflow", lnAll, 0.9f, workflowItem, users);
                info.initializeValue_DetailWorkflow(buttonAction);
                info.initializeView();
                break;
            }
            case Variable.WorkflowAction.CreateTask: {
                Intent intent = new Intent(activity, DetailCreateTaskActivity.class);
                intent.putExtra("WorkflowItemId", workflowItem.getID());
                intent.putExtra("isClickFromAction", true);
                intent.putExtra("taskId", -1);
                launcher.launch(intent);
                break;
            }
            // 32 - Action Thu hồi
            default: {
                workflowControlDynamicListener.OnSendAPI(buttonAction, "", new HashMap<>());
                break;
            }
        }
    }

    public void handleBottomClicks(Activity
                                           activity, ActivityResultLauncher<Intent> launcher,
                                   ArrayList<ViewSection> sections,
                                   WorkflowItem workflowItem, ArrayList<WorkflowHistory> workflowHistories,
                                   View lnAll,
                                   String strElement, ComponentButtonBot componentButtonBot) {
        ViewElement viewElement = new Gson().fromJson(strElement, ViewElement.class);

        if (viewElement.getID().equals("more")) {
            ArrayList<ButtonAction> actions = componentButtonBot.getLstActionMore();
            if (actions != null && actions.size() > 0) {
                SharedView_PopupActionMore sharedView_popupActionMore = new SharedView_PopupActionMore(activity.getLayoutInflater(), activity, "", null);
                sharedView_popupActionMore.initializeValue_DetailWorkflow_ActionMore(actions);
                sharedView_popupActionMore.initializeView();
            }
        } else {
            // Các Action khác
            if (!DetailFunc.share.validateRequiredForm(sections)) {
                Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("MESS_FIELD_REQUIRE",
                        "Vui lòng nhập đầy đủ thông tin."), Functions.share.getTitle("TEXT_CLOSE", "Close"), activity);
                return;
            } else {
                ButtonAction action = new ButtonAction();
                action.setID(Integer.parseInt(viewElement.getID()));
                action.setTitle(viewElement.getTitle());
                action.setValue(viewElement.getValue());
                action.setNotes(viewElement.getNotes());
                bottomActionClick(activity, launcher, workflowItem, workflowHistories, lnAll, action);
            }
        }
    }

    public void gotoSelf(Activity activity, Intent intent, WorkflowItem workflowItem) {
        WorkFlowRelated related = new Gson().fromJson(intent.getStringExtra("related"), WorkFlowRelated.class);
        String workflowId;
        if (related.getItemID() == Integer.parseInt(workflowItem.getID())) {
            workflowId = String.valueOf(related.getItemRLID());
        } else {
            workflowId = String.valueOf(related.getItemID());
        }

        Intent i = new Intent(activity, DetailWorkflowActivity.class);
        i.putExtra("WorkflowItemId", workflowId);
        activity.startActivity(i);
    }

    public void gotoTask(Activity activity, ActivityResultLauncher<Intent> launcher, Intent
            intent) {
        Intent i = new Intent(activity, DetailCreateTaskActivity.class);
        i.putExtra("WorkflowItemId", intent.getIntExtra("WorkflowItemId", 0));
        i.putExtra("isClickFromAction", false);
        i.putExtra("taskId", intent.getIntExtra("taskId", 0));
        //launcher.launch(i);
        activity.startActivity(i);
    }

    public void updateValueForPopupGridDetail(TemplateValueTypeAdapter
                                                      adapterTemplateControlGrid, ViewElement parentElement, ViewElement childElement, JSONObject
                                                      jObjectChild, String _newValue) {
        if (adapterTemplateControlGrid != null) {
            jObjectChild = adapterTemplateControlGrid.getCurrentJObject();

            try {
                Object type = jObjectChild.get(childElement.getInternalName());
                if (type instanceof Integer || type instanceof Float || type instanceof Double || type instanceof Long) {
                    jObjectChild.put(childElement.getInternalName(), Long.parseLong(_newValue));
                } else {
                    jObjectChild.put(childElement.getInternalName(), _newValue);
                }

            } catch (Exception ex) {
                Log.d("ERR updateValueForPopupGridDetail", ex.getMessage());
            }

            adapterTemplateControlGrid.updateCurrentJObject(jObjectChild);
            adapterTemplateControlGrid.notifyDataSetChanged();
        }
    }

    public void handleChildActionClicks(Activity activity, View
            lnAll, TemplateValueTypeAdapter adapterTemplateControlGrid, ViewElement
                                                elementParent, ViewElement elementChild, JSONObject jsonOb, int flagView) {
        if (flagView != Vars.FlagViewControlAttachment.DetailWorkflow) {
            return;
        }

        switch (elementChild.getDataType()) {
            case VarsControl.SELECTUSER: {
                if (elementChild.isEnable()) {
                    SharedView_PopupControlSelectUserGroup popup = new SharedView_PopupControlSelectUserGroup(activity.getLayoutInflater(), activity, "DetailWorkflow", lnAll, false);
                    popup.initializeValue_InputGridDetail(elementParent, elementChild, jsonOb);
                    popup.initializeView();
                }
                break;
            }
            case VarsControl.SELECTUSERGROUP: {
                if (elementChild.isEnable()) {
                    SharedView_PopupControlSelectUserGroup popup = new SharedView_PopupControlSelectUserGroup(activity.getLayoutInflater(), activity, "DetailWorkflow", lnAll, true);
                    popup.initializeValue_InputGridDetail(elementParent, elementChild, jsonOb);
                    popup.initializeView();
                }
                break;
            }
            case VarsControl.SELECTUSERMULTI: {
                if (elementChild.isEnable()) {
                    SharedView_PopupControlSelectUserGroupMulti popup = new SharedView_PopupControlSelectUserGroupMulti(activity.getLayoutInflater(), activity, "WorkflowDetail", lnAll, true);
                    popup.initializeValue_InputGridDetail(elementParent, elementChild, jsonOb);
                    popup.initializeView();
                }
                break;
            }
            case VarsControl.DATE: {
                if (elementChild.isEnable()) {
                    SharedView_PopupControlDate popup = new SharedView_PopupControlDate(activity.getLayoutInflater(), activity, "DetailWorkflow", lnAll);
                    popup.initializeValue_InputGridDetail(elementParent, elementChild, jsonOb);
                    popup.initializeView();
                }
                break;
            }
            case VarsControl.DATETIME: {
                if (elementChild.isEnable()) {
                    SharedView_PopupControlDateTime popup = new SharedView_PopupControlDateTime(activity.getLayoutInflater(), activity, "DetailWorklfow", lnAll);
                    popup.initializeValue_InputGridDetail(elementParent, elementChild, jsonOb);
                    popup.initializeView();
                }
                break;
            }
            case VarsControl.TIME:
                break;
            case VarsControl.SINGLECHOICE: {
                if (elementChild.isEnable()) {
                    SharedView_PopupControlSingleChoice popup = new SharedView_PopupControlSingleChoice(activity.getLayoutInflater(), activity, "DetailWorklfow", lnAll);
                    popup.initializeValue_InputGridDetail(elementParent, elementChild, jsonOb);
                    popup.initializeView();
                }
                break;
            }
            case VarsControl.SINGLELOOKUP: {
                if (elementChild.isEnable()) {
                    SharedView_PopupControlSingleLookup popup = new SharedView_PopupControlSingleLookup(activity.getLayoutInflater(), activity, "DetailWorklfow", lnAll);
                    popup.initializeValue_InputGridDetail(elementParent, elementChild, jsonOb);
                    popup.initializeView();
                }
                break;
            }
            case VarsControl.MULTIPLECHOICE: {
                if (elementChild.isEnable()) {
                    SharedView_PopupControlMultiChoice popup = new SharedView_PopupControlMultiChoice(activity.getLayoutInflater(), activity, "DetailWorkflow", lnAll);
                    popup.initializeValue_InputGridDetail(elementParent, elementChild, jsonOb);
                    popup.initializeView();
                }
                break;
            }
            case VarsControl.MULTIPLELOOKUP: {
                if (elementChild.isEnable()) {
                    SharedView_PopupControlMultiLookup popup = new SharedView_PopupControlMultiLookup(activity.getLayoutInflater(), activity, "DetailWorkflow", lnAll);
                    popup.initializeValue_InputGridDetail(elementParent, elementChild, jsonOb);
                    popup.initializeView();
                }
                break;
            }
            case VarsControl.NUMBER: {
                if (elementChild.isEnable()) {
                    SharedView_PopupControlNumber popup = new SharedView_PopupControlNumber(activity.getLayoutInflater(), activity, "DetailWorkflow", lnAll);
                    popup.initializeValue_InputGridDetail(elementParent, elementChild, jsonOb);
                    popup.initializeView();
                }
                break;
            }
            case VarsControl.TABS:
                break;
            case VarsControl.ATTACHMENT:
            case VarsControl.ATTACHMENTVERTICAL:
                break;
            case VarsControl.YESNO: {
                if (elementChild.isEnable()) {
                    updateValueForPopupGridDetail(adapterTemplateControlGrid, elementParent, elementChild, jsonOb, elementChild.getValue());
                }
                break;
            }
            case VarsControl.TREE:
                break;
            case VarsControl.ATTACHMENTVERTICALFORMFRAME:
                break;
            case VarsControl.TEXTINPUT:
            case VarsControl.TEXTINPUTMULTILINE: {
                if (elementChild.isEnable()) {
                    SharedView_PopupControlTextInput popup = new SharedView_PopupControlTextInput(activity.getLayoutInflater(), activity, "DetailWorkflow", lnAll);
                    popup.initializeValue_InputGridDetail(elementParent, elementChild, jsonOb);
                    popup.initializeView();
                }
                break;
            }
            case VarsControl.TEXTINPUTFORMAT: {
                if (elementChild.isEnable()) {
                    SharedView_PopupControlTextInputFormat popup = new SharedView_PopupControlTextInputFormat(activity.getLayoutInflater(), activity, "DetailWorkflow", lnAll);
                    popup.initializeValue_InputGridDetail(elementParent, elementChild, jsonOb);
                    popup.initializeView();
                }
                break;
            }
            case VarsControl.INPUTATTACHMENTHORIZON:
            case VarsControl.INPUTATTACHMENTVERTICAL:
            case VarsControl.INPUTWORKRELATED:
                break;
        }
    }

    public void submitAction(Intent intent) {
        ButtonAction buttonAction = new Gson().fromJson(intent.getStringExtra("buttonAction"), ButtonAction.class);
        String comment = intent.getStringExtra("comment");

        HashMap<String, String> hashMap = new HashMap<>();
        if (intent.hasExtra("extension")) {
            hashMap = (HashMap<String, String>) intent.getSerializableExtra("extension");
        }

        workflowControlDynamicListener.OnSendAPI(buttonAction, comment, hashMap);
    }

    public void mappingFields(ArrayList<ViewSection> sections, ArrayList<ViewRow> rows) {
        for (ViewSection s : sections) {
            for (ViewRow r : s.getViewRows()) {
                for (ViewElement e : r.getElements()) {
                    for (ViewRow rf : rows) {
                        for (ViewElement ef : rf.getElements()) {
                            if (e.getDataType().equals(VarsControl.INPUTATTACHMENTHORIZON) || e.getDataType().equals(VarsControl.INPUTGRIDDETAILS)) {
                                if (e.getID().equals(ef.getID()) && e.getDataType().equals(ef.getDataType())) {
                                    r.setElements(rf.getElements());
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void getAppBaseItem(String rid) {
        Call<ApiList<AppBase>> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getAppBaseItem(rid);
        call.enqueue(new Callback<ApiList<AppBase>>() {
            @Override
            public void onResponse(Call<ApiList<AppBase>> call, Response<ApiList<AppBase>> response) {
                if (response.isSuccessful()) {
                    if (response.body() != null && response.body().getStatus().equals("SUCCESS")) {
                        pushNotificationListener.OnGetItemSuccess(response.body().getData().get(0));
                    } else {
                        pushNotificationListener.OnGetItemError(Functions.share.getTitle("MESS_WORKFLOW_NOTFOUND", "Không tìm thấy thông tin phiếu, vui lòng thử lại sau!"));
                    }
                } else {
                    pushNotificationListener.OnGetItemError(Functions.share.getTitle("MESS_WORKFLOW_NOTFOUND", "Không tìm thấy thông tin phiếu, vui lòng thử lại sau!"));
                }
            }

            @Override
            public void onFailure(Call<ApiList<AppBase>> call, Throwable e) {
                if (e instanceof UnknownHostException || e instanceof ConnectException) {
                    pushNotificationListener.OnGetItemError(Functions.share.getTitle("TEXT_INTERNET", "Không có kết nối mạng!"));
                } else {
                    pushNotificationListener.OnGetItemError(Functions.share.getTitle("MESS_WORKFLOW_NOTFOUND", "Không tìm thấy thông tin phiếu, vui lòng thử lại sau!"));
                }
            }
        });
    }
}



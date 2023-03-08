package com.vuthao.bpmop.detail.presenter;

import android.app.Activity;

import com.google.gson.Gson;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.api.ApiController;
import com.vuthao.bpmop.base.model.app.Status;
import com.vuthao.bpmop.base.model.custom.AttachFile;
import com.vuthao.bpmop.base.model.custom.Comment;
import com.vuthao.bpmop.base.model.custom.OtherResource;
import com.vuthao.bpmop.base.realm.RealmController;
import com.vuthao.bpmop.detail.DetailWorkflowActivity;
import com.vuthao.bpmop.task.DetailCreateTaskActivity;

import java.io.File;
import java.util.ArrayList;
import java.util.HashMap;

import io.realm.Realm;
import okhttp3.MediaType;
import okhttp3.MultipartBody;
import okhttp3.RequestBody;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ReplyCommentPresenter implements Callback {
    private ReplyCommentListener listener;
    private Realm realm;

    @Override
    public void onResponse(Call call, Response response) {

    }

    @Override
    public void onFailure(Call call, Throwable t) {

    }

    public interface ReplyCommentListener {
        void OnSendCommentSuccess();
        void OnLikeCommentSuccess();
        void OnSendCommentErr(String err);
        void OnLikeCommentErr(String err);
    }

    public ReplyCommentPresenter() {
    }

    public ReplyCommentPresenter(ReplyCommentListener listener) {
        this.listener = listener;
        realm = new RealmController().getRealm();
    }

    public void sendComment(String comment, ArrayList<AttachFile> files, String _OtherResourceId, String _ResourceCategoryId, String parentCommentId) {
        OtherResource otherResource = new OtherResource();
        otherResource.setContent(comment.trim().replace("  ", " "));
        otherResource.setResourceId(_OtherResourceId);
        otherResource.setResourceCategoryId(Integer.parseInt(_ResourceCategoryId));
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

            // Add thêm request body vào trong builder
            filesComment[i] = MultipartBody.Part.createFormData("file",
                    f.getName(),
                    requestBody);
        }

        Call<Status> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).addComment(filesComment, data);
        call.enqueue(new Callback<Status>() {
            @Override
            public void onResponse(Call<Status> call, Response<Status> response) {
                if (response.isSuccessful()) {
                    if (response.body().getStatus().equals("SUCCESS")) {
                        listener.OnSendCommentSuccess();
                    } else {
                        listener.OnSendCommentErr(Functions.share.getTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!"));
                    }
                } else {
                    listener.OnSendCommentErr(Functions.share.getTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!"));
                }
            }

            @Override
            public void onFailure(Call<Status> call, Throwable t) {
                listener.OnSendCommentErr(Functions.share.getTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!"));
            }
        });
    }

    public void addLikeComment(Comment comment, String _ResourceId, boolean _Flag, int _ResourceCategoryId) {
        HashMap<String, String> hashMap = new HashMap<>();
        hashMap.put("ResourceId", _ResourceId);
        hashMap.put("ResourceCategoryId", String.valueOf(_ResourceCategoryId));

        String func = _Flag ? "like" : "unlike";
        String data = new Gson().toJson(hashMap);

        Call<Status> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).addLikeComment(func, data);
        call.enqueue(this);
    }

    public void setLikeComment(Activity activity, ArrayList<Comment> comments, Comment comment) {
        if (activity instanceof DetailCreateTaskActivity) {
            for (int i = 0; i < comments.size(); i++) {
                if (comment.getID().equals(comments.get(i).getID())) {
                    comments.get(i).setLiked(comment.isLiked() == 1 ? 0 : 1);

                    if (comments.get(i).isLiked() == 1) {
                        comments.get(i).setLikeCount(comment.getLikeCount() + 1);
                    } else {
                        comments.get(i).setLikeCount(comments.get(i).getLikeCount() - 1 < 0 ? 0 : comments.get(i).getLikeCount() - 1);
                    }
                }
            }
        } else if (activity instanceof DetailWorkflowActivity) {
            new RealmController().getRealm().executeTransaction(new Realm.Transaction() {
                @Override
                public void execute(Realm realm) {
                    for (int i = 0; i < comments.size(); i++) {
                        if (comment.getID().equals(comments.get(i).getID())) {
                            comments.get(i).setLiked(comment.isLiked() == 1 ? 0 : 1);

                            if (comments.get(i).isLiked() == 1) {
                                comments.get(i).setLikeCount(comment.getLikeCount() + 1);
                            } else {
                                comments.get(i).setLikeCount(comments.get(i).getLikeCount() - 1 < 0 ? 0 : comments.get(i).getLikeCount() - 1);
                            }

                            realm.copyToRealmOrUpdate(comments.get(i));
                        }
                    }
                }
            });
        }

        listener.OnLikeCommentSuccess();
    }
}

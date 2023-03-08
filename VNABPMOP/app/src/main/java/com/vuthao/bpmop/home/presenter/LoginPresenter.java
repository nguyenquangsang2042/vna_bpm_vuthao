package com.vuthao.bpmop.home.presenter;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.net.Uri;

import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.activity.SplashActivity;
import com.vuthao.bpmop.base.api.ApiAuthController;
import com.vuthao.bpmop.base.api.ApiController;
import com.vuthao.bpmop.base.model.app.DBVariable;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.realm.RealmHelper;
import com.vuthao.bpmop.base.vars.VarsTable;

import java.net.ConnectException;
import java.net.UnknownHostException;

import okhttp3.MediaType;
import okhttp3.RequestBody;
import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.Callback;

public class LoginPresenter {
    private Context context;
    private LoginListener listener;
    private ApiAuthController apiAuth;

    public interface LoginListener {
        void OnAuthSucess(String cookie);

        void OnGetAuthError(String error);

        void OnLoginSuccess(User user);

        void OnLoginErr(String err);

        void OnCookieExpire(String err);
        void OnGetDataSuccess();
    }

    public LoginPresenter(Context context, LoginListener listener) {
        this.context = context;
        this.listener = listener;
        apiAuth = new ApiAuthController(listener);
    }

    public LoginPresenter(Context context) {
        this.context = context;
    }

    public void authencate(String username, String password) {
        if (Functions.isNullOrEmpty(username) || Functions.isNullOrEmpty(password)) {
            listener.OnGetAuthError(Functions.share.getTitle("TEXT_LOGINFAIL", "Login information is incorrect, please try again."));
        } else {
            String soap =
                    "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                            "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">"
                            + "<soap:Body>"
                            + "<Login xmlns=\"http://schemas.microsoft.com/sharepoint/soap/\">"
                            + "<username>" + username + "</username>"
                            + "<password>" + password + "</password>"
                            + "</Login>" + "</soap:Body>"
                            + "</soap:Envelope>";

            MediaType mediaType = MediaType.parse("text/xml");
            RequestBody requestBody = RequestBody.create(mediaType, soap);

            Call<ResponseBody> call = new ApiController().getApiRouteToken("").authencate(requestBody);
            call.enqueue(new Callback<ResponseBody>() {
                @Override
                public void onResponse(Call<ResponseBody> call, retrofit2.Response<ResponseBody> response) {
                    if (response.isSuccessful()) {
                        if (Functions.isNullOrEmpty(response.headers().get("Set-Cookie"))) {
                            listener.OnGetAuthError(Functions.share.getTitle("TEXT_LOGINFAIL", "Login information is incorrect, please try again."));
                        } else {
                            listener.OnAuthSucess(response.headers().get("Set-Cookie"));
                        }
                    } else {
                        listener.OnGetAuthError(Functions.share.getTitle("TEXT_LOGINFAIL", "Login information is incorrect, please try again."));
                    }
                }

                @Override
                public void onFailure(Call<ResponseBody> call, Throwable e) {
                    if (e instanceof UnknownHostException || e instanceof ConnectException) {
                        listener.OnGetAuthError(Functions.share.getTitle("MESS_REQUIRE_NETWORK", "No network connection, please try again."));
                    } else {
                        listener.OnGetAuthError(Functions.share.getTitle("TEXT_LOGINFAIL", "Login information is incorrect, please try again."));
                    }
                }
            });
        }
    }

    public void Login(String token, String deviceInfo, String loginType) {
        apiAuth.Login(token, deviceInfo, loginType);
    }

    private String getModified(String table, boolean flgGetAll) {
        String modified = "";
        if (flgGetAll) {
            if (table.equals(VarsTable.APPLANGUAGE)) {
                modified = "";
            } else {
                modified = Functions.share.getDateStringApi(-Constants.dataLimitDay);
            }
        } else {
            DBVariable db = new RealmHelper<DBVariable>().getItemById(DBVariable.class, "Id", table);
            if (db != null) {
                modified = db.getValue();
            }
        }

        return modified;
    }

    public void getData(boolean flgGetAll) {
        apiAuth.getSettings(getModified(VarsTable.SETTINGS, flgGetAll),  !flgGetAll ? "1" : "0");
        apiAuth.getAppLanguage(getModified(VarsTable.APPLANGUAGE, !flgGetAll));
        apiAuth.getTimeLanguage(getModified(VarsTable.TIMELANGUAGE, flgGetAll),  !flgGetAll ? "1" : "0");
        apiAuth.getAppStatus(getModified(VarsTable.APPSTATUS, flgGetAll),  !flgGetAll ? "1" : "0");
        apiAuth.getPositions(getModified(VarsTable.POSITION, flgGetAll),  !flgGetAll ? "1" : "0");
        apiAuth.getWorkflowStepDefine(getModified(VarsTable.WORKFLOWSTEPDEFINE, flgGetAll),  !flgGetAll ? "1" : "0");
        apiAuth.getResourceView(getModified(VarsTable.RESOURCEVIEW, flgGetAll),  !flgGetAll ? "1" : "0");
        apiAuth.getWorkflowStatus(getModified(VarsTable.WORKFLOWSTATUS, flgGetAll),  !flgGetAll ? "1" : "0");

        //apiAuth.getNotify(getModified(VarsTable.NOTIFY, flgGetAll),  !flgGetAll ? "1" : "0");
        //apiAuth.getAppBase(getModified(VarsTable.APPBASE, flgGetAll),  !flgGetAll ? "1" : "0");
        //apiAuth.getWorkflows(getModified(VarsTable.WORKFLOWS, flgGetAll),  !flgGetAll ? "1" : "0");
        //apiAuth.getWorkflowFollow(getModified(VarsTable.WORKFLOWFOLLOW, flgGetAll),  !flgGetAll ? "1" : "0");
        //apiAuth.getWorkflowCategory(getModified(VarsTable.WORKFLOWCATEGORY, flgGetAll),  !flgGetAll ? "1" : "0");
        //apiAuth.getWorkflowItems(getModified(VarsTable.WORKFLOWITEM, flgGetAll),  !flgGetAll ? "1" : "0");
        //apiAuth.getUsers(getModified(VarsTable.USERS, flgGetAll),  !flgGetAll ? "1" : "0");
        //apiAuth.getGroups(getModified(VarsTable.GROUPS, flgGetAll),  !flgGetAll ? "1" : "0");
    }
}

package com.vuthao.bpmop.leftmenu.presenter;

import android.app.Activity;
import android.content.Intent;

import androidx.annotation.NonNull;

import com.google.gson.Gson;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.activity.SplashActivity;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.api.ApiController;
import com.vuthao.bpmop.base.model.ApiList;
import com.vuthao.bpmop.base.model.app.AppLanguage;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.DBVariable;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.model.app.Workflow;
import com.vuthao.bpmop.base.model.custom.ObjectData;
import com.vuthao.bpmop.base.realm.RealmController;
import com.vuthao.bpmop.base.realm.RealmHelper;
import com.vuthao.bpmop.leftmenu.LeftMenuFragment;

import java.net.ConnectException;
import java.net.UnknownHostException;
import java.util.ArrayList;
import java.util.HashMap;

import io.realm.Realm;
import io.realm.RealmResults;
import io.realm.Sort;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class LeftMenuPresenter {
    private LeftMenuFragment leftMenuFragment;
    private LeftMenuListener listener;
    private LeftMenuBottomNavListener bottomNavListener;
    private LeftMenuLanguageListenter languageListenter;

    public interface LeftMenuListener {
        void OnVDTCount(int count);

        void OnCountFollow(int count);

        void OnVTBDCount(int count);

    }

    public interface LeftMenuBottomNavListener {
        void OnBottomClick(int redirect);
    }

    public interface LeftMenuLanguageListenter {
        void OnChangeLanguageError(String errorVN);

        void OnChangeLangueSuccess(String langCode);

        void OnUpdateAppLanguageSuccess(String langCode);
    }

    public LeftMenuPresenter(LeftMenuFragment leftMenuFragment, LeftMenuListener listener, LeftMenuLanguageListenter languageListenter) {
        this.leftMenuFragment = leftMenuFragment;
        this.listener = listener;
        this.languageListenter = languageListenter;
    }

    public LeftMenuPresenter(LeftMenuFragment leftMenuFragment, LeftMenuListener listener) {
        this.leftMenuFragment = leftMenuFragment;
        this.listener = listener;
    }

    public LeftMenuPresenter(LeftMenuBottomNavListener bottomNavListener) {
        this.bottomNavListener = bottomNavListener;
    }

    public void setCountVDT(int count) {
        listener.OnVDTCount(count);
    }

    public void setCountFollow(int count) {
        listener.OnCountFollow(count);
    }

    public void setCountVTBD(int count) {
        listener.OnVTBDCount(count);
    }

    public void signOut() {
        String message = Functions.share.getTitle("TEXT_CONFIRM_SIGNOUT", "Bạn có muốn đăng xuất khỏi tài khoản?");
        String textOk = Functions.share.getTitle("TEXT_AGREE", "Đồng ý");
        String textCancle = Functions.share.getTitle("TEXT_CANCEL", "Hủy");

        Utility.share.showAlertWithOKCancel(message, textOk, textCancle, leftMenuFragment.requireActivity(), () -> {
            BaseActivity.setToken(null);
            Utility.share.resetData(leftMenuFragment.requireContext());
            Activity activity = leftMenuFragment.requireActivity();

            Intent intent = new Intent(activity, SplashActivity.class);
            activity.startActivity(intent);
            activity.finish();
        }, () -> {
        });
    }

    public void updateUserLanguage(String langCode) {
        HashMap<String, String> hashMap = new HashMap<>();
        hashMap.put("ID", CurrentUser.getInstance().getUser().getID());
        hashMap.put("Language", langCode);

        String data = new Gson().toJson(hashMap);
        Call<ObjectData> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).updateUserLanguage(data);
        call.enqueue(new Callback<ObjectData>() {
            @Override
            public void onResponse(@NonNull Call<ObjectData> call, @NonNull Response<ObjectData> response) {

            }

            @Override
            public void onFailure(@NonNull Call<ObjectData> call, @NonNull Throwable e) {
                if (e instanceof UnknownHostException || e instanceof ConnectException) {
                    languageListenter.OnChangeLanguageError(Functions.share.getTitle("TEXT_LOGINFAIL", "Không có kết nối mạng."));
                } else {
                    languageListenter.OnChangeLanguageError(Functions.share.getTitle("TEXT_LOGINFAIL", "Không có kết nối mạng."));
                }
            }
        });
    }

    public void updateAppLanguage(String langCode) {
        Call<ApiList<AppLanguage>> call = new ApiController().getApiRouteToken("").updateAppLanguage(langCode);
        call.enqueue(new Callback<ApiList<AppLanguage>>() {
            @Override
            public void onResponse(@NonNull Call<ApiList<AppLanguage>> call, @NonNull Response<ApiList<AppLanguage>> response) {
                if (response.isSuccessful()) {
                    ApiList<AppLanguage> setting = response.body();
                    assert setting != null;
                    if (setting.getStatus().equals("SUCCESS")) {
                        if (setting.getData() != null && setting.getData().size() > 0)
                            new RealmHelper<AppLanguage>().addNewItems(setting.getData());
                        DBVariable db = new DBVariable("AppLanguage", setting.getDateNow());
                        new RealmHelper<DBVariable>().addNewItem(db);
                        languageListenter.OnUpdateAppLanguageSuccess(langCode);
                    }
                }
            }

            @Override
            public void onFailure(@NonNull Call<ApiList<AppLanguage>> call, @NonNull Throwable t) {

            }
        });
    }

    public void onBottomNavigationClick(int to) {
        bottomNavListener.OnBottomClick(to);
    }

    public ArrayList<Workflow> getApps() {
        RealmResults<Workflow> results = new RealmController().getRealm().where(Workflow.class)
                .equalTo("Favorite", 1)
                .and()
                .equalTo("StatusName", "Active")
                .sort("Title", Sort.ASCENDING)
                .findAll();
        return new ArrayList<>(results);
    }
}

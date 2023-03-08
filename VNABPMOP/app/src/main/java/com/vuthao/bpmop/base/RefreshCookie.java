package com.vuthao.bpmop.base;

import android.app.Activity;
import android.content.Intent;

import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.activity.SplashActivity;
import com.vuthao.bpmop.base.api.ApiController;
import com.vuthao.bpmop.base.model.ApiObject;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.User;

import java.net.ConnectException;
import java.net.UnknownHostException;

import retrofit2.Call;
import retrofit2.Callback;

public class RefreshCookie extends ApiController {
    private final RefreshCookieListener callback;
    private final String ERRVN = "Phiên đăng nhập hết hạn. Vui lòng đăng nhập lại!";
    private final String ERREN = "Login session expired. Please log in again!";
    private String ERR = "";

    public RefreshCookie(RefreshCookieListener callback) {
        this.callback = callback;

        if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
            ERR = ERRVN;
        } else {
            ERR = ERREN;
        }
    }

    public void refreshCookie(String deviceInfo, String loginType) {
        Call<ApiObject<User>> call = getApiRouteToken(BaseActivity.getToken()).Login(deviceInfo, loginType);
        call.enqueue(new Callback<ApiObject<User>>() {
            @Override
            public void onResponse(Call<ApiObject<User>> call, retrofit2.Response<ApiObject<User>> response) {
                if (response.isSuccessful()) {
                    ApiObject<User> data = response.body();
                    assert data != null;
                    if (data.getStatus().equals("ERR")) {
                        if (data.getMess().getKey().equals("998")) {
                            callback.OnRefreshCookieErr(ERR);
                        }
                    }
                } else {
                    callback.OnRefreshCookieErr(ERR);
                }
            }

            @Override
            public void onFailure(Call<ApiObject<User>> call, Throwable e) {
                if (e instanceof UnknownHostException || e instanceof ConnectException) {
                } else {
                    callback.OnRefreshCookieErr(ERR);
                }
            }
        });
    }

    public void refresh(Activity activity) {
        BaseActivity.setToken(null);
        Utility.share.resetData(activity);

        Intent intent = new Intent(activity, SplashActivity.class);
        activity.startActivity(intent);
        activity.finish();
    }

    public interface RefreshCookieListener {
        void OnRefreshCookieErr(String err);
    }
}

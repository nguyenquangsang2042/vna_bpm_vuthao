package com.vuthao.bpmop.activity;

import android.Manifest;
import android.annotation.SuppressLint;
import android.content.Intent;
import android.graphics.Typeface;
import android.os.Build;
import android.os.Bundle;
import android.provider.Settings;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.View;
import android.view.inputmethod.EditorInfo;
import android.webkit.WebChromeClient;
import android.webkit.WebView;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.cardview.widget.CardView;
import androidx.core.content.res.ResourcesCompat;

import com.bumptech.glide.BuildConfig;
import com.google.android.gms.tasks.OnCompleteListener;
import com.google.android.gms.tasks.OnFailureListener;
import com.google.android.gms.tasks.Task;
import com.google.firebase.installations.FirebaseInstallations;
import com.google.firebase.installations.InstallationTokenResult;
import com.google.firebase.messaging.FirebaseMessaging;
import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.NetworkUtil;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.activity.SplashActivity;
import com.vuthao.bpmop.base.keyboard.KeyboardManager;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.DeviceInfo;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.realm.RealmHelper;
import com.vuthao.bpmop.home.presenter.LoginPresenter;

import java.util.concurrent.Executor;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;

public class StartActivity extends BaseActivity implements LoginPresenter.LoginListener {
    @BindView(R.id.tv_ViewStartView_Login)
    TextView tvLogin;
    @BindView(R.id.tv_ViewStartView_Note)
    TextView tvNote;
    @BindView(R.id.edt_ViewStartView_Pass)
    EditText edtPass;
    @BindView(R.id.edt_ViewStartView_Username)
    EditText edtUser;
    @BindView(R.id.ln_ViewStartView_Name)
    LinearLayout lnName;
    @BindView(R.id.ln_ViewStartView_Pass)
    LinearLayout lnPass;
    @BindView(R.id.card_ViewStartView_Login)
    CardView cardLogin;
    @BindView(R.id.web_ViewStartView_Advertisement)
    WebView webAd;

    private boolean isAutoLogin = false;
    private LoginPresenter presenter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.fragment_start_view);
        ButterKnife.bind(this);

        addListener();
        init();
        checkLogin();
        getDeviceInfo();

        edtUser.setText("nvien2");
        edtPass.setText("VTlamson123!@#");
    }

    private void checkLogin() {
        getPreferencesController().getUserLogin(userId -> {
            if (!Functions.isNullOrEmpty(userId)) {
                tvNote.setVisibility(View.VISIBLE);
                tvNote.setText("Please wait a minute...");
                lnName.setVisibility(View.INVISIBLE);
                lnPass.setVisibility(View.INVISIBLE);
                cardLogin.setVisibility(View.INVISIBLE);

                String deviceInfo = getPreferencesController().getDeviceInfo();
                presenter.Login(BaseActivity.getToken(), deviceInfo, "1");
                isAutoLogin = true;
            } else {
                isAutoLogin = false;
            }
        });
    }

    @SuppressLint("ClickableViewAccessibility")
    private void init() {
        presenter = new LoginPresenter(this, this);

        webAd.loadDataWithBaseURL("file:///android_res/drawable/", "<style>img{display: inline;width: auto;max-height: 50%;}</style> <img src='img_background_startview_3_small.png' />", "text/html", "utf-8", null);
        webAd.setVerticalScrollBarEnabled(false);
        webAd.setHorizontalScrollBarEnabled(false);
        webAd.getSettings().setLoadWithOverviewMode(true);
        webAd.getSettings().setUseWideViewPort(true);
        webAd.setWebChromeClient(new WebChromeClient());
        webAd.setInitialScale(1);
        webAd.setOnTouchListener((v, event) -> false);
    }

    @OnClick({R.id.tv_ViewStartView_Login})
    public void OnViewClicked(View v) {
        switch (v.getId()) {
            case R.id.tv_ViewStartView_Login: {
                checkUserPermission(new BaseActivity.PermissionListener() {
                    @Override
                    public void OnAcceptedAllPermission() {
                        Login();
                    }

                    @Override
                    public void OnCancelPermission() {
                    }

                    @Override
                    public void OnNeverRequestPermission() {
                    }
                }, new String[]{Manifest.permission.INTERNET,
                        Manifest.permission.CAMERA,
                        Manifest.permission.WRITE_EXTERNAL_STORAGE,
                        Manifest.permission.READ_EXTERNAL_STORAGE,
                        Manifest.permission.VIBRATE});
                break;
            }
        }
    }

    private void setVisble() {
        lnName.setVisibility(View.VISIBLE);
        lnPass.setVisibility(View.VISIBLE);
        cardLogin.setVisibility(View.VISIBLE);
        lnName.setEnabled(true);
        lnPass.setEnabled(true);
    }

    @SuppressLint("SetTextI18n")
    private void Login() {
        KeyboardManager.hideKeyboard(edtPass, this);
        tvNote.setVisibility(View.VISIBLE);
        tvNote.setText("Please wait a minute...");
        edtUser.setEnabled(false);
        edtPass.setEnabled(false);
        tvLogin.setEnabled(false);

        presenter.authencate(edtUser.getText().toString(), edtPass.getText().toString());
    }

    private void addListener() {
        edtUser.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {

            }

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {

            }

            @Override
            public void afterTextChanged(Editable s) {
                if (Functions.isNullOrEmpty(s.toString())) {
                    edtUser.setTypeface(ResourcesCompat.getFont(getApplicationContext(), R.font.fontarial), Typeface.ITALIC);
                } else {
                    edtUser.setTypeface(ResourcesCompat.getFont(getApplicationContext(), R.font.fontarial), Typeface.NORMAL);
                }
            }
        });

        edtPass.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {

            }

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {

            }

            @Override
            public void afterTextChanged(Editable s) {
                if (Functions.isNullOrEmpty(s.toString())) {
                    edtPass.setTypeface(ResourcesCompat.getFont(getApplicationContext(), R.font.fontarial), Typeface.ITALIC);
                } else {
                    edtPass.setTypeface(ResourcesCompat.getFont(getApplicationContext(), R.font.fontarial), Typeface.NORMAL);
                }
            }
        });

        edtPass.setOnEditorActionListener((v, actionId, event) -> {
            switch (actionId) {
                case EditorInfo.IME_ACTION_DONE:
                case EditorInfo.IME_ACTION_NEXT:
                case EditorInfo.IME_ACTION_GO: {
                    Login();
                    break;
                }
            }
            return false;
        });

        edtUser.setOnEditorActionListener((v, actionId, event) -> {
            switch (actionId) {
                case EditorInfo.IME_ACTION_DONE:
                case EditorInfo.IME_ACTION_NEXT:
                case EditorInfo.IME_ACTION_GO: {
                    edtPass.setFocusable(true);
                    edtPass.setFocusableInTouchMode(true);
                    edtPass.requestFocus();
                    break;
                }
            }
            return false;
        });
    }

    @SuppressLint("HardwareIds")
    private void getDeviceInfo() {
//        if (NetworkUtil.getConnectivityStatus(this) != NetworkUtil.NETWORK_STATUS_NOT_CONNECTED) {
//            DeviceInfo objDevice = new DeviceInfo();
//            FirebaseMessaging.getInstance().getToken().addOnCompleteListener(task -> {
//                Constants.deviceToken = task.getResult();
//                objDevice.setDevicePushToken(Constants.deviceToken);
//                objDevice.setDeviceName(Build.MODEL);
//                objDevice.setDeviceModel(Build.MODEL);
//                objDevice.setDeviceId(Settings.Secure.getString(getContentResolver(), Settings.Secure.ANDROID_ID));
//                objDevice.setDeviceOS(1);
//                objDevice.setDeviceOSVersion(String.valueOf(Build.VERSION.SDK_INT));
//                objDevice.setAppVersion(BuildConfig.VERSION_NAME);
//                getPreferencesController().saveDeviceInfo(new Gson().toJson(objDevice));
//            });
//        }

        DeviceInfo objDevice = new DeviceInfo();
        objDevice.setDevicePushToken("");
        objDevice.setDeviceName(Build.MODEL);
        objDevice.setDeviceModel(Build.MODEL);
        objDevice.setDeviceId(Settings.Secure.getString(getContentResolver(), Settings.Secure.ANDROID_ID));
        objDevice.setDeviceOS(1);
        objDevice.setDeviceOSVersion(String.valueOf(Build.VERSION.SDK_INT));
        objDevice.setAppVersion(BuildConfig.VERSION_NAME);
        getPreferencesController().saveDeviceInfo(new Gson().toJson(objDevice));
    }

    @Override
    public void OnAuthSucess(String cookie) {
        getPreferencesController().saveUserToken(cookie);
        String deviceInfo = getPreferencesController().getDeviceInfo();
        presenter.Login(cookie, deviceInfo, "1");
    }

    @Override
    public void OnGetAuthError(String errorVN) {
        Utility.share.showAlertWithOnlyOK(errorVN, "Close", StartActivity.this, () -> {
            setVisble();
            tvNote.setVisibility(View.INVISIBLE);
            edtUser.setEnabled(true);
            edtPass.setEnabled(true);
            tvLogin.setEnabled(true);
        });
    }

    @Override
    public void OnLoginSuccess(User user) {
        CurrentUser.getInstance().setUser(user);
        getPreferencesController().saveUserId(user.getID());
        //khong can doan nay vi xai` Cookie roi
        //getPreferencesController().saveUserLogin(edtUser.getText().toString(), edtPass.getText().toString());
        presenter.getData(isAutoLogin);
    }

    @Override
    public void OnLoginErr(String err) {
        isAutoLogin = false;
        OnGetAuthError(err);
    }

    @Override
    public void OnCookieExpire(String err) {
        Utility.share.showAlertWithOnlyOK(err, "Close", this, () -> {
            BaseActivity.setToken(null);
            Utility.share.resetData(this);

            setVisble();
            tvNote.setVisibility(View.INVISIBLE);
            edtUser.setEnabled(true);
            edtPass.setEnabled(true);
            tvLogin.setEnabled(true);
        });
    }

    @SuppressLint("SetTextI18n")
    @Override
    public void OnGetDataSuccess() {
        tvNote.setText("Update finished");
        Intent intent = new Intent(StartActivity.this, TabsActivity.class);
        startActivity(intent);
        finish();
    }
}

package com.vuthao.bpmop.base;

import android.content.Context;
import android.content.SharedPreferences;

import androidx.annotation.NonNull;

import com.vuthao.bpmop.R;

public class SharedPreferencesController {
    private static SharedPreferences sharedPreferences;
    private SharedPreferences.Editor editor;

    public SharedPreferencesController(@NonNull Context context) {
        if (sharedPreferences == null)
            sharedPreferences = context.getSharedPreferences(Functions.share.encodeBase64(context.getResources().getString(R.string.app_name)), Context.MODE_PRIVATE);
    }

    public void saveUserId(String userId) {
        SharedPreferences.Editor editor = sharedPreferences.edit();
        editor.putString(Functions.share.encodeBase64("ID"), Functions.share.encodeBase64(userId));
        editor.apply();
    }

    public void saveTourGuide(String nameTour) {
        SharedPreferences.Editor editor = sharedPreferences.edit();
        editor.putBoolean(Functions.share.encodeBase64(nameTour), true);
        editor.apply();
    }

    public void saveUserToken(String token) {
        SharedPreferences.Editor editor = sharedPreferences.edit();
        editor.putString(Functions.share.encodeBase64("token"), Functions.share.encodeBase64(token));
        editor.apply();
    }

    public void saveDeviceInfo(String deviceInfo) {
        SharedPreferences.Editor editor = sharedPreferences.edit();
        editor.putString(Functions.share.encodeBase64("deviceInfo"), Functions.share.encodeBase64(deviceInfo));
        editor.apply();
    }

    public String getDeviceInfo() {
        return Functions.share.decodeBase64(sharedPreferences.getString(Functions.share.encodeBase64("deviceInfo"), ""));
    }

    public String getUserToken() {
        return Functions.share.decodeBase64(sharedPreferences.getString(Functions.share.encodeBase64("token"), ""));
    }

    public boolean getTourGuide(String nameTour) {
        return sharedPreferences.getBoolean(Functions.share.encodeBase64(nameTour), false);
    }

    public String getUserId() {
        return Functions.share.decodeBase64(sharedPreferences.getString(Functions.share.encodeBase64("ID"), ""));
    }

    public void getUserLogin(OnUserLoginInterface callback) {
        String s = getUserId();
        if(!Functions.isNullOrEmpty(s))
            callback.OnLoaded(getUserId());
        else
            callback.OnLoaded("");
    }

    public interface OnUserLoginInterface {
        void OnLoaded(String userId);
    }
}

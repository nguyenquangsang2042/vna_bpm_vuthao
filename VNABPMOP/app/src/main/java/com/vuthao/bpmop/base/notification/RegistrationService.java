package com.vuthao.bpmop.base.notification;

import android.app.IntentService;
import android.content.Intent;
import android.util.Log;

import androidx.annotation.Nullable;

import com.google.firebase.FirebaseApp;
import com.google.firebase.messaging.FirebaseMessaging;
import com.vuthao.bpmop.base.Constants;

public class RegistrationService extends IntentService {
    public RegistrationService() {
        super("RegistrationService");
    }

    @Override
    protected void onHandleIntent(@Nullable Intent intent) {
        FirebaseApp.initializeApp(this);
        Constants.deviceToken = FirebaseMessaging.getInstance().getToken().toString();
        Log.e(Constants.TAG, "onHandleIntent: " + Constants.deviceToken);
    }
}

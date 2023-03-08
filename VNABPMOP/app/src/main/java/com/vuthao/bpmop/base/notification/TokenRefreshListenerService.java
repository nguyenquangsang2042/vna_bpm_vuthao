package com.vuthao.bpmop.base.notification;

import android.content.Intent;
import android.util.Log;

import androidx.annotation.NonNull;

import com.google.firebase.FirebaseApp;
import com.google.firebase.installations.FirebaseInstallations;
import com.google.firebase.messaging.FirebaseMessaging;
import com.google.firebase.messaging.FirebaseMessagingService;

public class TokenRefreshListenerService extends FirebaseMessagingService {
    @Override
    public void onNewToken(@NonNull String token) {
        FirebaseApp.initializeApp(this);
        String refreshedToken = FirebaseMessaging.getInstance().getToken().toString();
        Log.e("true", "onTokenRefresh: " + refreshedToken);
        Intent i = new Intent(this, RegistrationService.class);
        startService(i);
    }

}

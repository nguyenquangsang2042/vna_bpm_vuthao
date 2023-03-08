package com.vuthao.bpmop.base;

import android.app.Application;
import android.content.Context;
import android.graphics.Typeface;
import android.util.Log;

import androidx.appcompat.app.AppCompatDelegate;

import com.google.firebase.FirebaseApp;

import io.realm.Realm;

public class BPMApplication extends Application implements AppLifeCycleHandler.AppLifeCycleCallback {
    public static Context context;
    private static BPMApplication mInstance;
    private boolean isDatabaseUpdate;
    private boolean isInBackground = false;

    public boolean isDatabaseUpdate() {
        return isDatabaseUpdate;
    }
    public void setDatabaseUpdate(boolean databaseUpdate) {
        isDatabaseUpdate = databaseUpdate;
    }

    @Override
    public void onCreate() {
        super.onCreate();
        AppLifeCycleHandler appLifeCycleHandler = new AppLifeCycleHandler(this);
        registerActivityLifecycleCallbacks(appLifeCycleHandler);
        registerComponentCallbacks(appLifeCycleHandler);

        mInstance = this;
        AppCompatDelegate.setCompatVectorFromResourcesEnabled(true);

        context = getApplicationContext();

        FirebaseApp.initializeApp(this);
        Realm.init(this);
    }

    public static synchronized BPMApplication getInstance() {
        return mInstance;
    }

    @Override
    public void onAppBackground() {
        Log.d("LifecycleEvent", "onAppBackground");
    }

    @Override
    public void onAppForeground() {
        Log.d("LifecycleEvent", "onAppForeground");
    }
}

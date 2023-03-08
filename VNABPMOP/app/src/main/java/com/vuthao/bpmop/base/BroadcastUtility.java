package com.vuthao.bpmop.base;

import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;

import androidx.localbroadcastmanager.content.LocalBroadcastManager;

public class BroadcastUtility {
    public static LocalBroadcastManager manager(Context context) {
        return LocalBroadcastManager.getInstance(context);
    }

    public static void register(Activity activity, BroadcastReceiver receiver, String intentName) {
        manager(activity).registerReceiver(receiver, new IntentFilter(intentName));
    }

    public static void unregister(Activity activity, BroadcastReceiver receiver) {
        manager(activity).unregisterReceiver(receiver);
    }

    public static void send(Activity activity, Intent intent) {
        manager(activity).sendBroadcast(intent);
    }
}

package com.vuthao.bpmop.base;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;

public class NetworkChangeReceiver extends BroadcastReceiver {
    private ReceiverCallback callback;
    private boolean isShow;

    public NetworkChangeReceiver(ReceiverCallback callback) {
        this.callback = callback;
    }

    @Override
    public void onReceive(Context context, Intent intent) {
        int status = NetworkUtil.getConnectivityStatus(context);
        if (intent.getAction().contains("android.net.conn.CONNECTIVITY_CHANGE")) {
            if (status == NetworkUtil.NETWORK_STATUS_NOT_CONNECTED && !isShow) {
                isShow = true;
                callback.OnShowInfoNetWork(isShow);
            } else {
                isShow = false;
                callback.OnShowInfoNetWork(isShow);
            }
        }
    }

    public interface ReceiverCallback {
        void OnShowInfoNetWork(boolean b);
    }
}

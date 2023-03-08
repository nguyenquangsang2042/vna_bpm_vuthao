package com.vuthao.bpmop.base.notification;

import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.media.RingtoneManager;
import android.net.Uri;

import androidx.annotation.NonNull;
import androidx.core.app.NotificationCompat;

import com.google.firebase.messaging.FirebaseMessagingService;
import com.google.firebase.messaging.RemoteMessage;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.activity.SplashActivity;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.detail.DetailWorkflowActivity;
import com.vuthao.bpmop.detail.presenter.DetailWorkflowPresenter;
import com.vuthao.bpmop.task.DetailCreateTaskActivity;

import java.util.Map;

public class NotificationsListenerService extends FirebaseMessagingService implements DetailWorkflowPresenter.PushNotificationListener {
    private static final String TAG = "NotificationsService";
    private final DetailWorkflowPresenter presenter = new DetailWorkflowPresenter(this);
    private Map<String, String> data;
    @Override
    public void onMessageReceived(RemoteMessage remoteMessage) {
        data = remoteMessage.getData();
        if (BaseActivity.sBaseActivity != null) {
            String workflowItemId = String.valueOf(data.get("ResourceId"));
            presenter.getAppBaseItem(workflowItemId);
        } else {
            makeIconNotification(data);
        }
    }

    private void makeIconNotification(Map<String, String> data) {
        String message = data.get("NotifyContent");
        String workflowItemId = String.valueOf(data.get("ResourceId"));

        Intent intent = new Intent(this, SplashActivity.class);;
        intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP | Intent.FLAG_ACTIVITY_SINGLE_TOP);
        intent.putExtra("WorkflowItemId", workflowItemId);

        PendingIntent pendingIntent;
        if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.S) {
            pendingIntent = PendingIntent.getActivity
                    (this, 0, intent, PendingIntent.FLAG_MUTABLE);
        } else {
            pendingIntent = PendingIntent.getActivity
                    (this, 0, intent, PendingIntent.FLAG_UPDATE_CURRENT);
        }

        Uri defaultSoundUri = RingtoneManager.getDefaultUri(RingtoneManager.TYPE_NOTIFICATION);

        String channelId = getString(R.string.app_name);
        NotificationCompat.Builder notificationBuilder =
                new NotificationCompat.Builder(this, channelId)
                        .setSmallIcon(R.drawable.logo_ver2)
                        .setContentTitle(getString(R.string.app_name))
                        .setContentText(message)
                        .setAutoCancel(true)
                        .setSound(defaultSoundUri)
                        .setContentIntent(pendingIntent);

        notificationBuilder.setVisibility(NotificationCompat.VISIBILITY_PUBLIC);
        notificationBuilder.setPriority(NotificationCompat.PRIORITY_HIGH);

        NotificationManager notificationManager =
                (NotificationManager) getSystemService(Context.NOTIFICATION_SERVICE);

        // Since android Oreo notification channel is needed.
        NotificationChannel channel = new NotificationChannel(channelId, "Testing_Audio", NotificationManager.IMPORTANCE_HIGH);
        notificationManager.createNotificationChannel(channel);
        notificationManager.notify((int)System.currentTimeMillis(), notificationBuilder.build());
    }

    private void makeIconNotification(Map<String, String> data, AppBase appBase) {
        String message = data.get("NotifyContent");
        String workflowItemId = String.valueOf(data.get("ResourceId"));

        Intent intent = null;
        if (appBase.getResourceCategoryId() == 16) {
            intent = new Intent(this, DetailCreateTaskActivity.class);
            intent.putExtra("WorkflowItemId", Functions.share.getWorkflowItemIDByUrl(appBase.getItemUrl()));
            intent.putExtra("isClickFromAction", false);
            intent.putExtra("taskId", appBase.getID());
        } else {
            intent = new Intent(this, DetailWorkflowActivity.class);
            intent.putExtra("WorkflowItemId", workflowItemId);
            intent.putExtra("IsFromPushNotification", true);
        }

        intent.setAction(VarsReceiver.PUSHNOTIFICATION);
        BroadcastUtility.send(BaseActivity.sBaseActivity, intent);

        intent.putExtra("WorkflowItemId", workflowItemId);
        intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP | Intent.FLAG_ACTIVITY_SINGLE_TOP);

        PendingIntent pendingIntent;
        if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.S) {
            pendingIntent = PendingIntent.getActivity
                    (this, 0, intent, PendingIntent.FLAG_MUTABLE);
        } else {
            pendingIntent = PendingIntent.getActivity
                    (this, 0, intent, PendingIntent.FLAG_UPDATE_CURRENT);
        }

        Uri defaultSoundUri = RingtoneManager.getDefaultUri(RingtoneManager.TYPE_NOTIFICATION);

        String channelId = getString(R.string.app_name);
        NotificationCompat.Builder notificationBuilder =
                new NotificationCompat.Builder(this, channelId)
                        .setSmallIcon(R.drawable.logo_ver2)
                        .setContentTitle(getString(R.string.app_name))
                        .setContentText(message)
                        .setAutoCancel(true)
                        .setSound(defaultSoundUri)
                        .setContentIntent(pendingIntent);

        notificationBuilder.setVisibility(NotificationCompat.VISIBILITY_PUBLIC);
        notificationBuilder.setPriority(NotificationCompat.PRIORITY_HIGH);

        NotificationManager notificationManager =
                (NotificationManager) getSystemService(Context.NOTIFICATION_SERVICE);

        // Since android Oreo notification channel is needed.
        NotificationChannel channel = new NotificationChannel(channelId, "Testing_Audio", NotificationManager.IMPORTANCE_HIGH);
        notificationManager.createNotificationChannel(channel);
        notificationManager.notify((int)System.currentTimeMillis(), notificationBuilder.build());
    }

    @Override
    public void onNewToken(@NonNull String token) {
        super.onNewToken(token);
        Constants.deviceToken = token;
    }

    @Override
    public void OnGetItemSuccess(AppBase appBase) {
        makeIconNotification(data, appBase);
    }

    @Override
    public void OnGetItemError(String err) {

    }
}

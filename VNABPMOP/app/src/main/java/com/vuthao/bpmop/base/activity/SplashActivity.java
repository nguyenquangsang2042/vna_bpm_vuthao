package com.vuthao.bpmop.base.activity;

import android.Manifest;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.util.Log;

import androidx.core.content.ContextCompat;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.activity.StartActivity;
import com.vuthao.bpmop.activity.TabsActivity;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.realm.RealmHelper;
import com.vuthao.bpmop.detail.DetailWorkflowActivity;

public class SplashActivity extends BaseActivity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_splash);

        if (getIntent() != null) {
            onNewIntent(getIntent());
        }

        new Handler().postDelayed(() -> {
            Intent intent = new Intent(SplashActivity.this, StartActivity.class);
            startActivity(intent);
            finish();
        }, 1000);
    }

    @Override
    protected void onNewIntent(Intent intent) {
        super.onNewIntent(intent);
        Bundle bundle = getIntent().getExtras();
        if (bundle != null) {
            Constants.mResourceId = bundle.getString("WorkflowItemId");
        }
    }
}
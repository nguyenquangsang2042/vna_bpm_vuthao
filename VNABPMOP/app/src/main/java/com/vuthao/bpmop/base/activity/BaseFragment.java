package com.vuthao.bpmop.base.activity;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.graphics.Typeface;
import android.os.Bundle;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.annotation.Nullable;
import androidx.core.content.ContextCompat;
import androidx.core.content.res.ResourcesCompat;
import androidx.fragment.app.Fragment;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.vars.VarsReceiver;

public class BaseFragment extends Fragment {
    protected BaseActivity sBaseActivity;

    private final BroadcastReceiver mFollow = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            onBroadcastReceived(intent);
        }
    };

    private final BroadcastReceiver mChangeLanaguage = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            onBroadcastReceived(intent);
        }
    };

    private final BroadcastReceiver mCreateTask = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            onBroadcastReceived(intent);
        }
    };

    // Refresh khi submit action trong Task hoac Van ban
    private final BroadcastReceiver mRefreshAfterSubmitAction = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            onBroadcastReceived(intent);
        }
    };

    // Refresh click NavigationBottom tu LeftMenu
    private final BroadcastReceiver mRefreshNavigation = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            onBroadcastReceived(intent);
        }
    };

    // Refresh list khi co push notification
    private final BroadcastReceiver mPushNotification = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            onBroadcastReceived(intent);
        }
    };

    @Override
    public void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        sBaseActivity = (BaseActivity) requireActivity();

        BroadcastUtility.register(requireActivity(), mFollow, VarsReceiver.FOLLOW);
        BroadcastUtility.register(requireActivity(), mChangeLanaguage, VarsReceiver.CHANGELANGUAGE);
        BroadcastUtility.register(requireActivity(), mCreateTask, VarsReceiver.CREATE_TASK);
        BroadcastUtility.register(requireActivity(), mRefreshAfterSubmitAction, VarsReceiver.REFRESHAFTERSUBMITACTION);
        BroadcastUtility.register(requireActivity(), mRefreshAfterSubmitAction, VarsReceiver.REFRESHNAVIGATIONBOTTOM);
        BroadcastUtility.register(requireActivity(), mPushNotification, VarsReceiver.PUSHNOTIFICATION);
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        BroadcastUtility.unregister(requireActivity(), mFollow);
        BroadcastUtility.unregister(requireActivity(), mChangeLanaguage);
        BroadcastUtility.unregister(requireActivity(), mCreateTask);
        BroadcastUtility.unregister(requireActivity(), mRefreshAfterSubmitAction);
        BroadcastUtility.unregister(requireActivity(), mRefreshNavigation);
        BroadcastUtility.unregister(requireActivity(), mPushNotification);
    }

    public void setViewSelected(TextView tv, ImageView img, TextView tvCount) {
        if (tv != null && img != null) {
            tv.setTextColor(ContextCompat.getColor(sBaseActivity, R.color.clVer2BlueMain));
            tv.setTypeface(tv.getTypeface(), Typeface.BOLD);
            img.setColorFilter(ContextCompat.getColor(sBaseActivity, R.color.clBottomEnable));
            if (tvCount != null) {
                tvCount.setTextColor(ContextCompat.getColor(sBaseActivity, R.color.clOrangeFilter));
                tvCount.setTypeface(tvCount.getTypeface(), Typeface.NORMAL);
            }
        }
    }

    public void setViewSelected(TextView tv) {
        if (tv != null) {
            tv.setBackgroundResource(R.drawable.drawable_tabselected);
            tv.setTextColor(ContextCompat.getColor(sBaseActivity, R.color.clVer2BlueMain));
            tv.setTypeface(ResourcesCompat.getFont(sBaseActivity, R.font.fontarial), Typeface.BOLD);
        }
    }

    public void setViewUnSelected(TextView tv) {
        if (tv != null) {
            tv.setBackgroundResource(R.drawable.drawable_tabnotselected);
            tv.setTextColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
            tv.setTypeface(ResourcesCompat.getFont(sBaseActivity, R.font.fontarial), Typeface.NORMAL);
        }
    }

    public void setViewUnSelected(TextView tv, ImageView img, TextView tvCount) {
        if (tv != null && img != null) {
            tv.setTextColor(ContextCompat.getColor(sBaseActivity, R.color.clBlack));
            tv.setTypeface(null, Typeface.NORMAL);
            img.setColorFilter(ContextCompat.getColor(sBaseActivity, R.color.clBlack));
            if (tvCount != null) {
                tvCount.setTextColor(ContextCompat.getColor(sBaseActivity, R.color.clVer2BlueMain));
                tvCount.setTypeface(null, Typeface.NORMAL);
            }
        }
    }

    protected void onBroadcastReceived(Intent intent) {
        // Actions common to all activities extending this one can be done here in ExampleBase
    }
}

package com.vuthao.bpmop.base.activity;

import android.app.Dialog;
import android.content.Context;
import android.content.IntentFilter;
import android.content.pm.PackageManager;
import android.graphics.Rect;
import android.net.ConnectivityManager;
import android.os.Build;
import android.os.Bundle;
import android.util.Log;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.inputmethod.InputMethodManager;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.annotation.RequiresApi;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.ActivityCompat;
import androidx.drawerlayout.widget.DrawerLayout;
import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentManager;
import androidx.fragment.app.FragmentTransaction;
import androidx.viewpager2.widget.ViewPager2;

import com.google.android.material.bottomnavigation.BottomNavigationView;
import com.google.android.material.navigation.NavigationView;
import com.google.android.material.tabs.TabLayout;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.NetworkChangeReceiver;
import com.vuthao.bpmop.base.RefreshCookie;
import com.vuthao.bpmop.base.SharedPreferencesController;
import com.vuthao.bpmop.base.BPMApplication;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.custom.DialogNoConnection;
import com.vuthao.bpmop.base.keyboard.KeyboardManager;
import com.vuthao.bpmop.leftmenu.LeftMenuFragment;

public abstract class BaseActivity extends AppCompatActivity implements NetworkChangeReceiver.ReceiverCallback {
    public static BaseActivity sBaseActivity;
    private DrawerLayout mDrawerLayout;
    private NavigationView mNavigationView;
    private LeftMenuFragment objLeftMenu;
    private Toast mToast;
    private Dialog mProgressDialog;
    private static String token;

    private TabLayout tabLayout;
    private ViewPager2 viewPager;

    private SharedPreferencesController preferencesController;
    private NetworkChangeReceiver broadcastReceiver;
    private DialogNoConnection dialogNoConnection;
    private PermissionListener permissionListener;
    private static final int PERMISSIONS_REQUEST = 2251;

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        sBaseActivity = this;

        preferencesController = new SharedPreferencesController(this);
    }

    @Override
        public boolean dispatchTouchEvent(MotionEvent event) {
        if (event.getAction() == MotionEvent.ACTION_DOWN) {
            View v = getCurrentFocus();
            if (v instanceof EditText) {
                Rect outRect = new Rect();
                v.getGlobalVisibleRect(outRect);
                if (!outRect.contains((int) event.getRawX(), (int) event.getRawY())) {
                    v.clearFocus();
                    InputMethodManager imm = (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);
                    assert imm != null;
                    imm.hideSoftInputFromWindow(v.getWindowToken(), 0);
                }
            }
        }
        return super.dispatchTouchEvent(event);
    }

    public void hideBottomBar() {
        viewPager.setUserInputEnabled(false);
        viewPager.setVisibility(View.GONE);
        tabLayout.setVisibility(View.GONE);
    }

    public void showBottomBar() {
        viewPager.setUserInputEnabled(false);
        viewPager.setVisibility(View.VISIBLE);
        tabLayout.setVisibility(View.VISIBLE);
    }

    public void showFragment(int containerViewId, FragmentManager frgManager, Fragment frgToShow, String frgTag, @Nullable boolean animate) {
        FragmentTransaction frgTransaction;
        frgTransaction = frgManager.beginTransaction();
        if (animate)
            frgTransaction.setCustomAnimations(R.anim.fragment_enter, R.anim.exit_to_left, R.anim.enter_from_right, R.anim.exit_to_right);
        else
            frgTransaction.setCustomAnimations(R.animator.fade_in, R.animator.fade_out);
        frgTransaction.replace(containerViewId, frgToShow, frgTag);
        frgTransaction.addToBackStack(frgTag);
        frgTransaction.setTransition(FragmentTransaction.TRANSIT_FRAGMENT_FADE);
        frgTransaction.commit();
    }

    public void backFragment(@Nullable String name) {
        KeyboardManager.hideKeyboard(this);
        assert name != null;
        if (!name.isEmpty()) {
            String frg = getCurrentFragment();
            if (!Functions.isNullOrEmpty(frg)) {
                getSupportFragmentManager().popBackStack(name, FragmentManager.POP_BACK_STACK_INCLUSIVE);
            }
        } else {
            getSupportFragmentManager().popBackStack();
        }
    }

    public String getCurrentFragment() {
        return getSupportFragmentManager().getBackStackEntryAt(getSupportFragmentManager().getBackStackEntryCount() - 1).getName();
    }

    public void checkUserPermission(PermissionListener permissionListener, String[] permissions) {
        this.permissionListener = permissionListener;
        // >= android 6.0
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
            ActivityCompat.requestPermissions(
                    this,
                    permissions,
                    PERMISSIONS_REQUEST);
        } else {
            if (permissionListener != null) permissionListener.OnAcceptedAllPermission();
        }
    }

    public void openLeftMenu() {
        if (mDrawerLayout != null) {
            mDrawerLayout.openDrawer(mNavigationView);
        }
    }

    public void closeLeftMenu() {
        if (mDrawerLayout != null && mDrawerLayout.isDrawerOpen(mNavigationView)) {
            mDrawerLayout.closeDrawer(mNavigationView);
        }
    }

    @RequiresApi(api = Build.VERSION_CODES.M)
    @Override
    public void onRequestPermissionsResult(int requestCode, @NonNull String[] permissions, @NonNull int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);
        switch (requestCode) {
            case PERMISSIONS_REQUEST: {
                if (grantResults.length > 0) {
                    for (int i = 0; i < grantResults.length; i++) {
                        if (!shouldShowRequestPermissionRationale(permissions[i]) && grantResults[i] != PackageManager.PERMISSION_GRANTED) {
                            if (this.permissionListener != null) {
                                //permissionListener.OnNeverRequestPermission();
                                permissionListener.OnAcceptedAllPermission();
                                return;
                            }
                        } else {
                            if (grantResults[i] != PackageManager.PERMISSION_GRANTED) {
                                if (this.permissionListener != null) {
                                    permissionListener.OnCancelPermission();
                                }
                                return;
                            }
                        }

                    }

                    if (this.permissionListener != null) {
                        permissionListener.OnAcceptedAllPermission();
                    }
                }
            }
        }
    }

    public void showToast(String str, @Nullable ToastClickListener listener) {
        if (mToast == null) {
            mToast = new Toast(this);
            mToast.setGravity(Gravity.BOTTOM, 0, 0);
            mToast.setDuration(Toast.LENGTH_LONG);
        }

        LayoutInflater inflater = getLayoutInflater();
        View layout = inflater.inflate(R.layout.toast_layout,
                findViewById(R.id.container), false);
        TextView text = layout.findViewById(R.id.tvTitle);
        text.setText(str);
        layout.setOnClickListener(v -> {
            if (listener != null) listener.OnToastClicked();
        });
        mToast.setDuration(Toast.LENGTH_LONG);
        mToast.setView(layout);
        mToast.show();

    }

    public void showToast(String str) {
        showToast(str, null);
    }

    public void showProgressDialog() {
        if (mProgressDialog == null) {
            mProgressDialog = new Dialog(this, R.style.AlertDialogCustom);
            mProgressDialog.setContentView(R.layout.layout_progress_dialog);
            mProgressDialog.setCancelable(false);
        }
        try {
            if (!mProgressDialog.isShowing()) {
                mProgressDialog.show();
            }
        } catch (Exception ignored) {
            Log.d("ERR showProgressDialog", ignored.getMessage());
        }
    }

    public void hideProgressDialog() {
        if (mProgressDialog != null && mProgressDialog.isShowing()) {
            try {
                mProgressDialog.dismiss();
            } catch (Exception ignored) {
                Log.d("ERR hideProgressDialog", ignored.getMessage());
            }
        }
    }

    public static String getToken() {
        if (token == null)
            token = new SharedPreferencesController(BPMApplication.context).getUserToken();
        return token;
    }

    public static void setToken(String toke) {
        token = toke;
    }

    public DrawerLayout getmDrawerLayout() {
        return mDrawerLayout;
    }

    public void setmDrawerLayout(DrawerLayout mDrawerLayout) {
        this.mDrawerLayout = mDrawerLayout;
    }

    public NavigationView getmNavigationView() {
        return mNavigationView;
    }

    public void setmNavigationView(NavigationView mNavigationView) {
        this.mNavigationView = mNavigationView;
    }

    public LeftMenuFragment getObjLeftMenu() {
        return objLeftMenu;
    }

    public void setObjLeftMenu(LeftMenuFragment objLeftMenu) {
        this.objLeftMenu = objLeftMenu;
    }

    public TabLayout getTabLayout() {
        return tabLayout;
    }

    public void setTabLayout(TabLayout tabLayout) {
        this.tabLayout = tabLayout;
    }

    public ViewPager2 getViewPager() {
        return viewPager;
    }

    public void setViewPager(ViewPager2 viewPager) {
        this.viewPager = viewPager;
    }

    public SharedPreferencesController getPreferencesController() {
        return preferencesController;
    }

    public void setPreferencesController(SharedPreferencesController preferencesController) {
        this.preferencesController = preferencesController;
    }

    @Override
    protected void onResume() {
        super.onResume();
        if (broadcastReceiver == null) {
            broadcastReceiver = new NetworkChangeReceiver(this);
            IntentFilter filter = new IntentFilter();
            filter.addAction(ConnectivityManager.CONNECTIVITY_ACTION);
            this.registerReceiver(broadcastReceiver, filter);
        }
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        if (broadcastReceiver != null) unregisterReceiver(broadcastReceiver);
    }

    @Override
    public void OnShowInfoNetWork(boolean b) {
        if (dialogNoConnection == null)
            dialogNoConnection = new DialogNoConnection(BaseActivity.this);
        if (b && !dialogNoConnection.isShowing()) dialogNoConnection.show();
        else dialogNoConnection.cancel();
    }

    public interface ToastClickListener {
        void OnToastClicked();
    }

    public interface PermissionListener {
        void OnAcceptedAllPermission();

        void OnCancelPermission();

        void OnNeverRequestPermission();
    }
}

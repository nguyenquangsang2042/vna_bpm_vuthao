package com.vuthao.bpmop.base;

import android.app.Activity;
import android.app.AlertDialog;
import android.app.Dialog;
import android.content.Context;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.graphics.Color;
import android.graphics.Paint;
import android.graphics.Rect;
import android.graphics.drawable.ColorDrawable;
import android.os.Build;
import android.view.View;
import android.view.inputmethod.InputMethodManager;
import android.widget.EditText;
import android.widget.TextView;

import java.text.Normalizer;
import java.util.Objects;
import java.util.regex.Pattern;

import androidx.annotation.NonNull;
import androidx.core.widget.NestedScrollView;
import androidx.swiperefreshlayout.widget.SwipeRefreshLayout;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.realm.RealmController;

public class Utility {
    public static Utility share = new Utility();
    public static View alertView;
    public static Dialog dialog;

    public void showAlertWithOnlyOK(@NonNull String message, @NonNull Activity activity, final OkListener okListener) {
        showAlertWithOKCancel(message, null, activity.getResources().getString(R.string.ok), null, activity, okListener);
    }

    public void showAlertWithOnlyOK(@NonNull String message, @NonNull Activity activity) {
        showAlertWithOKCancel(message, null, activity.getResources().getString(R.string.ok), null, activity, null);
    }

    public void showAlertWithOnlyOK(@NonNull String message, String ok, @NonNull Activity activity) {
        showAlertWithOKCancel(message, null, ok, null, activity, null);
    }

    public void showAlertWithOnlyOK(@NonNull String message, String ok, @NonNull Activity activity, final OkListener okListener) {
        showAlertWithOKCancel(message, "", ok, null, activity, okListener);
    }

    public void showAlertWithOnlyOK(@NonNull String message, String title, String textOk, @NonNull Activity activity, final OkListener okListener) {
        showAlertWithOKCancel(message, title, textOk, null, activity, okListener);
    }

    public void showAlertWithOKCancel(@NonNull String message, @NonNull Activity activity, final OkListener okListener) {
        showAlertWithOKCancel(message, null, activity.getResources().getString(R.string.ok), activity.getResources().getString(R.string.cancel), activity, okListener);
    }

    public void showAlertWithOKCancel(@NonNull String message, String textCancel, String textOk, @NonNull Activity activity, final OkListener okListener) {
        showAlertWithOKCancel(message, null, textOk, textCancel, activity, okListener);
    }

    public void showAlertWithYesNo(@NonNull String message, @NonNull Activity activity, final OkListener okListener) {
        showAlertWithOKCancel(message, null, "Yes", "No", activity, okListener);
    }

    public void showForceUpdateDialog(@NonNull String message, String title, String textOK, String textCancel, @NonNull Activity activity, final OkListener okListener) {
        AlertDialog.Builder alertDialog;
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP) {
            alertDialog = new AlertDialog.Builder(activity, android.R.style.Theme_Material_Light_Dialog_Alert);
        } else {
            alertDialog = new AlertDialog.Builder(activity);
        }

        if (title != null) {
            activity.setTitle(title);
        }
        alertDialog.setCancelable(false);
        alertDialog.setMessage(message);
        alertDialog.setPositiveButton(textOK, (dialog, which) -> {
            if (okListener != null) okListener.onOkListener();
            else dialog.cancel();
        });

        if (textCancel != null)
            alertDialog.setNegativeButton(textCancel, (dialog, which) -> dialog.cancel());

        alertDialog.show();
    }

    public void showAlertWithOKCancel(@NonNull String message, String title, String textOK, String textCancel, @NonNull Activity activity, final OkListener okListener) {
        AlertDialog.Builder alertDialog;
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP) {
            alertDialog = new AlertDialog.Builder(activity, android.R.style.Theme_Material_Light_Dialog_Alert);
        } else {
            alertDialog = new AlertDialog.Builder(activity);
        }

        if (title != null) {
            activity.setTitle(title);
        }
        alertDialog.setMessage(message);
        alertDialog.setPositiveButton(textOK, (dialog, which) -> {
            if (okListener != null) okListener.onOkListener();
            else dialog.cancel();
        });

        if (textCancel != null)
            alertDialog.setNegativeButton(textCancel, (dialog, which) -> dialog.cancel());

        alertDialog.setCancelable(false);
        alertDialog.show();
    }

    public void showAlertWithOKCancel(String message, String textOK, String textCancel, Activity activity, final OkListener okListener, final CancelListener cancelListener) {
        AlertDialog.Builder alertDialog;
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP) {
            alertDialog = new AlertDialog.Builder(activity, android.R.style.Theme_Material_Light_Dialog_Alert);
        } else {
            alertDialog = new AlertDialog.Builder(activity);
        }
        alertDialog.setCancelable(false);
        alertDialog.setMessage(message);
        alertDialog.setPositiveButton(textOK, (dialog, which) -> {
            if (okListener != null) okListener.onOkListener();
            else dialog.cancel();
        });
        if (textCancel != null)
            alertDialog.setNegativeButton(textCancel, (dialog, which) -> {
                if (cancelListener != null)
                    cancelListener.onCancelListener();
                else
                    dialog.cancel();
            });
        alertDialog.show();
    }

    public void showAlertWithOnlyOK(Activity activity, String mess, String title, String negative, final OkListener okListener) {
        if (alertView == null) {
            alertView = activity.getLayoutInflater().inflate(R.layout.popup_custom_alertdialog, null);
            dialog = new Dialog(activity);
            dialog.getWindow().setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));
            dialog.setCancelable(false);
            dialog.setContentView(alertView);
        }

        TextView tv_PopupCustomAlertDialog_Title = alertView.findViewById(R.id.tv_PopupCustomAlertDialog_Title);
        TextView tv_PopupCustomAlertDialog_Message = alertView.findViewById(R.id.tv_PopupCustomAlertDialog_Message);
        TextView tv_PopupCustomAlertDialog_Positive = alertView.findViewById(R.id.tv_PopupCustomAlertDialog_Positive);
        TextView tv_PopupCustomAlertDialog_Negative = alertView.findViewById(R.id.tv_PopupCustomAlertDialog_Negative);
        View vw_PopupCustomAlertDialog_Divider = alertView.findViewById(R.id.vw_PopupCustomAlertDialog_Divider);

        tv_PopupCustomAlertDialog_Title.setText(!Functions.isNullOrEmpty(title) ? title : "Alert");
        tv_PopupCustomAlertDialog_Message.setText(!Functions.isNullOrEmpty(mess) ? mess : "");
        tv_PopupCustomAlertDialog_Positive.setText(!Functions.isNullOrEmpty(negative) ? negative : "Close");

        tv_PopupCustomAlertDialog_Negative.setVisibility(View.GONE);
        vw_PopupCustomAlertDialog_Divider.setVisibility(View.GONE);

        tv_PopupCustomAlertDialog_Positive.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (okListener != null)
                    okListener.onOkListener();
                else dialog.cancel();
            }
        });

        if (!activity.isFinishing()) {
            dialog.show();
        }
    }

    public void showAlertWithOKCancle(Activity activity, String mess, String title, String negative, final OkListener okListener, final CancelListener cancelListener) {
        if (alertView == null) {
            alertView = activity.getLayoutInflater().inflate(R.layout.popup_custom_alertdialog, null);
            dialog = new Dialog(activity);
            dialog.getWindow().setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));
            dialog.setCancelable(false);
            dialog.setContentView(alertView);
        }

        TextView tv_PopupCustomAlertDialog_Title = alertView.findViewById(R.id.tv_PopupCustomAlertDialog_Title);
        TextView tv_PopupCustomAlertDialog_Message = alertView.findViewById(R.id.tv_PopupCustomAlertDialog_Message);
        TextView tv_PopupCustomAlertDialog_Positive = alertView.findViewById(R.id.tv_PopupCustomAlertDialog_Positive);
        TextView tv_PopupCustomAlertDialog_Negative = alertView.findViewById(R.id.tv_PopupCustomAlertDialog_Negative);
        View vw_PopupCustomAlertDialog_Divider = alertView.findViewById(R.id.vw_PopupCustomAlertDialog_Divider);

        tv_PopupCustomAlertDialog_Title.setText(!Functions.isNullOrEmpty(title) ? title : "Alert");
        tv_PopupCustomAlertDialog_Message.setText(!Functions.isNullOrEmpty(mess) ? mess : "");
        tv_PopupCustomAlertDialog_Negative.setText(!Functions.isNullOrEmpty(negative) ? negative : "Close");

        vw_PopupCustomAlertDialog_Divider.setVisibility(View.GONE);

        tv_PopupCustomAlertDialog_Positive.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (okListener != null)
                    okListener.onOkListener();
                else dialog.cancel();
            }
        });

        tv_PopupCustomAlertDialog_Negative.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (cancelListener != null)
                    cancelListener.onCancelListener();
                else dialog.cancel();
            }
        });

        dialog.show();
    }

    public void showAlertWithOKCancle(Activity activity, String mess, String title, String positive, String nagative, final OkListener okListener, final CancelListener cancelListener) {
        if (alertView == null) {
            alertView = activity.getLayoutInflater().inflate(R.layout.popup_custom_alertdialog, null);
            dialog = new Dialog(activity);
            dialog.getWindow().setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));
            dialog.setCancelable(false);
            dialog.setContentView(alertView);
        }

        TextView tv_PopupCustomAlertDialog_Title = alertView.findViewById(R.id.tv_PopupCustomAlertDialog_Title);
        TextView tv_PopupCustomAlertDialog_Message = alertView.findViewById(R.id.tv_PopupCustomAlertDialog_Message);
        TextView tv_PopupCustomAlertDialog_Positive = alertView.findViewById(R.id.tv_PopupCustomAlertDialog_Positive);
        TextView tv_PopupCustomAlertDialog_Negative = alertView.findViewById(R.id.tv_PopupCustomAlertDialog_Negative);
        View vw_PopupCustomAlertDialog_Divider = alertView.findViewById(R.id.vw_PopupCustomAlertDialog_Divider);

        tv_PopupCustomAlertDialog_Message.setText(mess);
        tv_PopupCustomAlertDialog_Positive.setText(positive);
        tv_PopupCustomAlertDialog_Negative.setText(nagative);

        if (String.valueOf(CurrentUser.getInstance().getUser().getLanguage()).equals(Constants.mLangVN)) {
            tv_PopupCustomAlertDialog_Title.setText(!Functions.isNullOrEmpty(title) ? title : "Thông báo");
        } else {
            tv_PopupCustomAlertDialog_Title.setText(!Functions.isNullOrEmpty(title) ? title : "Alert");
        }

        vw_PopupCustomAlertDialog_Divider.setVisibility(View.GONE);

        tv_PopupCustomAlertDialog_Positive.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (okListener != null)
                    okListener.onOkListener();
                else dialog.cancel();
            }
        });

        tv_PopupCustomAlertDialog_Negative.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (cancelListener != null)
                    cancelListener.onCancelListener();
                else dialog.cancel();
            }
        });

        dialog.show();
    }

    public interface CancelListener {
        void onCancelListener();
    }

    public static int calculateLineCount(String text, TextView textView) {
        Rect bounds = new Rect();
        Paint paint = new Paint();
        paint.setTextSize(textView.getTextSize());
        paint.getTextBounds(text, 0, text.length(), bounds);

        return (int) Math.ceil((float) bounds.width() / textView.getTextSize());
    }

    public void hideKeyboard(Context context, View view) {
        if (view == null) return;
        InputMethodManager inputManager = (InputMethodManager) context.getSystemService(Context.INPUT_METHOD_SERVICE);
        assert inputManager != null;
        inputManager.hideSoftInputFromWindow(view.getWindowToken(), InputMethodManager.HIDE_NOT_ALWAYS);

    }

    public boolean compareVersion(String currentVer, String newVer) {
        if (!currentVer.equals("") && !newVer.equals("")) {
            int current = Integer.valueOf(currentVer.replaceAll("\\.", ""));
            int newVersion = Integer.valueOf(newVer.replaceAll("\\.", ""));
            if (newVersion > current)
                return true;
        }
        return false;
    }

    public void showKeyboard(Context context) {
        InputMethodManager inputManager = (InputMethodManager) context.getSystemService(Context.INPUT_METHOD_SERVICE);
        assert inputManager != null;
        inputManager.toggleSoftInput(InputMethodManager.SHOW_FORCED, InputMethodManager.HIDE_IMPLICIT_ONLY);
    }

    public void onFocusChange(View v, boolean hasFocus, Context context) {
        if (v instanceof EditText) {
            if (!hasFocus) {
                InputMethodManager imm = (InputMethodManager) context.getSystemService(Context.INPUT_METHOD_SERVICE);
            }
        }
    }

    public void setupSwipeRefreshLayout(SwipeRefreshLayout swipe) {
        swipe.setDistanceToTriggerSync(Constants.mSwipeDistance); // in dips
        swipe.setColorSchemeResources(R.color.clVer2BlueMain);
    }

    public void scrollToView(final NestedScrollView scrollView, final View view) {

        scrollView.post(() -> scrollView.scrollTo(0, view.getBottom()));
    }

    public static void avoidDoubleClicks(final View view) {
        final long DELAY_IN_MS = 900;
        if (!view.isClickable()) {
            return;
        }
        view.setClickable(false);
        view.postDelayed(() -> view.setClickable(true), DELAY_IN_MS);
    }

    public String getCurrentVersion(Activity activity) {
        PackageManager pm = activity.getPackageManager();
        PackageInfo pInfo = null;
        try {
            pInfo = pm.getPackageInfo(activity.getPackageName(), 0);

        } catch (PackageManager.NameNotFoundException e1) {
            e1.printStackTrace();
        }
        assert pInfo != null;
        return pInfo.versionName;
    }

    public static String removeAccent(String s) {
        String temp = Normalizer.normalize(s, Normalizer.Form.NFD);
        Pattern pattern = Pattern.compile("\\p{InCombiningDiacriticalMarks}+");
        return pattern.matcher(temp).replaceAll("");
    }

    public void resetData(Context context) {
        // Deleting the important things from this device
        deleteSharePreferences(context);
        deleteImportantRealmData();
    }

    public void deleteSharePreferences(Context context) {
        SharedPreferencesController sharedPreferencesController = new SharedPreferencesController(Objects.requireNonNull(context));
        sharedPreferencesController.saveUserToken("");
        sharedPreferencesController.saveUserId("");
        //sharedPreferencesController.saveUserLogin("", "");
    }

    public void deleteImportantRealmData() {
        new RealmController().getRealm().executeTransaction(realm -> realm.deleteAll());
    }

    public interface OkListener {
        void onOkListener();
    }
}

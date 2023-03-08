package com.vuthao.bpmop.base.custom;

import android.app.Activity;
import android.app.Dialog;
import android.os.Bundle;
import android.view.Gravity;
import android.view.Window;
import android.view.WindowManager;

import androidx.annotation.NonNull;

import com.vuthao.bpmop.R;


public class DialogNoConnection extends Dialog {
    private Activity context;

    public DialogNoConnection(@NonNull Activity context) {
        super(context);
        this.context = context;
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.dialog_no_connection);
        setCancelable(false);

        // Remove opacity behind
        Window window = this.getWindow();
        window.getDecorView().setBackgroundResource(android.R.color.transparent);
        window.setDimAmount(0.0f);

        // Set layout located
        WindowManager.LayoutParams wlp = window.getAttributes();
        wlp.gravity = Gravity.BOTTOM;
        wlp.x = 0;   //x position
        wlp.y = 100;

        window.setAttributes(wlp);

        // Disable touch on this dialog
        window.setFlags(WindowManager.LayoutParams.FLAG_NOT_TOUCHABLE,
                WindowManager.LayoutParams.FLAG_NOT_TOUCHABLE);
        window.clearFlags(WindowManager.LayoutParams.FLAG_DIM_BEHIND);
    }

    @Override
    public void onBackPressed() {
        context.onBackPressed();
    }
}

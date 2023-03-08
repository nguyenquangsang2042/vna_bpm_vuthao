package com.vuthao.bpmop.shareview;

import android.app.Activity;
import android.app.Dialog;
import android.content.Intent;
import android.graphics.Color;
import android.graphics.drawable.ColorDrawable;
import android.util.DisplayMetrics;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.ListView;
import android.widget.TextView;

import androidx.localbroadcastmanager.content.LocalBroadcastManager;

import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.custom.ButtonAction;
import com.vuthao.bpmop.core.adapter.ControlActionMoreAdapter;

import java.util.ArrayList;

public class SharedView_PopupActionMore extends SharedView_PopupActionBase {
    public SharedView_PopupActionMore(LayoutInflater inflater, Activity mainAct, String fragmentTag, View rootView) {
        super(inflater, mainAct, fragmentTag, rootView);
    }

    @Override
    public void initializeValue_DetailWorkflow_ActionMore(ArrayList<ButtonAction> lstActionMore) {
        super.initializeValue_DetailWorkflow_ActionMore(lstActionMore);
    }

    @Override
    public void initializeView() {
        super.initializeView();

        View view = inflater.inflate(R.layout.popup_action_more, null);
        ListView lvAction = view.findViewById(R.id.lv_PopupActionMore);
        TextView tvClose = view.findViewById(R.id.tv_PopupActionMore_Close);

        tvClose.setText(Functions.share.getTitle("TEXT_CLOSE", "Thoát"));

        Dialog dialog = new Dialog(mainAct);
        Window window = dialog.getWindow();
        dialog.requestWindowFeature(1);
        dialog.setCanceledOnTouchOutside(false);
        dialog.setCancelable(true);
        window.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_ADJUST_PAN);
        window.setGravity(Gravity.BOTTOM);
        DisplayMetrics displayMetrics = mainAct.getResources().getDisplayMetrics();
        dialog.setContentView(view);
        dialog.show();
        WindowManager.LayoutParams s = window.getAttributes();
        s.width = displayMetrics.widthPixels;
        s.height = WindowManager.LayoutParams.WRAP_CONTENT;
        window.setAttributes(s);
        window.setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));

        ControlActionMoreAdapter adapter = new ControlActionMoreAdapter(mainAct, lstActionMore);
        lvAction.setAdapter(adapter);
        lvAction.setOnItemClickListener((parent, view1, position, id) -> {
            dialog.dismiss();
            Intent intent = new Intent();
            intent.setAction("INBOTTOMMORE");
            intent.putExtra("action", new Gson().toJson(lstActionMore.get(position)));
            BroadcastUtility.send(mainAct, intent);
        });

        tvClose.setOnClickListener(v -> dialog.dismiss());
    }
}

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
import android.widget.CalendarView;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.annotation.NonNull;

import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.core.Vars;
import com.vuthao.bpmop.detail.custom.DetailFunc;

import org.json.JSONObject;

import java.util.Calendar;

public class SharedView_PopupControlDate extends SharedView_PopupControlBase implements View.OnClickListener, CalendarView.OnDateChangeListener {
    private String result = "";

    private Dialog dialog;
    private View view;
    private ImageView imgDone;
    private ImageView imgClose;
    private TextView tvTitle;
    private CalendarView calendar;
    private LinearLayout lnClear;

    public SharedView_PopupControlDate(LayoutInflater inflater, Activity mainAct, String fragmentTag, View rootView) {
        super(inflater, mainAct, fragmentTag, rootView);
    }

    @Override
    public void initializeValue_Master(ViewElement viewElement) {
        super.initializeValue_Master(viewElement);
    }

    @Override
    public void initializeValue_InputGridDetail(ViewElement elementParent, ViewElement elementPopup, JSONObject JObjectChild) {
        super.initializeValue_InputGridDetail(elementParent, elementPopup, JObjectChild);
    }

    @Override
    public void initializeView() {
        super.initializeView();

        view = inflater.inflate(R.layout.popup_control_date_picker, null);
        imgDone = view.findViewById(R.id.img_PopupControl_DatePicker_Done);
        imgClose = view.findViewById(R.id.img_PopupControl_DatePicker_Close);
        tvTitle = view.findViewById(R.id.tv_PopupControl_DatePicker_Title);
        calendar = view.findViewById(R.id.Calendar_PopupControl_DatePicker);
        lnClear = view.findViewById(R.id.ln_PopupControl_DatePicker_Clear);

        switch (flagView) {
            case Vars.FlagViewControlDynamic.DetailWorkflow:
            default: {
                tvTitle.setText(elementParent.getTitle());

                if (elementParent.isEnable()) {
                    imgDone.setVisibility(View.VISIBLE);
                    lnClear.setVisibility(View.VISIBLE);
                    calendar.setEnabled(true);
                } else {
                    imgDone.setVisibility(View.INVISIBLE);
                    lnClear.setVisibility(View.GONE);
                    calendar.setEnabled(false);
                }

                break;
            }
            case Vars.FlagViewControlDynamic.DetailWorkflow_InputGridDetail: {
                tvTitle.setText(elementPopup.getTitle());

                if (elementPopup.isEnable()) {
                    imgDone.setVisibility(View.VISIBLE);
                    lnClear.setVisibility(View.VISIBLE);
                    calendar.setEnabled(true);
                } else {
                    imgDone.setVisibility(View.INVISIBLE);
                    lnClear.setVisibility(View.GONE);
                    calendar.setEnabled(false);
                }
                break;
            }
        }

        //region Dialog
        dialog = new Dialog(mainAct);
        Window window = dialog.getWindow();
        dialog.requestWindowFeature(1);
        dialog.setCanceledOnTouchOutside(false);
        dialog.setCancelable(true);
        window.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_ADJUST_PAN);
        window.setGravity(Gravity.CENTER);
        DisplayMetrics displayMetrics = mainAct.getResources().getDisplayMetrics();
        dialog.setContentView(view);
        dialog.show();

        WindowManager.LayoutParams s = window.getAttributes();
        s.width = displayMetrics.widthPixels;
        s.height = WindowManager.LayoutParams.WRAP_CONTENT;
        window.setAttributes(s);
        window.setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));
        //endregion

        imgClose.setOnClickListener(this);
        calendar.setOnDateChangeListener(this);
        imgDone.setOnClickListener(this);
        lnClear.setOnClickListener(this);
    }

    private void clear() {
        switch (flagView) {
            case Vars.FlagViewControlDynamic.DetailWorkflow: {
                Intent intent = new Intent();
                intent.setAction("UPDATEFORM");
                intent.putExtra("element", new Gson().toJson(elementParent));
                intent.putExtra("newValue", "");
                BroadcastUtility.send(mainAct, intent);
                break;
            }
            case Vars.FlagViewControlDynamic.DetailWorkflow_InputGridDetail: {
                Intent intent = new Intent();
                intent.setAction("UPDATECHILDACTION");
                intent.putExtra("elementParent", new Gson().toJson(elementParent));
                intent.putExtra("elementChild", new Gson().toJson(elementPopup));
                intent.putExtra("json", JObjectChild.toString());
                intent.putExtra("newValue", "");
                BroadcastUtility.send(mainAct, intent);
                break;
            }
        }

        dialog.dismiss();
    }

    private void done() {
        switch (flagView) {
            case Vars.FlagViewControlDynamic.DetailWorkflow: {
                Intent intent = new Intent();
                intent.setAction("UPDATEFORM");
                intent.putExtra("element", new Gson().toJson(elementParent));
                intent.putExtra("newValue", result);
                BroadcastUtility.send(mainAct, intent);
                break;
            }
            case Vars.FlagViewControlDynamic.DetailWorkflow_InputGridDetail: {
                Intent intent = new Intent();
                intent.setAction("UPDATECHILDACTION");
                intent.putExtra("elementParent", new Gson().toJson(elementParent));
                intent.putExtra("elementChild", new Gson().toJson(elementPopup));
                intent.putExtra("json", JObjectChild.toString());
                intent.putExtra("newValue", result);
                BroadcastUtility.send(mainAct, intent);
                break;
            }
        }

        dialog.dismiss();
    }

    @Override
    public void onSelectedDayChange(@NonNull CalendarView calendarView, int year, int month, int dayOfMonth) {
        Calendar c = Calendar.getInstance();
        c.set(year, month, dayOfMonth);
        long l = c.getTimeInMillis();
        if (flagView == Vars.FlagViewControlDynamic.DetailWorkflow) {
            result = DetailFunc.share.formatDateTimeWhenSelectedPopup(l, "yyyy-MM-dd HH:mm:ss");
        } else {
            result = DetailFunc.share.formatDateTimeWhenSelectedPopup(l, Constants.mDateApi);
        }
    }

    @Override
    public void onClick(View view) {
        switch (view.getId()) {
            case R.id.img_PopupControl_DatePicker_Close: {
                dialog.dismiss();
                break;
            }
            case R.id.img_PopupControl_DatePicker_Done: {
                done();
                break;
            }
            case R.id.ln_PopupControl_DatePicker_Clear: {
                clear();
                break;
            }
        }
    }
}

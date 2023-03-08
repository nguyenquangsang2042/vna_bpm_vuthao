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
import android.widget.DatePicker;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.TimePicker;

import androidx.localbroadcastmanager.content.LocalBroadcastManager;

import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.CalendarUltis;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.core.Vars;
import com.vuthao.bpmop.detail.custom.DetailFunc;

import org.json.JSONObject;

import java.util.Calendar;

public class SharedView_PopupControlDateTime extends SharedView_PopupControlBase implements View.OnClickListener {
    private Dialog dialog;
    private View view;
    private DatePicker datePicker;
    private TimePicker timePicker;
    private ImageView imgDelete;
    private ImageView imgToday;
    private ImageView imgClose;
    private TextView tvTitle;
    private LinearLayout lnApply;
    private TextView tvApply;

    public SharedView_PopupControlDateTime(LayoutInflater inflater, Activity mainAct, String fragmentTag, View rootView) {
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

        view = inflater.inflate(R.layout.popup_control_datetime_picker, null);
        datePicker = view.findViewById(R.id.dp_PopupControl_DateTimePicker);
        timePicker = view.findViewById(R.id.tp_PopupControl_DateTimePicker);
        imgDelete = view.findViewById(R.id.img_PopupControl_DateTimePicker_Delete);
        imgToday = view.findViewById(R.id.img_PopupControl_DateTimePicker_Today);
        imgClose = view.findViewById(R.id.img_PopupControl_DateTimePicker_Close);
        tvTitle = view.findViewById(R.id.tv_PopupControl_DateTimePicker_Title);
        lnApply = view.findViewById(R.id.ln_PopupControl_DateTimePicker_Clear);
        tvApply = view.findViewById(R.id.tv_PopupControl_DateTimePicker_Apply);

        timePicker.setIs24HourView(Boolean.TRUE);
        tvApply.setText(Functions.share.getTitle("TEXT_APPLY", "Áp dụng"));
        CalendarUltis calendar = null;
        switch (flagView) {
            case Vars.FlagViewControlDynamic.DetailWorkflow:
            default: {
                if (!Functions.isNullOrEmpty(elementParent.getValue())) {
                    calendar = new CalendarUltis(elementParent.getValue());
                    tvTitle.setText(DetailFunc.share.formatDateTimePopup(elementParent.getValue()));
                } else {
                    calendar = new CalendarUltis();
                    tvTitle.setText(Functions.share.formatLongToString(calendar.getTimeInMillis(), "dd/MM/yyyy"));
                }

                datePicker.init(calendar.YEAR, calendar.MONTH, calendar.DAY_OF_MONTH, null);
                timePicker.setHour(calendar.HOUR);
                timePicker.setMinute(calendar.MINUTE);

                if (elementParent.isEnable()) {
                    imgDelete.setVisibility(View.VISIBLE);
                    imgToday.setVisibility(View.VISIBLE);
                    tvApply.setVisibility(View.VISIBLE);
                    lnApply.setVisibility(View.VISIBLE);
                    datePicker.setEnabled(true);
                    timePicker.setEnabled(true);
                } else {
                    imgDelete.setVisibility(View.INVISIBLE);
                    imgToday.setVisibility(View.INVISIBLE);
                    tvApply.setVisibility(View.GONE);
                    lnApply.setVisibility(View.GONE);
                    datePicker.setEnabled(false);
                    timePicker.setEnabled(false);
                }
                break;
            }
            case Vars.FlagViewControlDynamic.DetailWorkflow_InputGridDetail: {
                if (!Functions.isNullOrEmpty(elementPopup.getValue())) {
                    calendar = new CalendarUltis(elementPopup.getValue());
                    tvTitle.setText(DetailFunc.share.formatDateTimePopup(elementParent.getValue()));
                } else {
                    calendar = new CalendarUltis();
                    tvTitle.setText(Functions.share.formatLongToString(calendar.getTimeInMillis(), "dd/MM/yyyy"));
                }

                datePicker.init(calendar.YEAR, calendar.MONTH, calendar.DAY_OF_MONTH, null);
                timePicker.setHour(calendar.HOUR);
                timePicker.setMinute(calendar.MINUTE);

                tvTitle.setText(DetailFunc.share.formatDateTimePopup(elementParent.getValue()));

                if (elementPopup.isEnable()) {
                    imgDelete.setVisibility(View.VISIBLE);
                    imgToday.setVisibility(View.VISIBLE);
                    tvApply.setVisibility(View.VISIBLE);
                    lnApply.setVisibility(View.VISIBLE);
                    datePicker.setEnabled(true);
                    timePicker.setEnabled(true);
                } else {
                    imgDelete.setVisibility(View.INVISIBLE);
                    imgToday.setVisibility(View.INVISIBLE);
                    tvApply.setVisibility(View.GONE);
                    lnApply.setVisibility(View.GONE);
                    datePicker.setEnabled(false);
                    timePicker.setEnabled(false);
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

        imgToday.setOnClickListener(this);
        imgDelete.setOnClickListener(this);
        lnApply.setOnClickListener(this);
        imgClose.setOnClickListener(this);
    }

    private void today() {
        CalendarUltis c = new CalendarUltis();
        datePicker.init(c.YEAR, c.MONTH, c.DAY_OF_MONTH, null);
        timePicker.setHour(c.HOUR);
        timePicker.setMinute(c.MINUTE);
    }

    private void delete() {
        switch (flagView) {
            case Vars.FlagViewControlDynamic.DetailWorkflow: {
                Intent intent = new Intent();
                intent.setAction(VarsReceiver.UPDATEFORM);
                intent.putExtra("element", new Gson().toJson(elementParent));
                intent.putExtra("newValue", "");
                BroadcastUtility.send(mainAct, intent);
                dialog.dismiss();
                break;
            }
            case Vars.FlagViewControlDynamic.DetailWorkflow_InputGridDetail: {
                Intent intent = new Intent();
                intent.setAction(VarsReceiver.UPDATECHILDACTION);
                intent.putExtra("elementParent", new Gson().toJson(elementParent));
                intent.putExtra("elementChild", new Gson().toJson(elementPopup));
                intent.putExtra("json", JObjectChild.toString());
                intent.putExtra("newValue", "");
                BroadcastUtility.send(mainAct, intent);
                dialog.dismiss();
                break;
            }
        }
    }

    private void apply() {
        int day = datePicker.getDayOfMonth();
        int month = datePicker.getMonth();
        int year = datePicker.getYear();
        int hour = timePicker.getHour();
        int minute = timePicker.getMinute();

        Calendar calendar1 = Calendar.getInstance();
        calendar1.set(year, month, day, hour, minute);
        String result = Functions.share.formatLongToString(calendar1.getTimeInMillis(), "yyyy-MM-dd hh:mm:ss");

        switch (flagView) {
            case Vars.FlagViewControlDynamic.DetailWorkflow:
            default: {
                Intent intent = new Intent();
                intent.setAction(VarsReceiver.UPDATEFORM);
                intent.putExtra("element", new Gson().toJson(elementParent));
                intent.putExtra("newValue", result);
                BroadcastUtility.send(mainAct, intent);
                dialog.dismiss();
                break;
            }
            case Vars.FlagViewControlDynamic.DetailWorkflow_InputGridDetail: {
                Intent intent = new Intent();
                intent.setAction(VarsReceiver.UPDATECHILDACTION);
                intent.putExtra("elementParent", new Gson().toJson(elementParent));
                intent.putExtra("elementChild", new Gson().toJson(elementPopup));
                intent.putExtra("json", JObjectChild.toString());
                intent.putExtra("newValue", result);
                BroadcastUtility.send(mainAct, intent);
                dialog.dismiss();
                break;
            }
        }
    }

    @Override
    public void onClick(View view) {
        switch (view.getId()) {
            case R.id.img_PopupControl_DateTimePicker_Today: {
                today();
                break;
            }
            case R.id.img_PopupControl_DateTimePicker_Delete: {
                delete();
                break;
            }
            case R.id.ln_PopupControl_DateTimePicker_Clear: {
                apply();
                break;
            }
            case R.id.img_PopupControl_DateTimePicker_Close: {
                dialog.dismiss();
                break;
            }
        }
    }
}

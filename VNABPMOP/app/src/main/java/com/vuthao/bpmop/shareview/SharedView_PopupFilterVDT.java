package com.vuthao.bpmop.shareview;

import android.app.Activity;
import android.app.Dialog;
import android.graphics.Color;
import android.graphics.drawable.ColorDrawable;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.CalendarView;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.PopupWindow;
import android.widget.TextView;

import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.recyclerview.widget.StaggeredGridLayoutManager;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.AnimationController;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.DateTimeUtility;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.model.app.AppStatus;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.custom.LookupData;
import com.vuthao.bpmop.base.realm.AppBaseController;
import com.vuthao.bpmop.home.HomePageFragment;
import com.vuthao.bpmop.shareview.adapter.PopupFilterMultiChoiceAdapter;
import com.vuthao.bpmop.shareview.adapter.PopupFilterSingleChoiceAdapter;
import com.vuthao.bpmop.home.presenter.HomePagePresenter;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Calendar;
import java.util.HashMap;
import java.util.Objects;

public class SharedView_PopupFilterVDT implements View.OnClickListener {
    private View popupView;
    private TextView tvTinhTrang;
    private LinearLayout lnTinhTrang_Content;
    private TextView tvTinhTrang_Content;
    private TextView tvTrangThai;
    private LinearLayout lnTrangThai_Content;
    private TextView tvTrangThai_Content;
    private TextView tvHanXuLy;
    private LinearLayout lnHanXuLy_Content;
    private TextView tvHanXuLy_Content;
    private TextView tvNgay;
    private TextView tvNgayTuNgay;
    private TextView tvNgayDenNgay;
    private TextView lblNgayTuNgay;
    private TextView lblNgayDenNgay;
    private LinearLayout lnNgayTuNgay;
    private LinearLayout lnNgayDenNgay;
    private TextView tvMacDinh;
    private TextView tvApDung;
    private LinearLayout lnBlurTop;
    private final HomePagePresenter presenter;
    private final HomePagePresenter.HomePageListener listener;
    private ArrayList<LookupData> lstTinhTrang = new ArrayList<>();
    private ArrayList<AppStatus> lstTrangThai = new ArrayList<>();
    private ArrayList<LookupData> lstHanXuLy = new ArrayList<>();
    private String tuNgay;
    private String denNgay;
    private PopupWindow popupFilter;
    private PopupFilterMultiChoiceAdapter adapter;
    private PopupFilterSingleChoiceAdapter singleChoiceAdapter;
    private PopupFilterSingleChoiceAdapter popupFilterSingleChoiceAdapter;
    private final AppBaseController appBaseController;
    private Activity activity;
    private ImageView imgFilter;
    private int workflowId;
    private String type;

    public SharedView_PopupFilterVDT(HomePagePresenter presenter, HomePageFragment homePageFragment,
                                     HomePagePresenter.HomePageListener listener) {
        this.presenter = presenter;
        this.listener = listener;
        appBaseController = new AppBaseController();
    }

    public SharedView_PopupFilterVDT(HomePagePresenter presenter, HomePagePresenter.HomePageListener listener) {
        this.presenter = presenter;
        this.listener = listener;
        appBaseController = new AppBaseController();
    }

    public void filterVDT(Activity activity, LayoutInflater inflater, View showAsDropdown, ImageView imgFilter, String type, int workflowId, boolean isShowStatus) {
        this.activity = activity;
        this.workflowId = workflowId;
        this.imgFilter = imgFilter;
        this.type = type;

        if (popupFilter == null) {
            popupView = inflater.inflate(R.layout.popup_filter_app_base, null);
            popupFilter = new PopupWindow(popupView, WindowManager.LayoutParams.MATCH_PARENT, WindowManager.LayoutParams.MATCH_PARENT);

            //region Init View
            View vwTop = popupView.findViewById(R.id.vw_PopupVer4FilterAppBase_Top);
            tvTinhTrang = popupView.findViewById(R.id.tv_PopupVer4FilterAppBase_TinhTrang);
            lnTinhTrang_Content = popupView.findViewById(R.id.ln_PopupVer4FilterAppBase_TinhTrang_Content);
            tvTinhTrang_Content = popupView.findViewById(R.id.tv_PopupVer4FilterAppBase_TinhTrang_Content);

            tvTrangThai = popupView.findViewById(R.id.tv_PopupVer4FilterAppBase_TrangThai);
            lnTrangThai_Content = popupView.findViewById(R.id.ln_PopupVer4FilterAppBase_TrangThai_Content);
            tvTrangThai_Content = popupView.findViewById(R.id.tv_PopupVer4FilterAppBase_TrangThai_Content);

            tvHanXuLy = popupView.findViewById(R.id.tv_PopupVer4FilterAppBase_HanXuLy);
            lnHanXuLy_Content = popupView.findViewById(R.id.ln_PopupVer4FilterAppBase_HanXuLy_Content);
            tvHanXuLy_Content = popupView.findViewById(R.id.tv_PopupVer4FilterAppBase_HanXuLy_Content);

            tvNgay = popupView.findViewById(R.id.tv_PopupVer4FilterAppBase_Ngay);
            tvNgayTuNgay = popupView.findViewById(R.id.tv_PopupVer4FilterAppBase_Ngay_TuNgay);
            tvNgayDenNgay = popupView.findViewById(R.id.tv_PopupVer4FilterAppBase_Ngay_DenNgay);
            lblNgayTuNgay = popupView.findViewById(R.id.lbl_PopupVer4FilterAppBase_Ngay_TuNgay);
            lblNgayDenNgay = popupView.findViewById(R.id.lbl_PopupVer4FilterAppBase_Ngay_DenNgay);

            lnNgayTuNgay = popupView.findViewById(R.id.ln_PopupVer4FilterAppBase_Ngay_TuNgay);
            lnNgayDenNgay = popupView.findViewById(R.id.ln_PopupVer4FilterAppBase_Ngay_DenNgay);

            tvMacDinh = popupView.findViewById(R.id.tv_PopupVer4FilterAppBase_MacDinh);
            tvApDung = popupView.findViewById(R.id.tv_PopupVer4FilterAppBase_Ngay_ApDung);
            lnBlurTop = popupView.findViewById(R.id.ln_PopupVer4FilterAppBase_TopBlur);

            tvTinhTrang.setText(Functions.share.getTitle("TEXT_STATE", "Trạng thái"));
            tvTrangThai.setText(Functions.share.getTitle("TEXT_STATUS", "Tình trạng"));
            tvTrangThai_Content.setHint(Functions.share.getTitle("TEXT_ALL", "Tất cả"));
            tvHanXuLy.setText(Functions.share.getTitle("TEXT_DUEDATE", "Hạn xử lý"));
            tvNgay.setText(Functions.share.getTitle("TEXT_DATE_OF_ARRIVAL", "Ngày gửi đến"));
            lblNgayTuNgay.setText(Functions.share.getTitle("TEXT_FROMDATE", "Từ ngày"));
            tvNgayTuNgay.setText(Functions.share.getTitle("TEXT_FROMDATE", "Từ ngày"));
            lblNgayDenNgay.setText(Functions.share.getTitle("TEXT_TODATE", "Đến ngày"));
            tvNgayDenNgay.setText(Functions.share.getTitle("TEXT_TODATE", "Đến ngày"));
            tvMacDinh.setText(Functions.share.getTitle("TEXT_RESET_FILTER", "Thiết lập lại"));
            tvApDung.setText(Functions.share.getTitle("TEXT_APPLY", "Áp dụng"));
            //endregion

            if (!isShowStatus) {
                lnTinhTrang_Content.setVisibility(View.GONE);
            }

            if (lstTinhTrang.isEmpty()) {
                lstTinhTrang.add(new LookupData("1", Functions.share.getTitle("TEXT_INPROCESS", "Đang xử lý"), false));
                lstTinhTrang.add(new LookupData("2", Functions.share.getTitle("TEXT_PROCESSED", "Đã xử lý"), false));
                lnTrangThai_Content.setVisibility(View.GONE);

                if (type.equals("processed")) {
                    lstTinhTrang.get(1).setSelected(true);
                    tvTinhTrang_Content.setText(lstTinhTrang.get(1).getTitle());
                } else {
                    lstTinhTrang.get(0).setSelected(true);
                    tvTinhTrang_Content.setText(lstTinhTrang.get(0).getTitle());
                }
            } else {
                if (type.equals("processed")) {
                    lstTinhTrang.get(1).setSelected(true);
                    lstTinhTrang.get(0).setSelected(false);
                    tvTinhTrang_Content.setText(lstTinhTrang.get(1).getTitle());
                } else {
                    lstTinhTrang.get(0).setSelected(true);
                    lstTinhTrang.get(1).setSelected(false);
                    tvTinhTrang_Content.setText(lstTinhTrang.get(0).getTitle());
                }
            }

            if (lstTrangThai.isEmpty()) {
                lstTrangThai.addAll(appBaseController.getListAppStatusVDTDaXuLyFilter());
                AppStatus app = new AppStatus();
                app.setID(0);
                app.setSelected(true);
                app.setTitle(Functions.share.getTitle("TEXT_ALL", "Tất cả"));
                app.setTitleEN(app.getTitle());
                lstTrangThai.add(0, app);

                /*lstTrangThai.addAll(appBaseController.getListAppStatusFilter());
                if (lstTinhTrang.get(0).isSelected()) {
                    String itemSelected = appBaseController.getValueSelected(Constants.APPSTATUS_TOME_DANGXULY);
                    ArrayList<AppStatus> temps = new ArrayList<>();
                    if (!Functions.isNullOrEmpty(itemSelected)) {
                        String[] arr = itemSelected.split(",");
                        for (AppStatus item : lstTrangThai) {
                            if (Arrays.stream(arr).anyMatch(r -> r.equals(String.valueOf(item.getID())))) {
                                temps.add(item);
                            }
                        }

                        lstTrangThai = new ArrayList<>(temps);
                        AppStatus app = new AppStatus();
                        app.setID(0);
                        app.setSelected(true);
                        app.setTitle(Functions.share.getTitle("TEXT_ALL", "Tất cả"));
                        app.setTitleEN(app.getTitle());
                        lstTrangThai.add(0, app);
                    }
                } else {
                    AppStatus app = new AppStatus();
                    app.setID(0);
                    app.setTitle(Functions.share.getTitle("TEXT_ALL", "Tất cả"));
                    app.setTitleEN(app.getTitle());
                    lstTrangThai.add(0, app);
                    boolean isAll = false;
                    // Khong xai` nua
                    String itemSelected = appBaseController.getValueSelected(Constants.APPSTATUS_TOME_DAXULY);
                    if (!Functions.isNullOrEmpty(itemSelected)) {
                        String[] arr = itemSelected.split(",");
                        if (arr.length == lstTrangThai.size() - 1) {
                            isAll = true;
                        }

                        if (isAll) {
                            lstTrangThai.get(0).setSelected(true);
                        } else {
                            for (AppStatus item : lstTrangThai) {
                                if (Arrays.stream(arr).anyMatch(r -> r.equals(String.valueOf(item.getID())))) {
                                    item.setSelected(true);
                                }
                            }
                        }
                    }
                }*/
            }

            presenter.bindTextTrangThai(tvTrangThai_Content, lstTrangThai);

            if (lstHanXuLy.isEmpty()) {
                lstHanXuLy.add(new LookupData("0", Functions.share.getTitle("TEXT_ALL", "Tất cả"), true));
                lstHanXuLy.add(new LookupData("1", Functions.share.getTitle("TEXT_TODAY1", "Trong ngày"), false));
                lstHanXuLy.add(new LookupData("2", Functions.share.getTitle("TEXT_OVERDUE", "Trễ hạn"), false));

                tvHanXuLy_Content.setText(lstHanXuLy.get(0).getTitle());
            } else {
                for (LookupData data : lstHanXuLy) {
                    if (data.isSelected()) {
                        tvHanXuLy_Content.setText(data.getTitle());
                    }
                }
            }

            if (Functions.isNullOrEmpty(tuNgay)) {
                tvNgayTuNgay.setText(Functions.share.getToDay("dd/MM/yyyy", -30));
                tuNgay = tvNgayTuNgay.getText().toString();
            } else {
                tvNgayTuNgay.setText(tuNgay);
            }

            if (Functions.isNullOrEmpty(denNgay)) {
                tvNgayDenNgay.setText(Functions.share.getToDay("dd/MM/yyyy"));
                denNgay = tvNgayDenNgay.getText().toString();
            } else {
                tvNgayDenNgay.setText(denNgay);
            }

            lnTrangThai_Content.setVisibility(View.VISIBLE);
            lnTinhTrang_Content.setOnClickListener(this);
            lnTrangThai_Content.setOnClickListener(this);
            lnHanXuLy_Content.setOnClickListener(this);
            lnBlurTop.setOnClickListener(this);
            lnNgayTuNgay.setOnClickListener(this);
            lnNgayDenNgay.setOnClickListener(this);
            tvMacDinh.setOnClickListener(this);
            tvApDung.setOnClickListener(this);

            lnBlurTop.getLayoutParams().height = showAsDropdown.getHeight();
            popupFilter.setFocusable(true);
            popupFilter.setOutsideTouchable(false);

            //popupFilter.setOnDismissListener(listener::OnFilterDissmiss);
        }

        popupFilter.showAsDropDown(showAsDropdown);
    }

    public void setDefaultValue() {
        popupFilter = null;
        lstTinhTrang = new ArrayList<>();
        lstTrangThai = new ArrayList<>();
        lstHanXuLy = new ArrayList<>();
        tuNgay = "";
        denNgay = "";
    }

    @Override
    public void onClick(View view) {
        switch (view.getId()) {
            case R.id.ln_PopupVer4FilterAppBase_TinhTrang_Content: {
                tinhtrang();
                break;
            }
            case R.id.ln_PopupVer4FilterAppBase_TrangThai_Content: {
                status();
                break;
            }
            case R.id.ln_PopupVer4FilterAppBase_HanXuLy_Content: {
                duedate();
                break;
            }
            case R.id.ln_PopupVer4FilterAppBase_TopBlur: {
                popupFilter.dismiss();
                listener.OnFilterDissmiss();
                break;
            }
            case R.id.ln_PopupVer4FilterAppBase_Ngay_TuNgay: {
                fromdate();
                break;
            }
            case R.id.ln_PopupVer4FilterAppBase_Ngay_DenNgay: {
                todate();
                break;
            }
            case R.id.tv_PopupVer4FilterAppBase_MacDinh: {
                popupFilter.dismiss();
                setDefaultValue();
                listener.OnDefaultFilter(type);
                imgFilter.setColorFilter(ContextCompat.getColor(activity, R.color.clBottomDisable));
                break;
            }
            case R.id.tv_PopupVer4FilterAppBase_Ngay_ApDung: {
                apply();
                break;
            }
        }
    }

    private void apply() {
        HashMap<String, String> hashMap = new HashMap<>();
        hashMap.put("resourceviewid", Constants.mResourceIdToMe);
        hashMap.put("lcid", String.valueOf(CurrentUser.getInstance().getUser().getLanguage()));

        String err = "";
        if (Functions.isNullOrEmpty(tvNgayTuNgay.getText().toString())) {
             err = Functions.share.getTitle("TEXT_FROMDATE_EMPTY", "Từ ngày phải có giá trị");
            listener.OnFilterErr(err);
            return;
        }

        if (Functions.isNullOrEmpty(tvNgayDenNgay.getText().toString())) {
             err = Functions.share.getTitle("TEXT_TODATE_EMPTY", "Đến ngày phải có giá trị");
            listener.OnFilterErr(err);
            return;
        }

        /*long l = Functions.share.formatStringToLong(tvNgayTuNgay.getText().toString(), "dd/MM/yy");
        long l1 = Functions.share.formatStringToLong(tvNgayDenNgay.getText().toString(), "dd/MM/yyyy");
        if (DateTimeUtility.isAfterDay(l1, l)) {
             err = Functions.share.getTitle("TEXT_DATE_COMPARE1", "Ngày bắt đầu không thể lớn hơn ngày kết thúc, vui lòng chọn lại");
            listener.OnFilterErr(err);
            return;
        }*/

        for (LookupData data : lstTinhTrang) {
            if (data.isSelected()) {
                if (data.getID().equals("1")) {
                    hashMap.put("viewtype", "2");
                } else {
                    hashMap.put("viewtype", "4");
                }
            }
        }

        StringBuilder ids = new StringBuilder();
        boolean isAll = false;
        if (lnTrangThai_Content.getVisibility() == View.VISIBLE) {
            for (AppStatus status : lstTrangThai) {
                if (status.isSelected()) {
                    if (status.getID() == 0) {
                        isAll = true;
                        continue;
                    } else {
                        ids.append(status.getID()).append(",");
                    }
                }

                if (isAll) {
                    ids.append(status.getID()).append(",");
                }
            }
        } else {
            ids = new StringBuilder(appBaseController.getValueSelected(Constants.APPSTATUS_TOME_DANGXULY) + ",");
        }


        if (ids.length() > 0) {
            hashMap.put("statusgroup", ids.substring(0, ids.length() - 1));
        } else {
            for (AppStatus status : lstTrangThai) {
                if (status.getID() != 0) {
                    ids.append(status.getID()).append(",");
                }
            }

            hashMap.put("statusgroup", ids.substring(0, ids.length() - 1));
        }

        for (LookupData data : lstHanXuLy) {
            if (data.isSelected()) {
                if (data.getID().equals("1")) {
                    hashMap.put("duedate-gte", Functions.share.getToDay("yyyy-MM-dd ") + "00:00");
                    hashMap.put("duedate-lte", Functions.share.getToDay("yyyy-MM-dd ") + "23:59");
                } else if (data.getID().equals("2")) {
                    hashMap.put("duedate-lte", Functions.share.getToDay("yyyy-MM-dd"));
                }
            }
        }

        if (!Functions.isNullOrEmpty(tuNgay)) {
            hashMap.put("created-gte", DateTimeUtility.format(Functions.share.formatStringToLong(tuNgay), "yyyy-MM-dd HH:mm"));
        }

        if (!Functions.isNullOrEmpty(denNgay))
            hashMap.put("created-lte", DateTimeUtility.format(Functions.share.formatStringToLong(denNgay), "yyyy-MM-dd HH:mm"));

        if (workflowId > 0) {
            hashMap.put("workflowid", String.valueOf(workflowId));
        }

        String status = Objects.requireNonNull(lstTinhTrang.stream().filter(LookupData::isSelected).findFirst().orElse(null)).getID().equals("1") ? "inprocess" : "processed";
        ((BaseActivity) activity).showProgressDialog();
        presenter.filterVDTApi(hashMap, Constants.mFilterLimit, 0, -1, type, status);
        popupFilter.dismiss();
    }

    private void todate() {
        tvNgayTuNgay.setTextColor(ContextCompat.getColor(activity, R.color.clBlack));
        tvNgayDenNgay.setTextColor(ContextCompat.getColor(activity, R.color.clBlack));

        View viewPopupControl = activity.getLayoutInflater().inflate(R.layout.popup_date_picker, null);
        ImageView imgClose = viewPopupControl.findViewById(R.id.img_PopupControl_DatePicker_Ver2_Close);
        ImageView imgDelete = viewPopupControl.findViewById(R.id.img_PopupControl_DatePicker_Ver2_Delete);
        ImageView imgToday = viewPopupControl.findViewById(R.id.img_PopupControl_DatePicker_Ver2_Today);
        CalendarView calendar = viewPopupControl.findViewById(R.id.Calendar_PopupControl_DatePicker_Ver2);

        if (tvNgayDenNgay.getText().toString().isEmpty()) {
            calendar.setDate(Functions.share.formatStringToLong(Functions.share.getToDay("dd/MM/yyyy")));
        } else {
            calendar.setDate(Functions.share.formatStringToLong(tvNgayDenNgay.getText().toString()));
        }

        Dialog dialog = new Dialog(activity);
        dialog.requestWindowFeature(1);
        dialog.setCanceledOnTouchOutside(false);
        dialog.setCancelable(false);
        dialog.setContentView(viewPopupControl);
        dialog.show();

        Window window = dialog.getWindow();
        WindowManager.LayoutParams params = window.getAttributes();
        params.width = activity.getResources().getDisplayMetrics().widthPixels;
        params.height = WindowManager.LayoutParams.WRAP_CONTENT;
        window.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_ADJUST_PAN);
        window.setGravity(Gravity.CENTER);
        window.setAttributes(params);
        window.setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));

        calendar.setOnDateChangeListener((view, year, month, dayOfMonth) -> {
            Calendar c = Calendar.getInstance();
            c.set(year, month, dayOfMonth);

            tvNgayDenNgay.setText(Functions.share.formatLongToDay(c.getTimeInMillis()));
            denNgay = tvNgayDenNgay.getText().toString();
            dialog.dismiss();
        });

        imgToday.setOnClickListener(view -> {
            tvNgayDenNgay.setText(Functions.share.getToDay("dd/MM/yyyy"));
            denNgay = tvNgayDenNgay.getText().toString();
            dialog.dismiss();
        });

        imgClose.setOnClickListener(v12 -> dialog.dismiss());

        imgDelete.setOnClickListener(v1 -> {
            lnNgayDenNgay.setBackgroundResource(R.color.transparent);
            tvNgayDenNgay.setText("");
            denNgay = "";
            dialog.dismiss();
        });
    }

    private void fromdate() {
        tvNgayTuNgay.setTextColor(ContextCompat.getColor(activity, R.color.clBlack));
        tvNgayDenNgay.setTextColor(ContextCompat.getColor(activity, R.color.clBlack));

        View viewPopupControl = activity.getLayoutInflater().inflate(R.layout.popup_date_picker, null);
        ImageView imgClose = viewPopupControl.findViewById(R.id.img_PopupControl_DatePicker_Ver2_Close);
        ImageView imgDelete = viewPopupControl.findViewById(R.id.img_PopupControl_DatePicker_Ver2_Delete);
        ImageView imgToday = viewPopupControl.findViewById(R.id.img_PopupControl_DatePicker_Ver2_Today);
        CalendarView calendar = viewPopupControl.findViewById(R.id.Calendar_PopupControl_DatePicker_Ver2);

        if (tvNgayTuNgay.getText().toString().isEmpty()) {
            calendar.setDate(Functions.share.formatStringToLong(Functions.share.getToDay("dd/MM/yyyy")));
        } else {
            calendar.setDate(Functions.share.formatStringToLong(tvNgayTuNgay.getText().toString()));
        }

        Dialog dialog = new Dialog(activity);
        dialog.requestWindowFeature(1);
        dialog.setCanceledOnTouchOutside(false);
        dialog.setCancelable(false);
        dialog.setContentView(viewPopupControl);
        dialog.show();

        Window window = dialog.getWindow();
        WindowManager.LayoutParams params = window.getAttributes();
        params.width = activity.getResources().getDisplayMetrics().widthPixels;
        params.height = WindowManager.LayoutParams.WRAP_CONTENT;
        window.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_ADJUST_PAN);
        window.setGravity(Gravity.CENTER);
        window.setAttributes(params);
        window.setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));

        calendar.setOnDateChangeListener((view, year, month, dayOfMonth) -> {
            Calendar c = Calendar.getInstance();
            c.set(year, month, dayOfMonth);

            tvNgayTuNgay.setText(Functions.share.formatLongToDay(c.getTimeInMillis()));
            tuNgay = tvNgayTuNgay.getText().toString();
            dialog.dismiss();
        });

        imgToday.setOnClickListener(view -> {
            tvNgayTuNgay.setText(Functions.share.getToDay("dd/MM/yyyy"));
            tuNgay = tvNgayTuNgay.getText().toString();
            dialog.dismiss();
        });

        imgClose.setOnClickListener(v15 -> dialog.dismiss());

        imgDelete.setOnClickListener(v14 -> {
            lnNgayTuNgay.setBackgroundResource(R.color.transparent);
            tvNgayTuNgay.setText("");
            tuNgay = "";
            dialog.dismiss();
        });
    }

    private void tinhtrang() {
        View viewPopupControl = activity.getLayoutInflater().inflate(R.layout.popup_control_single_choice, null);
        ImageView imgClose = viewPopupControl.findViewById(R.id.img_PopupControl_SingleChoice_Close);
        TextView tvTitle = viewPopupControl.findViewById(R.id.tv_PopupControl_SingleChoice_Title);
        RecyclerView recyData = viewPopupControl.findViewById(R.id.recy_PopupControl_SingleChoice_Data);
        ImageView imgDone = viewPopupControl.findViewById(R.id.img_PopupControl_SingleChoice_Done);

        imgDone.setVisibility(View.INVISIBLE);

        //region Dialog
        Dialog dialog = new Dialog(activity, R.style.Theme_Custom_BPMOP_Dialog_FullScreen);
        dialog.requestWindowFeature(1);
        dialog.setCanceledOnTouchOutside(false);
        dialog.setCancelable(false);

        Window window = dialog.getWindow();
        WindowManager.LayoutParams params = window.getAttributes();
        params.width = WindowManager.LayoutParams.MATCH_PARENT;
        params.height = WindowManager.LayoutParams.MATCH_PARENT;
        window.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_ADJUST_PAN);
        window.setGravity(Gravity.BOTTOM);
        window.setAttributes(params);
        window.setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));

        dialog.setContentView(viewPopupControl);
        dialog.show();
        //endregion

        tvTitle.setText(Functions.share.getTitle("TEXT_STATE", "Trạng thái"));

        popupFilterSingleChoiceAdapter = new PopupFilterSingleChoiceAdapter(activity, lstTinhTrang, pos -> {
            lstTrangThai.clear();
            if (lstTinhTrang.get(pos).getID().equals("1")) {
                lstTrangThai.addAll(appBaseController.getListAppStatusFilter());
                String itemSelected = appBaseController.getValueSelected(Constants.APPSTATUS_TOME_DANGXULY);
                ArrayList<AppStatus> temps = new ArrayList<>();
                if (!Functions.isNullOrEmpty(itemSelected)) {
                    String[] arr = itemSelected.split(",");
                    for (AppStatus item : lstTrangThai) {
                        if (Arrays.stream(arr).anyMatch(r -> r.equals(String.valueOf(item.getID())))) {
                            temps.add(item);
                        }
                    }

                    lstTrangThai = new ArrayList<>(temps);
                    AppStatus app = new AppStatus();
                    app.setID(0);
                    app.setSelected(true);
                    app.setTitle(Functions.share.getTitle("TEXT_ALL", "Tất cả"));
                    app.setTitleEN(app.getTitle());
                    lstTrangThai.add(0, app);
                }
            } else {
                lstTrangThai.addAll(appBaseController.getListAppStatusVDTDaXuLyFilter());
                AppStatus app = new AppStatus();
                app.setID(0);
                app.setSelected(true);
                app.setTitle(Functions.share.getTitle("TEXT_ALL", "Tất cả"));
                app.setTitleEN(app.getTitle());
                lstTrangThai.add(0, app);

                /*boolean isAll = false;
                String itemSelected = appBaseController.getValueSelected(Constants.APPSTATUS_TOME_DAXULY);
                if (!Functions.isNullOrEmpty(itemSelected)) {
                    String[] arr = itemSelected.split(",");
                    if (arr.length == lstTrangThai.size() - 1) {
                        isAll = true;
                    }

                    if (isAll) {
                        lstTrangThai.get(0).setSelected(true);
                    } else {
                        for (AppStatus item : lstTrangThai) {
                            if (Arrays.stream(arr).anyMatch(r -> r.equals(String.valueOf(item.getID())))) {
                                item.setSelected(true);
                            }
                        }
                    }
                }*/
            }

            presenter.bindTextTrangThai(tvTrangThai_Content, lstTrangThai);
            for (LookupData data : lstTinhTrang) {
                data.setSelected(data.getID().equals(lstTinhTrang.get(pos).getID()));
            }

            tvTinhTrang_Content.setText(lstTinhTrang.get(pos).getTitle());
            tvTinhTrang_Content.startAnimation(AnimationController.share.fadeIn(activity));

            dialog.dismiss();
        });

        StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.VERTICAL);
        recyData.setAdapter(popupFilterSingleChoiceAdapter);
        recyData.setLayoutManager(staggeredGridLayoutManager);

        imgClose.setOnClickListener(v -> dialog.dismiss());
    }

    private void status() {
        View viewPopupControl = activity.getLayoutInflater().inflate(R.layout.popup_control_single_choice, null);
        ImageView imgClose = viewPopupControl.findViewById(R.id.img_PopupControl_SingleChoice_Close);
        TextView tvTitle = viewPopupControl.findViewById(R.id.tv_PopupControl_SingleChoice_Title);
        RecyclerView recyData = viewPopupControl.findViewById(R.id.recy_PopupControl_SingleChoice_Data);
        ImageView imgDone = viewPopupControl.findViewById(R.id.img_PopupControl_SingleChoice_Done);

        Dialog dialog = new Dialog(activity, R.style.Theme_Custom_BPMOP_Dialog_FullScreen);
        dialog.requestWindowFeature(1);
        dialog.setCanceledOnTouchOutside(false);
        dialog.setCancelable(false);

        //region Dialog
        Window window = dialog.getWindow();
        WindowManager.LayoutParams params = window.getAttributes();
        params.width = WindowManager.LayoutParams.MATCH_PARENT;
        params.height = WindowManager.LayoutParams.MATCH_PARENT;
        window.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_ADJUST_PAN);
        window.setGravity(Gravity.BOTTOM);
        window.setAttributes(params);
        window.setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));

        dialog.setContentView(viewPopupControl);
        dialog.show();
        //endregion

        tvTitle.setText(Functions.share.getTitle("TEXT_STATUS", "Trạng thái"));

        adapter = new PopupFilterMultiChoiceAdapter(activity, lstTrangThai, pos -> {
            if (lstTrangThai.get(pos).getID() == 0) {
                for (AppStatus item : lstTrangThai) {
                    item.setSelected(false);
                }
                lstTrangThai.get(0).setSelected(true);
            } else {
                int countSelected = lstTrangThai.size();
                for (AppStatus item : lstTrangThai) {
                    if (item.getID() == lstTrangThai.get(pos).getID()) {
                        item.setSelected(!item.isSelected());
                    }

                    if (!item.isSelected()) {
                        countSelected--;
                    }
                }

                if (countSelected == lstTrangThai.size() - 1) {
                    for (AppStatus item : lstTrangThai) {
                        item.setSelected(false);
                    }
                    lstTrangThai.get(0).setSelected(true);
                } else {
                    lstTrangThai.get(0).setSelected(countSelected == 0);
                }
            }

            adapter.notifyDataSetChanged();
        });

        StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.VERTICAL);
        recyData.setAdapter(adapter);
        recyData.setLayoutManager(staggeredGridLayoutManager);

        imgClose.setOnClickListener(v19 -> dialog.dismiss());

        imgDone.setOnClickListener(v18 -> {
            presenter.bindTextTrangThai(tvTrangThai_Content, lstTrangThai);
            dialog.dismiss();
        });
    }

    private void duedate() {
        View viewPopupControl = activity.getLayoutInflater().inflate(R.layout.popup_control_single_choice, null);
        ImageView imgClose = viewPopupControl.findViewById(R.id.img_PopupControl_SingleChoice_Close);
        TextView tvTitle = viewPopupControl.findViewById(R.id.tv_PopupControl_SingleChoice_Title);
        RecyclerView recyData = viewPopupControl.findViewById(R.id.recy_PopupControl_SingleChoice_Data);
        ImageView imgDone = viewPopupControl.findViewById(R.id.img_PopupControl_SingleChoice_Done);

        Dialog dialog = new Dialog(activity, R.style.Theme_Custom_BPMOP_Dialog_FullScreen);
        dialog.requestWindowFeature(1);
        dialog.setCanceledOnTouchOutside(false);
        dialog.setCancelable(false);

        Window window = dialog.getWindow();
        WindowManager.LayoutParams params = window.getAttributes();
        params.width = WindowManager.LayoutParams.MATCH_PARENT;
        params.height = WindowManager.LayoutParams.MATCH_PARENT;
        window.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_ADJUST_PAN);
        window.setGravity(Gravity.BOTTOM);
        window.setAttributes(params);
        window.setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));

        dialog.setContentView(viewPopupControl);
        dialog.show();

        imgDone.setVisibility(View.INVISIBLE);
        tvTitle.setText(Functions.share.getTitle("TEXT_DUEDATE", "Hạn xử lý"));

        singleChoiceAdapter = new PopupFilterSingleChoiceAdapter(activity, lstHanXuLy, new PopupFilterSingleChoiceAdapter.PopupFilterSingleChoiceListener() {
            @Override
            public void OnClick(int pos) {
                for (LookupData data : lstHanXuLy) {
                    data.setSelected(data.getID().equals(lstHanXuLy.get(pos).getID()));
                }

                singleChoiceAdapter.notifyDataSetChanged();

                tvHanXuLy_Content.setText(lstHanXuLy.get(pos).getTitle());
                dialog.dismiss();
            }
        });

        StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.VERTICAL);
        recyData.setAdapter(singleChoiceAdapter);
        recyData.setLayoutManager(staggeredGridLayoutManager);

        imgClose.setOnClickListener(v17 -> dialog.dismiss());
    }

    public void changeLanguage() {
        if (popupFilter != null) {
            tvTinhTrang.setText(Functions.share.getTitle("TEXT_STATE", "Trạng thái"));
            tvTrangThai.setText(Functions.share.getTitle("TEXT_STATUS", "Tình trạng"));
            tvHanXuLy.setText(Functions.share.getTitle("TEXT_DUEDATE", "Hạn xử lý"));
            tvNgay.setText(Functions.share.getTitle("TEXT_DATE_OF_ARRIVAL", "Ngày gửi đến"));
            lblNgayTuNgay.setText(Functions.share.getTitle("TEXT_FROMDATE", "Từ ngày"));
            lblNgayDenNgay.setText(Functions.share.getTitle("TEXT_TODATE", "Đến ngày"));
            tvMacDinh.setText(Functions.share.getTitle("TEXT_RESET_FILTER", "Thiết lập lại"));
            tvApDung.setText(Functions.share.getTitle("TEXT_APPLY", "Áp dụng"));

            String currentHXLSelected = "";
            for (LookupData data : lstHanXuLy) {
                if (data.isSelected()) {
                    currentHXLSelected = data.getID();
                    break;
                }
            }

            lstHanXuLy.set(0, new LookupData("0", Functions.share.getTitle("TEXT_ALL", "Tất cả"), "0".equals(currentHXLSelected)));
            lstHanXuLy.set(1, new LookupData("1", Functions.share.getTitle("TEXT_TODAY1", "Trong ngày"), "1".equals(currentHXLSelected)));
            lstHanXuLy.set(2, new LookupData("2", Functions.share.getTitle("TEXT_OVERDUE", "Trễ hạn"), "2".equals(currentHXLSelected)));

            if (singleChoiceAdapter != null) {
                singleChoiceAdapter.notifyDataSetChanged();
            }

            for (LookupData data : lstHanXuLy) {
                if (data.isSelected()) {
                    tvHanXuLy_Content.setText(data.getTitle());
                    break;
                }
            }

            String currentStateSelected = "";
            for (LookupData data : lstTinhTrang) {
                if (data.isSelected()) {
                    currentStateSelected = data.getID();
                    break;
                }
            }

            lstTinhTrang.set(0, new LookupData("1", Functions.share.getTitle("TEXT_INPROCESS", "Đang xử lý"), "1".equals(currentStateSelected)));
            lstTinhTrang.set(1, new LookupData("2", Functions.share.getTitle("TEXT_PROCESSED", "Đã xử lý"), "2".equals(currentHXLSelected)));


            if (popupFilterSingleChoiceAdapter != null) {
                popupFilterSingleChoiceAdapter.notifyDataSetChanged();
            }

            for (LookupData data : lstTinhTrang) {
                if (data.isSelected()) {
                    tvTinhTrang_Content.setText(data.getTitle());
                    break;
                }
            }

            presenter.bindTextTrangThai(tvTrangThai_Content, lstTrangThai);
        }
    }
}


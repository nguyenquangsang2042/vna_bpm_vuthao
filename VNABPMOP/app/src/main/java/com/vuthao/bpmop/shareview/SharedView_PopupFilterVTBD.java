package com.vuthao.bpmop.shareview;

import android.app.Activity;
import android.app.Dialog;
import android.graphics.Color;
import android.graphics.drawable.ColorDrawable;
import android.view.Gravity;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.CalendarView;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.PopupWindow;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.recyclerview.widget.StaggeredGridLayoutManager;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.DateTimeUtility;
import com.vuthao.bpmop.base.Functions;
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

public class SharedView_PopupFilterVTBD implements View.OnClickListener {
    private Activity activity;
    private View vwTop;
    // Tình Trạng
    private TextView tvTinhTrang;
    private TextView tvTinhTrang_Content;
    private LinearLayout lnTinhTrang_Content;
    // Trạng Thái
    private TextView tvTrangThai;
    private TextView tvTrangThai_Content;
    private LinearLayout lnTrangThai_Content;
    private LinearLayout lnHanXuLy_Content;
    // Ngày gửi đến
    private TextView tvNgay;

    // Hạn xử lý
    private TextView tvHanXuLy;
    private TextView tvHanXuLy_Content;
    private LinearLayout lnNgayDenNgay;
    private TextView tvNgayDenNgay;
    private TextView lblNgayDenNgay;

    private LinearLayout _lnNgayTuNgay;
    private TextView _tvNgayTuNgay;
    private TextView lblNgayTuNgay;
    // Default
    private TextView tvMacDinh;
    private TextView tvApDung;
    private LinearLayout lnBlurTop;

    private ImageView imgFilter;

    private String tuNgay;
    private String denNgay;

    private ArrayList<LookupData> lstHanXuLy = new ArrayList<>();
    private ArrayList<AppStatus> lstTrangThai = new ArrayList<>();

    private View popupViewFilter;
    private PopupWindow popupFilter;
    private HomePagePresenter presenter;
    private PopupFilterMultiChoiceAdapter adapter;
    private PopupFilterSingleChoiceAdapter singleChoiceAdapter;
    private HomePagePresenter.HomePageListener listener;
    private AppBaseController appBaseController;
    private int workflowId;

    public SharedView_PopupFilterVTBD(HomePagePresenter presenter, HomePageFragment homePageFragment,
                                      HomePagePresenter.HomePageListener listener) {
        this.presenter = presenter;
        this.listener = listener;
        appBaseController = new AppBaseController();
    }

    public SharedView_PopupFilterVTBD(HomePagePresenter presenter, HomePagePresenter.HomePageListener listener) {
        this.presenter = presenter;
        this.listener = listener;
        appBaseController = new AppBaseController();
    }

    public void filter(Activity activity, View showAsDropdown, ImageView imgFilter, int workflowId) {
        this.activity = activity;
        this.imgFilter = imgFilter;
        this.workflowId = workflowId;

        if (popupFilter == null) {
            popupViewFilter = activity.getLayoutInflater().inflate(R.layout.popup_filter_app_base, null);
            popupFilter = new PopupWindow(popupViewFilter, WindowManager.LayoutParams.MATCH_PARENT, WindowManager.LayoutParams.MATCH_PARENT);
            //region Init View
            vwTop = popupViewFilter.findViewById(R.id.vw_PopupVer4FilterAppBase_Top);

            tvTinhTrang = popupViewFilter.findViewById(R.id.tv_PopupVer4FilterAppBase_TinhTrang);
            lnTinhTrang_Content = popupViewFilter.findViewById(R.id.ln_PopupVer4FilterAppBase_TinhTrang_Content);
            tvTinhTrang_Content = popupViewFilter.findViewById(R.id.tv_PopupVer4FilterAppBase_TinhTrang_Content);

            tvTrangThai = popupViewFilter.findViewById(R.id.tv_PopupVer4FilterAppBase_TrangThai);
            lnTrangThai_Content = popupViewFilter.findViewById(R.id.ln_PopupVer4FilterAppBase_TrangThai_Content);
            tvTrangThai_Content = popupViewFilter.findViewById(R.id.tv_PopupVer4FilterAppBase_TrangThai_Content);

            tvHanXuLy = popupViewFilter.findViewById(R.id.tv_PopupVer4FilterAppBase_HanXuLy);
            lnHanXuLy_Content = popupViewFilter.findViewById(R.id.ln_PopupVer4FilterAppBase_HanXuLy_Content);
            tvHanXuLy_Content = popupViewFilter.findViewById(R.id.tv_PopupVer4FilterAppBase_HanXuLy_Content);

            tvNgay = popupViewFilter.findViewById(R.id.tv_PopupVer4FilterAppBase_Ngay);
            _tvNgayTuNgay = popupViewFilter.findViewById(R.id.tv_PopupVer4FilterAppBase_Ngay_TuNgay);
            tvNgayDenNgay = popupViewFilter.findViewById(R.id.tv_PopupVer4FilterAppBase_Ngay_DenNgay);
            lblNgayTuNgay = popupViewFilter.findViewById(R.id.lbl_PopupVer4FilterAppBase_Ngay_TuNgay);
            lblNgayDenNgay = popupViewFilter.findViewById(R.id.lbl_PopupVer4FilterAppBase_Ngay_DenNgay);

            _lnNgayTuNgay = popupViewFilter.findViewById(R.id.ln_PopupVer4FilterAppBase_Ngay_TuNgay);
            lnNgayDenNgay = popupViewFilter.findViewById(R.id.ln_PopupVer4FilterAppBase_Ngay_DenNgay);

            tvMacDinh = popupViewFilter.findViewById(R.id.tv_PopupVer4FilterAppBase_MacDinh);
            tvApDung = popupViewFilter.findViewById(R.id.tv_PopupVer4FilterAppBase_Ngay_ApDung);
            lnBlurTop = popupViewFilter.findViewById(R.id.ln_PopupVer4FilterAppBase_TopBlur);

            vwTop.setVisibility(View.GONE);
            lnTinhTrang_Content.setVisibility(View.GONE);

            tvTrangThai.setText(Functions.share.getTitle("TEXT_STATUS", "Tình trạng"));
            tvTrangThai_Content.setHint(Functions.share.getTitle("TEXT_ALL", "Tất cả"));
            tvHanXuLy.setText(Functions.share.getTitle("TEXT_DUEDATE", "Hạn xử lý"));
            tvNgay.setText(Functions.share.getTitle("TEXT_CREATEDDATE", "Ngày tạo").replace("khởi ", ""));
            lblNgayTuNgay.setText(Functions.share.getTitle("TEXT_FROMDATE", "Từ ngày"));
            _tvNgayTuNgay.setText(Functions.share.getTitle("TEXT_FROMDATE", "Từ ngày"));
            lblNgayDenNgay.setText(Functions.share.getTitle("TEXT_TODATE", "Đến ngày"));
            tvNgayDenNgay.setText(Functions.share.getTitle("TEXT_TODATE", "Đến ngày"));
            tvMacDinh.setText(Functions.share.getTitle("TEXT_RESET_FILTER", "Thiết lập lại"));
            tvApDung.setText(Functions.share.getTitle("TEXT_APPLY", "Áp dụng"));
            //endregion

            if (lstTrangThai.isEmpty()) {
                AppStatus app = new AppStatus();
                app.setID(0);
                app.setSelected(true);
                app.setTitle(Functions.share.getTitleByRegion("TEXT_ALL", "Tất cả", 1066));
                app.setTitleEN(Functions.share.getTitleByRegion("TEXT_ALL", "Tất cả", 1033));
                lstTrangThai.add(app);
                lstTrangThai.addAll(appBaseController.getListAppStatusFilter());

                //set selected value
                boolean isAll = false;
                String itemSelected = appBaseController.getValueSelected(Constants.APPSTATUS_FROMME);
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
                _tvNgayTuNgay.setText(Functions.share.getToDay("dd/MM/yyyy", -30));
                tuNgay = _tvNgayTuNgay.getText().toString();
            } else {
                _tvNgayTuNgay.setText(tuNgay);
            }

            if (Functions.isNullOrEmpty(denNgay)) {
                tvNgayDenNgay.setText(Functions.share.getToDay("dd/MM/yyyy"));
                denNgay = tvNgayDenNgay.getText().toString();
            } else {
                tvNgayDenNgay.setText(denNgay);
            }

            lnTrangThai_Content.setOnClickListener(this);
            lnHanXuLy_Content.setOnClickListener(this);
            lnBlurTop.setOnClickListener(this);
            _lnNgayTuNgay.setOnClickListener(this);
            lnNgayDenNgay.setOnClickListener(this);
            tvMacDinh.setOnClickListener(this);
            tvApDung.setOnClickListener(this);

            lnBlurTop.getLayoutParams().height = showAsDropdown.getHeight();
            popupFilter.setFocusable(true);
            popupFilter.setOutsideTouchable(false);

            //popupFilter.setOnDismissListener(() -> listener.OnFilterDissmiss());
        }

        popupFilter.showAsDropDown(showAsDropdown);
    }

    public void setDefaultValue() {
        popupFilter = null;
        lstHanXuLy = new ArrayList<>();
        lstTrangThai = new ArrayList<>();
        tuNgay = "";
        denNgay = "";
    }

    @Override
    public void onClick(View view) {
        switch (view.getId()) {
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
                today();
                break;
            }
            case R.id.tv_PopupVer4FilterAppBase_MacDinh: {
                popupFilter.dismiss();
                setDefaultValue();
                listener.OnDefaultFilter("VTBD");
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
        hashMap.put("resourceviewid", Constants.mResourceIdFromMe);
        hashMap.put("lcid", String.valueOf(CurrentUser.getInstance().getUser().getLanguage()));

        String err = "";
        if (Functions.isNullOrEmpty(_tvNgayTuNgay.getText().toString())) {
            err = Functions.share.getTitle("TEXT_FROMDATE_EMPTY", "Từ ngày phải có giá trị");
            listener.OnFilterErr(err);
            return;
        }

        if (Functions.isNullOrEmpty(tvNgayDenNgay.getText().toString())) {
            err = Functions.share.getTitle("TEXT_TODATE_EMPTY", "Đến ngày phải có giá trị");
            listener.OnFilterErr(err);
            return;
        }

        /*long l = Functions.share.formatStringToLong(_tvNgayTuNgay.getText().toString(), "dd/MM/yy");
        long l1 = Functions.share.formatStringToLong(tvNgayDenNgay.getText().toString(), "dd/MM/yyyy");
        if (DateTimeUtility.isAfterDay(l1, l)) {
            err = Functions.share.getTitle("TEXT_DATE_COMPARE1", "Ngày bắt đầu không thể lớn hơn ngày kết thúc, vui lòng chọn lại");
            listener.OnFilterErr(err);
            return;
        }*/

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
            ids = new StringBuilder(appBaseController.getValueSelected(Constants.APPSTATUS_TOME_DAXULY) + ",");
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

        ((BaseActivity) activity).showProgressDialog();
        presenter.filterVTDBApi(hashMap, Constants.mFilterLimit, 0, -1);
        popupFilter.dismiss();
    }

    private void today() {
        _tvNgayTuNgay.setTextColor(ContextCompat.getColor(activity, R.color.clBlack));
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

        imgClose.setOnClickListener(v -> dialog.dismiss());

        imgDelete.setOnClickListener(v -> {
            lblNgayDenNgay.setBackgroundResource(R.color.transparent);
            tvNgayDenNgay.setText("");
            denNgay = "";
            dialog.dismiss();
        });
    }

    private void fromdate() {
        _tvNgayTuNgay.setTextColor(ContextCompat.getColor(activity, R.color.clBlack));
        tvNgayDenNgay.setTextColor(ContextCompat.getColor(activity, R.color.clBlack));

        View view = activity.getLayoutInflater().inflate(R.layout.popup_date_picker, null);
        ImageView imgClose = view.findViewById(R.id.img_PopupControl_DatePicker_Ver2_Close);
        ImageView imgDelete = view.findViewById(R.id.img_PopupControl_DatePicker_Ver2_Delete);
        ImageView imgToday = view.findViewById(R.id.img_PopupControl_DatePicker_Ver2_Today);
        CalendarView calendar = view.findViewById(R.id.Calendar_PopupControl_DatePicker_Ver2);

        if (_tvNgayTuNgay.getText().toString().isEmpty()) {
            calendar.setDate(Functions.share.formatStringToLong(Functions.share.getToDay("dd/MM/yyyy")));
        } else {
            calendar.setDate(Functions.share.formatStringToLong(_tvNgayTuNgay.getText().toString()));
        }

        Dialog dialog = new Dialog(activity);
        dialog.requestWindowFeature(1);
        dialog.setCanceledOnTouchOutside(false);
        dialog.setCancelable(false);
        dialog.setContentView(view);
        dialog.show();

        Window window = dialog.getWindow();
        WindowManager.LayoutParams params = window.getAttributes();
        params.width = activity.getResources().getDisplayMetrics().widthPixels;
        params.height = WindowManager.LayoutParams.WRAP_CONTENT;
        window.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_ADJUST_PAN);
        window.setGravity(Gravity.CENTER);
        window.setAttributes(params);
        window.setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));

        calendar.setOnDateChangeListener((v, year, month, dayOfMonth) -> {
            Calendar c = Calendar.getInstance();
            c.set(year, month, dayOfMonth);

            _tvNgayTuNgay.setText(Functions.share.formatLongToDay(c.getTimeInMillis()));
            tuNgay = _tvNgayTuNgay.getText().toString();
            dialog.dismiss();
        });

        imgToday.setOnClickListener(v -> {
            _tvNgayTuNgay.setText(Functions.share.getToDay("dd/MM/yyyy"));
            tuNgay = _tvNgayTuNgay.getText().toString();
            dialog.dismiss();
        });

        imgClose.setOnClickListener(v -> dialog.dismiss());

        imgDelete.setOnClickListener(v -> {
            _lnNgayTuNgay.setBackgroundResource(R.color.transparent);
            _tvNgayTuNgay.setText("");
            tuNgay = "";
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

        singleChoiceAdapter = new PopupFilterSingleChoiceAdapter(activity, lstHanXuLy, pos -> {
            for (LookupData data : lstHanXuLy) {
                if (data.getID().equals(lstHanXuLy.get(pos).getID())) {
                    data.setSelected(true);
                } else {
                    data.setSelected(false);
                }
            }

            singleChoiceAdapter.notifyDataSetChanged();

            tvHanXuLy_Content.setText(lstHanXuLy.get(pos).getTitle());
            dialog.dismiss();
        });

        StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.VERTICAL);
        recyData.setAdapter(singleChoiceAdapter);
        recyData.setLayoutManager(staggeredGridLayoutManager);

        imgClose.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                dialog.dismiss();
            }
        });
    }

    private void status() {
        View viewPopupControl = activity.getLayoutInflater().inflate(R.layout.popup_control_single_choice, null);
        ImageView imgClose = viewPopupControl.findViewById(R.id.img_PopupControl_SingleChoice_Close);
        TextView tvTitle = viewPopupControl.findViewById(R.id.tv_PopupControl_SingleChoice_Title);
        RecyclerView recyData = viewPopupControl.findViewById(R.id.recy_PopupControl_SingleChoice_Data);
        ImageView imgDone = viewPopupControl.findViewById(R.id.img_PopupControl_SingleChoice_Done);

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

        imgClose.setOnClickListener(v -> dialog.dismiss());

        imgDone.setOnClickListener(v -> {
            presenter.bindTextTrangThai(tvTrangThai_Content, lstTrangThai);
            dialog.dismiss();
        });
    }

    public void changeLanguage() {
        if (popupFilter != null) {
            tvTrangThai.setText(Functions.share.getTitle("TEXT_STATUS", "Tình trạng"));
            tvHanXuLy.setText(Functions.share.getTitle("TEXT_DUEDATE", "Hạn xử lý"));
            tvNgay.setText(Functions.share.getTitle("TEXT_CREATEDDATE", "Ngày tạo").replace("khởi ", ""));
            lblNgayTuNgay.setText(Functions.share.getTitle("TEXT_FROMDATE", "Từ ngày"));
            lblNgayDenNgay.setText(Functions.share.getTitle("TEXT_TODATE", "Đến ngày"));
            tvMacDinh.setText(Functions.share.getTitle("TEXT_RESET_FILTER", "Thiết lập lại"));
            tvApDung.setText(Functions.share.getTitle("TEXT_APPLY", "Áp dụng"));

            String currentSelected = "";
            for (LookupData data : lstHanXuLy) {
                if (data.isSelected()) {
                    currentSelected = data.getID();
                    break;
                }
            }

            lstHanXuLy.set(0, new LookupData("0", Functions.share.getTitle("TEXT_ALL", "Tất cả"), "0".equals(currentSelected)));
            lstHanXuLy.set(1, new LookupData("1", Functions.share.getTitle("TEXT_TODAY1", "Trong ngày"), "1".equals(currentSelected)));
            lstHanXuLy.set(2, new LookupData("2", Functions.share.getTitle("TEXT_OVERDUE", "Trễ hạn"), "2".equals(currentSelected)));

            if (singleChoiceAdapter != null) {
                singleChoiceAdapter.notifyDataSetChanged();
            }

            for (LookupData data : lstHanXuLy) {
                if (data.isSelected()) {
                    tvHanXuLy_Content.setText(data.getTitle());
                    break;
                }
            }

            presenter.bindTextTrangThai(tvTrangThai_Content, lstTrangThai);
        }
    }
}

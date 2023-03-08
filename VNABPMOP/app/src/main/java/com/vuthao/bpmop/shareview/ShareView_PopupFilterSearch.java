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
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.PopupWindow;
import android.widget.TextView;

import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.recyclerview.widget.StaggeredGridLayoutManager;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.AnimationController;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.model.app.AppStatus;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.model.app.Workflow;
import com.vuthao.bpmop.base.model.app.WorkflowStatus;
import com.vuthao.bpmop.base.model.custom.LookupData;
import com.vuthao.bpmop.base.model.custom.ObjectFilter;
import com.vuthao.bpmop.base.model.custom.ObjectPropertySearch;
import com.vuthao.bpmop.base.realm.AppBaseController;
import com.vuthao.bpmop.base.realm.RealmController;
import com.vuthao.bpmop.home.presenter.HomePagePresenter;
import com.vuthao.bpmop.shareview.adapter.FilterSearchWorkflowStatusAdapter;
import com.vuthao.bpmop.shareview.adapter.PopupFilterMultiChoiceAdapter;
import com.vuthao.bpmop.shareview.adapter.PopupFilterSingleChoiceAdapter;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Calendar;

import io.realm.Realm;
import io.realm.RealmResults;
import io.realm.Sort;

public class ShareView_PopupFilterSearch implements View.OnClickListener {
    private View popupView;
    private TextView tvTinhTrang;
    private LinearLayout lnTinhTrang_Content;
    private TextView tvTinhTrang_Content;
    private TextView tvTrangThai;
    private LinearLayout lnTrangThai_Content;
    private TextView tvTrangThai_Content;
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
    private EditText edtSearch;
    private ArrayList<LookupData> workflows = new ArrayList<>();
    private ArrayList<WorkflowStatus> status = new ArrayList<>();
    private String tuNgay;
    private String denNgay;
    private PopupWindow popupFilter;
    private Activity activity;
    private FilterSearchListener listener;
    private Realm realm;
    private FilterSearchWorkflowStatusAdapter adapter;
    private PopupFilterSingleChoiceAdapter singleChoiceAdapter;
    private ObjectPropertySearch objectPropertySearch;

    public ShareView_PopupFilterSearch(Activity activity, FilterSearchListener listener) {
        this.activity = activity;
        this.listener = listener;

        realm = new RealmController().getRealm();
    }

    public void filter(View showAsDropdown, ObjectPropertySearch objectPropertySearch) {
        this.objectPropertySearch = objectPropertySearch;
        if (popupFilter == null) {
            popupView = activity.getLayoutInflater().inflate(R.layout.popup_filter_search, null);
            popupFilter = new PopupWindow(popupView, WindowManager.LayoutParams.MATCH_PARENT, WindowManager.LayoutParams.MATCH_PARENT);

            //region Init View
            edtSearch = popupView.findViewById(R.id.edt_ListWorkflowView_Search);
            tvTinhTrang = popupView.findViewById(R.id.tv_PopupVer4FilterAppBase_TinhTrang);
            lnTinhTrang_Content = popupView.findViewById(R.id.ln_PopupVer4FilterAppBase_TinhTrang_Content);
            tvTinhTrang_Content = popupView.findViewById(R.id.tv_PopupVer4FilterAppBase_TinhTrang_Content);

            tvTrangThai = popupView.findViewById(R.id.tv_PopupVer4FilterAppBase_TrangThai);
            lnTrangThai_Content = popupView.findViewById(R.id.ln_PopupVer4FilterAppBase_TrangThai_Content);
            tvTrangThai_Content = popupView.findViewById(R.id.tv_PopupVer4FilterAppBase_TrangThai_Content);

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

            tvTinhTrang.setText(Functions.share.getTitle("TEXT_WORKFLOW", "Quy trình"));
            tvTrangThai.setText(Functions.share.getTitle("TEXT_STATUS", "Tình trạng"));
            tvTrangThai_Content.setHint(Functions.share.getTitle("TEXT_ALL", "Tất cả"));
            tvNgay.setText(Functions.share.getTitle("TEXT_DATE_OF_ARRIVAL", "Ngày gửi đến"));
            lblNgayTuNgay.setText(Functions.share.getTitle("TEXT_FROMDATE", "Từ ngày"));
            tvNgayTuNgay.setText(Functions.share.getTitle("TEXT_FROMDATE", "Từ ngày"));
            lblNgayDenNgay.setText(Functions.share.getTitle("TEXT_TODATE", "Đến ngày"));
            tvNgayDenNgay.setText(Functions.share.getTitle("TEXT_TODATE", "Đến ngày"));
            tvMacDinh.setText(Functions.share.getTitle("TEXT_RESET_FILTER", "Thiết lập lại"));
            tvApDung.setText(Functions.share.getTitle("TEXT_APPLY", "Áp dụng"));
            //endregion

            if (workflows.isEmpty()) {
                workflows.add(new LookupData("-1", "Tất cả", true));
                workflows.addAll(getWorkflows());

                tvTinhTrang_Content.setText(workflows.get(0).getTitle());
            }

            if (status.isEmpty()) {
                WorkflowStatus app = new WorkflowStatus();
                app.setID(0);
                app.setSelected(true);
                app.setTitle(Functions.share.getTitle("TEXT_ALL", "Tất cả"));
                app.setTitleEN(app.getTitle());
                status.add(app);

                status.addAll(getStatus());

                tvTrangThai_Content.setText(status.get(0).getTitle());
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

            lnTinhTrang_Content.setOnClickListener(this);
            lnTrangThai_Content.setOnClickListener(this);
            lnNgayTuNgay.setOnClickListener(this);
            lnNgayDenNgay.setOnClickListener(this);
            tvApDung.setOnClickListener(this);
            tvMacDinh.setOnClickListener(this);
            lnBlurTop.setOnClickListener(this);

            lnBlurTop.getLayoutParams().height = showAsDropdown.getHeight();
            popupFilter.setFocusable(true);
            popupFilter.setOutsideTouchable(false);

            popupFilter.setOnDismissListener(() -> listener.OnFilterDismiss());
        }

        popupFilter.showAsDropDown(showAsDropdown);
    }

    @Override
    public void onClick(View view) {
        switch (view.getId()) {
            case R.id.ln_PopupVer4FilterAppBase_TinhTrang_Content: {
                workflows();
                break;
            }
            case R.id.ln_PopupVer4FilterAppBase_TopBlur: {
                popupFilter.dismiss();
                break;
            }
            case R.id.ln_PopupVer4FilterAppBase_TrangThai_Content: {
                status();
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
            case R.id.tv_PopupVer4FilterAppBase_Ngay_ApDung: {
                apply();
                break;
            }
            case R.id.tv_PopupVer4FilterAppBase_MacDinh: {
                popupFilter.dismiss();
                setDefaultValue();
                listener.OnDefaultFilter();
                break;
            }
        }
    }

    private void workflows() {
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

        tvTitle.setText(Functions.share.getTitle("TEXT_WORKFLOW", "Quy trình"));

        singleChoiceAdapter = new PopupFilterSingleChoiceAdapter(activity, workflows, pos -> {
            for (LookupData data : workflows) {
                data.setSelected(false);
            }

            workflows.get(pos).setSelected(true);
            tvTinhTrang_Content.setText(workflows.get(pos).getTitle());
            singleChoiceAdapter.notifyItemChanged(pos);
            dialog.dismiss();
        });

        StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.VERTICAL);
        recyData.setAdapter(singleChoiceAdapter);
        recyData.setLayoutManager(staggeredGridLayoutManager);

        imgClose.setOnClickListener(v -> dialog.dismiss());
    }

    private void status() {
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

        tvTitle.setText(Functions.share.getTitle("TEXT_STATUS", "Tình trạng"));

        adapter = new FilterSearchWorkflowStatusAdapter(activity, status, pos -> {
            for (WorkflowStatus s : status) {
                s.setSelected(false);
            }

            status.get(pos).setSelected(true);
            tvTrangThai_Content.setText(status.get(pos).getTitle());
            adapter.notifyItemChanged(pos);
            dialog.dismiss();
        });

        StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.VERTICAL);
        recyData.setAdapter(adapter);
        recyData.setLayoutManager(staggeredGridLayoutManager);

        imgClose.setOnClickListener(v19 -> dialog.dismiss());

        imgDone.setOnClickListener(v18 -> {
            dialog.dismiss();
        });
    }

    private void todate() {
        tvNgayTuNgay.setTextColor(ContextCompat.getColor(activity, R.color.clBlack));
        tvNgayDenNgay.setTextColor(ContextCompat.getColor(activity, R.color.clBlack));

        View viewPopupControl = activity.getLayoutInflater().inflate(R.layout.popup_date_picker, null);
        ImageView imgClose = viewPopupControl.findViewById(R.id.img_PopupControl_DatePicker_Ver2_Close);
        ImageView imgDelete = viewPopupControl.findViewById(R.id.img_PopupControl_DatePicker_Ver2_Delete);
        ImageView imgToday = viewPopupControl.findViewById(R.id.img_PopupControl_DatePicker_Ver2_Today);
        CalendarView calendar = viewPopupControl.findViewById(R.id.Calendar_PopupControl_DatePicker_Ver2);

        calendar.setDate(Functions.share.formatStringToLong(tvNgayDenNgay.getText().toString()));

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

        calendar.setDate(Functions.share.formatStringToLong(tvNgayTuNgay.getText().toString()));

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

    private void apply() {
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

        ArrayList<ObjectFilter> filters = new Gson().fromJson(objectPropertySearch.getLstProSeach(), new TypeToken<ArrayList<ObjectFilter>>() {
        }.getType());

        for (int i = 0; i < workflows.size(); i++) {
            if (workflows.get(i).isSelected()) {
                if (workflows.get(i).getID().equals("-1")) {
                    filters.set(1, new ObjectFilter("WorkflowId", "eq", "", "", "text"));
                } else {
                    filters.set(1, new ObjectFilter("WorkflowId", "eq", workflows.get(i).getID(), "", "text"));
                }
                break;
            }
        }

        for (int i = 0; i < status.size(); i++) {
            if (status.get(i).isSelected()) {
                if (status.get(i).getID() == 0) {
                    filters.set(4, new ObjectFilter("Status", "eq", "", "", "text"));
                } else {
                    filters.set(4, new ObjectFilter("Status", "eq", String.valueOf(status.get(i).getID()), "", "text"));
                }
                break;
            }
        }

        if (tuNgay.isEmpty()) {
            filters.set(2, new ObjectFilter("FromDate", "gte", Functions.share.getToDay("yyyy-MM-dd", -30), "", "datetime"));
        } else {
            long l = Functions.share.formatStringToLong(tuNgay, "dd/MM/yyyy");
            filters.set(2, new ObjectFilter("FromDate", "gte", Functions.share.formatLongToString(l, "yyyy-MM-dd"), "", "datetime"));
        }

        if (denNgay.isEmpty()) {
            filters.set(3, new ObjectFilter("lte", "eq", Functions.share.getToDay("yyyy-MM-dd"), "", "datetime"));
        } else {
            long l = Functions.share.formatStringToLong(tuNgay, "dd/MM/yyyy");
            filters.set(3, new ObjectFilter("lte", "eq", Functions.share.formatLongToString(l, "yyyy-MM-dd"), "", "datetime"));
        }

        filters.set(5, new ObjectFilter("KeyWord", "contains", edtSearch.getText().toString(), "", "text"));
        filters.set(0, new ObjectFilter("lcid", "eq", String.valueOf(CurrentUser.getInstance().getUser().getLanguage()), "", "text"));

        objectPropertySearch.setLstProSeach(new Gson().toJson(filters));
        objectPropertySearch.setLimit(Constants.mFilterLimit - 40);
        objectPropertySearch.setOffset(0);
        objectPropertySearch.setTotal(-1);

        listener.OnFilterSuccess(objectPropertySearch);
        popupFilter.dismiss();
    }

    private ArrayList<LookupData> getWorkflows() {
        boolean isVN = CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN);
        ArrayList<LookupData> items = new ArrayList<>();
        RealmResults<Workflow> results = realm.where(Workflow.class)
                .equalTo("StatusName", "Active")
                .sort("Title", Sort.ASCENDING)
                .findAll();

        for (Workflow workflow : results) {
            LookupData lookupData = new LookupData();
            lookupData.setID(String.valueOf(workflow.getWorkflowID()));
            lookupData.setTitle(isVN ? workflow.getTitle() : workflow.getTitleEN());
            lookupData.setSelected(false);

            items.add(lookupData);
        }

        return items;
    }

    private ArrayList<WorkflowStatus> getStatus() {
        RealmResults<WorkflowStatus> results = realm.where(WorkflowStatus.class)
                .findAll();
        return new ArrayList<>(results);
    }

    public void setDefaultValue() {
        popupFilter = null;
        status = new ArrayList<>();
        workflows = new ArrayList<>();
        tuNgay = "";
        denNgay = "";
    }

    public interface FilterSearchListener {
        void OnFilterSuccess(ObjectPropertySearch objectPropertySearch);

        void OnFilterErr(String err);

        void OnFilterDismiss();
        void OnDefaultFilter();
    }
}

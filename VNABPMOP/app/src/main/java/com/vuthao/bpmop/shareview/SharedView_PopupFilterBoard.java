package com.vuthao.bpmop.shareview;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.res.ColorStateList;
import android.graphics.Typeface;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.View;
import android.view.WindowManager;
import android.widget.CalendarView;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.PopupWindow;
import android.widget.RelativeLayout;
import android.widget.TextView;

import androidx.core.content.ContextCompat;
import androidx.core.content.res.ResourcesCompat;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.AnimationController;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.child.fragment.board.VarsChildBoard;

import java.util.Calendar;

public class SharedView_PopupFilterBoard extends SharedViewBase implements View.OnClickListener {

    public interface SharedView_PopupFilterBoardListener {
        void OnFilterSuccess(String fromday, String today, Integer[] status);

        void OnDefaultFilter();

        void OnFilterDismiss();

        void OnHeaderDay(String fromday, String today, Integer[] status);
    }

    // Ngày tạo
    private TextView tvToday;
    private TextView tvYesterday;
    private TextView tv7Days;
    private TextView tv30Days;

    private TextView tvNgayTao;

    private RelativeLayout relaNgayTao_TuNgay;
    private LinearLayout lnNgayTao_TuNgay;
    private CalendarView calendarTuNgay;
    private ImageView imgNgayTao_TuNgay_Today;
    private ImageView imgNgayTao_TuNgay_Delete;
    private ImageView imgNgayTao_TuNgay;
    private TextView tvNgayTao_TuNgay;

    private RelativeLayout relaNgayTao_DenNgay;
    private LinearLayout lnNgayTao_DenNgay;
    private CalendarView calendarDenNgay;
    private ImageView imgNgayTao_DenNgay_Today;
    private ImageView imgNgayTao_DenNgay_Delete;
    private ImageView img_PopupVer2FilterBoard_DenNgay_Clear;
    private ImageView img_PopupVer2FilterBoard_TuNgay_Clear;
    private ImageView imgNgayTao_DenNgay;
    private TextView tvNgayTao_DenNgay;

    // Tình trạng
    private TextView tvTinhTrang;
    private TextView tvTatCa;
    private TextView tvChoPheDuyet;
    private TextView tvDaPheDuyet;
    private TextView tvTuChoi;

    private TextView tvApDung;
    private TextView tvMacDinh;
    private LinearLayout lnTopBlur;
    private LinearLayout lnBottomBlur;

    private SharedView_PopupFilterBoardListener listener;
    private PopupWindow popupFilter;
    private RelativeLayout showAs;
    private boolean isReset;
    private String tuNgay;
    private String denNgay;
    private boolean isEN;
    private boolean isToday, isYesterday, is7Days, isLastMonth;
    private boolean isAll = true;
    private boolean isFirst = true;
    private boolean isPending, isDone, isReject;

    public SharedView_PopupFilterBoard(LayoutInflater inflater, Activity mainAct, String fragmentTag, View rootView) {
        super(inflater, mainAct, fragmentTag, rootView);
    }

    public void init(SharedView_PopupFilterBoardListener listener, RelativeLayout showAs) {
        this.listener = listener;
        this.showAs = showAs;

        isEN = CurrentUser.getInstance().getUser().getLanguage() != Integer.parseInt(Constants.mLangVN);
    }

    @Override
    public void initializeView() {
        super.initializeView();

        //region Init View
        View view = inflater.inflate(R.layout.popup_filter_board, null);
        popupFilter = new PopupWindow(view, WindowManager.LayoutParams.MATCH_PARENT, WindowManager.LayoutParams.MATCH_PARENT);

        tvToday = view.findViewById(R.id.tv_PopupVer2FilterBoard_Today);
        tvYesterday = view.findViewById(R.id.tv_PopupVer2FilterBoard_Yesterday);
        tv7Days = view.findViewById(R.id.tv_PopupVer2FilterBoard_7Days);
        tv30Days = view.findViewById(R.id.tv_PopupVer2FilterBoard_30Days);

        relaNgayTao_TuNgay = view.findViewById(R.id.rela_PopupVer2FilterBoard_TuNgay);
        tvNgayTao = view.findViewById(R.id.tv_PopupVer2FilterBoard_NgayTao);
        lnNgayTao_TuNgay = view.findViewById(R.id.ln_PopupVer2FilterBoard_NgayTao_TuNgay);
        calendarTuNgay = view.findViewById(R.id.calendar_PopupVer2FilterBoard_TuNgay);
        imgNgayTao_TuNgay = view.findViewById(R.id.img_PopupVer2FilterBoard_NgayTao_TuNgay);
        imgNgayTao_TuNgay_Today = view.findViewById(R.id.img_PopupVer2FilterBoard_TuNgay_Today);
        imgNgayTao_TuNgay_Delete = view.findViewById(R.id.img_PopupVer2FilterBoard_TuNgay_Delete);
        tvNgayTao_TuNgay = view.findViewById(R.id.tv_PopupVer2FilterBoard_NgayTao_TuNgay);

        relaNgayTao_DenNgay = view.findViewById(R.id.rela_PopupVer2FilterBoard_NgayTao_DenNgay);
        lnNgayTao_DenNgay = view.findViewById(R.id.ln_PopupVer2FilterBoard_NgayTao_DenNgay);
        calendarDenNgay = view.findViewById(R.id.calendar_PopupVer2FilterBoard_DenNgay);
        imgNgayTao_DenNgay = view.findViewById(R.id.img_PopupVer2FilterBoard_NgayTao_DenNgay);
        imgNgayTao_DenNgay_Today = view.findViewById(R.id.img_PopupVer2FilterBoard_DenNgay_Today);
        imgNgayTao_DenNgay_Delete = view.findViewById(R.id.img_PopupVer2FilterBoard_DenNgay_Delete);
        img_PopupVer2FilterBoard_TuNgay_Clear = view.findViewById(R.id.img_PopupVer2FilterBoard_TuNgay_Clear);
        img_PopupVer2FilterBoard_DenNgay_Clear = view.findViewById(R.id.img_PopupVer2FilterBoard_DenNgay_Clear);
        tvNgayTao_DenNgay = view.findViewById(R.id.tv_PopupVer2FilterBoard_NgayTao_DenNgay);

        tvTinhTrang = view.findViewById(R.id.tv_PopupVer2FilterBoard_TinhTrang);
        tvTatCa = view.findViewById(R.id.tv_PopupVer2FilterBoard_TinhTrang_TatCa);
        tvChoPheDuyet = view.findViewById(R.id.tv_PopupVer2FilterBoard_TinhTrang_ChoPheDuyet);
        tvDaPheDuyet = view.findViewById(R.id.tv_PopupVer2FilterBoard_TinhTrang_DaPheDuyet);
        tvTuChoi = view.findViewById(R.id.tv_PopupVer2FilterBoard_TinhTrang_TuChoi);

        tvApDung = view.findViewById(R.id.tv_PopupVer2FilterBoard_ApDung);
        tvMacDinh = view.findViewById(R.id.tv_PopupVer2FilterBoard_MacDinh);

        lnTopBlur = view.findViewById(R.id.ln_PopupVer2FilterBoard_TopBlur);
        lnBottomBlur = view.findViewById(R.id.ln_PopupVer2FilterBoard_BottomBlur);
        // endregion

        tvToday.setText(Functions.share.getTitle("TEXT_TODAY", "Hôm nay"));
        tv7Days.setText(Functions.share.getTitle("TEXT_7DAYS", "7 ngày"));
        tv30Days.setText(Functions.share.getTitle("TEXT_30DAYS", "30 ngày"));
        tvYesterday.setText(Functions.share.getTitle("TEXT_YESTERDAY", "Hôm qua"));
        tvNgayTao.setText(Functions.share.getTitle("TEXT_CREATEDDATE", "Ngày khởi tạo"));
        tvTinhTrang.setText(Functions.share.getTitle("TEXT_STATUS", "Trạng thái"));
        tvTatCa.setText(Functions.share.getTitle("TEXT_ALL", "Tất cả"));
        tvChoPheDuyet.setText(Functions.share.getTitle("TEXT_WAITING_APPROVE", "Chờ duyệt"));
        tvDaPheDuyet.setText(Functions.share.getTitle("TEXT_APPROVED", "Đã duyệt"));
        tvTuChoi.setText(Functions.share.getTitle("TEXT_REJECT", "Từ chối"));
        tvMacDinh.setText(Functions.share.getTitle("TEXT_RESET_FILTER", "Thiết lập lại"));
        tvApDung.setText(Functions.share.getTitle("TEXT_APPLY", "Áp dụng"));

        if (isAll) {
            setTextview_Selected_Filter(tvTatCa);
        }

        if (isDone) {
            setTextview_Selected_Filter(tvDaPheDuyet);
        }

        if (isPending) {
            setTextview_Selected_Filter(tvChoPheDuyet);
        }

        if (isReject) {
            setTextview_Selected_Filter(tvTuChoi);
        }

        if (isToday) {
            if (isFirst) {
                setTextView_DateFilter(1);
            } else {
                tuNgay = Functions.share.getToDay("dd/MM/yyyy");
                denNgay = tuNgay;
            }
        } else if (isYesterday) {
            setTextView_DateFilter(2);
            tuNgay = Functions.share.getToDay("dd/MM/yyyy", -1);
            denNgay = Functions.share.getToDay("dd/MM/yyyy");
        } else if (is7Days) {
            setTextView_DateFilter(3);
            tuNgay = Functions.share.getToDay("dd/MM/yyyy", -7);
            denNgay = Functions.share.getToDay("dd/MM/yyyy");
        } else if (isLastMonth) {
            setTextView_DateFilter(4);
            tuNgay = Functions.share.getToDay("dd/MM/yyyy", -30);
            denNgay = Functions.share.getToDay("dd/MM/yyyy");
        } else {
            setTextView_DateFilter(0);
        }

        if (Functions.isNullOrEmpty(tuNgay)) {
            if (isEN) {
                tvNgayTao_TuNgay.setText("From Date");
            } else {
                tvNgayTao_TuNgay.setText("Từ ngày");
            }
        } else {
            tvNgayTao_TuNgay.setText(tuNgay);
        }

        if (Functions.isNullOrEmpty(denNgay)) {
            if (isEN) {
                tvNgayTao_DenNgay.setText("To Date");
            } else {
                tvNgayTao_DenNgay.setText("Đến ngày");
            }
        } else {
            tvNgayTao_DenNgay.setText(denNgay);
        }

        //region Event
        tvToday.setOnClickListener(this);
        tvYesterday.setOnClickListener(this);
        tv7Days.setOnClickListener(this);
        tv30Days.setOnClickListener(this);
        lnNgayTao_TuNgay.setOnClickListener(this);
        lnNgayTao_DenNgay.setOnClickListener(this);
        imgNgayTao_TuNgay_Today.setOnClickListener(this);
        imgNgayTao_DenNgay_Today.setOnClickListener(this);
        imgNgayTao_TuNgay_Delete.setOnClickListener(this);
        imgNgayTao_DenNgay_Delete.setOnClickListener(this);
        img_PopupVer2FilterBoard_DenNgay_Clear.setOnClickListener(this);
        img_PopupVer2FilterBoard_TuNgay_Clear.setOnClickListener(this);
        tvTatCa.setOnClickListener(this);
        tvChoPheDuyet.setOnClickListener(this);
        tvDaPheDuyet.setOnClickListener(this);
        tvTuChoi.setOnClickListener(this);
        tvMacDinh.setOnClickListener(this);
        tvApDung.setOnClickListener(this);

        calendarTuNgay.setOnDateChangeListener((calendarView, year, month, dayOfMonth) -> {
            lnNgayTao_TuNgay.setBackgroundResource(R.drawable.drawable_popupfilter_notselected);
            imgNgayTao_TuNgay.setColorFilter(ContextCompat.getColor(mainAct, R.color.clGraytitle));
            relaNgayTao_TuNgay.setVisibility(View.GONE);
            Calendar c = Calendar.getInstance();
            c.set(year, month, dayOfMonth);

            tvNgayTao_TuNgay.setText(Functions.share.formatLongToDay(c.getTimeInMillis()));
            tuNgay = tvNgayTao_TuNgay.getText().toString();
        });

        calendarDenNgay.setOnDateChangeListener((calendarView, year, month, dayOfMonth) -> {
            lnNgayTao_DenNgay.setBackgroundResource(R.drawable.drawable_popupfilter_notselected);
            imgNgayTao_DenNgay.setColorFilter(ContextCompat.getColor(mainAct, R.color.clGraytitle));
            relaNgayTao_DenNgay.setVisibility(View.GONE);

            Calendar c = Calendar.getInstance();
            c.set(year, month, dayOfMonth);

            tvNgayTao_DenNgay.setText(Functions.share.formatLongToDay(c.getTimeInMillis()));
            denNgay = tvNgayTao_DenNgay.getText().toString();
        });

        tvNgayTao_TuNgay.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {

            }

            @Override
            public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {

            }

            @Override
            public void afterTextChanged(Editable editable) {
                if (editable.length() > 0) {
                    tvNgayTao_TuNgay.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.ITALIC);
                } else {
                    tvNgayTao_TuNgay.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.NORMAL);
                }
            }
        });

        tvNgayTao_DenNgay.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {

            }

            @Override
            public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {

            }

            @Override
            public void afterTextChanged(Editable editable) {
                if (editable.length() > 0) {
                    tvNgayTao_DenNgay.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.ITALIC);
                } else {
                    tvNgayTao_DenNgay.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.NORMAL);
                }
            }
        });

        lnTopBlur.setOnClickListener(view1 -> popupFilter.dismiss());
        lnBottomBlur.setOnClickListener(view12 -> popupFilter.dismiss());
        //endregion

        //region Dialog
        popupFilter.setFocusable(true);
        popupFilter.setOutsideTouchable(false);
        popupFilter.showAsDropDown(showAs);
        popupFilter.setOnDismissListener(() -> listener.OnFilterDismiss());
        //endregion
    }

    private void clear() {
        isFirst = true;
        isToday = false;
        isYesterday = false;
        is7Days = false;
        isLastMonth = false;
        isPending = false;
        isDone = false;
        isReject = false;
        isAll = true;
        tuNgay = "";
        denNgay = "";
    }

    @Override
    public void onClick(View view) {
        switch (view.getId()) {
            case R.id.tv_PopupVer2FilterBoard_Today: {
                header(1);
                break;
            }
            case R.id.tv_PopupVer2FilterBoard_Yesterday: {
                header(-1);
                break;
            }
            case R.id.tv_PopupVer2FilterBoard_7Days: {
                header(7);
                break;
            }
            case R.id.tv_PopupVer2FilterBoard_30Days: {
                header(30);
                break;
            }
            case R.id.ln_PopupVer2FilterBoard_NgayTao_TuNgay: {
                fromDate();
                break;
            }
            case R.id.ln_PopupVer2FilterBoard_NgayTao_DenNgay: {
                toDate();
                break;
            }
            case R.id.img_PopupVer2FilterBoard_TuNgay_Today: {
                lnNgayTao_TuNgay.setBackgroundResource(R.drawable.drawable_popupfilter_notselected);
                imgNgayTao_TuNgay.setColorFilter(ContextCompat.getColor(mainAct, R.color.clGraytitle));
                relaNgayTao_TuNgay.setVisibility(View.GONE);
                tvNgayTao_TuNgay.setText(Functions.share.getToDay("dd/MM/yyyy"));
                tvNgayTao_TuNgay.startAnimation(AnimationController.share.fadeIn(mainAct));
                tuNgay = tvNgayTao_TuNgay.getText().toString();
                break;
            }
            case R.id.img_PopupVer2FilterBoard_DenNgay_Today: {
                lnNgayTao_DenNgay.setBackgroundResource(R.drawable.drawable_popupfilter_notselected);
                imgNgayTao_DenNgay.setColorFilter(ContextCompat.getColor(mainAct, R.color.clGraytitle));
                relaNgayTao_DenNgay.setVisibility(View.GONE);
                tvNgayTao_DenNgay.setText(Functions.share.getToDay("dd/MM/yyyy"));
                tvNgayTao_DenNgay.startAnimation(AnimationController.share.fadeIn(mainAct));
                denNgay = tvNgayTao_DenNgay.getText().toString();
                break;
            }
            case R.id.img_PopupVer2FilterBoard_TuNgay_Delete: {
                relaNgayTao_TuNgay.setVisibility(View.GONE);
                break;
            }
            case R.id.img_PopupVer2FilterBoard_DenNgay_Delete: {
                relaNgayTao_DenNgay.setVisibility(View.GONE);
                break;
            }
            case R.id.img_PopupVer2FilterBoard_TuNgay_Clear: {
                fromDateClear();
                break;
            }
            case R.id.img_PopupVer2FilterBoard_DenNgay_Clear: {
                toDateClear();
                break;
            }
            case R.id.tv_PopupVer2FilterBoard_TinhTrang_TatCa: {
                isAll = !isAll;
                isDone = false;
                isPending = false;
                isReject = false;
                all();
                break;
            }
            case R.id.tv_PopupVer2FilterBoard_TinhTrang_ChoPheDuyet: {
                pending();
                break;
            }
            case R.id.tv_PopupVer2FilterBoard_TinhTrang_DaPheDuyet: {
                done();
                break;
            }
            case R.id.tv_PopupVer2FilterBoard_TinhTrang_TuChoi: {
                reject();
                break;
            }
            case R.id.tv_PopupVer2FilterBoard_MacDinh: {
                clear();
                listener.OnDefaultFilter();
                popupFilter.dismiss();
                break;
            }
            case R.id.tv_PopupVer2FilterBoard_ApDung: {
                apply();
                break;
            }
        }
    }

    private void apply() {
        if (Functions.isNullOrEmpty(tuNgay)) {
            String err = Functions.share.getTitle("TEXT_FROMDATE_EMPTY", "Từ ngày phải có giá trị");
            Utility.share.showAlertWithOnlyOK(err, mainAct);
            return;
        }

        if (Functions.isNullOrEmpty(denNgay)) {
            String err = Functions.share.getTitle("TEXT_TODATE_EMPTY", "Từ ngày phải có giá trị");
            Utility.share.showAlertWithOnlyOK(err, mainAct);
            return;
        }

//        if (tuNgay.contains("/") && denNgay.contains("/")) {
//            long l = Functions.share.formatStringToLong(tuNgay, "dd/MM/yyyy");
//            long l1 = Functions.share.formatStringToLong(denNgay, "dd/MM/yyyy");
//            if (l > l1) {
//                Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("TEXT_DATE_COMPARE1", "Ngày bắt đầu hoạc ngày kết thúc không thể trống, vui lòng chọn lại"),
//                        mainAct);
//                return;
//            }
//        } else {
//            if (tuNgay.isEmpty() || denNgay.isEmpty()) {
//                Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("TEXT_DATE_COMPARE1", "Ngày bắt đầu hoạc ngày kết thúc không thể trống, vui lòng chọn lại"),
//                        mainAct);
//                return;
//            }
//
//
//        }

        if (tuNgay.equals(denNgay)) {
            header(1);
        } else {
            Integer[] status = new Integer[]{};
            if (isDone) {
                status = VarsChildBoard.ApprovedListID;
            } else if (isPending) {
                status = VarsChildBoard.WaitingListID;
            } else if (isReject) {
                status = VarsChildBoard.RejectedListID;
            }

            listener.OnFilterSuccess(tuNgay, denNgay, status);
            popupFilter.dismiss();
        }
    }

    private void all() {
        setTextview_Selected_Filter(tvTatCa);
        setTextview_NotSelected_Filter(tvChoPheDuyet);
        setTextview_NotSelected_Filter(tvDaPheDuyet);
        setTextview_NotSelected_Filter(tvTuChoi);
    }

    private void reject() {
        isReject = !isReject;
        if (isReject) {
            if (isDone && isPending) {
                isAll = true;
                isDone = false;
                isPending = false;
                isReject = false;
                all();
            } else {
                isAll = false;
                setTextview_Selected_Filter(tvTuChoi);
                setTextview_NotSelected_Filter(tvTatCa);
            }
        } else {
            if (!isPending && !isDone) {
                setTextview_Selected_Filter(tvTatCa);
                setTextview_NotSelected_Filter(tvTuChoi);
            } else {
                setTextview_NotSelected_Filter(tvTuChoi);
            }
            isAll = false;
        }
    }

    private void done() {
        isDone = !isDone;
        if (isDone) {
            if (isReject && isPending) {
                isAll = true;
                isDone = false;
                isPending = false;
                isReject = false;
                all();
            } else {
                isAll = false;
                setTextview_Selected_Filter(tvDaPheDuyet);
                setTextview_NotSelected_Filter(tvTatCa);
            }
        } else {
            if (!isPending && isReject) {
                setTextview_Selected_Filter(tvTatCa);
                setTextview_NotSelected_Filter(tvDaPheDuyet);
            } else {
                setTextview_NotSelected_Filter(tvDaPheDuyet);
            }
            isAll = false;
        }
    }

    private void pending() {
        isPending = !isPending;
        if (isPending) {
            if (isDone && isReject) {
                isAll = true;
                isDone = false;
                isPending = false;
                isReject = false;
                all();
            } else {
                isAll = false;
                setTextview_Selected_Filter(tvChoPheDuyet);
                setTextview_NotSelected_Filter(tvTatCa);
            }
        } else {
            if (!isDone && isReject) {
                setTextview_Selected_Filter(tvTatCa);
                setTextview_NotSelected_Filter(tvChoPheDuyet);
            } else {
                setTextview_NotSelected_Filter(tvChoPheDuyet);
            }
            isAll = false;
        }
    }

    @SuppressLint("SetTextI18n")
    private void fromDateClear() {
        lnNgayTao_TuNgay.setBackgroundResource(R.drawable.drawable_popupfilter_notselected);
        imgNgayTao_TuNgay.setColorFilter(ContextCompat.getColor(mainAct, R.color.clGraytitle));
        relaNgayTao_TuNgay.setVisibility(View.GONE);

        tuNgay = "";
        if (isEN) {
            tvNgayTao_TuNgay.setText("From date");
        } else {
            tvNgayTao_TuNgay.setText("Từ ngày");
        }
    }

    @SuppressLint("SetTextI18n")
    private void toDateClear() {
        lnNgayTao_DenNgay.setBackgroundResource(R.drawable.drawable_popupfilter_notselected);
        imgNgayTao_DenNgay.setColorFilter(ContextCompat.getColor(mainAct, R.color.clGraytitle));
        relaNgayTao_DenNgay.setVisibility(View.GONE);

        denNgay = "";
        if (isEN) {
            tvNgayTao_DenNgay.setText("To date");
        } else {
            tvNgayTao_DenNgay.setText("Đến ngày");
        }
    }

    private void toDate() {
        setTextView_DateFilter(0);
        if (relaNgayTao_DenNgay.getVisibility() == View.VISIBLE) {
            relaNgayTao_TuNgay.setVisibility(View.GONE);
            relaNgayTao_DenNgay.setVisibility(View.GONE);
        } else {
            relaNgayTao_TuNgay.setVisibility(View.GONE);
            relaNgayTao_DenNgay.setVisibility(View.VISIBLE);
        }

        if (!Functions.isNullOrEmpty(denNgay)) {
            calendarDenNgay.setDate(Functions.share.formatStringToLong(denNgay));
        } else {
            calendarDenNgay.setDate(Functions.share.formatStringToLong(Functions.share.getToDay("dd/MM/yyyy")));
        }
    }

    private void fromDate() {
        setTextView_DateFilter(0);
        if (relaNgayTao_TuNgay.getVisibility() == View.VISIBLE) {
            relaNgayTao_TuNgay.setVisibility(View.GONE);
            relaNgayTao_DenNgay.setVisibility(View.GONE);
        } else {
            relaNgayTao_TuNgay.setVisibility(View.VISIBLE);
            relaNgayTao_DenNgay.setVisibility(View.GONE);
        }

        if (!Functions.isNullOrEmpty(tuNgay)) {
            calendarTuNgay.setDate(Functions.share.formatStringToLong(tuNgay));
        } else {
            calendarTuNgay.setDate(Functions.share.formatStringToLong(Functions.share.getToDay("dd/MM/yyyy", -30)));
        }
    }

    private void header(int day) {
        String today = "";
        String fromday = "";
        switch (day) {
            case 1: {
                isToday = true;
                isYesterday = false;
                is7Days = false;
                isLastMonth = false;

                fromday = Functions.share.getToDay("dd/MM/yyyy");
                today = Functions.share.getToDay("dd/MM/yyyy", 1);
                break;
            }
            case -1: {
                isToday = false;
                isYesterday = true;
                is7Days = false;
                isLastMonth = false;
                fromday = Functions.share.getToDay("dd/MM/yyyy", day);
                today = Functions.share.getToDay("dd/MM/yyyy");
                break;
            }
            case 7: {
                isToday = false;
                isYesterday = false;
                is7Days = true;
                isLastMonth = false;
                fromday = Functions.share.getToDay("dd/MM/yyyy", -day);
                today = Functions.share.getToDay("dd/MM/yyyy");
                break;
            }
            case 30: {
                isToday = false;
                isYesterday = false;
                is7Days = false;
                isLastMonth = true;
                fromday = Functions.share.getToDay("dd/MM/yyyy", -day);
                today = Functions.share.getToDay("dd/MM/yyyy");
                break;
            }
        }

        Integer[] status = new Integer[]{};
        if (isDone) {
            status = VarsChildBoard.ApprovedListID;
        } else if (isPending) {
            status = VarsChildBoard.WaitingListID;
        } else if (isReject) {
            status = VarsChildBoard.RejectedListID;
        }

        isFirst = false;
        listener.OnHeaderDay(fromday, today, status);
        uncheckViewTuNgayDenNgay();
        popupFilter.dismiss();
    }

    private void uncheckViewTuNgayDenNgay() {
        imgNgayTao_TuNgay.setColorFilter(ContextCompat.getColor(mainAct, R.color.clGraytitle));
        imgNgayTao_DenNgay.setColorFilter(ContextCompat.getColor(mainAct, R.color.clGraytitle));
        lnNgayTao_TuNgay.setBackgroundResource(R.drawable.textcornerstrokegray);
        lnNgayTao_DenNgay.setBackgroundResource(R.drawable.textcornerstrokegray);
        relaNgayTao_TuNgay.setVisibility(View.GONE);
        relaNgayTao_DenNgay.setVisibility(View.GONE);
    }

    private void setTextView_DateFilter(int type) {
        switch (type) {
            default:
            case 0: {
                tvToday.setTextColor(ContextCompat.getColor(mainAct, R.color.clBottomDisable));
                tvYesterday.setTextColor(ContextCompat.getColor(mainAct, R.color.clBottomDisable));
                tv7Days.setTextColor(ContextCompat.getColor(mainAct, R.color.clBottomDisable));
                tv30Days.setTextColor(ContextCompat.getColor(mainAct, R.color.clBottomDisable));

                tvToday.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.NORMAL);
                tvYesterday.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.NORMAL);
                tv7Days.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.NORMAL);
                tv30Days.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.NORMAL);
                break;
            }
            case 1: {
                tvToday.setTextColor(ContextCompat.getColor(mainAct, R.color.clVer2BlueMain));
                tvYesterday.setTextColor(ContextCompat.getColor(mainAct, R.color.clBottomDisable));
                tv7Days.setTextColor(ContextCompat.getColor(mainAct, R.color.clBottomDisable));
                tv30Days.setTextColor(ContextCompat.getColor(mainAct, R.color.clBottomDisable));

                tvToday.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.BOLD);
                tvYesterday.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.NORMAL);
                tv7Days.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.NORMAL);
                tv30Days.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.NORMAL);
                break;
            }
            case 2: {
                tvToday.setTextColor(ContextCompat.getColor(mainAct, R.color.clBottomDisable));
                tvYesterday.setTextColor(ContextCompat.getColor(mainAct, R.color.clVer2BlueMain));
                tv7Days.setTextColor(ContextCompat.getColor(mainAct, R.color.clBottomDisable));
                tv30Days.setTextColor(ContextCompat.getColor(mainAct, R.color.clBottomDisable));

                tvToday.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.NORMAL);
                tvYesterday.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.BOLD);
                tv7Days.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.NORMAL);
                tv30Days.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.NORMAL);
                break;
            }
            case 3: {
                tvToday.setTextColor(ContextCompat.getColor(mainAct, R.color.clBottomDisable));
                tvYesterday.setTextColor(ContextCompat.getColor(mainAct, R.color.clBottomDisable));
                tv7Days.setTextColor(ContextCompat.getColor(mainAct, R.color.clVer2BlueMain));
                tv30Days.setTextColor(ContextCompat.getColor(mainAct, R.color.clBottomDisable));

                tvToday.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.NORMAL);
                tvYesterday.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.NORMAL);
                tv7Days.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.BOLD);
                tv30Days.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.NORMAL);
                break;
            }
            case 4: {
                tvToday.setTextColor(ContextCompat.getColor(mainAct, R.color.clBottomDisable));
                tvYesterday.setTextColor(ContextCompat.getColor(mainAct, R.color.clBottomDisable));
                tv7Days.setTextColor(ContextCompat.getColor(mainAct, R.color.clBottomDisable));
                tv30Days.setTextColor(ContextCompat.getColor(mainAct, R.color.clVer2BlueMain));

                tvToday.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.NORMAL);
                tvYesterday.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.NORMAL);
                tv7Days.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.NORMAL);
                tv30Days.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.BOLD);
                break;
            }
        }
    }

    private void setTextview_Selected_Filter(TextView tv) {
        tv.startAnimation(AnimationController.share.fadeIn(mainAct));
        tv.setBackgroundResource(R.drawable.textcornerviolet2);
        tv.setBackgroundTintList(ColorStateList.valueOf(ContextCompat.getColor(mainAct, R.color.clVer2BlueBackground)));
        tv.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.BOLD);
        tv.setTextColor(ContextCompat.getColor(mainAct, R.color.clVer2BlueMain));
    }

    private void setTextview_NotSelected_Filter(TextView tv) {
        tv.setBackgroundResource(R.drawable.textcornergray);
        tv.setBackgroundTintList(null);
        tv.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.NORMAL);
        tv.setTextColor(ContextCompat.getColor(mainAct, R.color.clBottomDisable));
    }
}

package com.vuthao.bpmop.task.child;

import androidx.annotation.Nullable;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.recyclerview.widget.StaggeredGridLayoutManager;

import android.app.Dialog;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.graphics.Color;
import android.graphics.drawable.ColorDrawable;
import android.net.Uri;
import android.os.Bundle;
import android.text.Editable;
import android.text.Html;
import android.text.InputFilter;
import android.text.TextWatcher;
import android.util.DisplayMetrics;
import android.util.Log;
import android.view.Gravity;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.DatePicker;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.TimePicker;

import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.AnimationController;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.CalendarUltis;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.DateTimeUtility;
import com.vuthao.bpmop.base.DownloadFile;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.SwipeHelper;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.custom.CustomFlexBoxRecyclerView;
import com.vuthao.bpmop.base.custom.editor.RichEditor;
import com.vuthao.bpmop.base.custom.flexbox.FlexDirection;
import com.vuthao.bpmop.base.custom.flexbox.FlexboxLayoutManager;
import com.vuthao.bpmop.base.custom.flexbox.JustifyContent;
import com.vuthao.bpmop.base.model.custom.DetailTask;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.base.keyboard.KeyboardManager;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.custom.AttachFile;
import com.vuthao.bpmop.base.model.custom.LookupData;
import com.vuthao.bpmop.base.model.custom.Task;
import com.vuthao.bpmop.base.model.custom.UserAndGroup;
import com.vuthao.bpmop.base.realm.WorkflowController;
import com.vuthao.bpmop.core.Vars;
import com.vuthao.bpmop.detail.adapter.SelectUserGroupMultipleAdapter;
import com.vuthao.bpmop.detail.custom.DetailFunc;
import com.vuthao.bpmop.shareview.SharedView_PopupChooseFile;
import com.vuthao.bpmop.shareview.adapter.FormControlSingleChoiceAdapter;
import com.vuthao.bpmop.shareview.adapter.SelectUserGroupMultiple_Text2Adapter;
import com.vuthao.bpmop.shareview.adapter.SelectUserGroupMultiple_TextAdapter;
import com.vuthao.bpmop.task.FuncDetailCreateTask;
import com.vuthao.bpmop.task.adapter.CreateTaskAttachmentAdapter;
import com.vuthao.bpmop.task.presenter.DetailCreateTaskPresenter;

import java.io.File;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.List;
import java.util.stream.Collectors;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;

public class CreateChildTaskActivity extends BaseActivity implements DetailCreateTaskPresenter.DetailCreateTaskListener {
    @BindView(R.id.ln_ViewDetailCreateTask_All)
    LinearLayout lnAll;
    @BindView(R.id.img_ViewDetailCreateTask_Back)
    ImageView imgBack;
    @BindView(R.id.img_ViewDetailCreateTask_Done)
    ImageView imgSave;
    @BindView(R.id.tv_ViewDetailCreateTask_Name)
    TextView tvName;
    @BindView(R.id.img_ViewDetailCreateTask_Name)
    ImageView imgTitleName;
    @BindView(R.id.ln_ViewDetailCreateTask_Data)
    LinearLayout lnData;
    @BindView(R.id.ln_ViewDetailCreateTask_NoData)
    LinearLayout lnNoData;
    @BindView(R.id.ln_ViewDetailCreateTask_ChiTiet_Content)
    LinearLayout lnChiTietContent;
    @BindView(R.id.tv_ViewDetailCreateTask_NoData)
    TextView tvNoData;
    @BindView(R.id.tv_ViewDetailCreateTask_TieuDe)
    TextView tvTieuDe;
    @BindView(R.id.ln_ViewDetailCreateTask_TieuDe_Content)
    LinearLayout lnTieuDeContent;
    @BindView(R.id.tv_ViewDetailCreateTask_TieuDe_Content)
    TextView tvTieuDeContent;
    @BindView(R.id.tv_ViewDetailCreateTask_NguoiXuLy)
    TextView tvNguoiXuLy;
    @BindView(R.id.ln_ViewDetailCreateTask_NguoiXuLy_Content)
    LinearLayout lnNguoiXuLyContent;
    @BindView(R.id.recy_ViewDetailCreateTask_NguoiXuLy)
    CustomFlexBoxRecyclerView recyNguoiXuLy;
    @BindView(R.id.img_ViewDetailCreateTask_NguoiXuLy)
    ImageView imgNguoiXuLy;
    @BindView(R.id.tv_ViewDetailCreateTask_HanHoanTat)
    TextView tvHanHoanTat;
    @BindView(R.id.img_ViewDetailCreateTask_HanHoanTat_Content)
    ImageView imgHanHoanTat;
    @BindView(R.id.ln_ViewDetailCreateTask_HanHoanTat_Content)
    LinearLayout lnHanHoanTatContent;
    @BindView(R.id.tv_ViewDetailCreateTask_HanHoanTat_Content)
    TextView tvHanHoanTatContent;
    @BindView(R.id.tv_ViewDetailCreateTask_TinhTrang)
    TextView tvTinhTrang;
    @BindView(R.id.ln_ViewDetailCreateTask_TinhTrang_Content)
    LinearLayout lnTinhTrangContent;
    @BindView(R.id.tv_ViewDetailCreateTask_TinhTrang_Content)
    TextView tvTinhTrangContent;
    @BindView(R.id.ln_ViewDetailCreateTask_NguoiGiao)
    LinearLayout lnNguoiGiao;
    @BindView(R.id.tv_ViewDetailCreateTask_NguoiGiao)
    TextView tvNguoiGiao;
    @BindView(R.id.tv_ViewDetailCreateTask_NguoiGiao_Content)
    TextView tvNguoiGiaoContent;
    @BindView(R.id.tv_ViewDetailCreateTask_NoiDung)
    TextView tvNoiDung;
    @BindView(R.id.ln_ViewDetailCreateTask_NoiDung_Content)
    LinearLayout lnNoiDungContent;
    @BindView(R.id.ln_ViewDetailCreateTask_NoiDung_ContentClick)
    LinearLayout lnNoiDungContentClick;
    @BindView(R.id.richEditor_ViewDetailCreateTask_NoiDung_Content)
    RichEditor richEditorNoiDung;
    @BindView(R.id.recy_ViewDetailCreateTask_AttachFile)
    RecyclerView recyAttachFile;
    @BindView(R.id.ln_ViewDetailCreateTask_AttachFile)
    LinearLayout lnAttachFile;
    @BindView(R.id.ln_ViewDetailCreateTask_AttachFile_TaoMoi)
    LinearLayout lnTaoMoi;
    @BindView(R.id.tv_ViewDetailCreateTask_AttachFile)
    TextView tvAttachFile;
    @BindView(R.id.tv_ViewDetailCreateTask_AttachFile_TaoMoi)
    TextView tvAttachFileTaoMoi;
    @BindView(R.id.tv_ViewDetailCreateTask_AttachFile_Child1)
    TextView tvAttachFileChild1;
    @BindView(R.id.tv_ViewDetailCreateTask_AttachFile_Child2)
    TextView tvAttachFileChild2;
    @BindView(R.id.ln_ViewDetailCreateTask_Comment)
    LinearLayout lnComment;

    private AnimationController animationController;
    private ArrayList<UserAndGroup> lstCurrentUserGroup;
    private ArrayList<AttachFile> lstAttachFileFull;
    private ArrayList<AttachFile> lstAttFileControl_Deleted;
    private ArrayList<UserAndGroup> lstUserAndGroupAll;
    private ArrayList<UserAndGroup> lstSelected;
    private SelectUserGroupMultiple_Text2Adapter adapterListUserText;
    private SelectUserGroupMultipleAdapter adapterListUser;
    private CreateTaskAttachmentAdapter adapterAttachment;
    private SelectUserGroupMultiple_TextAdapter adapterListUserSelected;
    private WorkflowController workflowController;
    private DetailCreateTaskPresenter presenter;
    private Task parentItem;
    private boolean isClickFromAction;
    private int flagUserPermission;
    private Dialog dialog;
    private boolean isChange = true;
    public Uri uri;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_create_child_task);
        ButterKnife.bind(this);

        init();
        setTitle();
        setData();
    }

    private void init() {
        presenter = new DetailCreateTaskPresenter(this);
        workflowController = new WorkflowController();
        lstCurrentUserGroup = new ArrayList<>();
        lstAttachFileFull = new ArrayList<>();
        lstAttFileControl_Deleted = new ArrayList<>();
        lstUserAndGroupAll = new ArrayList<>();
        lstSelected = new ArrayList<>();

        imgTitleName.setVisibility(View.GONE);
        lnTinhTrangContent.setVisibility(View.INVISIBLE);
        tvTinhTrang.setVisibility(View.INVISIBLE);
        lnNguoiGiao.setVisibility(View.GONE);

        richEditorNoiDung.setEnabled(false);

        flagUserPermission = FuncDetailCreateTask.FlagUserPermission.Creator;

        Bundle bundle = getIntent().getExtras();
        parentItem = new Gson().fromJson(bundle.getString("task"), Task.class);
        isClickFromAction = bundle.getBoolean("isClickFromAction");

        registerReceiver();
    }

    private void setTitle() {
        animationController = new AnimationController();
        tvName.setText(Functions.share.getTitle("TEXT_ASSIGNTASK", "Phân công công việc"));
        tvTieuDe.setText(Functions.share.getTitle("TEXT_TITLE_REQUIRE", "Tiêu đề (*)"));
        tvNguoiXuLy.setText(Functions.share.getTitle("TEXT_USER_PROCESS_REQUIRE", "Người xử lý (*)"));
        tvHanHoanTat.setText(Functions.share.getTitle("TEXT_DUEDATE", "Hạn hoàn tất"));
        tvTinhTrang.setText(Functions.share.getTitle("TEXT_STATUS", "Tình trạng"));
        tvNoiDung.setText(Functions.share.getTitle("TEXT_CONTENT", "Nội dung"));
        tvAttachFile.setText(Functions.share.getTitle("TEXT_ATTACHMENT", "Tài liệu đính kèm"));
        tvAttachFileTaoMoi.setText(Functions.share.getTitle("TEXT_ADDNEW_ATTACHMENT", "Tạo mới"));
        tvAttachFileChild1.setText(Functions.share.getTitle("TEXT_CONTROL_DOCUMENTNAME", "Tên tài liệu"));
        tvAttachFileChild2.setText(Functions.share.getTitle("TEXT_ASSIGNORS", "Người giao"));
        tvNguoiGiao.setText(Functions.share.getTitle("TEXT_CREATOR", "Người giao"));
        tvNoData.setText(Functions.share.getTitle("TEXT_NODATA", "Không có dữ liệu"));

        Functions.share.setTVHighlightControl(getApplicationContext(), tvTieuDe);
        Functions.share.setTVHighlightControl(getApplicationContext(), tvNguoiXuLy);
    }

    @OnClick({R.id.ln_ViewDetailCreateTask_All, R.id.img_ViewDetailCreateTask_Back
            , R.id.ln_ViewDetailCreateTask_HanHoanTat_Content, R.id.ln_ViewDetailCreateTask_NguoiXuLy_Content
            , R.id.ln_ViewDetailCreateTask_NoiDung_ContentClick, R.id.ln_ViewDetailCreateTask_TieuDe_Content
            , R.id.ln_ViewDetailCreateTask_TinhTrang_Content, R.id.ln_ViewDetailCreateTask_AttachFile_TaoMoi,
            R.id.img_ViewDetailCreateTask_Done})
    public void OnViewClicked(View v) {
        switch (v.getId()) {
            case R.id.ln_ViewDetailCreateTask_All: {
                break;
            }
            case R.id.img_ViewDetailCreateTask_Back: {
                imgBack.startAnimation(animationController.fadeIn(getApplicationContext()));
                BroadcastUtility.unregister(this, mReceiver);
                finish();
                break;
            }
            case R.id.ln_ViewDetailCreateTask_HanHoanTat_Content: {
                duedate();
                break;
            }
            case R.id.ln_ViewDetailCreateTask_NguoiXuLy_Content: {
                assigned();
                break;
            }
            case R.id.ln_ViewDetailCreateTask_NoiDung_ContentClick: {
                content();
                break;
            }
            case R.id.ln_ViewDetailCreateTask_TieuDe_Content: {
                title();
                break;
            }
            case R.id.ln_ViewDetailCreateTask_TinhTrang_Content: {
                status();
                break;
            }
            case R.id.ln_ViewDetailCreateTask_AttachFile_TaoMoi: {
                SharedView_PopupChooseFile popupChooseFile = new SharedView_PopupChooseFile(getLayoutInflater(), this, "DetailCreateTask", lnAll,
                        Constants.mTaskChildAttachment, Constants.mTaskChildAttachmentCamera,
                        Vars.FlagView.DetailCreateTask_Child_ControlInputAttachmentVertical);
                popupChooseFile.initializeView();
                break;
            }
            case R.id.img_ViewDetailCreateTask_Done: {
                save();
                break;
            }
        }
    }

    private void save() {
        if (Functions.isNullOrEmpty(tvTieuDeContent.getText().toString())) {
            Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("MESS_REQUIRE_TITLE", "Please enter title")
                    , Functions.share.getTitle("TEXT_CLOSE", "Close"), this);
            return;
        }

        if (lstCurrentUserGroup.isEmpty()) {
            Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("MESS_REQUIRE_USERGROUP", "Vui lòng chọn người để thực hiện.")
                    , Functions.share.getTitle("TEXT_CLOSE", "Close"), this);
            return;
        }

        showProgressDialog();
        ArrayList<AttachFile> lstAttachmentLocal = new ArrayList<>();
        if (!lstAttachFileFull.isEmpty()) {
            for (int i = 0; i < lstAttachFileFull.size(); i++) {
                if (lstAttachFileFull.get(i).getID().equals("")) {
                    lstAttachmentLocal.add(lstAttachFileFull.get(i));
                }
            }
        }

        Task task = new Task();
        task.setID(0);
        task.setTitle(tvTieuDeContent.getText().toString());
        if (!Functions.isNullOrEmpty(tvHanHoanTatContent.getText().toString())) {
            String format = DateTimeUtility.getFormat("task");
            long l;
            if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
                l = Functions.share.formatStringToLong(tvHanHoanTatContent.getText().toString(), format);
            } else {
                l = Functions.share.formatStringToLong(tvHanHoanTatContent.getText().toString(), format);
            }

            long currentDay = Functions.share.formatStringToLong(Functions.share.getToDay(format), format);
            if (DateTimeUtility.isAfterDay(currentDay, l)) {
                Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("TEXT_DATE_COMPARE2", "Hạn hoàn tất không được nhỏ hơn thời gian hiện tại!"),
                        Functions.share.getTitle("TEXT_CLOSE", "Close"), CreateChildTaskActivity.this);
                return;
            } else {

                long lDate = Functions.share.formatStringToLong(tvHanHoanTatContent.getText().toString(), "dd/MM/yy HH:mm");
                String newDueDate = Functions.share.formatLongToDay(lDate, "yyyy-MM-dd HH:mm");
                task.setDueDate(newDueDate);
            }
        } else {
            task.setDueDate(null);
        }

        task.setParent(parentItem.getID());
        task.setWorkflowId(parentItem.getWorkflowId());
        task.setSPItemId(parentItem.getSPItemId());
        task.setStep(parentItem.getStep());
        task.setContent(!Functions.isNullOrEmpty(richEditorNoiDung.getHtml()) ? richEditorNoiDung.getHtml() : "");
        task.setStatus(0);

        presenter.sendCreateTaskAction(task, lstCurrentUserGroup, new ArrayList<>(), lstAttachmentLocal, FuncDetailCreateTask.FlagActionPermission.CreateNew);
    }

    private void status() {
        ArrayList<LookupData> status = presenter.getListLookupStatus();
        for (int i = 0; i < status.size(); i++) {
            if (status.get(i).getTitle().contains(tvTinhTrangContent.getText().toString())) {
                status.get(i).setSelected(true);
                break;
            }
        }

        View view = getLayoutInflater().inflate(R.layout.popup_control_single_choice, null);
        ImageView imgClose = view.findViewById(R.id.img_PopupControl_SingleChoice_Close);
        TextView tvTitle = view.findViewById(R.id.tv_PopupControl_SingleChoice_Title);
        RecyclerView recyData = view.findViewById(R.id.recy_PopupControl_SingleChoice_Data);
        ImageView imgDone = view.findViewById(R.id.img_PopupControl_SingleChoice_Done);

        imgDone.setVisibility(View.INVISIBLE);

        if (!Functions.isNullOrEmpty(tvTinhTrangContent.getText().toString())) {
            tvTitle.setText(tvTinhTrangContent.getText().toString());
        } else {
            tvTitle.setText("");
        }

        FormControlSingleChoiceAdapter adapterFormControlSingleChoice = new FormControlSingleChoiceAdapter(this, getApplicationContext(), status, new FormControlSingleChoiceAdapter.FormControlSingleChoiceListener() {
            @Override
            public void OnClick(int pos) {
                LookupData selectedLookupItem = status.get(pos);
                if (selectedLookupItem != null) {
                    parentItem.setStatus(Integer.parseInt(selectedLookupItem.getID()));
                    tvTinhTrangContent.setText(selectedLookupItem.getTitle());
                }

                dialog.dismiss();
            }
        });

        StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, StaggeredGridLayoutManager.VERTICAL);
        recyData.setAdapter(adapterFormControlSingleChoice);
        recyData.setLayoutManager(staggeredGridLayoutManager);

        imgClose.setOnClickListener(v -> dialog.dismiss());

        // region Dialog
        dialog = new Dialog(this, R.style.Theme_Custom_BPMOP_Dialog_FullScreen);
        Window window = dialog.getWindow();
        dialog.requestWindowFeature(1);
        dialog.setCanceledOnTouchOutside(false);
        dialog.setCancelable(true);
        window.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_ADJUST_PAN);
        window.setGravity(Gravity.BOTTOM);
        dialog.setContentView(view);
        dialog.show();

        WindowManager.LayoutParams s = window.getAttributes();
        s.width = WindowManager.LayoutParams.MATCH_PARENT;
        s.height = WindowManager.LayoutParams.MATCH_PARENT;
        window.setAttributes(s);
        // endregion
    }

    private void content() {
        if (lnNoiDungContent.isEnabled()) {
            View view = getLayoutInflater().inflate(R.layout.popup_control_text_input_format, null);
            ImageView imgBack = view.findViewById(R.id.img_PopupControl_InputTextFormat_Close);
            TextView tvTitle = view.findViewById(R.id.tv_PopupControl_InputTextFormat_Title);
            ImageView imgDone = view.findViewById(R.id.img_PopupControl_InputTextFormat_Done);
            ImageView imgDelete = view.findViewById(R.id.img_PopupControl_InputTextFormat_Delete);
            RichEditor mEditor = view.findViewById(R.id.editor_PopupControl_InputTextFormat);

            //region Rich Editor
            ImageView undo = view.findViewById(R.id.img_PopupControl_InputTextFormat_undo);
            ImageView redo = view.findViewById(R.id.img_PopupControl_InputTextFormat_redo);
            ImageView bold = view.findViewById(R.id.img_PopupControl_InputTextFormat_bold);
            ImageView italic = view.findViewById(R.id.img_PopupControl_InputTextFormat_italic);
            ImageView subscript = view.findViewById(R.id.img_PopupControl_InputTextFormat_subscript);
            ImageView superscript = view.findViewById(R.id.img_PopupControl_InputTextFormat_superscript);
            ImageView strikethrough = view.findViewById(R.id.img_PopupControl_InputTextFormat_strikethrough);
            ImageView underline = view.findViewById(R.id.img_PopupControl_InputTextFormat_underline);
            ImageView heading1 = view.findViewById(R.id.img_PopupControl_InputTextFormat_heading1);
            ImageView heading2 = view.findViewById(R.id.img_PopupControl_InputTextFormat_heading2);
            ImageView heading3 = view.findViewById(R.id.img_PopupControl_InputTextFormat_heading3);
            ImageView heading4 = view.findViewById(R.id.img_PopupControl_InputTextFormat_heading4);
            ImageView heading5 = view.findViewById(R.id.img_PopupControl_InputTextFormat_heading5);
            ImageView heading6 = view.findViewById(R.id.img_PopupControl_InputTextFormat_heading6);
            ImageView txt_color = view.findViewById(R.id.img_PopupControl_InputTextFormat_txt_color);
            ImageView bg_color = view.findViewById(R.id.img_PopupControl_InputTextFormat_bg_color);
            ImageView indent = view.findViewById(R.id.img_PopupControl_InputTextFormat_indent);
            ImageView outdent = view.findViewById(R.id.img_PopupControl_InputTextFormat_outdent);
            ImageView left = view.findViewById(R.id.img_PopupControl_InputTextFormat_align_left);
            ImageView center = view.findViewById(R.id.img_PopupControl_InputTextFormat_align_center);
            ImageView right = view.findViewById(R.id.img_PopupControl_InputTextFormat_align_right);
            ImageView blockquote = view.findViewById(R.id.img_PopupControl_InputTextFormat_blockquote);
            ImageView bullets = view.findViewById(R.id.img_PopupControl_InputTextFormat_insert_bullets);
            ImageView numbers = view.findViewById(R.id.img_PopupControl_InputTextFormat_insert_numbers);
            ImageView image = view.findViewById(R.id.img_PopupControl_InputTextFormat_insert_image);
            ImageView link = view.findViewById(R.id.img_PopupControl_InputTextFormat_insert_link);
            ImageView checkbox = view.findViewById(R.id.img_PopupControl_InputTextFormat_insert_checkbox);
            //endregion

            imgDelete.setVisibility(View.GONE);
            if (!Functions.isNullOrEmpty(tvNoiDung.getText().toString())) {
                tvTitle.setText(tvNoiDung.getText().toString());
            } else {
                tvTitle.setText("");
            }

            if (!Functions.isNullOrEmpty(richEditorNoiDung.getHtml())) {
                mEditor.setHtml(richEditorNoiDung.getHtml());
            }

            imgBack.setOnClickListener(v -> {
                KeyboardManager.hideKeyboard(CreateChildTaskActivity.this);
                dialog.dismiss();
            });

            imgDelete.setOnClickListener(v -> {
                mEditor.setHtml("");
                imgDone.performClick();
            });

            imgDone.setOnClickListener(v -> {
                String result = "";
                if (!Functions.isNullOrEmpty(mEditor.getHtml())) {
                    result = mEditor.getHtml();
                }

                richEditorNoiDung.setHtml(result);
                KeyboardManager.hideKeyboard(CreateChildTaskActivity.this);
                dialog.dismiss();
            });

            // region Editor
            undo.setOnClickListener(v -> mEditor.undo());

            underline.setOnClickListener(v -> mEditor.setUnderline());

            redo.setOnClickListener(v -> mEditor.redo());

            bold.setOnClickListener(v -> mEditor.setBold());

            italic.setOnClickListener(v -> mEditor.setItalic());

            subscript.setOnClickListener(v -> mEditor.setSubscript());

            strikethrough.setOnClickListener(v -> mEditor.setStrikeThrough());

            superscript.setOnClickListener(v -> mEditor.setSuperscript());

            heading1.setOnClickListener(v -> mEditor.setHeading(1));
            heading2.setOnClickListener(v -> mEditor.setHeading(2));
            heading3.setOnClickListener(v -> mEditor.setHeading(3));
            heading4.setOnClickListener(v -> mEditor.setHeading(4));
            heading5.setOnClickListener(v -> mEditor.setHeading(5));
            heading6.setOnClickListener(v -> mEditor.setHeading(6));
            txt_color.setOnClickListener(v -> {
                mEditor.setTextColor(isChange ? Color.BLACK : Color.RED);
                isChange = !isChange;
            });
            bg_color.setOnClickListener(v -> {
                mEditor.setBackgroundColor(isChange ? Color.TRANSPARENT : Color.YELLOW);
                isChange = !isChange;
            });
            indent.setOnClickListener(v -> mEditor.setIndent());
            outdent.setOnClickListener(v -> mEditor.setOutdent());
            left.setOnClickListener(v -> mEditor.setAlignLeft());
            center.setOnClickListener(v -> mEditor.setAlignCenter());
            right.setOnClickListener(v -> mEditor.setAlignRight());
            blockquote.setOnClickListener(v -> mEditor.setBlockquote());
            bullets.setOnClickListener(v -> mEditor.setBullets());
            numbers.setOnClickListener(v -> mEditor.setNumbers());
            image.setOnClickListener(v -> mEditor.insertImage("http://www.1honeywan.com/dachshund/image/7.21/7.21_3_thumb.JPG", "dachshund"));
            link.setOnClickListener(v -> mEditor.insertLink("https://github.com/wasabeef", "wasabeef"));
            checkbox.setOnClickListener(v -> mEditor.insertTodo());

            //endregion

            // region Dialog
            dialog = new Dialog(this, R.style.Theme_Custom_BPMOP_Dialog_FullScreen);
            Window window = dialog.getWindow();
            dialog.requestWindowFeature(1);
            dialog.setCanceledOnTouchOutside(false);
            dialog.setCancelable(true);
            window.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_ADJUST_PAN);
            window.setGravity(Gravity.BOTTOM);
            dialog.setContentView(view);
            dialog.show();

            WindowManager.LayoutParams s = window.getAttributes();
            s.width = WindowManager.LayoutParams.MATCH_PARENT;
            s.height = WindowManager.LayoutParams.MATCH_PARENT;
            window.setAttributes(s);
            // endregion
        } else {
            View view = getLayoutInflater().inflate(R.layout.popup_control_text_input_format_disable, null);
            TextView tvTitle = view.findViewById(R.id.tv_PopupControl_InputTextFormat_Title);
            TextView txtDetailInDisable = view.findViewById(R.id.txtDetailInDisable);
            ImageView imgBack = view.findViewById(R.id.img_PopupControl_InputTextFormat_Close);

            if (!Functions.isNullOrEmpty(tvNoiDung.getText().toString())) {
                tvTitle.setText(tvNoiDung.getText().toString());
            } else {
                tvTitle.setText("");
            }

            txtDetailInDisable.setText(Html.fromHtml(richEditorNoiDung.getHtml(), Html.FROM_HTML_MODE_LEGACY));
            imgBack.setOnClickListener(v -> dialog.dismiss());

            // region Dialog
            dialog = new Dialog(this, R.style.Theme_Custom_BPMOP_Dialog_FullScreen);
            Window window = dialog.getWindow();
            dialog.requestWindowFeature(1);
            dialog.setCanceledOnTouchOutside(false);
            dialog.setCancelable(true);
            window.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_ADJUST_PAN);
            window.setGravity(Gravity.BOTTOM);
            dialog.setContentView(view);
            dialog.show();

            WindowManager.LayoutParams s = window.getAttributes();
            s.width = WindowManager.LayoutParams.MATCH_PARENT;
            s.height = WindowManager.LayoutParams.MATCH_PARENT;
            window.setAttributes(s);
            // endregion
        }
    }

    private void duedate() {
        View view = getLayoutInflater().inflate(R.layout.popup_control_datetime_picker, null);
        DatePicker datePicker = view.findViewById(R.id.dp_PopupControl_DateTimePicker);
        TimePicker timePicker = view.findViewById(R.id.tp_PopupControl_DateTimePicker);
        ImageView imgDelete = view.findViewById(R.id.img_PopupControl_DateTimePicker_Delete);
        ImageView imgToday = view.findViewById(R.id.img_PopupControl_DateTimePicker_Today);
        ImageView imgClose = view.findViewById(R.id.img_PopupControl_DateTimePicker_Close);
        TextView tvTitle = view.findViewById(R.id.tv_PopupControl_DateTimePicker_Title);
        LinearLayout lnApply = view.findViewById(R.id.ln_PopupControl_DateTimePicker_Clear);
        TextView tvApply = view.findViewById(R.id.tv_PopupControl_DateTimePicker_Apply);

        timePicker.setIs24HourView(Boolean.TRUE);
        tvApply.setText(Functions.share.getTitle("TEXT_APPLY", "Áp dụng"));
        CalendarUltis calendar = null;

        if (tvHanHoanTatContent.getText().toString().length() > 0) {
            calendar = new CalendarUltis(tvHanHoanTatContent.getText().toString(), DateTimeUtility.getFormat("task"));
            tvTitle.setText(tvHanHoanTatContent.getText().toString());
        } else {
            calendar = new CalendarUltis();
            tvTitle.setText(FuncDetailCreateTask.getToDayTask());
        }

        datePicker.init(calendar.YEAR, calendar.MONTH, calendar.DAY_OF_MONTH, null);
        timePicker.setHour(calendar.HOUR);
        timePicker.setMinute(calendar.MINUTE);

        //region Dialog
        dialog = new Dialog(this);
        Window window = dialog.getWindow();
        dialog.requestWindowFeature(1);
        dialog.setCanceledOnTouchOutside(false);
        dialog.setCancelable(true);
        window.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_ADJUST_PAN);
        window.setGravity(Gravity.CENTER);
        DisplayMetrics displayMetrics = getResources().getDisplayMetrics();
        dialog.setContentView(view);
        dialog.show();

        WindowManager.LayoutParams s = window.getAttributes();
        s.width = displayMetrics.widthPixels;
        s.height = WindowManager.LayoutParams.WRAP_CONTENT;
        window.setAttributes(s);
        window.setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));
        //endregion

        imgToday.setOnClickListener(v -> {
            CalendarUltis c = new CalendarUltis();
            datePicker.init(c.YEAR, c.MONTH, c.DAY_OF_MONTH, null);
            timePicker.setHour(c.HOUR);
            timePicker.setMinute(c.MINUTE);
        });

        imgDelete.setOnClickListener(v -> {
            tvHanHoanTatContent.setText("");
            dialog.dismiss();
        });

        lnApply.setOnClickListener(v -> {
            int day = datePicker.getDayOfMonth();
            int month = datePicker.getMonth();
            int year = datePicker.getYear();
            int hour = timePicker.getHour();
            int minute = timePicker.getMinute();

            Calendar calendar1 = Calendar.getInstance();
            calendar1.set(year, month, day, hour, minute);
            String result = Functions.share.formatLongToString(calendar1.getTimeInMillis(), Constants.mDateApi);
            String date = Functions.share.formatDateLanguage(result);
            tvHanHoanTatContent.setText(date);
            dialog.dismiss();
        });

        imgClose.setOnClickListener(v -> dialog.dismiss());
    }

    private void assigned() {
        View viewPopupControl = getLayoutInflater().inflate(R.layout.popup_control_choose_user, null);
        ImageView imgCloseChooseUser = viewPopupControl.findViewById(R.id.img_PopupControl_ChooseUser_Close);
        ImageView imgAcceptChooseUser = viewPopupControl.findViewById(R.id.img_PopupControl_ChooseUser_Accept);
        ImageView imgDeleteChooseUser = viewPopupControl.findViewById(R.id.img_PopupControl_ChooseUser_Delete);
        TextView tvTitleChooseUser = viewPopupControl.findViewById(R.id.tv_PopupControl_ChooseUser_Title);
        EditText edtSearchChooseUser = viewPopupControl.findViewById(R.id.edt_PopupControl_ChooseUser_Search);
        RecyclerView recyChooseUser = viewPopupControl.findViewById(R.id.recy_PopupControl_ChooseUser);
        CustomFlexBoxRecyclerView recySelectedUser = viewPopupControl.findViewById(R.id.recy_PopupControl_SelectedUser);

        recySelectedUser.setVisibility(View.VISIBLE);
        imgAcceptChooseUser.setVisibility(View.VISIBLE);
        imgDeleteChooseUser.setVisibility(View.VISIBLE);
        recySelectedUser.setMaxRowAndRowHeight(Functions.share.convertDpToPixel(35, this), 3);

        tvTitleChooseUser.setText(Functions.share.getTitle("TEXT_TITLE_USERGROUP", "Chọn người hoặc nhóm"));
        edtSearchChooseUser.setHint(Functions.share.getTitle("MESS_REQUIRE_USERGROUP", "Vui lòng chọn người hoặc nhóm để thực hiện"));

        lstUserAndGroupAll = workflowController.getUserAndGroup();
        if (!lstUserAndGroupAll.isEmpty() && !lstSelected.isEmpty()) {
            for (int i = 0; i < lstSelected.size(); i++) {
                int finalI = i;
                lstUserAndGroupAll = (ArrayList<UserAndGroup>) lstUserAndGroupAll.stream().filter(r -> !r.getID().equals(lstSelected.get(finalI).getID())).collect(Collectors.toList());
            }
        }

        adapterListUser = new SelectUserGroupMultipleAdapter(this, lstUserAndGroupAll, userAndGroup -> {
            lstSelected.add(userAndGroup);
            lstUserAndGroupAll.remove(userAndGroup);
            adapterListUser.updateCurrentList(lstUserAndGroupAll);
            adapterListUserSelected.updateItemListIsClicked(lstSelected);
            adapterListUserSelected.notifyDataSetChanged();
            edtSearchChooseUser.setText("");
        });

        StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, StaggeredGridLayoutManager.VERTICAL);
        recyChooseUser.setAdapter(adapterListUser);
        recyChooseUser.setLayoutManager(staggeredGridLayoutManager);

        adapterListUserSelected = new SelectUserGroupMultiple_TextAdapter(this, lstSelected, true, pos -> {
            lstUserAndGroupAll.add(lstSelected.get(pos));
            lstSelected.remove(lstSelected.get(pos));
            adapterListUser.updateCurrentList(lstUserAndGroupAll);
            adapterListUserSelected.updateItemListIsClicked(lstSelected);
            adapterListUserSelected.notifyDataSetChanged();
            edtSearchChooseUser.setText(edtSearchChooseUser.getText());
            edtSearchChooseUser.setSelection(edtSearchChooseUser.getText().length());
        });

        FlexboxLayoutManager flexboxLayoutManager = new FlexboxLayoutManager(this);
        flexboxLayoutManager.setFlexDirection(FlexDirection.ROW);
        flexboxLayoutManager.setJustifyContent(JustifyContent.FLEX_START);
        recySelectedUser.setAdapter(adapterListUserSelected);
        recySelectedUser.setLayoutManager(flexboxLayoutManager);

        edtSearchChooseUser.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {

            }

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {

            }

            @Override
            public void afterTextChanged(Editable s) {
                adapterListUser.getFilter().filter(s.toString());
            }
        });

        imgCloseChooseUser.setOnClickListener(v -> {
            KeyboardManager.hideKeyboard(edtSearchChooseUser, CreateChildTaskActivity.this);
            dialog.dismiss();
        });

        imgDeleteChooseUser.setOnClickListener(v -> {
            lstSelected.clear();
            lstCurrentUserGroup = new ArrayList<>();
            adapterListUserText.updateItemListIsClicked(lstCurrentUserGroup);
            adapterListUserText.notifyDataSetChanged();
            setDataItemTask(parentItem);
            KeyboardManager.hideKeyboard(edtSearchChooseUser, CreateChildTaskActivity.this);
            dialog.dismiss();
        });

        imgAcceptChooseUser.setOnClickListener(v -> {
            ArrayList<UserAndGroup> results = new ArrayList<>();
            if (adapterListUserSelected != null) {
                results = adapterListUserSelected.getListIsclicked();
            }

            lstCurrentUserGroup = new ArrayList<>(results);
            adapterListUserText.updateItemListIsClicked(lstCurrentUserGroup);
            adapterListUserText.notifyDataSetChanged();
            setDataItemTask(parentItem);
            KeyboardManager.hideKeyboard(edtSearchChooseUser, CreateChildTaskActivity.this);
            dialog.dismiss();
        });

        // region dialog
        dialog = new Dialog(this, R.style.Theme_Custom_BPMOP_Dialog_FullScreen);
        Window window = dialog.getWindow();
        dialog.requestWindowFeature(1);
        dialog.setCanceledOnTouchOutside(false);
        dialog.setCancelable(true);
        window.setGravity(Gravity.BOTTOM);
        window.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_ADJUST_PAN);
        dialog.setContentView(viewPopupControl);
        dialog.show();

        WindowManager.LayoutParams w = window.getAttributes();
        w.height = WindowManager.LayoutParams.MATCH_PARENT;
        w.width = WindowManager.LayoutParams.MATCH_PARENT;
        window.setAttributes(w);
        // endregion

        edtSearchChooseUser.setText("");
        edtSearchChooseUser.requestFocus();
        KeyboardManager.showKeyBoard(edtSearchChooseUser, this);
    }

    private void title() {
        View view = getLayoutInflater().inflate(R.layout.popup_control_input_text, null);
        ImageView imgBack = view.findViewById(R.id.img_PopupControl_InputText_Close);
        TextView tvTitle = view.findViewById(R.id.tv_PopupControl_InputText_Title);
        ImageView imgDone = view.findViewById(R.id.img_PopupControl_InputText_Done);
        ImageView imgDelete = view.findViewById(R.id.img_PopupControl_InputText_Delete);
        EditText edtContent = view.findViewById(R.id.edt_PopupControl_InputText);
        ImageView imgClearText = view.findViewById(R.id.img_PopupControl_InputText_ClearText);

        imgDelete.setVisibility(View.GONE);
        tvTitle.setText(tvTieuDe.getText().toString());
        Functions.share.setTVHighlightControl(this, tvTitle);
        edtContent.setFilters(new InputFilter[]{new InputFilter.LengthFilter(255)});

        if (!Functions.isNullOrEmpty(tvTieuDeContent.getText().toString())) {
            edtContent.setText(DetailFunc.share.formatHTMLToString(tvTieuDeContent.getText().toString()));
        } else {
            edtContent.setText("");
        }

        // region Dialog
        Dialog dialog = new Dialog(this, R.style.Theme_Custom_BPMOP_Dialog_FullScreen);
        Window window = dialog.getWindow();
        dialog.requestWindowFeature(1);
        dialog.setCanceledOnTouchOutside(false);
        dialog.setCancelable(true);
        window.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_ADJUST_PAN);
        window.setGravity(Gravity.BOTTOM);
        dialog.setContentView(view);
        dialog.show();

        WindowManager.LayoutParams s = window.getAttributes();
        s.width = WindowManager.LayoutParams.MATCH_PARENT;
        s.height = WindowManager.LayoutParams.MATCH_PARENT;
        window.setAttributes(s);
        // endregion

        imgBack.setOnClickListener(v -> {
            KeyboardManager.hideKeyboard(edtContent, CreateChildTaskActivity.this);
            dialog.dismiss();
        });

        imgClearText.setOnClickListener(v -> edtContent.setText(""));

        imgDelete.setOnClickListener(v -> {
            edtContent.setText("");
            imgDone.performClick();
        });

        imgDone.setOnClickListener(v -> {
            tvTieuDeContent.setText(edtContent.getText().toString());
            KeyboardManager.hideKeyboard(edtContent, CreateChildTaskActivity.this);
            dialog.dismiss();
        });

        edtContent.requestFocus();
        KeyboardManager.showKeyBoard(edtContent, this);
    }

    private void registerReceiver() {
        BroadcastUtility.register(this, mReceiver, VarsReceiver.COMMENT);
        BroadcastUtility.register(this, mReceiver, VarsReceiver.REFRESHCOMMENT);
    }

    private void setData() {
        setDataItemTask(null);
        setDataAttachFile();
    }

    private void setDataItemTask(Task task) {
        if (task != null) {
            if (!Functions.isNullOrEmpty(task.getTitle())) {
                tvTieuDeContent.setText(task.getTitle());
            } else {
                tvTieuDeContent.setText("");
            }

            if (!Functions.isNullOrEmpty(task.getContent())) {
                richEditorNoiDung.setHtml(task.getContent());
            }
        }

        adapterListUserText = new SelectUserGroupMultiple_Text2Adapter(this, getApplicationContext(), lstCurrentUserGroup, pos -> {
            lstCurrentUserGroup = (ArrayList<UserAndGroup>) lstCurrentUserGroup.stream().filter(r -> !r.getID().equals(lstCurrentUserGroup.get(pos).getID())).collect(Collectors.toList());
            adapterListUserText.updateItemListIsClicked(lstCurrentUserGroup);
            adapterListUserText.notifyDataSetChanged();
        });

        FlexboxLayoutManager layoutManager = new FlexboxLayoutManager(getApplicationContext());
        layoutManager.setFlexDirection(FlexDirection.ROW);
        layoutManager.setJustifyContent(JustifyContent.FLEX_START);
        recyNguoiXuLy.setAdapter(adapterListUserText);
        recyNguoiXuLy.setLayoutManager(layoutManager);
    }

    private void setDataAttachFile() {
        adapterAttachment = new CreateTaskAttachmentAdapter(this, lstAttachFileFull, flagUserPermission, isClickFromAction, parentItem, new CreateTaskAttachmentAdapter.CreateTaskAttachmentListener() {
            @Override
            public void OnViewClick(int pos) {
                // mở file từ server
                if (!lstAttachFileFull.get(pos).getID().equals("")) {
                    new DownloadFile().execute(Constants.BASE_URL + lstAttachFileFull.get(pos).getPath() + ";#" + lstAttachFileFull.get(pos).getTitle() + "." + lstAttachFileFull.get(pos).getExtension());
                } else {
                    if (new File(lstAttachFileFull.get(pos).getPath()).exists()) {
                        DetailFunc.share.openFile(CreateChildTaskActivity.this, lstAttachFileFull.get(pos).getPath());
                    }
                }
            }

            @Override
            public void OnSaveClick(int pos) {

            }

            @Override
            public void OnDeleteClick(int pos) {

            }
        });

        recyAttachFile.setLayoutManager(new LinearLayoutManager(getApplicationContext(), RecyclerView.VERTICAL, false));
        recyAttachFile.setAdapter(adapterAttachment);
        recyAttachFile.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, Functions.share.convertDpToPixel(60, getApplicationContext()) * lstAttachFileFull.size()));

        if (parentItem != null && parentItem.getStatus() == FuncDetailCreateTask.ActionStatusID.Hold) {

        } else {
            int buttonWidth = (int) (getResources().getDisplayMetrics().widthPixels * 0.15);
            new SwipeHelper(getApplicationContext(), recyAttachFile, buttonWidth) {
                @Override
                public void instantiateUnderlayButton(RecyclerView.ViewHolder viewHolder, List<UnderlayButton> underlayButtons) {
                    // thằng tạo file mới dc xóa
                    if (lstAttachFileFull.get(viewHolder.getAdapterPosition()).getCreatedBy().toLowerCase().equals(CurrentUser.getInstance().getUser().getID())) {
                        CreateTaskAttachmentAdapter currentAdapter = (CreateTaskAttachmentAdapter) recyAttachFile.getAdapter();
                        assert currentAdapter != null;
                        if (currentAdapter.getFlagUserPermission() == FuncDetailCreateTask.FlagUserPermission.Creator
                                || currentAdapter.getFlagUserPermission() == FuncDetailCreateTask.FlagUserPermission.Handler
                                || currentAdapter.getFlagUserPermission() == FuncDetailCreateTask.FlagUserPermission.CreatorAndHandler) {
                            if (currentAdapter.getParentItem() != null && currentAdapter.getParentItem().getStatus() != 2) {
                                // chưa done thì dc xóa
                                underlayButtons.add(new UnderlayButton(getApplicationContext(), "Delete", R.drawable.icon_ver2_star_controlattch_delete, "#EB342E", buttonWidth / 3, pos -> {
                                    if (lstAttachFileFull.get(pos).getCreatedBy().toLowerCase().equals(CurrentUser.getInstance().getUser().getID().toLowerCase())) {
                                        Utility.share.showAlertWithOKCancel(Functions.share.getTitle("TEXT_DELETE_CONFIRM", "Bạn có chắc muốn xóa file này không ?"),
                                                Functions.share.getTitle("TEXT_CANCLE", "Hủy"), Functions.share.getTitle("TEXT_AGREE", "Đồng ý"),
                                                CreateChildTaskActivity.this, () -> {
                                                    // file mới thêm local thì không cần add vào list Remove
                                                    if (!lstAttachFileFull.get(pos).getID().equals("")) {
                                                        lstAttFileControl_Deleted.add(lstAttachFileFull.get(pos));
                                                    }

                                                    lstAttachFileFull = (ArrayList<AttachFile>) lstAttachFileFull.stream().filter(r -> !r.getTitle().equals(lstAttachFileFull.get(pos).getTitle())).collect(Collectors.toList());
                                                    adapterAttachment.notifyDataSetChanged();
                                                    recyAttachFile.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, Functions.share.convertDpToPixel(60, getApplicationContext()) * lstAttachFileFull.size()));
                                                });
                                    }
                                }));
                            } else if (currentAdapter.getParentItem() == null && currentAdapter.getFlagUserPermission() == FuncDetailCreateTask.FlagUserPermission.Creator) {
                                // người tạo mới vào -> dc xóa
                                underlayButtons.add(new UnderlayButton(getApplicationContext(), "Delete", R.drawable.icon_ver2_star_controlattch_delete, "#EB342E", buttonWidth / 3, pos -> {
                                    if (lstAttachFileFull.get(pos).getCreatedBy().toLowerCase().equals(CurrentUser.getInstance().getUser().getID().toLowerCase())) {
                                        Utility.share.showAlertWithOKCancel(Functions.share.getTitle("TEXT_DELETE_CONFIRM", "Bạn có chắc muốn xóa file này không ?"),
                                                Functions.share.getTitle("TEXT_CANCLE", "Hủy"), Functions.share.getTitle("TEXT_AGREE", "Đồng ý"),
                                                CreateChildTaskActivity.this, () -> {
                                                    // file mới thêm local thì không cần add vào list Remove
                                                    if (!lstAttachFileFull.get(pos).getID().equals("")) {
                                                        lstAttFileControl_Deleted.add(lstAttachFileFull.get(pos));
                                                    }

                                                    lstAttachFileFull = (ArrayList<AttachFile>) lstAttachFileFull.stream().filter(r -> !r.getTitle().equals(lstAttachFileFull.get(pos).getTitle())).collect(Collectors.toList());
                                                    adapterAttachment.notifyDataSetChanged();
                                                    recyAttachFile.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, Functions.share.convertDpToPixel(60, getApplicationContext()) * lstAttachFileFull.size()));
                                                });
                                    }
                                }));
                            }
                        }
                    }

                    underlayButtons.add(new UnderlayButton(getApplicationContext(), "Save", R.drawable.icon_ver2_download_small, "#28B0FF", buttonWidth / 3, pos -> {
                        if (!lstAttachFileFull.get(pos).getID().equals("")) {
                            new DownloadFile().execute(Constants.BASE_URL + lstAttachFileFull.get(pos).getPath() + ";#" + lstAttachFileFull.get(pos).getTitle() + "." + lstAttachFileFull.get(pos).getExtension());
                        } else {
                            if (new File(lstAttachFileFull.get(pos).getPath()).exists()) {
                                DetailFunc.share.openFile(CreateChildTaskActivity.this, lstAttachFileFull.get(pos).getPath());
                            }
                        }
                    }));
                }
            };
        }
    }

    @Override
    protected void onResume() {
        super.onResume();

    }

    @Override
    protected void onPause() {
        super.onPause();
        BroadcastUtility.unregister(this, mReceiver);
    }

    private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {

        }
    };

    @Override
    protected void onActivityResult(int requestCode, int resultCode, @Nullable Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        try {
            // đính kèm comment
            if ((requestCode == Constants.mTaskChildAttachment || requestCode == Constants.mTaskChildAttachmentCamera) && resultCode == RESULT_OK) {
                AttachFile beanAttachFile = new AttachFile();
                // chọn file thường
                if (requestCode == Constants.mTaskChildAttachment)
                {
                    assert data != null;
                    beanAttachFile = DetailFunc.share.getAttachFileFromURI(this, data.getData());
                } else {
                    if (uri != null) {
                        beanAttachFile = DetailFunc.share.getAttachFileFromURICamera(this, uri);
                    } else {
                        beanAttachFile.setPath("");
                    }
                }

                if (Functions.isNullOrEmpty(beanAttachFile.getPath())) {
                    Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Vui lòng thử lại!"),
                            this);
                    return;
                }

                lstAttachFileFull.add(beanAttachFile);
                adapterAttachment.notifyDataSetChanged();
                recyAttachFile.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, Functions.share.convertDpToPixel(60, this) * lstAttachFileFull.size()));
            }
        } catch (Exception ex) {
            Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Vui lòng thử lại!"),
                    this);
            Log.d("ERR onActivityResult", ex.getMessage());
        }
    }

    @Override
    public void OnGetTaskSuccess(DetailTask detailTask) {

    }

    @Override
    public void OnGetTaskErr(String err) {

    }

    @Override
    public void OnCreateTaskSuccess() {
        hideProgressDialog();
        Intent i = new Intent();
        i.setAction(VarsReceiver.CREATE_TASK);
        //setResult(VarsReceiver.TASK_RESULT);
        BroadcastUtility.send(this, i);

        finish();
    }

    @Override
    public void OnCreateTaskErr(String err) {
        hideProgressDialog();
        Utility.share.showAlertWithOnlyOK(err, Functions.share.getTitle("TEXT_CLOSE", "Close"), this);
    }
}
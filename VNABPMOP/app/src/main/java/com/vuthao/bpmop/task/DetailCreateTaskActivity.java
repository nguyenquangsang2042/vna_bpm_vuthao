package com.vuthao.bpmop.task;

import androidx.activity.result.ActivityResultLauncher;
import androidx.activity.result.contract.ActivityResultContracts;
import androidx.annotation.Nullable;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.recyclerview.widget.StaggeredGridLayoutManager;

import android.annotation.SuppressLint;
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
import android.widget.HorizontalScrollView;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;
import android.widget.TextView;
import android.widget.TimePicker;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.AnimationController;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.CalendarUltis;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.DateTimeUtility;
import com.vuthao.bpmop.base.DownloadFile;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.MultipleClickGuard;
import com.vuthao.bpmop.base.SwipeHelper;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.api.ApiBPM;
import com.vuthao.bpmop.base.custom.CustomFlexBoxRecyclerView;
import com.vuthao.bpmop.base.custom.editor.RichEditor;
import com.vuthao.bpmop.base.custom.flexbox.FlexDirection;
import com.vuthao.bpmop.base.custom.flexbox.FlexboxLayoutManager;
import com.vuthao.bpmop.base.custom.flexbox.JustifyContent;
import com.vuthao.bpmop.base.model.custom.DetailTask;
import com.vuthao.bpmop.base.model.custom.DownloadedFiles;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.base.keyboard.KeyboardManager;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.model.app.WorkflowItem;
import com.vuthao.bpmop.base.model.custom.AttachFile;
import com.vuthao.bpmop.base.model.custom.Comment;
import com.vuthao.bpmop.base.model.custom.LookupData;
import com.vuthao.bpmop.base.model.custom.ObjectSubmitAction;
import com.vuthao.bpmop.base.model.custom.Task;
import com.vuthao.bpmop.base.model.custom.UserAndGroup;
import com.vuthao.bpmop.base.realm.AppBaseController;
import com.vuthao.bpmop.base.realm.RealmController;
import com.vuthao.bpmop.base.realm.WorkflowController;
import com.vuthao.bpmop.core.Vars;
import com.vuthao.bpmop.detail.adapter.SelectUserGroupMultipleAdapter;
import com.vuthao.bpmop.shareview.adapter.FormControlSingleChoiceAdapter;
import com.vuthao.bpmop.shareview.adapter.SelectUserGroupMultiple_Text2Adapter;
import com.vuthao.bpmop.core.component.ComponentComment;
import com.vuthao.bpmop.core.component.ComponentTaskList;
import com.vuthao.bpmop.detail.custom.DetailFunc;
import com.vuthao.bpmop.detail.fragment.ReplyCommentFragment;
import com.vuthao.bpmop.detail.presenter.DetailWorkflowPresenter;
import com.vuthao.bpmop.shareview.SharedView_PopupChooseFile;
import com.vuthao.bpmop.shareview.adapter.SelectUserGroupMultiple_TextAdapter;
import com.vuthao.bpmop.task.adapter.CreateTaskAttachmentAdapter;
import com.vuthao.bpmop.task.child.CreateChildTaskActivity;
import com.vuthao.bpmop.task.presenter.DetailCreateTaskPresenter;

import java.io.File;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.List;
import java.util.stream.Collectors;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;
import io.realm.Realm;

public class DetailCreateTaskActivity extends BaseActivity implements ApiBPM.ApiBPMRefreshListener, DetailCreateTaskPresenter.DeleleTaskListener, DetailCreateTaskPresenter.DetailCreateCommentTaskListener, DetailWorkflowPresenter.CommentListener, DetailCreateTaskPresenter.DetailCreateTaskListener {
    @BindView(R.id.ln_ViewDetailCreateTask_All)
    LinearLayout lnAll;
    @BindView(R.id.ln_ViewDetailCreateTask_SubToolbar)
    LinearLayout lnSubToolbar;
    @BindView(R.id.img_ViewDetailCreateTask_Back)
    ImageView imgBack;
    @BindView(R.id.img_ViewDetailCreateTask_Done)
    ImageView imgSave;
    @BindView(R.id.img_ViewDetailCreateTask_ChildTask)
    ImageView imgCreateChildTask;
    @BindView(R.id.img_ViewDetailCreateTask_DoneTask)
    ImageView imgDoneTask;
    @BindView(R.id.img_ViewDetailCreateTask_DeleteTask)
    ImageView imgDeleteTask;
    @BindView(R.id.tv_ViewDetailCreateTask_Name)
    TextView tvName;
    @BindView(R.id.img_ViewDetailCreateTask_Name)
    ImageView imgTitleName;
    @BindView(R.id.ln_ViewDetailCreateTask_Data)
    LinearLayout lnData;
    @BindView(R.id.ln_ViewDetailCreateTask_NoData)
    LinearLayout lnNoData;
    @BindView(R.id.ln_ViewDetailCreateTask_ChiTiet)
    LinearLayout lnChiTiet;
    @BindView(R.id.ln_ViewDetailCreateTask_CongViecCon)
    LinearLayout lnCongViecCon;
    @BindView(R.id.ln_ViewDetailCreateTask_ChiTiet_Content)
    LinearLayout lnChiTietContent;
    @BindView(R.id.ln_ViewDetailCreateTask_CongViecCon_Content)
    LinearLayout lnCongViecConContent;
    @BindView(R.id.tv_ViewDetailCreateTask_NoData)
    TextView tvNoData;
    @BindView(R.id.tv_ViewDetailCreateTask_ChiTiet)
    TextView tvChiTiet;
    @BindView(R.id.tv_ViewDetailCreateTask_CongViecCon)
    TextView tvCongViecCon;
    @BindView(R.id.vw_ViewDetailCreateTask_ChiTiet)
    View vwChiTiet;
    @BindView(R.id.vw_ViewDetailCreateTask_CongViecCon)
    View vwCongViecCon;
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

    public int currentFlagPage = 0; // 0 la chi tiet, 1 la cong viec con
    private DetailCreateTaskPresenter presenter;
    private AnimationController animationController;
    private boolean isClickFromAction = false; // Check xem l?? Click t??? Action hay Control
    private int flagUserPermission;
    private WorkflowItem workflowItem;
    private Task parentItem;
    private ArrayList<UserAndGroup> lstCurrentUserGroup;
    private ArrayList<AttachFile> lstAttachFileFull;
    private ArrayList<Comment> lstComment;
    // Comment
    private ArrayList<AttachFile> lstAttachComment;
    private ArrayList<Task> lstChildTask;
    private ArrayList<UserAndGroup> lstUserAndGroupAll;
    private ArrayList<UserAndGroup> lstSelected;
    private WorkflowController workflowController;
    private String CommentChanged;
    private String OtherResourceId;
    private ComponentComment componentComment;
    private SelectUserGroupMultiple_Text2Adapter adapterListUserText;
    private CreateTaskAttachmentAdapter adapterAttachment;
    private SelectUserGroupMultipleAdapter adapterListUser;
    private SelectUserGroupMultiple_TextAdapter adapterListUserSelected;
    // l??u l???i nh???ng item n??o ???? b??? x??a ra kh???i Control inputattachmenthorizon
    private ArrayList<AttachFile> lstAttFileControl_Deleted;
    private int taskId = 0;
    private Realm realm;
    private DetailWorkflowPresenter detailWorkflowPresenter;
    private Dialog dialog;
    private boolean isChange = true;
    private ApiBPM apiBPM;
    private boolean isChildTask = false;
    private boolean isDeleteChildTask = false;
    public Uri uri;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_detail_create_task);
        ButterKnife.bind(this);

        getWorkflowItem();

        lnNguoiXuLyContent.setOnClickListener(new MultipleClickGuard(view ->  assigned(), 2000));
    }

    private void getWorkflowItem() {
        Bundle bundle = getIntent().getExtras();
        String workflowItemId = bundle.get("WorkflowItemId").toString();
        workflowItem = new AppBaseController().getWorkflowItem(workflowItemId);
        if (workflowItem == null) {
            lnAll.setVisibility(View.INVISIBLE);
            Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("MESS_WORKFLOW_NOTFOUND", "Kh??ng t??m th???y th??ng tin phi???u, vui l??ng th??? l???i sau!"), Functions.share.getTitle("TEXT_CLOSE", "Close"), this, this::finish);
        } else {
            isClickFromAction = bundle.getBoolean("isClickFromAction");
            isChildTask = bundle.getBoolean("isChildTask", false);
            taskId = bundle.getInt("taskId", 0);

            init();
            setTitle();
            setData();
        }
    }

    private void init() {
        apiBPM = new ApiBPM(this, 3);
        workflowController = new WorkflowController();
        presenter = new DetailCreateTaskPresenter(this, this, this);
        animationController = new AnimationController();
        lstCurrentUserGroup = new ArrayList<>();
        lstAttachFileFull = new ArrayList<>();
        lstAttFileControl_Deleted = new ArrayList<>();
        lstUserAndGroupAll = new ArrayList<>();
        lstSelected = new ArrayList<>();
        lstComment = new ArrayList<>();
        lstChildTask = new ArrayList<>();
        lstAttachComment = new ArrayList<>();
        realm = new RealmController().getRealm();
        detailWorkflowPresenter = new DetailWorkflowPresenter(this);

        richEditorNoiDung.setEnabled(false);
        flagUserPermission = FuncDetailCreateTask.FlagUserPermission.Viewer;

        lnNguoiGiao.setVisibility(View.GONE);
    }

    private void setTitle() {
        tvName.setText(Functions.share.getTitle("TEXT_ASSIGNTASK", "Ph??n c??ng c??ng vi???c"));
        tvChiTiet.setText(Functions.share.getTitle("TEXT_DETAIL", "Chi ti???t"));
        tvCongViecCon.setText(Functions.share.getTitle("TEXT_CHILDTASK", "C??ng vi???c con"));
        tvTieuDe.setText(Functions.share.getTitle("TEXT_TITLE_REQUIRE", "Ti??u ????? (*)"));
        tvNguoiXuLy.setText(Functions.share.getTitle("TEXT_USER_PROCESS_REQUIRE", "Ng?????i x??? l?? (*)"));
        tvHanHoanTat.setText(Functions.share.getTitle("TEXT_DUEDATE", "H???n ho??n t???t"));
        tvTinhTrang.setText(Functions.share.getTitle("TEXT_STATUS", "T??nh tr???ng"));
        tvNoiDung.setText(Functions.share.getTitle("TEXT_CONTENT", "N???i dung"));
        tvAttachFile.setText(Functions.share.getTitle("TEXT_ATTACHMENT", "T??i li???u ????nh k??m"));
        tvAttachFileTaoMoi.setText(Functions.share.getTitle("TEXT_ADDNEW_ATTACHMENT", "T???o m???i"));
        tvAttachFileChild1.setText(Functions.share.getTitle("TEXT_CONTROL_DOCUMENTNAME", "T??n t??i li???u"));
        tvAttachFileChild2.setText(Functions.share.getTitle("TEXT_ASSIGNORS", "Ng?????i giao"));
        tvNguoiGiao.setText(Functions.share.getTitle("TEXT_CREATOR", "Ng?????i giao"));
        tvNoData.setText(Functions.share.getTitle("TEXT_NODATA", "Kh??ng c?? d??? li???u"));

        Functions.share.setTVHighlightControl(this, tvTieuDe);
        Functions.share.setTVHighlightControl(this, tvNguoiXuLy);
    }

    private void setData() {
        // tr?????ng h???p t???o m???i
        if (isClickFromAction) {
            flagUserPermission = FuncDetailCreateTask.FlagUserPermission.Creator;
            setDataItemTask(parentItem);
            setDataAttachFile();
            setViewPermission();
        } else {
            // Click t??? form
            lnData.setVisibility(View.GONE);
            lnNoData.setVisibility(View.GONE);

            imgSave.setVisibility(View.INVISIBLE);
            imgCreateChildTask.setVisibility(View.GONE);
            imgDoneTask.setVisibility(View.GONE);
            imgDeleteTask.setVisibility(View.GONE);

            showProgressDialog();
            presenter.checkWorkflowPermisson(workflowItem, taskId, !isClickFromAction);
        }
    }

    private void setDataItemTask(Task task) {
        if (task != null) {
            if (!Functions.isNullOrEmpty(task.getTitle())) {
                tvTieuDeContent.setText(task.getTitle());
            } else {
                tvTieuDeContent.setText("");
            }

            if (!Functions.isNullOrEmpty(task.getDueDate())) {
                tvHanHoanTatContent.setText(Functions.share.formatDateLanguage(task.getDueDate()));
            } else {
                tvHanHoanTatContent.setText("");
            }

            tvTinhTrangContent.setText(FuncDetailCreateTask.getStatusNameByID(task.getStatus()));

            if (!Functions.isNullOrEmpty(task.getContent())) {
                richEditorNoiDung.setHtml(task.getContent());
            }

            if (!Functions.isNullOrEmpty(task.getCreatedBy())) {
                User user = realm.where(User.class)
                        .equalTo("ID", task.getCreatedBy().toLowerCase())
                        .findFirst();
                if (user != null) {
                    tvNguoiGiaoContent.setText(user.getFullName());
                } else {
                    tvNguoiGiaoContent.setText("");
                }
            }
        }

            adapterListUserText = new SelectUserGroupMultiple_Text2Adapter(this, getApplicationContext(), lstCurrentUserGroup, pos -> {
            lstCurrentUserGroup = (ArrayList<UserAndGroup>) lstCurrentUserGroup.stream().filter(r -> !r.getID().equals(lstCurrentUserGroup.get(pos).getID())).collect(Collectors.toList());
            adapterListUserText.updateItemListIsClicked(lstCurrentUserGroup);
            adapterListUserText.notifyDataSetChanged();
        });

        FlexboxLayoutManager layoutManager = new FlexboxLayoutManager(this);
        layoutManager.setFlexDirection(FlexDirection.ROW);
        layoutManager.setJustifyContent(JustifyContent.FLEX_START);
        recyNguoiXuLy.setAdapter(adapterListUserText);
        recyNguoiXuLy.setLayoutManager(layoutManager);
    }

    @OnClick({R.id.img_ViewDetailCreateTask_Back, R.id.ln_ViewDetailCreateTask_HanHoanTat_Content,
            R.id.ln_ViewDetailCreateTask_ChiTiet,
            R.id.ln_ViewDetailCreateTask_TieuDe_Content, R.id.ln_ViewDetailCreateTask_NoiDung_ContentClick,
            R.id.ln_ViewDetailCreateTask_TinhTrang_Content, R.id.img_ViewDetailCreateTask_Done,
            R.id.ln_ViewDetailCreateTask_AttachFile_TaoMoi, R.id.ln_ViewDetailCreateTask_CongViecCon,
            R.id.img_ViewDetailCreateTask_DoneTask, R.id.img_ViewDetailCreateTask_ChildTask,
            R.id.img_ViewDetailCreateTask_DeleteTask})
    public void OnViewClicked(View v) {
        switch (v.getId()) {
            case R.id.img_ViewDetailCreateTask_Back: {
                BroadcastUtility.unregister(this, mReceiver);
                finish();
                break;
            }
            case R.id.ln_ViewDetailCreateTask_CongViecCon: {
                if (lstChildTask.isEmpty() || currentFlagPage == 1) {
                    break;
                }

                currentFlagPage = 1;
                Functions.share.setTVHighligtColor(tvCongViecCon, tvCongViecCon.getText().toString(), "(", ")", 0);
                presenter.setToolbarItem_Selected(this, tvCongViecCon, vwCongViecCon);
                presenter.setToolbarItem_NotSelected(this, tvChiTiet, vwChiTiet);
                lnCongViecConContent.startAnimation(animationController.fadeIn(this));
                lnCongViecConContent.setVisibility(View.VISIBLE);
                lnChiTietContent.setVisibility(View.GONE);
                break;
            }
            case R.id.ln_ViewDetailCreateTask_TieuDe_Content: {
                title();
                break;
            }
            case R.id.ln_ViewDetailCreateTask_HanHoanTat_Content: {
                duedate();
                break;
            }
            case R.id.ln_ViewDetailCreateTask_NoiDung_ContentClick: {
                content();
                break;
            }
            case R.id.ln_ViewDetailCreateTask_TinhTrang_Content: {
                status();
                break;
            }
            case R.id.img_ViewDetailCreateTask_Done: {
                save();
                break;
            }
            case R.id.ln_ViewDetailCreateTask_AttachFile_TaoMoi: {
                SharedView_PopupChooseFile popupChooseFile = new SharedView_PopupChooseFile(getLayoutInflater(), this, "DetailCreateTask", lnAll,
                        Constants.mTaskAttachment, Constants.mTaskAttachmentCamera,
                        Vars.FlagView.DetailCreateTask_ControlInputAttachmentVertical);
                popupChooseFile.initializeView();
                break;
            }
            case R.id.ln_ViewDetailCreateTask_ChiTiet: {
                if (currentFlagPage == 0) break;

                currentFlagPage = 0;
                Functions.share.setTVHighligtColor(tvCongViecCon, tvCongViecCon.getText().toString(), "(", ")", 0);
                presenter.setToolbarItem_Selected(this, tvChiTiet, vwChiTiet);
                presenter.setToolbarItem_NotSelected(this, tvCongViecCon, vwCongViecCon);
                lnChiTietContent.startAnimation(animationController.fadeIn(this));
                lnChiTietContent.setVisibility(View.VISIBLE);
                lnCongViecConContent.setVisibility(View.GONE);
                break;
            }
            case R.id.img_ViewDetailCreateTask_DoneTask: {
                parentItem.setStatus(2);
                tvTinhTrangContent.setText(FuncDetailCreateTask.getStatusNameByID(2));
                OnViewClicked(imgSave);
                break;
            }
            case R.id.img_ViewDetailCreateTask_ChildTask: {
                Intent intent = new Intent(this, CreateChildTaskActivity.class);
                intent.putExtra("task", new Gson().toJson(parentItem));
                intent.putExtra("isClickFromAction", isClickFromAction);
                startActivity(intent);
                //launcher.launch(intent);
                break;
            }
            case R.id.img_ViewDetailCreateTask_DeleteTask: {
                delete();
                break;
            }
        }
    }

    private void delete() {
        Utility.share.showAlertWithOKCancel(Functions.share.getTitle("TEXT_DELETE_CONFIRM_TASK", "B???n c?? ch???c mu???n x??a task n??y kh??ng?"),
                Functions.share.getTitle("TEXT_CANCEL", "H???y"),
                Functions.share.getTitle("TEXT_AGREE", "?????ng ??"),
                this, () -> {
                    showProgressDialog();
                    isDeleteChildTask = isChildTask;
                    presenter.deleteTask(parentItem.getID());
                });
    }

    private void save() {
        if (Functions.isNullOrEmpty(tvTieuDeContent.getText().toString())) {
            Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("MESS_REQUIRE_TITLE", "Vui l??ng nh???p ti??u ?????"),
                    Functions.share.getTitle("TEXT_CLOSE", "Close"), DetailCreateTaskActivity.this);
            return;
        }

        if (lstCurrentUserGroup.isEmpty()) {
            Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("MESS_REQUIRE_USERGROUP", "Vui l??ng ch???n ng?????i ????? th???c hi???n."),
                    Functions.share.getTitle("TEXT_CLOSE", "Close"), DetailCreateTaskActivity.this);
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

        // X??? l?? file b??? x??a ??i
        ArrayList<ObjectSubmitAction> lstSubmitActionData = new ArrayList<>();
        if (!lstAttFileControl_Deleted.isEmpty()) {
            ObjectSubmitAction objectSubmitAction = new ObjectSubmitAction();
            objectSubmitAction.setID("");
            objectSubmitAction.setValue(new Gson().toJson(lstAttFileControl_Deleted));
            objectSubmitAction.setTypeSP("RemoveAttachment");
            objectSubmitAction.setDataType("");
            lstSubmitActionData.add(objectSubmitAction);
        }

        switch (flagUserPermission) {
            case FuncDetailCreateTask.FlagUserPermission.Creator: {
                if (isClickFromAction) {
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
                            Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("TEXT_DATE_COMPARE2", "H???n ho??n t???t kh??ng ???????c nh??? h??n th???i gian hi???n t???i!"),
                                    Functions.share.getTitle("TEXT_CLOSE", "Close"), DetailCreateTaskActivity.this);
                            hideProgressDialog();
                            break;
                        }
                    }

                    Task task = new Task();
                    task.setID(0);
                    task.setWorkflowId(workflowItem.getWorkflowID());
                    task.setSPItemId(Integer.parseInt(workflowItem.getID()));
                    task.setStep(workflowItem.getStep() > 0 ? workflowItem.getStep() : -1);
                    task.setTitle(tvTieuDeContent.getText().toString());

                    if (!Functions.isNullOrEmpty(tvHanHoanTatContent.getText().toString())) {
                        long l = Functions.share.formatStringToLong(tvHanHoanTatContent.getText().toString(), "dd/MM/yy HH:mm");
                        String newDueDate = Functions.share.formatLongToDay(l, "yyyy-MM-dd HH:mm");
                        task.setDueDate(newDueDate);
                    } else {
                        task.setDueDate(null);
                    }

                    task.setParent(0);
                    task.setContent(!Functions.isNullOrEmpty(richEditorNoiDung.getHtml()) ? richEditorNoiDung.getHtml() : "");
                    task.setStatus(0);

                    presenter.sendCreateTaskAction(task, lstCurrentUserGroup, lstSubmitActionData, lstAttachmentLocal, FuncDetailCreateTask.FlagActionPermission.CreateNew);
                } else {
                    if (!Functions.isNullOrEmpty(tvHanHoanTatContent.getText().toString())) {
                        String format = DateTimeUtility.getFormat("task");

                        long l;
                        if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
                            l = Functions.share.formatStringToLong(tvHanHoanTatContent.getText().toString(), format);
                        } else {
                            l = Functions.share.formatStringToLong(tvHanHoanTatContent.getText().toString(), format);
                        }

                        long currentDay = Functions.share.formatStringToLong(Functions.share.getToDay(format), format);
                        if (DateTimeUtility.isAfterDay(l, currentDay)) {
                            Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("TEXT_DATE_COMPARE2", "H???n ho??n t???t kh??ng ???????c nh??? h??n th???i gian hi???n t???i!"),
                                    Functions.share.getTitle("TEXT_CLOSE", "Close"), DetailCreateTaskActivity.this);
                            hideProgressDialog();
                            break;
                        }

                        if (!Functions.isNullOrEmpty(tvHanHoanTatContent.getText().toString())) {
                            long time = Functions.share.formatStringToLong(tvHanHoanTatContent.getText().toString(), "dd/MM/yy HH:mm");
                            String newDueDate = Functions.share.formatLongToDay(time, "yyyy-MM-dd HH:mm");
                            parentItem.setDueDate(newDueDate);
                        } else {
                            parentItem.setDueDate(null);
                        }
                    }

                    parentItem.setTitle(tvTieuDeContent.getText().toString());
                    parentItem.setContent(!Functions.isNullOrEmpty(richEditorNoiDung.getHtml()) ? richEditorNoiDung.getHtml() : "");
                    presenter.sendCreateTaskAction(parentItem, lstCurrentUserGroup, lstSubmitActionData, lstAttachmentLocal, FuncDetailCreateTask.FlagActionPermission.CreatorUpdate);
                }
                break;
            }
            case FuncDetailCreateTask.FlagUserPermission.CreatorAndHandler:
            case FuncDetailCreateTask.FlagUserPermission.Handler: {
                parentItem.setTitle(tvTieuDeContent.getText().toString());
                if (!Functions.isNullOrEmpty(tvHanHoanTatContent.getText().toString())) {
                    long l = Functions.share.formatStringToLong(tvHanHoanTatContent.getText().toString(), "dd/MM/yy HH:mm");
                    String newDueDate = Functions.share.formatLongToDay(l, "yyyy-MM-dd HH:mm");
                    parentItem.setDueDate(newDueDate);
                } else {
                    parentItem.setDueDate(null);
                }

                parentItem.setContent(!Functions.isNullOrEmpty(richEditorNoiDung.getHtml()) ? richEditorNoiDung.getHtml() : "");
                int flag = flagUserPermission == FuncDetailCreateTask.FlagUserPermission.Handler ? FuncDetailCreateTask.FlagActionPermission.HandlerUpdate :
                        FuncDetailCreateTask.FlagActionPermission.CreatorHandlerUpdate;
                presenter.sendCreateTaskAction(parentItem, lstCurrentUserGroup, lstSubmitActionData, lstAttachmentLocal, flag);
                break;
            }
        }
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
                LookupData _selectedLookupItem = status.get(pos);
                if (_selectedLookupItem != null) {
                    parentItem.setStatus(Integer.parseInt(_selectedLookupItem.getID()));
                    tvTinhTrangContent.setText(_selectedLookupItem.getTitle());
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
                KeyboardManager.hideKeyboard(DetailCreateTaskActivity.this);
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
                KeyboardManager.hideKeyboard(DetailCreateTaskActivity.this);
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

            txtDetailInDisable.setText(!Functions.isNullOrEmpty(Html.fromHtml(richEditorNoiDung.getHtml(), Html.FROM_HTML_MODE_LEGACY).toString()) ? Html.fromHtml(richEditorNoiDung.getHtml(), Html.FROM_HTML_MODE_LEGACY) : "");
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
        datePicker.setDescendantFocusability(DatePicker.FOCUS_BLOCK_DESCENDANTS);
        timePicker.setDescendantFocusability(TimePicker.FOCUS_BLOCK_DESCENDANTS);
        tvApply.setText(Functions.share.getTitle("TEXT_APPLY", "??p d???ng"));

        CalendarUltis calendar = null;
        if (!Functions.isNullOrEmpty(tvHanHoanTatContent.getText().toString())) {
            String format = DateTimeUtility.getFormat("task");
            calendar = new CalendarUltis(tvHanHoanTatContent.getText().toString(), format);
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
        View view = getLayoutInflater().inflate(R.layout.popup_control_choose_user, null);
        ImageView imgClose = view.findViewById(R.id.img_PopupControl_ChooseUser_Close);
        ImageView imgAccept = view.findViewById(R.id.img_PopupControl_ChooseUser_Accept);
        ImageView imgDelete = view.findViewById(R.id.img_PopupControl_ChooseUser_Delete);
        TextView tvTitle = view.findViewById(R.id.tv_PopupControl_ChooseUser_Title);
        EditText edtSearch = view.findViewById(R.id.edt_PopupControl_ChooseUser_Search);
        RecyclerView recyChooseUser = view.findViewById(R.id.recy_PopupControl_ChooseUser);
        CustomFlexBoxRecyclerView recySelectedUser = view.findViewById(R.id.recy_PopupControl_SelectedUser);

        recySelectedUser.setVisibility(View.VISIBLE);
        imgAccept.setVisibility(View.VISIBLE);
        imgDelete.setVisibility(View.VISIBLE);
        recySelectedUser.setMaxRowAndRowHeight(Functions.share.convertDpToPixel(35, this), 3);

        tvTitle.setText(Functions.share.getTitle("TEXT_TITLE_USERGROUP", "Ch???n ng?????i ho???c nh??m"));
        edtSearch.setHint(Functions.share.getTitle("MESS_REQUIRE_USERGROUP", "Vui l??ng ch???n ng?????i ho???c nh??m ????? th???c hi???n"));

        lstSelected = new ArrayList<>(lstCurrentUserGroup);
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
            edtSearch.setText("");
        });

        StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, StaggeredGridLayoutManager.VERTICAL);
        recyChooseUser.setAdapter(adapterListUser);
        recyChooseUser.setLayoutManager(staggeredGridLayoutManager);

        adapterListUserSelected = new SelectUserGroupMultiple_TextAdapter(this, lstSelected, true, pos -> {
            UserAndGroup _clickedItem = lstSelected.get(pos);
            lstSelected.remove(_clickedItem);
            lstUserAndGroupAll.add(_clickedItem);
            adapterListUser.updateCurrentList(lstUserAndGroupAll);
            adapterListUserSelected.updateItemListIsClicked(lstSelected);
            adapterListUserSelected.notifyDataSetChanged();
            edtSearch.setText(edtSearch.getText());
            edtSearch.setSelection(edtSearch.length());
        });

        FlexboxLayoutManager flexboxLayoutManager = new FlexboxLayoutManager(this);
        flexboxLayoutManager.setFlexDirection(FlexDirection.ROW);
        flexboxLayoutManager.setJustifyContent(JustifyContent.FLEX_START);
        recySelectedUser.setAdapter(adapterListUserSelected);
        recySelectedUser.setLayoutManager(flexboxLayoutManager);

        edtSearch.addTextChangedListener(new TextWatcher() {
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

        imgClose.setOnClickListener(v -> {
            KeyboardManager.hideKeyboard(edtSearch, DetailCreateTaskActivity.this);
            dialog.dismiss();
        });

        imgDelete.setOnClickListener(v -> {
            lstSelected.clear();
            lstCurrentUserGroup = new ArrayList<>();
            adapterListUserText.updateItemListIsClicked(lstCurrentUserGroup);
            adapterListUserText.notifyDataSetChanged();
            setDataItemTask(parentItem);
            KeyboardManager.hideKeyboard(edtSearch, DetailCreateTaskActivity.this);
            dialog.dismiss();
        });

        imgAccept.setOnClickListener(v -> {
            ArrayList<UserAndGroup> results = new ArrayList<>();
            if (adapterListUserSelected != null) {
                results = adapterListUserSelected.getListIsclicked();
            }

            lstCurrentUserGroup = new ArrayList<>(results);
            adapterListUserText.updateItemListIsClicked(lstCurrentUserGroup);
            adapterListUserText.notifyDataSetChanged();
            setDataItemTask(parentItem);
            KeyboardManager.hideKeyboard(edtSearch, DetailCreateTaskActivity.this);
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
        dialog.setContentView(view);
        dialog.show();

        WindowManager.LayoutParams w = window.getAttributes();
        w.height = WindowManager.LayoutParams.MATCH_PARENT;
        w.width = WindowManager.LayoutParams.MATCH_PARENT;
        window.setAttributes(w);
        // endregion

        edtSearch.setText("");
        edtSearch.requestFocus();
        KeyboardManager.showKeyBoard(edtSearch, this);
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
            KeyboardManager.hideKeyboard(edtContent, DetailCreateTaskActivity.this);
            dialog.dismiss();
        });

        imgClearText.setOnClickListener(v -> edtContent.setText(""));

        imgDelete.setOnClickListener(v -> {
            edtContent.setText("");
            imgDone.performClick();
        });

        imgDone.setOnClickListener(v -> {
            tvTieuDeContent.setText(edtContent.getText().toString());
            KeyboardManager.hideKeyboard(edtContent, DetailCreateTaskActivity.this);
            dialog.dismiss();
        });

        edtContent.requestFocus();
        KeyboardManager.showKeyBoard(edtContent, this);
    }

    @Override
    public void OnGetTaskSuccess(DetailTask task) {
        parentItem = task.getParentItem();
        lstCurrentUserGroup = task.getAssignTo();
        lstAttachFileFull = task.getAttachment();
        lstChildTask = task.getChildTask();
        flagUserPermission = task.getUserPermission();
        CommentChanged = parentItem.getCommentChanged();
        OtherResourceId = parentItem.getOtherResourceId();

        presenter.getDetailOtherResource(OtherResourceId, workflowItem, CommentChanged, "16");
        setDataItemTask(parentItem);
        setDataAttachFile();
        setDataChildTask();
        setDataComment();
        setViewPermission();

        lnData.setVisibility(View.VISIBLE);
        lnNoData.setVisibility(View.GONE);
        hideProgressDialog();
    }

    private void setViewPermission() {
        if (parentItem != null) {
            lnNguoiGiao.setVisibility(View.VISIBLE);
        } else {
            lnNguoiGiao.setVisibility(View.GONE);
        }

        switch (flagUserPermission) {
            // Ng?????i xem: ch??? dc xem v?? ko dc thao t??c g?? th??m
            case FuncDetailCreateTask.FlagUserPermission.Viewer: {
                tvName.setText(Functions.share.getTitle("TEXT_TASK", "C??ng vi???c"));

                //Toolbar
                imgTitleName.setVisibility(View.GONE);
                imgSave.setVisibility(View.INVISIBLE);
                imgDoneTask.setVisibility(View.GONE);
                imgCreateChildTask.setVisibility(View.GONE);
                imgDeleteTask.setVisibility(View.GONE);

                lnSubToolbar.setVisibility(View.VISIBLE);
                lnSubToolbar.setEnabled(false);
                lnSubToolbar.setBackgroundColor(Color.TRANSPARENT);

                // ???????c xem t??nh tr???ng - ko dc Edit
                tvTinhTrang.setVisibility(View.VISIBLE);
                lnTinhTrangContent.setVisibility(View.VISIBLE);

                // Ti??u ?????
                FuncDetailCreateTask.setViewControl_NotEdited(lnTieuDeContent, tvTieuDeContent);

                // Ng?????i x??? l??
                FuncDetailCreateTask.setViewControl_NotEdited(lnNguoiXuLyContent, null);
                recyNguoiXuLy.setPadding(0, 0, 0, 0);
                imgNguoiXuLy.setVisibility(View.GONE);

                // H???n ho??n t???t
                FuncDetailCreateTask.setViewControl_NotEdited(lnHanHoanTatContent, tvHanHoanTatContent);
                imgHanHoanTat.setVisibility(View.GONE);

                // T??nh tr???ng
                FuncDetailCreateTask.setViewControl_NotEdited(lnTinhTrangContent, tvTinhTrangContent);

                // N???i dung
                FuncDetailCreateTask.setViewControl_NotEdited(lnNoiDungContent, null);
                richEditorNoiDung.setBackgroundColor(Color.TRANSPARENT);

                // ????nh k??m -> ko dc ????nh k??m, ch??? dc down
                lnTaoMoi.setVisibility(View.INVISIBLE);

                // B??? h???t (*) c???a TextView
                removeIsRequiredAllTextView();
                break;
            }
            case FuncDetailCreateTask.FlagUserPermission.Handler: {
                tvName.setText(Functions.share.getTitle("TEXT_TASK", "C??ng vi???c"));

                //Permission - Handler - ???????c ch???nh t??nh tr???ng, file ????nh k??m, t???o Task con
                imgTitleName.setVisibility(View.GONE);

                // Cho ch???nh s???a t??nh tr???ng
                tvTinhTrang.setVisibility(View.VISIBLE);
                lnTinhTrangContent.setVisibility(View.VISIBLE);
                lnSubToolbar.setVisibility(View.VISIBLE);

                // Ti??u ?????
                FuncDetailCreateTask.setViewControl_NotEdited(lnTieuDeContent, tvTieuDeContent);

                // Ng?????i x??? l??
                FuncDetailCreateTask.setViewControl_NotEdited(lnNguoiXuLyContent, null);
                recyNguoiXuLy.setPadding(0, 0, 0, 0);
                imgNguoiXuLy.setVisibility(View.GONE);

                //  H???n ho??n t???t
                FuncDetailCreateTask.setViewControl_NotEdited(lnHanHoanTatContent, tvHanHoanTatContent);
                imgHanHoanTat.setVisibility(View.GONE);

                // N???i dung
                FuncDetailCreateTask.setViewControl_NotEdited(lnNoiDungContent, null);
                richEditorNoiDung.setBackgroundColor(Color.TRANSPARENT);

                // ????nh k??m -> ????? quy???n
                lnTaoMoi.setVisibility(View.VISIBLE);

                removeIsRequiredAllTextView();

                // do x??i relative n??n ch???nh l???i theo ki???u n??y ch??? ko x??i Visibility
                imgDoneTask.setVisibility(View.VISIBLE);
                imgSave.setVisibility(View.VISIBLE);
                imgCreateChildTask.setVisibility(View.VISIBLE);
                imgDeleteTask.setVisibility(View.GONE);

                switch (parentItem.getStatus()) {
                    case FuncDetailCreateTask.ActionStatusID.Completed: {
                        //Check Status "Ho??n t???t" - kh??ng cho Update + Ho??n t???t + Save
                        RelativeLayout.LayoutParams params = new RelativeLayout.LayoutParams(0, 0);
                        params.addRule(RelativeLayout.ALIGN_PARENT_RIGHT);

                        imgDoneTask.setVisibility(View.GONE);
                        imgSave.setVisibility(View.INVISIBLE);
                        imgSave.setLayoutParams(params);

                        FuncDetailCreateTask.setViewControl_NotEdited(lnTinhTrangContent, tvTinhTrangContent);
                        lnTaoMoi.setVisibility(View.INVISIBLE);
                        break;
                    }
                    case FuncDetailCreateTask.ActionStatusID.Hold: {
                        //Check status "T???m ho??n" - kh??ng ???????c t???o Task con, ????nh k??m
                        imgCreateChildTask.setVisibility(View.GONE);
                        lnTaoMoi.setVisibility(View.INVISIBLE);
                        break;
                    }
                }

                break;
            }
            case FuncDetailCreateTask.FlagUserPermission.Creator: {
                // Click t??? Action v??o -> t???o m???i: ???n t??nh tr???ng ??i
                if (isClickFromAction) {
                    tvName.setText(Functions.share.getTitle("TEXT_ASSIGNTASK", "Ph??n c??ng c??ng vi???c"));

                    //T???o m???i Task
                    imgSave.setVisibility(View.VISIBLE);
                    imgDoneTask.setVisibility(View.GONE);
                    imgCreateChildTask.setVisibility(View.GONE);
                    lnSubToolbar.setVisibility(View.GONE);

                    tvTinhTrang.setVisibility(View.INVISIBLE);
                    lnTinhTrangContent.setVisibility(View.INVISIBLE);
                } else {
                    // Click t??? Task v??o -> Update l???i Task ???? c??
                    tvName.setText(Functions.share.getTitle("TEXT_TASK", "C??ng vi???c"));

                    switch (parentItem.getStatus()) {
                        // check xem n???u phi???u Hold th?? ko cho l??m g?? -> gi???ng viewer
                        // check xem n???u phi???u Done -> ch??? dc t???o Task con
                        case FuncDetailCreateTask.ActionStatusID.Completed:
                        case FuncDetailCreateTask.ActionStatusID.Hold: {
                            if (parentItem.getStatus() == FuncDetailCreateTask.ActionStatusID.Completed) {
                                imgCreateChildTask.setVisibility(View.VISIBLE);
                            } else {
                                imgCreateChildTask.setVisibility(View.GONE);
                            }

                            //Permission - Viewer - Ch??? ???????c xem, kh??ng ???????c thao t??c g?? th??m
                            imgTitleName.setVisibility(View.GONE);
                            RelativeLayout.LayoutParams _params = new RelativeLayout.LayoutParams(0, 0);
                            _params.addRule(RelativeLayout.ALIGN_PARENT_RIGHT);
                            imgSave.setLayoutParams(_params);

                            imgDoneTask.setVisibility(View.GONE);
                            imgDeleteTask.setVisibility(View.GONE);

                            lnSubToolbar.setVisibility(View.VISIBLE);
                            lnSubToolbar.setEnabled(false);
                            lnSubToolbar.setBackgroundColor(Color.TRANSPARENT);

                            tvTinhTrang.setVisibility(View.VISIBLE);
                            lnTinhTrangContent.setVisibility(View.VISIBLE);

                            FuncDetailCreateTask.setViewControl_NotEdited(lnTieuDeContent, tvTieuDeContent);

                            FuncDetailCreateTask.setViewControl_NotEdited(lnNguoiXuLyContent, null);
                            recyNguoiXuLy.setPadding(0, 0, 0, 0);
                            imgNguoiXuLy.setVisibility(View.GONE);
                            FuncDetailCreateTask.setViewControl_NotEdited(lnHanHoanTatContent, tvHanHoanTatContent);
                            imgHanHoanTat.setVisibility(View.GONE);

                            FuncDetailCreateTask.setViewControl_NotEdited(lnTinhTrangContent, tvTinhTrangContent);
                            FuncDetailCreateTask.setViewControl_NotEdited(lnNoiDungContent, null);
                            lnTaoMoi.setVisibility(View.INVISIBLE);
                            removeIsRequiredAllTextView();
                            break;
                        }
                        default: {
                            // tr???ng th??i kh??c ho??n t???t v?? l?? ng t???o -> dc x??a
                            if (parentItem != null && parentItem.getStatus() != 2) {
                                // L?? ng?????i t???o v?? xem l???i phi???u -> ???????c quy???n x??a phi???u
                                imgDeleteTask.setVisibility(View.VISIBLE);
                            }

                            lnSubToolbar.setVisibility(View.VISIBLE);
                            tvTinhTrang.setVisibility(View.INVISIBLE);
                            lnTinhTrangContent.setVisibility(View.INVISIBLE);
                            imgSave.setVisibility(View.VISIBLE);
                            imgDoneTask.setVisibility(View.GONE);
                            imgCreateChildTask.setVisibility(View.VISIBLE);
                            imgDeleteTask.setVisibility(View.VISIBLE);
                            break;
                        }
                    }
                }
                break;
            }
            case FuncDetailCreateTask.FlagUserPermission.CreatorAndHandler: {
                tvName.setText(Functions.share.getTitle("TEXT_TASK", "C??ng vi???c"));
                switch (parentItem.getStatus()) {
                    case FuncDetailCreateTask.ActionStatusID.Completed: {
                        //Check Status "Ho??n t???t" - ???? ho??n t???t th?? ko dc l??m g??
                        imgTitleName.setVisibility(View.GONE);
                        tvTinhTrang.setVisibility(View.VISIBLE);
                        lnTinhTrangContent.setVisibility(View.VISIBLE);
                        lnSubToolbar.setVisibility(View.VISIBLE);

                        lnTieuDeContent.setEnabled(false);
                        lnTieuDeContent.setBackgroundColor(Color.TRANSPARENT);
                        tvTieuDeContent.setPadding(0, 0, 0, 0);
                        lnNguoiXuLyContent.setEnabled(false);
                        lnNguoiXuLyContent.setBackgroundColor(Color.TRANSPARENT);
                        recyNguoiXuLy.setPadding(0, 0, 0, 0);
                        imgNguoiXuLy.setVisibility(View.GONE);
                        lnHanHoanTatContent.setEnabled(false);
                        lnHanHoanTatContent.setBackgroundColor(Color.TRANSPARENT);
                        tvHanHoanTatContent.setPadding(0, 0, 0, 0);
                        imgHanHoanTat.setVisibility(View.GONE);
                        lnNoiDungContent.setEnabled(false);
                        lnNoiDungContent.setBackgroundColor(Color.TRANSPARENT);
                        richEditorNoiDung.setBackgroundColor(Color.TRANSPARENT);
                        tvHanHoanTatContent.setPadding(0, 0, 0, 0);

                        lnTaoMoi.setVisibility(View.VISIBLE);
                        removeIsRequiredAllTextView();
                        RelativeLayout.LayoutParams _params = new RelativeLayout.LayoutParams(0, 0);
                        _params.addRule(RelativeLayout.ALIGN_PARENT_RIGHT);

                        imgDoneTask.setVisibility(View.GONE);
                        imgSave.setVisibility(View.INVISIBLE);
                        imgSave.setLayoutParams(_params);
                        lnTinhTrangContent.setEnabled(false);
                        lnTinhTrangContent.setBackgroundColor(Color.TRANSPARENT);
                        tvTinhTrangContent.setPadding(0, 0, 0, 0);

                        lnTaoMoi.setVisibility(View.INVISIBLE);
                        break;
                    }
                    case FuncDetailCreateTask.ActionStatusID.Hold: {
                        // Permission - Handler - ???????c ch???nh t??nh tr???ng, file ????nh k??m, t???o Task con
                        imgTitleName.setVisibility(View.GONE);
                        tvTinhTrang.setVisibility(View.VISIBLE);
                        lnTinhTrangContent.setVisibility(View.VISIBLE);
                        lnSubToolbar.setVisibility(View.VISIBLE);

                        FuncDetailCreateTask.setViewControl_NotEdited(lnTieuDeContent, tvTieuDeContent);
                        FuncDetailCreateTask.setViewControl_NotEdited(lnNguoiXuLyContent, null);
                        recyNguoiXuLy.setPadding(0, 0, 0, 0);
                        imgNguoiXuLy.setVisibility(View.GONE);
                        FuncDetailCreateTask.setViewControl_NotEdited(lnHanHoanTatContent, tvHanHoanTatContent);
                        imgHanHoanTat.setVisibility(View.GONE);
                        FuncDetailCreateTask.setViewControl_Edited(lnTinhTrangContent, tvTinhTrangContent);
                        FuncDetailCreateTask.setViewControl_NotEdited(lnNoiDungContent, tvTinhTrangContent);
                        richEditorNoiDung.setBackgroundColor(Color.TRANSPARENT);

                        lnTaoMoi.setVisibility(View.VISIBLE);
                        removeIsRequiredAllTextView();

                        //Check status "T???m ho??n" - kh??ng ???????c t???o Task con, kh??ng dc c???p nh???t file
                        lnTaoMoi.setVisibility(View.INVISIBLE);
                        imgCreateChildTask.setVisibility(View.GONE);
                        break;
                    }
                    default: {
                        //Permission - Creator + Handler - All quy???n
                        tvTinhTrang.setVisibility(View.VISIBLE);
                        lnTinhTrangContent.setVisibility(View.VISIBLE);
                        imgSave.setVisibility(View.VISIBLE);
                        imgDoneTask.setVisibility(View.VISIBLE);
                        imgCreateChildTask.setVisibility(View.VISIBLE);
                        tvName.setText(Functions.share.getTitle("TEXT_TASK", "C??ng vi???c"));
                        lnSubToolbar.setVisibility(View.VISIBLE);
                        break;
                    }
                }
                break;
            }
        }
    }

    private void removeIsRequiredAllTextView() {
        if (tvTieuDe.getText().toString().contains("(*)")) {
            tvTieuDe.setText(tvTieuDe.getText().toString().replace("(*)", ""));
        }

        if (tvNguoiXuLy.getText().toString().contains("(*)")) {
            tvNguoiXuLy.setText(tvNguoiXuLy.getText().toString().replace("(*)", ""));
        }
    }

    private void setDataComment() {
        if (componentComment == null) {
            componentComment = new ComponentComment(this, getApplicationContext(), lnComment, lstComment, false);
            componentComment.initFlagRecalculateView(true);
            componentComment.initializeFrameView(lnComment);
            componentComment.setTitle();
            componentComment.setValue();
            componentComment.setEnable();
            componentComment.setProprety();

            if (lstAttachComment.size() > 0) {
                componentComment.updateListParentAttach(lstAttachComment);
            }
        }
    }

    @SuppressLint("SetTextI18n")
    private void setDataChildTask() {
        if (lstChildTask.size() > 0) {
            lnCongViecConContent.removeAllViews();
            ComponentTaskList taskList = new ComponentTaskList(this, getApplicationContext(), lnCongViecConContent, lstChildTask, true);
            taskList.initializeFrameView(lnCongViecConContent);
            taskList.setTitle();
            taskList.setValue();
            taskList.setEnable();
            taskList.setProprety();

            tvCongViecCon.setText(Functions.share.getTitle("TEXT_CHILDTASK", "C??ng vi???c con") + String.format(" (%s) ", lstChildTask.stream().filter(r -> r.getParent() == parentItem.getID()).collect(Collectors.toList()).size()));
            Functions.share.setTVHighligtColor(tvCongViecCon, tvCongViecCon.getText().toString(), "(", ")", 0);
            if (currentFlagPage == 1) {
                Functions.share.setTVHighligtColor(tvCongViecCon, tvCongViecCon.getText().toString(), "(", ")", 0);
            }
        } else {
            tvCongViecCon.setText(Functions.share.getTitle("TEXT_CHILDTASK", "C??ng vi???c con"));
            lnCongViecConContent.removeAllViews();
        }
    }

    private void setDataAttachFile() {
        adapterAttachment = new CreateTaskAttachmentAdapter(this, lstAttachFileFull, flagUserPermission, isClickFromAction, parentItem, new CreateTaskAttachmentAdapter.CreateTaskAttachmentListener() {
            @Override
            public void OnViewClick(int pos) {
                // m??? file t??? server
                if (!lstAttachFileFull.get(pos).getID().equals("")) {
                    new DownloadFile().execute(Constants.BASE_URL + lstAttachFileFull.get(pos).getPath() + ";#" + lstAttachFileFull.get(pos).getTitle() + "." + lstAttachFileFull.get(pos).getExtension());
                } else {
                    if (new File(lstAttachFileFull.get(pos).getPath()).exists()) {
                        DetailFunc.share.openFile(DetailCreateTaskActivity.this, lstAttachFileFull.get(pos).getPath());
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

        recyAttachFile.setLayoutManager(new LinearLayoutManager(this, RecyclerView.VERTICAL, false));
        recyAttachFile.setAdapter(adapterAttachment);
        recyAttachFile.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, Functions.share.convertDpToPixel(60, this) * lstAttachFileFull.size()));

        if (parentItem != null && parentItem.getStatus() == FuncDetailCreateTask.ActionStatusID.Hold) {

        } else {
            int buttonWidth = (int) (getResources().getDisplayMetrics().widthPixels * 0.15);
            new SwipeHelper(this, recyAttachFile, buttonWidth) {
                @Override
                public void instantiateUnderlayButton(RecyclerView.ViewHolder viewHolder, List<UnderlayButton> underlayButtons) {
                    // th???ng t???o file m???i dc x??a
                    if (lstAttachFileFull.get(viewHolder.getAdapterPosition()).getCreatedBy().toLowerCase().equals(CurrentUser.getInstance().getUser().getID())) {
                        CreateTaskAttachmentAdapter currentAdapter = (CreateTaskAttachmentAdapter) recyAttachFile.getAdapter();
                        assert currentAdapter != null;
                        if (currentAdapter.getFlagUserPermission() == (int) FuncDetailCreateTask.FlagUserPermission.Creator
                                || currentAdapter.getFlagUserPermission() == (int) FuncDetailCreateTask.FlagUserPermission.Handler
                                || currentAdapter.getFlagUserPermission() == (int) FuncDetailCreateTask.FlagUserPermission.CreatorAndHandler) {
                            if (currentAdapter.getParentItem() != null && currentAdapter.getParentItem().getStatus() != 2) {
                                // ch??a done th?? dc x??a
                                underlayButtons.add(new UnderlayButton(DetailCreateTaskActivity.this, "Delete", R.drawable.icon_ver2_star_controlattch_delete, "#EB342E", buttonWidth / 3, pos -> {
                                    if (lstAttachFileFull.get(pos).getCreatedBy().toLowerCase().equals(CurrentUser.getInstance().getUser().getID().toLowerCase())) {
                                        Utility.share.showAlertWithOKCancel(Functions.share.getTitle("TEXT_DELETE_CONFIRM", "B???n c?? ch???c mu???n x??a file n??y kh??ng ?"),
                                                Functions.share.getTitle("TEXT_CANCLE", "H???y"), Functions.share.getTitle("TEXT_AGREE", "?????ng ??"),
                                                DetailCreateTaskActivity.this, () -> {
                                                    // file m???i th??m local th?? kh??ng c???n add v??o list Remove
                                                    if (!lstAttachFileFull.get(pos).getID().equals("")) {
                                                        lstAttFileControl_Deleted.add(lstAttachFileFull.get(pos));
                                                    }

                                                    lstAttachFileFull = (ArrayList<AttachFile>) lstAttachFileFull.stream().filter(r -> !r.getTitle().equals(lstAttachFileFull.get(pos).getTitle())).collect(Collectors.toList());
                                                    adapterAttachment.notifyDataSetChanged();
                                                    recyAttachFile.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, Functions.share.convertDpToPixel(60, DetailCreateTaskActivity.this) * lstAttachFileFull.size()));
                                                });
                                    }
                                }));
                            } else if (currentAdapter.getParentItem() == null && currentAdapter.getFlagUserPermission() == FuncDetailCreateTask.FlagUserPermission.Creator) {
                                // ng?????i t???o m???i v??o -> dc x??a
                                underlayButtons.add(new UnderlayButton(DetailCreateTaskActivity.this, "Delete", R.drawable.icon_ver2_star_controlattch_delete, "#EB342E", buttonWidth / 3, pos -> {
                                    if (lstAttachFileFull.get(pos).getCreatedBy().toLowerCase().equals(CurrentUser.getInstance().getUser().getID().toLowerCase())) {
                                        Utility.share.showAlertWithOKCancel(Functions.share.getTitle("TEXT_DELETE_CONFIRM", "B???n c?? ch???c mu???n x??a file n??y kh??ng ?"),
                                                Functions.share.getTitle("TEXT_CANCLE", "H???y"), Functions.share.getTitle("TEXT_AGREE", "?????ng ??"),
                                                DetailCreateTaskActivity.this, () -> {
                                                    // file m???i th??m local th?? kh??ng c???n add v??o list Remove
                                                    if (!lstAttachFileFull.get(pos).getID().equals("")) {
                                                        lstAttFileControl_Deleted.add(lstAttachFileFull.get(pos));
                                                    }

                                                    lstAttachFileFull = (ArrayList<AttachFile>) lstAttachFileFull.stream().filter(r -> !r.getTitle().equals(lstAttachFileFull.get(pos).getTitle())).collect(Collectors.toList());
                                                    adapterAttachment.notifyDataSetChanged();
                                                    recyAttachFile.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, Functions.share.convertDpToPixel(60, DetailCreateTaskActivity.this) * lstAttachFileFull.size()));
                                                });
                                    }
                                }));
                            }
                        }
                    }

                    underlayButtons.add(new UnderlayButton(DetailCreateTaskActivity.this, "Save", R.drawable.icon_ver2_download_small, "#28B0FF", buttonWidth / 3, pos -> {
                        if (!lstAttachFileFull.get(pos).getID().equals("")) {
                            new DownloadFile().execute(Constants.BASE_URL + lstAttachFileFull.get(pos).getUrl() + ";#" + lstAttachFileFull.get(pos).getTitle() + "." + lstAttachFileFull.get(pos).getExtension());
                        } else {
                            if (new File(lstAttachFileFull.get(pos).getPath()).exists()) {
                                DetailFunc.share.openFile(DetailCreateTaskActivity.this, lstAttachFileFull.get(pos).getPath());
                            }
                        }
                    }));
                }
            };
        }
    }

    @Override
    public void OnGetTaskErr(String err) {
        runOnUiThread(() -> Utility.share.showAlertWithOnlyOK(err, Functions.share.getTitle("TEXT_CLOSE", "????ng"), DetailCreateTaskActivity.this, new Utility.OkListener() {
            @Override
            public void onOkListener() {
                BroadcastUtility.unregister(DetailCreateTaskActivity.this, mReceiver);
                finish();
            }
        }));
    }

    @Override
    public void OnCreateTaskSuccess() {
        apiBPM.updateDataSubmitAction(false);
    }

    @Override
    public void OnCreateTaskErr(String err) {
        hideProgressDialog();
        Utility.share.showAlertWithOnlyOK(err, Functions.share.getTitle("TEXT_CLOSE", "Close"), this);
    }

    @Override
    protected void onResume() {
        super.onResume();
        registerReceiver();
    }

    private void registerReceiver() {
        BroadcastUtility.register(this, mReceiver, VarsReceiver.COMMENT);
        BroadcastUtility.register(this, mReceiver, VarsReceiver.REFRESHCOMMENT);
        //BroadcastUtility.register(this, mReceiver, VarsReceiver.CREATE_CHILD_TASK);
        BroadcastUtility.register(this, mReceiver, VarsReceiver.CREATE_TASK);
        BroadcastUtility.register(this, mReceiver, VarsReceiver.TASK_CLICK);
        BroadcastUtility.register(this, mReceiver, VarsReceiver.SELECT_FILE_INAPP);
    }

    @Override
    protected void onPause() {
        super.onPause();

    }

    private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            switch (intent.getAction()) {
                case VarsReceiver.COMMENT: {
                    String type = intent.getStringExtra("type");
                    switch (type) {
                        case "attachfile": {
                            fileComment();
                            break;
                        }
                        case "reply": {
                            Comment comment = new Gson().fromJson(intent.getStringExtra("obj"), Comment.class);
                            replyComment(comment);
                            break;
                        }
                        case "like": {
                            Comment comment = new Gson().fromJson(intent.getStringExtra("obj"), Comment.class);

                            for (int i = 0; i < lstComment.size(); i++) {
                                if (comment.getID().equals(lstComment.get(i).getID())) {
                                    lstComment.get(i).setLiked(comment.isLiked() == 1 ? 0 : 1);

                                    if (lstComment.get(i).isLiked() == 1) {
                                        lstComment.get(i).setLikeCount(comment.getLikeCount() + 1);
                                    } else {
                                        lstComment.get(i).setLikeCount(Math.max(lstComment.get(i).getLikeCount() - 1, 0));
                                    }
                                }
                            }

                            detailWorkflowPresenter.addLikeComment(comment, comment.getID(), comment.isLiked() != 1, 32);
                            break;
                        }
                        case "comment": {
                            String lstAtt = intent.getStringExtra("lstAttachfile");
                            String content = intent.getStringExtra("content");
                            sendComment(content, lstAtt);
                            break;
                        }
                        case "delete": {
                            AttachFile file = new Gson().fromJson(intent.getStringExtra("obj"), AttachFile.class);
                            lstAttachComment.removeIf(r -> r.getPath().equals(file.getPath()));
                            componentComment.updateListParentAttach(lstAttachComment);
                            break;
                        }
                    }
                    break;
                }
                case VarsReceiver.REFRESHCOMMENT: {
                    String type = intent.getStringExtra("type");
                    if ("like".equals(type)) {
                        componentComment.notifyDataCommentChange();
                    } else {
                        presenter.getDetailOtherResource(OtherResourceId, workflowItem, CommentChanged, "32");
                    }
                    break;
                }
                case VarsReceiver.CREATE_TASK:
                case VarsReceiver.CREATE_CHILD_TASK: {
                    showProgressDialog();
                    presenter.checkWorkflowPermisson(workflowItem, taskId, !isClickFromAction);
                    break;
                }
                case VarsReceiver.TASK_CLICK: {
                    presenter.gotoTask(DetailCreateTaskActivity.this, launcher, intent);
                    break;
                }
                case VarsReceiver.SELECT_FILE_INAPP: {
                    selectFileInApp(intent);
                    break;
                }
            }
        }
    };

    private void selectFileInApp(Intent intent) {
        DownloadedFiles file = new Gson().fromJson(intent.getStringExtra("file"), DownloadedFiles.class);
        AttachFile attachFile = DetailFunc.share.getAttachFileFromURI(this, Uri.fromFile(new File(file.getPath())));
        String type = intent.getStringExtra("type");

        switch (type) {
            case "inputattachmenthorizon": {
                if (DetailFunc.share.checkFileExits(lstAttachFileFull, attachFile)) {
                    Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("TEXT_EXISTINGFILE", "File ???? t???n t???i trong danh s??ch"), this);
                    break;
                }

                lstAttachFileFull.add(attachFile);
                adapterAttachment.notifyDataSetChanged();
                recyAttachFile.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, Functions.share.convertDpToPixel(60, this) * lstAttachFileFull.size()));

                break;
            }
            case "comment": {
                if (DetailFunc.share.checkFileExits(lstAttachComment, attachFile)) {
                    Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("TEXT_EXISTINGFILE", "File ???? t???n t???i trong danh s??ch"), this);
                    break;
                }

                lstAttachComment.add(attachFile);
                componentComment.updateListParentAttach(lstAttachComment);
                break;
            }
        }
    }

    // Update detail khi th???c hi???n action trong Task
    private final ActivityResultLauncher<Intent> launcher = registerForActivityResult(
            new ActivityResultContracts.StartActivityForResult(),
            result -> {
                if (result.getResultCode() == VarsReceiver.TASK_RESULT) {
                    showProgressDialog();
                    presenter.checkWorkflowPermisson(workflowItem, taskId, !isClickFromAction);
                }
            });

    private void sendComment(String content, String lstAttachFile) {
        showProgressDialog();
        ArrayList<AttachFile> files = new Gson().fromJson(lstAttachFile, new TypeToken<ArrayList<AttachFile>>() {
        }.getType());

        detailWorkflowPresenter.sendComment(content, files, OtherResourceId, "16", null);
    }

    private void replyComment(Comment comment) {
        ReplyCommentFragment reply = new ReplyCommentFragment(this, comment, lstComment, OtherResourceId, "16");
        sBaseActivity.showFragment(R.id.container, getSupportFragmentManager(), reply, "ReplyCommentFragment", false);
    }

    private void fileComment() {
        SharedView_PopupChooseFile popupChooseFile = new SharedView_PopupChooseFile(getLayoutInflater(),
                this,
                "DetailCreateTask"
                , lnAll, Constants.mTaskComment
                , Constants.mTaskCommentCamera,
                Vars.FlagView.DetailCreateTask_Comment);

        popupChooseFile.initializeView();
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, @Nullable Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        try {
            if ((requestCode == Constants.mTaskAttachment || requestCode == Constants.mTaskAttachmentCamera) && resultCode == RESULT_OK) {
                AttachFile beanAttachFile = new AttachFile();
                if (requestCode == Constants.mTaskAttachment) // ch???n file th?????ng
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
                    Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("TEXT_ACTIONFAIL", "Thao t??c kh??ng th???c hi???n ???????c. Vui l??ng th??? l???i!"),
                            this);
                    return;
                }

                if (DetailFunc.share.checkFileExits(lstAttachFileFull, beanAttachFile)) {
                    Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("TEXT_EXISTINGFILE", "File ???? t???n t???i trong danh s??ch"), this);
                    return;
                }

                lstAttachFileFull.add(beanAttachFile);
                adapterAttachment.notifyDataSetChanged();
                recyAttachFile.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, Functions.share.convertDpToPixel(60, this) * lstAttachFileFull.size()));
            } else if ((requestCode == Constants.mTaskComment || requestCode == Constants.mTaskCommentCamera) && resultCode == RESULT_OK) {
                // ????nh k??m comment
                AttachFile beanAttachFile = new AttachFile();
                if (requestCode == Constants.mTaskComment) {
                    beanAttachFile = DetailFunc.share.getAttachFileFromURI(this, data.getData());
                } else {
                    if (uri != null) {
                        beanAttachFile = DetailFunc.share.getAttachFileFromURICamera(this, uri);
                    } else {
                        beanAttachFile.setPath("");
                    }
                }

                if (Functions.isNullOrEmpty(beanAttachFile.getPath())) {
                    Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("TEXT_ACTIONFAIL", "Thao t??c kh??ng th???c hi???n ???????c. Vui l??ng th??? l???i!"),
                            this);
                    return;
                }

                if (DetailFunc.share.checkFileExits(lstAttachComment, beanAttachFile)) {
                    Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("TEXT_EXISTINGFILE", "File ???? t???n t???i trong danh s??ch"), this);
                    return;
                }

                if (componentComment != null) {
                    lstAttachComment.add(beanAttachFile);
                    componentComment.updateListParentAttach(lstAttachComment);
                }
            }
        } catch (Exception ex) {
            Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("TEXT_ACTIONFAIL", "Thao t??c kh??ng th???c hi???n ???????c. Vui l??ng th??? l???i!"),
                    this);
            Log.d("ERR onActivityResult", ex.getMessage());
        }
    }

    @Override
    public void OnOtherResourceSuccess(String _OtherResourceId) {
        // ko xai cua DetailWofklowActivity
    }

    @Override
    public void OnCommentSuccess() {
        componentComment.clearContent();
        presenter.getDetailOtherResource(OtherResourceId, workflowItem, CommentChanged, "32");
    }

    @Override
    public void OnLikeCommentSuccess() {
        componentComment.updateListComment(lstComment);
    }

    @Override
    public void OnLikeCommentErr(String err) {
    }

    @Override
    public void OnCommentErr(String err) {
        hideProgressDialog();
        Utility.share.showAlertWithOnlyOK(err, Functions.share.getTitle("TEXT_CLOSE", "????ng"), DetailCreateTaskActivity.this);
    }

    @Override
    public void OnGetCommentTaskSuccess(String otherResourceId, ArrayList<Comment> comments) {
        hideProgressDialog();
        this.OtherResourceId = otherResourceId;
        lstComment.clear();
        lstComment.addAll(comments);
        componentComment.updateListComment(lstComment);
    }

    @Override
    public void OnDeleteSuccess() {
        apiBPM.updateDataSubmitAction(false);
    }

    @Override
    public void OnDeleteErr(String err) {
        hideProgressDialog();
        Utility.share.showAlertWithOnlyOK(err, Functions.share.getTitle("TEXT_CLOSE", "Close"), this);
    }

    @Override
    public void OnRefreshSuccess() {
        hideProgressDialog();
        Intent i = new Intent();
        i.setAction(VarsReceiver.CREATE_TASK);
//        if (isDeleteChildTask) {
//            i.setAction(VarsReceiver.DELETE_CHILD_TASK);
//        } else {
//            i.setAction(VarsReceiver.CREATE_TASK);
//        }

        //setResult(VarsReceiver.TASK_RESULT, i);
        BroadcastUtility.send(this, i);
        BroadcastUtility.unregister(this, mReceiver);
        finish();
    }

    @Override
    public void OnRefreshErr() {
        hideProgressDialog();
    }
}
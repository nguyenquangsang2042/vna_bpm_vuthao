package com.vuthao.bpmop.detail;

import android.annotation.SuppressLint;
import android.app.Dialog;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.graphics.Color;
import android.graphics.drawable.ColorDrawable;
import android.net.Uri;
import android.os.Bundle;
import android.util.DisplayMetrics;
import android.util.Log;
import android.view.Gravity;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.activity.result.ActivityResultLauncher;
import androidx.activity.result.contract.ActivityResultContracts;
import androidx.annotation.Nullable;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.LinearSmoothScroller;
import androidx.recyclerview.widget.RecyclerView;
import androidx.recyclerview.widget.StaggeredGridLayoutManager;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.AnimationController;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.DownloadFile;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.api.ApiBPM;
import com.vuthao.bpmop.base.api.ApiWorkflowController;
import com.vuthao.bpmop.base.model.app.Notify;
import com.vuthao.bpmop.base.model.custom.AttachFileCategory;
import com.vuthao.bpmop.base.model.custom.DownloadedFiles;
import com.vuthao.bpmop.base.model.custom.FormDetailInfo;
import com.vuthao.bpmop.base.model.custom.GridDetails;
import com.vuthao.bpmop.base.model.custom.WorkFlowRelated;
import com.vuthao.bpmop.base.model.app.WorkflowItem;
import com.vuthao.bpmop.base.model.custom.AttachFile;
import com.vuthao.bpmop.base.model.custom.ButtonAction;
import com.vuthao.bpmop.base.model.custom.Comment;
import com.vuthao.bpmop.base.model.custom.ObjectSubmitAction;
import com.vuthao.bpmop.base.model.custom.Task;
import com.vuthao.bpmop.base.model.custom.WFDetailsHeader;
import com.vuthao.bpmop.base.model.custom.WorkflowHistory;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.model.dynamic.ViewRow;
import com.vuthao.bpmop.base.model.dynamic.ViewSection;
import com.vuthao.bpmop.base.realm.AppBaseController;
import com.vuthao.bpmop.base.realm.RealmUtility;
import com.vuthao.bpmop.base.vars.Variable;
import com.vuthao.bpmop.base.vars.VarsControl;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.core.Vars;
import com.vuthao.bpmop.core.adapter.FormControlInputAttachmentHorizontalAdapter;
import com.vuthao.bpmop.core.adapter.TemplateValueTypeAdapter;
import com.vuthao.bpmop.core.component.ComponentButtonBot;
import com.vuthao.bpmop.detail.adapter.DetailDynamicControlAdapter;
import com.vuthao.bpmop.detail.custom.DetailFunc;
import com.vuthao.bpmop.detail.fragment.DetailAttachFileFragment;
import com.vuthao.bpmop.detail.fragment.ReplyCommentFragment;
import com.vuthao.bpmop.detail.fragment.ShareFragment;
import com.vuthao.bpmop.detail.fragment.WorkflowHistoryFragment;
import com.vuthao.bpmop.detail.presenter.DetailWorkflowPresenter;
import com.vuthao.bpmop.shareview.SharedView_PopupChooseFile;

import org.json.JSONArray;
import org.json.JSONObject;

import java.io.File;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.stream.Collectors;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;

public class DetailWorkflowActivity extends BaseActivity implements DetailWorkflowPresenter.WorkflowItemListener, DetailWorkflowPresenter.DetailGridsListener, ApiWorkflowController.WorkflowHistoryListener, DetailWorkflowPresenter.WorkflowControlDynamicListener, ApiBPM.ApiBPMRefreshListener, DetailWorkflowPresenter.DetailWorkflowBottomActionListener, DetailWorkflowPresenter.CommentListener, DetailWorkflowPresenter.FollowListener, ApiWorkflowController.DetailWorkflowListener {
    @BindView(R.id.ln_ViewDetailWorkflow_All)
    LinearLayout lnAll;
    @BindView(R.id.tv_ViewDetailWorkflow_Name)
    TextView tvTitle;
    @BindView(R.id.img_ViewDetailWorkflow_Back)
    ImageView imgBack;
    @BindView(R.id.img_ViewDetailWorkflow_Proce)
    ImageView imgProcess;
    @BindView(R.id.img_ViewDetailWorkflow_AttachFile)
    ImageView imgAttachFile;
    @BindView(R.id.img_ViewDetailWorkflow_Comment)
    ImageView imgComment;
    @BindView(R.id.img_ViewDetailWorkflow_Share)
    ImageView imgShare;
    @BindView(R.id.img_ViewDetailWorkflow_Subcribe)
    ImageView imgSubcribe;
    @BindView(R.id.ln_ViewDetailWorkflow_ActionAll)
    LinearLayout lnActionAll;
    @BindView(R.id.recy_ViewDetailWorkflow_Data)
    RecyclerView recyData;
    @BindView(R.id.ln_ViewDetailWorkflow_Data)
    LinearLayout lnData;
    @BindView(R.id.ln_ViewDetailWorkflow_NoData)
    LinearLayout lnNoData;
    @BindView(R.id.tv_ViewDetailWorkflow_NoData)
    TextView tvNoData;
    @BindView(R.id.ln_ViewDetailWorkflow_TaskName)
    LinearLayout relaTaskName;
    @BindView(R.id.ln_ViewDetailWorkflow_ItemAll)
    LinearLayout lnItemAll;
    @BindView(R.id.tv_ViewDetailWorkflow_Avatar)
    TextView tvItemAvatar;
    @BindView(R.id.img_ViewDetailWorkflow_Avatar)
    ImageView imgAvatar;
    @BindView(R.id.tv_ViewDetailWorkflow_Title)
    TextView tvItemTitle;
    @BindView(R.id.tv_ViewDetailWorkflow_Time)
    TextView tvItemTime;
    @BindView(R.id.tv_ViewDetailWorkflow_Description)
    TextView tvItemDescription;
    @BindView(R.id.img_ViewDetailWorkflow_Flag)
    ImageView imgItemFlag;
    @BindView(R.id.img_ViewDetailWorkflow_ItemAttachFile)
    ImageView imgItemAttach;

    private String workflowItemId;
    private WorkflowItem workflowItem;
    private Notify notifyItem;
    private DetailWorkflowPresenter presenter;
    private ArrayList<WorkflowHistory> workflowHistories;
    private ArrayList<AttachFile> attachfilesDelete;
    private ArrayList<ViewSection> sections;
    private ArrayList<WorkFlowRelated> workflowRelaties;
    private ArrayList<Task> tasks;
    private ArrayList<Comment> comments;
    private ArrayList<AttachFile> files;
    private ArrayList<AttachFile> attachComments;
    private ViewElement elementAttachment;
    private ViewRow actions;
    private DetailDynamicControlAdapter dynamicControlAdapter;
    private ComponentButtonBot componentButtonBot;
    private AnimationController animationController;
    private TemplateValueTypeAdapter templateControlGridAdapter;
    private String commentChanged;
    private String otherResourceId = "";
    public Uri fileCamera = null;
    private ApiBPM apiBPM;
    private LinearLayoutManager mLayoutManager;
    // List lưu những Element có edit để send lên API
    private ArrayList<ViewElement> elementEdits;
    // List lưu lại những item nào đã bị xóa ra khỏi Control InputgridDetail
    public ArrayList<JSONObject> gridEdits;
    private ArrayList<JSONObject> JObjectRow;
    private ArrayList<AttachFile> attachsFull;
    private ArrayList<AttachFileCategory> fileCategories;
    private GridDetails gridDetails;
    private Dialog dialog;

    private int maxLinesTvPeople = 1;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_detail_workflow);
        ButterKnife.bind(this);

        init();
        getWorkflowItem();
    }

    private void getWorkflowItem() {
        showProgressDialog();
        Bundle bundle = getIntent().getExtras();
        workflowItemId = bundle.getString("WorkflowItemId");
        workflowItem = new AppBaseController().getWorkflowItem(workflowItemId);
        if (workflowItem == null) {
            presenter.getWorkflowItemById(workflowItemId);
        } else {
            setView();
            getFormDefineInfo();
            getWorkflowHistory();
        }
    }

    private void init() {
        animationController = new AnimationController();
        apiBPM = new ApiBPM(this, 3);
        presenter = new DetailWorkflowPresenter(this, this, this, this, this, this, this, this);
        actions = new ViewRow();
        sections = new ArrayList<>();
        workflowRelaties = new ArrayList<>();
        tasks = new ArrayList<>();
        comments = new ArrayList<>();
        attachComments = new ArrayList<>();
        workflowHistories = new ArrayList<>();
        attachfilesDelete = new ArrayList<>();
        attachsFull = new ArrayList<>();
        gridEdits = new ArrayList<>();
        elementEdits = new ArrayList<>();
        JObjectRow = new ArrayList<>();
        files = new ArrayList<>();

        mLayoutManager = new LinearLayoutManager(this, RecyclerView.VERTICAL, false);
    }

    private void registerReceiver() {
        BroadcastUtility.register(this, mReceiver, VarsReceiver.BOTTOM);
        BroadcastUtility.register(this, mReceiver, VarsReceiver.FORMCLICK);
        BroadcastUtility.register(this, mReceiver, VarsReceiver.INNERACTIONCLICK);
        BroadcastUtility.register(this, mReceiver, VarsReceiver.COMMENT);
        BroadcastUtility.register(this, mReceiver, VarsReceiver.CHILDACTION);
        BroadcastUtility.register(this, mReceiver, VarsReceiver.REFRESHCOMMENT);
        BroadcastUtility.register(this, mReceiver, VarsReceiver.SUBMITACTION);
        BroadcastUtility.register(this, mReceiver, VarsReceiver.UPDATEFORM);
        BroadcastUtility.register(this, mReceiver, VarsReceiver.UPDATECHILDACTION);
        BroadcastUtility.register(this, mReceiver, VarsReceiver.INBOTTOMMORE);
        BroadcastUtility.register(this, mReceiver, VarsReceiver.CREATE_TASK);
        BroadcastUtility.register(this, mReceiver, VarsReceiver.FLOW_RELATE_CLICK);
        BroadcastUtility.register(this, mReceiver, VarsReceiver.TASK_CLICK);
        BroadcastUtility.register(this, mReceiver, VarsReceiver.SELECT_FILE_INAPP);
        //BroadcastUtility.register(this, mReceiver, VarsReceiver.DELETE_CHILD_TASK);
    }

    @SuppressLint("SetTextI18n")
    private void setView() {
        tvNoData.setText(Functions.share.getTitle("K_Mess_NoData", "Không có dữ liệu"));
        tvTitle.setText(workflowItem.getContent());

        if (!Functions.isNullOrEmpty(workflowItem.getAssignedTo())) {
            String value = workflowItem.getAssignedTo().split(",")[0].toLowerCase();
            RealmUtility.share.setAvatarUser(this, value, "ID", imgAvatar, tvItemAvatar);
        } else {
            tvItemAvatar.setVisibility(View.INVISIBLE);
        }

        if (!Functions.isNullOrEmpty(workflowItem.getCreatedByName())) {
            tvItemTitle.setText(workflowItem.getCreatedByName());
        } else {
            tvItemTitle.setVisibility(View.INVISIBLE);
        }

        if (!Functions.isNullOrEmpty(workflowItem.getCreated())) {
            tvItemTime.setText(Functions.share.formatDateLanguage(workflowItem.getCreated()));
        } else {
            tvItemTime.setVisibility(View.INVISIBLE);
        }

        if (!Functions.isNullOrEmpty(workflowItem.getAssignedTo())) {
            String[] arr = Functions.share.getArrayFullNameFromArrayID(workflowItem.getAssignedTo().toLowerCase().split(","));

            switch (workflowItem.getStatusGroup()) {
                case (int) Variable.AppStatusID.Completed: {
                    tvItemDescription.setText(Functions.share.getTitle("TEXT_TITLE_APPROVED", "Phê duyệt: ") + arr[0]);
                    break;
                }
                case (int) Variable.AppStatusID.Canceled: {
                    tvItemDescription.setText(Functions.share.getTitle("TEXT_TITLE_CANCEL", "Hủy: ") + arr[0]);
                    break;
                }
                case (int) Variable.AppStatusID.Rejected: {
                    tvItemDescription.setText(Functions.share.getTitle("TEXT_TITLE_REJECTED", "Từ chối: ") + arr[0]);
                    break;
                }
                case (int) Variable.AppStatusID.Draft: {
                    tvItemDescription.setVisibility(View.INVISIBLE);
                    break;
                }
                default: {
                    Functions.share.setTextView_FormatMultiUser_DetailWorkflow(tvItemDescription, arr, true);
                    break;
                }
            }
        } else {
            tvItemDescription.setVisibility(View.INVISIBLE);
        }

        imgItemFlag.setVisibility(View.INVISIBLE);

        if (workflowItem.isFollow()) {
            imgSubcribe.setImageResource(R.drawable.icon_ver2_star_checked);
            imgSubcribe.setColorFilter(ContextCompat.getColor(this, R.color.clYellow));
        } else {
            imgSubcribe.setImageResource(R.drawable.icon_ver2_star_unchecked);
            imgSubcribe.setColorFilter(ContextCompat.getColor(this, R.color.clBottomDisable));
        }

        imgItemAttach.setVisibility(View.INVISIBLE);
    }

    private void getFormDefineInfo() {
        presenter.getTicketRequestControlDynamicForm(workflowItem);
        presenter.getGridsDetails(workflowItemId);
    }

    private void getWorkflowHistory() {
        presenter.getListProcessHistory(workflowItemId);
    }

    @OnClick({R.id.img_ViewDetailWorkflow_Back, R.id.img_ViewDetailWorkflow_AttachFile,
            R.id.img_ViewDetailWorkflow_Proce, R.id.img_ViewDetailWorkflow_Subcribe,
            R.id.img_ViewDetailWorkflow_Share, R.id.img_ViewDetailWorkflow_Comment,
    R.id.tv_ViewDetailWorkflow_Description})
    public void OnViewClicked(View v) {
        switch (v.getId()) {
            case R.id.img_ViewDetailWorkflow_Back: {
                imgBack.startAnimation(animationController.fadeIn(this));
                BroadcastUtility.unregister(this, mReceiver);
                finish();
                break;
            }
            case R.id.img_ViewDetailWorkflow_AttachFile: {
                imgAttachFile.startAnimation(animationController.fadeIn(this));
                detailAttachment();
                break;
            }
            case R.id.img_ViewDetailWorkflow_Proce: {
                imgProcess.startAnimation(animationController.fadeIn(this));
                WorkflowHistoryFragment process = new WorkflowHistoryFragment(workflowHistories, workflowItem, notifyItem);
                showFragment(R.id.container, getSupportFragmentManager(), process, "DetailWorkflowHistoryFragment", false);
                break;
            }
            case R.id.img_ViewDetailWorkflow_Subcribe: {
                imgSubcribe.startAnimation(animationController.fadeIn(this));
                follow();
                break;
            }
            case R.id.img_ViewDetailWorkflow_Share: {
                imgShare.startAnimation(animationController.fadeIn(this));
                ShareFragment share = new ShareFragment(workflowItem);
                showFragment(R.id.container, getSupportFragmentManager(), share, "ShareFragment", false);
                break;
            }
            case R.id.img_ViewDetailWorkflow_Comment: {
                imgComment.startAnimation(animationController.fadeIn(this));
                if (dynamicControlAdapter != null) {
                    RecyclerView.SmoothScroller smoothScroller = new LinearSmoothScroller(this) {
                        @Override
                        protected int getVerticalSnapPreference() {
                            return LinearSmoothScroller.SNAP_TO_START;
                        }
                    };
                    smoothScroller.setTargetPosition(dynamicControlAdapter.getItemCount() - 1);
                    mLayoutManager.startSmoothScroll(smoothScroller);
                }
                break;
            }
            case R.id.tv_ViewDetailWorkflow_Description: {
                showFullUserAssignedTo();
                break;
            }
        }
    }

    private void showFullUserAssignedTo() {
        int[] arr = new int[] {10, -1, 6};
        if (Arrays.stream(arr).anyMatch(r -> r == workflowItem.getActionStatusID())) {
            return;
        }

        if (!workflowItem.getAssignedTo().contains(",")) {
            return;
        }

        tvItemDescription.startAnimation(animationController.fadeIn(this));

        tvItemDescription.setMaxLines(maxLinesTvPeople == 1 ? Integer.MAX_VALUE : 1);

        String[] fullName = Functions.share.getArrayFullNameFromArrayID(workflowItem.getAssignedTo().split(","));
        Functions.share.setTextView_FormatMultiUser_DetailWorkflow(tvItemDescription, fullName, Functions.isNullOrEmpty(tvItemDescription.getText().toString()) || !tvItemDescription.getText().toString().contains("+"));
    }

    private void follow() {
        String message = Functions.share.getTitle(workflowItem.isFollow() ? "MESS_UNFOLLOW_TASK" : "MESS_FOLLOW_TASK", "Đặt theo dõi công việc này?");
        Utility.share.showAlertWithOKCancel(message, "", Functions.share.getTitle("TEXT_AGREE", "Đồng ý"), Functions.share.getTitle("TEXT_CANCEL", "Hủy"), this, () -> {
            presenter.updateOrInsertFollow(workflowItem, !workflowItem.isFollow());
            presenter.updateFollow(workflowItem);
            workflowItem.setFollow(!workflowItem.isFollow());
            if (workflowItem.isFollow()) {
                imgSubcribe.setImageResource(R.drawable.icon_ver2_star_checked);
                imgSubcribe.setColorFilter(ContextCompat.getColor(DetailWorkflowActivity.this, R.color.clYellow));
            } else {
                imgSubcribe.setImageResource(R.drawable.icon_ver2_star_unchecked);
                imgSubcribe.setColorFilter(ContextCompat.getColor(DetailWorkflowActivity.this, R.color.clBottomDisable));
            }

            Intent intent = new Intent();
            intent.setAction(VarsReceiver.FOLLOW);
            intent.putExtra("WorkflowId", workflowItem.getID());
            intent.putExtra("isFollow", workflowItem.isFollow());
            BroadcastUtility.send(this, intent);
        });
    }

    private void detailAttachment() {
        if (imgAttachFile.getAlpha() != 0.2f) {
            DetailAttachFileFragment detailAttachFileFragment = new DetailAttachFileFragment(elementAttachment);
            showFragment(R.id.container, getSupportFragmentManager(), detailAttachFileFragment, "DetailAttachFileFragment", false);
        }
    }

    private void setInfoWorkflowHistory() {
        // Da phe duyet - Da huy - Da tu choi
        if (workflowItem.getActionStatusID() == 10 || workflowItem.getActionStatusID() == -1 || workflowItem.getActionStatusID() == 6) {
            if (!Functions.isNullOrEmpty(workflowItem.getCreatedBy())) {
                String info = workflowItem.getCreatedBy().toLowerCase().split(",")[0];
                RealmUtility.share.setAvatarUser(this, info, "ID", imgAvatar, tvItemAvatar);
            }

            if (!Functions.isNullOrEmpty(workflowItem.getCreatedByName())) {
                tvItemTitle.setText(workflowItem.getCreatedByName());
            }
        } else if (workflowHistories.size() > 0) {
            if (!Functions.isNullOrEmpty(workflowHistories.get(0).getFromUserId())) {
                String info = workflowHistories.get(0).getFromUserId().toLowerCase().split(",")[0];
                RealmUtility.share.setAvatarUser(this, info, "ID", imgAvatar, tvItemAvatar);
            }

            if (!Functions.isNullOrEmpty(workflowHistories.get(0).getFromUserName())) {
                tvItemTitle.setVisibility(View.VISIBLE);
                tvItemTitle.setText(workflowHistories.get(0).getFromUserName());
            }
        }
    }

    private void setStatusBar() {
        workflowItem.setFollow(sections.get(0).isFollow());
        if (workflowItem.isFollow()) {
            imgSubcribe.setImageResource(R.drawable.icon_ver2_star_checked);
            imgSubcribe.setColorFilter(ContextCompat.getColor(this, R.color.clYellow));
        } else {
            imgSubcribe.setImageResource(R.drawable.icon_ver2_star_unchecked);
            imgSubcribe.setColorFilter(ContextCompat.getColor(this, R.color.clBottomDisable));
        }

        elementAttachment = presenter.getElementAttachFromGrids(gridDetails);
        if (elementAttachment != null) {
            files = presenter.getAttachFiles(elementAttachment.getValue());
            if (files.isEmpty()) {
                imgAttachFile.setAlpha(0.2f);
            }
        } else {
            imgAttachFile.setAlpha(0.2f);
        }
    }

    @Override
    public void OnGetWorkflowDynamicSucess(FormDetailInfo formDetailInfo) {

    }

    @Override
    public void OnGetGridsSuccess(FormDetailInfo formDetailInfo, GridDetails grids) {
        gridDetails = grids;
        sections = formDetailInfo.getForm();
        actions = formDetailInfo.getAction();

        if (formDetailInfo.getMoreInfo() != null) {
            commentChanged = formDetailInfo.getMoreInfo().getCommentChanged();
            otherResourceId = formDetailInfo.getMoreInfo().getOtherResourceId();
        }

        ArrayList<ViewRow> rows = new ArrayList<>();

        if (gridDetails.getAttachment() != null) {
            rows.addAll(gridDetails.getAttachment());
        }

        if (gridDetails.getDetails() != null) {
            rows.addAll(gridDetails.getDetails());
        }

        if (gridDetails.getRelated() != null) {
            workflowRelaties = new ArrayList<>(gridDetails.getRelated());
        }

        if (gridDetails.getTask() != null) {
            tasks = new ArrayList<>(gridDetails.getTask());
        }

        presenter.mappingFields(sections, rows);

        setStatusBar();
        setInfoWorkflowHistory();
        setDynamicFormDefineInfo();
        setDynamicButtomAction();

        lnItemAll.setVisibility(View.VISIBLE);
        hideProgressDialog();
    }

    private void setDynamicFormDefineInfo() {
        if (sections.size() > 0) {
            dynamicControlAdapter = new DetailDynamicControlAdapter(this, this, Vars.FlagViewControlAttachment.DetailWorkflow, sections, workflowRelaties, tasks, comments, workflowItem, notifyItem);
            recyData.setAdapter(dynamicControlAdapter);
            recyData.setLayoutManager(mLayoutManager);
            recyData.setItemViewCacheSize(dynamicControlAdapter.getItemCount());
        }
    }

    private void setDynamicButtomAction() {
        if (actions.getElements() != null && actions.getElements().size() > 0) {
            lnActionAll.setVisibility(View.VISIBLE);
            lnActionAll.removeAllViews();

            componentButtonBot = new ComponentButtonBot(this, lnActionAll, actions, getResources());
            componentButtonBot.initializeFrameView(lnActionAll);
            componentButtonBot.setTitle();
            componentButtonBot.setValue();
            componentButtonBot.setEnable();
            componentButtonBot.setProprety();
        } else {
            lnActionAll.setVisibility(View.GONE);
        }
    }

    @Override
    public void OnGetWorkflowErr(String err) {
        hideProgressDialog();
        Utility.share.showAlertWithOnlyOK(err, Functions.share.getTitle("TEXT_AGREE", "Đồng ý"), this, this::onBackPressed);
    }

    @Override
    public void OnFollowSuccess() {
    }

    @Override
    public void OnFollowErr(String err) {
        Utility.share.showAlertWithOnlyOK(err, "", Functions.share.getTitle("TEXT_CANCLE", "Đồng ý"), this, new Utility.OkListener() {
            @Override
            public void onOkListener() {

            }
        });
    }

    protected void onResume() {
        super.onResume();
        registerReceiver();
    }

    @Override
    protected void onPause() {
        super.onPause();
    }

    private void handleBottomClicks(String strElement) {
        presenter.handleBottomClicks(this, launcher, sections, workflowItem, workflowHistories, lnAll, strElement, componentButtonBot);
    }

    private void bottomActionClick(ButtonAction buttonAction) {
        presenter.bottomActionClick(this, launcher, workflowItem, workflowHistories, lnAll, buttonAction);
    }

    private void actionSendAPI(ButtonAction buttonAction, String comment, HashMap<String, String> hashMap) {
        showProgressDialog();
        ArrayList<ObjectSubmitAction> lstSubmitActionData = presenter.getObjectSubmitActions(elementEdits, attachfilesDelete, gridEdits);
        ArrayList<AttachFile> files = presenter.getAttachmentsEdits(elementEdits);

        // Send API
        hashMap.put("func", buttonAction.getValue());
        hashMap.put("fid", workflowItem.getID());
        hashMap.put("data", new Gson().toJson(lstSubmitActionData));
        hashMap.put("lcid", "1066");

        if (!Functions.isNullOrEmpty(comment)) {
            hashMap.put("idea", comment);
        }

        presenter.submitAction(files, hashMap);
    }

    private void handleInnerClicks(String element, int actionId, int positionToAction, int flagViewID) {
        if (flagViewID != Vars.FlagViewControlAttachment.DetailWorkflow) {
            return;
        }

        ViewElement _clickedElement = new Gson().fromJson(element, ViewElement.class);

        switch (_clickedElement.getDataType()) {
            case VarsControl.INPUTATTACHMENTHORIZON:
            case VarsControl.INPUTATTACHMENTVERTICAL: {
                if (actionId == Vars.ControlInputAttachmentVertical_InnerActionID.View) {
                    controlInputAttachmentVertical_InnerAction(_clickedElement, actionId, positionToAction);
                } else if (_clickedElement.isEnable()) {
                    if (actionId == Vars.ControlInputAttachmentVertical_InnerActionID.Create) {
                        elementAttachment = _clickedElement;

                        SharedView_PopupChooseFile sharedView_popupChooseFile = new SharedView_PopupChooseFile(getLayoutInflater(), this, "DetailWorkflow", null, Constants.mDetailAttachment, Constants.mDetailAttachmentCamera, Vars.FlagView.DetailWorkflow_ControlInputAttachmentVertical);
                        sharedView_popupChooseFile.initializeView();
                        break;
                    } else {
                        controlInputAttachmentVertical_InnerAction(_clickedElement, actionId, positionToAction);
                        break;
                    }
                }
                break;
            }
            case VarsControl.INPUTGRIDDETAILS: {
                controlInputGridDetailInnerAction(_clickedElement, actionId, positionToAction);
                break;
            }
        }
    }

    private void controlInputAttachmentVertical_InnerAction(ViewElement clickedElement, int actionId, int position) {
        attachsFull = new Gson().fromJson(clickedElement.getValue(), new TypeToken<ArrayList<AttachFile>>() {
        }.getType());

        switch (actionId) {
            case Vars.ControlInputAttachmentVertical_InnerActionID.Delete: {
                Utility.share.showAlertWithOKCancel(Functions.share.getTitle("TEXT_DELETE_CONFIRM", "Bạn có muốn xóa file này không ?"), "",
                        Functions.share.getTitle("TEXT_AGREE", "Agree"), Functions.share.getTitle("TEXT_CANCEL", "Hủy"), this, () -> {
                            if (!attachsFull.get(position).getID().equals("")) {

                                attachfilesDelete.add(attachsFull.get(position));
                            }

                            attachsFull.remove(position);
                            updateValueForElement(clickedElement, new Gson().toJson(attachsFull), true);
                        });
                break;
            }
            case Vars.ControlInputAttachmentVertical_InnerActionID.Edit: {
                fileCategories = new Gson().fromJson(clickedElement.getDataSource(), new TypeToken<ArrayList<AttachFileCategory>>() {
                }.getType());

                if (fileCategories == null) {
                    fileCategories = new ArrayList<>();
                }

                String selectedCategory = "";
                if (!Functions.isNullOrEmpty(attachsFull.get(position).getAttachTypeName())) {
                    selectedCategory = attachsFull.get(position).getAttachTypeName();
                }

                if (fileCategories.size() > 0) {
                    if (!Functions.isNullOrEmpty(selectedCategory)) {
                        for (int i = 0; i < fileCategories.size(); i++) {
                            if (fileCategories.get(i).getTitle().equals(selectedCategory)) {
                                fileCategories.get(i).setSelected(true);
                            }
                        }
                    }
                }

                View view = getLayoutInflater().inflate(R.layout.popup_control_single_choice, null);
                ImageView imgClose = view.findViewById(R.id.img_PopupControl_SingleChoice_Close);
                TextView tvTitle = view.findViewById(R.id.tv_PopupControl_SingleChoice_Title);
                TextView tvNoData = view.findViewById(R.id.tv_PopupControl_SingleChoice_NoData);
                RecyclerView recyData = view.findViewById(R.id.recy_PopupControl_SingleChoice_Data);
                ImageView imgDone = view.findViewById(R.id.img_PopupControl_SingleChoice_Done);

                tvNoData.setText(Functions.share.getTitle("TEXT_NODATA", "No data"));

                imgDone.setVisibility(View.INVISIBLE);
                if (!Functions.isNullOrEmpty(clickedElement.getTitle())) {
                    tvTitle.setText(clickedElement.getTitle());
                }

                if (fileCategories.size() > 0) {
                    tvNoData.setVisibility(View.GONE);
                    FormControlInputAttachmentHorizontalAdapter adapter = new FormControlInputAttachmentHorizontalAdapter(this, fileCategories, pos -> {
                        attachsFull.get(position).setAttachTypeId(fileCategories.get(pos).getID());
                        attachsFull.get(position).setAttachTypeName(fileCategories.get(pos).getTitle());
                        updateValueForElement(clickedElement, new Gson().toJson(attachsFull), true);
                        dialog.dismiss();
                    });
                    StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, StaggeredGridLayoutManager.VERTICAL);
                    recyData.setAdapter(adapter);
                    recyData.setLayoutManager(staggeredGridLayoutManager);
                } else {
                    tvNoData.setVisibility(View.VISIBLE);
                }

                imgClose.setOnClickListener(view1 -> {
                    dialog.dismiss();
                });

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

                break;
            }
            case Vars.ControlInputAttachmentVertical_InnerActionID.View: {
                // mở file từ server
                if (!attachsFull.get(position).getID().equals("")) {
                    new DownloadFile().execute(Constants.BASE_URL + attachsFull.get(position).getPath() + ";#" + attachsFull.get(position).getTitle() + "." + attachsFull.get(position).getExtension());
                } else {
                    if (new File(attachsFull.get(position).getPath()).exists()) {
                        DetailFunc.share.openFile(this, (attachsFull.get(position).getPath()));
                    }
                }
                break;
            }
        }
    }

    private void controlInputGridDetailInnerAction(ViewElement clickedElement, int actionId, int positionToAction) {
        View view = getLayoutInflater().inflate(R.layout.popup_control_input_grid_detail, null);
        TextView tvTitle = view.findViewById(R.id.tv_PopupControl_InputGridDetail_Title);
        RecyclerView recyContent = view.findViewById(R.id.recy_PopupControl_InputGridDetail_Content);
        ImageView imgDone = view.findViewById(R.id.img_PopupControl_InputGridDetail_Done);
        ImageView imgDelete = view.findViewById(R.id.img_PopupControl_InputGridDetail_Delete);
        ImageView imgBack = view.findViewById(R.id.img_PopupControl_InputGridDetail_Back);

        tvTitle.setText(clickedElement.getTitle());

        if (clickedElement.isEnable()) {
            imgDelete.setVisibility(View.VISIBLE);
            imgDone.setVisibility(View.VISIBLE);
        } else {
            imgDelete.setVisibility(View.GONE);
            imgDone.setVisibility(View.GONE);
        }

        // region dialog
        Dialog dialog = new Dialog(this, R.style.Theme_Custom_BPMOP_Dialog_FullScreen);
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
            s.height = WindowManager.LayoutParams.MATCH_PARENT;
            window.setAttributes(s);
            window.setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));
        // endregion

        imgBack.setOnClickListener(v -> dialog.dismiss());

        if (actionId == Vars.ControlInputGridDetails_InnerActionID.Create) {
            imgDelete.setVisibility(View.GONE);

            ArrayList<JSONObject> ListJObjectRow = new ArrayList<>();
            if (!Functions.isNullOrEmpty(clickedElement.getValue())) {
                try {
                    JSONArray jsonArray = new JSONArray(clickedElement.getValue());
                    for (int i = 0; i < jsonArray.length(); i++) {
                        ListJObjectRow.add(jsonArray.getJSONObject(i));
                    }
                } catch (Exception ex) {
                    Log.d("ERR controlInputGridDetailInnerAction", ex.getMessage());
                }
            }

            templateControlGridAdapter = new TemplateValueTypeAdapter(this, getApplicationContext(), clickedElement, Vars.FlagViewControlAttachment.DetailWorkflow);
            recyContent.setAdapter(templateControlGridAdapter);
            recyContent.setLayoutManager(new StaggeredGridLayoutManager(1, LinearLayout.VERTICAL));

            imgDone.setOnClickListener(v -> {
                JSONObject _currentJObject = templateControlGridAdapter.getCurrentJObject();
                try {
                    for (WFDetailsHeader header : templateControlGridAdapter.lstHeader) {
                        if (header.isRequire() && Functions.isNullOrEmpty(_currentJObject.getString(header.getInternalName()))) {
                            // kiểm tra xem có thằng nào Require mà chưa nhập không
                            Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("K_Action_PleaseChooseUser", "Vui lòng nhập đầy đủ thông tin."),
                                    Functions.share.getTitle("TEXT_CLOSE", "Close"), DetailWorkflowActivity.this);
                            return;
                        }
                    }
                } catch (Exception ex) {
                    Log.d("ERR imgDone", ex.getMessage());
                }

                ListJObjectRow.add(_currentJObject);
                updateValueForElement(clickedElement, ListJObjectRow.toString(), true);
                dialog.dismiss();
            });

        } else if (actionId == Vars.ControlInputGridDetails_InnerActionID.Edit) {
            JObjectRow = new ArrayList<>();

            if (!Functions.isNullOrEmpty(clickedElement.getValue())) {
                try {
                    JSONArray jsonArray = new JSONArray(clickedElement.getValue());
                    for (int i = 0; i < jsonArray.length(); i++) {
                        JObjectRow.add(jsonArray.getJSONObject(i));
                    }
                } catch (Exception ex) {
                    Log.d("ERR Convert JSON TO LIST JSON", ex.getMessage());
                }

                templateControlGridAdapter = new TemplateValueTypeAdapter(this, getApplicationContext(), clickedElement, JObjectRow.get(positionToAction), Vars.FlagViewControlAttachment.DetailWorkflow);
                recyContent.setAdapter(templateControlGridAdapter);
                recyContent.setLayoutManager(new StaggeredGridLayoutManager(1, LinearLayout.VERTICAL));

                imgDelete.setOnClickListener(v -> Utility.share.showAlertWithOKCancel(Functions.share.getTitle("TEXT_DELETE_CONFIRM", "Bạn có chắc muốn xóa file này không ?")
                        , Functions.share.getTitle("TEXT_CANCEL", "Hủy"), Functions.share.getTitle("TEXT_AGREE", "Đồng ý"), DetailWorkflowActivity.this, new Utility.OkListener() {
                            @Override
                            public void onOkListener() {
                                gridEdits.add(JObjectRow.get(positionToAction));
                                JObjectRow.remove(positionToAction);
                                updateValueForElement(clickedElement, JObjectRow.toString(), true);
                                dialog.dismiss();
                            }
                        }));

                imgDone.setOnClickListener(v -> {
                    JSONObject currentJObject = templateControlGridAdapter.getCurrentJObject();
                    try {
                        for (WFDetailsHeader header : templateControlGridAdapter.lstHeader) {
                            if (header.isRequire() && Functions.isNullOrEmpty(currentJObject.getString(header.getInternalName()))) {
                                // kiểm tra xem có thằng nào Require mà chưa nhập không
                                Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("K_Action_PleaseChooseUser", "Vui lòng nhập đầy đủ thông tin."),
                                        Functions.share.getTitle("TEXT_CLOSE", "Close"), DetailWorkflowActivity.this);
                                return;
                            }
                        }
                    } catch (Exception ex) {
                        Log.d("ERR  imgDone.setOnClickListener", ex.getMessage());
                        dialog.dismiss();
                    }

                    JObjectRow.set(positionToAction, currentJObject);
                    updateValueForElement(clickedElement, JObjectRow.toString(), true);
                    dialog.dismiss();
                });
            }
        }
    }

    private void handleChildActionClicks(ViewElement elementParent, ViewElement elementChild, JSONObject jsonOb, int flagView) {
        presenter.handleChildActionClicks(this, lnAll, templateControlGridAdapter, elementParent, elementChild, jsonOb, flagView);
    }

    private void updateValueForPopupGridDetail(ViewElement parentElement, ViewElement childElement, JSONObject jObjectChild, String _newValue) {
        presenter.updateValueForPopupGridDetail(templateControlGridAdapter, parentElement, childElement, jObjectChild, _newValue);
    }

    private void replyComment(Comment comment) {
        ReplyCommentFragment reply = new ReplyCommentFragment(this, comment, comments, otherResourceId, "8");
        showFragment(R.id.container, getSupportFragmentManager(), reply, "ReplyCommentFragment", false);
    }

    private void fileComment() {
        SharedView_PopupChooseFile popupChooseFile = new SharedView_PopupChooseFile(getLayoutInflater(), this, "DetailWorkflow", lnAll, Constants.mDetailComment
                , Constants.mDetailCommentCamera, Vars.FlagView.DetailWorkflow_Comment);
        popupChooseFile.initializeView();
    }

    private void sendComment(String content, String lstAttachFile) {
        showProgressDialog();
        ArrayList<AttachFile> files = new Gson().fromJson(lstAttachFile, new TypeToken<ArrayList<AttachFile>>() {
        }.getType());

        presenter.sendComment(content, files, otherResourceId, "8", null);
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, @Nullable Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        try {
            if ((requestCode == Constants.mDetailComment || requestCode == Constants.mDetailCommentCamera) &&
                    resultCode == RESULT_OK) {
                // COMMENT
                AttachFile attachFile = new AttachFile();
                // chọn file thường
                if (requestCode == Constants.mDetailComment) {
                    assert data != null;
                    attachFile = DetailFunc.share.getAttachFileFromURI(this, data.getData());
                } else {
                    if (fileCamera != null) {
                        attachFile = DetailFunc.share.getAttachFileFromURICamera(this, fileCamera);
                    } else {
                        attachFile.setPath("");
                    }
                }

                if (Functions.isNullOrEmpty(attachFile.getPath())) {
                    Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Vui lòng thử lại!"),
                            this);
                    return;
                }

                if (DetailFunc.share.checkFileExits(attachComments, attachFile)) {
                    Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("TEXT_EXISTINGFILE", "File đã tồn tại trong danh sách"), this);
                    return;
                }

                attachComments.add(attachFile);
                dynamicControlAdapter.updateListAttachment(attachComments);
                dynamicControlAdapter.notifyDataSetChanged();
            } else if ((requestCode == Constants.mDetailAttachment || requestCode == Constants.mDetailAttachmentCamera) &&
                    resultCode == RESULT_OK) {
                AttachFile attachFile = new AttachFile();

                if (requestCode == Constants.mDetailAttachment) {
                    assert data != null;
                    attachFile = DetailFunc.share.getAttachFileFromURI(this, data.getData());
                } else {
                    // chụp từ camera
                    if (fileCamera != null) {
                        attachFile = DetailFunc.share.getAttachFileFromURICamera(this, fileCamera);
                    } else {
                        attachFile.setPath("");
                    }
                }

                if (Functions.isNullOrEmpty(attachFile.getPath())) {
                    Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Vui lòng thử lại!"),
                            this);
                    return;
                }

                if (elementAttachment != null) {
                        ArrayList<AttachFile> lstAttFileControl_Full = new Gson().fromJson(elementAttachment.getValue(), new TypeToken<ArrayList<AttachFile>>() {}.getType());

                    if (lstAttFileControl_Full != null && lstAttFileControl_Full.size() > 0) {
                        if (DetailFunc.share.checkFileExits(lstAttFileControl_Full, attachFile)) {
                            Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("TEXT_EXISTINGFILE", "File đã tồn tại trong danh sách"), this);
                            return;
                        }
                    }

                    assert lstAttFileControl_Full != null;
                    lstAttFileControl_Full.add(attachFile);
                    updateValueForElement(elementAttachment, new Gson().toJson(lstAttFileControl_Full), true);
                }
            }
        } catch (Exception ex) {
            Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Vui lòng thử lại!"), this);
            Log.d("ERR onActivityResult", ex.getMessage());
        }
    }

    private void selectFileInApp(Intent intent) {
        DownloadedFiles file = new Gson().fromJson(intent.getStringExtra("file"), DownloadedFiles.class);
        AttachFile attachFile = DetailFunc.share.getAttachFileFromURI(this, Uri.fromFile(new File(file.getPath())));
        String type = intent.getStringExtra("type");

        switch (type) {
            case "inputattachmenthorizon": {
                if (elementAttachment != null) {
                    ArrayList<AttachFile> lstAttFileControl_Full = new Gson().fromJson(elementAttachment.getValue(), new TypeToken<ArrayList<AttachFile>>() {}.getType());
                    if (lstAttFileControl_Full != null && lstAttFileControl_Full.size() > 0) {
                        if (DetailFunc.share.checkFileExits(lstAttFileControl_Full, attachFile)) {
                            Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("TEXT_EXISTINGFILE", "File đã tồn tại trong danh sách"), this);
                            break;
                        }
                    }

                    assert lstAttFileControl_Full != null;
                    lstAttFileControl_Full.add(attachFile);
                    updateValueForElement(elementAttachment, new Gson().toJson(lstAttFileControl_Full), true);
                }
                break;
            }
            case "comment": {
                if (DetailFunc.share.checkFileExits(attachComments, attachFile)) {
                    Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("TEXT_EXISTINGFILE", "File đã tồn tại trong danh sách"), this);
                    break;
                }

                attachComments.add(attachFile);
                dynamicControlAdapter.updateListAttachment(attachComments);
                dynamicControlAdapter.notifyDataSetChanged();
                break;
            }
        }
    }

    private void updateValueForElement(ViewElement clickedElement, String newValue, boolean notifyDataSetChange) {
        clickedElement.setValue(newValue);
        sections = DetailFunc.share.updateValueElement_InListSection(sections, clickedElement, true);
        updateItemForEditedElement(clickedElement);
        dynamicControlAdapter.updateCurrentList(sections);
        if (notifyDataSetChange) {
            dynamicControlAdapter.notifyDataSetChanged();
        }
    }

    // Update Item vào List đã được Edit để send Action đi
    private void updateItemForEditedElement(ViewElement editedElement) {
        // Nếu Element này đã có trong List Edited -> Update
        for (int i = 0; i < elementEdits.size(); i++) {
            if (elementEdits.get(i).getID().equals(editedElement.getID())) {
                elementEdits.set(i, editedElement);
                return;
            }
        }

        elementEdits.add(editedElement);
    }

    @Override
    public void OnOtherResourceSuccess(String otherResourceId) {
        comments = presenter.getComments(otherResourceId);
        if (dynamicControlAdapter != null) {
            dynamicControlAdapter.updateListComment(comments);
            dynamicControlAdapter.notifyItemChanged(dynamicControlAdapter.getItemCount() - 1);
        }
    }

    @Override
    public void OnCommentSuccess() {
        hideProgressDialog();
        dynamicControlAdapter.clearComment();
        presenter.getDetailOtherResource(otherResourceId, workflowItem, "", "8");
    }

    @Override
    public void OnLikeCommentSuccess() {
        dynamicControlAdapter.notifyItemChanged(dynamicControlAdapter.getItemCount() - 1);
    }

    @Override
    public void OnLikeCommentErr(String err) {
        Utility.share.showAlertWithOnlyOK(err, Functions.share.getTitle("TEXT_CLOSE", "Đóng"), DetailWorkflowActivity.this);
    }

    @Override
    public void OnCommentErr(String err) {
        hideProgressDialog();
        Utility.share.showAlertWithOnlyOK(err, Functions.share.getTitle("TEXT_CLOSE", "Đóng"), DetailWorkflowActivity.this);
    }

    @Override
    public void OnSubmitActionSuccess() {
        // refresh data
        apiBPM.updateDataSubmitAction(false);
    }

    @Override
    public void OnSubmitActionErr(String err) {
        hideProgressDialog();
        Utility.share.showAlertWithOnlyOK(err, Functions.share.getTitle("TEXT_CLOSE", "Close"), this);
    }

    @Override
    public void OnRefreshSuccess() {
        Intent intent = new Intent();
        intent.setAction(VarsReceiver.REFRESHAFTERSUBMITACTION);
        BroadcastUtility.send(this, intent);
        hideProgressDialog();
        BroadcastUtility.unregister(this, mReceiver);
        finish();
    }

    @Override
    public void OnRefreshErr() {
    }

    private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            switch (intent.getAction()) {
                case VarsReceiver.BOTTOM: {
                    String element = intent.getStringExtra("element");
                    handleBottomClicks(element);
                    break;
                }
                case VarsReceiver.INBOTTOMMORE: {
                    ButtonAction buttonAction = new Gson().fromJson(intent.getStringExtra("action"), ButtonAction.class);
                    bottomActionClick(buttonAction);
                    break;
                }
                case VarsReceiver.FORMCLICK: {
                    String element = intent.getStringExtra("element");
                    presenter.handleFormClicks(DetailWorkflowActivity.this, lnAll, element);
                    break;
                }
                case VarsReceiver.INNERACTIONCLICK: {
                    String element = intent.getStringExtra("element");
                    int actionId = intent.getIntExtra("actionId", 0);
                    int positionToAction = intent.getIntExtra("positionToAction", 0);
                    int flagViewID = intent.getIntExtra("flagViewID", 0);
                    handleInnerClicks(element, actionId, positionToAction, flagViewID);
                    break;
                }
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
                            presenter.addLikeComment(comment, comment.getID(), comment.isLiked() != 1, 32);
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
                            attachComments.removeIf(r -> r.getPath().equals(file.getPath()));
                            //attachComments = (ArrayList<AttachFile>) attachComments.stream().filter(r -> !r.getPath().equals(file.getPath())).collect(Collectors.toList());
                            dynamicControlAdapter.updateListAttachment(attachComments);
                            break;
                        }
                    }
                    break;
                }
                case VarsReceiver.CHILDACTION: {
                    try {
                        ViewElement elementParent = new Gson().fromJson(intent.getStringExtra("elementParent"), ViewElement.class);
                        ViewElement elementChild = new Gson().fromJson(intent.getStringExtra("elementChild"), ViewElement.class);
                        JSONObject jsonOb = new JSONObject(intent.getStringExtra("json"));
                        int flagView = intent.getIntExtra("flagview", 0);
                        handleChildActionClicks(elementParent, elementChild, jsonOb, flagView);
                    } catch (Exception ex) {
                        Log.d("ERR mReceiverLocation", ex.getMessage());
                        break;
                    }
                    break;
                }
                case VarsReceiver.REFRESHCOMMENT: {
                    //like
                    if (intent.getStringExtra("type").equals("like")) {
                        dynamicControlAdapter.notifyDataSetChanged();
                    } else {
                        //Update Comment new
                        presenter.getDetailOtherResource(otherResourceId, workflowItem, commentChanged, "8");
                    }
                    break;
                }
                case VarsReceiver.UPDATEFORM: {
                    ViewElement element = new Gson().fromJson(intent.getStringExtra("element"), ViewElement.class);
                    String newValue = intent.getStringExtra("newValue");
                    updateValueForElement(element, newValue, true);
                    break;
                }
                case VarsReceiver.SUBMITACTION: {
                    presenter.submitAction(intent);
                    break;
                }
                case VarsReceiver.UPDATECHILDACTION: {
                    try {
                        ViewElement elementParent = new Gson().fromJson(intent.getStringExtra("elementParent"), ViewElement.class);
                        ViewElement elementChild = new Gson().fromJson(intent.getStringExtra("elementChild"), ViewElement.class);
                        JSONObject json = new JSONObject(intent.getStringExtra("json"));
                        String newValue = intent.getStringExtra("newValue");
                        updateValueForPopupGridDetail(elementParent, elementChild, json, newValue);
                    } catch (Exception ex) {
                        Log.d("ERR mReceiverLocation", ex.getMessage());
                        break;
                    }
                    break;
                }
                case VarsReceiver.FLOW_RELATE_CLICK: {
                    BroadcastUtility.unregister(DetailWorkflowActivity.this, mReceiver);
                    presenter.gotoSelf(DetailWorkflowActivity.this, intent, workflowItem);
                    break;
                }
                case VarsReceiver.TASK_CLICK: {
                    presenter.gotoTask(DetailWorkflowActivity.this, launcher, intent);
                    break;
                }
                case VarsReceiver.SELECT_FILE_INAPP: {
                    selectFileInApp(intent);
                    break;
                }
                case VarsReceiver.CREATE_TASK: {
                    getFormDefineInfo();
                    break;
                }
            }
        }
    };

    // Update detail khi thực hiện action bên Task
    private final ActivityResultLauncher<Intent> launcher = registerForActivityResult(
            new ActivityResultContracts.StartActivityForResult(),
            result -> {
                if (result.getResultCode() == VarsReceiver.TASK_RESULT) {
                    getFormDefineInfo();
                }
            });

    @Override
    public void OnUpdateValueForElement(ViewElement element, boolean isNotifyDataSetChange) {
        updateValueForElement(element, element.getValue(), isNotifyDataSetChange);
    }

    @Override
    public void OnSendAPI(ButtonAction action, String comment, HashMap<String, String> extension) {
        actionSendAPI(action, comment, extension);
    }

    @Override
    public void OnGetWorkflowHistorySuccess(ArrayList<WorkflowHistory> histories) {
        workflowHistories = new ArrayList<>(histories);
    }

    @Override
    public void OnGetWorkflowItemSuccess(WorkflowItem workflowItem) {
        this.workflowItem = workflowItem;

        setView();
        getFormDefineInfo();
        getWorkflowHistory();
    }

    @Override
    public void OnGetWorkflowItemErr(String err) {
        Utility.share.showAlertWithOnlyOK(err, Functions.share.getTitle("TEXT_CLOSE", "Đóng"), DetailWorkflowActivity.this);
    }
}
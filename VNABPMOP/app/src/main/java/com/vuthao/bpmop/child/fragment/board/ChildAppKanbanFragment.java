package com.vuthao.bpmop.child.fragment.board;

import android.app.Dialog;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.graphics.Color;
import android.graphics.Typeface;
import android.graphics.drawable.ColorDrawable;
import android.os.Bundle;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.core.content.res.ResourcesCompat;

import android.text.Editable;
import android.text.TextWatcher;
import android.util.DisplayMetrics;
import android.view.Gravity;
import android.view.KeyEvent;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.view.Window;
import android.view.WindowManager;
import android.view.inputmethod.EditorInfo;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;
import android.widget.TextView;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.AnimationController;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.DownloadFile;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.NetworkUtil;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.activity.BaseFragment;
import com.vuthao.bpmop.base.api.ApiBPM;
import com.vuthao.bpmop.base.custom.MyCustomBoardView;
import com.vuthao.bpmop.base.custom.boardview.BoardView;
import com.vuthao.bpmop.base.keyboard.KeyboardManager;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.Workflow;
import com.vuthao.bpmop.base.model.app.WorkflowItem;
import com.vuthao.bpmop.base.model.app.WorkflowStepDefine;
import com.vuthao.bpmop.base.model.custom.AttachFile;
import com.vuthao.bpmop.base.model.custom.BoardKanBan;
import com.vuthao.bpmop.base.model.custom.ButtonAction;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.child.fragment.board.adapter.BoardKanbanAdapter;
import com.vuthao.bpmop.child.fragment.board.presenter.ChildAppKanbanPresenter;
import com.vuthao.bpmop.detail.DetailWorkflowActivity;
import com.vuthao.bpmop.detail.custom.DetailFunc;
import com.vuthao.bpmop.shareview.SharedView_PopupFilterBoard;

import java.io.File;
import java.util.ArrayList;
import java.util.Objects;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;

public class ChildAppKanbanFragment extends BaseFragment implements ChildAppKanbanPresenter.ChildAppKanbanListener, ApiBPM.ApiBPMRefreshListener, BoardView.ItemClickListener, BoardView.DragItemStartCallback, SharedView_PopupFilterBoard.SharedView_PopupFilterBoardListener, TextWatcher, TextView.OnEditorActionListener {
    @BindView(R.id.rela_ViewChildAppKanBan_Toolbar)
    RelativeLayout relaToolBar;
    @BindView(R.id.tv_ViewChildAppKanBan_Name)
    TextView tvTitle;
    @BindView(R.id.img_ViewChildAppKanBan_Back)
    ImageView imgBack;
    @BindView(R.id.img_ViewChildAppKanBan_Filter)
    ImageView imgFilter;
    @BindView(R.id.img_ViewChildAppKanBan_ShowSearch)
    ImageView imgShowSearch;
    @BindView(R.id.ln_ViewChildAppKanBan_SubTitle)
    LinearLayout lnSubtitle;
    @BindView(R.id.tv_ViewChildAppKanBan_SubTitle)
    TextView tvSubtitle;
    @BindView(R.id.img_ViewChildAppKanBan_SubTitle_Previous)
    ImageView imgSubtitlePrevious;
    @BindView(R.id.img_ViewChildAppKanBan_SubTitle_Next)
    ImageView imgSubtitleNext;
    @BindView(R.id.ln_ViewChildAppKanBan_Search)
    LinearLayout lnSearch;
    @BindView(R.id.edt_ViewChildAppKanBan_Search)
    EditText edtSearch;
    @BindView(R.id.img_ViewChildAppKanBan_Search_Delete)
    ImageView imgDelete;
    @BindView(R.id.boardView_ViewChildAppKanBan)
    MyCustomBoardView boardView;
    @BindView(R.id.tv_ViewChildAppKanBan_NoData)
    TextView tvNoData;

    private View rootView;
    private Workflow workflow;
    private ChildAppKanbanPresenter presenter;
    private ArrayList<WorkflowStepDefine> lstStepDefine;
    private ArrayList<BoardKanBan> lstBoardKanBan_Full;
    private ArrayList<BoardKanBan> lstBoardKanBanSearch;
    private ArrayList<BoardKanBan> lstBoardKanBanFilter;
    private AnimationController animationController;
    private BoardKanbanAdapter adapter;
    private SharedView_PopupFilterBoard filterBoard;
    private ApiBPM apiBPM;
    private boolean isFilter;
    private final int _ApprovedStepID = -1;
    private final int _RejectedStepID = -2;

    public ChildAppKanbanFragment() {
        // Required empty public constructor
    }

    public ChildAppKanbanFragment(Workflow workflow) {
        this.workflow = workflow;
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        if (rootView == null) {
            rootView = inflater.inflate(R.layout.fragment_child_app_kanban, container, false);
            ButterKnife.bind(this, rootView);

            init();
            setTitle();
            setDataStepDefine();
            setData();
            setKanban(lstBoardKanBan_Full);

            edtSearch.addTextChangedListener(this);
            edtSearch.setOnEditorActionListener(this);
            boardView.setOnItemClickListener(this);
            boardView.setOnDragItemListener(this);
        }
        return rootView;
    }

    private void init() {
        apiBPM = new ApiBPM(this);
        lstBoardKanBanFilter = new ArrayList<>();
        lstBoardKanBanSearch = new ArrayList<>();
        lstStepDefine = new ArrayList<>();
        lstBoardKanBan_Full = new ArrayList<>();
        presenter = new ChildAppKanbanPresenter(this);
        animationController = new AnimationController();
        filterBoard = new SharedView_PopupFilterBoard(getLayoutInflater(), requireActivity(), "ChildAppKanban", rootView);
        filterBoard.init(this, relaToolBar);

        imgDelete.setVisibility(View.INVISIBLE);
        edtSearch.setImeOptions(EditorInfo.IME_ACTION_DONE);
        BroadcastUtility.register(requireActivity(), mReceiver, VarsReceiver.FOLLOW);
        BroadcastUtility.register(requireActivity(), mReceiver, VarsReceiver.REFRESHCOMMENT);
        BroadcastUtility.register(requireActivity(), mReceiver, VarsReceiver.REFRESHAFTERSUBMITACTION);
    }

    private void setTitle() {
        if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
            tvTitle.setText(workflow.getTitle());

        } else {
            tvTitle.setText(workflow.getTitleEN());
        }

        edtSearch.setHint(Functions.share.getTitle("TEXT_SEARCH", "Tìm kiếm"));
        tvNoData.setText(Functions.share.getTitle("TEXT_NODATA", "Không có dữ liệu"));
    }

    private void setDataStepDefine() {
        lstStepDefine = presenter.getListStepDefine(workflow.getWorkflowID(), _ApprovedStepID, _RejectedStepID);
    }

    private void setData() {
        lstBoardKanBan_Full = presenter.setListBoardKanBan(workflow, lstStepDefine, _ApprovedStepID, _RejectedStepID);
    }

    private void setKanban(ArrayList<BoardKanBan> boardKanBans) {
        if (boardKanBans.isEmpty()) {
            boardView.setVisibility(View.GONE);
            tvNoData.setVisibility(View.VISIBLE);
        } else {
            adapter = new BoardKanbanAdapter(requireContext(), requireActivity(), boardKanBans);
            boardView.setAdapter(adapter);
        }
    }

    private void search() {
        imgShowSearch.startAnimation(animationController.fadeIn(requireContext()));
        if (lnSearch.getVisibility() == View.GONE) {
            lnSearch.setVisibility(View.VISIBLE);
            imgShowSearch.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clVer2BlueMain));
            edtSearch.requestFocus();
            KeyboardManager.showKeyBoard(edtSearch, requireActivity());
        } else {
            KeyboardManager.hideKeyboard(edtSearch, requireActivity());
            lnSearch.setVisibility(View.GONE);
            imgShowSearch.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clBottomDisable));
            edtSearch.setText("");
        }
    }

    private void filter() {
        imgFilter.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clGreenDueDate));
        filterBoard.initializeView();
    }

    @OnClick({R.id.img_ViewChildAppKanBan_Back, R.id.img_ViewChildAppKanBan_ShowSearch,
            R.id.img_ViewChildAppKanBan_Search_Delete, R.id.img_ViewChildAppKanBan_Filter
            , R.id.img_ViewChildAppKanBan_Refresh})
    public void OnViewClicked(View v) {
        switch (v.getId()) {
            case R.id.img_ViewChildAppKanBan_Back: {
                Objects.requireNonNull(requireActivity()).finish();
                break;
            }
            case R.id.img_ViewChildAppKanBan_ShowSearch: {
                search();
                break;
            }
            case R.id.img_ViewChildAppKanBan_Search_Delete: {
                edtSearch.setText("");
                setKanban(lstBoardKanBan_Full);
                break;
            }
            case R.id.img_ViewChildAppKanBan_Filter: {
                imgFilter.startAnimation(animationController.fadeIn(requireContext()));
                filter();
                break;
            }
            case R.id.img_ViewChildAppKanBan_Refresh: {
                sBaseActivity.showProgressDialog();
                apiBPM.updateAllMasterData(false);
                break;
            }
        }
    }

    @Override
    public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {

    }

    @Override
    public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {

    }

    @Override
    public void afterTextChanged(Editable editable) {
        if (editable.length() > 0) {
            edtSearch.setTypeface(ResourcesCompat.getFont(requireActivity(), R.font.fontarial), Typeface.NORMAL);
            imgDelete.setVisibility(View.VISIBLE);
        } else {
            edtSearch.setTypeface(ResourcesCompat.getFont(requireActivity(), R.font.fontarial), Typeface.ITALIC);
            imgDelete.setVisibility(View.GONE);
        }
    }

    @Override
    public boolean onEditorAction(TextView textView, int i, KeyEvent keyEvent) {
        if (i == EditorInfo.IME_ACTION_DONE) {
            edtSearch.setSelection(edtSearch.getText().toString().length());
            lstBoardKanBanSearch = presenter.search(lstBoardKanBan_Full, edtSearch.getText().toString());
            setKanban(lstBoardKanBanSearch);
        }
        return true;
    }

    @Override
    public void OnFilterSuccess(String fromday, String today, Integer[] status) {
        isFilter = true;
        lstBoardKanBanFilter = presenter.filter(lstBoardKanBan_Full, fromday, today, status);
        setKanban(lstBoardKanBanFilter);
    }

    @Override
    public void OnDefaultFilter() {
        isFilter = false;
        imgFilter.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clBottomDisable));
        setKanban(lstBoardKanBan_Full);
    }

    @Override
    public void OnFilterDismiss() {
        if (isFilter) {
            imgFilter.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clGreenDueDate));
        } else {
            imgFilter.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clBottomDisable));
        }
    }

    @Override
    public void OnHeaderDay(String fromday, String today, Integer[] status) {
        isFilter = true;
        lstBoardKanBanFilter = presenter.filter(lstBoardKanBan_Full, fromday, today, status);
        setKanban(lstBoardKanBanFilter);
    }

    @Override
    public void onClick(View v, int column_pos, int item_pos) {
        AppBase item = adapter.getItem(column_pos, item_pos);
        // Task không cho click
        if (item.getResourceCategoryId() != 16) {
            Intent intent = new Intent(requireActivity(), DetailWorkflowActivity.class);
            intent.putExtra("WorkflowItemId", Functions.share.getWorkflowItemIDByUrl(item.getItemUrl()));
            requireActivity().startActivity(intent);
        }
    }

    @Override
    public void OnRefreshSuccess() {
        setDataStepDefine();
        setData();
        setKanban(lstBoardKanBan_Full);

        if (edtSearch.getText().length() > 0) {
            edtSearch.setSelection(edtSearch.getText().toString().length());
            lstBoardKanBanSearch = presenter.search(lstBoardKanBan_Full, edtSearch.getText().toString());
            setKanban(lstBoardKanBanSearch);
        }

        sBaseActivity.hideProgressDialog();
    }

    @Override
    public void OnRefreshErr() {
        sBaseActivity.hideProgressDialog();
    }

    @Override
    public void startDrag(View itemView, int originalPosition, int originalColumn) {

    }

    @Override
    public void changedPosition(View itemView, int originalPosition, int originalColumn, int newPosition, int newColumn) {

    }

    @Override
    public void dragging(View itemView, MotionEvent event) {

    }

    @Override
    public void endDrag(View itemView, int originalPosition, int originalColumn, int newPosition, int newColumn) {
        if (NetworkUtil.getConnectivityStatus(requireContext()) == NetworkUtil.NETWORK_STATUS_NOT_CONNECTED) {
            Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("TEXT_OFFLINE", "Bạn đang ở chế độ offline"),
                    Functions.share.getTitle("TEXT_CLOSE", "Close"), requireActivity());
            return;
        }

        presenter.endDrag(adapter, originalPosition, originalColumn, newPosition, newColumn);
    }

    private void accept(WorkflowItem workflowItem, ButtonAction buttonAction) {
        View v = getLayoutInflater().inflate(R.layout.popup_action_accept, null);
        TextView tvTitle = v.findViewById(R.id.tv_PopupAction_Accept_Title);
        ImageView imgAction = v.findViewById(R.id.img_PopupAction_Accept);
        EditText edtComment = v.findViewById(R.id.edt_PopupAction_Accept_YKien);
        TextView tvCancel = v.findViewById(R.id.tv_PopupAction_Accept_Huy);
        TextView tvAccept = v.findViewById(R.id.tv_PopupAction_Accept_HoanTat);

        tvTitle.setText(buttonAction.getTitle());
        edtComment.setHint(Functions.share.getTitle("MESS_REQUIRE_COMMENT", "Vui lòng nhập ý kiến"));
        tvCancel.setText(Functions.share.getTitle("TEXT_EXIT", "Thoát"));
        tvAccept.setText(buttonAction.getTitle());

        String imageName = "icon_bpm_Btn_action_" + buttonAction.getID();
        int resId = requireActivity().getResources().getIdentifier(imageName.toLowerCase(), "drawable", requireActivity().getPackageName());
        imgAction.setImageResource(resId);

        // region Dialog
        Dialog dialog = new Dialog(requireActivity());
        Window window = dialog.getWindow();
        dialog.requestWindowFeature(1);
        dialog.setCancelable(true);
        dialog.setCanceledOnTouchOutside(false);
        window.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_ADJUST_PAN);
        window.setGravity(Gravity.CENTER);

        DisplayMetrics displayMetrics = requireActivity().getResources().getDisplayMetrics();
        dialog.setContentView(v);
        dialog.show();

        WindowManager.LayoutParams s = window.getAttributes();
        s.width = displayMetrics.widthPixels;
        window.setAttributes(s);
        window.setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));
        // endregion

        tvAccept.setOnClickListener(view -> {
            if (edtComment.length() == 0) {
                Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("MESS_REQUIRE_COMMENT", "Vui lòng nhập ý kiến"),
                        Functions.share.getTitle("TEXT_CLOSE", "Close"), requireActivity());
                return;
            } else {
                sBaseActivity.showProgressDialog();
                KeyboardManager.hideKeyboard(edtComment, requireActivity());
                dialog.dismiss();
                presenter.submit(workflowItem, buttonAction, edtComment.getText().toString());
            }
        });

        tvCancel.setOnClickListener(view -> {
            KeyboardManager.hideKeyboard(edtComment, requireActivity());
            setKanban(lstBoardKanBan_Full);
            dialog.dismiss();
        });

        edtComment.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {

            }

            @Override
            public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {

            }

            @Override
            public void afterTextChanged(Editable editable) {
                if (editable.length() == 0) {
                    edtComment.setTypeface(ResourcesCompat.getFont(requireActivity(), R.font.fontarial), Typeface.ITALIC);
                } else {
                    edtComment.setTypeface(ResourcesCompat.getFont(requireActivity(), R.font.fontarial), Typeface.NORMAL);
                }
            }
        });
    }

    @Override
    public void OnSubmitSuccess() {
        apiBPM.updateAllMasterData(false);
    }

    @Override
    public void OnBackAction(WorkflowItem workflowItem, ButtonAction buttonAction) {
        sBaseActivity.hideProgressDialog();
        accept(workflowItem, buttonAction);
    }

    @Override
    public void OnSubmitErr(String err) {
        sBaseActivity.hideProgressDialog();
        Utility.share.showAlertWithOnlyOK(err, Functions.share.getTitle("TEXT_CLOSE", "Close"), requireActivity(), new Utility.OkListener() {
            @Override
            public void onOkListener() {
                setKanban(adapter.getListData());
            }
        });
    }

    @Override
    public void OnEndDragSuccess(AppBase app, int originalPosition, boolean isNext) {
        sBaseActivity.showProgressDialog();
        presenter.handleBoardAction(originalPosition, app, isNext);
    }

    @Override
    public void OnEndDragErr(String err, int originalColumn) {
        if (err.isEmpty()) {
            boardView.scrollToColumn(originalColumn, true);
            setKanban(adapter.getListData());
        } else {
            sBaseActivity.hideProgressDialog();
            Utility.share.showAlertWithOnlyOK(err, Functions.share.getTitle("TEXT_CLOSE", "Close"), requireActivity(), new Utility.OkListener() {
                @Override
                public void onOkListener() {
                    boardView.scrollToColumn(originalColumn, true);
                    setKanban(adapter.getListData());
                }
            });
        }
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        BroadcastUtility.unregister(requireActivity(), mReceiver);
    }

    private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            switch (intent.getAction()) {
                case VarsReceiver.FOLLOW: {
                    String workflowId = intent.getStringExtra("WorkflowId");
                    boolean isFollow = intent.getBooleanExtra("isFollow", false);
                    ArrayList<BoardKanBan> newList = adapter.setFollow(workflowId, isFollow);
                    setKanban(newList);
                    break;
                }
                case VarsReceiver.REFRESHCOMMENT:
                case VarsReceiver.REFRESHAFTERSUBMITACTION: {
                    apiBPM.updateAllMasterData(false);
                    break;
                }
            }
        }
    };
}
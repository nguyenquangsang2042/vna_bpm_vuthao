package com.vuthao.bpmop.detail.fragment;

import android.app.Dialog;
import android.os.Bundle;

import android.text.Editable;
import android.text.TextWatcher;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.Window;
import android.view.WindowManager;
import android.widget.EditText;
import android.widget.ExpandableListView;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;
import androidx.recyclerview.widget.StaggeredGridLayoutManager;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.AnimationController;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.activity.BaseFragment;
import com.vuthao.bpmop.base.custom.CustomFlexBoxRecyclerView;
import com.vuthao.bpmop.base.custom.flexbox.FlexDirection;
import com.vuthao.bpmop.base.custom.flexbox.FlexboxLayoutManager;
import com.vuthao.bpmop.base.custom.flexbox.JustifyContent;
import com.vuthao.bpmop.base.keyboard.KeyboardManager;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.model.app.WorkflowItem;
import com.vuthao.bpmop.base.model.custom.GroupShareHistory;
import com.vuthao.bpmop.base.model.custom.ShareHistory;
import com.vuthao.bpmop.base.model.custom.UserAndGroup;
import com.vuthao.bpmop.base.realm.WorkflowController;
import com.vuthao.bpmop.detail.adapter.ExpandActionShareHistoryAdapter;
import com.vuthao.bpmop.detail.adapter.SelectUserGroupMultipleAdapter;
import com.vuthao.bpmop.detail.adapter.SelectUserGroupMultipleTextAdapter;
import com.vuthao.bpmop.detail.presenter.DetailSharePresenter;

import org.json.JSONObject;

import java.util.ArrayList;
import java.util.stream.Collectors;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;

public class ShareFragment extends BaseFragment implements SelectUserGroupMultipleTextAdapter.SelectUserGroupMultipleTextListener, DetailSharePresenter.DetailShareListener {
    @BindView(R.id.ln_ViewDetailShare_All)
    LinearLayout lnAll;
    @BindView(R.id.img_ViewDetailShare_Back)
    ImageView imgBack;
    @BindView(R.id.img_ViewDetailShare_Done)
    ImageView imgDone;
    @BindView(R.id.recy_ViewDetailShare_User)
    CustomFlexBoxRecyclerView recyUser;
    @BindView(R.id.tv_ViewDetailShare_Name)
    TextView tvName;
    @BindView(R.id.tv_ViewDetailShare_User)
    TextView tvUser;
    @BindView(R.id.tv_ViewDetailShare_History)
    TextView tvShareHistory;
    @BindView(R.id.ln_ViewDetailShare_User)
    LinearLayout lnChooseUser;
    @BindView(R.id.tv_ViewDetailShare_Comment)
    TextView tvComment;
    @BindView(R.id.img_ViewDetailShare_User)
    ImageView imgUser;
    @BindView(R.id.edt_ViewDetailShare_Comment)
    EditText edtComment;
    @BindView(R.id.expand_ViewDetailShare_History)
    ExpandableListView expandHistory;
    @BindView(R.id.ln_ViewDetailShare_History)
    LinearLayout lnShare;

    private View rootView;
    private DetailSharePresenter presenter;
    private SelectUserGroupMultipleTextAdapter adapterListUserSelected;
    private SelectUserGroupMultipleTextAdapter adapterListUserText;
    private SelectUserGroupMultipleAdapter adapterListUser;
    private ArrayList<UserAndGroup> lstUserAndGroupAll;
    private ArrayList<UserAndGroup> lstCurrentUserGroup;
    private ArrayList<UserAndGroup> lstResult;
    private ArrayList<UserAndGroup> lstSelected;
    private WorkflowItem workflowItem;
    private LayoutInflater inflater;
    private Dialog dialog;
    private WorkflowController controller;
    private AnimationController animationController;

    public ShareFragment() {
        // Required empty public constructor
    }

    public ShareFragment(WorkflowItem workflowItem) {
        this.workflowItem = workflowItem;
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        this.inflater = inflater;
        if (rootView == null) {
            rootView = inflater.inflate(R.layout.fragment_detail_share, container, false);
            ButterKnife.bind(this, rootView);

            init();
            setTitle();
            getData();
            setData();
            expandHistory.setOnGroupClickListener(null);
        }

        return rootView;
    }

    private void init() {
        animationController = new AnimationController();
        controller = new WorkflowController();
        presenter = new DetailSharePresenter(getContext(), this);
        lstResult = new ArrayList<>();
        lstUserAndGroupAll = new ArrayList<>();
        lstCurrentUserGroup = new ArrayList<>();
        lstSelected = new ArrayList<>();

        recyUser.setMaxRowAndRowHeight(Functions.share.convertDpToPixel(35, rootView.getContext()), 3);

        expandHistory.setGroupIndicator(null);
        expandHistory.setChildIndicator(null);
        expandHistory.setDividerHeight(0);

        FlexboxLayoutManager flexboxLayoutManager = new FlexboxLayoutManager(requireActivity());
        flexboxLayoutManager.setFlexDirection(FlexDirection.ROW);
        flexboxLayoutManager.setJustifyContent(JustifyContent.FLEX_START);
        recyUser.setLayoutManager(flexboxLayoutManager);

    }

    private void setTitle() {
        tvName.setText(Functions.share.getTitle("TEXT_SHARE", "Chia sẻ"));
        tvUser.setText(Functions.share.getTitle("TEXT_SHARE_USERORGROUP", "Chọn người hoặc group muốn chia sẻ"));
        tvComment.setText(Functions.share.getTitle("TEXT_SHARE_IDEA", "Ý kiến"));
        tvShareHistory.setText(Functions.share.getTitle("TEXT_SHARE_HISTORY", "Lịch sử chia sẻ"));
    }

    private void getData() {
        presenter.getListShareHistory(workflowItem.getID());
    }

    @OnClick({R.id.img_ViewDetailShare_Back, R.id.img_ViewDetailShare_Done, R.id.ln_ViewDetailShare_All, R.id.img_ViewDetailShare_User, R.id.ln_ViewDetailShare_User})
    public void OnViewClicked(View v) {
        switch (v.getId()) {
            case R.id.img_ViewDetailShare_Back: {
                imgBack.startAnimation(animationController.fadeIn(requireContext()));
                KeyboardManager.hideKeyboard(requireActivity());
                sBaseActivity.backFragment("");
                break;
            }
            case R.id.ln_ViewDetailShare_All: {
                break;
            }
            case R.id.ln_ViewDetailShare_User:
            case R.id.img_ViewDetailShare_User: {
                imgUser.startAnimation(animationController.fadeIn(requireContext()));
                selectUser();
                break;
            }
            case R.id.img_ViewDetailShare_Done: {
                imgDone.startAnimation(animationController.fadeIn(requireContext()));
                sBaseActivity.showProgressDialog();
                presenter.share(workflowItem, lstCurrentUserGroup, edtComment.getText().toString());
                break;
            }
        }
    }

    private void setData() {
        if (lstCurrentUserGroup.size() > 0) {
            adapterListUserText = new SelectUserGroupMultipleTextAdapter(requireActivity(), pos -> {
                lstCurrentUserGroup = (ArrayList<UserAndGroup>) lstCurrentUserGroup.stream().filter(r -> !r.getID().equals(lstCurrentUserGroup.get(pos).getID())).collect(Collectors.toList());
                adapterListUserText.updateItemListIsClicked(lstCurrentUserGroup);
                adapterListUserText.notifyDataSetChanged();
            }, lstCurrentUserGroup, true);
            recyUser.setAdapter(adapterListUserText);
        } else {
            lstCurrentUserGroup = new ArrayList<>();
            adapterListUserText = new SelectUserGroupMultipleTextAdapter(requireActivity(), pos -> {
            }, lstCurrentUserGroup, true);
            recyUser.setAdapter(adapterListUserText);
        }
    }

    private void selectUser() {
        View view = inflater.inflate(R.layout.popup_control_choose_user, null);
        ImageView imgCloseChooseUser = view.findViewById(R.id.img_PopupControl_ChooseUser_Close);
        ImageView imgAcceptChooseUser = view.findViewById(R.id.img_PopupControl_ChooseUser_Accept);
        ImageView imgDeleteChooseUser = view.findViewById(R.id.img_PopupControl_ChooseUser_Delete);
        TextView tvTitleChooseUser = view.findViewById(R.id.tv_PopupControl_ChooseUser_Title);
        EditText edtSearchChooseUser = view.findViewById(R.id.edt_PopupControl_ChooseUser_Search);
        RecyclerView recyChooseUser = view.findViewById(R.id.recy_PopupControl_ChooseUser);
        CustomFlexBoxRecyclerView recySelectedUser = view.findViewById(R.id.recy_PopupControl_SelectedUser);

        recySelectedUser.setVisibility(View.VISIBLE);
        imgDeleteChooseUser.setVisibility(View.VISIBLE);
        recySelectedUser.setMaxRowAndRowHeight(Functions.share.convertDpToPixel(35, rootView.getContext()), 3);

        tvTitleChooseUser.setText(Functions.share.getTitle("TEXT_TITLE_USERGROUP", "Chọn người hoặc nhóm"));
        edtSearchChooseUser.setHint(Functions.share.getTitle("MESS_REQUIRE_USERGROUP", "Vui lòng chọn người hoặc nhóm để thực hiện"));

        lstSelected = new ArrayList<>(lstCurrentUserGroup);
        lstUserAndGroupAll = controller.getUserAndGroup();

        // Người đã được chọn sẽ không hiển thị vào list
        if (lstUserAndGroupAll.size() > 0 && lstSelected.size() > 0) {
            for (int i = 0; i < lstSelected.size(); i++) {
                int finalI = i;
                lstUserAndGroupAll = (ArrayList<UserAndGroup>) lstUserAndGroupAll.stream().filter(r -> !r.getID().equals(lstSelected.get(finalI).getID())).collect(Collectors.toList());
            }
        }

        adapterListUser = new SelectUserGroupMultipleAdapter(rootView.getContext(), lstUserAndGroupAll, userAndGroup -> {
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

        adapterListUserSelected = new SelectUserGroupMultipleTextAdapter(requireActivity(), pos -> {
            UserAndGroup _clickedItem = lstSelected.get(pos);
            lstSelected.remove(_clickedItem);
            lstUserAndGroupAll.add(_clickedItem);
            adapterListUser.updateCurrentList(lstUserAndGroupAll);
            adapterListUserSelected.updateItemListIsClicked(lstSelected);
            adapterListUserSelected.notifyDataSetChanged();
            edtSearchChooseUser.setText(edtSearchChooseUser.getText());
            edtSearchChooseUser.setSelection(edtSearchChooseUser.length());
        }, lstSelected, true);

        FlexboxLayoutManager flexboxLayoutManager = new FlexboxLayoutManager(rootView.getContext());
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
            KeyboardManager.hideKeyboard(edtSearchChooseUser, requireActivity());
            dialog.dismiss();
        });

        imgAcceptChooseUser.setOnClickListener(v -> {
            lstResult = adapterListUserSelected.getListSelected();
            lstCurrentUserGroup = lstResult;
            adapterListUserText.updateItemListIsClicked(lstCurrentUserGroup);
            adapterListUserText.notifyDataSetChanged();
            setData();
            KeyboardManager.hideKeyboard(edtSearchChooseUser, requireActivity());
            dialog.dismiss();
        });

        imgDeleteChooseUser.setOnClickListener(v -> {
            lstCurrentUserGroup = new ArrayList<>();
            adapterListUserText.updateItemListIsClicked(lstCurrentUserGroup);
            adapterListUserText.notifyDataSetChanged();
            setData();
            KeyboardManager.hideKeyboard(edtSearchChooseUser, requireActivity());
            dialog.dismiss();
        });

        // region dialog
        dialog = new Dialog(rootView.getContext(), R.style.Theme_Custom_BPMOP_Dialog_FullScreen);
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

        edtSearchChooseUser.setText("");
        edtSearchChooseUser.requestFocus();
        KeyboardManager.showKeyBoard(rootView, requireActivity());
    }

    @Override
    public void OnGetShareSuccess(ArrayList<ShareHistory> shareHistories) {
        if (shareHistories != null && shareHistories.size() > 0) {
            animationController.showView(lnShare);
            ArrayList<GroupShareHistory> groupShareHistories = presenter.cloneListGroupShareHistory(shareHistories);
            ExpandActionShareHistoryAdapter expandActionShareHistoryAdapter = new ExpandActionShareHistoryAdapter(getActivity(), getContext(), groupShareHistories);
            expandHistory.setAdapter(expandActionShareHistoryAdapter);

            for (int i = 0; i < expandActionShareHistoryAdapter.getGroupCount(); i++) {
                expandHistory.expandGroup(i);
            }
        } else {
            lnShare.setVisibility(View.GONE);
        }
    }

    @Override
    public void OnShareSuccess() {
        sBaseActivity.hideProgressDialog();
        sBaseActivity.backFragment("");
    }

    @Override
    public void OnShareErr(String err) {
        sBaseActivity.hideProgressDialog();
        Utility.share.showAlertWithOnlyOK(err, Functions.share.getTitle("TEXT_CLOSE", "Close"), requireActivity());
    }

    @Override
    public void OnItemDelete(int pos) {

    }
}
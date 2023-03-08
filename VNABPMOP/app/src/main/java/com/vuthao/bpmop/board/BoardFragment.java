package com.vuthao.bpmop.board;

import android.app.Dialog;
import android.content.Intent;
import android.graphics.Color;
import android.graphics.drawable.ColorDrawable;
import android.os.Bundle;

import androidx.annotation.NonNull;
import androidx.cardview.widget.CardView;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.swiperefreshlayout.widget.SwipeRefreshLayout;

import android.text.Editable;
import android.text.TextWatcher;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.Window;
import android.view.WindowManager;
import android.view.animation.AnimationUtils;
import android.widget.EditText;
import android.widget.ExpandableListView;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;
import android.widget.TextView;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.AnimationController;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.ImageLoader;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.activity.BaseFragment;
import com.vuthao.bpmop.base.api.ApiBPM;
import com.vuthao.bpmop.base.custom.expandable.AnimatedExpandableListView;
import com.vuthao.bpmop.base.keyboard.KeyboardManager;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.Workflow;
import com.vuthao.bpmop.base.model.app.WorkflowCategory;
import com.vuthao.bpmop.base.model.custom.BoardWorkflow;
import com.vuthao.bpmop.base.realm.BoardController;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.board.adapter.BoardChooseCategoryAdapter;
import com.vuthao.bpmop.board.adapter.BoardMainGroupAdapter;
import com.vuthao.bpmop.board.adapter.ExpandBoardMainGroupAdapter;
import com.vuthao.bpmop.board.presenter.BoardPresenter;

import java.util.ArrayList;
import java.util.Comparator;
import java.util.stream.Collectors;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;
import de.hdodenhof.circleimageview.CircleImageView;

public class BoardFragment extends BaseFragment implements ExpandableListView.OnGroupClickListener, ApiBPM.ApiBPMRefreshListener, BoardMainGroupAdapter.RcvBoardMainGroupListener, ExpandBoardMainGroupAdapter.ExpandBoardMainGroupListener, TextWatcher, SwipeRefreshLayout.OnRefreshListener {
    @BindView(R.id.swipe_ViewBoard)
    SwipeRefreshLayout swipe;
    @BindView(R.id.tv_ViewBoard_Name)
    TextView tvTitle;
    @BindView(R.id.img_ViewBoard_Avatar)
    CircleImageView imgAvatar;
    @BindView(R.id.img_ViewBoard_Subcribe)
    ImageView imgFavorite;
    @BindView(R.id.img_ViewBoard_ShowSearch)
    ImageView imgShowSearch;
    @BindView(R.id.ln_ViewBoard_Search)
    LinearLayout cardSearch;
    @BindView(R.id.expand_ViewBoard_Data)
    AnimatedExpandableListView expandData;
    @BindView(R.id.recy_ViewBoard_Data)
    RecyclerView recyData;
    @BindView(R.id.rela_ViewBoard_Data)
    RelativeLayout relaDataExpand;
    @BindView(R.id.rela_ViewBoard_Data_Subcribe)
    RelativeLayout relaDataFavorite;
    @BindView(R.id.ln_ViewBoard_NoData)
    LinearLayout lnNoDataExpand;
    @BindView(R.id.ln_ViewBoard_NoData_Subcribe)
    LinearLayout lnNoDataFavorite;
    @BindView(R.id.tv_ViewBoard_NoData)
    TextView tvNoDataExpand;
    @BindView(R.id.tv_ViewBoard_NoData_Subcribe)
    TextView tvNoDataFavorite;
    @BindView(R.id.card_ViewBoard_GroupWorkflow)
    LinearLayout cardGroupWorkflow;
    @BindView(R.id.tv_ViewBoard_CurrentGroupWorkflow)
    TextView tvCurrentGroup;
    @BindView(R.id.edt_ViewBoard_Search)
    EditText edtSearch;
    @BindView(R.id.img_ViewBoard_Search_Delete)
    ImageView imgDelete;

    private View rootView;
    private BoardController controller;
    private ArrayList<WorkflowCategory> lstWFCategory; // List nhóm quy trình
    private ArrayList<BoardWorkflow> lstExpand; // List của Expandable ListView
    private ArrayList<Workflow> favorites;
    private ExpandBoardMainGroupAdapter expandBoardAdapter;
    private BoardMainGroupAdapter rcvBoardMainGroupAdapter;
    private AnimationController animationController;
    private WorkflowCategory currentExpandWFCategory = null;
    private LayoutInflater inflater;
    private BoardPresenter presenter;
    private Dialog dialog;
    private ApiBPM apiBPM;
    private LinearLayoutManager mLayoutManager;

    public BoardFragment() {
        // Required empty public constructor
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
            rootView = inflater.inflate(R.layout.fragment_board, container, false);
            ButterKnife.bind(this, rootView);

            init();
            setTitle();
            setData();

            swipe.setOnRefreshListener(this);
            edtSearch.addTextChangedListener(this);
            expandData.setOnGroupClickListener(this);
        }
        return rootView;
    }

    private void init() {
        apiBPM = new ApiBPM(this);
        animationController = new AnimationController();
        controller = new BoardController();
        favorites = new ArrayList<>();
        presenter = new BoardPresenter();
        lstWFCategory = new ArrayList<>();
        lstExpand = new ArrayList<>();

        ImageLoader.getInstance().loadImageUserWithToken(requireContext(), Constants.BASE_URL + CurrentUser.getInstance().getUser().getImagePath(), imgAvatar);
        mLayoutManager = new LinearLayoutManager(requireContext(), LinearLayoutManager.VERTICAL, false);
        recyData.setLayoutManager(mLayoutManager);
        recyData.setDrawingCacheEnabled(true);
        recyData.setHasFixedSize(false);
        recyData.setItemViewCacheSize(20);

        expandData.setGroupIndicator(null);
        expandData.setChildIndicator(null);
        expandData.setDividerHeight(0);

        Utility.share.setupSwipeRefreshLayout(swipe);
    }

    public void scrollToTop() {
        if (relaDataFavorite.getVisibility() == View.VISIBLE) {
            if (favorites.size() > 0) {
                if (mLayoutManager.findFirstCompletelyVisibleItemPosition() > 0) {
                    recyData.smoothScrollToPosition(0);
                }
            }
        } else {
            expandData.smoothScrollToPosition(0);
        }
    }

    private void setFavorites(ArrayList<Workflow> favorites) {
        if (favorites.size() > 0) {
            if (rcvBoardMainGroupAdapter == null) {
                rcvBoardMainGroupAdapter = new BoardMainGroupAdapter(requireActivity(), favorites, this);
                recyData.setAdapter(rcvBoardMainGroupAdapter);
            } else {
                rcvBoardMainGroupAdapter.setList(favorites, edtSearch.getText().toString());
            }

            recyData.startAnimation(AnimationController.share.fadeIn(requireActivity()));
            recyData.setVisibility(View.VISIBLE);
            lnNoDataFavorite.setVisibility(View.GONE);
        } else {
            recyData.setVisibility(View.GONE);
            lnNoDataFavorite.setVisibility(View.VISIBLE);
        }
    }

    private void setTitle() {
        tvTitle.setText(Functions.share.getTitle("TEXT_APPLICATION", "Ứng dụng"));
        tvNoDataExpand.setText(Functions.share.getTitle("TEXT_NODATA", "Không có dữ liệu"));
        tvNoDataFavorite.setText(Functions.share.getTitle("TEXT_NODATA", "Không có dữ liệu"));
        if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
            edtSearch.setHint("Tìm kiếm theo tên quy trình...");
        } else {
            edtSearch.setHint("Search by workflow name...");
        }
    }

    private void registerReceiver() {
        Intent intent = new Intent();
        intent.setAction(VarsReceiver.UPDATEAPP);
        BroadcastUtility.send(requireActivity(), intent);
    }

    private void setData() {
        ArrayList<WorkflowCategory> temps = new ArrayList<>();
        WorkflowCategory categoryAll = new WorkflowCategory();
        categoryAll.setID(0);
        categoryAll.setTitle(Functions.share.getTitle("TEXT_ALL", "Tất cả"));
        categoryAll.setSelected(true);

        temps.add(categoryAll);
        temps.addAll(controller.getWorkflowCategory());

        lstExpand.clear();
        lstWFCategory.clear();

        for (int i = 0; i < temps.size(); i++) {
            if (temps.get(i).getID() != 0) {
                ArrayList<Workflow> workflows = controller.getWorkflows(temps.get(i).getID());
                if (workflows.size() > 0) {
                    lstWFCategory.add(temps.get(i));
                    lstExpand.add(new BoardWorkflow(temps.get(i), workflows));
                }
            } else {
                lstWFCategory.add(temps.get(i));
            }
        }

        currentExpandWFCategory = categoryAll;
        tvCurrentGroup.setText(currentExpandWFCategory.getTitle());

        setListExpandBoard(lstExpand);

        favorites = controller.getFavorites();
        setFavorites(favorites);
    }

    private void setListExpandBoard(ArrayList<BoardWorkflow> lstBoardWorkflow) {
        if (lstBoardWorkflow.size() > 0) {
            expandData.setVisibility(View.VISIBLE);
            lnNoDataExpand.setVisibility(View.GONE);

            expandData.startAnimation(AnimationUtils.loadAnimation(requireContext(), R.anim.anim_fade_in));
            expandBoardAdapter = new ExpandBoardMainGroupAdapter(requireActivity(), lstBoardWorkflow, this);
            expandBoardAdapter.setWFCategory(currentExpandWFCategory);
            expandData.setAdapter(expandBoardAdapter);
            for (int i = 0; i < expandBoardAdapter.getGroupCount(); i++) {
                if (lstExpand.get(i).getWorkflows().size() > 0)
                    expandData.expandGroup(i);
            }

            if (!edtSearch.getText().toString().isEmpty()) {
                expandBoardAdapter.search(edtSearch.getText().toString());
            }
        } else {
            expandData.setVisibility(View.GONE);
            lnNoDataExpand.setVisibility(View.VISIBLE);
        }
    }

    @OnClick({R.id.img_ViewBoard_Avatar, R.id.img_ViewBoard_Subcribe, R.id.img_ViewBoard_ShowSearch,
            R.id.card_ViewBoard_GroupWorkflow, R.id.img_ViewBoard_Search_Delete,
            R.id.tv_ViewBoard_Name})
    public void OnViewClicked(View v) {
        switch (v.getId()) {
            case R.id.tv_ViewBoard_Name:
            case R.id.img_ViewBoard_Avatar: {
                sBaseActivity.openLeftMenu();
                break;
            }
            case R.id.img_ViewBoard_Subcribe: {
                favorite();
                break;
            }
            case R.id.img_ViewBoard_ShowSearch: {
                search();
                break;
            }
            case R.id.img_ViewBoard_Search_Delete: {
                edtSearch.setText("");
                break;
            }
            case R.id.card_ViewBoard_GroupWorkflow: {
                category();
                break;
            }
        }
    }

    private void category() {
        KeyboardManager.hideKeyboard(requireActivity());
        View view = inflater.inflate(R.layout.popup_control_single_choice, null);
        ImageView imgClose = view.findViewById(R.id.img_PopupControl_SingleChoice_Close);
        TextView tvTitle = view.findViewById(R.id.tv_PopupControl_SingleChoice_Title);
        RecyclerView recyData = view.findViewById(R.id.recy_PopupControl_SingleChoice_Data);
        ImageView imgDone = view.findViewById(R.id.img_PopupControl_SingleChoice_Done);

        imgDone.setVisibility(View.INVISIBLE);
        tvTitle.setText(CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN) ? "Nhóm quy trình" : "Group workflow");

        BoardChooseCategoryAdapter adapter = new BoardChooseCategoryAdapter(requireActivity(), lstWFCategory, pos -> {
            if (edtSearch.getText().toString().length() > 0) {
                edtSearch.setText("");
            }

            for (WorkflowCategory category : lstWFCategory) {
                category.setSelected(false);
            }

            lstWFCategory.get(pos).setSelected(true);

            currentExpandWFCategory = lstWFCategory.get(pos);
            expandBoardAdapter.setWFCategory(lstWFCategory.get(pos));
            tvCurrentGroup.setText(lstWFCategory.get(pos).getTitle());
            expandBoardAdapter.filter();
            dialog.dismiss();
        });

        recyData.setAdapter(adapter);
        recyData.setLayoutManager(new LinearLayoutManager(requireActivity()));

        imgClose.setOnClickListener(v -> dialog.dismiss());

        //region Dialog
        dialog = new Dialog(rootView.getContext(), R.style.Theme_Custom_BPMOP_Dialog_FullScreen);
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
        window.setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));
        //endregion
    }

    private void search() {
        imgShowSearch.startAnimation(animationController.fadeIn(requireContext()));
        if (cardSearch.getVisibility() == View.GONE) {
            cardSearch.setVisibility(View.VISIBLE);
            imgShowSearch.setColorFilter(ContextCompat.getColor(requireContext(), R.color.clVer2BlueMain));
            edtSearch.requestFocus();
            KeyboardManager.showKeyBoard(rootView, requireActivity());

        } else {
            cardSearch.setVisibility(View.GONE);
            imgShowSearch.setColorFilter(ContextCompat.getColor(requireContext(), R.color.clBottomDisable));
            KeyboardManager.hideKeyboard(getActivity());
        }
    }

    private void favorite() {
        imgFavorite.startAnimation(AnimationController.share.fadeIn(requireContext()));
        KeyboardManager.hideKeyboard(requireActivity());
        if (relaDataFavorite.getVisibility() == View.GONE) {
            imgFavorite.setImageResource(R.drawable.icon_ver2_favorite_check);
            imgFavorite.setColorFilter(ContextCompat.getColor(requireContext(), R.color.clVer2BlueMain));

            cardGroupWorkflow.setVisibility(View.GONE);
            relaDataExpand.setVisibility(View.GONE);

            animationController.showView(relaDataFavorite);

        } else {
            cardGroupWorkflow.setVisibility(View.VISIBLE);
            relaDataFavorite.setVisibility(View.GONE);

            imgFavorite.setColorFilter(null);
            imgFavorite.setImageResource(R.drawable.icon_ver2_favorite_uncheck);
            animationController.showView(relaDataExpand);
        }

        edtSearch.setText("");
    }

    @Override
    public void onRefresh() {
        apiBPM.updateAllMasterData(false);
    }

    @Override
    public void onResume() {
        super.onResume();
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
    }

    @Override
    protected void onBroadcastReceived(Intent intent) {
        super.onBroadcastReceived(intent);
        setTitle();

    }

    @Override
    public void beforeTextChanged(CharSequence s, int start, int count, int after) {

    }

    @Override
    public void onTextChanged(CharSequence s, int start, int before, int count) {

    }

    @Override
    public void afterTextChanged(Editable s) {
        imgDelete.setVisibility(s.toString().length() > 0 ? View.VISIBLE : View.GONE);
        if (relaDataFavorite.getVisibility() == View.VISIBLE) {
            if (rcvBoardMainGroupAdapter != null) {
                rcvBoardMainGroupAdapter.getFilter().filter(s.toString());
            }
        } else {
            if (expandBoardAdapter != null) {
                expandBoardAdapter.search(s.toString());
            }
        }
    }

    @Override
    public void OnFavoriteClick(Workflow workflow) {
        presenter.setFavorite(workflow.getWorkflowID(), workflow.isFavorite() != 1);
        if (lstExpand.size() > 0) {
            expandBoardAdapter.updateItem(workflow, workflow.isFavorite() != 1);
            expandBoardAdapter.notifyDataSetChanged();
        }

        if (workflow.isFavorite() == 1) {
            favorites.add(workflow);
            favorites.sort((workflow1, t1) -> workflow1.getTitle().compareTo(t1.getTitle()));

        } else {
            favorites = (ArrayList<Workflow>) favorites.stream().filter(r -> r.getWorkflowID() != workflow.getWorkflowID()).collect(Collectors.toList());
        }

        if (favorites.size() > 0) {
            recyData.setVisibility(View.VISIBLE);
            lnNoDataFavorite.setVisibility(View.GONE);

            if (rcvBoardMainGroupAdapter != null) {
                rcvBoardMainGroupAdapter.updateListFavorite(favorites);
            } else {
                setFavorites(favorites);
            }
        } else {
            recyData.setVisibility(View.GONE);
            lnNoDataFavorite.setVisibility(View.VISIBLE);
        }

        registerReceiver();
    }

    @Override
    public void OnItemClick(Workflow workflow) {
        presenter.handleClicks(requireActivity(), workflow);
    }

    @Override
    public void OnRefreshSuccess() {
        if (swipe.isRefreshing()) {
            swipe.setRefreshing(false);
        }

        setData();
        registerReceiver();
    }

    @Override
    public void OnRefreshErr() {
        if (swipe.isRefreshing()) {
            swipe.setRefreshing(false);
        }
    }

    @Override
    public void OnFavoriteItemClick(Workflow workflow) {
        OnFavoriteClick(workflow);
    }

    @Override
    public void OnItemBoardClick(Workflow workflow) {
        presenter.handleClicks(requireActivity(), workflow);
    }

    @Override
    public boolean onGroupClick(ExpandableListView expandableListView, View view, int groupPosition, long l) {
        expandBoardAdapter.setSearch(false);
        if (expandData.isGroupExpanded(groupPosition)) {
            expandData.collapseGroupWithAnimation(groupPosition);
        } else {
            expandData.expandGroupWithAnimation(groupPosition);
        }
        return true;
    }
}
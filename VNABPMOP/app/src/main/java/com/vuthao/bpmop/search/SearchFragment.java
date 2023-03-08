package com.vuthao.bpmop.search;

import android.annotation.SuppressLint;
import android.content.Intent;
import android.os.Bundle;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.swiperefreshlayout.widget.SwipeRefreshLayout;

import android.os.Handler;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;
import android.widget.TextView;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.AnimationController;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.ImageLoader;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.activity.BaseFragment;
import com.vuthao.bpmop.base.keyboard.KeyboardManager;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.custom.ObjectPropertySearch;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.detail.DetailWorkflowActivity;
import com.vuthao.bpmop.search.adapter.SearchAdapter;
import com.vuthao.bpmop.search.presenter.SearchPresenter;
import com.vuthao.bpmop.shareview.ShareView_PopupFilterSearch;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;
import de.hdodenhof.circleimageview.CircleImageView;

public class SearchFragment extends BaseFragment implements SearchPresenter.SearchListener, SearchAdapter.SearchOnClickListener, ShareView_PopupFilterSearch.FilterSearchListener, SwipeRefreshLayout.OnRefreshListener {
    @BindView(R.id.ln_ViewSearch_All)
    LinearLayout lnAll;
    @BindView(R.id.rela_ViewSearch_Toolbar)
    RelativeLayout relaToolbar;
    @BindView(R.id.img_ViewSearch_Avata)
    CircleImageView imgAvatar;
    @BindView(R.id.tv_ViewSearch_Name)
    TextView tvName;
    @BindView(R.id.ln_ViewSearch_Filter)
    LinearLayout lnFilter;
    @BindView(R.id.img_ViewSearch_Filter)
    ImageView imgFilter;
    @BindView(R.id.img_ListWorkflowView_Search_Delete)
    ImageView imgDeleteSearch;
    @BindView(R.id.edt_ListWorkflowView_Search)
    EditText edtSearch;
    @BindView(R.id.swipe_ViewSearch)
    SwipeRefreshLayout swipe;
    @BindView(R.id.ln_ViewSearch_NoData)
    LinearLayout lnNoData;
    @BindView(R.id.ln_ViewSearch_Content)
    LinearLayout lnContent;
    @BindView(R.id.tv_ViewSearch_NoData)
    TextView tvNoData;
    @BindView(R.id.recy_ViewSearch)
    RecyclerView recyData;
    @BindView(R.id.img_ViewSearch_ShowSearch)
    ImageView imgShowSearch;
    @BindView(R.id.ln_ViewSearch_Search)
    LinearLayout lnSearch;
    @BindView(R.id.img_ViewSearch_Search)
    ImageView imgSearch;

    private View rootView;
    private LinearLayoutManager mLayoutManager;
    private SearchPresenter presenter;
    private ArrayList<AppBase> filters;
    private SearchAdapter adapter;
    private ShareView_PopupFilterSearch filterSearch;
    private AnimationController animationController;
    private boolean isFilter;
    private ObjectPropertySearch objectPropertySearch;
    private boolean isLoading;

    public SearchFragment() {
        // Required empty public constructor
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        if (rootView == null) {
            rootView = inflater.inflate(R.layout.fragment_search, container, false);
            ButterKnife.bind(this, rootView);

            init();
            setTitle();
            setData();
            initScrollListener();

            swipe.setOnRefreshListener(this);
        }

        return rootView;
    }

    private void initScrollListener() {
        recyData.addOnScrollListener(new RecyclerView.OnScrollListener() {
            @Override
            public void onScrolled(@NonNull RecyclerView recyclerView, int dx, int dy) {
                super.onScrolled(recyclerView, dx, dy);
                if (isLoading) {
                    if (mLayoutManager.findLastCompletelyVisibleItemPosition() == filters.size() - 1) {
                        loadMore();
                        isLoading = false;
                    }
                }
            }
        });
    }

    private void loadMore() {
        filters.add(null);
        adapter.notifyItemInserted(filters.size() - 1);

        objectPropertySearch.setOffset(filters.size() - 1);
        Handler handler = new Handler();

        handler.postDelayed(() -> {
            presenter.getMoreFilters(objectPropertySearch);
        }, 1000);
    }

    private void init() {
        presenter = new SearchPresenter(this);
        filters = new ArrayList<>();
        animationController = new AnimationController();
        filterSearch = new ShareView_PopupFilterSearch(requireActivity(), this);

        Utility.share.setupSwipeRefreshLayout(swipe);
        ImageLoader.getInstance().loadImageUserWithToken(requireActivity(), Constants.BASE_URL + CurrentUser.getInstance().getUser().getImagePath(), imgAvatar);

        mLayoutManager = new LinearLayoutManager(requireActivity(), LinearLayoutManager.VERTICAL, false);
        recyData.setLayoutManager(mLayoutManager);
        recyData.setDrawingCacheEnabled(true);
        recyData.setHasFixedSize(true);
        recyData.setItemViewCacheSize(20);
    }

    @SuppressLint("SetTextI18n")
    public void setTitle() {
        tvName.setText(Functions.share.getTitle("TEXT_SEARCH2", "Tra cứu"));
        tvNoData.setText(Functions.share.getTitle("TEXT_NODATA", "Không có dữ liệu"));
    }

    private void setData() {
        presenter.getFilters(objectPropertySearch);
    }

    public void scrollToTop() {
        if (filters.size() > 0) {
            if (mLayoutManager.findFirstCompletelyVisibleItemPosition() > 0) {
                recyData.smoothScrollToPosition(0);
            }
        }
    }

    private void search() {
        if (lnSearch.getVisibility() == View.VISIBLE) {
            lnSearch.setVisibility(View.GONE);
            edtSearch.setText("");
            KeyboardManager.hideKeyboard(edtSearch, requireActivity());
        } else {
            lnSearch.setVisibility(View.VISIBLE);
            KeyboardManager.showKeyBoard(edtSearch, requireActivity());
        }
    }

    private void filter() {
        filterSearch.filter(relaToolbar, objectPropertySearch);
    }

    @OnClick({R.id.img_ViewSearch_Avata, R.id.img_ListWorkflowView_Search_Delete, R.id.img_ViewSearch_Search,
    R.id.img_ViewSearch_Filter})
    public void OnViewClicked(View v) {
        switch (v.getId()) {
            case R.id.img_ViewSearch_Avata: {
                sBaseActivity.openLeftMenu();
                break;
            }
            case R.id.img_ViewSearch_Search: {
                imgSearch.startAnimation(animationController.fadeIn(requireActivity()));
                search();
                break;
            }
            case R.id.img_ListWorkflowView_Search_Delete: {
                edtSearch.setText("");
                break;
            }
            case R.id.img_ViewSearch_Filter: {
                imgFilter.startAnimation(animationController.fadeIn(requireActivity()));
                imgFilter.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clGreenDueDate));
                filter();
                break;
            }
        }
    }

    @Override
    public void onRefresh() {
        objectPropertySearch.setOffset(0);
        objectPropertySearch.setTotal(-1);
        objectPropertySearch.setLimit(Constants.mFilterLimit - 40);
        presenter.getFilters(objectPropertySearch);
    }

    @Override
    public void OnFilterSuccess(ObjectPropertySearch objectPropertySearch) {
        isFilter = true;
        this.objectPropertySearch = objectPropertySearch;
        presenter.getFilters(objectPropertySearch);
    }

    @Override
    public void OnFilterErr(String err) {
        if (!err.isEmpty()) {
            Utility.share.showAlertWithOnlyOK(err, requireActivity());
        } else {
            lnNoData.setVisibility(View.VISIBLE);
            recyData.setVisibility(View.GONE);
        }
    }

    @Override
    public void OnFilterDismiss() {
        if (isFilter) {
            imgFilter.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clGreenDueDate));
        } else {
            imgFilter.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clBottomDisable));
            filterSearch.setDefaultValue();
        }
    }

    @Override
    public void OnDefaultFilter() {
        isFilter = false;
        imgFilter.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clBottomDisable));
        presenter.getFilters(null);
    }

    @Override
    public void OnGetDataSuccess(ArrayList<AppBase> appBases, ObjectPropertySearch objectPropertySearch) {
        if (swipe.isRefreshing()) {
            swipe.setRefreshing(false);
        }

        this.objectPropertySearch = objectPropertySearch;
        isLoading = appBases.size() >= Constants.mFilterLimit - 40;
        filters = appBases;

        if (filters.isEmpty()) {
            lnNoData.setVisibility(View.VISIBLE);
            recyData.setVisibility(View.GONE);
        } else {
            if (adapter == null) {
                adapter = new SearchAdapter(requireActivity(), filters, this);
                recyData.setAdapter(adapter);
            } else {
                adapter.setList(filters);
            }

            lnNoData.setVisibility(View.GONE);
            recyData.setVisibility(View.VISIBLE);
        }
    }

    @Override
    public void OnGetMoreDataSuccess(ArrayList<AppBase> appBases, ObjectPropertySearch objectPropertySearch) {
        this.objectPropertySearch = objectPropertySearch;
        isLoading = appBases.size() >= Constants.mFilterLimit - 40;
        this.filters.remove(this.filters.size() - 1);
        adapter.notifyItemRemoved(filters.size());

        this.filters.addAll(appBases);
        adapter.notifyDataSetChanged();
    }

    @Override
    public void OnGetDataErr() {
        isFilter = false;
        lnNoData.setVisibility(View.VISIBLE);
        recyData.setVisibility(View.GONE);
    }

    @Override
    public void OnClick(AppBase appBase) {
        Intent intent = new Intent(requireActivity(), DetailWorkflowActivity.class);
        intent.putExtra("WorkflowItemId", String.valueOf(appBase.getID()));
        requireActivity().startActivity(intent);
    }

    @Override
    protected void onBroadcastReceived(Intent intent) {
        super.onBroadcastReceived(intent);
        switch (intent.getAction()) {
            case VarsReceiver.CHANGELANGUAGE: {
                setTitle();
                break;
            }
            default: {
                presenter.getFilters(objectPropertySearch);
                break;
            }
        }
    }
}
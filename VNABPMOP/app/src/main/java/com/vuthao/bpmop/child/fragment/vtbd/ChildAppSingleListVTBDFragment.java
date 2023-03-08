package com.vuthao.bpmop.child.fragment.vtbd;

import android.content.Intent;
import android.graphics.Typeface;
import android.os.Bundle;
import android.os.Handler;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.core.content.res.ResourcesCompat;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.swiperefreshlayout.widget.SwipeRefreshLayout;

import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.AnimationController;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.activity.BaseFragment;
import com.vuthao.bpmop.base.api.ApiBPM;
import com.vuthao.bpmop.base.api.ApiController;
import com.vuthao.bpmop.base.model.ApiObject;
import com.vuthao.bpmop.base.model.custom.ObjectPropertySearch;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.base.keyboard.KeyboardManager;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.Workflow;
import com.vuthao.bpmop.base.realm.AppBaseController;
import com.vuthao.bpmop.detail.DetailWorkflowActivity;
import com.vuthao.bpmop.home.adapter.HomePageVTBDAdapter;
import com.vuthao.bpmop.home.presenter.HomePagePresenter;
import com.vuthao.bpmop.shareview.SharedView_PopupFilterVTBD;
import com.vuthao.bpmop.task.DetailCreateTaskActivity;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ChildAppSingleListVTBDFragment extends BaseFragment implements HomePagePresenter.RefreshFilter, ApiBPM.ApiBPMRefreshListener, SwipeRefreshLayout.OnRefreshListener, HomePagePresenter.HomePageListener, TextWatcher, HomePageVTBDAdapter.HomePageVTBDListener {
    @BindView(R.id.ln_ViewChildAppListWorkflow_All)
    LinearLayout lnAll;
    @BindView(R.id.ln_ViewChildAppListWorkflow_Tab)
    LinearLayout lnTab;
    @BindView(R.id.ln_ViewChildAppListWorkflow_Search)
    LinearLayout lnSearch;
    @BindView(R.id.rela_ViewChildAppListWorkflow_Toolbar)
    RelativeLayout relaToolbar;
    @BindView(R.id.tv_ViewChildAppListWorkflow_Name)
    TextView tvName;
    @BindView(R.id.img_ViewChildAppListWorkflow_Back)
    ImageView imgBack;
    @BindView(R.id.ln_ViewChildAppListWorkflow_Filter)
    LinearLayout lnFilter;
    @BindView(R.id.img_ViewChildAppListWorkflow_Filter)
    ImageView imgFilter;
    @BindView(R.id.img_ListWorkflowView_Search_Delete)
    ImageView imgDeleteSearch;
    @BindView(R.id.img_ViewChildAppListWorkflow_ShowSearch)
    ImageView imgShowSearch;
    @BindView(R.id.edt_ListWorkflowView_Search)
    EditText edtSearch;
    @BindView(R.id.swipe_ViewChildAppListWorkflow)
    SwipeRefreshLayout swipe;
    @BindView(R.id.ln_ViewChildAppListWorkflow_NoData)
    LinearLayout lnNoData;
    @BindView(R.id.ln_ViewChildAppListWorkflow_Content)
    LinearLayout lnContent;
    @BindView(R.id.tv_ViewChildAppListWorkflow_NoData)
    TextView tvNoData;
    @BindView(R.id.recy_ViewChildAppListWorkflow)
    RecyclerView recyData;

    private View rootView;
    private Workflow workflow;
    private AppBaseController appBaseController;
    private HomePagePresenter presenter;
    private SharedView_PopupFilterVTBD sharedView_popupFilterVTBD;
    private ApiBPM apiBPM;
    private HomePageVTBDAdapter adapter;
    private ArrayList<AppBase> appBases;
    private LinearLayoutManager mLayoutManager;
    private boolean isFilter = false;
    private boolean isLoading = false;
    private ObjectPropertySearch propertySearch;

    public ChildAppSingleListVTBDFragment() {
    }

    public ChildAppSingleListVTBDFragment(Workflow workflow) {
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
            rootView = inflater.inflate(R.layout.fragment_child_app_single_list_v_t_b_d, container, false);
            ButterKnife.bind(this, rootView);

            init();
            setTitle();
            setData();
            initScrollListener();

            edtSearch.addTextChangedListener(this);
            swipe.setOnRefreshListener(this);
        }
        return rootView;
    }

    private void init() {
        appBaseController = new AppBaseController();
        presenter = new HomePagePresenter(this, this, appBaseController);
        sharedView_popupFilterVTBD = new SharedView_PopupFilterVTBD(presenter, this);
        apiBPM = new ApiBPM(this);
        appBases = new ArrayList<>();
        Utility.share.setupSwipeRefreshLayout(swipe);
        lnTab.setVisibility(View.GONE);
        lnSearch.setVisibility(View.GONE);
        imgDeleteSearch.setVisibility(View.GONE);
        imgShowSearch.setVisibility(View.VISIBLE);

        mLayoutManager = new LinearLayoutManager(getActivity(), LinearLayoutManager.VERTICAL, false);
        recyData.setLayoutManager(mLayoutManager);
        recyData.setDrawingCacheEnabled(true);
        recyData.setHasFixedSize(true);
        recyData.setItemViewCacheSize(20);
    }

    private void initScrollListener() {
        recyData.addOnScrollListener(new RecyclerView.OnScrollListener() {
            @Override
            public void onScrolled(@NonNull RecyclerView recyclerView, int dx, int dy) {
                super.onScrolled(recyclerView, dx, dy);
                if (isLoading) {
                    if (mLayoutManager.findLastCompletelyVisibleItemPosition() == appBases.size() - 1) {
                        loadMore();
                        isLoading = false;
                    }
                }
            }
        });
    }

    private void loadMore() {
        appBases.add(null);
        adapter.notifyItemInserted(appBases.size() - 1);

        propertySearch.setOffset(appBases.size() - 1);
        String data = new Gson().toJson(propertySearch);

        Handler handler = new Handler();
        handler.postDelayed(() -> {
            Call<ApiObject<AppBase>> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getListFilterMyRequest(data);
            call.enqueue(new Callback<ApiObject<AppBase>>() {
                @Override
                public void onResponse(Call<ApiObject<AppBase>> call, Response<ApiObject<AppBase>> response) {
                    if (response.isSuccessful() && response.body() != null) {
                        if (response.body().getData() != null) {
                            appBases.remove(appBases.size() - 1);
                            adapter.notifyItemRemoved(appBases.size());
                            ArrayList<AppBase> newData = appBaseController.modifiedFilters("VTBD", response.body().getData().getData());
                            appBases.addAll(newData);
                            isLoading = newData.size() >= Constants.mFilterLimit;
                        } else {
                            isLoading = false;
                        }
                    } else {
                        isLoading = false;
                    }
                }

                @Override
                public void onFailure(Call<ApiObject<AppBase>> call, Throwable t) {
                    isLoading = false;
                }
            });
        }, 1000);
    }

    private void setTitle() {
        if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
            tvName.setText(workflow.getTitle());

        } else {
            tvName.setText(workflow.getTitleEN());
        }

        tvNoData.setText(Functions.share.getTitle("TEXT_NODATA", "Không có dữ liệu"));
        edtSearch.setHint(Functions.share.getTitle("TEXT_SEARCH", "Tìm kiếm"));
    }

    private void setData() {
        appBases = appBaseController.getVTBDItems(workflow.getWorkflowID());
        if (appBases.size() > 0) {
            lnNoData.setVisibility(View.GONE);
            recyData.setVisibility(View.VISIBLE);
            if (adapter == null) {
                adapter = new HomePageVTBDAdapter(appBases, requireContext(), this);
                recyData.setAdapter(adapter);
            } else {
                adapter.setListRefresh(appBases);
                if (edtSearch.getText().length() > 0) {
                    edtSearch.setText(edtSearch.getText());
                }
            }
        } else {
            lnNoData.setVisibility(View.VISIBLE);
            recyData.setVisibility(View.GONE);
        }
    }

    public void scrollToTop() {
        if (appBases.size() > 0) {
            if (mLayoutManager.findFirstCompletelyVisibleItemPosition() > 0) {
                recyData.smoothScrollToPosition(0);
            }
        }
    }

    @OnClick({R.id.img_ViewChildAppListWorkflow_Back, R.id.ln_ViewChildAppListWorkflow_Filter, R.id.img_ListWorkflowView_Search_Delete, R.id.img_ViewChildAppListWorkflow_ShowSearch})
    public void OnViewClicked(View v) {
        switch (v.getId()) {
            case R.id.img_ViewChildAppListWorkflow_Back: {
                requireActivity().finish();
                break;
            }
            case R.id.img_ViewChildAppListWorkflow_ShowSearch: {
                search();
                break;
            }
            case R.id.img_ListWorkflowView_Search_Delete: {
                edtSearch.setText("");
                break;
            }
            case R.id.ln_ViewChildAppListWorkflow_Filter: {
                filter();
                break;
            }
        }
    }

    private void filter() {
        imgFilter.setColorFilter(ContextCompat.getColor(requireContext(), R.color.clGreenDueDate));
        sharedView_popupFilterVTBD.filter(requireActivity(), relaToolbar, imgFilter, workflow.getWorkflowID());
    }

    private void search() {
        imgShowSearch.startAnimation(AnimationController.share.fadeIn(requireActivity()));
        if (lnSearch.getVisibility() == View.GONE) {
            imgShowSearch.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clVer2BlueMain));
            lnSearch.setVisibility(View.VISIBLE);
            edtSearch.requestFocus();
            KeyboardManager.showKeyBoard(rootView, requireActivity());
        } else {
            imgShowSearch.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clBottomDisable));
            KeyboardManager.hideKeyboard(getActivity());
            lnSearch.setVisibility(View.GONE);
        }
    }

    @Override
    public void OnVTDBClick(AppBase app) {
        //adapter clicks
        if (!app.isRead()) {
            adapter.updateItemRead(app.getID());
        }

        if (app.getResourceCategoryId() == 16) {
            Intent intent = new Intent(getActivity(), DetailCreateTaskActivity.class);
            intent.putExtra("WorkflowItemId", Functions.share.getWorkflowItemIDByUrl(app.getItemUrl()));
            intent.putExtra("isClickFromAction", false);
            intent.putExtra("taskId", app.getID());
            requireActivity().startActivity(intent);
        } else {
            Intent intent = new Intent(getActivity(), DetailWorkflowActivity.class);
            intent.putExtra("WorkflowItemId", Functions.share.getWorkflowItemIDByUrl(app.getItemUrl()));
            requireActivity().startActivity(intent);
        }
    }

    @Override
    public void beforeTextChanged(CharSequence s, int start, int count, int after) {

    }

    @Override
    public void onTextChanged(CharSequence s, int start, int before, int count) {

    }

    @Override
    public void afterTextChanged(Editable s) {
        if (s.toString().length() > 0) {
            imgDeleteSearch.setVisibility(View.VISIBLE);
            edtSearch.setTypeface(ResourcesCompat.getFont(requireActivity(), R.font.fontarial), Typeface.NORMAL);
        } else {
            imgDeleteSearch.setVisibility(View.GONE);
            edtSearch.setTypeface(ResourcesCompat.getFont(requireActivity(), R.font.fontarial), Typeface.ITALIC);
        }

        if (adapter != null) {
            adapter.getFilter().filter(s.toString());
        }
    }

    @Override
    public void OnFilterSuccess(ArrayList<AppBase> apps, String type, String status, ObjectPropertySearch propertySearch) {
        sBaseActivity.hideProgressDialog();
        this.propertySearch = propertySearch;
        isLoading = apps.size() >= Constants.mFilterLimit;

        apps = appBaseController.modifiedFilters("VTBD", apps);
        if (apps.size() > 0) {
            lnNoData.setVisibility(View.GONE);
            recyData.setVisibility(View.VISIBLE);
            if (adapter == null) {
                adapter = new HomePageVTBDAdapter(apps, requireActivity(), this);
                recyData.setAdapter(adapter);
            } else {
                adapter.setListFilter(apps);
            }
        } else {
            lnNoData.setVisibility(View.VISIBLE);
            recyData.setVisibility(View.GONE);
        }

        isFilter = true;
        imgFilter.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clGreenDueDate));
    }

    @Override
    public void OnFilterErr(String err) {
        sBaseActivity.hideProgressDialog();

        if (!err.isEmpty()) {
            Utility.share.showAlertWithOnlyOK(err, requireActivity());
        }

        imgFilter.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clBottomDisable));
    }

    @Override
    public void OnDefaultFilter(String type) {
        isLoading = false;

        if (appBases.size() > 0) {
            lnNoData.setVisibility(View.GONE);
            recyData.setVisibility(View.VISIBLE);
        }

        isFilter = false;
        setData();
        imgFilter.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clBottomDisable));
    }

    @Override
    public void OnFilterDissmiss() {
        if (isFilter) {
            imgFilter.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clGreenDueDate));
        } else {
            imgFilter.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clBottomDisable));
            sharedView_popupFilterVTBD.setDefaultValue();
        }
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
    }

    @Override
    public void onRefresh() {
        apiBPM.updateAllMasterData(false);
    }

    @Override
    public void OnRefreshSuccess() {
        if (swipe.isRefreshing()) {
            swipe.setRefreshing(false);
        }

        if (isFilter) {
            presenter.refreshActionWithFilter("VTBD", appBaseController, propertySearch);
        } else {
            setData();
        }
    }

    @Override
    public void OnRefreshErr() {
        if (swipe.isRefreshing()) {
            swipe.setRefreshing(false);
        }
    }

    @Override
    protected void onBroadcastReceived(Intent intent) {
        super.onBroadcastReceived(intent);
        switch (intent.getAction()) {
            case VarsReceiver.FOLLOW: {
                String workflowId = intent.getStringExtra("WorkflowId");
                boolean isFollow = intent.getBooleanExtra("isFollow", false);
                adapter.updateFollow(workflowId, isFollow);
                break;
            }
            case VarsReceiver.REFRESHAFTERSUBMITACTION: {
                OnRefreshSuccess();
                break;
            }
        }
    }

    @Override
    public void OnRefreshActionFilterSuccess(ArrayList<AppBase> appBases) {
        if (adapter != null) {
            adapter.setListRefresh(appBases);
        } else {
            adapter = new HomePageVTBDAdapter(appBases, requireContext(), this);
            recyData.setAdapter(adapter);
        }
    }

    @Override
    public void OnRefreshActionFilterErr() {

    }
}

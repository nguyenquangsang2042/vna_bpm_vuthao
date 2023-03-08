package com.vuthao.bpmop.child.fragment.vdt;

import android.os.Bundle;
import android.os.Handler;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.activity.BaseFragment;
import com.vuthao.bpmop.base.api.ApiController;
import com.vuthao.bpmop.base.model.ApiObject;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.model.app.Workflow;
import com.vuthao.bpmop.base.model.custom.ObjectPropertySearch;
import com.vuthao.bpmop.base.realm.AppBaseController;
import com.vuthao.bpmop.base.vars.Variable;
import com.vuthao.bpmop.home.adapter.HomePageVDTAdapter;
import com.vuthao.bpmop.vdt.presenter.SingleListVDTPresenter;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ChildAppVDTProcessedFragment extends BaseFragment implements SingleListVDTPresenter.RefreshFilterListener, HomePageVDTAdapter.HomePageRecyVDTListener {
    @BindView(R.id.ln_ViewChildAppListWorkflow_All)
    LinearLayout lnAll;
    @BindView(R.id.ln_ViewChildAppListWorkflow_NoData)
    LinearLayout lnNoData;
    @BindView(R.id.tv_ViewChildAppListWorkflow_NoData)
    TextView tvNoData;
    @BindView(R.id.recy_ViewChildAppListWorkflow)
    RecyclerView recyData;

    private View rootView;
    private Workflow workflow;
    private AppBaseController appBaseController;
    private ArrayList<AppBase> appBases;
    private HomePageVDTAdapter adapter;
    private LinearLayoutManager mLayoutManager;
    private SingleListVDTPresenter presenter;
    private boolean isLoading = false;
    private ObjectPropertySearch propertySearch;

    public ChildAppVDTProcessedFragment() {
        // Required empty public constructor
    }

    public ChildAppVDTProcessedFragment(Workflow workflow) {
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
            rootView = inflater.inflate(R.layout.fragment_child_app_single_list_v_d_t, container, false);
            ButterKnife.bind(this, rootView);

            init();
            setData();
            initScrollListener();
        }
        return rootView;
    }

    private void init() {
        presenter = new SingleListVDTPresenter(this);
        appBases = new ArrayList<>();
        appBaseController = new AppBaseController();
        tvNoData.setText(Functions.share.getTitle("TEXT_NODATA", "No data"));

        mLayoutManager = new LinearLayoutManager(requireActivity(), LinearLayoutManager.VERTICAL, false);
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
            Call<ApiObject<AppBase>> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getListFilterMyTask(data);

            call.enqueue(new Callback<ApiObject<AppBase>>() {
                @Override
                public void onResponse(Call<ApiObject<AppBase>> call, Response<ApiObject<AppBase>> response) {
                    if (response.isSuccessful()) {
                        appBases.remove(appBases.size() - 1);
                        adapter.notifyDataSetChanged();

                        if (response.body() != null && response.body().getData() != null) {
                            ArrayList<AppBase> newData = appBaseController.modifiedFilters("VDT", response.body().getData().getData());
                            appBases.addAll(newData);
                            adapter.notifyDataSetChanged();
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

    public void scrollToTop() {
        if (appBases.size() > 0) {
            if (mLayoutManager.findFirstCompletelyVisibleItemPosition() > 0) {
                recyData.smoothScrollToPosition(0);
            }
        }
    }

    private void setData() {
        appBases = appBaseController.getListVDTDaXuLy(workflow.getWorkflowID());
        if (appBases.size() > 0) {
            lnNoData.setVisibility(View.GONE);
            recyData.setVisibility(View.VISIBLE);
            if (adapter == null) {
                adapter = new HomePageVDTAdapter(this, requireContext(), appBases, Variable.BottomNavigation.SingleListVDT);
                recyData.setAdapter(adapter);
            } else {
                adapter.setListRefresh(appBases);
            }
        } else {
            lnNoData.setVisibility(View.VISIBLE);
            recyData.setVisibility(View.GONE);
        }
    }

    @Override
    public void OnClick(AppBase app) {
        presenter.handleClicks(requireActivity(), appBaseController, adapter, app);
    }

    public HomePageVDTAdapter getAdapter() {
        return adapter;
    }


    public void setListFilter(ArrayList<AppBase> apps, String type, ObjectPropertySearch propertySearch) {
        this.propertySearch = propertySearch;
        isLoading = apps.size() >= Constants.mFilterLimit;

        if (apps.size() > 0) {
            ArrayList<AppBase> appBasesFilter = new ArrayList<>(apps);
            if (adapter == null) {
                adapter = new HomePageVDTAdapter(this, getContext(), appBasesFilter, Variable.BottomNavigation.Filter);
                recyData.setAdapter(adapter);
            } else {
                adapter.setListFilter(apps);
            }

            lnNoData.setVisibility(View.GONE);
            recyData.setVisibility(View.VISIBLE);
        } else {
            lnNoData.setVisibility(View.VISIBLE);
            recyData.setVisibility(View.GONE);
        }
    }

    public void setDefaultFilter() {
        isLoading = false;
        setData();
    }

    public void refresh(boolean isFilter) {
        if (isFilter) {
            presenter.refreshFilterAfterSubmitAction(appBaseController, propertySearch);
        } else {
            setData();
        }
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
    }

    @Override
    public void OnRefreshFilterSuccess(ArrayList<AppBase> appBases) {
        if (adapter != null) {
            adapter.setListRefresh(appBases);
        } else {
            adapter = new HomePageVDTAdapter(this, requireContext(), appBases, Variable.BottomNavigation.SingleListVDT);
            recyData.setAdapter(adapter);
        }
    }

    @Override
    public void OnRefreshFilterErr() {

    }
}

package com.vuthao.bpmop.vdt;

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

public class SingleListVDTProcessedFragment extends BaseFragment implements SingleListVDTPresenter.RefreshFilterListener, HomePageVDTAdapter.HomePageRecyVDTListener  {
    @BindView(R.id.ln_ViewListWorkflow_All)
    LinearLayout lnAll;
    @BindView(R.id.ln_ViewListWorkflow_NoData)
    LinearLayout lnNoData;
    @BindView(R.id.tv_ViewListWorkflow_NoData)
    TextView tvNoData;
    @BindView(R.id.recy_ViewListWorkflow)
    RecyclerView recyData;

    private View rootView;
    private HomePageVDTAdapter adapter;
    private AppBaseController controller;
    private ArrayList<AppBase> appBases;
    private LinearLayoutManager mLayoutManager;
    private SingleListVDTPresenter presenter;
    private boolean isLoading = false;
    private ObjectPropertySearch propertySearch;


    public SingleListVDTProcessedFragment() {
        // Required empty public constructor
    }

    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        if (rootView == null) {
            rootView = inflater.inflate(R.layout.fragment_single_list_v_d_t, container, false);
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
        controller = new AppBaseController();

        mLayoutManager = new LinearLayoutManager(requireActivity(), LinearLayoutManager.VERTICAL, false);
        recyData.setLayoutManager(mLayoutManager);
        recyData.setDrawingCacheEnabled(true);
        recyData.setHasFixedSize(true);
        recyData.setItemViewCacheSize(20);

        setTextLanguageChanges();
    }

    public void setTextLanguageChanges() {
        tvNoData.setText(Functions.share.getTitle("TEXT_NODATA", "No data"));
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
                            ArrayList<AppBase> newData = controller.modifiedFilters("VDT", response.body().getData().getData());
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

    public HomePageVDTAdapter getAdapter() {
        return adapter;
    }

    private void setData() {
        appBases = controller.getListVDTDaXuLy(0);
        if (appBases.size() > 0) {
            lnNoData.setVisibility(View.GONE);
            recyData.setVisibility(View.VISIBLE);
            if (adapter == null) {
                adapter = new HomePageVDTAdapter(this, requireActivity(), appBases, Variable.BottomNavigation.SingleListVDT);
                recyData.setAdapter(adapter);
            } else {
                adapter.setListRefresh(appBases);
            }
        } else {
            lnNoData.setVisibility(View.VISIBLE);
            recyData.setVisibility(View.GONE);
        }
    }

    public void filterSearch(String s) {
        if (adapter != null){
            adapter.getFilter().filter(s);
        }
    }

    public void refresh(boolean isFilter) {
        if (!isFilter) {
            setData();
        } else {
            presenter.refreshFilterAfterSubmitAction(controller, propertySearch);
        }
    }

    public void setListFilter(ArrayList<AppBase> apps, String type, ObjectPropertySearch propertySearch) {
        this.propertySearch = propertySearch;
        isLoading = apps.size() >= Constants.mFilterLimit;

        if (apps.size() > 0) {
            if (adapter == null) {
                adapter = new HomePageVDTAdapter(this, getContext(), apps, Variable.BottomNavigation.Filter);
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
       setData();
    }

    public void scrollToTop() {
        if (appBases.size() > 0) {
            if (mLayoutManager.findFirstCompletelyVisibleItemPosition() > 0) {
                recyData.smoothScrollToPosition(0);
            }
        }
    }

    @Override
    public void OnClick(AppBase app) {
        presenter.handleClicks(requireActivity(),controller, adapter, app);
    }

    @Override
    public void OnRefreshFilterSuccess(ArrayList<AppBase> appBases) {
        if (adapter != null) {
            adapter.setListRefresh(appBases);
        } else {
            adapter = new HomePageVDTAdapter(this, requireActivity(), appBases, Variable.BottomNavigation.SingleListVDT);
            recyData.setAdapter(adapter);
        }
    }

    @Override
    public void OnRefreshFilterErr() {

    }
}

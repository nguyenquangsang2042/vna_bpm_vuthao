package com.vuthao.bpmop.home;

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
import com.vuthao.bpmop.home.adapter.HomePageVTBDAdapter;
import com.vuthao.bpmop.home.presenter.HomePagePresenter;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class HomePageVTBDFragment extends BaseFragment implements HomePagePresenter.RefreshFilter, HomePageVDTAdapter.HomePageRecyVDTListener, HomePageVTBDAdapter.HomePageVTBDListener {
    @BindView(R.id.recy_ViewHomePagePager)
    RecyclerView rcvViewHomePagePager;
    @BindView(R.id.ln_ViewHomePagePager_NotData)
    LinearLayout lnNoData;
    @BindView(R.id.tv_ViewHomePagePager_NotData)
    TextView tvNodata;

    private View rootView;
    private AppBaseController appBaseController;
    private HomePageVTBDAdapter adapter;
    private ArrayList<AppBase> appBaseExts;
    private LinearLayoutManager mLayoutManager;
    private boolean isLoading = false;
    private ObjectPropertySearch propertySearch;
    private HomePagePresenter presenter;

    public HomePageVTBDFragment() {
        // Required empty public constructor
    }

    public HomePageVTBDAdapter getAdapter() {
        return adapter;
    }

    public void setAdapter(HomePageVTBDAdapter adapter) {
        this.adapter = adapter;
    }

    public ObjectPropertySearch getPropertySearch() {
        return propertySearch;
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        if (rootView == null) {
            rootView = inflater.inflate(R.layout.fragment_pager_home_page_single_list, container, false);
            ButterKnife.bind(this, rootView);

            init();
            setData();
            initScrollListener();
        }

        return rootView;
    }

    private void initScrollListener() {
        rcvViewHomePagePager.addOnScrollListener(new RecyclerView.OnScrollListener() {
            @Override
            public void onScrolled(@NonNull RecyclerView recyclerView, int dx, int dy) {
                super.onScrolled(recyclerView, dx, dy);
                if (isLoading) {
                    if (mLayoutManager.findLastCompletelyVisibleItemPosition() == appBaseExts.size() - 1) {
                        loadMore();
                        isLoading = false;
                    }
                }
            }
        });
    }

    private void loadMore() {
        appBaseExts.add(null);
        adapter.notifyItemInserted(appBaseExts.size() - 1);

        propertySearch.setOffset(appBaseExts.size() - 1);
        String data = new Gson().toJson(propertySearch);

        Handler handler = new Handler();
        handler.postDelayed(() -> {
            Call<ApiObject<AppBase>> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getListFilterMyRequest(data);

            call.enqueue(new Callback<ApiObject<AppBase>>() {
                @Override
                public void onResponse(Call<ApiObject<AppBase>> call, Response<ApiObject<AppBase>> response) {
                    if (response.isSuccessful()) {
                        appBaseExts.remove(appBaseExts.size() - 1);
                        adapter.notifyItemRemoved(appBaseExts.size());

                        if (response.body() != null && response.body().getData() != null) {
                            ArrayList<AppBase> newData = appBaseController.modifiedFilters("VTBD", response.body().getData().getData());
                            appBaseExts.addAll(newData);
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

    public void setDefaultFilter(boolean isFilter) {
        setData();
    }

    private void init() {
        presenter = new HomePagePresenter(this);
        appBaseController = new AppBaseController();
        appBaseExts = new ArrayList<>();
        appBaseExts = new ArrayList<>();

        mLayoutManager = new LinearLayoutManager(requireActivity(), LinearLayoutManager.VERTICAL, false);
        rcvViewHomePagePager.setLayoutManager(mLayoutManager);
        rcvViewHomePagePager.setDrawingCacheEnabled(true);
        rcvViewHomePagePager.setHasFixedSize(true);
        rcvViewHomePagePager.setItemViewCacheSize(20);

        setTextLanguageChanges();
    }

    public void setTextLanguageChanges() {
        tvNodata.setText(Functions.share.getTitle("TEXT_NODATA", "No data"));
    }

    private void setData() {
        appBaseExts = appBaseController.getVTBDItems(0);
        if (appBaseExts.size() > 0) {
            lnNoData.setVisibility(View.GONE);
            rcvViewHomePagePager.setVisibility(View.VISIBLE);
            if (adapter == null) {
                adapter = new HomePageVTBDAdapter(appBaseExts, requireActivity(), this);
                rcvViewHomePagePager.setAdapter(adapter);
            } else {
                adapter.setListRefresh(appBaseExts);
            }
        } else {
            lnNoData.setVisibility(View.VISIBLE);
            rcvViewHomePagePager.setVisibility(View.GONE);
        }
    }

    public void refresh(boolean isFilter) {
        if (!isFilter) {
            setData();
        } else {
            presenter.refreshActionWithFilter("VTDB", appBaseController, propertySearch);
        }
    }

    public void scrollToTop() {
        if (appBaseExts.size() > 0) {
            if (mLayoutManager.findFirstCompletelyVisibleItemPosition() > 0) {
                rcvViewHomePagePager.smoothScrollToPosition(0);
            }
        }
    }

    public void setListFilter(ArrayList<AppBase> filter, String type, ObjectPropertySearch propertySearch) {
        this.propertySearch = propertySearch;
        isLoading = filter.size() >= Constants.mFilterLimit;
        if (filter.size() > 0) {
            if (adapter == null) {
                adapter = new HomePageVTBDAdapter(filter, getContext(), this);
                rcvViewHomePagePager.setAdapter(adapter);
            } else {
                adapter.setListFilter(filter);
            }
            rcvViewHomePagePager.setVisibility(View.VISIBLE);
            lnNoData.setVisibility(View.GONE);
        } else {
            lnNoData.setVisibility(View.VISIBLE);
            rcvViewHomePagePager.setVisibility(View.GONE);
        }
    }

    @Override
    public void OnClick(AppBase app) {
        handleClicks(app);
    }

    @Override
    public void OnVTDBClick(AppBase app) {
        handleClicks(app);
    }

    private void handleClicks(AppBase appBase) {
        presenter.hanldeVTBDClicks(requireActivity(), appBase, appBaseController, adapter);
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
    public void onPause() {
        super.onPause();
    }

    @Override
    public void OnRefreshActionFilterSuccess(ArrayList<AppBase> appBases) {
        if (adapter != null) {
            adapter.setListRefresh(appBases);
        } else {
            adapter = new HomePageVTBDAdapter(appBases, requireContext(), this);
            rcvViewHomePagePager.setAdapter(adapter);
        }
    }

    @Override
    public void OnRefreshActionFilterErr() {

    }
}

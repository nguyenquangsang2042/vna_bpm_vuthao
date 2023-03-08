package com.vuthao.bpmop.home;

import android.content.Intent;
import android.os.Bundle;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import android.os.Handler;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.LinearLayout;
import android.widget.TextView;

import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.activity.BaseFragment;
import com.vuthao.bpmop.base.api.ApiController;
import com.vuthao.bpmop.base.model.ApiObject;
import com.vuthao.bpmop.base.model.custom.ObjectPropertySearch;
import com.vuthao.bpmop.base.vars.Variable;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.realm.AppBaseController;
import com.vuthao.bpmop.detail.DetailWorkflowActivity;
import com.vuthao.bpmop.home.adapter.HomePageVDTAdapter;
import com.vuthao.bpmop.home.adapter.HomePageVTBDAdapter;
import com.vuthao.bpmop.home.presenter.HomePagePresenter;
import com.vuthao.bpmop.task.DetailCreateTaskActivity;
import com.vuthao.bpmop.vdt.presenter.SingleListVDTPresenter;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class HomePageVDTFragment extends BaseFragment implements HomePagePresenter.RefreshFilter, HomePageVDTAdapter.HomePageRecyVDTListener, HomePageVTBDAdapter.HomePageVTBDListener {
    @BindView(R.id.recy_ViewHomePagePager)
    RecyclerView rcvViewHomePagePager;
    @BindView(R.id.ln_ViewHomePagePager_NotData)
    LinearLayout lnNoData;
    @BindView(R.id.tv_ViewHomePagePager_NotData)
    TextView tvNodata;

    private View rootView;
    private AppBaseController appBaseController;
    private HomePageVDTAdapter adapter;
    private ArrayList<AppBase> appBaseExts;
    private LinearLayoutManager mLayoutManager;
    private boolean isLoading = false;
    private ObjectPropertySearch propertySearch;
    private HomePagePresenter presenter;
    private SingleListVDTPresenter.PagerSingleListVDTListener listener;

    public HomePageVDTFragment() {
        // Required empty public constructor
    }

    public HomePageVDTFragment(SingleListVDTPresenter.PagerSingleListVDTListener listener) {
        this.listener = listener;
    }

    public HomePageVDTAdapter getAdapter() {
        return adapter;
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

    public void setTextLanguageChanges() {
        tvNodata.setText(Functions.share.getTitle("TEXT_NODATA", "No data"));
    }

    private void loadMore() {
        appBaseExts.add(null);
        adapter.notifyItemInserted(appBaseExts.size() - 1);


        propertySearch.setOffset(appBaseExts.size() - 1);
        String data = new Gson().toJson(propertySearch);

        Handler handler = new Handler();
        handler.postDelayed(() -> {
            Call<ApiObject<AppBase>> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getListFilterMyTask(data);
            call.enqueue(new Callback<ApiObject<AppBase>>() {
                @Override
                public void onResponse(Call<ApiObject<AppBase>> call, Response<ApiObject<AppBase>> response) {
                    if (response.isSuccessful()) {
                        appBaseExts.remove(appBaseExts.size() - 1);
                        adapter.notifyItemRemoved(appBaseExts.size());

                        if (response.body() != null) {
                            if (response.body().getData() != null) {
                                ArrayList<AppBase> newData = appBaseController.modifiedFilters("VDT", response.body().getData().getData());
                                appBaseExts.addAll(newData);
                                adapter.notifyDataSetChanged();

                                listener.OnFilterCount(response.body().getData().getMoreInfo().getTotalRecord());
                                isLoading = newData.size() >= Constants.mFilterLimit;
                            } else {
                                isLoading = false;
                            }
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

    public void setDefaultFilter() {
        isLoading = false;
        setData();
    }

    private void init() {
        presenter = new HomePagePresenter(this);
        appBaseController = new AppBaseController();
        appBaseExts = new ArrayList<>();

        mLayoutManager = new LinearLayoutManager(requireActivity(), LinearLayoutManager.VERTICAL, false);
        rcvViewHomePagePager.setLayoutManager(mLayoutManager);
        rcvViewHomePagePager.setDrawingCacheEnabled(true);
        rcvViewHomePagePager.setHasFixedSize(true);
        rcvViewHomePagePager.setItemViewCacheSize(20);

        setTextLanguageChanges();
    }

    private void setData() {
        appBaseExts = appBaseController.getVDTItems(0, 0);
        if (appBaseExts.size() > 0) {
            lnNoData.setVisibility(View.GONE);
            rcvViewHomePagePager.setVisibility(View.VISIBLE);
            if (adapter == null) {
                adapter = new HomePageVDTAdapter(this, requireActivity(), appBaseExts, Variable.BottomNavigation.HomePage);
                rcvViewHomePagePager.setAdapter(adapter);
            } else {
                adapter.setListRefresh(appBaseExts);
                listener.OnFilterCount(appBaseExts.size());
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
            presenter.refreshActionWithFilter("VDT", appBaseController, propertySearch);
        }
    }

    public void scrollToTop() {
        if (appBaseExts.size() > 0) {
            if (mLayoutManager.findFirstCompletelyVisibleItemPosition() > 0) {
                rcvViewHomePagePager.smoothScrollToPosition(0);
            }
        }
    }

    public void setListFilter(ArrayList<AppBase> filter, String type, String status, ObjectPropertySearch propertySearch) {
        this.propertySearch = propertySearch;
        isLoading = filter.size() >= Constants.mFilterLimit;
        if (filter.size() > 0) {
            if (adapter == null) {
                adapter = new HomePageVDTAdapter(this, getContext(), filter, status.equals("processed") ? Variable.BottomNavigation.SingleListVDT : Variable.BottomNavigation.Filter);
                rcvViewHomePagePager.setAdapter(adapter);
            } else {
                adapter.setType(status.equals("processed") ? Variable.BottomNavigation.SingleListVDT : Variable.BottomNavigation.Filter);
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
        presenter.setReadNotify(app.getNotifyId());
        handleClicks(app);
    }

    @Override
    public void OnVTDBClick(AppBase app) {
        handleClicks(app);
    }

    private void handleClicks(AppBase appBase) {
        presenter.hanldeVDTClicks(requireActivity(), appBase, appBaseController, adapter);
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
            this.appBaseExts.clear();
            this.appBaseExts.addAll(appBases);
            adapter.setListRefresh(this.appBaseExts);
            listener.OnFilterCount(this.appBaseExts.size());
        } else {
            adapter = new HomePageVDTAdapter(this, requireActivity(), appBases, Variable.BottomNavigation.HomePage);
            rcvViewHomePagePager.setAdapter(adapter);
        }
    }

    @Override
    public void OnRefreshActionFilterErr() {

    }
}
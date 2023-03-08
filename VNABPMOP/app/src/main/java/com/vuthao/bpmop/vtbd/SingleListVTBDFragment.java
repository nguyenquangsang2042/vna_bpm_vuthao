package com.vuthao.bpmop.vtbd;

import android.annotation.SuppressLint;
import android.content.Intent;
import android.graphics.Typeface;
import android.os.Bundle;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.core.content.res.ResourcesCompat;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.swiperefreshlayout.widget.SwipeRefreshLayout;

import android.os.Handler;
import android.text.Editable;
import android.text.TextWatcher;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;
import android.widget.TextView;

import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.AnimationController;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.ImageLoader;
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
import com.vuthao.bpmop.base.realm.AppBaseController;
import com.vuthao.bpmop.detail.DetailWorkflowActivity;
import com.vuthao.bpmop.home.adapter.HomePageVTBDAdapter;
import com.vuthao.bpmop.home.presenter.HomePagePresenter;
import com.vuthao.bpmop.shareview.SharedView_PopupFilterVTBD;
import com.vuthao.bpmop.task.DetailCreateTaskActivity;
import com.vuthao.bpmop.vdt.presenter.SingleListVDTPresenter;
import com.vuthao.bpmop.vtbd.presenter.SingleListVTBDPresenter;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;
import de.hdodenhof.circleimageview.CircleImageView;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class SingleListVTBDFragment extends BaseFragment implements SingleListVTBDPresenter.RefreshFilterListener, ApiBPM.ApiBPMRefreshListener, HomePagePresenter.HomePageListener, TextWatcher, HomePageVTBDAdapter.HomePageVTBDListener, SwipeRefreshLayout.OnRefreshListener {
    @BindView(R.id.ln_ViewListWorkflow_All)
    LinearLayout lnAll;
    @BindView(R.id.rela_ViewListWorkflow_Toolbar)
    RelativeLayout relaToolbar;
    @BindView(R.id.img_ViewListWorkflow_Avata)
    CircleImageView imgAvatar;
    @BindView(R.id.tv_ViewListWorkflow_Name)
    TextView tvName;
    @BindView(R.id.ln_ViewListWorkflow_Filter)
    LinearLayout lnFilter;
    @BindView(R.id.img_ViewListWorkflow_Filter)
    ImageView imgFilter;
    @BindView(R.id.img_ListWorkflowView_Search_Delete)
    ImageView imgDeleteSearch;
    @BindView(R.id.edt_ListWorkflowView_Search)
    EditText edtSearch;
    @BindView(R.id.swipe_ViewListWorkflow)
    SwipeRefreshLayout swipe;
    @BindView(R.id.ln_ViewListWorkflow_NoData)
    LinearLayout lnNoData;
    @BindView(R.id.ln_ViewListWorkflow_Content)
    LinearLayout lnContent;
    @BindView(R.id.tv_ViewListWorkflow_NoData)
    TextView tvNoData;
    @BindView(R.id.recy_ViewListWorkflow)
    RecyclerView recyData;
    @BindView(R.id.img_ViewListWorkflow_ShowSearch)
    ImageView imgShowSearch;
    @BindView(R.id.ln_ViewListWorkflow_Search)
    LinearLayout lnSearch;

    private View rootView;
    private HomePageVTBDAdapter adapter;
    private AppBaseController controller;
    private ApiBPM apiBPM;
    private ArrayList<AppBase> appBases;
    private SharedView_PopupFilterVTBD sharedView_popupFilterVTBD;
    private boolean isFilter = false;
    private LinearLayoutManager mLayoutManager;
    private boolean isLoading = false;
    private SingleListVTBDPresenter presenter;
    private ObjectPropertySearch propertySearch;

    public SingleListVTBDFragment() {
        // Required empty public constructor
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        if (rootView == null) {
            rootView = inflater.inflate(R.layout.fragment_single_list_v_t_b_d, container, false);
            ButterKnife.bind(this, rootView);

            init();
            setData();
            setTitle();
            initScrollListener();

            swipe.setOnRefreshListener(this);
            edtSearch.addTextChangedListener(this);
        }

        return rootView;
    }

    private void init() {
        apiBPM = new ApiBPM(this);
        controller = new AppBaseController();
        presenter = new SingleListVTBDPresenter(this);
        HomePagePresenter presenter = new HomePagePresenter(this, controller);
        sharedView_popupFilterVTBD = new SharedView_PopupFilterVTBD(presenter, this);
        appBases = new ArrayList<>();
        imgDeleteSearch.setVisibility(View.GONE);
        imgShowSearch.setVisibility(View.VISIBLE);

        Utility.share.setupSwipeRefreshLayout(swipe);
        ImageLoader.getInstance().loadImageUserWithToken(requireActivity(), Constants.BASE_URL + CurrentUser.getInstance().getUser().getImagePath(), imgAvatar);

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
            Call<ApiObject<AppBase>> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getListFilterMyRequest(data);
            call.enqueue(new Callback<ApiObject<AppBase>>() {
                @Override
                public void onResponse(Call<ApiObject<AppBase>> call, Response<ApiObject<AppBase>> response) {
                    if (response.isSuccessful() && response.body() != null) {
                        if (response.body().getData() != null) {
                            appBases.remove(appBases.size() - 1);
                            adapter.notifyItemRemoved(appBases.size());
                            ArrayList<AppBase> newData = controller.modifiedFilters("VTBD", response.body().getData().getData());
                            appBases.addAll(newData);

                            setCount(response.body().getData().getMoreInfo().getTotalRecord());
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

    @SuppressLint("SetTextI18n")
    public void setTitle() {
        tvName.setText(Functions.share.getTitle("TEXT_FROMME", "Tôi bắt đầu") + " " + Functions.share.getCountOfNumText(tvName.getText().toString()));
        tvNoData.setText(Functions.share.getTitle("TEXT_NODATA", "Không có dữ liệu"));
        edtSearch.setHint(Functions.share.getTitle("TEXT_SEARCH", "Tìm kiếm"));
        Functions.share.setTVHighligtColor(tvName, tvName.getText().toString(), "(", ")", 0);
    }

    public void scrollToTop() {
        if (appBases.size() > 0) {
            if (mLayoutManager.findFirstCompletelyVisibleItemPosition() > 0) {
                recyData.smoothScrollToPosition(0);
            }
        }
    }

    private void setData() {
        appBases = controller.getVTBDItems(0);
        if (appBases.size() > 0) {
            lnNoData.setVisibility(View.GONE);
            recyData.setVisibility(View.VISIBLE);
            if (adapter == null) {
                adapter = new HomePageVTBDAdapter(appBases, sBaseActivity, this);
                recyData.setAdapter(adapter);
            } else {
                adapter.setListRefresh(appBases);

                // vẽ lại trạng thái search
                if (edtSearch.getText().length() > 0) {
                    edtSearch.setText(edtSearch.getText());
                }
            }
        } else {
            lnNoData.setVisibility(View.VISIBLE);
            recyData.setVisibility(View.GONE);
        }

        setCount(controller.getVTBDCounts(0));
    }

    private void setCount(int count) {
        Functions.share.setFormatItemCount(tvName, count, "vtbd", "");
        Functions.share.setTVHighligtColor(tvName, tvName.getText().toString(), "(", ")", 0);
    }

    @OnClick({R.id.img_ViewListWorkflow_Avata, R.id.tv_ViewListWorkflow_Name, R.id.img_ViewListWorkflow_ShowSearch, R.id.img_ListWorkflowView_Search_Delete, R.id.ln_ViewListWorkflow_Filter})
    public void OnViewClicked(View v) {
        switch (v.getId()) {
            case R.id.tv_ViewListWorkflow_Name:
            case R.id.img_ViewListWorkflow_Avata: {
                sBaseActivity.openLeftMenu();
                break;
            }
            case R.id.img_ViewListWorkflow_ShowSearch: {
                search();
                break;
            }
            case R.id.img_ListWorkflowView_Search_Delete: {
                edtSearch.setText("");
                break;
            }
            case R.id.ln_ViewListWorkflow_Filter: {
                filter();
                break;
            }
        }
    }

    private void filter() {
        imgFilter.setColorFilter(ContextCompat.getColor(BaseActivity.sBaseActivity, R.color.clGreenDueDate));
        sharedView_popupFilterVTBD.filter(requireActivity(), relaToolbar, imgFilter, 0);
    }

    private void search() {
        imgShowSearch.startAnimation(AnimationController.share.fadeIn(getActivity()));
        if (lnSearch.getVisibility() == View.GONE) {
            imgShowSearch.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clVer2BlueMain));
            lnSearch.setVisibility(View.VISIBLE);
            edtSearch.requestFocus();
            KeyboardManager.showKeyBoard(rootView, requireActivity());
        } else {
            KeyboardManager.hideKeyboard(getActivity());
            lnSearch.setVisibility(View.GONE);
            imgShowSearch.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clBottomDisable));
        }
    }

    @Override
    public void onRefresh() {
        apiBPM.updateAllMasterData(false);
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
    public void OnVTDBClick(AppBase app) {
       presenter.handleClicks(requireActivity(), app);
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
        switch (intent.getAction()) {
            case VarsReceiver.FOLLOW: {
                String workflowId = intent.getStringExtra("WorkflowId");
                boolean isFollow = intent.getBooleanExtra("isFollow", false);
                adapter.updateFollow(workflowId, isFollow);
                break;
            }
            case VarsReceiver.CHANGELANGUAGE: {
                setTitle();
                if (adapter != null) {
                    adapter.notifyDataSetChanged();
                }

                sharedView_popupFilterVTBD.changeLanguage();
                break;
            }
            case VarsReceiver.PUSHNOTIFICATION: {
                onRefresh();
                break;
            }
            case VarsReceiver.CREATE_TASK:
            case VarsReceiver.REFRESHAFTERSUBMITACTION: {
                OnRefreshSuccess();
                break;
            }
            case VarsReceiver.REFRESHNAVIGATIONBOTTOM: {
                new ApiBPM(this, 3).updateDataSubmitAction(false);
                break;
            }
        }
    }

    @Override
    public void OnFilterSuccess(ArrayList<AppBase> apps, String type, String status, ObjectPropertySearch propertySearch) {
        sBaseActivity.hideProgressDialog();
        this.propertySearch = propertySearch;
        apps = controller.modifiedFilters("VTBD", apps);
        isLoading = apps.size() >= Constants.mFilterLimit;
        if (apps.size() > 0) {
            lnNoData.setVisibility(View.GONE);
            recyData.setVisibility(View.VISIBLE);
            if (adapter == null) {
                adapter = new HomePageVTBDAdapter(apps, requireActivity(), this);
                recyData.setAdapter(adapter);
            } else {
                adapter.setListFilter(apps);
            }

            setCount(apps.size());
        } else {
            lnNoData.setVisibility(View.VISIBLE);
            recyData.setVisibility(View.GONE);
            setCount(0);
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
        isFilter = false;
        setData();
        setCount(controller.getVTBDCounts(0));
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
    public void OnRefreshSuccess() {
        if (swipe.isRefreshing()) {
            swipe.setRefreshing(false);
        }

        if(isFilter) {
            presenter.refreshFilterAfterSubmitAction(controller, propertySearch);
        } else {
            setData();
            setCount(controller.getVTBDCounts(0));
        }

        setTitle();
        sBaseActivity.getObjLeftMenu().OnVTBDCount(controller.getVTBDCounts(0));
    }

    @Override
    public void OnRefreshErr() {
        if (swipe.isRefreshing()) {
            swipe.setRefreshing(false);
        }
    }

    @Override
    public void OnRefreshFilterSuccess(ArrayList<AppBase> appBases) {
        if (adapter != null) {
            adapter.setListRefresh(appBases);
        } else {
            adapter = new HomePageVTBDAdapter(appBases, sBaseActivity, this);
            recyData.setAdapter(adapter);
        }
    }

    @Override
    public void OnRefreshFilterErr() {

    }
}
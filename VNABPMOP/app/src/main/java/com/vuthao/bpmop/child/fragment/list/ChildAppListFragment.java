package com.vuthao.bpmop.child.fragment.list;

import android.content.BroadcastReceiver;
import android.content.Context;
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
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.activity.BaseFragment;
import com.vuthao.bpmop.base.api.ApiBPM;
import com.vuthao.bpmop.base.keyboard.KeyboardManager;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.ResourceView;
import com.vuthao.bpmop.base.model.app.Workflow;
import com.vuthao.bpmop.base.model.custom.DetailList;
import com.vuthao.bpmop.base.model.custom.ObjectPropertySearch;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.child.fragment.list.adapter.ListDynamicAdapter;
import com.vuthao.bpmop.child.fragment.list.presenter.ChildAppListPresenter;
import com.vuthao.bpmop.shareview.ShareView_PopupFilterList;

import org.json.JSONObject;

import java.util.ArrayList;
import java.util.HashMap;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;

public class ChildAppListFragment extends BaseFragment implements ShareView_PopupFilterList.PopupFilterListListener, ApiBPM.ApiBPMRefreshListener, TextWatcher, ListDynamicAdapter.ListDynamicListener, ChildAppListPresenter.ChildAppListListener, SwipeRefreshLayout.OnRefreshListener {
    @BindView(R.id.img_ViewList_Back)
    ImageView imgBack;
    @BindView(R.id.tv_ViewList_Title)
    TextView tvTitle;
    @BindView(R.id.tv_ViewList_SubTitle)
    TextView tvSubTitle;
    @BindView(R.id.swipe_ViewList)
    SwipeRefreshLayout swipe;
    @BindView(R.id.ln_ViewList_Toolbar)
    LinearLayout lnToolbar;
    @BindView(R.id.img_ViewList_Filter)
    ImageView imgFilter;
    @BindView(R.id.img_ViewList_SubTitle_Previous)
    ImageView imgSubPrevious;
    @BindView(R.id.img_ViewList_SubTitle_Next)
    ImageView imgSubNext;
    @BindView(R.id.tv_ViewList_NoData2)
    TextView tvNoData2;
    @BindView(R.id.ln_ViewList_Search)
    LinearLayout lnSearch;
    @BindView(R.id.edt_ViewList_Search)
    EditText edtSearch;
    @BindView(R.id.img_ViewList_Search_Delete)
    ImageView imgDeleteSearch;
    @BindView(R.id.img_ViewList_ShowSearch)
    ImageView imgShowSearch;
    @BindView(R.id.recy_ViewList_Category_Dynamic)
    RecyclerView recyCategoryDynamic;
    @BindView(R.id.ln_ViewList_Category_Dynamic)
    LinearLayout lnCategoryDynamic;
    @BindView(R.id.ln_ViewList_NoData2)
    LinearLayout lnNoData2;
    @BindView(R.id.ln_ViewList_lnLoading)
    LinearLayout lnLoading;
    @BindView(R.id.rlView)
    RelativeLayout rlView;

    private View rootView;
    private Workflow workflow;
    private ArrayList<ResourceView> lstResourceView;
    ArrayList<JSONObject> lstJObjectDynamic;
    private ArrayList<DetailList.Headers> headers;
    private ResourceView currentResourceView;
    private ChildAppListPresenter presenter;
    private ListDynamicAdapter adapter;
    private AnimationController animationController;
    private ShareView_PopupFilterList popupFilterList;
    private ObjectPropertySearch objectPropertySearch;
    private int position = 0;
    private boolean isLoading = false;
    private boolean isFilter;
    private LinearLayoutManager mLayoutManager;
    private ApiBPM apiBPM;

    public ChildAppListFragment(Workflow workflow) {
        this.workflow = workflow;
    }

    public ChildAppListFragment() {
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
            rootView = inflater.inflate(R.layout.fragment_child_app_list, container, false);
            ButterKnife.bind(this, rootView);

            init();
            setTitle();
            setData();
            initScrollListener();

            swipe.setOnRefreshListener(this);
            edtSearch.addTextChangedListener(this);
        }
        return rootView;
    }

    private void init() {
        presenter = new ChildAppListPresenter(this);
        animationController = new AnimationController();
        lstResourceView = new ArrayList<>();
        lstJObjectDynamic = new ArrayList<>();
        apiBPM = new ApiBPM(this);

        headers = new ArrayList<>();
        popupFilterList = new ShareView_PopupFilterList(requireActivity(), rlView, this);
        Utility.share.setupSwipeRefreshLayout(swipe);
        BroadcastUtility.register(requireActivity(), mReceiver, VarsReceiver.FORMCLICK);
        BroadcastUtility.register(requireActivity(), mReceiver, VarsReceiver.UPDATEFORM);
        BroadcastUtility.register(requireActivity(), mReceiver, VarsReceiver.FOLLOW);
        BroadcastUtility.register(requireActivity(), mReceiver, VarsReceiver.REFRESHAFTERSUBMITACTION);

        mLayoutManager = new LinearLayoutManager(requireActivity(), LinearLayoutManager.VERTICAL, false);
        recyCategoryDynamic.setLayoutManager(mLayoutManager);
        recyCategoryDynamic.setDrawingCacheEnabled(true);
        recyCategoryDynamic.setHasFixedSize(false);
        recyCategoryDynamic.setItemViewCacheSize(20);

        // Chưa load APIs xong thì chưa cho click
        imgFilter.setEnabled(false);
        imgShowSearch.setEnabled(false);
    }

    //Khi bam vao tabs moi goi api tranh bi lag
    private void setData() {
        lstResourceView = presenter.getListResource(workflow.getWorkflowID());
        if (!lstResourceView.isEmpty()) {
            setResourceView(lstResourceView.get(0));
            position = 0;
        } else {
            lnNoData2.setVisibility(View.VISIBLE);
            lnCategoryDynamic.setVisibility(View.GONE);
            lnLoading.setVisibility(View.GONE);
        }
    }

    private void initScrollListener() {
        recyCategoryDynamic.addOnScrollListener(new RecyclerView.OnScrollListener() {
            @Override
            public void onScrolled(@NonNull RecyclerView recyclerView, int dx, int dy) {
                super.onScrolled(recyclerView, dx, dy);
                if (isLoading) {
                    if (mLayoutManager.findLastCompletelyVisibleItemPosition() == lstJObjectDynamic.size() - 1) {
                        loadMore();
                        isLoading = false;
                    }
                }
            }
        });
    }

    private void loadMore() {
        lstJObjectDynamic.add(null);
        adapter.notifyItemInserted(lstJObjectDynamic.size() - 1);

        objectPropertySearch.setOffset(lstJObjectDynamic.size() - 1);
        Handler handler = new Handler();

        handler.postDelayed(() -> {
            presenter.getDynamicMoreFormField(objectPropertySearch);
        }, 1000);
    }

    private void setResourceView(ResourceView resourceView) {
        currentResourceView = resourceView;
        if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
            tvSubTitle.setText(currentResourceView.getTitle());
        } else {
            tvSubTitle.setText(currentResourceView.getTitleEN());
        }

        presenter.getDynamicFormField(currentResourceView.getID(), null);
    }

    private void setListDynamic() {
        lnNoData2.setVisibility(View.GONE);
        lnCategoryDynamic.setVisibility(View.VISIBLE);

        adapter = new ListDynamicAdapter(requireActivity(), headers, lstJObjectDynamic, this);
        recyCategoryDynamic.setAdapter(adapter);

        if (edtSearch.getText().length() > 0) {
            edtSearch.setText(edtSearch.getText());
        }
    }

    private void setTitle() {
        if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
            tvTitle.setText(workflow.getTitle());
        } else {
            tvTitle.setText(workflow.getTitleEN());
        }

        tvNoData2.setText(Functions.share.getTitle("TEXT_NODATA", "No data"));
    }

    private void previous() {
        int index = position - 1 >= 0 ? position - 1 : position;
        if (index != -1 && index != position) {
            lnLoading.setVisibility(View.VISIBLE);
            position = index;
            setResourceView(lstResourceView.get(index));
            popupFilterList.clear();
            if (!edtSearch.getText().toString().isEmpty()) {
                edtSearch.setText("");
            }
        }
    }

    private void next() {
        int index = position + 1 <= lstResourceView.size() - 1 ? position + 1 : position;
        if (index != -1 && index != position) {
            lnLoading.setVisibility(View.VISIBLE);
            position = index;
            setResourceView(lstResourceView.get(index));
            popupFilterList.clear();
            if (!edtSearch.getText().toString().isEmpty()) {
                edtSearch.setText("");
            }
        }
    }

    private void filter() {
        imgFilter.startAnimation(animationController.fadeIn(requireActivity()));
        imgFilter.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clGreenDueDate));

        popupFilterList.filter(headers,  currentResourceView, objectPropertySearch);
    }

    private void search() {
        if (lnSearch.getVisibility() == View.VISIBLE) {
            imgShowSearch.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clBottomDisable));
            lnSearch.setVisibility(View.GONE);
            edtSearch.setText("");
            KeyboardManager.hideKeyboard(edtSearch, requireActivity());
        } else {
            imgShowSearch.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clVer2BlueMain));
            lnSearch.setVisibility(View.VISIBLE);
            edtSearch.requestFocus();
            KeyboardManager.showKeyBoard(edtSearch, requireActivity());
        }
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        BroadcastUtility.unregister(requireActivity(), mReceiver);
    }

    @OnClick({R.id.img_ViewList_Back, R.id.img_ViewList_SubTitle_Next, R.id.img_ViewList_SubTitle_Previous, R.id.img_ViewList_Filter, R.id.img_ViewList_ShowSearch,
            R.id.img_ViewList_Search_Delete})
    public void OnViewClicked(View v) {
        switch (v.getId()) {
            case R.id.img_ViewList_Back: {
                requireActivity().finish();
                break;
            }
            case R.id.img_ViewList_SubTitle_Previous: {
                imgSubPrevious.startAnimation(animationController.fadeIn(requireContext()));
                previous();
                break;
            }
            case R.id.img_ViewList_SubTitle_Next: {
                imgSubNext.startAnimation(animationController.fadeIn(requireContext()));
                next();
                break;
            }
            case R.id.img_ViewList_Filter: {
                filter();
                break;
            }
            case R.id.img_ViewList_ShowSearch: {
                imgShowSearch.startAnimation(animationController.fadeIn(requireContext()));
                search();
                break;
            }
            case R.id.img_ViewList_Search_Delete: {
                edtSearch.setText("");
                break;
            }
        }
    }

    @Override
    public void onRefresh() {
//        if (objectPropertySearch != null) {
//            objectPropertySearch.setLimit(Constants.mFilterLimit - 40);
//            objectPropertySearch.setOffset(0);
//            presenter.getDynamicWorkflowItem(this.headers, objectPropertySearch);
//        }

        apiBPM.updateAllMasterData(false);
    }

    @Override
    public void OnGetDataSuccess(ArrayList<DetailList.Headers> headers, ArrayList<JSONObject> lstJObjectDynamic, ObjectPropertySearch objectPropertySearch) {
        this.headers = headers;
        this.objectPropertySearch = objectPropertySearch;
        this.lstJObjectDynamic = lstJObjectDynamic;

        if ((headers != null && headers.size() > 0) && (lstJObjectDynamic != null && lstJObjectDynamic.size() > 0)) {
            isLoading = lstJObjectDynamic.size() >= Constants.mFilterLimit - 40;
            setListDynamic();
        } else {
            lnNoData2.setVisibility(View.VISIBLE);
            lnCategoryDynamic.setVisibility(View.GONE);
        }

        imgFilter.setEnabled(true);
        imgShowSearch.setEnabled(true);

        lnLoading.setVisibility(View.GONE);
    }

    @Override
    public void OnGetMoreDataSuccess(ArrayList<DetailList.Headers> headers, ArrayList<JSONObject> lstJObjectDynamic, ObjectPropertySearch objectPropertySearch) {
        this.lstJObjectDynamic.remove(this.lstJObjectDynamic.size() - 1);
        adapter.notifyItemRemoved(lstJObjectDynamic.size());

        this.lstJObjectDynamic.addAll(lstJObjectDynamic);
        adapter.notifyDataSetChanged();

        isLoading = lstJObjectDynamic.size() >= Constants.mFilterLimit - 40;
    }

    @Override
    public void OnGetDataErr() {
        lnNoData2.setVisibility(View.VISIBLE);
        lnCategoryDynamic.setVisibility(View.GONE);
        lnLoading.setVisibility(View.GONE);
    }

    @Override
    public void OnDynamicClick(JSONObject object) {
        presenter.handleClicks(requireActivity(), object);
    }

    @Override
    public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {
    }

    @Override
    public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {

    }

    @Override
    public void afterTextChanged(Editable s) {
        if (s.length() > 0) {
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

    private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            String element = intent.getStringExtra("element");
            switch (intent.getAction()) {
                case VarsReceiver.FORMCLICK: {
                    popupFilterList.receiverFormClicks(element);
                    break;
                }
                case VarsReceiver.UPDATEFORM: {
                    String newValue = intent.getStringExtra("newValue");
                    popupFilterList.update(element, newValue);
                    break;
                }
                case VarsReceiver.FOLLOW:
                case VarsReceiver.REFRESHAFTERSUBMITACTION: {
                    onRefresh();
                    break;
                }
            }
        }
    };

    @Override
    public void OnFilterSuccess(ObjectPropertySearch propertySearch) {
        lnLoading.setVisibility(View.VISIBLE);

        isFilter = true;
        this.objectPropertySearch = propertySearch;
        presenter.getDynamicWorkflowItem(this.headers , objectPropertySearch);
    }

    @Override
    public void OnDefaultFilter(ObjectPropertySearch propertySearch) {
        lnLoading.setVisibility(View.VISIBLE);

        this.objectPropertySearch = propertySearch;
        presenter.getDynamicWorkflowItem(this.headers , objectPropertySearch);

        imgFilter.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clBottomDisable));
        isFilter = false;
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
    public void OnFilterErr() {
        isFilter = false;
    }

    @Override
    public void OnRefreshSuccess() {

        setData();

        if (swipe.isRefreshing()) {
            swipe.setRefreshing(false);
        }
    }

    @Override
    public void OnRefreshErr() {
        if (swipe.isRefreshing()) {
            swipe.setRefreshing(false);
        }
    }
}
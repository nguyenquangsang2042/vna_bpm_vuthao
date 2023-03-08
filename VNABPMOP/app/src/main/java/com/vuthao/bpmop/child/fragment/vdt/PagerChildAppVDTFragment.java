package com.vuthao.bpmop.child.fragment.vdt;

import android.annotation.SuppressLint;
import android.content.Intent;
import android.content.res.ColorStateList;
import android.graphics.Typeface;
import android.os.Bundle;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.swiperefreshlayout.widget.SwipeRefreshLayout;
import androidx.viewpager.widget.ViewPager;

import android.os.Handler;
import android.text.Editable;
import android.text.SpannableString;
import android.text.TextWatcher;
import android.text.style.TextAppearanceSpan;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.AnimationController;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.activity.BaseFragment;
import com.vuthao.bpmop.base.api.ApiBPM;
import com.vuthao.bpmop.base.custom.MyCustomViewPager;
import com.vuthao.bpmop.base.keyboard.KeyboardManager;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.Workflow;
import com.vuthao.bpmop.base.model.custom.ObjectPropertySearch;
import com.vuthao.bpmop.base.realm.AppBaseController;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.child.fragment.vdt.adapter.ChildAppVDTPagerAdapter;
import com.vuthao.bpmop.home.presenter.HomePagePresenter;
import com.vuthao.bpmop.shareview.SharedView_PopupFilterVDT;
import com.vuthao.bpmop.vdt.presenter.SingleListVDTPresenter;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;

public class PagerChildAppVDTFragment extends BaseFragment implements SingleListVDTPresenter.PagerSingleListVDTListener, HomePagePresenter.HomePageListener, SwipeRefreshLayout.OnRefreshListener, View.OnScrollChangeListener, ViewPager.OnPageChangeListener, TextWatcher, ApiBPM.ApiBPMRefreshListener {
    @BindView(R.id.pager_ChildViewVDT)
    MyCustomViewPager pager_ViewHomePage;
    @BindView(R.id.ln_ViewHomePage_Toolbar)
    LinearLayout lnToolbar;
    @BindView(R.id.tv_ViewChildAppListWorkflow_Name)
    TextView tvName;
    @BindView(R.id.img_ViewChildAppListWorkflow_Back)
    ImageView imgBack;
    @BindView(R.id.ln_ViewHomePage_Filter)
    LinearLayout lnFilter;
    @BindView(R.id.img_ViewHomePage_Search)
    ImageView imgSearch;
    @BindView(R.id.img_ViewHomePage_Filter)
    ImageView imgFilter;
    @BindView(R.id.ln_ViewListWorkflow_Tab)
    LinearLayout lnTab;
    @BindView(R.id.tv_ViewHomePage_VDT)
    TextView tvVDT;
    @BindView(R.id.tv_ViewHomePage_VTBD)
    TextView tvVTBD;
    @BindView(R.id.ln_ViewListWorkflow_Search)
    LinearLayout lnSearch;
    @BindView(R.id.img_ListWorkflowView_Search_Delete)
    ImageView imgDeleteSearch;
    @BindView(R.id.edt_ListWorkflowView_Search)
    EditText edtSearch;
    @BindView(R.id.swipe_ViewHomePage)
    SwipeRefreshLayout swipeRefreshLayout;

    private View rootView;
    private String type = "inprocess";
    private Workflow workflow;
    private ApiBPM apiBPM;
    private ChildAppVDTFragment childAppSingleListVDTFragment;
    private ChildAppVDTProcessedFragment childAppSingleListVDTProcessedFragment;
    private AppBaseController controller;
    private SharedView_PopupFilterVDT sharedView_popupFilterVDT;
    private SharedView_PopupFilterVDT sharedView_popupFilterProcessedVDT;
    private boolean isInProcessFilter, isProcessFilter;

    public PagerChildAppVDTFragment() {
        // Required empty public constructor
    }

    public PagerChildAppVDTFragment(Workflow workflow) {
        this.workflow = workflow;
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        if (rootView == null) {
            rootView = inflater.inflate(R.layout.fragment_pager_child_app_single_list_v_d_t, container, false);
            ButterKnife.bind(this, rootView);

            init();
            setTitle();
            setupPager();
            setCount();

            swipeRefreshLayout.setOnRefreshListener(this);
            pager_ViewHomePage.setOnScrollChangeListener(this);
            pager_ViewHomePage.addOnPageChangeListener(this);
            edtSearch.addTextChangedListener(this);
        }
        return rootView;
    }

    private void init() {
        apiBPM = new ApiBPM(this);
        controller = new AppBaseController();
        HomePagePresenter homePagePresenter = new HomePagePresenter(this, controller);
        sharedView_popupFilterVDT = new SharedView_PopupFilterVDT(homePagePresenter, this);
        sharedView_popupFilterProcessedVDT = new SharedView_PopupFilterVDT(homePagePresenter, this);
        childAppSingleListVDTProcessedFragment = new ChildAppVDTProcessedFragment(workflow);
        childAppSingleListVDTFragment = new ChildAppVDTFragment(workflow, this);

        swipeRefreshLayout.setEnabled(true);
        Utility.share.setupSwipeRefreshLayout(swipeRefreshLayout);
        pager_ViewHomePage.setVerticalFadingEdgeEnabled(true);
    }

    private void filter() {
        imgFilter.setColorFilter(ContextCompat.getColor(sBaseActivity, R.color.clGreenDueDate));
        if (type.equals("inprocess")) {
            sharedView_popupFilterVDT.filterVDT(requireActivity(), getLayoutInflater(), lnToolbar, imgFilter, "inprocess", workflow.getWorkflowID(), false);
        } else {
            sharedView_popupFilterProcessedVDT.filterVDT(requireActivity(), getLayoutInflater(), lnToolbar, imgFilter, "processed", workflow.getWorkflowID(), false);
        }
    }

    @SuppressLint("SetTextI18n")
    private void setTitle() {
        if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
            tvName.setText(workflow.getTitle());

        } else {
            tvName.setText(workflow.getTitleEN());
        }

        edtSearch.setHint(Functions.share.getTitle("TEXT_SEARCH", "Tìm kiếm"));
        tvVDT.setText(Functions.share.getTitle("TEXT_INPROCESS", "Đang xử lý") + " " + Functions.share.getCountOfNumText(tvVDT.getText().toString()));
        tvVTBD.setText(Functions.share.getTitle("TEXT_PROCESSED", "Đã xử lý"));

        Functions.share.setTVHighligtColor(tvVDT, tvVDT.getText().toString(), "(", ")", 0);
    }

    private void setupPager() {
        ChildAppVDTPagerAdapter adapter = new ChildAppVDTPagerAdapter(getParentFragmentManager(), childAppSingleListVDTFragment, childAppSingleListVDTProcessedFragment);
        pager_ViewHomePage.setSaveEnabled(true);
        pager_ViewHomePage.setAdapter(adapter);
        pager_ViewHomePage.setPagingEnabled(false);
    }

    private void setCount() {
        ArrayList<AppBase> appBases = controller.getVDTItems(0, workflow.getWorkflowID());
        if (appBases.size() > 0) {
            Functions.share.setFormatItemCount(tvVDT, appBases.size(), "vdt", "inprocess");
            if (type.equals("inprocess")) {
                Functions.share.setTVHighligtColor(tvVDT, tvVDT.getText().toString(), "(", ")", 0);
            }
        } else {
            Functions.share.setFormatItemCount(tvVDT, 0, "vdt", "inprocess");
            Functions.share.setTVHighligtColor(tvVDT, tvVDT.getText().toString(), null, null, 0);
        }
    }

    private void search() {
        imgSearch.startAnimation(AnimationController.share.fadeIn(requireActivity()));
        if (lnSearch.getVisibility() == View.GONE) {
            AnimationController.share.slideDown(lnSearch);
            imgSearch.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clVer2BlueMain));
            new Handler().postDelayed(() -> {
                edtSearch.requestFocus();
                KeyboardManager.showKeyBoard(rootView, requireActivity());
                lnTab.setVisibility(View.GONE);
            }, 300);
        } else {
            AnimationController.share.slideUp(lnSearch);
            lnTab.setVisibility(View.VISIBLE);
            KeyboardManager.hideKeyboard(getActivity());
            edtSearch.setText("");
            imgSearch.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clBottomDisable));
        }
    }

    public void scrollToTop() {
        if (type.equals("inprocess")) {
            childAppSingleListVDTFragment.scrollToTop();
        } else {
            childAppSingleListVDTProcessedFragment.scrollToTop();
        }
    }

    @OnClick({R.id.tv_ViewHomePage_VDT, R.id.tv_ViewHomePage_VTBD, R.id.img_ViewChildAppListWorkflow_Back,
            R.id.img_ViewHomePage_Filter, R.id.img_ViewHomePage_Search})
    public void OnViewClicked(View v) {
        switch (v.getId()) {
            case R.id.tv_ViewHomePage_VDT: {
                pager_ViewHomePage.setCurrentItem(0, false);
                break;
            }
            case R.id.tv_ViewHomePage_VTBD: {
                pager_ViewHomePage.setCurrentItem(1, false);
                break;
            }
            case R.id.img_ViewChildAppListWorkflow_Back: {
                requireActivity().finish();
                break;
            }
            case R.id.ln_ViewHomePage_Filter:
            case R.id.img_ViewHomePage_Filter: {
                filter();
                break;
            }
            case R.id.img_ViewHomePage_Search: {
                search();
                break;
            }
        }
    }

    @Override
    public void OnRefreshSuccess() {
        if (swipeRefreshLayout.isRefreshing()) {
            swipeRefreshLayout.setRefreshing(false);
        }

        if (type.equals("inprocess")) {
            childAppSingleListVDTFragment.refresh(isInProcessFilter);
        } else {
            childAppSingleListVDTProcessedFragment.refresh(isProcessFilter);
        }

        if (edtSearch.getText().length() > 0) {
            edtSearch.setText(edtSearch.getText());
        }
    }

    @Override
    public void OnRefreshErr() {
        swipeRefreshLayout.setRefreshing(false);
    }

    @Override
    public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {
    }

    @Override
    public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {

    }

    @Override
    public void afterTextChanged(Editable s) {
        if (type.equals("inprocess")) {
            if (childAppSingleListVDTFragment.getAdapter() != null) {
                childAppSingleListVDTFragment.getAdapter().getFilter().filter(s.toString());
            }
        } else {
            if (childAppSingleListVDTProcessedFragment.getAdapter() != null) {
                childAppSingleListVDTProcessedFragment.getAdapter().getFilter().filter(s.toString());
            }
        }
    }

    @Override
    public void onScrollChange(View v, int scrollX, int scrollY, int oldScrollX, int oldScrollY) {
        swipeRefreshLayout.setEnabled((scrollX <= scrollY) || scrollX >= v.getWidth());
    }

    @Override
    public void onRefresh() {
        apiBPM.updateAllMasterData(false);
    }

    @Override
    public void onPageScrolled(int position, float positionOffset, int positionOffsetPixels) {
        if (position == 0) {
            type = "inprocess";
            Functions.share.setTVSelected(tvVDT);
            Functions.share.setTVUnSelected(tvVTBD);
            if (tvVDT.getText().toString().contains("(")) {
                Functions.share.setTVHighligtColor(tvVDT, tvVDT.getText().toString(), "(", ")", 0);
            } else {
                SpannableString spannable = new SpannableString(tvVDT.getText().toString().trim());
                ColorStateList color = new ColorStateList(new int[][]{new int[]{}}, new int[]{ContextCompat.getColor(sBaseActivity, R.color.clVer2BlueMain)});
                TextAppearanceSpan highlightSpan = new TextAppearanceSpan("", Typeface.NORMAL, -1, color, null);
                spannable.setSpan(highlightSpan, 0, tvVDT.getText().toString().length() - 1, SpannableString.SPAN_EXCLUSIVE_EXCLUSIVE);
                tvVDT.setText(spannable, TextView.BufferType.SPANNABLE);
            }

            if (isProcessFilter) {
                imgFilter.setColorFilter(ContextCompat.getColor(BaseActivity.sBaseActivity, R.color.clGreenDueDate));
            } else {
                imgFilter.setColorFilter(ContextCompat.getColor(BaseActivity.sBaseActivity, R.color.clBottomDisable));
            }
        } else {
            type = "processed";
            Functions.share.setTVSelected(tvVTBD);
            Functions.share.setTVUnSelected(tvVDT);
            Functions.share.setTVHighligtColor(tvVDT, tvVDT.getText().toString(), null, null, 0);

            if (isInProcessFilter) {
                imgFilter.setColorFilter(ContextCompat.getColor(BaseActivity.sBaseActivity, R.color.clGreenDueDate));
            } else {
                imgFilter.setColorFilter(ContextCompat.getColor(BaseActivity.sBaseActivity, R.color.clBottomDisable));
            }
        }
    }

    @Override
    public void onPageSelected(int position) {

    }

    @Override
    public void onPageScrollStateChanged(int state) {

    }

    @Override
    public void OnFilterSuccess(ArrayList<AppBase> apps, String tab, String status, ObjectPropertySearch propertySearch) {
        sBaseActivity.hideProgressDialog();
        apps = controller.modifiedFilters("VDT", apps);
        if (tab.equals("inprocess")) {
            childAppSingleListVDTFragment.setListFilter(apps, tab, propertySearch);
            Functions.share.setFormatItemCount(tvVDT, apps.size(), "vdt", "inprocess");
            if (tvVDT.getText().toString().contains("(")) {
                Functions.share.setTVHighligtColor(tvVDT, tvVDT.getText().toString(), "(", ")", 0);
            }
            isInProcessFilter = true;
        } else {
            childAppSingleListVDTProcessedFragment.setListFilter(apps, tab, propertySearch);
            isProcessFilter = true;
        }

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
        if (type.equals("inprocess")) {
            setCount();
            isInProcessFilter = false;
            childAppSingleListVDTFragment.setDefaultFilter();
        } else {
            isProcessFilter = false;
            childAppSingleListVDTProcessedFragment.setDefaultFilter();
        }

        imgFilter.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clBottomDisable));
    }

    @Override
    public void OnFilterDissmiss() {
        if (!isInProcessFilter) {
            imgFilter.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clBottomDisable));
            sharedView_popupFilterVDT.setDefaultValue();
        } else if (!isProcessFilter) {
            imgFilter.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clBottomDisable));
            sharedView_popupFilterProcessedVDT.setDefaultValue();
        }
    }

    @Override
    protected void onBroadcastReceived(Intent intent) {
        super.onBroadcastReceived(intent);
        switch (intent.getAction()) {
            case VarsReceiver.FOLLOW: {
                String workflowId = intent.getStringExtra("WorkflowId");
                boolean isFollow = intent.getBooleanExtra("isFollow", false);

                if (type.equals("inprocess")) {
                    if (childAppSingleListVDTFragment.getAdapter() != null) {
                        childAppSingleListVDTFragment.getAdapter().updateFollow(workflowId, isFollow);
                    }
                } else {
                    if (childAppSingleListVDTProcessedFragment.getAdapter() != null) {
                        childAppSingleListVDTProcessedFragment.getAdapter().updateFollow(workflowId, isFollow);
                    }
                }

                break;
            }
            case VarsReceiver.REFRESHAFTERSUBMITACTION: {
                OnRefreshSuccess();
                break;
            }
        }
    }

    @Override
    public void OnFilterCount(int count) {
        Functions.share.setFormatItemCount(tvVDT, count, "vdt", "inprocess");
        if (tvVDT.getText().toString().contains("(")) {
            Functions.share.setTVHighligtColor(tvVDT, tvVDT.getText().toString(), "(", ")", 0);
        }
    }
}
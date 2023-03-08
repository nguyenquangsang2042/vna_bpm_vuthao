package com.vuthao.bpmop.vdt;

import android.annotation.SuppressLint;
import android.content.Intent;
import android.content.res.ColorStateList;
import android.graphics.Typeface;
import android.os.Bundle;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.core.content.res.ResourcesCompat;
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
import com.vuthao.bpmop.base.ImageLoader;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.activity.BaseFragment;
import com.vuthao.bpmop.base.api.ApiBPM;
import com.vuthao.bpmop.base.custom.MyCustomViewPager;
import com.vuthao.bpmop.base.keyboard.KeyboardManager;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.custom.ObjectPropertySearch;
import com.vuthao.bpmop.base.realm.AppBaseController;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.home.presenter.HomePagePresenter;
import com.vuthao.bpmop.shareview.SharedView_PopupFilterVDT;
import com.vuthao.bpmop.vdt.adapter.VDTPagerAdapter;
import com.vuthao.bpmop.vdt.presenter.SingleListVDTPresenter;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;
import de.hdodenhof.circleimageview.CircleImageView;

public class PagerSingleListVDTFragment extends BaseFragment implements ApiBPM.ApiBPMRefreshListener, SingleListVDTPresenter.PagerSingleListVDTListener, TextWatcher, HomePagePresenter.HomePageListener, SwipeRefreshLayout.OnRefreshListener, View.OnScrollChangeListener, ViewPager.OnPageChangeListener {
    @BindView(R.id.pager_ViewVDT)
    MyCustomViewPager pager_ViewHomePage;
    @BindView(R.id.swipe_ViewHomePage)
    SwipeRefreshLayout swipe;
    @BindView(R.id.img_ViewHomePage_Avata)
    CircleImageView imgAvata;
    @BindView(R.id.tv_ViewHomePage_VDT)
    TextView tvVDT;
    @BindView(R.id.tv_ViewHomePage_VTBD)
    TextView tvVTBD;
    @BindView(R.id.tv_ViewHomePage_TrangChu)
    TextView tvTrangChu;
    @BindView(R.id.ln_ViewHomePage_Filter)
    LinearLayout lnFilter;
    @BindView(R.id.ln_ViewHomePage_Toolbar)
    LinearLayout lnToolbar;
    @BindView(R.id.ln_ViewHomePage_Content)
    LinearLayout lnContent;
    @BindView(R.id.img_ViewHomePage_Filter)
    ImageView imgFilter;
    @BindView(R.id.img_ViewHomePage_Search)
    ImageView imgSearch;
    @BindView(R.id.ln_ViewListWorkflow_Search)
    LinearLayout lnSearch;
    @BindView(R.id.img_ListWorkflowView_Search_Delete)
    ImageView imgSearchDelete;
    @BindView(R.id.edt_ListWorkflowView_Search)
    EditText edtSearch;
    @BindView(R.id.ln_ViewListWorkflow_Tab)
    LinearLayout lnTab;

    private View rootView;
    private String type = "inprocess";
    private AppBaseController controller;
    private SharedView_PopupFilterVDT sharedView_popupFilterVDT;
    private SharedView_PopupFilterVDT sharedView_popupFilterVDTProcess;
    private SingleListVDTFragment singleListVDTFragment;
    private SingleListVDTProcessedFragment singleListVDTProcessedFragment;
    private boolean isInProcessFilter = false;
    private boolean isProcessFilter = false;
    private ApiBPM apiBPM;

    public PagerSingleListVDTFragment() {
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
            rootView = inflater.inflate(R.layout.fragment_pager_single_list_v_d_t, container, false);
            ButterKnife.bind(this, rootView);

            init();
            setTitle();
            setupPager();
            setCount();

            swipe.setOnRefreshListener(this);
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
        sharedView_popupFilterVDTProcess = new SharedView_PopupFilterVDT(homePagePresenter, this);
        singleListVDTFragment = new SingleListVDTFragment(this);
        singleListVDTProcessedFragment = new SingleListVDTProcessedFragment();

        swipe.setEnabled(true);
        Utility.share.setupSwipeRefreshLayout(swipe);
        pager_ViewHomePage.setVerticalFadingEdgeEnabled(true);

        ImageLoader.getInstance().loadImageUserWithToken(requireActivity(), Constants.BASE_URL + CurrentUser.getInstance().getUser().getImagePath(), imgAvata);
    }

    @SuppressLint("SetTextI18n")
    private void setTitle() {
        tvVDT.setText(Functions.share.getTitle("TEXT_INPROCESS", "Đang xử lý") + " " + Functions.share.getCountOfNumText(tvVDT.getText().toString()));
        tvVTBD.setText(Functions.share.getTitle("TEXT_PROCESSED", "Đã xử lý"));
        tvTrangChu.setText(Functions.share.getTitle("TEXT_TOME", "Đến tôi"));
        edtSearch.setHint(Functions.share.getTitle("TEXT_SEARCH", "Tìm kiếm"));

        if (type.equals("inprocess")) {
            Functions.share.setTVHighligtColor(tvVDT, tvVDT.getText().toString(), "(", ")", 0);
        }
    }

    private void setCount() {
        ArrayList<AppBase> appBases = controller.getVDTItems(0, 0);
        if (appBases.size() > 0) {
            Functions.share.setFormatItemCount(tvVDT, appBases.size(), "vdt", "inprocess");
            if (type.equals("inprocess")) {
                Functions.share.setTVHighligtColor(tvVDT, tvVDT.getText().toString(), "(", ")", 0);
            }
        } else {
            Functions.share.setFormatItemCount(tvVDT, 0, "vdt", "");
            Functions.share.setTVHighligtColor(tvVDT, tvVDT.getText().toString(), null, null, 0);
        }
    }

    private void setupPager() {
        VDTPagerAdapter adapter = new VDTPagerAdapter(getParentFragmentManager(), singleListVDTFragment, singleListVDTProcessedFragment);
        pager_ViewHomePage.setSaveEnabled(true);
        pager_ViewHomePage.setAdapter(adapter);
        pager_ViewHomePage.setPagingEnabled(false);
    }

    private void filter() {
        imgFilter.setColorFilter(ContextCompat.getColor(sBaseActivity, R.color.clGreenDueDate));
        if (type.equals("inprocess")) {
            sharedView_popupFilterVDT.filterVDT(requireActivity(), getLayoutInflater(), lnToolbar, imgFilter, "inprocess", 0, false);
        } else {
            sharedView_popupFilterVDTProcess.filterVDT(requireActivity(), getLayoutInflater(), lnToolbar, imgFilter, "processed", 0, false);
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
            imgSearch.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clBottomDisable));
            AnimationController.share.slideUp(lnSearch);
            lnTab.setVisibility(View.VISIBLE);
            KeyboardManager.hideKeyboard(requireActivity());
            edtSearch.setText("");
        }
    }

    public void scrollToTop() {
        if (type.equals("inprocess")) {
            singleListVDTFragment.scrollToTop();
        } else {
            singleListVDTProcessedFragment.scrollToTop();
        }
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

            if (isInProcessFilter) {
                imgFilter.setColorFilter(ContextCompat.getColor(BaseActivity.sBaseActivity, R.color.clGreenDueDate));
            } else {
                imgFilter.setColorFilter(ContextCompat.getColor(BaseActivity.sBaseActivity, R.color.clBottomDisable));
            }
        } else {
            type = "processed";
            Functions.share.setTVSelected(tvVTBD);
            Functions.share.setTVUnSelected(tvVDT);
            Functions.share.setTVHighligtColor(tvVDT, tvVDT.getText().toString(), null, null, 0);

            if (isProcessFilter) {
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
    public void onScrollChange(View v, int scrollX, int scrollY, int oldScrollX, int oldScrollY) {
        swipe.setEnabled((scrollX <= scrollY) || scrollX >= v.getWidth());
    }

    @OnClick({R.id.tv_ViewHomePage_VDT, R.id.tv_ViewHomePage_TrangChu, R.id.tv_ViewHomePage_VTBD,
            R.id.img_ViewHomePage_Avata, R.id.img_ViewHomePage_Filter, R.id.img_ViewHomePage_Search,
            R.id.img_ListWorkflowView_Search_Delete})
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
            case R.id.tv_ViewHomePage_TrangChu:
            case R.id.img_ViewHomePage_Avata: {
                sBaseActivity.openLeftMenu();
                break;
            }
            case R.id.img_ViewHomePage_Filter: {
                filter();
                break;
            }
            case R.id.img_ViewHomePage_Search: {
                search();
                break;
            }
            case R.id.img_ListWorkflowView_Search_Delete: {
                edtSearch.setText("");
                break;
            }
        }
    }

    @Override
    public void onRefresh() {
        apiBPM.updateAllMasterData(false);
    }

    @Override
    public void OnFilterSuccess(ArrayList<AppBase> apps, String tab, String status, ObjectPropertySearch propertySearch) {
        sBaseActivity.hideProgressDialog();
        apps = controller.modifiedFilters("VDT", apps);
        if (tab.equals("inprocess")) {
            singleListVDTFragment.setListFilter(apps, tab, propertySearch);
            Functions.share.setFormatItemCount(tvVDT, apps.size(), "vdt", "inprocess");
            if (tvVDT.getText().toString().contains("(")) {
                Functions.share.setTVHighligtColor(tvVDT, tvVDT.getText().toString(), "(", ")", 0);
            }
            isInProcessFilter = true;
        } else {
            singleListVDTProcessedFragment.setListFilter(apps, tab, propertySearch);
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
            singleListVDTFragment.setDefaultFilter();
        } else {
            isProcessFilter = false;
            singleListVDTProcessedFragment.setDefaultFilter();
        }

        imgFilter.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clBottomDisable));
    }

    @Override
    public void OnFilterDissmiss() {
        if (type.equals("inprocess")) {
            if (!isInProcessFilter) {
                imgFilter.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clBottomDisable));
                sharedView_popupFilterVDT.setDefaultValue();
            }
        } else {
            if (!isProcessFilter) {
                imgFilter.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clBottomDisable));
                sharedView_popupFilterVDTProcess.setDefaultValue();
            }
        }
    }

    @Override
    public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {

    }

    @Override
    public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {

    }

    @Override
    public void afterTextChanged(Editable s) {
        if (s.toString().length() > 0) {
            edtSearch.setTypeface(ResourcesCompat.getFont(requireActivity(), R.font.fontarial), Typeface.NORMAL);
            imgSearchDelete.setVisibility(View.VISIBLE);
        } else {
            edtSearch.setTypeface(ResourcesCompat.getFont(requireActivity(), R.font.fontarial), Typeface.ITALIC);
            imgSearchDelete.setVisibility(View.GONE);
        }

        if (type.equals("inprocess")) {
            singleListVDTFragment.filterSearch(s.toString());
        } else {
            singleListVDTProcessedFragment.filterSearch(s.toString());
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
                    if (singleListVDTFragment.getAdapter() != null) {
                        singleListVDTFragment.getAdapter().updateFollow(workflowId, isFollow);
                    }
                } else {
                    if (singleListVDTProcessedFragment.getAdapter() != null) {
                        singleListVDTProcessedFragment.getAdapter().updateFollow(workflowId, isFollow);
                    }
                }

                break;
            }
            case VarsReceiver.CHANGELANGUAGE: {
                setTitle();
                singleListVDTFragment.setTextLanguageChanges();
                if (singleListVDTFragment.getAdapter() != null) {
                    singleListVDTFragment.getAdapter().notifyDataSetChanged();
                }
                sharedView_popupFilterVDTProcess.changeLanguage();
                sharedView_popupFilterVDT.changeLanguage();
                singleListVDTProcessedFragment.setTextLanguageChanges();
                if (singleListVDTProcessedFragment.getAdapter() != null) {
                    singleListVDTProcessedFragment.getAdapter().notifyDataSetChanged();
                }
                break;
            }
            case VarsReceiver.PUSHNOTIFICATION:
            case VarsReceiver.REFRESHNAVIGATIONBOTTOM:
            case VarsReceiver.CREATE_TASK:
            case VarsReceiver.REFRESHAFTERSUBMITACTION: {
                OnRefreshSuccess();
                break;
            }
        }
    }

    @Override
    public void OnRefreshSuccess() {
        if (swipe.isRefreshing()) {
            swipe.setRefreshing(false);
        }

        setTitle();

        if (type.equals("inprocess")) {
            singleListVDTFragment.refresh(isInProcessFilter);
        } else {
            singleListVDTProcessedFragment.refresh(isProcessFilter);
        }

        // vẽ lại trạng thái search
        if (edtSearch.getText().length() > 0) {
            edtSearch.setText(edtSearch.getText());
        }
    }

    @Override
    public void OnRefreshErr() {
        if (swipe.isRefreshing()) {
            swipe.setRefreshing(false);
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


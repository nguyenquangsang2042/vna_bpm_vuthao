package com.vuthao.bpmop.home;

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
import com.vuthao.bpmop.base.model.custom.ObjectPropertySearch;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.realm.AppBaseController;
import com.vuthao.bpmop.follow.presenter.FollowPresenter;
import com.vuthao.bpmop.home.adapter.HomePageViewPagerAdapter;
import com.vuthao.bpmop.home.presenter.HomePagePresenter;
import com.vuthao.bpmop.leftmenu.presenter.LeftMenuPresenter;
import com.vuthao.bpmop.qrcode.QRCodeActivity;
import com.vuthao.bpmop.shareview.SharedView_PopupFilterVDT;
import com.vuthao.bpmop.shareview.SharedView_PopupFilterVTBD;
import com.vuthao.bpmop.task.DetailCreateTaskActivity;
import com.vuthao.bpmop.vdt.presenter.SingleListVDTPresenter;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;
import de.hdodenhof.circleimageview.CircleImageView;

public class HomePageFragment extends BaseFragment implements TextWatcher, SingleListVDTPresenter.PagerSingleListVDTListener, ApiBPM.ApiBPMRefreshListener, HomePagePresenter.HomePageListener, SwipeRefreshLayout.OnRefreshListener, ViewPager.OnPageChangeListener, View.OnScrollChangeListener {
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
    @BindView(R.id.pager_ViewHomePage)
    MyCustomViewPager pager_ViewHomePage;
    @BindView(R.id.img_ViewHomePage_Filter)
    ImageView imgFilter;
    @BindView(R.id.img_ViewHomePage_QRCode)
    ImageView imgQRCode;
    @BindView(R.id.swipe_ViewHomePage)
    SwipeRefreshLayout swipeViewHomePage;
    @BindView(R.id.img_ViewHomePage_Search)
    ImageView imgSearch;
    @BindView(R.id.ln_ViewListWorkflow_Tab)
    LinearLayout lnTab;
    @BindView(R.id.ln_ViewListWorkflow_Search)
    LinearLayout lnSearch;
    @BindView(R.id.img_ListWorkflowView_Search_Delete)
    ImageView imgDelete;
    @BindView(R.id.edt_ListWorkflowView_Search)
    EditText edtSearch;

    private View rootView;
    private AppBaseController appBaseController;
    private ArrayList<AppBase> follows;
    private LeftMenuPresenter leftMenuPresenter;
    private LayoutInflater inflater;
    public String type = "VDT";
    private boolean isFilterVDT = false;
    private boolean isFilterVTBD = false;
    private SharedView_PopupFilterVDT sharedView_popupFilterVDT;
    private SharedView_PopupFilterVTBD sharedView_popupFilterVTBD;
    private HomePageVDTFragment homePageVDTFragment;
    private HomePageVTBDFragment homePageVTBDFragment;
    private int filterCount;
    private HomePagePresenter presenter;
    private ApiBPM apiBPM;

    public HomePageFragment() {

    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        this.inflater = inflater;
        if (rootView == null) {
            rootView = inflater.inflate(R.layout.fragment_home_page, container, false);
            ButterKnife.bind(this, rootView);

            init();
            setTitle();
            setupPager();
            setCount();
            setCountFollow();

            swipeViewHomePage.setOnRefreshListener(this);
            pager_ViewHomePage.setOnScrollChangeListener(this);
            pager_ViewHomePage.addOnPageChangeListener(this);
            edtSearch.addTextChangedListener(this);
        }
        return rootView;
    }

    private void setupPager() {
        HomePageViewPagerAdapter homePageViewPagerAdapter = new HomePageViewPagerAdapter(getParentFragmentManager(), homePageVDTFragment, homePageVTBDFragment);
        pager_ViewHomePage.setSaveEnabled(true);
        pager_ViewHomePage.setAdapter(homePageViewPagerAdapter);
        pager_ViewHomePage.setPagingEnabled(false);
    }

    private void init() {
        follows = new ArrayList<>();
        apiBPM = new ApiBPM(this);
        appBaseController = new AppBaseController();
        presenter = new HomePagePresenter(this, appBaseController);
        leftMenuPresenter = new LeftMenuPresenter(sBaseActivity.getObjLeftMenu(), sBaseActivity.getObjLeftMenu());
        sharedView_popupFilterVDT = new SharedView_PopupFilterVDT(presenter, this, this);
        sharedView_popupFilterVTBD = new SharedView_PopupFilterVTBD(presenter, this, this);
        homePageVDTFragment = new HomePageVDTFragment(this);
        homePageVTBDFragment = new HomePageVTBDFragment();
        swipeViewHomePage.setEnabled(true);
        Utility.share.setupSwipeRefreshLayout(swipeViewHomePage);
        pager_ViewHomePage.setVerticalFadingEdgeEnabled(true);

        ImageLoader.getInstance().loadImageUserWithToken(requireActivity(), Constants.BASE_URL + CurrentUser.getInstance().getUser().getImagePath(), imgAvata);
    }

    public void scrollToTop() {
        if (type.equals("VDT")) {
            homePageVDTFragment.scrollToTop();
        } else {
            homePageVTBDFragment.scrollToTop();
        }
    }

    @SuppressLint("SetTextI18n")
    private void setTitle() {
        tvVDT.setText(Functions.share.getTitle("TEXT_TOME", "Đến tôi") + " " + Functions.share.getCountOfNumText(tvVDT.getText().toString()));
        tvVTBD.setText(Functions.share.getTitle("TEXT_FROMME", "Tôi bắt đầu"));
        tvTrangChu.setText(Functions.share.getTitle("TEXT_MAINVIEW", "Trang chủ"));

        if (type.equals("VDT")) {
            Functions.share.setTVHighligtColor(tvVDT, tvVDT.getText().toString(), "(", ")", 0);
        }
    }

    private void setCount() {
        if (type.equals("VDT")) {
            int count = appBaseController.getVDTItems(0, 0).size();
            if (count > 0) {
                Functions.share.setFormatItemCount(tvVDT, count, "vdt", "");
                Functions.share.setTVHighligtColor(tvVDT, tvVDT.getText().toString(), null, null, 0);
                if (tvVDT.getText().toString().contains("(") && type.equals("VDT")) {
                    Functions.share.setTVHighligtColor(tvVDT, tvVDT.getText().toString(), "(", ")", 0);
                }
                leftMenuPresenter.setCountVDT(appBaseController.getVDTItems(0, 0).size());
            } else {
                Functions.share.setFormatItemCount(tvVDT, 0, "vdt", "");
                Functions.share.setTVHighligtColor(tvVDT, tvVDT.getText().toString(), "(", ")", 0);
            }
        }

        int countVDT = appBaseController.getVTBDCounts(0);
        if (countVDT > 0) {
            leftMenuPresenter.setCountVTBD(countVDT);
        } else {
            leftMenuPresenter.setCountVTBD(0);
        }
    }

    private void setCountFollow() {
        follows = new FollowPresenter().getListFollow();
        if (!follows.isEmpty()) {
            leftMenuPresenter.setCountFollow(follows.size());
        } else {
            leftMenuPresenter.setCountFollow(0);
        }
    }

    private void search() {
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
            imgSearch.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clBottomDisable));
            edtSearch.setText("");
        }
    }

    @OnClick({R.id.tv_ViewHomePage_VDT, R.id.tv_ViewHomePage_VTBD, R.id.img_ViewHomePage_QRCode,
            R.id.img_ViewHomePage_Avata, R.id.img_ViewHomePage_Filter,
            R.id.tv_ViewHomePage_TrangChu, R.id.img_ListWorkflowView_Search_Delete,
            R.id.img_ViewHomePage_Search})
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
                imgFilter.startAnimation(AnimationController.share.fadeIn(getActivity()));
                imgFilter.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clGreenDueDate));

                if (type.equals("VDT")) {
                    sharedView_popupFilterVDT.filterVDT(requireActivity(), inflater, lnToolbar, imgFilter, "VDT", 0, true);
                } else {
                    sharedView_popupFilterVTBD.filter(requireActivity(), lnToolbar, imgFilter, 0);
                }
                break;
            }
            case R.id.img_ViewHomePage_QRCode: {
                imgQRCode.startAnimation(AnimationController.share.fadeIn(requireContext()));
                Intent intent = new Intent(requireActivity(), QRCodeActivity.class);
                requireActivity().startActivity(intent);
                break;
            }
            case R.id.img_ViewHomePage_Search: {
                imgSearch.startAnimation(new AnimationController().fadeIn(requireActivity()));
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
        if (type.equals("VDT")) {
            if (isFilterVDT) {
                presenter.refreshFilters(type, homePageVDTFragment.getPropertySearch());
            } else {
                apiBPM.updateAllMasterData(false);
            }
        } else {
            if (isFilterVTBD) {
                presenter.refreshFilters(type, homePageVTBDFragment.getPropertySearch());
            } else {
                apiBPM.updateAllMasterData(false);
            }
        }
    }

    @Override
    public void onScrollChange(View v, int scrollX, int scrollY, int oldScrollX, int oldScrollY) {
        swipeViewHomePage.setEnabled((scrollX <= scrollY) || scrollX >= v.getWidth());
    }

    @Override
    public void onPageScrolled(int position, float positionOffset, int positionOffsetPixels) {
        if (position == 0) {
            type = "VDT";
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

            if (isFilterVDT) {
                imgFilter.setColorFilter(ContextCompat.getColor(BaseActivity.sBaseActivity, R.color.clGreenDueDate));
            } else {
                imgFilter.setColorFilter(ContextCompat.getColor(BaseActivity.sBaseActivity, R.color.clBottomDisable));
            }
        } else {
            type = "VTBD";
            Functions.share.setTVSelected(tvVTBD);
            Functions.share.setTVUnSelected(tvVDT);

            Functions.share.setTVHighligtColor(tvVDT, tvVDT.getText().toString(), null, null, 0);

            if (isFilterVTBD) {
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
        if (swipeViewHomePage.isRefreshing()) {
            swipeViewHomePage.setRefreshing(false);
        }

        sBaseActivity.hideProgressDialog();
        apps = appBaseController.modifiedFilters(tab, apps);
        filterCount = apps.size();
        if (tab.equals("VDT")) {
            homePageVDTFragment.setListFilter(apps, tab, status, propertySearch);
            Functions.share.setFormatItemCount(tvVDT, apps.size(), "vdt", "");
            if (tvVDT.getText().toString().contains("(")) {
                Functions.share.setTVHighligtColor(tvVDT, tvVDT.getText().toString(), "(", ")", 0);
            }
            leftMenuPresenter.setCountVDT(apps.size());
            isFilterVDT = true;
        } else {
            homePageVTBDFragment.setListFilter(apps, tab, propertySearch);
            isFilterVTBD = true;
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
        if (type.equals("VDT")) {
            homePageVDTFragment.setDefaultFilter();
            setCount();
            isFilterVDT = false;
        } else {
            homePageVTBDFragment.setDefaultFilter(isFilterVTBD);
            isFilterVTBD = false;
        }

        imgFilter.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clBottomDisable));
    }

    @Override
    public void OnFilterDissmiss() {
        if (type.equals("VDT")) {
            if (!isFilterVDT) {
                imgFilter.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clBottomDisable));
                sharedView_popupFilterVDT.setDefaultValue();
            }
        } else {
            if (!isFilterVTBD) {
                imgFilter.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clBottomDisable));
                sharedView_popupFilterVTBD.setDefaultValue();
            }
        }
    }

    @Override
    protected void onBroadcastReceived(Intent intent) {
        super.onBroadcastReceived(intent);
        switch (intent.getAction()) {
            case VarsReceiver.FOLLOW: {
                String workflowId = intent.getStringExtra("WorkflowId");
                boolean isFollow = intent.getBooleanExtra("isFollow", false);

                if (type.equals("VDT")) {
                    homePageVDTFragment.getAdapter().updateFollow(workflowId, isFollow);
                } else {
                    homePageVTBDFragment.getAdapter().updateFollow(workflowId, isFollow);
                }

                setCountFollow();
                break;
            }
            case VarsReceiver.CHANGELANGUAGE: {
                setTitle();
                homePageVDTFragment.setTextLanguageChanges();
                if (homePageVDTFragment.getAdapter() != null) {
                    homePageVDTFragment.getAdapter().notifyDataSetChanged();
                }

                sharedView_popupFilterVTBD.changeLanguage();
                sharedView_popupFilterVDT.changeLanguage();
                homePageVTBDFragment.setTextLanguageChanges();
                if (homePageVTBDFragment.getAdapter() != null) {
                    homePageVTBDFragment.getAdapter().notifyDataSetChanged();
                }
                break;
            }
            case VarsReceiver.PUSHNOTIFICATION:
            case VarsReceiver.REFRESHNAVIGATIONBOTTOM:
            case VarsReceiver.CREATE_TASK:
            case VarsReceiver.REFRESHAFTERSUBMITACTION: {
                onRefresh();
                break;
            }
        }
    }

    @Override
    public void OnRefreshSuccess() {
        if (swipeViewHomePage.isRefreshing()) {
            swipeViewHomePage.setRefreshing(false);
        }

        if (type.equals("VDT")) {
            homePageVDTFragment.refresh(isFilterVDT);
        } else {
            homePageVTBDFragment.refresh(isFilterVTBD);
        }

        setCountFollow();
    }

    @Override
    public void OnRefreshErr() {
        if (swipeViewHomePage.isRefreshing()) {
            swipeViewHomePage.setRefreshing(false);
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
            imgDelete.setVisibility(View.VISIBLE);
            edtSearch.setTypeface(ResourcesCompat.getFont(requireActivity(), R.font.fontarial), Typeface.NORMAL);
        } else {
            imgDelete.setVisibility(View.GONE);
            edtSearch.setTypeface(ResourcesCompat.getFont(requireActivity(), R.font.fontarial), Typeface.ITALIC);
        }

        if (type.equals("VDT") && homePageVDTFragment.getAdapter() != null) {
            homePageVDTFragment.getAdapter().getFilter().filter(s.toString());
        } else if (type.equals("VTBD") && homePageVTBDFragment.getAdapter() != null){
            homePageVTBDFragment.getAdapter().getFilter().filter(s.toString());
        }
    }

    @Override
    public void OnFilterCount(int count) {
        Functions.share.setFormatItemCount(tvVDT, count, "vdt", "");
        if (tvVDT.getText().toString().contains("(")) {
            Functions.share.setTVHighligtColor(tvVDT, tvVDT.getText().toString(), "(", ")", 0);
        }
    }
}
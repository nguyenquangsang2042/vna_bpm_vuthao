package com.vuthao.bpmop.activity;

import androidx.annotation.Nullable;
import androidx.core.content.ContextCompat;
import androidx.drawerlayout.widget.DrawerLayout;
import androidx.fragment.app.FragmentTransaction;

import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.res.Resources;
import android.os.Bundle;
import android.util.DisplayMetrics;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import com.google.android.material.floatingactionbutton.FloatingActionButton;
import com.google.android.material.tabs.TabLayout;
import com.google.android.material.tabs.TabLayoutMediator;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.activity.presenter.TabPresenter;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.RefreshCookie;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.realm.RealmController;
import com.vuthao.bpmop.base.vars.Variable;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.board.BoardFragment;
import com.vuthao.bpmop.detail.DetailWorkflowActivity;
import com.vuthao.bpmop.follow.FollowFragment;
import com.vuthao.bpmop.home.HomePageFragment;
import com.vuthao.bpmop.leftmenu.LeftMenuFragment;
import com.vuthao.bpmop.home.adapter.MyViewPagerAdapter;
import com.vuthao.bpmop.leftmenu.presenter.LeftMenuPresenter;
import com.vuthao.bpmop.search.SearchFragment;
import com.vuthao.bpmop.task.DetailCreateTaskActivity;
import com.vuthao.bpmop.vdt.PagerSingleListVDTFragment;
import com.vuthao.bpmop.vtbd.SingleListVTBDFragment;
import com.vuthao.bpmop.workflow.ListWorkflowActivity;

import java.util.Objects;

public class TabsActivity extends BaseActivity implements TabPresenter.TabSelectedListener, View.OnClickListener, RefreshCookie.RefreshCookieListener, TabLayout.OnTabSelectedListener {
    private LeftMenuPresenter presenter;
    private int lastPage = 0;
    private final int[] imageResId = {
            R.drawable.icon_ver2_home,
            R.drawable.icon_ver4_tome,
            R.drawable.icon_ver4_fromme,
            R.drawable.icon_ver2_star_unchecked,
            R.drawable.icon_ver3_app
    };

    private final String[] titles = new String[]{"Home", "Search", "Application"};
    private final String[] titlesVN = new String[]{"Trang chủ", "Tra cứu", "Ứng dụng"};

    private HomePageFragment homePageFragment;
    private PagerSingleListVDTFragment pagerSingleListVDTFragment;
    private SingleListVTBDFragment singleListVTBDFragment;
    private BoardFragment boardFragment;
    private FollowFragment followFragment;
    private RefreshCookie refreshCookie;
    private SearchFragment searchFragment;

    private LinearLayout lnHome;
    private LinearLayout lnSearch;
    private LinearLayout lnApp;
    private ImageView imgHome;
    private ImageView imgSearch;
    private ImageView imgApp;
    private TextView tvHome;
    private TextView tvSearch;
    private TextView tvApp;

    private boolean owner;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_tabs);

        setTabLayout(findViewById(R.id.tab_layout));
        setViewPager(findViewById(R.id.viewPager));
        lnHome = findViewById(R.id.lnHome);
        lnSearch = findViewById(R.id.lnSearch);
        lnApp = findViewById(R.id.lnApp);
        imgHome = findViewById(R.id.imgHome);
        imgSearch = findViewById(R.id.imgSearch);
        imgApp = findViewById(R.id.imgApp);
        tvHome = findViewById(R.id.tvHome);
        tvSearch = findViewById(R.id.tvSearch);
        tvApp = findViewById(R.id.tvApp);

        FloatingActionButton fabWorkflow = findViewById(R.id.fabCreateWorkflow);

        Constants.mResourceIdFromMe = Functions.share.getAppSettings("MOBILE_RESOURCEVIEWID_FROMME");
        Constants.mResourceIdToMe = Functions.share.getAppSettings("MOBILE_RESOURCEVIEWID_TOME");

        init();
        initLeftMenu();
        initView();
        getNotification();

        BroadcastUtility.register(this, mReceiver, VarsReceiver.CHANGELANGUAGE);

        //getTabLayout().addOnTabSelectedListener(this);
        fabWorkflow.setOnClickListener(this);
        lnHome.setOnClickListener(this);
        lnSearch.setOnClickListener(this);
        lnApp.setOnClickListener(this);
    }

    private void init() {
        homePageFragment = new HomePageFragment();
        pagerSingleListVDTFragment = new PagerSingleListVDTFragment();
        singleListVTBDFragment = new SingleListVTBDFragment();
        boardFragment = new BoardFragment();
        followFragment = new FollowFragment();
        searchFragment = new SearchFragment();

        new RealmController().getRealm().executeTransaction(realm -> {
              realm.copyToRealmOrUpdate(CurrentUser.getInstance().getUser());
        });

//        //Neu la autologin thi kiem tra xem Cookie co con xai` dc ko
//        if (Constants.isAutoLogin) {
//            refreshCookie = new RefreshCookie(this);
//            refreshCookie.refreshCookie(getPreferencesController().getDeviceInfo(), "1");
//        } else {
//            // Cap nhat lai CurrentUser trong table User vi User tra ve ko du thong tin nhu CurrentUser
//            new RealmController().getRealm().executeTransaction(realm -> {
//                realm.copyToRealmOrUpdate(CurrentUser.getInstance().getUser());
//            });
//        }
    }

    private void initLeftMenu() {
        setmDrawerLayout(findViewById(R.id.drawerlayout_ActivityMain_Content));
        getmDrawerLayout().setDrawerLockMode(DrawerLayout.LOCK_MODE_UNLOCKED);
        setmNavigationView(findViewById(R.id.navigation_ActivityMain_leftmenu));
        DisplayMetrics mMetrics = Resources.getSystem().getDisplayMetrics();
        int sizeLeft = Integer.parseInt(String.valueOf(((mMetrics.widthPixels * 3) / 4)));
        getmNavigationView().setMinimumWidth(sizeLeft);
        FragmentTransaction fragmentTx;
        fragmentTx = getSupportFragmentManager().beginTransaction();
        setObjLeftMenu(new LeftMenuFragment(this));
        presenter = new LeftMenuPresenter(sBaseActivity.getObjLeftMenu());
        fragmentTx.replace(R.id.navigation_ActivityMain_leftmenu, getObjLeftMenu(), "LeftMenuFragment");
        fragmentTx.commit();
    }

    private void getNotification() {
        if (!Functions.isNullOrEmpty(Constants.mResourceId)) {
            AppBase appBase = new RealmController().getRealm().where(AppBase.class)
                    .equalTo("ID", Integer.parseInt(Constants.mResourceId))
                    .findFirst();
            if (appBase != null) {
                if (appBase.getResourceCategoryId() == 16) {
                    Intent intent = new Intent(this, DetailCreateTaskActivity.class);
                    intent.putExtra("WorkflowItemId", Functions.share.getWorkflowItemIDByUrl(appBase.getItemUrl()));
                    intent.putExtra("taskId", appBase.getID());
                    startActivity(intent);
                } else {
                    Intent i = new Intent(this, DetailWorkflowActivity.class);
                    i.putExtra("WorkflowItemId", Constants.mResourceId);
                    startActivity(i);
                }
            }

            Constants.mResourceId = "";
        }
    }

    public void initView() {
        MyViewPagerAdapter adapter = new MyViewPagerAdapter(this, homePageFragment, pagerSingleListVDTFragment, singleListVTBDFragment
                , followFragment, boardFragment, searchFragment);
        getViewPager().setAdapter(adapter);
        getViewPager().setOffscreenPageLimit(adapter.getItemCount());

        /*new TabLayoutMediator(getTabLayout(), getViewPager(), (tab, position) -> {
        }).attach();

        for (int i = 0; i < 3; i++) {
            LinearLayout tab = (LinearLayout) LayoutInflater.from(this).inflate(R.layout.customtab, null);

            TextView mLabel = tab.findViewById(R.id.nav_label);
            ImageView mIcon = tab.findViewById(R.id.nav_icon);

            if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
                mLabel.setText(titlesVN[i]);
            } else {
                mLabel.setText(titles[i]);
            }

            if (i == 0) {
                mLabel.setTextColor(ContextCompat.getColor(this, R.color.clVer2BlueMain));
                mIcon.setImageResource(imageResId[i]);
                mIcon.setColorFilter(ContextCompat.getColor(this, R.color.clVer2BlueMain));
            } else {
                mIcon.setImageResource(imageResId[i]);
                mIcon.setColorFilter(ContextCompat.getColor(this, R.color.clBottomDisable));
            }

            Objects.requireNonNull(getTabLayout().getTabAt(i)).setCustomView(tab);
        }*/
    }

    @Override
    public void onBackPressed() {
        if (getFragmentManager() != null) {
            getSupportFragmentManager().popBackStack();
        }
    }

    @Override
    protected void onResume() {
        super.onResume();
        if (CurrentUser.getInstance().getUser().getID().isEmpty()) {
            hideBottomBar();
        } else {
            showBottomBar();
        }
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        BroadcastUtility.unregister(this, mReceiver);
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, @Nullable Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        if (resultCode == Activity.RESULT_OK) {
            assert data != null;
            boolean backfromTab = data.getBooleanExtra("backfromtab", false);
            if (backfromTab) {
                getViewPager().setCurrentItem(0, false);
            }
        }
    }

    @Override
    public void onTabSelected(TabLayout.Tab tab) {
        /*switch (tab.getPosition()) {
            case 0: {
                if (calculateTab(lastPage, tab.getPosition())) {
                    getViewPager().setCurrentItem(0, false);
                }

                lastPage = tab.getPosition();
                presenter.onBottomNavigationClick(Variable.BottomNavigation.HomePage);
                break;
            }
            case 1: {
                if (calculateTab(lastPage, tab.getPosition())) {
                    getViewPager().setCurrentItem(1, false);
                }

                lastPage = tab.getPosition();
                presenter.onBottomNavigationClick(Variable.BottomNavigation.SingleListVDT);
                break;
            }
            case 2: {
                if (calculateTab(lastPage, tab.getPosition())) {
                    getViewPager().setCurrentItem(2, false);
                }

                lastPage = tab.getPosition();
                presenter.onBottomNavigationClick(Variable.BottomNavigation.SingleListVTBD);
                break;
            }
            case 3: {
                if (calculateTab(lastPage, tab.getPosition())) {
                    getViewPager().setCurrentItem(3, false);
                }

                lastPage = tab.getPosition();
                presenter.onBottomNavigationClick(Variable.BottomNavigation.Follow);
                break;
            }
            case 4: {
                if (calculateTab(lastPage, tab.getPosition())) {
                    getViewPager().setCurrentItem(4, false);
                }

                lastPage = tab.getPosition();
                presenter.onBottomNavigationClick(Variable.BottomNavigation.Board);
                break;
            }
        }

        View tabView = tab.getCustomView();

        assert tabView != null;
        TextView mTablabel = tabView.findViewById(R.id.nav_label);
        ImageView mTabicon = tabView.findViewById(R.id.nav_icon);

        mTablabel.setTextColor(ContextCompat.getColor(this, R.color.clVer2BlueMain));
        mTabicon.setColorFilter(ContextCompat.getColor(this, R.color.clVer2BlueMain));*/
    }

    @Override
    public void onTabUnselected(TabLayout.Tab tab) {
        /*View tabView = tab.getCustomView();
        assert tabView != null;
        TextView mLabel = tabView.findViewById(R.id.nav_label);
        ImageView mIcon = tabView.findViewById(R.id.nav_icon);

        mLabel.setTextColor(ContextCompat.getColor(this, R.color.clBlack));
        mIcon.setColorFilter(ContextCompat.getColor(this, R.color.clBottomDisable));*/
    }

    @Override
    public void onTabReselected(TabLayout.Tab tab) {
       /* switch (tab.getPosition()) {
            case 0: {
                homePageFragment.scrollToTop();
                break;
            }
            case 1: {
                pagerSingleListVDTFragment.scrollToTop();
                break;
            }
            case 2: {
                singleListVTBDFragment.scrollToTop();
                break;
            }
            case 3: {
                followFragment.scrollToTop();
                break;
            }
            case 4: {
                boardFragment.scrollToTop();
                break;
            }
        }*/
    }

    private boolean calculateTab(int lastPage, int directPage) {
        if (lastPage > directPage && (lastPage - directPage) == 1) {
            return false;
        } else {
            return directPage - lastPage != 1;
        }
    }

    private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
                tvHome.setText(titlesVN[0]);
                tvSearch.setText(titlesVN[1]);
                tvApp.setText(titlesVN[2]);
            } else {
                tvHome.setText(titles[0]);
                tvSearch.setText(titles[1]);
                tvApp.setText(titles[2]);
            }

            /*for (int i = 0; i < getTabLayout().getTabCount(); i++) {
                View tabView = Objects.requireNonNull(getTabLayout().getTabAt(i)).getCustomView();
                assert tabView != null;
                TextView mLabel = tabView.findViewById(R.id.nav_label);
                if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
                    mLabel.setText(titlesVN[i]);
                } else {
                    mLabel.setText(titles[i]);
                }
            }*/
        }
    };

    @Override
    public void OnRefreshCookieErr(String err) {
        Utility.share.showAlertWithOnlyOK(err, "Đóng", this, () -> {
            refreshCookie.refresh(this);
        });
    }

    public void tabReselected(int pos) {
        switch (pos) {
            case 0: {
                homePageFragment.scrollToTop();
                break;
            }
            case 1: {
                pagerSingleListVDTFragment.scrollToTop();
                break;
            }
            case 2: {
                singleListVTBDFragment.scrollToTop();
                break;
            }
            case 3: {
                followFragment.scrollToTop();
                break;
            }
            case 4: {
                searchFragment.scrollToTop();
                break;
            }
            case 5: {
                boardFragment.scrollToTop();
                break;
            }
        }
    }

    @Override
    public void onClick(View view) {
        switch (view.getId()) {
            case R.id.fabCreateWorkflow: {
                Intent intent = new Intent(this, ListWorkflowActivity.class);
                startActivity(intent);
                break;
            }
            case R.id.lnHome: {
                if (getViewPager().getCurrentItem() == lastPage) {
                    tabReselected(lastPage);
                } else {
                    getViewPager().setCurrentItem(lastPage, false);
                    OnTabSelected(0, lastPage);
                }

                presenter.onBottomNavigationClick(lastPage);
                break;
            }
            case R.id.lnSearch: {
                if (getViewPager().getCurrentItem() == 4) {
                    tabReselected(4);
                } else {
                    getViewPager().setCurrentItem(4, false);
                    OnTabSelected(1, -1);
                }
                presenter.onBottomNavigationClick(Variable.BottomNavigation.Search);
                break;
            }
            case R.id.lnApp: {
                if (getViewPager().getCurrentItem() == 5) {
                    tabReselected(5);
                } else {
                    getViewPager().setCurrentItem(5, false);
                    OnTabSelected(2, -1);
                }

                presenter.onBottomNavigationClick(Variable.BottomNavigation.Board);
                break;
            }
        }
    }

    @Override
    public void OnTabSelected(int position, int lastPage) {
        switch (position) {
            case 0: {
                this.lastPage = lastPage;
                tvHome.setTextColor(ContextCompat.getColor(this, R.color.clVer2BlueMain));
                tvApp.setTextColor(ContextCompat.getColor(this, R.color.clBottomDisable));
                tvSearch.setTextColor(ContextCompat.getColor(this, R.color.clBottomDisable));

                imgHome.setColorFilter(ContextCompat.getColor(this, R.color.clVer2BlueMain));
                imgApp.setColorFilter(ContextCompat.getColor(this, R.color.clBottomDisable));
                imgSearch.setColorFilter(ContextCompat.getColor(this, R.color.clBottomDisable));
                break;
            }
            case 1: {
                tvSearch.setTextColor(ContextCompat.getColor(this, R.color.clVer2BlueMain));
                tvHome.setTextColor(ContextCompat.getColor(this, R.color.clBottomDisable));
                tvApp.setTextColor(ContextCompat.getColor(this, R.color.clBottomDisable));

                imgSearch.setColorFilter(ContextCompat.getColor(this, R.color.clVer2BlueMain));
                imgHome.setColorFilter(ContextCompat.getColor(this, R.color.clBottomDisable));
                imgApp.setColorFilter(ContextCompat.getColor(this, R.color.clBottomDisable));
                break;
            }
            case 2: {
                tvApp.setTextColor(ContextCompat.getColor(this, R.color.clVer2BlueMain));
                tvHome.setTextColor(ContextCompat.getColor(this, R.color.clBottomDisable));
                tvSearch.setTextColor(ContextCompat.getColor(this, R.color.clBottomDisable));

                imgApp.setColorFilter(ContextCompat.getColor(this, R.color.clVer2BlueMain));
                imgHome.setColorFilter(ContextCompat.getColor(this, R.color.clBottomDisable));
                imgSearch.setColorFilter(ContextCompat.getColor(this, R.color.clBottomDisable));
                break;
            }
        }
    }

    @Override
    public void OnTabReselected(int position) {

    }
}
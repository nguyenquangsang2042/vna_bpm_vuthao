package com.vuthao.bpmop.child.activity;

import androidx.core.content.ContextCompat;
import androidx.viewpager2.widget.ViewPager2;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import com.google.android.material.tabs.TabLayout;
import com.google.android.material.tabs.TabLayoutMediator;
import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.Workflow;
import com.vuthao.bpmop.child.activity.adapter.MyChildViewPagerAdapter;
import com.vuthao.bpmop.child.fragment.board.ChildAppKanbanFragment;
import com.vuthao.bpmop.child.fragment.list.ChildAppListFragment;
import com.vuthao.bpmop.child.fragment.report.ChildAppReportFragment;
import com.vuthao.bpmop.child.fragment.vdt.PagerChildAppVDTFragment;
import com.vuthao.bpmop.child.fragment.vtbd.ChildAppSingleListVTBDFragment;

import java.util.Objects;

import butterknife.BindView;
import butterknife.ButterKnife;

public class ChildTabsActivity extends BaseActivity implements TabLayout.OnTabSelectedListener {
    @BindView(R.id.viewPager)
    ViewPager2 viewPager;
    @BindView(R.id.tab_layoutChild)
    TabLayout tabLayout;

    private int lastPage = 0;
    private ChildAppSingleListVTBDFragment childAppSingleListVTBDFragment;
    private PagerChildAppVDTFragment pagerSingleListVDTFragment;
    private ChildAppKanbanFragment childAppKanbanFragment;
    private ChildAppReportFragment childAppReportFragment;
    private ChildAppListFragment childAppListFragment;
    private boolean isVN;
    private final String BACK = "backfromtab";

    int[] imageResId = {
            /*R.drawable.icon_ver2_home,*/
            R.drawable.icon_ver4_tome,
            R.drawable.icon_ver4_fromme,
            R.drawable.icon_ver2_board2,
            R.drawable.icon_ver2_list,
            R.drawable.icon_ver2_report
    };

    String[] titles = new String[]{
            /*"Home",*/
            "To me",
            "From me",
            "Board",
            "List",
            "Report"
    };

    String[] titlesVN = new String[]{
            /*"Trang chủ",*/
            "Đến tôi",
            "Tôi bắt đầu",
            "Bảng",
            "Danh sách",
            "Báo cáo"
    };

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_child_tabs);
        ButterKnife.bind(this);

        init();
        setup();
        tabLayout.addOnTabSelectedListener(this);
        /*viewPager.registerOnPageChangeCallback(new ViewPager2.OnPageChangeCallback() {
            @Override
            public void onPageSelected(int position) {
                if  (position == 0) {
                    Intent resultIntent = new Intent();
                    resultIntent.putExtra(BACK, true);
                    setResult(Activity.RESULT_OK, resultIntent);
                    finish();
                }
            }
        });*/
    }

    public void init() {
        Bundle extras = getIntent().getExtras();
        String value = extras.getString("workflow");
        Workflow workflow = new Gson().fromJson(value, Workflow.class);
        isVN = CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN);

        childAppSingleListVTBDFragment = new ChildAppSingleListVTBDFragment(workflow);
        pagerSingleListVDTFragment = new PagerChildAppVDTFragment(workflow);
        childAppReportFragment = new ChildAppReportFragment(workflow);
        childAppListFragment = new ChildAppListFragment(workflow);
        childAppKanbanFragment = new ChildAppKanbanFragment(workflow);
    }

    private void setup() {
        MyChildViewPagerAdapter adapter = new MyChildViewPagerAdapter(this, childAppSingleListVTBDFragment, pagerSingleListVDTFragment, childAppKanbanFragment, childAppReportFragment, childAppListFragment);
        viewPager.setAdapter(adapter);
        //viewPager.setOffscreenPageLimit(adapter.getItemCount());
        viewPager.setUserInputEnabled(false);
        new TabLayoutMediator(tabLayout, viewPager, (tab, position) -> {
        }).attach();

        for (int i = 0; i < tabLayout.getTabCount(); i++) {
            LinearLayout tab = (LinearLayout) LayoutInflater.from(this).inflate(R.layout.customtab, null);

            TextView mLabel = tab.findViewById(R.id.nav_label);
            ImageView mIcon = tab.findViewById(R.id.nav_icon);

            if (isVN) {
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
                mIcon.setColorFilter(ContextCompat.getColor(this, R.color.clGrayIcon));
            }

            Objects.requireNonNull(tabLayout.getTabAt(i)).setCustomView(tab);
        }
    }

    @Override
    public void onTabSelected(TabLayout.Tab tab) {
        switch (tab.getPosition()) {
            case 0: {
                if (calculateTab(lastPage, tab.getPosition())) {
                    viewPager.setCurrentItem(0, false);
                }

                lastPage = tab.getPosition();
                break;
            }
            case 1: {
                if (calculateTab(lastPage, tab.getPosition())) {
                    viewPager.setCurrentItem(1, false);
                }

                lastPage = tab.getPosition();
                break;
            }
            case 2: {
                if (calculateTab(lastPage, tab.getPosition())) {
                    viewPager.setCurrentItem(2, false);
                }

                lastPage = tab.getPosition();
                break;
            }
            case 3: {
                if (calculateTab(lastPage, tab.getPosition())) {
                    viewPager.setCurrentItem(3, false);
                }

                lastPage = tab.getPosition();
                break;
            }
            case 4: {
                if (calculateTab(lastPage, tab.getPosition())) {
                    viewPager.setCurrentItem(4, false);
                }

                lastPage = tab.getPosition();
                break;
            }

        }

        View tabView = tab.getCustomView();

        assert tabView != null;
        TextView mLabel = tabView.findViewById(R.id.nav_label);
        ImageView mIcon = tabView.findViewById(R.id.nav_icon);

        mLabel.setTextColor(ContextCompat.getColor(this, R.color.clVer2BlueMain));
        mIcon.setColorFilter(ContextCompat.getColor(this, R.color.clVer2BlueMain));
    }

    @Override
    public void onTabUnselected(TabLayout.Tab tab) {
        View tabView = tab.getCustomView();
        assert tabView != null;
        TextView mLabel = tabView.findViewById(R.id.nav_label);
        ImageView mIcon = tabView.findViewById(R.id.nav_icon);

        // Back to the black color
        mLabel.setTextColor(ContextCompat.getColor(this, R.color.clGrayIcon));
        mIcon.setColorFilter(ContextCompat.getColor(this, R.color.clGrayIcon));
    }

    @Override
    public void onTabReselected(TabLayout.Tab tab) {
        switch (tab.getPosition()) {
            case 0: {
                pagerSingleListVDTFragment.scrollToTop();
                break;
            }
            case 1: {
                childAppSingleListVTBDFragment.scrollToTop();
                break;
            }
        }
    }

    private boolean calculateTab(int lastPage, int directPage) {
        if (lastPage > directPage && (lastPage - directPage) == 1) {
            return false;
        } else {
            return directPage - lastPage != 1;
        }
    }
}
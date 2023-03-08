package com.vuthao.bpmop.home.adapter;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentManager;
import androidx.fragment.app.FragmentStatePagerAdapter;

import com.vuthao.bpmop.home.HomePageVDTFragment;
import com.vuthao.bpmop.home.HomePageVTBDFragment;
import com.vuthao.bpmop.home.presenter.HomePagePresenter;

public class HomePageViewPagerAdapter extends FragmentStatePagerAdapter  {
    private HomePageVDTFragment homePageVDTFragment;
    private HomePageVTBDFragment homePageVTBDFragment;

    public HomePageViewPagerAdapter(@NonNull FragmentManager fm, HomePageVDTFragment homePageVDTFragment, HomePageVTBDFragment homePageVTBDFragment) {
        super(fm, FragmentStatePagerAdapter.BEHAVIOR_RESUME_ONLY_CURRENT_FRAGMENT);
        this.homePageVDTFragment = homePageVDTFragment;
        this.homePageVTBDFragment = homePageVTBDFragment;
    }

    @Override
    public int getItemPosition(@NonNull Object object) {
        return super.getItemPosition(object);
    }

    @NonNull
    @Override
    public Fragment getItem(int position) {
        if (position == 0) {
            return homePageVDTFragment;
        } else {
            return homePageVTBDFragment;
        }
    }

    @Override
    public int getCount() {
        return 2;
    }
}

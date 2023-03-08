package com.vuthao.bpmop.home.adapter;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentActivity;
import androidx.viewpager2.adapter.FragmentStateAdapter;

import com.vuthao.bpmop.board.BoardFragment;
import com.vuthao.bpmop.follow.FollowFragment;
import com.vuthao.bpmop.home.HomePageFragment;
import com.vuthao.bpmop.search.SearchFragment;
import com.vuthao.bpmop.vdt.PagerSingleListVDTFragment;
import com.vuthao.bpmop.vdt.SingleListVDTFragment;
import com.vuthao.bpmop.vtbd.SingleListVTBDFragment;

public class MyViewPagerAdapter extends FragmentStateAdapter {
    private final HomePageFragment homePageFragment;
    private final PagerSingleListVDTFragment pagerSingleListVDTFragment;
    private final SingleListVTBDFragment singleListVTBDFragment;
    private final BoardFragment boardFragment;
    private final FollowFragment followFragment;
    private final SearchFragment searchFragment;

    public MyViewPagerAdapter(@NonNull FragmentActivity fragmentActivity, HomePageFragment homePageFragment,
                              PagerSingleListVDTFragment pagerSingleListVDTFragment,
                              SingleListVTBDFragment singleListVTBDFragment, FollowFragment followFragment,
                              BoardFragment boardFragment, SearchFragment searchFragment) {
        super(fragmentActivity);
        this.homePageFragment = homePageFragment;
        this.pagerSingleListVDTFragment = pagerSingleListVDTFragment;
        this.singleListVTBDFragment = singleListVTBDFragment;
        this.followFragment = followFragment;
        this.boardFragment = boardFragment;
        this.searchFragment = searchFragment;
    }

    @NonNull
    @Override
    public Fragment createFragment(int position) {
        switch (position) {
            case 0: {
                return homePageFragment;
            }
            case 1: {
                return pagerSingleListVDTFragment;
            }
            case 2: {
                return singleListVTBDFragment;
            }
            case 3: {
                return followFragment;
            }
            case 4: {
                return searchFragment;
            }
            case 5: {
                return boardFragment;
            }
        }
        return null;
    }

    @Override
    public int getItemCount() {
        return 6;
    }
}

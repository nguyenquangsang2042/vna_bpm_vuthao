package com.vuthao.bpmop.vdt.adapter;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentManager;
import androidx.fragment.app.FragmentStatePagerAdapter;

import com.vuthao.bpmop.vdt.PagerSingleListVDTFragment;
import com.vuthao.bpmop.vdt.SingleListVDTFragment;
import com.vuthao.bpmop.vdt.SingleListVDTProcessedFragment;
import com.vuthao.bpmop.vdt.presenter.SingleListVDTPresenter;

public class VDTPagerAdapter extends FragmentStatePagerAdapter {
    private final SingleListVDTFragment singleListVDTFragment;
    private final SingleListVDTProcessedFragment singleListVDTProcessedFragment;

    public VDTPagerAdapter(@NonNull FragmentManager fm, SingleListVDTFragment singleListVDTFragment, SingleListVDTProcessedFragment singleListVDTProcessedFragment) {
        super(fm, FragmentStatePagerAdapter.BEHAVIOR_RESUME_ONLY_CURRENT_FRAGMENT);
        this.singleListVDTFragment = singleListVDTFragment;
        this.singleListVDTProcessedFragment = singleListVDTProcessedFragment;
    }

    @Override
    public int getItemPosition(@NonNull Object object) {
        return super.getItemPosition(object);
    }

    @NonNull
    @Override
    public Fragment getItem(int position) {
        if (position == 0) {
            return singleListVDTFragment;
        } else {
            return singleListVDTProcessedFragment;
        }
    }

    @Override
    public int getCount() {
        return 2;
    }
}

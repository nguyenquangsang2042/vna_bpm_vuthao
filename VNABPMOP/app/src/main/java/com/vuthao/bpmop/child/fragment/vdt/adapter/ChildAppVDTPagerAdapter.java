package com.vuthao.bpmop.child.fragment.vdt.adapter;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentManager;
import androidx.fragment.app.FragmentStatePagerAdapter;

import com.vuthao.bpmop.child.fragment.vdt.ChildAppVDTFragment;
import com.vuthao.bpmop.child.fragment.vdt.ChildAppVDTProcessedFragment;

public class ChildAppVDTPagerAdapter extends FragmentStatePagerAdapter {
    private final ChildAppVDTFragment childAppSingleListVDTFragment;
    private final ChildAppVDTProcessedFragment childAppSingleListVDTProcessedFragment;

    public ChildAppVDTPagerAdapter(@NonNull FragmentManager fm, ChildAppVDTFragment childAppSingleListVDTFragment, ChildAppVDTProcessedFragment childAppSingleListVDTProcessedFragment) {
        super(fm, FragmentStatePagerAdapter.BEHAVIOR_RESUME_ONLY_CURRENT_FRAGMENT);
        this.childAppSingleListVDTFragment = childAppSingleListVDTFragment;
        this.childAppSingleListVDTProcessedFragment = childAppSingleListVDTProcessedFragment;
    }

    @Override
    public int getItemPosition(@NonNull Object object) {
        return super.getItemPosition(object);
    }

    @NonNull
    @Override
    public Fragment getItem(int position) {
        if (position == 0) {
            return childAppSingleListVDTFragment;
        } else {
            return childAppSingleListVDTProcessedFragment;
        }
    }

    @Override
    public int getCount() {
        return 2;
    }
}

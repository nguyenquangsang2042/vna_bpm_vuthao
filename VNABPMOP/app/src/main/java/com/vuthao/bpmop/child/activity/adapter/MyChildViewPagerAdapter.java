package com.vuthao.bpmop.child.activity.adapter;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentActivity;
import androidx.viewpager2.adapter.FragmentStateAdapter;

import com.vuthao.bpmop.child.fragment.board.ChildAppKanbanFragment;
import com.vuthao.bpmop.child.fragment.list.ChildAppListFragment;
import com.vuthao.bpmop.child.fragment.report.ChildAppReportFragment;
import com.vuthao.bpmop.child.fragment.vdt.PagerChildAppVDTFragment;
import com.vuthao.bpmop.child.fragment.vtbd.ChildAppSingleListVTBDFragment;

public class MyChildViewPagerAdapter extends FragmentStateAdapter {
    private ChildAppSingleListVTBDFragment childAppSingleListVTBDFragment;
    private PagerChildAppVDTFragment childAppSingleListVDTFragment;
    private ChildAppKanbanFragment childAppKanbanFragment;
    private ChildAppReportFragment childAppReportFragment;
    private ChildAppListFragment childAppListFragment;

    public MyChildViewPagerAdapter(@NonNull FragmentActivity fragmentActivity,
                                   ChildAppSingleListVTBDFragment childAppSingleListVTBDFragment,
                                   PagerChildAppVDTFragment childAppSingleListVDTFragment,
                                   ChildAppKanbanFragment childAppKanbanFragment,
                                   ChildAppReportFragment childAppReportFragment,
                                   ChildAppListFragment childAppListFragment) {
        super(fragmentActivity);
        this.childAppSingleListVDTFragment = childAppSingleListVDTFragment;
        this.childAppSingleListVTBDFragment = childAppSingleListVTBDFragment;
        this.childAppListFragment = childAppListFragment;
        this.childAppReportFragment = childAppReportFragment;
        this.childAppKanbanFragment = childAppKanbanFragment;
    }


    @NonNull
    @Override
    public Fragment createFragment(int position) {
        switch (position) {
            /*case 0: {
                return new EmptyFragment();
            }
            case 1: {
                return childAppSingleListVDTFragment;
            }
            case 2: {
                return childAppSingleListVTBDFragment;
            }
            case 3: {
                return childAppReportFragment;
            }
            case 4: {
                return childAppListFragment;
            }
            case 5: {
                return childAppKanbanFragment;
            }
            default:
                return null;*/

            case 0: {
                return childAppSingleListVDTFragment;
            }
            case 1: {
                return childAppSingleListVTBDFragment;
            }
            case 2: {
                return childAppKanbanFragment;
            }
            case 3: {
                return childAppListFragment;
            }
            case 4: {
                return childAppReportFragment;
            }
        }

        return null;
    }

    @Override
    public int getItemCount() {
        return 5;
    }
}

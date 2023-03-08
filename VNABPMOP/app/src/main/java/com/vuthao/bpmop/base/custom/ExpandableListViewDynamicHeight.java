package com.vuthao.bpmop.base.custom;

import android.content.Context;
import android.util.AttributeSet;
import android.widget.ExpandableListView;

public class ExpandableListViewDynamicHeight extends ExpandableListView {
    public ExpandableListViewDynamicHeight(Context context) {
        super(context);
    }

    public ExpandableListViewDynamicHeight(Context context, AttributeSet attrs) {
        super(context, attrs);
    }

    public ExpandableListViewDynamicHeight(Context context, AttributeSet attrs, int defStyleAttr) {
        super(context, attrs, defStyleAttr);
    }

    public ExpandableListViewDynamicHeight(Context context, AttributeSet attrs, int defStyleAttr, int defStyleRes) {
        super(context, attrs, defStyleAttr, defStyleRes);
    }

    @Override
    public boolean collapseGroup(int groupPos) {
        return super.collapseGroup(groupPos);
    }

    @Override
    public boolean expandGroup(int groupPos) {
        return super.expandGroup(groupPos);
    }

    @Override
    public void setOnGroupExpandListener(OnGroupExpandListener onGroupExpandListener) {
        super.setOnGroupExpandListener(onGroupExpandListener);
    }

    @Override
    public void setOnGroupCollapseListener(OnGroupCollapseListener onGroupCollapseListener) {
        super.setOnGroupCollapseListener(onGroupCollapseListener);
    }
}

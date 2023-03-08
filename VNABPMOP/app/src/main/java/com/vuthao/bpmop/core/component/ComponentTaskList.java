package com.vuthao.bpmop.core.component;

import android.app.Activity;
import android.content.Context;
import android.graphics.Typeface;
import android.util.TypedValue;
import android.view.View;
import android.widget.ExpandableListView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.core.content.ContextCompat;
import androidx.core.content.res.ResourcesCompat;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.AnimationController;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.custom.ExpandTask;
import com.vuthao.bpmop.base.model.custom.Task;
import com.vuthao.bpmop.core.adapter.ExpandComponentTaskListAdapter;
import com.vuthao.bpmop.detail.custom.DetailFunc;

import java.util.ArrayList;

public class ComponentTaskList extends ComponentBase {
    private final Activity mainAct;
    private Context context;
    private LinearLayout lnExpandable;
    private LinearLayout lnTitle;
    private TextView tvTitle;
    private ExpandableListView expandTaskList;
    private final ArrayList<Task> lstTask;
    // Nếu là view DetailCreateTask sẽ ẩn Title
    private final boolean isDetailCreateTaskView;

    public ComponentTaskList(Activity mainAct, Context context, LinearLayout parentView, ArrayList<Task> lstTask, boolean isDetailCreateTaskView) {
        this.mainAct = mainAct;
        this.context = context;
        this.lstTask = lstTask;
        this.isDetailCreateTaskView = isDetailCreateTaskView;
        initializeComponent();
    }

    @Override
    public void initializeComponent() {
        super.initializeComponent();

        lnExpandable = new LinearLayout(mainAct);
        expandTaskList = new ExpandableListView(mainAct);

        lnTitle = new LinearLayout(mainAct);
        tvTitle = new TextView(mainAct);

        tvTitle.setText(Functions.share.getTitle("TEXT_TASKLIST", "Danh sách công việc"));
        tvTitle.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.BOLD);
        tvTitle.setTextColor(ContextCompat.getColor(context, R.color.clBlack));
        tvTitle.setTextSize(TypedValue.COMPLEX_UNIT_SP, 12);

        expandTaskList.setGroupIndicator(null);
        expandTaskList.setChildIndicator(null);
        expandTaskList.setDividerHeight(0);
        expandTaskList.setNestedScrollingEnabled(true);

        lnTitle.setOrientation(LinearLayout.VERTICAL);
        lnExpandable.setOrientation(LinearLayout.VERTICAL);
    }

    @Override
    public void initializeFrameView(LinearLayout frame) {
        context = frame.getContext();
        super.initializeFrameView(frame);

        int padding = (int) TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, 3, mainAct.getResources().getDisplayMetrics());
        LinearLayout.LayoutParams paramsTitle = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WRAP_CONTENT, LinearLayout.LayoutParams.WRAP_CONTENT);
        LinearLayout.LayoutParams paramsLine = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, (int)TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_PX, 1, mainAct.getResources().getDisplayMetrics()));
        LinearLayout.LayoutParams paramsRecy = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.MATCH_PARENT);

        paramsLine.setMargins(0, 3 * padding, 0, 3 * padding);
        paramsRecy.setMargins(0, 0, 0, 2 * padding);

        tvTitle.setLayoutParams(paramsTitle);
        expandTaskList.setLayoutParams(paramsRecy);

        frame.setPadding(2 * padding, 2 * padding, 2 * padding, 0);
        tvTitle.setPadding(0, 3 * padding, padding, 6 * padding);

        lnTitle.addView(tvTitle);

        expandTaskList.setNestedScrollingEnabled(true);

        // View chi tiết Task ko có Title
        if (!isDetailCreateTaskView) {
            frame.addView(lnTitle);
        }

        frame.addView(lnExpandable);
    }

    @Override
    public void setTitle() {
        super.setTitle();
    }

    @Override
    public void setValue() {
        super.setValue();

        if (lstTask != null && lstTask.size() > 0) {
            int itemHeight = Functions.share.convertDpToPixel(100, context);
            ArrayList<ExpandTask> lstExpandTaskClone = DetailFunc.share.cloneListExpandTask(lstTask);

            if (lstExpandTaskClone != null && lstExpandTaskClone.size() > 0) {
                ExpandComponentTaskListAdapter adapterComponentTaskList = new ExpandComponentTaskListAdapter(mainAct, context, lnExpandable, lstExpandTaskClone);
                expandTaskList.setAdapter(adapterComponentTaskList);

                for (int i = 0; i < adapterComponentTaskList.getGroupCount(); i++) {
                    expandTaskList.expandGroup(i);
                }

                expandTaskList.setOnGroupClickListener((parent, v, groupPosition, id) -> {
                    int totalChildItemCount = lstExpandTaskClone.get(groupPosition).getLstChild().size();
                    for (ExpandTask task : lstExpandTaskClone.get(groupPosition).getLstChild()) {
                        totalChildItemCount += task.getLstChild().size();
                    }

                    if (expandTaskList.isGroupExpanded(groupPosition)) {
                        expandTaskList.collapseGroup(groupPosition);
                        lnExpandable.getLayoutParams().height -= itemHeight * totalChildItemCount;
                    } else {
                        expandTaskList.expandGroup(groupPosition);
                        lnExpandable.getLayoutParams().height += itemHeight * totalChildItemCount;
                    }
                    return true;
                });

                lnExpandable.removeAllViews();
                lnExpandable.addView(expandTaskList);
                lnExpandable.startAnimation(AnimationController.share.fadeIn(context));
                lnExpandable.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, (itemHeight * lstTask.size()) + 20));
            }
        }
    }
}

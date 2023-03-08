package com.vuthao.bpmop.core.component;

import android.app.Activity;
import android.content.Context;
import android.graphics.Typeface;
import android.text.TextUtils;
import android.util.TypedValue;
import android.view.Gravity;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.cardview.widget.CardView;
import androidx.core.content.ContextCompat;
import androidx.core.content.res.ResourcesCompat;
import androidx.recyclerview.widget.RecyclerView;
import androidx.recyclerview.widget.StaggeredGridLayoutManager;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.AnimationController;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.app.Notify;
import com.vuthao.bpmop.base.model.custom.WorkFlowRelated;
import com.vuthao.bpmop.base.model.app.WorkflowItem;
import com.vuthao.bpmop.core.adapter.ComponentFlowRelatedAdapter;

import java.util.ArrayList;

public class ComponentFlowRelated extends ComponentBase {
    private final Activity mainAct;
    private Context context;
    private CardView cardViewRecycler;
    private LinearLayout lnTitle;
    private TextView tvTitle;
    private RecyclerView recyAttachment;

    private final ArrayList<WorkFlowRelated> lstWorkflowItem;
    private final WorkflowItem currentWorkflowItem;
    private final Notify currentNotifyItem;

    public ComponentFlowRelated(Activity mainAct, Context context, LinearLayout parentView, ArrayList<WorkFlowRelated> lstWorkflowItem, WorkflowItem currentWorkflowItem, Notify currentNotifyItem) {
        this.mainAct = mainAct;
        this.context = context;
        this.lstWorkflowItem = lstWorkflowItem;
        this.currentWorkflowItem = currentWorkflowItem;
        this.currentNotifyItem = currentNotifyItem;
        initializeComponent();
    }

    @Override
    public void initializeComponent() {
        super.initializeComponent();

        recyAttachment = new RecyclerView(mainAct);
        cardViewRecycler = new CardView(mainAct);
        lnTitle = new LinearLayout(mainAct);
        tvTitle = new TextView(mainAct);

        tvTitle.setText(Functions.share.getTitle("TEXT_WORKFLOW_RELATE", "Quy trình / Công việc liên kết"));
        tvTitle.setTextSize(TypedValue.COMPLEX_UNIT_SP, 12);
        tvTitle.setTextColor(ContextCompat.getColor(context, R.color.clBlack));
        tvTitle.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.BOLD);

        tvTitle.setEllipsize(TextUtils.TruncateAt.END);
        tvTitle.setGravity(Gravity.CENTER);

        cardViewRecycler.setUseCompatPadding(true);
        cardViewRecycler.setRadius(5f);

        recyAttachment.setNestedScrollingEnabled(true);
        lnTitle.setOrientation(LinearLayout.VERTICAL);
    }

    @Override
    public void initializeFrameView(LinearLayout frame) {
        context = frame.getContext();
        super.initializeFrameView(frame);

        int padding = (int) TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, 3, mainAct.getResources().getDisplayMetrics());
        LinearLayout.LayoutParams paramsTitle = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WRAP_CONTENT, LinearLayout.LayoutParams.WRAP_CONTENT);
        LinearLayout.LayoutParams paramsLnRecy = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.MATCH_PARENT);

        tvTitle.setLayoutParams(paramsTitle);

        frame.setPadding(2 * padding, 2 * padding, 2 * padding, 0);
        tvTitle.setPadding(padding, 2 * padding, padding, 4 * padding);
        recyAttachment.setLayoutParams(paramsLnRecy);

        lnTitle.addView(tvTitle);
        frame.addView(lnTitle);
        frame.addView(cardViewRecycler);
    }

    @Override
    public void setTitle() {
        super.setTitle();
    }

    @Override
    public void setValue() {
        super.setValue();

        if (lstWorkflowItem != null && lstWorkflowItem.size() > 0) {
            cardViewRecycler.removeAllViews();
            cardViewRecycler.addView(recyAttachment);
            ComponentFlowRelatedAdapter adapterFlowRelated = new ComponentFlowRelatedAdapter(mainAct, context, lstWorkflowItem, currentWorkflowItem, currentNotifyItem);
            recyAttachment.setLayoutManager(new StaggeredGridLayoutManager(1, LinearLayout.VERTICAL));
            recyAttachment.setAdapter(adapterFlowRelated);
            cardViewRecycler.startAnimation(AnimationController.share.fadeIn(context));
        }
    }
}

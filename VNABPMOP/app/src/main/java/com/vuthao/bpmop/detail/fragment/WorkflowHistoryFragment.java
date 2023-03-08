package com.vuthao.bpmop.detail.fragment;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ExpandableListView;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.annotation.NonNull;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.AnimationController;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.activity.BaseFragment;
import com.vuthao.bpmop.base.api.ApiWorkflowController;
import com.vuthao.bpmop.base.model.app.Notify;
import com.vuthao.bpmop.base.model.app.WorkflowItem;
import com.vuthao.bpmop.base.model.custom.WorkflowHistory;
import com.vuthao.bpmop.detail.adapter.ExpandDetailProcessAdapter;
import com.vuthao.bpmop.detail.presenter.DetailWorkflowPresenter;

import java.util.ArrayList;
import java.util.Date;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;

public class WorkflowHistoryFragment extends BaseFragment {
    @BindView(R.id.ln_ViewDetailProcess_All)
    LinearLayout lnAll;
    @BindView(R.id.img_ViewDetailProcess_Back)
    ImageView imgBack;
    @BindView(R.id.tv_ViewDetailProcess_Name)
    TextView tvTitle;
    @BindView(R.id.tv_ViewDetailProcess_WorkflowName)
    TextView tvWorkflowTitle;
    @BindView(R.id.ln_ViewDetailProcess_Process)
    LinearLayout lnProcess;
    @BindView(R.id.ln_ViewDetailProcess_NoData)
    LinearLayout lnNoData;
    @BindView(R.id.expand_ViewDetailProcess_Process)
    ExpandableListView expandProcess;

    private View rootView;
    private ArrayList<WorkflowHistory> workflowHistories;
    private WorkflowItem workflowItem;
    private Notify notifyItem;
    private AnimationController animationController;

    public WorkflowHistoryFragment() { }

    public WorkflowHistoryFragment(ArrayList<WorkflowHistory> workflowHistories, WorkflowItem _workflowItem, Notify _notifyItem) {
        this.workflowHistories = workflowHistories;
        this.workflowItem = _workflowItem;
        this.notifyItem = _notifyItem;
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        if (rootView == null) {
            rootView = inflater.inflate(R.layout.fragment_detail_workflow_history, container, false);
            ButterKnife.bind(this, rootView);

            init();
            setData();
        }

        return rootView;
    }

    private void init() {
        animationController = new AnimationController();
        expandProcess.setGroupIndicator(null);
        expandProcess.setChildIndicator(null);
        expandProcess.setDividerHeight(0);

        tvWorkflowTitle.setText(workflowItem.getContent());
        tvTitle.setText(Functions.share.getTitle("TEXT_WORKFLOW_HISTORY", "Luồng phê duyệt"));
    }

    private void setData() {
        if (workflowHistories.size() > 0) {
            workflowHistories.sort((o1, o2) -> {
                Date t1Val = Functions.share.formatStringToDate(o1.getCreated());
                Date t2Val = Functions.share.formatStringToDate(o2.getCreated());
                return (t1Val.getTime() > t2Val.getTime() ? 1 : -1);
            });

            ExpandDetailProcessAdapter expandDetailProcess = new ExpandDetailProcessAdapter(rootView.getContext(), workflowHistories);
            expandProcess.setAdapter(expandDetailProcess);
            for (int i = 0; i < expandDetailProcess.getGroupCount(); i++) {
                expandProcess.expandGroup(i);
            }

            lnProcess.startAnimation(animationController.fadeIn(rootView.getContext()));
            lnProcess.setVisibility(View.VISIBLE);
            lnNoData.setVisibility(View.GONE);
        } else {
            lnProcess.setVisibility(View.GONE);
            lnNoData.setVisibility(View.VISIBLE);
        }
    }

    @OnClick({R.id.img_ViewDetailProcess_Back, R.id.ln_ViewDetailProcess_All})
    public void OnViewClicked(View v) {
        switch (v.getId()) {
            case R.id.img_ViewDetailProcess_Back: {
                imgBack.startAnimation(AnimationController.share.fadeIn(requireContext()));
                sBaseActivity.backFragment("");
                break;
            }
            case R.id.ln_ViewDetailProcess_All: {
                break;
            }
        }
    }
}
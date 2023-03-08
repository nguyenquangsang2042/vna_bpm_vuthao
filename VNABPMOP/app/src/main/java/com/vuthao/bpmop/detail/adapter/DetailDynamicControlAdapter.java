package com.vuthao.bpmop.detail.adapter;

import android.app.Activity;
import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.LinearLayout;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.app.Notify;
import com.vuthao.bpmop.base.model.custom.WorkFlowRelated;
import com.vuthao.bpmop.base.model.app.WorkflowItem;
import com.vuthao.bpmop.base.model.custom.AttachFile;
import com.vuthao.bpmop.base.model.custom.Comment;
import com.vuthao.bpmop.base.model.custom.Task;
import com.vuthao.bpmop.base.model.dynamic.ViewRow;
import com.vuthao.bpmop.base.model.dynamic.ViewSection;
import com.vuthao.bpmop.core.component.ComponentBase;
import com.vuthao.bpmop.core.component.ComponentComment;
import com.vuthao.bpmop.core.component.ComponentFlowRelated;
import com.vuthao.bpmop.core.component.ComponentRow1;
import com.vuthao.bpmop.core.component.ComponentRow2;
import com.vuthao.bpmop.core.component.ComponentRow3;
import com.vuthao.bpmop.core.component.ComponentTaskList;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;

public class DetailDynamicControlAdapter extends RecyclerView.Adapter<DetailDynamicControlAdapter.DetailDynamicControlHolder> {
    private Context context;
    private Activity activity;
    private int flagView = -1;
    private ArrayList<ViewSection> lstSection;
    private ArrayList<WorkFlowRelated> lstFlowRelated;
    private ArrayList<AttachFile> lstAttachComment = new ArrayList<AttachFile>();
    private ArrayList<Task> lstTask;
    private WorkflowItem currentWorkflowItem;
    private ArrayList<Comment> lstComment;
    private Notify currentNotifyItem;
    private ComponentComment componentComment;
    private String commentContent = "";
    private ArrayList<ViewRow> rows;

    public DetailDynamicControlAdapter(Context _context, Activity activity, int _flagView, ArrayList<ViewSection> _lstSection,
                                       ArrayList<WorkFlowRelated> _lstFlowRelated, ArrayList<Task> lstTask, ArrayList<Comment> lstComment, WorkflowItem _currentWorkflowItem, Notify _currentNotifyItem ) {
        this.context = _context;
        this.activity = activity;
        this.flagView = _flagView;
        this.lstSection = _lstSection;
        this.lstFlowRelated = _lstFlowRelated;
        this.lstTask = lstTask;
        this.lstComment = lstComment;
        this.currentWorkflowItem = _currentWorkflowItem;
        this.currentNotifyItem = _currentNotifyItem;
        rows = this.lstSection.get(0).getViewRows();
    }

    public void updateListAttachment(ArrayList<AttachFile> _lstAttachComment) {
        this.lstAttachComment.clear();
        this.lstAttachComment.addAll(_lstAttachComment);

        if (componentComment != null) {
            componentComment.updateListParentAttach(this.lstAttachComment);
            commentContent = componentComment.getCurrentParentCommentContent();
        }
    }

    public void clearComment() {
        lstAttachComment.clear();
        componentComment.clearContent();
    }

    public void updateListComment(ArrayList<Comment> comments) {
        lstComment = comments;
    }

    public ArrayList<AttachFile> getListAttachComment()
    {
        if (lstAttachComment != null && lstAttachComment.size() > 0)
            return lstAttachComment;
        return new ArrayList<>();
    }

    public void updateCurrentList(ArrayList<ViewSection> sections) {
        lstSection = sections;
    }
    @NonNull
    @Override
    public DetailDynamicControlHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_detail_workflow, parent, false);
        return new DetailDynamicControlHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull DetailDynamicControlHolder holder, int position) {
        holder.lnContent.removeAllViews();
        if (position < getItemCount() - 3) {
            ViewRow viewRow = rows.get(position);
            ComponentBase components = null;

            if (viewRow.getElements() != null && viewRow.getElements().size() > 0) {
                switch (viewRow.getRowType()) {
                    case 1: {
                        components = new ComponentRow1(activity, holder.lnContent, viewRow.getElements().get(0), -1, true, flagView);
                        break;
                    }
                    case 2: {
                        components = new ComponentRow2(activity, holder.lnContent, viewRow, -1, false, flagView);
                        break;
                    }
                    case 3: {
                        components = new ComponentRow3(activity, holder.lnContent, viewRow, -1, false, flagView);
                        break;
                    }
                    case 4: {
                        components = new ComponentRow1(activity, holder.lnContent, viewRow.getElements().get(0), -1, true, flagView);
                    }
                }

                assert components != null;
                components.initializeFrameView(holder.lnContent);
                components.setTitle();
                components.setValue();
                components.setEnable();
                components.setProprety();
            }
        } else {
            if (position == getItemCount() - 3) {
                // Nếu có Quy trình liên kết mới hiện
                if (lstFlowRelated != null && lstFlowRelated.size() > 0) {
                    ComponentFlowRelated related = new ComponentFlowRelated(activity, context, holder.lnContent, lstFlowRelated, currentWorkflowItem, currentNotifyItem);
                    related.initializeFrameView(holder.lnContent);
                    related.setTitle();
                    related.setValue();
                    related.setEnable();
                    related.setProprety();
                }
            } else if (position == getItemCount() - 2) {
                // Nếu có Task mới hiện
                if (lstTask != null && lstTask.size() > 0) {
                    ComponentTaskList taskList = new ComponentTaskList(activity, context, holder.lnContent, lstTask, false);
                    taskList.initializeFrameView(holder.lnContent);
                    taskList.setTitle();
                    taskList.setValue();
                    taskList.setEnable();
                    taskList.setProprety();
                }
            } else if (position == getItemCount() - 1) {
                // Nếu có Comment mới hiện, ko có cmt hiện edittext
                componentComment = new ComponentComment(activity, context, holder.lnContent, lstComment, false);
                if (!Functions.isNullOrEmpty(commentContent)) {
                    componentComment.updateCurrentParentCommentContent(commentContent);
                }

                componentComment.initFlagRecalculateView(false);
                componentComment.initializeFrameView(holder.lnContent);
                componentComment.setTitle();
                componentComment.setValue();
                componentComment.setEnable();
                componentComment.setProprety();

                if (lstAttachComment != null && lstAttachComment.size() > 0) {
                    componentComment.updateListParentAttach(lstAttachComment);
                }
            }
        }
    }

    @Override
    public int getItemCount() {
        int groupcount = 3; // +3 cho FlowRelated, Task, Comment nam o cuoi
        int childcount = 0;
        for (int i = 0; i < lstSection.size(); i++) {
            if (lstSection.get(i).getViewRows() != null) {
                childcount += lstSection.get(i).getViewRows().size();
            }
        }
        return groupcount + childcount;
    }

    public class DetailDynamicControlHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.ln_ItemExpandDetailWorkflow_Content)
        LinearLayout lnContent;

        public DetailDynamicControlHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
        }
    }
}

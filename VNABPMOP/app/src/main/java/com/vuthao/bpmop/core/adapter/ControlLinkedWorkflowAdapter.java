package com.vuthao.bpmop.core.adapter;

import android.app.Activity;
import android.content.Context;
import android.content.res.ColorStateList;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.app.WorkflowItem;
import com.vuthao.bpmop.core.component.ControlBase;
import com.vuthao.bpmop.detail.custom.DetailFunc;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;
import de.hdodenhof.circleimageview.CircleImageView;

public class ControlLinkedWorkflowAdapter extends RecyclerView.Adapter {
    private final Context context;
    private final ArrayList<WorkflowItem> lstWorkflowItem;
    private final ControlBase.ControlLinkedWorkflowListener listener;
    private int widthScreenTablet = -1;

    public ControlLinkedWorkflowAdapter(Activity mainAct, Context context, ArrayList<WorkflowItem> lstWorkflowItem, ControlBase.ControlLinkedWorkflowListener listener, int widthScreenTablet) {
        this.context = context;
        this.lstWorkflowItem = lstWorkflowItem;
        this.listener = listener;
        this.widthScreenTablet = widthScreenTablet;
    }

    @NonNull
    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        if (widthScreenTablet == -1) {
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_control_linked_workflow_phone, parent, false);
            return new ControlLinkedWorkflowPhoneHolder(view);
        } else {
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_control_linked_workflow, parent, false);
            return new ControlLinkedWorkflowHolder(view);
        }
    }

    @Override
    public void onBindViewHolder(@NonNull RecyclerView.ViewHolder holder, int position) {
        WorkflowItem item = lstWorkflowItem.get(position);
        if (item != null) {
            if (widthScreenTablet == -1) {
               ControlLinkedWorkflowPhoneHolder holderPhoneRelated = (ControlLinkedWorkflowAdapter.ControlLinkedWorkflowPhoneHolder) holder;
                if (!Functions.isNullOrEmpty(item.getWorkflowTitle())) {
                    holderPhoneRelated.tvTitle.setText(item.getWorkflowTitle());
                } else {
                    holderPhoneRelated.tvTitle.setText("");
                }

                if (!Functions.isNullOrEmpty(item.getActionStatus())) {
                    holderPhoneRelated.tvStatus.setVisibility(View.VISIBLE);
                    holderPhoneRelated.tvStatus.setText(item.getActionStatus());
                    holderPhoneRelated.tvStatus.setBackgroundTintList(ColorStateList.valueOf(DetailFunc.share.getColorByActionID(context, item.getActionStatusID())));
                } else {
                    holderPhoneRelated.tvStatus.setVisibility(View.INVISIBLE);
                }

                holderPhoneRelated.imgIschecked.setVisibility(View.VISIBLE);
            } else {
                ControlLinkedWorkflowHolder holderRelated = (ControlLinkedWorkflowHolder) holder;
                if (!Functions.isNullOrEmpty(item.getWorkflowTitle())) {
                    holderRelated.tvTitle.setText(item.getWorkflowTitle());
                } else {
                    holderRelated.tvTitle.setText("");
                }

                if (!Functions.isNullOrEmpty(item.getActionStatus())) {
                    holderRelated.tvStatus.setVisibility(View.VISIBLE);
                    holderRelated.tvStatus.setText(item.getActionStatus());
                    holderRelated.tvStatus.setBackgroundTintList(ColorStateList.valueOf(DetailFunc.share.getColorByActionID(context, item.getActionStatusID())));
                } else {
                    holderRelated.tvStatus.setVisibility(View.INVISIBLE);
                }

                holderRelated.imgIschecked.setVisibility(View.VISIBLE);
            }
        }
    }

    @Override
    public int getItemCount() {
        return lstWorkflowItem.size();
    }

    public class ControlLinkedWorkflowHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.ln_ItemControlLinkedWorkflow_All)
        LinearLayout lnAll;
        @BindView(R.id.view_ItemControlLinkedWorkflow_LeftSelected)
        View viewIsSelected;
        @BindView(R.id.tv_ItemControlLinkedWorkflow_Avatar)
        TextView tvAvatar;
        @BindView(R.id.img_ItemControlLinkedWorkflow_Avatar)
        CircleImageView imgAvatar;
        @BindView(R.id.tv_ItemControlLinkedWorkflow_Title)
        TextView tvTitle;
        @BindView(R.id.tv_ItemControlLinkedWorkflow_Description)
        TextView tvDescription;
        @BindView(R.id.tv_ItemControlLinkedWorkflow_Time)
        TextView tvTime;
        @BindView(R.id.tv_ItemControlLinkedWorkflow_Status)
        TextView tvStatus;
        @BindView(R.id.img_ItemControlLinkedWorkflow_Checked)
        ImageView imgIschecked;

        public ControlLinkedWorkflowHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);

            imgIschecked.setOnClickListener(v -> listener.OnDeleteItemClick(getAdapterPosition()));
        }
    }

    public class ControlLinkedWorkflowPhoneHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.ln_ItemControlLinkedWorkflow_Phone_All)
        LinearLayout lnAll;
        @BindView(R.id.view_ItemControlLinkedWorkflow_Phone_LeftSelected)
        View viewIsSelected;
        @BindView(R.id.tv_ItemControlLinkedWorkflow_Phone_Avatar)
        TextView tvAvatar;
        @BindView(R.id.img_ItemControlLinkedWorkflow_Phone_Avatar)
        CircleImageView imgAvatar;
        @BindView(R.id.tv_ItemControlLinkedWorkflow_Phone_Title)
        TextView tvTitle;
        @BindView(R.id.tv_ItemControlLinkedWorkflow_Phone_Description)
        TextView tvDescription;
        @BindView(R.id.tv_ItemControlLinkedWorkflow_Phone_Time)
        TextView tvTime;
        @BindView(R.id.tv_ItemControlLinkedWorkflow_Phone_Status)
        TextView tvStatus;
        @BindView(R.id.img_ItemControlLinkedWorkflow_Phone_Checked)
        ImageView imgIschecked;

        public ControlLinkedWorkflowPhoneHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);

            imgIschecked.setOnClickListener(v -> listener.OnDeleteItemClick(getAdapterPosition()));
        }
    }
}

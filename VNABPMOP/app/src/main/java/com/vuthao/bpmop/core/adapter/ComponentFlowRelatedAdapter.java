package com.vuthao.bpmop.core.adapter;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.res.ColorStateList;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.RecyclerView;

import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.ImageLoader;
import com.vuthao.bpmop.base.model.app.WorkflowStatus;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.base.model.app.AppStatus;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.Notify;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.model.custom.WorkFlowRelated;
import com.vuthao.bpmop.base.model.app.WorkflowItem;
import com.vuthao.bpmop.base.realm.RealmController;
import com.vuthao.bpmop.detail.DetailWorkflowActivity;
import com.vuthao.bpmop.detail.custom.DetailFunc;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;
import de.hdodenhof.circleimageview.CircleImageView;

public class ComponentFlowRelatedAdapter extends RecyclerView.Adapter<ComponentFlowRelatedAdapter.ComponentFlowRelatedHolder> {
    public Activity mainAct;
    public Context context;
    public ArrayList<WorkFlowRelated> lstWorkflowItem;
    private final WorkflowItem currentWorkflowItem;

    public ComponentFlowRelatedAdapter(Activity mainAct, Context context, ArrayList<WorkFlowRelated> lstWorkflowItem, WorkflowItem currentWorkflowItem, Notify _currentNotifyItem) {
        this.mainAct = mainAct;
        this.context = context;
        this.lstWorkflowItem = lstWorkflowItem;
        this.currentWorkflowItem = currentWorkflowItem;
    }

    @NonNull
    @Override
    public ComponentFlowRelatedHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_control_workflow_related, parent, false);
        return new ComponentFlowRelatedHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull ComponentFlowRelatedAdapter.ComponentFlowRelatedHolder holder, int position) {
        if (position % 2 == 0) {
            holder.lnAll.setBackgroundColor(ContextCompat.getColor(context, R.color.clVer2BlueNavigation));
        } else {
            holder.lnAll.setBackgroundColor(ContextCompat.getColor(context, R.color.clTransparent));
        }

        WorkFlowRelated related = lstWorkflowItem.get(position);
        if (related != null) {
            String currentWorkFlowID = currentWorkflowItem.getID();

            if (!Functions.isNullOrEmpty(related.getCreatedBy())) {
                User user = new RealmController().getRealm().where(User.class)
                        .equalTo("FullName", related.getCreatedBy())
                        .findFirst();
                if (user != null) {
                    ImageLoader.getInstance().loadImageUserWithToken(mainAct, Constants.BASE_URL + user.getImagePath(), holder.imgAvatar);
                } else {
                    holder.imgAvatar.setImageResource(R.drawable.icon_avatar64);
                }
            } else {
                holder.imgAvatar.setImageResource(R.drawable.icon_avatar64);
            }

            if (currentWorkFlowID.equals(String.valueOf(related.getItemRLID()))) {
                holder.tvTitle.setText(related.getWorkflowContent());
                holder.tvDescription.setText(related.getItemCode());

                if (!Functions.isNullOrEmpty(related.getCreated())) {
                    holder.tvTime.setText(Functions.share.formatDateLanguage(related.getCreated()));
                } else {
                    holder.tvTime.setText("");
                }

                if (related.getStatusWorkflowID() >= 0) {
                    holder.tvStatus.setVisibility(View.VISIBLE);
                    holder.tvStatus.setBackgroundTintList(ColorStateList.valueOf(DetailFunc.share.getColorByAppStatus(related.getStatusWorkflowID())));
                    AppStatus status = new RealmController().getRealm().where(AppStatus.class)
                            .equalTo("ID", related.getStatusWorkflowID()).findFirst();
                    if (status != null) {
                        if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
                            holder.tvStatus.setText(status.getTitle());
                        } else {
                            holder.tvStatus.setText(status.getTitleEN());
                        }
                    } else {
                        holder.tvStatus.setText(related.getStatusWorkflow());
                    }
                } else {
                    holder.tvStatus.setText("");
                }
            } else if (currentWorkFlowID.equals(String.valueOf(related.getItemID()))) {
                holder.tvTitle.setText(related.getWorkflowContentRL());
                holder.tvDescription.setText(related.getRelatedCode());

                if (!Functions.isNullOrEmpty(related.getCreatedRL())) {
                    holder.tvTime.setText(Functions.share.formatDateLanguage(related.getCreatedRL()));
                } else {
                    holder.tvTime.setText("");
                }

                if (related.getStatusWorkflowRLID() > 0) {
                    holder.tvStatus.setVisibility(View.VISIBLE);
                    holder.tvStatus.setBackgroundTintList(ColorStateList.valueOf(DetailFunc.share.getColorByAppStatus(related.getStatusWorkflowRLID())));
                    WorkflowStatus status = new RealmController().getRealm()
                            .where(WorkflowStatus.class)
                            .equalTo("ID", related.getStatusWorkflowRLID())
                            .findFirst();
                    if (status != null) {
                        if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
                            holder.tvStatus.setText(status.getTitle());
                        } else {
                            holder.tvStatus.setText(status.getTitleEN());
                        }
                    } else {
                        holder.tvStatus.setText(related.getStatusWorkflow());
                    }
                } else {
                    holder.tvStatus.setVisibility(View.INVISIBLE);
                }
            }

            holder.lnAll.setOnClickListener(v -> {
                Intent i = new Intent();
                i.setAction(VarsReceiver.FLOW_RELATE_CLICK);
                i.putExtra("related", new Gson().toJson(related));
                i.putExtra("WorkflowItemId", related.getItemID());
                BroadcastUtility.send(mainAct, i);
            });
        }
    }

    @Override
    public int getItemCount() {
        return lstWorkflowItem.size();
    }

    public static class ComponentFlowRelatedHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.ln_ItemControlFlowRelated_All)
        LinearLayout lnAll;
        @BindView(R.id.view_ItemControlFlowRelated_LeftSelected)
        View viewIsSelected;
        @BindView(R.id.img_ItemControlFlowRelated_Avatar)
        CircleImageView imgAvatar;
        @BindView(R.id.tv_ItemControlFlowRelated_Title)
        TextView tvTitle;
        @BindView(R.id.tv_ItemControlFlowRelated_Description)
        TextView tvDescription;
        @BindView(R.id.tv_ItemControlFlowRelated_Time)
        TextView tvTime;
        @BindView(R.id.tv_ItemControlFlowRelated_Status)
        TextView tvStatus;

        public ComponentFlowRelatedHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
        }
    }
}

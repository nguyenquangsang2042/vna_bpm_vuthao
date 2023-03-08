package com.vuthao.bpmop.detail.adapter;

import android.content.Context;
import android.graphics.drawable.GradientDrawable;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.RecyclerView;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.ImageLoader;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.custom.WorkflowHistory;
import com.vuthao.bpmop.detail.custom.DetailFunc;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;
import de.hdodenhof.circleimageview.CircleImageView;

public class ChildProcessAdapter extends RecyclerView.Adapter<ChildProcessAdapter.ChildProcessHolder> {
    private final Context context;
    private final ArrayList<WorkflowHistory> workflowHistories;

    public ChildProcessAdapter(Context context, ArrayList<WorkflowHistory> workflowHistories) {
        this.context = context;
        this.workflowHistories = workflowHistories;
    }

    @NonNull
    @Override
    public ChildProcessHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_expand_detail_process_child_lv2,parent,false);
        return new ChildProcessHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull ChildProcessHolder holder, int position) {
        WorkflowHistory history = workflowHistories.get(position);

        if (workflowHistories.size() - 1 == position) {
            holder.viewMarginBottom.setVisibility(View.VISIBLE);
        } else {
            holder.viewMarginBottom.setVisibility(View.GONE);
        }

        holder.vwGroupLine.setVisibility(View.INVISIBLE);
        if (history != null) {
            if (!Functions.isNullOrEmpty(history.getAssignUserAvatar())) {
                holder.tvAvatar.setVisibility(View.GONE);
                holder.imgAvatar.setVisibility(View.VISIBLE);
                ImageLoader.getInstance().loadImageUserWithToken(context, Constants.BASE_URL + history.getAssignUserAvatar(), holder.imgAvatar);
            } else {
                holder.tvAvatar.setVisibility(View.VISIBLE);
                holder.imgAvatar.setVisibility(View.GONE);
            }

            if (!Functions.isNullOrEmpty(history.getAssignUserName())) {
                holder.tvAvatar.setText(Functions.share.getAvatarName(history.getAssignUserName()));
                holder.tvName.setText(history.getAssignUserName());
            } else {
                holder.tvName.setText("");
            }

            if (!Functions.isNullOrEmpty(history.getAssignPositionTitle())) {
                holder.tvPosition.setText(history.getAssignPositionTitle());
            } else {
                holder.tvPosition.setText("");
            }

            if (!Functions.isNullOrEmpty(history.getCompletedDate())) {
                holder.tvDate.setText(Functions.share.formatDateLanguage(history.getCompletedDate()));
            } else {
                holder.tvDate.setText("");
            }

            if (!Functions.isNullOrEmpty(history.getComment())) {
                holder.tvYKien.setText(history.getComment());
            } else {
                holder.tvYKien.setText("");
            }

            if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
                if (!Functions.isNullOrEmpty(history.getSubmitAction())) {
                    GradientDrawable drawable = new GradientDrawable();
                    drawable.setStroke(2, ContextCompat.getColor(context,R.color.clWhite));
                    drawable.setCornerRadius(7);
                    drawable.setShape(GradientDrawable.RECTANGLE);
                    drawable.setColor(DetailFunc.share.getColorByActionIDProcess(context, history.getSubmitActionId()));

                    holder.tvAction.setText(history.getSubmitAction().trim());
                    holder.tvAction.setBackground(drawable);
                } else {
                    holder.tvAction.setVisibility(View.GONE);
                }
            } else {
                if (!Functions.isNullOrEmpty(history.getSubmitActionEN())) {
                    GradientDrawable drawable = new GradientDrawable();
                    drawable.setStroke(2, ContextCompat.getColor(context,R.color.clWhite));
                    drawable.setCornerRadius(7);
                    drawable.setShape(GradientDrawable.RECTANGLE);
                    drawable.setColor(DetailFunc.share.getColorByActionIDProcess(context, history.getSubmitActionId()));

                    holder.tvAction.setText(history.getSubmitActionEN().trim());
                    holder.tvAction.setBackground(drawable);
                } else {
                    holder.tvAction.setVisibility(View.GONE);
                }
            }
        }
    }

    @Override
    public int getItemCount() {
        return workflowHistories.size();
    }

    public class ChildProcessHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.tv_ItemExpandDetaiProcessChild_Ver2_Lv2_Date)
        TextView tvDate;
        @BindView(R.id.tv_ItemExpandDetaiProcessChild_Ver2_Lv2_YKien)
        TextView tvYKien;
        @BindView(R.id.tv_ItemExpandDetaiProcessChild_Ver2_Lv2_Name)
        TextView tvName;
        @BindView(R.id.tv_ItemExpandDetaiProcessChild_Ver2_Lv2_Position)
        TextView tvPosition;
        @BindView(R.id.tv_ItemExpandDetaiProcessChild_Ver2_Lv2_Action)
        TextView tvAction;
        @BindView(R.id.vw_ItemExpandDetaiProcessChild_Ver2_Lv2_GroupLine)
        View vwGroupLine;
        @BindView(R.id.tv_ItemExpandDetaiProcessChild_Ver2_Lv2_Avata)
        TextView tvAvatar;
        @BindView(R.id.img_ItemExpandDetaiProcessChild_Ver2_Lv2_Avata)
        CircleImageView imgAvatar;
        @BindView(R.id.vw_ItemExpandDetaiProcessChild_Ver2_Lv2_MarginBottom)
        View viewMarginBottom;

        public ChildProcessHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
        }
    }
}

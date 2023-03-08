package com.vuthao.bpmop.detail.adapter;

import android.content.Context;
import android.graphics.drawable.GradientDrawable;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseExpandableListAdapter;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.RecyclerView;
import androidx.recyclerview.widget.StaggeredGridLayoutManager;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.ImageLoader;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.custom.WorkflowHistory;
import com.vuthao.bpmop.detail.custom.DetailFunc;

import java.util.ArrayList;

import de.hdodenhof.circleimageview.CircleImageView;

public class ExpandDetailProcessAdapter extends BaseExpandableListAdapter {
    private final Context context;
    private final ArrayList<WorkflowHistory> workflowHistories;

    public ExpandDetailProcessAdapter(Context context, ArrayList<WorkflowHistory> workflowHistories) {
        this.context = context;
        this.workflowHistories = workflowHistories;
    }

    @Override
    public int getGroupCount() {
        return workflowHistories.size();
    }

    @Override
    public int getChildrenCount(int groupPosition) {
        return workflowHistories.get(groupPosition).getChildHistory().size();
    }

    @Override
    public Object getGroup(int groupPosition) {
        return groupPosition;
    }

    @Override
    public Object getChild(int groupPosition, int childPosition) {
        return childPosition;
    }

    @Override
    public long getGroupId(int groupPosition) {
        return groupPosition;
    }

    @Override
    public long getChildId(int groupPosition, int childPosition) {
        return childPosition;
    }

    @Override
    public boolean hasStableIds() {
        return false;
    }

    @Override
    public View getGroupView(int groupPosition, boolean isExpanded, View convertView, ViewGroup parent) {
        View rootView = convertView;
        if (rootView == null) {
            LayoutInflater inflater = LayoutInflater.from(context);
            rootView = inflater.inflate(R.layout.item_expand_detail_process_group, parent, false);
        }

        TextView tvTitle = rootView.findViewById(R.id.tv_ItemExpandDetaiProcessGroup_Title);
        ImageView imgStatus = rootView.findViewById(R.id.img_ItemExpandDetaiProcessGroup);
        View vwMarginTop = rootView.findViewById(R.id.vw_ItemExpandDetaiProcessGroup_MarginTop);

        if (groupPosition == 0) {
            vwMarginTop.setVisibility(View.VISIBLE);
        } else {
            vwMarginTop.setVisibility(View.GONE);
        }

        if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
            if (!Functions.isNullOrEmpty(workflowHistories.get(groupPosition).getTitle())) {
                tvTitle.setText(workflowHistories.get(groupPosition).getTitle());
            } else {
                tvTitle.setText("");
            }
        }
        else
        {
            if (!Functions.isNullOrEmpty(workflowHistories.get(groupPosition).getTitleEN())) {
                tvTitle.setText(workflowHistories.get(groupPosition).getTitleEN());
            } else {
                tvTitle.setText("");
            }
        }

        if (workflowHistories.get(groupPosition).isStatus() == 1) {
            imgStatus.setColorFilter(ContextCompat.getColor(context, R.color.clActionGreen));
        } else {
            imgStatus.setColorFilter(ContextCompat.getColor(context, R.color.clActionYellow));
        }

        return rootView;
    }

    @Override
    public View getChildView(int groupPosition, int childPosition, boolean isLastChild, View convertView, ViewGroup parent) {
        WorkflowHistory history = workflowHistories.get(groupPosition).getChildHistory().get(childPosition);

        View rootView = convertView;
        if (rootView == null) {
            LayoutInflater inflater = LayoutInflater.from(context);
            rootView = inflater.inflate(R.layout.item_expand_detail_process_child, parent, false);
        }

        TextView tvDate = rootView.findViewById(R.id.tv_ItemExpandDetaiProcessChild_Ver2_Date);
        TextView tvYKien = rootView.findViewById(R.id.tv_ItemExpandDetaiProcessChild_Ver2_YKien);
        TextView tvName = rootView.findViewById(R.id.tv_ItemExpandDetaiProcessChild_Ver2_Name);
        TextView tvPosition = rootView.findViewById(R.id.tv_ItemExpandDetaiProcessChild_Ver2_Position);
        TextView tvAction = rootView.findViewById(R.id.tv_ItemExpandDetaiProcessChild_Ver2_Action);
        View vwGroupLine = rootView.findViewById(R.id.vw_ItemExpandDetaiProcessChild_Ver2_GroupLine);
        TextView tvAvatar = rootView.findViewById(R.id.tv_ItemExpandDetaiProcessChild_Ver2_Avata);
        CircleImageView imgAvatar = rootView.findViewById(R.id.img_ItemExpandDetaiProcessChild_Ver2_Avata);
        View vwMarginTop = rootView.findViewById(R.id.vw_ItemExpandDetaiProcessChild_Ver2_MarginTop);
        RecyclerView recyChild = rootView.findViewById(R.id.recy_ItemExpandDetaiProcessChild_Ver2_Child);

        // Item đầu tiên -> margin top
        if (childPosition == 0) {
            vwMarginTop.setVisibility(View.VISIBLE);
        } else {
            vwMarginTop.setVisibility(View.GONE);
        }

        if (groupPosition == workflowHistories.size() - 1) {
            vwGroupLine.setVisibility(View.INVISIBLE);
        } else {
            vwGroupLine.setVisibility(View.VISIBLE);
        }

        if(!Functions.isNullOrEmpty(history.getAssignUserAvatar())) {
            tvAvatar.setVisibility(View.GONE);
            imgAvatar.setVisibility(View.VISIBLE);
            ImageLoader.getInstance().loadImageUserWithToken(context, Constants.BASE_URL + history.getAssignUserAvatar(), imgAvatar);
        } else {
            tvAvatar.setVisibility(View.VISIBLE);
            imgAvatar.setVisibility(View.GONE);
        }

        if (!Functions.isNullOrEmpty(history.getAssignUserName())) {
            tvAvatar.setText(Functions.share.getAvatarName(history.getAssignUserName()));
            tvName.setText(history.getAssignUserName());
        } else {
            tvName.setText("");
        }

        if (!Functions.isNullOrEmpty(history.getAssignPositionTitle())) {
            tvPosition.setText(history.getAssignPositionTitle());
        } else {
            tvPosition.setText("");
        }

        if (!Functions.isNullOrEmpty(history.getCompletedDate())) {
            tvDate.setText(Functions.share.formatDateLanguage(history.getCompletedDate()));
        } else {
            tvDate.setText("");
        }

        if (!Functions.isNullOrEmpty(history.getComment())) {
            tvYKien.setText(history.getComment());
        } else {
            tvYKien.setText("");
        }

        if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
            if (!Functions.isNullOrEmpty(history.getSubmitAction())) {
                GradientDrawable drawable = new GradientDrawable();
                drawable.setStroke(2, ContextCompat.getColor(context,R.color.clWhite));
                drawable.setCornerRadius(7);
                drawable.setShape(GradientDrawable.RECTANGLE);
                drawable.setColor(DetailFunc.share.getColorByActionIDProcess(context, history.getSubmitActionId()));

                tvAction.setText(history.getSubmitAction().trim());
                tvAction.setBackground(drawable);
            } else {
                tvAction.setVisibility(View.GONE);
            }
        } else {
            if (!Functions.isNullOrEmpty(history.getSubmitActionEN())) {
                GradientDrawable drawable = new GradientDrawable();
                drawable.setStroke(2, ContextCompat.getColor(context,R.color.clWhite));
                drawable.setCornerRadius(7);
                drawable.setShape(GradientDrawable.RECTANGLE);
                drawable.setColor(DetailFunc.share.getColorByActionIDProcess(context, history.getSubmitActionId()));

                tvAction.setText(history.getSubmitActionEN().trim());
                tvAction.setBackground(drawable);
            } else {
                tvAction.setVisibility(View.GONE);
            }
        }

        if (history.getChildHistory() != null && history.getChildHistory().size() > 0) {
            recyChild.setVisibility(View.VISIBLE);
            ChildProcessAdapter adapter = new ChildProcessAdapter(context, history.getChildHistory());
            StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayout.VERTICAL);
            recyChild.setAdapter(adapter);
            recyChild.setLayoutManager(staggeredGridLayoutManager);
        } else {
            recyChild.setVisibility(View.GONE);
        }

        return rootView;
    }

    @Override
    public boolean isChildSelectable(int groupPosition, int childPosition) {
        return false;
    }
}

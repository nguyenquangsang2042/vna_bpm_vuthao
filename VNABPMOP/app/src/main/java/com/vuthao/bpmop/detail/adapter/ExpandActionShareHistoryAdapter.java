package com.vuthao.bpmop.detail.adapter;

import android.app.Activity;
import android.content.Context;
import android.content.res.ColorStateList;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseExpandableListAdapter;
import android.widget.LinearLayout;
import android.widget.TextView;
import androidx.core.content.ContextCompat;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.ImageLoader;
import com.vuthao.bpmop.base.model.custom.GroupShareHistory;
import com.vuthao.bpmop.base.model.custom.ShareHistory;

import java.util.ArrayList;

import de.hdodenhof.circleimageview.CircleImageView;

public class ExpandActionShareHistoryAdapter extends BaseExpandableListAdapter {
    private Activity activity;
    private Context context;
    private ArrayList<GroupShareHistory> shareHistories;

    public ExpandActionShareHistoryAdapter(Activity activity, Context context, ArrayList<GroupShareHistory> shareHistories) {
        this.activity = activity;
        this.context = context;
        this.shareHistories = shareHistories;
    }

    @Override
    public int getGroupCount() {
        return shareHistories.size();
    }

    @Override
    public int getChildrenCount(int groupPosition) {
        if (shareHistories.get(groupPosition).getListChild() != null &&
        shareHistories.get(groupPosition).getListChild().size() > 0) {
            return shareHistories.get(groupPosition).getListChild().size();
        } else {
            return 0;
        }
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
        LayoutInflater mInflater = LayoutInflater.from(context);
        View rootView = mInflater.inflate(R.layout.item_share_history_group, null);

        LinearLayout lnAll = rootView.findViewById(R.id.ln_ItemShareHistory_Group_All);
        TextView tvAvatar = rootView.findViewById(R.id.tv_ItemShareHistory_Group_Avatar);
        CircleImageView imgAvatar = rootView.findViewById(R.id.img_ItemShareHistory_Group_Avatar);
        TextView tvTitle = rootView.findViewById(R.id.tv_ItemShareHistory_Group_Title);
        TextView tvTime = rootView.findViewById(R.id.tv_ItemShareHistory_Group_Time);
        TextView tvEmail = rootView.findViewById(R.id.tv_ItemShareHistory_Group_Email);
        TextView tvComment = rootView.findViewById(R.id.tv_ItemShareHistory_Group_Comment);

        if (groupPosition % 2 == 0) {
            lnAll.setBackgroundColor(ContextCompat.getColor(context, R.color.clVer2BlueNavigation));
        } else {
            lnAll.setBackgroundColor(ContextCompat.getColor(context, R.color.clWhite));
        }

        ShareHistory shareHistory = shareHistories.get(groupPosition).getParentItem();

        if (shareHistory != null) {
            if (!Functions.isNullOrEmpty(shareHistory.getUserImagePath())) {
                imgAvatar.setVisibility(View.VISIBLE);
                tvAvatar.setVisibility(View.GONE);

                ImageLoader.getInstance().loadImageUserWithToken(context, Constants.BASE_URL + shareHistory.getUserImagePath(), imgAvatar);
            }
        }

        assert shareHistory != null;
        if (!Functions.isNullOrEmpty(shareHistory.getUserName())) {
            tvTitle.setText(shareHistory.getUserName());
            tvAvatar.setText(Functions.share.getAvatarName(shareHistory.getUserName()));
            tvAvatar.setBackgroundTintList(ColorStateList.valueOf(Functions.share.getColorByUsername(shareHistory.getUserName())));
        } else {
            tvTitle.setText("");
        }

        if (!Functions.isNullOrEmpty(shareHistory.getDateShared())) {
            tvTime.setText(Functions.share.formatDateLanguage(shareHistory.getDateShared()));
        } else {
            tvTime.setText("");
        }

        if (!Functions.isNullOrEmpty(shareHistory.getUserPosition())) {
            tvEmail.setText(shareHistory.getUserPosition());
        } else {
            tvEmail.setText("");
        }

        if (!Functions.isNullOrEmpty(shareHistory.getComment())) {
            tvComment.setVisibility(View.VISIBLE);
            tvComment.setText(shareHistory.getComment());
        } else {
            tvComment.setVisibility(View.GONE);
        }

        return  rootView;
    }

    @Override
    public View getChildView(int groupPosition, int childPosition, boolean isLastChild, View convertView, ViewGroup parent) {
        LayoutInflater inflater = LayoutInflater.from(context);
        View rootView = inflater.inflate(R.layout.item_share_history_child, null);
        LinearLayout lnAll = rootView.findViewById(R.id.ln_ItemShareHistory_Child_All);
        TextView tvAvatar = rootView.findViewById(R.id.tv_ItemShareHistory_Child_Avatar);
        CircleImageView imgAvatar = rootView.findViewById(R.id.img_ItemShareHistory_Child_Avatar);
        TextView tvTitle = rootView.findViewById(R.id.tv_ItemShareHistory_Child_Title);
        TextView tvEmail = rootView.findViewById(R.id.tv_ItemShareHistory_Child_Email);

        if (groupPosition % 2 == 0) {
            lnAll.setBackgroundColor(ContextCompat.getColor(context, R.color.clVer2BlueNavigation));
        } else {
            lnAll.setBackgroundColor(ContextCompat.getColor(context, R.color.clWhite));
        }
        ShareHistory shareHistory = shareHistories.get(groupPosition).getListChild().get(childPosition);

        if (shareHistory != null) {
            if (!Functions.isNullOrEmpty(shareHistory.getUserImagePath())) {
                imgAvatar.setVisibility(View.VISIBLE);
                tvAvatar.setVisibility(View.GONE);

                ImageLoader.getInstance().loadImageUserWithToken(context, Constants.BASE_URL + shareHistory.getUserImagePath(), imgAvatar);
            }

            if (!Functions.isNullOrEmpty(shareHistory.getUserName())) {
                tvTitle.setText(shareHistory.getUserName());
                tvAvatar.setText(Functions.share.getAvatarName(shareHistory.getUserName()));
                tvAvatar.setBackgroundTintList(ColorStateList.valueOf(Functions.share.getColorByUsername(shareHistory.getUserName())));
            } else {
                tvTitle.setText("");
            }

            if (!Functions.isNullOrEmpty(shareHistory.getUserPosition())) {
                tvEmail.setText(shareHistory.getUserPosition());
            } else {
                tvEmail.setText("");
            }
        }

        return rootView;
    }

    @Override
    public boolean isChildSelectable(int groupPosition, int childPosition) {
        return false;
    }
}

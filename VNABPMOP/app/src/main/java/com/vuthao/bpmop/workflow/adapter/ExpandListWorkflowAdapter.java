package com.vuthao.bpmop.workflow.adapter;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.core.content.ContextCompat;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.ImageLoader;
import com.vuthao.bpmop.base.custom.expandable.AnimatedExpandableListAdapter;
import com.vuthao.bpmop.base.custom.expandable.AnimatedExpandableListView;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.Workflow;
import com.vuthao.bpmop.base.model.app.WorkflowCategory;
import com.vuthao.bpmop.base.model.custom.BoardWorkflow;
import com.vuthao.bpmop.base.realm.RealmController;
import com.vuthao.bpmop.board.adapter.ExpandBoardMainGroupAdapter;

import java.util.ArrayList;

public class ExpandListWorkflowAdapter extends AnimatedExpandableListAdapter {
    private final Context context;
    private final ArrayList<BoardWorkflow> workflows;
    private final ArrayList<BoardWorkflow> workflowsBackup = new ArrayList<>();
    private final ArrayList<BoardWorkflow> workflowsFilter = new ArrayList<>();
    private final ExpandListWorkflowListener listener;

    public WorkflowCategory getWFCategory() {
        return WFCategory;
    }

    public void setWFCategory(WorkflowCategory WFCategory) {
        this.WFCategory = WFCategory;
    }

    private WorkflowCategory WFCategory;

    public interface ExpandListWorkflowListener {
        void OnItemClick(Workflow workflow);
    }

    public ExpandListWorkflowAdapter(Context context, ArrayList<BoardWorkflow> workflows, ExpandListWorkflowListener listener) {
        this.context = context;
        this.workflows = workflows;
        this.listener = listener;
        workflowsFilter.addAll(workflows);
        workflowsBackup.addAll(workflows);
    }

    public void filter() {
        workflows.clear();
        if (getWFCategory().getTitle().equals(Functions.share.getTitle("TEXT_ALL", "Tất cả"))) {
            workflows.addAll(workflowsBackup);
        } else {
            BoardWorkflow temp = new BoardWorkflow();
            for (BoardWorkflow workflow : workflowsFilter) {
                if (workflow.getWorkflowCategory().getTitle().equals(getWFCategory().getTitle())) {
                    temp = workflow;
                    break;
                }
            }

            workflows.add(temp);
        }

        notifyDataSetChanged();
    }

    @Override
    public int getGroupCount() {
        return workflows.size();
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
        if (getGroupCount() >= 2) {
            LayoutInflater inflater = LayoutInflater.from(context);

            View rootView = inflater.inflate(R.layout.item_expand_board_group, null);
            ImageView imgExpand = rootView.findViewById(R.id.img_ItemExpandBoardGroup_Expand);
            TextView tvTitle = rootView.findViewById(R.id.tv_ItemExpandBoardGroup_Title);

            WorkflowCategory category = workflows.get(groupPosition).getWorkflowCategory();
            if (category != null) {
                if (isExpanded) {
                    imgExpand.setRotation(0);
                } else {
                    imgExpand.setRotation(-90);
                }

                tvTitle.setText(category.getTitle());
            }

            return rootView;
        }
        return new View(context);
    }

    @Override
    public View getRealChildView(int groupPosition, int childPosition, boolean isLastChild, View convertView, ViewGroup parent) {
        LayoutInflater inflater = LayoutInflater.from(context);
        View rootView = inflater.inflate(R.layout.item_expand_board_child, null);

        LinearLayout lnAll = rootView.findViewById(R.id.ln_ItemVer2ExpandBoardChild_All);
        ImageView imgFavorite = rootView.findViewById(R.id.img_ItemVer2ExpandBoardChild_Favorite);
        ImageView imgAvatar = rootView.findViewById(R.id.img_ItemVer2ExpandBoardChild_Avatar);
        TextView tvTitle = rootView.findViewById(R.id.tv_ItemVer2ExpandBoardChild_Title);
        TextView tvDescription = rootView.findViewById(R.id.tv_ItemVer2ExpandBoardChild_Description);

        imgFavorite.setVisibility(View.GONE);
        Workflow workflow = workflows.get(groupPosition).getWorkflows().get(childPosition);

        if (workflow != null) {
            if (calculateCurrentPosition(groupPosition, childPosition) % 2 == 0) {
                lnAll.setBackgroundColor(ContextCompat.getColor(context, R.color.clWhite));
            } else {
                lnAll.setBackgroundColor(ContextCompat.getColor(context, R.color.clVer2BlueNavigation));
            }

            if (!Functions.isNullOrEmpty(workflow.getImageURL())) {
                ImageLoader.getInstance().loadImageWithToken(context, Constants.BASE_URL + workflow.getImageURL()
                        , imgAvatar, R.drawable.icon_ver3_app);
            } else {
                imgAvatar.setImageResource(R.drawable.icon_ver3_app);
            }

            if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
                tvTitle.setText(workflow.getTitle());
            } else {
                tvTitle.setText(workflow.getTitleEN());
            }

            tvDescription.setText("");

            lnAll.setOnClickListener(v -> listener.OnItemClick(workflows.get(groupPosition).getWorkflows().get(childPosition)));
        }

        return rootView;
    }

    @Override
    public int getRealChildrenCount(int groupPosition) {
        return workflows.get(groupPosition).getWorkflows().size();
    }

    private int calculateCurrentPosition(int groupPos, int childPos) {
        int result = -1;
        for (int i = 0; i < groupPos + 1; i++) {
            result += workflows.get(i).getWorkflows().size();
        }

        result += childPos + 1;
        return result;
    }

    @Override
    public boolean isChildSelectable(int groupPosition, int childPosition) {
        return true;
    }

    @Override
    public void onGroupExpanded(int groupPosition) {
        super.onGroupExpanded(groupPosition);
    }
}

package com.vuthao.bpmop.core.adapter;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.res.ColorStateList;
import android.graphics.drawable.GradientDrawable;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseExpandableListAdapter;
import android.widget.ExpandableListView;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;
import android.widget.TextView;

import androidx.core.content.ContextCompat;
import androidx.localbroadcastmanager.content.LocalBroadcastManager;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.ImageLoader;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.base.model.app.Group;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.model.custom.ExpandTask;
import com.vuthao.bpmop.base.model.custom.Task;
import com.vuthao.bpmop.base.realm.RealmController;
import com.vuthao.bpmop.detail.custom.DetailFunc;
import com.vuthao.bpmop.task.DetailCreateTaskActivity;

import java.util.ArrayList;

import de.hdodenhof.circleimageview.CircleImageView;

public class ExpandComponentTaskListAdapter extends BaseExpandableListAdapter {
    private Activity mainAct;
    private Context context;
    private ArrayList<ExpandTask> lstGroupTask;
    private int itemHeight;

    public ExpandComponentTaskListAdapter(Activity mainAct, Context context, LinearLayout _lnExpandParent, ArrayList<ExpandTask> lstGroupTask) {
        this.mainAct = mainAct;
        this.context = context;
        this.lstGroupTask = lstGroupTask;

        itemHeight = Functions.share.convertDpToPixel(100, context);
    }

    @Override
    public int getGroupCount() {
        return lstGroupTask.size();
    }

    @Override
    public int getChildrenCount(int groupPosition) {
        if (lstGroupTask.get(groupPosition).getLstChild() != null &&
                lstGroupTask.get(groupPosition).getLstChild().size() > 0) {
            return lstGroupTask.get(groupPosition).getLstChild().size();
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
        ComponentTaskList_GroupHolder holder;
        Task currentTaskItem = lstGroupTask.get(groupPosition).getGroupItem();
        if (convertView == null) {
            convertView = LayoutInflater.from(context).inflate(R.layout.item_control_task_list_group, null);
            holder = new ComponentTaskList_GroupHolder();
            holder.lnAll = convertView.findViewById(R.id.ln_ItemControlTaskList_Group_All);
            holder.lnContentClick = convertView.findViewById(R.id.ln_ItemControlTaskList_Group_ContentClick);
            holder.vwMarginTop = convertView.findViewById(R.id.vw_ItemControlTaskList_Group_MarginTop);
            holder.vwGroupLine = convertView.findViewById(R.id.vw_ItemControlTaskList_GroupLine);
            holder.relaGroup = convertView.findViewById(R.id.rela_ItemControlTaskList_Child_Group);
            holder.tvTitle = convertView.findViewById(R.id.tv_ItemControlTaskList_Group_Title);
            holder.imgCollapsed = convertView.findViewById(R.id.img_ItemControlTaskList_Group);
            holder.imgAvatar = convertView.findViewById(R.id.img_ItemControlTaskList_Group_Avata);
            holder.tvName = convertView.findViewById(R.id.tv_ItemControlTaskList_Group_Name);
            holder.tvDate = convertView.findViewById(R.id.tv_ItemControlTaskList_Group_Date);
            holder.tvPosition = convertView.findViewById(R.id.tv_ItemControlTaskList_Group_Position);
            holder.tvAction = convertView.findViewById(R.id.tv_ItemControlTaskList_Group_Action);

            convertView.setTag(holder);

            holder.lnContentClick.setOnClickListener(v -> {
                Intent intent = new Intent(mainAct, DetailCreateTaskActivity.class);
                //intent.setAction(VarsReceiver.TASK_CLICK);
                intent.putExtra("WorkflowItemId", lstGroupTask.get(groupPosition).getGroupItem().getSPItemId());
                intent.putExtra("taskId", lstGroupTask.get(groupPosition).getGroupItem().getID());
                intent.putExtra("isClickFromAction", false);
                mainAct.startActivity(intent);
                //BroadcastUtility.send(mainAct, intent);
            });

            if (groupPosition % 2 == 0) {
                GradientDrawable drawable = new GradientDrawable();
                if (isExpanded) {
                    drawable.setCornerRadii(new float[]{10, 10, 10, 10, 0, 0, 0, 0});
                } else {
                    drawable.setCornerRadius(10);
                }

                drawable.setShape(GradientDrawable.RECTANGLE);
                drawable.setColor(ContextCompat.getColor(context, R.color.clVer2BlueNavigation));
                holder.lnAll.setBackground(drawable);
            } else {
                GradientDrawable _drawable = new GradientDrawable();
                _drawable.setColor(ContextCompat.getColor(context, R.color.clWhite));
                holder.lnAll.setBackground(_drawable);
            }

            if (isExpanded) {
                holder.vwGroupLine.setVisibility(View.VISIBLE);
                holder.imgCollapsed.setImageResource(R.drawable.icon_ver2_show);
            } else {
                holder.vwGroupLine.setVisibility(View.INVISIBLE);
                holder.imgCollapsed.setImageResource(R.drawable.icon_ver2_close);
            }

            if (lstGroupTask.get(groupPosition).getLstChild() == null || lstGroupTask.get(groupPosition).getLstChild().size() == 0) {
                holder.relaGroup.setVisibility(View.INVISIBLE);
            } else {
                holder.relaGroup.setVisibility(View.VISIBLE);
            }

            setDataItemTaskToView(currentTaskItem, mainAct, context, holder.tvTitle, holder.imgAvatar, holder.tvName, holder.tvDate, holder.tvPosition, holder.tvAction);
        } else {
            holder = (ComponentTaskList_GroupHolder) convertView.getTag();
        }

        return convertView;
    }

    private void setDataItemTaskToView(Task currentTaskItem, Activity mainAct, Context context,
            /* Line 1 */ TextView tvTitle, CircleImageView imgAvatar, TextView tvName,
            /* Line 2 */ TextView tvDate,
            /* Line 3 */ TextView tvPosition, TextView tvAction) {

        User _userItem = null;
        if (!Functions.isNullOrEmpty(currentTaskItem.getTitle())) {
            tvTitle.setText(currentTaskItem.getTitle());
        } else {
            tvTitle.setText("");
        }

        if (Functions.isNullOrEmpty(currentTaskItem.getAssignedId())) {
            imgAvatar.setImageResource(R.drawable.icon_ver2_group);
            if (!Functions.isNullOrEmpty(currentTaskItem.getAssignedName())) {
                tvName.setText(currentTaskItem.getAssignedName());
            } else {
                tvName.setText("");
            }
        } else {
            // phân công User
            Group group = new RealmController().getRealm()
                    .where(Group.class)
                    .equalTo("ID", String.valueOf(currentTaskItem.getAssignedId()))
                    .findFirst();

            if (group != null) {
                imgAvatar.setImageResource(R.drawable.icon_ver2_group);
                if (!Functions.isNullOrEmpty(currentTaskItem.getAssignedName())) {
                    tvName.setText(currentTaskItem.getAssignedName());
                } else {
                    tvName.setText("");
                }
            } else {
                User user = new RealmController().getRealm()
                        .where(User.class)
                        .equalTo("ID", String.valueOf(currentTaskItem.getAssignedId()))
                        .findFirst();
                if (user != null) {
                    _userItem = user;
                    if (!Functions.isNullOrEmpty(_userItem.getImagePath())) {
                        ImageLoader.getInstance().loadImageUserWithToken(mainAct, Constants.BASE_URL + _userItem.getImagePath(), imgAvatar);
                    } else {
                        imgAvatar.setImageResource(R.drawable.icon_avatar64);
                    }

                    if (!Functions.isNullOrEmpty(currentTaskItem.getAssignedName())) {
                        if (currentTaskItem.getAssignedName().contains("+")) {
                            tvName.setText(currentTaskItem.getAssignedName());
                        } else {
                            if (!Functions.isNullOrEmpty(_userItem.getFullName())) {
                                tvName.setText(_userItem.getFullName());
                            }
                        }
                    } else {
                        tvName.setText("");
                    }
                }
            }
        }

        if (!Functions.isNullOrEmpty(currentTaskItem.getDueDate())) {
            tvDate.setText(Functions.share.formatDateLanguage(currentTaskItem.getDueDate()));
            tvDate.setTextColor(Functions.share.getColorByDueDate(currentTaskItem.getDueDate()));
        } else {
            tvDate.setText("");
        }

        if (_userItem != null && !Functions.isNullOrEmpty(_userItem.getPosition())) {
            tvPosition.setText(_userItem.getPosition());
        } else {
            tvPosition.setText("");
        }

        tvAction.setText(DetailFunc.share.getStatusNameByID(currentTaskItem.getStatus()));
        tvAction.setBackgroundTintList(ColorStateList.valueOf(DetailFunc.share.getStatusColorByID(context, currentTaskItem.getStatus())));
    }

    @Override
    public View getChildView(int groupPosition, int childPosition, boolean isLastChild, View convertView, ViewGroup parent) {
        // chỉ đổ thằng đầu tiên
        if (childPosition == 0) {
            ArrayList<ExpandTask> currentLstChild = lstGroupTask.get(groupPosition).getLstChild();

            ComponentTaskList_ChildHolder holder;
            convertView = LayoutInflater.from(context).inflate(R.layout.item_control_task_list_child, null);
            holder = new ComponentTaskList_ChildHolder();
            holder.lnAll = convertView.findViewById(R.id.ln_ItemControlTaskList_All);
            holder.expandChild = convertView.findViewById(R.id.expand_ItemControlTaskList_Child);

            holder.expandChild.setGroupIndicator(null);
            holder.expandChild.setChildIndicator(null);
            holder.expandChild.setForeground(null);
            holder.expandChild.setDividerHeight(0);
            convertView.setTag(holder);

            if (currentLstChild != null && currentLstChild.size() > 0) {
                GradientDrawable drawable = new GradientDrawable();
                if (groupPosition % 2 == 0) {
                    //{mTopLeftRadius, mTopLeftRadius, mTopRightRadius, mTopRightRadius, mBottomRightRadius, mBottomRightRadius, mBottomLeftRadius, mBottomLeftRadius}
                    drawable.setCornerRadii(new float[]{0, 0, 0, 0, 10, 10, 10, 10});
                    drawable.setShape(GradientDrawable.RECTANGLE);
                    drawable.setColor(ContextCompat.getColor(context, R.color.clVer2BlueNavigation));
                    holder.lnAll.setBackground(drawable);
                } else {
                    drawable.setColor(ContextCompat.getColor(context, R.color.clWhite));
                    holder.lnAll.setBackground(drawable);
                }

                ExpandComponentTaskList_ChildAdapter adapter = new ExpandComponentTaskList_ChildAdapter(mainAct, context, currentLstChild, this, childPosition == (lstGroupTask.get(groupPosition).getLstChild().size() - 1) ? true : false);
                holder.expandChild.setVisibility(View.VISIBLE);
                holder.expandChild.setAdapter(adapter);
                holder.expandChild.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, itemHeight * adapter.getGroupCount()));

                holder.expandChild.setOnGroupCollapseListener(groupPosition13 -> holder.expandChild.getLayoutParams().height -= adapter.getChildrenCount(groupPosition13) * itemHeight);

                holder.expandChild.setOnGroupExpandListener(groupPosition12 -> holder.expandChild.getLayoutParams().height += adapter.getChildrenCount(groupPosition12) * itemHeight);

                holder.expandChild.setOnChildClickListener((parent1, v, groupPosition1, childPosition1, id) -> {
                    ExpandComponentTaskList_ChildAdapter child = (ExpandComponentTaskList_ChildAdapter) holder.expandChild.getExpandableListAdapter();
                    ExpandTask clickedItem = child.getItemChildClick(groupPosition1, childPosition1);
                    Intent intent = new Intent(mainAct, DetailCreateTaskActivity.class);
                    //intent.setAction(VarsReceiver.TASK_CLICK);
                    intent.putExtra("WorkflowItemId", clickedItem.getGroupItem().getSPItemId());
                    intent.putExtra("taskId", clickedItem.getGroupItem().getID());
                    intent.putExtra("isClickFromAction", false);
                    mainAct.startActivity(intent);
                    //BroadcastUtility.send(mainAct, intent);
                    return true;
                });

                for (int i = 0; i < adapter.getGroupCount(); i++) {
                    holder.expandChild.expandGroup(i);
                }

            } else {
                holder.expandChild.setVisibility(View.GONE);
            }
            return convertView;
        } else {
            return new View(context);
        }
    }

    @Override
    public boolean isChildSelectable(int groupPosition, int childPosition) {
        return true;
    }

    public class ComponentTaskList_ChildHolder {
        public LinearLayout lnAll;
        public ExpandableListView expandChild;
    }

    public class ComponentTaskList_GroupHolder {
        public LinearLayout lnAll;
        public LinearLayout lnContentClick;
        public View vwMarginTop;
        public View vwGroupLine;
        public RelativeLayout relaGroup;
        public TextView tvTitle;
        public ImageView imgCollapsed;
        public CircleImageView imgAvatar;
        public TextView tvName;
        public TextView tvDate;
        public TextView tvPosition;
        public TextView tvAction;
    }

    public class ExpandComponentTaskList_ChildAdapter extends BaseExpandableListAdapter {
        private Activity mainAct;
        private Context context;
        private ArrayList<ExpandTask> lstGroupTask;

        public ExpandComponentTaskList_ChildAdapter(Activity mainAct, Context context, ArrayList<ExpandTask> lstGroupTask, ExpandComponentTaskListAdapter adapterParent, boolean isLastChild) {
            this.mainAct = mainAct;
            this.context = context;
            this.lstGroupTask = lstGroupTask;
        }

        public ExpandTask getItemChildClick(int groupPosition, int childPosition) {
            return lstGroupTask.get(groupPosition).getLstChild().get(childPosition);
        }

        @Override
        public int getGroupCount() {
            return lstGroupTask.size();
        }

        @Override
        public int getChildrenCount(int groupPosition) {
            if (lstGroupTask.get(groupPosition).getLstChild() != null &&
                    lstGroupTask.get(groupPosition).getLstChild().size() > 0) {
                return lstGroupTask.get(groupPosition).getLstChild().size();
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
            ComponentTaskListChild_Holder_Lv1 holder;
            ExpandTask currentItem = lstGroupTask.get(groupPosition);
            Task currentTaskItem = currentItem.getGroupItem();

            if (convertView == null) {
                convertView = LayoutInflater.from(context).inflate(R.layout.item_control_task_list_child_lv1, null);
                holder = new ComponentTaskListChild_Holder_Lv1();
                holder.lnContentClick = convertView.findViewById(R.id.ln_ItemControlTaskList_Child_Lv1_ContentClick);
                holder.vwMarginTop = convertView.findViewById(R.id.vw_ItemControlTaskList_Child_Lv1_MarginTop);
                holder.vwGroupHorizon = convertView.findViewById(R.id.vw_ItemControlTaskList_Child_Lv1_GroupHorizon);
                holder.lnGroup1 = convertView.findViewById(R.id.ln_ItemControlTaskList_Child_Lv1_Group1);
                holder.vwGroup1 = convertView.findViewById(R.id.vw_ItemControlTaskList_Child_Lv1_Group1);
                holder.lnGroup2 = convertView.findViewById(R.id.ln_ItemControlTaskList_Child_Lv1_Group2);
                holder.vwGroup2 = convertView.findViewById(R.id.vw_ItemControlTaskList_Child_Lv1_Group2);
                holder.imgGroup2 = convertView.findViewById(R.id.img_ItemControlTaskList_Child_Lv1_Group2);
                holder.tvTitle = convertView.findViewById(R.id.tv_ItemControlTaskList_Child_Lv1_Title);
                holder.imgAvatar = convertView.findViewById(R.id.img_ItemControlTaskList_Child_Lv1_Avata);
                holder.tvName = convertView.findViewById(R.id.tv_ItemControlTaskList_Child_Lv1_Name);
                holder.tvDate = convertView.findViewById(R.id.tv_ItemControlTaskList_Child_Lv1_Date);
                holder.tvPosition = convertView.findViewById(R.id.tv_ItemControlTaskList_Child_Lv1_Position);
                holder.tvAction = convertView.findViewById(R.id.tv_ItemControlTaskList_Child_Lv1_Action);

                convertView.setTag(holder);


            } else {
                holder = (ComponentTaskListChild_Holder_Lv1) convertView.getTag();
            }

            if (isExpanded) {
                holder.vwGroup2.setVisibility(View.VISIBLE);
                holder.imgGroup2.setImageResource(R.drawable.icon_ver2_show);
            } else {
                holder.vwGroup2.setVisibility(View.INVISIBLE);
                holder.imgGroup2.setImageResource(R.drawable.icon_ver2_close);
            }

            if (currentItem.getLstChild() != null && currentItem.getLstChild().size() > 0) {
                holder.imgGroup2.setVisibility(View.VISIBLE);
                holder.lnGroup2.setVisibility(View.VISIBLE);

                holder.vwGroupHorizon.setLayoutParams(new LinearLayout.LayoutParams(Functions.share.convertDpToPixel(10, context), Functions.share.convertDpToPixel(1.5f, context)));
            } else {
                holder.imgGroup2.setVisibility(View.GONE);
                holder.lnGroup2.setVisibility(View.GONE);

                holder.vwGroupHorizon.setLayoutParams(new LinearLayout.LayoutParams(Functions.share.convertDpToPixel(30, context), Functions.share.convertDpToPixel(1.5f, context)));
            }

            if (groupPosition == lstGroupTask.size() - 1) {
                holder.vwGroup1.setLayoutParams(new LinearLayout.LayoutParams(Functions.share.convertDpToPixel(1.5f, context), Functions.share.convertDpToPixel(17, context)));
            } else {
                holder.vwGroup1.setLayoutParams(new LinearLayout.LayoutParams(Functions.share.convertDpToPixel(1.5f, context), LinearLayout.LayoutParams.MATCH_PARENT));
            }

            setDataItemTaskToView(currentTaskItem, mainAct, context, holder.tvTitle, holder.imgAvatar,
                    holder.tvName, holder.tvDate, holder.tvPosition, holder.tvAction);

            holder.lnContentClick.setOnClickListener(v -> {
                Intent intent = new Intent(mainAct, DetailCreateTaskActivity.class);
                //intent.setAction(VarsReceiver.TASK_CLICK);
                intent.putExtra("WorkflowItemId", currentItem.getGroupItem().getSPItemId());
                intent.putExtra("taskId", currentItem.getGroupItem().getID());
                intent.putExtra("isClickFromAction", false);
                mainAct.startActivity(intent);
                //BroadcastUtility.send(mainAct, intent);
            });

            return convertView;
        }

        @Override
        public View getChildView(int groupPosition, int childPosition, boolean isLastChild, View convertView, ViewGroup parent) {
            ComponentTaskListChild_Holder_Lv2 holder;
            Task currentTaskItem = lstGroupTask.get(groupPosition).getLstChild().get(childPosition).getGroupItem();

            if (convertView == null) {
                convertView = LayoutInflater.from(context).inflate(R.layout.item_control_task_list_child_lv2, null);
                holder = new ComponentTaskListChild_Holder_Lv2();
                holder.vwMarginTop = convertView.findViewById(R.id.vw_ItemControlTaskList_Child_Lv2_MarginTop);
                holder.vwGroupHorizon = convertView.findViewById(R.id.vw_ItemControlTaskList_Child_Lv2_GroupHorizon);
                holder.lnGroup1 = convertView.findViewById(R.id.ln_ItemControlTaskList_Child_Lv2_Group1);
                holder.vwGroup1 = convertView.findViewById(R.id.vw_ItemControlTaskList_Child_Lv2_Group1);
                holder.lnGroup2 = convertView.findViewById(R.id.ln_ItemControlTaskList_Child_Lv2_Group2);
                holder.vwGroup2 = convertView.findViewById(R.id.vw_ItemControlTaskList_Child_Lv2_Group2);
                holder.tvTitle = convertView.findViewById(R.id.tv_ItemControlTaskList_Child_Lv2_Title);
                holder.imgAvatar = convertView.findViewById(R.id.img_ItemControlTaskList_Child_Lv2_Avata);
                holder.tvName = convertView.findViewById(R.id.tv_ItemControlTaskList_Child_Lv2_Name);
                holder.tvDate = convertView.findViewById(R.id.tv_ItemControlTaskList_Child_Lv2_Date);
                holder.tvPosition = convertView.findViewById(R.id.tv_ItemControlTaskList_Child_Lv2_Position);
                holder.tvAction = convertView.findViewById(R.id.tv_ItemControlTaskList_Child_Lv2_Action);
                holder.vwItemSpacing = convertView.findViewById(R.id.vw_ItemControlTaskList_Child_Lv2_ItemSpacing);

                convertView.setTag(holder);

            } else {
                holder = (ComponentTaskListChild_Holder_Lv2) convertView.getTag();
            }

            if (childPosition == lstGroupTask.get(groupPosition).getLstChild().size() - 1) {
                holder.vwGroup2.setLayoutParams(new LinearLayout.LayoutParams(Functions.share.convertDpToPixel(1.5f, context), Functions.share.convertDpToPixel(10, context)));
                holder.vwItemSpacing.setVisibility(View.INVISIBLE);
            } else {
                holder.vwGroup2.setLayoutParams(new LinearLayout.LayoutParams(Functions.share.convertDpToPixel(1.5f, context), LinearLayout.LayoutParams.MATCH_PARENT));
                holder.vwItemSpacing.setVisibility(View.VISIBLE);
            }

            if (groupPosition == lstGroupTask.size() - 1) {
                holder.vwGroup1.setVisibility(View.GONE);
            } else {
                holder.vwGroup1.setVisibility(View.VISIBLE);
            }

            setDataItemTaskToView(currentTaskItem, mainAct, context, holder.tvTitle, holder.imgAvatar,
                    holder.tvName, holder.tvDate, holder.tvPosition, holder.tvAction);

            return convertView;
        }

        @Override
        public boolean isChildSelectable(int groupPosition, int childPosition) {
            return true;
        }
    }

    public class ComponentTaskListChild_Holder_Lv1 {
        public LinearLayout lnContentClick;
        public View vwMarginTop;
        public View vwGroupHorizon;
        public LinearLayout lnGroup1;
        public View vwGroup1;
        public LinearLayout lnGroup2;
        public View vwGroup2;
        public ImageView imgGroup2;

        public TextView tvTitle;
        public CircleImageView imgAvatar;
        public TextView tvName;
        public TextView tvDate;
        public TextView tvPosition;
        public TextView tvAction;
    }

    public class ComponentTaskListChild_Holder_Lv2 {
        public View vwMarginTop;
        public View vwGroupHorizon;
        public LinearLayout lnGroup1;
        public View vwGroup1;
        public LinearLayout lnGroup2;
        public View vwGroup2;
        public TextView tvTitle;
        public CircleImageView imgAvatar;
        public TextView tvName;
        public TextView tvDate;
        public TextView tvPosition;
        public TextView tvAction;
        public View vwItemSpacing;
    }
}

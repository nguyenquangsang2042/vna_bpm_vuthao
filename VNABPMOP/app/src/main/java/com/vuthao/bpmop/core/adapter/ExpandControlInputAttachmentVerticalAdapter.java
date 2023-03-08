package com.vuthao.bpmop.core.adapter;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.recyclerview.widget.StaggeredGridLayoutManager;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.SwipeHelper;
import com.vuthao.bpmop.base.custom.expandable.AnimatedExpandableListAdapter;
import com.vuthao.bpmop.base.model.custom.AttachFile;
import com.vuthao.bpmop.base.model.custom.GroupAttachFile;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.core.Vars;
import com.vuthao.bpmop.core.controller.ControllerDetailAttachFile;
import com.vuthao.bpmop.detail.custom.DetailFunc;

import java.util.ArrayList;
import java.util.List;

public class ExpandControlInputAttachmentVerticalAdapter extends AnimatedExpandableListAdapter {
    private Activity mainAct;
    private Context context;
    private ArrayList<GroupAttachFile> lstGroupAttachFile;
    private ArrayList<AttachFile> files;
    private ViewElement element;
    private int flagView;
    private final ControllerDetailAttachFile CTRLDetailAttachFile = new ControllerDetailAttachFile();

    public ExpandControlInputAttachmentVerticalAdapter(Context context, Activity activity, ArrayList<GroupAttachFile> lstGroupAttachFile, ArrayList<AttachFile> files, ViewElement element, int flagView) {
        this.context = context;
        this.mainAct = activity;
        this.lstGroupAttachFile = lstGroupAttachFile;
        this.files = files;
        this.element = element;
        this.flagView = flagView;
    }

    @Override
    public int getGroupCount() {
        return lstGroupAttachFile.size();
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

    @SuppressLint({"SetTextI18n", "DefaultLocale"})
    @Override
    public View getGroupView(int groupPosition, boolean isExpanded, View convertView, ViewGroup parent) {
        View rootView = convertView;
        LayoutInflater inflater = LayoutInflater.from(context);
        rootView = inflater.inflate(R.layout.item_control_input_attachment_vertical_group, null);
        ImageView imgExpand = rootView.findViewById(R.id.img_ItemControlInputAttachmentVerticalGroup_Expand);
        TextView tvtitle = rootView.findViewById(R.id.tv_ItemControlInputAttachmentVerticalGroup_Title);

        if (isExpanded) {
            imgExpand.setRotation(-90);
        } else {
            imgExpand.setRotation(-180);
        }

        if (!Functions.isNullOrEmpty(lstGroupAttachFile.get(groupPosition).getCategory())) {
            if (lstGroupAttachFile.get(groupPosition).getCategory().contains(";#")) {
                tvtitle.setText(lstGroupAttachFile.get(groupPosition).getCategory().split(";#")[0]);
            } else {
                tvtitle.setText(lstGroupAttachFile.get(groupPosition).getCategory());
            }

            if (lstGroupAttachFile.get(groupPosition).getAttachFiles() != null && lstGroupAttachFile.get(groupPosition).getAttachFiles().size() > 0) {
                tvtitle.setText(tvtitle.getText() + String.format(" (%d)", lstGroupAttachFile.get(groupPosition).getAttachFiles().size()));
            }
        } else {
            tvtitle.setText("");
        }

        return rootView;
    }

    @Override
    public View getRealChildView(int groupPosition, int childPosition, boolean isLastChild, View convertView, ViewGroup parent) {
        View rootView = convertView;
        LayoutInflater inflater = LayoutInflater.from(context);
        rootView = inflater.inflate(R.layout.item_control_input_attachment_vertical_child, null);

        // chỉ load cho item đầu tiên, mấy item sau ẩn
        if (childPosition == 0) {
            RecyclerView recyListAttach = rootView.findViewById(R.id.recy_ItemControlInputAttachmentVerticalChild);
            ControlAttachmentImportAdapter adapter = new ControlAttachmentImportAdapter(mainAct, context, lstGroupAttachFile.get(groupPosition).getAttachFiles(), files, element, flagView);
            StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayoutManager.VERTICAL);
            recyListAttach.setLayoutManager(staggeredGridLayoutManager);
            recyListAttach.setAdapter(adapter);

            if (element.isEnable()) {
                if (flagView == Vars.FlagViewControlAttachment.DetailWorkflow || flagView == Vars.FlagViewControlAttachment.DetailCreateTask) {
                    int buttonWidth = (int) (mainAct.getResources().getDisplayMetrics().widthPixels * 0.15);
                    new SwipeHelper(context, recyListAttach, buttonWidth) {
                        @Override
                        public void instantiateUnderlayButton(RecyclerView.ViewHolder viewHolder, List<UnderlayButton> underlayButtons) {
                            underlayButtons.add(new UnderlayButton(context, "Delete", R.drawable.icon_ver2_star_controlattch_delete, "#EB342E", buttonWidth / 3, pos -> {
                                ArrayList<AttachFile> lstAttachFileFull = new Gson().fromJson(element.getValue(), new TypeToken<ArrayList<AttachFile>>() {
                                }.getType());

                                ArrayList<GroupAttachFile> lstGroup = CTRLDetailAttachFile.cloneListAttachFiles(lstAttachFileFull);
                                AttachFile clickItem = lstGroup.get(groupPosition).getAttachFiles().get(pos);
                                pos = DetailFunc.share.findIndexOfItemInListAttach(clickItem, lstAttachFileFull);
                                if (pos != -1) {
                                    Intent intent = new Intent();
                                    intent.setAction(VarsReceiver.INNERACTIONCLICK);
                                    intent.putExtra("element", new Gson().toJson(element));
                                    intent.putExtra("actionId", Vars.ControlInputAttachmentVertical_InnerActionID.Delete);
                                    intent.putExtra("positionToAction", pos);
                                    intent.putExtra("flagViewID", flagView);
                                    BroadcastUtility.send(mainAct, intent);
                                }
                            }));

                            underlayButtons.add(new UnderlayButton(context, "Update", R.drawable.icon_ver2_star_controlattch_edit, "#335FB3", buttonWidth / 3, new UnderlayButtonClickListener() {
                                @Override
                                public void onClick(int pos) {
                                    ArrayList<AttachFile> lstAttachFileFull = new Gson().fromJson(element.getValue(), new TypeToken<ArrayList<AttachFile>>() {
                                    }.getType());

                                    ArrayList<GroupAttachFile> lstGroup = CTRLDetailAttachFile.cloneListAttachFiles(lstAttachFileFull);
                                    AttachFile clickItem = lstGroup.get(groupPosition).getAttachFiles().get(pos);
                                    pos = DetailFunc.share.findIndexOfItemInListAttach(clickItem, lstAttachFileFull);
                                    if (pos != -1) {
                                        Intent intent = new Intent();
                                        intent.setAction(VarsReceiver.INNERACTIONCLICK);
                                        intent.putExtra("element", new Gson().toJson(element));
                                        intent.putExtra("actionId", Vars.ControlInputAttachmentVertical_InnerActionID.Edit);
                                        intent.putExtra("positionToAction", pos);
                                        intent.putExtra("flagViewID", flagView);
                                        BroadcastUtility.send(mainAct, intent);
                                    }
                                }
                            }));
                        }

                        @Override
                        public int getSwipeDirs(@NonNull RecyclerView recyclerView, @NonNull RecyclerView.ViewHolder viewHolder) {
                            int pos = viewHolder.getAdapterPosition();
                            if (lstGroupAttachFile.get(groupPosition).getAttachFiles().get(pos).isAuthor()) {
                                return super.getSwipeDirs(recyclerView, viewHolder);
                            } else {
                                return 0;
                            }
                        }
                    };
                }
            }
        }

        return rootView;
    }

    @Override
    public int getRealChildrenCount(int groupPosition) {
        if (lstGroupAttachFile.get(groupPosition).getAttachFiles() != null) {
            return lstGroupAttachFile.get(groupPosition).getAttachFiles().size();
        } else {
            return 0;
        }
    }

    @Override
    public boolean isChildSelectable(int groupPosition, int childPosition) {
        return false;
    }
}

package com.vuthao.bpmop.core.adapter;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.RecyclerView;

import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.custom.AttachFile;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.core.Vars;
import com.vuthao.bpmop.core.controller.ControllerDetailAttachFile;
import com.vuthao.bpmop.detail.custom.DetailFunc;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;

public class ControlAttachmentImportAdapter extends RecyclerView.Adapter<ControlAttachmentImportAdapter.ControlAttachmentImportHolder> {
    public Activity mainAct;
    public Context context;
    public ArrayList<AttachFile> lstAttachment;
    public int recyHeight = 0;
    private final ControllerDetailAttachFile CTRLDetailAttachFile = new ControllerDetailAttachFile();
    private ArrayList<AttachFile> files;
    private ViewElement element;
    private int flagView;

    public ControlAttachmentImportAdapter(Activity mainAct, Context context, ArrayList<AttachFile> lstAttachment, ArrayList<AttachFile> files, ViewElement element, int flagView) {
        this.mainAct = mainAct;
        this.context = context;
        this.lstAttachment = lstAttachment;
        this.files = files;
        this.element = element;
        this.flagView = flagView;
    }

    public int getRecyHeight() {
        return recyHeight;
    }

    @NonNull
    @Override
    public ControlAttachmentImportHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_control_input_attachment_vertical, parent, false);
        return new ControlAttachmentImportHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull ControlAttachmentImportHolder holder, int position) {
        if (position % 2 == 0) {
            holder.lnAll.setBackgroundColor(ContextCompat.getColor(context, R.color.clVer2BlueNavigation));
        } else {
            holder.lnAll.setBackgroundColor(ContextCompat.getColor(context, R.color.clWhite));
        }

        AttachFile file = lstAttachment.get(position);

        if (file != null) {
            if (Functions.isNullOrEmpty(file.getID())) {
                holder.imgNewFile.setVisibility(View.VISIBLE);
            } else {
                holder.imgNewFile.setVisibility(View.GONE);
            }

            holder.imgExtension.setImageResource(CTRLDetailAttachFile.getResourceIDAttachment(file.getPath()));

            if (!Functions.isNullOrEmpty(file.getTitle())) {
                if (file.getTitle().contains(";#")) {
                    holder.tvName.setText(file.getTitle().split(";#")[0]);
                } else {
                    holder.tvName.setText(file.getTitle());
                }
            } else {
                holder.tvName.setText("");
            }

            holder.tvSize.setText(Functions.share.getFormatFileSize(file.getSize()));
            holder.tvCategory.setText(file.getCreatedName());
            holder.tvPosition.setText(file.getCreatedPositon());

            holder.lnAll.setOnClickListener(v -> {
                int index = DetailFunc.share.findIndexOfItemInListAttach(file, files);
                if (index != -1) {
                    Intent intent = new Intent();
                    intent.setAction(VarsReceiver.INNERACTIONCLICK);
                    intent.putExtra("element", new Gson().toJson(element));
                    intent.putExtra("actionId", Vars.ControlInputAttachmentVertical_InnerActionID.View);
                    intent.putExtra("positionToAction", index);
                    intent.putExtra("flagViewID", flagView);
                    BroadcastUtility.send(mainAct, intent);
                }
            });
        }
    }

    @Override
    public int getItemCount() {
        return lstAttachment.size();
    }

    public class ControlAttachmentImportHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.tv_ItemControlInputAttachmentVertical_Name)
        TextView tvName;
        @BindView(R.id.tv_ItemControlInputAttachmentVertical_Size)
        TextView tvSize;
        @BindView(R.id.img_ItemControlInputAttachmentVertical_Extension)
        ImageView imgExtension;
        @BindView(R.id.img_ItemControlInputAttachmentVertical_NewFile)
        ImageView imgNewFile;
        @BindView(R.id.tv_ItemControlInputAttachmentVertical_Category)
        TextView tvCategory;
        @BindView(R.id.ln_ItemControlInputAttachmentVertical_Name)
        LinearLayout lnName;
        @BindView(R.id.ln_ItemControlInputAttachmentVertical_Category)
        LinearLayout lnCategory;
        @BindView(R.id.ln_ItemControlInputAttachmentVertical)
        LinearLayout lnAll;
        @BindView(R.id.tv_ItemControlInputAttachmentVertical_Position)
        TextView tvPosition;

        public ControlAttachmentImportHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
            lnAll.setOnClickListener(v -> {
            });
        }
    }
}


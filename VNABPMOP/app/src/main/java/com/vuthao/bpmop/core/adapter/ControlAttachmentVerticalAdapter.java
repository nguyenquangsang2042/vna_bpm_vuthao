package com.vuthao.bpmop.core.adapter;

import android.app.Activity;
import android.util.Pair;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.core.component.ControlAttachmentVertical;
import com.vuthao.bpmop.core.component.ControlBase;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;

public class ControlAttachmentVerticalAdapter extends RecyclerView.Adapter<ControlAttachmentVerticalAdapter.ControlAttachmentVerticalHolder> {
    private ControlBase.ControlAttachmentVerticalListener listener;
    public Activity mainAct;
    public ArrayList<Pair<String, String>> lstAttachment;

    public ControlAttachmentVerticalAdapter(ControlBase.ControlAttachmentVerticalListener listener, Activity mainAct, ArrayList<Pair<String, String>> lstAttachment) {
        this.listener = listener;
        this.mainAct = mainAct;
        this.lstAttachment = lstAttachment;
    }

    @NonNull
    @Override
    public ControlAttachmentVerticalHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_control_attachment_vertical, parent, false);
        return new ControlAttachmentVerticalHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull ControlAttachmentVerticalHolder holder, int position) {
        Pair<String, String> item = lstAttachment.get(position);
        if(!Functions.isNullOrEmpty(item.second)) {
            holder.tvFileName.setText(item.second);
        }
    }

    @Override
    public int getItemCount() {
        return lstAttachment.size();
    }

    public class ControlAttachmentVerticalHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.img_ItemControlAttachmentVertical_Avatar)
        ImageView imgAvatar;
        @BindView(R.id.tv_ItemControlAttachmentVertical_FileName)
        TextView tvFileName;

        public ControlAttachmentVerticalHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
        }
    }
}


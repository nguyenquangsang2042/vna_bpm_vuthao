package com.vuthao.bpmop.core.adapter;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.localbroadcastmanager.content.LocalBroadcastManager;
import androidx.recyclerview.widget.RecyclerView;

import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.DownloadFile;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.custom.AttachFile;
import com.vuthao.bpmop.detail.custom.DetailFunc;

import java.io.File;
import java.util.ArrayList;
import java.util.stream.Collectors;

import butterknife.BindView;
import butterknife.ButterKnife;

public class ParentAttachFileAdapter extends RecyclerView.Adapter<ParentAttachFileAdapter.ParentAttachFileHolder> {
    private Activity mainAct;
    private Context context;
    private ArrayList<AttachFile> lstData;
    private boolean showImgDelete;
    private boolean isReply;

    public ParentAttachFileAdapter(Activity mainAct, Context context, ArrayList<AttachFile> lstData
            , boolean showImgDelete, boolean isReply) {
        this.mainAct = mainAct;
        this.context = context;
        this.showImgDelete = showImgDelete;
        this.isReply = isReply;
        this.lstData = (ArrayList<AttachFile>) lstData.stream().filter(r -> !r.isImage()).collect(Collectors.toList());
    }

    public void updateListAttach(ArrayList<AttachFile> lstData) {
        this.lstData = (ArrayList<AttachFile>) lstData.stream().filter(r -> !r.isImage()).collect(Collectors.toList());
    }

    @NonNull
    @Override
    public ParentAttachFileHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_component_comment_attach_parent, parent, false);
        return new ParentAttachFileHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull ParentAttachFileAdapter.ParentAttachFileHolder holder, int position) {
        AttachFile file = lstData.get(position);

        if (file != null) {
            if (!Functions.isNullOrEmpty(file.getUrl())) {
                holder.imgExtension.setImageResource(DetailFunc.share.getResourceIDAttachment(file.getUrl()));
            } else {
                holder.imgExtension.setImageResource(DetailFunc.share.getResourceIDAttachment(file.getPath()));
            }

            if (!Functions.isNullOrEmpty(file.getTitle())) {
                holder.tvTitle.setText(DetailFunc.share.getFormatTitleFile(file.getTitle()));
            } else {
                holder.tvTitle.setText("");
            }

            if (showImgDelete) {
                holder.imgDelete.setVisibility(View.VISIBLE);
            } else {
                holder.imgDelete.setVisibility(View.GONE);
            }

            holder.tvTitle.setOnClickListener(v -> {
                if (!Functions.isNullOrEmpty(file.getUrl())) {
                    new DownloadFile().execute(Constants.BASE_URL + file.getUrl() + ";#" + file.getTitle() + "." + file.getExtension());
                } else {
                    if (new File(file.getPath()).exists()) {
                        DetailFunc.share.openFile(mainAct, file.getPath());
                    }
                }
            });

            holder.imgDelete.setOnClickListener(v -> {
                Intent intent = new Intent();
                if (isReply) {
                    intent.setAction("REPLYCOMMENT");
                } else {
                    intent.setAction("COMMENT");
                }

                intent.putExtra("type", "delete");
                intent.putExtra("obj", new Gson().toJson(file));
                BroadcastUtility.send(mainAct, intent);
            });
        }
    }

    @Override
    public int getItemCount() {
        return this.lstData.size();
    }

    public class ParentAttachFileHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.tv_ItemComponentComment_AttachParent_Title)
        TextView tvTitle;
        @BindView(R.id.img_ItemComponentComment_AttachParent_Delete)
        ImageView imgDelete;
        @BindView(R.id.img_ItemComponentComment_AttachParent_Extension)
        ImageView imgExtension;

        public ParentAttachFileHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
        }
    }
}

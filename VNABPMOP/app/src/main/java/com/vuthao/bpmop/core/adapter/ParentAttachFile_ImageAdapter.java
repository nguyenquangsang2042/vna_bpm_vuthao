package com.vuthao.bpmop.core.adapter;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;

import androidx.annotation.NonNull;
import androidx.localbroadcastmanager.content.LocalBroadcastManager;
import androidx.recyclerview.widget.RecyclerView;

import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.DownloadFile;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.ImageLoader;
import com.vuthao.bpmop.base.model.custom.AttachFile;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.detail.custom.DetailFunc;

import java.io.File;
import java.util.ArrayList;
import java.util.stream.Collectors;

import butterknife.BindView;
import butterknife.ButterKnife;

public class ParentAttachFile_ImageAdapter extends RecyclerView.Adapter<ParentAttachFile_ImageAdapter.CommentAttachFile_ImageHolder> {
    private Activity mainAct;
    private Context context;
    private ArrayList<AttachFile> lstData;
    private boolean isReply;
    private boolean showImgDelete;

    public ParentAttachFile_ImageAdapter(Activity mainAct, Context context, ArrayList<AttachFile> lstData
            , boolean showImgDelete, boolean isReply) {
        this.mainAct = mainAct;
        this.context = context;
        this.showImgDelete = showImgDelete;
        this.isReply = isReply;

        this.lstData = (ArrayList<AttachFile>) lstData.stream().filter(AttachFile::isImage).collect(Collectors.toList());
    }

    public void updateListAttach(ArrayList<AttachFile> lstData) {
        this.lstData = (ArrayList<AttachFile>) lstData.stream().filter(AttachFile::isImage).collect(Collectors.toList());
    }

    @NonNull
    @Override
    public CommentAttachFile_ImageHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_component_comment_attach_parent_image, parent,false);
        return new CommentAttachFile_ImageHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull ParentAttachFile_ImageAdapter.CommentAttachFile_ImageHolder holder, int position) {
        AttachFile file = lstData.get(position);
        if (file != null) {
            if (showImgDelete) {
                holder.imgDelete.setVisibility(View.VISIBLE);
            } else {
                holder.imgDelete.setVisibility(View.GONE);
            }

            // offline từ máy
            if (!Functions.isNullOrEmpty(file.getPath())) {
                ImageLoader.getInstance().loadImageUser(context, Uri.fromFile(new File(file.getPath())), holder.imageContent);
            } else {
                ImageLoader.getInstance().loadImageUserWithToken(context, Constants.BASE_URL + file.getUrl(), holder.imageContent);
            }

            holder.imageContent.setOnClickListener(v -> {
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
                    intent.setAction(VarsReceiver.REPLYCOMMENT);
                } else {
                    intent.setAction(VarsReceiver.COMMENT);
                }

                intent.putExtra("type", "delete");
                intent.putExtra("obj", new Gson().toJson(file));
                BroadcastUtility.send(mainAct, intent);
            });
        }
    }

    @Override
    public int getItemCount() {
        return lstData.size();
    }

    public class CommentAttachFile_ImageHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.img_ItemComponentComment_AttachParent_Image_Delete)
        ImageView imgDelete;
        @BindView(R.id.img_ItemComponentComment_AttachParent_Image)
        ImageView imageContent;

        public CommentAttachFile_ImageHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
        }
    }
}

package com.vuthao.bpmop.shareview.adapter;

import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.RecyclerView;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.custom.DownloadedFiles;
import com.vuthao.bpmop.core.controller.ControllerDetailAttachFile;

import java.io.File;
import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;

public class DownloadedFilesAdapter extends RecyclerView.Adapter<DownloadedFilesAdapter.DownloadedFilesHolder>  {
    private Activity activity;
    private ArrayList<DownloadedFiles> files;
    private DownloadedFilesListener listener;
    private final ControllerDetailAttachFile CTRLDetailAttachFile = new ControllerDetailAttachFile();

    public interface DownloadedFilesListener {
        void OnItemClick(DownloadedFiles file);
    }

    public DownloadedFilesAdapter(Activity activity, ArrayList<DownloadedFiles> files, DownloadedFilesListener listener) {
        this.activity = activity;
        this.files = files;
        this.listener = listener;
    }

    @NonNull
    @Override
    public DownloadedFilesHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_downloaded_file, parent,false);
        return new DownloadedFilesHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull DownloadedFilesHolder holder, int position) {
        if (position % 2 == 0) {
            holder.lnAll.setBackgroundColor(ContextCompat.getColor(activity, R.color.clVer2BlueNavigation));
        } else {
            holder.lnAll.setBackgroundColor(ContextCompat.getColor(activity, R.color.clWhite));
        }

        DownloadedFiles file = files.get(position);
        if (file != null) {
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
        }
    }

    @Override
    public int getItemCount() {
        return files.size();
    }

    public class DownloadedFilesHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.tv_ItemControlInputAttachmentVertical_Name)
        TextView tvName;
        @BindView(R.id.tv_ItemControlInputAttachmentVertical_Size)
        TextView tvSize;
        @BindView(R.id.img_ItemControlInputAttachmentVertical_Extension)
        ImageView imgExtension;
        @BindView(R.id.ln_ItemControlInputAttachmentVertical_Name)
        LinearLayout lnName;
        @BindView(R.id.ln_ItemControlInputAttachmentVertical)
        LinearLayout lnAll;

        public DownloadedFilesHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
            lnAll.setOnClickListener(view -> listener.OnItemClick(files.get(getAdapterPosition())));
        }
    }
}

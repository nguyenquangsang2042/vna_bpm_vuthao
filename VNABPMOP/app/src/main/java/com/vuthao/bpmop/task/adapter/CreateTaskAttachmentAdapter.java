package com.vuthao.bpmop.task.adapter;

import android.app.Activity;
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
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.app.Position;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.model.custom.AttachFile;
import com.vuthao.bpmop.base.model.custom.Task;
import com.vuthao.bpmop.base.realm.RealmController;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;

public class CreateTaskAttachmentAdapter extends RecyclerView.Adapter<CreateTaskAttachmentAdapter.DetailCreateTask_AttachmentHolder> {
    private Activity activity;
    private ArrayList<AttachFile> lstAttachment;
    private int flagUserPermission;
    private Task parentItem;
    private CreateTaskAttachmentListener listener;

    public interface CreateTaskAttachmentListener {
        void OnViewClick(int pos);

        void OnSaveClick(int pos);

        void OnDeleteClick(int pos);
    }

    public CreateTaskAttachmentAdapter(Activity mainAct, ArrayList<AttachFile> lstAttachment, int flagUserPermission, boolean IsClickFromAction, Task parentItem, CreateTaskAttachmentListener listener) {
        this.activity = mainAct;
        this.lstAttachment = lstAttachment;
        this.flagUserPermission = flagUserPermission;
        this.parentItem = parentItem;
        this.listener = listener;
    }

    @NonNull
    @Override
    public DetailCreateTask_AttachmentHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_control_input_attachment_vertical, parent, false);
        return new DetailCreateTask_AttachmentHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull CreateTaskAttachmentAdapter.DetailCreateTask_AttachmentHolder holder, int position) {
        AttachFile file = lstAttachment.get(position);
        if (position % 2 == 0) {
            holder.lnAll.setBackgroundColor(ContextCompat.getColor(activity, R.color.clVer2BlueNavigation));
        } else {
            holder.lnAll.setBackgroundColor(ContextCompat.getColor(activity, R.color.clWhite));
        }

        if (file != null) {
            if (Functions.isNullOrEmpty(file.getID())) {
                holder.imgNewFile.setVisibility(View.VISIBLE);
            } else {
                holder.imgNewFile.setVisibility(View.GONE);
            }

            if (!Functions.isNullOrEmpty(file.getPath())) {
                String exten = file.getPath();
                if (exten != null) {
                    if (exten.toLowerCase().contains(".doc") || exten.toLowerCase().contains(".docx")) {
                        holder.imgExtension.setImageResource(R.drawable.icon_word);
                    } else if (exten.toLowerCase().contains(".txt")) {
                        holder.imgExtension.setImageResource(R.drawable.icon_attachfile_txt);
                    } else if (exten.toLowerCase().contains(".png") || exten.toLowerCase().contains(".jpeg") || exten.toLowerCase().contains(".jpg")) {
                        holder.imgExtension.setImageResource(R.drawable.icon_attachfile_photo);
                    } else if (exten.toLowerCase().contains(".xls") || exten.toLowerCase().contains(".xlsx")) {
                        holder.imgExtension.setImageResource(R.drawable.icon_attachfile_excel);
                    } else if (exten.toLowerCase().contains(".pdf")) {
                        holder.imgExtension.setImageResource(R.drawable.icon_attachfile_pdf);
                    } else if (exten.toLowerCase().contains(".ppt")) {
                        holder.imgExtension.setImageResource(R.drawable.icon_attachfile_ppt);
                    } else {
                        holder.imgExtension.setImageResource(R.drawable.icon_attachfile_other);
                    }
                } else {
                    holder.imgExtension.setImageResource(R.drawable.icon_attachfile_other);
                }
            } else {
                holder.imgExtension.setImageResource(R.drawable.icon_attachfile_other);
            }

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

            if (!Functions.isNullOrEmpty(file.getCreatedBy())) {
                User user = new RealmController().getRealm().where(User.class)
                        .equalTo("ID", file.getCreatedBy().toLowerCase())
                        .findFirst();
                if (user != null) {
                    if (!Functions.isNullOrEmpty(user.getFullName())) {
                        holder.tvCategory.setText(user.getFullName());
                    } else {
                        holder.tvCategory.setText("");
                    }

                    if (!Functions.isNullOrEmpty(user.getPosition())) {
                        holder.tvPosition.setText(user.getPosition());
                    } else {
                        holder.tvPosition.setText(user.getPositionTitle());
                    }
                }
            } else {
                if (!Functions.isNullOrEmpty(file.getCreatedName())) {
                    holder.tvCategory.setText(file.getCreatedName());
                } else {
                    holder.tvCategory.setText("");
                }

                if (!Functions.isNullOrEmpty(file.getCreatedPositon())) {
                    holder.tvPosition.setText(file.getCreatedPositon());
                } else {
                    holder.tvPosition.setText("");
                }
            }
        }
    }

    @Override
    public int getItemCount() {
        return lstAttachment.size();
    }

    public int getFlagUserPermission() {
        return flagUserPermission;
    }

    public void setFlagUserPermission(int flagUserPermission) {
        this.flagUserPermission = flagUserPermission;
    }

    public Task getParentItem() {
        return parentItem;
    }

    public class DetailCreateTask_AttachmentHolder extends RecyclerView.ViewHolder {
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
        @BindView(R.id.ln_ItemControlInputAttachmentVertical)
        LinearLayout lnAll;
        @BindView(R.id.tv_ItemControlInputAttachmentVertical_Position)
        TextView tvPosition;

        public DetailCreateTask_AttachmentHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
            lnAll.setOnClickListener(v -> listener.OnViewClick(getAdapterPosition()));
        }
    }
}

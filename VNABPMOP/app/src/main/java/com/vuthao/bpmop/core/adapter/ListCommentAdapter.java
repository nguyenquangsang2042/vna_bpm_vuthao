package com.vuthao.bpmop.core.adapter;

import android.app.Activity;
import android.content.Context;
import android.content.res.ColorStateList;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.ImageLoader;
import com.vuthao.bpmop.base.model.app.Position;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.model.custom.AttachFile;
import com.vuthao.bpmop.base.model.custom.Comment;
import com.vuthao.bpmop.base.realm.RealmController;
import com.vuthao.bpmop.detail.custom.DetailFunc;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;
import de.hdodenhof.circleimageview.CircleImageView;

public class ListCommentAdapter extends RecyclerView.Adapter<ListCommentAdapter.ListCommentHolder> {
    private Activity mainAct;
    private Context context;
    private ArrayList<Comment> lstData;
    private boolean isReplyView;
    private int widthView = -1;
    private ListCommentListener listener;

    public interface ListCommentListener {
        void OnLikeClick(int pos);
        void OnReplyClick(int pos);
    }

    public ListCommentAdapter(Activity mainAct, Context context, ListCommentListener listener, ArrayList<Comment> lstData, boolean isReplyView, int widthView) {
        this.mainAct = mainAct;
        this.context = context;
        this.lstData = lstData;
        this.isReplyView = isReplyView;
        this.widthView = widthView;
        this.listener = listener;
    }

    public void updateListComment(ArrayList<Comment> comments) {
        lstData.clear();
        lstData.addAll(comments);
        notifyDataSetChanged();
    }
    @NonNull
    @Override
    public ListCommentHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_component_comment_list_comment, parent, false);
        return new ListCommentHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull ListCommentAdapter.ListCommentHolder holder, int position) {
        Comment comment = lstData.get(position);
        if (comment != null) {

            holder.tvReply.setText(Functions.share.getTitle("TEXT_REPLY", "Trả lời"));
            User user = new RealmController().getRealm()
                    .where(User.class)
                    .equalTo("ID", comment.getAuthor().toLowerCase())
                    .findFirst();
            if (!Functions.isNullOrEmpty(comment.getParentCommentId())) {
                holder.vwMarginLeft.setVisibility(View.VISIBLE);
            } else {
                holder.vwMarginLeft.setVisibility(View.GONE);
                holder.lnAction.setVisibility(View.VISIBLE);
            }

            if (user != null) {
                if (!Functions.isNullOrEmpty(user.getImagePath())) {
                    holder.tvAvatar.setVisibility(View.GONE);
                    holder.imgAvatar.setVisibility(View.VISIBLE);
                    ImageLoader.getInstance().loadImageUserWithToken(context, Constants.BASE_URL + user.getImagePath(), holder.imgAvatar);
                } else {
                    holder.tvAvatar.setVisibility(View.VISIBLE);
                    holder.imgAvatar.setVisibility(View.GONE);

                    holder.tvAvatar.setText(Functions.share.getAvatarName(user.getFullName()));
                    holder.tvAvatar.setBackgroundTintList(ColorStateList.valueOf(Functions.share.getColorByUsername(user.getFullName())));
                }

                if (!Functions.isNullOrEmpty(user.getFullName())) {
                    holder.tvName.setText(user.getFullName());
                } else {
                    holder.tvName.setText("");
                }

                if (!Functions.isNullOrEmpty(user.getPosition())) {
                    holder.tvPosition.setText(user.getPosition());
                } else if (!Functions.isNullOrEmpty(user.getPositionTitle())){
                    holder.tvPosition.setText(user.getPositionTitle());
                } else {
                    holder.tvPosition.setText("");
                }
            } else {
                holder.tvAvatar.setVisibility(View.INVISIBLE);
                holder.imgAvatar.setVisibility(View.INVISIBLE);
                holder.tvPosition.setText("");
            }

            if (!Functions.isNullOrEmpty(comment.getCreated())) {
                holder.tvDate.setText(Functions.share.formatDateLanguage(comment.getCreated()));
            } else {
                holder.tvDate.setText("");
            }

            if (!Functions.isNullOrEmpty(comment.getAttachFiles())) {
                ArrayList<AttachFile> lstAttach = new Gson().fromJson(comment.getAttachFiles(), new TypeToken<ArrayList<AttachFile>>() {
                }.getType());

                if (lstAttach != null && lstAttach.size() > 0) {
                    holder.lnAttach.setVisibility(View.VISIBLE);
                    holder.tvAttachCount.setText(String.valueOf(lstAttach.size()));
                } else {
                    holder.lnAttach.setVisibility(View.INVISIBLE);
                }

                holder.recyAttach_Image.setVisibility(View.VISIBLE);
                holder.recyAttach.setVisibility(View.VISIBLE);

                ArrayList<AttachFile> lstAttachTemp = DetailFunc.share.classifyListAttachFile(lstAttach);
                ParentAttachFileAdapter adapterCommentAttach = new ParentAttachFileAdapter(mainAct, context, lstAttachTemp, false, isReplyView);
                holder.recyAttach.setAdapter(adapterCommentAttach);
                holder.recyAttach.setLayoutManager(new LinearLayoutManager(context, LinearLayoutManager.VERTICAL, false));

                ParentAttachFile_ImageAdapter adapterCommentAttach_Image = new ParentAttachFile_ImageAdapter(mainAct, context, lstAttachTemp, false, isReplyView);
                holder.recyAttach_Image.setAdapter(adapterCommentAttach_Image);
                holder.recyAttach_Image.setLayoutManager(new LinearLayoutManager(context, LinearLayoutManager.VERTICAL, false));
            } else {
                holder.lnAttach.setVisibility(View.INVISIBLE);
                holder.recyAttach_Image.setVisibility(View.GONE);
                holder.recyAttach.setVisibility(View.GONE);
            }

            if (comment.getLikeCount() > 0) {
                holder.lnLike.setVisibility(View.VISIBLE);
                holder.tvLikeCount.setText(String.valueOf(comment.getLikeCount()));
            } else {
                holder.lnLike.setVisibility(View.INVISIBLE);
            }

            if (!Functions.isNullOrEmpty(comment.getContent())) {
                holder.tvComment.setText(comment.getContent());
            } else {
                holder.tvComment.setText("");
            }

            holder.tvLike.setText(Functions.share.getTitle("TEXT_LIKE", "Thích"));

            if (comment.isLiked() == 1) {
                holder.tvLike.setTextColor(ContextCompat.getColor(context, R.color.clBlueEnable));
                holder.tvLike.setText(Functions.share.getTitle("TEXT_UNLIKE", "Bỏ thích"));
            } else {
                holder.tvLike.setTextColor(ContextCompat.getColor(context, R.color.clBottomDisable));
                holder.tvLike.setText(Functions.share.getTitle("TEXT_LIKE", "Thích"));
            }
        }
    }

    @Override
    public int getItemCount() {
        return lstData.size();
    }

    public class ListCommentHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.ln_ItemComponentComment_ListComment_All)
        LinearLayout lnAll;
        @BindView(R.id.vw_ItemComponentComment_ListComment_MarginLeft)
        View vwMarginLeft;
        @BindView(R.id.tv_ItemComponentComment_ListComment_AttachCount)
        TextView tvAttachCount;
        @BindView(R.id.ln_ItemComponentComment_ListComment_Attach)
        LinearLayout lnAttach;
        @BindView(R.id.ln_ItemComponentComment_ListComment_Like)
        LinearLayout lnLike;
        @BindView(R.id.img_ItemComponentComment_ListComment_Avata)
        CircleImageView imgAvatar;
        @BindView(R.id.tv_ItemComponentComment_ListComment_Avata)
        TextView tvAvatar;
        @BindView(R.id.tv_ItemComponentComment_ListComment_Name)
        TextView tvName;
        @BindView(R.id.tv_ItemComponentComment_ListComment_Date)
        TextView tvDate;
        @BindView(R.id.tv_ItemComponentComment_ListComment_Position)
        TextView tvPosition;
        @BindView(R.id.tv_ItemComponentComment_ListComment_Comment)
        TextView tvComment;
        @BindView(R.id.tv_ItemComponentComment_ListComment_Like)
        TextView tvLike;
        @BindView(R.id.tv_ItemComponentComment_ListComment_Reply)
        TextView tvReply;
        @BindView(R.id.tv_ItemComponentComment_ListComment_LikeCount)
        TextView tvLikeCount;
        @BindView(R.id.ln_ItemComponentComment_ListComment_Action)
        LinearLayout lnAction;
        @BindView(R.id.recy_ItemComponentComment_ListComment_Attach_Image)
        RecyclerView recyAttach_Image;
        @BindView(R.id.recy_ItemComponentComment_ListComment_Attach)
        RecyclerView recyAttach;

        public ListCommentHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);

            if (widthView != -1) {
                lnAll.setLayoutParams(new LinearLayout.LayoutParams(widthView, LinearLayout.LayoutParams.WRAP_CONTENT));
            }

            tvReply.setOnClickListener(v -> listener.OnReplyClick(getAdapterPosition()));

            tvLike.setOnClickListener(v -> listener.OnLikeClick(getAdapterPosition()));
        }
    }
}

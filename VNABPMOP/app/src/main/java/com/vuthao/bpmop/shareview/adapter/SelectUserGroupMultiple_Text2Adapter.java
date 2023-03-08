package com.vuthao.bpmop.shareview.adapter;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Context;
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
import com.vuthao.bpmop.base.model.custom.UserAndGroup;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;

public class SelectUserGroupMultiple_Text2Adapter extends RecyclerView.Adapter<SelectUserGroupMultiple_Text2Adapter.SelectUserGroupMultiple_Text2Holder> {
    private Activity mainAct;
    private ArrayList<UserAndGroup> userAndGroups;
    private SelectUserGroupMultiple_Text2Listener listener;

    public interface SelectUserGroupMultiple_Text2Listener {
        void OnDeleteClick(int pos);
    }

    public SelectUserGroupMultiple_Text2Adapter(Activity mainAct, Context context, ArrayList<UserAndGroup> userAndGroups, SelectUserGroupMultiple_Text2Listener listener) {
        this.mainAct = mainAct;
        this.userAndGroups = userAndGroups;
        this.listener = listener;
    }

    public void updateItemListIsClicked(ArrayList<UserAndGroup> lstUser)
    {
        this.userAndGroups = lstUser;
    }

    @NonNull
    @Override
    public SelectUserGroupMultiple_Text2Holder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_choose_mutil_user_text_2, parent, false);
        return new SelectUserGroupMultiple_Text2Holder(view);
    }

    @SuppressLint("SetTextI18n")
    @Override
    public void onBindViewHolder(@NonNull SelectUserGroupMultiple_Text2Adapter.SelectUserGroupMultiple_Text2Holder holder, int position) {
        UserAndGroup uag = userAndGroups.get(position);
        if (uag != null) {
            holder.imgDelete.setVisibility(View.GONE);
            holder.lnAll.setBackgroundColor(ContextCompat.getColor(mainAct, R.color.clWhite));

            if (!Functions.isNullOrEmpty(uag.getName())) {
                holder.tvName.setText(uag.getName());
                if (position != userAndGroups.size() - 1) {
                    holder.tvName.setText(holder.tvName.getText() + "; ");
                }
            } else {
                holder.tvName.setText("");
            }
        }
    }

    @Override
    public int getItemCount() {
        return userAndGroups.size();
    }

    public class SelectUserGroupMultiple_Text2Holder extends RecyclerView.ViewHolder {
        @BindView(R.id.ln_ItemChooseMultiUser_Text2_All)
        LinearLayout lnAll;
        @BindView(R.id.tv_ItemChooseMultiUser_Text2_Name)
        TextView tvName;
        @BindView(R.id.tv_ItemChooseMultiUser_Text2_Delete)
        ImageView imgDelete;

        public SelectUserGroupMultiple_Text2Holder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);

            imgDelete.setOnClickListener(v -> listener.OnDeleteClick(getAdapterPosition()));
        }
    }
}

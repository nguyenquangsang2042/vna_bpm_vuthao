package com.vuthao.bpmop.shareview.adapter;

import android.app.Activity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.custom.UserAndGroup;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;

public class SelectUserGroupMultiple_TextAdapter extends RecyclerView.Adapter<SelectUserGroupMultiple_TextAdapter.SelectUserGroupMultiple_TextHolder> {
    private ArrayList<UserAndGroup> lstUser;
    public boolean showDeleteButton;
    private SelectUserGroupMultiple_TextListener listener;

    public SelectUserGroupMultiple_TextAdapter(Activity mainAct, ArrayList<UserAndGroup> lstUser, boolean showDeleteButton, SelectUserGroupMultiple_TextListener listener) {
        this.lstUser = lstUser;
        this.showDeleteButton = showDeleteButton;
        this.listener = listener;
    }

    public void updateItemListIsClicked(ArrayList<UserAndGroup> _lstUser) {
        this.lstUser = _lstUser;
    }

    public ArrayList<UserAndGroup> getListIsclicked()
    {
        if (lstUser != null && lstUser.size() > 0)
            return lstUser;
        else
            return new ArrayList<>();
    }

    @NonNull
    @Override
    public SelectUserGroupMultiple_TextHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_choose_mutil_user_text, parent, false);
        return new SelectUserGroupMultiple_TextHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull SelectUserGroupMultiple_TextAdapter.SelectUserGroupMultiple_TextHolder holder, int position) {
        UserAndGroup user = lstUser.get(position);
        if (user != null) {
            if (!Functions.isNullOrEmpty(user.getName())) {
                holder.tvName.setText(user.getName());
            } else {
                holder.tvName.setText("");
            }

            if (showDeleteButton) {
                holder.imgDelete.setVisibility(View.VISIBLE);
            } else {
                holder.imgDelete.setVisibility(View.GONE);
            }
        }
    }

    @Override
    public int getItemCount() {
        return lstUser.size();
    }

    public interface SelectUserGroupMultiple_TextListener {
        void OnDeleteItem(int pos);
    }

    public class SelectUserGroupMultiple_TextHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.tv_ItemChooseMultiUser_Text_Name)
        TextView tvName;
        @BindView(R.id.tv_ItemChooseMultiUser_Text_Delete)
        ImageView imgDelete;
        public SelectUserGroupMultiple_TextHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
            imgDelete.setOnClickListener(v -> {
                listener.OnDeleteItem(getAdapterPosition());
                notifyDataSetChanged();
            });
        }
    }
}

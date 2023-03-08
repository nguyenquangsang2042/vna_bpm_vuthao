package com.vuthao.bpmop.detail.adapter;

import android.content.Context;
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

public class SelectUserGroupMultipleTextAdapter extends RecyclerView.Adapter<SelectUserGroupMultipleTextAdapter.SelectUserGroupMultipleTextHolder> {
    private Context context;
    private SelectUserGroupMultipleTextListener listener;
    private ArrayList<UserAndGroup> userAndGroups;
    public boolean showDeleteButton;

    public SelectUserGroupMultipleTextAdapter(Context context, SelectUserGroupMultipleTextListener listener, ArrayList<UserAndGroup> userAndGroups, boolean showDeleteButton) {
        this.context = context;
        this.listener = listener;
        this.userAndGroups = userAndGroups;
        this.showDeleteButton = showDeleteButton;
    }

    public ArrayList<UserAndGroup> getListSelected() {
        if (userAndGroups != null && userAndGroups.size() > 0) {
            return userAndGroups;
        } else {
            return new ArrayList<>();
        }
    }

    public void updateItemListIsClicked(ArrayList<UserAndGroup> _lstUser) {
        userAndGroups = _lstUser;
    }

    @NonNull
    @Override
    public SelectUserGroupMultipleTextHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_choose_mutil_user_text, parent, false);
        return new SelectUserGroupMultipleTextHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull SelectUserGroupMultipleTextAdapter.SelectUserGroupMultipleTextHolder holder, int position) {
        UserAndGroup user = userAndGroups.get(position);
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
        return userAndGroups.size();
    }

    public interface SelectUserGroupMultipleTextListener {
        void OnItemDelete(int pos);
    }

    public class SelectUserGroupMultipleTextHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.tv_ItemChooseMultiUser_Text_Name)
        TextView tvName;
        @BindView(R.id.tv_ItemChooseMultiUser_Text_Delete)
        ImageView imgDelete;

        public SelectUserGroupMultipleTextHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);

            imgDelete.setOnClickListener(v -> listener.OnItemDelete(getAdapterPosition()));
        }
    }
}

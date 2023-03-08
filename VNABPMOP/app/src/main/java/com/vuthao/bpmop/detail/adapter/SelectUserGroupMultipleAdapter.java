package com.vuthao.bpmop.detail.adapter;

import android.content.Context;
import android.content.res.ColorStateList;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Filter;
import android.widget.Filterable;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.RecyclerView;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.ImageLoader;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.model.custom.UserAndGroup;

import java.util.ArrayList;
import java.util.Locale;

import butterknife.BindView;
import butterknife.ButterKnife;
import de.hdodenhof.circleimageview.CircleImageView;

public class SelectUserGroupMultipleAdapter extends RecyclerView.Adapter<SelectUserGroupMultipleAdapter.SelectUserGroupMultipleHolder> implements Filterable {
    private Context context;
    private ArrayList<UserAndGroup> userAndGroups;
    private ArrayList<UserAndGroup> userAndGroupsFilter = new ArrayList<>();
    private SelectUserGroupMultipleListener listener;

    public SelectUserGroupMultipleAdapter(Context context, ArrayList<UserAndGroup> userAndGroups, SelectUserGroupMultipleListener listener) {
        this.context = context;
        this.userAndGroups = userAndGroups;
        this.listener = listener;

        userAndGroupsFilter.addAll(userAndGroups);
    }

    public void setList(ArrayList<UserAndGroup> items) {
        userAndGroups.clear();
        userAndGroups.addAll(items);
        notifyDataSetChanged();
    }

    public void updateCurrentList(ArrayList<UserAndGroup> lstUser) {
        userAndGroupsFilter.clear();
        userAndGroupsFilter.addAll(lstUser);
    }

    public ArrayList<UserAndGroup> search(String charText) {
        ArrayList<UserAndGroup> searchs = new ArrayList<>();
        charText = Utility.removeAccent(charText.toLowerCase(Locale.getDefault()));
        for (UserAndGroup appBase : userAndGroupsFilter) {
            if (Utility.removeAccent(appBase.getName().toLowerCase()).contains(charText)) {
                searchs.add(appBase);
            }
        }

        return searchs;
    }

    @NonNull
    @Override
    public SelectUserGroupMultipleHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_select_user_and_group, parent, false);
        return new SelectUserGroupMultipleHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull SelectUserGroupMultipleAdapter.SelectUserGroupMultipleHolder holder, int position) {
        holder.viewIsSelected.setVisibility(View.INVISIBLE);
        if (position % 2 == 0) {
            holder.lnAll.setBackgroundColor(ContextCompat.getColor(context, R.color.clGraySearchUser));
        } else {
            holder.lnAll.setBackgroundColor(ContextCompat.getColor(context, R.color.clWhite));
        }

        UserAndGroup userAndGroup = userAndGroups.get(position);

        if (userAndGroup != null) {

            // 0 là User - 1 là Group
            if (userAndGroup.getType().equals("1")) {
                holder.tvTitle.setMaxLines(2);
                holder.tvEmail.setVisibility(View.GONE);
                holder.imgAvatar.setVisibility(View.VISIBLE);
                holder.tvAvatar.setVisibility(View.GONE);
                holder.imgAvatar.setImageResource(R.drawable.icon_ver2_group);
            } else {
                holder.tvTitle.setMaxLines(1);
                holder.tvEmail.setVisibility(View.VISIBLE);

                if (!Functions.isNullOrEmpty(userAndGroup.getImagePath())) {
                    holder.imgAvatar.setVisibility(View.VISIBLE);
                    holder.tvAvatar.setVisibility(View.GONE);

                    ImageLoader.getInstance().loadImageUserWithToken(context, Constants.BASE_URL + userAndGroup.getImagePath(), holder.imgAvatar);
                } else {
                    holder.imgAvatar.setVisibility(View.GONE);
                    holder.tvAvatar.setVisibility(View.VISIBLE);

                    holder.tvAvatar.setText(Functions.share.getAvatarName(userAndGroup.getAccountName()));
                    holder.tvAvatar.setBackgroundTintList(ColorStateList.valueOf(Functions.share.getColorByUsername(userAndGroup.getAccountName())));
                }
            }

            if (!Functions.isNullOrEmpty(userAndGroup.getName())) {
                holder.tvTitle.setText(userAndGroup.getName());
            } else {
                holder.tvTitle.setText("");
            }

            if (!Functions.isNullOrEmpty(userAndGroup.getEmail())) {
                holder.tvEmail.setText(userAndGroup.getEmail());
            } else {
                holder.tvEmail.setText("");
            }
        }
    }

    @Override
    public int getItemCount() {
        return userAndGroups.size();
    }

    @Override
    public Filter getFilter() {
        return new Filter() {
            @Override
            protected FilterResults performFiltering(CharSequence charSequence) {
                return new FilterResults();
            }

            @Override
            protected void publishResults(CharSequence charSequence, FilterResults filterResults) {
                String charString = Functions.removeAccent(charSequence.toString().toLowerCase());
                if (charString.isEmpty()) {
                    userAndGroups = new ArrayList<>();
                } else {
                    ArrayList<UserAndGroup> filteredList = new ArrayList<>();
                    for (UserAndGroup userAndGroup : userAndGroupsFilter) {
                        if (userAndGroup.getSearch().toLowerCase().contains(charString)) {
                            filteredList.add(userAndGroup);
                        }
                    }

                    userAndGroups = filteredList;
                }

                notifyDataSetChanged();
            }
        };
    }

    public interface SelectUserGroupMultipleListener {
        void OnClick(UserAndGroup userAndGroup);
    }

    public class SelectUserGroupMultipleHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.ln_ItemSelectUserAndGroup_All)
        LinearLayout lnAll;
        @BindView(R.id.view_ItemSelectUserAndGroup_LeftSelected)
        View viewIsSelected;
        @BindView(R.id.tv_ItemSelectUserAndGroup_Avatar)
        TextView tvAvatar;
        @BindView(R.id.img_ItemSelectUserAndGroup_Avatar)
        CircleImageView imgAvatar;
        @BindView(R.id.tv_ItemSelectUserAndGroup_Title)
        TextView tvTitle;
        @BindView(R.id.tv_ItemSelectUserAndGroup_Email)
        TextView tvEmail;

        public SelectUserGroupMultipleHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);

            lnAll.setOnClickListener(v -> listener.OnClick(userAndGroups.get(getAdapterPosition())));
        }
    }
}

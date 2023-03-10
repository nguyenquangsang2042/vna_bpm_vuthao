package com.vuthao.bpmop.shareview.adapter;

import android.app.Activity;
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
import com.vuthao.bpmop.base.model.custom.UserAndGroup;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;
import de.hdodenhof.circleimageview.CircleImageView;

public class SelectUserSingleAdapter extends RecyclerView.Adapter<SelectUserSingleAdapter.SelectUserSingleHolder> implements Filterable {
    private Activity mainAct;
    private ArrayList<UserAndGroup> userAndGroups;
    private ArrayList<UserAndGroup> userAndGroupsFilter = new ArrayList<>();
    private SelectUserSingleListener listener;

    public SelectUserSingleAdapter(Activity mainAct, ArrayList<UserAndGroup> userAndGroups, UserAndGroup clickedUser, SelectUserSingleListener listener) {
        this.mainAct = mainAct;
        this.userAndGroups = userAndGroups;
        this.listener = listener;
        userAndGroupsFilter.addAll(userAndGroups);
    }

    @NonNull
    @Override
    public SelectUserSingleHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_select_user_and_group, parent, false);
        return new SelectUserSingleHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull SelectUserSingleAdapter.SelectUserSingleHolder holder, int position) {
        holder.viewIsSelected.setVisibility(View.INVISIBLE);
        if (position % 2 == 0) {
            holder.lnAll.setBackgroundColor(ContextCompat.getColor(mainAct, R.color.clGraySearchUser));
        } else {
            holder.lnAll.setBackgroundColor(ContextCompat.getColor(mainAct, R.color.clWhite));
        }

        UserAndGroup user = userAndGroups.get(position);

        if (user != null) {

            // 0 l?? User - 1 l?? Group
            if (user.getType().equals("1")) {
                holder.tvTitle.setMaxLines(2);
                holder.tvEmail.setVisibility(View.GONE);
                holder.imgAvatar.setVisibility(View.VISIBLE);
                holder.tvAvatar.setVisibility(View.GONE);

                holder.imgAvatar.setImageResource(R.drawable.icon_ver2_group);
            } else {
                holder.tvTitle.setMaxLines(1);
                holder.tvEmail.setVisibility(View.VISIBLE);

                if (!Functions.isNullOrEmpty(user.getImagePath())) {
                    holder.imgAvatar.setVisibility(View.VISIBLE);
                    holder.tvAvatar.setVisibility(View.GONE);

                    ImageLoader.getInstance().loadImageUserWithToken(mainAct, Constants.BASE_URL + user.getImagePath(), holder.imgAvatar);
                } else {
                    holder.imgAvatar.setVisibility(View.GONE);
                    holder.tvAvatar.setVisibility(View.VISIBLE);

                    holder.tvAvatar.setText(Functions.share.getAvatarName(user.getAccountName()));
                    holder.tvAvatar.setBackgroundTintList(ColorStateList.valueOf(Functions.share.getColorByUsername(user.getAccountName())));
                }
            }

            if (!Functions.isNullOrEmpty(user.getName())) {
                holder.tvTitle.setText(user.getName());
            } else {
                holder.tvTitle.setText("");
            }

            if (!Functions.isNullOrEmpty(user.getEmail())) {
                holder.tvEmail.setText(user.getEmail());
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

    public interface SelectUserSingleListener {
        void OnClick(UserAndGroup userAndGroup);
    }

    public class SelectUserSingleHolder extends RecyclerView.ViewHolder {
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
        public SelectUserSingleHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
            lnAll.setOnClickListener(v -> listener.OnClick(userAndGroups.get(getAdapterPosition())));
        }
    }
}

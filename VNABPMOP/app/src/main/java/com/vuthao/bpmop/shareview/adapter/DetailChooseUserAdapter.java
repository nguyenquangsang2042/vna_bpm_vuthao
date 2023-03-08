package com.vuthao.bpmop.shareview.adapter;

import android.app.Activity;
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
import androidx.recyclerview.widget.RecyclerView;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.ImageLoader;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.model.app.User;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;
import de.hdodenhof.circleimageview.CircleImageView;

public class DetailChooseUserAdapter extends RecyclerView.Adapter<DetailChooseUserAdapter.DetailChooseUserHolder> implements Filterable {
    private Activity mainAct;
    private ArrayList<User> users;
    private ArrayList<User> usersFilter = new ArrayList<>();
    private User isClickedUser;
    private DetailChooseUserListener listener;

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
                    users = new ArrayList<>();
                } else {
                    ArrayList<User> filteredList = new ArrayList<>();
                    for (User data : usersFilter) {
                        if (Utility.removeAccent(data.getFullName().toLowerCase()).contains(charString)) {
                            filteredList.add(data);
                        }
                    }

                    users = filteredList;
                }

                notifyDataSetChanged();
            }
        };
    }

    public void updateListFilter(ArrayList<User> filters) {
        users.clear();
        users.addAll(filters);
        notifyDataSetChanged();
    }

    public interface DetailChooseUserListener {
        void OnClick(User user);
    }

    public DetailChooseUserAdapter(Activity mainAct, Context context, ArrayList<User> user, User isClickedUser, DetailChooseUserListener listener) {
        this.mainAct = mainAct;
        this.users = user;
        this.isClickedUser = isClickedUser;
        this.listener = listener;
        usersFilter.addAll(users);
    }

    @NonNull
    @Override
    public DetailChooseUserHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_choose_multi_user, parent, false);
        return new DetailChooseUserHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull DetailChooseUserAdapter.DetailChooseUserHolder holder, int position) {
        User user = users.get(position);
        if (user != null) {
            if (isClickedUser != null && !Functions.isNullOrEmpty(isClickedUser.getID())) {
                if (isClickedUser.getID().equals(user.getID())) {
                    holder.viewIsSelected.setVisibility(View.VISIBLE);
                } else {
                    holder.viewIsSelected.setVisibility(View.INVISIBLE);
                }
            } else {
                holder.viewIsSelected.setVisibility(View.INVISIBLE);
            }

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

            if (!Functions.isNullOrEmpty(user.getFullName())) {
                holder.tvTitle.setText(user.getFullName());
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
        return users.size();
    }

    public class DetailChooseUserHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.ln_ItemChooseMultiUser_All)
        LinearLayout lnAll;
        @BindView(R.id.view_ItemChooseMultiUser_LeftSelected)
        View viewIsSelected;
        @BindView(R.id.tv_ItemChooseMultiUser_Avatar)
        TextView tvAvatar;
        @BindView(R.id.img_ItemChooseMultiUser_Avatar)
        CircleImageView imgAvatar;
        @BindView(R.id.tv_ItemChooseMultiUser_Title)
        TextView tvTitle;
        @BindView(R.id.tv_ItemChooseMultiUser_Email)
        TextView tvEmail;

        public DetailChooseUserHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);

            lnAll.setOnClickListener(v -> listener.OnClick(users.get(getAdapterPosition())));
        }
    }
}

package com.vuthao.bpmop.search.adapter;

import android.app.Activity;
import android.content.res.ColorStateList;
import android.graphics.Typeface;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ProgressBar;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.core.content.res.ResourcesCompat;
import androidx.recyclerview.widget.RecyclerView;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.ImageLoader;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.vars.Variable;
import com.vuthao.bpmop.child.fragment.list.adapter.ListDynamicAdapter;
import com.vuthao.bpmop.detail.custom.DetailFunc;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;
import de.hdodenhof.circleimageview.CircleImageView;

public class SearchAdapter extends RecyclerView.Adapter {
    private Activity activity;
    private ArrayList<AppBase> filters;

    private SearchOnClickListener listener;

    private final int VIEW_TYPE_ITEM = 0;
    private final int VIEW_TYPE_LOADING = 1;

    private int YESTERDAY = -1;
    private int OLDER = -1;

    public interface SearchOnClickListener {
        void OnClick(AppBase appBase);
    }

    public SearchAdapter(Activity activity, ArrayList<AppBase> filters, SearchOnClickListener listener) {
        this.activity = activity;
        this.filters = filters;
        this.listener = listener;

        YESTERDAY = getIndexOfYesterday(filters);
        OLDER = getIndexOfOlder(filters);
    }

    public void setList(ArrayList<AppBase> appBases) {
        filters = appBases;

        YESTERDAY = getIndexOfYesterday(filters);
        OLDER = getIndexOfOlder(filters);

        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        if (viewType == VIEW_TYPE_ITEM) {
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_appbase_todo_list, parent, false);
            return new SearchHolder(view);
        } else {
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_load_more, parent, false);
            return new SearchLoadingHolder(view);
        }
    }

    @Override
    public void onBindViewHolder(@NonNull RecyclerView.ViewHolder holder, int position) {
        if (holder instanceof SearchHolder) {
            populateItemRows((SearchHolder) holder, position);
        } else if (holder instanceof SearchLoadingHolder) {
            showLoadingView((SearchLoadingHolder) holder, position);
        }
    }

    private void showLoadingView(SearchLoadingHolder viewHolder, int position) {
        //ProgressBar would be displayed

    }

    public void populateItemRows(SearchHolder holder, int position) {
        AppBase app = filters.get(position);
        if (position % 2 == 1) {
            holder.itemView.setBackgroundColor(ContextCompat.getColor(activity, R.color.clWhite));
        } else {
            holder.itemView.setBackgroundColor(ContextCompat.getColor(activity, R.color.clVer2BlueNavigation));
        }

        if (position == YESTERDAY) {
            holder.tvCategory.setText(Functions.share.getTitle("TEXT_YESTERDAY", "Hôm qua"));
            holder.lnCategory.setVisibility(View.VISIBLE);
        } else if (position == OLDER) {
            holder.tvCategory.setText(Functions.share.getTitle("TEXT_OLDER", "Cũ hơn"));
            holder.lnCategory.setVisibility(View.VISIBLE);
        } else {
            holder.lnCategory.setVisibility(View.GONE);
        }

        if (app != null) {
            holder.tvTitle.setText(app.getContent());

            if (!Functions.isNullOrEmpty(app.getWorkflowTitle())) {
                holder.tvDescription.setText(app.getWorkflowTitle());
            } else {
                holder.tvDescription.setText("");
            }

            if (!Functions.isNullOrEmpty(app.getCreated())) {
                holder.tvTime.setText(Functions.share.getDateLanguage(app.getCreated(), 0, CurrentUser.getInstance().getUser().getLanguage()));
            } else {
                holder.tvTime.setVisibility(View.INVISIBLE);
            }

            if (app.getUser() != null) {
                if (app.getUser().getType().equals("0")) {
                    ImageLoader.getInstance().loadImageUserWithToken(activity, Constants.BASE_URL + app.getUser().getImagePath(), holder.imgAvatar);
                } else {
                    holder.imgAvatar.setImageResource(R.drawable.icon_ver2_group);
                }
            } else {
                holder.imgAvatar.setImageResource(R.drawable.icon_avatar64);
            }

            if (app.getFileCount() > 0) {
                holder.imgAttachFile.setVisibility(View.VISIBLE);
            } else {
                holder.imgAttachFile.setVisibility(View.INVISIBLE);
            }

            holder.imgFlag.setVisibility(View.GONE);

            if (app.isFollow() == 1) {
                holder.imgFavorite.setImageResource(R.drawable.icon_ver2_star_checked);
            } else {
                holder.imgFavorite.setImageResource(R.drawable.icon_ver2_star_unchecked);
            }

            if (app.getWorkflowStatus() != null) {
                holder.tvStatus.setVisibility(View.VISIBLE);
                holder.tvStatus.setText(CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN) ? app.getWorkflowStatus().getTitle() : app.getWorkflowStatus().getTitleEN());
                holder.tvStatus.setBackgroundTintList(ColorStateList.valueOf(DetailFunc.share.getColorByActionID(activity, app.getApprovalStatus())));
            } else {
                holder.tvStatus.setVisibility(View.INVISIBLE);
            }

            holder.tvStatusTime.setText("");
        }
    }

    @Override
    public int getItemViewType(int position) {
        return filters.get(position) == null ? VIEW_TYPE_LOADING : VIEW_TYPE_ITEM;
    }

    private int getIndexOfYesterday(ArrayList<AppBase> list) {
        int pos = 0;
        long dateNow = Functions.share.formatStringToLong(Functions.share.getToDay(-1));
        for (AppBase myObj : list) {
            long newDate = 0;
            newDate = Functions.share.formatStringToLong(Functions.share.getDateString(myObj.getCreated()));
            if (dateNow == newDate)
                return pos;
            pos++;
        }

        return -1;
    }

    private int getIndexOfOlder(ArrayList<AppBase> list) {
        int pos = 0;
        long dateNow = Functions.share.formatStringToLong(Functions.share.getToDay(-1));
        for (AppBase myObj : list) {
            long newDate = 0;
            newDate = Functions.share.formatStringToLong(Functions.share.getDateString(myObj.getCreated()));
            if (dateNow > newDate)
                return pos;
            pos++;
        }

        return -1;
    }

    @Override
    public int getItemCount() {
        return filters.size();
    }

    public class SearchHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.ln_ItemAppBaseToDoList_Category)
        LinearLayout lnCategory;
        @BindView(R.id.tv_ItemAppBaseToDoList_Category)
        TextView tvCategory;
        @BindView(R.id.img_ItemAppBaseToDoList_Avatar)
        CircleImageView imgAvatar;
        @BindView(R.id.tv_ItemAppBaseToDoList_Title)
        TextView tvTitle;
        @BindView(R.id.tv_ItemAppBaseToDoList_Time)
        TextView tvTime;
        @BindView(R.id.tv_ItemAppBaseToDoList_Description)
        TextView tvDescription;
        @BindView(R.id.img_ItemAppBaseToDoList_Favorite)
        ImageView imgFavorite;
        @BindView(R.id.img_ItemAppBaseToDoList_Flag)
        ImageView imgFlag;
        @BindView(R.id.img_ItemAppBaseToDoList_AttachFile)
        ImageView imgAttachFile;
        @BindView(R.id.tv_ItemAppBaseToDoList_Status)
        TextView tvStatus;
        @BindView(R.id.tv_ItemAppBaseToDoList_StatusTime)
        TextView tvStatusTime;

        public SearchHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);

            itemView.setOnClickListener(view -> listener.OnClick(filters.get(getAdapterPosition())));
        }
    }

    public class SearchLoadingHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.prLoadMore)
        ProgressBar prLoadMore;

        public SearchLoadingHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
        }
    }
}

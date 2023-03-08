package com.vuthao.bpmop.home.adapter;

import android.annotation.SuppressLint;
import android.content.Context;
import android.content.res.ColorStateList;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Filter;
import android.widget.Filterable;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ProgressBar;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.RecyclerView;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.ImageLoader;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.vars.Variable;
import com.vuthao.bpmop.base.keyboard.KeyboardManager;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.model.app.CurrentUser;

import java.util.ArrayList;
import java.util.Locale;

import butterknife.BindView;
import butterknife.ButterKnife;
import de.hdodenhof.circleimageview.CircleImageView;

public class HomePageVTBDAdapter extends RecyclerView.Adapter implements Filterable {
    private ArrayList<AppBase> appBases;
    private final ArrayList<AppBase> appBasesFilter = new ArrayList<>();
    private final Context context;
    private final HomePageVTBDListener listener;
    private int YESTERDAY = -1;
    private int OLDER = -1;
    private final int VIEW_TYPE_ITEM = 0;
    private final int VIEW_TYPE_LOADING = 1;

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
                    appBases = appBasesFilter;
                } else {
                    ArrayList<AppBase> filteredList = new ArrayList<>();
                    for (AppBase appBase : appBasesFilter) {
                        if (Utility.removeAccent(appBase.getContent().toLowerCase()).contains(charString)) {
                            filteredList.add(appBase);
                        }
                    }

                    appBases = filteredList;
                }

                notifyDataSetChanged();
            }
        };
    }

    public void setListRefresh(ArrayList<AppBase> appBaseExts) {
        this.appBases = appBaseExts;
        this.appBasesFilter.clear();
        this.appBasesFilter.addAll(this.appBases);

        YESTERDAY = getIndexOfYesterday(appBases);
        OLDER = getIndexOfOlder(appBases);
        notifyDataSetChanged();
    }

    public interface HomePageVTBDListener {
        void OnVTDBClick(AppBase appBase);
    }

    public HomePageVTBDAdapter(ArrayList<AppBase> appBases, Context context, HomePageVTBDListener listener) {
        this.appBases = appBases;
        this.context = context;
        this.listener = listener;

        YESTERDAY = getIndexOfYesterday(appBases);
        OLDER = getIndexOfOlder(appBases);
        appBasesFilter.addAll(this.appBases);
    }

    public void updateItemRead(int Id) {
        int index = -1;
        for (int i = 0; i < appBases.size(); i++) {
            if (appBases.get(i).getID() == Id) {
                appBases.get(i).setRead(true);
                index = i;
                break;
            }
        }
        if (index != -1) {
            notifyItemChanged(index);
        }
    }

    public void updateFollow(String workflowId, boolean isFollow) {
        for (int i = 0; i < appBases.size(); i++) {
            String id = Functions.share.getWorkflowItemIDByUrl(appBases.get(i).getItemUrl());
            if (workflowId.equals(id)) {
                appBases.get(i).setFollow(isFollow ? 1 : 0);
                break;
            }
        }

        notifyDataSetChanged();
    }

    public void search(String charText) {
        charText = Utility.removeAccent(charText.toLowerCase(Locale.getDefault()));
        appBases.clear();
        if (charText.length() == 0) {
            appBases.addAll(appBasesFilter);
            notifyDataSetChanged();
        } else {
            for (AppBase appBase : appBasesFilter) {
                if (Utility.removeAccent(appBase.getContent().toLowerCase()).contains(charText)) {
                    appBases.add(appBase);
                    notifyDataSetChanged();
                }
            }
        }
    }

    public void setDefaultFilter() {
        this.appBases.clear();
        this.appBases.addAll(appBasesFilter);
        notifyDataSetChanged();
    }

    public void setListFilter(ArrayList<AppBase> filters) {
        this.appBases.clear();
        this.appBasesFilter.clear();
        this.appBases.addAll(filters);
        this.appBasesFilter.addAll(this.appBases);

        YESTERDAY = getIndexOfYesterday(appBases);
        OLDER = getIndexOfOlder(appBases);

        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        if (viewType == VIEW_TYPE_ITEM) {
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_appbase_todo_list, parent, false);
            return new HomePageRecyVTBDHolder(view);
        } else {
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_load_more, parent, false);
            return new HomePageRecyVTBDLoadMoreHolder(view);
        }
    }

    @SuppressLint("SetTextI18n")
    @Override
    public void onBindViewHolder(@NonNull RecyclerView.ViewHolder holder, int position) {
        if (holder instanceof HomePageRecyVTBDHolder) {
            populateItemRows((HomePageRecyVTBDHolder) holder, position);
        } else if (holder instanceof HomePageVDTAdapter.HomePageRecyVDTLoadMoreHolder) {
            showLoadingView((HomePageRecyVTBDLoadMoreHolder) holder, position);
        }
    }

    private void showLoadingView(HomePageRecyVTBDLoadMoreHolder viewHolder, int position) {
        //ProgressBar would be displayed
    }

    private void populateItemRows(HomePageRecyVTBDHolder holder, int position) {
        if (position % 2 == 1) {
            holder.itemView.setBackgroundColor(ContextCompat.getColor(context, R.color.clWhite));
        } else {
            holder.itemView.setBackgroundColor(ContextCompat.getColor(context, R.color.clVer2BlueNavigation));
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

        AppBase app = appBases.get(position);
        if (app != null) {
            if (app.getUser() != null) {
                if (app.getUser().getType().equals("0")) {
                    ImageLoader.getInstance().loadImageUserWithToken(context, Constants.BASE_URL + app.getUser().getImagePath(), holder.imgAvatar);
                } else {
                    holder.imgAvatar.setImageResource(R.drawable.icon_avatar64);
                }
            } else {
                holder.imgAvatar.setImageResource(R.drawable.icon_avatar64);
            }

            holder.tvTitle.setText(app.getContent());

            if (!Functions.isNullOrEmpty(app.getCreated())) {
                holder.tvTime.setText(Functions.share.getDateLanguage(app.getCreated(), 0, CurrentUser.getInstance().getUser().getLanguage()));
            } else {
                holder.tvTime.setVisibility(View.INVISIBLE);
            }

            if (!Functions.isNullOrEmpty(app.getAssignedTo())) {
                holder.tvDescription.setVisibility(View.VISIBLE);

                switch (app.getStatusGroup()) {
                    case Variable.AppStatusID.Completed:
                        holder.tvDescription.setText(Functions.share.getTitle("TEXT_TITLE_APPROVED", "Phê duyệt: ") + app.getUser().getName());
                        break;
                    case Variable.AppStatusID.Canceled:
                        holder.tvDescription.setText(Functions.share.getTitle("TEXT_TITLE_CANCEL", "Hủy: ") + app.getUser().getName());
                        break;
                    case Variable.AppStatusID.Rejected:
                        holder.tvDescription.setText(Functions.share.getTitle("TEXT_TITLE_REJECTED", "Từ chối: ") + app.getUser().getName());
                        break;
                    default:
                        String[] fullName = Functions.share.getArrayFullNameFromArrayID(app.getAssignedTo().split(","));
                        fullName[0] = app.getUser().getName();
                        Functions.share.setTextView_FormatMultiUser(holder.tvDescription, fullName, false, true);
                        break;
                }
            } else {
                holder.tvDescription.setVisibility(View.INVISIBLE);
            }

            if (app.getFileCount() > 0) {
                holder.imgAttachFile.setVisibility(View.VISIBLE);
            } else {
                holder.imgAttachFile.setVisibility(View.INVISIBLE);
            }

            if (app.isFollow() == 1) {
                holder.imgFavorite.setImageResource(R.drawable.icon_ver2_star_checked);
            } else {
                holder.imgFavorite.setImageResource(R.drawable.icon_ver2_star_unchecked);
            }

            holder.imgFlag.setVisibility(View.GONE);

            if (app.getAppStatus() != null) {
                holder.tvStatus.setVisibility(View.VISIBLE);
                holder.tvStatus.setText(CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN) ? app.getAppStatus().getTitle() : app.getAppStatus().getTitleEN());
                holder.tvStatus.setBackgroundTintList(ColorStateList.valueOf(Functions.share.getColorByAppStatus(app.getStatusGroup())));
            } else {
                holder.tvStatus.setVisibility(View.INVISIBLE);
            }

            if (!Functions.isNullOrEmpty(app.getDueDate())) {
                switch (app.getStatusGroup()) {
                    case Variable.AppStatusID.Completed:
                    case Variable.AppStatusID.Canceled:
                    case Variable.AppStatusID.Rejected:
                        holder.tvStatusTime.setVisibility(View.INVISIBLE);
                        holder.tvStatusTime.setText("");
                        break;
                    default:
                        holder.tvStatusTime.setVisibility(View.VISIBLE);
                        holder.tvStatusTime.setTextColor(Functions.share.getColorByDueDate(app.getDueDate()));
                        holder.tvStatusTime.setText(Functions.share.getDateLanguage(app.getDueDate(), 1, CurrentUser.getInstance().getUser().getLanguage()));
                        break;
                }

            } else {
                holder.tvStatusTime.setVisibility(View.INVISIBLE);
                holder.tvStatusTime.setText("");
            }
        }
    }

    @Override
    public int getItemCount() {
        return appBases.size();
    }

    @Override
    public int getItemViewType(int position) {
        return appBases.get(position) == null ? VIEW_TYPE_LOADING : VIEW_TYPE_ITEM;
    }

    public class HomePageRecyVTBDLoadMoreHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.prLoadMore)
        ProgressBar prLoadMore;

        public HomePageRecyVTBDLoadMoreHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
        }
    }

    public class HomePageRecyVTBDHolder extends RecyclerView.ViewHolder {
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

        public HomePageRecyVTBDHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);

            lnCategory.setOnClickListener(v -> KeyboardManager.hideKeyboard(BaseActivity.sBaseActivity));
            itemView.setOnClickListener(v -> listener.OnVTDBClick(appBases.get(getAdapterPosition())));
        }
    }

    private int getIndexOfYesterday(ArrayList<AppBase> list) {
        int pos = 0;
        long dateNow = Functions.share.formatStringToLong(Functions.share.getToDay(-1));
        for (AppBase myObj : list) {
            long newDate = Functions.share.formatStringToLong(Functions.share.getDateString(myObj.getCreated()));
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
            long newDate = Functions.share.formatStringToLong(Functions.share.getDateString(myObj.getCreated()));
            if (dateNow > newDate)
                return pos;
            pos++;
        }

        return -1;
    }
}

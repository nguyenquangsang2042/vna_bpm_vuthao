package com.vuthao.bpmop.home.adapter;

import android.content.Context;
import android.content.res.ColorStateList;
import android.graphics.Typeface;
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
import androidx.core.content.res.ResourcesCompat;
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
import com.vuthao.bpmop.base.model.app.AppStatus;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.model.app.Workflow;
import com.vuthao.bpmop.base.realm.RealmHelper;
import com.vuthao.bpmop.base.realm.WorkflowController;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;
import de.hdodenhof.circleimageview.CircleImageView;

public class HomePageVDTAdapter extends RecyclerView.Adapter implements Filterable {
    private final HomePageRecyVDTListener listener;
    private final Context context;
    private ArrayList<AppBase> appBaseExts;
    private final ArrayList<AppBase> appBaseFilter = new ArrayList<>();
    private int type = 1;
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
                    appBaseExts = appBaseFilter;
                } else {
                    ArrayList<AppBase> filteredList = new ArrayList<>();
                    for (AppBase appBase : appBaseFilter) {
                        if (Utility.removeAccent(appBase.getContent().toLowerCase()).contains(charString)) {
                            filteredList.add(appBase);
                        }
                    }

                    appBaseExts = filteredList;
                }

                notifyDataSetChanged();
            }
        };
    }

    public interface HomePageRecyVDTListener {
        void OnClick(AppBase app);
    }

    public void search(String charText) {
        charText = Utility.removeAccent(charText.toLowerCase());
        appBaseExts.clear();
        if (charText.length() == 0) {
            appBaseExts.addAll(appBaseFilter);
            notifyDataSetChanged();
        } else {
            for (AppBase appBase : appBaseFilter) {
                if (Utility.removeAccent(appBase.getContent().toLowerCase()).contains(charText)) {
                    appBaseExts.add(appBase);
                    notifyDataSetChanged();
                }
            }
        }
    }

    public void updateItemRead(int Id) {
        int index = -1;
        for (int i = 0; i < appBaseExts.size(); i++) {
            if (appBaseExts.get(i).getID() == Id) {
                appBaseExts.get(i).setRead(true);
                index = i;
                break;
            }
        }
        if (index != -1) {
            notifyItemChanged(index);
        }
    }

    public void updateFollow(String workflowId, boolean isFollow) {
        for (int i = 0; i < appBaseExts.size(); i++) {
            String itemId = Functions.share.getWorkflowItemIDByUrl(appBaseExts.get(i).getItemUrl());
            if (workflowId.equals(itemId)) {
                appBaseExts.get(i).setFollow(isFollow ? 1 : 0);
                break;
            }
        }

        notifyDataSetChanged();
    }

    public void setListRefresh(ArrayList<AppBase> appBaseExts) {
        this.type = Variable.BottomNavigation.SingleListVDT;
        this.appBaseExts = appBaseExts;
        this.appBaseFilter.clear();
        this.appBaseFilter.addAll(this.appBaseExts);
        YESTERDAY = getIndexOfYesterday(appBaseExts);
        OLDER = getIndexOfOlder(appBaseExts);
        notifyDataSetChanged();
    }

    public HomePageVDTAdapter(HomePageRecyVDTListener listener, Context context, ArrayList<AppBase> appBaseExts, int type) {
        this.listener = listener;
        this.context = context;
        this.appBaseExts = appBaseExts;
        this.type = type;

        YESTERDAY = getIndexOfYesterday(appBaseExts);
        OLDER = getIndexOfOlder(appBaseExts);
        appBaseFilter.addAll(this.appBaseExts);
    }

    public void setType(int type) {
        this.type = type;
    }

    public void setDefaultFilter() {
        this.appBaseExts.clear();
        this.appBaseExts.addAll(appBaseFilter);
        notifyDataSetChanged();
    }

    public void setListFilter(ArrayList<AppBase> filters) {
        this.appBaseExts.clear();
        this.appBaseFilter.clear();
        this.appBaseExts.addAll(filters);
        this.appBaseFilter.addAll(this.appBaseExts);

        YESTERDAY = getIndexOfYesterday(appBaseExts);
        OLDER = getIndexOfOlder(appBaseExts);

        notifyDataSetChanged();
    }

    public void setListLoadMore(ArrayList<AppBase> appBases) {
        this.appBaseExts.clear();
        this.appBaseFilter.clear();
        this.appBaseExts.addAll(appBases);
        this.appBaseFilter.addAll(this.appBaseExts);
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        if (viewType == VIEW_TYPE_ITEM) {
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_appbase_todo_list, parent, false);
            return new HomePageRecyVDTHolder(view);
        } else {
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_load_more, parent, false);
            return new HomePageRecyVDTLoadMoreHolder(view);
        }
    }

    @Override
    public void onBindViewHolder(@NonNull RecyclerView.ViewHolder holder, int position) {
        if (holder instanceof HomePageRecyVDTHolder) {
            populateItemRows((HomePageRecyVDTHolder) holder, position);
        } else if (holder instanceof HomePageRecyVDTLoadMoreHolder) {
            showLoadingView((HomePageRecyVDTLoadMoreHolder) holder, position);
        }
    }

    private void populateItemRows(HomePageRecyVDTHolder holder, int position) {
        AppBase app = appBaseExts.get(position);
        if (position % 2 == 1) {
            holder.itemView.setBackgroundColor(ContextCompat.getColor(context, R.color.clWhite));
        } else {
            holder.itemView.setBackgroundColor(ContextCompat.getColor(context, R.color.clVer2BlueNavigation));
        }

        if (app != null) {
            if (position == YESTERDAY) {
                holder.tvCategory.setText(Functions.share.getTitle("TEXT_YESTERDAY", "Hôm qua"));
                holder.lnCategory.setVisibility(View.VISIBLE);
            } else if (position == OLDER) {
                holder.tvCategory.setText(Functions.share.getTitle("TEXT_OLDER", "Cũ hơn"));
                holder.lnCategory.setVisibility(View.VISIBLE);
            } else {
                holder.lnCategory.setVisibility(View.GONE);
            }

            holder.tvTitle.setText(app.getContent());
            if (app.isRead()) {
                holder.tvTitle.setTypeface(ResourcesCompat.getFont(context, R.font.fontarial), Typeface.NORMAL);
            } else {
                holder.tvTitle.setTypeface(ResourcesCompat.getFont(context, R.font.fontarial), Typeface.BOLD);
            }

            if (!Functions.isNullOrEmpty(app.getCreated())) {
                holder.tvTime.setText(Functions.share.getDateLanguage(app.getCreated(), 0, CurrentUser.getInstance().getUser().getLanguage()));
            } else {
                holder.tvTime.setVisibility(View.INVISIBLE);
            }

            if (app.getUser() != null) {
                if (!Functions.isNullOrEmpty(app.getUser().getType())&&app.getUser().getType().equals("0")) {
                    ImageLoader.getInstance().loadImageUserWithToken(context, Constants.BASE_URL + app.getUser().getImagePath(), holder.imgAvatar);
                } else {
                    holder.imgAvatar.setImageResource(R.drawable.icon_ver2_group);
                }
            } else {
                holder.imgAvatar.setImageResource(R.drawable.icon_avatar64);
            }

            boolean isTask = false;
            if (app.getResourceCategoryId() > 0 && app.getResourceSubCategoryId() >= 0) {
                isTask = app.getResourceCategoryId() == 16 && app.getResourceSubCategoryId() == 0;
            }

            if (isTask) {
                holder.tvDescription.setText(Functions.share.getTitle("TEXT_TASK", "Công việc"));
            } else {
                if (app.getWorkflow() != null) {
                    holder.tvDescription.setText(CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN) ? app.getWorkflow().getTitle() : app.getWorkflow().getTitleEN());
                } else {
                    holder.tvDescription.setText("");
                }
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

            if (app.getAppStatus() != null) {
                holder.tvStatus.setVisibility(View.VISIBLE);
                holder.tvStatus.setText(CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN) ? app.getAppStatus().getTitle() : app.getAppStatus().getTitleEN());
                holder.tvStatus.setBackgroundTintList(ColorStateList.valueOf(Functions.share.getColorByAppStatus(app.getStatusGroup())));
            } else {
                holder.tvStatus.setVisibility(View.INVISIBLE);
            }

            if (!Functions.isNullOrEmpty(app.getDueDate())) {
                holder.tvStatusTime.setTextColor(Functions.share.getColorByDueDate(app.getDueDate()));
                holder.tvStatusTime.setText(Functions.share.getDateLanguage(app.getDueDate(), 1, CurrentUser.getInstance().getUser().getLanguage()));
            } else {
                holder.tvStatusTime.setText("");
            }

            if (isTask) {
                if (!Functions.isNullOrEmpty(app.getDueDate()) && app.getStatusGroup() > 0 && app.getStatusGroup() != 8) {
                    holder.tvStatusTime.setTextColor(Functions.share.getColorByDueDate(app.getDueDate()));
                    holder.tvStatusTime.setText(Functions.share.getDateLanguage(app.getDueDate(), 1, CurrentUser.getInstance().getUser().getLanguage()));
                } else {
                    holder.tvStatusTime.setText("");
                }
            } else {
                if (!Functions.isNullOrEmpty(app.getDueDate()) && app.getStatusGroup() > 0) {
                    switch (app.getStatusGroup()) {
                        case 8:
                        case 64:
                        case 16:
                            holder.tvStatusTime.setText("");
                            break;
                        default:
                            holder.tvStatusTime.setTextColor(Functions.share.getColorByDueDate(app.getDueDate()));
                            holder.tvStatusTime.setText(Functions.share.getDateLanguage(app.getDueDate(), 1, CurrentUser.getInstance().getUser().getLanguage()));
                            break;
                    }
                }
            }
        }
    }

    private void showLoadingView(HomePageRecyVDTLoadMoreHolder viewHolder, int position) {
        //ProgressBar would be displayed
    }

    @Override
    public int getItemCount() {
        return appBaseExts.size();
    }

    @Override
    public int getItemViewType(int position) {
        return appBaseExts.get(position) == null ? VIEW_TYPE_LOADING : VIEW_TYPE_ITEM;
    }

    public static class HomePageRecyVDTLoadMoreHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.prLoadMore)
        ProgressBar prLoadMore;

        public HomePageRecyVDTLoadMoreHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
        }
    }

    public class HomePageRecyVDTHolder extends RecyclerView.ViewHolder {
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

        public HomePageRecyVDTHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);

            lnCategory.setOnClickListener(v -> KeyboardManager.hideKeyboard(BaseActivity.sBaseActivity));

            itemView.setOnClickListener(v -> listener.OnClick(appBaseExts.get(getAdapterPosition())));
        }
    }

    private int getIndexOfYesterday(ArrayList<AppBase> list) {
        int pos = 0;
        long dateNow = Functions.share.formatStringToLong(Functions.share.getToDay(-1));
        for (AppBase myObj : list) {
            long newDate = 0;
            if (type == Variable.BottomNavigation.SingleListVDT) {
                newDate = Functions.share.formatStringToLong(Functions.share.getDateString(myObj.getModified()));
            } else if (type == Variable.BottomNavigation.Filter) {
                newDate = Functions.share.formatStringToLong(Functions.share.getDateString(myObj.getNotifyCreated()));
            } else {
                newDate = Functions.share.formatStringToLong(Functions.share.getDateString(myObj.getStartDate()));
            }

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
            if (type == Variable.BottomNavigation.SingleListVDT) {
                newDate = Functions.share.formatStringToLong(Functions.share.getDateString(myObj.getModified()));
            } else if (type == Variable.BottomNavigation.Filter) {
                newDate = Functions.share.formatStringToLong(Functions.share.getDateString(myObj.getNotifyCreated()));
            } else {
                newDate = Functions.share.formatStringToLong(Functions.share.getDateString(myObj.getStartDate()));
            }
            if (dateNow > newDate)
                return pos;
            pos++;
        }

        return -1;
    }
}

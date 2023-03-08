package com.vuthao.bpmop.follow.adapter;

import android.app.Activity;
import android.content.res.ColorStateList;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Filter;
import android.widget.Filterable;
import android.widget.ImageView;
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
import com.vuthao.bpmop.base.vars.Variable;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.model.app.AppStatus;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.model.app.Workflow;
import com.vuthao.bpmop.base.realm.AppBaseController;
import com.vuthao.bpmop.base.realm.RealmController;
import com.vuthao.bpmop.base.realm.RealmHelper;

import java.util.ArrayList;
import java.util.Locale;

import butterknife.BindView;
import butterknife.ButterKnife;
import de.hdodenhof.circleimageview.CircleImageView;
import io.realm.Realm;

public class FollowAdapter extends RecyclerView.Adapter<FollowAdapter.FollowHolder> implements Filterable {
    private final Activity activity;
    private ArrayList<AppBase> appBaseExts;
    private final ArrayList<AppBase> appBaseFilter = new ArrayList<>();
    private int YESTERDAY = -1;
    private int OLDER = -1;
    private final FollowListener listener;

    public FollowAdapter(Activity activity, ArrayList<AppBase> appBaseExts, FollowListener listener) {
        this.activity = activity;
        this.appBaseExts = appBaseExts;
        this.listener = listener;

        YESTERDAY = getIndexOfYesterday(appBaseExts);
        OLDER = getIndexOfOlder(appBaseExts);
        appBaseFilter.addAll(appBaseExts);
    }

    public void setListRefresh(ArrayList<AppBase> appBases) {
        this.appBaseExts.clear();
        this.appBaseFilter.clear();
        this.appBaseExts.addAll(appBases);
        this.appBaseFilter.addAll(this.appBaseExts);

        YESTERDAY = getIndexOfYesterday(this.appBaseExts);
        OLDER = getIndexOfOlder(this.appBaseExts);

        notifyDataSetChanged();
    }

    public void search(String charText) {
        charText = Utility.removeAccent(charText.toLowerCase(Locale.getDefault()));
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

    @NonNull
    @Override
    public FollowHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext())
                .inflate(R.layout.item_appbase_todo_list, parent, false);
        return new FollowHolder(view);
    }

    @Override
    public int getItemCount() {
        return appBaseExts.size();
    }

    @Override
    public void onBindViewHolder(@NonNull FollowHolder holder, int position) {
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

        AppBase app = appBaseExts.get(position);
        if (app != null) {

            if (app.getUser() != null) {
                if (app.getUser().getType().equals("0")) {
                    ImageLoader.getInstance().loadImageUserWithToken(activity, Constants.BASE_URL + app.getUser().getImagePath(), holder.imgAvatar);
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

            if (app.getWorkflow() != null) {
                if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
                    holder.tvDescription.setText(app.getWorkflow().getTitle());
                } else {
                    holder.tvDescription.setText(app.getWorkflow().getTitleEN());
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

    public interface FollowListener {
        void OnClick(int pos);
    }

    public class FollowHolder extends RecyclerView.ViewHolder {
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

        public FollowHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);

            itemView.setOnClickListener(v -> listener.OnClick(getAdapterPosition()));
        }
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
}

package com.vuthao.bpmop.board.adapter;

import android.content.Context;
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
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.Workflow;

import java.util.ArrayList;
import java.util.Locale;

import butterknife.BindView;
import butterknife.ButterKnife;

public class BoardMainGroupAdapter extends RecyclerView.Adapter<BoardMainGroupAdapter.BoardMainGroupHolder> implements Filterable {
    private final Context context;
    private ArrayList<Workflow> workflows;
    private final ArrayList<Workflow> workflowsFilters = new ArrayList<>();
    private final RcvBoardMainGroupListener listener;

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
                    workflows.clear();
                    workflows.addAll(workflowsFilters);
                } else {
                    ArrayList<Workflow> filteredList = new ArrayList<>();
                    for (Workflow workflow : workflowsFilters) {
                        if (Functions.removeAccent(workflow.getTitle()).toLowerCase().contains(charString) ||
                                Functions.removeAccent(workflow.getTitleEN()).toLowerCase().contains(charString)) {
                            filteredList.add(workflow);
                        }
                    }

                    workflows = filteredList;
                }

                notifyDataSetChanged();
            }
        };
    }

    public interface RcvBoardMainGroupListener {
        void OnFavoriteItemClick(Workflow workflow);
        void OnItemBoardClick(Workflow workflow);
    }

    public BoardMainGroupAdapter(Context context, ArrayList<Workflow> workflows, RcvBoardMainGroupListener listener) {
        this.context = context;
        this.workflows = workflows;
        this.listener = listener;
        workflowsFilters.addAll(workflows);
    }

    public void updateListFavorite(ArrayList<Workflow> workflows) {
        workflowsFilters.clear();
        workflowsFilters.addAll(workflows);
        this.workflows.clear();
        this.workflows.addAll(workflowsFilters);
        notifyDataSetChanged();

    }

    public void setList(ArrayList<Workflow> workflows, String searchText) {
        this.workflows = workflows;
        this.workflowsFilters.clear();
        this.workflowsFilters.addAll(this.workflows);

        if (searchText.isEmpty()) {
            notifyDataSetChanged();
        } else {
            getFilter().filter(searchText);
        }
    }
    public void search(String charText) {
        charText = Utility.removeAccent(charText.toLowerCase(Locale.getDefault()));
        workflows.clear();
        if (charText.length() == 0) {
            workflows.addAll(workflowsFilters);
            notifyDataSetChanged();
        } else {
            for (Workflow appBase : workflowsFilters) {
                if (Utility.removeAccent(appBase.getTitle().toLowerCase()).contains(charText)
                 || Utility.removeAccent(appBase.getTitleEN().toLowerCase()).contains(charText)) {
                    workflows.add(appBase);
                    notifyDataSetChanged();
                }
            }
        }
    }

    @NonNull
    @Override
    public BoardMainGroupHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_expand_board_child, parent, false);
        return new BoardMainGroupHolder(view);
    }

    @Override
    public int getItemCount() {
        return workflows.size();
    }

    @Override
    public void onBindViewHolder(@NonNull BoardMainGroupHolder holder, int position) {
        if (position % 2 == 1) {
            holder.lnContainer.setBackgroundColor(ContextCompat.getColor(context,R.color.clWhite));
        } else {
            holder.lnContainer.setBackgroundColor(ContextCompat.getColor(context,R.color.clVer2BlueNavigation));
        }

        Workflow workflow = workflows.get(position);
        if (workflow != null) {
            if (!Functions.isNullOrEmpty(workflow.getImageURL())) {
                ImageLoader.getInstance().loadImageWithToken(context, Constants.BASE_URL + workflow.getImageURL()
                        , holder.imgAvatar, R.drawable.icon_ver3_app);
            } else {
                holder.imgAvatar.setImageResource(R.drawable.icon_ver3_app);
                holder.imgAvatar.setColorFilter(ContextCompat.getColor(context, R.color.clVer2BlueMain));
            }

            if (workflow.isFavorite() == 1) {
                holder.imgFavorite.setImageResource(R.drawable.icon_ver2_favorite_check);
                holder.imgFavorite.setColorFilter(ContextCompat.getColor(context,R.color.clVer2BlueMain));
            } else {
                holder.imgFavorite.setImageResource(R.drawable.icon_ver2_favorite_uncheck);
                holder.imgFavorite.setColorFilter(ContextCompat.getColor(context,R.color.clBottomDisable));
            }

            if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
                holder.tvTitle.setText(workflow.getTitle());
            } else {
                holder.tvTitle.setText(workflow.getTitleEN());
            }

            holder.tvDescription.setText("");
        }
    }

    public class BoardMainGroupHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.ln_ItemVer2ExpandBoardChild_Container)
        LinearLayout lnContainer;
        @BindView(R.id.ln_ItemVer2ExpandBoardChild_All)
        LinearLayout lnAll;
        @BindView(R.id.tv_ItemVer2ExpandBoardChild_Title)
        TextView tvTitle;
        @BindView(R.id.tv_ItemVer2ExpandBoardChild_Description)
        TextView tvDescription;
        @BindView(R.id.img_ItemVer2ExpandBoardChild_Avatar)
        ImageView imgAvatar;
        @BindView(R.id.img_ItemVer2ExpandBoardChild_Favorite)
        ImageView imgFavorite;

        public BoardMainGroupHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);

            imgFavorite.setOnClickListener(v -> listener.OnFavoriteItemClick(workflows.get(getAdapterPosition())));
            lnAll.setOnClickListener(v -> listener.OnItemBoardClick(workflows.get(getAdapterPosition())));
        }
    }
}

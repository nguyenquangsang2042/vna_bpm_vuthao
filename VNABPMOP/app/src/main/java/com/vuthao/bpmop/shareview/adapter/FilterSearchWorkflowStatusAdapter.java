package com.vuthao.bpmop.shareview.adapter;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.model.app.AppStatus;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.WorkflowStatus;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;

public class FilterSearchWorkflowStatusAdapter extends RecyclerView.Adapter<FilterSearchWorkflowStatusAdapter.FilterSearchWorkflowStatusHolder> {
    private Context context;
    private ArrayList<WorkflowStatus> statuses;
    private FilterSearchWorkflowStatusListener listener;

    public interface FilterSearchWorkflowStatusListener {
        void OnClick(int pos);
    }

    public void setData(ArrayList<WorkflowStatus> statuses) {
        this.statuses.clear();
        this.statuses.addAll(statuses);
        notifyDataSetChanged();
    }

    public FilterSearchWorkflowStatusAdapter(Context context, ArrayList<WorkflowStatus> statuses, FilterSearchWorkflowStatusListener listener) {
        this.context = context;
        this.statuses = statuses;
        this.listener = listener;
    }

    @NonNull
    @Override
    public FilterSearchWorkflowStatusHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_popup_filter, parent, false);
        return new FilterSearchWorkflowStatusHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull FilterSearchWorkflowStatusHolder holder, int position) {
        WorkflowStatus status = statuses.get(position);
        if (position == statuses.size() - 1) {
            holder.viewLine.setVisibility(View.GONE);
        } else {
            holder.viewLine.setVisibility(View.VISIBLE);
        }

        if (status != null) {
            if (status.isSelected()) {
                holder.imgCheck.setVisibility(View.VISIBLE);
            } else {
                holder.imgCheck.setVisibility(View.INVISIBLE);
            }

            if (Constants.mLangVN.equals(String.valueOf(CurrentUser.getInstance().getUser().getLanguage()))) {
                holder.tvTitle.setText(status.getTitle());
            } else {
                holder.tvTitle.setText(status.getTitleEN());
            }
        }
    }

    @Override
    public int getItemCount() {
        return statuses.size();
    }

    public class FilterSearchWorkflowStatusHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.ln_ItemPopupFilter)
        LinearLayout lnFilter;
        @BindView(R.id.tv_ItemPopupFilter_Title)
        TextView tvTitle;
        @BindView(R.id.img_ItemPopupFilter_Check)
        ImageView imgCheck;
        @BindView(R.id.viewLine)
        View viewLine;

        public FilterSearchWorkflowStatusHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
            itemView.setOnClickListener(v -> listener.OnClick(getAdapterPosition()));
        }
    }
}

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

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;

public class PopupFilterMultiChoiceAdapter extends RecyclerView.Adapter<PopupFilterMultiChoiceAdapter.PopupFilterMultiChoiceHolder> {
    private Context context;
    private ArrayList<AppStatus> statuses;
    private PopupFilterMultiChoiceListener listener;

    public interface PopupFilterMultiChoiceListener {
        void OnClick(int pos);
    }

    public void setData(ArrayList<AppStatus> statuses) {
        this.statuses.clear();
        this.statuses.addAll(statuses);
        notifyDataSetChanged();
    }

    public PopupFilterMultiChoiceAdapter(Context context, ArrayList<AppStatus> statuses, PopupFilterMultiChoiceListener listener) {
        this.context = context;
        this.statuses = statuses;
        this.listener = listener;
    }

    @NonNull
    @Override
    public PopupFilterMultiChoiceHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_popup_filter, parent, false);
        return new PopupFilterMultiChoiceHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull PopupFilterMultiChoiceAdapter.PopupFilterMultiChoiceHolder holder, int position) {
        AppStatus status = statuses.get(position);
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

    public class PopupFilterMultiChoiceHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.ln_ItemPopupFilter)
        LinearLayout lnFilter;
        @BindView(R.id.tv_ItemPopupFilter_Title)
        TextView tvTitle;
        @BindView(R.id.img_ItemPopupFilter_Check)
        ImageView imgCheck;
        @BindView(R.id.viewLine)
        View viewLine;

        public PopupFilterMultiChoiceHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
            itemView.setOnClickListener(v -> listener.OnClick(getAdapterPosition()));
        }
    }
}

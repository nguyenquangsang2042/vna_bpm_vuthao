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
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.custom.LookupData;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;

public class PopupFilterSingleChoiceAdapter extends RecyclerView.Adapter<PopupFilterSingleChoiceAdapter.PopupFilterSingleChoiceHolder> {
    private ArrayList<LookupData> datas;
    private PopupFilterSingleChoiceListener listener;

    public PopupFilterSingleChoiceAdapter(Context context, ArrayList<LookupData> datas, PopupFilterSingleChoiceListener listener) {
        this.datas = datas;
        this.listener = listener;
    }

    @NonNull
    @Override
    public PopupFilterSingleChoiceHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_popup_filter, parent, false);
        return new PopupFilterSingleChoiceHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull PopupFilterSingleChoiceAdapter.PopupFilterSingleChoiceHolder holder, int position) {
        LookupData data = datas.get(position);

        if (position == datas.size() - 1) {
            holder.viewLine.setVisibility(View.GONE);
        } else {
            holder.viewLine.setVisibility(View.VISIBLE);
        }

        if (data != null) {
            if (data.isSelected()) {
                holder.imgCheck.setVisibility(View.VISIBLE);
            } else {
                holder.imgCheck.setVisibility(View.INVISIBLE);
            }

            if (!Functions.isNullOrEmpty(data.getTitle())) {
                holder.tvTitle.setText(data.getTitle());
            } else {
                holder.tvTitle.setText("");
            }
        }
    }

    @Override
    public int getItemCount() {
        return datas.size();
    }

    public interface PopupFilterSingleChoiceListener {
        void OnClick(int pos);
    }

    public class PopupFilterSingleChoiceHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.ln_ItemPopupFilter)
        LinearLayout lnFilter;
        @BindView(R.id.tv_ItemPopupFilter_Title)
        TextView tvTitle;
        @BindView(R.id.img_ItemPopupFilter_Check)
        ImageView imgCheck;
        @BindView(R.id.viewLine)
        View viewLine;

        public PopupFilterSingleChoiceHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
            lnFilter.setOnClickListener(v -> listener.OnClick(getAdapterPosition()));
        }
    }

}

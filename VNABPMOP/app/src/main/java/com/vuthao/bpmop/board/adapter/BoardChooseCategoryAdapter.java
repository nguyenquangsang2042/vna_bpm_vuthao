package com.vuthao.bpmop.board.adapter;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.RecyclerView;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.model.app.WorkflowCategory;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;

public class BoardChooseCategoryAdapter extends RecyclerView.Adapter<BoardChooseCategoryAdapter.BoardChooseCategoryHolder> {
    private final Context context;
    private final ArrayList<WorkflowCategory> categories;
    private final BoardChooseCategoryListener listener;

    public interface BoardChooseCategoryListener {
        void OnClick(int pos);
    }

    public BoardChooseCategoryAdapter(Context context, ArrayList<WorkflowCategory> categories, BoardChooseCategoryListener listener) {
        this.context = context;
        this.categories = categories;
        this.listener = listener;
    }

    @NonNull
    @Override
    public BoardChooseCategoryHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_form_control_choice, parent, false);
        return new BoardChooseCategoryHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull BoardChooseCategoryAdapter.BoardChooseCategoryHolder holder, int position) {
        WorkflowCategory category = categories.get(position);
        if (position == categories.size() - 1) {
            holder.vLine.setVisibility(View.GONE);
        } else {
            holder.vLine.setVisibility(View.VISIBLE);
        }

        if (category != null) {
            if (category.isSelected()) {
                holder.imgIsChecked.setVisibility(View.VISIBLE);
                holder.tvTitle.setTextColor(ContextCompat.getColor(context, R.color.clBottomEnable));
            } else {
                holder.imgIsChecked.setVisibility(View.GONE);
                holder.tvTitle.setTextColor(ContextCompat.getColor(context, R.color.clBlack));
            }

            holder.tvTitle.setText(category.getTitle());
        }
    }

    @Override
    public int getItemCount() {
        return categories.size();
    }

    public class BoardChooseCategoryHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.tv_ItemFormControlChoice_Title)
        TextView tvTitle;
        @BindView(R.id.img_ItemFormControlChoice_Check)
        ImageView imgIsChecked;
        @BindView(R.id.view_ItemHomePageToDoList_Category)
        View vLine;

        public BoardChooseCategoryHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);

            itemView.setOnClickListener(v -> listener.OnClick(getAdapterPosition()));
        }
    }
}

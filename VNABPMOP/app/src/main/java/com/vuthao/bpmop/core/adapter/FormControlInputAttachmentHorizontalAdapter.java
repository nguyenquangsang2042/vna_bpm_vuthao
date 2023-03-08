package com.vuthao.bpmop.core.adapter;

import android.app.Activity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.RecyclerView;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.custom.AttachFileCategory;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;

public class FormControlInputAttachmentHorizontalAdapter extends RecyclerView.Adapter<FormControlInputAttachmentHorizontalAdapter.FormControlInputAttachmentHorizontalHolder> {
    private Activity activity;
    private ArrayList<AttachFileCategory> categories;
    private FormControlInputAttachmentHorizontalListener listener;

    public interface FormControlInputAttachmentHorizontalListener {
        void OnClick(int pos);
    }

    public FormControlInputAttachmentHorizontalAdapter(Activity activity, ArrayList<AttachFileCategory> categories, FormControlInputAttachmentHorizontalListener listener) {
        this.activity = activity;
        this.categories = categories;
        this.listener = listener;
    }

    @NonNull
    @Override
    public FormControlInputAttachmentHorizontalHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_form_control_choice, parent, false);
        return new FormControlInputAttachmentHorizontalHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull FormControlInputAttachmentHorizontalHolder holder, int position) {
        AttachFileCategory category = categories.get(position);
        if (category != null) {
            if (category.isSelected()) {
                holder.imgIsChecked.setVisibility(View.VISIBLE);
                holder.tvTitle.setTextColor(ContextCompat.getColor(activity, R.color.clBottomEnable));
            } else {
                holder.imgIsChecked.setVisibility(View.GONE);
                holder.tvTitle.setTextColor(ContextCompat.getColor(activity, R.color.clBlack));
            }

            if (!Functions.isNullOrEmpty(category.getTitle())) {
                holder.tvTitle.setText(category.getTitle());
            } else {
                holder.tvTitle.setText("");
            }
        }
    }

    @Override
    public int getItemCount() {
        return categories.size();
    }

    public class FormControlInputAttachmentHorizontalHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.tv_ItemFormControlChoice_Title)
        TextView tvTitle;
        @BindView(R.id.img_ItemFormControlChoice_Check)
        ImageView imgIsChecked;

        public FormControlInputAttachmentHorizontalHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
            itemView.setOnClickListener(view -> listener.OnClick(getAdapterPosition()));
        }
    }
}

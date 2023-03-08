package com.vuthao.bpmop.shareview.adapter;

import android.app.Activity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.LinearLayout;
import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.core.component.ComponentBase;
import com.vuthao.bpmop.core.component.ComponentRow1;
import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;

public class PopupFilterListAdapter extends RecyclerView.Adapter<PopupFilterListAdapter.PopupFilterListHolder> {
    private final Activity activity;
    private final ArrayList<ViewElement> elements;
    private ArrayList<ViewElement> elementsFilter = new ArrayList<>();

    public PopupFilterListAdapter(Activity activity, ArrayList<ViewElement> elements) {
        this.activity = activity;
        this.elements = elements;
        this.elementsFilter.addAll(this.elements);
    }

    @NonNull
    @Override
    public PopupFilterListHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_detail_workflow, parent, false);
        return new PopupFilterListHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull PopupFilterListHolder holder, int position) {
        holder.lnContent.removeAllViews();
        ComponentBase components = new ComponentRow1(activity, holder.lnContent, elements.get(position), -1, true, 1);
        components.initializeFrameView(holder.lnContent);
        components.setTitle();
        components.setValue();
        components.setEnable();
        components.setProprety();
    }

    @Override
    public int getItemCount() {
        return elements.size();
    }

    public static class PopupFilterListHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.ln_ItemExpandDetailWorkflow_Content)
        LinearLayout lnContent;

        public PopupFilterListHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);

        }
    }
}

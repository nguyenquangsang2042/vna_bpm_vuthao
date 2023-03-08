package com.vuthao.bpmop.core.adapter;

import android.app.Activity;
import android.util.DisplayMetrics;
import android.util.Pair;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.RecyclerView;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Functions;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;

public class ControlTabsAdapter extends RecyclerView.Adapter<ControlTabsAdapter.ControlTabsHolder> {
    public interface ControlTabsListener {
        void OnClick(int pos);
    }

    private Activity _mainAct;
    private ArrayList<Pair<String, String>> lstTabs;
    private int selectedPostion = -1;
    private int widthScreenTablet = -1;
    private ControlTabsAdapter.ControlTabsListener listener;


    public ControlTabsAdapter(Activity mainAct, ArrayList<Pair<String, String>> lstTabs, int selectedPostion, int widthScreenTablet, ControlTabsAdapter.ControlTabsListener listener) {
        this._mainAct = mainAct;
        this.lstTabs = lstTabs;
        this.selectedPostion = selectedPostion;
        this.widthScreenTablet = widthScreenTablet;
        this.listener = listener;
    }

    @NonNull
    @Override
    public ControlTabsHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_control_tabs, parent, false);
        if (lstTabs.size() > 0) {
            int widthRow;
            if (widthScreenTablet == -1) {
                DisplayMetrics dm = _mainAct.getResources().getDisplayMetrics();
                widthRow = dm.widthPixels / lstTabs.size();
            } else {
                widthRow = widthScreenTablet / lstTabs.size();
            }
            view.setLayoutParams(new ViewGroup.LayoutParams(widthRow - (int) Functions.share.convertDpToPixel(10, parent.getContext()), ViewGroup.LayoutParams.WRAP_CONTENT));
        }

        return new ControlTabsHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull ControlTabsAdapter.ControlTabsHolder holder, int position) {
        if (position == selectedPostion) {
            holder.tvTitle.setBackgroundResource(R.drawable.item_control_tabs_selected);
            holder.tvTitle.setTextColor(ContextCompat.getColor(_mainAct, R.color.clWhite));
        } else {
            holder.tvTitle.setBackgroundResource(R.drawable.item_control_tabs_unselected);
            holder.tvTitle.setTextColor(ContextCompat.getColor(_mainAct, R.color.clBlueEnable));
        }

        if (!Functions.isNullOrEmpty(lstTabs.get(position).second)) {
            holder.tvTitle.setText(lstTabs.get(position).second);
        } else {
            holder.tvTitle.setText("");
        }
    }

    @Override
    public int getItemCount() {
        return lstTabs.size();
    }


    public class ControlTabsHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.tv_ItemControlTabs_Title)
        TextView tvTitle;

        public ControlTabsHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
        }
    }
}


package com.vuthao.bpmop.core.adapter;

import android.annotation.SuppressLint;
import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.ImageView;
import android.widget.TextView;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.model.custom.ButtonAction;

import java.util.ArrayList;

public class ControlActionMoreAdapter extends BaseAdapter {
    private Context context;
    private ArrayList<ButtonAction> lstAction;

    public ControlActionMoreAdapter(Context context, ArrayList<ButtonAction> lstAction) {
        this.context = context;
        this.lstAction = lstAction;
    }

    @Override
    public int getCount() {
        return lstAction.size();
    }

    @Override
    public Object getItem(int position) {
        return position;
    }

    @Override
    public long getItemId(int position) {
        return position;
    }

    @SuppressLint("ViewHolder")
    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
        LayoutInflater inflater = LayoutInflater.from(context);
        View rootView = inflater.inflate(R.layout.item_popup_action, null);

        TextView tvTitle = rootView.findViewById(R.id.tv_ItemPopupAction);
        ImageView img = rootView.findViewById(R.id.img_ItemPopupAction);
        View viewLine = rootView.findViewById(R.id.view_ItemPopupAction);

        if (position == lstAction.size() - 1) {
            viewLine.setVisibility(View.GONE);
        } else {
            viewLine.setVisibility(View.VISIBLE);
        }

        tvTitle.setText(lstAction.get(position).getTitle());

        String nameOfImage = "icon_bpm_btn_action_" + String.valueOf(lstAction.get(position).getID());
        int drawableId = context.getResources().getIdentifier(nameOfImage.toLowerCase(), "drawable", context.getPackageName());
        img.setImageResource(drawableId);
        return rootView;
    }
}

package com.vuthao.bpmop.leftmenu.adapter;

import android.app.Activity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.ImageLoader;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.Workflow;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;

public class ApplicationAdapter extends RecyclerView.Adapter<ApplicationAdapter.ApplicationHolder> {
    private Activity activity;
    private ArrayList<Workflow> workflows;
    private  ApplicationListener listener;

    public interface ApplicationListener {
        void OnClick(int pos);
    }

    public ApplicationAdapter(Activity activity, ArrayList<Workflow> workflows, ApplicationListener listener) {
        this.activity = activity;
        this.workflows = workflows;
        this.listener = listener;
    }

    @NonNull
    @Override
    public ApplicationHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_left_menu_app, parent, false);
        return new ApplicationHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull ApplicationAdapter.ApplicationHolder holder, int position) {
        Workflow workflow = workflows.get(position);
        if (workflow != null) {
            if (!Functions.isNullOrEmpty(workflow.getImageURL())) {
                holder.img.setVisibility(View.VISIBLE);
                ImageLoader.getInstance().loadImageUserWithToken(activity, Constants.BASE_URL + workflow.getImageURL(), holder.img);
            } else {
                holder.img.setImageResource(R.drawable.icon_ver3_app);
            }

            if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
                holder.tvTitle.setText(workflow.getTitle());
            } else {
                holder.tvTitle.setText(workflow.getTitleEN());
            }
        }
    }

    @Override
    public int getItemCount() {
        return workflows.size();
    }

    public class ApplicationHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.tv_ItemLeftMenuApp_Title)
        TextView tvTitle;
        @BindView(R.id.img_ItemLeftMenuApp)
        ImageView img;

        public ApplicationHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);

            itemView.setOnClickListener(v -> listener.OnClick(getAdapterPosition()));
        }
    }
}

package com.vuthao.bpmop.core.component;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.util.DisplayMetrics;
import android.util.Pair;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;
import androidx.recyclerview.widget.StaggeredGridLayoutManager;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;

public class ControlAttachment extends ControlBase implements ControlBase.ControlAttachmentHorizontalListener {
    private final ViewElement element;
    private RecyclerView recyAttachment;
    public int widthScreenTablet = -1;
    private final ArrayList<Pair<String, String>> lstAttachment = new ArrayList<Pair<String, String>>();

    public ControlAttachment(Activity mainAct, LinearLayout parentView, ViewElement element, int widthScreenTablet)
    {
        super(mainAct);
        this.mainAct = mainAct;
        this.element = element;
        this.widthScreenTablet = widthScreenTablet;
        initializeComponent();
    }

    @Override
    public void initializeComponent() {
        super.initializeComponent();
        recyAttachment = new RecyclerView(mainAct);
    }

    @Override
    public void initializeFrameView(LinearLayout frame) {
        if (element.isHidden()) return;
        super.initializeFrameView(frame);

        tvValue.setVisibility(View.GONE);
        lnLine.setVisibility(View.GONE);
        lnContent.removeView(lnLine);
        lnContent.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT));

        frame.addView(recyAttachment);
        frame.addView(lnLine);
    }

    @SuppressLint("SetTextI18n")
    @Override
    public void setTitle() {
        super.setTitle();

        tvTitle.setText(element.getTitle());
        if (element.isRequire() && element.isEnable()) {
            tvTitle.setText(tvTitle.getText() + " (*)");
            Functions.share.setTVHighlightControl(mainAct, tvTitle);
        }
    }

    @Override
    public void setValue() {
        super.setValue();

        String data = element.getValue().trim();
        if (data.contains(";#")) {
            String[] arrAttachment = data.split(";#");
            if (arrAttachment.length > 2) {
                for (int i = 0; i < arrAttachment.length; i++) {
                    Pair<String, String> item = new Pair<>(arrAttachment[i], arrAttachment[i + 1]);
                    lstAttachment.add(item);
                }
            } else {
                lstAttachment.add(new Pair<>(arrAttachment[0], arrAttachment[1]));
            }
        }

        StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayout.HORIZONTAL);
        ControlAttachmentHorizontalAdapter _adapterAttachFile = new ControlAttachmentHorizontalAdapter(this, mainAct, lstAttachment, widthScreenTablet);
        recyAttachment.setLayoutManager(staggeredGridLayoutManager);
        recyAttachment.setAdapter(_adapterAttachFile);
    }

    @Override
    public void OnClick(int pos) {
        // attach Click to detail
    }

    public class ControlAttachmentHorizontalAdapter extends RecyclerView.Adapter<ControlAttachmentHorizontalAdapter.ControlAttachmentHorizontalHolder>  {
        public Activity mainAct;
        public ArrayList<Pair<String, String>> lstAttachment;
        public int widthScreenTablet = -1;

        public ControlAttachmentHorizontalAdapter(ControlAttachmentHorizontalListener listener, Activity mainAct, ArrayList<Pair<String, String>> lstAttachment, int widthScreenTablet) {
            this.mainAct = mainAct;
            this.lstAttachment = lstAttachment;
            this.widthScreenTablet = widthScreenTablet;
        }

        @NonNull
        @Override
        public ControlAttachmentHorizontalHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_control_attachment_horizontal, parent, false);
            int widthRow;
            if (widthScreenTablet == -1) {
                DisplayMetrics dm = mainAct.getResources().getDisplayMetrics();
                widthRow = dm.widthPixels / 3;
            } else {
                widthRow = widthScreenTablet / 3;
            }

            view.setLayoutParams(new ViewGroup.LayoutParams(widthRow, ViewGroup.LayoutParams.WRAP_CONTENT));

            return new ControlAttachmentHorizontalHolder(view);
        }

        @Override
        public void onBindViewHolder(@NonNull ControlAttachment.ControlAttachmentHorizontalAdapter.ControlAttachmentHorizontalHolder holder, int position) {
            Pair<String,String> currentAttachment = lstAttachment.get(position);
            if (!Functions.isNullOrEmpty(currentAttachment.second)) {
                holder.tvFileName.setText(currentAttachment.second);
            } else {
                holder.tvFileName.setText("");
            }
        }

        @Override
        public int getItemCount() {
            return lstAttachment.size();
        }

        public class ControlAttachmentHorizontalHolder extends RecyclerView.ViewHolder {
            @BindView(R.id.img_ItemControlAttachmentHorizontal_Avatar)
            ImageView imgAvatar;
            @BindView(R.id.tv_ItemControlAttachmentHorizontal_FileName)
            TextView tvFileName;
            public ControlAttachmentHorizontalHolder(@NonNull View itemView) {
                super(itemView);
                ButterKnife.bind(this, itemView);
            }
        }
    }
}

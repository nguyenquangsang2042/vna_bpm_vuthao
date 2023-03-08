package com.vuthao.bpmop.core.component;

import android.annotation.SuppressLint;
import android.app.Activity;
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
import com.vuthao.bpmop.core.adapter.ControlAttachmentVerticalAdapter;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;

public class ControlAttachmentVertical extends ControlBase implements ControlBase.ControlAttachmentVerticalListener {
    private final ViewElement element;
    private RecyclerView recyAttachment;
    private final ArrayList<Pair<String, String>> lstAttachment = new ArrayList<Pair<String, String>>();

    public ControlAttachmentVertical(Activity mainAct, LinearLayout parentView, ViewElement element) {
        super(mainAct);
        this.mainAct = mainAct;
        this.element = element;
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
        recyAttachment.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, 3 * Functions.share.convertDpToPixel(40, frame.getContext()))); // default 3 item);

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
                for (int i = 0; i < arrAttachment.length; i+=2) {
                    Pair<String, String> item = new Pair<>(arrAttachment[i], arrAttachment[i+1]);
                    lstAttachment.add(item);
                }
            } else {
                lstAttachment.add(new Pair<>(arrAttachment[0], arrAttachment[1]));
            }
        }

        StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, LinearLayout.VERTICAL);
        ControlAttachmentVerticalAdapter attachmentVerticalAdapter = new ControlAttachmentVerticalAdapter(this, mainAct, lstAttachment);
        recyAttachment.setLayoutManager(staggeredGridLayoutManager);
        recyAttachment.setAdapter(attachmentVerticalAdapter);
    }

    @Override
    public void OnClick(int pos) {
        // attch to detail clicks
    }
}

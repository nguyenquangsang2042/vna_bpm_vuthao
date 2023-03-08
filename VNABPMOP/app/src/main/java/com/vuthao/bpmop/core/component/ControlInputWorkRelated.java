package com.vuthao.bpmop.core.component;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Context;
import android.content.res.ColorStateList;
import android.graphics.Typeface;
import android.text.TextUtils;
import android.util.TypedValue;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.core.content.res.ResourcesCompat;
import androidx.recyclerview.widget.RecyclerView;
import androidx.recyclerview.widget.StaggeredGridLayoutManager;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.app.WorkflowItem;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.core.adapter.ControlLinkedWorkflowAdapter;
import com.vuthao.bpmop.detail.custom.DetailFunc;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;
import de.hdodenhof.circleimageview.CircleImageView;

public class ControlInputWorkRelated extends ControlBase implements ControlBase.ControlLinkedWorkflowListener {
    private Context context;
    private final LinearLayout parentView;
    private RecyclerView recyAttachment;
    private final ViewElement element;

    private int widthScreenTablet = -1;
    private final StaggeredGridLayoutManager staggeredGridLayoutManager;

    public ControlInputWorkRelated(Activity mainAct, LinearLayout parentView, ViewElement element, int widthScreenTablet) {
        super(mainAct);
        this.mainAct = mainAct;
        this.parentView = parentView;
        this.element = element;
        this.widthScreenTablet = widthScreenTablet;
        staggeredGridLayoutManager  = new StaggeredGridLayoutManager(1, LinearLayout.VERTICAL);
        initializeComponent();
    }

    @SuppressLint("SetTextI18n")
    @Override
    public void initializeComponent() {
        super.initializeComponent();
        recyAttachment = new RecyclerView(mainAct);
        LinearLayout lnFileInfo = new LinearLayout(mainAct);
        TextView tvFileInfoChild1 = new TextView(mainAct);
        TextView tvFileInfoChild2 = new TextView(mainAct);
        TextView tvFileInfoChild3 = new TextView(mainAct);

        tvTitle.setTextSize(TypedValue.COMPLEX_UNIT_SP, 16);
        tvTitle.setTextColor(ContextCompat.getColor(mainAct, R.color.clViolet));
        tvTitle.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.BOLD);
        tvTitle.setEllipsize(TextUtils.TruncateAt.END);
        tvTitle.setGravity(Gravity.CENTER);

        tvFileInfoChild1.setTextSize(TypedValue.COMPLEX_UNIT_SP, 14);
        tvFileInfoChild1.setTextColor(ContextCompat.getColor(mainAct, R.color.clBlack));
        tvFileInfoChild1.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.BOLD);
        tvFileInfoChild1.setEllipsize(TextUtils.TruncateAt.END);
        tvFileInfoChild1.setGravity(Gravity.CENTER);
        tvFileInfoChild1.setText("Mã phiếu");

        tvFileInfoChild2.setTextSize(TypedValue.COMPLEX_UNIT_SP, 16);
        tvFileInfoChild2.setTextColor(ContextCompat.getColor(mainAct, R.color.clBlack));
        tvFileInfoChild2.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.BOLD);
        tvFileInfoChild2.setEllipsize(TextUtils.TruncateAt.END);
        tvFileInfoChild2.setGravity(Gravity.CENTER);
        tvFileInfoChild2.setText("Nội dung");

        tvFileInfoChild3.setTextSize(TypedValue.COMPLEX_UNIT_SP, 16);
        tvFileInfoChild3.setTextColor(ContextCompat.getColor(mainAct, R.color.clBlack));
        tvFileInfoChild3.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.BOLD);
        tvFileInfoChild3.setEllipsize(TextUtils.TruncateAt.END);
        tvFileInfoChild3.setGravity(Gravity.CENTER);
        tvFileInfoChild3.setText("Tình trạng");

        recyAttachment.setId(Constants.mFileCode);
        lnFileInfo.setOrientation(LinearLayout.HORIZONTAL);
    }

    @Override
    public void initializeFrameView(LinearLayout frame) {
        if (element.isHidden()) return;
        context = frame.getContext();
        super.initializeFrameView(frame);

        tvValue.setVisibility(View.GONE);
        lnLine.setVisibility(View.GONE);

        lnContent.removeView(lnLine);
        int padding = (int) TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, 6, mainAct.getResources().getDisplayMetrics());
        LinearLayout.LayoutParams paramsTitle = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WRAP_CONTENT, LinearLayout.LayoutParams.WRAP_CONTENT);

        tvTitle.setLayoutParams(paramsTitle);
        recyAttachment.setRecycledViewPool(new RecyclerView.RecycledViewPool());

        LinearLayout.LayoutParams paramsRecy = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT);
        paramsRecy.setMargins(0, 0, 0, 2 * padding);
        recyAttachment.setLayoutParams(paramsRecy);

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
        String data = element.getValue();
        ArrayList<WorkflowItem> lstWorkflowItem = new Gson().fromJson(data, new TypeToken<ArrayList<WorkflowItem>>() {
        }.getType());

        ControlLinkedWorkflowAdapter linkedWorkflowAdapter = new ControlLinkedWorkflowAdapter(mainAct, context, lstWorkflowItem, this, widthScreenTablet);
        recyAttachment.setLayoutManager(staggeredGridLayoutManager);
        recyAttachment.setAdapter(linkedWorkflowAdapter);
    }

    @Override
    public void OnDeleteItemClick(int pos) {
        if (parentView != null) {
            // attach clicks to detail
        }
    }
}

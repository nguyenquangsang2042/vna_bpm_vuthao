package com.vuthao.bpmop.core.component;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Context;
import android.content.Intent;
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
import androidx.cardview.widget.CardView;
import androidx.core.content.ContextCompat;
import androidx.core.content.res.ResourcesCompat;
import androidx.localbroadcastmanager.content.LocalBroadcastManager;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.recyclerview.widget.StaggeredGridLayoutManager;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.AnimationController;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.SwipeHelper;
import com.vuthao.bpmop.base.custom.expandable.AnimatedExpandableListAdapter;
import com.vuthao.bpmop.base.custom.expandable.AnimatedExpandableListView;
import com.vuthao.bpmop.base.model.custom.AttachFile;
import com.vuthao.bpmop.base.model.custom.GroupAttachFile;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.core.Vars;
import com.vuthao.bpmop.core.adapter.ExpandControlInputAttachmentVerticalAdapter;
import com.vuthao.bpmop.core.controller.ControllerDetailAttachFile;
import com.vuthao.bpmop.detail.custom.DetailFunc;

import java.util.ArrayList;
import java.util.List;

import butterknife.BindView;
import butterknife.ButterKnife;

public class ControlInputAttachmentVertical extends ControlBase {
    private Context context;
    private CardView cardViewExpandable;
    private LinearLayout lnCardView; // linear parent Card View
    private LinearLayout lnExpandable;
    private LinearLayout lnTitleImport; // chứa _tvTitle và _lnImport
    private LinearLayout lnImport;
    private LinearLayout lnFileInfo;
    private LinearLayout lnFileInfoChild2;
    private LinearLayout lnFileInfoChild3;
    private TextView tvFileInfoChild2;
    private TextView tvFileInfoChild3;
    private ImageView imgImport;
    private TextView tvImport;
    private AnimatedExpandableListView expandAttachment;
    private ViewElement element;

    private ExpandControlInputAttachmentVerticalAdapter adapter;
    private final ControllerDetailAttachFile CTRLDetailAttachFile = new ControllerDetailAttachFile();
    // Flag này để ẩn 1 số chức năng vì trang chi tiết ko cần hiển thị đủ - 1: DetailWorkflow, 2: DetailAttachFile, 3 - DetailCreateTask
    private int flagView;

    public ControlInputAttachmentVertical(Activity mainAct) {
        super(mainAct);
    }

    public ControlInputAttachmentVertical(Activity mainAct, LinearLayout parentView, ViewElement element, int widthScreenTablet, int flagView) {
        super(mainAct);
        this.mainAct = mainAct;
        this.element = element;
        this.flagView = flagView;
        initializeComponent();
    }

    @Override
    public void initializeComponent() {
        super.initializeComponent();
        expandAttachment = new AnimatedExpandableListView(mainAct);
        cardViewExpandable = new CardView(mainAct);
        lnCardView = new LinearLayout(mainAct);
        lnExpandable = new LinearLayout(mainAct);
        lnTitleImport = new LinearLayout(mainAct);
        lnFileInfo = new LinearLayout(mainAct);
        lnFileInfoChild2 = new LinearLayout(mainAct);
        lnFileInfoChild3 = new LinearLayout(mainAct);
        tvFileInfoChild2 = new TextView(mainAct);
        tvFileInfoChild3 = new TextView(mainAct);
        lnImport = new LinearLayout(mainAct);
        imgImport = new ImageView(mainAct);
        tvImport = new TextView(mainAct);

        lnExpandable.setOrientation(LinearLayout.VERTICAL);

        tvTitle.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.BOLD);
        tvTitle.setTextColor(ContextCompat.getColor(mainAct, R.color.clBlack));

        tvImport.setTextSize(TypedValue.COMPLEX_UNIT_SP, 12);
        tvImport.setTextColor(ContextCompat.getColor(mainAct, R.color.clBottomDisable));
        tvImport.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.NORMAL);
        tvImport.setEllipsize(TextUtils.TruncateAt.END);
        tvImport.setGravity(Gravity.CENTER);

        tvFileInfoChild2.setTextSize(TypedValue.COMPLEX_UNIT_SP, 12);
        tvFileInfoChild2.setTextColor(ContextCompat.getColor(mainAct, R.color.clBlack));
        tvFileInfoChild2.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.BOLD);
        tvFileInfoChild2.setEllipsize(TextUtils.TruncateAt.END);
        tvFileInfoChild2.setGravity(Gravity.CENTER);

        tvFileInfoChild3.setTextSize(TypedValue.COMPLEX_UNIT_SP, 12);
        tvFileInfoChild3.setTextColor(ContextCompat.getColor(mainAct, R.color.clBlack));
        tvFileInfoChild3.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.BOLD);
        tvFileInfoChild3.setEllipsize(TextUtils.TruncateAt.END);
        tvFileInfoChild3.setGravity(Gravity.CENTER);

        tvImport.setText(Functions.share.getTitle("TEXT_ADDNEW_ATTACHMENT", "Tạo mới"));
        tvFileInfoChild2.setText(Functions.share.getTitle("TEXT_CONTROL_DOCUMENTNAME", "Tên tài liệu"));
        tvFileInfoChild3.setText(Functions.share.getTitle("TEXT_CONTROL_CREATOR", "Người tạo"));

        imgImport.setColorFilter(ContextCompat.getColor(mainAct, R.color.clBottomEnable));

        cardViewExpandable.setUseCompatPadding(true);
        cardViewExpandable.setRadius(5f);

        lnFileInfo.setOrientation(LinearLayout.HORIZONTAL);
        lnTitleImport.setOrientation(LinearLayout.HORIZONTAL);
        lnTitleImport.setGravity(Gravity.CENTER);
        lnImport.setOrientation(LinearLayout.HORIZONTAL);
        lnImport.setGravity(Gravity.RIGHT);
        tvImport.setGravity(Gravity.BOTTOM);

        lnCardView.setOrientation(LinearLayout.VERTICAL);

        expandAttachment.setGroupIndicator(null);
        expandAttachment.setChildIndicator(null);
        expandAttachment.setDividerHeight(0);
        expandAttachment.setNestedScrollingEnabled(true);
    }

    @Override
    public void initializeFrameView(LinearLayout frame) {
        if (element.isHidden()) return;
        context = frame.getContext();
        super.initializeFrameView(frame);

        tvValue.setVisibility(View.GONE);
        lnLine.setVisibility(View.GONE);
        lnContent.removeView(lnLine);
        lnContent.removeView(tvTitle);

        int padding = (int) TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, 6, mainAct.getResources().getDisplayMetrics());

        LinearLayout.LayoutParams paramslnTitleImport = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT);
        LinearLayout.LayoutParams paramsTitle = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WRAP_CONTENT, LinearLayout.LayoutParams.WRAP_CONTENT);
        LinearLayout.LayoutParams paramslnImport = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT);
        LinearLayout.LayoutParams paramsimgImport = new LinearLayout.LayoutParams(Functions.share.convertDpToPixel(20, frame.getContext()), Functions.share.convertDpToPixel(20, frame.getContext()));
        paramsimgImport.setMargins(padding, 0, 2 * padding, 0);
        paramslnTitleImport.setMargins(0, 0, 0, padding);
        lnTitleImport.setLayoutParams(paramslnTitleImport);
        tvTitle.setLayoutParams(paramsTitle);
        lnImport.setLayoutParams(paramslnImport);
        imgImport.setLayoutParams(paramsimgImport);

        imgImport.setBackground(ContextCompat.getDrawable(frame.getContext(), R.drawable.icon_ver2_addfile));
        imgImport.setPadding(padding, 0, padding, 0);
        lnImport.setPadding(padding, 0, 2 * padding, 0);

        if (element.isEnable()) {
            lnImport.setVisibility(View.VISIBLE);
            lnImport.setOnClickListener(v -> {
                Intent intent = new Intent();
                intent.setAction(VarsReceiver.INNERACTIONCLICK);
                intent.putExtra("element", new Gson().toJson(element));
                intent.putExtra("actionId", Vars.ControlInputGridDetails_InnerActionID.Create);
                intent.putExtra("positionToAction", -1);
                intent.putExtra("flagViewID", flagView);
                BroadcastUtility.send(mainAct, intent);
            });
        } else {
            lnImport.setVisibility(View.INVISIBLE);
        }

        lnImport.addView(imgImport);
        lnImport.addView(tvImport);
        lnTitleImport.addView(tvTitle);
        lnTitleImport.addView(lnImport);

        // File info
        LinearLayout.LayoutParams paramsLnInfo = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT);
        LinearLayout.LayoutParams paramsLnInfoChild2 = new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WRAP_CONTENT, 0.65f);
        LinearLayout.LayoutParams paramsLnInfoChild3 = new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WRAP_CONTENT, 0.35f);

        lnFileInfo.setLayoutParams(paramsLnInfo);
        lnFileInfo.setPadding(padding, 2 * padding, padding, 2 * padding);
        lnFileInfo.setBackgroundColor(ContextCompat.getColor(context, R.color.clGraySearchUser));

        lnFileInfoChild2.setLayoutParams(paramsLnInfoChild2);
        lnFileInfoChild3.setLayoutParams(paramsLnInfoChild3);

        lnFileInfoChild2.setPadding(padding, padding, padding, padding);
        lnFileInfoChild3.setPadding(padding, padding, padding, padding);

        lnFileInfoChild2.addView(tvFileInfoChild2);
        lnFileInfoChild3.addView(tvFileInfoChild3);

        lnFileInfo.addView(lnFileInfoChild2);
        lnFileInfo.addView(lnFileInfoChild3);
        //end

        LinearLayout.LayoutParams paramsCardView = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT);
        cardViewExpandable.setLayoutParams(paramsCardView);
        lnCardView.setLayoutParams(paramsCardView);

        frame.setPadding(padding, 0, padding, 0);

        if (flagView == Vars.FlagViewControlAttachment.DetailWorkflow || flagView == Vars.FlagViewControlAttachment.DetailCreateTask) {
            // Trang chi tiết file đính kèm chỉ hiện List
            frame.addView(lnTitleImport);
            lnCardView.addView(lnFileInfo);
        }

        lnCardView.addView(lnExpandable);
        cardViewExpandable.addView(lnCardView);
        frame.addView(cardViewExpandable);
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
        String data = "";
        if (!Functions.isNullOrEmpty(element.getValue())) {
            data = element.getValue().trim();
        }

        int groupHeight = Functions.share.convertDpToPixel(45, context); // Group View height = 45dp
        int childHeight = Functions.share.convertDpToPixel(60, context); // child item view height = 60dp

        ArrayList<AttachFile> files = new Gson().fromJson(data, new TypeToken<ArrayList<AttachFile>>() {
        }.getType());

        if (files == null) {
            files = new ArrayList<>();
        }

        ArrayList<GroupAttachFile> groupAttachFiles = CTRLDetailAttachFile.cloneListAttachFiles(files);

        // adapter expandable
        adapter = new ExpandControlInputAttachmentVerticalAdapter(context, mainAct, groupAttachFiles, files, element, flagView);
        expandAttachment.setAdapter(adapter);

        for (int i = 0; i < adapter.getGroupCount(); i++) {
            expandAttachment.expandGroup(i);
        }

        expandAttachment.setOnGroupExpandListener(groupPosition -> {
            expandAttachment.getLayoutParams().height += adapter.getChildrenCount(groupPosition) * childHeight;
            adapter.notifyDataSetChanged();
        });

        expandAttachment.setOnGroupCollapseListener(groupPosition -> {
            expandAttachment.getLayoutParams().height -= adapter.getChildrenCount(groupPosition) * childHeight;
            adapter.notifyDataSetChanged();
        });

        expandAttachment.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, lnExpandable.getLayoutParams().height));
        lnCardView.removeView(lnExpandable);
        lnCardView.addView(expandAttachment);
        expandAttachment.startAnimation(AnimationController.share.fadeIn(mainAct));
        expandAttachment.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, (childHeight * files.size()) + (groupHeight * groupAttachFiles.size())));
    }
}

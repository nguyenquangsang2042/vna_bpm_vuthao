package com.vuthao.bpmop.core.component;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.graphics.Typeface;
import android.text.Editable;
import android.text.InputType;
import android.text.TextUtils;
import android.text.TextWatcher;
import android.util.TypedValue;
import android.view.Gravity;
import android.view.View;
import android.view.inputmethod.EditorInfo;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.cardview.widget.CardView;
import androidx.core.content.ContextCompat;
import androidx.core.content.res.ResourcesCompat;
import androidx.localbroadcastmanager.content.LocalBroadcastManager;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.custom.AttachFile;
import com.vuthao.bpmop.base.model.custom.Comment;
import com.vuthao.bpmop.base.realm.RealmController;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.core.adapter.ListCommentAdapter;
import com.vuthao.bpmop.core.adapter.ParentAttachFile_ImageAdapter;
import com.vuthao.bpmop.core.adapter.ParentAttachFileAdapter;
import com.vuthao.bpmop.detail.DetailWorkflowActivity;
import com.vuthao.bpmop.detail.custom.DetailFunc;
import com.vuthao.bpmop.task.DetailCreateTaskActivity;

import java.util.ArrayList;
import java.util.Comparator;
import java.util.Date;
import java.util.stream.Collectors;

public class ComponentComment extends ComponentBase implements ListCommentAdapter.ListCommentListener, TextWatcher {
    private final Activity mainAct;
    private final Context context;
    private final LinearLayout parentView;
    private TextView tvTitle;
    private CardView cardViewParentComment;
    private LinearLayout lnParentComment;
    private LinearLayout lnParentCommentContent;
    private LinearLayout lnParentCommentAttach;
    private LinearLayout lnTopLine;
    private EditText edtParentComment;
    private ImageView imgAttachParentComment;
    private ImageView imgCommentParentComment;
    private RecyclerView recyAttachParentComment_Other;
    private RecyclerView recyAttachParentComment_Image; // List đính kèm của Image
    private RecyclerView recyListComment;
    private final boolean isReplyView;
    private String parentCommentContent = "";

    private ParentAttachFileAdapter adapterParentAttach;
    private ParentAttachFile_ImageAdapter adapterParentAttach_Image;
    private ListCommentAdapter adapterListComment;

    private ArrayList<Comment> lstComment;
    private ArrayList<AttachFile> lstParentAttach = new ArrayList<AttachFile>();

    private LinearLayout.LayoutParams paramsRecyComment;

    // nếu đặt trong scrollview thì bật cái này lên
    private boolean flagRecalculateView = false;

    public ComponentComment(Activity mainAct, Context context, LinearLayout parentView, ArrayList<Comment> lstComment, boolean isReplyView) {
        this.mainAct = mainAct;
        this.context = context;
        this.parentView = parentView;
        this.lstComment = lstComment;
        this.isReplyView = isReplyView;
        initializeComponent();
    }

    public void initFlagRecalculateView(boolean flagRecalculateView) {
        this.flagRecalculateView = flagRecalculateView;
    }

    @Override
    public void initializeComponent() {
        super.initializeComponent();

        tvTitle = new TextView(mainAct);
        cardViewParentComment = new CardView(mainAct);
        lnParentComment = new LinearLayout(mainAct);
        lnParentCommentContent = new LinearLayout(mainAct);
        lnParentCommentAttach = new LinearLayout(mainAct);
        lnTopLine = new LinearLayout(mainAct);
        edtParentComment = new EditText(mainAct);
        imgAttachParentComment = new ImageView(mainAct);
        imgCommentParentComment = new ImageView(mainAct);
        recyAttachParentComment_Image = new RecyclerView(mainAct);
        recyAttachParentComment_Other = new RecyclerView(mainAct);
        recyListComment = new RecyclerView(mainAct);
        recyListComment.setNestedScrollingEnabled(false);

        tvTitle.setTextSize(TypedValue.COMPLEX_UNIT_SP, 12);
        tvTitle.setTextColor(ContextCompat.getColor(context, R.color.clBlack));
        tvTitle.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.BOLD);
        tvTitle.setEllipsize(TextUtils.TruncateAt.END);
        tvTitle.setGravity(Gravity.LEFT);

        edtParentComment.setTextSize(TypedValue.COMPLEX_UNIT_SP, 14);
        edtParentComment.setMaxLines(3);
        edtParentComment.setLines(3);
        edtParentComment.setSingleLine(false);
        edtParentComment.setImeOptions(EditorInfo.IME_ACTION_NONE);
        edtParentComment.setVerticalScrollBarEnabled(true);
        edtParentComment.setInputType(InputType.TYPE_CLASS_TEXT | InputType.TYPE_TEXT_FLAG_MULTI_LINE);
        edtParentComment.setGravity(Gravity.TOP);
        edtParentComment.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.ITALIC);
        edtParentComment.setBackgroundColor(ContextCompat.getColor(mainAct, R.color.transparent));

        cardViewParentComment.setBackgroundResource(R.drawable.textcornerstrokegraywhitesolid2);
        cardViewParentComment.setUseCompatPadding(true);
        cardViewParentComment.setRadius(5f);

        lnParentComment.setOrientation(LinearLayout.VERTICAL);
        lnParentCommentContent.setOrientation(LinearLayout.HORIZONTAL);
        lnParentCommentAttach.setOrientation(LinearLayout.VERTICAL);

        lnTopLine.setBackgroundColor(ContextCompat.getColor(mainAct, R.color.clControlGrayLight));
        lnTopLine.setOrientation(LinearLayout.VERTICAL);

        imgAttachParentComment.setImageResource(R.drawable.icon_ver2_attach);
        imgCommentParentComment.setImageResource(R.drawable.icon_ver2_comment);

        edtParentComment.setHint(Functions.share.getTitle("TEXT_HINT_REQUIRE_COMMENT", "Nhập ý kiến"));
    }

    @Override
    public void initializeFrameView(LinearLayout frame) {
        super.initializeFrameView(frame);

        int padding = (int) TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, 3, mainAct.getResources().getDisplayMetrics());
        LinearLayout.LayoutParams paramsParentComment = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT);
        LinearLayout.LayoutParams paramsEdtParentComment = new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WRAP_CONTENT, 1);
        LinearLayout.LayoutParams paramsImgParentComment = new LinearLayout.LayoutParams(Functions.share.convertDpToPixel(30, frame.getContext()), Functions.share.convertDpToPixel(30, frame.getContext()));
        paramsRecyComment = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT);
        LinearLayout.LayoutParams paramslnTopLine = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, Functions.share.convertDpToPixel(1, frame.getContext()));

        paramsParentComment.setMargins(0, padding, 0, 2 * padding);
        paramsEdtParentComment.setMargins(0, 0, 2 * padding, 0);
        paramsImgParentComment.setMargins(0, 2 * padding, 3 * padding, 0);
        paramsRecyComment.setMargins(0, padding, 0, 0);
        paramslnTopLine.setMargins(0, padding, 0, 2 * padding);

        edtParentComment.setLayoutParams(paramsEdtParentComment);
        imgAttachParentComment.setLayoutParams(paramsImgParentComment);
        imgCommentParentComment.setLayoutParams(paramsImgParentComment);

        recyAttachParentComment_Other.setLayoutParams(paramsParentComment);
        recyAttachParentComment_Image.setLayoutParams(paramsParentComment);

        lnTopLine.setLayoutParams(paramslnTopLine);
        recyListComment.setLayoutParams(paramsParentComment);

        tvTitle.setPadding(0, padding, 0, 2 * padding);
        lnParentComment.setPadding(padding, padding, padding, padding);
        imgAttachParentComment.setPadding(2 * padding, 2 * padding, 2 * padding, 2 * padding);
        imgCommentParentComment.setPadding(2 * padding, 2 * padding, 2 * padding, 2 * padding);

        lnParentCommentContent.addView(edtParentComment);
        lnParentCommentContent.addView(imgAttachParentComment);
        lnParentCommentContent.addView(imgCommentParentComment);
        lnParentCommentAttach.addView(recyAttachParentComment_Image);
        lnParentCommentAttach.addView(recyAttachParentComment_Other);
        lnParentComment.addView(lnParentCommentContent);
        lnParentComment.addView(lnParentCommentAttach);

        edtParentComment.addTextChangedListener(this);
        imgAttachParentComment.setOnClickListener(v -> {
            Intent intent = new Intent();
            intent.setAction(VarsReceiver.COMMENT);
            intent.putExtra("type", "attachfile");
            BroadcastUtility.send(mainAct, intent);
        });

        imgCommentParentComment.setOnClickListener(v -> {
            Intent intent = new Intent();
            intent.setAction(VarsReceiver.COMMENT);
            intent.putExtra("type", "comment");
            intent.putExtra("content", parentCommentContent);
            intent.putExtra("lstAttachfile", new Gson().toJson(lstParentAttach));
            BroadcastUtility.send(mainAct, intent);
        });

        if (!isReplyView) {
            cardViewParentComment.addView(lnParentComment);
            frame.setPadding(2 * padding, 2 * padding, 2 * padding, 2 * padding);
            frame.addView(lnTopLine);
            frame.addView(tvTitle);
            frame.addView(cardViewParentComment);
        } else {
            // View reply  ko cần padding -> đụng bottom keyboard
            frame.setPadding(0, 0, 0, 0);
        }

        frame.addView(recyListComment);
    }

    @Override
    public void setTitle() {
        super.setTitle();
        tvTitle.setText(Functions.share.getTitle("TEXT_COMMENT", "Bình luận"));
    }

    @Override
    public void setValue() {
        super.setValue();
        if (!Functions.isNullOrEmpty(parentCommentContent)) {
            edtParentComment.setText(parentCommentContent);
        }

        if (lstParentAttach != null) {
            lstParentAttach.size();
            if (lstParentAttach.size() > 0) {
                // phân loại nếu ảnh thì IsImage = true
                lstParentAttach = DetailFunc.share.classifyListAttachFile(lstParentAttach);
            }

            adapterParentAttach = new ParentAttachFileAdapter(mainAct, parentView.getContext(), lstParentAttach, true, isReplyView);
            recyAttachParentComment_Other.setAdapter(adapterParentAttach);
            recyAttachParentComment_Other.setLayoutManager(new LinearLayoutManager(parentView.getContext(), LinearLayoutManager.VERTICAL, false));

            adapterParentAttach_Image = new ParentAttachFile_ImageAdapter(mainAct, parentView.getContext(), lstParentAttach, true, isReplyView);
            recyAttachParentComment_Image.setAdapter(adapterParentAttach_Image);
            recyAttachParentComment_Image.setLayoutManager(new LinearLayoutManager(parentView.getContext(), RecyclerView.HORIZONTAL, false));
        }

        if (lstComment != null && lstComment.size() > 0) {
            setListComment(lstComment);
        }
    }

    private ArrayList<Comment> cloneListComment(ArrayList<Comment> lstComment) {
        ArrayList<Comment> result = new ArrayList<>();
        lstComment.sort((o1, o2) -> {
            Date t1Val = Functions.share.formatStringToDate(o1.getCreated());
            Date t2Val = Functions.share.formatStringToDate(o2.getCreated());
            return (t1Val.getTime() > t2Val.getTime() ? -1 : 1);     //descending
        });

        // List cha thằng mới nhất lên trên cùng 12/01/20
        ArrayList<Comment> lstParent = (ArrayList<Comment>) lstComment.stream().filter(r -> Functions.isNullOrEmpty(r.getParentCommentId())).collect(Collectors.toList());
        for (Comment comment : lstParent) {
            // List con thằng cũ nhất lên trên cùng 12/01/20
            ArrayList<Comment> lstChild = (ArrayList<Comment>) lstComment.stream().filter(r -> !Functions.isNullOrEmpty(r.getParentCommentId()) && r.getParentCommentId().equals(comment.getID())).collect(Collectors.toList());
            result.add(comment);
            result.addAll(lstChild);
        }
        return result;
    }

    public void updateListComment(ArrayList<Comment> comments) {
        if (adapterListComment != null) {
            comments = cloneListComment(comments);
            adapterListComment.updateListComment(comments);
        } else {
            setListComment(comments);
        }
    }

    public void notifyDataCommentChange() {
        if (adapterListComment != null) {
            adapterListComment.notifyDataSetChanged();
        }
    }

    private void setListComment(ArrayList<Comment> comments) {
        if (comments != null && comments.size() > 0) {
            lstComment = cloneListComment(lstComment);

            if (flagRecalculateView) {
                adapterListComment = new ListCommentAdapter(mainAct, parentView.getContext(), this, lstComment, isReplyView, mainAct.getResources().getDisplayMetrics().widthPixels - Functions.share.convertDpToPixel(12, context));
            } else {
                adapterListComment = new ListCommentAdapter(mainAct, parentView.getContext(), this, lstComment, isReplyView, -1);
            }

            recyListComment.setItemViewCacheSize(lstComment.size());
            recyListComment.setAdapter(adapterListComment);
            recyListComment.setLayoutManager(new LinearLayoutManager(parentView.getContext(), LinearLayoutManager.VERTICAL, false));

            /*if (flagRecalculateView) {
                calculateHeight();
            }*/
        }
    }

    private void calculateHeight() {
        recyListComment.post(() -> {
            int recyHeight = DetailFunc.share.getRecyclerViewHeight(recyListComment);
            int screenHeight = mainAct.getResources().getDisplayMetrics().heightPixels;

            if (recyHeight > screenHeight) {
                paramsRecyComment.width = LinearLayout.LayoutParams.WRAP_CONTENT;
                paramsRecyComment.height = screenHeight;
                recyListComment.setLayoutParams(paramsRecyComment);
            } else {
                paramsRecyComment.width = LinearLayout.LayoutParams.WRAP_CONTENT;
                paramsRecyComment.height = recyHeight;
                recyListComment.setLayoutParams(paramsRecyComment);
            }
        });
    }

    @Override
    public void beforeTextChanged(CharSequence s, int start, int count, int after) {

    }

    @Override
    public void onTextChanged(CharSequence s, int start, int before, int count) {

    }

    @Override
    public void afterTextChanged(Editable s) {
        if (s.length() > 0) {
            edtParentComment.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.ITALIC);
        } else {
            edtParentComment.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.NORMAL);
        }

        parentCommentContent = s.toString();
    }

    public void updateListParentAttach(ArrayList<AttachFile> lstParentAttach) {
        this.lstParentAttach = DetailFunc.share.classifyListAttachFile(lstParentAttach);
        if (adapterParentAttach_Image != null) {
            adapterParentAttach_Image.updateListAttach(this.lstParentAttach);
            adapterParentAttach_Image.notifyDataSetChanged();
        }

        if (adapterParentAttach != null) {
            adapterParentAttach.updateListAttach(this.lstParentAttach);
            adapterParentAttach.notifyDataSetChanged();
        }
    }

    public String getCurrentParentCommentContent() {
        return parentCommentContent;
    }

    public void updateCurrentParentCommentContent(String content) {
        this.parentCommentContent = content;
    }

    public void clearContent() {
        edtParentComment.setText("");
        updateListParentAttach(new ArrayList<>());
    }

    @Override
    public void OnLikeClick(int pos) {
        Intent intent = new Intent();

        if (isReplyView) {
            intent.setAction("REPLYCOMMENT");
        } else {
            intent.setAction("COMMENT");
        }
        intent.putExtra("type", "like");
        // Bấm từ DetailWorkflowActivity thì xài realm
        if (mainAct instanceof DetailWorkflowActivity) {
            intent.putExtra("obj", new Gson().toJson(new RealmController().getRealm().copyFromRealm(lstComment.get(pos))));
        } else if (mainAct instanceof DetailCreateTaskActivity) {
            // Không xài realm, chỉ xài list online
            intent.putExtra("obj", new Gson().toJson(lstComment.get(pos)));
        }

        BroadcastUtility.send(mainAct, intent);
    }

    @Override
    public void OnReplyClick(int pos) {
        Intent intent = new Intent();

        if (isReplyView) {
            intent.setAction(VarsReceiver.REPLYCOMMENT);
        } else {
            intent.setAction(VarsReceiver.COMMENT);
        }

        intent.putExtra("type", "reply");

        // Bấm từ DetailWorkflowActivity thì xài realm
        if (mainAct instanceof DetailWorkflowActivity) {
            intent.putExtra("obj", new Gson().toJson(new RealmController().getRealm().copyFromRealm(lstComment.get(pos))));
        } else if (mainAct instanceof DetailCreateTaskActivity) {
            // Không xài realm, chỉ xài list online
            intent.putExtra("obj", new Gson().toJson(lstComment.get(pos)));
        }

        BroadcastUtility.send(mainAct, intent);
    }
}

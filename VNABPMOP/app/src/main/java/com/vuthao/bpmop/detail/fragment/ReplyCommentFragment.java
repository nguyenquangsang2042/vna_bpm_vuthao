package com.vuthao.bpmop.detail.fragment;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;

import androidx.activity.result.ActivityResult;
import androidx.activity.result.ActivityResultCallback;
import androidx.activity.result.ActivityResultLauncher;
import androidx.activity.result.contract.ActivityResultContracts;
import androidx.recyclerview.widget.RecyclerView;
import androidx.recyclerview.widget.StaggeredGridLayoutManager;

import android.text.Editable;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.WindowManager;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.AnimationController;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.activity.BaseFragment;
import com.vuthao.bpmop.base.custom.CustomFlexBoxRecyclerView;
import com.vuthao.bpmop.base.custom.CustomResizeLinearLayout;
import com.vuthao.bpmop.base.keyboard.KeyboardManager;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.model.custom.AttachFile;
import com.vuthao.bpmop.base.model.custom.Comment;
import com.vuthao.bpmop.base.model.custom.DownloadedFiles;
import com.vuthao.bpmop.base.realm.RealmController;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.core.Vars;
import com.vuthao.bpmop.core.adapter.ParentAttachFile_ImageAdapter;
import com.vuthao.bpmop.core.component.ComponentComment;
import com.vuthao.bpmop.core.adapter.ParentAttachFileAdapter;
import com.vuthao.bpmop.detail.custom.DetailFunc;
import com.vuthao.bpmop.detail.presenter.ReplyCommentPresenter;
import com.vuthao.bpmop.shareview.SharedView_PopupChooseFile;

import java.io.File;
import java.util.ArrayList;
import java.util.stream.Collectors;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;

import static android.app.Activity.RESULT_OK;

public class ReplyCommentFragment extends BaseFragment implements ReplyCommentPresenter.ReplyCommentListener, TextWatcher, CustomResizeLinearLayout.KeyboardStateChange {
    @BindView(R.id.ln_ViewReplyComment_All)
    LinearLayout lnAll;
    @BindView(R.id.ln_ViewReplyComment_Content)
    CustomResizeLinearLayout lnContentCustomResize;
    @BindView(R.id.ln_ViewDetailCreateTask_KeyBoard)
    LinearLayout lnKeyBoard;
    @BindView(R.id.ln_ViewReplyComment_ComponentComment)
    LinearLayout lnComponentComment;
    @BindView(R.id.tv_ViewReplyComment_Title)
    TextView tvTitle;
    @BindView(R.id.img_ViewReplyComment_Back)
    ImageView imgBack;
    @BindView(R.id.img_ViewReplyComment_Attach)
    ImageView imgAttach;
    @BindView(R.id.img_ViewReplyComment_Comment)
    ImageView imgComment;
    @BindView(R.id.edt_ViewReplyComment_Comment)
    EditText edtComment;
    @BindView(R.id.recy_ViewReplyComment_AttachFile)
    CustomFlexBoxRecyclerView recyAttach;
    @BindView(R.id.recy_ViewReplyComment_AttachFile_Image)
    RecyclerView recyAttach_Image;

    private View rootView;
    private LayoutInflater inflater;
    private Comment comment;
    private ArrayList<Comment> comments;
    private ArrayList<AttachFile> attachfiles;
    private String OtherResourceId;
    private String ResourceCategoryId;
    private boolean isDeleteFullName = false;
    private String currentFullName = "";
    private ComponentComment componentComment;
    private ParentAttachFileAdapter adapterCommentAttach;
    private ParentAttachFile_ImageAdapter adapterCommentAttach_Image;
    public Uri fileImage;
    private ReplyCommentPresenter presenter;
    private Activity activity;

    public ReplyCommentFragment(Activity activity, Comment comment, ArrayList<Comment> comments, String _OtherResourceId, String _ResourceCategoryId) {
        this.activity = activity;
        this.comment = comment;
        this.comments = comments;
        this.OtherResourceId = _OtherResourceId;
        this.ResourceCategoryId = _ResourceCategoryId;
    }

    public ReplyCommentFragment() {
        // Required empty public constructor
    }


    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        requireActivity().getWindow().setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_ADJUST_RESIZE);
        this.inflater = inflater;
        if (rootView == null) {
            rootView = inflater.inflate(R.layout.fragment_reply_comment, container, false);
            ButterKnife.bind(this, rootView);

            init();
            setData();

            edtComment.addTextChangedListener(this);
        }

        return rootView;
    }

    @SuppressLint("SetTextI18n")
    public void init() {
        presenter = new ReplyCommentPresenter(this);
        attachfiles = new ArrayList<>();
        recyAttach.setMaxRowAndRowHeight(Functions.share.convertDpToPixel(40, rootView.getContext()), 3);
        tvTitle.setText(Functions.share.getTitle("TEXT_COMMENT", "Bình luận"));
        lnContentCustomResize.setKeyboardStateListener(this);
        KeyboardManager.showKeyBoard(edtComment, requireActivity());
        edtComment.requestFocus();

        User user = new RealmController().getRealm().where(User.class)
                .equalTo("ID", comment.getAuthor()).findFirst();
        if (user != null) {
            edtComment.setText(" " + user.getFullName() + "  ");
            currentFullName = " " + user.getFullName() + " ";
            DetailFunc.share.hightLightTextSpecific(edtComment, edtComment.getText().toString(), user.getFullName(), "#e5eeff");
            edtComment.setSelection(edtComment.length());
        }

        BroadcastUtility.register(requireActivity(), mReceiver, VarsReceiver.REPLYCOMMENT);
        BroadcastUtility.register(requireActivity(), mReceiver, VarsReceiver.SELECT_FILE_INAPP);
    }

    private void setData() {
        // Reply cho comment parent -> search theo chính nó
        if (Functions.isNullOrEmpty(comment.getParentCommentId())) {
            comments = (ArrayList<Comment>) comments.stream().filter(r -> r.getID().equals(comment.getID()) || !Functions.isNullOrEmpty(r.getParentCommentId()) && r.getParentCommentId().equals(comment.getID())).collect(Collectors.toList());
        } else {
            // Reply cho comment child -> search theo parent
            comments = (ArrayList<Comment>) comments.stream().filter(r -> r.getID().equals(comment.getParentCommentId()) || !Functions.isNullOrEmpty(r.getParentCommentId()) && r.getParentCommentId().equals(comment.getParentCommentId())).collect(Collectors.toList());
            // Gán root lại cho Parent vì chỉ có thể reply cho Root (root thì parent = null)
            for (Comment item : comments) {
                if (Functions.isNullOrEmpty(comment.getParentCommentId())) {
                    comment = item;
                    break;
                }
            }
        }

        lnComponentComment.removeAllViews();
        componentComment = new ComponentComment(requireActivity(), rootView.getContext(), lnComponentComment, comments, true);
        componentComment.initializeFrameView(lnComponentComment);
        componentComment.setTitle();
        componentComment.setValue();
        componentComment.setEnable();
        componentComment.setProprety();

        if (attachfiles != null && attachfiles.size() > 0) {
            attachfiles = DetailFunc.share.classifyListAttachFile(attachfiles);
        }

        adapterCommentAttach = new ParentAttachFileAdapter(requireActivity(), rootView.getContext(), attachfiles, true, true);
        recyAttach.setAdapter(adapterCommentAttach);
        recyAttach.setLayoutManager(new StaggeredGridLayoutManager(1, StaggeredGridLayoutManager.VERTICAL));

        adapterCommentAttach_Image = new ParentAttachFile_ImageAdapter(requireActivity(), rootView.getContext(), attachfiles, true, true);
        recyAttach_Image.setAdapter(adapterCommentAttach_Image);
        recyAttach_Image.setLayoutManager(new StaggeredGridLayoutManager(1, StaggeredGridLayoutManager.HORIZONTAL));
    }

    @OnClick({R.id.ln_ViewReplyComment_All, R.id.img_ViewReplyComment_Back, R.id.img_ViewReplyComment_Attach,
            R.id.img_ViewReplyComment_Comment})
    public void OnViewClicked(View v) {
        switch (v.getId()) {
            case R.id.ln_ViewReplyComment_All: {
                break;
            }
            case R.id.img_ViewReplyComment_Back: {
                Intent intent = new Intent();
                intent.setAction(VarsReceiver.REFRESHCOMMENT);
                intent.putExtra("type", "like");
                BroadcastUtility.send(requireActivity(), intent);
                sBaseActivity.backFragment("");
                break;
            }
            case R.id.img_ViewReplyComment_Attach: {
                KeyboardManager.hideKeyboard(edtComment, requireActivity());
                SharedView_PopupChooseFile file = new SharedView_PopupChooseFile(inflater, getActivity(), this, mLauncherNormal, mLauncherCamera, "ReplyComment", rootView,
                        Constants.mReplyCommentAttachFile, Constants.mReplyCommentAttachFileCamera, Vars.FlagView.ReplyComment);
                file.initializeView();
                break;
            }
            case R.id.img_ViewReplyComment_Comment: {
                presenter.sendComment(edtComment.getText().toString(), attachfiles, OtherResourceId, ResourceCategoryId, comment.getID());
                break;
            }
        }
    }

    private BroadcastReceiver mReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            switch (intent.getAction()) {
                case VarsReceiver.REPLYCOMMENT: {
                    String type = intent.getStringExtra("type");
                    switch (type) {
                        case "like": {
                            Comment comment = new Gson().fromJson(intent.getStringExtra("obj"), Comment.class);
                            presenter.setLikeComment(activity, comments, comment);
                            presenter.addLikeComment(comment, comment.getID(), comment.isLiked() != 1, 32);
                            break;
                        }
                        case "reply": {
                            Comment comment = new Gson().fromJson(intent.getStringExtra("obj"), Comment.class);
                            replyComment(comment);
                            break;
                        }
                        case "delete": {
                            AttachFile file = new Gson().fromJson(intent.getStringExtra("obj"), AttachFile.class);
                            attachfiles = (ArrayList<AttachFile>) attachfiles.stream().filter(r -> !r.getPath().equals(file.getPath())).collect(Collectors.toList());

                            if (adapterCommentAttach != null) {
                                adapterCommentAttach.updateListAttach(attachfiles);
                                adapterCommentAttach.notifyDataSetChanged();
                            }

                            if (adapterCommentAttach_Image != null) {
                                adapterCommentAttach_Image.updateListAttach(attachfiles);
                                adapterCommentAttach_Image.notifyDataSetChanged();
                            }

                            break;
                        }
                    }
                    break;
                }
                case VarsReceiver.SELECT_FILE_INAPP: {
                    selectFileInApp(intent);
                    break;
                }
            }
        }
    };

    private void selectFileInApp(Intent intent) {
        DownloadedFiles file = new Gson().fromJson(intent.getStringExtra("file"), DownloadedFiles.class);
        AttachFile attachFile = DetailFunc.share.getAttachFileFromURI(requireActivity(), Uri.fromFile(new File(file.getPath())));

        if (DetailFunc.share.checkFileExits(attachfiles, attachFile)) {
            Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("TEXT_EXISTINGFILE", "File đã tồn tại trong danh sách"), requireActivity());
            return;
        }

        attachfiles.add(attachFile);
        attachfiles = DetailFunc.share.classifyListAttachFile(attachfiles);

        adapterCommentAttach.updateListAttach(attachfiles);
        adapterCommentAttach.notifyDataSetChanged();

        adapterCommentAttach_Image.updateListAttach(attachfiles);
        adapterCommentAttach_Image.notifyDataSetChanged();
    }

    private void replyComment(Comment comment) {
        User user = new RealmController().getRealm().where(User.class)
                .equalTo("ID", comment.getAuthor()).findFirst();
        if (user != null) {
            edtComment.setText(" " + user.getFullName() + "  ");
            currentFullName = " " + user.getFullName() + " ";
            DetailFunc.share.hightLightTextSpecific(edtComment, edtComment.getText().toString(), user.getFullName(), "#e5eeff");
        }

        edtComment.requestFocus();
        edtComment.setSelection(edtComment.length());

        // file
        attachfiles = new ArrayList<>();

        if (adapterCommentAttach != null) {
            adapterCommentAttach.updateListAttach(attachfiles);
            adapterCommentAttach.notifyDataSetChanged();
        }
        if (adapterCommentAttach_Image != null) {
            adapterCommentAttach_Image.updateListAttach(attachfiles);
            adapterCommentAttach_Image.notifyDataSetChanged();
        }
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        BroadcastUtility.unregister(requireActivity(), mReceiver);
    }

    @Override
    public void OnKeyboardShow() {
        lnKeyBoard.startAnimation(AnimationController.share.fadeIn(rootView.getContext()));
    }

    @Override
    public void OnKeyboardHide() {
        lnKeyBoard.startAnimation(AnimationController.share.fadeIn(rootView.getContext()));
    }

    @Override
    public void beforeTextChanged(CharSequence s, int start, int count, int after) {

    }

    @Override
    public void onTextChanged(CharSequence s, int start, int before, int count) {
        if (s.toString().equals(currentFullName)) {
            isDeleteFullName = true;
        } else {
            isDeleteFullName = false;
        }
    }

    @Override
    public void afterTextChanged(Editable s) {
        if (isDeleteFullName) {
            edtComment.setText("");
        }
    }

    ActivityResultLauncher<Intent> mLauncherNormal = registerForActivityResult(
            new ActivityResultContracts.StartActivityForResult(),
            new ActivityResultCallback<ActivityResult>() {
                @Override
                public void onActivityResult(ActivityResult result) {
                    // Do your code from onActivityResult
                    if (result.getResultCode() == RESULT_OK) {
                        AttachFile attachFile = DetailFunc.share.getAttachFileFromURI(requireActivity(), result.getData().getData());

                        if (Functions.isNullOrEmpty(attachFile.getPath())) {
                            Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Vui lòng thử lại!"),
                                    requireActivity());
                            return;
                        }

                        if (DetailFunc.share.checkFileExits(attachfiles, attachFile)) {
                            Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("TEXT_EXISTINGFILE", "File đã tồn tại trong danh sách"), getActivity());
                            return;
                        }

                        attachfiles.add(attachFile);
                        attachfiles = DetailFunc.share.classifyListAttachFile(attachfiles);

                        if (adapterCommentAttach != null) {
                            adapterCommentAttach.updateListAttach(attachfiles);
                            adapterCommentAttach.notifyDataSetChanged();
                        }

                        if (adapterCommentAttach_Image != null) {
                            adapterCommentAttach_Image.updateListAttach(attachfiles);
                            adapterCommentAttach_Image.notifyDataSetChanged();
                        }
                    }
                }
            });

    ActivityResultLauncher<Intent> mLauncherCamera = registerForActivityResult(
            new ActivityResultContracts.StartActivityForResult(),
            new ActivityResultCallback<ActivityResult>() {
                @Override
                public void onActivityResult(ActivityResult result) {
                    // Do your code from onActivityResult
                    if (result.getResultCode() == RESULT_OK) {
                        AttachFile attachFile;

                        if (fileImage != null) {
                            attachFile = DetailFunc.share.getAttachFileFromURICamera(requireActivity(), fileImage);
                        } else {
                            Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Vui lòng thử lại!"),
                                    requireActivity());
                            return;
                        }

                        if (DetailFunc.share.checkFileExits(attachfiles, attachFile)) {
                            Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("TEXT_EXISTINGFILE", "File đã tồn tại trong danh sách"), getActivity());
                            return;
                        }

                        attachfiles.add(attachFile);
                        attachfiles = DetailFunc.share.classifyListAttachFile(attachfiles);

                        if (adapterCommentAttach != null) {
                            adapterCommentAttach.updateListAttach(attachfiles);
                            adapterCommentAttach.notifyDataSetChanged();
                        }

                        if (adapterCommentAttach_Image != null) {
                            adapterCommentAttach_Image.updateListAttach(attachfiles);
                            adapterCommentAttach_Image.notifyDataSetChanged();
                        }
                    }
                }
            });

    @Override
    public void OnSendCommentSuccess() {
        // refresh lại detail
        Intent intent = new Intent();
        intent.setAction(VarsReceiver.REFRESHCOMMENT);
        intent.putExtra("type", "comment");
        BroadcastUtility.send(requireActivity(), intent);
        sBaseActivity.backFragment("");
    }

    @Override
    public void OnLikeCommentSuccess() {
        componentComment.notifyDataCommentChange();

        Intent intent = new Intent();
        intent.setAction(VarsReceiver.REFRESHCOMMENT);
        intent.putExtra("type", "like");
        BroadcastUtility.send(requireActivity(), intent);
    }

    @Override
    public void OnSendCommentErr(String err) {
        Utility.share.showAlertWithOnlyOK(err, Functions.share.getTitle("TEXT_CLOSE", "Đóng"), requireActivity());

    }

    @Override
    public void OnLikeCommentErr(String err) {
        Utility.share.showAlertWithOnlyOK(err, Functions.share.getTitle("TEXT_CLOSE", "Đóng"), requireActivity());
    }
}
package com.vuthao.bpmop.shareview;

import android.app.Activity;
import android.app.Dialog;
import android.content.Intent;
import android.content.res.ColorStateList;
import android.graphics.Color;
import android.graphics.Typeface;
import android.graphics.drawable.ColorDrawable;
import android.media.Image;
import android.text.Editable;
import android.text.TextWatcher;
import android.util.DisplayMetrics;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.core.content.res.ResourcesCompat;
import androidx.localbroadcastmanager.content.LocalBroadcastManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.recyclerview.widget.StaggeredGridLayoutManager;

import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.ImageLoader;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.vars.Variable;
import com.vuthao.bpmop.base.keyboard.KeyboardManager;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.model.app.WorkflowItem;
import com.vuthao.bpmop.base.model.custom.ButtonAction;
import com.vuthao.bpmop.base.realm.RealmController;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.detail.custom.DetailFunc;
import com.vuthao.bpmop.shareview.adapter.DetailChooseUserAdapter;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.stream.Collectors;

import de.hdodenhof.circleimageview.CircleImageView;
import io.realm.Realm;
import io.realm.RealmResults;
import io.realm.Sort;

public class SharedView_PopupActionForward extends SharedView_PopupActionBase implements DetailChooseUserAdapter.DetailChooseUserListener, TextWatcher, View.OnClickListener {
    private float popupScaleWidth_Full = 1;
    private final WorkflowItem workflowItem;
    private User selectedUser = new User();
    private final Realm realm;
    private final ArrayList<User> users = new ArrayList<>();
    private ArrayList<User> filters = new ArrayList<>();
    private ArrayList<User> searchs = new ArrayList<>();
    private DetailChooseUserAdapter adapter;
    private final String arr;
    private Dialog dialog;

    private View view;
    private LinearLayout lnChooseUser;
    private ImageView imgCloseChooseUser;
    private ImageView imgAcceptChooseUser;
    private TextView tvTitleChooseUser;
    private EditText edtSearchChooseUser;
    private RecyclerView recyChooseUser;
    private LinearLayout lnAction;
    private LinearLayout lnActionSelectedUser;
    private ImageView imgAction;
    private TextView tvTitleAction;
    private EditText edtCommentAction;
    private TextView tvCloseAction;
    private TextView tvDoneAction;
    private LinearLayout lnChooseUserAction;
    private CircleImageView imgAvatarAction;
    private TextView tvAvatarAction;
    private TextView tvUserNameAction;
    private TextView tvEmailAction;
    private TextView tvPleaseChooseUserAction;

    public SharedView_PopupActionForward(LayoutInflater inflater, Activity mainAct, String fragmentTag, View rootView, WorkflowItem workflowItem, float popupScaleWidth_Full) {
        super(inflater, mainAct, fragmentTag, rootView);
        this.popupScaleWidth_Full = popupScaleWidth_Full;
        this.workflowItem = workflowItem;
        realm = new RealmController().getRealm();
        arr = workflowItem.getAssignedTo();
    }

    @Override
    public void initializeValue_DetailWorkflow(ButtonAction buttonAction) {
        super.initializeValue_DetailWorkflow(buttonAction);
    }

    @Override
    public void initializeView() {
        super.initializeView();

        view = inflater.inflate(R.layout.popup_action_choose_user_and_action, null);
        lnChooseUser = view.findViewById(R.id.ln_PopupAction_ChooseUserAndAction_ChooseUser);
        imgCloseChooseUser = view.findViewById(R.id.img_PopupAction_ChooseUserAndAction_ChooseUser_Close);
        imgAcceptChooseUser = view.findViewById(R.id.img_PopupAction_ChooseUserAndAction_ChooseUser_Accept);
        tvTitleChooseUser = view.findViewById(R.id.tv_PopupAction_ChooseUserAndAction_ChooseUser_Title);
        edtSearchChooseUser = view.findViewById(R.id.edt_PopupAction_ChooseUserAndAction_ChooseUser_Search);
        recyChooseUser = view.findViewById(R.id.recy_PopupAction_ChooseUserAndAction_ChooseUser);
        lnAction = view.findViewById(R.id.ln_PopupAction_ChooseUserAndAction_Action);
        lnActionSelectedUser = view.findViewById(R.id.ln_PopupAction_ChooseUserAndAction_Action_SelectedUser);
        imgAction = view.findViewById(R.id.img_PopupAction_ChooseUserAndAction_Action);
        tvTitleAction = view.findViewById(R.id.tv_PopupAction_ChooseUserAndAction_Action_Title);
        edtCommentAction = view.findViewById(R.id.edt_PopupAction_ChooseUserAndAction_Action_Comment);
        tvCloseAction = view.findViewById(R.id.tv_PopupAction_ChooseUserAndAction_Action_Close);
        tvDoneAction = view.findViewById(R.id.tv_PopupAction_ChooseUserAndAction_Action_Done);
        lnChooseUserAction = view.findViewById(R.id.ln_PopupAction_ChooseUserAndAction_Action_User);
        imgAvatarAction = view.findViewById(R.id.img_PopupAction_ChooseUserAndAction_Action_Avatar);
        tvAvatarAction = view.findViewById(R.id.tv_PopupAction_ChooseUserAndAction_Action_Avatar);
        tvUserNameAction = view.findViewById(R.id.tv_PopupAction_ChooseUserAndAction_Action_UserName);
        tvEmailAction = view.findViewById(R.id.tv_PopupAction_ChooseUserAndAction_Action_Email);
        tvPleaseChooseUserAction = view.findViewById(R.id.tv_PopupAction_ChooseUserAndAction_Action_PleaseChooseUser);

        lnActionSelectedUser.setVisibility(View.GONE);
        lnAction.setVisibility(View.VISIBLE);
        lnChooseUser.setVisibility(View.GONE);

        tvTitleAction.setText(buttonAction.getTitle());
        tvPleaseChooseUserAction.setText(Functions.share.getTitle("TEXT_CONTROL_CHOOSE_USERS", "Tìm người"));
        edtCommentAction.setHint(Functions.share.getTitle("MESS_REQUIRE_COMMENT", "Vui lòng nhập ý kiến"));
        tvCloseAction.setText(Functions.share.getTitle("TEXT_EXIT", "Thoát"));
        tvDoneAction.setText(buttonAction.getTitle());

        tvTitleChooseUser.setText(Functions.share.getTitle("MESS_REQUIRE_USER", "Vui lòng chọn người thực hiện"));
        tvPleaseChooseUserAction.setText(Functions.share.getTitle("TEXT_CONTROL_CHOOSE_USERS", "Tìm người"));
        edtSearchChooseUser.setHint(Functions.share.getTitle("TEXT_HINT_USER_EMAIL", "Vui lòng nhập tên hoặc địa chỉ email..."));

        String imageName = "icon_bpm_Btn_action_" + buttonAction.getID();
        int resId = mainAct.getResources().getIdentifier(imageName.toLowerCase(), "drawable", mainAct.getPackageName());
        imgAction.setImageResource(resId);

        //region Dialog
        dialog = new Dialog(mainAct);
        Window window = dialog.getWindow();
        dialog.requestWindowFeature(1);
        dialog.setCancelable(true);
        dialog.setCanceledOnTouchOutside(false);
        window.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_ADJUST_PAN);
        window.setGravity(Gravity.CENTER);

        DisplayMetrics displayMetrics = mainAct.getResources().getDisplayMetrics();
        dialog.setContentView(view);
        dialog.show();

        WindowManager.LayoutParams s = window.getAttributes();
        s.width = (int) (displayMetrics.widthPixels * popupScaleWidth_Full);
        s.height = WindowManager.LayoutParams.WRAP_CONTENT;
        window.setAttributes(s);
        window.setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));
        //endregion

        RealmResults<User> results = realm.where(User.class)
                .not()
                .equalTo("Email", CurrentUser.getInstance().getUser().getEmail())
                .sort("FullName", Sort.ASCENDING)
                .findAll();
        users.addAll(results);
        filters.addAll(users);

        adapter = new DetailChooseUserAdapter(mainAct, mainAct.getApplicationContext(), users, selectedUser, this);
        StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, StaggeredGridLayoutManager.VERTICAL);
        recyChooseUser.setAdapter(adapter);
        recyChooseUser.setLayoutManager(staggeredGridLayoutManager);

        edtSearchChooseUser.addTextChangedListener(this);
        imgCloseChooseUser.setOnClickListener(this);
        imgAcceptChooseUser.setOnClickListener(this);
        edtCommentAction.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {

            }

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {

            }

            @Override
            public void afterTextChanged(Editable s) {
                if (s.length() > 0) {
                    edtCommentAction.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.NORMAL);
                } else {
                    edtCommentAction.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.ITALIC);
                }
            }
        });

        lnChooseUserAction.setOnClickListener(this);
        tvCloseAction.setOnClickListener(this);
        tvDoneAction.setOnClickListener(this);

        edtSearchChooseUser.setText("");
        edtCommentAction.setText("");
    }

    private void select() {
        lnAction.setVisibility(View.GONE);
        lnChooseUser.setVisibility(View.VISIBLE);

        Window window13 = dialog.getWindow();
        WindowManager.LayoutParams s13 = window13.getAttributes();
        DisplayMetrics displayMetrics13 = mainAct.getResources().getDisplayMetrics();
        s13.width = displayMetrics13.widthPixels;
        s13.height = displayMetrics13.heightPixels;
        window13.setAttributes(s13);
    }

    private void done() {
        KeyboardManager.hideKeyboard(edtCommentAction, mainAct);
        if (DetailFunc.share.checkActionHasSelectedUser(selectedUser)) {
            if (!Functions.isNullOrEmpty(edtCommentAction.getText().toString())) {
                Intent intent = new Intent();
                intent.setAction(VarsReceiver.SUBMITACTION);
                intent.putExtra("buttonAction", new Gson().toJson(buttonAction));
                intent.putExtra("comment", edtCommentAction.getText().toString());

                HashMap<String, Object> hashMap = new HashMap<>();
                hashMap.put("userValues", selectedUser.getAccountName());

                intent.putExtra("extension", hashMap);
                BroadcastUtility.send(mainAct, intent);

                dialog.dismiss();
            } else {
                Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("MESS_REQUIRE_COMMENT", "Vui lòng nhập ý kiến."),
                        Functions.share.getTitle("TEXT_CLOSE", "Đóng"), mainAct);
            }
        } else {
            Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("K_Action_PleaseChooseUser", "Vui lòng chọn người để thực hiện."),
                    Functions.share.getTitle("TEXT_CLOSE", "Đóng"), mainAct);
        }
    }

    private void close() {
        KeyboardManager.hideKeyboard(edtSearchChooseUser, mainAct);
        lnChooseUser.setVisibility(View.GONE);
        lnAction.setVisibility(View.VISIBLE);

        Window window12 = dialog.getWindow();
        WindowManager.LayoutParams s12 = window12.getAttributes();
        DisplayMetrics displayMetrics12 = mainAct.getResources().getDisplayMetrics();
        s12.width = (int) (displayMetrics12.widthPixels * popupScaleWidth_Full);
        s12.height = WindowManager.LayoutParams.WRAP_CONTENT;
        window12.setAttributes(s12);
    }

    @Override
    public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {

    }

    @Override
    public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {

    }

    @Override
    public void afterTextChanged(Editable s) {
        if (s.length() > 0) {
            // Tham vấn check ko hiện mấy thằng trong bước đó, Foward thì bình thường
            if (buttonAction.getID() == Variable.WorkflowAction.RequestIdea && !Functions.isNullOrEmpty(workflowItem.getAssignedTo())) {
                searchs = (ArrayList<User>) filters.stream()
                        .filter(r -> r.getFullName().contains(s.toString())
                                || r.getEmail().contains(s.toString())
                                && !r.getID().contains(arr))
                        .collect(Collectors.toList());
            } else {
                searchs = (ArrayList<User>) filters.stream()
                        .filter(r -> r.getFullName().contains(s.toString()) || r.getEmail().contains(s.toString()))
                        .collect(Collectors.toList());
            }
        } else {
            // Tham vấn check ko hiện mấy thằng trong bước đó, Foward thì bình thường
            if (buttonAction.getID() == Variable.WorkflowAction.RequestIdea && !Functions.isNullOrEmpty(workflowItem.getAssignedTo())) {
                searchs = (ArrayList<User>) filters.stream()
                        .filter(r -> !r.getID().contains(arr))
                        .collect(Collectors.toList());
            } else {
                searchs = new ArrayList<>(filters);
            }
        }

        adapter.updateListFilter(searchs);
    }

    @Override
    public void onClick(View view) {
        switch (view.getId()) {
            case R.id.img_PopupAction_ChooseUserAndAction_ChooseUser_Close: {
                close();
                break;
            }
            case R.id.ln_PopupAction_ChooseUserAndAction_Action_User: {
                select();
                break;
            }
            case R.id.tv_PopupAction_ChooseUserAndAction_Action_Close: {
                KeyboardManager.hideKeyboard(edtSearchChooseUser, mainAct);
                dialog.dismiss();
                break;
            }
            case R.id.tv_PopupAction_ChooseUserAndAction_Action_Done: {
                done();
                break;
            }
        }
    }

    @Override
    public void OnClick(User user) {
        tvPleaseChooseUserAction.setVisibility(View.GONE);
        lnActionSelectedUser.setVisibility(View.VISIBLE);
        selectedUser = user;

        if (!Functions.isNullOrEmpty(selectedUser.getID())) {
            lnChooseUser.setVisibility(View.GONE);
            lnAction.setVisibility(View.VISIBLE);

            Window window1 = dialog.getWindow();
            WindowManager.LayoutParams s1 = window1.getAttributes();
            DisplayMetrics displayMetrics1 = mainAct.getResources().getDisplayMetrics();
            s1.width = (int) (displayMetrics1.widthPixels * popupScaleWidth_Full);
            s1.height = WindowManager.LayoutParams.WRAP_CONTENT;
            window1.setAttributes(s1);

            if (!Functions.isNullOrEmpty(selectedUser.getImagePath())) {
                imgAvatarAction.setVisibility(View.VISIBLE);
                tvAvatarAction.setVisibility(View.GONE);

                ImageLoader.getInstance().loadImageUserWithToken(mainAct, Constants.BASE_URL + selectedUser.getImagePath(), imgAvatarAction);
            } else {
                imgAvatarAction.setVisibility(View.GONE);
                tvAvatarAction.setVisibility(View.VISIBLE);

                tvAvatarAction.setText(Functions.share.getAvatarName(selectedUser.getAccountName()));
                tvAvatarAction.setBackgroundTintList(ColorStateList.valueOf(Functions.share.getColorByUsername(selectedUser.getAccountName())));
            }

            if (!Functions.isNullOrEmpty(selectedUser.getFullName())) {
                tvUserNameAction.setText(selectedUser.getFullName());
            } else {
                tvUserNameAction.setText("");
            }

            if (!Functions.isNullOrEmpty(selectedUser.getEmail())) {
                tvEmailAction.setText(selectedUser.getEmail());
            } else {
                tvEmailAction.setText("");
            }
        } else {
            Utility.share.showAlertWithOnlyOK(Functions.share.getTitle("K_Action_PleaseChooseUser", "Vui lòng chọn người để thực hiện."),
                    Functions.share.getTitle("TEXT_CLOSE", "Đóng"), mainAct);
        }
    }
}

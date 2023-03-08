package com.vuthao.bpmop.shareview;

import android.app.Activity;
import android.app.Dialog;
import android.content.Intent;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.localbroadcastmanager.content.LocalBroadcastManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.recyclerview.widget.StaggeredGridLayoutManager;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.custom.CustomFlexBoxRecyclerView;
import com.vuthao.bpmop.base.custom.flexbox.FlexDirection;
import com.vuthao.bpmop.base.custom.flexbox.FlexboxLayoutManager;
import com.vuthao.bpmop.base.custom.flexbox.JustifyContent;
import com.vuthao.bpmop.base.keyboard.KeyboardManager;
import com.vuthao.bpmop.base.model.app.Group;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.model.custom.UserAndGroup;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.realm.RealmController;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.core.Vars;
import com.vuthao.bpmop.shareview.adapter.SelectUserGroupMultiple_TextAdapter;
import com.vuthao.bpmop.shareview.adapter.SelectUserSingleAdapter;

import org.json.JSONObject;

import java.util.ArrayList;
import java.util.stream.Collectors;

import io.realm.Realm;
import io.realm.RealmResults;

public class SharedView_PopupControlSelectUserGroup extends SharedView_PopupControlBase implements View.OnClickListener, SelectUserGroupMultiple_TextAdapter.SelectUserGroupMultiple_TextListener, SelectUserSingleAdapter.SelectUserSingleListener {
    private final boolean isUserAndGroup;
    private final Realm realm;
    private UserAndGroup selectedUserAndGroup = new UserAndGroup();
    private final ArrayList<UserAndGroup> lstSelected = new ArrayList<>();
    private Dialog dialog;

    private View view;
    private LinearLayout lnSearch;
    private ImageView imgCloseChooseUser;
    private ImageView imgAcceptChooseUser;
    private ImageView imgDeleteChooseUser;
    private TextView tvTitleChooseUser;
    private EditText edtSearchChooseUser;
    private RecyclerView recyChooseUser;
    private CustomFlexBoxRecyclerView recySelectedUser;

    public SharedView_PopupControlSelectUserGroup(LayoutInflater inflater, Activity mainAct, String fragmentTag, View rootView, boolean isUserAndGroup) {
        super(inflater, mainAct, fragmentTag, rootView);
        this.isUserAndGroup = isUserAndGroup;
        realm = new RealmController().getRealm();
    }

    @Override
    public void initializeValue_Master(ViewElement viewElement) {
        super.initializeValue_Master(viewElement);
    }

    @Override
    public void initializeValue_InputGridDetail(ViewElement elementParent, ViewElement elementPopup, JSONObject JObjectChild) {
        super.initializeValue_InputGridDetail(elementParent, elementPopup, JObjectChild);
    }

    @Override
    public void initializeView() {
        super.initializeView();

        view = inflater.inflate(R.layout.popup_control_choose_user, null);
        lnSearch = view.findViewById(R.id.ln_PopupControl_ChooseUser_Search);
        imgCloseChooseUser = view.findViewById(R.id.img_PopupControl_ChooseUser_Close);
        imgAcceptChooseUser = view.findViewById(R.id.img_PopupControl_ChooseUser_Accept);
        imgDeleteChooseUser = view.findViewById(R.id.img_PopupControl_ChooseUser_Delete);
        tvTitleChooseUser = view.findViewById(R.id.tv_PopupControl_ChooseUser_Title);
        edtSearchChooseUser = view.findViewById(R.id.edt_PopupControl_ChooseUser_Search);
        recyChooseUser = view.findViewById(R.id.recy_PopupControl_ChooseUser);
        recySelectedUser = view.findViewById(R.id.recy_PopupControl_SelectedUser);

        // chọn single người thì luôn ẩn vì chọn từ adapter
        imgAcceptChooseUser.setVisibility(View.INVISIBLE);
        imgAcceptChooseUser.getLayoutParams().width = 0;

        recySelectedUser.setVisibility(View.VISIBLE);
        recySelectedUser.setMaxRowAndRowHeight(Functions.share.convertDpToPixel(35, mainAct), 1);

        if (isUserAndGroup) {
            edtSearchChooseUser.setHint(Functions.share.getTitle("MESS_REQUIRE_USERGROUP", "Vui lòng chọn người hoặc nhóm để thực hiện"));
        } else {
            edtSearchChooseUser.setHint(Functions.share.getTitle("MESS_REQUIRE_USER", "Vui lòng chọn người thực hiện"));
        }

        boolean isEnable;
        switch (flagView) {
            case Vars.FlagViewControlDynamic.DetailWorkflow:
            default: {
                tvTitleChooseUser.setText(elementParent.getTitle());
                isEnable = elementParent.isEnable();
                if (!Functions.isNullOrEmpty(elementParent.getValue())) {
                    ArrayList<UserAndGroup> beanUserAndGroup = new Gson().fromJson(elementParent.getValue(), new TypeToken<ArrayList<UserAndGroup>>() {
                    }.getType());

                    if (beanUserAndGroup != null && beanUserAndGroup.size() > 0) {
                        selectedUserAndGroup = beanUserAndGroup.get(0);
                    }

                    if (elementParent.isEnable()) {
                        lnSearch.setVisibility(View.VISIBLE);
                        imgDeleteChooseUser.setVisibility(View.VISIBLE);
                        KeyboardManager.showKeyBoard(mainAct);
                    } else {
                        lnSearch.setVisibility(View.GONE);
                        imgDeleteChooseUser.setVisibility(View.GONE);
                    }
                }
                break;
            }
            case Vars.FlagViewControlDynamic.DetailWorkflow_InputGridDetail: {
                tvTitleChooseUser.setText(elementPopup.getTitle());
                isEnable = elementPopup.isEnable();
                if (!Functions.isNullOrEmpty(elementPopup.getValue())) {
                    ArrayList<UserAndGroup> beanUserAndGroup = new Gson().fromJson(elementPopup.getValue(), new TypeToken<ArrayList<UserAndGroup>>() {
                    }.getType());

                    if (beanUserAndGroup != null && beanUserAndGroup.size() > 0) {
                        selectedUserAndGroup = beanUserAndGroup.get(0);
                    }

                    if (elementPopup.isEnable()) {
                        lnSearch.setVisibility(View.VISIBLE);
                        imgDeleteChooseUser.setVisibility(View.VISIBLE);
                        KeyboardManager.showKeyBoard(mainAct);
                    } else {
                        lnSearch.setVisibility(View.GONE);
                        imgDeleteChooseUser.setVisibility(View.GONE);
                    }
                }
                break;
            }
        }

        if (isEnable) {
            ArrayList<UserAndGroup> lstUserAndGroupAll = getUserAndGroups();

            if (selectedUserAndGroup != null && lstUserAndGroupAll.size() > 0) {
                lstUserAndGroupAll = (ArrayList<UserAndGroup>) lstUserAndGroupAll.stream().filter(r -> !r.getID().equals(selectedUserAndGroup.getID())).collect(Collectors.toList());
            }
            assert selectedUserAndGroup != null;
            if (selectedUserAndGroup.getID() != null) {
                lstSelected.add(selectedUserAndGroup);
                SelectUserGroupMultiple_TextAdapter adapterListUserSelected = new SelectUserGroupMultiple_TextAdapter(mainAct, lstSelected, elementParent.isEnable(), this);
                FlexboxLayoutManager flexboxLayoutManager = new FlexboxLayoutManager(mainAct);
                flexboxLayoutManager.setFlexDirection(FlexDirection.ROW);
                flexboxLayoutManager.setJustifyContent(JustifyContent.FLEX_START);
                recySelectedUser.setAdapter(adapterListUserSelected);
                recySelectedUser.setLayoutManager(flexboxLayoutManager);
            }

            SelectUserSingleAdapter adapterListUser = new SelectUserSingleAdapter(mainAct, lstUserAndGroupAll, selectedUserAndGroup, this);
            StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, StaggeredGridLayoutManager.VERTICAL);
            recyChooseUser.setAdapter(adapterListUser);
            recyChooseUser.setLayoutManager(staggeredGridLayoutManager);

            edtSearchChooseUser.addTextChangedListener(new TextWatcher() {
                @Override
                public void beforeTextChanged(CharSequence s, int start, int count, int after) {

                }

                @Override
                public void onTextChanged(CharSequence s, int start, int before, int count) {

                }

                @Override
                public void afterTextChanged(Editable s) {
                    adapterListUser.getFilter().filter(s.toString());
                }
            });
        } else {
            assert selectedUserAndGroup != null;
            if (selectedUserAndGroup.getID() != null) {
                lstSelected.add(selectedUserAndGroup);
                SelectUserGroupMultiple_TextAdapter adapterListUserSelected = new SelectUserGroupMultiple_TextAdapter(mainAct, lstSelected, elementParent.isEnable(), this);

                FlexboxLayoutManager flexboxLayoutManager = new FlexboxLayoutManager(mainAct);
                flexboxLayoutManager.setFlexDirection(FlexDirection.ROW);
                flexboxLayoutManager.setJustifyContent(JustifyContent.FLEX_START);
                recySelectedUser.setAdapter(adapterListUserSelected);
                recySelectedUser.setLayoutManager(flexboxLayoutManager);
            }
        }

        // region Dialog
        dialog = new Dialog(mainAct, R.style.Theme_Custom_BPMOP_Dialog_FullScreen);
        Window window = dialog.getWindow();
        dialog.requestWindowFeature(1);
        dialog.setCanceledOnTouchOutside(false);
        dialog.setCancelable(true);
        window.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_ADJUST_PAN);
        window.setGravity(Gravity.BOTTOM);
        dialog.setContentView(view);
        dialog.show();

        WindowManager.LayoutParams s = window.getAttributes();
        s.width = WindowManager.LayoutParams.MATCH_PARENT;
        s.height = WindowManager.LayoutParams.MATCH_PARENT;
        window.setAttributes(s);
        // endregion

        imgDeleteChooseUser.setOnClickListener(this);
        imgCloseChooseUser.setOnClickListener(this);

        edtSearchChooseUser.setText("");
        edtSearchChooseUser.requestFocus();
    }

    private void delete() {
        KeyboardManager.hideKeyboard(edtSearchChooseUser, mainAct);
        switch (flagView) {
            case Vars.FlagViewControlDynamic.DetailWorkflow: {
                Intent intent = new Intent();
                intent.setAction(VarsReceiver.UPDATEFORM);
                intent.putExtra("element", new Gson().toJson(elementParent));
                intent.putExtra("newValue", "");
                BroadcastUtility.send(mainAct, intent);
                break;
            }
            case Vars.FlagViewControlDynamic.DetailWorkflow_InputGridDetail: {
                Intent intent = new Intent();
                intent.setAction(VarsReceiver.UPDATECHILDACTION);
                intent.putExtra("elementParent", new Gson().toJson(elementParent));
                intent.putExtra("elementChild", new Gson().toJson(elementPopup));
                intent.putExtra("json", JObjectChild.toString());
                intent.putExtra("newValue", "");
                BroadcastUtility.send(mainAct, intent);
                break;
            }
        }
        dialog.dismiss();
    }

    private ArrayList<UserAndGroup> getUserAndGroups() {
        ArrayList<UserAndGroup> lstUserAndGroupAll = new ArrayList<>();
        RealmResults<User> results = realm.where(User.class).isNotNull("Email").findAll();
        if (isUserAndGroup) {
            for (User r : results) {
                UserAndGroup user = new UserAndGroup();
                user.setID(r.getID());
                user.setAccountName(r.getAccountName());
                user.setEmail(r.getEmail());
                user.setName(r.getFullName());
                user.setType("0");
                user.setImagePath(r.getImagePath());
                user.setSearch(r.getFullName() + r.getEmail());
                lstUserAndGroupAll.add(user);
            }

            RealmResults<Group> groupRealmResults = realm.where(Group.class).isNotNull("Description").findAll();
            for (Group r : groupRealmResults) {
                UserAndGroup user = new UserAndGroup();
                user.setID(r.getID());
                user.setAccountName(r.getTitle());
                user.setEmail(r.getDescription());
                user.setName(r.getTitle());
                user.setType("1");
                user.setImagePath(r.getImage());
                user.setSearch(r.getTitle() + r.getDescription());
                lstUserAndGroupAll.add(user);
            }
        } else {
            for (User r : results) {
                UserAndGroup user = new UserAndGroup();
                user.setID(r.getID());
                user.setAccountName(r.getAccountName());
                user.setEmail(r.getEmail());
                user.setName(r.getFullName());
                user.setType("0");
                user.setImagePath(r.getImagePath());
                user.setSearch(r.getFullName() + r.getEmail());
                lstUserAndGroupAll.add(user);
            }
        }
        return lstUserAndGroupAll;
    }

    @Override
    public void OnDeleteItem(int pos) {
        KeyboardManager.hideKeyboard(edtSearchChooseUser, mainAct);
        switch (flagView) {
            case Vars.FlagViewControlDynamic.DetailWorkflow: {
                Intent intent = new Intent();
                intent.setAction(VarsReceiver.UPDATEFORM);
                intent.putExtra("element", new Gson().toJson(elementParent));
                intent.putExtra("newValue", "");
                BroadcastUtility.send(mainAct, intent);
                break;
            }
            case Vars.FlagViewControlDynamic.DetailWorkflow_InputGridDetail: {
                Intent intent = new Intent();
                intent.setAction(VarsReceiver.UPDATECHILDACTION);
                intent.putExtra("elementParent", new Gson().toJson(elementParent));
                intent.putExtra("elementChild", new Gson().toJson(elementPopup));
                intent.putExtra("json", JObjectChild.toString());
                intent.putExtra("newValue", "");
                BroadcastUtility.send(mainAct, intent);
                break;
            }
        }
        dialog.dismiss();
    }

    @Override
    public void OnClick(UserAndGroup userAndGroup) {
        KeyboardManager.hideKeyboard(edtSearchChooseUser, mainAct);
        selectedUserAndGroup = userAndGroup;
        String result;
        ArrayList<UserAndGroup> lstResult = new ArrayList<>();
        lstResult.add(selectedUserAndGroup);
        result = new Gson().toJson(lstResult);

        switch (flagView) {
            case Vars.FlagViewControlDynamic.DetailWorkflow: {
                Intent intent = new Intent();
                intent.setAction(VarsReceiver.UPDATEFORM);
                intent.putExtra("element", new Gson().toJson(elementParent));
                intent.putExtra("newValue", result);
                BroadcastUtility.send(mainAct, intent);
                break;
            }
            case Vars.FlagViewControlDynamic.DetailWorkflow_InputGridDetail: {
                Intent intent = new Intent();
                intent.setAction(VarsReceiver.UPDATECHILDACTION);
                intent.putExtra("elementParent", new Gson().toJson(elementParent));
                intent.putExtra("elementChild", new Gson().toJson(elementPopup));
                intent.putExtra("json", JObjectChild.toString());
                intent.putExtra("newValue", result);
                BroadcastUtility.send(mainAct, intent);
                break;
            }
        }
        dialog.dismiss();
    }

    @Override
    public void onClick(View view) {
        switch (view.getId()) {
            case R.id.img_PopupControl_ChooseUser_Delete: {
                delete();
                break;
            }
            case R.id.img_PopupControl_ChooseUser_Close: {
                KeyboardManager.hideKeyboard(edtSearchChooseUser, mainAct);
                dialog.dismiss();
                break;
            }
        }
    }
}

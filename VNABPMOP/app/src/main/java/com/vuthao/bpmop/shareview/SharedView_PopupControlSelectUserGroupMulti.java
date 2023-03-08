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
import com.vuthao.bpmop.core.Vars;
import com.vuthao.bpmop.detail.adapter.SelectUserGroupMultipleAdapter;
import com.vuthao.bpmop.shareview.adapter.SelectUserGroupMultiple_TextAdapter;

import org.json.JSONObject;

import java.util.ArrayList;
import java.util.stream.Collectors;

import io.realm.Realm;
import io.realm.RealmResults;

public class SharedView_PopupControlSelectUserGroupMulti extends SharedView_PopupControlBase implements TextWatcher, View.OnClickListener, SelectUserGroupMultiple_TextAdapter.SelectUserGroupMultiple_TextListener, SelectUserGroupMultipleAdapter.SelectUserGroupMultipleListener {
    private final boolean isUserAndGroup;
    private ArrayList<UserAndGroup> lstUserAndGroupAll = new ArrayList<UserAndGroup>();
    private ArrayList<UserAndGroup> lstSelected = new ArrayList<UserAndGroup>();
    private ArrayList<UserAndGroup> lstResult = new ArrayList<UserAndGroup>();
    private SelectUserGroupMultipleAdapter adapterListUser;
    private SelectUserGroupMultiple_TextAdapter adapterListUserSelected;
    private String result;
    private final Realm realm;

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

    public SharedView_PopupControlSelectUserGroupMulti(LayoutInflater inflater, Activity mainAct, String fragmentTag, View rootView, boolean isUserAndGroup) {
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

        recySelectedUser.setVisibility(View.VISIBLE);

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
                if (!Functions.isNullOrEmpty(elementParent.getValue())) {
                    lstSelected = new Gson().fromJson(elementParent.getValue(), new TypeToken<ArrayList<UserAndGroup>>() {
                    }.getType());
                }

                isEnable = elementParent.isEnable();
                break;
            }
            case Vars.FlagViewControlDynamic.DetailWorkflow_InputGridDetail: {
                tvTitleChooseUser.setText(elementPopup.getTitle());
                if (!Functions.isNullOrEmpty(elementPopup.getValue())) {
                    lstSelected = new Gson().fromJson(elementPopup.getValue(), new TypeToken<ArrayList<UserAndGroup>>() {
                    }.getType());
                }

                isEnable = elementPopup.isEnable();
                break;
            }
        }

        if (isEnable) {
            lnSearch.setVisibility(View.VISIBLE);
            imgDeleteChooseUser.setVisibility(View.VISIBLE);
            imgAcceptChooseUser.setVisibility(View.VISIBLE);
            recySelectedUser.setMaxRowAndRowHeight(Functions.share.convertDpToPixel(35f, mainAct.getBaseContext()), 3);

            lstUserAndGroupAll = getUserAndGroups();
            if (!lstUserAndGroupAll.isEmpty() && !lstSelected.isEmpty()) {
                for (int i = 0; i < lstSelected.size(); i++) {
                    int finalI = i;
                    lstUserAndGroupAll = (ArrayList<UserAndGroup>) lstUserAndGroupAll.stream().filter(r -> !r.getID().equals(lstSelected.get(finalI).getID())).collect(Collectors.toList());
                }
            }

            adapterListUser = new SelectUserGroupMultipleAdapter(mainAct, lstUserAndGroupAll, this);
            StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, StaggeredGridLayoutManager.VERTICAL);
            recyChooseUser.setAdapter(adapterListUser);
            recyChooseUser.setLayoutManager(staggeredGridLayoutManager);

            adapterListUserSelected = new SelectUserGroupMultiple_TextAdapter(mainAct, lstSelected, elementParent.isEnable(), this);
            FlexboxLayoutManager flexboxLayoutManager = new FlexboxLayoutManager(mainAct);
            flexboxLayoutManager.setFlexDirection(FlexDirection.ROW);
            flexboxLayoutManager.setJustifyContent(JustifyContent.FLEX_START);
            recySelectedUser.setAdapter(adapterListUserSelected);
            recySelectedUser.setLayoutManager(flexboxLayoutManager);

            edtSearchChooseUser.addTextChangedListener(this);
        } else {
            lnSearch.setVisibility(View.GONE);
            imgDeleteChooseUser.setVisibility(View.GONE);
            imgAcceptChooseUser.setVisibility(View.INVISIBLE);

            //trừ đi top navigation và / 35 số cứng
            int maxLines = (mainAct.getResources().getDisplayMetrics().heightPixels - (int) Functions.share.convertDpToPixel(50, mainAct.getBaseContext()) / Functions.share.convertDpToPixel(35, mainAct.getBaseContext()));
            recySelectedUser.setMaxRowAndRowHeight(Functions.share.convertDpToPixel(35, mainAct.getBaseContext()), maxLines);

            adapterListUserSelected = new SelectUserGroupMultiple_TextAdapter(mainAct, lstSelected, elementParent.isEnable(), pos -> {
            });

            FlexboxLayoutManager flexboxLayoutManager = new FlexboxLayoutManager(mainAct);
            flexboxLayoutManager.setFlexDirection(FlexDirection.ROW);
            flexboxLayoutManager.setJustifyContent(JustifyContent.FLEX_START);
            recySelectedUser.setAdapter(adapterListUserSelected);
            recySelectedUser.setLayoutManager(flexboxLayoutManager);
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

        imgCloseChooseUser.setOnClickListener(this);

        imgDeleteChooseUser.setOnClickListener(this);

        imgAcceptChooseUser.setOnClickListener(this);

        edtSearchChooseUser.setText("");
        edtSearchChooseUser.requestFocus();

        if (elementParent.isEnable()) {
            KeyboardManager.showKeyBoard(edtSearchChooseUser, mainAct);
        }
    }

    private void delete() {
        KeyboardManager.hideKeyboard(edtSearchChooseUser, mainAct);
        switch (flagView) {
            case Vars.FlagViewControlDynamic.DetailWorkflow: {
                Intent intent = new Intent();
                intent.setAction("UPDATEFORM");
                intent.putExtra("element", new Gson().toJson(elementParent));
                intent.putExtra("newValue", "");
                BroadcastUtility.send(mainAct, intent);
                break;
            }
            case Vars.FlagViewControlDynamic.DetailWorkflow_InputGridDetail: {
                Intent intent = new Intent();
                intent.setAction("UPDATECHILDACTION");
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

    private void accept() {
        KeyboardManager.hideKeyboard(edtSearchChooseUser, mainAct);
        if (adapterListUserSelected != null) {
            lstResult = adapterListUserSelected.getListIsclicked();
        }

        result = new Gson().toJson(lstResult);
        switch (flagView) {
            case Vars.FlagViewControlDynamic.DetailWorkflow: {
                Intent intent = new Intent();
                intent.setAction("UPDATEFORM");
                intent.putExtra("element", new Gson().toJson(elementParent));
                intent.putExtra("newValue", result);
                BroadcastUtility.send(mainAct, intent);
                break;
            }
            case Vars.FlagViewControlDynamic.DetailWorkflow_InputGridDetail: {
                Intent intent = new Intent();
                intent.setAction("UPDATECHILDACTION");
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

    private ArrayList<UserAndGroup> getUserAndGroups() {
        ArrayList<UserAndGroup> _lstUserAndGroupAll = new ArrayList<>();
        if (isUserAndGroup) {
            RealmResults<User> results = realm.where(User.class).isNotNull("Email").findAll();
            for (User r : results) {
                UserAndGroup user = new UserAndGroup();
                user.setID(r.getID());
                user.setAccountName(r.getAccountName());
                user.setEmail(r.getEmail());
                user.setName(r.getFullName());
                user.setType("0");
                user.setImagePath(r.getImagePath());
                user.setSearch(r.getFullName() + r.getEmail());
                _lstUserAndGroupAll.add(user);
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
                _lstUserAndGroupAll.add(user);
            }
        } else {
            RealmResults<User> results = realm.where(User.class).isNotNull("Email").findAll();
            for (User r : results) {
                UserAndGroup user = new UserAndGroup();
                user.setID(r.getID());
                user.setAccountName(r.getAccountName());
                user.setEmail(r.getEmail());
                user.setName(r.getFullName());
                user.setType("0");
                user.setImagePath(r.getImagePath());
                user.setSearch(r.getFullName() + r.getEmail());
                _lstUserAndGroupAll.add(user);
            }
        }
        return _lstUserAndGroupAll;
    }

    @Override
    public void OnClick(UserAndGroup userAndGroup) {
        lstSelected.add(userAndGroup);
        lstUserAndGroupAll.remove(userAndGroup);
        adapterListUser.updateCurrentList(lstUserAndGroupAll);
        adapterListUserSelected.updateItemListIsClicked(lstSelected);
        adapterListUserSelected.notifyDataSetChanged();
        edtSearchChooseUser.setText("");
    }

    @Override
    public void OnDeleteItem(int pos) {
        lstUserAndGroupAll.add(lstSelected.get(pos));
        lstSelected.remove(lstSelected.get(pos));
        adapterListUser.updateCurrentList(lstUserAndGroupAll);
        adapterListUserSelected.updateItemListIsClicked(lstSelected);
        adapterListUserSelected.notifyDataSetChanged();
        edtSearchChooseUser.setText(edtSearchChooseUser.getText());
        edtSearchChooseUser.setSelection(edtSearchChooseUser.length());
    }

    @Override
    public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {

    }

    @Override
    public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {

    }

    @Override
    public void afterTextChanged(Editable s) {
        adapterListUser.getFilter().filter(s.toString());
    }

    @Override
    public void onClick(View view) {
        switch (view.getId()) {
            case R.id.img_PopupControl_ChooseUser_Close: {
                KeyboardManager.hideKeyboard(edtSearchChooseUser, mainAct);
                dialog.dismiss();
                break;
            }
            case R.id.img_PopupControl_ChooseUser_Delete: {
                delete();
                break;
            }
            case R.id.img_PopupControl_ChooseUser_Accept: {
                accept();
                break;
            }
        }
    }
}

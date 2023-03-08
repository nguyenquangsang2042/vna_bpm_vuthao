package com.vuthao.bpmop.shareview;

import android.app.Activity;
import android.app.Dialog;
import android.content.Intent;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.localbroadcastmanager.content.LocalBroadcastManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.recyclerview.widget.StaggeredGridLayoutManager;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.keyboard.KeyboardManager;
import com.vuthao.bpmop.base.model.custom.LookupData;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.core.Vars;
import com.vuthao.bpmop.shareview.adapter.FormControlSingleChoiceAdapter;

import org.json.JSONObject;

import java.util.ArrayList;

public class SharedView_PopupControlSingleChoice extends SharedView_PopupControlBase implements View.OnClickListener, FormControlSingleChoiceAdapter.FormControlSingleChoiceListener {
    private final ArrayList<LookupData> lstLookupData = new ArrayList<>();
    private Dialog dialog;
    private View view;
    private ImageView imgClose;
    private ImageView imgDelete;
    private TextView tvTitle;
    private RecyclerView recyData;
    private ImageView imgDone;

    public SharedView_PopupControlSingleChoice(LayoutInflater inflater, Activity mainAct, String fragmentTag, View rootView) {
        super(inflater, mainAct, fragmentTag, rootView);
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

        view = inflater.inflate(R.layout.popup_control_single_choice, null);
        imgClose = view.findViewById(R.id.img_PopupControl_SingleChoice_Close);
        imgDelete = view.findViewById(R.id.img_PopupControl_SingleChoice_Delete);
        tvTitle = view.findViewById(R.id.tv_PopupControl_SingleChoice_Title);
        recyData = view.findViewById(R.id.recy_PopupControl_SingleChoice_Data);
        imgDone = view.findViewById(R.id.img_PopupControl_SingleChoice_Done);

        imgDone.setVisibility(View.INVISIBLE);
        imgDone.getLayoutParams().width = 0;

        if (elementParent.isEnable()) {
            imgDelete.setVisibility(View.VISIBLE);
        } else {
            imgDelete.setVisibility(View.GONE);
        }

        switch (flagView) {
            case Vars.FlagViewControlDynamic.DetailWorkflow:
            default: {
                tvTitle.setText(elementParent.getTitle());

                if (!Functions.isNullOrEmpty(elementParent.getDataSource())) {
                    lstLookupData.addAll(new Gson().fromJson(elementParent.getDataSource(), new TypeToken<ArrayList<LookupData>>() {
                    }.getType()));

                    if (lstLookupData.size() > 0) {
                        for (int i = 0; i < lstLookupData.size(); i++) {
                            if (elementParent.getValue().contains(lstLookupData.get(i).getID())) {
                                lstLookupData.get(i).setSelected(true);
                                break;
                            }
                        }
                    }
                }

                break;
            }
            case Vars.FlagViewControlDynamic.DetailWorkflow_InputGridDetail: {
                tvTitle.setText(elementPopup.getTitle());

                if (!Functions.isNullOrEmpty(elementPopup.getDataSource())) {
                    lstLookupData.addAll(new Gson().fromJson(elementPopup.getDataSource(), new TypeToken<ArrayList<LookupData>>() {
                    }.getType()));

                    if (lstLookupData.size() > 0) {
                        for (int i = 0; i < lstLookupData.size(); i++) {
                            if (elementPopup.getValue().contains(lstLookupData.get(i).getID())) {
                                lstLookupData.get(i).setSelected(true);
                                break;
                            }
                        }
                    }
                }

                break;
            }
        }

        FormControlSingleChoiceAdapter adapterFormControlSingleChoice = new FormControlSingleChoiceAdapter(mainAct, mainAct.getApplicationContext(), lstLookupData, this);
        StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, StaggeredGridLayoutManager.VERTICAL);
        recyData.setAdapter(adapterFormControlSingleChoice);
        recyData.setLayoutManager(staggeredGridLayoutManager);

        imgDelete.setOnClickListener(this);
        imgClose.setOnClickListener(this);

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
    }

    private void delete() {
        switch (flagView) {
            case Vars.FlagViewControlDynamic.DetailWorkflow:
            default: {
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
    public void OnClick(int pos) {
        if (elementParent.isEnable()) {
            LookupData selectedLookupItem = lstLookupData.get(pos);
            if (selectedLookupItem != null) {
                ArrayList<LookupData> lstResult = new ArrayList<>();
                lstResult.add(selectedLookupItem);
                switch (flagView) {
                    case Vars.FlagViewControlDynamic.DetailWorkflow:
                    default: {
                        Intent intent = new Intent();
                        intent.setAction(VarsReceiver.UPDATEFORM);
                        intent.putExtra("element", new Gson().toJson(elementParent));
                        intent.putExtra("newValue", new Gson().toJson(lstResult));
                        BroadcastUtility.send(mainAct, intent);
                        break;
                    }
                    case Vars.FlagViewControlDynamic.DetailWorkflow_InputGridDetail: {
                        Intent intent = new Intent();
                        intent.setAction(VarsReceiver.UPDATECHILDACTION);
                        intent.putExtra("elementParent", new Gson().toJson(elementParent));
                        intent.putExtra("elementChild", new Gson().toJson(elementPopup));
                        intent.putExtra("json", JObjectChild.toString());
                        intent.putExtra("newValue", new Gson().toJson(lstResult));
                        BroadcastUtility.send(mainAct, intent);
                        break;
                    }
                }
                dialog.dismiss();
            }
        }
    }

    @Override
    public void onClick(View view) {
        switch (view.getId()) {
            case R.id.img_PopupControl_SingleChoice_Delete: {
                delete();
                break;
            }
            case R.id.img_PopupControl_SingleChoice_Close: {
                KeyboardManager.hideKeyboard(mainAct);
                dialog.dismiss();
                break;
            }
        }
    }
}

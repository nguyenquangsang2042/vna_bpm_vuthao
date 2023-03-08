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
import com.vuthao.bpmop.base.model.custom.LookupData;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.core.Vars;
import com.vuthao.bpmop.shareview.adapter.FormControlMultiChoiceAdapter;

import org.json.JSONObject;

import java.util.ArrayList;
import java.util.stream.Collectors;

public class SharedView_PopupControlMultiChoice extends SharedView_PopupControlBase implements FormControlMultiChoiceAdapter.FormControlMultiChoiceListener, View.OnClickListener {
    private ArrayList<LookupData> lstLookupData = new ArrayList<LookupData>();
    private ArrayList<LookupData> lstselected = new ArrayList<LookupData>();
    private FormControlMultiChoiceAdapter adapter;
    private String result;

    private Dialog dialog;
    private View view;
    private ImageView imgClose;
    private ImageView imgDelete;
    private TextView tvTitle;
    private RecyclerView recyData;
    private ImageView imgDone;

    public SharedView_PopupControlMultiChoice(LayoutInflater inflater, Activity mainAct, String fragmentTag, View rootView) {
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

        if (elementParent.isEnable()) {
            imgDone.setVisibility(View.VISIBLE);
            imgDelete.setVisibility(View.VISIBLE);
        } else {
            imgDone.setVisibility(View.INVISIBLE);
            imgDelete.setVisibility(View.INVISIBLE);
        }

        ArrayList<LookupData> lstValue;
        switch (flagView) {
            case Vars.FlagViewControlDynamic.DetailWorkflow:
            default: {
                tvTitle.setText(elementParent.getTitle());
                lstLookupData = new Gson().fromJson(elementParent.getDataSource(), new TypeToken<ArrayList<LookupData>>() {
                }.getType());

                if (!lstLookupData.isEmpty() && !Functions.isNullOrEmpty(elementParent.getValue())) {
                    lstValue = new Gson().fromJson(elementParent.getValue(), new TypeToken<ArrayList<LookupData>>() {
                    }.getType());

                    if (lstValue.isEmpty()) {
                        break;
                    }

                    for (LookupData itemLookup : lstLookupData) {
                        for (LookupData itemValue : lstValue) {
                            // item nào check qua rồi ko cần check lại.
                            if (itemLookup.getID().equals(itemValue.getID()) && !itemLookup.isSelected()) {
                                itemLookup.setSelected(true);
                            }
                        }
                    }
                }
                break;
            }
            case Vars.FlagViewControlDynamic.DetailWorkflow_InputGridDetail: {
                tvTitle.setText(elementPopup.getTitle());
                lstLookupData = new Gson().fromJson(elementPopup.getDataSource(), new TypeToken<ArrayList<LookupData>>() {
                }.getType());

                if (!lstLookupData.isEmpty() && !Functions.isNullOrEmpty(elementPopup.getValue())) {
                    lstValue = new Gson().fromJson(elementPopup.getValue(), new TypeToken<ArrayList<LookupData>>() {
                    }.getType());

                    if (lstValue.isEmpty()) {
                        break;
                    }

                    for (LookupData itemLookup : lstLookupData) {
                        for (LookupData itemValue : lstValue) {
                            // item nào check qua rồi ko cần check lại.
                            if (itemLookup.getID().equals(itemValue.getID()) && !itemLookup.isSelected()) {
                                itemLookup.setSelected(true);
                            }
                        }
                    }
                }
                break;
            }
        }

        adapter = new FormControlMultiChoiceAdapter(mainAct, lstLookupData, this);
        StaggeredGridLayoutManager staggeredGridLayoutManager = new StaggeredGridLayoutManager(1, StaggeredGridLayoutManager.VERTICAL);
        recyData.setAdapter(adapter);
        recyData.setLayoutManager(staggeredGridLayoutManager);

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

        imgDone.setOnClickListener(this);
        imgDelete.setOnClickListener(this);
        imgClose.setOnClickListener(this);
    }

    private void done() {
        lstselected = (ArrayList<LookupData>) lstLookupData.stream().filter(LookupData::isSelected).collect(Collectors.toList());
        if (!lstselected.isEmpty()) {
            result = new Gson().toJson(lstselected);
        }

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

    private void delete() {
        for (int i = 0; i < lstLookupData.size(); i++) {
            lstLookupData.get(i).setSelected(false);
        }
        imgDone.performClick();
    }

    @Override
    public void onClick(View view) {
        switch (view.getId()) {
            case R.id.img_PopupControl_SingleChoice_Done: {
                done();
                break;
            }
            case R.id.img_PopupControl_SingleChoice_Delete: {
                delete();
                break;
            }
            case R.id.img_PopupControl_SingleChoice_Close: {
                dialog.dismiss();
                break;
            }
        }
    }

    @Override
    public void OnClick(LookupData data) {
        //Nếu control dc Enable mới xử lý
        if (elementParent.isEnable()) {
            for (int i = 0; i < lstLookupData.size(); i++) {
                if (lstLookupData.get(i).getID().equals(data.getID())) {
                    lstLookupData.get(i).setSelected(!lstLookupData.get(i).isSelected());
                    adapter.notifyDataSetChanged();
                    break;
                }
            }
        }
    }
}

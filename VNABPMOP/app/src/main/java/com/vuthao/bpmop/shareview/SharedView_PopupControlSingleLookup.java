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

import androidx.annotation.NonNull;
import androidx.localbroadcastmanager.content.LocalBroadcastManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.recyclerview.widget.StaggeredGridLayoutManager;

import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.api.ApiController;
import com.vuthao.bpmop.base.model.ApiList;
import com.vuthao.bpmop.base.model.custom.LookupData;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.core.Vars;
import com.vuthao.bpmop.shareview.adapter.FormControlSingleChoiceAdapter;

import org.json.JSONObject;

import java.util.ArrayList;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class SharedView_PopupControlSingleLookup extends SharedView_PopupControlBase implements TextWatcher, View.OnClickListener {
    private ArrayList<LookupData> lstLookupData = new ArrayList<>();
    private FormControlSingleChoiceAdapter adapter;
    private Dialog dialog;

    private View view;
    private LinearLayout lnProgressBar;
    private LinearLayout lnSearch;
    private ImageView imgClose;
    private ImageView imgDelete;
    private TextView tvTitle;
    private RecyclerView recyData;
    private ImageView imgDone;
    private EditText edtSearch;
    private TextView tvNodata;

    public SharedView_PopupControlSingleLookup(LayoutInflater inflater, Activity mainAct, String fragmentTag, View rootView) {
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
        lnProgressBar = view.findViewById(R.id.lnProgressbar);
        lnSearch = view.findViewById(R.id.ln_PopupControl_SingleChoice_Search);
        imgClose = view.findViewById(R.id.img_PopupControl_SingleChoice_Close);
        imgDelete = view.findViewById(R.id.img_PopupControl_SingleChoice_Delete);
        tvTitle = view.findViewById(R.id.tv_PopupControl_SingleChoice_Title);
        recyData = view.findViewById(R.id.recy_PopupControl_SingleChoice_Data);
        imgDone = view.findViewById(R.id.img_PopupControl_SingleChoice_Done);
        edtSearch = view.findViewById(R.id.edt_PopupControl_SingleChoice_Search);
        tvNodata = view.findViewById(R.id.tv_PopupControl_SingleChoice_NoData);

        edtSearch.setHint(Functions.share.getTitle("TEXT_SEARCH", "Search..."));

        lnSearch.setVisibility(View.VISIBLE);
        lnProgressBar.setVisibility(View.VISIBLE);
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
                getDataSource(elementParent);
                break;
            }
            case Vars.FlagViewControlDynamic.DetailWorkflow_InputGridDetail: {
                tvTitle.setText(elementPopup.getTitle());
                getDataSource(elementPopup);
                break;
            }
        }

        adapter = new FormControlSingleChoiceAdapter(mainAct, mainAct.getApplicationContext(), lstLookupData, pos -> {
            if (elementParent.isEnable()) {
                LookupData selectedLookupItem = lstLookupData.get(pos);
                ArrayList<LookupData> lstSelected = new ArrayList<>();
                lstSelected.add(selectedLookupItem);

                switch (flagView) {
                    case Vars.FlagViewControlDynamic.DetailWorkflow:
                    default: {
                        Intent intent = new Intent();
                        intent.setAction(VarsReceiver.UPDATEFORM);
                        intent.putExtra("element", new Gson().toJson(elementParent));
                        intent.putExtra("newValue", new Gson().toJson(lstSelected));
                        BroadcastUtility.send(mainAct, intent);
                        break;
                    }
                    case Vars.FlagViewControlDynamic.DetailWorkflow_InputGridDetail: {
                        Intent intent = new Intent();
                        intent.setAction(VarsReceiver.UPDATECHILDACTION);
                        intent.putExtra("elementParent", new Gson().toJson(elementParent));
                        intent.putExtra("elementChild", new Gson().toJson(elementPopup));
                        intent.putExtra("json", JObjectChild.toString());
                        intent.putExtra("newValue", new Gson().toJson(lstSelected));
                        BroadcastUtility.send(mainAct, intent);
                        break;
                    }
                }
            }

            dialog.dismiss();
        });

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

        imgClose.setOnClickListener(this);
        imgDelete.setOnClickListener(this);
        edtSearch.addTextChangedListener(this);
    }

    private void getDataSource(ViewElement element) {
        if (element.getListProprety() != null && element.getListProprety().size() > 0) {
            String lookupWebId = element.getListProprety().get(0).values().toArray()[0].toString();
            String lookupList = element.getListProprety().get(1).values().toArray()[0].toString();
            String lookupField = element.getListProprety().get(2).values().toArray()[0].toString();
            Call<ApiList<LookupData>> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getDataSource(lookupWebId, lookupList, lookupField);
            call.enqueue(new Callback<ApiList<LookupData>>() {
                @Override
                public void onResponse(@NonNull Call<ApiList<LookupData>> call, @NonNull Response<ApiList<LookupData>> response) {
                    assert response.body() != null;
                    lstLookupData = new ArrayList<>(response.body().getData());
                    switch (flagView) {
                        default:
                        case Vars.FlagViewControlDynamic.DetailWorkflow: {
                            for (int i = 0; i < lstLookupData.size(); i++) {
                                if (elementParent.getValue().contains(new Gson().toJson(lstLookupData.get(i)))) {
                                    lstLookupData.get(i).setSelected(true);
                                    break;
                                }
                            }
                            break;
                        }
                        case Vars.FlagViewControlDynamic.DetailWorkflow_InputGridDetail: {
                            for (int i = 0; i < lstLookupData.size(); i++) {
                                if (elementPopup.getValue().contains(new Gson().toJson(lstLookupData.get(i)))) {
                                    lstLookupData.get(i).setSelected(true);
                                    break;
                                }
                            }
                            break;
                        }
                    }

                    adapter.setList(lstLookupData);
                    edtSearch.setText("");
                    lnProgressBar.setVisibility(View.GONE);
                }

                @Override
                public void onFailure(@NonNull Call<ApiList<LookupData>> call, @NonNull Throwable t) {
                    lnProgressBar.setVisibility(View.GONE);
                }
            });
        } else {
            lnSearch.setVisibility(View.GONE);
            lnProgressBar.setVisibility(View.GONE);
            tvNodata.setVisibility(View.VISIBLE);
        }
    }

    @Override
    public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {

    }

    @Override
    public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {

    }

    @Override
    public void afterTextChanged(Editable s) {
        if (adapter != null) {
            adapter.getFilter().filter(s.toString());
        }
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
    public void onClick(View view) {
        switch (view.getId()) {
            case R.id.img_PopupControl_SingleChoice_Close: {
                dialog.dismiss();
                break;
            }
            case R.id.img_PopupControl_SingleChoice_Delete: {
                delete();
                break;
            }
        }
    }
}

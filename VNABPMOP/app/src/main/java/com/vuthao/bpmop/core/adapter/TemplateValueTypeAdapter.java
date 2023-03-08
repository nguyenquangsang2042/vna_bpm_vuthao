package com.vuthao.bpmop.core.adapter;

import android.app.Activity;
import android.content.Context;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.LinearLayout;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Formula;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.custom.WFDetailsHeader;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.core.Vars;
import com.vuthao.bpmop.core.component.TemplateValueType;

import org.json.JSONArray;
import org.json.JSONObject;

import java.math.BigInteger;
import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;

public class TemplateValueTypeAdapter extends RecyclerView.Adapter<TemplateValueTypeAdapter.RecyTemplateValueTypeHolder> {
    private Activity mainAct;
    private ViewElement element;
    public ArrayList<WFDetailsHeader> lstHeader = new ArrayList<WFDetailsHeader>();
    private ArrayList<JSONObject> ListJObjectRow = new ArrayList<JSONObject>();
    private JSONObject clickedObject = new JSONObject();
    private int flagView = -1;

    //Edit
    public TemplateValueTypeAdapter(Activity mainAct, Context context, ViewElement element, JSONObject clickedObject, int flagView) {
        this.mainAct = mainAct;
        this.element = element;
        this.clickedObject = clickedObject;
        this.flagView = flagView;

        //Headers
        if (!Functions.isNullOrEmpty(element.getDataSource())) {
            lstHeader = new Gson().fromJson(element.getDataSource(), new TypeToken<ArrayList<WFDetailsHeader>>() {
            }.getType());
        }

        //Component Value
        try {
            JSONArray jsonArray = new JSONArray(element.getValue());
            for (int i = 0; i < jsonArray.length(); i++) {
                ListJObjectRow.add(jsonArray.getJSONObject(i));
            }
        } catch (Exception ex) {
            Log.d("ERR Convert JSON TO LIST JSON", ex.getMessage());
        }
    }

    public JSONObject getCurrentJObject()
    {
        return clickedObject;
    }

    public void updateCurrentJObject(JSONObject clickedObject) {
        this.clickedObject = clickedObject;
        try {
            for (WFDetailsHeader header : lstHeader) {
                if (!Functions.isNullOrEmpty(header.getFormula())) {
                    String value = Formula.evaluate(header.getFormula(), clickedObject);
                    Object type = clickedObject.get(header.getInternalName());
                    if (type instanceof Integer || type instanceof Float || type instanceof Long || type instanceof Double) {
                        clickedObject.put(header.getInternalName(), Long.parseLong(value));
                    } else {
                        clickedObject.put(header.getInternalName(), value);
                    }
                }
            }
        } catch (Exception ex) {
            Log.d("ERR updateCurrentJObject" , ex.getMessage());
        }
    }

    //Create
    public TemplateValueTypeAdapter(Activity mainAct, Context context, ViewElement element, int flagView) {
        this.mainAct = mainAct;
        this.element = element;
        this.flagView = flagView;

        //Headers
        if (!Functions.isNullOrEmpty(element.getDataSource())) {
            lstHeader = new Gson().fromJson(element.getDataSource(), new TypeToken<ArrayList<WFDetailsHeader>>() {
            }.getType());
        }

        //Component Value
        try {
            for (WFDetailsHeader header : lstHeader) {
                if (!Functions.isNullOrEmpty(header.getInternalName())) {
                    clickedObject.put(header.getInternalName(), "");
                }
            }

            // bằng 0 là trường hợp tạo mới
            clickedObject.put("ID", 0);

        } catch (Exception ex) {
            Log.d("ERR", ex.getMessage());
        }
    }

    @NonNull
    @Override
    public RecyTemplateValueTypeHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_expand_detail_workflow, parent, false);
        return new RecyTemplateValueTypeHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull TemplateValueTypeAdapter.RecyTemplateValueTypeHolder holder, int position) {
        WFDetailsHeader header = lstHeader.get(position);
        try {
            if (header != null) {
                if (!Functions.isNullOrEmpty(header.getInternalName())) {
                    ViewElement elementItem = new ViewElement();
                    elementItem.setValue("");
                    elementItem.setEnable(element.isEnable());
                    elementItem.setInternalName(header.getInternalName());
                    elementItem.setDataSource("");
                    elementItem.setTitle(header.getTitle());
                    elementItem.setRequire(header.isRequire());
                    elementItem.setDataType(header.getDataType());
                    elementItem.setDataSource(header.getDataSource());

                    if (header.getDataType().equals("number")) {
                        if (Functions.isNullOrEmpty(clickedObject.getString(header.getInternalName()))) {
                            elementItem.setValue(clickedObject.get(header.getInternalName()).toString());
                        } else {
                            BigInteger integer = BigInteger.valueOf(clickedObject.getLong(header.getInternalName()));
                            elementItem.setValue(integer.toString());
                        }
                    } else {
                        elementItem.setValue(clickedObject.get(header.getInternalName()).toString());
                    }

                    holder.lnContent.removeAllViews();

                    TemplateValueType templateValue = new TemplateValueType(mainAct, holder.lnContent, element, elementItem, clickedObject, true, flagView);
                    templateValue.initializeFrameView(holder.lnContent);
                    templateValue.initializeCategory(Vars.EnumDynamicControlCategory.TemplateValue);
                    templateValue.setTitle();
                    templateValue.setValue();
                    templateValue.setEnable();
                    templateValue.setProprety();
                }
            }
        } catch (Exception ex) {
            Log.d("ERR onBindViewHolder", ex.getMessage());
        }
    }

    @Override
    public int getItemCount() {
        return lstHeader.size();
    }

    public JSONObject getCurrentObject() {
        return clickedObject;
    }

    public class RecyTemplateValueTypeHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.ln_ItemExpandDetailWorkflow_Content)
        LinearLayout lnContent;

        public RecyTemplateValueTypeHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
        }
    }
}

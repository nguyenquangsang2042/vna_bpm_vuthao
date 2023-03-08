package com.vuthao.bpmop.core.adapter;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.graphics.Typeface;
import android.util.Log;
import android.util.Pair;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.core.content.res.ResourcesCompat;
import androidx.recyclerview.widget.RecyclerView;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.custom.LookupData;
import com.vuthao.bpmop.base.model.custom.UserAndGroup;
import com.vuthao.bpmop.base.model.custom.WFDetailsHeader;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.vars.VarsControl;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.core.Vars;
import com.vuthao.bpmop.core.component.ControlInputGridDetails;
import com.vuthao.bpmop.detail.custom.DetailFunc;

import org.json.JSONObject;

import java.math.BigInteger;
import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;

public class ListGridDataAdapter extends RecyclerView.Adapter {
    private final Activity activty;
    private final Context context;
    private final LinearLayout parentView;
    private final ViewElement element;
    private final int flagView;
    private final ArrayList<WFDetailsHeader> lstHeader;
    private final ArrayList<Pair<String, Float>> lstColumnTextLength;
    private final ArrayList<JSONObject> lstJObjectRow;
    private boolean isShowSumColumn = false;
    private int allRowCount = -1;

    public ListGridDataAdapter(Activity mainAct, Context context, ArrayList<WFDetailsHeader> lstHeader, ArrayList<JSONObject> lstJObjectRow, LinearLayout parentView, ViewElement element, int flagView) {
        this.activty = mainAct;
        this.context = context;
        this.lstHeader = lstHeader;
        this.lstJObjectRow = lstJObjectRow;
        this.parentView = parentView;
        this.element = element;
        this.flagView = flagView;

        for (WFDetailsHeader header : lstHeader) {
            if (header.isSum()) {
                isShowSumColumn = true;
                break;
            }
        }

        lstColumnTextLength = getListColumnTextLength(lstHeader, lstJObjectRow);
    }

    private ArrayList<Pair<String, Float>> getListSumColumnValue(ArrayList<WFDetailsHeader> lstHeader, ArrayList<JSONObject> lstJObjectRow) {
        ArrayList<Pair<String, Float>> result = new ArrayList<>();
        for (WFDetailsHeader header : lstHeader) {
            if (header.isSum()) {
                for (JSONObject json : lstJObjectRow) {
                    if (!json.isNull(header.getInternalName())) {
                        float sumValue = 0;
                        for (int i = 0; i < lstJObjectRow.size(); i++) {
                            if (!lstJObjectRow.get(i).isNull(header.getInternalName())) {
                                sumValue++;
                            }
                        }

                        result.add(new Pair<>(header.getInternalName(), sumValue));
                    }
                }
            }
        }
        return result;
    }

    private ArrayList<Pair<String, Float>> getListColumnTextLength(ArrayList<WFDetailsHeader> lstHeader, ArrayList<JSONObject> lstJObjectRow) {
        ArrayList<Pair<String, Float>> result = new ArrayList<>();
        for (WFDetailsHeader header : lstHeader) {
            float maxColumnLength = header.getTitle().length();
            if (lstJObjectRow != null && lstJObjectRow.size() > 0) {
                float maxObjectLength = 0;

                try {
                    for (JSONObject json : lstJObjectRow) {
                        if (!json.isNull(header.getInternalName())) {
                            maxObjectLength += header.getInternalName().length();
                        }
                    }

                } catch (Exception ex) {
                    maxObjectLength = 0;
                    Log.d("ERR getListColumnTextLength", ex.getMessage());
                }
                if (maxObjectLength > maxColumnLength) {
                    maxColumnLength = maxObjectLength;
                    // limit lại nếu quá dài
                    if (maxColumnLength > 30)
                        maxColumnLength = 30;
                }
            }

            if (header.getInternalName() != null) {
                result.add(new Pair<>(header.getInternalName(), maxColumnLength));
            }
        }
        return result;
    }

    @NonNull
    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        if (viewType == 0) {
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_control_input_grid_detail_recy, parent, false);
            return new ListGridDataHeaderHolder(view);
        } else {
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_control_input_grid_detail_recy, parent, false);
            return new ListGridDataValueHolder(view);
        }
    }

    @Override
    public void onBindViewHolder(@NonNull RecyclerView.ViewHolder holder, int position) {
        // 1 character = 10dp
        int dpPerCharacter = 5;
        if (holder instanceof ListGridDataHeaderHolder) {
            ListGridDataHeaderHolder holderGridData = (ListGridDataHeaderHolder) holder;
            holderGridData.lnContent.removeAllViews();
            holderGridData.lnContent.setForeground(null);

            for (WFDetailsHeader header : lstHeader) {
                float textlength = 0;
                for (Pair<String, Float> pair : lstColumnTextLength) {
                    if (pair.first.equals(header.getInternalName())) {
                        textlength = pair.second;
                        break;
                    }
                }

                if (textlength == 0) {
                    textlength = 3; // trường hợp #
                }

                LinearLayout.LayoutParams paramsItemRow = new LinearLayout.LayoutParams(Functions.share.convertDpToPixel((textlength * dpPerCharacter) + 25, context),
                        Functions.share.convertDpToPixel(45, context)); // + thêm 2 cái margin Left Right là 20dp

                View itemViewHeader = LayoutInflater.from(context).inflate(R.layout.item_control_input_grid_detail_recy_header, holderGridData.lnContent, false);
                ListGridDataAdapter_ViewHolder_Header_Item holderItem = new ListGridDataAdapter_ViewHolder_Header_Item(itemViewHeader);
                holderItem.lnContent.setLayoutParams(paramsItemRow);
                holderItem.lnContent.setBackgroundColor(ContextCompat.getColor(context, R.color.clGray));

                if (!Functions.isNullOrEmpty(header.getTitle())) {
                    holderItem.tvContent.setText(header.getTitle());
                } else {
                    holderItem.tvContent.setText("");
                }

                holderGridData.lnContent.addView(holderItem.itemView);
            }
        } else if (holder instanceof ListGridDataValueHolder) {
            ((ListGridDataValueHolder) holder).lnContent.removeAllViews();

            if (position % 2 == 0) {
                ((ListGridDataValueHolder) holder).lnContent.setBackgroundColor(ContextCompat.getColor(activty, R.color.clVer2BlueNavigation));
            } else {
                ((ListGridDataValueHolder) holder).lnContent.setBackgroundColor(ContextCompat.getColor(activty, R.color.clWhite));
            }

            for (WFDetailsHeader header : lstHeader) {
                View itemViewValue = LayoutInflater.from(context).inflate(R.layout.item_control_input_grid_detail_recy_value, ((ListGridDataValueHolder) holder).lnContent, false);

                ListGridDataAdapter_ViewHolder_Value_Item holderItem = new ListGridDataAdapter_ViewHolder_Value_Item(itemViewValue);
                float textlength = 0;
                for (Pair<String, Float> pair : lstColumnTextLength) {
                    if (pair.first.equals(header.getInternalName())) {
                        textlength = pair.second;
                        break;
                    }
                }

                if (textlength == 0) {
                    textlength = 3; // trường hợp #
                }

                LinearLayout.LayoutParams _paramsItemRow = new LinearLayout.LayoutParams(Functions.share.convertDpToPixel((textlength * dpPerCharacter) + 25, context),
                        Functions.share.convertDpToPixel(45, context)); // + thêm 2 cái margin Left Right là 20dp

                holderItem.lnContent.setLayoutParams(_paramsItemRow);

                if (position == allRowCount - 1 && isShowSumColumn) {
                    bindingSumData(header, lstJObjectRow, holderItem.tvContent);
                } else {
                    if (!Functions.isNullOrEmpty(header.getInternalName())) {
                        JSONObject json = lstJObjectRow.get(position - 1);
                        bindingValueData(header, json, holderItem.tvContent);
                    } else { // trường hợp #
                        holderItem.tvContent.setText(String.valueOf(position));
                    }
                }

                ((ListGridDataValueHolder) holder).lnContent.addView(holderItem.itemView);
            }

            ((ListGridDataValueHolder) holder).lnContent.setOnClickListener(v -> {
                try {
                    if (isShowSumColumn) {
                        if (position < allRowCount - 1) {
                            int index = 0;
                            for (int i = 0; i < lstJObjectRow.size(); i++) {
                                if (lstJObjectRow.get(i).equals(lstJObjectRow.get(position - 1))) {
                                    index = i;
                                    break;
                                }
                            }

                            if (parentView != null) {
                                Intent intent = new Intent();
                                intent.setAction(VarsReceiver.INNERACTIONCLICK);
                                intent.putExtra("element", new Gson().toJson(element));
                                intent.putExtra("actionId", Vars.ControlInputGridDetails_InnerActionID.Edit);
                                intent.putExtra("positionToAction", index);
                                intent.putExtra("flagViewID", flagView);
                                BroadcastUtility.send(activty, intent);
                            }
                        }
                    } else {
                        int index = 0;
                        for (int i = 0; i < lstJObjectRow.size(); i++) {
                            // Vì item count + 1 nên position - 1
                            if (lstJObjectRow.get(i).equals(lstJObjectRow.get(position - 1))) {
                                index = i;
                                break;
                            }
                        }

                        if (parentView != null) {
                            Intent intent = new Intent();
                            intent.setAction(VarsReceiver.INNERACTIONCLICK);
                            intent.putExtra("element", new Gson().toJson(element));
                            intent.putExtra("actionId", Vars.ControlInputGridDetails_InnerActionID.Edit);
                            intent.putExtra("positionToAction", index);
                            intent.putExtra("flagViewID", flagView);
                            BroadcastUtility.send(activty, intent);
                        }
                    }

                } catch (Exception ex) {
                    Log.d("ERR Convert to List JSON", ex.getMessage());
                }
            });
        }
    }

    private void bindingValueData(WFDetailsHeader header, JSONObject currentJObjectRow, TextView tvContent) {
        try {
            switch (header.getDataType().toLowerCase()) {
                case VarsControl.SELECTUSER:
                case VarsControl.SELECTUSERGROUP: {
                    String data = currentJObjectRow.get(header.getInternalName()).toString();
                    ArrayList<UserAndGroup> beanUserAndGroup = new Gson().fromJson(data, new TypeToken<ArrayList<UserAndGroup>>() {
                    }.getType());
                    if (beanUserAndGroup != null && beanUserAndGroup.size() > 0) {
                        tvContent.setText(beanUserAndGroup.get(0).getName());
                    } else {
                        tvContent.setText("");
                    }
                    break;
                }
                case VarsControl.SELECTUSERMULTI:
                case VarsControl.SELECTUSERGROUPMULTI: {
                    String data = currentJObjectRow.get(header.getInternalName()).toString();
                    ArrayList<String> lstName = new ArrayList<>();
                    ArrayList<UserAndGroup> beanUserAndGroup = new Gson().fromJson(data, new TypeToken<ArrayList<UserAndGroup>>() {
                    }.getType());
                    if (beanUserAndGroup != null && beanUserAndGroup.size() > 0) {
                        for (int i = 0; i < beanUserAndGroup.size(); i++) {
                            lstName.add(beanUserAndGroup.get(i).getName().trim());
                        }

                        tvContent.setText(String.join("; ", lstName));
                    } else {
                        tvContent.setText("");
                    }

                    break;
                }
                case VarsControl.DATE: {
                    String data = currentJObjectRow.get(header.getInternalName()).toString();
                    if (Functions.isNullOrEmpty(data)) {
                        tvContent.setText("");
                    } else {
                        tvContent.setText(Functions.share.formatDayLanguage(data));
                    }
                    break;
                }
                case VarsControl.TIME:
                case VarsControl.DATETIME: {
                    String data = currentJObjectRow.get(header.getInternalName()).toString();
                    if (Functions.isNullOrEmpty(data)) {
                        tvContent.setText("");
                    } else {
                        tvContent.setText(Functions.share.formatDateLanguage(data));
                    }
                    break;
                }
                case VarsControl.SINGLECHOICE:
                case VarsControl.SINGLELOOKUP: {
                    String data = currentJObjectRow.get(header.getInternalName()).toString();
                    ArrayList<LookupData> _lstValue = new Gson().fromJson(data, new TypeToken<ArrayList<LookupData>>() {
                    }.getType());
                    if (_lstValue != null && _lstValue.size() > 0) {
                        tvContent.setText(_lstValue.get(0).getTitle());
                    } else {
                        tvContent.setText("");
                    }
                    break;
                }
                case VarsControl.MULTIPLECHOICE:
                case VarsControl.MULTIPLELOOKUP: {
                    String data = currentJObjectRow.get(header.getInternalName()).toString();
                    String result = "";
                    ArrayList<LookupData> lstObject = new Gson().fromJson(data, new TypeToken<ArrayList<LookupData>>() {
                    }.getType());
                    ArrayList<String> lstValue = new ArrayList<>();

                    if (lstObject != null && lstObject.size() > 0) {
                        for (LookupData item : lstObject) {
                            lstValue.add(item.getTitle());
                        }
                        result = String.join(", ", lstValue);
                    }

                    tvContent.setText(result);
                    break;
                }
                case VarsControl.NUMBER: {
                    ViewElement element = new ViewElement();
                    element.setDataSource(header.getDataSource());
                    if (Functions.isNullOrEmpty(currentJObjectRow.getString(header.getInternalName()))) {
                        element.setValue(currentJObjectRow.getString(header.getInternalName()));
                    } else {
                        BigInteger integer = BigInteger.valueOf(currentJObjectRow.getLong(header.getInternalName()));
                        element.setValue(integer.toString());
                    }

                    if (element.getValue().isEmpty()) {
                        tvContent.setText("");
                    } else {
                        tvContent.setText(DetailFunc.share.getFormatControlDecimal(element));
                    }
                    break;
                }
                case VarsControl.YESNO:
                case VarsControl.TEXTINPUTMULTILINE:
                case VarsControl.TEXTINPUTFORMAT:
                case VarsControl.TEXT:
                default: {
                    tvContent.setText(currentJObjectRow.get(header.getInternalName()).toString());
                    break;
                }
            }
        } catch (Exception ex) {
            Log.d("ERR bindingValueData", ex.getMessage());
        }
    }

    private void bindingSumData(WFDetailsHeader header, ArrayList<JSONObject> lstJObjectRow, TextView tvContent) {
        tvContent.setText("");
        ArrayList<Pair<String, Float>> lstSum = getListSumColumnValue(lstHeader, lstJObjectRow);

        if (!Functions.isNullOrEmpty(header.getInternalName())) {
            float value = 0;
            for (Pair<String, Float> pair : lstSum) {
                if (pair.first.equals(header.getInternalName())) {
                    value = pair.second;
                    break;
                }
            }

            if (value > 0) {
                tvContent.setTypeface(ResourcesCompat.getFont(activty, R.font.fontarial), Typeface.BOLD);
                tvContent.setText(String.valueOf(value));
            }
        }
    }

    @Override
    public int getItemCount() {
        if (isShowSumColumn) {
            allRowCount = (lstJObjectRow.size() + 1) + 1;
        } else {
            allRowCount = lstJObjectRow.size() + 1;
        }
        return allRowCount;
    }

    @Override
    public int getItemViewType(int position) {
        if (position == 0) {
            return 0;
        }
        return 1;
    }

    public class ListGridDataHeaderHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.ln_ItemControlInputGridDetails_Recy_Content)
        LinearLayout lnContent;

        public ListGridDataHeaderHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
        }
    }

    public class ListGridDataValueHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.ln_ItemControlInputGridDetails_Recy_Content)
        LinearLayout lnContent;

        public ListGridDataValueHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
        }
    }

    public class ListGridDataAdapter_ViewHolder_Header_Item extends RecyclerView.ViewHolder {
        @BindView(R.id.ln_ItemControlInputGridDetails_Recy_Header)
        LinearLayout lnContent;
        @BindView(R.id.tv_ItemControlInputGridDetails_Recy_Header)
        TextView tvContent;

        public ListGridDataAdapter_ViewHolder_Header_Item(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
        }
    }

    public class ListGridDataAdapter_ViewHolder_Value_Item extends RecyclerView.ViewHolder {
        @BindView(R.id.ln_ItemControlInputGridDetails_Recy_Value)
        LinearLayout lnContent;
        @BindView(R.id.tv_ItemControlInputGridDetails_Recy_Value)
        TextView tvContent;

        public ListGridDataAdapter_ViewHolder_Value_Item(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
        }
    }
}


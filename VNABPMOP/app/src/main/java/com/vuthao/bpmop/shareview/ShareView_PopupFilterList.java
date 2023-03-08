package com.vuthao.bpmop.shareview;

import android.app.Activity;
import android.util.Log;
import android.view.View;
import android.view.WindowManager;
import android.widget.LinearLayout;
import android.widget.PopupWindow;
import android.widget.TextView;

import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.app.AppStatus;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.ResourceView;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.model.custom.DetailList;
import com.vuthao.bpmop.base.model.custom.LookupData;
import com.vuthao.bpmop.base.model.custom.ObjectFilter;
import com.vuthao.bpmop.base.model.custom.ObjectPropertySearch;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.realm.RealmController;
import com.vuthao.bpmop.base.vars.VarsControl;
import com.vuthao.bpmop.child.FuncChild;
import com.vuthao.bpmop.shareview.adapter.PopupFilterListAdapter;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.stream.Collectors;

import io.realm.Realm;
import io.realm.RealmResults;

public class ShareView_PopupFilterList implements View.OnClickListener {
    private final Activity activity;
    private final View showAs;
    private PopupWindow popupFilter;
    private RecyclerView rvListFilter;
    private LinearLayout lnTopBlur;
    private LinearLayout lnBottomBlur;
    private TextView tvThietLapLai;
    private TextView tvApDung;
    private PopupFilterListListener listener;
    private ViewElement element;
    private ArrayList<ViewElement> elements = new ArrayList<>();
    private View v;
    private PopupFilterListAdapter adapter;
    private HashMap<String, String> hashMap = new HashMap<>();
    private Realm realm;
    private ObjectPropertySearch objectPropertySearch;
    private ArrayList<DetailList.Headers> headers;
    private ResourceView resourceView;

    public interface PopupFilterListListener {
        void OnFilterSuccess(ObjectPropertySearch propertySearch);

        void OnDefaultFilter(ObjectPropertySearch propertySearch);

        void OnFilterErr();

        void OnFilterDismiss();
    }

    public ShareView_PopupFilterList(Activity activity, View showAs, PopupFilterListListener listener) {
        this.activity = activity;
        this.showAs = showAs;
        this.listener = listener;

        realm = new RealmController().getRealm();
    }

    public void filter(ArrayList<DetailList.Headers> headers, ResourceView resourceView, ObjectPropertySearch objectPropertySearch) {
        this.objectPropertySearch = objectPropertySearch;
        this.headers = headers;
        this.resourceView = resourceView;

        v = activity.getLayoutInflater().inflate(R.layout.popup_list_filter, null);
        popupFilter = new PopupWindow(v, WindowManager.LayoutParams.MATCH_PARENT, WindowManager.LayoutParams.MATCH_PARENT);
        rvListFilter = v.findViewById(R.id.rvListFilter);
        lnTopBlur = v.findViewById(R.id.ln_PopupListFilter_TopBlur);
        lnBottomBlur = v.findViewById(R.id.ln_PopupListFilter_BottomBlur);
        tvThietLapLai = v.findViewById(R.id.tv_PopupListFilter_ThietLapLai);
        tvApDung = v.findViewById(R.id.tv_PopupListFilter_ApDung);

        tvThietLapLai.setText(Functions.share.getTitle("TEXT_RESET_FILTER", "Thiết lập lại"));
        tvApDung.setText(Functions.share.getTitle("TEXT_APPLY", "Áp dụng"));

        rvListFilter.setNestedScrollingEnabled(false);
        rvListFilter.setHasFixedSize(true);
        rvListFilter.setItemViewCacheSize(20);
        rvListFilter.setLayoutManager(new LinearLayoutManager(activity, RecyclerView.VERTICAL, false));

        //region show
        popupFilter.setFocusable(true);
        popupFilter.setOutsideTouchable(false);
        popupFilter.showAsDropDown(showAs);
        //endregion

        if (headers != null && headers.size() > 0) {
            headers = (ArrayList<DetailList.Headers>) headers.stream().filter(r ->
                    !r.getInternalName().toLowerCase().equals("filecount")
                            && !r.getInternalName().toLowerCase().equals("isfollow")
                            && !r.getInternalName().toLowerCase().equals("workflowid"))
                    .collect(Collectors.toList());

            if (elements.isEmpty()) {
                for (int i = 0; i < headers.size(); i++) {
                    String internalName = headers.get(i).getInternalName().toLowerCase();

                    DetailList.Headers header = headers.get(i);
                    ViewElement element = new ViewElement();
                    element.setID(i + "filter");
                    element.setRequire(false);
                    element.setEnable(true);
                    element.setInternalName(header.getInternalName());
                    element.setTitle(CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN) ? header.getTitle() : header.getTitleEN());
                    element.setValue("");

                    DetailList.Option option = new DetailList.Option();
                    if (header.getOption() != null) {
                        option = new Gson().fromJson(header.getOption(), DetailList.Option.class);
                    }

                    switch (header.getFieldTypeId()) {
                        case FuncChild.DynamicFieldTypeId.UserGroup: {

                            if (!option.isPeopleOnly() && option.isAllowMultipleValues()) {
                                element.setDataType(VarsControl.SELECTUSERGROUPMULTI);
                            } else {
                                element.setDataType(VarsControl.SELECTUSER);
                            }
                            break;
                        }
                        case FuncChild.DynamicFieldTypeId.DateAndTime: {
                            if (option.isDateOnly()) {
                                element.setDataType(VarsControl.DATE);
                            } else {
                                element.setDataType(VarsControl.DATETIME);
                            }
                            break;
                        }
                        case FuncChild.DynamicFieldTypeId.Lookup: {
                            ArrayList<HashMap<String, String>> props = new ArrayList<>();

                            HashMap<String, String> hashMap = new HashMap<>();
                            hashMap.put("LookupWebId", option.getLookupWebId());
                            props.add(hashMap);

                            hashMap = new HashMap<>();
                            hashMap.put("LookupListId", option.getLookupListId());
                            props.add(hashMap);

                            hashMap = new HashMap<>();
                            hashMap.put("LookupField", option.getLookupField());
                            props.add(hashMap);

                            element.setListProprety(props);
                            element.setDataType(VarsControl.MULTIPLELOOKUP);
                            break;
                        }
                        case FuncChild.DynamicFieldTypeId.ComboBox:
                        case FuncChild.DynamicFieldTypeId.DropDownList:
                        case FuncChild.DynamicFieldTypeId.Radio:
                        case FuncChild.DynamicFieldTypeId.CheckBox:
                        case FuncChild.DynamicFieldTypeId.Choice: {
                            if (internalName.equals("status")) {
                                RealmResults<AppStatus> status = realm.where(AppStatus.class)
                                        .equalTo("IsShow", true)
                                        .findAll();
                                ArrayList<LookupData> datas = new ArrayList<>();
                                for (AppStatus s : status) {
                                    LookupData data = new LookupData();
                                    data.setID(String.valueOf(s.getID()));
                                    data.setTitle(s.getTitle());
                                    datas.add(data);
                                }

                                element.setDataSource(new Gson().toJson(datas));
                            } else if (internalName.equals("choice")) {
                                ArrayList<LookupData> datas = new ArrayList<>();
                                for (String s : option.getChoices()) {
                                    LookupData data = new LookupData();
                                    data.setID(s);
                                    data.setTitle(s);
                                    datas.add(data);
                                }
                                element.setDataSource(new Gson().toJson(datas));
                            }

                            element.setDataType(VarsControl.SINGLECHOICE);
                            break;
                        }
                        case FuncChild.DynamicFieldTypeId.Calculated:
                        case FuncChild.DynamicFieldTypeId.Currency:
                        case FuncChild.DynamicFieldTypeId.Number:
                        case FuncChild.DynamicFieldTypeId.MultipleLinesText:
                        case FuncChild.DynamicFieldTypeId.SingleLineText:
                        default: {
                            element.setDataType(VarsControl.TEXTINPUTMULTILINE);
                            break;
                        }
                    }

                    elements.add(element);
                }
            }

            adapter = new PopupFilterListAdapter(activity, elements);
            rvListFilter.setAdapter(adapter);
        }

        popupFilter.setOnDismissListener(listener::OnFilterDismiss);
        lnTopBlur.setOnClickListener(this);
        lnBottomBlur.setOnClickListener(this);
        tvThietLapLai.setOnClickListener(this);
        tvApDung.setOnClickListener(this);
    }

    public void receiverFormClicks(String str) {
        element = new Gson().fromJson(str, ViewElement.class);
        switch (element.getDataType()) {
            case VarsControl.SELECTUSER: {
                SharedView_PopupControlSelectUserGroup popup = new SharedView_PopupControlSelectUserGroup(activity.getLayoutInflater(), activity, "", null, false);
                popup.initializeValue_Master(element);
                popup.initializeView();
                break;
            }
            case VarsControl.SELECTUSERGROUP: {
                SharedView_PopupControlSelectUserGroup popup = new SharedView_PopupControlSelectUserGroup(activity.getLayoutInflater(), activity, "", null, true);
                popup.initializeValue_Master(element);
                popup.initializeView();
                break;
            }
            case VarsControl.SINGLECHOICE: {
                SharedView_PopupControlSingleChoice popup = new SharedView_PopupControlSingleChoice(activity.getLayoutInflater(), activity, "DetailWorkflow", v);
                popup.initializeValue_Master(element);
                popup.initializeView();
                break;
            }
            case VarsControl.SELECTUSERGROUPMULTI: {
                SharedView_PopupControlSelectUserGroupMulti popup = new SharedView_PopupControlSelectUserGroupMulti(activity.getLayoutInflater(), activity, "DetailWorkflow", v, true);
                popup.initializeValue_Master(element);
                popup.initializeView();
                break;
            }
            case VarsControl.MULTIPLELOOKUP: {
                SharedView_PopupControlMultiLookup popup = new SharedView_PopupControlMultiLookup(activity.getLayoutInflater(), activity, "DetailWorkflow", v);
                popup.initializeValue_Master(element);
                popup.initializeView();
                break;
            }
            case VarsControl.DATE: {
                SharedView_PopupControlDate popup = new SharedView_PopupControlDate(activity.getLayoutInflater(), activity, "DetailWorkflow", v);
                popup.initializeValue_Master(element);
                popup.initializeView();
                break;
            }
            case VarsControl.DATETIME: {
                SharedView_PopupControlDateTime popup = new SharedView_PopupControlDateTime(activity.getLayoutInflater(), activity, "DetailWorkflow", v);
                popup.initializeValue_Master(element);
                popup.initializeView();
                break;
            }
            case VarsControl.TEXTINPUT:
            case VarsControl.TEXTINPUTMULTILINE: {
                SharedView_PopupControlTextInput popup = new SharedView_PopupControlTextInput(activity.getLayoutInflater(), activity, "DetailWorkflow", v);
                popup.initializeValue_Master(element);
                popup.initializeView();
                break;
            }
        }
    }

    public void update(String strElement, String newValue) {
        ViewElement element = new Gson().fromJson(strElement, ViewElement.class);
        for (ViewElement e : elements) {
            if (element.getID().equals(e.getID())) {
                e.setValue(newValue);
                break;
            }
        }

        adapter.notifyDataSetChanged();
    }

    private void apply() {
        ArrayList<ObjectFilter> filters = new Gson().fromJson(objectPropertySearch.getLstProSeach(), new TypeToken<ArrayList<ObjectFilter>>() {
        }.getType());
        ArrayList<ObjectFilter> lstProSearch = new ArrayList<>();
        lstProSearch.add(filters.get(0));

        for (int i = 0; i < elements.size(); i++) {
            if (!Functions.isNullOrEmpty(elements.get(i).getValue())) {
                DetailList.Headers value = headers.get(i);
                String fieldParam = headers.get(i).getInternalName();
                if (!Functions.isNullOrEmpty(value.getFieldMapping())) {
                    fieldParam = value.getFieldMapping();
                }

                DetailList.Option option = new Gson().fromJson(value.getOption(), DetailList.Option.class);

                switch (value.getFieldTypeId()) {
                    case FuncChild.DynamicFieldTypeId.UserGroup: {
                        ArrayList<User> users = new Gson().fromJson(elements.get(i).getValue(), new TypeToken<ArrayList<User>>() {
                        }.getType());

                        List<String> ids = new ArrayList<>();
                        for (User user : users) {
                            ids.add(user.getID().toUpperCase());
                        }

                        String s = String.join(",", ids);

                        ObjectFilter people = generateFilters("text", fieldParam, "in", s, String.valueOf(value.getFieldTypeId()));
                        lstProSearch.add(people);
                        break;
                    }
                    case FuncChild.DynamicFieldTypeId.DateAndTime: {
                        boolean isDate = true;
                        if (option != null) {
                            if (!option.isDateOnly()) {
                                isDate = false;
                            }
                        }

                        String toDate  = Functions.share.getToDay("yyyy-MM-dd");
                        if (!isDate) {
                            toDate = toDate + " 23:59";
                        }

                        ObjectFilter from = generateFilters(isDate ? "date" : "datetime", fieldParam, "gte", elements.get(i).getValue(), String.valueOf(value.getFieldTypeId()));
                        ObjectFilter to = generateFilters(isDate ? "date" : "datetime", fieldParam, "lte", toDate, String.valueOf(value.getFieldTypeId()));

                        lstProSearch.add(from);
                        lstProSearch.add(to);
                        break;
                    }
                    case FuncChild.DynamicFieldTypeId.Lookup:
                    case FuncChild.DynamicFieldTypeId.ComboBox:
                    case FuncChild.DynamicFieldTypeId.DropDownList:
                    case FuncChild.DynamicFieldTypeId.Radio:
                    case FuncChild.DynamicFieldTypeId.CheckBox:
                    case FuncChild.DynamicFieldTypeId.Choice: {
                        ArrayList<LookupData> datas = new Gson().fromJson(elements.get(i).getValue(), new TypeToken<ArrayList<LookupData>>() {
                        }.getType());
                        List<String> keys = new ArrayList<>();
                        for (LookupData data : datas) {
                            if (value.getFieldTypeId() == FuncChild.DynamicFieldTypeId.Lookup) {
                                keys.add(data.getID() + ";#" + data.getTitle());
                            } else {
                                keys.add(data.getID());
                            }
                        }

                        int[] arr = new int[]{13, 6, 3};
                        ObjectFilter filter;
                        if (Arrays.stream(arr).anyMatch(r -> r == value.getFieldTypeId())) {
                            filter = generateFilters("text", fieldParam, "in", String.join(",", keys), "3");
                        } else {
                            filter = generateFilters("text", fieldParam, "in", String.join(",", keys), "");
                        }

                        lstProSearch.add(filter);
                        break;
                    }
                    case FuncChild.DynamicFieldTypeId.Calculated:
                    case FuncChild.DynamicFieldTypeId.Currency:
                    case FuncChild.DynamicFieldTypeId.Number:
                    case FuncChild.DynamicFieldTypeId.MultipleLinesText:
                    case FuncChild.DynamicFieldTypeId.SingleLineText:
                    default: {
                        ObjectFilter text = generateFilters("text", fieldParam, "contains", elements.get(i).getValue(), String.valueOf(value.getFieldTypeId()));
                        lstProSearch.add(text);
                        break;
                    }
                }
            }
        }

        objectPropertySearch.setOffset(0);
        objectPropertySearch.setLimit(Constants.mFilterLimit - 40);
        objectPropertySearch.setTotal(-1);
        objectPropertySearch.setLstProSeach(new Gson().toJson(lstProSearch));

        listener.OnFilterSuccess(objectPropertySearch);
    }

    public void clear() {
        elements.clear();
    }

    public ObjectFilter generateFilters(String contentType, String key, String loginCon, String value, String valueType) {
        ObjectFilter objectFilter = new ObjectFilter();
        objectFilter.setContentType(contentType);
        objectFilter.setKey(key);
        objectFilter.setLogicCon(loginCon);
        objectFilter.setValue(value);
        objectFilter.setValueType(valueType);
        return objectFilter;
    }

    @Override
    public void onClick(View v) {
        switch (v.getId()) {
            case R.id.tv_PopupListFilter_ThietLapLai: {
                elements.clear();

                ArrayList<ObjectFilter> lstProSearch = new ArrayList<>();
                lstProSearch.add(new ObjectFilter("ResourceViewID", "eq", String.valueOf(resourceView.getID()), "", ""));
                objectPropertySearch = new ObjectPropertySearch();
                objectPropertySearch.setLstProSeach(new Gson().toJson(lstProSearch));
                objectPropertySearch.setLimit(Constants.mFilterLimit - 40);
                objectPropertySearch.setOffset(0);
                objectPropertySearch.setTotal(-1);

                listener.OnDefaultFilter(objectPropertySearch);
                popupFilter.dismiss();
                break;
            }
            case R.id.tv_PopupListFilter_ApDung: {
                apply();
                popupFilter.dismiss();
                break;
            }
            case R.id.ln_PopupListFilter_BottomBlur:
            case R.id.ln_PopupListFilter_TopBlur: {
                popupFilter.dismiss();
                break;
            }
        }
    }
}

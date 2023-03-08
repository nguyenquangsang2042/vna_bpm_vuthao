package com.vuthao.bpmop.child;

import android.util.Log;

import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.custom.expression.Function;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.model.app.AppStatus;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.Group;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.model.custom.DetailList;
import com.vuthao.bpmop.base.model.custom.UserAndGroup;
import com.vuthao.bpmop.base.model.custom.WFDetailsHeader;
import com.vuthao.bpmop.base.realm.RealmController;

import org.json.JSONObject;

import io.realm.Realm;

public class FuncChild {
    public static FuncChild share = new FuncChild();
    private final Realm realm = new RealmController().getRealm();

    /// <summary>
    /// Lấy ra dạng dữ liệu thô cho GetFormattedValueByHeader
    /// </summary>
    /// <param name="header">Header cần tìm value</param>
    /// <param name="currentJObjectRow">JObject của item cần tìm</param>
    /// <returns>raw data String theo header tương ứng </returns>
    public String getRawValueByHeader(WFDetailsHeader header, JSONObject currentJObjectRow) {
        String result = "";
        Object object = null;
        try {
            if (!Functions.isNullOrEmpty(header.getFieldMapping())) {
                object = currentJObjectRow.get(header.getFieldMapping());
            } else if (!Functions.isNullOrEmpty(header.getInternalName())) {
                object = currentJObjectRow.get(header.getInternalName());
            }

            if (object != null) {
                result = object.toString();
            }
        } catch (Exception ex) {
            Log.d("ERR getRawValueByHeader", ex.getMessage());
        }

        return result;
    }

    /// <summary>
    /// Lấy ra dạng dữ liệu thô cho GetFormattedValueByHeader
    /// </summary>
    /// <param name="header">Header cần tìm value</param>
    /// <param name="currentJObjectRow">JObject của item cần tìm</param>
    /// <returns>raw data String theo header tương ứng </returns>
    public String getRawValueByHeader(DetailList.Headers header, JSONObject currentJObjectRow) {
        Object object = "";
        try {
            String fieldName = header.getInternalName();
            if (!Functions.isNullOrEmpty(header.getFieldMapping())) {
                fieldName = header.getFieldMapping();
            }

            if (currentJObjectRow.has(fieldName)) {
                object = currentJObjectRow.get(fieldName);
            }
        } catch (Exception ex) {
            Log.d("ERR getRawValueByHeader", ex.getMessage());
        }

        return object.toString();
    }

    public UserAndGroup getUserAndGroup(String ID) {
        UserAndGroup userAndGroup = new UserAndGroup();

        User user = realm.where(User.class)
                .equalTo("ID", ID)
                .findFirst();
        if (user != null) {
            userAndGroup.setID(user.getID());
            userAndGroup.setImagePath(user.getImagePath());
            userAndGroup.setAccountName(user.getAccountName());
            userAndGroup.setName(user.getName());
            userAndGroup.setType("0");
            userAndGroup.setEmail(user.getEmail());
        }

        Group group = realm.where(Group.class)
                .equalTo("ID", ID)
                .findFirst();
        if (group != null) {
            if (user != null) {
                userAndGroup.setID(group.getID());
                userAndGroup.setImagePath(group.getImage());
                userAndGroup.setAccountName(group.getTitle());
                userAndGroup.setName(group.getTitle());
                userAndGroup.setType("1");
                userAndGroup.setEmail(group.getDescription());
            }
        }

        return userAndGroup;
    }

    public String getFormattedValueByHeader(WFDetailsHeader header, JSONObject currentJObjectRow) {
        String result = "";
        String raw = getRawValueByHeader(header, currentJObjectRow);
        switch (header.getFieldTypeId()) {
            case 7: {
                String[] array = raw.toLowerCase().split(",");
                if (array.length > 0) {
                    User user = realm.where(User.class)
                            .equalTo("ID", array[0])
                            .findFirst();
                    if (user != null) {
                        result += user.getFullName();
                    } else {
                        Group group = realm.where(Group.class)
                                .equalTo("ID", array[0])
                                .findFirst();
                        if (group != null) {
                            result += group.getTitle();
                        }
                    }

                    if (array.length > 2) {
                        result += String.format(", +%s", array.length - 1);
                    }
                }
                break;
            }
            case 5: {
                result = Functions.share.formatDateLanguage(raw);
                break;
            }
            default: {
                result = raw;
                break;
            }
        }

        return result;
    }

    public String getFormattedValueByHeader(DetailList.Headers header, JSONObject currentJObjectRow) {
        String result = "";
        String raw = getRawValueByHeader(header, currentJObjectRow);
        switch (header.getFieldTypeId()) {
            case DynamicFieldTypeId.UserGroup: {
                String[] array = raw.toLowerCase().split(",");
                if (array.length > 0) {
                    User user = realm.where(User.class)
                            .equalTo("ID", array[0])
                            .findFirst();
                    if (user != null) {
                        result += user.getFullName();
                    } else {
                        Group group = realm.where(Group.class)
                                .equalTo("ID", array[0])
                                .findFirst();
                        if (group != null) {
                            result += group.getTitle();
                        }
                    }

                    if (array.length > 2) {
                        result += String.format(", +%s", array.length - 1);
                    }
                }
                break;
            }
            case DynamicFieldTypeId.DateAndTime: {
                if (raw.equals("")) {
                    result = raw;
                    break;
                }

                result = Functions.share.formatDateLanguage(raw);
                break;
            }
            case DynamicFieldTypeId.CheckBox:
            case DynamicFieldTypeId.Choice:
            case DynamicFieldTypeId.ComboBox:
            case DynamicFieldTypeId.Lookup: {
                if (header.getInternalName().toLowerCase().equals("status") && !Functions.isNullOrEmpty(raw)) {
                    AppStatus status = new RealmController().getRealm().where(AppStatus.class)
                            .equalTo("ID", Integer.parseInt(raw))
                            .findFirst();
                    if (status != null) {
                        if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
                            result = status.getTitle();
                        } else {
                            result = status.getTitleEN();
                        }
                    }
                } else {
                    result = raw;
                }
                break;
            }
            default: {
                result = raw;
                break;
            }
        }

        return result;
    }

    public static class DynamicFieldTypeId {
        public final static int SingleLineText = 1;
        public final static int MultipleLinesText = 2;
        public final static int Choice = 3;
        public final static int Number = 4;
        public final static int DateAndTime = 5;
        public final static int CheckBox = 6;
        public final static int UserGroup = 7;
        public final static int Currency = 8;
        public final static int Calculated = 9;
        public final static int Radio = 10;
        public final static int DropDownList = 11;
        public final static int ComboBox = 12;
        public final static int Lookup = 13;
    }
}

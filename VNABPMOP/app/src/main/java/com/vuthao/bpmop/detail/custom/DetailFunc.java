package com.vuthao.bpmop.detail.custom;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.graphics.Color;
import android.net.Uri;
import android.os.Build;
import android.os.ParcelFileDescriptor;
import android.text.Html;
import android.text.SpannableString;
import android.text.Spanned;
import android.text.style.BackgroundColorSpan;
import android.util.Log;
import android.view.View;
import android.widget.EditText;

import androidx.core.content.ContextCompat;
import androidx.core.content.FileProvider;
import androidx.recyclerview.widget.RecyclerView;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.DateTimeUtility;
import com.vuthao.bpmop.base.FileUtils;
import com.vuthao.bpmop.base.Formula;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.model.custom.LookupData;
import com.vuthao.bpmop.base.vars.Variable;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.Settings;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.model.custom.AttachFile;
import com.vuthao.bpmop.base.model.custom.ExpandTask;
import com.vuthao.bpmop.base.model.custom.ObjectSubmitDetailComment;
import com.vuthao.bpmop.base.model.custom.Task;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.model.dynamic.ViewRow;
import com.vuthao.bpmop.base.model.dynamic.ViewSection;
import com.vuthao.bpmop.base.realm.RealmController;
import com.vuthao.bpmop.core.controller.ControllerBase;

import org.json.JSONObject;

import java.io.File;
import java.lang.reflect.Field;
import java.text.DecimalFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Comparator;
import java.util.stream.Collectors;

import okhttp3.MediaType;
import okhttp3.RequestBody;

public class DetailFunc {
    public static DetailFunc share = new DetailFunc();

    public int getColorByActionIDProcess(Context context, int submitAction) {
        switch (submitAction) {
            case 12: // Gửi
            case 1: // Đồng ý
            case 53: // Thực hiện điều chỉnh
            case 7: // Yêu cầu bổ sung
            case 9: // Yêu cầu tham vấn
            case 10: // Thực hiện tham vấn
            case 3: // Chuyển xử lý
            case 6: // Thu hồi
                return ContextCompat.getColor(context, R.color.clDetailProcessBlue);
            case 2: // Phê duyệt
                return ContextCompat.getColor(context, R.color.clDetailProcessGreen);
            case 4: // Yêu cầu hiệu chỉnh
                return ContextCompat.getColor(context, R.color.clDetailProcessPink);
            case 51: // Hủy
            case 5: // Từ chối
                return ContextCompat.getColor(context, R.color.clDetailProcessRed);
            case 47: // Chờ xử lý
            case 48: // Chờ xử lý
            case 49: // Chờ xử lý
            case 50: // Chờ xử lý
                return ContextCompat.getColor(context, R.color.clDetailProcessYellow);
            default:
                return ContextCompat.getColor(context, R.color.clDetailProcessBlue);
        }
    }

    public int getColorByActionID(Context context, int actionId) {
        switch (actionId) {
            case -1: // hủy
            case 4: // từ chối
            case 6: // Từ chối
                return ContextCompat.getColor(context, R.color.clStatusRed);
            case 10: // Đã phê duyệt
                return ContextCompat.getColor(context, R.color.clStatusGreen);
            case 1: // Chờ phê duyệt
            case 2: // Chờ phê duyệt
            case 3: // Chờ phê duyệt
            case 5: // Chờ phê duyệt
                return ContextCompat.getColor(context, R.color.clStatusBlue);
            case 0: // Đang lưu
                return ContextCompat.getColor(context, R.color.clStatusGray);
            default:
                return ContextCompat.getColor(context, R.color.clStatusBlue);
        }
    }

    public ArrayList<String> getNameFromLookupData(String data) {
        ArrayList<String> result = new ArrayList<>();
        if (Functions.isNullOrEmpty(data)) return result;
        if (data.contains(";#")) {
            String[] arr = data.split(";#");
            for (int i = 0; i < arr.length - 1; i += 2) {
                result.add(arr[i + 1]);
            }
        } else {
            try {
                ArrayList<LookupData> items = new Gson().fromJson(data, new TypeToken<ArrayList<LookupData>>() {
            }.getType());
                for (LookupData item : items) {
                    result.add(item.getTitle());
                }
            } catch (Exception ex) {
                Log.d("ERR getNameFromLookupData", ex.toString());
                result.add(data);
                return result;
            }
        }

        return result;
    }

    public String getFormatControlDecimal(ViewElement element) {
        String result = "";
        double customValue = 0;
        if (!Functions.isNullOrEmpty(element.getValue())) {
            customValue = Double.parseDouble(element.getValue().trim());
        }

        int demicalCount = 0;
        String numberSigns = "";

        // _element.DataSource trả ra là số chữ số decimal
        if (!Functions.isNullOrEmpty(element.getDataSource())) {
            demicalCount = Integer.parseInt(element.getDataSource());
            if (demicalCount > 0) {
                numberSigns = generateNumberSigns(demicalCount);
                DecimalFormat fmt = new DecimalFormat("#," + numberSigns);
                result = fmt.format(customValue);
            } else {
                DecimalFormat fmt = new DecimalFormat("#,###");
                result = fmt.format(customValue);
            }
        } else {
            DecimalFormat fmt = new DecimalFormat("#,###");
            result = fmt.format(customValue);
        }

        return result.replaceAll("\\.", ","); // Some devices return like 1.000 => 1,000
    }

    private String generateNumberSigns(int n) {
        StringBuilder s = new StringBuilder();
        for (int i = 0; i < n; i++) {
            s.append("#");
        }
        return s.toString();
    }

    public String formatHTMLToString(String html) {
        String result = "";
        result = Html.fromHtml(html.replace("\n", "<br/>"), Html.FROM_HTML_MODE_LEGACY).toString();
        return result;
    }

    public ArrayList<ViewElement> sortListElementAction(ArrayList<ViewElement> lstElementAction) {
        ArrayList<ViewElement> result = new ArrayList<>();

        int[] rule = new int[]{
                Variable.WorkflowAction.Save,
                Variable.WorkflowAction.Next,
                Variable.WorkflowAction.Approve,
                Variable.WorkflowAction.Return,
                Variable.WorkflowAction.CreateTask,
                Variable.WorkflowAction.Forward,
                Variable.WorkflowAction.RequestInformation,
                Variable.WorkflowAction.RequestIdea,
                Variable.WorkflowAction.Cancel,
                Variable.WorkflowAction.Return,
                Variable.WorkflowAction.Reject,
        };

        for (int item : rule) {
            result.addAll(lstElementAction.stream().filter(r -> r.getID().equals(String.valueOf(item))).collect(Collectors.toList()));
            lstElementAction.removeIf(r -> r.getID().equals(String.valueOf(item)));
        }

        // Add thêm những Action không có trong list Rule
        if (lstElementAction.size() > 0) {
            result.addAll(lstElementAction);
        }

        return result;
    }

    public String getFormatControlDecimal(double s, ViewElement element) {
        String result = "";

        int demicalCount = 0;
        String numberSigns = "";

        // element.DataSource trả ra là số chữ số decimal
        if (!Functions.isNullOrEmpty(element.getDataSource())) {
            demicalCount = Integer.parseInt(element.getDataSource());
            if (demicalCount > 0) {
                numberSigns = generateNumberSigns(demicalCount);
                DecimalFormat fmt = new DecimalFormat("#," + numberSigns);
                result = fmt.format(s);
            } else {
                DecimalFormat fmt = new DecimalFormat("#,###");
                result = fmt.format(s);
            }
        } else {
            DecimalFormat fmt = new DecimalFormat("#,###");
            result = fmt.format(s);
        }

        return result.replaceAll("\\.", ","); // Some devices return like 1.000 => 1,000
    }

    public String formatDateTimeControl(String value, String type, String format) {

        String result = "";
        long miliseconds = formatStringToLong(value, format);

        if (type.equals("date")) {
            if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
                result = Functions.share.formatLongToDay(miliseconds, "dd/MM/yy");
            } else {
                result = Functions.share.formatLongToDay(miliseconds, "MM/dd/yy");
            }
        } else if (type.equals("datetime")) {
            if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
                result = Functions.share.formatLongToDay(miliseconds, "dd/MM/yy HH:mm");
            } else {
                result = Functions.share.formatLongToDay(miliseconds, "MM/dd/yy HH:mm");
            }
        } else {
            result = DateTimeUtility.formatFullTime(miliseconds);
        }

        return result;
    }

    public String formatDateTimePopup(String value) {
        String result = "";
        long miliseconds = formatStringToLong(value);

        if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
            result = Functions.share.formatLongToDay(miliseconds, "dd/MM/yyyy");
        } else {
            result = Functions.share.formatLongToDay(miliseconds, "MM/dd/yyyy");
        }

        return result;
    }

    public String formatDateTimeWhenSelectedPopup(long miliseconds, String format) {
        //"yyyy-MM-dd HH:mm:ss"
        String result = "";
        if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
            result = Functions.share.formatLongToDay(miliseconds, format);
        } else {
            result = Functions.share.formatLongToDay(miliseconds, format);
        }

        return result;
    }

    public long formatStringToLong(String date, String format) {
        SimpleDateFormat sdf = new SimpleDateFormat(format);
        try {
            java.util.Date d = sdf.parse(date);
            long l = d.getTime();
            return l;
        } catch (Exception e) {
            return 0;
        }
    }

    public long formatStringToLong(String date) {
        SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
        try {
            java.util.Date d = sdf.parse(date);
            long l = d.getTime();
            return l;
        } catch (Exception e) {
            return 0;
        }
    }

    public AttachFile getAttachFileFromURI(Activity activity, Uri uri) {
        AttachFile attachFile = new AttachFile();
        FileUtils fileUtils = new FileUtils(activity.getApplicationContext());
        try {
            String path = fileUtils.getPath(uri);
            attachFile.setID("");
            attachFile.setTitle(fileUtils.getFileName(uri) + ";#" + Functions.share.getToDay("dd/MM/yy HH:mm"));
            attachFile.setPath(path);
            attachFile.setCreatedBy(CurrentUser.getInstance().getUser().getID());
            attachFile.setAuthor(true);
            attachFile.setCreatedName(CurrentUser.getInstance().getUser().getFullName());
            attachFile.setCreatedPositon(CurrentUser.getInstance().getUser().getPositionTitle());
            attachFile.setAttachTypeId(0);
            attachFile.setAttachTypeName("");

            ParcelFileDescriptor fd = activity.getContentResolver().openFileDescriptor(uri, "r");
            attachFile.setSize(fd.getStatSize());
            fd.close();
        } catch (Exception ex) {
            Log.d("ERR getAttachFileFromURI", ex.getMessage());
        }

        return attachFile;
    }

    public AttachFile getAttachFileFromURICamera(Activity activity, Uri uri) {
        AttachFile attachFile = new AttachFile();
        FileUtils fileUtils = new FileUtils(activity.getApplicationContext());
        try {
            String path = fileUtils.getPath(uri);
            attachFile.setID("");
            attachFile.setTitle(fileUtils.getFileName(uri) + ";#" + Functions.share.getToDay("dd/MM/yy HH:mm"));
            attachFile.setPath(path);
            attachFile.setCreatedBy(CurrentUser.getInstance().getUser().getID());
            attachFile.setCreatedName(CurrentUser.getInstance().getUser().getFullName());
            attachFile.setCreatedPositon(CurrentUser.getInstance().getUser().getPositionTitle());
            attachFile.setAttachTypeId(0);
            attachFile.setAttachTypeName("");
            ParcelFileDescriptor fd = activity.getContentResolver().openFileDescriptor(uri, "r");
            attachFile.setSize(fd.getStatSize());
            fd.close();
        } catch (Exception ex) {
            Log.d("ERR getAttachFileFromURICamera", ex.getMessage());
        }

        return attachFile;
    }

    public boolean checkFileExits(ArrayList<AttachFile> files, AttachFile file) {
        boolean result = false;
        for (AttachFile item : files) {
            if (item.getPath().equals(file.getPath())) {
                result = true;
                return result;
            }
        }
        return result;
    }

    public ArrayList<ViewSection> updateValueElement_InListSection(ArrayList<ViewSection> lstSections, ViewElement _element, boolean reCalculated) {
        try {
            // JObject những Element ko phải calculated
            JSONObject JObjectSource = new JSONObject();

            for (ViewSection section : lstSections) {
                for (ViewRow row : section.getViewRows()) {
                    for (ViewElement element : row.getElements()) {
                        if (element.getID().equals(_element.getID())) {
                            element.setValue(_element.getValue());
                        }

                        if (reCalculated) {
                            if (element.getDataType().equals("number")) {
                                JObjectSource.put(Functions.removeAccent(element.getTitle().toLowerCase()).replace(" ", ""), element.getValue());
                            }
                        }
                    }
                }
            }

            if (reCalculated) {
                for (ViewSection section : lstSections) {
                    for (ViewRow row : section.getViewRows()) {
                        for (ViewElement element : row.getElements()) {
                            if (!Functions.isNullOrEmpty(element.getFormula())) {
                                String value = Formula.evaluate(Functions.removeAccent(element.getFormula().toLowerCase()).replace(" ", ""), JObjectSource);
                                element.setValue(value);
                            }
                        }
                    }
                }
            }
        } catch (Exception ex) {
            Log.d("ERR updateValueElement_InListSection", ex.getMessage());
        }

        return lstSections;
    }

    public int findIndexOfItemInListAttach(AttachFile searchItem, ArrayList<AttachFile> lstAttach) {
        int result = -1;
        for (int i = 0; i < lstAttach.size(); i++) {
            if (lstAttach.get(i).getID().equals(searchItem.getID()) &&
                    lstAttach.get(i).getPath().equals(searchItem.getPath()) &&
                    lstAttach.get(i).getTitle().equals(searchItem.getTitle()) &&
                    lstAttach.get(i).isAuthor() == searchItem.isAuthor()) {
                result = i;
            }
        }

        return result;
    }

    public int getColorByAppStatus(int id) {
        return Functions.share.getColorByAppStatus(id);
    }

    public ArrayList<ExpandTask> cloneListExpandTask(ArrayList<Task> lstTask) {
        ArrayList<ExpandTask> result = new ArrayList<>();
        lstTask.sort((o1, o2) -> o1.getID() > o2.getID() ? 1 : -1);

        ArrayList<Task> lstParent = (ArrayList<Task>) lstTask.stream().filter(r -> r.getParent() == 0).collect(Collectors.toList());
        if (lstParent.size() > 0) {
            // từ cấp parent
            for (Task parentItem : lstParent) {
                ExpandTask itemLV1 = new ExpandTask();
                itemLV1.setGroupItem(parentItem);
                itemLV1.setLstChild(new ArrayList<>());
                ArrayList<Task> lstchildLV1 = (ArrayList<Task>) lstTask.stream().filter(r -> r.getParent() == parentItem.getID()).collect(Collectors.toList());
                for (Task childLv1Item : lstchildLV1) {
                    ExpandTask itemLV2 = new ExpandTask();
                    itemLV2.setGroupItem(childLv1Item);
                    itemLV2.setLstChild(new ArrayList<>());
                    // Xử lý Level 3
                    handleExpandLV3(lstTask, itemLV2.getLstChild(), childLv1Item);
                    itemLV1.getLstChild().add(itemLV2);
                }
                result.add(itemLV1);
            }
        } else {
            // xử lý từ cấp con
            for (Task parentItem : lstTask) {
                ExpandTask itemLV1 = new ExpandTask();
                itemLV1.setGroupItem(parentItem);
                itemLV1.setLstChild(new ArrayList<>());
                ArrayList<Task> lstchildLV1 = (ArrayList<Task>) lstTask.stream().filter(r -> r.getParent() == parentItem.getID()).collect(Collectors.toList());
                for (Task childLv1Item : lstchildLV1) {
                    ExpandTask itemLV2 = new ExpandTask();
                    itemLV2.setGroupItem(childLv1Item);
                    itemLV2.setLstChild(new ArrayList<>());
                    handleExpandLV3(lstTask, itemLV2.getLstChild(), childLv1Item);
                    itemLV1.getLstChild().add(itemLV2);
                }

                result.add(itemLV1);
            }
        }
        return result;
    }

    private void handleExpandLV3(ArrayList<Task> lstTask, ArrayList<ExpandTask> result, Task parentItem) {
        ArrayList<Task> lstchild = (ArrayList<Task>) lstTask.stream().filter(r -> r.getParent() == parentItem.getID()).collect(Collectors.toList());
        for (Task parent : lstchild) {
            ExpandTask itemChild = new ExpandTask();
            itemChild.setGroupItem(parent);
            itemChild.setLstChild(new ArrayList<>());
            result.add(itemChild);
            handleExpandLV3(lstTask, result, parent);
        }
    }

    public String getStatusNameByID(int status) {
        switch (status) {
            case 4:
                return Functions.share.getTitle("TEXT_CANCEL", "Hủy");
            case 2:
                return Functions.share.getTitle("TEXT_COMPLETED", "Hoàn tất");
            case 3:
                return Functions.share.getTitle("TEXT_HOLD", "Tạm hoãn");
            case 1:
                return Functions.share.getTitle("TEXT_INPROGRESS", "Đang thực hiện");
            default:
                return Functions.share.getTitle("TEXT_NOPROCESS", "Chưa thực hiện");
        }
    }

    public int getStatusColorByID(Context context, int status) {
        switch (status) {
            case 4:
                return ContextCompat.getColor(context, R.color.clTaskStatusRed);
            case 2:
                return ContextCompat.getColor(context, R.color.clTaskStatusGreen);
            case 3:
                return ContextCompat.getColor(context, R.color.clTaskStatusRed);
            case 1:
                return ContextCompat.getColor(context, R.color.clTaskStatusBlue);
            default:
                return ContextCompat.getColor(context, R.color.clTaskStatusGray);
        }
    }

    public ArrayList<AttachFile> classifyListAttachFile(ArrayList<AttachFile> lstAttachFile) {
        if (lstAttachFile != null && lstAttachFile.size() > 0) {
            String[] lstExtension = new String[]{".png", ".jpeg", ".jpg"};
            for (int i = 0; i < lstAttachFile.size(); i++) {
                String path = (!Functions.isNullOrEmpty(lstAttachFile.get(i).getPath()) ? lstAttachFile.get(i).getPath() : "").toLowerCase();
                for (String ext : lstExtension) {
                    if (path.contains(ext)) {
                        lstAttachFile.get(i).setImage(true);
                        break;
                    }
                }

                // Nếu Path chưa có -> check URL
                String url = (!Functions.isNullOrEmpty(lstAttachFile.get(i).getUrl()) ? lstAttachFile.get(i).getUrl() : "").toLowerCase();
                for (String ext : lstExtension) {
                    if (url.contains(ext)) {
                        lstAttachFile.get(i).setImage(true);
                        break;
                    }
                }
            }
        }

        return lstAttachFile;
    }

    public int getResourceIDAttachment(String path) {
        return new ControllerBase().getResourceIDAttachment(path);
    }

    public String getFormatTitleFile(String title) {
        String[] arr = title.split(";#");
        if (arr.length > 1) {
            return arr[0];
        } else {
            return title;
        }
    }

    public int getRecyclerViewHeight(RecyclerView recy) {
        int height = recy.getPaddingTop() + recy.getPaddingBottom();
        if (recy.getAdapter().getItemCount() > 0) {
            RecyclerView.ViewHolder holder = recy.findViewHolderForAdapterPosition(0);

            if (holder != null) {
                holder.itemView.measure(View.MeasureSpec.UNSPECIFIED, View.MeasureSpec.UNSPECIFIED);
                height += holder.itemView.getMeasuredHeight() * recy.getAdapter().getItemCount();
            }
        }
        return height;
    }

    public ObjectSubmitDetailComment initTrackingObjectSubmitDetail() {
        ObjectSubmitDetailComment objSubmitDetailComment = new ObjectSubmitDetailComment();
        objSubmitDetailComment.setDeviceName("App Android");
        objSubmitDetailComment.setCodeName("AppAndroid");
        objSubmitDetailComment.setAppName("BPM OP");
        objSubmitDetailComment.setPlatform("Android API " + Build.VERSION.SDK_INT);
        return objSubmitDetailComment;
    }

    public String getURLSettingComment(int ResourceSubCategoryId) {
        String result = "";
        RealmController controller = new RealmController();
        switch (ResourceSubCategoryId) {
            case 8: {
                Settings settings = controller.getRealm().where(Settings.class)
                        .equalTo("KEY", "WORKFLOW_URL")
                        .findFirst();
                if (settings != null) {
                    result = settings.getVALUE().replace("{0}", "%s");
                } else {
                    result = "/workflow/SitePages/Workflow.aspx?ItemId=%s";
                }
                break;
            }
            case 16: {
                Settings settings = controller.getRealm().where(Settings.class)
                        .equalTo("KEY", "TASK_URL")
                        .findFirst();
                if (settings != null) {
                    result = settings.getVALUE().replace("{0}", "%s");
                } else {
                    result = "/workflow/SitePages/InsertOrUpdateTask.aspx?TID=%s";
                }
                break;
            }
        }
        return result;
    }

    public String formatDateComment(String date) {
        long l = Functions.share.formatStringToLongApi(date);
        return DateTimeUtility.format(l, "yyyy-MM-dd HH:mm:ss");
    }

    public void hightLightTextSpecific(EditText editText, String mainString, String subString, String color) {
        if (mainString.contains(subString)) {
            int startIndex = mainString.indexOf(subString);
            int endIndex = startIndex + subString.length();

            SpannableString spannableString = new SpannableString(mainString);
            spannableString.setSpan(new BackgroundColorSpan(Color.parseColor(color)), startIndex, endIndex, Spanned.SPAN_EXCLUSIVE_EXCLUSIVE);
            editText.setText(spannableString, null);
        }
    }

    public void openFile(Activity activity, String filename) {
        try {
            File file = new File(filename);

            // Get URI and MIME type of file
            Uri uri = FileProvider.getUriForFile(activity, activity.getPackageName() + ".provider", file);
            String mime = activity.getContentResolver().getType(uri);

            // Open file with user selected app
            Intent intent = new Intent();
            intent.setAction(Intent.ACTION_VIEW);
            intent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_WHEN_TASK_RESET | Intent.FLAG_ACTIVITY_NEW_TASK);
            intent.addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);
            intent.setDataAndType(uri, mime);
            activity.startActivity(intent);
        } catch (Exception ex) {
            Utility.share.showAlertWithOnlyOK("Bạn không có ứng dụng có thể mở loại tệp này.", Functions.share.getTitle("TEXT_CLOSE", "Close"), activity);
            Log.d("ERR Open File", ex.getMessage());
        }
    }

    public boolean validateRequiredForm(ArrayList<ViewSection> LISTSECTION) {
        boolean result = true;
        for (ViewSection section : LISTSECTION) {
            for (ViewRow row : section.getViewRows()) {
                ArrayList<ViewElement> lstElement = (ArrayList<ViewElement>) row.getElements().stream().filter(r -> r.isEnable() && r.isRequire() && Functions.isNullOrEmpty(r.getValue())).collect(Collectors.toList());
                if (lstElement.size() > 0) {
                    result = false;
                }
            }
        }
        return result;
    }

    /// <summary>
    /// Check xem có beanUser selected chưa, chưa có -> hiện AlertDialog + return false
    /// </summary>
    /// <param name="_edtComment"></param>
    public boolean checkActionHasSelectedUser(User selectedUser) {
        return selectedUser != null && !Functions.isNullOrEmpty(selectedUser.getID());
    }

    public String roundnDecimals(int digits, double d) {
        String s = generateNumberSigns(digits);
        DecimalFormat twoDForm = new DecimalFormat("#." + s);
        return String.valueOf(Double.valueOf(twoDForm.format(d)));
    }

    public RequestBody toRequestBody(String value) {
        return RequestBody.create(MediaType.parse("text/plain"), value);
    }
}

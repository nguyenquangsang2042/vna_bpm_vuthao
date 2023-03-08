package com.vuthao.bpmop.base;

import android.app.Activity;
import android.content.Context;
import android.content.res.ColorStateList;
import android.content.res.Resources;
import android.graphics.Bitmap;
import android.graphics.Color;
import android.graphics.Typeface;
import android.net.Uri;
import android.os.Build;
import android.os.Vibrator;
import android.text.Html;
import android.text.Layout;
import android.text.Spannable;
import android.text.SpannableString;
import android.text.Spanned;
import android.text.TextUtils;
import android.text.style.TextAppearanceSpan;
import android.util.Base64;
import android.util.DisplayMetrics;
import android.util.Log;
import android.view.View;
import android.view.WindowManager;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import androidx.core.content.ContextCompat;
import androidx.core.content.res.ResourcesCompat;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.model.app.AppLanguage;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.Group;
import com.vuthao.bpmop.base.model.app.Settings;
import com.vuthao.bpmop.base.model.app.TimeLanguage;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.realm.RealmHelper;
import com.vuthao.bpmop.base.realm.TimeAppController;

import java.io.File;
import java.io.FileOutputStream;
import java.io.OutputStream;
import java.nio.charset.StandardCharsets;
import java.text.DateFormat;
import java.text.DecimalFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Calendar;
import java.util.Date;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class Functions {
    // Mang cac ky tu goc co dau
    private static final char[] SOURCE_CHARACTERS = {'À', 'Á', 'Â', 'Ã', 'È', 'É',
            'Ê', 'Ì', 'Í', 'Ò', 'Ó', 'Ô', 'Õ', 'Ù', 'Ú', 'Ý', 'à', 'á', 'â',
            'ã', 'è', 'é', 'ê', 'ì', 'í', 'ò', 'ó', 'ô', 'õ', 'ù', 'ú', 'ý',
            'Ă', 'ă', 'Đ', 'đ', 'Ĩ', 'ĩ', 'Ũ', 'ũ', 'Ơ', 'ơ', 'Ư', 'ư', 'Ạ',
            'ạ', 'Ả', 'ả', 'Ấ', 'ấ', 'Ầ', 'ầ', 'Ẩ', 'ẩ', 'Ẫ', 'ẫ', 'Ậ', 'ậ',
            'Ắ', 'ắ', 'Ằ', 'ằ', 'Ẳ', 'ẳ', 'Ẵ', 'ẵ', 'Ặ', 'ặ', 'Ẹ', 'ẹ', 'Ẻ',
            'ẻ', 'Ẽ', 'ẽ', 'Ế', 'ế', 'Ề', 'ề', 'Ể', 'ể', 'Ễ', 'ễ', 'Ệ', 'ệ',
            'Ỉ', 'ỉ', 'Ị', 'ị', 'Ọ', 'ọ', 'Ỏ', 'ỏ', 'Ố', 'ố', 'Ồ', 'ồ', 'Ổ',
            'ổ', 'Ỗ', 'ỗ', 'Ộ', 'ộ', 'Ớ', 'ớ', 'Ờ', 'ờ', 'Ở', 'ở', 'Ỡ', 'ỡ',
            'Ợ', 'ợ', 'Ụ', 'ụ', 'Ủ', 'ủ', 'Ứ', 'ứ', 'Ừ', 'ừ', 'Ử', 'ử', 'Ữ',
            'ữ', 'Ự', 'ự'};

    // Mang cac ky tu thay the khong dau
    private static final char[] DESTINATION_CHARACTERS = {'A', 'A', 'A', 'A', 'E',
            'E', 'E', 'I', 'I', 'O', 'O', 'O', 'O', 'U', 'U', 'Y', 'a', 'a',
            'a', 'a', 'e', 'e', 'e', 'i', 'i', 'o', 'o', 'o', 'o', 'u', 'u',
            'y', 'A', 'a', 'D', 'd', 'I', 'i', 'U', 'u', 'O', 'o', 'U', 'u',
            'A', 'a', 'A', 'a', 'A', 'a', 'A', 'a', 'A', 'a', 'A', 'a', 'A',
            'a', 'A', 'a', 'A', 'a', 'A', 'a', 'A', 'a', 'A', 'a', 'E', 'e',
            'E', 'e', 'E', 'e', 'E', 'e', 'E', 'e', 'E', 'e', 'E', 'e', 'E',
            'e', 'I', 'i', 'I', 'i', 'O', 'o', 'O', 'o', 'O', 'o', 'O', 'o',
            'O', 'o', 'O', 'o', 'O', 'o', 'O', 'o', 'O', 'o', 'O', 'o', 'O',
            'o', 'O', 'o', 'U', 'u', 'U', 'u', 'U', 'u', 'U', 'u', 'U', 'u',
            'U', 'u', 'U', 'u'};

    private static final String[] TEXT_CASE = {"A", "Ă", "Â", "B", "C", "D", "Đ", "E", "Ê", "G", "H", "I", "K", "L", "M", "N", "O", "Ô", "Ơ", "P", "Q", "R", "S", "T", "U", "Ư", "V", "X", "Y"};

    private static final int[] NUM = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};

    public static Functions share = new Functions();

    public void hideNavBar(Activity activity) {
        if (Build.VERSION.SDK_INT < 16) {
            activity.getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN,
                    WindowManager.LayoutParams.FLAG_FULLSCREEN);
        }
        else {
            View decorView = activity.getWindow().getDecorView();
            // Hide Status Bar.
            int uiOptions = View.SYSTEM_UI_FLAG_FULLSCREEN;
            decorView.setSystemUiVisibility(uiOptions);
        }
    }

    public void showNavBar(Activity activity) {
        if (Build.VERSION.SDK_INT < 16) {
            activity.getWindow().clearFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN);
        }
        else {
            View decorView = activity.getWindow().getDecorView();
            // Show Status Bar.
            int uiOptions = View.SYSTEM_UI_FLAG_VISIBLE;
            decorView.setSystemUiVisibility(uiOptions);
        }
    }

    /* Get current width of screen */
    public int getScreenWidth() {
        return Resources.getSystem().getDisplayMetrics().widthPixels;
    }

    /* Get current Height of screen */
    public int getScreenHeight() {
        return Resources.getSystem().getDisplayMetrics().heightPixels;
    }

    public String formatNumberMoney(double s) {
        DecimalFormat format = new DecimalFormat("###,###,###");
        try {
            format.format(s);
            return format.format(s).replaceAll("\\.", ","); // Some devices return like 1.000 => 1,000
        } catch (Exception e) {
            return "";
        }
    }

    public int convertDpToPixel(float dp, Context context) {
        return (int)(dp * ((float) context.getResources().getDisplayMetrics().densityDpi / (float) DisplayMetrics.DENSITY_DEFAULT));
    }

    public String formatMoneyVND(double s) {
        DecimalFormat format = new DecimalFormat("###,###,###");
        try {
            format.format(s);
            return (format.format(s) + " VND").replaceAll("\\.", ","); // Some devices return like 1.000 => 1,000
        } catch (Exception e) {
            return "";
        }
    }

    public String getToDay(String format, int addDays) {
        DateFormat dateFormat = new SimpleDateFormat(format.isEmpty() ? "dd/MM/yyyy" : format);
        Calendar cal = Calendar.getInstance();
        cal.add(Calendar.DATE, addDays);
        return dateFormat.format(cal.getTime());
    }

    public String getToDay(int addDays) {
        DateFormat dateFormat = new SimpleDateFormat("dd/MM/yyyy");
        Calendar cal = Calendar.getInstance();
        cal.add(Calendar.DATE, addDays);
        return dateFormat.format(cal.getTime());
    }

    public Date formatStringToDate(String dateStr) {
        SimpleDateFormat sdf = new SimpleDateFormat(Constants.mDateApi);
        Date date = null;
        try {
            date = sdf.parse(dateStr);
        } catch (Exception ex) {

        }
        return date;
    }

    public Date formatStringToDate(String dateStr, String format) {
        SimpleDateFormat sdf = new SimpleDateFormat(format);
        Date date = null;
        try {
            date = sdf.parse(dateStr);
        } catch (Exception ex) {

        }
        return date;
    }

    public static boolean isNullOrEmpty(String s) {
        return s == null || s.length() == 0 || s.trim().toLowerCase().equals("null");
    }

    public String getTimeCreate() {
        DateFormat df = new SimpleDateFormat("dd/MM/yyyy hh:mm:ss a");
        java.util.Date date = new java.util.Date();
        String dateString = "";
        try {
            dateString = df.format(date);
            return dateString;
        } catch (Exception e) {
            return "";
        }
    }

    public String formatMoneyUSD(double s) {
        DecimalFormat format = new DecimalFormat("###,###,###");
        try {
            format.format(s);
            return ("$ " + format.format(s)).replaceAll("\\.", ","); // Some devices return like 1.000 => 1,000
        } catch (Exception e) {
            return "";
        }
    }

    public String formatLongToString(long l) {
        DateFormat df = new SimpleDateFormat("dd/MM - HH:mm");
        String dateString = "";
        try {
            Date d = new Date(l);
            dateString = df.format(d);
            return dateString;
        } catch (Exception e) {
            return "";
        }
    }

    public String formatLongToString(long l, String format) {
        DateFormat df = new SimpleDateFormat(format);
        String dateString = "";
        try {
            Date d = new Date(l);
            dateString = df.format(d);
            return dateString;
        } catch (Exception e) {
            return "";
        }
    }

    public String formatLongToDay(long l) {
        DateFormat df = new SimpleDateFormat("dd/MM/yyyy");
        String dateString = "";
        try {
            Date d = new Date(l);
            dateString = df.format(d);
            return dateString;
        } catch (Exception e) {
            return "";
        }
    }

    public String formatLongToDay(long l, String format) {
        DateFormat df = new SimpleDateFormat(format);
        String dateString = "";
        try {
            Date d = new Date(l);
            dateString = df.format(d);
            return dateString;
        } catch (Exception e) {
            return "";
        }
    }

    public long formatStringToLong(String date) {
        SimpleDateFormat sdf = new SimpleDateFormat("dd/MM/yyyy");
        try {
            java.util.Date d = sdf.parse(date);
            long l = d.getTime();
            return l;
        } catch (Exception e) {
            return 0;
        }
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

    public long formatStringToLongApi(String date) {
        SimpleDateFormat sdf = new SimpleDateFormat(Constants.mDateApi);
        try {
            java.util.Date d = sdf.parse(date);
            long l = d.getTime();
            return l;
        } catch (Exception e) {
            return 0;
        }
    }

    public String getToDay() {
        DateFormat df = new SimpleDateFormat("dd/MM/yyyy");
        java.util.Date date = new java.util.Date();
        String dateString = "";
        try {
            dateString = df.format(date);
            return dateString;
        } catch (Exception e) {
            return "";
        }
    }

    public String getToDay(String format) {

        DateFormat df = new SimpleDateFormat(Functions.isNullOrEmpty(format) ? "dd/MM/yyyy" : format);
        Date date = new Date();
        String dateString = "";
        try {
            dateString = df.format(date);
            return dateString;
        } catch (Exception e) {
            return "";
        }
    }

    public String getDateString(int day, int minute, String format) {
        DateFormat dateFormat = new SimpleDateFormat(format);
        Calendar cal = Calendar.getInstance();
        cal.add(Calendar.MINUTE, minute);
        cal.add(Calendar.DATE, day);
        return dateFormat.format(cal.getTime());
    }

    public String getDateStringApi(int dataLimitDay) {
        DateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
        Calendar cal = Calendar.getInstance();
        cal.add(Calendar.DATE, dataLimitDay);
        return dateFormat.format(cal.getTime());
    }

    public String getLastTimeMessage() {
        DateFormat df = new SimpleDateFormat("dd/MM/ - HH:mm");
        java.util.Date date = new java.util.Date();
        String dateString = "";
        try {
            dateString = df.format(date);
            return dateString;
        } catch (Exception e) {
            return "";
        }
    }

    public boolean isPasswordCorrectRegister(String pass) {
        return pass.length() >= 4;
    }

    public String formatDateToString(Date date) {
        String dateString = "";
        try {
            DateFormat df = new SimpleDateFormat("yyyy-MM-dd  HH:mm:ss");
            dateString = df.format(date);
        } catch (Exception ex) {
            return "";
        }
        return dateString;
    }

    public boolean isPasswordCorrect(String pass) {
        return pass.length() >= 8 && isHaveNumber(pass) && isHaveCase(pass);
    }

    private boolean isHaveNumber(String pass) {
        for (char c : pass.toCharArray()) {
            for (int c1 : NUM) {
                if ((c1 + "").equals(c + "")) return true;
            }
        }
        return false;
    }

    private boolean isHaveCase(String pass) {
        for (char c : pass.toCharArray()) {
            for (String c1 : TEXT_CASE) {
                if ((c1).equals(c + "")) return true;
            }
        }
        return false;
    }

    public String formatMoneyD(double s) {
        DecimalFormat format = new DecimalFormat("###,###,###");
        try {
            format.format(s);
            return (format.format(s) + " đ").replaceAll("\\.", ","); // Some devices return like 1.000 => 1,000
        } catch (Exception e) {
            return "";
        }
    }

    public static char removeAccent(char ch) {
        int index = Arrays.binarySearch(SOURCE_CHARACTERS, ch);
        if (index >= 0) {
            ch = DESTINATION_CHARACTERS[index];
        }
        return ch;
    }

    /**
     * Bo dau 1 chuoi
     *
     * @param s String
     * @return String after removed
     */
    public static String removeAccent(String s) {
        StringBuilder sb = new StringBuilder(s);
        for (int i = 0; i < sb.length(); i++) {
            sb.setCharAt(i, removeAccent(sb.charAt(i)));
        }
        return sb.toString();
    }

    // This method help remove html tag in a string
    public Spanned stripHtml(String html) {
        if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.N) {
            return Html.fromHtml(html, Html.FROM_HTML_MODE_LEGACY);
        } else {
            return Html.fromHtml(html);
        }
    }

    public boolean isEdttEmpty(EditText editText) {
        return editText.getText().toString().isEmpty();
    }

    // Check Email validate
    public boolean isEmailValidate(String email) {
        if (TextUtils.isEmpty(email)) return false;
        boolean isValid = false;

        String expression = "^[\\w\\.-]+@([\\w\\-]+\\.)+[A-Z]{2,4}$";
        CharSequence inputStr = email;

        Pattern pattern = Pattern.compile(expression, Pattern.CASE_INSENSITIVE);
        Matcher matcher = pattern.matcher(inputStr);
        if (matcher.matches()) {
            isValid = true;
        }
        return isValid;
    }

    public static void Toast(Context context, String thongbao) {
        Toast.makeText(context, thongbao, Toast.LENGTH_SHORT).show();
    }

    public boolean isPhoneValidate(String phone) {
        return phone.length() > 0;
    }

    public String encodeBase64(String s) {
        String res = "";
        byte[] data = s.getBytes(StandardCharsets.UTF_8);
        res = Base64.encodeToString(data, Base64.DEFAULT);
        return res.trim();
    }

    public float getFileSizeInMB(File fileName) {
        float ret = getFileSizeInBytes(fileName);
        ret = ret / (float) (1024 * 1024);
        return ret;
    }

    private static long getFileSizeInBytes(File f) {
        long ret = 0;
        if (f.isFile()) {
            return f.length();
        } else if (f.isDirectory()) {
            File[] contents = f.listFiles();
            for (File content : contents) {
                if (content.isFile()) {
                    ret += content.length();
                } else if (content.isDirectory())
                    ret += getFileSizeInBytes(content);
            }
        }
        return ret;
    }

    public String decodeBase64(String s) {
        String res = "";
        byte[] data = Base64.decode(s, Base64.DEFAULT);
        res = new String(data, StandardCharsets.UTF_8);
        return res.trim();
    }

    public void startVibrator(Activity activity) {
        Vibrator vibrator = (Vibrator) activity.getSystemService(Context.VIBRATOR_SERVICE);
        if (vibrator.hasVibrator()) {
            vibrator.vibrate(500); // for 500 ms
        }
    }

    public int getQuality(File file) {
        float fileSize = getFileSizeInMB(file);

        if (fileSize < 1.3) return 100;

        float buff = fileSize / 1.4f;

        return (int) (100 / buff);
    }

    public String getDurationString(int seconds) {
        if (seconds < 0 || seconds > 2000000)//there is an codec problem and duration is not set correctly,so display meaningfull string
            seconds = 0;
        int hours = seconds / 3600;
        int minutes = (seconds % 3600) / 60;
        seconds = seconds % 60;

        if (hours == 0)
            return twoDigitString(minutes) + ":" + twoDigitString(seconds);
        else
            return twoDigitString(hours) + ":" + twoDigitString(minutes) + ":" + twoDigitString(seconds);
    }

    private String twoDigitString(int number) {

        if (number == 0) {
            return "00";
        }

        if (number / 10 == 0) {
            return "0" + number;
        }

        return String.valueOf(number);
    }

    public File persistImage(Bitmap bitmap, File oldFile, int quality) {
        File imageFile = new File(oldFile + ".jpg");

        OutputStream os;
        try {
            os = new FileOutputStream(imageFile);
            bitmap.compress(Bitmap.CompressFormat.JPEG, quality, os);
            os.flush();
            os.close();
        } catch (Exception e) {
            Log.e("TAG", "Error writing bitmap", e);
        }
        return imageFile;
    }

    public String getTitle(String fieldId, String defaultValue) {
        String retValue = defaultValue;
        AppLanguage result = new RealmHelper<AppLanguage>().getItemById(AppLanguage.class, "Key", fieldId);
        if (result == null) {
            return retValue;
        }

        retValue = CurrentUser.getInstance().getUser().getLanguage() == 1066 ? result.getValueVN() : result.getValueEN();
        return retValue;
    }

    public String getTitleByRegion(String fieldId, String defaultValue, int region) {
        String retValue = defaultValue;
        AppLanguage result = new RealmHelper<AppLanguage>().getItemById(AppLanguage.class, "Key", fieldId);
        if (result == null) {
            return retValue;
        }

        retValue = region == 1066 ? result.getValueVN() : result.getValueEN();
        return retValue;
    }

    public String getAppSettings(String key) {
        String retValue = "";
        Settings setting = new RealmHelper<Settings>().getItemById(Settings.class, "KEY", key);
        if (setting != null) {
            retValue = setting.getVALUE();
        }
        return retValue;
    }

    public String getDateString(String date) {
        String result = "";
        if(!Functions.isNullOrEmpty(date))
        {
            if(date.split("T").length==1)
            {
                String s = date.split("T")[0];
                String[] arr = s.split("-");
                result = arr[2] + "/" + arr[1] + "/" + arr[0];
                return result;
            }
        }
        return result;
    }

    public String getDateLanguage(String date, int type, int langCode) {
        String result = "";
        ArrayList<TimeLanguage> languages = new TimeAppController().getTimeLanguage(type);

        if (type == 0) {
            if (getDateString(date).equals(getToDay(-1)) || getDateString(date).equals(getToDay())) {
                result = DateTimeUtility.formatFullTime(formatStringToLongApi(date));
            } else {
                Date dateNow = formatStringToDate(getToDay(Constants.mDateApi));
                Date currentDate = formatStringToDate(date);
                long diff = dateNow.getTime() - currentDate.getTime();
                long seconds = diff / 1000;
                long minutes = seconds / 60;

                long value = minutes;
                if (minutes > 1200) {
                    value = minutes / 1200;
                } else if (minutes > 60) {
                    value = minutes / 60;
                }

                for (TimeLanguage time : languages) {
                    if (time.getTime() == null || minutes <= time.getTime()) {
                        if (time.getTitle().contains("yy") || time.getTitle().contains("HH")) {
                            result = DateTimeUtility.formatFullTime(formatStringToLongApi(date), langCode == 1066 ? time.getTitle() : time.getTitleEN());
                        } else {
                            result = String.format(langCode == 1066 ? time.getTitle().replace("{0}", "%d") : time.getTitleEN().replace("{0}", "%d"), value);
                        }
                        break;
                    }
                }
            }
        } else if (type == 1) {
            Date dateNow = formatStringToDate(getToDay(Constants.mDateApi));
            Date currentDate = formatStringToDate(date);
            long diff = dateNow.getTime() - currentDate.getTime();
            long dayCount = (long) Math.floor(((float) diff / (24 * 60 * 60 * 1000)) + 0.5d);

            for (TimeLanguage time : languages) {
                if (time.getTime() == null || dayCount <= time.getTime()) {
                    if (time.getTitle().contains("yy") || time.getTitle().contains("HH")) {
                        result = DateTimeUtility.formatFullTime(formatStringToLongApi(date), langCode == 1066 ? time.getTitle() : time.getTitleEN());
                    } else {
                        result = String.format(langCode == 1066 ? time.getTitle().replace("{0}", "%d") : time.getTitleEN().replace("{0}", "%d"), Math.abs(dayCount));
                    }
                    break;
                }
            }
        }

        return result;
    }

    public int getColorByAppStatus(int appStatusId) {
        switch (appStatusId) {
            case 2:
                return ContextCompat.getColor(BaseActivity.sBaseActivity, R.color.clTaskStatusGray);
            case 4:
                return ContextCompat.getColor(BaseActivity.sBaseActivity, R.color.clTaskStatusBlue);
            case 16:
            case 64:
            case 128:
                return ContextCompat.getColor(BaseActivity.sBaseActivity, R.color.clTaskStatusRed);
            case 1:
                return ContextCompat.getColor(BaseActivity.sBaseActivity, R.color.clStatusGray);
            case 8:
                return ContextCompat.getColor(BaseActivity.sBaseActivity, R.color.clTaskStatusGreen);
            case 32:
                return ContextCompat.getColor(BaseActivity.sBaseActivity, R.color.clStatusRed);
            default:
                return ContextCompat.getColor(BaseActivity.sBaseActivity, R.color.clStatusBlue);
        }
    }

    public int getColorByDueDate(String date) {
        if (!Functions.isNullOrEmpty(date)) {
            if (formatStringToLong(getDateString(date)) >= formatStringToLong(getToDay()) && formatStringToLong(getDateString(date)) < formatStringToLong(getToDay(1))) {
                return ContextCompat.getColor(BaseActivity.sBaseActivity, R.color.clPurpleToday);
            } else if (formatStringToLong(getDateString(date)) < formatStringToLong(getToDay())) {
                return ContextCompat.getColor(BaseActivity.sBaseActivity, R.color.clRedMain);
            }
        }

        return ContextCompat.getColor(BaseActivity.sBaseActivity, R.color.clBlack);
    }

    public String[] getArrayFullNameFromArrayID(String[] arrayId) {
        String[] result = new String[arrayId.length];
        for (int i = 0; i < arrayId.length; i++) {
            User user = new RealmHelper<User>().getItemById(User.class, "ID", arrayId[i].toLowerCase());
            if (user != null) {
                result[i] = user.getFullName();
            } else { // Nếu ko có user -> searchGroup
                Group group = new RealmHelper<Group>().getItemById(Group.class, "ID", arrayId[i].toLowerCase());
                if (group != null) {
                    result[i] = group.getTitle();
                }
            }
        }
        return result;
    }

    public void setTextView_FormatMultiUser(TextView tV, String[] arrUser, boolean isHighColor, boolean showFromText) {
        Spannable spannable;
        String result = "";

        if (showFromText) {
            if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
                result += "Đến: ";
            } else {
                result += "To: ";
            }
        }

        if (tV != null) {
            if (arrUser.length < 2) {
                result += arrUser[0];
            } else {
                result += String.format("%s, +%s", arrUser[0], arrUser.length - 1);
            }
        }

        if (isHighColor && result.contains("+")) {
            int startPos = result.indexOf('+');
            spannable = new SpannableString(result.trim());
            ColorStateList color = new ColorStateList(new int[][]{new int[]{}}, new int[]{ContextCompat.getColor(BaseActivity.sBaseActivity, R.color.clBottomEnable)});
            TextAppearanceSpan highlightSpan = new TextAppearanceSpan("", Typeface.NORMAL, -1, color, null);
            spannable.setSpan(highlightSpan, startPos, result.length(), Spanned.SPAN_EXCLUSIVE_EXCLUSIVE);
            tV.setText(spannable, TextView.BufferType.SPANNABLE);
        } else {
            tV.setText(result);
        }
    }

    public void setTextView_FormatMultiUser_DetailWorkflow(TextView tv, String[] lstUser, boolean plusMoreFormat) {
        String result = getTitle("TEXT_TO", "Đến: ");
        if (plusMoreFormat) {
            if (lstUser.length < 2) {
                result += lstUser[0];
            } else {
                result += String.format("%s, +%s", lstUser[0], lstUser.length - 1);
            }

            tv.setText(result);
            boolean isEllipsized = checkTVIsEllipsized(tv);

            if (!isEllipsized) {
                int startPos = result.indexOf('+');
                if (startPos != -1) {
                    Spannable spannable = new SpannableString(result.trim());
                    ColorStateList Color = new ColorStateList(new int[][] { new int[] { } }, new int[] { ContextCompat.getColor(BaseActivity.sBaseActivity, R.color.clBottomEnable) });
                    TextAppearanceSpan highlightSpan = new TextAppearanceSpan("",Typeface.NORMAL, -1, Color, null);
                    spannable.setSpan(highlightSpan, startPos,result.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
                    tv.setText(spannable, TextView.BufferType.SPANNABLE);
                } else {
                    tv.setText(result);
                }
            }
        } else {
            result += String.join(", ", lstUser);
            tv.setText(result);
        }
    }

    private boolean checkTVIsEllipsized(TextView tv) {
        if (tv == null) {
            return false;
        }

        TextUtils.TruncateAt truncateAt = tv.getEllipsize();
        if (truncateAt == null || TextUtils.TruncateAt.MARQUEE.equals(truncateAt)) {
            return  false;
        }

        Layout layout = tv.getLayout();
        if (layout == null) {
            return false;
        }

        for (int i = 0; i < layout.getLineCount(); i++) {
            if (layout.getEllipsisCount(i) > 0) {
                return true;
            }
        }

        return false;
    }
    public String getCountOfNumText(String num) {
        Pattern pattern = Pattern.compile("[0-9+()]+");
        Matcher matcher = pattern.matcher(num);
        if (matcher.find()) {
            return matcher.group();
        } else {
            return "";
        }
    }

    public void setTVSelected(TextView tV) {
        if (tV != null) {
            tV.setBackgroundResource(R.drawable.drawable_tabselected);
            tV.setTextColor(ContextCompat.getColor(BaseActivity.sBaseActivity, R.color.clVer2BlueMain));
            tV.setTypeface(ResourcesCompat.getFont(BaseActivity.sBaseActivity, R.font.fontarial), Typeface.BOLD);
        }
    }

    public void setTVUnSelected(TextView tV) {
        if (tV != null) {
            tV.setBackgroundResource(R.drawable.drawable_tabnotselected);
            tV.setTextColor(ContextCompat.getColor(BaseActivity.sBaseActivity, R.color.clWhite));
            tV.setTypeface(ResourcesCompat.getFont(BaseActivity.sBaseActivity, R.font.fontarial), Typeface.NORMAL);
        }
    }

    public void setFormatItemCount(TextView tV, int count, String category, String categoryVDT) {
        String result = "";
        String numFormat = "";
        if (count > 99) {
            if (Functions.isNullOrEmpty(category)) {
                numFormat = "99+";
            } else {
                numFormat = "(99+)";
            }
        } else if (count >= 10) {
            if (Functions.isNullOrEmpty(category)) {
                numFormat = String.valueOf(count);
            } else {
                numFormat = "(" + count + ")";
            }
        } else if (count > 0) {
            if (Functions.isNullOrEmpty(category)) {
                numFormat = "0" + count;
            } else {
                numFormat = "(0" + count + ")";
            }
        } else {
            numFormat = "";
        }

        switch (category) {
            case "vdt": {
                switch (categoryVDT) {
                    default: {
                        result = String.format("%s %s", getTitle("TEXT_TOME", "To me"), numFormat);
                        break;
                    }
                    case "inprocess": {
                        result = String.format("%s %s", getTitle("TEXT_INPROCESS", "In process"), numFormat);
                        break;
                    }
                    case "process": {
                        result = String.format("%s %s", getTitle("TEXT_PROCESSED", "Processed"), numFormat);
                        break;
                    }
                }
                break;
            }
            case "vtbd": {
                result = String.format("%s %s", getTitle("TEXT_FROMME", "From me"), numFormat);
                break;
            }
            case "follow": {
                result = String.format("%s %s", getTitle("TEXT_FOLLOW", "Theo dõi"), numFormat);
                break;
            }
            case "":
            default: {
                result = numFormat;
                break;
            }
        }

        if (tV != null) {
            tV.setText(result);
        }
    }

    public void setTVHighligtColor(TextView tV, String content, String startChar, String endChar, int type) {
        int startPos = -1, endPos = -1;
        if (startChar == null || endChar == null) {
            Spannable spannable = new SpannableString(content.trim());
            ColorStateList white = new ColorStateList(new int[][]{new int[]{}}, new int[]{ContextCompat.getColor(BaseActivity.sBaseActivity, R.color.clWhite)});
            TextAppearanceSpan highlightSpan = new TextAppearanceSpan("", Typeface.NORMAL, -1, white, null);
            spannable.setSpan(highlightSpan, 0, content.length() - 1, Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
            tV.setText(spannable, TextView.BufferType.SPANNABLE);
            return;
        }

        if (type == 0) {
            startPos = content.indexOf(startChar);
            endPos = content.indexOf(endChar) + 1;
        } else if (type == 1) {
            startPos = content.indexOf(startChar) + 1;
            endPos = content.indexOf(endChar);
        }

        if (startPos != -1 && endPos != -1) {
            Spannable spannable = new SpannableString(content.trim());
            ColorStateList red = new ColorStateList(new int[][]{new int[]{}}, new int[]{ContextCompat.getColor(BaseActivity.sBaseActivity, R.color.clOrangeFilter)});
            TextAppearanceSpan highlightSpan = new TextAppearanceSpan("", Typeface.NORMAL, -1, red, null);
            spannable.setSpan(highlightSpan, startPos, endPos, Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
            tV.setText(spannable, TextView.BufferType.SPANNABLE);
        }
    }

    public String getWorkflowItemIDByUrl(String url) {
        Uri uri = Uri.parse(url.toLowerCase());
        String v = uri.getQueryParameter("rid");
        if (isNullOrEmpty(v)) {
            v = uri.getQueryParameter("itemid");
        }

        return v;
    }

    public int getColorByUsername(String username) {
        if (!isNullOrEmpty(username)) {
            switch (String.valueOf(username.charAt(0)).toUpperCase()) {
                case "A":
                    return Color.parseColor(AvatarHex.A);
                case "B":
                    return Color.parseColor(AvatarHex.B);
                case "C":
                    return Color.parseColor(AvatarHex.C);
                case "D":
                    return Color.parseColor(AvatarHex.D);
                case "E":
                    return Color.parseColor(AvatarHex.E);
                case "F":
                    return Color.parseColor(AvatarHex.F);
                case "G":
                    return Color.parseColor(AvatarHex.G);
                case "H":
                    return Color.parseColor(AvatarHex.H);
                case "I":
                    return Color.parseColor(AvatarHex.I);
                case "J":
                    return Color.parseColor(AvatarHex.J);
                case "K":
                    return Color.parseColor(AvatarHex.K);
                case "L":
                    return Color.parseColor(AvatarHex.L);
                case "M":
                    return Color.parseColor(AvatarHex.M);
                case "N":
                    return Color.parseColor(AvatarHex.N);
                case "O":
                    return Color.parseColor(AvatarHex.O);
                case "P":
                    return Color.parseColor(AvatarHex.P);
                case "Q":
                    return Color.parseColor(AvatarHex.Q);
                case "R":
                    return Color.parseColor(AvatarHex.R);
                case "S":
                    return Color.parseColor(AvatarHex.S);
                case "T":
                    return Color.parseColor(AvatarHex.T);
                case "U":
                    return Color.parseColor(AvatarHex.U);
                case "V":
                    return Color.parseColor(AvatarHex.V);
                case "W":
                    return Color.parseColor(AvatarHex.W);
                case "X":
                    return Color.parseColor(AvatarHex.X);
                case "Y":
                    return Color.parseColor(AvatarHex.Y);
                case "Z":
                    return Color.parseColor(AvatarHex.Z);
                default:
                    return Color.parseColor(AvatarHex.A);
            }
        }

        return Color.parseColor(AvatarHex.A);
    }

    public String getAvatarName(String name) {
        String res = "";
        if (!Functions.isNullOrEmpty(name)) {
            name = name.trim();
            if (!Functions.isNullOrEmpty(name) && name.contains(" ")) {
                name = removeAccent(name);
                String[] arr = name.split(" ");
                String first = arr[0].substring(0, 1).toUpperCase();
                String last = arr[arr.length - 1].substring(0, 1).toUpperCase();
                res = first + last;
            } else {
                res = name.substring(0, 1).toUpperCase();
            }
        }

        return res;
    }

    public String formatDayLanguage(String date) {
        String result = "";
        long miliseconds = formatStringToLongApi(date);
        if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
            result = formatLongToDay(miliseconds, "dd/MM/yy");
        } else {
            result = formatLongToDay(miliseconds, "MM/dd/yy");
        }
        return  result;
    }

    public String formatDateLanguage(String date) {
        String result = "";
        long miliseconds = formatStringToLongApi(date);
        if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
            result = formatLongToDay(miliseconds, "dd/MM/yy HH:mm");
        } else {
            result = formatLongToDay(miliseconds, "MM/dd/yy HH:mm");
        }
        return  result;
    }

    public String getFormatFileSize(long size) {
        DecimalFormat df = new DecimalFormat("0.00");
        float sizeKb = 1024.0f;
        float sizeMb = sizeKb * sizeKb;
        float sizeGb = sizeMb * sizeKb;
        float sizeTerra = sizeGb * sizeKb;

        if(size < sizeMb)
            return df.format(size / sizeKb)+ " KB";
        else if(size < sizeGb)
            return df.format(size / sizeMb) + " MB";
        else if(size < sizeTerra)
            return df.format(size / sizeGb) + " GB";

        return "";
    }

    public void setTVHighlightControl(Context context, TextView tv) {
        int startPos = -1, endPos = -1;

        char[] s = tv.getText().toString().toCharArray();
        for (int i = 0; i < s.length; i++) {
            if (s[i] == '(') {
                startPos = i;
            }
            if (s[i] == ')') {
                endPos = i;
            }
        }

        if (startPos != -1 && endPos != -1) {
            Spannable spannable = new SpannableString(tv.getText().toString().trim());
            ColorStateList Red = new ColorStateList(new int[][] { new int[] { } }, new int[] { ContextCompat.getColor(context, R.color.clActionRed) });
            TextAppearanceSpan highlightSpan = new TextAppearanceSpan("", Typeface.BOLD, -1, Red, null);
            spannable.setSpan(highlightSpan, startPos, endPos + 1, Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
            tv.setText(spannable, TextView.BufferType.SPANNABLE);
        }
    }

    public static class AvatarHex
    {
        public static String A = "#4D004D";
        public static String B = "#0000B3";
        public static String C = "#CC3300";
        public static String D = "#CC6600";
        public static String E = "#B16E4B";
        public static String F = "#002DB3";
        public static String G = "#248F24";
        public static String H = "#CC0000";
        public static String I = "#990099";
        public static String J = "#9900CC";
        public static String K = "#330099";
        public static String L = "#B30000";
        public static String M = "#007399";
        public static String N = "#AF6E4E";
        public static String O = "#7A00CC";
        public static String P = "#0000FF";
        public static String Q = "#196619";
        public static String R = "#38908F";
        public static String S = "#003399";
        public static String T = "#662200";
        public static String U = "#5900B3";
        public static String V = "#196666";
        public static String W = "#005C99";
        public static String X = "#00994D";
        public static String Y = "#009900";
        public static String Z = "#009973";
    }
}

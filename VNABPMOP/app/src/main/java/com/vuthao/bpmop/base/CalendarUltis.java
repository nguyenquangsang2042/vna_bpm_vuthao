package com.vuthao.bpmop.base;

import android.util.Log;

import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Objects;

public class CalendarUltis {
    public int YEAR;
    public int MONTH;
    public int DAY_OF_MONTH;
    public int HOUR;
    public int MINUTE;
    public int SECOND;
    private final Calendar c = Calendar.getInstance();

    public CalendarUltis() {
        YEAR = c.get(Calendar.YEAR);
        MONTH = c.get(Calendar.MONTH);
        DAY_OF_MONTH = c.get(Calendar.DAY_OF_MONTH);
        HOUR = c.get(Calendar.HOUR);
        MINUTE = c.get(Calendar.MINUTE);
        SECOND = c.get(Calendar.SECOND);
    }

    public CalendarUltis(String date) {
        try {
            SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
            c.setTime(sdf.parse(date));// all done
        } catch (Exception exception) {
            Log.d("ERR ", exception.getMessage());
        }

        YEAR = c.get(Calendar.YEAR);
        MONTH = c.get(Calendar.MONTH);
        DAY_OF_MONTH = c.get(Calendar.DAY_OF_MONTH);
        HOUR = c.get(Calendar.HOUR);
        MINUTE = c.get(Calendar.MINUTE);
        SECOND = c.get(Calendar.SECOND);
    }

    public CalendarUltis(String date, String format) {
        try {
            SimpleDateFormat sdf = new SimpleDateFormat(format);
            c.setTime(Objects.requireNonNull(sdf.parse(date)));
        } catch (Exception exception) {
            Log.d("ERR ", exception.getMessage());
        }

        YEAR = c.get(Calendar.YEAR);
        MONTH = c.get(Calendar.MONTH);
        DAY_OF_MONTH = c.get(Calendar.DAY_OF_MONTH);
        HOUR = c.get(Calendar.HOUR);
        MINUTE = c.get(Calendar.MINUTE);
        SECOND = c.get(Calendar.SECOND);
    }

    public long getTimeInMillis() {
        return c.getTimeInMillis();
    }
}

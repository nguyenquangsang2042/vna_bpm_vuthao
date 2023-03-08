package com.vuthao.bpmop.base;

import com.vuthao.bpmop.base.model.app.CurrentUser;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Arrays;
import java.util.Calendar;
import java.util.Date;
import java.util.List;
import java.util.Locale;
import java.util.concurrent.TimeUnit;

public class DateTimeUtility {

    private static final List<String> timesStringViet = Arrays.asList("year", "month", "day", "hour", "minute", "second");

    public static String formatDateStart(long milliseconds) {
        return new SimpleDateFormat("dd/MM").format(milliseconds);
    }

    public static String getFormat(String type) {
        String format = "";
        if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
            switch (type) {
                case "task": {
                    format = "dd/MM/yy HH:mm";
                    break;
                }
            }
        } else {
            switch (type) {
                case "task": {
                    format = "MM/dd/yy HH:mm";
                    break;
                }
            }
        }

        return format;
    }

    public static String formatDateToDayMonth(long milliseconds) {
        return new SimpleDateFormat("dd MMMM").format(milliseconds);
    }

    public static String formatDateToDayMonthYear(long milliseconds) {
        return new SimpleDateFormat("dd/MM/yyyy").format(milliseconds);
    }

    public static String formatFullTime(long milliseconds) {
        return new SimpleDateFormat("HH:mm").format(milliseconds);
    }

    public static String formatFullTime(long milliseconds, String format) {
        return new SimpleDateFormat(format).format(milliseconds);
    }

    public static String formatEvent(long startMilliseconds, long endMilliseconds) {
        String timeStart = new SimpleDateFormat("hh:mma").format(startMilliseconds);
        String timeEnd = new SimpleDateFormat("hh:mma").format(endMilliseconds);
        String time = timeStart + " - " + timeEnd;
        return time;
    }

    public static String formatFullEventTime(long startMilliseconds, long endMilliseconds) {
        String dateStart = new SimpleDateFormat("EEEE, dd MMMM, yyyy").format(startMilliseconds);
        String dateEnd = new SimpleDateFormat("EEEE, dd MMMM, yyyy").format(endMilliseconds);
        String timeStart = new SimpleDateFormat("hh:mma").format(startMilliseconds);
        String timeEnd = new SimpleDateFormat("hh:mma").format(endMilliseconds);
        if (dateStart.equalsIgnoreCase(dateEnd)) {
            return dateStart + " " + timeStart + " - " + timeEnd;
        } else {
            return dateStart + " - " + dateEnd + " " + timeStart + " - " + timeEnd;
        }
    }

    public static String format(long milliseconds) {
        return new SimpleDateFormat("dd/MM/yyyy hh:mma").format(milliseconds);
    }

    public static String format(long milliseconds, String format) {
        return new SimpleDateFormat(format).format(milliseconds);
    }

    public static String formatDayofWeek(long milliseconds) {
        return new SimpleDateFormat("EEEE").format(milliseconds);
    }

    public static String formatTime(long milliseconds) {
        return new SimpleDateFormat("hh:mma").format(milliseconds);
    }

    public static String getHour(long milliseconds) {
        return new SimpleDateFormat("HH").format(milliseconds);
    }

    public static String getMinute(long milliseconds) {
        return new SimpleDateFormat("mm").format(milliseconds);
    }

    public static String getDay(long milliseconds) {
        return new SimpleDateFormat("dd").format(milliseconds);
    }

    public static String getMonth(long milliseconds) {
        return new SimpleDateFormat("M").format(milliseconds);
    }

    public static String getYear(long milliseconds) {
        return new SimpleDateFormat("yyyy").format(milliseconds);
    }

    public static String formatDateToMonth(long milliseconds) {
        return new SimpleDateFormat("MMMM").format(milliseconds);
    }

    public static String formatDate(long milliseconds) {
        return new SimpleDateFormat("dd/MM/yyyy").format(milliseconds);
    }

    public static String formatDateEvent(long timeStart, long timeEnd, long date) {
        String start = new SimpleDateFormat(" hh:mma ").format(timeStart);
        String end = new SimpleDateFormat(" hh:mma ").format(timeEnd);
        String dateStart = new SimpleDateFormat("dd/MM").format(timeStart);
        String dateEnd = new SimpleDateFormat("dd/MM").format(timeEnd);
        if (dateStart.equalsIgnoreCase(dateEnd)) {
            return dateStart + " " + start + "-" + end;
        } else {
            dateStart = new SimpleDateFormat("dd/MM").format(timeStart);
            dateEnd = new SimpleDateFormat("dd/MM").format(timeEnd);
            return dateStart + "-" + dateEnd + " " + start + "-" + end;
        }
    }

    /**
     * Checking two timestamp to know they are in one day or not
     *
     * @param time1 timestamp 1
     * @param time2 timestamp 2
     * @return True if they are in one day
     */
    public static boolean isOneDay(long time1, long time2) {
        return formatDateToDayMonthYear(time1).equalsIgnoreCase(formatDateToDayMonthYear(time2));
    }

    public static long getTimeStartOfDay(long millis) {
        Calendar cal = Calendar.getInstance();
        cal.setTimeInMillis(millis); // compute start of the day for the timestamp
        cal.set(Calendar.HOUR_OF_DAY, 0);
        cal.set(Calendar.MINUTE, 0);
        cal.set(Calendar.SECOND, 0);
        cal.set(Calendar.MILLISECOND, 0);
        return cal.getTimeInMillis();
    }

    public static long getTimeEndOfDay(long millis) {
        Calendar cal = Calendar.getInstance();
        cal.setTimeInMillis(millis); // compute start of the day for the timestamp
        cal.set(Calendar.HOUR_OF_DAY, 23);
        cal.set(Calendar.MINUTE, 59);
        cal.set(Calendar.SECOND, 59);
        cal.set(Calendar.MILLISECOND, 999);
        return cal.getTimeInMillis();
    }


    public static final List<Long> times = Arrays.asList(
            TimeUnit.DAYS.toMillis(365),
            TimeUnit.DAYS.toMillis(30),
            TimeUnit.DAYS.toMillis(1),
            TimeUnit.HOURS.toMillis(1),
            TimeUnit.MINUTES.toMillis(1),
            TimeUnit.SECONDS.toMillis(1));

    /**
     * Here is the place format time
     *
     * @param dateFrom the Past of time want to know where (millis)
     * @return format like 1 day ago, 1 hour ago, 20:00 (in current day) or full display time
     */
    public static String formatTimeMessage(Long dateFrom) {
        return DateTimeUtility.toDuration(System.currentTimeMillis(), dateFrom, Locale.getDefault().getDisplayLanguage());
    }

    public static String formatTimeComment(Long dateFrom) {
        return DateTimeUtility.toDuration(System.currentTimeMillis(), dateFrom, Locale.getDefault().getDisplayLanguage());
    }

    private static String toDuration(long now, long time, String language) {
        StringBuilder res = new StringBuilder();
        long sevenDay = 604800000;

        // Display the day if that day longer than 7 days
        if (now - time > sevenDay) {
            return formatDate(time) + " " + formatFullTime(time);
        }

        for (int i = 0; i < times.size(); i++) {
            Long current = times.get(i);
            long temp = (now - time) / current;
            if (temp > 0) {
                res.append(temp).append(" ").append(timesStringViet.get(i)).append(temp > 1 ? "s" : "")
                        .append(" ago");
                break;
            }
        }
        if ("".equals(res.toString()))
            return "Just now";
        else {
            String s = res.toString().toLowerCase();
            if (s.equalsIgnoreCase("1 day ago"))
                return "Yesterday";
            return res.toString();
        }
    }

    public static boolean isDateInCurrentWeek(long date) {
        Date date1 = new Date(date);
        return isDateInCurrentWeek(date1);
    }

    public static boolean isTheSameDay(long day1, long day2){
        return getTimeStartOfDay(day1) == getTimeStartOfDay(day2);
    }

    public static boolean isAfterDay(long day1, long day2){
        return getTimeStartOfDay(day1) < getTimeStartOfDay(day2);
    }

    public static boolean isBeforeDay(long day1, long day2){
        return getTimeStartOfDay(day1) > getTimeStartOfDay(day2);
    }

    private static boolean isDateInCurrentWeek(Date date) {
        Calendar currentCalendar = Calendar.getInstance();
        int week = currentCalendar.get(Calendar.WEEK_OF_YEAR);
        int year = currentCalendar.get(Calendar.YEAR);
        Calendar targetCalendar = Calendar.getInstance();
        targetCalendar.setTime(date);
        int targetWeek = targetCalendar.get(Calendar.WEEK_OF_YEAR);
        int targetYear = targetCalendar.get(Calendar.YEAR);
        return week == targetWeek && year == targetYear;
    }

    public static int getCountOfDays(String createdDateString, String expireDateString) {
        SimpleDateFormat dateFormat = new SimpleDateFormat("dd/MM/yyyy", Locale.getDefault());

        Date createdConvertedDate = null, expireCovertedDate = null, todayWithZeroTime = null;
        try {
            createdConvertedDate = dateFormat.parse(createdDateString);
            expireCovertedDate = dateFormat.parse(expireDateString);

            Date today = new Date();

            todayWithZeroTime = dateFormat.parse(dateFormat.format(today));
        } catch (ParseException e) {
            e.printStackTrace();
        }

        int cYear = 0, cMonth = 0, cDay = 0;

        if (createdConvertedDate.after(todayWithZeroTime)) {
            Calendar cCal = Calendar.getInstance();
            cCal.setTime(createdConvertedDate);
            cYear = cCal.get(Calendar.YEAR);
            cMonth = cCal.get(Calendar.MONTH);
            cDay = cCal.get(Calendar.DAY_OF_MONTH);

        } else {
            Calendar cCal = Calendar.getInstance();
            cCal.setTime(todayWithZeroTime);
            cYear = cCal.get(Calendar.YEAR);
            cMonth = cCal.get(Calendar.MONTH);
            cDay = cCal.get(Calendar.DAY_OF_MONTH);
        }

        Calendar eCal = Calendar.getInstance();
        eCal.setTime(expireCovertedDate);

        int eYear = eCal.get(Calendar.YEAR);
        int eMonth = eCal.get(Calendar.MONTH);
        int eDay = eCal.get(Calendar.DAY_OF_MONTH);

        Calendar date1 = Calendar.getInstance();
        Calendar date2 = Calendar.getInstance();

        date1.clear();
        date1.set(cYear, cMonth, cDay);
        date2.clear();
        date2.set(eYear, eMonth, eDay);

        long diff = date2.getTimeInMillis() - date1.getTimeInMillis();

        float dayCount = (float) diff / (24 * 60 * 60 * 1000);

        return (int)dayCount;
    }
}


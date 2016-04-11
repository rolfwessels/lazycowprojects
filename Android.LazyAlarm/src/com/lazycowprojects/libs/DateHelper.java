package com.lazycowprojects.libs;

import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.TimeZone;

/**
 * Helper class for dates
 * User: Rolf
 */
public class DateHelper {
    public static String FormatDate(Date date) {
        return new SimpleDateFormat("yyyy/MM/dd G  hh:mm:ss").format(date);
    }

    public static String Duration(Date fromTime, Date toTime) {
        long difference = Math.max(0L, fromTime.getTime() - toTime.getTime());
        Date date = new Date(difference);
        SimpleDateFormat simpleDateFormat = new SimpleDateFormat("HH:mm:ss");
        simpleDateFormat.setTimeZone(TimeZone.getTimeZone("GMT+0"));
        return simpleDateFormat.format(date);
    }
}

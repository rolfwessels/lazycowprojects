package com.lazycowprojects.lazyalarm;

import android.content.Context;
import android.util.Log;
import com.lazycowprojects.libs.DateHelper;

import java.util.Date;

/**
 * Generates the Timer classes
 * User: Rolf
 * Date: 18/08/11
 */
public class TimerViewFactory {

    private static final String TAG = "TimerViewFactory";

    private TimerViewFactory() {

    }

    public static TimerView FromHoursMinutes(int hours, int minutes, Context context) {
        TimerView timerView = new TimerView(context);
        int addTimeMinutes = (hours * 60) + minutes;
        Date now = new Date();
        Log.d(TAG, "Now:" + DateHelper.FormatDate(now));
        long date = now.getTime() + (addTimeMinutes*1000);
        Date then = new Date(date);
        Log.d(TAG,"Then:"+ DateHelper.FormatDate(then));
        timerView.setTime(then);
        return timerView;
    }
}

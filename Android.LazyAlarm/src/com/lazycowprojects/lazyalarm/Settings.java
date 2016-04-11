package com.lazycowprojects.lazyalarm;

import android.content.Context;
import android.media.RingtoneManager;
import android.net.Uri;
import android.os.Bundle;
import android.preference.PreferenceActivity;
import android.preference.PreferenceManager;

/**
 * Contains settings for timer
 * User: Rolf
 * Date: 14/08/11
 */
public class Settings extends PreferenceActivity {
    private static final String OPT_DEFAULT_MINUTES = "DefaultMinutes";
    private static final String OPT_DEFAULT_MINUTES_DEF = "3";
    private static final String OPT_DEFAULT_HOURS = "DefaultHours";
    private static final String OPT_DEFAULT_HOURS_DEF = "0";
    private static final String OPT_RINGTONE = "Ringtone";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        addPreferencesFromResource(R.xml.settings);

    }

    public static int getDefaultHours(Context context) {
        return Integer.parseInt( PreferenceManager.getDefaultSharedPreferences(context)
                .getString(OPT_DEFAULT_HOURS, OPT_DEFAULT_HOURS_DEF));
    }

    public static int getDefaultMinutes(Context context) {
        return Integer.parseInt( PreferenceManager.getDefaultSharedPreferences(context)
                .getString(OPT_DEFAULT_MINUTES, OPT_DEFAULT_MINUTES_DEF));
    }

    public static Uri getRingtone(Context context) {
        Uri defaultUri = RingtoneManager.getDefaultUri(RingtoneManager.TYPE_ALARM);
        return Uri.parse(PreferenceManager.getDefaultSharedPreferences(context)
                .getString(OPT_RINGTONE, defaultUri.toString()));
    }

}

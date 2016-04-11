package com.lazycowprojects.lazyalarm;

import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.app.Service;
import android.content.Intent;
import android.media.Ringtone;
import android.media.RingtoneManager;
import android.os.Binder;
import android.os.Handler;
import android.os.IBinder;
import android.util.Log;
import android.widget.Toast;

import java.util.ArrayList;

public class TimerService extends Service implements TimerView.IStateChangedListener {
    private NotificationManager mNM;
    ArrayList<TimerView> mTimerViews = new ArrayList<TimerView>();
    // Unique Identification Number for the Notification.
    // We use it on Notification start, and to cancel it.
    private int NOTIFICATION = R.string.local_service_started;
    private Ringtone mRingtone;
    Handler mHandler = new Handler();
    public void addTimer(TimerView timerView) {
        mTimerViews.add(timerView);
        timerView.addOnStateChanged(this);
        Toast.makeText(this, "Timer added", Toast.LENGTH_SHORT).show();

    }

    public void removeTimerView(TimerView timerView) {
        mTimerViews.remove(timerView);
//        if (mTimerViews.size() == 0) {
//            stopSelf();
//        }
        mHandler.postDelayed(new RemoveFromListener(timerView,this),1000);
        Toast.makeText(this, "Timer removed", Toast.LENGTH_SHORT).show();
    }

    public void StateChanged(TimerView.StateChangedEvent event) {
        switch (event.getState()) {
            case RingRing:
                stopRingtone();
                playRingTone();
                break;
            case Cleared:
                stopRingtoneIfAllTimersAreStopped();
                break;

        }
    }

    public int countTimerViews() {
        return mTimerViews.size();
    }

    public TimerView[] getAllTimers() {
        TimerView[] timerViews = new TimerView[mTimerViews.size()];
        mTimerViews.toArray(timerViews);
        return timerViews;
    }

    /**
     * Class for clients to access.  Because we know this service always
     * runs in the same process as its clients, we don't need to deal with
     * IPC.
     */
    public class LocalBinder extends Binder {
        TimerService getService() {
            return TimerService.this;
        }
    }

    @Override
    public void onCreate() {
        mNM = (NotificationManager) getSystemService(NOTIFICATION_SERVICE);
        showNotification();
        Log.i("TimerService", "Created");
    }

    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {
        Log.i("TimerService", "Received start id " + startId + ": " + intent);
        return START_STICKY;
    }

    @Override
    public void onDestroy() {
        // Cancel the persistent notification.
        mNM.cancel(NOTIFICATION);

        // Tell the user we stopped.
        Toast.makeText(this, R.string.local_service_stopped, Toast.LENGTH_SHORT).show();
    }

    @Override
    public IBinder onBind(Intent intent) {
        return mBinder;
    }

    // This is the object that receives interactions from clients.  See
    // RemoteService for a more complete example.
    private final IBinder mBinder = new LocalBinder();

    /**
     * Show a notification while this service is running.
     */
    private void showNotification() {
        // In this sample, we'll use the same text for the ticker and the expanded notification
        CharSequence text = getText(R.string.local_service_started);

        // Set the icon, scrolling text and timestamp
        Notification notification = new Notification(R.drawable.timer, text,
                System.currentTimeMillis());

        // The PendingIntent to launch our activity if the user selects this notification
        PendingIntent contentIntent = PendingIntent.getActivity(this, 0,
                new Intent(this, MainScreen.class), 0);

        // Set the info for the views that show in the notification panel.
        notification.setLatestEventInfo(this, getText(R.string.app_name),
                text, contentIntent);

        // Send the notification.
        mNM.notify(NOTIFICATION, notification);
    }


    private void playRingTone() {
        if (mRingtone == null) {
            mRingtone = RingtoneManager.getRingtone(this.getBaseContext(), Settings.getRingtone(this.getBaseContext()));
            mRingtone.play();
        }
    }

    //stop ringtone of nothing is ringing

    private void stopRingtoneIfAllTimersAreStopped() {
        boolean stillPlaying = false;
        for (TimerView v : mTimerViews) {
            if (v.getState() == TimerView.States.RingRing) {
                stillPlaying = true;
            }
        }
        if (!stillPlaying && mRingtone != null) {
            stopRingtone();
        }
    }

    private void stopRingtone() {
        if (mRingtone != null) {
            mRingtone.stop();
            mRingtone = null;
        }
    }

    private class RemoveFromListener implements Runnable {
        private TimerView mTimerView;
        private TimerView.IStateChangedListener mIStateChangedListener;

        public RemoveFromListener(TimerView timerView, TimerView.IStateChangedListener iStateChangedListener) {
            mTimerView = timerView;
            mIStateChangedListener = iStateChangedListener;
        }

        public void run() {
            mTimerView.removeOnStateChanged(mIStateChangedListener);
        }
    }
}

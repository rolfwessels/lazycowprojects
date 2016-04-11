package com.lazycowprojects.lazyalarm;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.LinearLayout;
import android.widget.Toast;
import com.lazycowprojects.libs.NumberPicker;

public class MainScreen extends Activity implements View.OnClickListener, TimerView.IStateChangedListener {
    private static final int MAX_TIMERS = 5;
    private NumberPicker mHour;
    private NumberPicker mMin;
    private LinearLayout mTimerContainer;
    private Handler mHandler;
    private RefreshRunnable mRefreshRunnable;
    private Button mSetButton;
    private TimerServiceClient mTimerServiceClient;


    /**
     * Called when the activity is first created.
     */
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.main);
        InitializeComponents();
        mHandler = new Handler();
        mRefreshRunnable = new RefreshRunnable();
        mRefreshRunnable.run();
        mTimerServiceClient = new TimerServiceClient(this);
    }

    private void InitializeComponents() {
        mHour = (NumberPicker) findViewById(R.id.select_hour);
        mMin = (NumberPicker) findViewById(R.id.select_minute);

        mSetButton = (Button) findViewById(R.id.set_button);
        mTimerContainer = (LinearLayout) findViewById(R.id.timer_container);
        mSetButton.setOnClickListener(this);

        mHour.setFormatter(NumberPicker.TWO_DIGIT_FORMATTER);
        mMin.setFormatter(NumberPicker.TWO_DIGIT_FORMATTER);
        mHour.setRange(0, 23);
        mMin.setRange(0, 59);
        mMin.setCurrent(Settings.getDefaultMinutes(this));
        mHour.setCurrent(Settings.getDefaultHours(this));
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        super.onCreateOptionsMenu(menu);
        MenuInflater menuInflater = getMenuInflater();
        menuInflater.inflate(R.menu.menu, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        switch (item.getItemId()) {
            case R.id.settings:
                startActivity(new Intent(this, Settings.class));
                return true;

        }
        return false;
    }


    public void onClick(View view) {
        switch (view.getId()) {
            case R.id.set_button:
                TimerView timerView = TimerViewFactory.FromHoursMinutes(mHour.getCurrent(), mMin.getCurrent(), this.getBaseContext());
                addTimerView(timerView);
                break;
        }
    }

    private void addTimerView(TimerView timerView) {
        Toast.makeText(this.getBaseContext(), getResources().getString(R.string.timer_started),
                Toast.LENGTH_SHORT).show();
        mTimerServiceClient.addTimer(timerView);
        timerView.addOnStateChanged(this);
        mTimerContainer.addView(timerView, 0);
        if (mTimerServiceClient.currentSize() >= MAX_TIMERS) {
            mSetButton.setEnabled(false);
        }
    }

    private void removeTimerView(final TimerView timerView) {
        mHandler.postDelayed(new RemoveFromListnerRunner(timerView,this),1000);
        mTimerServiceClient.removeTimerView(timerView);
        mTimerContainer.removeView(timerView);
        if (mTimerServiceClient.currentSize() < MAX_TIMERS && !mSetButton.isEnabled()) {
            mSetButton.setEnabled(true);
        }
    }


    private void refreshAllTimes() {
        for (TimerView view : mTimerServiceClient.getAllTimers()) {
            view.refreshTicker();
        }
    }

    public void StateChanged(TimerView.StateChangedEvent event) {

       if (event.getState() == TimerView.States.Cleared) {
            removeTimerView((TimerView) event.getSource());

       }
    }

    private static class RemoveFromListnerRunner implements Runnable {
        private final TimerView mTimerView;
        private TimerView.IStateChangedListener mMainScreen;

        public RemoveFromListnerRunner(TimerView timerView, TimerView.IStateChangedListener mainScreen) {
            mTimerView = timerView;
            mMainScreen = mainScreen;
        }

        public void run() {
            mTimerView.removeOnStateChanged(mMainScreen);
        }
    }

    private class RefreshRunnable implements Runnable {
        public void run() {
            refreshAllTimes();
            mHandler.postDelayed(mRefreshRunnable, 1000);
        }
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        mTimerServiceClient.doUnbindService();
    }
}


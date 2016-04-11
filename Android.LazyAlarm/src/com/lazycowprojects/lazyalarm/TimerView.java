package com.lazycowprojects.lazyalarm;

import android.content.Context;
import android.util.AttributeSet;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.Button;
import android.widget.LinearLayout;
import android.widget.TextView;
import com.lazycowprojects.libs.DateHelper;

import java.util.ArrayList;
import java.util.Date;
import java.util.EventObject;

import static com.lazycowprojects.lazyalarm.TimerView.States.Running;

/**
 * Timer view containing the code behind for timers
 * User: Rolf
 */
public class TimerView extends LinearLayout implements View.OnClickListener {
    public enum States {
        Cleared, Running, RingRing;

    };
    private Date mStopTime;
    private TextView mTimerText;
    private States mState;


    public TimerView(Context context, AttributeSet attrs) {
        super(context, attrs);
        LayoutInflater inflater = (LayoutInflater) context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
        inflater.inflate(R.layout.running_timer, this, true);
        mTimerText = (TextView) findViewById(R.id.time_text);
        Button mClearButton = (Button) findViewById(R.id.clear_button);
        mClearButton.setOnClickListener(this);


    }

    public TimerView(Context context) {
        this(context, null);
    }


    public void setTime(Date theTimeUntil) {
        mStopTime = theTimeUntil;
        setState(Running);
    }

    public void refreshTicker() {
        if (mState == Running) {
        mTimerText.setText(DateHelper.Duration(mStopTime, new Date()));
            if (mState != States.RingRing  && (new Date().getTime() >= mStopTime.getTime())) {
                setState(States.RingRing);
            }
        }
    }

    public void onClick(View view) {
       if (R.id.clear_button == view.getId()) {
            clearButtonClicked();
       }
    }

    private void clearButtonClicked() {
        setState(States.Cleared);

    }

    public void setState(States mState) {
        this.mState = mState;
        switch (mState) {
            case Running:
                mTimerText.setTextColor(getResources().getColor( R.color.timer_text_color));
                break;
            case Cleared:
                mTimerText.setTextColor(getResources().getColor(R.color.timer_text_color_cleared));
                break;
            case RingRing:
                mTimerText.setTextColor(getResources().getColor(R.color.timer_text_color_ring_ring));
                break;

        }
        raiseStateChanged(mState);
    }

    public States getState() {
        return mState;
    }

    private ArrayList<IStateChangedListener> mListener = new ArrayList<IStateChangedListener>();

    private void raiseStateChanged(States mState) {
        for( IStateChangedListener cl : mListener) {
            cl.StateChanged(new StateChangedEvent(this,mState));
        }
    }

    public void addOnStateChanged(IStateChangedListener listener) {
        mListener.add(listener);
    }

     public void removeOnStateChanged(IStateChangedListener listener) {
        mListener.remove(listener);
    }

    public interface IStateChangedListener  {
       void StateChanged(StateChangedEvent event);
    }

    public class StateChangedEvent extends EventObject {

        private States state;

        public StateChangedEvent(Object source, States state) {
            super(source);
            this.state = state;
        }

        public States getState() {
            return state;
        }
    }
}

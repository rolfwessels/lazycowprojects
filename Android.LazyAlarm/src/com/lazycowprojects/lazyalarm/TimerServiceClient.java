package com.lazycowprojects.lazyalarm;

import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.ServiceConnection;
import android.os.IBinder;
import android.util.Log;
import android.widget.Toast;


/**
 * User: Rolf
 * Date: 18/08/11
 */
public class TimerServiceClient {
    private TimerService mBoundService;
    private Context mContext;
    private static final String TAG = "TimerServiceClient";
    private ServiceConnectionRun mConnection;

    public TimerServiceClient(Context context) {
        mContext = context;
    }




    private boolean mIsBound;

    private void doBindService(ServiceConnection connection) {
        // Establish a connection with the service.  We use an explicit
        // class name because we want a specific service implementation that
        // we know will be running in our own process (and thus won't be
        // supporting component replacement by other applications).
        mContext.bindService(new Intent(mContext,
                TimerService.class), connection, Context.BIND_AUTO_CREATE);
        mIsBound = true;

    }

    protected void doUnbindService() {
        if (mIsBound) {
            // Detach our existing connection.
            mContext.unbindService(mConnection);
            mIsBound = false;
        }
    }



    public boolean isIsBound() {
        return mIsBound;
    }

    public void addTimer(final TimerView timerView) {
        waitForBinding(new IRunOnService() {
            public void run(TimerService service) {
                service.addTimer(timerView);
            }
        });
    }

    public void removeTimerView(final TimerView timerView) {
        waitForBinding( new IRunOnService() {
            public void run(TimerService service) {
                 service.removeTimerView(timerView);
            }
        } );
    }

    private void waitForBinding(IRunOnService service) {
        if (!isIsBound()) {
            mConnection = new ServiceConnectionRun(service);
            doBindService(mConnection);
             Log.i(TAG,"do Service binding");
        }
        if (mBoundService != null) {
            Log.i(TAG,"Running on bound system");
            service.run(mBoundService);
        };
    }

    public int currentSize() {
        final int[] value = new int[1];
         value[0] = 0;
        waitForBinding( new IRunOnService() {
            public void run(TimerService service) {
                 value[0] = service.countTimerViews();
            }
        } );
        return value[0];

    }

    public TimerView[] getAllTimers() {
        final TimerView[][] mAllTimers = new TimerView[1][1];

        waitForBinding( new IRunOnService() {
            public void run(TimerService service) {
                 mAllTimers[0] = service.getAllTimers();
            }
        } );
        return mAllTimers[0];
    }

    interface IRunOnService {
        public void run(TimerService service) ;
    }

    private class ServiceConnectionRun implements ServiceConnection {
        private IRunOnService mRunMe;

        private ServiceConnectionRun(IRunOnService runMe) {
            mRunMe = runMe;
        }

        public void onServiceConnected(ComponentName className, IBinder service) {
            // This is called when the connection with the service has been
            // established, giving us the service object we can use to
            // interact with the service.  Because we have bound to a explicit
            // service that we know is running in our own process, we can
            // cast its IBinder to a concrete class and directly access it.
            mBoundService = ((TimerService.LocalBinder) service).getService();

            Toast.makeText(mContext, R.string.local_service_connected,
                    Toast.LENGTH_SHORT).show();
            mRunMe.run(mBoundService);
        }

        public void onServiceDisconnected(ComponentName className) {
            // This is called when the connection with the service has been
            // unexpectedly disconnected -- that is, its process crashed.
            // Because it is running in our same process, we should never
            // see this happen.
            mBoundService = null;
            Toast.makeText(mContext, R.string.local_service_disconnected,
                    Toast.LENGTH_SHORT).show();
        }
    };
}

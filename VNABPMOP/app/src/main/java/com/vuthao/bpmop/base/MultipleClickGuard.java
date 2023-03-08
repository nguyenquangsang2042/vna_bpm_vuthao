package com.vuthao.bpmop.base;

import android.os.SystemClock;
import android.util.Log;
import android.view.View;

public class MultipleClickGuard implements View.OnClickListener {
    private long mLastClickTime;

    private final int mThresholdMillis;
    private final View.OnClickListener mListener;

    public MultipleClickGuard(View.OnClickListener listener, int thresholdMillis) {
        if (listener == null) {
            Log.d("ERR MultipleClickGuard", "Null OnClickListener");
        }
        if (thresholdMillis < 0) {
            Log.d("ERR MultipleClickGuard", "Negative click threshold: " + thresholdMillis);
        }

        mListener = listener;
        mThresholdMillis = thresholdMillis;
    }

    @Override
    public void onClick(View v) {
        // Using a time threshold to prevent successive clicks.
        if (SystemClock.elapsedRealtime() - mLastClickTime < mThresholdMillis) {
            return;
        }
        mLastClickTime = SystemClock.elapsedRealtime();

        // Forward the click event to the *real* listener.
        mListener.onClick(v);
    }
}

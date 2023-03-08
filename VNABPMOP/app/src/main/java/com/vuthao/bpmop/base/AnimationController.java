package com.vuthao.bpmop.base;
import android.animation.Animator;
import android.animation.AnimatorListenerAdapter;
import android.content.Context;
import android.view.View;
import android.view.animation.AlphaAnimation;
import android.view.animation.Animation;
import android.view.animation.AnimationUtils;
import android.view.animation.ScaleAnimation;
import android.view.animation.TranslateAnimation;

import androidx.annotation.Nullable;

import com.vuthao.bpmop.R;

public class AnimationController {

    private static final int SORT_TIME = 300;
    public static AnimationController share = new AnimationController();

    // slide the view from below itself to the current position
    public void slideUp(final View view) {
        view.animate().translationYBy(-view.getHeight())
                .setDuration(SORT_TIME)
                .setListener(new AnimatorListenerAdapter() {
                    @Override
                    public void onAnimationEnd(Animator animation) {
                        super.onAnimationEnd(animation);
                        view.setVisibility(View.GONE);
                    }
                });
    }

    // slide the view from below itself to the current position
    public void slideUp(final View view, final CallbackSlideListener callback) {
        view.animate().translationYBy(-view.getHeight())
                .setDuration(SORT_TIME)
                .setListener(new AnimatorListenerAdapter() {
                    @Override
                    public void onAnimationEnd(Animator animation) {
                        super.onAnimationEnd(animation);
                        view.setVisibility(View.GONE);
                        if (callback != null) callback.OnSlideFinished();
                    }

                    @Override
                    public void onAnimationStart(Animator animation) {
                        super.onAnimationStart(animation);
                        if (callback != null) callback.OnSlideStarted();
                    }
                });
    }

    // slide the view from below itself to the current position
    public void slideLeft(final View view, final CallbackSlideListener callback) {
        int fullSc = Functions.share.getScreenWidth();
        view.animate().translationXBy((float) -(fullSc - (fullSc / 2.5)))
                .setDuration(SORT_TIME)
                .setListener(new AnimatorListenerAdapter() {
                    @Override
                    public void onAnimationEnd(Animator animation) {
                        super.onAnimationEnd(animation);
                        if (callback != null) callback.OnSlideFinished();
                    }

                    @Override
                    public void onAnimationStart(Animator animation) {
                        super.onAnimationStart(animation);
                        if (callback != null) callback.OnSlideStarted();
                    }
                });
    }

    public void slideRight(final View view, final CallbackSlideListener callback) {
        int fullSc = Functions.share.getScreenWidth();
        view.animate().translationXBy((float) +(fullSc - (fullSc / 2.5)))
                .setDuration(SORT_TIME)
                .setListener(new AnimatorListenerAdapter() {
                    @Override
                    public void onAnimationEnd(Animator animation) {
                        super.onAnimationEnd(animation);
                        if (callback != null) callback.OnSlideFinished();
                    }

                    @Override
                    public void onAnimationStart(Animator animation) {
                        super.onAnimationStart(animation);
                        if (callback != null) callback.OnSlideStarted();
                    }
                });
    }

    // slide the view from below itself to the current position
    public void slideInRight(final View view, Context context) {
        Animation animation = AnimationUtils.loadAnimation(context, R.anim.slide_in_right);
        view.startAnimation(animation);
    }

    public void slideInLeft(final View view, Context context) {
        Animation animation = AnimationUtils.loadAnimation(context, android.R.anim.slide_in_left);
        view.startAnimation(animation);
    }

    public void slidePushDownIn(final View view, Context context) {
        Animation animation = AnimationUtils.loadAnimation(context, R.anim.slide_push_down_in);
        view.startAnimation(animation);
    }

    public void slidePushDownOut(final View view, Context context) {
        Animation animation = AnimationUtils.loadAnimation(context, R.anim.slide_push_down_out);
        view.startAnimation(animation);
    }

    public void slidePushUpIn(final View view, Context context) {
        Animation animation = AnimationUtils.loadAnimation(context, R.anim.slide_push_up_in);
        view.startAnimation(animation);
    }

    // slide the view from its current position to below itself
    public void slideDown(final View view) {
        view.setVisibility(View.VISIBLE);
        view.animate().translationYBy(view.getHeight())
                .setDuration(SORT_TIME)
                .setListener(null);
    }

    // slide the view from its current position to below itself
    public void slideDown(final View view, final CallbackSlideListener callback) {
        view.setVisibility(View.VISIBLE);
        view.animate().translationYBy(view.getHeight())
                .setDuration(SORT_TIME)
                .setListener(new AnimatorListenerAdapter() {
                    @Override
                    public void onAnimationEnd(Animator animation) {
                        super.onAnimationEnd(animation);
                        if (callback != null) callback.OnSlideFinished();
                    }

                    @Override
                    public void onAnimationStart(Animator animation) {
                        super.onAnimationStart(animation);
                        if (callback != null) callback.OnSlideStarted();
                    }
                });
    }

    public void showView(View view) {
        // Prepare the View for the animation
        showView(view, SORT_TIME);
    }

    public void showView(View view, int duration) {
        if (view.getVisibility() == View.VISIBLE) return;
        // Prepare the View for the animation
        view.setVisibility(View.VISIBLE);
        view.setAlpha(0.0f);

        // Start the animation
        view.animate()
                .alpha(1.0f)
                .setDuration(duration)
                .setListener(null);
    }

    public void hideView(final View view, int duration) {
        // Start the animation
        view.animate()
                .alpha(0.0f)
                .setDuration(duration)
                .setListener(new AnimatorListenerAdapter() {
                    @Override
                    public void onAnimationEnd(Animator animation) {
                        super.onAnimationEnd(animation);
                        view.setVisibility(View.GONE);
                    }
                });
    }

    public void hideView(final View view) {
        // Start the animation
        hideView(view, SORT_TIME);
    }

    public Animation enterAnimation() {
        Animation enterAnimation = new AlphaAnimation(0f, 1f);
        enterAnimation.setDuration(600);
        enterAnimation.setFillAfter(true);
        return enterAnimation;
    }

    public Animation exitAnimation() {
        Animation exitAnimation = new AlphaAnimation(1f, 0f);
        exitAnimation.setDuration(600);
        exitAnimation.setFillAfter(true);
        return exitAnimation;
    }

    public void scaleViewVertical(View v) {
        Animation anim = new ScaleAnimation(
                0f, 1f, // Start and end values for the X axis scaling
                1f, 1f, // Start and end values for the Y axis scaling
                Animation.RELATIVE_TO_SELF, 0f, // Pivot point of X scaling
                Animation.RELATIVE_TO_SELF, 1f); // Pivot point of Y scaling
        anim.setFillAfter(true); // Needed to keep the result of the animation
        anim.setDuration(SORT_TIME);
        v.startAnimation(anim);
    }

    public void scaleViewVertical2(View v) {
        Animation anim = new ScaleAnimation(
                0f, 1f, // Start and end values for the X axis scaling
                1f, 1f, // Start and end values for the Y axis scaling
                Animation.RELATIVE_TO_SELF, 1f, // Pivot point of X scaling
                Animation.RELATIVE_TO_SELF, 0f); // Pivot point of Y scaling
        anim.setFillAfter(true); // Needed to keep the result of the animation
        anim.setDuration(SORT_TIME);
        anim.setAnimationListener(new Animation.AnimationListener() {
            @Override
            public void onAnimationStart(Animation animation) {
                v.setVisibility(View.VISIBLE);
            }

            @Override
            public void onAnimationEnd(Animation animation) {

            }

            @Override
            public void onAnimationRepeat(Animation animation) {

            }
        });
        v.startAnimation(anim);
    }

    public void moveViewToRight(View view, @Nullable CallbackSlideListener callbackSlideListener) {
        TranslateAnimation animation = new TranslateAnimation(0, 80, 0, 0);
        animation.setDuration(SORT_TIME);
        animation.setFillAfter(true);
        animation.setAnimationListener(new Animation.AnimationListener() {
            @Override
            public void onAnimationStart(Animation animation) {
                if (callbackSlideListener != null) callbackSlideListener.OnSlideStarted();
            }

            @Override
            public void onAnimationEnd(Animation animation) {
                if (callbackSlideListener != null) callbackSlideListener.OnSlideFinished();
            }

            @Override
            public void onAnimationRepeat(Animation animation) {

            }
        });
        view.startAnimation(animation);
    }

    public void resetPoint(View view, @Nullable CallbackSlideListener callbackSlideListener) {
        TranslateAnimation animation = new TranslateAnimation(0, 0, 0, 0);
        animation.setDuration(SORT_TIME);
        animation.setFillAfter(true);
        animation.setAnimationListener(new Animation.AnimationListener() {
            @Override
            public void onAnimationStart(Animation animation) {
                if (callbackSlideListener != null) callbackSlideListener.OnSlideStarted();
            }

            @Override
            public void onAnimationEnd(Animation animation) {
                if (callbackSlideListener != null) callbackSlideListener.OnSlideFinished();
            }

            @Override
            public void onAnimationRepeat(Animation animation) {

            }
        });
        view.startAnimation(animation);
    }

    public void scaleViewHorizontal(View v) {
        Animation anim = new ScaleAnimation(
                1f, 1f, // Start and end values for the X axis scaling
                0f, 1f, // Start and end values for the Y axis scaling
                Animation.RELATIVE_TO_SELF, 0f, // Pivot point of X scaling
                Animation.RELATIVE_TO_SELF, 1f); // Pivot point of Y scaling
        anim.setFillAfter(true); // Needed to keep the result of the animation
        anim.setDuration(SORT_TIME);
        v.startAnimation(anim);
    }

    public void scaleViewReset(View v) {
        Animation anim = new ScaleAnimation(
                1.3f, 1f, // Start and end values for the X axis scaling
                1.3f, 1f, // Start and end values for the Y axis scaling
                Animation.RELATIVE_TO_SELF, 0.5f, // Pivot point of X scaling
                Animation.RELATIVE_TO_SELF, 0.5f); // Pivot point of Y scaling
        anim.setFillAfter(true); // Needed to keep the result of the animation
        anim.setDuration(SORT_TIME);
        v.startAnimation(anim);
    }

    public void scaleViewSize(View v, CallbackSlideListener callback) {
        Animation anim = new ScaleAnimation(
                1.2f, 1.5f, // Start and end values for the X axis scaling
                1.2f, 1.5f, // Start and end values for the Y axis scaling
                Animation.RELATIVE_TO_SELF, 0.5f, // Pivot point of X scaling
                Animation.RELATIVE_TO_SELF, 0.5f); // Pivot point of Y scaling
        anim.setFillAfter(true); // Needed to keep the result of the animation
        anim.setDuration(SORT_TIME);
        anim.setAnimationListener(new Animation.AnimationListener() {
            @Override
            public void onAnimationStart(Animation animation) {
                if (callback != null) callback.OnSlideStarted();
            }

            @Override
            public void onAnimationEnd(Animation animation) {
                if (callback != null) callback.OnSlideFinished();
            }

            @Override
            public void onAnimationRepeat(Animation animation) {

            }
        });
        v.startAnimation(anim);
    }

    public void blinkScale(View view) {
        Animation anim = new ScaleAnimation(
                0.2f, 1f, // Start and end values for the X axis scaling
                0.2f, 1f, // Start and end values for the Y axis scaling
                Animation.RELATIVE_TO_SELF, 0.5f, // Pivot point of X scaling
                Animation.RELATIVE_TO_SELF, 0.5f); // Pivot point of Y scaling
        anim.setDuration(200);
        anim.setRepeatMode(Animation.REVERSE);
        anim.setRepeatCount(Animation.INFINITE);
        view.startAnimation(anim);
    }

    public Animation fadeIn(Context context)
    {
        Animation anim = AnimationUtils.loadAnimation(context, R.anim.anim_clickview);
        anim.setDuration(SORT_TIME);
        return anim;
    }

    public interface CallbackSlideListener {
        void OnSlideFinished();

        void OnSlideStarted();
    }
}

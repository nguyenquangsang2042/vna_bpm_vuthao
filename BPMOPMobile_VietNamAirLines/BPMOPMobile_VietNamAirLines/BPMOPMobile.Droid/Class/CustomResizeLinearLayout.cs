using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace BPMOPMobile.Droid.Class
{
    /// <summary>
    /// Để khi view chuyển sang adjust resize
    /// </summary>
    public class CustomResizeLinearLayout : LinearLayout
    {
        private bool isKeyboardShown;
        private OnKeyboardStateChange listener;

        public interface OnKeyboardStateChange
        {
            void OnKeyboardShow();
            void OnKeyboardHide();
        }

        #region Constructor
        protected CustomResizeLinearLayout(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public CustomResizeLinearLayout(Context context) : base(context)
        {
        }

        public CustomResizeLinearLayout(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public CustomResizeLinearLayout(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public CustomResizeLinearLayout(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }
        #endregion

        #region Event
        public void SetKeyboardStateListener(OnKeyboardStateChange listener)
        {
            this.listener = listener;
        }

        public override bool DispatchKeyEventPreIme(KeyEvent e)
        {
            if (e.KeyCode == Keycode.Back)
            {
                // Keyboard is hiding
                if (isKeyboardShown)
                {
                    isKeyboardShown = false;
                    listener.OnKeyboardHide();
                }
            }
            return base.DispatchKeyEventPreIme(e);

        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            int proposedHeight = MeasureSpec.GetSize(heightMeasureSpec);
            int actualHeight = this.Height;
            if (actualHeight > proposedHeight)
            {
                // Keyboard is showing
                if (!isKeyboardShown)
                {
                    isKeyboardShown = true;
                    listener.OnKeyboardShow();
                }
            }
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace BPMOPMobile.Droid.Core.Class
{
    public class CustomEnabledHorizontalScrollView : HorizontalScrollView
    {
        private bool enableScrolling = true;

        public bool isEnableScrolling()
        {
            return enableScrolling;
        }
        public void setEnableScrolling(bool enableScrolling)
        {
            this.enableScrolling = enableScrolling;
        }
        public CustomEnabledHorizontalScrollView(Context context) : base(context)
        {
        }

        public CustomEnabledHorizontalScrollView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public CustomEnabledHorizontalScrollView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public CustomEnabledHorizontalScrollView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        protected CustomEnabledHorizontalScrollView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }
        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
           
            if (isEnableScrolling())
            {
                return base.OnInterceptTouchEvent(ev);
            }
            else
            {
                return false;
            }
        }
        public override bool OnTouchEvent(MotionEvent e)
        {
            if (isEnableScrolling())
            {
                return base.OnTouchEvent(e);
            }
            else
            {
                return false;
            }

        }
    }
}
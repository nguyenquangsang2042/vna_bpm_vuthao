using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace BPMOPMobile.Droid.Tablet.Class
{
    public class MyCustomViewPager : ViewPager
    {
        bool _flagTouchable = true;

        protected MyCustomViewPager(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            this._flagTouchable = true;
        }

        public MyCustomViewPager(Context context) : base(context)
        {
            this._flagTouchable = true;
        }

        public MyCustomViewPager(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            this._flagTouchable = true;
        }

        public void SetViewPagerTouchable(bool _flagTouchable)
        {
            this._flagTouchable = _flagTouchable;
        }
        public override bool OnTouchEvent(MotionEvent e)
        {
            return this._flagTouchable && base.OnTouchEvent(e);
        }
        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            return this._flagTouchable && base.OnInterceptTouchEvent(ev);
        }

    }
}
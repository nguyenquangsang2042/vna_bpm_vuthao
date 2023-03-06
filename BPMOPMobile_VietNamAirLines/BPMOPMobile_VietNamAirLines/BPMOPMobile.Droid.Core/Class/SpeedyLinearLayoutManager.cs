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

namespace BPMOPMobile.Droid.Core.Class
{
    public class SpeedyLinearLayoutManager : LinearLayoutManager
    {
        //private static float MILLISECONDS_PER_INCH = 5f; //default is 25f (bigger = slower)

        public SpeedyLinearLayoutManager(Context context) : base(context)
        {
        }

        public SpeedyLinearLayoutManager(Context context, int orientation, bool reverseLayout) : base(context, orientation, reverseLayout)
        {
        }

        public SpeedyLinearLayoutManager(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        protected SpeedyLinearLayoutManager(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }
        public override void SmoothScrollToPosition(RecyclerView recyclerView, RecyclerView.State state, int position)
        {
            base.SmoothScrollToPosition(recyclerView, state, position);
            LinearSmoothScroller linearSmoothScroller = new LinearSmoothScroller(recyclerView.Context)
            {
                
            };
             linearSmoothScroller.TargetPosition=position;
            StartSmoothScroll(linearSmoothScroller);
        }
    }
}
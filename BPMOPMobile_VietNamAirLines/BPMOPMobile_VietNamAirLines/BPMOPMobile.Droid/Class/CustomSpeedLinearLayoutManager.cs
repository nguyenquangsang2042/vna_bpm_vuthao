using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace BPMOPMobile.Droid.Class
{
    public class CustomSpeedLinearLayoutManager : LinearLayoutManager
    {
        private static float MILLISECONDS_PER_INCH = 100f; //default is 25f (bigger = slower)
        private int defaultExtraLayoutSpace = 600;
        private int extraLayoutSpace = -1;

        public CustomSpeedLinearLayoutManager(Context context) : base(context)
        {
        }
        public CustomSpeedLinearLayoutManager(Context context, int extraLayoutSpace) : base(context)
        {
            this.extraLayoutSpace = extraLayoutSpace;
        }

        public CustomSpeedLinearLayoutManager(Context context, int orientation, bool reverseLayout) : base(context, orientation, reverseLayout)
        {
        }

        public CustomSpeedLinearLayoutManager(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        protected CustomSpeedLinearLayoutManager(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }
        public override void SmoothScrollToPosition(RecyclerView recyclerView, RecyclerView.State state, int position)
        {
            base.SmoothScrollToPosition(recyclerView, state, position);
            // Giới hạn speed scroll
            //CustomLinearSmoothScroller customLinearSmoothScroller = new CustomLinearSmoothScroller(recyclerView.Context);
            //customLinearSmoothScroller.TargetPosition=position;
            //StartSmoothScroll(customLinearSmoothScroller);
        }
        public override int FindLastCompletelyVisibleItemPosition()
        {
            return base.FindLastCompletelyVisibleItemPosition();
        }

        public void setExtraLayoutSpace(int extraLayoutSpace)
        {
            this.extraLayoutSpace = extraLayoutSpace;
        }

        protected override int GetExtraLayoutSpace(RecyclerView.State state)
        {
            if (extraLayoutSpace > 0)
                return extraLayoutSpace;
            else 
                return defaultExtraLayoutSpace;
        }

        public class CustomLinearSmoothScroller : LinearSmoothScroller
        {
            Context mContext;
            public CustomLinearSmoothScroller(Context context) : base(context)
            {
                this.mContext = context;
            }

            public override PointF ComputeScrollVectorForPosition(int targetPosition)
            {
                return new CustomSpeedLinearLayoutManager(mContext).ComputeScrollVectorForPosition(targetPosition);
            }
            protected override float CalculateSpeedPerPixel(DisplayMetrics displayMetrics)
            {
                //return base.CalculateSpeedPerPixel(displayMetrics);
                return MILLISECONDS_PER_INCH / displayMetrics.Density;
            }

            protected override int VerticalSnapPreference => SnapToStart;
            public override int CalculateDyToMakeVisible(View view, int snapPreference)
            {
                return base.CalculateDyToMakeVisible(view, snapPreference);
            }
        }
    }
}
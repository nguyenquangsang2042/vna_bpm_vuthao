using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using BPMOPMobile.Droid.Core.Common;

namespace BPMOPMobile.Droid.Core.Class
{
    /// <summary>
    /// Expandable ListView tự động tính chiều cao khi Expand / Collapsed
    /// </summary>
    public class ExpandableListView_DynamicHeight : ExpandableListView
    {
        public ExpandableListView_DynamicHeight(Context context) : base(context) { }
        public ExpandableListView_DynamicHeight(Context context, IAttributeSet attrs) : base(context, attrs) { }
        public ExpandableListView_DynamicHeight(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { }

        public override bool CollapseGroup(int groupPos)
        {
            return base.CollapseGroup(groupPos);
        }

        public override bool ExpandGroup(int groupPos, bool animate)
        {
            return base.ExpandGroup(groupPos, animate);
        }

        public override void SetOnGroupCollapseListener(IOnGroupCollapseListener onGroupCollapseListener)
        {
            base.SetOnGroupCollapseListener(onGroupCollapseListener);
        }

        public override void SetOnGroupExpandListener(IOnGroupExpandListener onGroupExpandListener)
        {
            base.SetOnGroupExpandListener(onGroupExpandListener);
        }

        //protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        //{
        //    //int heightMeasureSpec_custom = MeasureSpec.MakeMeasureSpec((int)CmmDroidFunction.ConvertDpToPixel(200, base.Context), MeasureSpecMode.Exactly);
        //    base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        //    //iewGroup.LayoutParams @params = LayoutParameters;
        //    //@params.Height = MeasuredHeight;

        //}
    }
}
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
    /// Custom Recyclerview này giúp cho không vượt quá 3 Line của Choose User
    /// </summary>
    public class CustomFlexBoxRecyclerView : RecyclerView
    {
        int _rowHeight = -1, _maxRow = -1;
        public void SetMaxRowAndRowHeight(int _rowHeight, int _maxRow)
        {
            this._rowHeight = _rowHeight;
            this._maxRow = _maxRow;
        }
        public CustomFlexBoxRecyclerView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public CustomFlexBoxRecyclerView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
        }

        protected CustomFlexBoxRecyclerView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            if (_rowHeight != -1 && _maxRow != -1)
            {
                heightMeasureSpec = MeasureSpec.MakeMeasureSpec(_rowHeight * _maxRow, MeasureSpecMode.AtMost);
            }
            else
            {
                heightMeasureSpec = MeasureSpec.MakeMeasureSpec(95 * 3, MeasureSpecMode.AtMost);
            }
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        }

    }
}
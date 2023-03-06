using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace BPMOPMobile.Droid.Class
{
    public class CustomPrecatchingLinearLayoutManager : LinearLayoutManager
    {
        private int defaultExtraLayoutSpace = 600;
        private int extraLayoutSpace = -1;

        public CustomPrecatchingLinearLayoutManager(Context context) : base(context)
        {

        }

        public CustomPrecatchingLinearLayoutManager(Context context, int extraLayoutSpace) : base(context)
        {
            this.extraLayoutSpace = extraLayoutSpace;
        }

        public CustomPrecatchingLinearLayoutManager(Context context, int orientation, bool reverseLayout) : base(context, orientation, reverseLayout)
        {

        }

        public void setExtraLayoutSpace(int extraLayoutSpace)
        {
            this.extraLayoutSpace = extraLayoutSpace;
        }

        protected override int GetExtraLayoutSpace(RecyclerView.State state)
        {
            if (extraLayoutSpace > 0)
                return extraLayoutSpace;
            else return defaultExtraLayoutSpace;
        }
    }
}
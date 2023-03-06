using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using static Android.Support.V7.Widget.RecyclerView;

namespace BPMOPMobile.Droid.Core.Class
{
    public class CustomFlingListener : OnFlingListener
    {
        private int maxFling = 4000;

        public CustomFlingListener(int maxFling)
        {
            this.maxFling = maxFling;
        }

        public override bool OnFling(int velocityX, int velocityY)
        {
            if (velocityY > maxFling)
            {
                OnFling(velocityX, maxFling);
                return true;
            }
            else if (velocityY < -maxFling)
            {
                OnFling(velocityX, -maxFling);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
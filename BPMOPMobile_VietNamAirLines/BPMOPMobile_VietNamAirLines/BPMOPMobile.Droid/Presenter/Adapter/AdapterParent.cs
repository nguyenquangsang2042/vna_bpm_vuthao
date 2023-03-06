using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Runtime;
using Android.Support.V4.App;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Presenter.Fragment;

namespace BPMOPMobile.Droid.Presenter.Adapter
{

    public class AdapterParent : Android.Support.V4.App.FragmentStatePagerAdapter  /*FragmentStatePagerAdapter*/
    {

        public AdapterParent(Android.Support.V4.App.FragmentManager fm) : base(fm) { }
        public override int Count
        {
            get
            {
                return 4;
            }
        }
        public override int GetItemPosition(Java.Lang.Object @object)
        {
            return base.GetItemPosition(@object);
        }
        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            if (position == 0) return FragmentHomePage.newInstance();
            if (position == 1) return FragmentSingleListVDT.NewInstance();
            else if (position == 2) return FragmentSingleListVTBD_Ver2.newInstance();
            else return FragmentBoard.newInstance();
        }
    }
}
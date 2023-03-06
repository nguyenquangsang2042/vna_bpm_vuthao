using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Presenter.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPMOPMobile.Droid.Presenter.Fragment
{
    public class FragmentParent : CustomBaseFragment
    {
        private MainActivity _mainAct;
        private LayoutInflater _inflater;
        private View _rootView;
        public MyCustomViewPager _layout_parent_Viewpaper;
        private ProviderBase p_base ;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _inflater = inflater;
            _mainAct = (MainActivity)this.Activity;
            p_base = new ProviderBase();
            _mainAct._drawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
            if (_rootView == null)
            {
                _rootView = inflater.Inflate(Resource.Layout.layout_parent, null);
                _layout_parent_Viewpaper = _rootView.FindViewById<MyCustomViewPager>(Resource.Id.layout_parent_Viewpaper);
                AdapterParent adapter = new AdapterParent(_mainAct.SupportFragmentManager);
                _layout_parent_Viewpaper.BeginFakeDrag();
                _layout_parent_Viewpaper.OffscreenPageLimit = 4;
                _layout_parent_Viewpaper.Adapter = adapter;

            }
            MinionAction.RefreshFragmentParent += MinionAction_RefreshFragmentParent;
            return _rootView;
        }

        private  void MinionAction_RefreshFragmentParent(object sender, EventArgs e)
        {
            Action action = new Action(async () =>
            {
                await p_base.UpdateDataAppbase();
                AdapterParent adapter = new AdapterParent(_mainAct.SupportFragmentManager);
                _layout_parent_Viewpaper.BeginFakeDrag();
                _layout_parent_Viewpaper.OffscreenPageLimit = 4;
                _layout_parent_Viewpaper.Adapter = adapter;
            });
            new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime);
        }

        public void setViewPaperPosition(int position)
        {
            _layout_parent_Viewpaper.SetCurrentItem(position, false);
            

        }
    }
}
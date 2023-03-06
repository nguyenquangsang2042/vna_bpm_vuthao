using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Controller;
using BPMOPMobile.Droid.Presenter.SharedView;
using Newtonsoft.Json.Linq;
using Refractored.Controls;

namespace BPMOPMobile.Droid.Presenter.Fragment.FragmentChildApp
{
    public class FragmentChildAppReport : CustomBaseFragment
    {
        private MainActivity _mainAct;
        private View _rootView;
        private ImageView _imgCategory, _imgBack;
        private LinearLayout _lnBottomNavigation;
        private SwipeRefreshLayout _swipe;
        private TextView _tvTitle, _tvSubTitle;

        public BeanWorkflow _currentWorkflow;

        private ControllerHomePage CTRLHomePage = new ControllerHomePage();

        public FragmentChildAppReport() { }

        public FragmentChildAppReport(BeanWorkflow _currentWorkflow)
        {
            this._currentWorkflow = _currentWorkflow;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _mainAct = (MainActivity)this.Activity;
            _mainAct._drawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            if (_rootView == null)
            {
                _rootView = inflater.Inflate(Resource.Layout.ViewReport, null);
                _imgBack = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewReport_Back);
                _tvTitle = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewReport_Title);
                _tvSubTitle = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewReport_SubTitle);
                _swipe = _rootView.FindViewById<SwipeRefreshLayout>(Resource.Id.swipe_ViewReport);

                _lnBottomNavigation = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewReport_BottomNavigation);

                //_swipe.Refresh += Swipe_RefreshData;
                _imgBack.Click += Click_Menu;

                CmmDroidFunction.SetupSwipeRefreshLayout(_swipe);

                SetViewByLanguague();
                SetData();

            }
            else
            {
                if (MainActivity.FlagRefreshDataFragment) // Khi mở lại fragment đã có trước đó
                {
                    MainActivity.FlagRefreshDataFragment = false;
                    SetViewByLanguague();
                    SetData();
                }
            }
            CmmEvent.UpdateLangComplete += CmmEvent_ChangeLanguage;

            // Phải init lại Flag
            SharedView_BottomNavigationChildApp bottomNavigation = new SharedView_BottomNavigationChildApp(inflater, _mainAct, this, this.GetType().Name, _rootView);
            bottomNavigation.InitializeValue(_lnBottomNavigation);
            bottomNavigation.InitializeView();

            return _rootView;
        }

        private void SetViewByLanguague()
        {
            try
            {
                if (_currentWorkflow != null)
                {
                    if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                    {
                        _tvTitle.Text = !String.IsNullOrEmpty(_currentWorkflow.Title) ? _currentWorkflow.Title : "";
                        _tvSubTitle.Text = !String.IsNullOrEmpty(_currentWorkflow.Title) ? _currentWorkflow.Title : "";
                    }
                    else
                    {
                        _tvTitle.Text = !String.IsNullOrEmpty(_currentWorkflow.Title) ? _currentWorkflow.TitleEN : "";
                        _tvSubTitle.Text = !String.IsNullOrEmpty(_currentWorkflow.Title) ? _currentWorkflow.TitleEN : "";
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetViewByLanguage", ex);
#endif
            }
        }

        private void CmmEvent_ChangeLanguage(object sender, CmmEvent.UpdateEventArgs e)
        {

        }

        private void Click_Menu(object sender, EventArgs e)
        {
            try
            {
                _mainAct.HideFragment(typeof(FragmentBoard).Name);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_Menu", ex);
#endif
            }
        }

        private void SetData()
        {

        }
    }
}
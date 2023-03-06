using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.Content.Res;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Controller;
using BPMOPMobile.Droid.Presenter.Fragment;
using BPMOPMobile.Droid.Presenter.Fragment.FragmentChildApp;

namespace BPMOPMobile.Droid.Presenter.SharedView
{
    /// <summary>
    /// Bottom Navigation of BPM App
    /// </summary>
    public class SharedView_BottomNavigation : SharedViewBase
    {
        public int flagView { get; set; }
        public LinearLayout _lnContainer { get; set; }

        private ImageView _imgNavigation1 { get; set; }
        private ImageView _imgNavigation2 { get; set; }
        private ImageView _imgNavigation3 { get; set; }
        private ImageView _imgNavigation4 { get; set; }
        private ImageView _imgMore { get; set; }

        public SharedView_BottomNavigation(LayoutInflater _inflater, AppCompatActivity _mainAct, CustomBaseFragment _fragment, string _fragmentTag, View _rootView) : base(_inflater, _mainAct, _fragment, _fragmentTag, _rootView)
        {
            if (_fragment != null) // Init flagView
            {
                string _type = _fragment.GetType().Name;
                if (_type.Equals(typeof(FragmentHomePage).Name)) // Home
                {
                    flagView = (int)EnumBottomNavigationView.HomePage;
                }
                else if (_type.Equals(typeof(FragmentSingleListVDT).Name)) // VDT
                {
                    flagView = (int)EnumBottomNavigationView.SingleListVDT;
                }
                else if (_type.Equals(typeof(FragmentSingleListVTBD).Name)) // VTBD
                {
                    flagView = (int)EnumBottomNavigationView.SingleListVTBD;
                }
                else if (_type.Equals(typeof(FragmentSingleListVTBD_Ver2).Name)) // VTBD
                {
                    flagView = (int)EnumBottomNavigationView.SingleListVTBD;
                }
                else if (_type.Equals(typeof(FragmentBoard).Name) || _type.Equals(typeof(FragmentChildAppKanban).Name))
                {
                    flagView = (int)EnumBottomNavigationView.Board;
                }

            }
        }

        public void InitializeValue(LinearLayout _lnContainer)
        {
            this._lnContainer = _lnContainer;
        }

        public override void InitializeView()
        {
            try
            {
                #region Get View - Init Data
                View _view = _inflater.Inflate(Resource.Layout.ViewBottomNavigation, null);
                _imgNavigation1 = _view.FindViewById<ImageView>(Resource.Id.img_ViewBottomNavigation_Home);
                _imgNavigation2 = _view.FindViewById<ImageView>(Resource.Id.img_ViewBottomNavigation_VDT);
                _imgNavigation3 = _view.FindViewById<ImageView>(Resource.Id.img_ViewBottomNavigation_VTBD);
                _imgNavigation4 = _view.FindViewById<ImageView>(Resource.Id.img_ViewBottomNavigation_Board);
                _imgMore = _view.FindViewById<ImageView>(Resource.Id.img_ViewBottomNavigation_More);

                _imgNavigation1.SetImageResource(Resource.Drawable.icon_ver2_home);
                _imgNavigation2.SetImageResource(Resource.Drawable.icon_ver4_tome);
                _imgNavigation3.SetImageResource(Resource.Drawable.icon_ver4_fromme);
                _imgNavigation4.SetImageResource(Resource.Drawable.icon_ver3_app);

                switch (flagView)
                {
                    case (int)EnumBottomNavigationView.HomePage:
                    case (int)EnumBottomNavigationView.ChildAppHomePage:
                        {
                            _imgNavigation1.SetColorFilter((new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clVer2BlueMain))));
                            break;
                        }
                    case (int)EnumBottomNavigationView.SingleListVDT:
                    case (int)EnumBottomNavigationView.ChildAppSingleListVDT:
                        {
                            _imgNavigation2.SetColorFilter((new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clVer2BlueMain))));
                            break;
                        }
                    case (int)EnumBottomNavigationView.SingleListVTBD:
                    case (int)EnumBottomNavigationView.ChildAppSingleListVTBD:
                        {
                            _imgNavigation3.SetColorFilter((new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clVer2BlueMain))));
                            break;
                        }
                    case (int)EnumBottomNavigationView.Board:
                        //case (int)EnumBottomNavigationView.ChildAppReport:
                        {
                            _imgNavigation4.SetColorFilter((new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clVer2BlueMain))));
                            break;
                        }
                }

                #endregion

                #region Show View
                if (_lnContainer != null)
                {
                    _lnContainer.RemoveAllViews();
                    _lnContainer.AddView(_view);
                }
                #endregion

                #region Event

                _imgNavigation1.Click += Click_imgNavigation1;

                _imgNavigation2.Click += Click_imgNavigation2;

                _imgNavigation3.Click += Click_imgNavigation3;

                if (flagView != (int)EnumBottomNavigationView.Board)
                {
                    _imgNavigation4.Click += Click_imgNavigation4;
                }

                #endregion
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "InitializeView", ex);
#endif
            }
        }

        #region Event
        private void Click_imgNavigation1(object sender, EventArgs e)
        {
            try
            {
                _imgNavigation1.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                if (flagView != (int)EnumBottomNavigationView.HomePage)
                {
                    MainActivity.FlagNavigation = (int)EnumBottomNavigationView.HomePage; // Home
                    MinionAction.OnRedirectFragmentLeftMenu(null, null);
                }
                /*else
                {
                    MinionAction.OnMoveToHeadHomePage(null, null);
                }    */
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgNavigation1", ex);
#endif
            }
        }

        private void Click_imgNavigation2(object sender, EventArgs e)
        {
            try
            {
                _imgNavigation2.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                if (flagView != (int)EnumBottomNavigationView.SingleListVDT)
                {
                    MainActivity.FlagNavigation = (int)EnumBottomNavigationView.SingleListVDT;
                    MinionAction.OnRedirectFragmentLeftMenu(null, null);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgNavigation2", ex);
#endif
            }
        }

        private void Click_imgNavigation3(object sender, EventArgs e)
        {
            try
            {
                _imgNavigation3.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));
                if (flagView != (int)EnumBottomNavigationView.SingleListVTBD)
                {
                    MainActivity.FlagNavigation = (int)EnumBottomNavigationView.SingleListVTBD;
                    MinionAction.OnRedirectFragmentLeftMenu(null, null);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgNavigation3", ex);
#endif
            }
        }

        private void Click_imgNavigation4(object sender, EventArgs e)
        {
            try
            {
                _imgNavigation4.StartAnimation(ControllerAnimation.GetAnimationClick_FadeIn(_rootView.Context));

                if (flagView != (int)EnumBottomNavigationView.Board)
                {
                    MainActivity.FlagNavigation = (int)EnumBottomNavigationView.Board; // Board
                    MinionAction.OnRedirectFragmentLeftMenu(null, null);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_imgNavigation4", ex);
#endif
            }
        }

        #endregion

        private void SetViewIsSelected(LinearLayout _ln, ImageView _img, TextView _tv)
        {
            try
            {
                _ln.SetBackgroundColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clVer2BlueNavigation)));
                _img.SetColorFilter(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clVer2BlueMain)));
                _tv.SetTextColor(new Color(ContextCompat.GetColor(_mainAct, Resource.Color.clVer2BlueMain)));
                _tv.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), TypefaceStyle.Bold);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetViewIsSelected", ex);
#endif
            }
        }
    }
}
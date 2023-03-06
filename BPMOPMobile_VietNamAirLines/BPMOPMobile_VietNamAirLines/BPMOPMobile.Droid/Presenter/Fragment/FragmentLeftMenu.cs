using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Controller;
using BPMOPMobile.Droid.Presenter.Adapter;
using BPMOPMobile.Droid.Presenter.Fragment.FragmentChildApp;
using Refractored.Controls;
using SQLite;

namespace BPMOPMobile.Droid.Presenter.Fragment
{
    public class FragmentLeftMenu : CustomBaseFragment
    {
        private MainActivity _mainAct;
        private View _rootView;
        private TextView _tvWelcome, _tvSignout, _tvName,_tvMeeting, _tvEmail, _tvVDT, _tvFollow, _tvVDTCount, _tvFollowCount, _tvVTBD, _tvVTBDCount, _tvBoard, _tvHome, _tvAppVersion, _tvLanguage, _tvApplabel;
        private WebView _webAd;
        private RecyclerView _recyApp;
        private LinearLayout _lnHome, _lnVDT, _lnFollow, _lnVTBD, _lnBoard, _lnLogout, _lnApp,_lnMeeting;
        private View _viewHome, _viewVDT, _viewFollow, _viewVTBD, _viewBoard;
        private ImageView _imgSignOut, _imgVDT, _imgFollow, _imgVTBD, _imgHome, _imgBoard;
        private CircleImageView _imgAvatar;
        private Switch _switchLanguage;
        private ControllerLeftMenu CTRLLeftMenu = new ControllerLeftMenu();
        private ControllerHomePage CTRLHomePage = new ControllerHomePage();
        private AdapterLeftMenuApp _adapterListApp;
        public bool _isLocalDataLoading = true; // nếu = false là call API từ Server
        public string _queryVTBD = "";
        public string _queryVDT = "";
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _mainAct = (MainActivity)this.Activity;
            if (_rootView == null)
            {
                _rootView = inflater.Inflate(Resource.Layout.ViewLeftMenu, null);
                _tvName = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewLeftMenu_Name);
                _tvEmail = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewLeftMenu_Email);
                _tvWelcome = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewLeftMenu_Welcome);
                _tvSignout = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewLeftMenu_SignOut);
                _tvHome = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewLeftMenu_HomePage);
                _tvVTBD = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewLeftMenu_VTBD);
                _tvVTBDCount = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewLeftMenu_VTBDCount);
                _tvVDT = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewLeftMenu_VDT);
                _tvFollow = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewLeftMenu_Follow);
                _tvVDTCount = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewLeftMenu_VDTCount);
                _tvFollowCount = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewLeftMenu_FollowCount);
                _tvBoard = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewLeftMenu_Board);
                _imgSignOut = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewLeftMenu_SignOut);
                _tvLanguage = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewLeftMenu_Language);
                _imgBoard = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewLeftMenu_Board);
                _imgVTBD = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewLeftMenu_VTBD);
                _imgVDT = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewLeftMenu_VDT);
                _imgFollow = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewLeftMenu_Follow);
                _imgHome = _rootView.FindViewById<ImageView>(Resource.Id.img_ViewLeftMenu_HomePage);
                _imgAvatar = _rootView.FindViewById<CircleImageView>(Resource.Id.img_ViewLeftMenu_Avata);
                _switchLanguage = _rootView.FindViewById<Switch>(Resource.Id.sw_ViewLeftMenu_Language);
                _tvAppVersion = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewLeftMenu_InforApp);
                _tvApplabel = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewLeftMenu_InfoApp);
                _viewHome = _rootView.FindViewById<View>(Resource.Id.view_ViewLeftMenu_HomePage);
                _viewVDT = _rootView.FindViewById<View>(Resource.Id.view_ViewLeftMenu_VDT);
                _viewFollow = _rootView.FindViewById<View>(Resource.Id.view_ViewLeftMenu_Follow);
                _viewVTBD = _rootView.FindViewById<View>(Resource.Id.view_ViewLeftMenu_VTBD);
                _viewBoard = _rootView.FindViewById<View>(Resource.Id.view_ViewLeftMenu_Board);
                _webAd = _rootView.FindViewById<WebView>(Resource.Id.web_ViewLeftMenu_Advertisement);
                _recyApp = _rootView.FindViewById<RecyclerView>(Resource.Id.recy_ViewLeftMenu_App);

                _lnHome = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewLeftMenu_HomePage);
                _lnVDT = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewLeftMenu_VDT);
                _lnFollow = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewLeftMenu_Follow);
                _lnVTBD = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewLeftMenu_VTBD);
                _lnBoard = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewLeftMenu_Board);
                _lnApp = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewLeftMenu_App);
                _lnLogout = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewLeftMenu_Logout);

                _lnMeeting = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewLeftMenu_Meeting);
                _tvMeeting = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewLeftMenu_Meeting);


                _webAd.Settings.LoadWithOverviewMode = true;
                _webAd.Settings.UseWideViewPort = true;

                _switchLanguage.SetTextColor(ColorStateList.ValueOf(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clRed))));
                _switchLanguage.SetHintTextColor(ColorStateList.ValueOf(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clActionYellow))));

                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                    _switchLanguage.Checked = false;
                else
                    _switchLanguage.Checked = true;

                _switchLanguage.CheckedChange += CheckedChange_SwitchLanguage;
                _lnHome.Click += Click_lnHome;
                _lnVDT.Click += Click_lnVDT;
                _lnVTBD.Click += Click_lnVTBD;
                _lnBoard.Click += Click_lnBoard;
                _lnLogout.Click += Click_lnSignOut;
                _lnFollow.Click += _lnFollow_Click;
                _lnMeeting.Click += _lnMeeting_Click;
                SetData(_setAllData: true);
                CTRLHomePage.SetAvataForImageView(_mainAct, _imgAvatar);
                _imgHome.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clViolet)));
                MinionAction.RedirectFragmentLeftMenu += EventHandler_RedirectPage; // perform Click - dùng khi chuyển trang
                MinionAction.RenewDataAndShowFragmentLeftMenu += EventHandler_RenewDataAndShow; // Renew Data không perform Click - dùng khi show menu                
            }
            else
            {
                SetData(_setAllData: true);
                CTRLLeftMenu.SetAvataForImageView(_mainAct, _imgAvatar);
            }
            SetViewByLanguage();
            _lnApp.Visibility = ViewStates.Gone;
            return _rootView;
        }

        private void _lnMeeting_Click(object sender, EventArgs e)
        {
            _mainAct.CloseDrawer();
            FragmentMeeting frg = new FragmentMeeting();
            _mainAct.AddFragment(FragmentManager, frg, typeof(FragmentMeeting).Name, 1);
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            MinionAction.RedirectFragmentLeftMenu -= EventHandler_RedirectPage; // perform Click - dùng khi chuyển trang
            MinionAction.RenewDataAndShowFragmentLeftMenu -= EventHandler_RenewDataAndShow; // Renew Data không perform Click - dùng khi show menu
        }

        #region Event
        private void _lnFollow_Click(object sender, EventArgs e)
        {

            try
            {
                //SetViewByCurrentFragment(11);
                _mainAct.CloseDrawer();
                Action action = new Action(() =>
                {
                    Perform_RedirectPage((int)EnumBottomNavigationView.Follow);
                });
                new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime + 200);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_VDT", ex);
#endif
            }

        }

        private void SetViewByLanguage()
        {
            try
            {
                _tvWelcome.Text = CmmFunction.GetTitle("TEXT_WELCOME", "Xin chào!");
                _tvHome.Text = CmmFunction.GetTitle("TEXT_MAINVIEW", "Trang chủ");
                _tvVDT.Text = CmmFunction.GetTitle("TEXT_TOME", "Đến tôi");
                _tvVTBD.Text = CmmFunction.GetTitle("TEXT_FROMME", "Tôi bắt đầu");
                _tvBoard.Text = CmmFunction.GetTitle("TEXT_BOARD", "Ứng dụng");
                _tvFollow.Text = CmmFunction.GetTitle("TEXT_FOLLOW", "Theo dõi");
                _tvApplabel.Text = CmmFunction.GetTitle("TEXT_APPINFO", "Thông tin ứng dụng");
                _tvLanguage.Text = CmmFunction.GetTitle("TEXT_LANGUAGE", "Tiếng Việt");
                _tvSignout.Text = CmmFunction.GetTitle("TEXT_SIGNOUT", "Đăng xuất");
                _tvMeeting.Text= CmmFunction.GetTitle("TEXT_MEETING", "Lịch họp tuần");
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetViewByLanguage", ex);
#endif
            }
        }

        /// <summary>
        /// Hightlight View theo fragment hiện tại 
        /// </summary>
        private void SetViewByCurrentFragment(int _flagCurrentFragment)
        {
            try
            {
                switch (_flagCurrentFragment)
                {
                    case (int)EnumBottomNavigationView.HomePage: // Home
                        {
                            _viewHome.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clBottomEnable)));
                            _viewVDT.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _viewVTBD.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _viewBoard.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _viewFollow.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));

                            _lnHome.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraySearchUser)));
                            _lnVDT.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _lnVTBD.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _lnBoard.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _lnFollow.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));

                            CTRLLeftMenu.SetView_Selected(_mainAct, _tvHome, _imgHome);
                            CTRLLeftMenu.SetView_NotSelected(_mainAct, _tvVDT, _imgVDT, _tvVDTCount);
                            CTRLLeftMenu.SetView_NotSelected(_mainAct, _tvVTBD, _imgVTBD, _tvVTBDCount);
                            CTRLLeftMenu.SetView_NotSelected(_mainAct, _tvBoard, _imgBoard);
                            CTRLLeftMenu.SetView_NotSelected(_mainAct, _tvFollow, _imgFollow, _tvFollowCount);
                            break;
                        }
                    case (int)EnumBottomNavigationView.SingleListVDT: // VDT
                        {
                            _viewHome.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _viewVDT.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clBottomEnable)));
                            _viewVTBD.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _viewBoard.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _viewFollow.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));

                            _lnHome.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _lnVDT.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraySearchUser)));
                            _lnVTBD.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _lnBoard.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _lnFollow.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));

                            CTRLLeftMenu.SetView_NotSelected(_mainAct, _tvHome, _imgHome);
                            CTRLLeftMenu.SetView_Selected(_mainAct, _tvVDT, _imgVDT, _tvVDTCount);
                            CTRLLeftMenu.SetView_NotSelected(_mainAct, _tvVTBD, _imgVTBD, _tvVTBDCount);
                            CTRLLeftMenu.SetView_NotSelected(_mainAct, _tvBoard, _imgBoard);
                            CTRLLeftMenu.SetView_NotSelected(_mainAct, _tvFollow, _imgFollow, _tvFollowCount);
                            break;
                        }
                    case (int)EnumBottomNavigationView.SingleListVTBD: // VTBD
                        {
                            _viewHome.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _viewVDT.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _viewVTBD.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clBottomEnable)));
                            _viewBoard.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _viewFollow.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));

                            _lnHome.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _lnVDT.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _lnVTBD.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraySearchUser)));
                            _lnBoard.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _lnFollow.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));

                            CTRLLeftMenu.SetView_NotSelected(_mainAct, _tvHome, _imgHome);
                            CTRLLeftMenu.SetView_NotSelected(_mainAct, _tvVDT, _imgVDT, _tvVDTCount);
                            CTRLLeftMenu.SetView_Selected(_mainAct, _tvVTBD, _imgVTBD, _tvVTBDCount);
                            CTRLLeftMenu.SetView_NotSelected(_mainAct, _tvBoard, _imgBoard);
                            CTRLLeftMenu.SetView_NotSelected(_mainAct, _tvFollow, _imgFollow, _tvFollowCount);
                            break;
                        }
                    case (int)EnumBottomNavigationView.Board: // Board
                        {
                            _viewHome.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _viewVDT.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _viewVTBD.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _viewBoard.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clBottomEnable)));
                            _viewFollow.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));

                            _lnHome.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _lnVDT.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _lnVTBD.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _lnBoard.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraySearchUser)));
                            _lnFollow.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));


                            CTRLLeftMenu.SetView_NotSelected(_mainAct, _tvHome, _imgHome);
                            CTRLLeftMenu.SetView_NotSelected(_mainAct, _tvVDT, _imgVDT, _tvVDTCount);
                            CTRLLeftMenu.SetView_NotSelected(_mainAct, _tvVTBD, _imgVTBD, _tvVTBDCount);
                            CTRLLeftMenu.SetView_Selected(_mainAct, _tvBoard, _imgBoard);
                            CTRLLeftMenu.SetView_NotSelected(_mainAct, _tvFollow, _imgFollow, _tvFollowCount);
                            break;
                        }
                    case (int)EnumBottomNavigationView.Follow: // Follow
                        {
                            _viewHome.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _viewVDT.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _viewVTBD.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _viewBoard.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _viewFollow.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clBottomEnable)));

                            _lnHome.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _lnVDT.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _lnVTBD.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _lnBoard.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _lnFollow.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clGraySearchUser)));

                            CTRLLeftMenu.SetView_NotSelected(_mainAct, _tvHome, _imgHome);
                            CTRLLeftMenu.SetView_NotSelected(_mainAct, _tvVDT, _imgVDT, _tvVDTCount);
                            CTRLLeftMenu.SetView_NotSelected(_mainAct, _tvVTBD, _imgVTBD, _tvVTBDCount);
                            CTRLLeftMenu.SetView_NotSelected(_mainAct, _tvBoard, _imgBoard);
                            CTRLLeftMenu.SetView_Selected(_mainAct, _tvFollow, _imgFollow, _tvFollowCount);

                            break;
                        }
                    case -1:
                    default: // bỏ chọn hết
                        {
                            _viewHome.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _viewVDT.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _viewVTBD.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _viewBoard.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            _viewFollow.SetBackgroundColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clWhite)));
                            CTRLLeftMenu.SetView_NotSelected(_mainAct, _tvHome, _imgHome);
                            CTRLLeftMenu.SetView_NotSelected(_mainAct, _tvVDT, _imgVDT, _tvVDTCount);
                            CTRLLeftMenu.SetView_NotSelected(_mainAct, _tvVTBD, _imgVTBD, _tvVTBDCount);
                            CTRLLeftMenu.SetView_NotSelected(_mainAct, _tvFollow, _imgFollow, _tvFollowCount);
                            CTRLLeftMenu.SetView_NotSelected(_mainAct, _tvBoard, _imgBoard);
                            break;
                        }
                }

            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetViewByCurrentFragment", ex);
#endif
            }
        }

        private void Click_lnHome(object sender, EventArgs e)
        {
            try
            {
                SetViewByCurrentFragment(1);
                _mainAct.CloseDrawer();

                Action action = new Action(() =>
                {
                    //CustomBaseFragment frg = _mainAct.FragmentManager.FindFragmentByTag("FragmentHomePage");
                    //if (frg != null)
                    //{
                    //    MainActivity.FlagRefreshDataFragment = true;
                    //    _mainAct.HideFragment("FragmentHomePage");
                    //}
                    //else
                    //{
                    //    FragmentHomePage fragmentHomePage = new FragmentHomePage();
                    //    _mainAct.ShowFragment(FragmentManager, fragmentHomePage, "FragmentHomePage", 1);
                    //}
                    Perform_RedirectPage((int)EnumBottomNavigationView.HomePage);
                });
                new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime + 200);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_Home", ex);
#endif
            }
        }

        private void Click_lnVDT(object sender, EventArgs e)
        {
            try
            {
                SetViewByCurrentFragment(2);
                _mainAct.CloseDrawer();

                Action action = new Action(() =>
                {
                    //CustomBaseFragment frg = _mainAct.FragmentManager.FindFragmentByTag("FragmentSingleListVDT");
                    //if (frg != null)
                    //{
                    //    MainActivity.FlagRefreshDataFragment = true;
                    //    _mainAct.HideFragment("FragmentSingleListVDT");
                    //}
                    //else
                    //{
                    //    FragmentSingleListVDT FragmentSingleListVDT = new FragmentSingleListVDT();
                    //    _mainAct.ShowFragment(FragmentManager, FragmentSingleListVDT, "FragmentSingleListVDT", 1);
                    //}
                    Perform_RedirectPage((int)EnumBottomNavigationView.SingleListVDT);
                });
                new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime + 200);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_VDT", ex);
#endif
            }
        }

        private void Click_lnVTBD(object sender, EventArgs e)
        {
            try
            {
                SetViewByCurrentFragment(3);
                _mainAct.CloseDrawer();

                Action action = new Action(() =>
                {
                    //CustomBaseFragment frg = _mainAct.FragmentManager.FindFragmentByTag("FragmentSingleListVTBD");
                    //if (frg != null)
                    //{
                    //    MainActivity.FlagRefreshDataFragment = true;
                    //    _mainAct.HideFragment("FragmentSingleListVTBD");
                    //}
                    //else
                    //{
                    //    FragmentSingleListVTBD FragmentSingleListVTBD = new FragmentSingleListVTBD();
                    //    _mainAct.ShowFragment(FragmentManager, FragmentSingleListVTBD, "FragmentSingleListVTBD", 1);
                    //}
                    Perform_RedirectPage((int)EnumBottomNavigationView.SingleListVTBD);
                });
                new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime + 200);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_VTBD", ex);
#endif
            }
        }

        private void Click_lnBoard(object sender, EventArgs e)
        {
            try
            {
                SetViewByCurrentFragment(4);
                _mainAct.CloseDrawer();

                Action action = new Action(() =>
                {
                    //CustomBaseFragment frg = _mainAct.FragmentManager.FindFragmentByTag("FragmentBoard");
                    //if (frg != null)
                    //{
                    //    MainActivity.FlagRefreshDataFragment = true;
                    //    _mainAct.HideFragment("FragmentBoard");
                    //}
                    //else
                    //{
                    //    FragmentBoard fragmentBoard = new FragmentBoard();
                    //    _mainAct.ShowFragment(FragmentManager, fragmentBoard, "FragmentBoard", 1);
                    //}
                    Perform_RedirectPage((int)EnumBottomNavigationView.Board);
                });
                new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime + 200);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_VTBD", ex);
#endif
            }
        }

        private void Click_ItemRecyApp(object sender, BeanWorkflow e)
        {
            try
            {
                SetViewByCurrentFragment(-1);
                _mainAct.CloseDrawer();
                Action action = new Action(() =>
                {
                    MainActivity.ChildAppWorkflow = e;
                    Perform_RedirectPage((int)EnumBottomNavigationView.ChildAppHomePage);
                });
                new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime + 190);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData", ex);
#endif
            }
        }

        private void Click_lnSignOut(object sender, EventArgs e)
        {
            try
            {
                Action _actionPositiveButton = new Action(() =>
                {
                    _imgHome.SetColorFilter(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clViolet)));
                    _imgVTBD.SetColorFilter(null);
                    _imgVDT.SetColorFilter(null);
                    _tvHome.SetTextColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clViolet)));
                    _tvVTBD.SetTextColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clBlack)));
                    _tvVDT.SetTextColor(new Color(ContextCompat.GetColor(_rootView.Context, Resource.Color.clBlack)));
                    _mainAct.CloseDrawer();

                    Action action = new Action(() =>
                    {
                        _mainAct.SignOut();
                    });
                    new Handler().PostDelayed(action, CmmDroidVariable.M_ActionDelayTime + 200);
                });

                Action _actionNegativeButton = new Action(() =>
                {

                });

                CmmDroidFunction.ShowAlertDialogWithAction(_mainAct, CmmFunction.GetTitle("TEXT_CONFIRM_SIGNOUT", "Bạn có muốn đăng xuất khỏi tài khoản?"),
                _actionPositiveButton: new Action(() => { _actionPositiveButton(); }),
                _actionNegativeButton: new Action(() => { _actionNegativeButton(); }),
                _title: CmmDroidFunction.GetApplicationName(_rootView.Context),
                _positive: CmmFunction.GetTitle("TEXT_AGREE", "Đồng ý"),
                _negative: CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"));
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_SignOut", ex);
#endif
            }
        }

        private void CheckedChange_SwitchLanguage(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == true)
                {
                    _switchLanguage.Enabled = false;
                    if (CTRLLeftMenu.CheckAppHasConnection() == false)
                    {
                        CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại"),
                                                                   CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!"));

                        _switchLanguage.Checked = !e.IsChecked;
                        _switchLanguage.Enabled = true;
                        return;
                    }
                    else
                    {
                        if (e.IsChecked)
                        {
                            SendAPI_UpdateLanguageFromServer(CmmDroidVariable.M_SysLangEN);
                        }
                        else
                        {
                            SendAPI_UpdateLanguageFromServer(CmmDroidVariable.M_SysLangVN);
                        }
                        _switchLanguage.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Change_Language", ex);
#endif
            }
        }

        #endregion

        #region Data
        int setFollowCount()
        {

            try
            {
                List<BeanAppBaseExt> _lstAppBaseFollow = new List<BeanAppBaseExt>();
                List<BeanAppBaseExt> _lstAppBaseTemp = new List<BeanAppBaseExt>();
                SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
                string queryGetAllBeanAppBase = String.Format("select * from BeanAppBase where StatusGroup in ({0})", CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS));
                List<BeanAppBaseExt> allBeanAppBase = conn.Query<BeanAppBaseExt>(queryGetAllBeanAppBase);
                foreach (BeanAppBaseExt appbase in allBeanAppBase)
                {
                    string _workflowItemID = CmmFunction.GetWorkflowItemIDByUrl(appbase.ItemUrl);
                    string queryGetAllBeanWorkFlowFollow = String.Format("select * from BeanWorkflowFollow where WorkflowItemId = {0}", _workflowItemID);
                    List<BeanWorkflowFollow> _lstBeanWorkflowFollow = conn.Query<BeanWorkflowFollow>(queryGetAllBeanWorkFlowFollow);
                    if (_lstBeanWorkflowFollow != null && _lstBeanWorkflowFollow.Count != 0)
                    {
                        _lstAppBaseFollow.Add(appbase);
                    }


                }
                for (int i = 0; i < _lstAppBaseFollow.Count; i++)
                {
                    if (_lstAppBaseFollow[i].ResourceCategoryId.Value != 16) // Task không có follow
                    {
                        string _workflowItemID = CmmFunction.GetWorkflowItemIDByUrl(_lstAppBaseFollow[i].ItemUrl);
                        string _queryFollow = string.Format(CTRLHomePage._queryFavorite, _workflowItemID);
                        List<BeanWorkflowFollow> _lstFollow = conn.Query<BeanWorkflowFollow>(_queryFollow);
                        if (_lstFollow != null && _lstFollow.Count > 0)
                            _lstAppBaseFollow[i].IsFollow = _lstFollow[0].Status == 1 ? true : false;
                        if (_lstAppBaseFollow[i].IsFollow)
                        {
                            _lstAppBaseTemp.Add(_lstAppBaseFollow[i]);
                        }

                    }
                }
                if (_lstAppBaseTemp.Count > 0)
                {
                    return _lstAppBaseTemp.Count;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception e)
            {
                return 0;
                Console.WriteLine("LeftMenu setFollowCount error:" + e.ToString());
            }
        }

        /// <summary>
        /// Hàm SetData, truyền _setAllData = false nếu bỏ bớt 
        /// </summary>
        /// <param name="_setAllData">nếu bằng false thì bỏ bớt 1 số cái ko cần</param>
        public async void SetData(bool _setAllData)
        {
            SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
            try
            {
                if (_setAllData == true)
                {
                    if (CTRLLeftMenu.CheckAppHasConnection() == true)
                        _webAd.LoadData("<html><body><img src=\"" + CmmDroidVariable.M_DynamicBackgroundURL + "\" width=\"100%\" height=\"100%\"\"/></body></html>", "text/html", null);
                    else
                        _webAd.LoadDataWithBaseURL("file:///android_res/drawable/", "<style>img{display: inline;width: 100%;max-height: 100%;}</style> <img src='img_startview_center_003.png' />", "text/html", "utf-8", null);

                    _webAd.VerticalScrollBarEnabled = false;
                    _webAd.HorizontalScrollBarEnabled = false;
                    _webAd.Settings.LoadWithOverviewMode = true;
                    _webAd.Settings.UseWideViewPort = true;
                    _webAd.SetWebChromeClient(new WebChromeClient());
                    _webAd.SetInitialScale(1);
                    _webAd.Touch += (sender, e) => { }; // Disable scroll

                    string ver = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName;
                    if (!String.IsNullOrEmpty(ver))
                    {
                        _tvAppVersion.Text = ver;
                    }
                }

                if (!String.IsNullOrEmpty(CmmVariable.SysConfig.Title))
                    _tvName.Text = CmmVariable.SysConfig.Title;

                if (!String.IsNullOrEmpty(CmmVariable.SysConfig.PositionTitle))
                    _tvEmail.Text = CmmVariable.SysConfig.PositionTitle;

                #region Count VDT
                /* try
                 {
                     for (int i = 0; i < CTRLHomePage.LstFilterCondition_VDT.Count; i++) // Chỉ hiện đang xử lý tất cả trong app
                     {
                         switch (CTRLHomePage.LstFilterCondition_VDT[i].Key.ToLowerInvariant())
                         {
                             case "tình trạng":
                                 CTRLHomePage.LstFilterCondition_VDT[i] = new KeyValuePair<string, string>("Tình trạng", "1"); break;
                             case "hạn xử lý":  // tất cả 
                                 CTRLHomePage.LstFilterCondition_VDT[i] = new KeyValuePair<string, string>("Hạn xử lý", "1"); break;
                             case "từ ngày":
                                 CTRLHomePage.LstFilterCondition_VDT[i] = new KeyValuePair<string, string>("Từ ngày", ""); break;
                             case "đến ngày":
                                 CTRLHomePage.LstFilterCondition_VDT[i] = new KeyValuePair<string, string>("Đến ngày", ""); break;
                         }
                     }

                     string _queryVDTCount = CTRLHomePage.GetQueryStringAppBaseVDT_ByCondition(CTRLHomePage.LstFilterCondition_VDT, false, true);
                     List<CountNum> _lstVDTCount = conn.Query<CountNum>(_queryVDTCount);

                     if (_lstVDTCount != null && _lstVDTCount.Count > 0)
                         CTRLLeftMenu.SetTextview_FormatItemCount(_tvVDTCount, _lstVDTCount[0].Count, "");
                     else
                         _tvVDTCount.Text = "";
                 }
                 catch (Exception ex)
                 {
                     _tvVDTCount.Text = "";
                 }*/

                #endregion
                await Task.Run(() =>
                {
                    ProviderControlDynamic p_dynamic = new ProviderControlDynamic();
                    string CountVDTTBD = p_dynamic.GetListCountVDT_VTBD(String.Format("{0}|{1}|{2}", CmmVariable.KEY_COUNT_ASIGNTOME_INPROCESS, CmmVariable.KEY_COUNT_FROMME_INPROCESS, CmmVariable.KEY_COUNT_FOLLOW));
                    List<string> CountVDTTBD_split = CountVDTTBD.Split("|").ToList();
                    #region Count VDT Ver2
                    try
                    {

                        int _lstVDTCount = Convert.ToInt32(CountVDTTBD_split.Where(x => x.Contains(CmmVariable.KEY_COUNT_ASIGNTOME_INPROCESS)).First().Split(";#").Last());
                        _mainAct.RunOnUiThread(() =>
                        {
                            if (_lstVDTCount != 0)
                                CTRLLeftMenu.SetTextview_FormatItemCount(_tvVDTCount, _lstVDTCount, "");
                            else
                                _tvVDTCount.Text = "";
                        });
                        
                    }
                    catch (Exception ex)
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            _tvVDTCount.Text = "";
                        });
                    }

                    #endregion
                    #region Count Follow
                    int countFollow = 0;
                    countFollow = Convert.ToInt32(CountVDTTBD_split.Where(x => x.Contains(CmmVariable.KEY_COUNT_FOLLOW)).First().Split(";#").Last());
                    _mainAct.RunOnUiThread(() =>
                    {
                        if (countFollow > 0)
                        {
                            CTRLLeftMenu.SetTextview_FormatItemCount(_tvFollowCount, countFollow, "");
                        }
                        else
                        {
                            CTRLLeftMenu.SetTextview_FormatItemCount(_tvFollowCount, 0, "");
                        }
                    });
                    #endregion
                    #region Count VTBD Ver 2 
                    try
                    {
                        int _lstVTBDCount = 0;
                        _lstVTBDCount = Convert.ToInt32(CountVDTTBD_split.Where(x => x.Contains(CmmVariable.KEY_COUNT_FROMME_INPROCESS)).First().Split(";#").Last());
                        _mainAct.RunOnUiThread(() =>
                        {
                            if (_lstVTBDCount != 0)
                                CTRLLeftMenu.SetTextview_FormatItemCount(_tvVTBDCount, _lstVTBDCount, "");
                            else
                                _tvVTBDCount.Text = "";
                        });
                    }
                    catch (Exception)
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            _tvVTBDCount.Text = "";
                        });
                    }

                    #endregion
                });
                


                #region Count VTBD  
                /*try
                {
                    for (int i = 0; i < CTRLHomePage.LstFilterCondition_VTBD.Count; i++) // Chỉ hiện đang xử lý tất cả trong app
                    {
                        switch (CTRLHomePage.LstFilterCondition_VTBD[i].Key.ToLowerInvariant())
                        {
                            case "hạn xử lý":  // tất cả 
                                CTRLHomePage.LstFilterCondition_VTBD[i] = new KeyValuePair<string, string>("Hạn xử lý", "1"); break;
                            case "từ ngày":
                                CTRLHomePage.LstFilterCondition_VTBD[i] = new KeyValuePair<string, string>("Từ ngày", ""); break;
                            case "đến ngày":
                                CTRLHomePage.LstFilterCondition_VTBD[i] = new KeyValuePair<string, string>("Đến ngày", ""); break;
                        }
                    }

                    string _queryVTBDCount = CTRLHomePage.GetQueryStringAppBaseVTBD_ByCondition(CTRLHomePage.LstFilterCondition_VTBD, false, true);
                    List<CountNum> _lstVTBDCount = conn.Query<CountNum>(_queryVTBDCount);

                    if (_lstVTBDCount != null && _lstVTBDCount.Count > 0)
                        CTRLLeftMenu.SetTextview_FormatItemCount(_tvVTBDCount, _lstVTBDCount[0].Count, "");
                    else
                        _tvVTBDCount.Text = "";
                }
                catch (Exception)
                {
                    _tvVTBDCount.Text = "";
                }*/

                #endregion


                #region App
                //try
                //{
                //    string _queryWorkflow = "SELECT * FROM BeanWorkflow WHERE Favorite = 1 ORDER BY Title ASC";
                //    List<BeanWorkflow> _lstWFApp = conn.Query<BeanWorkflow>(_queryWorkflow);

                //    if (_lstWFApp != null && _lstWFApp.Count > 0)
                //    {
                //        _recyApp.Visibility = ViewStates.Visible;

                //        _adapterListApp = new AdapterLeftMenuApp(_mainAct, _rootView.Context, _lstWFApp);
                //        _adapterListApp.CustomItemClick += Click_ItemRecyApp;
                //        _recyApp.SetAdapter(_adapterListApp);
                //        _recyApp.SetLayoutManager(new LinearLayoutManager(_rootView.Context, LinearLayoutManager.Vertical, false));
                //        if (_lstWFApp.Count > 4)
                //        {
                //            //item set cứng height là 40dp, max 4 item-> 40 * 4
                //            _recyApp.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, (int)(CmmDroidFunction.ConvertDpToPixel(40, _rootView.Context) * 4));
                //        }
                //        else
                //            _recyApp.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
                //    }
                //    else
                //        _recyApp.Visibility = ViewStates.Gone;
                //}
                //catch (Exception)
                //{
                //    _recyApp.Visibility = ViewStates.Gone;
                //}

                #endregion
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData", ex);
#endif
            }
            finally
            {
                conn.Close();
            }
        }

        private async void SendAPI_UpdateLanguageFromServer(string langCode)
        {
            try
            {
                ProviderUser pUser = new ProviderUser();
                bool result;

                CmmDroidFunction.ShowProcessingDialog(_mainAct, CmmFunction.GetTitle("TEXT_WAIT", "Xin vui lòng đợi..."), CmmFunction.GetTitle("TEXT_WAIT", "Please wait a moment..."), false);

                await Task.Run(() =>
                {
                    #region Update Server Lang
                    result = pUser.UpdateUserLanguageChange(langCode);

                    if (result == false)
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            CmmDroidFunction.HideProcessingDialog();
                            CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại"),
                                           CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!"));
                        });
                        return;
                    }
                    #endregion

                    #region Get Database Lang

                    result = pUser.UpdateLangData(langCode, false, true);

                    if (result)
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            CmmVariable.SysConfig.LangCode = langCode;
                            SetViewByLanguage();
                            CmmFunction.WriteSetting();
                            CmmEvent.UpdateLangComplete_Performence(null, null);
                            CmmDroidFunction.HideProcessingDialog();
                        });
                    }
                    else
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            CmmDroidFunction.HideProcessingDialog();
                            CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại"),
                                           CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Your action failed. Please try again!"));
                        });
                    }
                    #endregion
                });
            }
            catch (Exception ex)
            {
                _mainAct.RunOnUiThread(() =>
                {
                    CmmDroidFunction.HideProcessingDialog();
                });
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "load_Language", ex);
#endif
            }
        }

        /// <summary>
        /// Hàm để Renew data và show drawer ra
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_RenewDataAndShow(object sender, EventArgs e)
        {
            try
            {
                _switchLanguage.CheckedChange -= CheckedChange_SwitchLanguage;
                if (CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN))
                    _switchLanguage.Checked = false;
                else
                    _switchLanguage.Checked = true;

                _switchLanguage.CheckedChange += CheckedChange_SwitchLanguage;

                SetData(false);
                CTRLLeftMenu.SetAvataForImageView(_mainAct, _imgAvatar);
                SetViewByLanguage();
                SetViewByCurrentFragment(MainActivity.FlagNavigation);
                _mainAct.OpenDrawer();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Update_LeftMenu", ex);
#endif
            }
        }

        /// <summary>
        /// Hàm để Redirect lại khi các trang khác gọi
        /// </summary>
        private void EventHandler_RedirectPage(object sender, EventArgs e)
        {
            try
            {
                SetViewByLanguage(); // Set Language lại phòng trường hợp có change view
                Perform_RedirectPage(); // Redirect lại
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Update_LeftMenu", ex);
#endif
            }
        }
        private void Perform_RedirectPage(int _flagNavigation = -1)
        {
            if (_flagNavigation == -1) // theo MainActivity.FlagNavigation
            {
                _flagNavigation = MainActivity.FlagNavigation;
            }

            try
            {
                switch (_flagNavigation)
                {
                    case (int)EnumBottomNavigationView.HomePage:
                        {
                            CustomBaseFragment frg = (CustomBaseFragment)_mainAct.SupportFragmentManager.FindFragmentByTag(typeof(FragmentHomePage).Name);
                            CustomBaseFragment frg_parent = (CustomBaseFragment)_mainAct.SupportFragmentManager.FindFragmentByTag(typeof(FragmentParent).Name);
                            CustomBaseFragment frg_kanban = (CustomBaseFragment)_mainAct.SupportFragmentManager.FindFragmentByTag(typeof(FragmentChildAppKanban).Name);
                            if (frg_kanban != null)
                                _mainAct.HideFragment();
                            if (frg_parent != null)
                            {
                                FragmentParent currentFrg = (FragmentParent)frg_parent;
                                currentFrg.setViewPaperPosition(0);
                            }
                            else
                            {
                                if (frg != null)
                                {
                                    MainActivity.FlagRefreshDataFragment = true;
                                    _mainAct.HideFragment(typeof(FragmentHomePage).Name);
                                }
                                else
                                {
                                    FragmentHomePage fragmentHomePage = new FragmentHomePage();
                                    _mainAct.ShowFragment(FragmentManager, fragmentHomePage, typeof(FragmentHomePage).Name, 1);
                                }
                            }
                            break;
                        }
                    case (int)EnumBottomNavigationView.SingleListVDT:
                        {
                            CustomBaseFragment frg = (CustomBaseFragment)_mainAct.SupportFragmentManager.FindFragmentByTag(typeof(FragmentSingleListVDT).Name);
                            CustomBaseFragment frg_parent = (CustomBaseFragment)_mainAct.SupportFragmentManager.FindFragmentByTag(typeof(FragmentParent).Name);
                            CustomBaseFragment frg_kanban = (CustomBaseFragment)_mainAct.SupportFragmentManager.FindFragmentByTag(typeof(FragmentChildAppKanban).Name);
                            if (frg_kanban != null)
                                _mainAct.HideFragment();
                            if (frg_parent != null)
                            {
                                FragmentParent currentFrg = (FragmentParent)frg_parent;
                                currentFrg.setViewPaperPosition(1);
                            }
                            else
                            {
                                if (frg != null)
                                {
                                    MainActivity.FlagRefreshDataFragment = true;
                                    _mainAct.HideFragment(typeof(FragmentSingleListVDT).Name);
                                }
                                else
                                {
                                    FragmentSingleListVDT FragmentSingleListVDT = new FragmentSingleListVDT();
                                    _mainAct.ShowFragment(FragmentManager, FragmentSingleListVDT, typeof(FragmentSingleListVDT).Name, 1);
                                }

                            }
                            break;
                        }
                    case (int)EnumBottomNavigationView.SingleListVTBD:
                        {
                            CustomBaseFragment frg = (CustomBaseFragment)_mainAct.SupportFragmentManager.FindFragmentByTag(typeof(FragmentSingleListVTBD_Ver2).Name);
                            CustomBaseFragment frg_parent = (CustomBaseFragment)_mainAct.SupportFragmentManager.FindFragmentByTag(typeof(FragmentParent).Name);
                            CustomBaseFragment frg_kanban = (CustomBaseFragment)_mainAct.SupportFragmentManager.FindFragmentByTag(typeof(FragmentChildAppKanban).Name);
                            if (frg_kanban != null)
                                _mainAct.HideFragment();
                            if (frg_parent != null)
                            {
                                FragmentParent currentFrg = (FragmentParent)frg_parent;
                                currentFrg.setViewPaperPosition(2);
                            }
                            else
                            {
                                if (frg != null)
                                {
                                    MainActivity.FlagRefreshDataFragment = true;
                                    _mainAct.HideFragment(typeof(FragmentSingleListVTBD_Ver2).Name);
                                }
                                else
                                {
                                    //FragmentSingleListVTBD FragmentSingleListVTBD = new FragmentSingleListVTBD();
                                    FragmentSingleListVTBD_Ver2 FragmentSingleListVTBD = new FragmentSingleListVTBD_Ver2();
                                    _mainAct.ShowFragment(FragmentManager, FragmentSingleListVTBD, typeof(FragmentSingleListVTBD_Ver2).Name, 1);
                                }
                            }
                            break;
                        }
                    case (int)EnumBottomNavigationView.Follow:
                        {
                            CustomBaseFragment frg = (CustomBaseFragment)_mainAct.SupportFragmentManager.FindFragmentByTag(typeof(FragmentSingleListFollow).Name);
                            if (frg != null)
                            {
                                MainActivity.FlagRefreshDataFragment = true;
                                _mainAct.HideFragment(typeof(FragmentSingleListFollow).Name);
                            }
                            else
                            {
                                FragmentSingleListFollow FragmentSingleListVTBD = new FragmentSingleListFollow();
                                _mainAct.ShowFragment(FragmentManager, FragmentSingleListVTBD, typeof(FragmentSingleListFollow).Name, 0);
                            }
                            break;
                        }
                    case (int)EnumBottomNavigationView.Board:
                        {
                            CustomBaseFragment frg = (CustomBaseFragment)_mainAct.SupportFragmentManager.FindFragmentByTag(typeof(FragmentBoard).Name);
                            CustomBaseFragment frg_parent = (CustomBaseFragment)_mainAct.SupportFragmentManager.FindFragmentByTag(typeof(FragmentParent).Name);
                            CustomBaseFragment frg_kanban = (CustomBaseFragment)_mainAct.SupportFragmentManager.FindFragmentByTag(typeof(FragmentChildAppKanban).Name);
                            if (frg_kanban != null)
                                _mainAct.HideFragment();
                            if (frg_parent != null)
                            {
                                FragmentParent currentFrg = (FragmentParent)frg_parent;
                                currentFrg.setViewPaperPosition(3);
                            }
                            else
                            {
                                if (frg != null)
                                {
                                    MainActivity.FlagRefreshDataFragment = true;
                                    _mainAct.HideFragment(typeof(FragmentBoard).Name);
                                }
                                else
                                {
                                    FragmentBoard fragmentBoard = new FragmentBoard();
                                    _mainAct.ShowFragment(FragmentManager, fragmentBoard, typeof(FragmentBoard).Name, 1);
                                }
                            }
                            break;
                        }
                    case (int)EnumBottomNavigationView.ChildAppHomePage:
                        {
                            CustomBaseFragment frg = (CustomBaseFragment)_mainAct.SupportFragmentManager.FindFragmentByTag(typeof(FragmentChildAppHomePage).Name);
                            if (frg != null)
                            {
                                MainActivity.FlagRefreshDataFragment = true;
                                ((FragmentChildAppHomePage)frg)._currentWorkflow = MainActivity.ChildAppWorkflow; // cập nhật lại Workflow
                                _mainAct.HideFragment(typeof(FragmentChildAppHomePage).Name, 1);
                            }
                            else
                            {
                                FragmentChildAppHomePage fragmentChildAppHomePage = new FragmentChildAppHomePage(MainActivity.ChildAppWorkflow);
                                _mainAct.ShowFragment(FragmentManager, fragmentChildAppHomePage, typeof(FragmentChildAppHomePage).Name, 0);
                            }
                            break;
                        }
                    case (int)EnumBottomNavigationView.ChildAppSingleListVDT:
                        {
                            CustomBaseFragment frg = (CustomBaseFragment)_mainAct.SupportFragmentManager.FindFragmentByTag(typeof(FragmentChildAppSingleListVDT).Name);
                            if (frg != null)
                            {
                                MainActivity.FlagRefreshDataFragment = true;
                                ((FragmentChildAppSingleListVDT)frg)._currentWorkflow = MainActivity.ChildAppWorkflow; // cập nhật lại Workflow
                                _mainAct.HideFragment(typeof(FragmentChildAppSingleListVDT).Name);
                            }
                            else
                            {
                                FragmentChildAppSingleListVDT fragmentChildAppSingleListVDT = new FragmentChildAppSingleListVDT(MainActivity.ChildAppWorkflow);
                                _mainAct.ShowFragment(FragmentManager, fragmentChildAppSingleListVDT, typeof(FragmentChildAppSingleListVDT).Name, 1);
                            }
                            break;
                        }
                    case (int)EnumBottomNavigationView.ChildAppSingleListVTBD:
                        {
                            MainActivity.FlagNavigation_ChildOptional = (int)EnumBottomNavigationView.ChildAppSingleListVTBD; // Set Flag Child Optional
                            CustomBaseFragment frg = (CustomBaseFragment)_mainAct.SupportFragmentManager.FindFragmentByTag(typeof(FragmentChildAppSingleListVTBD).Name);
                            if (frg != null)
                            {
                                MainActivity.FlagRefreshDataFragment = true;
                                ((FragmentChildAppSingleListVTBD)frg)._currentWorkflow = MainActivity.ChildAppWorkflow; // cập nhật lại Workflow
                                _mainAct.HideFragment(typeof(FragmentChildAppSingleListVTBD).Name);
                            }
                            else
                            {
                                FragmentChildAppSingleListVTBD fragmentChildAppSingleListVTBD = new FragmentChildAppSingleListVTBD(MainActivity.ChildAppWorkflow);
                                _mainAct.ShowFragment(FragmentManager, fragmentChildAppSingleListVTBD, typeof(FragmentChildAppSingleListVTBD).Name, 1);
                            }
                            break;
                        }
                    case (int)EnumBottomNavigationView.ChildAppKanban:
                        {
                            // Kanban view nặng nên tạo mới 
                            MainActivity.FlagNavigation_ChildOptional = (int)EnumBottomNavigationView.ChildAppKanban; // Set Flag Child Optional
                            CustomBaseFragment frg = (CustomBaseFragment)_mainAct.SupportFragmentManager.FindFragmentByTag(typeof(FragmentChildAppKanban).Name);
                            if (frg != null)
                            {
                                frg = new FragmentChildAppKanban(MainActivity.ChildAppWorkflow, 1);
                                ((FragmentChildAppKanban)frg)._currentWorkflow = MainActivity.ChildAppWorkflow; // cập nhật lại Workflow
                                _mainAct.AddFragment(FragmentManager, frg, typeof(FragmentChildAppKanban).Name, 1);
                            }
                            else
                            {
                                FragmentChildAppKanban fragmentChildAppKanban = new FragmentChildAppKanban(MainActivity.ChildAppWorkflow, 1);
                                _mainAct.AddFragment(FragmentManager, fragmentChildAppKanban, typeof(FragmentChildAppKanban).Name, 1);
                            }
                            break;
                        }
                    case (int)EnumBottomNavigationView.ChildAppList:
                        {
                            MainActivity.FlagNavigation_ChildOptional = (int)EnumBottomNavigationView.ChildAppList; // Set Flag Child Optional
                            CustomBaseFragment frg = (CustomBaseFragment)_mainAct.SupportFragmentManager.FindFragmentByTag(typeof(FragmentChildAppList).Name);
                            if (frg != null)
                            {
                                MainActivity.FlagRefreshDataFragment = true;
                                ((FragmentChildAppList)frg)._currentWorkflow = MainActivity.ChildAppWorkflow; // cập nhật lại Workflow
                                _mainAct.HideFragment(typeof(FragmentChildAppList).Name);
                            }
                            else
                            {
                                FragmentChildAppList fragmentList = new FragmentChildAppList(MainActivity.ChildAppWorkflow);
                                _mainAct.ShowFragment(FragmentManager, fragmentList, typeof(FragmentChildAppList).Name, 1);
                            }
                            break;
                        }
                    case (int)EnumBottomNavigationView.ChildAppReport:
                        {
                            MainActivity.FlagNavigation_ChildOptional = (int)EnumBottomNavigationView.ChildAppReport; // Set Flag Child Optional
                            CustomBaseFragment frg = (CustomBaseFragment)_mainAct.SupportFragmentManager.FindFragmentByTag(typeof(FragmentChildAppReport).Name);
                            if (frg != null)
                            {
                                MainActivity.FlagRefreshDataFragment = true;
                                ((FragmentChildAppReport)frg)._currentWorkflow = MainActivity.ChildAppWorkflow; // cập nhật lại Workflow
                                _mainAct.HideFragment(typeof(FragmentChildAppReport).Name);
                            }
                            else
                            {
                                FragmentChildAppReport fragmentReport = new FragmentChildAppReport(MainActivity.ChildAppWorkflow);
                                _mainAct.ShowFragment(FragmentManager, fragmentReport, typeof(FragmentChildAppReport).Name, 1);
                            }
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Perform_RedirectPage", ex);
#endif
            }
        }
        #endregion

    }
}

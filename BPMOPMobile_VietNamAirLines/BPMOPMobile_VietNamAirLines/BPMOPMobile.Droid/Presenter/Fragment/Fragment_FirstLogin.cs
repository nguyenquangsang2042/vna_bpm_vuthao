using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Controller;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BPMOPMobile.Droid.Presenter.Fragment
{

    public class Fragment_FirstLogin : CustomBaseFragment, IDialogInterfaceOnClickListener
    {
        private ControllerBase CTRLBase = new ControllerBase();
        private MainActivity _mainAct;
        private View _rootView;
        private CardView _btnSignIn, _btnSignUp;
        private FragmentStartView _fragmentStartView;
        private TextView _callhotline, _openFacebokVNA,_openWebVNA;
        public void OnClick(IDialogInterface dialog, int which)
        {
            throw new NotImplementedException();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _rootView = inflater.Inflate(Resource.Layout.Layout_FirstLoginxml, null);
            _mainAct = (MainActivity)this.Activity;
            if (_mainAct._drawerLayout != null)
            {
                _mainAct._drawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            }
            _btnSignIn = _rootView.FindViewById<CardView>(Resource.Id.btnSignIn);
            _btnSignUp = _rootView.FindViewById<CardView>(Resource.Id.btnSignUp);
            _callhotline = _rootView.FindViewById<TextView>(Resource.Id.callhotline);
            _openFacebokVNA = _rootView.FindViewById<TextView>(Resource.Id.openFacebokVNA);
            _openWebVNA = _rootView.FindViewById<TextView>(Resource.Id.openWebVNA);
            _callhotline.Click += _callhotline_Click;
            _openWebVNA.Click += _openWebVNA_Click;
            _openFacebokVNA.Click += _openFacebokVNA_Click; ;
            CTRLBase.RequestAppPermission(_mainAct);
            _fragmentStartView = new FragmentStartView();
            _fragmentStartView.InitApplicationVariable(_mainAct);
            checkVer();
            

            _btnSignIn.Click += _btnSignIn_Click;
            _btnSignUp.Click += _btnSignUp_Click;
            enableClickBottomView(false);
            return _rootView;
        }
        private bool ValidateAppVersion(ProviderUser pUser)
        {
            bool _result = true;
            try
            {
                if (pUser == null)
                    pUser = new ProviderUser();

                string _localVersion = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName;
                string _serverVersion = _localVersion; // default cho bằng Local vì Server luôn >= local

                List<BeanSettings> lstSetting = pUser.GetCustomerAppVersion(); // gọi API lấy Version
                if (lstSetting != null && lstSetting.Count > 0)
                {
                    List<BeanSettings> _AndroidVer = lstSetting.Where(x => x.KEY == "Android_AppVer").ToList();
                    if (_AndroidVer != null && _AndroidVer.Count > 0)
                    {
                        _serverVersion = _AndroidVer[0].VALUE;
                        if (!_localVersion.Equals(_serverVersion)) // Nếu khác Ver -> compare
                            _result = !CmmFunction.CheckIsNewVer(_serverVersion, _localVersion);
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "ValidateAppVersion", ex);
#endif
            }
            return _result;
        }
        void checkVer()
        {
            #region Validate Version -- Tạm đóng vì Server chưa có
            ProviderUser pUser = new ProviderUser();

            if (ValidateAppVersion(pUser) == false) // Version hợp lệ
            {
                _mainAct.RunOnUiThread(() =>
                {
                    Action _actionPositiveButton = new Action(() =>
                    {
                        if (File.Exists(CmmVariable.M_DataPath))
                            File.Delete(CmmVariable.M_DataPath);
                        var uri = Android.Net.Uri.Parse("https://play.google.com/store/apps/details?id=com.vuthao.BPM.VNABPM");
                        Intent intent = new Intent(Intent.ActionView, uri);
                        StartActivity(intent);
                    });

                    Action _actionNegativeButton = new Action(() =>
                    {
                        // đã có Dispose trong hàm
                        _mainAct.Finish();
                    });

                    CmmDroidFunction.ShowAlertDialogWithAction(_mainAct, CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? "Phiên bản mới đã có, vui lòng cập nhật để có được sự mượt mà và ổn định" : "The new version is available, please update to get smoother and more stable",
                    _actionPositiveButton: new Action(() => { _actionPositiveButton(); }),
                    _actionNegativeButton: new Action(() => { _actionNegativeButton(); }),
                    _title: CmmDroidFunction.GetApplicationName(_rootView.Context),
                    _positive: CmmFunction.GetTitle("TEXT_AGREE", "Đồng ý"),
                    _negative: CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"));
                });
            }
            else
            {
                bool relogin = CmmFunction.ReadSetting();
                if (relogin)
                {
                    _mainAct.HideFragment();
                    FragmentStartView fragmentStartView = new FragmentStartView();
                    _mainAct.ShowFragment(FragmentManager, fragmentStartView, "FragmentStartView");
                }
            }
            #endregion
        }
        private void enableClickBottomView(bool enable)
        {
            _callhotline.Enabled = enable;
            _openFacebokVNA.Enabled = enable;
            _openWebVNA.Enabled = enable;
        }
        private void _openFacebokVNA_Click(object sender, EventArgs e)
        {
            openWeb("https://www.facebook.com/VietnamAirlines/");
        }

        private void _openWebVNA_Click(object sender, EventArgs e)
        {
            openWeb("https://www.vietnamairlines.com/");
        }
        private void openWeb(string url)
        {
            Intent browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(url));
            StartActivity(browserIntent);
        }
        private void _callhotline_Click(object sender, EventArgs e)
        {
            string numberLink = "tel:" + _callhotline.Text.ToString();
            var uri = Android.Net.Uri.Parse(numberLink);
            var intent = new Intent(Intent.ActionDial, uri);
            StartActivity(intent);
        }

        private void _btnSignUp_Click(object sender, EventArgs e)
        {
            FragmentSignUp fragmentSignUp = new FragmentSignUp(0);
            _mainAct.ShowFragment(FragmentManager, fragmentSignUp, "FragmentSignUp", 1);
        }

        private void _btnSignIn_Click(object sender, EventArgs e)
        {
            FragmentStartView firstLogin = new FragmentStartView();
            _mainAct.ShowFragment(FragmentManager, firstLogin, "FragmentStartView", 1);
        }
        
    }
}
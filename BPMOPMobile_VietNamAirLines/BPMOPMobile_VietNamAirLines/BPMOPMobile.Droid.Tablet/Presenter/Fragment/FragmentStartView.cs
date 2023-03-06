using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.Droid.Tablet.Class;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Droid.Core.Controller;
using static Android.Provider.Settings;
using Firebase.Iid;
using Newtonsoft.Json;
using BPMOPMobile.Droid.Presenter.Fragment;

namespace BPMOPMobile.Droid.Tablet.Presenter.Fragment
{
    [Obsolete]
    public class FragmentStartView : Android.App.Fragment
    {
        private ControllerStartView CTRLStartView = new ControllerStartView();
        private MainActivity _mainAct;
        private View _rootView;
        private LinearLayout _lnName, _lnPass, _lnLogin;
        private TextView _tvLogin, _tvNote, _tvStatus;
        private EditText _edtUser, _edtPass;
        private ProgressBar _progess;
        private bool _checklogin, _checkAutoLogin;
        private long _lastClickTime;
        private string _loginName = string.Empty;
        private string _loginPass = string.Empty;
        private string _token = "";

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public override void OnDestroyView()
        {
            base.OnDestroyView();
            _mainAct.MDrawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _rootView = inflater.Inflate(Resource.Layout.ViewStartView, null);
            _mainAct = (MainActivity)this.Activity;
            if (_mainAct.MDrawerLayout != null)
            {
                _mainAct.MDrawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            }
            _mainAct.Window.SetNavigationBarColor(Android.Graphics.Color.Black);
            
            _tvLogin = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewStartView_Login);
            _tvNote = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewStartView_Note);
            _tvStatus = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewStartView_Status);
            _edtPass = _rootView.FindViewById<EditText>(Resource.Id.edt_ViewStartView_Pass);
            _edtUser = _rootView.FindViewById<EditText>(Resource.Id.edt_ViewStartView_Username);
            _progess = _rootView.FindViewById<ProgressBar>(Resource.Id.progess_ViewStartView);
            _lnName = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewStartView_Name);
            _lnPass = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewStartView_Pass);
            _lnLogin = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewStartView_Login);
            _tvNote.Visibility = ViewStates.Gone;
            _progess.Visibility = ViewStates.Visible;
            _lnLogin.Visibility = ViewStates.Invisible;
            _lnName.Visibility = ViewStates.Invisible;
            _lnPass.Visibility = ViewStates.Invisible;
            _tvLogin.Visibility = ViewStates.Invisible;
            _edtPass.EditorAction += NotEnter_pass;
            _edtUser.EditorAction += NotEnter_user;
            _edtUser.ImeOptions = ImeAction.Go;
            _edtPass.ImeOptions = ImeAction.Go;
            CTRLStartView.RequestAppPermission(_mainAct);
            CheckLogin();
            SetViewByLanguage();
            _tvLogin.Click += Click_tvLogin;
            return _rootView;
        }

        #region Event
        private void SetViewByLanguage()
        {
            try
            {
                if (File.Exists(CmmVariable.M_DataLangPath))
                {
                    if (CmmVariable.SysConfig.LangCode == "VN")
                    {
                        _edtPass.Hint = CmmFunction.GetTitle("K_Pass", "Mật khẩu");
                        _edtUser.Hint = CmmFunction.GetTitle("K_Uname", "Tên truy cập");
                        _tvLogin.Text = CmmFunction.GetTitle("K_Login", "Đăng nhập");
                        _tvStatus.Text = CmmFunction.GetTitle("K_Mess_LoginTitle", "Đăng nhập tài khoản của bạn");
                    }
                    else
                    {
                        _edtPass.Hint = CmmFunction.GetTitle("K_Pass", "Password");
                        _edtUser.Hint = CmmFunction.GetTitle("K_Uname", "Username");
                        _tvLogin.Text = CmmFunction.GetTitle("K_Login", "Login");
                        _tvStatus.Text = CmmFunction.GetTitle("K_Mes_LoginTitle", "Sign in to your account");
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentStartView - SetViewByLanguage - Error: " + ex.Message);
#endif
            }
        }
        private void Click_tvLogin(object sender, EventArgs e)
        {
            try
            {
                if (SystemClock.ElapsedRealtime() - _lastClickTime < 1000)
                {
                    return;
                }
                _lastClickTime = SystemClock.ElapsedRealtime();
                _edtPass.Enabled = false;
                _edtUser.Enabled = false;
                _tvLogin.Enabled = false;
                Login();
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentStartView - Click_tvLogin - Error: " + ex.Message);
#endif
            }
        }
        private void NotEnter_user(object sender, TextView.EditorActionEventArgs e)
        {
            if (e.ActionId == ImeAction.Done)
            {
                _edtPass.Focusable = true;
                _edtPass.FocusableInTouchMode = true;
                _edtPass.RequestFocus();
            }
            if (e.ActionId == ImeAction.Next)
            {
                _edtPass.Focusable = true;
                _edtPass.FocusableInTouchMode = true;
                _edtPass.RequestFocus();
            }
            if (e.ActionId == ImeAction.Go)
            {
                _edtPass.Focusable = true;
                _edtPass.FocusableInTouchMode = true;
                _edtPass.RequestFocus();
            }
        }
        private void NotEnter_pass(object sender, TextView.EditorActionEventArgs e)
        {
            if (e.ActionId == ImeAction.Done)
            {
                Login();
            }
            if (e.ActionId == ImeAction.Next)
            {
                Login();
            }
            if (e.ActionId == ImeAction.Go)
            {
                Login();
            }
        }
        #endregion

        #region Data
        private async void CheckLogin()
        {
            try
            {
                _tvNote.Visibility = ViewStates.Gone;
                _progess.Visibility = ViewStates.Visible;
                string appPath = _mainAct.FilesDir.ToString();
                if (!CmmVariable.M_DataPath.StartsWith(appPath))
                {
                    CmmVariable.M_DataPath = appPath + Path.PathSeparator + CmmVariable.M_DataPath;
                    CmmVariable.M_settingFileName = appPath + Path.PathSeparator + CmmVariable.M_settingFileName;
                    CmmVariable.M_Avatar = appPath + Path.PathSeparator + CmmVariable.M_Avatar;
                    CmmVariable.M_Folder_Avatar = appPath + "/" + CmmVariable.M_Folder_Avatar;
                    CmmVariable.M_DataLangPath = appPath + Path.PathSeparator + CmmVariable.M_DataLangPath;
                    CmmVariable.M_DataFolder = appPath + "/" + CmmVariable.M_DataFolder;
                    CmmVariable.M_AvatarCus = appPath + Path.PathSeparator + CmmVariable.M_AvatarCus;
                    if (!Directory.Exists(CmmVariable.M_Folder_Avatar))
                    {
                        Directory.CreateDirectory(CmmVariable.M_Folder_Avatar);
                    }
                    if (!Directory.Exists(CmmVariable.M_DataFolder))
                    {
                        Directory.CreateDirectory(CmmVariable.M_DataFolder);
                    }
#if DEBUG

                    if (CmmDroidFunction.CheckSdCard())
                    {
                        CmmVariable.M_LogPath = appPath + "/" + CmmVariable.M_LogPath;
                    }
                    else
                    {
                        CmmVariable.M_LogPath = appPath + "/" + CmmVariable.M_LogPath;
                    }
#else
                    if (CmmDroidFunction.CheckSdCard())
                    {
                        CmmVariable.M_LogPath = appPath + "/" + CmmVariable.M_LogPath;
                    }
                    else
                    {
                        CmmVariable.M_LogPath = appPath + "/" + CmmVariable.M_LogPath;
                    }
#endif

                }
                CmmVariable.SysConfig = new ConfigVariable();
                HttpClient client = new HttpClient();
                client.BaseAddress = new System.Uri(CmmVariable.M_Domain);
                CmmVariable.M_AuthenticatedHttpClient = client;
                ProviderUser pUser = new ProviderUser();
                List<BeanSettings> lstSetting;
                await Task.Run(() =>
                {
                    lstSetting = pUser.GetCustomerAppVersion();
                    var appVer = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName;
                    string stringAppVer = "1.0.0";
                    string serverConfigAppVersion = "1.0.0";
                    if (lstSetting != null && lstSetting.Count > 0)
                    {
                        var item = lstSetting.Where(i => i.KEY == "Android_AppVer").ToList();
                        if (item.Count > 0)
                        {
                            serverConfigAppVersion = item[0].VALUE;
                        }
                    }

                    stringAppVer = appVer;
                    if (CmmFunction.CheckIsNewVer(serverConfigAppVersion, stringAppVer)) //appVer < serverVer
                    {
                        _mainAct.RunOnUiThread(() =>
                        {
                            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(_mainAct);
                            alert.SetTitle("BPMOP");
                            if (CmmVariable.SysConfig.LangCode == "VN")
                            {
                                alert.SetMessage(CmmFunction.GetTitle("K_Mess_NewVersion", "Phiên bản mới đã có, vui lòng cập nhật để có được sự mượt mà và ổn định"));
                                alert.SetNegativeButton(CmmFunction.GetTitle("K_Agree", "Đồng ý"), (senderAlert, args) =>
                                {
                                    if (File.Exists(CmmVariable.M_DataPath))
                                        File.Delete(CmmVariable.M_DataPath);
                                    var uri = Android.Net.Uri.Parse("https://play.google.com/store/apps/details?id=com.VuThao.EuroWindow");
                                    Intent intent = new Intent(Intent.ActionView, uri);
                                    StartActivity(intent);
                                    alert.Dispose();
                                });
                            }
                            else
                            {
                                alert.SetMessage(CmmFunction.GetTitle("K_Mess_NewVersion", "The new version is available, please update to get smoother and more stable"));
                                alert.SetNegativeButton(CmmFunction.GetTitle("K_Agree", "Agree"), (senderAlert, args) =>
                                {
                                    if (File.Exists(CmmVariable.M_DataPath))
                                        File.Delete(CmmVariable.M_DataPath);
                                    var uri = Android.Net.Uri.Parse("https://play.google.com/store/apps/details?id=com.VuThao.EuroWindow");
                                    Intent intent = new Intent(Intent.ActionView, uri);
                                    StartActivity(intent);
                                    alert.Dispose();
                                });
                            }
                            Dialog dialog = alert.Create();
                            dialog.SetCanceledOnTouchOutside(false);
                            dialog.SetCancelable(false);
                            dialog.Show();
                        });
                    }
                    else
                    {
                        CmmEvent.ReloginRequest += CmmEvent_ReloginRequest;

                        bool flgClearData = false;
                        bool flgExistConfig = CmmFunction.ReadSetting();
                        if (CmmVariable.M_RenewDB)
                        {
                            // Truong dau tien chua thiet lap config
                            if (!flgExistConfig || string.IsNullOrEmpty(CmmVariable.SysConfig.AppConfigVersion))
                            {
                                if (File.Exists(CmmVariable.M_DataPath))
                                {
                                    flgClearData = true;
                                }
                            }
                            else
                            {
                                if (CmmVariable.SysConfig.AppConfigVersion != appVer)
                                {
                                    flgClearData = true;
                                }
                            }
                        }
                        if (flgClearData)
                        {
                            try
                            {
                                File.Delete(CmmVariable.M_DataPath);
                            }
                            catch (Exception ex)
                            {
#if DEBUG
                                Console.WriteLine("Author: khoahd - FragmentStartView - CheckLogin - Error: " + ex.Message);
#endif
                            }
                        }
                        CmmVariable.SysConfig.AppConfigVersion = appVer;
                        if (flgExistConfig)
                        {
                            if (!File.Exists(CmmVariable.M_DataPath))
                            {
                                CmmFunction.InstanceDB(CmmVariable.M_DataPath);
                                _checklogin = true;
                            }
                            else
                            {
                                ////_mainAct.RunOnUiThread(() =>
                                ////{
                                ////    _checkAutoLogin = true;
                                ////    FragmentHomePage homePage = new FragmentHomePage(true, _checkAutoLogin);
                                ////    _mainAct.ShowFragment(FragmentManager, homePage, "FragmentHomePage", 1);
                                ////});
                            }
                            _lnName.Visibility = ViewStates.Invisible;
                            _lnPass.Visibility = ViewStates.Invisible;
                            _tvLogin.Visibility = ViewStates.Invisible;
                            //kiem tra quyen user
                            _loginName = CmmVariable.SysConfig.LoginName; //login name = email
                            _loginPass = CmmVariable.SysConfig.LoginPassword;

                            CmmVariable.SysConfig.DataLimitDay = 30;

                            if (CmmVariable.SysConfig != null && !string.IsNullOrEmpty(_loginName) && !string.IsNullOrEmpty(_loginPass))//có user default
                            {

                                if (CTRLStartView.CheckAppHasConnection())
                                {

                                    string getCurrentUserUrl = CmmVariable.M_Domain.TrimEnd('/') + (new BeanUser().GetCurrentUserUrl());
                                    getCurrentUserUrl = getCurrentUserUrl.Replace("<#SiteName#>", "");
                                    CmmVariable.M_AuthenticatedHttpClient = CmmFunction.Login(getCurrentUserUrl, _loginName, _loginPass, true, 1);
                                    if (CmmVariable.M_AuthenticatedHttpClient != null) // ket noi voi server, authent thanh cong
                                    {

                                    }
                                    else//auto login false
                                    {

                                        _mainAct.RunOnUiThread(() =>
                                        {
                                            _checklogin = true;
                                            CmmEvent.ReloginRequest -= CmmEvent_ReloginRequest;
                                            _mainAct.SignOut();
                                            //ln_name.Visibility = ViewStates.Visible;
                                            //ln_pass.Visibility = ViewStates.Visible;
                                            //tv_login.Visibility = ViewStates.Visible;
                                            //ln_name.Enabled = true;
                                            //ln_pass.Enabled = true;
                                            //tv_note.Visibility = ViewStates.Visible;
                                            //progess.Visibility = ViewStates.Gone;
                                            //tv_note.Text = "Thông tin đăng nhập không chính xác, vui lòng thử lại.";
                                        });
                                    }
                                }
                                else
                                {
                                    _mainAct.RunOnUiThread(() =>
                                    {
                                        _lnLogin.Visibility = ViewStates.Visible;
                                        _lnName.Visibility = ViewStates.Visible;
                                        _lnPass.Visibility = ViewStates.Visible;
                                        _tvLogin.Visibility = ViewStates.Visible;
                                        _lnName.Enabled = true;
                                        _lnPass.Enabled = true;
                                        _tvNote.Visibility = ViewStates.Visible;
                                        _progess.Visibility = ViewStates.Gone;
                                        if (CmmVariable.SysConfig.LangCode == "VN")
                                        {
                                            _tvNote.Text = CmmFunction.GetTitle("K_Offline", "Bạn đang ở chế độ offline.");
                                        }
                                        else
                                        {
                                            _tvNote.Text = CmmFunction.GetTitle("K_Offline", "You are in offline mode");
                                        }
                                    });
                                }
                            }
                            else //không có thông user default , chuyển sang view login
                            {
                                _mainAct.RunOnUiThread(() =>
                                {
                                    _lnLogin.Visibility = ViewStates.Visible;
                                    _lnName.Visibility = ViewStates.Visible;
                                    _lnPass.Visibility = ViewStates.Visible;
                                    _tvLogin.Visibility = ViewStates.Visible;
                                    _lnName.Enabled = true;
                                    _lnPass.Enabled = true;
                                    _tvNote.Visibility = ViewStates.Gone;
                                    _progess.Visibility = ViewStates.Gone;

                                });
                                if (CTRLStartView.CheckAppHasConnection())
                                {
                                    if (!File.Exists(CmmVariable.M_DataPath))
                                    {
                                        CmmFunction.InstanceDB(CmmVariable.M_DataPath);
                                    }
                                    else
                                    {
                                        File.Delete(CmmVariable.M_DataPath);
                                        CmmFunction.InstanceDB(CmmVariable.M_DataPath);
                                    }
                                }
                                else
                                {
                                    _mainAct.RunOnUiThread(() =>
                                    {
                                        _lnLogin.Visibility = ViewStates.Visible;
                                        _lnName.Visibility = ViewStates.Visible;
                                        _lnPass.Visibility = ViewStates.Visible;
                                        _tvLogin.Visibility = ViewStates.Visible;
                                        _lnName.Enabled = true;
                                        _lnPass.Enabled = true;
                                        _tvNote.Visibility = ViewStates.Visible;
                                        _progess.Visibility = ViewStates.Gone;
                                        if (CmmVariable.SysConfig.LangCode == "VN")
                                        {
                                            _tvNote.Text = CmmFunction.GetTitle("K_Mess_CheckInternet", "Kiểm tra kết nối Internet.");
                                        }
                                        else
                                        {
                                            _tvNote.Text = CmmFunction.GetTitle("K_Mess_CheckInternet", "Check your Internet connection.");
                                        }
                                    });
                                }
                            }
                        }
                        else// không có thông tin cấu hình,. chuyển sang view login
                        {
                            _mainAct.RunOnUiThread(() =>
                            {
                                _lnLogin.Visibility = ViewStates.Visible;
                                _lnName.Visibility = ViewStates.Visible;
                                _lnPass.Visibility = ViewStates.Visible;
                                _tvLogin.Visibility = ViewStates.Visible;
                                _lnName.Enabled = true;
                                _lnPass.Enabled = true;
                                _tvNote.Visibility = ViewStates.Gone;
                                _progess.Visibility = ViewStates.Gone;

                            });
                            if (CTRLStartView.CheckAppHasConnection())
                            {
                                if (!File.Exists(CmmVariable.M_DataPath))
                                {
                                    CmmFunction.InstanceDB(CmmVariable.M_DataPath);
                                }
                                else
                                {
                                    File.Delete(CmmVariable.M_DataPath);
                                    CmmFunction.InstanceDB(CmmVariable.M_DataPath);
                                }
                            }
                            else
                            {
                                _mainAct.RunOnUiThread(() =>
                                {
                                    _lnLogin.Visibility = ViewStates.Visible;
                                    _lnName.Visibility = ViewStates.Visible;
                                    _lnPass.Visibility = ViewStates.Visible;
                                    _tvLogin.Visibility = ViewStates.Visible;
                                    _lnName.Enabled = true;
                                    _lnPass.Enabled = true;
                                    _tvNote.Visibility = ViewStates.Visible;
                                    _progess.Visibility = ViewStates.Gone;
                                    if (CmmVariable.SysConfig.LangCode == "VN")
                                    {
                                        _tvNote.Text = CmmFunction.GetTitle("K_Mess_CheckInternet", "Kiểm tra kết nối Internet.");
                                    }
                                    else
                                    {
                                        _tvNote.Text = CmmFunction.GetTitle("K_Mess_CheckInternet", "Check your Internet connection.");
                                    }
                                });
                            }
                        }

                    }
                });
            }
            catch (Exception)
            {
                _mainAct.RunOnUiThread(() =>
                {
                    _mainAct.RunOnUiThread(() =>
                    {
                        _lnLogin.Visibility = ViewStates.Visible;
                        _lnName.Visibility = ViewStates.Visible;
                        _lnPass.Visibility = ViewStates.Visible;
                        _tvLogin.Visibility = ViewStates.Visible;
                        _lnName.Enabled = true;
                        _lnPass.Enabled = true;
                        _tvNote.Visibility = ViewStates.Visible;
                        _progess.Visibility = ViewStates.Gone;
                        if (CmmVariable.SysConfig.LangCode == "VN")
                        {
                            _tvNote.Text = CmmFunction.GetTitle("K_Mess_loginFalse", "Thông tin đăng nhập không chính xác, vui lòng thử lại.");
                        }
                        else
                        {
                            _tvNote.Text = CmmFunction.GetTitle("K_Mess_loginFalse", "Login information is incorrect, please try again");
                        }
                    });
                });
            }
        }
        private async void Login()
        {
            try
            {
                CmmDroidFunction.HideSoftKeyBroad(_edtPass, _mainAct);
                if ((string.IsNullOrEmpty(_edtPass.Text) || (string.IsNullOrEmpty(_edtUser.Text))))
                {
                    _tvNote.Visibility = ViewStates.Visible;
                    if (CmmVariable.SysConfig.LangCode == "VN")
                    {
                        _tvNote.Text = CmmFunction.GetTitle("K_Mess_UsserOrPass", "Vui lòng nhập tên đăng nhập hoặc mật khẩu.");
                    }
                    else
                    {
                        _tvNote.Text = CmmFunction.GetTitle("K_Mess_UsserOrPass", "Please enter your username or password");
                    }
                    _progess.Visibility = ViewStates.Gone;
                    _edtPass.Enabled = true;
                    _edtUser.Enabled = true;
                    _tvLogin.Enabled = true;
                }
                else
                {
                    _tvNote.Visibility = ViewStates.Gone;
                    _progess.Visibility = ViewStates.Visible;
                    _loginName = _edtUser.Text.TrimEnd();
                    _loginPass = _edtPass.Text.TrimEnd();
                    CmmVariable.SysConfig.DataLimitDay = 30;
                    RegisDeviceInfo();
                    //CmmEvent.ReloginRequest += CmmEvent_ReloginRequest;
                    await Task.Run(() =>
                    {
                        string getCurrentUserUrl = CmmVariable.M_Domain.TrimEnd('/') + (new BeanUser().GetCurrentUserUrl());
                        getCurrentUserUrl = getCurrentUserUrl.Replace("<#SiteName#>", "");
                        if ((CmmVariable.M_AuthenticatedHttpClient = CmmFunction.Login(getCurrentUserUrl, _loginName, _loginPass, true, 1)) != null)//CmmFunction.Login(getCurrentUserUrl, loginName, loginPass, true, 1))
                        {
                            _checklogin = true;
                            _mainAct.RunOnUiThread(() =>
                            {
                            });

                        }
                        else
                        {
                            _mainAct.RunOnUiThread(() =>
                            {
                                _tvNote.Visibility = ViewStates.Visible;
                                if (CmmVariable.SysConfig.LangCode == "VN")
                                {
                                    _tvNote.Text = CmmFunction.GetTitle("K_Mess_loginFalse", "Thông tin đăng nhập không chính xác, vui lòng thử lại.");
                                }
                                else
                                {
                                    _tvNote.Text = CmmFunction.GetTitle("K_Mess_loginFalse", "Login information is incorrect, please try again");
                                }
                                _progess.Visibility = ViewStates.Gone;
                                _edtPass.Enabled = true;
                                _edtUser.Enabled = true;
                                _tvLogin.Enabled = true;
                            });
                        }
                    });
                }
            }
            catch (Exception)
            {
                _mainAct.RunOnUiThread(() =>
                {
                    _tvNote.Visibility = ViewStates.Visible;
                    if (CmmVariable.SysConfig.LangCode == "VN")
                    {
                        _tvNote.Text = CmmFunction.GetTitle("K_Mess_loginFalse", "Thông tin đăng nhập không chính xác, vui lòng thử lại.");
                    }
                    else
                    {
                        _tvNote.Text = CmmFunction.GetTitle("K_Mess_loginFalse", "Login information is incorrect, please try again");
                    }
                    _progess.Visibility = ViewStates.Gone;
                    _edtPass.Enabled = true;
                    _edtUser.Enabled = true;
                    _tvLogin.Enabled = true;
                });
            }
        }
        private void RegisDeviceInfo()
        {
            try
            {
                DeviceInfo objDevice = new DeviceInfo();
                objDevice.DeviceId = Secure.GetString(_rootView.Context.ContentResolver, Secure.AndroidId);
                objDevice.DeviceOS = 1;
                try
                {
                    _token = FirebaseInstanceId.Instance.Token;
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine("Author: khoahd - FragmentStartView - RegisDeviceInfo - Error: " + ex.Message);
#endif
                }

                if (!string.IsNullOrEmpty(_token))
                {
                    objDevice.DevicePushToken = _token;
                }
                objDevice.DeviceOSVersion = Build.VERSION.SdkInt.ToString();
                objDevice.AppVersion = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName;
                string device = JsonConvert.SerializeObject(objDevice);

                CmmVariable.SysConfig.DeviceInfo = device;
                if (CmmVariable.M_AuthenticatedHttpClient == null)
                {
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new System.Uri(CmmVariable.M_Domain);
                    CmmVariable.M_AuthenticatedHttpClient = client;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - FragmentStartView - RegisDeviceInfo - Error: " + ex.Message);
#endif
            }
        }
        private void CmmEvent_ReloginRequest(object sender, CmmEvent.LoginEventArgs e)
        {
            try
            {
                if (e != null)
                {
                    if (e.IsSuccess)
                    {
                        _mainAct.RunOnUiThread(() =>
                        {

                            CmmVariable.SysConfig.AvatarPath = e.UserInfo.ImagePath;
                            CmmVariable.SysConfig.UserId = e.UserInfo.ID;
                            CmmVariable.SysConfig.Title = e.UserInfo.FullName;
                            CmmVariable.SysConfig.DisplayName = e.UserInfo.FullName;
                            CmmVariable.SysConfig.Email = e.UserInfo.Email;
                            CmmVariable.SysConfig.Department = e.UserInfo.Department;
                            CmmVariable.SysConfig.Address = e.UserInfo.Address;
                            CmmVariable.SysConfig.Position = e.UserInfo.Position;
                            //if (e.UserInfo.Birthday.HasValue)
                            //    CmmVariable.SysConfig.Birthday = e.UserInfo.Birthday;
                            CmmVariable.SysConfig.Mobile = e.UserInfo.Mobile;
                            CmmVariable.SysConfig.LoginName = _loginName;
                            CmmVariable.SysConfig.LoginPassword = _loginPass;
                            CmmVariable.SysConfig.SiteName = e.UserInfo.SiteName;
                            CmmVariable.SysConfig.AppConfigVersion = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName;
                            if (File.Exists(CmmVariable.M_DataLangPath))
                            {
                                ProviderUser proUser = new ProviderUser();
                                string langCode = proUser.GetCurrentlangCode();
                                if (!string.IsNullOrEmpty(langCode))
                                {
                                    CmmVariable.SysConfig.LangCode = langCode;
                                }
                            }
                            CmmFunction.WriteSetting();
                            if (_checklogin) // Đăng nhập lần đầu -> Login
                            {
                                GetDataFirstTimeLogin();
                            }
                            else // Đã đăng nhập trước đó -> Relogin
                            {
                                GetDataAutoLogin();
                            }
                        });
                    }
                }

            }
            catch (Exception)
            {
                // ignored
            }

            CmmEvent.ReloginRequest -= CmmEvent_ReloginRequest;
        }
        private async void GetDataFirstTimeLogin()
        {
            try
            {
                await Task.Run(() =>
                {
                    if (!File.Exists(CmmVariable.M_DataPath))
                    {
                        CmmFunction.InstanceDB(CmmVariable.M_DataPath);
                    }
                    else
                    {
                        File.Delete(CmmVariable.M_DataPath);
                        CmmFunction.InstanceDB(CmmVariable.M_DataPath);
                    }
                    ProviderBase pBase = new ProviderBase();
                    ProviderUser pApp = new ProviderUser();
                    if (File.Exists(CmmVariable.M_DataLangPath))
                    {
                        pApp.UpdateLangData(CmmVariable.SysConfig.LangCode, true, false);
                    }
                    else
                    {
                        pApp.UpdateLangData(CmmVariable.SysConfig.LangCode, false, true);
                    }
                    pBase.UpdateAllMasterData(false, CmmVariable.SysConfig.DataLimitDay, true);
                    pBase.UpdateAllDynamicData(false, CmmVariable.SysConfig.DataLimitDay, true);

                    _mainAct.RunOnUiThread(() =>
                    {
                        _checkAutoLogin = false;
                        //_mainAct.RemoveFragmentInStack(FragmentManager, "FragmentHomePage");
                        // nho bo ra
                        FragmentHomePage homePage = new FragmentHomePage(true, _checkAutoLogin);
                        _mainAct.ShowFragment(FragmentManager, homePage, "FragmentHomePage", 1);
                    });
                });
            }
            catch (Exception)
            {
                // ignored
            }
        }
        private async void GetDataAutoLogin()
        {
            try
            {
                await Task.Run(() =>
                {

                    ProviderUser pApp = new ProviderUser();
                    if (File.Exists(CmmVariable.M_DataLangPath))
                    {
                        pApp.UpdateLangData(CmmVariable.SysConfig.LangCode, true, false);
                    }
                    else
                    {
                        pApp.UpdateLangData(CmmVariable.SysConfig.LangCode, false, true);
                    }
                    ProviderBase pBase = new ProviderBase();
                    pBase.UpdateAllMasterData(true);
                    pBase.UpdateAllDynamicData(true);

                    _mainAct.RunOnUiThread(() =>
                    {
                        GetNotifyKillApp();
                        _checkAutoLogin = true;

                        FragmentHomePage homePage = new FragmentHomePage(true, _checkAutoLogin);
                        _mainAct.ShowFragment(FragmentManager, homePage, "FragmentHomePage", 1);
                    });
                });
            }
            catch (Exception)
            {
                //
            }
        }
        private void GetNotifyKillApp()
        {
            if (!string.IsNullOrEmpty(MainActivity.IdNotify))
            {
                GetNotify(MainActivity.IdNotify);
            }
            else
            {

            }
        }
        /// <summary>
        /// lấy BeanWorkflowItem lúc push về để show detail
        /// </summary>
        /// <param name="itemId">ID beanNotify</param>
        private void GetNotify(string itemId)
        {
            try
            {
                SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                string query = "SELECT * FROM BeanNotify WHERE ID = ? ";
                var res = conn.Query<BeanNotify>(query, itemId);
                if (res != null && res.Count > 0)
                {
                    string queryNotify = string.Format("SELECT * FROM BeanWorkflowItem WHERE ItemID = {0} AND ListName = '{1}'", res[0].DocumentID, res[0].ListName);
                    var lstWorkflowItem = conn.Query<BeanWorkflowItem>(queryNotify);
                    if (lstWorkflowItem != null && lstWorkflowItem.Count > 0)
                    {
                        //nho bo ra
                        ////FragmentDetailWorkflow detailWorkFlow = new FragmentDetailWorkflow(lstWorkflowItem[0], null, "");
                        ////_mainAct.ShowFragment(FragmentManager, detailWorkFlow, "FragmentDetailWorkflow", 0);
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
            finally { MainActivity.IdNotify = ""; }
        }
        #endregion
    }
}
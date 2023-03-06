using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPMOPMobile.Droid.Class.Extension;
using BPMOPMobile.Droid.Class;
using _baseFrag = BPMOPMobile.Droid.Class.Extension.ExtensionBaseFragment;
using Android.Support.V4.Widget;
using BPMOPMobile.Class;
using System.Net.Http;
using System.IO;
using BPMOPMobile.Droid.Core.Common;
using System.Threading.Tasks;
using BPMOPMobile.DataProvider;
using System.Threading;
using BPMOPMobile.Bean;
using BPMOPMobile.Droid.Core.Controller;
using Firebase.Iid;
using Android.Bluetooth;
using static Android.Provider.Settings;
using Newtonsoft.Json;

namespace BPMOPMobile.Droid.Presenter.Fragment
{

    public class FragmentStartView_Ver2 : CustomBaseFragment
    {
        private string _loginName = "";
        private string _loginPass = "";
        private bool _isFirstLogin; // flag để check xem là login lần đầu hay relogin
        private int loadingStep = 0;
        private int randumvalue = 0;
        private bool _flagExistConfig;
        private bool _haveErroCodeFromCookie;


        #region InJectElement

        //Login View
        [InjectView(Resource.Id.frame_StartView_VNA_Ver2_Login)]
        private FrameLayout _frame_StartView_VNA_Ver2_Login;
        [InjectView(Resource.Id.tv_ViewStartView_VNAirlines_Note_Login)]
        private TextView _tvNoteLogin;
        [InjectView(Resource.Id.rl_Note_Login)]
        private RelativeLayout _relaNoteLogin;
        [InjectView(Resource.Id.process_Login)]
        private ProgressBar _processLogin;
        [InjectView(Resource.Id.edt_ViewStartView_VNAirlines_Username)]
        private EditText _edtUserName;
        [InjectView(Resource.Id.edt_ViewStartView_VNAirlines_Pass)]
        private EditText _edtPassword;
        [InjectView(Resource.Id.btn_ViewStartView_VNAirlines_SignIn)]
        private Button _btnSignIn;


        //Auto Login View
        [InjectView(Resource.Id.frame_StartView_VNA_Ver2_Autologin)]
        FrameLayout _frame_StartView_VNA_Ver2_Autologin;
        [InjectView(Resource.Id.tv_ViewStartView_VNAirlines_Note_AutoLogin)]
        TextView _tvNoteAutoLogin;
        #endregion
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            _baseFrag._mainAct._drawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
            CmmEvent.ReloginRequest -= CmmEvent_ReloginRequest;
            CmmEvent.SyncDataBackGroundRequest -= CmmEvent_SyncDataBackGroundRequest;

        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _baseFrag._mainAct = (MainActivity)this.Activity;
            _baseFrag._rootView = inflater.Inflate(Resource.Layout.ViewStartView_VNAirlines_Ver2, null);
            if (_baseFrag._mainAct._drawerLayout != null)
            {
                _baseFrag._mainAct._drawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            }
            _baseFrag._mainAct.Window.SetNavigationBarColor(Android.Graphics.Color.Black);
            Cheeseknife.Inject(this, _baseFrag._rootView);


            SetVisibilityLoginAndAutoLogin();
            CheckAutoLogin();
            _btnSignIn.Click += _btnSignIn_Click;
            return _baseFrag._rootView;
        }
        private async void LoginFunction()
        {
            try
            {
                CmmDroidFunction.HideSoftKeyBoard(_edtPassword, _baseFrag._mainAct);
                if ((String.IsNullOrEmpty(_edtPassword.Text) || (String.IsNullOrEmpty(_edtUserName.Text))))
                {
                    CmmDroidFunction.ShowAlertDialog(_baseFrag._mainAct, CmmFunction.GetTitle("TEXT_LOGINFAIL", "Login information is incorrect, please try again."),
                        CmmFunction.GetTitle("TEXT_LOGINFAIL1", "Vui lòng nhập email doanh nghiệp của bạn."), "Alert", "Alert", "Close", "Close");
                    _edtPassword.Enabled = true;
                    _edtUserName.Enabled = true;
                    _edtUserName.Text = "";
                    _edtPassword.Text = "";
                }
                else
                {

                    _loginName = _edtUserName.Text.TrimEnd();
                    _loginPass = _edtPassword.Text.TrimEnd();
                    CmmVariable.SysConfig.DataLimitDay = CmmVariable.M_DataFilterDefaultDays;

                    await Task.Run(() =>
                    {
                        RegistrationDeviceInfo(); // Đăng ký device info
                        CmmEvent.ReloginRequest += CmmEvent_ReloginRequest;
                        string _getCurrentUserUrl = (CmmVariable.M_Domain.TrimEnd('/') + (new BeanUser().GetCurrentUserUrl())).Replace("<#SiteName#>", "");
                        CmmVariable.M_AuthenticatedHttpClient = CmmFunction.Login(_getCurrentUserUrl, _loginName, _loginPass, true, 1);
                        if (CmmVariable.M_AuthenticatedHttpClient != null)
                        {
                            if (!_haveErroCodeFromCookie)
                            {
                                _isFirstLogin = true;
                            }
                            // Đợi tick Event
                        }
                        else
                        {
                            CmmEvent.ReloginRequest -= CmmEvent_ReloginRequest;
                            _baseFrag._mainAct.RunOnUiThread(() =>
                            {
                                _edtPassword.Enabled = true;
                                _edtUserName.Enabled = true;
                                _edtPassword.Text = "";
                                _edtUserName.Text = "";
                                //_btnBack_Startview.Visibility = ViewStates.Visible;
                                CmmDroidFunction.ShowAlertDialog(_baseFrag._mainAct, CmmFunction.GetTitle("TEXT_LOGINFAIL", "Login information is incorrect, please try again."),
                                    CmmFunction.GetTitle("TEXT_LOGINFAIL", "Login information is incorrect, please try again."), "Alert", "Alert", "Close", "Close");
                            });
                        }
                    });
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "LoginFunction", ex);
#endif
                _baseFrag._mainAct.RunOnUiThread(() =>
                {
      
                });
            }
        }
        private void RegistrationDeviceInfo()
        {
            try
            {
                DeviceInfo objDevice = new DeviceInfo();
                try
                {
                    objDevice.DevicePushToken = FirebaseInstanceId.Instance.Token;
                }
                catch (Exception ex)
                {
#if DEBUG
                    CmmDroidFunction.WriteTrackingError(this.GetType().Name, "RegisDeviceInfo", ex);
#endif
                    objDevice.DevicePushToken = "";
                }

                if (BluetoothAdapter.DefaultAdapter != null)
                    objDevice.DeviceName = BluetoothAdapter.DefaultAdapter.Name;
                else
                    objDevice.DeviceName = "";

                objDevice.DeviceModel = Build.Model;
                objDevice.DeviceId = Secure.GetString(_baseFrag._rootView.Context.ContentResolver, Secure.AndroidId);
                objDevice.DeviceOS = 1;
                objDevice.DeviceOSVersion = Build.VERSION.SdkInt.ToString();
                objDevice.AppVersion = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName;

                CmmVariable.SysConfig.DeviceInfo = JsonConvert.SerializeObject(objDevice);
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
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "RegisDeviceInfo", ex);
#endif
            }
        }

        private void _btnSignIn_Click(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == false) return;
                CmmEvent.SyncDataBackGroundRequest += CmmEvent_SyncDataBackGroundRequest;
                _baseFrag._mainAct.RunOnUiThread(() =>
                {
                    CmmDroidFunction.HideSoftKeyBoard(_edtPassword, _baseFrag._mainAct);
                });
                LoginFunction();
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Click_tvLogin", ex);
#endif
            }
        }

        public void SetVisibilityLoginAndAutoLogin(bool isHideViewLogin = true)
        {
            _baseFrag._mainAct.RunOnUiThread(() =>
            {
                if (isHideViewLogin)
                {
                    _frame_StartView_VNA_Ver2_Autologin.Visibility = ViewStates.Visible;
                    _frame_StartView_VNA_Ver2_Login.Visibility = ViewStates.Gone;
                }
                else
                {
                    _frame_StartView_VNA_Ver2_Autologin.Visibility = ViewStates.Gone;
                    _frame_StartView_VNA_Ver2_Login.Visibility = ViewStates.Visible;
                }
            });
        }
        public async void CheckAutoLogin()
        {
            InitApplicationVariable();
            await Task.Run(() =>
            {
                CmmEvent.SyncDataBackGroundRequest += CmmEvent_SyncDataBackGroundRequest;
                _baseFrag._cBase = new ControllerBase();
                #region Check version
                _baseFrag._pUser = new ProviderUser();
                if (!ValidateAppVersion(_baseFrag._pUser))// Version hợp lệ
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
                        _baseFrag._mainAct.Finish();
                    });
                    CmmDroidFunction.ShowAlertDialogWithAction(_baseFrag._mainAct, CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ? "Phiên bản mới đã có, vui lòng cập nhật để có được sự mượt mà và ổn định" : "The new version is available, please update to get smoother and more stable",
                            _actionPositiveButton: new Action(() => { _actionPositiveButton(); }),
                            _actionNegativeButton: new Action(() => { _actionNegativeButton(); }),
                            _title: CmmDroidFunction.GetApplicationName(_baseFrag._rootView.Context),
                            _positive: CmmFunction.GetTitle("TEXT_AGREE", "Đồng ý"),
                            _negative: CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"));
                }
                #endregion
                #region Check Data
                _flagExistConfig = CmmFunction.ReadSetting();
                ValidateRenewDB(_flagExistConfig);
                CmmVariable.SysConfig.AppConfigVersion = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName;
                if (!_flagExistConfig) // không có thông tin cấu hình -> Renew DB và Hiện View login lên
                {
                    _isFirstLogin = true;
                    if (File.Exists(CmmVariable.M_DataPath))
                        File.Delete(CmmVariable.M_DataPath);
                    CmmFunction.InstanceDB(CmmVariable.M_DataPath);
                    //pUser.UpdateAllMasterData(false, CmmVariable.SysConfig.DataLimitDay, true);
                    SetVisibilityLoginAndAutoLogin(isHideViewLogin: false);
                    _baseFrag._mainAct.RunOnUiThread(() =>
                    {
                        _relaNoteLogin.Visibility = ViewStates.Gone;
                    });

                    return;
                }
                else
                {
                    CmmEvent.ReloginRequest += CmmEvent_ReloginRequest;
                }
                // đã có thông tin cấu hình -> Relogin lại
                if (!File.Exists(CmmVariable.M_DataPath))
                {
                    CmmFunction.InstanceDB(CmmVariable.M_DataPath);
                    _isFirstLogin = true;
                }
                #region Check Authentication Server
                CmmVariable.SysConfig.DataLimitDay = CmmVariable.M_DataFilterDefaultDays;
                _loginName = CmmVariable.SysConfig.LoginName;
                _loginPass = CmmVariable.SysConfig.LoginPassword;
                if (!String.IsNullOrEmpty(_loginName) && !String.IsNullOrEmpty(_loginPass)) // có loginName và Password
                {

                    if (_baseFrag._cBase.CheckAppHasConnection())
                    {
                        string _getCurrentUserUrl = (CmmVariable.M_Domain.TrimEnd('/') + (new BeanUser().GetCurrentUserUrl())).Replace("<#SiteName#>", "");
                        CmmVariable.M_AuthenticatedHttpClient = CmmFunction.Login(_getCurrentUserUrl, _loginName, _loginPass, true, 1, "", !String.IsNullOrEmpty(CmmVariable.SysConfig.VerifyOTP) ? CmmVariable.SysConfig.VerifyOTP : "");

                        // Nếu (M_AuthenticatedHttpClient != null) thì Authen Success -> chờ Tick CmmEvent
                        // Nếu (M_AuthenticatedHttpClient == null) thì Authen Failed -> logout Account ra
                        if (CmmVariable.M_AuthenticatedHttpClient == null)
                        {
                            _baseFrag._mainAct.RunOnUiThread(() =>
                            {
                                CmmEvent.ReloginRequest -= CmmEvent_ReloginRequest;
                                CmmEvent.SyncDataBackGroundRequest -= CmmEvent_SyncDataBackGroundRequest;

                                /* if (!haveErroCodeFromCookie)
                                 {
                                     _isFirstLogin = true;
                                     _baseFrag._mainAct.SignOut();
                                 }
                                 else
                                 {
                                     _isFirstLogin = false;
                                 }*/
                            });
                        }
                    }
                    else // Có thông tin User Default nhưng không có connection -> Login offline vào HomePage
                    {
                        _baseFrag._mainAct.RunOnUiThread(() =>
                        {

                            /*FragmentHomePage homePage = new FragmentHomePage();
                            _mainAct.ShowFragment(FragmentManager, homePage, "FragmentHomePage", 1);*/

                            FragmentParent parent = new FragmentParent();
                            _baseFrag._mainAct.ShowFragment(FragmentManager, parent, "FragmentParent", 1);
                        });
                    }
                }
                else // không có thông user default, Instance lại DB và Visible Linear Login
                {
                    CmmEvent.ReloginRequest -= CmmEvent_ReloginRequest;
                    if (File.Exists(CmmVariable.M_DataPath))
                        File.Delete(CmmVariable.M_DataPath);
                    CmmFunction.InstanceDB(CmmVariable.M_DataPath);
                    //pUser.UpdateAllMasterData(false, CmmVariable.SysConfig.DataLimitDay, true);
                    _baseFrag._mainAct.RunOnUiThread(() =>
                    {
                        //_btnBack_Startview.Visibility = ViewStates.Visible;
                        if (_baseFrag._cBase.CheckAppHasConnection())
                        {


                        }
                        else
                        {

                        }
                    });
                }
                #endregion
                #endregion
            });
        }
        /// <summary>
        /// Kiểm tra all version -> nếu false là phiên bản chưa khớp
        /// </summary>
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

        /// <summary>
        /// Kiểm tra xem có cần Instance lại DB theo cấu trúc mới không
        /// </summary>
        private void ValidateRenewDB(bool _flagExistConfig)
        {
            try
            {
                if (CmmVariable.M_RenewDB == true)
                {
                    bool _flagClearData = false;

                    if (!_flagExistConfig || String.IsNullOrEmpty(CmmVariable.SysConfig.AppConfigVersion)) // chưa có thiết lập config hoặc chưa có app Ver
                    {
                        if (File.Exists(CmmVariable.M_DataPath))
                            _flagClearData = true;
                    }
                    else
                    {
                        if (CmmVariable.SysConfig.AppConfigVersion != Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName)
                            _flagClearData = true; // Nếu Khác Version -> Clear
                    }

                    if (_flagClearData == true)
                    {
                        try
                        {
                            File.Delete(CmmVariable.M_DataPath);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "ValidateRenewDB", ex);
#endif
            }
        }
        private void CmmEvent_SyncDataBackGroundRequest(object sender, CmmEvent.UpdateBackgroundEventArgs e)
        {
            //Console.WriteLine("BeanName: " + e.BeanName + "|| CountItem: " + e.ErrMess);
            loadingStep = loadingStep + 1;
            int rndTemp = 0;
            Random rnd = new Random();

            _baseFrag._mainAct.RunOnUiThread(() =>
            {
                if (loadingStep < 4)
                {
                    rndTemp = rnd.Next(randumvalue, 20);
                    /*                    _tvNote.Text = "Initialization Application..." + (randumvalue > rndTemp ? randumvalue.ToString() : rndTemp.ToString()) + "%";
                    */
                    if (randumvalue < rndTemp)
                        randumvalue = rndTemp;
                }
                else if (loadingStep < 8)
                {
                    rndTemp = rnd.Next(randumvalue, 40);
                    /*                    _tvNote.Text = "Connecting to server..." + (randumvalue > rndTemp ? randumvalue.ToString() : rndTemp.ToString()) + "%";
                    */
                    if (randumvalue < rndTemp)
                        randumvalue = rndTemp;
                }
                else if (loadingStep < 12)
                {
                    rndTemp = rnd.Next(randumvalue, 60);
                    /*                    _tvNote.Text = "Verifying user information..." + (randumvalue > rndTemp ? randumvalue.ToString() : rndTemp.ToString()) + "%";
                    */
                    if (randumvalue < rndTemp)
                        randumvalue = rndTemp;
                }
                else if (loadingStep < 16)
                {
                    rndTemp = rnd.Next(randumvalue, 80);
                    /*                    _tvNote.Text = "Initialization data..." + (randumvalue > rndTemp ? randumvalue.ToString() : rndTemp.ToString()) + "%";
                    */
                    if (randumvalue < rndTemp)
                        randumvalue = rndTemp;
                }
                else if (loadingStep < 20)
                {
                    rndTemp = rnd.Next(randumvalue, 100);
                    /*                    _tvNote.Text = "Loading data..." + (randumvalue > rndTemp ? randumvalue.ToString() : rndTemp.ToString()) + "%";
                    */
                    if (randumvalue < rndTemp)
                        randumvalue = rndTemp;
                }

            });
        }
        private void CmmEvent_ReloginRequest(object sender, CmmEvent.LoginEventArgs e)
        {
            CmmEvent.ReloginRequest -= CmmEvent_ReloginRequest;
            try
            {

                if (!e.ErrCode.Contains("<ErrorCode>PasswordNotMatch</ErrorCode>"))
                {
                    if (e != null && e.IsSuccess && (e.UserInfo != null)) // Login 1 lớp bình thường
                    {
                        _baseFrag._mainAct.RunOnUiThread(() =>
                        {


                            if (!String.IsNullOrEmpty(CmmVariable.SysConfig.AvatarPath) && CmmVariable.SysConfig.AvatarPath != e.UserInfo.ImagePath) // Có avatar mới
                            {
                                if (File.Exists(CmmVariable.M_Avatar)) // Xóa avatar cũ đi
                                    File.Delete(CmmVariable.M_Avatar);
                            }
                            CmmVariable.SysConfig.VerifyOTP = e.VerifyOTP;
                            CmmVariable.SysConfig.AvatarPath = e.UserInfo.ImagePath;
                            CmmVariable.SysConfig.UserId = e.UserInfo.ID;
                            CmmVariable.SysConfig.Title = e.UserInfo.FullName;
                            CmmVariable.SysConfig.DisplayName = e.UserInfo.FullName;
                            CmmVariable.SysConfig.Email = e.UserInfo.Email;
                            CmmVariable.SysConfig.Department = e.UserInfo.Department;
                            CmmVariable.SysConfig.Address = e.UserInfo.Address;
                            CmmVariable.SysConfig.PositionID = e.UserInfo.PositionID;
                            CmmVariable.SysConfig.PositionTitle = e.UserInfo.PositionTitle;

                            CmmVariable.SysConfig.Mobile = e.UserInfo.Mobile;
                            CmmVariable.SysConfig.LoginName = _loginName;
                            CmmVariable.SysConfig.LoginPassword = _loginPass;
                            CmmVariable.SysConfig.SiteName = e.UserInfo.SiteName;
                            CmmVariable.SysConfig.AppConfigVersion = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName;
                            if (CmmVariable.SysConfig.LangCode == "0" || string.IsNullOrEmpty(CmmVariable.SysConfig.LangCode))
                                CmmVariable.SysConfig.LangCode = CmmDroidVariable.M_SysLangVN;
                            else
                                CmmVariable.SysConfig.LangCode = e.UserInfo.Language.ToString();

                            if (String.IsNullOrEmpty(CmmVariable.SysConfig.LangCode)) // Ko có ngôn ngữ -> Lấy Default
                            {
                                if (File.Exists(CmmVariable.M_DataLangPath))
                                {
                                    _baseFrag._pUser = new ProviderUser();
                                    string langCode = _baseFrag._pUser.GetCurrentlangCode();
                                    if (!string.IsNullOrEmpty(langCode))
                                        CmmVariable.SysConfig.LangCode = langCode;
                                }
                            }

                            //_mainAct.UpdateDBLanguage(CmmVariable.SysConfig.LangCode);
                            CmmFunction.WriteSetting(); // Lưu Setting
                            CmmDroidFunction.ShowVibrateEvent(0.2);
                            if (_isFirstLogin) // Đăng nhập lần đầu -> Login
                            {
                                GetDataFirstTimeLogin();
                            }
                            else // Đã đăng nhập trước đó -> Relogin
                            {
                                GetDataAutoLogin();
                            }
                        });
                    }
                    else if (e != null && e.IsSuccess && (e.UserInfo == null && e.ErrCode.Equals("OTP"))) // Login 2 lớp OTP
                    {
                        _baseFrag._mainAct.RunOnUiThread(() =>
                        {
                            FragmentConfirmOTP fragmentConfirmOTP = new FragmentConfirmOTP(_loginName, _loginPass, _isFirstLogin);
                            _baseFrag._mainAct.ShowFragment(FragmentManager, fragmentConfirmOTP, "FragmentConfirmOTP", 1);
                        });
                    }
                }
                else
                {
                    _baseFrag._mainAct.RunOnUiThread(() =>
                    {
                        CmmVariable.SysConfig.LoginPassword = "_";
                        CmmFunction.WriteSetting();
                        CmmDroidFunction.ShowAlertDialog(_baseFrag._mainAct, CmmFunction.GetTitle("TEXT_LOGINFAIL", "Login information is incorrect, please try again."),
                       CmmFunction.GetTitle("TEXT_LOGINFAIL", "Login information is incorrect, please try again."), "Alert", "Alert", "Close", "Close");

                    });
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "CmmEvent_ReloginRequest", ex);
#endif
            }
        }


        /// <summary>
        /// Khởi tạo các Common Variable cho app
        /// </summary>
        public void InitApplicationVariable(MainActivity mainAct = null)
        {
            try
            {
                // Initialize SysConfig
                if (CmmVariable.SysConfig == null)
                    CmmVariable.SysConfig = new ConfigVariable();

                HttpClient client = new HttpClient();
                client.BaseAddress = new System.Uri(CmmVariable.M_Domain);
                CmmVariable.M_AuthenticatedHttpClient = client;

                // Initialize CmmVariable
                if (_baseFrag._mainAct == null)
                    _baseFrag._mainAct = mainAct;
                string appPath = _baseFrag._mainAct.FilesDir.ToString();
                if (!CmmVariable.M_DataPath.StartsWith(appPath))
                {
                    CmmVariable.M_DataPath = appPath + Path.PathSeparator + CmmVariable.M_DataPath;
                    CmmVariable.M_settingFileName = appPath + Path.PathSeparator + CmmVariable.M_settingFileName;
                    CmmVariable.M_Avatar = appPath + Path.PathSeparator + CmmVariable.M_Avatar;
                    CmmVariable.M_Folder_Avatar = appPath + "/" + CmmVariable.M_Folder_Avatar;
                    CmmVariable.M_DataLangPath = appPath + Path.PathSeparator + CmmVariable.M_DataLangPath;
                    CmmVariable.M_DataFolder = appPath + "/" + CmmVariable.M_DataFolder;
                    CmmVariable.M_AvatarCus = appPath + Path.PathSeparator + CmmVariable.M_AvatarCus;

                    if (!Directory.Exists(CmmVariable.M_Folder_Avatar)) // Create avatar folder
                        Directory.CreateDirectory(CmmVariable.M_Folder_Avatar);

                    if (!Directory.Exists(CmmVariable.M_DataFolder)) // Create data folder
                        Directory.CreateDirectory(CmmVariable.M_DataFolder);

                    if (CmmDroidFunction.CheckSdCard()) // Create log folder on SD Card
                        CmmVariable.M_LogPath = appPath + "/" + CmmVariable.M_LogPath;
                    else
                        CmmVariable.M_LogPath = Android.OS.Environment.DirectoryDocuments + "/" + CmmVariable.M_LogPath;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "InitApplicationVariable", ex);
#endif
            }
        }
        private async void GetDataFirstTimeLogin()
        {
            try
            {
                await Task.Run(async () =>
                {
                    /*if (File.Exists(CmmVariable.M_DataPath))
                        File.Delete(CmmVariable.M_DataPath);
                    CmmFunction.InstanceDB(CmmVariable.M_DataPath);*/

                    _baseFrag._pUser = new ProviderUser();
                    _baseFrag._pUser.UpdateLangData(CmmVariable.SysConfig.LangCode, false, true);
                    await _baseFrag._pUser.UpdateAllDynamicDataAndroid(false, CmmVariable.SysConfig.DataLimitDay, true);
                    _baseFrag._mainAct.RunOnUiThread(() =>
                    {
                        /*                        _tvNote.Text = CmmFunction.GetTitle("TEXT_UPDATE_FINISHED", "Update finished");
                        */
                    });
                    //if (File.Exists(CmmVariable.M_DataLangPath))
                    //    pUser.UpdateLangData(CmmVariable.SysConfig.LangCode, true, false);
                    //else
                    //    pUser.UpdateLangData(CmmVariable.SysConfig.LangCode, false, true);

                    _baseFrag._mainAct.RunOnUiThread(() =>
                    {
                        string _RVID_FromMe = CmmFunction.GetAppSettingValue("MOBILE_RESOURCEVIEWID_FROMME");
                        if (!string.IsNullOrEmpty(_RVID_FromMe))
                            CmmVariable.M_ResourceViewID_FromMe = _RVID_FromMe;

                        string _RVID_ToMe = CmmFunction.GetAppSettingValue("MOBILE_RESOURCEVIEWID_TOME");
                        if (!string.IsNullOrEmpty(_RVID_ToMe))
                            CmmVariable.M_ResourceViewID_ToMe = _RVID_ToMe;

                        /*FragmentHomePage homePage = new FragmentHomePage();
                        _mainAct.ShowFragment(FragmentManager, homePage, "FragmentHomePage", 1);*/
                        FragmentParent parent = new FragmentParent();
                        _baseFrag._mainAct.ShowFragment(FragmentManager, parent, "FragmentParent", 1);
                    });

                });
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "GetDataFirstTimeLogin", ex);
#endif
            }
        }

        private async void GetDataAutoLogin()
        {
            try
            {
                await Task.Run(async () =>
                {
                    _baseFrag._pUser = new ProviderUser();
                    _baseFrag._pUser.UpdateLangData(CmmVariable.SysConfig.LangCode, false, true);
                    //pUser.UpdateAllMasterData(true);
                    await _baseFrag._pUser.UpdateAllDynamicDataAndroid(true);
                    _baseFrag._mainAct.RunOnUiThread(() =>
                    {
                        /*                        _tvNote.Text = CmmFunction.GetTitle("TEXT_UPDATE_FINISHED", "Update finished");
                        */
                    });
                    //if (File.Exists(CmmVariable.M_DataLangPath))
                    //    pUser.UpdateLangData(CmmVariable.SysConfig.LangCode, true, false);
                    //else
                    //    pUser.UpdateLangData(CmmVariable.SysConfig.LangCode, false, true);
                    Thread.Sleep(500);
                    _baseFrag._mainAct.RunOnUiThread(() =>
                    {
                        string _RVID_FromMe = CmmFunction.GetAppSettingValue("MOBILE_RESOURCEVIEWID_FROMME");
                        if (!string.IsNullOrEmpty(_RVID_FromMe))
                            CmmVariable.M_ResourceViewID_FromMe = _RVID_FromMe;

                        string _RVID_ToMe = CmmFunction.GetAppSettingValue("MOBILE_RESOURCEVIEWID_TOME");
                        if (!string.IsNullOrEmpty(_RVID_ToMe))
                            CmmVariable.M_ResourceViewID_ToMe = _RVID_ToMe;

                        /*FragmentHomePage homePage = new FragmentHomePage();
                        _mainAct.ShowFragment(FragmentManager, homePage, "FragmentHomePage", 1);*/
                        FragmentParent parent = new FragmentParent();
                        _baseFrag._mainAct.ShowFragment(FragmentManager, parent, "FragmentParent", 1);
                    });

                });
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "GetDataAutoLogin", ex);
#endif
            }
        }

    }
}
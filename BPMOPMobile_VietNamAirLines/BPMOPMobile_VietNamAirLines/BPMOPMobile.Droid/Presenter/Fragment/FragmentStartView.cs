using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Hardware.Biometrics;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V4.Content.Res;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
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
using Firebase;
using Firebase.Iid;
using Newtonsoft.Json;
using SQLite;
using static Android.Provider.Settings;
using static BPMOPMobile.Droid.Core.Class.CustomBiometricHelper;

namespace BPMOPMobile.Droid.Presenter.Fragment
{
    public class FragmentStartView : CustomBaseFragment, IDialogInterfaceOnClickListener
    {
        private ControllerBase CTRLBase = new ControllerBase();
        private CustomBiometricHelper _bioHelper = new CustomBiometricHelper();
        private MainActivity _mainAct;
        private View _rootView;
        private CardView _cardLogin, _cardviewRegister;
        private LinearLayout _lnName, _lnPass;
        private WebView _webAd;
        private TextView _tvLogin, _tvNote;
        private EditText _edtUser, _edtPass;
        private bool _isFirstLogin; // flag để check xem là login lần đầu hay relogin
        private string _loginName = "";
        private string _loginPass = "";
        public bool _flagExistConfig;
        public Button _txtSignUp;
        private FrameLayout _relabackground;
        private RelativeLayout _relainputData;
        private LinearLayout _lnTextProcess;
        private ProgressBar _process_Login;
        private ImageView _imgcovera, _ImgViewlogoVNa;
        private ImageButton _btnBack_Startview;
        private bool isInitValue = false;
        private bool _fromRegister = false;
        bool _creatateSuccess = false;
        private string stringRegisterSuccess = "Thông tin xác nhận kích hoạt đã được gửi đến email của bạn, vui lòng kiểm tra email và làm theo hướng dẫn kich hoạt.";
        private string stringRegisterSuccessEN = "Activation confirmation has been sent to your email, please check your email and follow the activation instructions.";
        private int loadingStep = 0;
        private int randumvalue = 0;
        private bool haveErroCodeFromCookie = false;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public FragmentStartView()
        {

        }
        public FragmentStartView(bool fromRegister, bool _creatateSuccess)
        {
            this._fromRegister = fromRegister;
            this._creatateSuccess = _creatateSuccess;
        }
        public override void OnDestroyView()
        {
            base.OnDestroyView();
            _mainAct._drawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
            CmmEvent.ReloginRequest -= CmmEvent_ReloginRequest;
            CmmEvent.SyncDataBackGroundRequest -= CmmEvent_SyncDataBackGroundRequest;

        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _rootView = inflater.Inflate(Resource.Layout.ViewStartView_VNAirlines, null);
            _mainAct = (MainActivity)this.Activity;
            if (_mainAct._drawerLayout != null)
            {
                _mainAct._drawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            }
            _mainAct.Window.SetNavigationBarColor(Android.Graphics.Color.Black);
            _tvLogin = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewStartView_VNAirlines_Login);
            _tvNote = _rootView.FindViewById<TextView>(Resource.Id.tv_ViewStartView_VNAirlines_Note);
            _edtPass = _rootView.FindViewById<EditText>(Resource.Id.edt_ViewStartView_VNAirlines_Pass);
            _edtUser = _rootView.FindViewById<EditText>(Resource.Id.edt_ViewStartView_VNAirlines_Username);
            _lnName = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewStartView_VNAirlines_Name);
            _lnPass = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_ViewStartView_VNAirlines_Pass);
            _cardLogin = _rootView.FindViewById<CardView>(Resource.Id.card_ViewStartView_VNAirlines_Login);
            _webAd = _rootView.FindViewById<WebView>(Resource.Id.web_ViewStartView_VNAirlines_Advertisement);
            _txtSignUp = _rootView.FindViewById<Button>(Resource.Id.txtSignUp);
            _relainputData = _rootView.FindViewById<RelativeLayout>(Resource.Id.relainputData);
            _relabackground = _rootView.FindViewById<FrameLayout>(Resource.Id.relabackground);
            _imgcovera = _rootView.FindViewById<ImageView>(Resource.Id.imgcovera);
            _ImgViewlogoVNa = _rootView.FindViewById<ImageView>(Resource.Id.ImgViewlogoVNa);
            _btnBack_Startview = _rootView.FindViewById<ImageButton>(Resource.Id.btnBack_Startview);
            _lnTextProcess = _rootView.FindViewById<LinearLayout>(Resource.Id.ln_StartView_Loading);
            _process_Login = _rootView.FindViewById<ProgressBar>(Resource.Id.process_Login);
            _edtUser.TextChanged += TextChanged_EdtUser;
            _edtPass.TextChanged += TextChanged_EdtPass;
            _edtPass.EditorAction += EditorAction_edtPass;
            _edtUser.EditorAction += EditorAction_edtUser;
            _edtUser.ImeOptions = ImeAction.Go;
            _edtPass.ImeOptions = ImeAction.Go;
            _txtSignUp.Visibility = ViewStates.Gone;
            _txtSignUp.Click += _txtSignUp_Click;
            CTRLBase.RequestAppPermission(_mainAct);
            SetData();
            if (isInitValue || _fromRegister)
            {
                SetVisibleLinearLogin(ViewStates.Visible);
                _relabackground.SetBackgroundColor(Android.Graphics.Color.White);
                _imgcovera.Visibility = ViewStates.Visible;
                _ImgViewlogoVNa.Visibility = ViewStates.Gone;
                _tvNote.Visibility = ViewStates.Invisible;
                //_btnBack_Startview.Visibility = ViewStates.Visible;
            }
            else
            {
                CheckAutoLogin();
                isInitValue = true;
            }
            SetViewByLanguage();
            _tvLogin.Click += Click_tvLogin;
            _cardLogin.Click += Click_tvLogin;
            _btnBack_Startview.Click += _btnBack_Startview_Click;
            _edtUser.Text = "";
            _edtPass.Text = "";
            _relabackground.Touch += _relabackground_Touch;

#if DEBUG
            if (CmmVariable.M_Domain.Equals("https://bpm.vuthao.com") || CmmVariable.M_Domain.Equals("https://bpmon.vuthao.com"))
                _edtPass.Text = "VuThao123!@#";
#endif
            //HandleFingerPrint();
            if (_fromRegister && _creatateSuccess)
            {
                _mainAct.RunOnUiThread(() => { CmmDroidFunction.ShowAlertDialog(_mainAct, stringRegisterSuccess, stringRegisterSuccessEN); });
            }
            return _rootView;
        }


        #region Event
        private void CmmEvent_SyncDataBackGroundRequest(object sender, CmmEvent.UpdateBackgroundEventArgs e)
        {
            //Console.WriteLine("BeanName: " + e.BeanName + "|| CountItem: " + e.ErrMess);
            loadingStep = loadingStep + 1;
            int rndTemp = 0;
            Random rnd = new Random();

            _mainAct.RunOnUiThread(() =>
            {
                if (loadingStep < 4)
                {
                    rndTemp = rnd.Next(randumvalue, 20);
                    _tvNote.Text = "Initialization Application..." + (randumvalue > rndTemp ? randumvalue.ToString() : rndTemp.ToString()) + "%";
                    if (randumvalue < rndTemp)
                        randumvalue = rndTemp;
                }
                else if (loadingStep < 8)
                {
                    rndTemp = rnd.Next(randumvalue, 40);
                    _tvNote.Text = "Connecting to server..." + (randumvalue > rndTemp ? randumvalue.ToString() : rndTemp.ToString()) + "%";
                    if (randumvalue < rndTemp)
                        randumvalue = rndTemp;
                }
                else if (loadingStep < 12)
                {
                    rndTemp = rnd.Next(randumvalue, 60);
                    _tvNote.Text = "Verifying user information..." + (randumvalue > rndTemp ? randumvalue.ToString() : rndTemp.ToString()) + "%";
                    if (randumvalue < rndTemp)
                        randumvalue = rndTemp;
                }
                else if (loadingStep < 16)
                {
                    rndTemp = rnd.Next(randumvalue, 80);
                    _tvNote.Text = "Initialization data..." + (randumvalue > rndTemp ? randumvalue.ToString() : rndTemp.ToString()) + "%";
                    if (randumvalue < rndTemp)
                        randumvalue = rndTemp;
                }
                else if (loadingStep < 20)
                {
                    rndTemp = rnd.Next(randumvalue, 100);
                    _tvNote.Text = "Loading data..." + (randumvalue > rndTemp ? randumvalue.ToString() : rndTemp.ToString()) + "%";
                    if (randumvalue < rndTemp)
                        randumvalue = rndTemp;
                }

            });
        }

        private void _relabackground_Touch(object sender, View.TouchEventArgs e)
        {
            CmmDroidFunction.HideSoftKeyBoard(_edtPass, _mainAct);
        }

        private void _btnBack_Startview_Click(object sender, EventArgs e)
        {
            _mainAct.HideFragment();
        }

        private void _txtSignUp_Click(object sender, EventArgs e)
        {
            FragmentSignUp fragmentSignUp = new FragmentSignUp(1);
            _mainAct.ShowFragment(FragmentManager, fragmentSignUp, "FragmentSignUp", 1);
        }

        private void SetViewByLanguage()
        {
            try
            {
                _edtPass.Hint = CmmFunction.GetTitle("TEXT_PASSWORD", "Password");
                _edtUser.Hint = CmmFunction.GetTitle("TEXT_ACCOUNTNAME", "Account name");
                _tvLogin.Text = CmmFunction.GetTitle("TEXT_LOGIN", "Sign in");
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetViewByLanguage", ex);
#endif
            }
        }

        private void Click_tvLogin(object sender, EventArgs e)
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick() == false) return;
                CmmEvent.SyncDataBackGroundRequest += CmmEvent_SyncDataBackGroundRequest;
                _mainAct.RunOnUiThread(() =>
                {
                    SetVisibleLinearLogin(ViewStates.Invisible);
                    _relabackground.SetBackgroundColor(Android.Graphics.Color.White);
                    _btnBack_Startview.Visibility = ViewStates.Gone;
                    _lnTextProcess.Visibility = ViewStates.Visible;
                    CmmDroidFunction.HideSoftKeyBoard(_edtPass, _mainAct);
                    _edtPass.Enabled = false;
                    _edtUser.Enabled = false;
                    _tvLogin.Enabled = false;

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

        private void EditorAction_edtUser(object sender, TextView.EditorActionEventArgs e)
        {
            switch (e.ActionId)
            {
                case ImeAction.Done:
                case ImeAction.Next:
                case ImeAction.Go:
                    {
                        _edtPass.Focusable = true;
                        _edtPass.FocusableInTouchMode = true;
                        _edtPass.RequestFocus();
                        break;
                    }
            }
        }

        private void EditorAction_edtPass(object sender, TextView.EditorActionEventArgs e)
        {
            switch (e.ActionId)
            {
                case ImeAction.Done:
                case ImeAction.Next:
                case ImeAction.Go:
                    {
                        Click_tvLogin(null, null);
                        break;
                    }
            }
        }

        private void TextChanged_EdtUser(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(_edtUser.Text))
                    _edtUser.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), Android.Graphics.TypefaceStyle.Italic);
                else
                    _edtUser.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), Android.Graphics.TypefaceStyle.Normal);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "TextChanged_EdtUser", ex);
#endif
            }
        }

        private void TextChanged_EdtPass(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(_edtPass.Text))
                    _edtPass.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), Android.Graphics.TypefaceStyle.Italic);
                else
                    _edtPass.SetTypeface(ResourcesCompat.GetFont(_mainAct, Resource.Font.fontarial), Android.Graphics.TypefaceStyle.Normal);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "TextChanged_EdtPass", ex);
#endif
            }
        }

        private void SetVisibleLinearLogin(ViewStates viewStates)
        {
            _lnName.Visibility = viewStates;
            _lnPass.Visibility = viewStates;
            _cardLogin.Visibility = viewStates;
            //_txtSignUp.Visibility = viewStates;
            _relainputData.Visibility = viewStates;
            if (viewStates == ViewStates.Visible)
            {
                _lnName.Enabled = true;
                _lnPass.Enabled = true;
            }
        }
        #endregion

        #region Data
        private void SetData()
        {
            try
            {
                ////if (CTRLBase.CheckAppHasConnection() == true)
                ////{
                ////    _webAd.LoadDataWithBaseURL("file:///android_res/drawable/", "<style>img{display: inline;width: auto;max-height: 50%;}</style> <img src='img_background_startview_3.png' />", "text/html", "utf-8", null);

                ////    _webAd.LoadData("<html><body><img src=\"" + CmmDroidVariable.M_DynamicBackgroundURL + "\" width=\"100%\" height=\"100%\"\"/></body></html>", "text/html", null);
                ////}
                ////else
                _webAd.LoadDataWithBaseURL("file:///android_res/drawable/", "<style>img{display: inline;width: auto;max-height: 50%;}</style> <img src='img_background_startview_3_small.png' />", "text/html", "utf-8", null);

                _webAd.VerticalScrollBarEnabled = false;
                _webAd.HorizontalScrollBarEnabled = false;
                _webAd.Settings.LoadWithOverviewMode = true;
                _webAd.Settings.UseWideViewPort = true;
                _webAd.SetWebChromeClient(new WebChromeClient());
                _webAd.SetInitialScale(1);
                _webAd.Touch += (sender, e) => { }; // Disable scroll

            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SetData", ex);
#endif
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
                objDevice.DeviceId = Secure.GetString(_rootView.Context.ContentResolver, Secure.AndroidId);
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

        public async void CheckAutoLogin()
        {
            try
            {
                SetVisibleLinearLogin(ViewStates.Gone);
                _relabackground.SetBackgroundResource(Resource.Drawable.backgroud1);
                _imgcovera.Visibility = ViewStates.Gone;
                _ImgViewlogoVNa.Visibility = ViewStates.Visible;
                ProviderUser pUser = new ProviderUser();
                _tvNote.Visibility = ViewStates.Visible;
                _btnBack_Startview.Visibility = ViewStates.Gone;
                _tvNote.Text = CmmFunction.GetTitle("TEXT_LOADING", "Please wait a minute"); // only English

                InitApplicationVariable(); // Init các folder - variable

                await Task.Run(() =>
                {
                    CmmEvent.SyncDataBackGroundRequest += CmmEvent_SyncDataBackGroundRequest;
                    //// StartView luôn là tiếng Anh -> cập nhật lại DB
                    //CmmVariable.SysConfig.LangCode = CmmDroidVariable.M_SysLangEN;
                    //_mainAct.UpdateDBLanguage(CmmDroidVariable.M_SysLangEN);

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
                    /*else
                    {
                        bool relogin = CmmFunction.ReadSetting();
                        if (relogin)
                        {
                            _mainAct.HideFragment();
                            FragmentStartView fragmentStartView = new FragmentStartView();
                            _mainAct.ShowFragment(FragmentManager, fragmentStartView, "FragmentStartView");
                        }
                    }*/
                    #endregion

                    _flagExistConfig = CmmFunction.ReadSetting();

                    ValidateRenewDB(_flagExistConfig); // Renew lại DB Structure nếu cần

                    CmmVariable.SysConfig.AppConfigVersion = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName;

                    if (!_flagExistConfig) // không có thông tin cấu hình -> Renew DB và Hiện View login lên
                    {
                        _isFirstLogin = true;
                        if (File.Exists(CmmVariable.M_DataPath))
                            File.Delete(CmmVariable.M_DataPath);
                        CmmFunction.InstanceDB(CmmVariable.M_DataPath);

                        //pUser.UpdateAllMasterData(false, CmmVariable.SysConfig.DataLimitDay, true);
                        _mainAct.RunOnUiThread(() =>
                        {
                            SetVisibleLinearLogin(ViewStates.Visible);
                            _relabackground.SetBackgroundColor(Android.Graphics.Color.White);
                            _imgcovera.Visibility = ViewStates.Visible;
                            _ImgViewlogoVNa.Visibility = ViewStates.Gone;
                            //_btnBack_Startview.Visibility = ViewStates.Visible;

                            if (CTRLBase.CheckAppHasConnection())
                            {
                                _tvNote.Visibility = ViewStates.Invisible;
                                _process_Login.Visibility = ViewStates.Invisible;
                            }
                            else
                            {
                                _tvNote.Visibility = ViewStates.Visible;
                                _process_Login.Visibility = ViewStates.Invisible;

                                _tvNote.Text = CmmFunction.GetTitle("MESS_LOGIN_REQUIRE_CONNECTION", "No network connection, please try again.");
                            }
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
                        SetVisibleLinearLogin(ViewStates.Invisible);
                        _relabackground.SetBackgroundResource(Resource.Drawable.backgroud1);
                        _btnBack_Startview.Visibility = ViewStates.Gone;
                    }

                    #region Check Authentication Server
                    CmmVariable.SysConfig.DataLimitDay = CmmVariable.M_DataFilterDefaultDays;
                    _loginName = CmmVariable.SysConfig.LoginName;
                    _loginPass = CmmVariable.SysConfig.LoginPassword;
                    if (!String.IsNullOrEmpty(_loginName) && !String.IsNullOrEmpty(_loginPass)) // có loginName và Password
                    {

                        if (CTRLBase.CheckAppHasConnection())
                        {
                            string _getCurrentUserUrl = (CmmVariable.M_Domain.TrimEnd('/') + (new BeanUser().GetCurrentUserUrl())).Replace("<#SiteName#>", "");
                            CmmVariable.M_AuthenticatedHttpClient = CmmFunction.Login(_getCurrentUserUrl, _loginName, _loginPass, true, 1, "", !String.IsNullOrEmpty(CmmVariable.SysConfig.VerifyOTP) ? CmmVariable.SysConfig.VerifyOTP : "");

                            // Nếu (M_AuthenticatedHttpClient != null) thì Authen Success -> chờ Tick CmmEvent
                            // Nếu (M_AuthenticatedHttpClient == null) thì Authen Failed -> logout Account ra
                            if (CmmVariable.M_AuthenticatedHttpClient == null)
                            {
                                _mainAct.RunOnUiThread(() =>
                                {
                                    CmmEvent.ReloginRequest -= CmmEvent_ReloginRequest;
                                    CmmEvent.SyncDataBackGroundRequest -= CmmEvent_SyncDataBackGroundRequest;

                                    if (!haveErroCodeFromCookie)
                                    {
                                        _isFirstLogin = true;
                                        _mainAct.SignOut();
                                    }
                                    else
                                    {
                                        _isFirstLogin = false;
                                    }
                                });
                            }
                        }
                        else // Có thông tin User Default nhưng không có connection -> Login offline vào HomePage
                        {
                            _mainAct.RunOnUiThread(() =>
                            {
                                _tvNote.Visibility = ViewStates.Visible;
                                _tvNote.Text = CmmFunction.GetTitle("TEXT_OFFLINE", "You are in offline mode");

                                /*FragmentHomePage homePage = new FragmentHomePage();
                                _mainAct.ShowFragment(FragmentManager, homePage, "FragmentHomePage", 1);*/

                                FragmentParent parent = new FragmentParent();
                                _mainAct.ShowFragment(FragmentManager, parent, "FragmentParent", 1);
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
                        _mainAct.RunOnUiThread(() =>
                        {
                            SetVisibleLinearLogin(ViewStates.Visible);
                            _relabackground.SetBackgroundColor(Android.Graphics.Color.White);
                            //_btnBack_Startview.Visibility = ViewStates.Visible;
                            if (CTRLBase.CheckAppHasConnection())
                            {
                                _tvNote.Visibility = ViewStates.Invisible;
                                _process_Login.Visibility = ViewStates.Invisible;

                            }
                            else
                            {
                                _tvNote.Visibility = ViewStates.Visible;
                                _process_Login.Visibility = ViewStates.Invisible;

                                _tvNote.Text = CmmFunction.GetTitle("MESS_LOGIN_REQUIRE_CONNECTION", "No network connection, please try again.");
                            }
                        });
                    }
                    #endregion
                });
            }
            catch (Exception)
            {
                _mainAct.RunOnUiThread(() =>
                {
                    SetVisibleLinearLogin(ViewStates.Visible);
                    _relabackground.SetBackgroundColor(Android.Graphics.Color.White);
                    _tvNote.Visibility = ViewStates.Visible;
                   /* //_btnBack_Startview.Visibility = ViewStates.Visible;
                    CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_LOGINFAIL", "Login information is incorrect, please try again."),
                        CmmFunction.GetTitle("TEXT_LOGINFAIL", "Login information is incorrect, please try again."), "Alert", "Alert", "Close", "Close");*/
                });
            }
        }

        private async void LoginFunction()
        {
            try
            {
                CmmDroidFunction.HideSoftKeyBoard(_edtPass, _mainAct);
                if ((String.IsNullOrEmpty(_edtPass.Text) || (String.IsNullOrEmpty(_edtUser.Text))))
                {
                    CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_LOGINFAIL", "Login information is incorrect, please try again."),
                        CmmFunction.GetTitle("TEXT_LOGINFAIL1", "Vui lòng nhập email doanh nghiệp của bạn."), "Alert", "Alert", "Close", "Close");
                    // _btnBack_Startview.Visibility = ViewStates.Visible;
                    _edtPass.Enabled = true;
                    _edtUser.Enabled = true;
                    _tvLogin.Enabled = true;
                    SetVisibleLinearLogin(ViewStates.Visible);
                }
                else
                {
                    _process_Login.Visibility = ViewStates.Visible;
                    _lnTextProcess.SetBackgroundColor(new Android.Graphics.Color(Android.Graphics.Color.ParseColor("#c6ebf5")));
                    _tvNote.Visibility = ViewStates.Visible;
                    _tvNote.Text = CmmFunction.GetTitle("TEXT_CHECKINGDATA", "Checking data...");

                    _loginName = _edtUser.Text.TrimEnd();
                    _loginPass = _edtPass.Text.TrimEnd();
                    CmmVariable.SysConfig.DataLimitDay = CmmVariable.M_DataFilterDefaultDays;

                    await Task.Run(() =>
                    {
                        RegistrationDeviceInfo(); // Đăng ký device info
                        CmmEvent.ReloginRequest += CmmEvent_ReloginRequest;
                        string _getCurrentUserUrl = (CmmVariable.M_Domain.TrimEnd('/') + (new BeanUser().GetCurrentUserUrl())).Replace("<#SiteName#>", "");
                        CmmVariable.M_AuthenticatedHttpClient = CmmFunction.Login(_getCurrentUserUrl, _loginName, _loginPass, true, 1);
                        if (CmmVariable.M_AuthenticatedHttpClient != null)
                        {
                            if (!haveErroCodeFromCookie)
                            {
                                _isFirstLogin = true;
                            }
                            // Đợi tick Event
                        }
                        else
                        {
                            CmmEvent.ReloginRequest -= CmmEvent_ReloginRequest;
                            _mainAct.RunOnUiThread(() =>
                            {
                                SetVisibleLinearLogin(ViewStates.Visible);
                                _tvNote.Visibility = ViewStates.Invisible;
                                _lnTextProcess.SetBackgroundColor(new Android.Graphics.Color(Android.Graphics.Color.White));
                                _process_Login.Visibility = ViewStates.Invisible;
                                _edtPass.Enabled = true;
                                _edtUser.Enabled = true;
                                _tvLogin.Enabled = true;
                                _edtPass.Text = "";
                                _edtUser.Text = "";
                                //_btnBack_Startview.Visibility = ViewStates.Visible;
                                CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_LOGINFAIL", "Login information is incorrect, please try again."),
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
                _mainAct.RunOnUiThread(() =>
                {
                    _tvNote.Visibility = ViewStates.Invisible;
                    _process_Login.Visibility = ViewStates.Invisible;
                    _edtPass.Enabled = true;
                    _edtUser.Enabled = true;
                    _tvLogin.Enabled = true;
                    SetVisibleLinearLogin(ViewStates.Visible);
/*                    //_btnBack_Startview.Visibility = ViewStates.Visible;
                    CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_LOGINFAIL", "Login information is incorrect, please try again."),
                        CmmFunction.GetTitle("TEXT_LOGINFAIL", "Login information is incorrect, please try again."), "Alert", "Alert", "Close", "Close");*/
                });
            }
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
                        _mainAct.RunOnUiThread(() =>
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
                                    ProviderUser proUser = new ProviderUser();
                                    string langCode = proUser.GetCurrentlangCode();
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
                        _mainAct.RunOnUiThread(() =>
                        {
                            FragmentConfirmOTP fragmentConfirmOTP = new FragmentConfirmOTP(_loginName, _loginPass, _isFirstLogin);
                            _mainAct.ShowFragment(FragmentManager, fragmentConfirmOTP, "FragmentConfirmOTP", 1);
                        });
                    }
                }
                else
                {
                    _mainAct.RunOnUiThread(() =>
                    {
                        CmmVariable.SysConfig.LoginPassword = "_";
                        CmmFunction.WriteSetting();
                        SetVisibleLinearLogin(ViewStates.Visible);
                        _relabackground.SetBackgroundColor(Android.Graphics.Color.White);
                        _imgcovera.Visibility = ViewStates.Visible;
                        _ImgViewlogoVNa.Visibility = ViewStates.Gone;
                        _lnTextProcess.Visibility = ViewStates.Invisible;
                        _edtPass.Text = "";
                        _edtUser.Text = "";
                        _edtPass.Enabled = true;
                        _edtUser.Enabled = true;
                        try
                        {
                            SQLiteConnection con = new SQLiteConnection(CmmVariable.M_DataPath, false);

                            var item = con.Query<List<BeanWorkflowCategory>>("Select * from BeanWorkflowCategory limit 10");
                            con.Close();

                            if (item == null || item.Count == 0)
                            {
                                _isFirstLogin = true;
                                if (File.Exists(CmmVariable.M_DataPath))
                                {
                                    File.Delete(CmmVariable.M_DataPath);
                                    CmmFunction.InstanceDB(CmmVariable.M_DataPath);
                                }
                                if (File.Exists(CmmVariable.M_settingFileName))
                                    File.Delete(CmmVariable.M_settingFileName);
                            }
                            else
                            {
                                _isFirstLogin = false;
                            }
                        }
                        catch (Exception e)
                        {
                            _isFirstLogin = true;
                        }
                        haveErroCodeFromCookie = true;
                        CmmDroidFunction.ShowAlertDialog(_mainAct, CmmFunction.GetTitle("TEXT_LOGINFAIL", "Login information is incorrect, please try again."),
                       CmmFunction.GetTitle("TEXT_LOGINFAIL", "Login information is incorrect, please try again."), "Alert", "Alert", "Close", "Close");
                        haveErroCodeFromCookie = true;

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

        private async void GetDataFirstTimeLogin()
        {
            try
            {
                await Task.Run(async () =>
                {
                    /*if (File.Exists(CmmVariable.M_DataPath))
                        File.Delete(CmmVariable.M_DataPath);
                    CmmFunction.InstanceDB(CmmVariable.M_DataPath);*/
                    var watch = System.Diagnostics.Stopwatch.StartNew();

                    ProviderUser pUser = new ProviderUser();
                    pUser.UpdateLangData(CmmVariable.SysConfig.LangCode, false, true);
                    await pUser.UpdateAllDynamicDataAndroid(false, CmmVariable.SysConfig.DataLimitDay, true);
                    watch.Stop();
                    var elapsedMs = watch.ElapsedMilliseconds;
                    _mainAct.RunOnUiThread(() =>
                    {
                        _tvNote.Text = CmmFunction.GetTitle("TEXT_UPDATE_FINISHED", "Update finished");
                    });
                    Thread.Sleep(500);
                    //if (File.Exists(CmmVariable.M_DataLangPath))
                    //    pUser.UpdateLangData(CmmVariable.SysConfig.LangCode, true, false);
                    //else
                    //    pUser.UpdateLangData(CmmVariable.SysConfig.LangCode, false, true);

                    _mainAct.RunOnUiThread(() =>
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
                        _mainAct.ShowFragment(FragmentManager, parent, "FragmentParent", 1);
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
                    ProviderUser pUser = new ProviderUser();
                    pUser.UpdateLangData(CmmVariable.SysConfig.LangCode, false, true);
                    //pUser.UpdateAllMasterData(true);
                    await pUser.UpdateAllDynamicDataAndroid(true);
                    _mainAct.RunOnUiThread(() =>
                    {
                        _tvNote.Text = CmmFunction.GetTitle("TEXT_UPDATE_FINISHED", "Update finished");
                    });
                    //if (File.Exists(CmmVariable.M_DataLangPath))
                    //    pUser.UpdateLangData(CmmVariable.SysConfig.LangCode, true, false);
                    //else
                    //    pUser.UpdateLangData(CmmVariable.SysConfig.LangCode, false, true);
                    Thread.Sleep(500);                   
                    _mainAct.RunOnUiThread(() =>
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
                        _mainAct.ShowFragment(FragmentManager, parent, "FragmentParent", 1);
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
                if (_mainAct == null)
                    _mainAct = mainAct;
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

        #region Finger Print
        private void HandleFingerPrint()
        {
            try
            {
                if (_bioHelper == null)
                    _bioHelper = new CustomBiometricHelper();

                if (!_bioHelper.IsBiometricPromptEnabled())
                {
                    CmmDroidFunction.WriteTrackingError(this.GetType().Name, "HandleFingerPrint - IsBiometricPromptEnabled return", null);
                    return;
                }

                if (!_bioHelper.IsHardwareSupported(_rootView.Context))
                {
                    CmmDroidFunction.WriteTrackingError(this.GetType().Name, "HandleFingerPrint - IsHardwareSupported return", null);
                    return;
                }

                if (!_bioHelper.IsFingerprintAvailable(_rootView.Context))
                {
                    CmmDroidFunction.WriteTrackingError(this.GetType().Name, "HandleFingerPrint - IsFingerprintAvailable return", null);
                    return;
                }

                try
                {
                    if (!(_mainAct._keyguardManager).IsKeyguardSecure)
                    {
                        CmmDroidFunction.ShowAlertDialogWithAction(_mainAct, CmmVariable.SysConfig.LangCode.Equals(CmmDroidVariable.M_SysLangVN) ?
                            "Vui lòng thiết lập khóa màn hình để sử dụng dấu vân tay" :
                            "Vui lòng thiết lập khóa màn hình để sử dụng dấu vân tay",
                        _actionPositiveButton: null,
                        _actionNegativeButton: new Action(() => { }),
                        _title: CmmDroidFunction.GetApplicationName(_rootView.Context),
                        _positive: CmmFunction.GetTitle("TEXT_AGREE", "Đồng ý"),
                        _negative: CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"));

                        return;
                    }
                }
                catch (Exception ex)
                {
                    CmmDroidFunction.WriteTrackingError(this.GetType().Name, "HandleFingerPrint - IsFingerprintAvailable catch IsKeyguardSecure", null);
                    return;
                }

                BiometricPrompt.Builder _bioBuilder = _bioHelper.CreateBiometricPromptBuilder(_mainAct, _rootView.Context, this);
                if (_bioBuilder != null)
                {
                    BiometricPrompt _bioPromt = _bioBuilder.Build();
                    _bioPromt.Authenticate(_bioHelper.BuildCryptoObject(), new CancellationSignal(), _mainAct.MainExecutor, GetAuthenticationCallback());
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "HandleFingerPrint", ex);
#endif
            }
        }

        /// <summary>
        /// Implement sự kiện click Cancel trên CreateBiometricPromptBuilder
        /// </summary>
        /// <param name="dialog"></param>
        /// <param name="which"></param>
        public void OnClick(IDialogInterface dialog, int which)
        {
            try
            {

            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "OnClick", ex);
#endif
            }
        }

        /// <summary>
        /// Hàm Trigger event từ Fingerprint
        /// </summary>
        /// <returns></returns>

        public BiometricPrompt.AuthenticationCallback GetAuthenticationCallback()
        {
            var callback = new CustomBiometricAuthenticationCallback
            {
                Success = (BiometricPrompt.AuthenticationResult result) =>
                {
                    if (result.CryptoObject.Cipher != null)
                    {
                        // Xác thực thành công
                    };
                },
                Error = (BiometricErrorCode errorCode, Java.Lang.ICharSequence errString) =>
                {
                    // Xác thực thất bại
                },
                Failed = () =>
                {

                },
                Help = (BiometricAcquiredStatus helpCode, Java.Lang.ICharSequence helpString) =>
                {
                    // Hỗ trợ người dùng khi xác thực

                }
            };
            return callback;
        }
        #endregion

        #endregion
    }
}
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.iPad.IOSClass;
using BPMOPMobile.iPad.ViewControllers;
using CoreGraphics;
using Foundation;
using SlideMenuControllerXamarin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UIKit;

namespace BPMOPMobile.iPad
{
    public partial class StartView : UIViewController
    {
        AppDelegate appD;
        private string _pathToDatabase;
        private string documentFolder;
        NSObject app_ver;
        string serverConfigApp_version = "1.0.0";
        string loginName = string.Empty;
        string loginPass = string.Empty;

        private NSObject _willResignActiveNotificationObserver;
        private NSObject _didBecomeActiveNotificationObserver;
        bool isSavePass = true, isInitSuccess = false;
        int loadingStep = 0;

        public StartView(IntPtr handle) : base(handle)
        {
            documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            _pathToDatabase = Path.Combine(Path.Combine(documentFolder, CmmVariable.M_DataPath));
            appD = UIApplication.SharedApplication.Delegate as AppDelegate;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            app_ver = NSBundle.MainBundle.InfoDictionary["CFBundleVersion"];

            CmmIOSFunction.ResignFirstResponderOnTap(this.View);
            Viewconfiguration();
            LoadContent();
            //GetLangCode();
            InitAppConfig();

            #region delegate
            CmmEvent.SyncDataBackGroundRequest += CmmEvent_SyncDataBackGroundRequest;
            BT_login.TouchUpInside += BT_login_TouchUpInside;
            BT_register.TouchUpInside += BT_register_TouchUpInside;
            #endregion
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            appD.NavController = this.NavigationController;

            //_willResignActiveNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, KeyBoardUpNotification);
            //_didBecomeActiveNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyBoardDownNotification);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            if (_willResignActiveNotificationObserver != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(_willResignActiveNotificationObserver);

            if (_didBecomeActiveNotificationObserver != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(_didBecomeActiveNotificationObserver);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            lbl_loading.Text = "Please wait a minute...";
        }

        #region public - private method
        private void Viewconfiguration()
        {
            BT_login.Layer.BorderColor = UIColor.FromRGB(219, 164, 16).CGColor;
            BT_login.Layer.BorderWidth = 1f;
            BT_login.Layer.CornerRadius = 5;
            BT_login.Layer.MasksToBounds = true;
            BT_login.ClipsToBounds = true;

            BT_register.Layer.CornerRadius = 5;
            BT_register.Layer.MasksToBounds = true;
            BT_register.ClipsToBounds = true;
        }

        private void LoadContent()
        {
            #region DEBUG
            //var htmlString = "<p align=\"center\"><img alt=\"\" src=\"https://www.designfreelogoonline.com/wp-content/uploads/2017/05/000840-Infinity-logo-maker-Free-infinito-Logo-design-06.png\"></p>";
            //webView_content.LoadHtmlString(htmlString, null);

            //NSUrl htmlUrl = new NSUrl("Images/AdvertiseHtmlFile.html", false);
            //NSUrlRequest urelRequest = new NSUrlRequest(htmlUrl);
            //webView_content.ContentMode = UIViewContentMode.Center;

            //tf_account.Text = "";
            //tf_pass.Text = "VuThao123!@#";

            //if (CmmVariable.M_AuthenticatedHttpClient == null)
            //{
            //    HttpClientHandler handler = new HttpClientHandler();
            //    handler.CookieContainer = new CookieContainer();
            //    HttpClient client = new HttpClient(handler);
            //    CmmVariable.M_AuthenticatedHttpClient = client;
            //}
            #endregion
        }

        //private void GetLangCode()
        //{
        //    try
        //    {
        //        if (File.Exists(CmmVariable.M_DataLangPath))
        //        {
        //            tf_pass.Placeholder = CmmFunction.GetTitle("K_Pass", "Mật khẩu");
        //            tf_account.Placeholder = CmmFunction.GetTitle("K_Uname", "Tài khoản");
        //            BT_login.SetTitle(CmmFunction.GetTitle("K_Login", "Đăng nhập"), UIControlState.Normal);
        //            lbl_loading.Text = CmmFunction.GetTitle("K_Mess_Wait", "Xin vui lòng đợi...");
        //        }

        //    }
        //    catch (Exception ex)
        //    { }
        //}

        private async void InitAppConfig()
        {
            loading_indicator.StartAnimating();
            bool res = false;
            try
            {
                view_loading.Hidden = false;
                CmmVariable.M_DataPath = Path.Combine(documentFolder, CmmVariable.M_DataPath);
                CmmVariable.M_settingFileName = Path.Combine(documentFolder, CmmVariable.M_settingFileName);
                CmmVariable.M_DataFolder = Path.Combine(documentFolder, CmmVariable.M_DataFolder);
                CmmVariable.M_Folder_Avatar = Path.Combine(documentFolder, CmmVariable.M_Folder_Avatar);
                CmmVariable.M_DataLangPath = Path.Combine(documentFolder, CmmVariable.M_DataLangPath);

                CmmVariable.SysConfig = new ConfigVariable();

                if (CmmFunction.ReadSetting())//lấy cấu hình app
                {
                    string str_app_ver = app_ver.ToString();

                    if (string.IsNullOrEmpty(CmmVariable.SysConfig.AppConfigVersion) || CmmFunction.CheckIsNewVer(str_app_ver, CmmVariable.SysConfig.AppConfigVersion))
                    {
                        var dataFile = CmmVariable.M_DataPath;
                        if (File.Exists(dataFile) && CmmVariable.M_RenewDB)
                        {
                            try
                            {
                                File.Delete(dataFile);
                            }
                            catch
                            {
                                try
                                {
                                    if (File.Exists(dataFile))
                                        File.Delete(dataFile);
                                }
                                catch { }
                            }
                        }
                    }

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(CmmVariable.M_Domain);
                    CmmVariable.M_AuthenticatedHttpClient = client;
                    ProviderBase provider = new ProviderBase();
                    ProviderUser p_user = new ProviderUser();

                    List<BeanSettings> lst_setting = p_user.GetCustomerAppVersion();
                    if (lst_setting != null && lst_setting.Count > 0)
                    {
                        var item = lst_setting.Where(i => i.KEY == "Ipad_AppVer").ToList();
                        if (item != null && item.Count > 0)
                            serverConfigApp_version = item[0].VALUE;

                        var M_ResourceViewID_ToMe = lst_setting.Where(i => i.KEY == "MOBILE_RESOURCEVIEWID_TOME").ToList();
                        if (M_ResourceViewID_ToMe != null && M_ResourceViewID_ToMe.Count > 0)
                            CmmVariable.M_ResourceViewID_ToMe = M_ResourceViewID_ToMe[0].VALUE;

                        var M_ResourceViewID_FromMe = lst_setting.Where(i => i.KEY == "MOBILE_RESOURCEVIEWID_FROMME").ToList();
                        if (M_ResourceViewID_FromMe != null && M_ResourceViewID_FromMe.Count > 0)
                            CmmVariable.M_ResourceViewID_FromMe = M_ResourceViewID_FromMe[0].VALUE;
                    }

                    string string_app_ver = app_ver.ToString();
                    if (CmmFunction.CheckIsNewVer(serverConfigApp_version, string_app_ver)) //appVer < serverVer
                    {
                        UIAlertController alert = UIAlertController.Create("Thông báo"
                                            , "Vui lòng cập nhật phiên bản mới để có trải nghiệm tốt nhất, xin cảm ơn."
                                            , UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("Đóng", UIAlertActionStyle.Destructive, (UIAlertAction) =>
                        {
                            System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();// terminate app
                        }));

                        alert.AddAction(UIAlertAction.Create("Cập nhật", UIAlertActionStyle.Default, (UIAlertAction) =>
                        {
                            NSString iTunesLink;
                            iTunesLink = new NSString(@"https://apps.apple.com/us/app/vna-bpm-hd/id1620118270");

                            UIApplication.SharedApplication.OpenUrl(NSUrl.FromString(iTunesLink));

                            System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow(); // terminate app
                        }));

                        this.PresentViewController(alert, true, null);
                    }
                    else
                    {
                        #region khoi tao data dùng chung

                        CmmFunction.InstanceDB(CmmVariable.M_DataPath);

                        //cap nhat app version
                        CmmVariable.SysConfig.AppConfigVersion = app_ver.ToString();
                        CmmFunction.WriteSetting();

                        //kiểm tra folder có tồn tại hay chưa
                        CmmIOSFunction.CheckFolderExists();
                        #endregion

                        #region checkuser authentication
                        //kiem tra quyen user
                        loginName = CmmVariable.SysConfig.LoginName;
                        loginPass = CmmVariable.SysConfig.LoginPassword;

                        if (CmmVariable.SysConfig != null && !string.IsNullOrEmpty(loginName) && !string.IsNullOrEmpty(loginPass))
                        {
                            await Task.Run(() =>
                            {
                                CmmIOSFunction.CheckDomain(loginName);
                                if (Reachability.detectNetWork())
                                {
                                    CmmEvent.ReloginRequest += CmmEvent_ReloginRequest;
                                    Thread.Sleep(TimeSpan.FromSeconds(1));
                                    //Tiến hành autologin
                                    CmmVariable.SysConfig.DeviceInfo = CmmIOSFunction.collectDeviceInfo();

                                    string getCurrentUserUrl = CmmVariable.M_Domain.TrimEnd('/') + (new BeanUser().GetCurrentUserUrl());
                                    var userName = loginName.Contains("@vuthao") ? loginName.Split('@')[0] : loginName;
                                    if (!string.IsNullOrEmpty(CmmVariable.SysConfig.VerifyOTP))
                                        CmmVariable.M_AuthenticatedHttpClient = CmmFunction.Login(getCurrentUserUrl, userName, loginPass, true, 1, null, CmmVariable.SysConfig.VerifyOTP);
                                    else
                                        CmmVariable.M_AuthenticatedHttpClient = CmmFunction.Login(getCurrentUserUrl, userName, loginPass, true, 1, null, null);

                                    if (CmmVariable.M_AuthenticatedHttpClient != null) // ket noi voi server, authent thanh cong
                                    {
                                        Console.WriteLine("StartView - Autologin success");
                                        /*ProviderBase p_base = new ProviderBase();
                                        string url = CmmVariable.M_Domain + "/" + CmmVariable.SysConfig.AvatarPath.Split('?')[0];
                                        string localpath = Path.Combine(documentFolder, "avatar.jpg");
                                        p_base.DownloadFile(url, localpath, CmmVariable.M_AuthenticatedHttpClient);
                                        p_base.UpdateAllDynamicData(true, CmmVariable.SysConfig.DataLimitDay, false);*/

                                        isInitSuccess = true;
                                    }
                                    //khong co httpclient
                                    else
                                    {
                                        InvokeOnMainThread(() =>
                                        {
                                            //CmmIOSFunction.commonAlertMessage(this, "BPM", "Login information is incorrect, please try again!");
                                            //RedirectToMainView();

                                            UIAlertController alert = UIAlertController.Create("Thông báo"
                                            , "Login information is incorrect, please try again!"
                                            , UIAlertControllerStyle.Alert);
                                            alert.AddAction(UIAlertAction.Create("Đóng", UIAlertActionStyle.Destructive, (UIAlertAction) =>
                                            {
                                                ShowLoginForm();
                                            }));
                                            this.PresentViewController(alert, true, null);
                                        });
                                    }
                                }
                                else // khong co ket noi internet
                                {
                                    InvokeOnMainThread(() =>
                                    {
                                        //CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("K_Offline", "Không kết nối được với server, chuyển qua chế độ offline."));
                                        //RedirectToMainView();

                                        UIAlertController alert = UIAlertController.Create("Thông báo"
                                           , CmmFunction.GetTitle("K_Offline", "Không kết nối được với server, chuyển qua chế độ offline.")
                                           , UIAlertControllerStyle.Alert);
                                        alert.AddAction(UIAlertAction.Create("Đóng", UIAlertActionStyle.Destructive, (UIAlertAction) =>
                                        {
                                            ShowLoginForm();
                                        }));
                                        this.PresentViewController(alert, true, null);
                                    });
                                }
                            });
                        }
                        else // có cấu hình nhưng không có thông tin đăng nhập 
                        {
                        }
                        #endregion
                    }
                }
                else
                {
                    CmmFunction.InstanceDB(CmmVariable.M_DataPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("StartView - initAppConfig - Err: " + ex.ToString());

                /*view_loading.Hidden = true;
                loading_indicator.StopAnimating();
                view_login.Hidden = false;*/
                //ShowLoginForm();
            }
            finally
            {
                //if (isInitSuccess || (!isInitSuccess && CmmVariable.SysConfig != null && !string.IsNullOrEmpty(loginName) && !string.IsNullOrEmpty(loginPass)))
                //{
                //    //RedirectToMainView();
                //}
                //else
                //    ShowLoginForm();

                if (!isInitSuccess)
                    ShowLoginForm();
            }
        }

        void ShowLoginForm()
        {
            CmmEvent.SyncDataBackGroundRequest -= CmmEvent_SyncDataBackGroundRequest;
            view_login.Hidden = true;
            view_loading.Hidden = true;
            loading_indicator.StopAnimating();
            BT_login_TouchUpInside(null, null);
        }

        void RedirectToMainView()
        {
            CmmEvent.SyncDataBackGroundRequest -= CmmEvent_SyncDataBackGroundRequest;
            //isInitSuccess = true;
            appD.NavController = this.NavigationController;
            MainView mainview = (MainView)Storyboard.InstantiateViewController("MainView");
            MenuView menuview = (MenuView)Storyboard.InstantiateViewController("MenuView");
            menuview.SetContent(mainview);
            SlideMenuController slideMenuController = new SlideMenuController(mainview, menuview, null);
            var widthMenu = (this.View.Frame.Width / 3.5f) + 20;
            SlideMenuOptions.ContentViewScale = 1.0f;
            slideMenuController.LeftPanGesture = new UIPanGestureRecognizer();
            slideMenuController.ChangeLeftViewWidth(widthMenu);
            appD.SlideMenuController = slideMenuController;
            appD.mainView = mainview;
            appD.menu = menuview;
            appD.NavController.PushViewController(slideMenuController, true);
        }

        #endregion

        #region event

        private void BT_register_TouchUpInside(object sender, EventArgs e)
        {
            RegisterViewController loginViewController = (RegisterViewController)Storyboard.InstantiateViewController("RegisterViewController");
            this.NavigationController.PushViewController(loginViewController, true);
        }

        private void BT_login_TouchUpInside(object sender, EventArgs e)
        {
            LoginViewController loginViewController = (LoginViewController)Storyboard.InstantiateViewController("LoginViewController");
            this.NavigationController.PushViewController(loginViewController, true);
            //this.PresentViewController(loginViewController, true, null);
        }

        private async void CmmEvent_ReloginRequest(object sender, CmmEvent.LoginEventArgs e)
        {
            try
            {
                if (e != null)
                {
                    if (e.IsSuccess)
                    {
                        if (e.ErrCode == "OTP")
                        {
                            InvokeOnMainThread(() =>
                            {
                                //TranslationToOTPView(true);
                            });
                        }
                        else if (string.Compare(e.ErrCode, CmmFunction.errPassword) == 0)
                        {
                            InvokeOnMainThread(() =>
                            {
                                //CmmIOSFunction.commonAlertMessage(this, "VNA BPM", "Tài khoản đã bị vô hiệu hóa");
                                ShowLoginForm();
                                CmmIOSFunction.commonAlertMessage(this, "VNA BPM", CmmFunction.GetTitle("TEXT_LOGINFAIL", "Login information is incorrect, please try again!"));
                            });
                        }
                        else
                        {
                            if (e.UserInfo.Status.HasValue && e.UserInfo.Status.Value == 1)
                            {
                                //if (!string.IsNullOrEmpty(e.VerifyOTP))
                                //    CmmVariable.SysConfig.VerifyOTP = e.VerifyOTP;

                                CmmVariable.SysConfig.UserId = e.UserInfo.ID;
                                CmmVariable.SysConfig.UserIdNum = e.UserInfo.AccountID;
                                CmmVariable.SysConfig.Title = e.UserInfo.FullName;
                                CmmVariable.SysConfig.DisplayName = e.UserInfo.FullName;
                                CmmVariable.SysConfig.Email = e.UserInfo.Email;
                                CmmVariable.SysConfig.Department = e.UserInfo.Department;
                                CmmVariable.SysConfig.Address = e.UserInfo.Address;
                                CmmVariable.SysConfig.PositionID = e.UserInfo.PositionID;
                                CmmVariable.SysConfig.PositionTitle = e.UserInfo.PositionTitle;
                                CmmVariable.SysConfig.Mobile = e.UserInfo.Mobile;
                                CmmVariable.SysConfig.LoginName = loginName;
                                CmmVariable.SysConfig.LoginPassword = loginPass;
                                CmmVariable.SysConfig.SiteName = e.UserInfo.SiteName;
                                CmmVariable.SysConfig.LangCode = e.UserInfo.Language == 1033 || e.UserInfo.Language == 1066 ? e.UserInfo.Language.ToString() : "1066";
                                CmmVariable.SysConfig.AccountName = e.UserInfo.AccountName;

                                //ProviderUser p_user = new ProviderUser();
                                //p_user.UpdateLangData(CmmVariable.SysConfig.LangCode, false, true);

                                if (!string.IsNullOrEmpty(CmmVariable.SysConfig.AvatarPath) && CmmVariable.SysConfig.AvatarPath != e.UserInfo.ImagePath)
                                {
                                    ProviderBase p_base = new ProviderBase();
                                    string url = CmmVariable.M_Domain + "/" + CmmVariable.SysConfig.AvatarPath.Split('?')[0];
                                    string localpath = Path.Combine(documentFolder, "avatar.jpg");
                                    p_base.DownloadFile(url, localpath, CmmVariable.M_AuthenticatedHttpClient);
                                }

                                CmmVariable.SysConfig.AvatarPath = e.UserInfo.ImagePath;
                                CmmFunction.WriteSetting();

                                await new ProviderBase().GetAllDynamicData_FirstLogin(true, CmmVariable.SysConfig.DataLimitDay, true);
                                //await Task.Run(() =>
                                //{
                                //new ProviderBase().UpdateAllDynamicData(true, CmmVariable.SysConfig.DataLimitDay, false);

                                InvokeOnMainThread(() =>
                                {
                                    RedirectToMainView();
                                });
                                //});
                            }
                            else
                            {
                                InvokeOnMainThread(() =>
                                {
                                    //CmmIOSFunction.commonAlertMessage(this, "VNA BPM", "Tài khoản đã bị vô hiệu hóa");
                                    ShowLoginForm();
                                    CmmIOSFunction.commonAlertMessage(this, "VNA BPM", CmmFunction.GetTitle("TEXT_LOGINFAIL", "Login information is incorrect, please try again!"));
                                });
                            }
                        }
                    }
                    else
                    {
                        if (string.Compare(e.ErrCode, CmmFunction.errPassword) == 0)
                        {
                            InvokeOnMainThread(() =>
                            {
                                //CmmIOSFunction.commonAlertMessage(this, "VNA BPM", "Login information is incorrect, please try again!");
                                ShowLoginForm();
                                CmmIOSFunction.commonAlertMessage(this, "VNA BPM", CmmFunction.GetTitle("TEXT_LOGINFAIL", "Login information is incorrect, please try again!"));
                            });
                        }
                        else
                            Console.WriteLine("Relogin failed");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("StartView - CmmEvent_ReloginRequest - Err: " + ex.ToString());
            }

            CmmEvent.ReloginRequest -= CmmEvent_ReloginRequest;
        }

        private void CmmEvent_SyncDataBackGroundRequest(object sender, CmmEvent.UpdateBackgroundEventArgs e)
        {
            //Console.WriteLine("BeanName: " + e.BeanName + "|| CountItem: " + e.ErrMess);
            int min = loadingStep;
            int max = loadingStep + 12;
            Random rnd = new Random();

            InvokeOnMainThread(() =>
            {
                if (loadingStep < 10)
                    lbl_loading.Text = "Connecting to server... " + rnd.Next(min, max).ToString() + "%";
                else if (loadingStep < 30)
                    lbl_loading.Text = "Verifying user information... " + rnd.Next(min, max).ToString() + "%";
                else if (loadingStep < 60)
                    lbl_loading.Text = "Verifying user information... " + rnd.Next(min, max).ToString() + "%";
                else
                    //lbl_loading.Text = "Loading data..." + rnd.Next(min, max).ToString() + "%";
                    lbl_loading.Text = "Loading data... " + rnd.Next(min > 100 ? 90 : min, 100).ToString() + "%";
            });
            loadingStep = max;
        }
        #endregion
    }
}
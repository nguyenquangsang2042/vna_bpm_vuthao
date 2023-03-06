using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CoreGraphics;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobileV1.iOS.IOSClass;
using Foundation;
using ObjCRuntime;
using SidebarNavigation;
using UIKit;
using SlideMenuControllerXamarin;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class StartView : UIViewController
    {
        LoginLoading _loadPop;
        AppDelegate appD;
        private string _pathToDatabase;
        private string documentFolder;   // Controller that activated the keyboard
        NSObject app_ver;
        string serverConfigApp_version = "1.0.0";
        private bool isShowKeyboard = false;
        private UITapGestureRecognizer gestureRecognizer;
        string loginName = string.Empty;
        string loginPass = string.Empty;
        bool is_fromLoginForm = false;
        int loadingStep = 0;

        public StartView(IntPtr handle) : base(handle)
        {
            documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            _pathToDatabase = Path.Combine(Path.Combine(documentFolder, CmmVariable.M_DataPath));
            appD = UIApplication.SharedApplication.Delegate as AppDelegate;
        }

        #region override
        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            constraint_topLogo.Constant = UIScreen.MainScreen.Bounds.Height * ((float)90 / 736);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewConfiguration();
            //gestureRecognizer = new UITapGestureRecognizer(Self, new Selector("hideKeyboard"));
            //gestureRecognizer.ShouldReceiveTouch += delegate (UIGestureRecognizer recognizer, UITouch touch)
            //{
            //    var name = touch.View.Class.Name;
            //    var touchName = touch.View.Superview.Class.Name;

            //    if (touchName == "VuThao_iOS_ViewControllers_MainView_Todo_cell_custom" || touchName == "UITableViewCellContentView")
            //        return false;
            //    else
            //        return true;
            //};
            //View.AddGestureRecognizer(gestureRecognizer);
            //this.View.AddGestureRecognizer(gestureRecognizer);

            app_ver = NSBundle.MainBundle.InfoDictionary["CFBundleVersion"];

#if DEBUG
            //vna 
            //txt_Password.Text = "vna@08062021";
            //txt_LoginName.Text = "testpp";

            //vna live
            //txt_Password.Text = "vnaportal123!@#";
            //txt_LoginName.Text = "bpmadminsite";

            //vna dev
            //txt_Password.Text = "vnabpm@123";
            //txt_LoginName.Text = "fvnabpmadmin";

            txt_Password.Text = "VTlamson123!@#";
            txt_LoginName.Text = "tphong2";

            //txt_Password.Text = "bpm@123123";
            //txt_LoginName.Text = "trial.tphong1";

            //txt_Password.Text = "iloveyou@2025";
            //txt_LoginName.Text = "ngocvd";

            //txt_Password.Text = "VuThao123!@#";
            //txt_LoginName.Text = "tphong1";

            //txt_Password.Text = "Aa123456";
            //txt_LoginName.Text = "thuyntt";
#endif

            //GetLangCode();
            InitAppConfig();

            #region delegate
            CmmEvent.SyncDataBackGroundRequest += CmmEvent_SyncDataBackGroundRequest;
            //Cho nay de lam gi
            //CmmEvent.SyncDataBackgroundResultRequest += CmmEvent_SyncDataBackgroundRequest;
            BT_login.TouchUpInside += BT_login_TouchUpInside;
            BT_register.TouchUpInside += BT_register_TouchUpInside;
            BT_fanpage.TouchUpInside += BT_fanpage_TouchUpInside;
            BT_webpage.TouchUpInside += BT_webpage_TouchUpInside;
            BT_phone.TouchUpInside += BT_phone_TouchUpInside;
            #endregion
        }

        private void BT_phone_TouchUpInside(object sender, EventArgs e)
        {
            CmmIOSFunction.commonAlertMessage(this, "Thông báo", "BT_phone");
        }

        private void BT_webpage_TouchUpInside(object sender, EventArgs e)
        {
            CmmIOSFunction.commonAlertMessage(this, "Thông báo", "BT_webpage");
        }

        private void BT_fanpage_TouchUpInside(object sender, EventArgs e)
        {
            CmmIOSFunction.commonAlertMessage(this, "Thông báo", "BT_fanpage");
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            appD.NavController = this.NavigationController;
            lbl_loading.Text = "Please wait a minute...";
        }

        private void BT_register_TouchUpInside(object sender, EventArgs e)
        {
            RegisterViewController registerViewController = (RegisterViewController)Storyboard.InstantiateViewController("RegisterViewController");
            this.NavigationController.PushViewController(registerViewController, true);
        }

        #endregion

        #region private - public method
        private void GetLangCode()
        {
            try
            {
                CmmVariable.M_DataLangPath = Path.Combine(documentFolder, CmmVariable.M_DataLangPath);
                if (File.Exists(CmmVariable.M_DataLangPath))
                {
                    txt_LoginName.Placeholder = CmmFunction.GetTitle("TEXT_ACCOUNTNAME", "Account name");
                    txt_Password.Placeholder = CmmFunction.GetTitle("TEXT_PASSWORD", "Password");
                    BT_login.SetTitle(CmmFunction.GetTitle("TEXT_LOGIN", "Đăng nhập"), UIControlState.Normal);
                    lbl_loading.Text = "Please wait a minute...";// CmmFunction.GetTitle("TEXT_LOADING", "Xin vui lòng đợi...");
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void ViewConfiguration()
        {
            try
            {
                //var htmlString = "<p align=\"center\"><img alt=\"\" src=\"https://www.designfreelogoonline.com/wp-content/uploads/2017/05/000840-Infinity-logo-maker-Free-infinito-Logo-design-06.png\"></p>";
                //webkit_Ad.LoadHtmlString(htmlString, null);

                NSUrl bg_url = new NSUrl("Images/img_iphone_AdMenu.png", false);
                //webkit_Ad.LoadFileUrl(bg_url, bg_url);

                txt_LoginName.AttributedPlaceholder = new NSAttributedString("Account name", new UIStringAttributes
                {
                    ForegroundColor = UIColor.FromRGB(94, 94, 94),
                    Font = UIFont.FromName("Arial-ItalicMT", 15f)
                });
                txt_Password.AttributedPlaceholder = new NSAttributedString("Password", new UIStringAttributes
                {
                    ForegroundColor = UIColor.FromRGB(94, 94, 94),
                    Font = UIFont.FromName("Arial-ItalicMT", 15f)
                });
                //view_user.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
                //view_user.Layer.BorderWidth = 1;
                //view_user.Layer.CornerRadius = 5;

                view_loginForm.Hidden = true;

                view_user_pass.Layer.CornerRadius = 5;
                view_user_pass.ClipsToBounds = true;

                BT_login.Layer.CornerRadius = 5;
                BT_login.ClipsToBounds = true;
                BT_login.Layer.BorderColor = UIColor.FromRGB(219, 164, 16).CGColor;
                BT_login.Layer.BorderWidth = 0.5f;

                BT_register.Layer.CornerRadius = 5;
                BT_register.ClipsToBounds = true;

                BT_fanpage.SetTitle("", UIControlState.Normal);
                BT_webpage.SetTitle("", UIControlState.Normal);
                BT_phone.SetTitle("", UIControlState.Normal);

                BT_fanpage.Enabled = false;
                BT_webpage.Enabled = false;
                BT_phone.Enabled = false;

                //BT_login.Layer.ShadowColor = UIColor.LightGray.ColorWithAlpha(1).CGColor;
                //BT_login.Layer.BorderWidth = 1;
                //BT_login.Layer.BorderColor = UIColor.White.CGColor;
                //BT_login.Layer.ShadowRadius = 2;
                //BT_login.Layer.ShadowOffset = new CGSize(-1, 0);
                //BT_login.Layer.ShadowOpacity = 1f;
                //BT_login.Layer.MasksToBounds = false;

                //iphone x
                //BT_login.Layer.ShadowPath = UIBezierPath.FromRoundedRect(new CGRect(BT_login.Bounds.X, BT_login.Bounds.Y, BT_login_constraint_width.Constant - 40, BT_login.Bounds.Height), 5).CGPath;
                //BT_login.Layer.ShadowPath = UIBezierPath.FromRoundedRect(new CGRect(BT_login.Bounds.X, BT_login.Bounds.Y, BT_login_constraint_width.Constant, BT_login.Bounds.Height), 5).CGPath;

            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("StartView - ViewConfiguration - Err: " + ex.ToString());
#endif
            }
        }

        private async void InitAppConfig()
        {
            //loading_indicator.StartAnimating();
            //bool res = false;
            try
            {
                loading_view.Hidden = false;
                CmmVariable.M_DataPath = Path.Combine(documentFolder, CmmVariable.M_DataPath);
                CmmVariable.M_DataLangPath = Path.Combine(documentFolder, CmmVariable.M_DataLangPath);
                CmmVariable.M_settingFileName = Path.Combine(documentFolder, CmmVariable.M_settingFileName);
                CmmVariable.M_DataFolder = Path.Combine(documentFolder, CmmVariable.M_DataFolder);
                CmmVariable.M_Folder_Avatar = Path.Combine(documentFolder, CmmVariable.M_Folder_Avatar);

                CmmVariable.SysConfig = new ConfigVariable();
#if DEBUG
                Console.WriteLine("Database path - " + CmmVariable.M_DataPath);
#endif

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
                        var item = lst_setting.Where(i => i.KEY == "IOS_AppVer").ToList();
                        if (item != null && item.Count > 0)
                            serverConfigApp_version = item[0].VALUE;

                        //MOBILE_RESOURCEVIEWID_TOME
                        var resourceViewID_ToMe = lst_setting.Where(i => i.KEY == "MOBILE_RESOURCEVIEWID_TOME").ToList();
                        if (resourceViewID_ToMe != null && resourceViewID_ToMe.Count > 0)
                            CmmVariable.M_ResourceViewID_ToMe = resourceViewID_ToMe[0].VALUE;

                        //MOBILE_RESOURCEVIEWID_FROMME
                        var resourceViewID_FromMe = lst_setting.Where(i => i.KEY == "MOBILE_RESOURCEVIEWID_FROMME").ToList();
                        if (resourceViewID_FromMe != null && resourceViewID_FromMe.Count > 0)
                            CmmVariable.M_ResourceViewID_FromMe = resourceViewID_FromMe[0].VALUE;
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
                                                                                             //GetLoginData(e.UserInfo.ImagePath, is_fromLoginForm);
                        }));

                        alert.AddAction(UIAlertAction.Create("Cập nhật", UIAlertActionStyle.Default, (UIAlertAction) =>
                        {
                            NSString iTunesLink;
                            iTunesLink = new NSString(@"itms://itunes.apple.com/us/app/vna-bpm/id1619953695?mt=8"); /// Do public app theo kiểu private nên khi upadate thì dẫn user đến link purchased

                            UIApplication.SharedApplication.OpenUrl(NSUrl.FromString(iTunesLink));

                            System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow(); // terminate app
                        }));

                        this.PresentViewController(alert, true, null);
                    }
                    else
                    {
                        #region khoi tao data dùng chung

                        //kiem tra DB neu chu co thi tao moi
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
                                    var userName = loginName.Contains('@') ? loginName.Split('@')[0] : loginName;
                                    if (!string.IsNullOrEmpty(CmmVariable.SysConfig.VerifyOTP))
                                        CmmVariable.M_AuthenticatedHttpClient = CmmFunction.Login(getCurrentUserUrl, userName, loginPass, true, 1, null, CmmVariable.SysConfig.VerifyOTP);
                                    else
                                        CmmVariable.M_AuthenticatedHttpClient = CmmFunction.Login(getCurrentUserUrl, userName, loginPass, true, 1, null, null);

                                    if (CmmVariable.M_AuthenticatedHttpClient != null) // ket noi voi server, authent thanh cong
                                    {
                                        Console.WriteLine("StartView - Autologin success");
                                    }
                                    else
                                    {
                                        InvokeOnMainThread(() =>
                                        {
                                            RedirectToMainView();
                                        });
                                    }
                                }
                                else // khong co ket noi internet
                                {
                                    InvokeOnMainThread(() =>
                                    {
                                        loading_view.Hidden = true;

                                        CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("K_Offline", "Không kết nối được với server, chuyển qua chế độ offline."));
                                        RedirectToMainView();
                                    });
                                }
                            });
                        }
                        else // có cấu hình nhưng không có thông tin đăng nhập 
                        {
                            ShowLoginForm();
                        }
                        #endregion
                    }
                }
                else // không có thông tin cấu hình,. chuyển sang view login
                {
                    CmmFunction.InstanceDB(CmmVariable.M_DataPath);
                    ShowLoginForm();
                }
            }
            catch (Exception ex)
            {
                view_loginForm.Hidden = false;
                loading_view.Hidden = true;
                Console.WriteLine("StartView - initAppConfig - Err: " + ex.ToString());
            }
        }

        void ShowLoginForm()
        {
            CmmEvent.SyncDataBackGroundRequest -= CmmEvent_SyncDataBackGroundRequest;
            view_loginForm.Hidden = true;// !doShow;
            loading_view.Hidden = true;
            appD.NavController = this.NavigationController;
            BT_login_TouchUpInside(null, null);
        }

        void RedirectToMainView()
        {
            CmmEvent.SyncDataBackGroundRequest -= CmmEvent_SyncDataBackGroundRequest;
            MainView mainview = (MainView)Storyboard.InstantiateViewController("MainView");
            mainview.setContent(true);
            MenuView menuview = (MenuView)Storyboard.InstantiateViewController("MenuView");
            //RootNavigation rootNavigation = (RootNavigation)Storyboard.InstantiateViewController("RootNavigation");
            menuview.SetContent(mainview);
            //menuview.SetContent(mainview);
            appD.mainView = mainview;
            appD.menu = menuview;
            appD.NavController = this.NavigationController;
            SlideMenuController slideMenuController = new SlideMenuController(mainview, menuview, null);
            var widthMenu = (this.View.Frame.Width / 5) * 4;
            SlideMenuOptions.ContentViewScale = 1.0f;
            slideMenuController.LeftPanGesture = new UIPanGestureRecognizer();
            slideMenuController.ChangeLeftViewWidth(widthMenu);
            appD.SlideMenuController = slideMenuController;
            appD.NavController.PushViewController(slideMenuController, false);
        }
        #endregion

        #region events
        [Export("hideKeyboard")]
        private void hideKeyboard()
        {
            this.View.EndEditing(true);
        }

        private void BT_login_TouchUpInside(object sender, EventArgs e)
        {
            LoginViewController loginViewController = (LoginViewController)Storyboard.InstantiateViewController("LoginViewController");
            this.NavigationController.PushViewController(loginViewController, true);
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
                                AuthenticationOTPView authenticationOTPView = (AuthenticationOTPView)Storyboard.InstantiateViewController("AuthenticationOTPView");
                                authenticationOTPView.setContent(loginName, loginPass);
                                this.NavigationController.PushViewController(authenticationOTPView, true);
                            });
                        }
                        else
                        {
                            if (e.UserInfo.Status.HasValue && e.UserInfo.Status.Value == 1)
                            {
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

                                ProviderBase p_base = new ProviderBase();

                                if (!string.IsNullOrEmpty(CmmVariable.SysConfig.AvatarPath) && CmmVariable.SysConfig.AvatarPath != e.UserInfo.ImagePath)
                                {
                                    string url = CmmVariable.M_Domain + "/" + CmmVariable.SysConfig.AvatarPath.Split('?')[0];
                                    string localpath = Path.Combine(documentFolder, "avatar.jpg");
                                    p_base.DownloadFile(url, localpath, CmmVariable.M_AuthenticatedHttpClient);
                                }

                                CmmVariable.SysConfig.AvatarPath = e.UserInfo.ImagePath;
                                CmmFunction.WriteSetting();

                                await p_base.GetAllDynamicData_FirstLogin(true, CmmVariable.SysConfig.DataLimitDay, true);
                                //await Task.Run(() =>
                                //{
                                //  p_base.UpdateAllDynamicData(true, CmmVariable.SysConfig.DataLimitDay, false);
                                //});
                                InvokeOnMainThread(() =>
                                {
                                    RedirectToMainView();
                                });
                            }
                            else
                            {
                                InvokeOnMainThread(() =>
                                {
                                    //CmmIOSFunction.commonAlertMessage(this, "VNA BPM", "Tài khoản đã bị vô hiệu hóa.");
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
                        {
                            Console.WriteLine("Relogin failed");
                        }
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
            int max = loadingStep + 12; //12: số bước nhảy API

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
                    lbl_loading.Text = "Loading data... " + rnd.Next(min > 100 ? 90 : min, 100).ToString() + "%";
            });
            loadingStep = max;

            //appD.beanUpdateManagement[e.BeanName] = (NSString)e.ErrMess;
            //if (!appD.beanUpdateManagement.ContainsKey((NSString)e.BeanName))
            appD.beanUpdateManagement.SetValueForKey((NSString)e.ErrMess, (NSString)e.BeanName);
        }


        void CmmEvent_SyncDataBackgroundRequest(object sender, CmmEvent.SyncDataRequest e)
        {
            if (e.isDone)
            {
                CmmEvent.SyncDataBackgroundResultRequest -= CmmEvent_SyncDataBackgroundRequest;
            }
        }

        #endregion

        #region custom class
        public class LoginLoading : UIView
        {
            // control declarations
            UILabel loadingLabel;

            public LoginLoading(CGRect frame, string content) : base(frame)
            {
                CmmIOSFunction.EventUpdateLoading += setLoadingText;

                // configurable bits
                BackgroundColor = UIColor.White.ColorWithAlpha(0.7f);
                AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
                // create and configure the "Loading Data" label
                loadingLabel = new UILabel(new CGRect((frame.Width - 400) / 2, (frame.Height - 30) / 2, 400, 30));
                loadingLabel.TextColor = UIColor.FromRGB(0, 29, 35);
                loadingLabel.Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular);
                loadingLabel.Text = content;
                loadingLabel.TextAlignment = UITextAlignment.Center;
                loadingLabel.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;

                AddSubview(loadingLabel);
            }

            void setLoadingText(string text, string percent)
            {
                InvokeOnMainThread(delegate
                {
                    loadingLabel.Text = text;
                    //label_percent.Text = percent;
                });
            }
            public void Hide()
            {
                UIView.Animate(
                    0.5, // duration
                    () => { Alpha = 0; },
                    () => { RemoveFromSuperview(); }
                );
            }
        }
        #endregion
    }
}
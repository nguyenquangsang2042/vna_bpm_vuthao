using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobileV1.iOS.IOSClass;
using SlideMenuControllerXamarin;
using UIKit;
using ObjCRuntime;
using Foundation;
using System.Linq;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class LoginViewController : UIViewController
    {
        string loginName = string.Empty;
        string loginPass = string.Empty;
        bool is_fromLoginForm = false;
        private string documentFolder;   // Controller that activated the keyboard
        AppDelegate appD;
        NSObject app_ver; string serverConfigApp_version = "1.0.0";
        int loadingStep = 0;

        #region override
        public LoginViewController(IntPtr handle) : base(handle)
        {
            documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            appD = UIApplication.SharedApplication.Delegate as AppDelegate;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            app_ver = NSBundle.MainBundle.InfoDictionary["CFBundleVersion"];

            ConfigureController();
            RegistGesture();

            CmmEvent.ReloginRequest += CmmEvent_ReloginRequest;
            CmmEvent.SyncDataBackGroundRequest += CmmEvent_SyncDataBackGroundRequest;
            BT_back.TouchUpInside += BT_back_TouchUpInside;
            BT_login.TouchUpInside += BT_login_TouchUpInside;
            BT_register.TouchUpInside += BT_register_TouchUpInside;

            /*txt_loginName.ShouldEndEditing = (sender) =>
            {
                CheckUsername();
                return true;
            };*/
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        #endregion
        #region method
        private void ConfigureController()
        {
            BT_login.Layer.CornerRadius = 5;
            BT_login.ClipsToBounds = true;

            BT_register.ClipsToBounds = true;
            BT_register.Layer.CornerRadius = 5;
            BT_register.Layer.BorderColor = UIColor.FromRGB(219, 164, 16).CGColor;
            BT_register.Layer.BorderWidth = 0.5f;

            view_userNamePassword.Layer.CornerRadius = 5;
            view_userNamePassword.ClipsToBounds = true;

            view_userAcc.ClipsToBounds = true;
            view_userAcc.Layer.CornerRadius = 5;
            view_userAcc.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            view_userAcc.Layer.BorderWidth = 0.5f;

            view_password.ClipsToBounds = true;
            view_password.Layer.CornerRadius = 5;
            view_password.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            view_password.Layer.BorderWidth = 0.5f;

#if DEBUG
            //txt_password.Text = "vnabpm@123";
            //txt_loginName.Text = "fvnabpmadmin";
            if (string.Compare(CmmVariable.M_Domain, "https://vnabpm.vuthao.com") == 0)
            {
                txt_password.Text = "VTlamson123!@#";
                txt_loginName.Text = "tphong2@vuthao";
            }
            else if (string.Compare(CmmVariable.M_Domain, "https://bpm.vietnamairlines.com") == 0)
            {
                txt_password.Text = "vna@072022";
                txt_loginName.Text = "testtb";
            }
#endif
            var placeholderColor = UIColor.FromRGB(153, 153, 153);
            var placeholderStyle = UIFont.FromName("Arial-ItalicMT", 14f);

            string str_hintAccount = "Account name";
            var attHintAccount = new NSMutableAttributedString(str_hintAccount);
            attHintAccount.AddAttribute(UIStringAttributeKey.ForegroundColor, placeholderColor, new NSRange(0, str_hintAccount.Length));
            attHintAccount.AddAttribute(UIStringAttributeKey.Font, placeholderStyle, new NSRange(0, str_hintAccount.Length));
            txt_loginName.AttributedPlaceholder = attHintAccount;

            string str_hintPass = "Password";
            var attHintPass = new NSMutableAttributedString(str_hintPass);
            attHintPass.AddAttribute(UIStringAttributeKey.ForegroundColor, placeholderColor, new NSRange(0, str_hintPass.Length));
            attHintPass.AddAttribute(UIStringAttributeKey.Font, placeholderStyle, new NSRange(0, str_hintPass.Length));
            txt_password.AttributedPlaceholder = attHintPass;

            BT_back.Hidden = true;
        }

        private void RegistGesture()
        {
            UITapGestureRecognizer gestureRecognizer = new UITapGestureRecognizer(Self, new Selector("hideKeyboard"));
            gestureRecognizer.ShouldReceiveTouch += delegate (UIGestureRecognizer recognizer, UITouch touch)
            {
                return true;
            };
            View.AddGestureRecognizer(gestureRecognizer);
            this.View.AddGestureRecognizer(gestureRecognizer);
        }
        [Export("hideKeyboard")]
        private void hideKeyboard()
        {
            this.View.EndEditing(true);
        }
        #endregion

        #region event
        private void BT_back_TouchUpInside(object sender, EventArgs e)
        {
            this.NavigationController.PopViewController(true);
        }
        private async void BT_login_TouchUpInside(object sender, EventArgs e)
        {
            if (txt_loginName.IsFirstResponder)//if (txt_LoginName.IsEditing)
                txt_loginName.EndEditing(true);
            this.View.EndEditing(true);
            if ((string.IsNullOrEmpty(txt_loginName.Text) || (string.IsNullOrEmpty(txt_password.Text))))
            {
                CmmIOSFunction.commonAlertMessage(this, "VNA BPM", "Enter your business email.");
            }
            else
            {
                BT_back.Enabled = false;
                view_loading.Hidden = false;
                //CmmIOSFunction.IOSlog(null, "Step 1 - Login - Qua bước check user pass rỗng."); 
                loginName = txt_loginName.Text.TrimEnd();
                loginPass = txt_password.Text.TrimEnd();
                //CheckUsername();
                CmmIOSFunction.CheckDomain(loginName);
                view_login.Hidden = true;
                is_fromLoginForm = true;
                CmmVariable.SysConfig.DeviceInfo = CmmIOSFunction.collectDeviceInfo();
                //CmmEvent.ReloginRequest += CmmEvent_ReloginRequest;
                HttpClient clientLogin;
                await Task.Run(() =>
                {
                    string getCurrentUserUrl = CmmVariable.M_Domain.TrimEnd('/') + (new BeanUser().GetCurrentUserUrl());
                    //CmmIOSFunction.IOSlog(null, "Step 2 - Login - getCurrentUserUrl = " + getCurrentUserUrl);
                    CmmEvent.SyncDataBackgroundRequest_Performence(null, new CmmEvent.UpdateBackgroundEventArgs("Login", ""));

                    var userName = loginName.Contains("@vuthao") ? loginName.Split('@')[0] : loginName;
                    if ((clientLogin = CmmFunction.Login(getCurrentUserUrl, userName, loginPass, true, 1)) != null)
                    {

                    }
                    else
                    {
                        InvokeOnMainThread(() =>
                        {
                            view_loading.Hidden = true;
                            CmmEvent.ReloginRequest -= CmmEvent_ReloginRequest;
                            CmmIOSFunction.commonAlertMessage(this, "VNA BPM", "The email account you entered are incorrect or isn't linked with registed account. You can only sign in with an registed account via this app.");
                            view_login.Hidden = false;
                            BT_back.Enabled = true;
                        });
                    }
                });
            }
        }
        private void BT_register_TouchUpInside(object sender, EventArgs e)
        {
            RegisterViewController registerViewController = (RegisterViewController)Storyboard.InstantiateViewController("RegisterViewController");
            appD.NavController.PushViewController(registerViewController, true);
            if (appD.NavController != null)
            {
                var controllers = this.NavigationController.ViewControllers;
                List<UIViewController> newcontrollers = new List<UIViewController> { };

                foreach (var item in controllers)
                {
                    if (item.GetType() != typeof(LoginViewController))
                        newcontrollers.Add(item);
                }
                appD.NavController.ViewControllers = newcontrollers.ToArray();
            }
        }
        /*
        private async void CmmEvent_ReloginRequest(object sender, CmmEvent.LoginEventArgs e)
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
                        CmmVariable.SysConfig.LangCode = e.UserInfo.Language.ToString();

                        //Cập nhật ver
                        CmmVariable.SysConfig.AppConfigVersion = app_ver.ToString();

                        if (is_fromLoginForm)
                        {
                            CmmVariable.SysConfig.AvatarPath = e.UserInfo.ImagePath;
                            CmmFunction.WriteSetting();

                            ProviderBase p_base = new ProviderBase();
                            ProviderUser p_user = new ProviderUser();

                            await Task.Run(() =>
                            {
                                if (!string.IsNullOrEmpty(CmmVariable.SysConfig.AvatarPath))
                                {
                                    string url = CmmVariable.M_Domain + "/" + CmmVariable.SysConfig.AvatarPath.Split('?')[0];
                                    string localpath = Path.Combine(documentFolder, "avatar.jpg");
                                    p_base.DownloadFile(url, localpath, CmmVariable.M_AuthenticatedHttpClient);
                                }

                                p_user.UpdateLangData(CmmVariable.SysConfig.LangCode, false, true);
                                p_base.UpdateAllDynamicData(false, CmmVariable.SysConfig.DataLimitDay, true);

                                InvokeOnMainThread(() =>
                                {
                                    MainView mainview = (MainView)Storyboard.InstantiateViewController("MainView");
                                    MenuView menuview = (MenuView)Storyboard.InstantiateViewController("MenuView");
                                    //RootNavigation rootNavigation = (RootNavigation)Storyboard.InstantiateViewController("RootNavigation");
                                    menuview.SetContent(mainview);
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
                                });
                            });
                            LoadData(e);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(CmmVariable.SysConfig.AvatarPath) && CmmVariable.SysConfig.AvatarPath != e.UserInfo.ImagePath)
                            {
                                ProviderBase p_base = new ProviderBase();
                                string url = CmmVariable.M_Domain + "/" + CmmVariable.SysConfig.AvatarPath.Split('?')[0];
                                string localpath = Path.Combine(documentFolder, "avatar.jpg");
                                p_base.DownloadFile(url, localpath, CmmVariable.M_AuthenticatedHttpClient);
                            }

                            CmmVariable.SysConfig.AvatarPath = e.UserInfo.ImagePath;
                            CmmFunction.WriteSetting();

                            InvokeOnMainThread(() =>
                            {
                                MainView mainview = (MainView)Storyboard.InstantiateViewController("MainView");
                                mainview.setContent(true);
                                MenuView menuview = (MenuView)Storyboard.InstantiateViewController("MenuView");
                                menuview.SetContent(mainview);
                                appD.mainView = mainview;
                                appD.menu = menuview;
                                //appD.NavController = this.NavigationController;
                                SlideMenuController slideMenuController = new SlideMenuController(mainview, menuview, null);
                                var widthMenu = (this.View.Frame.Width / 5) * 4;
                                SlideMenuOptions.ContentViewScale = 1.0f;
                                slideMenuController.LeftPanGesture = new UIPanGestureRecognizer();
                                slideMenuController.ChangeLeftViewWidth(widthMenu);
                                appD.SlideMenuController = slideMenuController;
                                appD.NavController.PushViewController(slideMenuController, false);
                            });
                        }
                    }
                }
                else
                {
                    InvokeOnMainThread(() =>
                    {

                    });
                }

            }
            CmmEvent.ReloginRequest -= CmmEvent_ReloginRequest;
        }
        */

        private void CmmEvent_ReloginRequest(object sender, CmmEvent.LoginEventArgs e)
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
                            //cap nhat app version
                            CmmVariable.SysConfig.AppConfigVersion = app_ver.ToString();

                            if (is_fromLoginForm)
                            {
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
                                        iTunesLink = new NSString(@"itms://itunes.apple.com/us/app/vna-bpm/id1619953695?mt=8");
                                        UIApplication.SharedApplication.OpenUrl(NSUrl.FromString(iTunesLink));
                                        System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow(); // terminate app
                                    }));

                                    this.PresentViewController(alert, true, null);
                                }
                                else
                                {
                                    LoadData(e);
                                }
                            }
                        }
                        else
                        {
                            InvokeOnMainThread(() =>
                            {
                                view_loading.Hidden = true;
                                view_login.Hidden = false;
                                BT_back.Enabled = true;
                                CmmIOSFunction.commonAlertMessage(this, "VNA BPM", "Tài khoản đã bị vô hiệu hóa");
                            });
                        }
                    }
                }
                else
                {
                    InvokeOnMainThread(() =>
                    {

                    });
                }

            }
            CmmEvent.ReloginRequest -= CmmEvent_ReloginRequest;
        }

        /*async void LoadData(CmmEvent.LoginEventArgs e)
        {
            CmmVariable.SysConfig.AvatarPath = e.UserInfo.ImagePath;
            CmmFunction.WriteSetting();

            ProviderBase p_base = new ProviderBase();
            ProviderUser p_user = new ProviderUser();

            _ = Task.Run(() =>
            {
                p_base.UpdateSyncBackgroundData(false, CmmVariable.SysConfig.DataLimitDay, true);
            });

            _ = Task.Run(() =>
            {
                p_base.UpdateSyncBackgroundData1(false, CmmVariable.SysConfig.DataLimitDay, true);
            });

            _ = Task.Run(() =>
            {
                p_base.UpdateSyncBackgroundData2(false, CmmVariable.SysConfig.DataLimitDay, true);
            });

            _ = Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(CmmVariable.SysConfig.AvatarPath))
                {
                    string url = CmmVariable.M_Domain + "/" + CmmVariable.SysConfig.AvatarPath.Split('?')[0];
                    string localpath = Path.Combine(documentFolder, "avatar.jpg");
                    p_base.DownloadFile(url, localpath, CmmVariable.M_AuthenticatedHttpClient);
                }
            });

            _ = Task.Run(() =>
            {
                p_user.UpdateLangData(CmmVariable.SysConfig.LangCode, false, true);
            });

            await Task.Run(() =>
            {
                p_base.UpdateSyncBackgroundData3(false, CmmVariable.SysConfig.DataLimitDay, true);

                while (loadingStep.Equals(3))
                {
                    InvokeOnMainThread(() =>
                    {
                        MainView mainview = (MainView)Storyboard.InstantiateViewController("MainView");
                        MenuView menuview = (MenuView)Storyboard.InstantiateViewController("MenuView");
                        //RootNavigation rootNavigation = (RootNavigation)Storyboard.InstantiateViewController("RootNavigation");
                        menuview.SetContent(mainview);
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
                    });

                    Console.WriteLine("Navigate to MainView - " + loadingStep);
                    loadingStep = 0;
                    break;
                }
            });
        }*/

        async void LoadData(CmmEvent.LoginEventArgs e)
        {
            CmmVariable.SysConfig.AvatarPath = e.UserInfo.ImagePath;
            CmmFunction.WriteSetting();

            ProviderBase p_base = new ProviderBase();
            ProviderUser p_user = new ProviderUser();

            _ = Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(CmmVariable.SysConfig.AvatarPath))
                {
                    string url = CmmVariable.M_Domain + "/" + CmmVariable.SysConfig.AvatarPath.Split('?')[0];
                    string localpath = Path.Combine(documentFolder, "avatar.jpg");
                    p_base.DownloadFile(url, localpath, CmmVariable.M_AuthenticatedHttpClient);
                }
            });

            _ = Task.Run(() =>
            {
                p_user.UpdateLangData(CmmVariable.SysConfig.LangCode, false, true);
            });
            //p_base.GetAllDynamicData_FirstLoginSync(false, CmmVariable.SysConfig.DataLimitDay, true);
            await p_base.GetAllDynamicData_FirstLogin(false, CmmVariable.SysConfig.DataLimitDay, true);
            //p_base.UpdateAllDynamicData(false, CmmVariable.SysConfig.DataLimitDay, true);

            InvokeOnMainThread(() =>
            {
                CmmEvent.SyncDataBackGroundRequest -= CmmEvent_SyncDataBackGroundRequest;
                MainView mainview = (MainView)Storyboard.InstantiateViewController("MainView");
                mainview.setContent(false);
                MenuView menuview = (MenuView)Storyboard.InstantiateViewController("MenuView");
                menuview.SetContent(mainview);
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
            });
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
                    //lbl_loading.Text = "Loading data... " + rnd.Next(min, max).ToString() + "%";
                    lbl_loading.Text = "Loading data... " + rnd.Next(min > 100 ? 90 : min, 100).ToString() + "%";
            });
            loadingStep = max;

            //if (!appD.beanUpdateManagement.ContainsKey((NSString)e.BeanName))
            appD.beanUpdateManagement.SetValueForKey((NSString)e.ErrMess, (NSString)e.BeanName);
        }

        /*
        void CheckDomain()
        {
            CmmVariable.M_Domain = !string.IsNullOrEmpty(txt_loginName.Text.TrimEnd()) && txt_loginName.Text.TrimEnd().Contains("@vuthao") ? CmmVariable.M_Domain_develop : CmmVariable.M_Domain_active;
        }
        private void CheckUsername()
        {
            var _loginName = txt_loginName.Text.TrimEnd();

            if (!string.IsNullOrEmpty(_loginName)
                && ((!string.IsNullOrEmpty(loginName) && string.Compare(loginName, _loginName) != 0) || string.IsNullOrEmpty(loginName)))
            {
                loginName = _loginName.Contains("@vuthao") ? _loginName.Split("@vuthao")[0] : _loginName;//.Split('@')[0];
            }
            
            if (txt_loginName.Text.Contains("@vuthao") && txt_loginName.Text.TrimEnd().Split("@vuthao").Length > 0)
            {
                txt_loginName.Text = txt_loginName.Text.Split("@vuthao")[0];
            }
        }*/
        #endregion 
    }
}
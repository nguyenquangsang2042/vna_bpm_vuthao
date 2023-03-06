using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BPMOPMobile.Class;
using BPMOPMobile.iPad.IOSClass;
using Foundation;
using UIKit;
using ObjCRuntime;
using BPMOPMobile.DataProvider;
using SlideMenuControllerXamarin;
using System.Net.Http;
using BPMOPMobile.Bean;
using CoreGraphics;

namespace BPMOPMobile.iPad.ViewControllers
{
    public partial class LoginViewController : UIViewController
    {
        AppDelegate appD;
        string loginName = string.Empty;
        string loginPass = string.Empty;
        private string documentFolder;
        private string _pathToDatabase;
        UITapGestureRecognizer gestureRecognizer_keyboard;
        bool isShowKeyboard;
        int loadingStep = 0;

        #region override
        public LoginViewController(IntPtr handle) : base(handle)
        {
            appD = UIApplication.SharedApplication.Delegate as AppDelegate;
            documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            _pathToDatabase = Path.Combine(Path.Combine(documentFolder, CmmVariable.M_DataPath));
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            gestureRecognizer_keyboard = new UITapGestureRecognizer(Self, new Selector("keyboardPopup"));
            this.View.AddGestureRecognizer(gestureRecognizer_keyboard);

            // show, hide Keyboard
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.DidShowNotification, login_KeyBoardUpNotification);
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, login_KeyBoardDownNotification);

            RegistGesture();
            Viewconfiguration();

            CmmEvent.SyncDataBackGroundRequest += CmmEvent_SyncDataBackGroundRequest;
            BT_register.TouchUpInside += BT_register_TouchUpInside;
            BT_login.TouchDown += BT_login_TouchDown;
            tf_account.ShouldReturn = TF_account_ShouldReturn;
            //tf_account.EditingChanged += Tf_account_EditingChanged;
            tf_pass.ShouldReturn = TF_pass_ShouldReturn;
            BT_loginWithDifferrentAccount.TouchUpInside += BT_changeAccount_TouchUpInside;
            BT_back.TouchUpInside += BT_back_TouchUpInside;

            BT_resetPin.TouchUpInside += BT_resetPin_TouchUpInside;
            BT_1.TouchUpInside += delegate { calculatorPin(BT_1.TitleLabel.Text); };
            BT_2.TouchUpInside += delegate { calculatorPin(BT_2.TitleLabel.Text); };
            BT_3.TouchUpInside += delegate { calculatorPin(BT_3.TitleLabel.Text); };
            BT_4.TouchUpInside += delegate { calculatorPin(BT_4.TitleLabel.Text); };
            BT_5.TouchUpInside += delegate { calculatorPin(BT_5.TitleLabel.Text); };
            BT_6.TouchUpInside += delegate { calculatorPin(BT_6.TitleLabel.Text); };
            BT_7.TouchUpInside += delegate { calculatorPin(BT_7.TitleLabel.Text); };
            BT_8.TouchUpInside += delegate { calculatorPin(BT_8.TitleLabel.Text); };
            BT_9.TouchUpInside += delegate { calculatorPin(BT_9.TitleLabel.Text); };
            BT_0.TouchUpInside += delegate { calculatorPin(BT_0.TitleLabel.Text); };
        }

        private void BT_back_TouchUpInside(object sender, EventArgs e)
        {
            this.NavigationController.PopViewController(true);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        #endregion

        #region method
        private void Viewconfiguration()
        {
#if DEBUG
            //tf_pass.Text = "vnabpm@123";
            //tf_account.Text = "fvnabpmadmin";

            if (string.Compare(CmmVariable.M_Domain, CmmVariable.M_Domain_develop) == 0)
            {
                tf_pass.Text = "VTlamson123!@#";
                tf_account.Text = "tphong2@vuthao";
            }
            else if (string.Compare(CmmVariable.M_Domain, CmmVariable.M_Domain_active) == 0)
            {
                tf_pass.Text = "vna@072022";
                tf_account.Text = "testtb";
            }
#endif

            BT_register.ClipsToBounds = true;
            BT_register.Layer.CornerRadius = 5;
            BT_register.Layer.BorderColor = UIColor.FromRGB(219, 164, 16).CGColor;
            BT_register.Layer.BorderWidth = 1f;

            BT_login.Layer.BorderColor = UIColor.FromRGB(219, 164, 16).CGColor;
            BT_login.Layer.BorderWidth = 1f;
            BT_login.Layer.CornerRadius = 5;
            BT_login.Layer.MasksToBounds = true;

            view_userAcc.ClipsToBounds = true;
            view_userAcc.Layer.CornerRadius = 5;
            view_userAcc.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            view_userAcc.Layer.BorderWidth = 1f;

            view_password.ClipsToBounds = true;
            view_password.Layer.CornerRadius = 5;
            view_password.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            view_password.Layer.BorderWidth = 1f;

            var placeholderColor = UIColor.FromRGB(153, 153, 153);
            var placeholderStyle = UIFont.FromName("Arial-ItalicMT", 16f);

            string str_hintAccount = "Account name";
            var attHintAccount = new NSMutableAttributedString(str_hintAccount);
            attHintAccount.AddAttribute(UIStringAttributeKey.ForegroundColor, placeholderColor, new NSRange(0, str_hintAccount.Length));
            attHintAccount.AddAttribute(UIStringAttributeKey.Font, placeholderStyle, new NSRange(0, str_hintAccount.Length));
            tf_account.AttributedPlaceholder = attHintAccount;

            string str_hintPass = "Password";
            var attHintPass = new NSMutableAttributedString(str_hintPass);
            attHintPass.AddAttribute(UIStringAttributeKey.ForegroundColor, placeholderColor, new NSRange(0, str_hintPass.Length));
            attHintPass.AddAttribute(UIStringAttributeKey.Font, placeholderStyle, new NSRange(0, str_hintPass.Length));
            tf_pass.AttributedPlaceholder = attHintPass;

            BT_back.Hidden = true; //Rule từ ngày 19/05: không auto login đc thì tới thẳng view login
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
                            TranslationToOTPView(true);
                        });
                    }
                    else
                    {
                        if (e.UserInfo.Status.HasValue && e.UserInfo.Status.Value == 1)
                        {
                            if (!string.IsNullOrEmpty(e.VerifyOTP))
                                CmmVariable.SysConfig.VerifyOTP = e.VerifyOTP;

                            CmmVariable.SysConfig.AvatarPath = e.UserInfo.ImagePath;
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
                            CmmVariable.SysConfig.LangCode = e.UserInfo.Language == 1033 || e.UserInfo.Language == 1066 ? e.UserInfo.Language.ToString() : "1066"; //CmmVariable.SysConfig.LangCode = e.UserInfo.Language.ToString();
                            CmmVariable.SysConfig.AccountName = e.UserInfo.AccountName;
                            CmmFunction.WriteSetting();

                            LoadData(e);
                        }
                        else
                        {
                            InvokeOnMainThread(() =>
                            {
                                BT_back.Enabled = true;
                                view_loading.Hidden = true;
                                LoginView.Hidden = false;
                                loading_indicator.StopAnimating();
                                CmmIOSFunction.commonAlertMessage(this, "VNA BPM", "Tài khoản đã bị vô hiệu hóa");
                            });
                        }
                    }
                }
            }
            CmmEvent.ReloginRequest -= CmmEvent_ReloginRequest;
        }
        private bool TF_pass_ShouldReturn(UITextField textField)
        {
            View.EndEditing(true);
            return true;
        }
        private void TranslationToOTPView(bool is_show)
        {
            if (is_show)
            {
                //login_form.Frame = new CGRect(login_form.Frame.X, login_form.Frame.Y, login_form.Frame.Width, login_form.Frame.Height);
                //view_twofactor.Frame = new CGRect()
                UIView.BeginAnimations("show_animationShow_OTPView");
                UIView.SetAnimationDuration(0.2f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);
                LoginView.Hidden = true;
                view_twofactor.Hidden = false;
                view_loading.Hidden = true;
                UIView.CommitAnimations();
            }
            else
            {
                //login_form.Frame = new CGRect(login_form.Frame.X, login_form.Frame.Y, login_form.Frame.Width, login_form.Frame.Height);
                //view_twofactor.Frame = new CGRect()
                UIView.BeginAnimations("show_animationHide_OTPView");
                UIView.SetAnimationDuration(0.2f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);
                LoginView.Hidden = false;
                view_twofactor.Hidden = true;
                view_loading.Hidden = true;
                UIView.CommitAnimations();
            }
        }
        private void BT_changeAccount_TouchUpInside(object sender, EventArgs e)
        {
            TranslationToOTPView(false);
        }
        /*private void Tf_account_EditingChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tf_account.Text))
            {
                tf_account.Font = UIFont.FromName("ArialMT", 16f);
                tf_account.TextColor = UIColor.Black;
            }
            else
            {
                tf_account.Font = UIFont.FromName("Arial-ItalicMT", 16f);
                tf_account.TextColor = UIColor.LightGray;
            }
        }*/
        private bool TF_account_ShouldReturn(UITextField textField)
        {
            tf_pass.BecomeFirstResponder();
            return true;
        }
        private async void calculatorPin(string param)
        {
            try
            {
                if (string.IsNullOrEmpty(tf_1.Text))
                    tf_1.Text = param;
                else if (string.IsNullOrEmpty(tf_2.Text))
                    tf_2.Text = param;
                else if (string.IsNullOrEmpty(tf_3.Text))
                    tf_3.Text = param;
                else if (string.IsNullOrEmpty(tf_4.Text))
                    tf_4.Text = param;
                else if (string.IsNullOrEmpty(tf_5.Text))
                    tf_5.Text = param;
                else
                {
                    tf_6.Text = param;

                    if (!string.IsNullOrEmpty(tf_1.Text) && !string.IsNullOrEmpty(tf_2.Text) && !string.IsNullOrEmpty(tf_3.Text) && !string.IsNullOrEmpty(tf_4.Text) && !string.IsNullOrEmpty(tf_5.Text) && !string.IsNullOrEmpty(tf_6.Text))
                    {
                        string pincode = tf_1.Text + tf_2.Text + tf_3.Text + tf_4.Text + tf_5.Text + tf_6.Text;
                        view_loading.Hidden = false;
                        loading_indicator.StartAnimating();
                        LoginView.Hidden = true;
                        view_twofactor.Hidden = true;
                        BT_back.Enabled = false;

                        await Task.Run(() =>
                        {
                            CmmEvent.ReloginRequest += CmmEvent_ReloginRequest;
                            ProviderBase p_base = new ProviderBase();
                            HttpClient clientLogin = CmmVariable.M_AuthenticatedHttpClient;
                            string getCurrentUserUrl = CmmVariable.M_Domain.TrimEnd('/') + (new BeanUser().GetCurrentUserUrl());
                            //CmmIOSFunction.IOSlog(null, "Step 2 - Login - getCurrentUserUrl = " + getCurrentUserUrl);
                            if ((clientLogin = CmmFunction.Login(getCurrentUserUrl, loginName, loginPass, true, 1, pincode)) != null)
                            {
                                InvokeOnMainThread(() =>
                                {
                                    CmmVariable.M_AuthenticatedHttpClient = clientLogin;
                                });
                            }
                            else
                            {
                                InvokeOnMainThread(() =>
                                {
                                    BT_back.Enabled = true;
                                    view_twofactor.Hidden = false;
                                    view_loading.Hidden = true;
                                    loading_indicator.StopAnimating();
                                    CmmEvent.ReloginRequest -= CmmEvent_ReloginRequest;
                                    CmmIOSFunction.commonAlertMessage(this, "VNA BPM", CmmFunction.GetTitle("TEXT_LOGINFAIL", "Login information is incorrect, please try again!"));//"Login information is incorrect, please try again!"
                                    resetPin();
                                });
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                BT_back.Enabled = true;
                view_loading.Hidden = true;
                loading_indicator.StopAnimating();
                view_twofactor.Hidden = false;

                resetPin();
                CmmIOSFunction.commonAlertMessage(this, "VNA BPM", CmmFunction.GetTitle("TEXT_LOGINFAIL", "Login information is incorrect, please try again!"));
#if DEBUG
                Console.WriteLine("AutneticationOTP - calculatorPin - Err: " + ex.ToString());
#endif
            }
        }

        public void resetPin()
        {
            tf_1.Text = string.Empty;
            tf_2.Text = string.Empty;
            tf_3.Text = string.Empty;
            tf_4.Text = string.Empty;
            tf_5.Text = string.Empty;
            tf_6.Text = string.Empty;
        }
        private async void BT_login_TouchDown(object sender, EventArgs e)
        {
            view_loading.Hidden = false;
            loading_indicator.StartAnimating();
            LoginView.Hidden = true;
            BT_back.Enabled = false;
            if (tf_account.IsFirstResponder)
                tf_account.EndEditing(true);

            if (string.IsNullOrEmpty(tf_account.Text) || string.IsNullOrEmpty(tf_pass.Text))
            {
                CmmIOSFunction.commonAlertMessage(this, "VNA BPM", CmmFunction.GetTitle("TEXT_LOGINFAIL", "Login information is incorrect, please try again."));

                BT_back.Enabled = true;
                view_loading.Hidden = true;
                LoginView.Hidden = false;
                loading_indicator.StopAnimating();
            }
            else
            {
                loginName = tf_account.Text.TrimEnd();
                loginPass = tf_pass.Text.TrimEnd();
                this.View.EndEditing(true);
                //CheckUsername();
                CmmIOSFunction.CheckDomain(loginName);
                CmmVariable.SysConfig.DeviceInfo = CmmIOSFunction.collectDeviceInfo();
                CmmEvent.ReloginRequest += CmmEvent_ReloginRequest;
                HttpClient clientLogin;

                await Task.Run(() =>
                {
                    string getCurrentUserUrl = CmmVariable.M_Domain.TrimEnd('/') + (new BeanUser().GetCurrentUserUrl());
                    var userName = loginName.Contains("@vuthao") ? loginName.Split('@')[0] : loginName;
                    if ((clientLogin = CmmFunction.Login(getCurrentUserUrl, userName, loginPass, true, 1)) != null)
                    {
                        CmmVariable.M_AuthenticatedHttpClient = clientLogin;

                        ProviderBase p_base = new ProviderBase();
                        if (!string.IsNullOrEmpty(CmmVariable.SysConfig.AvatarPath))
                        {
                            string url = CmmVariable.M_Domain + "/" + CmmVariable.SysConfig.AvatarPath.Split('?')[0];
                            string localpath = Path.Combine(documentFolder, "avatar.jpg");
                            p_base.DownloadFile(url, localpath, CmmVariable.M_AuthenticatedHttpClient);
                        }

                        InvokeOnMainThread(() =>
                        {
                            LoginView.Hidden = true;
                            view_loading.Hidden = false;
                        });
                    }
                    else
                    {
                        InvokeOnMainThread(() =>
                        {
                            CmmEvent.ReloginRequest -= CmmEvent_ReloginRequest;
                            CmmIOSFunction.commonAlertMessage(this, "VNA BPM", CmmFunction.GetTitle("TEXT_LOGINFAIL", "Login information is incorrect, please try again."));

                            BT_back.Enabled = true;
                            view_loading.Hidden = true;
                            LoginView.Hidden = false;
                            loading_indicator.StopAnimating();
                        });
                    }
                });
            }
        }
        private void BT_resetPin_TouchUpInside(object sender, EventArgs e)
        {
            resetPin();
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
        async void LoadData(CmmEvent.LoginEventArgs e)
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
                        var widthMenu = (this.View.Frame.Width / 3.5f) + 20;//var widthMenu = (this.View.Frame.Width / 5) * 4;
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
        }
        */

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


            _ = Task.Run(() =>
            {
                p_base.UpdateMasterData<BeanUser>(null, true, CmmVariable.SysConfig.DataLimitDay, false);
            });

            await p_base.GetAllDynamicData_FirstLogin(false, CmmVariable.SysConfig.DataLimitDay, true);
            //p_base.UpdateAllDynamicData(false, CmmVariable.SysConfig.DataLimitDay, true);
            InvokeOnMainThread(() =>
            {
                CmmEvent.SyncDataBackGroundRequest -= CmmEvent_SyncDataBackGroundRequest;
                MainView mainview = (MainView)Storyboard.InstantiateViewController("MainView");
                MenuView menuview = (MenuView)Storyboard.InstantiateViewController("MenuView");
                menuview.SetContent(mainview);
                appD.mainView = mainview;
                appD.menu = menuview;
                appD.NavController = this.NavigationController;
                SlideMenuController slideMenuController = new SlideMenuController(mainview, menuview, null);
                //var widthMenu = (this.View.Frame.Width / 5) * 4;
                var widthMenu = (this.View.Frame.Width / 3.5f) + 20;
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

        #region keyboard popup/hide

        private void login_KeyBoardUpNotification(NSNotification notification)
        {
            try
            {
                if (!isShowKeyboard)
                {
                    isShowKeyboard = true;

                    if (View.Frame.Y == 0)
                    {
                        CGRect keyboardSize = UIKeyboard.BoundsFromNotification(notification);
                        CGRect custFrame = View.Frame;
                        custFrame.Y -= keyboardSize.Height / 3;
                        View.Frame = custFrame;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("LoginViewController - Err: " + ex.ToString());
            }
        }

        private void login_KeyBoardDownNotification(NSNotification notification)
        {
            try
            {
                if (isShowKeyboard)
                {
                    isShowKeyboard = false;

                    if (View.Frame.Y != 0)
                    {
                        CGRect custFrame = View.Frame;
                        custFrame.Y = 0;
                        View.Frame = custFrame;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("LoginviewController - Err: " + ex.ToString());
            }
        }

        #endregion
    }
}
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobileV1.iOS.IOSClass;
using CoreGraphics;
using Foundation;
using SlideMenuControllerXamarin;
using UIKit;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class AuthenticationOTPView : UIViewController
    {
        CmmLoading loading;
        AppDelegate appD;
        private string _pathToDatabase;
        private string documentFolder;   // Controller that activated the keyboard
        string loginName, loginPass;

        public AuthenticationOTPView(IntPtr handle) : base(handle)
        {
            documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            _pathToDatabase = Path.Combine(Path.Combine(documentFolder, CmmVariable.M_DataPath));
            appD = UIApplication.SharedApplication.Delegate as AppDelegate;
        }

        #region override

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            #region delegate
            BT_changeAccount.TouchUpInside += BT_changeAccount_TouchUpInside;
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
            #endregion
        }


        #endregion

        #region private - public method
        public void setContent(string _userName, string _password)
        {
            loginName = _userName;
            loginPass = _password;
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
                        loading = new CmmLoading(new CGRect((this.View.Bounds.Width - 200) / 2, (this.View.Bounds.Height - 200) / 2, 200, 200), "Loading...");
                        this.View.Add(loading);

                        await Task.Run(() =>
                        {
                            CmmEvent.ReloginRequest += CmmEvent_ReloginRequest;
                            ProviderBase p_base = new ProviderBase();
                            HttpClient clientLogin = CmmVariable.M_AuthenticatedHttpClient;
                            string getCurrentUserUrl = CmmVariable.M_Domain.TrimEnd('/') + (new BeanUser().GetCurrentUserUrl());
                            //CmmIOSFunction.IOSlog(null, "Step 2 - Login - getCurrentUserUrl = " + getCurrentUserUrl);
                            if ((clientLogin = CmmFunction.Login(getCurrentUserUrl, loginName, loginPass, true, 1, pincode)) != null)
                            {
                                CmmVariable.M_AuthenticatedHttpClient = clientLogin;
                                //if (!string.IsNullOrEmpty(CmmVariable.SysConfig.AvatarPath))
                                //{
                                //    string url = CmmVariable.M_Domain + "/" + CmmVariable.SysConfig.AvatarPath.Split('?')[0];
                                //    string localpath = Path.Combine(documentFolder, "avatar.jpg");
                                //    p_base.DownloadFile(url, localpath, CmmVariable.M_AuthenticatedHttpClient);
                                //}

                                InvokeOnMainThread(() =>
                                {
                                    CmmVariable.M_AuthenticatedHttpClient = clientLogin;
                                });

                            }
                            else
                            {
                                InvokeOnMainThread(() =>
                                {
                                    loading.Hide();
                                    CmmEvent.ReloginRequest -= CmmEvent_ReloginRequest;
                                    CmmIOSFunction.commonAlertMessage(this, "BPM", "Login information is incorrect, please try again!");
                                    resetPin();
                                });
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                loading.Hide();
                resetPin();
                CmmIOSFunction.commonAlertMessage(this, "BPM", "Login action failed. Please try again!");
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
        #endregion

        #region events
        private void BT_changeAccount_TouchUpInside(object sender, EventArgs e)
        {
            if (this.NavigationController != null)
            {

                try
                {
                    NSUserDefaults.StandardUserDefaults.SetString("", "ActiveUser");
                    //CmmEvent.SyncDataRequest -= CmmEvent_SyncDataRequest;

                    //if (timerResync != null)
                    //{
                    //    timerResync.timerReSync.Stop();
                    //}

                    //var dataFile = CmmVariable.M_DataPath;
                    //if (File.Exists(dataFile))
                    //    File.Delete(dataFile);

                    //string dataFile_shm = CmmVariable.M_DataPath + "-shm";
                    //if (File.Exists(dataFile_shm))
                    //    File.Delete(dataFile_shm);

                    //string dataFile_wal = CmmVariable.M_DataPath + "-wal";
                    //if (File.Exists(dataFile_wal))
                    //    File.Delete(dataFile_wal);

                    //var configFile = CmmVariable.M_settingFileName;
                    //if (File.Exists(configFile))
                    //    File.Delete(configFile);

                    //CmmVariable.M_DataPath = "DB_sqlite_XamDocument.db3";
                    //CmmVariable.M_settingFileName = "config.ini";
                    //CmmVariable.M_AuthenticatedHttpClient = null;
                    //CmmVariable.SysConfig = null;
                    //CmmFunction.WriteSetting();
                    DeleteFileToChangeAccount();
                    UIApplication.SharedApplication.CancelAllLocalNotifications();
                    AppDelegate appD = UIApplication.SharedApplication.Delegate as AppDelegate;

                    var navigationController = Storyboard.InstantiateViewController("RootNavigation") as RootNavigation;
                    //appD.Window.RootViewController = navigationController;
                    foreach (var view in UIApplication.SharedApplication.Windows)
                    {
                        if (view.RootViewController.GetType().Name == "RootNavigation")
                        {
                            view.RootViewController = navigationController;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("MenuView - SignOut - Err: " + ex.ToString());
                }

            }

        }

        private void DeleteFileToChangeAccount()
        {
            var dataFile = CmmVariable.M_DataPath;
            if (File.Exists(dataFile))
                File.Delete(dataFile);

            string dataFile_shm = CmmVariable.M_DataPath + "-shm";
            if (File.Exists(dataFile_shm))
                File.Delete(dataFile_shm);

            string dataFile_wal = CmmVariable.M_DataPath + "-wal";
            if (File.Exists(dataFile_wal))
                File.Delete(dataFile_wal);

            var configFile = CmmVariable.M_settingFileName;
            if (File.Exists(configFile))
                File.Delete(configFile);

            CmmVariable.M_DataPath = "DB_sqlite_XamDocument.db3";
            CmmVariable.M_settingFileName = "config.ini";
            CmmVariable.M_AuthenticatedHttpClient = null;
            CmmVariable.SysConfig = null;
            CmmFunction.WriteSetting();
        }
        private void BT_resetPin_TouchUpInside(object sender, EventArgs e)
        {
            resetPin();
        }

        #endregion

        #region events
        private async void CmmEvent_ReloginRequest(object sender, CmmEvent.LoginEventArgs e)
        {
            if (e != null)
            {
                if (e.IsSuccess)
                {
                    if (e.ErrCode == "OTP")
                    {
                        AuthenticationOTPView authenticationOTPView = (AuthenticationOTPView)Storyboard.InstantiateViewController("AuthenticationOTPView");
                        this.NavigationController.PushViewController(authenticationOTPView, true);
                    }
                    else
                    {
                        ProviderUser p_user = new ProviderUser();
                        //string currentLangCode = p_user.GetCurrentlangCode();

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
                        //CmmVariable.SysConfig.LangCode = currentLangCode;
                        CmmFunction.WriteSetting();


                        ProviderBase p_base = new ProviderBase();
                        await Task.Run(() =>
                        {
                            if (!string.IsNullOrEmpty(CmmVariable.SysConfig.AvatarPath))
                            {
                                string url = CmmVariable.M_Domain + "/" + CmmVariable.SysConfig.AvatarPath.Split('?')[0];
                                string localpath = Path.Combine(documentFolder, "avatar.jpg");
                                p_base.DownloadFile(url, localpath, CmmVariable.M_AuthenticatedHttpClient);
                            }

                            p_base.UpdateAllDynamicData(false, CmmVariable.SysConfig.DataLimitDay, true);

                            InvokeOnMainThread(() =>
                            {
                                loading.Hide();
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


                    }

                }

            }
            CmmEvent.ReloginRequest -= CmmEvent_ReloginRequest;
        }

        #endregion
    }
}


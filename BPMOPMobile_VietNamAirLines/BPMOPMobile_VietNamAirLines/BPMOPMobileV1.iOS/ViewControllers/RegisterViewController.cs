using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BPMOPMobileV1.iOS.IOSClass;
using Foundation;
using UIKit;
using ObjCRuntime;
using BPMOPMobile.Class;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class RegisterViewController : UIViewController
    {
        AppDelegate appD;
        #region override
        public RegisterViewController(IntPtr handle) : base(handle)
        {
            appD = UIApplication.SharedApplication.Delegate as AppDelegate;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            RegistGesture();
            ConfigureController();

            BT_back.TouchUpInside += BT_back_TouchUpInside;
            BT_login.TouchUpInside += BT_login_TouchUpInside;
            BT_register.TouchUpInside += BT_register_TouchUpInside;
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
            BT_login.Layer.BorderColor = UIColor.FromRGB(219, 164, 16).CGColor;
            BT_login.Layer.BorderWidth = 0.5f;
            BT_login.Layer.CornerRadius = 5;
            BT_login.ClipsToBounds = true;

            BT_register.Layer.CornerRadius = 5;
            BT_register.ClipsToBounds = true;

            view_userAcc.ClipsToBounds = true;
            view_userAcc.Layer.CornerRadius = 5;
            view_userAcc.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            view_userAcc.Layer.BorderWidth = 0.5f;

            var placeholderColor = UIColor.FromRGB(153, 153, 153);
            var placeholderStyle = UIFont.FromName("Arial-ItalicMT", 14f);

            string str_hintAccount = "Email";
            var attHintAccount = new NSMutableAttributedString(str_hintAccount);
            attHintAccount.AddAttribute(UIStringAttributeKey.ForegroundColor, placeholderColor, new NSRange(0, str_hintAccount.Length));
            attHintAccount.AddAttribute(UIStringAttributeKey.Font, placeholderStyle, new NSRange(0, str_hintAccount.Length));
            txt_userName.AttributedPlaceholder = attHintAccount;

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

        #region override
        private void BT_back_TouchUpInside(object sender, EventArgs e)
        {
            this.NavigationController.PopViewController(true);
        }

        private async void BT_register_TouchUpInside(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_userName.Text) || !txt_userName.Text.Contains("@vuthao.com"))
            {
                CmmIOSFunction.commonAlertMessage(this, "VNA BPM", "Please enter your business email.");
            }
            else
            {
                this.View.EndEditing(true);
                BT_back.Enabled = false;
                view_loading.Hidden = false;
                view_register.Hidden = true;
                var accountName = txt_userName.Text.TrimEnd();

                await Task.Run(() =>
                {
                    //thuc hien api register
                    if (CmmFunction.Register(accountName))
                    {
                        InvokeOnMainThread(() =>
                        {
                            try
                            {
                                //CmmIOSFunction.commonAlertMessage(this, "Thông báo", CmmIOSFunction.correctEmailAlert);

                                UIAlertController alert = UIAlertController.Create("Thông báo"//"VNA BPM"
                                        , "Please check your email! We have sent a confirmation message."
                                        , UIAlertControllerStyle.Alert);
                                alert.AddAction(UIAlertAction.Create("Close", UIAlertActionStyle.Destructive, (UIAlertAction) =>
                                {
                                    BT_login_TouchUpInside(null, null);
                                }));
                                this.PresentViewController(alert, true, null);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                        });
                    }
                    else
                    {
                        InvokeOnMainThread(() =>
                        {
                            CmmIOSFunction.commonAlertMessage(this, "VNA BPM", "Your email is not linked to account registed.You can only sign up with an registed account via this app.");

                            view_loading.Hidden = true;
                            view_register.Hidden = false;
                            BT_back.Enabled = true;
                        });
                    }

                    /*
                    // done
                    InvokeOnMainThread(() =>
                    {
                        if (txt_userName.Text == "thuyngo")
                        {
                            CmmIOSFunction.commonAlertMessage(this, "BPM", "Register information is correct");
                        }
                        else
                        {
                            view_loading.Hidden = true;
                            CmmIOSFunction.commonAlertMessage(this, "BPM", "Register information is incorrect, please try again.");
                            view_register.Hidden = false;
                            BT_back.Enabled = true;
                        }
                    });*/
                });
            }
        }

        private void BT_login_TouchUpInside(object sender, EventArgs e)
        {
            LoginViewController LoginViewController = (LoginViewController)Storyboard.InstantiateViewController("LoginViewController");
            appD.NavController.PushViewController(LoginViewController, true);
            if (appD.NavController != null)
            {
                var controllers = this.NavigationController.ViewControllers;
                List<UIViewController> newcontrollers = new List<UIViewController> { };

                foreach (var item in controllers)
                {
                    if (item.GetType() != typeof(RegisterViewController))
                        newcontrollers.Add(item);
                }
                appD.NavController.ViewControllers = newcontrollers.ToArray();
            }
        }
        #endregion

    }
}


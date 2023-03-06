using System;
using System.Drawing;
using AudioToolbox;
using CoreGraphics;
using BPMOPMobile.Bean;
using Foundation;
using UIKit;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class ToastMessageView : UIViewController
    {
        private System.Timers.Timer timerExpire;
        private int count_expire;
        private TimeSpan time_collase;
        SystemSound sound;
        private AppDelegate appD;
        string title, content, templateID;
        BeanNotify notify { get; set; }

        public ToastMessageView(IntPtr handle) : base(handle)
        {
            time_collase = TimeSpan.FromSeconds(5);
            this.View.Frame = new CoreGraphics.CGRect(0, 30, this.View.Bounds.Width, 70);
            appD = UIApplication.SharedApplication.Delegate as AppDelegate;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            toast_view.Layer.BorderWidth = 0.5f;
            toast_view.Layer.BorderColor = UIColor.LightGray.ColorWithAlpha(0.8f).CGColor;
            calculatorTimerExpire();

            #region delegate
            BT_moveDetail.TouchUpInside += BT_moveDetail_TouchUpInside;
            #endregion
        }

        #region private - public method

        public void SetContent(string _title, string _content, string _templateID, BeanNotify _notify)
        {
            title = _title;
            content = _content;
            templateID = _templateID;
            notify = _notify;
            LoadContent();
            ViewConfiguraion();
        }

        public void ViewConfiguraion()
        {
            this.View.Layer.CornerRadius = 5;
            this.View.Layer.ShadowColor = UIColor.DarkGray.CGColor;
            this.View.Layer.ShadowOpacity = 0.8f;
            this.View.Layer.ShadowRadius = 4f;
            this.View.Layer.ShadowOffset = new CGSize(3, 3);

            SystemSound systemSound = new SystemSound(1315);
            systemSound.PlayAlertSound();
        }

        private void LoadContent()
        {
            lbl_title.Text = title;
            lbl_content.Text = content;
        }

        private void calculatorTimerExpire()
        {
            try
            {
                timerExpire = new System.Timers.Timer();
                timerExpire.Interval = 1000;
                timerExpire.Elapsed += TimerExpire_Elapsed;

                count_expire = Convert.ToInt16(time_collase.TotalSeconds);
                timerExpire.Enabled = true;
                timerExpire.Start();
            }
            catch (Exception ex)
            {
                Console.Write("ToastMessageView - calculatorTimerExpire - ERR: " + ex.ToString());
            }
        }

        #endregion

        #region event

        private void BT_moveDetail_TouchUpInside(object sender, EventArgs e)
        {
            if (notify != null)
            {
                var window = UIApplication.SharedApplication.KeyWindow;
                var vc = window.RootViewController;
                while (vc.PresentedViewController != null)
                {
                    //if (vc.PresentedViewController.Class.Name == "AgreeOrRejectView")
                    vc.PresentedViewController.DismissViewControllerAsync(true);
                    break;
                }

                // new sidecontroller
                //if (appD.SideBarController.IsOpen)
                //    appD.SideBarController.ToggleMenu();

                #region chuyen sang ViewRequestDetailsv2
                //ViewRequestDetails detailView = (ViewRequestDetails)Storyboard.InstantiateViewController("ViewRequestDetails");
                //detailView.setContentFromPush(notify, true);
                //appD.NavController.PushViewController(detailView, true);
                #endregion
            }

            BT_moveDetail.UserInteractionEnabled = false;
            if (this.sound != null)
            {
                sound.Close();
            }
            timerExpire.Stop();
            this.DismissViewController(true, null);
            this.View.RemoveFromSuperview();
        }

        private void TimerExpire_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            count_expire--;
            TimeSpan time = TimeSpan.FromSeconds(count_expire);
            InvokeOnMainThread(() =>
            {
                if (count_expire == 0)
                {
                    if (this.sound != null)
                    {
                        sound.Close();
                    }
                    timerExpire.Stop();
                    this.DismissViewController(true, null);
                    this.View.RemoveFromSuperview();
                }
            });
        }

        #endregion
    }
}


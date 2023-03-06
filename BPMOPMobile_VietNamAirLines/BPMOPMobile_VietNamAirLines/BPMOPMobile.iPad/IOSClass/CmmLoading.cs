using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPMOPMobile.Class;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.IOSClass
{
    public class CmmLoading : UIView
    {
        // control declarations
        UIView view;
        UILabel loadingLabel;
        UIActivityIndicatorView activitySpinner;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="content">Không còn dùng</param>
        public CmmLoading(CGRect frame, string content)// : base(frame)
        {
            CmmIOSFunction.EventUpdateLoading += CmmIOSFunction_EventUpdateLoading;

            Frame = UIScreen.MainScreen.Bounds;
            BackgroundColor = UIColor.Black.ColorWithAlpha(0);
            view = new UIView(frame);
            view.Layer.CornerRadius = 10;
            // configurable bits
            view.BackgroundColor = UIColor.White.ColorWithAlpha(0.7f);
            view.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;

            // create the activity spinner, center it horizontall and put it 5 points above center x
            activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
            activitySpinner.Color = UIColor.FromRGB(51, 95, 190);
            activitySpinner.Frame = new CGRect((frame.Width - 50) / 2, 70, 50, 50);
            activitySpinner.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
            activitySpinner.StartAnimating();

            // create and configure the "Loading Data" label
            loadingLabel = new UILabel(new CGRect(0, activitySpinner.Frame.GetMaxY(), frame.Width, 30));
            loadingLabel.TextColor = UIColor.FromRGB(51, 95, 190);
            loadingLabel.Font = UIFont.SystemFontOfSize(16);
            //loadingLabel.Text = content;
            //loadingLabel.Text = CmmFunction.GetTitle("TEXT_LOADING_PROGRESSING", "Đang xử lý...");/// Do BeanAppLanguage chưa có field này nên tạm xử lý tay.
            loadingLabel.Text = CmmVariable.SysConfig.LangCode == "1033" ? "Loading..." : "Đang xử lý...";
            loadingLabel.TextAlignment = UITextAlignment.Center;
            loadingLabel.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;

            view.AddSubview(loadingLabel);
            view.AddSubview(activitySpinner);
            AddSubview(view);

        }

        private void CmmIOSFunction_EventUpdateLoading(string msg, string percent)
        {
            InvokeOnMainThread(delegate
            {
                loadingLabel.Text = msg;
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
}
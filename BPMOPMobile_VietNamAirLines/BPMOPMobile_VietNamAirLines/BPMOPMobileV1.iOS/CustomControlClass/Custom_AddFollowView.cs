using System;
using BPMOPMobile.Class;
using BPMOPMobileV1.iOS.ViewControllers;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobileV1.iOS.CustomControlClass
{
   public class Custom_AddFollowView : UIView
    {
        UIButton BT_action;
        UIImageView iv_left;
        UILabel lbl_title;
        UIView view_rectangle;
        public bool isFollow;

        private Custom_AddFollowView()
        {
            this.BackgroundColor = UIColor.White;

            iv_left = new UIImageView();
            iv_left.ContentMode = UIViewContentMode.ScaleAspectFit;


            lbl_title = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(14),
                TextColor = UIColor.FromRGB(51, 95, 179),
                TextAlignment = UITextAlignment.Left
            };

            BT_action = new UIButton();

            view_rectangle = new UIView();
            view_rectangle.BackgroundColor = UIColor.White;

            this.AddSubviews(new UIView[] { view_rectangle, iv_left, lbl_title, BT_action  });

            if (BT_action != null)
                BT_action.AddTarget(HandleBtnAction, UIControlEvent.TouchUpInside);
        }

        private void HandleBtnAction(object sender, EventArgs e)
        {
            if (viewController != null && viewController.GetType() == typeof(RequestDetailsV2))
            {
                RequestDetailsV2 controller = (RequestDetailsV2)viewController;
                controller.HandleAddFollow();
            }
        }

        private static Custom_AddFollowView instance = null;
        public static Custom_AddFollowView Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Custom_AddFollowView();
                }
                return instance;
            }
        }

        public void InitFrameView(CGRect frame)
        {
            this.Frame = frame;

            iv_left.Frame = new CGRect(20, 20, 16, 16);
            lbl_title.Frame = new CGRect(50, 13, Frame.Width - 56, 30);

            BT_action.Frame = new CGRect(0, 0, Frame.Width, Frame.Height);
            view_rectangle.Frame = new CGRect(0, 0, this.Frame.Width, this.Frame.Height);

            this.Layer.ShadowColor = UIColor.DarkGray.CGColor;
            this.Layer.ShadowPath = UIBezierPath.FromRect(new CGRect(2, 2, Frame.Width, Frame.Height)).CGPath;
            this.Layer.ShadowRadius = 2;
            this.Layer.ShadowOffset = new CGSize(1, 1);
            this.Layer.ShadowOpacity = 0.5f;
            this.BackgroundColor = UIColor.DarkGray.ColorWithAlpha(0.3f);

            view_rectangle.ClipsToBounds = true;
            UIBezierPath mPath = UIBezierPath.FromRoundedRect(Layer.Bounds, (UIRectCorner.BottomLeft | UIRectCorner.BottomRight), new CGSize(width: 5, height: 5));
            CAShapeLayer maskLayer = new CAShapeLayer();
            maskLayer.Frame = view_rectangle.Layer.Bounds;
            maskLayer.Path = mPath.CGPath;
            view_rectangle.Layer.Mask = maskLayer;
        }

        public void LoadContent()
        {
            if (isFollow)
            {
                lbl_title.Text = CmmFunction.GetTitle("MESS_UNFOLLOW_TASK", "Hủy theo dõi công việc này");
                iv_left.Image = UIImage.FromFile("Icons/icon_cancel.png");
            }
            else
            {
                lbl_title.Text = CmmFunction.GetTitle("MESS_FOLLOW_TASK", "Đặt theo dõi công việc này");
                iv_left.Image = UIImage.FromFile("Icons/icon_check.png");
            }

        }

        public UIViewController viewController { get; set; }
    }
}
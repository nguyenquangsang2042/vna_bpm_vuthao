using System;
using System.Collections.Generic;
using System.Drawing;
using BPMOPMobile.Bean;
using BPMOPMobileV1.iOS.ViewControllers;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobileV1.iOS.CustomControlClass
{
    public class UserGroup_CollectionCell : UICollectionViewCell
    {
        public static NSString Key = new NSString("userCellId");
        UILabel lbl_title;
        UIButton BT_remove;
        KeyValuePair<string, bool> section;
        BeanUserAndGroup userGroup { get; set; }
        public UIViewController parentView { get; set; }

        [Export("initWithFrame:")]
        public UserGroup_CollectionCell(RectangleF frame) : base(frame)
        {
            ViewConfiguration();
        }
        private void ViewConfiguration()
        {
            this.BackgroundColor = UIColor.White;
            //this.Layer.BorderColor = UIColor.LightGray.ColorWithAlpha(0.5f).CGColor;
            //this.Layer.BorderWidth = 0.5f;
            //this.Layer.CornerRadius = 5;


            lbl_title = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(65, 80, 134),
                TextAlignment = UITextAlignment.Left,
            };

            BT_remove = new UIButton();
            UIImage img = UIImage.FromFile("Icons/icon_close_red.png");
            BT_remove.SetImage(img, UIControlState.Normal);
            BT_remove.TouchUpInside += delegate
            {
                if (parentView.GetType() == typeof(FormShareView))
                {
                    FormShareView formShareView = parentView as FormShareView;
                    formShareView.RemoveUserFromColletionView(userGroup);
                }
               
            };

            this.AddSubviews(lbl_title, BT_remove);
        }
        
        public void UpdateRow(BeanUserAndGroup _user, UIViewController _parent)
        {
            userGroup = _user;
            parentView = _parent;
            lbl_title.Text = userGroup.Name;
            
        }
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            var width = userGroup.Name.StringSize(UIFont.SystemFontOfSize(13)).Width + 5;
            lbl_title.Frame = new CGRect(5, 3, width, 20);
            BT_remove.Frame = new CGRect(lbl_title.Frame.Right, 4, 18, 18);
        }
    }
}

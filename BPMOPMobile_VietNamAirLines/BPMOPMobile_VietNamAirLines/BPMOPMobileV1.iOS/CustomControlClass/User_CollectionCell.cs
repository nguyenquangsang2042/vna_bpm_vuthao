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
    public class User_CollectionCell : UICollectionViewCell
    {
        public static NSString Key = new NSString("userCellId");
        UILabel lbl_title;
        UIButton BT_remove;
        KeyValuePair<string, bool> section;
        BeanUser user { get; set; }
        public UIViewController parentView { get; set; }

        [Export("initWithFrame:")]
        public User_CollectionCell(RectangleF frame) : base(frame)
        {
            ViewConfiguration();
        }
        private void ViewConfiguration()
        {
            this.BackgroundColor = UIColor.FromRGB(249, 249, 249);
            this.Layer.CornerRadius = 3;
            this.ClipsToBounds = true;


            lbl_title = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(65, 80, 134),
                TextAlignment = UITextAlignment.Left,
            };

            BT_remove = new UIButton();
            UIImage img = UIImage.FromFile("Icons/icon_close_red_userOrGroup.png");
            BT_remove.ContentEdgeInsets = new UIEdgeInsets(2, 2, 2, 2);
            BT_remove.SetImage(img, UIControlState.Normal);
            BT_remove.TouchUpInside += delegate
            {
                if (parentView.GetType() == typeof(FormShareView))
                {
                    //FormShareView formShareView = parentView as FormShareView;
                    //formShareView.RemoveUserFromColletionView(user);
                }
                else if (parentView.GetType() == typeof(ListUserView))
                {
                    ListUserView listUser = parentView as ListUserView;
                    listUser.RemoveUserFromColletionView(user);
                }
            };

            this.AddSubviews(lbl_title, BT_remove);
        }
        
        public void UpdateRow(BeanUser _user, UIViewController _parent)
        {
            user = _user;
            parentView = _parent;
            lbl_title.Text = user.FullName;
            
        }
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            var width = user.FullName.StringSize(UIFont.SystemFontOfSize(13)).Width + 5;
            lbl_title.Frame = new CGRect(5, 3, width, 20);
            BT_remove.Frame = new CGRect(lbl_title.Frame.Right, 4, 18, 18);
        }
    }
}

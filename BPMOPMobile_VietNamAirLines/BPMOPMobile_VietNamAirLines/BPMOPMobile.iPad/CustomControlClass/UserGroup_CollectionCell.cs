using System;
using System.Collections.Generic;
using System.Drawing;
using BPMOPMobile.Bean;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.CustomControlClass
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
            this.BackgroundColor = UIColor.FromRGB(239,239,239);
            this.Layer.CornerRadius = 12;
            
            lbl_title = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(65, 80, 134),
                TextAlignment = UITextAlignment.Left,
            };

            BT_remove = new UIButton();
            UIImage img = UIImage.FromFile("Icons/icon_close_circle_red.png");
            BT_remove.SetImage(img, UIControlState.Normal);
            BT_remove.TouchUpInside += delegate
            {
                if (parentView.GetType() == typeof(FormShareView))
                {
                    FormShareView formShareView = parentView as FormShareView;
                    formShareView.RemoveUserFromColletionView(userGroup);
                }
                else if (parentView.GetType() == typeof(FormUserAndGroupView))
                {
                    FormUserAndGroupView listUser = parentView as FormUserAndGroupView;
                    listUser.RemoveUserFromColletionView(userGroup);
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
            BT_remove.Frame = new CGRect(lbl_title.Frame.Right, 5, 16, 16);
        }
    }
}

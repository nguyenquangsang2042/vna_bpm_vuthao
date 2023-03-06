using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPMOPMobile.Bean;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.CustomControlClass
{
    public class Custom_UserCell: UITableViewCell
    {
        UIViewController parentView;
        UILabel lbl_imgCover, lbl_name, lbl_email, lbl_leftLine;
        UIButton btn_action;
        UIImageView iv_avatar, iv_group;
        BeanUser currentUser;
        bool itemSelected, isOdd;

        public Custom_UserCell(NSString cellID, UIViewController _parentView) : base(UITableViewCellStyle.Default, cellID)
        {
            Accessory = UITableViewCellAccessory.None;
            parentView = _parentView;
        }

        public Custom_UserCell(IntPtr handle) : base(handle)
        {
        }

        public Custom_UserCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
        {
        }

        public void UpdateCell(BeanUser _user, bool _selectedUser, bool _isOdd)
        {
            currentUser = _user;
            itemSelected = _selectedUser;
            isOdd = _isOdd;

            ViewConfiguration();
            LoadData();
        }

        private void ViewConfiguration()
        {
            iv_avatar = new UIImageView();
            iv_avatar.ContentMode = UIViewContentMode.ScaleAspectFit;
            iv_avatar.BackgroundColor = UIColor.White;
            iv_avatar.Image = UIImage.FromFile("Icons/icon_profile.png");
            iv_avatar.Hidden = true;

            iv_group = new UIImageView();
            iv_group.ContentMode = UIViewContentMode.ScaleAspectFit;
            iv_group.Image = UIImage.FromFile("Icons/icon_group.png");
            iv_group.Hidden = true;

            lbl_imgCover = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
                BackgroundColor = UIColor.Blue,
                TextColor = UIColor.White
            };

            lbl_name = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(14, UIFontWeight.Semibold),
                TextColor = UIColor.FromRGB(51, 51, 51),
                TextAlignment = UITextAlignment.Left,
                BackgroundColor = UIColor.Clear
            };

            lbl_email = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(12, UIFontWeight.Light),
                TextColor = UIColor.FromRGB(51, 51, 51),
                TextAlignment = UITextAlignment.Left,
                BackgroundColor = UIColor.Clear
            };

            btn_action = new UIButton();
            btn_action.SetImage(UIImage.FromFile("Icons/icon_check.png"), UIControlState.Normal);
            btn_action.ContentEdgeInsets = new UIEdgeInsets(8, 7, 8, 7);
            btn_action.Hidden = true;
            btn_action.UserInteractionEnabled = itemSelected;

            lbl_leftLine = new UILabel()
            {
                BackgroundColor = UIColor.FromRGB(51, 95, 179)
            };
            lbl_leftLine.Hidden = true;

            if(btn_action != null)
                btn_action.AddTarget(HandleBtnDelete, UIControlEvent.TouchUpInside);

            ContentView.AddSubviews(new UIView[] { lbl_imgCover, iv_avatar, lbl_name, lbl_email, btn_action, lbl_leftLine, iv_group });
        }

        private void HandleBtnDelete(object sender, EventArgs e)
        {
            if (parentView != null && parentView.GetType() == typeof(FormShareView))
            {
                //FormShareView controller = (FormShareView)parentView;
                //controller.RemoveUserFromList(currentUser);
            }
            else if (parentView != null && parentView.GetType() == typeof(FormCreateView))
            {
                FormCreateView controller = (FormCreateView)parentView;
                controller.RemoveUserFromList(currentUser);
            }
        }

        public void LoadData()
        {
            if (currentUser.IsGroup.HasValue && !currentUser.IsGroup.Value)
            {
                if (!string.IsNullOrEmpty(currentUser.Name))
                {
                    lbl_imgCover.Text = currentUser.Name.Substring(currentUser.Name.Length - 1, 1);
                    lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));
                }

                lbl_name.Text = currentUser.Name;
                lbl_email.Text = currentUser.Email;

                if (string.IsNullOrEmpty(currentUser.ImagePath))
                {
                    iv_avatar.Hidden = true;
                    lbl_imgCover.Hidden = false;
                }
                else
                {
                    iv_avatar.Hidden = false;
                    lbl_imgCover.Hidden = true;
                }

                iv_group.Hidden = true;
            }
            else
            {
                iv_avatar.Hidden = true;
                lbl_imgCover.Hidden = false;
                iv_group.Hidden = false;

                lbl_name.Text = currentUser.Name;
                lbl_imgCover.BackgroundColor = UIColor.FromRGB(65, 80, 134);
            }

            if (itemSelected)
            {
                btn_action.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);
                btn_action.SetImage(UIImage.FromFile("Icons/icon_close_circle_red.png"), UIControlState.Normal);
                btn_action.Hidden = false;
                lbl_leftLine.Hidden = true;
                if (isOdd)
                    ContentView.BackgroundColor = UIColor.White;
                else
                    ContentView.BackgroundColor = UIColor.FromRGB(249, 249, 249);
            }
            else
            {
                if (currentUser.IsSelected.HasValue && currentUser.IsSelected.Value)
                {
                    btn_action.ContentEdgeInsets = new UIEdgeInsets(8, 7, 8, 7);
                    ContentView.BackgroundColor = UIColor.FromRGB(246, 249, 255);
                    btn_action.Hidden = false;
                    lbl_leftLine.Hidden = false;
                }
                else
                {
                    btn_action.Hidden = true;
                    lbl_leftLine.Hidden = true;

                    if (isOdd)
                        ContentView.BackgroundColor = UIColor.White;
                    else
                        ContentView.BackgroundColor = UIColor.FromRGB(249, 249, 249);
                }
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            lbl_leftLine.Frame = new CGRect(0, 0, 5, ContentView.Frame.Height);
            
            lbl_imgCover.Frame = new CGRect(15, 20, 38, 38);
            iv_avatar.Frame = new CGRect(15, 20, 38, 38);
            btn_action.Frame = new CGRect(ContentView.Frame.Width - 55, 13, 30, 30);

            iv_group.Frame = new CGRect(lbl_imgCover.Frame.Left + 10, lbl_imgCover.Frame.Top + 10, 18, 15);

            if (currentUser.IsGroup.HasValue && !currentUser.IsGroup.Value)
            {
                lbl_name.Frame = new CGRect(lbl_imgCover.Frame.Right + 15, 19, this.ContentView.Frame.Width - (lbl_imgCover.Frame.Right + 15 + 55), 20);
                lbl_email.Frame = new CGRect(lbl_imgCover.Frame.Right + 15, lbl_name.Frame.Bottom, lbl_name.Frame.Width, 20);
            }
            else
            {
                lbl_name.Frame = new CGRect(lbl_imgCover.Frame.Right + 15, (ContentView.Frame.Height / 2) - 10, this.ContentView.Frame.Width - (lbl_imgCover.Frame.Right + 15 + 55), 20);
                lbl_email.Frame = CGRect.Empty;
            }

            lbl_imgCover.Layer.CornerRadius = lbl_imgCover.Frame.Width / 2;
            lbl_imgCover.ClipsToBounds = true;

            iv_avatar.Layer.CornerRadius = iv_avatar.Frame.Width / 2;
            iv_avatar.ClipsToBounds = true;
        }
    }
}
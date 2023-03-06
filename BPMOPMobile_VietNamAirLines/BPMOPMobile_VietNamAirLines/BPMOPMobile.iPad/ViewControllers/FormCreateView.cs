using BPMOPMobile.Bean;
using BPMOPMobile.iPad.CustomControlClass;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Globalization;
using UIKit;

namespace BPMOPMobile.iPad
{
    public partial class FormCreateView : UIViewController
    {
        private NSObject _willResignActiveNotificationObserver;
        private NSObject _didBecomeActiveNotificationObserver;
        private nfloat positionBotOfCurrentViewInput { get; set; }

        List<BeanUser> lst_selectedUser = new List<BeanUser>();

        public FormCreateView (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UITapGestureRecognizer gesture = new UITapGestureRecognizer(() => {
                View.EndEditing(true);
            });

            gesture.ShouldReceiveTouch += delegate (UIGestureRecognizer recognizer, UITouch touch)
            {
                var touchView = touch.View.Class.Name;
                if (touchView == "UITextField" || touchView == "UITextView")
                    positionBotOfCurrentViewInput = GetPositionBotView(touch.View);
                else
                    positionBotOfCurrentViewInput = 0.0f;

                return true;
            };

            gesture.CancelsTouchesInView = false;
            View.AddGestureRecognizer(gesture);

            ViewConfiguration();
            LoadContent();

            #region delegate
            BT_close.TouchUpInside += BT_close_TouchUpInside;
            txt_note.Started += Txt_Note_Started;
            txt_note.Ended += Txt_Note_Ended;
            BT_startDate.TouchUpInside += BT_startDate_TouchUpInside;
            BT_user.TouchUpInside += BT_user_TouchUpInside;
            #endregion
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            _willResignActiveNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, KeyBoardUpNotification);
            _didBecomeActiveNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyBoardDownNotification);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            if (_willResignActiveNotificationObserver != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(_willResignActiveNotificationObserver);

            if (_didBecomeActiveNotificationObserver != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(_didBecomeActiveNotificationObserver);
        }

        #region public - private method
        private void ViewConfiguration()
        {
            BT_close.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);
            BT_approve.ContentEdgeInsets = new UIEdgeInsets(7, 5, 7, 5);

            UIImageView iv_right = new UIImageView();
            iv_right.Image = UIImage.FromFile("Icons/icon_calendar.png");
            iv_right.ContentMode = UIViewContentMode.ScaleAspectFit;
            iv_right.Frame = new CGRect(0, 0, 16, 16);

            UIView rightView = new UIView(new CGRect(0, 0, 30, 16));
            rightView.AddSubview(iv_right);

            tf_startDate.RightView = rightView;
            tf_startDate.RightViewMode = UITextFieldViewMode.Always;

            UIImageView iv_rightUser = new UIImageView();
            iv_rightUser.Image = UIImage.FromFile("Icons/icon_user.png");
            iv_rightUser.ContentMode = UIViewContentMode.ScaleAspectFit;
            iv_rightUser.Frame = new CGRect(0, 0, 16, 16);

            UIView rightViewUser = new UIView(new CGRect(0, 0, 30, 16));
            rightViewUser.AddSubview(iv_rightUser);

            tf_user.RightView = rightViewUser;
            tf_user.RightViewMode = UITextFieldViewMode.Always;

            view_title.Layer.BorderWidth = 1;
            view_title.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;
            view_title.Layer.CornerRadius = 3;
            view_title.ClipsToBounds = true;

            CmmIOSFunction.AddBorderView(view_title);
            CmmIOSFunction.AddBorderView(view_startDate);
            CmmIOSFunction.AddBorderView(view_user);
            CmmIOSFunction.AddBorderView(view_note);

            CmmIOSFunction.AddAttributeTitle(lbl_title);
            //CmmIOSFunction.AddAttributeTitle(lbl_startDate);
            CmmIOSFunction.AddAttributeTitle(lbl_user);
            //CmmIOSFunction.AddAttributeTitle(lbl_note);

        }

        private void LoadContent()
        {
            table_selectedUser.Source = new Users_TableSource(lst_selectedUser, this);
            table_selectedUser.ReloadData();
        }

        private void EnableInputView(bool status)
        {
            tf_title.Enabled = status;
            txt_note.Editable = status;
        }

        private nfloat GetPositionBotView(UIView view)
        {
            nfloat bottom = view.Frame.Height;
            UIView supperView = view;
            do 
            {
                bottom += supperView.Frame.Y;
                supperView = supperView.Superview;

            } while (supperView != this.View);

            return bottom + 20;
        }

        public void AddUserToList(BeanUser _selectedUser)
        {
            var index = lst_selectedUser.FindIndex(item => item.ID == _selectedUser.ID);
            if (index == -1)
                lst_selectedUser.Add(_selectedUser);
        }

        private string GetStringUsers()
        {
            string strUsers = "";
            if (lst_selectedUser.Count > 0)
            {
                foreach (var item in lst_selectedUser)
                {
                    strUsers += item.Name + "; ";
                }
            }

            if (!string.IsNullOrEmpty(strUsers))
            {
                strUsers = strUsers.Trim().TrimEnd(';');
            }

            return strUsers;
        }

        private List<string> GetListIdUserSelected()
        {
            if (lst_selectedUser.Count > 0)
            {
                List<string> lst_result = new List<string>();
                foreach (var item in lst_selectedUser)
                {
                    lst_result.Add(item.ID);
                }

                return lst_result;
            }
            else
                return null;

        }

        public void RemoveUserFromList(BeanUser _removeUser)
        {
            lst_selectedUser.Remove(_removeUser);
            table_selectedUser.ReloadData();

            tf_user.Text = GetStringUsers();
        }
        #endregion

        #region event
        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            this.DismissModalViewController(true);
        }

        void Txt_Note_Ended(object sender, EventArgs e)
        {
        }

        void Txt_Note_Started(object sender, EventArgs e)
        {
        }

        private void BT_startDate_TouchUpInside(object sender, EventArgs e)
        {
            Custom_DateTimePickerView custom_dateTimePicker = Custom_DateTimePickerView.Instance;
            if (custom_dateTimePicker.Superview != null && custom_dateTimePicker.inputView == tf_startDate)
            {
                custom_dateTimePicker.RemoveFromSuperview();
                EnableInputView(true);
            }
            else
            {
                custom_dateTimePicker.viewController = this;
                custom_dateTimePicker.inputView = tf_startDate;
                custom_dateTimePicker.InitFrameView(new CGRect(BT_startDate.Frame.X, BT_startDate.Frame.Bottom + 5, BT_startDate.Frame.Width, 168));
                custom_dateTimePicker.AddShadowForView();
                custom_dateTimePicker.SetUpDate();

                view_content.AddSubview(custom_dateTimePicker);
                view_content.BringSubviewToFront(custom_dateTimePicker);

                EnableInputView(false);
            }
        }

        private void BT_user_TouchUpInside(object sender, EventArgs e)
        {
            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            FormUserAndGroupView formUserAndGroupView = (FormUserAndGroupView)Storyboard.InstantiateViewController("FormUserAndGroupView");
            //formUserAndGroupView.SetContent(this, true, true, GetListIdUserSelected());
            PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            formUserAndGroupView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            formUserAndGroupView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            formUserAndGroupView.TransitioningDelegate = transitioningDelegate;
            this.PresentViewControllerAsync(formUserAndGroupView, true);
        }

        public void HandleAddUserAndGroupResult(List<BeanUser> _users)
        {
            if (_users.Count > 0)
            {
                foreach (var item in _users)
                {
                    lst_selectedUser.Add(item);
                }
            }

            table_selectedUser.ReloadData();
            tf_user.Text = GetStringUsers();
        }

        private void KeyBoardUpNotification(NSNotification notification)
        {
            try
            {
                if (View.Frame.Y == 0)
                {
                    CGRect keyboardSize = UIKeyboard.BoundsFromNotification(notification);

                    var topKeybroad = this.View.Frame.Height - keyboardSize.Height;
                    if (topKeybroad < positionBotOfCurrentViewInput)
                    {
                        CGRect custFrame = View.Frame;
                        custFrame.Y -= (positionBotOfCurrentViewInput - topKeybroad);
                        View.Frame = custFrame;
                    }

                }
            }
            catch (Exception ex)
            { Console.WriteLine("FormCreateView - Err: " + ex.ToString()); }
        }
        private void KeyBoardDownNotification(NSNotification notification)
        {
            try
            {
                if (View.Frame.Y != 0)
                {
                    CGRect custFrame = View.Frame;
                    custFrame.Y = 0;
                    View.Frame = custFrame;
                }
            }
            catch (Exception ex)
            { Console.WriteLine("FormCreateView - Err: " + ex.ToString()); }
        }
        #endregion

        #region custom views
        #region table data source user
        private class Users_TableSource : UITableViewSource
        {
            List<BeanUser> lst_user;
            NSString cellIdentifier = new NSString("cellUser");
            FormCreateView parentView;

            public Users_TableSource(List<BeanUser> _users, FormCreateView _parentview)
            {
                parentView = _parentview;
                lst_user = _users;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return lst_user.Count;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 78;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                Custom_UserCell cell = new Custom_UserCell(cellIdentifier, parentView);
                var user = lst_user[indexPath.Row];

                bool isOdd = true;
                if (indexPath.Row % 2 == 0)
                    isOdd = false;

                cell.UpdateCell(user, true, isOdd);
                return cell;
            }
        }
        #endregion
        #endregion
    }
}
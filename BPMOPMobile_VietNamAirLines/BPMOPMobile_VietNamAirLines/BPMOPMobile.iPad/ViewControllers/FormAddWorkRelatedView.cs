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
    public partial class FormAddWorkRelatedView : UIViewController
    {
        private NSObject _willResignActiveNotificationObserver;
        private NSObject _didBecomeActiveNotificationObserver;
        private nfloat positionBotOfCurrentViewInput { get; set; }
        List<BeanNotify> lst_workRelated = new List<BeanNotify>();
        UIViewController parentView { get; set; }

        List<ClassMenu> lst_menuItemWorkFlow = new List<ClassMenu>();
        ClassMenu currentMenuWorkFlow { get; set; }
        List<ClassMenu> lst_menuItemStatus = new List<ClassMenu>();
        ClassMenu currentMenuStatus { get; set; }

        public FormAddWorkRelatedView (IntPtr handle) : base (handle)
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
            BT_exit.TouchUpInside += BT_exit_TouchUpInside;
            BT_save.TouchUpInside += BT_save_TouchUpInside;
            BT_formDate.TouchUpInside += BT_formDate_TouchUpInside;
            BT_toDate.TouchUpInside += BT_toDate_TouchUpInside;
            BT_workFlow.TouchUpInside += BT_workFlow_TouchUpInside;
            BT_status.TouchUpInside += BT_status_TouchUpInside;
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
            BT_save.ContentEdgeInsets = new UIEdgeInsets(7, 5, 7, 5);
            BT_search.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);
            BT_exit.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);

            UIImageView iv_right = new UIImageView();
            iv_right.Image = UIImage.FromFile("Icons/icon_calendar.png");
            iv_right.ContentMode = UIViewContentMode.ScaleAspectFit;
            iv_right.Frame = new CGRect(0, 0, 16, 16);

            UIView rightView = new UIView(new CGRect(0, 0, 30, 16));
            rightView.AddSubview(iv_right);

            tf_fromDate.RightView = rightView;
            tf_fromDate.RightViewMode = UITextFieldViewMode.Always;

            UIImageView iv_right2 = new UIImageView();
            iv_right2.Image = UIImage.FromFile("Icons/icon_calendar.png");
            iv_right2.ContentMode = UIViewContentMode.ScaleAspectFit;
            iv_right2.Frame = new CGRect(0, 0, 16, 16);

            UIView rightView2 = new UIView(new CGRect(0, 0, 30, 16));
            rightView2.AddSubview(iv_right2);

            tf_toDate.RightView = rightView2;
            tf_toDate.RightViewMode = UITextFieldViewMode.Always;

            BT_formDate.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;
            BT_formDate.Layer.CornerRadius = 3;
            BT_formDate.Layer.BorderWidth = 1;
            BT_formDate.ClipsToBounds = true;

            BT_toDate.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;
            BT_toDate.Layer.CornerRadius = 3;
            BT_toDate.Layer.BorderWidth = 1;
            BT_toDate.ClipsToBounds = true;

            BT_workFlow.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;
            BT_workFlow.Layer.CornerRadius = 3;
            BT_workFlow.Layer.BorderWidth = 1;
            BT_workFlow.ClipsToBounds = true;
            BT_workFlow.ContentEdgeInsets = new UIEdgeInsets(0, 16, 0, 45);

            BT_status.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;
            BT_status.Layer.CornerRadius = 3;
            BT_status.Layer.BorderWidth = 1;
            BT_status.ClipsToBounds = true;
            BT_status.ContentEdgeInsets = new UIEdgeInsets(0, 16, 0, 45);

            table_workRelated.ContentInset = new UIEdgeInsets(0, 0, 0, 0);

            string str_hintSearch = "Nhập mã hoặc nội dung";
            var attHintSearch = new NSMutableAttributedString(str_hintSearch);
            attHintSearch.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(14), new NSRange(0, str_hintSearch.Length));
            attHintSearch.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(153, 153, 153), new NSRange(0, str_hintSearch.Length));

            tf_search.AttributedPlaceholder = attHintSearch;

            CmmIOSFunction.AddBorderView(view_keySearch);
        }

        private void LoadContent()
        {
            BeanNotify notify = new BeanNotify() { ID = Guid.NewGuid(), Title = "Đề xuất phê duyệt tuyển dụng kế toán", Created = DateTime.Now.AddDays(-1), DueDate = DateTime.Now.AddDays(1), ListName = "Quy trình đăng kí tuyển dụng nhân sự", Note = "Chờ Phê Duyệt", EmailUpdate = "Thanh", Status = 0, RequestID = "ĐX-1517/2020" };
            BeanNotify notify1 = new BeanNotify() { ID = Guid.NewGuid(), Title = "Phê duyệt đề xuất đi công tác Hà Nội", Created = DateTime.Now.AddDays(-1), DueDate = DateTime.Now.AddDays(1), ListName = "Quy trình đề xuất đi công tác Hà Nội tháng 11", AssignedBy = "Bùi Thị B", Note = "Đã Phê Duyệt", Status = 1, RequestID = "TCHC-2004006" };
            BeanNotify notify2 = new BeanNotify() { ID = Guid.NewGuid(), Title = "Đề nghị tăng ngân sách công tác phí", Created = DateTime.Now.AddDays(-2), DueDate = DateTime.Now.AddDays(-1), ListName = "Quy trình đăng ký tăng công tác phí", AssignedBy = "Đỗ Văn Thừa", Note = "Bổ sung thông tin", EmailUpdate = "Khanh", Status = 2, RequestID = "RETY-6872020" };
            BeanNotify notify3 = new BeanNotify() { ID = Guid.NewGuid(), Title = "Yêu cầu đào tạo kỹ năng Software", Created = DateTime.Now.AddDays(-1), DueDate = DateTime.Now, ListName = "Quy trình đăng ký đào tạo kỹ năng Software", Note = "Từ Chối", EmailUpdate = "B", Status = 3, RequestID = "SYTG-157820" };
            BeanNotify notify4 = new BeanNotify() { ID = Guid.NewGuid(), Title = "Yêu cầu soạn hợp đồng chuyển khoản", Created = DateTime.Now.AddDays(-1), DueDate = DateTime.Now.AddDays(1), ListName = "Quy trình đăng ký soạn hợp đồng chuyển khoản", Note = "Chờ QCTT Phê Duyệt", EmailUpdate = "C", Status = 0, RequestID = "STFV-1347666" };
            BeanNotify notify5 = new BeanNotify() { ID = Guid.NewGuid(), Title = "Đề xuất phê duyệt tuyển dụng kế toán", Created = DateTime.Now.AddDays(-1), DueDate = DateTime.Now.AddDays(1), ListName = "Quy trình đăng kí tuyển dụng nhân sự", AssignedBy = "Thanh", Note = "Chờ Phê Duyệt", Status = 0, RequestID = "RETY-6872020" };
            BeanNotify notify6 = new BeanNotify() { ID = Guid.NewGuid(), Title = "Đề xuất phê duyệt tuyển dụng kế toán", Created = DateTime.Now.AddDays(-1), DueDate = DateTime.Now.AddDays(1), ListName = "Quy trình đăng kí tuyển dụng nhân sự", AssignedBy = "My", Note = "Chờ Phê Duyệt", Status = 0, RequestID = "SYTG-157820" };

            lst_workRelated.AddRange(new BeanNotify[] { notify, notify1, notify2, notify3, notify4, notify5, notify6 });

            table_workRelated.Source = new WorkRelated_TableSource(lst_workRelated, this);
            table_workRelated.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            table_workRelated.ReloadData();

            ClassMenu m1 = new ClassMenu() { ID = 0, section = -1, title = "Tất cả", isSelected = true};
            ClassMenu m2 = new ClassMenu() { ID = 1, section = 0, title = "Bảng dự trù chi phí đoàn thanh niên" };
            ClassMenu m3 = new ClassMenu() { ID = 2, section = 1, title = "Bảng dự trù kinh phí công đoàn" };
            ClassMenu m4 = new ClassMenu() { ID = 3, section = 2, title = "Bảng dự trù kinh phí đảng uỷ" };
            ClassMenu m5 = new ClassMenu() { ID = 4, section = 3, title = "Bảng đánh giá năng lực nhân viên" };
            ClassMenu m6 = new ClassMenu() { ID = 5, section = 4, title = "Báo cáo công việc gia hạn hợp đồng" };

            lst_menuItemWorkFlow.AddRange(new[] { m1, m2, m3, m4, m5, m6 });
            currentMenuWorkFlow = m1;

            ClassMenu mStatus1 = new ClassMenu() { ID = 0, section = -1, title = "Tất cả", isSelected = true };
            ClassMenu mStatus2 = new ClassMenu() { ID = 1, section = 0, title = "Chờ phê duyệt" };
            ClassMenu mStatus3 = new ClassMenu() { ID = 2, section = 1, title = "Đã phê duyệt" };
            ClassMenu mStatus4 = new ClassMenu() { ID = 3, section = 2, title = "Không phê duyệt" };
            ClassMenu mStatus5 = new ClassMenu() { ID = 4, section = 3, title = "Hủy bỏ" };
            ClassMenu mStatus6 = new ClassMenu() { ID = 5, section = 4, title = "Trang thái khác" };

            lst_menuItemStatus.AddRange(new[] { mStatus1, mStatus2, mStatus3, mStatus4, mStatus5, mStatus6 });
            currentMenuStatus = mStatus1;
        }

        public void SetContent(UIViewController _parentView)
        {
            parentView = _parentView;
        }

        private void CloseOptionMenu()
        {
            Custom_MenuOption custom_menuOption = Custom_MenuOption.Instance;
            if (custom_menuOption.Superview != null)
                custom_menuOption.RemoveFromSuperview();
        }

        private void CloseDatePicker()
        {
            Custom_DateTimePickerView custom_dateTimePicker = Custom_DateTimePickerView.Instance;
            if (custom_dateTimePicker.Superview != null)
                custom_dateTimePicker.RemoveFromSuperview();
        }

        private void EnableInputView(bool status)
        {
            tf_search.Enabled = status;
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
        #endregion

        #region event
        private void BT_exit_TouchUpInside(object sender, EventArgs e)
        {
            this.DismissModalViewController(true);
        }

        private void BT_formDate_TouchUpInside(object sender, EventArgs e)
        {
            CloseOptionMenu();

            Custom_DateTimePickerView custom_dateTimePicker = Custom_DateTimePickerView.Instance;
            if (custom_dateTimePicker.Superview != null && custom_dateTimePicker.inputView == tf_fromDate)
            {
                custom_dateTimePicker.RemoveFromSuperview();
                EnableInputView(true);
            }
            else
            {
                custom_dateTimePicker.viewController = this;
                custom_dateTimePicker.inputView = tf_fromDate;
                custom_dateTimePicker.InitFrameView(new CGRect(BT_formDate.Frame.X, BT_formDate.Frame.Bottom + 5, BT_formDate.Frame.Width, 168));
                custom_dateTimePicker.AddShadowForView();
                custom_dateTimePicker.SetUpDate();

                view_content.AddSubview(custom_dateTimePicker);
                view_content.BringSubviewToFront(custom_dateTimePicker);
                
                EnableInputView(false);
            }
        }

        private void BT_toDate_TouchUpInside(object sender, EventArgs e)
        {
            CloseOptionMenu();

            Custom_DateTimePickerView custom_dateTimePicker = Custom_DateTimePickerView.Instance;
            if (custom_dateTimePicker.Superview != null && custom_dateTimePicker.inputView == tf_toDate)
            {
                custom_dateTimePicker.RemoveFromSuperview();
                EnableInputView(true);
            }
            else
            {
                custom_dateTimePicker.viewController = this;
                custom_dateTimePicker.inputView = tf_toDate;
                custom_dateTimePicker.InitFrameView(new CGRect(BT_toDate.Frame.X, BT_toDate.Frame.Bottom + 5, BT_toDate.Frame.Width, 168));
                custom_dateTimePicker.AddShadowForView();
                custom_dateTimePicker.SetUpDate();

                view_content.AddSubview(custom_dateTimePicker);
                view_content.BringSubviewToFront(custom_dateTimePicker);

                EnableInputView(false);
            }
        }

        private void BT_workFlow_TouchUpInside(object sender, EventArgs e)
        {
            CloseDatePicker();

            Custom_MenuOption custom_menuOption = Custom_MenuOption.Instance;
            if (custom_menuOption.Superview != null && custom_menuOption.BtnInputView == BT_workFlow)
            {
                custom_menuOption.RemoveFromSuperview();
                EnableInputView(true);
            }
            else
            {
                custom_menuOption.ItemNoIcon = false;
                custom_menuOption.viewController = this;
                custom_menuOption.BtnInputView = BT_workFlow;
                custom_menuOption.InitFrameView(new CGRect(BT_workFlow.Frame.X, BT_workFlow.Frame.Bottom + 5, BT_workFlow.Frame.Width, 6 * custom_menuOption.RowHeigth));
                custom_menuOption.AddShadowForView();
                //custom_menuOption.ListItemMenu = lst_menuItemWorkFlow;
                custom_menuOption.TableLoadData();

                view_content.AddSubview(custom_menuOption);
                view_content.BringSubviewToFront(custom_menuOption);
                
                EnableInputView(false);
                
                if (string.IsNullOrEmpty(BT_workFlow.TitleLabel.Text))
                    BT_workFlow.SetTitle(currentMenuWorkFlow.title, UIControlState.Normal);
            }
        }

        private void BT_status_TouchUpInside(object sender, EventArgs e)
        {
            CloseDatePicker();

            Custom_MenuOption custom_menuOption = Custom_MenuOption.Instance;
            if (custom_menuOption.Superview != null && custom_menuOption.BtnInputView == BT_status)
            { 
                custom_menuOption.RemoveFromSuperview();
                EnableInputView(true);
            }
            else
            {
                custom_menuOption.ItemNoIcon = false;
                custom_menuOption.viewController = this;
                custom_menuOption.BtnInputView = BT_status;
                custom_menuOption.InitFrameView(new CGRect(BT_status.Frame.X, BT_status.Frame.Bottom + 5, BT_status.Frame.Width, 4 * custom_menuOption.RowHeigth));
                custom_menuOption.AddShadowForView();
                //custom_menuOption.ListItemMenu = lst_menuItemStatus;
                custom_menuOption.TableLoadData();

                view_content.AddSubview(custom_menuOption);
                view_content.BringSubviewToFront(custom_menuOption);

                EnableInputView(false);

                if (string.IsNullOrEmpty(BT_status.TitleLabel.Text))
                    BT_status.SetTitle(currentMenuStatus.title, UIControlState.Normal);
            }
        }

        private void BT_save_TouchUpInside(object sender, EventArgs e)
        {
            if (parentView != null && parentView.GetType() == typeof(CreateNewTaskView))
            {
                var lst_item = lst_workRelated.FindAll(item => item.IsSelected);
                if (lst_item.Count > 0)
                {
                    CreateNewTaskView controller = (CreateNewTaskView)parentView;
                    controller.HandleAddWorkRelatedResult(lst_item);
                }
            }

            this.DismissModalViewController(true);
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
            { Console.WriteLine("FormAddWorkRelatedView - KeyBoardUpNotification - Err: " + ex.ToString()); }
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
            { Console.WriteLine("FormAddWorkRelatedView - KeyBoardDownNotification - Err: " + ex.ToString()); }
        }

        public void HandleSeclectItem(BeanNotify _notify, NSIndexPath _indexPath)
        {
            _notify.IsSelected = !_notify.IsSelected;
            table_workRelated.ReloadRows(new[] { _indexPath }, UITableViewRowAnimation.None);
        }

        public void HandleMenuOptionResult(ClassMenu _menu)
        {
            Custom_MenuOption custom_menuOption = Custom_MenuOption.Instance;
            if (_menu != null)
            {
                _menu.isSelected = true;

                if (custom_menuOption.BtnInputView == BT_workFlow)
                {
                    if (currentMenuWorkFlow != null && currentMenuWorkFlow.ID != _menu.ID)
                    {
                        currentMenuWorkFlow.isSelected = false;
                        currentMenuWorkFlow = _menu;
                    }
                    else
                        currentMenuWorkFlow = _menu;

                    BT_workFlow.SetTitle(currentMenuWorkFlow.title, UIControlState.Normal);
                }
                else
                {
                    if (currentMenuStatus != null && currentMenuStatus.ID != _menu.ID)
                    {
                        currentMenuStatus.isSelected = false;
                        currentMenuStatus = _menu;
                    }
                    else
                        currentMenuStatus = _menu;

                    BT_status.SetTitle(currentMenuStatus.title, UIControlState.Normal);
                }
            }

            if (custom_menuOption.Superview != null)
                custom_menuOption.RemoveFromSuperview();

            EnableInputView(true);
        }

        public void RemoveItemInList(BeanNotify _notify)
        {
            var index = lst_workRelated.FindIndex(item => item.ID == _notify.ID);
            if (index != -1)
            {
                lst_workRelated.RemoveAt(index);
                table_workRelated.ReloadData();
            }
        }

        #endregion

        #region custom views
        #region attachment source table
        private class WorkRelated_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellAttachmentID");
            FormAddWorkRelatedView parentView;
            List<BeanNotify> lst_workRelated { get; set; }
            public WorkRelated_TableSource(List<BeanNotify> _lst_workRelated, FormAddWorkRelatedView _parentview)
            {
                lst_workRelated = _lst_workRelated;
                parentView = _parentview;
            }

            public override nfloat GetHeightForFooter(UITableView tableView, nint section)
            {
                return -1;
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                return -1;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 76;
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return 1;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return lst_workRelated.Count;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                var itemSelected = lst_workRelated[indexPath.Row];
                parentView.HandleSeclectItem(itemSelected, indexPath);
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                //Custom_WorkRelatedCell cell = new Custom_WorkRelatedCell(cellIdentifier, parentView, null);
                //var notify = lst_workRelated[indexPath.Row];

                //bool isOdd = true;
                //if (indexPath.Row % 2 == 0)
                //    isOdd = false;

                //cell.UpdateCell(notify, false, isOdd);
                //return cell;
                return null;
            }
        }
        #endregion
        #endregion
    }
}
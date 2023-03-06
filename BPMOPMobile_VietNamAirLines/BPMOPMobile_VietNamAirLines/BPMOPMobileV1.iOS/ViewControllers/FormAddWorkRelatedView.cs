using System;
using System.Collections.Generic;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobileV1.iOS.CustomControlClass;
using BPMOPMobileV1.iOS.IOSClass;
using CoreGraphics;
using Foundation;
using SQLite;
using UIKit;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class FormAddWorkRelatedView : UIViewController
    {
        private NSObject _willResignActiveNotificationObserver;
        private NSObject _didBecomeActiveNotificationObserver;
        public List<ClassMenu> lst_menuItemTicket { get; set; }
        ClassMenu currentTicketSelected { get; set; }
        private List<ClassMenu> lst_menuItemStatus { get; set; }
        ClassMenu currentStatusSelected { get; set; }
        List<BeanNotify> lst_workRelated = new List<BeanNotify>();
        UIViewController parentView { get; set; }
        bool isFromDate = true;
        DateTime fromDateSelected;
        DateTime fromDate { get; set; }
        DateTime toDateSelected;
        DateTime toDate { get; set; }

        Custom_ListPickeritemView custom_ItemPicker = Custom_ListPickeritemView.Instance;
        nfloat height_picker;
        UIRefreshControl refreshControl;
        int limit = 20;
        int offset = 0;
        nfloat datepicker_height;
        Custom_CalendarView custom_CalendarView = Custom_CalendarView.Instance;
        AppDelegate appD = UIApplication.SharedApplication.Delegate as AppDelegate;

        public FormAddWorkRelatedView(IntPtr handle) : base(handle)
        {
        }

        #region override
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ViewConfiguration();
            LoadContent();
            LoadListQuytrinh();
            LoadListStatus();

            #region delegate
            BT_close.TouchUpInside += BT_close_TouchUpInside;
            BT_accept.TouchUpInside += BT_accept_TouchUpInside;
            datePicker.ValueChanged += DatePicker_ValueChanged;
            BT_fromdate.TouchUpInside += BT_fromdate_TouchUpInside;
            BT_todate.TouchUpInside += BT_todate_TouchUpInside;
            BT_quytrinh.TouchUpInside += BT_quytrinh_TouchUpInside;
            BT_tinhtrang.TouchUpInside += BT_tinhtrang_TouchUpInside;
            #endregion
            // Perform any additional setup after loading the view, typically from a nib.
        }

        #endregion

        #region private - public method
        public void SetContent(UIViewController _parentView)
        {
            parentView = _parentView;
        }

        private void ViewConfiguration()
        {

            custom_CalendarView.viewController = this;
            var Y_positionCalendar = view_filter.Frame.Y + BT_fromdate.Frame.Bottom;
            custom_CalendarView.InitFrameView(new CGRect(16 - 8, Y_positionCalendar + 5, this.View.Frame.Width - (32 - 16), 260));
            custom_CalendarView.BackgroundColor = UIColor.White;
            custom_CalendarView.Layer.BorderColor = UIColor.Purple.CGColor;
            custom_CalendarView.Layer.BorderWidth = 1;
            custom_CalendarView.Layer.CornerRadius = 10;

            datepicker_height = this.View.Bounds.Height / 3;
            datePicker.BackgroundColor = UIColor.White;
            datePicker.Layer.BorderColor = UIColor.LightGray.ColorWithAlpha(0.3f).CGColor;
            datePicker.Layer.BorderWidth = 0.2f;
            datePicker.Layer.ShadowOffset = new CGSize(1, -1);
            datePicker.Layer.ShadowRadius = 1;
            datePicker.Layer.ShadowOpacity = 0.3f;
            datePicker.Layer.CornerRadius = 10;

            height_picker = this.View.Bounds.Height / 2;
            custom_ItemPicker.InitFrameView(new CGRect(0, this.View.Frame.Bottom, this.View.Frame.Width, this.View.Frame.Height));
            custom_ItemPicker.Layer.ShadowPath = UIBezierPath.FromRect(new CGRect(0, -1, this.View.Frame.Width, this.View.Frame.Height)).CGPath;
            custom_ItemPicker.Layer.ShadowColor = UIColor.LightGray.ColorWithAlpha(1).CGColor;
            custom_ItemPicker.Layer.ShadowRadius = 2;
            custom_ItemPicker.Layer.ShadowOffset = new CGSize(0, 1);
            custom_ItemPicker.Layer.ShadowOpacity = 1f;
            custom_ItemPicker.Layer.CornerRadius = 10;
            custom_ItemPicker.Alpha = 0;

        }

        private void LoadContent()
        {
            var conn = new SQLiteConnection(CmmVariable.M_DataPath);

            BeanNotify notify = new BeanNotify() { ID = Guid.NewGuid(), Title = "Đề xuất phê duyệt tuyển dụng kế toán", Created = DateTime.Now.AddDays(-1), DueDate = DateTime.Now.AddDays(1), ListName = "Quy trình đăng kí tuyển dụng nhân sự", Note = "Chờ Phê Duyệt", EmailUpdate = "Thanh", Status = 0, RequestID = "ĐX-1517/2020" };
            BeanNotify notify1 = new BeanNotify() { ID = Guid.NewGuid(), Title = "Phê duyệt đề xuất đi công tác Hà Nội", Created = DateTime.Now.AddDays(-1), DueDate = DateTime.Now.AddDays(1), ListName = "Quy trình đề xuất đi công tác Hà Nội tháng 11", AssignedBy = "Bùi Thị B", Note = "Đã Phê Duyệt", Status = 1, RequestID = "TCHC-2004006" };
            BeanNotify notify2 = new BeanNotify() { ID = Guid.NewGuid(), Title = "Đề nghị tăng ngân sách công tác phí", Created = DateTime.Now.AddDays(-2), DueDate = DateTime.Now.AddDays(-1), ListName = "Quy trình đăng ký tăng công tác phí", AssignedBy = "Đỗ Văn Thừa", Note = "Bổ sung thông tin", EmailUpdate = "Khanh", Status = 2, RequestID = "RETY-6872020" };
            BeanNotify notify3 = new BeanNotify() { ID = Guid.NewGuid(), Title = "Yêu cầu đào tạo kỹ năng Software", Created = DateTime.Now.AddDays(-1), DueDate = DateTime.Now, ListName = "Quy trình đăng ký đào tạo kỹ năng Software", Note = "Từ Chối", EmailUpdate = "B", Status = 3, RequestID = "SYTG-157820" };
            BeanNotify notify4 = new BeanNotify() { ID = Guid.NewGuid(), Title = "Yêu cầu soạn hợp đồng chuyển khoản", Created = DateTime.Now.AddDays(-1), DueDate = DateTime.Now.AddDays(1), ListName = "Quy trình đăng ký soạn hợp đồng chuyển khoản", Note = "Chờ QCTT Phê Duyệt", EmailUpdate = "C", Status = 0, RequestID = "STFV-1347666" };
            BeanNotify notify5 = new BeanNotify() { ID = Guid.NewGuid(), Title = "Đề xuất phê duyệt tuyển dụng kế toán", Created = DateTime.Now.AddDays(-1), DueDate = DateTime.Now.AddDays(1), ListName = "Quy trình đăng kí tuyển dụng nhân sự", AssignedBy = "Thanh", Note = "Chờ Phê Duyệt", Status = 0, RequestID = "RETY-6872020" };
            BeanNotify notify6 = new BeanNotify() { ID = Guid.NewGuid(), Title = "Đề xuất phê duyệt tuyển dụng kế toán", Created = DateTime.Now.AddDays(-1), DueDate = DateTime.Now.AddDays(1), ListName = "Quy trình đăng kí tuyển dụng nhân sự", AssignedBy = "My", Note = "Chờ Phê Duyệt", Status = 0, RequestID = "SYTG-157820" };

            lst_workRelated.AddRange(new BeanNotify[] { notify, notify1, notify2, notify3, notify4, notify5, notify6 });


            if (lst_workRelated != null)
            {
                table_content.Source = new WorkFlow_TableSource(lst_workRelated, this);
                table_content.ReloadData();
            }
        }

        private void LoadListQuytrinh()
        {
            ClassMenu b1 = new ClassMenu { ID = 1, title = "Đăng ký làm ngoài giờ", section = -1, };
            ClassMenu b2 = new ClassMenu { ID = 2, title = "Thanh toán công tác phí", section = 0, };
            ClassMenu b3 = new ClassMenu { ID = 3, title = "Đề nghị tuyển dụng", section = 1 };
            ClassMenu b4 = new ClassMenu { ID = 4, title = "Đề nghị thăng cấp", section = 2 };
            ClassMenu b5 = new ClassMenu { ID = 5, title = "Quy trình đào tạo", section = 3 };
            ClassMenu b6 = new ClassMenu { ID = 6, title = "Quản cáo thương hiệu", section = 4 };
            ClassMenu b7 = new ClassMenu { ID = 7, title = "Nhận dạng sản phẩm", section = 5 };
            ClassMenu b8 = new ClassMenu { ID = 8, title = "Thuê Showroom", section = 6 };
            ClassMenu b9 = new ClassMenu { ID = 9, title = "Mở rộng thị trường", section = 7 };
            ClassMenu b10 = new ClassMenu { ID = 10, title = "Dịch vụ xã hội", section = 8 };

            lst_menuItemTicket = new List<ClassMenu>();
            lst_menuItemTicket.AddRange(new[] { b1, b2, b3, b4, b5, b6, b7, b8, b9, b10 });
            //custom_ItemPicker.lst_items = lst_menuItemTicket;
            //custom_ItemPicker.PickerLoadData();
        }

        private void LoadListStatus()
        {
            ClassMenu mStatus1 = new ClassMenu() { ID = 0, section = -1, title = "Tất cả", isSelected = true };
            ClassMenu mStatus2 = new ClassMenu() { ID = 1, section = 0, title = "Chờ phê duyệt" };
            ClassMenu mStatus3 = new ClassMenu() { ID = 2, section = 1, title = "Đã phê duyệt" };
            ClassMenu mStatus4 = new ClassMenu() { ID = 3, section = 2, title = "Không phê duyệt" };
            ClassMenu mStatus5 = new ClassMenu() { ID = 4, section = 3, title = "Hủy bỏ" };
            ClassMenu mStatus6 = new ClassMenu() { ID = 5, section = 4, title = "Trang thái khác" };

            lst_menuItemStatus = new List<ClassMenu>();
            lst_menuItemStatus.AddRange(new[] { mStatus1, mStatus1, mStatus2, mStatus3, mStatus4, mStatus5, mStatus6 });
            //custom_ItemPicker.lst_items = lst_menuItemStatus;
            //custom_ItemPicker.PickerLoadData();
        }

        private void toggleItemPicker()
        {
            if (custom_ItemPicker.Alpha == 0)
            {
                custom_ItemPicker.Frame = new CGRect(custom_ItemPicker.Frame.X, custom_ItemPicker.Frame.Y, custom_ItemPicker.Frame.Width, height_picker);
                custom_ItemPicker.Alpha = 0;
                UIView.BeginAnimations("toogle_docmenu_slideShow_show");
                UIView.SetAnimationDuration(0.4f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);
                custom_ItemPicker.Alpha = 1;
                custom_ItemPicker.InitFrameView(new CGRect(custom_ItemPicker.Frame.X, this.View.Frame.Height - custom_ItemPicker.Frame.Height, custom_ItemPicker.Frame.Width, height_picker));
                UIView.CommitAnimations();
            }
            else
            {
                BT_quytrinh.Layer.BorderWidth = 0;
                BT_tinhtrang.Layer.BorderWidth = 0;
                custom_ItemPicker.Frame = new CGRect(custom_ItemPicker.Frame.X, this.View.Frame.Height - custom_ItemPicker.Frame.Height, custom_ItemPicker.Frame.Width, height_picker);
                custom_ItemPicker.Alpha = 1;
                UIView.BeginAnimations("toogle_docmenu_slideShow_collapse");
                UIView.SetAnimationDuration(0.4f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);
                custom_ItemPicker.InitFrameView(new CGRect(custom_ItemPicker.Frame.X, this.View.Frame.Bottom, custom_ItemPicker.Frame.Width, height_picker));
                custom_ItemPicker.Alpha = 0;

                UIView.CommitAnimations();
                custom_ItemPicker.RemoveFromSuperview();
            }
        }

        public void HandlePickerResult(ClassMenu _item)
        {
            Custom_ListPickeritemView custom_menuOption = Custom_ListPickeritemView.Instance;
            if (_item != null)
            {
                _item.isSelected = true;

                if (custom_menuOption.inputView == tf_quytrinh)
                {
                    if (currentTicketSelected != null && currentTicketSelected.ID != _item.ID)
                    {
                        currentTicketSelected.isSelected = false;
                        currentTicketSelected = _item;
                    }
                    else
                        currentTicketSelected = _item;

                    tf_quytrinh.Text = currentTicketSelected.title;
                }
                else
                {
                    if (currentStatusSelected != null && currentStatusSelected.ID != _item.ID)
                    {
                        currentStatusSelected.isSelected = false;
                        currentStatusSelected = _item;
                    }
                    else
                        currentStatusSelected = _item;

                    tf_tinhtrang.Text = currentStatusSelected.title;
                }
            }

            custom_ItemPicker.Alpha = 0;
            custom_ItemPicker.RemoveFromSuperview();

            BT_quytrinh.Layer.BorderWidth = 0;
            BT_tinhtrang.Layer.BorderWidth = 0;

        }

        private void RelateWorkFlowSelected(BeanNotify _notify, NSIndexPath _indexPath)
        {
            _notify.IsSelected = !_notify.IsSelected;
            table_content.ReloadRows(new[] { _indexPath }, UITableViewRowAnimation.None);
        }
        #endregion

        #region events
        private void BT_fromdate_TouchUpInside(object sender, EventArgs e)
        {
            isFromDate = true;
            BT_fromdate.Layer.BorderColor = UIColor.Purple.CGColor;
            BT_fromdate.Layer.BorderWidth = 1;

            BT_todate.Layer.BorderColor = UIColor.Purple.CGColor;
            BT_todate.Layer.BorderWidth = 0;
            BT_quytrinh.Layer.BorderColor = UIColor.Purple.CGColor;
            BT_quytrinh.Layer.BorderWidth = 0;
            BT_tinhtrang.Layer.BorderColor = UIColor.Purple.CGColor;
            BT_tinhtrang.Layer.BorderWidth = 0;

            if (custom_CalendarView.Superview != null && custom_CalendarView.inputView == tf_fromdate)
            {
                custom_CalendarView.RemoveFromSuperview();
            }
            else
            {
                custom_CalendarView.inputView = tf_fromdate;
                custom_CalendarView.SetUpDate();
                this.View.AddSubview(custom_CalendarView);
            }

            if (custom_ItemPicker.Superview != null)
            {
                custom_ItemPicker.RemoveFromSuperview();
            }
            //ToggleCalendar_FromMe();
        }

        private void BT_todate_TouchUpInside(object sender, EventArgs e)
        {
            isFromDate = false;
            BT_todate.Layer.BorderColor = UIColor.Purple.CGColor;
            BT_todate.Layer.BorderWidth = 1;

            BT_fromdate.Layer.BorderColor = UIColor.Purple.CGColor;
            BT_fromdate.Layer.BorderWidth = 0;
            BT_quytrinh.Layer.BorderColor = UIColor.Purple.CGColor;
            BT_quytrinh.Layer.BorderWidth = 0;
            BT_tinhtrang.Layer.BorderColor = UIColor.Purple.CGColor;
            BT_tinhtrang.Layer.BorderWidth = 0;
            
            if (custom_CalendarView.Superview != null && custom_CalendarView.inputView == tf_todate)
            {
                custom_CalendarView.RemoveFromSuperview();
            }
            else
            {
                custom_CalendarView.inputView = tf_todate;
                custom_CalendarView.SetUpDate();
                this.View.AddSubview(custom_CalendarView);
            }

            if (custom_ItemPicker.Superview != null)
            {
                custom_ItemPicker.RemoveFromSuperview();
            }
            //ToggleCalendar_FromMe();
        }

        private void DatePicker_ValueChanged(object sender, EventArgs e)
        {
            DateTime reference = new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var _date = reference.AddSeconds((sender as UIDatePicker).Date.SecondsSinceReferenceDate);
            var dateTime = _date.ToLocalTime().ToString("dd/MM/yyyy");

            if (isFromDate)
            {
                tf_fromdate.Text = dateTime;
                fromDate = _date.ToLocalTime();
            }
            else
            {
                tf_todate.Text = dateTime;
                toDate = _date.ToLocalTime();
            }
        }

        private void BT_quytrinh_TouchUpInside(object sender, EventArgs e)
        {
            BT_quytrinh.Layer.BorderColor = UIColor.Purple.CGColor;
            BT_quytrinh.Layer.BorderWidth = 1;

            BT_fromdate.Layer.BorderColor = UIColor.Purple.CGColor;
            BT_fromdate.Layer.BorderWidth = 0;
            BT_todate.Layer.BorderColor = UIColor.Purple.CGColor;
            BT_todate.Layer.BorderWidth = 0;
            BT_tinhtrang.Layer.BorderColor = UIColor.Purple.CGColor;
            custom_CalendarView.Layer.BorderWidth = 0;

            if (custom_ItemPicker.Superview == null)
            {
                custom_ItemPicker.viewController = this;
                custom_ItemPicker.inputView = tf_quytrinh;
                custom_ItemPicker.lst_items = lst_menuItemTicket;
                custom_ItemPicker.PickerLoadData();
                this.View.AddSubview(custom_ItemPicker);
            }
            toggleItemPicker();
        }

        private void BT_tinhtrang_TouchUpInside(object sender, EventArgs e)
        {
            BT_tinhtrang.Layer.BorderColor = UIColor.Purple.CGColor;
            BT_tinhtrang.Layer.BorderWidth = 1;

            BT_quytrinh.Layer.BorderColor = UIColor.Purple.CGColor;
            BT_quytrinh.Layer.BorderWidth = 0;
            BT_fromdate.Layer.BorderColor = UIColor.Purple.CGColor;
            BT_fromdate.Layer.BorderWidth = 0;
            BT_todate.Layer.BorderColor = UIColor.Purple.CGColor;
            BT_todate.Layer.BorderWidth = 0;

            if (custom_ItemPicker.Superview == null)
            {
                custom_ItemPicker.viewController = this;
                custom_ItemPicker.inputView = tf_tinhtrang;
                custom_ItemPicker.lst_items = lst_menuItemStatus;
                custom_ItemPicker.PickerLoadData();
                this.View.AddSubview(custom_ItemPicker);
            }
            toggleItemPicker();
        }

        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            if (this.NavigationController != null)
                this.NavigationController.PopViewController(true);
            else
                this.DismissModalViewController(true);
        }

        private void BT_accept_TouchUpInside(object sender, EventArgs e)
        {

            if (parentView != null && parentView.GetType() == typeof(CreateTicketFormView))
            {
                var lst_item = lst_workRelated.FindAll(item => item.IsSelected);
                if (lst_item.Count > 0)
                {
                    CreateTicketFormView controller = (CreateTicketFormView)parentView;
                    controller.HandleAddWorkRelatedResult(lst_item);
                }
            }

            this.DismissModalViewController(true);


        }
        #endregion

        #region custom class
        #region WorkFlowItem data source table
        private class WorkFlow_TableSource : UITableViewSource
        {
            List<BeanNotify> lst_noti;
            public static Dictionary<string, List<BeanWorkflowItem>> indexedCateSession;
            List<string> sectionKeys;
            List<bool> sectionState;
            NSString cellIdentifier = new NSString("cell");
            FormAddWorkRelatedView parentView;

            string query = "";
            int limit = 20;
            bool isLoadMore = true;

            public WorkFlow_TableSource(List<BeanNotify> _relateNoti, FormAddWorkRelatedView _parentview)
            {
                lst_noti = _relateNoti;
                parentView = _parentview;
            }

            
            public override nint NumberOfSections(UITableView tableView)
            {
                return 1;
            }
            public override nfloat GetHeightForFooter(UITableView tableView, nint section)
            {
                return -1;
            }
            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                return -1;
            }
            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return lst_noti.Count;
            }
            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 100;
            }          
            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                parentView.RelateWorkFlowSelected(lst_noti[indexPath.Row], indexPath);
                tableView.DeselectRow(indexPath, true);
            }
            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var todo = lst_noti[indexPath.Row];
                bool isOdd = true;
                if (indexPath.Row % 2 == 0)
                    isOdd = false;

                Custom_WorkRelatedCell cell = new Custom_WorkRelatedCell(cellIdentifier);
                //cell.UpdateCell(todo, false, isOdd);
                return cell;
            }
            
        }
        #endregion
        #endregion
    }
}


using System;
using System.Globalization;
using System.Threading;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using TelerikUI;
using UIKit;

namespace BPMOPMobile.iPad.ViewControllers
{
    public partial class FormDateTimeController : UIViewController
    {
        TKCalendar calendarView;
        TKCalendarMonthPresenter presenter;
        CalendarDelegate calendarDelegate;
        UIViewController parentView { get; set; }
        ViewElement element { get; set; }
        DateTime dateSelected;
        string textDate;
        UIDatePicker uIDatePicker = new UIDatePicker();

        public FormDateTimeController(IntPtr handle) : base(handle)
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

            if (element != null)
                loadContent_byElement();
            else
                loadContent_byText();

            #region delegate
            BT_close.TouchUpInside += BT_close_TouchUpInside;
            BT_currentDate.TouchUpInside += BT_currentDate_TouchUpInside;
            BT_clear.TouchUpInside += BT_clear_TouchUpInside;
            BT_accept.TouchUpInside += BT_accept_TouchUpInside;
            #endregion
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            //if (element.DataType == "datetime") uIDatePicker.Frame = view_control.Bounds;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (element.DataType == "datetime") uIDatePicker.Frame = view_control.Bounds;
        }

        #endregion

        #region private - public method

        public void setContent(UIViewController _parentView, ViewElement _element, string _textDate)
        {
            element = _element;
            parentView = _parentView;
            textDate = _textDate;
        }

        private void ViewConfiguration()
        {
            BT_accept.SetTitle(CmmFunction.GetTitle("TEXT_AGREE", "Đồng ý"), UIControlState.Normal);
            BT_accept.Hidden = true;
            BT_clear.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_currentDate.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_close.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);
        }

        private void loadContent_byElement()
        {
            try
            {
                // Change current culture
                CultureInfo culture;
                culture = CultureInfo.CreateSpecificCulture("en-US");
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;

                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
                //if (!string.IsNullOrEmpty(element.Value))
                //    dateSelected = DateTime.Parse(element.Value);
                //else// khong co gia tri datetime, gan mac dinh gia tri ngay gio hien tai 
                //    dateSelected = DateTime.Now;

                if (!string.IsNullOrEmpty(element.Value))
                {
                    if (parentView.GetType() == typeof(FormCreateTaskView) || parentView.GetType() == typeof(FormTaskDetails))
                    {
                        if (CmmVariable.SysConfig.LangCode.Equals("1033"))
                            dateSelected = DateTime.ParseExact(element.Value, "MM/dd/yy HH:mm", null);
                        else
                            dateSelected = DateTime.ParseExact(element.Value, "dd/MM/yy HH:mm", null);
                    }
                    else
                        dateSelected = DateTime.Parse(element.Value);
                }
                else// khong co gia tri datetime, gan mac dinh gia tri ngay gio hien tai 
                    dateSelected = DateTime.Now;

                if (element.DataType == "date")
                {
                    lbl_title.Text = dateSelected.ToString("dd/MM/yy");

                    calendarView = new TKCalendar(view_control.Bounds);
                    calendarDelegate = new CalendarDelegate(this);
                    calendarView.Layer.CornerRadius = 10;
                    calendarView.Locale = new NSLocale("vi_VI");
                    calendarView.SelectionMode = TKCalendarSelectionMode.Single;
                    calendarView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                    calendarView.Delegate = calendarDelegate;

                    NSDate date = NSDate.Now;
                    NSDateComponents components = new NSDateComponents();
                    components.Year = -1;
                    calendarView.MinDate = calendarView.Calendar.DateByAddingComponents(components, date, NSCalendarOptions.None);
                    components.Year = 1;
                    calendarView.MaxDate = calendarView.Calendar.DateByAddingComponents(components, date, NSCalendarOptions.None);

                    presenter = (TKCalendarMonthPresenter)calendarView.Presenter;
                    presenter.Style.BackgroundColor = UIColor.White;
                    presenter.Update(true);

                    view_control.AddSubview(calendarView);

                    calendarView.ReloadData();
                    calendarView.SelectedDate = CmmIOSFunction.DateTimeToNSDate(dateSelected);
                    calendarView.NavigateToDate(calendarView.SelectedDate, false);
                }
                else if (element.DataType == "datetime")
                {
                    lbl_title.Text = dateSelected.ToString("dd/MM/yy HH:mm");
                    BT_accept.Hidden = false;

                    //if (UIDevice.CurrentDevice.CheckSystemVersion(13, 4))
                    // uIDatePicker.PreferredDatePickerStyle = UIDatePickerStyle.Wheels;

                    //uIDatePicker.Frame = view_control.Bounds;
                    if (UIDevice.CurrentDevice.CheckSystemVersion(14, 0))
                        uIDatePicker.PreferredDatePickerStyle = UIDatePickerStyle.Wheels;

                    uIDatePicker.SetDate(CmmIOSFunction.DateTimeToNSDate(dateSelected), true);
                    uIDatePicker.ValueChanged += UIDatePicker_ValueChanged;

                    view_control.AddSubview(uIDatePicker);
                    //uIDatePicker.TranslatesAutoresizingMaskIntoConstraints = false;
                    //uIDatePicker.LeadingAnchor.ConstraintEqualTo(view_control.LeadingAnchor).Active = true;
                    //uIDatePicker.TopAnchor.ConstraintEqualTo(view_control.TopAnchor).Active = true;
                    //uIDatePicker.TrailingAnchor.ConstraintEqualTo(view_control.TrailingAnchor).Active = true;
                    //uIDatePicker.BottomAnchor.ConstraintEqualTo(view_control.BottomAnchor).Active = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("FormCalendarChoice - loadContent - ERR: " + ex.ToString());
            }
        }

        private void loadContent_byText()
        {
            try
            {
                if (!string.IsNullOrEmpty(textDate))
                    dateSelected = DateTime.Parse(element.Value);
                else// khong co gia tri datetime, gan mac dinh gia tri ngay gio hien tai 
                    dateSelected = DateTime.Now;


                lbl_title.Text = dateSelected.ToString("dd/MM/yy");

                calendarView = new TKCalendar(new CGRect(0, 60, view_content.Frame.Width, view_content.Frame.Height - 60));
                calendarDelegate = new CalendarDelegate(this);

                calendarView.Layer.CornerRadius = 10;
                calendarView.Locale = new NSLocale("vi_VI");
                calendarView.SelectionMode = TKCalendarSelectionMode.Single;
                calendarView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                calendarView.Delegate = calendarDelegate;

                NSDate date = NSDate.Now;
                NSDateComponents components = new NSDateComponents();
                components.Year = -1;
                calendarView.MinDate = calendarView.Calendar.DateByAddingComponents(components, date, NSCalendarOptions.None);
                components.Year = 1;
                calendarView.MaxDate = calendarView.Calendar.DateByAddingComponents(components, date, NSCalendarOptions.None);

                presenter = (TKCalendarMonthPresenter)calendarView.Presenter;
                presenter.Style.BackgroundColor = UIColor.White;
                presenter.Update(true);

                view_content.AddSubview(calendarView);

                calendarView.ReloadData();
                calendarView.SelectedDate = CmmIOSFunction.DateTimeToNSDate(dateSelected);
                calendarView.NavigateToDate(calendarView.SelectedDate, false);

            }
            catch (Exception ex)
            {
                Console.WriteLine("FormCalendarChoice - loadContent - ERR: " + ex.ToString());
            }
        }

        private void CalendarDateSelected(DateTime _dateSelected)
        {
            dateSelected = _dateSelected;
            //if (CmmVariable.SysConfig.LangCode == "1066") // VN
            //{
            //    if (element.DataType == "date")
            //        lbl_title.Text = dateSelected.ToString(CmmVariable.M_WorkDateFormatDateVN);
            //    else if (element.DataType == "datetime")
            //        lbl_title.Text = dateSelected.ToString(CmmVariable.M_WorkDateFormatDateTimeVN);
            //}
            //else if(CmmVariable.SysConfig.LangCode == "1033") // EN
            //{
            //    if (element.DataType == "date")
            //        lbl_title.Text = dateSelected.ToString(CmmVariable.M_WorkDateFormatDayEN);
            //    else if (element.DataType == "datetime")
            //        lbl_title.Text = dateSelected.ToString(CmmVariable.M_WorkDateFormatDateTimeEN);
            //}

            element.Value = dateSelected.ToString();

            if (parentView.GetType() == typeof(ToDoDetailView))
            {
                ToDoDetailView toDo = parentView as ToDoDetailView;
                toDo.HandleDateTimeChoiceChoice(element);
            }
            else if (parentView.GetType() == typeof(WorkflowDetailView))
            {
                WorkflowDetailView workflowDetailView = parentView as WorkflowDetailView;
                workflowDetailView.HandleDateTimeChoiceChoice(element);
            }
            else if (parentView.GetType() == typeof(FormTaskDetails))
            {
                FormTaskDetails formTaskDetails = parentView as FormTaskDetails;
                formTaskDetails.HandleDateTimeChoiceChoice(element);
            }
            else if (parentView.GetType() == typeof(FormWFDetailsProperty))
            {
                FormWFDetailsProperty formWFDetailsProperty = parentView as FormWFDetailsProperty;
                formWFDetailsProperty.HandleDateTimeChoiceChoice(element);
            }
            else if (parentView.GetType() == typeof(FollowListViewController))
            {
                FollowListViewController followListViewController = parentView as FollowListViewController;
                followListViewController.HandleDateTimeChoiceChoice(element);
            }

            this.DismissModalViewController(true);

        }
        #endregion

        #region events
        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            this.DismissModalViewController(true);
        }

        private void BT_accept_TouchUpInside(object sender, EventArgs e)
        {
            if (parentView.GetType() == typeof(ToDoDetailView))
            {
                element.Value = dateSelected.ToString();
                ToDoDetailView toDo = parentView as ToDoDetailView;
                toDo.HandleDateTimeChoiceChoice(element);

                this.DismissModalViewController(true);
            }
            else if (parentView.GetType() == typeof(WorkflowDetailView))
            {
                element.Value = dateSelected.ToString();
                WorkflowDetailView workflowDetailView = parentView as WorkflowDetailView;
                workflowDetailView.HandleDateTimeChoiceChoice(element);

                this.DismissModalViewController(true);
            }
            else if (parentView.GetType() == typeof(FormWorkFlowDetails))
            {
                element.Value = dateSelected.ToString();
                FormWorkFlowDetails formWorkFlowDetails = parentView as FormWorkFlowDetails;
                formWorkFlowDetails.HandleDateTimeChoiceChoice(element);

                this.DismissModalViewController(true);
            }
            else if (parentView.GetType() == typeof(FormWFDetailsProperty))
            {
                element.Value = dateSelected.ToString();
                FormWFDetailsProperty formWFDetailsProperty = parentView as FormWFDetailsProperty;
                formWFDetailsProperty.HandleDateTimeChoiceChoice(element);

                this.DismissModalViewController(true);
            }
            else if (parentView.GetType() == typeof(FormTaskDetails))
            {
                element.Value = dateSelected.ToString();
                FormTaskDetails formTaskDetails = parentView as FormTaskDetails;
                formTaskDetails.HandleDateTimeChoiceChoice(element);

                this.DismissModalViewController(true);
            }
            else if (parentView.GetType() == typeof(FollowListViewController))
            {
                element.Value = dateSelected.ToString();
                FollowListViewController followListViewController = parentView as FollowListViewController;
                followListViewController.HandleDateTimeChoiceChoice(element);

                this.DismissModalViewController(true);
            }
        }

        private void BT_clear_TouchUpInside(object sender, EventArgs e)
        {
            element.Value = "";

            if (parentView.GetType() == typeof(ToDoDetailView))
            {
                ToDoDetailView toDo = parentView as ToDoDetailView;
                toDo.HandleDateTimeChoiceChoice(element);
            }
            else if (parentView.GetType() == typeof(WorkflowDetailView))
            {
                WorkflowDetailView workflowDetailView = parentView as WorkflowDetailView;
                workflowDetailView.HandleDateTimeChoiceChoice(element);
            }
            else if (parentView.GetType() == typeof(FormWFDetailsProperty))
            {
                FormWFDetailsProperty formWFDetailsProperty = parentView as FormWFDetailsProperty;
                formWFDetailsProperty.HandleDateTimeChoiceChoice(element);
            }
            else if (parentView.GetType() == typeof(FormTaskDetails))
            {
                FormTaskDetails formTaskDetails = parentView as FormTaskDetails;
                formTaskDetails.HandleDateTimeChoiceChoice(element);
            }
            else if (parentView.GetType() == typeof(FollowListViewController))
            {
                FollowListViewController followListViewController = parentView as FollowListViewController;
                followListViewController.HandleDateTimeChoiceChoice(element);
            }

            this.DismissViewControllerAsync(true);
        }
        private void BT_currentDate_TouchUpInside(object sender, EventArgs e)
        {
            if (element.DataType == "date")
            {
                calendarView.SelectedDate = CmmIOSFunction.DateTimeToNSDate(DateTime.Now);
                dateSelected = DateTime.Now.Date;
            }
            else if (element.DataType == "datetime")
            {
                uIDatePicker.SetDate(CmmIOSFunction.DateTimeToNSDate(DateTime.Now), true);
                dateSelected = DateTime.Now;
            }
        }
        private void UIDatePicker_ValueChanged(object sender, EventArgs e)
        {
            DateTime reference = new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateSelected = reference.AddSeconds((sender as UIDatePicker).Date.SecondsSinceReferenceDate);
        }
        #endregion

        #region cust class
        class CalendarDelegate : TKCalendarDelegate
        {
            FormDateTimeController parentView;

            public CalendarDelegate(FormDateTimeController _parent)
            {
                this.parentView = _parent;
            }

            public override void UpdateVisualsForCell(TKCalendar calendar, TKCalendarCell cell)
            {
                cell.Style.TextFont = UIFont.SystemFontOfSize(14);
                cell.Style.ShapeStroke = new TKStroke(UIColor.FromRGB(65, 84, 134));
                cell.Style.ShapeFill = new TKSolidFill(UIColor.FromRGB(65, 84, 134));

                if (cell is TKCalendarDayCell)
                {
                    TKCalendarDayCell dayCell = (TKCalendarDayCell)cell;
                    if ((dayCell.State & TKCalendarDayState.Selected) != 0)
                        cell.Style.TextColor = UIColor.White;
                    else
                        cell.Style.TextColor = UIColor.Black;
                }
            }

            public override void DidSelectDate(TKCalendar calendar, NSDate date)
            {
                try
                {
                    var value = date.ToString();
                    DateTime dateValue = DateTime.Parse(value, new CultureInfo("vi", false));
                    parentView.CalendarDateSelected(dateValue);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Custom_CalendarView - DidSelectDate - ERR INDEX: " + ex.ToString());
                }
            }
        }
        #endregion
    }
}


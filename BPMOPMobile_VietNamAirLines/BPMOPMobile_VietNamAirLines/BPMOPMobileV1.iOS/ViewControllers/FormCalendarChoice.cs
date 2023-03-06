using System;
using System.Drawing;
using System.Globalization;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobileV1.iOS.IOSClass;
using CoreGraphics;
using Foundation;
using TelerikUI;
using UIKit;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class FormCalendarChoice : UIViewController
    {
        TKCalendar calendarView;
        UIDatePicker uIDatePicker;
        TKCalendarMonthPresenter presenter;
        CalendarDelegate calendarDelegate;
        UIViewController parentView { get; set; }
        ViewElement element { get; set; }
        DateTime dateSelected;


        public FormCalendarChoice(IntPtr handle) : base(handle)
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
            SetLangTitle();
            loadContent();

            #region delegate
            BT_close.TouchUpInside += BT_close_TouchUpInside;
            BT_accept.TouchUpInside += BT_accept_TouchUpInside;
            BT_clear.TouchUpInside += BT_clear_TouchUpInside;
            BT_current.TouchUpInside += BT_current_TouchUpInside;
            #endregion
        }

        #endregion

        #region private - public method

        public void setContent(UIViewController _parentView, ViewElement _element)
        {
            element = _element;
            parentView = _parentView;
        }

        private void ViewConfiguration()
        {
            BT_current.ImageEdgeInsets = new UIEdgeInsets(1, 1, 1, 1);
            BT_clear.ImageEdgeInsets = new UIEdgeInsets(1, 1, 1, 1);
            lbl_title.TextColor = UIColor.FromRGB(0, 95, 212);
        }
        private void SetLangTitle()
        {
            CmmIOSFunction.SetLangToView(this.View);
        }
        private void loadContent()
        {
            try
            {
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
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        lbl_title.Text = dateSelected.ToString("MM/dd/yy");
                    else
                        lbl_title.Text = dateSelected.ToString("dd/MM/yy");

                    calendarView = new TKCalendar(new CGRect(0, 60, view_content.Frame.Width, view_content.Frame.Height - 60));
                    calendarDelegate = new CalendarDelegate(this);

                    calendarView.Layer.CornerRadius = 10;
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        calendarView.Locale = new NSLocale("en_US");
                    else
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
                else if (element.DataType == "datetime")
                {
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        lbl_title.Text = dateSelected.ToString("MM/dd/yy HH:mm");
                    else
                        lbl_title.Text = dateSelected.ToString("dd/MM/yy HH:mm");

                    uIDatePicker = new UIDatePicker();
                    if (UIDevice.CurrentDevice.CheckSystemVersion(14, 0))
                        uIDatePicker.PreferredDatePickerStyle = UIDatePickerStyle.Wheels;

                    //if (CmmVariable.SysConfig.LangCode == "1066")
                    //    uIDatePicker.Locale = new NSLocale("vi_VI");
                    //else
                    //    uIDatePicker.Locale = new NSLocale("en_US");

                    if (CmmVariable.SysConfig.LangCode == "1033")
                        uIDatePicker.Locale = new NSLocale("en_gb");
                    else
                        uIDatePicker.Locale = new NSLocale("vi");

                    uIDatePicker.Frame = new CGRect(0, 60, view_content.Frame.Width, view_content.Frame.Height - 60);
                    uIDatePicker.SetDate(CmmIOSFunction.DateTimeToNSDate(dateSelected), true);
                    uIDatePicker.ValueChanged += UIDatePicker_ValueChanged;

                    view_content.AddSubview(uIDatePicker);

                    view_content.AddSubview(uIDatePicker);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("FormCalendarChoice - loadContent - ERR: " + ex.ToString());
            }
        }

        private void CalendarDateSelected(DateTime _dateSelected)
        {
            dateSelected = _dateSelected;
            if (element.DataType == "date")
            {
                if (CmmVariable.SysConfig.LangCode == "1033")
                    lbl_title.Text = dateSelected.ToString("MM/dd/yy");
                else
                    lbl_title.Text = dateSelected.ToString("dd/MM/yy");
            }
            else if (element.DataType == "datetime")
            {
                if (CmmVariable.SysConfig.LangCode == "1033")
                    lbl_title.Text = dateSelected.ToString("MM/dd/yy HH:mm");
                else
                    lbl_title.Text = dateSelected.ToString("dd/MM/yy HH:mm");
            }
        }
        #endregion

        #region events
        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            BT_close.Hidden = true;
            this.DismissModalViewController(true);
        }
        private void BT_accept_TouchUpInside(object sender, EventArgs e)
        {
            BT_close.Hidden = true;
            if (parentView.GetType() == typeof(RequestDetailsV2))
            {
                if (CmmVariable.SysConfig.LangCode == "1033")
                    element.Value = dateSelected.ToString();
                else //if (CmmVariable.SysConfig.LangCode == "1066")
                    element.Value = dateSelected.ToString();

                RequestDetailsV2 requestDetailsV2 = parentView as RequestDetailsV2;
                requestDetailsV2.HandleDateTimeChoiceChoice(element);

                this.DismissModalViewController(true);
            }
            if (parentView.GetType() == typeof(FormWFDetailsProperty))
            {
                if (CmmVariable.SysConfig.LangCode == "1033")
                    element.Value = dateSelected.ToString();
                else //if (CmmVariable.SysConfig.LangCode == "1066")
                    element.Value = dateSelected.ToString();

                FormWFDetailsProperty formWFDetailsProperty = parentView as FormWFDetailsProperty;
                formWFDetailsProperty.HandleDateTimeChoiceChoice(element);

                this.DismissModalViewController(true);
            }
            if (parentView.GetType() == typeof(FormCreateTaskView))//view nay khong hien thi am/pm
            {
                //if (CmmVariable.SysConfig.LangCode == "1066")
                //    element.Value = dateSelected.ToString();
                //else if (CmmVariable.SysConfig.LangCode == "1033")
                //    element.Value = dateSelected.ToString();

                //if (dateSelected > DateTime.Now)
                //{
                if (CmmVariable.SysConfig.LangCode == "1033")
                    element.Value = dateSelected.ToString(CmmVariable.M_WorkDateFormatDateTimeEN);
                else
                    element.Value = dateSelected.ToString(CmmVariable.M_WorkDateFormatDateTimeVN);

                FormCreateTaskView formCreateTaskView = parentView as FormCreateTaskView;
                formCreateTaskView.HandleDateTimeChoiceChoice(element);

                this.DismissModalViewController(true);
                //}
                //else
                //{
                //    CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetAppSettingValue("TEXT_DATE_COMPARE2"),);
                //}
            }
            if (parentView.GetType() == typeof(FormTaskDetails)) //view nay khong hien thi am/pm
            {
                //if (CmmVariable.SysConfig.LangCode == "1066")
                //    element.Value = dateSelected.ToString();
                //else if (CmmVariable.SysConfig.LangCode == "1033")
                //    element.Value = dateSelected.ToString();
                if (CmmVariable.SysConfig.LangCode == "1033")
                    element.Value = dateSelected.ToString(CmmVariable.M_WorkDateFormatDateTimeEN);
                else
                    element.Value = dateSelected.ToString(CmmVariable.M_WorkDateFormatDateTimeVN);

                FormTaskDetails formTaskDetails = parentView as FormTaskDetails;
                formTaskDetails.HandleDateTimeChoiceChoice(element);

                this.DismissModalViewController(true);
            }
        }
        private void UIDatePicker_ValueChanged(object sender, EventArgs e)
        {
            DateTime reference = new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateSelected = reference.AddSeconds((sender as UIDatePicker).Date.SecondsSinceReferenceDate);
            //string dt_str = dateSelected.ToLocalTime().ToString("dd/MM/yyyy");
            CalendarDateSelected(dateSelected.ToLocalTime());
        }
        private void BT_clear_TouchUpInside(object sender, EventArgs e)
        {
            element.Value = "";
            BT_close.Hidden = true;
            if (parentView.GetType() == typeof(RequestDetailsV2))
            {
                RequestDetailsV2 requestDetailsV2 = parentView as RequestDetailsV2;
                requestDetailsV2.HandleDateTimeChoiceChoice(element);

            }
            if (parentView.GetType() == typeof(FormWFDetailsProperty))
            {
                FormWFDetailsProperty formWFDetailsProperty = parentView as FormWFDetailsProperty;
                formWFDetailsProperty.HandleDateTimeChoiceChoice(element);

            }
            if (parentView.GetType() == typeof(FormCreateTaskView))
            {
                FormCreateTaskView formCreateTaskView = parentView as FormCreateTaskView;
                formCreateTaskView.HandleDateTimeChoiceChoice(element);

            }
            if (parentView.GetType() == typeof(FormTaskDetails))
            {
                FormTaskDetails formTaskDetails = parentView as FormTaskDetails;
                formTaskDetails.HandleDateTimeChoiceChoice(element);

            }
            this.DismissModalViewController(true);
        }
        private void BT_current_TouchUpInside(object sender, EventArgs e)
        {
            dateSelected = DateTime.Now;
            if (element.DataType == "date")
                calendarView.SelectedDate = CmmIOSFunction.DateTimeToNSDate(dateSelected);
            else if (element.DataType == "datetime")
                uIDatePicker.SetDate(CmmIOSFunction.DateTimeToNSDate(dateSelected), true);
            CalendarDateSelected(dateSelected);
        }
        #endregion

        #region cust class
        class CalendarDelegate : TKCalendarDelegate
        {
            FormCalendarChoice parentView;

            public CalendarDelegate(FormCalendarChoice _parent)
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


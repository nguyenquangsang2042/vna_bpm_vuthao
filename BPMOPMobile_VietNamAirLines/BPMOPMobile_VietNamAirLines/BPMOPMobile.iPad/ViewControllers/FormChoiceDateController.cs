using System;
using System.Globalization;
using BPMOPMobile.Class;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using TelerikUI;
using UIKit;

namespace BPMOPMobile.iPad.ViewControllers
{
    public partial class FormChoiceDateController : UIViewController
    {
        TKCalendar calendarView;
        TKCalendarMonthPresenter presenter;
        CalendarDelegate calendarDelegate;
        UIViewController parentView { get; set; }
        DateTime dateSelected;
        UILabel lbl_date { get; set; }
        string title;

        public FormChoiceDateController(IntPtr handle) : base(handle)
        {
        }

        #region override private - public method
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.PreferredContentSize = new CGSize(510, 510);
            ViewConfiguration();
            loadContent();

            #region delegate
            BT_close.TouchUpInside += BT_close_TouchUpInside;
            BT_currentDate.TouchUpInside += BT_currentDate_TouchUpInside;
            BT_clear.TouchUpInside += BT_clear_TouchUpInside;
            #endregion

        }

        #endregion

        #region private - public method

        public void setContent(UIViewController _parentView, string _title, UILabel _lbl_date)
        {
            parentView = _parentView;
            lbl_date = _lbl_date;
            title = _title;
        }

        private void ViewConfiguration()
        {
            BT_clear.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_currentDate.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_close.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);
        }

        private void loadContent()
        {
            try
            {
                lbl_title.Text = title;
                if (!string.IsNullOrEmpty(lbl_date.Text) && lbl_date.Text.Contains('/'))
                {
                    try
                    {
                        if (CmmVariable.SysConfig.LangCode == "1033")
                            dateSelected = DateTime.ParseExact(lbl_date.Text, "MM/dd/yyyy", new CultureInfo("en", false));
                        else //if (CmmVariable.SysConfig.LangCode == "1066")
                            dateSelected = DateTime.ParseExact(lbl_date.Text, "dd/MM/yyyy", new CultureInfo("vi", false));


                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine("Custom_CalendarView - SetUpDate " + ex);
                    }
                }
                else
                {
                    // khong co gia tri datetime, gan mac dinh gia tri ngay gio hien tai 
                    //dateSelected = DateTime.Now;

                }


                calendarView = new TKCalendar(new CGRect(0, 60, view_content.Frame.Width, view_content.Frame.Height - 60));
                calendarDelegate = new CalendarDelegate(this);

                calendarView.Layer.CornerRadius = 10;
                if (CmmVariable.SysConfig.LangCode == "1033")
                    calendarView.Locale = new NSLocale("en_US");
                else //if (CmmVariable.SysConfig.LangCode == "1066")
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


                if (dateSelected != default(DateTime))
                {
                    calendarView.SelectedDate = CmmIOSFunction.DateTimeToNSDate(dateSelected);
                    calendarView.NavigateToDate(calendarView.SelectedDate, false);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("FormCalendarChoice - loadContent - ERR: " + ex.ToString());
            }
        }

        public void CalendarDateSelected(string _dateSelected)
        {
            lbl_date.Text = _dateSelected;
            this.DismissViewControllerAsync(true);
        }


        #endregion

        #region events
        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            this.DismissViewControllerAsync(true);
        }
        private void BT_accept_TouchUpInside(object sender, EventArgs e)
        {
            if (parentView.GetType() == typeof(ToDoDetailView))
            {
                //element.Value = dateSelected.ToString();
                //ToDoDetailView toDo = parentView as ToDoDetailView;
                //toDo.HandleDateTimeChoiceChoice(element);

                this.DismissModalViewController(false);
            }
            else if (parentView.GetType() == typeof(WorkflowDetailView))
            {
                //element.Value = dateSelected.ToString();
                //WorkflowDetailView workflowDetailView = parentView as WorkflowDetailView;
                //workflowDetailView.HandleDateTimeChoiceChoice(element);

                this.DismissModalViewController(false);
            }
            else if (parentView.GetType() == typeof(FormWorkFlowDetails))
            {
                //element.Value = dateSelected.ToString();
                //FormWorkFlowDetails formWorkFlowDetails = parentView as FormWorkFlowDetails;
                //formWorkFlowDetails.HandleDateTimeChoiceChoice(element);

                this.DismissModalViewController(false);
            }
            else if (parentView.GetType() == typeof(FormWFDetailsProperty))
            {
                //element.Value = dateSelected.ToString();
                //FormWFDetailsProperty formWFDetailsProperty = parentView as FormWFDetailsProperty;
                //formWFDetailsProperty.HandleDateTimeChoiceChoice(element);

                this.DismissModalViewController(false);
            }
            else if (parentView.GetType() == typeof(FollowListViewController))
            {
                this.DismissModalViewController(false);
            }

        }
        private void BT_clear_TouchUpInside(object sender, EventArgs e)
        {
            calendarView.SelectedDate = null;
            lbl_date.Text = "";
            this.DismissViewControllerAsync(true);
        }
        private void BT_currentDate_TouchUpInside(object sender, EventArgs e)
        {
            calendarView.SelectedDate = CmmIOSFunction.DateTimeToNSDate(DateTime.Now);

            if (CmmVariable.SysConfig.LangCode == "1033")
                lbl_date.Text = DateTime.Now.ToString(@"MM/dd/yyyy", new CultureInfo("en"));
            else //if (CmmVariable.SysConfig.LangCode == "1066")
                lbl_date.Text = DateTime.Now.ToString(@"dd/MM/yyyy", new CultureInfo("vi"));

            lbl_date.TextColor = UIColor.Black;
        }
        private void UIDatePicker_ValueChanged(object sender, EventArgs e)
        {
            DateTime reference = new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateSelected = reference.AddSeconds((sender as UIDatePicker).Date.SecondsSinceReferenceDate);
            //string dt_str = dateSelected.ToLocalTime().ToString("dd/MM/yyyy");
            //CalendarDateSelected(dateSelected.ToLocalTime());
        }
        #endregion

        #region cust class
        class CalendarDelegate : TKCalendarDelegate
        {
            FormChoiceDateController parentView;

            public CalendarDelegate(FormChoiceDateController _parent)
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
                    string result = "";
                    DateTime dateValue = new DateTime();
                    if (CmmVariable.SysConfig.LangCode == "1033")
                    {
                        dateValue = DateTime.Parse(value, new CultureInfo("en", false));
                        result = dateValue.ToString(@"MM/dd/yyyy", new CultureInfo("en"));
                    }
                    else //if (CmmVariable.SysConfig.LangCode == "1066")
                    {
                        dateValue = DateTime.Parse(value, new CultureInfo("vi", false));
                        result = dateValue.ToString(@"dd/MM/yyyy", new CultureInfo("vi"));
                    }

                    parentView.CalendarDateSelected(result);
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


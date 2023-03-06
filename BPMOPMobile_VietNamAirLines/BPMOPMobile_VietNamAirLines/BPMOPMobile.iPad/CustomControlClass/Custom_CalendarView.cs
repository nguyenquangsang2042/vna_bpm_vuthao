using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using BPMOPMobile.Class;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using TelerikUI;
using UIKit;

namespace BPMOPMobile.iPad.CustomControlClass
{
    public class Custom_CalendarView : UIView
    {
        TKCalendar calendarView;
        TKCalendarMonthPresenter presenter;
        CalendarDelegate calendarDelegate;
        UIButton BT_currentDate, BT_clear;
        public UIViewController viewController { get; set; }
        public UILabel inputView { get; set; }
        public string hintText;

        private Custom_CalendarView()
        {

            calendarView = new TKCalendar(new RectangleF());
            calendarDelegate = new CalendarDelegate(this);

            calendarView.Layer.CornerRadius = 10;
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

            BT_currentDate = new UIButton();
            BT_currentDate.SetImage(UIImage.FromFile("Icons/icon_currentDate.png"), UIControlState.Normal);
            BT_currentDate.ImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
            BT_currentDate.ImageEdgeInsets = new UIEdgeInsets(1, 1, 1, 1);
            BT_currentDate.TouchUpInside += delegate
            {
                calendarView.SelectedDate = CmmIOSFunction.DateTimeToNSDate(DateTime.Now);

                if (CmmVariable.SysConfig.LangCode == "1033")
                    inputView.Text = DateTime.Now.ToString(@"MM/dd/yyyy", new CultureInfo("en"));
                else //if (CmmVariable.SysConfig.LangCode == "1066")
                    inputView.Text = DateTime.Now.ToString(@"dd/MM/yyyy", new CultureInfo("vi"));
            };

            BT_clear = new UIButton();
            BT_clear.SetImage(UIImage.FromFile("Icons/icon_closeRed.png"), UIControlState.Normal);
            BT_clear.ImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
            BT_clear.ImageEdgeInsets = new UIEdgeInsets(1, 1, 1, 1);
            BT_clear.TouchUpInside += delegate
            {
                calendarView.SelectedDate = null;
                inputView.TextColor = UIColor.FromRGB(153, 153, 153);
                inputView.Font = UIFont.FromName("Arial-ItalicMT", 14f);
                inputView.Text = hintText;
                CloseInstance();
            };

            AddSubviews(calendarView, BT_currentDate, BT_clear);
        }

        private static Custom_CalendarView instance = null;
        public static Custom_CalendarView Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Custom_CalendarView();
                }
                return instance;
            }
        }

        public void reset()
        {
            instance = new Custom_CalendarView();
        }

        public void InitFrameView(CGRect frame)
        {
            this.Frame = frame;
            const int padding = 20;

            calendarView.Frame = new CGRect(0, padding, Frame.Width, Frame.Height - (padding * 2));
            BT_clear.Frame = new CGRect(this.Frame.Width - (10 + 18), 20, 18, 18);
            BT_currentDate.Frame = new CGRect(BT_clear.Frame.X - (20 + 18), 20, 18, 18);
        }

        public void SetUpDate()
        {
            if (CmmVariable.SysConfig.LangCode == "1033")
                calendarView.Locale = new NSLocale("en_US");
            else //if (CmmVariable.SysConfig.LangCode == "1066")
                calendarView.Locale = new NSLocale("vi_VI");

            if (!string.IsNullOrEmpty(inputView.Text) && inputView.Text.Contains('/'))
            {
                try
                {
                    DateTime dt = new DateTime();
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        dt = DateTime.ParseExact(inputView.Text, "MM/dd/yyyy", new CultureInfo("en", false));
                    else //if (CmmVariable.SysConfig.LangCode == "1066")
                        dt = DateTime.ParseExact(inputView.Text, "dd/MM/yyyy", new CultureInfo("vi", false));

                    calendarView.ReloadData();
                    calendarView.SelectedDate = CmmIOSFunction.DateTimeToNSDate(dt);
                    calendarView.NavigateToDate(calendarView.SelectedDate, false);
                }
                catch (FormatException ex)
                {
                    Console.WriteLine("Custom_CalendarView - SetUpDate " + ex);
                }
            }
            else
            {
                calendarView.ReloadData();
                calendarView.ClearSelection();
            }
        }

        public void AddShadowForView()
        {
            this.Layer.ShadowColor = UIColor.DarkGray.CGColor;
            this.Layer.ShadowPath = UIBezierPath.FromRect(new CGRect(2, 2, Frame.Width, Frame.Height)).CGPath;
            this.Layer.ShadowRadius = 5;
            this.Layer.ShadowOffset = new CGSize(0, 2);
            this.Layer.ShadowOpacity = 1;
            this.ClipsToBounds = false;
        }

        private void CloseInstance()
        {
            if (viewController.GetType() == typeof(FormFillterToDoView))
            {
                FormFillterToDoView formFillterToDoView = viewController as FormFillterToDoView;
                formFillterToDoView.CloseCalendarInstance();
            }
            else if (viewController.GetType() == typeof(FormFillterToDoView))
            {
                FormFillterWorkFlowView formFillterWorkFlowView = viewController as FormFillterWorkFlowView;
                //formFillterWorkFlowView.CloseCalendarInstance();
            }
            else if (viewController.GetType() == typeof(SearchView))
            {
                SearchView search = viewController as SearchView;
                search.CloseCalendarInstance();
            }
        }

        #region cust class
        class CalendarDelegate : TKCalendarDelegate
        {
            Custom_CalendarView parentView;

            public CalendarDelegate(Custom_CalendarView _parent)
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
                    if (parentView.inputView != null)
                    {
                        var value = date.ToString();
                        DateTime dateValue = new DateTime();
                        if (CmmVariable.SysConfig.LangCode == "1033")
                        {
                            dateValue = DateTime.Parse(value, new CultureInfo("en", false));
                            parentView.inputView.Text = dateValue.ToString(@"MM/dd/yyyy", new CultureInfo("en"));
                        }
                        else //if (CmmVariable.SysConfig.LangCode == "1066")
                        {
                            dateValue = DateTime.Parse(value, new CultureInfo("vi", false));
                            parentView.inputView.Text = dateValue.ToString(@"dd/MM/yyyy", new CultureInfo("vi"));
                        }

                        parentView.inputView.Font = UIFont.SystemFontOfSize(14);
                        parentView.inputView.TextColor = UIColor.Black;
                        parentView.CloseInstance();
                    }
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using BPMOPMobile.Class;
using BPMOPMobileV1.iOS.IOSClass;
using BPMOPMobileV1.iOS.ViewControllers;
using CoreGraphics;
using Foundation;
using TelerikUI;
using UIKit;

namespace BPMOPMobileV1.iOS.CustomControlClass
{
    public class Custom_CalendarView : UIView
    {
        public TKCalendar calendarView;
        TKCalendarMonthPresenter presenter;
        CalendarDelegate calendarDelegate;
        UIButton BT_currentDate, BT_clear, BT_close;
        UIView viewIcon;
        public UIViewController viewController { get; set; }
        //public UILabel inputView { get; set; }
        public UITextField inputView { get; set; }
        //public string hintText;

        private Custom_CalendarView()
        {
            calendarView = new TKCalendar(new RectangleF());
            calendarDelegate = new CalendarDelegate(this);

            //calendarView.Layer.CornerRadius = 10;
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

            viewIcon = new UIView();
            viewIcon.BackgroundColor = UIColor.White;

            BT_currentDate = new UIButton();
            BT_currentDate.SetImage(UIImage.FromFile("Icons/icon_currentDate.png"), UIControlState.Normal);
            BT_currentDate.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            BT_currentDate.ImageEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_currentDate.TouchUpInside += delegate
            {
                calendarView.SelectedDate = CmmIOSFunction.DateTimeToNSDate(DateTime.Now);

                if (CmmVariable.SysConfig.LangCode == "1033")
                    inputView.Text = DateTime.Now.ToString(@"MM/dd/yyyy", new CultureInfo("en"));
                else //if (CmmVariable.SysConfig.LangCode == "1066")
                    inputView.Text = DateTime.Now.ToString(@"dd/MM/yyyy", new CultureInfo("vi"));
                CloseInstance();
                //ChangeDate();
            };

            BT_clear = new UIButton();
            BT_clear.SetImage(UIImage.FromFile("Icons/icon_deleteRed_calander.png"), UIControlState.Normal);
            BT_clear.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            BT_clear.ImageEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_clear.TouchUpInside += delegate
            {
                calendarView.SelectedDate = null;
                //inputView.TextColor = UIColor.FromRGB(153, 153, 153);
                //inputView.Font = UIFont.FromName("Arial-ItalicMT", 14f);
                //inputView.Text = hintText;
                inputView.Text = "";
                CloseInstance();
                //ChangeDate();
                EventChangeDateForKanBan();
            };
            BT_close = new UIButton();
            BT_close.SetImage(UIImage.FromFile("Icons/icon_x22.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
            BT_close.TintColor = UIColor.FromRGB(151, 151, 151);
            BT_close.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            BT_close.ImageEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_close.TouchUpInside += delegate
            {
                CloseInstance();
            };
            viewIcon.AddSubviews(BT_currentDate, BT_clear, BT_close);
            AddSubviews(calendarView, viewIcon);
            //var a = calendarView.Subviews;
            //foreach (var item in a)
            //{
            //    if (item.GetType() == typeof(UILabel))
            //    {

            //    }
            //}

        }
        //public void ChangeDate()
        //{
        //    if (viewController.GetType() == typeof(MainView))
        //    {
        //        MainView parent = viewController as MainView;
        //        parent.Tf_fromdate_ValueChanged(hintText);
        //    }
        //}
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
        public static Custom_CalendarView GetNewObject
        {
            get
            {
                if (instance != null)
                {
                    // Do any cleanup, perhaps call .Dispose if it's needed
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
            const int padding = 5;

            //calendarView.Frame = new CGRect(0, 40, Frame.Width, Frame.Height - 40);
            calendarView.Frame = new CGRect(0, padding, Frame.Width, Frame.Height - (padding * 2));
            viewIcon.Frame = new CGRect(Frame.Width - 40 * 3, 0, 45 * 3, 30);
            //viewIcon.Frame = new CGRect(Frame.Width - 45 * 3, 15, 45 * 3, 30);
            BT_close.Frame = new CGRect(40 * 2, 0, 30, 30);
            BT_clear.Frame = new CGRect(40, 0, 30, 30);
            BT_currentDate.Frame = new CGRect(0, 0, 30, 30);
        }



        public void SetUpDate()
        {
            if (!string.IsNullOrEmpty(inputView.Text) && inputView.Text.Contains('/'))
            {
                try
                {
                    DateTime dt = new DateTime();
                    if (CmmVariable.SysConfig.LangCode == "1033")
                    {
                        dt = DateTime.ParseExact(inputView.Text, "MM/dd/yyyy", new CultureInfo("vi", false));
                        calendarView.Locale = new NSLocale("en_US");
                    }
                    else //if (CmmVariable.SysConfig.LangCode == "1066")
                    {
                        dt = DateTime.ParseExact(inputView.Text, "dd/MM/yyyy", new CultureInfo("vi", false));
                        calendarView.Locale = new NSLocale("vi_VI");
                    }

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
                calendarView.SelectedDate = null;
                //try
                //{
                //    DateTime dt = DateTime.Now;
                //    calendarView.ReloadData();
                //    calendarView.SelectedDate = CmmIOSFunction.DateTimeToNSDate(dt);
                //}
                //catch (FormatException ex)
                //{
                //    Console.WriteLine("Custom_CalendarView - SetUpDate " + ex);
                //}
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
        public void EventChangeDateForKanBan()
        {
            if (viewController.GetType() == typeof(KanBanView))
            {
                KanBanView kanBanView = viewController as KanBanView;
                kanBanView.ChangeRangeDateWhenChooseDate();
            }
        }
        private void CloseInstance()
        {
            if (viewController.GetType() == typeof(MainView))
            {
                MainView mainView = viewController as MainView;
                mainView.CloseCalendarInstance();
                mainView.ToggleCalendar_toMe(false);
            }
            if (viewController.GetType() == typeof(MyRequestListView))
            {
                MyRequestListView myRequestListView = viewController as MyRequestListView;
                myRequestListView.CloseCalendarInstance();
                myRequestListView.ToggleCalendar_FromMe(false);
            }
            if (viewController.GetType() == typeof(RequestListView))
            {
                RequestListView requestListView = viewController as RequestListView;
                requestListView.CloseCalendarInstance();
                requestListView.ToggleCalendar_toMe(false);
            }
            //app
            //if (viewController.GetType() == typeof(MainViewApp))
            //{
            //    MainViewApp mainViewApp = viewController as MainViewApp;
            //    mainViewApp.CloseCalendarInstance();
            //    mainViewApp.ToggleCalendar_toMe(false);
            //}
            //if (viewController.GetType() == typeof(MyRequestListViewApp))
            //{
            //    MyRequestListViewApp myRequestListViewApp = viewController as MyRequestListViewApp;
            //    myRequestListViewApp.CloseCalendarInstance();
            //    myRequestListViewApp.ToggleCalendar_FromMe(false);
            //}
            //if (viewController.GetType() == typeof(RequestListViewApp))
            //{
            //    RequestListViewApp requestListViewApp = viewController as RequestListViewApp;
            //    requestListViewApp.CloseCalendarInstance();
            //    requestListViewApp.ToggleCalendar_toMe(false);
            //}
            if (viewController.GetType() == typeof(KanBanView))
            {
                KanBanView kanBanView = viewController as KanBanView;
                //kanBanView.CloseCustomCalendar();
                kanBanView.ToggleCalendar(false);
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
                        DateTime dateValue = DateTime.Parse(value, new CultureInfo("en", false));
                        parentView.inputView.Font = UIFont.SystemFontOfSize(14);
                        parentView.inputView.TextColor = UIColor.Black;
                        if (CmmVariable.SysConfig.LangCode == "1033")
                            parentView.inputView.Text = dateValue.ToString(@"MM/dd/yyyy", new CultureInfo("en"));
                        else
                            parentView.inputView.Text = dateValue.ToString(@"dd/MM/yyyy", new CultureInfo("vi"));
                    }
                    parentView.inputView.Font = UIFont.SystemFontOfSize(14);
                    parentView.inputView.TextColor = UIColor.Black;
                    //parentView.ChangeDate();
                    parentView.CloseInstance();
                    parentView.EventChangeDateForKanBan();

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


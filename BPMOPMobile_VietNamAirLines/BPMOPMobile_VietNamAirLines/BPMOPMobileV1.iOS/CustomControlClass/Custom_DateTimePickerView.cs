
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BPMOPMobile.Class;
using BPMOPMobileV1.iOS.IOSClass;
using BPMOPMobileV1.iOS.ViewControllers;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobileV1.iOS.CustomControlClass
{
    class Custom_DateTimePickerView : UIView
    {
        UIDatePicker datePicker;

        private Custom_DateTimePickerView()
        {
            this.BackgroundColor = UIColor.White;

            datePicker = new UIDatePicker();
            datePicker.Locale = NSLocale.FromLocaleIdentifier("en_GB");
            datePicker.BackgroundColor = UIColor.White;
            datePicker.Mode = UIDatePickerMode.DateAndTime;
            datePicker.MaximumDate = (NSDate)DateTime.Now.AddMonths(1);

            this.AddSubview(datePicker);

            datePicker.AddTarget(HandleChangeValue_DatePicker, UIControlEvent.ValueChanged);
        }

        private void HandleChangeValue_DatePicker(object sender, EventArgs e)
        {
            try
            {
                var value = datePicker.Date.ToString();
                DateTime dateValue = DateTime.Parse(value, new CultureInfo("en", false));

                if (inputView != null)
                {
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        inputView.Text = dateValue.ToString(CmmVariable.M_WorkDateFormatDateTimeEN, new CultureInfo("en-US"));
                    else
                        inputView.Text = dateValue.ToString(CmmVariable.M_WorkDateFormatDateTimeVN, new CultureInfo("vi-VN"));

                    if (viewController.GetType() == typeof(FormCreateTaskView))
                    {
                        if (dateValue < DateTime.Now)
                        {
                            CmmIOSFunction.commonAlertMessage(viewController, "BPM", "Hạn hoàn tất không được nhỏ hơn thời gian hiện tại.");

                            DateTime datenow = DateTime.Parse(DateTime.Now.ToString(), new CultureInfo("en-US", false));
                            if (CmmVariable.SysConfig.LangCode == "1033")
                                inputView.Text = datenow.ToString(CmmVariable.M_WorkDateFormatDateTimeEN, new CultureInfo("en-US"));
                            else
                                inputView.Text = datenow.ToString(CmmVariable.M_WorkDateFormatDateTimeVN, new CultureInfo("vi-VN"));

                            datePicker.SetDate(CmmIOSFunction.DateTimeToNSDate(datenow), true);
                        }
                    }
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Custom_DateTimePickerView - Unable to convert '{0}'.", datePicker.Date.ToString());
            }
        }

        private static Custom_DateTimePickerView instance = null;
        public static Custom_DateTimePickerView Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Custom_DateTimePickerView();
                }
                return instance;
            }
        }

        public void InitFrameView(CGRect frame)
        {
            this.Frame = frame;
            datePicker.Frame = new CGRect(0, 0, Frame.Width, Frame.Height);
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

        public void SetUpDate()
        {
            if (string.IsNullOrEmpty(inputView.Text))
            {
                // Tam khoa khong set default
                //try
                //{
                //    DateTime dateValue = DateTime.Parse(DateTime.Now.ToString(), new CultureInfo("en-US", false));
                //    if (CmmVariable.SysConfig.LangCode == "1066")
                //        inputView.Text = dateValue.ToString(CmmVariable.M_WorkDateFormatDateTimeVN, new CultureInfo("vi-VN"));
                //    else
                //        inputView.Text = dateValue.ToString(CmmVariable.M_WorkDateFormatDateTimeEN, new CultureInfo("en-US"));

                //    DateTime dt = DateTime.Now;
                //    datePicker.SetDate(CmmIOSFunction.DateTimeToNSDate(dt), true);
                //}
                //catch (FormatException ex)
                //{
                //    Console.WriteLine("Custom_DateTimePickerView - SetUpDate " + ex);
                //}
            }
            else
            {
                try
                {
                    DateTime dt = DateTime.ParseExact(inputView.Text, "dd/MM/yyyy", new CultureInfo("en-US", false));
                    datePicker.SetDate(CmmIOSFunction.DateTimeToNSDate(dt), true);
                }
                catch (FormatException ex)
                {
                    Console.WriteLine("Custom_DateTimePickerView - SetUpDate " + ex);
                }
            }
        }

        public UIViewController viewController { get; set; }

        public UITextField inputView { get; set; }
    }
}
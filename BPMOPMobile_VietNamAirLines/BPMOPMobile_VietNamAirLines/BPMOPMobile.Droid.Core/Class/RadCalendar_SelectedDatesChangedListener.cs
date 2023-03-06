using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Icu.Util;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Telerik.Widget.Calendar;

namespace BPMOPMobile.Droid.Core.Class
{
    /// <summary>
    /// Class này là Event Listener khi click vào RadCalendarView -> Update lên Textview tương ứng
    /// </summary>
    public class RadCalendar_SelectedDatesChangedListener : Java.Lang.Object, Com.Telerik.Widget.Calendar.RadCalendarView.IOnSelectedDatesChangedListener
    {
        private TextView _tvToSetData; // để set data khi có thay đổi dữ liệu
        DateTime selectedDateTime;
        public RadCalendar_SelectedDatesChangedListener(TextView _tvToSetData)
        {
            this._tvToSetData = _tvToSetData;
        }
        public void OnSelectedDatesChanged(RadCalendarView.SelectionContext context)
        {
            if (context.NewSelection().Count > 0)
            {
                Calendar calendar = Calendar.Instance;
                foreach (long item in context.NewSelection())
                {
                    calendar.TimeInMillis = item;
                    selectedDateTime = new DateTime(calendar.Get(CalendarField.Year), calendar.Get(CalendarField.Month) + 1, calendar.Get(CalendarField.DayOfMonth));
                    if (_tvToSetData != null)
                    {
                        if (BPMOPMobile.Class.CmmVariable.SysConfig.LangCode.Equals(BPMOPMobile.Droid.Core.Common.CmmDroidVariable.M_SysLangVN))
                            _tvToSetData.Text = selectedDateTime.ToString("dd/MM/yyyy");
                        else
                            _tvToSetData.Text = selectedDateTime.ToString("MM/dd/yyyy");
                    }
                }
            }
        }
        public DateTime GetCurrentSelectedDate()
        {
            return selectedDateTime;
        }
    }
}
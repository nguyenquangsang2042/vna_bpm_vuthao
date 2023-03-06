using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;

namespace BPMOPMobile.Droid.Core.Class
{
    public class BeanGroupShareHistory // ean này để sử dụng cho Adapter Share History
    {
        public BeanShareHistory parentItem { get; set; }
        public List<BeanShareHistory> listChild { get; set; }
    }
}
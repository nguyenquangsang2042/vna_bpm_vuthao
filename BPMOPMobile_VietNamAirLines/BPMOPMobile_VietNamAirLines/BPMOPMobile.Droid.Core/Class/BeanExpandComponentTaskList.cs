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
    public class BeanComponentTaskList
    {
        public BeanTask groupItem { get; set; }
        public List<BeanChildComponentTaskList> lstChild { get; set; }
        public bool IsExpand { get; set; }

    }
    public class BeanChildComponentTaskList
    {
        public BeanTask childItem { get; set; }
        public List<BeanTask> lstChildLV2 { get; set; }
        public bool IsExpand { get; set; }
    }
}
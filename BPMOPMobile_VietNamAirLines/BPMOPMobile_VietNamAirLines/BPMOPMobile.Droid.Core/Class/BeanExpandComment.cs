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
    public class BeanExpandComment
    {
        public BeanComment parentItem { get; set; }
        public List<BeanComment> lstChild { get; set; }
    }
}
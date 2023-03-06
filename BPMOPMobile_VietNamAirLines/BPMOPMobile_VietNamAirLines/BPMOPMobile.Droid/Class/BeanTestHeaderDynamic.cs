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

namespace BPMOPMobile.Droid.Class
{
    public class BeanTestHeaderDynamic
    {
        public string Title { get; set; }
        public string InternalName { get; set; }
        public string FieldID { get; set; }
        public int FieldIDInt { get; set; }
        public bool AllowSort { get; set; }
        public bool AllowFilter { get; set; }
        public bool IsSum { get; set; }
        public string Formular { get; set; }
        public string DataType { get; set; }
        public string FieldTypeID { get; set; }
        public string Option { get; set; } // Option = DataSource + Formular
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using SQLite;
using UIKit;

namespace BPMOPMobileV1.iOS.IOSClass
{
    public class ClassDynamicControl
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string icon { get; set; }
        [Ignore]
        public bool isSelected { get; set; }
    }
}
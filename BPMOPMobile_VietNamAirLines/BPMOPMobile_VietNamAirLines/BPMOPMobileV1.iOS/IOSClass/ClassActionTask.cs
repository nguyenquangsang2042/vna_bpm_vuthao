using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace BPMOPMobileV1.iOS.IOSClass
{
    public class ClassActionTask
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int Type { get; set; }
        public bool Visible { get; set; }
        public int Index { get; set; }
    }
}
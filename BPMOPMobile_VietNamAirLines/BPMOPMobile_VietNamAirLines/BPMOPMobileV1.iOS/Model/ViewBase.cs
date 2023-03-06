using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace BPMOPMobileV1.Model
{
    public class ViewBase
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }
        public bool Enable { get; set; }
        public bool IsRequire { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace BPMOPMobileV1.Model
{
    public class ViewSection: ViewBase
    {
        public bool ShowType { get; set; }
        public bool IsShowHint { get; set; }
        public List<ViewRow> ViewRows { get; set; }
    }
}
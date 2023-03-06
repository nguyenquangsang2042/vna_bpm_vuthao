using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace BPMOPMobileV1.Model
{
    public class ViewRow: ViewBase
    {
        public int RowType { get; set; }
        public List<ViewElement> Elements { get; set; }
    }
}
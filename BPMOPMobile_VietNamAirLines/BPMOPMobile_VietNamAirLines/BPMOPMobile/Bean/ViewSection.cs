using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BPMOPMobile.Bean
{
    public class ViewSection: ViewBase
    {
        public bool ShowType { get; set; }
        public bool IsShowHint { get; set; }
        public bool IsFollow { get; set; }
        public List<ViewRow> ViewRows { get; set; }
    }
}
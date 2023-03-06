using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BPMOPMobile.Bean
{
    public class ViewRow: ViewBase
    {
        public int RowType { get; set; }
        public List<ViewElement> Elements { get; set; }
    }
}
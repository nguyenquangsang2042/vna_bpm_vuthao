using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPMOPMobile.Bean
{
    public class ViewBase
    {      
        public string ID { get; set; } //int
        public string Title { get; set; }
        public string Value { get; set; } // string
        public bool Enable { get; set; }
        public bool IsRequire { get; set; }
    }
}
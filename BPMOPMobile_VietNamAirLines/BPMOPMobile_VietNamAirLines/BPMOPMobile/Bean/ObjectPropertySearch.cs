using System;
using System.Collections.Generic;
using System.Text;

namespace BPMOPMobile.Bean
{
    public class ObjectPropertySearch
    {
        public string lstProSeach { get; set; } // Ex: [{\"ContentType\":\"text\",\"Key\":\"ResourceViewID\",\"LogicCon\":\"eq\",\"Value\":\"7\"}]
        public int offset { get; set; } // dành cho SQL
        public int limit { get; set; } // dành cho SQL
        public int total { get; set; } // dành cho SQL
    }
}

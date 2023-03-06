using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPMOPMobile.Bean
{
    public class ObjectFilter
    {
        public string Key { get; set; } // key của column filter
        public string LogicCon { get; set; } // toán tử logic: eq, lte, gte, in, ...
        public string Value { get; set; }
        public string ValueType { get; set; } // hiện chưa sử dụng

        [Ignore]
        public string ContentType { get; set; } // loại content: date, datetime, text, ...
    }
}

using System;
using SQLite;

namespace BPMOPMobile.Bean
{
    public class BeanWFDetailLookup : BeanBase
    {
        public string ID { get; set; }
        public object DataValue { get; set; }
        public string DataType { get; set; }
        public bool viewOnly { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace BPMOPMobile.Bean
{
    public class BeanSearchProperty
    {
        public BeanSearchProperty()
        {
            ContentType = "";
            LogicCon = "eq";
        }

        public BeanSearchProperty(string key, string value)
        {
            ContentType = "";
            LogicCon = "eq";
            Key = key;
            Value = value;
        }

        public BeanSearchProperty(string key, string logicCon, string contentType, string value)
        {
            Key = key;
            LogicCon = logicCon;
            ContentType = contentType;
            Value = value;
        }

        public int ID { get; set; }
        public string Key { get; set; }
        public string LogicCon { get; set; }
        public string Value { get; set; }
        public string ContentType { get; set; }
        public string ValueType { get; set; }
    }
}

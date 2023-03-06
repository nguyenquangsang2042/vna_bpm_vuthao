using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPMOPMobile.Bean
{
    public class BeanLookupData : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public string ID { get; set; }
        public string Title { get; set; }
        [Ignore]
        public bool IsSelected { get; set; }
    }
}

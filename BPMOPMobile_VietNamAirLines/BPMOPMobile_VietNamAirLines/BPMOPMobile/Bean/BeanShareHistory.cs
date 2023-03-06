using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPMOPMobile.Bean
{
    public class BeanShareHistory : BeanBase
    {
        public int ID { get; set; }
        public string UserId { get; set; } // ID Người Share
        public string UserName { get; set; }
        public string UserPosition { get; set; }
        public string UserImagePath { get; set; }
        public DateTime DateShared { get; set; }
        public string Comment { get; set; }
        public int? ParentId { get; set; } // Nếu null thì là người Share - còn lại là người được Share
    }
}

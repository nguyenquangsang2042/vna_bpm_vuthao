using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPMOPMobile.Bean
{
    public class BeanUserAndGroup : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public string ID { get; set; }
        public string Name { get; set; } // Nếu là User thì là FullName, Group là Group Name
        public string AccountName { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }
        public int Type { get; set; } // 0 là User - 1 là Group
        public bool IsSelected { get; set; }
    }
}

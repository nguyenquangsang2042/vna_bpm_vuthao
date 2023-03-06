using AutoGeneralView.Model;
using SQLite;
using System;
using System.Collections.Generic;

namespace AutoGeneralView
{
    [Serializable]
    public class BeanNotify : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public int ID { get; set; }
        public string Title { get; set; }
        public string SendUnit { get; set; }
        public DateTime? DueDate { get; set; }

        public int Sections { get; set; }
    }
}

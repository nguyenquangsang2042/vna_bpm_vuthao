using System;
using SQLite;

namespace BPMOPMobile.Bean
{
    public class BeanAttachFileCategory : BeanBase
    {
        //new file
        [PrimaryKey, PrimaryKeyS]
        public int ID { get; set; }
        public int DocumentTypeID { get; set; }
        public string Title { get; set; }
        public string DocumentTypeValue { get; set; }
        public bool Required { get; set; }
        public bool ExportQR { get; set; }
        public bool Signature { get; set; }
        [Ignore]
        public bool IsSelected { get; set; }
    }
}

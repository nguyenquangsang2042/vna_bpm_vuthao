using System;
using SQLite;

namespace BPMOPMobile.Bean
{
    public class BeanWFDetailsHeader
    {
        public string Title { get; set; }
        public string internalName { get; set; }
        public string fieldID { get; set; }
        public string field { get; set; }
        public bool allowSort { get; set; }
        public bool allowFilter { get; set; }
        public bool allowGroup { get; set; }
        public bool hidden { get; set; }
        public string kendoFieldType { get; set; }
        public string formula { get; set; }
        public bool isSum { get; set; }
        public string dataType { get; set; }
        public bool viewOnly { get; set; }
        public bool require { get; set; }
        public string DataSource { get; set; }
        public string TitleEN { get; set; }
        public int fieldIDInt { get; set; }     // giống FieldID nhưng là dạng int
        public int FieldTypeId { get; set; }    // để xác định xem là control nào: 1 2 4 8 9 -> textbox.
        public string FieldMapping { get; set; }// dùng để map, nếu rỗng thì dùng internal name map
        public string Option { get; set; }      // Bao gồm Formular, dataSource, ...
        public float EstWidth { get; set; }     // Chieu rong contet cua header
    }
}

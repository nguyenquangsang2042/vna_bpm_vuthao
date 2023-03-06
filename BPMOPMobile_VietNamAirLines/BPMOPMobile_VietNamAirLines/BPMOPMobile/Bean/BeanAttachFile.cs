using SQLite;
using System;

namespace BPMOPMobile.Bean
{
    public class BeanAttachFile : BeanBase
    {
        public string ID { get; set; }
        public string Path { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public long Size { get; set; }
        public string CreatedBy { get; set; }
        public bool IsAuthor { get; set; }
        public int WorkflowId { get; set; }
        public int WorkflowItemId { get; set; }
        public string Name { get; set; }
        public int? AttachTypeId { get; set; }
        [Ignore]
        public bool IsImage { get; set; } // để phân biệt xem là file ảnh hay file loại khác
        [Ignore]
        public string AttachTypeName { get; set; }
        public string Extension { get; set; }
        public string Url { get; set; } // URL file trên server - xài trong trường hợp file comment
        public bool QRCode { get; set; }
        public bool SignCA { get; set; }
        public bool GenerateBy { get; set; }
        public double? Index { get; set; } //Do có sử dụng trên list sharepoint nữa, nên để double cho đồng bộ :3
        public DateTime Modified { get; set; }
        public string ModifiedBy { get; set; }
        [Ignore]
        public string ModifiedPositon { get; set; }
        [Ignore]
        public string ModifiedName { get; set; }
        public DateTime Created { get; set; }
        [Ignore]
        public string CreatedPositon { get; set; }
        [Ignore]
        public string CreatedName { get; set; }
    }
}

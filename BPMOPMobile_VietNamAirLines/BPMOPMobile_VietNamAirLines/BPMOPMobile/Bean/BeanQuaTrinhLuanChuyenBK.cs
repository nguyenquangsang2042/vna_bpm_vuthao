using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPMOPMobile.Bean
{
    // Để Backup, không còn sử dụng
    class BeanQuaTrinhLuanChuyenBK : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public string ID { get; set; }
        public string Title { get; set; }
        public int DocumentID { get; set; }
        public int? TaskID { get; set; }
        public string Category { get; set; }
        public bool Type { get; set; }
        public string PersonEmail { get; set; }
        public string PersonAccount { get; set; }
        public string SendUnit { get; set; }
        public int Priority { get; set; }
        public string EmailUpdate { get; set; }
        public string AssignedBy { get; set; }
        public bool Status { get; set; }
        public string Action { get; set; }
        public DateTime? DueDate { get; set; }
        public string Content { get; set; }
        public int Percent { get; set; }
        public string Note { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? StartDate { get; set; }
        public bool Notified { get; set; }
        public bool Reminder { get; set; }
        public string Read { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Email { get; set; }
        public string Group { get; set; }
        public string Rate { get; set; }
        public string ListName { get; set; }
        public string HasFile { get; set; }
        public int Step { get; set; }
        public string TaskCategory { get; set; }
        public string SubmitAction { get; set; }
        public string TaskSubCategory { get; set; }
        public string SiteName { get; set; }
        public string AssignedTo { get; set; }
        public string AssignedToF { get; set; }
        public string DepartmentName { get; set; }
        public string ChucDanh { get; set; }
        public int Count { get; set; } // Để xác định xem là gửi lên lần thứ mấy

    }
}

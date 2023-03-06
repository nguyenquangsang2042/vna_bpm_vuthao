using System;
using System.Collections.Generic;
using SQLite;

namespace BPMOPMobile.Bean
{
    public class BeanQuaTrinhLuanChuyen : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public string ID { get; set; }
        public int Step { get; set; }
        public int WorkflowID { get; set; }
        public string Title { get; set; }
        public string TitleEN { get; set; }
        public int? SubmitActionId { get; set; }
        public string SubmitAction { get; set; }
        public string SubmitActionEN { get; set; }
        public string Comment { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string AssignUserId { get; set; }
        public string AssignUserName { get; set; }
        public string AssignUserAvatar { get; set; }
        public string AssignDepartmentTitle { get; set; }
        public string AssignPositionTitle { get; set; }
        public string FromUserId { get; set; }
        public string FromUserName { get; set; }
        public string FromUserAvatar { get; set; }
        public string FromDepartmentTitle { get; set; }
        public string FromPositionTitle { get; set; }
        public int Count { get; set; } // Để xác định xem là gửi lên lần thứ mấy
        public string ParentId { get; set; }
        public bool Status { get; set; }
        public List<BeanQuaTrinhLuanChuyen> ChildHistory { get; set; }
        [Ignore]
        public bool IsSublevel2 { get; set; }
        [Ignore]
        public string AssignedTo { get; set; }
    }
}

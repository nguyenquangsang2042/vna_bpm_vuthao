using SQLite;
using System;


namespace BPMOPMobile.Bean
{
    [Serializable]
    public class BeanNotify : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public Guid ID { get; set; }
        public string Title { get; set; }
        [Ignore]
        public string URL { get; set; }
        [Ignore]
        public string RequestID { get; set; }
        public int? SPItemId { get; set; }
        public int? TaskID { get; set; }
        public string Category { get; set; }
        public bool Type { get; set; } //0: phối hợp, 1: cần xử lý
        public string SendUnit { get; set; }
        public int? Priority { get; set; }
        public string EmailUpdate { get; set; }
        public string AssignedBy { get; set; }
        public int Status { get; set; } // 0: chưa hoàn tất, 1: hoàn tất
        public int StatusGroup { get; set; } // map qua bảng BeanAppStatus
        public string Action { get; set; }
        public DateTime? DueDate { get; set; }
        public string Content { get; set; }
        public float? Percent { get; set; }
        public string Note { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? StartDate { get; set; }
        public bool Notified { get; set; }
        public bool Reminder { get; set; }
        public bool? Read { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string ListName { get; set; } // tên list chứa data
        public string ListNameEN { get; set; } // tên list chứa data EN
        public bool? HasFile { get; set; }
        public int? Step { get; set; }
        public string TaskCategory { get; set; }
        public string TaskSubCategory { get; set; }
        public string TitleEN { get; set; }
        public string SubmitAction { get; set; }
        public int? SubmitActionId { get; set; }
        public string SubmitActionEN { get; set; }
        public string WFImageURL { get; set; }//icon hiển thị
        [Ignore]
        public bool IsFollow { get; set; }
        [Ignore]
        public bool IsSelected { get; set; }

        public override string GetServerUrl()
        {
            return "/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=" + GetType().Name;
        }
    }
}

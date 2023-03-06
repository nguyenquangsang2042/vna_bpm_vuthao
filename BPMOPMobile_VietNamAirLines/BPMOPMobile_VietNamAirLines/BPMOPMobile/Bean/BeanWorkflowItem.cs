using System;
using SQLite;

namespace BPMOPMobile.Bean
{
    public class BeanWorkflowItem : BeanBase
    {
        public BeanWorkflowItem()
        {
            IsChange = true; // lần đầu lấy về là true
        }
        [PrimaryKey, PrimaryKeyS]
        public string ID { get; set; }
        public int? ItemID { get; set; }                   // DocumentID
        public int WorkflowID { get; set; }
        public string WorkflowTitle { get; set; }
        public string WorkflowTitleEN { get; set; }
        public string Title { get; set; }
        public string ListName { get; set; }
        public string SiteUrl { get; set; }
        public DateTime? IssueDate { get; set; }
        public string Content { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string DonViNguoiNhanTien { get; set; }
        public long? Total { get; set; }
        public string Status { get; set; }
        public int? StatusGroup { get; set; }
        /// <summary>
        /// -2; -1. Xoá, huỷ 
        /// 0. Đang lưu
        /// 4.Từ chối
        /// 10.Đã phê duyệt
        /// </summary>
        public int? ActionStatusID { get; set; }
        public string ActionStatus { get; set; }
        public string ActionStatusEN { get; set; }
        public string Approval { get; set; }
        public int? ApprovalStatus { get; set; }
        public string ApprovalName { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? DueDate { get; set; }
        public string ListId { get; set; }
        public string AssignedToName { get; set; } // Fullname, Fullname, Fullname
        public string AssignedTo { get; set; } // ID, ID, ID
        public DateTime? Modified { get; set; }
        public int? Step { get; set; }
        public string WorkflowStep { get; set; }
        public bool? HasFile { get; set; }
        public string TicketRequestDetails { get; set; } //json ticket details
        public string WFImageURL { get; set; }//icon hiển thị 
        public bool IsInfo { get; set; } // đang chờ bổ sung thông tin 
        public bool IsConsul { get; set; } // đang chờ tham vấn 
        public bool IsChange { get; set; } // Phiếu có thay đổi giá trị form động hay không
        public DateTime? CommentChanged { get; set; } // Lần cuối phiếu cập nhật bình luận (null là lấy lần đầu)

        [Ignore]
        public bool IsFollow { get; set; }
        [Ignore]
        public bool Read { get; set; }
        [Ignore]
        public bool IsSelected { get; set; }
        public int FileCount { get; set; }
        public int CommentCount { get; set; }
        [Ignore]
        public string GroupName { get; set; }
        // <summary>
        // Lấy đường dẫn Url tương ứng lấy dữ liệu từ Server
        // </summary>
        /// <returns></returns>
        public override string GetServerUrl()
        {
            return "/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=" + this.GetType().Name;
        }
    }
}

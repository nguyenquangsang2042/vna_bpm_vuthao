using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPMOPMobile.Bean
{
    // hoangpq check edit SangNQ 123 456
    // check edi
    // sangnq
    public class BeanAppBase : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public int ID { get; set; }
        public string Title { get; set; }
        public string TitleEN { get; set; }
        public string Content { get; set; }
        public string AssignedTo { get; set; }
        public int? Status { get; set; }
        public int? StatusGroup { get; set; }
        public int? ResourceCategoryId { get; set; } // 16:Task || 8:WorkFlow
        public int? ResourceSubCategoryId { get; set; } // Định danh cho ResourceCategoryId (ResourceCategoryId = 16 và ResourceSubCategoryId = 0 : Task)
        public DateTime? CommentChanged { get; set; }
        public string OtherResourceId { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string Permission { get; set; } // List ID những user có quyền
        public int? WorkflowId { get; set; }
        public string NotifiedUsers { get; set; }
        public int? FileCount { get; set; }
        public int? CommentCount { get; set; }
        public int? NumComment { get; set; }
        public string ItemUrl { get; set; }
        public int? Step { get; set; }           // xac dinh buoc cua phieu tren board
        public int? AppFlg { get; set; }
        public string AssignedBy { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? Modified { get; set; }
        [Ignore]
        public bool IsFollow { get; set; }
        public int? ApprovalStatus { get; set; }

        public override string GetServerUrl()
        {
            return "/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=" + GetType().Name;
        }
    }
}

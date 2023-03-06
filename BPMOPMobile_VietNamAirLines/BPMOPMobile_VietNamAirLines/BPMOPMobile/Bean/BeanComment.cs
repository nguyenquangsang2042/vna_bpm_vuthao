using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPMOPMobile.Bean
{
    public class BeanComment : BeanBase
    {
        [PrimaryKey]
        public string ID { get; set; }
        public int CID { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public string ResourceId { get; set; }
        public int ResourceCategoryId { get; set; }
        public int ResourceSubCategoryId { get; set; }
        public string ParentCommentId { get; set; }
        public int Status { get; set; }         // 0: Đang chờ duyệt; 1: Đã phê duyệt; -1 là từ chối. (Chỉ riêng tin tức mới có)
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public string TagUsers { get; set; }
        public bool FlgChanged { get; set; }
        public bool IsLiked { get; set; } // true la user đang like comment này
        public string Author { get; set; }
        public string AttachFiles { get; set; }
        public string Approver { get; set; }    // Người phê duyệt. (Chỉ riêng tin tức mới có)
        public DateTime? Modified { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? ApprovedDate { get; set; }  // Ngày phê duyệt. (Chỉ riêng tin tức mới có)
        [Ignore]
        public string ApproverName { get; set; }    // FullName người duyệt
        [Ignore]
        public string ApproverPosition { get; set; }
        [Ignore]
        public string AccountID { get; set; }
        [Ignore]
        public string AccountName { get; set; }
        [Ignore]
        public string AuthorPosition { get; set; }
        [Ignore]
        public string FullName { get; set; }
        [Ignore]
        public string ImagePath { get; set; }
        [Ignore]
        public string ResourceTitle { get; set; }
        [Ignore]
        public string ResourceUrl { get; set; }
        [Ignore]
        public int ItemId { get; set; } // ItemId trên site của Reosurce
        [Ignore]
        public List<BeanComment> LstChldComments { get; set; }
    }
}

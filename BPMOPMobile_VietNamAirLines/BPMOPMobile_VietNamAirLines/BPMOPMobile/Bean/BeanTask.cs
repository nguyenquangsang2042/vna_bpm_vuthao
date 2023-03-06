using System;
using System.Collections.Generic;
using SQLite;

namespace BPMOPMobile.Bean
{
    public class BeanTask : BeanBase
    {
        public BeanTask()
        {
            IsExpand = true;
        }

        [PrimaryKey, PrimaryKeyS]
        public int ID { get; set; }
        public int WorkflowId { get; set; }
        public int SPItemId { get; set; }
        public int Step { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public long Parent { get; set; }
        public int Percent { get; set; }
        public int Status { get; set; }
        //public string AssignedBy { get; set; } // Ko xài
        public string AssignedId { get; set; }
        public string AssignedName { get; set; } // Nếu trên 2 người trả theo dạng "User A, +1"
        public string AssignedImage { get; set; }
        public string AssignedPosition { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? Created { get; set; }
        public bool IsChange { get; set; }
        public DateTime? CommentChanged { get; set; } // Lần cuối phiếu cập nhật bình luận (null là lấy lần đầu)
        [Ignore]
        public string OtherResourceId { get; set; }
        [Ignore]
        public List<BeanTask> ChildTask { get; set; }
        [Ignore]
        public bool IsSublevel2 { get; set; }
        [Ignore]
        public bool IsExpand { get; set; }
    }
}

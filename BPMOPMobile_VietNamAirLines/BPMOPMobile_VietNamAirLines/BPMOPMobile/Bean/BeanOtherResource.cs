using System;
using System.Collections.Generic;
using System.Text;

namespace BPMOPMobile.Bean
{
    public class BeanOtherResource
    {
        public string Content { get; set; }
        public string ResourceId { get; set; } // OtherResourceID
        public int ResourceCategoryId { get; set; } // 8 WFItem - 16 Task
        public int ResourceSubCategoryId { get; set; } // 8 WFItem - 16 Task
        public string Image { get; set; }
        public string ParentCommentId { get; set; } // Parent: null - Child: parentID 
    }
}
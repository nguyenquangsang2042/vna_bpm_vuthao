using System;
using SQLite;

namespace BPMOPMobile.Bean
{
    public class BeanWorkflow : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public int WorkflowID { get; set; }
        public string ListID { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string TitleEN { get; set; }
        public string ImageURL { get; set; }
        public bool Favorite { get; set; } // = True là có yêu thích và ngược lại
        public bool IsPermission { get; set; } // = True là hiện lên
        public string StatusName { get; set; } // Deactive / Draft / Active
        public int? WorkflowCategoryID { get; set; }
        public DateTime? Modified { get; set; }
        [Ignore]
        public bool isExpand { get; set; }
        public override string GetServerUrl()
        {
            return "/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=" + this.GetType().Name;
        }
    }
}

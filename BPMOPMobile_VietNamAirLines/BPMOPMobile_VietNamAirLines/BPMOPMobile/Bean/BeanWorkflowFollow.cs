using System;
using SQLite;

namespace BPMOPMobile.Bean
{
    public class BeanWorkflowFollow : BeanBase
    {

        [PrimaryKey, PrimaryKeyS]
        public int WorkflowItemId { get; set; }
        public string UserId { get; set; }
        public int Status { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public override string GetServerUrl()
        {
            return "/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=" + this.GetType().Name;
        }
    }
}

using System;
using SQLite;

namespace BPMOPMobile.Bean
{
    public class BeanWorkFlowRelated
    {
        public int? StatusWorkflowID { get; set; }
        public string StatusWorkflow { get; set; }
        public string WorkflowContent { get; set; }
        public int? StatusWorkflowRLID { get; set; }
        public string StatusWorkflowRL { get; set; }
        public string WorkflowContentRL { get; set; }
        public int ID { get; set; }
        public string Title { get; set; }
        public int ItemID { get; set; }
        public string ItemCode { get; set; }
        public string ListId { get; set; }
        public string ListName { get; set; }
        public string ListNameEN { get; set; }
        public int ItemRLID { get; set; }
        public string RelatedCode { get; set; }
        public string ListRLID { get; set; }
        public string ListNameRL { get; set; }
        public string Category { get; set; }
        public string AdditionPermission { get; set; }
        public string AdditionPermissionValue { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? CreatedRL { get; set; }
        public string CreatedByRL { get; set; }
        public string CreatedBy { get; set; }
    }
}
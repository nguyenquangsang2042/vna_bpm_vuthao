using System;
using SQLite;

namespace BPMOPMobile.Bean
{
    public class BeanWorkflowStepDefine : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public int WorkflowStepDefineID { get; set; }
        public int WorkflowID { get; set; }
        public int? SubWorkflowID { get; set; }
        public int Step { get; set; }
        public string Title { get; set; }
        public int Duration { get; set; }
        public string TimeUnit { get; set; }
        public string EnterDay { get; set; }
        public override string GetServerUrl()
        {
            return "/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=" + this.GetType().Name;
        }
    }
}

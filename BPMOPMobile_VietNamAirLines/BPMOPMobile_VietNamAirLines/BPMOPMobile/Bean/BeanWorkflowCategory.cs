using System;
using SQLite;

namespace BPMOPMobile.Bean
{
    public class BeanWorkflowCategory : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public int ID { get; set; }
        public string Title { get; set; }
        public int? Order { get; set; }
        [Ignore]
        public bool IsSelected { get; set; }
        public override string GetServerUrl()
        {
            return "/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=" + this.GetType().Name;
        }
    }
}

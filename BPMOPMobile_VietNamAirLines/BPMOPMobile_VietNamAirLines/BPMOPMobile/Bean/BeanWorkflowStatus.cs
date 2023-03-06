using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPMOPMobile.Bean
{
    public class BeanWorkflowStatus : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public int ID { get; set; }
        public string Title { get; set; }
        public string TitleEN { get; set; }
        public string Description { get; set; }
        public int Index { get; set; }
        public int StatusGroup { get; set; }

        public override string GetServerUrl()
        {
            return "/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=" + GetType().Name;
        }
    }
}

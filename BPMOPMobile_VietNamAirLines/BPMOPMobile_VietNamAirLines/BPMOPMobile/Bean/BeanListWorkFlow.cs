using System;
using SQLite;

namespace BPMOPMobile.Bean
{
    //Dung cho IPAD
    public class BeanListWorkFlow :  BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public int ID { get; set; }
        public string Title { get; set; }
        public string TitleEN { get; set; }
        public string ListID { get; set; }
        public int WorkflowID { get; set; }
        public string Category { get; set; }
        public string Code { get; set; }
        public DateTime? Modified { get; set; }
        public string ImageURL { get; set; }
        public bool? DeleteStatus { get; set; }

        public override string GetServerUrl()
        {
            return "/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=" + this.GetType().Name;
        }
    }
}

using SQLite;
using System;

namespace BPMOPMobile.Bean
{
    class BeanDepartment : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public int ID { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public int? DeptLevel { get; set; }
        public int? ParentID { get; set; }
        public string ChartCode { get; set; }
        public string Manager { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }

        public override string GetServerUrl()
        {
            return "/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=" + GetType().Name;
        }
    }
}

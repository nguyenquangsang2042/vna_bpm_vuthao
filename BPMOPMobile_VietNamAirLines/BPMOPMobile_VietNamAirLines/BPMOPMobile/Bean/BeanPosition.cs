using SQLite;
using System;

namespace BPMOPMobile.Bean
{
    /// <summary>
    /// Tạm không dùng
    /// </summary>
    public class BeanPosition : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public int ID { get; set; }
        public string Title { get; set; }
        public int? Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string ModifiedBy { get; set; }

        public override string GetServerUrl()
        {
            return "/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=" + this.GetType().Name;
        }
    }
}
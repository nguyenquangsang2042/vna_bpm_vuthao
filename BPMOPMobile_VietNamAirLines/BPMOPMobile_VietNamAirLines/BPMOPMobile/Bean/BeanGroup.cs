using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPMOPMobile.Bean
{
    /// <summary>
    /// Tạm không dùng
    /// </summary>
    public class BeanGroup : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public string ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }
        public int WhoView { get; set; }
        public int SPGroupId { get; set; }
        public DateTime? Modified { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? Created { get; set; }
        public string CreatedBy { get; set; }
        public string Image { get; set; }
        public int? GroupType { get; set; }
        public int? Status { get; set; }
        public override string GetServerUrl()
        {
            return "/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=" + this.GetType().Name;
        }
        public string GetCurrentUserUrl()
        {
            return "/_layouts/15/VuThao.BPMOP.API/ApiUser.ashx?func=login";
        }
    }
}

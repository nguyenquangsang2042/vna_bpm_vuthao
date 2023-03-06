using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPMOPMobile.Bean
{
    public class BeanSettings : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public string KEY { get; set; }
        public string VALUE { get; set; }
        public string DESC { get; set; }

        public string DEVICE { get; set; }
        public DateTime? Modified { get; set; }

        public override string GetServerUrl()
        {
            return "/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&type=2&bname=" + this.GetType().Name;
        }
    }
}

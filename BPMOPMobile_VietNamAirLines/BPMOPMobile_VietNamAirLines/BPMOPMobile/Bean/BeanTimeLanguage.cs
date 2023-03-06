using System;
using SQLite;

namespace BPMOPMobile.Bean
{
    public class BeanTimeLanguage : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public int ID { get; set; }
        public int? Time { get; set; }
        public int Type { get; set; }
        public string Title { get; set; }
        public string TitleEN { get; set; }
        public int Index { get; set; }
        public int Devices { get; set; } // 0: dùng chung, 1: Mobile, 2: Web
        public DateTime? Modified { get; set; }

        public override string GetServerUrl()
        {
            return "/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=" + GetType().Name;
        }

    }
}
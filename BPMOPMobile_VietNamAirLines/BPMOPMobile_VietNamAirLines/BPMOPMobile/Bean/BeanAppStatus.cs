﻿using System;
using SQLite;

namespace BPMOPMobile.Bean
{
    public class BeanAppStatus : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public int ID { get; set; }
        public string Title { get; set; }
        public string TitleEN { get; set; }
        public int Index { get; set; }
        public string StatusDetails { get; set; }
        public int? ResourceCategoryIds { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsShow { get; set; }
        [Ignore]
        public bool IsSelected { get; set; }
        public override string GetServerUrl()
        {
            return "/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=" + GetType().Name;
        }
    }
}

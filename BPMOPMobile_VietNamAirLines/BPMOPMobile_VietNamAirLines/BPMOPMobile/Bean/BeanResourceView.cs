using System;
using SQLite;

namespace BPMOPMobile.Bean
{
    public class BeanResourceView : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public int ID { get; set; }
        public int? ResourceId { get; set; } // map với WorkflowID của BeanWorkflow
        public int? TypeId { get; set; }
        public string Title { get; set; }
        public string TitleEN { get; set; }
        public int Index { get; set; }
        public int Status { get; set; }
        public string OptionChart { get; set; }
        public bool? ExportPDF { get; set; }
        public bool? ExportExcel { get; set; }
        public int? ExportLimit { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string Image { get; set; }
        public string ListId { get; set; }
        public int? ResourceCategoryId { get; set; }
        public int? ResourceSubCategoryId { get; set; }
        public int? DataPermission { get; set; }
        public bool? DefaultFilter { get; set; }
        public int? MenuId { get; set; }
        public string Description { get; set; }
        public string QuerySQLSelect { get; set; }
        public string QuerySQLSorting { get; set; }
        public string QuerySQLWhere { get; set; }
        public string QuerySQLWherePara { get; set; }
        public string QuerySQLWhereParaAdv { get; set; }

        public override string GetServerUrl()
        {
            return "/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=" + GetType().Name;
        }
    }
}

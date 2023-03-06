using SQLite;

namespace BPMOPMobile.Bean
{
    public class BeanItemDeleted : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public string ID { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string TableName { get; set; }
        public string BeanName { get; set; }

        public override string GetServerUrl()
        {
            return "/_layouts/15/VuThao.BPMOP.API/ApiHandler.ashx?func=getdata";
        }
    }
}

using SQLite;

namespace BPMOPMobile.Bean
{
    public class BeanAppLanguage : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public string Key { get; set; }
        public string Value { get; set; }

        public override string GetServerUrl()
        {
            return "/_layouts/15/VuThao.BPMOP.API/ApiAppLang.ashx";
        }

    }
}

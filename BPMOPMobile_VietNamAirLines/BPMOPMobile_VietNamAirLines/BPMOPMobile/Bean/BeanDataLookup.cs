using SQLite;

namespace BPMOPMobile.Bean
{
    public class BeanDataLookup : BeanBase
    {
        public string ID { get; set; }
        public string Title { get; set; }
        [Ignore]
        public bool IsSelected { get; set; }
    }
}

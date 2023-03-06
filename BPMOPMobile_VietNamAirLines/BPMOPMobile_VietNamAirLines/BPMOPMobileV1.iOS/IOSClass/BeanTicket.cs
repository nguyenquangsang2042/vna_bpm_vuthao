using BPMOPMobile.Bean;

namespace BPMOPMobileV1.iOS.IOSClass
{
    public class BeanTickit : BeanBase
    {
        public string ID { get; set; }
        public string Path { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public bool IsExpand { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BPMOPMobile.Bean
{
    public class ViewElement : ViewBase
    {
        public string DataType { get; set; }
        public string DataSource { get; set; }
        public string TypeSP { get; set; }
        public string InternalName { get; set; }
        public string Formula { get; set; } // công thức tính value dựa theo các field khác
        public bool Hidden { get; set; }
        public List<ObjectElementNote> Notes { get; set; }
        public List<KeyValuePair<string, string>> ListProprety { get; set; }
    }
}
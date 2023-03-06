using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace BPMOPMobileV1.Model
{
    public class ViewElement : ViewBase
    {
        public string DataType { get; set; }
        public string DataSource { get; set; }
        public List<KeyValuePair<string, string>> ListProprety { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace BPMOPMobile.Bean
{
    public class ButtonAction
    {
        public int ID { get; set; }
        public string Value { get; set; }
        public string Title { get; set; }

        public string TitleEN { get; set; }
        public string Color { get; set; }
        public List<ObjectElementNote> Notes { get; set; }
        public ButtonAction()
        {
            Color = "0b61ae";
        }

    }
}

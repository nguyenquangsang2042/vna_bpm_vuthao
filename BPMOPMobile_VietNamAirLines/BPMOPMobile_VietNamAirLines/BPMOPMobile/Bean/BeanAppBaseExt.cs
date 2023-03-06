using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPMOPMobile.Bean
{
    [Serializable]
    public class BeanAppBaseExt : BeanAppBase
    {
        public DateTime? StartDate { get; set; }
        public bool? Read { get; set; }
        public int? SPItemId { get; set; }
        public string SubmitAction { get; set; }
        public int? SubmitActionId { get; set; }
        public string SubmitActionEN { get; set; }
        public string SendUnit { get; set; }
        public string ListName { get; set; }
        public bool? HasFile { get; set; }
        public int? Priority { get; set; }
        public DateTime? NotifyCreated { get; set; }
        [Ignore]
        public bool IsSelected { get; set; }

        public string UserName { get; set; }
        public string UserImage { get; set; }
        public string WorkflowTitle { get; set; }
        public string WorkflowTitleEN { get; set; }
        public string StatusText { get; set; }
        public string StatusTextEN { get; set; }

    }
}
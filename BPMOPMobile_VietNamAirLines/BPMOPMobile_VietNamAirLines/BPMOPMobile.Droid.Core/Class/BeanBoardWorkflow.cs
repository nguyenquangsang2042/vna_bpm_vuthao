using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using SQLite;

namespace BPMOPMobile.Droid.Core.Class
{
    public class BeanBoardWorkflow 
    {
        public BeanWorkflowCategory beanWorkflowCategory { get; set; }
        public List<BeanWorkflow> lstBeanWorkflow { get; set; }
        [Ignore]
        public bool IsExpand { get; set; }
    }
}
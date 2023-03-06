using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BPMOPMobile.Droid.Core.Class
{
    public class EnumFormControlView
    {
        public enum FlagViewControlAttachment
        {
            //1: DetailWorkflow, 2: DetailAttachFile, 3 - DetailCreateTask
            [Description("DetailWorkflow")]
            DetailWorkflow = 1,
            [Description("DetailAttachFile")]
            DetailAttachFile = 2,
            [Description("DetailCreateTask")]
            DetailCreateTask = 3,
            [Description("DetailCreateTaskChild")]
            DetailCreateTaskChild = 4,
            [Description("CreateWorkflowDetail")]
            CreateWorkflowDetail = 5,
        }
    }
}
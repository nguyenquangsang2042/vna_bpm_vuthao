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
    class EnumWorkflowStatus
    {
        public enum WorkflowAction
        {
            [Description("Không hành động")]
            None = -3,

            [Description("Xóa")]
            Delete = -2,

            [Description("Hủy phiếu")]
            Reject = -1,

            [Description("Soạn thảo/Nháp")]
            Draft = 0,

            [Description("Chờ phê duyệt")]
            WaitingForApproval = 1,

            [Description("Chờ bổ sung")]
            RequestInformation = 2,

            [Description("Chờ điều chỉnh")]
            Adjustment = 3,

            [Description("Từ chối")]
            Return = 4,

            [Description("Thu hồi")]
            Recall = 5,

            [Description("Phê duyệt")]
            Approve = 10
        }
    }
}
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
    public static class EnumFormControlInnerAction
    {
        public enum ControlInputAttachmentVertical_InnerActionID
        {
            [Description("Tạo mới File")] // click vào nút tạo mới file
            Create = 1,
            [Description("Sửa File")] // Button Edit nằm dưới khi kéo Item qua trái
            Edit = 2,
            [Description("Xóa File")] // Button Delete nằm dưới khi kéo Item qua trái
            Delete = 3,
            [Description("Xem Full File")] // Click thẳng vào Item View
            View = 4,
        }
        public enum ControlInputGridDetails_InnerActionID
        {
            [Description("Tạo mới chi tiết")] // click vào nút tạo mới
            Create = 1,
            [Description("Sửa File")] // Button Edit nằm dưới khi kéo Item qua trái
            Edit = 2,
            [Description("Xóa File")] // Button Delete nằm dưới khi kéo Item qua trái
            Delete = 3,
            [Description("Xem Full File")] // Click thẳng vào Item View
            View = 4,
        }
        public enum FlowRelated_InnerActionID
        {
            [Description("Xóa File")] // Button Delete nằm dưới khi kéo Item qua trái
            Delete = 3,
            [Description("Xem Full File")] // Click thẳng vào Item View
            View = 4,
        }
    }

}
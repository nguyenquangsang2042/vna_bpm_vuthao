using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.IOSClass
{
    public class CommonClass
    {
        public CommonClass()
        {
        }
    }

    public enum FlagUserPermission // Check xem là người nào: -1: người xem, 1: người tạo, 2: người xử lý
    {
        [Description("Người xem")] //disable all action/ button
        Viewer = -1,
        [Description("Người tạo")] // all action except status, xoa attachment neu la author
        Creator = 1,
        [Description("Người xử lý")] // cap nhat trang thai, tao task con, xoa attachment neu la author
        Handler = 2,
        [Description("Người tạo đồng thời là người xử lý")]
        CreatorAndHandler = 3,
    }

    public enum ActionStatusID // ID của Status phiếu, truong hop khac la Chua thuc hien
    {
        [Description("Hủy")]
        Cancel = 4,
        [Description("Hoàn tất")]
        Completed = 2,
        [Description("Tạm hoãn")]
        Hold = 3,
        [Description("Đang thực hiện")]
        InProgress = 1,
    }

    public class ClassMenu
    {
        public int ID { get; set; }
        public int section { get; set; }
        public string title { get; set; } // keylang code
        public string iconUrl { get; set; }
        public string count { get; set; }
        public bool isSelected { get; set; }
    }

    public class CountNum
    {
        public int count { get; set; }
    }
}
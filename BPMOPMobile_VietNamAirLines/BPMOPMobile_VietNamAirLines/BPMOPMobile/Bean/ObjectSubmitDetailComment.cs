using System;
using System.Collections.Generic;
using System.Text;

namespace BPMOPMobile.Bean
{
    public class ObjectSubmitDetailComment
    {
        public string ID { get; set; }
        public string DeviceName { get; set; } // Android: "App Android" - IOS: "App IOS"
        public string CodeName { get; set; } // giống DeviceName bỏ khoảng trắng: Android: "AppAndroid" - IOS: "AppIOS"
        public string AppName { get; set; } // Ex: "Digital Process"
        public string Platform { get; set; } // Tên hệ điều hành
        public string ResourceCategoryId { get; set; } // 8 WFItem - 16 Task
        public string ResourceUrl { get; set; } // Lấy trong BeanSettings - Task: TASK_URL - WFItem: WORKFLOW_URL
        public string ItemId { get; set; } // WFItem ID phiếu - Task ID Task
        public string Author { get; set; } // ID người tạo
        public string AuthorName { get; set; }

    }
}

using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPMOPMobile.Bean
{
    public class BeanTicketRequest : BeanBase
    {
        [PrimaryKey]
        public int ID { get; set; }
        public string Title { get; set; }
        public string YKien { get; set; }
        public string IdeaComment { get; set; }
        public string PhongBanGuiPheDuyet { get; set; } // phòng ban
        public string NhaMayVanPhong { get; set; }      // Nhà máy/ Vp
        public long? Total { get; set; }                 // số tiền
        public int Step { get; set; }                   // Bước trong quy trình
        public string Status { get; set; }              // Trạng thái request
        public string ActionStatus { get; set; }        // Trạng thái action hiện tại request
        public string DuAn { get; set; }                // Loại chi phí
        public DateTime? DueDate { get; set; }          // hạn hoàn tất của task
        public string WorkflowContent { get; set; }     // nội dung
        public string ScanFile { get; set; }            // file pdf của phiếu
        public int ActionPermission { get; set; }       // Enum check action

        [Ignore]
        public long? Sum_Total { get; set; }
        [Ignore]
        public long? SoTienDuKien { get; set; }
        [Ignore]
        public long? SoTienPhaiThanhToan { get; set; }

        [Ignore]
        public string UserValues { get; set; }

        [Ignore]
        public DateTime Modified { get; set; }
        [Ignore]
        public string Editor { get; set; }
        [Ignore]
        public DateTime Created { get; set; }
        [Ignore]
        public string Author { get; set; }
        public override string GetServerUrl()
        {
            return "/workflow/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx" + this.GetType().Name;
        }
    }
}

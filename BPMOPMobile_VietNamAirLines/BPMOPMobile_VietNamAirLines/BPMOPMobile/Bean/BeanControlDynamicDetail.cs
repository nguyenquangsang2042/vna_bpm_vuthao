using SQLite;
using System;
using System.Collections.Generic;

namespace BPMOPMobile.Bean
{
    [Serializable]
    public class BeanControlDynamicDetail : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public int ID { get; set; }

        public string FormId { get; set; }

        public int FormSubId { get; set; } // View đang ở bước nào

        public string Title { get; set; } // Tên của view

        public string TitleEN { get; set; }

        public bool Enable { get; set; } // Cho phép nhập

        public bool Visble { get; set; } // Cho phép visible hay không 

        public bool IsRequire { get; set; } // Bắt buộc nhập hay không

        public int ControlType { get; set; } //1: Text, 2: Text Area, 4: TextNum, 8: DateTime, 16: Date, 32: Time, 64: Combobox drop, 128: choice, 256: multi, 512: Text title group, 1024: Yes/No, 8388608: User

        public int? DataType { get; set; } // 1: Text, 2: Int, 4: Float

        public string DataField { get; set; } // Dữ liệu được lấy từ trường nào

        [Ignore]
        public string ControlValue { get; set; } // Giá trị của control  

        public DateTime Modified { get; set; }

        public DateTime Created { get; set; }

        public string DataSource { get; set; }

        public int DataSourceType { get; set; } // 1: DataSoure là  Json , 2:  DataSoure là câu query , 0 : DataSoure = Null

        public string ButtonAction { get; set; } // Chuỗi Mảng Json Button, Với ID trong Enum [VuThao.EWH.Common.Class.Workflow] Vd: [{"ID":1,"Value":"Next","Title":"Duyệt","Next"},{"ID":16,"Value":"Reject","Title":"Từ chối","Reject"},{"ID":32,"Value":"Recall","Title":"Thu hồi","Recall"}] 

        public int Rank { get; set; }

        // <summary>
        // Lấy đường dẫn Url tương ứng lấy dữ liệu từ Server
        // </summary>
        /// <returns></returns>
        public override string GetServerUrl()
        {
            return "/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=" + GetType().Name;
        }

        public static List<string> GetLstFieldId(List<BeanControlDynamicDetail> lst)
        {
            List<String> retValue = new List<string>();
            if (lst == null || lst.Count == 0) return retValue;

            lst.ForEach(i => retValue.Add(i.DataField));

            return retValue;
        }

    }
}
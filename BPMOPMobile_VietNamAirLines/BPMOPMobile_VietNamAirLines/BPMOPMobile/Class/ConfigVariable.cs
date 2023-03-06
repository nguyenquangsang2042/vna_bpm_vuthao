using System;

namespace BPMOPMobile.Class
{
    [Serializable]
    public class ConfigVariable
    {
        public ConfigVariable()
        {
            DelaySynsTime = 1800;
            DataLimitDay = 30;
        }
        public string GetCurrentUserUrl { get; set; }// Đường dẫn lấy thông tin User

        //public string Domain { get; set; }// Domain kết nối 
        public string Subsite { get; set; }// Subsite kết nối 

        public string UserId { get; set; }// userId login account 
        public string UserIdNum { get; set; } // userId login account number
        public string AvatarPath { get; set; }
        public string LoginName { get; set; }// userId login account 
        public string SiteName { get; set; }// SubSite, User thuộc đợn vị Subsite
        public string Title { get; set; }// user name login account         
        public string LoginPassword { get; set; }// password login account
        public string DisplayName { get; set; }// display name login account 
        public string Email { get; set; }// email login account 
        public string Mobile { get; set; }
        public string Department { get; set; }
        public string Address { get; set; }
        public string PositionTitle { get; set; }
        public int? PositionID { get; set; }
        public string DeviceInfo { get; set; }  // Thông tin Device Build bằng Json
        public string VerifyOTP { get; set; } // Lưu lại thông tin OTP đã mã hóa
        public string AppConfigVersion { get; set; }
        public int DelaySynsTime { get; set; } // Thời gian lặp lại syns dữ liệu tính bằn phút
        public int DataLimitDay { get; set; } // Số ngày dữ liệu sẽ lưu lại trên máy
        public string LangCode = "1066"; //1033 là tiếng anh, 1066 là tiếng Việt

        public string AccountName { get; set; } // dủng để review app

    }
}

using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace BPMOPMobile.Class
{
    public class CmmVariable
    {
        //public static string M_Domain = "https://becamexupgrade.vuthao.com";          // Becamex
        //public static string M_Domain = "https://kbsv.vuthao.com";                    // KBSecurity Vietnam
        //public static string M_Domain = "https://demobpm.vuthao.com";                 // Demo
        //public static string M_Domain = "https://intranet.vuthao.com";                // Intranet
        //public static string M_Domain = "https://trialbpm.vuthao.com/";               // Trial
        //public static string M_Domain = "https://bpmon.vuthao.com";                   // UAT
        //public static string M_Domain = "https://bpm.vuthao.com";                       // Dev

        /// <summary>
        /// site vietnam airlines live
        /// </summary>
        //public static string M_Domain = "https://bpm.vietnamairlines.com";                 // vietnamairlines live

        /// <summary>
        ///  site DEV VNA BPM
        /// </summary>
        public static string M_Domain = @"https://vnabpm.vuthao.com";

        /// <summary>
        /// site sample - review
        /// </summary>
        public static string M_Domain_develop = @"https://vnabpm.vuthao.com"; //site DEV 
        /// <summary>
        /// site live 
        /// </summary>
        public static string M_Domain_active = @"https://bpm.vietnamairlines.com"; //site LIVE
        public static string M_DataFolder = "data";                                     // đường dẫn lưu file trên thiết bị
        public static string M_DataPath = "DB_sqlite_XamDocument.db3";                  // đường dẫn file DB trên thiết bị
        public static string M_DataLangPath = "DB_Lang.db3";                            // đường dẫn file DB Chứa language
        public static string M_Avatar = "avatar.jpg";
        public static string M_AvatarCus = "avatarCus.jpg";                             // đường dẫn Avatar trên thiết bị
        public static string M_Folder_Avatar = "Avatars";                               // Folder icon nitify and itemworkflow
        public static string M_settingFileName = "config.ini";                          // đường dẫn file setting trên thiết bị

        public static Dictionary<string, string> M_LangData = null;                     //Dictionary
        public static ConfigVariable SysConfig = null;                                  // Dữ liệu lưu dữ lại ghi xuống file như: site, subsite
        public static HttpClient M_AuthenticatedHttpClient = null;                      // httpClient sử dụng chung khi kết nối thành công server
        public static short M_AutoReLoginNum = 0;                                       // Số Auto login bị lỗi liên tiếp
        public static short M_AutoReLoginNumMax = 1;                                    // Số tối đa được phép Auto login lỗi liên tiếp, nếu > Max sẽ yêu cầu User đăng nhập lại
        public static string M_ApiPath = "/API";
        public static string M_ApiLogin = "";
        public static string M_ResourceViewID_ToMe = "221";                             // View template Item danh sách phiếu cho lưới Việc đến tôi
        public static string M_ResourceViewID_FromMe = "221";                           // View template Item danh sách phiếu cho lưới Việc tôi bắt đầu
        public static short M_DataFilterDefaultDays = 30;                               // So ngay default khi filter
        public static short M_DataFilterAPILimitData = 60;                              // so luong data (item) lay khi goi API filter
        public static short M_DataLimitRow = 20;                                        // So luong data (item) limit 1 lan load cua luoi danh sach
        public static string M_WorkDateFormatDateVN = "dd/MM/yy";
        public static string M_WorkDateFormatDateTimeVN = "dd/MM/yy HH:mm";
        public static string M_WorkDateFormatDayEN = "MM/dd/yy";
        public static string M_WorkDateFormatDateTimeEN = "MM/dd/yy HH:mm";
        public static string M_LogPath = "log.txt";
        public static int M_DiffHours = 0;

        public static bool M_RenewDB = false;                                           //true là xóa db để tạo lại theo cấu trúc mới

        public static string APPSTATUS = "MOBILE_APPSTATUS_ENABLE";                              // ID AppStatus Mobile được xài        
        public static string APPSTATUS_FROMME = "MOBILE_APPSTATUS_FROMME";                       // ID AppStatus Default VTBD
        public static string APPSTATUS_TOME_DAXULY = "MOBILE_APPSTATUS_TOME_DAXULY";             // ID AppStatus Default VDT - Đã xử lý
        public static string APPSTATUS_TOME_DANGXULY = "MOBILE_APPSTATUS_TOME_DANGXULY";         // ID AppStatus Default VDT - Đang xử lý
        public static string APPSTATUS_FROMME_DAXULY = "MOBILE_APPSTATUS_FROMME_PROCESSED";      // ID AppStatus Default VTBD - Đã xử lý
        public static string APPSTATUS_FROMME_DANGXULY = "MOBILE_APPSTATUS_FROMME_INPROCESS";   // ID AppStatus Default VTBD - Đang xử lý


        // key API get count p_dynamic GetListCountVDT_VTBD
        public static string KEY_COUNT_ASIGNTOME_INPROCESS = "AssignedToMeAll"; // key call api lấy số đến tôi
        public static string KEY_COUNT_FROMME_INPROCESS = "RequestByMeInProcessAll"; // key call api lấy số đến tôi
        public static string TEXT_ALERT_DRAFT = "TEXT_ALERT_DRAFT"; // key call api lấy số đến tôi
        public static string KEY_COUNT_FOLLOW = "FollowItemAll";

        // key call api lấy phiếu của đến tôi, tôi bắt đầu ,follow LoadMoreDataTFromSerVer – p_base
        public static string KEY_GET_TOME_INPROCESS = "AssignedToMeAll";
        public static string KEY_GET_TOME_PROCESSED = "TaskhandledAll";
        public static string KEY_GET_FROMME_INPROCESS = "RequestByMeInProcessAll";
        public static string KEY_GET_FROMME_PROCESSED = "RequestByMeCompletedAll";
        public static string KEY_GET_FOLLOW = "FollowItemAll";

        public static Cookie M_cookie = null;
    }
}
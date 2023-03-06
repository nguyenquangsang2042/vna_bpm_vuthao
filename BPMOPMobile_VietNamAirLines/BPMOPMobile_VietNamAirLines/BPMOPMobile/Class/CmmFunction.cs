using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.RegularExpressions;
using BPMOPMobile.Bean;
using BPMOPMobile.DataProvider;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite;
using VuThao.Calc;

namespace BPMOPMobile.Class
{
    public class CmmFunction
    {
        public enum ControlType
        {
            // 1: Text, 2: Text Area, 4: TextNum, 8: DateTime, 16: Date, 32: Time, 64: Combobox drop, 128: choice, 256: multi, 8388608: User
            FIELD_TEXT = 1,
            FIELD_TEXT_AREA = 2,
            FIELD_TEXT_NUM = 4,
            FIELD_DATETIME = 8,
            FIELD_DATE = 16,
            FIELD_TIME = 32,
            FIELD_COMBOBOX_DROP = 64,
            FIELD_CHOICE = 128,
            FIELD_MULTICHOICE = 256,
            FIELD_TITLE_GROUP = 512,
            FIELD_YES_NO = 1024,
            FIELD_USER = 8388608
        }
        public enum CommentResourceCategoryID
        {
            [Description("Comment Workflow Item")]
            WorkflowItem = 8,
            [Description("Comment Công việc")]
            Task = 16,
        }
        public enum DynamicFieldTypeID // Để test trước khi chuyển cấu trúc datatype
        {
            SingleLineText = 1,
            MultipleLinesText = 2,
            Choice = 3,
            Number = 4,
            DateAndTime = 5,
            CheckBox = 6,
            UserGroup = 7,
            Currency = 8,
            Calculated = 9,
            Radio = 10,
            DropDownList = 11,
            ComboBox = 12,
            Lookup = 13,
        }
        public enum AppStatusID // để reference các trạng thái StatusGroup - AppBase
        {
            [Description("Tạm hoãn")]
            Pending = 128,
            [Description("Hủy")]
            Canceled = 64,
            [Description("Thu hồi")]
            Recalled = 32,
            [Description("Từ chối")]
            Rejected = 16,
            [Description("Hoàn tất")]
            Completed = 8,
            [Description("Đang thực hiện")]
            InProgress = 4,
            [Description("Chưa thực hiện")]
            NotStart = 2,
            [Description("Soạn thảo")]
            Draft = 1,
        }

        public static readonly string errPassword = "PasswordNotMatch";
        public static string GetTitle(string fieldId, string defaultValue = "")
        {
            string retValue = defaultValue;
            try
            {
                if (CmmVariable.M_LangData == null)
                {
                    CmmVariable.M_LangData = new Dictionary<string, string>();
                    if (File.Exists(CmmVariable.M_DataLangPath))
                    {
                        using (var conn = new SQLiteConnection(CmmVariable.M_DataLangPath, SQLiteOpenFlags.ReadOnly, false))
                        {
                            List<BeanAppLanguage> lstLang = conn.Query<BeanAppLanguage>("SELECT * FROM BeanAppLanguage");
                            foreach (BeanAppLanguage langItem in lstLang)
                            {
                                CmmVariable.M_LangData.Add(langItem.Key, langItem.Value);
                            }
                        }
                    }
                }

                //Nếu trong dictionary không có thì lấy từ dư liệu sqlLite
                string outValue;
                if (CmmVariable.M_LangData.TryGetValue(fieldId, out outValue))
                {
                    retValue = outValue;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("CmmFunction - getTitle - ERR: " + ex.ToString());
            }


            return retValue;
        }
        public static bool Register(string email)
        {
            string url = "/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=register";
            try
            {
                string _getCurrentUserUrl = CmmVariable.M_Domain + url;
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();
                JObject data = new JObject
                {
                    {"Email",email },
                    {"Subject",String.Empty },
                    {"Body",String.Empty }
                };


                lstPost.Add(new KeyValuePair<string, string>("data", JsonConvert.SerializeObject(data)));
                ProviderBase pro = new ProviderBase();
                JObject retData = pro.GetJsonDataFromAPI(_getCurrentUserUrl, ref CmmVariable.M_AuthenticatedHttpClient, new ProviderBase.PAR(lstGet, lstPost), false);
                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CmmFunction - Register - ERR: " + ex.ToString());
            }
            return false;
        }
        /// <summary>
        /// Login tới server
        /// </summary>
        /// <param name="loginUrl">Địa chỉ Url API thực hiện login</param>
        /// <param name="LoginName">tài khoản</param>
        /// <param name="PassWord">Pass hoặc mã OTP</param>
        /// <param name="loginType">Phân loại login: 1: login thông thường, 2: auto login, 3: login ghi đè DeviceInfo</param>
        /// <param name="OTPCode"> Mã OTP 6 số - nếu không có thì Login như bình thường</param>
        /// <returns></returns>
        public static HttpClient Login(string loginUrl, string loginName, string passWord, bool flgTickEventLogin = false, int loginType = 1, string OTPCode = "", string verifyOTP = "")
        {
            HttpClient client = null;
            string messageReturn = "";
            try
            {
                if (string.IsNullOrEmpty(OTPCode))
                {
                    HttpClientHandler handler = new HttpClientHandler();
                    Cookie cookie = GetAuthCookie(CmmVariable.M_Domain, loginName, passWord, ref messageReturn);
                    if (cookie == null)
                    {
                        if (!string.IsNullOrEmpty(messageReturn))
                        {
                            if (flgTickEventLogin)
                            {
                                CmmEvent.ReloginRequest_Performence(null, new CmmEvent.LoginEventArgs(false, loginName, passWord, "", null, messageReturn));
                            }
                            return null;
                        }
                        else
                        if (loginType == 1)
                            return null;
                        handler.CookieContainer = new CookieContainer();
                    }
                    else
                    {
                        handler.CookieContainer.Add(cookie);
                        CmmVariable.M_cookie = cookie;
                    }

                    handler.AllowAutoRedirect = false;
                    handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                    client = new HttpClient(handler);
                    client.Timeout = TimeSpan.FromMilliseconds(20000);
                    client.BaseAddress = new Uri(CmmVariable.M_Domain, UriKind.RelativeOrAbsolute);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
                }
                else
                    client = CmmVariable.M_AuthenticatedHttpClient;

                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "login"));
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();
                lstPost.Add(new KeyValuePair<string, string>("deviceInfo", string.IsNullOrEmpty(CmmVariable.SysConfig.DeviceInfo) ? "" : CmmVariable.SysConfig.DeviceInfo));
                lstPost.Add(new KeyValuePair<string, string>("loginType", loginType.ToString()));

                if (!string.IsNullOrEmpty(OTPCode) && OTPCode.Length == 6)
                {
                    lstPost.Add(new KeyValuePair<string, string>("OTP", OTPCode));
                }

                if (!string.IsNullOrEmpty(verifyOTP))
                {
                    lstPost.Add(new KeyValuePair<string, string>("verifyOTP", verifyOTP));
                }

                ProviderBase pro = new ProviderBase();
                JObject retData = pro.GetJsonDataFromAPI(loginUrl, ref client, new ProviderBase.PAR(lstGet, lstPost), false);
                if (retData == null)
                {
                    if (flgTickEventLogin)
                        CmmEvent.ReloginRequest_Performence(null, null);

                    return null;
                }
                string strStatus = retData.Value<string>("status");
                if (strStatus == null)
                {
                    if (flgTickEventLogin)
                        CmmEvent.ReloginRequest_Performence(null, null);

                    return null;
                }

                if (strStatus.Equals("SUCCESS"))
                {
                    CmmVariable.M_AuthenticatedHttpClient = client;
                    if (flgTickEventLogin)
                    {
                        if (retData["data"].ToString().Equals("OTP")) // Tài khoản có Authen 2 lớp -> userInfo = null - errCode = "OTP"
                        {
                            CmmEvent.ReloginRequest_Performence(null, new CmmEvent.LoginEventArgs(true, loginName, passWord, "", null, "OTP"));
                        }
                        else // Login bình thường
                        {
                            BeanUser userInfo = retData["data"]["beanUser"].ToObject<BeanUser>();
                            string _verifyOTP = retData["data"]["verifyOTP"].ToString(); CmmEvent.ReloginRequest_Performence(null, new CmmEvent.LoginEventArgs(true, loginName, passWord, _verifyOTP, userInfo));
                        }
                    }
                }
                else if (strStatus.Equals("ERR"))
                {
                    KeyValuePair<string, string> mess = retData["mess"].ToObject<KeyValuePair<string, string>>();

                    if (retData["data"].ToString().Equals("OTP") && mess.Key.Equals("101")) // Nhập OTP Sai -> userInfo = null + Error Message
                    {
                        if (flgTickEventLogin)
                        {
                            CmmEvent.ReloginRequest_Performence(null, new CmmEvent.LoginEventArgs(false, loginName, passWord, "", null, mess.Value));
                        }
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CmmFunction - Login - ERROR: " + ex.ToString());
                //return null;
            }
            return client;
        }
        /// <summary>
        /// get cookie auth
        /// </summary>
        /// <param name="url">Domain</param>
        /// <param name="uname">user name</param>
        /// <param name="pswd">pass word</param>
        /// <returns></returns>
        public static Cookie GetAuthCookie(string url, string uname, string pswd, ref string message)
        {
            Cookie returnValue = null;
            try
            {
                string envelope =
                        "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">"
                        + "<soap:Body>"
                        + "<Login xmlns=\"http://schemas.microsoft.com/sharepoint/soap/\">"
                        + "<username>{0}</username>"
                        + "<password>{1}</password>"
                        + "</Login>" + "</soap:Body>"
                        + "</soap:Envelope>";

                CookieContainer cookieJar = new CookieContainer();
                Uri authServiceUri = new Uri(url + "/_vti_bin/authentication.asmx");
                HttpWebRequest spAuthReq = HttpWebRequest.Create(authServiceUri) as HttpWebRequest;
                spAuthReq.CookieContainer = cookieJar;
                spAuthReq.Headers["SOAPAction"] = "http://schemas.microsoft.com/sharepoint/soap/Login";
                spAuthReq.ContentType = "text/xml; charset=utf-8";
                spAuthReq.Method = "POST";

                envelope = string.Format(envelope, uname, pswd);
                StreamWriter streamWriter = new StreamWriter(spAuthReq.GetRequestStream());
                streamWriter.Write(envelope);
                streamWriter.Close();
                HttpWebResponse response = spAuthReq.GetResponse() as HttpWebResponse;
                if (response != null)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (var responseStream = new StreamReader(response.GetResponseStream()))
                        {
                            string texthtml = responseStream.ReadToEndAsync().Result;
                            responseStream.Dispose();
                            if (!texthtml.Contains("<ErrorCode>NoError</ErrorCode>"))
                            {
                                message = errPassword;
                                return null;
                            }
                            else
                            {
                                message = "";
                                if (response.Cookies.Count > 0)
                                {
                                    returnValue = response.Cookies[0];
                                }
                            }
                        }
                    }
                    response.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetAuthCookie - Err: " + ex.ToString());
            }

            return returnValue;
        }
        /// <summary>
        /// Lấy Danh sách Name trong chuỗi dữ liệu lookup của Sharepoint
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public static List<string> GetNameFromLookupData(string strData)
        {
            List<string> retvalue = new List<string>();
            /*if (string.IsNullOrEmpty(strData)) return retvalue;
            if (!strData.Contains(";#"))
                return new List<string>() { strData };

            string[] lstData = strData.Split(new string[] { ";#" }, StringSplitOptions.None);

            for (int i = 0; i < lstData.Length - 1; i += 2)
            {
                retvalue.Add(lstData[i + 1]);
            }*/

            try
            {
                List<BeanLookupData> lst = JsonConvert.DeserializeObject<List<BeanLookupData>>(strData);
                foreach (BeanLookupData item in lst)
                {
                    retvalue.Add(item.Title);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR - CmmFunction - GetNameFromLookupData: " + e.ToString());
            }
            return retvalue;
        }

        /// <summary>
        /// Get Avatar sort name
        /// </summary>
        /// <param name="name">full name</param>
        /// <returns></returns>
        public static string GetAvatarName(string name)
        {
            string res = string.Empty;
            name = name.Trim();
            if (name.Contains(' '))
            {
                string[] lst_char = name.Split(' ');

                var first = lst_char[0].Substring(0, 1).ToUpper();
                var second = lst_char.Last().Substring(0, 1).ToUpper();
                res = first + second;
            }
            else
                res = name.Substring(0, 1).ToUpper();

            return res;
        }

        /// <summary>
        /// Format ngày giờ theo ngôn ngữ định nghĩa trước
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="type"> 0: thời gian tạo / ngày tạo  || 1: hạn xử lý </param>
        /// <param name="langCode"> 1033 là tiếng anh, 1066 là tiếng Việt </param>
        /// <returns></returns>
        public static string GetStringDateTimeLang(DateTime _dateTime, int type, int langCode)
        {
            string result = string.Empty;
            List<BeanTimeLanguage> lstLang = new List<BeanTimeLanguage>();
            if (File.Exists(CmmVariable.M_DataPath))
            {
                using (var conn = new SQLiteConnection(CmmVariable.M_DataPath, SQLiteOpenFlags.ReadOnly, false))
                {
                    string queryTimeLang = string.Format("SELECT * FROM BeanTimeLanguage WHERE Type = {0} AND Devices <> 2 ORDER BY [Index]", type);
                    lstLang = conn.Query<BeanTimeLanguage>(queryTimeLang);
                }
            }

            if (type == 0) // Thời gian tạo
            {
                if (_dateTime.Date == DateTime.Now.AddDays(-1).Date || _dateTime.Date == DateTime.Now.Date) // Ngày hôm qua Hoặc hôm nay -> HH:mm
                {
                    result = _dateTime.ToString("HH:mm");
                }
                else
                {
                    int diff = (int)(DateTime.Now - _dateTime).TotalMinutes;
                    int value = diff;

                    if (diff > 1200)
                        value = (diff / 1200);
                    else if (diff > 60)
                        value = (diff / 60);

                    foreach (var item in lstLang)
                    {
                        if (diff <= item.Time || !item.Time.HasValue)
                        {
                            if (item.Title.Contains("yy") || item.Title.Contains("HH"))
                                result = _dateTime.ToString(langCode == 1066 ? item.Title : item.TitleEN);
                            else
                                result = string.Format(langCode == 1066 ? item.Title : item.TitleEN, value.ToString());
                            break;
                        }
                    }
                }
            }
            else if (type == 1) // Hạn xử lý
            {
                int diff = (int)(DateTime.Now.Date - _dateTime.Date).TotalDays;

                foreach (var item in lstLang)
                {
                    if (diff <= item.Time || !item.Time.HasValue)
                    {
                        if (item.Title.Contains("yy") || item.Title.Contains("HH"))
                            result = _dateTime.ToString(langCode == 1066 ? item.Title : item.TitleEN);
                        else
                            result = string.Format(langCode == 1066 ? item.Title : item.TitleEN, Math.Abs((int)diff).ToString());
                        break;
                    }
                }
            }
            return result;
        }

        public static string GetNumberMBProcessed(long value)
        {
            string process = string.Empty;

            double dbCurrent = 0.0;

            if (value >= 1048576000)
            {
                dbCurrent = value / 1048576000;  //GB
                process = Math.Round(dbCurrent, 0) + " GB";
            }
            else if (value >= 1048576)
            {
                dbCurrent = value / 1048576;     //MB
                process = Math.Round(dbCurrent, 0) + " MB";
            }
            else
            {
                dbCurrent = value / 1024;     //KB
                process = Math.Round(dbCurrent, 0) + " KB";
            }

            return process;
        }

        /// <summary>
        /// Khởi tạo Dữ liệu ban đâu khi chạy lần đầu tiên
        /// </summary>
        /// <param name="dataFilePath">Đường dẫn file DataBase sqlite</param>
        /// <returns></returns>
        public static bool InstanceDB(string dataFilePath)
        {
            try
            {
                if (!File.Exists(dataFilePath))
                {
                    Console.WriteLine("Database does not exist, greate new data");
                    using (var conn = new SQLiteConnection(dataFilePath, false))
                    {
                        conn.CreateTable<DBVariable>();
                        conn.CreateTable<BeanWorkflowItem>();
                        conn.CreateTable<BeanUser>();
                        conn.CreateTable<BeanGroup>();
                        conn.CreateTable<BeanDepartment>();
                        conn.CreateTable<BeanNotify>();
                        conn.CreateTable<BeanControlDynamicDetail>();
                        conn.CreateTable<BeanSettings>();
                        conn.CreateTable<BeanTimeLanguage>();
                        conn.CreateTable<BeanWorkflow>();
                        conn.CreateTable<BeanWorkflowCategory>();
                        conn.CreateTable<BeanWorkflowStepDefine>();
                        conn.CreateTable<BeanWorkflowFollow>();
                        conn.CreateTable<BeanWorkflowStatus>();
                        conn.CreateTable<BeanPosition>();
                        conn.CreateTable<BeanComment>();
                        conn.CreateTable<BeanAppBase>();
                        conn.CreateTable<BeanResourceView>();
                        conn.CreateTable<BeanAppStatus>();
                        conn.CreateTable<BeanAppBaseExt>();

                        // Thêm biến danh sách ID Văn Bản đến
                        UpdateDBVariable(new DBVariable("VBDListID", ""));

                        //test close connection
                        conn.Close();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR - CmmFunction - instanceDB: " + ex.ToString());
            }
            return false;
        }

        /// <summary>
        /// Cập nhật lại biến trong Table DBVariable
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="con"></param>
        /// <returns></returns>
        public static bool UpdateDBVariable(DBVariable variable, SQLiteConnection con = null, bool isClose = false)
        {
            try
            {
                if (con == null)
                {
                    isClose = true;
                    con = new SQLiteConnection(CmmVariable.M_DataPath, SQLiteOpenFlags.ReadWrite, false);
                }
                TableQuery<DBVariable> table = con.Table<DBVariable>();
                var items = from i in table
                            where i.Id == variable.Id
                            select i;
                if (items.Count() > 0)
                {
                    con.Update(variable);
                }
                else
                {
                    con.Insert(variable);
                }
                return true;
            }
            catch (Exception) { }
            finally
            {
                if (isClose) con.Close();
            }
            return false;
        }

        public static bool CheckIsNewVer(string serverConfigAppversion, string string_app_ver)
        {
            bool res = false;
            var _serverConfigApp_version = new Version(serverConfigAppversion);
            var _string_app_ver = new Version(string_app_ver);

            var result = _serverConfigApp_version.CompareTo(_string_app_ver);
            if (result > 0)
            {
                Console.WriteLine("version_application is greater");
                res = true;
            }
            else if (result < 0)
            {
                Console.WriteLine("version_config is greater");
                res = false;
            }
            else
            {
                Console.WriteLine("versions are equal");
                res = false;
            }

            return res;
        }

        /// <summary>
        /// Format Dynamics the control data value.
        /// </summary>
        /// <returns>The control data value.</returns>
        /// <param name="controlType">Loại dynamic control (text, datetime, user ...).</param>
        /// <param name="controlValue">Giá trị gốc của control .</param>
        public static string FormatDynamicControlDataValue(int controlType, string controlValue)
        {
            string value = string.Empty;
            if (string.IsNullOrEmpty(controlValue)) return controlValue;

            switch (controlType)
            {
                case (int)ControlType.FIELD_DATETIME:
                    value = DateTime.Parse(controlValue).ToString("dd/MM/yyyy HH:mm");
                    break;
                case (int)ControlType.FIELD_DATE:
                    value = DateTime.Parse(controlValue).ToString("dd/MM/yyyy");
                    break;
                case (int)ControlType.FIELD_TIME:
                    value = DateTime.Parse(controlValue).ToString("HH:mm");
                    break;
                case (int)ControlType.FIELD_COMBOBOX_DROP:
                case (int)ControlType.FIELD_MULTICHOICE:
                case (int)ControlType.FIELD_USER:
                    if (controlValue.Contains(";#"))
                        value = controlValue.Split(new string[] { ";#" }, StringSplitOptions.None)[1];
                    else
                        value = controlValue;
                    break;
                case (int)ControlType.FIELD_TEXT_NUM:
                    CultureInfo cul = CultureInfo.GetCultureInfo("en-US");
                    CultureInfo culVN = CultureInfo.GetCultureInfo("vi-VN");
                    value = double.Parse(controlValue, cul).ToString("N0", culVN);
                    break;
                case (int)ControlType.FIELD_CHOICE:
                    value = controlValue;
                    break;
                case (int)ControlType.FIELD_YES_NO:
                    if (controlValue == "true")
                        value = "Yes";
                    else if (controlValue == "false")
                        value = "No";
                    else
                        value = controlValue;
                    break;
                default:
                    value = controlValue;
                    break;
            }
            return value;
        }

        public static object GetPropertyValueByName(object obj, string key)
        {
            object retValue = null;
            Type type = obj.GetType();
            PropertyInfo perInfo = type.GetProperty(key);
            if (perInfo != null)
            {
                retValue = perInfo.GetValue(obj);
            }
            return retValue;
        }

        public static object GetPropertyValue(object obj, PropertyInfo perInfo)
        {
            object retValue = null;
            retValue = perInfo.GetValue(obj);

            return retValue;
        }

        public static PropertyInfo[] GetPropertysWithType(Type objType, Type proType)
        {
            PropertyInfo[] retValue = null;
            PropertyInfo[] arrPro = objType.GetProperties();
            List<PropertyInfo> lstPro = new List<PropertyInfo>();
            foreach (PropertyInfo pro in arrPro)
            {
                if (pro.PropertyType == proType)
                {
                    lstPro.Add(pro);
                }
            }

            if (lstPro.Count > 0) return lstPro.ToArray();

            return retValue;
        }

        /// <summary>
        /// Đọc config cửa chương trình từ file config lên
        /// </summary>
        /// <returns></returns>
        public static bool ReadSetting()
        {
            try
            {
                if (File.Exists(CmmVariable.M_settingFileName))
                {
                    FileStream strm = new FileStream(CmmVariable.M_settingFileName, FileMode.Open, FileAccess.Read);
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter biforInfor = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    CmmVariable.SysConfig = (ConfigVariable)biforInfor.Deserialize(strm);
                    strm.Close();
                    return true;
                }
                else
                {
                    string dirPath = Path.GetDirectoryName(CmmVariable.M_settingFileName);
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath ?? throw new InvalidOperationException());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("ERR readSetting: " + ex.Message);
            }

            return false;
        }

        /// <summary>
        /// Ghi thông tin config xuống file
        /// </summary>
        /// <returns></returns>
        public static bool WriteSetting()
        {
            try
            {
                FileStream strm = new FileStream(CmmVariable.M_settingFileName, FileMode.OpenOrCreate, FileAccess.Write);
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter biforSetting = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                biforSetting.Serialize(strm, CmmVariable.SysConfig);
                strm.Close();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("ERR writeSetting: " + ex.Message);
            }

            return false;
        }

        private static readonly string[] VietnameseSigns = new string[]{
                                                            "aAeEoOuUiIdDyY",
                                                            "áàạảãâấầậẩẫăắằặẳẵ",
                                                            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
                                                            "éèẹẻẽêếềệểễ",
                                                            "ÉÈẸẺẼÊẾỀỆỂỄ",
                                                            "óòọỏõôốồộổỗơớờợởỡ",
                                                            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
                                                            "úùụủũưứừựửữ",
                                                            "ÚÙỤỦŨƯỨỪỰỬỮ",
                                                            "íìịỉĩ",
                                                            "ÍÌỊỈĨ",
                                                            "đ",
                                                            "Đ",
                                                            "ýỳỵỷỹ",
                                                            "ÝỲỴỶỸ"
                                                            };

        /// <summary>
        /// Bỏ dấu tiếng việt
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string removeSignVietnamese(string str)
        {
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);

            }
            return str;

        }

        //remove html string
        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="strPropName"></param>
        /// <param name="value"></param>
        public static void SetPropertyValueByName(object obj, string strPropName, object value)
        {
            PropertyInfo propInfo = GetProperty(obj, strPropName);
            if (propInfo != null)
            {
                //Nullable<System.DateTime>
                Type t = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;

                object safeValue = (value == null) ? null : Convert.ChangeType(value, t);

                propInfo.SetValue(obj, safeValue, null);
            }

        }

        /// <summary>
        ///  Set giá trị cho thuộc tính của Object
        /// </summary>
        /// <param name="obj">Object muốn set giá trị</param>
        /// <param name="propInfo">Thuộc tính propertyInfo thuộc Class Object</param>
        /// <param name="value">Giá trị muốn set</param>
        /// <returns></returns>
        public static void SetPropertyValue(object obj, PropertyInfo propInfo, object value)
        {
            Type t = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;
            object safeValue = (value == null) ? null : Convert.ChangeType(value, t);
            propInfo.SetValue(obj, safeValue, null);


        }

        /// <summary>
        /// Giải mã chuỗi Json thành Object và bỏ qua Catch
        /// </summary>
        /// <typeparam name="T">Kiểu đối tượng muốn chuyển đổi thành</typeparam>
        /// <param name="value">Giá trị chuỗi Json</param>
        /// <returns></returns>
        public static T TryDeserializeObject<T>(string value)
        {
            T retValue = default(T);
            try
            {
                retValue = JsonConvert.DeserializeObject<T>(value);
            }
            catch { }
            return retValue;
        }

        /// <summary>
        /// Map phần Data field giữa 2 object
        /// </summary>
        /// <param name="objFrom">object chứa dữ liệu nguồn</param>
        /// <param name="objTo">object muốn map dữ liệu tới</param>
        /// <param name="lstColsFilter">Danh sách các cột muốn lọc lấy dự liệu</param>
        /// <returns></returns>
        public static object MapData(object objFrom, object objTo, List<string> lstColsFilter = null)
        {

            Type objFromType = objFrom.GetType();
            PropertyInfo[] arrProperty = objFromType.GetProperties();
            foreach (PropertyInfo prop in arrProperty)
            {

                // Nếu Property không tồn lại trong List lọc thì bỏ qua
                if (lstColsFilter != null && !lstColsFilter.Contains(prop.Name)) continue;

                object fieldValue = prop.GetValue(objFrom);
                SetPropertyValueByName(objTo, prop.Name, fieldValue);

            }

            return objTo;
        }

        public static PropertyInfo GetProperty(object obj, string strPropName)
        {
            Type type = obj.GetType();
            return type.GetProperty(strPropName);
        }

        public static object GetPropertyValue(object obj, string strPropName)
        {
            Type type = obj.GetType();
            return type.GetProperty(strPropName).GetValue(obj, null);
        }

        public static T ChangeToRealType<T>(object readData)
        {
            if (readData is T)
            {
                return (T)readData;
            }
            else
            {
                try
                {
                    return (T)Convert.ChangeType(readData, typeof(T));
                }
                catch (InvalidCastException)
                {
                    return default(T);
                }
            }
        }
        /// <summary>
        /// Lấy các quyền Action của tôi Enable
        /// </summary>
        /// <param name="lstButtonAction">Danh sách các Button Acction tại bước muốn kiểm tra</param>
        /// <param name="actionPermission">Action của User trên Item Phiếu yêu cầu</param>
        /// <returns></returns>
        public static List<ButtonAction> GetLstMyAction(List<ButtonAction> lstButtonAction, int actionPermission)
        {
            return lstButtonAction.FindAll(i => (i.ID & actionPermission) > 0);
        }
        public static List<ButtonAction> GetColorMyAction(List<ButtonAction> lstButtonAction, Dictionary<string, string> dictionary)
        {
            foreach (ButtonAction item in lstButtonAction)
            {
                string value = GetValue(dictionary, item.ID.ToString());
                if (!string.IsNullOrEmpty(value))
                {
                    item.Color = value;
                }
            }
            return lstButtonAction;
        }
        public static string GetValue(Dictionary<string, string> keyValuePairs, string key)
        {
            string value = "";
            try
            {
                if (keyValuePairs != null && !string.IsNullOrEmpty(key))
                {
                    if (keyValuePairs.ContainsKey(key))
                    {
                        value = (keyValuePairs[key]);
                        return value;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return value;
        }

        public static string CollectRequireData(List<BeanControlDynamicDetail> lstBeanControlDynamicDetails)
        {
            string requiredDataField = "";
            foreach (BeanControlDynamicDetail beanControlDynamicDetail in lstBeanControlDynamicDetails)
            {
                if (beanControlDynamicDetail.IsRequire && beanControlDynamicDetail.Enable)
                {
                    if (CmmVariable.SysConfig.LangCode == "1033")
                    {
                        requiredDataField += beanControlDynamicDetail.DataField + "," + beanControlDynamicDetail.TitleEN + "|";
                    }
                    else
                    {
                        requiredDataField += beanControlDynamicDetail.DataField + "," + beanControlDynamicDetail.Title + "|";
                    }
                }
            }
            return requiredDataField;
        }

        public static string ValidateEmptyRequiredField(List<BeanControlDynamicDetail> lstBeanControlDynamicDetails, Dictionary<string, string> dicValues)
        {
            string emptyRequiredField = "";
            string[] fields = CollectRequireData(lstBeanControlDynamicDetails).Split("|");
            foreach (string fieldName in fields)
            {
                if (String.IsNullOrEmpty(CmmFunction.GetValue(dicValues, fieldName.Split(",")[0])) && fieldName != "")
                {
                    emptyRequiredField += fieldName.Split(",")[1] + ", ";
                }
            }
            return emptyRequiredField;
        }

        public static List<BeanStepQTLC> CloneLstStepQTLC(List<BeanQuaTrinhLuanChuyen> lstQTLC)
        {
            List<BeanStepQTLC> lstResult = new List<BeanStepQTLC>();
            if (lstQTLC == null || lstQTLC.Count == 0)
                return lstResult;

            try
            {
                lstQTLC = lstQTLC.OrderBy(x => x.Step).ThenBy(x => x.Created).ToList();
                BeanStepQTLC _currentStepQTLC = new BeanStepQTLC()
                {
                    Action = CmmVariable.SysConfig.LangCode.Equals("1033") ? lstQTLC[0].TitleEN : lstQTLC[0].Title,
                    Step = lstQTLC[0].Step,
                    ListStepQTLC = new List<BeanQuaTrinhLuanChuyen>() { lstQTLC[0] }
                };
                lstResult.Add(_currentStepQTLC);

                for (int i = 1; i < lstQTLC.Count; i++)
                {
                    if (_currentStepQTLC.Step != lstQTLC[i].Step) // Đã qua step khác -> renew và Add vào
                    {
                        _currentStepQTLC = new BeanStepQTLC()
                        {
                            Action = CmmVariable.SysConfig.LangCode.Equals("1033") ? lstQTLC[i].TitleEN : lstQTLC[i].Title,
                            Step = lstQTLC[i].Step,
                            ListStepQTLC = new List<BeanQuaTrinhLuanChuyen>() { lstQTLC[i] }
                        };
                        lstResult.Add(_currentStepQTLC);
                    }
                    else
                        _currentStepQTLC.ListStepQTLC.Add(lstQTLC[i]);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("CmmFunction - CloneLstStepQTLC - ERR: " + ex.ToString());
#endif
            }
            return lstResult;
        }

        /// <summary>
        /// Chuyển đổi dung lượng file thành chuỗi text theo format
        /// </summary>
        public static class FileSizeFormatter
        {
            // Load all suffixes in an array
            static readonly string[] suffixes =
            { "Bytes", "KB", "MB", "GB", "TB", "PB" };
            public static string FormatSize(Int64 bytes)
            {
                int counter = 0;
                decimal number = (decimal)bytes;
                while (Math.Round(number / 1024) >= 1)
                {
                    number = number / 1024;
                    counter++;
                }
                return string.Format("{0:n1} {1}", number, suffixes[counter]);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static bool CheckIsImageAttachmentType(string fileExtention)
        {
            bool res;
            string[] fileImageExtensionArray = new string[] { "png", "jpg", "gif", "jpeg", "bpm" };

            if (fileImageExtensionArray.Contains(fileExtention.ToLower()))
                res = true;
            else
                res = false;

            return res;
        }

        public static List<BeanComment> GetListComment(BeanWorkflowItem workflowItem, string _OtherResourceId, DateTime _CommentChanged)
        {
            try
            {
                List<BeanComment> lst_comments = new List<BeanComment>();
                SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                ProviderControlDynamic p_dynamic = new ProviderControlDynamic();
                if (workflowItem.CommentChanged == null) // Lấy lần đầu
                {
                    string _APIdatenow = "";
                    lst_comments = p_dynamic.GetListComment(_OtherResourceId, (int)CmmFunction.CommentResourceCategoryID.WorkflowItem, null, ref _APIdatenow);
                    if (lst_comments != null && lst_comments.Count > 0)
                    {
                        workflowItem.CommentChanged = DateTime.Parse(_APIdatenow);
                        workflowItem.IsChange = false;
                        conn.Update(workflowItem);
                    }
                    return lst_comments;
                }
                else // ko phải lần đầu -> so sánh với local
                {
                    if (workflowItem.CommentChanged < _CommentChanged)
                        workflowItem.IsChange = true;

                    if (workflowItem.IsChange == true) // trên server có thay đổi -> update list cũ
                    {
                        string _APIdatenow = "";

                        p_dynamic.GetListComment(_OtherResourceId, (int)CmmFunction.CommentResourceCategoryID.WorkflowItem, workflowItem.CommentChanged, ref _APIdatenow);

                        workflowItem.CommentChanged = DateTime.Parse(_APIdatenow);
                        workflowItem.IsChange = false;
                        conn.Update(workflowItem);

                        // GetListComment Đã update Sqlite -> gọi Local lên
                        string _queryComment = string.Format(@"SELECT * FROM BeanComment WHERE ResourceId = '{0}'", _OtherResourceId);
                        List<BeanComment> _lstCommentLocal = conn.Query<BeanComment>(_queryComment); // List Local
                        lst_comments = _lstCommentLocal;
                    }
                    else // ko có thay đổi -> lấy từ local
                    {
                        string _queryComment = string.Format(@"SELECT * FROM BeanComment WHERE ResourceId = '{0}'", _OtherResourceId);
                        List<BeanComment> _lstCommentLocal = conn.Query<BeanComment>(_queryComment); // List Local
                        lst_comments = _lstCommentLocal;
                    }
                    return lst_comments;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CmmFunction - GetListComment - Err: " + ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Tìm ra BeanUser bằng Id, nếu ko ra trả null
        /// </summary>
        /// <param name="_userID"></param>
        /// <returns></returns>
        public static BeanUser GetBeanUserByID(string _userID, SQLiteConnection conn = null)
        {
            BeanUser _res = null;
            try
            {
                if (conn == null)
                    conn = new SQLiteConnection(CmmVariable.M_DataPath, SQLiteOpenFlags.ReadWrite, false);

                string queryUser = string.Format("SELECT * FROM BeanUser WHERE ID = '{0}'", _userID.ToLowerInvariant().Trim());
                List<BeanUser> _lstUser = conn.Query<BeanUser>(queryUser);
                if (_lstUser != null && _lstUser.Count > 0)
                    _res = _lstUser[0];
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("CmmFunction - GetBeanUserFromID - ERR: " + ex.ToString());
#endif
                return null;
            }
            finally
            {
                conn.Close();
            }
            return _res;
        }

        /// <summary>
        /// Không sử dụng
        /// </summary>
        /// <param name="_positionID"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static BeanPosition GetUserBeanPositionByUserID(int _positionID, SQLiteConnection conn = null)
        {
            BeanPosition _res = null;
            try
            {
                if (conn == null)
                    conn = new SQLiteConnection(CmmVariable.M_DataPath, SQLiteOpenFlags.ReadWrite, false);

                string queryPosition = string.Format("SELECT * FROM BeanPosition WHERE ID = ?");
                List<BeanPosition> _lstPos = conn.Query<BeanPosition>(queryPosition, _positionID);
                if (_lstPos != null && _lstPos.Count > 0)
                    _res = _lstPos[0];
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("CmmFunction - GetUserBeanPositionByUserID - ERR: " + ex.ToString());
#endif
                return null;
            }
            finally
            {
                conn.Close();
            }
            return _res;
        }

        /// <summary>
        /// Query đầy đủ thông tin của List<BeanUserAndGroup>
        /// </summary>
        /// <param name="_lstAssign"></param>
        /// <returns></returns>
        public static List<BeanUserAndGroup> QueryInfoListAssign(List<BeanUserAndGroup> _lstAssign, SQLiteConnection conn = null)
        {
            string _querGroupToBeanUserGroup = @"SELECT ID, Title as Name, Title as AccountName, Description as Email, Description as ImagePath, 1 as Type FROM BeanGroup WHERE ID= '{0}'";
            string _querUserToBeanUserGroup = @"SELECT ID, FullName as Name, AccountName as AccountName, Email, ImagePath as ImagePath, 0 as Type FROM BeanUser WHERE ID= '{0}'";

            List<BeanUserAndGroup> _res = new List<BeanUserAndGroup>();
            try
            {
                if (conn == null)
                    conn = new SQLiteConnection(CmmVariable.M_DataPath);

                foreach (BeanUserAndGroup _item in _lstAssign)
                {
                    if (_item.Type == 0) // User
                    {
                        List<BeanUserAndGroup> _temp = conn.Query<BeanUserAndGroup>(String.Format(_querUserToBeanUserGroup, _item.ID));
                        if (_temp != null && _temp.Count > 0)
                            _res.Add(_temp[0]);
                    }
                    else // Group
                    {
                        List<BeanUserAndGroup> _temp = conn.Query<BeanUserAndGroup>(String.Format(_querGroupToBeanUserGroup, _item.ID));
                        if (_temp != null && _temp.Count > 0)
                            _res.Add(_temp[0]);
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("CmmFunction - QueryInfoListAssign - ERR: " + ex.ToString());
#endif
            }
            finally
            {
                conn.Close();
            }
            return _res;
        }

        /// <summary>
        /// Lấy URL comment từ setting ra, nếu ko có thì lấy giá trị mặc định
        /// </summary>
        /// <param name="ResourceSubCategoryId">8 WFItem - 16 Task</param>
        /// <returns></returns>
        public static string GetURLSettingComment(int ResourceSubCategoryId, SQLiteConnection conn = null)
        {
            string _result = "";
            try
            {
                if (conn == null)
                    conn = new SQLiteConnection(CmmVariable.M_DataPath);

                string _query = "SELECT * FROM BeanSettings WHERE KEY = '{0}'";
                switch (ResourceSubCategoryId)
                {
                    case 8: // Comment ở WFItem
                        {
                            List<BeanSettings> _lstSetting = conn.Query<BeanSettings>(string.Format(_query, "WORKFLOW_URL"));
                            if (_lstSetting != null && _lstSetting.Count > 0)
                                _result = _lstSetting[0].VALUE;
                            else // nếu chưa setting gán giá trị default
                                _result = "/workflow/SitePages/Workflow.aspx?ItemId={0}";

                            break;
                        }
                    case 16: // Comment ở Task
                        {
                            List<BeanSettings> _lstSetting = conn.Query<BeanSettings>(string.Format(_query, "TASK_URL"));
                            if (_lstSetting != null && _lstSetting.Count > 0)
                                _result = _lstSetting[0].VALUE;
                            else // nếu chưa setting gán giá trị default
                                _result = "/workflow/SitePages/InsertOrUpdateTask.aspx?TID={0}";

                            break;
                        }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("CmmFunction - GetURLSettingComment - ERR: " + ex.ToString());
#endif
            }
            finally
            {
                conn.Close();
            }
            return _result;
        }

        /// <summary>
        /// Tính toán giá trị trên Object
        /// </summary>
        /// <param name="extendCondition">Chuỗi điều kiện</param>
        /// <param name="objData">Đối tượng chứa dữ liệu</param>
        /// <returns></returns>
        public static object CalculateObject(string extendCondition, object objData)
        {
            try
            {
                extendCondition = extendCondition.TrimStart('='); // chỉ để lại công thức dạng [SoLuong]*[DonGia]
                Expression exp = new Expression(extendCondition);
                exp.EvaluateFunction += Formula.Exp_EvaluateFunction;
                List<string> lstParaName = GetParaInStr(extendCondition);
                if (lstParaName != null && lstParaName.Count > 0)
                {
                    foreach (string paraName in lstParaName)
                    {
                        object fieldValue = GetPropertyValueByNameNew(objData, paraName);
                        if (!exp.Parameters.ContainsKey(paraName))
                            exp.Parameters.Add(paraName, fieldValue);
                    }
                }
                return exp.Evaluate();
            }
            catch (Exception ex)
            {
                Console.WriteLine("CmmFunction - CalculateObject - ERR: " + ex.ToString());
                return null;
            }
        }

        public static object GetPropertyValueByNameNew(object obj, string key)
        {
            object retValue = null;
            try
            {
                Type type = obj.GetType();

                if (type == typeof(Newtonsoft.Json.Linq.JObject))
                {
                    retValue = ((JObject)obj)[key].ToObject<object>();
                }
                else
                {
                    PropertyInfo perInfo = type.GetProperty(key);
                    if (perInfo != null)
                    {
                        retValue = perInfo.GetValue(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CmmFunction - GetPropertyValueByNameNew - ERR: " + ex.ToString());
            }
            return retValue;
        }

        public static List<string> GetParaInStr(string strInput)
        {
            List<string> retValue = new List<string>();

            System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex(@"\[(\d|\w|\.|_)+\]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.MatchCollection matches = rgx.Matches(strInput);
            if (matches.Count <= 0) return retValue;
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                string itemF = match.Value.Replace("[", "");
                itemF = itemF.Replace("]", "");
                if (!retValue.Contains(itemF))
                    retValue.Add(itemF);
            }
            return retValue;
        }

        public static List<ViewElement> SortListElementAction(List<ViewElement> _lstElementAction)
        {
            List<ViewElement> _result = new List<ViewElement>();
            try
            {
                int[] _lstRule = new int[]
                {
                    (int)WorkflowAction.Action.Save,
                    (int)WorkflowAction.Action.Next,
                    (int)WorkflowAction.Action.Approve,
                    (int)WorkflowAction.Action.Return,
                    (int)WorkflowAction.Action.CreateTask,
                    (int)WorkflowAction.Action.Forward,
                    (int)WorkflowAction.Action.RequestInformation,
                    (int)WorkflowAction.Action.RequestIdea,
                    (int)WorkflowAction.Action.Cancel,
                    (int)WorkflowAction.Action.Return,
                    (int)WorkflowAction.Action.Reject,
                };

                foreach (int item in _lstRule)
                {
                    _result.AddRange(_lstElementAction.FindAll(x => x.ID == item.ToString()).ToList());
                    _lstElementAction.RemoveAll(x => x.ID == item.ToString());
                }
                // Add thêm những Action không có trong list Rule
                if (_lstElementAction != null && _lstElementAction.Count > 0)
                {
                    _result.AddRange(_lstElementAction);
                }

            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("CmmFunction - GetURLSettingComment - ERR: " + ex.ToString());
#endif
            }
            return _result;
        }

        /// <summary>
        /// Dành cho Control Number -> trả ra kiểu hiển thị theo format
        /// </summary>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        public static string GetFormatControlDecimal(ViewElement elementNumber)
        {
            string _result = "";
            try
            {
                CultureInfo _culEN = CultureInfo.GetCultureInfo("en-US");
                //CultureInfo _culVN = CultureInfo.GetCultureInfo("vi-VN");

                double _customValue;
                if (string.IsNullOrEmpty(elementNumber.Value))
                    _customValue = 0;
                else
                    _customValue = double.Parse(elementNumber.Value.Trim(), _culEN);

                if (!String.IsNullOrEmpty(elementNumber.DataSource)) // _element.DataSource trả ra là số chữ số decimal
                {
                    int _demicalCount = int.Parse(elementNumber.DataSource);
                    _result = _customValue.ToString("N" + ((_demicalCount > 0) ? _demicalCount.ToString() : "0"), _culEN);
                }
                else
                    _result = _customValue.ToString("N0", _culEN);
            }
            catch (Exception ex)
            {
                Console.WriteLine("CmmFunction - GetFormatDecimal - ERR: " + ex.ToString());
            }
            return _result;
        }

        /// <summary>
        /// trả ra WorkflowItemID bằng URL AppBase
        /// </summary>
        /// <param name="url">nếu không thấy trả empty</param>
        /// <returns></returns>
        public static string GetWorkflowItemIDByUrl(string itemURL)
        {
            string workflowItemID = "";
            try
            {
                string urlLower = itemURL.ToLowerInvariant();
                Dictionary<string, string> dictURL = urlLower.Split('&').ToDictionary(c => c.Split('=')[0], c => System.Uri.UnescapeDataString(c.Split('=')[1]));
                if (dictURL.ContainsKey("rid"))
                    workflowItemID = dictURL["rid"];
                else if (dictURL.ContainsKey("itemid"))
                    workflowItemID = dictURL["itemid"];
            }
            catch (Exception ex)
            {
                Console.WriteLine("CmmFunction - GetWorkflowItemIDByUrl - ERR: " + ex.ToString());
            }
            return workflowItemID;
        }

        /// <summary>
        /// Check xem có trường nào chưa nhập data không -> return false nếu có control required chưa có data
        /// </summary>
        public static bool ValidateRequiredForm(List<ViewSection> _LISTSECTION)
        {
            bool _res = true;
            try
            {
                foreach (ViewSection _itemSection in _LISTSECTION)
                    foreach (ViewRow _itemRow in _itemSection.ViewRows)
                    {
                        List<ViewElement> _lstElement = _itemRow.Elements.Where(x => x.Enable == true && x.IsRequire == true && String.IsNullOrEmpty(x.Value)).ToList();
                        if (_lstElement != null && _lstElement.Count > 0)
                            _res = false;
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CmmFunction - ValidateRequiredForm - ERR: " + ex.ToString());
            }
            return _res;
        }

        /// <summary>
        /// Trả ra value Setting ứng theo key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAppSettingValue(string key)
        {
            string _result = "";
            SQLiteConnection connection = new SQLiteConnection(CmmVariable.M_DataPath, false);
            try
            {
                string query = string.Format("SELECT VALUE FROM BeanSettings WHERE KEY = '{0}' LIMIT 1 OFFSET 0", key);

                List<BeanSettings> _lstResult = connection.Query<BeanSettings>(query);
#if DEBUG
                var l = connection.Query<BeanSettings>("SELECT * FROM BeanSettings");
#endif
                if (_lstResult != null && _lstResult.Count > 0)
                    _result = _lstResult[0].VALUE;
            }
            catch (Exception ex)
            {
                Console.WriteLine("CmmFunction - GetAppSettingValue - ERR: " + ex.ToString());
            }
            finally
            {
                connection.Close();
            }
            return _result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="_resourceViewID"> ID cua ViewResoureView template</param>
        /// <param name="_trangthai"> 1: Da xu ly || 2: dang xu ly</param>
        /// <param name="_tinhtrang">danh sach tinh trang chon theo format: "1,2,4,8" ...</param>
        /// <param name="_Hanxuly"> 1: tat ca | 2: trong ngay | 3: tre han</param>
        /// <param name="_ngaytaoFrom">Từ ngày</param>
        /// <param name="_ngaytaoTo">Đến ngày</param>
        /// <param name="_workflowID">dành cho Application - chỉ trả ra theo workflowID này</param>
        /// <returns></returns>
        public static Dictionary<string, string> BuildListPropertiesFilter(string _resourceViewID, string _trangthai, string _tinhtrang, int _Hanxuly, DateTime _ngaytaoFrom, DateTime _ngaytaoTo, string _workflowID = "")
        {
            Dictionary<string, string> _lstProperties = new Dictionary<string, string>();
            _lstProperties.Add("ResourceViewID", _resourceViewID);
            _lstProperties.Add("lcid", CmmVariable.SysConfig.LangCode);

            // Trang Thai - Toi bat dau khong co trangthai (dang XL / Da XL)
            if (!string.IsNullOrEmpty(_trangthai))
                _lstProperties.Add("viewtype", _trangthai);

            _lstProperties.Add("StatusGroup", _tinhtrang);

            // Hạn hoàn tất
            if (_Hanxuly == 1) // Tat ca
            {
                //duedate_condition = "";
            }
            else if (_Hanxuly == 2) // Trong ngay
            {
                _lstProperties.Add("duedate-gte", DateTime.Now.Date.ToString("yyyy-MM-dd HH:mm"));
                _lstProperties.Add("duedate-lte", DateTime.Now.Date.AddDays(1).AddMinutes(-1).ToString("yyyy-MM-dd HH:mm"));

            }

            else if (_Hanxuly == 3) // Tre han
            {
                _lstProperties.Add("duedate-lte", DateTime.Now.Date.ToString("yyyy-MM-dd"));
            }

            // Ngay tao
            if (_ngaytaoFrom != default)
                _lstProperties.Add("created-gte", _ngaytaoFrom.Date.ToString("yyyy-MM-dd HH:mm"));

            if (_ngaytaoTo != default)
                _lstProperties.Add("created-lte", _ngaytaoTo.Date.AddDays(1).AddMinutes(-1).ToString("yyyy-MM-dd HH:mm"));

            // WorkflowID
            if (!string.IsNullOrEmpty(_workflowID))
                _lstProperties.Add("WorkflowID", _workflowID);

            return _lstProperties;
        }

        /// <summary>
        /// Lấy ra dạng dữ liệu thô cho GetFormattedValueByHeader
        /// </summary>
        /// <param name="_itemHeader">Header cần tìm value</param>
        /// <param name="_currentJObjectRow">JObject của item cần tìm</param>
        /// <returns>raw data String theo header tương ứng </returns>
        public static string GetRawValueByHeader(BeanWFDetailsHeader _itemHeader, JObject _currentJObjectRow)
        {
            string _res = "";
            try
            {
                JToken _curJToken = null;
                if (!String.IsNullOrEmpty(_itemHeader.FieldMapping)) // ưu tiền FieldMapping, nếu không có thì xài internalName
                    _curJToken = _currentJObjectRow[_itemHeader.FieldMapping.ToString()];
                else if (!String.IsNullOrEmpty(_itemHeader.internalName))
                    _curJToken = _currentJObjectRow[_itemHeader.internalName.ToString()];

                if (_curJToken != null)
                    _res = _curJToken.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("CmmFunction - GetAppSettingValue - ERR: " + ex.ToString());
            }
            return _res;
        }

        /// <summary>
        /// Hàm không dùng Để tìm ra value String cho item Header tương ứng - cho phần List sử dụng 
        /// </summary>
        /// <param name="_itemHeader">Header cần tìm value</param>
        /// <param name="_currentJObjectRow">JObject của item cần tìm</param>
        /// <returns>data đã format theo control cho Dynamic List</returns>
        public static string GetFormattedValueByHeader(BeanWFDetailsHeader _itemHeader, JObject _currentJObjectRow)
        {
            SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
            string _res = "";
            try
            {
                string rawData = CmmFunction.GetRawValueByHeader(_itemHeader, _currentJObjectRow);

                switch (_itemHeader.FieldTypeId)
                {
                    case (int)CmmFunction.DynamicFieldTypeID.UserGroup: // User group -> "Hoàng Đăng Khoa, +4"
                        {
                            string[] _arrayId = rawData.Trim().ToLowerInvariant().Split(",");
                            if (_arrayId != null && _arrayId.Length > 0)
                            {
                                string queryUser = string.Format(@"SELECT FullName FROM BeanUser WHERE ID = '{0}' LIMIT 1 OFFSET 0", _arrayId[0].ToLowerInvariant().Trim());
                                List<BeanUser> _lstUser = conn.Query<BeanUser>(queryUser);
                                if (_lstUser != null && _lstUser.Count > 0)
                                    _res += _lstUser[0].FullName;
                                else // Nếu ko có user -> searchGroup
                                {
                                    string queryGroup = string.Format(@"SELECT Title FROM BeanGroup WHERE ID = '{0}' LIMIT 1 OFFSET 0", _arrayId[0].ToLowerInvariant().Trim());
                                    List<BeanGroup> _lstGroup = conn.Query<BeanGroup>(queryGroup);
                                    if (_lstGroup != null && _lstGroup.Count > 0)
                                        _res += _lstGroup[0].Title;
                                }

                                if (_arrayId.Length > 2) // thêm ", +n-1"
                                    _res += String.Format(", +{0}", _arrayId.Length - 1);

                            }
                            break;
                        }
                    case (int)CmmFunction.DynamicFieldTypeID.DateAndTime: // Date time
                        {
                            DateTime dateValue = DateTime.Parse(rawData);
                            if (CmmVariable.SysConfig.LangCode.Equals("1033"))
                                _res = dateValue.ToString(CmmVariable.M_WorkDateFormatDateTimeEN).ToLower().Trim();
                            else
                                _res = dateValue.ToString(CmmVariable.M_WorkDateFormatDateTimeVN).ToLower().Trim();
                            break;
                        }
                    case (int)CmmFunction.DynamicFieldTypeID.Lookup:
                    case (int)CmmFunction.DynamicFieldTypeID.ComboBox:
                    case (int)CmmFunction.DynamicFieldTypeID.DropDownList:
                    case (int)CmmFunction.DynamicFieldTypeID.Radio:
                    case (int)CmmFunction.DynamicFieldTypeID.CheckBox:
                    case (int)CmmFunction.DynamicFieldTypeID.Choice:
                    case (int)CmmFunction.DynamicFieldTypeID.Calculated:
                    case (int)CmmFunction.DynamicFieldTypeID.Currency:
                    case (int)CmmFunction.DynamicFieldTypeID.Number:
                    case (int)CmmFunction.DynamicFieldTypeID.MultipleLinesText:
                    case (int)CmmFunction.DynamicFieldTypeID.SingleLineText:
                    default:
                        {
                            _res = rawData;
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CmmFunction - GetFormattedValueByHeader - ERR: " + ex.ToString());
            }
            finally
            {
                conn.Close();
            }
            return _res;
        }
    }

    public static class ExtensionCopy
    {
        #region coppy list
        public static List<T> CopyAll<T>(this List<T> list)
        {
            List<T> ret = new List<T>();
            string tmpStr = JsonConvert.SerializeObject(list);
            ret = JsonConvert.DeserializeObject<List<T>>(tmpStr);
            return ret;
        }
        public static T CopyAll<T>(this T list)
        {
            T ret;
            string tmpStr = JsonConvert.SerializeObject(list);
            ret = JsonConvert.DeserializeObject<T>(tmpStr);
            return ret;
        }
        #endregion
    }
}

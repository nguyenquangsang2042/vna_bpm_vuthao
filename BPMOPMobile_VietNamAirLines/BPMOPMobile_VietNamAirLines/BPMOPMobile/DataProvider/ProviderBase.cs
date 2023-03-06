using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using BPMOPMobile.Class;
using BPMOPMobile.Bean;
using System.Threading.Tasks;

namespace BPMOPMobile.DataProvider
{
    public class ProviderBase
    {
        public class PAR
        {
            public List<KeyValuePair<string, string>> lstGet { get; set; }
            public List<KeyValuePair<string, string>> lstPost { get; set; }
            public List<KeyValuePair<string, string>> lstFile { get; set; }

            public PAR()
            {
            }

            public PAR(List<KeyValuePair<string, string>> lstGet, List<KeyValuePair<string, string>> lstPost = null, List<KeyValuePair<string, string>> lstFile = null)
            {
                this.lstGet = lstGet;
                this.lstPost = lstPost;
                this.lstFile = lstFile;
            }
        }
        //BPM
        private const string MCrypterKey = "MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBAIL+WHEn6D91a7iOrD/iFog7OXU9j6SYLkOVwMfB86A6lXGONCtwpof9BzCtPfMFLbF/tJJlEv5EesDfAyTB5V8CAwEAAQ==";
        //Becamex
        //private const string MCrypterKey = "MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBAI+NO4nW8+n7EIglziaqJ2q3hKArHrVHulvfj6TbOCL7aSEw6EnAb1zHqH3sg7vqulVd4ZAWvTqwr+tgv/2ZGUECAwEAAQ==";

        //private const string MCrypterKey = ""; // Disable RSA

        /// <summary>
        /// Lấy dữ liệu dang Text từ API Server 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="httpClient"></param>
        /// <param name="parSendData"></param>
        /// <returns></returns>
        public string GetStrDataFromAPI(string url, HttpClient httpClient, PAR parSendData = null)
        {
            string retValue = null;

            try
            {
                if (!string.IsNullOrEmpty(MCrypterKey))
                {
                    //GIAO THUC GET KHONG CAN RSA
                    if (parSendData.lstGet == null)
                        parSendData.lstGet = new List<KeyValuePair<string, string>>();

                    if (parSendData.lstPost != null)
                    {
                        KeyValuePair<string, string> encKey = new KeyValuePair<string, string>("enc", "RSA");
                        if (!parSendData.lstGet.Contains(encKey))
                        {
                            parSendData.lstGet.Add(encKey);
                        }
                    }
                }

                if (parSendData != null && parSendData.lstGet != null)
                {
                    string para = "";
                    foreach (KeyValuePair<string, string> keyItem in parSendData.lstGet)
                    {
                        if (url.Contains("?" + keyItem.Key + "=") || url.Contains("&" + keyItem.Key + "="))
                        {
                            continue;
                        }
                        if (para != "") para += "&";
                        para += keyItem.Key + "=" + WebUtility.UrlEncode(keyItem.Value);
                    }

                    if (url.IndexOf("?") > 0)
                    {
                        para = "&" + para;
                    }
                    else
                    {
                        para = "?" + para;
                    }

                    url += para;
                }
#if DEBUG
                Console.WriteLine("API: " + url);
#endif
                HttpContent content = null;

                MultipartFormDataContent contentM = null;
                if (parSendData == null)
                {
                    parSendData = new PAR();
                }
                if ((parSendData.lstPost != null && parSendData.lstPost.Count > 0) || (parSendData.lstFile != null && parSendData.lstFile.Count > 0))
                {

                    contentM = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString("yyyyMMddHHmmss") + "----");

                    if (parSendData.lstPost != null && parSendData.lstPost.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> postItem in parSendData.lstPost)
                        {
                            string encodeData = postItem.Value;
                            if (!string.IsNullOrEmpty(MCrypterKey))
                            {
                                Crypter cry = new Crypter(MCrypterKey);
                                encodeData = cry.EncryptStr(postItem.Value);
                            }
                            HttpContent contentP = new StringContent(encodeData, Encoding.UTF8);//encode list get
                            contentM.Add(contentP, postItem.Key);
                        }
                    }
                    if (parSendData.lstFile != null && parSendData.lstFile.Count > 0)
                    {
                        for (int i = 0; i < parSendData.lstFile.Count; i++)
                        {
                            KeyValuePair<string, string> fileItem = parSendData.lstFile[i];
                            var streamContentF = new StreamContent(File.OpenRead(fileItem.Value));
                            contentM.Add(streamContentF, fileItem.Key, Path.GetFileName(fileItem.Value));
                        }
                    }
                }
                else
                {
                    if (parSendData.lstPost == null) parSendData.lstPost = new List<KeyValuePair<string, string>>();
                    content = new FormUrlEncodedContent(parSendData.lstPost);
                }

                var res = httpClient.PostAsync(url, contentM == null ? content : contentM);
                if (res.Result.StatusCode != HttpStatusCode.OK) return retValue;
                using (var responseStream = new StreamReader(res.Result.Content.ReadAsStreamAsync().Result))
                {
                    string texthtml = responseStream.ReadToEndAsync().Result;
                    responseStream.Dispose();
                    return texthtml;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERR ProviderBase - GetStrDataFromAPI : " + ex.ToString());
            }
            return retValue;
        }

        /// <summary>
        /// Lấy dữ liệu dang Json Object từ API Server 
        /// </summary>
        /// <param name="url">URl lấy dữ liệu</param>
        /// <param name="httpClient">Đối httpClient đã Authern (Nếu yêu cầu) dùng để lấy dữ liệu</param>
        /// <param name="parSendData">Các Tham số gửi liên server</param>
        /// <param name="flgCheckLoginRequie">Biến kiểm tra login, Nếu chưa thì tự động login</param>
        /// <returns></returns>
        public JObject GetJsonDataFromAPI(string url, ref HttpClient httpClient, PAR parSendData = null, bool flgCheckLoginRequie = true)
        {
            JObject retValue = null;

            if (httpClient == null) return null;

            string strData = GetStrDataFromAPI(url, httpClient, parSendData);

            if (strData != null && strData != "")
            {
                bool flgReloginRequie = false;
                if (strData.IndexOf("DOCTYPE html") < 0)
                {
                    retValue = JObject.Parse(strData);
                }

                if (flgCheckLoginRequie)
                {
                    // Kiểm tra hê thống có yêu cầu login ko? nếu có thì thực hiện login
                    if (retValue != null)
                    {
                        string strStatus = retValue.Value<string>("status");
                        if (strStatus.Equals("ERR"))
                        {
                            KeyValuePair<string, string> mess = retValue["mess"].ToObject<KeyValuePair<string, string>>();
                            // Nếu hệ thống yêu cầu tự động login và số lần login lỗi nhỏ hơn Max thì tiếp thực hiện lại relogin
                            if (mess.Key.Equals("998") && CmmVariable.M_AutoReLoginNum < CmmVariable.M_AutoReLoginNumMax)
                            {
                                flgReloginRequie = true;
                            }
                        }
                    }

                    // Thực hiện relogin và tiếp tực thực hiện lấy dữ liệu
                    if (flgReloginRequie)
                    {
                        string loginUrl = CmmVariable.M_Domain;
                        //if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) loginUrl += "/" + CmmVariable.sysConfig.Subsite;
                        HttpClient relogin;
                        if ((relogin = CmmFunction.Login(loginUrl + CmmVariable.M_ApiLogin, CmmVariable.SysConfig.LoginName, CmmVariable.SysConfig.LoginPassword, true, 2)) != null)
                        {
                            httpClient = relogin;
                            retValue = GetJsonDataFromAPI(url, ref httpClient, parSendData);
                        }
                    }
                }
            }

            return retValue;
        }

        /// <summary>
        /// Update toàn bộ dữ Master từ server về
        /// </summary>
        /// <param name="flgChkUpdate"></param>
        /// <param name="dataLimitDay"></param>
        /// <param name="flgGetAll"></param>
        /// <param name="flgReSyns"></param>    
        /// <returns></returns>
        public bool UpdateAllMasterData(bool flgChkUpdate = true, int dataLimitDay = 30, bool flgGetAll = false, bool flgReSyns = false)
        {
            bool retValue = true;
            try
            {
                if ((UpdateMasterData<BeanControlDynamicDetail>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                    retValue = false;
                if ((UpdateMasterData<BeanSettings>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                    retValue = false;
                if ((UpdateMasterData<BeanDepartment>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                    retValue = false;
                if ((UpdateMasterData<BeanWorkflowStatus>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                    retValue = false;
                if ((UpdateMasterData<BeanAppStatus>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                    retValue = false;
            }
            catch (Exception ex)
            {
                retValue = false;
                System.Diagnostics.Debug.Write("ERR UpdateAllMasterData: " + ex.Message);
            }

            return retValue;
        }

        /// <summary>
        /// Update toàn bộ dữ dynamic data từ server về
        /// </summary>
        /// <param name="FlgChkUpdate"></param>
        /// /// <param name="dataLimitDay"></param>
        /// <param name="flgGetAll"></param>
        /// <param name="flgReSyns"></param>
        /// <returns></returns>
        public bool UpdateAllDynamicData(bool flgChkUpdate = true, int dataLimitDay = 30, bool flgGetAll = false, bool flgReSyns = false)
        {
            bool retValue = true;
            try
            {
                //SQLiteConnection con = new SQLiteConnection(CmmVariable.M_DataPath, false);
                if (CmmVariable.SysConfig != null && !string.IsNullOrEmpty(CmmVariable.SysConfig.LoginName) && !string.IsNullOrEmpty(CmmVariable.SysConfig.LoginPassword))
                {
                    ////Master Data
                    /////Không có reference
                    ////if ((UpdateMasterData<BeanControlDynamicDetail>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                    ////    retValue = false;
                    //if ((UpdateMasterData<BeanSettings>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                    //    retValue = false;
                    //if ((UpdateMasterData<BeanDepartment>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                    //    retValue = false;
                    //if ((UpdateMasterData<BeanWorkflowStatus>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                    //    retValue = false;
                    //if ((UpdateMasterData<BeanAppStatus>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                    //    retValue = false;

                    ////Dynamic Data
                    if ((UpdateMasterData<BeanUser>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;
                    if ((UpdateMasterData<BeanGroup>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;
                    //if ((UpdateMasterData<BeanNotify>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                    //    retValue = false;
                    if ((UpdateMasterData<BeanWorkflowItem>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;
                    if ((UpdateMasterData<BeanWorkflow>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;
                    if ((UpdateMasterData<BeanWorkflowCategory>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;
                    if ((UpdateMasterData<BeanWorkflowStepDefine>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;
                    if ((UpdateMasterData<BeanWorkflowFollow>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;
                    if ((UpdateMasterData<BeanTimeLanguage>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                        retValue = false;
                    //if ((UpdateMasterData<BeanPosition>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                    //    retValue = false;
                    //if ((UpdateMasterData<BeanAppBase>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                    //    retValue = false;

                    //if ((UpdateMasterData<BeanResourceView>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                    //    retValue = false;
                    //if (( UpdateMasterData<BeanItemDeleted>(null, flgChkUpdate, dataLimitDay, flgGetAll)) == false)
                    //return false;
                }

                CmmEvent.SyncDataRequest_Performence(null, null);
            }
            catch (Exception ex)
            {
                retValue = false;
                System.Diagnostics.Debug.Write("ERR UpdateAllDynamicData: " + ex.Message);
            }

            return retValue;
        }

        public async Task UpdateDataAppbase()
        {
            SQLiteConnection con = new SQLiteConnection(CmmVariable.M_DataPath, false);
            List<dynamic> lstType = new List<dynamic>();
            List<Task> lstTask = new List<Task>();
            lstTask.Add(Task.Run(() =>
            {
                List<BeanAppBaseExt> lst = LoadMoreDataTFromSerVer(CmmVariable.KEY_GET_FROMME_INPROCESS, 20, 0);
                if (lst != null)
                {
                    lstType.AddRange(lst);
                }
            }));
            lstTask.Add(Task.Run(() =>
            {
                List<BeanAppBaseExt> lst = LoadMoreDataTFromSerVer(CmmVariable.KEY_GET_FROMME_PROCESSED, 20, 0);
                if (lst != null)
                {
                    lstType.AddRange(lst);
                }
            }));
            lstTask.Add(Task.Run(() =>
            {
                List<BeanAppBaseExt> lst = LoadMoreDataTFromSerVer(CmmVariable.KEY_GET_TOME_INPROCESS, 20, 0);
                if (lst != null)
                {
                    lstType.AddRange(lst);
                }
            }));
            lstTask.Add(Task.Run(() =>
            {
                List<BeanAppBaseExt> lst = LoadMoreDataTFromSerVer(CmmVariable.KEY_GET_TOME_PROCESSED, 20, 0);
                if (lst != null)
                {
                    lstType.AddRange(lst);
                }
            }));

            await Task.WhenAll(lstTask);
            con.BeginTransaction();
            Console.WriteLine("Start save DB Time" + DateTime.Now.ToString() + " || " + lstType.Count);
            try
            {
                foreach (var item in lstType)
                    con.InsertOrReplace(item);
                con.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ProviderBase - UpdateDataAppbase - Err: " + ex.ToString());
            }
            finally
            {
                con.Close();
                Console.WriteLine("Done save DB Time" + DateTime.Now.ToString());
            }
        }

        /// <summary>
        /// Update toàn bộ dữ liệu dynamic data từ server về
        /// </summary>
        /// <param name="FlgChkUpdate"></param>
        /// /// <param name="dataLimitDay"></param>
        /// <param name="flgGetAll"></param>
        /// <param name="flgReSyns"></param>
        /// <returns></returns>
        public async Task GetAllDynamicData_FirstLogin(bool flgChkUpdate = true, int dataLimitDay = 30, bool flgGetAll = false, bool flgReSyns = false)
        {
            List<Task> lstTask = new List<Task>();
            //List<dynamic> lstType = new List<dynamic>();
            var busyTimeOut = 60 * 1000;
            try
            {
                if (CmmVariable.SysConfig != null && !string.IsNullOrEmpty(CmmVariable.SysConfig.LoginName) && !string.IsNullOrEmpty(CmmVariable.SysConfig.LoginPassword))
                {
                    lstTask.AddRange(new List<Task>
                    {
                        GetListBeanObj<BeanWorkflow>(flgChkUpdate, dataLimitDay, flgGetAll,busyTimeOut),
                        GetListBeanObj<BeanWorkflowCategory>(flgChkUpdate, dataLimitDay, flgGetAll,busyTimeOut),
                        GetListBeanObj<BeanAppStatus>(flgChkUpdate, dataLimitDay, flgGetAll,busyTimeOut),

                        GetListBeanObj<BeanSettings>(flgChkUpdate, dataLimitDay, flgGetAll,busyTimeOut),
                        GetListBeanObj<BeanWorkflowFollow>(flgChkUpdate, dataLimitDay, flgGetAll,busyTimeOut),
                        GetListBeanObj<BeanTimeLanguage>(flgChkUpdate, dataLimitDay, flgGetAll,busyTimeOut),

                        GetListBeanObj<BeanGroup>(flgChkUpdate, dataLimitDay, flgGetAll,busyTimeOut),
                    });

                    //_ = GetListBeanObj<BeanUser>(flgChkUpdate, dataLimitDay, flgGetAll, busyTimeOut);
                    //_ = GetListBeanObj<BeanWorkflowItem>(flgChkUpdate, dataLimitDay, flgGetAll, busyTimeOut);
                    //_ = GetListBeanObj<BeanWorkflowStepDefine>(flgChkUpdate, dataLimitDay, flgGetAll, busyTimeOut);

                    //_ = GetListBeanObj<BeanAppBase>(flgChkUpdate, dataLimitDay, flgGetAll, busyTimeOut);
                    //_ = GetListBeanObj<BeanPosition>(flgChkUpdate, dataLimitDay, flgGetAll, busyTimeOut);
                    //_ = GetListBeanObj<BeanNotify>(flgChkUpdate, dataLimitDay, flgGetAll, busyTimeOut);

                    _ = Task.Run(() =>
                      {
                          GetListBeanObj<BeanUser>(flgChkUpdate, dataLimitDay, flgGetAll, busyTimeOut, false);
                          GetListBeanObj<BeanWorkflowItem>(flgChkUpdate, dataLimitDay, flgGetAll, busyTimeOut, false);
                          GetListBeanObj<BeanWorkflowStepDefine>(flgChkUpdate, dataLimitDay, flgGetAll, busyTimeOut, false);

                          //System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
                          //CmmEvent.SyncDataRequest_Performence(null, new CmmEvent.SyncDataRequest(true));
                      });
                }
                await Task.WhenAll(lstTask);
                //await UpdateDataAppbase();
                //con.Commit();
                //con.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("ERR GetAllDynamicData_FirstLogin: " + ex.ToString());
                Console.WriteLine("ProviderBase - GetAllDynamicData_FirstLogin - Err: " + ex.ToString());
            }
        }

        //public void GetAllDynamicData_FirstLoginSync(bool flgChkUpdate = true, int dataLimitDay = 30, bool flgGetAll = false, bool flgReSyns = false, int busyTimeOut = 60 * 1000)
        //{
        //    _ = Task.Run(() =>
        //    {
        //        GetListBeanObj<BeanUser>(flgChkUpdate, dataLimitDay, flgGetAll, busyTimeOut);
        //        GetListBeanObj<BeanWorkflowItem>(flgChkUpdate, dataLimitDay, flgGetAll, busyTimeOut);
        //        GetListBeanObj<BeanAppBase>(flgChkUpdate, dataLimitDay, flgGetAll, busyTimeOut);
        //        GetListBeanObj<BeanWorkflowStepDefine>(flgChkUpdate, dataLimitDay, flgGetAll, busyTimeOut);
        //        GetListBeanObj<BeanPosition>(flgChkUpdate, dataLimitDay, flgGetAll, busyTimeOut);
        //        GetListBeanObj<BeanNotify>(flgChkUpdate, dataLimitDay, flgGetAll, busyTimeOut);
        //    });
        //}
        public async Task<bool> UpdateAllDynamicDataAndroid(bool flgChkUpdate = true, int dataLimitDay = 30, bool flgGetAll = false, bool flgReSyns = false)
        {
            bool retValue = true;
            try
            {


                //SQLiteConnection con = new SQLiteConnection(CmmVariable.M_DataPath, false);
                if (CmmVariable.SysConfig != null && !string.IsNullOrEmpty(CmmVariable.SysConfig.LoginName) && !string.IsNullOrEmpty(CmmVariable.SysConfig.LoginPassword))
                {
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    bool updateAllBeanUser = false;
                    List<dynamic> lstType = new List<dynamic>();
                    List<Task> lstTask = new List<Task>();
                    await UpdateDataAppbase();
                    SQLiteConnection con = new SQLiteConnection(CmmVariable.M_DataPath, false);

                    lstTask.Add(Task.Run(() =>
                    {
                        List<BeanControlDynamicDetail> lst = GetApiData_FirstLogin<BeanControlDynamicDetail>(con, flgChkUpdate, dataLimitDay, flgGetAll);
                        if (lst != null)
                        {
                            Console.WriteLine(String.Format("{0} {1} {2}", "BeanControlDynamicDetail", "Insert", lst.Count.ToString()));
                            lstType.AddRange(lst);
                        }

                        CmmEvent.SyncDataBackgroundRequest_Performence(null, new CmmEvent.UpdateBackgroundEventArgs("BeanControlDynamicDetail", lst != null ? lst.Count().ToString() : "0"));

                    }));
                    lstTask.Add(Task.Run(() =>
                    {
                        List<BeanSettings> lst = GetApiData_FirstLogin<BeanSettings>(con, flgChkUpdate, dataLimitDay, flgGetAll);
                        if (lst != null)
                        {
                            Console.WriteLine(String.Format("{0} {1} {2}", "BeanSettings", "Insert", lst.Count.ToString()));
                            lstType.AddRange(lst);
                        }

                        CmmEvent.SyncDataBackgroundRequest_Performence(null, new CmmEvent.UpdateBackgroundEventArgs("BeanSettings", lst != null ? lst.Count().ToString() : "0"));


                    }));
                    lstTask.Add(Task.Run(() =>
                    {
                        List<BeanWorkflowStatus> lst = GetApiData_FirstLogin<BeanWorkflowStatus>(con, flgChkUpdate, dataLimitDay, flgGetAll);
                        if (lst != null)
                        {
                            Console.WriteLine(String.Format("{0} {1} {2}", "BeanWorkflowStatus", "Insert", lst.Count.ToString()));
                            lstType.AddRange(lst);
                        }

                        CmmEvent.SyncDataBackgroundRequest_Performence(null, new CmmEvent.UpdateBackgroundEventArgs("BeanWorkflowStatus", lst != null ? lst.Count().ToString() : "0"));


                    }));
                    lstTask.Add(Task.Run(() =>
                    {
                        List<BeanAppStatus> lst = GetApiData_FirstLogin<BeanAppStatus>(con, flgChkUpdate, dataLimitDay, flgGetAll);
                        if (lst != null)
                        {
                            Console.WriteLine(String.Format("{0} {1} {2}", "BeanAppStatus", "Insert", lst.Count.ToString()));
                            lstType.AddRange(lst);
                        }

                        CmmEvent.SyncDataBackgroundRequest_Performence(null, new CmmEvent.UpdateBackgroundEventArgs("BeanAppStatus", lst != null ? lst.Count().ToString() : "0"));


                    }));
                    lstTask.Add(Task.Run(() =>
                    {
                        List<BeanPosition> lst = GetApiData_FirstLogin<BeanPosition>(con, flgChkUpdate, dataLimitDay, flgGetAll);
                        if (lst != null)
                        {
                            Console.WriteLine(String.Format("{0} {1} {2}", "BeanPosition", "Insert", lst.Count.ToString()));
                            lstType.AddRange(lst);
                        }

                        CmmEvent.SyncDataBackgroundRequest_Performence(null, new CmmEvent.UpdateBackgroundEventArgs("BeanPosition", lst != null ? lst.Count().ToString() : "0"));


                    }));
                    lstTask.Add(Task.Run(() =>
                    {
                        List<BeanTimeLanguage> lst = GetApiData_FirstLogin<BeanTimeLanguage>(con, flgChkUpdate, dataLimitDay, flgGetAll);
                        if (lst != null)
                        {
                            Console.WriteLine(String.Format("{0} {1} {2}", "BeanTimeLanguage", "Insert", lst.Count.ToString()));
                            lstType.AddRange(lst);
                        }

                        CmmEvent.SyncDataBackgroundRequest_Performence(null, new CmmEvent.UpdateBackgroundEventArgs("BeanTimeLanguage", lst != null ? lst.Count().ToString() : "0"));


                    }));
                    lstTask.Add(Task.Run(() =>
                    {
                        List<BeanUser> lst = GetApiData_FirstLogin<BeanUser>(con, flgChkUpdate, dataLimitDay, flgGetAll);
                        if (lst != null)
                        {
                            Console.WriteLine(String.Format("{0} {1} {2}", "BeanUser", "Insert", lst.Count.ToString()));
                            lstType.AddRange(lst);
                        }


                        CmmEvent.SyncDataBackgroundRequest_Performence(null, new CmmEvent.UpdateBackgroundEventArgs("BeanUser", lst != null ? lst.Count().ToString() : "0"));

                    }));
                    lstTask.Add(Task.Run(() =>
                    {
                        List<BeanGroup> lst = GetApiData_FirstLogin<BeanGroup>(con, flgChkUpdate, dataLimitDay, flgGetAll);
                        if (lst != null)
                        {
                            Console.WriteLine(String.Format("{0} {1} {2}", "BeanGroup", "Insert", lst.Count.ToString()));
                            lstType.AddRange(lst);
                        }

                        CmmEvent.SyncDataBackgroundRequest_Performence(null, new CmmEvent.UpdateBackgroundEventArgs("BeanGroup", lst != null ? lst.Count().ToString() : "0"));


                    }));
                    lstTask.Add(Task.Run(() =>
                    {
                        List<BeanNotify> lst = GetApiData_FirstLogin<BeanNotify>(con, flgChkUpdate, dataLimitDay, flgGetAll);
                        if (lst != null)
                        {
                            Console.WriteLine(String.Format("{0} {1} {2}", "BeanNotify", "Insert", lst.Count.ToString()));
                            lstType.AddRange(lst);
                        }

                        CmmEvent.SyncDataBackgroundRequest_Performence(null, new CmmEvent.UpdateBackgroundEventArgs("BeanNotify", lst != null ? lst.Count().ToString() : "0"));


                    }));
                    /* lstTask.Add(Task.Run(() =>
                     {
                         List<BeanWorkflowItem> lst = GetApiData_FirstLogin<BeanWorkflowItem>(con, flgChkUpdate, dataLimitDay, flgGetAll);
                         if (lst != null)
                         {
                             Console.WriteLine(String.Format("{0} {1} {2}", "BeanWorkflowItem", "Insert", lst.Count.ToString()));
                             lstType.AddRange(lst);
                         }
                         Thread.Sleep(500);

                         CmmEvent.SyncDataBackgroundRequest_Performence(null, new CmmEvent.UpdateBackgroundEventArgs("BeanWorkflowItem", lst != null ? lst.Count().ToString() : "0"));


                     }));*/
                    lstTask.Add(Task.Run(() =>
                    {
                        List<BeanWorkflow> lst = GetApiData_FirstLogin<BeanWorkflow>(con, flgChkUpdate, dataLimitDay, flgGetAll);
                        if (lst != null)
                        {
                            Console.WriteLine(String.Format("{0} {1} {2}", "BeanWorkflow", "Insert", lst.Count.ToString()));
                            lstType.AddRange(lst);
                        }

                        CmmEvent.SyncDataBackgroundRequest_Performence(null, new CmmEvent.UpdateBackgroundEventArgs("BeanWorkflow", lst != null ? lst.Count().ToString() : "0"));


                    }));
                    lstTask.Add(Task.Run(() =>
                    {
                        List<BeanWorkflowCategory> lst = GetApiData_FirstLogin<BeanWorkflowCategory>(con, flgChkUpdate, dataLimitDay, flgGetAll);
                        if (lst != null)
                        {
                            Console.WriteLine(String.Format("{0} {1} {2}", "BeanWorkflowCategory", "Insert", lst.Count.ToString()));
                            lstType.AddRange(lst);
                        }

                        CmmEvent.SyncDataBackgroundRequest_Performence(null, new CmmEvent.UpdateBackgroundEventArgs("BeanWorkflowCategory", lst != null ? lst.Count().ToString() : "0"));


                    }));
                    lstTask.Add(Task.Run(() =>
                    {
                        List<BeanWorkflowStepDefine> lst = GetApiData_FirstLogin<BeanWorkflowStepDefine>(con, flgChkUpdate, dataLimitDay, flgGetAll);
                        if (lst != null)
                        {
                            Console.WriteLine(String.Format("{0} {1} {2}", "BeanWorkflowStepDefine", "Insert", lst.Count.ToString()));
                            lstType.AddRange(lst);
                        }

                        CmmEvent.SyncDataBackgroundRequest_Performence(null, new CmmEvent.UpdateBackgroundEventArgs("BeanWorkflowStepDefine", lst != null ? lst.Count().ToString() : "0"));

                    }));
                    lstTask.Add(Task.Run(() =>
                    {
                        List<BeanWorkflowFollow> lst = GetApiData_FirstLogin<BeanWorkflowFollow>(con, flgChkUpdate, dataLimitDay, flgGetAll);
                        if (lst != null)
                        {
                            Console.WriteLine(String.Format("{0} {1} {2}", "BeanWorkflowFollow", "Insert", lst.Count.ToString()));
                            lstType.AddRange(lst);
                        }

                        CmmEvent.SyncDataBackgroundRequest_Performence(null, new CmmEvent.UpdateBackgroundEventArgs("BeanWorkflowFollow", lst != null ? lst.Count().ToString() : "0"));

                    }));
                    /* lstTask.Add(Task.Run(() =>
                     {
                         List<BeanAppBase> lst = GetApiData_FirstLogin<BeanAppBase>(con, flgChkUpdate, dataLimitDay, flgGetAll);
                         if (lst != null)
                         {
                             Console.WriteLine(String.Format("{0} {1} {2}", "BeanAppBase", "Insert", lst.Count.ToString()));
                             lstType.AddRange(lst);
                         }
                         Thread.Sleep(500);

                         CmmEvent.SyncDataBackgroundRequest_Performence(null, new CmmEvent.UpdateBackgroundEventArgs("BeanAppBase", lst != null ? lst.Count().ToString() : "0"));

                     }));*/
                    lstTask.Add(Task.Run(() =>
                    {
                        List<BeanResourceView> lst = GetApiData_FirstLogin<BeanResourceView>(con, flgChkUpdate, dataLimitDay, flgGetAll);
                        if (lst != null)
                        {
                            Console.WriteLine(String.Format("{0} {1} {2}", "BeanResourceView", "Insert", lst.Count.ToString()));
                            lstType.AddRange(lst);
                        }

                        CmmEvent.SyncDataBackgroundRequest_Performence(null, new CmmEvent.UpdateBackgroundEventArgs("BeanResourceView", lst != null ? lst.Count().ToString() : "0"));

                    }));
                    lstTask.Add(Task.Run(() =>
                    {
                        List<BeanDepartment> lst = GetApiData_FirstLogin<BeanDepartment>(con, flgChkUpdate, dataLimitDay, flgGetAll);
                        if (lst != null)
                        {
                            Console.WriteLine(String.Format("{0} {1} {2}", "BeanDepartment", "Insert", lst.Count.ToString()));
                            lstType.AddRange(lst);
                        }
                        CmmEvent.SyncDataBackgroundRequest_Performence(null, new CmmEvent.UpdateBackgroundEventArgs("BeanDepartment", lst != null ? lst.Count().ToString() : "0"));


                    }));

                    await Task.WhenAll(lstTask);
                    con.BeginTransaction();
                    Console.WriteLine("Start Time" + DateTime.Now.ToString() + " || " + lstType.Count);
                    con.InsertAll(lstType, true);
                    con.Commit();
                    con.Close();
                    watch.Stop();
                    var elapsedMs = watch.ElapsedMilliseconds;
                    Console.WriteLine(" End Time" + DateTime.Now.ToString());
                    Console.WriteLine(String.Format(" Time update: {0} s", elapsedMs / 1000));
                    CmmEvent.SyncDataBackgroundRequest_Performence(null, new CmmEvent.UpdateBackgroundEventArgs("Done", "All"));

                }

                CmmEvent.SyncDataRequest_Performence(null, null);
            }
            catch (Exception ex)
            {
                retValue = false;
                System.Diagnostics.Debug.Write("ERR UpdateAllDynamicData: " + ex.Message);
            }

            return retValue;
        }

        Task GetListBeanObj<T>(bool flgChkUpdate = true, int dataLimitDay = 30, bool flgGetAll = false, int busyTimeOut = 0, bool doTickEvent = true)
        {
            SQLiteConnection con = new SQLiteConnection(CmmVariable.M_DataPath, false);
            SQLite3.BusyTimeout(con.Handle, busyTimeOut);
            con.BeginTransaction();
            List<T> lst = GetApiData_FirstLogin<T>(con, flgChkUpdate, dataLimitDay, flgGetAll);
            if (lst != null && lst.Count > 0)
            {
                //con.InsertAll(lst, false);
                foreach (var o in lst)
                {
                    con.InsertOrReplace(o);
                }
            }
            con.Commit();
            con.Close();
            CmmEvent.SyncDataBackgroundRequest_Performence(null, new CmmEvent.UpdateBackgroundEventArgs(typeof(T).Name, lst != null ? lst.Count().ToString() : CmmEvent.errMess));
            if (doTickEvent) CmmEvent.SyncDataRequest_Performence(null, new CmmEvent.SyncDataRequest(true));
            return Task.CompletedTask;
        }

        private List<T> GetApiData_FirstLogin<T>(SQLiteConnection con = null, bool flgChkUpdate = true, int dataLimitDay = 30, bool flgGetAll = false, bool flgDeleteOldData = false, PAR extPara = null)
        {
            bool resValue = false;
            List<T> lstInsertItem = new List<T>();

            try
            {
                Type type = typeof(T);
                string ID = type.Name;
                string Modified = "";

                if (flgGetAll)
                {
                    if (type.Name == "BeanControlDynamicDetail"
                    || type.Name == "BeanDepartment"
                    || type.Name == "BeanUser"
                    || type.Name == "BeanSettings"
                    || type.Name == "BeanWorkflowStatus"
                    || type.Name == "BeanAppStatus"
                    ///xóa trong tương lai
                    || type.Name == "BeanNotify"
                    || type.Name == "BeanAppBase"
                    || type.Name == "BeanWorkFlowItem")
                        Modified = string.Empty;
                    else
                        Modified = DateTime.Now.AddDays(dataLimitDay * -1).ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    TableQuery<DBVariable> table = con.Table<DBVariable>();
                    var items = from i in table
                                where i.Id == ID
                                select i;
                    if (items.Count() > 0)
                    {
                        Modified = items.First().Value;
                    }
                    else
                    {
                        Modified = DateTime.Now.AddDays(dataLimitDay * -1).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }

                BeanBase objMst = (BeanBase)(Activator.CreateInstance(type));

                string combieUrl = CmmVariable.M_Domain;

                if (extPara == null)
                {
                    extPara = new PAR();
                }
                List<KeyValuePair<string, string>> lstGet = extPara.lstGet;

                if (lstGet == null)
                {
                    lstGet = new List<KeyValuePair<string, string>>();
                }

                lstGet.Add(new KeyValuePair<string, string>("Modified", Modified));
                lstGet.Add(new KeyValuePair<string, string>("isFirst", flgGetAll ? "1" : "0"));
                extPara.lstGet = lstGet;
                string apiServerUrl = objMst.GetServerUrl();

                JObject retData = GetJsonDataFromAPI(combieUrl + apiServerUrl, ref CmmVariable.M_AuthenticatedHttpClient, extPara, true);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus == null || !strStatus.Equals("SUCCESS"))
                {
                    string err = retData.Value<JObject>("mess")["Key"].ToString();
                    Console.WriteLine("BeanName: " + ID + "VuThaoError: " + err);

                    if (err == "010")
                        return null;
                    else
                        return null;
                }

                List<T> lstMstData = retData["data"].ToObject<List<T>>();
                String LocalKeyName = BeanBase.GetPriKey(typeof(T))[0];
                String serverKeyName = BeanBase.GetPriKeyS(typeof(T))[0];

                if (lstMstData == null || lstMstData.Count == 0) return null;

                List<string> lstHtmlDecode = BeanBase.GetLstProName(type, typeof(HtmlEncodeAttribute));
                List<string> lstHtmlRemove = BeanBase.GetLstProName(type, typeof(HtmlRemoveAttribute));

                // Lay danh sach cac pro kieu DateTime
                PropertyInfo[] arrProDateTime = CmmFunction.GetPropertysWithType(type, typeof(DateTime?));

                if (type.Name != "BeanTaskDeleted")
                {
                    if (flgDeleteOldData)
                    {
                        con.Execute("DELETE FROM " + type.Name);
                    }
                    foreach (T item in lstMstData)
                    {
                        // Decode Dữ liệu bị encode html
                        foreach (string decodeFieldName in lstHtmlDecode)
                        {
                            object decodeValue = CmmFunction.GetPropertyValueByName(item, decodeFieldName);
                            if (decodeValue != null)
                            {
                                decodeValue = WebUtility.HtmlDecode(decodeValue + string.Empty);
                                CmmFunction.SetPropertyValueByName(item, decodeFieldName, decodeValue);
                            }
                        }
                        // Remove Dữ liệu có html tag
                        foreach (string decodeFieldName in lstHtmlRemove)
                        {
                            PropertyInfo propInfo = CmmFunction.GetProperty(item, decodeFieldName);



                            object decodeValue = CmmFunction.GetPropertyValue(item, propInfo);
                            if (decodeValue != null)
                            {
                                decodeValue = Regex.Replace(decodeValue + "", "<.*?>", String.Empty);
                                CmmFunction.SetPropertyValue(item, propInfo, decodeValue);
                            }
                        }
                        //kiem tra convert datetime to UTC
                        if (arrProDateTime != null)
                        {
                            foreach (PropertyInfo pro in arrProDateTime)
                            {
                                object objValue = CmmFunction.GetPropertyValue(item, pro);



                                if (objValue != null)
                                {
                                    //DateTime objDateTime = (DateTime)objValue;
                                    //objDateTime.ToUniversalTime().AddHours(-8);
                                    DateTime objDateTime = ((DateTime)objValue).AddHours(CmmVariable.M_DiffHours);
                                    CmmFunction.SetPropertyValueByName(item, pro.Name, objDateTime);
                                }
                            }
                        }
                    }

                    lstInsertItem = lstMstData;
                }

                string sysDateNow = retData.Value<string>("dateNow");
                if (string.IsNullOrEmpty(sysDateNow))
                {
                    return lstInsertItem;
                }
                //CmmFunction.UpdateDBVariable(new DBVariable(ID, sysDateNow), con);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERR UpdateMasterData: - " + typeof(T).Name + " - " + ex.Message);
                System.Diagnostics.Debug.Write("ERR UpdateMasterData: " + ex.Message);
                return null;
            }
            finally
            {
            }
            return lstInsertItem;
        }

        /// <summary>
        /// Upload Dữ liệu 1 bảng Master từ server
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="con"></param>
        /// <param name="flgChkUpdate"></param>
        /// <param name="dataLimitDay"></param>
        /// <param name="flgGetAll"></param>
        /// <param name="flgDeleteOldData"></param>
        /// <param name="extPara"></param>
        public bool UpdateMasterData<T>(SQLiteConnection con = null, bool flgChkUpdate = true, int dataLimitDay = 30, bool flgGetAll = false, bool flgDeleteOldData = false, PAR extPara = null)
        {
            bool resValue = false;
            try
            {
                Type type = typeof(T);
                string ID = type.Name;
                string Modified = "";
                if (con == null)
                {
                    con = new SQLiteConnection(CmmVariable.M_DataPath);
                }
                if (flgGetAll)
                {
                    //Modified = DateTime.Now.AddDays(dataLimitDay * -1).ToString("yyyy-MM-dd HH:mm:ss");
                    if (type.Name == "BeanControlDynamicDetail"
                        || type.Name == "BeanDepartment"
                        || type.Name == "BeanUser"
                        || type.Name == "BeanSettings"
                        || type.Name == "BeanWorkflowStatus"
                        || type.Name == "BeanAppStatus")
                        Modified = string.Empty;
                    else
                        Modified = DateTime.Now.AddDays(dataLimitDay * -1).ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    TableQuery<DBVariable> table = con.Table<DBVariable>();
                    var items = from i in table
                                where i.Id == ID
                                select i;
                    if (items.Count() > 0)
                    {
                        Modified = items.First().Value;
                    }
                    else
                    {
                        Modified = DateTime.Now.AddDays(dataLimitDay * -1).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }

                BeanBase objMst = (BeanBase)(Activator.CreateInstance(type));

                string combieUrl = CmmVariable.M_Domain;
                //if (!string.IsNullOrEmpty(CmmVariable.sysConfig.Subsite)) combieUrl += "/" + CmmVariable.sysConfig.Subsite + "/";
                //combieUrl += CmmVariable.M_ApiPath;

                if (extPara == null)
                {
                    extPara = new PAR();
                }
                List<KeyValuePair<string, string>> lstGet = extPara.lstGet;

                if (lstGet == null)
                {
                    lstGet = new List<KeyValuePair<string, string>>();
                }

                lstGet.Add(new KeyValuePair<string, string>("Modified", Modified));
                lstGet.Add(new KeyValuePair<string, string>("isFirst", flgGetAll ? "1" : "0"));
                extPara.lstGet = lstGet;
                string apiServerUrl = objMst.GetServerUrl();
                //string SiteName = CmmVariable.sysConfig.SiteName;
                //if (CmmVariable.sysConfig.SiteName.Contains(";"))
                //{
                //    SiteName = CmmVariable.sysConfig.SiteName.Replace(";", " ").TrimStart();
                //}
                //ApiServerUrl = ApiServerUrl.Replace("<#SiteName#>", string.IsNullOrEmpty(CmmVariable.sysConfig.SiteName) ? "" : ("/" + CmmVariable.sysConfig.SiteName));
                JObject retData = GetJsonDataFromAPI(combieUrl + apiServerUrl, ref CmmVariable.M_AuthenticatedHttpClient, extPara, true);
                if (retData == null) return false;

                string strStatus = retData.Value<string>("status");
                if (strStatus == null || !strStatus.Equals("SUCCESS"))
                {
                    string err = retData.Value<JObject>("mess")["Key"].ToString();
                    if (err == "010")
                        return true;
                    else
                        return false;
                }

                List<T> lstMstData = retData["data"].ToObject<List<T>>();
                String LocalKeyName = BeanBase.GetPriKey(typeof(T))[0];
                String serverKeyName = BeanBase.GetPriKeyS(typeof(T))[0];

                if (lstMstData == null || lstMstData.Count == 0) return true;

                List<T> lstInsertItem = new List<T>();
                List<T> lstUpdateItem = new List<T>();


                List<string> lstHtmlDecode = BeanBase.GetLstProName(type, typeof(HtmlEncodeAttribute));
                List<string> lstHtmlRemove = BeanBase.GetLstProName(type, typeof(HtmlRemoveAttribute));

                // Lay danh sach cac pro kieu DateTime
                PropertyInfo[] arrProDateTime = CmmFunction.GetPropertysWithType(type, typeof(DateTime?));

                if (flgChkUpdate)
                {
                    foreach (T item in lstMstData)
                    {
                        string sqlSel;
                        // Thực hiện xóa item
                        if (type.Name == "BeanItemDeleted")
                        {
                            BeanItemDeleted itemDelete = CmmFunction.ChangeToRealType<BeanItemDeleted>(item);

                            if (!string.IsNullOrEmpty(itemDelete.BeanName))
                            {
                                try
                                {
                                    sqlSel = string.Format("DELETE FROM {0} WHERE {1} = ?", itemDelete.BeanName, itemDelete.Key);
                                    con.Execute(sqlSel, itemDelete.Value.ToLower());
                                }
                                catch (Exception)
                                {
                                    // ignored
                                }
                            }
                            continue;
                        }

                        // Decode Dữ liệu bị encode html
                        foreach (string decodeFieldName in lstHtmlDecode)
                        {
                            PropertyInfo propInfo = CmmFunction.GetProperty(item, decodeFieldName);

                            object decodeValue = CmmFunction.GetPropertyValue(item, propInfo);
                            if (decodeValue != null)
                            {
                                decodeValue = WebUtility.HtmlDecode(decodeValue + string.Empty);
                                CmmFunction.SetPropertyValue(item, propInfo, decodeValue);
                            }
                        }

                        // Remove Dữ liệu có html tag
                        foreach (string decodeFieldName in lstHtmlRemove)
                        {
                            PropertyInfo propInfo = CmmFunction.GetProperty(item, decodeFieldName);

                            object decodeValue = CmmFunction.GetPropertyValue(item, propInfo);
                            if (decodeValue != null)
                            {
                                decodeValue = Regex.Replace(decodeValue + "", "<.*?>", String.Empty);
                                CmmFunction.SetPropertyValue(item, propInfo, decodeValue);
                            }
                        }

                        //kiem tra convert datetime to UTC
                        if (arrProDateTime != null)
                        {

                            foreach (PropertyInfo pro in arrProDateTime)
                            {
                                object objValue = CmmFunction.GetPropertyValue(item, pro);
                                if (objValue != null)
                                {
                                    DateTime objDateTime = ((DateTime)objValue).AddHours(CmmVariable.M_DiffHours);
                                    CmmFunction.SetPropertyValueByName(item, pro.Name, objDateTime);
                                }
                            }

                        }

                        // Kiểm tra nếu tồn tại rồi thì update
                        sqlSel = string.Format("SELECT * FROM {0} WHERE {1} = ?", type.Name, serverKeyName);
                        List<object> lstObjChk = con.Query(new TableMapping(type), sqlSel, CmmFunction.GetPropertyValueByName(item, serverKeyName));

                        if (lstObjChk.Count > 0)
                        {
                            T objChk = (T)lstObjChk[0];
                            CmmFunction.SetPropertyValueByName(item, LocalKeyName, CmmFunction.GetPropertyValueByName(objChk, LocalKeyName));

                            // Xóa bỏ MobileKeyDiff
                            if (type.Name == "BeanTask")
                            {
                                CmmFunction.SetPropertyValueByName(item, "MobileKeyDiff", "");
                            }

                            //con.Update(item);
                            lstUpdateItem.Add(item);
                        }
                        else// Nếu không tồn tại thì Insert
                        {

                            if (type.Name == "BeanTask")
                            {
                                object MobileKeyDiff = CmmFunction.GetPropertyValueByName(item, "MobileKeyDiff");

                                // Neu khong co diffket thi thuc hien insert
                                if (MobileKeyDiff == null || string.IsNullOrEmpty(MobileKeyDiff.ToString()))
                                {
                                    //con.Insert(item);
                                    lstInsertItem.Add(item);
                                    continue;
                                }

                                sqlSel = "SELECT * FROM BeanTask WHERE MobileKeyDiff = ?";
                                lstObjChk = con.Query(new TableMapping(type), sqlSel, MobileKeyDiff.ToString());

                                // Xóa bỏ MobileKeyDiff
                                CmmFunction.SetPropertyValueByName(item, "MobileKeyDiff", "");
                                if (lstObjChk.Count > 0)
                                {
                                    T objChk = (T)lstObjChk[0];
                                    CmmFunction.SetPropertyValueByName(item, LocalKeyName, CmmFunction.GetPropertyValueByName(objChk, LocalKeyName));

                                    //con.Update(item);
                                    lstUpdateItem.Add(item);
                                }
                                else
                                {
                                    //con.Insert(item);
                                    lstInsertItem.Add(item);
                                }
                            }
                            else
                            {
                                //con.Insert(item);
                                lstInsertItem.Add(item);
                            }

                        }
                    }
                }
                else //insert
                {
                    if (type.Name != "BeanTaskDeleted")
                    {

                        if (flgDeleteOldData)
                        {
                            con.Execute("DELETE FROM " + type.Name);
                        }

                        foreach (T item in lstMstData)
                        {
                            // Decode Dữ liệu bị encode html
                            foreach (string decodeFieldName in lstHtmlDecode)
                            {
                                object decodeValue = CmmFunction.GetPropertyValueByName(item, decodeFieldName);
                                if (decodeValue != null)
                                {
                                    decodeValue = WebUtility.HtmlDecode(decodeValue + string.Empty);
                                    CmmFunction.SetPropertyValueByName(item, decodeFieldName, decodeValue);
                                }
                            }

                            // Remove Dữ liệu có html tag
                            foreach (string decodeFieldName in lstHtmlRemove)
                            {
                                PropertyInfo propInfo = CmmFunction.GetProperty(item, decodeFieldName);

                                object decodeValue = CmmFunction.GetPropertyValue(item, propInfo);
                                if (decodeValue != null)
                                {
                                    decodeValue = Regex.Replace(decodeValue + "", "<.*?>", String.Empty);
                                    CmmFunction.SetPropertyValue(item, propInfo, decodeValue);
                                }
                            }

                            //kiem tra convert datetime to UTC
                            if (arrProDateTime != null)
                            {
                                foreach (PropertyInfo pro in arrProDateTime)
                                {
                                    object objValue = CmmFunction.GetPropertyValue(item, pro);

                                    if (objValue != null)
                                    {
                                        //DateTime objDateTime = (DateTime)objValue;
                                        //objDateTime.ToUniversalTime().AddHours(-8);
                                        DateTime objDateTime = ((DateTime)objValue).AddHours(CmmVariable.M_DiffHours);
                                        CmmFunction.SetPropertyValueByName(item, pro.Name, objDateTime);
                                    }
                                }
                            }
                        }

                        lstInsertItem = lstMstData;
                    }
                }

                string sysDateNow = retData.Value<string>("dateNow");
                if (string.IsNullOrEmpty(sysDateNow)) return resValue;

                con.BeginTransaction();

                if (lstInsertItem.Count > 0)
                    con.InsertAll(lstInsertItem, false);
                if (lstUpdateItem.Count > 0)
                    con.UpdateAll(lstUpdateItem, false);

                CmmFunction.UpdateDBVariable(new DBVariable(ID, sysDateNow), con);
                con.Commit();

                //con.Close();
                resValue = true;
            }
            catch (Exception ex)
            {
                resValue = false;
                if (con.IsInTransaction)
                    con.Rollback();
                Console.WriteLine("ERR UpdateMasterData: - " + typeof(T).Name + " - " + ex.Message);
                System.Diagnostics.Debug.Write("ERR UpdateMasterData: " + ex.Message);
            }
            finally
            {
                con.Close();
            }

            return resValue;
        }

        /// <summary>
        /// Cập nhật lại biến trong Table DBVariable
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="con"></param>
        /// <returns></returns>
        public bool UpdateDbVariable(DBVariable variable, SQLiteConnection con = null)
        {
            try
            {
                if (con == null)
                {
                    con = new SQLiteConnection(CmmVariable.M_DataPath);
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
            catch (Exception)
            {
                // ignored
            }
            return false;
        }

        /// <summary>
        /// Download File ffrom server
        /// </summary>
        /// <param name="url"></param>
        /// <param name="localPath"></param>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        public bool DownloadFile(string url, string localPath, HttpClient httpClient)
        {
            try
            {
                Uri filepath = new Uri(url);
                if (httpClient != null)
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, filepath);
                    request.Headers.Add("ACCEPT", "*/*");
                    HttpResponseMessage response = httpClient.SendAsync(request).Result;
                    if (response.StatusCode != HttpStatusCode.OK) return false;
                    byte[] byteArray = response.Content.ReadAsByteArrayAsync().Result;
                    File.WriteAllBytes(localPath, byteArray);
                }
                return true;
            }
            catch (Exception ex)
            {
                if (File.Exists(localPath))
                {
                    try
                    {
                        File.Delete(localPath);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
                Console.WriteLine("ProviderBase - DownloadFile - Err: " + ex);
                return false;
            }
        }

        /// <summary>
        /// Load more data - neu trong DB da load het thi load them tu server
        /// </summary>
        /// <returns><c>true</c>, if more data was loaded, <c>false</c> otherwise.</returns>
        public List<T> LoadMoreData<T>(string _querry, int limit, int _offset, SQLiteConnection con = null)
        {

            Type type = typeof(T);
            BeanBase objMst = (BeanBase)(Activator.CreateInstance(type));
            List<T> retValue = new List<T>();
            try
            {
                if (con == null)
                    con = new SQLiteConnection(CmmVariable.M_DataPath);

                //string querry_more = string.Format(_querry + "LIMIT ? OFFSET ?");
                //List<object> lstObjChk = con.Query(new TableMapping(type), querry_more, limit, offset);
                string querry_more = string.Format(_querry);
                List<T> lstObjChk = con.Query<T>(querry_more, limit, _offset);
                con.Close();

                if (lstObjChk != null && lstObjChk.Count > 0 && lstObjChk.Count <= limit) // load more from server
                {
                    retValue = lstObjChk.Cast<T>().ToList();
                    //List<T> lstGetServer = GetMoreData<T>(null, limit);
                    //lstObjChk.AddRange(lstGetServer);
                }
                return retValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR - LoadMoreData: " + ex.ToString());
            }

            return null;
        }
        /// <summary>
        ///  Load more data - neu trong DB
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Querry"></param>
        /// <param name="Limit">truyền limit 1 lần lấy thêm dữ liệu</param>
        /// <param name="Args"> params các giá trị biến cần gán vào </param>
        /// <returns></returns>
        public List<T> LoadMoreDataT<T>(string Querry, int Limit, params object[] Args)
        {
            List<T> retValue = new List<T>();
            try
            {
                SQLiteConnection con = new SQLiteConnection(CmmVariable.M_DataPath);

                string querryMore = string.Format(Querry);
                List<T> lstObjChk = con.Query<T>(querryMore, Args);

                if (lstObjChk != null && lstObjChk.Count > 0 && lstObjChk.Count <= Limit) // load more from server
                {
                    retValue = lstObjChk.ToList();
                    //List<T> lstGetServer = GetMoreData<T>(null, limit);
                    //lstObjChk.AddRange(lstGetServer);
                }
                return retValue;
            }
            catch (Exception ex) { Console.WriteLine("ERROR - LoadMoreData: " + ex); }

            return null;
        }

        /// <summary>
        /// Gets the modify start.
        /// </summary>
        /// <returns>The modify start.</returns>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static string getModifyStart<T>()
        {
            string retValue = "";

            SQLiteConnection con = new SQLiteConnection(CmmVariable.M_DataPath);

            TableQuery<DBVariable> table = con.Table<DBVariable>();
            string temp = typeof(T).Name + "Start";
            var items = from i in table
                        where i.Id == temp
                        select i;

            if (items.Count() > 0)
                retValue = items.First().Value;

            return retValue;
        }

        /// <summary>
        ///  Load more data - Từ server
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lcid">mã ngôn ngữ</param>
        /// <param name="Offset">truyền limit 1 lần lấy thêm dữ liệu</param>
        /// <param name="Limit">truyền limit 1 lần lấy thêm dữ liệu</param>
        /// <param name="FuncName"> params các giá trị biến cần gán vào </param>
        /// <returns></returns>
        public List<BeanAppBaseExt> LoadMoreDataTFromSerVer(string FuncName = "", int limit = 20, int offset = 0)
        {
            List<BeanAppBaseExt> retValue = new List<BeanAppBaseExt>();
            try
            {
                PAR par = new PAR(null, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.SysConfig.Subsite)) combieUrl += "/" + CmmVariable.SysConfig.Subsite;
                ///_layouts/15/VuThao.BPMOP.API/ApiResourceView.ashx?func=GetListNotifyItem&lcid=1066&data={"funcName":"AssignedToMeAll","type":0,"limit":"20","offset":0}
                string url_site = string.Format("{0}{1}{2}&data={3}",
                    combieUrl,
                    "/_layouts/15/VuThao.BPMOP.API/ApiResourceView.ashx?func=GetListNotifyItem&lcid=",
                    CmmVariable.SysConfig.LangCode,
                    "{" + string.Format("\"funcName\":\"{0}\",", FuncName) + "\"type\":\"0\"," + string.Format("\"limit\":\"{0}\",", limit) + string.Format("\"offset\":{0}", offset) + "}"
                    );
                JObject retData = GetJsonDataFromAPI(url_site, ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return retValue;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    retValue = JsonConvert.DeserializeObject<List<BeanAppBaseExt>>(retData["data"].ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR - LoadMoreData from server: " + ex);
            }

            return retValue;
        }

        /// <summary>
        /// Update từng item xuống DB
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="BeanAppBaseExt">Item dữ liệu</param>
        /// <returns></returns>
        public void UpdateItemDataNewLoading(BeanAppBaseExt itemAppBase = null, SQLiteConnection conn = null)
        {
            try
            {
                string query = "Select * from BeanAppBaseExt where ID = {0}";

                string queryItem = string.Format(query, itemAppBase.ID);
                BeanAppBaseExt lst = conn.Query<BeanAppBaseExt>(queryItem).FirstOrDefault();
                if (lst != null)// && lst.Count() != 0
                {
                    conn.Update(itemAppBase);
                }
                else
                {
                    conn.Insert(itemAppBase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdateDataNewLoading() " + ex.ToString());
            }
        }

        public List<BeanAppBaseExt> getAllItemKanBan(string workflowID)
        {
            List<BeanAppBaseExt> retValue = new List<BeanAppBaseExt>();
            try
            {
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();
                object data = new
                {
                    Limit = -1,
                    Key = "",
                    FromDate = "",
                    ToDate = "",
                    WorkflowId = workflowID,
                    Status = "0",
                    UserAss = ""
                };

                lstPost.Add(new KeyValuePair<string, string>("data", JsonConvert.SerializeObject(data)));
                PAR par = new PAR(null, lstPost);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.SysConfig.Subsite)) combieUrl += "/" + CmmVariable.SysConfig.Subsite;

                JObject retData = GetJsonDataFromAPI(combieUrl + "/_layouts/15/VuThao.BPMOP.API/ApiBoard.ashx?func=GetBoardList", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return retValue;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    retValue = JsonConvert.DeserializeObject<List<BeanAppBaseExt>>(retData["data"]["Data"].ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR - LoadMoreData from server: " + ex);
            }

            return retValue;
        }
    }
}
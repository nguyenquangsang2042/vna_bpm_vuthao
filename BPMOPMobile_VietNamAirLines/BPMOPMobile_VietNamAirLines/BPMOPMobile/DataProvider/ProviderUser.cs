using System;
using System.Collections.Generic;
using System.IO;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using Newtonsoft.Json.Linq;
using SQLite;

namespace BPMOPMobile.DataProvider
{
    public class ProviderUser : ProviderBase
    {
        /// <summary>
        /// lấy dữ liệu BeanSettings
        /// </summary>
        /// <returns></returns>
        public List<BeanSettings> GetCustomerAppVersion()
        {
            try
            {
                PAR par = new PAR(null, null, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.SysConfig.Subsite)) combieUrl = combieUrl.TrimEnd('/') + "/" + CmmVariable.SysConfig.Subsite;
                combieUrl += CmmVariable.M_ApiPath;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&type=2&bname=BeanSettings", ref CmmVariable.M_AuthenticatedHttpClient, par, false);

                if (retData == null) return null;
                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("ERR"))
                    return null;
                if (strStatus.Equals("SUCCESS"))
                {
                    List<BeanSettings> lstRetValue = retData["data"].ToObject<List<BeanSettings>>();
                    return lstRetValue;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ProviderUser - getCustomerAppVersion - Err:" + ex);
            }

            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public string GetCurrentlangCode()
        {
            return CmmFunction.GetTitle("CMM_DEFAULT_LANGCODE", "1066");
        }

        /// <summary>
        /// Update lai Lang từ server
        /// </summary>
        /// <param name="langCode"></param>
        /// <param name="flgChkUpdate">true: tạo bảng BeanAppLanguage </param>
        /// <param name="flgResetData"></param>
        /// <returns></returns>
        public bool UpdateLangData(string langCode, bool flgChkUpdate = true, bool flgResetData = false)
        {
            Type type = typeof(BeanAppLanguage);
            string id = type.Name;
            string modified = "";
            string errMess = "";

            SQLiteConnection con = new SQLiteConnection(CmmVariable.M_DataPath, false);
            SQLiteConnection conLang = new SQLiteConnection(CmmVariable.M_DataLangPath, false);
            try
            {
                if (!flgResetData)
                {
                    TableQuery<DBVariable> table = con.Table<DBVariable>();
                    var items = from i in table
                                where i.Id == id
                                select i;
                    if (items.Count() > 0)
                    {
                        modified = items.First().Value;
                    }
                }

                BeanAppLanguage objMst = new BeanAppLanguage();

                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.SysConfig.Subsite)) combieUrl = combieUrl.TrimEnd('/') + "/" + CmmVariable.SysConfig.Subsite;
                combieUrl += CmmVariable.M_ApiPath;
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "get"));
                lstGet.Add(new KeyValuePair<string, string>("lang", langCode));

                lstGet.Add(new KeyValuePair<string, string>("Modified", modified));
                JObject retData = GetJsonDataFromAPI(combieUrl + objMst.GetServerUrl(), ref CmmVariable.M_AuthenticatedHttpClient, new PAR(lstGet));
                if (retData == null) return false;

                string strStatus = retData.Value<string>("status");
                if (strStatus == null || !strStatus.Equals("SUCCESS"))
                {
                    return false;
                }

                List<BeanAppLanguage> lstMstData = retData["data"].ToObject<List<BeanAppLanguage>>();
                if (lstMstData == null || lstMstData.Count == 0) return true;

                if (flgResetData)
                {
                    File.Delete(CmmVariable.M_DataLangPath);
                    conLang = new SQLiteConnection(CmmVariable.M_DataLangPath);
                    conLang.CreateTable<BeanAppLanguage>();
                    BeanAppLanguage currentLang = new BeanAppLanguage();
                    currentLang.Key = "CurrentLang";
                    currentLang.Value = langCode;
                    conLang.Insert(currentLang);
                    conLang.Close();
                    conLang = new SQLiteConnection(CmmVariable.M_DataLangPath);
                }

                String LocalKeyName = BeanBase.GetPriKey(type)[0];
                String serverKeyName = BeanBase.GetPriKeyS(type)[0];
                List<BeanAppLanguage> lstInsertItem = new List<BeanAppLanguage>();
                List<BeanAppLanguage> lstUpdateItem = new List<BeanAppLanguage>();

                if (!flgResetData && flgChkUpdate)
                {
                    foreach (BeanAppLanguage item in lstMstData)
                    {
                        string sqlSel;

                        object serKeyValue = CmmFunction.GetPropertyValueByName(item, serverKeyName);

                        // Kiểm tra nếu tồn tại rồi thì update
                        sqlSel = string.Format("SELECT * FROM {0} WHERE {1} = ?", type.Name, serverKeyName);
                        List<BeanAppLanguage> lstObjChk = conLang.Query<BeanAppLanguage>(sqlSel, serKeyValue);

                        if (lstObjChk.Count > 0)
                        {
                            BeanAppLanguage objChk = lstObjChk[0];
                            CmmFunction.SetPropertyValueByName(item, LocalKeyName, CmmFunction.GetPropertyValueByName(objChk, LocalKeyName));

                            lstUpdateItem.Add(item);

                            // Nếu không tồn tại thì Insert
                        }
                        else
                        {
                            lstInsertItem.Add(item);
                        }
                    }
                }
                else
                {
                    lstInsertItem = lstMstData;
                }

                string sysDateNow = retData.Value<string>("dateNow");
                if (string.IsNullOrEmpty(sysDateNow)) return false;

                conLang.BeginTransaction();
                if (lstInsertItem.Count > 0)
                {
                    conLang.InsertAll(lstInsertItem, false);
                }
                if (lstUpdateItem.Count > 0)
                {
                    conLang.UpdateAll(lstUpdateItem, false);
                }
                conLang.Commit();
                conLang.Close();

                UpdateDbVariable(new DBVariable(id, sysDateNow), con);
                con.Close();

                if (flgResetData)
                {
                    CmmVariable.M_LangData = null;
                    //CmmEvent.UpdateLangComplete_Performence(null, new CmmEvent.UpdateEventArgs(true, langCode));
                }
                return true;
            }
            catch (Exception ex)
            {
                conLang.Rollback();
                System.Diagnostics.Debug.Write("ERR updateLangData: " + ex.Message);
            }
            finally
            {
                con.Close();
                conLang.Close();
            }
            //CmmEvent.UpdateLangComplete_Performence(null, new CmmEvent.UpdateEventArgs(false, langCode, errMess));
            return false;

        }

        /// <summary>
        /// Hàm gọi để Renew lại Data cho User
        /// </summary>
        /// <param name="localPath">Nơi chứa file Avatar tùy theo OS</param>
        /// <param name="loginType"></param>
        /// <returns></returns>
        public bool UpdateCurrentUserInfo(string localPath, int loginType = 1)
        {
            bool _result = false;
            try
            {
                string _getCurrentUserUrl = CmmVariable.M_Domain.TrimEnd('/') + (new BeanUser().GetCurrentUserUrl());
                _getCurrentUserUrl = _getCurrentUserUrl.Replace("<#SiteName#>", "");

                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "login"));
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();
                lstPost.Add(new KeyValuePair<string, string>("deviceInfo", string.IsNullOrEmpty(CmmVariable.SysConfig.DeviceInfo) ? "" : CmmVariable.SysConfig.DeviceInfo));
                lstPost.Add(new KeyValuePair<string, string>("loginType", loginType.ToString()));

                ProviderBase pro = new ProviderBase();
                JObject retData = pro.GetJsonDataFromAPI(_getCurrentUserUrl, ref CmmVariable.M_AuthenticatedHttpClient, new ProviderBase.PAR(lstGet, lstPost), false);

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    BeanUser userInfo = retData["data"]["beanUser"].ToObject<BeanUser>();
                    string _verifyOTP = retData["data"]["verifyOTP"].ToString();
                    if (userInfo != null)
                    {
                        // Cập nhật thông tin User lại
                        if (CmmVariable.SysConfig.AvatarPath != userInfo.ImagePath) // Có avatar mới
                        {
                            CmmVariable.SysConfig.AvatarPath = userInfo.ImagePath;
                            DownloadFile(CmmVariable.M_Domain + CmmVariable.SysConfig.AvatarPath, localPath, CmmVariable.M_AuthenticatedHttpClient);
                        }
                        CmmVariable.SysConfig.VerifyOTP = _verifyOTP;
                        CmmVariable.SysConfig.UserId = userInfo.ID;
                        CmmVariable.SysConfig.Title = userInfo.FullName;
                        CmmVariable.SysConfig.DisplayName = userInfo.FullName;
                        CmmVariable.SysConfig.Email = userInfo.Email;
                        CmmVariable.SysConfig.Department = userInfo.Department;
                        CmmVariable.SysConfig.Address = userInfo.Address;
                        CmmVariable.SysConfig.PositionID = userInfo.PositionID;
                        CmmVariable.SysConfig.PositionTitle = userInfo.PositionTitle;
                        CmmVariable.SysConfig.Mobile = userInfo.Mobile;
                        CmmVariable.SysConfig.SiteName = userInfo.SiteName;
                        CmmVariable.SysConfig.LangCode = userInfo.Language.ToString();
                        CmmVariable.SysConfig.AccountName = userInfo.AccountName;
                        CmmFunction.WriteSetting(); // Lưu Setting
                        _result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdateCurrentUserInfo - Login - ERROR: " + ex.ToString());
            }
            return _result;
        }

        public bool UpdateUserLanguageChange(string langCode)
        {
            bool _result = false;
            try
            {
                //{ ID: 'A057DE5E-FFD6-42EA-B00E-78169A918A0A', Language: 1033}
                JObject objData = new JObject
                {
                    { "ID", CmmVariable.SysConfig.UserId },
                    { "Language", langCode }
                };

                string _updateUserLangChange = CmmVariable.M_Domain.TrimEnd('/') + "/_layouts/15/VuThao.BPMOP.API/ApiUser.ashx";
                _updateUserLangChange = _updateUserLangChange.Replace("<#SiteName#>", "");

                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "UpdateUser"));

                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();
                lstPost.Add(new KeyValuePair<string, string>("data", objData.ToString()));
                ProviderBase pro = new ProviderBase();
                JObject retData = pro.GetJsonDataFromAPI(_updateUserLangChange, ref CmmVariable.M_AuthenticatedHttpClient, new ProviderBase.PAR(lstGet, lstPost), false);

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    _result = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdateUserLanguageChange - Login - ERROR: " + ex.ToString());
            }
            return _result;
        }

        /// <summary>
        /// Vô hiệu hóa user
        /// </summary>
        /// <returns></returns>
        public bool DeactivateAccount()
        {
            bool res = false;
            try
            {
                JObject objData = new JObject
                {
                    { "IsActive", 0 },
                    { "UserId", CmmVariable.SysConfig.UserId},
                };

                //string _updateUserLangChange = CmmVariable.M_Domain.TrimEnd('/') + "/_layouts/15/VuThao.BPMOP.API/ApiUser.ashx";
                //_updateUserLangChange = _updateUserLangChange.Replace("<#SiteName#>", "");

                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "deoractiveuser"));

                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();
                lstPost.Add(new KeyValuePair<string, string>("data", objData.ToString()));

                PAR par = new PAR(lstGet, lstPost, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.SysConfig.Subsite)) combieUrl = combieUrl.TrimEnd('/') + "/" + CmmVariable.SysConfig.Subsite;
                combieUrl += CmmVariable.M_ApiPath;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?", ref CmmVariable.M_AuthenticatedHttpClient, par, false);//func=deoractiveuser

                if (retData != null)
                {
                    string strStatus = retData.Value<string>("status");
                    if (strStatus.Equals("SUCCESS"))
                    {
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ProviderUser - getCustomerAppVersion - Err:" + ex);
            }

            return res;
        }
    }
}

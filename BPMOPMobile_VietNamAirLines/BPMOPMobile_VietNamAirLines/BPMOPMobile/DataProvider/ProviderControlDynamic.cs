using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite;
using System;
using System.Collections.Generic;

namespace BPMOPMobile.DataProvider
{
    public class ProviderControlDynamic : ProviderBase
    {
        /// <summary>
        /// lấy data cols 
        /// </summary>
        /// <param name="workflowItem"></param>
        /// <param name="lstparaCols">các cột cần lấy dữ liệu</param>
        /// <returns></returns>
        public string GetTicketRequestByWorkflowItemId(BeanWorkflowItem workflowItem, List<string> lstparaCols)
        {
            string retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "get"));
                lstGet.Add(new KeyValuePair<string, string>("rid", workflowItem.ItemID.ToString()));
                lstGet.Add(new KeyValuePair<string, string>("lname", workflowItem.ListName));
                lstGet.Add(new KeyValuePair<string, string>("type", "1"));
                lstGet.Add(new KeyValuePair<string, string>("scanFile", "1"));
                lstGet.Add(new KeyValuePair<string, string>("actionPer", "1"));

                lstPost.Add(new KeyValuePair<string, string>("cols", JsonConvert.SerializeObject(lstparaCols)));

                PAR par = new PAR(lstGet, lstPost, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.SysConfig.Subsite)) combieUrl += "/" + CmmVariable.SysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/workflow/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par, true);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    retValue = retData["data"].ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderTicketRequest - GetTicketRequestByWorkflowItemId - Err:" + ex);
                return null;
            }
            return retValue;
        }
        /// <summary>
        /// lấy file đính kèm
        /// </summary>
        /// <param name="workflowItem"></param>
        /// <returns></returns>
        public List<BeanAttachFile> GetAttFileByWorkflowItem(BeanWorkflowItem workflowItem)
        {
            List<BeanAttachFile> retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getAttachFiles"));
                lstGet.Add(new KeyValuePair<string, string>("rid", workflowItem.ItemID.ToString()));
                lstGet.Add(new KeyValuePair<string, string>("lname", workflowItem.ListName));
                lstGet.Add(new KeyValuePair<string, string>("type", "1"));

                PAR par = new PAR(lstGet, null, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.SysConfig.Subsite)) combieUrl += "/" + CmmVariable.SysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/workflow/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    retValue = retData["data"].ToObject<List<BeanAttachFile>>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderTicketRequest - GetDinhKemFromRequestTicket - Err:" + ex);
                return null;
            }
            return retValue;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="func">Value trong BeanButtonAction lấy ra từ ButtonAction</param>
        /// <param name="jsonData">String Json Của TicketRequest (Chú ý phần này không có Bean sài JObject)</param>
        /// <param name="jsonColoumEnable">String Json mảng các Column dữ liệu muốn Update (Lấy từ BeanControlDynamicDetail các trường có Enable=1)</param>
        /// <param name="listName"></param>
        /// <param name="lstExtent"></param>
        /// <returns></returns>
        public bool DynamicAction(string func, string jsonData, string jsonColoumEnable, string listName, List<KeyValuePair<string, string>> lstExtent = null)
        {
            bool retValue = false;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", func));
                lstPost.Add(new KeyValuePair<string, string>("data", jsonData));
                lstPost.Add(new KeyValuePair<string, string>("cols", jsonColoumEnable));
                lstGet.Add(new KeyValuePair<string, string>("lname", listName));

                if (lstExtent != null && lstExtent.Count > 0)
                {
                    lstPost.AddRange(lstExtent);
                }

                PAR par = new PAR(lstGet, lstPost, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.SysConfig.Subsite)) combieUrl += "/" + CmmVariable.SysConfig.Subsite;
                //JObject retData = GetJsonDataFromAPI(combieUrl + "/workflow/_layouts/15/VuThao.Becamex.API/ApiTicketRequest.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                JObject retData = GetJsonDataFromAPI(combieUrl + "/workflow/_layouts/15/VuThao.BPMOP.API/WorkflowRequest.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return false;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {

                    retValue = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderTicketRequest - ApproveTicketRequest - Err:" + ex);
                return retValue;
            }
            return retValue;
        }

        public List<BeanQuaTrinhLuanChuyen> GetListProcessHistory(BeanWorkflowItem workflowItem)
        {
            List<BeanQuaTrinhLuanChuyen> retValue = null;
            try
            {
                //var param = new
                //{
                //    SPItemId = workflowItem.ID,
                //    ListId = workflowItem.ListId,
                //    Site = "",
                //    Loai = "1",
                //    SiteUrl = ""
                //};

                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getHistory"));
                lstGet.Add(new KeyValuePair<string, string>("fid", workflowItem.ID));
                string combieUrl = CmmVariable.M_Domain + "/workflow/_layouts/15/VuThao.BPMOP.API/TicketRequest.ashx";
                PAR par = new PAR(lstGet, null, null);
                JObject retData = GetJsonDataFromAPI(combieUrl, ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    retValue = retData["data"].ToObject<List<BeanQuaTrinhLuanChuyen>>();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderControlDynamic - GetListUserWithFilter - Err:" + ex);
                return null;
            }
            return retValue;
        }

        public List<BeanShareHistory> GetListShareHistory(BeanWorkflowItem workflowItem)
        {
            List<BeanShareHistory> retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getShareHistory"));
                lstGet.Add(new KeyValuePair<string, string>("fid", workflowItem.ID));
                string combieUrl = CmmVariable.M_Domain + "/workflow/_layouts/15/VuThao.BPMOP.API/TicketRequest.ashx";
                PAR par = new PAR(lstGet, null, null);
                JObject retData = GetJsonDataFromAPI(combieUrl, ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    retValue = retData["data"].ToObject<List<BeanShareHistory>>();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderUser - GetListShareHistory - Err:" + ex);
                return null;
            }
            return retValue;
        }

        /// <summary>
        /// limit default: 10
        /// </summary>
        /// <param name="jsonFilter">filter lay tu column FilterData menu</param>
        /// <param name="limit"></param>
        /// <param name="offset">vi tri bat dau lay du lieu neu la 0 se xoa cac records trung key insert moi</param>
        /// <returns></returns>
        public List<BeanQuaTrinhLuanChuyen> GetListUserWithFilter(string jsonFilter, int limit, int offset)
        {
            List<BeanQuaTrinhLuanChuyen> retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "filter"));
                lstGet.Add(new KeyValuePair<string, string>("limit", limit.ToString()));
                lstGet.Add(new KeyValuePair<string, string>("off", offset.ToString()));

                lstPost.Add(new KeyValuePair<string, string>("filter", jsonFilter));
                PAR par = new PAR(lstGet, lstPost, null);
                string combieUrl = CmmVariable.M_Domain + "/workflow/_layouts/15/VuThao.Becamex.API/ApiMobilePublic.ashx";

                JObject retData = GetJsonDataFromAPI(combieUrl, ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    retValue = retData["data"].ToObject<List<BeanQuaTrinhLuanChuyen>>();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderUser - GetListUserWithFilter - Err:" + ex);
                return null;
            }
            return retValue;
        }
        /// <summary>
        /// lấy dữ liệu Combobox
        /// </summary>
        /// <param name="listName"></param>
        /// <returns></returns>
        public List<BeanDataLookup> GetDataLookup(string listName)
        {
            List<BeanDataLookup> retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "get"));
                lstGet.Add(new KeyValuePair<string, string>("type", "1"));
                lstGet.Add(new KeyValuePair<string, string>("cols", "[\"ID\",\"Title\", \"ParentDept\"]"));
                string combieUrl;
                if (listName.Contains("/"))
                {
                    string value = listName.Split(new[] { "/" }, StringSplitOptions.None)[0];
                    lstPost.Add(new KeyValuePair<string, string>("lname", listName.Split(new[] { "/" }, StringSplitOptions.None)[1]));
                    combieUrl = CmmVariable.M_Domain + "/" + value + "/_layouts/15/VuThao.Becamex.API/ApiMobilePublic.ashx";
                }
                else
                {
                    lstPost.Add(new KeyValuePair<string, string>("lname", listName));
                    combieUrl = CmmVariable.M_Domain + "/_layouts/15/VuThao.Becamex.API/ApiMobilePublic.ashx";
                }

                PAR par = new PAR(lstGet, lstPost, null);


                JObject retData = GetJsonDataFromAPI(combieUrl, ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    retValue = retData["data"].ToObject<List<BeanDataLookup>>();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderUser - GetListUserWithFilter - Err:" + ex);
                return null;
            }
            return retValue;
        }

        /// <summary>
        /// lấy dữ liệu theo control động cho trang Detail workflow
        /// </summary>
        /// <param name="workflowItem"></param>
        /// <param name="lstparaCols">các cột cần lấy dữ liệu</param>
        /// <returns></returns>
        public string GetTicketRequestControlDynamicForm(BeanWorkflowItem workflowItem, string lcid = "1066")
        {
            string retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getForm"));
                lstGet.Add(new KeyValuePair<string, string>("mode", "2")); // 1 là tạo mới quy trình, 2 là chi tiết, 3 giống 2 nhưng cho edit
                lstGet.Add(new KeyValuePair<string, string>("listid", workflowItem.ListId));
                lstGet.Add(new KeyValuePair<string, string>("fid", workflowItem.ID));
                lstGet.Add(new KeyValuePair<string, string>("lcid", lcid)); // 1033 là tiếng anh, 1066 là tiếng Việt

                PAR par = new PAR(lstGet, lstPost, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.SysConfig.Subsite)) combieUrl += "/" + CmmVariable.SysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/workflow/_layouts/15/VuThao.BPMOP.API/TicketRequest.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par, true);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    retValue = retData["data"].ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderTicketRequest - GetTicketRequestControlDynamicForm - Err:" + ex);
                return null;
            }
            return retValue;
        }

        /// <summary>
        /// lấy dữ liệu theo control động cho trang Tạo mới
        /// </summary>
        /// <param name="workflowItem"></param>
        /// <param name="lstparaCols">các cột cần lấy dữ liệu</param>
        /// <returns></returns>
        public string GetCreateWorkflowControlDynamicForm(BeanWorkflow workflow, string lcid = "1066", string mode = "1")
        {
            string retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "getForm"));
                lstGet.Add(new KeyValuePair<string, string>("mode", mode)); // 1 là tạo mới quy trình, 2 là chi tiết, 3 giống 2 nhưng cho edit
                lstGet.Add(new KeyValuePair<string, string>("listid", workflow.ListID));
                lstGet.Add(new KeyValuePair<string, string>("fid", "0"));
                lstGet.Add(new KeyValuePair<string, string>("lcid", lcid)); // 1033 là tiếng anh, 1066 là tiếng Việt

                PAR par = new PAR(lstGet, lstPost, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.SysConfig.Subsite)) combieUrl += "/" + CmmVariable.SysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/workflow/_layouts/15/VuThao.BPMOP.API/TicketRequest.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par, true);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    retValue = retData["data"].ToString();
                }
            }
            catch (Exception ex)
            {
                //bluds
                Console.WriteLine("Error - ProviderTicketRequest - GetTicketRequestControlDynamicForm - Err:" + ex);
                return null;
            }
            return retValue;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="func"> tên Function của Action</param>
        /// <param name="jsonFormData"> Data của form động (không bao gồm action)</param>
        /// <param name="jsonColoumEnable"></param>
        /// <param name="lstExtent">List này bao gồm các biến thêm tùy trường hợp: userValues, idea, ...</param>
        /// <returns></returns>
        public bool SendControlDynamicAction(string func, string fid, string FormDefineInfo, string json_editElement, ref string messageERR, List<KeyValuePair<string, string>> lstFile, List<KeyValuePair<string, string>> lstExtent = null)
        {
            bool retValue = false;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();

                lstGet.Add(new KeyValuePair<string, string>("func", func));
                lstPost.Add(new KeyValuePair<string, string>("fid", fid));
                lstPost.Add(new KeyValuePair<string, string>("formDefineInfo", FormDefineInfo));
                lstPost.Add(new KeyValuePair<string, string>("data", json_editElement));
                lstPost.Add(new KeyValuePair<string, string>("lcid", CmmVariable.SysConfig.LangCode));

                if (lstExtent != null && lstExtent.Count > 0)
                {
                    lstPost.AddRange(lstExtent);
                }

                PAR par = new PAR(lstGet, lstPost, lstFile);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.SysConfig.Subsite)) combieUrl += "/" + CmmVariable.SysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/workflow/_layouts/15/VuThao.BPMOP.API/WorkflowRequest.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return false;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    retValue = true;
                }
                else
                {
                    if (retData["mess"]["Key"].ToString().Equals("001")) // lỗi hệ thống -> xài câu default language
                        messageERR = CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!");
                    else
                        messageERR = retData["mess"]["Value"].ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderTicketRequest - ApproveTicketRequest - Err:" + ex);
                return retValue;
            }
            return retValue;
        }

        /// <summary>
        /// API để phân công công việc
        /// </summary>
        /// <param name="_itemTask">item tạo mới bao gồm các thông tin cơ bản: tiêu đề, thời hạn, ...</param>
        /// <param name="_lstAssignUser">danh sách người và nhóm xử lý task đó</param>
        /// <param name="_lstAssignUser">Giống bên GetCreateWorkflowControlDynamicForm</param>
        /// <param name="_lstFileDeleted"> List file đã xóa ra bằng app </param>
        /// <param name="lstFile">List file đính kèm</param>
        /// <param name="flag"> -1: không thao tác (người xem), 0: tạo mới, 1: người tạo update, 2: người xử lý update</param>
        /// <returns></returns>
        public bool SendCreateTaskAction(BeanTask _itemTask, List<BeanUserAndGroup> _lstAssignUser, List<ObjectSubmitAction> lstSubmitAction, List<KeyValuePair<string, string>> lstFile, int _flag)
        {
            bool retValue = false;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();

                _lstAssignUser = CmmFunction.QueryInfoListAssign(_lstAssignUser); // Query cho đầy đủ thông tin list

                List<string> _lstAccountName = new List<string>();
                foreach (BeanUserAndGroup item in _lstAssignUser)
                {
                    _lstAccountName.Add(item.AccountName);
                }

                var jsonObject = new
                {
                    itemTask = _itemTask, // Parent = 0: nếu là tạo mới công việc không cha -  Status = 0: tạo mới mặc định là 0 - SPItemId : ID của WorkflowItem
                    Assign = string.Join(";#", _lstAccountName.ToArray()), // người xử lý dạng: username1;#username2
                    Flag = _flag, // Flag: -1: không thao tác(người xem), 0: tạo mới, 1: người tạo update, 2: người xử lý update
                    JsonEdit = lstSubmitAction // Để check xem có xóa file hay gì không 
                };

                lstGet.Add(new KeyValuePair<string, string>("func", "CreateTask"));
                lstPost.Add(new KeyValuePair<string, string>("data", JsonConvert.SerializeObject(jsonObject)));

                PAR par = new PAR(lstGet, lstPost, lstFile);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.SysConfig.Subsite)) combieUrl += "/" + CmmVariable.SysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/workflow/_layouts/15/VuThao.BPMOP.API/ApiTask.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return false;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    retValue = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderTicketRequest - ApproveTicketRequest - Err:" + ex);
                return retValue;
            }
            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WorkflowId"></param>
        /// <param name="Flag"> Flag - 0: Bỏ thích, 1: Thích </param>
        /// <returns></returns>
        public bool SetFavoriteWorkflow(int workflowId, bool flag)
        {
            bool retValue = false;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();

                var PostObj = new { Flag = flag == true ? 1 : 0, WorkflowId = workflowId };

                lstGet.Add(new KeyValuePair<string, string>("func", "SetFavorite"));
                lstPost.Add(new KeyValuePair<string, string>("data", JsonConvert.SerializeObject(PostObj)));

                PAR par = new PAR(lstGet, lstPost, null);
                string combieUrl = CmmVariable.M_Domain + "/_layouts/15/VuThao.BPMOP.API/ApiBoard.ashx";

                JObject retData = GetJsonDataFromAPI(combieUrl, ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return false;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    retValue = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderUser - GetListUserWithFilter - Err:" + ex);
                return false;
            }
            return retValue;
        }

        /// <summary>
        /// API gọi dữ liệu chi tiết công việc 
        /// </summary>
        /// <returns></returns>
        public string GetDetailTaskForm(int taskID)
        {
            string retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();


                lstGet.Add(new KeyValuePair<string, string>("func", "DetailTask"));
                lstPost.Add(new KeyValuePair<string, string>("taskID", taskID.ToString()));

                PAR par = new PAR(lstGet, lstPost, null);
                string combieUrl = CmmVariable.M_Domain + "/_layouts/15/VuThao.BPMOP.API/ApiTask.ashx";

                JObject retData = GetJsonDataFromAPI(combieUrl, ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    retValue = retData["data"].ToString();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProvideControlDynamic - GetDetailTaskForm - Err:" + ex);
                return null;
            }
            return retValue;
        }

        /// <summary>
        /// API Xóa Task - return true nếu thành công
        /// </summary>
        /// <param name="taskID">Task ID</param>
        /// <returns></returns>
        public bool DeleteDetailTaskForm(int taskID)
        {
            bool retValue = false;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();


                lstGet.Add(new KeyValuePair<string, string>("func", "DeleteTask"));
                lstPost.Add(new KeyValuePair<string, string>("IID", taskID.ToString()));

                PAR par = new PAR(lstGet, lstPost, null);
                string combieUrl = CmmVariable.M_Domain + "/_layouts/15/VuThao.BPMOP.API/ApiTask.ashx";

                JObject retData = GetJsonDataFromAPI(combieUrl, ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return false;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    retValue = true;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProvideControlDynamic - DeleteDetailTaskForm - Err:" + ex);
                return retValue;
            }
            return retValue;
        }

        /// <summary>
        /// Hàm để lấy danh sách bình luận
        /// </summary>
        /// <param name="_OtherResourceID">OtherResourceID Lấy từ API get Form</param>
        /// <param name="_resourceCategoryID">_resourceCategoryID 8-WFItem 16 Task</param>
        /// <param name="_Modified">Thời gian lần cuối lấy Comment - truyền null nếu là lần đầu</param>
        /// <param name="datenow">Để lấy datenow ra</param>
        /// <returns></returns>

        public List<BeanComment> GetListComment(string _OtherResourceID, int _resourceCategoryID, DateTime? _Modified, ref string _Datenow, SQLiteConnection _conn = null)
        {
            List<BeanComment> retValue = null;
            try
            {
                if (_conn == null)
                    _conn = new SQLiteConnection(CmmVariable.M_DataPath);

                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();

                List<ObjectFilter> _lstFilter = new List<ObjectFilter>();
                _Modified = null;
                _lstFilter.Add(new ObjectFilter() { Key = "FilterType", LogicCon = "eq", Value = "COMMENT", ValueType = "" });
                _lstFilter.Add(new ObjectFilter() { Key = "ItemId", LogicCon = "eq", Value = _OtherResourceID, ValueType = "" });
                _lstFilter.Add(new ObjectFilter() { Key = "Modified", LogicCon = "eq", Value = _Modified == null ? "" : _Modified.Value.ToString("yyyy-MM-dd HH:mm:ss"), ValueType = "" });

                lstGet.Add(new KeyValuePair<string, string>("func", "filter"));
                lstPost.Add(new KeyValuePair<string, string>("filter", JsonConvert.SerializeObject(_lstFilter)));

                PAR par = new PAR(lstGet, lstPost);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.SysConfig.Subsite)) combieUrl += "/" + CmmVariable.SysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/workflow/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                _Datenow = retData.Value<string>("dateNow");
                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    retValue = JsonConvert.DeserializeObject<List<BeanComment>>(retData["data"].ToString());

                    #region Update Sqlite BeanComment
                    if (retValue != null && retValue.Count > 0)
                    {
                        List<BeanComment> lstInsert = new List<BeanComment>();
                        List<BeanComment> lstUpdate = new List<BeanComment>();
                        switch (_resourceCategoryID)
                        {
                            case 8: // nếu là Comment WFItem thì xử lý Sqlite
                                {
                                    string _queryComment = @"SELECT * FROM BeanComment WHERE ResourceId = '{0}' AND ID = '{1}'";
                                    foreach (BeanComment item in retValue)
                                    {
                                        List<BeanComment> lstLocal = _conn.Query<BeanComment>(string.Format(_queryComment, _OtherResourceID, item.ID));

                                        if (lstLocal != null && lstLocal.Count > 0) // đã tồn tại trong DB -> Update
                                            lstUpdate.Add(item);
                                        else // Chưa có trong DB -> Insert
                                            lstInsert.Add(item);
                                    }

                                    if (lstInsert.Count > 0)
                                        _conn.InsertAll(lstInsert, false);
                                    if (lstUpdate.Count > 0)
                                        _conn.UpdateAll(lstUpdate, false);

                                    _conn.Commit();
                                    break;
                                }

                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderControlDynamic - GetListComment - Err:" + ex);
                return retValue;
            }
            finally
            {
                _conn.Close();
            }
            return retValue;
        }

        /// <summary>
        /// Hàm gọi để lấy OtherResourceID nếu form chưa có, nếu có rồi vẫn gọi để server xử lý nhanh hơn
        /// </summary>
        /// <param name="_objDetailComment">Note trong Object</param>
        /// <returns></returns>
        public string GetDetailOtherResource(ObjectSubmitDetailComment _objDetailComment)
        {
            string retValue = "";
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();

                List<ObjectFilter> _lstFilter = new List<ObjectFilter>();

                lstGet.Add(new KeyValuePair<string, string>("func", "detail"));
                lstPost.Add(new KeyValuePair<string, string>("data", JsonConvert.SerializeObject(_objDetailComment)));

                PAR par = new PAR(lstGet, lstPost);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.SysConfig.Subsite)) combieUrl += "/" + CmmVariable.SysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/workflow/_layouts/15/VuThao.BPMOP.Social/OtherResouce.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return "";

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    retValue = retData["data"]["detail"]["ID"].ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderControlDynamic - GetAuthenComment - Err:" + ex);
                return retValue;
            }
            return retValue;
        }

        /// <summary>
        /// Comment cha: ParentCommentId = null - Comment con: ParentCommentId = ....
        /// </summary>
        /// <returns></returns>
        public bool AddComment(BeanOtherResource _otherResouceItem, List<KeyValuePair<string, string>> lstFile)
        {
            bool retValue = false;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();

                List<ObjectFilter> _lstFilter = new List<ObjectFilter>();

                lstGet.Add(new KeyValuePair<string, string>("func", "add"));
                lstPost.Add(new KeyValuePair<string, string>("data", JsonConvert.SerializeObject(_otherResouceItem)));

                PAR par = new PAR(lstGet, lstPost, lstFile);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.SysConfig.Subsite)) combieUrl += "/" + CmmVariable.SysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/workflow/_layouts/15/VuThao.BPMOP.Social/Comment.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return false;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    retValue = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderControlDynamic - GetAuthenComment - Err:" + ex);
                return retValue;
            }
            return retValue;
        }

        /// <summary>
        /// Set Like / Unlike comment
        /// </summary>
        /// <param name="ResourceId">ID của comment</param>
        /// <param name="Flag">true = like - false = unlike</param>
        /// <param name="ResourceCategoryId">default comment là 32</param>
        /// <param name=""></param>
        /// <returns></returns>
        public bool SetLikeComment(string _ResourceId, bool _Flag, int _ResourceCategoryId = 32)
        {
            bool retValue = false;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();

                // {"ResourceId":"fc19af3a-6438-49b6-9dbb-8593a7e8ea83","ResourceCategoryId":32}
                var param = new
                {
                    ResourceId = _ResourceId,
                    ResourceCategoryId = _ResourceCategoryId,
                };

                lstGet.Add(new KeyValuePair<string, string>("func", _Flag == true ? "like" : "unlike"));
                lstPost.Add(new KeyValuePair<string, string>("data", JsonConvert.SerializeObject(param)));

                PAR par = new PAR(lstGet, lstPost);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.SysConfig.Subsite)) combieUrl += "/" + CmmVariable.SysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/_layouts/15/VuThao.BPMOP.Social/Like.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return false;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    retValue = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderControlDynamic - GetAuthenComment - Err:" + ex);
                return retValue;
            }
            return retValue;
        }

        /// <summary>
        /// Lấy dữ liệu Formfild cho Header của lưới động
        /// </summary>
        /// <param name="_resourceViewID">ID của view</param>
        /// <param name="_limit">limit trong DB</param>
        /// <param name="_offset">offset trong DB</param>
        /// <param name="_total">dành cho kendo Grid, khới tạo = -1, filter lại = -1, clear = -1</param>
        /// <returns></returns>
        public List<BeanWFDetailsHeader> GetDynamicFormField(int _resourceViewID, int _limit, int _offset, int _total = -1)
        {
            List<BeanWFDetailsHeader> retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();

                List<ObjectFilter> _lstProSearch = new List<ObjectFilter>() { new ObjectFilter() { Key = "ResourceViewID", LogicCon = "eq", Value = _resourceViewID.ToString() } };
                ObjectPropertySearch _objectPropertySearch = new ObjectPropertySearch()
                {
                    lstProSeach = JsonConvert.SerializeObject(_lstProSearch),
                    limit = _limit,
                    offset = _offset,
                    total = _total
                };

                lstGet.Add(new KeyValuePair<string, string>("func", "getList"));
                lstGet.Add(new KeyValuePair<string, string>("resouce", "GetFormField"));
                lstPost.Add(new KeyValuePair<string, string>("data", JsonConvert.SerializeObject(_objectPropertySearch)));

                PAR par = new PAR(lstGet, lstPost);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.SysConfig.Subsite)) combieUrl += "/" + CmmVariable.SysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    retValue = JsonConvert.DeserializeObject<List<BeanWFDetailsHeader>>(retData["data"]["Data"].ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderControlDynamic - GetDynamicFormField - Err:" + ex);
                return retValue;
            }
            return retValue;
        }

        /// <summary>
        /// Lấy dữ liệu WorkflowItem cho Data của lưới động
        /// </summary>
        /// <param name="_resourceViewID">ID của view</param>
        /// <param name="_created">để lấy theo ngày tạo của phiếu, nếu truyền null thì lấy all</param>
        /// <param name="_limit">limit trong DB</param>
        /// <param name="_offset">offset trong DB</param>
        /// <param name="_total">dành cho kendo Grid, khới tạo = -1, filter lại = -1, clear = -1</param>
        /// <returns></returns>
        public List<JObject> GetDynamicWorkflowItem(int _resourceViewID, DateTime? _created, int _limit, int _offset, int _total = -1)
        {
            List<JObject> retValue = null;
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();

                List<ObjectFilter> _lstProSearch = new List<ObjectFilter>();
                _lstProSearch.Add(new ObjectFilter() { Key = "ResourceViewID", ContentType = "text", LogicCon = "eq", Value = _resourceViewID.ToString() });
                if (_created != null)
                    _lstProSearch.Add(new ObjectFilter() { Key = "Created", ContentType = "text", LogicCon = "gte", Value = _created.Value.ToString("yyyy-MM-dd HH:mm:ss") });

                ObjectPropertySearch _objectPropertySearch = new ObjectPropertySearch()
                {
                    lstProSeach = JsonConvert.SerializeObject(_lstProSearch),
                    limit = _limit,
                    offset = _offset,
                    total = _total
                };

                lstGet.Add(new KeyValuePair<string, string>("func", "getList"));
                lstGet.Add(new KeyValuePair<string, string>("resource", "GetWorkFlowItem"));
                lstPost.Add(new KeyValuePair<string, string>("data", JsonConvert.SerializeObject(_objectPropertySearch)));

                PAR par = new PAR(lstGet, lstPost);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.SysConfig.Subsite)) combieUrl += "/" + CmmVariable.SysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/_layouts/15/VuThao.BPMOP.API/ApiResourceView.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    retValue = JsonConvert.DeserializeObject<List<JObject>>(retData["data"]["Data"].ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderControlDynamic - GetDynamicWorkflowItem - Err:" + ex);
                return retValue;
            }
            return retValue;
        }

        /// <summary>
        /// Hàm xử lý dictionary thành llist obj filter cho API Filter động
        /// </summary>
        /// <param name="_lstProperties"></param>
        /// <returns></returns>
        public List<ObjectFilter> HandleObjectFilterDictionary(Dictionary<string, string> _lstProperties)
        {
            List<ObjectFilter> _result = new List<ObjectFilter>();
            try
            {
                foreach (KeyValuePair<string, string> item in _lstProperties) // Xử lý Object Filter    
                {
                    switch (item.Key.Trim().ToLowerInvariant())
                    {
                        case "resourceviewid": // xác định xem là view cấu hình nào - xem ID trong DB
                            {
                                _result.Add(new ObjectFilter() { ContentType = "text", Key = "ResourceViewID", LogicCon = "eq", Value = item.Value.ToString() }); break;
                            }
                        case "viewtype": // trạng thái - 2: đang xử lý - 4: đã xử lý (việc đến tôi mới cần)
                            {
                                _result.Add(new ObjectFilter() { ContentType = "text", Key = "ViewType", LogicCon = "eq", Value = item.Value.ToString() }); break;
                            }
                        case "statusgroup": // Ex: 1,2,3,4,5,6
                            {
                                _result.Add(new ObjectFilter() { ContentType = "text", Key = "StatusGroup", LogicCon = "in", Value = item.Value.ToString() }); break;
                            }
                        case "duedate-gte": // Hạn hoàn tất - từ ngày - Ex: "2021-04-07 00:00"
                            {
                                _result.Add(new ObjectFilter() { ContentType = "datetime", Key = "DueDate", LogicCon = "gte", Value = item.Value.ToString() }); break;
                            }
                        case "duedate-lte": // Hạn hoàn tất - đến ngày - Ex: "2021-04-07 00:00"
                            {
                                _result.Add(new ObjectFilter() { ContentType = "datetime", Key = "DueDate", LogicCon = "lte", Value = item.Value.ToString() }); break;
                            }
                        case "created-gte": // ngày gửi đến - từ ngày - Ex: "2021-04-07"
                            {
                                _result.Add(new ObjectFilter() { ContentType = "date", Key = "Created", LogicCon = "gte", Value = item.Value.ToString() }); break;
                            }
                        case "created-lte": // ngày gửi đến - đến ngày - Ex: "2021-04-07"
                            {
                                _result.Add(new ObjectFilter() { ContentType = "date", Key = "Created", LogicCon = "lte", Value = item.Value.ToString() }); break;
                            }
                        case "lcid": // Ngôn ngữ của Data - Ex: 1033 - 1066
                            {
                                _result.Add(new ObjectFilter() { ContentType = "text", Key = "lcid", LogicCon = "eq", Value = item.Value.ToString() }); break;
                            }
                        case "workflowid": // chỉ trả ra những phiếu theo id quy trình này - Ex: "1010"
                            {
                                _result.Add(new ObjectFilter() { ContentType = "text", Key = "WorkflowId", LogicCon = "eq", Value = item.Value.ToString() }); break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderControlDynamic - HandleObjectFilterDictionary - Err:" + ex);
            }
            return _result;
        }

        /// <summary>
        /// Filter động trên server Việc đến tôi
        /// </summary>
        /// <param name="_lstProperties">list dictionary chứa key filter</param>
        /// <param name="_totalRecord">Tong so count cua cau query</param>
        /// <param name="_limit">limit 1 lần lấy, -1 là lấy all</param>
        /// <param name="_offset">offset của DB</param>
        /// <param name="_total">1: Tat ca || -1: theo limit</param>
        /// <returns></returns>
        public List<BeanAppBaseExt> GetListFilterMyTask(Dictionary<string, string> _lstProperties, ref int _totalRecord, int _limit = -1, int _offset = 0, int _total = -1)
        {
            List<BeanAppBaseExt> _result = new List<BeanAppBaseExt>();
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();

                List<ObjectFilter> _lstObjectFilter = HandleObjectFilterDictionary(_lstProperties);

                ObjectPropertySearch _objectPropertySearch = new ObjectPropertySearch()
                {
                    lstProSeach = JsonConvert.SerializeObject(_lstObjectFilter),
                    limit = _limit,
                    offset = _offset,
                    total = _total
                };

                lstGet.Add(new KeyValuePair<string, string>("func", "getList"));
                lstGet.Add(new KeyValuePair<string, string>("resource", "GetMyTaskV2"));
                lstPost.Add(new KeyValuePair<string, string>("data", JsonConvert.SerializeObject(_objectPropertySearch)));

                PAR par = new PAR(lstGet, lstPost);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.SysConfig.Subsite)) combieUrl += "/" + CmmVariable.SysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/_layouts/15/VuThao.BPMOP.API/ApiResourceView.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    _result = JsonConvert.DeserializeObject<List<BeanAppBaseExt>>(retData["data"]["Data"].ToString());
                    _totalRecord = int.Parse(retData["data"]["MoreInfo"]["totalRecord"].ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderControlDynamic - GetListFilterMyTask - Err:" + ex);
            }
            return _result;
        }

        /// <summary>
        /// Filter động trên server Việc tôi bắt đầu
        /// </summary>
        /// <param name="_lstProperties">list dictionary chứa key filter</param>
        /// <param name="_limit">limit 1 lần lấy, -1 là lấy all</param>
        /// <param name="_offset">offset của DB</param>
        /// <param name="_total"></param>
        /// <returns></returns>
        public List<BeanAppBaseExt> GetListFilterMyRequest(Dictionary<string, string> _lstProperties, ref int _totalRecord, int _limit = -1, int _offset = 0, int _total = -1)
        {
            List<BeanAppBaseExt> _result = new List<BeanAppBaseExt>();
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> lstPost = new List<KeyValuePair<string, string>>();

                List<ObjectFilter> _lstObjectFilter = HandleObjectFilterDictionary(_lstProperties);

                ObjectPropertySearch _objectPropertySearch = new ObjectPropertySearch()
                {
                    lstProSeach = JsonConvert.SerializeObject(_lstObjectFilter),
                    limit = _limit,
                    offset = _offset,
                    total = _total
                };

                lstGet.Add(new KeyValuePair<string, string>("func", "getList"));
                lstGet.Add(new KeyValuePair<string, string>("resource", "GetMyRequestV2"));
                lstPost.Add(new KeyValuePair<string, string>("data", JsonConvert.SerializeObject(_objectPropertySearch)));

                PAR par = new PAR(lstGet, lstPost);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.SysConfig.Subsite)) combieUrl += "/" + CmmVariable.SysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/_layouts/15/VuThao.BPMOP.API/ApiResourceView.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return null;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    _result = JsonConvert.DeserializeObject<List<BeanAppBaseExt>>(retData["data"]["Data"].ToString());
                    _totalRecord = int.Parse(retData["data"]["MoreInfo"]["totalRecord"].ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderControlDynamic - GetListFilterMyRequest - Err:" + ex);
            }
            return _result;
        }

        /// <summary>
        /// Get số phiếu Đến tôi và tôi giao
        /// </summary>
        /// <param name="key">thông tin cần lấy 3 type: AssignedToMeAll|RequestByMeInProcessAll -> lấy hết lấy 1 thì chỉ truyền 1 trong 2 giá trị cần lấy </param>
        /// <returns></returns>
        public string GetListCountVDT_VTBD(string key)
        {
            string retValue = "";
            try
            {
                PAR par = new PAR(null, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.SysConfig.Subsite)) combieUrl += "/" + CmmVariable.SysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/_layouts/15/VuThao.BPMOP.API/ApiResourceView.ashx?func=GetCountItem&data={'listFuncName':'" + key + "'}", ref CmmVariable.M_AuthenticatedHttpClient, par);
                if (retData == null) return retValue;

                string strStatus = retData.Value<string>("status");
                if (strStatus.Equals("SUCCESS"))
                {
                    retValue = retData["data"].ToString(); ;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderControlDynamic - GetListCountVDT_VTBD - Err:" + ex);
                //return retValue;
            }
            return retValue;
        }

        /// <summary>
        ///lấy phiếu nếu database trả về rỗng
        /// </summary>
        /// <param name="rid">ID phiếu</param>
        /// <returns></returns>
        public List<BeanWorkflowItem> getWorkFlowItemByRID(string rid)
        {
            List<BeanWorkflowItem> lst = new List<BeanWorkflowItem>();
            try
            {
                List<KeyValuePair<string, string>> lstGet = new List<KeyValuePair<string, string>>();
                lstGet.Add(new KeyValuePair<string, string>("func", "get"));
                lstGet.Add(new KeyValuePair<string, string>("bname", "BeanWorkflowitem"));
                lstGet.Add(new KeyValuePair<string, string>("rid", rid));
                PAR par = new PAR(lstGet, null);
                string combieUrl = CmmVariable.M_Domain;
                if (!string.IsNullOrEmpty(CmmVariable.SysConfig.Subsite)) combieUrl += "/" + CmmVariable.SysConfig.Subsite;
                JObject retData = GetJsonDataFromAPI(combieUrl + "/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx", ref CmmVariable.M_AuthenticatedHttpClient, par);

                if (retData == null) return lst;
                else
                {
                    lst = JsonConvert.DeserializeObject<List<BeanWorkflowItem>>(retData["data"].ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - ProviderControlDynamic - getWorkFlowItemByRID - Err:" + ex);
            }
            return lst;
        }
    }
}


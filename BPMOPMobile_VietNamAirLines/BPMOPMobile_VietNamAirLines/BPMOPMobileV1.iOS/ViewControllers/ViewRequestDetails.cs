using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobileV1.iOS.IOSClass;
using Foundation;
using Newtonsoft.Json;
using SQLite;
using ObjCRuntime;
using UIKit;
using System.IO;
using System.Globalization;
using BPMOPMobileV1.iOS.CustomControlClass;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class ViewRequestDetails : UIViewController
    {
        //List<BeanControlDynamicDetail> lst_beanControlDynamicDetail; tam khoa
        List<BeanQuaTrinhLuanChuyen> lst_qtlc;
        List<BeanControlDynamicDetail> lst_control_hasValue;
        Dictionary<string, string> dic_valueObject = new Dictionary<string, string>();
        public Dictionary<string, string> dic_singleChoiceSelected = new Dictionary<string, string>();
        public Dictionary<string, string> dic_datetimePickerSelected = new Dictionary<string, string>();
        //các trường nào cho phép chỉnh sửa thì luu lại 
        private List<string> lst_ControlEnable = new List<string>();

        //list extend những value add thêm không dynamic control (ghi chú, ý kiến, người được chuyển xử lý...)
        // nội dung, ý kiến: key = "idea"
        // user, người xử lý: key = "userValues"
        private List<KeyValuePair<string, string>> lstExtent = new List<KeyValuePair<string, string>>();

        UIView view_background_effect;
        UIWebView webView_pdf_mode;
        BeanNotify notify { get; set; }
        BeanTicketRequest ticketRequest;
        BeanWorkflowItem workflowItem;
        BeanControlDynamicDetail beanControlDynamic;
        List<ButtonAction> lst_action;
        List<ButtonAction> lst_actionmore;
        List<BeanAttachFile> lst_attachFile;
        UITableView table_qtlc;
        string localDocumentFilepath = string.Empty;
        string str_url_scanPath = string.Empty;
        string str_json_quatrinhluanchuyen = string.Empty;
        bool isViewfileMode = false;
        bool isFromPush;
        bool isViewQTLC = false;
        bool controlEditStatus = false;
        //int viewIndex = 0; // 0: form, 1:fileScan, 2: qua trinh luan chuyen - tam khoa
        int menuIndex = 0; // 2 : yêu cầu của tôi, 3: phê duyệt yêu cầu
        private UITapGestureRecognizer gestureRecognizer;
        CmmLoading loading;


        public ViewRequestDetails(IntPtr handle) : base(handle)
        {
            localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }

        #region View override
        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewConfiguration();
            CheckDataExist();

            #region delegate
            BT_back.TouchUpInside += BT_back_TouchUpInside;
            //BT_viewMode.TouchUpInside += BT_viewMode_TouchUpInside;
            BT_progress.TouchUpInside += BT_progress_TouchUpInside;
            BT_action1.TouchUpInside += BT_action1_TouchUpInside;
            BT_action2.TouchUpInside += BT_action2_TouchUpInside;
            BT_more.TouchUpInside += BT_More_TouchUpInside;
            BT_moreaction_exit.TouchUpInside += BT_moreaction_exit_TouchUpInside;
            BT_attachement.TouchUpInside += BT_attachement_TouchUpInside;
            #endregion
        }
        #endregion

        #region private - public method
        //Navigator from - MainView - Viec can xu ly
        public void setContent(BeanNotify _notify)
        {
            notify = _notify;
            if (notify != null)
            {
                SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
                string q_update = "UPDATE BeanNotify SET Read = 1 WHERE ID = ?";
                conn.Execute(q_update, notify.ID);
            }
        }
        public void setContentFromPush(BeanNotify _notify, bool _fromPush)
        {
            notify = _notify;
            isFromPush = _fromPush;
            if (notify != null)
            {
                SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
                string q_update = "UPDATE BeanNotify SET Read = 1 WHERE ID = ?";
                conn.Execute(q_update, notify.ID);
            }
        }
        // Navigate from - MyRequestView || ViewRequestTodo
        public void setContent(BeanWorkflowItem _worflowItem, int _menuindex)
        {
            workflowItem = _worflowItem;
            menuIndex = _menuindex;
        }
        private void ViewConfiguration()
        {
            string documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string localpath = Path.Combine(documentFolder, CmmVariable.M_Avatar);


            table_content.Frame = new CGRect(0, 60, table_content.Frame.Width, table_content.Frame.Height);
            table_content.ContentInset = new UIEdgeInsets(-35, 0, 0, 0);

            //refreshControl = new UIRefreshControl();
            //refreshControl.TintColor = UIColor.FromRGB(9, 171, 78);

            var firstAttributes = new UIStringAttributes
            {
                // ForegroundColor = UIColor.White,
                Font = UIFont.SystemFontOfSize(12)
            };
            //refreshControl.AttributedTitle = new NSAttributedString("Loading...", firstAttributes);
            //table_content.AddSubview(refreshControl);
            //view_loadmore.BackgroundColor = UIColor.FromWhiteAlpha(1, 0.7f);


            table_actionmore.ContentInset = new UIEdgeInsets(-30, 0, 0, 0);
            table_actionmore.SeparatorInset = UIEdgeInsets.Zero;
            table_actionmore.AllowsSelection = true;

            //this.View.AddSubview(webView_pdf_mode);
            //this.View.AddSubview(table_qtlc);
            //this.View.AddSubview(table_action);
            //this.View.AddSubview(view_background_effect);
        }

        // kiểm tra xem có BeanWorkFlowItem chưa?
        private async void CheckDataExist()
        {
            try
            {
                loading = new CmmLoading(new CGRect((this.View.Bounds.Width - 200) / 2, (this.View.Bounds.Height - 200) / 2, 200, 200), "Đang xử lý...");
                this.View.Add(loading);

                SQLiteAsyncConnection conn = new SQLiteAsyncConnection(CmmVariable.M_DataPath);
                CultureInfo culture = null;
                if (CmmVariable.SysConfig.LangCode == "1033")
                    culture = new CultureInfo("en-EN");
                else //if (CmmVariable.SysConfig.LangCode == "1066")
                    culture = new CultureInfo("vi-VN");

                if (notify != null)
                {
                    if (!string.IsNullOrEmpty(notify.SendUnit))
                        lbl_imgCover.Text = notify.SendUnit.Substring(0, 2).ToUpper();

                    lbl_code.Text = notify.Content;
                    lbl_title.Text = notify.Title;
                    lbl_subTitle.Text = notify.ListName;

                    if (notify.Created.HasValue)
                    {
                        if (notify.Created == DateTime.Now)
                            lbl_sentTime.Text = notify.Created.Value.ToString("HH:mm");
                        else if (notify.Created < DateTime.Now)
                            lbl_sentTime.Text = culture.DateTimeFormat.GetDayName(notify.Created.Value.DayOfWeek);
                    }

                    // kiểm tra xem có BeanWorkFlowItem chưa?
                    // - nếu chưa có thì get về từ server, cập nhật (insert) lại vào BeanWorkFlow DB local
                    // - có rồi thì load  từ DB local lên

                    //gan tam data de test
                    notify.SPItemId = 20;


                    if ((notify.SPItemId != null && notify.SPItemId > 0) && !string.IsNullOrEmpty(notify.ListName))
                    {
                        string query_workflowItem = string.Format("SELECT * FROM BeanWorkflowItem WHERE ID = {0}", notify.SPItemId);
                        var _list_workFlowItem = conn.QueryAsync<BeanWorkflowItem>(query_workflowItem).Result;




                        loading.Hide();
                    }
                }
                // da co workflowItem - co the navigate tu MyRequestListView
                else if (workflowItem != null)
                {
                    //line 1
                    if (!string.IsNullOrEmpty(workflowItem.CreatedByName))
                        lbl_imgCover.Text = workflowItem.CreatedByName.Substring(0, 2).ToUpper();

                    lbl_title.Text = workflowItem.WorkflowTitle;

                    if (workflowItem.Created.HasValue)
                    {
                        if (workflowItem.Created == DateTime.Now)
                            lbl_sentTime.Text = workflowItem.Created.Value.ToString("HH:mm");
                        else if (workflowItem.Created < DateTime.Now)
                            lbl_sentTime.Text = culture.DateTimeFormat.GetDayName(workflowItem.Created.Value.DayOfWeek);
                    }



                    //line 2
                    lbl_subTitle.Text = "Đến: " + workflowItem.AssignedToName;

                    //line 3
                    string query_controlDynamic = string.Format("SELECT * FROM BeanControlDynamicDetail " +
                                                                "WHERE  (FormId = '{0}' AND (FormSubId = 0 OR FormSubId = {1})) ORDER BY Rank"
                                                                , workflowItem.WorkflowID, workflowItem.WorkflowStep);

                    var lst_controlDynamic_novalue = conn.QueryAsync<BeanControlDynamicDetail>(query_controlDynamic).Result;

                    // sau khi có list dynamic control thì lấy danh sách template control tra ve add vào list cols
                    List<string> cols = new List<string>();

                    if (lst_controlDynamic_novalue != null && lst_controlDynamic_novalue.Count > 0)
                        cols = BeanControlDynamicDetail.GetLstFieldId(lst_controlDynamic_novalue);

                    //add them truong ID vao danh sach cols dynamic control
                    cols.Add("ID");

                    // neu ticketRequestDetails co data thi
                    if (!string.IsNullOrEmpty(workflowItem.TicketRequestDetails))
                    {
                        ProviderControlDynamic p_ticketRequest = new ProviderControlDynamic();

                        var jsonDataWorkflowItem = workflowItem.TicketRequestDetails;
                        dic_valueObject = new Dictionary<string, string>();
                        dic_valueObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDataWorkflowItem);

                        if (dic_valueObject != null && dic_valueObject.Count > 0)
                        {
                            //kiểm tra xem view có được phép edit hay không
                            controlEditStatus = bool.Parse(dic_valueObject["IsEnableEdit"]);

                            // gán value vào các dynamic control tương ứng
                            foreach (var item in lst_controlDynamic_novalue)
                            {
                                // cap nhat danh sach cac truong cho phep edit
                                if (item.Enable)
                                {
                                    lst_ControlEnable.Add(item.DataField);
                                }

                                if (dic_valueObject.ContainsKey(item.DataField))
                                {
                                    var value = dic_valueObject[item.DataField];
                                    item.ControlValue = value;

                                    // trường hơp controlEditStatus = false => đã qua bước xử lý (hoặc lý do khác) nên mọi control đều enable = false
                                    if (!controlEditStatus)
                                        item.Enable = false;
                                }
                            }
                        }

                        //cap nhat lst_control_dynamic da fill value vao
                        lst_control_hasValue = lst_controlDynamic_novalue;
                        lst_attachFile = p_ticketRequest.GetAttFileByWorkflowItem(workflowItem);
                        if (lst_attachFile != null && lst_attachFile.Count > 0)
                            lbl_attach_count.Text = lst_attachFile.Count.ToString();

                        table_content.Source = new control_TableSource(lst_control_hasValue, lst_attachFile, this);
                        table_content.ReloadData();

                        // lấy thông tin file scan pdf của form
                        if (dic_valueObject != null && dic_valueObject.Count > 0)
                        {
                            if (dic_valueObject.ContainsKey("ScanFile"))
                                str_url_scanPath = dic_valueObject["ScanFile"];
                        }

                        // lay thong tin luan chuyen
                        List<BeanSearchProperty> arrSearchProperty = new List<BeanSearchProperty>();
                        BeanSearchProperty filterType = new BeanSearchProperty("FilterType", "TICKET_REQUEST_PROCESS");
                        BeanSearchProperty keyWord = new BeanSearchProperty("DocumentID", workflowItem.ItemID.ToString());
                        BeanSearchProperty ListName = new BeanSearchProperty("ListName", workflowItem.ListName);
                        arrSearchProperty.Add(filterType);
                        arrSearchProperty.Add(keyWord);
                        arrSearchProperty.Add(ListName);
                        string searchProperty = JsonConvert.SerializeObject(arrSearchProperty);

                        //lấy danh sách các bước xử lý - quá trình luân chuyển
                        str_json_quatrinhluanchuyen = searchProperty;
                        loadQuaTrinhluanchuyen();

                        if (jsonDataWorkflowItem != null)
                            workflowItem.TicketRequestDetails = jsonDataWorkflowItem;

                        // lấy thông tin action của phiếu yêu cầu
                        if (lst_control_hasValue != null && lst_control_hasValue.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(lst_control_hasValue[0].ButtonAction))
                            {
                                var lst_BT_action = JsonConvert.DeserializeObject<List<ButtonAction>>(lst_control_hasValue[0].ButtonAction);
                                lst_action = CmmFunction.GetLstMyAction(lst_BT_action, int.Parse(dic_valueObject["ActionPermission"]));
                                lst_actionmore = new List<ButtonAction>();
                                checkActionLayout(lst_action);

                                if (lst_actionmore != null && lst_actionmore.Count > 0)
                                {
                                    table_actionmore.Source = new MenuAction_TableSource(lst_actionmore, this);
                                    table_actionmore.ReloadData();
                                }
                                else
                                    BT_more.Hidden = true;
                            }
                            else
                                moreaction_view.Hidden = true;
                        }
                        else
                        {
                            //view_action.Hidden = true;
                        }
                    }
                    // ticketRequestDetails khong co data thi goi API lay ticketRequestDetails theo
                    else
                    {
                        ProviderControlDynamic p_ticketRequest = new ProviderControlDynamic();
                        // lấy danh sách value của dynamic control tương ứng ("cols")
                        string jsonDataWorkflowItem = "";
                        await Task.Run(() =>
                        {
                            jsonDataWorkflowItem = p_ticketRequest.GetTicketRequestByWorkflowItemId(workflowItem, cols);
                        });

                        dic_valueObject = new Dictionary<string, string>();
                        if (!string.IsNullOrEmpty(jsonDataWorkflowItem))
                        {
                            workflowItem.TicketRequestDetails = jsonDataWorkflowItem;
                            dic_valueObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDataWorkflowItem);

                            if (dic_valueObject != null && dic_valueObject.Count > 0)
                            {
                                //kiểm tra xem view có được phép edit hay không
                                controlEditStatus = bool.Parse(dic_valueObject["IsEnableEdit"]);

                                // gán value vào các dynamic control tương ứng
                                foreach (var item in lst_controlDynamic_novalue)
                                {
                                    // cap nhat danh sach cac truong cho phep edit
                                    if (item.Enable)
                                    {
                                        lst_ControlEnable.Add(item.DataField);
                                    }

                                    if (dic_valueObject.ContainsKey(item.DataField))
                                    {
                                        var value = dic_valueObject[item.DataField];
                                        item.ControlValue = value;

                                        // trường hơp controlEditStatus = false => đã qua bước xử lý (hoặc lý do khác) nên mọi control đều enable = false
                                        if (!controlEditStatus)
                                            item.Enable = false;
                                    }
                                }
                            }

                        }

                        //cap nhat lst_control_dynamic da fill value vao
                        lst_control_hasValue = lst_controlDynamic_novalue;

                        await Task.Run(() =>
                        {
                            lst_attachFile = p_ticketRequest.GetAttFileByWorkflowItem(workflowItem);
                        });

                        if (lst_attachFile != null && lst_attachFile.Count > 0)
                            lbl_attach_count.Text = lst_attachFile.Count.ToString();

                        table_content.Source = new control_TableSource(lst_control_hasValue, lst_attachFile, this);
                        table_content.ReloadData();

                        // lấy thông tin file scan pdf của form
                        if (dic_valueObject != null && dic_valueObject.Count > 0)
                        {
                            if (dic_valueObject.ContainsKey("ScanFile"))
                                str_url_scanPath = dic_valueObject["ScanFile"];
                        }

                        // lay thong tin luan chuyen
                        List<BeanSearchProperty> arrSearchProperty = new List<BeanSearchProperty>();
                        BeanSearchProperty filterType = new BeanSearchProperty("FilterType", "TICKET_REQUEST_PROCESS");
                        BeanSearchProperty keyWord = new BeanSearchProperty("DocumentID", workflowItem.ItemID.ToString());
                        BeanSearchProperty ListName = new BeanSearchProperty("ListName", workflowItem.ListName);
                        arrSearchProperty.Add(filterType);
                        arrSearchProperty.Add(keyWord);
                        arrSearchProperty.Add(ListName);
                        string searchProperty = JsonConvert.SerializeObject(arrSearchProperty);

                        //lấy danh sách các bước xử lý - quá trình luân chuyển
                        str_json_quatrinhluanchuyen = searchProperty;
                        loadQuaTrinhluanchuyen();

                        if (jsonDataWorkflowItem != null)
                            workflowItem.TicketRequestDetails = jsonDataWorkflowItem;

                        // lấy thông tin action của phiếu yêu cầu
                        if (lst_control_hasValue != null && lst_control_hasValue.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(lst_control_hasValue[0].ButtonAction))
                            {
                                var lst_BT_action = JsonConvert.DeserializeObject<List<ButtonAction>>(lst_control_hasValue[0].ButtonAction);
                                lst_action = CmmFunction.GetLstMyAction(lst_BT_action, int.Parse(dic_valueObject["ActionPermission"]));
                                lst_actionmore = new List<ButtonAction>();
                                checkActionLayout(lst_action);

                                if (lst_actionmore != null && lst_actionmore.Count > 0)
                                {
                                    table_actionmore.Source = new MenuAction_TableSource(lst_actionmore, this);
                                    table_actionmore.ReloadData();
                                }
                                else
                                    BT_more.Hidden = true;
                            }
                            else
                                moreaction_view.Hidden = true;
                        }
                        else
                        {
                            //view_action.Hidden = true;
                        }
                    }

                    loading.Hide();
                }
            }
            catch (Exception ex)
            {
                loading.Hide();
                Console.WriteLine("TaskDetailsView - CheckDataExist - ERR: " + ex.ToString());
            }
        }
        private async void CheckDataExist_bk()
        {
            try
            {
                loading = new CmmLoading(new CGRect((this.View.Bounds.Width - 200) / 2, (this.View.Bounds.Height - 200) / 2, 200, 200), "Đang xử lý...");
                this.View.Add(loading);

                SQLiteAsyncConnection conn = new SQLiteAsyncConnection(CmmVariable.M_DataPath);
                CultureInfo culture = null;
                if (CmmVariable.SysConfig.LangCode == "1033")
                    culture = new CultureInfo("en-EN");
                else //if (CmmVariable.SysConfig.LangCode == "1066")
                    culture = new CultureInfo("vi-VN");

                if (notify != null)
                {
                    if (!string.IsNullOrEmpty(notify.SendUnit))
                        lbl_imgCover.Text = notify.SendUnit.Substring(0, 2).ToUpper();

                    lbl_code.Text = notify.Content;
                    lbl_title.Text = notify.Title;
                    lbl_subTitle.Text = notify.ListName;

                    if (notify.Created.HasValue)
                    {
                        if (notify.Created == DateTime.Now)
                            lbl_sentTime.Text = notify.Created.Value.ToString("HH:mm");
                        else if (notify.Created < DateTime.Now)
                            lbl_sentTime.Text = culture.DateTimeFormat.GetDayName(notify.Created.Value.DayOfWeek);
                    }

                    // kiểm tra xem có BeanWorkFlowItem chưa?
                    // - nếu chưa có thì get về từ server, cập nhật (insert) lại vào BeanWorkFlow DB local
                    // - có rồi thì load  từ DB local lên

                    //gan tam data de test
                    notify.SPItemId = 20;


                    if ((notify.SPItemId != null && notify.SPItemId > 0) && !string.IsNullOrEmpty(notify.ListName))
                    {
                        string query_workflowItem = string.Format("SELECT * FROM BeanWorkflowItem WHERE ID = {0}", notify.SPItemId);
                        var _list_workFlowItem = conn.QueryAsync<BeanWorkflowItem>(query_workflowItem).Result;

                        // sau khi có workflowItem thì lấy template view Dynamic control
                        if (_list_workFlowItem != null && _list_workFlowItem.Count > 0)
                        {
                            workflowItem = _list_workFlowItem[0];
                            string query_controlDynamic = string.Format("SELECT * FROM BeanControlDynamicDetail " +
                                                                        "WHERE Visble = 1 AND (FormId = '{0}' AND (FormSubId = 0 OR FormSubId = {1})) ORDER BY Rank"
                                                                        , _list_workFlowItem[0].WorkflowID, _list_workFlowItem[0].WorkflowStep);

                            var lst_controlDynamic_novalue = conn.QueryAsync<BeanControlDynamicDetail>(query_controlDynamic).Result;

                            // sau khi có list dynamic control thì lấy danh sách template control tra ve add vào list cols
                            List<string> cols = new List<string>();

                            if (lst_controlDynamic_novalue != null && lst_controlDynamic_novalue.Count > 0)
                                cols = BeanControlDynamicDetail.GetLstFieldId(lst_controlDynamic_novalue);

                            //add them truong ID vao danh sach cols dynamic control
                            cols.Add("ID");

                            // neu ticketRequestDetails co data
                            if (!string.IsNullOrEmpty(workflowItem.TicketRequestDetails))
                            {
                                ProviderControlDynamic p_ticketRequest = new ProviderControlDynamic();

                                var jsonDataWorkflowItem = workflowItem.TicketRequestDetails;
                                dic_valueObject = new Dictionary<string, string>();
                                dic_valueObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDataWorkflowItem);

                                if (dic_valueObject != null && dic_valueObject.Count > 0)
                                {
                                    //kiểm tra xem view có được phép edit hay không
                                    controlEditStatus = bool.Parse(dic_valueObject["IsEnableEdit"]);

                                    // gán value vào các dynamic control tương ứng
                                    foreach (var item in lst_controlDynamic_novalue)
                                    {
                                        // cap nhat danh sach cac truong cho phep edit
                                        if (item.Enable)
                                        {
                                            lst_ControlEnable.Add(item.DataField);
                                        }

                                        if (dic_valueObject.ContainsKey(item.DataField))
                                        {
                                            var value = dic_valueObject[item.DataField];
                                            item.ControlValue = value;

                                            // trường hơp controlEditStatus = false => đã qua bước xử lý (hoặc lý do khác) nên mọi control đều enable = false
                                            if (!controlEditStatus)
                                                item.Enable = false;
                                        }
                                    }
                                }

                                //cap nhat lst_control_dynamic da fill value vao
                                lst_control_hasValue = lst_controlDynamic_novalue;
                                await Task.Run(() =>
                                {
                                    lst_attachFile = p_ticketRequest.GetAttFileByWorkflowItem(workflowItem);

                                });


                                if (lst_attachFile != null && lst_attachFile.Count > 0)
                                    lbl_attach_count.Text = lst_attachFile.Count.ToString();

                                table_content.Source = new control_TableSource(lst_control_hasValue, lst_attachFile, this);
                                table_content.ReloadData();

                                // lấy thông tin file scan pdf của form
                                if (dic_valueObject != null && dic_valueObject.Count > 0)
                                {
                                    if (dic_valueObject.ContainsKey("ScanFile"))
                                        str_url_scanPath = dic_valueObject["ScanFile"];
                                }

                                // lay thong tin luan chuyen
                                List<BeanSearchProperty> arrSearchProperty = new List<BeanSearchProperty>();
                                BeanSearchProperty filterType = new BeanSearchProperty("FilterType", "TICKET_REQUEST_PROCESS");
                                BeanSearchProperty keyWord = new BeanSearchProperty("DocumentID", workflowItem.ItemID.ToString());
                                BeanSearchProperty ListName = new BeanSearchProperty("ListName", workflowItem.ListName);
                                arrSearchProperty.Add(filterType);
                                arrSearchProperty.Add(keyWord);
                                arrSearchProperty.Add(ListName);
                                string searchProperty = JsonConvert.SerializeObject(arrSearchProperty);

                                //lấy danh sách các bước xử lý - quá trình luân chuyển
                                str_json_quatrinhluanchuyen = searchProperty;
                                loadQuaTrinhluanchuyen();

                                if (jsonDataWorkflowItem != null)
                                    workflowItem.TicketRequestDetails = jsonDataWorkflowItem;

                                // lấy thông tin action của phiếu yêu cầu
                                if (lst_control_hasValue != null && lst_control_hasValue.Count > 0)
                                {
                                    if (!string.IsNullOrEmpty(lst_control_hasValue[0].ButtonAction))
                                    {
                                        var lst_BT_action = JsonConvert.DeserializeObject<List<ButtonAction>>(lst_control_hasValue[0].ButtonAction);
                                        lst_action = CmmFunction.GetLstMyAction(lst_BT_action, int.Parse(dic_valueObject["ActionPermission"]));
                                        lst_actionmore = new List<ButtonAction>();
                                        checkActionLayout(lst_action);

                                        if (lst_actionmore != null && lst_actionmore.Count > 0)
                                        {
                                            table_actionmore.Source = new MenuAction_TableSource(lst_actionmore, this);
                                            table_actionmore.ReloadData();
                                        }
                                        else
                                            BT_more.Hidden = true;
                                    }
                                    else
                                        moreaction_view.Hidden = true;
                                }
                                else
                                {
                                    //view_action.Hidden = true;
                                }
                            }
                            // ticketRequestDetails khong co data thi goi API lay ticketRequestDetails
                            else
                            {
                                ProviderControlDynamic p_ticketRequest = new ProviderControlDynamic();
                                // lấy danh sách value của dynamic control tương ứng ("cols")
                                string jsonDataWorkflowItem = "";
                                await Task.Run(() =>
                                {
                                    jsonDataWorkflowItem = p_ticketRequest.GetTicketRequestByWorkflowItemId(workflowItem, cols);
                                });

                                dic_valueObject = new Dictionary<string, string>();
                                if (!string.IsNullOrEmpty(jsonDataWorkflowItem))
                                {
                                    workflowItem.TicketRequestDetails = jsonDataWorkflowItem;
                                    dic_valueObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDataWorkflowItem);

                                    if (dic_valueObject != null && dic_valueObject.Count > 0)
                                    {
                                        //kiểm tra xem view có được phép edit hay không
                                        controlEditStatus = bool.Parse(dic_valueObject["IsEnableEdit"]);

                                        // gán value vào các dynamic control tương ứng
                                        foreach (var item in lst_controlDynamic_novalue)
                                        {
                                            // cap nhat danh sach cac truong cho phep edit
                                            if (item.Enable)
                                            {
                                                lst_ControlEnable.Add(item.DataField);
                                            }

                                            if (dic_valueObject.ContainsKey(item.DataField))
                                            {
                                                var value = dic_valueObject[item.DataField];
                                                item.ControlValue = value;

                                                // trường hơp controlEditStatus = false => đã qua bước xử lý (hoặc lý do khác) nên mọi control đều enable = false
                                                if (!controlEditStatus)
                                                    item.Enable = false;
                                            }
                                        }
                                    }
                                }

                                //cap nhat lst_control_dynamic da fill value vao
                                lst_control_hasValue = lst_controlDynamic_novalue;
                                lst_attachFile = p_ticketRequest.GetAttFileByWorkflowItem(workflowItem);

                                if (lst_attachFile != null && lst_attachFile.Count > 0)
                                    lbl_attach_count.Text = lst_attachFile.Count.ToString();

                                table_content.Source = new control_TableSource(lst_control_hasValue, lst_attachFile, this);
                                table_content.ReloadData();

                                // lấy thông tin file scan pdf của form
                                if (dic_valueObject != null && dic_valueObject.Count > 0)
                                {
                                    if (dic_valueObject.ContainsKey("ScanFile"))
                                        str_url_scanPath = dic_valueObject["ScanFile"];
                                }

                                // lay thong tin luan chuyen
                                List<BeanSearchProperty> arrSearchProperty = new List<BeanSearchProperty>();
                                BeanSearchProperty filterType = new BeanSearchProperty("FilterType", "TICKET_REQUEST_PROCESS");
                                BeanSearchProperty keyWord = new BeanSearchProperty("DocumentID", workflowItem.ItemID.ToString());
                                BeanSearchProperty ListName = new BeanSearchProperty("ListName", workflowItem.ListName);
                                arrSearchProperty.Add(filterType);
                                arrSearchProperty.Add(keyWord);
                                arrSearchProperty.Add(ListName);
                                string searchProperty = JsonConvert.SerializeObject(arrSearchProperty);

                                //lấy danh sách các bước xử lý - quá trình luân chuyển
                                str_json_quatrinhluanchuyen = searchProperty;
                                loadQuaTrinhluanchuyen();

                                if (jsonDataWorkflowItem != null)
                                    workflowItem.TicketRequestDetails = jsonDataWorkflowItem;

                                // lấy thông tin action của phiếu yêu cầu
                                if (lst_control_hasValue != null && lst_control_hasValue.Count > 0)
                                {
                                    if (!string.IsNullOrEmpty(lst_control_hasValue[0].ButtonAction))
                                    {
                                        var lst_BT_action = JsonConvert.DeserializeObject<List<ButtonAction>>(lst_control_hasValue[0].ButtonAction);
                                        lst_action = CmmFunction.GetLstMyAction(lst_BT_action, int.Parse(dic_valueObject["ActionPermission"]));
                                        lst_actionmore = new List<ButtonAction>();
                                        checkActionLayout(lst_action);

                                        if (lst_actionmore != null && lst_actionmore.Count > 0)
                                        {
                                            table_actionmore.Source = new MenuAction_TableSource(lst_actionmore, this);
                                            table_actionmore.ReloadData();
                                        }
                                        else
                                            BT_more.Hidden = true;
                                    }
                                    else
                                        moreaction_view.Hidden = true;
                                }
                                else
                                {
                                    //view_action.Hidden = true;
                                }
                            }
                        }
                        else // khong co workflowitem
                        {
                            CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("K_Mess_ConnectServer", "Không kết nối được đến server, vui lòng kiểm tra lại đường truyền. Xin cám ơn."));
                        }

                        loading.Hide();
                    }
                }
                // da co workflowItem - co the navigate tu MyRequestListView
                else if (workflowItem != null)
                {
                    //line 1
                    if (!string.IsNullOrEmpty(workflowItem.CreatedByName))
                        lbl_imgCover.Text = workflowItem.CreatedByName.Substring(0, 2).ToUpper();

                    lbl_title.Text = workflowItem.WorkflowTitle;

                    if (workflowItem.Created.HasValue)
                    {
                        if (workflowItem.Created == DateTime.Now)
                            lbl_sentTime.Text = workflowItem.Created.Value.ToString("HH:mm");
                        else if (workflowItem.Created < DateTime.Now)
                            lbl_sentTime.Text = culture.DateTimeFormat.GetDayName(workflowItem.Created.Value.DayOfWeek);
                    }



                    //line 2
                    lbl_subTitle.Text = "Đến: " + workflowItem.AssignedToName;


                    string query_controlDynamic = string.Format("SELECT * FROM BeanControlDynamicDetail " +
                                                                "WHERE  (FormId = '{0}' AND (FormSubId = 0 OR FormSubId = {1})) ORDER BY Rank"
                                                                , workflowItem.WorkflowID, workflowItem.WorkflowStep);

                    var lst_controlDynamic_novalue = conn.QueryAsync<BeanControlDynamicDetail>(query_controlDynamic).Result;

                    // sau khi có list dynamic control thì lấy danh sách template control tra ve add vào list cols
                    List<string> cols = new List<string>();

                    if (lst_controlDynamic_novalue != null && lst_controlDynamic_novalue.Count > 0)
                        cols = BeanControlDynamicDetail.GetLstFieldId(lst_controlDynamic_novalue);

                    //add them truong ID vao danh sach cols dynamic control
                    cols.Add("ID");

                    // neu ticketRequestDetails co data thi
                    if (!string.IsNullOrEmpty(workflowItem.TicketRequestDetails))
                    {
                        ProviderControlDynamic p_ticketRequest = new ProviderControlDynamic();

                        var jsonDataWorkflowItem = workflowItem.TicketRequestDetails;
                        dic_valueObject = new Dictionary<string, string>();
                        dic_valueObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDataWorkflowItem);

                        if (dic_valueObject != null && dic_valueObject.Count > 0)
                        {
                            //kiểm tra xem view có được phép edit hay không
                            controlEditStatus = bool.Parse(dic_valueObject["IsEnableEdit"]);

                            // gán value vào các dynamic control tương ứng
                            foreach (var item in lst_controlDynamic_novalue)
                            {
                                // cap nhat danh sach cac truong cho phep edit
                                if (item.Enable)
                                {
                                    lst_ControlEnable.Add(item.DataField);
                                }

                                if (dic_valueObject.ContainsKey(item.DataField))
                                {
                                    var value = dic_valueObject[item.DataField];
                                    item.ControlValue = value;

                                    // trường hơp controlEditStatus = false => đã qua bước xử lý (hoặc lý do khác) nên mọi control đều enable = false
                                    if (!controlEditStatus)
                                        item.Enable = false;
                                }
                            }
                        }

                        //cap nhat lst_control_dynamic da fill value vao
                        lst_control_hasValue = lst_controlDynamic_novalue;
                        lst_attachFile = p_ticketRequest.GetAttFileByWorkflowItem(workflowItem);
                        if (lst_attachFile != null && lst_attachFile.Count > 0)
                            lbl_attach_count.Text = lst_attachFile.Count.ToString();

                        table_content.Source = new control_TableSource(lst_control_hasValue, lst_attachFile, this);
                        table_content.ReloadData();

                        // lấy thông tin file scan pdf của form
                        if (dic_valueObject != null && dic_valueObject.Count > 0)
                        {
                            if (dic_valueObject.ContainsKey("ScanFile"))
                                str_url_scanPath = dic_valueObject["ScanFile"];
                        }

                        // lay thong tin luan chuyen
                        List<BeanSearchProperty> arrSearchProperty = new List<BeanSearchProperty>();
                        BeanSearchProperty filterType = new BeanSearchProperty("FilterType", "TICKET_REQUEST_PROCESS");
                        BeanSearchProperty keyWord = new BeanSearchProperty("DocumentID", workflowItem.ItemID.ToString());
                        BeanSearchProperty ListName = new BeanSearchProperty("ListName", workflowItem.ListName);
                        arrSearchProperty.Add(filterType);
                        arrSearchProperty.Add(keyWord);
                        arrSearchProperty.Add(ListName);
                        string searchProperty = JsonConvert.SerializeObject(arrSearchProperty);

                        //lấy danh sách các bước xử lý - quá trình luân chuyển
                        str_json_quatrinhluanchuyen = searchProperty;
                        loadQuaTrinhluanchuyen();

                        if (jsonDataWorkflowItem != null)
                            workflowItem.TicketRequestDetails = jsonDataWorkflowItem;

                        // lấy thông tin action của phiếu yêu cầu
                        if (lst_control_hasValue != null && lst_control_hasValue.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(lst_control_hasValue[0].ButtonAction))
                            {
                                var lst_BT_action = JsonConvert.DeserializeObject<List<ButtonAction>>(lst_control_hasValue[0].ButtonAction);
                                lst_action = CmmFunction.GetLstMyAction(lst_BT_action, int.Parse(dic_valueObject["ActionPermission"]));
                                lst_actionmore = new List<ButtonAction>();
                                checkActionLayout(lst_action);

                                if (lst_actionmore != null && lst_actionmore.Count > 0)
                                {
                                    table_actionmore.Source = new MenuAction_TableSource(lst_actionmore, this);
                                    table_actionmore.ReloadData();
                                }
                                else
                                    BT_more.Hidden = true;
                            }
                            else
                                moreaction_view.Hidden = true;
                        }
                        else
                        {
                            //view_action.Hidden = true;
                        }
                    }
                    // ticketRequestDetails khong co data thi goi API lay ticketRequestDetails theo
                    else
                    {
                        ProviderControlDynamic p_ticketRequest = new ProviderControlDynamic();
                        // lấy danh sách value của dynamic control tương ứng ("cols")
                        string jsonDataWorkflowItem = "";
                        await Task.Run(() =>
                        {
                            jsonDataWorkflowItem = p_ticketRequest.GetTicketRequestByWorkflowItemId(workflowItem, cols);
                        });

                        dic_valueObject = new Dictionary<string, string>();
                        if (!string.IsNullOrEmpty(jsonDataWorkflowItem))
                        {
                            workflowItem.TicketRequestDetails = jsonDataWorkflowItem;
                            dic_valueObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDataWorkflowItem);

                            if (dic_valueObject != null && dic_valueObject.Count > 0)
                            {
                                //kiểm tra xem view có được phép edit hay không
                                controlEditStatus = bool.Parse(dic_valueObject["IsEnableEdit"]);

                                // gán value vào các dynamic control tương ứng
                                foreach (var item in lst_controlDynamic_novalue)
                                {
                                    // cap nhat danh sach cac truong cho phep edit
                                    if (item.Enable)
                                    {
                                        lst_ControlEnable.Add(item.DataField);
                                    }

                                    if (dic_valueObject.ContainsKey(item.DataField))
                                    {
                                        var value = dic_valueObject[item.DataField];
                                        item.ControlValue = value;

                                        // trường hơp controlEditStatus = false => đã qua bước xử lý (hoặc lý do khác) nên mọi control đều enable = false
                                        if (!controlEditStatus)
                                            item.Enable = false;
                                    }
                                }
                            }

                        }

                        //cap nhat lst_control_dynamic da fill value vao
                        lst_control_hasValue = lst_controlDynamic_novalue;

                        await Task.Run(() =>
                        {
                            lst_attachFile = p_ticketRequest.GetAttFileByWorkflowItem(workflowItem);
                        });

                        if (lst_attachFile != null && lst_attachFile.Count > 0)
                            lbl_attach_count.Text = lst_attachFile.Count.ToString();

                        table_content.Source = new control_TableSource(lst_control_hasValue, lst_attachFile, this);
                        table_content.ReloadData();

                        // lấy thông tin file scan pdf của form
                        if (dic_valueObject != null && dic_valueObject.Count > 0)
                        {
                            if (dic_valueObject.ContainsKey("ScanFile"))
                                str_url_scanPath = dic_valueObject["ScanFile"];
                        }

                        // lay thong tin luan chuyen
                        List<BeanSearchProperty> arrSearchProperty = new List<BeanSearchProperty>();
                        BeanSearchProperty filterType = new BeanSearchProperty("FilterType", "TICKET_REQUEST_PROCESS");
                        BeanSearchProperty keyWord = new BeanSearchProperty("DocumentID", workflowItem.ItemID.ToString());
                        BeanSearchProperty ListName = new BeanSearchProperty("ListName", workflowItem.ListName);
                        arrSearchProperty.Add(filterType);
                        arrSearchProperty.Add(keyWord);
                        arrSearchProperty.Add(ListName);
                        string searchProperty = JsonConvert.SerializeObject(arrSearchProperty);

                        //lấy danh sách các bước xử lý - quá trình luân chuyển
                        str_json_quatrinhluanchuyen = searchProperty;
                        loadQuaTrinhluanchuyen();

                        if (jsonDataWorkflowItem != null)
                            workflowItem.TicketRequestDetails = jsonDataWorkflowItem;

                        // lấy thông tin action của phiếu yêu cầu
                        if (lst_control_hasValue != null && lst_control_hasValue.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(lst_control_hasValue[0].ButtonAction))
                            {
                                var lst_BT_action = JsonConvert.DeserializeObject<List<ButtonAction>>(lst_control_hasValue[0].ButtonAction);
                                lst_action = CmmFunction.GetLstMyAction(lst_BT_action, int.Parse(dic_valueObject["ActionPermission"]));
                                lst_actionmore = new List<ButtonAction>();
                                checkActionLayout(lst_action);

                                if (lst_actionmore != null && lst_actionmore.Count > 0)
                                {
                                    table_actionmore.Source = new MenuAction_TableSource(lst_actionmore, this);
                                    table_actionmore.ReloadData();
                                }
                                else
                                    BT_more.Hidden = true;
                            }
                            else
                                moreaction_view.Hidden = true;
                        }
                        else
                        {
                            //view_action.Hidden = true;
                        }
                    }

                    loading.Hide();
                }
            }
            catch (Exception ex)
            {
                loading.Hide();
                Console.WriteLine("TaskDetailsView - CheckDataExist - ERR: " + ex.ToString());
            }
        }
        // load file scan pdf
        private async void checkFileLocalIsExist(string filepathURL)
        {
            try
            {
                bool result = false;
                localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string filename = filepathURL.Split('/').Last();
                string localfilePath = Path.Combine(localDocumentFilepath, filename);

                if (!File.Exists(localfilePath))
                {
                    await Task.Run(() =>
                    {
                        ProviderBase provider = new ProviderBase();
                        if (provider.DownloadFile(filepathURL, localfilePath, CmmVariable.M_AuthenticatedHttpClient))
                            result = true;
                        else
                            result = false;
                    });

                    if (result == true)
                        openFile(filename);
                    else
                        CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("K_Mess_ConnectServer", "Không kết nối được đến server, vui lòng kiểm tra lại đường truyền. Xin cám ơn."));
                }
                else
                    openFile(localfilePath);
            }
            catch (Exception ex)
            {
                loading.Hide();
                Console.WriteLine("FileViewController - checkFileLocalIsExist - Err: " + ex.ToString());
            }
        }
        private void openFile(string localfilename)
        {
            string localfilePath = Path.Combine(localDocumentFilepath, localfilename);
            webView_pdf_mode.LoadRequest(new NSUrlRequest(new NSUrl(localfilePath, false)));
            webView_pdf_mode.ScalesPageToFit = true;
        }
        //load thong tin luan chuyenBT_action1
        private async void loadQuaTrinhluanchuyen()
        {
            if (lst_qtlc != null && lst_qtlc.Count > 0)
            {
                //table_qtlc.Source = new QTLC_DataSource(lst_qtlc, this);
                //table_qtlc.ReloadData();
            }
            else
            {
                if (!string.IsNullOrEmpty(str_json_quatrinhluanchuyen))
                {
                    await Task.Run(() =>
                    {
                        lst_qtlc = new List<BeanQuaTrinhLuanChuyen>();
                        ProviderControlDynamic p_dynamic = new ProviderControlDynamic();
                        lst_qtlc = p_dynamic.GetListUserWithFilter(str_json_quatrinhluanchuyen, 100, 0);
                        if (lst_qtlc != null)
                        {
                            InvokeOnMainThread(() =>
                            {
                                //table_qtlc.Source = new QTLC_DataSource(lst_qtlc, this);
                                //table_qtlc.ReloadData();
                            });

                        }
                    });
                }
            }
        }
        private void loadSingleChoiceControlData(BeanControlDynamicDetail _control)
        {
            //if (CmmVariable.SysConfig.LangCode == "VN")
            //    lbl_TitleSinglechoice.Text = _control.Title;
            //else if (CmmVariable.SysConfig.LangCode == "EN")
            //    lbl_TitleSinglechoice.Text = _control.TitleEN;

            //List<ClassDynamicControl> lstObject_value = JsonConvert.DeserializeObject<List<ClassDynamicControl>>(_control.DataSource);
            //if (lstObject_value != null && lstObject_value.Count > 0)
            //{
            //    int index_selected = 0;
            //    foreach (var item in lstObject_value)
            //    {
            //        if (item.Title == _control.ControlValue)
            //        {
            //            item.isSelected = true;
            //            index_selected = lstObject_value.IndexOf(item);
            //        }
            //    }

            //    picker_choice.Model = new SingleChoice_PickerSource(_control, lstObject_value, this);
            //    picker_choice.ReloadAllComponents();

            //    picker_choice.Select(index_selected, 0, true);
            //}
        }

        private void checkActionLayout(List<ButtonAction> _lstaction)
        {
            try
            {
                if (lst_action != null && lst_action.Count > 0)
                {
                    GetColorAction();
                    for (int i = 0; i < lst_action.Count; i++)
                    {
                        if (i == 0)
                        {
                            BT_action1.Hidden = false;
                            if (CmmVariable.SysConfig.LangCode == "1033")
                                BT_action1.SetTitle(lst_action[0].TitleEN, UIControlState.Normal);
                            else //if (CmmVariable.SysConfig.LangCode == "1066")
                                BT_action1.SetTitle(lst_action[0].Title, UIControlState.Normal);

                            BT_action1.SetImage(UIImage.FromFile("Icons/icon_Btn_action_" + lst_action[i].ID).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
                            BT_action1.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
                            UIColor color = null;
                            int hexColor = int.Parse(lst_action[i].Color, System.Globalization.NumberStyles.HexNumber);
                            BT_action1.TintColor = CmmIOSFunction.FromHex(color, hexColor);
                        }
                        else if (i == 1)
                        {
                            BT_action2.Hidden = false;
                            if (CmmVariable.SysConfig.LangCode == "1033")
                                BT_action2.SetTitle(lst_action[1].TitleEN, UIControlState.Normal);
                            else  //if (CmmVariable.SysConfig.LangCode == "1066")
                                BT_action2.SetTitle(lst_action[1].Title, UIControlState.Normal);

                            BT_action2.SetImage(UIImage.FromFile("Icons/icon_Btn_action_" + lst_action[1].ID).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
                            UIColor color = null;
                            int hexColor = int.Parse(lst_action[i].Color, System.Globalization.NumberStyles.HexNumber);
                            BT_action2.TintColor = CmmIOSFunction.FromHex(color, hexColor);
                        }
                        else if (i > 1)
                        {
                            lst_actionmore.Add(lst_action[i]);
                            BT_more.Hidden = false;
                        }
                        else
                            BT_more.Hidden = true;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            //int approve_next = (int)WorkflowAction.Action.Next; // next - duyet qua buoc tiep theo
            //int approve = (int)WorkflowAction.Action.Approve; // approve - duyet ket thuc
            //if (!CmmFunction.checkExistAction(approve_next, ticketRequest.ActionPermission) && !CmmFunction.checkExistAction(approve, ticketRequest.ActionPermission))
            //{
            //    //phê duyệt
            //    BT_action1.UserInteractionEnabled = false;
            //    BT_action1.Alpha = 0.2f;
            //}

            //int reject = (int)WorkflowAction.Action.Return; // reject - tra ve buoc khoi dau
            //if (!CmmFunction.checkExistAction(reject, ticketRequest.ActionPermission))
            //{
            //    //từ chối
            //    BT_action2.UserInteractionEnabled = false;
            //    BT_action2.Alpha = 0.2f;
            //}

            //int choykien = (int)WorkflowAction.Action.Idea; // cho y kien - duyet qua buoc tiep theo
            //if (CmmFunction.checkExistAction(choykien, ticketRequest.ActionPermission))
            //{
            //    BT_action1.UserInteractionEnabled = true;
            //    BT_action1.Alpha = 1f;
            //    BT_action1.SetImage(UIImage.FromFile("Icons/icon_choykien.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
            //    BT_action1.TintColor = UIColor.FromRGB(9, 171, 77);

            //    BT_action1.SetTitle("Cho ý kiến", UIControlState.Normal);
            //}

            //lst_action = new List<ButtonAction>();

            //int requestInfo = (int)WorkflowAction.Action.RequestInformation; // yeu cau tham van
            //if (CmmFunction.checkExistAction(requestInfo, ticketRequest.ActionPermission))
            //{
            //    //ClassActionTask action1 = new ClassActionTask();
            //    //action1.ID = 1;
            //    //action1.Title = "Yêu cầu bổ sung";
            //    //action1.Type = 1;
            //    //action1.Visible = true;
            //    //action1.Index = lst_action.Count();
            //    //lst_action.Add(action1);
            //}

            //int requestIdea = (int)WorkflowAction.Action.RequestIdea;   // tham van y kien
            //if (CmmFunction.checkExistAction(requestIdea, ticketRequest.ActionPermission))
            //{
            //    //ClassActionTask action2 = new ClassActionTask();
            //    //action2.ID = 2;
            //    //action2.Title = "Tham vấn ý kiến";
            //    //action2.Type = 1;
            //    //action2.Visible = true;
            //    //action2.Index = lst_action.Count();
            //    //lst_action.Add(action2);
            //}

            //if (lst_action != null & lst_action.Count > 0)
            //{
            //    table_action.Source = new menu_TableSource(lst_action, this);
            //    table_action.ReloadData();
            //}
            //else
            //BT_more.Hidden = true;
        }
        private void GetColorAction()
        {
            try
            {
                SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
                string query = string.Format("SELECT * FROM  BeanSettings WHERE KEY = 'BtnActionColor' ");
                var lst_setting = conn.Query<BeanSettings>(query);
                if (lst_setting != null && lst_setting.Count > 0)
                {
                    var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(lst_setting[0].VALUE);
                    lst_action = CmmFunction.GetColorMyAction(lst_action, dictionary);
                }
            }
            catch (Exception ex)
            {

            }
        }
        private async void loadWorkRequestTicket()
        {
            SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);

            //kiểm tra nếu trong BeanWorkFlowItem có TicketReQuestDetail (json) không?
            // - nếu có thì DeserializeObject Json  rồi load contentForm
            // - không có thì get TicketReQuest từ server về rồi load ContentForm , update string json từ object TicketRequest vào BeanWorkflowItem.TicketRequestDetails
            if (!string.IsNullOrEmpty(workflowItem.TicketRequestDetails))
            {
                ticketRequest = JsonConvert.DeserializeObject<BeanTicketRequest>(workflowItem.TicketRequestDetails);

                loading.Hide();
            }
            else
            {
                await Task.Run(() =>
                {
                    ProviderControlDynamic p_ticketRequest = new ProviderControlDynamic();
                    //ticketRequest = p_ticketRequest.GetTicketRequestByWorkflowItemId(workflowItem);
                    //if (ticketRequest != null)
                    //{
                    //    string str_json_TicketRequest = JsonConvert.SerializeObject(ticketRequest);
                    //    InvokeOnMainThread(() =>
                    //    {
                    //        loadContent();
                    //        workflowItem.TicketRequestDetails = str_json_TicketRequest;
                    //        conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath, false);
                    //        conn.Update(workflowItem);
                    //        loading.Hide();
                    //    });
                    //}
                    //else
                    //{
                    //    InvokeOnMainThread(() =>
                    //    {
                    //        //CmmIOSFunction.commonAlertMessage("Thông báo", "Chi tiết phiếu không tồn tại, vui lòng thử lại sau hoặc liên hệ bộ phận quản trị. Xin cảm ơn.");
                    //        CmmIOSFunction.commonAlertMessage("EuroWindow", "Không kết nối được server.").Show();
                    //        loading.Hide();
                    //    });
                    //}
                });
            }
        }
        private void ToggelPickerDate()
        {
            //if (view_pickerDate.Hidden)
            //{
            //    view_background_effect.Frame = new CGRect(this.View.Frame.X, this.View.Frame.Y, this.View.Frame.Width, this.View.Frame.Height - 220);
            //    view_pickerDate.Frame = new CGRect(view_pickerDate.Frame.X, this.View.Bounds.Height, view_pickerDate.Frame.Width, 220);
            //    view_pickerDate.Hidden = false;
            //    view_background_effect.Alpha = 0;
            //    UIView.BeginAnimations("toogle_view_pickerDate_slideShow");
            //    UIView.SetAnimationDuration(0.4f);
            //    UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
            //    UIView.SetAnimationRepeatCount(0);
            //    UIView.SetAnimationRepeatAutoreverses(false);
            //    UIView.SetAnimationDelegate(this);
            //    view_pickerDate.Frame = new CGRect(view_pickerDate.Frame.X, this.View.Bounds.Height - 220, view_pickerDate.Frame.Width, 220);
            //    view_pickerDate.Alpha = 1;
            //    view_background_effect.Alpha = 0.5f;
            //    UIView.CommitAnimations();
            //}
            //else
            //{
            //    hiddenPicker();
            //}
        }
        private void ToggelPickerChoice()
        {
            //if (view_picker_choice.Hidden)
            //{
            //    view_background_effect.Frame = new CGRect(this.View.Frame.X, this.View.Frame.Y, this.View.Frame.Width, this.View.Frame.Height - 220);
            //    view_picker_choice.Frame = new CGRect(view_pickerDate.Frame.X, this.View.Bounds.Height, view_pickerDate.Frame.Width, 220);
            //    view_picker_choice.Hidden = false;
            //    view_background_effect.Alpha = 0;
            //    UIView.BeginAnimations("toogle_view_pickerDate_slideShow");
            //    UIView.SetAnimationDuration(0.4f);
            //    UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
            //    UIView.SetAnimationRepeatCount(0);
            //    UIView.SetAnimationRepeatAutoreverses(false);
            //    UIView.SetAnimationDelegate(this);
            //    view_picker_choice.Frame = new CGRect(view_pickerDate.Frame.X, this.View.Bounds.Height - view_picker_choice.Frame.Height, view_pickerDate.Frame.Width, 220);
            //    view_picker_choice.Alpha = 1;
            //    view_background_effect.Alpha = 0.5f;
            //    UIView.CommitAnimations();
            //}
            //else
            //{
            //    hiddenPicker();
            //}
        }
        private void menu_action_Toggle()
        {
            try
            {
                int cell_height = 60;
                int maxheight = lst_actionmore.Count * cell_height;
                //maxheight = maxheight; //60 chieu cao cong them cua bottom

                if (moreaction_view.Alpha == 0)
                {

                    table_actionmore.Alpha = 0;
                    //table_actionmore.Frame = new CGRect(table_actionmore.Frame.X, moreaction_view.Frame.Bottom, table_actionmore.Frame.Width, 0);
                    UIView.BeginAnimations("toogle_docmenu_slideShow");
                    UIView.SetAnimationDuration(0.4f);
                    UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
                    UIView.SetAnimationRepeatCount(0);
                    UIView.SetAnimationRepeatAutoreverses(false);
                    UIView.SetAnimationDelegate(this);

                    table_actionmore.Frame = new CGRect(table_actionmore.Frame.X, moreaction_view.Frame.Height - (maxheight + BT_moreaction_exit.Frame.Height + 60), table_actionmore.Frame.Width, maxheight + 10);
                    table_actionmore.Alpha = 1;
                    moreaction_view.Alpha = 1;
                    UIView.CommitAnimations();
                }
                else
                {
                    table_actionmore.Alpha = 1;
                    //table_actionmore.Frame = new CGRect(table_actionmore.Frame.X, moreaction_view.Frame.Height - maxheight, table_actionmore.Frame.Width, table_actionmore.Frame.Height);
                    UIView.BeginAnimations("toogle_docmenu_slideClose");
                    UIView.SetAnimationDuration(0.4f);
                    UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
                    UIView.SetAnimationRepeatCount(0);
                    UIView.SetAnimationRepeatAutoreverses(false);
                    UIView.SetAnimationDelegate(this);
                    table_actionmore.Alpha = 0;
                    table_actionmore.Frame = new CGRect(table_actionmore.Frame.X, moreaction_view.Frame.Bottom, table_actionmore.Frame.Width, 0);
                    moreaction_view.Alpha = 0;
                    UIView.CommitAnimations();
                }
            }
            catch (Exception ex)
            { Console.WriteLine(ex.ToString()); }
        }
        private void ShowviewIndexMode(int _index)
        {
            switch (_index)
            {
                case 0: // view form
                case 1: // view file
                    isViewQTLC = false;
                    if (!isViewfileMode)
                    {
                        isViewfileMode = true;

                        table_content.Alpha = 0;
                        webView_pdf_mode.Alpha = 1;
                        table_qtlc.Alpha = 0;
                        BT_progress.TintColor = UIColor.White;

                    }
                    else
                    {
                        isViewfileMode = false;

                        table_content.Alpha = 1;
                        webView_pdf_mode.Alpha = 0;
                        table_qtlc.Alpha = 0;
                        BT_progress.TintColor = UIColor.White;

                    }

                    break;
                case 2:
                    isViewQTLC = true;
                    table_content.Alpha = 0;
                    webView_pdf_mode.Alpha = 0;
                    table_qtlc.Alpha = 1;
                    BT_progress.TintColor = UIColor.Orange;

                    break;
            }

        }
        private void hiddenPicker()
        {
            //if (view_picker_choice.Hidden == false)
            //{
            //    UIView.BeginAnimations("toogle_view_picker_choice_slideClose");
            //    UIView.SetAnimationDuration(0.4f);
            //    UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
            //    UIView.SetAnimationRepeatCount(0);
            //    UIView.SetAnimationRepeatAutoreverses(false);
            //    UIView.SetAnimationDelegate(this);
            //    view_picker_choice.Frame = new CGRect(view_picker_choice.Frame.X, this.View.Bounds.Height, view_picker_choice.Frame.Width, 220);
            //    view_picker_choice.Alpha = 0;
            //    view_background_effect.Alpha = 0;
            //    view_picker_choice.Hidden = true;
            //    UIView.CommitAnimations();
            //}

            //if (view_pickerDate.Hidden == false)
            //{
            //    UIView.BeginAnimations("toogle_view_pickerDate_slideHide");
            //    UIView.SetAnimationDuration(0.4f);
            //    UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
            //    UIView.SetAnimationRepeatCount(0);
            //    UIView.SetAnimationRepeatAutoreverses(false);
            //    UIView.SetAnimationDelegate(this);
            //    view_pickerDate.Frame = new CGRect(view_pickerDate.Frame.X, this.View.Bounds.Height, view_pickerDate.Frame.Width, 220);
            //    view_pickerDate.Alpha = 0;
            //    view_background_effect.Alpha = 0;
            //    view_pickerDate.Hidden = true;
            //    UIView.CommitAnimations();
            //}
        }
        public void updateMultichoiceValue(BeanControlDynamicDetail _control, List<ClassDynamicControl> _lstcontrolValue)
        {
            List<ClassMultichoice> lst_multichoice = new List<ClassMultichoice>();
            string str_value = "";
            if (_lstcontrolValue != null && _lstcontrolValue.Count > 0)
            {
                foreach (var item in _lstcontrolValue)
                {
                    str_value += item.ID + ";#" + item.Title + ";#";
                }
            }
            str_value = str_value.TrimEnd(';', '#');
            updateDictValue(_control.DataField, str_value);
        }
        public void updateDictValue(string _key, string _value)
        {
            if (dic_valueObject != null)
            {
                if (dic_valueObject.ContainsKey(_key))
                    dic_valueObject[_key] = _value;

                // gán lai value vào các dynamic control tương ứng
                if (lst_control_hasValue != null)
                {
                    foreach (var item in lst_control_hasValue)
                    {
                        if (dic_valueObject.ContainsKey(item.DataField))
                        {
                            var value = dic_valueObject[item.DataField];
                            item.ControlValue = value;
                        }
                    }
                    table_content.Source = new control_TableSource(lst_control_hasValue, lst_attachFile, this);
                    table_content.ReloadData();

                }
            }

            //ANDROID
            //CmmFunction.SetPropertyValue(tv_ChangeValue, prop, lst_num[num.Value]);
            //dic_values[tag_ViewChangeValue] = lst_dataLookup[num.Value].ID + ";#" + lst_dataLookup[num.Value].Title;
        }
        public void showfull_QtLC_content(BeanQuaTrinhLuanChuyen qtlc)
        {
            FullTextView viewController = (FullTextView)Storyboard.InstantiateViewController("FullTextView");
            viewController.SetContent(qtlc.Title, qtlc.Comment);
            this.NavigationController.PushViewController(viewController, true);
        }
        private void dynamicControlSelected(BeanControlDynamicDetail beanControlDynamicDetail, bool _isExpand)
        {
            PresentationDelegate transitioningDelegate;
            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);

            if (_isExpand)
            {
                FullTextView viewController = (FullTextView)Storyboard.InstantiateViewController("FullTextView");
                string title = "";
                if (CmmVariable.SysConfig.LangCode == "1033")
                    title = beanControlDynamicDetail.TitleEN;
                else
                    title = beanControlDynamicDetail.Title;

                viewController.SetContent(title, beanControlDynamicDetail.ControlValue);
                this.NavigationController.PushViewController(viewController, true);
            }
            else
            {
                // 1: Text, 2: Text Area, 4: TextNum, 8: DateTime, 16: Date, 32: Time, 64: Combobox drop, 128: choice, 256: multi, 8388608: User
                switch (beanControlDynamicDetail.ControlType)
                {
                    case (int)CmmFunction.ControlType.FIELD_USER:
                        ListUserView listUserView = (ListUserView)Storyboard.InstantiateViewController("ListUserView");
                        if (!string.IsNullOrEmpty(beanControlDynamicDetail.ControlValue))
                        {
                            var userID = beanControlDynamicDetail.ControlValue.Split(new string[] { ";#" }, StringSplitOptions.None)[0];

                            var conn = new SQLiteConnection(CmmVariable.M_DataPath);
                            string query = string.Format("SELECT * FROM BeanUser WHERE UserID = ?");
                            var lst_userSelected = conn.Query<BeanUser>(query, userID);

                            //listUserView.setContent(this, false, lst_userSelected, beanControlDynamicDetail);
                            transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                            listUserView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                            listUserView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                            listUserView.TransitioningDelegate = transitioningDelegate;
                            this.PresentViewControllerAsync(listUserView, true);
                        }
                        break;
                    case (int)CmmFunction.ControlType.FIELD_DATE:
                    case (int)CmmFunction.ControlType.FIELD_DATETIME:
                        DateTime dt;
                        if (!string.IsNullOrEmpty(beanControlDynamicDetail.ControlValue))
                            dt = DateTime.Parse(beanControlDynamicDetail.ControlValue);
                        else
                            dt = DateTime.Now;
                        //picker_date.SetDate(CmmIOSFunction.DateTimeToNSDate(dt), true);
                        dic_datetimePickerSelected.Clear();
                        dic_datetimePickerSelected.Add(beanControlDynamicDetail.DataField, beanControlDynamicDetail.ControlValue);
                        ToggelPickerDate();

                        break;
                    case (int)CmmFunction.ControlType.FIELD_CHOICE:
                        loadSingleChoiceControlData(beanControlDynamicDetail);
                        ToggelPickerChoice();
                        break;
                    case (int)CmmFunction.ControlType.FIELD_MULTICHOICE: // tam chua active multichoice
                        //MultiChoiceView multichoiceView = (MultiChoiceView)Storyboard.InstantiateViewController("MultiChoiceView");
                        //multichoiceView.setContent(this, 0, beanControlDynamicDetail);
                        //transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        //multichoiceView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        //multichoiceView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        //multichoiceView.TransitioningDelegate = transitioningDelegate;
                        //this.PresentViewControllerAsync(multichoiceView, true);
                        break;
                    case (int)CmmFunction.ControlType.FIELD_TEXT:
                    case (int)CmmFunction.ControlType.FIELD_TEXT_AREA:
                        //TextViewControlView textViewControl = (TextViewControlView)Storyboard.InstantiateViewController("TextViewControlView");
                        //textViewControl.setContent(this, 0, beanControlDynamicDetail);
                        //transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        //textViewControl.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        //textViewControl.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        //textViewControl.TransitioningDelegate = transitioningDelegate;
                        //this.PresentViewControllerAsync(textViewControl, true);
                        break;
                    case (int)CmmFunction.ControlType.FIELD_TEXT_NUM:
                        NumberControlView numberViewControl = (NumberControlView)Storyboard.InstantiateViewController("NumberControlView");
                        //numberViewControl.setContent(this, 0, beanControlDynamicDetail);
                        showSize = new CGSize(this.View.Frame.Width - 40, 250);
                        transitioningDelegate = new PresentationDelegate(startFrame, showSize, 550);// y: càng lớn view cang cao
                        numberViewControl.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        numberViewControl.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        numberViewControl.TransitioningDelegate = transitioningDelegate;
                        this.PresentViewControllerAsync(numberViewControl, true);
                        break;
                }
            }
        }

        private void attachmentSelected(BeanAttachFile _attachFile)
        {
            PresentationDelegate transitioningDelegate;
            ShowAttachmentView attachmentView = (ShowAttachmentView)Storyboard.InstantiateViewController("ShowAttachmentView");
            attachmentView.setContent(this, _attachFile);
            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            attachmentView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            attachmentView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            attachmentView.TransitioningDelegate = transitioningDelegate;
            this.PresentViewControllerAsync(attachmentView, true);
        }
        private void ActionSelected(ButtonAction action)
        {
            string requiredcols = CmmFunction.ValidateEmptyRequiredField(lst_control_hasValue, dic_valueObject);
            if (!String.IsNullOrEmpty(requiredcols))
            {
                if (CmmVariable.SysConfig.LangCode == "1033")
                {
                    CmmIOSFunction.commonAlertMessage(this, "BPM", "Please insert information into " + requiredcols.Remove(requiredcols.Length - 2) + ".");
                }
                else
                {
                    CmmIOSFunction.commonAlertMessage(this, "BPM", "Vui lòng nhập thông tin vào " + requiredcols.Remove(requiredcols.Length - 2) + ".");
                }
            }
            else
            {
                PresentationDelegate transitioningDelegate;
                CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
                CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);

                switch (action.ID)
                {
                    case (int)WorkflowAction.Action.Next: //  1 - duyet
                        AgreeOrRejectView agreeOrReject = (AgreeOrRejectView)Storyboard.InstantiateViewController("AgreeOrRejectView");
                        //agreeOrReject.setContent(action, this, workflowItem, null);
                        transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        agreeOrReject.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        agreeOrReject.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        agreeOrReject.TransitioningDelegate = transitioningDelegate;
                        this.PresentViewControllerAsync(agreeOrReject, true);
                        break;
                    case (int)WorkflowAction.Action.Approve: // 2 - phe duyet bước cuối
                        AgreeOrRejectView approve = (AgreeOrRejectView)Storyboard.InstantiateViewController("AgreeOrRejectView");
                        //approve.setContent(action, this, workflowItem, null);
                        transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        approve.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        approve.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        approve.TransitioningDelegate = transitioningDelegate;
                        this.PresentViewControllerAsync(approve, true);
                        break;
                    case (int)WorkflowAction.Action.Forward: // 4 - chuyen xu ly
                        ChangeUserProgress changeUserProgress = (ChangeUserProgress)Storyboard.InstantiateViewController("ChangeUserProgress");
                        changeUserProgress.setContent(this, action);
                        transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        changeUserProgress.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        changeUserProgress.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        changeUserProgress.TransitioningDelegate = transitioningDelegate;
                        this.PresentViewControllerAsync(changeUserProgress, true);
                        break;
                    case (int)WorkflowAction.Action.Return: // 8 - tu choi
                        AgreeOrRejectView reject = (AgreeOrRejectView)Storyboard.InstantiateViewController("AgreeOrRejectView");
                        //reject.setContent(action, this, workflowItem, null);
                        transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        reject.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        reject.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        reject.TransitioningDelegate = transitioningDelegate;
                        this.PresentViewControllerAsync(reject, true);
                        break;
                    case (int)WorkflowAction.Action.Reject: // 16 - huy
                        AgreeOrRejectView cancel = (AgreeOrRejectView)Storyboard.InstantiateViewController("AgreeOrRejectView");
                        //cancel.setContent(action, this, workflowItem, null);
                        transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        cancel.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        cancel.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        cancel.TransitioningDelegate = transitioningDelegate;
                        this.PresentViewControllerAsync(cancel, true);
                        break;
                    case (int)WorkflowAction.Action.Recall: // 32 - thu hoi
                        submitAction(action, null);
                        break;
                    case (int)WorkflowAction.Action.RequestInformation: // 64 - yeu cau bo sung
                        RequestAddInfo requestAddInfo = (RequestAddInfo)Storyboard.InstantiateViewController("RequestAddInfo");
                        //requestAddInfo.setContent(this, action, lst_qtlc, workflowItem, null);
                        transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        requestAddInfo.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        requestAddInfo.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        requestAddInfo.TransitioningDelegate = transitioningDelegate;
                        this.PresentViewControllerAsync(requestAddInfo, true);
                        break;
                    case (int)WorkflowAction.Action.RecallAfterApproved: // 128 - thu hoi
                        break;
                    case (int)WorkflowAction.Action.RequestIdea: // 256 - xin y kien tham van
                        break;
                    case (int)WorkflowAction.Action.Idea: // 512 - cho y kien
                        break;
                    case (int)WorkflowAction.Action.Save: // 1028 -  luu
                        break;
                }
            }

        }

        // Thuc hien action tu cac popup hoac form
        public async void submitAction(ButtonAction _buttonAction, List<KeyValuePair<string, string>> _lstExtent)
        {
            loading = new CmmLoading(new CGRect((this.View.Bounds.Width - 200) / 2, (this.View.Bounds.Height - 200) / 2, 200, 200), "Đang xử lý...");
            this.View.Add(loading);
            try
            {
                await Task.Run(() =>
                {
                    bool result = false;
                    ProviderBase b_pase = new ProviderBase();
                    ProviderControlDynamic providerControl = new ProviderControlDynamic();
                    string json = JsonConvert.SerializeObject(dic_valueObject);

                    lstExtent = _lstExtent;

                    if (lstExtent != null && lstExtent.Count > 0)
                        result = providerControl.DynamicAction(_buttonAction.Value, json, JsonConvert.SerializeObject(lst_ControlEnable), workflowItem.ListName, lstExtent);
                    else
                        result = providerControl.DynamicAction(_buttonAction.Value, json, JsonConvert.SerializeObject(lst_ControlEnable), workflowItem.ListName);

                    if (result)
                    {
                        b_pase.UpdateAllDynamicData(true);
                        InvokeOnMainThread(() =>
                        {
                            loading.Hide();

                            if (!isFromPush)
                            {
                                if (this.NavigationController != null)
                                    this.NavigationController.PopViewController(true);
                                else
                                    this.DismissViewControllerAsync(true);
                            }
                            else
                                this.NavigationController.PopToRootViewController(true);

                            //this.NavigationController.PopViewController(true);
                        });
                    }
                    else
                    {
                        InvokeOnMainThread(() =>
                        {
                            loading.Hide();
                            CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("K_Mess_ActionFalse", "Thao tác không thực hiện được, vui lòng thử lại."));
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                loading.Hide();
                Console.WriteLine("ViewRequestDetails - submitAction - ERR: " + ex.ToString());
            }
        }

        #endregion

        #region events
        private void BT_back_TouchUpInside(object sender, EventArgs e)
        {
            if (!isFromPush)
            {
                if (this.NavigationController != null)
                    this.NavigationController.PopViewController(true);
                else
                    this.DismissViewControllerAsync(true);
            }
            else
                this.NavigationController.PopToRootViewController(true);
        }

        [Export("hideKeyboard")]
        private void hideKeyboard()
        {
            this.View.EndEditing(true);
            //ToggelPickerDate();
            hiddenPicker();

            //if (!view_pickerDate.Hidden)
            //{
            //    UIView.BeginAnimations("toogle_view_pickerDate_slideHide");
            //    UIView.SetAnimationDuration(0.4f);
            //    UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
            //    UIView.SetAnimationRepeatCount(0);
            //    UIView.SetAnimationRepeatAutoreverses(false);
            //    UIView.SetAnimationDelegate(this);
            //    view_pickerDate.Frame = new CGRect(view_pickerDate.Frame.X, this.View.Bounds.Height, view_pickerDate.Frame.Width, 220);
            //    view_pickerDate.Alpha = 0;
            //    view_pickerDate.Hidden = true;
            //    UIView.CommitAnimations();
            //}

            if (table_actionmore.Alpha == 1)
            {
                table_actionmore.Alpha = 1;
                //view_background_effect.Alpha = 0.5f;
                //table_action.Frame = new CGRect(0, table_action.Frame.Y, this.View.Bounds.Width, table_action.Frame.Height);
                UIView.BeginAnimations("toogle_docmenu_slideClose");
                UIView.SetAnimationDuration(0.4f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);
                table_actionmore.Frame = new CGRect(0, this.View.Frame.Height, this.View.Bounds.Width, 0);
                table_actionmore.Alpha = 0;
                //view_background_effect.Alpha = 0;
                UIView.CommitAnimations();
            }
        }
        private void BT_viewMode_TouchUpInside(object sender, EventArgs e)
        {
            if (!isViewfileMode) // open file
            {
                UIView.BeginAnimations("show_animationShowmap");
                UIView.SetAnimationDuration(0.2f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);
                ShowviewIndexMode(1);
                UIView.CommitAnimations();

                //BT_viewMode.SetImage(UIImage.FromFile("Icons/icon_formMode30.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
                //BT_viewMode.TintColor = UIColor.Orange;
                checkFileLocalIsExist(str_url_scanPath);
            }
            else // open form
            {
                UIView.BeginAnimations("show_animationShowmonth");
                UIView.SetAnimationDuration(0.2f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);
                ShowviewIndexMode(0);
                UIView.CommitAnimations();

                //BT_viewMode.SetImage(UIImage.FromFile("Icons//icon_fileMode30.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
                //BT_viewMode.TintColor = UIColor.Orange;
            }
        }
        private void BT_attachement_TouchUpInside(object sender, EventArgs e)
        {
            if (lst_attachFile != null && lst_attachFile.Count > 0)
            {
                PresentationDelegate transitioningDelegate;
                CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
                CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);

                AttachmentListView attachment = (AttachmentListView)Storyboard.InstantiateViewController("AttachmentListView");
                //attachment.SetContent(lst_attachFile,"");
                transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                attachment.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;
                attachment.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                attachment.TransitioningDelegate = transitioningDelegate;
                this.PresentViewControllerAsync(attachment, true);
            }
        }
        private void BT_progress_TouchUpInside(object sender, EventArgs e)
        {
            PresentationDelegate transitioningDelegate;
            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);

            ProgressView progressView = (ProgressView)Storyboard.InstantiateViewController("ProgressView");
            progressView.SetContent(workflowItem, "");
            transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            progressView.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;
            progressView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            progressView.TransitioningDelegate = transitioningDelegate;
            this.PresentViewControllerAsync(progressView, true);
        }
        private void BT_action1_TouchUpInside(object sender, EventArgs e)
        {
            ActionSelected(lst_action[0]);
        }
        private void BT_action2_TouchUpInside(object sender, EventArgs e)
        {
            ActionSelected(lst_action[1]);
        }
        private void BT_More_TouchUpInside(object sender, EventArgs e)
        {
            menu_action_Toggle();
        }
        private void BT_moreaction_exit_TouchUpInside(object sender, EventArgs e)
        {
            menu_action_Toggle();
        }
        private void BT_Pickerchoice_Done_TouchUpInside(object sender, EventArgs e)
        {
            if (dic_singleChoiceSelected.Count > 0)
            {
                updateDictValue(dic_singleChoiceSelected.First().Key, dic_singleChoiceSelected.First().Value);
                ToggelPickerChoice();
            }
        }
        #endregion

        #region custom class

        #region dynamic controls source table
        private class control_TableSource : UITableViewSource
        {
            List<BeanControlDynamicDetail> lst_controls;
            List<BeanAttachFile> lst_attachfiles;
            NSString cellIdentifier = new NSString("cell");
            ViewRequestDetails parentView;
            //int limit = 20;
            //bool isLoadMore = true;

            public control_TableSource(List<BeanControlDynamicDetail> _control, List<BeanAttachFile> _attachFiles, ViewRequestDetails _parentview)
            {
                lst_controls = _control;
                //lst_attachfiles = _attachFiles;
                parentView = _parentview;
            }
            public override nint NumberOfSections(UITableView tableView)
            {
                if (lst_attachfiles != null)
                    return 2;
                else
                    return 1;
            }
            public override nint RowsInSection(UITableView tableview, nint section)
            {
                if (section == 0)
                    return lst_controls.Count;
                else
                    return lst_attachfiles.Count;
            }

            public override UIView GetViewForHeader(UITableView tableView, nint section)
            {
                UIView view = new UIView();
                view.Frame = new CGRect(0, 0, parentView.View.Bounds.Width, 5);
                view.BackgroundColor = UIColor.FromRGB(243, 246, 251);
                UILabel line = new UILabel();
                //line.BackgroundColor = UIColor.LightGray.ColorWithAlpha(0.7f);
                //line.Frame = new CGRect(20, 1, parentview.View.Bounds.Width, 1);
                //line.Hidden = true;
                view.Add(line);
                if (section == 1)
                    line.Hidden = false;
                return view;
            }
            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                if (section == 0)
                    return 1;
                else
                    return 5;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                if (indexPath.Section == 0)
                    return 50;
                else
                    return 40;
            }
            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                if (indexPath.Section == 0)
                {
                    var control = lst_controls[indexPath.Row];
                    var contentHeight = StringExtensions.StringHeight(control.ControlValue, UIFont.SystemFontOfSize(13f), tableView.Frame.Width - 160);

                    if (control.Enable)
                        parentView.dynamicControlSelected(control, false);
                    else if (contentHeight >= 31)
                    {
                        parentView.dynamicControlSelected(control, true);
                    }
                }
                else
                {
                    var attach = lst_attachfiles[indexPath.Row];
                    parentView.attachmentSelected(attach);
                }
                tableView.DeselectRow(indexPath, true);
            }
            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                if (indexPath.Section == 0)
                {
                    var control = lst_controls[indexPath.Row];
                    control_cell_custom cell = new control_cell_custom(cellIdentifier);
                    cell.UpdateCell(control);
                    return cell;
                }
                else
                {
                    var attch = lst_attachfiles[indexPath.Row];
                    attachment_cell_custom cell = new attachment_cell_custom(cellIdentifier);
                    cell.UpdateCell(attch);
                    return cell;
                }
            }

        }
        private class control_cell_custom : UITableViewCell
        {
            BeanControlDynamicDetail control { get; set; }
            UILabel lbl_title, lbl_value;

            public control_cell_custom(NSString cellID) : base(UITableViewCellStyle.Default, cellID)
            {
                Accessory = UITableViewCellAccessory.None;
                viewConfiguration();
            }

            private void viewConfiguration()
            {
                ContentView.BackgroundColor = UIColor.White;

                lbl_title = new UILabel()
                {
                    Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
                    TextColor = UIColor.LightGray,
                    TextAlignment = UITextAlignment.Left,
                };

                lbl_value = new UILabel()
                {
                    Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
                    TextAlignment = UITextAlignment.Left,
                    TextColor = UIColor.DarkGray,
                };

                ContentView.AddSubviews(new UIView[] { lbl_title, lbl_value });
            }

            public void UpdateCell(BeanControlDynamicDetail _control)
            {
                control = _control;
                loadData();
            }

            private void loadData()
            {
                try
                {
                    //key
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        lbl_title.Text = control.TitleEN;
                    else //if (CmmVariable.SysConfig.LangCode == "1066")
                        lbl_title.Text = control.Title;

                    //value
                    string value = string.Empty;
                    if (!string.IsNullOrEmpty(control.ControlValue))
                    {
                        value = CmmFunction.FormatDynamicControlDataValue(control.ControlType, control.ControlValue);
                        lbl_value.Text = value;
                    }

                    if (control.Enable == true)
                        lbl_value.TextColor = UIColor.FromRGB(0, 96, 175);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("MainView - Todo_cell_custom - loaddata- ERR: " + ex.ToString());
                }
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                lbl_title.Frame = new CGRect(20, 5, ContentView.Frame.Width - 40, 20);
                lbl_value.Frame = new CGRect(20, 25, ContentView.Frame.Width - 40, 20);
            }
        }

        private class attachment_cell_custom : UITableViewCell
        {
            BeanAttachFile attachment { get; set; }
            UIImageView img_type;
            UILabel lbl_title;

            public attachment_cell_custom(NSString cellID) : base(UITableViewCellStyle.Default, cellID)
            {
                Accessory = UITableViewCellAccessory.None;
                viewConfiguration();
            }

            private void viewConfiguration()
            {
                ContentView.BackgroundColor = UIColor.White;

                img_type = new UIImageView();

                lbl_title = new UILabel()
                {
                    Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
                    TextColor = UIColor.DarkGray,
                    TextAlignment = UITextAlignment.Left,
                    LineBreakMode = UILineBreakMode.TailTruncation
                };

                ContentView.AddSubviews(new UIView[] { img_type, lbl_title });
            }

            public void UpdateCell(BeanAttachFile _attch)
            {
                try
                {
                    attachment = _attch;
                    string fileExt = string.Empty;
                    if (!string.IsNullOrEmpty(attachment.Path))
                        fileExt = attachment.Path.Split('.').Last().ToLower();

                    switch (fileExt)
                    {
                        case "doc":
                        case "docx":
                            img_type.Image = UIImage.FromFile("Icons/icon_docx.png");
                            break;
                        case "pdf":
                            img_type.Image = UIImage.FromFile("Icons/icon_pdf.png");
                            break;
                        case "xls":
                        case "xlsx":
                            img_type.Image = UIImage.FromFile("Icons/icon_xlsx.png");
                            break;
                        case "jpg":
                            img_type.Image = UIImage.FromFile("Icons/icon_image.png");
                            break;
                        case "png":
                            img_type.Image = UIImage.FromFile("Icons/icon_image.png");
                            break;
                        case "jpeg":
                            img_type.Image = UIImage.FromFile("Icons/icon_image.png");
                            break;
                        default:
                            img_type.Image = UIImage.FromFile("Icons/icon_file_blank.png");
                            break;
                    }

                    //title
                    lbl_title.Text = attachment.Title;

                }
                catch (Exception ex)
                {
                    Console.WriteLine("attachment_cell_custom - UpdateCell - ERR: " + ex.ToString());
                }
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                img_type.Frame = new CGRect(20, 10, 20, 20);
                lbl_title.Frame = new CGRect(img_type.Frame.Right + 15, 5, this.ContentView.Frame.Width - 70, 30);
            }
        }
        #endregion

        #region action menu
        private class MenuAction_TableSource : UITableViewSource
        {
            static readonly NSString cellIdentifier = new NSString("Moreaction_cell");
            List<ButtonAction> items;
            ViewRequestDetails parentview;

            public MenuAction_TableSource(List<ButtonAction> lst_items, ViewRequestDetails controler)
            {
                parentview = controler;
                items = lst_items;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return items.Count;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 60;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                parentview.ActionSelected(items[indexPath.Row]);
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                Moreaction_cell cell = tableView.DequeueReusableCell(cellIdentifier, indexPath) as Moreaction_cell;
                if (indexPath.Row == items.Count - 1)
                    cell.UpdateCell(items[indexPath.Row], true);
                else
                    cell.UpdateCell(items[indexPath.Row], false);

                return cell;

            }
        }

        #endregion

        #region picker type
        private class SingleChoice_PickerSource : UIPickerViewModel
        {
            ViewRequestDetails parentview;
            BeanControlDynamicDetail control;
            List<ClassDynamicControl> lst_controlValue;
            List<KeyValuePair<string, string>> lst_level;

            public SingleChoice_PickerSource(BeanControlDynamicDetail _control, List<ClassDynamicControl> _lst_controlvalue, ViewRequestDetails _parentview)
            {
                control = _control;
                parentview = _parentview;
                lst_controlValue = _lst_controlvalue;
            }

            public override nint GetComponentCount(UIPickerView pickerView)
            {
                return 1;
            }

            public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
            {
                return lst_controlValue.Count;
            }

            public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
            {
                UILabel lbl_title = new UILabel();
                lbl_title.Font = UIFont.SystemFontOfSize(13, UIFontWeight.Semibold);
                lbl_title.TextColor = UIColor.DarkGray;

                if (lst_controlValue != null)
                    lbl_title.Text = lst_controlValue[(int)row].Title;

                lbl_title.TextAlignment = UITextAlignment.Center;
                return lbl_title;
            }

            public override void Selected(UIPickerView picker, nint row, nint component)
            {
                var selectedIndex = lst_controlValue[(int)row];
                parentview.dic_singleChoiceSelected.Clear();
                parentview.dic_singleChoiceSelected.Add(control.DataField, selectedIndex.ID);

            }
        }
        #endregion

        #region class multichoice
        private class ClassMultichoice
        {
            public string ID { get; set; }
            public string Title { get; set; }
        }
        #endregion

        #endregion
    }
}


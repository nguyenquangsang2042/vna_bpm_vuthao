using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.iPad.CustomControlClass;
using BPMOPMobile.iPad.IOSClass;
using BPMOPMobile.iPad.KanBanCustomClass;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite;
using UIKit;
using ObjCRuntime;

namespace BPMOPMobile.iPad
{
    public partial class KanBanView : UIViewController
    {
        BeanWorkflow workflowSelected { get; set; }
        public BeanAppBaseExt workflowItemSelected { get; set; }
        public KanBanModel kanBanModel { get; set; }
        List<BeanWorkflowStepDefine> beanWorkflowStepDefines;
        Dictionary<string, List<BeanAppBaseExt>> dict_workFlowByStep = new Dictionary<string, List<BeanAppBaseExt>>();
        List<KeyValuePair<string, bool>> lst_sectionState;
        CollectionWorkFlowStep_Source collectionWorkFlowStep_Source;
        List<ClassMenu> lst_menu_cate;
        List<BeanAppBaseExt> lst_workflowItems;
        List<BeanAppBaseExt> lst_search_workflowItems;
        public CmmLoading loading;
        string Json_FormDataString = string.Empty;
        string str_json_FormDefineInfo = string.Empty;
        ClassMenu currentMenu { get; set; }
        Custom_CalendarView custom_CalendarView;
        bool isShowCalendar;
        int index_ContentSearch = 1; //1: search content | 2: search user
        int currentIndex_menu = 0; //0: board - 1: list
        Dictionary<string, int> dict_indexRangeDayChoice;
        int indexRangeSelected;

        DateTime fromDate_default;
        DateTime toDate_default;
        DateTime fromDateSelected;
        DateTime toDateSelected;

        public readonly int[] WaitingListID = { 1, 4 };                // AppStatusID Chờ phê duyệt (đang lưu - chờ xử lý - bổ sung thông tin - tham vấn - yêu cầu hiệu chỉnh)
        public readonly int[] ApprovedListID = { 8 };                  // AppStatusID Phê duyệt
        public readonly int[] RejectedListID = { 16, 64 };             // AppStatusID Từ chối (từ chối - hủy)
        private string hintTextSearchTitle = CmmFunction.GetTitle("TEXT_SEARCH", "Tìm kiếm theo tên quy trình");
        ButtonsActionBotBar buttonActionBotBar;
        private nfloat heightView_filterdate;
        public const int _ApprovedStepID = -1;
        public const int _RejectedStepID = -2;
        List<BeanAppBaseExt> lstAllItem = new List<BeanAppBaseExt>();

        public KanBanView(IntPtr handle) : base(handle)
        {
        }

        #region override

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UITapGestureRecognizer gesture = new UITapGestureRecognizer(() =>
            {
                View.EndEditing(true);
            });

            gesture.ShouldReceiveTouch += delegate (UIGestureRecognizer recognizer, UITouch touch)
            {
                var touchView = touch.View.Class.Name;

                return true;
            };

            gesture.CancelsTouchesInView = false;
            View.AddGestureRecognizer(gesture);

            CloseCustomCalendar();
            ViewConfiguration();
            LoadStatusFilterCategory();

            dict_indexRangeDayChoice = new Dictionary<string, int>
            {
                { "today", 0 },
                { "yesterday", 1 },
                { "7days", 7 },
                { "30days", 30 },
                { "custom", -1},
            };

            //default
            indexRangeSelected = dict_indexRangeDayChoice["30days"];

            toDateSelected = DateTime.Now.Date;
            toDate_default = toDateSelected;
            fromDateSelected = toDateSelected.AddDays(-indexRangeSelected);
            fromDate_default = fromDateSelected;

            if (CmmVariable.SysConfig.LangCode == "1033")
            {
                lbl_todate.Text = toDateSelected.ToString("MM/dd/yyyy");
                lbl_fromdate.Text = fromDateSelected.ToString("MM/dd/yyyy");
            }
            else
            {
                lbl_todate.Text = toDateSelected.ToString("dd/MM/yyyy");
                lbl_fromdate.Text = fromDateSelected.ToString("dd/MM/yyyy");
            }

            UpdateLbl_dateText();

            currentMenu = lst_menu_cate[0];
            lbl_category.Text = currentMenu.title;

            if (CmmVariable.SysConfig.LangCode == "1033")
                lbl_topBar_title.Text = lbl_topBar_title.Text + workflowSelected.TitleEN;
            else //if (CmmVariable.SysConfig.LangCode == "1066")
                lbl_topBar_title.Text = lbl_topBar_title.Text + workflowSelected.Title;

            SetLangTitle();
            LoadContent("", true);
            //LoadContentList();
            SwitchBoardOrList();

            #region delelegate
            BT_back.TouchUpInside += BT_back_TouchUpInside;
            BT_category.TouchUpInside += BT_category_TouchUpInside;
            BT_filterDate.TouchUpInside += BT_filterDate_TouchUpInside;
            BT_date_0.TouchUpInside += BT_date_0_TouchUpInside;
            BT_date_1.TouchUpInside += BT_date_1_TouchUpInside;
            BT_date_2.TouchUpInside += BT_date_2_TouchUpInside;
            BT_date_3.TouchUpInside += BT_date_3_TouchUpInside;

            BT_filter_fromdate.TouchUpInside += BT_filter_fromdate_TouchUpInside;
            BT_filter_todate.TouchUpInside += BT_filter_todate_TouchUpInside;

            BT_filterDateChoice.TouchUpInside += BT_filterDateChoice_TouchUpInside;
            tf_search_title.Started += delegate
            {
                tf_search_nxl.Text = "";
                //index_ContentSearch = 1;
            };
            //tf_search_title.EditingChanged += Tf_search_title_EditingChanged;
            tf_search_title.ReturnKeyType = UIReturnKeyType.Search;
            tf_search_title.ShouldReturn = (tf) =>
            {
                tf_search_title.EndEditing(true);
                Tf_search_title_EditingChanged(tf, null);
                return true;
            };

            tf_search_nxl.Started += delegate
            {
                tf_search_title.Text = "";
                //index_ContentSearch = 2;
            };
            //tf_search_nxl.EditingChanged += Tf_search_nxl_EditingChanged;
            tf_search_nxl.ReturnKeyType = UIReturnKeyType.Search;
            tf_search_nxl.ShouldReturn = (tf) =>
            {
                tf_search_nxl.EndEditing(true);
                Tf_search_nxl_EditingChanged(tf, null);
                return true;
            };
            #endregion
        }

        #endregion

        #region private - public method
        public void SetContent(BeanWorkflow _workflow, int _indexMenu)
        {
            workflowSelected = _workflow;
            currentIndex_menu = _indexMenu;
        }

        private void ViewConfiguration()
        {
            var flowLayout = new UICollectionViewFlowLayout()
            {
                SectionInset = new UIEdgeInsets(0, 0, 0, 0),
                ScrollDirection = UICollectionViewScrollDirection.Horizontal,
                MinimumInteritemSpacing = 2, // minimum spacing between cells
                MinimumLineSpacing = 20 // minimum spacing between rows if ScrollDirection is Vertical or between columns if Horizontal
            };

            view_filterdate.Layer.ShadowColor = UIColor.DarkGray.CGColor;
            view_filterdate.Layer.ShadowPath = UIBezierPath.FromRect(new CGRect(2, 2, view_filterdate.Frame.Width, view_filterdate.Frame.Height)).CGPath;
            view_filterdate.Layer.ShadowRadius = 5;
            view_filterdate.Layer.ShadowOffset = new CGSize(0, 2);
            view_filterdate.Layer.ShadowOpacity = 1;
            view_filterdate.Layer.CornerRadius = 5;
            view_filterdate.ClipsToBounds = false;

            view_fromdate.Layer.BorderWidth = 1;
            view_fromdate.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;
            view_fromdate.Layer.CornerRadius = 3;
            view_fromdate.ClipsToBounds = true;

            view_todate.Layer.BorderWidth = 1;
            view_todate.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;
            view_todate.Layer.CornerRadius = 3;
            view_todate.ClipsToBounds = true;

            custom_CalendarView = Custom_CalendarView.Instance;
            custom_CalendarView.viewController = this;

            view_content.AddSubview(custom_CalendarView);
            view_content.BringSubviewToFront(custom_CalendarView);

            KanbanCollection.SetCollectionViewLayout(flowLayout, true);
            KanbanCollection.RegisterClassForCell(typeof(WorkFlowItem_CollectionCell), WorkFlowItem_CollectionCell.CellID);
            KanbanCollection.AllowsMultipleSelection = false;
            //KanbanCollection.AlwaysBounceVertical = true;
        }

        private void SetLangTitle()
        {
            CmmIOSFunction.SetLangToView(this.View);

            if (CmmVariable.SysConfig.LangCode == "1033")
                lbl_topBar_title.Text = workflowSelected.TitleEN;
            else //if (CmmVariable.SysConfig.LangCode == "1066")
                lbl_topBar_title.Text = workflowSelected.Title;
            tf_search_title.Placeholder = CmmFunction.GetTitle("TEXT_CONTENT", "Nội dung");
            tf_search_nxl.Placeholder = CmmFunction.GetTitle("TEXT_USER_PROCESS", "Người xử lý");
            //lbl_catelogy.Placeholder = CmmFunction.GetTitle("TEXT_STATUS", "Tình trạng");
            //tf_date.Placeholder = CmmFunction.GetTitle("TEXT_LIMIT", "Phạm vi");

            var str_transalte = lbl_topBar_title.Text;
            NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
            att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(0, str_transalte.Length));

            lbl_topBar_title.AttributedText = att;
        }

        private void LoadContent_bk(string searchParam)
        {
            try
            {
                //var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);

                //string query_step = string.Format("SELECT * FROM BeanWorkflowStepDefine WHERE WorkflowID = ?");
                //var lst_step = conn.Query<BeanWorkflowStepDefine>(query_step, workflowSelected.WorkflowID);

                //if (lst_step != null && lst_step.Count > 0)
                //{
                //    BeanWorkflowStepDefine beanWorkflowStep_approved = new BeanWorkflowStepDefine();
                //    beanWorkflowStep_approved.WorkflowStepDefineID = 10;
                //    beanWorkflowStep_approved.Title = CmmFunction.GetTitle("TEXT_APPROVED", "Đã phê duyệt");
                //    beanWorkflowStep_approved.WorkflowID = workflowSelected.WorkflowID;

                //    BeanWorkflowStepDefine beanWorkflowStep_rejected = new BeanWorkflowStepDefine();
                //    beanWorkflowStep_rejected.WorkflowStepDefineID = 6;
                //    beanWorkflowStep_rejected.Title = CmmFunction.GetTitle("TEXT_REJECT", "Từ chối");
                //    beanWorkflowStep_rejected.WorkflowID = workflowSelected.WorkflowID;

                //    lst_step.Add(beanWorkflowStep_approved); lst_step.Add(beanWorkflowStep_rejected);

                //    dict_workFlowByStep = new Dictionary<string, List<BeanWorkflowItem>>();
                //    string date_filter = string.Empty;

                //    if (fromDateSelected.Year.ToString() != "1" && toDateSelected.Year.ToString() != "1")
                //        date_filter = string.Format("Created IS NOT NULL AND (Created >= '{0}' AND Created <= '{1}')", fromDateSelected.ToString("yyyy-MM-dd"), toDateSelected.AddDays(1).ToString("yyyy-MM-dd"));

                //    foreach (var item in lst_step)
                //    {
                //        List<BeanWorkflowItem> lst_res = new List<BeanWorkflowItem>();
                //        // tat ca
                //        if (currentMenu.ID == 0)
                //        {
                //            // Da phe duyet
                //            if (item.WorkflowStepDefineID == 10)
                //            {
                //                string query_approved = string.Format("SELECT * FROM BeanWorkflowItem WHERE {0} AND WorkflowID = ? AND ActionStatusId = 10 ORDER BY Created DESC", date_filter);
                //                lst_res = conn.Query<BeanWorkflowItem>(query_approved, item.WorkflowID);
                //            }
                //            // Da bi tu choi
                //            else if (item.WorkflowStepDefineID == 6)
                //            {
                //                string query_approved = string.Format("SELECT * FROM BeanWorkflowItem WHERE {0} AND WorkflowID = ? AND ActionStatusId = 6 ORDER BY Created DESC", date_filter);
                //                lst_res = conn.Query<BeanWorkflowItem>(query_approved, item.WorkflowID);
                //            }
                //            else
                //            {
                //                string query_worflow = string.Format("SELECT * FROM BeanWorkflowItem WHERE {0} AND WorkflowID = ? AND Step = ? AND ActionStatusId NOT IN (10,6) ORDER BY Created DESC", date_filter);
                //                lst_res = conn.Query<BeanWorkflowItem>(query_worflow, item.WorkflowID, item.Step);
                //            }

                //            if (!string.IsNullOrEmpty(searchParam))
                //            {
                //                lst_search_workflowItems = lst_res;
                //                List<BeanWorkflowItem> _lst_searchResult = new List<BeanWorkflowItem>();

                //                //search by title
                //                if (index_ContentSearch == 1)
                //                {
                //                    _lst_searchResult = (from wfi in lst_search_workflowItems
                //                                         where (!string.IsNullOrEmpty(wfi.Content) && CmmFunction.removeSignVietnamese(wfi.Content.ToLowerInvariant()).Contains(searchParam))
                //                                         select wfi).ToList();
                //                }
                //                // search by assignedTo
                //                else
                //                {
                //                    _lst_searchResult = (from wfi in lst_search_workflowItems
                //                                         where (!string.IsNullOrEmpty(wfi.AssignedToName) && CmmFunction.removeSignVietnamese(wfi.AssignedToName.ToLowerInvariant()).Contains(searchParam))
                //                                         select wfi).ToList();
                //                }
                //                dict_workFlowByStep.Add(item.Title, _lst_searchResult);
                //            }
                //            else
                //                dict_workFlowByStep.Add(item.Title, lst_res);
                //        }
                //        // cho phe duyet
                //        else if (currentMenu.ID == 1)
                //        {
                //            if (item.WorkflowStepDefineID == 10)
                //            {
                //                string query_approved = string.Format("SELECT * FROM BeanWorkflowItem WHERE {0} AND WorkflowID = ? AND ActionStatusId = 10 ORDER BY Created DESC", date_filter);
                //                lst_res = conn.Query<BeanWorkflowItem>(query_approved, item.WorkflowID);
                //                //if (lst_workflowItem != null && lst_workflowItem.Count > 0)
                //            }
                //            // Da bi tu choi
                //            else if (item.WorkflowStepDefineID == 6)
                //            {
                //                string query_approved = string.Format("SELECT * FROM BeanWorkflowItem WHERE {0} AND WorkflowID = ? AND ActionStatusId = 6 ORDER BY Created DESC", date_filter);
                //                lst_res = conn.Query<BeanWorkflowItem>(query_approved, item.WorkflowID);
                //            }
                //            else
                //            {
                //                string query_worflow = string.Format("SELECT * FROM BeanWorkflowItem WHERE {0} AND WorkflowID = ? AND Step = ? AND ActionStatusId IN (1,2,3,4,5) ORDER BY Created DESC", date_filter);
                //                lst_res = conn.Query<BeanWorkflowItem>(query_worflow, item.WorkflowID, item.Step);
                //            }

                //            if (!string.IsNullOrEmpty(searchParam))
                //            {
                //                lst_search_workflowItems = lst_res;
                //                List<BeanWorkflowItem> _lst_searchResult = new List<BeanWorkflowItem>();

                //                //search by title
                //                if (index_ContentSearch == 1)
                //                {
                //                    _lst_searchResult = (from wfi in lst_search_workflowItems
                //                                         where (!string.IsNullOrEmpty(wfi.Content) && CmmFunction.removeSignVietnamese(wfi.Content.ToLowerInvariant()).Contains(searchParam))
                //                                         select wfi).ToList();
                //                }
                //                // search by assignedTo
                //                else
                //                {
                //                    _lst_searchResult = (from wfi in lst_search_workflowItems
                //                                         where (!string.IsNullOrEmpty(wfi.AssignedToName) && CmmFunction.removeSignVietnamese(wfi.AssignedToName.ToLowerInvariant()).Contains(searchParam))
                //                                         select wfi).ToList();
                //                }
                //                dict_workFlowByStep.Add(item.Title, _lst_searchResult);
                //            }
                //            else
                //                dict_workFlowByStep.Add(item.Title, lst_res);
                //        }
                //        // da phe duyet
                //        else if (currentMenu.ID == 2)
                //        {
                //            // Da phe duyet
                //            if (item.WorkflowStepDefineID == 10)
                //            {
                //                string query_approved = string.Format("SELECT * FROM BeanWorkflowItem WHERE {0} AND WorkflowID = ? AND ActionStatusId = 10 ORDER BY Created DESC", date_filter);
                //                lst_res = conn.Query<BeanWorkflowItem>(query_approved, item.WorkflowID);
                //            }
                //            else
                //            {
                //                //string query_worflow = @"SELECT * FROM BeanWorkflowItem WHERE WorkflowID = ? AND Step = ? AND ActionStatusId NOT IN (10,6) ";
                //                ////List<BeanWorkflowItem> lst_workflowItem = conn.Query<BeanWorkflowItem>(query_worflow, item.WorkflowID, item.Step);
                //                lst_res = new List<BeanWorkflowItem>();
                //            }

                //            if (!string.IsNullOrEmpty(searchParam))
                //            {
                //                lst_search_workflowItems = lst_res;
                //                List<BeanWorkflowItem> _lst_searchResult = new List<BeanWorkflowItem>();

                //                //search by title
                //                if (index_ContentSearch == 1)
                //                {
                //                    _lst_searchResult = (from wfi in lst_search_workflowItems
                //                                         where (!string.IsNullOrEmpty(wfi.Content) && CmmFunction.removeSignVietnamese(wfi.Content.ToLowerInvariant()).Contains(searchParam))
                //                                         select wfi).ToList();
                //                }
                //                // search by assignedTo
                //                else
                //                {
                //                    _lst_searchResult = (from wfi in lst_search_workflowItems
                //                                         where (!string.IsNullOrEmpty(wfi.AssignedToName) && CmmFunction.removeSignVietnamese(wfi.AssignedToName.ToLowerInvariant()).Contains(searchParam))
                //                                         select wfi).ToList();
                //                }
                //                dict_workFlowByStep.Add(item.Title, _lst_searchResult);
                //            }
                //            else
                //                dict_workFlowByStep.Add(item.Title, lst_res);

                //        }
                //        // bi tu choi
                //        else if (currentMenu.ID == 3)
                //        {
                //            if (item.WorkflowStepDefineID == 6)
                //            {
                //                string query_approved = string.Format("SELECT * FROM BeanWorkflowItem WHERE {0} AND WorkflowID = ? AND ActionStatusId = 6 ORDER BY Created DESC", date_filter);
                //                lst_res = conn.Query<BeanWorkflowItem>(query_approved, item.WorkflowID);
                //            }
                //            else
                //            {
                //                //string query_worflow = @"SELECT * FROM BeanWorkflowItem WHERE WorkflowID = ? AND Step = ? AND ActionStatusId NOT IN (10,6) ";
                //                ////List<BeanWorkflowItem> lst_workflowItem = conn.Query<BeanWorkflowItem>(query_worflow, item.WorkflowID, item.Step);
                //                lst_res = new List<BeanWorkflowItem>();
                //            }

                //            if (!string.IsNullOrEmpty(searchParam))
                //            {
                //                lst_search_workflowItems = lst_res;
                //                List<BeanWorkflowItem> _lst_searchResult = new List<BeanWorkflowItem>();

                //                //search by title
                //                if (index_ContentSearch == 1)
                //                {
                //                    _lst_searchResult = (from wfi in lst_search_workflowItems
                //                                         where (!string.IsNullOrEmpty(wfi.Content) && CmmFunction.removeSignVietnamese(wfi.Content.ToLowerInvariant()).Contains(searchParam))
                //                                         select wfi).ToList();
                //                }
                //                // search by assignedTo
                //                else
                //                {
                //                    _lst_searchResult = (from wfi in lst_search_workflowItems
                //                                         where (!string.IsNullOrEmpty(wfi.AssignedToName) && CmmFunction.removeSignVietnamese(wfi.AssignedToName.ToLowerInvariant()).Contains(searchParam))
                //                                         select wfi).ToList();
                //                }
                //                dict_workFlowByStep.Add(item.Title, _lst_searchResult);
                //            }
                //            else
                //                dict_workFlowByStep.Add(item.Title, lst_res);
                //        }
                //    }
                //}

                //collectionWorkFlowStep_Source = new CollectionWorkFlowStep_Source(this, dict_workFlowByStep, lst_sectionState);
                //KanbanCollection.Source = collectionWorkFlowStep_Source;
                //KanbanCollection.Delegate = new CustomFlowLayoutDelegate(this, collectionWorkFlowStep_Source, KanbanCollection);
                //KanbanCollection.ReloadData();

            }
            catch (Exception ex)
            {
                Console.WriteLine("KanBanView - LoadContent - Err: " + ex.ToString());
            }
        }

        async void LoadContent(string searchParam = "", bool isFirstLoad = false)
        {
            loading = new CmmLoading(new CGRect(KanbanCollection.Center.X - 100, KanbanCollection.Center.Y - 100, 200, 200), "Đang xử lý...");
            this.View.Add(loading);
            this.View.BringSubviewToFront(loading);
            await Task.Run(() =>
            {
                try
                {
                    if (isFirstLoad)
                        lstAllItem = GetListKanBanOnline(workflowSelected.WorkflowID.ToString());

                    if (lstAllItem != null || lstAllItem.Count > 0)
                    {
                        var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                        var StatusGroup = new HashSet<int>() { };

                        string query_step = string.Format("SELECT * FROM BeanWorkflowStepDefine WHERE WorkflowID = ?");
                        var lst_step = conn.Query<BeanWorkflowStepDefine>(query_step, workflowSelected.WorkflowID);
                        if (lst_step != null && lst_step.Count > 0)
                        {
                            //Thêm cột phê duyệt
                            BeanWorkflowStepDefine beanWorkflowStep_approved = new BeanWorkflowStepDefine();
                            beanWorkflowStep_approved.WorkflowStepDefineID = 10;
                            beanWorkflowStep_approved.Title = CmmFunction.GetTitle("TEXT_APPROVED", "Đã phê duyệt");
                            beanWorkflowStep_approved.WorkflowID = workflowSelected.WorkflowID;

                            //Thêm cột từ chối
                            BeanWorkflowStepDefine beanWorkflowStep_rejected = new BeanWorkflowStepDefine();
                            beanWorkflowStep_rejected.WorkflowStepDefineID = 6;
                            beanWorkflowStep_rejected.Title = CmmFunction.GetTitle("TEXT_REJECT", "Từ chối");
                            beanWorkflowStep_rejected.WorkflowID = workflowSelected.WorkflowID;

                            lst_step.Add(beanWorkflowStep_approved);
                            lst_step.Add(beanWorkflowStep_rejected);

                            dict_workFlowByStep = new Dictionary<string, List<BeanAppBaseExt>>();
                            string date_filter = string.Empty;

                            //ngày
                            if (fromDateSelected.Year.ToString() != "1" && toDateSelected.Year.ToString() != "1")
                                date_filter = string.Format("Created IS NOT NULL AND (Created >= '{0}' AND Created <= '{1}') AND ", fromDateSelected.ToString("yyyy-MM-dd"), toDateSelected.AddDays(1).ToString("yyyy-MM-dd"));

                            //Filter từ category
                            switch (currentMenu.ID)
                            {
                                case 0: // "Tất cả" 
                                    {
                                        break;
                                    }
                                case 1: // Chờ phê duyệt
                                    {
                                        foreach (var itemStatus in WaitingListID)
                                            StatusGroup.Add(itemStatus);
                                        break;
                                    }
                                case 2: // đã duyệt
                                    {
                                        foreach (var itemStatus in ApprovedListID)
                                            StatusGroup.Add(itemStatus);
                                        break;
                                    }
                                case 3: // từ chối
                                    {
                                        foreach (var itemStatus in RejectedListID)
                                            StatusGroup.Add(itemStatus);
                                        break;
                                    }
                                default:
                                    break;
                            }

                            foreach (var item in lst_step)
                            {
                                List<BeanAppBaseExt> lst_res = new List<BeanAppBaseExt>();
                                var StatusGroupInItem = new HashSet<int>() { };
                                string strStatusGroup = "";
                                string strStep = "";
                                bool flagWaiting = false;

                                //Filter trong từ item
                                // Da phe duyet
                                if (item.WorkflowStepDefineID == 10)
                                {
                                    foreach (var itemStatus in ApprovedListID)
                                        StatusGroupInItem.Add(itemStatus);
                                    item.Step = _ApprovedStepID;
                                }
                                // Da bi tu choi
                                else if (item.WorkflowStepDefineID == 6)
                                {
                                    foreach (var itemStatus in RejectedListID)
                                        StatusGroupInItem.Add(itemStatus);
                                    item.Step = _RejectedStepID;
                                }
                                else
                                {
                                    foreach (var itemStatus in RejectedListID)
                                        StatusGroupInItem.Add(itemStatus);
                                    foreach (var itemStatus in ApprovedListID)
                                        StatusGroupInItem.Add(itemStatus);
                                    flagWaiting = true;
                                    strStep = string.Format(@" AND Step = {0} ", item.Step);
                                }

                                // query
                                if (Reachability.detectNetWork())
                                {
                                    if (lstAllItem != null && lstAllItem.Count > 0)
                                    {
                                        /*lst_res = lstAllItem.Where(x => ((StatusGroupInItem != null && StatusGroupInItem.Count > 0) ?
                                        x.StatusGroup.HasValue && (flagWaiting ?
                                            !StatusGroupInItem.Contains(x.StatusGroup.Value)
                                            : StatusGroupInItem.Contains(x.StatusGroup.Value))
                                        : true)).ToList();*/
                                        //var lst_res1 = lstAllItem.Where(x => (x.Step.HasValue && x.Step.Value == item.Step)).ToList();//Phần còn lại
                                        //var lst_res2 = lstAllItem.Where(x => (x.Created.HasValue && x.Created.Value >= fromDateSelected && x.Created.Value <= toDateSelected)).ToList();
                                        //var lst_res3 = lstAllItem.Where(x => (x.ApprovalStatus.HasValue && x.ApprovalStatus.Value != 0)).ToList();

                                        lst_res = lstAllItem.Where(
                                            x => (x.Step.HasValue && x.Step.Value == item.Step
                                            && ((currentMenu.ID == 2 && x.Step.Value == _ApprovedStepID) // filter đã xử lý
                                            || (currentMenu.ID == 3 && x.Step.Value == _RejectedStepID)// filter đã từ chối
                                            || (currentMenu.ID == 1 && (x.Step.Value != _ApprovedStepID && x.Step.Value != _RejectedStepID)) //filter chờ xử lý
                                            || (currentMenu.ID == 0))//filter all tình trạng
                                            && (x.Created.HasValue && x.Created.Value >= fromDateSelected && x.Created.Value <= toDateSelected.AddDays(1))
                                            && (x.ApprovalStatus.HasValue && x.ApprovalStatus.Value != 0))
                                        ).OrderByDescending(o => o.Created).ToList();
                                    }
                                }
                                else
                                {
                                    if (StatusGroup.Count > 0)
                                    {
                                        strStatusGroup = string.Format(@" AND( StatusGroup IN ({0})", String.Join(", ", StatusGroup));
                                        if (StatusGroupInItem.Count > 0)
                                        {
                                            if (flagWaiting)
                                                strStatusGroup += string.Format(@" AND StatusGroup NOT IN ({0}) ) ", String.Join(", ", StatusGroupInItem));
                                            else
                                                strStatusGroup += string.Format(@" AND StatusGroup IN ({0}) ) ", String.Join(", ", StatusGroupInItem));
                                        }
                                        else
                                            strStatusGroup += " ) ";
                                    }
                                    else
                                    {
                                        if (flagWaiting)
                                            strStatusGroup += string.Format(@" AND StatusGroup NOT IN ({0}) ", String.Join(", ", StatusGroupInItem));
                                        else
                                            strStatusGroup += string.Format(@" AND StatusGroup IN ({0}) ", String.Join(", ", StatusGroupInItem));
                                    }
                                    string query_approved = string.Format(@"SELECT * FROM BeanAppBase 
                                                                     WHERE {0} WorkflowID = {1} AND ResourceCategoryId <> 16 {2} {3}
                                                                     ORDER BY Created DESC",
                                                                           date_filter, item.WorkflowID, strStatusGroup, strStep);
                                    lst_res = conn.Query<BeanAppBaseExt>(query_approved);
                                }

                                //search
                                if (!string.IsNullOrEmpty(searchParam) && lst_res.Count > 0)// && searchParam.Length > 2
                                {
                                    lst_search_workflowItems = lst_res;
                                    List<BeanAppBaseExt> _lst_searchResult = new List<BeanAppBaseExt>();
                                    if (index_ContentSearch == 1)//search by title
                                    {
                                        _lst_searchResult = (from wfi in lst_search_workflowItems
                                                             where (!string.IsNullOrEmpty(wfi.Content)
                                                             && CmmFunction.removeSignVietnamese(wfi.Content.ToLowerInvariant()).Contains(CmmFunction.removeSignVietnamese(searchParam)))
                                                             select wfi).ToList();
                                    }
                                    else if (index_ContentSearch == 2)//search by người xử lý
                                    {
                                        string query_user = string.Format(@"SELECT * FROM BeanUser WHERE FullName like '%{0}%' ",
                                            CmmFunction.removeSignVietnamese(searchParam).ToLowerInvariant().Trim());//tf_search_nxl.Text
                                        var list_user = conn.Query<BeanUser>(query_user);

                                        _lst_searchResult = (from wfi in lst_search_workflowItems
                                                             where (!string.IsNullOrEmpty(wfi.AssignedTo) && list_user.Exists(o => string.Compare(o.ID.ToLowerInvariant(), wfi.AssignedTo.ToLowerInvariant()) == 0))
                                                             select wfi).ToList();
                                    }
                                    if (dict_workFlowByStep.Where(s => s.Key == item.Title).ToList().Count == 0)
                                        dict_workFlowByStep.Add(item.Title, _lst_searchResult);
                                }
                                else
                                {
                                    if (dict_workFlowByStep.Where(s => s.Key == item.Title).ToList().Count == 0)
                                        dict_workFlowByStep.Add(item.Title, lst_res);
                                }
                            }
                        }

                    }
                    else
                    {
                        dict_workFlowByStep = new Dictionary<string, List<BeanAppBaseExt>>();
                        lst_sectionState = new List<KeyValuePair<string, bool>>();
                    }

                    InvokeOnMainThread(() =>
                    {
                        collectionWorkFlowStep_Source = new CollectionWorkFlowStep_Source(this, dict_workFlowByStep, lst_sectionState);
                        KanbanCollection.Source = collectionWorkFlowStep_Source;
                        KanbanCollection.Delegate = new CustomFlowLayoutDelegate(this, collectionWorkFlowStep_Source, KanbanCollection);
                        KanbanCollection.ReloadData();
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("KanBanView - LoadContent - Err: " + ex.ToString());
                }
                finally
                {
                    InvokeOnMainThread(() =>
                    {
                        loading.Hide();
                    });
                }
            });
        }

        /// <summary>
        /// Điều chỉnh từ ngày 15.06.22 lấy danh sách online để đồng bộ với web
        /// </summary>
        List<BeanAppBaseExt> GetListKanBanOnline(string workFlowID)
        {
            List<BeanAppBaseExt> lstRet = new List<BeanAppBaseExt>();
            try
            {
                var lstRes = new ProviderBase().getAllItemKanBan(workFlowID);
                if (lstRes != null && lstRes.Count > 0)
                {
                    lstRet = lstRes;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("KanBanView - GetListKanBanOnline - Err: " + ex.ToString());
            }
            return lstRet;
        }

        /// <summary>
        /// Lấy danh sách item từ local
        /// </summary>
        /// <param name="searchParam"></param>
        private void LoadContentList(string searchParam = "")
        {
            try
            {
                var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);

                string date_filter = string.Empty;

                if (fromDateSelected.Year.ToString() != "1" && toDateSelected.Year.ToString() != "1")
                {
                    date_filter = string.Format("Created IS NOT NULL AND (Created >= '{0}' AND Created <= '{1}')", fromDateSelected.ToString("yyyy-MM-dd"), toDateSelected.AddDays(1).ToString("yyyy-MM-dd"));
                }

                // tat ca
                if (currentMenu.ID == 0)
                {
                    string query_worflow = string.Format("SELECT * FROM BeanWorkflowItem WHERE {0} AND WorkflowID = ? AND ActionStatusId <> 2 ORDER BY Created DESC", date_filter);
                    lst_workflowItems = conn.Query<BeanAppBaseExt>(query_worflow, workflowSelected.WorkflowID);
                }
                // cho phe duyet
                else if (currentMenu.ID == 1)
                {
                    string query_worflow = string.Format("SELECT * FROM BeanWorkflowItem WHERE {0} AND WorkflowID = ? AND ActionStatusId IN (1,2,3,4,5) ORDER BY Created DESC", date_filter);
                    lst_workflowItems = conn.Query<BeanAppBaseExt>(query_worflow, workflowSelected.WorkflowID);
                }
                // da phe duyet
                else if (currentMenu.ID == 2)
                {
                    string query_worflow = string.Format("SELECT * FROM BeanWorkflowItem WHERE {0} AND WorkflowID = ? AND ActionStatusId = 10 ORDER BY Created DESC", date_filter);
                    lst_workflowItems = conn.Query<BeanAppBaseExt>(query_worflow, workflowSelected.WorkflowID);
                }
                // bi tu choi
                else if (currentMenu.ID == 3)
                {
                    //string query_worflow = string.Format("SELECT * FROM BeanWorkflowItem WHERE {0} AND WorkflowID = ? AND ActionStatusId = 6 ORDER BY Created DESC", date_filter);
                    //lst_workflowItems = conn.Query<BeanWorkflowItem>(query_worflow, workflowSelected.WorkflowID);
                }

                lst_search_workflowItems = lst_workflowItems != null && lst_workflowItems.Count > 0 ? lst_workflowItems : new List<BeanAppBaseExt>();

            }
            catch (Exception ex)
            {
                Console.WriteLine("KanBanView - LoadContentList - Err: " + ex.ToString());
            }
        }

        public void reloadData()
        {
            //if (currentIndex_menu == 0)
            //{
            //    if (index_ContentSearch == 1)
            //        LoadContent(tf_search_title.Text);
            //    else if (index_ContentSearch == 2)
            //        LoadContent(tf_search_nxl.Text);
            //}
            //else
            //{
            //    /*if (index_ContentSearch == 1)
            //           LoadContentList(tf_search_title.Text);
            //       else if (index_ContentSearch == 2)
            //           LoadContentList(tf_search_nxl.Text);*/
            //    if (index_ContentSearch == 1)
            //        LoadContent(tf_search_title.Text);
            //    else if (index_ContentSearch == 2)
            //        LoadContent(tf_search_nxl.Text);
            //}

            if (index_ContentSearch == 1)
                LoadContent(tf_search_title.Text, true);
            else if (index_ContentSearch == 2)
                LoadContent(tf_search_nxl.Text, true);
        }

        private void LoadDataFilter()
        {
            if (CmmVariable.SysConfig.LangCode == "1033")
            {
                lbl_todate.Text = toDateSelected.ToString("MM/dd/yy");
                lbl_fromdate.Text = fromDateSelected.ToString("MM/dd/yyyy");
            }
            else
            {
                lbl_todate.Text = toDateSelected.ToString("dd/MM/yyyy");
                lbl_fromdate.Text = fromDateSelected.ToString("dd/MM/yyyy");
            }

            UpdateLbl_dateText();

            /*if (currentIndex_menu == 0)
                LoadContent();
            else if (currentIndex_menu == 1)
                LoadContent();*///LoadContentList();
            LoadContent();
        }

        void UpdateLbl_dateText()
        {
            lbl_date.Text = lbl_fromdate.Text.Remove(lbl_fromdate.Text.Length - 2, 2) + " - " + lbl_todate.Text.Remove(lbl_todate.Text.Length - 2, 2);
        }

        private void SearchData(string _searchPara)
        {
            try
            {
                /*if (currentIndex_menu == 0)
                {
                    LoadContent(_searchPara);
                }
                else
                {
                    List<BeanAppBaseExt> _lst_searchResult = new List<BeanAppBaseExt>();
                    if (lst_search_workflowItems != null && lst_search_workflowItems.Count > 0)
                    {
                        // content
                        if (index_ContentSearch == 1)
                        {
                            _lst_searchResult = (from wfi in lst_search_workflowItems
                                                 where (!string.IsNullOrEmpty(wfi.Content) && CmmFunction.removeSignVietnamese(wfi.Content.ToLowerInvariant()).Contains(_searchPara))
                                                 select wfi).ToList();
                        }
                        // AssignedToName
                        else
                        {
                            //_lst_searchResult = (from wfi in lst_search_workflowItems
                            //                     where (!string.IsNullOrEmpty(wfi.AssignedToName) && CmmFunction.removeSignVietnamese(wfi.AssignedToName.ToLowerInvariant()).Contains(_searchPara))
                            //                     select wfi).ToList();
                        }

                        if (_lst_searchResult != null && _lst_searchResult.ToList().Count() > 0)
                        {
                            //table_listview.Source = new WorkFlow_TableSource(_lst_searchResult.ToList(), this);
                            //table_listview.ReloadData();
                        }
                        else
                        {
                            var noResult = new List<BeanAppBaseExt>();

                        }
                    }
                }
                */
                reloadData();
            }
            catch (Exception ex)
            {
                Console.WriteLine("KanBanView - SearchData - Err: " + ex.ToString());
            }
        }

        private void LoadStatusFilterCategory()
        {
            lst_menu_cate = new List<ClassMenu>();

            ClassMenu menu0 = new ClassMenu();
            menu0.title = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
            menu0.ID = 0;
            menu0.isSelected = true;

            ClassMenu menu1 = new ClassMenu();
            menu1.title = CmmFunction.GetTitle("TEXT_WAITING_APPROVE", "Đang thực hiện");
            menu1.ID = 1;

            ClassMenu menu2 = new ClassMenu();
            menu2.title = CmmFunction.GetTitle("TEXT_APPROVED", "Hoàn tất");
            menu2.ID = 2;

            ClassMenu menu3 = new ClassMenu();
            menu3.title = CmmFunction.GetTitle("TEXT_REJECT", "Từ chối");
            menu3.ID = 3;

            lst_menu_cate.Add(menu0);
            lst_menu_cate.Add(menu1);
            lst_menu_cate.Add(menu2);
            lst_menu_cate.Add(menu3);
        }

        private void ToggleMenuCategory()
        {
            Custom_MenuOption custom_MenuOption = Custom_MenuOption.Instance;
            if (custom_MenuOption.Superview != null)
                custom_MenuOption.RemoveFromSuperview();
            else
            {
                custom_MenuOption.ItemNoIcon = false;
                custom_MenuOption.viewController = this;
                custom_MenuOption.InitFrameView(new CGRect(view_category.Frame.X - 50, view_category.Frame.Bottom + 2, view_category.Frame.Width + 50, lst_menu_cate.Count * custom_MenuOption.RowHeigth));
                custom_MenuOption.AddShadowForView();
                custom_MenuOption.ListItemMenu = lst_menu_cate;
                custom_MenuOption.TableLoadData();

                view_content.AddSubview(custom_MenuOption);
                view_content.BringSubviewToFront(custom_MenuOption);
            }
        }

        private void ToggleMenuDate()
        {
            if (view_filterdate.Hidden)
            {
                view_filterdate.Hidden = false;
                custom_CalendarView.Hidden = false;
            }
            else
            {
                view_filterdate.Hidden = true;
                custom_CalendarView.Hidden = true;
            }
        }

        void UpdateFocusedButton()
        {
            var dates = toDateSelected - fromDateSelected;
            switch (dates.Days)
            {
                case 0:
                    DateRangeSwitch(0);
                    break;
                case 1:
                    DateRangeSwitch(1);
                    break;
                case 7:
                    DateRangeSwitch(2);
                    break;
                case 30:
                    DateRangeSwitch(3);
                    break;
                default:
                    DateRangeSwitch(-1);
                    break;
            }
        }

        private void SwitchBoardOrList()
        {
            if (currentIndex_menu != 0)
            {
                currentIndex_menu = 0;
                UIView.BeginAnimations("show_animationShowTable");
                UIView.SetAnimationDuration(0.2f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);

                UIView.CommitAnimations();
            }
            else if (currentIndex_menu != 1)
            {
                currentIndex_menu = 1;
                UIView.BeginAnimations("show_animationShowTable");
                UIView.SetAnimationDuration(0.2f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);

                UIView.CommitAnimations();
            }
        }

        private void DateRangeSwitch(int index)
        {
            switch (index)
            {
                case 0:
                    BT_date_0.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);
                    BT_date_0.Font = UIFont.FromName("Arial-BoldMT", 14f);
                    indexRangeSelected = dict_indexRangeDayChoice["today"];
                    BT_date_1.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_1.Font = UIFont.FromName("ArialMT", 14f);
                    BT_date_2.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_2.Font = UIFont.FromName("ArialMT", 14f);
                    BT_date_3.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_3.Font = UIFont.FromName("ArialMT", 14f);

                    toDateSelected = DateTime.Now.Date;
                    fromDateSelected = toDateSelected.AddDays(-indexRangeSelected);

                    LoadDataFilter();
                    break;
                case 1:
                    BT_date_0.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_0.Font = UIFont.FromName("ArialMT", 14f);
                    BT_date_1.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);
                    BT_date_1.Font = UIFont.FromName("Arial-BoldMT", 14f);
                    indexRangeSelected = dict_indexRangeDayChoice["yesterday"];
                    BT_date_2.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_2.Font = UIFont.FromName("ArialMT", 14f);
                    BT_date_3.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_3.Font = UIFont.FromName("ArialMT", 14f);

                    toDateSelected = DateTime.Now.Date;
                    fromDateSelected = toDateSelected.AddDays(-indexRangeSelected);

                    LoadDataFilter();
                    break;
                case 2:
                    BT_date_0.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_0.Font = UIFont.FromName("ArialMT", 14f);
                    BT_date_1.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_1.Font = UIFont.FromName("ArialMT", 14f);
                    BT_date_2.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);
                    BT_date_2.Font = UIFont.FromName("Arial-BoldMT", 14f);
                    indexRangeSelected = dict_indexRangeDayChoice["7days"];
                    BT_date_3.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_3.Font = UIFont.FromName("ArialMT", 14f);

                    toDateSelected = DateTime.Now.Date;
                    fromDateSelected = toDateSelected.AddDays(-indexRangeSelected);

                    LoadDataFilter();
                    break;
                case 3:
                    BT_date_0.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_0.Font = UIFont.FromName("ArialMT", 14f);
                    BT_date_1.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_1.Font = UIFont.FromName("ArialMT", 14f);
                    BT_date_2.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_2.Font = UIFont.FromName("ArialMT", 14f);
                    BT_date_3.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);
                    BT_date_3.Font = UIFont.FromName("Arial-BoldMT", 14f);
                    indexRangeSelected = dict_indexRangeDayChoice["30days"];

                    toDateSelected = DateTime.Now.Date;
                    fromDateSelected = toDateSelected.AddDays(-indexRangeSelected);

                    LoadDataFilter();
                    break;
                default:
                    BT_date_0.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_0.Font = UIFont.FromName("ArialMT", 14f);
                    BT_date_1.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_1.Font = UIFont.FromName("ArialMT", 14f);
                    BT_date_2.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_2.Font = UIFont.FromName("ArialMT", 14f);
                    BT_date_3.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_3.Font = UIFont.FromName("ArialMT", 14f);
                    indexRangeSelected = dict_indexRangeDayChoice["custom"];
                    break;
            }
        }

        private bool validateDateFilter()
        {
            bool res = false;
            if (fromDateSelected > toDateSelected)
                res = false;
            else
                res = true;

            return res;
        }

        public void HandleMenuOptionResult(ClassMenu _menu)
        {
            if (_menu != null)
            {
                _menu.isSelected = true;

                if (currentMenu != null && currentMenu.ID != _menu.ID)
                {
                    currentMenu.isSelected = false;
                    currentMenu = _menu;
                    lbl_category.Text = _menu.title;
                    LoadContent();
                    //LoadContentList();
                }
            }

            Custom_MenuOption custom_menuOption = Custom_MenuOption.Instance;
            if (custom_menuOption.Superview != null)
                custom_menuOption.RemoveFromSuperview();
        }

        public async void SubmitAction(BeanAppBaseExt _beanWorkflowItem, string action)
        {
            workflowItemSelected = _beanWorkflowItem;
            try
            {
                loading = new CmmLoading(new CGRect((this.View.Bounds.Width - 200) / 2, (this.View.Bounds.Height - 200) / 2, 200, 200), "Đang xử lý...");
                this.View.Add(loading);
                await Task.Run(() =>
                {
                    if (workflowItemSelected.ID > 0)
                    {
                        SQLiteAsyncConnection conn = new SQLiteAsyncConnection(CmmVariable.M_DataPath, false);
                        string query_workflowItem = string.Format("SELECT * FROM BeanWorkflowItem WHERE ID = {0}", workflowItemSelected.ID);
                        var _workflowItemSelected = conn.QueryAsync<BeanWorkflowItem>(query_workflowItem).Result;

                        if (_workflowItemSelected != null && _workflowItemSelected.Count > 0)
                        {
                            ProviderControlDynamic p_controlDynamic = new ProviderControlDynamic();
                            Json_FormDataString = p_controlDynamic.GetTicketRequestControlDynamicForm(_workflowItemSelected[0]);
                        }
                    }
                });

                if (!string.IsNullOrEmpty(Json_FormDataString))
                {
                    JObject retValue = JObject.Parse(Json_FormDataString);
                    JArray json_dataForm = JArray.Parse(retValue["form"].ToString());
                    str_json_FormDefineInfo = json_dataForm[0]["FormDefineInfo"].ToString();

                    JObject jsonButtonBot = JObject.Parse(retValue["action"].ToString());
                    var buttonBot = jsonButtonBot.ToObject<ViewRow>();
                    List<string> lst_action = new List<string>();
                    if (action.Contains('|'))
                        lst_action = action.Split('|').ToList();
                    else
                        lst_action.Add(action);

                    if (buttonBot != null && buttonBot.Elements.Count > 0)
                    {
                        ButtonAction _buttonAction = null;
                        foreach (var item in buttonBot.Elements)
                        {
                            if (lst_action.Contains(item.ID))
                            {
                                _buttonAction = new ButtonAction();
                                _buttonAction.ID = Convert.ToInt16(item.ID);
                                _buttonAction.Value = item.Value;
                                _buttonAction.Title = item.Title;
                            }
                        }

                        if (_buttonAction != null)
                        {
                            InvokeOnMainThread(() =>
                            {
                                if (_buttonAction.ID == 1 || _buttonAction.ID == 2 || _buttonAction.ID == 12)
                                {
                                    ActionApprove(_buttonAction);
                                }
                                else if (_buttonAction.ID == 4)
                                {
                                    CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
                                    CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
                                    PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);

                                    FormApproveOrRejectView formApproveOrReject = (FormApproveOrRejectView)Storyboard.InstantiateViewController("FormApproveOrRejectView");
                                    formApproveOrReject.SetContent(this, _buttonAction);
                                    transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                                    formApproveOrReject.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                                    formApproveOrReject.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                                    formApproveOrReject.TransitioningDelegate = transitioningDelegate;
                                    this.PresentViewControllerAsync(formApproveOrReject, true);
                                }
                            });
                        }
                        else
                        {
                            InvokeOnMainThread(() =>
                            {
                                loading.Hide();
                                CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("TEXT_ACTION_NOT_PERMIT", "Bạn không có quyền thực hiện hành động này."));
                            });
                        }
                    }
                    else
                    {
                        InvokeOnMainThread(() =>
                        {
                            loading.Hide();
                            CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("TEXT_ACTION_NOT_PERMIT", "Bạn không có quyền thực hiện hành động này."));
                        });
                    }
                }
                else
                {
                    loading.Hide();
                    UIAlertController alert = UIAlertController.Create("Thông báo", "Không tìm thấy thông tin phiếu, vui lòng thử lại sau.", UIAlertControllerStyle.Alert);//"BPM"
                    alert.AddAction(UIAlertAction.Create("Đóng", UIAlertActionStyle.Default, alertAction =>
                    {

                    }));
                    this.PresentViewController(alert, true, null);
                }
            }
            catch (Exception ex)
            {
                loading.Hide();
                Console.WriteLine("TodoDetailView - GetIndexItemFromDictionnary - ERR: " + ex.ToString());
            }
        }

        public async void ActionApprove(ButtonAction _buttonAction)
        {
            await Task.Run(() =>
            {
                bool result = false;
                ProviderBase b_pase = new ProviderBase();
                ProviderControlDynamic providerControl = new ProviderControlDynamic();
                List<KeyValuePair<string, string>> lstExtent = new List<KeyValuePair<string, string>>();

                //KeyValuePair<string, string> note = new KeyValuePair<string, string>();
                //lstExtent.Add(note);

                string str_errMess = "";
                if (lstExtent != null && lstExtent.Count > 0)
                    result = providerControl.SendControlDynamicAction(_buttonAction.Value, workflowItemSelected.ID.ToString(), str_json_FormDefineInfo, "", ref str_errMess, null, lstExtent);
                else
                    result = providerControl.SendControlDynamicAction(_buttonAction.Value, workflowItemSelected.ID.ToString(), str_json_FormDefineInfo, "", ref str_errMess, null);

                if (result)
                {
                    b_pase.UpdateAllDynamicData(true);
                    InvokeOnMainThread(() =>
                    {
                        loading.Hide();
                        reloadData();
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

        public async void ActionReject(ButtonAction _buttonAction, List<KeyValuePair<string, string>> _lstExtent)
        {
            try
            {
                await Task.Run(() =>
                {
                    bool result = false;
                    ProviderBase b_pase = new ProviderBase();
                    ProviderControlDynamic providerControl = new ProviderControlDynamic();
                    List<KeyValuePair<string, string>> lstExtent = new List<KeyValuePair<string, string>>();

                    lstExtent = _lstExtent;
                    string str_errMess = "";
                    if (lstExtent != null && lstExtent.Count > 0)
                        result = providerControl.SendControlDynamicAction(_buttonAction.Value, workflowItemSelected.ID.ToString(), str_json_FormDefineInfo, "", ref str_errMess, null, lstExtent);
                    else
                        result = providerControl.SendControlDynamicAction(_buttonAction.Value, workflowItemSelected.ID.ToString(), str_json_FormDefineInfo, "", ref str_errMess, null);

                    if (result)
                    {
                        b_pase.UpdateAllDynamicData(true);
                        InvokeOnMainThread(() =>
                        {
                            reloadData();
                            loading.Hide();
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
                Console.WriteLine("kanBanView - ActionReject - ERR: " + ex.ToString());
            }

        }

        public void CloseCustomCalendar()
        {
            Custom_CalendarView custom_CalendarView = Custom_CalendarView.Instance;
            if (custom_CalendarView.Superview != null)
            {
                custom_CalendarView.RemoveFromSuperview();
                custom_CalendarView.reset();
            }
        }

        public void NavigateToWorkFlowDetails(BeanAppBaseExt beanWorkflowItem)
        {
            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            FormWorkFlowDetails detail = (FormWorkFlowDetails)Storyboard.InstantiateViewController("FormWorkFlowDetails");

            SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
            string query_follow = string.Format("SELECT * FROM BeanWorkflowItem WHERE ID = ?");
            var lst_item = conn.Query<BeanWorkflowItem>(query_follow, beanWorkflowItem.ID);

            if (lst_item != null && lst_item.Count > 0)
            {
                lst_item[0].WorkflowTitle = workflowSelected.Title;
                lst_item[0].WorkflowTitleEN = workflowSelected.TitleEN;
                detail.SetContent(lst_item[0], false, this);
            }

            PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            detail.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            detail.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            detail.TransitioningDelegate = transitioningDelegate;
            this.PresentViewControllerAsync(detail, true);
        }
        #endregion

        #region events

        private void BT_back_TouchUpInside(object sender, EventArgs e)
        {
            this.NavigationController.PopViewController(true);
        }
        private void BT_category_TouchUpInside(object sender, EventArgs e)
        {
            ToggleMenuCategory();
        }
        private void BT_filterDate_TouchUpInside(object sender, EventArgs e)
        {
            ToggleMenuDate();
        }
        private void BT_date_0_TouchUpInside(object sender, EventArgs e)
        {
            DateRangeSwitch(0);
        }
        private void BT_date_1_TouchUpInside(object sender, EventArgs e)
        {
            DateRangeSwitch(1);
        }
        private void BT_date_2_TouchUpInside(object sender, EventArgs e)
        {
            DateRangeSwitch(2);
        }
        private void BT_date_3_TouchUpInside(object sender, EventArgs e)
        {
            DateRangeSwitch(3);
        }
        private void BT_filter_fromdate_TouchUpInside(object sender, EventArgs e)
        {
            if (!isShowCalendar)
            {
                view_fromdate.Layer.BorderColor = UIColor.FromRGB(65, 80, 134).CGColor;
                view_todate.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;

                isShowCalendar = true;
                custom_CalendarView.inputView = lbl_fromdate;
                custom_CalendarView.SetUpDate();
                custom_CalendarView.InitFrameView(new CGRect(view_filterdate.Frame.X, view_filterdate.Frame.Bottom, view_filterdate.Frame.Width, 260));
                view_filterdate_constant.Constant = 430;
                BT_filterDateChoice.Hidden = false;
                custom_CalendarView.Hidden = false;
                view_filterdate.Layer.ShadowPath = UIBezierPath.FromRect(new CGRect(2, 2, view_filterdate.Frame.Width, 430)).CGPath;
            }
            else
            {
                view_fromdate.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;
                view_todate.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;

                isShowCalendar = false;
                view_filterdate_constant.Constant = 120;
                view_filterdate.Layer.ShadowPath = UIBezierPath.FromRect(new CGRect(2, 2, view_filterdate.Frame.Width, 120)).CGPath;
                BT_filterDateChoice.Hidden = true;
                custom_CalendarView.Hidden = true;
            }

        }
        private void BT_filter_todate_TouchUpInside(object sender, EventArgs e)
        {
            if (!isShowCalendar)
            {
                view_fromdate.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;
                view_todate.Layer.BorderColor = UIColor.FromRGB(65, 80, 134).CGColor;

                isShowCalendar = true;
                custom_CalendarView.inputView = lbl_todate;
                custom_CalendarView.SetUpDate();
                custom_CalendarView.InitFrameView(new CGRect(view_filterdate.Frame.X, view_filterdate.Frame.Bottom, view_filterdate.Frame.Width, 260));
                view_filterdate_constant.Constant = 430;
                BT_filterDateChoice.Hidden = false;
                custom_CalendarView.Hidden = false;
                view_filterdate.Layer.ShadowPath = UIBezierPath.FromRect(new CGRect(2, 2, view_filterdate.Frame.Width, 430)).CGPath;
            }
            else
            {
                view_fromdate.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;
                view_todate.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;

                isShowCalendar = false;
                view_filterdate_constant.Constant = 120;
                view_filterdate.Layer.ShadowPath = UIBezierPath.FromRect(new CGRect(2, 2, view_filterdate.Frame.Width, 120)).CGPath;
                BT_filterDateChoice.Hidden = true;
                custom_CalendarView.Hidden = true;
            }
        }

        private void BT_board_TouchUpInside(object sender, EventArgs e)
        {
            SwitchBoardOrList();
        }

        private void BT_list_TouchUpInside(object sender, EventArgs e)
        {
            SwitchBoardOrList();
        }

        private async void Tf_search_title_EditingChanged(object sender, EventArgs e)
        {
            index_ContentSearch = 1;
            await Task.Run(() =>
            {
                InvokeOnMainThread(() =>
                {
                    SearchData(tf_search_title.Text);
                });
            });
        }
        private async void Tf_search_nxl_EditingChanged(object sender, EventArgs e)
        {
            index_ContentSearch = 2; ;
            await Task.Run(() =>
            {
                InvokeOnMainThread(() =>
                {
                    SearchData(tf_search_nxl.Text);
                });
            });
        }

        private void BT_filterDateChoice_TouchUpInside(object sender, EventArgs e)
        {
            if (CmmVariable.SysConfig.LangCode == "1033")
            {
                fromDateSelected = DateTime.Parse(lbl_fromdate.Text, new CultureInfo("en", false));
                toDateSelected = DateTime.Parse(lbl_todate.Text, new CultureInfo("en", false));
            }
            else //if (CmmVariable.SysConfig.LangCode == "1066")
            {
                fromDateSelected = DateTime.Parse(lbl_fromdate.Text, new CultureInfo("vi", false));
                toDateSelected = DateTime.Parse(lbl_todate.Text, new CultureInfo("vi", false));
            }

            if (validateDateFilter())
            {
                UpdateLbl_dateText();
                LoadContent();
                //LoadContentList();
                ToggleMenuDate();
                UpdateFocusedButton();
            }
            else
            {
                CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("TEXT_DATE_INVALID", "Ngày bắt đầu không được lớn hơn ngày kết thúc"));
            }
        }
        #endregion

        #region custom class
        #region collection ticket categories
        public class CollectionWorkFlowStep_Source : UICollectionViewSource
        {
            KanBanView parentView { get; set; }
            public Dictionary<string, List<BeanAppBaseExt>> dict_groupWorkFlow;
            public List<string> sectionKeys;
            //bool => Colapse
            List<KeyValuePair<string, bool>> lst_sectionState;
            public List<BeanWorkflow> items;

            public CollectionWorkFlowStep_Source(KanBanView _parentview, Dictionary<string, List<BeanAppBaseExt>> _dict_StepWorkFlow, List<KeyValuePair<string, bool>> _sectionState)
            {
                parentView = _parentview;
                lst_sectionState = _sectionState;
                dict_groupWorkFlow = _dict_StepWorkFlow;
                LoadData();
            }

            public void LoadData()
            {
                if (lst_sectionState == null)
                {
                    //lst_sectionState = new List<KeyValuePair<string, bool>>();   
                    sectionKeys = new List<string>();

                    foreach (var item in dict_groupWorkFlow)
                    {
                        sectionKeys.Add(item.Key);
                        //KeyValuePair<string, bool> keypair_section = new KeyValuePair<string, bool>(item.Key, false);
                        //lst_sectionState.Add(keypair_section);
                    }

                    //parentView.lst_sectionState = lst_sectionState;
                }
                else
                {
                    //lst_sectionState = new List<KeyValuePair<string, bool>>();
                    sectionKeys = new List<string>();
                    //sectionKeys = items.Select(x => x.WorkflowCategoryID.Value).Distinct().ToList();

                    foreach (var item in dict_groupWorkFlow)
                    {
                        sectionKeys.Add(item.Key);
                        //KeyValuePair<string, bool> keypair_section = new KeyValuePair<string, bool>(item.Key, false);
                        //lst_sectionState.Add(keypair_section);
                    }
                }
            }

            public override nint NumberOfSections(UICollectionView collectionView)
            {
                return 1;
            }
            public override nint GetItemsCount(UICollectionView collectionView, nint section)
            {
                return dict_groupWorkFlow.Count;
            }
            public override bool ShouldHighlightItem(UICollectionView collectionView, NSIndexPath indexPath)
            {
                return true;
            }
            public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
            {
                //parentView.NavigateToViewByCate(items[indexPath.Row], indexPath.Row);
            }
            public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
            {
                bool isOdd = true;
                if (indexPath.Row % 2 == 0)
                    isOdd = false;

                KeyValuePair<string, List<BeanAppBaseExt>> stepItem = dict_groupWorkFlow.Where(x => x.Key == sectionKeys[indexPath.Row]).First();
                var cell = (WorkFlowItem_CollectionCell)collectionView.DequeueReusableCell(WorkFlowItem_CollectionCell.CellID, indexPath);
                cell.RemoveFromSuperview();
                cell.UpdateRow(stepItem, parentView, isOdd, indexPath);

                return cell;
            }
        }

        private class CustomFlowLayoutDelegate : UICollectionViewDelegateFlowLayout
        {
            static KanBanView parentView;
            CollectionWorkFlowStep_Source collect_source;
            UICollectionView CollectionView;

            #region Constructors
            public CustomFlowLayoutDelegate(KanBanView _parent, CollectionWorkFlowStep_Source _collect_source, UICollectionView collectionView)
            {
                collect_source = _collect_source;
                CollectionView = collectionView;
                parentView = _parent;
            }
            #endregion

            #region Override Methods
            public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
            {
                KeyValuePair<string, List<BeanAppBaseExt>> stepItem = collect_source.dict_groupWorkFlow.Where(x => x.Key == collect_source.sectionKeys[indexPath.Row]).First();
                //var height = stepItem.Value.Count * 167;
                return new CGSize(300, 510);
            }

            public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
            {
                //var itemSelected = collect_source.lst_workFlow[indexPath.Row];
                //parentView.HandleSeclectItem(itemSelected);
            }
            #endregion
        }
        #endregion

        #region workflow data source table
        private class WorkFlow_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellWorkFlow");
            KanBanView parentView;
            List<BeanAppBaseExt> lst_beanWorkflowItems;

            public WorkFlow_TableSource(List<BeanAppBaseExt> _lst_beanWorkflowItems, KanBanView _parentview)
            {
                lst_beanWorkflowItems = _lst_beanWorkflowItems;
                parentView = _parentview;
                LoadData();
            }

            private void LoadData()
            {

            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return 1;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return lst_beanWorkflowItems.Count;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 60;
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                return 1;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                var workFlow = lst_beanWorkflowItems[indexPath.Row];
                parentView.NavigateToWorkFlowDetails(workFlow);
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {

                var workFlow = lst_beanWorkflowItems[indexPath.Row];

                bool isOdd = true;
                if (indexPath.Row % 2 == 0)
                    isOdd = false;

                Custom_WorkFlowCell cell = new Custom_WorkFlowCell(cellIdentifier);
                cell.UpdateCell(workFlow, isOdd);
                return cell;
            }
        }

        public class Custom_WorkFlowCell : UITableViewCell
        {
            string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            BeanAppBaseExt workflowItem { get; set; }
            UILabel lbl_content, lbl_nguoitao, lbl_nguoitao_chucvu, lbl_nguoiXL, lbl_nguoiXL_chucvu, lbl_status, lbl_duedate, lbl_line;
            private UIImageView img_attach, iv_nguoitao, img_follow, iv_nguoiXL;
            private bool isOdd;

            public Custom_WorkFlowCell(NSString cellID) : base(UITableViewCellStyle.Default, cellID)
            {
                Accessory = UITableViewCellAccessory.None;
            }

            public void UpdateCell(BeanAppBaseExt _workflowItem, bool _isOdd)
            {
                isOdd = _isOdd;
                workflowItem = _workflowItem;
                viewConfiguration();
                loadData();
            }

            private void viewConfiguration()
            {
                if (isOdd)
                    ContentView.BackgroundColor = UIColor.White;
                else
                    ContentView.BackgroundColor = UIColor.FromRGB(250, 250, 250);

                img_follow = new UIImageView();
                img_follow.ContentMode = UIViewContentMode.ScaleAspectFill;

                lbl_content = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 14f),
                    TextColor = UIColor.FromRGB(25, 25, 30),
                    LineBreakMode = UILineBreakMode.TailTruncation,
                    Lines = 2,
                    TextAlignment = UITextAlignment.Left,
                };

                iv_nguoitao = new UIImageView();
                iv_nguoitao.ContentMode = UIViewContentMode.ScaleAspectFill;
                iv_nguoitao.ClipsToBounds = true;
                iv_nguoitao.Layer.CornerRadius = 20;

                lbl_nguoitao = new UILabel
                {
                    TextAlignment = UITextAlignment.Left,
                    Font = UIFont.FromName("ArialMT", 14f),
                    TextColor = UIColor.FromRGB(25, 25, 30),
                };

                lbl_nguoitao_chucvu = new UILabel
                {
                    TextAlignment = UITextAlignment.Left,
                    Font = UIFont.FromName("ArialMT", 11f),
                    TextColor = UIColor.FromRGB(94, 94, 94)
                };

                iv_nguoiXL = new UIImageView();
                iv_nguoiXL.ContentMode = UIViewContentMode.ScaleAspectFill;
                iv_nguoiXL.ClipsToBounds = true;
                iv_nguoiXL.Layer.CornerRadius = 20;

                lbl_nguoiXL = new UILabel
                {
                    TextAlignment = UITextAlignment.Left,
                    Font = UIFont.FromName("ArialMT", 14f),
                    TextColor = UIColor.FromRGB(25, 25, 30),
                };

                lbl_nguoiXL_chucvu = new UILabel
                {
                    TextAlignment = UITextAlignment.Left,
                    Font = UIFont.FromName("ArialMT", 11f),
                    TextColor = UIColor.FromRGB(94, 94, 94)
                };

                lbl_status = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 13f),
                    TextColor = UIColor.Black,
                    TextAlignment = UITextAlignment.Center,
                };

                lbl_status.ClipsToBounds = true;
                lbl_status.Layer.BorderColor = UIColor.White.CGColor;
                lbl_status.Layer.BorderWidth = 0.5f;
                lbl_status.Layer.CornerRadius = 5;

                img_attach = new UIImageView();
                img_attach.ContentMode = UIViewContentMode.ScaleAspectFit;
                img_attach.Image = UIImage.FromFile("Icons/icon_attach3x.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                img_attach.TintColor = UIColor.FromRGB(94, 94, 94);
                img_attach.Hidden = true;

                lbl_duedate = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 12f),
                    TextColor = UIColor.FromRGB(94, 94, 94),
                    TextAlignment = UITextAlignment.Right,
                };

                lbl_line = new UILabel();
                lbl_line.BackgroundColor = UIColor.FromRGB(51, 95, 179);

                ContentView.AddSubviews(new UIView[] { img_follow, lbl_content, iv_nguoitao, lbl_nguoitao, lbl_nguoitao_chucvu, iv_nguoiXL, lbl_nguoiXL, lbl_nguoiXL_chucvu, img_attach, lbl_duedate, lbl_status, lbl_line });
            }

            private void loadData()
            {
                try
                {
                    //SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath, false);

                    //List<BeanWorkflowFollow> lst_follow = new List<BeanWorkflowFollow>();
                    //string query_follow = string.Format("SELECT * FROM BeanWorkflowFollow WHERE WorkflowItemId = ?");
                    //var lst_followResult = conn.Query<BeanWorkflowFollow>(query_follow, workflowItem.ID);

                    //if (lst_followResult != null && lst_followResult.Count > 0)
                    //{
                    //    if (lst_followResult[0].Status == 1)
                    //        img_follow.Image = UIImage.FromFile("Icons/icon_Star_on.png");
                    //    else
                    //        img_follow.Image = UIImage.FromFile("Icons/icon_Star_off.png");
                    //}
                    //else
                    //    img_follow.Image = UIImage.FromFile("Icons/icon_Star_off.png");

                    //lbl_content.Text = workflowItem.Content;

                    ////Chi tiet nguoi tao
                    //List<BeanUser> lst_nguoitao = new List<BeanUser>();
                    //BeanUser nguoitao = new BeanUser();
                    //if (!string.IsNullOrEmpty(workflowItem.CreatedBy))
                    //{
                    //    string query_user = string.Format("SELECT * FROM BeanUser WHERE ID = ?");
                    //    lst_nguoitao = conn.Query<BeanUser>(query_user, workflowItem.CreatedBy.ToLower());

                    //    if (lst_nguoitao != null && lst_nguoitao.Count > 0)
                    //        nguoitao = lst_nguoitao[0];

                    //    checkFileLocalIsExist(nguoitao, iv_nguoitao);
                    //    lbl_nguoitao.Text = nguoitao.FullName;
                    //    lbl_nguoitao_chucvu.Text = nguoitao.Position;
                    //}

                    ////Chi tiet nguoi XL
                    //string assignedTo = "";
                    //string[] arr_assignedTo;
                    //BeanUser first_nguoiXL = new BeanUser();
                    //List<string> lst_userName = new List<string>();
                    //nfloat temp_width = 0;
                    //if (!string.IsNullOrEmpty(workflowItem.AssignedTo))
                    //{
                    //    if (workflowItem.AssignedTo.Contains(','))
                    //    {
                    //        arr_assignedTo = workflowItem.AssignedTo.Split(',');
                    //        //string res = string.Empty;

                    //        for (int i = 0; i < arr_assignedTo.Length; i++)
                    //        {
                    //            List<BeanUser> lst_userResult = new List<BeanUser>();
                    //            string query_user = string.Format("SELECT * FROM BeanUser WHERE ID = ?");
                    //            lst_userResult = conn.Query<BeanUser>(query_user, arr_assignedTo[i].Trim().ToLower());

                    //            if (lst_userResult != null && lst_userResult.Count > 0)
                    //            {
                    //                if (i == 0)
                    //                    first_nguoiXL = lst_userResult[i];

                    //                lst_userName.Add(lst_userResult[0].FullName);
                    //            }
                    //        }

                    //        if (lst_userName.Count > 1)
                    //        {
                    //            int num_remain = lst_userName.Count - 1;
                    //            assignedTo = lst_userName[0] + ", +" + num_remain.ToString();
                    //        }
                    //        else
                    //            assignedTo = lst_userName[0];
                    //    }
                    //    else
                    //    {
                    //        List<BeanUser> lst_userResult = new List<BeanUser>();
                    //        string query_user = string.Format("SELECT * FROM BeanUser WHERE ID = ?");
                    //        lst_userResult = conn.Query<BeanUser>(query_user, workflowItem.AssignedTo.ToLower());
                    //        if (lst_userResult != null && lst_userResult.Count > 0)
                    //        {
                    //            first_nguoiXL = lst_userResult[0];
                    //            assignedTo = lst_userResult[0].FullName;

                    //        }
                    //    }

                    //    checkFileLocalIsExist(first_nguoiXL, iv_nguoiXL);
                    //    lbl_nguoiXL.Text = assignedTo;
                    //    lbl_nguoiXL_chucvu.Text = first_nguoiXL.Position;
                    //}

                    //if (workflowItem.HasFile.HasValue && workflowItem.HasFile.Value)
                    //    img_attach.Hidden = false;

                    //if (!string.IsNullOrEmpty(workflowItem.Status)) //= status
                    //{
                    //    lbl_status.Hidden = false;
                    //    lbl_status.Frame = new CGRect(90, 60, 50, 20);

                    //    if (CmmVariable.SysConfig.LangCode == "1066")
                    //        lbl_status.Text = workflowItem.ActionStatus;
                    //    else if (CmmVariable.SysConfig.LangCode == "1033")
                    //        lbl_status.Text = workflowItem.ActionStatusEN;

                    //    lbl_status.Lines = 1;
                    //    lbl_status.SizeToFit();

                    //    if (workflowItem.ActionStatusID == 10) // Đã phê duyệt - xanh la - 220,255,218
                    //    {
                    //        lbl_status.BackgroundColor = UIColor.FromRGB(229, 255, 228);
                    //        lbl_duedate.Hidden = true;
                    //    }
                    //    else if (workflowItem.ActionStatusID == -1 || workflowItem.ActionStatusID == 6) // huy, tu choi
                    //    {
                    //        lbl_status.BackgroundColor = UIColor.FromRGB(255, 203, 203); // đỏ
                    //        lbl_duedate.Hidden = true;
                    //    }
                    //    else if (workflowItem.ActionStatusID == 0) // Đang lưu
                    //        lbl_status.BackgroundColor = UIColor.FromRGB(243, 243, 243);// xam
                    //    else if (workflowItem.ActionStatusID == 1)
                    //        lbl_status.BackgroundColor = UIColor.FromRGB(209, 233, 255); // xanh da troi
                    //    else
                    //        lbl_status.BackgroundColor = UIColor.FromRGB(209, 233, 255); // xanh da troi

                    //}
                    //else
                    //{
                    //    lbl_status.Hidden = true;
                    //}

                    //if (workflowItem.DueDate.HasValue)
                    //{
                    //    if (CmmVariable.SysConfig.LangCode == "1033")
                    //        lbl_duedate.Text = CmmFunction.GetStringDateTimeLang(workflowItem.DueDate.Value, 1, 1033);
                    //    else if (CmmVariable.SysConfig.LangCode == "1066")
                    //        lbl_duedate.Text = CmmFunction.GetStringDateTimeLang(workflowItem.DueDate.Value, 1, 1066);

                    //    if (workflowItem.DueDate.Value.Date < DateTime.Now.Date) // qua han => mau do
                    //        lbl_duedate.TextColor = UIColor.Red;
                    //    else if (workflowItem.DueDate.Value.Date == DateTime.Now.Date) // trong ngay => mau tim
                    //        lbl_duedate.TextColor = UIColor.FromRGB(139, 79, 183);
                    //    else if (workflowItem.DueDate.Value.Date > DateTime.Now.Date && workflowItem.DueDate.Value.Date < DateTime.Now.Date.AddDays(5))
                    //    {
                    //        //lbl_duedate.TextColor = UIColor.FromRGB(119, 224, 117);
                    //        // chuyen mau xanh la => black
                    //        lbl_duedate.TextColor = UIColor.Black;
                    //    }
                    //    else //if (noti.Status == 1)
                    //    {
                    //        lbl_duedate.TextColor = UIColor.Black;
                    //    }
                    //}

                }
                catch (Exception ex)
                {
                    Console.WriteLine("MainView - Custom_WorkFlowItemCell - loaddata- ERR: " + ex.ToString());
                }
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                img_follow.Frame = new CGRect(15, 24, 16, 16);
                lbl_content.Frame = new CGRect(img_follow.Frame.Right + 15, 10, 220, 50);

                iv_nguoitao.Frame = new CGRect(lbl_content.Frame.Right + 10, 12, 35, 35);
                lbl_nguoitao.Frame = new CGRect(iv_nguoitao.Frame.Right + 10, 10, 150, 20);
                lbl_nguoitao_chucvu.Frame = new CGRect(iv_nguoitao.Frame.Right + 10, lbl_nguoitao.Frame.Bottom, 150, 20);

                iv_nguoiXL.Frame = new CGRect(lbl_nguoitao.Frame.Right + 10, 10, 35, 35);
                lbl_nguoiXL.Frame = new CGRect(iv_nguoiXL.Frame.Right + 10, 10, 150, 20);
                lbl_nguoiXL_chucvu.Frame = new CGRect(iv_nguoiXL.Frame.Right + 10, lbl_nguoiXL.Frame.Bottom, 150, 20);

                lbl_status.Frame = new CGRect(lbl_nguoiXL.Frame.Right + 10, 18, 120, 25);
                lbl_duedate.Frame = new CGRect(ContentView.Frame.Width - 130, lbl_status.Frame.Y, 120, 22);
            }

            private async void checkFileLocalIsExist(BeanUser contact, UIImageView image_view)
            {
                try
                {
                    string filename = contact.ImagePath.Split('/').Last();
                    string filepathURL = CmmVariable.M_Domain + "/" + CmmVariable.SysConfig.Subsite + contact.ImagePath;
                    string localfilePath = Path.Combine(localDocumentFilepath, filename);

                    if (!File.Exists(localfilePath))
                    {
                        UIImage avatar = null;
                        await Task.Run(() =>
                        {
                            ProviderBase provider = new ProviderBase();
                            if (provider.DownloadFile(filepathURL, localfilePath, CmmVariable.M_AuthenticatedHttpClient))
                            {
                                NSData data = NSData.FromUrl(new NSUrl(localfilePath, false));

                                InvokeOnMainThread(() =>
                                {
                                    if (data != null)
                                    {
                                        UIImage image = UIImage.LoadFromData(data);
                                        if (image != null)
                                        {
                                            avatar = CmmIOSFunction.MaxResizeImage(image, 200, 200);
                                            image_view.Image = avatar;
                                        }
                                        else
                                            image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                                    }
                                    else
                                        image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");

                                });

                                if (data != null && avatar != null)
                                {
                                    NSError err = null;
                                    NSData imgData = avatar.AsPNG();
                                    if (imgData.Save(localfilePath, false, out err))
                                        Console.WriteLine("saved as " + localfilePath);
                                    return;
                                }
                            }
                            else
                            {
                                InvokeOnMainThread(() =>
                                {

                                });
                            }
                        });
                    }
                    else
                    {
                        openFile(filename, image_view);

                    }
                }
                catch (Exception ex)
                {
                    image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                    Console.WriteLine("ListUserView - checkFileLocalIsExist - Err: " + ex.ToString());
                    //CmmIOSFunction.IOSlog(null, "PopupContactDetailView - loadAvatar - " + ex.ToString());
                }
            }

            private async void openFile(string localfilename, UIImageView image_view)
            {
                try
                {
                    NSData data = null;
                    await Task.Run(() =>
                    {
                        string localfilePath = Path.Combine(localDocumentFilepath, localfilename);
                        data = NSData.FromUrl(new NSUrl(localfilePath, false));
                    });

                    if (data != null)
                    {
                        UIImage image = UIImage.LoadFromData(data);
                        if (image != null)
                        {
                            image_view.Image = image;
                        }
                        else
                        {
                            image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                        }
                    }
                    else
                    {
                        image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("ContactView - openFile - Err: " + ex.ToString());
                }
            }
        }
        #endregion
        #endregion

    }
}
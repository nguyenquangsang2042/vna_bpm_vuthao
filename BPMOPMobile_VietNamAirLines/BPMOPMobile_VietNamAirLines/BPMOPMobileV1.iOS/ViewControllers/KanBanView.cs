
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
using BPMOPMobileV1.iOS.CustomControlClass;
using BPMOPMobileV1.iOS.IOSClass;
using BPMOPMobileV1.iOS.KanBanCustomClass;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite;
using UIKit;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class KanBanView : UIViewController
    {
        public bool isFisrt = true;
        BeanWorkflow workflowSelected { get; set; }
        public BeanAppBaseExt workflowItemSelected { get; set; }
        public KanBanModel kanBanModel { get; set; }

        List<BeanWorkflowStepDefine> beanWorkflowStepDefines;
        Dictionary<string, List<BeanAppBaseExt>> dict_workFlowByStep = new Dictionary<string, List<BeanAppBaseExt>>();
        List<KeyValuePair<string, bool>> lst_sectionState;
        CollectionWorkFlowStep_Source collectionWorkFlowStep_Source;
        List<ClassMenu> lst_menu_cate;
        List<UIButton> lst_BTstatus = new List<UIButton> { };
        List<BeanAppBaseExt> lst_workflowItems;
        List<BeanAppBaseExt> lst_search_workflowItems;
        CmmLoading loading;
        string Json_FormDataString = string.Empty;
        string str_json_FormDefineInfo = string.Empty;
        ClassMenu currentMenu { get; set; }
        Custom_CalendarView custom_CalendarView;
        //int index_ContentSearch = 1; //1: search content | 2: search user
        Dictionary<string, int> dict_indexRangeDayChoice;
        int indexRangeSelected;
        int rangeDateSelect = 3, RangeTemp;
        UIColor previousColorFilter = UIColor.FromRGB(0, 0, 0);// lay gia tri icon ban dau mac dinh

        DateTime fromDate_default;
        DateTime toDate_default;
        DateTime fromDateSelected;
        DateTime toDateSelected;
        private bool isSearch = false;
        bool isFilter = false;

        public readonly int[] WaitingListID = { 1, 4 };                // AppStatusID Chờ phê duyệt (đang lưu - chờ xử lý - bổ sung thông tin - tham vấn - yêu cầu hiệu chỉnh)
        public readonly int[] ApprovedListID = { 8 };                  // AppStatusID Phê duyệt
        public readonly int[] RejectedListID = { 16, 64 };             // AppStatusID Từ chối (từ chối - hủy)
        private string hintTextSearchTitle = CmmFunction.GetTitle("TEXT_SEARCH", "Tìm kiếm theo tên quy trình");
        //ButtonsActionBroadBotBarApplication buttonActionBotBarApplication;
        ButtonsActionBotBar buttonActionBotBar;
        nfloat heightFilterDate;
        //public nfloat heightCollection;
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
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (!isFisrt && !isFilter)
            {
                LoadContent("", false);
            }
            ////heightCollection = bottom_view.Frame.Top - view_choose_kanban.Frame.Bottom - 10;
            ////KanbanCollection.Frame = new CGRect(KanbanCollection.Frame.X,
            ////    view_choose_kanban.Frame.Bottom, view_choose_kanban.Frame.Width, heightCollection);
            //if (buttonActionBotBarApplication != null)
            //{
            //    bottom_view.AddSubviews(buttonActionBotBarApplication);
            //}
            if (buttonActionBotBar != null)
            {
                bottom_view.AddSubviews(buttonActionBotBar);
            }
        }
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            isFisrt = false;
        }
        //public override void ViewWillLayoutSubviews()
        //{
        //    base.ViewWillLayoutSubviews();

        //}
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ReloadFailedBeanData();

            ToggleCalendar(false);
            ViewConfiguration();
            SetLangTitle();
            LoadStatusFilterCategory();
            SetDate();
            currentMenu = lst_menu_cate[0];
            LoadContent("", true);
            ChangeColorButtonStatus(0);

            //SwitchBoardOrList();

            #region delelegate
            BT_back.TouchUpInside += BT_back_TouchUpInside;
            //BT_category.TouchUpInside += BT_category_TouchUpInside;
            BT_filterDate.TouchUpInside += BT_filterDate_TouchUpInside;
            BT_date_0.TouchUpInside += BT_date_0_TouchUpInside;
            BT_date_1.TouchUpInside += BT_date_1_TouchUpInside;
            BT_date_2.TouchUpInside += BT_date_2_TouchUpInside;
            BT_date_3.TouchUpInside += BT_date_3_TouchUpInside;
            BT_kanBanPrevious.TouchUpInside += BT_kanBanPrevious_TouchUpInside;
            BT_kanBanNext.TouchUpInside += BT_kanBanNext_TouchUpInside;

            BT_filter_fromdate.TouchUpInside += BT_filter_fromdate_TouchUpInside;
            BT_filter_todate.TouchUpInside += BT_filter_todate_TouchUpInside;

            BT_all.TouchUpInside += BT_all_TouchUpInside;
            BT_InProgress.TouchUpInside += BT_InProgress_TouchUpInside;
            BT_completed.TouchUpInside += BT_completed_TouchUpInside;
            BT_reject.TouchUpInside += BT_reject_TouchUpInside;
            BT_reset_filter.TouchUpInside += BT_reset_filter_TouchUpInside;
            BT_search.TouchUpInside += BT_search_TouchUpInside;

            BT_apply.TouchUpInside += BT_apply_TouchUpInside;
            //tf_search_title.EditingChanged += Tf_search_title_EditingChanged;
            tf_search_title.Started += Tf_search_title_Started;
            tf_search_title.Ended += Tf_search_title_Ended;

            tf_search_title.ReturnKeyType = UIReturnKeyType.Search;
            tf_search_title.ShouldReturn = (tf) =>
            {
                tf.EndEditing(true);
                Tf_search_title_EditingChanged(tf, null);
                return true;
            };
            #endregion
        }

        #endregion

        #region private - public method
        //app
        //public void SetContent(BeanWorkflow _workflow)
        //{
        //    workflowSelected = _workflow;
        //}
        public void SetContent(BeanWorkflow _workflow, int _indexMenu)
        {
            workflowSelected = _workflow;
        }
        private void ViewConfiguration()
        {
            BT_kanBanNext.Hidden = true;
            BT_kanBanPrevious.Hidden = true;

            SetConstraint();
            BT_search.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_filterDate.ContentEdgeInsets = new UIEdgeInsets(7, 6, 6, 6);
            BT_back.ContentEdgeInsets = new UIEdgeInsets(5, 0, 5, 0);

            BT_kanBanNext.SetImage(UIImage.FromFile("Icons/icon_next_catagory.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
            BT_kanBanPrevious.SetImage(UIImage.FromFile("Icons/icon_previous_catagory.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
            lbl_fromdate.Layer.BorderWidth = 0;
            lbl_todate.Layer.BorderWidth = 0;

            heightFilterDate = view_filterdate.Frame.Height;
            var flowLayout = new UICollectionViewFlowLayout()
            {
                SectionInset = new UIEdgeInsets(0, 0, 0, 0),
                ScrollDirection = UICollectionViewScrollDirection.Horizontal,
                MinimumInteritemSpacing = 10, // minimum spacing between cells
                MinimumLineSpacing = 10 // minimum spacing between rows if ScrollDirection is Vertical or between columns if Horizontal
            };
            constraint_heightSearch.Constant = 0;
            //view_search.Frame = new CGRect(view_search.Frame.X, view_search.Frame.Y, view_search.Frame.Width, 0);

            view_choose_kanban.Layer.CornerRadius = 4;
            view_choose_kanban.ClipsToBounds = true;

            view_search.Layer.CornerRadius = 4;
            view_search.ClipsToBounds = true;

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

            view_filterdate.AddSubview(custom_CalendarView);
            view_filterdate.BringSubviewToFront(custom_CalendarView);

            KanbanCollection.SetCollectionViewLayout(flowLayout, true);
            KanbanCollection.RegisterClassForCell(typeof(WorkFlowItem_CollectionCell), WorkFlowItem_CollectionCell.CellID);
            KanbanCollection.AllowsMultipleSelection = false;
            //KanbanCollection.AlwaysBounceVertical = true;

            //button status 
            lst_BTstatus.Add(BT_all);
            lst_BTstatus.Add(BT_InProgress);
            lst_BTstatus.Add(BT_completed);
            lst_BTstatus.Add(BT_reject);

            //bttom view
            //buttonActionBotBarApplication = ButtonsActionBroadBotBarApplication.Instance;
            //CGRect bottomBarFrame = new CGRect(bottom_view.Frame.X, bottom_view.Frame.Y, this.View.Frame.Width, bottom_view.Frame.Height);
            //buttonActionBotBarApplication.InitFrameView(bottomBarFrame);
            //buttonActionBotBarApplication.LoadStatusButton(2);
            //bottom_view.AddSubview(buttonActionBotBarApplication);
            //CmmIOSFunction.AddShadowForTopORBotBar(bottom_view, false);

            //bttom view
            buttonActionBotBar = ButtonsActionBotBar.Instance;
            //CGRect bottomBarFrame = new CGRect(bottom_view.Frame.X, bottom_view.Frame.Y, this.View.Frame.Width, bottom_view.Frame.Height);
            //buttonActionBotBar.InitFrameView(bottomBarFrame);
            buttonActionBotBar.fromKanband = true;
            buttonActionBotBar.InitFrameView(bottom_view.Frame);
            buttonActionBotBar.LoadStatusButton(3);
            bottom_view.AddSubviews(buttonActionBotBar);
            CmmIOSFunction.AddShadowForTopORBotBar(bottom_view, false);

            SettingButtonTitleKanBan();
        }
        private void SetLangTitle()
        {
            CmmIOSFunction.SetLangToView(this.View);
        }
        private void SetConstraint()
        {
            headerView_constantHeight.Constant = 45 + CmmIOSFunction.GetHeaderViewHeight();
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                if (UIApplication.SharedApplication.KeyWindow?.SafeAreaInsets.Bottom > 0)
                {
                    contraint_heightViewNavBot.Constant += 10;
                }
            }
        }
        private void ChangeColorButtonStatus(int index)
        {
            for (int i = 0; i < lst_BTstatus.Count; i++)
            {
                if (i == index)
                {
                    lst_BTstatus[i].BackgroundColor = UIColor.FromRGB(205, 227, 255);
                    lst_BTstatus[i].SetTitleColor(UIColor.FromRGB(0, 95, 212), UIControlState.Normal);
                    lst_BTstatus[i].ClipsToBounds = true;
                    lst_BTstatus[i].Layer.CornerRadius = 4;
                    lst_BTstatus[i].Font = UIFont.FromName("Arial-BoldMT", 13f);
                }
                else
                {
                    lst_BTstatus[i].BackgroundColor = UIColor.FromRGB(245, 245, 245);
                    lst_BTstatus[i].SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    lst_BTstatus[i].ClipsToBounds = true;
                    lst_BTstatus[i].Layer.CornerRadius = 4;
                    lst_BTstatus[i].Font = UIFont.FromName("ArialMT", 13f);
                }
            }
        }
        private async void LoadContent(string searchParam = "", bool doFirstLoad = false)
        {
            if (loading != null && loading.IsDescendantOfView(this.View))
            {
                loading.Hide();
            }
            loading = new CmmLoading(new CGRect(KanbanCollection.Center.X - 100, KanbanCollection.Center.Y - 100, 200, 200), "Đang xử lý...");
            this.View.Add(loading);
            this.View.BringSubviewToFront(loading);
            await Task.Run(() =>
            {
                try
                {
                    if (doFirstLoad)
                        lstAllItem = GetListKanBanOnline(workflowSelected.WorkflowID.ToString());

                    //InvokeOnMainThread(() =>
                    //{
                    //    if (CmmVariable.SysConfig.LangCode == "1033")
                    //        lbl_titleWorkFlow.Text = workflowSelected.TitleEN;
                    //    else
                    //        lbl_titleWorkFlow.Text = workflowSelected.Title;
                    //});

                    if (lstAllItem != null || lstAllItem.Count >= 0)
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
                                        strStatusGroup += string.Format(@" AND StatusGroup NOT IN ({0}) ", string.Join(", ", StatusGroupInItem));
                                    else
                                        strStatusGroup += string.Format(@" AND StatusGroup IN ({0}) ", string.Join(", ", StatusGroupInItem));
                                }
                                // query
                                if (Reachability.detectNetWork())
                                {
                                    if (lstAllItem != null && lstAllItem.Count > 0)
                                    {
                                        lst_res = lstAllItem.Where(
                                            x => x.Step.HasValue && x.Step.Value == item.Step
                                            && ((currentMenu.ID == 2 && x.Step.Value == _ApprovedStepID) // filter đã xử lý
                                            || (currentMenu.ID == 3 && x.Step.Value == _RejectedStepID)// filter đã từ chối
                                            || (currentMenu.ID == 1 && (x.Step.Value != _ApprovedStepID && x.Step.Value != _RejectedStepID)) //filter chờ xử lý
                                            || (currentMenu.ID == 0))//filter all tình trạng
                                            && (x.Created.HasValue && x.Created.Value >= fromDateSelected && x.Created.Value <= toDateSelected.AddDays(1))
                                            && (x.ApprovalStatus.HasValue && x.ApprovalStatus.Value != 0)
                                        ).OrderByDescending(o => o.Created).ToList();
                                    }
                                }
                                else
                                {
                                    string query_approved = string.Format(@"SELECT * FROM BeanAppBaseExt"//BeanAppBase
                                    + "WHERE {0} WorkflowID = {1} AND ResourceCategoryId <> 16 {2} {3} "
                                    + "ORDER BY Created DESC"
                                    , date_filter, item.WorkflowID, strStatusGroup, strStep);

                                    lst_res = conn.Query<BeanAppBaseExt>(query_approved);
                                    var lst = lst_res.Where(x => x.ApprovalStatus.HasValue && x.ApprovalStatus.Value != 0).ToList();
                                    lst_res = lst;
                                }
                                //search
                                if (!string.IsNullOrEmpty(searchParam) && lst_res.Count > 0)
                                {
                                    searchParam = CmmFunction.removeSignVietnamese(searchParam.ToLowerInvariant());
                                    lst_search_workflowItems = lst_res;
                                    List<BeanAppBaseExt> _lst_searchResult = new List<BeanAppBaseExt>();
                                    //search by title
                                    _lst_searchResult = (from wfi in lst_search_workflowItems
                                                         where (!string.IsNullOrEmpty(wfi.Content)
                                                         && CmmFunction.removeSignVietnamese(wfi.Content.ToLowerInvariant()).Contains(CmmFunction.removeSignVietnamese(searchParam)))
                                                         select wfi).ToList();
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

                        //KanbanCollection.DragDelegate = new CustomKanBanDragCollection(this, collectionWorkFlowStep_Source, KanbanCollection);
                        //KanbanCollection.DropDelegate = new CustomKanBanDropCollection(this, collectionWorkFlowStep_Source, KanbanCollection);
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

        void ReloadFailedBeanData()
        {
            CmmIOSFunction.UpdateBeanData<BeanWorkflow>();
            CmmIOSFunction.UpdateBeanData<BeanWorkflowItem>();
            CmmIOSFunction.UpdateBeanData<BeanWorkflowStepDefine>();
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

        private void SetDate()
        {
            dict_indexRangeDayChoice = new Dictionary<string, int>();
            dict_indexRangeDayChoice.Add("today", 0);
            dict_indexRangeDayChoice.Add("yesterday", 1);
            dict_indexRangeDayChoice.Add("7days", 7);
            dict_indexRangeDayChoice.Add("30days", 30);

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
                lbl_titleWorkFlow.Text = workflowSelected.TitleEN;
            }
            else
            {
                lbl_todate.Text = toDateSelected.ToString("dd/MM/yyyy");
                lbl_fromdate.Text = fromDateSelected.ToString("dd/MM/yyyy");
                lbl_titleWorkFlow.Text = workflowSelected.Title;
            }
        }
        public void reloadData()
        {
            LoadContent(tf_search_title.Text, true);
        }
        //private void LoadDataFilter()
        //{
        //    if (CmmVariable.SysConfig.LangCode == "1066")
        //    {
        //        lbl_todate.Text = toDateSelected.ToString("dd/MM/yyyy");
        //        lbl_fromdate.Text = fromDateSelected.ToString("dd/MM/yyyy");
        //    }
        //    else
        //    {
        //        lbl_todate.Text = toDateSelected.ToString("MM/dd/yy");
        //        lbl_fromdate.Text = fromDateSelected.ToString("MM/dd/yyyy");
        //    }

        //    //lbl_date.Text = lbl_fromdate.Text.Remove(lbl_fromdate.Text.Length - 2, 2) + " - " + lbl_todate.Text.Remove(lbl_todate.Text.Length - 2, 2);

        //    LoadContent(null);
        //}
        private void SearchToggle()
        {
            if (!isSearch) // search dang dong
            {
                if (view_search.Frame.Height > 0)
                {
                    if (!string.IsNullOrEmpty(tf_search_title.Text.Trim()))
                    {
                        isSearch = true;
                        tf_search_title.BecomeFirstResponder();
                        BT_search.Enabled = true;
                    }
                    else
                    {
                        isSearch = false;
                        UIView.BeginAnimations("search_slideAnimationShow");
                        UIView.SetAnimationDuration(0.4f);
                        UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                        UIView.SetAnimationRepeatCount(0);
                        UIView.SetAnimationRepeatAutoreverses(false);
                        UIView.SetAnimationDelegate(this);
                        view_search.Alpha = 0;
                        view_search.Hidden = true;
                        constraint_heightSearch.Constant = 0;
                        this.View.EndEditing(true);
                        UIView.CommitAnimations();
                        BT_search.Enabled = true;
                        BT_search.SetImage(UIImage.FromFile("Icons/icon_Search.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
                        BT_search.TintColor = UIColor.FromRGB(0, 0, 0);
                    }
                }
                else
                {
                    isSearch = true;

                    view_search.Alpha = 1;
                    view_search.Hidden = false;
                    UIView.BeginAnimations("search_slideAnimationShow");
                    UIView.SetAnimationDuration(0.4f);
                    UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
                    UIView.SetAnimationRepeatCount(0);
                    UIView.SetAnimationRepeatAutoreverses(false);
                    UIView.SetAnimationDelegate(this);
                    constraint_heightSearch.Constant = 44;
                    UIView.CommitAnimations();
                    tf_search_title.BecomeFirstResponder();
                    BT_search.Enabled = true;
                    BT_search.SetImage(UIImage.FromFile("Icons/icon_Search.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
                    BT_search.TintColor = UIColor.FromRGB(40, 176, 255);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(tf_search_title.Text.Trim()))
                {
                    isSearch = false;
                    this.View.EndEditing(true);
                    BT_search.Enabled = true;
                }
                else
                {
                    isSearch = false;
                    UIView.BeginAnimations("search_slideAnimationShow");
                    UIView.SetAnimationDuration(0.4f);
                    UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                    UIView.SetAnimationRepeatCount(0);
                    UIView.SetAnimationRepeatAutoreverses(false);
                    UIView.SetAnimationDelegate(this);
                    view_search.Alpha = 0;
                    view_search.Hidden = true;
                    constraint_heightSearch.Constant = 0;
                    this.View.EndEditing(true);
                    UIView.CommitAnimations();
                    BT_search.Enabled = true;
                    BT_search.SetImage(UIImage.FromFile("Icons/icon_Search.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
                    BT_search.TintColor = UIColor.FromRGB(0, 0, 0);
                }
            }
        }

        private void SearchData(string _searchPara)
        {
            try
            {
                LoadContent(_searchPara, false);
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
            menu1.title = CmmFunction.GetTitle("TEXT_WAITING_APPROVE", "Chờ phê duyệt");
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

        //private void ToggleMenuCategory()
        //{
        //    Custom_MenuOption custom_MenuOption = Custom_MenuOption.Instance;
        //    if (custom_MenuOption.Superview != null)
        //        custom_MenuOption.RemoveFromSuperview();
        //    else
        //    {
        //        custom_MenuOption.ItemNoIcon = false;
        //        custom_MenuOption.viewController = this;
        //        custom_MenuOption.InitFrameView(new CGRect(view_category.Frame.X - 50, view_category.Frame.Bottom + 2, view_category.Frame.Width + 50, lst_menu_cate.Count * custom_MenuOption.RowHeigth));
        //        custom_MenuOption.AddShadowForView();
        //        custom_MenuOption.ListItemMenu = lst_menu_cate;
        //        custom_MenuOption.TableLoadData();

        //        view_content.AddSubview(custom_MenuOption);
        //        view_content.BringSubviewToFront(custom_MenuOption);
        //    }
        //}

        //private void ToggleMenuDate()
        //{
        //    if (!isFilter)
        //    {
        //        isFilter = true;
        //        view_filter.Hidden = false;
        //        //custom_CalendarView.Hidden = false;
        //    }
        //    else
        //    {
        //        isFilter = false;
        //        view_filter.Hidden = true;
        //        //custom_CalendarView.Hidden = true;
        //    }
        //}
        private void ToggleMenuDate()
        {
            if (!isFilter)
            {
                isFilter = true;
                view_filter.Hidden = false;
                custom_CalendarView.Hidden = false;

                view_filter.Frame = new CGRect(0, 0, view_filter.Frame.Width, 0);
                UIView.BeginAnimations("show_animationShowFilter");
                UIView.SetAnimationDuration(0.2f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);
                view_filter.Alpha = 1;
                view_filter.Frame = new CGRect(0, 0, view_filter.Frame.Width, heightFilterDate);
                UIView.CommitAnimations();

                BT_filterDate.SetImage(UIImage.FromFile("Icons/icon_filter_broad.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
                BT_filterDate.TintColor = UIColor.FromRGB(35, 151, 32); // moi lan mo len thi bat filter mau xanh
                ToggleCalendar(false);
                SetValueFormFilter();
            }
            else
            {
                isFilter = false;
                view_filter.Hidden = true;
                custom_CalendarView.Hidden = true;

                view_filter.Frame = new CGRect(0, 0, view_filter.Frame.Width, heightFilterDate);
                UIView.BeginAnimations("show_animationHideFilter");
                UIView.SetAnimationDuration(0.2f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);
                view_filter.Frame = new CGRect(0, 0, view_filter.Frame.Width, 0);
                view_filter.Alpha = 0;
                UIView.CommitAnimations();
                BT_filterDate.SetImage(UIImage.FromFile("Icons/icon_filter_broad.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
                BT_filterDate.TintColor = previousColorFilter;
            }
        }

        public void ToggleCalendar(bool _show)
        {
            if (!_show)
            {
                if (custom_CalendarView != null)
                {
                    custom_CalendarView.RemoveFromSuperview();
                    custom_CalendarView.reset();
                }
            }
            else
            {
                custom_CalendarView.Hidden = false;
                view_filterdate.AddSubview(custom_CalendarView);
                view_filterdate.BringSubviewToFront(custom_CalendarView);
                custom_CalendarView.InitFrameView(new CGRect(view_filterdate.Frame.X, BT_apply.Frame.Bottom + 20, view_filterdate.Frame.Width, 260));
            }
        }

        public void ChangeRangeDateWhenChooseDate()
        {
            DateTime fromDate = new DateTime();
            DateTime toDate = new DateTime();

            if (lbl_fromdate.Text != "")
                fromDate = DateTime.Parse(lbl_fromdate.Text, new CultureInfo(CmmVariable.SysConfig.LangCode == "1033" ? "en" : "vi", false));
            else
                fromDate = new DateTime();

            if (lbl_todate.Text != "")
                toDate = DateTime.Parse(lbl_todate.Text, new CultureInfo(CmmVariable.SysConfig.LangCode == "1033" ? "en" : "vi", false));
            else
                toDate = new DateTime();
            SetCurrentRangeDateFollowDate(toDate, fromDate);
        }
        private void SetCurrentRangeDateFollowDate(DateTime todate, DateTime fromdate)
        {
            if (fromdate.Date == DateTime.Now.Date && todate.Date == DateTime.Now.Date)// Today
            {
                RangeTemp = rangeDateSelect = 0;
                DateRangeSwitch(rangeDateSelect);
            }
            else if (fromdate.Date == DateTime.Now.Date.AddDays(-1) && todate.Date == DateTime.Now.Date.AddDays(-1))// Yesterday
            {
                RangeTemp = rangeDateSelect = 1;
                DateRangeSwitch(rangeDateSelect);
            }
            else if (fromdate.Date == DateTime.Now.Date.AddDays(-7) && todate.Date == DateTime.Now.Date) // 7 Days
            {
                RangeTemp = rangeDateSelect = 2;
                DateRangeSwitch(rangeDateSelect);
            }
            else if (fromdate.Date == DateTime.Now.Date.AddDays(-30) && todate.Date == DateTime.Now.Date)// 1 months
            {
                RangeTemp = rangeDateSelect = 3;
                DateRangeSwitch(rangeDateSelect);
            }
            else
            {
                RangeTemp = rangeDateSelect = -1;
                DateRangeSwitch(rangeDateSelect, todate, fromdate);
            }
        }
        private void SetValueFormFilter()
        {
            //current rangedate
            SetCurrentRangeDateFollowDate(toDateSelected, fromDateSelected);

            //current  date
            view_fromdate.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;
            view_todate.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;

            //current status
            var index = lst_menu_cate.FindIndex(s => s.ID == currentMenu.ID);
            if (index >= 0)
                ChangeColorButtonStatus(index);
        }
        private void DateRangeSwitch(int index, DateTime todate = default, DateTime fromdate = default)
        {
            DateTime toDateSelectedTemp = DateTime.Now.Date;
            DateTime fromDateSelectedTemp = DateTime.Now.Date;

            switch (index)
            {
                case 0: //Today
                    BT_date_0.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);
                    BT_date_0.Font = UIFont.FromName("Arial-BoldMT", 14f);
                    indexRangeSelected = dict_indexRangeDayChoice["today"];
                    BT_date_1.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_1.Font = UIFont.FromName("ArialMT", 14f);
                    BT_date_2.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_2.Font = UIFont.FromName("ArialMT", 14f);
                    BT_date_3.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_3.Font = UIFont.FromName("ArialMT", 14f);

                    toDateSelectedTemp = DateTime.Now.Date;
                    fromDateSelectedTemp = fromDateSelectedTemp.AddDays(-indexRangeSelected);

                    //LoadDataFilter();
                    break;
                case 1: //Yesterday
                    BT_date_0.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_0.Font = UIFont.FromName("ArialMT", 14f);
                    BT_date_1.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);
                    BT_date_1.Font = UIFont.FromName("Arial-BoldMT", 14f);
                    indexRangeSelected = dict_indexRangeDayChoice["yesterday"];
                    BT_date_2.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_2.Font = UIFont.FromName("ArialMT", 14f);
                    BT_date_3.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_3.Font = UIFont.FromName("ArialMT", 14f);

                    toDateSelectedTemp = DateTime.Now.Date.AddDays(-1);
                    fromDateSelectedTemp = fromDateSelectedTemp.AddDays(-indexRangeSelected);

                    //LoadDataFilter();
                    break;
                case 2: //7 days
                    BT_date_0.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_0.Font = UIFont.FromName("ArialMT", 14f);
                    BT_date_1.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_1.Font = UIFont.FromName("ArialMT", 14f);
                    BT_date_2.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);
                    BT_date_2.Font = UIFont.FromName("Arial-BoldMT", 14f);
                    indexRangeSelected = dict_indexRangeDayChoice["7days"];
                    BT_date_3.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_3.Font = UIFont.FromName("ArialMT", 14f);

                    toDateSelectedTemp = DateTime.Now.Date;
                    fromDateSelectedTemp = fromDateSelectedTemp.AddDays(-indexRangeSelected);

                    //LoadDataFilter();
                    break;
                case 3: //30 days
                    BT_date_0.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_0.Font = UIFont.FromName("ArialMT", 14f);
                    BT_date_1.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_1.Font = UIFont.FromName("ArialMT", 14f);
                    BT_date_2.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_2.Font = UIFont.FromName("ArialMT", 14f);
                    BT_date_3.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);
                    BT_date_3.Font = UIFont.FromName("Arial-BoldMT", 14f);
                    indexRangeSelected = dict_indexRangeDayChoice["30days"];

                    toDateSelectedTemp = DateTime.Now.Date;
                    fromDateSelectedTemp = fromDateSelectedTemp.AddDays(-indexRangeSelected);

                    //LoadDataFilter();
                    break;
                default: //user select date
                    BT_date_0.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_0.Font = UIFont.FromName("ArialMT", 14f);
                    BT_date_1.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_1.Font = UIFont.FromName("ArialMT", 14f);
                    BT_date_2.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_2.Font = UIFont.FromName("ArialMT", 14f);
                    BT_date_3.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                    BT_date_3.Font = UIFont.FromName("ArialMT", 14f);
                    indexRangeSelected = dict_indexRangeDayChoice["30days"];

                    toDateSelectedTemp = todate;
                    fromDateSelectedTemp = fromdate;
                    break;
            }

            if (toDateSelectedTemp != default(DateTime))
            {
                if (CmmVariable.SysConfig.LangCode == "1033")
                    lbl_todate.Text = toDateSelectedTemp.ToString("MM/dd/yyyy");
                else
                    lbl_todate.Text = toDateSelectedTemp.ToString("dd/MM/yyyy");
            }
            else
                lbl_todate.Text = "";

            if (fromDateSelectedTemp != default(DateTime))
            {
                if (CmmVariable.SysConfig.LangCode == "1033")
                    lbl_fromdate.Text = fromDateSelectedTemp.ToString("MM/dd/yyyy");
                else
                    lbl_fromdate.Text = fromDateSelectedTemp.ToString("dd/MM/yyyy");
            }
            else
                lbl_fromdate.Text = "";
        }

        private bool validateDateFilter()
        {
            bool res = false;
            if (fromDateSelected > toDateSelected && toDateSelected != default(DateTime))
                res = false;
            else
                res = true;

            return res;
        }

        //public void HandleMenuOptionResult(ClassMenu _menu)
        //{
        //    if (_menu != null)
        //    {
        //        _menu.isSelected = true;

        //        if (currentMenu != null && currentMenu.ID != _menu.ID)
        //        {
        //            currentMenu.isSelected = false;
        //            currentMenu = _menu;
        //            //lbl_category.Text = _menu.title;
        //            LoadContent(null);
        //            LoadContentList();
        //        }
        //    }

        //    Custom_MenuOption custom_menuOption = Custom_MenuOption.Instance;
        //    if (custom_menuOption.Superview != null)
        //        custom_menuOption.RemoveFromSuperview();
        //}

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
                                    loading.Hide();

                                    CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
                                    CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
                                    PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);

                                    AgreeOrRejectView reject = (AgreeOrRejectView)Storyboard.InstantiateViewController("AgreeOrRejectView");
                                    reject.setContent(_buttonAction, this, null, null, workflowItemSelected);
                                    transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                                    reject.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                                    reject.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                                    reject.TransitioningDelegate = transitioningDelegate;
                                    this.PresentViewControllerAsync(reject, true);

                                    //FormApproveOrRejectView formApproveOrReject = (FormApproveOrRejectView)Storyboard.InstantiateViewController("FormApproveOrRejectView");
                                    //formApproveOrReject.SetContent(this, _buttonAction);
                                    //transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                                    //formApproveOrReject.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                                    //formApproveOrReject.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                                    //formApproveOrReject.TransitioningDelegate = transitioningDelegate;
                                    //this.PresentViewControllerAsync(formApproveOrReject, true);
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
                            CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("TEXT_ACTION_NOT_PERMIT", "Bạn không có quyền thực hiện hành động này."));//
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
        public async void ActionApprove(ButtonAction _buttonAction, List<KeyValuePair<string, string>> lstExtent = null)
        {
            await Task.Run(() =>
            {
                bool result = false;
                ProviderBase b_pase = new ProviderBase();
                ProviderControlDynamic providerControl = new ProviderControlDynamic();
                //List<KeyValuePair<string, string>> lstExtent = new List<KeyValuePair<string, string>>();


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
        //public void CloseCustomCalendar()
        //{
        //    Custom_CalendarView custom_CalendarView = Custom_CalendarView.Instance;
        //    if (custom_CalendarView.Superview != null)
        //    {
        //        custom_CalendarView.RemoveFromSuperview();
        //        custom_CalendarView.reset();
        //    }
        //}
        public void NavigateToWorkFlowDetails(BeanAppBaseExt _beanAppBaseExt)
        {
            RequestDetailsV2 requestDetailsV2 = (RequestDetailsV2)Storyboard.InstantiateViewController("RequestDetailsV2");
            requestDetailsV2.setContent(this, _beanAppBaseExt);
            this.NavigationController.PushViewController(requestDetailsV2, true);
        }
        #endregion

        #region events
        private void BT_back_TouchUpInside(object sender, EventArgs e)
        {
            //app
            //buttonActionBotBarApplication.broadView.BackFromeKanBanView();
            buttonActionBotBar.fromKanband = false;
            this.NavigationController.PopViewController(true);
        }
        //private void BT_category_TouchUpInside(object sender, EventArgs e)
        //{
        //    ToggleMenuCategory();
        //}
        private void BT_filterDate_TouchUpInside(object sender, EventArgs e)
        {
            if (isSearch)
                this.View.EndEditing(true);
            ToggleMenuDate();
        }
        private void BT_date_0_TouchUpInside(object sender, EventArgs e)
        {
            RangeTemp = 0;
            DateRangeSwitch(RangeTemp);
        }
        private void BT_date_1_TouchUpInside(object sender, EventArgs e)
        {
            RangeTemp = 1;
            DateRangeSwitch(RangeTemp);
        }
        private void BT_date_2_TouchUpInside(object sender, EventArgs e)
        {
            RangeTemp = 2;
            DateRangeSwitch(RangeTemp);
        }
        private void BT_date_3_TouchUpInside(object sender, EventArgs e)
        {
            RangeTemp = 3;
            DateRangeSwitch(RangeTemp);
        }
        private void BT_filter_fromdate_TouchUpInside(object sender, EventArgs e)
        {
            view_fromdate.Layer.BorderColor = UIColor.FromRGB(65, 80, 134).CGColor;
            view_todate.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;

            custom_CalendarView.inputView = lbl_fromdate;
            custom_CalendarView.viewController = this;
            custom_CalendarView.SetUpDate();

            ToggleCalendar(true);

        }
        private void BT_filter_todate_TouchUpInside(object sender, EventArgs e)
        {
            view_fromdate.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;
            view_todate.Layer.BorderColor = UIColor.FromRGB(65, 80, 134).CGColor;

            custom_CalendarView.inputView = lbl_todate;
            custom_CalendarView.viewController = this;
            custom_CalendarView.SetUpDate();

            ToggleCalendar(true);
        }
        private void Tf_search_title_EditingChanged(object sender, EventArgs e)
        {
            if (tf_search_title.Text != hintTextSearchTitle)
                SearchData(tf_search_title.Text);
        }
        private void BT_apply_TouchUpInside(object sender, EventArgs e)
        {
            if (lbl_fromdate.Text != "")
                fromDateSelected = DateTime.Parse(lbl_fromdate.Text, new CultureInfo(CmmVariable.SysConfig.LangCode == "1033" ? "en" : "vi", false));
            else
                fromDateSelected = new DateTime();

            if (lbl_todate.Text != "")
                toDateSelected = DateTime.Parse(lbl_todate.Text, new CultureInfo(CmmVariable.SysConfig.LangCode == "1033" ? "en" : "vi", false));
            else
                toDateSelected = new DateTime();

            if (validateDateFilter())
            {
                if (isSearch)
                {
                    tf_search_title.Text = "";
                    SearchToggle();
                }

                previousColorFilter = UIColor.FromRGB(35, 151, 32);
                currentMenu = lst_menu_cate.Find(s => s.isSelected == true);
                rangeDateSelect = RangeTemp;
                ToggleMenuDate();
                LoadContent(null, false);
            }
            else
            {
                CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("TEXT_DATE_INVALID", "Ngày bắt đầu không được lớn hơn ngày kết thúc"));
            }
        }
        private void Tf_search_title_Started(object sender, EventArgs e)
        {
            //tf_search_title.Text = "";
        }
        private void Tf_search_title_Ended(object sender, EventArgs e)
        {
            if (tf_search_title.Text == "")
            {
                tf_search_title.Text = hintTextSearchTitle;
            }
        }
        private void BT_search_TouchUpInside(object sender, EventArgs e)
        {
            BT_search.UserInteractionEnabled = false;
            if (isFilter)
            {
                ToggleMenuDate();
            }
            tf_search_title.Text = string.IsNullOrEmpty(tf_search_title.Text)
                ? ""
                : string.Compare(tf_search_title.Text, tf_search_title.Placeholder.ToString()) == 0
                    ? ""
                    : tf_search_title.Text;
            SearchToggle();
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1f));

            BT_search.UserInteractionEnabled = true;
        }

        private void BT_reset_filter_TouchUpInside(object sender, EventArgs e)
        {
            previousColorFilter = UIColor.FromRGB(0, 0, 0);
            view_fromdate.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;
            view_todate.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;
            LoadStatusFilterCategory();
            SetDate();
            RangeTemp = rangeDateSelect = 3;
            DateRangeSwitch(rangeDateSelect);
            // tat ca
            currentMenu = lst_menu_cate[0];
            ChangeColorButtonStatus(0);
            ToggleMenuDate();
            LoadContent(null, false);
        }

        //private void BT_reset_filter_TouchUpInside(object sender, EventArgs e)
        //{
        //    LoadStatusFilterCategory();
        //    SetDate();
        //    // tat ca
        //    currentMenu = lst_menu_cate[0];
        //    ChangeColorButtonStatus(0);
        //}

        private void BT_reject_TouchUpInside(object sender, EventArgs e)
        {

            // tu choi
            lst_menu_cate = lst_menu_cate.Select(a => { a.isSelected = false; return a; }).ToList();
            lst_menu_cate[3].isSelected = true;
            ChangeColorButtonStatus(3);
        }

        private void BT_completed_TouchUpInside(object sender, EventArgs e)
        {
            // hoan thanh
            lst_menu_cate = lst_menu_cate.Select(a => { a.isSelected = false; return a; }).ToList();
            lst_menu_cate[2].isSelected = true;
            ChangeColorButtonStatus(2);
        }

        private void BT_InProgress_TouchUpInside(object sender, EventArgs e)
        {
            // dang thuc hien
            lst_menu_cate = lst_menu_cate.Select(a => { a.isSelected = false; return a; }).ToList();
            lst_menu_cate[1].isSelected = true;
            ChangeColorButtonStatus(1);
        }

        private void BT_all_TouchUpInside(object sender, EventArgs e)
        {
            // tat ca
            lst_menu_cate = lst_menu_cate.Select(a => { a.isSelected = false; return a; }).ToList();
            lst_menu_cate[0].isSelected = true;
            ChangeColorButtonStatus(0);
        }

        private void SettingButtonTitleKanBan()
        {
            //bt_next
            SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
            string _queryNext = String.Format(@"SELECT * FROM BeanWorkflow WHERE WorkflowID > {0} AND StatusName = 'Active' ORDER BY WorkflowID ASC LIMIT 1 OFFSET 0", workflowSelected.WorkflowID);
            List<BeanWorkflow> lst_Next = conn.Query<BeanWorkflow>(_queryNext);
            if (lst_Next == null || lst_Next.Count == 0)
            {
                BT_kanBanNext.Enabled = false;
                BT_kanBanNext.ImageView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                BT_kanBanNext.ImageView.TintColor = UIColor.LightGray;
            }
            else
            {
                BT_kanBanNext.ImageView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
                BT_kanBanNext.Enabled = true;
            }

            //bt_previous
            string _queryPrevious = String.Format(@"SELECT * FROM BeanWorkflow WHERE WorkflowID < {0} AND StatusName = 'Active' ORDER BY WorkflowID DESC LIMIT 1 OFFSET 0", workflowSelected.WorkflowID);
            List<BeanWorkflow> lst_Previous = conn.Query<BeanWorkflow>(_queryPrevious);
            if (lst_Previous == null || lst_Previous.Count == 0)
            {
                BT_kanBanPrevious.Enabled = false;
                BT_kanBanPrevious.ImageView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                BT_kanBanPrevious.ImageView.TintColor = UIColor.LightGray;
            }
            else
            {
                BT_kanBanPrevious.ImageView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
                BT_kanBanPrevious.Enabled = true;
            }

        }
        private void BT_kanBanNext_TouchUpInside(object sender, EventArgs e)
        {
            SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
            string _queryNext = String.Format(@"SELECT * FROM BeanWorkflow WHERE WorkflowID > {0} AND StatusName = 'Active' ORDER BY WorkflowID ASC LIMIT 1 OFFSET 0", workflowSelected.WorkflowID);
            List<BeanWorkflow> lst_Next = conn.Query<BeanWorkflow>(_queryNext);
            if (lst_Next != null && lst_Next.Count > 0)
            {
                workflowSelected = lst_Next[0];
                LoadStatusFilterCategory();
                SetDate();
                LoadContent(null, false);
                SettingButtonTitleKanBan();
            }

        }
        private void BT_kanBanPrevious_TouchUpInside(object sender, EventArgs e)
        {
            SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath);
            string _queryPrevious = String.Format(@"SELECT * FROM BeanWorkflow WHERE WorkflowID < {0} AND StatusName = 'Active' ORDER BY WorkflowID DESC LIMIT 1 OFFSET 0", workflowSelected.WorkflowID);
            List<BeanWorkflow> lst_Previous = conn.Query<BeanWorkflow>(_queryPrevious);
            if (lst_Previous != null && lst_Previous.Count > 0)
            {
                workflowSelected = lst_Previous[0];

                LoadStatusFilterCategory();
                SetDate();
                LoadContent(null, false);
                SettingButtonTitleKanBan();
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
                return new CGSize(300, collectionView.Frame.Height);
                //return new CGSize(300, parentView.heightCollection);
            }

            public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
            {
                //var itemSelected = collect_source.lst_workFlow[indexPath.Row];
                //parentView.HandleSeclectItem(itemSelected);
            }
            #endregion
        }

        #endregion

        #endregion

    }
}


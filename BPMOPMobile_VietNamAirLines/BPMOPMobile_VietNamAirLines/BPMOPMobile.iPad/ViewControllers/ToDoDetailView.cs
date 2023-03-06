using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.iPad.Components;
using BPMOPMobile.iPad.CustomControlClass;
using BPMOPMobile.iPad.IOSClass;
using BPMOPMobile.iPad.ViewControllers;
using CoreGraphics;
using Foundation;
using MobileCoreServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UIKit;

namespace BPMOPMobile.iPad
{
    public partial class ToDoDetailView : UIViewController
    {
        private NSObject _willResignActiveNotificationObserver;
        private NSObject _didBecomeActiveNotificationObserver;
        AppDelegate appD;
        ButtonsActionTopBar buttonActionTopBar;
        ButtonsActionBotBar buttonActionBotBar;
        UIRefreshControl todo_refreshControl;
        List<BeanNotify> lst_notify_cxl = new List<BeanNotify>();
        List<BeanAppBaseExt> lst_appBase_cxl = new List<BeanAppBaseExt>();
        List<BeanAppBaseExt> lst_notify_cxl_results = new List<BeanAppBaseExt>();
        public BeanWorkflowItem workflowItem;
        public BeanTask taskItem;
        List<BeanWorkFlowRelated> lstWorkFlowRelateds;
        List<BeanTask> lst_tasks;
        List<CountNum> countnum_vcxl = new List<CountNum>();
        CmmLoading loading, lstViewLoading;
        List<BeanAttachFile> lst_attachFile;
        List<BeanAttachFile> lst_addAttachment;
        public List<BeanAttachFile> lst_addCommentAttachment;
        Dictionary<string, string> dic_valueObject = new Dictionary<string, string>();
        List<BeanQuaTrinhLuanChuyen> lst_qtlc;
        List<string> lst_userName = new List<string>();
        string str_json_FormDefineInfo = string.Empty;
        string json_attachRemove;
        string json_PropertyRemove;
        JObject JObjectSource = new JObject(); // JObject những Element ko phải calculated
        public List<JObject> lstGridDetail_Deleted = new List<JObject>(); // lưu lại những item nào đã bị xóa ra khỏi Control InputgridDetail
        string[] arr_assignedTo;
        public string searchKeyword;
        int dangxuly_count;
        int limit = CmmVariable.M_DataLimitRow;
        int offset = 0;
        CGRect view_buttonAction_default;
        public DateTime fromDateSelected;
        public DateTime toDateSelected;
        public bool isFilter;
        public List<ClassMenu> lst_trangthai;
        public ClassMenu TrangThaiSelected;
        public List<BeanAppStatus> lst_appStatus;
        private List<BeanAppStatus> lst_appStatus_selected;
        public List<ClassMenu> lst_dueDateMenu;
        private ClassMenu DuedateSelected;
        public string date_filter = string.Empty;
        bool isExpandUser;
        public bool isSearch;
        public bool isShowTask;
        bool isLoadMore = true;
        bool isLoadData = false;
        public bool isShowKeyBoarFromComment;
        bool isFollow;
        public bool tab_Dangxuly = true;
        string query_dangxuly;
        nfloat origin_view_header_content_height_constant;
        nfloat origin_constraint_rightBT_Search;
        public nfloat estCommmentViewRowHeight;
        UIImagePickerController imagePicker;
        UIDocumentPickerViewController docPicker;
        //value form default
        public DateTime fromDate_default;
        public DateTime toDate_default;
        string localDocumentFilepath = string.Empty;
        List<BeanNotify> lst_fullNotify = new List<BeanNotify>();
        Dictionary<string, List<BeanAppBaseExt>> dict_todo = new Dictionary<string, List<BeanAppBaseExt>>();
        Dictionary<string, bool> dict_sectionTodo = new Dictionary<string, bool>();
        public BeanAppBaseExt currentSelectedItem { get; set; }
        NSIndexPath currentIndexSelected { get; set; }
        List<ViewSection> lst_section { get; set; }
        private List<KeyValuePair<string, string>> lstExtent = new List<KeyValuePair<string, string>>();
        //các trường nào cho phép chỉnh sửa thì luu lại 
        private List<string> lst_ControlEnable = new List<string>();
        string Json_FormDataString = string.Empty;
        ComponentButtonBot componentButton;
        // Comment
        private List<BeanComment> lst_comments;
        public List<BeanAttachFile> _lstAttachComment = new List<BeanAttachFile>();
        private DateTime _CommentChanged;
        public string _OtherResourceId = "";
        //Attachments
        int numRowAttachmentFile = 0;
        ViewElement attachmentElement;
        List<BeanNotify> lst_notify_result = new List<BeanNotify>();
        bool IsFirstLoad = true;
        Dictionary<string, List<BeanAppBaseExt>> dict_todo_result = new Dictionary<string, List<BeanAppBaseExt>>();

        List<ButtonAction> lst_menuItem = new List<ButtonAction>();
        UIButton btn_ReadFull;
        int vcxlCount = 0;
        bool isOnline = true;
        UIStringAttributes firstAttributes = new UIStringAttributes
        {
            Font = UIFont.FromName("ArialMT", 13f)
        };

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="handle"></param>
        public ToDoDetailView(IntPtr handle) : base(handle)
        {
            appD = UIApplication.SharedApplication.Delegate as AppDelegate;
            localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }

        #region override
        bool shouldHideHomeIndicator = true;
        public override bool PrefersHomeIndicatorAutoHidden => shouldHideHomeIndicator;

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            shouldHideHomeIndicator = true;
            this.SetNeedsUpdateOfHomeIndicatorAutoHidden();

            if (isSearch)
            {
                isSearch = !isSearch;
                BT_search_TouchUpInside(null, null);
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            _willResignActiveNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, KeyBoardUpNotification);
            _didBecomeActiveNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyBoardDownNotification);

            /*if (buttonActionBotBar == null)
            {
                buttonActionBotBar = ButtonsActionBotBar.Instance;
                buttonActionBotBar.InitFrameView(view_bot_bar.Frame);
                buttonActionBotBar.LoadStatusButton(3);
                view_BotBar_ConstantHeight.Constant = view_BotBar_ConstantHeight.Constant + 10;
                view_bot_bar.AddSubviews(buttonActionBotBar);
            }
            else
            if (buttonActionTopBar != null && buttonActionBotBar != null)
            {
                view_top_bar.AddSubviews(buttonActionTopBar);
                view_bot_bar.AddSubviews(buttonActionBotBar);
            }*/

            if (buttonActionBotBar != null)
            {
                view_bot_bar.AddSubviews(buttonActionBotBar);
            }
        }

        public override void ViewDidLayoutSubviews()
        {
            if (IsFirstLoad)
            {
                IsFirstLoad = false;
                ViewConfiguration();
            }
        }

        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();

            try
            {
                toDate_default = DateTime.Now.Date;
                fromDate_default = DateTime.Now.Date.AddDays(-CmmVariable.M_DataFilterDefaultDays);
                view_buttonAction_default = view_buttonAction.Frame;
                todo_refreshControl = new UIRefreshControl();
                view_BotBar_ConstantHeight.Constant = view_BotBar_ConstantHeight.Constant + 10;

                btn_ReadFull = new UIButton
                {
                    BackgroundColor = UIColor.Clear,
                    Hidden = true,
                };
                this.View.AddSubview(btn_ReadFull);
                this.View.BringSubviewToFront(btn_ReadFull);

                if (workflowItem != null)
                    loadQuaTrinhluanchuyen();
                SetLangTitle();

                //BT_DangXL.SetTitle(CmmFunction.GetTitle(CmmVariable.APPSTATUS_TOME_DANGXULY), UIControlState.Normal); // tat ca
                //BT_DaXL.SetTitle(CmmFunction.GetTitle("TEXT_IPAD_TOME", "Trang chủ  >  Đến tôi"), UIControlState.Normal);

                _ = Task.Run(() =>
                {
                    ////set value default filter date = 30 days
                    //if (toDateSelected == default)
                    //{
                    //    toDateSelected = DateTime.Now.Date;
                    //    toDate_default = toDateSelected;
                    //}

                    //if (fromDateSelected == default)
                    //{
                    //    fromDateSelected = toDateSelected.AddDays(-CmmVariable.M_DataFilterDefaultDays);
                    //    fromDate_default = fromDateSelected;
                    //}
                    //LoadTrangThaiCategory();
                    //LoadStatusCategory();
                    //LoadDueDateCategory();
                    SetDefaultFilter();

                    InvokeOnMainThread(() =>
                    {
                        if (isFilter)
                            LoadDataFilterFromServer(false);
                        else
                            LoadDataFilterTodo();

                        //if (isSearch)
                        //{
                        //    tf_search.Text = searchKeyword;
                        //    searchData();
                        //}
                    });
                });

                #region delegate
                CmmEvent.UpdateLangComplete += CmmEvent_UpdateLangComplete;
                todo_refreshControl.ValueChanged += Todo_refreshControl_ValueChanged;

                BT_moreUser.TouchUpInside += BT_moreUser_TouchUpInside;
                BT_avatar.TouchUpInside += BT_avatar_TouchUpInside;
                BT_follow.TouchUpInside += BT_follow_TouchUpInside;
                BT_comment.TouchUpInside += BT_comment_TouchUpInside;
                BT_share.TouchUpInside += BT_share_TouchUpInside;
                BT_history.TouchUpInside += BT_history_TouchUpInside;
                BT_attachement.TouchUpInside += BT_attachement_TouchUpInside;
                BT_search.TouchUpInside += BT_search_TouchUpInside;
                BT_filter.TouchUpInside += BT_filter_TouchUpInside;
                BT_DangXL.TouchUpInside += BT_DangXL_TouchUpInside;
                BT_DaXL.TouchUpInside += BT_DaXL_TouchUpInside;

                tf_search.EditingChanged += Tf_search_EditingChanged;
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine("TodoView - ViewDidLoad - Err: " + ex.ToString());
            }
        }

        #endregion

        #region public - private method
        public void SetContent(BeanAppBaseExt _appBaseSelected, UIViewController parent)
        {
            currentSelectedItem = _appBaseSelected;
        }
        public void SetContentFromSearch(int _statusIndex, int _duedateIndex, DateTime _fromdate, DateTime _todate, string _keyword)
        {
            fromDateSelected = _fromdate;
            toDateSelected = _todate;
            searchKeyword = _keyword;
        }
        public void ViewConfiguration()
        {
            BT_DangXL.Layer.ShadowOffset = new CGSize(1, 2);
            BT_DangXL.Layer.ShadowRadius = 4;
            BT_DangXL.Layer.ShadowColor = UIColor.Black.CGColor;
            BT_DangXL.Layer.ShadowOpacity = 0.5f;
            BT_DangXL.SetTitle(CmmFunction.GetTitle("TEXT_INPROCESS", "Đang xử lý"), UIControlState.Normal);

            BT_DaXL.Layer.ShadowOffset = new CGSize(-1, 2);
            BT_DaXL.Layer.ShadowRadius = 4;
            BT_DaXL.Layer.ShadowColor = UIColor.Black.CGColor;
            BT_DaXL.Layer.ShadowOpacity = 0.0f;
            BT_DaXL.SetTitle(CmmFunction.GetTitle("TEXT_PROCESSED", "Đã xử lý"), UIControlState.Normal);

            tf_search.Placeholder = CmmFunction.GetTitle("TEXT_SEARCH", "Tìm kiếm...");

            //buttonActionTopBar = ButtonsActionTopBar.Instance;
            //buttonActionTopBar.InitFrameView(view_top_bar.Frame);
            //view_top_bar.AddSubview(buttonActionTopBar);

            buttonActionBotBar = ButtonsActionBotBar.Instance;
            buttonActionBotBar.InitFrameView(view_bot_bar.Frame);
            buttonActionBotBar.LoadStatusButton(1);

            view_bot_bar.AddSubview(buttonActionBotBar);

            origin_view_header_content_height_constant = view_header_content_height_constant.Constant;
            origin_constraint_rightBT_Search = constraint_rightBT_Search.Constant;

            CmmIOSFunction.MakeCornerTopLeftRight(view_task_left, 8);
            CmmIOSFunction.MakeCornerTopLeftRight(view_task_right, 8);

            todo_refreshControl.TintColor = UIColor.FromRGB(65, 80, 134);
            todo_refreshControl.AttributedTitle = new NSAttributedString(CmmFunction.GetTitle("TEXT_REFRESH_LOADING", "Loading ..."), firstAttributes);
            table_todo.AddSubview(todo_refreshControl);

            CmmIOSFunction.CreateCircleButton(BT_avatar);
            string documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string localpath = Path.Combine(documentFolder, CmmVariable.M_Avatar);
            BT_avatar.ImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
            if (File.Exists(localpath))
                BT_avatar.SetImage(UIImage.FromFile(localpath), UIControlState.Normal);
            else
                BT_avatar.SetImage(UIImage.FromFile("Icons/icon_profile.png"), UIControlState.Normal);

            BT_search.ContentEdgeInsets = new UIEdgeInsets(6, 6, 6, 6);
            BT_filter.ContentEdgeInsets = new UIEdgeInsets(7, 14, 7, 0);
            BT_follow.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_comment.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_share.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_attachement.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_history.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);

            //tam khoa shadow
            //CmmIOSFunction.AddShadowForTopORBotBar(view_top, true);
            //CmmIOSFunction.AddShadowForTopORBotBar(view_bot_bar, false);
        }

        void SetDefaultFilter()
        {
            //set value default filter date = 30 days
            if (toDateSelected == default)
            {
                toDateSelected = DateTime.Now.Date;
                toDate_default = toDateSelected;
            }

            if (fromDateSelected == default)
            {
                fromDateSelected = toDateSelected.AddDays(-CmmVariable.M_DataFilterDefaultDays);
                fromDate_default = fromDateSelected;
            }

            LoadTrangThaiCategory();
            LoadStatusCategory();
            LoadDueDateCategory();
            //isFilter = false;
        }

        #region toMe
        private void LoadData_count(int countNum)
        {
            try
            {
                //if (tab_Dangxuly)
                //{
                BT_DangXL.SetAttributedTitle(new NSAttributedString(""), UIControlState.Normal); ;
                string str_dangxuly = CmmFunction.GetTitle("TEXT_INPROCESS", "Đang xử lý");
                dangxuly_count = countNum;//GetCountNumber();
                if (dangxuly_count >= 100)
                {
                    BT_DangXL.SetTitle(str_dangxuly + " (99+)", UIControlState.Normal);
                    //BT_DangXL.TitleLabel.Text = str_dangxuly + " (99+)";
                }
                else if (dangxuly_count > 9 && dangxuly_count < 100)
                {
                    BT_DangXL.SetTitle(str_dangxuly + " (" + dangxuly_count.ToString() + ")", UIControlState.Normal);
                    //BT_DangXL.TitleLabel.Text = str_dangxuly + " (" + dangxuly_count.ToString() + ")";
                }
                else if (dangxuly_count > 0 && dangxuly_count <= 9)
                {
                    BT_DangXL.SetTitle(str_dangxuly + " (0" + dangxuly_count.ToString() + ")", UIControlState.Normal);
                    //BT_DangXL.TitleLabel.Text = str_dangxuly + " (0" + dangxuly_count.ToString() + ")";
                }
                else
                {
                    BT_DangXL.SetTitle(str_dangxuly, UIControlState.Normal);
                    //BT_DangXL.TitleLabel.Text = str_dangxuly;
                }
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine("TodoDetailViews - loadData - Err: " + ex.ToString());
            }
        }

        int GetCountNumber()
        {
            int vcxl_count = 0;
            try
            {
                string count = "";

                count = new ProviderControlDynamic().GetListCountVDT_VTBD(CmmVariable.KEY_COUNT_ASIGNTOME_INPROCESS);

                if (!string.IsNullOrEmpty(count))
                {
                    var allCount = count.Split(";#");
                    if (allCount.Length > 0)
                    {
                        if (!int.TryParse(allCount[1], out vcxl_count))
                        {
                            vcxl_count = 0;
                        }
                    }
                }
                else
                {
                    vcxl_count = 0;
#if DEBUG
                    Console.WriteLine("GetCountNumber trả về chuỗi trống.");
#endif
                }
            }
            catch (Exception ex)
            {
                vcxl_count = 0;
#if DEBUG
                Console.WriteLine("GetCountNumber - Err: " + ex.ToString());
#endif
            }
            return vcxl_count;
        }

        public async void LoadDataFilterTodo(bool isTabChange = false)
        {
            if (lstViewLoading != null && lstViewLoading.IsDescendantOfView(view_task_left))
            {
                lstViewLoading.Hide();
            }
            lstViewLoading = new CmmLoading(new CGRect(view_task_left.Center.X - 100, view_task_left.Center.Y - 100, 200, 200), "Đang xử lý...");
            view_task_left.Add(lstViewLoading);
            isOnline = Reachability.detectNetWork();

            //dict_todo = new Dictionary<string, List<BeanAppBaseExt>>();
            dict_sectionTodo = new Dictionary<string, bool>();
            lst_notify_cxl = new List<BeanNotify>();
            try
            {
                ShowObjContent();
                isFilter = false;
                if (isTabChange) SetDefaultFilter();

                if (isFilter)
                    BT_filter.TintColor = UIColor.Orange;
                else
                    BT_filter.TintColor = UIColor.Black;

                _ = Task.Run(() =>
                {
                    if (isOnline)
                        LoadDataToDoOnLine(isTabChange);
                    else
                        lst_appBase_cxl = LoadDataToDoOffLine();

                    InvokeOnMainThread(() =>
                    {
                        if (lst_appBase_cxl != null && lst_appBase_cxl.Count > 0)
                        {
                            if (!isFilter) LoadData_count(GetCountNumber());
                            if (tab_Dangxuly)
                            {
                                ////string str_toMe = CmmFunction.GetTitle("TEXT_IPAD_TOME", "Trang chủ  >  Đến tôi");
                                //string str_dangxuly = CmmFunction.GetTitle("TEXT_INPROCESS", "Đang xử lý");
                                //if (dangxuly_count >= 100)
                                //    BT_DangXL.SetTitle(str_dangxuly + " (99+)", UIControlState.Normal);
                                //else if (dangxuly_count > 0 && dangxuly_count < 100)
                                //{
                                //    str_dangxuly = str_dangxuly + " (" + dangxuly_count.ToString() + ")";
                                //    BT_DangXL.SetTitle(str_dangxuly, UIControlState.Normal);
                                //}
                                //else
                                //    BT_DangXL.SetTitle(str_dangxuly, UIControlState.Normal);

                                ButtonMenuStyleChange(BT_DangXL, true, 0);
                                ButtonMenuStyleChange(BT_DaXL, false, 1);
                            }
                            else
                            {
                                ButtonMenuStyleChange(BT_DangXL, false, 0);
                                ButtonMenuStyleChange(BT_DaXL, true, 1);
                            }

                            //currentSelectedItem = lst_appBase_cxl[0];

                            table_todo.Alpha = 1;
                            lbl_nodata_left.Hidden = true;

                            view_task_right.Alpha = 1;
                            lbl_nodata.Hidden = true;

                            LoadDataNotifyTypeAppBase(lst_appBase_cxl, false);

                            if (isTabChange || IsFirstLoad)
                            {
                                table_todo.ScrollRectToVisible(new CGRect(0, 0, 0, 0), false);
                            }

                            if (!string.IsNullOrEmpty(searchKeyword))
                            {
                                tf_search.Text = searchKeyword;
                                searchData(true);
                            }

                            if (currentSelectedItem != null && currentSelectedItem.ResourceCategoryId == (int)CmmFunction.CommentResourceCategoryID.Task)
                                isShowTask = true;

                            LoadItemSelected(true);
                        }
                        else
                        {
                            table_todo.Hidden = true;
                            lbl_nodata_left.Hidden = false;
                            view_task_right.Hidden = true;
                            lbl_nodata.Hidden = false;
                            LoadData_count(0);

                            if (tab_Dangxuly)
                            {
                                BT_DangXL.SetTitle(CmmFunction.GetTitle("TEXT_INPROCESS", "Đang xử lý"), UIControlState.Normal);
                                //BT_DangXL.SetAttributedTitle(new NSMutableAttributedString(CmmFunction.GetTitle("TEXT_INPROCESS", "Đang xử lý")), UIControlState.Normal);
                            }
                        }

                        lstViewLoading?.Hide();
                    });
                });
            }
            catch (Exception ex)
            {
                lstViewLoading?.Hide();
                lbl_topBar_title.Text = CmmFunction.GetTitle("TEXT_IPAD_TOME", "Trang chủ  >  Đến tôi");
                table_todo.Source = null;
                table_todo.ReloadData();
                Console.WriteLine("TodoDetailsView - LoadDataFilterTodo - Err: " + ex.ToString());
            }
            finally
            {
                //lstViewLoading?.Hide();
            }
        }

        void LoadDataToDoOnLine(bool isTabChange = false)
        {
            List<BeanAppBaseExt> lstObj = new List<BeanAppBaseExt>();
            try
            {
                lstObj = new ProviderBase().LoadMoreDataTFromSerVer(tab_Dangxuly ? CmmVariable.KEY_GET_TOME_INPROCESS : CmmVariable.KEY_GET_TOME_PROCESSED, 20, isLoadMore && !isTabChange ? lst_appBase_cxl.Count : 0);

                if (lstObj != null && lstObj.Count > 0)
                {
                    CmmIOSFunction.UpdateNewListDataOnline(lstObj, new SQLiteConnection(CmmVariable.M_DataPath));
                    if (isLoadMore && !isTabChange)
                    {
                        if (lst_appBase_cxl == null) lst_appBase_cxl = new List<BeanAppBaseExt>();
                        lst_appBase_cxl.AddRange(lstObj);
                    }
                    else
                        lst_appBase_cxl = lstObj;

                    //Check DueDate
                    if (DuedateSelected != null)
                        switch (DuedateSelected.ID)
                        {
                            case 2: // Trong ngày
                                lst_appBase_cxl = lst_appBase_cxl.FindAll(o => o.DueDate.HasValue && o.DueDate.Value.Date == DateTime.Now.Date);
                                break;
                            case 3: // Trễ hạn
                                lst_appBase_cxl = lst_appBase_cxl.FindAll(o => o.DueDate.HasValue && o.DueDate.Value.Date < DateTime.Now.Date);
                                break;
                            default:// Tất cả
                                break;
                        }

                    //Check Filter Status
                    if (lst_appStatus != null)
                    {
                        lst_appStatus_selected = lst_appStatus.Where(s => s.IsSelected == true).ToList();
                        if (lst_appStatus_selected != null && lst_appStatus_selected.Count > 0)
                            lst_appBase_cxl = lst_appBase_cxl.FindAll(o => o.StatusGroup.HasValue && lst_appStatus_selected.FindIndex(s => s.ID == o.StatusGroup.Value) > -1);
                    }
                }
                else
                {
                    isLoadMore = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetListObj - Err: " + ex.ToString());
            }
        }

        List<BeanAppBaseExt> LoadDataToDoOffLine()
        {
            var conn = new SQLiteConnection(CmmVariable.M_DataPath);
            string count_query_vcxl = string.Empty;
            string query = string.Empty;
            var res = new List<BeanAppBaseExt>();

            //Check filter DueDate
            if (lst_dueDateMenu != null)
                DuedateSelected = lst_dueDateMenu.Where(s => s.isSelected == true).FirstOrDefault();

            string duedate_condition = "";
            if (DuedateSelected == null || DuedateSelected.ID == 1) // Tat ca
                duedate_condition = "";
            else if (DuedateSelected.ID == 2) // Trong ngay
                duedate_condition = @"AND (AB.DueDate IS NOT NULL AND date(AB.DueDate) = date('now'))";
            else if (DuedateSelected.ID == 3) // Tre han
                duedate_condition = @"AND (AB.DueDate IS NOT NULL AND AB.DueDate < date('now'))";

            //Check Filter Status
            if (lst_appStatus != null)
                lst_appStatus_selected = lst_appStatus.Where(s => s.IsSelected == true).ToList();

            //Check switch status
            //dang xu ly
            if (tab_Dangxuly)
            {
                query_dangxuly = "NOTI.Type = 1 AND NOTI.Status = 0";
                if (isFilter)
                {
                    if (lst_appStatus_selected != null && (lst_appStatus_selected.Count > 0 && lst_appStatus_selected.Count < lst_appStatus.Count))
                    {
                        string str_status = string.Join(',', lst_appStatus_selected.Select(i => i.ID));

                        query = string.Format(@"SELECT AB.*, NOTI.StartDate, NOTI.Read, NOTI.SPItemId, NOTI.SubmitAction,NOTI.SubmitActionEN, NOTI.SendUnit
                                                FROM BeanAppBase AB
                                                INNER JOIN BeanNotify NOTI ON AB.ID = NOTI.SPItemId
                                                WHERE {0} AB.AssignedTo LIKE '%{1}%' {2} 
                                                AND AB.StatusGroup IN ({3}) {4}
                                                Order By AB.Created DESC LIMIT ? OFFSET ?",
                         string.IsNullOrEmpty(query_dangxuly) ? "" : query_dangxuly + " AND", CmmVariable.SysConfig.UserId.ToUpper(),
                         string.IsNullOrEmpty(date_filter) ? "" : "AND " + date_filter,
                         str_status,
                         duedate_condition);
                    }

                }
                // default - dang xu ly
                else
                {
                    /*string defaultStatus_dangxuly = CmmFunction.GetAppSettingValue("MOBILE_APPSTATUS_TOME_DANGXULY");
                    //cap nhat count VCXL
                    string query_countvcxl = string.Format(@"SELECT Count(*) as count
                                                FROM BeanAppBase AB
                                                INNER JOIN BeanNotify NOTI
                                                    ON AB.ID = NOTI.SPItemId
                                                WHERE (AB.AssignedTo LIKE '%{0}%' OR AB.NotifiedUsers LIKE '%{0}%')
                                                AND AB.StatusGroup IN ({1}) AND NOTI.Type = 1 AND NOTI.Status = 0", CmmVariable.SysConfig.UserId.ToUpper(), defaultStatus_dangxuly);
                    */
                    string defaultStatus_dangxuly = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS_TOME_DANGXULY); // tat ca
                    if (!string.IsNullOrEmpty(defaultStatus_dangxuly))
                        defaultStatus_dangxuly = string.Format(@" AND AB.StatusGroup IN({0})", defaultStatus_dangxuly);

                    //cap nhat count VCXL
                    string query_countvcxl = string.Format("SELECT Count(*) as count " +
                        "FROM BeanAppBase AB " +
                        "INNER JOIN BeanNotify NOTI ON AB.ID = NOTI.SPItemId " +
                        "WHERE NOTI.Type = 1 AND NOTI.Status = 0 AND " +
                        "(AB.AssignedTo LIKE '%{0}%' OR AB.NotifiedUsers LIKE '%{0}%') {1} ", CmmVariable.SysConfig.UserId.ToUpper(), defaultStatus_dangxuly);

                    List<CountNum> countnum_dangxuly = conn.Query<CountNum>(query_countvcxl);
                    if (countnum_dangxuly != null)
                        dangxuly_count = countnum_dangxuly[0].count;
                    #region old query
                    /*
                    query = string.Format(@"SELECT AB.*, NOTI.StartDate, NOTI.Read, NOTI.SPItemId, NOTI.SubmitAction,NOTI.SubmitActionEN, NOTI.SendUnit  " +
                        "FROM BeanAppBase AB " +
                        "INNER JOIN BeanNotify NOTI " +
                        "ON AB.ID = NOTI.SPItemId " +
                        "WHERE {0} AB.StatusGroup IN ({4}) " +
                        "AND (AB.AssignedTo LIKE '%{1}%' OR AB.NotifiedUsers LIKE '%{1}%') {2} {3} " +
                        "ORDER BY NOTI.Created DESC LIMIT ? OFFSET ?",
                     string.IsNullOrEmpty(query_dangxuly) ? "" : query_dangxuly + " AND", CmmVariable.SysConfig.UserId.ToUpper(), (string.IsNullOrEmpty(date_filter) ? "" : "AND " + date_filter), duedate_condition, defaultStatus_dangxuly);
                    */
                    #endregion
                    query = string.Format(@"SELECT AB.*, NOTI.StartDate, NOTI.Read, NOTI.SPItemId, NOTI.SubmitAction, NOTI.SubmitActionEN, NOTI.SendUnit " +
                        "FROM BeanAppBase AB " +
                        "INNER JOIN BeanNotify NOTI ON AB.ID = NOTI.SPItemId " +
                        "WHERE NOTI.Type = 1 AND NOTI.Status = 0 AND " +
                        "(AB.AssignedTo LIKE '%{0}%' OR AB.NotifiedUsers LIKE '%{0}%') {1} " +
                        "Order By NOTI.Created DESC LIMIT ? OFFSET ?", CmmVariable.SysConfig.UserId.ToUpper(), defaultStatus_dangxuly);
                }
            }
            //da xu ly
            else
            {
                query_dangxuly = "";
                if (lst_appStatus_selected != null && (lst_appStatus_selected.Count > 0 && lst_appStatus_selected.Count < lst_appStatus.Count))
                {
                    string str_status = string.Join(',', lst_appStatus_selected.Select(i => i.ID));

                    query = string.Format(@"SELECT * FROM BeanAppBase AB
                                                WHERE AB.StatusGroup IN ({3}) AND AB.NotifiedUsers LIKE '%{0}%'
                                                AND (AB.ResourceCategoryId <> 16 OR AB.CreatedBy <> '%{0}%') {1} {2} 
                                            ORDER BY AB.Created DESC LIMIT ? OFFSET ?", CmmVariable.SysConfig.UserId.ToLower(), string.IsNullOrEmpty(date_filter) ? "" : "AND " + date_filter, duedate_condition, str_status);
                }
                // default - da xu ly
                else
                {
                    string defaultStatus_daxuly = CmmFunction.GetAppSettingValue("MOBILE_APPSTATUS_TOME_DAXULY");

                    query = string.Format(@"SELECT * FROM BeanAppBase AB
                                                WHERE AB.StatusGroup IN ({3}) AND AB.NotifiedUsers LIKE '%{0}%'
                                                AND (AB.ResourceCategoryId <> 16 OR AB.CreatedBy <> '%{0}%') {1} {2} 
                                            ORDER BY AB.Created DESC LIMIT ? OFFSET ?", CmmVariable.SysConfig.UserId.ToLower(), string.IsNullOrEmpty(date_filter) ? "" : "AND " + date_filter, duedate_condition, defaultStatus_daxuly);
                }
            }
            try
            {
                res = conn.Query<BeanAppBaseExt>(query, limit, offset);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ToDoDetailViews - LoadDataToDoOffLine - Err: " + ex.ToString());
                res = new List<BeanAppBaseExt>();
            }
            finally
            {
                conn.Close();
            }
            return res;
        }

        async void LoadDataFilterTodo_loadmore(int _limit, int _offset)
        {
            try
            {
                _ = Task.Run(() =>
                {
                    if (isOnline)
                    {
                        LoadDataToDoOnLine();
                    }
                    else
                        LoadDataMoreOffline(_limit, _offset);
                    if (lst_appBase_cxl != null && lst_appBase_cxl.Count > 0)
                    {
                        dict_todo = new Dictionary<string, List<BeanAppBaseExt>>();
                        dict_sectionTodo = new Dictionary<string, bool>();
                        string KEY_TODAY = CmmFunction.GetTitle("TEXT_TODAY", "Hôm nay");
                        string KEY_YESTERDAY = CmmFunction.GetTitle("TEXT_YESTERDAY", "Hôm qua");
                        string KEY_ORTHER = CmmFunction.GetTitle("TEXT_OLDER", "Cũ hơn");

                        List<BeanAppBaseExt> lst_temp1 = new List<BeanAppBaseExt>();
                        List<BeanAppBaseExt> lst_temp2 = new List<BeanAppBaseExt>();
                        List<BeanAppBaseExt> lst_temp3 = new List<BeanAppBaseExt>();
                        dict_todo.Add(KEY_TODAY, lst_temp1);
                        dict_todo.Add(KEY_YESTERDAY, lst_temp2);
                        dict_todo.Add(KEY_ORTHER, lst_temp3);

                        foreach (var item in lst_appBase_cxl)
                        {
                            if (item.StartDate.HasValue)
                            {
                                if (item.StartDate.Value.Date == DateTime.Now.Date) // today
                                {
                                    List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                                    if (dict_todo.ContainsKey(KEY_TODAY))
                                        dict_todo[KEY_TODAY].Add(item);
                                    else
                                        dict_todo.Add(KEY_TODAY, lst_temp);
                                }
                                else if (item.StartDate.Value.Date == DateTime.Now.AddDays(-1).Date) // hom qua
                                {
                                    List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                                    if (dict_todo.ContainsKey(KEY_YESTERDAY))
                                        dict_todo[KEY_YESTERDAY].Add(item);
                                    else
                                        dict_todo.Add(KEY_YESTERDAY, lst_temp);
                                }
                                else if (item.StartDate.Value.Date < DateTime.Now.AddDays(-1).Date) // cuhon
                                {
                                    List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                                    if (dict_todo.ContainsKey(KEY_ORTHER))
                                        dict_todo[KEY_ORTHER].Add(item);
                                    else
                                        dict_todo.Add(KEY_ORTHER, lst_temp);
                                }
                            }
                        }

                        InvokeOnMainThread(() =>
                        {
                            table_todo.Source = new ToDo_TableSource(dict_todo, this);
                            table_todo.ReloadData();
                        });
                    }
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine("MainView - LoadDataFilterTodo - Err: " + ex.ToString());
            }
        }

        void LoadDataMoreOffline(int _limit, int _offset)
        {

            List<BeanAppBaseExt> lst_todo_more = new List<BeanAppBaseExt>();
            var conn = new SQLiteConnection(CmmVariable.M_DataPath);
            string count_query_vcxl = string.Empty;
            string query = string.Empty;

            dict_todo = new Dictionary<string, List<BeanAppBaseExt>>();
            dict_sectionTodo = new Dictionary<string, bool>();
            string defaultStatus_dangxuly = CmmFunction.GetAppSettingValue("MOBILE_APPSTATUS_TOME_DANGXULY");

            query = string.Format(@"SELECT AB.*, NOTI.StartDate, NOTI.Read, NOTI.SPItemId, NOTI.SubmitAction, NOTI.SubmitActionEN, NOTI.SendUnit
                                                    FROM BeanAppBase AB
                                                    INNER JOIN BeanNotify NOTI
                                                        ON AB.ID = NOTI.SPItemId
                                                    WHERE AB.AssignedTo LIKE '%{0}%' AND AB.StatusGroup IN ({1}) AND NOTI.Type = 1 AND NOTI.Status = 0
                                                    Order By NOTI.Created DESC LIMIT ? OFFSET ?", CmmVariable.SysConfig.UserId.ToUpper(), defaultStatus_dangxuly);

            lst_todo_more = conn.Query<BeanAppBaseExt>(query, _limit, _offset);

            if (lst_todo_more != null && lst_todo_more.Count > 0)
            {
                lst_appBase_cxl.AddRange(lst_todo_more);
            }
        }

        public void LoadDataFilterFromServer(bool isLoadmore)
        {
            if (!isLoadmore)
            {
                if (lstViewLoading != null && lstViewLoading.IsDescendantOfView(view_task_left))
                {
                    lstViewLoading.Hide();
                }
                lstViewLoading = new CmmLoading(new CGRect(view_task_left.Center.X - 100, view_task_left.Center.Y - 100, 200, 200), "Đang xử lý...");
                view_task_left.Add(lstViewLoading);
            }

            try
            {
                if (isFilter)
                    BT_filter.TintColor = UIColor.Orange;
                else
                    BT_filter.TintColor = UIColor.Black;

                dict_todo = new Dictionary<string, List<BeanAppBaseExt>>();

                if (lst_appBase_cxl == null)
                    lst_appBase_cxl = new List<BeanAppBaseExt>();

                string str_status = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS); // tat ca fromme

                //Check Filter Status
                if (lst_appStatus != null)
                    lst_appStatus_selected = lst_appStatus.Where(s => s.IsSelected == true).ToList();

                if (lst_appStatus_selected != null && lst_appStatus_selected.Count > 0 && lst_appStatus_selected.Count < lst_appStatus.Count) // co chon status
                    str_status = string.Join(',', lst_appStatus_selected.Select(i => i.ID));

                //Check filter DueDate
                if (lst_dueDateMenu != null)
                    DuedateSelected = lst_dueDateMenu.Where(s => s.isSelected == true).FirstOrDefault();

                ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();
                int totalRecord = 0;
                var listPropertiesFilter = CmmFunction.BuildListPropertiesFilter(CmmVariable.M_ResourceViewID_ToMe, tab_Dangxuly ? "2" : "4", str_status, DuedateSelected.ID, fromDateSelected, toDateSelected);

                //loadmore then cu filter
                if (isLoadmore)
                {
                    List<BeanAppBaseExt> lst_appBase_cxl_more = new List<BeanAppBaseExt>();
                    lst_appBase_cxl_more = _pControlDynamic.GetListFilterMyTask(listPropertiesFilter, ref totalRecord, CmmVariable.M_DataFilterAPILimitData, lst_appBase_cxl.Count);// lst_appBase_fromMe.Count
                    lst_appBase_cxl_more = lst_appBase_cxl_more.OrderByDescending(s => s.NotifyCreated).ToList();

                    if (lst_appBase_cxl_more != null && lst_appBase_cxl_more.Count > 0)
                    {
                        lst_appBase_cxl.AddRange(lst_appBase_cxl_more);
                    }
                    else
                        return;
                }
                // Lan dau filter
                else
                {
                    lst_appBase_cxl = _pControlDynamic.GetListFilterMyTask(listPropertiesFilter, ref totalRecord, CmmVariable.M_DataFilterAPILimitData, 0);// lst_appBase_fromMe.Count

                    if (tab_Dangxuly)
                    {
                        lst_appBase_cxl = lst_appBase_cxl.OrderByDescending(s => s.NotifyCreated).ToList();
                        dangxuly_count = totalRecord;
                    }
                    else
                        lst_appBase_cxl = lst_appBase_cxl.OrderByDescending(s => s.Modified).ToList();
                }

                if (lst_appBase_cxl != null && lst_appBase_cxl.Count > 0)
                {
                    if (tab_Dangxuly)
                    {
                        ////string str_toMe = CmmFunction.GetTitle("TEXT_IPAD_TOME", "Trang chủ  >  Đến tôi");
                        //string str_dangxuly = CmmFunction.GetTitle("TEXT_INPROCESS", "Đang xử lý");
                        //if (dangxuly_count >= 100)
                        //    BT_DangXL.TitleLabel.Text = str_dangxuly + " (99+)";
                        //else if (dangxuly_count > 0 && dangxuly_count < 100)
                        //{
                        //    str_dangxuly = str_dangxuly + " (" + dangxuly_count.ToString() + ")";
                        //    BT_DangXL.SetTitle(str_dangxuly, UIControlState.Normal);
                        //}
                        //else
                        //    BT_DangXL.SetTitle(str_dangxuly, UIControlState.Normal);
                        LoadData_count(lst_appBase_cxl.Count);
                        ButtonMenuStyleChange(BT_DangXL, true, 0);
                        ButtonMenuStyleChange(BT_DaXL, false, 1);
                    }
                    else
                    {
                        //tab_Dangxuly = false;
                        BT_DangXL.BackgroundColor = UIColor.Clear;
                        BT_DangXL.SetTitleColor(UIColor.White, UIControlState.Normal);
                        BT_DaXL.BackgroundColor = UIColor.White;
                        BT_DaXL.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);

                        LoadData_count(0);
                        ButtonMenuStyleChange(BT_DangXL, false, 0);
                        ButtonMenuStyleChange(BT_DaXL, true, 1);
                    }

                    table_todo.Alpha = 1;
                    lbl_nodata_left.Hidden = true;
                    view_task_right.Alpha = 1;
                    lbl_nodata.Hidden = true;

                    if (!string.IsNullOrEmpty(searchKeyword))
                    {
                        tf_search.Text = searchKeyword;
                        searchData(true);
                    }
                    else
                    {
                        LoadDataNotifyTypeAppBase(lst_appBase_cxl, true);

                    }
                }
                else
                {
                    if (tab_Dangxuly)
                    {
                        string str_transalte = CmmFunction.GetTitle("TEXT_INPROCESS", BT_DangXL.TitleLabel.Text);
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(0, str_transalte.Length));
                        BT_DangXL.SetAttributedTitle(att, UIControlState.Normal);
                    }
                    LoadData_count(0);
                    ButtonMenuStyleChange(BT_DangXL, tab_Dangxuly, 0);
                    ButtonMenuStyleChange(BT_DaXL, !tab_Dangxuly, 1);

                    //hide table danh sach left
                    table_todo.Alpha = 0;
                    lbl_nodata_left.Hidden = false;

                    //hide table chi tiet form
                    view_task_right.Alpha = 0;
                    lbl_nodata.Hidden = false;
                }
            }
            catch (Exception ex)
            {
                dangxuly_count = 0;
                table_todo.Source = null;
                table_todo.ReloadData();

                table_todo.Alpha = 0;
                lbl_nodata.Hidden = false;

                Console.WriteLine("ToDoDetailView - loadDataFilterFromServer - Err: " + ex.ToString());
            }
            finally
            {
                if (!isLoadmore) lstViewLoading?.Hide();
            }
        }

        public async void LoadmoreData()
        {
            view_task_left_loadmore.Hidden = false;
            loadmore_indicator.StartAnimating();

            await Task.Run(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5f));
                InvokeOnMainThread(() =>
                {
                    if (isFilter)
                        LoadDataFilterFromServer(true);
                    else
                        LoadDataFilterTodo_loadmore(limit, lst_appBase_cxl.Count);

                    loadmore_indicator.StopAnimating();
                    view_task_left_loadmore.Hidden = true;
                });
            });
        }

        private void LoadDataNotifyTypeAppBase(List<BeanAppBaseExt> lst_appBase_cxl, bool isOnline)
        {
            dict_todo = new Dictionary<string, List<BeanAppBaseExt>>();
            dict_sectionTodo = new Dictionary<string, bool>();
            /*
            if (currentSelectedItem != null)
                currentSelectedItem.IsSelected = true;
            else
                currentSelectedItem = lst_appBase_cxl[0];
            */

            string KEY_TODAY = CmmFunction.GetTitle("TEXT_TODAY", "Hôm nay");
            string KEY_YESTERDAY = CmmFunction.GetTitle("TEXT_YESTERDAY", "Hôm qua");
            string KEY_ORTHER = CmmFunction.GetTitle("TEXT_OLDER", "Cũ hơn");

            List<BeanAppBaseExt> lst_temp1 = new List<BeanAppBaseExt>();
            List<BeanAppBaseExt> lst_temp2 = new List<BeanAppBaseExt>();
            List<BeanAppBaseExt> lst_temp3 = new List<BeanAppBaseExt>();
            dict_todo.Add(KEY_TODAY, lst_temp1);
            dict_todo.Add(KEY_YESTERDAY, lst_temp2);
            dict_todo.Add(KEY_ORTHER, lst_temp3);

            foreach (var item in lst_appBase_cxl)
            {
                // Dang Xu Ly
                if (tab_Dangxuly)
                {
                    if (isOnline) // server sort theo NotifyCreated
                    {
                        if (item.NotifyCreated.HasValue)
                        {
                            if (item.NotifyCreated.Value.Date == DateTime.Now.Date) // today
                            {
                                List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                                if (dict_todo.ContainsKey(KEY_TODAY))
                                    dict_todo[KEY_TODAY].Add(item);
                                else
                                    dict_todo.Add(KEY_TODAY, lst_temp);
                            }
                            else if (item.NotifyCreated.Value.Date == DateTime.Now.AddDays(-1).Date) // hom qua
                            {

                                List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                                if (dict_todo.ContainsKey(KEY_YESTERDAY))
                                    dict_todo[KEY_YESTERDAY].Add(item);
                                else
                                    dict_todo.Add(KEY_YESTERDAY, lst_temp);
                            }
                            else if (item.NotifyCreated.Value.Date < DateTime.Now.AddDays(-1).Date) // cuhon
                            {
                                List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                                if (dict_todo.ContainsKey(KEY_ORTHER))
                                    dict_todo[KEY_ORTHER].Add(item);
                                else
                                    dict_todo.Add(KEY_ORTHER, lst_temp);
                            }
                        }
                    }
                    //local sort theo StartDate
                    else
                    {
                        if (item.StartDate.HasValue)
                        {
                            if (item.StartDate.Value.Date == DateTime.Now.Date) // today
                            {
                                List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                                if (dict_todo.ContainsKey(KEY_TODAY))
                                    dict_todo[KEY_TODAY].Add(item);
                                else
                                    dict_todo.Add(KEY_TODAY, lst_temp);
                            }
                            else if (item.StartDate.Value.Date == DateTime.Now.AddDays(-1).Date) // hom qua
                            {

                                List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                                if (dict_todo.ContainsKey(KEY_YESTERDAY))
                                    dict_todo[KEY_YESTERDAY].Add(item);
                                else
                                    dict_todo.Add(KEY_YESTERDAY, lst_temp);
                            }
                            else if (item.StartDate.Value.Date < DateTime.Now.AddDays(-1).Date) // cuhon
                            {
                                List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                                if (dict_todo.ContainsKey(KEY_ORTHER))
                                    dict_todo[KEY_ORTHER].Add(item);
                                else
                                    dict_todo.Add(KEY_ORTHER, lst_temp);
                            }
                        }
                    }
                }
                // Da Xu Ly
                // server hay local thi cung sort theo Modified
                else
                {
                    if (item.Modified.HasValue)
                    {
                        if (item.Modified.Value.Date == DateTime.Now.Date) // today
                        {
                            List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                            if (dict_todo.ContainsKey(KEY_TODAY))
                                dict_todo[KEY_TODAY].Add(item);
                            else
                                dict_todo.Add(KEY_TODAY, lst_temp);
                        }
                        else if (item.Modified.Value.Date == DateTime.Now.AddDays(-1).Date) // hom qua
                        {
                            List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                            if (dict_todo.ContainsKey(KEY_YESTERDAY))
                                dict_todo[KEY_YESTERDAY].Add(item);
                            else
                                dict_todo.Add(KEY_YESTERDAY, lst_temp);
                        }
                        else if (item.Modified.Value.Date < DateTime.Now.AddDays(-1).Date) // cuhon
                        {
                            List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                            if (dict_todo.ContainsKey(KEY_ORTHER))
                                dict_todo[KEY_ORTHER].Add(item);
                            else
                                dict_todo.Add(KEY_ORTHER, lst_temp);
                        }
                    }
                }

                //if (item.ID == currentSelectedItem.ID)
                //    item.IsSelected = true;
                //else
                //    item.IsSelected = false;
            }

            if (currentSelectedItem == null)
            {
                //currentSelectedItem = lst_appBase_cxl[0];
                if (dict_todo[KEY_TODAY].Count > 0)
                {
                    dict_todo[KEY_TODAY][0].IsSelected = true;
                    currentSelectedItem = dict_todo[KEY_TODAY][0];
                }
                else if (dict_todo[KEY_YESTERDAY].Count > 0)
                {
                    dict_todo[KEY_YESTERDAY][0].IsSelected = true;
                    currentSelectedItem = dict_todo[KEY_YESTERDAY][0];
                }
                else if (dict_todo[KEY_ORTHER].Count > 0)
                {
                    dict_todo[KEY_ORTHER][0].IsSelected = true;
                    currentSelectedItem = dict_todo[KEY_ORTHER][0];
                }
                currentSelectedItem.IsSelected = true;
            }
            lst_appBase_cxl.ForEach(o => o.IsSelected = o.ID == currentSelectedItem.ID);
            table_todo.Hidden = false;
            lbl_nodata_left.Hidden = true;

            table_todo.Source = new ToDo_TableSource(dict_todo, this);
            table_todo.ReloadData();
        }

        void SetSelectedItem(List<BeanAppBaseExt> _lst_appBase_cxl, bool isOnline)
        {
            // Dang Xu Ly
            if (tab_Dangxuly)
            {
                if (isOnline) // server sort theo NotifyCreated
                {
                    lst_appBase_cxl.OrderByDescending(o => o.NotifyCreated.Value);
                }
                //local sort theo StartDate
                else
                {
                    lst_appBase_cxl.OrderByDescending(o => o.StartDate.Value);
                }
            }
            // Da Xu Ly
            // server hay local thi cung sort theo Modified
            else
            {
                lst_appBase_cxl.OrderByDescending(o => o.Modified.Value);
            }

            if (currentSelectedItem == null)
                currentSelectedItem = lst_appBase_cxl[0];
            currentSelectedItem.IsSelected = true;
        }

        /// <summary>
        /// Set màu cho trang đang được focus
        /// </summary>
        void SetLangTitle()
        {
            lbl_nodata_left.Text = string.Compare(CmmVariable.SysConfig.LangCode, "1033") == 0 ? "No data yet" : "Không có dữ liệu";
            lbl_nodata.Text = lbl_nodata_left.Text;

            lbl_topBar_title.Text = CmmFunction.GetTitle("TEXT_IPAD_TOME", "Trang chủ  >  Đến tôi");
            var str_transalte = lbl_topBar_title.Text;
            NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
            att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(94, 94, 94), new NSRange(0, str_transalte.Length));
            //den toi
            var indexA = str_transalte.IndexOf('/');
            if (indexA != -1)
                att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 104, 133), new NSRange(indexA + 1, str_transalte.Length - indexA + 1));
            else
            {
                indexA = str_transalte.IndexOf('>');
                if (indexA != -1)
                    att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 104, 133), new NSRange(indexA + 2, str_transalte.Length - indexA - 2));
            }
            lbl_topBar_title.AttributedText = att;
        }

        private void LoadTrangThaiCategory()
        {
            lst_trangthai = new List<ClassMenu>();
            ClassMenu m1 = new ClassMenu() { ID = 1, section = 0, title = CmmFunction.GetTitle("TEXT_INPROCESS", "Đang xử lý"), isSelected = false };
            ClassMenu m2 = new ClassMenu() { ID = 2, section = 0, title = CmmFunction.GetTitle("TEXT_PROCESSED", "Đã xử lý"), isSelected = false };

            lst_trangthai.AddRange(new[] { m1, m2 });
        }

        private void LoadStatusCategory()
        {
            if (lst_appStatus == null || lst_appStatus.Count == 0)
            {
                var condition = CmmFunction.GetAppSettingValue("MOBILE_APPSTATUS_ENABLE");
                var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                string query_worflowCategory = string.Format(@"SELECT * FROM BeanAppStatus WHERE ID IN ({0})", condition);
                lst_appStatus = conn.Query<BeanAppStatus>(query_worflowCategory);
            }
            if (!isFilter)
            {
                var defaultStatus = CmmFunction.GetAppSettingValue(tab_Dangxuly ? CmmVariable.APPSTATUS_TOME_DANGXULY : CmmVariable.APPSTATUS_TOME_DAXULY);
                var defaultSelected = defaultStatus.Split(",");
                if (defaultSelected != null && defaultSelected.Length > 0)
                    lst_appStatus.ForEach(o =>
                    {
                        //if (defaultSelected != null && defaultSelected.Length > 0 && defaultSelected.Contains(o.ID.ToString()))
                        //{
                        //    o.IsSelected = true;
                        //}
                        //else
                        o.IsSelected = defaultSelected.Any(s => string.Compare(s, o.ID.ToString()) == 0);//defaultSelected.Contains()
                    });
            }
        }

        private void LoadDueDateCategory()
        {
            if (lst_dueDateMenu == null || lst_dueDateMenu.Count == 0)
            {
                lst_dueDateMenu = new List<ClassMenu>();
                ClassMenu m1 = new ClassMenu() { ID = 1, section = 0, title = CmmFunction.GetTitle("TEXT_ALL", "Tất cả") };
                ClassMenu m2 = new ClassMenu() { ID = 2, section = 0, title = CmmFunction.GetTitle("TEXT_TODAY1", "Trong ngày") };
                ClassMenu m3 = new ClassMenu() { ID = 3, section = 0, title = CmmFunction.GetTitle("TEXT_OVERDUE", "Trễ hạn") };

                lst_dueDateMenu.AddRange(new[] { m1, m2, m3 });
            }

            if (!isFilter)
            {
                DuedateSelected = null;
            }
        }

        public void ReLoadTableListTodo()
        {
            table_todo.ReloadData();
        }
        /*
        private void SwichProgressStatus()
        {
            if (tab_Dangxuly) // dang la trang thai todo cua toi
            {
                tab_Dangxuly = true;
                BT_DangXL.BackgroundColor = UIColor.White;
                BT_DangXL.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);
                BT_DaXL.BackgroundColor = UIColor.Clear;
                BT_DaXL.SetTitleColor(UIColor.White, UIControlState.Normal);

                ButtonMenuStyleChange(BT_DangXL, true, 0);
                ButtonMenuStyleChange(BT_DaXL, false, 1);
            }
            else // dang la trang thai tu toi
            {
                tab_Dangxuly = false;
                BT_DangXL.BackgroundColor = UIColor.Clear;
                BT_DangXL.SetTitleColor(UIColor.White, UIControlState.Normal);
                BT_DaXL.BackgroundColor = UIColor.White;
                BT_DaXL.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);

                ButtonMenuStyleChange(BT_DangXL, false, 0);
                ButtonMenuStyleChange(BT_DaXL, true, 1);
            }

        }
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_button"></param>
        /// <param name="isSelected"></param>
        /// <param name="_index"> 0: đang xử lý; 1: đã xử lý</param>
        private void ButtonMenuStyleChange(UIButton _button, bool isSelected, int _index)
        {
            string str_transalte = "";
            if (!isSelected)
            {
                if (_index == 0)
                {
                    str_transalte = BT_DangXL.TitleLabel.Text;
                    if (BT_DangXL.TitleLabel.Text.Contains("("))
                    {
                        //str_transalte = BT_DangXL.TitleLabel.Text;
                        //str_transalte = 
                        //if (!str_transalte.Contains("("))
                        //    str_transalte = str_transalte + " (" + dangxuly_count + ")";
                        var lst = str_transalte.Split('(');
                        if (lst.Length > 1)
                            str_transalte.Replace(lst[0], CmmFunction.GetTitle("TEXT_INPROCESS", BT_DangXL.TitleLabel.Text + " ("));
                        else
                            str_transalte = CmmFunction.GetTitle("TEXT_INPROCESS", BT_DangXL.TitleLabel.Text);

                        var indexA = str_transalte.IndexOf('(');
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.White, new NSRange(0, str_transalte.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.White, new NSRange(indexA, str_transalte.Length - indexA));//FromRGB(255, 122, 58)
                        _button.SetAttributedTitle(att, UIControlState.Normal);
                    }
                    else
                    {
                        str_transalte = CmmFunction.GetTitle("TEXT_INPROCESS", BT_DangXL.TitleLabel.Text);
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.White, new NSRange(0, str_transalte.Length));
                        _button.SetAttributedTitle(att, UIControlState.Normal);
                    }
                }
                else if (_index == 1)
                {
                    str_transalte = CmmFunction.GetTitle("TEXT_PROCESSED", BT_DaXL.TitleLabel.Text);
                    if (BT_DaXL.TitleLabel.Text.Contains("("))
                    {
                        //str_transalte = BT_DaXL.TitleLabel.Text;
                        //if (!str_transalte.Contains("("))
                        //    str_transalte = str_transalte + " (" + 50 + ")";
                        //var indexA = str_transalte.IndexOf('(');
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.White, new NSRange(0, str_transalte.Length));
                        _button.SetAttributedTitle(att, UIControlState.Normal);
                    }
                    else
                    {
                        //str_transalte = BT_DaXL.TitleLabel.Text;
                        //str_transalte = CmmFunction.GetTitle("TEXT_PROCESSED", BT_DaXL.TitleLabel.Text);
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.White, new NSRange(0, str_transalte.Length));
                        _button.SetAttributedTitle(att, UIControlState.Normal);
                    }
                }
            }
            else //selected
            {
                if (_index == 0)
                {
                    str_transalte = BT_DangXL.Title(UIControlState.Normal);
                    if (BT_DangXL.Title(UIControlState.Normal).Contains("("))//BT_DangXL.TitleLabel.Text.Contains("("))
                    {
                        //str_transalte = BT_DangXL.Title(UIControlState.Normal);//BT_DangXL.TitleLabel.Text;

                        //str_transalte = CmmFunction.GetTitle("TEXT_INPROCESS", BT_DangXL.TitleLabel.Text);
                        //if (!str_transalte.Contains("("))
                        //    str_transalte = str_transalte + " (" + dangxuly_count + ")";

                        var lst = str_transalte.Split('(');
                        if (lst.Length > 1)
                            str_transalte.Replace(lst[0], CmmFunction.GetTitle("TEXT_INPROCESS", BT_DangXL.TitleLabel.Text + " ("));
                        else
                            str_transalte = CmmFunction.GetTitle("TEXT_INPROCESS", BT_DangXL.TitleLabel.Text);
                        var indexA = str_transalte.IndexOf('(');

                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(_button.Font.PointSize, UIFontWeight.Semibold), new NSRange(0, att.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(0, str_transalte.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(255, 122, 58), new NSRange(indexA, str_transalte.Length - indexA));
                        _button.SetAttributedTitle(att, UIControlState.Normal);
                    }
                    else
                    {
                        //str_transalte = CmmFunction.GetTitle("TEXT_INPROCESS", BT_DangXL.TitleLabel.Text);
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(_button.Font.PointSize, UIFontWeight.Semibold), new NSRange(0, att.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(0, att.Length));
                        _button.SetAttributedTitle(att, UIControlState.Normal);
                    }
                }
                else if (_index == 1)
                {
                    str_transalte = BT_DaXL.Title(UIControlState.Normal);//BT_DaXL.TitleLabel.Text;
                    if (BT_DaXL.Title(UIControlState.Normal).Contains("("))
                    {
                        //str_transalte = BT_DaXL.TitleLabel.Text;
                        //str_transalte = CmmFunction.GetTitle("TEXT_PROCESSED", BT_DaXL.TitleLabel.Text);
                        //if (!str_transalte.Contains("("))
                        //    str_transalte = str_transalte + " (" + 50 + ")";

                        var indexA = str_transalte.IndexOf('(');
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(_button.Font.PointSize, UIFontWeight.Semibold), new NSRange(0, att.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(0, att.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(255, 122, 58), new NSRange(indexA, str_transalte.Length - indexA)); // inbox => blue
                        _button.SetAttributedTitle(att, UIControlState.Normal);
                    }
                    else
                    {
                        //str_transalte = CmmFunction.GetTitle("TEXT_PROCESSED", BT_DaXL.TitleLabel.Text);
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(_button.Font.PointSize, UIFontWeight.Semibold), new NSRange(0, att.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(0, att.Length));
                        _button.SetAttributedTitle(att, UIControlState.Normal);
                    }
                }
            }
        }

        private async void LoadItemSelected(bool isScrollToTop)
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(CmmVariable.M_DataPath, false);
            ShowObjContent();
            try
            {
                lst_addAttachment = new List<BeanAttachFile>();
                lst_addCommentAttachment = new List<BeanAttachFile>();

                //hide menu button default va table content, show lai khi load data hoan tat
                if (isScrollToTop)
                {
                    view_buttonDefault.Hidden = true;
                    table_content_right.Hidden = true;
                }

                if (currentSelectedItem != null)
                {
                    await Task.Run(() =>
                    {
                        string workflowID = "";
                        if (!string.IsNullOrEmpty(currentSelectedItem.ItemUrl))
                            workflowID = CmmFunction.GetWorkflowItemIDByUrl(currentSelectedItem.ItemUrl);

                        if (!string.IsNullOrEmpty(workflowID))
                        {
                            string query_workflowItem = string.Format("SELECT * FROM BeanWorkflowItem WHERE ID = {0}", workflowID);
                            var _list_workFlowItem = conn.QueryAsync<BeanWorkflowItem>(query_workflowItem).Result;

                            if (_list_workFlowItem != null && _list_workFlowItem.Count > 0)
                                workflowItem = _list_workFlowItem[0];
                            else
                            {
                                try
                                {
                                    workflowItem = Reachability.detectNetWork() ? new ProviderControlDynamic().getWorkFlowItemByRID(workflowID).FirstOrDefault() : null;
                                }
                                catch (Exception ex)
                                {
                                    workflowItem = null;
                                    Console.WriteLine("Không lấy item được: " + ex.ToString());
                                }
                            }
                        }

                        if (workflowItem != null)
                        {
                            List<BeanWorkflowFollow> lst_follow = new List<BeanWorkflowFollow>();
                            string query_follow = string.Format("SELECT * FROM BeanWorkflowFollow WHERE WorkflowItemId = ?");
                            var lst_followResult = conn.QueryAsync<BeanWorkflowFollow>(query_follow, workflowItem.ID).Result;
                            InvokeOnMainThread(() =>
                            {
                                if (lst_followResult != null && lst_followResult.Count > 0)
                                {
                                    if (lst_followResult[0].Status == 1)
                                    {
                                        BT_follow.SetImage(UIImage.FromFile("Icons/icon_Star_on.png"), UIControlState.Normal);
                                        isFollow = true;
                                    }
                                    else
                                    {
                                        BT_follow.SetImage(UIImage.FromFile("Icons/icon_Star_off.png"), UIControlState.Normal);
                                        isFollow = false;
                                    }
                                }
                                else
                                {
                                    BT_follow.SetImage(UIImage.FromFile("Icons/icon_Star_off.png"), UIControlState.Normal);
                                    isFollow = false;
                                }
                            });

                            string query_worflow = @"SELECT * FROM BeanWorkflow WHERE WorkflowID = ?";
                            BeanWorkflow workflow = conn.QueryAsync<BeanWorkflow>(query_worflow, workflowItem.WorkflowID).Result.FirstOrDefault();
                            if (workflow != null)
                            {
                                workflowItem.WorkflowTitle = workflow.Title;
                                workflowItem.WorkflowTitleEN = workflow.TitleEN;
                            }

                            InvokeOnMainThread(() =>
                            {
                                lbl_listName.Text = (CmmVariable.SysConfig.LangCode == "1033" ? workflowItem.WorkflowTitleEN : workflowItem.WorkflowTitle) + " - " + currentSelectedItem.Title;
                                //lbl_listName.Text = workflowItem.Title;
                                var heightTextContent = StringExtensions.StringHeight(lbl_listName.Text, lbl_listName.Font, lbl_listName.Frame.Width);
                                if (heightTextContent > lbl_listName.Frame.Height - 10)
                                {
                                    btn_ReadFull.Hidden = false;
                                    btn_ReadFull.Frame = lbl_listName.Frame;
                                    btn_ReadFull.TouchUpInside += delegate
                                    {
                                        NavigatorToFullTextView(new ViewElement { Value = lbl_listName.Text, Title = "", DataType = "textinput" }, null, null);
                                    };
                                }
                                else
                                {
                                    btn_ReadFull.Hidden = true;
                                }

                                #region AssignedTo - lay danh sach nguoi xu ly hien tai
                                arr_assignedTo = null;
                                string assignedTo = workflowItem.AssignedTo;
                                var maxStatusWidth = (lbl_assignedTo.Frame.Width / 3) * 2;
                                nfloat temp_width = 0;
                                string res = string.Empty;
                                if (!string.IsNullOrEmpty(assignedTo))
                                {
                                    List<BeanUser> lst_userResult = new List<BeanUser>();
                                    string query_user = string.Format("SELECT * FROM BeanUser WHERE ID = ?");
                                    if (assignedTo.Contains(','))
                                    {
                                        arr_assignedTo = assignedTo.Split(',');
                                        lst_userName = new List<string>();
                                        for (int i = 0; i < arr_assignedTo.Length; i++)
                                        {
                                            lst_userResult = conn.QueryAsync<BeanUser>(query_user, arr_assignedTo[i].Trim().ToLower()).Result;

                                            if (lst_userResult != null && lst_userResult.Count > 0)
                                            {
                                                lst_userName.Add(lst_userResult[0].FullName);
                                            }
                                        }

                                        string first_user = "";
                                        if (assignedTo.Contains(','))
                                            first_user = assignedTo.Split(',')[0];
                                        else
                                            first_user = assignedTo;

                                        if (lst_userName.Count > 1)
                                        {
                                            int num_remain = lst_userName.Count - 1;
                                            assignedTo = lst_userName[0] + ", +" + num_remain.ToString();
                                        }
                                        else
                                            assignedTo = lst_userName[0];
                                    }
                                    else
                                    {
                                        lst_userResult = conn.QueryAsync<BeanUser>(query_user, assignedTo.Trim().ToLower()).Result;
                                        if (lst_userResult != null && lst_userResult.Count > 0)
                                            assignedTo = lst_userResult[0].FullName;
                                    }

                                    if (workflowItem.ActionStatusID == 10) // da phe duyet
                                    {
                                        res = CmmFunction.GetTitle("TEXT_TITLE_APPROVED", "Phê duyệt: ") + assignedTo;
                                        lbl_assignedTo.Text = res.TrimEnd(',');
                                    }
                                    else if (workflowItem.ActionStatusID == -1) // da huy
                                    {
                                        res = CmmFunction.GetTitle("TEXT_TITLE_CANCEL", "Hủy: ") + assignedTo;
                                        lbl_assignedTo.Text = res.TrimEnd(',');
                                    }
                                    else if (workflowItem.ActionStatusID == 6)
                                    {
                                        res = CmmFunction.GetTitle("TEXT_TITLE_REJECTED", "Từ chối: ") + assignedTo;
                                        lbl_assignedTo.Text = res.TrimEnd(',');
                                    }
                                    else
                                    {
                                        res = CmmFunction.GetTitle("TEXT_TO", "Đến: ") + assignedTo;
                                        if (res.Contains('+'))
                                        {
                                            var indexA = res.IndexOf('+');
                                            NSMutableAttributedString att = new NSMutableAttributedString(res);
                                            att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(94, 94, 94), new NSRange(0, res.Length));
                                            att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(65, 80, 134), new NSRange(indexA, res.Length - indexA));
                                            att.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(lbl_listName.Font.PointSize, UIFontWeight.Semibold), new NSRange(indexA, res.Length - indexA));
                                            lbl_assignedTo.AttributedText = att;//(att, UIControlState.Normal);
                                        }
                                        else
                                            lbl_assignedTo.Text = res.TrimEnd(',');
                                    }
                                }
                                #endregion
                            });

                            ProviderControlDynamic p_controlDynamic = new ProviderControlDynamic();
                            Json_FormDataString = p_controlDynamic.GetTicketRequestControlDynamicForm(workflowItem);

                            if (!string.IsNullOrEmpty(Json_FormDataString))
                            {
                                JObject retValue = JObject.Parse(Json_FormDataString);

                                //danh sach chi tiet form controls
                                JArray json_dataForm = JArray.Parse(retValue["form"].ToString());

                                //danh sach quy trinh lien quan
                                JArray json_workflowRelated = JArray.Parse(retValue["related"].ToString());
                                lstWorkFlowRelateds = json_workflowRelated.ToObject<List<BeanWorkFlowRelated>>();

                                //danh sach cong viec phan cong
                                JArray json_tasks = JArray.Parse(retValue["task"].ToString());
                                lst_tasks = json_tasks.ToObject<List<BeanTask>>();

                                lst_comments = new List<BeanComment>();
                                if (!string.IsNullOrEmpty(retValue["moreInfo"]["CommentChanged"].ToString()))//HasValues
                                    _CommentChanged = DateTime.Parse(retValue["moreInfo"]["CommentChanged"].ToString());
                                else
                                    _CommentChanged = new DateTime();

                                if (!string.IsNullOrEmpty(retValue["moreInfo"]["OtherResourceId"].ToString()))
                                    _OtherResourceId = retValue["moreInfo"]["OtherResourceId"].ToString();
                                else
                                    _OtherResourceId = "";

                                ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();
                                //tracking
                                ObjectSubmitDetailComment _objSubmitDetailComment = new ObjectSubmitDetailComment();
                                // comment
                                _objSubmitDetailComment.ID = _OtherResourceId; // empty or result
                                _objSubmitDetailComment.ResourceCategoryId = "8";
                                _objSubmitDetailComment.ResourceUrl = string.Format(CmmFunction.GetURLSettingComment(8), workflowItem.ID); // lấy trong beansetting
                                _objSubmitDetailComment.ItemId = workflowItem.ID;
                                _objSubmitDetailComment.Author = CmmVariable.SysConfig.UserId;
                                _objSubmitDetailComment.AuthorName = CmmVariable.SysConfig.DisplayName;

                                if (string.IsNullOrEmpty(_OtherResourceId))
                                    _OtherResourceId = _pControlDynamic.GetDetailOtherResource(_objSubmitDetailComment);
                                else
                                    _pControlDynamic.GetDetailOtherResource(_objSubmitDetailComment);

                                lst_comments = CmmFunction.GetListComment(workflowItem, _OtherResourceId, _CommentChanged);

                                workflowItem.IsFollow = Convert.ToBoolean(json_dataForm[0]["IsFollow"]);
                                str_json_FormDefineInfo = json_dataForm[0]["FormDefineInfo"].ToString();
                                lst_section = json_dataForm.ToObject<List<ViewSection>>();

                                var result = lst_section.SelectMany(s => s.ViewRows)
                                            .FirstOrDefault(s => s.Elements.Any(d => d.DataType == "inputattachmenthorizon"));
                                var isReadonly = workflowItem.StatusGroup.HasValue && workflowItem.StatusGroup.Value == 1;

                                InvokeOnMainThread(() =>
                                {
                                    if (result != null)
                                    {
                                        JArray json = JArray.Parse(result.Elements[0].Value);
                                        lst_attachFile = json.ToObject<List<BeanAttachFile>>();
                                        if (lst_attachFile != null && lst_attachFile.Count > 0)
                                        {
                                            BT_attachement.Alpha = 1;
                                            BT_attachement.UserInteractionEnabled = true;
                                        }
                                        else
                                        {
                                            BT_attachement.Alpha = 0.2f;
                                            BT_attachement.UserInteractionEnabled = false;
                                        }
                                    }
                                    else
                                    {
                                        BT_attachement.Alpha = 0.2f;
                                        BT_attachement.UserInteractionEnabled = false;
                                    }
                                    #region get danh sách action
                                    if (isReadonly)
                                    {
                                        view_buttonDefault.Hidden = false;
                                        BT_comment.Enabled = false;
                                        BT_attachement.Enabled = false;
                                    }
                                    else
                                    {
                                        BT_comment.Enabled = true;
                                        BT_attachement.Enabled = true;

                                        constant_ButtonactionDefaut.Constant = view_buttonAction_default.Width;
                                        //view_buttonAction.Frame = new CGRect(view_buttonAction_default.X, view_buttonAction.Frame.Y, view_buttonAction_default.Width, view_buttonAction.Frame.Height);
                                        JObject jsonButtonBot = JObject.Parse(retValue["action"].ToString());
                                        var buttonBot = jsonButtonBot.ToObject<ViewRow>();

                                        if (componentButton != null)
                                            componentButton.RemoveFromSuperview();

                                        if (buttonBot.Elements == null || buttonBot.Elements.Count == 0)
                                        {
                                            isReadonly = workflowItem.StatusGroup.HasValue && workflowItem.StatusGroup.Value == 4;// phiếu đang yêu cầu hiệu chỉnh thì không đc thao tác trên app
                                        }
                                        else
                                        {
                                            componentButton = new ComponentButtonBot(this, buttonBot);
                                            buttonBot.Elements = CmmFunction.SortListElementAction(buttonBot.Elements);
                                            //int count_item;
                                            //if (buttonBot.Elements.Count > 4)
                                            //    count_item = 4;
                                            //else
                                            //    count_item = buttonBot.Elements.Count;

                                            //nfloat buttonWidth = (view_buttonAction.Frame.Width - 20) / 4;
                                            //int item_menuMiss = 4 - count_item;
                                            //nfloat startPoint = view_buttonAction.Frame.X + item_menuMiss * buttonWidth;

                                            //var view_buttonAction_width = (((view_buttonAction.Frame.Width - 20) / 4) + 5) * count_item;
                                            componentButton.InitializeFrameView(view_buttonAction.Bounds);
                                            componentButton.SetTitle();
                                            componentButton.SetValue();
                                            componentButton.SetEnable();
                                            componentButton.SetProprety();

                                            //view_buttonAction.Frame = new CGRect(startPoint, view_buttonAction.Frame.Y, view_buttonAction_width, view_buttonAction.Frame.Height);
                                            view_buttonAction.Add(componentButton);
                                            //constant_ButtonactionDefaut.Constant = view_buttonAction_width;
                                            lst_menuItem = componentButton.lst_moreActions;
                                        }

                                        //show menu button default
                                        view_buttonDefault.Hidden = false;
                                    }
                                    #endregion
                                    //table_content_right.Source = new Control_TableSource(lst_section, lstWorkFlowRelateds, lst_tasks, lst_comments, this);
                                    if (lstWorkFlowRelateds != null && lstWorkFlowRelateds.Count > 0)
                                        table_content_right.Source = new Control_TableSource(lst_section, lstWorkFlowRelateds, lst_tasks, lst_comments, this, isReadonly);
                                    else
                                        table_content_right.Source = new Control_TableSource(lst_section, null, lst_tasks, lst_comments, this, isReadonly);
                                    //view_task_right.Hidden = false;
                                    //lbl_nodata.Hidden = true;
                                    table_content_right.ReloadData();

                                    if (isScrollToTop)
                                        table_content_right.ScrollToRow(NSIndexPath.FromRowSection(0, 0), UITableViewScrollPosition.Top, true);

                                    ////Kiem tra co phai Cong Viec hay khong
                                    //if (currentSelectedItem.ResourceCategoryId == (int)CmmFunction.CommentResourceCategoryID.Task && currentSelectedItem.ResourceSubCategoryId == 0)
                                    //{
                                    //    if (isShowTask)
                                    //    {
                                    //        BeanTask task = lst_tasks.Where(t => t.ID == currentSelectedItem.ID).FirstOrDefault();
                                    //        if (task != null)
                                    //            Handle_TaskSelected(task, null);
                                    //        else
                                    //            CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("MESS_TASK_NOTFOUND", "Không tìm thấy thông tin công việc, vui lòng thử lại sau!"));
                                    //    }
                                    //}

                                    #region get danh sách action
                                    //if (isReadonly)
                                    //{
                                    //    view_buttonDefault.Hidden = false;
                                    //    BT_comment.Enabled = false;
                                    //    BT_attachement.Enabled = false;
                                    //}
                                    //else
                                    //{
                                    //    BT_comment.Enabled = true;
                                    //    BT_attachement.Enabled = true;

                                    //    constant_ButtonactionDefaut.Constant = view_buttonAction_default.Width;
                                    //    //view_buttonAction.Frame = new CGRect(view_buttonAction_default.X, view_buttonAction.Frame.Y, view_buttonAction_default.Width, view_buttonAction.Frame.Height);
                                    //    JObject jsonButtonBot = JObject.Parse(retValue["action"].ToString());
                                    //    var buttonBot = jsonButtonBot.ToObject<ViewRow>();

                                    //    if (componentButton != null)
                                    //        componentButton.RemoveFromSuperview();

                                    //    if (buttonBot.Elements == null || buttonBot.Elements.Count == 0)
                                    //    {
                                    //        isReadonly = workflowItem.StatusGroup.HasValue && workflowItem.StatusGroup.Value == 4;// phiếu đang yêu cầu hiệu chỉnh thì không đc thao tác trên app
                                    //    }
                                    //    else
                                    //    {
                                    //        componentButton = new ComponentButtonBot(this, buttonBot);
                                    //        buttonBot.Elements = CmmFunction.SortListElementAction(buttonBot.Elements);
                                    //        //int count_item;
                                    //        //if (buttonBot.Elements.Count > 4)
                                    //        //    count_item = 4;
                                    //        //else
                                    //        //    count_item = buttonBot.Elements.Count;

                                    //        //nfloat buttonWidth = (view_buttonAction.Frame.Width - 20) / 4;
                                    //        //int item_menuMiss = 4 - count_item;
                                    //        //nfloat startPoint = view_buttonAction.Frame.X + item_menuMiss * buttonWidth;

                                    //        //var view_buttonAction_width = (((view_buttonAction.Frame.Width - 20) / 4) + 5) * count_item;
                                    //        componentButton.InitializeFrameView(view_buttonAction.Bounds);
                                    //        componentButton.SetTitle();
                                    //        componentButton.SetValue();
                                    //        componentButton.SetEnable();
                                    //        componentButton.SetProprety();

                                    //        //view_buttonAction.Frame = new CGRect(startPoint, view_buttonAction.Frame.Y, view_buttonAction_width, view_buttonAction.Frame.Height);
                                    //        view_buttonAction.Add(componentButton);
                                    //        //constant_ButtonactionDefaut.Constant = view_buttonAction_width;
                                    //        lst_menuItem = componentButton.lst_moreActions;
                                    //    }

                                    //    //show menu button default
                                    //    view_buttonDefault.Hidden = false;
                                    //}
                                    #endregion

                                    if (isReadonly)
                                        CmmIOSFunction.commonAlertMessage(this, "VNA BPM", CmmFunction.GetTitle(CmmVariable.TEXT_ALERT_DRAFT, "Vui lòng sử dụng phiên bản web để chỉnh sửa phiếu này!"));
                                    //table_content_right.Hidden = false;
                                    ShowObjContent(true, true);
                                });
                                loadQuaTrinhluanchuyen();
                            }
                            else
                            {
                                //view_task_right.Hidden = true;
                                //lbl_nodata.Hidden = false;
                                InvokeOnMainThread(() =>
                                {
                                    ShowObjContent(true, false);
                                    UIAlertController alert = UIAlertController.Create("Thông báo", CmmFunction.GetTitle("MESS_WORKFLOW_NOTFOUND", "Không tìm thấy thông tin phiếu, vui lòng thử lại sau!"), UIAlertControllerStyle.Alert);//"BPM"
                                    alert.AddAction(UIAlertAction.Create("Đóng", UIAlertActionStyle.Default, alertAction =>
                                    {

                                    }));
                                    this.PresentViewController(alert, true, null);
                                });
                            }
                        }
                        else
                        {
                            InvokeOnMainThread(() =>
                            {
                                //view_task_right.Hidden = true;
                                //lbl_nodata.Hidden = false;
                                //lbl_listName.Hidden = view_task_right.Hidden;
                                ShowObjContent(true, false);
                                Console.WriteLine("Item được chọn vẫn null.");
                            });
                        }
                    });

                    await Task.Run(() =>
                    {
                        //Kiem tra co phai Cong Viec hay khong
                        if (currentSelectedItem != null && currentSelectedItem.ResourceCategoryId.HasValue && currentSelectedItem.ResourceCategoryId.Value == (int)CmmFunction.CommentResourceCategoryID.Task
                            && currentSelectedItem.ResourceSubCategoryId.HasValue && currentSelectedItem.ResourceSubCategoryId.Value == 0
                            && isShowTask)
                        {
                            BeanTask task = lst_tasks.Where(t => t.ID == currentSelectedItem.ID).FirstOrDefault();
                            if (task != null)
                            {
                                InvokeOnMainThread(() =>
                                {
                                    Handle_TaskSelected(task, null);
                                });
                            }
                            else
                                CmmIOSFunction.commonAlertMessage(this, "VNA BPM", CmmFunction.GetTitle("MESS_TASK_NOTFOUND", "Không tìm thấy thông tin công việc, vui lòng thử lại sau!"));
                        }
                    });
                }
                else
                {
                    //view_task_right.Hidden = true;
                    //lbl_nodata.Hidden = false;
                    //lbl_listName.Hidden = view_task_right.Hidden;
                    ShowObjContent(true, false);
                    Console.WriteLine("Item được chọn vẫn null.");
                }
            }
            catch (Exception ex)
            {
                ShowObjContent(true, false);
                CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"));
                Console.WriteLine("TodoDetailView - GetIndexItemFromDictionnary - ERR: " + ex.ToString());
            }
            finally
            {
                //loading.Hide();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doShow">true: hiện loading spinner</param>
        /// <param name="hasValue">true: hiện content phiếu</param>
        void ShowObjContent(bool doShow = false, bool hasValue = false)
        {
            if (doShow) //hiện view content, loading.
            {
                table_content_right.Hidden = !hasValue;
                view_task_right.Hidden = !hasValue;
                lbl_nodata.Hidden = hasValue;
                view_userInfo.Hidden = !hasValue;

                loading?.Hide();
            }
            else //hiện loading và ẩn content
            {
                if (loading != null && loading.IsDescendantOfView(view_task_right))
                    loading?.Hide();
                //loading = new CmmLoading(new CGRect(view_task_right.Center.X - 100, view_task_right.Center.Y - 100, 200, 200), "Loading...");
                loading = new CmmLoading(new CGRect((view_task_right.Bounds.Width - 200) / 2, (view_task_right.Bounds.Height - 200) / 2, 200, 200), "Đang xử lý...");
                view_task_right.Add(loading);

                //isShowTask = false;
                table_content_right.Hidden = true;
                view_task_right.Hidden = false;
                lbl_nodata.Hidden = true;
                view_userInfo.Hidden = true;
            }
            lbl_listName.Hidden = view_task_right.Hidden;
        }

        /// <summary>
        /// Refresh view
        /// </summary>
        public void ReLoadDataFromServer(bool scrollToTopList, bool scrollToTopForm)
        {
            //Reset lai trang thai cua 
            tf_search.Text = string.Empty;
            BT_search.TintColor = UIColor.Black;
            this.View.EndEditing(true);
            if (isSearch)
            {
                UIView.BeginAnimations("show_animationShowTable");
                UIView.SetAnimationDuration(0.5f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);
                constraint_rightBT_Search.Constant = origin_constraint_rightBT_Search;
                BT_search.SetImage(UIImage.FromFile("Icons/icon_search.png"), UIControlState.Normal);
                UIView.CommitAnimations();
                view_segments.Hidden = false;
                isSearch = false;

                if (!string.IsNullOrEmpty(tf_search.Text))
                    BT_search.TintColor = UIColor.Orange;
                else
                    BT_search.TintColor = UIColor.Black;
            }

            if (scrollToTopList)
            {
                currentSelectedItem = null;
                isFilter = false;
                SetDefaultFilter();
                LoadDataFilterTodo(true);

                //if (lst_appBase_cxl != null && lst_appBase_cxl.Count > 0)
                //{
                //    for (int i = 0; i < table_todo.NumberOfSections(); i++)
                //    {
                //        if (table_todo.NumberOfRowsInSection(i) > 0)
                //        {
                //            table_todo.ScrollToRow(NSIndexPath.FromRowSection(0, i), UITableViewScrollPosition.Top, true);
                //            break;
                //        }
                //    }
                //}
            }
            else
            {
                LoadItemSelected(false);
            }

            ToggleTodo();
            //LoadItemSelected(scrollToTopForm);
        }
        #endregion

        private NSIndexPath GetIndexItemFromDictionnary(BeanAppBaseExt _item)
        {
            NSIndexPath indexResult = null;
            var i = 0;
            do
            {
                var j = 0;
                var key = dict_todo.ElementAt(i).Key;
                do
                {
                    if ((dict_todo[key][j] != null) && _item.ID == dict_todo[key][j].ID)
                    {
                        indexResult = NSIndexPath.FromRowSection(j, i);
                        break;
                    }
                    j++;
                }
                while (j < dict_todo[key].Count);
                i++;
            }
            while (i < dict_todo.Count && indexResult == null);

            return indexResult;
        }

        private async void checkFileLocalIsExist(BeanUser contact, UIImageView image_view)
        {
            try
            {
                bool result = false;
                string filename = contact.ImagePath.Split('/').Last();
                string filepathURL = CmmVariable.M_Domain + "/" + CmmVariable.SysConfig.Subsite + contact.ImagePath;
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
                    {
                        openFile(filename, image_view);
                        image_view.Hidden = false;
                    }
                    else
                        image_view.Hidden = true;
                }
                else
                {
                    openFile(filename, image_view);
                    image_view.Hidden = false;
                }
            }
            catch (Exception ex)
            {
                image_view.Image = UIImage.FromFile("Icons/icon_profile.png");
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
                        image_view.Image = UIImage.FromFile("Icons/icon_profile.png");
                    }
                }
                else
                {
                    image_view.Image = UIImage.FromFile("Icons/icon_profile.png");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("ContactView - openFile - Err: " + ex.ToString());
            }
        }

        public void UpdateItemSelect(BeanAppBaseExt _itemSelected, NSIndexPath index)
        {
            try
            {
                IsFirstLoad = true;
                //loading.Hide();
                CloseAddFollow();
                HandleAttachFileClose();

                Custom_MenuAction custom_menuAction = Custom_MenuAction.Instance;
                if (custom_menuAction.Superview != null)
                    custom_menuAction.RemoveFromSuperview();
                HandleWorkFollowViewResult();

                currentSelectedItem = _itemSelected;
                if (currentSelectedItem.ResourceCategoryId == (int)CmmFunction.CommentResourceCategoryID.Task)
                    isShowTask = true;

                foreach (var item in lst_appBase_cxl)
                {
                    if (item.ID == currentSelectedItem.ID)
                        item.IsSelected = true;
                    else
                        item.IsSelected = false;
                }
                table_todo.ReloadData();
                LoadItemSelected(true);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("ToDoDetailView - UpdateItemSelect - err : " + ex.ToString());
#endif
            }
        }

        public void UpdateTableSections(int sectionIndex, BeanComment comment)
        {
            var item = lst_comments.Where(i => i.ID == comment.ID).FirstOrDefault();
            item = comment;

            LoadItemSelected(false);
            //table_content_right.ReloadData();
        }

        private void CloseAddFollow()
        {
            Custom_AddFollowView custom_AddFollowView = Custom_AddFollowView.Instance;
            if (custom_AddFollowView.Superview != null)
                custom_AddFollowView.RemoveFromSuperview();
        }

        private void CloseMenuOption()
        {
            Custom_MenuAction custom_MenuAction = Custom_MenuAction.Instance;
            if (custom_MenuAction.Superview != null)
                custom_MenuAction.RemoveFromSuperview();
        }

        public UITableView GetTableView
        {
            get
            {
                return this.table_content_right;
            }
        }

        private ViewElement GetViewElementByDataType(string _dataType)
        {
            ViewElement temp = null;
            var numSection = 0;
            do
            {
                var numRow = 0;
                var viewSection = lst_section[numSection];
                do
                {
                    var viewRow = viewSection.ViewRows[numRow];
                    foreach (var item in viewRow.Elements)
                    {
                        if (item.DataType == _dataType)
                        {
                            temp = item;
                            break;
                        }
                    }
                    numRow++;
                } while (temp == null && numRow < viewSection.ViewRows.Count);

                numSection++;
            } while (temp == null && numSection < lst_section.Count);

            return temp;
        }

        #region handle user choice
        public void HandleUserMultiChoiceSelected(ViewElement element, List<BeanUser> _userSelected)
        {
            List<BeanUser> lst_userChoice = _userSelected;
            string jsonString = string.Empty;

            if (lst_userChoice != null && lst_userChoice.Count > 0)
            {
                foreach (var item in lst_userChoice)
                {
                    item.Name = item.FullName;
                }
                jsonString = JsonConvert.SerializeObject(lst_userChoice);
            }

            element.Value = jsonString;
            UpdateValueElement_InListSection(element);
            table_content_right.ReloadData();
        }
        public void HandleUserSingleChoiceSelected(ViewElement element, BeanUser _userSelected)
        {
            List<BeanUser> lst_userChoice = new List<BeanUser>();
            lst_userChoice.Add(_userSelected);
            var jsonString = JsonConvert.SerializeObject(lst_userChoice);
            element.Value = jsonString;
            UpdateValueElement_InListSection(element);
            table_content_right.ReloadData();
        }
        public void NavigatorToUserChoiceView(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
        {
            if (element != null)
            {
                CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
                CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
                FormUsersView formUsersView = (FormUsersView)Storyboard.InstantiateViewController("FormUsersView");
                if (element.DataType == "selectuser")
                    formUsersView.SetContent(this, false, null, false, element, element.Title);
                else if (element.DataType == "selectusermulti")
                    formUsersView.SetContent(this, true, null, false, element, element.Title);
                PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                formUsersView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                formUsersView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                formUsersView.TransitioningDelegate = transitioningDelegate;
                this.PresentViewControllerAsync(formUsersView, true);

            }
            else
            {
                CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("MESS_WORKFLOW_NOTFOUND", "Không tìm thấy thông tin phiếu, vui lòng thử lại sau!"));
            }
        }
        public void NavigateToUserOrGroupChoiceView(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
        {
            if (element != null)
            {
                FormUserAndGroupView listUserOrGroup = (FormUserAndGroupView)Storyboard.InstantiateViewController("FormUserAndGroupView");
                if (element.DataType == "selectusergroup")
                    listUserOrGroup.SetContent(this, false, null, false, element, element.Title, false);
                else if (element.DataType == "selectusergroupmulti")
                    listUserOrGroup.SetContent(this, true, null, false, element, element.Title, false);

                CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
                CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
                PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                listUserOrGroup.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                listUserOrGroup.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                listUserOrGroup.TransitioningDelegate = transitioningDelegate;
                this.PresentViewControllerAsync(listUserOrGroup, true);
            }
            else
            {
                CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("MESS_WORKFLOW_NOTFOUND", "Không tìm thấy thông tin phiếu, vui lòng thử lại sau!"));
            }
        }
        public void HandleUserOrGroupSingleChoiceSelected(ViewElement element, BeanUserAndGroup _userSelected)
        {
            List<BeanUserAndGroup> lst_userChoice = new List<BeanUserAndGroup>();
            lst_userChoice.Add(_userSelected);
            var jsonString = JsonConvert.SerializeObject(lst_userChoice);
            element.Value = jsonString;
            UpdateValueElement_InListSection(element);
            table_content_right.ReloadData();
        }
        public void HandleUserOrGroupMultiChoiceSelected(ViewElement element, List<BeanUserAndGroup> _userSelected)
        {
            List<BeanUserAndGroup> lst_userChoice = _userSelected;

            if (lst_userChoice.Count > 0)
            {
                foreach (var item in lst_userChoice)
                {
                    item.Name = item.Name;
                }
            }

            string jsonString = string.Empty;
            if (lst_userChoice != null && lst_userChoice.Count > 0)
                jsonString = JsonConvert.SerializeObject(lst_userChoice);

            element.Value = jsonString;
            UpdateValueElement_InListSection(element);
            table_content_right.ReloadData();
        }

        #region handle input text
        public void NavigatorToEditTextView(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
        {
            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            FormEditTextController textViewControlView = (FormEditTextController)Storyboard.InstantiateViewController("FormEditTextController");
            textViewControlView.setContent(this, 1, true, element, "");
            PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            textViewControlView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            textViewControlView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            textViewControlView.TransitioningDelegate = transitioningDelegate;
            this.PresentViewControllerAsync(textViewControlView, true);

        }
        public void HandleSingleLine(ViewElement element)
        {
            UpdateValueElement_InListSection(element);
            table_content_right.ReloadData();
        }
        public void NavigatorToFullTextView(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
        {
            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            FormEditTextController textViewControlView = (FormEditTextController)Storyboard.InstantiateViewController("FormEditTextController");
            textViewControlView.setContent(this, 1, false, element, "");
            PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            textViewControlView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            textViewControlView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            textViewControlView.TransitioningDelegate = transitioningDelegate;
            this.PresentViewControllerAsync(textViewControlView, true);
        }
        #endregion
        #endregion

        #region handle number - currence
        public void NavigatorToEditNumberView(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
        {
            NumberControlView numberControlView = (NumberControlView)Storyboard.InstantiateViewController("NumberControlView");
            numberControlView.setContent(this, 1, element);

            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            numberControlView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            numberControlView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            numberControlView.TransitioningDelegate = transitioningDelegate;
            this.PresentViewControllerAsync(numberControlView, true);
        }
        public void HandleEditNumber(ViewElement element)
        {
            UpdateValueElement_InListSection(element);
            table_content_right.ReloadData();
        }
        #endregion

        #region handle choice
        public void HandleChoiceSelected(ViewElement element)
        {
            UpdateValueElement_InListSection(element);
            table_content_right.ReloadData();
        }
        public void NavigatorToChoiceView(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
        {
            if (element != null)
            {
                FormListItemsChoice itemsChoiceView = (FormListItemsChoice)Storyboard.InstantiateViewController("FormListItemsChoice");
                if (element.DataType == "singlechoice")
                    itemsChoiceView.setContent(this, false, element);
                else if (element.DataType == "singlelookup")
                    itemsChoiceView.setContent(this, false, element);
                else if (element.DataType == "multiplechoice")
                    itemsChoiceView.setContent(this, true, element);
                else if (element.DataType == "multilookup")
                    itemsChoiceView.setContent(this, true, element);

                CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
                CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
                PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                itemsChoiceView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                itemsChoiceView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                itemsChoiceView.TransitioningDelegate = transitioningDelegate;
                this.PresentViewControllerAsync(itemsChoiceView, true);
            }
            else
            {
                CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("MESS_WORKFLOW_NOTFOUND", "Không tìm thấy thông tin phiếu, vui lòng thử lại sau!"));
            }
        }
        #endregion

        #region handle Attachment
        public void HandleAddAttachment(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
        {
            try
            {
                CloseAddFollow();
                this.View.EndEditing(true);
                Custom_AttachFileView attachFileView = Custom_AttachFileView.Instance;
                attachmentElement = element;
                attachFileView.element = element;
                attachFileView.viewController = this;
                attachFileView.InitFrameView(new CGRect(0, 0, this.View.Frame.Width, this.View.Frame.Height));
                attachFileView.TableLoadData();

                view_task_right.AddSubview(attachFileView);

                attachFileView.Frame = new CGRect(view_task_right.Frame.Right, 0, view_task_right.Frame.Width, view_task_right.Frame.Height);
                UIView.BeginAnimations("show_animationShowTable");
                UIView.SetAnimationDuration(0.3f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);
                attachFileView.Frame = new CGRect(10, 0, view_task_right.Frame.Width, view_task_right.Frame.Height);
                UIView.CommitAnimations();

            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("CreateNewTaskView - HandleAddAttachment - Err: " + ex.ToString());
#endif
            }
        }

        public void HandleAttachFileClose()
        {
            Custom_AttachFileView custom_AttachFileView = Custom_AttachFileView.Instance;
            if (custom_AttachFileView.Superview != null)
                custom_AttachFileView.RemoveFromSuperview();
        }

        public void HandleAddAttachFileResult(BeanAttachFileLocal _attachFile, string elementdataType)
        {
            ViewElement viewElement = GetViewElementByDataType(elementdataType);
            if (elementdataType == "inputattachmenthorizon")
            {
                if (lst_addAttachment == null)
                    lst_addAttachment = new List<BeanAttachFile>();

                if (!string.IsNullOrEmpty(viewElement.Value))
                {
                    lst_addAttachment = new List<BeanAttachFile>();
                    JArray json = JArray.Parse(viewElement.Value);
                    lst_addAttachment = json.ToObject<List<BeanAttachFile>>();
                }

                numRowAttachmentFile++;
                string custID = numRowAttachmentFile + "";
                BeanAttachFile attachFile = new BeanAttachFile()
                {
                    ID = "",
                    Title = _attachFile.Name + ";#" + DateTime.Now.ToShortTimeString(),
                    Path = _attachFile.Path,
                    Size = _attachFile.Size,
                    IsImage = _attachFile.IsImage,
                    Category = "",
                    IsAuthor = true,
                    CreatedBy = CmmVariable.SysConfig.UserId,
                    CreatedName = CmmVariable.SysConfig.DisplayName,
                    CreatedPositon = CmmVariable.SysConfig.PositionTitle,
                    AttachTypeId = null,
                    AttachTypeName = "",
                    WorkflowId = workflowItem.WorkflowID,
                    WorkflowItemId = int.Parse(workflowItem.ID)
                };

                lst_addAttachment.Add(attachFile);
                var jsonString = JsonConvert.SerializeObject(lst_addAttachment);
                viewElement.Value = jsonString;
            }
            else if (elementdataType == "inputcomments")
            {
                if (lst_addCommentAttachment == null)
                    lst_addCommentAttachment = new List<BeanAttachFile>();

                BeanAttachFile attachFile = new BeanAttachFile()
                {
                    ID = "",
                    Title = _attachFile.Name + ";#" + DateTime.Now.ToShortTimeString(),
                    Path = _attachFile.Path,
                    Size = _attachFile.Size,
                    IsImage = _attachFile.IsImage,
                    Category = "",
                    IsAuthor = true,
                    CreatedBy = CmmVariable.SysConfig.UserId,
                    CreatedName = CmmVariable.SysConfig.DisplayName,
                    CreatedPositon = CmmVariable.SysConfig.PositionTitle,
                    AttachTypeId = null,
                    AttachTypeName = "",
                    WorkflowId = workflowItem.WorkflowID,
                    WorkflowItemId = int.Parse(workflowItem.ID)
                };

                lst_addCommentAttachment.Add(attachFile);

                List<BeanAttachFile> lst_attachImage = new List<BeanAttachFile>();
                List<BeanAttachFile> lst_attachDoc = new List<BeanAttachFile>();
                foreach (var attach in lst_addCommentAttachment)
                {
                    if (attach.IsImage)
                        lst_attachImage.Add(attach);
                    else
                        lst_attachDoc.Add(attach);
                }

                var jsonStringImage = JsonConvert.SerializeObject(lst_attachImage);
                var jsonStringDoc = JsonConvert.SerializeObject(lst_attachDoc);

                ObjectElementNote note1 = new ObjectElementNote { Key = "image", Value = jsonStringImage };
                ObjectElementNote note2 = new ObjectElementNote { Key = "doc", Value = jsonStringDoc };
                List<ObjectElementNote> objectElementNotes = new List<ObjectElementNote>();
                objectElementNotes.Add(note1); objectElementNotes.Add(note2);

                viewElement.Notes = objectElementNotes;
            }

            table_content_right.ReloadData();
            HandleAttachFileClose();
        }
        public void HandleAddAttachCommentFileResult(BeanAttachFileLocal _attachFile, string elementdataType)
        {
            ViewElement viewElement = GetViewElementByDataType(elementdataType);

            if (lst_addCommentAttachment == null)
                lst_addCommentAttachment = new List<BeanAttachFile>();

            BeanAttachFile attachFile = new BeanAttachFile()
            {
                ID = "",
                Title = _attachFile.Name + ";#" + DateTime.Now.ToShortTimeString(),
                Path = _attachFile.Path,
                Size = _attachFile.Size,
                IsImage = _attachFile.IsImage,
                Category = "",
                IsAuthor = true,
                CreatedBy = CmmVariable.SysConfig.UserId,
                CreatedName = CmmVariable.SysConfig.DisplayName,
                CreatedPositon = CmmVariable.SysConfig.PositionTitle,
                AttachTypeId = null,
                AttachTypeName = "",
                WorkflowId = workflowItem.WorkflowID,
                WorkflowItemId = int.Parse(workflowItem.ID)
            };

            lst_addCommentAttachment.Add(attachFile);

            List<BeanAttachFile> lst_attachImage = new List<BeanAttachFile>();
            List<BeanAttachFile> lst_attachDoc = new List<BeanAttachFile>();
            foreach (var attach in lst_addCommentAttachment)
            {
                if (attach.IsImage)
                    lst_attachImage.Add(attach);
                else
                    lst_attachDoc.Add(attach);
            }

            var jsonString = JsonConvert.SerializeObject(lst_addCommentAttachment);
            var jsonStringImage = JsonConvert.SerializeObject(lst_attachImage);
            var jsonStringDoc = JsonConvert.SerializeObject(lst_attachDoc);

            ObjectElementNote note1 = new ObjectElementNote { Key = "image", Value = jsonStringImage };
            ObjectElementNote note2 = new ObjectElementNote { Key = "doc", Value = jsonStringDoc };
            List<ObjectElementNote> objectElementNotes = new List<ObjectElementNote>();
            objectElementNotes.Add(note1); objectElementNotes.Add(note2);

            viewElement.Notes = objectElementNotes;
            table_content_right.ReloadData();

            HandleAttachFileClose();
        }
        public void NavigationToDocumentPicker(string elementDatatype)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                var allowedUTIs = new string[] {
                            //UTType.UTF8PlainText,
                            //UTType.PlainText,
                            UTType.RTF,
                            UTType.PNG,
                            UTType.Text,
                            UTType.PDF,
                            UTType.Image,
                            //UTType.Data,
                            UTType.Content,
                            new NSString("com.microsoft.word.doc")
                        };

                docPicker = new UIDocumentPickerViewController(allowedUTIs, UIDocumentPickerMode.Import);
                docPicker.WasCancelled += (s, wasCancelledArgs) =>
                {
                };

                docPicker.DidPickDocumentAtUrls += (object s, UIDocumentPickedAtUrlsEventArgs ev) =>
                {
                    try
                    {
                        string filePath = ev.Urls[0].Path;
                        string fileName = ev.Urls[0].LastPathComponent;

                        var index = fileName.LastIndexOf('.');
                        var type = fileName.Substring((index + 1), fileName.Length - (index + 1));

                        string[] arrType = new string[] { "doc", "docx", "xls", "xlsx", "pdf", "png", "jpeg", "jpg" };

                        if (arrType.Contains(type.ToLower()))
                        {
                            var FileManager = new NSFileManager();
                            var size = (Int64)FileManager.Contents(filePath).Length;

                            BeanAttachFileLocal itemiCloudAndDevice = new BeanAttachFileLocal() { ID = -1, Name = fileName, Path = filePath, Size = size, Type = type, IsImage = false };
                            //HandleAddAttachFileResult(itemiCloudAndDevice, addAttachmentsView);
                            HandleAddAttachFileResult(itemiCloudAndDevice, elementDatatype);
                        }
                        else
                        {
                            CmmIOSFunction.AlertUnsupportFile(this);
                            Console.WriteLine("Selected file type: " + type);
                        }
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Console.WriteLine("DidPickDocumentAtUrls - err - :" + ex.ToString());
#endif
                    }
                };

                docPicker.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                PresentViewController(docPicker, true, null);
            }
            else
                CmmIOSFunction.commonAlertMessage(this, "BPM", "Chỉ hỗ trợ thêm tập tin office đính kèm từ hệ điều hành 11 trở lên.");
        }
        public void NavigationToImagePicker()
        {
            if (imagePicker != null)
            {
                imagePicker.Dispose();
                imagePicker = null;
            }

            imagePicker = new UIImagePickerController();

            imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
            //imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);
            imagePicker.MediaTypes = new string[] { UTType.Image };
            imagePicker.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            imagePicker.ModalPresentationStyle = UIModalPresentationStyle.PageSheet;

            imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
            imagePicker.Canceled += Handle_Canceled;

            this.PresentModalViewController(imagePicker, true);

        }
        public void NavigationToCameraPicker()
        {
            if (imagePicker != null)
            {
                imagePicker.Dispose();
                imagePicker = null;
            }

            imagePicker = new UIImagePickerController();

            imagePicker.SourceType = UIImagePickerControllerSourceType.Camera;
            //imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.Camera);
            imagePicker.MediaTypes = new string[] { UTType.Image };
            imagePicker.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            imagePicker.ModalPresentationStyle = UIModalPresentationStyle.PageSheet;

            imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
            imagePicker.Canceled += Handle_Canceled;

            this.PresentModalViewController(imagePicker, true);
        }
        protected void Handle_FinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e)
        {
            bool isImage = false;
            var mediaType = e.Info[UIImagePickerController.MediaType].ToString();
            switch (mediaType)
            {
                case "public.image":
                    Console.WriteLine("Image selected");
                    isImage = true;
                    break;
                case "public.video":
                case "public.movie":
                    Console.WriteLine("Video or Movie selected");
                    break;
                default:
                    Console.WriteLine("Selected media type: " + mediaType);
                    break;
            }

            // get common info (shared between images and video)
            NSUrl filePath = e.Info[new NSString("UIImagePickerControllerImageURL")] as NSUrl;

            // if it was an image, get the other image info
            if (isImage)
            {
                // get the original image
                UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;

                if (originalImage != null)
                {
                    if (filePath != null)
                    {
                        string[] fileName = filePath.ToString().Split("/");
                        var FileManager = new NSFileManager();
                        var size = (Int64)FileManager.Contents(filePath.Path).Length;

                        BeanAttachFileLocal itemiCloudAndDevice = new BeanAttachFileLocal() { ID = -1, Name = fileName[fileName.Length - 1], Path = filePath.Path, Size = size, IsImage = true };
                        HandleAddAttachFileResult(itemiCloudAndDevice, attachmentElement.DataType);
                    }
                    else
                    {
                        string fileName = "IMG_" + DateTime.Now.ToString("MMss") + ".JPG";


                        NSError err = null;

                        var imageFormat = CmmIOSFunction.RotateCameraImageToProperOrientation(originalImage, 1024);

                        var documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                        string img_Path = Path.Combine(documentFolder, fileName);
                        NSData imgData = imageFormat.AsJPEG();
                        if (imgData.Save(img_Path, false, out err))
                            Console.WriteLine("saved as " + img_Path);

                        var fileNameCust = fileName.Substring(fileName.LastIndexOf('/') + 1);
                        var FileManager = new NSFileManager();
                        var size = (Int64)FileManager.Contents(img_Path).Length;

                        BeanAttachFileLocal itemiCloudAndDevice = new BeanAttachFileLocal() { ID = -1, Name = fileName, Path = img_Path, Size = size, IsImage = true };
                        HandleAddAttachFileResult(itemiCloudAndDevice, attachmentElement.DataType);
                    }
                }
                // dismiss the picker
                imagePicker.DismissModalViewController(true);
                var vc = this.PresentedViewController;
                vc.DismissViewController(true, null);
            }
            else
            {
                // dismiss the picker
                imagePicker.DismissModalViewController(true);
                CmmIOSFunction.AlertUnsupportFile(this);
            }
        }

        private void Handle_Canceled(object sender, EventArgs e)
        {
            imagePicker.DismissModalViewController(true);
        }
        public void HandleAttachmentRemove(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase, int numRow, string _json_attachRemove)
        {
            json_attachRemove = _json_attachRemove;
            List<BeanAttachFile> lst_item = new List<BeanAttachFile>();
            if (!string.IsNullOrEmpty(element.Value))
            {
                JArray json = JArray.Parse(element.Value);
                lst_item = json.ToObject<List<BeanAttachFile>>();
            }

            var jsonString = JsonConvert.SerializeObject(lst_item);
            element.Value = jsonString;

            table_content_right.ReloadData();
        }
        public void HandleAttachmentThumbRemove(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase, int numRow, string _json_attachRemove)
        {
            json_attachRemove = _json_attachRemove;

            //JArray json = JArray.Parse(json_attachRemove);
            //List<BeanAttachFile> lst_thumbRemove = json.ToObject<List<BeanAttachFile>>();
            //var jsonString = JsonConvert.SerializeObject(lst_addAttachment);

            if (element.Notes != null && element.Notes.Count > 0)
            {
                foreach (var note in element.Notes)
                {
                    if (!string.IsNullOrEmpty(note.Value) && note.Value != "[]")
                    {
                        if (note.Key == "image")
                            note.Value = json_attachRemove;
                        else if (note.Key == "doc")
                            note.Value = json_attachRemove;
                    }
                }
            }

            table_content_right.ReloadData();
        }
        public void HandleAttachmentEdit(ViewElement element, NSIndexPath indexPath, BeanAttachFile _attach, ControlBase _controlBase)
        {
            FormEditAttachFileView formEditAttach = (FormEditAttachFileView)Storyboard.InstantiateViewController("FormEditAttachFileView");
            formEditAttach.SetContent(this, _attach);
            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            formEditAttach.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            formEditAttach.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            formEditAttach.TransitioningDelegate = transitioningDelegate;
            this.PresentViewControllerAsync(formEditAttach, true);
        }
        public void ReloadAttachmentElement(ViewElement _element, BeanAttachFile attachFile)
        {
            List<BeanAttachFile> lst_attachFile = new List<BeanAttachFile>();
            if (!string.IsNullOrEmpty(_element.Value))
            {
                JArray json = JArray.Parse(_element.Value);
                lst_attachFile = json.ToObject<List<BeanAttachFile>>();
            }

            var index = lst_attachFile.FindIndex(item => item.ID == attachFile.ID);
            if (index != -1)
                lst_attachFile[index] = attachFile;

            var jsonString = JsonConvert.SerializeObject(lst_attachFile);
            _element.Value = jsonString;

            table_content_right.ReloadData();
        }
        public void NavigateToShowAttachView(BeanAttachFile currentAttachFile)
        {
            ShowAttachmentView showAttachmentView = Storyboard.InstantiateViewController("ShowAttachmentView") as ShowAttachmentView;
            showAttachmentView.setContent(this, currentAttachFile);
            this.NavigationController.PushViewController(showAttachmentView, true);
        }
        #endregion

        #region handle WorkflowRelate
        public void HandleRemoveWorkFlowRelate(BeanWorkFlowRelated wfRelate, NSIndexPath nSIndexPath)
        {
            var index = lstWorkFlowRelateds.FindIndex(item => item.ID == wfRelate.ID);
            if (index != -1)
                lstWorkFlowRelateds.RemoveAt(index);

            table_content_right.ReloadRows(new NSIndexPath[] { nSIndexPath }, UITableViewRowAnimation.Fade);
        }
        public void HandleWorkRelatedSelected(BeanWorkFlowRelated beanWorkFlowRelated, NSIndexPath nSIndexPath)
        {
            var conn = new SQLiteConnection(CmmVariable.M_DataPath);
            string query = string.Format("SELECT * FROM BeanWorkflowItem WHERE ID = ?");
            List<BeanWorkflowItem> lst_result = new List<BeanWorkflowItem>();

            if (beanWorkFlowRelated.ItemID.ToString() != workflowItem.ID)
                lst_result = conn.Query<BeanWorkflowItem>(query, beanWorkFlowRelated.ItemID);

            if (beanWorkFlowRelated.ItemRLID.ToString() != workflowItem.ID)
                lst_result = conn.Query<BeanWorkflowItem>(query, beanWorkFlowRelated.ItemRLID);

            if (lst_result != null && lst_result.Count > 0)
            {
                CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
                CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
                FormWorkFlowDetails detail = (FormWorkFlowDetails)Storyboard.InstantiateViewController("FormWorkFlowDetails");
                bool isTask = false;
                if (currentSelectedItem.ResourceCategoryId == 8)
                    isTask = false;
                else if (currentSelectedItem.ResourceCategoryId == 16)
                    isTask = true;
                detail.SetContent(lst_result[0], isTask, this);
                PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                detail.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;
                detail.ModalPresentationStyle = UIModalPresentationStyle.BlurOverFullScreen;
                detail.TransitioningDelegate = transitioningDelegate;
                this.PresentModalViewController(detail, true);
            }
            else
            {
                CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("MESS_WORKFLOW_NOTFOUND", "Không tìm thấy thông tin phiếu, vui lòng thử lại sau!"));
            }
        }
        public void HandleWorkRelatedResult(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase, int numRow)
        {
            table_content_right.ReloadData();
        }
        #endregion

        #region handle DateTime choice
        public void HandleDateTimeChoiceChoice(ViewElement element)
        {
            UpdateValueElement_InListSection(element);
            table_content_right.ReloadData();
        }
        public void NavigatorToDateTimeChoice(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
        {
            if (element != null)
            {
                FormDateTimeController formDateTimeController = (FormDateTimeController)Storyboard.InstantiateViewController("FormDateTimeController");
                formDateTimeController.setContent(this, element, null);

                CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
                CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
                PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                formDateTimeController.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                formDateTimeController.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                formDateTimeController.TransitioningDelegate = transitioningDelegate;
                this.PresentViewControllerAsync(formDateTimeController, true);
            }
            else
            {
                CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("MESS_WORKFLOW_NOTFOUND", "Không tìm thấy thông tin phiếu, vui lòng thử lại sau!"));
            }
        }
        #endregion

        #region handle Assignment Tasks
        public async void Handle_RemoveTask(BeanTask _task, NSIndexPath nSIndexPath)
        {
            try
            {
                //loading = new CmmLoading(new CGRect((this.View.Bounds.Width - 200) / 2, (this.View.Bounds.Height - 200) / 2, 200, 200), "Đang xử lý...");
                //this.View.Add(loading);
                loading = new CmmLoading(new CGRect((view_task_right.Bounds.Width - 200) / 2, (view_task_right.Bounds.Height - 200) / 2, 200, 200), "Đang xử lý...");
                view_task_right.Add(loading);

                bool res = false;
                await Task.Run(() =>
                {
                    ProviderControlDynamic p_dynamic = new ProviderControlDynamic();
                    res = p_dynamic.DeleteDetailTaskForm(_task.ID);

                    InvokeOnMainThread(() =>
                    {
                        //loading.Hide();

                        if (res)
                            LoadItemSelected(true);
                        else
                        {
                            loading.Hide();
                            CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("K_Mess_ActionFalse", "Thao tác không thực hiện được, vui lòng thử lại."));
                        }
                    });

                });
            }
            catch (Exception ex)
            {
                loading.Hide();
                Console.WriteLine("ToDoDetailsView - Handle_RemoveTask - Err: " + ex.ToString());
            }
        }
        public void Handle_TaskSelected(BeanTask _task, NSIndexPath nSIndexPath)
        {
            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            FormTaskDetails taskDetails = (FormTaskDetails)Storyboard.InstantiateViewController("FormTaskDetails");
            taskDetails.SetContent(_task, workflowItem, this);
            PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            taskDetails.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            taskDetails.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            taskDetails.TransitioningDelegate = transitioningDelegate;
            this.PresentModalViewController(taskDetails, true);
        }
        public void Handle_TaskResult(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase, int numRow)
        {
            table_content_right.ReloadData();
        }
        #endregion

        #region handle proprties details
        public void NavigateToPropertyDetails(ViewElement element, ViewRow datajObject, JObject jObject, int _itemIndex, bool _isAddnew)
        {
            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            FormWFDetailsProperty detailsProperty = (FormWFDetailsProperty)Storyboard.InstantiateViewController("FormWFDetailsProperty");
            detailsProperty.SetContent(element, datajObject, jObject, workflowItem, _itemIndex, this, _isAddnew);
            PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            detailsProperty.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            detailsProperty.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            detailsProperty.TransitioningDelegate = transitioningDelegate;
            this.PresentModalViewController(detailsProperty, true);
        }
        public void HandlePropertyDetailsRemove(JObject jObjectRemove)
        {
            lstGridDetail_Deleted.Add(jObjectRemove);
            table_content_right.ReloadData();
        }
        #endregion

        #region handle Comments
        public void SubmitLikeAction(NSIndexPath sectionIndex, BeanComment comment)
        {
            UpdateTableSections(sectionIndex.Section, comment);
        }

        //Comment - reply
        public void NavigateToReplyComment(NSIndexPath _itemIndex, BeanComment comment)
        {
            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            FormCommentView formComment = (FormCommentView)Storyboard.InstantiateViewController("FormCommentView");
            bool isTask = false;
            if (currentSelectedItem.ResourceCategoryId == 8)
                isTask = false;
            else if (currentSelectedItem.ResourceCategoryId == 16)
                isTask = true;
            formComment.SetContent(this, isTask, workflowItem, comment, _OtherResourceId, _itemIndex);
            PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            formComment.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            formComment.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            formComment.TransitioningDelegate = transitioningDelegate;
            this.PresentModalViewController(formComment, true);
        }
        public void ScrollToCommentViewRow(nfloat estHeight)
        {
            if (estHeight > 420)
                table_content_right.ScrollToRow(NSIndexPath.FromRowSection(lst_section[0].ViewRows.Count - 1, 0), UITableViewScrollPosition.Top, true);
            else
            {
                CGRect keyboardSize = new CGRect(0, 0, 1194, 400);
                CGRect custFrame = table_content_right.Frame;
                custFrame.Y -= keyboardSize.Height;
                table_content_right.Frame = custFrame;
            }
        }
        #endregion

        private async void loadQuaTrinhluanchuyen()
        {
            try
            {
                SQLiteAsyncConnection conn = new SQLiteAsyncConnection(CmmVariable.M_DataPath);
                _ = Task.Run(() =>
                {
                    lst_qtlc = new List<BeanQuaTrinhLuanChuyen>();
                    ProviderControlDynamic p_dynamic = new ProviderControlDynamic();
                    lst_qtlc = p_dynamic.GetListProcessHistory(workflowItem);

                    if (lst_qtlc != null)
                    {
                        List<BeanUser> lst_userResult = new List<BeanUser>();
                        string query_user0 = string.Format("SELECT * FROM BeanUser WHERE ID = ?");

                        //ActionStatusID = 10 || -1: phieu da phe duyet / Huy => Nguoi xu ly buoc truoc se la nguoi tao
                        if (workflowItem.ActionStatusID == 10 || workflowItem.ActionStatusID == -1 || workflowItem.ActionStatusID == 4)
                        {
                            lst_userResult = conn.QueryAsync<BeanUser>(query_user0, workflowItem.CreatedBy.Trim().ToLower()).Result;
                        }
                        else
                        {
                            string userAssignedID = lst_qtlc.OrderBy(t => t.Created).ToList()[0].AssignUserId;
                            lst_userResult = conn.QueryAsync<BeanUser>(query_user0, userAssignedID.Trim().ToLower()).Result;
                        }

                        InvokeOnMainThread(() =>
                        {
                            try
                            {
                                string user_imagePath = "";
                                if (lst_userResult.Count > 0)
                                    user_imagePath = lst_userResult[0].ImagePath;

                                if (string.IsNullOrEmpty(user_imagePath))
                                {
                                    lbl_imgCover.Hidden = false;
                                    img_avatar_sentUnit.Hidden = true;
                                    if (lst_userResult != null && lst_userResult.Count > 0)
                                    {
                                        lbl_imgCover.Text = CmmFunction.GetAvatarName(lst_userResult[0].FullName);
                                        lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));
                                    }
                                    else
                                        lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar("A"));
                                }
                                else
                                {
                                    lbl_imgCover.Hidden = false;
                                    img_avatar_sentUnit.Hidden = true;
                                    lbl_imgCover.Text = CmmFunction.GetAvatarName(lst_userResult[0].FullName);
                                    lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));

                                    checkFileLocalIsExist(lst_userResult[0], img_avatar_sentUnit);
                                    lbl_imgCover.Hidden = true;
                                    img_avatar_sentUnit.Hidden = false;
                                }

                                if (lst_userResult != null && lst_userResult.Count > 0)
                                    lbl_sender.Text = lst_userResult[0].FullName;//+ " (" + lst_userResult[0].Position + ")";

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("ToDoDetailView - loadQuaTrinhluanchuyen-InvokeOnMainThread - Err: " + ex.ToString());
                            }
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("ToDoDetailView - loadQuaTrinhluanchuyen - Err: " + ex.ToString());
            }
        }

        // tinh lai formula cho form
        public void UpdateValueElement_InListSection(ViewElement _element)
        {
            try
            {
                //update element in JObjectSource
                JObjectSource[_element.Title] = _element.Value;

                //calculator
                foreach (ViewSection section in lst_section ?? new List<ViewSection>())
                    foreach (ViewRow row in section.ViewRows ?? new List<ViewRow>())
                        foreach (ViewElement element in row.Elements ?? new List<ViewElement>())
                        {
                            if (!string.IsNullOrEmpty(element.Formula))
                                element.Value = CmmFunction.CalculateObject(element.Formula, JObjectSource).ToString();
                        }
            }
            catch (Exception ex)
            {
                Console.WriteLine("RequestDetailsV2 - UpdateValue_ForElement - Err: " + ex.ToString());
            }
        }

        private void searchData(bool isFromMain = false)
        {
            try
            {
                string content = CmmFunction.removeSignVietnamese(tf_search.Text.Trim().ToLowerInvariant());
                //if (!string.IsNullOrEmpty(content))
                //{
                var items = from item in lst_appBase_cxl
                            where (!string.IsNullOrEmpty(item.Title) && CmmFunction.removeSignVietnamese(item.Title.ToLowerInvariant()).Contains(content)) ||
                                       (!string.IsNullOrEmpty(item.Content) && CmmFunction.removeSignVietnamese(item.Content.ToLowerInvariant()).Contains(content)) ||
                                       (!string.IsNullOrEmpty(item.ListName) && CmmFunction.removeSignVietnamese(item.ListName.ToLowerInvariant()).Contains(content))
                            select item;

                if (items != null && items.Count() > 0)
                {
                    table_todo.Hidden = false;
                    lbl_nodata_left.Hidden = true;
                    lst_notify_cxl_results = items.ToList();
                    if (isFromMain)
                        currentSelectedItem = lst_notify_cxl_results.FirstOrDefault();
                    /*
                    dict_todo_result = new Dictionary<string, List<BeanAppBaseExt>>();
                    if (dict_todo_result.ContainsKey("Today"))
                        dict_todo_result["Today"] = lst_notify_cxl_results;
                    else
                        dict_todo_result.Add("Today", lst_notify_cxl_results);

                    table_todo.Source = new ToDo_TableSource(dict_todo_result, this);
                    table_todo.ReloadData();
                    */

                    isLoadMore = false;
                    //if (!isFilter)
                    //SortListSearchAppBase("", false, lst_notify_cxl_results);
                    //else
                    LoadDataNotifyTypeAppBase(lst_notify_cxl_results, isFilter);
                }
                else
                {
                    isLoadMore = true;
                    table_todo.Hidden = true;
                    lbl_nodata_left.Hidden = false;

                    table_todo.Source = null;
                    table_todo.ReloadData();
                }
                //}
                //else
                //{
                //    table_todo.Hidden = false;
                //    lbl_nodata_left.Hidden = true;

                //    table_todo.Source = new ToDo_TableSource(dict_todo, this);
                //    table_todo.ReloadData();
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine("ToDoDetailView - Tf_search_EditingChanged - Err: " + ex.ToString());
            }
        }

        private void menu_action_Toggle()
        {
            try
            {
                CloseAddFollow();

                Custom_MenuAction custom_menuAction = Custom_MenuAction.Instance;
                if (custom_menuAction.Superview != null)
                    custom_menuAction.RemoveFromSuperview();
                else
                {
                    int cell_height = 35;
                    int maxheight = lst_menuItem.Count * cell_height;

                    custom_menuAction.ItemNoIcon = false;
                    custom_menuAction.viewController = this;
                    custom_menuAction.InitFrameView(new CGRect(this.view_task_right.Frame.Width - (205 + 25), 40, 205, maxheight + 3));
                    custom_menuAction.AddShadowForView();
                    custom_menuAction.ListItemMenu = lst_menuItem;
                    custom_menuAction.TableLoadData();

                    view_task_right.AddSubview(custom_menuAction);
                    view_task_right.BringSubviewToFront(custom_menuAction);
                }
            }
            catch (Exception ex)
            { Console.WriteLine(ex.ToString()); }
        }

        public void HandleMenuOptionResult(ButtonAction action)
        {
            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);

            switch (action.ID)
            {
                case (int)WorkflowAction.Action.Next:       // 1- Duyệt
                case (int)WorkflowAction.Action.Approve:    // 2- phe duyet
                case (int)WorkflowAction.Action.Return:     // 4 - Yêu cầu hiệu chỉnh
                case (int)WorkflowAction.Action.Reject:     // 5 - Từ chối
                case (int)WorkflowAction.Action.Idea:       // 10 - cho y kien
                case (int)WorkflowAction.Action.Cancel:     // 51 -  Huy
                    CloseAddFollow();
                    FormApproveOrRejectView formApproveOrReject = (FormApproveOrRejectView)Storyboard.InstantiateViewController("FormApproveOrRejectView");
                    formApproveOrReject.SetContent(this, action);
                    transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                    formApproveOrReject.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                    formApproveOrReject.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                    formApproveOrReject.TransitioningDelegate = transitioningDelegate;
                    this.PresentViewControllerAsync(formApproveOrReject, true);
                    break;
                case (int)WorkflowAction.Action.Forward: // 3 - chuyen xmou ly
                    FormTransferHandleView formTransferHandleView = (FormTransferHandleView)Storyboard.InstantiateViewController("FormTransferHandleView");
                    formTransferHandleView.SetContent(this, action);
                    transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                    formTransferHandleView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                    formTransferHandleView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                    formTransferHandleView.TransitioningDelegate = transitioningDelegate;
                    this.PresentViewControllerAsync(formTransferHandleView, true);
                    break;
                case (int)WorkflowAction.Action.Recall: // 6 - thu hoi
                    SubmitAction(action, null);
                    break;
                case (int)WorkflowAction.Action.RequestInformation: // 7 - yeu cau bo sung
                    FormAdditionalInformationView requestAddInfo = (FormAdditionalInformationView)Storyboard.InstantiateViewController("FormAdditionalInformationView");
                    requestAddInfo.SetContent(this, action, lst_qtlc, workflowItem, null);
                    transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                    requestAddInfo.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                    requestAddInfo.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                    requestAddInfo.TransitioningDelegate = transitioningDelegate;
                    this.PresentViewControllerAsync(requestAddInfo, true);
                    break;
                case (int)WorkflowAction.Action.RecallAfterApproved: // 8 - Thu hồi đã phê duyệt
                    break;
                case (int)WorkflowAction.Action.RequestIdea: // 9 - xin y kien tham van
                    FormTransferHandleView RequestIdea = (FormTransferHandleView)Storyboard.InstantiateViewController("FormTransferHandleView");
                    RequestIdea.SetContent(this, action);
                    transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                    RequestIdea.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                    RequestIdea.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                    RequestIdea.TransitioningDelegate = transitioningDelegate;
                    this.PresentViewControllerAsync(RequestIdea, true);
                    break;
                case (int)WorkflowAction.Action.Save: // 11 -  luu
                    SubmitAction(action, null);
                    break;
                case (int)WorkflowAction.Action.Submit: // 12 -  Gửi
                                                        //SubmitAction(action, null);
                    break;
                case (int)WorkflowAction.Action.Share: // 14 -  share
                    break;
                case (int)WorkflowAction.Action.CreateTask: // 54 -  Phan cong xu ly
                    FormCreateTaskView createtask = (FormCreateTaskView)Storyboard.InstantiateViewController("FormCreateTaskView");
                    createtask.SetContent(workflowItem, null, this);
                    createtask.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                    createtask.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                    createtask.TransitioningDelegate = transitioningDelegate;
                    this.PresentModalViewController(createtask, true);
                    break;
            }

            CloseAddFollow();
            CloseMenuOption();
        }

        public void HandleButtonBot(ViewElement element)
        {

            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);

            if (element.ID != "more")
            {
                int ID_action = Convert.ToInt32(element.ID);
                ButtonAction action = new ButtonAction();
                action.ID = ID_action;
                action.Title = element.Title;
                action.Value = element.Value;
                action.Notes = element.Notes;

                switch (action.ID)
                {
                    case (int)WorkflowAction.Action.Next:       // 1- Duyệt
                    case (int)WorkflowAction.Action.Approve:    // 2- phe duyet
                    case (int)WorkflowAction.Action.Return:     // 4 - Yêu cầu hiệu chỉnh
                    case (int)WorkflowAction.Action.Reject:     // 5 - Từ chối
                    case (int)WorkflowAction.Action.Idea:       // 10 - cho y kien
                    case (int)WorkflowAction.Action.Cancel:     // 51 -  Huy
                        CloseAddFollow();
                        FormApproveOrRejectView formApproveOrReject = (FormApproveOrRejectView)Storyboard.InstantiateViewController("FormApproveOrRejectView");
                        formApproveOrReject.SetContent(this, action);
                        PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        formApproveOrReject.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        formApproveOrReject.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        formApproveOrReject.TransitioningDelegate = transitioningDelegate;
                        this.PresentViewControllerAsync(formApproveOrReject, true);
                        break;
                    case (int)WorkflowAction.Action.Forward: // 3 - chuyen xu ly
                        FormTransferHandleView formTransferHandleView = (FormTransferHandleView)Storyboard.InstantiateViewController("FormTransferHandleView");
                        formTransferHandleView.SetContent(this, action);
                        transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        formTransferHandleView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        formTransferHandleView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        formTransferHandleView.TransitioningDelegate = transitioningDelegate;
                        this.PresentViewControllerAsync(formTransferHandleView, true);
                        break;
                    case (int)WorkflowAction.Action.Recall: // 6 - thu hoi
                        SubmitAction(action, null);
                        break;
                    case (int)WorkflowAction.Action.RequestInformation: // 7 - yeu cau bo sung
                        FormAdditionalInformationView additionalInformationView = (FormAdditionalInformationView)Storyboard.InstantiateViewController("FormAdditionalInformationView");
                        additionalInformationView.SetContent(this, action, lst_qtlc, workflowItem, null);
                        transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        additionalInformationView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        additionalInformationView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        additionalInformationView.TransitioningDelegate = transitioningDelegate;
                        this.PresentViewControllerAsync(additionalInformationView, true);
                        break;
                    case (int)WorkflowAction.Action.RecallAfterApproved: // 8 - Thu hồi đã phê duyệt
                        break;
                    case (int)WorkflowAction.Action.RequestIdea: // 9 - xin y kien tham van
                        FormTransferHandleView RequestIdea = (FormTransferHandleView)Storyboard.InstantiateViewController("FormTransferHandleView");
                        RequestIdea.SetContent(this, action);
                        transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        RequestIdea.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        RequestIdea.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        RequestIdea.TransitioningDelegate = transitioningDelegate;
                        this.PresentViewControllerAsync(RequestIdea, true);
                        break;

                    case (int)WorkflowAction.Action.Save: // 11 -  luu
                        SubmitAction(action, null);
                        break;
                    case (int)WorkflowAction.Action.Submit: // 12 -  Gửi
                                                            //SubmitAction(action, null);
                        break;
                    case (int)WorkflowAction.Action.Share: // 14 -  share
                        break;
                    case (int)WorkflowAction.Action.CreateTask: // 54 -  Phan cong xu ly
                        FormCreateTaskView createtask = (FormCreateTaskView)Storyboard.InstantiateViewController("FormCreateTaskView");
                        createtask.SetContent(workflowItem, null, this);
                        transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        createtask.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        createtask.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        createtask.TransitioningDelegate = transitioningDelegate;
                        this.PresentModalViewController(createtask, true);
                        break;
                }
            }
            else
            {
                menu_action_Toggle();
            }
        }

        // Thuc hien action tu cac popup hoac form
        public async void SubmitAction(ButtonAction _buttonAction, List<KeyValuePair<string, string>> _lstExtent)
        {
            //loading = new CmmLoading(new CGRect((this.View.Bounds.Width - 200) / 2, (this.View.Bounds.Height - 200) / 2, 200, 200), "Đang xử lý...");
            //this.View.Add(loading);
            loading = new CmmLoading(new CGRect((view_task_right.Bounds.Width - 200) / 2, (view_task_right.Bounds.Height - 200) / 2, 200, 200), "Đang xử lý...");
            view_task_right.Add(loading);

            try
            {
                Custom_MenuAction custom_menuAction = Custom_MenuAction.Instance;
                if (custom_menuAction.Superview != null)
                    custom_menuAction.RemoveFromSuperview();

                var temp = JsonConvert.SerializeObject(lst_section);
                List<ObjectSubmitAction> lst_object_edit = new List<ObjectSubmitAction>();
                foreach (var row in lst_section[0].ViewRows)
                {
                    foreach (var element in row.Elements)
                    {
                        if (element.Enable == true)
                        {
                            if (element.DataType == "inputattachmenthorizon")
                            {
                                //Attachment add new or edit
                                ObjectSubmitAction ob = new ObjectSubmitAction();
                                ob.ID = element.ID;
                                ob.Value = element.Value;
                                ob.TypeSP = "Attachment";
                                ob.DataType = element.DataType;
                                lst_object_edit.Add(ob);

                                //Attachment remove
                                if (!string.IsNullOrEmpty(json_attachRemove))
                                {
                                    ObjectSubmitAction ob_remove = new ObjectSubmitAction();
                                    ob_remove.ID = element.ID;
                                    ob_remove.Value = json_attachRemove;
                                    ob_remove.TypeSP = "RemoveAttachment";
                                    ob_remove.DataType = element.DataType;
                                    lst_object_edit.Add(ob_remove);
                                }
                            }
                            else if (element.DataType == "inputgriddetails")
                            {
                                //property detail add new or edit
                                ObjectSubmitAction ob = new ObjectSubmitAction();
                                ob.ID = element.ID;
                                ob.Value = element.Value;
                                ob.TypeSP = "GridDetails";
                                ob.DataType = element.DataType;
                                lst_object_edit.Add(ob);

                                //property detail remove
                                if (lstGridDetail_Deleted != null && lstGridDetail_Deleted.Count > 0)
                                {
                                    json_PropertyRemove = JsonConvert.SerializeObject(lstGridDetail_Deleted);

                                    ObjectSubmitAction ob_remove = new ObjectSubmitAction();
                                    ob_remove.ID = element.ID;
                                    ob_remove.Value = json_PropertyRemove;
                                    ob_remove.TypeSP = "RemoveGridDetails";
                                    ob_remove.DataType = element.DataType;
                                    lst_object_edit.Add(ob_remove);
                                }
                            }
                            else if (element.DataType != "inputcomments")
                            {
                                ObjectSubmitAction ob = new ObjectSubmitAction();
                                ob.ID = element.ID;
                                ob.Value = element.Value;
                                ob.TypeSP = element.TypeSP;
                                ob.DataType = element.DataType;
                                lst_object_edit.Add(ob);
                            }
                        }
                    }
                }

                string json_edit_element = JsonConvert.SerializeObject(lst_object_edit);

                await Task.Run(() =>
                {
                    bool result = false;
                    ProviderBase b_pase = new ProviderBase();
                    ProviderControlDynamic providerControl = new ProviderControlDynamic();

                    string json = JsonConvert.SerializeObject(dic_valueObject);

                    lstExtent = _lstExtent;

                    List<KeyValuePair<string, string>> lst_files = new List<KeyValuePair<string, string>>();
                    if (lst_addAttachment != null && lst_addAttachment.Count > 0)
                    {
                        foreach (var item in lst_addAttachment)
                        {
                            if (string.IsNullOrEmpty(item.ID))
                            {
                                string key = item.Title;
                                KeyValuePair<string, string> img_info = new KeyValuePair<string, string>(key, item.Path);
                                lst_files.Add(img_info);
                            }
                        }
                    }

                    string str_errMess = string.Empty;
                    if (lstExtent != null && lstExtent.Count > 0)
                        result = providerControl.SendControlDynamicAction(_buttonAction.Value, workflowItem.ID.ToString(), str_json_FormDefineInfo, json_edit_element, ref str_errMess, lst_files, lstExtent);
                    else
                        result = providerControl.SendControlDynamicAction(_buttonAction.Value, workflowItem.ID.ToString(), str_json_FormDefineInfo, json_edit_element, ref str_errMess, lst_files, null);

                    if (result)
                    {
                        //b_pase.UpdateSyncBackgroundData3(true);
                        b_pase.UpdateAllDynamicData(true);
                        InvokeOnMainThread(() =>
                        {
                            try
                            {
                                //loading.Hide();
                                // currentItemSelected = null, auto focus to item first
                                if (_buttonAction.Value != "Follow" && _buttonAction.Value != "Save")
                                {
                                    //currentItemSelected = null;
                                    ReLoadDataFromServer(true, true);
                                    //LoadDataFilterTodo();
                                    //LoadDataFilterTodo(status_selected_index, duedate_selected_index, fromDate_default, toDate_default);                                   
                                    table_todo.ScrollRectToVisible(new CGRect(0, 0, 0, 0), false); //table_todo.ScrollToRow(NSIndexPath.FromRowSection(0, 0), UITableViewScrollPosition.Top, true);
                                }
                                else
                                {

                                    ReLoadDataFromServer(false, false);
                                    LoadDataFilterTodo();
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("TodoDetailsView - SubmitAction - Invoke - Err: " + ex.ToString());
                            }

                        });
                    }
                    else
                    {
                        InvokeOnMainThread(() =>
                        {
                            //loading.Hide();
                            CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("K_Mess_ActionFalse", "Thao tác không thực hiện được, vui lòng thử lại."));
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                //loading.Hide();
                Console.WriteLine("ViewRequestDetails - submitAction - ERR: " + ex.ToString());
            }
            finally
            {
                loading.Hide();
            }
        }

        /// <summary>
        /// Cap nhat lai Danh sach Data / Form
        /// </summary>
        /// <param name="isChangeFocus"></param>
        public void ReloadDataForm(bool isChangeFocus)
        {
            if (isChangeFocus)
            {
                // currentItemSelected = null, auto focus to item first
                currentSelectedItem = null;
            }

            LoadDataFilterTodo();

            //LoadDataFilterTodo(status_selected_index, duedate_selected_index, fromDate_default, toDate_default);
            LoadItemSelected(true);
        }

        #endregion

        #region event
        private void BT_avatar_TouchUpInside(object sender, EventArgs e)
        {
            appD.menu.UpdateItemSelect(1);
            appD.SlideMenuController.OpenLeft();

            CloseAddFollow();
        }

        private void BT_follow_TouchUpInside(object sender, EventArgs e)
        {
            Custom_AddFollowView custom_AddFollowView = Custom_AddFollowView.Instance;
            if (custom_AddFollowView.Superview != null && custom_AddFollowView.viewController.GetType() == typeof(ToDoDetailView))
                custom_AddFollowView.RemoveFromSuperview();
            else
            {
                if (isFollow)
                {
                    var width = StringExtensions.MeasureString(CmmFunction.GetTitle("MESS_UNFOLLOW_TASK", "Hủy theo dõi công việc này"), 14).Width + 20 + 70;
                    custom_AddFollowView.viewController = this;
                    custom_AddFollowView.isFollow = isFollow;
                    custom_AddFollowView.LoadContent();
                    custom_AddFollowView.InitFrameView(new CGRect(this.view_task_right.Frame.Right - (width + 95), view_top_bar.Frame.Bottom, width, 56));

                    this.View.AddSubview(custom_AddFollowView);
                    this.View.BringSubviewToFront(custom_AddFollowView);
                }
                else
                {
                    var width = StringExtensions.MeasureString(CmmFunction.GetTitle("MESS_FOLLOW_TASK", "Đặt theo dõi công việc này"), 14).Width + 20 + 70;
                    custom_AddFollowView.viewController = this;
                    custom_AddFollowView.isFollow = isFollow;
                    custom_AddFollowView.LoadContent();
                    custom_AddFollowView.InitFrameView(new CGRect(this.view_task_right.Frame.Right - (width + 95), view_top_bar.Frame.Bottom, width, 56));

                    this.View.AddSubview(custom_AddFollowView);
                    this.View.BringSubviewToFront(custom_AddFollowView);
                }
            }
        }

        private void BT_comment_TouchUpInside(object sender, EventArgs e)
        {
            try
            {
                CloseAddFollow();
                table_content_right.ScrollToRow(NSIndexPath.FromRowSection(lst_section[0].ViewRows.Count - 1, 0), UITableViewScrollPosition.Top, true);

                //CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
                //CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
                //FormCommentView formComment = (FormCommentView)Storyboard.InstantiateViewController("FormCommentView");
                //formComment.SetContent(this, workflowItem, str_json_FormDefineInfo, lst_comments, _OtherResourceId);
                //PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                //formComment.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                //formComment.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                //formComment.TransitioningDelegate = transitioningDelegate;
                //this.PresentViewControllerAsync(formComment, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ToDoView - BT_comment_TouchUpInside - Err: " + ex.ToString());
            }

        }

        private void BT_share_TouchUpInside(object sender, EventArgs e)
        {
            CloseAddFollow();

            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            FormShareView formShareView = (FormShareView)Storyboard.InstantiateViewController("FormShareView");
            formShareView.SetContent(this, workflowItem, str_json_FormDefineInfo);
            PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            formShareView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            formShareView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            formShareView.TransitioningDelegate = transitioningDelegate;
            this.PresentViewControllerAsync(formShareView, true);
        }

        private void BT_history_TouchUpInside(object sender, EventArgs e)
        {
            CloseAddFollow();

            Custom_WorkFlowView custom_WorkFollowView = Custom_WorkFlowView.Instance;
            custom_WorkFollowView.InitFrameView(new CGRect(10, 0, view_task_right.Frame.Width, view_task_right.Frame.Height));
            custom_WorkFollowView.viewController = this;
            custom_WorkFollowView.list_QTLC = lst_qtlc;
            custom_WorkFollowView.TableLoadData();

            view_task_right.AddSubview(custom_WorkFollowView);

            custom_WorkFollowView.Frame = new CGRect(view_task_right.Frame.Right, 0, view_task_right.Frame.Width, view_task_right.Frame.Height);
            UIView.BeginAnimations("show_animationShowTable");
            UIView.SetAnimationDuration(0.3f);
            UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
            UIView.SetAnimationRepeatCount(0);
            UIView.SetAnimationRepeatAutoreverses(false);
            UIView.SetAnimationDelegate(this);
            custom_WorkFollowView.Frame = new CGRect(10, 0, view_task_right.Frame.Width, view_task_right.Frame.Height);
            UIView.CommitAnimations();
        }

        private void BT_attachement_TouchUpInside(object sender, EventArgs e)
        {
            CloseAddFollow();
            if (lst_attachFile != null && lst_attachFile.Count > 0)
            {
                CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
                CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
                ListAttachmentView attachmentView = (ListAttachmentView)Storyboard.InstantiateViewController("ListAttachmentView");
                attachmentView.SetContent(lst_attachFile, workflowItem.Content, this, attachmentElement);
                PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                attachmentView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                attachmentView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                attachmentView.TransitioningDelegate = transitioningDelegate;
                this.PresentViewControllerAsync(attachmentView, true);
            }
        }

        private void BT_create_TouchUpInside(object sender, EventArgs e)
        {
            CloseAddFollow();

            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            FormCreateView formCreateView = (FormCreateView)Storyboard.InstantiateViewController("FormCreateView");
            PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            formCreateView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            formCreateView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            formCreateView.TransitioningDelegate = transitioningDelegate;
            this.PresentViewControllerAsync(formCreateView, true);
        }

        private void BT_search_TouchUpInside(object sender, EventArgs e)
        {
            if (isSearch)
            {
                UIView.BeginAnimations("show_animationShowTable");
                UIView.SetAnimationDuration(0.5f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);
                constraint_rightBT_Search.Constant = origin_constraint_rightBT_Search;
                BT_search.SetImage(UIImage.FromFile("Icons/icon_search.png"), UIControlState.Normal);
                UIView.CommitAnimations();
                view_segments.Hidden = false;
                isSearch = false;

                this.View.EndEditing(true);
                if (!string.IsNullOrEmpty(tf_search.Text))
                    BT_search.TintColor = UIColor.Orange;
                else
                    BT_search.TintColor = UIColor.Black;
            }
            else
            {
                view_segments.Hidden = true;
                constraint_rightBT_Search.Constant = origin_constraint_rightBT_Search;
                UIView.BeginAnimations("show_animationShowTable");
                UIView.SetAnimationDuration(0.5f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);
                constraint_rightBT_Search.Constant = (view_taskLeft_top.Frame.Width - BT_filter.Frame.Width) - 5;
                BT_search.SetImage(UIImage.FromFile("Icons/icon_backSearch.png"), UIControlState.Normal);
                UIView.CommitAnimations();

                BT_search.TintColor = UIColor.Black;
                isSearch = true;
            }
            tf_search.Hidden = !isSearch;
        }

        private void Tf_search_EditingChanged(object sender, EventArgs e)
        {
            searchData();
        }

        private void BT_filter_TouchUpInside(object sender, EventArgs e)
        {
            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(450, 450);

            UINavigationController nav = new UINavigationController();
            nav.PrefersStatusBarHidden();
            nav.NavigationBarHidden = true;
            nav.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;

            if (tab_Dangxuly)
            {
                lst_trangthai[0].isSelected = true;
                lst_trangthai[1].isSelected = false;
            }
            else
            {
                lst_trangthai[0].isSelected = false;
                lst_trangthai[1].isSelected = true;
            }

            FormFillterToDoView formFillterToDoView = (FormFillterToDoView)Storyboard.InstantiateViewController("FormFillterToDoView");
            formFillterToDoView.SetContent(this, lst_trangthai, lst_appStatus, lst_dueDateMenu, isFilter);
            //formFillterToDoView.SetContent(this,lst_trangthai, lst_appStatus, lst_dueDateMenu);
            nav.AddChildViewController(formFillterToDoView);

            this.PresentModalViewController(nav, true);
        }

        public async void HandleAddFollow()
        {
            Custom_AddFollowView view_follow = Custom_AddFollowView.Instance;
            ButtonAction bt_follow = new ButtonAction();
            bt_follow.Value = "Follow";
            List<KeyValuePair<string, string>> _lstExtent = new List<KeyValuePair<string, string>>();
            _lstExtent.Add(new KeyValuePair<string, string>("status", workflowItem.IsFollow ? "0" : "1"));

            SubmitAction(bt_follow, _lstExtent);

            if (view_follow.Superview != null)
                view_follow.RemoveFromSuperview();
        }

        public void HandleWorkFollowViewResult()
        {
            Custom_WorkFlowView custom_WorkFollowView = Custom_WorkFlowView.Instance;
            if (custom_WorkFollowView.Superview != null)
            {
                var delayTimer = new Timer((state) => InvokeOnMainThread(() => custom_WorkFollowView.RemoveFromSuperview()), null, 300, Timeout.Infinite);

                custom_WorkFollowView.Frame = new CGRect(0, 0, view_task_right.Frame.Width, view_task_right.Frame.Height);
                UIView.BeginAnimations("show_animationShowTable");
                UIView.SetAnimationDuration(0.3f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);
                custom_WorkFollowView.Frame = new CGRect(view_task_right.Frame.Right, 0, view_task_right.Frame.Width, view_task_right.Frame.Height);
                UIView.CommitAnimations();
            }
        }

        public void HandleSectionTable(nint section, string key, int tableIndex)
        {
            dict_sectionTodo[key] = !dict_sectionTodo[key];
            table_todo.ReloadSections(new NSIndexSet((uint)section), UITableViewRowAnimation.None);
        }

        private async void Todo_refreshControl_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                todo_refreshControl.BeginRefreshing();
                ProviderBase provider = new ProviderBase();
                await Task.Run(() =>
                {
                    provider.UpdateAllMasterData(true);
                    //provider.UpdateSyncBackgroundData3(true);
                    provider.UpdateAllDynamicData(true);

                    InvokeOnMainThread(() =>
                    {
                        if (isFilter)
                            LoadDataFilterFromServer(false);
                        else
                            LoadDataFilterTodo();

                        //LoadDataFilterTodo(status_selected_index, duedate_selected_index, fromDateSelected, toDateSelected);
                        todo_refreshControl.EndRefreshing();
                    });
                });
            }
            catch (Exception ex)
            {
                todo_refreshControl.EndRefreshing();
                Console.WriteLine("Error - ToDoDetailView - refreshControl_valuechange - Er: " + ex.ToString());
            }
        }

        private void BT_moreUser_TouchUpInside(object sender, EventArgs e)
        {
            if (arr_assignedTo != null && arr_assignedTo.Length > 1)
            {
                if (!isExpandUser)
                {
                    lbl_assignedTo.LineBreakMode = UILineBreakMode.WordWrap;
                    isExpandUser = true;
                    string combindedStringUser = string.Join(", ", lst_userName);
                    lbl_assignedTo.Text = CmmFunction.GetTitle("TEXT_TO", "Đến: ") + combindedStringUser;

                    var heightRequire = StringExtensions.StringHeight(lbl_assignedTo.Text, UIFont.FromName("ArialMT", 12), lbl_assignedTo.Frame.Width);
                    if (heightRequire > 20)
                    {
                        view_header_content_height_constant.Constant = origin_view_header_content_height_constant + 20;
                        lbl_assignedTo.Lines = 2;
                    }

                    if (heightRequire > 40)
                    {
                        view_header_content_height_constant.Constant = origin_view_header_content_height_constant + 40;
                        lbl_assignedTo.Lines = 3;
                    }
                }
                else
                {
                    isExpandUser = false;
                    view_header_content_height_constant.Constant = origin_view_header_content_height_constant;
                    string assignedToSample = string.Join(", ", lst_userName);
                    //var widthStatus = StringExtensions.MeasureString(workflowItem.AssignedToName, 12).Width + 20;

                    var maxStatusWidth = (lbl_assignedTo.Frame.Width / 3) * 2;

                    nfloat temp_width = 0;
                    if (!string.IsNullOrEmpty(assignedToSample))
                    {
                        string[] users = assignedToSample.Split(',');
                        string res = string.Empty;

                        if (users.Length > 1)
                        {
                            int num_remain = users.Length - 1;
                            res = users[0] + ", +" + num_remain.ToString();
                        }
                        else
                            res = users[0];

                        if (!string.IsNullOrEmpty(res))
                            res = CmmFunction.GetTitle("TEXT_TO", "Đến: ") + res;

                        if (res.Contains('+'))
                        {
                            var indexA = res.IndexOf('+');
                            NSMutableAttributedString att = new NSMutableAttributedString(res);
                            att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(94, 94, 94), new NSRange(0, res.Length));
                            att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(65, 80, 134), new NSRange(indexA, res.Length - indexA));
                            att.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(lbl_assignedTo.Font.PointSize, UIFontWeight.Semibold), new NSRange(indexA, res.Length - indexA));
                            lbl_assignedTo.AttributedText = att;//(att, UIControlState.Normal);
                        }
                        else
                            lbl_assignedTo.Text = res; // nguoi nhan
                    }
                }
            }
        }

        private void CmmEvent_UpdateLangComplete(object sender, CmmEvent.UpdateEventArgs e)
        {
            try
            {
                InvokeOnMainThread(() =>
                {
                    if (loading != null)
                        loading.Hide();

                    LoadTrangThaiCategory();
                    LoadStatusCategory();
                    LoadDueDateCategory();

                    //lbl_topBar_title.Text = CmmFunction.GetTitle("TEXT_IPAD_TOME", "Trang chủ  >  Đến tôi");
                    tf_search.Placeholder = CmmFunction.GetTitle("TEXT_SEARCH", "Tìm kiếm...");
                    //(status_selected_index, duedate_selected_index, fromDateSelected, toDateSelected);
                    todo_refreshControl.AttributedTitle = new NSAttributedString(CmmFunction.GetTitle("TEXT_REFRESH_LOADING", "Loading ..."), firstAttributes);

                    LoadDataFilterTodo();
                    LoadItemSelected(true);

                    SetLangTitle();
                    ToggleTodo();

                    Custom_MenuAction custom_menuAction = Custom_MenuAction.Instance;
                    if (custom_menuAction.Superview != null)
                        custom_menuAction.RemoveFromSuperview();

                    //loadQuaTrinhluanchuyen();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("ToDoDetailView - CmmEvent_UpdateLangComplete - ERR: " + ex.ToString());
            }
        }

        private void BT_DangXL_TouchUpInside(object sender, EventArgs e)
        {
            tab_Dangxuly = true;
            currentSelectedItem = null;
            ToggleTodo();
            LoadDataFilterTodo(true);
            //if (isFilter)
            //    LoadDataFilterFromServer(false);
            //else
            //    LoadDataFilterTodo(true);
        }

        private void BT_DaXL_TouchUpInside(object sender, EventArgs e)
        {
            tab_Dangxuly = false;
            currentSelectedItem = null;
            ToggleTodo();
            LoadDataFilterTodo(true);
            //if (isFilter)
            //    LoadDataFilterFromServer(false);
            //else
            //    LoadDataFilterTodo(true);
        }

        private void ToggleTodo()
        {
            if (tab_Dangxuly) // dang la trang thai todo cua toi
            {
                tab_Dangxuly = true;
                BT_DangXL.BackgroundColor = UIColor.White;
                BT_DangXL.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);
                BT_DaXL.BackgroundColor = UIColor.Clear;
                BT_DaXL.SetTitleColor(UIColor.White, UIControlState.Normal);

                BT_DangXL.Layer.ShadowOpacity = 0.5f;
                BT_DaXL.Layer.ShadowOpacity = 0.0f;

                ButtonMenuStyleChange(BT_DangXL, true, 0);
                ButtonMenuStyleChange(BT_DaXL, false, 1);
            }
            else
            {
                tab_Dangxuly = false;
                BT_DangXL.BackgroundColor = UIColor.Clear;
                BT_DangXL.SetTitleColor(UIColor.White, UIControlState.Normal);
                BT_DaXL.BackgroundColor = UIColor.White;
                BT_DaXL.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);

                BT_DangXL.Layer.ShadowOpacity = 0.0f;
                BT_DaXL.Layer.ShadowOpacity = 0.5f;

                ButtonMenuStyleChange(BT_DangXL, false, 0);
                ButtonMenuStyleChange(BT_DaXL, true, 1);
            }
        }

        private void KeyBoardUpNotification(NSNotification notification)
        {
            try
            {
                if (this.PresentedViewController != null)
                {
                    if (this.PresentedViewController.GetType() == typeof(NumberControlView)) { }
                    else if (this.PresentedViewController.GetType() == typeof(FormUserAndGroupView)) { }
                    else if (this.PresentedViewController.GetType() == typeof(FormApproveOrRejectView)) { }
                    else if (this.PresentedViewController.GetType() == typeof(FormEditTextController)) { }
                    else
                    {
                        if (tf_search.IsFirstResponder)
                            isShowKeyBoarFromComment = false;
                        else
                            isShowKeyBoarFromComment = true;

                        CGRect keyboardSize = UIKeyboard.BoundsFromNotification(notification);
                        if (estCommmentViewRowHeight > keyboardSize.Height)
                            table_content_right.ScrollToRow(NSIndexPath.FromRowSection(lst_section[0].ViewRows.Count - 1, 0), UITableViewScrollPosition.Top, true);
                        else
                        {
                            table_content_right.ScrollToRow(NSIndexPath.FromRowSection(lst_section[0].ViewRows.Count - 1, 0), UITableViewScrollPosition.Bottom, true);

                            CGPoint point = table_content_right.ContentOffset;
                            var heightContent_noKeyboard = table_content_right.ContentSize.Height - keyboardSize.Height;
                            var heightRemain = keyboardSize.Height - estCommmentViewRowHeight;
                            point.Y = (heightContent_noKeyboard + heightRemain) - 50;
                            table_content_right.ContentOffset = point;
                        }
                    }
                }
                else
                {
                    if (tf_search.IsFirstResponder)
                        isShowKeyBoarFromComment = false;
                    else
                        isShowKeyBoarFromComment = true;

                    CGRect keyboardSize = UIKeyboard.BoundsFromNotification(notification);
                    if (estCommmentViewRowHeight > keyboardSize.Height)
                        table_content_right.ScrollToRow(NSIndexPath.FromRowSection(lst_section[0].ViewRows.Count - 1, 0), UITableViewScrollPosition.Top, true);
                    else
                    {
                        table_content_right.ScrollToRow(NSIndexPath.FromRowSection(lst_section[0].ViewRows.Count - 1, 0), UITableViewScrollPosition.Bottom, true);

                        CGPoint point = table_content_right.ContentOffset;
                        var heightContent_noKeyboard = table_content_right.ContentSize.Height - keyboardSize.Height;
                        var heightRemain = keyboardSize.Height - estCommmentViewRowHeight;
                        point.Y = (heightContent_noKeyboard + heightRemain) - 50;
                        table_content_right.ContentOffset = point;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Todo - Err: " + ex.ToString());
            }
        }

        private void KeyBoardDownNotification(NSNotification notification)
        {
            try
            {
                if (isShowKeyBoarFromComment)
                {
                    table_content_right.ScrollToRow(NSIndexPath.FromRowSection(lst_section[0].ViewRows.Count - 1, 0), UITableViewScrollPosition.Top, true);
                    if (table_content_right.Frame.Y != 0)
                    {
                        CGRect custFrame = table_content_right.Frame;
                        custFrame.Y = 90;
                        table_content_right.Frame = custFrame;
                    }
                }
            }
            catch (Exception ex)
            { Console.WriteLine("StartView - Err: " + ex.ToString()); }
        }

        /// <summary>
        /// sort section from search
        /// </summary>
        /// <param name="query"></param>
        private void SortListSearchAppBase(string query, bool isOnline, List<BeanAppBaseExt> lstSearch)
        {
            var dict = new Dictionary<string, List<BeanAppBaseExt>>();
            dict = new Dictionary<string, List<BeanAppBaseExt>>();
            if (currentSelectedItem != null)
                currentSelectedItem.IsSelected = true;
            else
                currentSelectedItem = lstSearch[0];

            string KEY_TODAY = CmmFunction.GetTitle("TEXT_TODAY", "Hôm nay");
            string KEY_YESTERDAY = CmmFunction.GetTitle("TEXT_YESTERDAY", "Hôm qua");
            string KEY_ORTHER = CmmFunction.GetTitle("TEXT_OLDER", "Cũ hơn");

            List<BeanAppBaseExt> lst_temp1 = new List<BeanAppBaseExt>();
            List<BeanAppBaseExt> lst_temp2 = new List<BeanAppBaseExt>();
            List<BeanAppBaseExt> lst_temp3 = new List<BeanAppBaseExt>();
            dict.Add(KEY_TODAY, lst_temp1);
            dict.Add(KEY_YESTERDAY, lst_temp2);
            dict.Add(KEY_ORTHER, lst_temp3);

            foreach (var item in lstSearch)
            {
                if (item.ID == currentSelectedItem.ID)
                    item.IsSelected = true;
                else
                    item.IsSelected = false;

                // Dang Xu Ly
                if (tab_Dangxuly)
                {
                    if (item.ID == currentSelectedItem.ID)
                        item.IsSelected = true;
                    else
                        item.IsSelected = false;

                    if (isOnline) // server sort theo NotifyCreated
                    {
                        if (item.NotifyCreated.HasValue)
                        {
                            if (item.NotifyCreated.Value.Date == DateTime.Now.Date) // today
                            {
                                List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                                if (dict.ContainsKey(KEY_TODAY))
                                    dict[KEY_TODAY].Add(item);
                                else
                                    dict.Add(KEY_TODAY, lst_temp);
                            }
                            else if (item.NotifyCreated.Value.Date == DateTime.Now.AddDays(-1).Date) // hom qua
                            {
                                List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                                if (dict.ContainsKey(KEY_YESTERDAY))
                                    dict[KEY_YESTERDAY].Add(item);
                                else
                                    dict.Add(KEY_YESTERDAY, lst_temp);
                            }
                            else if (item.NotifyCreated.Value.Date < DateTime.Now.AddDays(-1).Date) // cuhon
                            {
                                List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                                if (dict.ContainsKey(KEY_ORTHER))
                                    dict[KEY_ORTHER].Add(item);
                                else
                                    dict.Add(KEY_ORTHER, lst_temp);
                            }
                        }
                    }
                    //local sort theo StartDate
                    else
                    {
                        if (item.StartDate.HasValue)
                        {
                            if (item.StartDate.Value.Date == DateTime.Now.Date) // today
                            {
                                List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                                if (dict.ContainsKey(KEY_TODAY))
                                    dict[KEY_TODAY].Add(item);
                                else
                                    dict.Add(KEY_TODAY, lst_temp);
                            }
                            else if (item.StartDate.Value.Date == DateTime.Now.AddDays(-1).Date) // hom qua
                            {

                                List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                                if (dict.ContainsKey(KEY_YESTERDAY))
                                    dict[KEY_YESTERDAY].Add(item);
                                else
                                    dict.Add(KEY_YESTERDAY, lst_temp);
                            }
                            else if (item.StartDate.Value.Date < DateTime.Now.AddDays(-1).Date) // cuhon
                            {
                                List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                                if (dict.ContainsKey(KEY_ORTHER))
                                    dict[KEY_ORTHER].Add(item);
                                else
                                    dict.Add(KEY_ORTHER, lst_temp);
                            }
                        }
                    }
                }
                // Da Xu Ly
                // server hay local thi cung sort theo Modified
                else
                {
                    if (item.Modified.HasValue)
                    {
                        if (item.Modified.Value.Date == DateTime.Now.Date) // today
                        {
                            List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                            if (dict.ContainsKey(KEY_TODAY))
                                dict[KEY_TODAY].Add(item);
                            else
                                dict.Add(KEY_TODAY, lst_temp);
                        }
                        else if (item.Modified.Value.Date == DateTime.Now.AddDays(-1).Date) // hom qua
                        {

                            List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                            if (dict.ContainsKey(KEY_YESTERDAY))
                                dict[KEY_YESTERDAY].Add(item);
                            else
                                dict.Add(KEY_YESTERDAY, lst_temp);
                        }
                        else if (item.Modified.Value.Date < DateTime.Now.AddDays(-1).Date) // cuhon
                        {
                            List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                            if (dict.ContainsKey(KEY_ORTHER))
                                dict[KEY_ORTHER].Add(item);
                            else
                                dict.Add(KEY_ORTHER, lst_temp);
                        }
                    }
                }
            }

            table_todo.Hidden = false;
            lbl_nodata_left.Hidden = true;

            table_todo.Source = new ToDo_TableSource(dict, this);
            table_todo.ReloadData();
        }

        #endregion

        #region custom view
        #region workflow data source table
        private class ToDo_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellWorkFlow");
            ToDoDetailView parentView;
            Dictionary<string, List<BeanAppBaseExt>> dict_todo { get; set; }
            Dictionary<string, bool> dict_section { get; set; }

            public ToDo_TableSource(Dictionary<string, List<BeanAppBaseExt>> _dict_todo, ToDoDetailView _parentview)
            {
                dict_todo = _dict_todo;
                parentView = _parentview;
                GetDictSection();
            }

            private void GetDictSection()
            {
                if (parentView.dict_sectionTodo.Count > 0)
                    parentView.dict_sectionTodo.Clear();

                dict_section = parentView.dict_sectionTodo;
                foreach (var item in dict_todo)
                {
                    dict_section.Add(item.Key, true);
                }
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return dict_todo.Count;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                var sectionItem = dict_section.ElementAt(Convert.ToInt32(section));
                var numRow = dict_todo[sectionItem.Key].Count;

                if (sectionItem.Value)
                    return numRow;
                else
                    return 0;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 105;
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                if (section == 0)
                    return 1;
                else
                {
                    var key = dict_section.ElementAt(Convert.ToInt32(section)).Key;
                    if (dict_todo[key].Count > 0)
                        return 44;
                    else
                        return 1;
                }
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                var key = dict_section.ElementAt(indexPath.Section).Key;
                var itemSelected = dict_todo[key][indexPath.Row];

                if (parentView.componentButton != null)
                    parentView.componentButton.RemoveFromSuperview();

                parentView.UpdateItemSelect(itemSelected, indexPath);

                tableView.DeselectRow(indexPath, true);
            }

            public override UIView GetViewForHeader(UITableView tableView, nint section)
            {
                if (section == 0)
                {
                    return null;
                }
                else
                {
                    var key = dict_section.ElementAt(Convert.ToInt32(section)).Key;
                    if (dict_todo[key].Count > 0)
                    {
                        CGRect frame = new CGRect(0, 0, tableView.Frame.Width, 45);

                        Custom_ToDoHeader headerView = new Custom_ToDoHeader(parentView, frame);
                        headerView.LoadData(section, key);
                        return headerView;
                    }
                    else
                        return null;
                }
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var key = dict_section.ElementAt(indexPath.Section).Key;
                var todo = dict_todo[key][indexPath.Row];

                bool isOdd = true;
                if (indexPath.Row % 2 == 0)
                    isOdd = false;

                Custom_TodoCell cell = new Custom_TodoCell(cellIdentifier);
                cell.UpdateCell(todo, isOdd);
                return cell;
            }

            public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
            {
                if (parentView.isLoadMore)
                {
                    //var lst_appBase_cxl = parentView.lst_appBase_cxl;
                    var key = dict_section.ElementAt(indexPath.Section).Key;
                    var todo = dict_todo[key];

                    if (dict_section.Count - 1 == indexPath.Section)
                    {
                        if (indexPath.Row + 1 == todo.Count)
                        {
                            parentView.view_task_left_loadmore.Hidden = false;
                            parentView.loadmore_indicator.StartAnimating();
                            //parentView.isLoadData = true;
                            parentView.LoadmoreData();
                        }
                    }
                }
            }
        }

        #endregion

        #region dynamic controls source table
        private class Control_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cell");
            ToDoDetailView parentView;
            List<ViewSection> lst_section;
            Dictionary<string, List<ViewRow>> dict_control = new Dictionary<string, List<ViewRow>>();
            List<BeanWorkFlowRelated> lstWorkFlowRelated;
            List<BeanTask> lst_tasks;
            List<BeanComment> lst_comment;
            //bool canEditWF = false;
            //tam an session
            int heightHeader = -1;

            public Control_TableSource(List<ViewSection> _lst_section, List<BeanWorkFlowRelated> _lstWorkFlowRelated, List<BeanTask> _lst_tasks, List<BeanComment> _lst_comment, ToDoDetailView _parentview, bool _isSaved)
            {
                //_isSaved;
                if (_isSaved)
                {
                    _lst_section.ForEach(o =>
                    {
                        o.Enable = false;
                        o.ViewRows.ForEach(q =>
                        {
                            q.Enable = false;
                            q.Elements.ForEach(e =>
                            {
                                e.Enable = false;
                            });
                        });
                    });
                }
                if (_lstWorkFlowRelated != null && _lstWorkFlowRelated.Count > 0)
                {
                    lstWorkFlowRelated = _lstWorkFlowRelated;
                    var dataSource = JsonConvert.SerializeObject(_lstWorkFlowRelated);

                    ViewElement element = new ViewElement();
                    element.Title = CmmFunction.GetTitle("TEXT_WORKFLOW_RELATE", "Quy trình / Công việc liên kết");
                    element.DataSource = dataSource;
                    element.Value = dataSource;
                    element.DataType = "inputworkrelated";

                    List<ViewRow> lst_viewRow = new List<ViewRow>();
                    ViewRow rowWorkFlowRelate = new ViewRow();

                    List<ViewElement> lst_element = new List<ViewElement>();
                    lst_element.Add(element);
                    rowWorkFlowRelate.Elements = lst_element;

                    lst_viewRow.Add(rowWorkFlowRelate);
                    _lst_section[0].ViewRows.Add(rowWorkFlowRelate);// = lst_viewRow;
                }

                if (_lst_tasks != null && _lst_tasks.Count > 0)
                {
                    lst_tasks = _lst_tasks;
                    var dataSource = JsonConvert.SerializeObject(lst_tasks);

                    ViewElement element = new ViewElement();
                    element.Title = CmmFunction.GetTitle("TEXT_TASKLIST", "Danh sách công việc ");
                    element.DataSource = dataSource;
                    element.Value = dataSource;
                    element.DataType = "inputtasks";

                    List<ViewRow> lst_viewRow = new List<ViewRow>();
                    ViewRow rowTask = new ViewRow();

                    List<ViewElement> lst_element = new List<ViewElement>();
                    lst_element.Add(element);
                    rowTask.Elements = lst_element;

                    lst_viewRow.Add(rowTask);
                    _lst_section[0].ViewRows.Add(rowTask);// = lst_viewRow;
                }

                if (_lst_comment != null && _lst_comment.Count > 0)
                {
                    lst_comment = _lst_comment;
                    var dataSource = JsonConvert.SerializeObject(lst_comment);

                    ViewElement element = new ViewElement();
                    element.Title = CmmFunction.GetTitle("TEXT_COMMENT", "Bình luận");
                    element.DataSource = dataSource;
                    element.Value = dataSource;
                    element.DataType = "inputcomments";
                    element.Enable = !_isSaved;

                    List<ViewRow> lst_viewRow = new List<ViewRow>();
                    ViewRow rowComment = new ViewRow();

                    List<ViewElement> lst_element = new List<ViewElement>();
                    lst_element.Add(element);
                    rowComment.Elements = lst_element;

                    lst_viewRow.Add(rowComment);
                    _lst_section[0].ViewRows.Add(rowComment);
                }
                else
                {
                    ViewElement element = new ViewElement();
                    element.Title = CmmFunction.GetTitle("TEXT_COMMENT", "Bình luận");
                    element.DataSource = null;
                    element.Value = null;
                    element.DataType = "inputcomments";
                    element.Enable = !_isSaved;

                    List<ViewRow> lst_viewRow = new List<ViewRow>();
                    ViewRow rowComment = new ViewRow();

                    List<ViewElement> lst_element = new List<ViewElement>();
                    lst_element.Add(element);
                    rowComment.Elements = lst_element;

                    lst_viewRow.Add(rowComment);
                    _lst_section[0].ViewRows.Add(rowComment);
                }

                lst_section = _lst_section;
                parentView = _parentview;
                GetListRowInSection();
            }

            public void GetListRowInSection()
            {
                foreach (var item in lst_section)
                {
                    dict_control.Add(item.ID, item.ViewRows);
                }
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                return heightHeader;
            }

            public override nfloat GetHeightForFooter(UITableView tableView, nint section)
            {
                return -1;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                try
                {
                    var element = dict_control[lst_section[indexPath.Section].ID][indexPath.Row].Elements[0];
                    var control = dict_control[lst_section[indexPath.Section].ID][indexPath.Row];
                    switch (element.DataType)
                    {
                        case "tabs":
                            return 80;
                        case "inputattachmenthorizon":
                            if (!string.IsNullOrEmpty(element.Value) && element.Value != "[]")
                            {
                                int sectionHeightTotal = 0;
                                List<BeanAttachFile> lst_attach = new List<BeanAttachFile>();
                                JArray json = JArray.Parse(element.Value);
                                lst_attach = json.ToObject<List<BeanAttachFile>>();
                                parentView.lst_attachFile = lst_attach;

                                if (lst_attach.Count > 0)
                                {
                                    List<string> sectionKeys = lst_attach.Select(x => x.AttachTypeName).Distinct().ToList();
                                    if (sectionKeys != null && sectionKeys.Count > 0)
                                        sectionHeightTotal = sectionKeys.Count * 44;

                                    return (lst_attach.Count * 65) + 75 + sectionHeightTotal;//header height: 75 - cell row height: 60 - padding top của table : 10
                                }
                                else
                                    return 81;
                            }
                            else
                                return 81;
                        case "attachmentverticalformframe":
                            {
                                var arrAttachment = element.Value.Split(new string[] { ";#" }, StringSplitOptions.None);
                                int numItem = arrAttachment.Length / 2;

                                return (numItem >= 3) ? 265 : (85 + (numItem * 60)); //header view height: 85 | cell height: 60 | max cell: 3 cell
                            }
                        case "textinputmultiline":
                            {
                                string value = CmmFunction.StripHTML(element.Value);
                                var height_ets = StringExtensions.StringRect(value, UIFont.SystemFontOfSize(14, UIFontWeight.Regular), tableView.Frame.Width - 20);
                                if (height_ets.Height < 200)
                                {
                                    if (height_ets.Height > 25 && height_ets.Height <= 25)
                                        return (height_ets.Height) + 25;
                                    else if (height_ets.Height > 70)
                                        return (height_ets.Height) + 40;
                                    else
                                        return 75;
                                }
                                else
                                    return 140;
                            }
                        case "textinputformat":
                            {
                                var height_ets = StringExtensions.StringRect(element.Value, UIFont.SystemFontOfSize(14, UIFontWeight.Regular), tableView.Frame.Width - 20);
                                if (height_ets.Height < 200)
                                    return 100 + 25;
                                else
                                    return height_ets.Height + 25;
                            }
                        case "inputgriddetails":
                            {
                                nfloat height = 90;
                                var data_source = element.DataSource.Trim();
                                var data_value = element.Value.Trim();

                                if (string.IsNullOrEmpty(data_value) || !data_value.Equals("[]"))
                                {
                                    List<JObject> lst_jobject = new List<JObject>();
                                    if (!string.IsNullOrEmpty(data_value))
                                    {
                                        JArray rowItem = JArray.Parse(data_value);
                                        foreach (JObject ob in rowItem)
                                        {
                                            lst_jobject.Add(ob);
                                        }
                                    }

                                    nfloat height_expand = 0;
                                    var lst_titleHeader = new List<BeanWFDetailsHeader>();
                                    if (!string.IsNullOrEmpty(data_source) && data_source != "[]")
                                    {
                                        JArray json = JArray.Parse(data_source);
                                        lst_titleHeader = json.ToObject<List<BeanWFDetailsHeader>>();

                                        foreach (var item in lst_titleHeader)
                                        {
                                            //if (item.internalName == "TongTien")
                                            //    item.isSum = true;

                                            if (item.isSum)
                                            {
                                                item.isSum = true;
                                                height_expand = 50;
                                            }
                                        }
                                        height = height + (lst_jobject.Count * 50);

                                    }
                                    else
                                        height = height + (lst_jobject.Count * 50);

                                    return height + height_expand;
                                }
                                else
                                    return 90;
                            }
                        case "inputworkrelated":
                            {
                                var tableHeight = lstWorkFlowRelated.Count * 80;
                                return tableHeight + 50;
                            }
                        case "inputtasks":
                            {
                                //rowNum = 0;
                                //var lst_parent = lst_tasks.Where(i => i.Parent == 0).ToList();
                                //foreach (var parent in lst_parent)
                                //{
                                //    if (parent.IsExpand)
                                //    {
                                //        //rowNum = 1;
                                //        LoadCountSubTask(parent);
                                //    }
                                //    else
                                //        rowNum = 1;
                                //}

                                //return (rowNum * 90) + 90;

                                var tableHeight = lst_tasks.Count * 90;
                                return tableHeight + 90;
                            }
                        case "inputcomments":
                            {
                                nfloat basicHeight = 160;
                                nfloat height = 0;
                                //notes => add comment, dinh kem comment 
                                if (element.Notes != null && element.Notes.Count > 0)
                                {
                                    foreach (var note in element.Notes)
                                    {
                                        if (note.Key == "image")
                                            height = height + 120;
                                        else if (note.Key == "doc")
                                        {
                                            JArray json = JArray.Parse(note.Value);
                                            var lst_addAttachment = json.ToObject<List<BeanAttachFile>>();
                                            if (lst_addAttachment != null && lst_addAttachment.Count > 0)
                                            {
                                                height = height + (lst_addAttachment.Count() * 40);
                                            }
                                        }
                                    }
                                    height = height + basicHeight;
                                }
                                else
                                    height = basicHeight;

                                if (!string.IsNullOrEmpty(element.DataSource) || element.DataSource != "[]")
                                {

                                }

                                //danh sach tat ca comment trong phieu
                                if (lst_comment != null && lst_comment.Count > 0)
                                {
                                    foreach (var comment in lst_comment)
                                    {
                                        // comment co dinh kem
                                        if (!string.IsNullOrEmpty(comment.AttachFiles))
                                        {
                                            JArray json = JArray.Parse(comment.AttachFiles);
                                            List<BeanAttachFile> newSortList = new List<BeanAttachFile>();
                                            var lst_attachFiles = json.ToObject<List<BeanAttachFile>>();

                                            foreach (var attach in lst_attachFiles)
                                            {
                                                string fileExt = string.Empty;
                                                if (!string.IsNullOrEmpty(attach.Url))
                                                    fileExt = attach.Url.Split('.').Last();

                                                bool isThumb = CmmFunction.CheckIsImageAttachmentType(fileExt);
                                                if (isThumb)
                                                {
                                                    //height = height + 190 + 200;
                                                    height = height + 230;
                                                }
                                                else
                                                {
                                                    newSortList.Insert(newSortList.Count, attach);
                                                    height = height + 40;
                                                }
                                            }

                                            nfloat heightText = 0;
                                            if (!string.IsNullOrEmpty(comment.Content))
                                            {
                                                CGRect rect = StringExtensions.StringRect(comment.Content, UIFont.FromName("ArialMT", 14f), (parentView.table_content_right.Frame.Width / 5) * 4.4f);
                                                if (rect.Height > 0 && rect.Height < 20)
                                                    rect.Height = 30;
                                                heightText = rect.Height + 50;
                                            }
                                            else
                                                heightText = 80;

                                            height = height + heightText;

                                        }
                                        // comment khong co dinh kem
                                        else
                                        {
                                            height = height + 100;
                                        }
                                    }
                                }

                                parentView.estCommmentViewRowHeight = height;
                                return height;
                            }
                        default:
                            {
                                return 60;
                            }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ToDoDetailView - Control_TableSource - GetHeightForRow - Err: " + ex.ToString());
                    return 0;
                }
            }

            int rowNum;
            private void LoadCountSubTask(BeanTask parent_task)
            {
                if (parent_task.ChildTask != null)
                {
                    foreach (var i2 in parent_task.ChildTask)
                    {
                        rowNum++;
                        var lv2 = lst_tasks.Where(i => i.Parent == i2.ID).ToList();
                        if (lv2 != null && lv2.Count() > 0)
                        {
                            LoadCountSubTask(i2);
                        }
                    }
                }
                else
                    rowNum++;
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return lst_section.Count;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                var sectionItem = lst_section[Convert.ToInt32(section)];
                var lst_row = dict_control[sectionItem.ID];
                if (sectionItem.ShowType)
                    return lst_row.Count;
                else
                    return 0;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                tableView.DeselectRow(indexPath, true);
            }

            public override bool ShouldHighlightRow(UITableView tableView, NSIndexPath rowIndexPath)
            {
                return false;
            }

            //public override UIView GetViewForHeader(UITableView tableView, nint section)
            //{
            //    var sectionItem = lst_section[Convert.ToInt32(section)];

            //    ComponentSection componentSection = new ComponentSection(parentView, sectionItem, section);
            //    componentSection.InitializeFrameView(new CGRect(0, 0, parentView.View.Bounds.Width, heightHeader));
            //    componentSection.UpdateContentSection();

            //    return componentSection;
            //}

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var control = dict_control[lst_section[indexPath.Section].ID][indexPath.Row];
                Control_cell_custom cell = new Control_cell_custom(parentView, cellIdentifier, control, indexPath);
                return cell;
            }
        }

        private class Control_cell_custom : UITableViewCell
        {
            ToDoDetailView parentView { get; set; }
            ViewRow control { get; set; }
            NSIndexPath currentIndexPath { get; set; }
            public ComponentBase components;

            public Control_cell_custom(ToDoDetailView _parentView, NSString cellID, ViewRow _control, NSIndexPath _currentIndexPath) : base(UITableViewCellStyle.Default, cellID)
            {
                parentView = _parentView;
                control = _control;
                currentIndexPath = _currentIndexPath;
                Accessory = UITableViewCellAccessory.None;
                viewConfiguration();
            }

            private void viewConfiguration()
            {
                ContentView.BackgroundColor = UIColor.White;

                switch (control.RowType)
                {
                    case 1:
                        components = new ComponentRow1(parentView, control.Elements[0], currentIndexPath);
                        break;
                    case 2:
                        components = new ComponentRow2(parentView, control, currentIndexPath);
                        break;
                    case 3:
                        components = new ComponentRow3(parentView, control, currentIndexPath);
                        break;
                    default:
                        components = new ComponentRow1(parentView, control.Elements[0], currentIndexPath);
                        break;
                }

                ContentView.Add(components);
                loadData();
            }

            public void loadData()
            {
                try
                {
                    components.SetTitle();
                    components.SetValue();
                    components.SetEnable();
                    components.SetProprety();
                    components.SetRequire();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("RequestDetailsView - Control_cell_custom - loaddata- ERR: " + ex.ToString());
                }
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();

                components.InitializeFrameView(new CGRect(18, 0, ContentView.Frame.Width - 36, ContentView.Frame.Height));
            }
        }

        #endregion
        #endregion
    }
}
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UIKit;

namespace BPMOPMobile.iPad
{
    public partial class WorkflowDetailView : UIViewController
    {
        private NSObject _willResignActiveNotificationObserver;
        private NSObject _didBecomeActiveNotificationObserver;
        AppDelegate appD;
        //ButtonsActionTopBar buttonActionTopBar;
        ButtonsActionBotBar buttonActionBotBar;
        UIRefreshControl workflow_refreshControl;
        //int fromMe_count;
        CmmLoading loading, lstViewLoading;

        public DateTime fromDateSelected;
        public DateTime toDateSelected;
        bool isExpandUser;
        bool isTask = false;
        //value form default

        public DateTime fromDate_default;
        public DateTime toDate_default;
        public bool isFilter;
        bool isLoadMore = true;
        public List<BeanAppStatus> lst_appStatus;
        private List<BeanAppStatus> lst_appStatus_selected;
        public List<ClassMenu> lst_dueDateMenu;
        private ClassMenu DuedateSelected;
        public string date_filter = string.Empty;

        nfloat origin_view_header_content_height_constant;
        public nfloat estCommmentViewRowHeight;

        public bool isShowKeyBoarFromComment;
        bool isFollow;
        public bool isShowTask;

        int limit = 20;
        int offset = 0;
        //CGRect view_buttonAction_default;
        List<BeanTask> lst_tasks;
        List<BeanWorkFlowRelated> lstWorkFlowRelateds;
        List<BeanAppBaseExt> lst_appBase_MyRequest;
        List<BeanAppBaseExt> lst_workflow_fromMe_result = new List<BeanAppBaseExt>();
        //List<CountNum> countnum_workflow_fromMe = new List<CountNum>();

        List<string> lst_userName = new List<string>();
        Dictionary<string, List<BeanAppBaseExt>> dict_workflow = new Dictionary<string, List<BeanAppBaseExt>>();
        Dictionary<string, string> dic_valueObject = new Dictionary<string, string>();
        List<BeanQuaTrinhLuanChuyen> lst_qtlc;
        string str_json_FormDefineInfo = string.Empty;
        string[] arr_assignedTo;
        string json_attachRemove;
        public string searchKeyword;
        Dictionary<string, bool> dict_sectionWorkFlow = new Dictionary<string, bool>();
        public BeanAppBaseExt currentSelectedItem { get; set; }
        public BeanWorkflowItem workflowItem;
        private List<KeyValuePair<string, string>> lstExtent = new List<KeyValuePair<string, string>>();
        List<ViewSection> lst_section { get; set; }
        string Json_FormDataString = string.Empty;
        List<BeanQuaTrinhLuanChuyen> lst_QTLC = new List<BeanQuaTrinhLuanChuyen>();
        string localDocumentFilepath = string.Empty;
        ComponentButtonBot componentButton;
        List<BeanAttachFile> lst_attachFile;
        List<BeanAttachFile> lst_addAttachment;
        public List<BeanAttachFile> lst_addCommentAttachment;
        Dictionary<string, List<BeanAppBaseExt>> dict_workflow_result = new Dictionary<string, List<BeanAppBaseExt>>();
        List<ButtonAction> lst_menuItem = new List<ButtonAction>();
        bool IsFirstLoad = true;
        UIImagePickerController imagePicker;
        UIDocumentPickerViewController docPicker;
        JObject JObjectSource = new JObject(); // JObject những Element ko phải calculated
        // Comment
        private List<BeanComment> lst_comments;
        public List<BeanAttachFile> _lstAttachComment = new List<BeanAttachFile>();
        private DateTime _CommentChanged;
        public string _OtherResourceId = "";
        //Attachments
        int numRowAttachmentFile = 0;
        ViewElement attachmentElement;
        UIButton btn_ReadFull;
        //List<BeanNotify> lst_notify_result = new List<BeanNotify>();
        bool tab_inprogress = true;
        public bool isSearch;
        nfloat origin_constraint_rightBT_Search;
        bool isOnline = true;
        UIStringAttributes firstAttributes = new UIStringAttributes
        {
            Font = UIFont.FromName("ArialMT", 13f)
        };

        public WorkflowDetailView(IntPtr handle) : base(handle)
        {
            appD = UIApplication.SharedApplication.Delegate as AppDelegate;
            localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }

        #region override
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            _willResignActiveNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, KeyBoardUpNotification);
            _didBecomeActiveNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyBoardDownNotification);
            /*
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
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UITapGestureRecognizer gesture = new UITapGestureRecognizer(() =>
            {
                View.EndEditing(true);
                HandleCloseAddFollow();
            });

            gesture.ShouldReceiveTouch += delegate (UIGestureRecognizer recognizer, UITouch touch)
            {
                var touchView = touch.View.Class.Name;
                if (touchView == "UIButton" || touchView == "UITableViewCellContentView")
                    return false;
                else
                    CloseMenuOption();

                return true;
            };

            gesture.CancelsTouchesInView = false;
            View.AddGestureRecognizer(gesture);

            btn_ReadFull = new UIButton
            {
                BackgroundColor = UIColor.Clear,
                Hidden = true,
            };
            this.View.AddSubview(btn_ReadFull);
            this.View.BringSubviewToFront(btn_ReadFull);

            workflow_refreshControl = new UIRefreshControl();
            view_BotBar_ConstantHeight.Constant = view_BotBar_ConstantHeight.Constant + 10;

            if (workflowItem != null)
                loadQuaTrinhluanchuyen();

            SetLangTitle();
            _ = Task.Run(() =>
            {
                SetDefaultFilter();
                InvokeOnMainThread(() =>
                {
                    if (isFilter)
                        LoadDataFilterFromServer(false);
                    else
                    {
                        //LoadTinhTrangCategory();
                        //LoadDueDateCategory();
                        ////LoadDataFilterFromMe(fromDate_default, toDate_default);
                        LoadDataFilterFromMe();
                    }

                    //LoadDataFilterFromMe();

                    //if (currentSelectedItem != null && currentSelectedItem.ResourceCategoryId == (int)CmmFunction.CommentResourceCategoryID.Task)
                    //    isShowTask = true;
                    //BT_DangXL_TouchUpInside(null, null);
                    //LoadItemSelected(true);
                });
            });

            #region delegate
            CmmEvent.UpdateLangComplete += CmmEvent_UpdateLangComplete;
            workflow_refreshControl.ValueChanged += Workflow_refreshControl_ValueChanged;
            BT_comment.TouchUpInside += BT_comment_TouchUpInside;
            BT_moreUser.TouchUpInside += BT_moreUser_TouchUpInside;
            BT_avatar.TouchUpInside += BT_avatar_TouchUpInside;
            BT_start.TouchUpInside += BT_start_TouchUpInside;
            BT_share.TouchUpInside += BT_share_TouchUpInside;
            BT_history.TouchUpInside += BT_history_TouchUpInside;
            BT_filter_search.TouchUpInside += BT_filter_search_TouchUpInside;
            BT_attachement.TouchUpInside += BT_attachement_TouchUpInside;
            BT_search.TouchUpInside += BT_search_TouchUpInside;
            tf_search.EditingChanged += Tf_search_EditingChanged;
            BT_DangXL.TouchUpInside += BT_DangXL_TouchUpInside;
            BT_DaXL.TouchUpInside += BT_DaXL_TouchUpInside;
            #endregion
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (isSearch)
            {
                isSearch = !isSearch;
                BT_search_TouchUpInside(null, null);
            }
        }

        #endregion

        #region public - private method
        public void SetContent(BeanAppBaseExt _currentSelected, UIViewController parent)
        {
            currentSelectedItem = _currentSelected;
        }

        public void SetContentFromSearch(int _statusIndex, int _duedateIndex, DateTime _fromdate, DateTime _todate, string _keyword)
        {
            //status_selected_index = _statusIndex;
            //duedate_selected_index = _duedateIndex;
            fromDateSelected = _fromdate;
            toDateSelected = _todate;
            searchKeyword = _keyword;
        }

        private void ViewConfiguration()
        {
            BT_DangXL.Layer.ShadowOffset = new CGSize(1, 2);
            BT_DangXL.Layer.ShadowRadius = 4;
            BT_DangXL.Layer.ShadowColor = UIColor.Black.CGColor;
            BT_DangXL.Layer.ShadowOpacity = 0.5f;

            BT_DaXL.Layer.ShadowOffset = new CGSize(-1, 2);
            BT_DaXL.Layer.ShadowRadius = 4;
            BT_DaXL.Layer.ShadowColor = UIColor.Black.CGColor;
            BT_DaXL.Layer.ShadowOpacity = 0.0f;

            tf_search.Placeholder = CmmFunction.GetTitle("TEXT_SEARCH", "Tìm kiếm...");

            //buttonActionTopBar = ButtonsActionTopBar.Instance;
            //buttonActionTopBar.InitFrameView(view_top_bar.Frame);
            //view_top_bar.AddSubview(buttonActionTopBar);

            origin_view_header_content_height_constant = view_header_content_height_constant.Constant;
            origin_constraint_rightBT_Search = constraint_rightBT_Search.Constant;

            buttonActionBotBar = ButtonsActionBotBar.Instance;
            buttonActionBotBar.InitFrameView(view_bot_bar.Frame);
            buttonActionBotBar.LoadStatusButton(2);
            view_bot_bar.AddSubview(buttonActionBotBar);

            CmmIOSFunction.MakeCornerTopLeftRight(view_task_left, 8);
            CmmIOSFunction.MakeCornerTopLeftRight(view_task_right, 8);

            workflow_refreshControl.TintColor = UIColor.FromRGB(65, 80, 134);
            workflow_refreshControl.AttributedTitle = new NSAttributedString(CmmFunction.GetTitle("TEXT_REFRESH_LOADING", "Loading ..."), firstAttributes);
            table_workflow.AddSubview(workflow_refreshControl);

            //iv_search.Image = iv_search.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            //iv_search.TintColor = UIColor.Black;

            CmmIOSFunction.CreateCircleButton(BT_avatar);
            string documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string localpath = Path.Combine(documentFolder, CmmVariable.M_Avatar);
            BT_avatar.ImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
            if (File.Exists(localpath))
                BT_avatar.SetImage(UIImage.FromFile(localpath), UIControlState.Normal);
            else
                BT_avatar.SetImage(UIImage.FromFile("Icons/icon_profile.png"), UIControlState.Normal);

            BT_search.ContentEdgeInsets = new UIEdgeInsets(6, 6, 6, 6);
            BT_filter_search.ContentEdgeInsets = new UIEdgeInsets(7, 14, 7, 0);
            BT_start.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_comment.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_share.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_attachement.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_history.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);

            //tam khoa shadow
            //CmmIOSFunction.AddShadowForTopORBotBar(view_top, true);
            //CmmIOSFunction.AddShadowForTopORBotBar(view_bot_bar, false);
            SetTopBarTitle();
        }

        void SetTopBarTitle()
        {
            lbl_topBar_title.Text = CmmFunction.GetTitle("TEXT_IPAD_FROMME", "Trang chủ > Tôi bắt đầu");
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

        /// <summary>
        /// Không sử dụng
        /// </summary>
        /// <param name="_fromdate"></param>
        /// <param name="_todate"></param>
        public void LoadDataFilterFromMe(DateTime _fromdate, DateTime _todate)
        {
            try
            {
                if (isFilter)
                    BT_filter_search.TintColor = UIColor.Orange;
                else
                    BT_filter_search.TintColor = UIColor.Black;

                var conn = new SQLiteConnection(CmmVariable.M_DataPath);
                string count_query_fromMe = string.Empty;
                string query_lstAppBase = string.Empty;

                dict_workflow = new Dictionary<string, List<BeanAppBaseExt>>();
                dict_sectionWorkFlow = new Dictionary<string, bool>();
                lst_appBase_MyRequest = new List<BeanAppBaseExt>();

                //Check filter DueDate
                if (lst_dueDateMenu != null)
                    DuedateSelected = lst_dueDateMenu.Where(s => s.isSelected == true).FirstOrDefault();

                string duedate_condition = "";
                if (DuedateSelected == null || DuedateSelected.ID == 1) // Tat ca
                {
                    duedate_condition = "";
                }
                else if (DuedateSelected.ID == 2) // Trong ngay
                {
                    duedate_condition = @"AND (AB.DueDate IS NOT NULL AND date(AB.DueDate) = date('now'))";
                }
                else if (DuedateSelected.ID == 3) // Tre han
                {
                    duedate_condition = @"AND (DueDate IS NOT NULL AND DueDate < date('now'))";
                }

                //Check Filter Status
                if (lst_appStatus != null)
                    lst_appStatus_selected = lst_appStatus.Where(s => s.IsSelected == true).ToList();

                if (lst_appStatus_selected != null && lst_appStatus_selected.Count > 0)
                {
                    string str_status = string.Join(',', lst_appStatus_selected.Select(i => i.ID));

                    if (!string.IsNullOrEmpty(str_status))
                        str_status = string.Format(@" AND AB.StatusGroup IN({0})", str_status);

                    query_lstAppBase = string.Format(@"SELECT * FROM BeanAppBase AB
                                                WHERE AB.CreatedBy LIKE '%{1}%'
                                                {0} {2} {3}
                                                ORDER BY AB.Created DESC LIMIT ? OFFSET ?",
                                                (string.IsNullOrEmpty(date_filter) ? "" : "AND " + date_filter), CmmVariable.SysConfig.UserId.ToLower(), str_status, duedate_condition);
                }
                // default 
                else
                {
                    string strDefaultFromMe = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS_FROMME); // tat ca
                    if (!string.IsNullOrEmpty(strDefaultFromMe))
                        strDefaultFromMe = string.Format(@" AND AB.StatusGroup IN({0})", strDefaultFromMe);

                    string query_today = string.Format(@"SELECT Count(*) as count
                                                    FROM BeanAppBase AB
                                                    LEFT JOIN BeanAppStatus AST
                                                        ON AST.ID = AB.StatusGroup
                                                    WHERE AB.CreatedBy LIKE '%{0}%' {1}", CmmVariable.SysConfig.UserId.ToUpper(), strDefaultFromMe);

                    List<CountNum> countnum_myRequest = conn.Query<CountNum>(query_today);

                    //if (countnum_myRequest != null)
                    //    fromMe_count = countnum_myRequest[0].count;

                    query_lstAppBase = string.Format(@"SELECT AB.* FROM BeanAppBase AB
                                        LEFT JOIN BeanAppStatus AST
                                            ON AST.ID = AB.StatusGroup
                                        WHERE AB.CreatedBy LIKE '%{0}%' {1}
                                        ORDER BY AB.Created DESC LIMIT ? OFFSET ?", CmmVariable.SysConfig.UserId.ToUpper(), strDefaultFromMe);
                }

                query_lstAppBase = CreateQueryFromMe(false);
                lst_appBase_MyRequest = conn.Query<BeanAppBaseExt>(query_lstAppBase, limit, offset);

                #region Load count from me obj
                var queryCount = CreateQueryFromMe(true);
                var countObj = conn.Query<CountNum>(queryCount);

                string str_toMe = CmmFunction.GetTitle("TEXT_IPAD_FROMME", "Trang chủ > Tôi bắt đầu");
                if (countObj != null && countObj.Count > 0)
                {
                    var fromMe_count = countObj[0].count;
                }
                #endregion

                if (lst_appBase_MyRequest != null & lst_appBase_MyRequest.Count > 0)
                {
                    /*string str_toMe = CmmFunction.GetTitle("TEXT_IPAD_FROMME", "Trang chủ > Tôi bắt đầu");
                    if (fromMe_count >= 100)
                        lbl_topBar_title.Text = str_toMe + " (99+)";
                    else if (fromMe_count > 0 && fromMe_count < 100)
                    {
                        str_toMe = str_toMe + " (" + fromMe_count.ToString() + ")";
                        lbl_topBar_title.Text = str_toMe;
                    }
                    else
                        lbl_topBar_title.Text = str_toMe;

                    var str_transalte = lbl_topBar_title.Text;
                    var indexA = str_transalte.IndexOf('(');
                    if (indexA != -1)
                    {
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(94, 94, 94), new NSRange(0, str_transalte.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(255, 122, 58), new NSRange(indexA, str_transalte.Length - indexA));
                        lbl_topBar_title.AttributedText = att;
                    }*/
                    SetLangTitle();
                    if (currentSelectedItem != null)
                        currentSelectedItem.IsSelected = true;
                    else
                        currentSelectedItem = lst_appBase_MyRequest[0];
                    SortListAppBase();
                    /*
                    const string KEY_TODAY = "Hôm nay";
                    const string KEY_YESTERDAY = "Hôm qua";
                    const string KEY_ORTHER = "Cũ hơn";

                    List<BeanAppBaseExt> lst_temp1 = new List<BeanAppBaseExt>();
                    List<BeanAppBaseExt> lst_temp2 = new List<BeanAppBaseExt>();
                    List<BeanAppBaseExt> lst_temp3 = new List<BeanAppBaseExt>();
                    dict_workflow.Add(KEY_TODAY, lst_temp1);
                    dict_workflow.Add(KEY_YESTERDAY, lst_temp2);
                    dict_workflow.Add(KEY_ORTHER, lst_temp3);

                    foreach (var item in lst_appBase_MyRequest)
                    {
                        if (item.ID == currentItemSelected.ID)
                            item.IsSelected = true;
                        else
                            item.IsSelected = false;

                        if (item.Created.Value.Date == DateTime.Now.Date) // today
                        {
                            List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                            if (dict_workflow.ContainsKey(KEY_TODAY))
                                dict_workflow[KEY_TODAY].Add(item);
                            else
                                dict_workflow.Add(KEY_TODAY, lst_temp);
                        }
                        else if (item.Created.Value.Date == DateTime.Now.AddDays(-1).Date) // hom qua
                        {
                            List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                            if (dict_workflow.ContainsKey(KEY_YESTERDAY))
                                dict_workflow[KEY_YESTERDAY].Add(item);
                            else
                                dict_workflow.Add(KEY_YESTERDAY, lst_temp);
                        }
                        else if (item.Created.Value.Date < DateTime.Now.AddDays(-1).Date) // cuhon
                        {
                            List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                            if (dict_workflow.ContainsKey(KEY_ORTHER))
                                dict_workflow[KEY_ORTHER].Add(item);
                            else
                                dict_workflow.Add(KEY_ORTHER, lst_temp);
                        }
                    }

                    table_workflow.Hidden = false;
                    lbl_nodata_left.Hidden = true;

                    table_workflow.Source = new WorkFlow_TableSource(dict_workflow, this);
                    table_workflow.ReloadData();*/
                }
                else
                {
                    table_workflow.Hidden = true;
                    lbl_nodata_left.Hidden = false;

                    view_task_right.Hidden = true;
                    lbl_nodata_right.Hidden = false;
                }

                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    tf_search.Text = searchKeyword;
                    searchData(true);
                }
            }
            catch (Exception ex)
            {
                //lbl_topBar_title.Text = CmmFunction.GetTitle("TEXT_IPAD_FROMME", "Trang chủ > Tôi bắt đầu");
                table_workflow.Source = null;
                table_workflow.ReloadData();
                Console.WriteLine("MyRequestListView - FilterFromMe - Err: " + ex.ToString());
            }
        }

        public async void LoadDataFilterFromMe(bool isTabChange = false)
        {
            lstViewLoading = new CmmLoading(new CGRect(view_task_left.Center.X - 100, view_task_left.Center.Y - 100, 200, 200), "Đang xử lý...");
            view_task_left.Add(lstViewLoading);
            string count_query_fromMe = string.Empty;
            dict_workflow = new Dictionary<string, List<BeanAppBaseExt>>();
            dict_sectionWorkFlow = new Dictionary<string, bool>();
            lst_appBase_MyRequest = new List<BeanAppBaseExt>();
            try
            {
                ShowObjContent();
                if (isTabChange) SetDefaultFilter();

                if (isFilter)
                    BT_filter_search.TintColor = UIColor.Orange;
                else
                    BT_filter_search.TintColor = UIColor.Black;

                #region Load count from me obj
                try
                {
                    SetLangTitle();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Count Err: " + e.ToString());
                }
                #endregion

                #region Load List Obj
                try
                {
                    _ = Task.Run(() =>
                    {
                        isOnline = Reachability.detectNetWork();
                        if (isOnline)
                            LoadDataOnline(isTabChange);
                        else
                            LoadDataOffline();

                        InvokeOnMainThread(() =>
                        {
                            if (lst_appBase_MyRequest != null & lst_appBase_MyRequest.Count > 0)
                            {
                                //if (currentSelectedItem != null)
                                //    currentSelectedItem.IsSelected = true;
                                //else
                                currentSelectedItem = lst_appBase_MyRequest[0];

                                #region filter with search Key
                                if (!string.IsNullOrEmpty(searchKeyword))
                                {
                                    tf_search.Text = searchKeyword;
                                    searchData(true);
                                }
                                else
                                {
                                    SortListAppBase();
                                }
                                #endregion

                                if (currentSelectedItem != null && currentSelectedItem.ResourceCategoryId == (int)CmmFunction.CommentResourceCategoryID.Task)
                                    isShowTask = true;

                                //BT_DangXL_TouchUpInside(null, null);
                                //SetLangTitle();
                                //LoadDataFilterFromMe();

                                LoadItemSelected(true);
                            }
                            else
                            {
                                table_workflow.Hidden = true;
                                lbl_nodata_left.Hidden = false;

                                view_task_right.Hidden = true;
                                lbl_nodata_right.Hidden = false;
                            }
                            lstViewLoading?.Hide();
                        });
                    });
                }
                catch (Exception e)
                {
                    lstViewLoading?.Hide();
                    Console.WriteLine("Load List Err: " + e.ToString());
                }

                #endregion
            }
            catch (Exception ex)
            {
                //lbl_topBar_title.Text = CmmFunction.GetTitle("TEXT_IPAD_FROMME", "Trang chủ > Tôi bắt đầu");
                table_workflow.Source = null;
                table_workflow.ReloadData();
                Console.WriteLine("MyRequestListView - FilterFromMe - Err: " + ex.ToString());
            }
        }

        void LoadDataOnline(bool isTabChange = false)
        {
            List<BeanAppBaseExt> lstObj = new List<BeanAppBaseExt>();
            try
            {
                lstObj = new ProviderBase().LoadMoreDataTFromSerVer(tab_inprogress ? CmmVariable.KEY_GET_FROMME_INPROCESS : CmmVariable.KEY_GET_FROMME_PROCESSED, 20, isLoadMore && !isTabChange ? lst_appBase_MyRequest.Count : 0);

                if (lstObj != null && lstObj.Count > 0)
                {
                    CmmIOSFunction.UpdateNewListDataOnline(lstObj, new SQLiteConnection(CmmVariable.M_DataPath));
                    if (isLoadMore && !isTabChange)
                    {
                        if (lst_appBase_MyRequest == null) lst_appBase_MyRequest = new List<BeanAppBaseExt>();
                        lst_appBase_MyRequest.AddRange(lstObj);
                    }
                    else
                        lst_appBase_MyRequest = lstObj;

                    //Check DueDate
                    if (DuedateSelected != null)
                        switch (DuedateSelected.ID)
                        {
                            case 2: // Trong ngày
                                lst_appBase_MyRequest = lst_appBase_MyRequest.FindAll(o => o.DueDate.HasValue && o.DueDate.Value.Date == DateTime.Now.Date);
                                break;
                            case 3: // Trễ hạn
                                lst_appBase_MyRequest = lst_appBase_MyRequest.FindAll(o => o.DueDate.HasValue && o.DueDate.Value.Date < DateTime.Now.Date);
                                break;
                            default:// Tất cả
                                break;
                        }

                    //Check Filter Status
                    if (lst_appStatus != null)
                    {
                        lst_appStatus_selected = lst_appStatus.Where(s => s.IsSelected == true).ToList();
                        if (lst_appStatus_selected != null && lst_appStatus_selected.Count > 0)
                            lst_appBase_MyRequest = lst_appBase_MyRequest.FindAll(o => o.StatusGroup.HasValue && lst_appStatus_selected.FindIndex(s => s.ID == o.StatusGroup.Value) > -1);
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

        void LoadDataOffline()
        {
            var conn = new SQLiteConnection(CmmVariable.M_DataPath);
            string query_lstAppBase = CreateQueryFromMe(false);
            lst_appBase_MyRequest = conn.Query<BeanAppBaseExt>(query_lstAppBase, limit, offset);
        }

        private string CreateQueryFromMe(bool isGetCount)
        {
            string query = string.Empty;
            //Check filter DueDate
            //string duedate_condition = "";
            //if (DuedateSelected_fromMe == null || DuedateSelected_fromMe.ID == 1) // Tat ca
            //    duedate_condition = "";
            //else if (DuedateSelected_fromMe.ID == 2) // Trong ngay
            //    duedate_condition = @"AND (AB.DueDate IS NOT NULL AND date(AB.DueDate) = date('now'))";
            //else if (DuedateSelected_fromMe.ID == 3) // Tre han
            //    duedate_condition = @"AND (AB.DueDate IS NOT NULL AND AB.DueDate < date('now'))";

            //Check Filter Status
            string str_status = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS); // tat ca fromme
            //if (lst_appStatus_selected != null && lst_appStatus_selected.Count > 0) // co chon status
            //    str_status = string.Join(',', lst_appStatus_selected.Select(i => i.ID));

            var toggleFocused = CmmFunction.GetAppSettingValue(tab_inprogress ? CmmVariable.APPSTATUS_FROMME_DANGXULY : CmmVariable.APPSTATUS_FROMME_DAXULY);
            string[] arrayStatus = !string.IsNullOrEmpty(toggleFocused) ? toggleFocused.Split(",") : str_status.Split(",");
            if (lst_appStatus_selected != null && lst_appStatus_selected.Count > 0) // co chon status
            {
                var selectedStatus = lst_appStatus_selected.FindAll(o => arrayStatus.Contains(o.ID.ToString())).ToList();
                str_status = string.Join(',', selectedStatus.Select(i => i.ID));
            }
            else
            {
                str_status = toggleFocused;
            }
            if (!string.IsNullOrEmpty(str_status))
                str_status = string.Format(@" AND AB.StatusGroup IN({0})", str_status);

            if (isGetCount)
                query = string.Format(@"SELECT Count(*) as count FROM BeanAppBase AB
                                        LEFT JOIN BeanAppStatus AST
                                            ON AST.ID = AB.StatusGroup
                                        WHERE AB.CreatedBy LIKE '%{0}%' {1} ",
                                        CmmVariable.SysConfig.UserId.ToUpper(), str_status);
            else
                query = string.Format(@"SELECT AB.* FROM BeanAppBase AB
                                        LEFT JOIN BeanAppStatus AST
                                            ON AST.ID = AB.StatusGroup
                                        WHERE AB.CreatedBy LIKE '%{0}%' {1}
                                        ORDER BY AB.Created DESC LIMIT ? OFFSET ?", CmmVariable.SysConfig.UserId.ToUpper(), str_status);
            return query;
        }

        /// <summary>
        /// Sort section from result list
        /// </summary>
        private void SortListAppBase()
        {
            string KEY_TODAY = "TEXT_TODAY`Hôm nay";
            string KEY_YESTERDAY = "TEXT_YESTERDAY`Hôm qua";
            string KEY_ORTHER = "TEXT_OLDER`Cũ hơn";

            List<BeanAppBaseExt> lst_temp1 = new List<BeanAppBaseExt>();
            List<BeanAppBaseExt> lst_temp2 = new List<BeanAppBaseExt>();
            List<BeanAppBaseExt> lst_temp3 = new List<BeanAppBaseExt>();
            dict_workflow.Add(KEY_TODAY, lst_temp1);
            dict_workflow.Add(KEY_YESTERDAY, lst_temp2);
            dict_workflow.Add(KEY_ORTHER, lst_temp3);
            /*
            if (currentSelectedItem != null)
                currentSelectedItem.IsSelected = true;
            else
                currentSelectedItem = lst_appBase_MyRequest[0];
            */
            foreach (var item in lst_appBase_MyRequest)
            {
                if (item.ID == currentSelectedItem.ID)
                    item.IsSelected = true;
                else
                    item.IsSelected = false;

                if (item.Created.HasValue) //server,local sort Created
                {
                    if (item.Created.Value.Date == DateTime.Now.Date) // today
                    {
                        List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                        if (dict_workflow.ContainsKey(KEY_TODAY))
                            dict_workflow[KEY_TODAY].Add(item);
                        else
                            dict_workflow.Add(KEY_TODAY, lst_temp);
                    }
                    else if (item.Created.Value.Date == DateTime.Now.AddDays(-1).Date) // hom qua
                    {

                        List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                        if (dict_workflow.ContainsKey(KEY_YESTERDAY))
                            dict_workflow[KEY_YESTERDAY].Add(item);
                        else
                            dict_workflow.Add(KEY_YESTERDAY, lst_temp);
                    }
                    else if (item.Created.Value.Date < DateTime.Now.AddDays(-1).Date) // cuhon
                    {
                        List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                        if (dict_workflow.ContainsKey(KEY_ORTHER))
                            dict_workflow[KEY_ORTHER].Add(item);
                        else
                            dict_workflow.Add(KEY_ORTHER, lst_temp);
                    }
                }
            }
            if (currentSelectedItem == null)
            {
                //currentSelectedItem = lst_appBase_cxl[0];
                if (dict_workflow[KEY_TODAY].Count > 0)
                {
                    dict_workflow[KEY_TODAY][0].IsSelected = true;
                    currentSelectedItem = dict_workflow[KEY_TODAY][0];
                }
                else if (dict_workflow[KEY_YESTERDAY].Count > 0)
                {
                    dict_workflow[KEY_YESTERDAY][0].IsSelected = true;
                    currentSelectedItem = dict_workflow[KEY_YESTERDAY][0];
                }
                else if (dict_workflow[KEY_ORTHER].Count > 0)
                {
                    dict_workflow[KEY_ORTHER][0].IsSelected = true;
                    currentSelectedItem = dict_workflow[KEY_ORTHER][0];
                }
            }
            lst_appBase_MyRequest.ForEach(o => o.IsSelected = o.ID == currentSelectedItem.ID);


            table_workflow.Hidden = false;
            lbl_nodata_left.Hidden = true;

            table_workflow.Source = new WorkFlow_TableSource(dict_workflow, this);
            table_workflow.ReloadData();
        }

        /// <summary>
        /// Sort section from result list
        /// </summary>
        private void SortListSearchAppBase(List<BeanAppBaseExt> lstSeach)
        {
            var dict = new Dictionary<string, List<BeanAppBaseExt>>();
            string KEY_TODAY = "TEXT_TODAY`Hôm nay";
            string KEY_YESTERDAY = "TEXT_YESTERDAY`Hôm qua";
            string KEY_ORTHER = "TEXT_OLDER`Cũ hơn";

            List<BeanAppBaseExt> lst_temp1 = new List<BeanAppBaseExt>();
            List<BeanAppBaseExt> lst_temp2 = new List<BeanAppBaseExt>();
            List<BeanAppBaseExt> lst_temp3 = new List<BeanAppBaseExt>();
            dict.Add(KEY_TODAY, lst_temp1);
            dict.Add(KEY_YESTERDAY, lst_temp2);
            dict.Add(KEY_ORTHER, lst_temp3);

            foreach (var item in lstSeach)
            {
                //if (item.ID == currentSelectedItem.ID)
                //    item.IsSelected = true;
                //else
                //    item.IsSelected = false;

                if (item.Created.HasValue) //server,local sort Created
                {
                    if (item.Created.Value.Date == DateTime.Now.Date) // today
                    {
                        List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                        if (dict.ContainsKey(KEY_TODAY))
                            dict[KEY_TODAY].Add(item);
                        else
                            dict.Add(KEY_TODAY, lst_temp);
                    }
                    else if (item.Created.Value.Date == DateTime.Now.AddDays(-1).Date) // hom qua
                    {

                        List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                        if (dict.ContainsKey(KEY_YESTERDAY))
                            dict[KEY_YESTERDAY].Add(item);
                        else
                            dict.Add(KEY_YESTERDAY, lst_temp);
                    }
                    else if (item.Created.Value.Date < DateTime.Now.AddDays(-1).Date) // cuhon
                    {
                        List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                        if (dict.ContainsKey(KEY_ORTHER))
                            dict[KEY_ORTHER].Add(item);
                        else
                            dict.Add(KEY_ORTHER, lst_temp);
                    }
                }
            }

            table_workflow.Hidden = false;
            lbl_nodata_left.Hidden = true;

            table_workflow.Source = new WorkFlow_TableSource(dict, this);
            table_workflow.ReloadData();
        }

        string LoadData_count(int countNum)
        {
            string str_translate = "";
            int _dataCount = 0;
            try
            {
                //var conn = new SQLiteConnection(CmmVariable.M_DataPath);
                //var queryCount = CreateQueryFromMe(true);
                //var countObj = conn.Query<CountNum>(queryCount);
                //_dataCount = (countObj != null && countObj.Count > 0) ? countObj[0].count : 0;

                //string str_fromMe = CmmFunction.GetTitle("TEXT_IPAD_FROMME", "Trang chủ > Tôi bắt đầu");
                _dataCount = countNum;//GetCountNumber();
                string str_fromMe = CmmFunction.GetTitle("TEXT_INPROCESS", BT_DangXL.TitleLabel.Text);
                if (_dataCount >= 100)
                {
                    str_translate = str_fromMe + " (99+)";
                }
                else if (_dataCount > 0 && _dataCount < 100)
                {
                    if (_dataCount > 0 && _dataCount < 10)
                        str_translate = str_fromMe + " (0" + _dataCount.ToString() + ")";// hien thi 2 so vd: 08
                    else
                        str_translate = str_fromMe + " (" + _dataCount.ToString() + ")";
                }
                else
                    str_translate = str_fromMe;
            }
            catch (Exception ex)
            {
                Console.WriteLine("MainView - loadData - Err: " + ex.ToString());
            }
            return str_translate;
        }

        int GetCountNumber()
        {
            int vcxl_count = 0;
            try
            {
                string count = "";

                count = new ProviderControlDynamic().GetListCountVDT_VTBD(CmmVariable.KEY_COUNT_FROMME_INPROCESS);

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

        #region old functon
        /// <summary>
        /// Không còn dùng
        /// </summary>
        /// <param name="_limit"></param>
        /// <param name="_offset"></param>
        public void LoadDataFilterFromMe_loadmore(int _limit, int _offset)
        {
            try
            {
                if (isFilter)
                    BT_filter_search.TintColor = UIColor.Orange;
                else
                    BT_filter_search.TintColor = UIColor.Black;

                dict_workflow = new Dictionary<string, List<BeanAppBaseExt>>();
                dict_sectionWorkFlow = new Dictionary<string, bool>();

                if (isOnline)
                {
                    LoadDataOnline();

                    if (lst_appBase_MyRequest != null & lst_appBase_MyRequest.Count > 0)
                    {
                        if (currentSelectedItem != null)
                            currentSelectedItem.IsSelected = true;
                        else
                            currentSelectedItem = lst_appBase_MyRequest[0];
                        SortListAppBase();
                    }
                    //else
                    //{ 
                    //table_workflow.Hidden = true;
                    //lbl_nodata_left.Hidden = false;

                    //    view_task_right.Hidden = true;
                    //    lbl_nodata_right.Hidden = false;
                    //}
                }
                else
                {
                    var conn = new SQLiteConnection(CmmVariable.M_DataPath);
                    string count_query_fromMe = string.Empty;
                    string query = string.Empty;
                    List<BeanAppBaseExt> lst_appBase_MyRequest_more = new List<BeanAppBaseExt>();

                    // default 
                    string defaultStatus_fromMe = CmmFunction.GetAppSettingValue("MOBILE_APPSTATUS_FROMME");
                    if (!string.IsNullOrEmpty(defaultStatus_fromMe))
                        defaultStatus_fromMe = string.Format(@" AND AB.StatusGroup IN({0})", defaultStatus_fromMe);

                    query = string.Format(@"SELECT AB.* FROM BeanAppBase AB
                                        LEFT JOIN BeanAppStatus AST
                                            ON AST.ID = AB.StatusGroup
                                        WHERE AB.CreatedBy LIKE '%{0}%' {1}
                                        ORDER BY AB.Created DESC LIMIT ? OFFSET ?", CmmVariable.SysConfig.UserId.ToUpper(), defaultStatus_fromMe);
                    lst_appBase_MyRequest_more = conn.Query<BeanAppBaseExt>(query, _limit, _offset);

                    if (lst_appBase_MyRequest_more != null & lst_appBase_MyRequest_more.Count > 0)
                    {
                        lst_appBase_MyRequest.AddRange(lst_appBase_MyRequest_more);
                        if (lst_appBase_MyRequest_more.Count < CmmVariable.M_DataLimitRow)
                            isLoadMore = false;

                        /*string str_toMe = CmmFunction.GetTitle("TEXT_IPAD_FROMME", "Trang chủ > Tôi bắt đầu");
                        if (fromMe_count >= 100)
                            lbl_topBar_title.Text = str_toMe + " (99+)";
                        else if (fromMe_count > 0 && fromMe_count < 100)
                        {
                            str_toMe = str_toMe + " (" + fromMe_count.ToString() + ")";
                            lbl_topBar_title.Text = str_toMe;
                        }
                        else
                            lbl_topBar_title.Text = str_toMe;

                        var str_transalte = lbl_topBar_title.Text;
                        var indexA = str_transalte.IndexOf('(');
                        if (indexA != -1)
                        {
                            NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                            att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(94, 94, 94), new NSRange(0, str_transalte.Length));
                            att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(255, 122, 58), new NSRange(indexA, str_transalte.Length - indexA));
                            lbl_topBar_title.AttributedText = att;
                        }*/
                        SetLangTitle();

                        if (currentSelectedItem != null)
                            currentSelectedItem.IsSelected = true;
                        else
                            currentSelectedItem = lst_appBase_MyRequest[0];
                        SortListAppBase();
                        /*
                        const string KEY_TODAY = "Hôm nay";
                        const string KEY_YESTERDAY = "Hôm qua";
                        const string KEY_ORTHER = "Cũ hơn";

                        List<BeanAppBaseExt> lst_temp1 = new List<BeanAppBaseExt>();
                        List<BeanAppBaseExt> lst_temp2 = new List<BeanAppBaseExt>();
                        List<BeanAppBaseExt> lst_temp3 = new List<BeanAppBaseExt>();
                        dict_workflow.Add(KEY_TODAY, lst_temp1);
                        dict_workflow.Add(KEY_YESTERDAY, lst_temp2);
                        dict_workflow.Add(KEY_ORTHER, lst_temp3);

                        foreach (var item in lst_appBase_MyRequest)
                        {
                            if (item.ID == currentItemSelected.ID)
                                item.IsSelected = true;
                            else
                                item.IsSelected = false;

                            if (item.Created.Value.Date == DateTime.Now.Date) // today
                            {
                                List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                                if (dict_workflow.ContainsKey(KEY_TODAY))
                                    dict_workflow[KEY_TODAY].Add(item);
                                else
                                    dict_workflow.Add(KEY_TODAY, lst_temp);
                            }
                            else if (item.Created.Value.Date == DateTime.Now.AddDays(-1).Date) // hom qua
                            {

                                List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                                if (dict_workflow.ContainsKey(KEY_YESTERDAY))
                                    dict_workflow[KEY_YESTERDAY].Add(item);
                                else
                                    dict_workflow.Add(KEY_YESTERDAY, lst_temp);
                            }
                            else if (item.Created.Value.Date < DateTime.Now.AddDays(-1).Date) // cuhon
                            {
                                List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                                if (dict_workflow.ContainsKey(KEY_ORTHER))
                                    dict_workflow[KEY_ORTHER].Add(item);
                                else
                                    dict_workflow.Add(KEY_ORTHER, lst_temp);
                            }
                        }

                        table_workflow.Hidden = false;
                        lbl_nodata_left.Hidden = true;

                        table_workflow.Source = new WorkFlow_TableSource(dict_workflow, this);
                        table_workflow.ReloadData();*/
                    }
                    else
                        isLoadMore = false;

                }

                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    tf_search.Text = searchKeyword;
                    searchData();
                }
            }
            catch (Exception ex)
            {
                //lbl_topBar_title.Text = CmmFunction.GetTitle("TEXT_IPAD_FROMME", "Trang chủ > Tôi bắt đầu");
                table_workflow.Source = null;
                table_workflow.ReloadData();
                Console.WriteLine("MyRequestListView - LoadDataFilterFromMe_loadmore - Err: " + ex.ToString());
            }
        }
        #endregion

        public void LoadDataFilterFromServer(bool isLoadmore)
        {
            if (!isLoadmore)
            {
                lstViewLoading = new CmmLoading(new CGRect(view_task_left.Center.X - 100, view_task_left.Center.Y - 100, 200, 200), "Đang xử lý...");
                view_task_left.Add(lstViewLoading);
            };

            try
            {
                if (isFilter)
                    BT_filter_search.TintColor = UIColor.Orange;
                else
                    BT_filter_search.TintColor = UIColor.Black;

                dict_workflow = new Dictionary<string, List<BeanAppBaseExt>>();
                dict_sectionWorkFlow = new Dictionary<string, bool>();

                if (lst_appBase_MyRequest == null)
                    lst_appBase_MyRequest = new List<BeanAppBaseExt>();

                //Check Filter Status
                if (lst_appStatus != null)
                    lst_appStatus_selected = lst_appStatus.Where(s => s.IsSelected == true).ToList();

                string str_status = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS); // tat ca fromme
                if (lst_appStatus_selected != null && lst_appStatus_selected.Count > 0) // co chon status
                    str_status = string.Join(',', lst_appStatus_selected.Select(i => i.ID));

                //Check filter DueDate
                if (lst_dueDateMenu != null)
                    DuedateSelected = lst_dueDateMenu.Where(s => s.isSelected == true).FirstOrDefault();

                ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();
                int totalRecord = 0;
                var listPropertiesFilter = CmmFunction.BuildListPropertiesFilter(CmmVariable.M_ResourceViewID_ToMe, null, str_status, DuedateSelected.ID, fromDateSelected, toDateSelected);

                //loadmore then cu filter
                if (isLoadmore)
                {
                    List<BeanAppBaseExt> lst_appBase_MyRequest_more = new List<BeanAppBaseExt>();
                    lst_appBase_MyRequest_more = _pControlDynamic.GetListFilterMyRequest(listPropertiesFilter, ref totalRecord, CmmVariable.M_DataFilterAPILimitData, lst_appBase_MyRequest.Count);// lst_appBase_fromMe.Count
                    lst_appBase_MyRequest_more = lst_appBase_MyRequest_more.OrderByDescending(s => s.NotifyCreated).ToList();

                    if (lst_appBase_MyRequest_more != null && lst_appBase_MyRequest_more.Count > 0)
                    {
                        lst_appBase_MyRequest.AddRange(lst_appBase_MyRequest_more);
                    }
                    else
                        return;
                }
                // Lan dau filter
                else
                {
                    lst_appBase_MyRequest = _pControlDynamic.GetListFilterMyRequest(listPropertiesFilter, ref totalRecord, CmmVariable.M_DataFilterAPILimitData, 0);// lst_appBase_fromMe.Count
                    lst_appBase_MyRequest = lst_appBase_MyRequest.OrderByDescending(s => s.NotifyCreated).ToList();
                    //fromMe_count = totalRecord;
                }

                if (lst_appBase_MyRequest != null & lst_appBase_MyRequest.Count > 0)
                {
                    #region Lấy danh sách đang xử lý
                    var inProgressStat = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS_FROMME_DANGXULY);
                    string[] arrayInProgressStatus = !string.IsNullOrEmpty(inProgressStat) ? inProgressStat.Split(",") : new string[] { };
                    var lstInProgress = lst_appBase_MyRequest.FindAll(o => arrayInProgressStatus.Contains(o.StatusGroup.ToString()));
                    #endregion

                    #region Lấy danh sách đã xử lý
                    var progressedStat = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS_FROMME_DAXULY);
                    string[] arrayProgressedStatus = !string.IsNullOrEmpty(progressedStat) ? progressedStat.Split(",") : new string[] { };
                    var lstProgressed = lst_appBase_MyRequest.FindAll(o => arrayProgressedStatus.Contains(o.StatusGroup.ToString()));
                    #endregion

                    //SetLangTitle();
                    BT_DangXL.SetTitle(LoadData_count(tab_inprogress ? lstInProgress.Count : 0), UIControlState.Normal);
                    ToggleTodo();

                    if (tab_inprogress)
                    {
                        if (lstInProgress != null && lstInProgress.Count > 0)
                        {
                            if (currentSelectedItem != null)
                                currentSelectedItem.IsSelected = true;
                            else
                                currentSelectedItem = lstInProgress[0];
                            SortListSearchAppBase(lstInProgress);
                            return;
                        }
                    }
                    else
                    {
                        if (lstProgressed != null && lstProgressed.Count > 0)
                        {
                            if (currentSelectedItem != null)
                                currentSelectedItem.IsSelected = true;
                            else
                                currentSelectedItem = lstProgressed[0];
                            SortListSearchAppBase(lstProgressed);
                            return;
                        }
                    }

                    table_workflow.Source = null;
                    table_workflow.ReloadData();
                    table_workflow.Hidden = true;

                    lbl_nodata_left.Hidden = false;
                    lbl_nodata_right.Hidden = false;
                }
                else
                {
                    table_workflow.Hidden = true;
                    lbl_nodata_left.Hidden = false;

                    view_task_right.Hidden = true;
                    lbl_nodata_right.Hidden = false;
                    BT_DangXL.SetTitle(LoadData_count(0), UIControlState.Normal);
                    ToggleTodo();
                }

                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    tf_search.Text = searchKeyword;
                    searchData();
                }
            }
            catch (Exception ex)
            {
                //lbl_topBar_title.Text = CmmFunction.GetTitle("TEXT_IPAD_FROMME", "Trang chủ > Tôi bắt đầu");
                table_workflow.Source = null;
                table_workflow.ReloadData();
                Console.WriteLine("MyRequestListView - LoadDataFilterFromMe_loadmore - Err: " + ex.ToString());
            }
            finally
            {
                if (!isLoadmore) lstViewLoading?.Hide();
            }
        }

        public async void LoadmoreData()
        {
            view_request_left_loadmore.Hidden = false;
            loadmore_indicator.StartAnimating();

            await Task.Run(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5f));
                InvokeOnMainThread(() =>
                {
                    if (isFilter)
                        LoadDataFilterFromServer(true);
                    else
                        LoadDataFilterFromMe_loadmore(limit, lst_appBase_MyRequest.Count);

                    loadmore_indicator.StopAnimating();
                    view_request_left_loadmore.Hidden = true;
                });
            });
        }

        private void SetLangTitle()
        {
            /*string str_transalte = CmmFunction.GetTitle("TEXT_IPAD_FROMME", "Trang chủ > Tôi bắt đầu");
            NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
            att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(94, 94, 94), new NSRange(0, str_transalte.Length));
            //den toi
            var indexB = str_transalte.IndexOf('/');
            if (indexB != -1)
            {
                att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 104, 133), new NSRange(indexB + 1, str_transalte.Length - indexB - 1));
            }
            else
            {
                indexB = str_transalte.IndexOf('>');
                if (indexB != -1)
                {
                    att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 104, 133), new NSRange(indexB + 2, str_transalte.Length - indexB - 2));
                }
            }
            
            ////number
            //var indexA = str_transalte.IndexOf('(');
            //if (indexA != -1)
            //{
            //    att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(255, 122, 58), new NSRange(indexA, str_transalte.Length - indexA));
            //}
            lbl_topBar_title.AttributedText = att;*/

            lbl_nodata_left.Text = string.Compare(CmmVariable.SysConfig.LangCode, "1033") == 0 ? "No data yet" : "Không có dữ liệu";
            lbl_nodata_right.Text = lbl_nodata_left.Text;

            if (!isFilter)
                BT_DangXL.SetTitle(LoadData_count(GetCountNumber()), UIControlState.Normal);
            ToggleTodo();
        }

        private void LoadTinhTrangCategory()
        {
            if (lst_appStatus == null || lst_appStatus.Count == 0)
            {
                var condition = CmmFunction.GetAppSettingValue("MOBILE_APPSTATUS_ENABLE");//
                var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                string query_worflowCategory = string.Format(@"SELECT * FROM BeanAppStatus WHERE ID IN ({0})", condition);
                lst_appStatus = conn.Query<BeanAppStatus>(query_worflowCategory);

                //var defaultStatus = CmmFunction.GetAppSettingValue(tab_inprogress ? CmmVariable.APPSTATUS_FROMME_DANGXULY : CmmVariable.APPSTATUS_FROMME_DAXULY);
                //var defaultSelected = defaultStatus.Split(",");
                //if (defaultSelected != null && defaultSelected.Length > 0)
                //    lst_appStatus.ForEach(o =>
                //    {
                //        //if (defaultSelected != null && defaultSelected.Length > 0 && defaultSelected.Contains(o.ID.ToString()))
                //        //{
                //        //    o.IsSelected = true;
                //        //}
                //        //else
                //        o.IsSelected = defaultSelected.Any(s => string.Compare(s, o.ID.ToString()) == 0);//defaultSelected.Contains()
                //    });
            }
            var defaultStatus = CmmFunction.GetAppSettingValue(tab_inprogress ? CmmVariable.APPSTATUS_FROMME_DANGXULY : CmmVariable.APPSTATUS_FROMME_DAXULY);
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

        private void LoadDueDateCategory()
        {
            lst_dueDateMenu = new List<ClassMenu>();
            ClassMenu m1 = new ClassMenu() { ID = 1, section = 0, title = CmmFunction.GetTitle("TEXT_ALL", "Tất cả") };
            ClassMenu m2 = new ClassMenu() { ID = 2, section = 0, title = CmmFunction.GetTitle("TEXT_TODAY1", "Trong ngày") };
            ClassMenu m3 = new ClassMenu() { ID = 3, section = 0, title = CmmFunction.GetTitle("TEXT_OVERDUE", "Trễ hạn") };

            lst_dueDateMenu.AddRange(new[] { m1, m2, m3 });
        }

        public void ReLoadTableListWorkFlow()
        {
            table_workflow.ReloadData();
        }

        private async void LoadItemSelected(bool isScrollToTop)
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(CmmVariable.M_DataPath, false);
            ShowObjContent();
            try
            {

                //hide menu button default va table content, show lai khi load data hoan tat
                //table_content_right.Hidden = true;

                lst_addAttachment = new List<BeanAttachFile>();
                lst_addCommentAttachment = new List<BeanAttachFile>();

                if (isScrollToTop)
                {
                    view_buttonDefault.Hidden = true;
                    table_content_right.ScrollsToTop = true;
                }

                if (currentSelectedItem != null)
                {
                    if (currentSelectedItem.ResourceCategoryId == 8)
                        isTask = false;
                    else if (currentSelectedItem.ResourceCategoryId == 16)
                        isTask = true;

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
                                    workflowItem = new ProviderControlDynamic().getWorkFlowItemByRID(workflowID).FirstOrDefault();
                                }
                                catch (Exception ex)
                                {
                                    workflowItem = null;
                                    Console.WriteLine("Không lấy item được: " + ex.ToString());
                                }
                            }
                        }

                        if (workflowItem == null)
                        {
                            //view_task_right.Hidden = true;
                            //lbl_nodata_right.Hidden = false;
                            //loading.Hide();
                            InvokeOnMainThread(() =>
                            {
                                ShowObjContent(true, false);
                            });
                            Console.WriteLine("Item được chọn vẫn null.");
                            return;
                        }

                        List<BeanWorkflowFollow> lst_follow = new List<BeanWorkflowFollow>();
                        string query_follow = string.Format("SELECT * FROM BeanWorkflowFollow WHERE WorkflowItemId = ?");
                        var lst_followResult = conn.QueryAsync<BeanWorkflowFollow>(query_follow, workflowItem.ID).Result;
                        InvokeOnMainThread(() =>
                        {
                            if (lst_followResult != null && lst_followResult.Count > 0)
                            {
                                if (lst_followResult[0].Status == 1)
                                {
                                    BT_start.SetImage(UIImage.FromFile("Icons/icon_Star_on.png"), UIControlState.Normal);
                                    isFollow = true;
                                }
                                else
                                {
                                    BT_start.SetImage(UIImage.FromFile("Icons/icon_Star_off.png"), UIControlState.Normal);
                                    isFollow = false;
                                }
                            }
                            else
                            {
                                BT_start.SetImage(UIImage.FromFile("Icons/icon_Star_off.png"), UIControlState.Normal);
                                isFollow = false;
                            }
                        });

                        //if (CmmVariable.SysConfig.LangCode == "1066")
                        //    lbl_listName.Text = workflowItem.WorkflowTitle;
                        //else if (CmmVariable.SysConfig.LangCode == "1033")
                        //    lbl_listName.Text = workflowItem.WorkflowTitleEN;

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

                            #region AssignedTo - lay danh sach nguoi xu ly hien tai tu workflowItem
                            string assignedTo = workflowItem.AssignedTo;//currentItemSelected.AssignedTo;
                            arr_assignedTo = null;
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
                                        att.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(lbl_assignedTo.Font.PointSize, UIFontWeight.Semibold), new NSRange(indexA, res.Length - indexA));
                                        lbl_assignedTo.AttributedText = att;//(att, UIControlState.Normal);
                                    }
                                    lbl_assignedTo.Text = res.TrimEnd(','); // nguoi xu ly hien tai
                                }
                            }
                            #endregion
                        });

                        ProviderControlDynamic p_controlDynamic = new ProviderControlDynamic();
                        Json_FormDataString = p_controlDynamic.GetTicketRequestControlDynamicForm(workflowItem, CmmVariable.SysConfig.LangCode);

                        if (!string.IsNullOrEmpty(Json_FormDataString))
                        {
                            JObject retValue = JObject.Parse(Json_FormDataString);
                            JArray json_dataForm = JArray.Parse(retValue["form"].ToString());
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

                            if (workflowItem.CommentChanged < _CommentChanged || _CommentChanged == new DateTime())
                            {
                                workflowItem.IsChange = true;
                            }

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

                            bool isFollow = false;
                            currentSelectedItem.IsFollow = bool.TryParse(json_dataForm[0]["IsFollow"].ToString(), out isFollow) ? isFollow : false;//Convert.ToBoolean(json_dataForm[0]["IsFollow"]);
                            str_json_FormDefineInfo = json_dataForm[0]["FormDefineInfo"].ToString();
                            lst_section = json_dataForm.ToObject<List<ViewSection>>();
                            var result = lst_section.SelectMany(s => s.ViewRows)
                                        .FirstOrDefault(s => s.Elements.Any(d => d.DataType == "inputattachmenthorizon"));

                            var isReadOnly = workflowItem.StatusGroup.HasValue && workflowItem.StatusGroup.Value == 1;

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
                                //if (lstWorkFlowRelateds != null && lstWorkFlowRelateds.Count > 0)
                                //    table_content_right.Source = new Control_TableSource(lst_section, lstWorkFlowRelateds, this);
                                //else
                                //    table_content_right.Source = new Control_TableSource(lst_section, null, this);

                                ShowObjContent(true, true);

                                if (isScrollToTop)
                                    table_content_right.ScrollRectToVisible(new CGRect(0, 0, 0, 0), false);

                                #region get danh sách action
                                if (isReadOnly)
                                {
                                    view_buttonDefault.Hidden = false;
                                    BT_comment.Enabled = false;
                                    BT_attachement.Enabled = false;
                                }
                                else
                                {
                                    BT_comment.Enabled = true;
                                    BT_attachement.Enabled = true;

                                    //constant_ButtonactionDefaut.Constant = view_buttonAction_default.Width;
                                    //view_buttonAction.Frame = new CGRect(view_buttonAction_default.X, view_buttonAction.Frame.Y, view_buttonAction_default.Width, view_buttonAction.Frame.Height);
                                    JObject jsonButtonBot = JObject.Parse(retValue["action"].ToString());
                                    var buttonBot = jsonButtonBot.ToObject<ViewRow>();
                                    buttonBot.Elements = CmmFunction.SortListElementAction(buttonBot.Elements);

                                    if (componentButton != null)
                                        componentButton.RemoveFromSuperview();

                                    if (buttonBot.Elements == null || buttonBot.Elements.Count == 0)
                                    {
                                        isReadOnly = workflowItem.StatusGroup.HasValue && workflowItem.StatusGroup.Value == 4;// phiếu đang yêu cầu hiệu chỉnh thì không đc thao tác trên app
                                    }
                                    else
                                    {
                                        componentButton = new ComponentButtonBot(this, buttonBot);
                                        int count_item;
                                        if (buttonBot.Elements.Count > 4)
                                            count_item = 4;
                                        else
                                            count_item = buttonBot.Elements.Count;

                                        nfloat buttonWidth = (view_buttonAction.Frame.Width - 20) / 4;
                                        int item_menuMiss = 4 - count_item;
                                        nfloat startPoint = view_buttonAction.Frame.X + item_menuMiss * buttonWidth;

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

                                    //show menu button default va table content
                                    view_buttonDefault.Hidden = false;
                                }
                                #endregion

                                table_content_right.Source = new Control_TableSource(lst_section, lstWorkFlowRelateds, lst_tasks, lst_comments, this, isReadOnly);
                                table_content_right.ReloadData();

                                if (isReadOnly)
                                    CmmIOSFunction.commonAlertMessage(this, "VNA BPM", CmmFunction.GetTitle(CmmVariable.TEXT_ALERT_DRAFT, "Vui lòng sử dụng phiên bản web để chỉnh sửa phiếu này!"));
                            });

                            loadQuaTrinhluanchuyen();
                        }
                        else
                        {
                            //loading.Hide();
                            //view_task_right.Hidden = true;
                            //lbl_nodata_right.Hidden = false;
                            InvokeOnMainThread(() =>
                            {
                                ShowObjContent(true, false);
                            });

                            UIAlertController alert = UIAlertController.Create("Thông báo", "Không tìm thấy thông tin phiếu, vui lòng thử lại sau.", UIAlertControllerStyle.Alert);//"BPM"
                            alert.AddAction(UIAlertAction.Create("Đóng", UIAlertActionStyle.Default, alertAction =>
                            {

                            }));
                            this.PresentViewController(alert, true, null);
                        }
                    });

                    await Task.Run(() =>
                    {
                        //Kiem tra co phai Cong Viec hay khong
                        if (currentSelectedItem.ResourceCategoryId == (int)CmmFunction.CommentResourceCategoryID.Task && currentSelectedItem.ResourceSubCategoryId == 0 && isShowTask)
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
                    //lbl_nodata_left.Hidden = false;

                    ShowObjContent(true, false);
                    loading.Hide();
                }
            }
            catch (Exception ex)
            {
                ShowObjContent(true, false);
                Console.WriteLine("WorkflowDetailView - GetIndexItemFromDictionnary - ERR: " + ex.ToString());
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
                lbl_nodata_right.Hidden = hasValue;
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
                lbl_nodata_right.Hidden = true;
                view_userInfo.Hidden = true;
            }
            lbl_listName.Hidden = view_task_right.Hidden;
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
            LoadTinhTrangCategory();
            LoadDueDateCategory();
            //LoadDueDateCategory();

            isFilter = false;
        }

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
        #endregion

        private async void loadQuaTrinhluanchuyen()
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
                        lst_userResult = conn.QueryAsync<BeanUser>(query_user0, currentSelectedItem.CreatedBy.Trim().ToLower()).Result;
                    }
                    else
                    {
                        string userAssignedID = lst_qtlc.OrderBy(t => t.Created).ToList()[0].AssignUserId;
                        lst_userResult = conn.QueryAsync<BeanUser>(query_user0, userAssignedID.Trim().ToLower()).Result;
                    }

                    InvokeOnMainThread(() =>
                    {
                        string user_imagePath = "";
                        if (lst_userResult.Count > 0)
                            user_imagePath = lst_userResult[0].ImagePath;

                        if (string.IsNullOrEmpty(user_imagePath))
                        {
                            lbl_imgCover.Hidden = false;
                            img_avatar_sentUnit.Hidden = true;
                            lbl_imgCover.Text = CmmFunction.GetAvatarName(lst_userResult[0].FullName);
                            lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));

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

                        //lbl_sender.Text = lst_userResult[0].FullName + " (" + lst_userResult[0].Position + ")";
                        lbl_sender.Text = lst_userResult[0].FullName;
                    });
                }
            });
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
                            if (!String.IsNullOrEmpty(element.Formula))
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
                if (!string.IsNullOrEmpty(content))
                {
                    var items = from item in lst_appBase_MyRequest
                                where ((!string.IsNullOrEmpty(item.Title) && CmmFunction.removeSignVietnamese(item.Title.ToLowerInvariant()).Contains(content)) ||
                                           (!string.IsNullOrEmpty(item.Content) && CmmFunction.removeSignVietnamese(item.Content.ToLowerInvariant()).Contains(content)) ||
                                           (!string.IsNullOrEmpty(item.ListName) && CmmFunction.removeSignVietnamese(item.ListName.ToLowerInvariant()).Contains(content)))
                                orderby item.Title
                                select item;

                    if (items != null && items.Count() > 0)
                    {
                        lst_workflow_fromMe_result = items.ToList();
                        /*if (dict_workflow_result.ContainsKey("Today"))
                            dict_workflow_result["Today"] = lst_workflow_fromMe_result;
                        else
                            dict_workflow_result.Add("Today", lst_workflow_fromMe_result);

                        table_workflow.Hidden = false;
                        lbl_nodata_left.Hidden = true;
                        table_workflow.Source = new WorkFlow_TableSource(dict_workflow_result, this);
                        table_workflow.ReloadData();*/

                        isLoadMore = false;
                        if (isFromMain)
                        {
                            currentSelectedItem = lst_workflow_fromMe_result.FirstOrDefault();
                            currentSelectedItem.IsSelected = true;
                        }
                        SortListSearchAppBase(lst_workflow_fromMe_result);
                    }
                    else
                    {
                        table_workflow.Hidden = true;
                        lbl_nodata_left.Hidden = false;
                        table_workflow.Source = null;
                        table_workflow.ReloadData();
                    }
                }
                else
                {
                    table_workflow.Hidden = false;
                    lbl_nodata_left.Hidden = true;

                    isLoadMore = true;

                    //table_workflow.Source = new WorkFlow_TableSource(dict_workflow, this);
                    //table_workflow.ReloadData();
                    SortListSearchAppBase(lst_appBase_MyRequest);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("WorkflowDetails - Tf_search_EditingChanged - Err: " + ex.ToString());
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
                loading.Hide();

                CloseAddFollow();
                HandleAttachFileClose();
                Custom_MenuAction custom_menuAction = Custom_MenuAction.Instance;
                if (custom_menuAction.Superview != null)
                    custom_menuAction.RemoveFromSuperview();
                HandleWorkFollowViewResult();

                currentSelectedItem = _itemSelected;
                if (currentSelectedItem.ResourceCategoryId == (int)CmmFunction.CommentResourceCategoryID.Task)
                    isShowTask = true;

                foreach (var item in lst_appBase_MyRequest)
                {
                    if (item.ID == currentSelectedItem.ID)
                        item.IsSelected = true;
                    else
                        item.IsSelected = false;

                }
                table_workflow.ReloadData();
                LoadItemSelected(true);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("WorkflowDetailView - UpdateItemSelect - err : " + ex.ToString());
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

        private NSIndexPath GetIndexItemFromDictionnary(BeanAppBaseExt _item)
        {
            NSIndexPath indexTemp = null;
            var i = 0;
            do
            {
                var j = 0;
                var key = dict_workflow.ElementAt(i).Key;
                do
                {
                    if (_item == dict_workflow[key][j])
                    {
                        indexTemp = NSIndexPath.FromRowSection(j, i);
                        break;
                    }
                    j++;
                }
                while (j < dict_workflow[key].Count);
                i++;
            }
            while (i < dict_workflow.Count && indexTemp == null);

            return indexTemp;
        }

        public void CloseAddFollow()
        {
            Custom_AddFollowView custom_AddFollowView = Custom_AddFollowView.Instance;
            if (custom_AddFollowView.Superview != null)
                custom_AddFollowView.RemoveFromSuperview();
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
                    transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
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
            loading = new CmmLoading(new CGRect((this.View.Bounds.Width - 200) / 2, (this.View.Bounds.Height - 200) / 2, 200, 200), "Đang xử lý...");
            this.View.Add(loading);
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
                        if (element.Enable)
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
                            else if ((element.DataType != "inputcomments"))
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
                        result = providerControl.SendControlDynamicAction(_buttonAction.Value, currentSelectedItem.ID.ToString(), str_json_FormDefineInfo, json_edit_element, ref str_errMess, lst_files, lstExtent);
                    else
                        result = providerControl.SendControlDynamicAction(_buttonAction.Value, currentSelectedItem.ID.ToString(), str_json_FormDefineInfo, json_edit_element, ref str_errMess, null);

                    if (result)
                    {
                        b_pase.UpdateAllDynamicData(true);
                        InvokeOnMainThread(() =>
                        {
                            loading.Hide();

                            //status_selected_index = 1;
                            //status_selected_index_default = 1;
                            //duedate_selected_index = 0;
                            //duedate_selected_index_default = 0;

                            // currentItemSelected = null, auto focus to item first
                            if (_buttonAction.Value != "Follow" && _buttonAction.Value != "Save")
                            {
                                currentSelectedItem = null;
                                //LoadDataFilterFromMe(fromDate_default, toDate_default);
                                LoadDataFilterFromMe();
                                table_workflow.ScrollToRow(NSIndexPath.FromRowSection(0, 0), UITableViewScrollPosition.Top, true);
                            }
                            else
                                table_workflow.ReloadData();

                            LoadItemSelected(true);
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

        private void CloseMenuOption()
        {
            Custom_MenuOption custom_menuOption = Custom_MenuOption.Instance;
            if (custom_menuOption.Superview != null)
                custom_menuOption.RemoveFromSuperview();
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
                CmmIOSFunction.commonAlertMessage(this, "BPM", "Không tìm thấy thông tin phiếu");
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
                CmmIOSFunction.commonAlertMessage(this, "BPM", "Không tìm thấy thông tin phiếu");
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
            textViewControlView.setContent(this, 1, element.Enable, element, "");
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
                else if (element.DataType == "multiplechoice")
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
                CmmIOSFunction.commonAlertMessage(this, "BPM", "Không tìm thấy thông tin phiếu");
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
            Custom_AttachFileView custom_menuOption = Custom_AttachFileView.Instance;
            if (custom_menuOption.Superview != null)
                custom_menuOption.RemoveFromSuperview();
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
                    Title = _attachFile.Name + ";#" + DateTime.Now.ToString(@"dd/MM/yyyy hh:mm:ss", new CultureInfo("vi")),//DateTime.Now.ToShortTimeString()
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
                CmmIOSFunction.commonAlertMessage(this, "Thông báo", "Chỉ hỗ trợ thêm tập tin office đính kèm từ hệ điều hành 11 trở lên.");
        }
        public void NavigationToDocumentPicker()
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

                            BeanAttachFileLocal itemiCloudAndDevice = new BeanAttachFileLocal() { ID = -1, Name = fileName, Path = filePath, Size = size, Type = type };
                            //HandleAddAttachFileResult(itemiCloudAndDevice, addAttachmentsView);
                            HandleAddAttachFileResult(itemiCloudAndDevice, attachmentElement.DataType);
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
                CmmIOSFunction.commonAlertMessage(this, "Thông báo", "Chỉ hỗ trợ thêm tập tin office đính kèm từ hệ điều hành 11 trở lên.");
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
                    if (note.Key == "image")
                        note.Value = json_attachRemove;
                    else if (note.Key == "doc")
                        note.Value = _json_attachRemove;
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

            //var index = lst_attachFile.FindIndex(item => item.ID == attachFile.ID);
            var index = -1;
            if (!string.IsNullOrEmpty(attachFile.ID))
                index = lst_attachFile.FindIndex(item => item.ID == attachFile.ID);

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

                detail.SetContent(lst_result[0], isTask, detail);
                PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                detail.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;
                detail.ModalPresentationStyle = UIModalPresentationStyle.BlurOverFullScreen;
                detail.TransitioningDelegate = transitioningDelegate;
                this.PresentModalViewController(detail, true);
            }
            else
            {
                CmmIOSFunction.commonAlertMessage(this, "BPM", "Không tìm thấy thông tin phiếu");
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
                CmmIOSFunction.commonAlertMessage(this, "BPM", "Không tìm thấy thông tin phiếu");
            }
        }
        #endregion

        #region handle Assignment Tasks
        public async void Handle_RemoveTask(BeanTask _task, NSIndexPath nSIndexPath)
        {
            try
            {
                loading = new CmmLoading(new CGRect((view_task_right.Bounds.Width - 200) / 2, (view_task_right.Bounds.Height - 200) / 2, 200, 200), "Đang xử lý...");
                view_task_right.Add(loading);

                bool res = false;
                await Task.Run(() =>
                {
                    ProviderControlDynamic p_dynamic = new ProviderControlDynamic();
                    res = p_dynamic.DeleteDetailTaskForm(_task.ID);

                    InvokeOnMainThread(() =>
                    {
                        loading.Hide();

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

        #region handle Comments
        public void SubmitLikeAction(NSIndexPath sectionIndex, BeanComment comment)
        {
            UpdateTableSections(sectionIndex.Section, comment);
        }
        public async void SubmitComment(string content, List<BeanAttachFile> lst_commentAddAttachFile)
        {
            try
            {
                string commentvalue = null;
                if (!string.IsNullOrEmpty(content))
                    commentvalue = content;
                else
                {

                }

                ProviderControlDynamic p_dynamic = new ProviderControlDynamic();
                List<KeyValuePair<string, string>> _lstKeyVarAttachmentLocal = new List<KeyValuePair<string, string>>();
                if (lst_commentAddAttachFile != null && lst_commentAddAttachFile.Count > 0) // Lấy những file thêm mới từ App ra
                {
                    foreach (var item in lst_commentAddAttachFile)
                    {
                        if (item.ID == "")
                        {
                            string key = item.Title;
                            KeyValuePair<string, string> _UploadFile = new KeyValuePair<string, string>(key, item.Path);
                            _lstKeyVarAttachmentLocal.Add(_UploadFile);
                        }
                    }
                }

                await Task.Run(() =>
                {
                    BeanOtherResource beanOtherResource = new BeanOtherResource();
                    beanOtherResource.Content = commentvalue;
                    beanOtherResource.ResourceId = _OtherResourceId;
                    beanOtherResource.ResourceCategoryId = (int)CmmFunction.CommentResourceCategoryID.WorkflowItem;
                    beanOtherResource.ResourceSubCategoryId = 0;
                    beanOtherResource.Image = "";
                    beanOtherResource.ParentCommentId = null; // cmt mới nên ko có parent

                    bool _result = p_dynamic.AddComment(beanOtherResource, _lstKeyVarAttachmentLocal);

                    InvokeOnMainThread(() =>
                    {
                        if (_result)
                        {
                            try
                            {
                                loading.Hide();

                                //status_selected_index = 1;
                                //status_selected_index_default = 1;
                                //duedate_selected_index = 0;
                                //duedate_selected_index_default = 0;

                                //LoadDataFilterFromMe(status_selected_index, duedate_selected_index, fromDate_default, toDate_default);
                                LoadItemSelected(true);

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("TodoDetailsView - SubmitAction - Invoke - Err: " + ex.ToString());
                                CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"));
                            }
                        }
                        else
                        {
                            CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"));
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("FormCommentView - SubmitComment - Err: " + ex.ToString());
            }
        }
        //Comment - reply
        public void NavigateToReplyComment(NSIndexPath _itemIndex, BeanComment comment)
        {
            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            FormCommentView formComment = (FormCommentView)Storyboard.InstantiateViewController("FormCommentView");
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

        public void ReLoadDataFromServer(bool scrollToTopList, bool scrollToTopForm)
        {
            this.View.EndEditing(true);
            currentSelectedItem = null;

            /*if (isFilter)
                LoadDataFilterFromMe(fromDateSelected, toDateSelected);
            else
                LoadDataFilterFromMe(fromDate_default, toDate_default);
            */
            //LoadDataFilterFromMe(status_selected_index, duedate_selected_index, fromDate_default, toDate_default);
            LoadDataFilterFromMe();
            if (scrollToTopList)
            {
                if (lst_appBase_MyRequest != null && lst_appBase_MyRequest.Count > 0)
                {
                    for (int i = 0; i < table_workflow.NumberOfSections(); i++)
                    {
                        if (table_workflow.NumberOfRowsInSection(i) > 0)
                        {
                            table_workflow.ScrollToRow(NSIndexPath.FromRowSection(0, i), UITableViewScrollPosition.Top, true);
                            break;
                        }
                    }
                }
            }

            LoadItemSelected(scrollToTopForm);
        }
        #endregion

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

            //status_selected_index = 1;
            //status_selected_index_default = 1;
            //duedate_selected_index = 0;
            //duedate_selected_index_default = 0;

            //LoadDataFilterFromMe(status_selected_index, duedate_selected_index, fromDate_default, toDate_default);
            LoadItemSelected(true);
        }

        public void ReLoadDataFromServer(bool refreshAllList)
        {
            currentSelectedItem = null;

            /*if (isFilter)
                LoadDataFilterFromMe(fromDateSelected, toDateSelected);
            else
                LoadDataFilterFromMe(fromDate_default, toDate_default);
            */
            SetDefaultFilter();
            LoadDataFilterFromMe();

            //LoadDataFilterFromMe(status_selected_index, duedate_selected_index, fromDate_default, toDate_default);

            //if (refreshAllList)
            //{
            //    if (lst_appBase_MyRequest != null && lst_appBase_MyRequest.Count > 0)
            //    {
            //        for (int i = 0; i < table_workflow.NumberOfSections(); i++)
            //        {
            //            if (table_workflow.NumberOfRowsInSection(i) > 0)
            //            {
            //                table_workflow.ScrollToRow(NSIndexPath.FromRowSection(0, i), UITableViewScrollPosition.Top, true);
            //                break;
            //            }
            //        }
            //    }
            //}

            LoadItemSelected(false);
        }
        #endregion

        #region event
        private void BT_avatar_TouchUpInside(object sender, EventArgs e)
        {
            appD.menu.UpdateItemSelect(2);
            appD.SlideMenuController.OpenLeft();

            HandleCloseAddFollow();
        }

        private void BT_start_TouchUpInside(object sender, EventArgs e)
        {
            Custom_AddFollowView custom_AddFollowView = Custom_AddFollowView.Instance;
            if (custom_AddFollowView.Superview != null && custom_AddFollowView.viewController.GetType() == typeof(WorkflowDetailView))
                custom_AddFollowView.RemoveFromSuperview();
            else
            {
                if (isFollow)
                {
                    var width = StringExtensions.MeasureString("Hủy theo dõi công việc này", 14).Width + 20 + 70;
                    custom_AddFollowView.viewController = this;
                    custom_AddFollowView.isFollow = isFollow;
                    custom_AddFollowView.LoadContent();
                    custom_AddFollowView.InitFrameView(new CGRect(this.view_task_right.Frame.Right - (width + 95), view_top_bar.Frame.Bottom, width, 56));

                    this.View.AddSubview(custom_AddFollowView);
                    this.View.BringSubviewToFront(custom_AddFollowView);
                }
                else
                {
                    var width = StringExtensions.MeasureString("Đặt theo dõi công việc này", 14).Width + 20 + 70;
                    custom_AddFollowView.viewController = this;
                    custom_AddFollowView.isFollow = isFollow;
                    custom_AddFollowView.LoadContent();
                    custom_AddFollowView.InitFrameView(new CGRect(this.view_task_right.Frame.Right - (width + 95), view_top_bar.Frame.Bottom, width, 56));

                    this.View.AddSubview(custom_AddFollowView);
                    this.View.BringSubviewToFront(custom_AddFollowView);
                }
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
                attachmentView.SetContent(lst_attachFile, currentSelectedItem.Content, this, attachmentElement);
                PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                attachmentView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                attachmentView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                attachmentView.TransitioningDelegate = transitioningDelegate;
                this.PresentViewControllerAsync(attachmentView, true);
            }
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
                constraint_rightBT_Search.Constant = (view_taskLeft_top.Frame.Width - BT_filter_search.Frame.Width) - 5;
                BT_search.SetImage(UIImage.FromFile("Icons/icon_backSearch.png"), UIControlState.Normal);
                UIView.CommitAnimations();

                BT_search.TintColor = UIColor.Black;
                isSearch = true;
            }
            tf_search.Hidden = !isSearch;
        }

        private void BT_DangXL_TouchUpInside(object sender, EventArgs e)
        {
            tab_inprogress = true;
            currentSelectedItem = null;
            //BT_DangXL.SetTitle(LoadData_count(GetCountNumber()), UIControlState.Normal);
            //ToggleTodo();
            SetLangTitle();

            //if (isFilter)
            //    LoadDataFilterFromServer(false);
            //else
            LoadDataFilterFromMe(true);
        }

        private void BT_DaXL_TouchUpInside(object sender, EventArgs e)
        {
            tab_inprogress = false;
            currentSelectedItem = null;

            //BT_DangXL.SetTitle(LoadData_count(GetCountNumber()), UIControlState.Normal);
            //ToggleTodo();
            SetLangTitle();

            //if (isFilter)
            //    LoadDataFilterFromServer(false);
            //else
            LoadDataFilterFromMe(true);
        }

        private void ToggleTodo()
        {
            if (tab_inprogress) // dang la trang thai todo cua toi
            {
                //tab_inprogress = true;
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
                //tab_inprogress = false;
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

        private void ButtonMenuStyleChange(UIButton _button, bool isSelected, int _index)
        {
            string str_transalte = _button.Title(UIControlState.Normal);
            if (!isSelected)
            {
                if (_index == 0)
                {
                    //str_transalte = LoadData_count(GetCountNumber());
                    //str_transalte = CmmFunction.GetTitle("TEXT_INPROCESS", BT_DangXL.TitleLabel.Text);
                    if (str_transalte.Contains("("))
                    {
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
                        //if (!str_transalte.Contains("("))
                        //    str_transalte = str_transalte + " (" + 50 + ")";
                        //var indexA = str_transalte.IndexOf('(');
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.White, new NSRange(0, str_transalte.Length));
                        _button.SetAttributedTitle(att, UIControlState.Normal);
                    }
                    else
                    {
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
                    //str_transalte = LoadCountDetail(GetCountNumber());
                    //str_transalte = CmmFunction.GetTitle("TEXT_INPROCESS", BT_DangXL.TitleLabel.Text);
                    if (str_transalte.Contains("("))
                    {
                        var indexA = str_transalte.IndexOf('(');
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(_button.Font.PointSize, UIFontWeight.Semibold), new NSRange(0, att.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(0, str_transalte.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(255, 122, 58), new NSRange(indexA, str_transalte.Length - indexA));
                        _button.SetAttributedTitle(att, UIControlState.Normal);
                    }
                    else
                    {
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(_button.Font.PointSize, UIFontWeight.Semibold), new NSRange(0, att.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(0, att.Length));
                        _button.SetAttributedTitle(att, UIControlState.Normal);
                    }
                }
                else if (_index == 1)
                {
                    str_transalte = CmmFunction.GetTitle("TEXT_PROCESSED", BT_DaXL.TitleLabel.Text);
                    if (BT_DaXL.TitleLabel.Text.Contains("("))
                    {
                        var indexA = str_transalte.IndexOf('(');
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(_button.Font.PointSize, UIFontWeight.Semibold), new NSRange(0, att.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(0, att.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(255, 122, 58), new NSRange(indexA, str_transalte.Length - indexA)); // inbox => blue
                        _button.SetAttributedTitle(att, UIControlState.Normal);
                    }
                    else
                    {
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(_button.Font.PointSize, UIFontWeight.Semibold), new NSRange(0, att.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(0, att.Length));
                        _button.SetAttributedTitle(att, UIControlState.Normal);
                    }
                }
            }
        }

        private void Tf_search_EditingChanged(object sender, EventArgs e)
        {
            searchKeyword = tf_search.Text;
            searchData();
        }

        private void BT_filter_search_TouchUpInside(object sender, EventArgs e)
        {
            UINavigationController nav = new UINavigationController();
            nav.PrefersStatusBarHidden();
            nav.NavigationBarHidden = true;
            nav.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;

            FormFillterWorkFlowView formFillterWork = (FormFillterWorkFlowView)Storyboard.InstantiateViewController("FormFillterWorkFlowView");
            formFillterWork.SetContent(this, lst_appStatus, lst_dueDateMenu, isFilter, tab_inprogress);
            nav.AddChildViewController(formFillterWork);

            this.PresentModalViewController(nav, true);
        }

        private void BT_comment_TouchUpInside(object sender, EventArgs e)
        {
            try
            {
                CloseAddFollow();
                table_content_right.ScrollToRow(NSIndexPath.FromRowSection(lst_section[0].ViewRows.Count - 1, 0), UITableViewScrollPosition.Top, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ToDoView - BT_comment_TouchUpInside - Err: " + ex.ToString());
            }

        }

        public void HandleAddFollow()
        {
            Custom_AddFollowView view_follow = Custom_AddFollowView.Instance;
            ButtonAction bt_follow = new ButtonAction();
            bt_follow.Value = "Follow";
            List<KeyValuePair<string, string>> _lstExtent = new List<KeyValuePair<string, string>>();
            _lstExtent.Add(new KeyValuePair<string, string>("status", currentSelectedItem.IsFollow ? "0" : "1"));

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

        private void HandleCloseAddFollow()
        {
            Custom_AddFollowView custom_AddFollowView = Custom_AddFollowView.Instance;
            if (custom_AddFollowView.Superview != null)
                custom_AddFollowView.RemoveFromSuperview();
        }

        public void HandleSectionTable(nint section, string key, int tableIndex)
        {
            var keyInList = "";
            try
            {
                var keyLst = dict_sectionWorkFlow.Keys.ToList().FindAll(o => string.Compare(CmmFunction.GetTitle(o.Split("  ")[0], o.Split("`")[1]), key) == 0).FirstOrDefault();
                if (!string.IsNullOrEmpty(keyLst))
                {
                    keyInList = keyLst;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HandleSectionTable - Err: " + ex.ToString());
            }
            //dict_sectionWorkFlow[key] = !dict_sectionWorkFlow[key];

            dict_sectionWorkFlow[keyInList] = !dict_sectionWorkFlow[keyInList];
            table_workflow.ReloadSections(new NSIndexSet((uint)section), UITableViewRowAnimation.None);
        }

        private async void Workflow_refreshControl_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                /*workflow_refreshControl.BeginRefreshing();
                ProviderBase provider = new ProviderBase();
                await Task.Run(() =>
                {
                    provider.UpdateAllMasterData(true);
                    provider.UpdateAllDynamicData(true);

                    InvokeOnMainThread(() =>
                    {
                        LoadDataFilterFromMe(fromDateSelected, toDateSelected);
                        workflow_refreshControl.EndRefreshing();
                    });
                });*/
                workflow_refreshControl.BeginRefreshing();
                ProviderBase provider = new ProviderBase();
                ProviderUser p_user = new ProviderUser();
                await Task.Run(() =>
                {
                    provider.UpdateAllMasterData(true);
                    provider.UpdateAllDynamicData(true);

                    string documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    string localpath = Path.Combine(documentFolder, CmmVariable.M_Avatar);
                    p_user.UpdateCurrentUserInfo(localpath);

                    InvokeOnMainThread(() =>
                    {
                        if (File.Exists(localpath))
                            BT_avatar.SetImage(UIImage.FromFile(localpath), UIControlState.Normal);

                        //LoadDataFilterFromMe(fromDateSelected, toDateSelected);
                        LoadDataFilterFromMe();
                        workflow_refreshControl.EndRefreshing();
                    });
                });
            }
            catch (Exception ex)
            {
                workflow_refreshControl.EndRefreshing();
                Console.WriteLine("Error - WorkFlowDetailsView - refreshControl_valuechange - Er: " + ex.ToString());
            }
        }

        private void BT_moreUser_TouchUpInside(object sender, EventArgs e)
        {
            if (arr_assignedTo != null && arr_assignedTo.Length > 1)
            {
                string strHeaderUserMore = "";
                List<BeanAppStatus> _lstAppStatus = new List<BeanAppStatus>();
                var conn = new SQLiteConnection(CmmVariable.M_DataPath);

                try
                {
                    string query = string.Format("SELECT * FROM BeanAppStatus WHERE ID = '{0}' LIMIT 1 OFFSET 0", currentSelectedItem.StatusGroup);
                    _lstAppStatus = conn.Query<BeanAppStatus>(query);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Err: " + ex.ToString());
                    _lstAppStatus = new List<BeanAppStatus>();
                }
                finally
                {
                    conn.Close();
                }

                if (_lstAppStatus != null && _lstAppStatus.Count > 0)
                {
                    if (_lstAppStatus[0].ID == 8) // da phe duyet
                        strHeaderUserMore = CmmFunction.GetTitle("TEXT_TITLE_APPROVED", "Phê duyệt: ");
                    else if (_lstAppStatus[0].ID == 64) // da huy
                        strHeaderUserMore = CmmFunction.GetTitle("TEXT_TITLE_CANCEL", "Hủy: ");
                    else if (_lstAppStatus[0].ID == 16)
                        strHeaderUserMore = CmmFunction.GetTitle("TEXT_TITLE_REJECTED", "Từ chối: ");
                    else
                        strHeaderUserMore = CmmFunction.GetTitle("TEXT_TO", "Đến: ");
                }
                else
                    strHeaderUserMore = CmmFunction.GetTitle("TEXT_TO", "Đến: ");

                if (!isExpandUser)
                {
                    lbl_assignedTo.LineBreakMode = UILineBreakMode.WordWrap;
                    isExpandUser = true;
                    string combindedStringUser = string.Join(", ", lst_userName);
                    lbl_assignedTo.Text = strHeaderUserMore + combindedStringUser;//CmmFunction.GetTitle("TEXT_TO", "Đến: ")

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

                    SetTopBarTitle();

                    LoadTinhTrangCategory();
                    LoadDueDateCategory();

                    //lbl_topBar_title.Text = CmmFunction.GetTitle("TEXT_IPAD_TOME", "Trang chủ  >  Đến tôi");
                    tf_search.Placeholder = CmmFunction.GetTitle("TEXT_SEARCH", "Tìm kiếm...");
                    //LoadDataFilterFromMe(status_selected_index, duedate_selected_index, fromDateSelected, toDateSelected);
                    workflow_refreshControl.AttributedTitle = new NSAttributedString(CmmFunction.GetTitle("TEXT_REFRESH_LOADING", "Loading ..."), firstAttributes);

                    LoadDataFilterFromMe();
                    LoadItemSelected(true);

                    SetLangTitle();
                    ToggleTodo();

                    loadQuaTrinhluanchuyen();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("WorkFlowDetailView - CmmEvent_UpdateLangComplete - ERR: " + ex.ToString());
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
                    else if (this.PresentedViewController.GetType() == typeof(FormUsersView)) { }
                    else if (this.PresentedViewController.GetType() == typeof(FormUserAndGroupView)) { }
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

                //if (isShowKeyBoarFromComment)
                //{
                //    if (View.Frame.Y == 0)
                //    {
                //        CGRect keyboardSize = UIKeyboard.BoundsFromNotification(notification);
                //        CGRect custFrame = View.Frame;
                //        custFrame.Y -= keyboardSize.Height;
                //        View.Frame = custFrame;
                //    }
                //}
            }
            catch (Exception ex)
            { Console.WriteLine("StartView - Err: " + ex.ToString()); }
        }
        private void KeyBoardDownNotification(NSNotification notification)
        {
            try
            {
                if (isShowKeyBoarFromComment)
                {
                    if (View.Frame.Y != 0)
                    {
                        CGRect custFrame = View.Frame;
                        custFrame.Y = 0;
                        View.Frame = custFrame;
                    }
                }
            }
            catch (Exception ex)
            { Console.WriteLine("StartView - Err: " + ex.ToString()); }
        }
        #endregion

        #region custom view
        #region workflow data source table
        private class WorkFlow_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellWorkFlow");
            WorkflowDetailView parentView;
            Dictionary<string, List<BeanAppBaseExt>> dict_workflow { get; set; }
            Dictionary<string, bool> dict_section { get; set; }

            public WorkFlow_TableSource(Dictionary<string, List<BeanAppBaseExt>> _dict_workflow, WorkflowDetailView _parentview)
            {
                dict_workflow = _dict_workflow;
                parentView = _parentview;
                GetDictSection();
            }

            private void GetDictSection()
            {
                if (parentView.dict_sectionWorkFlow.Count > 0)
                    parentView.dict_sectionWorkFlow.Clear();

                dict_section = parentView.dict_sectionWorkFlow;
                foreach (var item in dict_workflow)
                {
                    dict_section.Add(item.Key, true);
                }
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return dict_workflow.Count;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                var sectionItem = dict_section.ElementAt(Convert.ToInt32(section));
                var numRow = dict_workflow[sectionItem.Key].Count;

                if (sectionItem.Value)
                    return numRow;
                else
                    return 0;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 90;
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                if (section == 0)
                    return 1;
                else
                {
                    var key = dict_section.ElementAt(Convert.ToInt32(section)).Key;
                    if (dict_workflow[key].Count > 0)
                        return 40;
                    else
                        return 1;
                }
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                var key = dict_section.ElementAt(indexPath.Section).Key;
                var itemSelected = dict_workflow[key][indexPath.Row];

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
                    if (dict_workflow[key].Count > 0)
                    {
                        CGRect frame = new CGRect(0, 0, tableView.Frame.Width, 45);

                        Custom_ToDoHeader headerView = new Custom_ToDoHeader(parentView, frame);

                        if (key.Contains("`"))
                        {
                            var arrKey = key.Split("`");
                            key = CmmFunction.GetTitle(arrKey[0], arrKey[1]);
                        }
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
                var workFlow = dict_workflow[key][indexPath.Row];

                bool isOdd = true;
                if (indexPath.Row % 2 == 0)
                    isOdd = false;

                Custom_WorkFlowCell cell = new Custom_WorkFlowCell(cellIdentifier);
                cell.UpdateCell(workFlow, isOdd);
                return cell;
            }

            public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
            {
                if (parentView.isLoadMore)
                {
                    //var lst_appBase_cxl = parentView.lst_appBase_MyRequest;
                    //var key = dict_section.ElementAt(indexPath.Section).Key;
                    //var todo = dict_workflow[key];

                    //if (dict_section.Count - 1 == indexPath.Section)
                    //{
                    //    if (indexPath.Row + 1 == todo.Count)
                    //    {
                    //        parentView.view_request_left_loadmore.Hidden = false;
                    //        parentView.loadmore_indicator.StartAnimating();
                    //        //parentView.isLoadData = true;
                    //        parentView.LoadmoreData();
                    //    }
                    //}
                    var lst_appBase_fromme = parentView.lst_appBase_MyRequest;
                    int sumItem = 0;
                    for (int i = 0; i < indexPath.Section; i++)
                    {
                        var keySum = dict_section.ElementAt(i).Key;
                        var todoSum = dict_workflow[keySum];
                        sumItem += todoSum.Count;
                    }
                    sumItem += 1;//them mot item
                    sumItem += indexPath.Row;
                    if (sumItem % (parentView.isFilter ? CmmVariable.M_DataFilterAPILimitData : CmmVariable.M_DataLimitRow) == 0 && lst_appBase_fromme.Count == sumItem) // boi so cua 20 va dong thoi la item cuoi cung
                    {
                        parentView.view_request_left_loadmore.Hidden = false;
                        parentView.loadmore_indicator.StartAnimating();
                        //parentView.isLoadMore = true;
                        parentView.LoadmoreData();
                    }

                }
            }
        }
        #endregion

        #region dynamic controls source table
        private class Control_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cell");
            WorkflowDetailView parentView;
            List<ViewSection> lst_section;
            Dictionary<string, List<ViewRow>> dict_control = new Dictionary<string, List<ViewRow>>();
            List<BeanWorkFlowRelated> lstWorkFlowRelated;
            List<BeanTask> lst_tasks;
            List<BeanComment> lst_comment;

            //tam an session
            int heightHeader = 1;

            public Control_TableSource(List<ViewSection> _lst_section, List<BeanWorkFlowRelated> _lstWorkFlowRelated, List<BeanTask> _lst_tasks, List<BeanComment> _lst_comment, WorkflowDetailView _parentview, bool _isSaved)
            {
                if (_isSaved)
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
                RemoveElementHidden();
                parentView = _parentview;
                GetListRowInSection();
            }

            public void RemoveElementHidden()
            {
                var itemViewRowRS = new List<ViewRow>() { };
                var itemElementRS = new List<ViewElement>() { };
                foreach (var itemSection in lst_section)
                {
                    itemViewRowRS = new List<ViewRow>() { };
                    foreach (var itemViewRow in itemSection.ViewRows)
                    {
                        itemElementRS = new List<ViewElement>() { };
                        int i = 0;
                        foreach (var itemElement in itemViewRow.Elements)
                        {

                            if (!itemElement.Hidden)
                            {
                                itemElementRS.Add(itemElement);
                                i++;
                            }

                            //itemViewRow.Elements.Remove(itemElement);
                        }
                        if (itemElementRS != null && itemElementRS.Count > 0)
                        {
                            itemViewRow.Elements = itemElementRS;
                            itemViewRow.RowType = i;
                            itemViewRowRS.Add(itemViewRow);
                        }
                    }
                    itemSection.ViewRows = itemViewRowRS;
                }
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
                nfloat maxHeight = 0;

                var element = dict_control[lst_section[indexPath.Section].ID][indexPath.Row].Elements;
                var control = dict_control[lst_section[indexPath.Section].ID][indexPath.Row];

                nfloat widthTemp = 0;
                if (control.RowType == 1)
                    widthTemp = tableView.Frame.Width;
                else if (control.RowType == 2)
                    widthTemp = tableView.Frame.Width / 2 - 5;
                else if (control.RowType == 3)
                    widthTemp = (tableView.Frame.Width - 20) / 3;
                foreach (var item in element)
                {
                    nfloat heightTemp = 0;
                    switch (item.DataType)
                    {
                        case "tabs":
                            heightTemp = 80;
                            break;
                        case "inputattachmenthorizon":
                            if (!string.IsNullOrEmpty(item.Value))
                            {
                                int sectionHeightTotal = 0;
                                List<BeanAttachFile> lst_attach = new List<BeanAttachFile>();
                                JArray json = JArray.Parse(item.Value);
                                lst_attach = json.ToObject<List<BeanAttachFile>>();
                                parentView.lst_attachFile = lst_attach;

                                if (lst_attach.Count > 0)
                                {
                                    List<string> sectionKeys = lst_attach.Select(x => x.AttachTypeName).Distinct().ToList();
                                    if (sectionKeys != null && sectionKeys.Count > 0)
                                        sectionHeightTotal = sectionKeys.Count * 44;

                                    heightTemp = (lst_attach.Count * 65) + 75 + sectionHeightTotal;//header height: 75 - cell row height: 60 - padding top của table : 10
                                }
                                else
                                    heightTemp = 40;
                            }
                            else
                                heightTemp = 40;
                            break;
                        case "attachmentverticalformframe":
                            {
                                var arrAttachment = item.Value.Split(new string[] { ";#" }, StringSplitOptions.None);
                                int numItem = arrAttachment.Length / 2;

                                heightTemp = (numItem >= 3) ? 265 : (85 + (numItem * 60)); //header view height: 85 | cell height: 60 | max cell: 3 cell
                                break;
                            }
                        case "textinputmultiline":
                            {

                                string value = CmmFunction.StripHTML(item.Value);
                                var height_ets = StringExtensions.StringRect(value, UIFont.SystemFontOfSize(14, UIFontWeight.Regular), (tableView.Frame.Width - 36) / control.RowType);
                                if (height_ets.Height < 52)
                                {
                                    if (height_ets.Height > 30)
                                        heightTemp = 90;
                                    else
                                        heightTemp = 70;
                                }
                                else
                                    heightTemp = 110;
                                break;
                            }
                        case "textinputformat":
                            {
                                var height_ets = StringExtensions.StringRectHTML(item.Value, UIFont.SystemFontOfSize(14, UIFontWeight.Regular), tableView.Frame.Width - 20);
                                if (height_ets.Height < 200)
                                    heightTemp = 100 + 40;
                                else
                                    heightTemp = height_ets.Height + 40;
                                break;
                            }
                        case "inputgriddetails":
                            {
                                nfloat height = 90;
                                var data_source = item.DataSource.Trim();
                                var data_value = item.Value.Trim();

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

                                        foreach (var itemH in lst_titleHeader)
                                        {
                                            //if (item.internalName == "TongTien")
                                            //    item.isSum = true;

                                            if (itemH.isSum)
                                            {
                                                itemH.isSum = true;
                                                height_expand = 50;
                                            }
                                        }
                                        height = height + (lst_jobject.Count * 50);

                                    }
                                    else
                                        height = height + (lst_jobject.Count * 50);
                                    heightTemp = height + height_expand; ;
                                }
                                else
                                    heightTemp = 90;
                                break;
                            }
                        case "inputworkrelated":
                            {
                                var tableHeight = lstWorkFlowRelated.Count * 100;
                                heightTemp = (tableHeight + 30);
                                break;
                            }
                        case "inputtasks":
                            {
                                /*var lst_parent = lst_tasks.Where(i => i.Parent == 0).ToList();
                                foreach (var parent in lst_parent)
                                {
                                    if (parent.IsExpand)
                                    {
                                        //rowNum = 1;
                                        LoadCountSubTask(parent);
                                    }
                                    else
                                        rowNum = 1;
                                }

                                heightTemp = (rowNum * 90) + 80;
                                break;*/
                                var tableHeight = lst_tasks.Count * 90;
                                heightTemp = tableHeight + 90;
                                break;
                            }
                        case "inputcomments":
                            {
                                nfloat basicHeight = 160;
                                nfloat height = 0;
                                //notes => add comment, dinh kem comment 
                                if (item.Notes != null && item.Notes.Count > 0)
                                {
                                    foreach (var note in item.Notes)
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

                                if (!string.IsNullOrEmpty(item.DataSource) || item.DataSource != "[]")
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
                                                    height = height + 300;
                                                }
                                                else
                                                {
                                                    newSortList.Insert(newSortList.Count, attach);
                                                    height = height + 30;
                                                }
                                            }

                                        }
                                        // comment khong co dinh kem
                                        else
                                        {
                                            height = height + 100;
                                        }
                                    }
                                }

                                parentView.estCommmentViewRowHeight = height;

                                heightTemp = height;
                                break;
                            }
                        default:
                            heightTemp = 60;
                            break;
                    }
                    Dictionary<int, nfloat> heightInItem = new Dictionary<int, nfloat>();
                    if (heightTemp > maxHeight)
                        maxHeight = heightTemp;
                }
                return maxHeight;
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
            WorkflowDetailView parentView { get; set; }
            ViewRow control { get; set; }
            NSIndexPath currentIndexPath { get; set; }
            public ComponentBase components;

            public Control_cell_custom(WorkflowDetailView _parentView, NSString cellID, ViewRow _control, NSIndexPath _currentIndexPath) : base(UITableViewCellStyle.Default, cellID)
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
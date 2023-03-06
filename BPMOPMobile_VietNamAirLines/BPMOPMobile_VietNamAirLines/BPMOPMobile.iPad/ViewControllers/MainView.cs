using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.iPad.CustomControlClass;
using BPMOPMobile.iPad.IOSClass;
using BPMOPMobile.iPad.ViewControllers;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using SlideMenuControllerXamarin;
using SQLite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UIKit;

namespace BPMOPMobile.iPad
{
    public partial class MainView : UIViewController
    {
        AppDelegate appD;

        int currentIndexTaskLeft = 0; //0: den toi - 1: tu toi
        int toMe_count, fromMe_count;
        bool IsFirstLoad = true;
        bool tab_dentoi = true;
        bool isLoadMore = true;
        bool isLoadData = false;
        public bool isBackFromFilter = false;
        UIRefreshControl todo_refreshControl, workflow_refreshControl;

        List<BeanAppBaseExt> lst_AppBaseSummarize = new List<BeanAppBaseExt>();

        public List<BeanAppBaseExt> lst_appBase_cxl;
        //List<CountNum> countnum_vcxl = new List<CountNum>();

        List<BeanAppBaseExt> lst_appBase_fromMe;
        //List<CountNum> countnum_workflow_fromMe = new List<CountNum>();

        Dictionary<string, List<BeanAppBaseExt>> dict_todo = new Dictionary<string, List<BeanAppBaseExt>>();
        Dictionary<string, List<BeanAppBaseExt>> dict_myrequest = new Dictionary<string, List<BeanAppBaseExt>>();

        Dictionary<string, bool> dict_sectionTodo;
        Dictionary<string, bool> dict_sectionWorkFlow = new Dictionary<string, bool>();

        ButtonsActionTopBar buttonActionTopBar;
        ButtonsActionBotBar buttonActionBotBar;

        WorkflowDetailView workflowDetailView { get; set; }
        ToDoDetailView toDoDetailView { get; set; }

        int limit = 20;
        int offset = 0;
        bool isOnline = true;

        public MainView(IntPtr handle) : base(handle)
        {
            appD = UIApplication.SharedApplication.Delegate as AppDelegate;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (buttonActionTopBar != null && buttonActionBotBar != null)
            {
                view_top_bar.AddSubviews(buttonActionTopBar);
                view_bot_bar.AddSubviews(buttonActionBotBar);
            }
            if (!isBackFromFilter)
            {
                if (!IsFirstLoad)
                {
                    if (tab_dentoi)
                        LoadDataFilterTodo();
                    else
                        LoadDataFilterFromMe();

                    LoadDataWorkFlowByGroup();
                }
            }
        }

        public override void ViewDidLayoutSubviews()
        {
            if (IsFirstLoad)
            {
                IsFirstLoad = false;
                ViewConfiguration();
                var flowLayout = new UICollectionViewFlowLayout()
                {
                    //ItemSize = new CGSize((float)UIScreen.MainScreen.Bounds.Size.Width / 3.0f, (float)UIScreen.MainScreen.Bounds.Size.Width / 3.0f),
                    ItemSize = new CGSize((view_task_right.Bounds.Width - 60) / 2, (view_task_right.Bounds.Width - 60) / 3f),
                    //HeaderReferenceSize = new CGSize(view_task_right.Frame.Width, 10),
                    SectionInset = new UIEdgeInsets(20, 20, 20, 20),
                    ScrollDirection = UICollectionViewScrollDirection.Vertical,
                    MinimumInteritemSpacing = 2, // minimum spacing between cells
                    MinimumLineSpacing = 20 // minimum spacing between rows if ScrollDirection is Vertical or between columns if Horizontal
                };
                Collection_workflow.SetCollectionViewLayout(flowLayout, true);
                Collection_workflow.RegisterClassForSupplementaryView(typeof(Custom_CollectionHeader), UICollectionElementKindSection.Header, Custom_CollectionHeader.Key);
                Collection_workflow.RegisterClassForCell(typeof(WorkFlowGroup_CollectionCell), WorkFlowGroup_CollectionCell.CellID);
                Collection_workflow.AllowsMultipleSelection = false;
                Collection_workflow.AlwaysBounceVertical = true;
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            CmmIOSFunction.ResignFirstResponderOnTap(this.View);

            //cau hinh background color cho statusbar
            //UIView statusbarView = new UIView();
            //statusbarView.Frame = UIApplication.SharedApplication.StatusBarFrame;
            //statusbarView.BackgroundColor = UIColor.DarkGray;
            //UIApplication.SharedApplication.KeyWindow?.AddSubview(statusbarView);
            //UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.DarkContent;

            workflow_refreshControl = new UIRefreshControl();
            todo_refreshControl = new UIRefreshControl();

            LoadContent();

            ButtonMenuStyleChange(BT_taskLeft_denToi, true, 0);
            ButtonMenuStyleChange(BT_taskLeft_toiBatDau, false, 1);

            #region delegate
            CmmEvent.UpdateLangComplete += CmmEvent_UpdateLangComplete;
            todo_refreshControl.ValueChanged += Todo_refreshControl_ValueChanged;
            workflow_refreshControl.ValueChanged += Workflow_refreshControl_ValueChanged;
            BT_avatar.TouchUpInside += BT_avatar_TouchUpInside;
            BT_taskLeft_denToi.TouchUpInside += BT_taskLeft_denToi_TouchUpInside;
            BT_taskLeft_toiBatDau.TouchUpInside += BT_taskLeft_toiBatDau_TouchUpInside;
            BT_search.TouchUpInside += BT_search_TouchUpInside;
            #endregion
        }

        #region public - private method
        private void ViewConfiguration()
        {
            BT_taskLeft_denToi.Layer.ShadowOffset = new CGSize(1, 2);
            BT_taskLeft_denToi.Layer.ShadowRadius = 4;
            BT_taskLeft_denToi.Layer.ShadowColor = UIColor.Black.CGColor;
            BT_taskLeft_denToi.Layer.ShadowOpacity = 0.5f;

            BT_taskLeft_toiBatDau.Layer.ShadowOffset = new CGSize(-1, 2);
            BT_taskLeft_toiBatDau.Layer.ShadowRadius = 4;
            BT_taskLeft_toiBatDau.Layer.ShadowColor = UIColor.Black.CGColor;
            BT_taskLeft_toiBatDau.Layer.ShadowOpacity = 0.0f;

            buttonActionTopBar = ButtonsActionTopBar.Instance;
            buttonActionTopBar.mainView = this;
            buttonActionTopBar.InitFrameView(view_top_bar.Frame);
            view_top_bar.AddSubviews(buttonActionTopBar);

            buttonActionBotBar = ButtonsActionBotBar.Instance;
            buttonActionBotBar.mainView = this;
            buttonActionBotBar.InitFrameView(view_bot_bar.Frame);
            buttonActionBotBar.LoadStatusButton(0);
            view_BotBar_ConstantHeight.Constant = view_BotBar_ConstantHeight.Constant + 10;
            view_bot_bar.AddSubviews(buttonActionBotBar);

            todo_refreshControl.TintColor = UIColor.FromRGB(65, 80, 134);
            var todoAttributes = new UIStringAttributes
            {
                Font = UIFont.SystemFontOfSize(12)
            };
            todo_refreshControl.AttributedTitle = new NSAttributedString("Loading...", todoAttributes);
            table_toDo.AddSubview(todo_refreshControl);

            workflow_refreshControl.TintColor = UIColor.FromRGB(65, 80, 134);
            var workflowAttributes = new UIStringAttributes
            {
                Font = UIFont.SystemFontOfSize(12)
            };
            workflow_refreshControl.AttributedTitle = new NSAttributedString("Loading...", workflowAttributes);
            table_workflow.AddSubview(workflow_refreshControl);

            CmmIOSFunction.CreateCircleButton(BT_avatar);

            string documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string localpath = Path.Combine(documentFolder, CmmVariable.M_Avatar);

            BT_avatar.ImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
            if (File.Exists(localpath))
                BT_avatar.SetImage(UIImage.FromFile(localpath), UIControlState.Normal);
            else
                BT_avatar.SetImage(UIImage.FromFile("Icons/icon_profile.png"), UIControlState.Normal);

            string str_hintSearch = "Tìm kiếm...";
            var attHintSearch = new NSMutableAttributedString(str_hintSearch);
            attHintSearch.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.White, new NSRange(0, str_hintSearch.Length));

            //CmmIOSFunction.MakeCornerTopLeftRight(view_task_left, 8);
            //CmmIOSFunction.MakeCornerTopLeftRight(view_task_right, 8);
            view_task_left.ClipsToBounds = true;
            view_task_left.Layer.CornerRadius = 6;
            view_task_left.Layer.BorderWidth = 0.1f;
            view_task_left.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;

            view_task_right.ClipsToBounds = true;
            view_task_right.Layer.CornerRadius = 6;
            view_task_right.Layer.BorderWidth = 0.1f;
            view_task_right.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;

            table_toDo.Frame = new CGRect(0, view_taskLeft_top.Frame.Bottom, view_task_left.Frame.Width, view_task_left.Frame.Height - view_taskLeft_top.Frame.Bottom);
            table_workflow.Frame = new CGRect(view_task_left.Frame.Right, view_taskLeft_top.Frame.Bottom, view_task_left.Frame.Width, view_task_left.Frame.Height - view_taskLeft_top.Frame.Bottom);

            //var temp = this.View.Bounds;

            //tam khoa shadow
            //CmmIOSFunction.AddShadowForTopORBotBar(view_top, true);
            //CmmIOSFunction.AddShadowForTopORBotBar(view_bot_bar, false);
        }

        private void LoadContent()
        {
            lbl_search.Text = CmmFunction.GetTitle("TEXT_SEARCH", "Tìm kiếm ...");

            LoadDataWorkFlowByGroup();
            LoadDataFilterTodo();
            LoadDataFilterFromMe();
        }

        public void ReloadContent()
        {
            LoadContent();
        }

        private void LoadDataWorkFlowByGroup()
        {
            try
            {
                var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                lst_AppBaseSummarize = new List<BeanAppBaseExt>();

                string query_worflow = @"SELECT * FROM BeanWorkflow WHERE StatusName = ? AND Favorite = 1";
                List<BeanWorkflow> lst_workflow = conn.Query<BeanWorkflow>(query_worflow, "Active");

                Custom_CollectionWorkFlow custom_CollectionWorkFlow = new Custom_CollectionWorkFlow(this, lst_workflow, false);
                Collection_workflow.Source = custom_CollectionWorkFlow;
                Collection_workflow.ReloadData();

            }
            catch (Exception ex)
            {

            }
        }

        private void HightLightText(UILabel _lable, bool _isSelected)
        {
            if (_lable.Text.Contains("("))
            {
                var str_transalte = _lable.Text;
                var indexA = str_transalte.IndexOf('(');
                NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);

                if (_isSelected)
                {
                    att.AddAttribute(UIStringAttributeKey.Font, UIFont.BoldSystemFontOfSize(14), new NSRange(0, str_transalte.Length));
                    att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(65, 80, 134), new NSRange(0, str_transalte.Length));
                    att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(255, 122, 58), new NSRange(indexA, str_transalte.Length - indexA));
                }
                else
                {
                    att.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(14, UIFontWeight.Regular), new NSRange(0, str_transalte.Length));
                    att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.Black, new NSRange(0, str_transalte.Length));
                }
                _lable.AttributedText = att;
            }
            else
            {
                var str_transalte = _lable.Text;
                NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);

                if (_isSelected)
                {
                    att.AddAttribute(UIStringAttributeKey.Font, UIFont.BoldSystemFontOfSize(14), new NSRange(0, str_transalte.Length));
                    att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(65, 80, 134), new NSRange(0, str_transalte.Length));
                }
                else
                {
                    att.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(14, UIFontWeight.Regular), new NSRange(0, str_transalte.Length));
                    att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.Black, new NSRange(0, str_transalte.Length));
                }

                _lable.AttributedText = att;
            }
        }

        #region toMe
        /// <summary>
        /// Viec Den toi
        /// </summary>
        /// <param name="_statusIndex">0: Tatca | 1: Can XL | 2: Da XL</param>
        /// <param name="_dueDateIndex">0: Tatca | 1: QuaHan | 2: TrongHan </param>
        private void LoadDataFilterTodo()
        {
            isLoadMore = true;
            isOnline = Reachability.detectNetWork();

            dict_todo = new Dictionary<string, List<BeanAppBaseExt>>();
            dict_sectionTodo = new Dictionary<string, bool>();
            lst_appBase_cxl = new List<BeanAppBaseExt>();

            try
            {
                LoadDataToDoOffLine();
                if (isOnline)
                    LoadDataToDoOnLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("MainView - LoadDataFilterTodo - Err: " + ex.ToString());
            }
        }

        void LoadDataToDoOnLine()
        {
            dict_todo = new Dictionary<string, List<BeanAppBaseExt>>();
            dict_sectionTodo = new Dictionary<string, bool>();
            lst_appBase_cxl = new List<BeanAppBaseExt>();

            //GetCountNumber();
            loadData_count(0, true);
            GetListToDoOnline();
        }

        void LoadDataToDoOffLine()
        {
            //string count_query_vcxl = string.Empty;
            var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);

            string defaultStatus_dangxuly = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS_TOME_DANGXULY); // tat ca

            if (!string.IsNullOrEmpty(defaultStatus_dangxuly))
                defaultStatus_dangxuly = string.Format(@" AND AB.StatusGroup IN({0})", defaultStatus_dangxuly);

            //cap nhat count VCXL
            //string query_countvcxl = string.Format("SELECT Count(*) as count " +
            //    "FROM BeanAppBase AB " +
            //    "INNER JOIN BeanNotify NOTI ON AB.ID = NOTI.SPItemId " +
            //    "WHERE NOTI.Type = 1 AND NOTI.Status = 0 AND " + /*local chi hien thi dang xu ly duoi local*/
            //"(AB.AssignedTo LIKE '%{0}%' OR AB.NotifiedUsers LIKE '%{0}%') {1} ", CmmVariable.SysConfig.UserId.ToUpper(), defaultStatus_dangxuly);

            //List<CountNum> countnum_vcxl = conn.Query<CountNum>(query_countvcxl);
            //toMe_count = countnum_vcxl != null ? countnum_vcxl[0].count : 0;
            loadData_count(toMe_count, true);

            //Render luoi danh sach data VCXL theo limit va offset
            string query = string.Format(@"SELECT AB.*, NOTI.StartDate, NOTI.Read, NOTI.SPItemId, NOTI.SubmitAction, NOTI.SubmitActionEN, NOTI.SendUnit " +
                "FROM BeanAppBase AB " +
                "INNER JOIN BeanNotify NOTI ON AB.ID = NOTI.SPItemId " +
                "WHERE NOTI.Type = 1 AND NOTI.Status = 0 AND " + /*local chi hien thi dang xu ly duoi local*/
                "(AB.AssignedTo LIKE '%{0}%' OR AB.NotifiedUsers LIKE '%{0}%') {1} " +
                "Order By NOTI.Created DESC LIMIT ? OFFSET ?", CmmVariable.SysConfig.UserId.ToUpper(), defaultStatus_dangxuly);

            //Console.WriteLine("MainView - LoadDataFilterTodo - Load list to do list - Query: " + query);

            lst_appBase_cxl = conn.Query<BeanAppBaseExt>(query, limit, offset);
            //countnum_vcxl = conn.Query<CountNum>(count_query_vcxl);

            if (lst_appBase_cxl != null && lst_appBase_cxl.Count > 0)
            {
                /*string KEY_TODAY = CmmFunction.GetTitle("TEXT_TODAY", "Hôm nay");
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
                    if (item.Created.HasValue)
                    {
                        if (item.Created.Value.Date == DateTime.Now.Date) // today
                        {
                            List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                            if (dict_todo.ContainsKey(KEY_TODAY))
                                dict_todo[KEY_TODAY].Add(item);
                            else
                                dict_todo.Add(KEY_TODAY, lst_temp);
                        }
                        else if (item.Created.Value.Date == DateTime.Now.AddDays(-1).Date) // hom qua
                        {
                            List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                            if (dict_todo.ContainsKey(KEY_YESTERDAY))
                                dict_todo[KEY_YESTERDAY].Add(item);
                            else
                                dict_todo.Add(KEY_YESTERDAY, lst_temp);
                        }
                        else if (item.Created.Value.Date < DateTime.Now.AddDays(-1).Date) // cuhon
                        {
                            List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                            if (dict_todo.ContainsKey(KEY_ORTHER))
                                dict_todo[KEY_ORTHER].Add(item);
                            else
                                dict_todo.Add(KEY_ORTHER, lst_temp);
                        }
                    }
                }

                table_toDo.Source = new Todo_TableSource(dict_todo, this);
                table_toDo.ReloadData();*/
                SortListAppBaseToDo();
            }
            else
                BT_taskLeft_denToi.SetTitle(CmmFunction.GetTitle("TEXT_IPAD_TOME", "Trang chủ  >  Đến tôi"), UIControlState.Normal);
        }

        void GetListToDoOnline(bool isLoadMore = false)
        {
            List<BeanAppBaseExt> lstObj = new List<BeanAppBaseExt>();
            try
            {
                lstObj = new ProviderBase().LoadMoreDataTFromSerVer(CmmVariable.KEY_GET_TOME_INPROCESS, 20, isLoadMore ? lst_appBase_cxl.Count : 0);

                if (lstObj != null && lstObj.Count > 0)
                {
                    if (isLoadMore)
                    {
                        if (lst_appBase_cxl == null) lst_appBase_cxl = new List<BeanAppBaseExt>();
                        lst_appBase_cxl.AddRange(lstObj);
                    }
                    else
                        lst_appBase_cxl = lstObj;

                    SortListAppBaseToDo();
                    if (!isLoadMore && lst_appBase_cxl != null && lst_appBase_cxl.Count > 0)
                        //table_toDo.ScrollToRow(NSIndexPath.FromItemSection(0, 0), UITableViewScrollPosition.Top, true);
                        table_toDo.ContentOffset = new CGPoint(0, -table_toDo.ContentInset.Top);

                    return;
                }
                else
                {
                    this.isLoadMore = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetListObj - Err: " + ex.ToString());
            }

            if (!isLoadMore)
            {
                lst_appBase_cxl = new List<BeanAppBaseExt>();
                //table_toDo.Alpha = 0;
                //BT_taskLeft_denToi.SetTitle(CmmFunction.GetTitle("TEXT_IPAD_TOME", "Trang chủ  >  Đến tôi"), UIControlState.Normal);
            }
        }

        private void LoadDataFilterTodo_loadmore(int _limit, int _offset)
        {
            try
            {
                isLoadMore = true;
                dict_todo = new Dictionary<string, List<BeanAppBaseExt>>();
                dict_sectionTodo = new Dictionary<string, bool>();

                if (isOnline)
                {
                    GetListToDoOnline(true);
                }
                else
                {
                    List<BeanAppBaseExt> lst_todo_more = new List<BeanAppBaseExt>();
                    var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                    string count_query_vcxl = string.Empty;
                    string query = string.Empty;

                    string defaultStatus_dangxuly = CmmFunction.GetAppSettingValue("MOBILE_APPSTATUS_TOME_DANGXULY");

                    //string defaultStatus_dangxuly = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS); // tat ca fromme
                    //if (ListAppStatus_selected_toMe != null && ListAppStatus_selected_toMe.Count > 0) // co chon status
                    //    str_status = string.Join(',', ListAppStatus_selected_toMe.Select(i => i.ID));

                    //if (!string.IsNullOrEmpty(str_status))
                    //    str_status = string.Format(@" AND AB.StatusGroup IN({0})", str_status);

                    query = string.Format(@"SELECT AB.*, NOTI.StartDate, NOTI.Read, NOTI.SPItemId, NOTI.SubmitAction, NOTI.SubmitActionEN, NOTI.SendUnit " + /*local chi hien thi dang xu ly duoi local*/
                        "FROM BeanAppBase AB " +
                        "INNER JOIN BeanNotify NOTI " +
                        "ON AB.ID = NOTI.SPItemId " +
                        "WHERE " + (tab_dentoi ? " " : "NOTI.Type = 1 AND NOTI.Status = 0 AND ") +
                        "(AB.AssignedTo LIKE '%{0}%' OR AB.NotifiedUsers LIKE '%{0}%') " +
                        "AND AB.StatusGroup IN ({1}) " +
                        "Order By NOTI.Created DESC LIMIT ? OFFSET ?", CmmVariable.SysConfig.UserId.ToUpper(), defaultStatus_dangxuly);

                    lst_todo_more = conn.Query<BeanAppBaseExt>(query, _limit, _offset);

                    if (lst_todo_more != null && lst_todo_more.Count > 0)
                    {
                        lst_appBase_cxl.AddRange(lst_todo_more);
                        /*
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
                            if (item.Created.HasValue)
                            {
                                if (item.Created.Value.Date == DateTime.Now.Date) // today
                                {
                                    List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                                    if (dict_todo.ContainsKey(KEY_TODAY))
                                        dict_todo[KEY_TODAY].Add(item);
                                    else
                                        dict_todo.Add(KEY_TODAY, lst_temp);
                                }
                                else if (item.Created.Value.Date == DateTime.Now.AddDays(-1).Date) // hom qua
                                {
                                    List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                                    if (dict_todo.ContainsKey(KEY_YESTERDAY))
                                        dict_todo[KEY_YESTERDAY].Add(item);
                                    else
                                        dict_todo.Add(KEY_YESTERDAY, lst_temp);
                                }
                                else if (item.Created.Value.Date < DateTime.Now.AddDays(-1).Date) // cuhon
                                {
                                    List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                                    if (dict_todo.ContainsKey(KEY_ORTHER))
                                        dict_todo[KEY_ORTHER].Add(item);
                                    else
                                        dict_todo.Add(KEY_ORTHER, lst_temp);
                                }
                            }
                        }

                        table_toDo.Source = new Todo_TableSource(dict_todo, this);
                        table_toDo.ReloadData();*/
                        SortListAppBaseToDo();
                    }
                    else
                    {
                        isLoadMore = false;
                        BT_taskLeft_denToi.SetTitle(CmmFunction.GetTitle("TEXT_IPAD_TOME", "Trang chủ  >  Đến tôi"), UIControlState.Normal);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MainView - LoadDataFilterTodo - Err: " + ex.ToString());
            }
        }

        #endregion

        #region from me
        private void LoadDataFilterFromMe()
        {
            isLoadMore = true;
            isOnline = Reachability.detectNetWork();
            dict_myrequest = new Dictionary<string, List<BeanAppBaseExt>>();
            dict_sectionWorkFlow = new Dictionary<string, bool>();
            lst_appBase_fromMe = new List<BeanAppBaseExt>();

            try
            {
                LoadDataFromMeOffLine();
                if (isOnline)
                    LoadDataFromMeOnLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("MainView - LoadDataFilterFromMe - Err: " + ex.ToString());
            }
        }

        void LoadDataFromMeOffLine()
        {
            var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
            string count_query_fromMe = string.Empty;
            string query = string.Empty;

            //Check Filter Status
            string str_status = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS); // tat ca fromme
            if (!string.IsNullOrEmpty(str_status))
                str_status = string.Format(@" AND AB.StatusGroup IN({0})", str_status);

            //string query_today = string.Format(@"SELECT Count(*) as count FROM BeanAppBase AB "
            //    + "LEFT JOIN BeanAppStatus AST ON AST.ID = AB.StatusGroup "
            //    + "WHERE AB.CreatedBy LIKE '%{0}%' {1}", CmmVariable.SysConfig.UserId.ToUpper(), str_status);

            //List<CountNum> countnum_vcxl = conn.Query<CountNum>(query_today);
            //fromMe_count = countnum_vcxl != null ? countnum_vcxl[0].count : 0;
            loadData_count(fromMe_count, false);

            query = string.Format(@"SELECT AB.* FROM BeanAppBase AB "
                + "LEFT JOIN BeanAppStatus AST ON AST.ID = AB.StatusGroup "
                + "WHERE AB.CreatedBy LIKE '%{0}%' {1} "
                + "ORDER BY AB.Created DESC LIMIT ? OFFSET ?", CmmVariable.SysConfig.UserId.ToUpper(), str_status);

            lst_appBase_fromMe = conn.Query<BeanAppBaseExt>(query, limit, offset);
            if (lst_appBase_fromMe != null)
            {
                SortListAppBaseFromMe();
            }
        }

        void LoadDataFromMeOnLine()
        {
            //GetCountNumber();
            loadData_count(0, false);
            GetListFromMeOnline();
        }

        void GetListFromMeOnline(bool isLoadMore = false)
        {
            List<BeanAppBaseExt> lstObj = new List<BeanAppBaseExt>();
            try
            {
                lstObj = new ProviderBase().LoadMoreDataTFromSerVer(CmmVariable.KEY_GET_FROMME_INPROCESS, 20, isLoadMore ? lst_appBase_fromMe.Count : 0);

                if (lstObj != null && lstObj.Count > 0)
                {
                    if (isLoadMore)
                    {
                        if (lst_appBase_fromMe == null) lst_appBase_fromMe = new List<BeanAppBaseExt>();
                        lst_appBase_fromMe.AddRange(lstObj);
                    }
                    else
                        lst_appBase_fromMe = lstObj;

                    SortListAppBaseFromMe();

                    if (!isLoadMore && lst_appBase_fromMe != null && lst_appBase_fromMe.Count > 0)
                        //table_workflow.ScrollToRow(NSIndexPath.FromItemSection(0, 0), UITableViewScrollPosition.Top, true);
                        table_workflow.ContentOffset = new CGPoint(0, -table_workflow.ContentInset.Top);

                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetListObj - Err: " + ex.ToString());
            }

            if (!isLoadMore)
            {
                lst_appBase_fromMe = new List<BeanAppBaseExt>();
                //table_workflow.Alpha = 0;
            }
        }

        void SortListAppBaseToDo()
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
                if (item.Created.HasValue)
                {
                    if (item.Created.Value.Date == DateTime.Now.Date) // today
                    {
                        List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                        if (dict_todo.ContainsKey(KEY_TODAY))
                            dict_todo[KEY_TODAY].Add(item);
                        else
                            dict_todo.Add(KEY_TODAY, lst_temp);
                    }
                    else if (item.Created.Value.Date == DateTime.Now.AddDays(-1).Date) // hom qua
                    {
                        List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                        if (dict_todo.ContainsKey(KEY_YESTERDAY))
                            dict_todo[KEY_YESTERDAY].Add(item);
                        else
                            dict_todo.Add(KEY_YESTERDAY, lst_temp);
                    }
                    else if (item.Created.Value.Date < DateTime.Now.AddDays(-1).Date) // cuhon
                    {
                        List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                        if (dict_todo.ContainsKey(KEY_ORTHER))
                            dict_todo[KEY_ORTHER].Add(item);
                        else
                            dict_todo.Add(KEY_ORTHER, lst_temp);
                    }
                }
            }

            table_toDo.Source = new Todo_TableSource(dict_todo, this);
            table_toDo.ReloadData();
        }

        void SortListAppBaseFromMe()
        {
            dict_myrequest = new Dictionary<string, List<BeanAppBaseExt>>();
            dict_sectionWorkFlow = new Dictionary<string, bool>();

            string KEY_TODAY = CmmFunction.GetTitle("TEXT_TODAY", "Hôm nay");
            string KEY_YESTERDAY = CmmFunction.GetTitle("TEXT_YESTERDAY", "Hôm qua");
            string KEY_ORTHER = CmmFunction.GetTitle("TEXT_OLDER", "Cũ hơn");

            List<BeanAppBaseExt> lst_temp1 = new List<BeanAppBaseExt>();
            List<BeanAppBaseExt> lst_temp2 = new List<BeanAppBaseExt>();
            List<BeanAppBaseExt> lst_temp3 = new List<BeanAppBaseExt>();
            dict_myrequest.Add(KEY_TODAY, lst_temp1);
            dict_myrequest.Add(KEY_YESTERDAY, lst_temp2);
            dict_myrequest.Add(KEY_ORTHER, lst_temp3);

            foreach (var item in lst_appBase_fromMe)
            {
                if (item.Created.Value.Date == DateTime.Now.Date) // today
                {
                    List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                    if (dict_myrequest.ContainsKey(KEY_TODAY))
                        dict_myrequest[KEY_TODAY].Add(item);
                    else
                        dict_myrequest.Add(KEY_TODAY, lst_temp);
                }
                else if (item.Created.Value.Date == DateTime.Now.AddDays(-1).Date) // hom qua
                {

                    List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                    if (dict_myrequest.ContainsKey(KEY_YESTERDAY))
                        dict_myrequest[KEY_YESTERDAY].Add(item);
                    else
                        dict_myrequest.Add(KEY_YESTERDAY, lst_temp);
                }
                else if (item.Created.Value.Date < DateTime.Now.AddDays(-1).Date) // cuhon
                {
                    List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                    if (dict_myrequest.ContainsKey(KEY_ORTHER))
                        dict_myrequest[KEY_ORTHER].Add(item);
                    else
                        dict_myrequest.Add(KEY_ORTHER, lst_temp);
                }
            }

            table_workflow.Source = new WorkFlow_TableSource(dict_myrequest, this);
            table_workflow.ReloadData();
        }

        private void LoadDataFilterFromMe_loadmore(int _limit, int _offset)
        {
            try
            {
                dict_myrequest = new Dictionary<string, List<BeanAppBaseExt>>();
                dict_sectionWorkFlow = new Dictionary<string, bool>();

                if (isOnline)
                {
                    GetListFromMeOnline(true);
                }
                else
                {
                    List<BeanAppBaseExt> lst_appBase_fromMe_more = new List<BeanAppBaseExt>();
                    var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                    string count_query_fromMe = string.Empty;
                    string query = string.Empty;

                    //Check Filter Status
                    string str_status = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS); // tat ca fromme
                    if (!string.IsNullOrEmpty(str_status))
                        str_status = string.Format(@" AND AB.StatusGroup IN({0})", str_status);

                    query = string.Format(@"SELECT AB.* FROM BeanAppBase AB "
                        + "LEFT JOIN BeanAppStatus AST ON AST.ID = AB.StatusGroup "
                        + "WHERE AB.CreatedBy LIKE '%{0}%' {1} "
                        + "ORDER BY AB.Created DESC LIMIT ? OFFSET ?", CmmVariable.SysConfig.UserId.ToUpper(), str_status);

                    lst_appBase_fromMe_more = conn.Query<BeanAppBaseExt>(query, _limit, _offset);
                    if (lst_appBase_fromMe_more != null && lst_appBase_fromMe_more.Count > 0)
                    {
                        lst_appBase_fromMe.AddRange(lst_appBase_fromMe_more);
                        /*
                        string KEY_TODAY = CmmFunction.GetTitle("TEXT_TODAY", "Hôm nay");
                        string KEY_YESTERDAY = CmmFunction.GetTitle("TEXT_YESTERDAY", "Hôm qua");
                        string KEY_ORTHER = CmmFunction.GetTitle("TEXT_OLDER", "Cũ hơn");

                        List<BeanAppBaseExt> lst_temp1 = new List<BeanAppBaseExt>();
                        List<BeanAppBaseExt> lst_temp2 = new List<BeanAppBaseExt>();
                        List<BeanAppBaseExt> lst_temp3 = new List<BeanAppBaseExt>();
                        dict_myrequest.Add(KEY_TODAY, lst_temp1);
                        dict_myrequest.Add(KEY_YESTERDAY, lst_temp2);
                        dict_myrequest.Add(KEY_ORTHER, lst_temp3);

                        foreach (var item in lst_appBase_fromMe)
                        {
                            if (item.Created.Value.Date == DateTime.Now.Date) // today
                            {
                                List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                                if (dict_myrequest.ContainsKey(KEY_TODAY))
                                    dict_myrequest[KEY_TODAY].Add(item);
                                else
                                    dict_myrequest.Add(KEY_TODAY, lst_temp);
                            }
                            else if (item.Created.Value.Date == DateTime.Now.AddDays(-1).Date) // hom qua
                            {

                                List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                                if (dict_myrequest.ContainsKey(KEY_YESTERDAY))
                                    dict_myrequest[KEY_YESTERDAY].Add(item);
                                else
                                    dict_myrequest.Add(KEY_YESTERDAY, lst_temp);
                            }
                            else if (item.Created.Value.Date < DateTime.Now.AddDays(-1).Date) // cuhon
                            {
                                List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                                if (dict_myrequest.ContainsKey(KEY_ORTHER))
                                    dict_myrequest[KEY_ORTHER].Add(item);
                                else
                                    dict_myrequest.Add(KEY_ORTHER, lst_temp);
                            }
                        }

                        table_workflow.Source = new WorkFlow_TableSource(dict_myrequest, this);
                        table_workflow.ReloadData();*/
                        SortListAppBaseFromMe();
                    }
                    else
                    {
                        isLoadMore = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MainView - LoadDataFilterFromMe - Err: " + ex.ToString());
            }
        }
        #endregion

        private void ToggleTodo()
        {
            if (tab_dentoi) // dang la trang thai todo cua toi
            {
                tab_dentoi = true;
                BT_taskLeft_denToi.BackgroundColor = UIColor.White;
                BT_taskLeft_denToi.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);
                BT_taskLeft_toiBatDau.BackgroundColor = UIColor.Clear;
                BT_taskLeft_toiBatDau.SetTitleColor(UIColor.White, UIControlState.Normal);

                BT_taskLeft_denToi.Layer.ShadowOpacity = 0.5f;
                BT_taskLeft_toiBatDau.Layer.ShadowOpacity = 0.0f;

                ButtonMenuStyleChange(BT_taskLeft_denToi, true, 0);
                ButtonMenuStyleChange(BT_taskLeft_toiBatDau, false, 1);
            }
            else
            {
                BT_taskLeft_toiBatDau.Layer.ShadowOffset = new CGSize(-1, 2);
                tab_dentoi = false;
                BT_taskLeft_denToi.BackgroundColor = UIColor.Clear;
                BT_taskLeft_denToi.SetTitleColor(UIColor.White, UIControlState.Normal);
                BT_taskLeft_toiBatDau.BackgroundColor = UIColor.White;
                BT_taskLeft_toiBatDau.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);

                BT_taskLeft_denToi.Layer.ShadowOpacity = 0.0f;
                BT_taskLeft_toiBatDau.Layer.ShadowOpacity = 0.5f;

                ButtonMenuStyleChange(BT_taskLeft_denToi, false, 0);
                ButtonMenuStyleChange(BT_taskLeft_toiBatDau, true, 1);
            }
        }

        private void loadData_count(int _dataCount, bool _isToMe)
        {
            try
            {
                GetCountNumber();
                if (_isToMe)
                {
                    string str_toMe = string.Empty;
                    //toMe_count = _dataCount;
                    str_toMe = CmmFunction.GetTitle("TEXT_TOME", "Đến tôi");
                    BT_taskLeft_denToi.SetAttributedTitle(null, UIControlState.Normal);
                    if (toMe_count >= 100)
                    {
                        BT_taskLeft_denToi.SetTitle(str_toMe + " (99+)", UIControlState.Normal);
                        //BT_taskLeft_denToi.TitleLabel.Text = str_toMe + " (99+)";
                    }
                    else if (toMe_count > 0 && toMe_count < 100)
                    {
                        str_toMe = str_toMe + " (" + (toMe_count < 10 ? "0" : "") + toMe_count.ToString() + ")";
                        BT_taskLeft_denToi.SetTitle(str_toMe, UIControlState.Normal);
                        //BT_taskLeft_denToi.TitleLabel.Text = str_toMe;
                    }
                    else
                    {
                        BT_taskLeft_denToi.SetTitle(str_toMe, UIControlState.Normal);
                        //BT_taskLeft_denToi.TitleLabel.Text = str_toMe;
                    }
                }
                // From me
                else
                {
                    BT_taskLeft_toiBatDau.SetAttributedTitle(null, UIControlState.Normal);
                    //fromMe_count = _dataCount;
                    string str_fromMe = CmmFunction.GetTitle("TEXT_FROMME", "Tôi bắt đầu");
                    if (fromMe_count >= 100)
                    {
                        BT_taskLeft_toiBatDau.SetTitle(str_fromMe + " (99+)", UIControlState.Normal);
                        //BT_taskLeft_toiBatDau.TitleLabel.Text = str_fromMe + " (99+)";
                    }
                    else if (fromMe_count > 0 && fromMe_count <= 100)
                    {
                        str_fromMe = str_fromMe + " (" + (fromMe_count < 10 ? "0" : "") + fromMe_count.ToString() + ")";
                        BT_taskLeft_toiBatDau.SetTitle(str_fromMe, UIControlState.Normal);
                        //BT_taskLeft_toiBatDau.TitleLabel.Text = str_fromMe;
                    }
                    else
                    {
                        BT_taskLeft_toiBatDau.SetTitle(str_fromMe, UIControlState.Normal);
                        //BT_taskLeft_toiBatDau.TitleLabel.Text = str_fromMe;
                    }
                }

                ToggleTodo();
            }
            catch (Exception ex)
            {
                Console.WriteLine("MainView - loadData - Err: " + ex.ToString());
            }
        }

        void GetCountNumber()
        {
            try
            {
                string count = "";

                count = new ProviderControlDynamic().GetListCountVDT_VTBD(CmmVariable.KEY_COUNT_FROMME_INPROCESS + "|" + CmmVariable.KEY_COUNT_ASIGNTOME_INPROCESS);

                if (!string.IsNullOrEmpty(count))
                {
                    var allCount = count.Split('|');
                    if (allCount.Length > 0)
                    {
                        foreach (var number in allCount)
                        {
                            var str = number.Split(";#");
                            if (string.Compare(str[0], CmmVariable.KEY_COUNT_ASIGNTOME_INPROCESS) == 0)
                            {
                                if (!int.TryParse(str[1], out toMe_count))
                                {
                                    toMe_count = 0;
                                }
                            }
                            else if (string.Compare(str[0], CmmVariable.KEY_COUNT_FROMME_INPROCESS) == 0)
                            {
                                if (!int.TryParse(str[1], out fromMe_count))
                                {
                                    fromMe_count = 0;
                                }
                            }

                        }
                    }
                }
                else
                {
                    toMe_count = 0;
                    fromMe_count = 0;
#if DEBUG
                    Console.WriteLine("GetCountNumber trả về chuỗi trống.");
#endif
                }
            }
            catch (Exception ex)
            {
                toMe_count = 0;
                fromMe_count = 0;
#if DEBUG
                Console.WriteLine("GetCountNumber - Err: " + ex.ToString());
#endif
            }
        }

        public async void loadmoreData()
        {
            view_task_left_loadmore.Hidden = false;
            loadmore_indicator.StartAnimating();

            //string query = condition;
            //List<BeanJob> more_jobs = new List<BeanJob>();
            //ProviderBase p_base = new ProviderBase();
            //string dateStart = ProviderDoc.getModifyStart<BeanJob>();

            //DateTime date = new DateTime();
            //DateTime.TryParse(dateStart, out date);

            await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.5f));
                InvokeOnMainThread(() =>
                {
                    if (tab_dentoi)
                        LoadDataFilterTodo_loadmore(limit, lst_appBase_cxl.Count);
                    else
                        LoadDataFilterFromMe_loadmore(limit, lst_appBase_fromMe.Count);

                    //parentView.table_content.ReloadData();
                    loadmore_indicator.StopAnimating();
                    view_task_left_loadmore.Hidden = true;
                });

                //more_jobs = p_base.LoadMoreData<BeanJob>(query, limit, jobs.Count, null);
                //if (more_jobs != null && more_jobs.Count > 0) // có data trong local DB
                //{
                //    if (more_jobs.Count == limit) // vẫn còn data trong DB
                //    {
                //        jobs.AddRange(more_jobs);
                //        Thread.Sleep(TimeSpan.FromSeconds(1));
                //        InvokeOnMainThread(() =>
                //        {
                //            parentView.table_content.ReloadData();
                //            parentView.view_loadmore.Hidden = true;
                //            parentView.indicator_more.StopAnimating();
                //        });
                //    }
                //    else if (more_jobs.Count < limit) // không còn thêm data trong DB
                //    {
                //        jobs.AddRange(more_jobs);
                //        isEndofData = true;

                //        InvokeOnMainThread(() =>
                //        {
                //            parentView.table_content.ReloadData();
                //            parentView.indicator_more.StopAnimating();
                //            parentView.view_loadmore.Hidden = true;
                //        });
                //    }
                //}
                //else // không có data trong DB
                //{
                //isEndofData = true;

                //InvokeOnMainThread(() =>
                //{
                //    parentView.indicator_more.StopAnimating();
                //    parentView.view_loadmore.Hidden = true;
                //});
                //}
            });
        }

        private void ButtonMenuStyleChange(UIButton _button, bool isSelected, int _index)
        {
            string str_transalte = "";
            if (!isSelected)
            {
                if (_index == 0)
                {
                    if (BT_taskLeft_denToi.TitleLabel.Text.Contains("("))
                    {
                        //str_transalte = CmmFunction.GetTitle("TEXT_TOME", "Đến tôi");
                        str_transalte = BT_taskLeft_denToi.TitleLabel.Text;
                        //if (!str_transalte.Contains("("))
                        //    str_transalte = str_transalte + " (" + toMe_count + ")";
                        //var indexA = str_transalte.IndexOf('(');
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.White, new NSRange(0, str_transalte.Length));
                        _button.SetAttributedTitle(att, UIControlState.Normal);
                    }
                    else
                    {
                        str_transalte = BT_taskLeft_denToi.TitleLabel.Text;//CmmFunction.GetTitle("TEXT_TOME", "Đến tôi");
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.White, new NSRange(0, str_transalte.Length));
                        _button.SetAttributedTitle(att, UIControlState.Normal);
                    }
                }
                else if (_index == 1)
                {
                    if (BT_taskLeft_toiBatDau.TitleLabel.Text.Contains("("))
                    {
                        //str_transalte = CmmFunction.GetTitle("TEXT_FROMME", "Tôi bắt đầu");
                        str_transalte = BT_taskLeft_toiBatDau.TitleLabel.Text;
                        /*if (!str_transalte.Contains("("))
                            str_transalte = str_transalte + " (" + fromMe_count + ")";
                        var indexA = str_transalte.IndexOf('(');*/
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.White, new NSRange(0, str_transalte.Length));
                        _button.SetAttributedTitle(att, UIControlState.Normal);
                    }
                    else
                    {
                        //str_transalte = CmmFunction.GetTitle("TEXT_FROMME", "Tôi bắt đầu");
                        str_transalte = BT_taskLeft_toiBatDau.TitleLabel.Text;
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
                    if (BT_taskLeft_denToi.TitleLabel.Text.Contains("("))
                    {
                        str_transalte = BT_taskLeft_denToi.TitleLabel.Text;
                        //str_transalte = CmmFunction.GetTitle("TEXT_TOME", "Đến tôi");
                        //if (!str_transalte.Contains("("))
                        //    str_transalte = str_transalte + " (" + toMe_count + ")";
                        var indexA = str_transalte.IndexOf('(');
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(_button.Font.PointSize, UIFontWeight.Semibold), new NSRange(0, att.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(0, str_transalte.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(255, 122, 58), new NSRange(indexA, str_transalte.Length - indexA));
                        _button.SetAttributedTitle(att, UIControlState.Normal);
                    }
                    else
                    {
                        str_transalte = BT_taskLeft_denToi.TitleLabel.Text;
                        //str_transalte = CmmFunction.GetTitle("TEXT_TOME", "Đến tôi");
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(_button.Font.PointSize, UIFontWeight.Semibold), new NSRange(0, att.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(0, att.Length));
                        _button.SetAttributedTitle(att, UIControlState.Normal);
                    }
                }
                else if (_index == 1)
                {
                    if (BT_taskLeft_toiBatDau.TitleLabel.Text.Contains("("))
                    {
                        /*str_transalte = CmmFunction.GetTitle("TEXT_FROMME", "Tôi bắt đầu");
                        if (!str_transalte.Contains("("))
                            str_transalte = str_transalte + " (" + fromMe_count + ")";*/
                        str_transalte = BT_taskLeft_toiBatDau.TitleLabel.Text;
                        var indexA = str_transalte.IndexOf('(');
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(_button.Font.PointSize, UIFontWeight.Semibold), new NSRange(0, att.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(0, att.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(255, 122, 58), new NSRange(indexA, str_transalte.Length - indexA)); // inbox => blue
                        _button.SetAttributedTitle(att, UIControlState.Normal);
                    }
                    else
                    {
                        str_transalte = BT_taskLeft_toiBatDau.TitleLabel.Text;
                        //str_transalte = CmmFunction.GetTitle("TEXT_FROMME", "Tôi bắt đầu");
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(_button.Font.PointSize, UIFontWeight.Semibold), new NSRange(0, att.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(0, att.Length));
                        _button.SetAttributedTitle(att, UIControlState.Normal);
                    }
                }
            }
        }
        #endregion

        #region event
        private void CmmEvent_UpdateLangComplete(object sender, CmmEvent.UpdateEventArgs e)
        {
            try
            {
                InvokeOnMainThread(() =>
                {
                    if (buttonActionBotBar != null)
                        buttonActionBotBar.UpdateLangTitle();

                    lbl_search.Text = CmmFunction.GetTitle("TEXT_SEARCH", "Tìm kiếm ...");
                    LoadDataFilterTodo();
                    LoadDataFilterFromMe();
                    LoadDataWorkFlowByGroup();

                    ToggleTodo();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("MenuView - CmmEvent_UpdateLangComplete - ERR: " + ex.ToString());
            }
        }

        private void BT_avatar_TouchUpInside(object sender, EventArgs e)
        {
            appD.menu.UpdateItemSelect(0);
            appD.SlideMenuController.OpenLeft();
        }

        private void BT_taskLeft_denToi_TouchUpInside(object sender, EventArgs e)
        {
            if (!tab_dentoi)
            {
                tab_dentoi = true;
                ToggleTodo();

                table_workflow.Frame = new CGRect(0, view_taskLeft_top.Frame.Bottom, view_task_left.Frame.Width, view_task_left.Frame.Height - view_taskLeft_top.Frame.Bottom);
                UIView.BeginAnimations("show_animationShowTable");
                UIView.SetAnimationDuration(0.2f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);
                table_workflow.Frame = new CGRect(view_task_left.Frame.Right, view_taskLeft_top.Frame.Bottom, view_task_left.Frame.Width, view_task_left.Frame.Height - view_taskLeft_top.Frame.Bottom);
                UIView.CommitAnimations();
            }
        }

        private void BT_taskLeft_toiBatDau_TouchUpInside(object sender, EventArgs e)
        {
            if (tab_dentoi)
            {
                tab_dentoi = false;
                ToggleTodo();

                table_workflow.Frame = new CGRect(view_task_left.Frame.Right, view_taskLeft_top.Frame.Bottom, view_task_left.Frame.Width, view_task_left.Frame.Height - view_taskLeft_top.Frame.Bottom);
                UIView.BeginAnimations("show_animationShowTable");
                UIView.SetAnimationDuration(0.2f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);
                table_workflow.Frame = new CGRect(0, view_taskLeft_top.Frame.Bottom, view_task_left.Frame.Width, view_task_left.Frame.Height - view_taskLeft_top.Frame.Bottom);
                UIView.CommitAnimations();
            }
        }

        public void SignOut()
        {
            try
            {
                NSUserDefaults.StandardUserDefaults.SetString("", "ActiveUser");
                var dataFile = CmmVariable.M_DataPath;
                if (File.Exists(dataFile))
                    File.Delete(dataFile);

                string dataFile_shm = CmmVariable.M_DataPath + "-shm";
                if (File.Exists(dataFile_shm))
                    File.Delete(dataFile_shm);

                string dataFile_wal = CmmVariable.M_DataPath + "-wal";
                if (File.Exists(dataFile_wal))
                    File.Delete(dataFile_wal);

                var configFile = CmmVariable.M_settingFileName;
                if (File.Exists(configFile))
                    File.Delete(configFile);

                CmmVariable.M_DataPath = "DB_sqlite_XamDocument.db3";
                CmmVariable.M_settingFileName = "config.ini";
                CmmVariable.M_AuthenticatedHttpClient = null;
                CmmVariable.SysConfig = null;
                CmmFunction.WriteSetting();

                UIApplication.SharedApplication.CancelAllLocalNotifications();

                var navigationController = Storyboard.InstantiateViewController("RootNavigationController") as RootNavigationController;
                appD.Window.RootViewController = navigationController;
                this.DismissViewController(true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("MainView - SignOut - Err: " + ex.ToString());
            }
        }

        public void NavigateToDetailsFromSearch(bool isTome, int trangThaiID, bool isFilter, List<BeanAppStatus> lst_appStatus, List<ClassMenu> lst_dueDateMenu, DateTime _fromDateSelected, DateTime _toDateSelected, string keyword)
        {
            if (isTome)
            {
                toDoDetailView = (ToDoDetailView)this.Storyboard.InstantiateViewController("ToDoDetailView");

                toDoDetailView.isFilter = isFilter;
                toDoDetailView.date_filter = CmmIOSFunction.GetStringDateFilter(_fromDateSelected, _toDateSelected);
                if (trangThaiID == 1)
                    toDoDetailView.tab_Dangxuly = true;
                else if (trangThaiID == 2)
                    toDoDetailView.tab_Dangxuly = false;

                toDoDetailView.fromDateSelected = _fromDateSelected;
                toDoDetailView.toDateSelected = _toDateSelected;
                toDoDetailView.lst_appStatus = lst_appStatus;
                toDoDetailView.lst_dueDateMenu = lst_dueDateMenu;
                toDoDetailView.searchKeyword = keyword;

                if (!string.IsNullOrEmpty(keyword))
                    toDoDetailView.isSearch = true;
                else
                    toDoDetailView.isSearch = false;

                //cap nhat bottom navagation button 
                if (appD.SlideMenuController.MainViewController.GetType() != typeof(ToDoDetailView))
                {
                    appD.SlideMenuController.ChangeMainViewcontroller(toDoDetailView, true);
                    ButtonsActionBotBar buttonActionBotBar = ButtonsActionBotBar.Instance;
                    buttonActionBotBar.LoadStatusButton(1);
                }
            }
            else
            {
                workflowDetailView = (WorkflowDetailView)this.Storyboard.InstantiateViewController("WorkflowDetailView");
                workflowDetailView.isFilter = isFilter;
                workflowDetailView.fromDateSelected = _fromDateSelected;
                workflowDetailView.toDateSelected = _toDateSelected;
                workflowDetailView.lst_appStatus = lst_appStatus;
                workflowDetailView.lst_dueDateMenu = lst_dueDateMenu;
                workflowDetailView.searchKeyword = keyword;

                if (!string.IsNullOrEmpty(keyword))
                    workflowDetailView.isSearch = true;
                else
                    workflowDetailView.isSearch = false;

                //cap nhat bottom navagation button 
                if (appD.SlideMenuController.MainViewController.GetType() != typeof(ToDoDetailView))
                {
                    appD.SlideMenuController.ChangeMainViewcontroller(workflowDetailView, true);
                    ButtonsActionBotBar buttonActionBotBar = ButtonsActionBotBar.Instance;
                    buttonActionBotBar.LoadStatusButton(2);
                }
            }
        }

        public void NavigateToDetails(BeanAppBaseExt _appBase, bool isPopup, bool isTodo)
        {
            if (isPopup)
            {
                string workflowID = "";
                if (!string.IsNullOrEmpty(_appBase.ItemUrl))
                    workflowID = CmmFunction.GetWorkflowItemIDByUrl(_appBase.ItemUrl);

                SQLiteAsyncConnection conn = new SQLiteAsyncConnection(CmmVariable.M_DataPath, false);
                string query_workflowItem = string.Format("SELECT * FROM BeanWorkflowItem WHERE ID = {0}", workflowID);
                var lst_result = conn.QueryAsync<BeanWorkflowItem>(query_workflowItem).Result;

                if (lst_result != null && lst_result.Count > 0)
                {
                    BeanWorkflowItem beanWorkflowItem = lst_result[0];
                    CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
                    CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
                    FormWorkFlowDetails detail = (FormWorkFlowDetails)Storyboard.InstantiateViewController("FormWorkFlowDetails");

                    detail.SetContent(beanWorkflowItem, false, this);
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
            else
            {
                if (isTodo)
                {
                    toDoDetailView = (ToDoDetailView)this.Storyboard.InstantiateViewController("ToDoDetailView");
                    toDoDetailView.SetContent(_appBase, this);

                    //cap nhat bottom navagation button
                    if (appD.SlideMenuController.MainViewController.GetType() != typeof(ToDoDetailView))
                    {
                        appD.SlideMenuController.ChangeMainViewcontroller(toDoDetailView, true);
                        ButtonsActionBotBar buttonActionBotBar = ButtonsActionBotBar.Instance;
                        buttonActionBotBar.LoadStatusButton(1);
                    }
                }
                else
                {
                    workflowDetailView = (WorkflowDetailView)this.Storyboard.InstantiateViewController("WorkflowDetailView");
                    workflowDetailView.SetContent(_appBase, this);

                    if (appD.SlideMenuController.MainViewController.GetType() != typeof(WorkflowDetailView))
                    {
                        appD.SlideMenuController.ChangeMainViewcontroller(workflowDetailView, true);
                        ButtonsActionBotBar buttonActionBotBar = ButtonsActionBotBar.Instance;
                        buttonActionBotBar.LoadStatusButton(2);
                    }
                }
            }
        }

        public void NavigateToViewByCate(BeanWorkflow workflowSelected)
        {
            KanBanView kanBanView = (KanBanView)Storyboard.InstantiateViewController("KanBanView");
            kanBanView.SetContent(workflowSelected, 0);
            this.NavigationController.PushViewController(kanBanView, true);
        }

        public void HandleSectionTable(nint section, string key, int tableIndex)
        {
            if (tableIndex == 0)
            {
                dict_sectionTodo[key] = !dict_sectionTodo[key];
                table_toDo.ReloadSections(new NSIndexSet((uint)section), UITableViewRowAnimation.None);
            }
            else
            {
                dict_sectionWorkFlow[key] = !dict_sectionWorkFlow[key];
                table_workflow.ReloadSections(new NSIndexSet((uint)section), UITableViewRowAnimation.None);
            }
        }

        private async void Workflow_refreshControl_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                workflow_refreshControl.BeginRefreshing();
                ProviderBase provider = new ProviderBase();
                ProviderUser p_user = new ProviderUser();

                string documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string localpath = Path.Combine(documentFolder, CmmVariable.M_Avatar);

                await Task.Run(() =>
                {
                    provider.UpdateAllMasterData(true);
                    provider.UpdateAllDynamicData(true);
                    p_user.UpdateCurrentUserInfo(localpath);

                    InvokeOnMainThread(() =>
                    {

                        //if (currentIndexTaskLeft == 0)
                        //    LoadDataFilterTodo();
                        //else
                        //    LoadDataFilterFromMe();

                        //LoadDataWorkFlowByGroup();



                        workflow_refreshControl.EndRefreshing();

                        if (File.Exists(localpath))
                            BT_avatar.SetImage(UIImage.FromFile(localpath), UIControlState.Normal);

                        if (tab_dentoi)
                        {
                            //HightLightText(lbl_taskLeft_denToi, true);
                            //HightLightText(lbl_taskLeft_toiBatDau, false);
                            LoadDataFilterTodo();
                        }
                        else
                        {
                            //HightLightText(lbl_taskLeft_denToi, false);
                            //HightLightText(lbl_taskLeft_toiBatDau, true);
                            LoadDataFilterFromMe();
                        }
                        LoadDataWorkFlowByGroup();
                    });
                });
            }
            catch (Exception ex)
            {
                workflow_refreshControl.EndRefreshing();
                Console.WriteLine("Error - MainView - refreshControl_valuechange - Er: " + ex.ToString());
            }
        }

        private async void Todo_refreshControl_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                todo_refreshControl.BeginRefreshing();
                ProviderUser p_user = new ProviderUser();
                ProviderBase provider = new ProviderBase();
                string documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string localpath = Path.Combine(documentFolder, CmmVariable.M_Avatar);

                await Task.Run(() =>
                {
                    provider.UpdateAllMasterData(true);
                    //provider.UpdateAllDynamicData(true);

                    provider.UpdateAllDynamicData(true);
                    p_user.UpdateCurrentUserInfo(localpath);

                    InvokeOnMainThread(() =>
                    {
                        todo_refreshControl.EndRefreshing();
                        if (File.Exists(localpath))
                            BT_avatar.SetImage(UIImage.FromFile(localpath), UIControlState.Normal);

                        if (tab_dentoi)
                        {
                            //HightLightText(lbl_taskLeft_denToi, true);
                            //HightLightText(lbl_taskLeft_toiBatDau, false);

                            LoadDataFilterTodo();
                        }
                        else
                        {
                            //HightLightText(lbl_taskLeft_denToi, false);
                            //HightLightText(lbl_taskLeft_toiBatDau, true);

                            LoadDataFilterFromMe();
                        }
                        LoadDataWorkFlowByGroup();
                    });
                });
            }
            catch (Exception ex)
            {
                todo_refreshControl.EndRefreshing();
                Console.WriteLine("Error - MainView - refreshControl_valuechange - Er: " + ex.ToString());
            }
        }

        private void BT_search_TouchUpInside(object sender, EventArgs e)
        {
            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(510, 510);
            Nav_searchPopup nav_Search = (Nav_searchPopup)Storyboard.InstantiateViewController("Nav_searchPopup");
            //PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            nav_Search.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            nav_Search.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
            //nav_Search.TransitioningDelegate = transitioningDelegate;
            this.PresentModalViewController(nav_Search, true);
        }

        #endregion

        #region custom class
        #region todo toMe data source table
        private class Todo_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cell");
            MainView parentView;
            Dictionary<string, List<BeanAppBaseExt>> dict_todo { get; set; }
            Dictionary<string, bool> dict_section { get; set; }

            public Todo_TableSource(Dictionary<string, List<BeanAppBaseExt>> _dict_todo, MainView _parentview)
            {
                dict_todo = _dict_todo;
                parentView = _parentview;
                GetDictSection();
            }

            private void GetDictSection()
            {
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
                var item = dict_todo[key][indexPath.Row];

                parentView.NavigateToDetails(item, false, true);
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
                        CGRect frame = new CGRect(0, 0, tableView.Frame.Width, 44);

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
                    var lst_appBase_cxl = parentView.lst_appBase_cxl;
                    var key = dict_section.ElementAt(indexPath.Section).Key;
                    var todo = dict_todo[key];

                    if (dict_section.Count - 1 == indexPath.Section)
                    {
                        if (indexPath.Row + 1 == todo.Count)
                        {
                            parentView.view_task_left_loadmore.Hidden = false;
                            parentView.loadmore_indicator.StartAnimating();
                            parentView.isLoadData = true;
                            parentView.loadmoreData();
                        }
                    }
                }
            }
        }
        #endregion

        #region workflow data source table
        private class WorkFlow_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellWorkFlow");
            MainView parentView;
            Dictionary<string, List<BeanAppBaseExt>> dict_workflow { get; set; }
            Dictionary<string, bool> dict_section { get; set; }

            public WorkFlow_TableSource(Dictionary<string, List<BeanAppBaseExt>> _dict_workflow, MainView _parentview)
            {
                dict_workflow = _dict_workflow;
                parentView = _parentview;
                GetDictSection();
            }

            private void GetDictSection()
            {
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
                return 105;
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
                var item = dict_workflow[key][indexPath.Row];

                parentView.NavigateToDetails(item, false, false);
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
                    var lst_appBase_fromMe = parentView.lst_appBase_fromMe;
                    var key = dict_section.ElementAt(indexPath.Section).Key;
                    var lst_workflow = dict_workflow[key];

                    if (dict_section.Count - 1 == indexPath.Section)
                    {
                        if (indexPath.Row + 1 == lst_workflow.Count)
                        {
                            parentView.view_task_left_loadmore.Hidden = false;
                            parentView.loadmore_indicator.StartAnimating();
                            parentView.isLoadData = true;
                            parentView.loadmoreData();
                        }
                    }
                }
            }
        }
        #endregion

        #region new workflow data source table
        private class NewWorkFlow_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellNewWorkFlow");
            List<BeanAppBaseExt> lst_appbaseSummarize;
            MainView parentView;

            public NewWorkFlow_TableSource(List<BeanAppBaseExt> _workflow, MainView _parentview)
            {
                lst_appbaseSummarize = _workflow;
                parentView = _parentview;
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return 1;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return lst_appbaseSummarize.Count;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 185;
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                return 0;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                var item = lst_appbaseSummarize[indexPath.Row];

                parentView.NavigateToDetails(item, true, true);

                //if (item.CreatedBy.ToLower() == CmmVariable.SysConfig.UserId.ToLower())
                //    parentView.NavigateToDetails(item, false);
                //else if (item.AssignedTo.ToLower() == CmmVariable.SysConfig.UserId.ToLower())
                //    parentView.NavigateToDetails(item, true);

                tableView.DeselectRow(indexPath, true);

            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var item = lst_appbaseSummarize[indexPath.Row];

                Custom_NewWorkFlowCell cell = new Custom_NewWorkFlowCell(cellIdentifier, indexPath.Row);
                cell.UpdateCell(item);
                return cell;
            }
        }

        #endregion
        #endregion
    }
}
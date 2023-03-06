using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreGraphics;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using Foundation;
using UIKit;
using ObjCRuntime;
using SQLite;
using BPMOPMobileV1.iOS.CustomControlClass;
using BPMOPMobileV1.iOS.IOSClass;
using System.Globalization;
using Xamarin.iOS;
using Newtonsoft.Json;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class MyRequestListView : UIViewController
    {
        AppDelegate appD = UIApplication.SharedApplication.Delegate as AppDelegate;
        List<BeanAppBaseExt> lst_appBase_fromMe;
        List<BeanAppBaseExt> lst_workflow_fromMe_result = new List<BeanAppBaseExt>();
        int fromMe_count;
        Dictionary<string, List<BeanAppBaseExt>> dict_workflow;
        Dictionary<string, bool> dict_sectionWorkFlow;
        Dictionary<string, List<BeanAppBaseExt>> dict_workflow_result = new Dictionary<string, List<BeanAppBaseExt>>();
        Custom_CalendarView custom_CalendarView = Custom_CalendarView.GetNewObject;
        ButtonsActionBotBar buttonActionBotBar;
        string menuAction;
        string query_action = string.Empty;
        UIRefreshControl refreshControl;
        bool isFirst = true;
        bool isSearch = false;
        bool isFilter = false;
        bool isFilterServer = false;
        bool isLoadMore = true;
        //bool isOnline = true;
        public bool doReload = false;
        private UITapGestureRecognizer gestureRecognizer;

        DateTime date_default;
        /// <summary>
        /// Cờ check đang thực hiện
        /// </summary>
        bool tab_inprogress = true;
        int inProcess_toMe_count;

        //filter
        public List<ClassMenu> lst_dueDateMenu_fromMe;
        public ClassMenu DuedateSelected_fromMe;
        public List<BeanAppStatus> lst_appStatus_fromMe = new List<BeanAppStatus>();
        public List<BeanAppStatus> ListAppStatus_selected_fromMe;
        DateTime temp_fromDateSelectedFromMe = new DateTime();
        DateTime temp_toDateSelectedFromMe = new DateTime();
        UIView view_custom_CalendarView;
        UIColor previousColorFilter = UIColor.FromRGB(0, 0, 0);// lay gia tri icon ban dau mac dinh

        nfloat height_search;

        Custom_AppStatusCategory custom_AppStatusCategory;
        Custom_DuedateCategory custom_DuedateCategory;

        public bool isBackFromFilter = false, isBackWithoutEditing = false;

        UIStringAttributes firstAttributes = new UIStringAttributes
        {
            // ForegroundColor = UIColor.White,
            Font = UIFont.FromName("ArialMT", 12f)
        };

        public MyRequestListView(IntPtr handle) : base(handle)
        {
        }

        #region override
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (!isBackFromFilter && !isBackWithoutEditing)
            {
                if (!isFirst)
                {
                    isLoadMore = true;
                    if (isFilterServer)
                        FilterServer(false);
                    else
                        LoadDataFilterFromMe();

                    if (!string.IsNullOrEmpty(searchBar.Text.Trim()))
                    {
                        SearchBar_TextChanged(null, null);
                    }
                }
                else
                {
                    tab_inprogress = false;
                    BT_Inprogress_TouchUpInside(null, null);
                }
            }

            if (buttonActionBotBar != null)
                bottom_view.AddSubviews(buttonActionBotBar);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            isFirst = false;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            isBackWithoutEditing = false;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ReloadFailedBeanData();

            gestureRecognizer = new UITapGestureRecognizer(Self, new Selector("hideKeyboard"));
            gestureRecognizer.ShouldReceiveTouch += delegate (UIGestureRecognizer recognizer, UITouch touch)
            {
                var name = touch.View.Class.Name;
                var touchName = touch.View.Superview.Superview.Class.Name;

                if (name == "UITableViewCellContentView")
                    return false;
                else
                    return true;
            };
            this.View.AddGestureRecognizer(gestureRecognizer);

            ViewConfiguration();
            CreateCircle();
            BT_filter.TintColor = UIColor.FromRGB(0, 0, 0);
            previousColorFilter = UIColor.FromRGB(0, 0, 0);

            LoadStatusCategory();
            LoadDueDateCategory();
            LoadDuedataFromMe();
            LoadStatusDataFromMe();
            SetDateTime();

            //LoadDataFilterFromMe();
            SetLangTitle();

            lbl_title.Text = menuAction;

            #region delegate
            BT_filter.TouchUpInside += BT_filter_TouchUpInside;
            BT_reset_filter.TouchUpInside += BT_reset_filter_TouchUpInside;
            BT_Search.TouchUpInside += BT_Search_TouchUpInside;
            BT_menu.TouchUpInside += BT_menu_TouchUpInside;
            BT_Inprogress.TouchUpInside += BT_Inprogress_TouchUpInside;
            BT_Prosessd.TouchUpInside += BT_Prosessd_TouchUpInside;

            #region fromMe
            BT_status.TouchUpInside += BT_status_TouchUpInside;
            BT_duedate.TouchUpInside += BT_duedate_TouchUpInside;

            BT_fromdate.TouchUpInside += BT_fromdate_TouchUpInside;
            BT_todate.TouchUpInside += BT_todate_TouchUpInside;
            BT_Apply_ToMe.TouchUpInside += BT_Apply_FromMe_TouchUpInside;
            #endregion

            searchBar.TextChanged += SearchBar_TextChanged;
            refreshControl.ValueChanged += RefreshControl_ValueChanged;
            CmmEvent.UpdateLangComplete += CmmEvent_UpdateLangComplete;
            #endregion
        }

        #endregion

        #region private - public method

        void ReloadFailedBeanData()
        {
            CmmIOSFunction.UpdateBeanData<BeanAppStatus>();
        }

        /// <summary>
        /// // Phê duyệt yêu cầu
        /// </summary>
        /// <param name="_menuAction">Menu action.</param>
        public void setContent(string _menuAction)
        {
            menuAction = _menuAction;
        }
        private void SetLangTitle()
        {
            searchBar.Placeholder = CmmFunction.GetTitle("TEXT_SEARCH", "Tìm kiếm …");
            custom_CalendarView.calendarView.ReloadData();
            CmmIOSFunction.SetLangToView(this.View);
            //loadData_count(fromMe_count);
        }
        private void SetConstraint()
        {
            headerView_constantHeight.Constant = 45 + 10 + CmmIOSFunction.GetHeaderViewHeight();
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                if (UIApplication.SharedApplication.KeyWindow?.SafeAreaInsets.Bottom > 0)
                {
                    contraint_heightViewNavBot.Constant += 10;
                }
            }
        }
        private void LoadStatusCategory()
        {
            var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
            string defaultIdAppstatus = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS);
            string defaultIdAppstatusFromme = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS_FROMME);

            string _whereClause = !string.IsNullOrEmpty(defaultIdAppstatus) ? string.Format(@"WHERE ID IN ({0})", defaultIdAppstatus) : "";
            string query_worflowCategory = string.Format(@"SELECT * FROM BeanAppStatus {0}", _whereClause);
            lst_appStatus_fromMe = conn.Query<BeanAppStatus>(query_worflowCategory);

            string[] arrayStatus = defaultIdAppstatusFromme.Split(",");
            for (int i = 0; i < lst_appStatus_fromMe.Count; i++)
            {
                if (arrayStatus.Contains(lst_appStatus_fromMe[i].ID.ToString()))
                    lst_appStatus_fromMe[i].IsSelected = true;
            }
        }

        private void LoadDueDateCategory()
        {
            lst_dueDateMenu_fromMe = new List<ClassMenu>();
            ClassMenu m1 = new ClassMenu() { ID = 1, section = 0, titleEN = "All", title = "Tất cả" };
            ClassMenu m2 = new ClassMenu() { ID = 2, section = 0, titleEN = "Today", title = "Trong ngày" };
            ClassMenu m3 = new ClassMenu() { ID = 3, section = 0, titleEN = "Overdue", title = "Trễ hạn" };

            lst_dueDateMenu_fromMe.AddRange(new[] { m1, m2, m3 });
            lst_dueDateMenu_fromMe[0].isSelected = true;
        }

        private void LoadDuedataFromMe()
        {
            // Tat ca
            DuedateSelected_fromMe = lst_dueDateMenu_fromMe[0];
        }
        private void LoadStatusDataFromMe()
        {
            if (ListAppStatus_selected_fromMe != null && ListAppStatus_selected_fromMe.Count >= 0)
                ListAppStatus_selected_fromMe.Clear();
            else
                ListAppStatus_selected_fromMe = new List<BeanAppStatus>();

            ListAppStatus_selected_fromMe = lst_appStatus_fromMe.FindAll(s => s.IsSelected == true);
        }
        private void SetDateTime()
        {
            temp_fromDateSelectedFromMe = DateTime.Now.AddDays(-CmmVariable.M_DataFilterDefaultDays);
            temp_toDateSelectedFromMe = DateTime.Now;
        }
        private void ViewConfiguration()
        {
            SetConstraint();
            DrawToggleView();
            view_borderInFilter1.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            view_borderInFilter1.Layer.BorderWidth = 0.6f;
            view_borderInFilter2.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            view_borderInFilter2.Layer.BorderWidth = 0.6f;

            view_custom_CalendarView = new UIView()
            {
                Frame = UIScreen.MainScreen.Bounds,
                BackgroundColor = UIColor.FromRGB(25, 25, 30).ColorWithAlpha(0.9f)
            };
            View.AddSubview(view_custom_CalendarView);
            view_custom_CalendarView.Hidden = true;
            view_custom_CalendarView.Frame = View.Frame;

            //var model = DeviceHardware.Model;
            //if (model.Contains("X") || model.Contains("11"))
            //{
            //    headerView_constantHeight.Constant = 80;
            //}

            height_search = 44;
            constraint_heightSearch.Constant = 0;

            searchBar.SearchTextField.TintColor = UIColor.Black;
            //searchBar.SearchTextField.TintColor = UIColor.FromRGB(245, 245, 245);
            searchBar.SearchTextField.Layer.CornerRadius = 4;
            searchBar.SearchTextField.Font = UIFont.FromName("ArialMT", 16f);

            //lbl_title.Text = menuAction;

            string documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string localpath = Path.Combine(documentFolder, CmmVariable.M_Avatar);

            BT_menu.ImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
            if (File.Exists(localpath))
                BT_menu.SetImage(UIImage.FromFile(localpath), UIControlState.Normal);
            else
                BT_menu.SetImage(UIImage.FromFile("Icons/icon_avatar64.png"), UIControlState.Normal);

            custom_CalendarView.viewController = this;
            custom_CalendarView.BackgroundColor = UIColor.White;
            custom_CalendarView.Layer.BorderColor = UIColor.FromRGB(65, 80, 134).CGColor;
            custom_CalendarView.Layer.BorderWidth = 1;
            custom_CalendarView.Layer.CornerRadius = 10;
            custom_CalendarView.ClipsToBounds = true;

            buttonActionBotBar = ButtonsActionBotBar.Instance;
            CGRect bottomBarFrame = new CGRect(bottom_view.Frame.X, bottom_view.Frame.Y, this.View.Frame.Width, bottom_view.Frame.Height);
            buttonActionBotBar.InitFrameView(bottomBarFrame);
            //buttonActionBotBar.InitFrameView(bottom_view.Frame);
            buttonActionBotBar.LoadStatusButton(2);
            //buttonActionBotBar.UpdateChildBroad(false);
            bottom_view.AddSubview(buttonActionBotBar);
            CmmIOSFunction.AddShadowForTopORBotBar(bottom_view, false);

            table_content.ContentInset = new UIEdgeInsets(-7, 0, 0, 0);

            refreshControl = new UIRefreshControl();
            refreshControl.TintColor = UIColor.FromRGB(9, 171, 78);

            refreshControl.AttributedTitle = new NSAttributedString(CmmFunction.GetTitle("TEXT_REFRESH_LOADING", "Loading ..."), firstAttributes);
            table_content.AddSubview(refreshControl);
            view_loadmore.BackgroundColor = UIColor.FromWhiteAlpha(1, 0.7f);
        }

        void DrawToggleView()
        {
            BT_Prosessd.SetTitle(CmmFunction.GetTitle("TEXT_PROCESSED", "Đã xử lý"), UIControlState.Normal);
            BT_Inprogress.SetTitle(CmmFunction.GetTitle("TEXT_INPROCESS", "Đang xử lý"), UIControlState.Normal);
            view_borderInFilter1.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            view_borderInFilter1.Layer.BorderWidth = 0.6f;
            view_borderInFilter2.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            view_borderInFilter2.Layer.BorderWidth = 0.6f;
            view_borderInFilter3.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            view_borderInFilter3.Layer.BorderWidth = 0.6f;

            //dang xu ly
            BT_Inprogress.BackgroundColor = UIColor.White;
            BT_Inprogress.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);
            BT_Prosessd.BackgroundColor = UIColor.Clear;
            BT_Prosessd.SetTitleColor(UIColor.White, UIControlState.Normal);
            view_choose_status.BackgroundColor = UIColor.FromRGB(0, 95, 212);

            BT_Inprogress.Layer.ShadowOffset = new CGSize(1, 2);
            BT_Inprogress.Layer.ShadowRadius = 4;
            BT_Inprogress.Layer.ShadowColor = UIColor.Black.CGColor;
            BT_Inprogress.Layer.ShadowOpacity = 0.5f;

            BT_Prosessd.Layer.ShadowOffset = new CGSize(-1, 2);
            BT_Prosessd.Layer.ShadowRadius = 4;
            BT_Prosessd.Layer.ShadowColor = UIColor.Black.CGColor;
            BT_Prosessd.Layer.ShadowOpacity = 0.0f;
        }
        private void CreateCircle()
        {
            try
            {
                double min = Math.Min(BT_menu.ImageView.Frame.Width, BT_menu.ImageView.Frame.Height);
                BT_menu.ImageView.Layer.CornerRadius = (float)(min / 2.0);
                BT_menu.ImageView.Layer.MasksToBounds = false;
                BT_menu.ImageView.Layer.BorderColor = UIColor.Clear.CGColor;
                BT_menu.ImageView.Layer.BorderWidth = 0;
                BT_menu.ImageView.ClipsToBounds = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("SettingViewController - CreateCircle - Err: " + ex.ToString());
            }
        }
        //private bool Check_FiltedStatus(DateTime _fromDateSelected, DateTime _toDateSelected)
        //{
        //    if (custom_AppStatusCategory != null) 
        //    {
        //        // so sanh cai moi va cai default
        //        string defaultIdAppstatustoMe = "";
        //        defaultIdAppstatustoMe = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS_FROMME);
        //        string[] arrayStatus = defaultIdAppstatustoMe.Split(",");
        //        var itemSelect = ExtensionCopy.CopyAll(custom_AppStatusCategory.ListAppStatus.FindAll(s => s.IsSelected == true));
        //        int sum = 0;
        //        for (int i = 0; i < itemSelect.Count; i++)
        //        {
        //            if (arrayStatus.Contains(itemSelect[i].ID.ToString()))
        //                sum++;
        //        }
        //        if (sum != arrayStatus.Length)
        //            return false;
        //    }

        //    if (custom_DuedateCategory != null)
        //    {
        //        if (!custom_DuedateCategory.ListClassMenu[0].isSelected) //!= tat ca
        //            return false;
        //    }

        //    DateTime to = new DateTime();
        //    DateTime from = new DateTime();
        //    if (CmmVariable.SysConfig.LangCode == "1066")
        //    {
        //        to = DateTime.Parse(DateTime.Now.ToString(@"dd/MM/yyyy", new CultureInfo("vi")), new CultureInfo("vi", false));
        //        from = DateTime.Parse(DateTime.Now.AddDays(-CmmVariable.M_DataFilterDefaultDays).ToString(@"dd/MM/yyyy", new CultureInfo("vi")), new CultureInfo("vi", false));
        //    }
        //    else if (CmmVariable.SysConfig.LangCode == "1033")
        //    {
        //        to = DateTime.Parse(DateTime.Now.ToString(@"MM/dd/yyyy", new CultureInfo("en")), new CultureInfo("en", false));
        //        from = DateTime.Parse(DateTime.Now.AddDays(-CmmVariable.M_DataFilterDefaultDays).ToString(@"MM/dd/yyyy", new CultureInfo("en")), new CultureInfo("en", false));
        //    }
        //    if (from != _fromDateSelected)
        //        return false;

        //    return true;
        //}
        private void ToggleFilter()
        {
            if (filter_view.Alpha == 0)
            {
                isFilter = true;
                view_filterForm_FromMe.Frame = new CGRect(0, 0, filter_view.Frame.Width, 0);

                UIView.BeginAnimations("show_animationShowFilter_fromMe");
                UIView.SetAnimationDuration(0.2f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);
                filter_view.Alpha = 1;
                view_filterForm_FromMe.Frame = new CGRect(0, 0, filter_view.Frame.Width, 460);
                view_filterForm_FromMe.Alpha = 1;
                UIView.CommitAnimations();

                tf_fromdate.TextColor = UIColor.FromRGB(0, 0, 0);
                tf_todate.TextColor = UIColor.FromRGB(0, 0, 0);
                BT_filter.TintColor = UIColor.FromRGB(35, 151, 32); // moi lan mo len thi bat filter mau xanh
                SetValueFormFilter();
            }
            else
            {
                isFilter = false;
                view_filterForm_FromMe.Frame = new CGRect(0, 0, filter_view.Frame.Width, 460);
                UIView.BeginAnimations("show_animationHideFilter_fromMe");
                UIView.SetAnimationDuration(0.2f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);
                view_filterForm_FromMe.Frame = new CGRect(0, 0, filter_view.Frame.Width, 0);
                filter_view.Alpha = 0;
                UIView.CommitAnimations();
                BT_filter.TintColor = previousColorFilter;
            }
        }

        public void HandleSectionTable(nint section, string key, int tableIndex)
        {
            dict_sectionWorkFlow[key] = !dict_sectionWorkFlow[key];
            table_content.ReloadSections(new NSIndexSet((uint)section), UITableViewRowAnimation.None);
        }

        #region fromMe
        //LOAD DATA
        private string loadData_count(int num)
        {
            string txt = "";
            try
            {
                int _dataCount = num;// GetCountNumber();
                txt = CmmFunction.GetTitle("TEXT_INPROCESS", "Đang xử lý"); //+ " (" + _dataCount.ToString() + ")";//CmmFunction.GetTitle("TEXT_FROMME", "Tôi bắt đầu")

                if (_dataCount >= 100)
                {
                    txt += " (99+)";
                }
                else if (_dataCount > 0 && _dataCount < 100)
                {
                    if (_dataCount > 0 && _dataCount < 10)
                        txt += " (0" + _dataCount.ToString() + ")";// hien thi 2 so vd: 08
                    else
                        txt += " (" + _dataCount.ToString() + ")";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MainView - loadData - Err: " + ex.ToString());
            }
            return txt;
        }

        int GetCountNumber()
        {
            int myrequest_count = 0;
            try
            {
                string count = "";

                count = new ProviderControlDynamic().GetListCountVDT_VTBD(CmmVariable.KEY_COUNT_FROMME_INPROCESS);

                if (!string.IsNullOrEmpty(count))
                {
                    var allCount = count.Split(";#");
                    if (allCount.Length > 0 && string.Compare(allCount[0], CmmVariable.KEY_COUNT_FROMME_INPROCESS) == 0)
                    {
                        if (!int.TryParse(allCount[1], out myrequest_count))
                        {
                            myrequest_count = 0;
                        }
                    }
                }
                else
                {
                    myrequest_count = 0;
#if DEBUG
                    Console.WriteLine("GetCountNumber trả về chuỗi trống.");
#endif
                }
            }
            catch (Exception ex)
            {
                myrequest_count = 0;
#if DEBUG
                Console.WriteLine("GetCountNumber - Err: " + ex.ToString());
#endif
            }
            return myrequest_count;
        }

        void LoadBtnInProgressVisual(string str_transalte)
        {
            if (tab_inprogress)
            {
                var indexA = str_transalte.IndexOf('(');
                if (indexA >= 0)
                {
                    NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                    att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(0, str_transalte.Length));
                    att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(255, 122, 58), new NSRange(indexA, str_transalte.Length - indexA)); // inbox => blue
                    BT_Inprogress.SetAttributedTitle(att, UIControlState.Normal);
                }
                else
                {
                    NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                    att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(0, str_transalte.Length));
                    BT_Inprogress.SetAttributedTitle(att, UIControlState.Normal);
                }
            }
            else
            {
                NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.White, new NSRange(0, str_transalte.Length));
                att.AddAttribute(UIStringAttributeKey.Font, UIFont.FromName("ArialMT", 14f), new NSRange(0, str_transalte.Length));
                BT_Inprogress.SetAttributedTitle(att, UIControlState.Normal);
            }
        }

        /// <summary>
        /// filter different default then get from server
        /// </summary>
        private void FilterServer(bool isLoadMoreDataFilter)
        {
            try
            {
                //dict_workflow = new Dictionary<string, List<BeanAppBaseExt>>();
                //dict_sectionWorkFlow = new Dictionary<string, bool>();
                if (!isLoadMoreDataFilter)
                    lst_appBase_fromMe = new List<BeanAppBaseExt>();

                //Check Filter Status
                string str_status = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS); // tat ca fromme
                if (ListAppStatus_selected_fromMe != null && ListAppStatus_selected_fromMe.Count > 0) // co chon status
                    str_status = string.Join(',', ListAppStatus_selected_fromMe.Select(i => i.ID));

                ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();
                int totalRecord = 0;
                var listPropertiesFilter = CmmFunction.BuildListPropertiesFilter(CmmVariable.M_ResourceViewID_FromMe, "", str_status, DuedateSelected_fromMe.ID, temp_fromDateSelectedFromMe, temp_toDateSelectedFromMe);
                //loadmore then cu filter
                if (isLoadMoreDataFilter)
                {
                    List<BeanAppBaseExt> lst_appBase_fromme_more = new List<BeanAppBaseExt>();
                    lst_appBase_fromme_more = _pControlDynamic.GetListFilterMyRequest(listPropertiesFilter, ref totalRecord, CmmVariable.M_DataFilterAPILimitData, lst_appBase_fromMe.Count);// lst_appBase_fromMe.Count
                    lst_appBase_fromme_more = lst_appBase_fromme_more.OrderByDescending(s => s.Created).ToList();

                    if (lst_appBase_fromme_more != null && lst_appBase_fromme_more.Count > 0)
                    {
                        lst_appBase_fromMe.AddRange(lst_appBase_fromme_more);

                        if (lst_appBase_fromme_more.Count < CmmVariable.M_DataFilterAPILimitData)
                            isLoadMore = false;
                    }
                    else
                    {
                        isLoadMore = false;
                        return;
                    }
                }
                else// Lan dau filter
                {
                    lst_appBase_fromMe = _pControlDynamic.GetListFilterMyRequest(listPropertiesFilter, ref totalRecord, CmmVariable.M_DataFilterAPILimitData, 0);// lst_appBase_fromMe.Count
                    lst_appBase_fromMe = lst_appBase_fromMe != null && lst_appBase_fromMe.Count > 0 ? lst_appBase_fromMe.OrderByDescending(s => s.Created).ToList() : new List<BeanAppBaseExt>();
                }

                if (lst_appBase_fromMe != null && lst_appBase_fromMe.Count > 0)
                {
                    #region Lấy danh sách đang xử lý
                    var inProgressStat = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS_FROMME_DANGXULY);
                    string[] arrayInProgressStatus = !string.IsNullOrEmpty(inProgressStat) ? inProgressStat.Split(",") : new string[] { };
                    var lstInProgress = lst_appBase_fromMe.FindAll(o => arrayInProgressStatus.Contains(o.StatusGroup.ToString()));
                    #endregion

                    #region Lấy danh sách đã xử lý
                    var progressedStat = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS_FROMME_DAXULY);
                    string[] arrayProgressedStatus = !string.IsNullOrEmpty(progressedStat) ? progressedStat.Split(",") : new string[] { };
                    var lstProgressed = lst_appBase_fromMe.FindAll(o => arrayProgressedStatus.Contains(o.StatusGroup.ToString()));
                    #endregion

                    if (tab_inprogress)
                    {
                        LoadBtnInProgressVisual(loadData_count(lstInProgress.Count()));//totalRecord
                        if (lstInProgress != null && lstInProgress.Count > 0)
                        {
                            SortListSearchAppBase("", lstInProgress);
                            return;
                        }
                    }
                    else
                    {
                        LoadBtnInProgressVisual(loadData_count(0));
                        if (lstProgressed != null && lstProgressed.Count > 0)
                        {
                            SortListSearchAppBase("", lstProgressed);
                            return;
                        }
                    }

                    //SortListAppBase("");
                    table_content.Source = null;
                    table_content.ReloadData();
                    table_content.Alpha = 0;
                    lbl_nodata.Hidden = false;
                }
                else
                {
                    LoadBtnInProgressVisual(loadData_count(0));
                    table_content.Alpha = 0;
                    lbl_nodata.Hidden = false;
                }
            }
            catch (Exception ex)
            {
                //fromMe_count = 0;
                //loadData_count(fromMe_count);
                table_content.Source = null;
                table_content.ReloadData();
                table_content.Alpha = 0;
                lbl_nodata.Hidden = false;
                Console.WriteLine("MainView - FilterServer - Err: " + ex.ToString());
            }
        }

        /// <summary>
        /// get data when in local
        /// </summary>
        private void LoadDataFilterFromMe()
        {
            try
            {
                LoadDataFromMeOffLine();
                if (Reachability.detectNetWork())
                {
                    LoadDataFromMeOnLine();
                }
            }
            catch (Exception ex)
            {
                fromMe_count = 0;
                //loadData_count(fromMe_count);
                table_content.Source = null;
                table_content.ReloadData();

                table_content.Alpha = 0;
                lbl_nodata.Hidden = false;

                Console.WriteLine("MainView - LoadDataFilterFromMe - Err: " + ex.ToString());
            }
        }

        void LoadDataFromMeOffLine()
        {
            var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
            string query = string.Empty;
            //string queryCount = string.Empty;

            //dict_workflow = new Dictionary<string, List<BeanAppBaseExt>>();
            //dict_sectionWorkFlow = new Dictionary<string, bool>();
            lst_appBase_fromMe = new List<BeanAppBaseExt>();
            //List<CountNum> lst_count_appBase_fromMe = new List<CountNum>();
            /*
            //get count
            queryCount = CreateQueryFromMe(true);
            lst_count_appBase_fromMe = conn.Query<CountNum>(queryCount);
            if (lst_count_appBase_fromMe != null && lst_count_appBase_fromMe.Count > 0)
            {
                fromMe_count = lst_count_appBase_fromMe.First().count;
                loadData_count(fromMe_count);
            }
            else
            {
                fromMe_count = 0;
                loadData_count(fromMe_count);
            }*/
            //data
            query = CreateQueryFromMe(false);
            lst_appBase_fromMe = conn.Query<BeanAppBaseExt>(query, CmmVariable.M_DataLimitRow, 0);
            if (lst_appBase_fromMe != null && lst_appBase_fromMe.Count > 0)
            {
                LoadBtnInProgressVisual(loadData_count(lst_appBase_fromMe.Count));
                SortListAppBase(query);
            }
            else
            {
                //fromMe_count = 0;
                LoadBtnInProgressVisual(loadData_count(0));
                table_content.Alpha = 0;
                lbl_nodata.Hidden = false;
            }
        }

        void LoadDataFromMeOnLine()
        {
            //dict_workflow = new Dictionary<string, List<BeanAppBaseExt>>();
            //dict_sectionWorkFlow = new Dictionary<string, bool>();
            //lst_appBase_fromMe = new List<BeanAppBaseExt>();

            //LoadBtnInProgressVisual(loadData_count(GetCountNumber()));
            GetListFromMeOnline();
        }

        async void GetListFromMeOnline(bool isLoadMore = false)
        {
            _ = Task.Run(() =>
            {
                if (!isLoadMore)
                {
                    InvokeOnMainThread(() =>
                                   {
                                       LoadBtnInProgressVisual(loadData_count(GetCountNumber()));
                                   });
                }


                try
                {
                    List<BeanAppBaseExt> lstObj = new List<BeanAppBaseExt>();
                    if (!doReload && !isLoadMore) lst_appBase_fromMe = new List<BeanAppBaseExt>();

                    lstObj = new ProviderBase().LoadMoreDataTFromSerVer(tab_inprogress ? CmmVariable.KEY_GET_FROMME_INPROCESS : CmmVariable.KEY_GET_FROMME_PROCESSED,
                        doReload ? lst_appBase_fromMe.Count : 20,
                        isLoadMore ? lst_appBase_fromMe.Count : 0);

                    if (lstObj != null && lstObj.Count > 0)
                    {
                        CmmIOSFunction.UpdateNewListDataOnline(lstObj, new SQLiteConnection(CmmVariable.M_DataPath, false));
                        if (isLoadMore)
                        {
                            if (lst_appBase_fromMe == null) lst_appBase_fromMe = new List<BeanAppBaseExt>();
                            lst_appBase_fromMe.AddRange(lstObj);
                        }
                        else
                            lst_appBase_fromMe = lstObj;

                        InvokeOnMainThread(() =>
                        {
                            SortListAppBase("");
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("GetListObj - Err: " + ex.ToString());
                }

                if (!isLoadMore)
                {
                    InvokeOnMainThread(() =>
                    {
                        if (lst_appBase_fromMe != null && lst_appBase_fromMe.Count > 0)
                        {
                            //table_content.ScrollToRow(NSIndexPath.FromItemSection(0, 0), UITableViewScrollPosition.Top, true);
                            //table_content.ContentOffset = new CGPoint(0, -table_content.ContentInset.Top);
                            table_content.ScrollRectToVisible(new CGRect(0, 0, 0, 0), false);
                        }
                        else
                        {
                            lst_appBase_fromMe = new List<BeanAppBaseExt>();
                            table_content.Alpha = 0;
                            lbl_nodata.Hidden = false;
                        }
                    });
                }
            });
        }

        private void LoadMoreDataFromMe()
        {
            try
            {
                //dict_workflow = new Dictionary<string, List<BeanAppBaseExt>>();
                //dict_sectionWorkFlow = new Dictionary<string, bool>();

                if (Reachability.detectNetWork())
                {
                    GetListFromMeOnline(true);
                }
                else
                {
                    List<BeanAppBaseExt> lst_appBase_fromMe_more = new List<BeanAppBaseExt>();
                    var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                    string query = string.Empty;
                    //data
                    query = CreateQueryFromMe(false);
                    lst_appBase_fromMe_more = conn.Query<BeanAppBaseExt>(query, CmmVariable.M_DataLimitRow, lst_appBase_fromMe.Count);

                    if (lst_appBase_fromMe_more != null && lst_appBase_fromMe_more.Count > 0)
                    {
                        lst_appBase_fromMe.AddRange(lst_appBase_fromMe_more);
                        SortListAppBase("");
                        if (lst_appBase_fromMe_more.Count < CmmVariable.M_DataLimitRow)
                            isLoadMore = false;
                    }
                    else
                    {
                        isLoadMore = false;
                    }
                }
            }
            catch (Exception ex)
            {
                table_content.Source = null;
                table_content.ReloadData();
                table_content.Alpha = 0;
                lbl_nodata.Hidden = false;
                Console.WriteLine("MainView - LoadMoreDataFromMe - Err: " + ex.ToString());
            }
        }
        private string CreateQueryFromMe(bool isGetCount)
        {
            #region Query cách cũ
            /*string query = string.Empty;
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
            var toggleFocused = CmmFunction.GetAppSettingValue(tab_inprogress ? CmmVariable.APPSTATUS_FROMME_DANGXULY : CmmVariable.APPSTATUS_FROMME_DAXULY);
            string[] arrayStatus = !string.IsNullOrEmpty(toggleFocused) ? toggleFocused.Split(",") : str_status.Split(",");
            if (ListAppStatus_selected_fromMe != null && ListAppStatus_selected_fromMe.Count > 0) // co chon status
            {
                var selectedStatus = ListAppStatus_selected_fromMe.FindAll(o => arrayStatus.Contains(o.ID.ToString())).ToList();
                str_status = string.Join(',', selectedStatus.Select(i => i.ID));
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
            return query;*/
            #endregion
            return CmmIOSFunction.GetQueryStringAppBaseVTBD(tab_inprogress);
        }
        /// <summary>
        /// sort secsion from create
        /// </summary>
        /// <param name="query"></param>
        private void SortListAppBase(string query)
        {
            if (lst_appBase_fromMe != null && lst_appBase_fromMe.Count > 0)
            {
                dict_workflow = new Dictionary<string, List<BeanAppBaseExt>>();
                dict_sectionWorkFlow = new Dictionary<string, bool>();

                string KEY_TODAY = "TEXT_TODAY`Hôm nay";
                string KEY_YESTERDAY = "TEXT_YESTERDAY`Hôm qua";
                string KEY_ORTHER = "TEXT_OLDER`Cũ hơn";

                List<BeanAppBaseExt> lst_temp1 = new List<BeanAppBaseExt>();
                List<BeanAppBaseExt> lst_temp2 = new List<BeanAppBaseExt>();
                List<BeanAppBaseExt> lst_temp3 = new List<BeanAppBaseExt>();
                dict_workflow.Add(KEY_TODAY, lst_temp1);
                dict_workflow.Add(KEY_YESTERDAY, lst_temp2);
                dict_workflow.Add(KEY_ORTHER, lst_temp3);

                //string a = JsonConvert.SerializeObject(lst_appBase_fromMe);
                foreach (var item in lst_appBase_fromMe)
                {
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

                table_content.Source = new WorkFlow_TableSource(dict_workflow, this, query);
                table_content.ReloadData();

                table_content.Alpha = 1;
                lbl_nodata.Hidden = true;
            }
            else
            {
                table_content.Source = null;
                table_content.ReloadData();

                table_content.Alpha = 0;
                lbl_nodata.Hidden = false;
            }
        }

        /// <summary>
        /// sort section from search
        /// </summary>
        /// <param name="query"></param>
        private void SortListSearchAppBase(string query, List<BeanAppBaseExt> lstSearch)
        {
            var dict = new Dictionary<string, List<BeanAppBaseExt>>();
            dict_sectionWorkFlow = new Dictionary<string, bool>();

            string KEY_TODAY = "TEXT_TODAY`Hôm nay";
            string KEY_YESTERDAY = "TEXT_YESTERDAY`Hôm qua";
            string KEY_ORTHER = "TEXT_OLDER`Cũ hơn";

            List<BeanAppBaseExt> lst_temp1 = new List<BeanAppBaseExt>();
            List<BeanAppBaseExt> lst_temp2 = new List<BeanAppBaseExt>();
            List<BeanAppBaseExt> lst_temp3 = new List<BeanAppBaseExt>();
            dict.Add(KEY_TODAY, lst_temp1);
            dict.Add(KEY_YESTERDAY, lst_temp2);
            dict.Add(KEY_ORTHER, lst_temp3);

            foreach (var item in lstSearch)
            {
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

            table_content.Alpha = 1;
            lbl_nodata.Hidden = true;

            table_content.Source = new WorkFlow_TableSource(dict, this, query);
            table_content.ReloadData();
        }

        public void ToggleCalendar_FromMe(bool _show)
        {
            if (!_show)
            {
                nfloat height = UIScreen.MainScreen.Bounds.Height * ((float)297 / 736);

                custom_CalendarView.InitFrameView(new CGRect(15, UIScreen.MainScreen.Bounds.X / 2 - height / 2, custom_CalendarView.Frame.Width - 30, height));
                custom_CalendarView.RemoveFromSuperview();
                view_custom_CalendarView.Hidden = true;
            }
            else
            {
                view_custom_CalendarView.Hidden = false;
                view_custom_CalendarView.AddSubview(custom_CalendarView);

                nfloat heightCalenda = UIScreen.MainScreen.Bounds.Height * ((float)297 / 736);

                view_custom_CalendarView.Hidden = false;
                view_custom_CalendarView.AddSubview(custom_CalendarView);
                custom_CalendarView.InitFrameView(new CGRect(15, UIScreen.MainScreen.Bounds.Height / 2 - heightCalenda / 2, UIScreen.MainScreen.Bounds.Width - 30, heightCalenda));
            }
        }
        #endregion

        public void CloseCalendarInstance()
        {
            Custom_CalendarView custom_CalendarView = Custom_CalendarView.Instance;
            if (custom_CalendarView.Superview != null)
            {
                custom_CalendarView.RemoveFromSuperview();
            }
        }
        private void NavigateToDetails(BeanAppBaseExt beanAppBase)
        {
            if (beanAppBase.ResourceCategoryId == 16) //task
            {
                string _workflowItemID = CmmFunction.GetWorkflowItemIDByUrl(beanAppBase.ItemUrl);
                string _query = string.Format("SELECT * FROM BeanWorkflowItem WHERE ID = '{0}' LIMIT 1 OFFSET 0", _workflowItemID);
                SQLiteAsyncConnection conn = new SQLiteAsyncConnection(CmmVariable.M_DataPath, false);
                List<BeanWorkflowItem> _list_workFlowItem = conn.QueryAsync<BeanWorkflowItem>(_query).Result;

                //FormTaskDetails taskDetails = (FormTaskDetails)Storyboard.InstantiateViewController("FormTaskDetails");
                //taskDetails.SetContent(beanAppBase, _list_workFlowItem[0], this);
                //this.NavigationController.PushViewController(taskDetails, true);

                BeanWorkflowItem workflowItem = null;

                if (_list_workFlowItem != null && _list_workFlowItem.Count > 0)
                    workflowItem = _list_workFlowItem[0];
                else
                {
                    try
                    {
                        workflowItem = Reachability.detectNetWork() ? new ProviderControlDynamic().getWorkFlowItemByRID(_workflowItemID).FirstOrDefault() : null;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Không lấy item được: " + ex.ToString());
                    }
                }

                if (workflowItem != null)
                {
                    FormTaskDetails taskDetails = (FormTaskDetails)Storyboard.InstantiateViewController("FormTaskDetails");
                    taskDetails.SetContent(beanAppBase, workflowItem, this);
                    this.NavigationController.PushViewController(taskDetails, true);
                }
                else
                {
                    CmmIOSFunction.commonAlertMessage(this, "VNA BPM", "Thao tác không thực hiện được.");
                }
            }
            else //chi tiet phieu{
            {
                RequestDetailsV2 v2 = (RequestDetailsV2)Storyboard.InstantiateViewController("RequestDetailsV2");
                v2.setContent(this, beanAppBase);
                this.NavigationController.PushViewController(v2, true);
            }
        }
        #endregion

        #region events

        [Export("hideKeyboard")]
        private void hideKeyboard()
        {
            this.View.EndEditing(true);
        }

        private void BT_menu_TouchUpInside(object sender, EventArgs e)
        {
            if (isSearch)
            {
                SearchToggle();
            }
            if (isFilter)
                ToggleFilter();

            AppDelegate appD = UIApplication.SharedApplication.Delegate as AppDelegate;
            appD.menu.loadData_count();
            appD.menu.UpdateItemSelect(2);
            appD.SlideMenuController.OpenLeft();
        }

        private void SearchBar_TextChanged(object sender, UISearchBarTextChangedEventArgs e)
        {
            try
            {
                string content = CmmFunction.removeSignVietnamese(searchBar.Text.Trim().ToLowerInvariant());
                if (!string.IsNullOrEmpty(content))
                {
                    var items = from item in lst_appBase_fromMe// lst_workflow_fromMe
                                where ((!string.IsNullOrEmpty(item.Title) && CmmFunction.removeSignVietnamese(item.Title.ToLowerInvariant()).Contains(content)) ||
                                //thuyngo change
                                //where ((!string.IsNullOrEmpty(item.WorkflowTitle) && CmmFunction.removeSignVietnamese(item.WorkflowTitle.ToLowerInvariant()).Contains(content)) ||
                                           (!string.IsNullOrEmpty(item.Title) && item.Title.ToLowerInvariant().Contains(content)) ||
                                           (!string.IsNullOrEmpty(item.Content) && CmmFunction.removeSignVietnamese(item.Content.ToLowerInvariant()).Contains(content)))
                                orderby item.Title
                                select item;

                    if (items != null && items.Count() > 0)
                    {
                        lst_workflow_fromMe_result = items.ToList();
                        /*if (dict_workflow_result.ContainsKey("Today"))
                            dict_workflow_result["Today"] = lst_workflow_fromMe_result;
                        else
                            dict_workflow_result.Add("Today", lst_workflow_fromMe_result);

                        table_content.Hidden = false;
                        table_content.Source = new WorkFlow_TableSource(dict_workflow_result, this, query_action);
                        table_content.ReloadData();*/

                        SortListSearchAppBase("", lst_workflow_fromMe_result);
                        lbl_nodata.Hidden = true;
                    }
                    else
                    {
                        lbl_nodata.Hidden = false;
                        //table_content.Hidden = true;
                        table_content.Alpha = 0;
                        table_content.Source = null;
                        table_content.ReloadData();
                    }
                }
                else
                {
                    /*if (query_action != null && query_action.Count() > 0)
                        lbl_nodata.Hidden = true;
                    else
                        lbl_nodata.Hidden = false;
                    */
                    /*if (lst_appBase_fromMe != null && lst_appBase_fromMe.Count > 0)
                    {
                        SortListSearchAppBase("", lst_appBase_fromMe);
                        lbl_nodata.Hidden = true;
                    }
                    else
                    {
                        lbl_nodata.Hidden = false;
                        table_content.Hidden = true;
                        table_content.Source = null;
                        table_content.ReloadData();
                    }*/

                    SortListAppBase("");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("AssignedToView - SearchBar_user_TextChanged - Err: " + ex.ToString());
            }
        }

        private async void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                isLoadMore = true;
                if (isSearch)
                {
                    searchBar.Text = "";
                    SearchToggle();
                }
                refreshControl.BeginRefreshing();
                ProviderBase provider = new ProviderBase();
                ProviderUser p_user = new ProviderUser();

                string documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string localpath = Path.Combine(documentFolder, CmmVariable.M_Avatar);
                p_user.UpdateCurrentUserInfo(localpath);
                await Task.Run(() =>
                {
                    provider.UpdateAllMasterData(true);
                    provider.UpdateAllDynamicData(true);

                    InvokeOnMainThread(() =>
                    {
                        if (File.Exists(localpath))
                            BT_menu.SetImage(UIImage.FromFile(localpath), UIControlState.Normal);

                        refreshControl.EndRefreshing();
                        if (isFilterServer)// co thay doi thi filter server
                            FilterServer(false);
                        else // default filter local
                            LoadDataFilterFromMe();
                    });
                });
            }
            catch (Exception ex)
            {
                refreshControl.EndRefreshing();
                Console.WriteLine("Error - MainView - refreshControl_valuechange - Er: " + ex.ToString());
            }
        }

        private void BT_filter_TouchUpInside(object sender, EventArgs e)
        {
            if (isSearch)
            {
                searchBar.Text = "";
                SearchToggle();
            }
            ToggleFilter();
        }

        private void SetValueFormFilter()
        {
            isBackFromFilter = false;
            ///current Border date
            if (temp_toDateSelectedFromMe != default(DateTime))
            {
                if (CmmVariable.SysConfig.LangCode == "1033")
                    tf_todate.Text = temp_toDateSelectedFromMe.ToString("MM/dd/yyyy");
                else
                    tf_todate.Text = temp_toDateSelectedFromMe.ToString("dd/MM/yyyy");
            }
            else
                tf_todate.Text = "";

            if (temp_fromDateSelectedFromMe != default(DateTime))
            {
                if (CmmVariable.SysConfig.LangCode == "1033")
                    tf_fromdate.Text = temp_fromDateSelectedFromMe.ToString("MM/dd/yyyy");
                else
                    tf_fromdate.Text = temp_fromDateSelectedFromMe.ToString("dd/MM/yyyy");
            }
            else
                tf_fromdate.Text = "";


            //current status
            if (ListAppStatus_selected_fromMe.Count == 1)
            {
                if (CmmVariable.SysConfig.LangCode == "1033")
                    tf_lbl_status.Text = ListAppStatus_selected_fromMe[0].TitleEN;
                else
                    tf_lbl_status.Text = ListAppStatus_selected_fromMe[0].Title;
            }
            else if (ListAppStatus_selected_fromMe.Count > 1 && ListAppStatus_selected_fromMe.Count <= lst_appStatus_fromMe.Count - 1)
            {
                if (CmmVariable.SysConfig.LangCode == "1033")
                    tf_lbl_status.Text = ListAppStatus_selected_fromMe[0].TitleEN;
                else
                    tf_lbl_status.Text = ListAppStatus_selected_fromMe[0].Title;

                tf_lbl_status.Text += ",+(" + (ListAppStatus_selected_fromMe.Count - 1).ToString() + ")";
            }
            else //tat ca
            {
                tf_lbl_status.Text = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
            }

            //current duedate
            if (CmmVariable.SysConfig.LangCode == "1033")
                lbl_dueDate.Text = DuedateSelected_fromMe.titleEN;
            else
                lbl_dueDate.Text = DuedateSelected_fromMe.title;

            //setting current select of Custom_AppStatusCategory
            if (custom_AppStatusCategory != null)
            {
                lst_appStatus_fromMe = lst_appStatus_fromMe.Select(a => { a.IsSelected = false; return a; }).ToList();
                foreach (var itemselect in ListAppStatus_selected_fromMe)
                {
                    foreach (var itemmenu in lst_appStatus_fromMe)
                    {
                        if (itemmenu.ID == itemselect.ID)
                        {
                            itemmenu.IsSelected = true;
                            break;
                        }
                    }
                }
            }
            //setting current select of Custom_DuedateCategory
            if (custom_DuedateCategory != null)
            {
                lst_dueDateMenu_fromMe = lst_dueDateMenu_fromMe.Select(a => { a.isSelected = false; return a; }).ToList();
                foreach (var itemmenu in lst_dueDateMenu_fromMe)
                {
                    if (itemmenu.ID == DuedateSelected_fromMe.ID)
                    {
                        itemmenu.isSelected = true;
                        break;
                    }
                }
            }
        }

        private void BT_reset_filter_TouchUpInside(object sender, EventArgs e)
        {
            ResetFilter();
        }

        private void ResetFilter()
        {
            isLoadMore = true;
            isFilterServer = false;
            BT_filter.TintColor = UIColor.FromRGB(0, 0, 0);
            previousColorFilter = UIColor.FromRGB(0, 0, 0);
            //init data
            LoadStatusCategory();
            LoadDueDateCategory();
            //load data
            LoadDuedataFromMe();
            LoadStatusDataFromMe();
            SetDateTime();

            LoadDataFilterFromMe();

            if (custom_AppStatusCategory != null)
            {
                custom_AppStatusCategory.ListAppStatus = lst_appStatus_fromMe;
                custom_AppStatusCategory.ListAppStatus_selected = ListAppStatus_selected_fromMe;
            }
            if (custom_DuedateCategory != null)
            {
                custom_DuedateCategory.ListClassMenu = lst_dueDateMenu_fromMe;
                custom_DuedateCategory.ClassMenu_selected = DuedateSelected_fromMe;
            }
            ResetSetValueFormFilter(ListAppStatus_selected_fromMe, DuedateSelected_fromMe);
            ToggleFilter();
            isBackFromFilter = false;
        }

        private void ResetSetValueFormFilter(List<BeanAppStatus> selectStatus, ClassMenu selectDuedate)
        {
            if (CmmVariable.SysConfig.LangCode == "1033")
                tf_fromdate.Text = DateTime.Now.AddDays(-CmmVariable.M_DataFilterDefaultDays).ToString(@"MM/dd/yyyy", new CultureInfo("en"));
            else //if (CmmVariable.SysConfig.LangCode == "1066")
                tf_fromdate.Text = DateTime.Now.AddDays(-CmmVariable.M_DataFilterDefaultDays).ToString(@"dd/MM/yyyy", new CultureInfo("vi"));

            if (CmmVariable.SysConfig.LangCode == "1033")
                tf_todate.Text = DateTime.Now.ToString(@"MM/dd/yyyy", new CultureInfo("en"));
            else //if (CmmVariable.SysConfig.LangCode == "1066")
                tf_todate.Text = DateTime.Now.ToString(@"dd/MM/yyyy", new CultureInfo("vi"));

            BT_fromdate.Layer.BorderColor = UIColor.FromRGB(65, 80, 134).CGColor;
            BT_fromdate.Layer.BorderWidth = 0;

            BT_todate.Layer.BorderColor = UIColor.FromRGB(65, 80, 134).CGColor;
            BT_todate.Layer.BorderWidth = 0;
            //current status
            if (selectStatus != null)
            {
                if (selectStatus.Count == 1)
                {
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        tf_lbl_status.Text = selectStatus[0].TitleEN;
                    else
                        tf_lbl_status.Text = selectStatus[0].Title;
                }
                else if (selectStatus.Count > 1 && selectStatus.Count <= lst_appStatus_fromMe.Count - 1)
                {
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        tf_lbl_status.Text = selectStatus[0].TitleEN;
                    else
                        tf_lbl_status.Text = selectStatus[0].Title;

                    tf_lbl_status.Text += ",+(" + (selectStatus.Count - 1).ToString() + ")";
                }
                else //tat ca
                {
                    tf_lbl_status.Text = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
                }
            }
            else //tat ca
            {
                tf_lbl_status.Text = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
            }
            //current duedate
            if (selectDuedate != null)
            {
                if (CmmVariable.SysConfig.LangCode == "1033")
                    lbl_dueDate.Text = selectDuedate.titleEN;
                else
                    lbl_dueDate.Text = selectDuedate.title;
            }

        }

        private void BT_Search_TouchUpInside(object sender, EventArgs e)
        {
            if (isFilter)
            {
                isFilter = false;
                ToggleFilter();
            }
            SearchToggle();
        }

        private void SearchToggle()
        {
            if (!isSearch) // search dang dong
            {
                if (view_search.Frame.Height > 0)
                {
                    if (!string.IsNullOrEmpty(searchBar.Text.Trim()))
                    {
                        isSearch = true;
                        searchBar.BecomeFirstResponder();
                        BT_Search.Enabled = true;
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
                        BT_Search.Enabled = true;
                        BT_Search.TintColor = UIColor.FromRGB(0, 0, 0);
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
                    constraint_heightSearch.Constant = height_search;
                    UIView.CommitAnimations();
                    searchBar.BecomeFirstResponder();
                    BT_Search.Enabled = true;
                    BT_Search.TintColor = UIColor.FromRGB(40, 176, 255);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(searchBar.Text.Trim()))
                {
                    isSearch = false;
                    this.View.EndEditing(true);
                    BT_Search.Enabled = true;
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
                    BT_Search.Enabled = true;
                    BT_Search.TintColor = UIColor.FromRGB(0, 0, 0);
                }
            }
        }

        private void BT_home_TouchUpInside(object sender, EventArgs e)
        {
            MainView mainview = (MainView)Storyboard.InstantiateViewController("MainView");
            appD.NavController.PushViewController(mainview, false);
        }

        private void BT_RequestList_TouchUpInside(object sender, EventArgs e)
        {
            RequestListView requestListView = (RequestListView)Storyboard.InstantiateViewController("RequestListView");
            requestListView.setContent(CmmFunction.GetTitle("TEXT_TOME", "Đến tôi"));
            appD.NavController.PushViewController(requestListView, false);
        }

        private void BT_MyRequestList_TouchUpInside(object sender, EventArgs e)
        {
            //reload data
        }

        private void CmmEvent_UpdateLangComplete(object sender, CmmEvent.UpdateEventArgs e)
        {
            try
            {
                InvokeOnMainThread(() =>
                {
                    SetLangTitle();
                    table_content.ReloadData();
                    refreshControl.AttributedTitle = new NSAttributedString(CmmFunction.GetTitle("TEXT_REFRESH_LOADING", "Loading ..."), firstAttributes);

                    ChangeTab();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("MainView - CmmEvent_UpdateLangComplete - ERR: " + ex.ToString());
            }
        }

        private void BT_Inprogress_TouchUpInside(object sender, EventArgs e)
        {
            if (!tab_inprogress)
            {
                BT_filter.TintColor = UIColor.FromRGB(0, 0, 0);
                previousColorFilter = UIColor.FromRGB(0, 0, 0);
                tab_inprogress = true;

                ChangeTab();
            }
        }
        private void BT_Prosessd_TouchUpInside(object sender, EventArgs e)
        {
            if (tab_inprogress)
            {
                BT_filter.TintColor = UIColor.FromRGB(0, 0, 0);
                previousColorFilter = UIColor.FromRGB(0, 0, 0);
                tab_inprogress = false;

                ChangeTab();
            }
        }
        #region tab_fromMe

        private void ChangeTab()
        {
            isLoadMore = true;
            isFilterServer = false;

            LoadStatusCategory();
            LoadDueDateCategory();
            LoadDuedataFromMe();
            LoadStatusDataFromMe();
            SetDateTime();

            tf_todate.Layer.BorderColor = UIColor.FromRGB(65, 80, 134).CGColor;
            tf_todate.Layer.BorderWidth = 0;

            tf_fromdate.Layer.BorderColor = UIColor.FromRGB(65, 80, 134).CGColor;
            tf_fromdate.Layer.BorderWidth = 0;

            ///filter
            //if (isFilterServer)
            //    FilterServer(false);
            //else
            LoadDataFilterFromMe();

            ToggleTodo();
        }

        private void ToggleTodo()
        {
            if (isFilter && !isFilterServer)
            {
                isFilter = false;
                ToggleFilter();
            }

            if (tab_inprogress) // dang la trang thai da xu ly -> dang xu ly
            {
                BT_Inprogress.BackgroundColor = UIColor.White;
                BT_Inprogress.SetTitleColor(UIColor.FromRGB(0, 95, 212), UIControlState.Normal);
                BT_Prosessd.BackgroundColor = UIColor.Clear;
                BT_Prosessd.SetTitleColor(UIColor.White, UIControlState.Normal);

                BT_Prosessd.Layer.ShadowOpacity = 0.0f;
                BT_Inprogress.Layer.ShadowOpacity = 0.5f;
            }
            else // dang la trang thai dang xu ly -> da xu ly 
            {
                BT_Prosessd.BackgroundColor = UIColor.White;
                BT_Prosessd.SetTitleColor(UIColor.FromRGB(0, 95, 212), UIControlState.Normal);
                BT_Inprogress.BackgroundColor = UIColor.Clear;
                BT_Inprogress.SetTitleColor(UIColor.White, UIControlState.Normal);

                BT_Prosessd.Layer.ShadowOpacity = 0.5f;
                BT_Inprogress.Layer.ShadowOpacity = 0.0f;
            }
            ButtonMenuStyleChange();
        }

        private void ButtonMenuStyleChange()
        {
            // set str default
            string str_daxuly = CmmFunction.GetTitle("TEXT_PROCESSED", "Đã xử lý");
            string str_dangxuly = isFilterServer ? BT_Inprogress.TitleLabel.Text : loadData_count(GetCountNumber());//CmmFunction.GetTitle("TEXT_INPROCESS", "Đang xử lý");
            //BT_Inprogress.Title(UIControlState.Normal)
            // set title
            //BT_Prosessd.SetTitle(str_daxuly, UIControlState.Normal);//Đã xử lý
            //BT_Prosessd.TitleLabel.Text = str_daxuly;
            /*
            if (inProcess_toMe_count >= 100)//Đang xử lý
            {
                str_dangxuly = str_dangxuly + " (99+)";
                BT_Inprogress.SetTitle(str_dangxuly, UIControlState.Normal);
            }
            else if (inProcess_toMe_count > 0 && inProcess_toMe_count < 100)
            {
                if (inProcess_toMe_count > 0 && inProcess_toMe_count < 10)
                    str_dangxuly = str_dangxuly + " (0" + inProcess_toMe_count.ToString() + ")";// hien thi 2 so vd: 08
                else
                    str_dangxuly = str_dangxuly + " (" + inProcess_toMe_count.ToString() + ")";

                BT_Inprogress.SetTitle(str_dangxuly, UIControlState.Normal);
            }
            else
            {
                BT_Inprogress.SetTitle(str_dangxuly, UIControlState.Normal);
                //BT_Inprogress.TitleLabel.Text = str_dangxuly;
            }*/

            // setting attribuilt
            NSMutableAttributedString att_dxl = new NSMutableAttributedString(str_daxuly);
            NSMutableAttributedString att_dangxl = new NSMutableAttributedString(str_dangxuly);
            if (tab_inprogress)
            {
                /*
                //Đã xử lý khong to mau, khong co so
                att_dxl.AddAttribute(UIStringAttributeKey.Font, UIFont.FromName("ArialMT", 14f), new NSRange(0, str_daxuly.Length));
                att_dxl.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.White, new NSRange(0, str_daxuly.Length));
                BT_Prosessd.SetAttributedTitle(att_dxl, UIControlState.Normal);

                //Đang xử lý to mau , co so
                if (str_dangxuly.Contains("("))
                {
                    if (!str_dangxuly.Contains("("))
                        str_dangxuly = str_dangxuly + " (" + inProcess_toMe_count + ")";
                    var indexA = str_dangxuly.IndexOf('(');

                    att_dangxl.AddAttribute(UIStringAttributeKey.Font, UIFont.FromName("Arial-BoldMT", 14f), new NSRange(0, att_dangxl.Length));
                    att_dangxl.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(0, att_dangxl.Length));
                    att_dangxl.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(255, 122, 58), new NSRange(indexA, str_dangxuly.Length - indexA));
                    BT_Inprogress.SetAttributedTitle(att_dangxl, UIControlState.Normal);

                }
                else
                {
                    //att_dangxl = new NSMutableAttributedString(str_dangxuly);
                    att_dangxl.AddAttribute(UIStringAttributeKey.Font, UIFont.FromName("Arial-BoldMT", 14f), new NSRange(0, att_dangxl.Length));
                    att_dangxl.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(0, att_dangxl.Length));
                    BT_Inprogress.SetAttributedTitle(att_dangxl, UIControlState.Normal);
                }*/
                //int countObj = GetCountNumber();

                //var btnInProgessTxt = loadData_count(GetCountNumber());
                LoadBtnInProgressVisual(str_dangxuly);//btnInProgessTxt

                att_dxl.AddAttribute(UIStringAttributeKey.Font, UIFont.FromName("ArialMT", 14f), new NSRange(0, att_dxl.Length));
                att_dxl.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.White, new NSRange(0, att_dxl.Length));
                BT_Prosessd.SetAttributedTitle(att_dxl, UIControlState.Normal);
            }
            else
            {
                //Đã xử lý khong to mau, khong co so
                //att_dxl = new NSMutableAttributedString(str_daxuly);
                att_dxl.AddAttribute(UIStringAttributeKey.Font, UIFont.FromName("Arial-BoldMT", 14f), new NSRange(0, att_dxl.Length));
                att_dxl.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(0, att_dxl.Length));
                BT_Prosessd.SetAttributedTitle(att_dxl, UIControlState.Normal);


                //Đang xử lý co so ng khong to mau
                //att_dangxl = new NSMutableAttributedString(str_dangxuly);
                att_dangxl.AddAttribute(UIStringAttributeKey.Font, UIFont.FromName("ArialMT", 14f), new NSRange(0, att_dangxl.Length));
                att_dangxl.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.White, new NSRange(0, att_dangxl.Length));
                BT_Inprogress.SetAttributedTitle(att_dangxl, UIControlState.Normal);
            }
        }

        #region handle task

        public void NavigateToAttachView(BeanAttachFile currentAttachFile)
        {
            ShowAttachmentView showAttachmentView = Storyboard.InstantiateViewController("ShowAttachmentView") as ShowAttachmentView;
            showAttachmentView.setContent(this, currentAttachFile);
            this.NavigationController.PushViewController(showAttachmentView, true);
        }
        public void ReloadDataForm()
        {
            LoadStatusCategory();
            LoadDueDateCategory();
            LoadDuedataFromMe();
            LoadStatusDataFromMe();

            LoadDataFilterFromMe();
        }
        public async void loadmoreData()
        {
            view_loadmore.Hidden = false;
            indicator_loadmore.StartAnimating();

            await Task.Run(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5f));
                InvokeOnMainThread(() =>
                {
                    if (isFilterServer)
                    {
                        FilterServer(true);
                    }
                    else
                    {
                        LoadMoreDataFromMe();
                    }
                    indicator_loadmore.StopAnimating();
                    view_loadmore.Hidden = true;
                });
            });
        }

        #endregion

        private void BT_duedate_TouchUpInside(object sender, EventArgs e)
        {
            ToggleMenuDueDate();
        }

        private void BT_status_TouchUpInside(object sender, EventArgs e)
        {
            ToggleMenuStatus();
        }

        private void ToggleMenuStatus()
        {
            custom_AppStatusCategory = Custom_AppStatusCategory.Instance;
            custom_AppStatusCategory.ItemNoIcon = false;
            custom_AppStatusCategory.viewController = this;
            custom_AppStatusCategory.LBL_inputView = tf_lbl_status;
            custom_AppStatusCategory.showItemClear = false;
            custom_AppStatusCategory.CheckAll = true;

            string defaultIdAppstatustoMe = CmmFunction.GetAppSettingValue(tab_inprogress ? CmmVariable.APPSTATUS_FROMME_DANGXULY : CmmVariable.APPSTATUS_FROMME_DAXULY);
            string[] arrayStatus = defaultIdAppstatustoMe.Split(",");
            custom_AppStatusCategory.ListAppStatus = lst_appStatus_fromMe.FindAll(s => arrayStatus.FirstOrDefault(o => string.Compare(o, s.ID.ToString()) == 0) != null);//lst_appStatus_toMe;

            //custom_AppStatusCategory.ListAppStatus = lst_appStatus_fromMe;
            custom_AppStatusCategory.ListAppStatus_selected = ListAppStatus_selected_fromMe;
            custom_AppStatusCategory.TableLoadData();
            custom_AppStatusCategory.BackupData();

            MultiChoiceTableView multiChoiceTableView = Storyboard.InstantiateViewController("MultiChoiceTableView") as MultiChoiceTableView;
            multiChoiceTableView.setContent(this, true, custom_AppStatusCategory, CmmFunction.GetTitle("TEXT_STATUS", "Tình trạng"));
            this.NavigationController.PushViewController(multiChoiceTableView, true);
        }
        private void ToggleMenuDueDate()
        {
            custom_DuedateCategory = Custom_DuedateCategory.Instance;
            custom_DuedateCategory.ItemNoIcon = true;
            custom_DuedateCategory.viewController = this;
            custom_DuedateCategory.LBL_inputView = lbl_dueDate;
            custom_DuedateCategory.ClassMenu_selected = DuedateSelected_fromMe;
            custom_DuedateCategory.ListClassMenu = lst_dueDateMenu_fromMe;
            custom_DuedateCategory.TableLoadData();
            custom_DuedateCategory.BackupData();

            MultiChoiceTableView multiChoiceTableView = Storyboard.InstantiateViewController("MultiChoiceTableView") as MultiChoiceTableView;
            multiChoiceTableView.setContent(this, false, custom_DuedateCategory, CmmFunction.GetTitle("TEXT_DUEDATE", "Hạn hoàn thành"));
            this.NavigationController.PushViewController(multiChoiceTableView, true);
        }
        public void CloseDueDateCateInstance()
        {
            Custom_DuedateCategory custom_DuedateCategory = Custom_DuedateCategory.Instance;
            if (custom_DuedateCategory.Superview != null)
                custom_DuedateCategory.RemoveFromSuperview();
        }
        private void BT_fromdate_TouchUpInside(object sender, EventArgs e)
        {
            tf_fromdate.TextColor = UIColor.FromRGB(0, 0, 0);
            tf_todate.TextColor = UIColor.FromRGB(0, 0, 0);

            custom_CalendarView.inputView = tf_fromdate;
            custom_CalendarView.viewController = this;
            custom_CalendarView.SetUpDate();

            ToggleCalendar_FromMe(true);
        }
        private void BT_todate_TouchUpInside(object sender, EventArgs e)
        {
            tf_fromdate.TextColor = UIColor.FromRGB(0, 0, 0);
            tf_todate.TextColor = UIColor.FromRGB(0, 0, 0);

            custom_CalendarView.inputView = tf_todate;
            custom_CalendarView.viewController = this;
            custom_CalendarView.SetUpDate();

            ToggleCalendar_FromMe(true);
        }

        public Dictionary<string, string> GetDictionaryFilter(List<KeyValuePair<string, string>> _lstFilter, bool IsVDT)
        {

            string _flagTrangThai = _lstFilter.Where(x => x.Key.ToLowerInvariant().Contains("trạng thái")).First().Value;
            string _flagHanXuLy = _lstFilter.Where(x => x.Key.ToLowerInvariant().Contains("hạn xử lý")).First().Value;
            string _flagTuNgay = _lstFilter.Where(x => x.Key.ToLowerInvariant().Contains("từ ngày")).First().Value;
            string _flagDenNgay = _lstFilter.Where(x => x.Key.ToLowerInvariant().Contains("đến ngày")).First().Value;

            Dictionary<string, string> _dict = new Dictionary<string, string>();

            if (IsVDT) // VDT có tình trạng
            {
                _dict.Add("resourceviewid", "221"); // add để test

                string _flagTinhTrang = _lstFilter.Where(x => x.Key.ToLowerInvariant().Contains("tình trạng")).First().Value;
                _dict.Add("viewtype", _flagTinhTrang.Equals("1") ? "2" : "4");
            }

            _dict.Add("statusgroup", _flagTrangThai);

            switch (_flagHanXuLy)
            {
                default:
                case "1": // "Tất cả"
                    { break; }
                case "2": // "Trong ngày"
                    {
                        _dict.Add("duedate-gte", "2021-03-08");
                        //_dict.Add("duedate-gte", DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd")); 2021 - 03 - 08
                        _dict.Add("duedate-lte", DateTime.Now.Date.AddDays(1).ToString("yyyy-MM-dd"));
                        break;
                    }
                case "3": // "Trễ hạn"
                    {
                        _dict.Add("duedate-lte", DateTime.Now.Date.ToString("yyyy-MM-dd")); break;
                    }
            }

            if (!String.IsNullOrEmpty(_flagTuNgay))
            {
                DateTime _temp = DateTime.ParseExact(_flagTuNgay, "dd/MM/yyyy", null);
                _dict.Add("created-gte", _temp.Date.ToString("yyyy-MM-dd"));
            }
            if (!String.IsNullOrEmpty(_flagDenNgay))
            {
                DateTime _temp = DateTime.ParseExact(_flagDenNgay, "dd/MM/yyyy", null);
                _dict.Add("created-lte", _temp.Date.ToString("yyyy-MM-dd"));
            }
            _dict.Add("lcid", CmmVariable.SysConfig.LangCode);

            return _dict;
        }

        private void BT_Apply_FromMe_TouchUpInside(object sender, EventArgs e)
        {
            ApplyFilter();
        }
        private void ApplyFilter()
        {
            DateTime fromDate = new DateTime();
            DateTime toDate = new DateTime();

            if (tf_fromdate.Text != "")
                fromDate = DateTime.Parse(tf_fromdate.Text, new CultureInfo(CmmVariable.SysConfig.LangCode == "1033" ? "en" : "vi", false));
            else
                fromDate = new DateTime();
            if (tf_todate.Text != "")
                toDate = DateTime.Parse(tf_todate.Text, new CultureInfo(CmmVariable.SysConfig.LangCode == "1033" ? "en" : "vi", false));
            else
                toDate = new DateTime();

            if (fromDate > toDate && toDate != default(DateTime))
            {
                CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("TEXT_DATE_INVALID", "Ngày bắt đầu không thể lớn hơn ngày kết thúc, vui lòng chọn lại"));

                tf_fromdate.TextColor = UIColor.FromRGB(179, 0, 10);
                tf_todate.TextColor = UIColor.FromRGB(179, 0, 10);
            }
            else
            {
                BT_filter.TintColor = UIColor.FromRGB(35, 151, 32);
                previousColorFilter = UIColor.FromRGB(35, 151, 32);
                isFilterServer = true;
                isLoadMore = true;
                temp_fromDateSelectedFromMe = fromDate;
                temp_toDateSelectedFromMe = toDate;

                if (custom_AppStatusCategory != null)
                    ListAppStatus_selected_fromMe = custom_AppStatusCategory.ListAppStatus_selected;
                if (custom_DuedateCategory != null)
                    DuedateSelected_fromMe = custom_DuedateCategory.ClassMenu_selected;

                FilterServer(false);

                searchBar.Text = string.Empty;
                ToggleFilter();
                this.View.EndEditing(true);
            }
        }
        #endregion

        #endregion

        #region custom class

        #region WorkFlowItem data source table
        private class WorkFlow_TableSource : UITableViewSource
        {
            List<BeanAppBaseExt> lst_workflow;
            public static Dictionary<string, List<BeanAppBaseExt>> indexedCateSession;
            NSString cellIdentifier = new NSString("cell");
            MyRequestListView parentView;

            Dictionary<string, List<BeanAppBaseExt>> dict_workflow { get; set; }
            Dictionary<string, bool> dict_section { get; set; }

            string query = "";
            int limit = 20;
            bool isLoadMore = true;
            bool isFilterServer = true;

            public WorkFlow_TableSource(Dictionary<string, List<BeanAppBaseExt>> _dict_workflow, MyRequestListView _parentview, string _query)
            {
                dict_workflow = _dict_workflow;
                parentView = _parentview;
                query = _query;
                isFilterServer = parentView.isFilterServer;
                GetDictSection();
            }

            private void GetDictSection()
            {
                dict_section = new Dictionary<string, bool>();
                //dict_section = parentView.dict_sectionWorkFlow;
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
            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 90;
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
                        var arrKey = key.Split("`");
                        if (key.Contains("`"))
                            key = CmmFunction.GetTitle(arrKey[0], arrKey[1]);
                        headerView.LoadData(section, key);
                        return headerView;
                    }
                    else
                        return null;
                }
            }
            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                var key = dict_section.ElementAt(indexPath.Section).Key;
                var workFlow = dict_workflow[key][indexPath.Row];

                parentView.NavigateToDetails(workFlow);
                tableView.DeselectRow(indexPath, true);
            }
            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var key = dict_section.ElementAt(indexPath.Section).Key;
                var workFlow = dict_workflow[key][indexPath.Row];

                bool isOdd = true;
                if (indexPath.Row % 2 == 0)
                    isOdd = false;

                Custom_WorkFlowItemCell cell = new Custom_WorkFlowItemCell(cellIdentifier);
                cell.UpdateCell(workFlow, isOdd);
                return cell;
            }
            public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
            {
                if (parentView.isLoadMore)
                {
                    var lst_appBase_fromme = parentView.lst_appBase_fromMe;
                    int sumItem = 0;
                    for (int i = 0; i < indexPath.Section; i++)
                    {
                        var keySum = dict_section.ElementAt(i).Key;
                        var todoSum = dict_workflow[keySum];
                        sumItem += todoSum.Count;
                    }
                    sumItem += 1;//them mot item
                    sumItem += indexPath.Row;
                    if (sumItem % (isFilterServer ? CmmVariable.M_DataFilterAPILimitData : CmmVariable.M_DataLimitRow) == 0 && lst_appBase_fromme.Count == sumItem) // boi so cua 20 va dong thoi la item cuoi cung
                    {
                        parentView.view_loadmore.Hidden = false;
                        parentView.indicator_loadmore.StartAnimating();
                        parentView.loadmoreData();
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}

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
using SQLite;
using ObjCRuntime;
using UIKit;
using System.Net;
using BPMOPMobileV1.iOS.CustomControlClass;
using BPMOPMobileV1.iOS.IOSClass;
using System.Globalization;
using Xamarin.iOS;
using Newtonsoft.Json;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class RequestListViewApp : UIViewController
    {
        List<BeanAppBaseExt> lst_appBase_cxl = new List<BeanAppBaseExt>();
        List<BeanAppBaseExt> lst_notify_cxl_results = new List<BeanAppBaseExt>();

        int inProcess_toMe_count = 0;
        int Process_toMe_count = 0;
        Dictionary<string, List<BeanAppBaseExt>> dict_todo;
        Dictionary<string, List<BeanAppBaseExt>> dict_todo_result = new Dictionary<string, List<BeanAppBaseExt>>();
        Custom_CalendarView custom_CalendarView = Custom_CalendarView.GetNewObject;
        string menuAction;
        string query_action = string.Empty;
        bool isFromDate_ToMe = true;
        UIRefreshControl refreshControl;
        //int limit = 100;
        //int offset = 0;
        bool isFisrt = true;
        private UITapGestureRecognizer gestureRecognizer;
        AppDelegate appD = UIApplication.SharedApplication.Delegate as AppDelegate;
        private nfloat origin_filterForm_toMe_height;
        ButtonsActionBroadBotBarApplication buttonsActionBroadBotBarApplication;

        //filter
        public List<ClassMenu> lst_dueDateMenu_toMe;
        public ClassMenu DuedateSelected_toMe;
        public List<ClassMenu> lst_conditionMenu_toMe;
        public ClassMenu conditionSelected_toMe;
        public List<BeanAppStatus> lst_appStatus_toMe = new List<BeanAppStatus>();
        public List<BeanAppStatus> ListAppStatus_selected_toMe;
        DateTime temp_fromDateSelectedToMe = new DateTime();
        DateTime temp_toDateSelectedToMe = new DateTime();
        UIColor previousColorFilter = UIColor.FromRGB(0, 0, 0);// lay gia tr icon ban dau mac dinh

        //DateTime date_default;
        UIView view_custom_CalendarView;
        bool isSearch = false;
        bool isFilter = false;
        bool isFilterServer = false;
        bool tab_inprogress = true;
        bool isLoadMore = true;
        nfloat height_search;
        Custom_AppStatusCategory custom_AppStatusCategory;
        Custom_DuedateCategory custom_DuedateCategory;
        Custom_AppConditionCategory custom_ConditionCategory;
        // BeanWorkflow
        BeanWorkflow workflowSelected { get; set; }


        public RequestListViewApp(IntPtr handle) : base(handle)
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

            if (!isFisrt)
            {
                isLoadMore = true;
                if (isFilterServer)
                    FilterServer(false);
                else
                    LoadDataFilterTodo();
                ButtonMenuStyleChange();
            }

            if (buttonsActionBroadBotBarApplication != null)
                bottom_view.AddSubviews(buttonsActionBroadBotBarApplication);
        }
        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            isFisrt = false;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

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
            //CreateCircle();
            //init data
            LoadStatusCategory(tab_inprogress ? CmmVariable.APPSTATUS_TOME_DANGXULY : CmmVariable.APPSTATUS_TOME_DAXULY);
            LoadDueDateCategory();
            LoadConditionCategory();
            // data
            LoadDuedataToMe();
            LoadConditionDataToMe();
            LoadStatusDataToMe();
            SetDateTime();
            //filter
            LoadDataFilterTodo();
            ButtonMenuStyleChange();

            SetLangTitle();

            BT_filter.TintColor = UIColor.FromRGB(0, 0, 0);
            #region delegate
            // bt topview
            BT_menu.TouchUpInside += BT_back_TouchUpInside;
            BT_filter.TouchUpInside += BT_filter_TouchUpInside;
            BT_reset_filter.TouchUpInside += BT_reset_filter_TouchUpInside;
            BT_Search.TouchUpInside += BT_Search_TouchUpInside;
            BT_Inprogress.TouchUpInside += BT_Inprogress_TouchUpInside;
            BT_Prosessd.TouchUpInside += BT_Prosessd_TouchUpInside;
            searchBar.TextChanged += SearchBar_TextChanged;

            #region filter toMe
            BT_status.TouchUpInside += BT_status_TouchUpInside;
            BT_duedate.TouchUpInside += BT_duedate_TouchUpInside;
            BT_State.TouchUpInside += BT_State_TouchUpInside;

            BT_fromdate.TouchUpInside += BT_fromdate_TouchUpInside;
            BT_todate.TouchUpInside += BT_todate_TouchUpInside;
            BT_Apply_ToMe.TouchUpInside += BT_Apply_ToMe_TouchUpInside;
            #endregion


            refreshControl.ValueChanged += RefreshControl_ValueChanged;
            CmmEvent.UpdateLangComplete += CmmEvent_UpdateLangComplete;
            #endregion
        }



        private void BT_Inprogress_TouchUpInside(object sender, EventArgs e)
        {
            if (!tab_inprogress)
            {
                //condition in filter
                conditionSelected_toMe = lst_conditionMenu_toMe[0];
                lst_conditionMenu_toMe = lst_conditionMenu_toMe.Select(s => { s.isSelected = false; return s; }).ToList();
                lst_conditionMenu_toMe[0].isSelected = true;

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
                //condition in filter
                conditionSelected_toMe = lst_conditionMenu_toMe[1];
                lst_conditionMenu_toMe = lst_conditionMenu_toMe.Select(s => { s.isSelected = false; return s; }).ToList();
                lst_conditionMenu_toMe[1].isSelected = true;


                BT_filter.TintColor = UIColor.FromRGB(0, 0, 0);
                previousColorFilter = UIColor.FromRGB(0, 0, 0);
                tab_inprogress = false;

                ChangeTab();
            }
        }
        private void ChangeTab()
        {
            isLoadMore = true;
            isFilterServer = false;
            //init data
            LoadStatusCategory(tab_inprogress ? CmmVariable.APPSTATUS_TOME_DANGXULY : CmmVariable.APPSTATUS_TOME_DAXULY);
            LoadDueDateCategory();
            LoadConditionCategory();
            // data
            LoadDuedataToMe();
            LoadConditionDataToMe();
            LoadStatusDataToMe();
            SetDateTime();
            tf_todate.Layer.BorderColor = UIColor.FromRGB(65, 80, 134).CGColor;
            tf_todate.Layer.BorderWidth = 0;

            tf_fromdate.Layer.BorderColor = UIColor.FromRGB(65, 80, 134).CGColor;
            tf_fromdate.Layer.BorderWidth = 0;
            //filter
            LoadDataFilterTodo();

            ToggleTodo();
            HiddenViewStatus();
        }

        private void ToggleTodo()
        {
            if (isFilter)
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
            string str_dangxuly = CmmFunction.GetTitle("TEXT_INPROCESS", "Đang xử lý");

            // set title
            BT_Prosessd.SetTitle(str_daxuly, UIControlState.Normal);//Đã xử lý
            BT_Prosessd.TitleLabel.Text = str_daxuly;

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
            }
            // setting attribuilt
            NSMutableAttributedString att_dxl = new NSMutableAttributedString(str_daxuly);
            NSMutableAttributedString att_dangxl = new NSMutableAttributedString(str_dangxuly);
            if (tab_inprogress)
            {
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
                }
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

        #endregion

        #region private - public method

        /// <summary>
        /// // Phê duyệt yêu cầu
        /// </summary>
        /// <param name="_menuAction">Menu action.</param>
        public void setContent(string _menuAction)
        {
            menuAction = _menuAction;
        }
        public void setContent(string _menuAction, BeanWorkflow _workflow)
        {
            menuAction = _menuAction;
            workflowSelected = _workflow;
        }

        private void ViewConfiguration()
        {
            SetConstraint();
            BT_Prosessd.SetTitle(CmmFunction.GetTitle("TEXT_PROCESSED", "Đã xử lý"), UIControlState.Normal);
            BT_Inprogress.SetTitle(CmmFunction.GetTitle("TEXT_INPROCESS", "Đang xử lý"), UIControlState.Normal);
            view_borderInFilter1.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            view_borderInFilter1.Layer.BorderWidth = 0.6f;
            view_borderInFilter2.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            view_borderInFilter2.Layer.BorderWidth = 0.6f;
            view_borderInFilter3.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            view_borderInFilter3.Layer.BorderWidth = 0.6f;

            //dang su ly
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

            view_custom_CalendarView = new UIView()
            {
                Frame = UIScreen.MainScreen.Bounds,
                BackgroundColor = UIColor.FromRGB(25, 25, 30).ColorWithAlpha(0.9f)
            };
            View.AddSubview(view_custom_CalendarView);
            view_custom_CalendarView.Hidden = true;
            view_custom_CalendarView.Frame = View.Frame;

            height_search = 44;
            constraint_heightSearch.Constant = 0;

            var ver = DeviceHardware.Version;
            var model = DeviceHardware.Model;
            if (model.Contains("X") || model.Contains("11"))
            {
                headerView_constantHeight.Constant = 80;
            }

            var item = UIDevice.CurrentDevice.CheckSystemVersion(13, 0);
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                searchBar.SearchTextField.Frame = new CGRect(searchBar.SearchTextField.Frame.X, 5, searchBar.SearchTextField.Frame.Width, searchBar.SearchTextField.Frame.Height - 10);
                //searchBar.SearchTextField.TintColor = UIColor.FromRGB(245, 245, 245);
                searchBar.SearchTextField.TintColor = UIColor.Black;
                searchBar.SearchTextField.Layer.CornerRadius = 4;

                searchBar.SearchTextField.Font = UIFont.FromName("ArialMT", 16f);
            }
            else
            {
                var textFieldInsideUISearchBar = searchBar.ValueForKey(new NSString("searchField")) as UITextField;
                foreach (var v in searchBar.Subviews)
                {
                    if (v.GetType() == typeof(UITextView))
                    {

                    }
                }
            }

            origin_filterForm_toMe_height = View_filter_Form_ToMe.Frame.Height;

            lbl_title.Text = menuAction;
            string documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string localpath = Path.Combine(documentFolder, CmmVariable.M_Avatar);

            origin_filterForm_toMe_height = View_filter_Form_ToMe.Frame.Height;

            //BT_menu.ImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
            //if (File.Exists(localpath))
            //    BT_menu.SetImage(UIImage.FromFile(localpath), UIControlState.Normal);
            //else
            //    BT_menu.SetImage(UIImage.FromFile("Icons/icon_user20.png"), UIControlState.Normal);

            custom_CalendarView.viewController = this;
            custom_CalendarView.BackgroundColor = UIColor.White;
            custom_CalendarView.Layer.BorderColor = UIColor.FromRGB(65, 80, 134).CGColor;
            custom_CalendarView.Layer.BorderWidth = 1;
            custom_CalendarView.Layer.CornerRadius = 10;
            custom_CalendarView.ClipsToBounds = true;

            buttonsActionBroadBotBarApplication = ButtonsActionBroadBotBarApplication.Instance;
            CGRect bottomBarFrame = new CGRect(bottom_view.Frame.X, bottom_view.Frame.Y, this.View.Frame.Width, bottom_view.Frame.Height);
            buttonsActionBroadBotBarApplication.InitFrameView(bottomBarFrame);
            buttonsActionBroadBotBarApplication.LoadStatusButton(1);
            bottom_view.AddSubview(buttonsActionBroadBotBarApplication);
            CmmIOSFunction.AddShadowForTopORBotBar(bottom_view, false);

            table_content.Frame = new CGRect(0, 60, table_content.Frame.Width, table_content.Frame.Height);
            table_content.ContentInset = new UIEdgeInsets(-5, 0, 0, 0);


            refreshControl = new UIRefreshControl();
            refreshControl.TintColor = UIColor.FromRGB(9, 171, 78);
            var firstAttributes = new UIStringAttributes
            {
                // ForegroundColor = UIColor.White,
                Font = UIFont.FromName("ArialMT", 12f)
            };
            refreshControl.AttributedTitle = new NSAttributedString(CmmFunction.GetTitle("TEXT_REFRESH_LOADING", "Loading ..."), firstAttributes);
            table_content.AddSubview(refreshControl);

            view_loadmore.BackgroundColor = UIColor.FromWhiteAlpha(1, 0.7f);
        }
        private void SetLangTitle()
        {
            searchBar.Placeholder = CmmFunction.GetTitle("TEXT_SEARCH", "Tìm kiếm …");
            custom_CalendarView.calendarView.ReloadData();
            CmmIOSFunction.SetLangToView(this.View);
        }
        private void SetConstraint()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                if (UIApplication.SharedApplication.KeyWindow?.SafeAreaInsets.Bottom > 0)
                {
                    contraint_heightViewNavBot.Constant += 10;
                }
            }
        }
        //private void CreateCircle()
        //{
        //    try
        //    {
        //        double min = Math.Min(BT_menu.ImageView.Frame.Width, BT_menu.ImageView.Frame.Height);
        //        BT_menu.ImageView.Layer.CornerRadius = (float)(min / 2.0);
        //        BT_menu.ImageView.Layer.MasksToBounds = false;
        //        BT_menu.ImageView.Layer.BorderColor = UIColor.Clear.CGColor;
        //        BT_menu.ImageView.Layer.BorderWidth = 0;
        //        BT_menu.ImageView.ClipsToBounds = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("SettingViewController - CreateCircle - Err: " + ex.ToString());
        //    }
        //}
        private void LoadStatusCategory(string key)
        {

            var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
            string defaultIdAppstatus = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS);

            string _whereClause = !String.IsNullOrEmpty(defaultIdAppstatus) ? String.Format(@"WHERE ID IN ({0})", defaultIdAppstatus) : "";
            string query_worflowCategory = string.Format(@"SELECT * FROM BeanAppStatus {0}", _whereClause);
            lst_appStatus_toMe = conn.Query<BeanAppStatus>(query_worflowCategory);

            string appStatusToMe = key;
            string defaultIdAppstatustoMe = CmmFunction.GetAppSettingValue(appStatusToMe);
            string[] arrayStatus = defaultIdAppstatustoMe.Split(",");
            for (int i = 0; i < lst_appStatus_toMe.Count; i++)
            {
                if (arrayStatus.Contains(lst_appStatus_toMe[i].ID.ToString()))
                    lst_appStatus_toMe[i].IsSelected = true;
            }

        }
        private void LoadDueDateCategory()
        {
            lst_dueDateMenu_toMe = new List<ClassMenu>();
            ClassMenu m1 = new ClassMenu() { ID = 1, section = 0, titleEN = "All", title = "Tất cả" };
            ClassMenu m2 = new ClassMenu() { ID = 2, section = 0, titleEN = "Today", title = "Trong ngày" };
            ClassMenu m3 = new ClassMenu() { ID = 3, section = 0, titleEN = "Overdue", title = "Trễ hạn" };

            lst_dueDateMenu_toMe.AddRange(new[] { m1, m2, m3 });
            lst_dueDateMenu_toMe[0].isSelected = true;
        }
        private void LoadConditionCategory()
        {
            lst_conditionMenu_toMe = new List<ClassMenu>();
            ClassMenu m1 = new ClassMenu() { ID = 1, section = 0, titleEN = "In process", title = "Đang xử lý" };
            ClassMenu m2 = new ClassMenu() { ID = 2, section = 0, titleEN = "Processed", title = "Đã xử lý" };

            lst_conditionMenu_toMe.AddRange(new[] { m1, m2 });
            // Đang xử lý 
            if (tab_inprogress)
                lst_conditionMenu_toMe[0].isSelected = true;
            else//Đã xử lý
                lst_conditionMenu_toMe[1].isSelected = true;
        }
        private void LoadDuedataToMe()
        {
            // Tat ca
            DuedateSelected_toMe = lst_dueDateMenu_toMe[0];
        }

        private void LoadConditionDataToMe()
        {
            // Đang xử lý 
            if (tab_inprogress)
                conditionSelected_toMe = lst_conditionMenu_toMe[0];
            else//Đã xử lý
                conditionSelected_toMe = lst_conditionMenu_toMe[1];
        }

        private void LoadStatusDataToMe()
        {
            if (ListAppStatus_selected_toMe != null && ListAppStatus_selected_toMe.Count >= 0)
                ListAppStatus_selected_toMe.Clear();
            else
                ListAppStatus_selected_toMe = new List<BeanAppStatus>();

            ListAppStatus_selected_toMe = lst_appStatus_toMe.FindAll(s => s.IsSelected == true);

        }
        private void SetDateTime()
        {
            temp_fromDateSelectedToMe = DateTime.Now.AddDays(-CmmVariable.M_DataFilterDefaultDays);
            temp_toDateSelectedToMe = DateTime.Now;
        }

        //private bool Check_FiltedStatus(DateTime _fromDateSelected, DateTime _toDateSelected)
        //{

        //    if (custom_AppStatusCategory != null) //custom_ConditionCategory
        //    {
        //        // so sanh cai moi va cai default
        //        string defaultIdAppstatustoMe = "";
        //        if (lst_conditionMenu_toMe[0].isSelected)// tab dang xu ly
        //        {
        //            defaultIdAppstatustoMe = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS_TOME_DANGXULY);
        //        }
        //        else // da xu ly
        //        {
        //            defaultIdAppstatustoMe = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS_TOME_DAXULY);
        //        }
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

        //    if (to != _toDateSelected)
        //        return false;

        //    return true;
        //}

        #region toMe
        //LOAD DATA
        private void loadData_count(int _dataCount)
        {
            try
            {
                if (tab_inprogress)
                {
                    inProcess_toMe_count = _dataCount;
                    string str_dangxuly = CmmFunction.GetTitle("TEXT_INPROCESS", "Đang xử lý");
                    if (inProcess_toMe_count >= 100)
                    {
                        BT_Inprogress.SetTitle(str_dangxuly + " (99+)", UIControlState.Normal);
                    }
                    else if (inProcess_toMe_count > 0 && inProcess_toMe_count < 100)
                    {
                        str_dangxuly = str_dangxuly + " (" + inProcess_toMe_count.ToString() + ")";
                        BT_Inprogress.SetTitle(str_dangxuly, UIControlState.Normal);
                    }
                    else
                    {
                        BT_Inprogress.SetTitle(str_dangxuly, UIControlState.Normal);
                    }
                }
                else
                {
                    Process_toMe_count = _dataCount;
                    BT_Prosessd.SetTitle(CmmFunction.GetTitle("TEXT_PROCESSED", "Đã xử lý"), UIControlState.Normal);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MainView - loadData - Err: " + ex.ToString());
            }
        }
        /// <summary>
        /// filter different default then get from server
        /// </summary>
        private void FilterServer(bool isLoadMoreDataFilter)
        {
            try
            {
                dict_todo = new Dictionary<string, List<BeanAppBaseExt>>();
                if (!isLoadMoreDataFilter)
                    lst_appBase_cxl = new List<BeanAppBaseExt>();

                //Check filter DueDate
                string str_status = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS); // tat ca fromme
                if (ListAppStatus_selected_toMe != null && ListAppStatus_selected_toMe.Count > 0) // co chon status
                    str_status = string.Join(',', ListAppStatus_selected_toMe.Select(i => i.ID));

                ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();
                int totalRecord = 0;
                var listPropertiesFilter = CmmFunction.BuildListPropertiesFilter(CmmVariable.M_ResourceViewID_ToMe, tab_inprogress ? "2" : "4", str_status, DuedateSelected_toMe.ID, temp_fromDateSelectedToMe, temp_toDateSelectedToMe, workflowSelected.WorkflowID.ToString());

                //loadmore then cu filter
                if (isLoadMoreDataFilter)
                {
                    List<BeanAppBaseExt> lst_appBase_cxl_more = new List<BeanAppBaseExt>();
                    lst_appBase_cxl_more = _pControlDynamic.GetListFilterMyTask(listPropertiesFilter, ref totalRecord, CmmVariable.M_DataFilterAPILimitData, lst_appBase_cxl.Count);// lst_appBase_fromMe.Count
                    lst_appBase_cxl_more = lst_appBase_cxl_more.OrderByDescending(s => s.NotifyCreated).ToList();

                    if (lst_appBase_cxl_more != null && lst_appBase_cxl_more.Count > 0)
                    {
                        //sort
                        if (tab_inprogress)
                            lst_appBase_cxl_more = lst_appBase_cxl_more.OrderByDescending(s => s.NotifyCreated).ToList();
                        else
                            lst_appBase_cxl_more = lst_appBase_cxl_more.OrderByDescending(s => s.Modified).ToList();

                        lst_appBase_cxl.AddRange(lst_appBase_cxl_more);

                        if (lst_appBase_cxl_more.Count < CmmVariable.M_DataFilterAPILimitData)
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
                    lst_appBase_cxl = _pControlDynamic.GetListFilterMyTask(listPropertiesFilter, ref totalRecord, CmmVariable.M_DataFilterAPILimitData, 0);// lst_appBase_fromMe.Count

                    if (tab_inprogress)
                        lst_appBase_cxl = lst_appBase_cxl.OrderByDescending(s => s.NotifyCreated).ToList();
                    else
                        lst_appBase_cxl = lst_appBase_cxl.OrderByDescending(s => s.Modified).ToList();
                }

                if (lst_appBase_cxl != null && lst_appBase_cxl.Count > 0)
                {
                    if (tab_inprogress)
                        inProcess_toMe_count = totalRecord;

                    SortListAppBase("", true);
                }
                else
                {
                    if (tab_inprogress)
                        inProcess_toMe_count = 0;
                    table_content.Alpha = 0;
                    lbl_nodata.Hidden = false;
                }
            }
            catch (Exception ex)
            {
                if (tab_inprogress)
                    inProcess_toMe_count = 0;
                table_content.Source = null;
                table_content.ReloadData();
                table_content.Alpha = 0;
                lbl_nodata.Hidden = false;

                Console.WriteLine("MainView - LoadDataFilterTodo - Err: " + ex.ToString());
            }

        }
        /// <summary>
        /// get data when in local
        /// </summary>
        private void LoadDataFilterTodo()
        {
            try
            {
                var conn = new SQLiteConnection(CmmVariable.M_DataPath);
                string query = string.Empty;
                string queryCount = string.Empty;
                List<CountNum> lst_count_appBase_cxl = new List<CountNum>();

                dict_todo = new Dictionary<string, List<BeanAppBaseExt>>();
                lst_appBase_cxl = new List<BeanAppBaseExt>();

                //getcount
                queryCount = CreateQueryToDo(true);
                lst_count_appBase_cxl = conn.Query<CountNum>(queryCount);
                if (lst_count_appBase_cxl != null && lst_count_appBase_cxl.Count > 0)
                {
                    if (tab_inprogress)
                        inProcess_toMe_count = lst_count_appBase_cxl.First().count;
                }
                else
                {
                    inProcess_toMe_count = 0;
                }
                //data
                query = CreateQueryToDo(false);
                lst_appBase_cxl = conn.Query<BeanAppBaseExt>(query, CmmVariable.M_DataLimitRow, 0);
                if (lst_appBase_cxl != null && lst_appBase_cxl.Count > 0)
                {
                    SortListAppBase(query, false);
                }
                else
                {
                    table_content.Alpha = 0;
                    lbl_nodata.Hidden = false;
                }
            }
            catch (Exception ex)
            {
                inProcess_toMe_count = 0;
                table_content.Source = null;
                table_content.ReloadData();

                table_content.Alpha = 0;
                lbl_nodata.Hidden = false;

                Console.WriteLine("MainView - LoadDataFilterTodo - Err: " + ex.ToString());
            }
        }
        private void LoadMoreDataTodo()
        {
            try
            {
                List<BeanAppBaseExt> lst_todo_more = new List<BeanAppBaseExt>();
                var conn = new SQLiteConnection(CmmVariable.M_DataPath);
                string query = string.Empty;

                dict_todo = new Dictionary<string, List<BeanAppBaseExt>>();

                //DATA
                query = CreateQueryToDo(false);
                lst_todo_more = conn.Query<BeanAppBaseExt>(query, CmmVariable.M_DataLimitRow, lst_appBase_cxl.Count);

                if (lst_todo_more != null && lst_todo_more.Count > 0)
                {
                    lst_appBase_cxl.AddRange(lst_todo_more);
                    SortListAppBase("", false);
                    if (lst_todo_more.Count < CmmVariable.M_DataLimitRow)
                        isLoadMore = false;
                }
                else
                {
                    isLoadMore = false;
                }
            }
            catch (Exception ex)
            {
                //inProcess_toMe_count = 0;
                table_content.Source = null;
                table_content.ReloadData();

                table_content.Alpha = 0;
                lbl_nodata.Hidden = false;

                Console.WriteLine("MainView - LoadDataFilterTodo - Err: " + ex.ToString());
            }
        }

        private string CreateQueryToDo(bool isGetCount)
        {
            string query = string.Empty;
            //Check filter DueDate
            //string duedate_condition = "";
            //if (DuedateSelected_toMe == null || DuedateSelected_toMe.ID == 1) // Tat ca
            //    duedate_condition = "";
            //else if (DuedateSelected_toMe.ID == 2) // Trong ngay
            //    duedate_condition = @"AND (AB.DueDate IS NOT NULL AND date(AB.DueDate) = date('now'))";
            //else if (DuedateSelected_toMe.ID == 3) // Tre han
            //    duedate_condition = @"AND (AB.DueDate IS NOT NULL AND AB.DueDate < date('now'))";



            string str_status = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS); // tat ca fromme
            if (ListAppStatus_selected_toMe != null && ListAppStatus_selected_toMe.Count > 0) // co chon status
                str_status = string.Join(',', ListAppStatus_selected_toMe.Select(i => i.ID));

            if (!string.IsNullOrEmpty(str_status))
                str_status = string.Format(@" AND AB.StatusGroup IN({0})", str_status);

           

            //Check switch status
            string query_inprogress = "";
            if (tab_inprogress) // neu DANG xu ly
            {
                query_inprogress = "NOTI.Type = 1 AND NOTI.Status = 0";
                string query_workflow = "";
                query_workflow = string.Format(@" AND  AB.WorkflowID = '{0}' ", +workflowSelected.WorkflowID);


                if (isGetCount) //count
                    query = string.Format(@"SELECT Count(*) as count
                                            FROM BeanAppBase AB
                                            INNER JOIN BeanNotify NOTI
                                               ON AB.ID = NOTI.SPItemId
                                            WHERE {0} {1} AND AB.AssignedTo LIKE '%{2}%' {3}",
                        query_inprogress, query_workflow,  CmmVariable.SysConfig.UserId.ToUpper(), str_status);
                else //data
                    query = string.Format(@"SELECT AB.*, NOTI.StartDate, NOTI.Read, NOTI.SPItemId, NOTI.SubmitAction, NOTI.SendUnit
                                            FROM BeanAppBase AB
                                            INNER JOIN BeanNotify NOTI
                                               ON AB.ID = NOTI.SPItemId
                                            WHERE {0} {1} AND AB.AssignedTo LIKE '%{2}%' {3} 
                                            Order By NOTI.StartDate DESC LIMIT ? OFFSET ?",
             query_inprogress, query_workflow, CmmVariable.SysConfig.UserId.ToUpper(), str_status);

            }
            else //da xu ly - khong join voi notification 
            {
                string query_workflow = "";
                query_workflow = string.Format(@" AB.WorkflowID = '{0}'", + workflowSelected.WorkflowID);

                if (isGetCount)
                    query = string.Format(@"SELECT Count(*) as count
                                        FROM BeanAppBase AB
                                        WHERE {0} AND AB.StatusGroup <> 256
                                              AND AB.NotifiedUsers LIKE '%{1}%'
                                              AND (AB.ResourceCategoryId <> 16 OR AB.CreatedBy <> '%{1}%') {2}",
                                      query_workflow, CmmVariable.SysConfig.UserId.ToLower(), str_status);
                else
                    query = string.Format(@"SELECT AB.*
                                        FROM BeanAppBase AB
                                        WHERE {0} AND AB.StatusGroup <> 256
                                              AND AB.NotifiedUsers LIKE '%{1}%'
                                              AND (AB.ResourceCategoryId <> 16 OR AB.CreatedBy <> '%{1}%') {2}
                                   Order By AB.Modified DESC LIMIT ? OFFSET ?",
                                       query_workflow, CmmVariable.SysConfig.UserId.ToLower(), str_status);
            }
            return query;
        }
        /// <summary>
        /// sort secsion from create
        /// </summary>
        /// <param name="query"></param>
        private void SortListAppBase(string query, bool isOnline)
        {
            string KEY_TODAY = "TEXT_TODAY`Hôm nay";
            string KEY_YESTERDAY = "TEXT_YESTERDAY`Hôm qua";
            string KEY_ORTHER = "TEXT_OLDER`Cũ hơn";


            List<BeanAppBaseExt> lst_temp1 = new List<BeanAppBaseExt>();
            List<BeanAppBaseExt> lst_temp2 = new List<BeanAppBaseExt>();
            List<BeanAppBaseExt> lst_temp3 = new List<BeanAppBaseExt>();
            dict_todo.Add(KEY_TODAY, lst_temp1);
            dict_todo.Add(KEY_YESTERDAY, lst_temp2);
            dict_todo.Add(KEY_ORTHER, lst_temp3);

            foreach (var item in lst_appBase_cxl)
            {
                if (tab_inprogress)
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
                    else //local sort theo StartDate
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
                else
                { // server hay local thi cung sort theo NotifyCreated
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
            }

            table_content.Alpha = 1;
            lbl_nodata.Hidden = true;

            table_content.Source = new Todo_TableSource(dict_todo, this, query);
            table_content.ReloadData();
        }
        public void ToggleCalendar_toMe(bool _show)
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
        //private void ToggleStatus_toMe()
        //{
        //    //tat ca
        //    if (status_selected_index == 0)
        //    {
        //        //tat ca
        //        BT_status_all.BackgroundColor = UIColor.FromRGB(65, 80, 134);
        //        BT_status_all.SetTitleColor(UIColor.White, UIControlState.Normal);
        //        //can xu ly
        //        BT_status_todo.BackgroundColor = UIColor.FromRGB(245, 245, 245);
        //        BT_status_todo.SetTitleColor(UIColor.DarkGray, UIControlState.Normal);
        //        //da xu ly
        //        BT_status_completed.BackgroundColor = UIColor.FromRGB(245, 245, 245);
        //        BT_status_completed.SetTitleColor(UIColor.DarkGray, UIControlState.Normal);
        //    }
        //    //can xu ly
        //    else if (status_selected_index == 1)
        //    {
        //        //tat ca
        //        BT_status_all.BackgroundColor = UIColor.FromRGB(245, 245, 245);
        //        BT_status_all.SetTitleColor(UIColor.DarkGray, UIControlState.Normal);
        //        //can xu ly
        //        BT_status_todo.BackgroundColor = UIColor.FromRGB(65, 80, 134);
        //        BT_status_todo.SetTitleColor(UIColor.White, UIControlState.Normal);
        //        //da xu ly
        //        BT_status_completed.BackgroundColor = UIColor.FromRGB(245, 245, 245);
        //        BT_status_completed.SetTitleColor(UIColor.DarkGray, UIControlState.Normal);
        //    }
        //    //da xu ly
        //    else if (status_selected_index == 2)
        //    {
        //        //tat ca
        //        BT_status_all.BackgroundColor = UIColor.FromRGB(245, 245, 245);
        //        BT_status_all.SetTitleColor(UIColor.DarkGray, UIControlState.Normal);
        //        //can xu ly
        //        BT_status_todo.BackgroundColor = UIColor.FromRGB(245, 245, 245);
        //        BT_status_todo.SetTitleColor(UIColor.DarkGray, UIControlState.Normal);
        //        //da xu ly
        //        BT_status_completed.BackgroundColor = UIColor.FromRGB(65, 80, 134);
        //        BT_status_completed.SetTitleColor(UIColor.White, UIControlState.Normal);
        //    }
        //}
        //private void ToggleDuedate()
        //{
        //    //tat ca
        //    if (duedate_selected_index == 0)
        //    {
        //        //tat ca
        //        BT_duedate_all.BackgroundColor = UIColor.FromRGB(65, 80, 134);
        //        BT_duedate_all.SetTitleColor(UIColor.White, UIControlState.Normal);
        //        //qua han
        //        BT_duedate_overdue.BackgroundColor = UIColor.FromRGB(245, 245, 245);
        //        BT_duedate_overdue.SetTitleColor(UIColor.DarkGray, UIControlState.Normal);
        //        //trong han
        //        BT_duedate_ontime.BackgroundColor = UIColor.FromRGB(245, 245, 245);
        //        BT_duedate_ontime.SetTitleColor(UIColor.DarkGray, UIControlState.Normal);
        //    }
        //    //qua han
        //    else if (duedate_selected_index == 1)
        //    {
        //        //tat ca
        //        BT_duedate_all.BackgroundColor = UIColor.FromRGB(245, 245, 245);
        //        BT_duedate_all.SetTitleColor(UIColor.DarkGray, UIControlState.Normal);
        //        //qua han
        //        BT_duedate_overdue.BackgroundColor = UIColor.FromRGB(65, 80, 134);
        //        BT_duedate_overdue.SetTitleColor(UIColor.White, UIControlState.Normal);
        //        //trong han
        //        BT_duedate_ontime.BackgroundColor = UIColor.FromRGB(245, 245, 245);
        //        BT_duedate_ontime.SetTitleColor(UIColor.DarkGray, UIControlState.Normal);
        //    }
        //    //trong han
        //    else if (duedate_selected_index == 2)
        //    {
        //        //tat ca
        //        BT_duedate_all.BackgroundColor = UIColor.FromRGB(245, 245, 245);
        //        BT_duedate_all.SetTitleColor(UIColor.DarkGray, UIControlState.Normal);
        //        //qua han
        //        BT_duedate_overdue.BackgroundColor = UIColor.FromRGB(245, 245, 245);
        //        BT_duedate_overdue.SetTitleColor(UIColor.DarkGray, UIControlState.Normal);
        //        //trong han
        //        BT_duedate_ontime.BackgroundColor = UIColor.FromRGB(65, 80, 134);
        //        BT_duedate_ontime.SetTitleColor(UIColor.White, UIControlState.Normal);
        //    }
        //}
        //public void ToggleCalendar_toMe(bool _show)
        //{
        //    if (!_show)
        //    {
        //        BT_Apply_ToMe.Hidden = false;

        //        View_filter_Form_ToMe.Frame = new CGRect(0, 0, filter_view.Frame.Width, filter_view.Frame.Height);
        //        UIView.BeginAnimations("show_animationShowCalendar");
        //        UIView.SetAnimationDuration(0.2f);
        //        UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
        //        UIView.SetAnimationRepeatCount(0);
        //        UIView.SetAnimationRepeatAutoreverses(false);
        //        UIView.SetAnimationDelegate(this);
        //        View_filter_Form_ToMe.Frame = new CGRect(0, 0, filter_view.Frame.Width, origin_filterForm_toMe_height);
        //        //constrainHeight_filterTome_Form.Constant = origin_filterForm_toMe_height;
        //        UIView.CommitAnimations();

        //        custom_CalendarView.RemoveFromSuperview();
        //    }
        //    else
        //    {
        //        BT_Apply_ToMe.Hidden = true;
        //        View_filter_Form_ToMe.Frame = new CGRect(0, 0, filter_view.Frame.Width, View_filter_Form_ToMe.Frame.Height);

        //        UIView.BeginAnimations("show_animationHideCalendar");
        //        UIView.SetAnimationDuration(0.2f);
        //        UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
        //        UIView.SetAnimationRepeatCount(0);
        //        UIView.SetAnimationRepeatAutoreverses(false);
        //        UIView.SetAnimationDelegate(this);
        //        View_filter_Form_ToMe.Frame = new CGRect(0, 0, filter_view.Frame.Width, filter_view.Frame.Height);
        //        //constrainHeight_filterTome_Form.Constant = filter_view.Frame.Height;
        //        UIView.CommitAnimations();

        //        var Y_positionCalendar = view_fromdate.Frame.Bottom;
        //        custom_CalendarView.InitFrameView(new CGRect(BT_fromdate.Frame.X, Y_positionCalendar, View_filter_Form_ToMe.Frame.Width - (BT_fromdate.Frame.X * 2), filter_view.Frame.Height - Y_positionCalendar - 10));
        //    }
        //}
        #endregion

        private void ToggleFilter()
        {
            if (filter_view.Alpha == 0)
            {
                isFilter = true;
                View_filter_Form_ToMe.Frame = new CGRect(0, 0, filter_view.Frame.Width, 0);
                UIView.BeginAnimations("show_animationShowFilter");
                UIView.SetAnimationDuration(0.2f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);
                filter_view.Alpha = 1;
                View_filter_Form_ToMe.Frame = new CGRect(0, 0, filter_view.Frame.Width, 482);
                View_filter_Form_ToMe.Alpha = 1;
                UIView.CommitAnimations();

                tf_fromdate.TextColor = UIColor.FromRGB(0, 0, 0);
                tf_todate.TextColor = UIColor.FromRGB(0, 0, 0);
                BT_filter.TintColor = UIColor.FromRGB(35, 151, 32); // moi lan mo len thi bat filter mau xanh
                SetValueFormFilter();
            }
            else
            {
                isFilter = false;
                View_filter_Form_ToMe.Frame = new CGRect(0, 0, filter_view.Frame.Width, 482);
                UIView.BeginAnimations("show_animationHideFilter");
                UIView.SetAnimationDuration(0.2f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);
                View_filter_Form_ToMe.Frame = new CGRect(0, 0, filter_view.Frame.Width, 0);
                filter_view.Alpha = 0;
                UIView.CommitAnimations();
                BT_filter.TintColor = previousColorFilter;
            }

        }
        public void CloseCalendarInstance()
        {
            Custom_CalendarView custom_CalendarView = Custom_CalendarView.Instance;
            if (custom_CalendarView.Superview != null)
            {
                custom_CalendarView.RemoveFromSuperview();
                //constraintHeightRoot.Constant = view_fromDate.Frame.Bottom + 20;
            }
        }

        private void NavigateToDetails(BeanAppBaseExt beanAppBase)
        {
            this.View.EndEditing(true);

            if (beanAppBase.ResourceCategoryId == 16) //task
            {
                SQLiteAsyncConnection conn = new SQLiteAsyncConnection(CmmVariable.M_DataPath, false);
                string _workflowItemID = CmmFunction.GetWorkflowItemIDByUrl(beanAppBase.ItemUrl);
                string _query = string.Format("SELECT * FROM BeanWorkflowItem WHERE ID = '{0}' LIMIT 1 OFFSET 0", _workflowItemID);
                List<BeanWorkflowItem> workflowItem = conn.QueryAsync<BeanWorkflowItem>(_query).Result;


                FormTaskDetails taskDetails = (FormTaskDetails)Storyboard.InstantiateViewController("FormTaskDetails");
                taskDetails.SetContent(beanAppBase, workflowItem[0], this);
                this.NavigationController.PushViewController(taskDetails, true);
            }
            else //chi tiet phieu{
            {
                RequestDetailsV2 v2 = (RequestDetailsV2)Storyboard.InstantiateViewController("RequestDetailsV2");
                v2.setContent(beanAppBase);
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
        private void BT_back_TouchUpInside(object sender, EventArgs e)
        {
            //if (isSearch)
            //{
            //    SearchToggle();
            //}
            //if (isFilter)
            //    ToggleFilter();

            //AppDelegate appD = UIApplication.SharedApplication.Delegate as AppDelegate;
            //appD.menu.loadData_count();
            //appD.menu.UpdateItemSelect(1);
            //appD.SlideMenuController.OpenLeft();
            buttonsActionBroadBotBarApplication.broadView.BackFromeKanBanView();
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

            HiddenViewStatus();
            ///current Border date
            if (temp_toDateSelectedToMe != default(DateTime))
            {
                if (CmmVariable.SysConfig.LangCode == "1066")
                    tf_todate.Text = temp_toDateSelectedToMe.ToString("dd/MM/yyyy");
                else
                    tf_todate.Text = temp_toDateSelectedToMe.ToString("MM/dd/yyyy");
            }
            else
                tf_todate.Text = "";

            if (temp_fromDateSelectedToMe != default(DateTime))
            {
                if (CmmVariable.SysConfig.LangCode == "1066")
                    tf_fromdate.Text = temp_fromDateSelectedToMe.ToString("dd/MM/yyyy");
                else
                    tf_fromdate.Text = temp_fromDateSelectedToMe.ToString("MM/dd/yyyy");
            }
            else
                tf_fromdate.Text = "";

            //current status
            if (ListAppStatus_selected_toMe.Count == 1)
            {
                if (CmmVariable.SysConfig.LangCode == "1066")
                    tf_lbl_status.Text = ListAppStatus_selected_toMe[0].Title;
                else
                    tf_lbl_status.Text = ListAppStatus_selected_toMe[0].TitleEN;
            }
            else if (ListAppStatus_selected_toMe.Count > 1 && ListAppStatus_selected_toMe.Count <= lst_appStatus_toMe.Count - 1)
            {
                if (CmmVariable.SysConfig.LangCode == "1066")
                    tf_lbl_status.Text = ListAppStatus_selected_toMe[0].Title;
                else
                    tf_lbl_status.Text = ListAppStatus_selected_toMe[0].TitleEN;

                tf_lbl_status.Text += ",+(" + (ListAppStatus_selected_toMe.Count - 1).ToString() + ")";
            }
            else //tat ca
            {
                tf_lbl_status.Text = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
            }

            //current duedate
            if (CmmVariable.SysConfig.LangCode == "1066")
                lbl_duedate.Text = DuedateSelected_toMe.title;
            else
                lbl_duedate.Text = DuedateSelected_toMe.titleEN;
            //curent condition
            if (CmmVariable.SysConfig.LangCode == "1066")
                lbl_State.Text = conditionSelected_toMe.title;
            else
                lbl_State.Text = conditionSelected_toMe.titleEN;

            HiddenViewStatus();

            //setting current select of Custom_AppStatusCategory
            if (custom_AppStatusCategory != null)
            {
                lst_appStatus_toMe = lst_appStatus_toMe.Select(a => { a.IsSelected = false; return a; }).ToList();
                foreach (var itemselect in ListAppStatus_selected_toMe)
                {
                    foreach (var itemmenu in lst_appStatus_toMe)
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
                lst_dueDateMenu_toMe = lst_dueDateMenu_toMe.Select(a => { a.isSelected = false; return a; }).ToList();
                foreach (var itemmenu in lst_dueDateMenu_toMe)
                {
                    if (itemmenu.ID == DuedateSelected_toMe.ID)
                    {
                        itemmenu.isSelected = true;
                        break;
                    }
                }
            }

            //setting current select of Custom_DuedateCategory
            if (custom_ConditionCategory != null)
            {
                lst_conditionMenu_toMe = lst_conditionMenu_toMe.Select(a => { a.isSelected = false; return a; }).ToList();
                foreach (var itemmenu in lst_conditionMenu_toMe)
                {
                    if (itemmenu.ID == conditionSelected_toMe.ID)
                    {
                        itemmenu.isSelected = true;
                        break;
                    }
                }
            }
            //truyen du lieu cho form custom vi co 2 trang thai dang xu ly va da xu ly dug chung mot custom_AppStatusCategory.instance
            if (custom_AppStatusCategory != null)
            {
                custom_AppStatusCategory.ListAppStatus = lst_appStatus_toMe;
                custom_AppStatusCategory.ListAppStatus_selected = ListAppStatus_selected_toMe;
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
            LoadStatusCategory(tab_inprogress ? CmmVariable.APPSTATUS_TOME_DANGXULY : CmmVariable.APPSTATUS_TOME_DAXULY);
            LoadDueDateCategory();
            LoadConditionCategory();
            // data
            LoadDuedataToMe();
            LoadConditionDataToMe();
            LoadStatusDataToMe();
            SetDateTime();
            //filter
            LoadDataFilterTodo();
            ButtonMenuStyleChange();

            //truyen du lieu cho form custom
            if (custom_AppStatusCategory != null)
            {
                custom_AppStatusCategory.ListAppStatus = lst_appStatus_toMe;
                custom_AppStatusCategory.ListAppStatus_selected = ListAppStatus_selected_toMe;
            }
            if (custom_DuedateCategory != null)
            {
                custom_DuedateCategory.ListClassMenu = lst_dueDateMenu_toMe;
                custom_DuedateCategory.ClassMenu_selected = DuedateSelected_toMe;
            }
            if (custom_ConditionCategory != null)
            {
                custom_ConditionCategory.ListClassMenu = lst_conditionMenu_toMe;
                custom_ConditionCategory.ClassMenu_selected = conditionSelected_toMe;
            }
            //curent condition

            ResetSetValueFormFilter(lst_appStatus_toMe.FindAll(s => s.IsSelected == true), lst_dueDateMenu_toMe.Find(s => s.isSelected == true), lst_conditionMenu_toMe.Find(s => s.isSelected == true));
            HiddenViewStatus();
            ToggleFilter();
        }
        private void ResetSetValueFormFilter(List<BeanAppStatus> selectStatus, ClassMenu selectDuedate, ClassMenu condition)
        {
            if (CmmVariable.SysConfig.LangCode == "1066")
                tf_fromdate.Text = DateTime.Now.AddDays(-CmmVariable.M_DataFilterDefaultDays).ToString(@"dd/MM/yyyy", new CultureInfo("vi"));
            else if (CmmVariable.SysConfig.LangCode == "1033")
                tf_fromdate.Text = DateTime.Now.AddDays(-CmmVariable.M_DataFilterDefaultDays).ToString(@"MM/dd/yyyy", new CultureInfo("en"));

            if (CmmVariable.SysConfig.LangCode == "1066")
                tf_todate.Text = DateTime.Now.ToString(@"dd/MM/yyyy", new CultureInfo("vi"));
            else if (CmmVariable.SysConfig.LangCode == "1033")
                tf_todate.Text = DateTime.Now.ToString(@"MM/dd/yyyy", new CultureInfo("en"));

            tf_fromdate.Layer.BorderColor = UIColor.FromRGB(65, 80, 134).CGColor;
            tf_fromdate.Layer.BorderWidth = 0;

            tf_todate.Layer.BorderColor = UIColor.FromRGB(65, 80, 134).CGColor;
            tf_todate.Layer.BorderWidth = 0;
            //current status
            if (selectStatus != null)
            {
                if (selectStatus.Count == 1)
                {
                    if (CmmVariable.SysConfig.LangCode == "1066")
                        tf_lbl_status.Text = selectStatus[0].Title;
                    else
                        tf_lbl_status.Text = selectStatus[0].TitleEN;
                }
                else if (selectStatus.Count > 1 && selectStatus.Count <= lst_appStatus_toMe.Count - 1)
                {
                    if (CmmVariable.SysConfig.LangCode == "1066")
                        tf_lbl_status.Text = selectStatus[0].Title;
                    else
                        tf_lbl_status.Text = selectStatus[0].TitleEN;

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
                if (CmmVariable.SysConfig.LangCode == "1066")
                    lbl_duedate.Text = selectDuedate.title;
                else
                    lbl_duedate.Text = selectDuedate.titleEN;
            }

            if (condition != null)
            {
                if (CmmVariable.SysConfig.LangCode == "1066")
                    lbl_State.Text = condition.title;
                else
                    lbl_State.Text = condition.titleEN;
            }
        }
        private void CmmEvent_UpdateLangComplete(object sender, CmmEvent.UpdateEventArgs e)
        {
            try
            {
                InvokeOnMainThread(() =>
                {
                    SetLangTitle();
                    ButtonMenuStyleChange();
                    table_content.ReloadData();
                    refreshControl.AttributedTitle = new NSAttributedString(CmmFunction.GetTitle("TEXT_REFRESH_LOADING", "Loading ..."));
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("RequestListViewApp - CmmEvent_UpdateLangComplete - ERR: " + ex.ToString());
            }
        }

        #region todo toMe
        private void BT_duedate_TouchUpInside(object sender, EventArgs e)
        {
            ToggleMenuDueDate();
        }

        private void BT_status_TouchUpInside(object sender, EventArgs e)
        {
            ToggleMenuStatus();
        }
        private void BT_State_TouchUpInside(object sender, EventArgs e)
        {
            ToggleMenuCondition();
        }


        private void ToggleMenuStatus()
        {
            custom_AppStatusCategory = Custom_AppStatusCategory.Instance;
            custom_AppStatusCategory.ItemNoIcon = false;
            custom_AppStatusCategory.viewController = this;
            custom_AppStatusCategory.CheckAll = true;
            custom_AppStatusCategory.LBL_inputView = tf_lbl_status;
            custom_AppStatusCategory.ListAppStatus = lst_appStatus_toMe;
            custom_AppStatusCategory.ListAppStatus_selected = ListAppStatus_selected_toMe;
            custom_AppStatusCategory.TableLoadData();
            custom_AppStatusCategory.BackupData();

            MultiChoiceTableView multiChoiceTableView = Storyboard.InstantiateViewController("MultiChoiceTableView") as MultiChoiceTableView;
            multiChoiceTableView.setContent(this, true, custom_AppStatusCategory, CmmFunction.GetTitle("TEXT_STATUS", "Tình trạng"));
            this.NavigationController.PushViewController(multiChoiceTableView, true);
        }
        private void ToggleMenuDueDate()
        {
            custom_DuedateCategory = Custom_DuedateCategory.Instance;
            custom_DuedateCategory.ItemNoIcon = false;
            custom_DuedateCategory.viewController = this;
            custom_DuedateCategory.LBL_inputView = lbl_duedate;
            custom_DuedateCategory.ClassMenu_selected = DuedateSelected_toMe;
            custom_DuedateCategory.ListClassMenu = lst_dueDateMenu_toMe;
            custom_DuedateCategory.TableLoadData();
            custom_DuedateCategory.BackupData();

            MultiChoiceTableView multiChoiceTableView = Storyboard.InstantiateViewController("MultiChoiceTableView") as MultiChoiceTableView;
            multiChoiceTableView.setContent(this, false, custom_DuedateCategory, CmmFunction.GetTitle("TEXT_DUEDATE", "Hạn hoàn thành"));
            this.NavigationController.PushViewController(multiChoiceTableView, true);
        }

        private void ToggleMenuCondition()
        {
            custom_ConditionCategory = Custom_AppConditionCategory.Instance;
            custom_ConditionCategory.ItemNoIcon = false;
            custom_ConditionCategory.viewController = this;
            custom_ConditionCategory.LBL_inputView = lbl_State;
            custom_ConditionCategory.ClassMenu_selected = conditionSelected_toMe;
            custom_ConditionCategory.ListClassMenu = lst_conditionMenu_toMe;
            custom_ConditionCategory.TableLoadData();
            custom_ConditionCategory.BackupData();

            MultiChoiceTableView multiChoiceTableView = Storyboard.InstantiateViewController("MultiChoiceTableView") as MultiChoiceTableView;
            multiChoiceTableView.setContent(this, false, custom_ConditionCategory, CmmFunction.GetTitle("TEXT_STATE", "Trạng thái"));
            this.NavigationController.PushViewController(multiChoiceTableView, true);
        }

        public void CloseConditionInstance()
        {
            Custom_AppConditionCategory custom_AppCondition = Custom_AppConditionCategory.Instance;
            if (custom_AppCondition.Superview != null)
                custom_AppCondition.RemoveFromSuperview();
        }

        private void HiddenViewStatus()
        {
            if (lst_conditionMenu_toMe[0].isSelected)
                constraint_heighViewStatus.Constant = 0;
            else
                constraint_heighViewStatus.Constant = 46;
        }
        public void ClearViewStatus()
        {
            HiddenViewStatus();
            LoadStatusCategory(lst_conditionMenu_toMe[0].isSelected ? CmmVariable.APPSTATUS_TOME_DANGXULY : CmmVariable.APPSTATUS_TOME_DAXULY);
            LoadDueDateCategory();
            if ((tab_inprogress && lst_conditionMenu_toMe[0].isSelected) || (!tab_inprogress && lst_conditionMenu_toMe[1].isSelected)) //neu dang tro ve man hinh den toi thi lay old
            {
                //cap nhat select ListAppStatus_selected_toMe
                for (int i = 0; i < lst_appStatus_toMe.Count; i++)
                {
                    if (ListAppStatus_selected_toMe.Find(s => s.ID == lst_appStatus_toMe[i].ID) != null)
                        lst_appStatus_toMe[i].IsSelected = true;
                }
                //cap nhat select ListAppStatus_selected_toMe
                for (int i = 0; i < lst_dueDateMenu_toMe.Count; i++)
                {
                    if (DuedateSelected_toMe.ID == lst_dueDateMenu_toMe[i].ID)
                        lst_appStatus_toMe[i].IsSelected = true;
                }
                //truyen du lieu cho form custom
                if (custom_AppStatusCategory != null)
                {
                    custom_AppStatusCategory.ListAppStatus = lst_appStatus_toMe;
                    custom_AppStatusCategory.ListAppStatus_selected = ListAppStatus_selected_toMe;
                }
                //truyen du lieu cho form custom
                if (custom_DuedateCategory != null)
                {
                    custom_DuedateCategory.ListClassMenu = lst_dueDateMenu_toMe;
                    custom_DuedateCategory.ClassMenu_selected = DuedateSelected_toMe;
                }
                //danh lable tf_lbl_status
                if (ListAppStatus_selected_toMe.Count == 1)
                {
                    if (CmmVariable.SysConfig.LangCode == "1066")
                        tf_lbl_status.Text = ListAppStatus_selected_toMe[0].Title;
                    else
                        tf_lbl_status.Text = ListAppStatus_selected_toMe[0].TitleEN;
                }
                else if (ListAppStatus_selected_toMe.Count > 1 && ListAppStatus_selected_toMe.Count <= lst_appStatus_toMe.Count - 1)
                {
                    if (CmmVariable.SysConfig.LangCode == "1066")
                        tf_lbl_status.Text = ListAppStatus_selected_toMe[0].Title;
                    else
                        tf_lbl_status.Text = ListAppStatus_selected_toMe[0].TitleEN;

                    tf_lbl_status.Text += ",+(" + (ListAppStatus_selected_toMe.Count - 1).ToString() + ")";
                }
                else
                {
                    tf_lbl_status.Text = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
                }
                // danh lai lable duedate
                if (CmmVariable.SysConfig.LangCode == "1066")
                    lbl_duedate.Text = DuedateSelected_toMe.title;
                else
                    lbl_duedate.Text = DuedateSelected_toMe.titleEN;

            }
            else // get new
            {
                //cap nhat select ListAppStatus_selected_toMe
                //cap nhat select lst_appStatus_toMe

                string appStatusToMe = lst_conditionMenu_toMe[0].isSelected ? CmmVariable.APPSTATUS_TOME_DANGXULY : CmmVariable.APPSTATUS_TOME_DAXULY;
                string defaultIdAppstatustoMe = CmmFunction.GetAppSettingValue(appStatusToMe);
                string[] arrayStatus = defaultIdAppstatustoMe.Split(",");
                for (int i = 0; i < lst_appStatus_toMe.Count; i++)
                {
                    if (arrayStatus.Contains(lst_appStatus_toMe[i].ID.ToString()))
                        lst_appStatus_toMe[i].IsSelected = true;
                }

                var selectedStatus = lst_appStatus_toMe.FindAll(s => s.IsSelected == true);
                //truyen du lieu cho form custom
                if (custom_AppStatusCategory != null)
                {
                    custom_AppStatusCategory.ListAppStatus = lst_appStatus_toMe;
                    custom_AppStatusCategory.ListAppStatus_selected = selectedStatus;
                }
                //truyen du lieu cho form custom
                var selectedDue = lst_dueDateMenu_toMe.FindAll(s => s.isSelected == true)[0];
                if (custom_DuedateCategory != null)
                {
                    custom_DuedateCategory.ListClassMenu = lst_dueDateMenu_toMe;
                    custom_DuedateCategory.ClassMenu_selected = selectedDue;
                }
                //danh lable tf_lbl_status
                if (selectedStatus.Count == 1)
                {
                    if (CmmVariable.SysConfig.LangCode == "1066")
                        tf_lbl_status.Text = selectedStatus[0].Title;
                    else
                        tf_lbl_status.Text = selectedStatus[0].TitleEN;
                }
                else if (selectedStatus.Count > 1 && selectedStatus.Count <= lst_appStatus_toMe.Count - 1)
                {
                    if (CmmVariable.SysConfig.LangCode == "1066")
                        tf_lbl_status.Text = selectedStatus[0].Title;
                    else
                        tf_lbl_status.Text = selectedStatus[0].TitleEN;

                    tf_lbl_status.Text += ",+(" + (selectedStatus.Count - 1).ToString() + ")";
                }
                else
                {
                    tf_lbl_status.Text = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
                }
                // danh lai lable duedate
                if (CmmVariable.SysConfig.LangCode == "1066")
                    lbl_duedate.Text = selectedDue.title;
                else
                    lbl_duedate.Text = selectedDue.titleEN;
            }

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

            ToggleCalendar_toMe(true);
        }
        private void BT_todate_TouchUpInside(object sender, EventArgs e)
        {
            tf_fromdate.TextColor = UIColor.FromRGB(0, 0, 0);
            tf_todate.TextColor = UIColor.FromRGB(0, 0, 0);

            custom_CalendarView.inputView = tf_todate;
            custom_CalendarView.viewController = this;
            custom_CalendarView.SetUpDate();

            ToggleCalendar_toMe(true);
        }

        private void BT_Apply_ToMe_TouchUpInside(object sender, EventArgs e)
        {
            AppLyFilter();
        }
        private void AppLyFilter()
        {
            DateTime fromDate = new DateTime();
            DateTime toDate = new DateTime();

            if (tf_fromdate.Text != "")
                fromDate = DateTime.Parse(tf_fromdate.Text, new CultureInfo(CmmVariable.SysConfig.LangCode == "1066" ? "vi" : "en", false));
            else
                fromDate = new DateTime();
            if (tf_todate.Text != "")
                toDate = DateTime.Parse(tf_todate.Text, new CultureInfo(CmmVariable.SysConfig.LangCode == "1066" ? "vi" : "en", false));
            else
                toDate = new DateTime();

            if (fromDate > toDate && toDate != default(DateTime))
            {
                tf_fromdate.TextColor = UIColor.FromRGB(179, 0, 10);
                tf_todate.TextColor = UIColor.FromRGB(179, 0, 10);
                CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("TEXT_DATE_INVALID", "Ngày bắt đầu không thể lớn hơn ngày kết thúc, vui lòng chọn lại"));
            }
            else
            {
                BT_filter.TintColor = UIColor.FromRGB(35, 151, 32);
                previousColorFilter = UIColor.FromRGB(35, 151, 32);
                isFilterServer = true;
                isLoadMore = true;

                temp_fromDateSelectedToMe = fromDate;
                temp_toDateSelectedToMe = toDate;

                if (custom_ConditionCategory != null)
                {
                    if (lst_conditionMenu_toMe[0].isSelected) // do 2 view dang dung chung dang xu ly va da xu ly nen nhung luc khong thao tac
                    {
                        tab_inprogress = true;
                        conditionSelected_toMe = lst_conditionMenu_toMe[0];
                    }
                    else
                    {
                        tab_inprogress = false;
                        conditionSelected_toMe = lst_conditionMenu_toMe[1];
                    }
                }

                if (tab_inprogress)// dang xu ly thi status = default
                {
                    LoadStatusCategory(CmmVariable.APPSTATUS_TOME_DANGXULY);
                    LoadStatusDataToMe();

                }
                else
                {
                    if (custom_AppStatusCategory != null)
                        ListAppStatus_selected_toMe = custom_AppStatusCategory.ListAppStatus_selected;
                    else
                        ListAppStatus_selected_toMe = ExtensionCopy.CopyAll(lst_appStatus_toMe.FindAll(s => s.IsSelected == true));
                }
                if (custom_DuedateCategory != null)
                    DuedateSelected_toMe = custom_DuedateCategory.ClassMenu_selected;

                FilterServer(false);
                ButtonMenuStyleChange();

                searchBar.Text = string.Empty;
                ToggleFilter();
                this.View.EndEditing(true);
            }
        }
        #endregion

        #region handle task

        public void NavigateToAttachView(BeanAttachFile currentAttachFile)
        {
            ShowAttachmentView showAttachmentView = Storyboard.InstantiateViewController("ShowAttachmentView") as ShowAttachmentView;
            showAttachmentView.setContent(this, currentAttachFile);
            this.NavigationController.PushViewController(showAttachmentView, true);
        }
        public void ReloadDataForm()
        {
            //LoadDataFilterTodo();
            ChangeTab();
        }
        public async void loadmoreData()
        {
            view_loadmore.Hidden = false;
            indicator_loadmore.StartAnimating();

            await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.5f));
                InvokeOnMainThread(() =>
                {
                    if (isFilterServer)
                    {
                        FilterServer(true);
                    }
                    else
                    {
                        LoadMoreDataTodo();
                    }
                    indicator_loadmore.StopAnimating();
                    view_loadmore.Hidden = true;
                });
            });
        }
        #endregion

        private void SearchBar_TextChanged(object sender, UISearchBarTextChangedEventArgs e)
        {
            try
            {
                string content = CmmFunction.removeSignVietnamese(searchBar.Text.Trim().ToLowerInvariant());
                if (!string.IsNullOrEmpty(content))
                {
                    var items = from item in lst_appBase_cxl // lst_notify_cxl
                                where ((!string.IsNullOrEmpty(item.Title) && CmmFunction.removeSignVietnamese(item.Title.ToLowerInvariant()).Contains(content)) ||
                                           (!string.IsNullOrEmpty(item.Title) && item.Title.ToLowerInvariant().Contains(content)) ||
                                           (!string.IsNullOrEmpty(item.Content) && CmmFunction.removeSignVietnamese(item.Content.ToLowerInvariant()).Contains(content)))
                                orderby item.Title
                                select item;

                    if (items != null && items.Count() > 0)
                    {
                        dict_todo_result = new Dictionary<string, List<BeanAppBaseExt>>();
                        lst_notify_cxl_results = items.ToList();
                        if (dict_todo_result.ContainsKey("Today"))
                            dict_todo_result["Today"] = lst_notify_cxl_results;
                        else
                            dict_todo_result.Add("Today", lst_notify_cxl_results);

                        table_content.Source = new Todo_TableSource(dict_todo_result, this, query_action);
                        table_content.ReloadData();
                        lbl_nodata.Hidden = true;
                    }
                    else
                    {
                        lbl_nodata.Hidden = false;
                        table_content.Source = null;
                        table_content.ReloadData();
                    }
                }
                else
                {
                    if (query_action != null && query_action.Count() > 0)
                        lbl_nodata.Hidden = true;
                    else
                        lbl_nodata.Hidden = false;

                    table_content.Source = new Todo_TableSource(dict_todo, this, query_action);
                    table_content.ReloadData();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("RequestListViewApp - SearchBar_user_TextChanged - Err: " + ex.ToString());
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

                await Task.Run(() =>
                {
                    provider.UpdateAllMasterData(true);
                    provider.UpdateAllDynamicData(true);
                    p_user.UpdateCurrentUserInfo(localpath);

                    InvokeOnMainThread(() =>
                    {
                        //if (File.Exists(localpath))
                        //    BT_menu.SetImage(UIImage.FromFile(localpath), UIControlState.Normal);

                        refreshControl.EndRefreshing();
                        if (isFilterServer)
                            FilterServer(false);
                        else
                            LoadDataFilterTodo();
                    });
                });
            }
            catch (Exception ex)
            {
                refreshControl.EndRefreshing();
                Console.WriteLine("Error - MainView - refreshControl_valuechange - Er: " + ex.ToString());
            }
        }
        private void BT_home_TouchUpInside(object sender, EventArgs e)
        {
            MainView mainview = (MainView)Storyboard.InstantiateViewController("MainView");
            appD.NavController.PushViewController(mainview, false);
        }
        private void BT_RequestList_TouchUpInside(object sender, EventArgs e)
        {

        }
        private void BT_MyRequestList_TouchUpInside(object sender, EventArgs e)
        {
            RequestListViewApp myRequest = (RequestListViewApp)Storyboard.InstantiateViewController("RequestListViewApp");
            myRequest.setContent(CmmFunction.GetTitle("K_Request", "Tôi bắt đầu"));
            appD.NavController.PushViewController(myRequest, false);
        }

        #endregion

        #region custom class
        #region todo toMe data source table
        private class Todo_TableSource : UITableViewSource
        {
            List<BeanAppBaseExt> lst_todo;
            NSString cellIdentifier = new NSString("cell");
            RequestListViewApp parentView;
            int limit = 20;
            string queryFillter;
            bool isLoadMore = true;
            bool isFilterServer = false;
            public static Dictionary<string, List<BeanAppBaseExt>> indexedCateSession;
            Dictionary<string, List<BeanAppBaseExt>> dict_todo { get; set; }
            Dictionary<string, bool> dict_section { get; set; }

            public Todo_TableSource(Dictionary<string, List<BeanAppBaseExt>> _dict_todo, RequestListViewApp _parentview, string query)
            {
                dict_todo = _dict_todo;
                parentView = _parentview;
                queryFillter = query;
                isFilterServer = parentView.isFilterServer;
                GetDictSection();
            }

            private void GetDictSection()
            {
                dict_section = new Dictionary<string, bool>();
                //dict_section = parentView.dict_sectionTodo;
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
                return 90;
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
                var todo = dict_todo[key][indexPath.Row];

                parentView.NavigateToDetails(todo);
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var key = dict_section.ElementAt(indexPath.Section).Key;
                var todo = dict_todo[key][indexPath.Row];

                bool isOdd = true;
                if (indexPath.Row % 2 == 0)
                    isOdd = false;

                Todo_cell_custom cell = new Todo_cell_custom(cellIdentifier);
                cell.UpdateCell(todo, isOdd);
                return cell;
            }

            public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
            {
                if (parentView.isLoadMore)
                {
                    var lst_appBase_cxl = parentView.lst_appBase_cxl;
                    int sumItem = 0;
                    for (int i = 0; i < indexPath.Section; i++)
                    {
                        var keySum = dict_section.ElementAt(i).Key;
                        var todoSum = dict_todo[keySum];
                        sumItem += todoSum.Count;
                    }
                    sumItem += 1;//them mot item
                    sumItem += indexPath.Row;
                    if (sumItem % (isFilterServer ? CmmVariable.M_DataFilterAPILimitData : CmmVariable.M_DataLimitRow) == 0 && lst_appBase_cxl.Count == sumItem) // boi so cua 20
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



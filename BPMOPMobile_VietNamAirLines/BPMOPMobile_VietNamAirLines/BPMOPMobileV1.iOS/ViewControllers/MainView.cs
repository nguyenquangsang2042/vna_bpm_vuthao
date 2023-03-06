using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobileV1.iOS.Components;
using BPMOPMobileV1.iOS.CustomControlClass;
using BPMOPMobileV1.iOS.IOSClass;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json;
using SQLite;
using UIKit;
using Xamarin.iOS;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class MainView : UIViewController
    {
        public bool isFirst = true;
        bool isAutologin = false;
        int toMe_count, fromMe_count;
        public bool tab_toMe = true;
        UIRefreshControl refreshControl;
        //int limit = 100;
        //int offset = 0;
        AppDelegate appD = UIApplication.SharedApplication.Delegate as AppDelegate;
        Custom_CalendarView custom_CalendarView = Custom_CalendarView.Instance;
        ButtonsActionBotBar buttonActionBotBar;
        CmmLoading loading;

        //filter

        //tome
        public List<ClassMenu> lst_dueDateMenu_toMe;
        public ClassMenu DuedateSelected_toMe;
        public List<BeanAppStatus> lst_appStatus_toMe = new List<BeanAppStatus>();
        public List<BeanAppStatus> ListAppStatus_selected_toMe;
        public List<ClassMenu> lst_conditionMenu_toMe;
        public ClassMenu conditionSelected_toMe;
        DateTime temp_fromDateSelectedToMe = new DateTime();
        DateTime temp_toDateSelectedToMe = new DateTime();
        //public string date_filterToMe = string.Empty;

        //fromme
        public List<ClassMenu> lst_dueDateMenu_fromMe;
        public ClassMenu DuedateSelected_fromMe;
        public List<BeanAppStatus> lst_appStatus_fromMe = new List<BeanAppStatus>();
        public List<BeanAppStatus> ListAppStatus_selected_fromMe;
        DateTime temp_fromDateSelectedFromMe = new DateTime();
        DateTime temp_toDateSelectedFromMe = new DateTime();
        //public string date_filterFromMe = string.Empty;
        UIView view_custom_CalendarView;
        UIColor previousColorFilter = UIColor.FromRGB(0, 0, 0);// lay gia tr icon ban dau mac dinh

        //DateTime date_default;

        List<BeanAppBaseExt> lst_appBase_cxl = new List<BeanAppBaseExt>();
        List<BeanAppBaseExt> lst_appBase_fromMe;
        Dictionary<string, List<BeanAppBaseExt>> dict_todo;
        Dictionary<string, bool> dict_sectionTodo;
        Dictionary<string, List<BeanAppBaseExt>> dict_workflow;
        Dictionary<string, bool> dict_sectionWorkFlow;
        Custom_AppStatusCategory custom_AppStatusCategory;
        Custom_DuedateCategory custom_DuedateCategory;
        nfloat heightTopViewStatus;
        bool tab_inprogress = true;
        /// <summary>
        /// Cờ check hiển thị view filter
        /// </summary>
        bool isFilter = false; // truong hop  dang filter thi nhay vao ViewWillAppear bi bug reload lai form cap nhat lai gia tri
        /// <summary>
        /// Cờ check tình trạng filter
        /// </summary>
        bool isFilterServer = false;
        bool isLoadMore = true;
        bool isOnline = true;
        public bool isBackFromFilter = false, isBackWithoutEditing = false;
        public bool doReload = false;
        UIStringAttributes firstAttributes = new UIStringAttributes
        {
            Font = UIFont.FromName("ArialMT", 12f)
        };

        public MainView(IntPtr handle) : base(handle)
        {
        }

        #region override
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            CreateCircle();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (!isFirst && !isFilter && !isBackFromFilter && !isBackWithoutEditing)
            {
                isLoadMore = true;
                if (tab_toMe)
                {
                    if (isFilterServer)
                        FilterServerTodo(false);
                    else
                        LoadDataFilterTodo();
                }
                else
                {
                    if (isFilterServer)
                        FilterServerFromMe(false);
                    else
                        LoadDataFilterFromMe();
                }
            }

            if (buttonActionBotBar != null)
                bottom_view.AddSubviews(buttonActionBotBar);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            isFirst = false;
            isBackWithoutEditing = false;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            if (!isAutologin)
            {
                listView_indicator.StartAnimating();
                CmmEvent.SyncDataBackgroundResultRequest += CmmEvent_SyncDataBackgroundRequest;
            }
            else
                listView_indicator.StopAnimating();

            ReloadFailedBeanData();

            tab_toMe = true;
            ViewConfiguration();
            //if (string.IsNullOrEmpty(appD.pushDeviceToken))
            //    CmmIOSFunction.commonAlertMessage(this, "pushDeviceToken", "null");
            //else
            //    CmmIOSFunction.commonAlertMessage(this, "pushDeviceToken", appD.pushDeviceToken);
            //UITextView a = new UITextView();
            //this.View.AddSubview(a);
            //a.Frame = new CGRect(0, View.Frame.Height / 2, View.Frame.Width, View.Frame.Height / 2);
            //a.Text = CmmVariable.aaa;

            ButtonMenuStyleChange(BT_todo, true, 0);
            ButtonMenuStyleChange(BT_fromMe, false, 1);

            //init data
            LoadStatusCategory(tab_inprogress ? CmmVariable.APPSTATUS_TOME_DANGXULY : CmmVariable.APPSTATUS_TOME_DAXULY);
            LoadDueDateCategory();
            LoadConditionCategory();

            //load data
            LoadDuedataToMe();
            LoadStatusDataToMe();
            LoadConditionData();
            LoadDuedataFromMe();
            LoadStatusDataFromMe();

            SetDateTime();
            LoadDataFilterTodo();
            LoadDataFilterFromMe();
            SetLangTitle();

            //To me
            BT_todo.BackgroundColor = UIColor.White;
            BT_todo.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);
            BT_fromMe.BackgroundColor = UIColor.Clear;
            BT_fromMe.SetTitleColor(UIColor.White, UIControlState.Normal);

            #region delegate
            CmmEvent.UpdateLangComplete += CmmEvent_UpdateLangComplete;

            BT_avatar.TouchUpInside += BT_avatar_TouchUpInside;
            BT_todo.TouchUpInside += BT_todo_TouchUpInside;
            BT_fromMe.TouchUpInside += BT_fromMe_TouchUpInside;
            BT_filter.TouchUpInside += BT_filter_TouchUpInside;
            BT_createTicket.TouchUpInside += BT_createTicket_TouchUpInside;
            refreshControl.ValueChanged += RefreshControl_ValueChanged;

            BT_status.TouchUpInside += BT_status_TouchUpInside;
            BT_duedate.TouchUpInside += BT_duedate_TouchUpInside;
            BT_State.TouchUpInside += BT_State_TouchUpInside;

            BT_fromdate.TouchUpInside += BT_fromdate_TouchUpInside;
            BT_todate.TouchUpInside += BT_todate_TouchUpInside;
            BT_Apply.TouchUpInside += BT_Apply_TouchUpInside;
            BT_reset_filter.TouchUpInside += BT_reset_filter_TouchUpInside;
            #endregion
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            //ToggleTodo();
        }

        #region private - public method

        void ReloadFailedBeanData()
        {
            CmmIOSFunction.UpdateBeanData<BeanAppStatus>();
        }

        public void setContent(bool _isAutologin)
        {
            isAutologin = _isAutologin;
        }

        #region common

        private void ViewConfiguration()
        {
            SetConstraint();
            view_borderInFilter1.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            view_borderInFilter1.Layer.BorderWidth = 0.6f;
            view_borderInFilter2.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            view_borderInFilter2.Layer.BorderWidth = 0.6f;
            view_borderInFilter3.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            view_borderInFilter3.Layer.BorderWidth = 0.6f;

            view_custom_CalendarView = new UIView()
            {
                Frame = UIScreen.MainScreen.Bounds,
                BackgroundColor = UIColor.FromRGB(25, 25, 30).ColorWithAlpha(0.9f)
            };
            View.AddSubview(view_custom_CalendarView);
            view_custom_CalendarView.Hidden = true;
            view_custom_CalendarView.Frame = View.Frame;

            BT_todo.Layer.ShadowOffset = new CGSize(1, 2);
            BT_todo.Layer.ShadowRadius = 4;
            BT_todo.Layer.ShadowColor = UIColor.Black.CGColor;
            BT_todo.Layer.ShadowOpacity = 0.5f;

            BT_fromMe.Layer.ShadowOffset = new CGSize(-1, 2);
            BT_fromMe.Layer.ShadowRadius = 4;
            BT_fromMe.Layer.ShadowColor = UIColor.Black.CGColor;
            BT_fromMe.Layer.ShadowOpacity = 0.0f;

            //var ver = DeviceHardware.Version;
            //var model = DeviceHardware.Model;
            //if (model.Contains("X") || model.Contains("11"))
            //{
            //    headerView_constantHeight.Constant = 120;
            //}
            heightTopViewStatus = constraint_topView_status.Constant;

            string documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string localpath = Path.Combine(documentFolder, CmmVariable.M_Avatar);
            BT_avatar.ImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
            if (File.Exists(localpath))
                BT_avatar.SetImage(UIImage.FromFile(localpath), UIControlState.Normal);
            else
                BT_avatar.SetImage(UIImage.FromFile("Icons/icon_avatar64.png"), UIControlState.Normal);

            buttonActionBotBar = ButtonsActionBotBar.Instance;
            buttonActionBotBar.mainView = this;
            CGRect bottomBarFrame = new CGRect(bottom_view.Frame.X, bottom_view.Frame.Y, this.View.Frame.Width, bottom_view.Frame.Height);
            buttonActionBotBar.InitFrameView(bottomBarFrame);
            //buttonActionBotBar.InitFrameView(bottom_view.Bounds);
            buttonActionBotBar.LoadStatusButton(0);
            //buttonActionBotBar.UpdateChildBroad(false);
            bottom_view.AddSubviews(buttonActionBotBar);
            CmmIOSFunction.AddShadowForTopORBotBar(bottom_view, false);

            custom_CalendarView.viewController = this;
            custom_CalendarView.BackgroundColor = UIColor.White;
            custom_CalendarView.Layer.BorderColor = UIColor.FromRGB(65, 80, 134).CGColor;
            custom_CalendarView.Layer.BorderWidth = 1;
            custom_CalendarView.Layer.CornerRadius = 10;
            custom_CalendarView.ClipsToBounds = true;

            refreshControl = new UIRefreshControl();
            refreshControl.TintColor = UIColor.FromRGB(9, 171, 78);
            refreshControl.AttributedTitle = new NSAttributedString(CmmFunction.GetTitle("TEXT_REFRESH_LOADING", "Loading ..."), firstAttributes);

            table_content.ContentInset = new UIEdgeInsets(-7, 0, 0, 0);
            table_content.AddSubview(refreshControl);
        }

        private void CreateCircle()
        {
            try
            {
                double min = Math.Min(BT_avatar.ImageView.Frame.Width, BT_avatar.ImageView.Frame.Height);
                BT_avatar.ImageView.Layer.CornerRadius = (float)(min / 2.0);
                BT_avatar.ImageView.Layer.MasksToBounds = false;
                BT_avatar.ImageView.Layer.BorderColor = UIColor.Clear.CGColor;
                BT_avatar.ImageView.Layer.BorderWidth = 0;
                BT_avatar.ImageView.ClipsToBounds = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("SettingViewController - CreateCircle - Err: " + ex.ToString());
            }
        }

        private void SetLangTitle()
        {
            if (CmmVariable.SysConfig.LangCode == "1033")
                custom_CalendarView.calendarView.Locale = new NSLocale("en_US");
            else
                custom_CalendarView.calendarView.Locale = new NSLocale("vi_VI");

            custom_CalendarView.calendarView.ReloadData();

            if (tab_toMe)
            {
                BT_todo.BackgroundColor = UIColor.White;
                BT_todo.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);
                BT_fromMe.BackgroundColor = UIColor.Clear;
                BT_fromMe.SetTitleColor(UIColor.White, UIControlState.Normal);

                ButtonMenuStyleChange(BT_todo, true, 0);
                ButtonMenuStyleChange(BT_fromMe, false, 1);
            }
            else
            {
                ButtonMenuStyleChange(BT_todo, false, 0);
                ButtonMenuStyleChange(BT_fromMe, true, 1);
            }

            CmmIOSFunction.SetLangToView(this.View);
        }

        private void SetConstraint()
        {
            headerView_constantHeight.Constant = 87 + 10 + CmmIOSFunction.GetHeaderViewHeight();
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                if (UIApplication.SharedApplication.KeyWindow?.SafeAreaInsets.Bottom > 0)
                {
                    contraint_heightViewNavBot.Constant += 10;
                }
            }
        }

        //LOAD DATA
        private void loadData_count(int _dataCount, bool _isToMe)
        {
            try
            {
                // danh sách yều cầu cần xử lý default
                var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                string str_toMe = string.Empty;
                // yêu cầu cần xử lý

                //string query_vcxl = string.Format("SELECT Count(*) as count FROM BeanNotify WHERE Status = 0");
                //List<CountNum> countnum_vcxl = conn.Query<CountNum>(query_vcxl);

                //if (countnum_vcxl != null)
                //toMe_count = countnum_vcxl[0].count;
                //GetCountNumber();
                if (_isToMe)
                {
                    toMe_count = _dataCount;
                    str_toMe = CmmFunction.GetTitle("TEXT_TOME", "Đến tôi");
                    BT_todo.SetAttributedTitle(null, UIControlState.Normal);
                    if (toMe_count >= 100)
                    {
                        BT_todo.SetTitle(str_toMe + " (99+)", UIControlState.Normal);
                        BT_todo.TitleLabel.Text = str_toMe + " (99+)";
                    }
                    else if (toMe_count > 0 && toMe_count < 100)
                    {
                        str_toMe = str_toMe + " (" + toMe_count.ToString() + ")";
                        BT_todo.SetTitle(str_toMe, UIControlState.Normal);
                        BT_todo.TitleLabel.Text = str_toMe;
                    }
                    else
                    {
                        BT_todo.SetTitle(str_toMe, UIControlState.Normal);
                        BT_todo.TitleLabel.Text = str_toMe;
                    }

                    ButtonMenuStyleChange(BT_todo, true, 0);
                    ButtonMenuStyleChange(BT_fromMe, false, 1);
                }
                // From me
                else
                {
                    //string query_today = string.Format("SELECT Count(*) as count FROM BeanWorkflowItem WHERE CreatedBy = '{0}' ORDER BY Created", CmmVariable.SysConfig.UserId);
                    //List<CountNum> countnum_myRequest = conn.Query<CountNum>(query_today);

                    //if (countnum_myRequest != null)
                    //    fromMe_count = countnum_myRequest[0].count;
                    BT_fromMe.SetAttributedTitle(null, UIControlState.Normal);
                    fromMe_count = _dataCount;
                    string str_fromMe = CmmFunction.GetTitle("TEXT_FROMME", "Tôi bắt đầu");
                    if (fromMe_count >= 100)
                    {
                        BT_fromMe.SetTitle(str_fromMe + " (99+)", UIControlState.Normal);
                        BT_fromMe.TitleLabel.Text = str_fromMe + " (99+)";
                    }
                    else if (fromMe_count > 0 && fromMe_count <= 100)
                    {
                        str_fromMe = str_fromMe + " (" + fromMe_count.ToString() + ")";
                        BT_fromMe.SetTitle(str_fromMe, UIControlState.Normal);
                        BT_fromMe.TitleLabel.Text = str_fromMe;
                    }
                    else
                    {
                        BT_fromMe.SetTitle(str_fromMe, UIControlState.Normal);
                        BT_fromMe.TitleLabel.Text = str_fromMe;
                    }

                    ButtonMenuStyleChange(BT_todo, false, 0);
                    ButtonMenuStyleChange(BT_fromMe, true, 1);
                }
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
                //await Task.Run(() =>
                //{
                //    count = new ProviderControlDynamic().GetListCountVDT_VTBD(CmmVariable.KEY_COUNT_FROMME_INPROCESS + "|" + CmmVariable.KEY_COUNT_ASIGNTOME_INPROCESS);
                //});
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

        private void ReloadMoreData(int _index, List<BeanAppBaseExt> _lst_vcxl, string _query)
        {
            if (_index == 0)
            {
                lst_appBase_cxl = _lst_vcxl;
                table_content.Source = new Todo_TableSource(dict_todo, this, _query);
                table_content.ReloadData();
            }
        }

        private void ToggleTodo()
        {
            filter_view.Alpha = 0;
            View_filter_Form_ToMe.Alpha = 0;

            if (tab_toMe) // dang la trang thai todo cua toi
            {
                tab_toMe = true;
                BT_todo.BackgroundColor = UIColor.White;
                BT_todo.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);
                BT_fromMe.BackgroundColor = UIColor.Clear;
                BT_fromMe.SetTitleColor(UIColor.White, UIControlState.Normal);

                BT_fromMe.Layer.ShadowOpacity = 0.0f;
                BT_todo.Layer.ShadowOpacity = 0.5f;

                ButtonMenuStyleChange(BT_todo, true, 0);
                ButtonMenuStyleChange(BT_fromMe, false, 1);
            }
            else // dang la trang thai tu toi
            {
                tab_toMe = false;
                BT_todo.BackgroundColor = UIColor.Clear;
                BT_todo.SetTitleColor(UIColor.White, UIControlState.Normal);
                BT_fromMe.BackgroundColor = UIColor.White;
                BT_fromMe.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);

                BT_fromMe.Layer.ShadowOpacity = 0.5f;
                BT_todo.Layer.ShadowOpacity = 0.0f;

                ButtonMenuStyleChange(BT_todo, false, 0);
                ButtonMenuStyleChange(BT_fromMe, true, 1);
            }
        }
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
                View_filter_Form_ToMe.Frame = new CGRect(0, 0, filter_view.Frame.Width, 422);
                View_filter_Form_ToMe.Alpha = 1;
                UIView.CommitAnimations();

                BT_filter.TintColor = UIColor.FromRGB(35, 151, 32); // moi lan mo len thi bat filter mau xanh
                SetValueFormFilter();
            }
            else
            {
                isFilter = false;
                //BT_filter.TintColor = UIColor.FromRGB(0, 0, 0);
                //BT_filter.TintColor = UIColor.White;
                View_filter_Form_ToMe.Frame = new CGRect(0, 0, filter_view.Frame.Width, 422);
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

        private void ButtonMenuStyleChange(UIButton _button, bool isSelected, int _index)
        {
            string str_transalte = "";
            if (!isSelected)
            {
                if (_index == 0)
                {
                    if (BT_todo.TitleLabel.Text.Contains("("))
                    {
                        str_transalte = CmmFunction.GetTitle("TEXT_TOME", BT_todo.TitleLabel.Text);
                        if (!str_transalte.Contains("("))
                        {
                            string str_toMe_count = "";
                            if (toMe_count >= 100)
                                str_toMe_count = " (99+)";
                            else if (toMe_count > 0 && toMe_count < 100)
                            {
                                if (toMe_count > 0 && toMe_count < 10)
                                    str_toMe_count = " (" + "0" + toMe_count.ToString() + ")"; // hien thi 2 so vd: 08
                                else
                                    str_toMe_count = " (" + toMe_count.ToString() + ")";
                            }
                            else
                                str_toMe_count = "";

                            str_transalte = str_transalte + str_toMe_count;
                        }

                        //var indexA = str_transalte.IndexOf('(');
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.Font, UIFont.FromName("ArialMT", 14f), new NSRange(0, str_transalte.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.White, new NSRange(0, str_transalte.Length));
                        _button.SetAttributedTitle(att, UIControlState.Normal);
                    }
                    else
                    {
                        str_transalte = CmmFunction.GetTitle("TEXT_TOME", BT_todo.TitleLabel.Text);
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.Font, UIFont.FromName("ArialMT", 14f), new NSRange(0, str_transalte.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.White, new NSRange(0, str_transalte.Length));
                        _button.SetAttributedTitle(att, UIControlState.Normal);
                    }
                }
                else if (_index == 1)
                {
                    if (BT_fromMe.TitleLabel.Text.Contains("("))
                    {
                        str_transalte = CmmFunction.GetTitle("TEXT_FROMME", BT_fromMe.TitleLabel.Text);
                        if (!str_transalte.Contains("("))
                        {
                            string str_fromMe_count = "";
                            if (fromMe_count >= 100)
                                str_fromMe_count = " (99+)";
                            else if (fromMe_count > 0 && fromMe_count < 100)
                            {
                                if (fromMe_count > 0 && fromMe_count < 10)
                                    str_fromMe_count = " (" + "0" + fromMe_count.ToString() + ")"; // hien thi 2 so vd: 08
                                else
                                    str_fromMe_count = " (" + fromMe_count.ToString() + ")";
                            }
                            else
                                str_fromMe_count = "";

                            str_transalte = str_transalte + str_fromMe_count;
                        }

                        //var indexA = str_transalte.IndexOf('(');
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.Font, UIFont.FromName("ArialMT", 14f), new NSRange(0, str_transalte.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.White, new NSRange(0, str_transalte.Length));
                        _button.SetAttributedTitle(att, UIControlState.Normal);
                    }
                    else
                    {
                        str_transalte = CmmFunction.GetTitle("TEXT_FROMME", BT_fromMe.TitleLabel.Text);
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.Font, UIFont.FromName("ArialMT", 14f), new NSRange(0, str_transalte.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.White, new NSRange(0, str_transalte.Length));
                        _button.SetAttributedTitle(att, UIControlState.Normal);
                    }
                }
            }
            else //selected
            {
                if (_index == 0)
                {
                    if (BT_todo.TitleLabel.Text.Contains("("))
                    {
                        str_transalte = CmmFunction.GetTitle("TEXT_TOME", BT_todo.TitleLabel.Text);
                        if (!str_transalte.Contains("("))
                        {
                            string str_toMe_count = "";
                            if (toMe_count >= 100)
                                str_toMe_count = " (99+)";
                            else if (toMe_count > 0 && toMe_count < 100)
                            {
                                if (toMe_count > 0 && toMe_count < 10)
                                    str_toMe_count = " (" + "0" + toMe_count.ToString() + ")"; // hien thi 2 so vd: 08
                                else
                                    str_toMe_count = " (" + toMe_count.ToString() + ")";
                            }
                            else
                                str_toMe_count = "";

                            str_transalte = str_transalte + str_toMe_count;
                        }

                        var indexA = str_transalte.IndexOf('(');
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.Font, UIFont.FromName("Arial-BoldMT", 14f), new NSRange(0, str_transalte.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(0, str_transalte.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(255, 122, 58), new NSRange(indexA, str_transalte.Length - indexA));
                        _button.SetAttributedTitle(att, UIControlState.Normal);
                    }
                    else
                    {
                        str_transalte = CmmFunction.GetTitle("TEXT_TOME", BT_todo.TitleLabel.Text);
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.Font, UIFont.FromName("Arial-BoldMT", 14f), new NSRange(0, str_transalte.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(0, att.Length));
                        _button.SetAttributedTitle(att, UIControlState.Normal);
                    }
                }
                else if (_index == 1)
                {
                    if (BT_fromMe.TitleLabel.Text.Contains("("))
                    {
                        str_transalte = CmmFunction.GetTitle("TEXT_FROMME", BT_fromMe.TitleLabel.Text);
                        if (!str_transalte.Contains("("))
                        {
                            string str_fromMe_count = "";
                            if (fromMe_count >= 100)
                                str_fromMe_count = " (99+)";
                            else if (fromMe_count > 0 && fromMe_count < 100)
                            {
                                if (fromMe_count > 0 && fromMe_count < 10)
                                    str_fromMe_count = " (" + "0" + fromMe_count.ToString() + ")"; // hien thi 2 so vd: 08
                                else
                                    str_fromMe_count = " (" + fromMe_count.ToString() + ")";
                            }
                            else
                                str_fromMe_count = "";

                            str_transalte = str_transalte + str_fromMe_count;
                        }
                        var indexA = str_transalte.IndexOf('(');
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.Font, UIFont.FromName("Arial-BoldMT", 14f), new NSRange(0, att.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(0, att.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(255, 122, 58), new NSRange(indexA, str_transalte.Length - indexA)); // inbox => blue
                        _button.SetAttributedTitle(att, UIControlState.Normal);
                    }
                    else
                    {
                        str_transalte = CmmFunction.GetTitle("TEXT_FROMME", BT_fromMe.TitleLabel.Text);
                        NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
                        att.AddAttribute(UIStringAttributeKey.Font, UIFont.FromName("Arial-BoldMT", 14f), new NSRange(0, att.Length));
                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(0, att.Length));
                        _button.SetAttributedTitle(att, UIControlState.Normal);
                    }
                }
            }
        }

        private void LoadStatusCategory(string key)
        {
            var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
            string defaultIdAppstatus = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS);
            string _whereClause = !string.IsNullOrEmpty(defaultIdAppstatus) ? string.Format(@"WHERE ID IN ({0})", defaultIdAppstatus) : "";
            string query_worflowCategory = string.Format(@"SELECT * FROM BeanAppStatus {0}", _whereClause);
            lst_appStatus_toMe = conn.Query<BeanAppStatus>(query_worflowCategory);
            lst_appStatus_fromMe = conn.Query<BeanAppStatus>(query_worflowCategory);

            string defaultIdAppstatustoMe = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS_FROMME);
            string[] arrayStatus = defaultIdAppstatustoMe.Split(",");
            for (int i = 0; i < lst_appStatus_fromMe.Count; i++)
            {
                if (arrayStatus.Contains(lst_appStatus_fromMe[i].ID.ToString()))
                    lst_appStatus_fromMe[i].IsSelected = true;
            }

            string defaultIdAppstatusToMe = CmmFunction.GetAppSettingValue(key);
            string[] arrayStatusToMe = defaultIdAppstatusToMe.Split(",");
            for (int i = 0; i < lst_appStatus_toMe.Count; i++)
            {
                if (arrayStatusToMe.Contains(lst_appStatus_toMe[i].ID.ToString()))
                    lst_appStatus_toMe[i].IsSelected = true;
            }
        }

        private void LoadDueDateCategory()
        {
            lst_dueDateMenu_toMe = new List<ClassMenu>();
            lst_dueDateMenu_fromMe = new List<ClassMenu>();
            ClassMenu m1 = new ClassMenu() { ID = 1, section = 0, titleEN = "All", title = "Tất cả" };
            ClassMenu m2 = new ClassMenu() { ID = 2, section = 0, titleEN = "Today", title = "Trong ngày" };
            ClassMenu m3 = new ClassMenu() { ID = 3, section = 0, titleEN = "Overdue", title = "Trễ hạn" };

            lst_dueDateMenu_toMe.AddRange(new[] { m1, m2, m3 });
            lst_dueDateMenu_fromMe.AddRange(new[] { m1, m2, m3 });

            lst_dueDateMenu_toMe[0].isSelected = true;
            lst_dueDateMenu_fromMe[0].isSelected = true;
        }

        private void LoadConditionCategory()
        {
            lst_conditionMenu_toMe = new List<ClassMenu>();
            ClassMenu m1 = new ClassMenu() { ID = 1, section = 0, titleEN = "In process", title = "Đang xử lý" };
            ClassMenu m2 = new ClassMenu() { ID = 2, section = 0, titleEN = "Processed", title = "Đã xử lý" };

            lst_conditionMenu_toMe.AddRange(new[] { m1, m2 });
            // Default chọn đang xử lý 
            lst_conditionMenu_toMe[0].isSelected = true;
        }

        private void LoadDuedataToMe()
        {
            // tat ca
            DuedateSelected_toMe = lst_dueDateMenu_toMe[0];
        }

        private void LoadStatusDataToMe()
        {
            if (ListAppStatus_selected_toMe != null && ListAppStatus_selected_toMe.Count >= 0)
                ListAppStatus_selected_toMe.Clear();
            else
                ListAppStatus_selected_toMe = new List<BeanAppStatus>();

            ListAppStatus_selected_toMe = lst_appStatus_toMe.FindAll(s => s.IsSelected == true);
        }

        private void LoadDuedataFromMe()
        {
            // tat ca
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
            //setting date
            temp_fromDateSelectedToMe = DateTime.Now.AddDays(-CmmVariable.M_DataFilterDefaultDays);
            temp_fromDateSelectedFromMe = DateTime.Now.AddDays(-CmmVariable.M_DataFilterDefaultDays);
            temp_toDateSelectedToMe = DateTime.Now;
            temp_toDateSelectedFromMe = DateTime.Now;
            //SetingDateFromTo();
            //date_filterToMe = CmmIOSFunction.GetStringDateFilter(temp_fromDateSelectedToMe, temp_toDateSelectedToMe);
            //date_filterFromMe = CmmIOSFunction.GetStringDateFilter(temp_fromDateSelectedFromMe, temp_toDateSelectedFromMe);
        }

        //private void SetingDateFromTo()
        //{
        //    // format time is 12:00 am
        //    if (CmmVariable.SysConfig.LangCode == "1066")
        //    {
        //        temp_fromDateSelectedToMe = DateTime.Parse(temp_fromDateSelectedToMe.ToString(@"dd/MM/yyyy", new CultureInfo("vi")), new CultureInfo("vi", false));
        //        temp_fromDateSelectedFromMe = DateTime.Parse(temp_fromDateSelectedFromMe.ToString(@"dd/MM/yyyy", new CultureInfo("vi")), new CultureInfo("vi", false));
        //        temp_toDateSelectedToMe = DateTime.Parse(temp_toDateSelectedToMe.ToString(@"dd/MM/yyyy", new CultureInfo("vi")), new CultureInfo("vi", false));
        //        temp_toDateSelectedFromMe = DateTime.Parse(temp_toDateSelectedFromMe.ToString(@"dd/MM/yyyy", new CultureInfo("vi")), new CultureInfo("vi", false));
        //    }
        //    else
        //    {
        //        temp_fromDateSelectedToMe = DateTime.Parse(temp_fromDateSelectedToMe.ToString(@"MM/dd/yyyy", new CultureInfo("en")), new CultureInfo("en", false));
        //        temp_fromDateSelectedFromMe = DateTime.Parse(temp_fromDateSelectedFromMe.ToString(@"MM/dd/yyyy", new CultureInfo("en")), new CultureInfo("en", false));
        //        temp_toDateSelectedToMe = DateTime.Parse(temp_toDateSelectedToMe.ToString(@"MM/dd/yyyy", new CultureInfo("en")), new CultureInfo("en", false));
        //        temp_toDateSelectedFromMe = DateTime.Parse(temp_toDateSelectedFromMe.ToString(@"MM/dd/yyyy", new CultureInfo("en")), new CultureInfo("en", false));
        //    }
        //    //seting time today folow 12:00 pm
        //    temp_toDateSelectedToMe = temp_toDateSelectedToMe.AddDays(1).AddMinutes(-1);
        //    temp_toDateSelectedFromMe = temp_toDateSelectedFromMe.AddDays(1).AddMinutes(-1);
        //}
        private void LoadConditionData()
        {
            // Đang xử lý 
            conditionSelected_toMe = lst_conditionMenu_toMe[0];
        }

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
                SQLiteAsyncConnection conn = new SQLiteAsyncConnection(CmmVariable.M_DataPath, false);
                string _workflowItemID = CmmFunction.GetWorkflowItemIDByUrl(beanAppBase.ItemUrl);
                string _query = string.Format("SELECT * FROM BeanWorkflowItem WHERE ID = '{0}' LIMIT 1 OFFSET 0", _workflowItemID);
                List<BeanWorkflowItem> _list_workFlowItem = conn.QueryAsync<BeanWorkflowItem>(_query).Result;

                //FormTaskDetails taskDetails = (FormTaskDetails)Storyboard.InstantiateViewController("FormTaskDetails");
                //taskDetails.SetContent(beanAppBase, workflowItem[0], this);
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
            else //chi tiet phieu
            {
                RequestDetailsV2 v2 = (RequestDetailsV2)Storyboard.InstantiateViewController("RequestDetailsV2");
                v2.setContent(this, beanAppBase);
                this.NavigationController.PushViewController(v2, true);
            }
        }
        /*
        public void HandleSectionTable(nint section, string key, int tableIndex)
        {
            if (tableIndex == 0)
            {
                //dict_sectionTodo[key] = !dict_sectionTodo[key];
                //table_content.ReloadSections(new NSIndexSet((uint)section), UITableViewRowAnimation.None);
            }
            else
            {
                dict_sectionWorkFlow[key] = !dict_sectionWorkFlow[key];
                table_content.ReloadSections(new NSIndexSet((uint)section), UITableViewRowAnimation.None);
            }
        }
        */
        public void SignOut()
        {
            try
            {
                NSUserDefaults.StandardUserDefaults.SetString("", "ActiveUser");
                //CmmEvent.SyncDataRequest -= CmmEvent_SyncDataRequest;

                //if (timerResync != null)
                //{
                //    timerResync.timerReSync.Stop();
                //}

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
                AppDelegate appD = UIApplication.SharedApplication.Delegate as AppDelegate;
                appD.beanUpdateManagement = new NSMutableDictionary();

                var navigationController = Storyboard.InstantiateViewController("RootNavigation") as RootNavigation;
                //appD.Window.RootViewController = navigationController;
                foreach (var view in UIApplication.SharedApplication.Windows)
                {
                    if (view.RootViewController.GetType().Name == "RootNavigation")
                    {
                        view.RootViewController = navigationController;
                    }
                }
                //UIApplication.SharedApplication.Windows[1].RootViewController = navigationController;
                this.DismissViewController(true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("MainView - SignOut - Err: " + ex.ToString());
            }
        }
        #endregion

        #region toMe
        /// <summary>
        /// filter different default then get from server
        /// </summary>
        private void FilterServerTodo(bool isLoadMoreDataFilter)
        {
            try
            {
                //dict_todo = new Dictionary<string, List<BeanAppBaseExt>>();
                //dict_sectionTodo = new Dictionary<string, bool>();
                if (!isLoadMoreDataFilter)
                    lst_appBase_cxl = new List<BeanAppBaseExt>();

                //Check filter DueDate
                string str_status = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS); // tat ca from me
                if (ListAppStatus_selected_toMe != null && ListAppStatus_selected_toMe.Count > 0) // co chon status
                    str_status = string.Join(',', ListAppStatus_selected_toMe.Select(i => i.ID));
                ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();
                int totalRecord = 0;
                var listPropertiesFilter = CmmFunction.BuildListPropertiesFilter(CmmVariable.M_ResourceViewID_ToMe, tab_inprogress ? "2" : "4", str_status, DuedateSelected_toMe.ID, temp_fromDateSelectedToMe, temp_toDateSelectedToMe);

                //loadmore then cu filter
                if (isLoadMoreDataFilter)
                {
                    List<BeanAppBaseExt> lst_appBase_cxl_more = new List<BeanAppBaseExt>();
                    lst_appBase_cxl_more = _pControlDynamic.GetListFilterMyTask(listPropertiesFilter, ref totalRecord, CmmVariable.M_DataFilterAPILimitData, lst_appBase_cxl.Count);// lst_appBase_fromMe.Count
                    lst_appBase_cxl_more = lst_appBase_cxl_more.OrderByDescending(s => s.NotifyCreated).ToList();

                    if (lst_appBase_cxl_more != null && lst_appBase_cxl_more.Count > 0)
                    {
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
                // Lan dau filter
                else
                {
                    lst_appBase_cxl = _pControlDynamic.GetListFilterMyTask(listPropertiesFilter, ref totalRecord, CmmVariable.M_DataFilterAPILimitData, 0);// lst_appBase_fromMe.Count
                    lst_appBase_cxl = lst_appBase_cxl.OrderByDescending(s => s.NotifyCreated).ToList();
                }

                if (lst_appBase_cxl != null && lst_appBase_cxl.Count > 0)
                {
                    toMe_count = totalRecord;
                    loadData_count(toMe_count, true);
                    SortListAppBaseToDo("", true);
                }
                else
                {
                    toMe_count = 0;
                    loadData_count(toMe_count, true);
                    table_content.Alpha = 0;
                    lbl_nodata.Hidden = false;
                }
            }
            catch (Exception ex)
            {
                //toMe_count = 0;
                //loadData_count(toMe_count, true);
                table_content.Source = null;
                table_content.ReloadData();

                table_content.Alpha = 0;
                lbl_nodata.Hidden = false;

                Console.WriteLine("MainView - FilterServerTodo - Err: " + ex.ToString());
            }
        }

        /// <summary>
        /// Viec Den toi local
        /// </summary>
        /// <param name="_statusIndex">0: Tatca | 1: Can XL | 2: Da XL</param>
        /// <param name="_dueDateIndex">0: Tatca | 1: QuaHan | 2: TrongHan </param>
        private void LoadDataFilterTodo()
        {
            isOnline = Reachability.detectNetWork();
            try
            {
                //dict_todo = new Dictionary<string, List<BeanAppBaseExt>>();
                //dict_sectionTodo = new Dictionary<string, bool>();
                //lst_appBase_cxl = new List<BeanAppBaseExt>();

                LoadDataToDoOffLine();
                if (isOnline)
                    LoadDataToDoOnLine();
            }
            catch (Exception ex)
            {
                toMe_count = 0;
                loadData_count(toMe_count, true);
                table_content.Source = null;
                table_content.ReloadData();

                table_content.Alpha = 0;
                lbl_nodata.Hidden = false;

                Console.WriteLine("MainView - LoadDataFilterTodo - Err: " + ex.ToString());
            }
        }

        void LoadDataToDoOnLine()
        {
            //dict_todo = new Dictionary<string, List<BeanAppBaseExt>>();
            //dict_sectionTodo = new Dictionary<string, bool>();
            //lst_appBase_cxl = new List<BeanAppBaseExt>();

            //GetCountNumber();
            //loadData_count(toMe_count, true);
            GetListToDoOnline();
        }

        void LoadDataToDoOffLine()
        {
            var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
            string query = string.Empty;
            string queryCount = string.Empty;
            List<CountNum> lst_count_appBase_cxl = new List<CountNum>();

            ////getcount
            //queryCount = CreateQueryToDo(true);
            //lst_count_appBase_cxl = conn.Query<CountNum>(queryCount);
            //if (lst_count_appBase_cxl != null && lst_count_appBase_cxl.Count > 0)
            //{
            //    toMe_count = lst_count_appBase_cxl.First().count;
            //}
            //else
            //{
            //    toMe_count = 0;
            //    loadData_count(toMe_count, true);
            //}
            //data
            query = CreateQueryToDo(false);
            lst_appBase_cxl = conn.Query<BeanAppBaseExt>(query, CmmVariable.M_DataLimitRow, 0);
            if (lst_appBase_cxl != null && lst_appBase_cxl.Count > 0)
            {
                loadData_count(toMe_count, true);
                SortListAppBaseToDo(query, false);
            }
            else
            {
                table_content.Alpha = 0;
                lbl_nodata.Hidden = false;
            }
        }

        async void GetListToDoOnline(bool isLoadMore = false)
        {
            await Task.Run(() =>
            {
                if (!isLoadMore)
                {
                    GetCountNumber();
                    InvokeOnMainThread(() =>
                    {
                        loadData_count(toMe_count, true);
                        //ToggleTodo();
                    });
                }
            });

            _ = Task.Run(() =>
            {
                try
                {
                    List<BeanAppBaseExt> lstObj = new List<BeanAppBaseExt>();
                    if (!doReload && !isLoadMore) lst_appBase_cxl = new List<BeanAppBaseExt>();
                    lstObj = new ProviderBase().LoadMoreDataTFromSerVer(
                        CmmVariable.KEY_GET_TOME_INPROCESS,
                        doReload ? lst_appBase_cxl.Count : 20,
                         isLoadMore ? lst_appBase_cxl.Count : 0);

                    if (lstObj != null && lstObj.Count > 0)
                    {
                        CmmIOSFunction.UpdateNewListDataOnline(lstObj, new SQLiteConnection(CmmVariable.M_DataPath, false));
                        if (isLoadMore)
                        {
                            if (lst_appBase_cxl == null) lst_appBase_cxl = new List<BeanAppBaseExt>();
                            lst_appBase_cxl.AddRange(lstObj);
                        }
                        else
                            lst_appBase_cxl = lstObj;

                        InvokeOnMainThread(() =>
                        {
                            if (tab_toMe)
                                SortListAppBaseToDo("", false);
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
                        if (lst_appBase_cxl != null && lst_appBase_cxl.Count > 0)
                        {
                            //table_content.ScrollToRow(NSIndexPath.FromItemSection(0, 0), UITableViewScrollPosition.Top, true);
                            //table_content.ContentOffset = new CGPoint(0, -table_content.ContentInset.Top);
                            table_content.ScrollRectToVisible(new CGRect(0, 0, 0, 0), false);
                        }
                        else
                        {
                            lst_appBase_cxl = new List<BeanAppBaseExt>();
                            table_content.Alpha = 0;
                            lbl_nodata.Hidden = false;
                        }
                    });
                }
            });
        }

        private void LoadMoreDataTodo()
        {
            try
            {
                List<BeanAppBaseExt> lst_todo_more = new List<BeanAppBaseExt>();
                //dict_todo = new Dictionary<string, List<BeanAppBaseExt>>();
                //dict_sectionTodo = new Dictionary<string, bool>();

                if (isOnline)
                {
                    GetListToDoOnline(true);
                }
                else
                {
                    var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                    string query = string.Empty;

                    //data
                    query = CreateQueryToDo(false);
                    lst_todo_more = conn.Query<BeanAppBaseExt>(query, CmmVariable.M_DataLimitRow, lst_appBase_cxl.Count);

                    if (lst_todo_more != null && lst_todo_more.Count > 0)
                    {
                        lst_appBase_cxl.AddRange(lst_todo_more);
                        SortListAppBaseToDo(query, false);
                        if (lst_todo_more.Count < CmmVariable.M_DataLimitRow)
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
                toMe_count = 0;
                loadData_count(toMe_count, true);
                table_content.Source = null;
                table_content.ReloadData();

                table_content.Alpha = 0;
                lbl_nodata.Hidden = false;

                Console.WriteLine("MainView - LoadDataFilterTodo - Err: " + ex.ToString());
            }
        }

        private string CreateQueryToDo(bool isGetCount)
        {
            #region kiểu cũ
            /*string query = string.Empty;
            //Check filter DueDate
            //string duedate_condition = "";
            //if (DuedateSelected_toMe == null || DuedateSelected_toMe.ID == 1) // Tat ca
            //    duedate_condition = "";
            //else if (DuedateSelected_toMe.ID == 2) // Trong ngay
            //    duedate_condition = @"AND (AB.DueDate IS NOT NULL AND date(AB.DueDate) = date('now'))";
            //else if (DuedateSelected_toMe.ID == 3) // Tre han
            //    duedate_condition = @"AND (AB.DueDate IS NOT NULL AND AB.DueDate < date('now'))";

            //Check switch status
            string query_inprogress = "";
            if (lst_conditionMenu_toMe[0].isSelected) //local chi hien thi dang xu ly duoi local
                query_inprogress = "NOTI.Type = 1 AND NOTI.Status = 0";

            string str_status = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS); // tat ca fromme
            if (ListAppStatus_selected_toMe != null && ListAppStatus_selected_toMe.Count > 0) // co chon status
                str_status = string.Join(',', ListAppStatus_selected_toMe.Select(i => i.ID));

            if (!string.IsNullOrEmpty(str_status))
                str_status = string.Format(@" AND AB.StatusGroup IN({0})", str_status);

            //WHERE(AB.AssignedTo LIKE '%{0}%'  OR AB.NotifiedUsers LIKE '%{0}%') AND AB.StatusGroup IN({ 1}) AND NOTI.Type = 1 AND NOTI.Status = 0
            if (isGetCount) // count *
                query = string.Format("SELECT Count(*) as count "
                    + "FROM BeanAppBase AB "
                    + "INNER JOIN BeanNotify NOTI "
                    + "ON AB.ID = NOTI.SPItemId "
                    + "WHERE {0} AND (AB.AssignedTo LIKE '%{1}%' OR AB.NotifiedUsers LIKE '%{1}%') {2} ",
                 query_inprogress, CmmVariable.SysConfig.UserId.ToUpper(), str_status);
            else
                query = string.Format(@"SELECT AB.*, NOTI.StartDate, NOTI.Read, NOTI.SPItemId, NOTI.SubmitAction, NOTI.SubmitActionEN, NOTI.SendUnit "
                    + "FROM BeanAppBase AB "
                    + "INNER JOIN BeanNotify NOTI "
                    + "ON AB.ID = NOTI.SPItemId "
                    + "WHERE {0} AND (AB.AssignedTo LIKE '%{1}%' OR AB.NotifiedUsers LIKE '%{1}%') {2} "
                    + "Order By NOTI.StartDate DESC LIMIT ? OFFSET ?",
                 query_inprogress, CmmVariable.SysConfig.UserId.ToUpper(), str_status);
            return query;
            */
            #endregion

            return CmmIOSFunction.GetQueryStringAppBaseVDT(true);
        }

        /// <summary>
        /// sort secsion from create
        /// </summary>
        /// <param name="query"></param>
        private void SortListAppBaseToDo(string query, bool isOnline)
        {
            if (lst_appBase_cxl != null && lst_appBase_cxl.Count > 0)
            {
                dict_todo = new Dictionary<string, List<BeanAppBaseExt>>();
                dict_sectionTodo = new Dictionary<string, bool>();

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
                    if (tab_inprogress) // tab dang xu ly
                    {
                        if (isOnline) //server sort Modified
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
                        else// local  sort StartDate
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
                    else // server, local sort Modified tab da xu ly
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
                }

                table_content.Source = new Todo_TableSource(dict_todo, this, query);
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

        public void ToggleCalendar_toMe(bool _show)
        {
            if (!_show)
            {
                nfloat height = UIScreen.MainScreen.Bounds.Height * ((float)297 / 736);
                BT_Apply.Hidden = false;

                custom_CalendarView.InitFrameView(new CGRect(15, UIScreen.MainScreen.Bounds.X / 2 - height / 2, custom_CalendarView.Frame.Width - 30, height));
                custom_CalendarView.RemoveFromSuperview();
                view_custom_CalendarView.Hidden = true;
            }
            else
            {
                view_custom_CalendarView.Hidden = false;
                view_custom_CalendarView.AddSubview(custom_CalendarView);
                BT_Apply.Hidden = true;
                nfloat heightCalenda = UIScreen.MainScreen.Bounds.Height * ((float)297 / 736);

                view_custom_CalendarView.Hidden = false;
                view_custom_CalendarView.AddSubview(custom_CalendarView);
                custom_CalendarView.InitFrameView(new CGRect(15, UIScreen.MainScreen.Bounds.Height / 2 - heightCalenda / 2, UIScreen.MainScreen.Bounds.Width - 30, heightCalenda));

            }
        }

        #region
        //private bool Check_FiltedStatus_toMe(DateTime _fromDateSelected, DateTime _toDateSelected )
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
        #endregion
        #endregion
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
                        if (tab_toMe)
                            FilterServerTodo(true);
                        else
                            FilterServerFromMe(true);
                    }
                    else
                    {
                        if (tab_toMe)
                            LoadMoreDataTodo();
                        else
                            LoadMoreDataFromMe();
                    }
                    indicator_loadmore.StopAnimating();
                    view_loadmore.Hidden = true;
                });
            });
        }

        #region fromMe
        /// <summary>
        /// filter different default then get from server
        /// </summary>
        private void FilterServerFromMe(bool isLoadMoreDataFilter)
        {
            try
            {
                var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                string query = string.Empty;

                dict_workflow = new Dictionary<string, List<BeanAppBaseExt>>();// dung duoi sortlist
                dict_sectionWorkFlow = new Dictionary<string, bool>();
                if (!isLoadMoreDataFilter)
                    lst_appBase_fromMe = new List<BeanAppBaseExt>();
                //Check Filter Status
                string str_status = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS); // tat ca fromme

                if (ListAppStatus_selected_fromMe != null && ListAppStatus_selected_fromMe.Count > 0) // co chon status
                    str_status = string.Join(',', ListAppStatus_selected_fromMe.Select(i => i.ID));

                ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();
                int totalRecord = 0;
                var listPropertiesFilter = CmmFunction.BuildListPropertiesFilter(CmmVariable.M_ResourceViewID_FromMe, "", str_status, DuedateSelected_fromMe.ID, temp_fromDateSelectedFromMe, temp_toDateSelectedFromMe);

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
                else
                {
                    lst_appBase_fromMe = _pControlDynamic.GetListFilterMyRequest(listPropertiesFilter, ref totalRecord, CmmVariable.M_DataFilterAPILimitData, 0);// lst_appBase_fromMe.Coun
                    lst_appBase_fromMe = lst_appBase_fromMe.OrderByDescending(s => s.Created).ToList();
                }

                if (lst_appBase_fromMe != null && lst_appBase_fromMe.Count > 0)
                {
                    fromMe_count = totalRecord;
                    loadData_count(fromMe_count, false);
                    SortListAppBaseFromMe("");
                }
                else
                {
                    fromMe_count = 0;
                    loadData_count(fromMe_count, false);
                    table_content.Alpha = 0;
                    lbl_nodata.Hidden = false;
                }
            }
            catch (Exception ex)
            {
                fromMe_count = 0;
                loadData_count(fromMe_count, false);
                table_content.Source = null;
                table_content.ReloadData();

                table_content.Alpha = 0;
                lbl_nodata.Hidden = false;

                Console.WriteLine("MainView - LoadDataFilterFromMe - Err: " + ex.ToString());
            }
        }

        /// <summary>
        /// get data when in local
        /// </summary>
        public void LoadDataFilterFromMe()
        {
            isOnline = Reachability.detectNetWork();
            dict_workflow = new Dictionary<string, List<BeanAppBaseExt>>();
            dict_sectionWorkFlow = new Dictionary<string, bool>();
            //lst_appBase_fromMe = new List<BeanAppBaseExt>();
            try
            {
                LoadDataFromMeOffLine();
                if (isOnline)
                    LoadDataFromMeOnLine();
                //else
            }
            catch (Exception ex)
            {
                fromMe_count = 0;
                loadData_count(fromMe_count, false);
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
            string queryCount = string.Empty;
            List<CountNum> lst_count_appBase_fromMe = new List<CountNum>();

            dict_workflow = new Dictionary<string, List<BeanAppBaseExt>>();
            dict_sectionWorkFlow = new Dictionary<string, bool>();

            ////count
            //queryCount = CreateQueryFromMe(true);
            //lst_count_appBase_fromMe = conn.Query<CountNum>(queryCount);
            //if (lst_count_appBase_fromMe != null && lst_count_appBase_fromMe.Count > 0)
            //{
            //    fromMe_count = lst_count_appBase_fromMe.First().count;
            //}
            //else
            //{
            //    fromMe_count = 0;
            //    loadData_count(fromMe_count, false);
            //}

            //data
            query = CreateQueryFromMe(false);
            lst_appBase_fromMe = conn.Query<BeanAppBaseExt>(query, CmmVariable.M_DataLimitRow, 0);
            if (lst_appBase_fromMe != null && lst_appBase_fromMe.Count > 0)
            {
                loadData_count(fromMe_count, false);
                SortListAppBaseFromMe(query);
                //table_content.ScrollToRow(NSIndexPath.FromItemSection(0, 0), UITableViewScrollPosition.Top, true);
                //table_content.ContentOffset = new CGPoint(0, -table_content.ContentInset.Top);
                table_content.ScrollRectToVisible(new CGRect(0, 0, 0, 0), false);
            }
            else
            {
                table_content.Alpha = 0;
                lbl_nodata.Hidden = false;
            }
        }

        void LoadDataFromMeOnLine()
        {
            //GetCountNumber();
            //loadData_count(fromMe_count, false);
            GetListFromMeOnline();
        }

        async void GetListFromMeOnline(bool isLoadMore = false)
        {
            dict_workflow = new Dictionary<string, List<BeanAppBaseExt>>();
            dict_sectionWorkFlow = new Dictionary<string, bool>();
            List<BeanAppBaseExt> lstObj = new List<BeanAppBaseExt>();
            if (!doReload && !isLoadMore) lst_appBase_fromMe = new List<BeanAppBaseExt>();

            await Task.Run(() =>
            {
                if (!isLoadMore)
                {
                    GetCountNumber();
                    InvokeOnMainThread(() =>
                    {
                        loadData_count(fromMe_count, false);
                        ToggleTodo();
                    });
                }
            });

            _ = Task.Run(() =>
            {
                try
                {
                    lstObj = new ProviderBase().LoadMoreDataTFromSerVer(CmmVariable.KEY_GET_FROMME_INPROCESS,
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
                            if (!tab_toMe)
                                SortListAppBaseFromMe("");
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
                List<BeanAppBaseExt> lst_appBase_fromMe_more = new List<BeanAppBaseExt>();
                dict_workflow = new Dictionary<string, List<BeanAppBaseExt>>();
                dict_sectionWorkFlow = new Dictionary<string, bool>();

                if (isOnline)
                {
                    GetListFromMeOnline(true);
                }
                else
                {
                    var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                    string query = string.Empty;
                    //data
                    query = CreateQueryFromMe(false);
                    lst_appBase_fromMe_more = conn.Query<BeanAppBaseExt>(query, CmmVariable.M_DataLimitRow, lst_appBase_fromMe.Count);

                    if (lst_appBase_fromMe_more != null && lst_appBase_fromMe_more.Count > 0)
                    {
                        lst_appBase_fromMe.AddRange(lst_appBase_fromMe_more);
                        SortListAppBaseFromMe(query);
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
                fromMe_count = 0;
                loadData_count(fromMe_count, false);
                table_content.Source = null;
                table_content.ReloadData();

                table_content.Alpha = 0;
                lbl_nodata.Hidden = false;

                Console.WriteLine("MainView - LoadDataFilterFromMe - Err: " + ex.ToString());
            }
        }

        private string CreateQueryFromMe(bool isGetCount)
        {
            #region kiểu cũ
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
            if (ListAppStatus_selected_fromMe != null && ListAppStatus_selected_fromMe.Count > 0) // co chon status
                str_status = string.Join(',', ListAppStatus_selected_fromMe.Select(i => i.ID));

            if (!string.IsNullOrEmpty(str_status))
                str_status = string.Format(@" AND AB.StatusGroup IN({0})", str_status);

            if (isGetCount)
                query = string.Format(@"SELECT Count(*) as count FROM BeanAppBase AB "
                    + "LEFT JOIN BeanAppStatus AST ON AST.ID = AB.StatusGroup "
                    + "WHERE AB.CreatedBy LIKE '%{0}%' {1}", CmmVariable.SysConfig.UserId.ToUpper(), str_status);
            else
                query = string.Format(@"SELECT AB.* FROM BeanAppBase AB "
                    + "LEFT JOIN BeanAppStatus AST ON AST.ID = AB.StatusGroup "
                    + "WHERE AB.CreatedBy LIKE '%{0}%' {1} "
                    + "ORDER BY AB.Created DESC LIMIT ? OFFSET ?", CmmVariable.SysConfig.UserId.ToUpper(), str_status);
            return query;*/
            #endregion

            return CmmIOSFunction.GetQueryStringAppBaseVTBD(true);
        }

        /// <summary>
        /// sort secsion from create
        /// </summary>
        /// <param name="query"></param>
        private void SortListAppBaseFromMe(string query)
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

                foreach (var item in lst_appBase_fromMe)
                {
                    if (item.Created.HasValue) // server, local sort Created
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

        private void ToggleMenuStatus()
        {
            custom_AppStatusCategory = Custom_AppStatusCategory.Instance;
            custom_AppStatusCategory.ItemNoIcon = false;
            custom_AppStatusCategory.CheckAll = true;
            custom_AppStatusCategory.viewController = this;
            custom_AppStatusCategory.LBL_inputView = tf_lbl_status;
            custom_AppStatusCategory.showItemClear = false;
            if (tab_toMe)
            {
                custom_AppStatusCategory.ListAppStatus = lst_appStatus_toMe;
                custom_AppStatusCategory.ListAppStatus_selected = ListAppStatus_selected_toMe;
            }
            else
            {
                custom_AppStatusCategory.ListAppStatus = lst_appStatus_fromMe;
                custom_AppStatusCategory.ListAppStatus_selected = ListAppStatus_selected_fromMe;
            }
            custom_AppStatusCategory.TableLoadData();
            custom_AppStatusCategory.BackupData();


            MultiChoiceTableView multiChoiceTableView = Storyboard.InstantiateViewController("MultiChoiceTableView") as MultiChoiceTableView;
            multiChoiceTableView.setContent(this, true, custom_AppStatusCategory, CmmFunction.GetTitle("TEXT_STATUS", "Tình trạng"));
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

        public void ClearViewStatus() // chi co tab den toi moi thuc hien 
        {
            HiddenViewStatus();
            LoadStatusCategory(lst_conditionMenu_toMe[0].isSelected ? CmmVariable.APPSTATUS_TOME_DANGXULY : CmmVariable.APPSTATUS_TOME_DAXULY);
            LoadDueDateCategory();
            //current status

            if ((tab_inprogress && lst_conditionMenu_toMe[0].isSelected) || (!tab_inprogress && lst_conditionMenu_toMe[1].isSelected)) //neu dang tro ve man hinh den toi thi lay old
            {
                //cap nhat select ListAppStatus_selected_toMe
                for (int i = 0; i < lst_appStatus_toMe.Count; i++)
                {
                    if (ListAppStatus_selected_toMe.Find(s => s.ID == lst_appStatus_toMe[i].ID) != null)
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
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        tf_lbl_status.Text = ListAppStatus_selected_toMe[0].TitleEN;
                    else
                        tf_lbl_status.Text = ListAppStatus_selected_toMe[0].Title;
                }
                else if (ListAppStatus_selected_toMe.Count > 1 && ListAppStatus_selected_toMe.Count <= lst_appStatus_toMe.Count - 1)
                {
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        tf_lbl_status.Text = ListAppStatus_selected_toMe[0].TitleEN;
                    else
                        tf_lbl_status.Text = ListAppStatus_selected_toMe[0].Title;

                    tf_lbl_status.Text += ",+(" + (ListAppStatus_selected_toMe.Count - 1).ToString() + ")";
                }
                else
                {
                    tf_lbl_status.Text = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
                }
                // danh lai lable duedate
                if (CmmVariable.SysConfig.LangCode == "1033")
                    lbl_dueDate.Text = DuedateSelected_toMe.titleEN;
                else
                    lbl_dueDate.Text = DuedateSelected_toMe.title;

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

                //truyen du lieu cho form custom
                var selectedStatus = lst_appStatus_toMe.FindAll(s => s.IsSelected == true);
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
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        tf_lbl_status.Text = selectedStatus[0].TitleEN;
                    else
                        tf_lbl_status.Text = selectedStatus[0].Title;
                }
                else if (selectedStatus.Count > 1 && selectedStatus.Count <= lst_appStatus_toMe.Count - 1)
                {
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        tf_lbl_status.Text = selectedStatus[0].TitleEN;
                    else
                        tf_lbl_status.Text = selectedStatus[0].Title;

                    tf_lbl_status.Text += ",+(" + (selectedStatus.Count - 1).ToString() + ")";
                }
                else
                {
                    tf_lbl_status.Text = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
                }
                //danh lai lable duedate
                if (CmmVariable.SysConfig.LangCode == "1033")
                    lbl_dueDate.Text = selectedDue.titleEN;
                else
                    lbl_dueDate.Text = selectedDue.title;

            }
        }

        private void HiddenViewStatus()
        {
            if (tab_toMe)
            {
                //if (lst_conditionMenu_toMe[0].isSelected)
                //{
                constraint_heighViewStatus.Constant = 0;
                //}
                //else
                //{
                //    constraint_heighViewStatus.Constant = 46;
                //}
            }
            else
            {
                constraint_heighViewStatus.Constant = 46;
            }

            isBackFromFilter = false;
        }

        private void ToggleMenuDueDate()
        {
            custom_DuedateCategory = Custom_DuedateCategory.Instance;
            custom_DuedateCategory.ItemNoIcon = true;
            custom_DuedateCategory.viewController = this;
            custom_DuedateCategory.LBL_inputView = lbl_dueDate;

            if (tab_toMe)
            {
                custom_DuedateCategory.ClassMenu_selected = DuedateSelected_toMe;
                custom_DuedateCategory.ListClassMenu = lst_dueDateMenu_toMe;
            }
            else
            {
                custom_DuedateCategory.ClassMenu_selected = DuedateSelected_fromMe;
                custom_DuedateCategory.ListClassMenu = lst_dueDateMenu_fromMe;
            }
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

        public void CloseConditionInstance()
        {
            Custom_AppConditionCategory custom_AppCondition = Custom_AppConditionCategory.Instance;
            if (custom_AppCondition.Superview != null)
                custom_AppCondition.RemoveFromSuperview();
        }

        //private bool Check_FiltedStatus_fromMe(DateTime _fromDateSelected, DateTime _toDateSelected)
        //{

        //    if (custom_AppStatusCategory != null) //custom_ConditionCategory
        //    {

        //        var itemSelect = ExtensionCopy.CopyAll(custom_AppStatusCategory.ListAppStatus.FindAll(s => s.IsSelected == true));
        //        int sum = 0;
        //        for (int i = 0; i < itemSelect.Count; i++)
        //        {
        //            if (ListAppStatus_selected_toMe.Find(s => s.ID == lst_appStatus_toMe[i].ID) != null)
        //                sum++;
        //        }
        //        if (sum != ListAppStatus_selected_toMe.Count)
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
            //if (tab_toMe)
            //    LoadDataFilterTodo();
            //else
            //    LoadDataFilterFromMe();
            ChangeTab();
        }
        #endregion

        #endregion

        #region events
        private void BT_todo_TouchUpInside(object sender, EventArgs e)
        {
            constraint_topView_status.Constant = heightTopViewStatus;
            view_stase.Hidden = false;

            BT_filter.TintColor = UIColor.FromRGB(0, 0, 0);
            previousColorFilter = UIColor.FromRGB(0, 0, 0);
            tab_toMe = true;
            //ToggleTodo();

            ChangeTab();
        }
        private void BT_fromMe_TouchUpInside(object sender, EventArgs e)
        {
            tab_inprogress = true;
            constraint_topView_status.Constant = 20;
            view_stase.Hidden = false;

            BT_filter.TintColor = UIColor.FromRGB(0, 0, 0);
            previousColorFilter = UIColor.FromRGB(0, 0, 0);
            tab_toMe = false;

            ChangeTab();
        }
        private void ChangeTab()
        {
            //table_content.SetContentOffset(new CGPoint( table_content.ContentOffset.X , -40), false);
            isLoadMore = true;
            isFilterServer = false;
            //init data
            LoadStatusCategory(tab_inprogress ? CmmVariable.APPSTATUS_TOME_DANGXULY : CmmVariable.APPSTATUS_TOME_DAXULY);
            LoadDueDateCategory();
            LoadConditionCategory();
            //load data
            LoadDuedataToMe();
            LoadStatusDataToMe();
            LoadConditionData();
            LoadDuedataFromMe();
            SetDateTime();
            tf_todate.Layer.BorderColor = UIColor.FromRGB(65, 80, 134).CGColor;
            tf_todate.Layer.BorderWidth = 0;

            tf_fromdate.Layer.BorderColor = UIColor.FromRGB(65, 80, 134).CGColor;
            tf_fromdate.Layer.BorderWidth = 0;
            LoadStatusDataFromMe();

            ToggleTodo();
            if (tab_toMe)
                LoadDataFilterTodo();
            else
                LoadDataFilterFromMe();
        }

        private void BT_filter_TouchUpInside(object sender, EventArgs e)
        {
            ToggleFilter();
        }

        private void SetValueFormFilter()
        {
            //hidden view_status
            HiddenViewStatus();
            tf_fromdate.TextColor = UIColor.FromRGB(0, 0, 0);
            tf_todate.TextColor = UIColor.FromRGB(0, 0, 0);

            if (tab_toMe)
            {
                // current date
                if (temp_fromDateSelectedToMe != default(DateTime))
                {
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        tf_fromdate.Text = temp_fromDateSelectedToMe.ToString(@"MM/dd/yyyy", new CultureInfo("en"));
                    else //if (CmmVariable.SysConfig.LangCode == "1066")
                        tf_fromdate.Text = temp_fromDateSelectedToMe.ToString(@"dd/MM/yyyy", new CultureInfo("vi"));

                }
                else
                {
                    tf_fromdate.Text = "";
                }
                if (temp_toDateSelectedToMe != default(DateTime))
                {
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        tf_todate.Text = temp_toDateSelectedToMe.ToString(@"MM/dd/yyyy", new CultureInfo("en"));
                    else //if (CmmVariable.SysConfig.LangCode == "1066")
                        tf_todate.Text = temp_toDateSelectedToMe.ToString(@"dd/MM/yyyy", new CultureInfo("vi"));

                }
                else
                    tf_todate.Text = "";

                //current status
                if (ListAppStatus_selected_toMe.Count == 1)
                {
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        tf_lbl_status.Text = ListAppStatus_selected_toMe[0].TitleEN;
                    else
                        tf_lbl_status.Text = ListAppStatus_selected_toMe[0].Title;
                }
                else if (ListAppStatus_selected_toMe.Count > 1 && ListAppStatus_selected_toMe.Count <= lst_appStatus_toMe.Count - 1)
                {
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        tf_lbl_status.Text = ListAppStatus_selected_toMe[0].TitleEN;
                    else
                        tf_lbl_status.Text = ListAppStatus_selected_toMe[0].Title;

                    tf_lbl_status.Text += ",+(" + (ListAppStatus_selected_toMe.Count - 1).ToString() + ")";
                }
                else //tat ca
                {
                    tf_lbl_status.Text = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
                }


                //current duedate
                if (CmmVariable.SysConfig.LangCode == "1033")
                    lbl_dueDate.Text = DuedateSelected_toMe.titleEN;
                else
                    lbl_dueDate.Text = DuedateSelected_toMe.title;

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
                //curent condition
                if (CmmVariable.SysConfig.LangCode == "1033")
                    lbl_State.Text = conditionSelected_toMe.titleEN;
                else
                    lbl_State.Text = conditionSelected_toMe.title;
            }
            else
            {
                // current date
                if (temp_fromDateSelectedFromMe != default(DateTime))
                {
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        tf_fromdate.Text = temp_fromDateSelectedFromMe.ToString(@"MM/dd/yyyy", new CultureInfo("en"));
                    else //if (CmmVariable.SysConfig.LangCode == "1066")
                        tf_fromdate.Text = temp_fromDateSelectedFromMe.ToString(@"dd/MM/yyyy", new CultureInfo("vi"));
                }
                else
                    tf_fromdate.Text = "";

                if (temp_toDateSelectedFromMe != default(DateTime))
                {
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        tf_todate.Text = temp_toDateSelectedFromMe.ToString(@"MM/dd/yyyy", new CultureInfo("en"));
                    else //if (CmmVariable.SysConfig.LangCode == "1066")
                        tf_todate.Text = temp_toDateSelectedFromMe.ToString(@"dd/MM/yyyy", new CultureInfo("vi"));
                    BT_todate.Layer.BorderWidth = 0;
                }
                else tf_todate.Text = "";
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
        }
        private void BT_avatar_TouchUpInside(object sender, EventArgs e)
        {
            if (isFilter)
                ToggleFilter();

            AppDelegate appD = UIApplication.SharedApplication.Delegate as AppDelegate;
            appD.menu.UpdateItemSelect(0);
            appD.SlideMenuController.OpenLeft();
        }
        private async void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                doReload = false;
                refreshControl.BeginRefreshing();
                ProviderBase provider = new ProviderBase();
                ProviderUser p_user = new ProviderUser();

                string documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string localpath = Path.Combine(documentFolder, CmmVariable.M_Avatar);

                await Task.Run(() =>
                {
                    isLoadMore = true;
                    if (!isFilterServer)
                    {
                        provider.UpdateAllMasterData(true);
                        provider.UpdateAllDynamicData(true);
                    }
                    p_user.UpdateCurrentUserInfo(localpath);

                    InvokeOnMainThread(() =>
                     {
                         refreshControl.EndRefreshing();
                         if (File.Exists(localpath))
                             BT_avatar.SetImage(UIImage.FromFile(localpath), UIControlState.Normal);

                         if (tab_toMe)
                         {
                             BT_todo.BackgroundColor = UIColor.White;
                             BT_todo.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);
                             BT_fromMe.BackgroundColor = UIColor.Clear;
                             BT_fromMe.SetTitleColor(UIColor.White, UIControlState.Normal);

                             if (isFilterServer)
                                 FilterServerTodo(false);
                             else
                                 LoadDataFilterTodo();
                         }
                         else
                         {
                             tab_toMe = false;
                             ToggleTodo();

                             if (isFilterServer)
                                 FilterServerFromMe(false);
                             else
                                 LoadDataFilterFromMe();
                         }
                     });
                });
            }
            catch (Exception ex)
            {
                refreshControl.EndRefreshing();
                Console.WriteLine("Error - MainView - refreshControl_valuechange - Er: " + ex.ToString());
            }
        }
        private void BT_createTicket_TouchUpInside(object sender, EventArgs e)
        {
            CreateTicketRequest createTicketRequest = (CreateTicketRequest)Storyboard.InstantiateViewController("CreateTicketRequest");
            this.NavigationController.PushViewController(createTicketRequest, true);
        }
        private void CmmEvent_UpdateLangComplete(object sender, CmmEvent.UpdateEventArgs e)
        {
            try
            {
                InvokeOnMainThread(() =>
                {
                    SetLangTitle();
                    table_content.ReloadData();
                    refreshControl.AttributedTitle = new NSAttributedString(CmmFunction.GetTitle("TEXT_REFRESH_LOADING", "Loading ..."));
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("MainView - CmmEvent_UpdateLangComplete - ERR: " + ex.ToString());
            }
        }

        //#region tab toMe
        //private void BT_status_all_TouchUpInside(object sender, EventArgs e)
        //{
        //    status_selected_index = 0;
        //    ToggleStatus_toMe();
        //}
        //private void BT_status_todo_TouchUpInside(object sender, EventArgs e)
        //{
        //    status_selected_index = 1;
        //    ToggleStatus_toMe();
        //}
        //private void BT_status_completed_TouchUpInside(object sender, EventArgs e)
        //{
        //    status_selected_index = 2;
        //    ToggleStatus_toMe();
        //}

        //private void BT_duedate_all_TouchUpInside(object sender, EventArgs e)
        //{
        //    duedate_selected_index = 0;
        //    ToggleDuedate();
        //}
        //private void BT_duedate_overdue_TouchUpInside(object sender, EventArgs e)
        //{
        //    duedate_selected_index = 1;
        //    ToggleDuedate();
        //}
        //private void BT_duedate_ontime_TouchUpInside(object sender, EventArgs e)
        //{
        //    duedate_selected_index = 2;
        //    ToggleDuedate();
        //}
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

        Custom_AppConditionCategory custom_ConditionCategory;

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

        private void BT_Apply_TouchUpInside(object sender, EventArgs e)
        {
            AppLyFilter();
        }
        private void AppLyFilter()
        {
            //kiem tra gia tri fromdate -> todate de set mau background bt_fillter
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

            // kiem tra ngay hop le khong(fromdate > todate)
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

                if (tab_toMe)
                {
                    temp_fromDateSelectedToMe = fromDate;
                    temp_toDateSelectedToMe = toDate;
                    if (custom_ConditionCategory != null)
                    {
                        if (lst_conditionMenu_toMe[0].isSelected) // do 2 view dang dung chung dang xu ly va da xu ly nen nhung luc khong thao tac thi bien select khong duoc cap nhat
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

                    FilterServerTodo(false);
                }
                else
                {
                    temp_fromDateSelectedFromMe = fromDate;
                    temp_toDateSelectedFromMe = toDate;

                    if (custom_AppStatusCategory != null)
                        ListAppStatus_selected_fromMe = custom_AppStatusCategory.ListAppStatus_selected;
                    if (custom_DuedateCategory != null)
                        DuedateSelected_fromMe = custom_DuedateCategory.ClassMenu_selected;
                    if (custom_ConditionCategory != null)
                        conditionSelected_toMe = custom_ConditionCategory.ClassMenu_selected;

                    FilterServerFromMe(false);
                }
                ToggleFilter();
            }
        }
        private void BT_reset_filter_TouchUpInside(object sender, EventArgs e)
        {

            ResetFilter();
        }
        private void ResetFilter()
        {
            tab_inprogress = true;
            isLoadMore = true;
            isFilterServer = false;
            BT_filter.TintColor = UIColor.FromRGB(0, 0, 0);
            previousColorFilter = UIColor.FromRGB(0, 0, 0);
            //init data
            LoadStatusCategory(CmmVariable.APPSTATUS_TOME_DANGXULY); // luc nao cung tra ve dang xu ly
            LoadDueDateCategory();
            LoadConditionCategory();
            //load data
            LoadConditionData();
            SetDateTime();
            if (tab_toMe)
            {
                LoadDuedataToMe();
                LoadStatusDataToMe();
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
                ResetSetValueFormFilter(ListAppStatus_selected_toMe, DuedateSelected_toMe, conditionSelected_toMe);
                LoadDataFilterTodo();
            }
            else
            {
                LoadDuedataFromMe();
                LoadStatusDataFromMe();
                LoadStatusCategory(CmmVariable.APPSTATUS_TOME_DANGXULY); // luc nao cung tra ve dang xu ly
                LoadDueDateCategory();
                LoadConditionCategory();
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
                ResetSetValueFormFilter(ListAppStatus_selected_fromMe, DuedateSelected_fromMe, null);
                LoadDataFilterFromMe();
            }
            HiddenViewStatus();
            ToggleFilter();
        }
        private void ResetSetValueFormFilter(List<BeanAppStatus> selectStatus, ClassMenu selectDuedate, ClassMenu condition)
        {
            if (CmmVariable.SysConfig.LangCode == "1033")
                tf_fromdate.Text = DateTime.Now.AddDays(-CmmVariable.M_DataFilterDefaultDays).ToString(@"MM/dd/yyyy", new CultureInfo("en"));
            else //if (CmmVariable.SysConfig.LangCode == "1066")
                tf_fromdate.Text = DateTime.Now.AddDays(-CmmVariable.M_DataFilterDefaultDays).ToString(@"dd/MM/yyyy", new CultureInfo("vi"));

            if (CmmVariable.SysConfig.LangCode == "1033")
                tf_todate.Text = DateTime.Now.ToString(@"MM/dd/yyyy", new CultureInfo("en"));
            else // if (CmmVariable.SysConfig.LangCode == "1066")
                tf_todate.Text = DateTime.Now.ToString(@"dd/MM/yyyy", new CultureInfo("vi"));

            tf_fromdate.Layer.BorderColor = UIColor.FromRGB(65, 80, 134).CGColor;
            tf_fromdate.Layer.BorderWidth = 0;

            tf_todate.Layer.BorderColor = UIColor.FromRGB(65, 80, 134).CGColor;
            tf_todate.Layer.BorderWidth = 0;
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
                else if (selectStatus.Count > 1 && selectStatus.Count <= (tab_toMe ? lst_appStatus_toMe.Count - 1 : lst_appStatus_fromMe.Count - 1))
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

            if (condition != null)
            {
                if (CmmVariable.SysConfig.LangCode == "1033")
                    lbl_State.Text = condition.titleEN;
                else
                    lbl_State.Text = condition.title;

            }
        }

        private void CmmEvent_SyncDataBackgroundRequest(object sender, CmmEvent.SyncDataRequest e)
        {
            InvokeOnMainThread(() =>
            {
                if (e.isDone)
                {
                    listView_indicator.Hidden = true;
                }
                else
                {
                    listView_indicator.Hidden = false;
                }
                CmmEvent.SyncDataBackgroundResultRequest -= CmmEvent_SyncDataBackgroundRequest;
            });
        }
        #endregion
        #endregion

        #region custom class

        #region todo toMe data source table
        private class Todo_TableSource : UITableViewSource
        {
            List<BeanNotify> lst_todo;
            NSString cellIdentifier = new NSString("cell");
            MainView parentView;
            int limit = 20;
            string queryFillter;
            bool isLoadMore = true;
            /// <summary>
            /// 
            /// </summary>
            bool isFilterServer = false;
            public static Dictionary<string, List<BeanNotify>> indexedCateSession;
            Dictionary<string, List<BeanAppBaseExt>> dict_todo { get; set; }
            Dictionary<string, bool> dict_section { get; set; }

            public Todo_TableSource(Dictionary<string, List<BeanAppBaseExt>> _dict_todo, MainView _parentview, string query)
            {
                dict_todo = _dict_todo;
                parentView = _parentview;
                queryFillter = query;
                isFilterServer = parentView.isFilterServer;
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
                        return 0;
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
                        CGRect frame = new CGRect(0, 0, tableView.Frame.Width, 44);

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
                //neu la secsion dau tien
                if (indexPath.Section == 0)
                {
                    // kiem tra chan le
                    if (indexPath.Row % 2 == 0)
                        isOdd = false;
                }
                else
                {
                    //sum item of cac section truoc
                    int sumCout = 0;
                    for (int i = 0; i < indexPath.Section; i++)
                    {
                        var keySum = dict_section.ElementAt(i).Key;
                        var todoSum = dict_todo[keySum];
                        if (todoSum != null && todoSum.Count > 0)
                            sumCout += todoSum.Count;
                    }
                    sumCout += 1;
                    sumCout += indexPath.Row;// cong them gia tri hien tai
                    if (sumCout % 2 != 0)// kiem tra chan le
                        isOdd = false;
                }

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

        #region WorkFlowItem data source table
        private class WorkFlow_TableSource : UITableViewSource
        {
            List<BeanWorkflowItem> lst_workflow;
            List<BeanWorkflowItem> fromMe_today;
            List<BeanWorkflowItem> fromMe_yesterday;
            public static Dictionary<string, List<BeanWorkflowItem>> indexedCateSession;
            Dictionary<string, List<BeanAppBaseExt>> dict_workflow { get; set; }
            Dictionary<string, bool> dict_section { get; set; }
            List<string> sectionKeys;
            List<bool> sectionState;
            NSString cellIdentifier = new NSString("cell");
            MainView parentView;

            string query = "";
            int limit = 20;
            bool isLoadMore = true;
            bool isFilterServer = false;

            public WorkFlow_TableSource(Dictionary<string, List<BeanAppBaseExt>> _dict_workflow, MainView _parentview, string _query)
            {
                dict_workflow = _dict_workflow;
                parentView = _parentview;
                query = _query;
                isFilterServer = parentView.isFilterServer;
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
                //neu la secsion dau tien
                if (indexPath.Section == 0)
                {
                    // kiem tra chan le
                    if (indexPath.Row % 2 == 0)
                        isOdd = false;
                }
                else
                {
                    //sum item of cac section truoc
                    int sumCout = 0;
                    for (int i = 0; i < indexPath.Section; i++)
                    {
                        var keySum = dict_section.ElementAt(i).Key;
                        var todoSum = dict_workflow[keySum];
                        if (todoSum != null && todoSum.Count > 0)
                            sumCout += todoSum.Count;
                    }
                    sumCout += indexPath.Row;// cong them gia tri hien tai
                    if (sumCout % 2 == 0)// kiem tra chan le
                        isOdd = false;
                }

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


using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.iPad.CustomControlClass;
using BPMOPMobile.iPad.IOSClass;
using BPMOPMobile.iPad.ViewControllers;
using CoreGraphics;
using Foundation;
using SlideMenuControllerXamarin;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UIKit;

namespace BPMOPMobile.iPad
{
    public partial class MenuView : UIViewController
    {
        AppDelegate appD;
        MainView mainView { get; set; }
        WorkflowDetailView workflowDetailView { get; set; }
        ToDoDetailView toDoDetailView { get; set; }
        BroadView broadView { get; set; }
        CmmLoading loading;
        bool IsFirstLoad = true;
        ClassMenu currentMenu { get; set; }
        Dictionary<int, List<ClassMenu>> list_menuItem = new Dictionary<int, List<ClassMenu>>();
        string str_vcxl_count, str_myrequestTicket_count, str_follow_count;//str_requestTicket,
        int indexMenuSelected = 0, vcxl_count = 0, myrequest_count = 0, follow_count = 0;

        public MenuView(IntPtr handle) : base(handle)
        {
            appD = UIApplication.SharedApplication.Delegate as AppDelegate;
        }

        public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (!IsFirstLoad)
            {
                await Task.Run(() =>
                {
                    InvokeOnMainThread(() =>
                    {
                        loadData_count();
                    });
                });
                LoadContent();
            }
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            if (IsFirstLoad)
            {
                IsFirstLoad = false;
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewConfiguration();
            setlangTitle();
            loadData_count();
            LoadContent();
            currentMenu = list_menuItem[0][0];

            #region delegate
            CmmEvent.UpdateLangComplete += CmmEvent_UpdateLangComplete;
            #endregion
        }

        #region public - private method
        public void SetContent(MainView _mainView)
        {
            mainView = _mainView;
        }

        private void ViewConfiguration()
        {
            iv_avatar.ContentMode = UIViewContentMode.ScaleAspectFill;
            LoadAvatar();
        }

        private void LoadContent()
        {
            CreateCircle();
            table_menu.ContentInset = new UIEdgeInsets(0, 0, 0, 0);

            if (CmmVariable.SysConfig != null)
            {
                lbl_name.Text = CmmVariable.SysConfig.DisplayName;

                if (!string.IsNullOrEmpty(CmmVariable.SysConfig.PositionTitle))
                    lbl_mail.Text = CmmVariable.SysConfig.PositionTitle;
            }
            LoadAvatar();
            InitMenuItem();
        }

        private void CreateCircle()
        {
            try
            {
                double min = Math.Min(iv_avatar.Frame.Width, iv_avatar.Frame.Height);
                iv_avatar.Layer.CornerRadius = (float)(min / 2.0);
                iv_avatar.Layer.MasksToBounds = false;
                iv_avatar.Layer.BorderColor = UIColor.LightGray.ColorWithAlpha(0.5f).CGColor;
                iv_avatar.Layer.BorderWidth = 0.5f;
                iv_avatar.ClipsToBounds = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("MenuView - CreateCircle - Err: " + ex.ToString());
            }
        }

        private void LoadAvatar()
        {
            string documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string localpath = Path.Combine(documentFolder, CmmVariable.M_Avatar);
            if (File.Exists(localpath))
                iv_avatar.Image = UIImage.FromFile(localpath);
            else
                iv_avatar.Image = UIImage.FromFile("Icons/icon_profile.png"); Console.WriteLine("Err: " + localpath);
        }

        private void InitMenuItem()
        {
            ClassMenu m1 = new ClassMenu() { ID = 0, section = 0, title = CmmFunction.GetTitle("TEXT_MAINVIEW", "Trang chủ"), iconUrl = "icon_home30.png", isSelected = (indexMenuSelected == 0 ? true : false) };
            ClassMenu m2 = new ClassMenu() { ID = 1, section = 0, title = CmmFunction.GetTitle("TEXT_TOME", "Đến tôi"), iconUrl = "icon_myRequest30.png", isSelected = (indexMenuSelected == 1 ? true : false), count = str_vcxl_count };
            ClassMenu m3 = new ClassMenu() { ID = 2, section = 0, title = CmmFunction.GetTitle("TEXT_FROMME", "Tôi bắt đầu"), iconUrl = "icon_Approve30.png", isSelected = (indexMenuSelected == 2 ? true : false), count = str_myrequestTicket_count };//
            ClassMenu m4 = new ClassMenu() { ID = 3, section = 0, title = CmmFunction.GetTitle("TEXT_BOARD", "Board"), iconUrl = "icon_Board30.png", isSelected = (indexMenuSelected == 3 ? true : false), count = "0" };
            var m13 = new ClassMenu { ID = 13, section = 0, title = CmmVariable.SysConfig.LangCode == "1033" ? "Follow" : "Theo dõi", iconUrl = "icon_follow.png", isSelected = false, count = str_follow_count };
            ClassMenu m5 = new ClassMenu() { ID = 4, section = 1, title = CmmFunction.GetTitle("TEXT_APPINFO", "Thông tin ứng dụng"), iconUrl = "icon_AppInfo30.png", count = appD.app_ver.ToString() };
            ClassMenu m6 = new ClassMenu() { ID = 5, section = 1, title = CmmFunction.GetTitle("TEXT_LANGUAGE", "Tiếng Việt"), iconUrl = "icon_language.png", count = CmmVariable.SysConfig.LangCode };
            ClassMenu m7 = new ClassMenu() { ID = 6, section = 2, title = CmmFunction.GetTitle("TEXT_SIGNOUT", "Đăng xuất"), iconUrl = "icon_signout30.png" };
            var mDeact = new ClassMenu() { ID = 99, section = 2, title = CmmVariable.SysConfig.LangCode == "1033" ? "Deactivate Account" : "Vô hiệu hóa tài khoản", iconUrl = "icon_deactivateUser.png" };//icon_signout30

            List<ClassMenu> list_menuItem0 = new List<ClassMenu>();
            list_menuItem0.Add(m1);
            list_menuItem0.Add(m2);
            list_menuItem0.Add(m3);
            list_menuItem0.Add(m13);
            list_menuItem0.Add(m4);

            List<ClassMenu> list_menuItem1 = new List<ClassMenu>();
            list_menuItem1.Add(m5);
            list_menuItem1.Add(m6);

            List<ClassMenu> list_menuItem2 = new List<ClassMenu>();
            if (string.Compare(CmmVariable.M_Domain, CmmVariable.M_Domain_develop) == 0)
                list_menuItem2.Add(mDeact);
            list_menuItem2.Add(m7);

            list_menuItem = new Dictionary<int, List<ClassMenu>>();

            list_menuItem.Add(0, list_menuItem0);
            list_menuItem.Add(1, list_menuItem1);
            list_menuItem.Add(2, list_menuItem2);

            table_menu.Source = new Menu_TableSource(list_menuItem, this);
            table_menu.ReloadData();

            currentMenu = m1;
        }

        public async void loadData_count()
        {
            try
            {
                str_follow_count = "";
                str_myrequestTicket_count = "";
                str_vcxl_count = "";
                await Task.Run(() =>
                {
                    GetCountNumber();
                });

                // danh sách yêu cầu cần xử lý default
                //var conn = new SQLiteConnection(CmmVariable.M_DataPath);

                //string strDefaultInProgessToDo = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS_TOME_DANGXULY); // tat ca

                #region yêu cầu cần xử lý: bỏ qua Chưa, Đang, Tạm hoãn
                /*if (!string.IsNullOrEmpty(strDefaultInProgessToDo))
                    strDefaultInProgessToDo = string.Format(@" AND AB.StatusGroup IN({0})", strDefaultInProgessToDo);

                string query_vcxl = string.Format(@"SELECT Count(*) as count  "
                    + "FROM BeanAppBase AB "
                    + "INNER JOIN BeanNotify NOTI "
                    + "ON AB.ID = NOTI.SPItemId "
                    + "WHERE (AB.AssignedTo LIKE '%{0}%' OR AB.NotifiedUsers LIKE '%{0}%') {1} "
                    + "AND NOTI.Type = 1 AND NOTI.Status = 0", CmmVariable.SysConfig.UserId.ToUpper(), strDefaultInProgessToDo);

                List<CountNum> countnum_vcxl = conn.Query<CountNum>(query_vcxl);
                if (countnum_vcxl != null)
                    vcxl_count = countnum_vcxl[0].count;*/

                if (vcxl_count >= 100)
                    str_vcxl_count = "99 +";
                else if (vcxl_count > 0 && vcxl_count < 100)
                {
                    if (vcxl_count > 0 && vcxl_count < 10)
                        str_vcxl_count = "0" + vcxl_count.ToString(); // hien thi 2 so vd: 08
                    else
                        str_vcxl_count = vcxl_count.ToString();
                }
                else
                    str_vcxl_count = "";
                #endregion

                #region yêu cầu của tôi: 
                /*string strDefaultFromMe = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS_FROMME); // tat ca
                if (!string.IsNullOrEmpty(strDefaultFromMe))
                    strDefaultFromMe = string.Format(@" AND AB.StatusGroup IN({0})", strDefaultFromMe);

                string query_today = string.Format(@"SELECT Count(*) as count "
                    + "FROM BeanAppBase AB LEFT JOIN BeanAppStatus AST ON AST.ID = AB.StatusGroup "
                    + "WHERE AB.CreatedBy LIKE '%{0}%' {1}"
                    , CmmVariable.SysConfig.UserId.ToUpper(), strDefaultFromMe);

                List<CountNum> countnum_myRequest = conn.Query<CountNum>(query_today);
                if (countnum_myRequest != null)
                    myrequest_count = countnum_myRequest[0].count;*/

                if (myrequest_count >= 100)
                    str_myrequestTicket_count = "99+";
                else if (myrequest_count > 0 && myrequest_count < 100)
                {
                    if (myrequest_count > 0 && myrequest_count < 10)
                        str_myrequestTicket_count = "0" + myrequest_count.ToString();// hien thi 2 so vd: 08
                    else
                        str_myrequestTicket_count = myrequest_count.ToString();
                }
                else
                    str_myrequestTicket_count = "";
                #endregion

                #region phê duyệt yêu cầu (yêu cầu vcxl đã duyệt và chưa duyệt)
                //string query_overdue = string.Format("SELECT Count(*) as count FROM BeanWorkflowItem WHERE CreatedBy <> '{0}' ORDER BY Created", CmmVariable.SysConfig.LoginName);
                //List<CountNum> countnum_request = conn.Query<CountNum>(query_overdue, DateTime.Now.Date.AddDays(1));
                //int requestTicket_count = 0;

                //if (countnum_request != null)
                //    requestTicket_count = countnum_request[0].count;

                //if (requestTicket_count >= 100)
                //    str_requestTicket = "99";
                //else if (requestTicket_count > 0 && requestTicket_count <= 100)
                //    str_requestTicket = requestTicket_count.ToString();
                //else
                //    str_requestTicket = "";
                #endregion

                #region Danh sách phiếu theo dõi
                //string strDefaultFollowToDo = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS_TOME_DANGXULY); // tat ca
                //if (!string.IsNullOrEmpty(strDefaultInProgessToDo))
                //    strDefaultInProgessToDo = string.Format(@" AND AB.StatusGroup IN({0})", strDefaultInProgessToDo);

                //string query_follow = string.Format(@"SELECT Count(*) as count "
                //+ "FROM BeanAppBase AB "
                //+ "INNER JOIN (SELECT StartDate, Read, SPItemId, SubmitAction, SendUnit, Type, Status FROM BeanNotify GROUP BY SPItemId) NOTI ON AB.ID = NOTI.SPItemId "
                //+ "WHERE (((AB.CreatedBy LIKE '%{0}%'OR AB.NotifiedUsers LIKE '%{0}%') AND AB.StatusGroup <> 256) "//Check đã xử lý
                //+ "OR (NOTI.Type = 1 AND NOTI.Status = 0 AND (AB.AssignedTo LIKE '%{0}%' OR AB.NotifiedUsers LIKE '%{0}%')))"//Check đang xử lý
                //+ "AND (EXISTS (SELECT 1 FROM BeanWorkflowFollow FF WHERE FF.WorkflowItemId = AB.ID AND FF.Status = 1)) "//Check isfollow
                //, CmmVariable.SysConfig.UserId.ToUpper());

                //List<CountNum> countnum_follow = conn.Query<CountNum>(query_follow);
                //if (countnum_follow != null && countnum_follow.Count > 0)
                //    follow_count = countnum_follow[0].count;

                if (follow_count >= 100)
                    str_follow_count = "99+";
                else if (follow_count > 0 && follow_count < 100)
                {
                    if (follow_count < 10)
                        str_follow_count = "0" + follow_count.ToString(); // hien thi 2 so vd: 08
                    else
                        str_follow_count = follow_count.ToString();
                }
                else
                    str_follow_count = "";
                #endregion

                InitMenuItem();
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

                count = new ProviderControlDynamic().GetListCountVDT_VTBD(CmmVariable.KEY_COUNT_FROMME_INPROCESS + "|" + CmmVariable.KEY_COUNT_ASIGNTOME_INPROCESS + "|" + CmmVariable.KEY_COUNT_FOLLOW);

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
                                if (!int.TryParse(str[1], out vcxl_count))
                                {
                                    vcxl_count = 0;
                                }
                            }
                            else if (string.Compare(str[0], CmmVariable.KEY_COUNT_FROMME_INPROCESS) == 0)
                            {
                                if (!int.TryParse(str[1], out myrequest_count))
                                {
                                    myrequest_count = 0;
                                }
                            }
                            else if (string.Compare(str[0], CmmVariable.KEY_COUNT_FOLLOW) == 0)
                            {
                                if (!int.TryParse(str[1], out follow_count))
                                {
                                    follow_count = 0;
                                }
                            }
                        }
                    }
                    return;
                }
                else
                {
#if DEBUG
                    Console.WriteLine("GetCountNumber trả về chuỗi trống.");
#endif
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("GetCountNumber - Err: " + ex.ToString());
#endif
            }
            vcxl_count = 0;
            myrequest_count = 0;
            follow_count = 0;
        }

        public async void changeLanguage()
        {
            try
            {
                loading = new CmmLoading(new CGRect((this.NavigationController.View.Bounds.Width - 200) / 2, (this.NavigationController.View.Bounds.Height - 200) / 2, 200, 200), "Đang xử lý...");
                this.NavigationController.Add(loading);

                ProviderUser p_app = new ProviderUser();
                bool result = false;

                await Task.Run(() =>
                {
                    if (File.Exists(CmmVariable.M_DataLangPath))
                    {
                        result = p_app.UpdateLangData(CmmVariable.SysConfig.LangCode, false, true);
                    }
                    else
                    {
                        result = p_app.UpdateLangData(CmmVariable.SysConfig.LangCode, false, true);
                    }

                    InvokeOnMainThread(() =>
                    {
                        if (result)
                            CmmEvent.UpdateLangComplete_Performence(null, null);
                        else
                            loading.Hide();
                    });
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("MenuView - changeLanguage - Err:" + ex.ToString());
                loading.Hide();
            }
        }

        private void setlangTitle()
        {
            CmmIOSFunction.SetLangToView(this.View);
        }

        private void NavigationTo(ClassMenu menuaction, NSIndexPath index)
        {
            if (menuaction.section == 0)
            {
                if (currentMenu != null)
                    currentMenu.isSelected = false;

                currentMenu = menuaction;
                currentMenu.isSelected = true;

                table_menu.ReloadData();
            }

            switch (menuaction.ID)
            {
                case 0: // trang chủ
                    if (appD.SlideMenuController.MainViewController.GetType() != typeof(MainView))
                    {
                        appD.SlideMenuController.ChangeMainViewcontroller(mainView, true);
                        ButtonsActionBotBar buttonActionBotBar = ButtonsActionBotBar.Instance;
                        buttonActionBotBar.LoadStatusButton(0);
                    }

                    appD.SlideMenuController.CloseLeft();
                    break;
                case 1: // đến tôi
                    if (toDoDetailView == null)
                        toDoDetailView = (ToDoDetailView)mainView.Storyboard.InstantiateViewController("ToDoDetailView");

                    if (appD.SlideMenuController.MainViewController.GetType() != typeof(ToDoDetailView))
                    {
                        appD.SlideMenuController.ChangeMainViewcontroller(toDoDetailView, true);
                        ButtonsActionBotBar buttonActionBotBar = ButtonsActionBotBar.Instance;
                        buttonActionBotBar.LoadStatusButton(1);
                    }

                    appD.SlideMenuController.CloseLeft();
                    break;
                case 2: // tôi bắt đầu
                    if (workflowDetailView == null)
                        workflowDetailView = (WorkflowDetailView)mainView.Storyboard.InstantiateViewController("WorkflowDetailView");

                    if (appD.SlideMenuController.MainViewController.GetType() != typeof(WorkflowDetailView))
                    {
                        appD.SlideMenuController.ChangeMainViewcontroller(workflowDetailView, true);
                        ButtonsActionBotBar buttonActionBotBar = ButtonsActionBotBar.Instance;
                        buttonActionBotBar.LoadStatusButton(2);
                    }

                    appD.SlideMenuController.CloseLeft();
                    break;
                case 3: // board
                    if (broadView == null)
                        broadView = (BroadView)mainView.Storyboard.InstantiateViewController("BroadView");

                    if (appD.SlideMenuController.MainViewController.GetType() != typeof(BroadView))
                    {
                        appD.SlideMenuController.ChangeMainViewcontroller(broadView, true);
                        ButtonsActionBotBar buttonActionBotBar = ButtonsActionBotBar.Instance;
                        buttonActionBotBar.LoadStatusButton(3);
                    }
                    appD.SlideMenuController.CloseLeft();
                    break;
                case 6: // đăng xuất
                    SignOut();
                    break;
                case 99:
                    Deactivate();
                    break;
                default:
                    appD.SlideMenuController.CloseLeft();
                    break;
            }
        }
        #endregion

        #region events
        private void CmmEvent_UpdateLangComplete(object sender, CmmEvent.UpdateEventArgs e)
        {
            try
            {
                InvokeOnMainThread(() =>
                {
                    InitMenuItem();
                    setlangTitle();
                    loading.Hide();
                });
            }
            catch (Exception ex)
            {
                if (loading != null)
                    loading.Hide();

                Console.WriteLine("MenuView - CmmEvent_UpdateLangComplete - ERR: " + ex.ToString());
            }
        }

        private void SignOut()
        {
            /*UIAlertView alert_signOut = new UIAlertView();
            alert_signOut.Title = "BPM OP";
            alert_signOut.Message = CmmFunction.GetTitle("TEXT_CONFIRM_SIGNOUT", "Bạn có muốn đăng xuất khỏi tài khoản? ");
            alert_signOut.AddButton(CmmFunction.GetTitle("TEXT_AGREE", "Đồng ý"));
            alert_signOut.AddButton(CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"));
            alert_signOut.Clicked += alert_signOut_Clicked;
            alert_signOut.Show();*/

            UIAlertController alert = UIAlertController.Create("Thông báo", CmmFunction.GetTitle("TEXT_CONFIRM_SIGNOUT", "Bạn có muốn đăng xuất khỏi tài khoản? "), UIAlertControllerStyle.Alert);//"BPM OP"
            alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_AGREE", "Đồng ý"), UIAlertActionStyle.Default, (action) =>
            {
                try
                {
                    if (mainView != null)
                        mainView.SignOut();
                    //CmmVariable.SysConfig.LangCode = "1033";
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SettingViewController - alert_signOut_Clicked - Err: " + ex.ToString());
                }
            }));
            alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"), UIAlertActionStyle.Default, null));
            this.appD.NavController.PresentViewController(alert, true, null);
        }

        void Deactivate()
        {
            //UIAlertController alert = UIAlertController.Create("VNA BPM", CmmVariable.SysConfig.LangCode == "1033" ? "Do you really want to deactivate this account?" : "Bạn có muốn vô hiệu hóa tài khoản?", UIAlertControllerStyle.Alert);
            //alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_AGREE", "Đồng ý"), UIAlertActionStyle.Default, (action) =>
            //{
            //    try
            //    {
            //        if (mainView != null)
            //            mainView.DeactivateAccount();
            //        appD.SlideMenuController.CloseLeft();
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine("SettingViewController - alert_signOut_Clicked - Err: " + ex.ToString());
            //    }
            //}));
            //alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"), UIAlertActionStyle.Default, null));
            //this.appD.NavController.PresentViewController(alert, true, null);

            //if (mainView != null)
            //    mainView.DeactivateAccount();

            DeactivateAccount();
            appD.SlideMenuController.CloseLeft();

        }

        /// <summary>
        /// Chức năng vô hiệu hóa dùng để pass Apple review.
        /// </summary>
        public void DeactivateAccount()
        {
            PresentationDelegate transitioningDelegate;
            DeactivateAccView deactivateAccView = (DeactivateAccView)Storyboard.InstantiateViewController("DeactivateAccView");
            deactivateAccView.SetContent(mainView);
            CGRect startFrame = new CGRect(UIScreen.MainScreen.Bounds.Width, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);//this.View.Frame.X, this.View.Frame.Height
            CGSize showSize = new CGSize(UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);
            transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            deactivateAccView.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;
            deactivateAccView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            deactivateAccView.TransitioningDelegate = transitioningDelegate;
            this.PresentViewControllerAsync(deactivateAccView, true);
        }

        void alert_signOut_Clicked(object sender, UIButtonEventArgs e)
        {
            try
            {
                if (e.ButtonIndex == 0)
                {
                    if (mainView != null)
                        mainView.SignOut();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MenuView - alert_signOut_Clicked - Err: " + ex.ToString());
            }
        }

        /// <summary>
        /// function cập nhật lại item được chọn ở menu 
        /// </summary>
        /// <param name="index">0: Trang chủ | 1: Đến tôi | 2: Tôi bắt đầu | 3: Broad | -1: Tạo mới</param>
        public void UpdateItemSelect(int index)
        {
            if (index != -1)
            {
                var itemSelect = list_menuItem[0][index];

                if (currentMenu != null)
                    currentMenu.isSelected = false;

                currentMenu = itemSelect;
                currentMenu.isSelected = true;
                indexMenuSelected = index;
            }
            else
            {
                if (currentMenu != null)
                    currentMenu.isSelected = false;
            }

            table_menu.ReloadData();
        }

        private void NavigateToFollowView()
        {
            appD = UIApplication.SharedApplication.Delegate as AppDelegate;
            try
            {
                FollowListViewController followListViewController = (FollowListViewController)Storyboard.InstantiateViewController("FollowListViewController");
                this.NavigationController.PushViewController(followListViewController, true);

                //UIAlertController alert = UIAlertController.Create("VNA BPM OP", "Tính năng đang phát triển", UIAlertControllerStyle.Alert);
                //alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_AGREE", "Đồng ý"), UIAlertActionStyle.Default, null));
                //this.appD.NavController.PresentViewController(alert, true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("MenuView - NavigateToFollowView: " + ex.ToString());
            }
            finally
            {
                appD.SlideMenuController.CloseLeft();
            }
        }
        #endregion

        #region custom class
        private class Menu_TableSource : UITableViewSource
        {
            static readonly NSString cellIdentifier = new NSString("menuCell");
            MenuView parentview;
            Dictionary<int, List<ClassMenu>> list_menuItem = new Dictionary<int, List<ClassMenu>>();

            public Menu_TableSource(Dictionary<int, List<ClassMenu>> _list_menuItem, MenuView controler)
            {
                parentview = controler;
                list_menuItem = _list_menuItem;
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return list_menuItem.Count;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                var numRow = list_menuItem[Convert.ToInt32(section)].Count;
                return numRow;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 60;
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                if (section == 0)
                    return 5;
                else
                    return 1;
            }

            public override UIView GetViewForHeader(UITableView tableView, nint section)
            {
                UIView view = new UIView();
                view.Frame = new CGRect(0, 0, parentview.View.Bounds.Width, parentview.View.Bounds.Width);
                UILabel line = new UILabel();
                line.BackgroundColor = UIColor.FromRGB(234, 237, 243);
                line.Frame = new CGRect(15, 0, parentview.View.Bounds.Width - 20, 1);
                view.Add(line);
                if (section == 0)
                    line.Hidden = true;

                return view;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                if (list_menuItem[Convert.ToInt32(indexPath.Section)][indexPath.Row].ID == 13)
                {
                    parentview.NavigateToFollowView();
                }
                else
                {
                    if (indexPath.Section == 0)
                        parentview.indexMenuSelected = indexPath.Row;

                    if (indexPath.Section == 0 || indexPath.Section == 2)
                    {
                        var item = list_menuItem[Convert.ToInt32(indexPath.Section)][indexPath.Row];
                        parentview.NavigationTo(item, indexPath);
                    }
                    tableView.DeselectRow(indexPath, true);
                }
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                Custom_menu_Cell cell = new Custom_menu_Cell(cellIdentifier, parentview);
                var item = list_menuItem[Convert.ToInt32(indexPath.Section)][indexPath.Row];
                cell.UpdateCell(item);
                return cell;

            }
        }

        public class Custom_menu_Cell : UITableViewCell
        {
            UILabel lbl_title, lbl_count, lbl_lineLeft;
            UIImageView ic;
            UISegmentedControl segment_language;
            ClassMenu menu { get; set; }
            MenuView parentview { get; set; }

            public Custom_menu_Cell(NSString cellID, MenuView _parent)
            {
                parentview = _parent;
                ViewConfiguration();
            }

            public void ViewConfiguration()
            {
                ContentView.BackgroundColor = UIColor.White;
                ic = new UIImageView();
                ic.ContentMode = UIViewContentMode.ScaleAspectFit;

                lbl_count = new UILabel()
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = UIFont.FromName("ArialMT", 15f),
                    TextColor = UIColor.FromRGB(0, 95, 212)
                };

                lbl_title = new UILabel
                {
                    TextAlignment = UITextAlignment.Left,
                    Font = UIFont.FromName("ArialMT", 15f),
                    TextColor = UIColor.FromRGB(0, 0, 0)// UIColor.FromRGB(94, 94, 94)
                };

                segment_language = new UISegmentedControl();
                segment_language.InsertSegment("VN", 0, false);
                segment_language.InsertSegment("EN", 1, false);
                segment_language.ValueChanged += delegate
                {
                    if (segment_language.SelectedSegment == 0)
                        CmmVariable.SysConfig.LangCode = "1066";
                    else
                        CmmVariable.SysConfig.LangCode = "1033";
                    CmmFunction.WriteSetting();

                    parentview.changeLanguage();
                    ProviderUser p_user = new ProviderUser();
                    p_user.UpdateUserLanguageChange(CmmVariable.SysConfig.LangCode);
                };

                lbl_lineLeft = new UILabel();
                lbl_lineLeft.BackgroundColor = UIColor.FromRGB(51, 95, 179);
                lbl_lineLeft.Hidden = true;

                ContentView.AddSubviews(new UIView[] { lbl_title, lbl_count, ic, lbl_lineLeft, segment_language });
            }

            public void UpdateCell(ClassMenu _menu)
            {
                menu = _menu;
                lbl_title.Text = menu.title;

                if (menu.iconUrl == "icon_language.png") // row - ngôn ngữ
                {
                    //if (menu.count == "1066")
                    //    segment_language.SelectedSegment = 0;
                    //else if (menu.count == "1033")
                    //    segment_language.SelectedSegment = 1;

                    segment_language.SelectedSegment = CmmVariable.SysConfig.LangCode.Equals("1033") ? 1 : 0;
                }
                else
                {
                    if (!string.IsNullOrEmpty(menu.count) && menu.count != "0")
                        lbl_count.Text = menu.count.ToString();
                    else
                        lbl_count.Hidden = true;
                }

                if (!string.IsNullOrEmpty(menu.iconUrl))
                    ic.Image = UIImage.FromFile("Icons/" + menu.iconUrl).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                if (menu.section == 0)
                {
                    if (menu.isSelected)
                    {
                        //ic.TintColor = UIColor.FromRGB(51, 95, 179);
                        lbl_title.TextColor = UIColor.FromRGB(0, 95, 212);// UIColor.FromRGB(51, 95, 179);
                        lbl_title.Font = UIFont.FromName("Arial-BoldMT", 15f);

                        //lbl_count.TextAlignment = UITextAlignment.Center;
                        //lbl_count.Font = UIFont.FromName("ArialMT", 14f);
                        //lbl_count.TextColor = UIColor.FromRGB(255, 122, 58);

                        lbl_lineLeft.Hidden = false;
                    }
                    else
                    {
                        //ic.TintColor = UIColor.FromRGB(94, 94, 94);
                        lbl_title.TextColor = UIColor.FromRGB(0, 0, 0);
                        lbl_title.Font = UIFont.FromName("ArialMT", 14f);
                    }
                }
                else
                {
                    if (menu.section == 2)
                    {
                        lbl_title.TextColor = UIColor.FromRGB(237, 28, 36);//UIColor.FromRGB(235, 52, 46)
                    }
                }
                ic.TintColor = lbl_title.TextColor;
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();

                lbl_lineLeft.Frame = new CGRect(0, 15, 6, ContentView.Frame.Height - 30);
                ic.Frame = new CGRect(25, 20, 20, 20);
                lbl_title.Frame = new CGRect(ic.Frame.Right + 20, 20, ContentView.Frame.Width - (ic.Frame.Right + 100), 20);
                if (menu.iconUrl == "icon_language.png")
                    segment_language.Frame = new CGRect(ContentView.Frame.Width - 120, 12, 110, 26);
                else
                    lbl_count.Frame = new CGRect(ContentView.Frame.Width - 70, 20, 50, 20);
            }
        }
        #endregion
    }
}
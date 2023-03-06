using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using CoreGraphics;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
//using BPMOPMobileV1.iOS.IOSClass;
using Foundation;
using SQLite;
using UIKit;
using System.Threading;
using BPMOPMobileV1.iOS.CustomControlClass;
using Xamarin.iOS;
using System.Globalization;
using BPMOPMobileV1.iOS.IOSClass;
using System.Linq;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class MenuView : UIViewController
    {
        AppDelegate appD = UIApplication.SharedApplication.Delegate as AppDelegate;
        Dictionary<int, List<ClassMenu>> list_menuItem = new Dictionary<int, List<ClassMenu>>();
        MainView mainView { get; set; }
        string str_vcxl_count, str_myrequestTicket_count, str_requestTicket, str_follow_count;
        CmmLoading loading;
        bool IsFirstLoad = true;
        ClassMenu currentMenu { get; set; }
        static string documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        string localpath = Path.Combine(documentFolder, CmmVariable.M_Avatar);
        int vcxl_count = 0, myrequest_count = 0, follow_count = 0;

        public MenuView(IntPtr handle) : base(handle)
        {
        }
        public int indexMenuSelected = 0;

        #region View override
        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (!IsFirstLoad)
            {
                loadData_count();
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
            initMenuItem();
            currentMenu = list_menuItem[0][0];

            loadData_count();
            LoadContent();

            #region delegate
            CmmEvent.UpdateLangComplete += CmmEvent_UpdateLangComplete;
            #endregion
        }

        #endregion

        #region private - public method
        public void SetContent(MainView _mainView)
        {
            mainView = _mainView;
        }

        private void ViewConfiguration()
        {
            //var ver = DeviceHardware.Version;
            //var model = DeviceHardware.Model;
            //if (model.Contains("X") || model.Contains("11"))
            //{
            //    headerView_constainHeight.Constant = 120;
            //}

            headerView_constainHeight.Constant = 80 + 10 + CmmIOSFunction.GetHeaderViewHeight();

            //string mStringUrl = "https://bpmon.vuthao.com/_layouts/15/eoffices/Login/imgBg.svg";
            //var htmlString = "<html>" +
            //    "<body style=\"height:100%; \">" +
            //        "<div style= \" display:flex; align-items:center; justify-content:center; -webkit-flex:1; -ms-flex:1; flex:1; text-align:center; width:100%; height:100%; \">" +
            //        "<img style= \"width:90%;\" src=\"" + mStringUrl + "\"></div>" +
            //    "</body></html>";
            //webkit_ad.LoadHtmlString(htmlString, null); //https://bpmon.vuthao.com/_layouts/15/eoffices/Login/imgBg.svg

            NSUrl bg_url = new NSUrl("Images/img_iphone_AdMenu.png", false);
            webkit_ad.LoadFileUrl(bg_url, bg_url);

            if (File.Exists(localpath))
                img_avatar.Image = UIImage.FromFile(localpath);
            else
                img_avatar.Image = UIImage.FromFile("Icons/icon_avatar64.png");
            img_avatar.ContentMode = UIViewContentMode.ScaleAspectFill;

            table_menu.ContentInset = new UIEdgeInsets(-20, 0, 0, 0);
            //this.View.BackgroundColor = UIColor.FromRGB(9, 171, 78);
        }

        private void LoadContent()
        {
            CreateCircle();
            table_menu.ContentInset = new UIEdgeInsets(20, 0, 0, 0);

            if (CmmVariable.SysConfig != null)
            {
                lbl_name.Text = CmmVariable.SysConfig.DisplayName;

                //if (!string.IsNullOrEmpty(CmmVariable.SysConfig.PositionTitle))
                lbl_email.Text = !string.IsNullOrEmpty(CmmVariable.SysConfig.PositionTitle) ? CmmVariable.SysConfig.PositionTitle : "";
                //else
                //    lbl_email.Text = CmmVariable.SysConfig.Email;
            }
        }

        public void loadData_count()
        {
            try
            {
                GetCountNumber();
                // danh sách yều cầu cần xử lý default
                //var conn = new SQLiteConnection(CmmVariable.M_DataPath);
                ////them dieu kien loc khong lay phieu co trang thai DRAF
                //string filter_draft_todo = string.Format("SubmitActionId <> 0");
                //string filter_draft_workflow = string.Format("ActionStatusId <> 0");

                #region yêu cầu cần xử lý: bỏ qua Chưa, Đang, Tạm hoãn
                /*string strDefaultInProgessToDo = CmmFunction.GetAppSettingValue(CmmVariable.APPSTATUS_TOME_DANGXULY); // tat ca
                if (!string.IsNullOrEmpty(strDefaultInProgessToDo))
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

                initMenuItem();
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

        private void initMenuItem()
        {
            ClassMenu m1 = new ClassMenu() { ID = 0, section = 0, title = CmmFunction.GetTitle("TEXT_MAINVIEW", "Trang chủ"), iconUrl = "icon_home30.png", isSelected = (indexMenuSelected == 0 ? true : false) };
            ClassMenu m2 = new ClassMenu() { ID = 1, section = 0, title = CmmFunction.GetTitle("TEXT_TOME", "Đến tôi"), iconUrl = "icon_myRequest30.png", isSelected = (indexMenuSelected == 1 ? true : false), count = str_vcxl_count };
            ClassMenu m3 = new ClassMenu() { ID = 2, section = 0, title = CmmFunction.GetTitle("TEXT_FROMME", "Tôi bắt đầu"), iconUrl = "icon_Approve30.png", isSelected = (indexMenuSelected == 2 ? true : false), count = str_myrequestTicket_count };
            //ClassMenu m4 = new ClassMenu() { ID = 3, section = 0, title = "Board", iconUrl = "icon_Board30.png", isSelected = indexMenuSelected == 3 ? true : false, count = "0" };
            ClassMenu m4 = new ClassMenu() { ID = 3, section = 0, title = CmmFunction.GetTitle("TEXT_BOARD", "Board"), iconUrl = "icon_Board30.png", isSelected = (indexMenuSelected == 3 ? true : false), count = "0" };
            var m8 = new ClassMenu { ID = 13, section = 0, title = CmmVariable.SysConfig.LangCode == "1033" ? "Follow" : "Theo dõi", iconUrl = "icon_follow.png", isSelected = false, count = str_follow_count };
            //ClassMenu m4 = new ClassMenu() { ID = 3, section = 0, title = CmmFunction.GetTitle("TEXT_APPLICATION", "Ứng dụng"), iconUrl = "icon_Board30.png", isSelected = (indexMenuSelected == 3 ? true : false), count = "0" };
            ClassMenu m5 = new ClassMenu() { ID = 4, section = 1, title = CmmFunction.GetTitle("TEXT_APPINFO", "Thông tin ứng dụng"), iconUrl = "icon_AppInfo30.png", count = appD.app_ver.ToString() };
            ClassMenu m6 = new ClassMenu() { ID = 5, section = 1, title = CmmFunction.GetTitle("TEXT_LANGUAGE", "Tiếng Việt"), iconUrl = "icon_language30.png", count = CmmVariable.SysConfig.LangCode };
            var mDeact = new ClassMenu() { ID = 99, section = 2, title = CmmVariable.SysConfig.LangCode == "1033" ? "Deactivate Account" : "Vô hiệu hóa tài khoản", iconUrl = "icon_deactivateUser.png" };//icon_signout30
            ClassMenu m7 = new ClassMenu() { ID = 6, section = 2, title = CmmFunction.GetTitle("TEXT_SIGNOUT", "Đăng xuất"), iconUrl = "icon_signout.png" };//icon_signout30

            List<ClassMenu> list_menuItem0 = new List<ClassMenu>();
            list_menuItem0.Add(m1);
            list_menuItem0.Add(m2);
            list_menuItem0.Add(m3);
            list_menuItem0.Add(m8);
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

            table_menu.Source = new menu_TableSource(list_menuItem, this);
            table_menu.ReloadData();

            table_menu_constaintHeight.Constant = table_menu.ContentSize.Height + 10;
        }

        public async void changeLanguage()
        {
            try
            {
                loading = new CmmLoading(new CGRect((this.View.Bounds.Width - 200) / 2, (this.View.Bounds.Height - 200) / 2, 200, 200), "Đang xử lý...");
                this.Add(loading);

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
                loading.Hide();
                Console.WriteLine("SettingViewController - changeLanguage - Err: " + ex.ToString());
            }
        }

        private void setlangTitle()
        {
            CmmIOSFunction.SetLangToView(this.View);
        }

        private void CreateCircle()
        {
            try
            {
                double min = Math.Min(img_avatar.Frame.Width, img_avatar.Frame.Height);
                img_avatar.Layer.CornerRadius = (float)(min / 2.0);
                img_avatar.Layer.MasksToBounds = false;
                img_avatar.Layer.BorderColor = UIColor.LightGray.ColorWithAlpha(0.5f).CGColor;
                img_avatar.Layer.BorderWidth = 0.5f;
                img_avatar.ClipsToBounds = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("SettingViewController - CreateCircle - Err: " + ex.ToString());
            }
        }

        private void NavigationTo(ClassMenu menuaction, NSIndexPath index)
        {
            appD = UIApplication.SharedApplication.Delegate as AppDelegate;

            if (index.Section == 0)
            {
                if (currentMenu != null)
                    currentMenu.isSelected = false;

                currentMenu = menuaction;
                currentMenu.isSelected = true;

                //var item = ExtensionCopy.CopyAll(list_menuItem[0]);
                //for(int i = 0; i < item.Count; i++)
                //{
                //    if(item[i].ID == menuaction.ID)
                //        item[i].isSelected = true;
                //    else
                //        item[i].isSelected = false;
                //}
                //list_menuItem[0] = item;
                //table_menu.ReloadData();

                if (menuaction.ID == 0) // trang chủ index.Row == 0
                {
                    MainView mainView = (MainView)Storyboard.InstantiateViewController("MainView");
                    appD.SlideMenuController.ChangeMainViewcontroller(mainView, true);
                    ButtonsActionBotBar buttonActionBotBar = ButtonsActionBotBar.Instance;
                    buttonActionBotBar.LoadStatusButton(0);
                    //buttonActionBotBar.UpdateChildBroad(false);
                    appD.SlideMenuController.CloseLeft();
                }

                // yêu cầu xử lý - danh sach Notify index.Row == 1
                if (menuaction.ID == 1)
                {
                    RequestListView requestListView = (RequestListView)Storyboard.InstantiateViewController("RequestListView");
                    requestListView.setContent(menuaction.title);

                    if (appD.SlideMenuController.MainViewController.GetType() != typeof(RequestListView))
                    {
                        appD.SlideMenuController.ChangeMainViewcontroller(requestListView, true);
                        ButtonsActionBotBar buttonActionBotBar = ButtonsActionBotBar.Instance;
                        buttonActionBotBar.LoadStatusButton(1);
                        //buttonActionBotBar.UpdateChildBroad(false);
                    }

                    appD.SlideMenuController.CloseLeft();
                }

                // yêu cầu của tôi - danh sach WorkFlowItem index.Row == 2
                if (menuaction.ID == 2)
                {
                    MyRequestListView myRequestListView = (MyRequestListView)Storyboard.InstantiateViewController("MyRequestListView");
                    myRequestListView.setContent(menuaction.title);

                    if (appD.SlideMenuController.MainViewController.GetType() != typeof(MyRequestListView))
                    {
                        appD.SlideMenuController.ChangeMainViewcontroller(myRequestListView, true);
                        ButtonsActionBotBar buttonActionBotBar = ButtonsActionBotBar.Instance;
                        buttonActionBotBar.LoadStatusButton(2);
                        //buttonActionBotBar.UpdateChildBroad(false);
                    }
                    appD.SlideMenuController.CloseLeft();
                }

                // Board index.Row == 4
                if (menuaction.ID == 3)
                {
                    BroadView broadView = (BroadView)Storyboard.InstantiateViewController("BroadView");
                    if (appD.SlideMenuController.MainViewController.GetType() != typeof(BroadView))
                    {
                        appD.SlideMenuController.ChangeMainViewcontroller(broadView, true);
                        ButtonsActionBotBar buttonActionBotBar = ButtonsActionBotBar.Instance;
                        buttonActionBotBar.LoadStatusButton(2);
                        //buttonActionBotBar.UpdateChildBroad(false);
                    }
                    appD.SlideMenuController.CloseLeft();
                }
            }
            else if (index.Section == 2)
            {
                if (menuaction.ID == 6) //index.Row == 0
                {
                    //UIAlertView alert_signOut = new UIAlertView();
                    //alert_signOut.Title = "BPM OP";
                    //alert_signOut.Message = CmmFunction.GetTitle("K_Mess_Logout", "Bạn có muốn đăng xuất khỏi tài khoản? ");
                    //alert_signOut.AddButton(CmmFunction.GetTitle("K_Confirm", "Đồng ý"));
                    //alert_signOut.AddButton(CmmFunction.GetTitle("K_Cancel", "Hủy"));
                    //alert_signOut.Clicked += alert_signOut_Clicked;
                    //alert_signOut.Show();

                    UIAlertController alert = UIAlertController.Create("Thông báo", CmmFunction.GetTitle("TEXT_CONFIRM_SIGNOUT", "Bạn có muốn đăng xuất khỏi tài khoản? "), UIAlertControllerStyle.Alert);//"VNA BPM"
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
                else if (menuaction.ID == 99)
                {
                    //UIAlertController alert = UIAlertController.Create("VNA BPM", CmmVariable.SysConfig.LangCode == "1033" ? "Do you really want to deactivate this account?" : "Bạn có muốn vô hiệu hóa tài khoản?", UIAlertControllerStyle.Alert);
                    //alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_AGREE", "Đồng ý"), UIAlertActionStyle.Default, (action) =>
                    //{
                    try
                    {
                        DeactivateAccount();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("SettingViewController - alert_signOut_Clicked - Err: " + ex.ToString());
                    }
                    appD.SlideMenuController.CloseLeft();
                    //}));
                    //alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"), UIAlertActionStyle.Default, null));
                    //this.appD.NavController.PresentViewController(alert, true, null);
                }
            }
        }

        /// <summary>
        /// Chức năng vô hiệu hóa dùng để pass Apple review.
        /// </summary>
        public void DeactivateAccount()
        {
            PresentationDelegate transitioningDelegate;
            DeactivateAccView deactivateAccView = (DeactivateAccView)Storyboard.InstantiateViewController("DeactivateAccView");
            deactivateAccView.SetContent(mainView);
            CGRect startFrame = new CGRect(UIScreen.MainScreen.Bounds.Width, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);
            CGSize showSize = new CGSize(UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);
            transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            deactivateAccView.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;
            deactivateAccView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            deactivateAccView.TransitioningDelegate = transitioningDelegate;
            this.PresentViewControllerAsync(deactivateAccView, true);
        }

        /// <summary>
        /// function cập nhật lại item được chọn ở menu 
        /// </summary>
        /// <param name="index">0: Trang chủ | 1: Đến tôi | 2: Tôi bắt đầu | 3: Broad | -1: Tạo mới</param>
        public void UpdateItemSelect(int index)
        {
            try
            {
                if (File.Exists(localpath))
                    img_avatar.Image = UIImage.FromFile(localpath);

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
            catch (Exception ex)
            {
                Console.WriteLine("MenuView - UpdateItemSelect - ERR: " + ex.ToString());
            }
        }

        private void NavigateToFollowView()
        {
            appD = UIApplication.SharedApplication.Delegate as AppDelegate;
            try
            {
                FollowListViewController followListViewController = (FollowListViewController)Storyboard.InstantiateViewController("FollowListViewController");
                /*if (appD.SlideMenuController.MainViewController.GetType() != typeof(FollowListViewController))
                {
                    appD.SlideMenuController.ChangeMainViewcontroller(followListViewController, true);
                }*/
                this.NavigationController.PushViewController(followListViewController, true);
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

        #region events
        private void CmmEvent_UpdateLangComplete(object sender, CmmEvent.UpdateEventArgs e)
        {
            try
            {
                InvokeOnMainThread(() =>
                {
                    initMenuItem();
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
                Console.WriteLine("SettingViewController - alert_signOut_Clicked - Err: " + ex.ToString());
            }
        }
        #endregion

        #region custom class
        private class menu_TableSource : UITableViewSource
        {
            static readonly NSString cellIdentifier = new NSString("menuCell");
            List<ClassMenu> lst_section0, lst_section1;
            List<ClassMenu> items;
            public static Dictionary<string, List<ClassMenu>> indexedSession;
            Dictionary<int, List<ClassMenu>> list_menuItem = new Dictionary<int, List<ClassMenu>>();
            List<string> sectionKeys;
            MenuView parentview;

            public menu_TableSource(Dictionary<int, List<ClassMenu>> _list_menuItem, MenuView controler)
            {
                parentview = controler;
                //items = _list_menuItem;
                list_menuItem = _list_menuItem;
                //loadData();
            }

            private void loadData()
            {
                sectionKeys = new List<string>();
                lst_section0 = new List<ClassMenu>();
                lst_section1 = new List<ClassMenu>();

                foreach (var item in items)
                {
                    if (item.section == 0)
                        lst_section0.Add(item);
                    if (item.section == 1)
                        lst_section1.Add(item);

                }
                indexedSession = new Dictionary<string, List<ClassMenu>>();
                sectionKeys.Add("Section0");
                indexedSession.Add("Section0", lst_section0);

                sectionKeys.Add("Section1");
                indexedSession.Add("Section1", lst_section1);
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
                return 40;
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                if (section == 0)
                    return 1;
                else
                    return 10;
            }

            public override UIView GetViewForHeader(UITableView tableView, nint section)
            {
                UIView view = new UIView();
                view.Frame = new CGRect(0, 0, parentview.View.Bounds.Width, 20);
                UILabel line = new UILabel();
                line.BackgroundColor = UIColor.FromRGB(245, 245, 245);
                line.Frame = new CGRect(20, 1, parentview.View.Bounds.Width - 40, 1);
                line.Hidden = false;
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
                }
                tableView.DeselectRow(indexPath, true);

                //if (indexPath.Section == 0)
                //{
                //    var item = indexedSession[sectionKeys[indexPath.Section]][indexPath.Row];
                //    parentview.NavigationTo(item, indexPath);
                //}
                //tableView.DeselectRow(indexPath, true);
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
            //UIView viewBase;
            UILabel lbl_lineLeft, title, lbl_count;
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

                //viewBase = new UIView();

                lbl_lineLeft = new UILabel();
                lbl_lineLeft.BackgroundColor = UIColor.FromRGB(0, 95, 212);
                lbl_lineLeft.Hidden = true;

                ic = new UIImageView();
                ic.ContentMode = UIViewContentMode.ScaleAspectFill;
                ic.TintColor = UIColor.FromRGB(0, 0, 0);

                lbl_count = new UILabel()
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = UIFont.FromName("ArialMT", 14f),
                    TextColor = UIColor.FromRGB(0, 95, 212)
                };

                title = new UILabel
                {
                    TextAlignment = UITextAlignment.Left,
                    //Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular),
                    Font = UIFont.FromName("ArialMT", 14f),
                    TextColor = UIColor.FromRGB(0, 0, 0)
                };

                segment_language = new UISegmentedControl();
                segment_language.InsertSegment("VN", 0, false);
                segment_language.InsertSegment("EN", 1, false);


                var firstAttributesSelect = new UITextAttributes
                {
                    TextColor = UIColor.FromRGB(0, 95, 212)
                };
                var firstAttributesNormal = new UITextAttributes
                {
                    TextColor = UIColor.FromRGB(0, 0, 0)
                };
                segment_language.SetTitleTextAttributes(firstAttributesSelect, UIControlState.Selected);

                segment_language.ValueChanged += delegate
                {
                    if (segment_language.SelectedSegment == 0)
                    {
                        CmmVariable.SysConfig.LangCode = "1066";
                    }
                    else
                    {
                        CmmVariable.SysConfig.LangCode = "1033";
                    }
                    CmmFunction.WriteSetting();

                    parentview.changeLanguage();
                    ProviderUser p_user = new ProviderUser();
                    p_user.UpdateUserLanguageChange(CmmVariable.SysConfig.LangCode);
                };

                ContentView.AddSubviews(new UIView[] { lbl_lineLeft, title, lbl_count, ic, segment_language });
            }

            public void UpdateCell(ClassMenu _menu)
            {
                menu = _menu;
                title.Text = menu.title;

                if (menu.iconUrl == "icon_language30.png") // row - ngôn ngữ
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
                        //ic.TintColor = UIColor.FromRGB(0, 95, 212);
                        title.TextColor = UIColor.FromRGB(0, 95, 212);
                        title.Font = UIFont.FromName("Arial-BoldMT", 15f);

                        //lbl_count.TextAlignment = UITextAlignment.Center;
                        //lbl_count.Font = UIFont.FromName("ArialMT", 14f);
                        //lbl_count.TextColor = UIColor.FromRGB(0, 95, 212);

                        lbl_lineLeft.Hidden = false;
                    }
                    else
                    {
                        //ic.TintColor = UIColor.FromRGB(0, 0, 0);
                        //title.TextColor = UIColor.FromRGB(0, 0, 0);
                        title.Font = UIFont.FromName("ArialMT", 14f);
                    }
                }
                //else //Dùng màu sẵn có trên icon
                //{
                //    if (!string.IsNullOrEmpty(menu.iconUrl))
                //        ic.Image = UIImage.FromFile("Icons/" + menu.iconUrl);
                //}

                else if (menu.section == 2)
                {
                    title.TextColor = UIColor.FromRGB(237, 28, 36);
                }
                //thuyngo add
                //if(menu.section == 0)
                //{
                //   if(menu.isSelected)
                //       viewBase.BackgroundColor = UIColor.FromRGB(245, 245, 245).ColorWithAlpha(0.8f);
                //}
                ic.TintColor = title.TextColor;

            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                var model = DeviceHardware.Model;
                if (model.Contains("X") || model.Contains("11"))
                    lbl_lineLeft.Frame = new CGRect(0, 1, 5, ContentView.Frame.Height - 2);
                else
                    lbl_lineLeft.Frame = new CGRect(0, 1, 3, ContentView.Frame.Height - 2);
                //viewBase.Frame = new CGRect(lbl_lineLeft.Frame.Right, 1 , ContentView.Frame.Width - lbl_lineLeft.Frame.Right, ContentView.Frame.Height - 2);

                if (menu.iconUrl == "icon_Approve30.png" || menu.iconUrl == "icon_Board30.png")
                    ic.Frame = new CGRect(30, 10, 19, 19);
                else
                    ic.Frame = new CGRect(30, 10, 20, 20);

                title.Frame = new CGRect(ic.Frame.Right + 15, 10, 250, 20);
                if (menu.iconUrl == "icon_language30.png")
                    segment_language.Frame = new CGRect(ContentView.Frame.Width - 120, 12, 110, 26);
                else
                    lbl_count.Frame = new CGRect(ContentView.Frame.Width - 70, 10, 50, 20);
            }
        }

        private class CountNum
        {
            public int count { get; set; }
        }
        #endregion
    }
}


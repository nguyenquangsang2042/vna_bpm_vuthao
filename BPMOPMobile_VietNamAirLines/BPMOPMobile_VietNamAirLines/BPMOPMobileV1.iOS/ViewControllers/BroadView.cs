using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobileV1.iOS.CustomControlClass;
using BPMOPMobileV1.iOS.IOSClass;
using CoreGraphics;
using Foundation;
using SlideMenuControllerXamarin;
using SQLite;
using UIKit;
using Xamarin.iOS;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class BroadView : UIViewController
    {
        AppDelegate appD;
        Broad_TableSource broad_TableSource;
        ButtonsActionBotBar buttonActionBotBar;
        UIRefreshControl board_refreshControl;
        List<KeyValuePair<string, bool>> lst_sectionState;
        List<BeanWorkflowCategory> lst_category;
        public BeanWorkflowCategory currentWorkFlowCateSelected { get; set; }
        Dictionary<string, List<BeanWorkflow>> dict_groupWorkFlow = new Dictionary<string, List<BeanWorkflow>>();
        Dictionary<string, List<BeanWorkflow>> dict_groupWorkFlow_searchResult = new Dictionary<string, List<BeanWorkflow>>();
        bool IsFirstLoad = true;
        bool filterAll = true; // true: all || false: favorite
        private bool isfirst = true;
        bool isFisrt = true;
        UIStringAttributes firstAttributes = new UIStringAttributes
        {
            Font = UIFont.FromName("ArialMT", 13f)
        };

        public BroadView(IntPtr handle) : base(handle)
        {
            appD = UIApplication.SharedApplication.Delegate as AppDelegate;
        }
        public override void ViewDidLayoutSubviews()
        {
            if (IsFirstLoad)
            {
                IsFirstLoad = false;
                ViewConfiguration();
            }
        }
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            isFisrt = false;
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            CreateCircle();

            var str_transalte = CmmFunction.GetTitle("TEXT_BOARD", "Board");
            lbl_title.Text = str_transalte;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ReloadFailedBeanData();

            CmmIOSFunction.ResignFirstResponderOnTap(this.View);
            board_refreshControl = new UIRefreshControl();
            //ViewConfiguration();
            SetConstraint();
            CmmIOSFunction.AddShadowForTopORBotBar(view_bot_bar, false);
            LoadListCategory();
            SetLangTitle();
            LoadContent();

            #region delegate
            board_refreshControl.ValueChanged += Board_refreshControl_ValueChanged;
            BT_avatar.TouchUpInside += BT_back_TouchUpInside;
            tf_search.EditingChanged += Tf_search_EditingChanged;
            BT_category.TouchUpInside += BT_category_TouchUpInside;
            BT_filter_favorite.TouchUpInside += BT_filter_favorite_TouchUpInside;
            BT_search.TouchUpInside += BT_search_TouchUpInside;
            CmmEvent.UpdateLangComplete += CmmEvent_UpdateLangComplete;
            #endregion
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (!isFisrt)
            {
                LoadListCategory();
                LoadContent();
            }

            if (buttonActionBotBar != null)
            {
                view_bot_bar.AddSubviews(buttonActionBotBar);
            }
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        #region public - private method

        void ReloadFailedBeanData()
        {
            CmmIOSFunction.UpdateBeanData<BeanWorkflow>();
            CmmIOSFunction.UpdateBeanData<BeanWorkflowCategory>();

#if DEBUG
            GetAllBeanWorkflow();
#endif
        }

        private void ViewConfiguration()
        {
            lbl_title.BackgroundColor = UIColor.White;

            board_refreshControl.TintColor = UIColor.FromRGB(65, 80, 134);
            BT_search.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_filter_favorite.ContentEdgeInsets = new UIEdgeInsets(5, 5, 0, 5);
            board_refreshControl.AttributedTitle = new NSAttributedString(CmmFunction.GetTitle("TEXT_REFRESH_LOADING", "Loading ..."), firstAttributes);
            table_content.AddSubview(board_refreshControl);

            view_filter_category.Layer.BorderColor = UIColor.FromRGB(232, 232, 232).CGColor;
            view_filter_category.Layer.BorderWidth = 0.8f;
            view_filter_category.Layer.CornerRadius = 4;
            view_filter_category.ClipsToBounds = true;

            tf_search.TintColor = UIColor.Black;

            //var model = DeviceHardware.Model;
            //if (model.Contains("X") || model.Contains("11"))
            //{
            //    headerView_constantHeight.Constant = 80;
            //}

            constraint_heightSearch.Constant = 0;
            constraint_topcatalogy.Constant = 0;
            view_search.Hidden = true;
            //CmmIOSFunction.MakeCornerTopLeftRight(view_lstWorkFlow, 8);

            //CmmIOSFunction.CreateCircleButton(BT_avatar);
            string documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string localpath = Path.Combine(documentFolder, CmmVariable.M_Avatar);

            BT_avatar.ImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
            if (File.Exists(localpath))
                BT_avatar.SetImage(UIImage.FromFile(localpath), UIControlState.Normal);
            else
                BT_avatar.SetImage(UIImage.FromFile("Icons/icon_avatar64.png"), UIControlState.Normal);

            buttonActionBotBar = ButtonsActionBotBar.Instance;
            buttonActionBotBar.InitFrameView(view_bot_bar.Frame);
            buttonActionBotBar.LoadStatusButton(3);
            //buttonActionBotBar.UpdateChildBroad(false);
            view_bot_bar.AddSubviews(buttonActionBotBar);
        }

        private void LoadListCategory()
        {
            lst_category = new List<BeanWorkflowCategory>();
            var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
            string query_worflowCategory = @"SELECT * FROM BeanWorkflowCategory ORDER BY Title";
            //lst_category = conn.Query<BeanWorkflowCategory>(query_worflowCategory);
            try
            {
                var lst_cate = conn.Query<BeanWorkflowCategory>(query_worflowCategory);
                if (lst_cate != null && lst_cate.Count > 0)
                {
                    foreach (var item in lst_cate)
                    {
                        string query_worflow = @"SELECT * FROM BeanWorkflow WHERE StatusName = ? AND WorkflowCategoryID = ? ";
                        List<BeanWorkflow> lst_workflow = conn.Query<BeanWorkflow>(query_worflow, "Active", item.ID);
                        if (lst_workflow != null && lst_workflow.Count > 0)
                            lst_category.Add(item);
                    }
                }

                if (lst_category != null && lst_category.Count > 0)
                {
                    BeanWorkflowCategory cate_all = new BeanWorkflowCategory();
                    cate_all.ID = 0;
                    cate_all.Title = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");

                    lst_category.Insert(0, cate_all);
                    HandelMenuWorkFlowCategoryResult(lst_category[0]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("BroadView - LoadListCategory - Err: " + ex.ToString());
            }
            finally
            {
                conn.Close();
            }
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
            CmmIOSFunction.SetLangToView(this.View);
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

        public void LoadContent(bool isfirst = true)
        {
            int pos = 0;
            var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
            string query_worflowCategory = @"SELECT * FROM BeanWorkflowCategory ORDER BY [Order]";//Từ ngày 28.06.22 xếp thứ tự theo cột Order chứ ko phải Title, đổi thêm trong loadContentByCateID
            var lst_cate = conn.Query<BeanWorkflowCategory>(query_worflowCategory);
            List<BeanWorkflow> lst_workflowFavorite = new List<BeanWorkflow>();

            if (lst_cate != null && lst_category.Count > 0)
            {
                dict_groupWorkFlow = new Dictionary<string, List<BeanWorkflow>>();
                foreach (var item in lst_cate)
                {
                    if (filterAll)
                    {
                        string query_worflow = @"SELECT * FROM BeanWorkflow WHERE StatusName = ? AND WorkflowCategoryID = ? AND IsPermission = 1 ORDER BY WorkflowID ASC";
                        List<BeanWorkflow> lst_workflow = conn.Query<BeanWorkflow>(query_worflow, "Active", item.ID);
                        if (lst_workflow != null && lst_workflow.Count > 0)
                            dict_groupWorkFlow.Add(item.Title + ";#" + pos, lst_workflow);
                    }
                    else
                    {
                        string query_worflow = @"SELECT * FROM BeanWorkflow WHERE StatusName = ? AND Favorite = ? AND WorkflowCategoryID = ? AND IsPermission = 1 ORDER BY WorkflowID ASC";
                        List<BeanWorkflow> lst_workflow = conn.Query<BeanWorkflow>(query_worflow, "Active", true, item.ID);
                        if (lst_workflow != null && lst_workflow.Count > 0)
                            lst_workflowFavorite.AddRange(lst_workflow);
                        //dict_groupWorkFlow.Add(item.Title, lst_workflow);
                    }
                    pos++;
                }
                if (!filterAll)
                {
                    if (lst_workflowFavorite != null && lst_workflowFavorite.Count > 0)
                        dict_groupWorkFlow.Add("", lst_workflowFavorite);
                }
            }

            if (dict_groupWorkFlow != null && dict_groupWorkFlow.Count > 0)
            {
                if (filterAll)
                {
                    //table_content = new UITableView(table_content.Frame, UITableViewStyle.Grouped);
                    //table_content.ContentInset = new UIEdgeInsets(-30, 0, 0, 0);
                    broad_TableSource = new Broad_TableSource(this, dict_groupWorkFlow, lst_sectionState);
                }

                else
                {
                    //table_content = new UITableView(table_content.Frame, UITableViewStyle.Plain);
                    table_content.ContentInset = new UIEdgeInsets(-30, 0, 0, 0);
                    broad_TableSource = new Broad_TableSource(this, dict_groupWorkFlow, null);
                }

                //isfirst = false;

                table_content.Source = broad_TableSource;
                table_content.ReloadData();
                table_content.Hidden = false;
                lbl_dodata.Hidden = true;
            }
            else
            {
                table_content.Hidden = true;
                lbl_dodata.Hidden = false;
            }
        }

        void GetAllBeanWorkflow()
        {
            var conn = new SQLiteAsyncConnection(CmmVariable.M_DataPath, false);
            List<BeanWorkflow> lst1 = new List<BeanWorkflow>();
            List<BeanWorkflowCategory> lst2 = new List<BeanWorkflowCategory>();
            try
            {
                lst1 = conn.QueryAsync<BeanWorkflow>(@"SELECT * FROM BeanWorkflow").Result;
                lst2 = conn.QueryAsync<BeanWorkflowCategory>(@"SELECT * FROM BeanWorkflowCategory").Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetAllBeanWorkflow - Err: " + ex.ToString());
            }

            Console.WriteLine("BeanWorkflow: " + lst1.Count + "\tBeanWorkflowCategory: " + lst2.Count);
        }

        public void loadContentByCateID(int WorkFlowCateID)
        {
            try
            {
                var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                string query_worflowCategory = "";
                if (WorkFlowCateID == 0)
                    query_worflowCategory = @"SELECT * FROM BeanWorkflowCategory ORDER BY [Order]";
                else
                    query_worflowCategory = @"SELECT * FROM BeanWorkflowCategory WHERE ID = ? ORDER BY [Order]";

                var lst_cate = conn.Query<BeanWorkflowCategory>(query_worflowCategory, WorkFlowCateID);

                if (lst_cate != null && lst_category.Count > 0)
                {
                    dict_groupWorkFlow = new Dictionary<string, List<BeanWorkflow>>();
                    foreach (var item in lst_cate)
                    {
                        if (filterAll)
                        {
                            string query_worflow = @"SELECT * FROM BeanWorkflow WHERE StatusName = ? AND WorkflowCategoryID = ? AND IsPermission = 1 ORDER BY WorkflowID ASC";
                            List<BeanWorkflow> lst_workflow = conn.Query<BeanWorkflow>(query_worflow, "Active", item.ID);
                            if (lst_workflow != null && lst_workflow.Count > 0)
                                dict_groupWorkFlow.Add(item.Title, lst_workflow);
                        }
                        //favorite khong co filter
                        //else
                        //{
                        //    string query_worflow = @"SELECT * FROM BeanWorkflow WHERE StatusName = ? AND Favorite = ? AND WorkflowCategoryID = ?";
                        //    List<BeanWorkflow> lst_workflow = conn.Query<BeanWorkflow>(query_worflow, "Active", true, item.ID);
                        //    if (lst_workflow != null && lst_workflow.Count > 0)
                        //        dict_groupWorkFlow.Add(item.Title, lst_workflow);
                        //}
                    }

                    //dict_groupWorkFlow = new Dictionary<string, List<BeanWorkflow>>();
                    //if (filterAll)
                    //{
                    //    string query_worflow = @"SELECT * FROM BeanWorkflow WHERE WorkflowCategoryID = ?";
                    //    List<BeanWorkflow> lst_workflow = conn.Query<BeanWorkflow>(query_worflow, lst_cate[0].ID);
                    //    if (lst_workflow != null && lst_workflow.Count > 0)
                    //        dict_groupWorkFlow.Add(lst_cate[0].Title, lst_workflow);
                    //}
                    //else
                    //{
                    //    string query_worflow = @"SELECT * FROM BeanWorkflow WHERE Favorite = ? AND WorkflowCategoryID = ?";
                    //    List<BeanWorkflow> lst_workflow = conn.Query<BeanWorkflow>(query_worflow, true, lst_cate[0].ID);
                    //    if (lst_workflow != null && lst_workflow.Count > 0)
                    //        dict_groupWorkFlow.Add(lst_cate[0].Title, lst_workflow);
                    //}
                }

                if (dict_groupWorkFlow != null && dict_groupWorkFlow.Count > 0)
                {
                    Broad_TableSource broad_TableSource = new Broad_TableSource(this, dict_groupWorkFlow, lst_sectionState);
                    table_content.Source = broad_TableSource;
                    table_content.ReloadData();
                    table_content.Hidden = false;
                    lbl_dodata.Hidden = true;
                }
                else
                {
                    table_content.Hidden = true;
                    lbl_dodata.Hidden = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("BoardsView - loadContentByCateID - Err: " + ex.ToString());
            }
        }

        public void UpdateColletionView(int index)
        {
            table_content.ReloadSections(NSIndexSet.FromIndex(index), UITableViewRowAnimation.Automatic);
        }

        private void searchData()
        {
            try
            {
                string content = CmmFunction.removeSignVietnamese(tf_search.Text.Trim().ToLowerInvariant());
                if (!string.IsNullOrEmpty(content))
                {
                    var dict_groupWorkFlow_searchResult_temp = new Dictionary<string, List<BeanWorkflow>>();
                    var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                    string query_worflowCategory = "";
                    if (currentWorkFlowCateSelected.ID != 0)
                        query_worflowCategory = string.Format(@"SELECT * FROM BeanWorkflowCategory WHERE ID = {0}", currentWorkFlowCateSelected.ID);
                    else //ALL
                        query_worflowCategory = string.Format(@"SELECT * FROM BeanWorkflowCategory");

                    List<BeanWorkflowCategory> lst_category = conn.Query<BeanWorkflowCategory>(query_worflowCategory);
                    if (lst_category != null && lst_category.Count > 0)
                    {
                        List<BeanWorkflow> lst_workflowFavorite = new List<BeanWorkflow>();
                        dict_groupWorkFlow_searchResult_temp = new Dictionary<string, List<BeanWorkflow>>();
                        foreach (var item in lst_category)
                        {
                            List<BeanWorkflow> lst_workflow = new List<BeanWorkflow>();
                            if (filterAll)
                            {
                                string query_worflow = @"SELECT * FROM BeanWorkflow WHERE StatusName = ? AND WorkflowCategoryID = ? AND IsPermission = 1 ORDER BY WorkflowID ASC";
                                lst_workflow = conn.Query<BeanWorkflow>(query_worflow, "Active", item.ID);
                                if (lst_workflow != null && lst_workflow.Count > 0)
                                {
                                    var items = from wf in lst_workflow
                                                where (!string.IsNullOrEmpty(wf.Title) && CmmFunction.removeSignVietnamese(wf.Title.ToLowerInvariant()).Contains(content))
                                                select wf;

                                    if (items != null && items.Count() > 0)
                                    {
                                        dict_groupWorkFlow_searchResult_temp.Add(item.Title, items.ToList());
                                    }
                                }
                            }
                            else
                            {
                                string query_worflow = @"SELECT * FROM BeanWorkflow WHERE StatusName = ? AND Favorite = ? AND WorkflowCategoryID = ? AND IsPermission = 1 ORDER BY WorkflowID ASC";
                                lst_workflow = conn.Query<BeanWorkflow>(query_worflow, "Active", true, item.ID);
                                if (lst_workflow != null && lst_workflow.Count > 0)
                                {
                                    var items = from wf in lst_workflow
                                                where (!string.IsNullOrEmpty(wf.Title) && CmmFunction.removeSignVietnamese(wf.Title.ToLowerInvariant()).Contains(content))
                                                select wf;

                                    if (items != null && items.Count() > 0)
                                    {
                                        lst_workflowFavorite.AddRange(items);
                                    }
                                }
                            }
                        }
                        if (!filterAll)
                        {
                            if (lst_workflowFavorite != null && lst_workflowFavorite.Count > 0)
                                dict_groupWorkFlow_searchResult_temp.Add("", lst_workflowFavorite);
                        }
                    }

                    if (dict_groupWorkFlow_searchResult_temp != null && dict_groupWorkFlow_searchResult_temp.Count > 0)
                    {
                        Broad_TableSource broad_TableSource_temp;
                        if (filterAll)
                        {
                            broad_TableSource_temp = new Broad_TableSource(this, dict_groupWorkFlow_searchResult_temp, lst_sectionState);
                        }

                        else
                        {
                            table_content.ContentInset = new UIEdgeInsets(-30, 0, 0, 0);
                            broad_TableSource_temp = new Broad_TableSource(this, dict_groupWorkFlow_searchResult_temp, null);
                        }

                        //Broad_TableSource broad_TableSource = new Broad_TableSource(this, dict_groupWorkFlow_searchResult_temp, lst_sectionState);
                        table_content.Source = broad_TableSource_temp;
                        table_content.ReloadData();
                        table_content.Hidden = false;
                        lbl_dodata.Hidden = true;
                    }
                    else
                    {
                        table_content.Hidden = true;
                        lbl_dodata.Hidden = false;
                    }
                }
                else
                {
                    table_content.Hidden = false;
                    lbl_dodata.Hidden = true;
                    broad_TableSource = new Broad_TableSource(this, dict_groupWorkFlow, lst_sectionState);
                    table_content.Source = broad_TableSource;
                    table_content.ReloadData();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ToDoDetailView - Tf_search_EditingChanged - Err: " + ex.ToString());
            }
        }

        private void ToggleMenuCategory()
        {
            Custom_WorkflowCategory custom_WorkflowCategory = Custom_WorkflowCategory.Instance;
            if (custom_WorkflowCategory.Superview != null)
                custom_WorkflowCategory.RemoveFromSuperview();
            else
            {
                nfloat height = lst_category.Count * custom_WorkflowCategory.RowHeigth;
                if (height + (view_filter_category.Frame.Bottom) >= UIScreen.MainScreen.Bounds.Bottom)
                    height = UIScreen.MainScreen.Bounds.Height - view_filter_category.Frame.Bottom;
                custom_WorkflowCategory.ItemNoIcon = false;
                custom_WorkflowCategory.viewController = this;
                custom_WorkflowCategory.InitFrameView(new CGRect(view_filter_category.Frame.Left, view_filter_category.Frame.Bottom + 2, view_filter_category.Frame.Width, height));
                custom_WorkflowCategory.AddShadowForView();
                custom_WorkflowCategory.ListItemMenu = lst_category;
                custom_WorkflowCategory.TableLoadData();

                View.AddSubview(custom_WorkflowCategory);
                View.BringSubviewToFront(custom_WorkflowCategory);
            }
        }

        public void HandelMenuWorkFlowCategoryResult(BeanWorkflowCategory _workFlowCate)
        {
            if (_workFlowCate != null)
            {
                if (isSearch)
                {
                    tf_search.Text = "";
                    SearchToggle();
                }
                _workFlowCate.IsSelected = true;
                currentWorkFlowCateSelected = _workFlowCate;

                foreach (var item in lst_category)
                {
                    if (item.ID == currentWorkFlowCateSelected.ID)
                        item.IsSelected = true;
                    else
                        item.IsSelected = false;
                }

                if (currentWorkFlowCateSelected.ID == 0) // ALL
                    LoadContent();
                else
                    loadContentByCateID(currentWorkFlowCateSelected.ID); //By CateID

                lbl_categorySelected.Text = currentWorkFlowCateSelected.Title;
            }

            Custom_WorkflowCategory custom_WorkflowCategory = Custom_WorkflowCategory.Instance;
            if (custom_WorkflowCategory.Superview != null)
                custom_WorkflowCategory.RemoveFromSuperview();
        }

        /// <summary>
        /// TypeView - 0: Board, 1: List
        /// </summary>
        /// <param name="workflowSelected"></param>
        /// <param name="typeView"></param
        public void NavigateToKanBan(BeanWorkflow workflowSelected, int typeView)
        {

            KanBanView kanBanView = (KanBanView)Storyboard.InstantiateViewController("KanBanView");
            kanBanView.SetContent(workflowSelected, typeView);
            this.NavigationController.PushViewController(kanBanView, true);

            ////KanBanView kanBanView = (KanBanView)Storyboard.InstantiateViewController("KanBanView");
            ////kanBanView.SetContent(this, workflowSelected, typeView);
            ////this.NavigationController.PushViewController(kanBanView, true);

            //List<ButtonActionApp> classMenu = new List<ButtonActionApp>();

            //ButtonActionApp m1 = new ButtonActionApp() { ID = 0, section = 0, title = CmmFunction.GetTitle("TEXT_MAINVIEW", "Trang chủ"), iconUrl = "Icons/icon_home30.png", isSelected = true , opption = false , TypeParent = typeof(MainViewApp).ToString()};
            //ButtonActionApp m2 = new ButtonActionApp() { ID = 1, section = 0, title = CmmFunction.GetTitle("TEXT_TOME", "Đến tôi"), iconUrl = "Icons/icon_myRequest30.png", isSelected = false, opption = false, TypeParent = typeof(RequestListViewApp).ToString() };
            //ButtonActionApp m3 = new ButtonActionApp() { ID = 2, section = 0, title = CmmFunction.GetTitle("TEXT_FROMME", "Tôi bắt đầu"), iconUrl = "Icons/icon_Approve30.png", isSelected = true, opption = true, TypeParent = typeof(MyRequestListViewApp).ToString() };
            //ButtonActionApp m4 = new ButtonActionApp() { ID = 3, section = 0, title = "", iconUrl = "Icons/icon_more.png", isSelected = false, opption = false, TypeParent = "more" };
            //ButtonActionApp m5 = new ButtonActionApp() { ID = 4, section = 0, title = CmmFunction.GetTitle("TEXT_BOARD", "Broad"), iconUrl = "Icons/icon_broad_havecolor.png", isSelected = false, opption = true, TypeParent = typeof(KanBanView).ToString() };
            ////ButtonActionApp m6 = new ButtonActionApp() { ID = 5, section = 0, title = "list", iconUrl = "Icons/icon_list.png", isSelected = false, opption = true, TypeParent = typeof(ListView).ToString() };
            ////ButtonActionApp m7 = new ButtonActionApp() { ID = 6, section = 0, title = "report", iconUrl = "Icons/icon_report.png", isSelected = false, opption = true, TypeParent = typeof(KanBanView).ToString() };

            //classMenu.Add(m1);
            //classMenu.Add(m2);
            //classMenu.Add(m3);
            //classMenu.Add(m4);
            //classMenu.Add(m5);
            ////classMenu.Add(m6);
            ////classMenu.Add(m7);

            //ButtonsActionBroadBotBarApplication buttonActionBotBarApplication;
            //ButtonsActionBroadBotBarApplication.classMenu = classMenu;
            //ButtonsActionBroadBotBarApplication.classMenuSelect = m1;
            //ButtonsActionBroadBotBarApplication.Reset();
            //buttonActionBotBarApplication = ButtonsActionBroadBotBarApplication.Instance;
            //buttonActionBotBarApplication.InitFrameView(view_bot_bar.Frame);
            //MainViewApp mainViewApp = (MainViewApp)Storyboard.InstantiateViewController("MainViewApp");
            //buttonActionBotBarApplication.mainViewApp = mainViewApp;

            //buttonActionBotBarApplication.broadView = this;
            //buttonActionBotBarApplication.workflow = workflowSelected;
            //buttonActionBotBarApplication.Select_BT_Group1();
        }
        public void BackFromeKanBanView()
        {
            buttonActionBotBar.Select_BT_Group4();
        }
        #endregion

        #region event
        private void BT_back_TouchUpInside(object sender, EventArgs e)
        {
            if (isSearch)
            {
                SearchToggle();
            }
            AppDelegate appD = UIApplication.SharedApplication.Delegate as AppDelegate;
            //appD.menu.loadData_count();
            appD.menu.UpdateItemSelect(3);
            appD.SlideMenuController.OpenLeft();

            //appD.menu.UpdateItemSelect(0);
            //appD.SlideMenuController.OpenLeft();
        }
        private void BT_category_TouchUpInside(object sender, EventArgs e)
        {
            ToggleMenuCategory();
        }
        private void BT_filter_favorite_TouchUpInside(object sender, EventArgs e)
        {

            filterAll = !filterAll;
            tf_search.Text = string.Empty;

            if (filterAll == false) // on
            {
                BT_filter_favorite.SetImage(UIImage.FromFile("Icons/icon_favorite_on.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
                constraint_toptable.Constant = 0; //view header table
                view_filter_category.Hidden = true;
                LoadListCategory();
            }
            else //off
            {
                BT_filter_favorite.SetImage(UIImage.FromFile("Icons/icon_favorite_off.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
                constraint_toptable.Constant = 40;//view header table
                view_filter_category.Hidden = false;
            }
            BT_filter_favorite.ContentMode = UIViewContentMode.ScaleAspectFit;

            if (lst_category != null && lst_category.Count > 0)
            {
                HandelMenuWorkFlowCategoryResult(lst_category[0]);
            }

            //LoadContent();

        }
        private bool isSearch = false;
        private void BT_search_TouchUpInside(object sender, EventArgs e)
        {
            SearchToggle();
        }
        private void SearchToggle()
        {
            if (!isSearch) // search dang dong
            {
                if (view_search.Frame.Height > 0)
                {
                    if (!string.IsNullOrEmpty(tf_search.Text.Trim()))
                    {
                        isSearch = true;
                        tf_search.BecomeFirstResponder();
                        BT_search.Enabled = true;
                    }
                    else
                    {
                        isSearch = false;
                        constraint_heightSearch.Constant = 44;
                        UIView.BeginAnimations("search_slideAnimationShow");
                        UIView.SetAnimationDuration(0.4f);
                        UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                        UIView.SetAnimationRepeatCount(0);
                        UIView.SetAnimationRepeatAutoreverses(false);
                        UIView.SetAnimationDelegate(this);
                        view_search.Alpha = 0;
                        view_search.Hidden = true;
                        constraint_heightSearch.Constant = 0;
                        constraint_topcatalogy.Constant = 0;// padding top catagoly
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
                    constraint_heightSearch.Constant = 0;
                    UIView.BeginAnimations("search_slideAnimationShow");
                    UIView.SetAnimationDuration(0.4f);
                    UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
                    UIView.SetAnimationRepeatCount(0);
                    UIView.SetAnimationRepeatAutoreverses(false);
                    UIView.SetAnimationDelegate(this);
                    constraint_heightSearch.Constant = 44;
                    constraint_topcatalogy.Constant = 5; // padding top catagoly
                    UIView.CommitAnimations();
                    tf_search.BecomeFirstResponder();
                    BT_search.Enabled = true;
                    BT_search.SetImage(UIImage.FromFile("Icons/icon_Search.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
                    BT_search.TintColor = UIColor.FromRGB(40, 176, 255);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(tf_search.Text.Trim()))
                {
                    isSearch = false;
                    this.View.EndEditing(true);
                    BT_search.Enabled = true;
                }
                else
                {
                    isSearch = false;
                    constraint_heightSearch.Constant = 44;
                    UIView.BeginAnimations("search_slideAnimationShow");
                    UIView.SetAnimationDuration(0.4f);
                    UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                    UIView.SetAnimationRepeatCount(0);
                    UIView.SetAnimationRepeatAutoreverses(false);
                    UIView.SetAnimationDelegate(this);
                    view_search.Alpha = 0;
                    view_search.Hidden = true;
                    constraint_heightSearch.Constant = 0;
                    constraint_topcatalogy.Constant = 0;// padding top catagoly
                    this.View.EndEditing(true);
                    UIView.CommitAnimations();
                    BT_search.Enabled = true;
                    BT_search.SetImage(UIImage.FromFile("Icons/icon_Search.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
                    BT_search.TintColor = UIColor.FromRGB(0, 0, 0);
                }
            }
        }
        private void Tf_search_EditingChanged(object sender, EventArgs e)
        {
            searchData();
        }
        private async void Board_refreshControl_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (isSearch)
                {
                    tf_search.Text = "";
                    SearchToggle();
                }
                board_refreshControl.BeginRefreshing();
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
                        //filterAll = true;
                        //BT_filter_all.SetTitle(CmmFunction.GetTitle("TEXT_ALL", "Tất cả"), UIControlState.Normal);

                        if (File.Exists(localpath))
                            BT_avatar.SetImage(UIImage.FromFile(localpath), UIControlState.Normal);

                        LoadListCategory();
                        currentWorkFlowCateSelected.ID = 0;
                        if (lst_category != null && lst_category.Count > 0)
                            HandelMenuWorkFlowCategoryResult(lst_category[0]);
                        //LoadContent();
                        board_refreshControl.EndRefreshing();
                    });
                });
            }
            catch (Exception ex)
            {
                board_refreshControl.EndRefreshing();
                Console.WriteLine("Error - BroadView - refreshControl_valuechange - Er: " + ex.ToString());
            }
        }
        private void CmmEvent_UpdateLangComplete(object sender, CmmEvent.UpdateEventArgs e)
        {
            try
            {
                InvokeOnMainThread(() =>
                {
                    SetLangTitle();
                    tf_search.Placeholder = CmmFunction.GetTitle("TEXT_SEARCH", "Tìm kiếm theo tên quy trình");
                    lbl_categorySelected.Text = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
                    board_refreshControl.AttributedTitle = new NSAttributedString(CmmFunction.GetTitle("TEXT_REFRESH_LOADING", "Loading ..."), firstAttributes);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("WorkFlowDetailView - CmmEvent_UpdateLangComplete - ERR: " + ex.ToString());
            }
        }
        #endregion

        #region list broad source table
        private class Broad_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellBroadID");
            BroadView parentView { get; set; }
            public static Dictionary<string, List<BeanWorkflow>> dict_groupWorkFlow;
            List<string> sectionKeys;
            List<KeyValuePair<string, bool>> lst_sectionState;
            public List<BeanWorkflow> items;

            public Broad_TableSource(BroadView _parentview, Dictionary<string, List<BeanWorkflow>> _dict_groupWorkFlow, List<KeyValuePair<string, bool>> _sectionState)
            {
                parentView = _parentview;
                lst_sectionState = _sectionState;
                dict_groupWorkFlow = _dict_groupWorkFlow;
                LoadData();
            }
            public void LoadData()
            {
                if (lst_sectionState == null)
                {
                    lst_sectionState = new List<KeyValuePair<string, bool>>();
                    sectionKeys = new List<string>();

                    foreach (var item in dict_groupWorkFlow)
                    {
                        sectionKeys.Add(item.Key);
                        KeyValuePair<string, bool> keypair_section = new KeyValuePair<string, bool>(item.Key, false);
                        lst_sectionState.Add(keypair_section);
                    }

                    parentView.lst_sectionState = lst_sectionState;
                }
                else
                {
                    //giu lai nhung key dang dong hoac mo
                    List<KeyValuePair<string, bool>> temp_lst_sectionState = new List<KeyValuePair<string, bool>>();
                    foreach (var item in lst_sectionState)
                    {
                        temp_lst_sectionState.Add(new KeyValuePair<string, bool>(item.Key, item.Value));
                    }

                    lst_sectionState = new List<KeyValuePair<string, bool>>();
                    sectionKeys = new List<string>();
                    bool flagTouch = false;
                    foreach (var item in dict_groupWorkFlow)
                    {
                        sectionKeys.Add(item.Key);
                        var itemTouch = temp_lst_sectionState.Find(s => s.Key == item.Key);
                        flagTouch = itemTouch.Value;
                        KeyValuePair<string, bool> keypair_section = new KeyValuePair<string, bool>(item.Key, flagTouch);
                        lst_sectionState.Add(keypair_section);
                    }
                }
            }
            public override nfloat GetHeightForFooter(UITableView tableView, nint section)
            {
                return -1;
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                if (lst_sectionState.Count == 1 && lst_sectionState[0].Key == "")
                    return 0;
                return 50;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 105;
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return lst_sectionState.Count;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                foreach (var i in lst_sectionState)
                {
                    if (i.Key == sectionKeys[(int)section] && i.Value == false)
                        return dict_groupWorkFlow[sectionKeys[(int)section]].Count;
                    else if (i.Key == sectionKeys[(int)section] && i.Value == true)
                        return 0;
                }

                return 0;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                tableView.DeselectRow(indexPath, true);
                KeyValuePair<string, List<BeanWorkflow>> lst_itemInSection = dict_groupWorkFlow.Where(x => x.Key == sectionKeys[indexPath.Section]).First();
                BeanWorkflow workflowSelected = lst_itemInSection.Value[indexPath.Row];

                if (parentView.GetType() == typeof(BroadView))
                {
                    BroadView broadView = parentView as BroadView;
                    broadView.NavigateToKanBan(workflowSelected, 0);
                }
            }

            public override UIView GetViewForHeader(UITableView tableView, nint section)
            {
                if (lst_sectionState.Count == 1 && lst_sectionState[0].Key == "")
                    return null;

                //declear
                UIImageView img_arrow;
                UILabel lbl_title;
                UIButton BT_section;
                UIView baseView = new UIView();
                KeyValuePair<string, bool> Data = new KeyValuePair<string, bool>();

                //ViewConfiguration
                baseView.Frame = new CGRect(0, 0, tableView.Frame.Width, 50);
                baseView.BackgroundColor = UIColor.White;

                img_arrow = new UIImageView();
                img_arrow.ContentMode = UIViewContentMode.ScaleAspectFill;

                lbl_title = new UILabel()
                {
                    Font = UIFont.FromName("Arial-BoldMT", 15),
                    TextColor = UIColor.FromRGB(0, 95, 212),
                    TextAlignment = UITextAlignment.Left,
                };
                BT_section = new UIButton();

                BT_section.TouchUpInside += delegate
                {
                    KeyValuePair<string, bool> sectionState;
                    sectionState = new KeyValuePair<string, bool>(Data.Key, !Data.Value);
                    lst_sectionState[(int)section] = sectionState;

                    parentView.UpdateColletionView((int)section);
                };
                baseView.AddSubviews(img_arrow, lbl_title, BT_section);

                //UpdateRow
                Data = lst_sectionState[(int)section];
                lbl_title.Text = Data.Key.Split(";#").Length > 0 ? Data.Key.Split(";#")[0] : Data.Key;
                if (Data.Value)
                {
                    img_arrow.Frame = new CGRect(15, 20, 10, 5);
                    img_arrow.Image = UIImage.FromFile("Icons/icon_arrowUpDown_colapse.png");
                }

                else
                {
                    img_arrow.Frame = new CGRect(15, 20, 5, 10);
                    img_arrow.Image = UIImage.FromFile("Icons/icon_arrowUpDown.png");
                }
                //frame

                lbl_title.Frame = new CGRect(img_arrow.Frame.Right + 10, 15, baseView.Frame.Width - 50, 20);
                BT_section.Frame = new CGRect(0, 0, baseView.Frame.Width, 50);

                return baseView;
            }
            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                KeyValuePair<string, List<BeanWorkflow>> lst_itemInSection = dict_groupWorkFlow.Where(x => x.Key == sectionKeys[indexPath.Section]).First();
                BeanWorkflow tickit = lst_itemInSection.Value[indexPath.Row];
                BroadView_cell_custom cell = new BroadView_cell_custom(cellIdentifier);
                if (indexPath.Row % 2 == 0)
                    cell.BackgroundColor = UIColor.FromRGB(243, 249, 255);
                cell.UpdateRow(tickit, parentView);
                return cell;
            }
        }
        private class BroadView_cell_custom : UITableViewCell
        {
            string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            UIView bg_view = new UIView();
            UIImageView img;
            UILabel lbl_title, lbl_content;
            UIImageView img_favorite;
            UIViewController controller { get; set; }
            UIButton BT_favorite;
            UIActivityIndicatorView favoriteLoading;
            BeanWorkflow workflowSelected;

            public BroadView_cell_custom(NSString cellID) : base(UITableViewCellStyle.Default, cellID)
            {
                Accessory = UITableViewCellAccessory.None;
                viewConfiguration();
            }

            private void viewConfiguration()
            {
                bg_view = new UIView();

                img_favorite = new UIImageView();
                img_favorite.ContentMode = UIViewContentMode.ScaleAspectFill;

                lbl_title = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 13f),
                    TextColor = UIColor.FromRGB(51, 51, 51),
                    TextAlignment = UITextAlignment.Left,
                    LineBreakMode = UILineBreakMode.WordWrap,
                    Lines = 2
                };
                lbl_content = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 11f),
                    TextColor = UIColor.FromRGB(94, 94, 94),
                    TextAlignment = UITextAlignment.Left,
                    LineBreakMode = UILineBreakMode.WordWrap,
                    Lines = 3
                };

                img = new UIImageView();
                //img.BackgroundColor = UIColor.FromRGB(94, 94, 94);
                img.ContentMode = UIViewContentMode.ScaleAspectFill;

                favoriteLoading = new UIActivityIndicatorView();
                favoriteLoading.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Medium;
                favoriteLoading.TintColor = UIColor.FromRGB(65, 80, 134);
                favoriteLoading.HidesWhenStopped = true;

                BT_favorite = new UIButton();
                BT_favorite.ImageEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
                BT_favorite.TouchUpInside += BT_favorite_TouchUpInside;

                bg_view.AddSubviews(new UIView[] { lbl_title, lbl_content, img, img_favorite, BT_favorite, favoriteLoading });
                ContentView.AddSubviews(bg_view);
            }

            private async void BT_favorite_TouchUpInside(object sender, EventArgs e)
            {
                try
                {
                    ProviderControlDynamic p_dynamic = new ProviderControlDynamic();
                    img_favorite.Hidden = true;
                    favoriteLoading.StartAnimating();
                    await Task.Run(() =>
                    {
                        if (p_dynamic.SetFavoriteWorkflow(workflowSelected.WorkflowID, !workflowSelected.Favorite))
                        {
                            ProviderBase p_base = new ProviderBase();
                            p_base.UpdateMasterData<BeanWorkflow>(null, true, CmmVariable.SysConfig.DataLimitDay, false);
                            InvokeOnMainThread(() =>
                            {
                                if (controller.GetType() == typeof(BroadView))
                                {
                                    BroadView broadView = controller as BroadView;
                                    broadView.LoadContent();
                                    //if (broadView.currentWorkFlowCateSelected != null)
                                    //    broadView.loadContentByCateID(broadView.currentWorkFlowCateSelected.ID);
                                    //else
                                    //    broadView.LoadContent();
                                }
                                favoriteLoading.StopAnimating();
                                img_favorite.Hidden = false;
                            });
                        }
                        else
                        {
                            InvokeOnMainThread(() =>
                            {
                                CmmIOSFunction.commonAlertMessage(controller, "BPM", CmmFunction.GetTitle("K_Mess_ActionFalse", "Thao tác không thực hiện được, vui lòng thử lại."));
                                favoriteLoading.StopAnimating();
                                img_favorite.Hidden = false;
                            });
                        }
                    });
                }
                catch (Exception ex)
                {
                    favoriteLoading.StopAnimating();
                    Console.WriteLine("WorkFlowGroup_collectionCell - BT_favorite_TouchUpInside - ERR: " + ex.ToString());
                }
            }

            //private void BT_List_TouchUpInside(object sender, EventArgs e)
            //{
            //    if (controller.GetType() == typeof(BroadView))
            //    {
            //        BroadView broadView = controller as BroadView;
            //        broadView.NavigateToKanBan(workflowSelected, 0);
            //    }
            //}

            //private void BT_Board_TouchUpInside(object sender, EventArgs e)
            //{
            //    if (controller.GetType() == typeof(BroadView))
            //    {
            //        BroadView broadView = controller as BroadView;
            //        broadView.NavigateToKanBan(workflowSelected, 1);
            //    }
            //}

            public void UpdateRow(BeanWorkflow element, UIViewController _controller)
            {
                workflowSelected = element;
                controller = _controller;

                if (CmmVariable.SysConfig.LangCode == "1033")
                    lbl_title.Text = element.TitleEN;
                else //if (CmmVariable.SysConfig.LangCode == "1066")
                    lbl_title.Text = element.Title;

                if (!string.IsNullOrEmpty(element.ImageURL))
                    checkFileLocalIsExist(element.ImageURL, img);
                else
                    img.Image = UIImage.FromFile("Icons/icon_learn_temp.png");

                if (element.Favorite)
                    img_favorite.Image = UIImage.FromFile("Icons/icon_favorite_on.png");
                else
                    img_favorite.Image = UIImage.FromFile("Icons/icon_favorite_off.png");
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                bg_view.Frame = new CGRect(0, 0, ContentView.Frame.Width, ContentView.Frame.Height);
                img.Frame = new CGRect(30, 33, 45, 45);
                lbl_title.Frame = new CGRect(img.Frame.Right + 30, 15, ContentView.Frame.Width - (img.Frame.Right + 30 + 40), 30);//img.Frame.Right + 30 + 40 = (img.Frame.Right + 30 ) + BT_favorite.frame.left
                lbl_content.Frame = new CGRect(lbl_title.Frame.Left, lbl_title.Frame.Bottom + 10, lbl_title.Frame.Width, lbl_title.Frame.Height);
                img_favorite.Frame = new CGRect(ContentView.Frame.Width - 30, 15, 14, 14);
                BT_favorite.Frame = new CGRect(ContentView.Frame.Width - 40, 12, 40, 40);
                favoriteLoading.Frame = new CGRect(ContentView.Frame.Width - 33, 10, 20, 20);
            }

            private async void checkFileLocalIsExist(string url, UIImageView image_view)
            {
                try
                {
                    string filename = url.Split('/').Last();
                    string filepathURL = CmmVariable.M_Domain + "/" + CmmVariable.SysConfig.Subsite + url;
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
                                            //avatar = CmmIOSFunction.MaxResizeImage(image, 200, 200);
                                            image_view.Image = image;
                                        }
                                        else
                                            image_view.Image = UIImage.FromFile("Icons/icon_learn_temp.png");
                                    }
                                    else
                                        image_view.Image = UIImage.FromFile("Icons/icon_learn_temp.png");

                                    image_view.Hidden = false;
                                });

                                if (data != null && avatar != null)
                                {
                                    NSError err = null;
                                    NSData imgData = avatar.AsPNG();
                                    if (imgData.Save(localfilePath, false, out err))
                                        Console.WriteLine("saved as " + localfilePath);
                                    return;
                                }
                                //InvokeOnMainThread(() =>
                                //{
                                //    image_view.BackgroundColor = UIColor.Clear;
                                //});
                            }
                            else
                            {
                                InvokeOnMainThread(() =>
                                {
                                    image_view.Image = UIImage.FromFile("Icons/icon_learn_temp.png");
                                    image_view.BackgroundColor = UIColor.Clear;
                                });
                            }
                        });
                    }
                    else
                    {
                        openFile(filename, image_view);
                        image_view.Hidden = false;
                        image_view.BackgroundColor = UIColor.Clear;
                    }
                }
                catch (Exception ex)
                {
                    //image_view.BackgroundColor = UIColor.Clear;
                    image_view.Image = UIImage.FromFile("Icons/icon_learn_temp.png");
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
                            image_view.Image = UIImage.FromFile("Icons/icon_learn_temp.png");
                        }
                    }
                    else
                    {
                        image_view.Image = UIImage.FromFile("Icons/icon_learn_temp.png");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("ContactView - openFile - Err: " + ex.ToString());
                }
            }
        }
        #endregion


    }
}


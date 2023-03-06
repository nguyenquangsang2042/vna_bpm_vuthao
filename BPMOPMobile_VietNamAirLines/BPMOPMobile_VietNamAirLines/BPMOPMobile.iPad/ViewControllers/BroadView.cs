using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.iPad.CustomControlClass;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using SQLite;
using UIKit;

namespace BPMOPMobile.iPad
{
    public partial class BroadView : UIViewController
    {
        CollectionWorkFlowGroup_Source collectionWorkFlowGroup_Source;
        AppDelegate appD;
        ButtonsActionTopBar buttonActionTopBar;
        ButtonsActionBotBar buttonActionBotBar;
        UIRefreshControl board_refreshControl;
        List<KeyValuePair<string, bool>> lst_sectionState;
        List<BeanWorkflowCategory> lst_category;
        public BeanWorkflowCategory currentWorkFlowCateSelected { get; set; }
        Dictionary<string, List<BeanWorkflow>> dict_groupWorkFlow = new Dictionary<string, List<BeanWorkflow>>();
        Dictionary<string, List<BeanWorkflow>> dict_groupWorkFlow_searchResult = new Dictionary<string, List<BeanWorkflow>>();
        bool IsFirstLoad = true;
        bool filterAll = true; // true: all || false: favorite
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
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            CmmIOSFunction.ResignFirstResponderOnTap(this.View);
            board_refreshControl = new UIRefreshControl();
            //ViewConfiguration();
            LoadListCategory();
            if (lst_category != null && lst_category.Count > 0)
            {
                if (lst_category[0].ID == 0)
                {
                    lst_category[0].IsSelected = true;
                    currentWorkFlowCateSelected = lst_category[0];
                }
                LoadContent();
            }

            #region delegate
            board_refreshControl.ValueChanged += Board_refreshControl_ValueChanged;
            BT_avatar.TouchUpInside += BT_avatar_TouchUpInside;
            tf_search.EditingChanged += Tf_search_EditingChanged;
            BT_category.TouchUpInside += BT_category_TouchUpInside;
            BT_filter_all.TouchUpInside += BT_filter_all_TouchUpInside;
            BT_filter_favorite.TouchUpInside += BT_filter_favorite_TouchUpInside;
            CmmEvent.UpdateLangComplete += CmmEvent_UpdateLangComplete;
            #endregion
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (buttonActionTopBar != null && buttonActionBotBar != null)
            {
                view_top_bar.AddSubviews(buttonActionTopBar);
                view_bot_bar.AddSubviews(buttonActionBotBar);
            }
        }

        #region public - private method
        private void ViewConfiguration()
        {
            //lbl_topBar_title.Text = CmmFunction.GetTitle("TEXT_IPAD_BOARD", "Trang chủ  >  Board");
            tf_search.Placeholder = CmmFunction.GetTitle("TEXT_SEARCH", "Tìm kiếm...");
            lbl_filter_all.Text = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
            lbl_filter_favorite.Text = CmmFunction.GetTitle("TEXT_FAVORITE", "Yêu thích");
            lbl_nhomquytrinh.Text = CmmFunction.GetTitle("TEXT_WORKFLOW_GROUP", "Nhóm quy trình");
            lbl_categorySelected.Text = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
            SetLangTitle();
            board_refreshControl.TintColor = UIColor.FromRGB(65, 80, 134);
            board_refreshControl.AttributedTitle = new NSAttributedString(CmmFunction.GetTitle("TEXT_REFRESH_LOADING", "Loading ..."), firstAttributes);
            Collection_WorkflowCate.AddSubview(board_refreshControl);

            //default filter all
            lbl_filter_all.TextColor = UIColor.FromRGB(51, 95, 179);
            lbl_filter_all.Font = UIFont.FromName("Arial-BoldMT", 15f);
            img_filter_all.Image = UIImage.FromFile("Icons/icon_all3x.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            img_filter_all.TintColor = UIColor.FromRGB(51, 95, 179);

            lbl_filter_favorite.TextColor = UIColor.FromRGB(94, 94, 94);
            lbl_filter_favorite.Font = UIFont.FromName("ArialMT", 15f);
            img_filter_favorite.Image = UIImage.FromFile("Icons/icon_favorite_off.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            img_filter_favorite.TintColor = UIColor.FromRGB(94, 94, 94);

            buttonActionTopBar = ButtonsActionTopBar.Instance;
            buttonActionTopBar.InitFrameView(view_top_bar.Frame);
            view_top_bar.AddSubviews(buttonActionTopBar);

            buttonActionBotBar = ButtonsActionBotBar.Instance;
            buttonActionBotBar.InitFrameView(view_bot_bar.Frame);
            buttonActionBotBar.LoadStatusButton(3);
            view_BotBar_ConstantHeight.Constant = view_BotBar_ConstantHeight.Constant + 10;
            view_bot_bar.AddSubviews(buttonActionBotBar);

            view_filter_category.Layer.BorderColor = UIColor.FromRGB(232, 232, 232).CGColor;
            view_filter_category.Layer.BorderWidth = 0.8f;
            CmmIOSFunction.MakeCornerTopLeftRight(view_lstWorkFlow, 8);

            CmmIOSFunction.CreateCircleButton(BT_avatar);
            string documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string localpath = Path.Combine(documentFolder, CmmVariable.M_Avatar);

            BT_avatar.ImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
            if (File.Exists(localpath))
                BT_avatar.SetImage(UIImage.FromFile(localpath), UIControlState.Normal);
            else
                BT_avatar.SetImage(UIImage.FromFile("Icons/icon_avatar_temp.png"), UIControlState.Normal);

            var flowLayout = new UICollectionViewFlowLayout()
            {
                //ItemSize = new CGSize((float)UIScreen.MainScreen.Bounds.Size.Width / 3.0f, (float)UIScreen.MainScreen.Bounds.Size.Width / 3.0f),
                ItemSize = new CGSize(270, 270),
                HeaderReferenceSize = new CGSize(Collection_WorkflowCate.Frame.Width, 30),
                SectionInset = new UIEdgeInsets(2, 2, 2, 2),
                ScrollDirection = UICollectionViewScrollDirection.Vertical,
                MinimumInteritemSpacing = 2, // minimum spacing between cells
                MinimumLineSpacing = 20 // minimum spacing between rows if ScrollDirection is Vertical or between columns if Horizontal
            };
            Collection_WorkflowCate.SetCollectionViewLayout(flowLayout, true);
            Collection_WorkflowCate.RegisterClassForSupplementaryView(typeof(Custom_CollectionHeader), UICollectionElementKindSection.Header, Custom_CollectionHeader.Key);
            Collection_WorkflowCate.RegisterClassForCell(typeof(WorkFlowGroup_CollectionCell), WorkFlowGroup_CollectionCell.CellID);
            Collection_WorkflowCate.AllowsMultipleSelection = false;
            Collection_WorkflowCate.AlwaysBounceVertical = true;

            CmmIOSFunction.AddShadowForTopORBotBar(view_top, true);
            CmmIOSFunction.AddShadowForTopORBotBar(view_bot_bar, false);
        }

        /// <summary>
        /// Set màu cho trang đang được focus
        /// </summary>
        void SetLangTitle()
        {
            lbl_board.Text = CmmFunction.GetTitle("TEXT_BOARD", "Board").ToUpper();
            //var str_transalte = CmmFunction.GetTitle("TEXT_IPAD_BOARD", "Trang chủ  >  Bảng thống kê");

            var str_transalte = CmmFunction.GetTitle("TEXT_MAINVIEW", "Trang chủ") + " > " + CmmFunction.GetTitle("TEXT_BOARD", "Board");
            //var str_transalte = "Trang chủ  >  " + CmmFunction.GetTitle("TEXT_BOARD", "Board");
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

        private void LoadListCategory()
        {
            lst_category = new List<BeanWorkflowCategory>();
            var conn = new SQLiteConnection(CmmVariable.M_DataPath);

            string query_worflowCategory = @"SELECT * FROM BeanWorkflowCategory ORDER BY [Order]";
            lst_category = conn.Query<BeanWorkflowCategory>(query_worflowCategory);
            if (lst_category != null && lst_category.Count > 0)
            {
                BeanWorkflowCategory cate_all = new BeanWorkflowCategory();
                cate_all.ID = 0;
                cate_all.Title = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
                lst_category.Insert(0, cate_all);
            }
        }

        public void LoadContent()
        {
            var conn = new SQLiteConnection(CmmVariable.M_DataPath);
            string query_worflowCategory = @"SELECT * FROM BeanWorkflowCategory ORDER BY [Order]";
            var lst_cate = conn.Query<BeanWorkflowCategory>(query_worflowCategory);
            if (lst_cate != null && lst_category.Count > 0)
            {
                dict_groupWorkFlow = new Dictionary<string, List<BeanWorkflow>>();
                foreach (var item in lst_cate)
                {
                    if (filterAll)
                    {
                        string query_worflow = @"SELECT * FROM BeanWorkflow WHERE StatusName = ? AND WorkflowCategoryID = ?  AND IsPermission = 1 ORDER BY WorkflowID ASC";
                        List<BeanWorkflow> lst_workflow = conn.Query<BeanWorkflow>(query_worflow, "Active", item.ID);
                        if (lst_workflow != null && lst_workflow.Count > 0)
                            dict_groupWorkFlow.Add(item.Title, lst_workflow);
                    }
                    else
                    {
                        string query_worflow = @"SELECT * FROM BeanWorkflow WHERE StatusName = ? AND Favorite = ? AND WorkflowCategoryID = ? AND IsPermission = 1 ORDER BY WorkflowID ASC";
                        List<BeanWorkflow> lst_workflow = conn.Query<BeanWorkflow>(query_worflow, "Active", true, item.ID);
                        if (lst_workflow != null && lst_workflow.Count > 0)
                            dict_groupWorkFlow.Add(item.Title, lst_workflow);
                    }
                }
            }

            if (dict_groupWorkFlow != null && dict_groupWorkFlow.Count > 0)
            {
                collectionWorkFlowGroup_Source = new CollectionWorkFlowGroup_Source(this, dict_groupWorkFlow, lst_sectionState);
                Collection_WorkflowCate.Source = collectionWorkFlowGroup_Source;
                Collection_WorkflowCate.ReloadData();
                Collection_WorkflowCate.Hidden = false;
            }
            else
            {
                Collection_WorkflowCate.Hidden = true;
                lbl_dodata.Hidden = false;
            }
        }

        public void loadContentByCateID(int WorkFlowCateID)
        {
            try
            {
                var conn = new SQLiteConnection(CmmVariable.M_DataPath);
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
                        else
                        {
                            string query_worflow = @"SELECT * FROM BeanWorkflow WHERE StatusName = ? AND Favorite = ? AND WorkflowCategoryID = ? AND IsPermission = 1 ORDER BY WorkflowID ASC";
                            List<BeanWorkflow> lst_workflow = conn.Query<BeanWorkflow>(query_worflow, "Active", true, item.ID);
                            if (lst_workflow != null && lst_workflow.Count > 0)
                                dict_groupWorkFlow.Add(item.Title, lst_workflow);
                        }
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
                    collectionWorkFlowGroup_Source = new CollectionWorkFlowGroup_Source(this, dict_groupWorkFlow, lst_sectionState);
                    Collection_WorkflowCate.Source = collectionWorkFlowGroup_Source;
                    Collection_WorkflowCate.ReloadData();
                    Collection_WorkflowCate.Hidden = false;
                }
                else
                {
                    Collection_WorkflowCate.Hidden = true;
                    lbl_dodata.Hidden = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("BoardsView - loadContentByCateID - Err: " + ex.ToString());
            }
        }

        public void UpdateColletionView(KeyValuePair<string, bool> _sectionState)
        {
            if (currentWorkFlowCateSelected == null || currentWorkFlowCateSelected.ID == 0)
            {
                var index = lst_sectionState.FindIndex(x => x.Key == _sectionState.Key);
                lst_sectionState[index] = _sectionState;

                collectionWorkFlowGroup_Source = new CollectionWorkFlowGroup_Source(this, dict_groupWorkFlow, lst_sectionState);
                Collection_WorkflowCate.ReloadSections(NSIndexSet.FromIndex(index));
            }
        }

        private void searchData()
        {
            try
            {
                string content = CmmFunction.removeSignVietnamese(tf_search.Text.Trim().ToLowerInvariant());
                if (!string.IsNullOrEmpty(content))
                {
                    var conn = new SQLiteConnection(CmmVariable.M_DataPath);
                    string query_worflowCategory = @"SELECT * FROM BeanWorkflowCategory";
                    List<BeanWorkflowCategory> lst_category = conn.Query<BeanWorkflowCategory>(query_worflowCategory);
                    if (lst_category != null && lst_category.Count > 0)
                    {
                        dict_groupWorkFlow_searchResult = new Dictionary<string, List<BeanWorkflow>>();
                        foreach (var item in lst_category)
                        {
                            string query_worflow = @"SELECT * FROM BeanWorkflow WHERE StatusName = ? AND WorkflowCategoryID = ? AND IsPermission = 1 ORDER BY WorkflowID ASC";
                            List<BeanWorkflow> lst_workflow = conn.Query<BeanWorkflow>(query_worflow, "Active", item.ID);

                            var items = from wf in lst_workflow
                                        where (!string.IsNullOrEmpty(wf.Title) && CmmFunction.removeSignVietnamese(wf.Title.ToLowerInvariant()).Contains(content))
                                        select wf;

                            if (items != null && items.Count() > 0)
                            {
                                dict_groupWorkFlow_searchResult.Add(item.Title, items.ToList());
                            }
                        }
                    }

                    if (dict_groupWorkFlow_searchResult != null && dict_groupWorkFlow_searchResult.Count() > 0)
                    {
                        Collection_WorkflowCate.Hidden = false;
                        collectionWorkFlowGroup_Source = new CollectionWorkFlowGroup_Source(this, dict_groupWorkFlow_searchResult, lst_sectionState);
                        Collection_WorkflowCate.Source = collectionWorkFlowGroup_Source;
                    }
                    else
                    {
                        Collection_WorkflowCate.Hidden = true;
                        lbl_dodata.Hidden = false;
                    }
                }
                else
                {
                    Collection_WorkflowCate.Hidden = false;
                    collectionWorkFlowGroup_Source = new CollectionWorkFlowGroup_Source(this, dict_groupWorkFlow, lst_sectionState);
                    Collection_WorkflowCate.Source = collectionWorkFlowGroup_Source;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ToDoDetailView - Tf_search_EditingChanged - Err: " + ex.ToString());
            }
        }

        private void FilterSwitchUpdateLayout()
        {
            if (filterAll)
            {
                lbl_filter_all.TextColor = UIColor.FromRGB(51, 95, 179);
                lbl_filter_all.Font = UIFont.FromName("Arial-BoldMT", 15f);
                img_filter_all.Image = UIImage.FromFile("Icons/icon_all3x.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                img_filter_all.TintColor = UIColor.FromRGB(51, 95, 179);

                lbl_filter_favorite.TextColor = UIColor.FromRGB(94, 94, 94);
                lbl_filter_favorite.Font = UIFont.FromName("ArialMT", 15f);
                img_filter_favorite.Image = UIImage.FromFile("Icons/icon_favorite_off.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                img_filter_favorite.TintColor = UIColor.FromRGB(94, 94, 94);
            }
            else
            {
                lbl_filter_all.TextColor = UIColor.FromRGB(94, 94, 94);
                lbl_filter_all.Font = UIFont.FromName("ArialMT", 15f);
                img_filter_all.Image = UIImage.FromFile("Icons/icon_all3x.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                img_filter_all.TintColor = UIColor.FromRGB(94, 94, 94);//Icons/icon_all3x.png.

                lbl_filter_favorite.TextColor = UIColor.FromRGB(51, 95, 179);
                lbl_filter_favorite.Font = UIFont.FromName("Arial-BoldMT", 15f);
                img_filter_favorite.Image = UIImage.FromFile("Icons/icon_favorite_off.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                img_filter_favorite.TintColor = UIColor.FromRGB(51, 95, 179);
            }
        }

        private void ToggleMenuCategory()
        {
            Custom_WorkflowCategory custom_WorkflowCategory = Custom_WorkflowCategory.Instance;
            if (custom_WorkflowCategory.Superview != null)
                custom_WorkflowCategory.RemoveFromSuperview();
            else
            {
                custom_WorkflowCategory.ItemNoIcon = false;
                custom_WorkflowCategory.viewController = this;
                nfloat est_height = custom_WorkflowCategory.RowHeigth;
                if (lst_category.Count > 10)
                    est_height = custom_WorkflowCategory.RowHeigth * 10;
                else
                    est_height = custom_WorkflowCategory.RowHeigth * lst_category.Count;

                custom_WorkflowCategory.InitFrameView(new CGRect(view_filter_category.Frame.Left - 50, view_filter_category.Frame.Bottom + 2, view_filter_category.Frame.Width + 50, est_height + 15));
                custom_WorkflowCategory.AddShadowForView();
                custom_WorkflowCategory.ListItemMenu = lst_category;
                custom_WorkflowCategory.TableLoadData();

                view_lstWorkFlow.AddSubview(custom_WorkflowCategory);
                view_lstWorkFlow.BringSubviewToFront(custom_WorkflowCategory);
            }
        }

        public void HandelMenuWorkFlowCategoryResult(BeanWorkflowCategory _workFlowCate)
        {
            if (_workFlowCate != null)
            {
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

        private void LoadContent_bk()
        {
            //WorkFlowItemDemo item = new WorkFlowItemDemo() { ID = 0, iconUrl = "icon_learn_temp.png", title = "Đăng kí đi làm ngoài giờ", isExpand = true, workFlowID = 2 };
            //WorkFlowItemDemo item1 = new WorkFlowItemDemo() { ID = 1, iconUrl = "icon_pay_temp.png", title = "Thanh toán công tác phí", isExpand = true, workFlowID = 1 };
            //WorkFlowItemDemo item2 = new WorkFlowItemDemo() { ID = 2, iconUrl = "icon_recruitment_temp.png", title = "Đề nghị tuyển dụng", isExpand = true, workFlowID = 3 };
            //WorkFlowItemDemo item3 = new WorkFlowItemDemo() { ID = 3, iconUrl = "icon_training_temp.png", title = "Đề nghị tuyển dụng", isExpand = true, workFlowID = 4 };
            //WorkFlowItemDemo item4 = new WorkFlowItemDemo() { ID = 4, iconUrl = "", title = "Quy trình đào tạo", isExpand = true, workFlowID = 5 };

            //List<WorkFlowItemDemo> lst_item = new List<WorkFlowItemDemo>();
            //lst_item.AddRange(new[] { item, item1, item2, item3, item4, item2, item3, item4 });

            //List<WorkFlowItemDemo> lst_item1 = new List<WorkFlowItemDemo>();
            //lst_item1.AddRange(new[] { item1, item2 });

            //List<WorkFlowItemDemo> lst_item2 = new List<WorkFlowItemDemo>();
            //lst_item2.AddRange(new[] { item2, item1, item2 });

            //List<WorkFlowItemDemo> lst_item3 = new List<WorkFlowItemDemo>();
            //lst_item3.AddRange(new[] { item3, item1, item2, item3, item4, item1, item2 });

            //dict_groupWorkFlow.Add("Nhân sự", lst_item);
            //dict_groupWorkFlow.Add("Marketing", lst_item1);
            //dict_groupWorkFlow.Add("Kinh doanh", lst_item2);
            //dict_groupWorkFlow.Add("Kế toán", lst_item3);

            //table_groupWorkFlow.Source = new GroupWorkFlow_TableSource(this, dict_groupWorkFlow);

            //ClassMenu m1 = new ClassMenu() { ID = 0, section = -1, title = "Tất cả", isSelected = true };
            //ClassMenu m2 = new ClassMenu() { ID = 1, section = 0, title = "Nhân sự" };
            //ClassMenu m3 = new ClassMenu() { ID = 2, section = 1, title = "Marketing" };
            //ClassMenu m4 = new ClassMenu() { ID = 3, section = 2, title = "Kinh doanh" };
            //ClassMenu m5 = new ClassMenu() { ID = 4, section = 3, title = "Kế toán" };

            //lst_menuItem.AddRange(new[] { m1, m2, m3, m4, m5 });
            //currentMenu = lst_menuItem[0];

            //string dataString1 = @"[{'ShowType':true,'IsShowHint':false,'ViewRows':[{'RowType':1,'Elements':[{'DataType':'textinput','DataSource':null,'ListProprety':null,'ID':1,'Title':'Về việc','Value':'','Enable':true,'IsRequire':false}],'ID':1,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinput','DataSource':null,'ListProprety':null,'ID':1,'Title':'Kính trình','Value':'','Enable':true,'IsRequire':false}],'ID':2,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputformat','DataSource':null,'ListProprety':null,'ID':1,'Title':'Căn cứ','Value':'','Enable':true,'IsRequire':false}],'ID':3,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputmultiline','DataSource':null,'ListProprety':null,'ID':1,'Title':'Chi tiết','Value':'','Enable':true,'IsRequire':true}],'ID':4,'Title':null,'Value':null,'Enable':false},{'RowType':3,'Elements':[{'DataType':null,'DataSource':null,'ListProprety':null,'ID':1,'Title':'Người soạn thảo','Value':'Test Admin','Enable':false,'IsRequire':false},{'DataType':null,'DataSource':null,'ListProprety':null,'ID':2,'Title':'Chức vụ','Value':'Nhân viên Kinh doanh','Enable':false,'IsRequire':false},{'DataType':null,'DataSource':null,'ListProprety':null,'ID':3,'Title':'Đơn vị','Value':'Vũ Thảo Co. ltd','Enable':false,'IsRequire':false}],'ID':5,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputmultiline','DataSource':null,'ListProprety':null,'ID':1,'Title':'Ghi chú','Value':'','Enable':true,'IsRequire':false}],'ID':6,'Title':null,'Value':null,'Enable':false}],'ID':1,'Title':'Báo Cáo','Value':null,'Enable':false}]";
            //string dataString2 = @"[{'ShowType':true,'IsShowHint':false,'ViewRows':[{'RowType':1,'Elements':[{'DataType':'textinput','DataSource':null,'ListProprety':null,'ID':1,'Title':'Về việc','Value':'','Enable':true,'IsRequire':false}],'ID':1,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinput','DataSource':null,'ListProprety':null,'ID':1,'Title':'Kính trình','Value':'','Enable':true,'IsRequire':false}],'ID':2,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputformat','DataSource':null,'ListProprety':null,'ID':1,'Title':'Căn cứ','Value':'','Enable':true,'IsRequire':false}],'ID':3,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputmultiline','DataSource':null,'ListProprety':null,'ID':1,'Title':'Chi tiết','Value':'','Enable':true,'IsRequire':true}],'ID':4,'Title':null,'Value':null,'Enable':false},{'RowType':3,'Elements':[{'DataType':null,'DataSource':null,'ListProprety':null,'ID':1,'Title':'Người soạn thảo','Value':'Test Admin','Enable':false,'IsRequire':false},{'DataType':null,'DataSource':null,'ListProprety':null,'ID':2,'Title':'Chức vụ','Value':'Nhân viên Kinh doanh','Enable':false,'IsRequire':false},{'DataType':null,'DataSource':null,'ListProprety':null,'ID':3,'Title':'Đơn vị','Value':'Vũ Thảo Co. ltd','Enable':false,'IsRequire':false}],'ID':5,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputmultiline','DataSource':null,'ListProprety':null,'ID':1,'Title':'Ghi chú','Value':'','Enable':true,'IsRequire':false}],'ID':6,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'inputattachmentvertical','DataSource':null,'ListProprety':null,'ID':4,'Title':'File đính kèm','Value':'','Enable':false}],'ID':7,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'inputworkrelated','DataSource':null,'ListProprety':null,'ID':4,'Title':'Quy trình / Công việc liên kết','Value':'','Enable':false}],'ID':8,'Title':null,'Value':null,'Enable':false}],'ID':1,'Title':'Báo Cáo','Value':null,'Enable':false}]";

            //JArray json = JArray.Parse(dataString2);
            //lst_section = json.ToObject<List<ViewSection>>();

            //table_content_right.Source = new Control_TableSource(lst_section, this);
            //table_content_right.ReloadData();
        }

        /// <summary>
        /// TypeView - 0: Board, 1: List
        /// </summary>
        /// <param name="workflowSelected"></param>
        /// <param name="typeView"></param>
        public void NavigateToKanBan(BeanWorkflow workflowSelected, int typeView)
        {
            KanBanView kanBanView = (KanBanView)Storyboard.InstantiateViewController("KanBanView");
            kanBanView.SetContent(workflowSelected, typeView);
            this.NavigationController.PushViewController(kanBanView, true);
        }
        #endregion

        #region event
        private void BT_avatar_TouchUpInside(object sender, EventArgs e)
        {
            appD.menu.UpdateItemSelect(3);
            appD.SlideMenuController.OpenLeft();
        }
        private void BT_category_TouchUpInside(object sender, EventArgs e)
        {
            ToggleMenuCategory();
        }
        private void BT_filter_favorite_TouchUpInside(object sender, EventArgs e)
        {
            filterAll = false;
            tf_search.Text = string.Empty;
            HandelMenuWorkFlowCategoryResult(lst_category[0]);
            LoadContent();
            FilterSwitchUpdateLayout();
        }
        private void BT_filter_all_TouchUpInside(object sender, EventArgs e)
        {
            filterAll = true;
            tf_search.Text = string.Empty;
            HandelMenuWorkFlowCategoryResult(lst_category[0]);
            LoadContent();
            FilterSwitchUpdateLayout();
        }
        private void Tf_search_EditingChanged(object sender, EventArgs e)
        {
            searchData();
        }
        private async void Board_refreshControl_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                board_refreshControl.BeginRefreshing();
                ProviderBase provider = new ProviderBase();
                await Task.Run(() =>
                {
                    provider.UpdateAllMasterData(true);
                    //provider.UpdateSyncBackgroundData3(true);
                    provider.UpdateAllDynamicData(true);

                    InvokeOnMainThread(() =>
                    {
                        LoadListCategory();
                        HandelMenuWorkFlowCategoryResult(currentWorkFlowCateSelected);
                        board_refreshControl.EndRefreshing();
                    });
                });
            }
            catch (Exception ex)
            {
                board_refreshControl.EndRefreshing();
                Console.WriteLine("Error - BoardView - refreshControl_valuechange - Er: " + ex.ToString());
            }
        }
        private void CmmEvent_UpdateLangComplete(object sender, CmmEvent.UpdateEventArgs e)
        {
            try
            {
                InvokeOnMainThread(() =>
                {
                    //lbl_topBar_title.Text = CmmFunction.GetTitle("TEXT_IPAD_BOARD", "Trang chủ  >  Board");
                    SetLangTitle();
                    lbl_categorySelected.Text = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
                    tf_search.Placeholder = CmmFunction.GetTitle("TEXT_SEARCH", "Tìm kiếm...");
                    lbl_filter_all.Text = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
                    lbl_filter_favorite.Text = CmmFunction.GetTitle("TEXT_FAVORITE", "Yêu thích");
                    lbl_nhomquytrinh.Text = CmmFunction.GetTitle("TEXT_WORKFLOW_GROUP", "Nhóm quy trình");
                    board_refreshControl.AttributedTitle = new NSAttributedString(CmmFunction.GetTitle("TEXT_REFRESH_LOADING", "Loading ..."), firstAttributes);
                    Collection_WorkflowCate.ReloadData();

                    Custom_WorkflowCategory custom_WorkflowCategory = Custom_WorkflowCategory.Instance;
                    custom_WorkflowCategory.TableLoadData();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("WorkFlowDetailView - CmmEvent_UpdateLangComplete - ERR: " + ex.ToString());
            }
        }
        #endregion

        #region custom
        #region collection ticket categories
        public class CollectionWorkFlowGroup_Source : UICollectionViewSource
        {
            BroadView parentView { get; set; }
            public static Dictionary<string, List<BeanWorkflow>> dict_groupWorkFlow;
            List<string> sectionKeys;
            //bool => Colapse
            List<KeyValuePair<string, bool>> lst_sectionState;
            public List<BeanWorkflow> items;
            public CollectionWorkFlowGroup_Source(BroadView _parentview, Dictionary<string, List<BeanWorkflow>> _dict_groupWorkFlow, List<KeyValuePair<string, bool>> _sectionState)
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
                    lst_sectionState = new List<KeyValuePair<string, bool>>();
                    sectionKeys = new List<string>();
                    //sectionKeys = items.Select(x => x.WorkflowCategoryID.Value).Distinct().ToList();

                    foreach (var item in dict_groupWorkFlow)
                    {
                        sectionKeys.Add(item.Key);
                        KeyValuePair<string, bool> keypair_section = new KeyValuePair<string, bool>(item.Key, false);
                        lst_sectionState.Add(keypair_section);
                    }
                }
            }

            public override nint NumberOfSections(UICollectionView collectionView)
            {
                return dict_groupWorkFlow.Count;
            }
            public override nint GetItemsCount(UICollectionView collectionView, nint section)
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
            public override bool ShouldHighlightItem(UICollectionView collectionView, NSIndexPath indexPath)
            {
                return true;
            }
            public override UICollectionReusableView GetViewForSupplementaryElement(UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
            {
                if (elementKind == "UICollectionElementKindSectionHeader")
                {
                    Custom_CollectionHeader headerView = collectionView.DequeueReusableSupplementaryView(elementKind, Custom_CollectionHeader.Key, indexPath) as Custom_CollectionHeader;
                    headerView.boardView = parentView;
                    headerView.UpdateRow(lst_sectionState[indexPath.Section]);
                    return headerView;
                }
                else if (elementKind == "UICollectionElementKindSectionFooter")
                {
                    return null;
                }
                else
                    return null;
            }
            public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
            {
                KeyValuePair<string, List<BeanWorkflow>> lst_itemInSection = dict_groupWorkFlow.Where(x => x.Key == sectionKeys[indexPath.Section]).First();
                BeanWorkflow tickit = lst_itemInSection.Value[indexPath.Row];
                parentView.NavigateToKanBan(tickit, 0);
            }
            public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
            {
                KeyValuePair<string, List<BeanWorkflow>> lst_itemInSection = dict_groupWorkFlow.Where(x => x.Key == sectionKeys[indexPath.Section]).First();
                BeanWorkflow tickit = lst_itemInSection.Value[indexPath.Row];
                var cell = (WorkFlowGroup_CollectionCell)collectionView.DequeueReusableCell(WorkFlowGroup_CollectionCell.CellID, indexPath);
                cell.UpdateRow(tickit, parentView, true);
                return cell;
            }
        }
        #endregion
        #endregion
    }
}
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
using Newtonsoft.Json;
using SQLite;
using UIKit;
using Xamarin.iOS;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class FollowListViewController : UIViewController
    {
        bool isFirst = true, isFilter, isLoadMore = true, isSearch = false;//, tab_toMe = true;
        //private bool isFilterServer;
        int follow_count, fromMe_count;
        UIRefreshControl refreshControl;

        List<BeanAppBaseExt> lst_appBase_follow = new List<BeanAppBaseExt>();
        Dictionary<string, List<BeanAppBaseExt>> dict_todo;
        //Dictionary<string, bool> dict_sectionTodo;
        //Dictionary<string, List<BeanAppBaseExt>> dict_workflow;
        //Dictionary<string, bool> dict_sectionWorkFlow;
        private readonly nfloat height_search = 44;
        bool isOnline = true;

        public FollowListViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            ViewConfiguration();

            ButtonMenuStyleChange(BT_todo, true, 0);
            ButtonMenuStyleChange(BT_fromMe, false, 1);

            //LoadDataFilterTodo();
            LoadData();

            //To me
            BT_todo.BackgroundColor = UIColor.White;
            BT_todo.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);
            BT_fromMe.BackgroundColor = UIColor.Clear;
            BT_fromMe.SetTitleColor(UIColor.White, UIControlState.Normal);

            BT_avatar.TouchUpInside += BT_avatar_TouchUpInside;
            BT_todo.TouchUpInside += BT_todo_TouchUpInside;
            BT_fromMe.TouchUpInside += BT_fromMe_TouchUpInside;
            BT_search.TouchUpInside += BT_search_TouchUpInside;
            refreshControl.ValueChanged += RefreshControl_ValueChanged;

            searchBar.TextChanged += SearchBar_TextChanged;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            //height_search = constraint_heightSearch.Constant;
            //isSearch = true;
            //SearchToggle();
            //if (!isFirst) RefreshControl_ValueChanged(null, null);
            //isFirst = false;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            /*
            if (!isFisrt && !isFilter)
            {
                isLoadMore = true;
                if (tab_toMe)
                {
                    FilterServerTodo(false);
                }
                else
                {
                    FilterServerFromMe(false);
                }
            }*/
            //if (!isFirst) RefreshControl_ValueChanged(null, null);
            //isFirst = false;
        }

        private void SearchBar_TextChanged(object sender, UISearchBarTextChangedEventArgs e)
        {
            try
            {
                string content = CmmFunction.removeSignVietnamese(searchBar.Text.Trim().ToLowerInvariant());
                if (!string.IsNullOrEmpty(content))
                {
                    var items = from item in lst_appBase_follow
                                where ((!string.IsNullOrEmpty(item.Title) && CmmFunction.removeSignVietnamese(item.Title.ToLowerInvariant()).Contains(content)) ||
                                           (!string.IsNullOrEmpty(item.Title) && item.Title.ToLowerInvariant().Contains(content)) ||
                                           (!string.IsNullOrEmpty(item.Content) && CmmFunction.removeSignVietnamese(item.Content.ToLowerInvariant()).Contains(content)))
                                orderby item.Title
                                select item;

                    if (items != null && items.Count() > 0)
                    {
                        //dict_workflow = new Dictionary<string, List<BeanAppBaseExt>>();
                        /*lst_notify_cxl_results = items.ToList();
                        if (dict_workflow.ContainsKey("Today"))
                            dict_workflow["Today"] = lst_notify_cxl_results;
                        else
                            dict_workflow.Add("Today", lst_notify_cxl_results);

                        table_content.Source = new Todo_TableSource(dict_workflow, this, query_action);
                        table_content.ReloadData();*/
                        isLoadMore = false;
                        SortListAppBase(items.ToList());
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
                    SortListAppBase();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("RequestListView - SearchBar_user_TextChanged - Err: " + ex.ToString());
            }
        }

        public void LoadData()
        {
            lbl_title.Text = CmmVariable.SysConfig.LangCode == "1033" ? "Follow" : "Theo dõi";

            isOnline = Reachability.detectNetWork();
            if (isOnline)
                LoadDataOnLine();
            else
                LoadDataOffLine();
        }

        void LoadDataOffLine()
        {
            var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
            try
            {
                //CheckQuery(conn);

                List<CountNum> lst_count_appBase_follow = new List<CountNum>();

                //dict_workflow = new Dictionary<string, List<BeanAppBaseExt>>();
                //dict_sectionWorkFlow = new Dictionary<string, bool>();
                lst_appBase_follow = new List<BeanAppBaseExt>();

                //count
                var queryCount = CreateQueryFollow(true);
                lst_count_appBase_follow = conn.Query<CountNum>(queryCount);
                if (lst_count_appBase_follow != null && lst_count_appBase_follow.Count > 0)
                {
                    fromMe_count = lst_count_appBase_follow[0].count;
                }
                else
                {
                    fromMe_count = 0;
                }

                //data
                var queryLstObj = CreateQueryFollow(false);
                lst_appBase_follow = conn.Query<BeanAppBaseExt>(queryLstObj, CmmVariable.M_DataLimitRow, 0);
                if (lst_appBase_follow != null && lst_appBase_follow.Count > 0)
                {
                    SortListAppBase();
                    //table_content.ScrollToRow(NSIndexPath.FromItemSection(0, 0), UITableViewScrollPosition.Top, true);
                }
                else
                {
                    table_content.Source = null;
                    table_content.ReloadData();

                    table_content.Alpha = 0;
                    lbl_nodata.Hidden = false;
                }
                loadData_count(fromMe_count, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("FollowListView - LoadData: " + ex.ToString());
                fromMe_count = 0;
                loadData_count(fromMe_count, false);

                table_content.Source = null;
                table_content.ReloadData();

                table_content.Alpha = 0;
                lbl_nodata.Hidden = false;
            }
            finally
            {
                conn.Close();
            }
        }

        void LoadDataOnLine()
        {
            GetCountNumber();
            loadData_count(follow_count, false);
            GetListObj();
        }

        void GetCountNumber()
        {
            try
            {
                string count = "";
                count = new ProviderControlDynamic().GetListCountVDT_VTBD(CmmVariable.KEY_COUNT_FOLLOW);

                if (!string.IsNullOrEmpty(count))
                {
                    var str = count.Split(";#");
                    if (string.Compare(str[0], CmmVariable.KEY_COUNT_FOLLOW) == 0)
                    {
                        if (!int.TryParse(str[1], out follow_count))
                        {
                            follow_count = 0;
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
            follow_count = 0;
        }

        void GetListObj(bool isLoadMore = false)
        {
            List<BeanAppBaseExt> lstObj = new List<BeanAppBaseExt>();
            try
            {
                lstObj = new ProviderBase().LoadMoreDataTFromSerVer(CmmVariable.KEY_GET_FOLLOW, 20, isLoadMore ? lst_appBase_follow.Count : 0);

                if (lstObj != null && lstObj.Count > 0)
                {
                    if (isLoadMore)
                    {
                        if (lst_appBase_follow == null) lst_appBase_follow = new List<BeanAppBaseExt>();
                        lst_appBase_follow.AddRange(lstObj);
                    }
                    else
                        lst_appBase_follow = lstObj;

                    SortListAppBase();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetListObj - Err: " + ex.ToString());
            }

            if (!isLoadMore && (lst_appBase_follow == null || lst_appBase_follow.Count == 0))
            {
                lst_appBase_follow = new List<BeanAppBaseExt>();
                table_content.Alpha = 0;
                lbl_nodata.Hidden = false;
            }
        }

        /*void CheckQuery(SQLiteConnection conn)
        {
            var query = "SELECT * FROM BeanNotify WHERE Content LIKE '%129%' OR Title LIKE '%129%'";
            var lst = conn.Query<BeanAppBaseExt>(query);

            var query1 = "SELECT * FROM BeanAppBase AB "
                + "INNER JOIN (SELECT StartDate, Read, SPItemId, SubmitAction, SendUnit, Type, Status, Content FROM BeanNotify GROUP BY SPItemId) NOTI ON AB.ID = NOTI.SPItemId WHERE NOTI.Content LIKE '%129%' OR Title LIKE '%129%'";
            var lst1 = conn.Query<BeanAppBaseExt>(query1).FirstOrDefault();

            var query2 = "SELECT * FROM BeanWorkflowFollow WHERE WorkflowItemId = " + (lst1 != null ? lst1.ID : 0);
            var lst2 = conn.Query<BeanAppBaseExt>(query2);
        }*/

        async void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                refreshControl.BeginRefreshing();
                ProviderBase provider = new ProviderBase();
                ProviderUser p_user = new ProviderUser();

                string documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                //string localpath = Path.Combine(documentFolder, CmmVariable.M_Avatar);

                await Task.Run(() =>
                {
                    isLoadMore = true;

                    provider.UpdateAllMasterData(true);
                    provider.UpdateAllDynamicData(true);

                    //p_user.UpdateCurrentUserInfo(localpath);

                    InvokeOnMainThread(() =>
                    {
                        LoadData();
                        refreshControl.EndRefreshing();
                        //if (File.Exists(localpath))
                        //    BT_avatar.SetImage(UIImage.FromFile(localpath), UIControlState.Normal);

                        /*if (tab_toMe)
                        {
                            BT_todo.BackgroundColor = UIColor.White;
                            BT_todo.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);
                            BT_fromMe.BackgroundColor = UIColor.Clear;
                            BT_fromMe.SetTitleColor(UIColor.White, UIControlState.Normal);

                            FilterServerTodo(false);
                        }
                        else
                        {
                            tab_toMe = false;
                            ToggleTodo();
                            //if (isFilterServer)
                            //    FilterServerFromMe(false);
                        }*/

                    });
                });
            }
            catch (Exception ex)
            {
                refreshControl.EndRefreshing();
                Console.WriteLine("Error - FollowListViewController - refreshControl_valuechange - Er: " + ex.ToString());
            }
        }

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
                    if (!string.IsNullOrEmpty(searchBar.Text.Trim()))
                    {
                        isSearch = true;
                        searchBar.BecomeFirstResponder();
                        BT_search.Enabled = true;
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
                        BT_search.Enabled = true;
                        BT_search.TintColor = UIColor.FromRGB(0, 0, 0);
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
                    BT_search.Enabled = true;
                    BT_search.TintColor = UIColor.FromRGB(40, 176, 255);
                }
            }
            else
            {
                /*if (!string.IsNullOrEmpty(searchBar.Text.Trim()))
                {
                    isSearch = false;
                    this.View.EndEditing(true);
                    BT_search.Enabled = true;
                }
                else
                {*/
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
                BT_search.Enabled = true;
                BT_search.TintColor = UIColor.FromRGB(0, 0, 0);
                //}
                searchBar.Text = "";
                SortListAppBase();
            }
        }

        private void BT_fromMe_TouchUpInside(object sender, EventArgs e)
        {
        }

        private void BT_todo_TouchUpInside(object sender, EventArgs e)
        {
        }

        private void BT_avatar_TouchUpInside(object sender, EventArgs e)
        {
            this.NavigationController.PopViewController(true);
        }

        #region common
        private void ViewConfiguration()
        {
            SetConstraint();

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
            //    headerView_constantHeight.Constant = 70;
            //}
            //heightTopViewStatus = constraint_topView_status.Constant;

            BT_avatar.SetImage(UIImage.FromFile("Icons/icon_back20.png"), UIControlState.Normal);

            refreshControl = new UIRefreshControl();
            refreshControl.TintColor = UIColor.FromRGB(9, 171, 78);
            var firstAttributes = new UIStringAttributes
            {
                Font = UIFont.FromName("ArialMT", 12f)
            };
            refreshControl.AttributedTitle = new NSAttributedString(CmmFunction.GetTitle("TEXT_REFRESH_LOADING", "Loading ..."), firstAttributes);
            //table_content.ContentInset = new UIEdgeInsets(-5, 0, 0, 0);
            table_content.AddSubview(refreshControl);

            constraint_heightSearch.Constant = 0;
        }

        private void SetConstraint()
        {
            headerView_constantHeight.Constant = 45 + 10 + CmmIOSFunction.GetHeaderViewHeight();

            contraint_heightViewNavBot.Constant = 0;
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
                string str_follow = string.Empty;
                // yêu cầu cần xử lý

                //string query_vcxl = string.Format("SELECT Count(*) as count FROM BeanNotify WHERE Status = 0");
                //List<CountNum> countnum_vcxl = conn.Query<CountNum>(query_vcxl);

                //if (countnum_vcxl != null)
                //toMe_count = countnum_vcxl[0].count;
                /*if (_isToMe)
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

                }*/

                follow_count = _dataCount;
                var txtFollowCount = "";
                str_follow = CmmVariable.SysConfig.LangCode == "1033" ? "Follow" : "Theo dõi";

                if (follow_count >= 100)
                {
                    txtFollowCount = str_follow + " (99+)";
                }
                else if (follow_count > 0 && follow_count < 100)
                {
                    if (follow_count > 0 && follow_count < 10)
                        txtFollowCount = str_follow + " (0" + follow_count.ToString() + ")";// hien thi 2 so vd: 08 
                    else
                        txtFollowCount = str_follow + " (" + follow_count.ToString() + ")";
                }
                else
                {
                    txtFollowCount = str_follow;
                }

                var indexA = txtFollowCount.IndexOf('(');

                var att = new NSMutableAttributedString(txtFollowCount);
                att.AddAttribute(UIStringAttributeKey.Font, UIFont.FromName("ArialMT", 14f), new NSRange(0, txtFollowCount.Length));
                att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(0, txtFollowCount.Length));
                if (indexA > -1) att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(255, 122, 58), new NSRange(indexA, txtFollowCount.Length - indexA));
                lbl_title.AttributedText = att;
            }
            catch (Exception ex)
            {
                Console.WriteLine("FollowListViewController - loadData - Err: " + ex.ToString());
            }
        }

        private void ReloadMoreData(int _index, List<BeanAppBaseExt> _lst_vcxl, string _query)
        {
            if (_index == 0)
            {
                lst_appBase_follow = _lst_vcxl;
                table_content.Source = new Todo_TableSource(dict_todo, this);
                table_content.ReloadData();
            }
        }

        private void ToggleTodo()
        {
            /*if (tab_toMe) // dang la trang thai todo cua toi
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
            }*/
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
                            if (follow_count >= 100)
                                str_toMe_count = " (99+)";
                            else if (follow_count > 0 && follow_count < 100)
                            {
                                if (follow_count > 0 && follow_count < 10)
                                    str_toMe_count = " (" + "0" + follow_count.ToString() + ")"; // hien thi 2 so vd: 08
                                else
                                    str_toMe_count = " (" + follow_count.ToString() + ")";
                            }
                            else
                                str_toMe_count = "";

                            str_transalte = str_transalte + str_toMe_count;
                        }

                        var indexA = str_transalte.IndexOf('(');
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

                        var indexA = str_transalte.IndexOf('(');
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
                            if (follow_count >= 100)
                                str_toMe_count = " (99+)";
                            else if (follow_count > 0 && follow_count < 100)
                            {
                                if (follow_count > 0 && follow_count < 10)
                                    str_toMe_count = " (" + "0" + follow_count.ToString() + ")"; // hien thi 2 so vd: 08
                                else
                                    str_toMe_count = " (" + follow_count.ToString() + ")";
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

        private void NavigateToDetails(BeanAppBaseExt beanAppBase)
        {
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
            else //chi tiet phieu
            {
                RequestDetailsV2 v2 = (RequestDetailsV2)Storyboard.InstantiateViewController("RequestDetailsV2");
                v2.SetContentFromFollowView(this, beanAppBase);
                this.NavigationController.PushViewController(v2, true);
            }
        }

        public async void LoadmoreData()
        {
            view_loadmore.Hidden = false;
            indicator_loadmore.StartAnimating();

            await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.5f));
                InvokeOnMainThread(() =>
                {
                    /*if (tab_toMe)
                        LoadMoreDataTodo();
                    else
                        LoadMoreDataFromMe();*/
                    LoadMoreDatafollow();
                    indicator_loadmore.StopAnimating();
                    view_loadmore.Hidden = true;
                });
            });
        }
        #endregion

        /// <summary>
        /// Viec Den toi local
        /// </summary>
        /// <param name="_statusIndex">0: Tatca | 1: Can XL | 2: Da XL</param>
        /// <param name="_dueDateIndex">0: Tatca | 1: QuaHan | 2: TrongHan </param>
        /*private void LoadDataFilterTodo()
        {
            try
            {
                var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                string query = string.Empty;
                string queryCount = string.Empty;
                List<CountNum> lst_count_appBase_cxl = new List<CountNum>();

                dict_todo = new Dictionary<string, List<BeanAppBaseExt>>();
                dict_sectionTodo = new Dictionary<string, bool>();
                lst_appBase_follow = new List<BeanAppBaseExt>();

                //getcount
                queryCount = CreateQueryFollow(true);
                lst_count_appBase_cxl = conn.Query<CountNum>(queryCount);
                if (lst_count_appBase_cxl != null && lst_count_appBase_cxl.Count > 0)
                {
                    follow_count = lst_count_appBase_cxl.First().count;
                }
                else
                {
                    follow_count = 0;
                    loadData_count(follow_count, true);
                }
                //data
                query = CreateQueryFollow(false);
                lst_appBase_follow = conn.Query<BeanAppBaseExt>(query, CmmVariable.M_DataLimitRow, 0);
                string a = JsonConvert.SerializeObject(lst_appBase_follow);
                if (lst_appBase_follow != null && lst_appBase_follow.Count > 0)
                {
                    loadData_count(follow_count, true);
                    SortListAppBase();
                }
                else
                {
                    table_content.Alpha = 0;
                    lbl_nodata.Hidden = false;
                }
            }
            catch (Exception ex)
            {
                follow_count = 0;
                loadData_count(follow_count, true);
                table_content.Source = null;
                table_content.ReloadData();

                table_content.Alpha = 0;
                lbl_nodata.Hidden = false;

                Console.WriteLine("FollowListViewController - LoadDataFilterTodo - Err: " + ex.ToString());
            }
        }
        */
        private void LoadMoreDatafollow()
        {
            if (isOnline)
            {
                GetListObj(true);
            }
            else
            {
                var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                try
                {
                    List<BeanAppBaseExt> lst_todo_more = new List<BeanAppBaseExt>();
                    string query = string.Empty;
                    dict_todo = new Dictionary<string, List<BeanAppBaseExt>>();
                    //dict_sectionTodo = new Dictionary<string, bool>();

                    //data
                    query = CreateQueryFollow(false);
                    lst_todo_more = conn.Query<BeanAppBaseExt>(query, CmmVariable.M_DataLimitRow, lst_appBase_follow.Count);
                    //lst_todo_more = lst_appBase_follow.GroupBy(t => t.ID).Select(g => g.First()).ToList();

                    if (lst_todo_more != null && lst_todo_more.Count > 0)
                    {
                        if (lst_todo_more.Count < CmmVariable.M_DataLimitRow)
                            isLoadMore = false;
                        lst_appBase_follow.AddRange(lst_todo_more);
                        SortListAppBase();
                    }
                    else
                    {
                        isLoadMore = false;
                    }
                }
                catch (Exception ex)
                {
                    follow_count = 0;
                    loadData_count(follow_count, true);
                    table_content.Source = null;
                    table_content.ReloadData();

                    table_content.Alpha = 0;
                    lbl_nodata.Hidden = false;

                    Console.WriteLine("FollowListViewController - LoadDataFilterTodo - Err: " + ex.ToString());
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// Sort section from Start Date
        /// </summary>
        /// <param name="query"></param>
        private void SortListAppBase(List<BeanAppBaseExt> _lst_appBase = null)
        {
            var lst_appBase = _lst_appBase != null ? _lst_appBase : lst_appBase_follow;
            if (lst_appBase != null && lst_appBase.Count > 0)
            {
                dict_todo = new Dictionary<string, List<BeanAppBaseExt>>();
                string KEY_TODAY = "TEXT_TODAY`Hôm nay";
                string KEY_YESTERDAY = "TEXT_YESTERDAY`Hôm qua";
                string KEY_ORTHER = "TEXT_OLDER`Cũ hơn";

                List<BeanAppBaseExt> lst_temp1 = new List<BeanAppBaseExt>();
                List<BeanAppBaseExt> lst_temp2 = new List<BeanAppBaseExt>();
                List<BeanAppBaseExt> lst_temp3 = new List<BeanAppBaseExt>();
                dict_todo.Add(KEY_TODAY, lst_temp1);
                dict_todo.Add(KEY_YESTERDAY, lst_temp2);
                dict_todo.Add(KEY_ORTHER, lst_temp3);

                foreach (var item in lst_appBase)
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

                table_content.Alpha = 1;
                lbl_nodata.Hidden = true;

                table_content.Source = new Todo_TableSource(dict_todo, this);
                table_content.ReloadData();
            }
            else
            {
                table_content.Alpha = 0;
                lbl_nodata.Hidden = false;

                table_content.Source = null;
                table_content.ReloadData();
            }
        }
        /*
        private void LoadMoreDataFollow()
        {
            try
            {
                var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                string query = string.Empty;
                List<BeanAppBaseExt> lst_appBase_fromMe_more = new List<BeanAppBaseExt>();
                dict_workflow = new Dictionary<string, List<BeanAppBaseExt>>();
                dict_sectionWorkFlow = new Dictionary<string, bool>();
                //data
                query = CreateQueryFollow(false);
                lst_appBase_fromMe_more = conn.Query<BeanAppBaseExt>(query, CmmVariable.M_DataLimitRow, lst_appBase_follow.Count);

                if (lst_appBase_fromMe_more != null && lst_appBase_fromMe_more.Count > 0)
                {
                    lst_appBase_follow.AddRange(lst_appBase_fromMe_more);
                    SortListAppBase();
                    if (lst_appBase_fromMe_more.Count < CmmVariable.M_DataLimitRow)
                        isLoadMore = false;
                }
                else
                {
                    isLoadMore = false;
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

                Console.WriteLine("FollowListViewController - LoadDataFilterFromMe - Err: " + ex.ToString());
            }
        }
        */
        private string CreateQueryFollow(bool isGetCount)
        {
            string query = string.Format(@"SELECT " + (isGetCount ? "Count(*) as count " : "AB.*, NOTI.StartDate, NOTI.Read, NOTI.SPItemId, NOTI.SubmitAction, NOTI.SendUnit ")//AB.*, NOTI.StartDate, NOTI.Read, NOTI.SPItemId, NOTI.SubmitAction, NOTI.SendUnit "
                + "FROM BeanAppBase AB "
                //+ "INNER JOIN BeanNotify NOTI ON AB.ID = NOTI.SPItemId "
                + "INNER JOIN (SELECT StartDate, Read, SPItemId, SubmitAction, SendUnit, Type, Status FROM BeanNotify GROUP BY SPItemId) NOTI ON AB.ID = NOTI.SPItemId "
                + "WHERE (((AB.CreatedBy LIKE '%{0}%'OR AB.NotifiedUsers LIKE '%{0}%') AND AB.StatusGroup <> 256) "//Check đã xử lý
                + "OR (NOTI.Type = 1 AND NOTI.Status = 0 AND (AB.AssignedTo LIKE '%{0}%' OR AB.NotifiedUsers LIKE '%{0}%')))"//Check đang xử lý
                + "AND (EXISTS (SELECT 1 FROM BeanWorkflowFollow FF WHERE FF.WorkflowItemId = AB.ID AND FF.Status = 1)) "//Check isfollow
                + (isGetCount ? "" : "Order By AB.Created DESC LIMIT ? OFFSET ?"), CmmVariable.SysConfig.UserId.ToUpper());
            return query;
        }

        #region todo toMe data source table
        private class Todo_TableSource : UITableViewSource
        {
            List<BeanNotify> lst_todo;
            NSString cellIdentifier = new NSString("cell");
            FollowListViewController parentView;
            int limit = 20;
            //bool isLoadMore = true;
            public static Dictionary<string, List<BeanNotify>> indexedCateSession;
            Dictionary<string, List<BeanAppBaseExt>> dict_todo { get; set; }
            Dictionary<string, bool> dict_section { get; set; }

            public Todo_TableSource(Dictionary<string, List<BeanAppBaseExt>> _dict_todo, FollowListViewController _parentview)
            {
                dict_todo = _dict_todo;
                parentView = _parentview;
                GetDictSection();
            }

            private void GetDictSection()
            {
                dict_section = new Dictionary<string, bool>();// dict_sectionTodo;
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
                //neu la section dau tien
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
                cell.UpdateCell(todo, isOdd, true);
                return cell;
            }

            public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
            {
                if (parentView.isLoadMore)
                {
                    var lst_appBase_cxl = parentView.lst_appBase_follow;
                    int sumItem = 0;
                    for (int i = 0; i < indexPath.Section; i++)
                    {
                        var keySum = dict_section.ElementAt(i).Key;
                        var todoSum = dict_todo[keySum];
                        sumItem += todoSum.Count;
                    }
                    sumItem += 1;//them mot item
                    sumItem += indexPath.Row;
                    if (sumItem % (CmmVariable.M_DataLimitRow) == 0 && lst_appBase_cxl.Count == sumItem) // boi so cua 20
                    {
                        parentView.view_loadmore.Hidden = false;
                        parentView.indicator_loadmore.StartAnimating();
                        parentView.LoadmoreData();
                    }
                }
            }
        }
        #endregion

        #region WorkFlowItem data source table
        /*private class WorkFlow_TableSource : UITableViewSource
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
            FollowListViewController parentView;

            string query = "";
            int limit = 20;
            bool isLoadMore = true;

            public WorkFlow_TableSource(Dictionary<string, List<BeanAppBaseExt>> _dict_workflow, FollowListViewController _parentview, string _query)
            {
                dict_workflow = _dict_workflow;
                parentView = _parentview;
                query = _query;
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
                    var lst_appBase_follow = parentView.lst_appBase_follow;
                    int sumItem = 0;
                    for (int i = 0; i < indexPath.Section; i++)
                    {
                        var keySum = dict_section.ElementAt(i).Key;
                        var todoSum = dict_workflow[keySum];
                        sumItem += todoSum.Count;
                    }
                    sumItem += 1;//them mot item
                    sumItem += indexPath.Row;
                    if (sumItem % CmmVariable.M_DataLimitRow == 0 && lst_appBase_follow.Count == sumItem) // boi so cua 20 va dong thoi la item cuoi cung
                    {
                        parentView.view_loadmore.Hidden = false;
                        parentView.indicator_loadmore.StartAnimating();
                        parentView.LoadmoreData();
                    }
                }
            }
        }*/
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.iPad.Components;
using BPMOPMobile.iPad.CustomControlClass;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using MobileCoreServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite;
using UIKit;

namespace BPMOPMobile.iPad.ViewControllers
{
    public partial class FollowListViewController : UIViewController
    {
        private NSObject _willResignActiveNotificationObserver;
        private NSObject _didBecomeActiveNotificationObserver;
        #region define
        UIRefreshControl follow_refreshControl;
        //int fromMe_count;
        CmmLoading loading;

        bool isExpandUser;
        bool isTask = false;
        //value form default

        public bool isFilter;
        bool isLoadMore = true;
        nfloat origin_view_header_content_height_constant;
        public nfloat estCommmentViewRowHeight;

        public bool isShowKeyBoarFromComment;
        bool isFollow;
        public bool isShowTask;

        int limit = CmmVariable.M_DataLimitRow;
        int offset = 0;
        List<BeanTask> lst_tasks;
        List<BeanWorkFlowRelated> lstWorkFlowRelateds;
        List<BeanAppBaseExt> lst_appBase_follow;
        List<BeanAppBaseExt> lst_follow_result = new List<BeanAppBaseExt>();

        List<string> lst_userName = new List<string>();
        Dictionary<string, List<BeanAppBaseExt>> dict_follow = new Dictionary<string, List<BeanAppBaseExt>>();
        Dictionary<string, string> dic_valueObject = new Dictionary<string, string>();
        List<BeanQuaTrinhLuanChuyen> lst_qtlc;
        string str_json_FormDefineInfo = string.Empty;
        string[] arr_assignedTo;
        string json_attachRemove;
        public string searchKeyword;
        Dictionary<string, bool> dict_sectionFollow = new Dictionary<string, bool>();
        public BeanAppBaseExt currentItemSelected { get; set; }
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
        Dictionary<string, List<BeanAppBaseExt>> dict_follow_result = new Dictionary<string, List<BeanAppBaseExt>>();
        List<ButtonAction> lst_menuItem = new List<ButtonAction>();
        bool IsFirstLoad = true;
        UIImagePickerController imagePicker;
        UIDocumentPickerViewController docPicker;
        JObject JObjectSource = new JObject(); // JObject những Element ko phải calculated
        public List<JObject> lstGridDetail_Deleted = new List<JObject>(); // lưu lại những item nào đã bị xóa ra khỏi Control InputgridDetail
        string json_PropertyRemove;
        // Comment
        private List<BeanComment> lst_comments;
        public List<BeanAttachFile> _lstAttachComment = new List<BeanAttachFile>();
        private DateTime _CommentChanged;
        public string _OtherResourceId = "";
        //Attachments
        int numRowAttachmentFile = 0;
        ViewElement attachmentElement;
        int follow_count;
        bool isOnline = true;
        UIStringAttributes firstAttributes = new UIStringAttributes
        {
            Font = UIFont.FromName("ArialMT", 13f)
        };
        #endregion

        public FollowListViewController(IntPtr handle) : base(handle)
        {
            localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

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

            ViewConfiguration();
            LoadData();
            if (currentItemSelected != null && currentItemSelected.ResourceCategoryId == (int)CmmFunction.CommentResourceCategoryID.Task)
                isShowTask = true;
            LoadItemSelected(true);

            #region delegate
            follow_refreshControl.ValueChanged += Workflow_refreshControl_ValueChanged;
            BT_comment.TouchUpInside += BT_comment_TouchUpInside;
            BT_moreUser.TouchUpInside += BT_moreUser_TouchUpInside;
            BT_avatar.TouchUpInside += BT_avatar_TouchUpInside;
            BT_start.TouchUpInside += BT_start_TouchUpInside;
            BT_share.TouchUpInside += BT_share_TouchUpInside;
            BT_history.TouchUpInside += BT_history_TouchUpInside;
            tf_search.EditingChanged += Tf_search_EditingChanged;
            BT_attachement.TouchUpInside += BT_attachement_TouchUpInside;
            CmmEvent.UpdateLangComplete += CmmEvent_UpdateLangComplete;
            #endregion
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            _willResignActiveNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, KeyBoardUpNotification);
            _didBecomeActiveNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyBoardDownNotification);
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

            CmmIOSFunction.MakeCornerTopLeftRight(view_task_left, 8);
            CmmIOSFunction.MakeCornerTopLeftRight(view_task_right, 8);
        }

        #region Methods

        private void ViewConfiguration()
        {
            lbl_top_bar_title.Text = CmmFunction.GetTitle("TEXT_FOLLOW", "Theo dõi");

            tf_search.Placeholder = CmmFunction.GetTitle("TEXT_SEARCH", "Tìm kiếm...");

            origin_view_header_content_height_constant = view_header_content_height_constant.Constant;

            follow_refreshControl = new UIRefreshControl();
            follow_refreshControl.TintColor = UIColor.FromRGB(65, 80, 134);
            follow_refreshControl.AttributedTitle = new NSAttributedString(CmmFunction.GetTitle("TEXT_REFRESH_LOADING", "Loading ..."), firstAttributes);
            table_content_left.AddSubview(follow_refreshControl);

            iv_search.Image = iv_search.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            iv_search.TintColor = UIColor.Black;

            BT_filter_search.ContentEdgeInsets = new UIEdgeInsets(7, 14, 7, 0);
            BT_start.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_comment.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_share.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_attachement.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_history.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);

            BT_filter_search.Hidden = true;
        }

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
                         lst_userResult = conn.QueryAsync<BeanUser>(query_user0, currentItemSelected.CreatedBy.Trim().ToLower()).Result;
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

        public void LoadData()
        {
            try
            {
                isOnline = Reachability.detectNetWork();
                dict_follow = new Dictionary<string, List<BeanAppBaseExt>>();
                dict_sectionFollow = new Dictionary<string, bool>();
                lst_appBase_follow = new List<BeanAppBaseExt>();

                if (isOnline)
                    LoadDataOnLine();
                else
                    LoadDataOffLine();
            }
            catch (Exception ex)
            {
                lbl_top_bar_title.Text = CmmFunction.GetTitle("TEXT_FOLLOW", "Theo dõi");
                table_content_left.Source = null;
                table_content_left.ReloadData();
                Console.WriteLine("MyRequestListView - FilterFromMe - Err: " + ex.ToString());
            }
        }

        void LoadDataOffLine()
        {
            var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
            try
            {
                string count_query_fromMe = string.Empty;
                string query = string.Empty;

                //count
                List<CountNum> lst_count_appBase_fromMe = new List<CountNum>();
                count_query_fromMe = CreateQueryFollow(true);
                lst_count_appBase_fromMe = conn.Query<CountNum>(count_query_fromMe);
                if (lst_count_appBase_fromMe != null && lst_count_appBase_fromMe.Count > 0 && lst_count_appBase_fromMe[0].count > 0)
                    follow_count = lst_count_appBase_fromMe[0].count;
                else
                    follow_count = 0;
                loadData_count();

                //data
                query = CreateQueryFollow(false);
                lst_appBase_follow = conn.Query<BeanAppBaseExt>(query, limit, offset);
                lst_appBase_follow = lst_appBase_follow.GroupBy(t => t.ID).Select(g => g.First()).ToList();
                if (lst_appBase_follow != null && lst_appBase_follow.Count > 0)
                {
                    if (currentItemSelected != null)
                        currentItemSelected.IsSelected = true;
                    else
                        currentItemSelected = lst_appBase_follow[0];

                    SortListAppBase();
                }
                else
                {
                    table_content_left.Hidden = true;
                    lbl_nodata_left.Hidden = false;

                    view_task_right.Hidden = true;
                    lbl_nodata_right.Hidden = false;
                }
                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    tf_search.Text = searchKeyword;
                    searchData();
                }
                //CheckQuery(conn);

            }
            catch (Exception ex)
            {
                Console.WriteLine("FollowListView - LoadData: " + ex.ToString());
                loadData_count();
            }
            finally
            {
                conn.Close();
            }
        }

        void LoadDataOnLine()
        {
            GetCountNumber();
            loadData_count();
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
            // _ = Task.Run(() =>
            //{
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

                    if (currentItemSelected != null)
                        currentItemSelected.IsSelected = true;
                    else
                        currentItemSelected = lst_appBase_follow[0];

                    InvokeOnMainThread(() =>
                    {
                        SortListAppBase();
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetListObj - Err: " + ex.ToString());
            }
            if (!isLoadMore)
            {
                //lst_appBase_follow = new List<BeanAppBaseExt>();
            }
            //});
        }

        private void loadData_count()
        {
            try
            {
                string str_follow = string.Empty;
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
                if (indexA >= 0)
                    att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(255, 122, 58), new NSRange(indexA, txtFollowCount.Length - indexA));
                lbl_top_bar_title.AttributedText = att;
            }
            catch (Exception ex)
            {
                lbl_top_bar_title.Text = CmmFunction.GetTitle("TEXT_FOLLOW", "Theo dõi");
                Console.WriteLine("FollowListViewController - loadData - Err: " + ex.ToString());
            }
        }
        /*
        private void SortListAppBase()
        {
            dict_follow = new Dictionary<string, List<BeanAppBaseExt>>();
            dict_sectionFollow = new Dictionary<string, bool>();

            const string KEY_TODAY = "Hôm nay";
            const string KEY_YESTERDAY = "Hôm qua";
            const string KEY_ORTHER = "Cũ hơn";

            List<BeanAppBaseExt> lst_temp1 = new List<BeanAppBaseExt>();
            List<BeanAppBaseExt> lst_temp2 = new List<BeanAppBaseExt>();
            List<BeanAppBaseExt> lst_temp3 = new List<BeanAppBaseExt>();
            dict_follow.Add(KEY_TODAY, lst_temp1);
            dict_follow.Add(KEY_YESTERDAY, lst_temp2);
            dict_follow.Add(KEY_ORTHER, lst_temp3);

            foreach (var item in lst_appBase_follow)
            {
                if (item.ID != 0 && item.ID == currentItemSelected.ID || (item.ID == 0 && item.SPItemId == currentItemSelected.SPItemId))
                    item.IsSelected = true;
                else
                    item.IsSelected = false;

                if (item.Created.Value.Date == DateTime.Now.Date) // today
                {
                    List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                    if (dict_follow.ContainsKey(KEY_TODAY))
                        dict_follow[KEY_TODAY].Add(item);
                    else
                        dict_follow.Add(KEY_TODAY, lst_temp);
                }
                else if (item.Created.Value.Date == DateTime.Now.AddDays(-1).Date) // hom qua
                {
                    List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                    if (dict_follow.ContainsKey(KEY_YESTERDAY))
                        dict_follow[KEY_YESTERDAY].Add(item);
                    else
                        dict_follow.Add(KEY_YESTERDAY, lst_temp);
                }
                else if (item.Created.Value.Date < DateTime.Now.AddDays(-1).Date) // cuhon
                {
                    List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                    if (dict_follow.ContainsKey(KEY_ORTHER))
                        dict_follow[KEY_ORTHER].Add(item);
                    else
                        dict_follow.Add(KEY_ORTHER, lst_temp);
                }
            }

            table_content_left.Hidden = false;
            lbl_nodata_left.Hidden = true;

            table_content_left.Source = new ToDo_TableSource(dict_follow, this);
            table_content_left.ReloadData();
        }
        */
        private string CreateQueryFollow(bool isGetCount)
        {
            string query = string.Format(@"SELECT " + (isGetCount ? "Count(*) as count " : "AB.*, NOTI.StartDate, NOTI.Read, NOTI.SPItemId, NOTI.SubmitAction, NOTI.SendUnit ")//AB.*, NOTI.StartDate, NOTI.Read, NOTI.SPItemId, NOTI.SubmitAction, NOTI.SendUnit "
            + "FROM BeanAppBase AB "
            + "INNER JOIN (SELECT StartDate, Read, SPItemId, SubmitAction, SendUnit, Type, Status FROM BeanNotify GROUP BY SPItemId) NOTI ON AB.ID = NOTI.SPItemId "
            + "WHERE (((AB.CreatedBy LIKE '%{0}%'OR AB.NotifiedUsers LIKE '%{0}%') AND AB.StatusGroup <> 256) "//Check đã xử lý
            + "OR (NOTI.Type = 1 AND NOTI.Status = 0 AND (AB.AssignedTo LIKE '%{0}%' OR AB.NotifiedUsers LIKE '%{0}%')))"//Check đang xử lý
            + "AND (EXISTS (SELECT 1 FROM BeanWorkflowFollow FF WHERE FF.WorkflowItemId = AB.ID AND FF.Status = 1)) "//Check isfollow
            + (isGetCount ? "" : "Order By AB.Created DESC LIMIT ? OFFSET ?"), CmmVariable.SysConfig.UserId.ToUpper());
            return query;
        }

        private void searchData()
        {
            try
            {
                string content = CmmFunction.removeSignVietnamese(tf_search.Text.Trim().ToLowerInvariant());
                if (!string.IsNullOrEmpty(content))
                {
                    var items = from item in lst_appBase_follow
                                where ((!string.IsNullOrEmpty(item.Title) && CmmFunction.removeSignVietnamese(item.Title.ToLowerInvariant()).Contains(content)) ||
                                           (!string.IsNullOrEmpty(item.Content) && CmmFunction.removeSignVietnamese(item.Content.ToLowerInvariant()).Contains(content)) ||
                                           (!string.IsNullOrEmpty(item.ListName) && CmmFunction.removeSignVietnamese(item.ListName.ToLowerInvariant()).Contains(content)))
                                orderby item.Title
                                select item;

                    if (items != null && items.Count() > 0)
                    {
                        lst_follow_result = items.ToList();
                        if (dict_follow_result.ContainsKey("Today"))
                            dict_follow_result["Today"] = lst_follow_result;
                        else
                            dict_follow_result.Add("Today", lst_follow_result);

                        table_content_left.Hidden = false;
                        lbl_nodata_left.Hidden = true;
                        table_content_left.Source = new ToDo_TableSource(dict_follow_result, this);
                        table_content_left.ReloadData();
                    }
                    else
                    {
                        table_content_left.Hidden = true;
                        lbl_nodata_left.Hidden = false;
                        table_content_left.Source = null;
                        table_content_left.ReloadData();
                    }

                }
                else
                {
                    if (dict_follow != null && dict_follow.Count > 0)
                    {
                        table_content_left.Hidden = false;
                        lbl_nodata_left.Hidden = true;
                        table_content_left.Source = new ToDo_TableSource(dict_follow, this);
                        table_content_left.ReloadData();
                    }
                    else
                    {
                        table_content_left.Hidden = true;
                        lbl_nodata_left.Hidden = false;
                        table_content_left.Source = null;
                        table_content_left.ReloadData();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("WorkflowDetails - Tf_search_EditingChanged - Err: " + ex.ToString());
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

        private async void LoadItemSelected(bool isScrollToTop)
        {
            try
            {
                loading = new CmmLoading(new CGRect((view_task_right.Bounds.Width - 200) / 2, (view_task_right.Bounds.Height - 200) / 2, 200, 200), "Đang xử lý...");
                view_task_right.Add(loading);

                //hide menu button default va table content, show lai khi load data hoan tat
                view_buttonDefault.Hidden = true;
                table_content_right.Hidden = true;
                table_content_right.ScrollsToTop = true;

                SQLiteAsyncConnection conn = new SQLiteAsyncConnection(CmmVariable.M_DataPath, false);
                if (currentItemSelected != null)
                {
                    await Task.Run(() =>
                    {
                        lst_addAttachment = new List<BeanAttachFile>();
                        lst_addCommentAttachment = new List<BeanAttachFile>();

                        if (currentItemSelected.ResourceCategoryId == 8)
                            isTask = false;
                        else if (currentItemSelected.ResourceCategoryId == 16)
                            isTask = true;

                        string workflowID = "";
                        if (!string.IsNullOrEmpty(currentItemSelected.ItemUrl))
                            workflowID = CmmFunction.GetWorkflowItemIDByUrl(currentItemSelected.ItemUrl);

                        if (!string.IsNullOrEmpty(workflowID))
                        {
                            string query_workflowItem = string.Format("SELECT * FROM BeanWorkflowItem WHERE ID = {0}", workflowID);
                            var _list_workFlowItem = conn.QueryAsync<BeanWorkflowItem>(query_workflowItem).Result;

                            if (_list_workFlowItem != null && _list_workFlowItem.Count > 0)
                                workflowItem = _list_workFlowItem[0];
                            else
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

                        List<BeanWorkflowFollow> lst_follow = new List<BeanWorkflowFollow>();
                        string query_follow = string.Format("SELECT * FROM BeanWorkflowFollow WHERE WorkflowItemId = ?");
                        var lst_followResult = conn.QueryAsync<BeanWorkflowFollow>(query_follow, currentItemSelected.ID).Result;
                        nfloat maxStatusWidth = 0;
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
                                //if (CmmVariable.SysConfig.LangCode == "1066")
                                //    lbl_list_name.Text = workflowItem.WorkflowTitle;
                                //else if (CmmVariable.SysConfig.LangCode == "1033")
                                //    lbl_list_name.Text = workflowItem.WorkflowTitleEN;
                                lbl_list_name.Text = workflowItem.Title;
                                maxStatusWidth = (lbl_assignedTo.Frame.Width / 3) * 2;
                            });

                        #region AssignedTo - lay danh sach nguoi xu ly hien tai tu workflowItem
                        string assignedTo = workflowItem.AssignedTo;
                        arr_assignedTo = null;
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
                            string query = string.Format("SELECT * FROM BeanAppStatus WHERE ID = '{0}' LIMIT 1 OFFSET 0", workflowItem.StatusGroup);

                            List<BeanAppStatus> _lstAppStatus = conn.QueryAsync<BeanAppStatus>(query).Result;
                            InvokeOnMainThread(() =>
                        {
                            if (_lstAppStatus != null && _lstAppStatus.Count > 0)
                            {
                                if (_lstAppStatus[0].ID == 8) // da phe duyet
                                    res = CmmFunction.GetTitle("TEXT_TITLE_APPROVED", "Phê duyệt: ") + assignedTo;
                                else if (_lstAppStatus[0].ID == 64) // da huy
                                    res = CmmFunction.GetTitle("TEXT_TITLE_CANCEL", "Hủy: ") + assignedTo;
                                else if (_lstAppStatus[0].ID == 16)
                                    res = CmmFunction.GetTitle("TEXT_TITLE_REJECTED", "Từ chối: ") + assignedTo;
                                else
                                {
                                    res = CmmFunction.GetTitle("TEXT_TO", "Đến: ") + assignedTo;
                                    if (res.Contains('+'))
                                    {
                                        var indexA = res.IndexOf('+');
                                        NSMutableAttributedString att = new NSMutableAttributedString(res);
                                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(94, 94, 94), new NSRange(0, res.Length));
                                        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(indexA, res.Length - indexA));
                                        //att.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(lbl_subTitle.Font.PointSize, UIFontWeight.Semibold), new NSRange(indexA, res.Length - indexA));
                                        lbl_assignedTo.AttributedText = att;//(att, UIControlState.Normal);
                                    }
                                    lbl_assignedTo.Text = res.TrimEnd(','); // nguoi xu ly hien tai
                                }
                            }
                            else
                            {
                                res = CmmFunction.GetTitle("TEXT_TO", "Đến: ") + assignedTo;
                                if (res.Contains('+'))
                                {
                                    var indexA = res.IndexOf('+');
                                    NSMutableAttributedString att = new NSMutableAttributedString(res);
                                    att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(94, 94, 94), new NSRange(0, res.Length));
                                    att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(indexA, res.Length - indexA));
                                    lbl_assignedTo.AttributedText = att;//(att, UIControlState.Normal);
                                }
                                lbl_assignedTo.Text = res.TrimEnd(','); // nguoi xu ly hien tai
                            }

                            lbl_assignedTo.Text = res.TrimEnd(',');
                        });
                        }
                        #endregion

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

                            if (String.IsNullOrEmpty(_OtherResourceId))
                                _OtherResourceId = _pControlDynamic.GetDetailOtherResource(_objSubmitDetailComment);
                            else
                                _pControlDynamic.GetDetailOtherResource(_objSubmitDetailComment);

                            lst_comments = CmmFunction.GetListComment(workflowItem, _OtherResourceId, _CommentChanged);

                            currentItemSelected.IsFollow = Convert.ToBoolean(json_dataForm[0]["IsFollow"]);
                            str_json_FormDefineInfo = json_dataForm[0]["FormDefineInfo"].ToString();
                            lst_section = json_dataForm.ToObject<List<ViewSection>>();
                            var result = lst_section.SelectMany(s => s.ViewRows)
                                        .FirstOrDefault(s => s.Elements.Any(d => d.DataType == "inputattachmenthorizon"));

                            InvokeOnMainThread(() =>
                            {
                                //if (lstWorkFlowRelateds != null && lstWorkFlowRelateds.Count > 0)
                                //    table_content_right.Source = new Control_TableSource(lst_section, lstWorkFlowRelateds, this);
                                //else
                                //    table_content_right.Source = new Control_TableSource(lst_section, null, this);

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
                                var canEditWF = workflowItem.StatusGroup.HasValue && workflowItem.StatusGroup.Value == 1;

                                table_content_right.Source = new Control_TableSource(lst_section, lstWorkFlowRelateds, lst_tasks, lst_comments, this, canEditWF);

                                view_task_right.Hidden = false;
                                lbl_nodata_right.Hidden = true;
                                table_content_right.ReloadData();

                                if (isScrollToTop)
                                    //table_content_right.ScrollToRow(NSIndexPath.FromRowSection(0, 0), UITableViewScrollPosition.Top, true);
                                    table_content_right.ContentOffset = new CGPoint(0, -table_content_right.ContentInset.Top);

                                #region get danh sách action
                                if (canEditWF)
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

                                    if (buttonBot.Elements != null && buttonBot.Elements.Count > 0)
                                        buttonBot.Elements = CmmFunction.SortListElementAction(buttonBot.Elements);

                                    if (componentButton != null)
                                        componentButton.RemoveFromSuperview();

                                    if (buttonBot.Elements != null || buttonBot.Elements.Count > 0)
                                    {
                                        canEditWF = workflowItem.StatusGroup.HasValue && workflowItem.StatusGroup.Value == 4;// phiếu đang yêu cầu hiệu chỉnh thì không đc thao tác trên app
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

                                table_content_right.Hidden = false;
                                view_task_right.Hidden = false;
                                lbl_nodata_left.Hidden = true;
                                loading.Hide();

                                if (canEditWF)
                                    CmmIOSFunction.commonAlertMessage(this, "VNA BPM", CmmFunction.GetTitle(CmmVariable.TEXT_ALERT_DRAFT, "Vui lòng sử dụng phiên bản web để chỉnh sửa phiếu này!"));
                            });

                            loadQuaTrinhluanchuyen();
                        }
                        else
                        {
                            InvokeOnMainThread(() =>
                        {
                            loading.Hide();
                            view_task_right.Hidden = true;
                            lbl_nodata_right.Hidden = false;

                            UIAlertController alert = UIAlertController.Create("Thông báo", "Không tìm thấy thông tin phiếu, vui lòng thử lại sau.", UIAlertControllerStyle.Alert);//"BPM"
                            alert.AddAction(UIAlertAction.Create("Đóng", UIAlertActionStyle.Default, alertAction =>
                            {
                            }));
                            this.PresentViewController(alert, true, null);
                        });
                        }
                    });

                    await Task.Run(() =>
                    {
                        //Kiem tra co phai Cong Viec hay khong
                        if (currentItemSelected.ResourceCategoryId == (int)CmmFunction.CommentResourceCategoryID.Task && currentItemSelected.ResourceSubCategoryId == 0 && isShowTask)
                        {
                            BeanTask task = lst_tasks.Where(t => t.ID == currentItemSelected.ID).FirstOrDefault();
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
                    InvokeOnMainThread(() =>
                    {
                        loading.Hide();
                        view_task_right.Hidden = true;
                        lbl_nodata_left.Hidden = false;
                    });
                }
            }
            catch (Exception ex)
            {
                loading.Hide();
                Console.WriteLine("WorkflowDetailView - GetIndexItemFromDictionnary - ERR: " + ex.ToString());
            }

        }
        public async void LoadmoreData()
        {
            view_loadmore.Hidden = false;
            indicator_loadmore.StartAnimating();

            await Task.Run(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(0.5f));
                InvokeOnMainThread(() =>
                {
                    LoadMoreDatafollow();

                    view_loadmore.Hidden = true;
                    indicator_loadmore.StopAnimating();
                });
            });
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
        public void UpdateItemSelect(BeanAppBaseExt _itemSelected, NSIndexPath index)
        {
            try
            {
                IsFirstLoad = true;
                loading.Hide();

                HandleCloseAddFollow();
                HandleAttachFileClose();
                Custom_MenuAction custom_menuAction = Custom_MenuAction.Instance;
                if (custom_menuAction.Superview != null)
                    custom_menuAction.RemoveFromSuperview();
                HandleWorkFollowViewResult();

                currentItemSelected = _itemSelected;
                if (currentItemSelected.ResourceCategoryId == (int)CmmFunction.CommentResourceCategoryID.Task)
                    isShowTask = true;

                foreach (var item in lst_appBase_follow)
                {
                    if (item.SPItemId == currentItemSelected.SPItemId)
                        item.IsSelected = true;
                    else
                        item.IsSelected = false;
                }
                table_content_left.ReloadData();
                LoadItemSelected(true);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("WorkflowDetailView - UpdateItemSelect - err : " + ex.ToString());
#endif
            }
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
                image_view.Image = UIImage.FromFile("Icons/icon_profile.png");
                Console.WriteLine("ContactView - openFile - Err: " + ex.ToString());
            }
        }
        public void CloseAddFollow()
        {
            Custom_AddFollowView custom_AddFollowView = Custom_AddFollowView.Instance;
            if (custom_AddFollowView.Superview != null)
                custom_AddFollowView.RemoveFromSuperview();
        }
        private void HandleCloseAddFollow()
        {
            Custom_AddFollowView custom_AddFollowView = Custom_AddFollowView.Instance;
            if (custom_AddFollowView.Superview != null)
                custom_AddFollowView.RemoveFromSuperview();
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
        public void ReLoadDataFromServer(bool scrollToTopList, bool scrollToTopForm)
        {
            currentItemSelected = null;

            LoadData();

            //LoadDataFilterFromMe(status_selected_index, duedate_selected_index, fromDate_default, toDate_default);

            if (scrollToTopList)
            {
                if (lst_appBase_follow != null && lst_appBase_follow.Count > 0)
                {
                    for (int i = 0; i < table_content_left.NumberOfSections(); i++)
                    {
                        if (table_content_left.NumberOfRowsInSection(i) > 0)
                        {
                            table_content_left.ScrollToRow(NSIndexPath.FromRowSection(0, i), UITableViewScrollPosition.Top, true);
                            break;
                        }
                    }
                }

            }

            LoadItemSelected(scrollToTopForm);
        }
        public void UpdateTableSections(int sectionIndex, BeanComment comment)
        {
            var item = lst_comments.Where(i => i.ID == comment.ID).FirstOrDefault();
            item = comment;

            LoadItemSelected(false);
            //table_content_right.ReloadData();
        }

        public void HandleAddFollow()
        {
            Custom_AddFollowView view_follow = Custom_AddFollowView.Instance;
            ButtonAction bt_follow = new ButtonAction();
            bt_follow.Value = "Follow";
            List<KeyValuePair<string, string>> _lstExtent = new List<KeyValuePair<string, string>>();
            _lstExtent.Add(new KeyValuePair<string, string>("status", currentItemSelected.IsFollow ? "0" : "1"));

            SubmitAction(bt_follow, _lstExtent);
            if (view_follow.Superview != null)
                view_follow.RemoveFromSuperview();
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
                        if (element.Enable == true)
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
                            else if (element.DataType == "inputgriddetails")
                            {
                                //property detail add new or edit
                                ObjectSubmitAction ob = new ObjectSubmitAction();
                                ob.ID = element.ID;
                                ob.Value = element.Value;
                                ob.TypeSP = "GridDetails";
                                ob.DataType = element.DataType;
                                lst_object_edit.Add(ob);

                                //property detail remove
                                if (lstGridDetail_Deleted != null && lstGridDetail_Deleted.Count > 0)
                                {
                                    json_PropertyRemove = JsonConvert.SerializeObject(lstGridDetail_Deleted);

                                    ObjectSubmitAction ob_remove = new ObjectSubmitAction();
                                    ob_remove.ID = element.ID;
                                    ob_remove.Value = json_PropertyRemove;
                                    ob_remove.TypeSP = "RemoveGridDetails";
                                    ob_remove.DataType = element.DataType;
                                    lst_object_edit.Add(ob_remove);
                                }
                            }
                            else
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
                        result = providerControl.SendControlDynamicAction(_buttonAction.Value, currentItemSelected.ID.ToString(), str_json_FormDefineInfo, json_edit_element, ref str_errMess, lst_files, lstExtent);
                    else
                        result = providerControl.SendControlDynamicAction(_buttonAction.Value, currentItemSelected.ID.ToString(), str_json_FormDefineInfo, json_edit_element, ref str_errMess, null);

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
                            if (_buttonAction.Value != "Save")
                            {
                                currentItemSelected = null;
                                LoadData();
                                table_content_left.ScrollsToTop = true;
                            }
                            else
                                table_content_left.ReloadData();

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
        public void HandleSectionTable(nint section, string key, int tableIndex)
        {
            var keyInList = "";
            try
            {
                var keyLst = dict_sectionFollow.Keys.ToList().FindAll(o => string.Compare(CmmFunction.GetTitle(o.Split("`")[0], o.Split("`")[1]), key) == 0).FirstOrDefault();
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

            dict_sectionFollow[keyInList] = !dict_sectionFollow[keyInList];
            table_content_left.ReloadSections(new NSIndexSet((uint)section), UITableViewRowAnimation.None);
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
        public void ReloadDataForm(bool isChangeFocus)
        {
            if (isChangeFocus)
            {
                currentItemSelected = null;
            }
            LoadItemSelected(true);
        }
        public void ReLoadTableListWorkFlow()
        {
            table_content_left.ReloadData();
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
                else if (element.DataType == "singlelookup")
                    itemsChoiceView.setContent(this, false, element);
                else if (element.DataType == "multiplechoice")
                    itemsChoiceView.setContent(this, true, element);
                else if (element.DataType == "multilookup")
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
        public void HandlePropertyDetailsRemove(JObject jObjectRemove)
        {
            lstGridDetail_Deleted.Add(jObjectRemove);
            table_content_right.ReloadData();
        }
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
                    Title = _attachFile.Name + ";#" + DateTime.Now.ToString(@"dd/MM/yyyy hh:mm:ss", new CultureInfo("vi")),
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
            switch (e.Info[UIImagePickerController.MediaType].ToString())
            {
                case "public.image":
                    Console.WriteLine("Image selected");
                    isImage = true;
                    break;
                case "public.video":
                    Console.WriteLine("Video selected");
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
            }

            // dismiss the picker
            imagePicker.DismissModalViewController(true);
            var vc = this.PresentedViewController;
            vc.DismissViewController(true, null);
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
            formEditAttach.SetContent(this, _attach, element);
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
            else
                index = lst_attachFile.FindIndex(item => item.Title == attachFile.Title);

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
        #endregion

        #region handle Assignment Tasks
        public async void Handle_RemoveTask(BeanTask _task, NSIndexPath nSIndexPath)
        {
            try
            {
                //loading = new CmmLoading(new CGRect((this.View.Bounds.Width - 200) / 2, (this.View.Bounds.Height - 200) / 2, 200, 200), "Đang xử lý...");
                //this.View.Add(loading);
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

        public void Handle_TaskResult(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase, int numRow)
        {
            table_content_right.ReloadData();
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
            var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
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

        #region Events
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

        async void follow_refreshControl_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                follow_refreshControl.BeginRefreshing();
                ProviderBase provider = new ProviderBase();
                //ProviderUser p_user = new ProviderUser();

                //string documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
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
                        follow_refreshControl.EndRefreshing();
                    });
                });
            }
            catch (Exception ex)
            {
                follow_refreshControl.EndRefreshing();
                Console.WriteLine("Error - FollowListViewController - follow_refreshControl_valuechange - Er: " + ex.ToString());
            }
        }

        private void BT_avatar_TouchUpInside(object sender, EventArgs e)
        {
            this.NavigationController.PopViewController(true);
        }

        private void Tf_search_EditingChanged(object sender, EventArgs e)
        {
            SearchData(tf_search.Text);
        }

        private void BT_moreUser_TouchUpInside(object sender, EventArgs e)
        {
            CloseAllPopUp();
            if (arr_assignedTo != null && arr_assignedTo.Length > 1)
            {
                string strHeaderUserMore = "";
                List<BeanAppStatus> _lstAppStatus = new List<BeanAppStatus>();
                var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);

                try
                {
                    string query = string.Format("SELECT * FROM BeanAppStatus WHERE ID = '{0}' LIMIT 1 OFFSET 0", currentItemSelected.StatusGroup);
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

        private void BT_comment_TouchUpInside(object sender, EventArgs e)
        {
            CloseAllPopUp();
            try
            {
                table_content_right.ScrollToRow(NSIndexPath.FromRowSection(lst_section[0].ViewRows.Count - 1, 0), UITableViewScrollPosition.Top, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ToDoView - BT_comment_TouchUpInside - Err: " + ex.ToString());
            }

        }

        private void BT_share_TouchUpInside(object sender, EventArgs e)
        {
            CloseAllPopUp();
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
            CloseAllPopUp();
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
            CloseAllPopUp();
            if (lst_attachFile != null && lst_attachFile.Count > 0)
            {
                CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
                CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
                ListAttachmentView attachmentView = (ListAttachmentView)Storyboard.InstantiateViewController("ListAttachmentView");
                attachmentView.SetContent(lst_attachFile, workflowItem.Content, this, attachmentElement);
                PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                attachmentView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                attachmentView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                attachmentView.TransitioningDelegate = transitioningDelegate;
                this.PresentViewControllerAsync(attachmentView, true);
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
            }
            catch (Exception ex)
            {
                Console.WriteLine("FollowView - Err: " + ex.ToString());
            }
        }

        private void KeyBoardDownNotification(NSNotification notification)
        {
            try
            {
                if (isShowKeyBoarFromComment)
                {
                    /*table_content_right.ScrollToRow(NSIndexPath.FromRowSection(lst_section[0].ViewRows.Count - 1, 0), UITableViewScrollPosition.Top, true);
                    if (table_content_right.Frame.Y != 0)
                    {
                        CGRect custFrame = table_content_right.Frame;
                        custFrame.Y = 90;
                        table_content_right.Frame = custFrame;
                    }*/
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

        private async void Workflow_refreshControl_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                follow_refreshControl.BeginRefreshing();
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
                        LoadData();
                        follow_refreshControl.EndRefreshing();
                    });
                });
            }
            catch (Exception ex)
            {
                follow_refreshControl.EndRefreshing();
                Console.WriteLine("Error - WorkFlowDetailsView - refreshControl_valuechange - Er: " + ex.ToString());
            }
        }

        private async void CmmEvent_UpdateLangComplete(object sender, CmmEvent.UpdateEventArgs e)
        {
            try
            {
                InvokeOnMainThread(() =>
                {
                    if (loading != null)
                        loading.Hide();

                    //lbl_topBar_title.Text = CmmFunction.GetTitle("TEXT_IPAD_TOME", "Trang chủ  >  Đến tôi");
                    tf_search.Placeholder = CmmFunction.GetTitle("TEXT_SEARCH", "Tìm kiếm...");
                    //LoadDataFilterFromMe(status_selected_index, duedate_selected_index, fromDateSelected, toDateSelected);

                    lbl_nodata_left.Text = string.Compare(CmmVariable.SysConfig.LangCode, "1033") == 0 ? "No data yet" : "Không có dữ liệu";
                    lbl_nodata_right.Text = lbl_nodata_left.Text;

                    follow_refreshControl.AttributedTitle = new NSAttributedString(CmmFunction.GetTitle("TEXT_REFRESH_LOADING", "Loading ..."), firstAttributes);
                    table_content_left.ReloadData();
                    LoadItemSelected(true);
                });
                await Task.Run(() =>
                {
                    loadQuaTrinhluanchuyen();
                    InvokeOnMainThread(() =>
                    {
                        BT_history_TouchUpInside(null, null);
                    });
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("WorkFlowDetailView - CmmEvent_UpdateLangComplete - ERR: " + ex.ToString());
            }
        }
        #endregion

        #region Load list follow objs

        void SetConstraint()
        {
            contraint_heightViewNavBot.Constant = 0;
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                if (UIApplication.SharedApplication.KeyWindow?.SafeAreaInsets.Bottom > 0)
                {
                    contraint_heightViewNavBot.Constant += 10;
                }
            }
        }

        void SearchData(string searchKey)
        {
            try
            {
                string content = CmmFunction.removeSignVietnamese(searchKey.Trim().ToLowerInvariant());
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
                        isLoadMore = false;
                        SortListAppBase(items.ToList());
                    }
                    else
                    {
                        lbl_nodata_left.Hidden = false;
                        table_content_left.Source = null;
                        table_content_left.ReloadData();
                    }
                }
                else
                {
                    SortListAppBase();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("FollowListViewController - SearchData - Err: " + ex.ToString());
            }
        }

        /// <summary>
        /// Sort section from Start Date
        /// </summary>
        /// <param name="query"></param>
        void SortListAppBase(List<BeanAppBaseExt> _lst_appBase = null)
        {
            var lst_appBase = _lst_appBase != null ? _lst_appBase : lst_appBase_follow;
            dict_follow = new Dictionary<string, List<BeanAppBaseExt>>();
            string KEY_TODAY = "TEXT_TODAY`Hôm nay";
            string KEY_YESTERDAY = "TEXT_YESTERDAY`Hôm qua";
            string KEY_ORTHER = "TEXT_OLDER`Cũ hơn";

            List<BeanAppBaseExt> lst_temp1 = new List<BeanAppBaseExt>();
            List<BeanAppBaseExt> lst_temp2 = new List<BeanAppBaseExt>();
            List<BeanAppBaseExt> lst_temp3 = new List<BeanAppBaseExt>();
            dict_follow.Add(KEY_TODAY, lst_temp1);
            dict_follow.Add(KEY_YESTERDAY, lst_temp2);
            dict_follow.Add(KEY_ORTHER, lst_temp3);

            foreach (var item in lst_appBase)
            {
                if (item.SPItemId == currentItemSelected.SPItemId)
                    item.IsSelected = true;
                else
                    item.IsSelected = false;

                if (item.StartDate.HasValue)
                {
                    if (item.StartDate.Value.Date == DateTime.Now.Date) // today
                    {
                        List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                        if (dict_follow.ContainsKey(KEY_TODAY))
                            dict_follow[KEY_TODAY].Add(item);
                        else
                            dict_follow.Add(KEY_TODAY, lst_temp);
                    }
                    else if (item.StartDate.Value.Date == DateTime.Now.AddDays(-1).Date) // hom qua
                    {
                        List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                        if (dict_follow.ContainsKey(KEY_YESTERDAY))
                            dict_follow[KEY_YESTERDAY].Add(item);
                        else
                            dict_follow.Add(KEY_YESTERDAY, lst_temp);
                    }
                    else if (item.StartDate.Value.Date < DateTime.Now.AddDays(-1).Date) // cuhon
                    {
                        List<BeanAppBaseExt> lst_temp = new List<BeanAppBaseExt>();
                        if (dict_follow.ContainsKey(KEY_ORTHER))
                            dict_follow[KEY_ORTHER].Add(item);
                        else
                            dict_follow.Add(KEY_ORTHER, lst_temp);
                    }
                }
            }

            table_content_left.Alpha = 1;
            lbl_nodata_left.Hidden = true;

            table_content_left.Source = new ToDo_TableSource(dict_follow, this);
            table_content_left.ReloadData();
        }

        void ShowEmptyContentTbl()
        {
            follow_count = 0;
            LoadData_count(follow_count);
            table_content_left.Source = null;
            table_content_left.ReloadData();

            table_content_left.Alpha = 0;
            lbl_nodata_left.Hidden = false;
        }

        void LoadData_count(int _dataCount)
        {
            try
            {
                // danh sách yều cầu cần xử lý default
                var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                string str_follow = string.Empty;

                follow_count = _dataCount;
                var txtFollowCount = "";
                str_follow = CmmVariable.SysConfig.LangCode == "1033" ? "Follow" : "Theo dõi";
                if (follow_count >= 100)
                {
                    txtFollowCount = str_follow + " (99+)";
                }
                else if (follow_count > 0 && follow_count < 100)
                {
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
                att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(255, 122, 58), new NSRange(indexA, txtFollowCount.Length - indexA));
                lbl_top_bar_title.AttributedText = att;
            }
            catch (Exception ex)
            {
                Console.WriteLine("FollowListViewController - loadData - Err: " + ex.ToString());
            }
        }

        //void ReloadMoreData(int _index, List<BeanAppBaseExt> _lst_vcxl, string _query)
        //{
        //    if (_index == 0)
        //    {
        //        lst_appBase_follow = _lst_vcxl;
        //        table_content_left.Source = new ToDo_TableSource(dict_follow, this);
        //        table_content_left.ReloadData();
        //    }
        //}

        void LoadMoreDatafollow()
        {
            try
            {
                if (isOnline)
                {
                    GetListObj(true);
                }
                else
                {
                    List<BeanAppBaseExt> lst_todo_more = new List<BeanAppBaseExt>();
                    var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                    string query = string.Empty;
                    dict_follow = new Dictionary<string, List<BeanAppBaseExt>>();
                    dict_sectionFollow = new Dictionary<string, bool>();

                    //data
                    query = CreateQueryFollow(false);
                    lst_todo_more = conn.Query<BeanAppBaseExt>(query, CmmVariable.M_DataLimitRow, lst_appBase_follow.Count);
                    if (lst_todo_more != null && lst_todo_more.Count > 0)
                    {
                        lst_appBase_follow.AddRange(lst_todo_more);
                        SortListAppBase();
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
                follow_count = 0;
                LoadData_count(follow_count);
                table_content_left.Source = null;
                table_content_left.ReloadData();

                table_content_left.Alpha = 0;
                lbl_nodata_left.Hidden = false;

                Console.WriteLine("FollowListViewController - LoadDataFilterTodo - Err: " + ex.ToString());
            }
        }
        #endregion

        #region Load obj detail
        void CloseAllPopUp()
        {
            this.View?.EndEditing(true);
            CloseAddFollow();
        }
        #endregion

        #region View followed list data source
        class ToDo_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellWorkFlow");
            FollowListViewController parentView;
            Dictionary<string, List<BeanAppBaseExt>> dict_follow { get; set; }
            Dictionary<string, bool> dict_section { get; set; }

            public ToDo_TableSource(Dictionary<string, List<BeanAppBaseExt>> _dict_follow, FollowListViewController _parentview)
            {
                dict_follow = _dict_follow;
                parentView = _parentview;
                GetDictSection();
            }

            private void GetDictSection()
            {
                dict_section = new Dictionary<string, bool>();
                foreach (var item in dict_follow)
                {
                    //if (item.Key.Contains("`"))
                    //{
                    //    var arrKey = item.Key.Split("`");
                    //    var key = CmmFunction.GetTitle(arrKey[0], arrKey[1]);
                    //    dict_section.Add(key, true);
                    //}

                    dict_section.Add(item.Key, true);
                }
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return dict_section.Count;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                var sectionItem = dict_section.ElementAt(Convert.ToInt32(section));
                var numRow = dict_follow[sectionItem.Key].Count;

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
                    if (dict_follow[key].Count > 0)
                        return 44;
                    else
                        return 1;
                }
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                var key = dict_section.ElementAt(indexPath.Section).Key;
                var itemSelected = dict_follow[key][indexPath.Row];

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

                    if (dict_follow[key].Count > 0)
                    {
                        CGRect frame = new CGRect(0, 0, tableView.Frame.Width, 45);

                        Custom_ToDoHeader headerView = new Custom_ToDoHeader(parentView, frame);
                        headerView.LoadData(section, key.Contains('`') ? key.Split('`')[1] : key);
                        return headerView;
                    }
                    else
                        return null;
                }
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var key = dict_section.ElementAt(indexPath.Section).Key;
                var todo = dict_follow[key][indexPath.Row];

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
                    var key = dict_section.ElementAt(indexPath.Section).Key;
                    var todo = dict_follow[key];

                    if (dict_section.Count - 1 == indexPath.Section)
                    {
                        if (indexPath.Row + 1 == todo.Count)
                        {
                            //parentView.view_loadmore.Hidden = false;
                            //parentView.indicator_loadmore.StartAnimating();
                            //parentView.isLoadData = true;
                            parentView.LoadmoreData();
                        }
                    }
                }
            }
        }
        #endregion

        #region View content data source
        private class Control_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cell");
            FollowListViewController parentView;
            List<ViewSection> lst_section;
            Dictionary<string, List<ViewRow>> dict_control = new Dictionary<string, List<ViewRow>>();
            List<BeanWorkFlowRelated> lstWorkFlowRelated;
            List<BeanTask> lst_tasks;
            List<BeanComment> lst_comment;

            //tam an session
            int heightHeader = -1;

            public Control_TableSource(List<ViewSection> _lst_section, List<BeanWorkFlowRelated> _lstWorkFlowRelated, List<BeanTask> _lst_tasks, List<BeanComment> _lst_comment, FollowListViewController _parentview, bool _isSaved)
            {
                if (_isSaved)
                {
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
                }

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
                //try
                //{
                //    var element = dict_control[lst_section[indexPath.Section].ID][indexPath.Row].Elements[0];
                //    var control = dict_control[lst_section[indexPath.Section].ID][indexPath.Row];
                //    switch (element.DataType)
                //    {
                //        case "tabs":
                //            return 80;
                //        case "inputattachmenthorizon":
                //            if (!string.IsNullOrEmpty(element.Value) && element.Value != "[]")
                //            {
                //                int sectionHeightTotal = 0;
                //                List<BeanAttachFile> lst_attach = new List<BeanAttachFile>();
                //                JArray json = JArray.Parse(element.Value);
                //                lst_attach = json.ToObject<List<BeanAttachFile>>();
                //                parentView.lst_attachFile = lst_attach;

                //                if (lst_attach.Count > 0)
                //                {
                //                    List<string> sectionKeys = lst_attach.Select(x => x.AttachTypeName).Distinct().ToList();
                //                    if (sectionKeys != null && sectionKeys.Count > 0)
                //                        sectionHeightTotal = sectionKeys.Count * 44;

                //                    return (lst_attach.Count * 65) + 75 + sectionHeightTotal;//header height: 75 - cell row height: 60 - padding top của table : 10
                //                }
                //                else
                //                    return 81;
                //            }
                //            else
                //                return 81;
                //        case "attachmentverticalformframe":
                //            {
                //                var arrAttachment = element.Value.Split(new string[] { ";#" }, StringSplitOptions.None);
                //                int numItem = arrAttachment.Length / 2;

                //                return (numItem >= 3) ? 265 : (85 + (numItem * 60)); //header view height: 85 | cell height: 60 | max cell: 3 cell
                //            }
                //        case "textinputmultiline":
                //            {
                //                string value = CmmFunction.StripHTML(element.Value);
                //                var height_ets = StringExtensions.StringRect(value, UIFont.SystemFontOfSize(14, UIFontWeight.Regular), tableView.Frame.Width - 20);
                //                if (height_ets.Height < 200)
                //                {
                //                    if (height_ets.Height > 25 && height_ets.Height <= 25)
                //                        return (height_ets.Height) + 25;
                //                    else if (height_ets.Height > 70)
                //                        return (height_ets.Height) + 40;
                //                    else
                //                        return 75;
                //                }
                //                else
                //                    return 140;
                //            }
                //        case "textinputformat":
                //            {
                //                var height_ets = StringExtensions.StringRect(element.Value, UIFont.SystemFontOfSize(14, UIFontWeight.Regular), tableView.Frame.Width - 20);
                //                if (height_ets.Height < 200)
                //                    return 100 + 25;
                //                else
                //                    return height_ets.Height + 25;
                //            }
                //        case "inputgriddetails":
                //            {
                //                nfloat height = 90;
                //                var data_source = element.DataSource.Trim();
                //                var data_value = element.Value.Trim();

                //                if (string.IsNullOrEmpty(data_value) || !data_value.Equals("[]"))
                //                {
                //                    List<JObject> lst_jobject = new List<JObject>();
                //                    if (!string.IsNullOrEmpty(data_value))
                //                    {
                //                        JArray rowItem = JArray.Parse(data_value);
                //                        foreach (JObject ob in rowItem)
                //                        {
                //                            lst_jobject.Add(ob);
                //                        }
                //                    }

                //                    nfloat height_expand = 0;
                //                    var lst_titleHeader = new List<BeanWFDetailsHeader>();
                //                    if (!string.IsNullOrEmpty(data_source) && data_source != "[]")
                //                    {
                //                        JArray json = JArray.Parse(data_source);
                //                        lst_titleHeader = json.ToObject<List<BeanWFDetailsHeader>>();

                //                        foreach (var item in lst_titleHeader)
                //                        {
                //                            //if (item.internalName == "TongTien")
                //                            //    item.isSum = true;

                //                            if (item.isSum)
                //                            {
                //                                item.isSum = true;
                //                                height_expand = 50;
                //                            }
                //                        }
                //                        height = height + (lst_jobject.Count * 50);

                //                    }
                //                    else
                //                        height = height + (lst_jobject.Count * 50);

                //                    return height + height_expand;
                //                }
                //                else
                //                    return 90;
                //            }
                //        case "inputworkrelated":
                //            {
                //                var tableHeight = lstWorkFlowRelated.Count * 80;
                //                return tableHeight + 50;
                //            }
                //        case "inputtasks":
                //            {
                //                //rowNum = 0;
                //                //var lst_parent = lst_tasks.Where(i => i.Parent == 0).ToList();
                //                //foreach (var parent in lst_parent)
                //                //{
                //                //    if (parent.IsExpand)
                //                //    {
                //                //        //rowNum = 1;
                //                //        LoadCountSubTask(parent);
                //                //    }
                //                //    else
                //                //        rowNum = 1;
                //                //}

                //                //return (rowNum * 90) + 90;

                //                var tableHeight = lst_tasks.Count * 90;
                //                return tableHeight + 90;
                //            }
                //        case "inputcomments":
                //            {
                //                nfloat basicHeight = 160;
                //                nfloat height = 0;
                //                //notes => add comment, dinh kem comment 
                //                if (element.Notes != null && element.Notes.Count > 0)
                //                {
                //                    foreach (var note in element.Notes)
                //                    {
                //                        if (note.Key == "image")
                //                            height = height + 120;
                //                        else if (note.Key == "doc")
                //                        {
                //                            JArray json = JArray.Parse(note.Value);
                //                            var lst_addAttachment = json.ToObject<List<BeanAttachFile>>();
                //                            if (lst_addAttachment != null && lst_addAttachment.Count > 0)
                //                            {
                //                                height = height + (lst_addAttachment.Count() * 40);
                //                            }
                //                        }
                //                    }
                //                    height = height + basicHeight;
                //                }
                //                else
                //                    height = basicHeight;

                //                if (!string.IsNullOrEmpty(element.DataSource) || element.DataSource != "[]")
                //                {

                //                }

                //                //danh sach tat ca comment trong phieu
                //                if (lst_comment != null && lst_comment.Count > 0)
                //                {
                //                    foreach (var comment in lst_comment)
                //                    {
                //                        // comment co dinh kem
                //                        if (!string.IsNullOrEmpty(comment.AttachFiles))
                //                        {
                //                            JArray json = JArray.Parse(comment.AttachFiles);
                //                            List<BeanAttachFile> newSortList = new List<BeanAttachFile>();
                //                            var lst_attachFiles = json.ToObject<List<BeanAttachFile>>();

                //                            foreach (var attach in lst_attachFiles)
                //                            {
                //                                string fileExt = string.Empty;
                //                                if (!string.IsNullOrEmpty(attach.Url))
                //                                    fileExt = attach.Url.Split('.').Last();

                //                                bool isThumb = CmmFunction.CheckIsImageAttachmentType(fileExt);
                //                                if (isThumb)
                //                                {
                //                                    //height = height + 190 + 200;
                //                                    height = height + 230;
                //                                }
                //                                else
                //                                {
                //                                    newSortList.Insert(newSortList.Count, attach);
                //                                    height = height + 40;
                //                                }
                //                            }

                //                            nfloat heightText = 0;
                //                            if (!string.IsNullOrEmpty(comment.Content))
                //                            {
                //                                CGRect rect = StringExtensions.StringRect(comment.Content, UIFont.FromName("ArialMT", 14f), (parentView.table_content_right.Frame.Width / 5) * 4.4f);
                //                                if (rect.Height > 0 && rect.Height < 20)
                //                                    rect.Height = 30;
                //                                heightText = rect.Height + 50;
                //                            }
                //                            else
                //                                heightText = 80;

                //                            height = height + heightText;

                //                        }
                //                        // comment khong co dinh kem
                //                        else
                //                        {
                //                            height = height + 100;
                //                        }
                //                    }
                //                }

                //                parentView.estCommmentViewRowHeight = height;
                //                return height;
                //            }
                //        default:
                //            {
                //                return 50;
                //            }
                //    }
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine("ToDoDetailView - Control_TableSource - GetHeightForRow - Err: " + ex.ToString());
                //    return 0;
                //}

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

                                    heightTemp = (lst_attach.Count * 60) + 75 + sectionHeightTotal;//header height: 44 - cell row height: 60 - padding top của table : 10
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
                                //var lst_parent = lst_tasks.Where(i => i.Parent == 0).ToList();
                                //foreach (var parent in lst_parent)
                                //{
                                //    if (parent.IsExpand)
                                //    {
                                //        //rowNum = 1;
                                //        LoadCountSubTask(parent);
                                //    }
                                //    else
                                //        rowNum = 1;
                                //}

                                //heightTemp = (rowNum * 90) + 80;
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
                                        nfloat heightText = 0;
                                        if (!string.IsNullOrEmpty(comment.Content))
                                        {
                                            CGRect rect = StringExtensions.StringRect(comment.Content, UIFont.FromName("ArialMT", 14f), (tableView.Frame.Width / 5) * 4.4f);
                                            if (rect.Height > 0 && rect.Height < 20)
                                                rect.Height = 30;
                                            heightText = rect.Height + 50;
                                        }
                                        else
                                            heightText = 80;
                                        height += heightText;
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
            FollowListViewController parentView { get; set; }
            ViewRow control { get; set; }
            NSIndexPath currentIndexPath { get; set; }
            public ComponentBase components;

            public Control_cell_custom(FollowListViewController _parentView, NSString cellID, ViewRow _control, NSIndexPath _currentIndexPath) : base(UITableViewCellStyle.Default, cellID)
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
    }
}
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
using MobileCoreServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite;
using TEditor;
using TEditor.Abstractions;
using UIKit;
using Xamarin.iOS;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class RequestDetailsV2 : UIViewController, EditTorInterFace
    {
        //List<BeanControlDynamicDetail> lst_beanControlDynamicDetail; tam khoa
        AddAttachmentsView addAttachmentsView { get; set; }
        UIImagePickerController imagePicker;
        UIDocumentPickerViewController docPicker;
        List<BeanQuaTrinhLuanChuyen> lst_qtlc;
        List<BeanControlDynamicDetail> lst_control_hasValue;
        List<BeanAttachFile> lst_addAttachment;
        List<BeanAttachFile> lst_attachFile;
        Dictionary<string, string> dic_valueObject = new Dictionary<string, string>();
        public Dictionary<string, string> dic_singleChoiceSelected = new Dictionary<string, string>();
        public Dictionary<string, string> dic_datetimePickerSelected = new Dictionary<string, string>();
        List<BeanWorkFlowRelated> lstWorkFlowRelateds;
        nfloat origin_view_header_content_height_constant;
        /// <summary>
        /// Danh sách các trường được phép chỉnh sửa
        /// </summary>
        private List<string> lst_ControlEnable = new List<string>();
        string Json_FormDataString = string.Empty;
        List<ViewSection> lst_section;
        /// <summary>
        /// JObject những Element ko phải calculated
        /// </summary>
        JObject JObjectSource = new JObject();
        List<string> lst_userName = new List<string>();

        /// <summary>
        /// Danh sách những value add thêm không dynamic control (ghi chú, ý kiến, người được chuyển xử lý...),
        /// nội dung, ý kiến: key = "idea",
        /// user, người xử lý: key = "userValues"
        /// </summary>
        private List<KeyValuePair<string, string>> lstExtent = new List<KeyValuePair<string, string>>();
        public NSIndexPath AttachmentRowIndexPath;
        //UIView view_background_effect;
        UIWebView webView_pdf_mode;
        BeanAppBaseExt beanAppBaseExt { get; set; }
        //BeanTicketRequest ticketRequest;
        public BeanWorkflowItem workflowItem;
        //BeanControlDynamicDetail beanControlDynamic;
        //List<ButtonAction> lst_action;
        List<ButtonAction> lst_actionmore;
        public static List<BeanTaskDetail> lst_tasks;
        //List<BeanUser> lst_userChoice;
        //UITableView table_qtlc;
        string localDocumentFilepath = string.Empty;
        //string str_url_scanPath = string.Empty;
        string str_json_FormDefineInfo = string.Empty;
        string json_attachRemove;
        List<BeanAttachFile> lst_attachRemove = new List<BeanAttachFile>();
        string json_PropertyRemove;

        bool isExpandUser;
        //bool isViewfileMode = false;
        bool isFromPush;
        //bool isViewQTLC = false;
        //bool controlEditStatus = false;
        bool isReloadKanBanView = false;
        /// <summary>
        /// Kiểm tra phiếu có được thao tác trên App
        /// </summary>
        bool isReadOnly;
        //int viewIndex = 0; // 0: form, 1:fileScan, 2: qua trinh luan chuyen - tam khoa
        /// <summary>
        /// 1: WorkRelated; 0: detail
        /// </summary>
        int menuIndex = 0;
        UITableView Table_WorkFlowDetail;
        private UITapGestureRecognizer gestureRecognizer;
        CmmLoading loading;
        //Attachments
        int numRowAttachmentFile = 0;
        ViewElement attachmentElement;
        AppDelegate appD;
        string contentHtmlString = "";
        ViewElement currentElement { get; set; }
        public bool commentShowKey = false;
        NSIndexPath currentIndexPath { get; set; }
        // Comment
        private List<BeanComment> lst_comments;
        public List<BeanAttachFile> _lstAttachComment = new List<BeanAttachFile>();
        public List<BeanAttachFile> lst_addCommentAttachment;
        public string _OtherResourceId = "";
        private DateTime _CommentChanged;
        public string textComment = "";
        public string hintDefault = CmmFunction.GetTitle("TEXT_HINT_REQUIRE_COMMENT", "Vui lòng nhập bình luận tại đây..."); // Câp nhật lại giá trị cho text_note khi hoàn thành đính kèm file 
        public nfloat estCommmentViewRowHeight;
        private NSObject _willResignActiveNotificationObserver;
        private NSObject _willRemoveActiveNotificationObserver;
        long parentId = 0;
        /// <summary>
        /// Lưu lại những item nào đã bị xóa ra khỏi Control InputgridDetail
        /// </summary>
        public List<JObject> lstGridDetail_Deleted = new List<JObject>();
        bool isFollow, isFollowedDefault;
        //UIRefreshControl refreshControl
        UIViewController parentView = null;

        //TEditor
        TEditorViewController tvc;
        public bool isShowTEditor = false;
        bool EditTorInterFace.IsShowTEditor { get { return isShowTEditor; } set { isShowTEditor = value; } }

        public RequestDetailsV2(IntPtr handle) : base(handle)
        {
            localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            appD = UIApplication.SharedApplication.Delegate as AppDelegate;
        }

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

            _willResignActiveNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, KeyBoardUpNotification);
            _willRemoveActiveNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyBoardDownNotification);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ReloadFailedBeanData();

            gestureRecognizer = new UITapGestureRecognizer(Self, new ObjCRuntime.Selector("hideKeyboard"));
            gestureRecognizer.ShouldReceiveTouch += delegate (UIGestureRecognizer recognizer, UITouch touch)
            {
                var name = touch.View.Class.Name;
                var touchName = touch.View.Superview.Superview.Class.Name;

                if (name == "UITableViewCellContentView" || touchName == "BPMOPMobileV1_iOS_CustomControlClass_Custom_attachFileThumb")
                    return false;
                else
                    return true;
            };
            this.View.AddGestureRecognizer(gestureRecognizer);

            ViewConfiguration();
            SetLangTitle();
            LoadData();

            //loaddataTemplateDemo();

            #region delegate
            BT_back.TouchUpInside += BT_back_TouchUpInside;
            BT_progress.TouchUpInside += BT_progress_TouchUpInside;
            BT_share.TouchUpInside += BT_share_TouchUpInside;
            BT_moreaction_exit.TouchUpInside += BT_moreaction_exit_TouchUpInside;
            BT_attachement.TouchUpInside += BT_attachement_TouchUpInside;
            BT_moreUser.TouchUpInside += BT_moreUser_TouchUpInside;
            BT_code_fulltext.TouchUpInside += BT_code_fulltext_TouchUpInside;
            BT_comment.TouchUpInside += BT_comment_TouchUpInside;
            BT_star.TouchUpInside += BT_star_TouchUpInside;
            //refreshControl.ValueChanged += RefreshControl_ValueChanged;
            //CmmEvent.UpdateLangComplete += CmmEvent_UpdateLangComplete;
            #endregion
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        #endregion

        #region private - public method

        void ReloadFailedBeanData()
        {
            CmmIOSFunction.UpdateBeanData<BeanUser>();
        }

        //Navigator from - MainView - Viec can xu ly
        public void setContent(UIViewController main, BeanAppBaseExt _beanAppBaseExt)
        {
            beanAppBaseExt = _beanAppBaseExt;
            parentView = main;
            if (beanAppBaseExt != null)
            {
                SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                string q_update = "UPDATE BeanNotify SET Read = 1 WHERE ID = ?";
                try
                {
                    conn.Execute(q_update, beanAppBaseExt.ID);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("RequestDetailV2 - setContent - Update Read Noti Err: " + ex.ToString());
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        public void setContentFromPush(BeanAppBaseExt _notify, bool _fromPush)
        {
            beanAppBaseExt = _notify;
            isFromPush = _fromPush;
            if (beanAppBaseExt != null)
            {
                SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                string q_update = "UPDATE BeanNotify SET Read = 1 WHERE ID = ?";
                try
                {
                    conn.Execute(q_update, beanAppBaseExt.ID);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("RequestDetailV2 - setContent - Update Read Noti Err: " + ex.ToString());
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        // Navigate from - MyRequestView || ViewRequestTodo
        public void setContent(UIViewController view, BeanWorkflowItem _worflowItem)
        {
            parentView = view;
            workflowItem = _worflowItem;
            //menuIndex = _menuindex;
        }

        /// <summary>
        /// Navigate From FollowListViewController
        /// </summary>
        /// <param name="_followView"></param>
        public void SetContentFromFollowView(FollowListViewController _followView, BeanAppBaseExt _beanAppBaseExt)
        {
            beanAppBaseExt = _beanAppBaseExt;
            if (beanAppBaseExt != null)
            {
                SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                string q_update = "UPDATE BeanNotify SET Read = 1 WHERE ID = ?";
                try
                {
                    conn.Execute(q_update, beanAppBaseExt.ID);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("RequestDetailV2 - setContent - Update Read Noti Err: " + ex.ToString());
                }
                finally
                {
                    conn.Close();
                }
            }

            parentView = _followView;
        }
        private void ViewConfiguration()
        {
            SetConstraint();
            // an view chua du lieu
            bottom_view.Hidden = true;
            view_content.Hidden = true;
            view_header_content.Hidden = true;

            BT_star.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            //var model = DeviceHardware.Model;
            //if (model.Contains("X") || model.Contains("11"))
            //{
            //    headerView_constantHeight.Constant = 80;
            //}

            origin_view_header_content_height_constant = view_header_content_height_constant.Constant;

            BT_moreaction_exit.SetTitle(CmmFunction.GetTitle("TEXT_CLOSE", "Đóng"), UIControlState.Normal);

            string documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string localpath = Path.Combine(documentFolder, CmmVariable.M_Avatar);

            lbl_code.LineBreakMode = UILineBreakMode.TailTruncation;
            lbl_code.Lines = 2;

            table_content.Frame = new CGRect(0, 60, table_content.Frame.Width, table_content.Frame.Height);
            table_content.ContentInset = new UIEdgeInsets(-35, 0, 0, 0);

            //refreshControl = new UIRefreshControl();
            //refreshControl.TintColor = UIColor.FromRGB(9, 171, 78);

            var firstAttributes = new UIStringAttributes
            {
                // ForegroundColor = UIColor.White,
                Font = UIFont.SystemFontOfSize(12)
            };
            //refreshControl.AttributedTitle = new NSAttributedString("Loading...", firstAttributes);
            //table_content.AddSubview(refreshControl);
            //view_loadmore.BackgroundColor = UIColor.FromWhiteAlpha(1, 0.7f);

            table_actionmore.ContentInset = new UIEdgeInsets(-30, 0, 0, 0);
            table_actionmore.SeparatorInset = UIEdgeInsets.Zero;
            table_actionmore.AllowsSelection = true;

            //this.View.AddSubview(webView_pdf_mode);
            //this.View.AddSubview(table_qtlc);
            //this.View.AddSubview(table_action);
            //this.View.AddSubview(view_background_effect);
            //refreshControl = new UIRefreshControl();
            //refreshControl.TintColor = UIColor.FromRGB(9, 171, 78);
            //var firstAttributesRefresh = new UIStringAttributes
            //{
            //    Font = UIFont.FromName("ArialMT", 12f)
            //};
            //refreshControl.AttributedTitle = new NSAttributedString(CmmFunction.GetTitle("TEXT_REFRESH_LOADING", "Loading ..."), firstAttributesRefresh);
            //table_content.AddSubview(refreshControl);
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

        private async void LoadData()
        {
            if (loading != null)
                loading.Hide();

            if (parentView != null)
            {
                if (parentView is MainView)
                    (parentView as MainView).doReload = true;
                else if (parentView is RequestListView)
                    (parentView as RequestListView).doReload = true;
                else if (parentView is MyRequestListView)
                    (parentView as MyRequestListView).doReload = true;
                //else if (parentView is FollowListViewController)
                //   (parentView as FollowListViewController).doReload = true;
            }

            loading = new CmmLoading(new CGRect((this.View.Bounds.Width - 200) / 2, (this.View.Bounds.Height - 200) / 2, 200, 200), "Đang xử lý...");
            this.View.Add(loading);
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(CmmVariable.M_DataPath, false);
            try
            {
                CultureInfo culture = null;
                if (CmmVariable.SysConfig.LangCode == "1033")
                    culture = new CultureInfo("en-US");
                else //if (CmmVariable.SysConfig.LangCode == "1066")
                    culture = new CultureInfo("vi-VN");

                if (beanAppBaseExt != null && menuIndex == 0)
                {
                    //CmmIOSFunction.UpdateItemDataNewLoading(beanAppBaseExt, conn);
                    string strWorkFlowID = CmmFunction.GetWorkflowItemIDByUrl(beanAppBaseExt.ItemUrl);
                    int workFlowId = 0;
                    int.TryParse(strWorkFlowID, out workFlowId);

                    if (workFlowId > 0)
                    {
                        string query_workflowItem = string.Format("SELECT * FROM BeanWorkflowItem WHERE ID = {0}", strWorkFlowID);
                        var _list_workFlowItem = conn.QueryAsync<BeanWorkflowItem>(query_workflowItem).Result;

                        if (_list_workFlowItem != null && _list_workFlowItem.Count > 0)
                            workflowItem = _list_workFlowItem[0];
                        else
                        {
                            try
                            {
                                workflowItem = Reachability.detectNetWork() ? new ProviderControlDynamic().getWorkFlowItemByRID(strWorkFlowID).FirstOrDefault() : null;
                            }
                            catch (Exception ex)
                            {
                                workflowItem = null;
                                Console.WriteLine("Không lấy item được: " + ex.ToString());
                            }
                        }
                        //SetAssignToInfo(conn, beanAppBaseExt.UserName);
                    }
                    else
                    {
                        workflowItem = null;
                    }
                }

                if (workflowItem != null)
                {
                    if (workflowItem.Created.HasValue)
                    {
                        if (workflowItem.Created == DateTime.Now)
                            lbl_sentTime.Text = workflowItem.Created.Value.ToString("HH:mm");
                        else if (workflowItem.Created < DateTime.Now)
                        {
                            //lbl_sentTime.Text = workflowItem.Created.Value.ToString("dd/MM/yy HH:mm");
                            if (CmmVariable.SysConfig.LangCode == "1033")
                                lbl_sentTime.Text = CmmFunction.GetStringDateTimeLang(workflowItem.Created.Value, 0, 1033);
                            else //if (CmmVariable.SysConfig.LangCode == "1066")
                                lbl_sentTime.Text = CmmFunction.GetStringDateTimeLang(workflowItem.Created.Value, 0, 1066);
                        }
                    }

                    lbl_code.Text = workflowItem.Content;
                    var heightTextContent = StringExtensions.StringHeight(lbl_code.Text, lbl_code.Font, lbl_code.Frame.Width);
                    if (heightTextContent > lbl_code.Frame.Height)
                        CmmIOSFunction.CreateButtonReadAllContent(lbl_code, "", lbl_code.Text, this);
                    //setting image bt_start
                    List<BeanWorkflowFollow> lst_follow = new List<BeanWorkflowFollow>();

                    // Cập nhật Avatar cách cũ: Avatar, Nguoi xu ly buoc truoc, ngay gui- se cap nhat sau khi load QTLC
                    //SetSendUnitAvatar(new BeanUser { FullName = beanAppBaseExt.UserName, ImagePath = beanAppBaseExt.UserImage });

                    #region AssignedTo - lay danh sach nguoi xu ly hien tai
                    //GetAssignedUser(conn);
                    #endregion

                    await Task.Run(() =>
                    {
                        ProviderControlDynamic p_controlDynamic = new ProviderControlDynamic();
                        Json_FormDataString = p_controlDynamic.GetTicketRequestControlDynamicForm(workflowItem, CmmVariable.SysConfig.LangCode);

                        loadQuaTrinhluanchuyen();
                    });

                    if (!string.IsNullOrEmpty(Json_FormDataString))
                    {
                        // Cập nhật lại trường Read cho BeanNotify khi click vào
                        UpdateReadNotify();

                        //Hiện view đã chứa dữ liệu lên
                        bottom_view.Hidden = false;
                        view_content.Hidden = false;
                        view_header_content.Hidden = false;

                        JObject retValue = JObject.Parse(Json_FormDataString);
                        //danh sach chi tiet form controls
                        JArray json_dataForm = JArray.Parse(retValue["form"].ToString());

                        //img BT_star
                        isFollowedDefault = bool.Parse(json_dataForm[0]["IsFollow"].ToString());
                        if (isFollowedDefault == true)
                        {
                            BT_star.SetImage(UIImage.FromFile("Icons/icon_Star_on.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
                            isFollow = true;
                        }
                        else
                        {
                            BT_star.SetImage(UIImage.FromFile("Icons/icon_Star_off.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
                            isFollow = false;
                        }

                        //danh sach quy trinh lien quan
                        JArray json_workflowRelated = JArray.Parse(retValue["related"].ToString());
                        lstWorkFlowRelateds = json_workflowRelated.ToObject<List<BeanWorkFlowRelated>>();

                        str_json_FormDefineInfo = json_dataForm[0]["FormDefineInfo"].ToString();
                        lst_section = json_dataForm.ToObject<List<ViewSection>>();
                        //formular
                        try
                        {
                            //Console.WriteLine("Json: " + json_dataForm);
                            if (JObjectSource != null)
                                JObjectSource = new JObject();
                            foreach (ViewSection section in lst_section ?? new List<ViewSection>())
                                foreach (ViewRow row in section.ViewRows ?? new List<ViewRow>())
                                    foreach (ViewElement element in row.Elements ?? new List<ViewElement>())
                                    {
                                        JObjectSource.Add(element.Title, element.Value);
                                    }
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            Console.WriteLine("LoadData - Err: " + ex.Message + ex.StackTrace);
#endif
                        }

                        //danh sach cong viec phan cong
                        JArray json_tasks = JArray.Parse(retValue["task"].ToString());
                        lst_tasks = json_tasks.ToObject<List<BeanTaskDetail>>();
                        lst_tasks = Custom_TableTaskHelper.Instance().getSortedNodes(parentId, lst_tasks, true);
                        //danh index cho task
                        Setindexlistdata(parentId, 0);
                        //setting data to display in task
                        SettingDataLstTasks();

                        //danh sach comments
                        lst_comments = new List<BeanComment>();
                        if (!string.IsNullOrEmpty(retValue["moreInfo"]["CommentChanged"].ToString()))//HasValues
                            _CommentChanged = DateTime.Parse(retValue["moreInfo"]["CommentChanged"].ToString());
                        else
                            _CommentChanged = new DateTime();

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

                        if (string.IsNullOrEmpty(_OtherResourceId))
                            _OtherResourceId = _pControlDynamic.GetDetailOtherResource(_objSubmitDetailComment);
                        else
                            _pControlDynamic.GetDetailOtherResource(_objSubmitDetailComment);

                        lst_comments = CmmFunction.GetListComment(workflowItem, _OtherResourceId, _CommentChanged);

                        var result = lst_section.SelectMany(s => s.ViewRows)
                                    .FirstOrDefault(s => s.Elements.Any(d => d.DataType == "inputattachmenthorizon"));

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

                        isReadOnly = workflowItem.StatusGroup.HasValue && workflowItem.StatusGroup.Value == 1;// phiếu đang lưu

                        #region get danh sách action
                        if (isReadOnly)// phiếu đang lưu thì ko cho thao tác
                        {
                            contraint_heightViewNavBot.Constant = 0;
                            BT_comment.Enabled = false;
                            BT_attachement.Enabled = false;
                        }
                        else
                        {
                            BT_comment.Enabled = true;
                            BT_attachement.Enabled = true;
                            //var dataButtonBot0 = @"{'RowType':3,'Elements':[{'DataType':'buttonbot','DataSource': null,'ListProprety':null,'ID':8,'Title':'Từ chối','Value': 'IMG_temp/icon_reject.png','Enable':true},{'DataType':'buttonbot','DataSource': null,'ListProprety':null,'ID':1,'Title':'Đồng ý','Value': 'IMG_temp/icon_accept.png','Enable':true},{'DataType':'buttonbot','DataSource': null,'ListProprety':null,'ID':3,'Title': null,'Value': 'Icons/icon_more.png','Enable':true}],'ID':3,'Title':null,'Value':null,'Enable':false}";
                            JObject jsonButtonBot = JObject.Parse(retValue["action"].ToString());
                            string str = JsonConvert.SerializeObject(jsonButtonBot);
                            var buttonBot = jsonButtonBot.ToObject<ViewRow>();

                            if (buttonBot.Elements == null || buttonBot.Elements.Count == 0)
                            {
                                contraint_heightViewNavBot.Constant = 0;
                                isReadOnly = workflowItem.StatusGroup.HasValue && workflowItem.StatusGroup.Value == 4;// phiếu đang yêu cầu hiệu chỉnh thì không đc thao tác trên app
                            }
                            else
                            {
                                buttonBot.Elements = CmmFunction.SortListElementAction(buttonBot.Elements);

                                ComponentButtonBot componentButton = new ComponentButtonBot(this, buttonBot);
                                componentButton.InitializeFrameView(bottom_view.Bounds);
                                componentButton.SetTitle();
                                componentButton.SetValue();
                                componentButton.SetEnable();
                                componentButton.SetProprety();
                                bottom_view.Add(componentButton);
                                lst_actionmore = componentButton.lst_moreActions;

                                if (lst_actionmore != null)
                                {
                                    table_actionmore.Source = new MenuAction_TableSource(lst_actionmore, this);
                                    table_actionmore.ReloadData();
                                }
                            }
                        }
                        #endregion

                        InvokeOnMainThread(() =>
                        {
                            if (lstWorkFlowRelateds != null && lstWorkFlowRelateds.Count > 0)
                                table_content.Source = new Control_TableSource(lst_section, lstWorkFlowRelateds, lst_tasks, lst_comments, this, isReadOnly);
                            else
                                table_content.Source = new Control_TableSource(lst_section, null, lst_tasks, lst_comments, this, isReadOnly);

                            table_content.ReloadData();
                            loading.Hide();
                            //if (currentItemSelected.Action == "Task")
                            //{
                            //    if (isShowTask)
                            //    {
                            //        BeanTask task = lst_tasks.Where(t => t.ID == currentItemSelected.TaskID).FirstOrDefault();
                            //        Handle_TaskSelected(task, null);
                            //    }
                            //}

                        });
                    }
                    else
                    {
                        BT_star.SetImage(UIImage.FromFile("Icons/icon_Star_off.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
                        BT_star.TintColor = UIColor.FromRGB(94, 94, 94);
                        isFollow = false;
                        loading.Hide();

                        UIAlertController alert = UIAlertController.Create("Thông báo", CmmFunction.GetTitle("MESS_WORKFLOW_NOTFOUND", "Không tìm thấy thông tin phiếu, vui lòng thử lại sau."), UIAlertControllerStyle.Alert);//"BPM"
                        alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_CLOSE", "Đóng"), UIAlertActionStyle.Default, alertAction =>
                        {
                            if (!isFromPush)
                            {
                                if (this.NavigationController != null)
                                    this.NavigationController.PopViewController(true);
                                else
                                    this.DismissViewControllerAsync(true);
                            }
                            else
                                this.NavigationController.PopToRootViewController(true);
                        }));
                        this.PresentViewController(alert, true, null);
                    }
                }
                else
                {
                    loading.Hide();
                    CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"));
                }
            }
            catch (Exception ex)
            {
                loading.Hide();
                Console.WriteLine("RequestDetailsV2 - LoadData : " + ex.ToString());
            }
            finally
            {
                conn.CloseAsync();
            }

            if (isReadOnly)
                CmmIOSFunction.commonAlertMessage(this, "VNA BPM", CmmFunction.GetTitle(CmmVariable.TEXT_ALERT_DRAFT, "Vui lòng sử dụng phiên bản web để chỉnh sửa phiếu này!"));
        }
        /// <summary>
        /// tạm không dùng kiểu query user nữa do dùng appbase extension online
        /// </summary>
        /// <param name="conn"></param>
        void GetAssignedUser(SQLiteAsyncConnection conn)
        {
            string assignedTo = workflowItem.AssignedTo;
            string[] arr_assignedTo;
            if (!string.IsNullOrEmpty(assignedTo))
            {
                List<BeanUser> lst_userResult = new List<BeanUser>();
                string query_user = string.Format("SELECT * FROM BeanUser WHERE ID = ?");

                if (assignedTo.Contains(','))
                {
                    arr_assignedTo = assignedTo.Split(',');

                    for (int i = 0; i < arr_assignedTo.Length; i++)
                    {
                        lst_userResult = conn.QueryAsync<BeanUser>(query_user, arr_assignedTo[i].Trim().ToLower()).Result;

                        if (lst_userResult != null && lst_userResult.Count > 0)
                        {
                            lst_userName.Add(lst_userResult[0].FullName);
                        }
                    }

                    //string first_user = "";
                    //if (assignedTo.Contains(','))
                    //    first_user = assignedTo.Split(',')[0];
                    //else
                    //    first_user = assignedTo;
                    if (lst_userName != null && lst_userName.Count > 0)
                    {
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
                        assignedTo = "";
                    }
                }
                else
                {
                    lst_userResult = conn.QueryAsync<BeanUser>(query_user, assignedTo.Trim().ToLower()).Result;
                    if (lst_userResult != null && lst_userResult.Count > 0)
                    {
                        assignedTo = lst_userResult[0].FullName;
                        lst_userName.Add(lst_userResult[0].FullName);
                    }
                }
            }
            SetAssignToInfo(conn, assignedTo);
        }

        /// <summary>
        /// Rule mới ngày 04.08.22 dùng thông tin trả về trong beanAppbaseExt
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="assignedTo"></param>
        async void SetAssignToInfo(SQLiteAsyncConnection conn, string assignedTo)
        {
            //string res = string.Empty;

            ////string query = string.Format("SELECT * FROM BeanAppStatus WHERE ID = '{0}' LIMIT 1 OFFSET 0", beanAppBaseExt.StatusGroup);
            //string query = string.Format("SELECT * FROM BeanAppStatus WHERE ID = '{0}' LIMIT 1 OFFSET 0", workflowItem.StatusGroup);

            //List<BeanAppStatus> _lstAppStatus = await conn.QueryAsync<BeanAppStatus>(query);

            //if (_lstAppStatus != null && _lstAppStatus.Count > 0)
            //{
            //    if (_lstAppStatus[0].ID == 8) // da phe duyet
            //        res = CmmFunction.GetTitle("TEXT_TITLE_APPROVED", "Phê duyệt: ") + assignedTo;
            //    else if (_lstAppStatus[0].ID == 64) // da huy
            //        res = CmmFunction.GetTitle("TEXT_TITLE_CANCEL", "Hủy: ") + assignedTo;
            //    else if (_lstAppStatus[0].ID == 16)
            //        res = CmmFunction.GetTitle("TEXT_TITLE_REJECTED", "Từ chối: ") + assignedTo;
            //    else
            //    {
            //        res = CmmFunction.GetTitle("TEXT_TO", "Đến: ") + assignedTo;
            //        if (!string.IsNullOrEmpty(res) && res.Contains('+'))
            //        {
            //            var indexA = res.IndexOf('+');
            //            NSMutableAttributedString att = new NSMutableAttributedString(res);
            //            att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(94, 94, 94), new NSRange(0, res.Length));
            //            att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(indexA, res.Length - indexA));
            //            //att.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(lbl_subTitle.Font.PointSize, UIFontWeight.Semibold), new NSRange(indexA, res.Length - indexA));
            //            lbl_subTitle.AttributedText = att;//(att, UIControlState.Normal);
            //        }
            //        lbl_subTitle.Text = res.TrimEnd(','); // nguoi xu ly hien tai
            //    }
            //}
            //else
            //{
            //    res = CmmFunction.GetTitle("TEXT_TO", "Đến: ") + assignedTo;
            //    if (!string.IsNullOrEmpty(res) && res.Contains('+'))
            //    {
            //        var indexA = res.IndexOf('+');
            //        NSMutableAttributedString att = new NSMutableAttributedString(res);
            //        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(94, 94, 94), new NSRange(0, res.Length));
            //        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(indexA, res.Length - indexA));
            //        //att.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(lbl_subTitle.Font.PointSize, UIFontWeight.Semibold), new NSRange(indexA, res.Length - indexA));
            //        lbl_subTitle.AttributedText = att;//(att, UIControlState.Normal);
            //    }
            //    lbl_subTitle.Text = res.TrimEnd(','); // nguoi xu ly hien tai
            //}
            ////if (workflowItem.ActionStatusID == 10) // da phe duyet
            ////        res = CmmFunction.GetTitle("TEXT_TITLE_APPROVED", "Phê duyệt: ") + assignedTo;
            ////else if (workflowItem.ActionStatusID == -1) // da huy
            ////        res = CmmFunction.GetTitle("TEXT_TITLE_CANCEL", "Hủy: ") + assignedTo;
            ////else if(workflowItem.ActionStatusID == 6)
            ////    res = CmmFunction.GetTitle("TEXT_TITLE_REJECTED", "Từ chối: ") + assignedTo;
            ////else
            ////{
            ////    res = CmmFunction.GetTitle("TEXT_TO", "Đến: ") + assignedTo;
            ////    if (res.Contains('+'))
            ////    {
            ////        var indexA = res.IndexOf('+');
            ////        NSMutableAttributedString att = new NSMutableAttributedString(res);
            ////        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(94, 94, 94), new NSRange(0, res.Length));
            ////        att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(indexA, res.Length - indexA));
            ////        //att.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(lbl_subTitle.Font.PointSize, UIFontWeight.Semibold), new NSRange(indexA, res.Length - indexA));
            ////        lbl_subTitle.AttributedText = att;//(att, UIControlState.Normal);
            ////    }
            ////    lbl_subTitle.Text = res.TrimEnd(','); // nguoi xu ly hien tai
            ////}

            //lbl_subTitle.Text = res.TrimEnd(',');
        }

        //thuyngo add
        public void ReLoadDataFromServer()
        {
            LoadData();

            isReloadKanBanView = true;
        }

        private void SettingDataLstTasks()
        {
            var parent = lst_tasks.FindAll(s => s.Parent == parentId);
            int session = 0;
            foreach (var item in parent)
            {
                item.session = session++;
            }
            foreach (var item in lst_tasks)
            {
                //setNodeIcon(item);
                long num = -1;
                findsection(item.ID, ref num);
                var itemParent = lst_tasks.Find(s => s.ID == num);
                if (item.Parent != 0)
                    item.session = itemParent.session;
            }
            // kiem tra phai la node cuoi cung khong
            // danh dau thang cuoi cung trong mot group la not la
            if (parent.Count > 0)
                parent[parent.Count - 1].isRoorFinal = true;
            foreach (var item in parent)
            {
                LoadCountSubTask(item);
            }
        }

        // danh lai cay thuyngo add
        private void Setindexlistdata(long id, int index)
        {
            if (index == 3)
                return;
            var item = lst_tasks.FindAll(s => s.Parent == id);
            foreach (var item1 in item)
            {
                for (int i = 0; i < lst_tasks.Count; i++)
                {
                    if (lst_tasks[i].ID == item1.ID)
                    {

                        lst_tasks[i].index = index;
                        Setindexlistdata(lst_tasks[i].ID, index + 1);
                    }
                }
            }
        }

        private void LoadCountSubTask(BeanTaskDetail parent_task)
        {
            var groupChild = lst_tasks.FindAll(s => s.Parent == parent_task.ID);
            if (groupChild.Count > 0)
                groupChild[groupChild.Count - 1].isRoorFinal = true;
            foreach (var item in groupChild)
            {
                LoadCountSubTask(item);
            }
        }

        // danh lai session de to mau
        private void findsection(long id, ref long output)
        {
            var item = lst_tasks.Find(s => s.ID == id);
            if (item.Parent == parentId)
            {
                output = id;
                return;
            }
            findsection(item.Parent, ref output);
        }

        // tinh lai formula
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
                            if (!string.IsNullOrEmpty(element.Formula))
                                element.Value = CmmFunction.CalculateObject(element.Formula, JObjectSource).ToString();
                        }
            }
            catch (Exception ex)
            {
                Console.WriteLine("RequestDetailsV2 - UpdateValue_ForElement - Err: " + ex.ToString());
            }
        }
        private void UpdateReadNotify()
        {
            SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
            string q_update = string.Format(@"UPDATE BeanNotify Set Read = {0} WHERE SPItemId = {1}", true, workflowItem.ID);
            try
            {
                conn.Execute(q_update);
            }
            catch (Exception ex)
            {
                Console.WriteLine("RequestDetailV2 - setContent - Update Read Noti Err: " + ex.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        private async void loadQuaTrinhluanchuyen()
        {
            _ = Task.Run(() =>
              {
                  SQLiteAsyncConnection conn = new SQLiteAsyncConnection(CmmVariable.M_DataPath);
                  lst_qtlc = new List<BeanQuaTrinhLuanChuyen>();
                  ProviderControlDynamic p_dynamic = new ProviderControlDynamic();
                  lst_qtlc = p_dynamic.GetListProcessHistory(workflowItem);

                  if (lst_qtlc != null && lst_qtlc.Count > 0)
                  {
                      List<BeanUser> lst_userResult = new List<BeanUser>();
                      string query_user0 = string.Format("SELECT * FROM BeanUser WHERE ID = ?");

                      if (workflowItem.ActionStatusID == 10 || workflowItem.ActionStatusID == -1 || workflowItem.ActionStatusID == 4)
                      {
                          lst_userResult = conn.QueryAsync<BeanUser>(query_user0, workflowItem.CreatedBy.Trim().ToLower()).Result;
                      }
                      else
                      {
                          string userAssignedID = lst_qtlc.OrderBy(t => t.Created).ToList()[0].AssignUserId;
                          lst_userResult = conn.QueryAsync<BeanUser>(query_user0, userAssignedID.Trim().ToLower()).Result;
                      }

                      InvokeOnMainThread(() =>
                      {
                          //    //ActionStatusID = 10 || -1: phieu da phe duyet / Huy => Nguoi xu ly buoc truoc se la nguoi tao
                          //    //if (workflowItem.ActionStatusID == 10 || workflowItem.ActionStatusID == -1 || workflowItem.ActionStatusID == 4)
                          //    //{
                          //    //    lst_userResult = conn.QueryAsync<BeanUser>(query_user0, workflowItem.CreatedBy.Trim().ToLower()).Result;
                          //    //    if (workflowItem.Created.HasValue)
                          //    //    {
                          //    //        if (workflowItem.Created == DateTime.Now)
                          //    //            lbl_sentTime.Text = workflowItem.Created.Value.ToString("HH:mm");
                          //    //        else if (workflowItem.Created < DateTime.Now)
                          //    //            lbl_sentTime.Text = workflowItem.Created.Value.ToString("dd/MM/yy HH:mm");
                          //    //    }
                          //    //}
                          //    //else
                          //    //{
                          //    //    string userAssignedID = lst_qtlc.OrderByDescending(t => t.Created).ToList()[0].AssignUserId;
                          //    //    lst_userResult = conn.QueryAsync<BeanUser>(query_user0, userAssignedID.ToLower()).Result;
                          //    //    if (workflowItem.Created.HasValue)
                          //    //    {
                          //    //        var sent_time = lst_qtlc.OrderByDescending(t => t.Created).ToList()[0].Created.Value;
                          //    //        if (sent_time == DateTime.Now)
                          //    //            lbl_sentTime.Text = sent_time.ToString("HH:mm");
                          //    //        else if (workflowItem.Created < DateTime.Now)
                          //    //            lbl_sentTime.Text = sent_time.ToString("dd/MM/yy HH:mm");
                          //    //    }
                          //    //}

                          if (lst_userResult != null && lst_userResult.Count > 0)
                              SetSendUnitAvatar(lst_userResult[0]);

                          isExpandUser = true;
                          ShowAssignUsers();
                      });
                  }
              });
        }

        void SetSendUnitAvatar(BeanUser user)
        {
            string user_imagePath = user.ImagePath;

            if (string.IsNullOrEmpty(user_imagePath))
            {
                if (!string.IsNullOrEmpty(user.FullName))
                {
                    lbl_imgCover.Hidden = false;
                    img_avatar_sentUnit.Hidden = true;
                    lbl_imgCover.Text = CmmFunction.GetAvatarName(user.FullName);
                    lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));
                }
                else
                {
                    //lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar("A"));

                    ///Rule mới hiện avatar default khi ko có FullName
                    img_avatar_sentUnit.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                    lbl_imgCover.Hidden = true;
                    img_avatar_sentUnit.Hidden = false;
                }
            }
            else
            {
                lbl_imgCover.Hidden = false;
                img_avatar_sentUnit.Hidden = true;
                lbl_imgCover.Text = CmmFunction.GetAvatarName(user.FullName);
                lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));

                checkFileLocalIsExist(user_imagePath, img_avatar_sentUnit);
                lbl_imgCover.Hidden = true;
                img_avatar_sentUnit.Hidden = false;
            }
            lbl_title.Text = user.FullName;
        }

        private void ToggelPickerChoice()
        {
            //if (view_picker_choice.Hidden)
            //{
            //    view_background_effect.Frame = new CGRect(this.View.Frame.X, this.View.Frame.Y, this.View.Frame.Width, this.View.Frame.Height - 220);
            //    view_picker_choice.Frame = new CGRect(view_pickerDate.Frame.X, this.View.Bounds.Height, view_pickerDate.Frame.Width, 220);
            //    view_picker_choice.Hidden = false;
            //    view_background_effect.Alpha = 0;
            //    UIView.BeginAnimations("toogle_view_pickerDate_slideShow");
            //    UIView.SetAnimationDuration(0.4f);
            //    UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
            //    UIView.SetAnimationRepeatCount(0);
            //    UIView.SetAnimationRepeatAutoreverses(false);
            //    UIView.SetAnimationDelegate(this);
            //    view_picker_choice.Frame = new CGRect(view_pickerDate.Frame.X, this.View.Bounds.Height - view_picker_choice.Frame.Height, view_pickerDate.Frame.Width, 220);
            //    view_picker_choice.Alpha = 1;
            //    view_background_effect.Alpha = 0.5f;
            //    UIView.CommitAnimations();
            //}
            //else
            //{
            //    hiddenPicker();
            //}
        }

        private async void checkFileLocalIsExist(BeanUser contact, UIImageView image_view)
        {
            try
            {
                string filename = contact.ImagePath.Split('/').Last();
                string filepathURL = CmmVariable.M_Domain + "/" + CmmVariable.SysConfig.Subsite + contact.ImagePath;
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
                                        avatar = CmmIOSFunction.MaxResizeImage(image, 200, 200);
                                        image_view.Image = avatar;
                                    }
                                    else
                                        image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                                }
                                else
                                    image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");

                            });

                            if (data != null && avatar != null)
                            {
                                NSError err = null;
                                NSData imgData = avatar.AsPNG();
                                if (imgData.Save(localfilePath, false, out err))
                                    Console.WriteLine("saved as " + localfilePath);
                                return;
                            }
                        }
                        else
                        {
                            InvokeOnMainThread(() =>
                            {
                                image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                            });
                        }
                    });
                }
                else
                {
                    openFile(filename, image_view);
                    img_avatar_sentUnit.Hidden = false;
                    lbl_imgCover.Hidden = true;
                }
            }
            catch (Exception ex)
            {
                image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                Console.WriteLine("ListUserView - checkFileLocalIsExist - Err: " + ex.ToString());
                //CmmIOSFunction.IOSlog(null, "PopupContactDetailView - loadAvatar - " + ex.ToString());
            }
        }

        private async void checkFileLocalIsExist(string imgUrl, UIImageView image_view)
        {
            try
            {
                if (!string.IsNullOrEmpty(imgUrl))
                {
                    string filename = imgUrl.Split('/').Last();
                    string filepathURL = CmmVariable.M_Domain + "/" + CmmVariable.SysConfig.Subsite + imgUrl;
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
                                            avatar = CmmIOSFunction.MaxResizeImage(image, 200, 200);
                                            image_view.Image = avatar;
                                        }
                                        else
                                            image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                                    }
                                    else
                                        image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");

                                });

                                if (data != null && avatar != null)
                                {
                                    NSError err = null;
                                    NSData imgData = avatar.AsPNG();
                                    if (imgData.Save(localfilePath, false, out err))
                                        Console.WriteLine("saved as " + localfilePath);
                                    return;
                                }
                            }
                            else
                            {
                                InvokeOnMainThread(() =>
                                {
                                    image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                                });
                            }
                        });
                    }
                    else
                    {
                        openFile(filename, image_view);
                        img_avatar_sentUnit.Hidden = false;
                        lbl_imgCover.Hidden = true;
                    }
                }
                else
                {
                    Console.WriteLine("RequestDetailV2 - checkFileLocalIsExist - image Url rỗng.");
                }
            }
            catch (Exception ex)
            {
                image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                Console.WriteLine("RequestDetailV2 - checkFileLocalIsExist - Err: " + ex.ToString());
                //CmmIOSFunction.IOSlog(null, "PopupContactDetailView - loadAvatar - " + ex.ToString());
            }
        }

        private void openFile(string localfilename)
        {
            string localfilePath = Path.Combine(localDocumentFilepath, localfilename);
            webView_pdf_mode.LoadRequest(new NSUrlRequest(new NSUrl(localfilePath, false)));
            webView_pdf_mode.ScalesPageToFit = true;
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
                        image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                    }
                }
                else
                {
                    image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("ContactView - openFile - Err: " + ex.ToString());
            }
        }

        private void menu_action_Toggle()
        {
            try
            {
                int cell_height = 60;
                int maxheight = lst_actionmore.Count * cell_height;
                //maxheight = maxheight; //60 chieu cao cong them cua bottom
                moreaction_view.Frame = this.View.Bounds;
                //more_actionview_contant.Constant = this.View.Bounds.Height;
                if (moreaction_view.Alpha == 0)
                {
                    table_actionmore.Frame = new CGRect(table_actionmore.Frame.X, moreaction_view.Frame.Height - (maxheight + BT_moreaction_exit.Frame.Height + 60), moreaction_view.Frame.Width - (table_actionmore.Frame.X * 2), maxheight + 10);
                    table_actionmore.Alpha = 0;
                    //table_actionmore.Frame = new CGRect(table_actionmore.Frame.X, moreaction_view.Frame.Bottom, table_actionmore.Frame.Width, 0);
                    UIView.BeginAnimations("toogle_docmenu_slideShow");
                    UIView.SetAnimationDuration(0.3f);
                    UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
                    UIView.SetAnimationRepeatCount(0);
                    UIView.SetAnimationRepeatAutoreverses(false);
                    UIView.SetAnimationDelegate(this);

                    table_actionmore.Frame = new CGRect(table_actionmore.Frame.X, moreaction_view.Frame.Height - (maxheight + BT_moreaction_exit.Frame.Height + 60), moreaction_view.Frame.Width - (table_actionmore.Frame.X * 2), maxheight + 10);
                    table_actionmore.Alpha = 1;
                    moreaction_view.Alpha = 1;
                    UIView.CommitAnimations();
                }
                else
                {
                    UIView.BeginAnimations("toogle_docmenu_slideClose");
                    UIView.SetAnimationDuration(0.3f);
                    UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
                    UIView.SetAnimationRepeatCount(0);
                    UIView.SetAnimationRepeatAutoreverses(false);
                    UIView.SetAnimationDelegate(this);
                    table_actionmore.Frame = new CGRect(table_actionmore.Frame.X, moreaction_view.Frame.Bottom, table_actionmore.Frame.Width, maxheight + 10);
                    table_actionmore.Alpha = 0;
                    moreaction_view.Alpha = 0;
                    UIView.CommitAnimations();
                }
            }
            catch (Exception ex)
            { Console.WriteLine(ex.ToString()); }
        }

        public void HandleButtonBot(ViewElement element)
        {
            PresentationDelegate transitioningDelegate;
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
                    case (int)WorkflowAction.Action.Next: // 1- Duyệt
                        AgreeOrRejectView agreeOrReject = (AgreeOrRejectView)Storyboard.InstantiateViewController("AgreeOrRejectView");
                        agreeOrReject.setContent(action, this, workflowItem, null);
                        transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        agreeOrReject.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        agreeOrReject.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        agreeOrReject.TransitioningDelegate = transitioningDelegate;
                        this.PresentViewControllerAsync(agreeOrReject, true);
                        break;
                    case (int)WorkflowAction.Action.Approve: // 2 - Phê duyệt bước cuối
                        AgreeOrRejectView approve = (AgreeOrRejectView)Storyboard.InstantiateViewController("AgreeOrRejectView");
                        approve.setContent(action, this, workflowItem, null);
                        transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        approve.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        approve.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        approve.TransitioningDelegate = transitioningDelegate;
                        this.PresentViewControllerAsync(approve, true);
                        break;
                    case (int)WorkflowAction.Action.Forward: // 3 - chuyen xu ly
                        menu_action_Toggle();
                        ChangeUserProgress changeUserProgress = (ChangeUserProgress)Storyboard.InstantiateViewController("ChangeUserProgress");
                        changeUserProgress.setContent(this, action);
                        transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        changeUserProgress.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        changeUserProgress.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        changeUserProgress.TransitioningDelegate = transitioningDelegate;
                        this.PresentViewControllerAsync(changeUserProgress, true);
                        break;
                    case (int)WorkflowAction.Action.Return: // 4 - Yêu cầu hiệu chỉnh
                        menu_action_Toggle();
                        AgreeOrRejectView reject = (AgreeOrRejectView)Storyboard.InstantiateViewController("AgreeOrRejectView");
                        reject.setContent(action, this, workflowItem, null);
                        transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        reject.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        reject.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        reject.TransitioningDelegate = transitioningDelegate;
                        this.PresentViewControllerAsync(reject, true);
                        break;
                    case (int)WorkflowAction.Action.Reject: // 5 - Từ chối
                                                            // cho nay sau nay co them vo k ?
                                                            // neu co them thi them ham  menu_action_Toggle();
                    case (int)WorkflowAction.Action.Cancel:     // 51 -  Huy
                        menu_action_Toggle();
                        AgreeOrRejectView cancel = (AgreeOrRejectView)Storyboard.InstantiateViewController("AgreeOrRejectView");
                        cancel.setContent(action, this, workflowItem, null);
                        transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        cancel.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        cancel.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        cancel.TransitioningDelegate = transitioningDelegate;
                        this.PresentViewControllerAsync(cancel, true);
                        break;
                    case (int)WorkflowAction.Action.Recall: // 6 - thu hoi
                        SubmitAction(action, null);
                        break;
                    case (int)WorkflowAction.Action.RequestInformation: // 7 - yeu cau bo sung
                        menu_action_Toggle();
                        RequestAddInfo requestAddInfo = (RequestAddInfo)Storyboard.InstantiateViewController("RequestAddInfo");
                        requestAddInfo.setContent(this, action, lst_qtlc, workflowItem, null);
                        transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        requestAddInfo.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        requestAddInfo.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        requestAddInfo.TransitioningDelegate = transitioningDelegate;
                        this.PresentViewControllerAsync(requestAddInfo, true);
                        break;
                    case (int)WorkflowAction.Action.RecallAfterApproved: // 8 - Thu hồi đã phê duyệt
                        break;
                    case (int)WorkflowAction.Action.RequestIdea: // 9 - xin y kien tham van
                        break;
                    case (int)WorkflowAction.Action.Idea: // 10 - cho y kien
                        AgreeOrRejectView giveIdea = (AgreeOrRejectView)Storyboard.InstantiateViewController("AgreeOrRejectView");
                        giveIdea.setContent(action, this, workflowItem, null);
                        transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        giveIdea.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        giveIdea.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        giveIdea.TransitioningDelegate = transitioningDelegate;
                        this.PresentViewControllerAsync(giveIdea, true);
                        break;
                    case (int)WorkflowAction.Action.Save: // 11 -  luu
                        SubmitAction(action, null);
                        break;
                    case (int)WorkflowAction.Action.Submit: // 12 -  Gửi
                        SubmitAction(action, null);
                        break;
                    case (int)WorkflowAction.Action.Share: // 14 -  share
                        break;
                    case (int)WorkflowAction.Action.CreateTask: // 54 -  Phan cong xu ly
                                                                //CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("TEXT_FEATURE_UPDATING", "Tính năng đang được cập nhật..."));
                                                                //break;
                        menu_action_Toggle();
                        FormCreateTaskView createtask = (FormCreateTaskView)Storyboard.InstantiateViewController("FormCreateTaskView");
                        createtask.SetContent(workflowItem, null, this);
                        transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        createtask.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        createtask.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        createtask.TransitioningDelegate = transitioningDelegate;
                        this.NavigationController.PushViewController(createtask, true);
                        break;
                }
            }
            else
            {
                menu_action_Toggle();
            }
        }

        private void ActionSelected(ButtonAction action)
        {
            //string requiredcols = CmmFunction.ValidateEmptyRequiredField(lst_control_hasValue, dic_valueObject);
            string requiredcols = "";
            if (!String.IsNullOrEmpty(requiredcols))
            {
                if (CmmVariable.SysConfig.LangCode == "1033")
                {
                    //CmmIOSFunction.commonAlertMessage("Thông báo", "Please insert information into " + requiredcols.Remove(requiredcols.Length - 2) + ".").Show();
                }
                else
                {
                    //CmmIOSFunction.commonAlertMessage("Thông báo", "Vui lòng nhập thông tin vào " + requiredcols.Remove(requiredcols.Length - 2) + ".").Show();
                }
            }
            else
            {
                PresentationDelegate transitioningDelegate;
                CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
                CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);

                switch (action.ID)
                {
                    case (int)WorkflowAction.Action.Next: //  1 - duyet
                        AgreeOrRejectView agreeOrReject = (AgreeOrRejectView)Storyboard.InstantiateViewController("AgreeOrRejectView");
                        agreeOrReject.setContent(action, this, workflowItem, null);
                        transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        agreeOrReject.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        agreeOrReject.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        agreeOrReject.TransitioningDelegate = transitioningDelegate;
                        this.PresentViewControllerAsync(agreeOrReject, true);
                        break;
                    case (int)WorkflowAction.Action.Approve: // 2 - phe duyet bước cuối
                        AgreeOrRejectView approve = (AgreeOrRejectView)Storyboard.InstantiateViewController("AgreeOrRejectView");
                        approve.setContent(action, this, workflowItem, null);
                        transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        approve.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        approve.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        approve.TransitioningDelegate = transitioningDelegate;
                        this.PresentViewControllerAsync(approve, true);
                        break;
                    case (int)WorkflowAction.Action.Forward: // 3 - chuyen xu ly
                        menu_action_Toggle();
                        ChangeUserProgress changeUserProgress = (ChangeUserProgress)Storyboard.InstantiateViewController("ChangeUserProgress");
                        changeUserProgress.setContent(this, action);
                        transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        changeUserProgress.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        changeUserProgress.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        changeUserProgress.TransitioningDelegate = transitioningDelegate;
                        this.PresentViewControllerAsync(changeUserProgress, true);
                        break;
                    case (int)WorkflowAction.Action.Return: // 4 - Yêu cầu hiệu chỉnh
                        menu_action_Toggle();
                        AgreeOrRejectView reject = (AgreeOrRejectView)Storyboard.InstantiateViewController("AgreeOrRejectView");
                        reject.setContent(action, this, workflowItem, null);
                        transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        reject.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        reject.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        reject.TransitioningDelegate = transitioningDelegate;
                        this.PresentViewControllerAsync(reject, true);
                        break;
                    case (int)WorkflowAction.Action.Reject:     // 5 - Từ chối
                    case (int)WorkflowAction.Action.Cancel:     // 51 -  Huy
                        menu_action_Toggle();
                        AgreeOrRejectView cancel = (AgreeOrRejectView)Storyboard.InstantiateViewController("AgreeOrRejectView");
                        cancel.setContent(action, this, workflowItem, null);
                        transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        cancel.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        cancel.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        cancel.TransitioningDelegate = transitioningDelegate;
                        this.PresentViewControllerAsync(cancel, true);
                        break;
                    case (int)WorkflowAction.Action.Recall: // 6 - thu hoi
                        SubmitAction(action, null);
                        break;
                    case (int)WorkflowAction.Action.RequestInformation: // 7 - yeu cau bo sung
                        menu_action_Toggle();
                        RequestAddInfo requestAddInfo = (RequestAddInfo)Storyboard.InstantiateViewController("RequestAddInfo");
                        requestAddInfo.setContent(this, action, lst_qtlc, workflowItem, null);
                        transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        requestAddInfo.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        requestAddInfo.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        requestAddInfo.TransitioningDelegate = transitioningDelegate;
                        this.PresentViewControllerAsync(requestAddInfo, true);
                        break;
                    case (int)WorkflowAction.Action.RecallAfterApproved: // 8 - Thu hồi đã phê duyệt
                        break;
                    case (int)WorkflowAction.Action.RequestIdea: // 9 - xin y kien tham van
                        menu_action_Toggle();
                        ChangeUserProgress ConsultationIdea = (ChangeUserProgress)Storyboard.InstantiateViewController("ChangeUserProgress");
                        ConsultationIdea.setContent(this, action);
                        transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        ConsultationIdea.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        ConsultationIdea.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        ConsultationIdea.TransitioningDelegate = transitioningDelegate;
                        this.PresentViewControllerAsync(ConsultationIdea, true);
                        break;
                    case (int)WorkflowAction.Action.Idea: // 10 - cho y kien
                        break;
                    case (int)WorkflowAction.Action.Save: // 11 -  luu
                        SubmitAction(action, null);
                        break;
                    case (int)WorkflowAction.Action.Submit: // 12 -  Gửi
                        SubmitAction(action, null);
                        break;
                    case (int)WorkflowAction.Action.Share: // 14 -  share
                        FormShareView shareView = (FormShareView)Storyboard.InstantiateViewController("FormShareView");
                        //requestAddInfo.setContent(this, action, lst_qtlc, workflowItem, null);
                        transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        shareView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        shareView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        shareView.TransitioningDelegate = transitioningDelegate;
                        this.PresentViewControllerAsync(shareView, true);
                        break;
                    case (int)WorkflowAction.Action.CreateTask: // 54 -  Phan cong xu ly
                                                                //CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("TEXT_FEATURE_UPDATING", "Tính năng đang được cập nhật..."));
                                                                //break;
                        menu_action_Toggle();
                        FormCreateTaskView createtask = (FormCreateTaskView)Storyboard.InstantiateViewController("FormCreateTaskView");
                        createtask.SetContent(workflowItem, null, this);
                        transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        createtask.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        createtask.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        createtask.TransitioningDelegate = transitioningDelegate;
                        this.NavigationController.PushViewController(createtask, true);
                        break;
                }
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

        //thuyngo add cmt
        CGRect keyboardSize;
        nfloat a = 0.0f;
        public void KeyBoardUpNotification(NSNotification notification)
        {

            try
            {
                if (notification != null)
                    keyboardSize = UIKeyboard.BoundsFromNotification(notification);
                if (commentShowKey)
                {
                    table_content.ContentInset = new UIEdgeInsets(0, 0, keyboardSize.Height, 0);
                    table_content.ScrollToRow(NSIndexPath.FromRowSection(lst_section[0].ViewRows.Count - 1, 0), UITableViewScrollPosition.Top, true);
                    table_content.ScrollIndicatorInsets = table_content.ContentInset;
                }

            }
            catch (Exception ex)
            { Console.WriteLine("StartView - Err: " + ex.ToString()); }
        }
        private void KeyBoardDownNotification(NSNotification notification)
        {
            try
            {
                if (commentShowKey)
                {
                    table_content.ContentInset = new UIEdgeInsets(0, 0, 40, 0);
                    table_content.ScrollToRow(NSIndexPath.FromRowSection(lst_section[0].ViewRows.Count - 1, 0), UITableViewScrollPosition.Top, true);
                    table_content.ScrollIndicatorInsets = table_content.ContentInset;
                    commentShowKey = false;
                }
            }
            catch (Exception ex)
            { Console.WriteLine("FormCreateView - Err: " + ex.ToString()); }
        }

        public void UpdateTableSections(int sectionIndex, BeanComment comment)
        {
            var item = lst_comments.Where(i => i.ID == comment.ID).FirstOrDefault();
            item = comment;

            LoadData();
            //table_content.ReloadData();

        }
        //thuyngo add 
        private void CloseAddFollow()
        {

            Custom_AddFollowView custom_AddFollowView = Custom_AddFollowView.Instance;
            if (custom_AddFollowView.Superview != null)
                custom_AddFollowView.RemoveFromSuperview();
        }
        //thuyngo add 
        public void HandleAddFollow()
        {
            Custom_AddFollowView view_follow = Custom_AddFollowView.Instance;
            ButtonAction bt_follow = new ButtonAction();
            bt_follow.Value = "Follow";
            List<KeyValuePair<string, string>> _lstExtent = new List<KeyValuePair<string, string>>();
            _lstExtent.Add(new KeyValuePair<string, string>("status", isFollow ? "0" : "1"));

            SubmitAction(bt_follow, _lstExtent);

            if (view_follow.Superview != null)
                view_follow.RemoveFromSuperview();
        }
        //thuyngo add
        #region handle task
        public void Reloadrow(ViewElement element, NSIndexPath indexPath)
        {
            currentElement = element;
            currentIndexPath = indexPath;
            if (currentElement != null)
            {
                table_content.ReloadRows(new NSIndexPath[] { currentIndexPath }, UITableViewRowAnimation.Automatic);
            }
            else
                table_content.ReloadData();
        }
        #endregion
        //thuyngo add
        #region handle Comments
        public void SubmitLikeAction(ViewElement element, NSIndexPath sectionIndex, BeanComment comment)
        {
            UpdateTableSections(sectionIndex.Section, comment);
        }
        public async void SubmitComment(string content, List<BeanAttachFile> lst_commentAddAttachFile)
        {
            //try
            //{
            //    string commentvalue = null;
            //    if (!string.IsNullOrEmpty(content))
            //        commentvalue = content;
            //    else
            //    {

            //    }

            //    ProviderControlDynamic p_dynamic = new ProviderControlDynamic();
            //    List<KeyValuePair<string, string>> _lstKeyVarAttachmentLocal = new List<KeyValuePair<string, string>>();
            //    if (lst_commentAddAttachFile != null && lst_commentAddAttachFile.Count > 0) // Lấy những file thêm mới từ App ra
            //    {
            //        foreach (var item in lst_commentAddAttachFile)
            //        {
            //            if (item.ID == "")
            //            {
            //                string key = item.Title;
            //                KeyValuePair<string, string> _UploadFile = new KeyValuePair<string, string>(key, item.Path);
            //                _lstKeyVarAttachmentLocal.Add(_UploadFile);
            //            }
            //        }
            //    }

            //    await Task.Run(() =>
            //    {
            //        ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();

            //        //tracking
            //        ObjectSubmitDetailComment _objSubmitDetailComment = new ObjectSubmitDetailComment();
            //        // comment
            //        _objSubmitDetailComment.ID = _OtherResourceId; // empty or result
            //        _objSubmitDetailComment.ResourceCategoryId = "8";
            //        _objSubmitDetailComment.ResourceUrl = string.Format(CmmFunction.GetURLSettingComment(8), workflowItem.ID); // lấy trong beansetting
            //        _objSubmitDetailComment.ItemId = workflowItem.ID;
            //        _objSubmitDetailComment.Author = CmmVariable.SysConfig.UserId;
            //        _objSubmitDetailComment.AuthorName = CmmVariable.SysConfig.DisplayName;

            //        if (String.IsNullOrEmpty(_OtherResourceId))
            //            _OtherResourceId = _pControlDynamic.GetDetailOtherResource(_objSubmitDetailComment);
            //        else
            //            _pControlDynamic.GetDetailOtherResource(_objSubmitDetailComment);


            //        if (String.IsNullOrEmpty(_OtherResourceId))
            //            _OtherResourceId = _pControlDynamic.GetDetailOtherResource(_objSubmitDetailComment);
            //        else
            //            _pControlDynamic.GetDetailOtherResource(_objSubmitDetailComment);


            //        BeanOtherResource beanOtherResource = new BeanOtherResource();
            //        beanOtherResource.Content = commentvalue;
            //        beanOtherResource.ResourceId = _OtherResourceId;
            //        beanOtherResource.ResourceCategoryId = (int)CmmFunction.CommentResourceCategoryID.WorkflowItem;
            //        beanOtherResource.ResourceSubCategoryId = 0;
            //        beanOtherResource.Image = "";
            //        beanOtherResource.ParentCommentId = null; // cmt mới nên ko có parent


            //        bool _result = p_dynamic.AddComment(beanOtherResource, _lstKeyVarAttachmentLocal);

            //        InvokeOnMainThread(() =>
            //        {
            //            if (_result)
            //            {
            //                try
            //                {
            //                    loading.Hide();

            //                    status_selected_index = 1;
            //                    status_selected_index_default = 1;
            //                    duedate_selected_index = 0;
            //                    duedate_selected_index_default = 0;

            //                    LoadDataFilterTodo(status_selected_index, duedate_selected_index, fromDate_default, toDate_default);
            //                    LoadItemSelected(true);

            //                }
            //                catch (Exception ex)
            //                {
            //                    Console.WriteLine("TodoDetailsView - SubmitAction - Invoke - Err: " + ex.ToString());
            //                    CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"));
            //                }
            //            }
            //            else
            //            {
            //                CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"));
            //            }
            //        });
            //    });
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("FormCommentView - SubmitComment - Err: " + ex.ToString());
            //}
        }
        //Comment - reply
        public void NavigateToReplyComment(NSIndexPath _itemIndex, BeanComment comment)
        {
            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            FormCommentView formComment = (FormCommentView)Storyboard.InstantiateViewController("FormCommentView");
            formComment.SetContent(this, workflowItem, comment, _OtherResourceId, _itemIndex);
            PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            formComment.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            formComment.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            formComment.TransitioningDelegate = transitioningDelegate;
            this.NavigationController.PushViewController(formComment, true);
            //this.PresentModalViewController(formComment, true);
        }
        public void ScrollToCommentViewRow(nfloat estHeight)
        {
            //if (estHeight > 420)
            //    table_content_right.ScrollToRow(NSIndexPath.FromRowSection(lst_section[0].ViewRows.Count - 1, 0), UITableViewScrollPosition.Top, true);
            //else
            //{
            //    CGRect keyboardSize = new CGRect(0, 0, 1194, 400);
            //    CGRect custFrame = table_content_right.Frame;
            //    custFrame.Y -= keyboardSize.Height;
            //    table_content_right.Frame = custFrame;
            //}
        }
        #endregion

        #region handle user choice
        public void HandleUserMultiChoiceSelected(ViewElement element, List<BeanUser> _userSelected)
        {
            List<BeanUser> lst_userChoice = _userSelected;
            //if (!string.IsNullOrEmpty(element.Value))
            //{
            //    JArray json = JArray.Parse(element.Value);
            //    lst_userChoice = json.ToObject<List<BeanUser>>();
            //}

            if (lst_userChoice != null && lst_userChoice.Count > 0)
            {
                foreach (var item in lst_userChoice)
                {
                    item.Name = item.FullName;
                }
            }

            string jsonString = string.Empty;
            if (lst_userChoice != null && lst_userChoice.Count > 0)
                jsonString = JsonConvert.SerializeObject(lst_userChoice);

            element.Value = jsonString;
            UpdateValueElement_InListSection(element);
            table_content.ReloadData();
        }

        public void HandleUserSingleChoiceSelected(ViewElement element, BeanUser _userSelected)
        {
            List<BeanUser> lst_userChoice = new List<BeanUser>();
            if (_userSelected != null)
                lst_userChoice.Add(_userSelected);

            string jsonString = string.Empty;
            if (lst_userChoice != null && lst_userChoice.Count > 0)
                jsonString = JsonConvert.SerializeObject(lst_userChoice);

            element.Value = jsonString;
            UpdateValueElement_InListSection(element);
            table_content.ReloadData();
        }
        public void NavigatorToUserChoiceView(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
        {
            if (element != null)
            {
                List<BeanUser> lst_data = new List<BeanUser>();
                lbl_title.Text = element.Title;

                ListUserView listUserView = (ListUserView)Storyboard.InstantiateViewController("ListUserView");
                if (element.DataType == "selectuser")
                    listUserView.setContent(this, false, null, false, element, element.Title);
                else if (element.DataType == "selectusermulti")
                    listUserView.setContent(this, true, null, false, element, element.Title);
                this.NavigationController.PushViewController(listUserView, true);
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
                List<BeanUser> lst_data = new List<BeanUser>();
                lbl_title.Text = element.Title;

                ListUserOrGroupView listUserOrGroup = (ListUserOrGroupView)Storyboard.InstantiateViewController("ListUserOrGroupView");
                if (element.DataType == "selectusergroup")
                    listUserOrGroup.setContent(this, false, null, false, element, element.Title);
                else if (element.DataType == "selectusergroupmulti")
                    listUserOrGroup.setContent(this, true, null, false, element, element.Title);
                this.NavigationController.PushViewController(listUserOrGroup, true);
            }
            else
            {
                CmmIOSFunction.commonAlertMessage(this, "BPM", "Không tìm thấy thông tin phiếu");
            }
        }
        public void HandleUserOrGroupSingleChoiceSelected(ViewElement element, BeanUserAndGroup _userSelected)
        {
            List<BeanUserAndGroup> lst_userChoice = new List<BeanUserAndGroup>();
            if (_userSelected != null)
                lst_userChoice.Add(_userSelected);
            var jsonString = JsonConvert.SerializeObject(lst_userChoice);
            element.Value = jsonString;

            UpdateValueElement_InListSection(element);
            table_content.ReloadData();
        }
        public void HandleUserOrGroupMultiChoiceSelected(ViewElement element, List<BeanUserAndGroup> _userSelected)
        {
            List<BeanUserAndGroup> lst_userChoice = _userSelected;

            if (lst_userChoice != null && lst_userChoice.Count > 0)
            {
                foreach (var item in lst_userChoice)
                {
                    item.Name = item.Name;
                }
            }

            var jsonString = JsonConvert.SerializeObject(lst_userChoice);
            element.Value = jsonString;
            table_content.ReloadData();
        }
        #endregion

        #region Handle_YesNoResult
        public void Handle_YesNoResult(ViewElement element)
        {
            UpdateValueElement_InListSection(element);
            table_content.ReloadData();
        }
        #endregion

        #region handle number - currence
        public void NavigatorToEditNumberView(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
        {
            NumberControlView numberControlView = (NumberControlView)Storyboard.InstantiateViewController("NumberControlView");
            numberControlView.setContent(this, 1, element);
            this.NavigationController.PushViewController(numberControlView, true);
        }

        public void HandleEditNumber(ViewElement element)
        {
            UpdateValueElement_InListSection(element);
            table_content.ReloadData();
        }
        #endregion

        #region handle choice
        public void HandleChoiceSelected(ViewElement element)
        {
            UpdateValueElement_InListSection(element);
            table_content.ReloadData();
        }
        public void NavigatorToChoiceView(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
        {
            if (element != null)
            {
                MultiChoiceView multiChoiceView = (MultiChoiceView)Storyboard.InstantiateViewController("MultiChoiceView");
                //thuyngo del
                //if (element.DataType == "singlechoice")
                //    multiChoiceView.setContent(this, false, element);
                //else if (element.DataType == "multiplechoice")
                //    multiChoiceView.setContent(this, true, element);
                //thuyngo add
                if (element.DataType == "singlechoice")
                    multiChoiceView.setContent(this, false, element);
                else if (element.DataType == "singlelookup")
                    multiChoiceView.setContent(this, false, element);
                else if (element.DataType == "multiplechoice")
                    multiChoiceView.setContent(this, true, element);
                else if (element.DataType == "multilookup")
                    multiChoiceView.setContent(this, true, element);

                this.NavigationController.PushViewController(multiChoiceView, true);
            }
            else
            {
                CmmIOSFunction.commonAlertMessage(this, "BPM", "Không tìm thấy thông tin phiếu");
            }
        }
        #endregion
        #region handle input text format
        public async Task NavigatorToViewAsync(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
        {
            currentElement = element;
            currentIndexPath = indexPath;
            contentHtmlString = element.Value;

            this.View.EndEditing(true);

            if (_controlBase != null)
            {
                switch (_controlBase.GetType().Name)
                {
                    case "ControlTextInputFormat":
                        //TFormatEditorView formatEditorView = Storyboard.InstantiateViewController("TFormatEditorView") as TFormatEditorView;
                        //this.NavigationController.PushViewController(formatEditorView, false);
                        //formatEditorView.SetAutoFocusInput(true);

                        TEditorResponse response = await CmmIOSFunction.ShowTEditor(true, contentHtmlString, this, this, tvc, null);//
                                                                                                                                    //await ShowTEditor(contentHtmlString, null, true);
                        appD.NavController.NavigationBarHidden = true;
                        if (response.IsSave)
                        {
                            if (response.HTML != null)
                            {
                                contentHtmlString = response.HTML;
                                SetValueResult(contentHtmlString);
                            }
                        }
                        break;
                    case "ControlInputAttachmentHorizon":
                        FormEditAttachFileView formEditAttachFileView = Storyboard.InstantiateViewController("FormEditAttachFileView") as FormEditAttachFileView;
                        var control = _controlBase as ControlInputAttachmentHorizon;
                        formEditAttachFileView.SetContent(this, control.currentAttachment);
                        this.PresentModalViewController(formEditAttachFileView, true);
                        break;
                    default:
                        break;
                }
            }
        }
        public Task<TEditorResponse> ShowTEditor(string html, ToolbarBuilder toolbarBuilder = null, bool autoFocusInput = false)
        {
            TaskCompletionSource<TEditorResponse> taskRes = new TaskCompletionSource<TEditorResponse>();
            var tvc = new TEditorViewController();
            ToolbarBuilder builder = toolbarBuilder;
            if (toolbarBuilder == null)
                builder = new ToolbarBuilder().AddStandard();
            tvc.BuildToolbar(builder);
            tvc.SetHTML(html);
            tvc.SetAutoFocusInput(autoFocusInput);
            tvc.Title = currentElement.Title;

            UIView headerView = new UIView();
            headerView.Frame = top_view.Frame;
            headerView.BackgroundColor = UIColor.FromRGB(65, 80, 134);

            UIButton BT_left = new UIButton();
            BT_left.SetImage(UIImage.FromFile("Icons/icon_back3x.png"), UIControlState.Normal);
            BT_left.Frame = new CGRect(16, 35, 30, 30);
            BT_left.ContentMode = UIViewContentMode.Center;
            BT_left.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 15);
            BT_left.TouchUpInside += delegate
            {
                this.NavigationController.PopViewController(true);
            };

            UIButton BT_right = new UIButton();
            BT_right.SetImage(UIImage.FromFile("Icons/icon_check.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
            BT_right.TintColor = UIColor.White;
            BT_right.Frame = new CGRect(headerView.Frame.Width - 40, 35, 30, 30);
            BT_right.ContentMode = UIViewContentMode.Center;
            BT_right.ContentEdgeInsets = new UIEdgeInsets(6, 5, 5, 6);
            BT_right.TouchUpInside += async delegate
            {
                taskRes.SetResult(new TEditorResponse() { IsSave = true, HTML = await tvc.GetHTML() });
                this.NavigationController.PopViewController(true);
            };
            UILabel lbl_title = new UILabel()
            {
                Font = UIFont.FromName("ArialMT", 16f),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                Frame = new CGRect(BT_left.Frame.Right - 10, 35, headerView.Frame.Width - (BT_left.Frame.Right - 10) - BT_right.Frame.Width, 30),
                Text = currentElement.Title
            };
            headerView.AddSubviews(BT_left, BT_right, lbl_title);

            foreach (var item in tvc.View.Subviews)
            {
                if (item.GetType().Name == "UIWebView")
                {
                    item.Frame = new CGRect(item.Frame.X, headerView.Frame.Bottom, item.Frame.Width, item.Frame.Height - headerView.Frame.Height);
                }
            }

            tvc.View.AddSubview(headerView);

            UINavigationController nav = this.NavigationController;
            //nav.NavigationBar.Frame = new CGRect(nav.NavigationBar.Frame.X, nav.NavigationBar.Frame.Y, nav.NavigationBar.Frame.Width, 10);
            //nav.NavigationBar.BackgroundColor = UIColor.FromRGB(65, 80, 134);
            //nav.NavigationBar.BarTintColor = UIColor.FromRGB(65, 80, 134);
            //nav.NavigationBar.TintColor = UIColor.White;
            //nav.NavigationBar.TitleTextAttributes = new UIStringAttributes
            //{
            //    ForegroundColor = UIColor.White,
            //    Font = UIFont.SystemFontOfSize(15f, UIFontWeight.Bold)
            //};



            //tvc.NavigationItem.SetLeftBarButtonItem(new UIBarButtonItem(CrossTEditor.CancelText, UIBarButtonItemStyle.Plain, (item, args) =>
            //{
            //    if (nav != null)
            //        nav.PopViewController(false);

            //    taskRes.SetResult(new TEditorResponse() { IsSave = false, HTML = string.Empty });
            //}), true);

            //UIBarButtonItem barBtnLeft = new UIBarButtonItem(UIImage.FromFile("Icons/icon_back3x.png"), UIBarButtonItemStyle.Plain, (item, args) =>
            //{
            //    if (nav != null)
            //        nav.PopViewController(false);

            //    taskRes.SetResult(new TEditorResponse() { IsSave = false, HTML = string.Empty });
            //});
            ////barBtnLeft.ImageInsets = new UIEdgeInsets(8, -8, -8, -16);

            //tvc.NavigationItem.SetLeftBarButtonItem(barBtnLeft, false);

            //tvc.NavigationItem.SetRightBarButtonItem(new UIBarButtonItem(CrossTEditor.SaveText, UIBarButtonItemStyle.Done, async (item, args) =>
            //{
            //    if (nav != null)
            //        nav.PopViewController(false);

            //    taskRes.SetResult(new TEditorResponse() { IsSave = true, HTML = await tvc.GetHTML() });
            //}), true);

            if (nav != null)
                nav.PushViewController(tvc, false);

            //appD.NavController.NavigationBarHidden = false;

            return taskRes.Task;
        }
        public void SetValueResult(string value)
        {
            //if (!string.IsNullOrEmpty(value))
            //{
            if (currentElement != null)
            {
                currentElement.Value = value;
                table_content.ReloadRows(new NSIndexPath[] { currentIndexPath }, UITableViewRowAnimation.None);
            }
            //}
        }
        #endregion

        #region handle input text
        public void NavigatorToEditTextView(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
        {
            TextViewControlView textViewControlView = (TextViewControlView)Storyboard.InstantiateViewController("TextViewControlView");
            textViewControlView.setContent(this, 1, element);
            this.NavigationController.PushViewController(textViewControlView, true);
        }

        public void HandleSingleLine(ViewElement element)
        {
            UpdateValueElement_InListSection(element);
            table_content.ReloadData();
        }
        public void NavigatorTouchMoreToFullTextView(string title, string value)
        {
            FullTextView fullTextView = (FullTextView)Storyboard.InstantiateViewController("FullTextView");
            fullTextView.SetContent(title, value);
            this.NavigationController.PushViewController(fullTextView, true);
        }

        public void NavigatorToFullTextView(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
        {
            FullTextView fullTextView = (FullTextView)Storyboard.InstantiateViewController("FullTextView");
            fullTextView.SetContent(element.Title, element.Value);
            this.NavigationController.PushViewController(fullTextView, true);
        }
        #endregion

        #region handle Attachment
        public void HandleAddAttachment(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
        {
            try
            {
                //currentElementAttachFile = element;
                this.View.EndEditing(true);
                attachmentElement = element;
                addAttachmentsView = Storyboard.InstantiateViewController("AddAttachmentsView") as AddAttachmentsView;
                addAttachmentsView.SetContent(this, element);
                this.NavigationController.PushViewController(addAttachmentsView, true);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("CreateNewTaskView - HandleAddAttachment - Err: " + ex.ToString());
#endif
            }
        }

        public void HandleAddAttachFileResult(BeanAttachFileLocal _attachFile, AddAttachmentsView addAttachmentsView, string elementdataType)
        {
            ViewElement viewElement = GetViewElementByDataType(elementdataType);
            //ViewElement viewElement = GetViewElementByDataType("inputattachmenthorizon");
            if (elementdataType == "inputattachmenthorizon")
            {
                lst_addAttachment = new List<BeanAttachFile>();
                lst_addCommentAttachment = new List<BeanAttachFile>();
                if (!string.IsNullOrEmpty(viewElement.Value))
                {
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
            table_content.ReloadData();

            if (addAttachmentsView != null)
                addAttachmentsView.NavigationController.PopViewController(false);

        }

        public void NavigationToDocumentPicker(AddAttachmentsView addAttachmentsView, string elementDatatype)
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

                            //BeanAttachFileLocal itemiCloudAndDevice = new BeanAttachFileLocal() { ID = -1, Name = fileName, Path = filePath, Size = FileSizeFormatter.FormatSize(size),  Type = type };
                            BeanAttachFileLocal itemiCloudAndDevice = new BeanAttachFileLocal() { ID = -1, Name = fileName, Path = filePath, Size = size, Type = type };
                            HandleAddAttachFileResult(itemiCloudAndDevice, addAttachmentsView, elementDatatype);
                        }
                        else
                        {
                            CmmIOSFunction.AlertUnsupportFile(this);
                            Console.WriteLine("Selected file type: " + type);
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

        public void NavigationToImagePicker(AddAttachmentsView addAttachmentsView)
        {
            if (imagePicker != null)
            {
                imagePicker.Dispose();
                imagePicker = null;
            }

            imagePicker = new UIImagePickerController();

            imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
            // imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);
            imagePicker.MediaTypes = new string[] { UTType.Image };
            imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
            imagePicker.Canceled += Handle_Canceled;

            addAttachmentsView.PresentViewController(imagePicker, true, null);

        }

        public void NavigationToCameraPicker(AddAttachmentsView addAttachmentsView)
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

            addAttachmentsView.PresentViewController(imagePicker, true, null);
        }

        protected void Handle_FinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e)
        {
            bool isImage = false;
            var mediaType = e.Info[UIImagePickerController.MediaType].ToString();
            switch (mediaType)
            {
                case "public.image":
                    Console.WriteLine("Image selected");
                    isImage = true;
                    break;
                case "public.video":
                case "public.movie":
                    Console.WriteLine("Video or Movie selected");
                    break;
                default:
                    Console.WriteLine("Selected media type: " + mediaType);
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
                        HandleAddAttachFileResult(itemiCloudAndDevice, null, attachmentElement.DataType);
                    }
                    else
                    {
                        string fileName = "IMG_" + DateTime.Now.ToString("MMss") + ".JPG";
                        //CollectFileInfo(fileName, originalImage);

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
                        HandleAddAttachFileResult(itemiCloudAndDevice, null, attachmentElement.DataType);
                    }
                }
            }
            else
            {
                CmmIOSFunction.AlertUnsupportFile(this);
            }

            // dismiss the picker
            imagePicker.DismissModalViewController(false);
            var vc = this.PresentedViewController;
            vc.DismissViewController(true, null);

            if (addAttachmentsView != null)
                addAttachmentsView.HandleAddAttachFileClose();
        }

        private void Handle_Canceled(object sender, EventArgs e)
        {
            imagePicker.DismissModalViewController(true);
        }

        public void HandleAttachmentRemove(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase, int numRow, BeanAttachFile beanAttachFile)
        {
            if (beanAttachFile != null)
                lst_attachRemove.Add(beanAttachFile);

            json_attachRemove = Newtonsoft.Json.JsonConvert.SerializeObject(lst_attachRemove);

            List<BeanAttachFile> lst_item = new List<BeanAttachFile>();
            if (!string.IsNullOrEmpty(element.Value))
            {
                JArray json = JArray.Parse(element.Value);
                lst_item = json.ToObject<List<BeanAttachFile>>();
            }

            var jsonString = JsonConvert.SerializeObject(lst_item);
            element.Value = jsonString;

            table_content.ReloadData();
        }

        public void HandleAttachmentThumbRemove(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase, int numRow, string _json_attachRemove, string keyNote)
        {
            if (element.Notes != null && element.Notes.Count > 0)
            {
                foreach (var note in element.Notes)
                {
                    if (note.Key == "image" && keyNote == "image")
                    {
                        note.Value = _json_attachRemove;
                    }
                    else if (note.Key == "doc" && keyNote == "doc")
                    {
                        note.Value = _json_attachRemove;
                    }
                }
            }

            table_content.ReloadData();
        }
        public void HandleAttachmentEdit(ViewElement element, NSIndexPath indexPath, BeanAttachFile _attach, ControlBase _controlBase)
        {
            FormListItemView formListItem = (FormListItemView)Storyboard.InstantiateViewController("FormListItemView");
            formListItem.setContent(this, _attach, element, false);
            this.NavigationController.PushViewController(formListItem, true);
        }

        public void ReloadAttachmentElement(ViewElement _element, BeanAttachFile attachFile)
        {
            List<BeanAttachFile> lst_attachFile = new List<BeanAttachFile>();
            if (!string.IsNullOrEmpty(_element.Value))
            {
                JArray json = JArray.Parse(_element.Value);
                lst_attachFile = json.ToObject<List<BeanAttachFile>>();
            }
            var index = -1;
            if (!string.IsNullOrEmpty(attachFile.ID))
                index = lst_attachFile.FindIndex(item => item.ID == attachFile.ID);
            else
                index = lst_attachFile.FindIndex(item => item.Title == attachFile.Title);

            if (index != -1)
                lst_attachFile[index] = attachFile;
            var jsonString = JsonConvert.SerializeObject(lst_attachFile);
            _element.Value = jsonString;

            table_content.ReloadData();
        }

        public void NavigateToAttachView(BeanAttachFile currentAttachFile)
        {
            ShowAttachmentView showAttachmentView = Storyboard.InstantiateViewController("ShowAttachmentView") as ShowAttachmentView;
            showAttachmentView.setContent(this, currentAttachFile);
            this.NavigationController.PushViewController(showAttachmentView, true);
        }

        #endregion

        #region handle WorkflowRelate
        public void HandleRemoveWorkFlowRelate(BeanWorkFlowRelated wfRelate, NSIndexPath nSIndexPath)
        {
            var index = lstWorkFlowRelateds.FindIndex(item => item.ID == wfRelate.ID);
            if (index != -1)
                lstWorkFlowRelateds.RemoveAt(index);

            table_content.ReloadRows(new NSIndexPath[] { nSIndexPath }, UITableViewRowAnimation.Fade);
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
                RequestDetailsV2 v2 = (RequestDetailsV2)Storyboard.InstantiateViewController("RequestDetailsV2");
                v2.setContent(this, lst_result[0]);
                this.NavigationController.PushViewController(v2, true);
            }
            else
            {
                CmmIOSFunction.commonAlertMessage(this, "BPM", "Không tìm thấy thông tin phiếu");
            }
        }
        public void HandleWorkRelatedResult(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase, int numRow)
        {
            table_content.ReloadData();
        }
        #endregion

        #region handle Assignment Tasks
        public async void Handle_RemoveTask(BeanTask _task, NSIndexPath nSIndexPath)
        {
            try
            {
                loading = new CmmLoading(new CGRect((this.View.Bounds.Width - 200) / 2, (this.View.Bounds.Height - 200) / 2, 200, 200), "Đang xử lý...");
                this.View.Add(loading);

                bool res = false;
                await Task.Run(() =>
                {
                    ProviderControlDynamic p_dynamic = new ProviderControlDynamic();
                    res = p_dynamic.DeleteDetailTaskForm(_task.ID);

                    InvokeOnMainThread(() =>
                    {
                        loading.Hide();

                        if (res)
                            LoadData();
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
        public void Handle_TaskSelected(BeanTask _task, NSIndexPath nSIndexPath)
        {
            FormTaskDetails taskDetails = (FormTaskDetails)Storyboard.InstantiateViewController("FormTaskDetails");
            taskDetails.SetContent(_task, workflowItem, this);
            this.NavigationController.PushViewController(taskDetails, true);
        }
        public void Handle_TaskResult(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase, int numRow)
        {
            table_content.ReloadData();
        }
        #endregion

        #region handle DateTime choice
        public void HandleDateTimeChoiceChoice(ViewElement element)
        {
            UpdateValueElement_InListSection(element);
            table_content.ReloadData();
        }
        public void NavigatorToDateTimeChoice(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
        {
            PresentationDelegate transitioningDelegate;
            CGRect startFrame = new CGRect(this.View.Frame.Width / 2, this.View.Frame.Height / 2, 0, 0);
            CGSize showSize = new CGSize(384, 450);

            FormCalendarChoice formCalendarChoice = (FormCalendarChoice)Storyboard.InstantiateViewController("FormCalendarChoice");
            formCalendarChoice.setContent(this, element);
            transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            formCalendarChoice.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            formCalendarChoice.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            formCalendarChoice.TransitioningDelegate = transitioningDelegate;
            this.PresentViewControllerAsync(formCalendarChoice, true);
        }
        #endregion

        //thuyngo add
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
            this.NavigationController.PushViewController(detailsProperty, true);
        }

        public void HandlePropertyDetailsRemove(JObject jObjectRemove)
        {
            lstGridDetail_Deleted.Add(jObjectRemove);
            table_content.ReloadData();
        }
        #endregion

        // Thuc hien action tu cac popup hoac form
        public async void SubmitAction(ButtonAction _buttonAction, List<KeyValuePair<string, string>> _lstExtent)
        {
            // kiem tra cac truong co dau "*" dien day du chua
            bool flagRequier = CmmFunction.ValidateRequiredForm(lst_section);
            if (!flagRequier && _buttonAction.Value != "Follow")
            {
                CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("MESS_FIELD_REQUIRE", "Vui lòng nhập đầy đủ thông tin."));
                return;
            }

            loading = new CmmLoading(new CGRect((this.View.Bounds.Width - 200) / 2, (this.View.Bounds.Height - 200) / 2, 200, 200), "Đang xử lý...");
            this.View.Add(loading);
            try
            {
                if (moreaction_view.Alpha == 1)
                    menu_action_Toggle();

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
                            else if ((element.DataType != "inputcomments"))
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
                                KeyValuePair<string, string> img_info = new KeyValuePair<string, string>(key, item.Path);//new;
                                lst_files.Add(img_info);
                            }
                        }
                    }
                    string str_errMess = string.Empty;
                    if (lstExtent != null && lstExtent.Count > 0)
                        result = providerControl.SendControlDynamicAction(_buttonAction.Value, workflowItem.ID.ToString(), str_json_FormDefineInfo, json_edit_element, ref str_errMess, lst_files, lstExtent);
                    else
                        result = providerControl.SendControlDynamicAction(_buttonAction.Value, workflowItem.ID.ToString(), str_json_FormDefineInfo, json_edit_element, ref str_errMess, lst_files, null);

                    if (result)
                    {
                        isReloadKanBanView = true;

                        //Task.Run(() => {});
                        b_pase.UpdateAllDynamicData(true);

                        InvokeOnMainThread(() =>
                        {
                            loading.Hide();
                            if (_buttonAction.Value == "Follow")
                            {
                                //img star
                                if (isFollow == true)
                                {
                                    BT_star.SetImage(UIImage.FromFile("Icons/icon_Star_off.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
                                    isFollow = false;
                                }
                                else
                                {
                                    BT_star.SetImage(UIImage.FromFile("Icons/icon_Star_on.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
                                    isFollow = true;
                                }

                                // UPDATE lại dưới DB + Tick event Renew lại Item ở trang trước
                                SQLiteConnection conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                                string UpdateFavorite = @"UPDATE BeanWorkflowFollow SET Status = {0} WHERE WorkflowItemId = '{1}'";
                                string _queryUpdateFavorite = String.Format(UpdateFavorite, isFollow == true ? 1 : 0, workflowItem.ID);
                                conn.Execute(_queryUpdateFavorite);
                                conn.Close();
                            }
                            else
                            {
                                if (parentView != null)
                                {
                                    if (parentView is MainView)
                                        (parentView as MainView).doReload = false;
                                    else if (parentView is RequestListView)
                                        (parentView as RequestListView).doReload = false;
                                    else if (parentView is MyRequestListView)
                                        (parentView as MyRequestListView).doReload = false;
                                    //else if (parentView is FollowListViewController)
                                    //   (parentView as FollowListViewController).doReload = false;
                                    else if (parentView is KanBanView)
                                        (parentView as KanBanView).reloadData();
                                }

                                if (!isFromPush)
                                {
                                    if (this.NavigationController != null)
                                        this.NavigationController.PopViewController(true);
                                    else
                                        this.DismissViewControllerAsync(true);
                                }
                                else
                                    this.NavigationController.PopToRootViewController(true);
                            }

                            //this.NavigationController.PopViewController(true);
                        });
                    }
                    else
                    {
                        isReloadKanBanView = false;
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

        public void updateDictValue(string _key, string _value)
        {
            if (dic_valueObject != null)
            {
                if (dic_valueObject.ContainsKey(_key))
                    dic_valueObject[_key] = _value;

                // gán lai value vào các dynamic control tương ứng
                if (lst_control_hasValue != null)
                {
                    foreach (var item in lst_control_hasValue)
                    {
                        if (dic_valueObject.ContainsKey(item.DataField))
                        {
                            var value = dic_valueObject[item.DataField];
                            item.ControlValue = value;
                        }
                    }
                    //table_content.Source = new Control_TableSource(lst_control_hasValue, lst_attachFile, this);
                    //table_content.ReloadData();

                }
            }

            //ANDROID
            //CmmFunction.SetPropertyValue(tv_ChangeValue, prop, lst_num[num.Value]);
            //dic_values[tag_ViewChangeValue] = lst_dataLookup[num.Value].ID + ";#" + lst_dataLookup[num.Value].Title;
        }

        // <summary>
        /// Cap nhat lai Danh sach Data / Form
        /// </summary>
        /// <param name="isChangeFocus"></param>
        public void ReloadDataForm(bool isChangeFocus)
        {
            if (isChangeFocus)
            {
                // currentItemSelected = null, auto focus to item first
                currentElement = null;
            }

            //LoadDataFilterTodo(status_selected_index, duedate_selected_index, fromDate_default, toDate_default);
            LoadData();
        }
        /// <summary>
        /// Viec Den toi
        /// </summary>
        /// <param name="_statusIndex">0: Tatca | 1: Can XL | 2: Da XL</param>
        /// <param name="_dueDateIndex">0: Tatca | 1: QuaHan | 2: TrongHan </param>
        //public void LoadDataFilterTodo(int _statusIndex, int _dueDateIndex, DateTime _fromdate, DateTime _todate)
        //{
        //    try
        //    {
        //        if (isFilter)
        //            BT_filter.TintColor = UIColor.Orange;
        //        else
        //            BT_filter.TintColor = UIColor.Black;

        //        var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
        //        string count_query_vcxl = string.Empty;
        //        string query = string.Empty;

        //        dict_todo = new Dictionary<string, List<BeanNotify>>();
        //        dict_sectionTodo = new Dictionary<string, bool>();
        //        lst_notify_cxl = new List<BeanNotify>();

        //        string date_filter = string.Empty;
        //        if (_fromdate.Year != 01 && _todate.Year != 01)
        //        {
        //            fromDateSelected = _fromdate;
        //            toDateSelected = _todate;

        //            if (fromDateSelected.Year.ToString() != "1" && toDateSelected.Year.ToString() != "1")
        //                date_filter = string.Format("(Action = 'Task' OR (Action <> 'Task' AND SubmitActionId <> 0)) AND Created IS NOT NULL AND (Created >= '{0}' AND Created <= '{1}')", fromDateSelected.ToString("yyyy-MM-dd"), toDateSelected.AddDays(1).ToString("yyyy-MM-dd"));
        //        }

        //        // Status - tat ca
        //        if (_statusIndex == 0)
        //        {
        //            if (_dueDateIndex == 0) // DueDate - Tatca
        //            {
        //                if (!string.IsNullOrEmpty(date_filter))
        //                {
        //                    count_query_vcxl = string.Format("SELECT Count(*) as count FROM BeanNotify WHERE {0}", date_filter);
        //                    query = string.Format("SELECT * FROM BeanNotify WHERE {0} Order By Created DESC LIMIT ? OFFSET ?", date_filter);
        //                }

        //                countnum_vcxl = conn.Query<CountNum>(count_query_vcxl);
        //                lst_notify_cxl = conn.Query<BeanNotify>(query, limit, offset);
        //            }
        //            else if (_dueDateIndex == 1) // DueDate - Qua Han
        //            {
        //                if (!string.IsNullOrEmpty(date_filter))
        //                {
        //                    count_query_vcxl = string.Format("SELECT Count(*) as count FROM BeanNotify WHERE {0} AND (DueDate IS NOT NULL AND DueDate < date('now'))", date_filter);
        //                    query = string.Format("SELECT * FROM BeanNotify WHERE {0} AND (DueDate IS NOT NULL AND DueDate < date('now')) Order By Created DESC LIMIT ? OFFSET ?", date_filter);
        //                }

        //                countnum_vcxl = conn.Query<CountNum>(count_query_vcxl);
        //                lst_notify_cxl = conn.Query<BeanNotify>(query, limit, offset);
        //            }
        //            else if (_dueDateIndex == 2) // DueDate - Trong han
        //            {
        //                if (!string.IsNullOrEmpty(date_filter))
        //                {
        //                    count_query_vcxl = string.Format("SELECT Count(*) as count FROM BeanNotify WHERE {0} AND (DueDate IS NOT NULL AND DueDate >= ?)", date_filter);
        //                    query = string.Format("SELECT * FROM BeanNotify WHERE {0} AND (DueDate IS NOT NULL AND DueDate >= ?) Order By Created DESC LIMIT ? OFFSET ?", date_filter);
        //                }

        //                countnum_vcxl = conn.Query<CountNum>(count_query_vcxl, DateTime.Now.Date);
        //                lst_notify_cxl = conn.Query<BeanNotify>(query, DateTime.Now.Date, limit, offset);
        //            }
        //        }
        //        // Status - Can XL
        //        else if (_statusIndex == 1)
        //        {
        //            // Can XL - DueDate - Tatca
        //            if (_dueDateIndex == 0)
        //            {
        //                if (!string.IsNullOrEmpty(date_filter))
        //                {
        //                    count_query_vcxl = string.Format("SELECT Count(*) as count FROM BeanNotify WHERE {0} AND Status = 0", date_filter);
        //                    query = string.Format("SELECT * FROM BeanNotify WHERE {0} AND Status = 0 Order By Created DESC LIMIT ? OFFSET ?", date_filter);
        //                }

        //                countnum_vcxl = conn.Query<CountNum>(count_query_vcxl);
        //                lst_notify_cxl = conn.Query<BeanNotify>(query, limit, offset);
        //            }
        //            // Can XL - DueDate - Qua Han
        //            else if (_dueDateIndex == 1)
        //            {
        //                if (!string.IsNullOrEmpty(date_filter))
        //                {
        //                    count_query_vcxl = string.Format("SELECT Count(*) as count FROM BeanNotify WHERE {0} AND Status = 0 AND (DueDate IS NOT NULL AND DueDate < date('now'))", date_filter);
        //                    query = string.Format("SELECT * FROM BeanNotify WHERE {0} AND Status = 0 AND (DueDate IS NOT NULL AND DueDate < date('now')) Order By Created DESC LIMIT ? OFFSET ?", date_filter);
        //                }

        //                countnum_vcxl = conn.Query<CountNum>(count_query_vcxl);
        //                lst_notify_cxl = conn.Query<BeanNotify>(query, limit, offset);
        //            }
        //            // Can XL - DueDate - Trong han
        //            else if (_dueDateIndex == 2)
        //            {
        //                if (!string.IsNullOrEmpty(date_filter))
        //                {
        //                    count_query_vcxl = string.Format("SELECT Count(*) as count FROM BeanNotify WHERE {0} AND Status = 0 AND (DueDate IS NOT NULL AND DueDate >= ?)", date_filter);
        //                    query = string.Format("SELECT * FROM BeanNotify WHERE {0} AND Status = 0 AND (DueDate IS NOT NULL AND DueDate >= ?) Order By Created DESC LIMIT ? OFFSET ?", date_filter);
        //                }

        //                countnum_vcxl = conn.Query<CountNum>(count_query_vcxl, DateTime.Now.Date);
        //                lst_notify_cxl = conn.Query<BeanNotify>(query, DateTime.Now.Date, limit, offset);
        //            }
        //        }
        //        // Status - Da XL
        //        else if (_statusIndex == 2)
        //        {
        //            // Da XL - DueDate - Tatca
        //            if (_dueDateIndex == 0)
        //            {
        //                if (!string.IsNullOrEmpty(date_filter))
        //                {
        //                    count_query_vcxl = string.Format("SELECT Count(*) as count FROM BeanNotify WHERE {0} AND Status = 1", date_filter);
        //                    query = string.Format("SELECT * FROM BeanNotify WHERE {0} AND Status = 1 Order By Created DESC LIMIT ? OFFSET ?", date_filter);
        //                }

        //                countnum_vcxl = conn.Query<CountNum>(count_query_vcxl);
        //                lst_notify_cxl = conn.Query<BeanNotify>(query, limit, offset);
        //            }
        //            // Da XL - DueDate - Qua Han
        //            else if (_dueDateIndex == 1)
        //            {
        //                if (!string.IsNullOrEmpty(date_filter))
        //                {
        //                    count_query_vcxl = string.Format("SELECT Count(*) as count FROM BeanNotify WHERE {0} AND Status = 1 AND (DueDate IS NOT NULL AND DueDate < date('now'))", date_filter);
        //                    query = string.Format("SELECT * FROM BeanNotify WHERE {0} AND Status = 1 AND (DueDate IS NOT NULL AND DueDate < date('now')) Order By Created DESC LIMIT ? OFFSET ?", date_filter);
        //                }

        //                countnum_vcxl = conn.Query<CountNum>(count_query_vcxl);
        //                lst_notify_cxl = conn.Query<BeanNotify>(query, limit, offset);
        //            }
        //            // Da XL - DueDate - Trong han
        //            else if (_dueDateIndex == 2)
        //            {
        //                if (!string.IsNullOrEmpty(date_filter))
        //                {
        //                    count_query_vcxl = string.Format("SELECT Count(*) as count FROM BeanNotify WHERE {0} AND Status = 1 AND (DueDate IS NOT NULL AND DueDate >= ? )", date_filter);
        //                    query = string.Format("SELECT * FROM BeanNotify WHERE {0} AND Status = 1 AND (DueDate IS NOT NULL AND DueDate >= ?) Order By Created DESC LIMIT ? OFFSET ?", date_filter);
        //                }

        //                countnum_vcxl = conn.Query<CountNum>(count_query_vcxl, DateTime.Now.Date);
        //                lst_notify_cxl = conn.Query<BeanNotify>(query, DateTime.Now.Date, limit, offset);
        //            }
        //        }

        //        if (countnum_vcxl != null && countnum_vcxl[0].count > 0)
        //        {
        //            toMe_count = countnum_vcxl[0].count;
        //            string str_toMe = CmmFunction.GetTitle("TEXT_IPAD_TOME", "Trang chủ  >  Đến tôi");
        //            if (toMe_count >= 100)
        //                lbl_topBar_title.Text = str_toMe + " (99+)";
        //            else if (toMe_count > 0 && toMe_count < 100)
        //            {
        //                str_toMe = str_toMe + " (" + toMe_count.ToString() + ")";
        //                lbl_topBar_title.Text = str_toMe;
        //            }
        //            else
        //                lbl_topBar_title.Text = str_toMe;

        //            var str_transalte = lbl_topBar_title.Text;
        //            var indexA = str_transalte.IndexOf('(');
        //            if (indexA != -1)
        //            {
        //                NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);
        //                att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.White, new NSRange(0, str_transalte.Length));
        //                att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(255, 122, 58), new NSRange(indexA, str_transalte.Length - indexA));
        //                lbl_topBar_title.AttributedText = att;
        //            }
        //        }
        //        else
        //            lbl_topBar_title.Text = CmmFunction.GetTitle("TEXT_IPAD_TOME", "Trang chủ  >  Đến tôi");

        //        if (lst_notify_cxl != null && lst_notify_cxl.Count > 0)
        //        {
        //            if (currentItemSelected != null)
        //                currentItemSelected.IsSelected = true;
        //            else
        //                currentItemSelected = lst_notify_cxl[0];

        //            string KEY_TODAY = CmmFunction.GetTitle("TEXT_TODAY", "Hôm nay");
        //            string KEY_YESTERDAY = CmmFunction.GetTitle("TEXT_YESTERDAY", "Hôm qua");
        //            string KEY_ORTHER = CmmFunction.GetTitle("TEXT_OLDER", "Cũ hơn");

        //            List<BeanNotify> lst_temp1 = new List<BeanNotify>();
        //            List<BeanNotify> lst_temp2 = new List<BeanNotify>();
        //            List<BeanNotify> lst_temp3 = new List<BeanNotify>();
        //            dict_todo.Add(KEY_TODAY, lst_temp1);
        //            dict_todo.Add(KEY_YESTERDAY, lst_temp2);
        //            dict_todo.Add(KEY_ORTHER, lst_temp3);

        //            foreach (var item in lst_notify_cxl)
        //            {
        //                if (item.ID == currentItemSelected.ID)
        //                    item.IsSelected = true;
        //                else
        //                    item.IsSelected = false;

        //                if (item.StartDate.HasValue)
        //                {
        //                    if (item.StartDate.Value.Date == DateTime.Now.Date) // today
        //                    {
        //                        List<BeanNotify> lst_temp = new List<BeanNotify>();
        //                        if (dict_todo.ContainsKey(KEY_TODAY))
        //                            dict_todo[KEY_TODAY].Add(item);
        //                        else
        //                            dict_todo.Add(KEY_TODAY, lst_temp);
        //                    }
        //                    else if (item.StartDate.Value.Date == DateTime.Now.AddDays(-1).Date) // hom qua
        //                    {

        //                        List<BeanNotify> lst_temp = new List<BeanNotify>();
        //                        if (dict_todo.ContainsKey(KEY_YESTERDAY))
        //                            dict_todo[KEY_YESTERDAY].Add(item);
        //                        else
        //                            dict_todo.Add(KEY_YESTERDAY, lst_temp);
        //                    }
        //                    else if (item.StartDate.Value.Date < DateTime.Now.AddDays(-1).Date) // cuhon
        //                    {
        //                        List<BeanNotify> lst_temp = new List<BeanNotify>();
        //                        if (dict_todo.ContainsKey(KEY_ORTHER))
        //                            dict_todo[KEY_ORTHER].Add(item);
        //                        else
        //                            dict_todo.Add(KEY_ORTHER, lst_temp);
        //                    }
        //                }
        //            }

        //            table_todo.Hidden = false;
        //            lbl_nodata_left.Hidden = true;

        //            table_todo.Source = new ToDo_TableSource(dict_todo, this);
        //            table_todo.ReloadData();
        //        }
        //        else
        //        {
        //            table_todo.Hidden = true;
        //            lbl_nodata_left.Hidden = false;
        //            view_task_right.Hidden = true;
        //            lbl_nodata.Hidden = false;
        //        }

        //        if (!string.IsNullOrEmpty(searchKeyword))
        //        {
        //            tf_search.Text = searchKeyword;
        //            searchData();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        lbl_topBar_title.Text = CmmFunction.GetTitle("TEXT_IPAD_TOME", "Trang chủ  >  Đến tôi");
        //        table_todo.Source = null;
        //        table_todo.ReloadData();
        //        Console.WriteLine("TodoDetailsView - LoadDataFilterTodo - Err: " + ex.ToString());
        //    }
        //}
        private void hiddenPicker()
        {
            //if (view_picker_choice.Hidden == false)
            //{
            //    UIView.BeginAnimations("toogle_view_picker_choice_slideClose");
            //    UIView.SetAnimationDuration(0.4f);
            //    UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
            //    UIView.SetAnimationRepeatCount(0);
            //    UIView.SetAnimationRepeatAutoreverses(false);
            //    UIView.SetAnimationDelegate(this);
            //    view_picker_choice.Frame = new CGRect(view_picker_choice.Frame.X, this.View.Bounds.Height, view_picker_choice.Frame.Width, 220);
            //    view_picker_choice.Alpha = 0;
            //    view_background_effect.Alpha = 0;
            //    view_picker_choice.Hidden = true;
            //    UIView.CommitAnimations();
            //}

            //if (view_pickerDate.Hidden == false)
            //{
            //    UIView.BeginAnimations("toogle_view_pickerDate_slideHide");
            //    UIView.SetAnimationDuration(0.4f);
            //    UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
            //    UIView.SetAnimationRepeatCount(0);
            //    UIView.SetAnimationRepeatAutoreverses(false);
            //    UIView.SetAnimationDelegate(this);
            //    view_pickerDate.Frame = new CGRect(view_pickerDate.Frame.X, this.View.Bounds.Height, view_pickerDate.Frame.Width, 220);
            //    view_pickerDate.Alpha = 0;
            //    view_background_effect.Alpha = 0;
            //    view_pickerDate.Hidden = true;
            //    UIView.CommitAnimations();
            //}
        }

        /// <summary>
        /// Lấy danh sách người được phân công từ list thông tin luân chuyển
        /// </summary>
        /// <returns></returns>
        List<string> GetListAssignUsers()
        {
            var users = new List<string>();
            var assignUsers = lst_qtlc.FindAll(o => !o.Status).FirstOrDefault()?.ChildHistory.FindAll(o => !o.Status);
            if (assignUsers != null && assignUsers.Count > 0)
            {
                assignUsers.ForEach(o =>
                {
                    users.Add(o.AssignUserName);
                });
            }
            return users;
        }

        string strHeaderUserMore = "";
        /// <summary>
        /// Hiện các user được phân công nhưng chưa hoàn thành công việc
        /// </summary>
        void ShowAssignUsers()
        {
            if (string.IsNullOrEmpty(strHeaderUserMore))
            {
                var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                string query = string.Format("SELECT * FROM BeanAppStatus WHERE ID = '{0}' LIMIT 1 OFFSET 0", beanAppBaseExt.StatusGroup);
                List<BeanAppStatus> _lstAppStatus = conn.Query<BeanAppStatus>(query);

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
            }

            var lstUser = GetListAssignUsers();
            if (lstUser != null && lstUser.Count > 0)
            {
                string assignedToSample = string.Join(", ", lstUser);
                if (!isExpandUser)
                {
                    lbl_subTitle.LineBreakMode = UILineBreakMode.WordWrap;
                    lbl_subTitle.Text = strHeaderUserMore + assignedToSample;

                    var heightRequire = StringExtensions.StringHeight(lbl_subTitle.Text, UIFont.FromName("ArialMT", 12), lbl_subTitle.Frame.Width);
                    if (heightRequire > 20)
                    {
                        view_header_content_height_constant.Constant = origin_view_header_content_height_constant + 20;
                        lbl_subTitle.Lines = 2;
                    }
                    else if (heightRequire > 40)
                    {
                        view_header_content_height_constant.Constant = origin_view_header_content_height_constant + 40;
                        lbl_subTitle.Lines = 3;
                    }
                }
                else
                {
                    view_header_content_height_constant.Constant = origin_view_header_content_height_constant;

                    string res = string.Empty;

                    if (lstUser.Count > 1)
                    {
                        int num_remain = lstUser.Count - 1;
                        res = lstUser[0] + ", +" + num_remain.ToString();
                    }
                    else
                        res = lstUser[0];

                    if (!string.IsNullOrEmpty(res))
                        res = strHeaderUserMore + res;

                    lbl_subTitle.Text = res; // nguoi nhan
                }
                isExpandUser = !isExpandUser;
            }
            else
            {
                lbl_subTitle.Text = strHeaderUserMore + beanAppBaseExt.UserName;
            }
        }
        #endregion

        #region events
        private void BT_back_TouchUpInside(object sender, EventArgs e)
        {
            if (!isFromPush)
            {
                if (parentView != null)
                {
                    if (parentView is FollowListViewController)
                        (parentView as FollowListViewController).LoadData();
                    else if (parentView is KanBanView && isReloadKanBanView)
                        (parentView as KanBanView).reloadData();
                    else
                    {
                        var isBackWithoutEditing = isFollow == isFollowedDefault;
                        if (parentView is MainView)
                            (parentView as MainView).isBackWithoutEditing = isBackWithoutEditing;
                        else if (parentView is RequestListView)
                            (parentView as RequestListView).isBackWithoutEditing = isBackWithoutEditing;
                        else if (parentView is MyRequestListView)
                            (parentView as MyRequestListView).isBackWithoutEditing = isBackWithoutEditing;
                    }
                }

                if (this.NavigationController != null)
                    this.NavigationController.PopViewController(true);
                else
                    this.DismissViewControllerAsync(true);
            }
            else
                this.NavigationController.PopToRootViewController(true);
        }

        [Export("hideKeyboard")]
        private void hideKeyboard()
        {

            this.View.EndEditing(true);
            //if (table_actionmore.Alpha == 1)
            //{
            //    table_actionmore.Alpha = 1;
            //    UIView.BeginAnimations("toogle_docmenu_slideClose");
            //    UIView.SetAnimationDuration(0.4f);
            //    UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
            //    UIView.SetAnimationRepeatCount(0);
            //    UIView.SetAnimationRepeatAutoreverses(false);
            //    UIView.SetAnimationDelegate(this);
            //    table_actionmore.Frame = new CGRect(0, this.View.Frame.Height, this.View.Bounds.Width, 0);
            //    table_actionmore.Alpha = 0;
            //    UIView.CommitAnimations();
            //}
        }

        private void BT_attachement_TouchUpInside(object sender, EventArgs e)
        {
            if (lst_attachFile != null && lst_attachFile.Count > 0)
            {
                //PresentationDelegate transitioningDelegate;
                //CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
                //CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);

                AttachmentListView attachment = (AttachmentListView)Storyboard.InstantiateViewController("AttachmentListView");
                attachment.SetContent(lst_attachFile, lbl_code.Text, this, attachmentElement);

                //transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                //attachment.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;
                //attachment.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                //attachment.TransitioningDelegate = transitioningDelegate;
                //this.PresentViewControllerAsync(attachment, true);
                this.NavigationController.PushViewController(attachment, true);
            }
        }
        private void BT_share_TouchUpInside(object sender, EventArgs e)
        {
            try
            {
                //PresentationDelegate transitioningDelegate;
                //CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
                //CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);

                FormShareView formShareView = (FormShareView)Storyboard.InstantiateViewController("FormShareView");
                formShareView.SetContent(this, workflowItem, str_json_FormDefineInfo);
                //transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                //formShareView.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;
                //formShareView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                //formShareView.TransitioningDelegate = transitioningDelegate;
                //this.PresentViewControllerAsync(formShareView, true);

                this.NavigationController.PushViewController(formShareView, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("RequestDetails - BT_share - Err: " + ex.ToString());
            }
        }
        private void BT_progress_TouchUpInside(object sender, EventArgs e)
        {
            //PresentationDelegate transitioningDelegate;
            //CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            //CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);

            ProgressView progressView = (ProgressView)Storyboard.InstantiateViewController("ProgressView");
            progressView.SetContent(workflowItem, lbl_code.Text);
            //transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            //progressView.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;
            //progressView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            //progressView.TransitioningDelegate = transitioningDelegate;
            //this.PresentViewControllerAsync(progressView, true);

            this.NavigationController.PushViewController(progressView, true);
        }
        private void BT_moreaction_exit_TouchUpInside(object sender, EventArgs e)
        {
            menu_action_Toggle();
        }
        private void BT_Pickerchoice_Done_TouchUpInside(object sender, EventArgs e)
        {
            if (dic_singleChoiceSelected.Count > 0)
            {
                updateDictValue(dic_singleChoiceSelected.First().Key, dic_singleChoiceSelected.First().Value);
                ToggelPickerChoice();
            }
        }
        private void BT_moreUser_TouchUpInside(object sender, EventArgs e)
        {
            #region cách cũ
            //var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
            //string strHeaderUserMore = "";
            //string query = string.Format("SELECT * FROM BeanAppStatus WHERE ID = '{0}' LIMIT 1 OFFSET 0", beanAppBaseExt.StatusGroup);
            //List<BeanAppStatus> _lstAppStatus = conn.Query<BeanAppStatus>(query);

            //if (_lstAppStatus != null && _lstAppStatus.Count > 0)
            //{
            //    if (_lstAppStatus[0].ID == 8) // da phe duyet
            //        strHeaderUserMore = CmmFunction.GetTitle("TEXT_TITLE_APPROVED", "Phê duyệt: ");
            //    else if (_lstAppStatus[0].ID == 64) // da huy
            //        strHeaderUserMore = CmmFunction.GetTitle("TEXT_TITLE_CANCEL", "Hủy: ");
            //    else if (_lstAppStatus[0].ID == 16)
            //        strHeaderUserMore = CmmFunction.GetTitle("TEXT_TITLE_REJECTED", "Từ chối: ");
            //    else
            //        strHeaderUserMore = CmmFunction.GetTitle("TEXT_TO", "Đến: ");
            //}
            //else
            //    strHeaderUserMore = CmmFunction.GetTitle("TEXT_TO", "Đến: ");
            //#region
            ////if (!isExpandUser)
            ////{
            ////    lbl_subTitle.LineBreakMode = UILineBreakMode.WordWrap;
            ////    isExpandUser = true;
            ////    string combindedStringUser = string.Join(", ", lst_userName);
            ////    lbl_subTitle.Text = strHeaderUserMore + combindedStringUser;

            ////    var heightRequire = StringExtensions.StringHeight(lbl_subTitle.Text, UIFont.FromName("ArialMT", 12), lbl_subTitle.Frame.Width);
            ////    if (heightRequire > 20)
            ////    {
            ////        view_header_content_height_constant.Constant = origin_view_header_content_height_constant + 20;
            ////        lbl_subTitle.Lines = 2;
            ////    }

            ////    if (heightRequire > 40)
            ////    {
            ////        view_header_content_height_constant.Constant = origin_view_header_content_height_constant + 40;
            ////        lbl_subTitle.Lines = 3;
            ////    }
            ////}
            ////else
            ////{
            ////    isExpandUser = false;
            ////    view_header_content_height_constant.Constant = origin_view_header_content_height_constant;
            ////    string assignedToSample = string.Join(", ", lst_userName);
            ////    //var widthStatus = StringExtensions.MeasureString(workflowItem.AssignedToName, 12).Width + 20;

            ////    //var maxStatusWidth = (lbl_subTitle.Frame.Width / 3) * 2;

            ////    nfloat temp_width = 0;
            ////    if (!string.IsNullOrEmpty(assignedToSample))
            ////    {
            ////        string[] users = assignedToSample.Split(',');
            ////        string res = string.Empty;

            ////        if (users.Length > 1)
            ////        {
            ////            int num_remain = users.Length - 1;
            ////            res = users[0] + ", +" + num_remain.ToString();
            ////        }
            ////        else
            ////            res = users[0];

            ////        //for (int i = 0; i < users.Length; i++)
            ////        //{
            ////        //    var item_width = StringExtensions.MeasureString(users[i], 12).Width + 40;
            ////        //    if ((temp_width) < maxStatusWidth)
            ////        //    {
            ////        //        temp_width = temp_width + item_width;
            ////        //        res = res + users[i] + ",";
            ////        //        BT_moreUser.UserInteractionEnabled = false;
            ////        //    }
            ////        //    else
            ////        //    {
            ////        //        int num_remain = users.Length - i;
            ////        //        res = res + " +" + num_remain.ToString() + " người khác";
            ////        //        BT_moreUser.UserInteractionEnabled = true;
            ////        //        break;
            ////        //    }
            ////        //}

            ////        if (!string.IsNullOrEmpty(res))
            ////            res = strHeaderUserMore + res;

            ////        if (res.Contains('+'))
            ////        {
            ////            var indexA = res.IndexOf('+');
            ////            NSMutableAttributedString att = new NSMutableAttributedString(res);
            ////            att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(94, 94, 94), new NSRange(0, res.Length));
            ////            att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(65, 80, 134), new NSRange(indexA, res.Length - indexA));
            ////            att.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(lbl_subTitle.Font.PointSize, UIFontWeight.Semibold), new NSRange(indexA, res.Length - indexA));
            ////            lbl_subTitle.AttributedText = att;//(att, UIControlState.Normal);
            ////        }
            ////        else
            ////            lbl_subTitle.Text = res; // nguoi nhan
            ////    }
            ////}
            //#endregion
            //var lstUser = GetListAssignUsers();
            //if (lstUser != null && lstUser.Count > 0)
            //{
            //    string assignedToSample = string.Join(", ", lstUser);//lst_userName
            //    if (!isExpandUser)
            //    {
            //        lbl_subTitle.LineBreakMode = UILineBreakMode.WordWrap;
            //        lbl_subTitle.Text = strHeaderUserMore + assignedToSample;

            //        var heightRequire = StringExtensions.StringHeight(lbl_subTitle.Text, UIFont.FromName("ArialMT", 12), lbl_subTitle.Frame.Width);
            //        if (heightRequire > 20)
            //        {
            //            view_header_content_height_constant.Constant = origin_view_header_content_height_constant + 20;
            //            lbl_subTitle.Lines = 2;
            //        }
            //        else if (heightRequire > 40)
            //        {
            //            view_header_content_height_constant.Constant = origin_view_header_content_height_constant + 40;
            //            lbl_subTitle.Lines = 3;
            //        }
            //    }
            //    else
            //    {
            //        view_header_content_height_constant.Constant = origin_view_header_content_height_constant;

            //        if (!string.IsNullOrEmpty(assignedToSample))
            //        {
            //            string res = string.Empty;

            //            if (lstUser.Count > 1)
            //            {
            //                int num_remain = lstUser.Count - 1;
            //                res = lstUser[0] + ", +" + num_remain.ToString();
            //            }
            //            else
            //                res = lstUser[0];

            //            if (!string.IsNullOrEmpty(res))
            //                res = strHeaderUserMore + res;

            //            lbl_subTitle.Text = res; // nguoi nhan
            //        }
            //    }
            //    isExpandUser = !isExpandUser;
            //}
            //else
            //{
            //    lbl_subTitle.Text = strHeaderUserMore + beanAppBaseExt.UserName;
            //}
            #endregion
            ShowAssignUsers();
        }
        private void BT_code_fulltext_TouchUpInside(object sender, EventArgs e)
        {
            FullTextView fullTextView = (FullTextView)Storyboard.InstantiateViewController("FullTextView");
            fullTextView.SetContent("", lbl_code.Text);
            this.NavigationController.PushViewController(fullTextView, true);
        }
        private void BT_comment_TouchUpInside(object sender, EventArgs e)
        {
            try
            {
                CloseAddFollow();
                table_content.ScrollToRow(NSIndexPath.FromRowSection(lst_section[0].ViewRows.Count - 1, 0), UITableViewScrollPosition.Top, true);

                //CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
                //CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
                //FormCommentView formComment = (FormCommentView)Storyboard.InstantiateViewController("FormCommentView");
                //formComment.SetContent(this, workflowItem, str_json_FormDefineInfo, lst_comments, _OtherResourceId);
                //PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                //formComment.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                //formComment.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                //formComment.TransitioningDelegate = transitioningDelegate;
                //this.PresentViewControllerAsync(formComment, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("RequestDetailsV2 - BT_comment_TouchUpInside - Err: " + ex.ToString());
            }

        }
        private void BT_star_TouchUpInside(object sender, EventArgs e)
        {
            Custom_AddFollowView custom_AddFollowView = Custom_AddFollowView.Instance;
            if (custom_AddFollowView.Superview != null && custom_AddFollowView.viewController.GetType() == typeof(RequestDetailsV2))
                custom_AddFollowView.RemoveFromSuperview();
            else
            {
                if (isFollow)
                {
                    var width = StringExtensions.MeasureString("Hủy theo dõi công việc này", 14).Width + 20 + 50;
                    custom_AddFollowView.viewController = this;
                    custom_AddFollowView.isFollow = isFollow;
                    custom_AddFollowView.LoadContent();
                    custom_AddFollowView.InitFrameView(new CGRect(UIScreen.MainScreen.Bounds.Width - width - 15, top_view.Frame.Bottom + 1, width, 56));

                    View.AddSubview(custom_AddFollowView);
                    View.BringSubviewToFront(custom_AddFollowView);
                }
                else
                {
                    var width = StringExtensions.MeasureString("Đặt theo dõi công việc này", 14).Width + 20 + 50;
                    custom_AddFollowView.viewController = this;
                    custom_AddFollowView.isFollow = isFollow;
                    custom_AddFollowView.LoadContent();
                    custom_AddFollowView.InitFrameView(new CGRect(UIScreen.MainScreen.Bounds.Width - width - 15, top_view.Frame.Bottom + 1, width, 56));

                    View.AddSubview(custom_AddFollowView);
                    View.BringSubviewToFront(custom_AddFollowView);
                }
            }
        }

        //private async void RefreshControl_ValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        refreshControl.BeginRefreshing();
        //        ProviderBase provider = new ProviderBase();
        //        ProviderUser p_user = new ProviderUser();

        //        string documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        //        string localpath = Path.Combine(documentFolder, CmmVariable.M_Avatar);

        //        await Task.Run(() =>
        //        {
        //            provider.UpdateAllMasterData(true);
        //            provider.UpdateAllDynamicData(true);
        //            p_user.UpdateCurrentUserInfo(localpath);
        //            LoadData();
        //            InvokeOnMainThread(() =>
        //            {
        //                refreshControl.EndRefreshing();
        //            });
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        refreshControl.EndRefreshing();
        //        Console.WriteLine("Error - MainView - refreshControl_valuechange - Er: " + ex.ToString());
        //    }
        //}
        //private void CmmEvent_UpdateLangComplete(object sender, CmmEvent.UpdateEventArgs e)
        //{
        //    try
        //    {
        //        InvokeOnMainThread(() =>
        //        {
        //            SetLangTitle();
        //            table_content.ReloadData();
        //            refreshControl.AttributedTitle = new NSAttributedString(CmmFunction.GetTitle("TEXT_REFRESH_LOADING", "Loading ..."));
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("MainView - CmmEvent_UpdateLangComplete - ERR: " + ex.ToString());
        //    }
        //}
        #endregion

        #region custom class

        #region dynamic controls source table
        private class Control_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cell");
            RequestDetailsV2 parentView;
            List<ViewSection> lst_section;
            Dictionary<string, List<ViewRow>> dict_control = new Dictionary<string, List<ViewRow>>();
            Dictionary<int, Dictionary<int, float>> heightOfItem = new Dictionary<int, Dictionary<int, float>>();
            List<BeanWorkFlowRelated> lstWorkFlowRelated;
            List<BeanTaskDetail> lst_tasks;
            List<BeanComment> lst_comment;
            //int numRowLast = 0;
            //int heightHeader = 50;

            public Control_TableSource(List<ViewSection> _lst_section, List<BeanWorkFlowRelated> _lstWorkFlowRelated, List<BeanTaskDetail> _lst_tasks, List<BeanComment> _lst_comment, RequestDetailsV2 _parentview, bool _isSaved)
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

                #region Related
                if (_lstWorkFlowRelated != null)
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
                #endregion
                #region Tasks
                if (_lst_tasks != null && _lst_tasks.Count > 0)
                {
                    lst_tasks = _lst_tasks;
                    var dataSource = JsonConvert.SerializeObject(lst_tasks);

                    ViewElement element = new ViewElement();
                    element.Title = CmmFunction.GetTitle("TEXT_TASKLIST", "Danh sách công việc ");
                    element.DataSource = dataSource;
                    element.Value = dataSource;
                    element.DataType = "inputtasks";

                    //List<ViewRow> lst_viewRow = new List<ViewRow>();
                    ViewRow rowTask = new ViewRow();

                    List<ViewElement> lst_element = new List<ViewElement>();
                    lst_element.Add(element);
                    rowTask.Elements = lst_element;

                    //lst_viewRow.Add(rowTask);
                    _lst_section[0].ViewRows.Add(rowTask);// = lst_viewRow;
                }
                #endregion
                #region comment
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

                    //List<ViewRow> lst_viewRow = new List<ViewRow>();
                    ViewRow rowComment = new ViewRow();

                    List<ViewElement> lst_element = new List<ViewElement>();
                    lst_element.Add(element);
                    rowComment.Elements = lst_element;

                    //lst_viewRow.Add(rowComment);
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

                    //List<ViewRow> lst_viewRow = new List<ViewRow>();
                    ViewRow rowComment = new ViewRow();

                    List<ViewElement> lst_element = new List<ViewElement>();
                    lst_element.Add(element);
                    rowComment.Elements = lst_element;

                    //lst_viewRow.Add(rowComment);
                    _lst_section[0].ViewRows.Add(rowComment);
                }
                #endregion

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
                return 0;
            }

            public override nfloat GetHeightForFooter(UITableView tableView, nint section)
            {
                return -1;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                nfloat maxHeight = 0;
                nfloat widthTemp = 0;
                var data = dict_control[lst_section[indexPath.Section].ID][indexPath.Row];
                var items = data.Elements;

                if (data.RowType == 1)
                    widthTemp = tableView.Frame.Width;
                else if (data.RowType == 2)
                    widthTemp = tableView.Frame.Width / 2 - 5;
                else if (data.RowType == 3)
                    widthTemp = (tableView.Frame.Width - 20) / 3;

                foreach (var item in items)
                {
                    nfloat heightTemp = 0;
                    switch (item.DataType)
                    {
                        case "inputattachmenthorizon":
                            {
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    int sectionHeightTotal = 0;
                                    List<BeanAttachFile> lst_attach = new List<BeanAttachFile>();
                                    JArray json = JArray.Parse(item.Value);
                                    lst_attach = json.ToObject<List<BeanAttachFile>>();
                                    parentView.lst_attachFile = lst_attach;

                                    if (lst_attach.Count > 0)
                                    {
                                        List<string> sectionKeys;
                                        lst_attach.Where(i => i.AttachTypeName == null).ToList().ForEach(u => u.AttachTypeName = "");
                                        sectionKeys = lst_attach.Select(x => x.AttachTypeName).Distinct().ToList();

                                        if (sectionKeys != null && sectionKeys.Count > 0)
                                            sectionHeightTotal = sectionKeys.Count * 44;

                                        heightTemp = (lst_attach.Count * 60) + 75 + sectionHeightTotal;//header height: 75 - cell row height: 60 - padding top của table : 10
                                    }
                                    else
                                        heightTemp = 81;
                                }
                                else
                                    heightTemp = 81;
                                heightTemp += 20;//padding bottom
                            }
                            break;
                        case "textinputmultiline":
                            {
                                string value = CmmFunction.StripHTML(item.Value);
                                var height_ets = StringExtensions.StringRect(value, UIFont.SystemFontOfSize(14, UIFontWeight.Regular), widthTemp);
                                if (height_ets.Height < 50)
                                {
                                    if (height_ets.Height > 30)
                                        heightTemp = 95;
                                    else
                                        heightTemp = 70;
                                }
                                else
                                    heightTemp = 115;
                            }
                            break;
                        case "textinput":
                            {
                                string value = CmmFunction.StripHTML(item.Value);
                                var height_ets = StringExtensions.StringRect(value, UIFont.SystemFontOfSize(14, UIFontWeight.Regular), widthTemp);
                                if (height_ets.Height < 200)
                                {
                                    if (height_ets.Height > 25)
                                        heightTemp = (height_ets.Height) + 25;
                                    else
                                        heightTemp = 70;
                                }
                                else
                                    heightTemp = 140;
                            }
                            break;
                        case "textinputformat":
                            {
                                var height_ets = StringExtensions.StringRect(item.Value, UIFont.SystemFontOfSize(14, UIFontWeight.Regular), widthTemp);
                                if (height_ets.Height < 200)
                                    heightTemp = 100 + 25;
                                else
                                    heightTemp = height_ets.Height + 25;
                            }
                            break;
                        case "inputworkrelated":
                            {
                                var tableHeight = lstWorkFlowRelated.Count * 100;
                                heightTemp = tableHeight + 30;
                                heightTemp += 20;//padding bottom
                            }
                            break;
                        case "inputgriddetails":
                            {
                                nfloat height = 80;
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

                                        foreach (var itemHeader in lst_titleHeader)
                                        {
                                            //if (item.internalName == "TongTien")
                                            //    item.isSum = true;

                                            if (itemHeader.isSum)
                                            {
                                                itemHeader.isSum = true;
                                                height_expand = 50;
                                            }
                                        }
                                        height = height + (lst_jobject.Count * 50);

                                    }
                                    else
                                        height = height + (lst_jobject.Count * 50);

                                    heightTemp = height + height_expand;
                                }
                                else
                                    heightTemp = 80;
                                heightTemp += 20;//padding bottom
                            }
                            break;
                        case "inputtasks":
                            {
                                var lst_parent = lst_tasks.Where(i => i.Parent == 0).ToList();
                                rowNum = 0;
                                foreach (var parent in lst_parent)
                                {
                                    rowNum++;
                                    if (parent.isExpand)
                                    {
                                        LoadCountSubTask(parent);
                                    }
                                }
                                heightTemp = (rowNum * 90) + 40;
                                heightTemp += 20;//padding bottom
                            }
                            break;
                        case "inputcomments":
                            {
                                nfloat basicHeight = 160;
                                nfloat height = 0;
                                bool isFirtIMG = true;
                                //notes => add comment, dinh kem comment 
                                if (item.Notes != null && item.Notes.Count > 0)
                                {
                                    foreach (var note in item.Notes)
                                    {
                                        if (note.Key == "image" && isFirtIMG)
                                        {
                                            height = height + 55;
                                            isFirtIMG = !isFirtIMG;
                                        }
                                        else if (note.Key == "doc")
                                        {
                                            JArray json = JArray.Parse(note.Value);
                                            var lst_addAttachment = json.ToObject<List<BeanAttachFile>>();
                                            if (lst_addAttachment != null && lst_addAttachment.Count > 0)
                                            {
                                                height = height + (lst_addAttachment.Count() * 35);
                                            }
                                        }
                                    }
                                    height = height + basicHeight;
                                }
                                else
                                    height = basicHeight;
                                ///Dùng value ko cần dùng DataSource
                                //if (!string.IsNullOrEmpty(item.DataSource) || item.DataSource != "[]")
                                //{

                                //}

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
                                                    height = height + 110;
                                                }
                                                else
                                                {
                                                    newSortList.Insert(newSortList.Count, attach);
                                                    height = height + 35;
                                                }
                                            }
                                        }
                                        // comment khong co dinh kem
                                        //height = height + 100;
                                        nfloat heightText = 0;
                                        nfloat width = UIScreen.MainScreen.Bounds.Width;
                                        if (!string.IsNullOrEmpty(comment.Content))
                                        {
                                            CGRect rect;
                                            if (string.IsNullOrEmpty(comment.ParentCommentId))
                                                rect = StringExtensions.StringRect(comment.Content, UIFont.FromName("ArialMT", 14f), width - 132);
                                            else
                                                rect = StringExtensions.StringRect(comment.Content, UIFont.FromName("ArialMT", 14f), width - 187);// 210 - padding tu view vao item conten

                                            if (rect.Height > 0 && rect.Height < 20)
                                                rect.Height = 30;
                                            heightText = rect.Height + 50;
                                        }
                                        else
                                            heightText = 80;
                                        height = height + heightText + 20;
                                    }
                                }

                                parentView.estCommmentViewRowHeight = height;
                                heightTemp = height;
                            }
                            break;
                        default:
                            heightTemp = 70;
                            break;
                    }
                    Dictionary<int, nfloat> heightInItem = new Dictionary<int, nfloat>();
                    if (heightTemp > maxHeight)
                        maxHeight = heightTemp;
                }
                // var item = dict_control[lst_section[indexPath.Section].ID][indexPath.Row].Elements[0];
                return maxHeight;

            }
            int rowNum;
            private void LoadCountSubTask(BeanTaskDetail parent_task)
            {
                if (parent_task.children != null && parent_task.children.Count > 0)
                {
                    foreach (var i2 in parent_task.children)
                    {
                        rowNum++;
                        var lv2 = lst_tasks.Where(i => i.Parent == i2.ID).ToList();
                        if (i2.isExpand)
                        {
                            LoadCountSubTask(i2);
                        }
                    }
                }
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return lst_section.Count;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                var sectionItem = lst_section[Convert.ToInt32(section)];
                var lst_row = dict_control[sectionItem.ID];
                return lst_row.Count;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                tableView.DeselectRow(indexPath, true);
            }

            public override bool ShouldHighlightRow(UITableView tableView, NSIndexPath rowIndexPath)
            {
                return false;
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                //string dataSource = JsonConvert.SerializeObject(lstWorkFlowRelated);
                //ViewElement element = new ViewElement();
                //element.DataSource = dataSource;
                //element.Value = dataSource;
                //element.DataType = "inputworkrelated";
                //control.Elements.Add(element);

                var control = dict_control[lst_section[indexPath.Section].ID][indexPath.Row];
                Control_cell_custom cell = new Control_cell_custom(parentView, cellIdentifier, control, indexPath, lstWorkFlowRelated);
                return cell;
            }
        }

        private class Control_cell_custom : UITableViewCell
        {
            RequestDetailsV2 parentView { get; set; }
            ViewRow control { get; set; }
            NSIndexPath currentIndexPath { get; set; }
            public ComponentBase components;

            UIView view_workflowRelate;
            UITableView table_workflowRelate { get; set; }
            UILabel lbl_workflowRelate_title, lbl_line;
            List<BeanWorkFlowRelated> lstWorkFlowRelated { get; set; }

            public Control_cell_custom(RequestDetailsV2 _parentView, NSString cellID, ViewRow _control, NSIndexPath _currentIndexPath, List<BeanWorkFlowRelated> _lstWorkFlowRelated) : base(UITableViewCellStyle.Default, cellID)
            {
                parentView = _parentView;
                control = _control;
                currentIndexPath = _currentIndexPath;
                Accessory = UITableViewCellAccessory.None;
                lstWorkFlowRelated = _lstWorkFlowRelated;
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
                    Console.WriteLine("RequestDetailsV2 - Control_cell_custom - loaddata- ERR: " + ex.ToString());
                }
            }

            public void loadWorkFlowRelate()
            {
                if (lstWorkFlowRelated != null)
                {
                    lbl_line.HeightAnchor.ConstraintEqualTo(1).Active = true;
                    NSLayoutConstraint.Create(lbl_line, NSLayoutAttribute.Top, NSLayoutRelation.Equal, view_workflowRelate, NSLayoutAttribute.Top, 1.0f, 1).Active = true;
                    NSLayoutConstraint.Create(lbl_line, NSLayoutAttribute.Left, NSLayoutRelation.Equal, view_workflowRelate, NSLayoutAttribute.Left, 1, 0).Active = true;
                    NSLayoutConstraint.Create(lbl_line, NSLayoutAttribute.Right, NSLayoutRelation.Equal, view_workflowRelate, NSLayoutAttribute.Right, 1, 0).Active = true;

                    lbl_workflowRelate_title.HeightAnchor.ConstraintEqualTo(25).Active = true;
                    NSLayoutConstraint.Create(lbl_workflowRelate_title, NSLayoutAttribute.Top, NSLayoutRelation.Equal, view_workflowRelate, NSLayoutAttribute.Top, 1.0f, 15).Active = true;
                    NSLayoutConstraint.Create(lbl_workflowRelate_title, NSLayoutAttribute.Left, NSLayoutRelation.Equal, view_workflowRelate, NSLayoutAttribute.Left, 1.0f, 15).Active = true;
                    NSLayoutConstraint.Create(lbl_workflowRelate_title, NSLayoutAttribute.Right, NSLayoutRelation.Equal, view_workflowRelate, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;

                    table_workflowRelate.HeightAnchor.ConstraintEqualTo(lstWorkFlowRelated.Count * 100).Active = true;
                    NSLayoutConstraint.Create(table_workflowRelate, NSLayoutAttribute.Top, NSLayoutRelation.Equal, lbl_workflowRelate_title, NSLayoutAttribute.Bottom, 1.0f, 10).Active = true;
                    NSLayoutConstraint.Create(table_workflowRelate, NSLayoutAttribute.Left, NSLayoutRelation.Equal, view_workflowRelate, NSLayoutAttribute.Left, 1.0f, 5).Active = true;
                    NSLayoutConstraint.Create(table_workflowRelate, NSLayoutAttribute.Right, NSLayoutRelation.Equal, view_workflowRelate, NSLayoutAttribute.Right, 1.0f, 5).Active = true;

                    table_workflowRelate.Source = new WorkFlowRelate_TableSource(lstWorkFlowRelated, parentView);
                    view_workflowRelate.UserInteractionEnabled = true;
                    table_workflowRelate.UserInteractionEnabled = true;
                }
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                components.InitializeFrameView(new CGRect(15, 0, ContentView.Frame.Width - 30, ContentView.Frame.Height));
            }
        }

        #endregion

        #region WorkFlowItem data source table
        private class WorkFlowRelate_TableSource : UITableViewSource
        {
            List<BeanWorkFlowRelated> lst_workflowRelate;
            NSString cellIdentifier = new NSString("cell");
            RequestDetailsV2 parentView;

            public WorkFlowRelate_TableSource(List<BeanWorkFlowRelated> _lst_workflowRelate, RequestDetailsV2 _parentview)
            {
                lst_workflowRelate = _lst_workflowRelate;
                parentView = _parentview;
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return 1;
            }

            public override nfloat GetHeightForFooter(UITableView tableView, nint section)
            {
                return -1;
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                return -1;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return lst_workflowRelate.Count;
            }
            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 100;
            }
            public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
            {
                if (lst_workflowRelate.Count > 0)
                {
                    var item = lst_workflowRelate[indexPath.Row];
                    parentView.HandleRemoveWorkFlowRelate(item, indexPath);
                }
            }

            public override string TitleForDeleteConfirmation(UITableView tableView, NSIndexPath indexPath)
            {
                return "Xoá";
            }

            public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
            {
                return true;
            }
            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                //parentView.RelateWorkFlowSelected(lst_noti[indexPath.Row], indexPath);
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var workFlowRelated = lst_workflowRelate[indexPath.Row];
                bool isOdd = true;
                if (indexPath.Row % 2 == 0)
                    isOdd = false;

                Custom_WorkRelatedCell cell = new Custom_WorkRelatedCell(cellIdentifier);
                cell.UpdateCell(workFlowRelated, false, isOdd, "");
                return cell;
            }
        }
        #endregion

        #region action menu
        private class MenuAction_TableSource : UITableViewSource
        {
            static readonly NSString cellIdentifier = new NSString("Moreaction_cell");
            List<ButtonAction> items;
            RequestDetailsV2 parentview;

            public MenuAction_TableSource(List<ButtonAction> lst_items, RequestDetailsV2 controler)
            {
                parentview = controler;
                items = lst_items;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return items.Count;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 60;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                parentview.ActionSelected(items[indexPath.Row]);
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                Moreaction_cell cell = tableView.DequeueReusableCell(cellIdentifier, indexPath) as Moreaction_cell;
                if (indexPath.Row == items.Count - 1)
                    cell.UpdateCell(items[indexPath.Row], true);
                else
                    cell.UpdateCell(items[indexPath.Row], false);

                return cell;

            }
        }

        #endregion

        #region picker type
        //private class SingleChoice_PickerSource : UIPickerViewModel
        //{
        //    ViewRequestDetails parentview;
        //    BeanControlDynamicDetail control;
        //    List<ClassDynamicControl> lst_controlValue;
        //    List<KeyValuePair<string, string>> lst_level;

        //    public SingleChoice_PickerSource(BeanControlDynamicDetail _control, List<ClassDynamicControl> _lst_controlvalue, ViewRequestDetails _parentview)
        //    {
        //        control = _control;
        //        parentview = _parentview;
        //        lst_controlValue = _lst_controlvalue;
        //    }

        //    public override nint GetComponentCount(UIPickerView pickerView)
        //    {
        //        return 1;
        //    }

        //    public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        //    {
        //        return lst_controlValue.Count;
        //    }

        //    public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
        //    {
        //        UILabel lbl_title = new UILabel();
        //        lbl_title.Font = UIFont.SystemFontOfSize(13, UIFontWeight.Semibold);
        //        lbl_title.TextColor = UIColor.DarkGray;

        //        if (lst_controlValue != null)
        //            lbl_title.Text = lst_controlValue[(int)row].Title;

        //        lbl_title.TextAlignment = UITextAlignment.Center;
        //        return lbl_title;
        //    }

        //    public override void Selected(UIPickerView picker, nint row, nint component)
        //    {
        //        var selectedIndex = lst_controlValue[(int)row];
        //        parentview.dic_singleChoiceSelected.Clear();
        //        parentview.dic_singleChoiceSelected.Add(control.DataField, selectedIndex.ID);

        //    }
        //}
        #endregion

        #region class multichoice
        //private class ClassMultichoice
        //{
        //    public string ID { get; set; }
        //    public string Title { get; set; }
        //}
        #endregion

        #endregion
    }
}


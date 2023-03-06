
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
using CoreAnimation;
using CoreGraphics;
using Foundation;
using MobileCoreServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TEditor;
using TEditor.Abstractions;
using UIKit;
using static BPMOPMobileV1.iOS.IOSClass.CmmIOSFunction;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class FormTaskDetails : UIViewController, EditTorInterFace
    {
        private NSObject _willResignActiveNotificationObserver;
        private NSObject _didBecomeActiveNotificationObserver;
        private nfloat positionBotOfCurrentViewInput { get; set; }
        BeanTask task { get; set; }
        BeanTask TaskResult { get; set; }
        BeanWorkflowItem beanWorkflowItem { get; set; }
        List<BeanUserAndGroup> lst_currentUserAndGroup { get; set; }
        List<BeanAttachFile> lst_currentAttach { get; set; }
        List<BeanAttachFile> lst_addAttachFile = new List<BeanAttachFile>();
        List<BeanAttachFile> lst_removeAttachFile = new List<BeanAttachFile>();
        List<BeanTaskDetail> lst_childTask { get; set; }
        JObject detailsForm;
        UIViewController parent { get; set; }
        Custom_AttachFileView attachFileView { get; set; }
        List<BeanUser> lst_selectedUser = new List<BeanUser>();
        CmmLoading loading;
        int _flagUserPermission;
        int currentIndexDetails; // 0: details - 1: subtasks
        UIImagePickerController imagePicker;
        UIDocumentPickerViewController docPicker;
        int numRowAttachmentFile = 0;
        public List<ClassMenu> lst_menuItem = new List<ClassMenu>();
        ClassMenu currentStatusItem { get; set; }
        //UIImageView iv_rightUser = new UIImageView(); // user
        //UIImageView iv_right = new UIImageView(); // calendar

        //UIView view_header;
        //UILabel lbl_header_title, lbl_nguoiXL, lbl_hanXL, lbl_trangthai, lbl_line;

        public string hintDefault = CmmFunction.GetTitle("TEXT_HINT_REQUIRE_COMMENT", "Vui lòng nhập bình luận tại đây..."); // Câp nhật lại giá trị cho text_note khi hoàn thành đính kèm file
        public nfloat estCommmentViewRowHeight;
        public List<BeanAttachFile> lst_addCommentAttachment;
        public string OtherResourceId = "";
        public BeanWorkflowItem workflowItem;
        BeanAppBase beanAppBase;

        // lst_comment
        public List<BeanComment> lst_comment;
        nfloat heightComment;

        ViewElement viewElement = new ViewElement();
        UITableView table_comment;
        UILabel lbl_line;

        string contentHtmlString = "";
        TEditorViewController tvc;
        public bool isShowTEditor = false;
        bool EditTorInterFace.IsShowTEditor { get { return isShowTEditor; } set { isShowTEditor = value; } }

        public FormTaskDetails(IntPtr handle) : base(handle)
        {
            _flagUserPermission = (int)FlagUserPermission.Viewer;
        }

        #region override

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            _willResignActiveNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, KeyBoardUpNotification);
            _didBecomeActiveNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyBoardDownNotification);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            if (_willResignActiveNotificationObserver != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(_willResignActiveNotificationObserver);

            if (_didBecomeActiveNotificationObserver != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(_didBecomeActiveNotificationObserver);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            isFirst = false;
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            scrollview_details.Frame = new CGRect(0, view_chosse.Frame.Bottom, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height - view_chosse.Frame.Bottom);
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            //BorderView();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            UITapGestureRecognizer gesture = new UITapGestureRecognizer(() =>
            {
                View.EndEditing(true);
            });

            gesture.ShouldReceiveTouch += delegate (UIGestureRecognizer recognizer, UITouch touch)
            {
                var touchView = touch.View.Class.Name;
                if (touchView == "UITextField" || touchView == "UITextView")
                    positionBotOfCurrentViewInput = GetPositionBotView(touch.View);
                else
                    positionBotOfCurrentViewInput = 0.0f;

                return true;
            };

            gesture.CancelsTouchesInView = false;


            if (parent != null)
            {
                if (parent is MainView)
                    (parent as MainView).doReload = true;
                else if (parent is RequestListView)
                    (parent as RequestListView).doReload = true;
                else if (parent is MyRequestListView)
                    (parent as MyRequestListView).doReload = true;
                //else if (parentView is FollowListViewController)
                //   (parentView as FollowListViewController).doReload = true;
            }

            ViewConfiguration();

            //kiem tra co quyen xem cong viec hay khong
            CheckPermit(beanAppBase);

            View.AddGestureRecognizer(gesture);
            setlangTitle();

            #region delegate
            BT_close.TouchUpInside += BT_close_TouchUpInside;
            txt_note.Started += Txt_Note_Started;
            txt_note.Ended += Txt_Note_Ended;
            BT_note.TouchUpInside += BT_note_TouchUpInside;
            BT_date.TouchUpInside += BT_startDate_TouchUpInside;
            BT_user.TouchUpInside += BT_user_TouchUpInside;
            BT_moreUser.TouchUpInside += BT_moreUser_TouchUpInside;
            BT_details.TouchUpInside += BT_details_TouchUpInside;
            BT_subtask.TouchUpInside += BT_subtask_TouchUpInside;
            BT_add.TouchUpInside += BT_add_TouchUpInside;
            BT_assign.TouchUpInside += BT_assign_TouchUpInside;
            BT_moreTitle.TouchUpInside += BT_moreTitle_TouchUpInside;
            BT_addAttachment.TouchUpInside += BT_addAttachment_TouchUpInside;
            BT_status.TouchUpInside += BT_status_TouchUpInside;
            BT_approve.TouchUpInside += BT_approve_TouchUpInside;
            BT_delete.TouchUpInside += BT_delete_TouchUpInside;
            BT_complete.TouchUpInside += BT_complete_TouchUpInside;
            BT_contentMore.TouchUpInside += BT_contentMore_TouchUpInside;
            #endregion
        }

        #endregion

        #region public - private method
        public void SetContent(BeanTask _task, BeanWorkflowItem _workflowItem, UIViewController _parent)
        {
            task = _task;
            beanWorkflowItem = _workflowItem;
            parent = _parent;
        }

        public void SetContent(BeanAppBase _beanAppBase, BeanWorkflowItem _workflowItem, UIViewController _parent)
        {
            beanWorkflowItem = _workflowItem;
            parent = _parent;
            beanAppBase = _beanAppBase;
        }
        private async void CheckPermit(BeanAppBase _beanAppBase)
        {
            try
            {
                bool result = true;
                loading = new CmmLoading(new CGRect((this.View.Bounds.Width - 200) / 2, (this.View.Bounds.Height - 200) / 2, 200, 200), "Đang xử lý...");
                this.View.Add(loading);

                await Task.Run(() =>
                {
                    if (parent.GetType() == typeof(MainView)
                    || parent.GetType() == typeof(RequestListView)
                    || parent.GetType() == typeof(MyRequestListView)
                    //app
                    //|| parent.GetType() == typeof(MainViewApp)
                    //|| parent.GetType() == typeof(RequestListViewApp)
                    //|| parent.GetType() == typeof(MyRequestListViewApp)
                    )
                    {
                        ProviderControlDynamic p_controlDynamic = new ProviderControlDynamic();
                        string Json_FormDataString = p_controlDynamic.GetTicketRequestControlDynamicForm(beanWorkflowItem);
                        if (!string.IsNullOrEmpty(Json_FormDataString))
                        {
                            JObject retValue = JObject.Parse(Json_FormDataString);
                            //danh sach cong viec phan cong
                            JArray json_tasks = JArray.Parse(retValue["task"].ToString());
                            List<BeanTaskDetail> lst_tasks = json_tasks.ToObject<List<BeanTaskDetail>>();
                            if (lst_tasks != null && lst_tasks.Count > 0)
                            {
                                lst_tasks = Custom_TableTaskHelper.Instance().getSortedNodes(lst_tasks[0].Parent, lst_tasks, true);
                                task = lst_tasks.Find(s => s.ID == _beanAppBase.ID);

                                if (task == null)
                                    result = false;
                            }
                            else
                                result = false;
                        }
                        else
                            result = false;
                    }
                    else
                        result = true;
                    //else if (parent.GetType() == typeof(RequestDetailsV2) || parent.GetType() == typeof(MyRequestListView) || parent.GetType() == typeof(FormTaskDetails))
                    //    result = true;
                    //else
                    //    result = false;


                    InvokeOnMainThread(() =>
                    {
                        loading.Hide();
                        if (result) // co quyen xem, thao tac tren cong viec
                        {
                            //LoadContent();
                            LoadTaskDetails();
                        }
                        else// khogn co quyen xem, thao tac cong viec
                        {
                            UIAlertController alert = UIAlertController.Create("Thông báo", CmmFunction.GetTitle("MESS_TASK_NOTFOUND", "Không tìm thấy thông tin công việc, vui lòng thử lại sau!"), UIAlertControllerStyle.Alert);//"BPM"
                            alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_CLOSE", "Đóng"), UIAlertActionStyle.Default, alertAction =>
                            {
                                if (this.NavigationController != null)
                                    this.NavigationController.PopViewController(true);
                                else
                                    this.DismissViewControllerAsync(true);
                            }));
                            this.PresentViewController(alert, true, null);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                loading.Hide();
                Console.WriteLine("FormTaskDetails - CheckPermit - loaddata- ERR: " + ex.ToString());
            }
        }

        #region lst_comment

        private bool isFirst = true;
        public void LoadConfigurationComment()
        {
            try
            {

                //comment
                if (lst_comment != null && lst_comment.Count > 0)
                {
                    var dataSource = JsonConvert.SerializeObject(lst_comment);

                    viewElement.Title = CmmFunction.GetTitle("TEXT_COMMENT", "Bình luận");
                    viewElement.DataSource = dataSource;
                    viewElement.Value = dataSource;
                    //viewElement.DataType = "inputcomments";
                    //viewElement.Notes = null;
                }
                else
                {
                    viewElement.Title = CmmFunction.GetTitle("TEXT_COMMENT", "Bình luận");
                    viewElement.DataSource = null;
                    viewElement.Value = null;
                }
                viewElement.DataType = "inputcomments";
                viewElement.Notes = null;
                viewElement.Enable = true;

                heightComment = HeightComment();
                table_comment.Source = new Control_TableSource(viewElement, this);
                table_comment.ReloadData();
            }
            catch (Exception ex)
            {
                Console.WriteLine("FormTaskDetails - LoadConfigurationComment - loaddata- ERR: " + ex.ToString());
            }
        }

        public nfloat HeightComment()
        {
            nfloat basicHeight = 160;
            nfloat height = 0;
            //notes => add comment, dinh kem comment

            if (viewElement.Notes != null && viewElement.Notes.Count > 0)
            {
                foreach (var note in viewElement.Notes)
                {
                    if (note.Key == "image")
                        height = height + 55;
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
                    // content comment
                    height = height + 100;
                }
            }

            return height;
        }
        #endregion
        private void ViewConfiguration()
        {
            headerView_constantHeight.Constant = 45 + 10 + CmmIOSFunction.GetHeaderViewHeight();

            //BT_close.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_approve.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_add.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_delete.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_complete.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);

            //TABLE
            table_comment = new UITableView(new CGRect(0, 0, 0, 0), UITableViewStyle.Plain);
            table_comment.TableFooterView = new UIView();
            table_comment.TableHeaderView = new UIView();
            table_comment.SeparatorStyle = UITableViewCellSeparatorStyle.None;

            lbl_line = new UILabel()
            {
                BackgroundColor = UIColor.FromRGB(245, 245, 245)
            };

            scrollview_details.Add(table_comment);
            scrollview_details.Add(lbl_line);
            scrollview_details.BringSubviewToFront(table_comment);
            scrollview_details.BringSubviewToFront(lbl_line);
            table_comment.ScrollEnabled = false;

            UpdateBTActionLayout();

            table_subtask.ScrollEnabled = false;
            constraint_heightTablesub.Constant = UIScreen.MainScreen.Bounds.Width;

            txt_note.TextContainer.MaximumNumberOfLines = 3;
            txt_note.ScrollEnabled = false;
            txt_note.UserInteractionEnabled = false;
            txt_note.TextContainer.LineBreakMode = UILineBreakMode.TailTruncation;

            view_title.Layer.BorderWidth = 1;
            view_title.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;
            view_title.Layer.CornerRadius = 3;
            view_title.ClipsToBounds = true;

            table_attachment.ContentInset = new UIEdgeInsets(-30, 0, 0, 0);
            table_attachment.ScrollEnabled = false;

            CmmIOSFunction.AddBorderView(view_title);
            CmmIOSFunction.AddBorderView(view_date);
            CmmIOSFunction.AddBorderView(view_user);
            CmmIOSFunction.AddBorderView(view_note);
            CmmIOSFunction.AddBorderView(view_status);

            attachFileView = Custom_AttachFileView.Instance;
            attachFileView.parentview = this;
            attachFileView.InitFrameView(new CGRect(0, 0, view_content.Frame.Width, view_content.Frame.Height));
            attachFileView.TableLoadData();

            //ClassMenu m1 = new ClassMenu() { ID = (int)ActionStatusID.Cancel, section = 0, title = CmmFunction.GetTitle("TEXT_CANCEL", ActionStatusID.Cancel.ToString()) };
            ClassMenu m1 = new ClassMenu() { ID = (int)ActionStatusID.Completed, section = 0, title = CmmFunction.GetTitle("TEXT_COMPLETED", ActionStatusID.Completed.ToString()) };
            ClassMenu m2 = new ClassMenu() { ID = (int)ActionStatusID.Hold, section = 0, title = CmmFunction.GetTitle("TEXT_HOLD", ActionStatusID.Hold.ToString()) };
            ClassMenu m3 = new ClassMenu() { ID = (int)ActionStatusID.InProgress, section = 0, title = CmmFunction.GetTitle("TEXT_INPROGRESS", ActionStatusID.InProgress.ToString()) };
            //ClassMenu m5 = new ClassMenu() { ID = 0, section = 0, title = CmmFunction.GetTitle("TEXT_NOPROCESS", "Chưa thực hiện") };
            lst_menuItem.AddRange(new[] { m1, m2, m3 });

        }

        private void BorderView()
        {

            //bo goc cho view header
            view_description.ClipsToBounds = true;
            UIBezierPath mPath_view_header = UIBezierPath.FromRoundedRect(view_description.Layer.Bounds, (UIRectCorner.TopLeft | UIRectCorner.TopRight), new CGSize(width: 6, height: 6));
            CAShapeLayer maskLayer_view_header = new CAShapeLayer();
            maskLayer_view_header.Frame = view_description.Layer.Bounds;
            maskLayer_view_header.Path = mPath_view_header.CGPath;
            view_description.Layer.Mask = maskLayer_view_header;

            // bo goc cho  tableView_attachment
            table_attachment.ClipsToBounds = true;
            UIBezierPath mPath_tableView_attachment = UIBezierPath.FromRoundedRect(table_attachment.Layer.Bounds, (UIRectCorner.BottomLeft | UIRectCorner.BottomRight), new CGSize(width: 6, height: 6));
            CAShapeLayer maskLayer_tableView_attachment = new CAShapeLayer();
            maskLayer_tableView_attachment.Frame = table_attachment.Layer.Bounds;
            maskLayer_tableView_attachment.Path = mPath_tableView_attachment.CGPath;
            table_attachment.Layer.Mask = maskLayer_tableView_attachment;
        }

        private void UpdateBTActionLayout()
        {
            constraintRightBtApprove.Constant = 14;
            constraintRightBtAdd.Constant = 49;
            constraintRightBtDelete.Constant = 85;
            constraintRightBtComplete.Constant = 119;
            //BT_approve.Frame = new CGRect(UIScreen.MainScreen.Bounds.Width - 45, BT_close.Frame.Y, BT_close.Frame.Width, BT_close.Frame.Height);
            //BT_add.Frame = new CGRect(BT_approve.Frame.X - 45, BT_add.Frame.Y, BT_add.Frame.Width, BT_add.Frame.Height);
            //BT_delete.Frame = new CGRect(BT_add.Frame.X - 45, BT_delete.Frame.Y, BT_delete.Frame.Width, BT_delete.Frame.Height);
            //BT_complete.Frame = new CGRect(BT_delete.Frame.X - 45, BT_complete.Frame.Y, BT_complete.Frame.Width, BT_complete.Frame.Height);
        }

        private void LoadContent()
        {
            tf_title.Text = task.Title;
            // nguoi giao
            var user = CmmFunction.GetBeanUserByID(task.CreatedBy);
            if (user != null)
                lbl_assignName.Text = user.FullName;

            if (task.DueDate.HasValue)
            {
                if (CmmVariable.SysConfig.LangCode == "1033")
                    tf_date.Text = task.DueDate.Value.ToString(CmmVariable.M_WorkDateFormatDateTimeEN);
                else
                    tf_date.Text = task.DueDate.Value.ToString(CmmVariable.M_WorkDateFormatDateTimeVN);
            }

            if (lst_menuItem != null && lst_menuItem.Count > 0)
            {
                foreach (var item in lst_menuItem)
                {
                    if (item.ID == task.Status)
                    {
                        item.isSelected = true;
                        currentStatusItem = item;
                    }
                    else
                        item.isSelected = false;
                }
            }
            contentHtmlString = task.Content;
            LoadContentHtml(contentHtmlString);

            //if (task.Status == (int)ActionStatusID.Cancel)
            //    tf_status.Text = CmmFunction.GetTitle("TEXT_CANCEL", ActionStatusID.Cancel.ToString());
            if (task.Status == (int)ActionStatusID.Completed)
                tf_status.Text = CmmFunction.GetTitle("TEXT_COMPLETED", ActionStatusID.Completed.ToString());
            else if (task.Status == (int)ActionStatusID.Hold)
                tf_status.Text = CmmFunction.GetTitle("TEXT_HOLD", ActionStatusID.Hold.ToString());
            else if (task.Status == (int)ActionStatusID.InProgress)
                tf_status.Text = CmmFunction.GetTitle("TEXT_INPROGRESS", ActionStatusID.InProgress.ToString());
            else
                tf_status.Text = CmmFunction.GetTitle("TEXT_NOPROCESS", "Chưa thực hiện");
        }

        private void ViewMore()
        {
            //bt_assignName
            var flagchecViewMore = CmmIOSFunction.CheckStringTrunCated(lbl_assignName);
            if (flagchecViewMore)
            {
                BT_assign.UserInteractionEnabled = true;
                BT_assign.Enabled = true;
            }
            else
            {
                BT_assign.UserInteractionEnabled = false;
                BT_assign.Enabled = false;
            }
            //bt_user
            UILabel lbl_text = new UILabel
            {
                Font = UIFont.SystemFontOfSize(tf_user.Font.PointSize),
                Text = tf_user.Text,
                Frame = tf_user.Frame
            };
            flagchecViewMore = false;
            flagchecViewMore = CmmIOSFunction.CheckStringTrunCated(lbl_text);
            if (flagchecViewMore)
            {
                BT_moreUser.UserInteractionEnabled = true;
                BT_moreUser.Enabled = true;
            }
            else
            {
                BT_moreUser.UserInteractionEnabled = false;
                BT_moreUser.Enabled = false;
            }
            //bt_title
            lbl_text = new UILabel
            {
                Font = UIFont.SystemFontOfSize(tf_title.Font.PointSize),
                Text = tf_title.Text,
                Frame = tf_title.Frame
            };
            flagchecViewMore = false;
            flagchecViewMore = CmmIOSFunction.CheckStringTrunCated(lbl_text);
            if (flagchecViewMore)
            {
                BT_moreTitle.UserInteractionEnabled = true;
                BT_moreTitle.Enabled = true;
            }
            else
            {
                BT_moreTitle.UserInteractionEnabled = false;
                BT_moreTitle.Enabled = false;
            }
        }
        private void LoadContentHtml(string htmlText)
        {
            if (!string.IsNullOrEmpty(htmlText))
            {
                try
                {
                    /*
                    var myHtmlText = htmlText.Trim();
                    NSMutableAttributedString att = new NSMutableAttributedString(myHtmlText);
                    att.BeginEditing();
                    att.AddAttribute(UIStringAttributeKey.Font, UIFont.FromName("ArialMT", 14f), new NSRange(0, myHtmlText.Length));
                    att.EndEditing();
                    txt_note.AttributedText = att;
                    */

                    NSString htmlString = new NSString(htmlText);
                    NSData htmlData = NSData.FromString(GetHtmlStyle() + htmlString);
                    NSAttributedStringDocumentAttributes importParams = new NSAttributedStringDocumentAttributes
                    {
                        DocumentType = NSDocumentType.HTML,
                        StringEncoding = NSStringEncoding.UTF8,
                    };

                    NSError error = new NSError();
                    error = null;
                    NSDictionary dict = new NSDictionary();
                    UIFont font = UIFont.FromName("ArialMT", 14f);
                    if (font != null)
                    {
                        dict = new NSMutableDictionary()
                        {
                            {
                                UIStringAttributeKey.Font,font
                            }
                        };
                    }

                    var attrString = new NSAttributedString(htmlData, importParams, out dict, ref error);
                    if (error != null) Console.WriteLine("Create attributted string - err: " + error.ToString());
                    txt_note.AttributedText = attrString;

                    var height = StringExtensions.StringHeight(txt_note.Text, UIFont.SystemFontOfSize(14), txt_note.Frame.Width);
                    if (height > txt_note.Frame.Height)
                        BT_contentMore.Hidden = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ControlTextInputFormat - Value - err: " + ex.ToString());
                    //throw;
                }
            }
        }

        private void setlangTitle()
        {
            CmmIOSFunction.SetLangToView(this.View);
            CmmIOSFunction.AddAttributeTitle(lbl_title, 11);
            CmmIOSFunction.AddAttributeTitle(lbl_user, 11);
        }

        private async void LoadTaskDetails()
        {
            try
            {
                loading = new CmmLoading(new CGRect((this.View.Bounds.Width - 200) / 2, (this.View.Bounds.Height - 200) / 2, 200, 200), "Đang xử lý...");
                ProviderControlDynamic pControlDynamic = new ProviderControlDynamic();
                await Task.Run(() =>
                {
                    string _resultString = pControlDynamic.GetDetailTaskForm(task.ID);

                    if (!string.IsNullOrEmpty(_resultString))
                    {
                        detailsForm = JObject.Parse(_resultString);

                        TaskResult = JsonConvert.DeserializeObject<BeanTask>(detailsForm["parentItem"].ToString());
                        lst_currentUserAndGroup = JsonConvert.DeserializeObject<List<BeanUserAndGroup>>(detailsForm["assignTo"].ToString());
                        lst_currentAttach = JsonConvert.DeserializeObject<List<BeanAttachFile>>(detailsForm["attachment"].ToString());
                        lst_childTask = JsonConvert.DeserializeObject<List<BeanTaskDetail>>(detailsForm["childTask"].ToString());
                        _flagUserPermission = (int)detailsForm["userPermission"];

                        if (!string.IsNullOrEmpty(detailsForm["parentItem"]["OtherResourceId"].ToString()))
                            OtherResourceId = detailsForm["parentItem"]["OtherResourceId"].ToString();
                        else
                            OtherResourceId = "";

                        string _APIdatenow = "";
                        try // Authen View Comment -> để sau này gọi API comment
                        {
                            // tracking
                            ObjectSubmitDetailComment objSubmitDetailComment = new ObjectSubmitDetailComment();
                            // Comment
                            objSubmitDetailComment.ID = OtherResourceId; // empty or result
                            objSubmitDetailComment.ResourceCategoryId = ((int)CmmFunction.CommentResourceCategoryID.Task).ToString();
                            objSubmitDetailComment.ResourceUrl = string.Format(CmmFunction.GetURLSettingComment((int)CmmFunction.CommentResourceCategoryID.Task), task.ID); // lấy trong beansetting
                            objSubmitDetailComment.ItemId = task.ID.ToString();
                            objSubmitDetailComment.Author = CmmVariable.SysConfig.UserId;
                            objSubmitDetailComment.AuthorName = CmmVariable.SysConfig.DisplayName;
                            if (string.IsNullOrEmpty(OtherResourceId))
                                OtherResourceId = pControlDynamic.GetDetailOtherResource(objSubmitDetailComment);
                            else
                                pControlDynamic.GetDetailOtherResource(objSubmitDetailComment);
                        }
                        catch (Exception)
                        {
                            OtherResourceId = "";
                        }

                        lst_comment = pControlDynamic.GetListComment(OtherResourceId, (int)CmmFunction.CommentResourceCategoryID.Task, null, ref _APIdatenow);

                        if (lst_comment != null && lst_comment.Count > 0)
                        {
                            //_parentItem.CommentChanged = DateTime.Parse(_APIdatenow);
                        }

                        lst_childTask = Custom_TableTaskHelper.Instance().getSortedNodes(task.ID, lst_childTask, true);
                        //danh index cho task
                        Setindexlistdata(task.ID, 0);
                        //setting data to display in task
                        SettingDataLstTasks();

                        InvokeOnMainThread(() =>
                        {
                            if (TaskResult != null)
                                task = TaskResult;

                            LoadContent();
                            LoadConfigurationComment();

                            if (lst_currentUserAndGroup != null && lst_currentUserAndGroup.Count > 0)
                                LoadListUserProcess();
                            ViewMore();

                            if (currentIndexDetails == 1)
                            {
                                if (lst_childTask != null && lst_childTask.Count > 0)
                                    LoadSubtask();
                                HightLightText(lbl_details, false);
                                HightLightText(lbl_subtask, true);
                            }
                            else if (currentIndexDetails == 0)
                            {
                                if (lst_currentAttach != null && lst_currentAttach.Count > 0)
                                    LoadAttachments();

                                HightLightText(lbl_details, true);
                                HightLightText(lbl_subtask, false);
                            }
                            SetContentSizeScroll();
                            SetViewByPermission();
                            view_nodata.Hidden = true;
                            view_nodataTop.Hidden = true;

                            if (lst_childTask != null && lst_childTask.Count > 0)
                                lbl_subtask.Text = CmmFunction.GetTitle("TEXT_CHILDTASK", "Công việc con") + " (" + lst_childTask.Count.ToString() + ")";
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("FormTaskDetails - LoadTaskDetails - ERR: " + ex.ToString());
            }
        }

        private void SettingDataLstTasks()
        {
            var parent = lst_childTask.FindAll(s => s.Parent == task.ID);
            int session = 0;
            foreach (var item in parent)
            {
                item.session = session++;
            }
            foreach (var item in lst_childTask)
            {
                //setNodeIcon(item);
                long num = -1;
                findsection(item.ID, ref num);
                var itemParent = lst_childTask.Find(s => s.ID == num);
                if (item.Parent != 0)
                    item.session = itemParent.session;
            }
            // kiem tra phai la node cuoi cung khong
            // danh dau node cuoi cung trong mot group la node la
            if (parent != null && parent.Count > 0)
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
            var item = lst_childTask.FindAll(s => s.Parent == id);
            foreach (var item1 in item)
            {
                for (int i = 0; i < lst_childTask.Count; i++)
                {
                    if (lst_childTask[i].ID == item1.ID)
                    {

                        lst_childTask[i].index = index;
                        Setindexlistdata(lst_childTask[i].ID, index + 1);
                    }
                }
            }
        }

        private void LoadCountSubTask(BeanTaskDetail parent_task)
        {
            var groupChild = lst_childTask.FindAll(s => s.Parent == parent_task.ID);
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
            var item = lst_childTask.Find(s => s.ID == id);
            if (item.Parent == task.ID)
            {
                output = id;
                return;
            }
            findsection(item.Parent, ref output);
        }

        private void LoadListUserProcess()
        {
            if (lst_currentUserAndGroup != null && lst_currentUserAndGroup.Count > 0)
            {
                string res = "";
                foreach (var u in lst_currentUserAndGroup)
                {
                    res += u.Name + ", ";
                }
                tf_user.Text = res.TrimEnd(' ').TrimEnd(',');
            }
        }

        private void LoadAttachments()
        {
            scrollview_details.Frame = new CGRect(0, view_chosse.Frame.Bottom, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height - view_chosse.Frame.Bottom);
            SetContentSizeScroll();
            table_attachment.Source = new Attachment_TableSource(lst_currentAttach, this);
            table_attachment.ReloadData();
        }

        //setting contentsize scroll
        private void SetContentSizeScroll()
        {
            if (currentIndexDetails == 0) // tab chi tiet
            {
                int count = (lst_currentAttach != null && lst_currentAttach.Count > 0) ? lst_currentAttach.Count : 0;
                constraint_heightViewAttachment.Constant = (count * 60) + view_description.Frame.Bottom;

                lbl_line.Frame = new CGRect(15, view_attachment.Frame.Top + constraint_heightViewAttachment.Constant + 15, view_attachment.Frame.Width, 1);
                table_comment.Frame = new CGRect(15, view_attachment.Frame.Top + constraint_heightViewAttachment.Constant + 35, view_attachment.Frame.Width, heightComment);

            }
            else // tab child
            {
                int count = (lst_childTask != null && lst_childTask.Count > 0) ? lst_childTask.Count : 0;
                constraint_heightSubView.Constant = (count * 90);

                lbl_line.Frame = new CGRect(15, table_subtask.Frame.Top + constraint_heightSubView.Constant + 15, view_attachment.Frame.Width, 1);
                table_comment.Frame = new CGRect(15, table_subtask.Frame.Top + constraint_heightSubView.Constant + 35, view_attachment.Frame.Width, heightComment);

            }

            var heightScroll = UIScreen.MainScreen.Bounds.Height - view_chosse.Frame.Bottom;
            if (heightScroll < table_comment.Frame.Bottom)
            {
                scrollview_details.Frame = new CGRect(scrollview_details.Frame.X, scrollview_details.Frame.Y, UIScreen.MainScreen.Bounds.Width, heightScroll);
                scrollview_details.ContentSize = new CGSize(scrollview_details.Frame.Width, table_comment.Frame.Bottom + 10);
            }
            else
            {
                scrollview_details.Frame = new CGRect(scrollview_details.Frame.X, scrollview_details.Frame.Y, UIScreen.MainScreen.Bounds.Width, heightScroll);
                scrollview_details.ContentSize = new CGSize(scrollview_details.Frame.Width, heightScroll + 10);
            }
            //// bo goc cho  tableView_attachment
            //table_attachment.ClipsToBounds = true;
            //UIBezierPath mPath_tableView_attachment = UIBezierPath.FromRoundedRect(table_attachment.Layer.Bounds, (UIRectCorner.BottomLeft | UIRectCorner.BottomRight), new CGSize(width: 6, height: 6));
            //CAShapeLayer maskLayer_tableView_attachment = new CAShapeLayer();
            //maskLayer_tableView_attachment.Frame = table_attachment.Layer.Bounds;
            //maskLayer_tableView_attachment.Path = mPath_tableView_attachment.CGPath;
            //table_attachment.Layer.Mask = maskLayer_tableView_attachment;
        }

        private void LoadSubtask()
        {
            table_subtask.Source = new Custom_TableTask(lst_childTask, null, this, task.ID);
            //table_subtask.Source = new Tasks_TableSource(lst_childTask, this);
            table_subtask.ReloadData();
        }

        public void ReLoadRootView()
        {
            LoadTaskDetails();
            if (parent.GetType() == typeof(RequestDetailsV2))
            {
                //LoadContent();
                //LoadTaskDetails();
                RequestDetailsV2 requestDetailsV2 = parent as RequestDetailsV2;
                requestDetailsV2.ReloadDataForm(false);
            }
            else if (parent.GetType() == typeof(MainView))
            {
                MainView mainView = parent as MainView;
                mainView.ReloadDataForm();
            }
            else if (parent.GetType() == typeof(RequestListView))
            {
                RequestListView requestListView = parent as RequestListView;
                requestListView.ReloadDataForm();
            }
            else if (parent.GetType() == typeof(MyRequestListView))
            {
                MyRequestListView myRequestListView = parent as MyRequestListView;
                myRequestListView.ReloadDataForm();
            }
            //app
            //else if (parent.GetType() == typeof(MainViewApp))
            //{
            //    MainViewApp mainViewApp = parent as MainViewApp;
            //    mainViewApp.ReloadDataForm();
            //}
            //else if (parent.GetType() == typeof(RequestListViewApp))
            //{
            //    RequestListViewApp requestListViewApp = parent as RequestListViewApp;
            //    requestListViewApp.ReloadDataForm();
            //}
            //else if (parent.GetType() == typeof(MyRequestListViewApp))
            //{
            //    MyRequestListViewApp myRequestListViewApp = parent as MyRequestListViewApp;
            //    myRequestListViewApp.ReloadDataForm();
            //}
        }

        private void SetViewByPermission()
        {
            // -1: viewer || 1: Creator || 2: Handle || 3:Creator & handle

            // Viewer - chỉ dc xem và ko dc thao tác gì thêm
            if (_flagUserPermission == (int)FlagUserPermission.Viewer)
            {
                //khong hien thi boder voi  * cho "tieu de", khong cho chon
                tf_title.UserInteractionEnabled = false;
                lbl_title.Text = lbl_title.Text.Replace("(*)", "");
                title_ConstantLeft.Constant = 0;
                view_title.Layer.BorderWidth = 0;

                //khong hien thi boder ,  * , icon person cho "nguoi xu ly", khong cho chon
                BT_user.UserInteractionEnabled = false;
                iv_rightUser.Hidden = true;
                lbl_user.Text = lbl_user.Text.Replace("(*)", "");
                user_ConstantLeft.Constant = 0;
                view_user.Layer.BorderWidth = 0;

                //khong hien thi boder, switch date, icon date, khong cho chon
                BT_date.Enabled = false;
                date_ConstantLeft.Constant = 0;
                iv_right.Hidden = true;
                view_date.Layer.BorderWidth = 0;

                //khong hien thi boder tinh trang, khong cho chon
                BT_status.Enabled = false;
                view_status.Layer.BorderWidth = 0;
                img_dropdown.Hidden = true;
                status_ConstantLeft.Constant = 0;
                lbl_status.Hidden = false;

                //khong hien thi boder noi dung, cho sua noi dung
                BT_note.Enabled = false;
                txt_note.Editable = false;
                view_note.Layer.BorderWidth = 0;
                note_ConstantLeft.Constant = 0;

                // khong cho dinh kem file
                view_bt_addAttach.Hidden = true;

                // khong hien thi BT
                BT_approve.Hidden = true;
                BT_add.Hidden = true;
                BT_delete.Hidden = true;
                BT_complete.Hidden = true;
            }
            // Handler - được chỉnh tình trạng, file đính kèm, tạo Task con
            else if (_flagUserPermission == (int)FlagUserPermission.Handler)
            {
                //khong hien thi boder voi  * cho "tieu de", khong cho chon
                tf_title.UserInteractionEnabled = false;
                title_ConstantLeft.Constant = 0;
                lbl_title.Text = lbl_title.Text.Replace("(*)", "");
                view_title.Layer.BorderWidth = 0;

                //khong hien thi boder ,  * , icon person cho "nguoi xu ly", khong cho chon
                BT_user.UserInteractionEnabled = false;
                iv_rightUser.Hidden = true;
                user_ConstantLeft.Constant = 0;
                lbl_user.Text = lbl_user.Text.Replace("(*)", "");
                view_user.Layer.BorderWidth = 0;

                //khong hien thi boder, icon date, khong cho chon
                BT_date.Enabled = false;
                date_ConstantLeft.Constant = 0;
                iv_right.Hidden = true;
                view_date.Layer.BorderWidth = 0;

                //khong hien thi boder noi dung, cho sua noi dung
                BT_note.Enabled = false;
                txt_note.Editable = false;
                view_note.Layer.BorderWidth = 0;
                note_ConstantLeft.Constant = 0;

                //khong hien thi bt delete
                BT_approve.Hidden = false;
                BT_add.Hidden = false;
                BT_complete.Hidden = false;

                BT_delete.Hidden = true;
                constraintRightBtComplete.Constant = constraintRightBtDelete.Constant;


                // trang thai Hold / Complete thi khong dc lam gi
                if (task.Status == (int)ActionStatusID.Hold || task.Status == (int)ActionStatusID.Completed)
                {
                    // TRANG THAI HOAN TAT, bt duoc hien thi: bt_add
                    // TRANG THAI HOAN, khong co BT duoc hien thi
                    BT_complete.Hidden = true;
                    BT_delete.Hidden = true;
                    BT_approve.Hidden = true;

                    if (task.Status == (int)ActionStatusID.Completed)
                    {
                        // khong cho dinh kem file
                        view_bt_addAttach.Hidden = false;
                        //khong hien thi boder tinh trang, khong cho chon
                        BT_status.Enabled = false;
                        view_status.Layer.BorderWidth = 0;
                        img_dropdown.Hidden = true;
                        status_ConstantLeft.Constant = 0;

                        BT_add.Hidden = false;
                        constraintRightBtAdd.Constant = constraintRightBtApprove.Constant;
                    }
                    else
                    {
                        status_ConstantLeft.Constant = 15;
                        // cho dinh kem file
                        view_bt_addAttach.Hidden = true;

                        //cho luu va hoan thanh
                        BT_add.Hidden = true;
                        BT_complete.Hidden = false;
                        BT_approve.Hidden = false;
                        BT_complete.Frame = BT_add.Frame;
                    }
                }
            }
            // Creator
            else if (_flagUserPermission == (int)FlagUserPermission.Creator)
            {
                // check xem nếu phiếu đang tạm hoãn / Conpleted thì ko cho làm gì -> giống viewer
                if (task.Status == (int)ActionStatusID.Hold || task.Status == (int)ActionStatusID.Completed)
                {
                    //khong hien thi boder voi  * cho "tieu de", khong cho chon
                    tf_title.UserInteractionEnabled = false;
                    lbl_title.Text = lbl_title.Text.Replace("(*)", "");
                    title_ConstantLeft.Constant = 0;
                    view_title.Layer.BorderWidth = 0;

                    //khong hien thi boder ,  * , icon person cho "nguoi xu ly", khong cho chon
                    BT_user.UserInteractionEnabled = false;
                    lbl_user.Text = lbl_user.Text.Replace("(*)", "");
                    view_user.Layer.BorderWidth = 0;
                    user_ConstantLeft.Constant = 0;
                    iv_rightUser.Hidden = true;


                    //khong hien thi boder, icon date, khong cho chon
                    BT_date.Enabled = false;
                    date_ConstantLeft.Constant = 0;
                    view_date.Layer.BorderWidth = 0;
                    iv_right.Hidden = true;

                    //khong hien thi boder tinh trang, khong cho chon
                    BT_status.Enabled = false;
                    view_status.Layer.BorderWidth = 0;
                    img_dropdown.Hidden = true;
                    status_ConstantLeft.Constant = 0;

                    //khong hien thi boder noi dung, cho sua noi dung
                    BT_note.Enabled = false;
                    txt_note.Editable = false;
                    view_note.Layer.BorderWidth = 0;
                    note_ConstantLeft.Constant = 0;

                    // khong cho dinh kem file
                    view_bt_addAttach.Hidden = true;

                    // TRANG THAI HOAN TAT, bt duoc hien thi: bt_add
                    // TRANG THAI HOAN, khong co BT duoc hien thi

                    BT_complete.Hidden = true;
                    BT_delete.Hidden = true;
                    BT_approve.Hidden = true;

                    if (task.Status == (int)ActionStatusID.Completed)
                    {
                        BT_add.Hidden = false;
                        BT_add.Frame = BT_approve.Frame;
                    }
                    else
                    {
                        BT_add.Hidden = true;
                    }
                }
                else
                {
                    //bt duoc hien thi: bt_delete/ bt_add/ bt_save hien thi
                    BT_complete.Hidden = true;

                    // khong cho hien thi tinh trang
                    BT_status.Hidden = true;
                    tf_status.Hidden = true;
                    lbl_status.Hidden = true;
                    img_dropdown.Hidden = true;
                    view_status.Hidden = true;
                }
            }
            // Creator and handle
            else if (_flagUserPermission == (int)FlagUserPermission.CreatorAndHandler)
            {
                //Check Status "Hoàn tất" - đã hoàn tất thì ko dc làm gì, chi duoc giao task con
                if (task.Status == (int)ActionStatusID.Completed)
                {
                    //  remove * Tiêu đề , remove border content Tiêu đề
                    tf_title.UserInteractionEnabled = false;
                    lbl_title.Text = lbl_title.Text.Replace("(*)", "");
                    title_ConstantLeft.Constant = 0;
                    view_title.Layer.BorderWidth = 0;

                    //  remove *  Người xử lý , remove border content  Người xử lý
                    BT_user.UserInteractionEnabled = false;
                    iv_rightUser.Hidden = true;
                    lbl_user.Text = lbl_user.Text.Replace("(*)", "");
                    user_ConstantLeft.Constant = 0;
                    view_user.Layer.BorderWidth = 0;

                    // Hạn hoàn tất
                    BT_date.UserInteractionEnabled = false;
                    iv_right.Hidden = true;
                    view_date.Layer.BorderWidth = 0;
                    date_ConstantLeft.Constant = 0;

                    // Tình trạng ko dc Edit
                    BT_status.Enabled = false;
                    BT_status.UserInteractionEnabled = false;
                    img_dropdown.Hidden = true;
                    view_status.Layer.BorderWidth = 0;
                    status_ConstantLeft.Constant = 0;

                    // Nội dung
                    BT_note.Enabled = false;
                    txt_note.Editable = false;
                    view_note.Layer.BorderWidth = 0;
                    note_ConstantLeft.Constant = 0;

                    //Đính kèm
                    view_bt_addAttach.Hidden = true;

                    BT_add.Hidden = false;
                    BT_complete.Hidden = true;
                    //line_bt_complete.Hidden = true;

                    //line_1.Hidden = true;

                    BT_delete.Hidden = true;
                    //line_0.Hidden = true;

                    BT_approve.Hidden = true;
                    BT_add.Frame = BT_approve.Frame;
                }
                // Người xử lý đồng thời là người tạo và phiếu đang tạm hoãn -> quyền giống người xử lý
                else if (task.Status == (int)ActionStatusID.Hold)
                {
                    //khong hien thi boder voi  * cho "tieu de", khong cho chon
                    tf_title.UserInteractionEnabled = false;
                    lbl_title.Text = lbl_title.Text.Replace("(*)", "");
                    title_ConstantLeft.Constant = 0;
                    view_title.Layer.BorderWidth = 0;


                    //khong hien thi boder ,  * , icon person cho "nguoi xu ly", khong cho chon
                    BT_user.UserInteractionEnabled = false;
                    iv_rightUser.Hidden = true;
                    lbl_user.Text = lbl_user.Text.Replace("(*)", "");
                    user_ConstantLeft.Constant = 0;
                    view_user.Layer.BorderWidth = 0;

                    //khong hien thi boder, icon date, khong cho chon
                    BT_date.Enabled = false;
                    date_ConstantLeft.Constant = 0;
                    iv_right.Hidden = true;
                    view_date.Layer.BorderWidth = 0;
                    status_ConstantLeft.Constant = 0;

                    // khong hien thi boder, Nội dung, khong cho chon
                    BT_note.Enabled = false;
                    txt_note.Editable = false;
                    view_note.Layer.BorderWidth = 0;
                    note_ConstantLeft.Constant = 0;

                    // khong cho dinh kem file
                    //view_bt_addAttach.Hidden = true;

                    //view status
                    status_ConstantLeft.Constant = 15;

                    // BT Duoc hien thi: BT_complete/ BT_approve
                    constraintRightBtComplete.Constant = constraintRightBtAdd.Constant;
                    BT_delete.Hidden = true;
                    BT_add.Hidden = true;
                }
                // Các tình trạng còn lại -> dc làm tất cả

            }
            //isFirst = false;
            //UpdateBTActionLayout();
        }

        private void EnableInputView(bool status)
        {
            tf_title.Enabled = status;
            txt_note.Editable = status;
        }

        private nfloat GetPositionBotView(UIView view)
        {
            nfloat bottom = view.Frame.Height;
            UIView supperView = view;
            do
            {
                bottom += supperView.Frame.Y;
                supperView = supperView.Superview;

            } while (supperView != this.View);

            return bottom + 20;
        }

        public void AddUserToList(BeanUser _selectedUser)
        {
            var index = lst_selectedUser.FindIndex(item => item.ID == _selectedUser.ID);
            if (index == -1)
                lst_selectedUser.Add(_selectedUser);
        }

        private string GetStringUsers()
        {
            string strUsers = "";
            if (lst_selectedUser.Count > 0)
            {
                foreach (var item in lst_selectedUser)
                {
                    strUsers += item.Name + "; ";
                }
            }

            if (!string.IsNullOrEmpty(strUsers))
            {
                strUsers = strUsers.Trim().TrimEnd(';');
            }

            return strUsers;
        }

        private List<string> GetListIdUserSelected()
        {
            if (lst_selectedUser.Count > 0)
            {
                List<string> lst_result = new List<string>();
                foreach (var item in lst_selectedUser)
                {
                    lst_result.Add(item.ID);
                }

                return lst_result;
            }
            else
                return null;

        }
        private void HightLightText(UILabel _label, bool _isSelected)
        {
            if (_label.Text.Contains("("))
            {
                var str_transalte = _label.Text;
                var indexA = str_transalte.IndexOf('(');
                NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);

                if (_isSelected)
                {
                    att.AddAttribute(UIStringAttributeKey.Font, UIFont.BoldSystemFontOfSize(13), new NSRange(0, str_transalte.Length));
                    att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(0, str_transalte.Length));
                    att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(255, 122, 58), new NSRange(indexA, str_transalte.Length - indexA));
                }
                else
                {
                    att.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(13, UIFontWeight.Regular), new NSRange(0, str_transalte.Length));
                    att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(94, 94, 94), new NSRange(0, str_transalte.Length));
                }
                _label.AttributedText = att;
            }
            else
            {
                var str_transalte = _label.Text;
                NSMutableAttributedString att = new NSMutableAttributedString(str_transalte);

                if (_isSelected)
                {
                    att.AddAttribute(UIStringAttributeKey.Font, UIFont.BoldSystemFontOfSize(13), new NSRange(0, str_transalte.Length));
                    att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(0, 95, 212), new NSRange(0, str_transalte.Length));
                }
                else
                {
                    att.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(13, UIFontWeight.Regular), new NSRange(0, str_transalte.Length));
                    att.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.FromRGB(94, 94, 94), new NSRange(0, str_transalte.Length));
                }

                _label.AttributedText = att;
            }
        }
        public void RemoveUserFromList(BeanUser _removeUser)
        {
            lst_selectedUser.Remove(_removeUser);
            //table_selectedUser.ReloadData();

            tf_user.Text = GetStringUsers();
        }
        private bool ValidateFormControlValue()
        {
            if (String.IsNullOrEmpty(tf_title.Text))
            {
                CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("MESS_REQUIRE_TITLE", "Vui lòng nhập tiêu đề."));
                return false;
            }
            if (string.IsNullOrEmpty(tf_user.Text))
            {
                CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("MESS_REQUIRE_USERGROUP", "Vui lòng chọn người để thực hiện."));
                return false;
            }

            if (!string.IsNullOrEmpty(tf_date.Text))
            {
                DateTime _validateItem = DateTime.Now;
                if (CmmVariable.SysConfig.LangCode == "1033")
                    _validateItem = DateTime.ParseExact(tf_date.Text, CmmVariable.M_WorkDateFormatDateTimeEN, null);
                else
                    _validateItem = DateTime.ParseExact(tf_date.Text, CmmVariable.M_WorkDateFormatDateTimeVN, null);

                if (_validateItem < DateTime.Now)
                {
                    //CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("BPM", "Hạn hoàn tất không được nhỏ hơn thời gian hiện tại."));
                    tf_date.TextColor = UIColor.Red;
                    return true;
                }
            }


            //if (string.IsNullOrEmpty(txt_note.Text))
            //{
            //    CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("MESS_REQUIRE_TITLE", "Vui lòng nhập nội dung."));
            //    return false;
            //}

            return true;
        }
        public async void Handle_RemoveTask()
        {
            try
            {
                loading = new CmmLoading(new CGRect((this.View.Bounds.Width - 200) / 2, (this.View.Bounds.Height - 200) / 2, 200, 200), "Đang xử lý...");
                this.View.Add(loading);

                bool res = false;
                await Task.Run(() =>
                {
                    ProviderControlDynamic p_dynamic = new ProviderControlDynamic();
                    res = p_dynamic.DeleteDetailTaskForm(task.ID);

                    InvokeOnMainThread(() =>
                    {
                        loading.Hide();

                        if (res)
                        {
                            if (parent.GetType() == typeof(RequestDetailsV2))
                            {
                                RequestDetailsV2 requestDetailsV2 = parent as RequestDetailsV2;
                                requestDetailsV2.ReloadDataForm(false);
                            }

                            this.NavigationController.PopViewController(true);
                        }
                        else
                            CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("K_Mess_ActionFalse", "Thao tác không thực hiện được, vui lòng thử lại."));

                        loading.Hide();
                    });

                });
            }
            catch (Exception ex)
            {
                loading.Hide();
                Console.WriteLine("ToDoDetailsView - Handle_RemoveTask - Err: " + ex.ToString());
            }
        }

        public async void Handle_SubmitAction(bool actionComplete)
        {
            loading = new CmmLoading(new CGRect((this.View.Bounds.Width - 200) / 2, (this.View.Bounds.Height - 200) / 2, 200, 200), "Đang xử lý...");
            this.View.Add(loading);
            bool _result = false;
            try
            {
                ProviderControlDynamic _pControlDynamic = new ProviderControlDynamic();

                List<KeyValuePair<string, string>> _lstKeyVarAttachmentLocal = new List<KeyValuePair<string, string>>();
                if (lst_addAttachFile != null && lst_addAttachFile.Count > 0) // Lấy những file thêm mới từ App ra
                {
                    foreach (var item in lst_addAttachFile)
                    {
                        if (item.ID == "")
                        {
                            string key = item.Title;
                            KeyValuePair<string, string> _UploadFile = new KeyValuePair<string, string>(key, item.Path);
                            _lstKeyVarAttachmentLocal.Add(_UploadFile);
                        }
                    }
                }

                // Xử lý file bị xóa đi
                List<ObjectSubmitAction> _lstSubmitActionData = new List<ObjectSubmitAction>();

                if (lst_removeAttachFile != null && lst_removeAttachFile.Count > 0) // Nếu có xóa mới có list này
                {
                    // Loại bỏ những item Local ra
                    //_lstAttFileControl_Deleted = _lstAttFileControl_Deleted.Where(x => !x.ID.Equals("")).ToList();

                    ObjectSubmitAction _beanSubmitDeleted = new ObjectSubmitAction() // Object của những Attach File đã bị xóa khỏi List.
                    {
                        ID = "",
                        Value = JsonConvert.SerializeObject(lst_removeAttachFile),
                        TypeSP = "RemoveAttachment",
                        DataType = ""
                    };
                    _lstSubmitActionData.Add(_beanSubmitDeleted);
                }

                if (ValidateFormControlValue())
                {
                    task.Title = tf_title.Text;
                    //task.Content = txt_note.Text; //!string.IsNullOrEmpty(txt_note.AttributedText.Value) ? txt_note.AttributedText.ToString() : "";
                    task.Content = contentHtmlString;

                    if (actionComplete)
                        task.Status = 2;
                    else
                    {
                        if (currentStatusItem != null)
                            task.Status = currentStatusItem.ID;
                    }

                    if (!string.IsNullOrEmpty(tf_date.Text))
                    {
                        DateTime _validateItem = DateTime.Now;
                        if (CmmVariable.SysConfig.LangCode.Equals("1033"))
                            _validateItem = DateTime.ParseExact(tf_date.Text, "MM/dd/yy HH:mm", null);
                        else
                            _validateItem = DateTime.ParseExact(tf_date.Text, "dd/MM/yy HH:mm", null);
                        task.DueDate = _validateItem;
                    }

                    await Task.Run(() =>
                    {
                        _result = _pControlDynamic.SendCreateTaskAction(task, lst_currentUserAndGroup, _lstSubmitActionData, _lstKeyVarAttachmentLocal, _flagUserPermission);

                        if (_result)
                        {
                            ProviderBase pBase = new ProviderBase();
                            pBase.UpdateAllMasterData(true);
                            pBase.UpdateAllDynamicData(true);
                            InvokeOnMainThread(() =>
                            {
                                loading.Hide();
                                if (parent.GetType() == typeof(RequestDetailsV2))
                                {
                                    RequestDetailsV2 requestDetailsV2 = parent as RequestDetailsV2;
                                    requestDetailsV2.ReloadDataForm(false);
                                }
                                else if (parent.GetType() == typeof(FormTaskDetails))
                                {
                                    FormTaskDetails formTaskDetails = parent as FormTaskDetails;
                                    formTaskDetails.ReLoadRootView();
                                }

                                if (parent != null)
                                {
                                    if (parent is MainView)
                                        (parent as MainView).doReload = false;
                                    else if (parent is RequestListView)
                                        (parent as RequestListView).doReload = false;
                                    else if (parent is MyRequestListView)
                                        (parent as MyRequestListView).doReload = false;
                                    //else if (parentView is FollowListViewController)
                                    //   (parentView as FollowListViewController).doReload = true;
                                }

                                this.NavigationController.PopViewController(true);
                            });
                            if (currentIndexDetails == 0)
                                TouchDetail();
                            else if (currentIndexDetails == 1)
                            {
                                LoadTaskDetails();
                                TouchSubtask();
                            }
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
                else
                {
                    loading.Hide();
                }
            }
            catch (Exception ex)
            {
                loading.Hide();
                CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("K_Mess_ActionFalse", "Thao tác không thực hiện được, vui lòng thử lại."));
                Console.WriteLine("FormTaskDetails - BT_approve_TouchUpInside - Err: " + ex.ToString());
            }
        }
        #region RichTextEditor
        private async void ShowRichTextEditor(bool allowEdit)
        {
            try
            {
                if (allowEdit)
                {
                    TEditorResponse response;
                    /*if (!string.IsNullOrEmpty(contentHtmlString))//!string.IsNullOrEmpty(txt_note.AttributedText.Value) || !string.IsNullOrEmpty(txt_note.Text)
                        response = await ShowTEditor(allowEdit, contentHtmlString, null, true);
                    else
                        response = await ShowTEditor(allowEdit, "", null, true);*/

                    if (!string.IsNullOrEmpty(txt_note.AttributedText.Value) || !string.IsNullOrEmpty(txt_note.Text))
                        response = await CmmIOSFunction.ShowTEditor(allowEdit, contentHtmlString, this, this, tvc, null);
                    else
                        response = await CmmIOSFunction.ShowTEditor(allowEdit, "", this, this, tvc, null);

                    if (response.IsSave)
                    {
                        if (response.HTML != null)
                        {
                            contentHtmlString = response.HTML;
                            //SetValueResult(contentHtmlString);
                            LoadContentHtml(contentHtmlString);
                        }
                    }
                }
                else
                {
                    FullTextView fullTextView = (FullTextView)Storyboard.InstantiateViewController("FullTextView");
                    fullTextView.SetContent("", contentHtmlString, true);
                    this.NavigationController.PushViewController(fullTextView, true);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("FormCreateTaskView - ShowRichTextEditor - Err:" + ex.ToString());
            }
        }
        public void SetValueResult(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {

                NSString htmlString = new NSString(value);
                NSData htmlData = NSData.FromString(GetHtmlStyle() + htmlString);
                NSAttributedStringDocumentAttributes importParams = new NSAttributedStringDocumentAttributes();
                importParams.DocumentType = NSDocumentType.HTML;
                importParams.StringEncoding = NSStringEncoding.UTF8;

                NSError error = new NSError();
                error = null;
                NSDictionary dict = new NSDictionary();
                UIFont font = UIFont.FromName("ArialMT", 14f);
                if (font != null)
                {
                    dict = new NSMutableDictionary()
                    {
                        {
                            UIStringAttributeKey.Font,
                            font
                        }
                    };
                }

                var attrString = new NSAttributedString(htmlData, importParams, out dict, ref error);

                txt_note.AttributedText = attrString;
                task.Content = value;
            }
        }

        public Task<TEditorResponse> ShowTEditor(bool allowEdit, string html, ToolbarBuilder toolbarBuilder = null, bool autoFocusInput = false)
        {
            isShowTEditor = true;
            ///header
            UIView viewRichText = new UIView();
            viewRichText.Frame = new CGRect(0, 0, view_content.Frame.Width, 70);
            UIView header_line = new UIView();
            header_line.Frame = new CGRect(0, 69, viewRichText.Frame.Width, 1);
            header_line.BackgroundColor = UIColor.FromRGB(245, 245, 245);
            UIButton BT_RichTextback = new UIButton();
            BT_RichTextback.SetImage(UIImage.FromFile("Icons/icon_back20.png"), UIControlState.Normal);
            UIButton BT_RichTextCheck = new UIButton();
            BT_RichTextCheck.SetImage(UIImage.FromFile("Icons/icon_tick20.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);


            BT_RichTextback.Frame = new CGRect(10, 35, 35, 35);
            BT_RichTextback.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_RichTextCheck.Frame = new CGRect(view_content.Frame.Width - 45, 35, 35, 35);
            BT_RichTextCheck.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            viewRichText.AddSubviews(BT_RichTextback, header_line, BT_RichTextCheck);
            //content
            TaskCompletionSource<TEditorResponse> taskRes = new TaskCompletionSource<TEditorResponse>();
            tvc = new TEditorViewController();

            ToolbarBuilder builder = toolbarBuilder;
            if (toolbarBuilder == null)
            {
                builder = new ToolbarBuilder().AddStandard();
            }
            tvc.BuildToolbar(builder);

            tvc.SetHTML(html);
            if (!allowEdit)
            {
                (tvc.View.Subviews[0] as UIWebView).UserInteractionEnabled = true;//false
                //tvc.SetEditing(false, false);
                //tvc.Editing = false;
                //(tvc.View.Subviews[0] as UIWebView).MultipleTouchEnabled = false;
                tvc.View.Subviews[0].Frame = new CGRect(0, 70, tvc.View.Frame.Width, tvc.View.Subviews[0].Frame.Height);
            }
            else
            {
                tvc.SetAutoFocusInput(true);
            }

            tvc.View.BackgroundColor = UIColor.White;
            tvc.Add(viewRichText);

            CGRect startFrame = new CGRect(view_content.Frame.X, view_content.Frame.Height, view_content.Bounds.Width, view_content.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            tvc.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            tvc.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            tvc.TransitioningDelegate = transitioningDelegate;
            this.PresentViewControllerAsync(tvc, true);

            BT_RichTextCheck.TouchUpInside += async delegate
            {
                isShowTEditor = false;
                var task = new TEditorResponse() { IsSave = true, HTML = await tvc.GetHTML() };
                taskRes.SetResult(task);
                //edittedText = task.HTML;
                DismissModalViewController(true);
            };
            BT_RichTextback.TouchUpInside += delegate
            {
                DismissModalViewController(true);
            };
            return taskRes.Task;
        }
        #endregion

        #region order Handle
        public void UpdateMultiUserColletionView(List<BeanUserAndGroup> _lst_selected, int lines)
        {
            string str_assigedTo = "";
            if (_lst_selected != null && _lst_selected.Count > 0)
            {
                lst_currentUserAndGroup = _lst_selected;

                foreach (var us in lst_currentUserAndGroup)
                {
                    str_assigedTo += us.Name + "; ";
                }

                tf_user.Text = str_assigedTo.TrimEnd(' ').TrimEnd(';');
                tf_user.Font = UIFont.FromName("Arial", 14f);
                tf_user.TextColor = UIColor.Black;
            }
            else
            {
                if (lst_currentUserAndGroup != null)
                {
                    lst_currentUserAndGroup.Clear();
                    tf_user.Text = CmmFunction.GetTitle("TEXT_HINT_USER_EMAIL", "Vui lòng nhập tên hoặc địa chỉ email...");
                    tf_user.Font = UIFont.FromName("Arial-ItalicMT", 14f);
                    tf_user.TextColor = UIColor.FromRGB(153, 153, 153);
                }
            }
        }
        public void HandleMenuOptionResult(ClassMenu _menu)
        {
            Custom_MenuOption custom_menuOption = Custom_MenuOption.Instance;
            if (_menu != null)
            {
                _menu.isSelected = true;

                if (custom_menuOption.InputView == tf_status)
                {
                    if (currentStatusItem != null && currentStatusItem.ID != _menu.ID)
                    {
                        currentStatusItem.isSelected = false;
                        currentStatusItem = _menu;
                    }
                    else
                        currentStatusItem = _menu;

                    tf_status.Text = currentStatusItem.title;
                }
                else
                {
                    if (currentStatusItem != null && currentStatusItem.ID != _menu.ID)
                    {
                        currentStatusItem.isSelected = false;
                        currentStatusItem = _menu;
                    }
                    else
                        currentStatusItem = _menu;

                    tf_status.Text = currentStatusItem.title;
                }
            }

            //if (custom_menuOption.Superview != null)
            //    custom_menuOption.RemoveFromSuperview();

            EnableInputView(true);
        }
        public void NavigateToSubTask(BeanTask _task)
        {
            FormTaskDetails taskDetails = (FormTaskDetails)Storyboard.InstantiateViewController("FormTaskDetails");
            taskDetails.SetContent(_task, beanWorkflowItem, this);
            this.NavigationController.PushViewController(taskDetails, true);
        }
        #endregion

        #region handle Attachment
        public void HandleAddAttachment(bool _isComment)
        {
            try
            {
                this.View.EndEditing(true);
                view_content.AddSubview(attachFileView);
                attachFileView.isComment = _isComment;
                attachFileView.Frame = new CGRect(view_content.Frame.Right, 0, view_content.Frame.Width, view_content.Frame.Height);
                UIView.BeginAnimations("show_animationShowTable");
                UIView.SetAnimationDuration(0.3f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);
                attachFileView.Frame = new CGRect(0, 0, view_content.Frame.Width, view_content.Frame.Height);
                UIView.CommitAnimations();
                //SetViewByPermission();

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

            if (!attachFileView.isComment)
            {
                UpdateBTActionLayout();
                SetViewByPermission();
            }
        }
        public void HandleAddAttachFileResult(BeanAttachFileLocal _attachFile)
        {
            if (attachFileView.isComment)
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
                    //WorkflowId = workflowItem.WorkflowID,
                    //WorkflowItemId = int.Parse(workflowItem.ID)
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

                if (attachFile.IsImage && lst_attachImage.Count == 1)
                    heightComment += 55;
                else
                    heightComment += 35;
                SetContentSizeScroll();

                var jsonStringImage = JsonConvert.SerializeObject(lst_attachImage);
                var jsonStringDoc = JsonConvert.SerializeObject(lst_attachDoc);

                ObjectElementNote note1 = new ObjectElementNote { Key = "image", Value = jsonStringImage };
                ObjectElementNote note2 = new ObjectElementNote { Key = "doc", Value = jsonStringDoc };

                List<ObjectElementNote> objectElementNotes = new List<ObjectElementNote>();
                objectElementNotes.Add(note1); objectElementNotes.Add(note2);
                viewElement.Notes = objectElementNotes;
                table_comment.ReloadData();
            }
            else
            {
                //lst_addAttachment = new List<BeanAttachFile>();

                //numRowAttachmentFile++;
                string custID = numRowAttachmentFile + "";
                BeanAttachFile attachFile = new BeanAttachFile()
                {
                    ID = "",
                    Title = _attachFile.Name + ";#" + DateTime.Now.ToShortTimeString(),
                    Path = _attachFile.Path,
                    Size = _attachFile.Size,
                    Category = "",
                    IsAuthor = true,
                    CreatedBy = CmmVariable.SysConfig.UserId,
                    CreatedName = CmmVariable.SysConfig.DisplayName,
                    CreatedPositon = CmmVariable.SysConfig.PositionTitle,
                    AttachTypeId = null,
                    AttachTypeName = "",
                    //WorkflowId = currentItemSelected.WorkflowID,
                    //WorkflowItemId = int.Parse(currentItemSelected.ID)
                };
                lst_addAttachFile.Add(attachFile);
                lst_currentAttach.Add(attachFile);
                LoadAttachments();
            }
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
                            HandleAddAttachFileResult(itemiCloudAndDevice);

                            docPicker.DismissModalViewController(false);
                            HandleAttachFileClose();
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

            this.PresentViewController(imagePicker, true, null);
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

            // dismiss the picker
            imagePicker.DismissModalViewController(false);

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

                        //BeanAttachFileLocal itemiCloudAndDevice = new BeanAttachFileLocal() { ID = -1, Name = fileName[fileName.Length - 1], Path = filePath.Path, Size = size, IsImage = true };
                        BeanAttachFileLocal itemiCloudAndDevice = new BeanAttachFileLocal() { ID = -1, Name = fileName[fileName.Length - 1], Path = filePath.Path, Size = size, IsImage = true };
                        HandleAddAttachFileResult(itemiCloudAndDevice);
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
                        HandleAddAttachFileResult(itemiCloudAndDevice);
                    }
                }
            }
            else
                CmmIOSFunction.AlertUnsupportFile(this, true);

            //var vc = this.PresentedViewController;
            //vc.DismissViewController(true, null);
            HandleAttachFileClose();
        }

        private void Handle_Canceled(object sender, EventArgs e)
        {
            imagePicker.DismissModalViewController(true);
        }

        public void HandleAttachmentRemove(BeanAttachFile beanAttachRemove)
        {
            lst_removeAttachFile.Add(beanAttachRemove);
            foreach (var item in lst_removeAttachFile)
            {
                lst_currentAttach.Remove(item);
            }

            LoadAttachments();
        }

        public void HandleAttachmentEdit(ViewElement element, NSIndexPath indexPath, BeanAttachFile _attach)
        {
            //FormEditAttachFileView formEditAttach = (FormEditAttachFileView)Storyboard.InstantiateViewController("FormEditAttachFileView");
            //formEditAttach.SetContent(this, _attach);
            //CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            //CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            //PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            //formEditAttach.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            //formEditAttach.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            //formEditAttach.TransitioningDelegate = transitioningDelegate;
            //this.PresentViewControllerAsync(formEditAttach, true);

            FormListItemView formListItem = (FormListItemView)Storyboard.InstantiateViewController("FormListItemView");
            formListItem.setContent(this, _attach, element, false);
            this.NavigationController.PushViewController(formListItem, true);
        }

        public void ReloadAttachmentElement(ViewElement _element, BeanAttachFile attachFile)
        {
            //List<BeanAttachFile> lst_attachFile = new List<BeanAttachFile>();
            //if (!string.IsNullOrEmpty(_element.Value))
            //{
            //    JArray json = JArray.Parse(_element.Value);
            //    lst_attachFile = json.ToObject<List<BeanAttachFile>>();
            //}

            //var index = lst_attachFile.FindIndex(item => item.ID == attachFile.ID);
            //if (index != -1)
            //    lst_attachFile[index] = attachFile;

            //var jsonString = JsonConvert.SerializeObject(lst_attachFile);
            //_element.Value = jsonString;

            //table_content.ReloadData();
        }
        public void NavigateToShowAttachView(BeanAttachFile currentAttachFile)
        {
            if (parent != null && parent.GetType() == typeof(RequestDetailsV2))
            {
                if (parent.GetType() == typeof(RequestDetailsV2))
                {
                    RequestDetailsV2 requestDetailsV2 = (RequestDetailsV2)parent;
                    requestDetailsV2.NavigateToAttachView(currentAttachFile);
                }
            }
            else if (parent != null && parent.GetType() == typeof(MainView))
            {
                if (parent.GetType() == typeof(MainView))
                {
                    MainView mainView = (MainView)parent;
                    mainView.NavigateToAttachView(currentAttachFile);
                }
            }
            else if (parent != null && parent.GetType() == typeof(RequestListView))
            {
                if (parent.GetType() == typeof(RequestListView))
                {
                    RequestListView requestListView = (RequestListView)parent;
                    requestListView.NavigateToAttachView(currentAttachFile);
                }
            }
            else if (parent != null && parent.GetType() == typeof(MyRequestListView))
            {
                if (parent.GetType() == typeof(MyRequestListView))
                {
                    MyRequestListView myRequestListView = (MyRequestListView)parent;
                    myRequestListView.NavigateToAttachView(currentAttachFile);
                }
            }
            //app
            //else if (parent != null && parent.GetType() == typeof(MainViewApp))
            //{
            //    if (parent.GetType() == typeof(MainViewApp))
            //    {
            //        MainViewApp mainViewApp = (MainViewApp)parent;
            //        mainViewApp.NavigateToAttachView(currentAttachFile);
            //    }
            //}
            //else if (parent != null && parent.GetType() == typeof(RequestListViewApp))
            //{
            //    if (parent.GetType() == typeof(RequestListViewApp))
            //    {
            //        RequestListViewApp requestListViewApp = (RequestListViewApp)parent;
            //        requestListViewApp.NavigateToAttachView(currentAttachFile);
            //    }
            //}
            //else if (parent != null && parent.GetType() == typeof(MyRequestListViewApp))
            //{
            //    if (parent.GetType() == typeof(MyRequestListViewApp))
            //    {
            //        MyRequestListViewApp myRequestListViewApp = (MyRequestListViewApp)parent;
            //        myRequestListViewApp.NavigateToAttachView(currentAttachFile);
            //    }
            //}
        }

        #endregion

        #region handle comment
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
            viewElement = element;
            table_comment.ReloadData();
        }
        //thuyngo add
        public void ReLoadDataFromServer()
        {
            LoadTaskDetails();
        }
        public void SubmitLikeAction(ViewElement element, NSIndexPath sectionIndex, BeanComment comment)
        {
            LoadTaskDetails();
        }
        //Comment - reply
        public void NavigateToReplyComment(NSIndexPath _itemIndex, BeanComment comment)
        {
            FormCommentView formComment = (FormCommentView)Storyboard.InstantiateViewController("FormCommentView");
            formComment.SetContent(this, workflowItem, comment, OtherResourceId, _itemIndex);
            this.NavigationController.PushViewController(formComment, true);
        }
        #endregion

        #region handle Attachment comment
        ViewElement attachmentElement;
        AddAttachmentsView addAttachmentsView { get; set; }
        public void HandleAddAttachmentComent(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
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
        #endregion

        #region handle DateTime choice
        public void HandleDateTimeChoiceChoice(ViewElement element)
        {
            tf_date.Text = element.Value;
        }
        #endregion
        #endregion

        #region event
        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            if (parent != null)
            {
                if (parent is FollowListViewController)
                    (parent as FollowListViewController).LoadData();
                else if (parent is RequestListView)
                    (parent as RequestListView).isBackWithoutEditing = true;
                else if (parent is MyRequestListView)
                    (parent as MyRequestListView).isBackWithoutEditing = true;
                else if (parent is MainView)
                    (parent as MainView).isBackWithoutEditing = true;
            }

            this.NavigationController.PopViewController(true);
        }
        private async void BT_approve_TouchUpInside(object sender, EventArgs e)
        {
            Handle_SubmitAction(false);
        }
        private void BT_details_TouchUpInside(object sender, EventArgs e)
        {
            TouchDetail();
        }

        private void TouchDetail()
        {
            if (currentIndexDetails != 0)
            {
                scrollview_details.Hidden = false;
                view_subtask.Hidden = true;
                currentIndexDetails = 0;

                line_details.Hidden = false;
                line_subtask.Hidden = true;

                SetContentSizeScroll();
                HightLightText(lbl_details, true);
                HightLightText(lbl_subtask, false);
            }
        }
        private void BT_subtask_TouchUpInside(object sender, EventArgs e)
        {
            TouchSubtask();
        }

        private void TouchSubtask()
        {
            if (lst_childTask != null && lst_childTask.Count > 0)
            {
                if (currentIndexDetails != 1)
                {
                    //scrollview_details.Hidden = true;
                    view_subtask.Hidden = false;
                    LoadSubtask();
                    currentIndexDetails = 1;

                    line_details.Hidden = true;
                    line_subtask.Hidden = false;

                    SetContentSizeScroll();
                    HightLightText(lbl_details, false);
                    HightLightText(lbl_subtask, true);
                }
            }
        }
        void Txt_Note_Ended(object sender, EventArgs e)
        {
        }
        void Txt_Note_Started(object sender, EventArgs e)
        {
        }
        private void BT_note_TouchUpInside(object sender, EventArgs e)
        {
            ShowRichTextEditor(true);
        }
        private void BT_contentMore_TouchUpInside(object sender, EventArgs e)
        {
            ShowRichTextEditor(false);
        }

        private void BT_startDate_TouchUpInside(object sender, EventArgs e)
        {
            PresentationDelegate transitioningDelegate;
            CGRect startFrame = new CGRect(this.View.Frame.Width / 2, this.View.Frame.Height / 2, 0, 0);
            CGSize showSize = new CGSize(384, 450);

            ViewElement element = new ViewElement();
            if (string.IsNullOrEmpty(tf_date.Text))
            {
                if (CmmVariable.SysConfig.LangCode == "1033")
                    element.Title = DateTime.Now.ToString(@"MM/dd/yy HH:mm", new CultureInfo("en"));
                else //if (CmmVariable.SysConfig.LangCode == "1066")
                    element.Title = DateTime.Now.ToString(@"dd/MM/yy HH:mm", new CultureInfo("vi"));
            }
            else
                element.Title = tf_date.Text;
            element.DataSource = "";
            element.Value = element.Title;
            element.DataType = "datetime";

            FormCalendarChoice formCalendarChoice = (FormCalendarChoice)Storyboard.InstantiateViewController("FormCalendarChoice");
            formCalendarChoice.setContent(this, element);
            transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            formCalendarChoice.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            formCalendarChoice.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            formCalendarChoice.TransitioningDelegate = transitioningDelegate;
            this.PresentViewControllerAsync(formCalendarChoice, true);

            //Custom_DateTimePickerView custom_dateTimePicker = Custom_DateTimePickerView.Instance;
            //if (custom_dateTimePicker.Superview != null && custom_dateTimePicker.inputView == tf_date)
            //{
            //    custom_dateTimePicker.RemoveFromSuperview();
            //    EnableInputView(true);
            //}
            //else
            //{
            //    custom_dateTimePicker.viewController = this;
            //    custom_dateTimePicker.inputView = tf_date;
            //    custom_dateTimePicker.InitFrameView(new CGRect(view_date.Frame.X, view_date.Frame.Bottom + 5, view_user.Frame.Width, 168));
            //    custom_dateTimePicker.AddShadowForView();
            //    custom_dateTimePicker.SetUpDate();

            //    scrollview_details.AddSubview(custom_dateTimePicker);
            //    scrollview_details.BringSubviewToFront(custom_dateTimePicker);

            //    EnableInputView(false);
            //}
        }
        private void BT_user_TouchUpInside(object sender, EventArgs e)
        {
            var dataSource = JsonConvert.SerializeObject(lst_currentUserAndGroup);

            ViewElement element = new ViewElement();
            element.Title = CmmFunction.GetTitle("TEXT_USER_PROCESS", "Người xử lý");
            element.DataSource = dataSource;
            element.Value = dataSource;
            element.DataType = "selectusergroupmulti";

            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            ListUserOrGroupView listUserOrGroupView = (ListUserOrGroupView)Storyboard.InstantiateViewController("ListUserOrGroupView");
            listUserOrGroupView.setContent(this, true, lst_currentUserAndGroup, false, element, CmmFunction.GetTitle("TEXT_USER_PROCESS", "Người xử lý"));
            PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            listUserOrGroupView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            listUserOrGroupView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            listUserOrGroupView.TransitioningDelegate = transitioningDelegate;
            this.PresentViewControllerAsync(listUserOrGroupView, true);
        }
        private void BT_moreUser_TouchUpInside(object sender, EventArgs e)
        {
            FullTextView fullTextView = (FullTextView)Storyboard.InstantiateViewController("FullTextView");
            fullTextView.SetContent(CmmFunction.GetTitle("TEXT_USER_PROCESS_REQUIRE", "Chọn người xử lý").Replace("(*)", ""), tf_user.Text);
            this.NavigationController.PushViewController(fullTextView, true);
        }
        private void BT_status_TouchUpInside(object sender, EventArgs e)
        {
            //Custom_MenuOption custom_menuOption = Custom_MenuOption.Instance;
            //if (custom_menuOption.Superview != null)
            //    custom_menuOption.RemoveFromSuperview();
            //else
            //{
            //    custom_menuOption.ItemNoIcon = false;
            //    custom_menuOption.viewController = this;
            //    custom_menuOption.InitFrameView(new CGRect(BT_status.Frame.Left, BT_status.Frame.Bottom + 2, BT_status.Frame.Width, lst_menuItem.Count * custom_menuOption.RowHeigth));
            //    custom_menuOption.AddShadowForView();
            //    custom_menuOption.lst_menu = lst_menuItem;
            //    custom_menuOption.TableLoadData();

            //    scrollview_details.AddSubview(custom_menuOption);
            //    scrollview_details.BringSubviewToFront(custom_menuOption);
            //}
            ToggleMenuStatus();
        }
        private void ToggleMenuStatus()
        {
            Custom_MenuOption custom_menuOption = Custom_MenuOption.Instance;
            custom_menuOption.ItemNoIcon = false;
            custom_menuOption.viewController = this;
            custom_menuOption.lst_menu = lst_menuItem;
            custom_menuOption.InputView = tf_status;
            custom_menuOption.TableLoadData();
            custom_menuOption.BackupData();

            MultiChoiceTableView multiChoiceTableView = Storyboard.InstantiateViewController("MultiChoiceTableView") as MultiChoiceTableView;
            multiChoiceTableView.setContent(this, false, custom_menuOption, tf_status.Text);
            this.NavigationController.PushViewController(multiChoiceTableView, true);
        }

        public void HandleAddUserAndGroupResult(List<BeanUser> _users)
        {
            if (_users.Count > 0)
            {
                foreach (var item in _users)
                {
                    lst_selectedUser.Add(item);
                }
            }

            //table_selectedUser.ReloadData();
            tf_user.Text = GetStringUsers();
        }
        private void KeyBoardUpNotification(NSNotification notification)
        {
            try
            {
                CGRect keyboardSize = UIKeyboard.BoundsFromNotification(notification);
                if (isShowTEditor)
                {
                    tvc.View.Subviews[0].Frame = new CGRect(0, 70, tvc.View.Frame.Width, tvc.View.Subviews[0].Frame.Height + 100);
                }
                else
                {
                    if (View.Frame.Y == 0)
                    {
                        var topKeybroad = this.View.Frame.Height - keyboardSize.Height;
                        if (topKeybroad < positionBotOfCurrentViewInput)
                        {
                            scrollview_details.ContentSize = new CGSize(scrollview_details.Frame.Width, scrollview_details.ContentSize.Height + keyboardSize.Height);
                            var topKeybroad1 = lbl_line.Frame.Y - (scrollview_details.Frame.Height - 140 - keyboardSize.Height);
                            scrollview_details.SetContentOffset(new CGPoint(0, topKeybroad1), false);
                        }

                    }

                }
            }
            catch (Exception ex)
            { Console.WriteLine("FormCreateView - Err: " + ex.ToString()); }
        }
        private void KeyBoardDownNotification(NSNotification notification)
        {
            try
            {
                CGRect keyboardSize = UIKeyboard.BoundsFromNotification(notification);
                if (isShowTEditor)
                {
                    tvc.View.Subviews[0].Frame = new CGRect(0, 70, tvc.View.Frame.Width, tvc.View.Subviews[0].Frame.Height - 100);
                    isShowTEditor = false;
                }
                else
                {
                    if (View.Frame.Y != 0)
                    {
                        scrollview_details.ContentSize = new CGSize(scrollview_details.Frame.Width, scrollview_details.ContentSize.Height - keyboardSize.Height);
                    }
                }
            }
            catch (Exception ex)
            { Console.WriteLine("FormCreateView - Err: " + ex.ToString()); }
        }
        private void BT_addAttachment_TouchUpInside(object sender, EventArgs e)
        {
            HandleAddAttachment(false);
        }
        private void BT_add_TouchUpInside(object sender, EventArgs e)
        {
            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            FormCreateTaskView createTaskView = (FormCreateTaskView)Storyboard.InstantiateViewController("FormCreateTaskView");
            createTaskView.SetContent(beanWorkflowItem, task, this);
            PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            createTaskView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            createTaskView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            createTaskView.TransitioningDelegate = transitioningDelegate;
            this.NavigationController.PushViewController(createTaskView, true);
        }
        private void BT_assign_TouchUpInside(object sender, EventArgs e)
        {
            FullTextView fullTextView = (FullTextView)Storyboard.InstantiateViewController("FullTextView");
            fullTextView.SetContent(CmmFunction.GetTitle("TEXT_ASSIGNORS", "Người giao"), lbl_assignName.Text);
            this.NavigationController.PushViewController(fullTextView, true);
        }
        private void BT_moreTitle_TouchUpInside(object sender, EventArgs e)
        {
            FullTextView fullTextView = (FullTextView)Storyboard.InstantiateViewController("FullTextView");
            fullTextView.SetContent(CmmFunction.GetTitle("TEXT_TITLE_REQUIRE", "Tiêu đề").Replace("(*)", ""), lbl_assignName.Text);
            this.NavigationController.PushViewController(fullTextView, true);
        }
        private void BT_delete_TouchUpInside(object sender, EventArgs e)
        {
            //UIAlertView alert_signOut = new UIAlertView();
            //alert_signOut.Title = "BPM OP";
            //alert_signOut.Message = CmmFunction.GetTitle("TEXT_DELETE_CONFIRM_TASK", "Bạn có chắc muốn xóa task này không?");
            //alert_signOut.AddButton(CmmFunction.GetTitle("TEXT_DELETE", "Xóa"));
            //alert_signOut.AddButton(CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"));
            //alert_signOut.Clicked += alert_signOut_Clicked;
            //alert_signOut.Show();

            UIAlertController alert = UIAlertController.Create("Thông báo", CmmFunction.GetTitle("TEXT_DELETE_CONFIRM_TASK", "Bạn có chắc muốn xóa task này không?"), UIAlertControllerStyle.Alert);//"BPM"
            alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"), UIAlertActionStyle.Default, null));
            alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_AGREE", "Đồng ý"), UIAlertActionStyle.Default, alertAction =>
            {
                Handle_RemoveTask();
            }));
            this.PresentViewController(alert, true, null);

        }
        //private void alert_signOut_Clicked(object sender, UIButtonEventArgs e)
        //{
        //    try
        //    {
        //        if (e.ButtonIndex == 0)
        //            Handle_RemoveTask();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("MenuView - alert_signOut_Clicked - Err: " + ex.ToString());
        //    }
        //}
        private void BT_complete_TouchUpInside(object sender, EventArgs e)
        {
            //UIAlertView alert_complete = new UIAlertView();
            //alert_complete.Title = "BPM OP";
            //alert_complete.Message = CmmFunction.GetTitle("TEXT_COMPLETE_CONFIRM_TASK", "Bạn có chắc muốn hoàn tất công việc này không?");
            //alert_complete.AddButton(CmmFunction.GetTitle("TEXT_COMPLETED", "Hoàn tất"));
            //alert_complete.AddButton(CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"));
            //alert_complete.Clicked += Alert_complete_Clicked; ;
            //alert_complete.Show();

            UIAlertController alert = UIAlertController.Create("Thông báo", CmmFunction.GetTitle("TEXT_COMPLETE_CONFIRM_TASK", "Bạn có chắc muốn hoàn tất công việc này không?"), UIAlertControllerStyle.Alert);//"BPM"
            alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"), UIAlertActionStyle.Default, null));
            alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_COMPLETED", "Hoàn tất"), UIAlertActionStyle.Default, alertAction =>
            {
                Handle_SubmitAction(true);
            }));
            this.PresentViewController(alert, true, null);
        }
        //private void Alert_complete_Clicked(object sender, UIButtonEventArgs e)
        //{
        //    try
        //    {
        //        if (e.ButtonIndex == 0)
        //            Handle_SubmitAction(true);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("MenuView - alert_signOut_Clicked - Err: " + ex.ToString());
        //    }
        //}
        #endregion

        #region custom class
        #region attachment source table
        private class Attachment_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellAttachmentID");
            FormTaskDetails parentView;
            List<BeanAttachFile> lst_attachment = new List<BeanAttachFile>();
            List<string> sectionKeys;

            public Attachment_TableSource(List<BeanAttachFile> _lst_attachment, FormTaskDetails _parentview)
            {
                lst_attachment = _lst_attachment;
                parentView = _parentview;
                LoadData();
            }

            private void LoadData()
            {

            }

            public override nfloat GetHeightForFooter(UITableView tableView, nint section)
            {
                return -1;
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return 1;
            }


            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 60;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return lst_attachment.Count;
            }

            public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
            {
                //if (lst_workRelated.Count > 0)
                //{
                //    var item = lst_workRelated[indexPath.Row];
                //    parentView.HandleRemoveItem(item);
                //}
            }

            public override UISwipeActionsConfiguration GetTrailingSwipeActionsConfiguration(UITableView tableView, NSIndexPath indexPath)
            {
                var flagAction = ContextualFlagAction(indexPath.Row);
                var trailingSwipe = UISwipeActionsConfiguration.FromActions(new UIContextualAction[] { flagAction });

                trailingSwipe.PerformsFirstActionWithFullSwipe = false;
                return trailingSwipe;
            }

            // delete item
            public UIContextualAction ContextualFlagAction(int row)
            {
                var action = UIContextualAction.FromContextualActionStyle(UIContextualActionStyle.Normal,
                                                                          "",
                                                                          (FlagAction, view, success) =>
                                                                          {
                                                                              if (lst_attachment.Count > 0)
                                                                              {
                                                                                  var item = lst_attachment[row];
                                                                                  parentView.HandleAttachmentRemove(item);

                                                                              }
                                                                              success(true);
                                                                          });

                action.Image = UIImage.FromFile("Icons/icon_swipe_delete_white.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal).Scale(new CGSize(20, 20), 3);
                action.BackgroundColor = UIColor.FromRGB(235, 52, 46);
                return action;
            }

            public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
            {
                var attachment = lst_attachment[indexPath.Row];

                if (attachment.IsAuthor == true || attachment.CreatedBy == CmmVariable.SysConfig.UserId)
                    return true;
                else
                    return false;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                var itemSelected = lst_attachment[indexPath.Row];
                parentView.NavigateToShowAttachView(itemSelected);
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var attachment = lst_attachment[indexPath.Row];

                bool isOdd = true;
                if (indexPath.Row % 2 == 0)
                    isOdd = false;

                Attachment_cell_custom cell = new Attachment_cell_custom(parentView, cellIdentifier, attachment, indexPath, isOdd);
                return cell;
            }
        }
        private class Attachment_cell_custom : UITableViewCell
        {
            string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            FormTaskDetails parentView { get; set; }
            NSIndexPath currentIndexPath { get; set; }
            BeanAttachFile attachment { get; set; }
            UILabel lbl_title, lbl_creator, lbl_size, lbl_chucvu;
            UIImageView img_type;
            bool isOdd;

            public Attachment_cell_custom(FormTaskDetails _parentView, NSString cellID, BeanAttachFile _attachment, NSIndexPath _currentIndexPath, bool _isOdd) : base(UITableViewCellStyle.Default, cellID)
            {
                parentView = _parentView;
                attachment = _attachment;
                currentIndexPath = _currentIndexPath;
                Accessory = UITableViewCellAccessory.None;
                isOdd = _isOdd;
                viewConfiguration();
                UpdateCell();
            }

            private void viewConfiguration()
            {
                if (!isOdd)
                    ContentView.BackgroundColor = UIColor.White;
                else
                    ContentView.BackgroundColor = UIColor.FromRGB(243, 249, 255);


                img_type = new UIImageView();
                img_type.ContentMode = UIViewContentMode.ScaleAspectFill;

                lbl_title = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 14f),
                    TextColor = UIColor.FromRGB(59, 95, 179),
                    TextAlignment = UITextAlignment.Left,
                };
                lbl_creator = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 14f),
                    TextColor = UIColor.Black,
                    TextAlignment = UITextAlignment.Left,
                };

                lbl_size = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 11f),
                    TextColor = UIColor.FromRGB(153, 153, 153)
                };

                lbl_chucvu = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 11f),
                    TextColor = UIColor.FromRGB(153, 153, 153)
                };

                ContentView.AddSubviews(new UIView[] { img_type, lbl_title, lbl_creator, lbl_size, lbl_chucvu });
            }

            public void UpdateCell()
            {
                try
                {
                    string fileExt = string.Empty;
                    if (!string.IsNullOrEmpty(attachment.Path))
                        fileExt = attachment.Path.Split('.').Last().ToLower();

                    switch (fileExt)
                    {
                        case "doc":
                        case "docx":
                            img_type.Image = UIImage.FromFile("Icons/icon_word.png");
                            break;
                        case "txt":
                            img_type.Image = UIImage.FromFile("Icons/icon_attachFile_txt.png");
                            break;
                        case "pdf":
                            img_type.Image = UIImage.FromFile("Icons/icon_pdf.png");
                            break;
                        case "xls":
                        case "xlsx":
                            img_type.Image = UIImage.FromFile("Icons/icon_xlsx.png");
                            break;
                        case "jpg":
                            img_type.Image = UIImage.FromFile("Icons/icon_image.png");
                            break;
                        case "png":
                            img_type.Image = UIImage.FromFile("Icons/icon_image.png");
                            break;
                        case "jpeg":
                            img_type.Image = UIImage.FromFile("Icons/icon_image.png");
                            break;
                        default:
                            img_type.Image = UIImage.FromFile("Icons/icon_file_blank.png");
                            break;
                    }

                    //title
                    if (attachment.Title.Contains(";#"))
                        lbl_title.Text = attachment.Title.Split(";#")[0];
                    else
                        lbl_title.Text = attachment.Title;

                    //CreatedBy
                    if (!string.IsNullOrEmpty(attachment.CreatedBy))
                    {
                        List<BeanUser> lst_userResult = new List<BeanUser>();
                        var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath, false);
                        string query_user = string.Format("SELECT * FROM BeanUser WHERE ID =?");
                        lst_userResult = conn.Query<BeanUser>(query_user, attachment.CreatedBy);

                        string user_imagePath = "";
                        if (lst_userResult.Count > 0)
                        {
                            user_imagePath = lst_userResult[0].ImagePath;
                            lbl_creator.Text = lst_userResult[0].FullName;
                            lbl_chucvu.Text = lst_userResult[0].Position;
                        }
                    }
                    else
                    {
                    }

                    lbl_size.Text = CmmFunction.FileSizeFormatter.FormatSize(attachment.Size);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("attachment_cell_custom - UpdateCell - ERR: " + ex.ToString());
                }
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                var width = ContentView.Frame.Width;

                img_type.Frame = new CGRect(13, 10, 25, 25);
                lbl_title.Frame = new CGRect(img_type.Frame.Right + 11, 14, ((width - 60) / 3) * 2, 16);
                lbl_creator.Frame = new CGRect(lbl_title.Frame.Right + 10, 14, ((width - 60) / 3), 20);
                lbl_size.Frame = new CGRect(lbl_title.Frame.X, lbl_title.Frame.Bottom + 5, lbl_title.Frame.Width, 13);
                lbl_chucvu.Frame = new CGRect(lbl_creator.Frame.X, lbl_creator.Frame.Bottom + 5, lbl_creator.Frame.Width, 13);
            }
        }
        //private class Attachment_cell_custom : UITableViewCell
        //{
        //    string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        //    FormTaskDetails parentView { get; set; }
        //    NSIndexPath currentIndexPath { get; set; }
        //    BeanAttachFile attachment { get; set; }
        //    UILabel lbl_title, lbl_creator, lbl_CreatorCover, lbl_size, lbl_chucvu, lbl_line;
        //    UIImageView img_type, img_Creator;
        //    bool isOdd;

        //    public Attachment_cell_custom(FormTaskDetails _parentView, NSString cellID, BeanAttachFile _attachment, NSIndexPath _currentIndexPath, bool _isOdd) : base(UITableViewCellStyle.Default, cellID)
        //    {
        //        parentView = _parentView;
        //        attachment = _attachment;
        //        currentIndexPath = _currentIndexPath;
        //        Accessory = UITableViewCellAccessory.None;
        //        isOdd = _isOdd;
        //        viewConfiguration();
        //        UpdateCell();
        //    }

        //    private void viewConfiguration()
        //    {
        //        if (!isOdd)
        //            ContentView.BackgroundColor = UIColor.White;
        //        else
        //            ContentView.BackgroundColor = UIColor.FromRGB(249, 249, 249);


        //        img_type = new UIImageView();
        //        img_type.ContentMode = UIViewContentMode.ScaleAspectFill;

        //        img_Creator = new UIImageView();
        //        img_Creator.ContentMode = UIViewContentMode.ScaleAspectFill;

        //        lbl_title = new UILabel()
        //        {
        //            Font = UIFont.FromName("ArialMT", 14f),
        //            TextColor = UIColor.FromRGB(59, 95, 179),
        //            TextAlignment = UITextAlignment.Left,
        //        };

        //        lbl_CreatorCover = new UILabel();
        //        lbl_CreatorCover.Layer.CornerRadius = 20;
        //        lbl_CreatorCover.ClipsToBounds = true;

        //        lbl_creator = new UILabel()
        //        {
        //            Font = UIFont.FromName("ArialMT", 14f),
        //            TextColor = UIColor.Black,
        //            TextAlignment = UITextAlignment.Left,
        //        };

        //        lbl_size = new UILabel()
        //        {
        //            Font = UIFont.FromName("ArialMT", 11f),
        //            TextColor = UIColor.FromRGB(153, 153, 153)
        //        };

        //        lbl_chucvu = new UILabel()
        //        {
        //            Font = UIFont.FromName("ArialMT", 11f),
        //            TextColor = UIColor.FromRGB(153, 153, 153)
        //        };

        //        lbl_line = new UILabel()
        //        {
        //            BackgroundColor = UIColor.FromRGB(144, 164, 174)
        //        };

        //        ContentView.AddSubviews(new UIView[] { img_type, img_Creator, lbl_title, lbl_CreatorCover, lbl_creator, lbl_size, lbl_chucvu, lbl_line });
        //        lbl_line.Hidden = true;
        //    }

        //    public void UpdateCell()
        //    {
        //        try
        //        {
        //            string fileExt = string.Empty;
        //            if (!string.IsNullOrEmpty(attachment.Path))
        //                fileExt = attachment.Path.Split('.').Last();

        //            switch (fileExt)
        //            {
        //                case "doc":
        //                case "docx":
        //                    img_type.Image = UIImage.FromFile("Icons/icon_word.png");
        //                    break;
        //                case "pdf":
        //                    img_type.Image = UIImage.FromFile("Icons/icon_pdf.png");
        //                    break;
        //                case "xls":
        //                case "xlsx":
        //                    img_type.Image = UIImage.FromFile("Icons/icon_xlsx.png");
        //                    break;
        //                case "jpg":
        //                    img_type.Image = UIImage.FromFile("Icons/icon_image.png");
        //                    break;
        //                case "png":
        //                    img_type.Image = UIImage.FromFile("Icons/icon_image.png");
        //                    break;
        //                case "jpeg":
        //                    img_type.Image = UIImage.FromFile("Icons/icon_image.png");
        //                    break;
        //                default:
        //                    img_type.Image = UIImage.FromFile("Icons/icon_file_blank.png");
        //                    break;
        //            }

        //            //title
        //            if (attachment.Title.Contains(";#"))
        //                lbl_title.Text = attachment.Title.Split(";#")[0];
        //            else
        //                lbl_title.Text = attachment.Title;

        //            //CreatedBy
        //            if (!string.IsNullOrEmpty(attachment.CreatedBy))
        //            {
        //                List<BeanUser> lst_userResult = new List<BeanUser>();
        //                var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath, false);
        //                string query_user = string.Format("SELECT * FROM BeanUser WHERE ID =?");
        //                lst_userResult = conn.Query<BeanUser>(query_user, attachment.CreatedBy);

        //                string user_imagePath = "";
        //                if (lst_userResult.Count > 0)
        //                {
        //                    user_imagePath = lst_userResult[0].ImagePath;
        //                    lbl_creator.Text = lst_userResult[0].FullName;
        //                    lbl_chucvu.Text = lst_userResult[0].Position;
        //                }

        //                if (string.IsNullOrEmpty(user_imagePath))
        //                {

        //                    lbl_CreatorCover.Hidden = false;
        //                    img_Creator.Hidden = true;
        //                    lbl_CreatorCover.Text = CmmFunction.GetAvatarName(lst_userResult[0].FullName);
        //                    lbl_CreatorCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_CreatorCover.Text));

        //                }
        //                else
        //                {
        //                    lbl_CreatorCover.Hidden = false;
        //                    img_Creator.Hidden = true;
        //                    lbl_CreatorCover.Text = CmmFunction.GetAvatarName(lst_userResult[0].FullName);
        //                    lbl_CreatorCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_CreatorCover.Text));
        //                    checkFileLocalIsExist(lst_userResult[0], img_Creator);
        //                    //kiem tra xong cap nhat lai avatar
        //                    lbl_CreatorCover.Hidden = true;
        //                    img_Creator.Hidden = false;
        //                }
        //            }
        //            else
        //            {
        //                img_Creator.Hidden = false;
        //                lbl_CreatorCover.Hidden = true;
        //            }

        //            lbl_size.Text = FileSizeFormatter.FormatSize(attachment.Size);
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("attachment_cell_custom - UpdateCell - ERR: " + ex.ToString());
        //        }
        //    }

        //    private async void checkFileLocalIsExist(BeanUser contact, UIImageView image_view)
        //    {
        //        try
        //        {
        //            string filename = contact.ImagePath.Split('/').Last();
        //            string filepathURL = CmmVariable.M_Domain + "/" + CmmVariable.SysConfig.Subsite + contact.ImagePath;
        //            string localfilePath = Path.Combine(localDocumentFilepath, filename);

        //            if (!File.Exists(localfilePath))
        //            {
        //                UIImage avatar = null;
        //                await Task.Run(() =>
        //                {
        //                    ProviderBase provider = new ProviderBase();
        //                    if (provider.DownloadFile(filepathURL, localfilePath, CmmVariable.M_AuthenticatedHttpClient))
        //                    {
        //                        NSData data = NSData.FromUrl(new NSUrl(localfilePath, false));

        //                        InvokeOnMainThread(() =>
        //                        {
        //                            if (data != null)
        //                            {
        //                                UIImage image = UIImage.LoadFromData(data);
        //                                if (image != null)
        //                                {
        //                                    avatar = CmmIOSFunction.MaxResizeImage(image, 200, 200);
        //                                    image_view.Image = avatar;
        //                                }
        //                                else
        //                                    image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
        //                            }
        //                            else
        //                                image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");

        //                            img_Creator.Hidden = false;
        //                        });

        //                        if (data != null && avatar != null)
        //                        {
        //                            NSError err = null;
        //                            NSData imgData = avatar.AsPNG();
        //                            if (imgData.Save(localfilePath, false, out err))
        //                                Console.WriteLine("saved as " + localfilePath);
        //                            return;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        InvokeOnMainThread(() =>
        //                        {
        //                            image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
        //                            img_Creator.Hidden = false;
        //                        });
        //                    }
        //                });
        //            }
        //            else
        //            {
        //                openFile(filename, image_view);
        //                img_Creator.Hidden = false;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
        //            Console.WriteLine("ListUserView - checkFileLocalIsExist - Err: " + ex.ToString());
        //            //CmmIOSFunction.IOSlog(null, "PopupContactDetailView - loadAvatar - " + ex.ToString());
        //        }
        //    }

        //    private async void openFile(string localfilename, UIImageView image_view)
        //    {
        //        try
        //        {
        //            NSData data = null;
        //            await Task.Run(() =>
        //            {
        //                string localfilePath = Path.Combine(localDocumentFilepath, localfilename);
        //                data = NSData.FromUrl(new NSUrl(localfilePath, false));
        //            });

        //            if (data != null)
        //            {
        //                UIImage image = UIImage.LoadFromData(data);
        //                if (image != null)
        //                {
        //                    image_view.Image = image;
        //                }
        //                else
        //                {
        //                    image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
        //                }
        //            }
        //            else
        //            {
        //                image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("ContactView - openFile - Err: " + ex.ToString());
        //        }
        //    }

        //    public override void LayoutSubviews()
        //    {
        //        base.LayoutSubviews();
        //        var width = ContentView.Frame.Width;

        //        img_type.Frame = new CGRect(20, 15, 18, 18);
        //        lbl_title.Frame = new CGRect(50, 13, ((width - 50) / 3) * 1.7f, 20);
        //        lbl_CreatorCover.Frame = new CGRect(lbl_title.Frame.Right, 10, 40, 40);
        //        img_Creator.Frame = new CGRect(lbl_title.Frame.Right + 10, 10, 40, 40);
        //        lbl_creator.Frame = new CGRect(img_Creator.Frame.Right + 5, 13, (width - lbl_title.Frame.Right) - 60, 20);
        //        lbl_size.Frame = new CGRect(lbl_title.Frame.X, lbl_title.Frame.Bottom, lbl_title.Frame.Width, 20);
        //        lbl_chucvu.Frame = new CGRect(lbl_creator.Frame.X, lbl_creator.Frame.Bottom, lbl_creator.Frame.Width, 20);
        //        lbl_line.TranslatesAutoresizingMaskIntoConstraints = false;
        //        lbl_line.HeightAnchor.ConstraintEqualTo(1).Active = true;
        //        NSLayoutConstraint.Create(this.lbl_line, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
        //        NSLayoutConstraint.Create(this.lbl_line, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;
        //        NSLayoutConstraint.Create(this.lbl_line, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, this, NSLayoutAttribute.Bottom, 1.0f, -1).Active = true;

        //        lbl_CreatorCover.Layer.CornerRadius = img_Creator.Frame.Width / 2;
        //        lbl_CreatorCover.ClipsToBounds = true;
        //        img_Creator.Layer.CornerRadius = img_Creator.Frame.Width / 2;
        //        img_Creator.ClipsToBounds = true;
        //    }
        //}
        #endregion

        #region Task source table
        private class Tasks_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellTaskID");
            FormTaskDetails parentView;
            List<BeanTask> lst_parent { get; set; }
            List<BeanTask> lst_task { get; set; }
            List<BeanTask> lst_lv1 { get; set; }
            List<BeanTask> sectionKeys;
            nfloat height = 73;

            public Tasks_TableSource(List<BeanTask> _lst_task, FormTaskDetails _parentview)
            {
                lst_task = _lst_task;
                parentView = _parentview;
                LoadData();
            }

            private void LoadData()
            {
                try
                {
                    lst_task = lst_task.OrderBy(i => i.Created).ThenBy(i => i.Created).ToList();
                    //lst_parent = lst_task.Where(i => i.Parent == 0).ToList();
                    lst_lv1 = lst_task.Where(i => i.Parent != 0).ToList();

                    if (lst_parent != null && lst_parent.Count > 0)
                    {
                        foreach (var parent in lst_parent)
                        {
                            List<BeanTask> childTasks = new List<BeanTask>();
                            foreach (var item in lst_lv1)
                            {
                                if (parent.ID == item.Parent)
                                {
                                    childTasks.Add(item);
                                }

                            }

                            LoadSubTaskData(parent);
                            parent.ChildTask = childTasks;
                        }
                    }
                    else
                    {
                        //lst_parent = lst_task;
                        //lst_lv1 = lst_task.Where(i => i.Parent != 0).ToList();
                        //foreach (var parent in lst_parent)
                        //{
                        //    List<BeanTask> childTasks = new List<BeanTask>();
                        //    foreach (var item in lst_lv1)
                        //    {
                        //        if (parent.ID == item.Parent)
                        //        {
                        //            childTasks.Add(item);
                        //        }
                        //    }
                        //    LoadSubTaskData(parent);
                        //    parent.ChildTask = childTasks;
                        //}
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Tasks_TableSource - LoadData - Err: " + ex.ToString());
                }
            }

            private void LoadSubTaskData(BeanTask task)
            {
                foreach (var item in lst_lv1)
                {
                    if (item.Parent == task.ID)
                    {
                        var childLvl2 = lst_lv1.Where(i => i.Parent == task.ID).ToList();
                        if (childLvl2 != null && childLvl2.Count > 0)
                        {
                            task.ChildTask = childLvl2;
                            foreach (var i2 in childLvl2)
                            {
                                LoadSubTaskData(i2);
                            }
                        }
                    }
                }
            }

            public override nfloat GetHeightForFooter(UITableView tableView, nint section)
            {
                return 1;
            }

            public override UIView GetViewForHeader(UITableView tableView, nint section)
            {
                if (lst_parent != null && lst_parent.Count > 0)
                {
                    var key = lst_parent[(int)section];

                    UIButton btn_action = new UIButton();
                    btn_action.TouchUpInside += delegate
                    {
                        //parentView.HandleSelectedItem(key);
                    };

                    CGRect frame = new CGRect(0, -10, tableView.Frame.Width, 70);
                    btn_action.Frame = frame;
                    Custom_TaskSessionHeader headerView = new Custom_TaskSessionHeader(frame, true);
                    headerView.LoadData(key, tableView, false, null);
                    headerView.InsertSubview(btn_action, 100);
                    return headerView;
                }
                else
                    return null;
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                if (lst_parent != null && lst_parent.Count > 0)
                    return 70;
                else
                    return 0;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {

                return 70;
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                if (lst_parent != null && lst_parent.Count > 0)
                    return lst_parent.Count;
                else
                    return 1;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                int numRow;
                if (lst_parent != null && lst_parent.Count > 0)
                    numRow = lst_parent[Convert.ToInt32(section)].ChildTask.Count;
                else
                    numRow = lst_task.Count;

                return numRow;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                var item = lst_task[indexPath.Row];
                parentView.NavigateToSubTask(item);
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                BeanTask task = new BeanTask();
                if (lst_parent != null && lst_parent.Count > 0)
                    task = lst_parent[indexPath.Section].ChildTask[indexPath.Row];
                else
                    task = lst_task[indexPath.Row];

                Custom_TaskCell cell = new Custom_TaskCell(cellIdentifier);
                cell.UpdateCell(task, null, false, false, "", null);// tam set lvl = 0
                return cell;
            }
        }
        #endregion
        #endregion




        #region dynamic controls source table
        private class Control_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cell");
            FormTaskDetails parentView;
            List<ViewRow> lst_viewRow = new List<ViewRow>();
            ViewElement element;

            public Control_TableSource(ViewElement _element, FormTaskDetails _parentview)
            {
                parentView = _parentview;
                element = _element;
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
                return parentView.heightComment;
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return 1;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return 1;
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
                Control_cell_custom cell = new Control_cell_custom(parentView, cellIdentifier, element);
                return cell;
            }
        }

        private class Control_cell_custom : UITableViewCell
        {
            FormTaskDetails parentView { get; set; }
            public ComponentBase components;
            ViewElement element;


            public Control_cell_custom(FormTaskDetails _parentView, NSString cellID, ViewElement _element) : base(UITableViewCellStyle.Default, cellID)
            {
                element = _element;
                parentView = _parentView;
                Accessory = UITableViewCellAccessory.None;
                viewConfiguration();
            }

            private void viewConfiguration()
            {
                ContentView.BackgroundColor = UIColor.White;
                components = new ControlInputComments(parentView, element, null);

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
                    Console.WriteLine("FormTaskDetails - Control_cell_custom - loaddata- ERR: " + ex.ToString());
                }
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                components.InitializeFrameView(new CGRect(0, 0, ContentView.Frame.Width, ContentView.Frame.Height));
            }
        }
        #endregion
    }
}


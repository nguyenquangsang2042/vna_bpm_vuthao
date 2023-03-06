using System;
using System.Collections.Generic;
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
using BPMOPMobile.iPad.ViewControllers;
using CoreGraphics;
using Foundation;
using MobileCoreServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite;
using UIKit;

namespace BPMOPMobile.iPad
{
    public partial class FormWorkFlowDetails : UIViewController
    {
        NSObject _willResignActiveNotificationObserver;
        NSObject _didBecomeActiveNotificationObserver;
        ButtonsActionTopBar buttonActionTopBar;
        UIViewController parentView { get; set; }
        public BeanWorkflowItem currentItemSelected { get; set; }
        CmmLoading loading;
        bool isFollow;
        bool isTask;
        public nfloat estCommmentViewRowHeight;
        string json_attachRemove;
        List<string> lst_userName = new List<string>();
        string str_json_FormDefineInfo = string.Empty;
        string[] arr_assignedTo;
        ComponentButtonBot componentButton;
        List<ButtonAction> lst_menuItem = new List<ButtonAction>();
        Dictionary<string, List<BeanWorkflowItem>> dict_workflow = new Dictionary<string, List<BeanWorkflowItem>>();
        Dictionary<string, string> dic_valueObject = new Dictionary<string, string>();
        List<BeanQuaTrinhLuanChuyen> lst_qtlc;
        string Json_FormDataString = string.Empty;
        List<BeanWorkFlowRelated> lstWorkFlowRelateds;
        List<BeanTask> lst_tasks;
        List<ViewSection> lst_section { get; set; }
        CGRect view_buttonAction_default;
        public List<BeanAttachFile> lst_addCommentAttachment;
        List<BeanAttachFile> lst_attachFile;
        List<BeanAttachFile> lst_addAttachment;
        nfloat origin_view_header_content_height_constant;
        bool isExpandUser;
        string localDocumentFilepath = string.Empty;
        List<KeyValuePair<string, string>> lstExtent = new List<KeyValuePair<string, string>>();
        List<BeanWorkflowItem> lst_workflow_fromMe;
        UIImagePickerController imagePicker;
        UIDocumentPickerViewController docPicker;
        string json_PropertyRemove;
        public List<JObject> lstGridDetail_Deleted = new List<JObject>(); // lưu lại những item nào đã bị xóa ra khỏi Control InputgridDetail
        JObject JObjectSource = new JObject(); // JObject những Element ko phải calculated
        // Comment
        List<BeanComment> lst_comments;
        public List<BeanAttachFile> _lstAttachComment = new List<BeanAttachFile>();
        DateTime _CommentChanged;
        public string _OtherResourceId = "";
        //Attachments
        int numRowAttachmentFile = 0;
        ViewElement attachmentElement;

        public FormWorkFlowDetails(IntPtr handle) : base(handle)
        {
            localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }

        #region override
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UITapGestureRecognizer gesture = new UITapGestureRecognizer(() =>
            {
                View.EndEditing(true);
                //HandleCloseAddFollow();
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

            LoadItemSelected();

            #region delegate
            BT_moreUser.TouchUpInside += BT_moreUser_TouchUpInside;
            BT_start.TouchUpInside += BT_start_TouchUpInside;
            BT_share.TouchUpInside += BT_share_TouchUpInside;
            BT_history.TouchUpInside += BT_history_TouchUpInside;
            BT_comment.TouchUpInside += BT_comment_TouchUpInside;
            BT_attachement.TouchUpInside += BT_attachement_TouchUpInside;
            BT_close.TouchUpInside += BT_close_TouchUpInside;
            #endregion
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            _willResignActiveNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, KeyBoardUpNotification);
            _didBecomeActiveNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyBoardDownNotification);
        }
        #endregion

        #region public - private method
        public void SetContent(BeanWorkflowItem _currentSelected, bool _isTask, UIViewController parent)
        {
            currentItemSelected = _currentSelected;
            isTask = _isTask;
            parentView = parent;
        }

        private void ViewConfiguration()
        {
            buttonActionTopBar = ButtonsActionTopBar.Instance;
            view_buttonAction_default = view_buttonAction.Frame;
            buttonActionTopBar.InitFrameView(top_view.Frame);
            origin_view_header_content_height_constant = view_header_content_height_constant.Constant;

            //CmmIOSFunction.MakeCornerTopLeftRight(view_content, 8);
            view_content.ClipsToBounds = true;
            view_content.Layer.CornerRadius = 8;

            BT_start.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_share.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_attachement.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_history.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_comment.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_close.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            //CmmIOSFunction.AddShadowForTopORBotBar(top_view, true);
        }

        private async void LoadItemSelected()
        {
            try
            {
                loading = new CmmLoading(new CGRect((this.View.Bounds.Width - 200) / 2, (this.View.Bounds.Height - 200) / 2, 200, 200), "Đang xử lý...");
                this.View.Add(loading);

                SQLiteAsyncConnection conn = new SQLiteAsyncConnection(CmmVariable.M_DataPath, false);

                lst_addAttachment = new List<BeanAttachFile>();
                lst_addCommentAttachment = new List<BeanAttachFile>();
                if (currentItemSelected != null)
                {
                    List<BeanWorkflowFollow> lst_follow = new List<BeanWorkflowFollow>();
                    string query_follow = string.Format("SELECT * FROM BeanWorkflowFollow WHERE WorkflowItemId = ?");
                    var lst_followResult = conn.QueryAsync<BeanWorkflowFollow>(query_follow, currentItemSelected.ID).Result;

                    if (lst_followResult != null && lst_followResult.Count > 0 && lst_followResult[0].Status == 1)
                    {
                        BT_start.SetImage(UIImage.FromFile("Icons/icon_Star_on.png"), UIControlState.Normal);
                        isFollow = true;
                    }
                    else
                    {
                        BT_start.SetImage(UIImage.FromFile("Icons/icon_Star_off.png"), UIControlState.Normal);
                        isFollow = false;
                    }

                    lbl_workflowTitle.Text = (CmmVariable.SysConfig.LangCode == "1033" ? currentItemSelected.WorkflowTitleEN : currentItemSelected.WorkflowTitle) + " - " + currentItemSelected.Title;
                    lbl_content.Text = currentItemSelected.Content;

                    #region AssignedTo - lay danh sach nguoi xu ly hien tai
                    string assignedTo = currentItemSelected.AssignedTo;
                    var maxStatusWidth = (lbl_assignedTo.Frame.Width / 3) * 2;
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

                        if (currentItemSelected.ActionStatusID == 10) // da phe duyet
                        {
                            res = CmmFunction.GetTitle("TEXT_TITLE_APPROVED", "Phê duyệt: ") + assignedTo;
                            lbl_assignedTo.Text = res.TrimEnd(',');
                        }
                        else if (currentItemSelected.ActionStatusID == -1) // da huy
                        {
                            res = CmmFunction.GetTitle("TEXT_TITLE_CANCEL", "Hủy: ") + assignedTo;
                            lbl_assignedTo.Text = res.TrimEnd(',');
                        }
                        else if (currentItemSelected.ActionStatusID == 6)
                        {
                            res = CmmFunction.GetTitle("TEXT_TITLE_REJECTED", "Từ chối: ") + assignedTo;
                            lbl_assignedTo.Text = res.TrimEnd(',');
                        }
                        else
                        {
                            res = CmmFunction.GetTitle("TEXT_TO", "Đến: ") + assignedTo;
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
                                lbl_assignedTo.Text = res.TrimEnd(',');
                        }
                    }
                    #endregion

                    await Task.Run(() =>
                    {
                        ProviderControlDynamic p_controlDynamic = new ProviderControlDynamic();
                        Json_FormDataString = p_controlDynamic.GetTicketRequestControlDynamicForm(currentItemSelected);

                        //loadQuaTrinhluanchuyen();
                    });

                    if (!string.IsNullOrEmpty(Json_FormDataString))
                    {
                        JObject retValue = JObject.Parse(Json_FormDataString);
                        JArray json_dataForm = JArray.Parse(retValue["form"].ToString());
                        JArray json_workflowRelated = JArray.Parse(retValue["related"].ToString());
                        lstWorkFlowRelateds = json_workflowRelated.ToObject<List<BeanWorkFlowRelated>>();

                        //danh sach cong viec phan cong
                        JArray json_tasks = JArray.Parse(retValue["task"].ToString());
                        lst_tasks = json_tasks.ToObject<List<BeanTask>>();

                        #region danh sach comment
                        if (!string.IsNullOrEmpty(retValue["moreInfo"]["CommentChanged"].ToString()))//.HasValues
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

                        _objSubmitDetailComment.ID = _OtherResourceId; // empty or result
                        _objSubmitDetailComment.ResourceCategoryId = "8";
                        _objSubmitDetailComment.ResourceUrl = string.Format(CmmFunction.GetURLSettingComment(8), currentItemSelected.ID); // lấy trong beansetting
                        _objSubmitDetailComment.ItemId = currentItemSelected.ID;
                        _objSubmitDetailComment.Author = CmmVariable.SysConfig.UserId;
                        _objSubmitDetailComment.AuthorName = CmmVariable.SysConfig.DisplayName;

                        if (string.IsNullOrEmpty(_OtherResourceId))
                            _OtherResourceId = _pControlDynamic.GetDetailOtherResource(_objSubmitDetailComment);
                        else
                            _pControlDynamic.GetDetailOtherResource(_objSubmitDetailComment);

                        lst_comments = new List<BeanComment>();
                        lst_comments = CmmFunction.GetListComment(currentItemSelected, _OtherResourceId, _CommentChanged);
                        #endregion
                        //Kiem tra tinh trang follow
                        currentItemSelected.IsFollow = Convert.ToBoolean(json_dataForm[0]["IsFollow"]);
                        str_json_FormDefineInfo = json_dataForm[0]["FormDefineInfo"].ToString();
                        lst_section = json_dataForm.ToObject<List<ViewSection>>();
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

                        InvokeOnMainThread(() =>
                        {
                            if (lstWorkFlowRelateds != null && lstWorkFlowRelateds.Count > 0)
                                table_content.Source = new Control_TableSource(lst_section, lstWorkFlowRelateds, lst_tasks, lst_comments, this);
                            else
                                table_content.Source = new Control_TableSource(lst_section, null, lst_tasks, lst_comments, this);

                            view_details.Hidden = false;
                            table_content.ReloadData();
                            loading.Hide();
                        });
                        var isReadOnly = currentItemSelected.StatusGroup.HasValue && currentItemSelected.StatusGroup.Value == 1;

                        if (!isReadOnly)
                        {
                            constant_ButtonactionDefaut.Constant = view_buttonAction_default.Width;
                            //view_buttonAction.Frame = new CGRect(view_buttonAction_default.X, view_buttonAction.Frame.Y, view_buttonAction_default.Width, view_buttonAction.Frame.Height);
                            JObject jsonButtonBot = JObject.Parse(retValue["action"].ToString());
                            var buttonBot = jsonButtonBot.ToObject<ViewRow>();

                            if (componentButton != null)
                                componentButton.RemoveFromSuperview();
                            if (buttonBot.Elements != null || buttonBot.Elements.Count > 0)
                            {
                                isReadOnly = currentItemSelected.StatusGroup.HasValue && currentItemSelected.StatusGroup.Value == 4;// phiếu đang yêu cầu hiệu chỉnh thì không đc thao tác trên app
                            }
                            else
                            {
                                componentButton = new ComponentButtonBot(this, buttonBot);

                                componentButton.InitializeFrameView(view_buttonAction.Bounds);
                                componentButton.SetTitle();
                                componentButton.SetValue();
                                componentButton.SetEnable();
                                componentButton.SetProprety();

                                view_buttonAction.Add(componentButton);
                                lst_menuItem = componentButton.lst_moreActions;
                            }
                        }

                        if (parentView.GetType() == typeof(ToDoDetailView))
                        {
                            ToDoDetailView toDoDetailView = parentView as ToDoDetailView;
                            toDoDetailView.ReLoadTableListTodo();
                        }
                        else if (parentView.GetType() == typeof(WorkflowDetailView))
                        {
                            WorkflowDetailView workflowDetailView = parentView as WorkflowDetailView;
                            workflowDetailView.ReLoadTableListWorkFlow();
                        }
                        else if (parentView.GetType() == typeof(FollowListViewController))
                        {
                            FollowListViewController workflowDetailView = parentView as FollowListViewController;
                            workflowDetailView.ReLoadTableListWorkFlow();
                        }

                        InvokeOnMainThread(() =>
                        {
                            //if (isReadOnly)
                            //    CmmIOSFunction.commonAlertMessage(this, "VNA BPM", CmmFunction.GetTitle(CmmVariable.TEXT_ALERT_DRAFT, "Vui lòng sử dụng phiên bản web để chỉnh sửa phiếu này!"));
                        });
                    }
                    else
                    {
                        loading.Hide();
                        view_details.Hidden = true;

                        UIAlertController alert = UIAlertController.Create("Thông báo", "Không tìm thấy thông tin phiếu, vui lòng thử lại sau.", UIAlertControllerStyle.Alert);//"BPM"
                        alert.AddAction(UIAlertAction.Create("Đóng", UIAlertActionStyle.Default, alertAction =>
                        {

                        }));
                        this.PresentViewController(alert, true, null);
                    }

                    await Task.Run(() =>
                    {
                        loadQuaTrinhluanchuyen();
                    });
                }
                else
                {
                    view_details.Hidden = true;
                    loading.Hide();
                }
            }
            catch (Exception ex)
            {
                loading.Hide();
                Console.WriteLine("WorkflowDetailView - GetIndexItemFromDictionnary - ERR: " + ex.ToString());
            }
        }

        public void UpdateTableSections(int sectionIndex, BeanComment comment)
        {
            var item = lst_comments.Where(i => i.ID == comment.ID).FirstOrDefault();
            item = comment;

            LoadItemSelected();
        }

        private void loadQuaTrinhluanchuyen()
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(CmmVariable.M_DataPath);
            _ = Task.Run(() =>
            {
                lst_qtlc = new List<BeanQuaTrinhLuanChuyen>();
                ProviderControlDynamic p_dynamic = new ProviderControlDynamic();
                lst_qtlc = p_dynamic.GetListProcessHistory(currentItemSelected);

                if (lst_qtlc != null)
                {
                    List<BeanUser> lst_userResult = new List<BeanUser>();
                    string query_user0 = string.Format("SELECT * FROM BeanUser WHERE ID = ?");

                    //ActionStatusID = 10 || -1: phieu da phe duyet / Huy => Nguoi xu ly buoc truoc se la nguoi tao
                    if (currentItemSelected.ActionStatusID == 10 || currentItemSelected.ActionStatusID == -1 || currentItemSelected.ActionStatusID == 4)
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
                        if (lst_userResult != null && lst_userResult.Count > 0)
                        {
                            user_imagePath = lst_userResult[0].ImagePath;

                            lbl_sender.Text = lst_userResult[0].FullName;// + " (" + lst_userResult[0].Position + ")";
                        }

                        img_avatar_sentUnit.Hidden = true;
                        if (!string.IsNullOrEmpty(user_imagePath))
                        {
                            checkFileLocalIsExist(lst_userResult[0], img_avatar_sentUnit);
                            img_avatar_sentUnit.Hidden = false;
                        }

                    });
                }
            });
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
                    custom_menuAction.InitFrameView(new CGRect(this.view_details.Frame.Width - (205 + 25), this.view_userInfo.Frame.Bottom - 30, 205, maxheight + 3));
                    custom_menuAction.AddShadowForView();
                    custom_menuAction.ListItemMenu = lst_menuItem;
                    custom_menuAction.TableLoadData();

                    view_details.AddSubview(custom_menuAction);
                    view_details.BringSubviewToFront(custom_menuAction);
                }
            }
            catch (Exception ex)
            { Console.WriteLine(ex.ToString()); }
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
                image_view.Image = UIImage.FromFile("Icons/icon_account.png");
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
                        image_view.Image = UIImage.FromFile("Icons/icon_account.png");
                    }
                }
                else
                {
                    image_view.Image = UIImage.FromFile("Icons/icon_account.png");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("ContactView - openFile - Err: " + ex.ToString());
            }
        }
        private NSIndexPath GetIndexItemFromDictionnary(BeanWorkflowItem _item)
        {
            NSIndexPath indexTemp = null;
            var i = 0;
            do
            {
                var j = 0;
                var key = dict_workflow.ElementAt(i).Key;
                do
                {
                    if (_item == dict_workflow[key][j])
                    {
                        indexTemp = NSIndexPath.FromRowSection(j, i);
                        break;
                    }
                    j++;
                }
                while (j < dict_workflow[key].Count);
                i++;
            }
            while (i < dict_workflow.Count && indexTemp == null);

            return indexTemp;
        }

        public void CloseAddFollow()
        {
            Custom_AddFollowView custom_AddFollowView = Custom_AddFollowView.Instance;
            if (custom_AddFollowView.Superview != null)
                custom_AddFollowView.RemoveFromSuperview();
        }

        public void ReLoadDataFromServer()
        {
            LoadItemSelected();
        }

        public void UpdateItemCurrentSelect()
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(CmmVariable.M_DataPath, false);
            string query = string.Format("SELECT * FROM BeanWorkflowItem WHERE ID = ?");
            var lst_res = conn.QueryAsync<BeanWorkflowItem>(query, currentItemSelected.ID).Result;
            if (lst_res != null && lst_res.Count > 0)
                currentItemSelected = lst_res[0];
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
                    requestAddInfo.SetContent(this, action, lst_qtlc, currentItemSelected, null);
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
                    SubmitAction(action, null);
                    break;
                case (int)WorkflowAction.Action.Share: // 14 -  share
                    break;
                case (int)WorkflowAction.Action.CreateTask: // 54 -  Phan cong xu ly
                    CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("TEXT_FEATURE_UPDATING", "Tính năng đang được cập nhật..."));
                    break;
            }

            CloseAddFollow();
            CloseMenuOption();
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
                        additionalInformationView.SetContent(this, action, lst_qtlc, currentItemSelected, null);
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
                        CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("TEXT_FEATURE_UPDATING", "Tính năng đang được cập nhật..."));
                        break;
                }
            }
            else
            {
                menu_action_Toggle();
            }
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
                            else if (element.DataType != "inputcomments")
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
                        result = providerControl.SendControlDynamicAction(_buttonAction.Value, currentItemSelected.ID.ToString(), str_json_FormDefineInfo, json_edit_element, ref str_errMess, lst_files, null);

                    if (result)
                    {
                        b_pase.UpdateAllDynamicData(true);

                        InvokeOnMainThread(() =>
                        {
                            loading.Hide();

                            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(CmmVariable.M_DataPath, false);
                            List<BeanWorkflowItem> lst_follow = new List<BeanWorkflowItem>();
                            string query = string.Format("SELECT * FROM BeanWorkflowItem WHERE ID = ?");
                            var lst_res = conn.QueryAsync<BeanWorkflowItem>(query, currentItemSelected.ID).Result;
                            if (lst_res != null && lst_res.Count > 0)
                                currentItemSelected = lst_res[0];

                            LoadItemSelected();

                            if (parentView.GetType() == typeof(KanBanView))
                            {
                                KanBanView kanBanView = parentView as KanBanView;
                                kanBanView.reloadData();
                            }
                            else if (parentView.GetType() == typeof(MainView))
                            {
                                MainView mainview = parentView as MainView;
                                mainview.ReloadContent();
                            }
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
                return this.table_content;
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
            table_content.ReloadData();
        }
        public void HandleUserSingleChoiceSelected(ViewElement element, BeanUser _userSelected)
        {
            List<BeanUser> lst_userChoice = new List<BeanUser>();
            lst_userChoice.Add(_userSelected);
            var jsonString = JsonConvert.SerializeObject(lst_userChoice);
            element.Value = jsonString;
            UpdateValueElement_InListSection(element);
            table_content.ReloadData();
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
            table_content.ReloadData();
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
            table_content.ReloadData();
        }

        #region handle input text
        public void NavigatorToEditTextView(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
        {
            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            FormEditTextController textViewControlView = (FormEditTextController)Storyboard.InstantiateViewController("FormEditTextController");
            textViewControlView.setContent(this, 1, true, element, null);
            PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            textViewControlView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            textViewControlView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            textViewControlView.TransitioningDelegate = transitioningDelegate;
            this.PresentViewControllerAsync(textViewControlView, true);
        }
        public void HandleSingleLine(ViewElement element)
        {
            UpdateValueElement_InListSection(element);
            table_content.ReloadData();
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
                FormListItemsChoice itemsChoiceView = (FormListItemsChoice)Storyboard.InstantiateViewController("FormListItemsChoice");
                if (element.DataType == "singlechoice")
                    itemsChoiceView.setContent(this, false, element);
                else if (element.DataType == "multiplechoice")
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

        #region handle Attachment
        public void HandleAddAttachment(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
        {
            try
            {
                CloseAddFollow();
                this.View.EndEditing(true);
                Custom_AttachFileView attachFileView = Custom_AttachFileView.Instance;
                attachmentElement = element;
                attachFileView.viewController = this;
                attachFileView.InitFrameView(new CGRect(0, 0, this.View.Frame.Width, this.View.Frame.Height));
                attachFileView.TableLoadData();

                view_details.AddSubview(attachFileView);

                attachFileView.Frame = new CGRect(view_details.Frame.Right, 0, view_details.Frame.Width, view_details.Frame.Height);
                UIView.BeginAnimations("show_animationShowTable");
                UIView.SetAnimationDuration(0.3f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);
                attachFileView.Frame = new CGRect(10, 0, view_details.Frame.Width, view_details.Frame.Height);
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
            ViewElement viewElement = GetViewElementByDataType("inputattachmenthorizon");

            if (elementdataType == "inputattachmenthorizon")
            {
                lst_addAttachment = new List<BeanAttachFile>();
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
                    WorkflowId = currentItemSelected.WorkflowID,
                    WorkflowItemId = int.Parse(currentItemSelected.ID)
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
                    WorkflowId = currentItemSelected.WorkflowID,
                    WorkflowItemId = int.Parse(currentItemSelected.ID)
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

            HandleAttachFileClose();
            //if (addAttachmentsView != null)
            //    addAttachmentsView.NavigationController.PopViewController(false);
        }
        public void HandleAddAttachCommentFileResult(BeanAttachFileLocal _attachFile, string elementdataType)
        {
            ViewElement viewElement = GetViewElementByDataType(elementdataType);

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
                WorkflowId = currentItemSelected.WorkflowID,
                WorkflowItemId = int.Parse(currentItemSelected.ID)
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

            var jsonString = JsonConvert.SerializeObject(lst_addCommentAttachment);
            var jsonStringImage = JsonConvert.SerializeObject(lst_attachImage);
            var jsonStringDoc = JsonConvert.SerializeObject(lst_attachDoc);

            ObjectElementNote note1 = new ObjectElementNote { Key = "image", Value = jsonStringImage };
            ObjectElementNote note2 = new ObjectElementNote { Key = "doc", Value = jsonStringDoc };
            List<ObjectElementNote> objectElementNotes = new List<ObjectElementNote>();
            objectElementNotes.Add(note1); objectElementNotes.Add(note2);

            viewElement.Notes = objectElementNotes;
            table_content.ReloadData();

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

                            BeanAttachFileLocal itemiCloudAndDevice = new BeanAttachFileLocal() { ID = -1, Name = fileName, Path = filePath, Size = size, Type = type };
                            //HandleAddAttachFileResult(itemiCloudAndDevice, addAttachmentsView);
                            HandleAddAttachFileResult(itemiCloudAndDevice, elementDatatype);
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

            this.PresentModalViewController(imagePicker, true);
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

                        BeanAttachFileLocal itemiCloudAndDevice = new BeanAttachFileLocal() { ID = -1, Name = fileName[fileName.Length - 1], Path = filePath.Path, Size = size };
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

                    // dismiss the picker
                    imagePicker.DismissModalViewController(false);
                    var vc = this.PresentedViewController;
                    vc.DismissViewController(true, null);
                }
            }
            else
            {
                // dismiss the picker
                imagePicker.DismissModalViewController(false);
                CmmIOSFunction.AlertUnsupportFile(this);
            }
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

            table_content.ReloadData();
        }
        public void HandleAttachmentEdit(ViewElement element, NSIndexPath indexPath, BeanAttachFile _attach, ControlBase _controlBase)
        {
            FormEditAttachFileView formEditAttach = (FormEditAttachFileView)Storyboard.InstantiateViewController("FormEditAttachFileView");
            formEditAttach.SetContent(this, _attach);
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

            var index = lst_attachFile.FindIndex(item => item.ID == attachFile.ID);
            if (index != -1)
                lst_attachFile[index] = attachFile;

            var jsonString = JsonConvert.SerializeObject(lst_attachFile);
            _element.Value = jsonString;

            table_content.ReloadData();
        }
        public void NavigateToShowAttachView(BeanAttachFile currentAttachFile)
        {
            ShowAttachmentView showAttachmentView = Storyboard.InstantiateViewController("ShowAttachmentView") as ShowAttachmentView;
            showAttachmentView.setContent(this, currentAttachFile);
            this.PresentViewControllerAsync(showAttachmentView, true);
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
            var conn = new SQLiteConnection(CmmVariable.M_DataPath);
            string query = string.Format("SELECT * FROM BeanWorkflowItem WHERE ID = ?");
            List<BeanWorkflowItem> lst_result = new List<BeanWorkflowItem>();

            if (beanWorkFlowRelated.ItemID.ToString() != currentItemSelected.ID)
                lst_result = conn.Query<BeanWorkflowItem>(query, beanWorkFlowRelated.ItemID);
            if (beanWorkFlowRelated.ItemRLID.ToString() != currentItemSelected.ID)
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

        #region Handle Follow
        public void HandleWorkFollowViewResult()
        {
            Custom_WorkFlowView custom_WorkFollowView = Custom_WorkFlowView.Instance;
            if (custom_WorkFollowView.Superview != null)
            {
                var delayTimer = new Timer((state) => InvokeOnMainThread(() => custom_WorkFollowView.RemoveFromSuperview()), null, 300, Timeout.Infinite);

                custom_WorkFollowView.Frame = new CGRect(0, 0, view_details.Frame.Width, view_details.Frame.Height);
                UIView.BeginAnimations("show_animationShowTable");
                UIView.SetAnimationDuration(0.3f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);
                custom_WorkFollowView.Frame = new CGRect(view_details.Frame.Right, 0, view_details.Frame.Width, view_details.Frame.Height);
                UIView.CommitAnimations();
            }
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
        #endregion

        #region handle properties details
        public void NavigateToPropertyDetails(ViewElement element, ViewRow datajObject, JObject jObject, int _itemIndex, bool _isAddnew)
        {
            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            FormWFDetailsProperty detailsProperty = (FormWFDetailsProperty)Storyboard.InstantiateViewController("FormWFDetailsProperty");
            detailsProperty.SetContent(element, datajObject, jObject, currentItemSelected, _itemIndex, this, _isAddnew);
            PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            detailsProperty.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            detailsProperty.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            detailsProperty.TransitioningDelegate = transitioningDelegate;
            this.PresentModalViewController(detailsProperty, true);
        }
        public void HandlePropertyDetailsRemove(JObject jObjectRemove)
        {
            lstGridDetail_Deleted.Add(jObjectRemove);
            table_content.ReloadData();
        }
        #endregion

        #region handle Comments
        public void SubmitLikeAction(NSIndexPath sectionIndex, BeanComment comment)
        {
            UpdateTableSections(sectionIndex.Section, comment);
        }
        //Comment - reply
        public void NavigateToReplyComment(NSIndexPath _itemIndex, BeanComment comment)
        {
            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            FormCommentView formComment = (FormCommentView)Storyboard.InstantiateViewController("FormCommentView");
            formComment.SetContent(this, isTask, currentItemSelected, comment, _OtherResourceId, _itemIndex);
            PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            formComment.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            formComment.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            formComment.TransitioningDelegate = transitioningDelegate;
            this.PresentModalViewController(formComment, true);
        }
        public void ScrollToCommentViewRow(nfloat estHeight)
        {
            if (estHeight > 420)
                table_content.ScrollToRow(NSIndexPath.FromRowSection(lst_section[0].ViewRows.Count - 1, 0), UITableViewScrollPosition.Top, true);
            else
            {
                CGRect keyboardSize = new CGRect(0, 0, 1194, 400);
                CGRect custFrame = table_content.Frame;
                custFrame.Y -= keyboardSize.Height;
                table_content.Frame = custFrame;
            }
        }
        #endregion

        #endregion

        #region events
        private void BT_start_TouchUpInside(object sender, EventArgs e)
        {
            Custom_AddFollowView custom_AddFollowView = Custom_AddFollowView.Instance;
            if (custom_AddFollowView.Superview != null && custom_AddFollowView.viewController.GetType() == typeof(FormWorkFlowDetails))
                custom_AddFollowView.RemoveFromSuperview();
            else
            {
                if (isFollow)
                {
                    var width = StringExtensions.MeasureString("Hủy theo dõi công việc này", 14).Width + 20 + 70;
                    custom_AddFollowView.viewController = this;
                    custom_AddFollowView.isFollow = isFollow;
                    custom_AddFollowView.LoadContent();
                    custom_AddFollowView.InitFrameView(new CGRect(this.view_details.Frame.Width - (width + 10), (this.view_userInfo.Frame.Bottom / 2) - 10, width, 56));

                    this.view_details.AddSubview(custom_AddFollowView);
                    this.view_details.BringSubviewToFront(custom_AddFollowView);
                }
                else
                {
                    var width = StringExtensions.MeasureString("Đặt theo dõi công việc này", 14).Width + 20 + 70;
                    custom_AddFollowView.viewController = this;
                    custom_AddFollowView.isFollow = isFollow;
                    custom_AddFollowView.LoadContent();
                    custom_AddFollowView.InitFrameView(new CGRect(this.view_details.Frame.Width - (width + 10), (this.view_userInfo.Frame.Bottom / 2) - 10, width, 56));

                    this.view_details.AddSubview(custom_AddFollowView);
                    this.view_details.BringSubviewToFront(custom_AddFollowView);
                }
            }
        }
        private void BT_share_TouchUpInside(object sender, EventArgs e)
        {
            CloseAddFollow();

            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            FormShareView formShareView = (FormShareView)Storyboard.InstantiateViewController("FormShareView");
            formShareView.SetContent(this, currentItemSelected, str_json_FormDefineInfo);
            PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            formShareView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            formShareView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            formShareView.TransitioningDelegate = transitioningDelegate;
            this.PresentViewControllerAsync(formShareView, true);
        }
        private void BT_history_TouchUpInside(object sender, EventArgs e)
        {
            CloseAddFollow();

            Custom_WorkFlowView custom_WorkFollowView = Custom_WorkFlowView.Instance;
            custom_WorkFollowView.InitFrameView(new CGRect(10, top_view.Frame.Bottom, view_details.Frame.Width, view_details.Frame.Height));
            custom_WorkFollowView.viewController = this;
            custom_WorkFollowView.list_QTLC = lst_qtlc;
            custom_WorkFollowView.TableLoadData();

            view_content.AddSubview(custom_WorkFollowView);

            custom_WorkFollowView.Frame = new CGRect(view_details.Frame.Right, top_view.Frame.Bottom, view_details.Frame.Width, view_details.Frame.Height);
            UIView.BeginAnimations("show_animationShowTable");
            UIView.SetAnimationDuration(0.3f);
            UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
            UIView.SetAnimationRepeatCount(0);
            UIView.SetAnimationRepeatAutoreverses(false);
            UIView.SetAnimationDelegate(this);
            custom_WorkFollowView.Frame = new CGRect(10, top_view.Frame.Bottom, view_details.Frame.Width, view_details.Frame.Height);
            UIView.CommitAnimations();
        }
        private void BT_comment_TouchUpInside(object sender, EventArgs e)
        {
            try
            {
                CloseAddFollow();
                table_content.ScrollToRow(NSIndexPath.FromRowSection(lst_section[0].ViewRows.Count - 1, 0), UITableViewScrollPosition.Top, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ToDoView - BT_comment_TouchUpInside - Err: " + ex.ToString());
            }

        }
        private void BT_moreUser_TouchUpInside(object sender, EventArgs e)
        {
            if (arr_assignedTo != null && arr_assignedTo.Length > 1)
            {
                if (!isExpandUser)
                {
                    lbl_assignedTo.LineBreakMode = UILineBreakMode.WordWrap;
                    isExpandUser = true;
                    string combindedStringUser = string.Join(", ", lst_userName);
                    lbl_assignedTo.Text = CmmFunction.GetTitle("TEXT_TO", "Đến: ") + combindedStringUser;

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
        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            this.DismissModalViewController(true);
        }
        private void BT_attachement_TouchUpInside(object sender, EventArgs e)
        {
            CloseAddFollow();
            if (lst_attachFile != null && lst_attachFile.Count > 0)
            {
                CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
                CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
                ListAttachmentView attachmentView = (ListAttachmentView)Storyboard.InstantiateViewController("ListAttachmentView");
                attachmentView.SetContent(lst_attachFile, lbl_content.Text, this, attachmentElement);
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
                CGRect keyboardSize = UIKeyboard.BoundsFromNotification(notification);
                if (estCommmentViewRowHeight > keyboardSize.Height)
                    table_content.ScrollToRow(NSIndexPath.FromRowSection(lst_section[0].ViewRows.Count - 1, 0), UITableViewScrollPosition.Top, true);
                else
                {
                    table_content.ScrollToRow(NSIndexPath.FromRowSection(lst_section[0].ViewRows.Count - 1, 0), UITableViewScrollPosition.Bottom, true);

                    CGPoint point = table_content.ContentOffset;
                    var heightContent_noKeyboard = table_content.ContentSize.Height - keyboardSize.Height;
                    var heightRemain = keyboardSize.Height - estCommmentViewRowHeight;
                    point.Y = (heightContent_noKeyboard + heightRemain) - 50;
                    table_content.ContentOffset = point;
                }
            }
            catch (Exception ex)
            { Console.WriteLine("StartView - Err: " + ex.ToString()); }
        }
        private void KeyBoardDownNotification(NSNotification notification)
        {
            try
            {
                if (table_content.Frame.Y != 0)
                {
                    CGRect custFrame = table_content.Frame;
                    custFrame.Y = 90;
                    table_content.Frame = custFrame;
                }
            }
            catch (Exception ex)
            { Console.WriteLine("StartView - Err: " + ex.ToString()); }
        }

        #endregion

        #region custom class
        #region dynamic controls source table
        private class Control_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cell");
            FormWorkFlowDetails parentView;
            List<ViewSection> lst_section;
            Dictionary<string, List<ViewRow>> dict_control = new Dictionary<string, List<ViewRow>>();
            List<BeanTask> lst_tasks;
            List<BeanComment> lst_comment;
            List<BeanWorkFlowRelated> lstWorkFlowRelated;
            int heightHeader = 0;

            public Control_TableSource(List<ViewSection> _lst_section, List<BeanWorkFlowRelated> _lstWorkFlowRelated, List<BeanTask> _lst_tasks, List<BeanComment> _lst_comment, FormWorkFlowDetails _parentview)
            {
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
                    element.Enable = true;
                    element.DataType = "inputcomments";

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
                    element.Enable = true;
                    element.DataType = "inputcomments";

                    List<ViewRow> lst_viewRow = new List<ViewRow>();
                    ViewRow rowComment = new ViewRow();

                    List<ViewElement> lst_element = new List<ViewElement>();
                    lst_element.Add(element);
                    rowComment.Elements = lst_element;

                    lst_viewRow.Add(rowComment);
                    _lst_section[0].ViewRows.Add(rowComment);
                }

                lst_section = _lst_section;
                parentView = _parentview;
                GetListRowInSection();
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
                var element = dict_control[lst_section[indexPath.Section].ID][indexPath.Row].Elements[0];

                switch (element.DataType)
                {
                    case "tabs":
                        return 80;
                    case "inputattachmenthorizon":
                        if (!string.IsNullOrEmpty(element.Value))
                        {
                            int sectionHeightTotal = 0;
                            List<BeanAttachFile> lst_attach = new List<BeanAttachFile>();
                            JArray json = JArray.Parse(element.Value);
                            lst_attach = json.ToObject<List<BeanAttachFile>>();
                            parentView.lst_attachFile = lst_attach;

                            if (lst_attach.Count > 0)
                            {
                                List<string> sectionKeys = lst_attach.Select(x => x.AttachTypeName).Distinct().ToList();
                                if (sectionKeys != null && sectionKeys.Count > 0)
                                    sectionHeightTotal = sectionKeys.Count * 44;

                                return (lst_attach.Count * 65) + 75 + sectionHeightTotal;//header height: 75 - cell row height: 60 - padding top của table : 10
                            }
                            else
                                return 81;
                        }
                        else
                            return 81;
                    case "attachmentverticalformframe":
                        {
                            var arrAttachment = element.Value.Split(new string[] { ";#" }, StringSplitOptions.None);
                            int numItem = arrAttachment.Length / 2;

                            return (numItem >= 3) ? 265 : (85 + (numItem * 60)); //header view height: 85 | cell height: 60 | max cell: 3 cell
                        }
                    case "textinputmultiline":
                        {
                            string value = CmmFunction.StripHTML(element.Value);
                            var height_ets = StringExtensions.StringRect(value, UIFont.SystemFontOfSize(14, UIFontWeight.Regular), tableView.Frame.Width - 20);
                            if (height_ets.Height < 200)
                            {
                                if (height_ets.Height > 25)
                                    return (height_ets.Height) + 20;
                                else
                                    return 85;
                            }
                            else
                                return 140;
                        }
                    case "textinputformat":
                        {
                            var height_ets = StringExtensions.StringRect(element.Value, UIFont.SystemFontOfSize(14, UIFontWeight.Regular), tableView.Frame.Width - 20);
                            if (height_ets.Height < 200)
                                return 100 + 25;
                            else
                                return height_ets.Height + 25;
                        }
                    case "inputgriddetails":
                        {
                            nfloat height = 90;
                            var data_source = element.DataSource.Trim();
                            var data_value = element.Value.Trim();

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

                                    foreach (var item in lst_titleHeader)
                                    {
                                        //if (item.internalName == "TongTien")
                                        //    item.isSum = true;

                                        if (item.isSum)
                                        {
                                            item.isSum = true;
                                            height_expand = 50;
                                        }
                                    }
                                    height = height + (lst_jobject.Count * 50);

                                }
                                else
                                    height = height + (lst_jobject.Count * 50);

                                return height + height_expand;
                            }
                            else
                                return 90;
                        }
                    case "inputworkrelated":
                        {
                            var tableHeight = lstWorkFlowRelated.Count * 80;
                            return tableHeight + 50;
                        }
                    case "inputtasks":
                        {
                            var tableHeight = lst_tasks.Count * 95;
                            return tableHeight;
                        }
                    case "inputcomments":
                        {
                            nfloat basicHeight = 160;
                            nfloat height = 0;
                            //notes => add comment, dinh kem comment 
                            if (element.Notes != null && element.Notes.Count > 0)
                            {
                                foreach (var note in element.Notes)
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

                            if (!string.IsNullOrEmpty(element.DataSource) || element.DataSource != "[]")
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

                                }
                            }

                            parentView.estCommmentViewRowHeight = height;
                            return height;
                        }
                    default:
                        return 90;
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

            public override UIView GetViewForHeader(UITableView tableView, nint section)
            {
                var sectionItem = lst_section[Convert.ToInt32(section)];

                ComponentSection componentSection = new ComponentSection(parentView, sectionItem, section);
                componentSection.InitializeFrameView(new CGRect(0, 0, parentView.View.Bounds.Width, heightHeader));
                componentSection.UpdateContentSection();

                return componentSection;
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var control = dict_control[lst_section[indexPath.Section].ID][indexPath.Row];
                Control_cell_custom cell = new Control_cell_custom(parentView, cellIdentifier, control, indexPath);
                return cell;
            }
        }
        private class Control_cell_custom : UITableViewCell
        {
            FormWorkFlowDetails parentView { get; set; }
            ViewRow control { get; set; }
            NSIndexPath currentIndexPath { get; set; }
            public ComponentBase components;

            public Control_cell_custom(FormWorkFlowDetails _parentView, NSString cellID, ViewRow _control, NSIndexPath _currentIndexPath) : base(UITableViewCellStyle.Default, cellID)
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
        #endregion
    }
}


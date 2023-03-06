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
    public partial class FormCreateTaskView : UIViewController, EditTorInterFace
    {
        AppDelegate appD;
        private NSObject _willResignActiveNotificationObserver;
        private NSObject _didBecomeActiveNotificationObserver;
        private nfloat positionBotOfCurrentViewInput { get; set; }
        BeanWorkflowItem workflowitem { get; set; }
        UIViewController parent { get; set; }
        List<BeanUserAndGroup> lst_currentUserAndGroup { get; set; }
        List<BeanAttachFile> lst_addAttachFile = new List<BeanAttachFile>();
        List<BeanAttachFile> lst_removeAttachFile = new List<BeanAttachFile>();
        int numRowAttachmentFile = 0;
        BeanTask parenttask { get; set; }
        UIImagePickerController imagePicker;
        UIDocumentPickerViewController docPicker;
        List<BeanUser> lst_selectedUser = new List<BeanUser>();
        CmmLoading loading;
        string contentHtmlString = "";

        //TEditor
        TEditorViewController tvc;
        public bool isShowTEditor = false;
        bool EditTorInterFace.IsShowTEditor { get { return isShowTEditor; } set { isShowTEditor = value; } }

        public FormCreateTaskView(IntPtr handle) : base(handle)
        {
            appD = UIApplication.SharedApplication.Delegate as AppDelegate;
        }

        #region override
        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            scrollview_details.Frame = new CGRect(scrollview_details.Frame.X, scrollview_details.Frame.Y, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height * ((float)660 / 736));
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
            View.AddGestureRecognizer(gesture);

            ViewConfiguration();
            LoadContent();
            setlangTitle();

            #region delegate
            BT_close.TouchUpInside += BT_close_TouchUpInside;
            BT_approve.TouchUpInside += BT_approve_TouchUpInside;
            txt_note.Started += Txt_Note_Started;
            txt_note.Ended += Txt_Note_Ended;
            BT_note.TouchUpInside += BT_note_TouchUpInside;
            BT_startDate.TouchUpInside += BT_startDate_TouchUpInside;
            BT_user.TouchUpInside += BT_user_TouchUpInside;
            BT_addAttachment.TouchUpInside += BT_addAttachment_TouchUpInside;
            //switch_duedate.ValueChanged += Switch_duedate_ValueChanged;
            #endregion
        }

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
        #endregion

        #region public - private method
        public void SetContent(BeanWorkflowItem _workflowitem, BeanTask _parentTask, UIViewController _parent)
        {
            parenttask = _parentTask;
            workflowitem = _workflowitem;
            parent = _parent;
        }

        private void ViewConfiguration()
        {
            headerView_constantHeight.Constant = 45 + CmmIOSFunction.GetHeaderViewHeight();

            //BT_close.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_approve.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);

            table_attachment.ContentInset = new UIEdgeInsets(-20, 0, 0, 0);
            table_attachment.ScrollEnabled = false;

            view_title.Layer.BorderWidth = 1;
            view_title.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;
            view_title.Layer.CornerRadius = 3;
            view_title.ClipsToBounds = true;

            txt_note.TextContainer.MaximumNumberOfLines = 4;

            CmmIOSFunction.AddBorderView(view_title);
            CmmIOSFunction.AddBorderView(view_date);
            CmmIOSFunction.AddBorderView(view_user);
            CmmIOSFunction.AddBorderView(view_note);

            CmmIOSFunction.AddAttributeTitle(lbl_title, 11);
            CmmIOSFunction.AddAttributeTitle(lbl_user, 11);
        }

        private void BorderView()
        {
            //bo goc cho view header
            view_header_attachment.ClipsToBounds = true;
            UIBezierPath mPath_view_header = UIBezierPath.FromRoundedRect(view_header_attachment.Layer.Bounds, (UIRectCorner.TopLeft | UIRectCorner.TopRight), new CGSize(width: 6, height: 6));
            CAShapeLayer maskLayer_view_header = new CAShapeLayer();
            maskLayer_view_header.Frame = view_header_attachment.Layer.Bounds;
            maskLayer_view_header.Path = mPath_view_header.CGPath;
            view_header_attachment.Layer.Mask = maskLayer_view_header;

            // bo goc cho  tableView_attachment
            table_attachment.ClipsToBounds = true;
            UIBezierPath mPath_tableView_attachment = UIBezierPath.FromRoundedRect(table_attachment.Layer.Bounds, (UIRectCorner.BottomLeft | UIRectCorner.BottomRight), new CGSize(width: 6, height: 6));
            CAShapeLayer maskLayer_tableView_attachment = new CAShapeLayer();
            maskLayer_tableView_attachment.Frame = table_attachment.Layer.Bounds;
            maskLayer_tableView_attachment.Path = mPath_tableView_attachment.CGPath;
            table_attachment.Layer.Mask = maskLayer_tableView_attachment;
        }

        private void LoadContent()
        {
            //if(CmmVariable.SysConfig.LangCode == "1066")
            //    tf_date.Text = DateTime.Now.ToString(CmmVariable.M_WorkDateFormatDateTimeVN);
            //else
            //    tf_date.Text = DateTime.Now.ToString(CmmVariable.M_WorkDateFormatDateTimeEN);

            //table_selectedUser.Source = new Users_TableSource(lst_selectedUser, this);
            //table_selectedUser.ReloadData();
        }

        private void setlangTitle()
        {
            CmmIOSFunction.SetLangToView(this.View);
            CmmIOSFunction.AddAttributeTitle(lbl_title, 11);
            CmmIOSFunction.AddAttributeTitle(lbl_user, 11);
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

        public void RemoveUserFromList(BeanUser _removeUser)
        {
            //lst_selectedUser.Remove(_removeUser);
            //table_selectedUser.ReloadData();

            //tf_user.Text = GetStringUsers();
        }
        private void LoadAttachments()
        {
            if (lst_addAttachFile != null)
            {
                table_attachment.Frame = new CGRect(table_attachment.Frame.X, table_attachment.Frame.Y, table_attachment.Frame.Width, (lst_addAttachFile.Count * 60) + 60);
                view_attachment.Frame = new CGRect(view_attachment.Frame.X, view_attachment.Frame.Y, view_attachment.Frame.Width, table_attachment.Frame.Bottom);
                //scrollview_details.ContentSize = new CGSize(scrollview_details.Frame.Width, table_attachment.Frame.Bottom + 10);
                scrollview_details.ContentSize = new CGSize(scrollview_details.Frame.Width, view_attachment.Frame.Bottom + 10);
                table_attachment.Source = new Attachment_TableSource(lst_addAttachFile, this);
                table_attachment.ReloadData();
                SetHeightViewAttachment();
            }
        }
        private void SetHeightViewAttachment()
        {
            int count = (lst_addAttachFile != null && lst_addAttachFile.Count > 0) ? lst_addAttachFile.Count : 0;
            constraint_heightViewAttachment.Constant = (count * 60) + 20;
        }

        private bool ValidateFormControlValue()
        {
            if (string.IsNullOrEmpty(tf_title.Text))
            {
                CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("MESS_REQUIRE_TITLE", "Vui lòng nhập tiêu đề."));
                return false;
            }

            if (string.IsNullOrEmpty(tf_user.Text))
            {
                CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("MESS_REQUIRE_USERGROUP", "Vui lòng chọn người để thực hiện."));
                return false;
            }

            //if (!string.IsNullOrEmpty(tf_date.Text))
            //{
            //    DateTime _validateItem = DateTime.Now;
            //    if (CmmVariable.SysConfig.LangCode == "1066")
            //        _validateItem = DateTime.ParseExact(tf_date.Text, CmmVariable.M_WorkDateFormatDateTimeVN, null);
            //    else
            //        _validateItem = DateTime.ParseExact(tf_date.Text, CmmVariable.M_WorkDateFormatDateTimeEN, null);

            //    if (_validateItem < DateTime.Now)
            //    {
            //        CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("BPM", "Hạn hoàn tất không được nhỏ hơn thời gian hiện tại."));
            //        return false;
            //    }
            //}
            //else
            //{
            //    CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("BPM", "Vui lòng chọn hạn hoàn tất."));
            //    return false;
            //}

            //if (string.IsNullOrEmpty(txt_note.Text))
            //{
            //    CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("MESS_REQUIRE_TITLE", "Vui lòng nhập nội dung."));
            //    return false;
            //}

            return true;
        }

        #region RichTextEditor
        private async void ShowRichTextEditor()
        {
            try
            {
                TEditorResponse response;
                ///Đổi cách hiện
                /*if (!string.IsNullOrEmpty(txt_note.AttributedText.Value) || !string.IsNullOrEmpty(txt_note.Text))
                    response = await ShowTEditor(txt_note.Text, null, true);
                else
                    response = await ShowTEditor("", null, true);*/

                if (!string.IsNullOrEmpty(txt_note.AttributedText.Value) || !string.IsNullOrEmpty(txt_note.Text))
                    response = await CmmIOSFunction.ShowTEditor(true, contentHtmlString, this, this, tvc, null);
                else
                    response = await CmmIOSFunction.ShowTEditor(true, "", this, this, tvc, null);

                if (response.IsSave)
                {
                    if (response.HTML != null)
                    {
                        contentHtmlString = response.HTML;
                        SetValueResult(contentHtmlString);
                    }
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
            }
        }

        public Task<TEditorResponse> ShowTEditor(string html, ToolbarBuilder toolbarBuilder = null, bool autoFocusInput = false)
        {
            isShowTEditor = true;
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

            TaskCompletionSource<TEditorResponse> taskRes = new TaskCompletionSource<TEditorResponse>();
            tvc = new TEditorViewController();
            ToolbarBuilder builder = toolbarBuilder;
            if (toolbarBuilder == null)
                builder = new ToolbarBuilder().AddStandard();


            tvc.BuildToolbar(builder);
            tvc.SetHTML(html);
            tvc.SetAutoFocusInput(autoFocusInput);

            tvc.Title = "iPhone 12 Pro Max";
            if (!autoFocusInput)
            {
                tvc.View.Subviews[0].Frame = new CGRect(0, 70, tvc.View.Frame.Width, tvc.View.Frame.Height);
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
                taskRes.SetResult(new TEditorResponse() { IsSave = true, HTML = await tvc.GetHTML() });
                DismissModalViewController(true);
            };
            BT_RichTextback.TouchUpInside += async delegate
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
        #endregion

        #region handle Attachment
        public void HandleAddAttachment()
        {
            try
            {
                this.View.EndEditing(true);
                Custom_AttachFileView attachFileView = Custom_AttachFileView.Instance;
                attachFileView.parentview = this;
                attachFileView.InitFrameView(new CGRect(0, 0, view_content.Frame.Width, view_content.Frame.Height));
                attachFileView.TableLoadData();

                view_content.AddSubview(attachFileView);

                attachFileView.Frame = new CGRect(view_content.Frame.Right, 0, view_content.Frame.Width, view_content.Frame.Height);
                UIView.BeginAnimations("show_animationShowTable");
                UIView.SetAnimationDuration(0.3f);
                UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
                UIView.SetAnimationRepeatCount(0);
                UIView.SetAnimationRepeatAutoreverses(false);
                UIView.SetAnimationDelegate(this);
                attachFileView.Frame = new CGRect(0, BT_approve.Frame.Top, view_content.Frame.Width, view_content.Frame.Height);
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
        public void HandleAddAttachFileResult(BeanAttachFileLocal _attachFile)
        {
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
            LoadAttachments();
            SetHeightViewAttachment();

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

                        string[] arrType = new string[] { "doc", "docx", "xls", "xlsx", "pdf", "png", "jpeg", "jpg", "txt" };

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

                        BeanAttachFileLocal itemiCloudAndDevice = new BeanAttachFileLocal() { ID = -1, Name = fileName[fileName.Length - 1], Path = filePath.Path, Size = size };
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
            {
                CmmIOSFunction.AlertUnsupportFile(this, true);
            }

            //var vc = this.PresentedViewController;
            //vc.DismissViewController(true, null);
            HandleAttachFileClose();

        }
        private void Handle_Canceled(object sender, EventArgs e)
        {
            imagePicker.DismissModalViewController(true);
        }
        public void HandleAttachmentRemove(BeanAttachFile attachFileRemove)
        {
            lst_addAttachFile.Remove(attachFileRemove);
            LoadAttachments();
        }
        public void HandleAttachmentEdit(ViewElement element, NSIndexPath indexPath, BeanAttachFile _attach)
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
            //ShowAttachmentView showAttachmentView = Storyboard.InstantiateViewController("ShowAttachmentView") as ShowAttachmentView;
            //showAttachmentView.setContent(this, currentAttachFile);
            //this.PresentViewControllerAsync(showAttachmentView, true);
            if (parent != null && parent.GetType() == typeof(RequestDetailsV2))
            {
                //currentAttachment = _attachment;
                if (parent.GetType() == typeof(RequestDetailsV2))
                {
                    RequestDetailsV2 requestDetailsV2 = (RequestDetailsV2)parent;
                    requestDetailsV2.NavigateToAttachView(currentAttachFile);
                }
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
            this.NavigationController.PopViewController(true);
        }
        private async void BT_approve_TouchUpInside(object sender, EventArgs e)
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
                    #region Handle Bean Task
                    BeanTask _itemTask = new BeanTask();

                    // Tạo Task con cần thêm Parent
                    if (parenttask != null)
                        _itemTask.Parent = parenttask.ID;
                    else
                        // tao moi
                        _itemTask.Parent = 0;

                    _itemTask.ID = 0;
                    _itemTask.WorkflowId = workflowitem.WorkflowID;
                    _itemTask.SPItemId = int.Parse(workflowitem.ID);
                    _itemTask.Step = workflowitem.Step.HasValue ? workflowitem.Step.Value : -1;
                    _itemTask.Title = tf_title.Text;

                    if (!string.IsNullOrEmpty(tf_date.Text))
                    {
                        if (CmmVariable.SysConfig.LangCode == "1033")
                            _itemTask.DueDate = DateTime.ParseExact(tf_date.Text, CmmVariable.M_WorkDateFormatDateTimeEN, null);
                        else
                            _itemTask.DueDate = DateTime.ParseExact(tf_date.Text, CmmVariable.M_WorkDateFormatDateTimeVN, null);
                    }
                    else
                        _itemTask.DueDate = null;

                    //_itemTask.Content = !String.IsNullOrEmpty(_richEditorNoiDung.GetHtml()) ? _richEditorNoiDung.GetHtml() : "";
                    _itemTask.Content = contentHtmlString;//txt_note.Text;
                    _itemTask.Status = 0;
                    #endregion

                    await Task.Run(() =>
                    {
                        _result = _pControlDynamic.SendCreateTaskAction(_itemTask, lst_currentUserAndGroup, _lstSubmitActionData, _lstKeyVarAttachmentLocal, (int)FlagActionPermission.CreateNew);

                        if (_result)
                        {
                            //ProviderBase pBase = new ProviderBase();
                            //pBase.UpdateAllMasterData(true);
                            //pBase.UpdateAllDynamicData(true);
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
                                this.NavigationController.PopViewController(true);
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
                Console.WriteLine("FormCreateTaskView - BT_approve_TouchUpInside - Err: " + ex.ToString());
            }
        }
        private void BT_addAttachment_TouchUpInside(object sender, EventArgs e)
        {
            HandleAddAttachment();
        }
        void Txt_Note_Ended(object sender, EventArgs e)
        {
        }
        void Txt_Note_Started(object sender, EventArgs e)
        {
            // ShowRichTextEditor();

        }
        private void BT_note_TouchUpInside(object sender, EventArgs e)
        {
            ShowRichTextEditor();
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

            //formUserAndGroupView.SetContent(this, true, lst_currentUserAndGroup, false, null, CmmFunction.GetTitle("TEXT_USER_PROCESS", "Người xử lý"), true);
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
                    //tvc.View.Subviews[0].Frame = new CGRect(0, 70, tvc.View.Frame.Width, tvc.View.Subviews[0].Frame.Height - 100);
                }
                else if (View.Frame.Y == 0)
                {
                    var topKeybroad = this.View.Frame.Height - keyboardSize.Height;
                    if (topKeybroad < positionBotOfCurrentViewInput)
                    {
                        CGRect custFrame = View.Frame;
                        custFrame.Y -= (positionBotOfCurrentViewInput - topKeybroad);
                        View.Frame = custFrame;
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
                    //tvc.View.Subviews[0].Frame = new CGRect(0, 70, tvc.View.Frame.Width, tvc.View.Subviews[0].Frame.Height - 100);
                    //isShowTEditor = false;
                }
                else
                if (View.Frame.Y != 0)
                {
                    CGRect custFrame = View.Frame;
                    custFrame.Y = 0;
                    View.Frame = custFrame;
                }
            }
            catch (Exception ex)
            { Console.WriteLine("FormCreateView - Err: " + ex.ToString()); }
        }
        #endregion

        #region custom views
        #region attachment source table
        private class Attachment_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellAttachmentID");
            FormCreateTaskView parentView;
            List<BeanAttachFile> lst_attachment = new List<BeanAttachFile>();
            List<string> sectionKeys;

            public Attachment_TableSource(List<BeanAttachFile> _lst_attachment, FormCreateTaskView _parentview)
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
                if (attachment.IsAuthor == true)
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
            FormCreateTaskView parentView { get; set; }
            NSIndexPath currentIndexPath { get; set; }
            BeanAttachFile attachment { get; set; }
            UILabel lbl_title, lbl_creator, lbl_size, lbl_chucvu;
            UIImageView img_type;
            bool isOdd;

            public Attachment_cell_custom(FormCreateTaskView _parentView, NSString cellID, BeanAttachFile _attachment, NSIndexPath _currentIndexPath, bool _isOdd) : base(UITableViewCellStyle.Default, cellID)
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
                    TextColor = UIColor.FromRGB(0, 95, 212),
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

                                    //img_Creator.Hidden = false;
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
                                    //img_Creator.Hidden = false;
                                });
                            }
                        });
                    }
                    else
                    {
                        openFile(filename, image_view);
                        //img_Creator.Hidden = false;
                    }
                }
                catch (Exception ex)
                {
                    image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
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
                    image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
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
        #endregion
        #region table data source user
        private class Users_TableSource : UITableViewSource
        {
            List<BeanUser> lst_user;
            NSString cellIdentifier = new NSString("cellUser");
            FormCreateTaskView parentView;

            public Users_TableSource(List<BeanUser> _users, FormCreateTaskView _parentview)
            {
                parentView = _parentview;
                lst_user = _users;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return lst_user.Count;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 78;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                Custom_UserCell cell = new Custom_UserCell(cellIdentifier, parentView);
                var user = lst_user[indexPath.Row];

                bool isOdd = true;
                if (indexPath.Row % 2 == 0)
                    isOdd = false;

                cell.UpdateCell(user, true, isOdd);
                return cell;
            }
        }

        #endregion
        #endregion
    }
}

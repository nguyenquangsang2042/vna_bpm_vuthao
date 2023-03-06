using System;
using System.Collections.Generic;
using BPMOPMobile.Bean;
using BPMOPMobileV1.iOS.Components;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json.Linq;
using UIKit;
using TEditor;
using TEditor.Abstractions;
using System.Threading.Tasks;
using BPMOPMobileV1.iOS.IOSClass;
using BPMOPMobileV1.iOS.CustomControlClass;
using MobileCoreServices;
using System.Linq;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class CreateTicketFormView : UIViewController, EditTorInterFace
    {
        AppDelegate appD;
        List<ViewSection> lst_section { get; set; }
        private NSObject _willResignActiveNotificationObserver;
        private NSObject _didBecomeActiveNotificationObserver;
        private UITapGestureRecognizer gestureRecognizer_keyboard;
        ComponentBase controllActive;
        private UIView currentViewEditing { get; set; }

        string contentHtmlString = "";
        ViewElement currentElement { get; set; }
        NSIndexPath currentIndexPath { get; set; }
        int numRowWorkRelated = 0;

        //Attachments
        int numRowAttachmentFile = 0;
        ViewElement currentElementAttachFile { get; set; }
        UIImagePickerController imagePicker;

        //TEditor
        TEditorViewController tvc;
        public bool isShowTEditor = false;
        bool EditTorInterFace.IsShowTEditor { get { return isShowTEditor; } set { isShowTEditor = value; } }

        public CreateTicketFormView(IntPtr handle) : base(handle)
        {
            appD = UIApplication.SharedApplication.Delegate as AppDelegate;
        }

        #region override
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            //if (buttonActionTopBar != null && buttonActionBotBar != null)
            //{
            //    view_top_bar.AddSubviews(buttonActionTopBar);
            //    view_bot_bar.AddSubviews(buttonActionBotBar);
            //}
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _willResignActiveNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, KeyBoardUpNotification);
            _didBecomeActiveNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyBoardDownNotification);

            gestureRecognizer_keyboard = new UITapGestureRecognizer(() =>
            {
                View.EndEditing(true);
            });

            gestureRecognizer_keyboard.ShouldReceiveTouch += delegate (UIGestureRecognizer recognize, UITouch touch)
            {
                currentViewEditing = touch.View.Superview;
                var touchName = touch.View.Superview.GetType().Name;
                if (touchName == "ControlTextInput" || touchName == "ControlTextInputMultiLine" || touchName == "ControlTextInputFormat")
                    return false;
                else
                    return true;
            };

            gestureRecognizer_keyboard.CancelsTouchesInView = false;
            this.View.AddGestureRecognizer(gestureRecognizer_keyboard);

            ViewConfiguration();
            LoadContent();

            #region delegate
            CmmIOSFunction.ResignFirstResponderOnTap(this.View);
            BT_back.TouchUpInside += BT_back_TouchUpInside;
            #endregion
        }

        #endregion

        #region private - public method

        public void SetContent(int _index)
        {
            //index = _index;
        }

        private void ViewConfiguration()
        {
            SetConstraint();
            //int column = 3;
            //if (column == 3)
            //{
            //    nfloat button_width = this.View.Bounds.Width / 3;
            //    BT_action1.Frame = new CGRect(0, BT_action1.Frame.Y, button_width, BT_action1.Frame.Height);
            //    BT_action2.Frame = new CGRect(BT_action1.Frame.Right, BT_action2.Frame.Y, button_width, BT_action2.Frame.Height);
            //    BT_link.Frame = new CGRect(BT_action2.Frame.Right, BT_link.Frame.Y, button_width, BT_link.Frame.Height);
            //    BT_link.Hidden = false;
            //}
            //else if (column == 2)
            //{
            //    nfloat button_width = bottom_view.Frame.Width / 2;
            //    BT_action1.Frame = new CGRect(0, BT_action1.Frame.Y, button_width, BT_action1.Frame.Height);
            //    BT_action2.Frame = new CGRect(BT_action1.Frame.Right, BT_action2.Frame.Y, button_width, BT_action2.Frame.Height);
            //    BT_link.Hidden = true;
            //}
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

        private void LoadContent()
        {
            string dataString = "";
            //dataString = @"[{'ShowType':true,'IsShowHint':false,'ViewRows':[{'RowType':1,'Elements':[{'DataType':'textinput','DataSource':null,'ListProprety':null,'ID':1,'Title':'Về việc','Value':'','Enable':true,'IsRequire':false}],'ID':1,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinput','DataSource':null,'ListProprety':null,'ID':1,'Title':'Kính trình','Value':'','Enable':true,'IsRequire':false}],'ID':2,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputformat','DataSource':null,'ListProprety':null,'ID':1,'Title':'Căn cứ','Value':'','Enable':true,'IsRequire':false}],'ID':3,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputmultiline','DataSource':null,'ListProprety':null,'ID':1,'Title':'Chi tiết','Value':'','Enable':true,'IsRequire':true}],'ID':4,'Title':null,'Value':null,'Enable':false},{'RowType':3,'Elements':[{'DataType':null,'DataSource':null,'ListProprety':null,'ID':1,'Title':'Người soạn thảo','Value':'Test Admin','Enable':false,'IsRequire':false},{'DataType':null,'DataSource':null,'ListProprety':null,'ID':2,'Title':'Chức vụ','Value':'Nhân viên Kinh doanh','Enable':false,'IsRequire':false},{'DataType':null,'DataSource':null,'ListProprety':null,'ID':3,'Title':'Đơn vị','Value':'Vũ Thảo Co. ltd','Enable':false,'IsRequire':false}],'ID':5,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputmultiline','DataSource':null,'ListProprety':null,'ID':1,'Title':'Ghi chú','Value':'','Enable':true,'IsRequire':false}],'ID':6,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'inputattachmentvertical','DataSource':null,'ListProprety':null,'ID':4,'Title':'File đính kèm','Value':'','Enable':false}],'ID':1,'Title':null,'Value':null,'Enable':false}],'ID':1,'Title':'Báo Cáo','Value':null,'Enable':false}]";
            dataString = @"[{'ShowType':true,'IsShowHint':false,'ViewRows':[{'RowType':1,'Elements':[{'DataType':'textinput','DataSource':null,'ListProprety':null,'ID':1,'Title':'Về việc','Value':'','Enable':true,'IsRequire':false}],'ID':1,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinput','DataSource':null,'ListProprety':null,'ID':1,'Title':'Kính trình','Value':'','Enable':true,'IsRequire':false}],'ID':2,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputformat','DataSource':null,'ListProprety':null,'ID':1,'Title':'Căn cứ','Value':'','Enable':true,'IsRequire':false}],'ID':3,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputmultiline','DataSource':null,'ListProprety':null,'ID':1,'Title':'Chi tiết','Value':'','Enable':true,'IsRequire':true}],'ID':4,'Title':null,'Value':null,'Enable':false},{'RowType':3,'Elements':[{'DataType':null,'DataSource':null,'ListProprety':null,'ID':1,'Title':'Người soạn thảo','Value':'Test Admin','Enable':false,'IsRequire':false},{'DataType':null,'DataSource':null,'ListProprety':null,'ID':2,'Title':'Chức vụ','Value':'Nhân viên Kinh doanh','Enable':false,'IsRequire':false},{'DataType':null,'DataSource':null,'ListProprety':null,'ID':3,'Title':'Đơn vị','Value':'Vũ Thảo Co. ltd','Enable':false,'IsRequire':false}],'ID':5,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputmultiline','DataSource':null,'ListProprety':null,'ID':1,'Title':'Ghi chú','Value':'','Enable':true,'IsRequire':false}],'ID':6,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'inputattachmenthorizon','DataSource':null,'ListProprety':null,'ID':4,'Title':'File đính kèm','Value':'','Enable':false}],'ID':7,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'inputworkrelated','DataSource':null,'ListProprety':null,'ID':4,'Title':'Quy trình / Công việc liên kết','Value':'','Enable':false}],'ID':8,'Title':null,'Value':null,'Enable':false}],'ID':1,'Title':'Báo Cáo','Value':null,'Enable':false}]";
            JArray json = JArray.Parse(dataString);
            lst_section = json.ToObject<List<ViewSection>>();

            table_content.Source = new Control_TableSource(lst_section, this);
            table_content.ReloadData();

            var dataButtonBot = @"{'RowType':3,'Elements':[{'DataType':'buttonbot','DataSource': null,'ListProprety':null,'ID':1,'Title':'Lưu','Value': 'Icons/icon_save.png','Enable':true},{'DataType':'buttonbot','DataSource': null,'ListProprety':null,'ID':2,'Title':'Gửi','Value': 'Icons/icon_send.png','Enable':true},{'DataType':'buttonbot','DataSource': null,'ListProprety':null,'ID':3,'Title': 'Liên kết','Value': 'Icons/icon_link.png','Enable':true}],'ID':3,'Title':null,'Value':null,'Enable':false}";
            JObject jsonButtonBot = JObject.Parse(dataButtonBot);
            var buttonBot = jsonButtonBot.ToObject<ViewRow>();
            ComponentButtonBot componentButton = new ComponentButtonBot(this, buttonBot);
            componentButton.InitializeFrameView(bottom_view.Bounds);
            componentButton.SetTitle();
            componentButton.SetValue();
            componentButton.SetEnable();
            componentButton.SetProprety();
            bottom_view.Add(componentButton);
        }

        public async void NavigatorToView(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
        {
            currentElement = element;
            currentIndexPath = indexPath;

            this.View.EndEditing(true);

            if (_controlBase != null)
            {
                switch (_controlBase.GetType().Name)
                {
                    case "ControlTextInputFormat":
                        //TFormatEditorView formatEditorView = Storyboard.InstantiateViewController("TFormatEditorView") as TFormatEditorView;
                        //this.NavigationController.PushViewController(formatEditorView, false);
                        //formatEditorView.SetAutoFocusInput(true);

                        TEditorResponse response = await CmmIOSFunction.ShowTEditor(true, contentHtmlString, this, this, null); //await ShowTEditor(contentHtmlString, null, true);

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

            headerView.AddSubviews(BT_left, BT_right);

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
            if (!string.IsNullOrEmpty(value))
            {
                if (currentElement != null)
                {
                    currentElement.Value = value;
                    table_content.ReloadRows(new NSIndexPath[] { currentIndexPath }, UITableViewRowAnimation.None);
                }
            }
        }

        public void HandleAddAttachment(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
        {
            try
            {
                currentElementAttachFile = element;
                this.View.EndEditing(true);

                AddAttachmentsView addAttachmentsView = Storyboard.InstantiateViewController("AddAttachmentsView") as AddAttachmentsView;
                addAttachmentsView.SetContent(this, element);
                this.PresentViewController(addAttachmentsView, true, null);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("CreateNewTaskView - HandleAddAttachment - Err: " + ex.ToString());
#endif
            }
        }

        public void HandleRemoveAttachment(ViewElement element, NSIndexPath indexPath, ControlBase _controlBas, int numRow)
        {
            currentElementAttachFile = element;
            numRowAttachmentFile = numRow;
            table_content.ReloadData();
        }

        public void HandleAttachFileClose()
        {
            //Custom_AttachFileView custom_menuOption = Custom_AttachFileView.Instance;
            //AddAttachmentsView addAttachmentsView = Storyboard.InstantiateViewController("AddAttachmentsView") as AddAttachmentsView;
            //addAttachmentsView.HandleAddAttachFileClose();
        }

        public void NavigationToDocumentPicker(AddAttachmentsView addAttachmentsView)
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

                var docPicker = new UIDocumentPickerViewController(allowedUTIs, UIDocumentPickerMode.Import);
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
                            HandleAddAttachFileResult(itemiCloudAndDevice);
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
                addAttachmentsView.PresentViewController(docPicker, true, null);
            }
            else
                CmmIOSFunction.commonAlertMessage(this, "BPM", "Chỉ hỗ trợ thêm tập tin office đính kèm từ hệ điều hành 11 trở lên.");
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
            //imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);
            imagePicker.MediaTypes = new string[] { UTType.Image };
            imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
            imagePicker.Canceled += Handle_Canceled;

            addAttachmentsView.PresentViewController(imagePicker, true, null);
        }

        public void HandleButtonBot(ViewElement element)
        {
            //Save
            //if (element.ID == 1)
            //{

            //}
            ////Send
            //else if (element.ID == 2)
            //{

            //}
            //else if (element.ID == 3)
            //{
            //    ButtonAction btnSave = new ButtonAction();
            //    btnSave.ID = element.ID;

            //    FormAddWorkRelatedView formAddWorkRelatedView = Storyboard.InstantiateViewController("FormAddWorkRelatedView") as FormAddWorkRelatedView;
            //    formAddWorkRelatedView.SetContent(this);
            //    this.PresentModalViewController(formAddWorkRelatedView, true);

            //}
        }

        private void Handle_Canceled(object sender, EventArgs e)
        {
            imagePicker.DismissModalViewController(true);
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
                        HandleAddAttachFileResult(itemiCloudAndDevice);
                    }
                    else
                    {
                        string fileName = "IMG_" + DateTime.Now.ToString("MMss") + ".JPG";
                        //CollectFileInfo(fileName, originalImage);
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
        }

        public void HandleAddAttachFileResult(BeanAttachFileLocal _attachFile)
        {
            List<BeanAttachFile> lst_attachment = new List<BeanAttachFile>();
            if (!string.IsNullOrEmpty(currentElementAttachFile.Value))
            {
                JArray json = JArray.Parse(currentElementAttachFile.Value);
                lst_attachment = json.ToObject<List<BeanAttachFile>>();
            }

            numRowAttachmentFile++;
            string custID = numRowAttachmentFile + "";
            BeanAttachFile attachFile = new BeanAttachFile() { ID = custID, Title = _attachFile.Name, Path = _attachFile.Path, Size = _attachFile.Size };

            lst_attachment.Add(attachFile);

            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lst_attachment);
            currentElementAttachFile.Value = jsonString;

            //HandleAttachFileClose();
            table_content.ReloadData();
        }

        public void HandleAddWorkRelatedResult(List<BeanNotify> _lst_notify)
        {
            ViewElement viewElement = GetViewElementByDataType("inputworkrelated");

            List<BeanNotify> lst_item = new List<BeanNotify>();
            if (!string.IsNullOrEmpty(viewElement.Value))
            {
                JArray json = JArray.Parse(viewElement.Value);
                lst_item = json.ToObject<List<BeanNotify>>();
            }

            lst_item.AddRange(_lst_notify);
            numRowWorkRelated = lst_item.Count;

            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lst_item);
            viewElement.Value = jsonString;

            table_content.ReloadData();
        }

        public void HandleWorkRelatedResult(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase, int numRow)
        {
            numRowWorkRelated = numRow;
            table_content.ReloadData();
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
        #endregion

        #region events
        private void BT_back_TouchUpInside(object sender, EventArgs e)
        {
            this.NavigationController.PopViewController(true);
        }
        private void KeyBoardUpNotification(NSNotification notification)
        {
            try
            {
                switch (currentViewEditing.GetType().Name)
                {
                    case "ControlTextInput":
                        controllActive = (ControlTextInput)currentViewEditing;
                        break;
                    case "ControlTextInputMultiLine":
                        controllActive = (ControlTextInputMultiLine)currentViewEditing;
                        break;
                }

                var rectRow = table_content.RectForRowAtIndexPath(controllActive.IndexPath);
                table_content.SetContentOffset(new CGPoint(0, rectRow.Y), true);
            }
            catch (Exception ex)
            { Console.WriteLine("CreateTicketFormView - Err: " + ex.ToString()); }
        }
        private void KeyBoardDownNotification(NSNotification notification)
        {
            try
            {
                table_content.ScrollToRow(controllActive.IndexPath, UITableViewScrollPosition.Top, true);
            }
            catch (Exception ex)
            { Console.WriteLine("CreateTicketFormView - Err: " + ex.ToString()); }
        }
        #endregion

        #region custom
        #region dynamic controls source table
        private class Control_TableSource : UITableViewSource
        {
            CreateTicketFormView parentView;
            NSString cellIdentifier = new NSString("cell");
            List<ViewSection> lst_section;
            Dictionary<int, List<ViewRow>> dict_control = new Dictionary<int, List<ViewRow>>();
            int heightHeader = 50;

            public Control_TableSource(List<ViewSection> _lst_section, CreateTicketFormView _parentview)
            {
                lst_section = _lst_section;
                parentView = _parentview;
                GetListRowInSection();
            }

            public void GetListRowInSection()
            {
                foreach (var item in lst_section)
                {
                    dict_control.Add(Convert.ToInt16(item.ID), item.ViewRows);
                }
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                return -1;
            }

            public override nfloat GetHeightForFooter(UITableView tableView, nint section)
            {
                return -1;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                //var dataType = dict_control[lst_section[indexPath.Section].ID][indexPath.Row].Elements[0].DataType;
                //switch (dataType)
                //{
                //    case "textinputmultiline":
                //        return 115;
                //    case "inputattachmenthorizon":
                //        if (parentView.numRowAttachmentFile > 0)
                //            return 190;//header height: 75 - cell row height: 60 - padding top của table : 10
                //        else
                //            return 40;
                //    case "inputworkrelated":
                //        return 40 + (100 * parentView.numRowWorkRelated);//title height: 40 - cell row height: 76
                //    default:
                return 85;
                //}
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return lst_section.Count;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                var sectionItem = lst_section[Convert.ToInt32(section)];
                var lst_row = dict_control[Convert.ToInt16(sectionItem.ID)];
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

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var control = dict_control[Convert.ToInt16(lst_section[indexPath.Section].ID)][indexPath.Row];
                Control_cell_custom cell = new Control_cell_custom(parentView, cellIdentifier, control, indexPath);
                return cell;
            }
        }
        private class Control_cell_custom : UITableViewCell
        {
            CreateTicketFormView parentView { get; set; }
            ViewRow control { get; set; }
            NSIndexPath currentIndexPath { get; set; }
            public ComponentBase components;

            public Control_cell_custom(CreateTicketFormView _parentView, NSString cellID, ViewRow _control, NSIndexPath _currentIndexPath) : base(UITableViewCellStyle.Default, cellID)
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
                    Console.WriteLine("CreateNewTaskView - Control_cell_custom - loaddata- ERR: " + ex.ToString());
                }
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                components.InitializeFrameView(new CGRect(25, 0, ContentView.Frame.Width - 50, ContentView.Frame.Height));
            }
        }

        #endregion
        #endregion
    }
}


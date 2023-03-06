using BPMOPMobile.Class;
using BPMOPMobile.iPad.CustomControlClass;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UIKit;
using BPMOPMobile.Bean;
using Newtonsoft.Json.Linq;
using BPMOPMobile.iPad.Components;
using System.Threading.Tasks;
using MobileCoreServices;

namespace BPMOPMobile.iPad
{
    public partial class CreateNewTaskView : UIViewController
    {
        AppDelegate appD;
        ButtonsActionTopBar buttonActionTopBar;
        ButtonsActionBotBar buttonActionBotBar;

        Dictionary<string, List<BeanWorkflow>> dict_groupWorkFlow = new Dictionary<string, List<BeanWorkflow>>();
        List<ClassMenu> lst_menuItem = new List<ClassMenu>();
        ClassMenu currentMenu { get; set; }
        Dictionary<string, List<WorkFlowItemDemo>> dict_groupWorkFlow_fillter = new Dictionary<string, List<WorkFlowItemDemo>>();
        List<ViewSection> lst_section { get; set; }

        private NSObject _willResignActiveNotificationObserver;
        private NSObject _didBecomeActiveNotificationObserver;

        private UIView currentViewEditing { get; set; }
        ComponentBase controllActive;
        string contentHtmlString = "";
        ViewElement currentElement { get; set; }
        NSIndexPath currentIndexPath { get; set; }

        WorkflowDetailView workflowDetailView { get; set; }
        ToDoDetailView toDoDetailView { get; set; }
        int numRowAttachmentFile = 0;
        int numRowWorkRelated = 0;
        UIImagePickerController imagePicker;

        public CreateNewTaskView(IntPtr handle) : base(handle)
        {
            appD = UIApplication.SharedApplication.Delegate as AppDelegate;
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
                var touchSuperView = touch.View.Superview.GetType().Name;
                if (touchSuperView == "ControlTextInputMultiLine" || touchSuperView == "ControlTextInput" || touchSuperView == "ControlTextInputFormat")
                    currentViewEditing = touch.View.Superview;

                var touchView = touch.View.Class.Name;
                if (touchView == "UITableViewCellContentView" || touchView == "UIButton")
                    return false;
                else
                    HandleMenuOptionResult(null);

                return true;
            };

            gesture.CancelsTouchesInView = false;
            View.AddGestureRecognizer(gesture);

            ViewConfiguration();
            LoadContent();

            #region delegate
            BT_avatar.TouchUpInside += BT_avatar_TouchUpInside;
            BT_groupWorkFlow.TouchUpInside += BT_groupWorkFlow_TouchUpInside;
            BT_add.TouchUpInside += BT_add_TouchUpInside;
            BT_save.TouchUpInside += BT_save_TouchUpInside;
            BT_send.TouchUpInside += BT_send_TouchUpInside;
            BT_exit.TouchUpInside += BT_exit_TouchUpInside;
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

        #region public - private method
        private void ViewConfiguration()
        {
            buttonActionTopBar = ButtonsActionTopBar.Instance;
            buttonActionTopBar.InitFrameView(view_top_bar.Frame);
            view_top_bar.AddSubviews(buttonActionTopBar);

            buttonActionBotBar = ButtonsActionBotBar.Instance;
            buttonActionBotBar.InitFrameView(view_bot_bar.Frame);
            buttonActionBotBar.LoadStatusButton(-1);
            view_bot_bar.AddSubviews(buttonActionBotBar);

            CmmIOSFunction.MakeCornerTopLeftRight(view_lstWorkFlow, 8);

            view_popupCreate.BackgroundColor = UIColor.Black.ColorWithAlpha(0.5f);
            BT_add.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_save.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            BT_send.ContentEdgeInsets = new UIEdgeInsets(7, 5, 7, 5);
            BT_exit.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);
            table_content_right.ContentInset = new UIEdgeInsets(10, 0, 20, 0);

            CmmIOSFunction.CreateCircleButton(BT_avatar);
            string documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string localpath = Path.Combine(documentFolder, CmmVariable.M_Avatar);

            BT_avatar.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            if (File.Exists(localpath))
                BT_avatar.SetImage(UIImage.FromFile(localpath), UIControlState.Normal);
            else
                BT_avatar.SetImage(UIImage.FromFile("Icons/icon_avatar_temp.png"), UIControlState.Normal);

            string str_hintSearch = "Tìm kiếm...";
            var attHintSearch = new NSMutableAttributedString(str_hintSearch);
            attHintSearch.AddAttribute(UIStringAttributeKey.ForegroundColor, UIColor.White, new NSRange(0, str_hintSearch.Length));

            tf_search.AttributedPlaceholder = attHintSearch;

            table_groupWorkFlow.ContentInset = new UIEdgeInsets(0, 0, 0, 0);

            CmmIOSFunction.AddShadowForTopORBotBar(view_top, true);
            CmmIOSFunction.AddShadowForTopORBotBar(view_bot_bar, false);
        }

        private void LoadContent()
        {
            //BeanWorkflow item = new BeanWorkflow() { ID = 0, iconUrl = "icon_learn_temp.png", title = "Đăng kí đi làm ngoài giờ", isExpand = true, workFlowID = 2 };
            //BeanWorkflow item1 = new BeanWorkflow() { ID = 1, iconUrl = "icon_pay_temp.png", title = "Thanh toán công tác phí", isExpand = true, workFlowID = 1 };
            //BeanWorkflow item2 = new BeanWorkflow() { ID = 2, iconUrl = "icon_recruitment_temp.png", title = "Đề nghị tuyển dụng", isExpand = true, workFlowID = 3 };
            //BeanWorkflow item3 = new BeanWorkflow() { ID = 3, iconUrl = "icon_training_temp.png", title = "Đề nghị tuyển dụng", isExpand = true, workFlowID = 4 };
            //BeanWorkflow item4 = new BeanWorkflow() { ID = 4, iconUrl = "", title = "Quy trình đào tạo", isExpand = true, workFlowID = 5 };

            //List<BeanWorkflow> lst_item = new List<BeanWorkflow>();
            //lst_item.AddRange(new[] { item, item1, item2, item3, item4, item2, item3, item4 });

            //List<BeanWorkflow> lst_item1 = new List<BeanWorkflow>();
            //lst_item1.AddRange(new[] { item1, item2 });

            //List<BeanWorkflow> lst_item2 = new List<BeanWorkflow>();
            //lst_item2.AddRange(new[] { item2, item1, item2 });

            //List<BeanWorkflow> lst_item3 = new List<BeanWorkflow>();
            //lst_item3.AddRange(new[] { item3, item1, item2, item3, item4, item1, item2 });

            //dict_groupWorkFlow.Add("Nhân sự", lst_item);
            //dict_groupWorkFlow.Add("Marketing", lst_item1);
            //dict_groupWorkFlow.Add("Kinh doanh", lst_item2);
            //dict_groupWorkFlow.Add("Kế toán", lst_item3);

            table_groupWorkFlow.Source = new GroupWorkFlow_TableSource(this, dict_groupWorkFlow);

            ClassMenu m1 = new ClassMenu() { ID = 0, section = -1, title = "Tất cả", isSelected = true };
            ClassMenu m2 = new ClassMenu() { ID = 1, section = 0, title = "Nhân sự" };
            ClassMenu m3 = new ClassMenu() { ID = 2, section = 1, title = "Marketing" };
            ClassMenu m4 = new ClassMenu() { ID = 3, section = 2, title = "Kinh doanh" };
            ClassMenu m5 = new ClassMenu() { ID = 4, section = 3, title = "Kế toán" };

            lst_menuItem.AddRange(new[] { m1, m2, m3, m4, m5 });
            currentMenu = lst_menuItem[0];

            string dataString1 = @"[{'ShowType':true,'IsShowHint':false,'ViewRows':[{'RowType':1,'Elements':[{'DataType':'textinput','DataSource':null,'ListProprety':null,'ID':1,'Title':'Về việc','Value':'','Enable':true,'IsRequire':false}],'ID':1,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinput','DataSource':null,'ListProprety':null,'ID':1,'Title':'Kính trình','Value':'','Enable':true,'IsRequire':false}],'ID':2,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputformat','DataSource':null,'ListProprety':null,'ID':1,'Title':'Căn cứ','Value':'','Enable':true,'IsRequire':false}],'ID':3,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputmultiline','DataSource':null,'ListProprety':null,'ID':1,'Title':'Chi tiết','Value':'','Enable':true,'IsRequire':true}],'ID':4,'Title':null,'Value':null,'Enable':false},{'RowType':3,'Elements':[{'DataType':null,'DataSource':null,'ListProprety':null,'ID':1,'Title':'Người soạn thảo','Value':'Test Admin','Enable':false,'IsRequire':false},{'DataType':null,'DataSource':null,'ListProprety':null,'ID':2,'Title':'Chức vụ','Value':'Nhân viên Kinh doanh','Enable':false,'IsRequire':false},{'DataType':null,'DataSource':null,'ListProprety':null,'ID':3,'Title':'Đơn vị','Value':'Vũ Thảo Co. ltd','Enable':false,'IsRequire':false}],'ID':5,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputmultiline','DataSource':null,'ListProprety':null,'ID':1,'Title':'Ghi chú','Value':'','Enable':true,'IsRequire':false}],'ID':6,'Title':null,'Value':null,'Enable':false}],'ID':1,'Title':'Báo Cáo','Value':null,'Enable':false}]";
            string dataString2 = @"[{'ShowType':true,'IsShowHint':false,'ViewRows':[{'RowType':1,'Elements':[{'DataType':'textinput','DataSource':null,'ListProprety':null,'ID':1,'Title':'Về việc','Value':'','Enable':true,'IsRequire':false}],'ID':1,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinput','DataSource':null,'ListProprety':null,'ID':1,'Title':'Kính trình','Value':'','Enable':true,'IsRequire':false}],'ID':2,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputformat','DataSource':null,'ListProprety':null,'ID':1,'Title':'Căn cứ','Value':'','Enable':true,'IsRequire':false}],'ID':3,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputmultiline','DataSource':null,'ListProprety':null,'ID':1,'Title':'Chi tiết','Value':'','Enable':true,'IsRequire':true}],'ID':4,'Title':null,'Value':null,'Enable':false},{'RowType':3,'Elements':[{'DataType':null,'DataSource':null,'ListProprety':null,'ID':1,'Title':'Người soạn thảo','Value':'Test Admin','Enable':false,'IsRequire':false},{'DataType':null,'DataSource':null,'ListProprety':null,'ID':2,'Title':'Chức vụ','Value':'Nhân viên Kinh doanh','Enable':false,'IsRequire':false},{'DataType':null,'DataSource':null,'ListProprety':null,'ID':3,'Title':'Đơn vị','Value':'Vũ Thảo Co. ltd','Enable':false,'IsRequire':false}],'ID':5,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'textinputmultiline','DataSource':null,'ListProprety':null,'ID':1,'Title':'Ghi chú','Value':'','Enable':true,'IsRequire':false}],'ID':6,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'inputattachmentvertical','DataSource':null,'ListProprety':null,'ID':4,'Title':'File đính kèm','Value':'','Enable':false}],'ID':7,'Title':null,'Value':null,'Enable':false},{'RowType':1,'Elements':[{'DataType':'inputworkrelated','DataSource':null,'ListProprety':null,'ID':4,'Title':'Quy trình / Công việc liên kết','Value':'','Enable':false}],'ID':8,'Title':null,'Value':null,'Enable':false}],'ID':1,'Title':'Báo Cáo','Value':null,'Enable':false}]";

            JArray json = JArray.Parse(dataString2);
            lst_section = json.ToObject<List<ViewSection>>();

            table_content_right.Source = new Control_TableSource(lst_section, this);
            table_content_right.ReloadData();
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
            imagePicker.DismissModalViewController(true);
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

        #region event
        private void BT_avatar_TouchUpInside(object sender, EventArgs e)
        {
            appD.menu.UpdateItemSelect(-1);
            appD.SlideMenuController.OpenLeft();
        }

        private void BT_groupWorkFlow_TouchUpInside(object sender, EventArgs e)
        {
            Custom_MenuOption custom_menuOption = Custom_MenuOption.Instance;
            if (custom_menuOption.Superview != null)
                custom_menuOption.RemoveFromSuperview();
            else
            {
                custom_menuOption.ItemNoIcon = false;
                custom_menuOption.viewController = this;
                custom_menuOption.InitFrameView(new CGRect(BT_groupWorkFlow.Frame.Left, BT_groupWorkFlow.Frame.Bottom + 2, BT_groupWorkFlow.Frame.Width, lst_menuItem.Count * custom_menuOption.RowHeigth));
                custom_menuOption.AddShadowForView();
                //custom_menuOption.ListItemMenu = lst_menuItem;
                custom_menuOption.TableLoadData();

                view_lstWorkFlow.AddSubview(custom_menuOption);
                view_lstWorkFlow.BringSubviewToFront(custom_menuOption);
            }
        }

        private void BT_add_TouchUpInside(object sender, EventArgs e)
        {
            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            FormAddWorkRelatedView formAddWorkRelated = (FormAddWorkRelatedView)Storyboard.InstantiateViewController("FormAddWorkRelatedView");
            formAddWorkRelated.SetContent(this);
            PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            formAddWorkRelated.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            formAddWorkRelated.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            formAddWorkRelated.TransitioningDelegate = transitioningDelegate;
            this.PresentViewControllerAsync(formAddWorkRelated, true);
        }

        private async void BT_send_TouchUpInside(object sender, EventArgs e)
        {
            string name = await CmmIOSFunction.CheckFileLocalIsExist("https://file-examples.com/wp-content/uploads/2017/10/file-sample_150kB.pdf");
            string name1 = await CmmIOSFunction.CheckFileLocalIsExist("https://file-examples.com/wp-content/uploads/2017/02/file-sample_100kB.doc");
        }

        private void BT_save_TouchUpInside(object sender, EventArgs e)
        {
            NSError err = null;
            string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var FileManager = new NSFileManager();
            var lst_file = FileManager.GetDirectoryContent(localDocumentFilepath, out err);

            Console.WriteLine("Group Path: " + lst_file);
        }

        private void BT_exit_TouchUpInside(object sender, EventArgs e)
        {
            view_popupCreate.Hidden = true;
        }

        public void HandleMenuOptionResult(ClassMenu _menu)
        {
            if (_menu != null)
            {
                _menu.isSelected = true;

                if (currentMenu != null && currentMenu.ID != _menu.ID)
                {
                    currentMenu.isSelected = false;
                    currentMenu = _menu;

                    if (_menu.section == -1)
                    {
                        table_groupWorkFlow.Source = new GroupWorkFlow_TableSource(this, dict_groupWorkFlow);
                        table_groupWorkFlow.ReloadData();
                    }
                    else
                    {
                        if (dict_groupWorkFlow_fillter.Count > 0)
                            dict_groupWorkFlow_fillter.Clear();

                        //dict_groupWorkFlow_fillter.Add(currentMenu.title, dict_groupWorkFlow[currentMenu.title]);

                        //table_groupWorkFlow.Source = new GroupWorkFlow_TableSource(this, dict_groupWorkFlow_fillter);
                        //table_groupWorkFlow.ReloadData();
                    }
                }
            }

            Custom_MenuOption custom_menuOption = Custom_MenuOption.Instance;
            if (custom_menuOption.Superview != null)
                custom_menuOption.RemoveFromSuperview();
        }

        public void HandleGroupWorkFlowCellResult(BeanWorkflow _workFlowItem)
        {
            view_popupCreate.Hidden = false;
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
                        //TEditorResponse response = await ShowTEditor(contentHtmlString, null, true);

                        //appD.NavController.NavigationBarHidden = true;
                        //if (response.IsSave)
                        //{
                        //    if (response.HTML != null)
                        //    {
                        //        contentHtmlString = response.HTML;
                        //        SetValueResult(contentHtmlString);
                        //    }
                        //}

                        break;
                    case "ControlInputAttachmentVetical":
                        var controlInputAttachment = (ControlInputAttachmentVetical)_controlBase;
                        CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
                        CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
                        FormEditAttachFileView formEditAttachFileView = (FormEditAttachFileView)Storyboard.InstantiateViewController("FormEditAttachFileView");
                        formEditAttachFileView.SetContent(this, controlInputAttachment.currentAttachment);
                        PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
                        formEditAttachFileView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                        formEditAttachFileView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
                        formEditAttachFileView.TransitioningDelegate = transitioningDelegate;
                        this.PresentViewControllerAsync(formEditAttachFileView, true);
                        break;
                    default:
                        break;
                }
            }
        }

        public void SetValueResult(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (currentElement != null)
                {
                    currentElement.Value = value;
                    table_content_right.ReloadRows(new NSIndexPath[] { currentIndexPath }, UITableViewRowAnimation.None);
                }
            }
        }

        private void KeyBoardUpNotification(NSNotification notification)
        {
            try
            {
                if (currentViewEditing != null)
                {
                    switch (currentViewEditing.GetType().Name)
                    {
                        case "ControlTextInput":
                            controllActive = (ControlTextInput)currentViewEditing;
                            break;
                        case "ControlTextInputMultiLine":
                            controllActive = (ControlTextInputMultiLine)currentViewEditing;
                            break;
                        default:
                            controllActive = null;
                            break;
                    }

                    var rectRow = table_content_right.RectForRowAtIndexPath(controllActive.IndexPath);
                    table_content_right.SetContentOffset(new CGPoint(0, rectRow.Y), true);
                }
            }
            catch (Exception ex)
            { Console.WriteLine("CreateNewTaskView - Err: " + ex.ToString()); }
        }

        private void KeyBoardDownNotification(NSNotification notification)
        {
            try
            {
                if (controllActive != null)
                    table_content_right.ScrollToRow(controllActive.IndexPath, UITableViewScrollPosition.Top, true);
            }
            catch (Exception ex)
            { Console.WriteLine("CreateNewTaskView - Err: " + ex.ToString()); }
        }

        //public Task<TEditorResponse> ShowTEditor(string html, ToolbarBuilder toolbarBuilder = null, bool autoFocusInput = false)
        //{
        //    TaskCompletionSource<TEditorResponse> taskRes = new TaskCompletionSource<TEditorResponse>();
        //    var tvc = new TEditorViewController();
        //    ToolbarBuilder builder = toolbarBuilder;
        //    if (toolbarBuilder == null)
        //        builder = new ToolbarBuilder().AddStandard();
        //    tvc.BuildToolbar(builder);
        //    tvc.SetHTML(html);
        //    tvc.SetAutoFocusInput(autoFocusInput);
        //    tvc.Title = currentElement.Title;

        //    UINavigationController nav = appD.NavController;
        //    appD.NavController.NavigationBar.BackgroundColor = UIColor.FromRGB(65, 80, 134);
        //    appD.NavController.NavigationBar.BarTintColor = UIColor.FromRGB(65, 80, 134);
        //    appD.NavController.NavigationBar.TintColor = UIColor.White;
        //    appD.NavController.NavigationBar.TitleTextAttributes = new UIStringAttributes
        //    {
        //        ForegroundColor = UIColor.White,
        //        Font = UIFont.SystemFontOfSize(15f, UIFontWeight.Bold)
        //    };

        //    foreach (var vc in
        //        UIApplication.SharedApplication.Windows[0].RootViewController.ChildViewControllers)
        //    {
        //        if (vc is UINavigationController)
        //            nav = (UINavigationController)vc;
        //    }

        //    //tvc.NavigationItem.SetLeftBarButtonItem(new UIBarButtonItem(CrossTEditor.CancelText, UIBarButtonItemStyle.Plain, (item, args) =>
        //    //{
        //    //    if (nav != null)
        //    //        nav.PopViewController(false);

        //    //    taskRes.SetResult(new TEditorResponse() { IsSave = false, HTML = string.Empty });
        //    //}), true);

        //    //UIBarButtonItem barBtnLeft = new UIBarButtonItem(UIImage.FromFile("Icons/icon_arrow_left.png"), UIBarButtonItemStyle.Plain, (item, args) =>
        //    //{
        //    //    if (nav != null)
        //    //        nav.PopViewController(false);

        //    //    taskRes.SetResult(new TEditorResponse() { IsSave = false, HTML = string.Empty });
        //    //});
        //    //barBtnLeft.ImageInsets = new UIEdgeInsets(8, -8, -8, -16);

        //    //tvc.NavigationItem.SetLeftBarButtonItem(barBtnLeft, false);

        //    //tvc.NavigationItem.SetRightBarButtonItem(new UIBarButtonItem(CrossTEditor.SaveText, UIBarButtonItemStyle.Done, async (item, args) =>
        //    //{
        //    //    if (nav != null)
        //    //        nav.PopViewController(false);

        //    //    taskRes.SetResult(new TEditorResponse() { IsSave = true, HTML = await tvc.GetHTML() });
        //    //}), true);

        //    if (nav != null)
        //        nav.PushViewController(tvc, false);

        //    appD.NavController.NavigationBarHidden = false;

        //    return taskRes.Task;
        //}

        public void NavigateToDetails(BeanNotify _notify, bool isTodo = true)
        {
            if (isTodo)
            {
                if (toDoDetailView == null)
                    toDoDetailView = (ToDoDetailView)this.Storyboard.InstantiateViewController("ToDoDetailView");

                if (appD.SlideMenuController.MainViewController.GetType() != typeof(ToDoDetailView))
                {
                    appD.SlideMenuController.ChangeMainViewcontroller(toDoDetailView, true);
                    ButtonsActionBotBar buttonActionBotBar = ButtonsActionBotBar.Instance;
                    buttonActionBotBar.LoadStatusButton(1);
                }
            }
            else
            {
                if (workflowDetailView == null)
                    workflowDetailView = (WorkflowDetailView)this.Storyboard.InstantiateViewController("WorkflowDetailView");

                if (appD.SlideMenuController.MainViewController.GetType() != typeof(WorkflowDetailView))
                {
                    appD.SlideMenuController.ChangeMainViewcontroller(workflowDetailView, true);
                    ButtonsActionBotBar buttonActionBotBar = ButtonsActionBotBar.Instance;
                    buttonActionBotBar.LoadStatusButton(2);
                }
            }
        }

        public void HandleAddAttachment(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase)
        {
            try
            {
                this.View.EndEditing(true);

                Custom_AttachFileView custom_menuOption = Custom_AttachFileView.Instance;
                custom_menuOption.viewController = this;
                custom_menuOption.InitFrameView(new CGRect(0, 0, view_content.Frame.Width, view_content.Frame.Height));
                custom_menuOption.TableLoadData();

                view_content.AddSubview(custom_menuOption);
                view_content.BringSubviewToFront(custom_menuOption);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("CreateNewTaskView - HandleAddAttachment - Err: " + ex.ToString());
#endif
            }
        }

        public void HandleAddAttachFileResult(BeanAttachFileLocal _attachFile)
        {
            ViewElement viewElement = GetViewElementByDataType("inputattachmentvertical");

            List<BeanAttachFile> lst_item = new List<BeanAttachFile>();
            if (!string.IsNullOrEmpty(viewElement.Value))
            {
                JArray json = JArray.Parse(viewElement.Value);
                lst_item = json.ToObject<List<BeanAttachFile>>();
            }

            numRowAttachmentFile++;
            string custID = numRowAttachmentFile + "";
            BeanAttachFile attachFile = new BeanAttachFile() { ID = custID, Title = _attachFile.Name, Path = _attachFile.Path, Size = _attachFile.Size };
            lst_item.Add(attachFile);

            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lst_item);
            viewElement.Value = jsonString;

            HandleAttachFileClose();
            table_content_right.ReloadData();
        }

        public void HandleRemoveAttachment(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase, int numRow)
        {
            numRowAttachmentFile = numRow;
            table_content_right.ReloadData();
        }

        public void HandleAttachFileClose()
        {
            Custom_AttachFileView custom_menuOption = Custom_AttachFileView.Instance;
            if (custom_menuOption.Superview != null)
                custom_menuOption.RemoveFromSuperview();
        }

        public void HandleEditAttachFileResult(BeanAttachFile _beanAttachFile)
        {
            ViewElement viewElement = GetViewElementByDataType("inputattachmentvertical");

            List<BeanAttachFile> lst_item = new List<BeanAttachFile>();
            if (!string.IsNullOrEmpty(viewElement.Value))
            {
                JArray json = JArray.Parse(viewElement.Value);
                lst_item = json.ToObject<List<BeanAttachFile>>();
            }

            var index = lst_item.FindIndex(item => item.ID == _beanAttachFile.ID);
            lst_item[index].Type = _beanAttachFile.Type;

            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lst_item);
            viewElement.Value = jsonString;

            table_content_right.ReloadData();
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

            table_content_right.ReloadData();
        }

        public void HandleWorkRelatedResult(ViewElement element, NSIndexPath indexPath, ControlBase _controlBase, int numRow)
        {
            numRowWorkRelated = numRow;
            table_content_right.ReloadData();
        }
        #endregion

        #region cust view
        #region group work flow table source
        private class GroupWorkFlow_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cell");
            List<string> lst_key = new List<string>();
            Dictionary<string, List<BeanWorkflow>> dict_groupWorkFlow { get; set; }
            CreateNewTaskView parentView;
            const int HEIGHT_HEADER = 30;
            const int HEIGHT_ROW = 163;

            public GroupWorkFlow_TableSource(CreateNewTaskView _parentview, Dictionary<string, List<BeanWorkflow>> _dict_todo)
            {
                parentView = _parentview;
                dict_groupWorkFlow = _dict_todo;
                GetListKey();
            }

            private void GetListKey()
            {
                lst_key = dict_groupWorkFlow.Keys.ToList();
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return dict_groupWorkFlow.Count;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                var keyItem = lst_key[Convert.ToInt32(section)];
                var fistItem = dict_groupWorkFlow[keyItem][0];
                return fistItem.isExpand ? 1 : 0;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                var keyItem = lst_key[indexPath.Section];
                var count = dict_groupWorkFlow[keyItem].Count % 5;
                var temp = dict_groupWorkFlow[keyItem].Count / 5;

                return (count > 0) ? (HEIGHT_ROW * (temp + 1)) + 25 : (HEIGHT_ROW * temp) + 25;//footer height: 25
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                return HEIGHT_HEADER;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                tableView.DeselectRow(indexPath, true);
            }

            public override UIView GetViewForHeader(UITableView tableView, nint section)
            {
                var keyItem = lst_key[Convert.ToInt32(section)];
                var fistItem = dict_groupWorkFlow[keyItem][0];

                UIView rooView = new UIView();
                rooView.Frame = new CGRect(0, 0, tableView.Frame.Width, HEIGHT_HEADER);
                rooView.BackgroundColor = UIColor.White;

                UILabel lbl_title = new UILabel()
                {
                    Font = UIFont.BoldSystemFontOfSize(14),
                    TextColor = UIColor.FromRGB(65, 85, 134)
                };

                UIImageView iv_left = new UIImageView();
                iv_left.ContentMode = UIViewContentMode.ScaleAspectFit;
                UIImage img_left = UIImage.FromFile("Icons/icon_arrow_down_white.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                iv_left.Image = img_left;
                iv_left.TintColor = UIColor.FromRGB(65, 80, 134);

                UIButton btn_action = new UIButton();
                btn_action.TouchUpInside += (sender, ev) =>
                {
                    //fistItem.isExpand = !fistItem.isExpand;
                    tableView.ReloadSections(new NSIndexSet((uint)section), UITableViewRowAnimation.None);
                };

                rooView.AddSubviews(new UIView[] { iv_left, lbl_title, btn_action });

                iv_left.Frame = new CGRect(0, 9, 12, 12);
                lbl_title.Frame = new CGRect(iv_left.Frame.Right + 10, 0, rooView.Frame.Width - (iv_left.Frame.Right + 10), rooView.Frame.Height);
                btn_action.Frame = new CGRect(0, 0, rooView.Bounds.Width, rooView.Bounds.Height);

                lbl_title.Text = keyItem;

                if (fistItem.isExpand)
                    iv_left.Transform = CGAffineTransform.MakeRotation(0);
                else
                    iv_left.Transform = CGAffineTransform.MakeRotation(-((nfloat)Math.PI / 2));

                return rooView;
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var keyItem = lst_key[indexPath.Section];
                var lst_workFlow = dict_groupWorkFlow[keyItem];

                Custom_GroupWorkFlowCell cell = new Custom_GroupWorkFlowCell(cellIdentifier, parentView);
                cell.UpdateCell(lst_workFlow);

                return cell;
            }
        }
        #endregion

        #region dynamic controls source table
        private class Control_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cell");
            CreateNewTaskView parentView;
            List<ViewSection> lst_section;
            Dictionary<string, List<ViewRow>> dict_control = new Dictionary<string, List<ViewRow>>();

            public Control_TableSource(List<ViewSection> _lst_section, CreateNewTaskView _parentview)
            {
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
                return 0;
            }

            public override nfloat GetHeightForFooter(UITableView tableView, nint section)
            {
                return -1;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                var dataType = dict_control[lst_section[indexPath.Section].ID][indexPath.Row].Elements[0].DataType;
                switch (dataType)
                {
                    case "textinputmultiline":
                    case "textinputformat":
                        return 115;
                    case "inputattachmentvertical":
                        return 75 + 10 + (60 * parentView.numRowAttachmentFile);//header height: 75 - cell row height: 60 - padding top của table : 10
                    case "inputworkrelated":
                        return 40 + (76 * parentView.numRowWorkRelated);//title height: 40 - cell row height: 76
                    case "textinput":
                        return 85;
                    default:
                        return 65;
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

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var control = dict_control[lst_section[indexPath.Section].ID][indexPath.Row];
                Control_cell_custom cell = new Control_cell_custom(parentView, cellIdentifier, control, indexPath);
                return cell;
            }
        }
        private class Control_cell_custom : UITableViewCell
        {
            CreateNewTaskView parentView { get; set; }
            ViewRow control { get; set; }
            NSIndexPath currentIndexPath { get; set; }
            public ComponentBase components;

            public Control_cell_custom(CreateNewTaskView _parentView, NSString cellID, ViewRow _control, NSIndexPath _currentIndexPath) : base(UITableViewCellStyle.Default, cellID)
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

                components.InitializeFrameView(new CGRect(18, 0, ContentView.Frame.Width - 36, ContentView.Frame.Height));
            }
        }
        #endregion
        #endregion
    }

    public class WorkFlowItemDemo
    {
        public int ID { get; set; }
        public string title { get; set; }
        public string iconUrl { get; set; }
        public bool isExpand { get; set; }
        public int workFlowID { get; set; }
    }
}
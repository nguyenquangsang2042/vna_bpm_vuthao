using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json.Linq;
using UIKit;
using static BPMOPMobile.Class.CmmFunction;

namespace BPMOPMobile.iPad.Components
{
    class ControlAttachmentVerticalWithFormFrame : ControlBase
    {
        UIViewController parentView { get; set; }
        NSIndexPath indexPath { get; set; }
        ViewElement element { get; set; }
        List<BeanAttachFile> lst_attachment = new List<BeanAttachFile>();
        UITableView tableView_attachment;
        List<KeyValuePair<string, bool>> lst_sectionState;
        public BeanAttachFile currentAttachment { get; set; }

        UIView view_header;
        UILabel lbl_type, lbl_name, lbl_typeName;
        UIButton btn_add;

        public ControlAttachmentVerticalWithFormFrame(UIViewController _parentView, ViewElement _element, NSIndexPath _indexPath)
        {
            parentView = _parentView;
            element = _element;
            indexPath = _indexPath;
            InitializeComponent();
        }

        public override void InitializeComponent()
        {
            base.InitializeComponent();

            view_header = new UIView();
            view_header.BackgroundColor = UIColor.FromRGB(229, 229, 229);

            lbl_name = new UILabel()
            {
                Font = UIFont.FromName("Arial-BoldMT", 14),
                Text = CmmFunction.GetTitle("TEXT_DOCUMENTNAME", "Tên tài liệu")
            };

            lbl_typeName = new UILabel()
            {
                Font = UIFont.FromName("Arial-BoldMT", 14),
                TextAlignment = UITextAlignment.Left,
                Text = CmmFunction.GetTitle("TEXT_CREATOR", "Người tạo")
            };

            view_header.AddSubviews(new UIView[] { lbl_name, lbl_typeName });

            tableView_attachment = new UITableView();
            tableView_attachment.ScrollEnabled = false;

            btn_add = new UIButton();
            btn_add.SetTitle(CmmFunction.GetTitle("TEXT_ADDNEW_ATTACHMENT", "Tạo mới"), UIControlState.Normal);
            btn_add.Font = UIFont.FromName("ArialMT", 12);
            btn_add.SetImage(UIImage.FromFile("Icons/icon_document_add.png"), UIControlState.Normal);
            btn_add.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);
            //btn_add.Font = UIFont.SystemFontOfSize(14);

            if (element.Enable)
                btn_add.Hidden = false;
            else
                btn_add.Hidden = true;

            this.ClipsToBounds = true;
            this.Add(view_header);
            this.Add(tableView_attachment);
            this.Add(btn_add);

            this.WillRemoveSubview(lbl_value);
            this.WillRemoveSubview(BT_action);
            this.WillRemoveSubview(lbl_line);

            if (btn_add != null)
                btn_add.AddTarget(HandleBtnAdd, UIControlEvent.TouchUpInside);
        }

        public override void InitializeFrameView(CGRect frame)
        {
            this.Frame = frame;

            const int paddingTop = 5;
            const int spaceView = 5;
            var width = Frame.Width;
            btn_add.Frame = new CGRect(Frame.Width - 110, paddingTop, 100, 20);

            lbl_title.HeightAnchor.ConstraintEqualTo(20).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1.0f, paddingTop).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;

            if (frame.Height > 80)
            {
                view_header.Frame = new CGRect(0, 20 + paddingTop + spaceView, width, 40);
                lbl_name.Frame = new CGRect(20, 5, (width / 3) * 1.8f, 40);
                lbl_typeName.Frame = new CGRect(lbl_name.Frame.Right, 5, width - lbl_name.Frame.Right, 40);
                var topAnchor = (20 + paddingTop + spaceView + 50);
                tableView_attachment.Frame = new CGRect(0, view_header.Frame.Bottom, frame.Width, frame.Height - topAnchor);
            }

            if (UIDevice.CurrentDevice.CheckSystemVersion(15, 0))
                tableView_attachment.SectionHeaderTopPadding = 0;

            //set image left button add
            btn_add.ImageView.Frame = new CGRect(0, 0, 22, 22);
            btn_add.ImageEdgeInsets = new UIEdgeInsets(top: 0, left: 0, bottom: 0, right: 0);
            btn_add.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
        }

        public override void SetTitle()
        {
            base.SetTitle();

            if (!element.IsRequire)
                lbl_title.Text = element.Title;
            else
                lbl_title.Text = element.Title + " (*)";
        }

        public override void SetEnable()
        {
            base.SetEnable();
            if (!element.Enable)
            {
                BT_action.Hidden = true;
                //BT_action.UserInteractionEnabled = false;
            }
            else
            {
                BT_action.Hidden = false;
                //BT_action.UserInteractionEnabled = true;
            }
        }

        public override void SetValue()
        {
            base.SetValue();

            var data = element.Value.Trim();
            if (!string.IsNullOrEmpty(data) && data != "[]")
            {
                JArray json = JArray.Parse(data);
                lst_attachment = json.ToObject<List<BeanAttachFile>>();
            }

            tableView_attachment.Source = new Attachment_TableSource(lst_attachment, this, null);
            tableView_attachment.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            tableView_attachment.ReloadData();
        }

        public void HandleRemoveItem(BeanAttachFile attachFile)
        {
            UIAlertController alert = UIAlertController.Create("Thông báo", CmmFunction.GetTitle("TEXT_DELETE_CONFIRM", "Bạn có chắc muốn xóa file này không?"), UIAlertControllerStyle.Alert);//"BPM"
            alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"), UIAlertActionStyle.Default, null));
            alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_AGREE", "Đồng ý"), UIAlertActionStyle.Default, alertAction =>
            {
                List<BeanAttachFile> lst_attach_remove = new List<BeanAttachFile>();
                if (parentView != null && parentView.GetType() == typeof(ToDoDetailView))
                {
                    var index = lst_attachment.FindIndex(item => item.ID == attachFile.ID);
                    if (index != -1)
                    {
                        lst_attach_remove.Add(lst_attachment[index]);
                        lst_attachment.RemoveAt(index);
                    }

                    var json_attachRemove = Newtonsoft.Json.JsonConvert.SerializeObject(lst_attach_remove);
                    var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lst_attachment);
                    element.Value = jsonString;

                    ToDoDetailView controller = (ToDoDetailView)parentView;
                    controller.HandleAttachmentRemove(element, indexPath, this, lst_attachment.Count, json_attachRemove);
                }
                else if (parentView.GetType() == typeof(WorkflowDetailView))
                {
                    var index = lst_attachment.FindIndex(item => item.ID == attachFile.ID);
                    if (index != -1)
                    {
                        lst_attach_remove.Add(lst_attachment[index]);
                        lst_attachment.RemoveAt(index);
                    }

                    var json_attachRemove = Newtonsoft.Json.JsonConvert.SerializeObject(lst_attach_remove);
                    var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lst_attachment);
                    element.Value = jsonString;

                    WorkflowDetailView controller = (WorkflowDetailView)parentView;
                    controller.HandleAttachmentRemove(element, indexPath, this, lst_attachment.Count, json_attachRemove);
                }
            }));
            parentView.PresentViewController(alert, true, null);

        }

        public void HandleEditItem(BeanAttachFile attachFile)
        {
            if (parentView != null && parentView.GetType() == typeof(ToDoDetailView))
            {
                ToDoDetailView controller = (ToDoDetailView)parentView;
                controller.HandleAttachmentEdit(element, indexPath, attachFile, this);
            }
            else if (parentView != null && parentView.GetType() == typeof(WorkflowDetailView))
            {
                WorkflowDetailView controller = (WorkflowDetailView)parentView;
                controller.HandleAttachmentEdit(element, indexPath, attachFile, this);
            }
        }

        public void HandleSeclectItem(BeanAttachFile _attachment)
        {
            if (parentView != null && parentView.GetType() == typeof(ToDoDetailView))
            {
                currentAttachment = _attachment;
                if (parentView.GetType() == typeof(ToDoDetailView))
                {
                    ToDoDetailView requestDetailsV2 = (ToDoDetailView)parentView;
                    requestDetailsV2.NavigateToShowAttachView(_attachment);
                }
            }
            else if (parentView != null && parentView.GetType() == typeof(WorkflowDetailView))
            {
                currentAttachment = _attachment;
                if (parentView.GetType() == typeof(WorkflowDetailView))
                {
                    WorkflowDetailView workflowDetail = (WorkflowDetailView)parentView;
                    workflowDetail.NavigateToShowAttachView(_attachment);
                }
            }
            else if (parentView != null && parentView.GetType() == typeof(FormWorkFlowDetails))
            {
                currentAttachment = _attachment;
                if (parentView.GetType() == typeof(FormWorkFlowDetails))
                {
                    FormWorkFlowDetails workflowDetail = (FormWorkFlowDetails)parentView;
                    workflowDetail.NavigateToShowAttachView(_attachment);
                }
            }
        }

        private void HandleBtnAdd(object sender, EventArgs e)
        {
            if (parentView != null && parentView.GetType() == typeof(WorkflowDetailView))
            {
                WorkflowDetailView controller = (WorkflowDetailView)parentView;
                controller.HandleAddAttachment(element, indexPath, this);
            }
            else if (parentView.GetType() == typeof(ToDoDetailView))
            {
                ToDoDetailView requestDetailsV2 = parentView as ToDoDetailView;
                requestDetailsV2.HandleAddAttachment(element, indexPath, this);
            }
            else if (parentView.GetType() == typeof(FormWorkFlowDetails))
            {
                FormWorkFlowDetails requestDetailsV2 = parentView as FormWorkFlowDetails;
                requestDetailsV2.HandleAddAttachment(element, indexPath, this);
            }
        }

        public void UpdateTableSections(KeyValuePair<string, bool> _sectionState)
        {
            var index = lst_sectionState.FindIndex(x => x.Key == _sectionState.Key);
            lst_sectionState[index] = _sectionState;

            tableView_attachment.Source = new Attachment_TableSource(lst_attachment, this, lst_sectionState);
            tableView_attachment.ReloadSections(NSIndexSet.FromIndex(index), UITableViewRowAnimation.None);
        }

        #region custom views
        #region attachment source table
        private class Attachment_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellAttachmentID");
            ControlAttachmentVerticalWithFormFrame parentView;
            List<BeanAttachFile> lst_attachment = new List<BeanAttachFile>();
            Dictionary<string, List<BeanAttachFile>> dict_attachments = new Dictionary<string, List<BeanAttachFile>>();
            List<string> sectionKeys;
            //bool => Colapse
            List<KeyValuePair<string, bool>> lst_sectionState;
            KeyValuePair<string, bool> sectionState;

            public Attachment_TableSource(List<BeanAttachFile> _lst_attachment, ControlAttachmentVerticalWithFormFrame _parentview, List<KeyValuePair<string, bool>> _sectionState)
            {
                lst_attachment = _lst_attachment;
                parentView = _parentview;
                lst_sectionState = _sectionState;
                LoadData();
            }

            private void LoadData()
            {
                if (lst_sectionState == null)
                {
                    lst_sectionState = new List<KeyValuePair<string, bool>>();

                    lst_attachment.Where(i => i.AttachTypeName == null).ToList().ForEach(u => u.AttachTypeName = "");

                    sectionKeys = lst_attachment.Select(x => x.AttachTypeName).Distinct().ToList();

                    foreach (var section in sectionKeys)
                    {
                        KeyValuePair<string, bool> keypair_section;

                        List<BeanAttachFile> lst_item = lst_attachment.Where(x => x.AttachTypeName == section).ToList();

                        dict_attachments.Add(section, lst_item);
                        keypair_section = new KeyValuePair<string, bool>(section, false);

                        lst_sectionState.Add(keypair_section);
                    }

                    parentView.lst_sectionState = lst_sectionState;

                }
                else
                {
                    sectionKeys = lst_attachment.Select(x => x.AttachTypeName).Distinct().ToList();

                    foreach (var section in sectionKeys)
                    {
                        List<BeanAttachFile> lst_item = lst_attachment.Where(x => x.AttachTypeName == section).ToList();
                        dict_attachments.Add(section, lst_item);
                    }
                }
            }

            public override nfloat GetHeightForFooter(UITableView tableView, nint section)
            {
                return -1;
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return dict_attachments.Count;
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                //return 0;
                var key = dict_attachments.ElementAt(Convert.ToInt32(section)).Key;
                if (dict_attachments[key].Count > 0)
                    return 44;
                else
                    return 1;
            }

            public override UIView GetViewForHeader(UITableView tableView, nint section)
            {
                var sec = sectionKeys[(Int16)section];
                var key = dict_attachments.ElementAt(Convert.ToInt32(section)).Key;
                sectionState = lst_sectionState[(int)section];

                if (dict_attachments[key].Count > 0)
                {
                    UIView baseView = new UIView();
                    baseView.Frame = new CGRect(0, 0, tableView.Frame.Width, 44);
                    baseView.BackgroundColor = UIColor.FromRGB(241, 241, 241);

                    UILabel lbl_title = new UILabel()
                    {
                        Font = UIFont.FromName("Arial-BoldMT", 14f),
                        TextColor = UIColor.FromRGB(65, 80, 134)
                    };
                    if (string.IsNullOrEmpty(sec))
                        lbl_title.Text = CmmFunction.GetTitle("TEXT_ADDNEW_ATTACHMENT", "Tạo mới") + string.Format(" ({0})", dict_attachments[key].Count.ToString());
                    else if (sec.Contains(";#"))
                        lbl_title.Text = sec.Split(";#")[1] + string.Format(" ({0})", dict_attachments[key].Count.ToString());
                    else
                        lbl_title.Text = sec + string.Format(" ({0})", dict_attachments[key].Count.ToString());

                    var titleRect = StringExtensions.MeasureString(lbl_title.Text, 16);
                    lbl_title.Frame = new CGRect(20, 7, titleRect.Width + 5, 30);

                    UIImageView img_arrow = new UIImageView();
                    img_arrow.Frame = new CGRect(lbl_title.Frame.Right + 5, 18, 7, 7);
                    img_arrow.ContentMode = UIViewContentMode.ScaleAspectFill;
                    if (lst_sectionState[(int)section].Value)
                        img_arrow.Image = UIImage.FromFile("Icons/icon_collapse_attach.png");
                    else
                        img_arrow.Image = UIImage.FromFile("Icons/icon_expand_acttach.png");

                    UIView bottom_line = new UIView();
                    bottom_line.BackgroundColor = UIColor.FromRGB(240, 240, 240).ColorWithAlpha(0.4f);
                    bottom_line.Frame = new CGRect(0, baseView.Frame.Height - 1, baseView.Frame.Width, 1);

                    UIButton btn_action = new UIButton();
                    btn_action.Frame = new CGRect(0, 0, baseView.Frame.Width, baseView.Frame.Height);

                    //this.AddSubviews(new UIView[] { lbl_top_line, lbl_title, lbl_bot_line, btn_action });
                    baseView.AddSubviews(new UIView[] { lbl_title, img_arrow, bottom_line, btn_action });

                    btn_action.TouchUpInside += delegate
                    {
                        //KeyValuePair<string, bool> section;
                        //section = new KeyValuePair<string, bool>(sectionState.Key, !sectionState.Value);
                        parentView.UpdateTableSections(new KeyValuePair<string, bool>(lst_sectionState[(int)section].Key, !lst_sectionState[(int)section].Value));
                    };

                    return baseView;
                }
                else
                    return null;

            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 60;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                foreach (var i in lst_sectionState)
                {
                    if (i.Key == sectionKeys[(int)section] && i.Value == false)
                        return dict_attachments[sectionKeys[(int)section]].Count;
                    else if (i.Key == sectionKeys[(int)section] && i.Value == true)
                        return 0;
                }
                return 0;
                //return lst_attachment.Count;
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
                var definitionAction = ContextualDefinitionAction(indexPath);
                var flagAction = ContextualFlagAction(indexPath);
                var trailingSwipe = UISwipeActionsConfiguration.FromActions(new UIContextualAction[] { flagAction, definitionAction });

                trailingSwipe.PerformsFirstActionWithFullSwipe = false;
                return trailingSwipe;
            }
            // edit item
            public UIContextualAction ContextualDefinitionAction(NSIndexPath indexPath)
            {
                var action = UIContextualAction.FromContextualActionStyle(UIContextualActionStyle.Normal,
                                                                    CmmFunction.GetTitle("TEXT_EDIT", "Sửa"),
                                                                    (ReadLaterAction, view, success) =>
                                                                    {
                                                                        if (lst_attachment.Count > 0)
                                                                        {
                                                                            var attachment = dict_attachments[lst_attachment[indexPath.Section].AttachTypeName][indexPath.Row];
                                                                            parentView.HandleEditItem(attachment);
                                                                        }
                                                                    });

                action.Image = UIImage.FromFile("Icons/icon_edit.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate).Scale(new CGSize(20, 20), 3);
                action.Image.ApplyTintColor(UIColor.White);
                action.BackgroundColor = UIColor.FromRGB(51, 95, 179);
                return action;
            }
            // delete item
            public UIContextualAction ContextualFlagAction(NSIndexPath indexPath)
            {
                var action = UIContextualAction.FromContextualActionStyle(UIContextualActionStyle.Normal,
                                                                          "",
                                                                          (FlagAction, view, success) =>
                                                                          {
                                                                              if (lst_attachment.Count > 0)
                                                                              {
                                                                                  var attachment = dict_attachments[lst_attachment[indexPath.Section].AttachTypeName][indexPath.Row];
                                                                                  parentView.HandleRemoveItem(attachment);

                                                                              }
                                                                              success(true);
                                                                          });

                action.Image = UIImage.FromFile("Icons/icon_swipe_delete.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate).Scale(new CGSize(20, 20), 3);
                action.Image.ApplyTintColor(UIColor.White);
                action.BackgroundColor = UIColor.FromRGB(235, 52, 46);
                return action;
            }

            public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
            {
                var attachment = dict_attachments[lst_attachment[indexPath.Section].AttachTypeName][indexPath.Row];
                if (attachment.IsAuthor == true)
                    return true;
                else
                    return false;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                //lst_attachment[indexPath.Row];
                var itemSelected = dict_attachments[lst_attachment[indexPath.Section].AttachTypeName][indexPath.Row];
                parentView.HandleSeclectItem(itemSelected);
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                //dict_control[lst_section[indexPath.Section].ID][indexPath.Row];
                var attachment = dict_attachments[lst_attachment[indexPath.Section].AttachTypeName][indexPath.Row];

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
            ControlAttachmentVerticalWithFormFrame parentView { get; set; }
            NSIndexPath currentIndexPath { get; set; }
            BeanAttachFile attachment { get; set; }
            UILabel lbl_name, lbl_creator, lbl_CreatorCover, lbl_size, lbl_chucvu, lbl_line;
            UIImageView img_type, img_Creator;
            bool isOdd;

            public Attachment_cell_custom(ControlAttachmentVerticalWithFormFrame _parentView, NSString cellID, BeanAttachFile _attachment, NSIndexPath _currentIndexPath, bool _isOdd) : base(UITableViewCellStyle.Default, cellID)
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
                    ContentView.BackgroundColor = UIColor.FromRGB(249, 249, 249);


                img_type = new UIImageView();
                img_type.ContentMode = UIViewContentMode.ScaleAspectFill;

                img_Creator = new UIImageView();
                img_Creator.ContentMode = UIViewContentMode.ScaleAspectFill;

                lbl_name = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 14f),
                    TextColor = UIColor.FromRGB(59, 95, 179),
                    TextAlignment = UITextAlignment.Left,
                };

                lbl_CreatorCover = new UILabel();
                lbl_CreatorCover.Layer.CornerRadius = 20;
                lbl_CreatorCover.ClipsToBounds = true;

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

                lbl_line = new UILabel()
                {
                    BackgroundColor = UIColor.FromRGB(144, 164, 174)
                };

                ContentView.AddSubviews(new UIView[] { img_type, img_Creator, lbl_name, lbl_CreatorCover, lbl_creator, lbl_size, lbl_chucvu, lbl_line });
                lbl_line.Hidden = true;
            }

            public void UpdateCell()
            {
                try
                {
                    string fileExt = string.Empty;
                    if (!string.IsNullOrEmpty(attachment.Path))
                        fileExt = attachment.Path.Split('.').Last();

                    switch (fileExt)
                    {
                        case "doc":
                        case "docx":
                            img_type.Image = UIImage.FromFile("Icons/icon_word.png");
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
                        lbl_name.Text = attachment.Title.Split(";#")[0];
                    else
                        lbl_name.Text = attachment.Title;

                    //CreatedBy
                    if (!string.IsNullOrEmpty(attachment.CreatedBy))
                    {
                        List<BeanUser> lst_userResult = new List<BeanUser>();
                        var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath, false);
                        string query_user = string.Format("SELECT * FROM BeanUser WHERE ID =?");
                        lst_userResult = conn.Query<BeanUser>(query_user, attachment.CreatedBy);

                        string user_imagePath = "";
                        if (lst_userResult.Count > 0)
                            user_imagePath = lst_userResult[0].ImagePath;

                        if (string.IsNullOrEmpty(user_imagePath))
                        {

                            lbl_CreatorCover.Hidden = false;
                            img_Creator.Hidden = true;
                            lbl_CreatorCover.Text = CmmFunction.GetAvatarName(attachment.CreatedName);
                            lbl_CreatorCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_CreatorCover.Text));

                        }
                        else
                        {
                            lbl_CreatorCover.Hidden = false;
                            img_Creator.Hidden = true;
                            lbl_CreatorCover.Text = CmmFunction.GetAvatarName(attachment.CreatedName);
                            lbl_CreatorCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_CreatorCover.Text));
                            checkFileLocalIsExist(lst_userResult[0], img_Creator);
                            //kiem tra xong cap nhat lai avatar
                            lbl_CreatorCover.Hidden = true;
                            img_Creator.Hidden = false;
                        }
                    }
                    else
                    {
                        img_Creator.Hidden = false;
                        lbl_CreatorCover.Hidden = true;
                    }
                    if (!string.IsNullOrEmpty(attachment.CreatedName) && attachment.CreatedName.Contains(";#"))
                        lbl_creator.Text = attachment.CreatedBy.Split(";#")[1];
                    else
                        lbl_creator.Text = attachment.CreatedName;

                    lbl_size.Text = FileSizeFormatter.FormatSize(attachment.Size);

                    lbl_chucvu.Text = attachment.CreatedPositon;
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

                                    img_Creator.Hidden = false;
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
                                    img_Creator.Hidden = false;
                                });
                            }
                        });
                    }
                    else
                    {
                        openFile(filename, image_view);
                        img_Creator.Hidden = false;
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
                }
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();

                var width = ContentView.Frame.Width;

                img_type.Frame = new CGRect(20, 15, 18, 18);
                lbl_name.Frame = new CGRect(50, 13, ((width - 50) / 3) * 1.8f, 20);
                lbl_CreatorCover.Frame = new CGRect(lbl_name.Frame.Right, 10, 40, 40);
                img_Creator.Frame = new CGRect(lbl_name.Frame.Right, 10, 40, 40);
                lbl_creator.Frame = new CGRect(img_Creator.Frame.Right + 5, 13, (width - lbl_name.Frame.Right) - 50, 20);
                lbl_size.Frame = new CGRect(lbl_name.Frame.X, lbl_name.Frame.Bottom, lbl_name.Frame.Width, 20);
                lbl_chucvu.Frame = new CGRect(lbl_creator.Frame.X, lbl_creator.Frame.Bottom, lbl_creator.Frame.Width, 20);
                lbl_line.TranslatesAutoresizingMaskIntoConstraints = false;
                lbl_line.HeightAnchor.ConstraintEqualTo(1).Active = true;
                NSLayoutConstraint.Create(this.lbl_line, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
                NSLayoutConstraint.Create(this.lbl_line, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;
                NSLayoutConstraint.Create(this.lbl_line, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, this, NSLayoutAttribute.Bottom, 1.0f, -1).Active = true;

                lbl_CreatorCover.Layer.CornerRadius = img_Creator.Frame.Width / 2;
                lbl_CreatorCover.ClipsToBounds = true;
                img_Creator.Layer.CornerRadius = img_Creator.Frame.Width / 2;
                img_Creator.ClipsToBounds = true;
            }
        }
        #endregion
        #endregion
    }
}
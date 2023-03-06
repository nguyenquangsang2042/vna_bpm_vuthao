using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobileV1.iOS.CustomControlClass;
using BPMOPMobileV1.iOS.IOSClass;
using BPMOPMobileV1.iOS.ViewControllers;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json.Linq;
using UIKit;
using static BPMOPMobile.Class.CmmFunction;


namespace BPMOPMobileV1.iOS.Components
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
        //public List<BeanAttachFile> lst_attach_remove;

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

            lbl_title.Font = UIFont.BoldSystemFontOfSize(12);
            lbl_title.TextColor = UIColor.FromRGB(0, 0, 0);
            view_header = new UIView();
            view_header.BackgroundColor = UIColor.FromRGB(249, 249, 249);

            lbl_name = new UILabel()
            {
                Font = UIFont.FromName("Arial-BoldMT", 11),
                Text = CmmFunction.GetTitle("TEXT_DOCUMENTNAME", "Tên tài liệu")
            };

            lbl_typeName = new UILabel()
            {
                Font = UIFont.FromName("Arial-BoldMT", 11),
                TextAlignment = UITextAlignment.Left,
                Text = CmmFunction.GetTitle("TEXT_CREATOR", "Người tạo")
            };

            view_header.AddSubviews(new UIView[] { lbl_name, lbl_typeName });

            tableView_attachment = new UITableView();

            btn_add = new UIButton();
            btn_add.SetTitle(CmmFunction.GetTitle("TEXT_ADDNEW_ATTACHMENT", "Tạo mới"), UIControlState.Normal);
            btn_add.Font = UIFont.FromName("ArialMT", 11);
            btn_add.SetImage(UIImage.FromFile("Icons/icon_document_add.png"), UIControlState.Normal);
            btn_add.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);

            if (element.Enable)
                btn_add.Hidden = false;
            else
                btn_add.Hidden = true;

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
            btn_add.Frame = new CGRect(Frame.Width - 120, paddingTop, 100, 20);

            lbl_title.HeightAnchor.ConstraintEqualTo(20).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1.0f, paddingTop).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;

            view_header.Frame = new CGRect(0, 20 + paddingTop + spaceView, width, 50);
            lbl_name.Frame = new CGRect(20, 0, (width / 3) * 1.8f, 50);
            lbl_typeName.Frame = new CGRect(lbl_name.Frame.Right, 0, width - lbl_name.Frame.Right, 50);

            var topAnchor = (20 + paddingTop + spaceView + 50);
            tableView_attachment.Frame = new CGRect(0, topAnchor, frame.Width, frame.Height - topAnchor);

            //set image left button add
            btn_add.TitleEdgeInsets = new UIEdgeInsets(3, 0, -3, 0);
            btn_add.ImageView.Frame = new CGRect(0, 0, 22, 22);
            btn_add.ImageEdgeInsets = new UIEdgeInsets(top: 0, left: 0, bottom: 0, right: 0);
            btn_add.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;


            //bo goc cho view header
            view_header.ClipsToBounds = true;
            UIBezierPath mPath_view_header = UIBezierPath.FromRoundedRect(view_header.Layer.Bounds, (UIRectCorner.TopLeft | UIRectCorner.TopRight), new CGSize(width: 6, height: 6));
            CAShapeLayer maskLayer_view_header = new CAShapeLayer();
            maskLayer_view_header.Frame = view_header.Layer.Bounds;
            maskLayer_view_header.Path = mPath_view_header.CGPath;
            view_header.Layer.Mask = maskLayer_view_header;

            // bo goc cho  tableView_attachment
            tableView_attachment.ClipsToBounds = true;
            UIBezierPath mPath_tableView_attachment = UIBezierPath.FromRoundedRect(tableView_attachment.Layer.Bounds, (UIRectCorner.BottomLeft | UIRectCorner.BottomRight), new CGSize(width: 6, height: 6));
            //UIBezierPath mPath_tableView_attachment = centerStartBezierPath(tableView_attachment.Layer.Bounds, 6.0f);
            CAShapeLayer maskLayer_tableView_attachment = new CAShapeLayer();
            maskLayer_tableView_attachment.Frame = tableView_attachment.Layer.Bounds;
            maskLayer_tableView_attachment.Path = mPath_tableView_attachment.CGPath;
            tableView_attachment.Layer.Mask = maskLayer_tableView_attachment;

        }
        //temp
        UIBezierPath centerStartBezierPath(CGRect frame, nfloat cornerRadius)
        {
            nfloat M_PI = 3.14f;
            UIBezierPath path = new UIBezierPath();
            path.MoveTo(new CGPoint(frame.Width / 2.0, 0));
            path.AddLineTo(new CGPoint(frame.Width - cornerRadius, 0));
            path.AddArc(new CGPoint(frame.Width - cornerRadius, cornerRadius), cornerRadius, (nfloat)(-M_PI / 2), 0, true);
            path.AddLineTo(new CGPoint(frame.Width, frame.Height - cornerRadius));
            path.AddArc(new CGPoint(frame.Width - cornerRadius, frame.Height - cornerRadius), cornerRadius, 0, (nfloat)(M_PI / 2), true);
            path.AddLineTo(new CGPoint(cornerRadius, frame.Height));
            path.AddArc(new CGPoint(cornerRadius, frame.Height - cornerRadius), cornerRadius, (nfloat)(M_PI / 2), (nfloat)(M_PI), true);
            path.AddLineTo(new CGPoint(0, cornerRadius));
            path.AddArc(new CGPoint(cornerRadius, cornerRadius), cornerRadius, (nfloat)(M_PI), (nfloat)(M_PI * 3 / 2), true);
            path.ClosePath();
            //path.ApplyTransform(new CGAffineTransform());

            return path;
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
                BT_action.UserInteractionEnabled = false;
            }
            else
            {
                BT_action.UserInteractionEnabled = true;
            }
        }

        public override void SetValue()
        {
            base.SetValue();

            var data = element.Value.Trim();
            if (!string.IsNullOrEmpty(data))
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
                BeanAttachFile item_attach_remove = new BeanAttachFile();
                if (parentView != null && parentView.GetType() == typeof(RequestDetailsV2))
                {
                    var index = -1;
                    if (!string.IsNullOrEmpty(attachFile.ID))
                        index = lst_attachment.FindIndex(item => item.ID == attachFile.ID);
                    else
                        index = lst_attachment.FindIndex(item => item.Title == attachFile.Title);

                    if (index != -1)
                    {
                        item_attach_remove = lst_attachment[index];
                        lst_attachment.RemoveAt(index);
                    }
                    var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lst_attachment);

                    element.Value = jsonString;

                    RequestDetailsV2 controller = (RequestDetailsV2)parentView;
                    controller.HandleAttachmentRemove(element, indexPath, this, lst_attachment.Count, item_attach_remove);
                }
            }));
            parentView.PresentViewController(alert, true, null);

        }

        public void HandleEditItem(BeanAttachFile attachFile)
        {
            if (parentView != null && parentView.GetType() == typeof(RequestDetailsV2))
            {
                RequestDetailsV2 controller = (RequestDetailsV2)parentView;
                controller.HandleAttachmentEdit(element, indexPath, attachFile, this);
            }
        }

        public void HandleSeclectItem(BeanAttachFile _attachment)
        {
            if (parentView != null && parentView.GetType() == typeof(RequestDetailsV2))
            {
                currentAttachment = _attachment;
                if (parentView.GetType() == typeof(RequestDetailsV2))
                {
                    RequestDetailsV2 requestDetailsV2 = (RequestDetailsV2)parentView;
                    requestDetailsV2.NavigateToAttachView(_attachment);
                }
            }
        }

        private void HandleBtnAdd(object sender, EventArgs e)
        {
            if (parentView != null && parentView.GetType() == typeof(CreateTicketFormView))
            {
                //string custID = lst_attachment.Count + 1 + "";
                //BeanAttachFile attachmentEmpty = new BeanAttachFile() { ID = custID };
                //lst_attachment.Add(attachmentEmpty);

                //var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lst_attachment);
                //element.Value = jsonString;

                CreateTicketFormView controller = (CreateTicketFormView)parentView;
                controller.HandleAddAttachment(element, indexPath, this);
            }
            else if (parentView.GetType() == typeof(RequestDetailsV2))
            {
                RequestDetailsV2 requestDetailsV2 = parentView as RequestDetailsV2;
                requestDetailsV2.HandleAddAttachment(element, indexPath, this);
            }
        }

        public void UpdateTableSections(KeyValuePair<string, bool> _sectionState)
        {
            var index = lst_sectionState.FindIndex(x => x.Key == _sectionState.Key);
            lst_sectionState[index] = _sectionState;

            //tableView_attachment.Source = new Attachment_TableSource(lst_attachment, this, lst_sectionState);
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
                    baseView.BackgroundColor = UIColor.FromRGB(245, 245, 245);

                    UILabel lbl_title = new UILabel()
                    {
                        Font = UIFont.FromName("Arial-BoldMT", 14f),
                        TextColor = UIColor.FromRGB(0, 95, 212)
                    };
                    string strCount = "";
                    if (dict_attachments[key].Count > 0 && dict_attachments[key].Count < 10)
                        strCount = "0" + dict_attachments[key].Count.ToString();
                    else
                        strCount = dict_attachments[key].Count.ToString();

                    if (string.IsNullOrEmpty(sec))
                        lbl_title.Text = CmmFunction.GetTitle("TEXT_ADDNEW_ATTACHMENT", "Tạo mới") + string.Format(" ({0})", strCount);
                    else if (sec.Contains(";#"))
                        lbl_title.Text = sec.Split(";#")[1] + string.Format(" ({0})", strCount);
                    else
                        lbl_title.Text = sec + string.Format(" ({0})", strCount);

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
                var key = dict_attachments.ElementAt(indexPath.Section).Key;
                var item = dict_attachments[key][indexPath.Row];

                var definitionAction = ContextualDefinitionAction(indexPath.Row, item);
                var flagAction = ContextualFlagAction(indexPath.Row, item);
                var trailingSwipe = UISwipeActionsConfiguration.FromActions(new UIContextualAction[] { flagAction, definitionAction });//

                trailingSwipe.PerformsFirstActionWithFullSwipe = false;
                return trailingSwipe;
            }
            // edit item
            public UIContextualAction ContextualDefinitionAction(int row, BeanAttachFile item)
            {
                string word = lst_attachment[row].Title;

                var action = UIContextualAction.FromContextualActionStyle(UIContextualActionStyle.Normal,
                                                                    CmmFunction.GetTitle("TEXT_EDIT", "Sửa"),
                                                                    (ReadLaterAction, view, success) =>
                                                                    {
                                                                        //if (lst_attachment.Count > 0)
                                                                        //{
                                                                        //    var item = lst_attachment[row];
                                                                        //    parentView.HandleEditItem(item);
                                                                        //}
                                                                        parentView.HandleEditItem(item);
                                                                    });

                action.Image = UIImage.FromFile("Icons/icon_edit_white.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal).Scale(new CGSize(20, 20), 3);
                action.Image.ApplyTintColor(UIColor.White);
                action.BackgroundColor = UIColor.FromRGB(0, 95, 212);
                return action;
            }
            // delete item
            public UIContextualAction ContextualFlagAction(int row, BeanAttachFile item)
            {
                var action = UIContextualAction.FromContextualActionStyle(UIContextualActionStyle.Normal,
                                                                          CmmFunction.GetTitle("TEXT_DELETE", "Xóa"),
                                                                          (FlagAction, view, success) =>
                                                                          {
                                                                              //if (lst_attachment.Count > 0)
                                                                              //{
                                                                              //    var item = lst_attachment[row];
                                                                              //    parentView.HandleRemoveItem(item);

                                                                              //}
                                                                              parentView.HandleRemoveItem(item);
                                                                              success(true);
                                                                          });

                action.Image = UIImage.FromFile("Icons/icon_swipe_delete_white.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal).Scale(new CGSize(20, 20), 3);
                action.Image.ApplyTintColor(UIColor.White);
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
                parentView.HandleSeclectItem(itemSelected);
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                //var attachment = lst_attachment[indexPath.Row];
                var attachment = dict_attachments[sectionKeys[(int)indexPath.Section]][indexPath.Row];


                bool isOdd = true;
                if (indexPath.Row % 2 != 0)
                    isOdd = false;

                Attachment_cell_custom cell = new Attachment_cell_custom(parentView, cellIdentifier, attachment, indexPath, isOdd);
                return cell;
            }
        }
        private class Attachment_cell_custom : UITableViewCell
        {
            ControlAttachmentVerticalWithFormFrame parentView { get; set; }
            NSIndexPath currentIndexPath { get; set; }
            BeanAttachFile attachment { get; set; }
            UILabel lbl_name, lbl_typeName, lbl_size, lbl_chucvu, lbl_line;
            UIImageView img_type, img_noti_new;
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
                    ContentView.BackgroundColor = UIColor.FromRGB(243, 249, 255);
                else
                    ContentView.BackgroundColor = UIColor.White;

                img_type = new UIImageView();
                img_type.Image = UIImage.FromFile("Images/logo_VT_300_134.png");
                img_type.ContentMode = UIViewContentMode.ScaleAspectFill;

                img_noti_new = new UIImageView();
                img_noti_new.Image = UIImage.FromFile("Icons/icon_notiAttachNew.png");
                img_noti_new.ContentMode = UIViewContentMode.ScaleToFill;
                img_noti_new.Hidden = true;

                lbl_name = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 14f),
                    TextColor = UIColor.FromRGB(59, 95, 179),
                    TextAlignment = UITextAlignment.Left,
                };

                lbl_typeName = new UILabel()
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

                ContentView.AddSubviews(new UIView[] { img_noti_new, img_type, lbl_name, lbl_typeName, lbl_size, lbl_chucvu, lbl_line });
                lbl_line.Hidden = true;
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

                    //if (attachment.IsAuthor == true)
                    //    img_noti_new.Hidden = false;

                    //title
                    if (attachment.Title.Contains(";#"))
                        lbl_name.Text = attachment.Title.Split(";#")[0];
                    else
                        lbl_name.Text = attachment.Title;

                    //CreatedBy
                    if (!string.IsNullOrEmpty(attachment.CreatedName) && attachment.CreatedName.Contains(";#"))
                        lbl_typeName.Text = attachment.CreatedBy.Split(";#")[1];
                    else
                        lbl_typeName.Text = attachment.CreatedName;

                    if (attachment.ID == "")
                        img_noti_new.Hidden = false;

                    lbl_size.Text = CmmFunction.FileSizeFormatter.FormatSize(attachment.Size);
                    lbl_chucvu.Text = attachment.CreatedPositon;
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

                img_type.Frame = new CGRect(20, 15, 18, 18);
                //lbl_name.Frame = new CGRect(20, 0, (width / 3) * 1.8f, 50);
                //lbl_typeName.Frame = new CGRect(lbl_name.Frame.Right, 0, width - lbl_name.Frame.Right, 50);
                lbl_name.Frame = new CGRect(50, 13, ((width - (50 + 20)) / 3) * 1.8f, 20);
                img_noti_new.Frame = new CGRect(lbl_name.Frame.X - 7, 10, 10, 10);
                lbl_typeName.Frame = new CGRect(lbl_name.Frame.Right + 20, 13, (width - lbl_name.Frame.Right) - 15, 20);
                lbl_size.Frame = new CGRect(lbl_name.Frame.X, lbl_name.Frame.Bottom, lbl_name.Frame.Width, 20);
                lbl_chucvu.Frame = new CGRect(lbl_typeName.Frame.X, lbl_typeName.Frame.Bottom, lbl_typeName.Frame.Width, 20);
                lbl_line.TranslatesAutoresizingMaskIntoConstraints = false;
                lbl_line.HeightAnchor.ConstraintEqualTo(1).Active = true;
                NSLayoutConstraint.Create(this.lbl_line, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
                NSLayoutConstraint.Create(this.lbl_line, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;
                NSLayoutConstraint.Create(this.lbl_line, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, this, NSLayoutAttribute.Bottom, 1.0f, -1).Active = true;
            }
        }
        #endregion
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobileV1.iOS.IOSClass;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UIKit;
using Xamarin.iOS;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class AttachmentListView : UIViewController
    {
        UIViewController parentView { get; set; }
        List<BeanAttachFile> lst_beanAttachFiles;
        string title;
        List<KeyValuePair<string, bool>> lst_sectionState;
        public BeanAttachFile currentAttachment { get; set; }
        ViewElement element;

        public AttachmentListView(IntPtr handle) : base(handle)
        {
        }

        #region override
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            LoadDataContent();
            SetLangTitle();

            #region delegate
            BT_close.TouchUpInside += BT_close_TouchUpInside;
            #endregion
        }

        #endregion

        #region private - public method
        public void SetContent(List<BeanAttachFile> _lst_Attachment, string _title, UIViewController _parent, ViewElement _element)
        {
            title = _title;
            lst_beanAttachFiles = _lst_Attachment;
            parentView = _parent;
            element = _element;
        }

        private void SetLangTitle()
        {
            CmmIOSFunction.SetLangToView(this.View);
        }

        private void LoadDataContent()
        {
            //var model = DeviceHardware.Model;
            //if (model.Contains("X") || model.Contains("11"))
            //{
            //    headerView_constantHeight.Constant = 80;
            //}

            headerView_constantHeight.Constant = 45 + 10 + CmmIOSFunction.GetHeaderViewHeight();

            //lbl_title.LineBreakMode = UILineBreakMode.TailTruncation;
            //lbl_title.Lines = 2;
            //lbl_title.Text = title;

            table_content.Source = new Attachment_TableSource(lst_beanAttachFiles, this, null);
            table_content.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            table_content.ReloadData();
        }

        public void HandleRemoveItem(BeanAttachFile attachFile)
        {
            UIAlertController alert = UIAlertController.Create("Thông báo", CmmFunction.GetTitle("TEXT_DELETE_CONFIRM", "Bạn có chắc muốn xóa file này không?"), UIAlertControllerStyle.Alert);//BPM
            alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_CANCEL", "Hủy"), UIAlertActionStyle.Default, null));
            alert.AddAction(UIAlertAction.Create(CmmFunction.GetTitle("TEXT_AGREE", "Đồng ý"), UIAlertActionStyle.Default, alertAction =>
            {
                BeanAttachFile attach_remove = new BeanAttachFile();
                if (parentView != null && parentView.GetType() == typeof(RequestDetailsV2))
                {
                    var index = lst_beanAttachFiles.FindIndex(item => item.ID == attachFile.ID);
                    if (index != -1)
                    {
                        attach_remove = lst_beanAttachFiles[index];
                        //lst_attach_remove.Add(lst_beanAttachFiles[index]);
                        lst_beanAttachFiles.RemoveAt(index);
                    }

                    //var json_attachRemove = JsonConvert.SerializeObject(lst_attach_remove);
                    var jsonString = JsonConvert.SerializeObject(lst_beanAttachFiles);
                    element.Value = jsonString;

                    RequestDetailsV2 controller = (RequestDetailsV2)parentView;
                    controller.HandleAttachmentRemove(element, null, null, lst_beanAttachFiles.Count, attach_remove);
                }
            }));
            parentView.PresentViewController(alert, true, null);

        }

        public void HandleEditItem(BeanAttachFile attachFile)
        {
            if (parentView != null && parentView.GetType() == typeof(RequestDetailsV2))
            {
                //RequestDetailsV2 controller = (RequestDetailsV2)parentView;
                //controller.HandleAttachmentEdit(element, indexPath, attachFile, this);
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

        public void UpdateTableSections(KeyValuePair<string, bool> _sectionState)
        {
            var index = lst_sectionState.FindIndex(x => x.Key == _sectionState.Key);
            lst_sectionState[index] = _sectionState;

            //table_content.Source = new Attachment_TableSource(lst_beanAttachFiles, this, lst_sectionState);
            table_content.ReloadSections(NSIndexSet.FromIndex(index), UITableViewRowAnimation.None);
        }

        private void ShowAttachmentSelected(BeanAttachFile _attachFile)
        {
            PresentationDelegate transitioningDelegate;
            ShowAttachmentView attachmentView = (ShowAttachmentView)Storyboard.InstantiateViewController("ShowAttachmentView");
            attachmentView.setContent(this, _attachFile);
            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            attachmentView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            attachmentView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            attachmentView.TransitioningDelegate = transitioningDelegate;
            this.PresentViewControllerAsync(attachmentView, true);
        }
        #endregion

        #region events
        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            if (this.NavigationController != null)
                this.NavigationController.PopViewController(true);
            else
                this.DismissViewControllerAsync(true);
        }
        #endregion

        #region custom class
        #region dinh kem

        private class Attachment_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellAttachmentID");
            AttachmentListView parentView;
            List<BeanAttachFile> lst_attachment = new List<BeanAttachFile>();
            Dictionary<string, List<BeanAttachFile>> dict_attachments = new Dictionary<string, List<BeanAttachFile>>();
            List<string> sectionKeys;
            //bool => Colapse
            List<KeyValuePair<string, bool>> lst_sectionState;
            KeyValuePair<string, bool> sectionState;

            public Attachment_TableSource(List<BeanAttachFile> _lst_attachment, AttachmentListView _parentview, List<KeyValuePair<string, bool>> _sectionState)
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

                    //if (lst_sectionState[(int)section].Value)
                    //    img_arrow.Image = UIImage.FromFile("Icons/icon_arrowUpDown_colapse.png");
                    //else
                    //    img_arrow.Image = UIImage.FromFile("Icons/icon_arrowUpDown.png");

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

            #region tam khoa khong dung
            //public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
            //{
            //    //if (lst_workRelated.Count > 0)
            //    //{
            //    //    var item = lst_workRelated[indexPath.Row];
            //    //    parentView.HandleRemoveItem(item);
            //    //}
            //}

            //public override UISwipeActionsConfiguration GetTrailingSwipeActionsConfiguration(UITableView tableView, NSIndexPath indexPath)
            //{
            //    var definitionAction = ContextualDefinitionAction(indexPath.Row);
            //    var flagAction = ContextualFlagAction(indexPath.Row);
            //    var trailingSwipe = UISwipeActionsConfiguration.FromActions(new UIContextualAction[] { flagAction, definitionAction });

            //    trailingSwipe.PerformsFirstActionWithFullSwipe = false;
            //    return trailingSwipe;
            //}
            //// edit item
            //public UIContextualAction ContextualDefinitionAction(int row)
            //{
            //    string word = lst_attachment[row].Title;

            //    var action = UIContextualAction.FromContextualActionStyle(UIContextualActionStyle.Normal,
            //                                                        CmmFunction.GetTitle("TEXT_EDIT", "Sửa"),
            //                                                        (ReadLaterAction, view, success) => {
            //                                                            if (lst_attachment.Count > 0)
            //                                                            {
            //                                                                var item = lst_attachment[row];
            //                                                                parentView.HandleEditItem(item);
            //                                                            }
            //                                                        });

            //    action.Image = UIImage.FromFile("Icons/icon_edit.png");
            //    action.BackgroundColor = UIColor.FromRGB(51, 95, 179);
            //    return action;
            //}
            //// delete item
            //public UIContextualAction ContextualFlagAction(int row)
            //{
            //    var action = UIContextualAction.FromContextualActionStyle(UIContextualActionStyle.Normal,
            //                                                              CmmFunction.GetTitle("TEXT_DELETE", "Xóa"),
            //                                                              (FlagAction, view, success) => {
            //                                                                  if (lst_attachment.Count > 0)
            //                                                                  {
            //                                                                      var item = lst_attachment[row];
            //                                                                      parentView.HandleRemoveItem(item);

            //                                                                  }
            //                                                                  success(true);
            //                                                              });

            //    action.Image = UIImage.FromFile("Icons/icon_swipe_delete.png");
            //    action.BackgroundColor = UIColor.FromRGB(235, 52, 46);
            //    return action;
            //}

            //public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
            //{
            //    var attachment = lst_attachment[indexPath.Row];
            //    if (attachment.bIsAuthor == true)
            //        return true;
            //    else
            //        return false;
            //}
            #endregion

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                var itemSelected = lst_attachment[indexPath.Row];
                parentView.HandleSeclectItem(itemSelected);
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var attachment = lst_attachment[indexPath.Row];

                bool isOdd = true;
                if (indexPath.Row % 2 != 0)
                    isOdd = false;

                Attachment_cell_custom cell = new Attachment_cell_custom(parentView, cellIdentifier, attachment, indexPath, isOdd);
                return cell;
            }
        }

        private class Attachment_cell_custom : UITableViewCell
        {
            AttachmentListView parentView { get; set; }
            NSIndexPath currentIndexPath { get; set; }
            BeanAttachFile attachment { get; set; }
            UILabel lbl_name, lbl_typeName, lbl_size, lbl_chucvu, lbl_line;
            UIImageView img_type;
            bool isOdd;

            public Attachment_cell_custom(AttachmentListView _parentView, NSString cellID, BeanAttachFile _attachment, NSIndexPath _currentIndexPath, bool _isOdd) : base(UITableViewCellStyle.Default, cellID)
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

                lbl_name = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 14f),
                    TextColor = UIColor.FromRGB(0, 95, 212),
                    TextAlignment = UITextAlignment.Left,
                };

                lbl_typeName = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 14f),
                    TextColor = UIColor.FromRGB(25, 25, 30),
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

                ContentView.AddSubviews(new UIView[] { img_type, lbl_name, lbl_typeName, lbl_size, lbl_chucvu, lbl_line });
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

                    //title
                    lbl_name.Text = attachment.Title;

                    //CreatedBy
                    if (!string.IsNullOrEmpty(attachment.CreatedName) && attachment.CreatedName.Contains(";#"))
                        lbl_typeName.Text = attachment.CreatedBy.Split(";#")[1];
                    else
                        lbl_typeName.Text = attachment.CreatedName;


                    lbl_size.Text = CmmFunction.GetNumberMBProcessed(Convert.ToInt32(attachment.Size));

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

        //private class Attachment_cell_custom : UITableViewCell
        //{
        //    BeanAttachFile attachment { get; set; }
        //    UIImageView img_type;
        //    UILabel lbl_title;

        //    public Attachment_cell_custom(NSString cellID) : base(UITableViewCellStyle.Default, cellID)
        //    {
        //        Accessory = UITableViewCellAccessory.None;
        //        viewConfiguration();
        //    }

        //    private void viewConfiguration()
        //    {
        //        ContentView.BackgroundColor = UIColor.White;

        //        img_type = new UIImageView();
        //        img_type.ContentMode = UIViewContentMode.ScaleAspectFit;

        //        lbl_title = new UILabel()
        //        {
        //            Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
        //            TextColor = UIColor.DarkGray,
        //            TextAlignment = UITextAlignment.Left,
        //            LineBreakMode = UILineBreakMode.TailTruncation
        //        };

        //        ContentView.AddSubviews(new UIView[] { img_type, lbl_title });
        //    }

        //    public void UpdateCell(BeanAttachFile _attch)
        //    {
        //        try
        //        {
        //            attachment = _attch;
        //            string fileExt = string.Empty;
        //            if (!string.IsNullOrEmpty(attachment.Path))
        //                fileExt = attachment.Path.Split('.')[1];

        //            switch (fileExt)
        //            {
        //                case "doc":
        //                case "docx":
        //                    img_type.Image = UIImage.FromFile("Icons/icon_docx.png");
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
        //            lbl_title.Text = attachment.Title;

        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("attachment_cell_custom - UpdateCell - ERR: " + ex.ToString());
        //        }
        //    }

        //    public override void LayoutSubviews()
        //    {
        //        base.LayoutSubviews();
        //        img_type.Frame = new CGRect(20, 10, 20, 20);
        //        lbl_title.Frame = new CGRect(img_type.Frame.Right + 15, 5, this.ContentView.Frame.Width - 70, 30);
        //    }
        //}

        partial class Tree_AttachmentCell : UITableViewCell
        {
            //private UIImageView _imageView;
            //private UILabel _titleLabel;
            //public string TitleValue = "";

            //public AttachmentCell(IntPtr handle) : base(handle)
            //{
            //}

            //public int Level;

            //public AttachmentCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
            //{
            //}

            //public AttachmentCell()
            //{
            //}

            //public void SetCellContents(TreeNode node)
            //{
            //    Level = node.NodeLevel;

            //    _imageView = new UIImageView
            //    {
            //        Image = (node.NodeType == "folder") ? UIImage.FromFile("folder.png") : UIImage.FromFile("file.png"),
            //        ContentMode = UIViewContentMode.Left
            //    };
            //    _imageView.SizeToFit();

            //    _titleLabel = new UILabel()
            //    {
            //        TextColor = UIColor.Black,
            //        BackgroundColor = UIColor.Clear,
            //        Text = node.NodeName
            //    };
            //    _titleLabel.SizeToFit();

            //    //important: remove all previous subviews before adding the new ones!

            //    foreach (var subview in ContentView.Subviews)
            //    {
            //        subview.RemoveFromSuperview();
            //    }

            //    ContentView.AddSubviews(_imageView, _titleLabel);
            //}

            //public override void LayoutSubviews()
            //{
            //    base.LayoutSubviews();

            //    var contentRect = ContentView.Bounds;
            //    var boundsX = contentRect.X;
            //    int LevelIndent = 40, CellHeight = 40;

            //    var imageFrame = new RectangleF(((float)boundsX + Level - 1) * LevelIndent, 0, (LevelIndent), CellHeight);
            //    _imageView.Frame = imageFrame;

            //    var titleFrame = new RectangleF(((float)boundsX + Level - 1) * LevelIndent + (LevelIndent) + 2, 0, (float)_titleLabel.Bounds.Width, CellHeight);
            //    _titleLabel.Frame = titleFrame;
            //}
        }


        #endregion
        #endregion


    }
}


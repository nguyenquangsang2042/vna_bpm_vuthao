using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json.Linq;
using UIKit;

namespace BPMOPMobile.iPad.Components
{
    class ControlAttachmentVerticalWithFormFrame_BK: ControlBase
    {
        UIViewController parentView { get; set; }
        NSIndexPath indexPath { get; set; }
        ViewElement element { get; set; }

        UITableView tableView_attachment;
        List<BeanAttachFile> lst_attachment = new List<BeanAttachFile>();
        public BeanAttachFile currentAttachment { get; set; }
        List<KeyValuePair<string, bool>> lst_sectionState;

        UIView view_header;
        UILabel lbl_name, lbl_typeName, lbl_Creator;
        UIButton btn_add;

        public ControlAttachmentVerticalWithFormFrame_BK(UIViewController _parentView, ViewElement _element, NSIndexPath _indexPath)
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
            view_header.BackgroundColor = UIColor.FromRGB(245, 245, 245);

            lbl_name = new UILabel()
            {
                Font = UIFont.FromName("Arial-BoldMT", 14f),
                Text = "Tên tài liệu",
                TextAlignment = UITextAlignment.Left
            };

            lbl_typeName = new UILabel()
            {
                Font = UIFont.FromName("Arial-BoldMT", 14f),
                Text = "Loại tài liệu",
                TextAlignment = UITextAlignment.Left
            };

            lbl_Creator = new UILabel()
            {
                Font = UIFont.FromName("Arial-BoldMT", 14f),
                Text = "Người tạo",
                TextAlignment = UITextAlignment.Left
            };

            view_header.AddSubviews(new UIView[] { lbl_Creator, lbl_name, lbl_typeName, lbl_Creator });

            tableView_attachment = new UITableView();
            tableView_attachment.ScrollEnabled = false;

            btn_add = new UIButton();
            btn_add.SetTitle("Tạo mới", UIControlState.Normal);
            btn_add.SetImage(UIImage.FromFile("Icons/icon_document_add.png"), UIControlState.Normal);
            btn_add.SetTitleColor(UIColor.FromRGB(65, 80, 134), UIControlState.Normal);
            btn_add.Font = UIFont.SystemFontOfSize(14);

            this.Add(view_header);
            this.Add(tableView_attachment);
            this.Add(btn_add);

            this.WillRemoveSubview(lbl_value);
            this.WillRemoveSubview(BT_action);
            this.WillRemoveSubview(lbl_line);

            if (btn_add != null)
                btn_add.AddTarget(HandleBtnAdd, UIControlEvent.TouchUpInside);
        }

        public void HandleSeclectItem(BeanAttachFile _attachment)
        {
            if (parentView != null && parentView.GetType() == typeof(ToDoDetailView))
            {
                currentAttachment = _attachment;
                ToDoDetailView toDoDetailView = parentView as ToDoDetailView;
                //toDoDetailView.NavigateToAttachView(_attachment);
                
            }
            else if (parentView != null && parentView.GetType() == typeof(WorkflowDetailView))
            {
                currentAttachment = _attachment;
                WorkflowDetailView controller = (WorkflowDetailView)parentView;
                //controller.NavigatorToView(element, indexPath, this);
            }
        }

        private void HandleBtnAdd(object sender, EventArgs e)
        {
            if (parentView != null && parentView.GetType() == typeof(ToDoDetailView))
            {
                //string custID = lst_attachment.Count + 1 + "";
                //BeanAttachFile attachmentEmpty = new BeanAttachFile() { ID = custID };
                //lst_attachment.Add(attachmentEmpty);

                //var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lst_attachment);
                //element.Value = jsonString;

                ToDoDetailView controller = (ToDoDetailView)parentView;
                //controller.HandleAddAttachment(element, indexPath, this);
            }
            //else if (parentView.GetType() == typeof(RequestDetailsV2))
            //{
            //    RequestDetailsV2 requestDetailsV2 = parentView as RequestDetailsV2;
            //    requestDetailsV2.HandleAddAttachment(element, indexPath, this);
            //}
        }

        public void UpdateTableSections(KeyValuePair<string, bool> _sectionState)
        {
            var index = lst_sectionState.FindIndex(x => x.Key == _sectionState.Key);
            lst_sectionState[index] = _sectionState;

            //tableView_attachment.Source = new Attachment_TableSource(lst_attachment, this, lst_sectionState);
            //tableView_attachment.ReloadSections(NSIndexSet.FromIndex(index), UITableViewRowAnimation.None);
        }

        public override void InitializeFrameView(CGRect frame)
        {
            this.Frame = frame;

            const int paddingTop = 10;
            const int spaceView = 5;
            var width = Frame.Width;

            lbl_title.HeightAnchor.ConstraintEqualTo(20).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1.0f, paddingTop).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;

            view_header.Frame = new CGRect(0, 20 + paddingTop + spaceView, width, 50);
            lbl_name.Frame = new CGRect(20, 0, width * 0.4, 50);
            lbl_typeName.Frame = new CGRect(lbl_name.Frame.Right + 5, 0, width * 0.2, 50);
            lbl_typeName.BackgroundColor = UIColor.Red;
            lbl_Creator.Frame = new CGRect(lbl_typeName.Frame.Right, 0, width * 0.25, 50);
            lbl_Creator.BackgroundColor = UIColor.Purple;
            
            var topAnchor = (20 + paddingTop + spaceView + 50);
            tableView_attachment.Frame = new CGRect(0, topAnchor, frame.Width, frame.Height - topAnchor);

            //set image left button add
            btn_add.ImageView.Frame = new CGRect(0, 0, 20, 20);
            btn_add.ImageEdgeInsets = new UIEdgeInsets(top: 0, left: 0, bottom: 0, right: 5);
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

            //tableView_attachment.Source = new Attachment_TableSource(lst_attachment, this, null);
            tableView_attachment.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            tableView_attachment.ReloadData();
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
                    sectionKeys = lst_attachment.Select(x => x.Category).Distinct().ToList();

                    foreach (var section in sectionKeys)
                    {
                        List<BeanAttachFile> lst_item = lst_attachment.Where(x => x.Category == section).ToList();
                        dict_attachments.Add(section, lst_item);

                        KeyValuePair<string, bool> keypair_section;
                        keypair_section = new KeyValuePair<string, bool>(section, false);
                        lst_sectionState.Add(keypair_section);
                    }

                    //parentView.lst_sectionState = lst_sectionState;

                }
                else
                {
                    sectionKeys = lst_attachment.Select(x => x.Category).Distinct().ToList();

                    foreach (var section in sectionKeys)
                    {
                        List<BeanAttachFile> lst_item = lst_attachment.Where(x => x.Category == section).ToList();
                        dict_attachments.Add(section, lst_item);
                    }
                }
            }

            public override nfloat GetHeightForFooter(UITableView tableView, nint section)
            {
                return -1;
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
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
                    baseView.BackgroundColor = UIColor.White;

                    UILabel lbl_title = new UILabel()
                    {
                        Font = UIFont.FromName("Arial-BoldMT", 14f),
                        TextColor = UIColor.FromRGB(65, 80, 134)
                    };
                    if (string.IsNullOrEmpty(sec))
                        lbl_title.Text = CmmFunction.GetTitle("TEXT_ADDNEW_ATTACHMENT", "Tạo mới");
                    else if (sec.Contains(";#"))
                        lbl_title.Text = sec.Split(";#")[1];
                    else
                        lbl_title.Text = sec;

                    var titleRect = StringExtensions.MeasureString(lbl_title.Text, 16);
                    lbl_title.Frame = new CGRect(20, 7, titleRect.Width + 5, 30);

                    UIImageView img_arrow = new UIImageView();
                    img_arrow.Frame = new CGRect(lbl_title.Frame.Right + 5, 18, 7, 7);
                    img_arrow.ContentMode = UIViewContentMode.ScaleAspectFill;
                    if (lst_sectionState[(int)section].Value)
                        img_arrow.Image = UIImage.FromFile("Icons/icon_arrowUpDown_colapse.png");
                    else
                        img_arrow.Image = UIImage.FromFile("Icons/icon_arrowUpDown.png");

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
                        //parentView.UpdateTableSections(new KeyValuePair<string, bool>(lst_sectionState[(int)section].Key, !lst_sectionState[(int)section].Value));
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

            public override nint NumberOfSections(UITableView tableView)
            {
                return dict_attachments.Count;
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
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                var itemSelected = lst_attachment[indexPath.Row];
                //parentView.HandleSeclectItem(itemSelected);
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var attachment = lst_attachment[indexPath.Row];
                Attachment_cell_custom cell = new Attachment_cell_custom(parentView, cellIdentifier, attachment, indexPath);
                return cell;
            }
        }
        private class Attachment_cell_custom : UITableViewCell
        {
            ControlAttachmentVerticalWithFormFrame parentView { get; set; }
            NSIndexPath currentIndexPath { get; set; }

            BeanAttachFile attachment { get; set; }
            UILabel lbl_name, lbl_typeName, lbl_creator;
            UIImageView img_type;
            UIButton btn_download;

            public Attachment_cell_custom(ControlAttachmentVerticalWithFormFrame _parentView, NSString cellID, BeanAttachFile _attachment, NSIndexPath _currentIndexPath) : base(UITableViewCellStyle.Default, cellID)
            {
                parentView = _parentView;
                attachment = _attachment;
                currentIndexPath = _currentIndexPath;
                Accessory = UITableViewCellAccessory.None;
                viewConfiguration();
            }

            private void viewConfiguration()
            {
                ContentView.BackgroundColor = UIColor.White;

                img_type = new UIImageView();
                img_type.Image = UIImage.FromFile("Icons/icon_word.png");
                img_type.ContentMode = UIViewContentMode.ScaleAspectFit;

                lbl_name = new UILabel()
                {
                    Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular),
                    TextColor = UIColor.FromRGB(51, 95, 179),
                    TextAlignment = UITextAlignment.Left,
                };

                lbl_typeName = new UILabel()
                {
                    Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular),
                    TextColor = UIColor.Black,
                    TextAlignment = UITextAlignment.Left,
                };

                lbl_creator = new UILabel()
                {
                    Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular),
                    TextColor = UIColor.Black,
                    TextAlignment = UITextAlignment.Left,
                };

                btn_download = new UIButton();
                btn_download.SetImage(UIImage.FromFile("Icons/icon_download.png"), UIControlState.Normal);
                btn_download.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);

                ContentView.AddSubviews(new UIView[] { img_type, lbl_name, lbl_typeName, btn_download, lbl_creator });
                loadData();
            }

            public void loadData()
            {
                try
                {
                    //lbl_name.Text = attachment.Value;
                    //lbl_typeName.Text = "Hợp đồng / Phụ lục hợp đồng";
                    string fileExt = string.Empty;
                    if (!string.IsNullOrEmpty(attachment.Path))
                        fileExt = attachment.Path.Split('.')[1];

                    switch (fileExt)
                    {
                        case "doc":
                        case "docx":
                            img_type.Image = UIImage.FromFile("Icons/icon_docx.png");
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

                    if (!string.IsNullOrEmpty(attachment.Category) && attachment.Category.Contains(";#"))
                        lbl_typeName.Text = attachment.Category.Split(";#")[1];
                    else
                        lbl_typeName.Text = attachment.Category;

                    //CreatedBy
                    if (!string.IsNullOrEmpty(attachment.CreatedBy) && attachment.CreatedBy.Contains(";#"))
                        lbl_creator.Text = attachment.CreatedBy.Split(";#")[1];
                    else
                        lbl_creator.Text = attachment.CreatedBy;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ControlAttachmentVerticalWithFormFrame - Control_cell_custom - loaddata- ERR: " + ex.ToString());
                }
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();

                var width = ContentView.Frame.Width;

                img_type.Frame = new CGRect(25, 20, 20, 20);
                //lbl_name.Frame = new CGRect(70, 15, (width / 2) - 70, 30);
                //lbl_typeName.Frame = new CGRect(width / 2, 15, (width / 2) - 70, 30);
                //lbl_creator.Frame = new CGRect(300, 15, 150, 30);
                //btn_download.Frame = new CGRect(width - 60, 15, 30, 30);

                lbl_name.Frame = new CGRect(70, 0, (width * 0.4) - 50, 50);
                lbl_typeName.Frame = new CGRect(lbl_name.Frame.Right + 5, 0, width * 0.2, 50);
                lbl_creator.Frame = new CGRect(lbl_typeName.Frame.Right, 0, width * 0.25, 50);
                btn_download.Frame = new CGRect(width - 60, 15, 30, 30);

            }
        }
        #endregion
        #endregion
    }
}
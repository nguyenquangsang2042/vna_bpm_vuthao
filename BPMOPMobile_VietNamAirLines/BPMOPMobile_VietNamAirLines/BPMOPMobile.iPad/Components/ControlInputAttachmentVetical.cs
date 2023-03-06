using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPMOPMobile.Bean;
using BPMOPMobile.iPad.CustomControlClass;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json.Linq;
using UIKit;

namespace BPMOPMobile.iPad.Components
{
    class ControlInputAttachmentVetical: ControlBase
    {
        UIViewController parentView { get; set; }
        NSIndexPath indexPath { get; set; }
        ViewElement element { get; set; }

        UITableView tableView_attachment;
        List<BeanAttachFile> lst_attachment = new List<BeanAttachFile>();
        public BeanAttachFile currentAttachment { get; set; }

        UIView view_header;
        UILabel lbl_type, lbl_name;
        UIButton btn_add;

        public ControlInputAttachmentVetical(UIViewController _parentView, ViewElement _element, NSIndexPath _indexPath)
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

            lbl_type = new UILabel()
            {
                Font = UIFont.BoldSystemFontOfSize(14),
                Text = "Loại tài liệu",
                TextAlignment = UITextAlignment.Left
            };

            lbl_name = new UILabel()
            {
                Font = UIFont.BoldSystemFontOfSize(14),
                Text = "Tên file",
                TextAlignment = UITextAlignment.Left
            };

            view_header.AddSubviews(new UIView[] { lbl_type, lbl_name });

            tableView_attachment = new UITableView();
            tableView_attachment.ContentInset = new UIEdgeInsets(0, 0, 0, 0);

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

        private void HandleBtnAdd(object sender, EventArgs e)
        {
            if (parentView != null && parentView.GetType() == typeof(CreateNewTaskView))
            {
                CreateNewTaskView controller = (CreateNewTaskView)parentView;
                controller.HandleAddAttachment(element, indexPath, this);
            }
        }

        public void HandleSeclectItem(BeanAttachFile _attachment)
        {
            if (parentView != null && parentView.GetType() == typeof(CreateNewTaskView))
            {
                currentAttachment = _attachment;

                CreateNewTaskView controller = (CreateNewTaskView)parentView;
                controller.NavigatorToView(element, indexPath, this);
            }
        }

        public void HandleRemoveItem(BeanAttachFile _attachment)
        {
            if (parentView != null && parentView.GetType() == typeof(CreateNewTaskView))
            {
                var index = lst_attachment.FindIndex(item => item.ID == _attachment.ID);
                if (index != -1)
                    lst_attachment.RemoveAt(index);

                var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lst_attachment);
                element.Value = jsonString;

                CreateNewTaskView controller = (CreateNewTaskView)parentView;
                controller.HandleRemoveAttachment(element, indexPath, this, lst_attachment.Count);
            }
        }

        public override void InitializeFrameView(CGRect frame)
        {
            this.Frame = frame;

            const int paddingTop = 10;
            const int spaceView = 5;
            var width = Frame.Width;
            const int paddingLeft = 25;
            var widthlblHeader = width - 30;

            btn_add.Frame = new CGRect(Frame.Width - 100, paddingTop, 100, 20);

            lbl_title.HeightAnchor.ConstraintEqualTo(20).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1.0f, paddingTop).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, btn_add.Frame.Width).Active = true;

            view_header.Frame = new CGRect(0, 20 + paddingTop + spaceView, width, 30);
            lbl_name.Frame = new CGRect(paddingLeft, 0, (widthlblHeader * 0.5) - paddingLeft, view_header.Frame.Height);
            lbl_type.Frame = new CGRect(lbl_name.Frame.Right + paddingLeft, 0, (widthlblHeader * 0.5) - paddingLeft, view_header.Frame.Height);

            tableView_attachment.Frame = new CGRect(0, view_header.Frame.Bottom + 10, frame.Width, frame.Height - (view_header.Frame.Bottom + 10));

            //set image left button add
            btn_add.ImageView.Frame = new CGRect(0, 0, 20, 20);
            btn_add.ImageEdgeInsets = new UIEdgeInsets(top: 0, left: 0, bottom: 0, right: btn_add.ImageView.Frame.Width / 2);
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

        public override void SetValue()
        {
            base.SetValue();

            var data = element.Value.Trim();

            if (!string.IsNullOrEmpty(data))
            {
                JArray json = JArray.Parse(data);
                lst_attachment = json.ToObject<List<BeanAttachFile>>();
            }
            
            tableView_attachment.Source = new Attachment_TableSource(lst_attachment, this);
            tableView_attachment.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            tableView_attachment.ReloadData();
        }

        public override string Value { get => element.Value; set => base.Value = value; }

        #region custom views
        #region attachment source table
        private class Attachment_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellAttachmentID");
            ControlInputAttachmentVetical parentView;
            List<BeanAttachFile> lst_attachment { get; set; }
            public Attachment_TableSource(List<BeanAttachFile> _lst_attachment, ControlInputAttachmentVetical _parentview)
            {
                lst_attachment = _lst_attachment;
                parentView = _parentview;
            }

            public override nfloat GetHeightForFooter(UITableView tableView, nint section)
            {
                return -1;
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                return -1;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 60;
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return 1;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return lst_attachment.Count;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                var itemSelected = lst_attachment[indexPath.Row];
                parentView.HandleSeclectItem(itemSelected);
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
            ControlInputAttachmentVetical parentView { get; set; }
            NSIndexPath currentIndexPath { get; set; }

            BeanAttachFile attachment { get; set; }
            UILabel lbl_line;
            UILabel lbl_typeName, lbl_fileName, lbl_size;
            UIButton btn_remove;
            UIImageView iv_type;

            public Attachment_cell_custom(ControlInputAttachmentVetical _parentView, NSString cellID, BeanAttachFile _attachment, NSIndexPath _currentIndexPath) : base(UITableViewCellStyle.Default, cellID)
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

                iv_type = new UIImageView();
                iv_type.ContentMode = UIViewContentMode.ScaleAspectFit;
                iv_type.Image = UIImage.FromFile("Icons/icon_attachFile_pdf.png");

                lbl_typeName = new UILabel() { 
                    Font = UIFont.SystemFontOfSize(14)
                };

                lbl_fileName = new UILabel()
                {
                    Font = UIFont.SystemFontOfSize(14)
                };

                lbl_size = new UILabel()
                {
                    Font = UIFont.SystemFontOfSize(12),
                    TextColor = UIColor.FromRGB(153, 153, 153)
                };

                btn_remove = new UIButton();
                btn_remove.SetImage(UIImage.FromFile("Icons/icon_close_circle_red.png"), UIControlState.Normal);
                btn_remove.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);
                btn_remove.TouchUpInside += Btn_remove_TouchUpInside;

                lbl_line = new UILabel()
                {
                    BackgroundColor = UIColor.FromRGB(144, 164, 174),
                    Hidden = true
                };

                ContentView.AddSubviews(new UIView[] { iv_type, lbl_typeName, lbl_fileName, lbl_size, btn_remove, lbl_line });
                loadData();
            }

            private void Btn_remove_TouchUpInside(object sender, EventArgs e)
            {
                parentView.HandleRemoveItem(attachment);
            }

            public void loadData()
            {
                try
                {
                    //test data
                    lbl_typeName.Text = attachment.Type;
                    lbl_fileName.Text = attachment.Title;
                    lbl_size.Text = FileSizeFormatter.FormatSize(attachment.Size);

                    var index = attachment.Title.LastIndexOf('.');
                    var type = attachment.Title.Substring((index + 1), attachment.Title.Length - (index + 1));

                    switch (type.ToLower())
                    {
                        case "doc":
                        case "docx":
                            iv_type.Image = UIImage.FromFile("Icons/icon_word.png");
                            break;
                        case "xls":
                        case "xlsx":
                            iv_type.Image = UIImage.FromFile("Icons/icon_attachFile_excel.png");
                            break;
                        case "pdf":
                            iv_type.Image = UIImage.FromFile("Icons/icon_attachFile_pdf.png");
                            break;
                        case "png":
                        case "jpeg":
                        case "jpg":
                            iv_type.Image = UIImage.FromFile("Icons/icon_attachFile_photo.png");
                            break;
                        default:
                            iv_type.Image = UIImage.FromFile("Icons/icon_attachFile_other.png");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ControlInputAttachmentVetical - Control_cell_custom - loaddata- ERR: " + ex.ToString());
                }
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();

                const int paddingTop = 10;
                const int paddingLeft = 25;
                var width = ContentView.Frame.Width - 55;

                btn_remove.Frame = new CGRect(ContentView.Frame.Width - 55, 15, 30, 30);

                iv_type.Frame = new CGRect(paddingLeft, (ContentView.Frame.Height/2) - 15, 30, 30);
                lbl_fileName.Frame = new CGRect(iv_type.Frame.Right + 10, paddingTop, (width * 0.5) - (iv_type.Frame.Right + 10), 20);
                lbl_size.Frame = new CGRect(iv_type.Frame.Right + 10, lbl_fileName.Frame.Bottom, (width * 0.5) - (iv_type.Frame.Right + 10), 20);
                
                lbl_typeName.Frame = new CGRect(lbl_fileName.Frame.Right + paddingLeft, paddingTop, (width * 0.5) - paddingLeft, 20);

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
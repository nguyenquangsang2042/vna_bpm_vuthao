using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPMOPMobile.Bean;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobileV1.iOS.Components
{
    class ControlAttachmentVertical : ControlBase
    {
        UIViewController parentView { get; set; }
        NSIndexPath indexPath { get; set; }
        ViewElement element { get; set; }

        UITableView tableView_attachment;
        List<KeyValuePair<string, string>> lst_attachment = new List<KeyValuePair<string, string>>();
        public KeyValuePair<string, string> currentAttachment { get; set; }

        public ControlAttachmentVertical(UIViewController _parentView, ViewElement _element, NSIndexPath _indexPath)
        {
            parentView = _parentView;
            element = _element;
            indexPath = _indexPath;
            InitializeComponent();
        }

        public override void InitializeComponent()
        {
            base.InitializeComponent();


            tableView_attachment = new UITableView();

            this.Add(tableView_attachment);

            this.WillRemoveSubview(lbl_value);
            this.WillRemoveSubview(BT_action);
            this.WillRemoveSubview(lbl_line);
        }

        public void HandleSeclectItem(KeyValuePair<string, string> _attachment)
        {
            //if (parentView != null && parentView.GetType() == typeof(ViewDetailController))
            //{
            //    currentAttachment = _attachment;
            //    ViewDetailController controller = (ViewDetailController)parentView;
            //    controller.NavigatorToView(element, indexPath, this);
            //}
        }

        public override void InitializeFrameView(CGRect frame)
        {
            this.Frame = frame;

            const int paddingTop = 10;
            const int spaceView = 5;

            lbl_title.HeightAnchor.ConstraintEqualTo(20).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1.0f, paddingTop).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
            NSLayoutConstraint.Create(lbl_title, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;

            var topAnchor = (20 + paddingTop + spaceView);
            tableView_attachment.Frame = new CGRect(0, topAnchor, frame.Width, frame.Height - topAnchor);
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
            if (data.Contains(";#"))
            {
                var arrAttachment = data.Split(new string[] { ";#" }, StringSplitOptions.None);
                if (arrAttachment.Length > 2)
                {
                    for (var i = 0; i < arrAttachment.Length; i += 2)
                    {
                        KeyValuePair<string, string> item = new KeyValuePair<string, string>(arrAttachment[i], arrAttachment[i + 1]);
                        lst_attachment.Add(item);
                    }
                }
                else
                    lst_attachment.Add(new KeyValuePair<string, string>(arrAttachment[0], arrAttachment[1]));
            }

            tableView_attachment.Source = new Attachment_TableSource(lst_attachment, this);
            tableView_attachment.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            tableView_attachment.ReloadData();
        }

        #region custom views
        #region attachment source table
        private class Attachment_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellAttachmentID");
            ControlAttachmentVertical parentView;
            List<KeyValuePair<string, string>> lst_attachment = new List<KeyValuePair<string, string>>();

            public Attachment_TableSource(List<KeyValuePair<string, string>> _lst_attachment, ControlAttachmentVertical _parentview)
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
                return 1;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 50;
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
            ControlAttachmentVertical parentView { get; set; }
            NSIndexPath currentIndexPath { get; set; }

            KeyValuePair<string, string> attachment { get; set; }
            UILabel lbl_name, lbl_line;
            UIImageView iv_type;

            public Attachment_cell_custom(ControlAttachmentVertical _parentView, NSString cellID, KeyValuePair<string, string> _attachment, NSIndexPath _currentIndexPath) : base(UITableViewCellStyle.Default, cellID)
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
                iv_type.Image = UIImage.FromFile("Icons/icon_pdf.png");

                lbl_name = new UILabel()
                {
                    Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
                    TextColor = UIColor.DarkGray,
                    TextAlignment = UITextAlignment.Left,
                };

                lbl_line = new UILabel()
                {
                    BackgroundColor = UIColor.FromRGB(144, 164, 174),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };

                ContentView.AddSubviews(new UIView[] { iv_type, lbl_name, lbl_line });
                loadData();
            }

            public void loadData()
            {
                try
                {
                    lbl_name.Text = attachment.Value;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ComponentAttachmentVertical - Control_cell_custom - loaddata- ERR: " + ex.ToString());
                }
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();

                iv_type.TranslatesAutoresizingMaskIntoConstraints = false;
                iv_type.WidthAnchor.ConstraintEqualTo(20).Active = true;
                iv_type.HeightAnchor.ConstraintEqualTo(20).Active = true;
                NSLayoutConstraint.Create(this.iv_type, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
                NSLayoutConstraint.Create(this.iv_type, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, this, NSLayoutAttribute.CenterY, 1.0f, 0.0f).Active = true;

                lbl_name.TranslatesAutoresizingMaskIntoConstraints = false;
                lbl_name.HeightAnchor.ConstraintEqualTo(20).Active = true;
                NSLayoutConstraint.Create(this.lbl_name, NSLayoutAttribute.Left, NSLayoutRelation.Equal, iv_type, NSLayoutAttribute.Right, 1.0f, 5.0f).Active = true;
                NSLayoutConstraint.Create(this.lbl_name, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;
                NSLayoutConstraint.Create(this.lbl_name, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, this, NSLayoutAttribute.CenterY, 1.0f, 0.0f).Active = true;

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
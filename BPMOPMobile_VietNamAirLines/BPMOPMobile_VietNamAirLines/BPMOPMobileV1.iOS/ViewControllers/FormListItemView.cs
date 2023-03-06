using System;
using System.Collections.Generic;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobileV1.iOS.IOSClass;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json;
using UIKit;
using Xamarin.iOS;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class FormListItemView : UIViewController
    {
        BeanAttachFile attachFile;
        List<BeanAttachFileCategory> lst_attachCate;
        bool isMultichoice;
        ViewElement element;
        UIViewController parentView { get; set; }

        public FormListItemView(IntPtr handle) : base(handle)
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

            ViewConfiguration();
            loadContent();

            #region delegate
            BT_back.TouchUpInside += BT_back_TouchUpInside;
            #endregion
        }

        #endregion

        #region private - public
        public void setContent(UIViewController _parentView, BeanAttachFile _attach, ViewElement _element, bool _isMultiChoice)
        {
            parentView = _parentView;
            attachFile = _attach;
            element = _element;
        }

        private void ViewConfiguration()
        {
            //var model = DeviceHardware.Model;
            //if (model.Contains("X") || model.Contains("11"))
            //{
            //    headerView_constantHeight.Constant = 80;
            //}

            headerView_constantHeight.Constant = 45 + 10 + CmmIOSFunction.GetHeaderViewHeight();

            table_content.ContentInset = new UIEdgeInsets(-35, 0, 0, 0);
            table_content.TableFooterView = new UIView();
            table_content.TableHeaderView = new UIView();
            if (isMultichoice)
                BT_agree.Hidden = false;
            else
                BT_agree.Hidden = true;
        }

        private void loadContent()
        {
            lst_attachCate = JsonConvert.DeserializeObject<List<BeanAttachFileCategory>>(element.DataSource);

            string attachCate = "";
            if (!string.IsNullOrEmpty(attachFile.AttachTypeName))
            {
                attachCate = attachFile.AttachTypeName;
            }
            if (lst_attachCate != null)
            {
                foreach (var item in lst_attachCate)
                {
                    if (item.Title == attachCate)
                        item.IsSelected = true;
                    else
                        item.IsSelected = false;
                }
            }

            //if (!string.IsNullOrEmpty(attachFile.Category))
            //{
            //    if (attachFile.Category.Contains(";#"))
            //        attachCate = attachFile.Category.Split(";#", StringSplitOptions.None)[1];
            //    else
            //        attachCate = attachFile.Category;
            //}

            //if (lst_attachCate != null)
            //{
            //    foreach (var item in lst_attachCate)
            //    {
            //        if (item.DocumentTypeValue == attachCate)
            //            item.IsSelected = true;
            //        else
            //            item.IsSelected = false;
            //    }
            //}

            if (lst_attachCate != null && lst_attachCate.Count > 0)
            {
                lbl_nodata.Hidden = true;
                table_content.Hidden = false;
                table_content.Source = new AttachmentCate_TableSource(lst_attachCate, attachFile, this);
                table_content.ReloadData();
            }
            else
            {
                lbl_nodata.Hidden = false;
                table_content.Hidden = true;
            }


        }

        private void HandleSeclectItem(BeanAttachFileCategory fileCategorySelected)
        {
            foreach (var item in lst_attachCate)
            {
                if (item.DocumentTypeValue == fileCategorySelected.DocumentTypeValue)
                    item.IsSelected = true;
                else
                    item.IsSelected = false;
            }

            if (parentView.GetType() == typeof(RequestDetailsV2))
            {
                RequestDetailsV2 controller = (RequestDetailsV2)parentView;
                //attachFile.Category = fileCategorySelected.ID + ";#" + fileCategorySelected.DocumentTypeValue;
                attachFile.AttachTypeId = fileCategorySelected.ID;
                attachFile.AttachTypeName = fileCategorySelected.Title;
                controller.ReloadAttachmentElement(element, attachFile);
            }

            this.NavigationController.PopViewController(true);

        }
        #endregion

        #region events
        private void BT_back_TouchUpInside(object sender, EventArgs e)
        {
            this.NavigationController.PopViewController(true);
        }
        #endregion

        #region custom class
        #region attachment source table
        private class AttachmentCate_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellAttachmentID");
            FormListItemView parentView;
            List<BeanAttachFileCategory> lst_attachment_cate = new List<BeanAttachFileCategory>();
            BeanAttachFile attachFile;

            public AttachmentCate_TableSource(List<BeanAttachFileCategory> _lst_attachmentcate, BeanAttachFile _attach, FormListItemView _parentview)
            {
                lst_attachment_cate = _lst_attachmentcate;
                attachFile = _attach;
                parentView = _parentview;
            }

            public override nfloat GetHeightForFooter(UITableView tableView, nint section)
            {
                return -1;
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                return 0;
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
                return lst_attachment_cate.Count;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                var itemSelected = lst_attachment_cate[indexPath.Row];
                parentView.HandleSeclectItem(itemSelected);
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var attachmentCate = lst_attachment_cate[indexPath.Row];
                AttachmentCate_cell_custom cell = new AttachmentCate_cell_custom(parentView, cellIdentifier, attachmentCate, indexPath);
                return cell;
            }
        }
        private class AttachmentCate_cell_custom : UITableViewCell
        {
            FormListItemView parentView { get; set; }
            NSIndexPath currentIndexPath { get; set; }
            BeanAttachFileCategory attachmentCate { get; set; }
            UILabel lbl_name, lbl_line;
            UIImageView img_selected;

            public AttachmentCate_cell_custom(FormListItemView _parentView, NSString cellID, BeanAttachFileCategory _attachment, NSIndexPath _currentIndexPath) : base(UITableViewCellStyle.Default, cellID)
            {
                parentView = _parentView;
                attachmentCate = _attachment;
                currentIndexPath = _currentIndexPath;
                Accessory = UITableViewCellAccessory.None;
                viewConfiguration();
                UpdateCell();
            }

            private void viewConfiguration()
            {
                img_selected = new UIImageView();
                img_selected.Image = UIImage.FromFile("Icons/icon_check.png");
                img_selected.Hidden = true;

                lbl_name = new UILabel()
                {
                    Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular),
                    TextColor = UIColor.FromRGB(65, 80, 134),
                    TextAlignment = UITextAlignment.Left,
                };

                lbl_line = new UILabel()
                {
                    BackgroundColor = UIColor.FromRGB(144, 164, 174)
                };

                ContentView.AddSubviews(new UIView[] { img_selected, lbl_name, lbl_line });
                lbl_line.Hidden = true;
            }

            public void UpdateCell()
            {
                try
                {
                    string fileExt = string.Empty;

                    //title
                    lbl_name.Text = attachmentCate.Title;
                    if (attachmentCate.IsSelected)
                        img_selected.Hidden = false;
                    else
                        img_selected.Hidden = true;
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
                lbl_name.Frame = new CGRect(20, 15, width - 70, 30);
                img_selected.Frame = new CGRect(ContentView.Frame.Width - 50, 15, 16, 16);
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


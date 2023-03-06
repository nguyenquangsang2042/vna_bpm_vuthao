using System;
using System.Collections.Generic;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobileV1.iOS.ViewControllers;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobileV1.iOS.CustomControlClass
{
    class Custom_WorkflowCategory : UIView
    {
        UITableView table_menu;

        private Custom_WorkflowCategory()
        {
            this.BackgroundColor = UIColor.White;

            table_menu = new UITableView();
            table_menu.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            table_menu.ContentInset = new UIEdgeInsets(0, 0, 0, 0);

            this.AddSubview(table_menu);
        }

        private static Custom_WorkflowCategory instance = null;
        public static Custom_WorkflowCategory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Custom_WorkflowCategory();
                }
                return instance;
            }
        }

        public void InitFrameView(CGRect frame)
        {
            this.Frame = frame;

            table_menu.Frame = new CGRect(0, 0, Frame.Width, Frame.Height);
        }

        public void TableLoadData()
        {
            table_menu.Source = new MenuOption_TableSource(ListItemMenu, this);
        }

        public void AddShadowForView()
        {
            this.Layer.ShadowColor = UIColor.DarkGray.CGColor;
            this.Layer.ShadowPath = UIBezierPath.FromRect(new CGRect(2, 2, Frame.Width, Frame.Height)).CGPath;
            this.Layer.ShadowRadius = 5;
            this.Layer.ShadowOffset = new CGSize(0, 2);
            this.Layer.ShadowOpacity = 1;
            this.ClipsToBounds = false;
        }

        public List<BeanWorkflowCategory> ListItemMenu { get; set; }

        public UIViewController viewController { get; set; }

        public UIButton BtnInputView { get; set; }

        public bool ItemNoIcon { get; set; }

        public int RowHeigth => 34;

        private void HandleItemSelect(BeanWorkflowCategory _cate)
        {
            if (viewController != null && viewController.GetType() == typeof(BroadView))
            {
                BroadView controller = (BroadView)viewController;
                controller.HandelMenuWorkFlowCategoryResult(_cate);
            }

        }

        #region custom views
        #region table data source user
        private class MenuOption_TableSource : UITableViewSource
        {
            List<BeanWorkflowCategory> lst_menu;
            NSString cellIdentifier = new NSString("cellMenuOption");
            Custom_WorkflowCategory parentView;

            public MenuOption_TableSource(List<BeanWorkflowCategory> _menu, Custom_WorkflowCategory _parentview)
            {
                parentView = _parentview;
                lst_menu = _menu;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return (lst_menu != null) ? lst_menu.Count : 0;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 34;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                var itemSelected = lst_menu[indexPath.Row];
                parentView.HandleItemSelect(itemSelected);
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var menu = lst_menu[indexPath.Row];
                MenuOption_Cell_Custom cell = new MenuOption_Cell_Custom(parentView, cellIdentifier, menu, indexPath);

                return cell;
            }
        }

        private class MenuOption_Cell_Custom : UITableViewCell
        {
            Custom_WorkflowCategory parentView { get; set; }
            NSIndexPath currentIndexPath { get; set; }
            BeanWorkflowCategory menu { get; set; }
            UILabel lbl_name;
            UIImageView iv_icon;

            public MenuOption_Cell_Custom(Custom_WorkflowCategory _parentView, NSString cellID, BeanWorkflowCategory _menu, NSIndexPath _currentIndexPath) : base(UITableViewCellStyle.Default, cellID)
            {
                parentView = _parentView;
                menu = _menu;
                currentIndexPath = _currentIndexPath;
                Accessory = UITableViewCellAccessory.None;
                viewConfiguration();
            }

            private void viewConfiguration()
            {
                ContentView.BackgroundColor = UIColor.White;

                iv_icon = new UIImageView();
                iv_icon.ContentMode = UIViewContentMode.ScaleAspectFit;
                iv_icon.Image = UIImage.FromFile("Icons/icon_check.png");

                lbl_name = new UILabel()
                {
                    Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
                    TextColor = UIColor.DarkGray,
                    TextAlignment = UITextAlignment.Left,
                };

                ContentView.AddSubviews(new UIView[] { iv_icon, lbl_name });
                loadData();
            }

            public void loadData()
            {
                try
                {
                    if (menu.ID == 0)
                        lbl_name.Text = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");

                    lbl_name.Text = menu.Title;

                    if (menu.IsSelected)
                    {
                        lbl_name.TextColor = UIColor.FromRGB(51, 95, 190);
                        iv_icon.Hidden = false;
                    }
                    else
                    {
                        lbl_name.TextColor = UIColor.Black;
                        iv_icon.Hidden = true;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("MenuOption_Cell_Custom - MenuOption_Cell_Custom - loaddata- ERR: " + ex.ToString());
                }
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();

                if (parentView.ItemNoIcon)
                {
                    iv_icon.Frame = CGRect.Empty;

                    lbl_name.TranslatesAutoresizingMaskIntoConstraints = false;
                    lbl_name.HeightAnchor.ConstraintEqualTo(20).Active = true;
                    NSLayoutConstraint.Create(this.lbl_name, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 18.0f).Active = true;
                    NSLayoutConstraint.Create(this.lbl_name, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;
                    NSLayoutConstraint.Create(this.lbl_name, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, this, NSLayoutAttribute.CenterY, 1.0f, 0.0f).Active = true;
                }
                else
                {
                    iv_icon.TranslatesAutoresizingMaskIntoConstraints = false;
                    iv_icon.WidthAnchor.ConstraintEqualTo(16).Active = true;
                    iv_icon.HeightAnchor.ConstraintEqualTo(16).Active = true;
                    NSLayoutConstraint.Create(this.iv_icon, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 20.0f).Active = true;
                    NSLayoutConstraint.Create(this.iv_icon, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, this, NSLayoutAttribute.CenterY, 1.0f, 0.0f).Active = true;

                    lbl_name.TranslatesAutoresizingMaskIntoConstraints = false;
                    lbl_name.HeightAnchor.ConstraintEqualTo(20).Active = true;
                    NSLayoutConstraint.Create(this.lbl_name, NSLayoutAttribute.Left, NSLayoutRelation.Equal, iv_icon, NSLayoutAttribute.Right, 1.0f, 10.0f).Active = true;
                    NSLayoutConstraint.Create(this.lbl_name, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;
                    NSLayoutConstraint.Create(this.lbl_name, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, this, NSLayoutAttribute.CenterY, 1.0f, 0.0f).Active = true;
                }
            }
        }
        #endregion
        #endregion
    }
}

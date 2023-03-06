using System;
using System.Collections.Generic;
using System.Linq;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.CustomControlClass
{
    public class Custom_DuedateCategory : UIView
    {
        UIView baseView, line;
        UIButton BT_clear;
        UITableView table_menu;
        public List<ClassMenu> ListClassMenu { get; set; }
        public ClassMenu ClassMenu_selected { get; set; }
        public UIViewController viewController { get; set; }
        public UIButton BtnInputView { get; set; }
        public UILabel LBL_inputView { get; set; }
        public bool ItemNoIcon { get; set; }
        public int RowHeigth => 34;

        public Custom_DuedateCategory()
        {
            this.BackgroundColor = UIColor.White;

            baseView = new UIView();

            table_menu = new UITableView();
            table_menu.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            table_menu.ContentInset = new UIEdgeInsets(0, 0, 0, 0);
            table_menu.AllowsMultipleSelection = true;

            line = new UIView();
            line.BackgroundColor = UIColor.FromRGB(232, 232, 232);
        }

        private static Custom_DuedateCategory instance = null;
        public static Custom_DuedateCategory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Custom_DuedateCategory();
                }
                return instance;
            }
        }

        public void InitFrameView(CGRect frame, bool hasBT_Clear)
        {
            this.Frame = frame;
            if (hasBT_Clear)
            {
                BT_clear = new UIButton();
                BT_clear.SetTitleColor(UIColor.FromRGB(94, 94, 94), UIControlState.Normal);
                BT_clear.SetTitle(CmmFunction.GetTitle("TEXT_DELETE", "Xóa"), UIControlState.Normal);
                BT_clear.Font = UIFont.FromName("ArialMT", 14f);
                BT_clear.SetImage(UIImage.FromFile("Icons/icon_close.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
                BT_clear.ImageView.TintColor = UIColor.FromRGB(94, 94, 94);
                UIImage img = CmmIOSFunction.ScaleImageFollowHeight(UIImage.FromFile("Icons/icon_close.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), 17);
                BT_clear.SetImage(img, UIControlState.Normal);
                BT_clear.ImageEdgeInsets = new UIEdgeInsets(0, -20, 0, 0);
                BT_clear.TitleEdgeInsets = new UIEdgeInsets(0, 15, 0, 0);
                BT_clear.TouchUpInside += BT_clear_TouchUpInside;
                baseView.AddSubviews(table_menu, line, BT_clear);

                baseView.Frame = new CGRect(0, 0, Frame.Width, Frame.Height);
                table_menu.Frame = new CGRect(0, 0, Frame.Width, Frame.Height - 40);
                line.Frame = new CGRect(0, table_menu.Frame.Bottom, Frame.Width, 1);
                BT_clear.Frame = new CGRect(Frame.Width - 100, table_menu.Frame.Bottom, 100, 40);
            }
            else
            {
                baseView.AddSubviews(table_menu, line);
                baseView.Frame = new CGRect(0, 0, Frame.Width, Frame.Height);
                table_menu.Frame = new CGRect(0, 0, Frame.Width, Frame.Height);
            }
            this.AddSubview(baseView);


        }

        public void TableLoadData()
        {
            //cau hinh khong co icon phia truoc item
            ItemNoIcon = false;
            table_menu.Source = new Duedate_TableSource(ListClassMenu, this);
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

        private void HandleItemSelect(ClassMenu _menu)
        {
            foreach (var item in ListClassMenu)
            {
                if (item.ID == _menu.ID)
                    item.isSelected = true;
                else
                    item.isSelected = false;
            }

            ClassMenu_selected = ListClassMenu.Where(i => i.isSelected).FirstOrDefault();

            if (ClassMenu_selected != null)
            {
                LBL_inputView.Font = UIFont.FromName("ArialMT", 14f);
                LBL_inputView.TextColor = UIColor.Black;

                if (CmmVariable.SysConfig.LangCode == "1033")
                    LBL_inputView.Text = ClassMenu_selected.title;
                else
                    LBL_inputView.Text = ClassMenu_selected.title;
            }
            else
            {
                LBL_inputView.Text = CmmFunction.GetTitle("TEXT_STATUS", "Tình trạng");
                LBL_inputView.TextColor = UIColor.FromRGB(229, 229, 229);
                LBL_inputView.Font = UIFont.FromName("Arial-ItalicMT", 14f);
            }

            if (viewController.GetType() == typeof(SearchView))
            {
                SearchView searchView = viewController as SearchView;
                searchView.lst_trangthai = ListClassMenu;
                searchView.TrangThaiSelected = ClassMenu_selected;
            }

            CloseInstance();
        }

        private void CloseInstance()
        {
            if (viewController.GetType() == typeof(SearchView))
            {
                SearchView search = viewController as SearchView;
                search.CloseDueDateCateInstance();
            }
            if (viewController.GetType() == typeof(FormFillterWorkFlowView))
            {
                FormFillterWorkFlowView formFillterWorkFlowView = viewController as FormFillterWorkFlowView;
                //formFillterWorkFlowView.CloseDueDateCateInstance();
            }
            if (viewController.GetType() == typeof(FormFillterToDoView))
            {
                FormFillterToDoView formFillterToDoView = viewController as FormFillterToDoView;
                formFillterToDoView.CloseDueDateCateInstance();
            }

        }

        private void BT_clear_TouchUpInside(object sender, EventArgs e)
        {
            ListClassMenu = ListClassMenu.Select(a => { a.isSelected = false; return a; }).ToList();
            ClassMenu_selected = null;

            LBL_inputView.Text = CmmFunction.GetTitle("TEXT_STATUS", "Tình trạng");
            LBL_inputView.TextColor = UIColor.FromRGB(229, 229, 229);
            LBL_inputView.Font = UIFont.FromName("Arial-ItalicMT", 14f);

            table_menu.ReloadData();
        }

        #region custom views
        #region table data source user
        private class Duedate_TableSource : UITableViewSource
        {
            List<ClassMenu> lst_DuedateStatus;
            NSString cellIdentifier = new NSString("cellMenuOption");
            Custom_DuedateCategory parentView;

            public Duedate_TableSource(List<ClassMenu> _menu, Custom_DuedateCategory _parentview)
            {
                parentView = _parentview;
                lst_DuedateStatus = _menu;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return (lst_DuedateStatus != null) ? lst_DuedateStatus.Count : 0;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 40;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                var itemSelected = lst_DuedateStatus[indexPath.Row];
                parentView.HandleItemSelect(itemSelected);
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var menu = lst_DuedateStatus[indexPath.Row];
                AppStatus_Cell_Custom cell = new AppStatus_Cell_Custom(parentView, cellIdentifier, menu, indexPath);

                return cell;
            }
        }

        private class AppStatus_Cell_Custom : UITableViewCell
        {
            Custom_DuedateCategory parentView { get; set; }
            NSIndexPath currentIndexPath { get; set; }
            ClassMenu status { get; set; }
            UIView line;
            UILabel lbl_name;
            UIImageView iv_icon;

            public AppStatus_Cell_Custom(Custom_DuedateCategory _parentView, NSString cellID, ClassMenu _status, NSIndexPath _currentIndexPath) : base(UITableViewCellStyle.Default, cellID)
            {
                parentView = _parentView;
                status = _status;
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
                    TextColor = UIColor.Black,
                    TextAlignment = UITextAlignment.Left,
                };

                line = new UIView();
                line.BackgroundColor = UIColor.FromRGB(229, 229, 229).ColorWithAlpha(0.7f);

                ContentView.AddSubviews(new UIView[] { iv_icon, lbl_name, line });
                loadData();
            }

            public void loadData()
            {
                try
                {
                    if (status.ID == 0)
                        lbl_name.Text = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");

                    lbl_name.Text = status.title;
                    //if (status.IsSelected)
                    //    Accessory = UITableViewCellAccessory.Checkmark;
                    //else
                    //    Accessory = UITableViewCellAccessory.None;

                    if (status.isSelected)
                    {
                        iv_icon.Hidden = false;
                    }
                    else
                    {
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
                    //iv_icon.Frame = CGRect.Empty;

                    lbl_name.TranslatesAutoresizingMaskIntoConstraints = false;
                    lbl_name.HeightAnchor.ConstraintEqualTo(20).Active = true;
                    NSLayoutConstraint.Create(this.lbl_name, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 18.0f).Active = true;
                    NSLayoutConstraint.Create(this.lbl_name, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;
                    NSLayoutConstraint.Create(this.lbl_name, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, this, NSLayoutAttribute.CenterY, 1.0f, 0.0f).Active = true;

                    line.Frame = new CGRect(0, ContentView.Frame.Bottom, ContentView.Frame.Width, 0.7f);
                }
                else
                {
                    lbl_name.TranslatesAutoresizingMaskIntoConstraints = false;
                    lbl_name.HeightAnchor.ConstraintEqualTo(20).Active = true;
                    NSLayoutConstraint.Create(this.lbl_name, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 20.0f).Active = true;
                    NSLayoutConstraint.Create(this.lbl_name, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, -35.0f).Active = true;
                    NSLayoutConstraint.Create(this.lbl_name, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, this, NSLayoutAttribute.CenterY, 1.0f, 0.0f).Active = true;

                    iv_icon.TranslatesAutoresizingMaskIntoConstraints = false;
                    iv_icon.WidthAnchor.ConstraintEqualTo(16).Active = true;
                    iv_icon.HeightAnchor.ConstraintEqualTo(16).Active = true;
                    NSLayoutConstraint.Create(this.iv_icon, NSLayoutAttribute.Left, NSLayoutRelation.Equal, lbl_name, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;
                    NSLayoutConstraint.Create(this.iv_icon, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, this, NSLayoutAttribute.CenterY, 1.0f, 0.0f).Active = true;

                    line.TranslatesAutoresizingMaskIntoConstraints = false;
                    line.HeightAnchor.ConstraintEqualTo(1).Active = true;
                    line.WidthAnchor.ConstraintEqualTo(this.Frame.Width).Active = true;
                    NSLayoutConstraint.Create(this.line, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 0.0f).Active = true;
                    NSLayoutConstraint.Create(this.line, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;
                    NSLayoutConstraint.Create(this.line, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, this, NSLayoutAttribute.Bottom, 1.0f, -2.0f).Active = true;
                }
            }
        }
        #endregion
        #endregion
    }
}

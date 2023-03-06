using System;
using System.Collections.Generic;
using System.Linq;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobileV1.iOS.IOSClass;
using BPMOPMobileV1.iOS.ViewControllers;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobileV1.iOS.CustomControlClass
{
    public class Custom_AppConditionCategory : UIView
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
        public bool showItemClear = false;

        string title = "";
        List<ClassMenu> ListClassMenuBK { get; set; }
        ClassMenu ClassMenu_selectedBK { get; set; }

        public Custom_AppConditionCategory()
        {
            this.BackgroundColor = UIColor.White;

            baseView = new UIView();

            table_menu = new UITableView();
            table_menu.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            table_menu.ContentInset = new UIEdgeInsets(0, 0, 0, 0);
            table_menu.AllowsMultipleSelection = true;

            line = new UIView();
            line.BackgroundColor = UIColor.FromRGB(232, 232, 232);

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
            this.AddSubview(baseView);

            if (!showItemClear)
                BT_clear.Hidden = true;
        }

        private static Custom_AppConditionCategory instance = null;
        public static Custom_AppConditionCategory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Custom_AppConditionCategory();
                }
                return instance;
            }
        }

        public void InitFrameView(CGRect frame)
        {
            this.Frame = frame;
            baseView.Frame = new CGRect(0, 0, Frame.Width, Frame.Height);
            if (!showItemClear)
                table_menu.Frame = new CGRect(0, 0, Frame.Width, Frame.Height);
            else
                table_menu.Frame = new CGRect(0, 0, Frame.Width, Frame.Height - 40);
            line.Frame = new CGRect(0, table_menu.Frame.Bottom, Frame.Width, 1);
            BT_clear.Frame = new CGRect(Frame.Width - 120, table_menu.Frame.Bottom, 100, 40);
        }

        public void TableLoadData()
        {
            //cau hinh khong co icon phia truoc item
            ItemNoIcon = false;
            table_menu.Source = new AppCondition_TableSource(ListClassMenu, this);
        }
        public void BackupData()
        {
            ListClassMenuBK = ExtensionCopy.CopyAll(ListClassMenu);
            ClassMenu_selectedBK = ExtensionCopy.CopyAll(ClassMenu_selected);
            title = LBL_inputView.Text;
        }

        public void CallBackupData()
        {
            if (viewController.GetType() == typeof(MainView))
            {
                MainView mainView = viewController as MainView;
                mainView.lst_conditionMenu_toMe = ExtensionCopy.CopyAll(ListClassMenuBK);
                mainView.conditionSelected_toMe = ExtensionCopy.CopyAll(ClassMenu_selectedBK);
            }
            else if (viewController.GetType() == typeof(RequestListView))
            {
                RequestListView requestListView = viewController as RequestListView;
                requestListView.lst_conditionMenu_toMe = ExtensionCopy.CopyAll(ListClassMenuBK);
                requestListView.conditionSelected_toMe = ExtensionCopy.CopyAll(ClassMenu_selectedBK);
            }
            //app
            //else if (viewController.GetType() == typeof(MainViewApp))
            //{
            //    MainViewApp mainViewApp = viewController as MainViewApp;
            //    mainViewApp.lst_conditionMenu_toMe = ExtensionCopy.CopyAll(ListClassMenuBK);
            //    mainViewApp.conditionSelected_toMe = ExtensionCopy.CopyAll(ClassMenu_selectedBK);
            //}
            //else if (viewController.GetType() == typeof(RequestListViewApp))
            //{
            //    RequestListViewApp requestListViewApp = viewController as RequestListViewApp;
            //    requestListViewApp.lst_conditionMenu_toMe = ExtensionCopy.CopyAll(ListClassMenuBK);
            //    requestListViewApp.conditionSelected_toMe = ExtensionCopy.CopyAll(ClassMenu_selectedBK);
            //}

            ListClassMenu = ExtensionCopy.CopyAll(ListClassMenuBK);
            ClassMenu_selected = ExtensionCopy.CopyAll(ClassMenu_selectedBK);
            LBL_inputView.Text = title;
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
            //turn off selected
            foreach (var item in ListClassMenu)
            {
                item.isSelected = false;
            }
            //turn on new select
            _menu.isSelected = !_menu.isSelected;
            ClassMenu_selected = ListClassMenu.Where(i => i.ID == _menu.ID).FirstOrDefault();
            ClassMenu_selected = _menu;

            if (ClassMenu_selected != null)
            {
                if (CmmVariable.SysConfig.LangCode == "1033")
                    LBL_inputView.Text = ClassMenu_selected.titleEN;
                else
                    LBL_inputView.Text = ClassMenu_selected.title;
            }
            //CloseInstance();
            if (viewController.GetType() == typeof(MainView))
            {
                MainView mainView = viewController as MainView;
                mainView.ClearViewStatus();
            }
            else if (viewController.GetType() == typeof(RequestListView))
            {
                RequestListView requestListView = viewController as RequestListView;
                requestListView.ClearViewStatus();
            }
            //app
            //else if (viewController.GetType() == typeof(MainViewApp))
            //{
            //    MainViewApp mainViewApp = viewController as MainViewApp;
            //    mainViewApp.ClearViewStatus();
            //}
            //else if (viewController.GetType() == typeof(RequestListViewApp))
            //{
            //    RequestListViewApp requestListViewApp = viewController as RequestListViewApp;
            //    requestListViewApp.ClearViewStatus();
            //}
            viewController.NavigationController.PopViewController(true);
        }

        private void CloseInstance()
        {
            if (viewController.GetType() == typeof(MainView))
            {
                MainView mainView = viewController as MainView;
                mainView.CloseConditionInstance();
            }
            else if (viewController.GetType() == typeof(RequestListView))
            {
                RequestListView requestListView = viewController as RequestListView;
                requestListView.CloseConditionInstance();
            }
            //app
            //else if (viewController.GetType() == typeof(MainViewApp))
            // {
            //     MainViewApp mainViewApp = viewController as MainViewApp;
            //     mainViewApp.CloseConditionInstance();
            // }
            // else if (viewController.GetType() == typeof(RequestListViewApp))
            // {
            //     RequestListViewApp requestListViewApp = viewController as RequestListViewApp;
            //     requestListViewApp.CloseConditionInstance();
            // }
        }

        private void BT_clear_TouchUpInside(object sender, EventArgs e)
        {
            ListClassMenu = ListClassMenu.Select(a => { a.isSelected = false; return a; }).ToList();
            ClassMenu_selected = null;

            ListClassMenu[0].isSelected = true;
            ClassMenu_selected = ListClassMenu[0];

            if (CmmVariable.SysConfig.LangCode == "1033")
                LBL_inputView.Text = ClassMenu_selected.titleEN;
            else
                LBL_inputView.Text = ClassMenu_selected.title;

            if (viewController.GetType() == typeof(MainView))
            {
                MainView parent = viewController as MainView;
                parent.conditionSelected_toMe = ClassMenu_selected;
            }

            table_menu.ReloadData();
        }

        #region custom views
        #region table data source user
        private class AppCondition_TableSource : UITableViewSource
        {
            List<ClassMenu> lst_conditionStatus;
            NSString cellIdentifier = new NSString("cellMenuOption");
            Custom_AppConditionCategory parentView;

            public AppCondition_TableSource(List<ClassMenu> _menu, Custom_AppConditionCategory _parentview)
            {
                parentView = _parentview;
                lst_conditionStatus = _menu;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return (lst_conditionStatus != null) ? lst_conditionStatus.Count : 0;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 40;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                var itemSelected = lst_conditionStatus[indexPath.Row];
                parentView.HandleItemSelect(itemSelected);
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var menu = lst_conditionStatus[indexPath.Row];
                AppStatus_Cell_Custom cell = new AppStatus_Cell_Custom(parentView, cellIdentifier, menu, indexPath);

                return cell;
            }
        }

        private class AppStatus_Cell_Custom : UITableViewCell
        {
            Custom_AppConditionCategory parentView { get; set; }
            NSIndexPath currentIndexPath { get; set; }
            ClassMenu status { get; set; }
            UIView line;
            UILabel lbl_name;
            UIImageView iv_icon;

            public AppStatus_Cell_Custom(Custom_AppConditionCategory _parentView, NSString cellID, ClassMenu _status, NSIndexPath _currentIndexPath) : base(UITableViewCellStyle.Default, cellID)
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

                    if (CmmVariable.SysConfig.LangCode == "1033")
                        lbl_name.Text = status.titleEN;
                    else
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

                    line.Frame = new CGRect(15, ContentView.Frame.Bottom, ContentView.Frame.Width - 30, 0.7f);
                }
                else
                {
                    lbl_name.TranslatesAutoresizingMaskIntoConstraints = false;
                    lbl_name.HeightAnchor.ConstraintEqualTo(20).Active = true;
                    NSLayoutConstraint.Create(this.lbl_name, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 15.0f).Active = true;
                    NSLayoutConstraint.Create(this.lbl_name, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, -35.0f).Active = true;
                    NSLayoutConstraint.Create(this.lbl_name, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, this, NSLayoutAttribute.CenterY, 1.0f, 0.0f).Active = true;

                    iv_icon.TranslatesAutoresizingMaskIntoConstraints = false;
                    iv_icon.WidthAnchor.ConstraintEqualTo(16).Active = true;
                    iv_icon.HeightAnchor.ConstraintEqualTo(16).Active = true;
                    NSLayoutConstraint.Create(this.iv_icon, NSLayoutAttribute.Left, NSLayoutRelation.Equal, lbl_name, NSLayoutAttribute.Right, 1.0f, 0.0f).Active = true;
                    NSLayoutConstraint.Create(this.iv_icon, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, this, NSLayoutAttribute.CenterY, 1.0f, 0.0f).Active = true;

                    line.TranslatesAutoresizingMaskIntoConstraints = false;
                    line.HeightAnchor.ConstraintEqualTo(1).Active = true;
                    NSLayoutConstraint.Create(this.line, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1.0f, 15.0f).Active = true;
                    NSLayoutConstraint.Create(this.line, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1.0f, -15.0f).Active = true;
                    NSLayoutConstraint.Create(this.line, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, this, NSLayoutAttribute.Bottom, 1.0f, -2.0f).Active = true;
                }
            }
        }
        #endregion
        #endregion
    }
}


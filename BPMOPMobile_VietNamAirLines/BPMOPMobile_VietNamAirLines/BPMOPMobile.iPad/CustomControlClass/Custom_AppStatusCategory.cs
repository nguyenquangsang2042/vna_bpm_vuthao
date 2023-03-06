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
    public class Custom_AppStatusCategory : UIView
    {
        UIView baseView, line;
        UIButton BT_clear;
        UITableView table_menu;
        public List<BeanAppStatus> ListAppStatus { get; set; }
        public List<BeanAppStatus> ListAppStatus_selected { get; set; }
        public UIViewController viewController { get; set; }
        public UIButton BtnInputView { get; set; }
        public UILabel LBL_inputView { get; set; }
        public bool ItemNoIcon { get; set; }
        public int RowHeigth => 34;

        public Custom_AppStatusCategory()
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
        }

        private static Custom_AppStatusCategory instance = null;
        public static Custom_AppStatusCategory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Custom_AppStatusCategory();
                }
                return instance;
            }
        }

        public void InitFrameView(CGRect frame)
        {
            this.Frame = frame;
            baseView.Frame = new CGRect(0, 0, Frame.Width, Frame.Height);
            table_menu.Frame = new CGRect(0, 0, Frame.Width, Frame.Height - 40);
            line.Frame = new CGRect(0, table_menu.Frame.Bottom, Frame.Width, 1);
            BT_clear.Frame = new CGRect(Frame.Width - 120, table_menu.Frame.Bottom, 100, 40);
        }

        public void TableLoadData()
        {
            //cau hinh khong co icon phia truoc item
            ItemNoIcon = false;
            table_menu.Source = new AppStatus_TableSource(ListAppStatus, this);
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

        private void HandleItemSelect(BeanAppStatus _cate)
        {
            string temp = "";
            string res_title = "";
            if (ListAppStatus_selected == null)
                ListAppStatus_selected = new List<BeanAppStatus>();

            _cate.IsSelected = !_cate.IsSelected;
            var res = ListAppStatus.Where(i => i.ID == _cate.ID).FirstOrDefault();
            res = _cate;
            table_menu.ReloadData();

            ListAppStatus_selected = ListAppStatus.Where(c => c.IsSelected == true).ToList();
            if (ListAppStatus_selected.Count == 1)
            {
                LBL_inputView.Font = UIFont.FromName("ArialMT", 14f);
                LBL_inputView.TextColor = UIColor.Black;

                if (CmmVariable.SysConfig.LangCode == "1033")
                    LBL_inputView.Text = ListAppStatus_selected[0].TitleEN;
                else
                    LBL_inputView.Text = ListAppStatus_selected[0].Title;
            }
            else if (ListAppStatus_selected.Count > 1)
            {
                LBL_inputView.Font = UIFont.FromName("ArialMT", 14f);
                LBL_inputView.TextColor = UIColor.Black;

                List<string> lst_titleStatus = new List<string>();

                foreach (var item in ListAppStatus_selected)
                {
                    if (CmmVariable.SysConfig.LangCode == "1033")
                        lst_titleStatus.Add(item.TitleEN);
                    else //if (CmmVariable.SysConfig.LangCode == "1066")
                        lst_titleStatus.Add(item.Title);
                }

                temp = string.Join(", ", lst_titleStatus);
                var widthStatus = StringExtensions.MeasureString(temp, 14).Width + 50;

                nfloat max_width = 0;
                for (int i = 0; i < lst_titleStatus.Count; i++)
                {
                    max_width += StringExtensions.MeasureString(lst_titleStatus[i], 14).Width + 10;
                    if (max_width > LBL_inputView.Frame.Width)
                    {
                        LBL_inputView.Text = res_title + " +" + (lst_titleStatus.Count - i);
                        break;
                    }
                    else
                    {
                        res_title += lst_titleStatus[i] + ", ";
                        LBL_inputView.Text = temp.TrimEnd(',');
                    }
                }

            }
            else
            {
                LBL_inputView.Text = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
                //LBL_inputView.TextColor = UIColor.FromRGB(229, 229, 229);
                //LBL_inputView.Font = UIFont.FromName("Arial-ItalicMT", 14f);
            }
        }

        private void BT_clear_TouchUpInside(object sender, EventArgs e)
        {
            ListAppStatus = ListAppStatus.Select(a => { a.IsSelected = false; return a; }).ToList();
            ListAppStatus_selected = new List<BeanAppStatus>();

            LBL_inputView.Text = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");
            //LBL_inputView.TextColor = UIColor.FromRGB(229, 229, 229);
            //LBL_inputView.Font = UIFont.FromName("Arial-ItalicMT", 14f);

            table_menu.ReloadData();
        }

        #region custom views
        #region table data source user
        private class AppStatus_TableSource : UITableViewSource
        {
            List<BeanAppStatus> lst_appStatus;
            NSString cellIdentifier = new NSString("cellMenuOption");
            Custom_AppStatusCategory parentView;

            public AppStatus_TableSource(List<BeanAppStatus> _menu, Custom_AppStatusCategory _parentview)
            {
                parentView = _parentview;
                lst_appStatus = _menu;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return (lst_appStatus != null) ? lst_appStatus.Count : 0;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 40;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                var itemSelected = lst_appStatus[indexPath.Row];
                parentView.HandleItemSelect(itemSelected);
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var menu = lst_appStatus[indexPath.Row];
                AppStatus_Cell_Custom cell = new AppStatus_Cell_Custom(parentView, cellIdentifier, menu, indexPath);

                return cell;
            }
        }

        private class AppStatus_Cell_Custom : UITableViewCell
        {
            Custom_AppStatusCategory parentView { get; set; }
            NSIndexPath currentIndexPath { get; set; }
            BeanAppStatus status { get; set; }
            UIView line;
            UILabel lbl_name;
            UIImageView iv_icon;

            public AppStatus_Cell_Custom(Custom_AppStatusCategory _parentView, NSString cellID, BeanAppStatus _status, NSIndexPath _currentIndexPath) : base(UITableViewCellStyle.Default, cellID)
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
                    else
                        lbl_name.Text = status.Title;
                    //if (status.IsSelected)
                    //    Accessory = UITableViewCellAccessory.Checkmark;
                    //else
                    //    Accessory = UITableViewCellAccessory.None;

                    if (status.IsSelected)
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

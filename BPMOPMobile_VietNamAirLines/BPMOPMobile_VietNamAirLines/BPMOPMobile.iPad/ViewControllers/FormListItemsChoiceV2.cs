using System;
using System.Collections.Generic;
using System.Linq;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using SQLite;
using UIKit;

namespace BPMOPMobile.iPad
{
    public partial class FormListItemsChoiceV2 : UIViewController
    {
        UIViewController parentView { get; set; }
        bool isMultiSelect;
        int type = 0; //0: tranghtai || 1: tinhtrang || 2: hanxuly
        List<ClassMenu> lst_trangthai;
        public List<BeanAppStatus> lst_tinhtrang { get; set; }
        public List<BeanAppStatus> lst_tinhtrang_selected { get; set; }
        List<ClassMenu> lst_hanxuly;
        public bool isAll;

        public FormListItemsChoiceV2(IntPtr handel) : base(handel)
        {
        }

        #region override
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewConfiguration();
            setlangTitle();

            if (type == 0)
                loadContent_Trangthai();
            else if (type == 1)
                loadContent_Tinhtrang();
            else if (type == 2)
                loadContent_Hanxuly();

            #region delegate
            BT_back.TouchUpInside += BT_back_TouchUpInside;
            //BT_approve.TouchUpInside += BT_approve_TouchUpInside;

            #endregion
        }
        #endregion

        #region private - public method
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_parentView"></param>
        /// <param name="_lst_trangthai"></param>
        /// <param name="_lst_tinhtrang"></param>
        /// <param name="_lst_hanxuly"></param>
        /// <param name="_isMultiSelect"></param>
        /// <param name="_type">0: trangthai || 1: tinhtrang || 2: hanxuly</param>
        public void setContent(UIViewController _parentView, List<ClassMenu> _lst_trangthai, List<BeanAppStatus> _lst_tinhtrang, List<ClassMenu> _lst_hanxuly, bool _isMultiSelect, int _type)
        {
            isMultiSelect = _isMultiSelect;
            lst_trangthai = _lst_trangthai;
            lst_tinhtrang = _lst_tinhtrang;
            lst_hanxuly = _lst_hanxuly;
            parentView = _parentView;
            type = _type;
        }

        private void ViewConfiguration()
        {


            table_content.ContentInset = new UIEdgeInsets(0, 0, 0, 0);
        }

        private void loadContent_Trangthai()
        {
            try
            {
                table_content.Source = new Trangthai_TableSource(lst_trangthai, true, this);
                table_content.ReloadData();
            }
            catch (Exception ex)
            {
                Console.WriteLine("FromListItemsChoice - loadContent - ERR: " + ex.ToString());
            }
        }
        public void Handle_Trangthai_SeclectItem(ClassMenu _menu, NSIndexPath _indexPath)
        {

            foreach (var item in lst_trangthai)
            {
                if (item.ID == _menu.ID)
                    item.isSelected = true;
                else
                    item.isSelected = false;
            }

            if (parentView.GetType() == typeof(ViewSearchV2))
            {
                ViewSearchV2 searchView = parentView as ViewSearchV2;
                searchView.ReloadTrangThaiCategory(lst_trangthai);
            }
            else if (parentView.GetType() == typeof(FormFillterToDoView))
            {
                FormFillterToDoView formFillterToDo = parentView as FormFillterToDoView;
                formFillterToDo.ReloadTrangThaiCategory(lst_trangthai);
            }
            this.NavigationController.PopViewController(true);
        }

        private void loadContent_Tinhtrang()
        {
            try
            {
                CheckTinhtrangSelectAll();

                table_content.Source = new Tinhtrang_TableSource(lst_tinhtrang, this);
                table_content.ReloadData();
            }
            catch (Exception ex)
            {
                Console.WriteLine("FromListItemsChoice - loadContent - ERR: " + ex.ToString());
            }
        }
        public void Handle_Tinhtrang_SeclectItem(BeanAppStatus _menu, NSIndexPath _indexPath)
        {
            //string temp = "";
            //string res_title = "";
            if (lst_tinhtrang_selected == null)
                lst_tinhtrang_selected = new List<BeanAppStatus>();


            if (lst_tinhtrang.FindAll(o => o.IsSelected).Count == 1 && lst_tinhtrang.Where(i => i.ID == _menu.ID).FirstOrDefault().IsSelected)
            {
                return;
            }
            else
            {
                _menu.IsSelected = !_menu.IsSelected;
                var res = lst_tinhtrang.Where(i => i.ID == _menu.ID).FirstOrDefault();
                res = _menu;

                CheckTinhtrangSelectAll();
                table_content.ReloadData();
            }
        }

        private void CheckTinhtrangSelectAll()
        {
            var reslst = lst_tinhtrang.Where(c => c.IsSelected == true).ToList();
            if (reslst.Count == lst_tinhtrang.Count)
                isAll = true;
            //else if (reslst.Count == 0)
            //    isAll = true;
            else
                isAll = false;
        }

        public void Handel_Tinhtrang_AllSelected()
        {
            if (isAll)
            {
                lst_tinhtrang = lst_tinhtrang.Select(s => { s.IsSelected = true; return s; }).ToList();
            }
            else
            {
                lst_tinhtrang = lst_tinhtrang.Select(s => { s.IsSelected = false; return s; }).ToList();
            }

            table_content.ReloadData();
        }

        private void loadContent_Hanxuly()
        {
            try
            {
                table_content.Source = new Trangthai_TableSource(lst_hanxuly, false, this);
                table_content.ReloadData();
            }
            catch (Exception ex)
            {
                Console.WriteLine("FromListItemsChoice - loadContent - ERR: " + ex.ToString());
            }
        }
        public void Handle_Hanxuly_SeclectItem(ClassMenu _menu, NSIndexPath _indexPath)
        {

            foreach (var item in lst_hanxuly)
            {
                if (item.ID == _menu.ID)
                    item.isSelected = true;
                else
                    item.isSelected = false;
            }

            if (parentView.GetType() == typeof(ViewSearchV2))
            {
                ViewSearchV2 searchView = parentView as ViewSearchV2;
                searchView.ReloadDueDateCategory(lst_hanxuly);
            }
            else if (parentView.GetType() == typeof(FormFillterToDoView))
            {
                FormFillterToDoView formFillterToDoView = parentView as FormFillterToDoView;
                formFillterToDoView.ReloadDueDateCategory(lst_hanxuly);

            }
            else if (parentView.GetType() == typeof(FormFillterWorkFlowView))
            {
                FormFillterWorkFlowView formFillterWorkFlow = parentView as FormFillterWorkFlowView;
                formFillterWorkFlow.ReloadDueDateCategory(lst_hanxuly);
            }

            this.NavigationController.PopViewController(true);
        }

        private void setlangTitle()
        {
            CmmIOSFunction.SetLangToView(this.View);
        }
        #endregion

        #region events
        private void BT_back_TouchUpInside(object sender, EventArgs e)
        {
            if (parentView.GetType() == typeof(ViewSearchV2))
            {

                ViewSearchV2 viewSearchV2 = parentView as ViewSearchV2;
                if (type == 0)
                    viewSearchV2.ReloadTrangThaiCategory(lst_trangthai);
                else if (type == 1)
                    viewSearchV2.ReloadTinhtrangCategory(lst_tinhtrang);
                else if (type == 2)
                    viewSearchV2.ReloadDueDateCategory(lst_hanxuly);
            }
            else if (parentView.GetType() == typeof(FormFillterToDoView))
            {
                FormFillterToDoView formFillterToDo = parentView as FormFillterToDoView;
                if (type == 0)
                    formFillterToDo.ReloadTrangThaiCategory(lst_trangthai);
                else if (type == 1)
                    formFillterToDo.ReloadTinhtrangCategory(lst_tinhtrang);
                else if (type == 2)
                    formFillterToDo.ReloadDueDateCategory(lst_hanxuly);
            }
            else if (parentView.GetType() == typeof(FormFillterWorkFlowView))
            {
                FormFillterWorkFlowView formFillterWorkFlowView = parentView as FormFillterWorkFlowView;
                // Toi bat dau - khong co trang thai
                if (type == 1)
                    formFillterWorkFlowView.ReloadTinhtrangCategory(lst_tinhtrang);
                else if (type == 2)
                    formFillterWorkFlowView.ReloadDueDateCategory(lst_hanxuly);
            }

            this.NavigationController.PopViewController(true);
        }
        #endregion

        #region custom class

        #region trangthai
        private class Trangthai_TableSource : UITableViewSource
        {
            List<ClassMenu> lst_items;
            NSString cellIdentifier = new NSString("cellMenuOption");
            FormListItemsChoiceV2 parentView;
            bool isTrangthai;

            public Trangthai_TableSource(List<ClassMenu> _menu, bool _isTrangthai, FormListItemsChoiceV2 _parentview)
            {
                parentView = _parentview;
                lst_items = _menu;
                isTrangthai = _isTrangthai;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return (lst_items != null) ? lst_items.Count : 0;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 40;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                var itemSelected = lst_items[indexPath.Row];
                if (isTrangthai)
                    parentView.Handle_Trangthai_SeclectItem(itemSelected, indexPath);
                else
                    parentView.Handle_Hanxuly_SeclectItem(itemSelected, indexPath);

                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var item = lst_items[indexPath.Row];
                ChoiceValue_cell_custom cell = new ChoiceValue_cell_custom(cellIdentifier);
                cell.UpdateCell(item);
                return cell;
            }
        }
        private class ChoiceValue_cell_custom : UITableViewCell
        {

            UILabel lbl_title;
            UILabel line;

            public ChoiceValue_cell_custom(NSString cellID) : base(UITableViewCellStyle.Default, cellID)
            {
                viewConfiguration();
            }
            private void viewConfiguration()
            {
                lbl_title = new UILabel()
                {
                    Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular),
                    TextColor = UIColor.FromRGB(51, 51, 51),
                    TextAlignment = UITextAlignment.Left,
                    BackgroundColor = UIColor.Clear
                };

                ContentView.AddSubviews(new UIView[] { lbl_title });
            }

            public void UpdateCell(ClassMenu _controlvalue)
            {
                if (_controlvalue.isSelected)
                    Accessory = UITableViewCellAccessory.Checkmark;
                else
                    Accessory = UITableViewCellAccessory.None;

                lbl_title.Text = _controlvalue.title;
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                lbl_title.Frame = new CGRect(25, 15, ContentView.Frame.Width - 50, 20);
            }
        }
        #endregion

        #region tinhtrang
        private class Tinhtrang_TableSource : UITableViewSource
        {
            List<BeanAppStatus> lst_controlvalue;
            NSString cellIdentifier = new NSString("cell");
            FormListItemsChoiceV2 parentView;
            nint docCount = 0;
            bool isAll;

            public Tinhtrang_TableSource(List<BeanAppStatus> _lst_controlvalue, FormListItemsChoiceV2 _parentview)
            {
                parentView = _parentview;

                if (_lst_controlvalue != null)
                {
                    lst_controlvalue = _lst_controlvalue;
                }
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return 1;
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                return 50;
            }

            public override UIView GetViewForHeader(UITableView tableView, nint section)
            {
                CGRect frame = new CGRect(0, 0, tableView.Frame.Width, 50);
                UIView uIView = new UIView(frame);
                uIView.BackgroundColor = UIColor.White;

                UIView line = new UIView();
                line.Frame = new CGRect(0, frame.Bottom - 1, frame.Width, 1);
                line.BackgroundColor = UIColor.FromRGB(246, 246, 246);

                UILabel lbl_title = new UILabel();
                lbl_title.Frame = new CGRect(25, 10, 200, 40);
                lbl_title.Font = UIFont.FromName("ArialMT", 15f);
                lbl_title.Text = CmmFunction.GetTitle("TEXT_ALL", "Tất cả");

                UIImageView img_checkIcon = new UIImageView();
                img_checkIcon.Frame = new CGRect(uIView.Frame.Width - 40, 15, 20, 20);
                img_checkIcon.Image = UIImage.GetSystemImage("checkmark");//.FromFile("Icons/icon_searchAccept.png");

                isAll = parentView.isAll;

                if (isAll)
                    img_checkIcon.Hidden = false;
                else
                    img_checkIcon.Hidden = true;

                UIButton button = new UIButton(frame);
                button.TouchUpInside += delegate
                {
                    parentView.isAll = true;

                    parentView.Handel_Tinhtrang_AllSelected();
                };

                uIView.AddSubviews(lbl_title, img_checkIcon, button, line);
                return uIView;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return lst_controlvalue.Count;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 50;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                var item = lst_controlvalue[indexPath.Row];
                parentView.Handle_Tinhtrang_SeclectItem(item, indexPath);
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                ControlValue_cell_custom cell = new ControlValue_cell_custom(cellIdentifier);
                var controlvalue = lst_controlvalue[indexPath.Row];
                cell.UpdateCell(controlvalue);
                return cell;
            }
        }
        private class ControlValue_cell_custom : UITableViewCell
        {

            UILabel lbl_title;
            UILabel line;

            public ControlValue_cell_custom(NSString cellID) : base(UITableViewCellStyle.Default, cellID)
            {
                viewConfiguration();
            }
            private void viewConfiguration()
            {
                lbl_title = new UILabel()
                {
                    Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular),
                    TextColor = UIColor.FromRGB(51, 51, 51),
                    TextAlignment = UITextAlignment.Left,
                    BackgroundColor = UIColor.Clear
                };

                ContentView.AddSubviews(new UIView[] { lbl_title });
            }
            public void UpdateCell(BeanAppStatus _controlvalue)
            {
                if (_controlvalue.IsSelected)
                    Accessory = UITableViewCellAccessory.Checkmark;
                else
                    Accessory = UITableViewCellAccessory.None;

                if (CmmVariable.SysConfig.LangCode == "1033")
                    lbl_title.Text = _controlvalue.TitleEN;
                else //if (CmmVariable.SysConfig.LangCode == "1066")
                    lbl_title.Text = _controlvalue.Title;
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                lbl_title.Frame = new CGRect(25, 15, ContentView.Frame.Width - 50, 20);
            }
        }

        #endregion
        #endregion




    }
}


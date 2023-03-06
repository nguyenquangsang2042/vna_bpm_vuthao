using System;
using System.Collections.Generic;
using System.Linq;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobileV1.iOS.IOSClass;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class ListUserViewMultiChoice : UIViewController
    {
        UIViewController parentView { get; set; }
        ViewElement element { get; set; }
        bool isMultiSelect;
        BeanDataLookup currentDataSelected { get; set; }


        public ListUserViewMultiChoice(IntPtr handle) : base(handle)
        {
        }

        #region override

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ViewConfiguration();
            loadContent();
            setlangTitle();

            #region delegate
            //BT_close.TouchUpInside += BT_close_TouchUpInside;
            //BT_accept.TouchUpInside += BT_accept_TouchUpInside;
            //searchBar_user.TextChanged += SearchBar_user_TextChanged;
            //searchBar_user.ShouldBeginEditing = (textfield) =>
            //{
            //    //if (!isSearch)
            //    //{
            //    //    isSearch = true;
            //    //    table_content.Hidden = false;
            //    //}
            //    return true;
            //};
            #endregion
        }

        
        #endregion

        #region private - public method
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_parentView">pqrent view</param>
        /// <param name="_type">0: multiChoice | 1: singleChoice</param>
        /// <param name="_lst_user_select"></param>
        /// <param name="_controlDynamic"></param>
        public void setContent(UIViewController _parentView, ViewElement _element, NSIndexPath _indexPath)
        {
            //if (_type == 1)
            //    isMultiSelect = false;
            //else
            //    isMultiSelect = true;

            parentView = _parentView;
            element = _element;
        }
        public void setContentFormShareView(UIViewController _parentView, bool _isMultiSelect, List<BeanUser> _lst_user_select)
        {
            isMultiSelect = _isMultiSelect;
            //lst_user_select = _lst_user_select;
            parentView = _parentView;
            //isRequestAddInfoAction = _isRequestAddInfoAction;
        }

        private void ViewConfiguration()
        {
            //searchBar_user.Placeholder = CmmFunction.GetTitle("K_Search", "Tìm kiếm…");
            //table_content.ContentInset = new UIEdgeInsets(-20, 0, 0, 0);
        }

        private void loadContent()
        {
            if (parentView.GetType() == typeof(FormShareView))
            {
                //lbl_title.Text = "Chọn người được chia sẻ";
                //searchBar_user.Placeholder = "Vui lòng nhập tên hoặc email...";
            }

            var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);

            string query = string.Format("SELECT * FROM BeanUser Order BY FullName");
            //lst_user = conn.Query<BeanUser>(query);

            //if (lst_user_select != null && lst_user_select.Count > 0)
            //{
            //    foreach (var item in lst_user_select)
            //    {
            //        var obj = lst_user.FirstOrDefault(i => i.UserId == item.UserId);
            //        if (obj != null)
            //            obj.IsSelected = true;
            //    }

            //    table_content.Source = new users_TableSource(lst_user, this);
            //    table_content.ReloadData();
            //}
            //else
            //{
            //    if (lst_user != null && lst_user.Count > 0)
            //    {
            //        table_content.Source = new users_TableSource(lst_user, this);
            //        table_content.ReloadData();
            //    }
            //}
        }

        private void setlangTitle()
        {
            CmmIOSFunction.SetLangToView(this.View);
        }

        //public void selectedUser(BeanUser _user)
        //{
        //    this.View.EndEditing(true);
        //    isSearch = false;
        //    string usertype_lookup = _user.UserId + ";#" + _user.Name;
        //    if (viewRequestDetails != null)
        //        viewRequestDetails.updateDictValue(controlDynamic.DataField, usertype_lookup);
        //    else if (changeUserProgress != null)
        //        changeUserProgress.selectedUser(userSelected);
        //    this.DismissViewControllerAsync(true);
        //}

        public void HandleSeclectItem(BeanUser _user, NSIndexPath _indexPath)
        {
            if (!isMultiSelect)
            {
                //if (currentUserSelected != null && currentUserSelected.ID != _user.ID)
                //    currentUserSelected.IsSelected = false;

                //currentUserSelected = _user;

                //this.View.EndEditing(true);
                //isSearch = false;
                ////string usertype_lookup = _user.ID + ";#" + _user.Name;
                //string usertype_lookup = _user.AccountName;

                //if (parentView.GetType() == typeof(ViewRequestDetails))
                //{
                //    ViewRequestDetails viewRequestDetails = (ViewRequestDetails)parentView;
                //    viewRequestDetails.updateDictValue(controlDynamic.DataField, usertype_lookup);

                //}
                //else if (parentView.GetType() == typeof(ChangeUserProgress))
                //{
                //    ChangeUserProgress changeUserProgress = (ChangeUserProgress)parentView;
                //    changeUserProgress.selectedUser(currentUserSelected);
                //}
                //else if (parentView.GetType() == typeof(RequestAddInfo))
                //{
                //    RequestAddInfo requestAddInfo = (RequestAddInfo)parentView;
                //    requestAddInfo.selectedUser(currentUserSelected);
                //}


                this.DismissViewControllerAsync(true);
            }

            _user.IsSelected = _user.IsSelected.HasValue && _user.IsSelected.Value ? !_user.IsSelected : false;
            //table_content.ReloadData();
        }

        #endregion

        #region events
        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            this.NavigationController.PopViewController(true);
        }
        private void BT_accept_TouchUpInside(object sender, EventArgs e)
        {
            if (parentView.GetType() == typeof(FormShareView))
            {
                //if (lst_user_select == null)
                //    lst_user_select = new List<BeanUser>();

                //FormShareView formShareView = parentView as FormShareView;
                //foreach (var item in lst_user)
                //{
                //    if (item.IsSelected)
                //        lst_user_select.Add(item);
                //}
                //formShareView.LoadContent(lst_user_select);

                this.NavigationController.PopViewController(true);
            }
        }
        private void SearchBar_user_TextChanged(object sender, UISearchBarTextChangedEventArgs e)
        {
            try
            {
                //string content = CmmFunction.removeSignVietnamese(searchBar_user.Text.Trim().ToLowerInvariant());
                //if (!string.IsNullOrEmpty(content))
                //{
                    //var items = from item in lst_user
                    //            where ((!string.IsNullOrEmpty(item.Name) && CmmFunction.removeSignVietnamese(item.Name.ToLowerInvariant()).Contains(content)) ||
                    //                       (!string.IsNullOrEmpty(item.Email) && item.Email.ToLowerInvariant().Contains(content)))
                    //            orderby item.Name
                    //            select item;

                    //if (items != null && items.Count() > 0)
                    //{
                    //    lst_contact_result = items.ToList();
                    //    table_content.Alpha = 1;
                    //    table_content.Source = new users_TableSource(lst_contact_result, this);
                    //    table_content.ReloadData();
                    //}
                    //else
                    //    table_content.Alpha = 0;
                //}
                //else
                //{
                //    //table_content.Alpha = 1;
                //    //table_content.Source = new users_TableSource(lst_user, this);
                //    //table_content.ReloadData();
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine("AssignedToView - SearchBar_user_TextChanged - Err: " + ex.ToString());
            }
        }
        #endregion

        #region custom class

        #region table data source user
        private class users_TableSource : UITableViewSource
        {
            List<BeanUser> lst_user;
            NSString cellIdentifier = new NSString("cell");
            ListUserViewMultiChoice parentView;

            public users_TableSource(List<BeanUser> _user, ListUserViewMultiChoice _parentview)
            {
                parentView = _parentview;
                if (_user != null)
                {
                    lst_user = _user;
                }
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return lst_user.Count;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 60;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                var item = lst_user[indexPath.Row];
                parentView.HandleSeclectItem(item, indexPath);
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                userTCT_cell_custom cell = new userTCT_cell_custom(cellIdentifier);
                var user = lst_user[indexPath.Row];

                cell.UpdateCell(user);
                return cell;
            }
        }
        private class userTCT_cell_custom : UITableViewCell
        {

            UILabel lbl_imgCover, lbl_name, lbl_email;
            UILabel line;
            private Random rnd = new Random();

            public userTCT_cell_custom(NSString cellID) : base(UITableViewCellStyle.Default, cellID)
            {
                viewConfiguration();
            }
            private void viewConfiguration()
            {
                lbl_imgCover = new UILabel
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
                    BackgroundColor = UIColor.Blue,
                    TextColor = UIColor.White
                };

                lbl_imgCover.Layer.CornerRadius = 15;
                lbl_imgCover.ClipsToBounds = true;
                var color = UIColor.FromRGB(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                lbl_imgCover.BackgroundColor = color;

                lbl_name = new UILabel()
                {
                    Font = UIFont.SystemFontOfSize(12, UIFontWeight.Semibold),
                    TextColor = UIColor.FromRGB(51, 51, 51),
                    TextAlignment = UITextAlignment.Left,
                    BackgroundColor = UIColor.Clear
                };

                lbl_email = new UILabel()
                {
                    Font = UIFont.SystemFontOfSize(12, UIFontWeight.Light),
                    TextColor = UIColor.FromRGB(51, 51, 51),
                    TextAlignment = UITextAlignment.Left,
                    BackgroundColor = UIColor.Clear
                };

                line = new UILabel()
                {
                    BackgroundColor = UIColor.LightGray.ColorWithAlpha(0.5f)
                };

                ContentView.AddSubviews(new UIView[] { lbl_imgCover, lbl_name, lbl_email, line });
            }

            public void UpdateCell(BeanUser user)
            {
                if (user.IsSelected.HasValue && user.IsSelected.Value)
                    Accessory = UITableViewCellAccessory.Checkmark;
                else
                    Accessory = UITableViewCellAccessory.None;

                if (!string.IsNullOrEmpty(user.Name))
                {
                    lbl_imgCover.Hidden = false;
                    //iv_statusAvatar.Hidden = true;
                    lbl_imgCover.Text = CmmFunction.GetAvatarName(user.Name);
                    lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));
                }
                else
                {
                    lbl_imgCover.Hidden = false;
                }

                lbl_name.Text = user.Name;
                lbl_email.Text = user.Email;
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                lbl_imgCover.Frame = new CGRect(10, 15, 30, 30);
                lbl_name.Frame = new CGRect(lbl_imgCover.Frame.Right + 5, 8, this.ContentView.Frame.Width - 80, 25);
                lbl_email.Frame = new CGRect(lbl_imgCover.Frame.Right + 5, lbl_name.Frame.Bottom, 400, 20);
                line.Frame = new CGRect(lbl_name.Frame.X, ContentView.Frame.Bottom - 0.5, ContentView.Frame.Width - lbl_name.Frame.X, 0.5);
            }
        }

        #endregion

        #endregion


    }
}


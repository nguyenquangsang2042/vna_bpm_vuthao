using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CoreGraphics;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobileV1.iOS.IOSClass;
using Foundation;
using UIKit;
using BPMOPMobileV1.iOS.CustomControlClass;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;
using BPMOPMobile.DataProvider;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class ListUserView : UIViewController
    {
        ViewElement element { get; set; }
        bool isLoadData;
        int type = 0;
        User_CollectionSource user_CollectionSource;
        bool isSearch = false;
        bool isMultiSelect = true;
        bool isUserAndGroup = true;
        BeanUser currentUserSelected { get; set; }
        List<BeanUser> lst_user = new List<BeanUser>();
        List<BeanUser> lst_user_select = new List<BeanUser>();
        List<BeanUser> lst_contact_result = new List<BeanUser>();
        bool isRequestAddInfoAction { get; set; }
        UIViewController parentView { get; set; }
        string title;
        nfloat width = 0;
        int lines = 1;
        private UITapGestureRecognizer gestureRecognizer;

        public ListUserView(IntPtr handle) : base(handle)
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
            gestureRecognizer = new UITapGestureRecognizer(Self, new ObjCRuntime.Selector("hideKeyboard"));
            gestureRecognizer.ShouldReceiveTouch += delegate (UIGestureRecognizer recognizer, UITouch touch)
            {
                var name = touch.View.Class.Name;
                var touchName = touch.View.Superview.Superview.Class.Name;

                if (name == "UITableViewCellContentView")
                    return false;
                else
                    return true;
            };
            this.View.AddGestureRecognizer(gestureRecognizer);

            ViewConfiguration();
            if (isRequestAddInfoAction)
            {
                loadContentFromRequestAddInfo();
                BT_clear.Hidden = true;
            }
            else
                loadContent();

            setlangTitle();

            #region delegate
            BT_clear.TouchUpInside += BT_clear_TouchUpInside;
            BT_close.TouchUpInside += BT_close_TouchUpInside;
            BT_accept.TouchUpInside += BT_accept_TouchUpInside;
            tf_search.EditingChanged += Tf_search_EditingChanged;
            tf_search.ShouldBeginEditing = (textfield) =>
            {
                if (!isSearch)
                {
                    isSearch = true;
                    table_content.Hidden = false;
                }
                return true;
            };
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
        public void setContent(UIViewController _parentView, bool _isMultiSelect, List<BeanUser> _lst_user_select, bool _isRequestAddInfoAction, ViewElement _element, string _title)
        {
            isMultiSelect = _isMultiSelect;
            lst_user_select = _lst_user_select;
            parentView = _parentView;
            isRequestAddInfoAction = _isRequestAddInfoAction;
            element = _element;
            title = _title;
        }

        private void ViewConfiguration()
        {
            headerView_constantHeight.Constant = 45 + CmmIOSFunction.GetHeaderViewHeight();

            BT_clear.ImageEdgeInsets = new UIEdgeInsets(3, 3, 3, 3);
            tf_search.Placeholder = CmmFunction.GetTitle("TEXT_HINT_USER_EMAIL", "Vui lòng nhập tên hoặc địa chỉ email...");
            tf_search.Font = UIFont.FromName("Arial-ItalicMT", 14);
            //if (parentView.GetType() == typeof(FormShareView))
            //    BT_close.SetImage(UIImage.FromFile("Icons/icon_closeX.png"), UIControlState.Normal);

            if (!isMultiSelect)
            {
                BT_accept.Hidden = true;
                Constraint_rightBTClear.Constant = 0;
            }

            collectionUser_heightConstant.Constant = 0;
            User_CollectionView.RegisterClassForCell(typeof(User_CollectionCell), User_CollectionCell.Key);

            if (isRequestAddInfoAction)
                selectedUserView_heightConstant.Constant = 40;

            var layoutStyle = new CollectionViewLayoutStyle();
            User_CollectionView.SetCollectionViewLayout(layoutStyle, true);
            User_CollectionView.AllowsMultipleSelection = false;

            table_content.ContentInset = new UIEdgeInsets(-20, 0, 0, 0);
            tf_search.BecomeFirstResponder();
        }

        private void loadContent()
        {
            lbl_title.Text = title;

            if (element != null)
            {
                if (!string.IsNullOrEmpty(element.Value))
                    lst_user_select = JsonConvert.DeserializeObject<List<BeanUser>>(element.Value);

                if (element.DataType == "selectuser")
                {
                    if (parentView.GetType() == typeof(RequestDetailsV2) || parentView.GetType() == typeof(FormWFDetailsProperty)) // truong hop chi tiet phieu thi cho clear selectuser
                    {
                        if (lst_user_select != null)
                        {
                            foreach (var item in lst_user_select)
                            {
                                if (!string.IsNullOrEmpty(item.Name))
                                    item.FullName = item.Name;
                            }

                            lines = CalculatorCollectionLine(false);
                            collectionUser_heightConstant.Constant = lines * 35;
                            selectedUserView_heightConstant.Constant = collectionUser_heightConstant.Constant + 50;

                            user_CollectionSource = new User_CollectionSource(this, lst_user_select, null);
                            User_CollectionView.Source = user_CollectionSource;
                            User_CollectionView.Delegate = new User_CollectionFlowLayoutDelegate(user_CollectionSource, this);
                            User_CollectionView.ReloadData();
                        }
                    }
                    else
                    {
                        collectionUser_heightConstant.Constant = 0;
                        selectedUserView_heightConstant.Constant = collectionUser_heightConstant.Constant + 50;
                        BT_clear.Hidden = true;
                    }
                }
                else if (element.DataType == "selectusermulti")
                {
                    if (lst_user_select != null)
                    {
                        foreach (var item in lst_user_select)
                        {
                            if (!string.IsNullOrEmpty(item.Name))
                                item.FullName = item.Name;
                        }

                        lines = CalculatorCollectionLine(false);
                        collectionUser_heightConstant.Constant = lines * 35;
                        selectedUserView_heightConstant.Constant = collectionUser_heightConstant.Constant + 50;

                        user_CollectionSource = new User_CollectionSource(this, lst_user_select, null);
                        User_CollectionView.Source = user_CollectionSource;
                        User_CollectionView.Delegate = new User_CollectionFlowLayoutDelegate(user_CollectionSource, this);
                        User_CollectionView.ReloadData();
                    }
                }
            }
            else
            {
                if (parentView.GetType() != typeof(RequestDetailsV2) && parentView.GetType() != typeof(FormWFDetailsProperty)) // truong hop chi tiet phieu thi cho clear selectuser
                {
                    BT_clear.Hidden = true;
                }
                collectionUser_heightConstant.Constant = 0;
                selectedUserView_heightConstant.Constant = collectionUser_heightConstant.Constant + 50;
                if (lst_user_select != null)
                {
                    //foreach (var item in lst_user_select)
                    //{
                    //    item.FullName = item.Name;
                    //}

                    lines = CalculatorCollectionLine(false);
                    collectionUser_heightConstant.Constant = lines * 35;
                    selectedUserView_heightConstant.Constant = collectionUser_heightConstant.Constant + 50;

                    user_CollectionSource = new User_CollectionSource(this, lst_user_select, null);
                    User_CollectionView.Source = user_CollectionSource;
                    User_CollectionView.Delegate = new User_CollectionFlowLayoutDelegate(user_CollectionSource, this);
                    User_CollectionView.ReloadData();
                }
            }
        }

        private void loadContentFromRequestAddInfo()
        {
            lbl_title.Text = title;

            RequestAddInfo requestAddInfo = parentView as RequestAddInfo;
            lst_user = requestAddInfo.lst_userInWorkFlow;

            if (lst_user_select != null && lst_user_select.Count > 0)
            {
                foreach (var item in lst_user)
                {
                    var obj = lst_user_select.FirstOrDefault(i => i.ID == item.ID);
                    if (obj != null)
                        item.IsSelected = true;
                    else
                        item.IsSelected = false;
                }

                table_content.Source = new users_TableSource(lst_user, this);
                table_content.ReloadData();
            }
            else
            {
                if (lst_user != null && lst_user.Count > 0)
                {
                    table_content.Source = new users_TableSource(lst_user, this);
                    table_content.ReloadData();
                }
            }
        }

        private void setlangTitle()
        {
            CmmIOSFunction.SetLangToView(this.View);
        }
        [Export("hideKeyboard")]
        private void hideKeyboard()
        {
            this.View.EndEditing(true);
        }

        public void RemoveUserFromColletionView(BeanUser user)
        {
            lst_user_select.Remove(user);
            lines = CalculatorCollectionLine(true);
            user_CollectionSource.items.Remove(user);

            if (lst_user_select != null && lst_user_select.Count == 0)
            {
                collectionUser_heightConstant.Constant = 0 * 30;
                selectedUserView_heightConstant.Constant = collectionUser_heightConstant.Constant + 50;
            }
            else
            {
                collectionUser_heightConstant.Constant = lines * 30;
                selectedUserView_heightConstant.Constant = (lines * 30) + 50;
            }

            User_CollectionView.Source = user_CollectionSource;
            User_CollectionView.Delegate = new User_CollectionFlowLayoutDelegate(user_CollectionSource, this);
            User_CollectionView.ReloadData();
            if (element.DataType == "selectuser") // thoat ra 
            {
                bool isExit = false;
                if (parentView.GetType() == typeof(RequestDetailsV2)) // truong hop chi tiet phieu thi cho clear selectuser
                {
                    RequestDetailsV2 requestDetailsV2 = (RequestDetailsV2)parentView;
                    requestDetailsV2.HandleUserSingleChoiceSelected(element, null);
                    isExit = true;

                }
                if (parentView.GetType() == typeof(FormWFDetailsProperty)) // truong hop master detail chi tiet phieu thi cho clear selectuser
                {
                    FormWFDetailsProperty formWFDetailsProperty = (FormWFDetailsProperty)parentView;
                    formWFDetailsProperty.HandleUserSingleChoiceSelected(element, null);
                    isExit = true;
                }
                if (isExit)
                {
                    if (this.NavigationController != null)
                        this.NavigationController.PopViewController(true);
                    else
                        this.DismissViewControllerAsync(true);
                }
            }
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
                if (currentUserSelected != null && currentUserSelected.ID != _user.ID)
                    currentUserSelected.IsSelected = false;

                currentUserSelected = _user;
                currentUserSelected.Name = currentUserSelected.FullName;

                this.View.EndEditing(true);
                isSearch = false;
                //string usertype_lookup = _user.ID + ";#" + _user.Name;
                string usertype_lookup = _user.AccountName;

                if (parentView.GetType() == typeof(RequestDetailsV2))
                {
                    RequestDetailsV2 requestDetailsV2 = (RequestDetailsV2)parentView;
                    requestDetailsV2.HandleUserSingleChoiceSelected(element, currentUserSelected);
                }
                else if (parentView.GetType() == typeof(ChangeUserProgress))
                {
                    ChangeUserProgress changeUserProgress = (ChangeUserProgress)parentView;
                    changeUserProgress.selectedUser(currentUserSelected);
                }
                else if (parentView.GetType() == typeof(RequestAddInfo))
                {
                    RequestAddInfo requestAddInfo = (RequestAddInfo)parentView;
                    requestAddInfo.selectedUser(currentUserSelected);
                }
                else if (parentView.GetType() == typeof(FormWFDetailsProperty))
                {
                    FormWFDetailsProperty formWFDetailsProperty = (FormWFDetailsProperty)parentView;
                    formWFDetailsProperty.HandleUserSingleChoiceSelected(element, currentUserSelected);
                }

                if (this.NavigationController != null)
                    this.NavigationController.PopViewController(true);
                else
                    this.DismissViewControllerAsync(true);
            }
            else
            {
                if (lst_user_select == null)
                    lst_user_select = new List<BeanUser>();

                tf_search.Text = "";
                table_content.Alpha = 0;

                lst_contact_result.Remove(_user);
                _user.IsSelected = _user.IsSelected.HasValue && _user.IsSelected.Value ? !_user.IsSelected : false;
                table_content.ReloadData();

                lst_user_select.Add(_user);
                lines = CalculatorCollectionLine(false);
                //nfloat height = 0;
                //width = width + _user.FullName.StringSize(UIFont.SystemFontOfSize(13)).Width + 5 + 25;
                //foreach (var item in lst_user_select)
                //{
                //    if (width > User_CollectionView.Bounds.Width)
                //    {
                //        width = 0;
                //        lines++;
                //    }
                //}

                //if (lines > 3)
                //    lines = 3;

                collectionUser_heightConstant.Constant = lines * 35;
                selectedUserView_heightConstant.Constant = collectionUser_heightConstant.Constant + 50;

                user_CollectionSource = new User_CollectionSource(this, lst_user_select, null);
                User_CollectionView.Source = user_CollectionSource;
                User_CollectionView.Delegate = new User_CollectionFlowLayoutDelegate(user_CollectionSource, this);
                User_CollectionView.ReloadData();
            }


        }

        private int CalculatorCollectionLine(bool _isRemove)
        {
            width = 0;
            //if (_isRemove)
            lines = 0;

            foreach (var item in lst_user_select)
            {
                width = width + item.FullName.StringSize(UIFont.SystemFontOfSize(13)).Width + 5 + 30 + 20;
                //if (width > User_CollectionView.Bounds.Width)
                //{
                //    lines = (int)Math.Ceiling(width / User_CollectionView.Bounds.Width);
                //    //lines++;
                //}
            }

            nfloat numberLines = width / User_CollectionView.Bounds.Width;

            if (numberLines > 0 && numberLines < 1)
                lines = 1;
            else if (numberLines > 1 && numberLines < 2)
                lines = 2;
            else if (numberLines > 2)
                lines = 3;

            return lines;

        }
        #endregion

        #region events
        private void BT_clear_TouchUpInside(object sender, EventArgs e)
        {
            //lst_user_select = new List<BeanUser>();
            //lines = CalculatorCollectionLine(true);

            //collectionUser_heightConstant.Constant = lines * 30;
            //selectedUserView_heightConstant.Constant = (lines * 30) + 50;

            //user_CollectionSource = new User_CollectionSource(this, lst_user_select, null);
            //User_CollectionView.Source = user_CollectionSource;
            //User_CollectionView.Delegate = new User_CollectionFlowLayoutDelegate(user_CollectionSource, this);
            //User_CollectionView.ReloadData();
            lst_user_select = new List<BeanUser>();
            AcctionDone();

        }
        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            if (this.NavigationController != null)
                this.NavigationController.PopViewController(true);
            else
                this.DismissViewControllerAsync(true);
        }

        private void Tf_search_EditingChanged(object sender, EventArgs e)
        {
            try
            {
                string content = CmmFunction.removeSignVietnamese(tf_search.Text.Trim().ToLowerInvariant());

                if (isRequestAddInfoAction)
                {
                    var items = from item in lst_user
                                where (!string.IsNullOrEmpty(item.FullName) && CmmFunction.removeSignVietnamese(item.FullName.ToLowerInvariant()).Contains(content)) ||
                                      (!string.IsNullOrEmpty(item.Email) && item.Email.ToLowerInvariant().Contains(content)) ||
                                      (!string.IsNullOrEmpty(item.AccountName) && CmmFunction.removeSignVietnamese(item.AccountName.ToLowerInvariant()).Contains(content))
                                select item;

                    if (items != null && items.Count() > 0)
                    {
                        lst_contact_result = items.ToList();
                        table_content.Alpha = 1;
                        table_content.Source = new users_TableSource(lst_contact_result, this);
                        table_content.ReloadData();
                    }
                    else
                    {
                        lst_contact_result = new List<BeanUser>();
                        table_content.Alpha = 0;
                        table_content.Source = new users_TableSource(lst_contact_result, this);
                        table_content.ReloadData();
                    }
                }
                else
                {
                    var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath, false);

                    if (!string.IsNullOrEmpty(content))
                    {
                        List<string> lst_UserSelectedID = new List<string>();
                        string str_user_selected = "";
                        if (lst_user_select != null && lst_user_select.Count > 0)
                        {
                            foreach (var user in lst_user_select)
                            {
                                lst_UserSelectedID.Add(user.ID);
                            }
                        }
                        str_user_selected = "'" + String.Join("','", lst_UserSelectedID.ToArray()) + "'";

                        string query = string.Format("SELECT * FROM BeanUser WHERE (FullName LIKE '%{0}%' OR Email LIKE '%{1}%' OR AccountName LIKE '%{2}%') AND ID NOT IN ({3})", content, content, content, str_user_selected);
                        lst_contact_result = conn.Query<BeanUser>(query);

                        if (lst_contact_result != null && lst_contact_result.Count() > 0)
                        {
                            table_content.Alpha = 1;
                            table_content.Source = new users_TableSource(lst_contact_result, this);
                            table_content.ReloadData();
                        }
                        else
                            table_content.Alpha = 0;
                    }
                    else
                    {
                        table_content.Alpha = 1;
                        table_content.Source = new users_TableSource(lst_user, this);
                        table_content.ReloadData();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("AssignedToView - SearchBar_user_TextChanged - Err: " + ex.ToString());
            }
        }

        private void BT_accept_TouchUpInside(object sender, EventArgs e)
        {
            AcctionDone();
        }

        private void AcctionDone()
        {
            if (parentView.GetType() == typeof(FormShareView))
            {
                //FormShareView formShareView = parentView as FormShareView;
                //formShareView.UpdateMultiUserColletionView(lst_user_select, lines);

                //if (this.NavigationController != null)
                //    this.NavigationController.PopViewController(true);
                //else
                //    this.DismissModalViewController(true);
            }
            else if (parentView.GetType() == typeof(RequestDetailsV2))
            {
                RequestDetailsV2 formShareView = parentView as RequestDetailsV2;
                formShareView.HandleUserMultiChoiceSelected(element, lst_user_select);

                if (this.NavigationController != null)
                    this.NavigationController.PopViewController(true);
                else
                    this.DismissModalViewController(true);
            }
            else if (parentView.GetType() == typeof(FormWFDetailsProperty))
            {
                FormWFDetailsProperty formWFDetailsProperty = parentView as FormWFDetailsProperty;
                formWFDetailsProperty.HandleUserMultiChoiceSelected(element, lst_user_select);

                if (this.NavigationController != null)
                    this.NavigationController.PopViewController(true);
                else
                    this.DismissModalViewController(true);
            }
        }
        #endregion

        #region custom class

        #region table data source user
        private class users_TableSource : UITableViewSource
        {
            List<BeanUser> lst_user;
            NSString cellIdentifier = new NSString("cell");
            ListUserView parentView;

            public users_TableSource(List<BeanUser> _user, ListUserView _parentview)
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
                user_cell_custom cell = new user_cell_custom(cellIdentifier);
                var user = lst_user[indexPath.Row];

                cell.UpdateCell(user);
                return cell;
            }
        }
        private class user_cell_custom : UITableViewCell
        {
            string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            UIImageView imgAvatar;
            UILabel lbl_imgCover, lbl_name, lbl_email;
            UILabel line;

            public user_cell_custom(NSString cellID) : base(UITableViewCellStyle.Default, cellID)
            {
                viewConfiguration();
            }
            private void viewConfiguration()
            {

                imgAvatar = new UIImageView();
                imgAvatar.ContentMode = UIViewContentMode.ScaleAspectFill;
                imgAvatar.Layer.CornerRadius = 18;
                imgAvatar.ClipsToBounds = true;
                imgAvatar.Hidden = true;

                lbl_imgCover = new UILabel
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
                    BackgroundColor = UIColor.Blue,
                    TextColor = UIColor.White
                };

                lbl_imgCover.Layer.CornerRadius = 18;
                lbl_imgCover.ClipsToBounds = true;

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

                ContentView.AddSubviews(new UIView[] { imgAvatar, lbl_imgCover, lbl_name, lbl_email, line });
            }

            public void UpdateCell(BeanUser user)
            {
                if (user.IsSelected.HasValue && user.IsSelected.Value)
                    Accessory = UITableViewCellAccessory.Checkmark;
                else
                    Accessory = UITableViewCellAccessory.None;

                if (string.IsNullOrEmpty(user.ImagePath))
                {
                    if (!string.IsNullOrEmpty(user.FullName))
                    {
                        lbl_imgCover.Hidden = false;
                        imgAvatar.Hidden = true;
                        lbl_imgCover.Text = CmmFunction.GetAvatarName(user.FullName);
                        lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));
                    }
                }
                else
                {
                    checkFileLocalIsExist(user, imgAvatar);
                    lbl_imgCover.Hidden = true;
                }

                lbl_name.Text = user.FullName;
                lbl_email.Text = user.Email;
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                imgAvatar.Frame = new CGRect(10, 15, 36, 36);
                lbl_imgCover.Frame = new CGRect(10, 15, 36, 36);
                lbl_name.Frame = new CGRect(lbl_imgCover.Frame.Right + 5, 8, this.ContentView.Frame.Width - 80, 25);
                lbl_email.Frame = new CGRect(lbl_imgCover.Frame.Right + 5, lbl_name.Frame.Bottom, 400, 20);
                line.Frame = new CGRect(lbl_name.Frame.X, ContentView.Frame.Bottom - 0.5, ContentView.Frame.Width - lbl_name.Frame.X, 0.5);
            }

            private async void checkFileLocalIsExist(BeanUser contact, UIImageView image_view)
            {
                try
                {
                    string filename = contact.ImagePath.Split('/').Last();
                    string filepathURL = CmmVariable.M_Domain + "/" + CmmVariable.SysConfig.Subsite + contact.ImagePath;
                    string localfilePath = Path.Combine(localDocumentFilepath, filename);

                    if (!File.Exists(localfilePath))
                    {
                        UIImage avatar = null;
                        await Task.Run(() =>
                        {
                            ProviderBase provider = new ProviderBase();
                            if (provider.DownloadFile(filepathURL, localfilePath, CmmVariable.M_AuthenticatedHttpClient))
                            {
                                NSData data = NSData.FromUrl(new NSUrl(localfilePath, false));

                                InvokeOnMainThread(() =>
                                {
                                    if (data != null)
                                    {
                                        UIImage image = UIImage.LoadFromData(data);
                                        if (image != null)
                                        {
                                            avatar = CmmIOSFunction.MaxResizeImage(image, 200, 200);
                                            image_view.Image = avatar;
                                        }
                                        else
                                            image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                                    }
                                    else
                                        image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");

                                    imgAvatar.Hidden = false;
                                });

                                if (data != null && avatar != null)
                                {
                                    NSError err = null;
                                    NSData imgData = avatar.AsPNG();
                                    if (imgData.Save(localfilePath, false, out err))
                                        Console.WriteLine("saved as " + localfilePath);
                                    return;
                                }
                            }
                            else
                            {
                                InvokeOnMainThread(() =>
                                {
                                    image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                                    imgAvatar.Hidden = false;
                                });
                            }
                        });
                    }
                    else
                    {
                        openFile(filename, image_view);
                        imgAvatar.Hidden = false;
                    }
                }
                catch (Exception ex)
                {
                    image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                    Console.WriteLine("ListUserView - checkFileLocalIsExist - Err: " + ex.ToString());
                    //CmmIOSFunction.IOSlog(null, "PopupContactDetailView - loadAvatar - " + ex.ToString());
                }
            }

            private async void openFile(string localfilename, UIImageView image_view)
            {
                try
                {
                    NSData data = null;
                    await Task.Run(() =>
                    {
                        string localfilePath = Path.Combine(localDocumentFilepath, localfilename);
                        data = NSData.FromUrl(new NSUrl(localfilePath, false));
                    });

                    if (data != null)
                    {
                        UIImage image = UIImage.LoadFromData(data);
                        if (image != null)
                        {
                            image_view.Image = image;
                        }
                        else
                        {
                            image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                        }
                    }
                    else
                    {
                        image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("ContactView - openFile - Err: " + ex.ToString());
                }
            }

            private int HighlightKeySearch(string _keyWord, BeanUser contact)
            {
                var indexA = contact.FullName.IndexOf(_keyWord);

                return indexA;
            }
        }

        #endregion

        #region custom collection
        public class User_CollectionSource : UICollectionViewSource
        {
            ListUserView parentView { get; set; }
            public static Dictionary<string, List<BeanUser>> indexedSession;
            public List<BeanUser> items;

            public User_CollectionSource(ListUserView _parentview, List<BeanUser> _items, List<KeyValuePair<string, bool>> _sectionState)
            {
                parentView = _parentview;
                items = _items;
                LoadData();
            }

            public void LoadData()
            {


            }

            public override nint NumberOfSections(UICollectionView collectionView)
            {
                return 1;
            }
            public override nint GetItemsCount(UICollectionView collectionView, nint section)
            {
                return items.Count;
            }
            public override bool ShouldHighlightItem(UICollectionView collectionView, NSIndexPath indexPath)
            {
                return true;
            }
            public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
            {
                //parentView.NavigateToViewByCate(items[indexPath.Row], indexPath.Row);
            }
            public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
            {
                User_CollectionCell userCell = (User_CollectionCell)collectionView.DequeueReusableCell(User_CollectionCell.Key, indexPath);
                userCell.UpdateRow(items[indexPath.Row], parentView);
                return userCell;
            }
        }

        private class User_CollectionFlowLayoutDelegate : UICollectionViewDelegateFlowLayout
        {
            ListUserView parent;
            User_CollectionSource collectionSource_user;
            nfloat titleWidth = 0, height = 0;
            BeanUser user { get; set; }

            #region Constructors
            public User_CollectionFlowLayoutDelegate(User_CollectionSource _lst_user, ListUserView _parent)
            {
                collectionSource_user = _lst_user;
                parent = _parent;
            }
            #endregion

            #region Override Methods

            public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
            {
                user = collectionSource_user.items[indexPath.Row];
                string title = user.FullName;

                var widthStatus = StringExtensions.MeasureString(title, 13).Width + 5;
                var maxStatusWidth = parent.View.Frame.Width - 180;
                var width = title.StringSize(UIFont.SystemFontOfSize(13)).Width + 5;
                //if (widthStatus < maxStatusWidth)

                return new CGSize(width + 25, 25);
            }

            public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
            {
                if (!parent.isLoadData)
                {
                    var cateSelected = collectionSource_user.items[indexPath.Row];

                    //parent.isLoadData = true;
                    var cell = collectionView.CellForItem(indexPath);
                    cell.Selected = true;
                    //parent.nameCategoryItemSelected = cateSelected.ID.ToString();//cateSelected.Title;

                    collectionView.ReloadData();

                    parent.isLoadData = true;
                    //var cateSelected = lst_cate.lstCategory[indexPath.Row];
                    //parent.filterCateSelected(cateSelected, indexPath, isSubcate);
                }
            }
            #endregion
        }

        private class CollectionViewLayoutStyle : UICollectionViewFlowLayout
        {
            public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(CoreGraphics.CGRect rect)
            {

                var attributes = base.LayoutAttributesForElementsInRect(rect);

                if (attributes != null && attributes.Length > 0)
                {
                    //Add these lines to change the first cell's position of the collection view.
                    var firstCellFrame = attributes[0].Frame;
                    firstCellFrame.X = 0;
                    attributes[0].Frame = firstCellFrame;
                }

                for (var i = 1; i < attributes.Length; ++i)
                {
                    var currentLayoutAttributes = attributes[i];
                    var previousLayoutAttributes = attributes[i - 1];
                    var maximumSpacing = MinimumInteritemSpacing;
                    //var maximumSpacing = 5;
                    var previousLayoutEndPoint = previousLayoutAttributes.Frame.Right;

                    if (previousLayoutEndPoint + maximumSpacing + currentLayoutAttributes.Frame.Size.Width >= CollectionViewContentSize.Width)
                    {
                        var firstCellofRow = attributes[i].Frame;
                        firstCellofRow.X = 0;
                        attributes[i].Frame = firstCellofRow;
                        continue;
                    }
                    var frame = currentLayoutAttributes.Frame;
                    frame.X = previousLayoutEndPoint + maximumSpacing;
                    currentLayoutAttributes.Frame = frame;
                }
                return attributes;
            }
        }

        #endregion

        #endregion


    }
}


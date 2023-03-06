using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobileV1.iOS.CustomControlClass;
using BPMOPMobileV1.iOS.IOSClass;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json;
using UIKit;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class ListUserOrGroupView : UIViewController
    {
        ViewElement element { get; set; }
        bool isLoadData;
        int type = 0;
        User_CollectionSource user_CollectionSource;
        bool isSearch = false;
        bool isMultiSelect = true;
        bool isUserAndGroup = true;
        BeanUserAndGroup currentUserSelected { get; set; }
        List<BeanUserAndGroup> lst_user = new List<BeanUserAndGroup>();
        List<BeanUserAndGroup> lst_user_select = new List<BeanUserAndGroup>();
        List<BeanUserAndGroup> lst_contact_result = new List<BeanUserAndGroup>();
        bool isRequestAddInfoAction { get; set; }
        UIViewController parentView { get; set; }
        string title;
        nfloat width = 0;
        int lines = 1;
        private UITapGestureRecognizer gestureRecognizer;

        public ListUserOrGroupView(IntPtr handle) : base(handle)
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
            loadContent();
            setlangTitle();

            #region delegate
            BT_clear.TouchUpInside += BT_clear_TouchUpInside;
            BT_close.TouchUpInside += BT_close_TouchUpInside;
            BT_accept.TouchUpInside += BT_accept_TouchUpInside;
            tf_search.EditingChanged += Tf_search_EditingChanged; ;
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
        public void setContent(UIViewController _parentView, bool _isMultiSelect, List<BeanUserAndGroup> _lst_user_select, bool _isRequestAddInfoAction, ViewElement _element, string _title)
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
            if (!isMultiSelect)
            {
                Constraint_rightBTClear.Constant = 0;
                BT_accept.Hidden = true;
            }
            collectionUser_heightConstant.Constant = 0;
            lbl_lineSearchUser.Hidden = true;
            User_CollectionView.RegisterClassForCell(typeof(UserOrGroup_CollectionCell), UserOrGroup_CollectionCell.Key);

            var layoutStyle = new CollectionViewLayoutStyle();
            User_CollectionView.SetCollectionViewLayout(layoutStyle, true);
            User_CollectionView.AllowsMultipleSelection = false;

            table_content.ContentInset = new UIEdgeInsets(-20, 0, 0, 0);
            tf_search.BecomeFirstResponder();
        }

        private void loadContent()
        {
            if (element != null)
            {
                if (!string.IsNullOrEmpty(element.Value))
                    lst_user_select = JsonConvert.DeserializeObject<List<BeanUserAndGroup>>(element.Value);

                lbl_title.Text = title;

                if (element.DataType == "selectusergroup")
                {
                    if (parentView.GetType() == typeof(RequestDetailsV2) || parentView.GetType() == typeof(FormWFDetailsProperty) || parentView.GetType() == typeof(FormShareView)) // truong hop chi tiet phieu thi cho clear selectuser
                    {
                        if (lst_user_select != null)
                        {
                            int lines = CalculatorCollectionLine(false);
                            collectionUser_heightConstant.Constant = lines * 35;
                            selectedUserView_heightConstant.Constant = collectionUser_heightConstant.Constant + 50;
                            if (lines > 0)
                                lbl_lineSearchUser.Hidden = false;
                            else
                                lbl_lineSearchUser.Hidden = true;

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
                        lbl_lineSearchUser.Hidden = true;
                        BT_clear.Hidden = true;
                    }
                }
                else if (element.DataType == "selectusergroupmulti")
                {
                    if (lst_user_select != null)
                    {
                        int lines = CalculatorCollectionLine(false);
                        collectionUser_heightConstant.Constant = lines * 35;
                        selectedUserView_heightConstant.Constant = collectionUser_heightConstant.Constant + 50;
                        if (lines > 0)
                            lbl_lineSearchUser.Hidden = false;
                        else
                            lbl_lineSearchUser.Hidden = true;

                        user_CollectionSource = new User_CollectionSource(this, lst_user_select, null);
                        User_CollectionView.Source = user_CollectionSource;
                        User_CollectionView.Delegate = new User_CollectionFlowLayoutDelegate(user_CollectionSource, this);
                        User_CollectionView.ReloadData();
                    }
                }
            }
            else
            {
                if (parentView.GetType() != typeof(RequestDetailsV2) && parentView.GetType() != typeof(FormWFDetailsProperty) && parentView.GetType() != typeof(FormShareView)) // truong hop chi tiet phieu thi cho clear selectuser
                {
                    BT_clear.Hidden = true;
                }
                collectionUser_heightConstant.Constant = 0;
                selectedUserView_heightConstant.Constant = collectionUser_heightConstant.Constant + 50;
                lbl_lineSearchUser.Hidden = true;
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
        public void RemoveUserFromColletionView(BeanUserAndGroup user)
        {
            lst_user_select.Remove(user);
            int lines = CalculatorCollectionLine(true);
            user_CollectionSource.items.Remove(user);

            if (lst_user_select != null && lst_user_select.Count == 0)
            {
                collectionUser_heightConstant.Constant = 0 * 30;
                selectedUserView_heightConstant.Constant = collectionUser_heightConstant.Constant + 50;
                lbl_lineSearchUser.Hidden = true;
            }
            else
            {
                collectionUser_heightConstant.Constant = lines * 30;
                selectedUserView_heightConstant.Constant = (lines * 30) + 50;
                if (lines > 0)
                    lbl_lineSearchUser.Hidden = false;
                else
                    lbl_lineSearchUser.Hidden = true;
            }

            User_CollectionView.Source = user_CollectionSource;
            User_CollectionView.Delegate = new User_CollectionFlowLayoutDelegate(user_CollectionSource, this);
            User_CollectionView.ReloadData();
            if (element.DataType == "selectusergroup") // FormShareView chon multi nen khong nam trong day
            {
                bool isExit = false;
                if (parentView.GetType() == typeof(RequestDetailsV2))
                {
                    RequestDetailsV2 requestDetailsV2 = (RequestDetailsV2)parentView;
                    requestDetailsV2.HandleUserOrGroupSingleChoiceSelected(element, null);
                    isExit = true;
                }
                else if (parentView.GetType() == typeof(FormWFDetailsProperty))
                {
                    FormWFDetailsProperty formWFDetailsProperty = (FormWFDetailsProperty)parentView;
                    formWFDetailsProperty.HandleUserOrGroupSingleChoiceSelected(element, null);
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

        public void HandleSeclectItem(BeanUserAndGroup _user, NSIndexPath _indexPath)
        {
            if (!isMultiSelect)
            {
                if (currentUserSelected != null && currentUserSelected.ID != _user.ID)
                    currentUserSelected.IsSelected = false;

                currentUserSelected = _user;
                currentUserSelected.Name = currentUserSelected.Name;

                this.View.EndEditing(true);
                isSearch = false;

                if (parentView.GetType() == typeof(RequestDetailsV2))
                {
                    RequestDetailsV2 requestDetailsV2 = (RequestDetailsV2)parentView;
                    requestDetailsV2.HandleUserOrGroupSingleChoiceSelected(element, currentUserSelected);
                }
                else if (parentView.GetType() == typeof(ChangeUserProgress))
                {
                    //ChangeUserProgress changeUserProgress = (ChangeUserProgress)parentView;
                    ////changeUserProgress.selectedUser(currentUserSelected);
                }
                else if (parentView.GetType() == typeof(RequestAddInfo))
                {
                    //RequestAddInfo requestAddInfo = (RequestAddInfo)parentView;
                    ////requestAddInfo.selectedUser(currentUserSelected);
                }
                else if (parentView.GetType() == typeof(FormWFDetailsProperty))
                {
                    FormWFDetailsProperty formWFDetailsProperty = (FormWFDetailsProperty)parentView;
                    formWFDetailsProperty.HandleUserOrGroupSingleChoiceSelected(element, currentUserSelected);
                }

                if (this.NavigationController != null)
                    this.NavigationController.PopViewController(true);
                else
                    this.DismissViewControllerAsync(true);
            }
            else
            {
                if (lst_user_select == null)
                    lst_user_select = new List<BeanUserAndGroup>();

                tf_search.Text = "";
                table_content.Alpha = 0;

                lst_contact_result.Remove(_user);
                _user.IsSelected = !_user.IsSelected;
                table_content.ReloadData();

                lst_user_select.Add(_user);
                int lines = CalculatorCollectionLine(false);

                collectionUser_heightConstant.Constant = lines * 35;
                selectedUserView_heightConstant.Constant = collectionUser_heightConstant.Constant + 50;
                if (lines > 0)
                    lbl_lineSearchUser.Hidden = false;
                else
                    lbl_lineSearchUser.Hidden = true;

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
                width = width + item.Name.StringSize(UIFont.SystemFontOfSize(13)).Width + 5 + 30 + 20;
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
            //lst_user_select = new List<BeanUserAndGroup>();
            //lines = CalculatorCollectionLine(true);

            //collectionUser_heightConstant.Constant = lines * 30;
            //selectedUserView_heightConstant.Constant = (lines * 30) + 50;

            //user_CollectionSource = new User_CollectionSource(this, lst_user_select, null);
            //User_CollectionView.Source = user_CollectionSource;
            //User_CollectionView.Delegate = new User_CollectionFlowLayoutDelegate(user_CollectionSource, this);
            //User_CollectionView.ReloadData();

            lst_user_select = new List<BeanUserAndGroup>();
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
                var conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath, false);

                string content = CmmFunction.removeSignVietnamese(tf_search.Text.Trim().ToLowerInvariant());
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

                    //string _queryBeanUserGroup = string.Format(@"SELECT ID, Title as Name, Title AS AccountName, Description as Email, Image as ImagePath, 1 as Type FROM BeanGroup
                    //                                            WHERE (Name LIKE '%{0}%' OR Email LIKE '%{0}%' OR Description LIKE '%{0}%') AND ID NOT IN ({1})
                    //                                            UNION SELECT ID, FullName as Name, AccountName, Email, ImagePath, 0 as Type FROM BeanUser
                    //                                            WHERE (FullName LIKE '%{0}%' OR Email LIKE '%{0}%' OR AccountName LIKE '%{0}%') AND (ID NOT IN ({1}) AND ID <> '{2}')", content, str_user_selected, CmmVariable.SysConfig.UserId);

                    string _queryBeanUserGroup = string.Format(@"SELECT ID, Title as Name, Title AS AccountName, Description as Email, Image as ImagePath, 1 as Type FROM BeanGroup
                                                                WHERE (Name LIKE '%{0}%' OR Email LIKE '%{0}%' OR Description LIKE '%{0}%') AND ID NOT IN ({1})
                                                                UNION SELECT ID, FullName as Name, AccountName, Email, ImagePath, 0 as Type FROM BeanUser
                                                                WHERE (FullName LIKE '%{0}%' OR Email LIKE '%{0}%' OR AccountName LIKE '%{0}%') AND ID NOT IN ({1})", content, str_user_selected);

                    //string query = string.Format("SELECT * FROM BeanUser WHERE (FullName LIKE '%{0}%' OR Email LIKE '%{1}%' OR AccountName LIKE '%{2}%') AND ID NOT IN ({3})", content, content, content, str_user_selected);
                    lst_contact_result = conn.Query<BeanUserAndGroup>(_queryBeanUserGroup);

                    if (lst_contact_result != null && lst_contact_result.Count() > 0)
                    {
                        table_content.Alpha = 1;
                        table_content.Source = new users_TableSource(lst_contact_result, this);
                        table_content.ReloadData();
                    }
                    else
                    {
                        table_content.Alpha = 0;
                        table_content.Source = null;
                        table_content.ReloadData();
                    }
                }
                else
                {
                    table_content.Alpha = 1;
                    table_content.Source = new users_TableSource(lst_user, this);
                    table_content.ReloadData();
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
                FormShareView formShareView = parentView as FormShareView;
                formShareView.UpdateMultiUserColletionView(lst_user_select, lines);
            }
            else if (parentView.GetType() == typeof(RequestDetailsV2))
            {
                RequestDetailsV2 requestDetailsV2 = parentView as RequestDetailsV2;
                requestDetailsV2.HandleUserOrGroupMultiChoiceSelected(element, lst_user_select);
            }
            else if (parentView.GetType() == typeof(FormCreateTaskView))
            {
                FormCreateTaskView formCreateTaskView = parentView as FormCreateTaskView;
                formCreateTaskView.UpdateMultiUserColletionView(lst_user_select, 1);
            }
            else if (parentView.GetType() == typeof(FormTaskDetails))
            {
                FormTaskDetails formTaskDetails = parentView as FormTaskDetails;
                formTaskDetails.UpdateMultiUserColletionView(lst_user_select, 1);
            }
            else if (parentView.GetType() == typeof(FormWFDetailsProperty))
            {
                FormWFDetailsProperty formWFDetailsProperty = parentView as FormWFDetailsProperty;
                formWFDetailsProperty.HandleUserOrGroupMultiChoiceSelected(element, lst_user_select);
            }
            if (this.NavigationController != null)
                this.NavigationController.PopViewController(true);
            else
                this.DismissModalViewController(true);
        }
        #endregion

        #region custom class

        #region table data source user
        private class users_TableSource : UITableViewSource
        {
            List<BeanUserAndGroup> lst_user;
            NSString cellIdentifier = new NSString("cell");
            ListUserOrGroupView parentView;

            public users_TableSource(List<BeanUserAndGroup> _user, ListUserOrGroupView _parentview)
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
                UserOrGroup_cell_custom cell = new UserOrGroup_cell_custom(cellIdentifier);
                var user = lst_user[indexPath.Row];

                bool isOdd = true;
                if (indexPath.Row % 2 == 0)
                    isOdd = false;

                cell.UpdateCell(user, isOdd);
                return cell;
            }
        }
        private class UserOrGroup_cell_custom : UITableViewCell
        {
            string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            UILabel lbl_imgCover, lbl_name, lbl_email;
            UIImageView imgAvatar;
            UILabel line;
            BeanUserAndGroup userGroup { get; set; }
            bool isOdd;

            public UserOrGroup_cell_custom(NSString cellID) : base(UITableViewCellStyle.Default, cellID)
            {
                //viewConfiguration();
            }

            private void viewConfiguration()
            {
                if (isOdd)
                    ContentView.BackgroundColor = UIColor.White;
                else
                    ContentView.BackgroundColor = UIColor.FromRGB(250, 250, 250);

                lbl_imgCover = new UILabel
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = UIFont.FromName("ArialMT", 13f),
                    BackgroundColor = UIColor.Blue,
                    TextColor = UIColor.White
                };

                imgAvatar = new UIImageView();
                imgAvatar.Layer.CornerRadius = 18;
                imgAvatar.ClipsToBounds = true;
                imgAvatar.ContentMode = UIViewContentMode.ScaleAspectFill;
                imgAvatar.Image = UIImage.FromFile("Icons/icon_group.png");
                imgAvatar.Hidden = true;

                lbl_imgCover.Layer.CornerRadius = 18;
                lbl_imgCover.ClipsToBounds = true;

                lbl_name = new UILabel()
                {
                    Font = UIFont.FromName("Arial-BoldMT", 12f),
                    TextColor = UIColor.FromRGB(51, 51, 51),
                    TextAlignment = UITextAlignment.Left,
                    BackgroundColor = UIColor.Clear
                };

                lbl_email = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 12f),
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

            public void UpdateCell(BeanUserAndGroup user, bool _isOdd)
            {
                isOdd = _isOdd;

                viewConfiguration();

                userGroup = user;
                if (user.IsSelected)
                    Accessory = UITableViewCellAccessory.Checkmark;
                else
                    Accessory = UITableViewCellAccessory.None;

                if (user.Type == 0)//user
                {
                    if (string.IsNullOrEmpty(user.ImagePath))
                    {
                        if (!string.IsNullOrEmpty(user.Name))
                        {
                            imgAvatar.Hidden = true;
                            lbl_imgCover.Hidden = false;
                            lbl_imgCover.Text = CmmFunction.GetAvatarName(user.Name);
                            lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));
                        }
                        else
                        {
                            ///Rule mới hiện avatar default khi ko có FullName
                            imgAvatar.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                            imgAvatar.Hidden = false;
                            lbl_imgCover.Hidden = true;
                        }
                    }
                    else
                    {
                        checkFileLocalIsExist(user, imgAvatar);
                        lbl_imgCover.Hidden = true;
                    }

                    //if (!string.IsNullOrEmpty(user.Name))
                    //{
                    //    lbl_imgCover.Hidden = false;
                    //    imgAvatar.Hidden = true;
                    //    lbl_imgCover.Text = CmmFunction.GetAvatarName(user.Name);
                    //    lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));
                    //}

                }
                else if (user.Type == 1)
                {
                    lbl_imgCover.Hidden = true;
                    imgAvatar.Hidden = false;
                    imgAvatar.Image = UIImage.FromFile("Icons/icon_group.png");
                }

                lbl_name.Text = user.Name;
                lbl_email.Text = user.Email;
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                imgAvatar.Frame = new CGRect(15, 15, 36, 36);
                lbl_imgCover.Frame = new CGRect(15, 15, 36, 36);
                if (userGroup.Type == 0) // 0: USer - 1: Group
                {
                    lbl_name.Frame = new CGRect(lbl_imgCover.Frame.Right + 10, 8, this.ContentView.Frame.Width - 80, 25);
                    lbl_email.Frame = new CGRect(lbl_imgCover.Frame.Right + 10, lbl_name.Frame.Bottom, 400, 20);
                }
                else
                {
                    lbl_name.Frame = new CGRect(lbl_imgCover.Frame.Right + 10, 20, this.ContentView.Frame.Width - 80, 25);
                }
                //line.Frame = new CGRect(lbl_name.Frame.X, ContentView.Frame.Bottom - 0.5, ContentView.Frame.Width - lbl_name.Frame.X, 0.5);
            }

            private async void checkFileLocalIsExist(BeanUserAndGroup contact, UIImageView image_view)
            {
                try
                {
                    string filename = contact.ImagePath.Split('/').Last();
                    //string filepathURL = CmmVariable.M_Domain + "/" + CmmVariable.SysConfig.Subsite + contact.ImagePath + "?ver=" + DateTime.Now.ToString("yyyyMMddHHmmss");
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
        }

        #endregion

        #region custom collection
        public class User_CollectionSource : UICollectionViewSource
        {
            ListUserOrGroupView parentView { get; set; }
            public static Dictionary<string, List<BeanUserAndGroup>> indexedSession;
            public List<BeanUserAndGroup> items;

            public User_CollectionSource(ListUserOrGroupView _parentview, List<BeanUserAndGroup> _items, List<KeyValuePair<string, bool>> _sectionState)
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
                UserOrGroup_CollectionCell userCell = (UserOrGroup_CollectionCell)collectionView.DequeueReusableCell(UserOrGroup_CollectionCell.Key, indexPath);
                userCell.UpdateRow(items[indexPath.Row], parentView);
                return userCell;
            }

        }

        public class UserOrGroup_CollectionCell : UICollectionViewCell
        {
            public static NSString Key = new NSString("userCellId");
            UILabel lbl_title;
            UIButton BT_remove;
            KeyValuePair<string, bool> section;
            BeanUserAndGroup user { get; set; }
            public UIViewController parentView { get; set; }

            [Export("initWithFrame:")]
            public UserOrGroup_CollectionCell(RectangleF frame) : base(frame)
            {
                ViewConfiguration();
            }
            private void ViewConfiguration()
            {
                this.BackgroundColor = UIColor.FromRGB(249, 249, 249);
                this.Layer.CornerRadius = 3;
                this.ClipsToBounds = true;



                lbl_title = new UILabel()
                {
                    Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
                    TextColor = UIColor.FromRGB(65, 80, 134),
                    TextAlignment = UITextAlignment.Left,
                };

                BT_remove = new UIButton();
                UIImage img = UIImage.FromFile("Icons/icon_close_red_userOrGroup.png");
                BT_remove.SetImage(img, UIControlState.Normal);
                BT_remove.ContentEdgeInsets = new UIEdgeInsets(2, 2, 2, 2);
                BT_remove.TouchUpInside += delegate
                {
                    if (parentView.GetType() == typeof(FormShareView))
                    {
                        FormShareView formShareView = parentView as FormShareView;
                        //formShareView.RemoveUserFromColletionView(user);
                    }
                    else if (parentView.GetType() == typeof(ListUserOrGroupView))
                    {
                        ListUserOrGroupView listUser = parentView as ListUserOrGroupView;
                        listUser.RemoveUserFromColletionView(user);
                    }
                };

                this.AddSubviews(lbl_title, BT_remove);
            }

            public void UpdateRow(BeanUserAndGroup _user, UIViewController _parent)
            {
                user = _user;
                parentView = _parent;
                lbl_title.Text = user.Name;

            }
            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                var width = user.Name.StringSize(UIFont.SystemFontOfSize(13)).Width + 5;
                lbl_title.Frame = new CGRect(5, 3, width, 20);
                BT_remove.Frame = new CGRect(lbl_title.Frame.Right, 4, 18, 18);
            }
        }

        private class User_CollectionFlowLayoutDelegate : UICollectionViewDelegateFlowLayout
        {
            ListUserOrGroupView parent;
            User_CollectionSource collectionSource_user;
            nfloat titleWidth = 0, height = 0;
            BeanUserAndGroup user { get; set; }

            #region Constructors
            public User_CollectionFlowLayoutDelegate(User_CollectionSource _lst_user, ListUserOrGroupView _parent)
            {
                collectionSource_user = _lst_user;
                parent = _parent;
            }
            #endregion

            #region Override Methods

            public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
            {
                user = collectionSource_user.items[indexPath.Row];
                string title = user.Name;

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


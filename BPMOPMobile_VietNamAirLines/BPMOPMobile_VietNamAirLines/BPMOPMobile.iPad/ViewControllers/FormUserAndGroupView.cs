using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.iPad.CustomControlClass;
using BPMOPMobile.iPad.IOSClass;
using BPMOPMobile.iPad.ViewControllers;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UIKit;

namespace BPMOPMobile.iPad
{
    public partial class FormUserAndGroupView : UIViewController
    {
        public bool isAllowEdit = true;
        User_CollectionSource user_CollectionSource;
        ViewElement element { get; set; }
        UIViewController parentView { get; set; }
        List<BeanUserAndGroup> lst_user = new List<BeanUserAndGroup>();
        List<BeanUserAndGroup> lst_user_select = new List<BeanUserAndGroup>();
        List<BeanUserAndGroup> lst_user_result = new List<BeanUserAndGroup>();
        bool isRequestAddInfoAction { get; set; }
        bool isMultiSelect = true;
        bool isSearch = false;
        bool isLoadData;
        bool isUserAndGroup = true;
        bool allowChoiceCurrentUser;
        string title;
        BeanUserAndGroup currentUserSelected { get; set; }
        int lines = 1;
        nfloat width = 0;
        List<string> lst_user_selected { get; set; }

        public FormUserAndGroupView(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            CmmIOSFunction.ResignFirstResponderOnTap(this.View);
            ViewConfiguration();
            LoadContent();

            #region delegate
            BT_close.TouchUpInside += BT_close_TouchUpInside;
            BT_approve.TouchUpInside += BT_approve_TouchUpInside;
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

        #region public - private method
        private void ViewConfiguration()
        {
            BT_close.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);
            BT_approve.ContentEdgeInsets = new UIEdgeInsets(7, 5, 7, 5);

            tf_search.Placeholder = CmmFunction.GetTitle("TEXT_HINT_USER_EMAIL", "Vui lòng nhập tên hoặc địa chỉ email...");
            tf_search.Font = UIFont.FromName("Arial-ItalicMT", 14);

            User_CollectionView.RegisterClassForCell(typeof(UserGroup_CollectionCell), UserGroup_CollectionCell.Key);

            var layoutStyle = new CollectionViewLayoutStyle();
            User_CollectionView.SetCollectionViewLayout(layoutStyle, true);
            User_CollectionView.AllowsMultipleSelection = false;
            if (isAllowEdit)
                tf_search.BecomeFirstResponder();
        }

        //public void SetContent(UIViewController _parentView, bool _isMultiSelect, bool _isUserAndGroup, List<string> _lst_user_selected)
        //{
        //    parentView = _parentView;
        //    isMultiSelect = _isMultiSelect;
        //    isUserAndGroup = _isUserAndGroup;
        //    lst_user_selected = _lst_user_selected;
        //}

        public void SetContent(UIViewController _parentView, bool _isMultiSelect, List<BeanUserAndGroup> _lst_user_select, bool _isRequestAddInfoAction, ViewElement _element, string _title, bool _allowChoiceCurrentUser)
        {
            isMultiSelect = _isMultiSelect;
            lst_user_select = _lst_user_select;
            parentView = _parentView;
            isRequestAddInfoAction = _isRequestAddInfoAction;
            element = _element;
            title = _title;
            allowChoiceCurrentUser = _allowChoiceCurrentUser;
        }

        private void LoadContent()
        {
            lbl_title.Text = title;

            if (element != null)
            {
                if (!string.IsNullOrEmpty(element.Value))
                    lst_user_select = JsonConvert.DeserializeObject<List<BeanUserAndGroup>>(element.Value);

                if (element.DataType == "selectuser" || element.DataType == "selectusergroup")
                {
                    collectionUser_heightConstant.Constant = 0;
                    selectedUserView_heightConstant.Constant = collectionUser_heightConstant.Constant + 50;
                }
                else if (element.DataType == "selectusergroupmulti")
                {
                    if (lst_user_select != null)
                    {
                        int lines = CalculatorCollectionLine(false);
                        collectionUser_heightConstant.Constant = lines * 35;
                        selectedUserView_heightConstant.Constant = collectionUser_heightConstant.Constant + 50;

                        user_CollectionSource = new User_CollectionSource(this, lst_user_select, null);
                        User_CollectionView.Source = user_CollectionSource;
                        User_CollectionView.Delegate = new User_CollectionFlowLayoutDelegate(user_CollectionSource, this);
                        User_CollectionView.ReloadData();
                    }
                    else
                    {
                        collectionUser_heightConstant.Constant = 0;
                        selectedUserView_heightConstant.Constant = collectionUser_heightConstant.Constant + 50;
                    }
                }
            }
            //truong hop navigate tu form co dinh, khong co Element
            else if (lst_user_select != null && lst_user_select.Count > 0)
            {
                int lines = CalculatorCollectionLine(false);
                collectionUser_heightConstant.Constant = lines * 35;
                selectedUserView_heightConstant.Constant = collectionUser_heightConstant.Constant + 50;

                user_CollectionSource = new User_CollectionSource(this, lst_user_select, null);
                User_CollectionView.Source = user_CollectionSource;
                User_CollectionView.Delegate = new User_CollectionFlowLayoutDelegate(user_CollectionSource, this);
                User_CollectionView.ReloadData();
            }
            else
            {
                collectionUser_heightConstant.Constant = 0;
                selectedUserView_heightConstant.Constant = collectionUser_heightConstant.Constant + 50;
            }

            if (!isAllowEdit)
            {
                BT_approve.Hidden = true;
                tf_search.Hidden = true;
                //img_search.Hidden = true;
                User_CollectionView.UserInteractionEnabled = false;
            }
        }

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
                //string usertype_lookup = _user.ID + ";#" + _user.Name;
                string usertype_lookup = _user.AccountName;

                if (parentView.GetType() == typeof(ToDoDetailView))
                {
                    ToDoDetailView requestDetailsV2 = (ToDoDetailView)parentView;
                    requestDetailsV2.HandleUserOrGroupSingleChoiceSelected(element, currentUserSelected);
                }
                else if (parentView.GetType() == typeof(WorkflowDetailView))
                {
                    WorkflowDetailView requestDetailsV2 = (WorkflowDetailView)parentView;
                    requestDetailsV2.HandleUserOrGroupSingleChoiceSelected(element, currentUserSelected);
                }
                else if (parentView.GetType() == typeof(FormWorkFlowDetails))
                {
                    FormWorkFlowDetails requestDetailsV2 = (FormWorkFlowDetails)parentView;
                    requestDetailsV2.HandleUserOrGroupSingleChoiceSelected(element, currentUserSelected);
                }
                else if (parentView.GetType() == typeof(FormTransferHandleView))
                {
                    //FormTransferHandleView changeUserProgress = (FormTransferHandleView)parentView;
                    //changeUserProgress.selectedUser(currentUserSelected);
                }
                else if (parentView.GetType() == typeof(FormAdditionalInformationView))
                {
                    //FormAdditionalInformationView requestAddInfo = (FormAdditionalInformationView)parentView;
                    //requestAddInfo.selectedUser(currentUserSelected);
                }
                else if (parentView.GetType() == typeof(FollowListViewController))
                {
                    FollowListViewController followListViewController = (FollowListViewController)parentView;
                    followListViewController.HandleUserOrGroupSingleChoiceSelected(element, currentUserSelected);
                }

                this.View.EndEditing(true);
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

                lst_user_result.Remove(_user);
                _user.IsSelected = !_user.IsSelected;
                table_content.ReloadData();

                lst_user_select.Add(_user);
                lines = CalculatorCollectionLine(false);

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
            if (_isRemove)
                lines = 0;

            foreach (var item in lst_user_select)
            {
                width = width + item.Name.StringSize(UIFont.SystemFontOfSize(13)).Width + 5 + 30 + 20;

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

        private void CheckUserSelected()
        {
            if (lst_user_selected != null)
                lst_user = lst_user.Where(item => !lst_user_selected.Contains(item.ID)).ToList();
        }

        public void RemoveUserFromColletionView(BeanUserAndGroup user)
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
        }
        #endregion

        #region event
        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            this.DismissModalViewController(true);
        }

        private void BT_approve_TouchUpInside(object sender, EventArgs e)
        {
            var lst_item = lst_user.FindAll(item => item.IsSelected);
            if (parentView != null && parentView.GetType() == typeof(FormShareView))
            {
                FormShareView formShareView = parentView as FormShareView;
                formShareView.UpdateMultiUserColletionView(lst_user_select, lines);
            }
            else if (parentView != null && parentView.GetType() == typeof(ToDoDetailView))
            {
                ToDoDetailView controller = (ToDoDetailView)parentView;
                controller.HandleUserOrGroupMultiChoiceSelected(element, lst_user_select);
            }
            else if (parentView != null && parentView.GetType() == typeof(FormCreateView))
            {

            }
            else if (parentView != null && parentView.GetType() == typeof(FormTransferHandleView))
            {

            }
            else if (parentView != null && parentView.GetType() == typeof(FormTaskDetails))
            {
                FormTaskDetails controller = (FormTaskDetails)parentView;
                controller.UpdateMultiUserColletionView(lst_user_select, 1);
            }
            else if (parentView != null && parentView.GetType() == typeof(FormCreateTaskView))
            {
                FormCreateTaskView controller = (FormCreateTaskView)parentView;
                controller.UpdateMultiUserColletionView(lst_user_select, 1);
            }
            else if (parentView != null && parentView.GetType() == typeof(FollowListViewController))
            {
                FollowListViewController controller = (FollowListViewController)parentView;
                controller.HandleUserOrGroupMultiChoiceSelected(element, lst_user_select);
            }

            this.DismissModalViewController(true);
        }

        private void Tf_search_EditingChanged(object sender, EventArgs e)
        {
            try
            {
                if (!isAllowEdit) return;
                var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
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

                    if (allowChoiceCurrentUser)
                        str_user_selected = "'" + String.Join("','", lst_UserSelectedID.ToArray()) + "'";
                    else
                        str_user_selected = "'" + CmmVariable.SysConfig.UserId.ToLower() + "'," + "'" + String.Join("','", lst_UserSelectedID.ToArray()) + "'";

                    string _queryBeanUserGroup = string.Format(@"SELECT ID, Title as Name, Title AS AccountName, Description as Email, Image as ImagePath, 1 as Type FROM BeanGroup
                                                                WHERE (Name LIKE '%{0}%' OR Email LIKE '%{0}%' OR Description LIKE '%{0}%') AND ID NOT IN ({1})
                                                                UNION SELECT ID, FullName as Name, AccountName, Email, ImagePath, 0 as Type FROM BeanUser
                                                                WHERE (FullName LIKE '%{0}%' OR Email LIKE '%{0}%' OR AccountName LIKE '%{0}%') AND ID NOT IN ({1})", content, str_user_selected);

                    lst_user_result = conn.Query<BeanUserAndGroup>(_queryBeanUserGroup);

                    if (lst_user_result != null && lst_user_result.Count() > 0)
                    {
                        table_content.Alpha = 1;
                        table_content.Source = new users_TableSource(lst_user_result, this);
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
            catch (Exception ex)
            {
                Console.WriteLine("AssignedToView - SearchBar_user_TextChanged - Err: " + ex.ToString());
            }
        }
        #endregion

        #region table data source user
        //private class FullUsers_TableSource : UITableViewSource
        //{
        //    List<BeanUser> lst_user;
        //    NSString cellIdentifier = new NSString("cellFullUser");
        //    FormUserAndGroupView parentView;

        //    public FullUsers_TableSource(List<BeanUser> _users, FormUserAndGroupView _parentview)
        //    {
        //        parentView = _parentview;
        //        if (_users != null)
        //            lst_user = _users;
        //    }

        //    public override nint RowsInSection(UITableView tableview, nint section)
        //    {
        //        return lst_user.Count;
        //    }

        //    public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        //    {
        //        return 78;
        //    }

        //    public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        //    {
        //        var item = lst_user[indexPath.Row];
        //        parentView.HandleSeclectItem(item, indexPath);
        //        tableView.DeselectRow(indexPath, true);
        //    }

        //    public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        //    {
        //        Custom_UserCell cell = new Custom_UserCell(cellIdentifier, parentView);
        //        var user = lst_user[indexPath.Row];

        //        bool isOdd = true;
        //        if (indexPath.Row % 2 == 0)
        //            isOdd = false;

        //        cell.UpdateCell(user, false, isOdd);
        //        return cell;
        //    }
        //}

        #region table data source user
        private class users_TableSource : UITableViewSource
        {
            List<BeanUserAndGroup> lst_user;
            NSString cellIdentifier = new NSString("cell");
            FormUserAndGroupView parentView;

            public users_TableSource(List<BeanUserAndGroup> _user, FormUserAndGroupView _parentview)
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

            }
            private void viewConfiguration()
            {
                imgAvatar = new UIImageView();
                imgAvatar.Layer.CornerRadius = 18;
                imgAvatar.ClipsToBounds = true;
                imgAvatar.ContentMode = UIViewContentMode.ScaleAspectFit;
                imgAvatar.Image = UIImage.FromFile("Icons/icon_group.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
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

            public void UpdateCell(BeanUserAndGroup user)
            {
                viewConfiguration();

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
                            lbl_imgCover.Hidden = false;
                            imgAvatar.Hidden = true;
                            lbl_imgCover.Text = CmmFunction.GetAvatarName(user.Name);
                            lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));
                        }
                    }
                    else
                    {
                        checkFileLocalIsExist(user, imgAvatar);
                        lbl_imgCover.Hidden = true;
                    }
                }
                else if (user.Type == 1) //group
                {
                    lbl_imgCover.Hidden = true;
                    imgAvatar.Hidden = false;
                }

                lbl_name.Text = user.Name;
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

            private async void checkFileLocalIsExist(BeanUserAndGroup contact, UIImageView image_view)
            {
                try
                {
                    string filename = contact.ID + "_Avatar.jpg";//?ver=" + DateTime.Now.ToString("yyyyMMddHHmmss");
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
            FormUserAndGroupView parentView { get; set; }
            public static Dictionary<string, List<BeanUserAndGroup>> indexedSession;
            public List<BeanUserAndGroup> items;

            public User_CollectionSource(FormUserAndGroupView _parentview, List<BeanUserAndGroup> _items, List<KeyValuePair<string, bool>> _sectionState)
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
                UserGroup_CollectionCell userCell = (UserGroup_CollectionCell)collectionView.DequeueReusableCell(UserGroup_CollectionCell.Key, indexPath);
                userCell.UpdateRow(items[indexPath.Row], parentView);
                return userCell;
            }

        }

        private class User_CollectionFlowLayoutDelegate : UICollectionViewDelegateFlowLayout
        {
            FormUserAndGroupView parent;
            User_CollectionSource collectionSource_user;
            nfloat titleWidth = 0, height = 0;
            BeanUserAndGroup user { get; set; }

            #region Constructors
            public User_CollectionFlowLayoutDelegate(User_CollectionSource _lst_user, FormUserAndGroupView _parent)
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
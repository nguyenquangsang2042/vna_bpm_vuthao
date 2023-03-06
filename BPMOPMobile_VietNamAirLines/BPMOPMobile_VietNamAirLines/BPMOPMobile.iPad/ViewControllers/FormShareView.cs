using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.iPad.CustomControlClass;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UIKit;

namespace BPMOPMobile.iPad
{
    public partial class FormShareView : UIViewController
    {
        List<BeanUserAndGroup> lst_userGroupSelected { get; set; }
        User_CollectionSource user_CollectionSource;
        BeanWorkflowItem workflowItem;
        List<BeanShareHistory> lst_ShareHistory;
        bool isLoadData = false;
        UIViewController parent { get; set; }
        CmmLoading loading;
        string str_json_FormDefineInfo;
        string WorkflowActionID;
        public nfloat estRowHeight;


        private NSObject _willResignActiveNotificationObserver;
        private NSObject _didBecomeActiveNotificationObserver;
        private nfloat positionBotOfCurrentViewInput { get; set; }

        public FormShareView(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            UITapGestureRecognizer gesture = new UITapGestureRecognizer(() =>
            {
                View.EndEditing(true);
            });

            gesture.ShouldReceiveTouch += delegate (UIGestureRecognizer recognizer, UITouch touch)
            {
                var touchView = touch.View.Class.Name;
                if (touchView == "UITextField" || touchView == "UITextView")
                    positionBotOfCurrentViewInput = GetPositionBotView(touch.View);
                else
                    positionBotOfCurrentViewInput = 0.0f;

                return true;
            };

            gesture.CancelsTouchesInView = false;
            View.AddGestureRecognizer(gesture);

            ViewConfiguration();
            LoadDataShare();
            SetLangTitle();

            #region delegate
            BT_close.TouchUpInside += BT_close_TouchUpInside;
            txt_note.Started += Txt_Note_Started;
            txt_note.Ended += Txt_Note_Ended;
            BT_selectUser.TouchUpInside += BT_selectUser_TouchUpInside;
            BT_submit.TouchUpInside += BT_submit_TouchUpInside;
            #endregion
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            _willResignActiveNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, KeyBoardUpNotification);
            _didBecomeActiveNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyBoardDownNotification);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            if (_willResignActiveNotificationObserver != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(_willResignActiveNotificationObserver);

            if (_didBecomeActiveNotificationObserver != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(_didBecomeActiveNotificationObserver);
        }

        #region public - private method
        public void SetContent(UIViewController _parent, BeanWorkflowItem _workflowItem, string _str_json_FormDefineInfo)
        {
            workflowItem = _workflowItem;
            parent = _parent;
            str_json_FormDefineInfo = _str_json_FormDefineInfo;
            WorkflowActionID = _workflowItem.ID;
        }

        private void SetLangTitle()
        {
            CmmIOSFunction.SetLangToView(this.View);
        }

        private void ViewConfiguration()
        {
            BT_close.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);
            BT_submit.ContentEdgeInsets = new UIEdgeInsets(7, 5, 7, 5);

            view_shareUserSelected.Layer.BorderColor = UIColor.LightGray.ColorWithAlpha(0.6f).CGColor;
            view_shareUserSelected.Layer.BorderWidth = 0.5f;
            view_shareUserSelected.Layer.CornerRadius = 3;

            CmmIOSFunction.AddBorderView(view_note);

            CmmIOSFunction.AddAttributeTitle(lbl_userTitle);
            CmmIOSFunction.AddAttributeTitle(lbl_noteTitle);

            User_CollectionView.Layer.BorderColor = UIColor.LightGray.CGColor;
            User_CollectionView.Layer.BorderWidth = 0;
            User_CollectionView.Layer.CornerRadius = 5;
            User_CollectionView.RegisterClassForCell(typeof(UserGroup_CollectionCell), UserGroup_CollectionCell.Key);

            var layoutStyle = new CollectionViewLayoutStyle();
            User_CollectionView.SetCollectionViewLayout(layoutStyle, true);
            User_CollectionView.AllowsMultipleSelection = false;

            table_shareUser.ScrollEnabled = false;
        }

        private async void LoadDataShare()
        {
            await Task.Run(() =>
            {
                ProviderControlDynamic providerControlDynamic = new ProviderControlDynamic();
                lst_ShareHistory = providerControlDynamic.GetListShareHistory(workflowItem);
            });

            InvokeOnMainThread(() =>
            {
                if (lst_ShareHistory != null && lst_ShareHistory.Count > 0)
                {
                    view_shareItem.Hidden = false;
                    table_shareUser.Source = new ShareHistory_TableSource(lst_ShareHistory, this);
                    table_shareUser.SeparatorStyle = UITableViewCellSeparatorStyle.None;
                }
                else
                {
                    view_shareItem.Hidden = true;
                    lst_ShareHistory = new List<BeanShareHistory>();
                    table_shareUser.Source = new ShareHistory_TableSource(lst_ShareHistory, this);
                }
                UpdateLayoutHeight();
            });
        }

        public void UpdateLayoutHeight()
        {
            foreach (var item in lst_ShareHistory)
            {
                if (!item.ParentId.HasValue)
                {
                    if (!string.IsNullOrEmpty(item.Comment))
                    {
                        CGRect rect = StringExtensions.StringRect(item.Comment, UIFont.FromName("ArialMT", 14f), (table_shareUser.Frame.Width / 5) * 4);
                        if (rect.Height > 0 && rect.Height < 20)
                            rect.Height = 30;

                        estRowHeight += rect.Height + 50;
                    }
                    else
                        estRowHeight += 50;
                }
                else
                    estRowHeight += 60;
            }

            table_shareUser.ReloadData();
            view_tableUser_height_Constant.Constant = estRowHeight + 10;
            share_heightConstraint.Constant = view_tableUser_height_Constant.Constant + 50;
            scrollview_content.ContentSize = new CGSize(scrollview_content.Frame.Width, share_heightConstraint.Constant + 20);
        }

        public void RemoveUserFromColletionView(BeanUserAndGroup user)
        {
            user_CollectionSource.items.Remove(user);
            User_CollectionView.Source = user_CollectionSource;
            User_CollectionView.Delegate = new User_CollectionFlowLayoutDelegate(user_CollectionSource, this);
            User_CollectionView.ReloadData();
        }

        public void UpdateMultiUserColletionView(List<BeanUserAndGroup> _lst_selected, int lines)
        {
            if (_lst_selected != null && _lst_selected.Count > 0)
            {
                lst_userGroupSelected = _lst_selected;
                //view_tableUser_height_Constant.Constant = lines * 35;
                tf_placeholder.Hidden = true;

                user_CollectionSource = new User_CollectionSource(this, lst_userGroupSelected, null);

                //if (user_CollectionSource == null)
                //    user_CollectionSource = new User_CollectionSource(this, lst_userSelected, null);
                //else
                //    user_CollectionSource.items.AddRange(lst_userSelected);

                User_CollectionView.Source = user_CollectionSource;
                User_CollectionView.Delegate = new User_CollectionFlowLayoutDelegate(user_CollectionSource, this);
                User_CollectionView.ReloadData();
            }
        }

        private nfloat GetPositionBotView(UIView view)
        {
            nfloat bottom = view.Frame.Height;
            UIView supperView = view;
            do
            {
                bottom += supperView.Frame.Y;
                supperView = supperView.Superview;

            } while (supperView != this.View);

            return bottom + 20;
        }

        //private string GetStringUsers()
        //{
        //    string strUsers = "";
        //    if (lst_selectedUser.Count > 0)
        //    {
        //        foreach (var item in lst_selectedUser)
        //        {
        //            strUsers += item.Name + "; ";
        //        }
        //    }

        //    if (!string.IsNullOrEmpty(strUsers))
        //    {
        //       strUsers = strUsers.Trim().TrimEnd(';');
        //    }

        //    return strUsers;
        //}

        //private List<string> GetListIdUserSelected()
        //{
        //if (lst_selectedUser.Count > 0)
        //{
        //    List<string> lst_result = new List<string>();
        //    foreach (var item in lst_selectedUser)
        //    {
        //        lst_result.Add(item.ID);
        //    }

        //    return lst_result;
        //}
        //else
        //    return null;

        //}
        #endregion

        #region event
        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            this.DismissModalViewController(true);
        }

        private void BT_selectUser_TouchUpInside(object sender, EventArgs e)
        {
            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            FormUserAndGroupView formUserAndGroupView = (FormUserAndGroupView)Storyboard.InstantiateViewController("FormUserAndGroupView");
            formUserAndGroupView.SetContent(this, true, lst_userGroupSelected, false, null, CmmFunction.GetTitle("TEXT_TITLE_USERGROUP", "Chọn người hoặc nhóm"), false);
            PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            formUserAndGroupView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            formUserAndGroupView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            formUserAndGroupView.TransitioningDelegate = transitioningDelegate;
            this.PresentViewControllerAsync(formUserAndGroupView, true);
        }

        void Txt_Note_Ended(object sender, EventArgs e)
        {

        }

        void Txt_Note_Started(object sender, EventArgs e)
        {

        }

        private async void BT_submit_TouchUpInside(object sender, EventArgs e)
        {
            loading = new CmmLoading(new CGRect((this.View.Bounds.Width - 200) / 2, (this.View.Bounds.Height - 200) / 2, 200, 200), "Đang xử lý...");
            this.View.Add(loading);
            try
            {
                if (lst_userGroupSelected == null || lst_userGroupSelected.Count == 0)
                {
                    CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("MESS_REQUIRE_USERGROUP", "Vui lòng chọn người xử lý..."));
                    loading.Hide();
                }
                else
                {
                    string idea = txt_note.Text;
                    await Task.Run(() =>
                    {
                        bool result = false;
                        ProviderBase b_pase = new ProviderBase();
                        ProviderControlDynamic providerControl = new ProviderControlDynamic();
                        List<KeyValuePair<string, string>> lstExtent = new List<KeyValuePair<string, string>>();

                        KeyValuePair<string, string> note = new KeyValuePair<string, string>("idea", idea);
                        lstExtent.Add(note);

                        string usersvalue = "";
                        foreach (var item in lst_userGroupSelected)
                        {
                            usersvalue = usersvalue + item.AccountName + ";";
                        }

                        usersvalue.TrimEnd(';');
                        KeyValuePair<string, string> user = new KeyValuePair<string, string>("userValues", usersvalue);
                        lstExtent.Add(user);

                        ButtonAction _buttonAction = new ButtonAction();
                        _buttonAction.Value = "Share";

                        string str_ErrRef = "";
                        if (lstExtent != null && lstExtent.Count > 0)
                            result = providerControl.SendControlDynamicAction(_buttonAction.Value, WorkflowActionID, str_json_FormDefineInfo, "", ref str_ErrRef, null, lstExtent);
                        else
                            result = providerControl.SendControlDynamicAction(_buttonAction.Value, WorkflowActionID, str_json_FormDefineInfo, "", ref str_ErrRef, null);

                        if (result)
                        {
                            b_pase.UpdateAllDynamicData(true);
                            InvokeOnMainThread(() =>
                            {
                                loading.Hide();
                                if (this.NavigationController != null)
                                    this.NavigationController.PopViewController(true);
                                else
                                    this.DismissModalViewController(true);
                            });
                        }
                        else
                        {
                            InvokeOnMainThread(() =>
                            {
                                loading.Hide();
                                CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("K_Mess_ActionFalse", "Thao tác không thực hiện được, vui lòng thử lại."));
                            });
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                loading.Hide();
                Console.WriteLine("ViewRequestDetails - submitAction - ERR: " + ex.ToString());
            }
        }

        private void KeyBoardUpNotification(NSNotification notification)
        {
            try
            {
                if (View.Frame.Y == 0)
                {
                    CGRect keyboardSize = UIKeyboard.BoundsFromNotification(notification);

                    var topKeybroad = this.View.Frame.Height - keyboardSize.Height;
                    if (topKeybroad < positionBotOfCurrentViewInput)
                    {
                        CGRect custFrame = View.Frame;
                        custFrame.Y -= (positionBotOfCurrentViewInput - topKeybroad);
                        View.Frame = custFrame;
                    }

                }
            }
            catch (Exception ex)
            { Console.WriteLine("FormShareView - KeyBoardUpNotification - Err: " + ex.ToString()); }
        }

        private void KeyBoardDownNotification(NSNotification notification)
        {
            try
            {
                if (View.Frame.Y != 0)
                {
                    CGRect custFrame = View.Frame;
                    custFrame.Y = 0;
                    View.Frame = custFrame;
                }
            }
            catch (Exception ex)
            { Console.WriteLine("FormShareView - KeyBoardDownNotification - Err: " + ex.ToString()); }
        }
        #endregion

        #region custom views
        #region Table source ShareItem

        private class ShareHistory_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellShareItemID");
            FormShareView parentView;
            List<BeanShareHistory> lst_shareHistory { get; set; }
            Dictionary<BeanShareHistory, List<BeanShareHistory>> dict_shareitem = new Dictionary<BeanShareHistory, List<BeanShareHistory>>();
            List<BeanShareHistory> sectionKeys;
            private nfloat estHeight = 0;

            public ShareHistory_TableSource(List<BeanShareHistory> _lst_shareHistory, FormShareView _parentview)
            {
                lst_shareHistory = _lst_shareHistory;
                parentView = _parentview;
                LoadData();
            }

            private void LoadData()
            {
                if (lst_shareHistory != null)
                {
                    sectionKeys = lst_shareHistory.Where(x => !x.ParentId.HasValue).ToList();

                    List<BeanShareHistory> lst_item = new List<BeanShareHistory>();
                    foreach (var section in sectionKeys)
                    {
                        List<BeanShareHistory> lst = lst_shareHistory.Where(x => x.ParentId == section.ID).ToList();
                        dict_shareitem.Add(section, lst);

                        //KeyValuePair<string, bool> keypair_section;
                        //keypair_section = new KeyValuePair<string, bool>(section, false);
                        //lst_sectionState.Add(keypair_section);
                    }

                    //parentView.lst_sectionState = lst_sectionState;

                }
                else
                {
                    //sectionKeys = lst_attachment.Select(x => x.Category).Distinct().ToList();

                    //foreach (var section in sectionKeys)
                    //{
                    //    List<BeanAttachFile> lst_item = lst_attachment.Where(x => x.Category == section).ToList();
                    //    dict_attachments.Add(section, lst_item);
                    //}
                }
            }

            public override nfloat GetHeightForFooter(UITableView tableView, nint section)
            {
                return -1;
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                //var item = dict_shareitem.ElementAt(Convert.ToInt32(section)).Key;
                var item = sectionKeys[Convert.ToInt32(section)];
                if (!string.IsNullOrEmpty(item.Comment))
                {
                    CGRect rect = StringExtensions.StringRect(item.Comment, UIFont.FromName("ArialMT", 14f), (parentView.table_shareUser.Frame.Width / 5) * 4.4f);
                    if (rect.Height > 0 && rect.Height < 20)
                        rect.Height = 30;
                    return rect.Height + 50;
                }
                else
                    return 60;

                //if (dict_shareitem[key].Count > 0)
                //    return 70;
                //else
                //    return 1;

            }

            public override UIView GetViewForHeader(UITableView tableView, nint section)
            {
                var sec = sectionKeys[(Int16)section];
                var key = dict_shareitem.ElementAt(Convert.ToInt32(section)).Key;
                //sectionState = lst_sectionState[(int)section];

                if (dict_shareitem[key].Count > 0)
                {
                    UIView baseView = new UIView();
                    baseView.Frame = new CGRect(0, 0, tableView.Frame.Width, 70);

                    UILabel lbl_date = new UILabel()
                    {
                        Font = UIFont.FromName("ArialMT", 13f),
                        TextColor = UIColor.FromRGB(94, 94, 94)
                    };

                    UIImageView img_avatar = new UIImageView();
                    img_avatar.ContentMode = UIViewContentMode.ScaleAspectFill;

                    UILabel lbl_title = new UILabel()
                    {
                        Font = UIFont.FromName("Arial-BoldMT", 15f),
                        TextColor = UIColor.Black
                    };

                    UILabel lbl_avatar = new UILabel()
                    {
                        Font = UIFont.FromName("Arial-BoldMT", 16f),
                        TextColor = UIColor.White
                    };

                    UILabel lbl_sub_title = new UILabel()
                    {
                        Font = UIFont.FromName("ArialMT", 13f),
                        TextColor = UIColor.FromRGB(94, 94, 94)
                    };

                    UILabel lbl_ykien = new UILabel
                    {
                        TextAlignment = UITextAlignment.Left,
                        Font = UIFont.FromName("ArialMT", 14f),
                        LineBreakMode = UILineBreakMode.WordWrap,
                        Lines = 0,
                        TextColor = UIColor.Black,
                        Hidden = false
                    };

                    var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                    lbl_title.Text = key.UserName;
                    lbl_sub_title.Text = key.UserPosition;

                    if (!string.IsNullOrEmpty(key.UserId))
                    {
                        List<BeanUser> lst_userResult = new List<BeanUser>();
                        string query_user = string.Format("SELECT * FROM BeanUser WHERE ID =?");
                        lst_userResult = conn.Query<BeanUser>(query_user, key.UserId);

                        string user_imagePath = "";
                        if (lst_userResult.Count > 0)
                        {
                            user_imagePath = lst_userResult[0].ImagePath;
                            lbl_title.Text = lst_userResult[0].FullName;
                            lbl_sub_title.Text = lst_userResult[0].Position;

                            if (string.IsNullOrEmpty(user_imagePath))
                            {

                                lbl_avatar.Hidden = false;
                                img_avatar.Hidden = true;
                                lbl_avatar.Text = CmmFunction.GetAvatarName(lst_userResult[0].FullName);
                                lbl_avatar.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_avatar.Text));

                            }
                            else
                            {
                                lbl_avatar.Hidden = false;
                                img_avatar.Hidden = true;
                                lbl_avatar.Text = CmmFunction.GetAvatarName(lst_userResult[0].FullName);
                                lbl_avatar.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_avatar.Text));

                                checkFileLocalIsExist(lst_userResult[0], lbl_avatar, img_avatar);

                                //kiem tra xong cap nhat lai avatar
                                lbl_avatar.Hidden = true;
                                img_avatar.Hidden = false;
                            }
                        }

                    }

                    if (CmmVariable.SysConfig.LangCode == "1033")
                        lbl_date.Text = key.DateShared.ToString(CmmVariable.M_WorkDateFormatDateTimeEN);
                    else
                        lbl_date.Text = key.DateShared.ToString(CmmVariable.M_WorkDateFormatDateTimeVN);

                    lbl_ykien.Text = key.Comment;

                    img_avatar.Frame = new CGRect(10, 10, 40, 40);
                    img_avatar.Layer.CornerRadius = 20;
                    img_avatar.ClipsToBounds = true;
                    lbl_avatar.Frame = new CGRect(10, 10, 40, 40);
                    lbl_avatar.Layer.CornerRadius = 20;
                    lbl_avatar.ClipsToBounds = true;

                    lbl_title.Frame = new CGRect(img_avatar.Frame.Right + 10, 10, ((baseView.Frame.Width - img_avatar.Frame.Right) / 3) * 2, 20);
                    lbl_date.Frame = new CGRect(baseView.Frame.Width - 100, 10, 90, 20);
                    lbl_sub_title.Frame = new CGRect(lbl_title.Frame.X, lbl_title.Frame.Bottom, (baseView.Frame.Width - lbl_title.Frame.X) - 10, 20);

                    if (!string.IsNullOrEmpty(key.Comment))
                    {
                        lbl_ykien.Hidden = false;
                        lbl_ykien.Frame = new CGRect(lbl_title.Frame.X, lbl_sub_title.Frame.Bottom, lbl_sub_title.Frame.Width, 20);
                        lbl_ykien.Text = key.Comment;
                        lbl_ykien.SizeToFit();
                    }

                    if (section % 2 != 0)
                        baseView.BackgroundColor = UIColor.FromRGB(249, 249, 249);
                    else
                        baseView.BackgroundColor = UIColor.White;

                    baseView.AddSubviews(new UIView[] { img_avatar, lbl_avatar, lbl_title, lbl_date, lbl_sub_title, lbl_ykien });

                    return baseView;
                }
                else
                    return null;

            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 60;
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return sectionKeys.Count;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                //foreach (var i in dict_shareitem)
                //{
                return dict_shareitem[sectionKeys[(int)section]].Count;
                //}
                //return 0;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                //var item = lst_workRelated[indexPath.Row];
                //parentView.HandleSelectedItem(item);
                //tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var shareitem = dict_shareitem[sectionKeys[indexPath.Section]][indexPath.Row];

                //var isFirst = (indexPath.Row == 0) ? true : false;
                //var isLast = (indexPath.Section == sectionKeys.Count - 1 && indexPath.Row == (dict_shareitem[sectionKeys[indexPath.Section]].Count - 1)) ? true : false;
                //var isShowBotLine = (indexPath.Row == (dict_shareitem[sectionKeys[indexPath.Section]].Count - 1)) ? false : true;

                Custom_ShareItemCell cell = new Custom_ShareItemCell(cellIdentifier);

                cell.UpdateCell(shareitem);
                int section = indexPath.Section;
                if (section % 2 != 0)
                    cell.BackgroundColor = UIColor.FromRGB(249, 249, 249);
                else
                    cell.BackgroundColor = UIColor.White;
                return cell;
            }

            string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            private async void checkFileLocalIsExist(BeanUser contact, UILabel label_cover, UIImageView image_view)
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

                                    image_view.Hidden = false;

                                    //kiem tra xong cap nhat lai avatar
                                    label_cover.Hidden = true;
                                    image_view.Hidden = false;
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
                                    image_view.Hidden = true;
                                    label_cover.Hidden = false;
                                });
                            }
                        });
                    }
                    else
                    {
                        openFile(filename, image_view);
                        image_view.Hidden = false;
                        label_cover.Hidden = true;
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

        public class Custom_ShareItemCell : UITableViewCell
        {
            string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            UILabel lbl_imgCover, lbl_note, lbl_title, lbl_subTitle;
            private bool isOdd;
            private UIImageView iv_avatar;
            BeanShareHistory shareItem;
            string currentWorkFlowID;

            public Custom_ShareItemCell(NSString cellID) : base(UITableViewCellStyle.Default, cellID)
            {
                Accessory = UITableViewCellAccessory.None;
                BackgroundColor = UIColor.Green;
            }

            public void UpdateCell(BeanShareHistory _shareItem)
            {
                shareItem = _shareItem;
                ViewConfiguration();
                LoadData();
            }

            private void ViewConfiguration()
            {
                //if (isOdd)
                //    ContentView.BackgroundColor = UIColor.White;
                //else
                //    ContentView.BackgroundColor = UIColor.FromRGB(250, 250, 250);

                iv_avatar = new UIImageView();
                iv_avatar.ContentMode = UIViewContentMode.ScaleAspectFill;
                iv_avatar.ClipsToBounds = true;
                iv_avatar.Layer.CornerRadius = 20;
                iv_avatar.Hidden = true;

                lbl_imgCover = new UILabel
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular),
                    TextColor = UIColor.White
                };
                lbl_imgCover.Layer.CornerRadius = 20;
                lbl_imgCover.ClipsToBounds = true;

                lbl_title = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 14f),
                    TextColor = UIColor.Black,
                    TextAlignment = UITextAlignment.Left,
                };

                lbl_subTitle = new UILabel()
                {
                    Font = UIFont.FromName("ArialMT", 12f),
                    TextColor = UIColor.FromRGB(94, 94, 94)
                };

                ContentView.AddSubviews(new UIView[] { iv_avatar, lbl_imgCover, lbl_title, lbl_subTitle });
            }

            private void LoadData()
            {
                try
                {
                    var conn = new SQLiteConnection(CmmVariable.M_DataPath, false);
                    lbl_title.Text = shareItem.UserName;
                    lbl_subTitle.Text = shareItem.UserPosition;

                    if (!string.IsNullOrEmpty(shareItem.UserId))
                    {
                        List<BeanUser> lst_userResult = new List<BeanUser>();
                        string query_user = string.Format("SELECT * FROM BeanUser WHERE ID =?");
                        lst_userResult = conn.Query<BeanUser>(query_user, shareItem.UserId);

                        string user_imagePath = "";
                        if (lst_userResult.Count > 0)
                            user_imagePath = lst_userResult[0].ImagePath;

                        if (string.IsNullOrEmpty(user_imagePath))
                        {

                            lbl_imgCover.Hidden = false;
                            iv_avatar.Hidden = true;
                            lbl_imgCover.Text = CmmFunction.GetAvatarName(lst_userResult[0].FullName);
                            lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));

                        }
                        else
                        {
                            lbl_imgCover.Hidden = false;
                            iv_avatar.Hidden = true;
                            lbl_imgCover.Text = CmmFunction.GetAvatarName(lst_userResult[0].FullName);
                            lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));

                            checkFileLocalIsExist(lst_userResult[0], lbl_imgCover, iv_avatar);

                            //kiem tra xong cap nhat lai avatar
                            lbl_imgCover.Hidden = true;
                            iv_avatar.Hidden = false;
                        }
                    }

                    //if (string.IsNullOrEmpty(shareItem.UserImagePath))
                    //{
                    //    lbl_imgCover.Hidden = false;
                    //    iv_avatar.Hidden = true;
                    //    lbl_imgCover.Text = CmmFunction.GetAvatarName(shareItem.UserName);
                    //    lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));
                    //}
                    //else
                    //{
                    //    lbl_imgCover.Hidden = false;
                    //    iv_avatar.Hidden = true;
                    //    lbl_imgCover.Text = CmmFunction.GetAvatarName(shareItem.UserName);
                    //    lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));
                    //    checkFileLocalIsExist(shareItem, lbl_imgCover, iv_avatar);
                    //}

                }
                catch (Exception ex)
                {
                    Console.WriteLine("MainView - Todo_cell_custom - loaddata- ERR: " + ex.ToString());
                }
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                iv_avatar.Frame = new CGRect(55, 5, 40, 40);
                lbl_imgCover.Frame = new CGRect(55, 5, 40, 40);
                lbl_title.Frame = new CGRect(lbl_imgCover.Frame.Right + 10, 10, ContentView.Frame.Width - 60, 20);
                lbl_subTitle.Frame = new CGRect(lbl_title.Frame.X, lbl_title.Frame.Bottom, 270, 20);
            }

            private async void checkFileLocalIsExist(BeanUser contact, UILabel label_cover, UIImageView image_view)
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

                                    image_view.Hidden = false;

                                    //kiem tra xong cap nhat lai avatar
                                    label_cover.Hidden = true;
                                    image_view.Hidden = false;
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
                                    image_view.Hidden = true;
                                    label_cover.Hidden = false;
                                });
                            }
                        });
                    }
                    else
                    {
                        openFile(filename, image_view);
                        image_view.Hidden = false;
                        label_cover.Hidden = true;
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

        #region table data source user
        private class Users_TableSource : UITableViewSource
        {
            List<BeanUser> lst_user;
            NSString cellIdentifier = new NSString("cellUser");
            FormShareView parentView;

            public Users_TableSource(List<BeanUser> _users, FormShareView _parentview)
            {
                parentView = _parentview;
                lst_user = _users;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return lst_user.Count;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 78;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                Custom_UserCell cell = new Custom_UserCell(cellIdentifier, parentView);
                var user = lst_user[indexPath.Row];

                bool isOdd = true;
                if (indexPath.Row % 2 == 0)
                    isOdd = false;

                cell.UpdateCell(user, true, isOdd);
                return cell;
            }
        }
        #endregion

        #region User Collection source
        public class User_CollectionSource : UICollectionViewSource
        {
            FormShareView parentView { get; set; }
            public static Dictionary<string, List<BeanUser>> indexedSession;
            public List<BeanUserAndGroup> items;

            public User_CollectionSource(FormShareView _parentview, List<BeanUserAndGroup> _items, List<KeyValuePair<string, bool>> _sectionState)
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
                UserGroup_CollectionCell userGroupCell = (UserGroup_CollectionCell)collectionView.DequeueReusableCell(User_CollectionCell.Key, indexPath);
                userGroupCell.UpdateRow(items[indexPath.Row], parentView);
                //userCell.ApplyLayoutAttributes
                return userGroupCell;
            }
        }

        private class User_CollectionFlowLayoutDelegate : UICollectionViewDelegateFlowLayout
        {
            FormShareView parent;
            User_CollectionSource collectionSource_user;
            nfloat titleWidth = 0, height = 0;
            BeanUserAndGroup user { get; set; }

            #region Constructors
            public User_CollectionFlowLayoutDelegate(User_CollectionSource _lst_user, FormShareView _parent)
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
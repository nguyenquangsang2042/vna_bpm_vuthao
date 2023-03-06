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
using Xamarin.iOS;

namespace BPMOPMobileV1.iOS.ViewControllers
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

        public FormShareView(IntPtr handle) : base(handle)
        {
        }

        #region override

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ViewConfiguration();
            LoadDataShare();
            //LoadContent(null);
            SetLangTitle();

            #region delegate
            BT_cancel.TouchUpInside += BT_cancel_TouchUpInside;
            BT_submit.TouchUpInside += BT_submit_TouchUpInside;
            BT_selectUser.TouchUpInside += BT_selectUser_TouchUpInside;
            #endregion
        }

        #endregion

        #region private  - public method
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
            view_shareItem.Hidden = true;
            //var model = DeviceHardware.Model;
            //if (model.Contains("X") || model.Contains("11"))
            //{
            //    headerView_constantHeight.Constant = 80;
            //}
            headerView_constantHeight.Constant = 45 + 10 + CmmIOSFunction.GetHeaderViewHeight();

            view_userSelected.Layer.BorderWidth = 0.5f;
            view_userSelected.Layer.BorderColor = UIColor.LightGray.ColorWithAlpha(0.5f).CGColor;
            textview_ykien.Layer.BorderColor = UIColor.LightGray.ColorWithAlpha(0.5f).CGColor;
            textview_ykien.Layer.BorderWidth = 0.5f;
            textview_ykien.Layer.CornerRadius = 5;

            User_CollectionView.Layer.BorderColor = UIColor.LightGray.CGColor;
            User_CollectionView.Layer.BorderWidth = 0;
            User_CollectionView.Layer.CornerRadius = 5;
            //User_CollectionView.RegisterClassForSupplementaryView(typeof(Custom_CollectionHeader), UICollectionElementKindSection.Header, Custom_CollectionHeader.Key);
            //Collection_ticket.RegisterClassForSupplementaryView(typeof(UIView), UICollectionElementKindSection.Footer, "");
            User_CollectionView.RegisterClassForCell(typeof(UserGroup_CollectionCell), UserGroup_CollectionCell.Key);

            var layoutStyle = new CollectionViewLayoutStyle();
            User_CollectionView.SetCollectionViewLayout(layoutStyle, true);
            User_CollectionView.AllowsMultipleSelection = false;

            view_userSelected.Frame = new CGRect(view_userSelected.Frame.X, view_userSelected.Frame.Y, view_userSelected.Frame.Width, 100);
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
                    table_shareList.Source = new ShareHistory_TableSource(lst_ShareHistory, this);
                    table_shareList.SeparatorStyle = UITableViewCellSeparatorStyle.None;
                    table_shareList.ReloadData();
                }
                else
                {
                    view_shareItem.Hidden = true;
                    lst_ShareHistory = new List<BeanShareHistory>();
                    table_shareList.Source = new ShareHistory_TableSource(lst_ShareHistory, this);
                    table_shareList.ReloadData();
                }
            });


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
                view_userselected_hehght_Constant.Constant = lines * 35;
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
            else
            {
                if (lst_userGroupSelected != null)
                {
                    lst_userGroupSelected.Clear();
                    User_CollectionView.ReloadData();
                }
            }
        }
        #endregion

        #region events
        private void BT_cancel_TouchUpInside(object sender, EventArgs e)
        {
            var contentinset = User_CollectionView.ContentSize;
            var res = User_CollectionView.CollectionViewLayout.CollectionViewContentSize.Height;

            if (this.NavigationController != null)
                this.NavigationController.PopViewController(true);
            else
                this.DismissModalViewController(true);

        }

        private void BT_selectUser_TouchUpInside(object sender, EventArgs e)
        {
            ListUserOrGroupView userGroup = (ListUserOrGroupView)Storyboard.InstantiateViewController("ListUserOrGroupView");
            userGroup.setContent(this, true, lst_userGroupSelected, false, null, CmmFunction.GetTitle("TEXT_TITLE_USERGROUP", "Chọn người hoặc nhóm"));

            //ListUserView userView = (ListUserView)Storyboard.InstantiateViewController("ListUserView");
            //userView.setContent(this, true, lst_userSelected, false, null, CmmFunction.GetTitle("TEXT_CONTROL_USERGROUP","Chọn người hoặc nhóm..."));
            this.NavigationController.PushViewController(userGroup, true);
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
                    string idea = textview_ykien.Text;
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
                        string str_errMess = string.Empty;
                        if (lstExtent != null && lstExtent.Count > 0)
                            result = providerControl.SendControlDynamicAction(_buttonAction.Value, WorkflowActionID, str_json_FormDefineInfo, "", ref str_errMess, null, lstExtent);
                        else
                            result = providerControl.SendControlDynamicAction(_buttonAction.Value, WorkflowActionID, str_json_FormDefineInfo, "", ref str_errMess, null);

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
        #endregion

        #region custom class

        #region Table source ShareItem

        private class ShareHistory_TableSource : UITableViewSource
        {
            NSString cellIdentifier = new NSString("cellShareItemID");
            FormShareView parentView;
            List<BeanShareHistory> lst_shareHistory { get; set; }
            Dictionary<BeanShareHistory, List<BeanShareHistory>> dict_shareitem = new Dictionary<BeanShareHistory, List<BeanShareHistory>>();
            List<BeanShareHistory> sectionKeys;

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
                    CGRect rect = StringExtensions.StringRect(item.Comment, UIFont.FromName("ArialMT", 14f), (parentView.table_shareList.Frame.Width / 5) * 4);
                    if (rect.Height > 0 && rect.Height <= 30)
                        rect.Height = 15;

                    return rect.Height + 60;
                }
                else
                    return 50;

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
                        Font = UIFont.FromName("ArialMT", 12f),
                        TextColor = UIColor.FromRGB(94, 94, 94)
                    };

                    UIImageView img_avatar = new UIImageView();
                    img_avatar.ContentMode = UIViewContentMode.ScaleAspectFill;

                    UILabel lbl_title = new UILabel()
                    {
                        Font = UIFont.FromName("Arial-BoldMT", 16f),
                        TextColor = UIColor.Black
                    };

                    UILabel lbl_avatar = new UILabel()
                    {
                        Font = UIFont.FromName("Arial-BoldMT", 16f),
                        TextColor = UIColor.White
                    };

                    UILabel lbl_sub_title = new UILabel()
                    {
                        Font = UIFont.FromName("ArialMT", 12f),
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

                    if (string.IsNullOrEmpty(key.UserImagePath))
                    {
                        lbl_avatar.Text = CmmFunction.GetAvatarName(key.UserName);
                        lbl_avatar.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_avatar.Text));
                    }
                    else
                    {
                        lbl_avatar.Hidden = false;
                        img_avatar.Hidden = true;
                        lbl_avatar.Text = CmmFunction.GetAvatarName(key.UserName);
                        lbl_avatar.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_avatar.Text));
                        checkFileLocalIsExist(key, lbl_avatar, img_avatar);
                        //kiem tra xong cap nhat lai avatar
                        lbl_avatar.Hidden = true;
                        img_avatar.Hidden = false;
                    }

                    lbl_title.Text = key.UserName;
                    lbl_sub_title.Text = key.UserPosition;
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
                    lbl_date.Frame = new CGRect(lbl_title.Frame.Right + 5, 10, (baseView.Frame.Width - img_avatar.Frame.Right) / 3, 20);
                    //lbl_date.Frame = new CGRect(baseView.Frame.Width - 120, 10, 110, 20);
                    lbl_sub_title.Frame = new CGRect(lbl_title.Frame.X, lbl_title.Frame.Bottom, (baseView.Frame.Width - lbl_title.Frame.X) - 10, 20);

                    if (!string.IsNullOrEmpty(key.Comment))
                    {
                        lbl_ykien.Hidden = false;
                        lbl_ykien.Frame = new CGRect(lbl_title.Frame.X, lbl_sub_title.Frame.Bottom, lbl_sub_title.Frame.Width, 20);
                        lbl_ykien.Text = key.Comment;
                        lbl_ykien.SizeToFit();
                    }

                    if (section % 2 != 0)
                        baseView.BackgroundColor = UIColor.White;
                    else
                        baseView.BackgroundColor = UIColor.FromRGB(243, 249, 255);

                    baseView.AddSubviews(new UIView[] { img_avatar, lbl_avatar, lbl_title, lbl_date, lbl_sub_title, lbl_ykien });

                    return baseView;
                }
                else
                    return null;

            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                //var item = dict_shareitem[sectionKeys[indexPath.Section]][indexPath.Row];
                //if (!string.IsNullOrEmpty(item.Comment))
                //{
                //    CGRect rect = StringExtensions.StringRect(item.Comment, UIFont.FromName("ArialMT", 14f), (parentView.table_shareList.Frame.Width / 5) * 3.2f);
                //    if (rect.Height > 0 && rect.Height < 20)
                //        rect.Height = 30;
                //    return rect.Height + 50;
                //}
                //else
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
                    cell.BackgroundColor = UIColor.White;
                else
                    cell.BackgroundColor = UIColor.FromRGB(243, 249, 255);

                return cell;
            }

            string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            private async void checkFileLocalIsExist(BeanShareHistory shareItem, UILabel label_cover, UIImageView image_view)
            {
                try
                {
                    string filename = shareItem.UserImagePath.Split('/').Last();
                    string filepathURL = CmmVariable.M_Domain + "/" + CmmVariable.SysConfig.Subsite + shareItem.UserImagePath;
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

                lbl_note = new UILabel()
                {
                    TextAlignment = UITextAlignment.Left,
                    Font = UIFont.FromName("ArialMT", 14f),
                    LineBreakMode = UILineBreakMode.WordWrap,
                    Lines = 0,
                    TextColor = UIColor.Black,
                    Hidden = false

                };

                lbl_note.ClipsToBounds = true;
                lbl_note.Layer.CornerRadius = 5;

                ContentView.AddSubviews(new UIView[] { iv_avatar, lbl_imgCover, lbl_title, lbl_subTitle });
            }

            private void LoadData()
            {
                try
                {
                    lbl_title.Text = shareItem.UserName;
                    lbl_subTitle.Text = shareItem.UserPosition;

                    if (!string.IsNullOrEmpty(shareItem.Comment))
                    {
                        lbl_note.Hidden = false;
                        //lbl_note.Frame = new CGRect(lbl_note.Frame.X, lbl_note.Frame.Bottom + 4, this.ContentView.Frame.Width - (lbl_note.Frame.X + 10), 30);
                        lbl_note.Text = shareItem.Comment;
                        lbl_note.SizeToFit();
                    }

                    if (string.IsNullOrEmpty(shareItem.UserImagePath))
                    {
                        lbl_imgCover.Hidden = false;
                        iv_avatar.Hidden = true;
                        lbl_imgCover.Text = CmmFunction.GetAvatarName(shareItem.UserName);
                        lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));
                    }

                    else
                    {
                        lbl_imgCover.Hidden = false;
                        iv_avatar.Hidden = true;
                        lbl_imgCover.Text = CmmFunction.GetAvatarName(shareItem.UserName);
                        lbl_imgCover.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_imgCover.Text));
                        checkFileLocalIsExist(shareItem, lbl_imgCover, iv_avatar);
                    }

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

                //if (!string.IsNullOrEmpty(shareItem.Comment))
                //{
                //    lbl_note.Hidden = false;
                //    lbl_note.Frame = new CGRect(lbl_subTitle.Frame.X, lbl_subTitle.Frame.Bottom + 4, this.ContentView.Frame.Width - (lbl_subTitle.Frame.X + 10), 30);
                //    lbl_note.Text = shareItem.Comment;
                //    lbl_note.SizeToFit();
                //}

                //lbl_date.Frame = new CGRect(ContentView.Frame.Width - 130, lbl_title.Frame.Bottom, 110, 20);
                //var widthStatus = StringExtensions.MeasureString(lbl_status.Text, 12).Width + 20;
                //var maxStatusWidth = ContentView.Frame.Width - 180;
                //if (widthStatus < maxStatusWidth)
                //    lbl_status.Frame = new CGRect(lbl_subTitle.Frame.X, lbl_subTitle.Frame.Bottom + 5, widthStatus, 20);
                //else
                //    lbl_status.Frame = new CGRect(lbl_subTitle.Frame.X, lbl_subTitle.Frame.Bottom + 5, maxStatusWidth, 20);

            }

            private async void checkFileLocalIsExist(BeanShareHistory shareItem, UILabel label_cover, UIImageView image_view)
            {
                try
                {
                    string filename = shareItem.UserId + "_Avatar.jpg";//?ver=" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    //string filepathURL = CmmVariable.M_Domain + "/" + CmmVariable.SysConfig.Subsite + contact.ImagePath + "?ver=" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    string filepathURL = CmmVariable.M_Domain + "/" + CmmVariable.SysConfig.Subsite + shareItem.UserImagePath;
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


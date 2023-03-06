using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobileV1.iOS.IOSClass;
using Foundation;
using SQLite;
using UIKit;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class RequestAddInfo : UIViewController
    {
        string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        UIViewController parentView { get; set; }
        BeanUser userSelected = null;
        List<BeanUser> lst_user_select = new List<BeanUser>();
        List<BeanUser> lst_user = new List<BeanUser>();
        List<BeanQuaTrinhLuanChuyen> lst_qtlc { get; set; }
        public List<BeanUser> lst_userInWorkFlow;
        BeanWorkflowItem workflowItem { get; set; }
        BeanTicketRequest ticketRequest { get; set; }
        string hintDefault = CmmFunction.GetTitle("TEXT_HINT_REQUIRE_COMMENT", "Nhập ý kiến");
        ButtonAction control_action;
        private UITapGestureRecognizer gestureRecognizer;

        //list extend những value add thêm không dynamic control (ghi chú, ý kiến, người được chuyển xử lý...)
        // nội dung, ý kiến: key = "idea"
        // user, người xử lý: key = "userValues"
        private List<KeyValuePair<string, string>> lstExtent = new List<KeyValuePair<string, string>>();

        public RequestAddInfo(IntPtr handle) : base(handle)
        {
        }

        #region override

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            //ShowUserList(false);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            gestureRecognizer = new UITapGestureRecognizer(Self, new ObjCRuntime.Selector("hideKeyboard"));
            gestureRecognizer.ShouldReceiveTouch += delegate (UIGestureRecognizer recognizer, UITouch touch)
            {
                return true;
            };
            this.View.AddGestureRecognizer(gestureRecognizer);


            ViewConfiguration();
            loadContent();

            if (userSelected != null)
            {
                lbl_email_userSelected.Hidden = false;
                lbl_char_title_userSelected.Hidden = false;
                lbl_title_userSelected.Hidden = false;
                lbl_chonnguoixuly.Hidden = true;
                view_userSelected.Layer.BorderWidth = 1;
            }
            else
            {
                view_userSelected.Layer.BorderWidth = 0;
                lbl_email_userSelected.Hidden = true;
                lbl_char_title_userSelected.Hidden = true;
                lbl_title_userSelected.Hidden = true;
                lbl_chonnguoixuly.Hidden = false;

            }
            setlangTitle();

            #region delegate
            BT_cancel.TouchUpInside += BT_cancel_TouchUpInside;
            textview_ykien.Started += Textview_ykien_Started;
            textview_ykien.Ended += Textview_ykien_Ended;
            BT_selectUser.TouchUpInside += BT_selectUser_TouchUpInside;
            BT_agree.TouchUpInside += BT_agree_TouchUpInside;
            #endregion
        }

        #endregion

        #region private - public method
        public void setContent(UIViewController _parent, ButtonAction _controlAction, List<BeanQuaTrinhLuanChuyen> _lst_qtlc, BeanWorkflowItem _worklowItem, BeanTicketRequest _ticketRequest)
        {
            parentView = _parent;
            control_action = _controlAction;
            lst_qtlc = _lst_qtlc;
            workflowItem = _worklowItem;
            ticketRequest = _ticketRequest;
        }

        private void setlangTitle()
        {
            CmmIOSFunction.SetLangToView(this.View);
        }

        private void ViewConfiguration()
        {

            lbl_title.Text = control_action.Title;
            
            view_userSelected.Layer.BorderWidth = 1;
            view_userSelected.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            view_userSelected.Layer.CornerRadius = 4;
            view_userSelected.ClipsToBounds = true;

            lbl_chonnguoixuly.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            lbl_chonnguoixuly.Layer.BorderWidth = 1;
            lbl_chonnguoixuly.Layer.CornerRadius = 4;

            lbl_chonnguoixuly.Text = " " + CmmFunction.GetTitle("TEXT_CONTROL_CHOOSE_USERS", "Chọn người...");

            textview_ykien.Text = hintDefault;
            textview_ykien.Font = UIFont.FromName("Arial-ItalicMT", 14);
            textview_ykien.TextColor = UIColor.FromRGB(153,153,153);

            textview_ykien.Layer.BorderWidth = 1;
            textview_ykien.Layer.BorderColor = UIColor.FromRGB(229,229,229).CGColor;
            textview_ykien.ClipsToBounds = true;


        }

        private void loadContent()
        {
            lst_userInWorkFlow = new List<BeanUser>();
            var conn = new SQLiteConnection(CmmVariable.M_DataPath);
            if (lst_qtlc != null && lst_qtlc.Count > 0)
            {
                var currentstep = workflowItem.WorkflowStep;
                string currentuser_id = CmmVariable.SysConfig.UserId;
                foreach (var item in lst_qtlc)
                {
                    BeanUser user = null;
                    if (!string.IsNullOrEmpty(item.AssignUserId))
                    {
                        //string user_id = item.PersonAccount.Split(new string[] { ";#" }, StringSplitOptions.None)[0];
                        string user_id = item.AssignUserId;
                        if (user_id != currentuser_id && item.Step != workflowItem.Step)
                        {
                            string query = string.Format("SELECT * FROM BeanUser WHERE ID = ?");
                            var lst_res = conn.Query<BeanUser>(query, user_id);

                            if (lst_res != null && lst_res.Count > 0)
                                user = lst_res[0];
                        }
                    }

                    if (user != null)
                    {
                        if (!lst_userInWorkFlow.Any(i => i.ID == user.ID))
                            lst_userInWorkFlow.Add(user);
                    }
                }

                //ShowUserList(false);
            }
        }
        [Export("hideKeyboard")]
        private void hideKeyboard()
        {

            this.View.EndEditing(true);
        }
        public void selectedUser(BeanUser _user)
        {
            userSelected = _user;
            lst_user_select = new List<BeanUser>();
            lst_user_select.Add(userSelected);

            this.View.EndEditing(true);

            lbl_email_userSelected.Hidden = false;
            lbl_char_title_userSelected.Hidden = false;
            lbl_title_userSelected.Hidden = false;
            lbl_chonnguoixuly.Hidden = true;
            view_userSelected.Layer.BorderWidth = 1;

            lbl_title_userSelected.Text = userSelected.Name;
            if (string.IsNullOrEmpty(userSelected.ImagePath))
            {
                if (!string.IsNullOrEmpty(userSelected.FullName))
                {
                    lbl_char_title_userSelected.Hidden = false;
                    img_avatar.Hidden = true;
                    lbl_char_title_userSelected.Text = CmmFunction.GetAvatarName(userSelected.FullName);
                    lbl_char_title_userSelected.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_char_title_userSelected.Text));
                }

            }
            else
            {
                checkFileLocalIsExist(userSelected, img_avatar);
                lbl_char_title_userSelected.Hidden = true;
            }

            lbl_email_userSelected.Text = userSelected.Email;

            textview_ykien.Text = string.Empty;
            textview_ykien.BecomeFirstResponder();
        }

        private bool validateControlValue()
        {
            if (string.IsNullOrEmpty(textview_ykien.Text))
            {
                CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("MESS_REQUIRE_COMMENT", "Vui lòng nhập ý kiến..."));
                return false;
            }

            if (userSelected == null)
            {
                CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("MESS_REQUIRE_USER", "Vui lòng chọn người xử lý..."));
                return false;
            }

            return true;
        }

        private void ShowUserList(bool isAnimation)
        {
            PresentationDelegate transitioningDelegate;
            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);

            ListUserView user_choice = (ListUserView)Storyboard.InstantiateViewController("ListUserView");
            user_choice.setContent(this, false, lst_user_select, true, null, CmmFunction.GetTitle("TEXT_CONTROL_CHOOSE_USERS", "Chọn người..."));
            transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            user_choice.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            user_choice.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            user_choice.TransitioningDelegate = transitioningDelegate;
            this.PresentViewControllerAsync(user_choice, isAnimation);
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

                                img_avatar.Hidden = false;
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
                                img_avatar.Hidden = false;
                            });
                        }
                    });
                }
                else
                {
                    openFile(filename, image_view);
                    img_avatar.Hidden = false;
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
        #endregion

        #region events
        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            this.DismissViewControllerAsync(true);
        }
        private void BT_cancel_TouchUpInside(object sender, EventArgs e)
        {
            this.DismissViewControllerAsync(true);
        }
        private void BT_agree_TouchUpInside(object sender, EventArgs e)
        {
            if (validateControlValue())
            {
                KeyValuePair<string, string> note = new KeyValuePair<string, string>("idea", textview_ykien.Text);
                lstExtent.Add(note);

                KeyValuePair<string, string> user = new KeyValuePair<string, string>("userValues", userSelected.AccountName);
                lstExtent.Add(user);

                if (parentView.GetType() == typeof(RequestDetailsV2))
                {
                    RequestDetailsV2 requestDetailsV2 = parentView as RequestDetailsV2;
                    requestDetailsV2.SubmitAction(control_action, lstExtent);
                }

                this.DismissModalViewController(true);
            }
        }
        private void Textview_ykien_Ended(object sender, EventArgs e)
        {
            if (textview_ykien.Text == "")
            {
                textview_ykien.Text = hintDefault;
                textview_ykien.Font = UIFont.FromName("Arial-ItalicMT", 14);
                textview_ykien.TextColor = UIColor.FromRGB(153,153,153);
            }
        }
        private void Textview_ykien_Started(object sender, EventArgs e)
        {
            if (textview_ykien.Text == hintDefault || string.IsNullOrEmpty(textview_ykien.Text))
            {
                textview_ykien.Font = UIFont.FromName("ArialMT", 14);
                textview_ykien.TextColor = UIColor.Black;
                textview_ykien.Text = "";
            }
        }
        private void BT_selectUser_TouchUpInside(object sender, EventArgs e)
        {
            ShowUserList(true);
        }
        #endregion

        #region custom class
        #region table data source user
        private class users_TableSource : UITableViewSource
        {
            List<BeanUser> lst_user;
            BeanUser userSelected { get; set; }
            NSString cellIdentifier = new NSString("cell");
            RequestAddInfo parentView;

            public users_TableSource(List<BeanUser> _user, BeanUser _userSelected, RequestAddInfo _parentview)
            {
                parentView = _parentview;
                userSelected = _userSelected;
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
                item.IsSelected = true;
                parentView.selectedUser(item);
                tableView.DeselectRow(indexPath, true);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                userTCT_cell_custom cell = new userTCT_cell_custom(cellIdentifier);
                var user = lst_user[indexPath.Row];

                if (userSelected != null && userSelected == user)
                    user.IsSelected = true;
                else
                    user.IsSelected = false;

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

                if (!string.IsNullOrEmpty(user.FullName))
                    lbl_imgCover.Text = user.FullName.Substring(0, 1);

                lbl_name.Text = user.FullName;
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

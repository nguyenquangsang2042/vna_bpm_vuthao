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
using System.Threading.Tasks;
using System.IO;
using BPMOPMobile.DataProvider;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class ChangeUserProgress : UIViewController
    {
        string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        UIViewController parentView { get; set; }
        BeanUser userSelected = null;
        List<BeanUser> lst_user = new List<BeanUser>();
        List<BeanUser> lst_user_select = new List<BeanUser>();
        List<BeanUser> lst_contact_result = new List<BeanUser>();
        string hintDefault = CmmFunction.GetTitle("TEXT_HINT_REQUIRE_COMMENT", "Nhập ý kiến");
        ButtonAction control_action;
        private UITapGestureRecognizer gestureRecognizer;

        //list extend những value add thêm không dynamic control (ghi chú, ý kiến, người được chuyển xử lý...)
        // nội dung, ý kiến: key = "idea"
        // user, người xử lý: key = "userValues"
        private List<KeyValuePair<string, string>> lstExtent = new List<KeyValuePair<string, string>>();

        public ChangeUserProgress(IntPtr handle) : base(handle)
        {
        }

        #region override

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewConfiguration();
            gestureRecognizer = new UITapGestureRecognizer(Self, new ObjCRuntime.Selector("hideKeyboard"));
            gestureRecognizer.ShouldReceiveTouch += delegate (UIGestureRecognizer recognizer, UITouch touch)
            {
                    return true;
            };
            this.View.AddGestureRecognizer(gestureRecognizer);

            LoadContent();
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
            
            textview_ykien.Started += Textview_ykien_Started;
            textview_ykien.Ended += Textview_ykien_Ended;
            BT_selectUser.TouchUpInside += BT_selectUser_TouchUpInside;
            BT_agree.TouchUpInside += BT_agree_TouchUpInside;
            BT_cancel.TouchUpInside += BT_cancel_TouchUpInside;
            #endregion
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            //ShowUserList(false);
        }

        #endregion

        #region private -public method
        public void setContent(UIViewController _parent, ButtonAction _controlAction)
        {
            parentView = _parent;
            control_action = _controlAction;
        }

        private void setlangTitle()
        {
            CmmIOSFunction.SetLangToView(this.View);
        }

        private void ViewConfiguration()
        {
            if (control_action.ID == 9) // tham van y kien
            {
                img_action.Image = UIImage.FromFile("Icons/icon_Btn_action_9.png");
                lbl_chonnguoixuly.Text = " " + CmmFunction.GetTitle("TEXT_CONTROL_CHOOSE_USERS", "Chọn người...");
            }
            else // chuyen xu ly
            {
                img_action.Image = UIImage.FromFile("Icons/icon_Btn_action_3.png");
                lbl_chonnguoixuly.Text = " " + CmmFunction.GetTitle("TEXT_CONTROL_CHOOSE_USERS", "Chọn người...");
            }

            lbl_title.Text = control_action.Title;
            
            view_userSelected.Layer.BorderWidth = 1;
            view_userSelected.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            view_userSelected.Layer.CornerRadius = 4;
            view_userSelected.ClipsToBounds = true;

            lbl_chonnguoixuly.Layer.BorderColor = UIColor.FromRGB(229,229,229).CGColor;
            lbl_chonnguoixuly.Layer.BorderWidth = 1;
            lbl_chonnguoixuly.Layer.CornerRadius = 4;

            textview_ykien.Text = hintDefault;
            textview_ykien.Font = UIFont.FromName("Arial-ItalicMT", 14);
            textview_ykien.TextColor = UIColor.FromRGB(153, 153, 153);

            textview_ykien.Layer.BorderWidth = 0.5f;
            textview_ykien.Layer.BorderColor = UIColor.LightGray.CGColor;
            textview_ykien.ClipsToBounds = true;

            //BT_send.ImageEdgeInsets = new UIEdgeInsets(8, 8, 8, 8);
            //BT_cancleAssign.Hidden = true;

        }

        private void LoadContent()
        {
            try
            {
                lbl_title.Text = control_action.Title;
                
                //ShowUserList(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ChangeUserProgress - LoadContent - Err: " + ex.ToString());
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
            this.View.EndEditing(true);

            lbl_email_userSelected.Hidden = false;
            lbl_char_title_userSelected.Hidden = false;
            lbl_title_userSelected.Hidden = false;
            lbl_chonnguoixuly.Hidden = true;
            view_userSelected.Layer.BorderWidth = 1;

            lbl_title_userSelected.Text = userSelected.FullName;
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
                CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("MESS_REQUIRE_COMMENT", "Vui lòng nhập ý kiến."));
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
            user_choice.setContent(this, false, lst_user_select, false, null, CmmFunction.GetTitle("TEXT_CONTROL_CHOOSE_USERS", "Chọn người..."));
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
        private void Textview_ykien_Ended(object sender, EventArgs e)
        {
            if (textview_ykien.Text == "")
            {
                textview_ykien.Text = hintDefault;
                textview_ykien.Font = UIFont.FromName("Arial-ItalicMT", 14);
                textview_ykien.TextColor = UIColor.FromRGB(153, 153, 153);
            }
        }
        private void Textview_ykien_Started(object sender, EventArgs e)
        {
            if (textview_ykien.Text == hintDefault)
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

                //KeyValuePair<string, string> user = new KeyValuePair<string, string>("userValues", userSelected.UserId + ";#" + userSelected.Name);
                KeyValuePair<string, string> user = new KeyValuePair<string, string>("userValues", userSelected.AccountName);
                lstExtent.Add(user);

                if(parentView.GetType() == typeof(RequestDetailsV2))
                {
                    RequestDetailsV2 requestDetailsV2 = parentView as RequestDetailsV2;
                    requestDetailsV2.SubmitAction(control_action, lstExtent);
                }

                this.DismissModalViewController(true);
            }
        }

        private void Searchbar_TextChanged(object sender, UISearchBarTextChangedEventArgs e)
        {
            try
            {
                //string content = CmmFunction.removeSignVietnamese(searchbar.Text.Trim().ToLowerInvariant());
                //if (!string.IsNullOrEmpty(content))
                //{
                //    var items = from item in lst_user
                //                where ((!string.IsNullOrEmpty(item.Name) && CmmFunction.removeSignVietnamese(item.Name.ToLowerInvariant()).Contains(content)) ||
                //                           (!string.IsNullOrEmpty(item.Email) && item.Email.ToLowerInvariant().Contains(content)))
                //                orderby item.Name
                //                select item;

                //    if (items != null && items.Count() > 0)
                //    {
                //        lst_contact_result = items.ToList();
                        //table_user.Alpha = 1;
                        //table_user.Source = new users_TableSource(lst_contact_result, userSelected, this);
                        //table_user.ReloadData();
                    //}
                    //else
                        //table_user.Alpha = 0;
                //}
                //else
                //{
                //    //table_user.Alpha = 1;
                //    //table_user.Source = new users_TableSource(lst_user, userSelected, this);
                //    //table_user.ReloadData();
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine("AssignedToView - SearchBar_user_TextChanged - Err: " + ex.ToString());
            }

        }
        #endregion

        #region custom class

        #endregion
    }
}


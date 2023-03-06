using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.iPad.IOSClass;
using BPMOPMobile.iPad.ViewControllers;
using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UIKit;

namespace BPMOPMobile.iPad
{
    public partial class FormTransferHandleView : UIViewController
    {
        string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        private NSObject _willResignActiveNotificationObserver;
        private NSObject _didBecomeActiveNotificationObserver;
        private nfloat positionBotOfCurrentViewInput { get; set; }
        List<BeanUser> lst_user_select = new List<BeanUser>();
        string hintDefault = CmmFunction.GetTitle("TEXT_HINT_REQUIRE_COMMENT", "Nhập ý kiến");
        //list extend những value add thêm không dynamic control (ghi chú, ý kiến, người được chuyển xử lý...)
        // nội dung, ý kiến: key = "idea"
        // user, người xử lý: key = "userValues"
        private List<KeyValuePair<string, string>> lstExtent = new List<KeyValuePair<string, string>>();
        UIViewController parentView { get; set; }
        BeanUser userSelected = null;
        /// <summary>
        /// typeAction => 0 : chuyển xử lý | 1 : tham vấn ý kiến
        /// </summary>
        private ButtonAction control_action { get; set; }

        private BeanUser currentUserSelected { get; set; }

        public FormTransferHandleView(IntPtr handle) : base(handle)
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
            LoadContent();

            if (userSelected != null)
            {
                lbl_email_userSelected.Hidden = false;
                lbl_char_title_userSelected.Hidden = false;
                lbl_title_userSelected.Hidden = false;
                imgAvatar.Hidden = false;
                lbl_chonnguoixuly.Hidden = true;
                view_userSelected.Layer.BorderWidth = 1;
            }
            else
            {
                view_userSelected.Layer.BorderWidth = 0;
                lbl_email_userSelected.Hidden = true;
                lbl_char_title_userSelected.Hidden = true;
                lbl_title_userSelected.Hidden = true;
                imgAvatar.Hidden = true;
                lbl_chonnguoixuly.Hidden = false;

            }

            setlangTitle();

            #region delegate
            BT_close.TouchUpInside += BT_close_TouchUpInside;
            BT_approve.TouchUpInside += BT_approve_TouchUpInside;
            BT_selectUser.TouchUpInside += BT_user_TouchUpInside;
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
        private void ViewConfiguration()
        {
            BT_close.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);
            BT_approve.ContentEdgeInsets = new UIEdgeInsets(7, 5, 7, 5);

            lbl_userTitle.Text = CmmFunction.GetTitle("TEXT_TITLE_USERGROUP", "Chọn người hoặc nhóm") + " (*)";
            lbl_chonnguoixuly.Text = " " + CmmFunction.GetTitle("TEXT_CONTROL_CHOOSE_USERS", "Chọn người...");

            string idea = CmmFunction.GetTitle("TEXT_SHARE_IDEA", "Ý kiến");
            lbl_noteTitle.Text = idea + " (*)";
            CmmIOSFunction.AddAttributeTitle(lbl_noteTitle);

            if (CmmVariable.SysConfig.LangCode == "1033")
                lbl_title.Text = control_action.Value;
            else //if (CmmVariable.SysConfig.LangCode == "1066")
                lbl_title.Text = control_action.Title;

            view_userSelected.Layer.BorderWidth = 1;
            view_userSelected.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            view_userSelected.Layer.CornerRadius = 4;
            view_userSelected.ClipsToBounds = true;

            lbl_chonnguoixuly.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            lbl_chonnguoixuly.Layer.BorderWidth = 1;
            lbl_chonnguoixuly.Layer.CornerRadius = 4;

            textview_ykien.Text = hintDefault;
            textview_ykien.Font = UIFont.FromName("Arial-ItalicMT", 14);
            textview_ykien.TextColor = UIColor.FromRGB(153, 153, 153);

            textview_ykien.Layer.BorderWidth = 0.5f;
            textview_ykien.Layer.BorderColor = UIColor.LightGray.CGColor;
            textview_ykien.ClipsToBounds = true;

            CmmIOSFunction.AddAttributeTitle(lbl_userTitle);
            CmmIOSFunction.AddAttributeTitle(lbl_noteTitle);
        }

        private void LoadContent()
        {
            //if (typeAction == 1)// tham vấn ý kiến
            //{
            //    lbl_title.Text = "Tham vấn ý kiến";
            //    lbl_userTitle.Text = "Người được tham vấn ý kiến (*)";
            //    CmmIOSFunction.AddAttributeTitle(lbl_userTitle);
            //}
        }

        /// <summary>
        /// typeAction => 0 : chuyển xử lý | 1 : tham vấn ý kiến
        /// </summary>
        /// <param name="_parentView"></param>
        /// <param name="_typeAction">0 : chuyển xử lý | 1 : tham vấn ý kiến</param>
        public void SetContent(UIViewController _parentView, ButtonAction _typeAction)
        {
            parentView = _parentView;
            control_action = _typeAction;
        }

        private void setlangTitle()
        {
            CmmIOSFunction.SetLangToView(this.View);
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

        private List<string> GetListIdUserSelected()
        {
            if (currentUserSelected != null)
            {
                List<string> lst_result = new List<string>();
                lst_result.Add(currentUserSelected.ID);

                return lst_result;
            }
            else
                return null;
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

        public void HandleAddUserAndGroupResult(List<BeanUser> _users)
        {
            if (_users.Count > 0)
            {
                currentUserSelected = _users[0];
                //tf_user.Text = _users[0].Name;
            }
        }

        public void selectedUser(BeanUser _user)
        {
            userSelected = _user;
            this.View.EndEditing(true);

            lbl_email_userSelected.Hidden = false;
            lbl_char_title_userSelected.Hidden = false;
            lbl_title_userSelected.Hidden = false;
            lbl_chonnguoixuly.Hidden = true;
            imgAvatar.Hidden = true;
            view_userSelected.Layer.BorderWidth = 1;

            lbl_title_userSelected.Text = userSelected.FullName;

            if (string.IsNullOrEmpty(userSelected.ImagePath))
            {
                if (!string.IsNullOrEmpty(userSelected.FullName))
                {
                    lbl_char_title_userSelected.Hidden = false;
                    imgAvatar.Hidden = true;
                    lbl_char_title_userSelected.Text = CmmFunction.GetAvatarName(userSelected.FullName);
                    lbl_char_title_userSelected.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_char_title_userSelected.Text));
                }
            }
            else
            {
                checkFileLocalIsExist(userSelected, imgAvatar);
                lbl_char_title_userSelected.Hidden = true;
            }
            //if (!string.IsNullOrEmpty(userSelected.FullName))
            //{
            //    lbl_char_title_userSelected.Hidden = false;
            //    lbl_char_title_userSelected.Text = CmmFunction.GetAvatarName(userSelected.FullName);
            //    lbl_char_title_userSelected.BackgroundColor = ExtensionMethods.ToUIColor(CmmIOSFunction.GetDynamicColorAvatar(lbl_char_title_userSelected.Text));
            //}
            //else
            //{
            //    lbl_char_title_userSelected.Hidden = false;
            //}
            lbl_email_userSelected.Text = userSelected.Position;

            textview_ykien.Text = string.Empty;
            //textview_ykien.BecomeFirstResponder();
        }

        private async void checkFileLocalIsExist(BeanUser contact, UIImageView image_view)
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
                                image_view.Image = UIImage.FromFile("Icons/icon_avatar32.png");
                                imgAvatar.Hidden = false;
                            });
                        }
                    });
                }
                else
                {
                    openFile(filename, image_view);
                    image_view.Hidden = false;
                    lbl_char_title_userSelected.Hidden = true;
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

        #region event
        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            this.DismissModalViewController(true);
        }

        private void BT_user_TouchUpInside(object sender, EventArgs e)
        {
            CGRect startFrame = new CGRect(this.View.Frame.X, this.View.Frame.Height, this.View.Bounds.Width, this.View.Bounds.Height);
            CGSize showSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
            FormUsersView formUsersView = (FormUsersView)Storyboard.InstantiateViewController("FormUsersView");
            formUsersView.SetContent(this, false, lst_user_select, false, null, CmmFunction.GetTitle("TEXT_CONTROL_CHOOSE_USERS", "Chọn người..."));
            PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            formUsersView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            formUsersView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            formUsersView.TransitioningDelegate = transitioningDelegate;
            this.PresentViewControllerAsync(formUsersView, true);
        }

        private void BT_approve_TouchUpInside(object sender, EventArgs e)
        {
            if (validateControlValue())
            {
                KeyValuePair<string, string> note = new KeyValuePair<string, string>("idea", textview_ykien.Text);
                lstExtent.Add(note);

                //KeyValuePair<string, string> user = new KeyValuePair<string, string>("userValues", userSelected.UserId + ";#" + userSelected.Name);
                KeyValuePair<string, string> user = new KeyValuePair<string, string>("userValues", userSelected.AccountName);
                lstExtent.Add(user);

                if (parentView.GetType() == typeof(ToDoDetailView))
                {
                    ToDoDetailView toDoDetail = parentView as ToDoDetailView;
                    toDoDetail.SubmitAction(control_action, lstExtent);
                }
                else if (parentView.GetType() == typeof(WorkflowDetailView))
                {
                    WorkflowDetailView toDoDetail = parentView as WorkflowDetailView;
                    toDoDetail.SubmitAction(control_action, lstExtent);
                }
                else if (parentView.GetType() == typeof(FormWorkFlowDetails))
                {
                    FormWorkFlowDetails toDoDetail = parentView as FormWorkFlowDetails;
                    toDoDetail.SubmitAction(control_action, lstExtent);
                }
                else if (parentView.GetType() == typeof(FollowListViewController))
                {
                    FollowListViewController toDoDetail = parentView as FollowListViewController;
                    toDoDetail.SubmitAction(control_action, lstExtent);
                }

                this.DismissModalViewController(true);
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
            { Console.WriteLine("FormTransferHandleView - Err: " + ex.ToString()); }
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
            { Console.WriteLine("FormTransferHandleView - Err: " + ex.ToString()); }
        }
        #endregion
    }
}
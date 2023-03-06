using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.iPad.CustomControlClass;
using BPMOPMobile.iPad.IOSClass;
using BPMOPMobile.iPad.ViewControllers;
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
    public partial class FormAdditionalInformationView : UIViewController
    {
        string localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        private List<KeyValuePair<string, string>> lstExtent = new List<KeyValuePair<string, string>>();
        private NSObject _willResignActiveNotificationObserver;
        private NSObject _didBecomeActiveNotificationObserver;
        private nfloat positionBotOfCurrentViewInput { get; set; }
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

        public FormAdditionalInformationView(IntPtr handle) : base(handle)
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

            #region delegate
            BT_close.TouchUpInside += BT_close_TouchUpInside;
            BT_selectUser.TouchUpInside += BT_user_TouchUpInside;
            BT_approve.TouchUpInside += BT_approve_TouchUpInside;
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

        public void SetContent(UIViewController _parent, ButtonAction _controlAction, List<BeanQuaTrinhLuanChuyen> _lst_qtlc, BeanWorkflowItem _worklowItem, BeanTicketRequest _ticketRequest)
        {
            parentView = _parent;
            control_action = _controlAction;
            lst_qtlc = _lst_qtlc;
            workflowItem = _worklowItem;
            ticketRequest = _ticketRequest;
        }

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
            else  //if (CmmVariable.SysConfig.LangCode == "1066")
                lbl_title.Text = control_action.Title;

            view_userSelected.Layer.BorderWidth = 1;
            view_userSelected.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            view_userSelected.Layer.CornerRadius = 4;
            view_userSelected.ClipsToBounds = true;

            lbl_chonnguoixuly.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            lbl_chonnguoixuly.Layer.BorderWidth = 1;
            lbl_chonnguoixuly.Layer.CornerRadius = 4;

            tv_note.Text = hintDefault;
            tv_note.Font = UIFont.FromName("Arial-ItalicMT", 14);
            tv_note.TextColor = UIColor.FromRGB(153, 153, 153);

            tv_note.Layer.BorderWidth = 0.5f;
            tv_note.Layer.BorderColor = UIColor.LightGray.CGColor;
            tv_note.ClipsToBounds = true;

            CmmIOSFunction.AddAttributeTitle(lbl_userTitle);
            CmmIOSFunction.AddAttributeTitle(lbl_noteTitle);
        }

        private void LoadContent()
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

        private bool validateControlValue()
        {
            if (string.IsNullOrEmpty(tv_note.Text))
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

            lbl_email_userSelected.Text = userSelected.Email;

            tv_note.Text = string.Empty;
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
            FormUsersView formUserView = (FormUsersView)Storyboard.InstantiateViewController("FormUsersView");
            formUserView.SetContent(this, false, lst_user_select, true, null, CmmFunction.GetTitle("TEXT_CONTROL_CHOOSE_USERS", "Chọn người..."));
            PresentationDelegate transitioningDelegate = new PresentationDelegate(startFrame, showSize, 0);
            formUserView.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            formUserView.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            formUserView.TransitioningDelegate = transitioningDelegate;
            this.PresentViewControllerAsync(formUserView, true);
        }

        private void BT_approve_TouchUpInside(object sender, EventArgs e)
        {
            if (validateControlValue())
            {
                KeyValuePair<string, string> note = new KeyValuePair<string, string>("idea", tv_note.Text);
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
                    FollowListViewController followListViewController = parentView as FollowListViewController;
                    followListViewController.SubmitAction(control_action, lstExtent);
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
            { Console.WriteLine("FormAdditionalInformationView - Err: " + ex.ToString()); }
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
            { Console.WriteLine("FormAdditionalInformationView - Err: " + ex.ToString()); }
        }
        #endregion
    }
}
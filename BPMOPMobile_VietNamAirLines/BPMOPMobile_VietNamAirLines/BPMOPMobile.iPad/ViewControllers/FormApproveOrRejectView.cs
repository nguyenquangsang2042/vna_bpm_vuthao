using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.iPad.IOSClass;
using BPMOPMobile.iPad.ViewControllers;
using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;

namespace BPMOPMobile.iPad
{
    public partial class FormApproveOrRejectView : UIViewController
    {
        private NSObject _willResignActiveNotificationObserver;
        private NSObject _didBecomeActiveNotificationObserver;
        private nfloat positionBotOfCurrentViewInput { get; set; }
        //list extend những value add thêm không dynamic control (ghi chú, ý kiến, người được chuyển xử lý...)
        // nội dung, ý kiến: key = "idea"
        // user, người xử lý: key = "userValues"
        private List<KeyValuePair<string, string>> lstExtent = new List<KeyValuePair<string, string>>();
        string hintDefault = CmmFunction.GetTitle("TEXT_HINT_REQUIRE_COMMENT", "ý kiến...");
        private UIViewController parentView { get; set; }
        /// <summary>
        /// typeAction => 0 : approve | 1 : reject | 2 : cancel
        /// </summary>
        private ButtonAction control_action { get; set; }

        public FormApproveOrRejectView(IntPtr handle) : base(handle)
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

            #region delegate
            BT_close.TouchUpInside += BT_close_TouchUpInside;
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
        private void ViewConfiguration()
        {
            BT_close.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);
            BT_approve.ContentEdgeInsets = new UIEdgeInsets(7, 5, 7, 5);

            CmmIOSFunction.AddBorderView(view_note);

            string idea = CmmFunction.GetTitle("TEXT_SHARE_IDEA", "Ý kiến");
            if (control_action.ID == 1 || control_action.ID == 2) // duyet - khong can check y kien
                lbl_noteTitle.Text = idea;
            else
            {
                lbl_noteTitle.Text = idea + " (*)";
                CmmIOSFunction.AddAttributeTitle(lbl_noteTitle);
            }
        }

        private void LoadContent()
        {
            if (CmmVariable.SysConfig.LangCode == "1033")
                lbl_title.Text = control_action.Value;
            else //if (CmmVariable.SysConfig.LangCode == "1066")
                lbl_title.Text = control_action.Title;

            if (control_action.Notes != null)
            {
                ObjectElementNote objectElementNote = control_action.Notes.SingleOrDefault();
                lbl_note.TextColor = UIColor.Orange;
                lbl_note.Text = CmmFunction.GetTitle(objectElementNote.Key, "Phiếu đang chờ tham vấn ý kiến");
            }

            BT_approve.SetImage(UIImage.FromFile("Icons/icon_Btn_action_" + control_action.ID), UIControlState.Normal);
            BT_approve.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);



            //if (action.ID == 1) // từ chối yêu cầu
            //{
            //    lbl_title.Text = "Từ chối phê duyệt yêu cầu";
            //    BT_approve.SetImage(UIImage.FromFile("Icons/icon_reject"), UIControlState.Normal);
            //    BT_approve.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            //}
            //else if (action.ID == 2) // hủy yêu cầu
            //{
            //    lbl_title.Text = "Huỷ phê duyệt yêu cầu";
            //    BT_approve.SetImage(UIImage.FromFile("Icons/icon_cancel"), UIControlState.Normal);
            //    BT_approve.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
            //}
        }

        /// <summary>
        /// typeAction => 0 : approve | 1 : reject | 2 : cancel
        /// </summary>
        /// <param name="_typeAction"></param>
        public void SetContent(UIViewController _parentView, ButtonAction _typeAction)
        {
            parentView = _parentView;
            control_action = _typeAction;
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
        #endregion

        #region event
        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            if (parentView.GetType() == typeof(KanBanView))
            {
                KanBanView kanBanView = parentView as KanBanView;
                kanBanView.loading.Hide();
            }
            this.DismissModalViewController(true);
        }

        private void BT_approve_TouchUpInside(object sender, EventArgs e)
        {
            if (control_action.ID == 1) // duyet - khong can check y kien
            {
                string idea = "";
                if (!string.IsNullOrEmpty(textview_content.Text) || !textview_content.Text.Equals(hintDefault))
                    idea = textview_content.Text;

                KeyValuePair<string, string> note = new KeyValuePair<string, string>("idea", idea);
                lstExtent.Add(note);

                if (parentView.GetType() == typeof(ToDoDetailView))
                {
                    ToDoDetailView toDoDetailView = parentView as ToDoDetailView;
                    toDoDetailView.SubmitAction(control_action, lstExtent);
                }
                else if (parentView.GetType() == typeof(WorkflowDetailView))
                {
                    WorkflowDetailView workflowDetailView = parentView as WorkflowDetailView;
                    workflowDetailView.SubmitAction(control_action, lstExtent);
                }
                else if (parentView.GetType() == typeof(FormWorkFlowDetails))
                {
                    FormWorkFlowDetails formWorkFlowDetails = parentView as FormWorkFlowDetails;
                    formWorkFlowDetails.SubmitAction(control_action, lstExtent);
                }
                else if (parentView.GetType() == typeof(FollowListViewController))
                {
                    FollowListViewController followListViewController = parentView as FollowListViewController;
                    followListViewController.SubmitAction(control_action, lstExtent);
                }

                this.DismissModalViewController(true);
            }
            else if (control_action.ID == 2) // phe duyet - khong can check y kien
            {
                string idea = "";
                if (!string.IsNullOrEmpty(textview_content.Text) || !textview_content.Text.Equals(hintDefault))
                    idea = textview_content.Text;

                KeyValuePair<string, string> note = new KeyValuePair<string, string>("idea", idea);
                lstExtent.Add(note);

                if (parentView.GetType() == typeof(ToDoDetailView))
                {
                    ToDoDetailView toDoDetailView = parentView as ToDoDetailView;
                    toDoDetailView.SubmitAction(control_action, lstExtent);
                }
                else if (parentView.GetType() == typeof(WorkflowDetailView))
                {
                    WorkflowDetailView workflowDetailView = parentView as WorkflowDetailView;
                    workflowDetailView.SubmitAction(control_action, lstExtent);
                }
                else if (parentView.GetType() == typeof(FormWorkFlowDetails))
                {
                    FormWorkFlowDetails formWorkFlowDetails = parentView as FormWorkFlowDetails;
                    formWorkFlowDetails.SubmitAction(control_action, lstExtent);
                }
                else if (parentView.GetType() == typeof(FollowListViewController))
                {
                    FollowListViewController toDoDetailView = parentView as FollowListViewController;
                    toDoDetailView.SubmitAction(control_action, lstExtent);
                }
                this.DismissModalViewController(true);
            }
            else if (control_action.ID == 4) // yeu cau hieu chinh
            {
                if (!string.IsNullOrEmpty(textview_content.Text))
                {
                    KeyValuePair<string, string> note = new KeyValuePair<string, string>("idea", textview_content.Text);
                    lstExtent.Add(note);
                    if (parentView.GetType() == typeof(ToDoDetailView))
                    {
                        ToDoDetailView toDoDetailView = parentView as ToDoDetailView;
                        toDoDetailView.SubmitAction(control_action, lstExtent);

                    }
                    else if (parentView.GetType() == typeof(WorkflowDetailView))
                    {
                        WorkflowDetailView workflowDetailView = parentView as WorkflowDetailView;
                        workflowDetailView.SubmitAction(control_action, lstExtent);
                    }
                    else if (parentView.GetType() == typeof(KanBanView))
                    {
                        KanBanView kanBanView = parentView as KanBanView;
                        kanBanView.ActionReject(control_action, lstExtent);
                    }
                    else if (parentView.GetType() == typeof(FormWorkFlowDetails))
                    {
                        FormWorkFlowDetails formWorkFlowDetails = parentView as FormWorkFlowDetails;
                        formWorkFlowDetails.SubmitAction(control_action, lstExtent);
                    }
                    else if (parentView.GetType() == typeof(FollowListViewController))
                    {
                        FollowListViewController toDoDetailView = parentView as FollowListViewController;
                        toDoDetailView.SubmitAction(control_action, lstExtent);
                    }

                    this.DismissModalViewController(true);
                }
                else
                    CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("MESS_REQUIRE_COMMENT", "Vui lòng nhập ý kiến."));
            }
            else if (control_action.ID == 5) // tu choi
            {
                if (!string.IsNullOrEmpty(textview_content.Text))
                {
                    KeyValuePair<string, string> note = new KeyValuePair<string, string>("idea", textview_content.Text);
                    lstExtent.Add(note);
                    if (parentView.GetType() == typeof(ToDoDetailView))
                    {
                        ToDoDetailView toDoDetailView = parentView as ToDoDetailView;
                        toDoDetailView.SubmitAction(control_action, lstExtent);

                    }
                    else if (parentView.GetType() == typeof(WorkflowDetailView))
                    {
                        WorkflowDetailView workflowDetailView = parentView as WorkflowDetailView;
                        workflowDetailView.SubmitAction(control_action, lstExtent);
                    }
                    else if (parentView.GetType() == typeof(KanBanView))
                    {
                        KanBanView kanBanView = parentView as KanBanView;
                        kanBanView.ActionReject(control_action, lstExtent);

                    }
                    else if (parentView.GetType() == typeof(FormWorkFlowDetails))
                    {
                        FormWorkFlowDetails formWorkFlowDetails = parentView as FormWorkFlowDetails;
                        formWorkFlowDetails.SubmitAction(control_action, lstExtent);
                    }
                    else if (parentView.GetType() == typeof(FollowListViewController))
                    {
                        FollowListViewController followListViewController = parentView as FollowListViewController;
                        followListViewController.SubmitAction(control_action, lstExtent);
                    }
                    this.DismissModalViewController(true);
                }
                else
                    CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("MESS_REQUIRE_COMMENT", "Vui lòng nhập ý kiến."));
            }
            else if (control_action.ID == 51) // huy
            {
                if (!string.IsNullOrEmpty(textview_content.Text))
                {
                    KeyValuePair<string, string> note = new KeyValuePair<string, string>("idea", textview_content.Text);
                    lstExtent.Add(note);
                    if (parentView.GetType() == typeof(ToDoDetailView))
                    {
                        ToDoDetailView toDoDetailView = parentView as ToDoDetailView;
                        toDoDetailView.SubmitAction(control_action, lstExtent);

                    }
                    else if (parentView.GetType() == typeof(WorkflowDetailView))
                    {
                        WorkflowDetailView workflowDetailView = parentView as WorkflowDetailView;
                        workflowDetailView.SubmitAction(control_action, lstExtent);
                    }
                    else if (parentView.GetType() == typeof(KanBanView))
                    {
                        KanBanView kanBanView = parentView as KanBanView;
                        kanBanView.ActionReject(control_action, lstExtent);

                    }
                    else if (parentView.GetType() == typeof(FormWorkFlowDetails))
                    {
                        FormWorkFlowDetails formWorkFlowDetails = parentView as FormWorkFlowDetails;
                        formWorkFlowDetails.SubmitAction(control_action, lstExtent);
                    }
                    else if (parentView.GetType() == typeof(FollowListViewController))
                    {
                        FollowListViewController followListViewController = parentView as FollowListViewController;
                        followListViewController.SubmitAction(control_action, lstExtent);

                    }
                    this.DismissModalViewController(true);
                }
                else
                    CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("MESS_REQUIRE_COMMENT", "Vui lòng nhập ý kiến."));
            }
            else if (control_action.ID == 10) // cho y kien tham van
            {
                if (!string.IsNullOrEmpty(textview_content.Text))
                {
                    KeyValuePair<string, string> note = new KeyValuePair<string, string>("idea", textview_content.Text);
                    lstExtent.Add(note);
                    if (parentView.GetType() == typeof(ToDoDetailView))
                    {
                        ToDoDetailView toDoDetailView = parentView as ToDoDetailView;
                        toDoDetailView.SubmitAction(control_action, lstExtent);

                    }
                    this.DismissModalViewController(true);
                }
                else
                    CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("MESS_REQUIRE_COMMENT", "Vui lòng nhập ý kiến."));
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
            { Console.WriteLine("FormApproveOrRejectView - Err: " + ex.ToString()); }
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
            { Console.WriteLine("FormApproveOrRejectView - Err: " + ex.ToString()); }
        }
        #endregion
    }
}
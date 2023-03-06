using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobileV1.iOS.IOSClass;
using Foundation;
using UIKit;


namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class AgreeOrRejectView : UIViewController
    {
        //RequestDetailsV2
        UIViewController parentView { get; set; }
        ButtonAction control_action; // 0: tu choi - 1: phe duyet - 2: cho y kien
        private UITapGestureRecognizer gestureRecognizer_keyboard;
        //list extend những value add thêm không dynamic control (ghi chú, ý kiến, người được chuyển xử lý...)
        // nội dung, ý kiến: key = "idea"
        // user, người xử lý: key = "userValues"
        private List<KeyValuePair<string, string>> lstExtent = new List<KeyValuePair<string, string>>();
        string hintDefault = CmmFunction.GetTitle("TEXT_HINT_REQUIRE_COMMENT", "Nhập ý kiến");
        CmmLoading loading;
        BeanAppBaseExt appBaseItem;
        private UITapGestureRecognizer gestureRecognizer;

        public AgreeOrRejectView(IntPtr handle) : base(handle)
        {
        //1234567
        }

        #region override

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
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

            setlangTitle();
            #region delegate            
            textview_content.Started += Tv_Ykien_Started;
            textview_content.Ended += Tv_Ykien_Ended;
            BT_submit.TouchUpInside += BT_submit_TouchUpInside;
            BT_cancel.TouchUpInside += BT_Cancel_TouchUpInside;
            #endregion
        }

        #endregion

        #region private - public method

        public void setContent(ButtonAction _type_action, UIViewController _parent, BeanWorkflowItem _workflowItem, BeanControlDynamicDetail _controlDetails, BeanAppBaseExt _appBaseItem = null)
        {
            control_action = _type_action;
            parentView = _parent;
            appBaseItem = _appBaseItem;
        }

        private void ViewConfiguration()
        {
            textview_content.Layer.CornerRadius = 4;
            textview_content.Layer.BorderColor = UIColor.FromRGB(229,229,229).CGColor;
            textview_content.Layer.BorderWidth = 1f;
            textview_content.BecomeFirstResponder();
        }

        private void setlangTitle()
        {
            CmmIOSFunction.SetLangToView(this.View);
        }

        public void loadContent()
        {
            lbl_title.Text = control_action.Title;

            if (control_action.Notes != null)
            {
                ObjectElementNote objectElementNote = control_action.Notes.SingleOrDefault();
                if (objectElementNote.Key == "N001")
                    lbl_note.Text = CmmFunction.GetTitle(objectElementNote.Key, "Phiếu đang chờ tham vấn ý kiến");
            }
            else
                lbl_note.Text = "";

            img_title.Image = UIImage.FromFile("Icons/icon_Btn_action_" + control_action.ID);
        }
        [Export("hideKeyboard")]
        private void hideKeyboard()
        {

            this.View.EndEditing(true);
        }
        #endregion

        #region events
        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            this.DismissViewControllerAsync(true);
        }
        private void BT_Cancel_TouchUpInside(object sender, EventArgs e)
        {
            this.DismissViewControllerAsync(true);
        }
        void Tv_Ykien_Ended(object sender, EventArgs e)
        {
            //if (textview_content.Text == "")
            //{
            //    textview_content.Text = hintDefault;
            //    textview_content.TextColor = UIColor.LightGray;
            //}
        }
        void Tv_Ykien_Started(object sender, EventArgs e)
        {
            //textview_content.Text = string.Empty;
            //if (textview_content.Text == hintDefault)
            //{
            //    textview_content.TextColor = UIColor.DarkGray;
            //    textview_content.Text = "";
            //}
        }
        private void BT_submit_TouchUpInside(object sender, EventArgs e)
        {
            if (control_action.ID == 1) // duyet - khong can check y kien
            {
                string idea = "";
                if (!string.IsNullOrEmpty(textview_content.Text) || !textview_content.Text.Equals(hintDefault))
                    idea = textview_content.Text;

                KeyValuePair<string, string> note = new KeyValuePair<string, string>("idea", idea);
                lstExtent.Add(note);

                if(parentView.GetType() == typeof(RequestDetailsV2))
                {
                    RequestDetailsV2 requestDetailsV2 = parentView as RequestDetailsV2;
                    requestDetailsV2.SubmitAction(control_action, lstExtent);
                }
               

                this.DismissModalViewController(true);
            }
            else if (control_action.ID == 2) // duyet - khong can check y kien
            {
                string idea = "";
                if (!string.IsNullOrEmpty(textview_content.Text) || !textview_content.Text.Equals(hintDefault))
                    idea = textview_content.Text;

                KeyValuePair<string, string> note = new KeyValuePair<string, string>("idea", idea);
                lstExtent.Add(note);

                if (parentView.GetType() == typeof(RequestDetailsV2))
                {
                    RequestDetailsV2 requestDetailsV2 = parentView as RequestDetailsV2;
                    requestDetailsV2.SubmitAction(control_action, lstExtent);
                }
               

                this.DismissModalViewController(true);
            }
            else if (control_action.ID == 4) //  // yeu cau hieu chinh - return
            {
                if (!string.IsNullOrEmpty(textview_content.Text))
                {
                    KeyValuePair<string, string> note = new KeyValuePair<string, string>("idea", textview_content.Text);
                    lstExtent.Add(note);
                    if (parentView.GetType() == typeof(RequestDetailsV2))
                    {
                        RequestDetailsV2 requestDetailsV2 = parentView as RequestDetailsV2;
                        requestDetailsV2.SubmitAction(control_action, lstExtent);
                    }
                    else if (parentView.GetType() == typeof(KanBanView))
                    {
                        KanBanView kanBanView = parentView as KanBanView;
                        kanBanView.ActionApprove(control_action, lstExtent);
                    }
                    this.DismissModalViewController(true);
                }
                else
                    CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("MESS_REQUIRE_COMMENT", "Vui lòng nhập ý kiến."));
            }
            else if (control_action.ID == 5) // Từ chối
            {
                if (!string.IsNullOrEmpty(textview_content.Text))
                {
                    KeyValuePair<string, string> note = new KeyValuePair<string, string>("idea", textview_content.Text);
                    lstExtent.Add(note);
                    if (parentView.GetType() == typeof(RequestDetailsV2))
                    {
                        RequestDetailsV2 requestDetailsV2 = parentView as RequestDetailsV2;
                        requestDetailsV2.SubmitAction(control_action, lstExtent);
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
                    if (parentView.GetType() == typeof(RequestDetailsV2))
                    {
                        RequestDetailsV2 requestDetailsV2 = parentView as RequestDetailsV2;
                        requestDetailsV2.SubmitAction(control_action, lstExtent);
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
                    if (parentView.GetType() == typeof(RequestDetailsV2))
                    {
                        RequestDetailsV2 requestDetailsV2 = parentView as RequestDetailsV2;
                        requestDetailsV2.SubmitAction(control_action, lstExtent);
                    }
                    
                    this.DismissModalViewController(true);
                }
                else
                    CmmIOSFunction.commonAlertMessage(this, "BPM", CmmFunction.GetTitle("MESS_REQUIRE_COMMENT", "Vui lòng nhập ý kiến."));
            }
        }
        #endregion
    }
}


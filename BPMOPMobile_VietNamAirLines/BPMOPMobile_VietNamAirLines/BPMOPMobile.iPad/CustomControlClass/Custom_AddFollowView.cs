using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.iPad.IOSClass;
using BPMOPMobile.iPad.ViewControllers;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.CustomControlClass
{
    class Custom_AddFollowView : UIView
    {
        UIButton BT_action;
        UIImageView iv_left;
        UILabel lbl_title;
        public bool isFollow;

        private Custom_AddFollowView()
        {
            this.BackgroundColor = UIColor.White;

            iv_left = new UIImageView();
            iv_left.ContentMode = UIViewContentMode.ScaleAspectFit;


            lbl_title = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(14),
                TextColor = UIColor.FromRGB(51, 95, 179),
                TextAlignment = UITextAlignment.Left
            };

            BT_action = new UIButton();

            this.AddSubviews(new UIView[] { iv_left, lbl_title, BT_action });

            if (BT_action != null)
                BT_action.AddTarget(HandleBtnAction, UIControlEvent.TouchUpInside);
        }

        private void HandleBtnAction(object sender, EventArgs e)
        {
            if (viewController != null && viewController.GetType() == typeof(WorkflowDetailView))
            {
                WorkflowDetailView controller = (WorkflowDetailView)viewController;
                controller.HandleAddFollow();
            }
            else if (viewController != null && viewController.GetType() == typeof(ToDoDetailView))
            {
                ToDoDetailView controller = (ToDoDetailView)viewController;
                controller.HandleAddFollow();
            }
            else if (viewController != null && viewController.GetType() == typeof(FormWorkFlowDetails))
            {
                FormWorkFlowDetails controller = (FormWorkFlowDetails)viewController;
                controller.HandleAddFollow();
            }
            else if (viewController != null && viewController.GetType() == typeof(FollowListViewController))
            {
                FollowListViewController controller = (FollowListViewController)viewController;
                controller.HandleAddFollow();
            }

            //GetCountNumber();
        }

        void GetCountNumber()
        {
            var follow_count = 0;
            string count = "";
            try
            {
                count = new ProviderControlDynamic().GetListCountVDT_VTBD(CmmVariable.KEY_COUNT_FOLLOW);

                if (!string.IsNullOrEmpty(count))
                {

                    var str = count.Split(";#");
                    if (string.Compare(str[0], CmmVariable.KEY_COUNT_FOLLOW) == 0)
                    {
                        if (!int.TryParse(str[1], out follow_count))
                        {
                            follow_count = 0;
                        }
                    }
                }
                else
                {
#if DEBUG
                    Console.WriteLine("GetCountNumber trả về chuỗi trống.");
#endif
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("GetCountNumber - Err: " + ex.ToString());
#endif
            }
        }

        private static Custom_AddFollowView instance = null;
        public static Custom_AddFollowView Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Custom_AddFollowView();
                }
                return instance;
            }
        }

        public void InitFrameView(CGRect frame)
        {
            this.Frame = frame;

            iv_left.Frame = new CGRect(20, 20, 16, 16);
            lbl_title.Frame = new CGRect(56, 13, Frame.Width - 56, 30);

            BT_action.Frame = new CGRect(0, 0, Frame.Width, Frame.Height);

            this.Layer.ShadowColor = UIColor.DarkGray.CGColor;
            this.Layer.ShadowPath = UIBezierPath.FromRect(new CGRect(2, 2, Frame.Width, Frame.Height)).CGPath;
            this.Layer.ShadowRadius = 5;
            this.Layer.ShadowOffset = new CGSize(0, 2);
            this.Layer.ShadowOpacity = 1;
        }

        public void LoadContent()
        {
            if (isFollow)
            {
                lbl_title.Text = CmmFunction.GetTitle("MESS_UNFOLLOW_TASK", "Hủy theo dõi công việc này");
                iv_left.Image = UIImage.FromFile("Icons/icon_cancel.png");
            }
            else
            {
                lbl_title.Text = CmmFunction.GetTitle("MESS_FOLLOW_TASK", "Đặt theo dõi công việc này");
                iv_left.Image = UIImage.FromFile("Icons/icon_check.png");
            }

        }

        public UIViewController viewController { get; set; }
    }
}
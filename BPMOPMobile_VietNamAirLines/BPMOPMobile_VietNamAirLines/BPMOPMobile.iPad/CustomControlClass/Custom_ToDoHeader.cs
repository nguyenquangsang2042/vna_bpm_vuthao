using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPMOPMobile.iPad.ViewControllers;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.CustomControlClass
{
    public class Custom_ToDoHeader: UIView
    {
        UILabel lbl_title;
        UILabel lbl_top_line, lbl_bot_line;
        UIButton btn_action;
        UIViewController viewController;
        nint currentSection;
        int tableIndex;

        public Custom_ToDoHeader(UIViewController _viewController, CGRect _frame)
        {
            this.BackgroundColor = UIColor.FromRGB(229, 229, 229);
            viewController = _viewController;

            lbl_title = new UILabel()
            {
                Font = UIFont.FromName("Arial-BoldMT", 14f),
                TextColor = UIColor.FromRGB(94, 94, 94)
            };

            lbl_top_line = new UILabel();
            lbl_top_line.BackgroundColor = UIColor.FromRGB(240, 240, 240);

            lbl_bot_line = new UILabel();
            lbl_bot_line.BackgroundColor = UIColor.FromRGB(240, 240, 240);
            lbl_bot_line.Hidden = true;

            btn_action = new UIButton();

            this.AddSubviews(new UIView[] { lbl_top_line, lbl_title, lbl_bot_line, btn_action });

            if (btn_action != null)
                btn_action.AddTarget(HandleBtnAction, UIControlEvent.TouchUpInside);

            InitFrameViews(_frame);
        }

        private void HandleBtnAction(object sender, EventArgs e)
        {
            if (viewController != null && viewController.GetType() == typeof(MainView))
            {
                MainView controller = (MainView)viewController;
                controller.HandleSectionTable(currentSection, lbl_title.Text, tableIndex);
            }
            else if (viewController != null && viewController.GetType() == typeof(WorkflowDetailView))
            {
                WorkflowDetailView controller = (WorkflowDetailView)viewController;
                controller.HandleSectionTable(currentSection, lbl_title.Text, tableIndex);
            }
            else if (viewController != null && viewController.GetType() == typeof(ToDoDetailView))
            {
                ToDoDetailView controller = (ToDoDetailView)viewController;
                controller.HandleSectionTable(currentSection, lbl_title.Text, tableIndex);
            }
            else if (viewController != null && viewController.GetType() == typeof(FollowListViewController))
            {
                FollowListViewController controller = (FollowListViewController)viewController;
                controller.HandleSectionTable(currentSection, lbl_title.Text, tableIndex);
            }
        }

        private void InitFrameViews(CGRect _frame)
        {
            this.Frame = _frame;

            lbl_title.Frame = new CGRect(25, 0, Frame.Width - 25, Frame.Height);
            lbl_top_line.Frame = new CGRect(0, 0, Frame.Width, 1);
            lbl_bot_line.Frame = new CGRect(0, Frame.Bottom - 1, Frame.Width, 1);
            btn_action.Frame = new CGRect(0, 0, Frame.Width, Frame.Height);
        }

        public void LoadData(nint _section, string _title, int _tableIndex = 0)
        {
            tableIndex = _tableIndex;
            currentSection = _section;
            lbl_title.Text = _title;
        }
    }
}
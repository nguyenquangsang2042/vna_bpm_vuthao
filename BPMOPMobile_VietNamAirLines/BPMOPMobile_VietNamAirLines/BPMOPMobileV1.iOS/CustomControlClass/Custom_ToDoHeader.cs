using System;
using BPMOPMobile.Class;
using BPMOPMobileV1.iOS.ViewControllers;
using CoreGraphics;
using UIKit;

namespace BPMOPMobileV1.iOS.CustomControlClass
{
    public class Custom_ToDoHeader : UIView
    {
        UILabel lbl_title;
        UILabel lbl_top_line, lbl_bot_line;
        UIButton btn_action;
        UIViewController viewController;
        nint currentSection;
        int tableIndex;

        public Custom_ToDoHeader(UIViewController _viewController, CGRect _frame)
        {
            this.BackgroundColor = UIColor.FromRGB(229,229,229).ColorWithAlpha(0.4f);

            viewController = _viewController;

            lbl_title = new UILabel()
            {
                //Font = UIFont.SystemFontOfSize(14, UIFontWeight.Medium),
                Font= UIFont.FromName("Arial-BoldMT", 14f),
                TextColor = UIColor.FromRGB(94, 94, 94)
            };

            lbl_top_line = new UILabel();
            lbl_top_line.BackgroundColor = UIColor.FromRGB(240, 240, 240);

            lbl_bot_line = new UILabel();
            lbl_bot_line.BackgroundColor = UIColor.FromRGB(240, 240, 240);

            btn_action = new UIButton();

            //this.AddSubviews(new UIView[] { lbl_top_line, lbl_title, lbl_bot_line, btn_action });
            this.AddSubviews(new UIView[] { lbl_top_line, lbl_title, btn_action });

            if (btn_action != null)
                btn_action.AddTarget(HandleBtnAction, UIControlEvent.TouchUpInside);

            InitFrameViews(_frame);
        }

        private void HandleBtnAction(object sender, EventArgs e)
        {
            if (viewController != null && viewController.GetType() == typeof(MainView))
            {
                MainView controller = (MainView)viewController;
                //controller.HandleSectionTable(currentSection, lbl_title.Text, tableIndex);
            }
            else if (viewController != null && viewController.GetType() == typeof(MyRequestListView))
            {
                MyRequestListView controller = (MyRequestListView)viewController;
                controller.HandleSectionTable(currentSection, lbl_title.Text, tableIndex);
            }
            else if (viewController != null && viewController.GetType() == typeof(RequestListView))
            {
                RequestListView controller = (RequestListView)viewController;
                //controller.HandleSectionTable(currentSection, lbl_title.Text, tableIndex);
            }
            //app
            //else if (viewController != null && viewController.GetType() == typeof(MainViewApp))
            //{
            //    MainViewApp controller = (MainViewApp)viewController;
            //    //controller.HandleSectionTable(currentSection, lbl_title.Text, tableIndex);
            //}
            //else if (viewController != null && viewController.GetType() == typeof(MyRequestListViewApp))
            //{
            //    MyRequestListViewApp controller = (MyRequestListViewApp)viewController;
            //    controller.HandleSectionTable(currentSection, lbl_title.Text, tableIndex);
            //}
            //else if (viewController != null && viewController.GetType() == typeof(RequestListViewApp))
            //{
            //    RequestListViewApp controller = (RequestListViewApp)viewController;
            //    //controller.HandleSectionTable(currentSection, lbl_title.Text, tableIndex);
            //}
        }

        private void InitFrameViews(CGRect _frame)
        {
            this.Frame = _frame;

            lbl_title.Frame = new CGRect(16, 0, Frame.Width - 25, Frame.Height);
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

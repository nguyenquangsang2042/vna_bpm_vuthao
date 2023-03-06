using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.DataProvider;
using BPMOPMobile.iPad.IOSClass;
using CoreGraphics;
using Foundation;
using SQLite;
using UIKit;

namespace BPMOPMobile.iPad.CustomControlClass
{
    public class Custom_QTLCHeader : UIView
    {
        UILabel lbl_title, lbl_line;
        UIImageView icon_status;
        nint currentSection;
        int tableIndex;
        BeanQuaTrinhLuanChuyen qtlc { get; set; }
        int numInSection;


        public Custom_QTLCHeader(CGRect _frame)
        {
            this.BackgroundColor = UIColor.White;

            icon_status = new UIImageView();

            lbl_line = new UILabel();
            lbl_line.BackgroundColor = UIColor.FromRGB(229, 229, 229);

            lbl_title = new UILabel()
            {
                TextAlignment = UITextAlignment.Left,
                Font = UIFont.FromName("Arial-BoldMT", 13f),
                TextColor = UIColor.Black
            };

            this.AddSubviews(new UIView[] { lbl_line, icon_status, lbl_title });

            //if (btn_action != null)
            //    btn_action.AddTarget(HandleBtnAction, UIControlEvent.TouchUpInside);

            InitFrameViews(_frame);
        }

        private void HandleBtnAction(object sender, EventArgs e)
        {
            //if (viewController != null && viewController.GetType() == typeof(MainView))
            //{
            //    MainView controller = (MainView)viewController;
            //    controller.HandleSectionTable(currentSection, lbl_title.Text, tableIndex);
            //}
            //else if (viewController != null && viewController.GetType() == typeof(WorkflowDetailView))
            //{
            //    WorkflowDetailView controller = (WorkflowDetailView)viewController;
            //    controller.HandleSectionTable(currentSection, lbl_title.Text, tableIndex);
            //}
            //else if (viewController != null && viewController.GetType() == typeof(ToDoDetailView))
            //{
            //    ToDoDetailView controller = (ToDoDetailView)viewController;
            //    controller.HandleSectionTable(currentSection, lbl_title.Text, tableIndex);
            //}
        }

        private void InitFrameViews(CGRect _frame)
        {
            this.Frame = _frame;

            icon_status.Frame = new CGRect(15, 0, 20, 20);
            lbl_line.Frame = new CGRect(24, icon_status.Frame.Bottom, 1, this.Frame.Height - 20);
            lbl_title.Frame = new CGRect(icon_status.Frame.GetMaxX() + 10, 2, 240, 16);

        }

        public void LoadData(bool _isLast, nint _section, BeanQuaTrinhLuanChuyen _qtlc, int _tableIndex = 0)
        {
            tableIndex = _tableIndex;
            currentSection = _section;

            try
            {
                if (_isLast)
                    lbl_line.Hidden = true;

                qtlc = _qtlc;


                if (qtlc.Status)
                    icon_status.Image = UIImage.FromFile("Icons/icon_point_green.png");
                else
                    icon_status.Image = UIImage.FromFile("Icons/icon_point_orange.png");

                if (CmmVariable.SysConfig.LangCode == "1033")
                    lbl_title.Text = qtlc.TitleEN;
                else //if (CmmVariable.SysConfig.LangCode == "1066")
                    lbl_title.Text = qtlc.Title;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Custom_WorkFlowView - Custom_qtlc_Cell - UpdateCell: " + ex.ToString());
            }
        }
    }
}

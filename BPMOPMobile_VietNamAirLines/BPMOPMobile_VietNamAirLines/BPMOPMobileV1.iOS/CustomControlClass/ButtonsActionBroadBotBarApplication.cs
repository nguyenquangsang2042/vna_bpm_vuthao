using System;
using System.Collections.Generic;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobileV1.iOS.IOSClass;
using BPMOPMobileV1.iOS.ViewControllers;
using CoreGraphics;
using UIKit;

namespace BPMOPMobileV1.iOS.CustomControlClass
{
    class ButtonsActionBroadBotBarApplication : UIView
    {
        UIView viewGroup1, viewGroup2, viewGroup3, viewGroup4;
        UIImageView iv_group1, iv_group2, iv_group3, iv_group4;
        UIButton btn_group1, btn_group2, btn_group3, btn_group4;

        List<bool> lst_status = new List<bool>() { true, false, false, false };

        // các constraint sẽ thay đổi khi button action
        NSLayoutConstraint width_iv_group1, width_iv_group2, width_iv_group3, width_iv_group4;
        NSLayoutConstraint height_iv_group1, height_iv_group2, height_iv_group3, height_iv_group4;
        NSLayoutConstraint top_iv_group1, top_iv_group2, top_iv_group3, top_iv_group4;
        NSLayoutConstraint center_iv_group1, center_iv_group2, center_iv_group3, center_iv_group4;

        AppDelegate appD = UIApplication.SharedApplication.Delegate as AppDelegate;

        //public MainViewApp mainViewApp { get; set; }
        public MainViewApp mainViewApp { get; set; }
        public static List<ButtonActionApp> classMenu { get; set; }
        public BroadView broadView { get; set; }
        public BeanWorkflow workflow { get; set; }
        public int index { get; set; }
        MyRequestListViewApp myRequestListViewApp { get; set; }
        RequestListViewApp requestListViewApp { get; set; }
        KanBanView kanBanView { get; set; }
        ListView listView { get; set; }

        public static ButtonActionApp classMenuSelect { get; set; }


        private ButtonsActionBroadBotBarApplication()
        {
            //add views group 1
            ButtonActionApp classMenu1 = classMenu[0];
            viewGroup1 = new UIView();
            iv_group1 = new UIImageView();
            UIImage img_group1 = UIImage.FromFile(classMenu1.iconUrl).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            iv_group1.ContentMode = UIViewContentMode.ScaleAspectFit;
            iv_group1.Image = img_group1;
            iv_group1.TranslatesAutoresizingMaskIntoConstraints = false;

            btn_group1 = new UIButton();
            btn_group1.TranslatesAutoresizingMaskIntoConstraints = false;

            //add views group 2
            ButtonActionApp classMenu2 = classMenu[1];
            viewGroup2 = new UIView();
            iv_group2 = new UIImageView();
            UIImage img_group2 = UIImage.FromFile(classMenu2.iconUrl).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            iv_group2.ContentMode = UIViewContentMode.ScaleAspectFit;
            iv_group2.Image = img_group2;
            iv_group2.TranslatesAutoresizingMaskIntoConstraints = false;

            btn_group2 = new UIButton();
            btn_group2.TranslatesAutoresizingMaskIntoConstraints = false;

            //add views group 3
            ButtonActionApp classMenu3 = classMenu.Find(s => s.isSelected == true && s.opption == true);
            viewGroup3 = new UIView();
            iv_group3 = new UIImageView();
            UIImage img_group3 = UIImage.FromFile(classMenu3.iconUrl).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            iv_group3.ContentMode = UIViewContentMode.ScaleAspectFit;
            iv_group3.Image = img_group3;
            iv_group3.TranslatesAutoresizingMaskIntoConstraints = false;

            btn_group3 = new UIButton();
            btn_group3.TranslatesAutoresizingMaskIntoConstraints = false;

            //add views group 4
            ButtonActionApp classMenu4 = classMenu[3];
            viewGroup4 = new UIView();
            iv_group4 = new UIImageView();
            UIImage img_group4 = UIImage.FromFile(classMenu4.iconUrl).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            iv_group4.ContentMode = UIViewContentMode.ScaleAspectFit;
            iv_group4.Image = img_group4;
            iv_group4.TranslatesAutoresizingMaskIntoConstraints = false;

            btn_group4 = new UIButton();
            btn_group4.TranslatesAutoresizingMaskIntoConstraints = false;

            //add views group 4

            this.Add(viewGroup1);
            this.Add(viewGroup2);
            this.Add(viewGroup3);
            this.Add(viewGroup4);

            viewGroup1.AddSubviews(new UIView[] { iv_group1, btn_group1 });
            viewGroup2.AddSubviews(new UIView[] { iv_group2, btn_group2 });
            viewGroup3.AddSubviews(new UIView[] { iv_group3, btn_group3 });
            viewGroup4.AddSubviews(new UIView[] { iv_group4, btn_group4 });

            if (btn_group1 != null)
                btn_group1.AddTarget(Handle_Btn_group1, UIControlEvent.TouchUpInside);

            if (btn_group2 != null)
                btn_group2.AddTarget(Handle_Btn_group2, UIControlEvent.TouchUpInside);

            if (btn_group3 != null)
                btn_group3.AddTarget(Handle_Btn_group3, UIControlEvent.TouchUpInside);

            if (btn_group4 != null)
                btn_group4.AddTarget(Handle_Btn_group4, UIControlEvent.TouchUpInside);

        }

        private static ButtonsActionBroadBotBarApplication instance = null;
        public static ButtonsActionBroadBotBarApplication Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ButtonsActionBroadBotBarApplication();
                }
                return instance;
            }
        }
        public static void Reset() 
        {
            instance = new ButtonsActionBroadBotBarApplication();
        }

        public void InitFrameView(CGRect frame)
        {
            this.Frame = CGRect.Empty;
            if (this.Frame == CGRect.Empty)
            {
                this.Frame = new CGRect(0, 0, frame.Width, frame.Height);

                var widthViewGroup = Frame.Width / 4;
                viewGroup1.Frame = new CGRect(0, 0, widthViewGroup, Frame.Height);
                viewGroup2.Frame = new CGRect(widthViewGroup, 0, widthViewGroup, Frame.Height);
                viewGroup3.Frame = new CGRect(widthViewGroup * 2, 0, widthViewGroup, Frame.Height);
                viewGroup4.Frame = new CGRect(widthViewGroup * 3, 0, widthViewGroup, Frame.Height);

                var positionY = (viewGroup1.Frame.Height - 40) / 2;

                //add constrain views group 1
                width_iv_group1 = iv_group1.WidthAnchor.ConstraintEqualTo(20);
                width_iv_group1.Active = true;
                height_iv_group1 = iv_group1.HeightAnchor.ConstraintEqualTo(20);
                height_iv_group1.Active = true;
                top_iv_group1 = NSLayoutConstraint.Create(this.iv_group1, NSLayoutAttribute.Top, NSLayoutRelation.Equal, viewGroup1, NSLayoutAttribute.Top, 1.0f, positionY);
                top_iv_group1.Active = true;
                NSLayoutConstraint.Create(this.iv_group1, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, viewGroup1, NSLayoutAttribute.CenterX, 1.0f, 0.0f).Active = true;
                center_iv_group1 = NSLayoutConstraint.Create(this.iv_group1, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, viewGroup1, NSLayoutAttribute.CenterY, 1.0f, 0.0f);
                center_iv_group1.Active = false;

                btn_group1.WidthAnchor.ConstraintEqualTo(viewGroup1.Frame.Width / 2).Active = true;
                btn_group1.HeightAnchor.ConstraintEqualTo(viewGroup1.Frame.Height).Active = true;
                NSLayoutConstraint.Create(this.btn_group1, NSLayoutAttribute.Top, NSLayoutRelation.Equal, viewGroup1, NSLayoutAttribute.Top, 1.0f, 0.0f).Active = true;
                NSLayoutConstraint.Create(this.btn_group1, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, viewGroup1, NSLayoutAttribute.CenterX, 1.0f, 0.0f).Active = true;

                //add constrain views group 2
                width_iv_group2 = iv_group2.WidthAnchor.ConstraintEqualTo(25);
                width_iv_group2.Active = true;
                height_iv_group2 = iv_group2.HeightAnchor.ConstraintEqualTo(20);
                height_iv_group2.Active = true;
                top_iv_group2 = NSLayoutConstraint.Create(this.iv_group2, NSLayoutAttribute.Top, NSLayoutRelation.Equal, viewGroup2, NSLayoutAttribute.Top, 1.0f, positionY);
                top_iv_group2.Active = true;
                NSLayoutConstraint.Create(this.iv_group2, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, viewGroup2, NSLayoutAttribute.CenterX, 1.0f, 0.0f).Active = true;
                center_iv_group2 = NSLayoutConstraint.Create(this.iv_group2, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, viewGroup2, NSLayoutAttribute.CenterY, 1.0f, 0.0f);
                center_iv_group2.Active = false;

                btn_group2.WidthAnchor.ConstraintEqualTo(viewGroup2.Frame.Width / 2).Active = true;
                btn_group2.HeightAnchor.ConstraintEqualTo(viewGroup2.Frame.Height).Active = true;
                NSLayoutConstraint.Create(this.btn_group2, NSLayoutAttribute.Top, NSLayoutRelation.Equal, viewGroup2, NSLayoutAttribute.Top, 1.0f, 0.0f).Active = true;
                NSLayoutConstraint.Create(this.btn_group2, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, viewGroup2, NSLayoutAttribute.CenterX, 1.0f, 0.0f).Active = true;

                //add constrain views group 3
                width_iv_group3 = iv_group3.WidthAnchor.ConstraintEqualTo(18);
                width_iv_group3.Active = true;
                height_iv_group3 = iv_group3.HeightAnchor.ConstraintEqualTo(18);
                height_iv_group3.Active = true;
                top_iv_group3 = NSLayoutConstraint.Create(this.iv_group3, NSLayoutAttribute.Top, NSLayoutRelation.Equal, viewGroup3, NSLayoutAttribute.Top, 1.0f, positionY);
                top_iv_group3.Active = true;
                NSLayoutConstraint.Create(this.iv_group3, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, viewGroup3, NSLayoutAttribute.CenterX, 1.0f, 0.0f).Active = true;
                center_iv_group3 = NSLayoutConstraint.Create(this.iv_group3, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, viewGroup3, NSLayoutAttribute.CenterY, 1.0f, 0.0f);
                center_iv_group3.Active = false;

                btn_group3.WidthAnchor.ConstraintEqualTo(viewGroup3.Frame.Width / 2).Active = true;
                btn_group3.HeightAnchor.ConstraintEqualTo(viewGroup3.Frame.Height).Active = true;
                NSLayoutConstraint.Create(this.btn_group3, NSLayoutAttribute.Top, NSLayoutRelation.Equal, viewGroup3, NSLayoutAttribute.Top, 1.0f, 0.0f).Active = true;
                NSLayoutConstraint.Create(this.btn_group3, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, viewGroup3, NSLayoutAttribute.CenterX, 1.0f, 0.0f).Active = true;

                //add constrain views group 4
                width_iv_group4 = iv_group4.WidthAnchor.ConstraintEqualTo(18);
                width_iv_group4.Active = true;
                height_iv_group4 = iv_group4.HeightAnchor.ConstraintEqualTo(18);
                height_iv_group4.Active = true;
                top_iv_group4 = NSLayoutConstraint.Create(this.iv_group4, NSLayoutAttribute.Top, NSLayoutRelation.Equal, viewGroup4, NSLayoutAttribute.Top, 1.0f, positionY);
                top_iv_group4.Active = true;
                NSLayoutConstraint.Create(this.iv_group4, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, viewGroup4, NSLayoutAttribute.CenterX, 1.0f, 0.0f).Active = true;
                center_iv_group4 = NSLayoutConstraint.Create(this.iv_group4, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, viewGroup4, NSLayoutAttribute.CenterY, 1.0f, 0.0f);
                center_iv_group4.Active = false;

                btn_group4.WidthAnchor.ConstraintEqualTo(viewGroup4.Frame.Width / 2).Active = true;
                btn_group4.HeightAnchor.ConstraintEqualTo(viewGroup4.Frame.Height).Active = true;
                NSLayoutConstraint.Create(this.btn_group4, NSLayoutAttribute.Top, NSLayoutRelation.Equal, viewGroup4, NSLayoutAttribute.Top, 1.0f, 0.0f).Active = true;
                NSLayoutConstraint.Create(this.btn_group4, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, viewGroup4, NSLayoutAttribute.CenterX, 1.0f, 0.0f).Active = true;
            }
        }

        /// <summary>
        /// function cập nhật lại trạng thái button bot
        /// </summary>
        /// <param name="index">0: Trang chủ | 1: Đến tôi | 2: Tôi bắt đầu | 3: Broad | -1: Tạo mới</param>
        public void LoadStatusButton(int index)
        {
            switch (index)
            {
                case 0:
                    lst_status = new List<bool>() { true, false, false, false };
                    break;
                case 1:
                    lst_status = new List<bool>() { false, true, false, false };
                    break;
                case 2:
                    lst_status = new List<bool>() { false, false, true, false };
                    break;
                case 3:
                    lst_status = new List<bool>() { false, false, false, true };
                    break;
                default:
                    lst_status = new List<bool>() { false, false, false, false };
                    break;
            }

            if (lst_status[0])
            {
                width_iv_group1.Constant = 22;
                height_iv_group1.Constant = 22;
                //top_iv_group1.Active = true;
                center_iv_group1.Active = true;

                iv_group1.TintColor = UIColor.FromRGB(51, 95, 179);
            }
            else
            {
                width_iv_group1.Constant = 22;
                height_iv_group1.Constant = 22;
                //top_iv_group1.Active = false;
                center_iv_group1.Active = true;

                iv_group1.TintColor = UIColor.FromRGB(94, 94, 94);
            }

            if (lst_status[1])
            {
                width_iv_group2.Constant = 20;
                height_iv_group2.Constant = 20;
                //top_iv_group2.Active = true;
                center_iv_group2.Active = true;

                iv_group2.TintColor = UIColor.FromRGB(51, 95, 179);
            }
            else
            {
                width_iv_group2.Constant = 20;
                height_iv_group2.Constant = 20;
                //top_iv_group2.Active = false;
                center_iv_group2.Active = true;

                iv_group2.TintColor = UIColor.FromRGB(94, 94, 94);
            }

            if (lst_status[2])
            {
                width_iv_group3.Constant = 18;
                height_iv_group3.Constant = 18;
                //top_iv_group3.Active = true;
                center_iv_group3.Active = true;
                UIImage img_group3 = UIImage.FromFile(classMenuSelect.iconUrl).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                iv_group3.ContentMode = UIViewContentMode.ScaleAspectFit;
                iv_group3.Image = img_group3;
                iv_group3.TintColor = UIColor.FromRGB(51, 95, 179);
            }
            else
            {
                width_iv_group3.Constant = 18;
                height_iv_group3.Constant = 18;
                //top_iv_group3.Active = false;
                center_iv_group3.Active = true;

                iv_group3.TintColor = UIColor.FromRGB(94, 94, 94);
            }

            if (lst_status[3])
            {
                width_iv_group4.Constant = 18;
                height_iv_group4.Constant = 18;
                //top_iv_group3.Active = true;
                center_iv_group4.Active = true;

                iv_group4.TintColor = UIColor.FromRGB(51, 95, 179);
            }
            else
            {
                width_iv_group4.Constant = 18;
                height_iv_group4.Constant = 18;
                //top_iv_group3.Active = false;
                center_iv_group4.Active = true;

                iv_group4.TintColor = UIColor.FromRGB(94, 94, 94);
            }
        }
        #region event
        private void Handle_Btn_group1(object sender, EventArgs e)
        {
            LoadStatusButton(0);
            classMenuSelect = classMenu[0];
            classMenuSelect.Select(workflow, mainViewApp);
        }

        private void Handle_Btn_group2(object sender, EventArgs e)
        {
            LoadStatusButton(1);
            classMenuSelect = classMenu[1];
            classMenuSelect.Select(workflow, mainViewApp);
        }

        private void Handle_Btn_group3(object sender, EventArgs e)
        {
            LoadStatusButton(2);
            classMenuSelect = classMenu.Find(s => s.isSelected == true && s.opption == true);
            classMenuSelect.Select(workflow, mainViewApp);
        }

        private void Handle_Btn_group4(object sender, EventArgs e)
        {
            List<ButtonActionApp> classMenuOption = classMenu.FindAll(s => s.isSelected == false && s.opption == true);
            UIViewController parent = appD.SlideMenuController.MainViewController;
            classMenuSelect.SelectMore(mainViewApp, parent, classMenuOption);
        }
        public void Select_BT_Group1()
        {
            LoadStatusButton(0);
            classMenuSelect = classMenu[0];
            classMenuSelect.Select(workflow, mainViewApp);
        }
        public void SelectFromeMoreAcTion(ButtonActionApp buttonActionApp)
        {
            foreach(var item in classMenu)
            {
                if (item.opption)
                {
                    if(buttonActionApp.ID == item.ID)
                        item.isSelected = true;
                    else
                        item.isSelected = false;
                }  
            }
            
            classMenuSelect = classMenu.Find(s => s.ID == buttonActionApp.ID);
            LoadStatusButton(2);// dong nay o vi tri nay de sau khi cap nhat menuselect thi change icon va mau
            classMenuSelect.Select(workflow, mainViewApp);
        }
        #endregion
    }
}

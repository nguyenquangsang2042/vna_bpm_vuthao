using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.CustomControlClass
{
    public sealed class ButtonsActionTopBar: UIView
    {
        AppDelegate appD = UIApplication.SharedApplication.Delegate as AppDelegate;

        UIView view_todoList, view_create;
        UIButton BT_create, BT_todoList;
        UIImageView iv_todoList, iv_create;
        UILabel lbl_todoList_num, lbl_create;

        public MainView mainView { get; set; }
        CreateNewTaskView createNewTaskView { get; set; }
        ToDoDetailView toDoDetailView { get; set; }

        private ButtonsActionTopBar()
        {
            view_todoList = new UIView();

            iv_todoList = new UIImageView();
            iv_todoList.Image = UIImage.FromFile("Icons/icon_bell.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            iv_todoList.TintColor = UIColor.White;

            lbl_todoList_num = new UILabel() { 
                Font = UIFont.SystemFontOfSize(11),
                TextColor = UIColor.White,
                BackgroundColor = UIColor.Red,
                TextAlignment  = UITextAlignment.Center
            };
            lbl_todoList_num.Text = "19";

            BT_todoList = new UIButton();

            // tam an tinh nang chua co
            //this.Add(view_todoList);

            view_todoList.AddSubviews(new UIView[] { iv_todoList, lbl_todoList_num, BT_todoList });

            view_create = new UIView();
            view_create.BackgroundColor = UIColor.White;

            iv_create = new UIImageView();
            iv_create.Image = UIImage.FromFile("Icons/icon_create_new.png");

            lbl_create = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(15),
                TextColor = UIColor.FromRGB(65, 80, 134)
            };
            lbl_create.Text = "Tạo yêu cầu";

            BT_create = new UIButton();

            // tam an tinh nang chua co
            //this.Add(view_create);

            view_create.AddSubviews(new UIView[] { iv_create, lbl_create, BT_create });

            if (BT_create != null)
                BT_create.AddTarget(HandleBtnCreate, UIControlEvent.TouchUpInside);

            if (BT_todoList != null)
                BT_todoList.AddTarget(HandleBtnTodoList, UIControlEvent.TouchUpInside);
        }

        private static ButtonsActionTopBar instance = null;
        public static ButtonsActionTopBar Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ButtonsActionTopBar();
                }
                return instance;
            }
        }

        public void InitFrameView(CGRect frame)
        {
            if (this.Frame == CGRect.Empty)
            {
                this.Frame = new CGRect(frame.Width - 230, 0, 230, frame.Height);

                var positionY = (Frame.Height - (Frame.Height - 20)) / 2;
                view_todoList.Frame = new CGRect( Frame.Width - 95, positionY, 95, Frame.Height - 20);
                iv_todoList.Frame = new CGRect((view_todoList.Frame.Width - 20) / 2, (view_todoList.Frame.Height - 20) / 2, 20, 20);
                lbl_todoList_num.Frame = new CGRect(view_todoList.Frame.Width / 2, 0, 18, 18);
                BT_todoList.Frame = new CGRect(15, 0, view_todoList.Frame.Width - 30, view_todoList.Frame.Height);

                lbl_todoList_num.Layer.CornerRadius = 9;
                lbl_todoList_num.Layer.MasksToBounds = true;
                lbl_todoList_num.ClipsToBounds = true;

                var positionYCreate = (Frame.Height - (Frame.Height - 25)) / 2;
                view_create.Frame = new CGRect(0, positionYCreate, Frame.Width - 95, Frame.Height - 25);
                iv_create.Frame = new CGRect(15, (view_create.Frame.Height - 15) / 2, 15, 15);
                lbl_create.Frame = new CGRect(iv_create.Frame.Right + 10, 0, view_create.Frame.Width - (iv_create.Frame.Right + 10), view_create.Frame.Height);
                BT_create.Frame = new CGRect(0, 0, view_create.Frame.Width, view_create.Frame.Height);

                view_create.Layer.CornerRadius = 3;
                view_create.Layer.MasksToBounds = true;
                view_create.ClipsToBounds = true;
            }
        }

        #region event
        private void HandleBtnCreate(object sender, EventArgs e)
        {
            if (createNewTaskView == null)
                createNewTaskView = (CreateNewTaskView)mainView.Storyboard.InstantiateViewController("CreateNewTaskView");

            if (appD.SlideMenuController.MainViewController.GetType() != typeof(CreateNewTaskView))
            {
                appD.SlideMenuController.ChangeMainViewcontroller(createNewTaskView, true);
                ButtonsActionBotBar buttonActionBotBar = ButtonsActionBotBar.Instance;
                buttonActionBotBar.LoadStatusButton(-1);
            }
        }

        private void HandleBtnTodoList(object sender, EventArgs e)
        {
            if (toDoDetailView == null)
                toDoDetailView = (ToDoDetailView)mainView.Storyboard.InstantiateViewController("ToDoDetailView");

            if (appD.SlideMenuController.MainViewController.GetType() != typeof(ToDoDetailView))
            {
                appD.SlideMenuController.ChangeMainViewcontroller(toDoDetailView, true);
                ButtonsActionBotBar buttonActionBotBar = ButtonsActionBotBar.Instance;
                buttonActionBotBar.LoadStatusButton(1);
            }
        }
        #endregion
    }
}
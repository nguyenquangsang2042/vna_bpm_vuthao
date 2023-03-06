using BPMOPMobile.Bean;
using BPMOPMobile.iPad.CustomControlClass;
using BPMOPMobile.iPad.IOSClass;
using BPMOPMobile.iPad.ViewControllers;
using BPMOPMobile.iPad.ViewControllers.Applications;
using CoreGraphics;
using Foundation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UIKit;

namespace BPMOPMobile.iPad
{
    public partial class FormEditAttachFileView : UIViewController
    {
        BeanAttachFile currentAttachFile { get; set; }
        List<ClassMenu> lst_menuItem = new List<ClassMenu>();
        ClassMenu menuItemSelect = new ClassMenu();

        UIViewController parentView { get; set; }
        string localDocumentFilepath = "";
        ViewElement element;

        public FormEditAttachFileView(IntPtr handle) : base(handle)
        {
            localDocumentFilepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewConfiguration();
            LoadContent();

            #region delegate
            BT_close.TouchUpInside += BT_close_TouchUpInside;
            BT_type.TouchUpInside += BT_type_TouchUpInside;
            BT_save.TouchUpInside += BT_save_TouchUpInside;
            BT_back.TouchUpInside += BT_back_TouchUpInside;
            BT_name.TouchUpInside += BT_name_TouchUpInside;
            #endregion
        }

        #region public - private method
        private void ViewConfiguration()
        {
            BT_close.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);
            BT_save.ContentEdgeInsets = new UIEdgeInsets(7, 5, 7, 5);
            BT_back.ContentEdgeInsets = new UIEdgeInsets(7, 7, 7, 7);

            CmmIOSFunction.AddBorderView(view_type);

            UIImageView iv_right = new UIImageView();
            iv_right.Image = UIImage.FromFile("Icons/icon_arrow_down_black.png");
            iv_right.ContentMode = UIViewContentMode.ScaleAspectFit;
            iv_right.Frame = new CGRect(0, 0, 14, 14);

            UIView rightView = new UIView(new CGRect(0, 0, 29, 14));
            rightView.AddSubview(iv_right);

            tf_type.RightView = rightView;
            tf_type.RightViewMode = UITextFieldViewMode.Always;

            view_detailAttachFile.Frame = new CGRect(this.View.Frame.Right, 0, view_detailAttachFile.Frame.Width, view_detailAttachFile.Frame.Height);
        }
        //List<BeanAttachFileCategory> lst_attachCate;
        private void LoadContent()
        {
            BT_name.SetTitle(currentAttachFile.Title, UIControlState.Normal);

            if (!string.IsNullOrEmpty(currentAttachFile.AttachTypeName))
                tf_type.Text = currentAttachFile.AttachTypeName;

            var lst_attachCate = JsonConvert.DeserializeObject<List<BeanAttachFileCategory>>(element.DataSource);
            string attachCate = "";
            if (!string.IsNullOrEmpty(currentAttachFile.AttachTypeName))
            {
                attachCate = currentAttachFile.AttachTypeName;
            }
            if (lst_attachCate != null)
            {
                foreach (var item in lst_attachCate)
                {
                    if (item.Title == attachCate)
                    {
                        item.IsSelected = true;
                        menuItemSelect = new ClassMenu() { ID = item.ID, isSelected = item.IsSelected, title = item.Title };
                    }
                    else
                        item.IsSelected = false;
                    ClassMenu m1 = new ClassMenu() { ID = item.ID, isSelected = item.IsSelected, title = item.Title };
                    lst_menuItem.Add(m1);
                }
            }

            //ClassMenu m1 = new ClassMenu() { ID = 0, title = "Biên bản"};
            //ClassMenu m2 = new ClassMenu() { ID = 1, title = "Tờ trình" };
            //ClassMenu m3 = new ClassMenu() { ID = 2, title = "Công văn" };
            //ClassMenu m4 = new ClassMenu() { ID = 3, title = "Bản vẽ" };
            //ClassMenu m5 = new ClassMenu() { ID = 4, title = "Hạ tầng" };
            //ClassMenu m6 = new ClassMenu() { ID = 5, title = "Hoá đơn" };
            //ClassMenu m7 = new ClassMenu() { ID = 6, title = "Nghị quyết" };
            //ClassMenu m8 = new ClassMenu() { ID = 7, title = "Quyết định" };
            //ClassMenu m9 = new ClassMenu() { ID = 8, title = "Yêu cầu" };
            //ClassMenu m10 = new ClassMenu() { ID = 9, title = "Khác" };

            //lst_menuItem.AddRange(new[] { m1, m2, m3, m4, m5, m6, m7, m8, m9, m10 });
        }

        public void SetContent(UIViewController _parentView, BeanAttachFile _attachment, ViewElement _element = null)
        {
            parentView = _parentView;
            currentAttachFile = _attachment;
            element = _element;
        }

        private void OpenFile(string localfilename)
        {
            string localfilePath = Path.Combine(localDocumentFilepath, localfilename);
            webView_detailAttachFile.LoadRequest(new NSUrlRequest(new NSUrl(localfilePath, false)));
            webView_detailAttachFile.ScalesPageToFit = true;
        }
        #endregion

        #region event
        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            this.DismissModalViewController(true);
        }

        private void BT_type_TouchUpInside(object sender, EventArgs e)
        {
            Custom_MenuOption custom_menuOption = Custom_MenuOption.Instance;
            if (custom_menuOption.Superview != null)
                custom_menuOption.RemoveFromSuperview();
            else
            {
                var height = lst_menuItem.Count * custom_menuOption.RowHeigth;
                var maxHeight = view_editContent.Frame.Height - BT_type.Frame.Bottom;
                var menuHeight = (height > maxHeight) ? maxHeight : height;

                custom_menuOption.ItemNoIcon = true;
                custom_menuOption.viewController = this;
                custom_menuOption.InitFrameView(new CGRect(BT_type.Frame.Left, BT_type.Frame.Bottom + 2, BT_type.Frame.Width, menuHeight));
                custom_menuOption.AddShadowForView();
                custom_menuOption.ListItemMenu = lst_menuItem;
                custom_menuOption.TableLoadData();

                view_editContent.AddSubview(custom_menuOption);
                view_editContent.BringSubviewToFront(custom_menuOption);
            }
        }

        private void BT_save_TouchUpInside(object sender, EventArgs e)
        {
            if (parentView != null && parentView.GetType() == typeof(CreateNewTaskView))
            {
                currentAttachFile.Type = tf_type.Text;
                CreateNewTaskView controller = (CreateNewTaskView)parentView;
                controller.HandleEditAttachFileResult(currentAttachFile);
            }
            else if (parentView.GetType() == typeof(ToDoDetailView))
            {
                ToDoDetailView controller = (ToDoDetailView)parentView;
                //attachFile.Category = fileCategorySelected.ID + ";#" + fileCategorySelected.DocumentTypeValue;
                currentAttachFile.AttachTypeId = menuItemSelect.ID;
                currentAttachFile.AttachTypeName = menuItemSelect.title;
                controller.ReloadAttachmentElement(element, currentAttachFile);
            }
            else if (parentView.GetType() == typeof(WorkflowDetailView))
            {
                WorkflowDetailView controller = (WorkflowDetailView)parentView;
                //attachFile.Category = fileCategorySelected.ID + ";#" + fileCategorySelected.DocumentTypeValue;
                currentAttachFile.AttachTypeId = menuItemSelect.ID;
                currentAttachFile.AttachTypeName = menuItemSelect.title;
                controller.ReloadAttachmentElement(element, currentAttachFile);
            }
            else if (parentView.GetType() == typeof(FormWorkFlowDetails))
            {
                FormWorkFlowDetails controller = (FormWorkFlowDetails)parentView;
                //attachFile.Category = fileCategorySelected.ID + ";#" + fileCategorySelected.DocumentTypeValue;
                currentAttachFile.AttachTypeId = menuItemSelect.ID;
                currentAttachFile.AttachTypeName = menuItemSelect.title;
                controller.ReloadAttachmentElement(element, currentAttachFile);
            }
            else if (parentView.GetType() == typeof(FollowListViewController))
            {
                FollowListViewController controller = (FollowListViewController)parentView;
                //attachFile.Category = fileCategorySelected.ID + ";#" + fileCategorySelected.DocumentTypeValue;
                currentAttachFile.AttachTypeId = menuItemSelect.ID;
                currentAttachFile.AttachTypeName = menuItemSelect.title;
                controller.ReloadAttachmentElement(element, currentAttachFile);
            }

            this.DismissModalViewController(true);
        }

        private void BT_name_TouchUpInside(object sender, EventArgs e)
        {
            lbl_title.Text = currentAttachFile.Title;
            OpenFile(currentAttachFile.Path);
            view_detailAttachFile.Hidden = false;

            view_detailAttachFile.Frame = new CGRect(this.View.Frame.Right, 0, view_detailAttachFile.Frame.Width, view_detailAttachFile.Frame.Height);
            UIView.BeginAnimations("show_animationShowTable");
            UIView.SetAnimationDuration(0.2f);
            UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
            UIView.SetAnimationRepeatCount(0);
            UIView.SetAnimationRepeatAutoreverses(false);
            UIView.SetAnimationDelegate(this);
            view_detailAttachFile.Frame = new CGRect(0, 0, view_detailAttachFile.Frame.Width, view_detailAttachFile.Frame.Height);
            UIView.CommitAnimations();
        }

        private void BT_back_TouchUpInside(object sender, EventArgs e)
        {
            var delayTimer = new Timer((state) => InvokeOnMainThread(() => view_detailAttachFile.Hidden = true), null, 200, Timeout.Infinite);

            view_detailAttachFile.Frame = new CGRect(0, 0, view_detailAttachFile.Frame.Width, view_detailAttachFile.Frame.Height);
            UIView.BeginAnimations("show_animationShowTable");
            UIView.SetAnimationDuration(0.2f);
            UIView.SetAnimationCurve(UIViewAnimationCurve.Linear);
            UIView.SetAnimationRepeatCount(0);
            UIView.SetAnimationRepeatAutoreverses(false);
            UIView.SetAnimationDelegate(this);
            view_detailAttachFile.Frame = new CGRect(this.View.Frame.Right, 0, view_detailAttachFile.Frame.Width, view_detailAttachFile.Frame.Height);
            UIView.CommitAnimations();
        }
        //private void LoadContent()
        //{
        //    BT_name.SetTitle(currentAttachFile.Title, UIControlState.Normal);

        //    if (!string.IsNullOrEmpty(currentAttachFile.AttachTypeName))
        //        tf_type.Text = currentAttachFile.AttachTypeName;
        public void HandleMenuOptionResult(ClassMenu _menu)
        {
            tf_type.Text = _menu.title;
            menuItemSelect = _menu;
            foreach (var item in lst_menuItem)
            {
                if (item.ID == _menu.ID)
                {
                    item.isSelected = true;
                }
                else
                    item.isSelected = false;
            }


            Custom_MenuOption custom_menuOption = Custom_MenuOption.Instance;
            if (custom_menuOption.Superview != null)
                custom_menuOption.RemoveFromSuperview();
        }
        #endregion
    }
}
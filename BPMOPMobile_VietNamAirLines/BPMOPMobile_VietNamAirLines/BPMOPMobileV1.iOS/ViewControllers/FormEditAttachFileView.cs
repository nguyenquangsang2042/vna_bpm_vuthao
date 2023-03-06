using System;
using System.Collections.Generic;
using BPMOPMobile.Bean;
using BPMOPMobileV1.iOS.CustomControlClass;
using BPMOPMobileV1.iOS.IOSClass;
using CoreGraphics;
using UIKit;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class FormEditAttachFileView : UIViewController
    {
        BeanAttachFile currentAttachFile { get; set; }
        List<ClassMenu> lst_menuItem = new List<ClassMenu>();
        UIViewController parentView { get; set; }

        public FormEditAttachFileView(IntPtr handle) : base(handle)
        {
        }

        #region override
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewConfiguration();
            SetLangTitle();
            LoadContent();

            #region delegate
            BT_name.TouchUpInside += BT_name_TouchUpInside;
            BT_close.TouchUpInside += BT_close_TouchUpInside;
            BT_type.TouchUpInside += BT_type_TouchUpInside;
            #endregion
        }

        #endregion

        #region private - public method
        private void ViewConfiguration()
        {
            BT_type.Layer.BorderColor = UIColor.LightGray.ColorWithAlpha(0.3f).CGColor;
            BT_type.Layer.BorderWidth = 0.5f;
        }

        private void LoadContent()
        {
            BT_name.SetTitle(currentAttachFile.Title, UIControlState.Normal);
            if (!string.IsNullOrEmpty(currentAttachFile.Type))
                BT_type.SetTitle(currentAttachFile.Type, UIControlState.Normal);

            ClassMenu m1 = new ClassMenu() { ID = 0, title = "Biên bản" };
            ClassMenu m2 = new ClassMenu() { ID = 1, title = "Tờ trình" };
            ClassMenu m3 = new ClassMenu() { ID = 2, title = "Công văn" };
            ClassMenu m4 = new ClassMenu() { ID = 3, title = "Khác" };
            lst_menuItem.AddRange(new[] { m1, m2, m3, m4 });
        }

        public void SetContent(UIViewController _parentView, BeanAttachFile _attachment)
        {
            parentView = _parentView;
            currentAttachFile = _attachment;
        }

        private void SetLangTitle()
        {
            CmmIOSFunction.SetLangToView(this.View);
        }
        #endregion

        #region events
        private void BT_close_TouchUpInside(object sender, EventArgs e)
        {
            if (this.NavigationController != null)
                this.NavigationController.PopViewController(true);
            else
            {
                this.DismissModalViewController(true);
            }
        }
        private void BT_name_TouchUpInside(object sender, EventArgs e)
        {
            ShowAttachmentView showAttachmentView = Storyboard.InstantiateViewController("ShowAttachmentView") as ShowAttachmentView;
            showAttachmentView.setContent(parentView, currentAttachFile);
            this.PresentModalViewController(showAttachmentView, true);
        }
        private void BT_type_TouchUpInside(object sender, EventArgs e)
        {
            Custom_MenuOption custom_menuOption = Custom_MenuOption.Instance;
            if (custom_menuOption.Superview != null)
                custom_menuOption.RemoveFromSuperview();
            else
            {
                custom_menuOption.ItemNoIcon = true;
                custom_menuOption.viewController = this;
                custom_menuOption.InitFrameView(new CGRect(BT_type.Frame.Left, BT_type.Frame.Bottom + 2, BT_type.Frame.Width, 4 * custom_menuOption.RowHeigth));
                custom_menuOption.lst_menu = lst_menuItem;
                custom_menuOption.TableLoadData();
                custom_menuOption.Layer.BorderColor = UIColor.FromRGB(216, 216, 216).CGColor;
                custom_menuOption.Layer.CornerRadius = 3;
                custom_menuOption.Layer.BorderWidth = 1;
                custom_menuOption.ClipsToBounds = true;

                view_editContent.AddSubview(custom_menuOption);
                view_editContent.BringSubviewToFront(custom_menuOption);
            }
        }
        #endregion
    }
}


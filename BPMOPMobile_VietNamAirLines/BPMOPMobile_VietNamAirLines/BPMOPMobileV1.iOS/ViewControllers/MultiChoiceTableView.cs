
using System;
using System.Collections.Generic;
using System.Drawing;
using CoreGraphics;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobileV1.iOS.IOSClass;
using Foundation;
using Newtonsoft.Json;
using UIKit;
using Newtonsoft.Json.Linq;
using BPMOPMobileV1.iOS.CustomControlClass;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    public partial class MultiChoiceTableView : UIViewController
    {
        UIViewController parentView { get; set; }
        bool isMultiSelect;
        string title = "";
        public UIView view_table_content;
        public UIView view_table_content_Temp;

        public MultiChoiceTableView(IntPtr handle) : base(handle)
        {
        }

        #region override
        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewConfiguration();
            setlangTitle();

            #region delegate
            BT_back.TouchUpInside += BT_back_TouchUpInside;
            BT_done.TouchUpInside += BT_Done_TouchUpInside;
            #endregion
        }
        #endregion

        #region private - public method
        public void setContent(UIViewController _parentView, bool _isMultiSelect, UIView _view_table_content, string _title)
        {
            isMultiSelect = _isMultiSelect;
            parentView = _parentView;
            view_table_content = _view_table_content;
            title = _title;
            //view_table_content_Temp = _view_table_content.Copy() as UIView;
        }

        private void ViewConfiguration()
        {
            headerView_constantHeight.Constant = 45 + 10 + CmmIOSFunction.GetHeaderViewHeight();
            var topViewHeight = headerView_constantHeight.Constant;
            BT_back.ImageEdgeInsets = new UIEdgeInsets(3, 3, 3, 3);
            BT_done.ImageEdgeInsets = new UIEdgeInsets(3, 3, 3, 3);
            lbl_title.Text = title;

            if (isMultiSelect)
                BT_done.Hidden = false;
            else
                BT_done.Hidden = true;

            if (view_table_content.GetType() == typeof(Custom_AppStatusCategory))
            {
                Custom_AppStatusCategory custom_AppStatusCategory = view_table_content as Custom_AppStatusCategory;
                custom_AppStatusCategory.InitFrameView(new CGRect(top_view.Frame.Left, topViewHeight, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height - topViewHeight));
            }
            else if (view_table_content.GetType() == typeof(Custom_DuedateCategory))
            {
                Custom_DuedateCategory custom_DuedateCategory = view_table_content as Custom_DuedateCategory;
                custom_DuedateCategory.InitFrameView(new CGRect(top_view.Frame.Left, topViewHeight, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height - topViewHeight));
            }
            else if (view_table_content.GetType() == typeof(Custom_AppConditionCategory))
            {
                Custom_AppConditionCategory custom_AppConditionCategory = view_table_content as Custom_AppConditionCategory;
                custom_AppConditionCategory.InitFrameView(new CGRect(top_view.Frame.Left, topViewHeight, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height - topViewHeight));
            }
            else if (view_table_content.GetType() == typeof(Custom_MenuOption))
            {
                Custom_MenuOption custom_MenuOption = view_table_content as Custom_MenuOption;
                custom_MenuOption.InitFrameView(new CGRect(top_view.Frame.Left, topViewHeight, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height - topViewHeight));
            }

            View.AddSubview(view_table_content);
            View.BringSubviewToFront(view_table_content);
        }

        private void setlangTitle()
        {
            CmmIOSFunction.SetLangToView(this.View);
        }
        #endregion

        #region events
        private void BT_back_TouchUpInside(object sender, EventArgs e)
        {
            if (isMultiSelect)
            {
                if (view_table_content.GetType() == typeof(Custom_AppStatusCategory))
                {
                    Custom_AppStatusCategory custom_AppStatusCategory = view_table_content as Custom_AppStatusCategory;
                    custom_AppStatusCategory.CallBackupData();
                }
                else if (view_table_content.GetType() == typeof(Custom_DuedateCategory))
                {
                    Custom_DuedateCategory custom_DuedateCategory = view_table_content as Custom_DuedateCategory;
                    custom_DuedateCategory.CallBackupData();
                }
                else if (view_table_content.GetType() == typeof(Custom_DuedateCategory))
                {
                    Custom_AppConditionCategory custom_AppConditionCategory = view_table_content as Custom_AppConditionCategory;
                    custom_AppConditionCategory.CallBackupData();
                }
                else if (view_table_content.GetType() == typeof(Custom_MenuOption))
                {
                    Custom_MenuOption custom_MenuOption = view_table_content as Custom_MenuOption;
                    custom_MenuOption.CallBackupData();
                }
            }

            if (parentView != null)
            {
                if (parentView is MainView)
                    (parentView as MainView).isBackFromFilter = true;
                else if (parentView is RequestListView)
                    (parentView as RequestListView).isBackFromFilter = true;
                else if (parentView is MyRequestListView)
                    (parentView as MyRequestListView).isBackFromFilter = true;
            }
            this.NavigationController.PopViewController(true);
        }

        void BT_Done_TouchUpInside(object sender, EventArgs e)
        {
            ///Cập nhật lại danh sách các trạng thái được chọn
            if (view_table_content.GetType() == typeof(Custom_AppStatusCategory) && parentView is MainView)
            {
                Custom_AppStatusCategory custom_AppStatusCategory = view_table_content as Custom_AppStatusCategory;
                MainView parent = parentView as MainView;
                if (parent.tab_toMe)
                    parent.ListAppStatus_selected_toMe = custom_AppStatusCategory.ListAppStatus_selected;
                else
                    parent.ListAppStatus_selected_fromMe = custom_AppStatusCategory.ListAppStatus_selected;
            }

            if (parentView != null)
            {
                if (parentView is MainView)
                    (parentView as MainView).isBackFromFilter = true;
                else if (parentView is RequestListView)
                    (parentView as RequestListView).isBackFromFilter = true;
                else if (parentView is MyRequestListView)
                    (parentView as MyRequestListView).isBackFromFilter = true;
            }

            this.NavigationController.PopViewController(true);
        }
        #endregion
    }
}

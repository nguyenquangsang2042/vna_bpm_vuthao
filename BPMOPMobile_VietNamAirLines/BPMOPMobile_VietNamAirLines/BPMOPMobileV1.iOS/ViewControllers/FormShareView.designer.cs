// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    [Register ("FormShareView")]
    partial class FormShareView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_cancel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_selectUser { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_submit { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint headerView_constantHeight { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_title { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView table_shareList { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView textview_ykien { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField tf_placeholder { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UICollectionView User_CollectionView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_content { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_shareItem { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_userSelected { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint view_userselected_hehght_Constant { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BT_cancel != null) {
                BT_cancel.Dispose ();
                BT_cancel = null;
            }

            if (BT_selectUser != null) {
                BT_selectUser.Dispose ();
                BT_selectUser = null;
            }

            if (BT_submit != null) {
                BT_submit.Dispose ();
                BT_submit = null;
            }

            if (headerView_constantHeight != null) {
                headerView_constantHeight.Dispose ();
                headerView_constantHeight = null;
            }

            if (lbl_title != null) {
                lbl_title.Dispose ();
                lbl_title = null;
            }

            if (table_shareList != null) {
                table_shareList.Dispose ();
                table_shareList = null;
            }

            if (textview_ykien != null) {
                textview_ykien.Dispose ();
                textview_ykien = null;
            }

            if (tf_placeholder != null) {
                tf_placeholder.Dispose ();
                tf_placeholder = null;
            }

            if (User_CollectionView != null) {
                User_CollectionView.Dispose ();
                User_CollectionView = null;
            }

            if (view_content != null) {
                view_content.Dispose ();
                view_content = null;
            }

            if (view_shareItem != null) {
                view_shareItem.Dispose ();
                view_shareItem = null;
            }

            if (view_userSelected != null) {
                view_userSelected.Dispose ();
                view_userSelected = null;
            }

            if (view_userselected_hehght_Constant != null) {
                view_userselected_hehght_Constant.Dispose ();
                view_userselected_hehght_Constant = null;
            }
        }
    }
}
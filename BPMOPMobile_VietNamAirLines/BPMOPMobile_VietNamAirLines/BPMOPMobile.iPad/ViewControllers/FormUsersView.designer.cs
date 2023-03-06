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

namespace BPMOPMobile.iPad
{
    [Register ("FormUsersView")]
    partial class FormUsersView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_approve { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_close { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint collectionUser_heightConstant { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_title { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint selectedUserView_heightConstant { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView table_content { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField tf_search { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UICollectionView User_CollectionView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_content { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_searchUser { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BT_approve != null) {
                BT_approve.Dispose ();
                BT_approve = null;
            }

            if (BT_close != null) {
                BT_close.Dispose ();
                BT_close = null;
            }

            if (collectionUser_heightConstant != null) {
                collectionUser_heightConstant.Dispose ();
                collectionUser_heightConstant = null;
            }

            if (lbl_title != null) {
                lbl_title.Dispose ();
                lbl_title = null;
            }

            if (selectedUserView_heightConstant != null) {
                selectedUserView_heightConstant.Dispose ();
                selectedUserView_heightConstant = null;
            }

            if (table_content != null) {
                table_content.Dispose ();
                table_content = null;
            }

            if (tf_search != null) {
                tf_search.Dispose ();
                tf_search = null;
            }

            if (User_CollectionView != null) {
                User_CollectionView.Dispose ();
                User_CollectionView = null;
            }

            if (view_content != null) {
                view_content.Dispose ();
                view_content = null;
            }

            if (view_searchUser != null) {
                view_searchUser.Dispose ();
                view_searchUser = null;
            }
        }
    }
}
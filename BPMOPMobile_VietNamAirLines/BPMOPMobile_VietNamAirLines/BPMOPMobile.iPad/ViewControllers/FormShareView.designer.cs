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
    [Register ("FormShareView")]
    partial class FormShareView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_close { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_selectUser { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_submit { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_noteTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_userTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIScrollView scrollview_content { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint share_heightConstraint { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView table_shareUser { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField tf_placeholder { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView txt_note { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UICollectionView User_CollectionView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint userSelectedConstantHeight { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_line1 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_line2 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_line3 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_note { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_shareItem { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_shareUserSelected { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint view_tableUser_height_Constant { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BT_close != null) {
                BT_close.Dispose ();
                BT_close = null;
            }

            if (BT_selectUser != null) {
                BT_selectUser.Dispose ();
                BT_selectUser = null;
            }

            if (BT_submit != null) {
                BT_submit.Dispose ();
                BT_submit = null;
            }

            if (lbl_noteTitle != null) {
                lbl_noteTitle.Dispose ();
                lbl_noteTitle = null;
            }

            if (lbl_userTitle != null) {
                lbl_userTitle.Dispose ();
                lbl_userTitle = null;
            }

            if (scrollview_content != null) {
                scrollview_content.Dispose ();
                scrollview_content = null;
            }

            if (share_heightConstraint != null) {
                share_heightConstraint.Dispose ();
                share_heightConstraint = null;
            }

            if (table_shareUser != null) {
                table_shareUser.Dispose ();
                table_shareUser = null;
            }

            if (tf_placeholder != null) {
                tf_placeholder.Dispose ();
                tf_placeholder = null;
            }

            if (txt_note != null) {
                txt_note.Dispose ();
                txt_note = null;
            }

            if (User_CollectionView != null) {
                User_CollectionView.Dispose ();
                User_CollectionView = null;
            }

            if (userSelectedConstantHeight != null) {
                userSelectedConstantHeight.Dispose ();
                userSelectedConstantHeight = null;
            }

            if (view_line1 != null) {
                view_line1.Dispose ();
                view_line1 = null;
            }

            if (view_line2 != null) {
                view_line2.Dispose ();
                view_line2 = null;
            }

            if (view_line3 != null) {
                view_line3.Dispose ();
                view_line3 = null;
            }

            if (view_note != null) {
                view_note.Dispose ();
                view_note = null;
            }

            if (view_shareItem != null) {
                view_shareItem.Dispose ();
                view_shareItem = null;
            }

            if (view_shareUserSelected != null) {
                view_shareUserSelected.Dispose ();
                view_shareUserSelected = null;
            }

            if (view_tableUser_height_Constant != null) {
                view_tableUser_height_Constant.Dispose ();
                view_tableUser_height_Constant = null;
            }
        }
    }
}
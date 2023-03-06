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
    [Register ("RequestAddInfo")]
    partial class RequestAddInfo
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_agree { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_cancel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_selectUser { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UICollectionView collection_attach { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView img_avatar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_char_title_userSelected { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_chonnguoixuly { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_email_userSelected { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_title { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_title_userSelected { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView textview_ykien { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_userSelected { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BT_agree != null) {
                BT_agree.Dispose ();
                BT_agree = null;
            }

            if (BT_cancel != null) {
                BT_cancel.Dispose ();
                BT_cancel = null;
            }

            if (BT_selectUser != null) {
                BT_selectUser.Dispose ();
                BT_selectUser = null;
            }

            if (collection_attach != null) {
                collection_attach.Dispose ();
                collection_attach = null;
            }

            if (img_avatar != null) {
                img_avatar.Dispose ();
                img_avatar = null;
            }

            if (lbl_char_title_userSelected != null) {
                lbl_char_title_userSelected.Dispose ();
                lbl_char_title_userSelected = null;
            }

            if (lbl_chonnguoixuly != null) {
                lbl_chonnguoixuly.Dispose ();
                lbl_chonnguoixuly = null;
            }

            if (lbl_email_userSelected != null) {
                lbl_email_userSelected.Dispose ();
                lbl_email_userSelected = null;
            }

            if (lbl_title != null) {
                lbl_title.Dispose ();
                lbl_title = null;
            }

            if (lbl_title_userSelected != null) {
                lbl_title_userSelected.Dispose ();
                lbl_title_userSelected = null;
            }

            if (textview_ykien != null) {
                textview_ykien.Dispose ();
                textview_ykien = null;
            }

            if (view_userSelected != null) {
                view_userSelected.Dispose ();
                view_userSelected = null;
            }
        }
    }
}
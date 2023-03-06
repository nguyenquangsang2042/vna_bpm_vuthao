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
    [Register ("MenuView")]
    partial class MenuView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView iv_avatar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_mail { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_name { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView table_menu { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_top { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (iv_avatar != null) {
                iv_avatar.Dispose ();
                iv_avatar = null;
            }

            if (lbl_mail != null) {
                lbl_mail.Dispose ();
                lbl_mail = null;
            }

            if (lbl_name != null) {
                lbl_name.Dispose ();
                lbl_name = null;
            }

            if (table_menu != null) {
                table_menu.Dispose ();
                table_menu = null;
            }

            if (view_top != null) {
                view_top.Dispose ();
                view_top = null;
            }
        }
    }
}
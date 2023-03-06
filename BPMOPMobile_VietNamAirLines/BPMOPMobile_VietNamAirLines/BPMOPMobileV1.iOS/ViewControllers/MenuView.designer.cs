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
    [Register ("MenuView")]
    partial class MenuView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint headerView_constainHeight { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView img_avatar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_email { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_header_line { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_name { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView table_menu { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint table_menu_constaintHeight { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView top_view { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WebKit.WKWebView webkit_ad { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (headerView_constainHeight != null) {
                headerView_constainHeight.Dispose ();
                headerView_constainHeight = null;
            }

            if (img_avatar != null) {
                img_avatar.Dispose ();
                img_avatar = null;
            }

            if (lbl_email != null) {
                lbl_email.Dispose ();
                lbl_email = null;
            }

            if (lbl_header_line != null) {
                lbl_header_line.Dispose ();
                lbl_header_line = null;
            }

            if (lbl_name != null) {
                lbl_name.Dispose ();
                lbl_name = null;
            }

            if (table_menu != null) {
                table_menu.Dispose ();
                table_menu = null;
            }

            if (table_menu_constaintHeight != null) {
                table_menu_constaintHeight.Dispose ();
                table_menu_constaintHeight = null;
            }

            if (top_view != null) {
                top_view.Dispose ();
                top_view = null;
            }

            if (webkit_ad != null) {
                webkit_ad.Dispose ();
                webkit_ad = null;
            }
        }
    }
}
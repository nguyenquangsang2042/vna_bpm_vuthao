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

namespace BPMOPMobile.iPad.ViewControllers
{
    [Register ("FormListItemsChoice")]
    partial class FormListItemsChoice
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_approve { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_clear { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_close { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint Constant_left_BT_approve { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint Constant_width_BT_approve { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_title { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel separator_line { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView table_content { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_content { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BT_approve != null) {
                BT_approve.Dispose ();
                BT_approve = null;
            }

            if (BT_clear != null) {
                BT_clear.Dispose ();
                BT_clear = null;
            }

            if (BT_close != null) {
                BT_close.Dispose ();
                BT_close = null;
            }

            if (Constant_left_BT_approve != null) {
                Constant_left_BT_approve.Dispose ();
                Constant_left_BT_approve = null;
            }

            if (Constant_width_BT_approve != null) {
                Constant_width_BT_approve.Dispose ();
                Constant_width_BT_approve = null;
            }

            if (lbl_title != null) {
                lbl_title.Dispose ();
                lbl_title = null;
            }

            if (separator_line != null) {
                separator_line.Dispose ();
                separator_line = null;
            }

            if (table_content != null) {
                table_content.Dispose ();
                table_content = null;
            }

            if (view_content != null) {
                view_content.Dispose ();
                view_content = null;
            }
        }
    }
}
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
    [Register ("FormDateTimeController")]
    partial class FormDateTimeController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_accept { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_clear { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_close { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_currentDate { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint constraintHeightRoot { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_subtitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_title { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_content { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_control { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BT_accept != null) {
                BT_accept.Dispose ();
                BT_accept = null;
            }

            if (BT_clear != null) {
                BT_clear.Dispose ();
                BT_clear = null;
            }

            if (BT_close != null) {
                BT_close.Dispose ();
                BT_close = null;
            }

            if (BT_currentDate != null) {
                BT_currentDate.Dispose ();
                BT_currentDate = null;
            }

            if (constraintHeightRoot != null) {
                constraintHeightRoot.Dispose ();
                constraintHeightRoot = null;
            }

            if (lbl_subtitle != null) {
                lbl_subtitle.Dispose ();
                lbl_subtitle = null;
            }

            if (lbl_title != null) {
                lbl_title.Dispose ();
                lbl_title = null;
            }

            if (view_content != null) {
                view_content.Dispose ();
                view_content = null;
            }

            if (view_control != null) {
                view_control.Dispose ();
                view_control = null;
            }
        }
    }
}
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
    [Register ("FormApproveOrRejectView")]
    partial class FormApproveOrRejectView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_approve { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_close { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_note { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_noteTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_title { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView textview_content { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_note { get; set; }

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

            if (lbl_note != null) {
                lbl_note.Dispose ();
                lbl_note = null;
            }

            if (lbl_noteTitle != null) {
                lbl_noteTitle.Dispose ();
                lbl_noteTitle = null;
            }

            if (lbl_title != null) {
                lbl_title.Dispose ();
                lbl_title = null;
            }

            if (textview_content != null) {
                textview_content.Dispose ();
                textview_content = null;
            }

            if (view_note != null) {
                view_note.Dispose ();
                view_note = null;
            }
        }
    }
}
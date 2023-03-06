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
    [Register ("AgreeOrRejectView")]
    partial class AgreeOrRejectView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_cancel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_submit { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView img_title { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_note { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_title { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView textview_content { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_content { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BT_cancel != null) {
                BT_cancel.Dispose ();
                BT_cancel = null;
            }

            if (BT_submit != null) {
                BT_submit.Dispose ();
                BT_submit = null;
            }

            if (img_title != null) {
                img_title.Dispose ();
                img_title = null;
            }

            if (lbl_note != null) {
                lbl_note.Dispose ();
                lbl_note = null;
            }

            if (lbl_title != null) {
                lbl_title.Dispose ();
                lbl_title = null;
            }

            if (textview_content != null) {
                textview_content.Dispose ();
                textview_content = null;
            }

            if (view_content != null) {
                view_content.Dispose ();
                view_content = null;
            }
        }
    }
}
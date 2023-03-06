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
    [Register ("ToastMessageView")]
    partial class ToastMessageView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_moveDetail { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_content { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_title { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView toast_view { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BT_moveDetail != null) {
                BT_moveDetail.Dispose ();
                BT_moveDetail = null;
            }

            if (lbl_content != null) {
                lbl_content.Dispose ();
                lbl_content = null;
            }

            if (lbl_title != null) {
                lbl_title.Dispose ();
                lbl_title = null;
            }

            if (toast_view != null) {
                toast_view.Dispose ();
                toast_view = null;
            }
        }
    }
}
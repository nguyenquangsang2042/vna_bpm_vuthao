// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace BPMOPMobileV1.iOS.ViewControllers
{
    [Register ("MoreactionApp_cell")]
    partial class MoreactionApp_cell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView img_action { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_line { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_title { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (img_action != null) {
                img_action.Dispose ();
                img_action = null;
            }

            if (lbl_line != null) {
                lbl_line.Dispose ();
                lbl_line = null;
            }

            if (lbl_title != null) {
                lbl_title.Dispose ();
                lbl_title = null;
            }
        }
    }
}
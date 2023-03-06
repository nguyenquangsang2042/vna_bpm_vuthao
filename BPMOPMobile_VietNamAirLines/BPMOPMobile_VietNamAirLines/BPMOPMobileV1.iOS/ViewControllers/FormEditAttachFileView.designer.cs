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
    [Register ("FormEditAttachFileView")]
    partial class FormEditAttachFileView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_close { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_name { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_save { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_type { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView iv_btn_type { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_editContent { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BT_close != null) {
                BT_close.Dispose ();
                BT_close = null;
            }

            if (BT_name != null) {
                BT_name.Dispose ();
                BT_name = null;
            }

            if (BT_save != null) {
                BT_save.Dispose ();
                BT_save = null;
            }

            if (BT_type != null) {
                BT_type.Dispose ();
                BT_type = null;
            }

            if (iv_btn_type != null) {
                iv_btn_type.Dispose ();
                iv_btn_type = null;
            }

            if (view_editContent != null) {
                view_editContent.Dispose ();
                view_editContent = null;
            }
        }
    }
}
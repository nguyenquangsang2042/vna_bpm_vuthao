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
    [Register ("FormEditAttachFileView")]
    partial class FormEditAttachFileView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_back { get; set; }

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
        UIKit.UILabel lbl_title { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField tf_type { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_detailAttachFile { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_editContent { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_type { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIWebView webView_detailAttachFile { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BT_back != null) {
                BT_back.Dispose ();
                BT_back = null;
            }

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

            if (lbl_title != null) {
                lbl_title.Dispose ();
                lbl_title = null;
            }

            if (tf_type != null) {
                tf_type.Dispose ();
                tf_type = null;
            }

            if (view_detailAttachFile != null) {
                view_detailAttachFile.Dispose ();
                view_detailAttachFile = null;
            }

            if (view_editContent != null) {
                view_editContent.Dispose ();
                view_editContent = null;
            }

            if (view_type != null) {
                view_type.Dispose ();
                view_type = null;
            }

            if (webView_detailAttachFile != null) {
                webView_detailAttachFile.Dispose ();
                webView_detailAttachFile = null;
            }
        }
    }
}
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
    [Register ("ShowAttachmentView")]
    partial class ShowAttachmentView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_back { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint headerView_constantHeight { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_title { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIWebView webview_content { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BT_back != null) {
                BT_back.Dispose ();
                BT_back = null;
            }

            if (headerView_constantHeight != null) {
                headerView_constantHeight.Dispose ();
                headerView_constantHeight = null;
            }

            if (lbl_title != null) {
                lbl_title.Dispose ();
                lbl_title = null;
            }

            if (webview_content != null) {
                webview_content.Dispose ();
                webview_content = null;
            }
        }
    }
}
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
    [Register ("AddAttachmentsView")]
    partial class AddAttachmentsView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_back { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint headerView_constantHeight { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView top_view { get; set; }

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

            if (top_view != null) {
                top_view.Dispose ();
                top_view = null;
            }
        }
    }
}
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
    [Register ("ProgressView")]
    partial class ProgressView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_close { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_code_fulltext { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint headerView_constantHeight { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_title { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView table_content { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BT_close != null) {
                BT_close.Dispose ();
                BT_close = null;
            }

            if (BT_code_fulltext != null) {
                BT_code_fulltext.Dispose ();
                BT_code_fulltext = null;
            }

            if (headerView_constantHeight != null) {
                headerView_constantHeight.Dispose ();
                headerView_constantHeight = null;
            }

            if (lbl_title != null) {
                lbl_title.Dispose ();
                lbl_title = null;
            }

            if (table_content != null) {
                table_content.Dispose ();
                table_content = null;
            }
        }
    }
}
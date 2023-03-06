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
    [Register ("CreateTicketFormView")]
    partial class CreateTicketFormView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView bottom_view { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_back { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_progress { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint contraint_heightViewNavBot { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint headerView_constantHeight { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView table_content { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView top_view { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_content { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (bottom_view != null) {
                bottom_view.Dispose ();
                bottom_view = null;
            }

            if (BT_back != null) {
                BT_back.Dispose ();
                BT_back = null;
            }

            if (BT_progress != null) {
                BT_progress.Dispose ();
                BT_progress = null;
            }

            if (contraint_heightViewNavBot != null) {
                contraint_heightViewNavBot.Dispose ();
                contraint_heightViewNavBot = null;
            }

            if (headerView_constantHeight != null) {
                headerView_constantHeight.Dispose ();
                headerView_constantHeight = null;
            }

            if (table_content != null) {
                table_content.Dispose ();
                table_content = null;
            }

            if (top_view != null) {
                top_view.Dispose ();
                top_view = null;
            }

            if (view_content != null) {
                view_content.Dispose ();
                view_content = null;
            }
        }
    }
}
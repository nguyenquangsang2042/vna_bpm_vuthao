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
    [Register ("ActionMoreApp")]
    partial class ActionMoreApp
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_cancel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint constraint_heightTable { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView table_moreAction { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BT_cancel != null) {
                BT_cancel.Dispose ();
                BT_cancel = null;
            }

            if (constraint_heightTable != null) {
                constraint_heightTable.Dispose ();
                constraint_heightTable = null;
            }

            if (table_moreAction != null) {
                table_moreAction.Dispose ();
                table_moreAction = null;
            }
        }
    }
}
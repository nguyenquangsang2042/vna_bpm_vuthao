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
    [Register ("BroadView")]
    partial class BroadView_copy
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_avatar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UICollectionView Collection_WorkflowCate { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_topBar_title { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_bot_bar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_lstWorkFlow { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_top { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_top_bar { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BT_avatar != null) {
                BT_avatar.Dispose ();
                BT_avatar = null;
            }

            if (Collection_WorkflowCate != null) {
                Collection_WorkflowCate.Dispose ();
                Collection_WorkflowCate = null;
            }

            if (lbl_topBar_title != null) {
                lbl_topBar_title.Dispose ();
                lbl_topBar_title = null;
            }

            if (view_bot_bar != null) {
                view_bot_bar.Dispose ();
                view_bot_bar = null;
            }

            if (view_lstWorkFlow != null) {
                view_lstWorkFlow.Dispose ();
                view_lstWorkFlow = null;
            }

            if (view_top != null) {
                view_top.Dispose ();
                view_top = null;
            }

            if (view_top_bar != null) {
                view_top_bar.Dispose ();
                view_top_bar = null;
            }
        }
    }
}
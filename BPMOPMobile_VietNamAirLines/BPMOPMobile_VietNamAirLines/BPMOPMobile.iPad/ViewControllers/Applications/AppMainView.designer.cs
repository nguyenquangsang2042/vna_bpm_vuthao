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

namespace BPMOPMobile.iPad.ViewControllers.Applications
{
    [Register ("AppMainView")]
    partial class AppMainView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_back { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_search { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_taskLeft_denToi { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_taskLeft_toiBatDau { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UICollectionView Collection_workflow { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_search { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView table_toDo { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView table_workflow { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_bot_bar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint view_BotBar_ConstantHeight { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_task_left { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_task_right { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_taskLeft_top { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_top { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_top_bar { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BT_back != null) {
                BT_back.Dispose ();
                BT_back = null;
            }

            if (BT_search != null) {
                BT_search.Dispose ();
                BT_search = null;
            }

            if (BT_taskLeft_denToi != null) {
                BT_taskLeft_denToi.Dispose ();
                BT_taskLeft_denToi = null;
            }

            if (BT_taskLeft_toiBatDau != null) {
                BT_taskLeft_toiBatDau.Dispose ();
                BT_taskLeft_toiBatDau = null;
            }

            if (Collection_workflow != null) {
                Collection_workflow.Dispose ();
                Collection_workflow = null;
            }

            if (lbl_search != null) {
                lbl_search.Dispose ();
                lbl_search = null;
            }

            if (table_toDo != null) {
                table_toDo.Dispose ();
                table_toDo = null;
            }

            if (table_workflow != null) {
                table_workflow.Dispose ();
                table_workflow = null;
            }

            if (view_bot_bar != null) {
                view_bot_bar.Dispose ();
                view_bot_bar = null;
            }

            if (view_BotBar_ConstantHeight != null) {
                view_BotBar_ConstantHeight.Dispose ();
                view_BotBar_ConstantHeight = null;
            }

            if (view_task_left != null) {
                view_task_left.Dispose ();
                view_task_left = null;
            }

            if (view_task_right != null) {
                view_task_right.Dispose ();
                view_task_right = null;
            }

            if (view_taskLeft_top != null) {
                view_taskLeft_top.Dispose ();
                view_taskLeft_top = null;
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
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
    [Register ("BroadView")]
    partial class BroadView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_avatar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_category { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_filter_favorite { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_search { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint constraint_heightSearch { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint constraint_topcatalogy { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint constraint_toptable { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint contraint_heightViewNavBot { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint headerView_constantHeight { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_categorySelected { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_dodata { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_title { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView table_content { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField tf_search { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView top_view { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_bot_bar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_filter_category { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_search { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BT_avatar != null) {
                BT_avatar.Dispose ();
                BT_avatar = null;
            }

            if (BT_category != null) {
                BT_category.Dispose ();
                BT_category = null;
            }

            if (BT_filter_favorite != null) {
                BT_filter_favorite.Dispose ();
                BT_filter_favorite = null;
            }

            if (BT_search != null) {
                BT_search.Dispose ();
                BT_search = null;
            }

            if (constraint_heightSearch != null) {
                constraint_heightSearch.Dispose ();
                constraint_heightSearch = null;
            }

            if (constraint_topcatalogy != null) {
                constraint_topcatalogy.Dispose ();
                constraint_topcatalogy = null;
            }

            if (constraint_toptable != null) {
                constraint_toptable.Dispose ();
                constraint_toptable = null;
            }

            if (contraint_heightViewNavBot != null) {
                contraint_heightViewNavBot.Dispose ();
                contraint_heightViewNavBot = null;
            }

            if (headerView_constantHeight != null) {
                headerView_constantHeight.Dispose ();
                headerView_constantHeight = null;
            }

            if (lbl_categorySelected != null) {
                lbl_categorySelected.Dispose ();
                lbl_categorySelected = null;
            }

            if (lbl_dodata != null) {
                lbl_dodata.Dispose ();
                lbl_dodata = null;
            }

            if (lbl_title != null) {
                lbl_title.Dispose ();
                lbl_title = null;
            }

            if (table_content != null) {
                table_content.Dispose ();
                table_content = null;
            }

            if (tf_search != null) {
                tf_search.Dispose ();
                tf_search = null;
            }

            if (top_view != null) {
                top_view.Dispose ();
                top_view = null;
            }

            if (view_bot_bar != null) {
                view_bot_bar.Dispose ();
                view_bot_bar = null;
            }

            if (view_filter_category != null) {
                view_filter_category.Dispose ();
                view_filter_category = null;
            }

            if (view_search != null) {
                view_search.Dispose ();
                view_search = null;
            }
        }
    }
}
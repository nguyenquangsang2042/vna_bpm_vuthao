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
    [Register ("FormAddWorkRelatedView")]
    partial class FormAddWorkRelatedView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_accept { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_close { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_fromdate { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_quytrinh { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_search { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_tinhtrang { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_todate { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIDatePicker datePicker { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint left_constraint_viewFilter { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView table_content { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField tf_fromdate { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField tf_keyword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField tf_quytrinh { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField tf_tinhtrang { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField tf_todate { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_filter { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BT_accept != null) {
                BT_accept.Dispose ();
                BT_accept = null;
            }

            if (BT_close != null) {
                BT_close.Dispose ();
                BT_close = null;
            }

            if (BT_fromdate != null) {
                BT_fromdate.Dispose ();
                BT_fromdate = null;
            }

            if (BT_quytrinh != null) {
                BT_quytrinh.Dispose ();
                BT_quytrinh = null;
            }

            if (BT_search != null) {
                BT_search.Dispose ();
                BT_search = null;
            }

            if (BT_tinhtrang != null) {
                BT_tinhtrang.Dispose ();
                BT_tinhtrang = null;
            }

            if (BT_todate != null) {
                BT_todate.Dispose ();
                BT_todate = null;
            }

            if (datePicker != null) {
                datePicker.Dispose ();
                datePicker = null;
            }

            if (left_constraint_viewFilter != null) {
                left_constraint_viewFilter.Dispose ();
                left_constraint_viewFilter = null;
            }

            if (table_content != null) {
                table_content.Dispose ();
                table_content = null;
            }

            if (tf_fromdate != null) {
                tf_fromdate.Dispose ();
                tf_fromdate = null;
            }

            if (tf_keyword != null) {
                tf_keyword.Dispose ();
                tf_keyword = null;
            }

            if (tf_quytrinh != null) {
                tf_quytrinh.Dispose ();
                tf_quytrinh = null;
            }

            if (tf_tinhtrang != null) {
                tf_tinhtrang.Dispose ();
                tf_tinhtrang = null;
            }

            if (tf_todate != null) {
                tf_todate.Dispose ();
                tf_todate = null;
            }

            if (view_filter != null) {
                view_filter.Dispose ();
                view_filter = null;
            }
        }
    }
}
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

namespace BPMOPMobile.iPad.ViewControllers
{
    [Register ("FormCommentView")]
    partial class FormCommentView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_addAttach { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_close { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_sendComment { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BT_submit { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint constraintY_viewnote { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView table_attachment { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView table_comments { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView txt_note { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_comment { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_content { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView view_note { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint view_noteConstraintHeight { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BT_addAttach != null) {
                BT_addAttach.Dispose ();
                BT_addAttach = null;
            }

            if (BT_close != null) {
                BT_close.Dispose ();
                BT_close = null;
            }

            if (BT_sendComment != null) {
                BT_sendComment.Dispose ();
                BT_sendComment = null;
            }

            if (BT_submit != null) {
                BT_submit.Dispose ();
                BT_submit = null;
            }

            if (constraintY_viewnote != null) {
                constraintY_viewnote.Dispose ();
                constraintY_viewnote = null;
            }

            if (table_attachment != null) {
                table_attachment.Dispose ();
                table_attachment = null;
            }

            if (table_comments != null) {
                table_comments.Dispose ();
                table_comments = null;
            }

            if (txt_note != null) {
                txt_note.Dispose ();
                txt_note = null;
            }

            if (view_comment != null) {
                view_comment.Dispose ();
                view_comment = null;
            }

            if (view_content != null) {
                view_content.Dispose ();
                view_content = null;
            }

            if (view_note != null) {
                view_note.Dispose ();
                view_note = null;
            }

            if (view_noteConstraintHeight != null) {
                view_noteConstraintHeight.Dispose ();
                view_noteConstraintHeight = null;
            }
        }
    }
}
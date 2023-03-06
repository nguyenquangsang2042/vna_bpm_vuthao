// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace BPMOPMobileV1.iOS.ViewControllers
{
	[Register ("FormWFDetailsProperty")]
	partial class FormWFDetailsProperty
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_accept { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_close { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_delete { get; set; }

		[Outlet]
		UIKit.UILabel lbl_title { get; set; }

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
			if (BT_accept != null) {
				BT_accept.Dispose ();
				BT_accept = null;
			}

			if (BT_close != null) {
				BT_close.Dispose ();
				BT_close = null;
			}

			if (BT_delete != null) {
				BT_delete.Dispose ();
				BT_delete = null;
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

			if (lbl_title != null) {
				lbl_title.Dispose ();
				lbl_title = null;
			}
		}
	}
}

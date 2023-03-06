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
	[Register ("MultiChoiceView")]
	partial class MultiChoiceView
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_back { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_clear { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_done { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.NSLayoutConstraint Constraint_rightBTClear { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint headerView_constantHeight { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel lbl_title { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UITableView table_content { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BT_back != null) {
				BT_back.Dispose ();
				BT_back = null;
			}

			if (BT_clear != null) {
				BT_clear.Dispose ();
				BT_clear = null;
			}

			if (BT_done != null) {
				BT_done.Dispose ();
				BT_done = null;
			}

			if (Constraint_rightBTClear != null) {
				Constraint_rightBTClear.Dispose ();
				Constraint_rightBTClear = null;
			}

			if (lbl_title != null) {
				lbl_title.Dispose ();
				lbl_title = null;
			}

			if (table_content != null) {
				table_content.Dispose ();
				table_content = null;
			}

			if (headerView_constantHeight != null) {
				headerView_constantHeight.Dispose ();
				headerView_constantHeight = null;
			}
		}
	}
}

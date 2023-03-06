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
	[Register ("NumberControlView")]
	partial class NumberControlView
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_back { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_done { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint headerView_constantHeight { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel lbl_title { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UITextField textfield_num { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BT_back != null) {
				BT_back.Dispose ();
				BT_back = null;
			}

			if (BT_done != null) {
				BT_done.Dispose ();
				BT_done = null;
			}

			if (lbl_title != null) {
				lbl_title.Dispose ();
				lbl_title = null;
			}

			if (textfield_num != null) {
				textfield_num.Dispose ();
				textfield_num = null;
			}

			if (headerView_constantHeight != null) {
				headerView_constantHeight.Dispose ();
				headerView_constantHeight = null;
			}
		}
	}
}

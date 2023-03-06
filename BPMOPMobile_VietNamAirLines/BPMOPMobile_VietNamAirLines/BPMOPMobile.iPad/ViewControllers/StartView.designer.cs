// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace BPMOPMobile.iPad
{
	[Register ("StartView")]
	partial class StartView
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_login { get; set; }

		[Outlet]
		UIKit.UIButton BT_register { get; set; }

		[Outlet]
		UIKit.UILabel lbl_loading { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIActivityIndicatorView loading_indicator { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView view_loading { get; set; }

		[Outlet]
		UIKit.UIView view_login { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BT_login != null) {
				BT_login.Dispose ();
				BT_login = null;
			}

			if (BT_register != null) {
				BT_register.Dispose ();
				BT_register = null;
			}

			if (loading_indicator != null) {
				loading_indicator.Dispose ();
				loading_indicator = null;
			}

			if (view_loading != null) {
				view_loading.Dispose ();
				view_loading = null;
			}

			if (view_login != null) {
				view_login.Dispose ();
				view_login = null;
			}

			if (lbl_loading != null) {
				lbl_loading.Dispose ();
				lbl_loading = null;
			}
		}
	}
}

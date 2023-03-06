// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace BPMOPMobile.iPad.ViewControllers
{
	[Register ("RegisterViewController")]
	partial class RegisterViewController
	{
		[Outlet]
		UIKit.UIButton BT_back { get; set; }

		[Outlet]
		UIKit.UIButton BT_login { get; set; }

		[Outlet]
		UIKit.UIButton BT_register { get; set; }

		[Outlet]
		UIKit.UITextField txt_userName { get; set; }

		[Outlet]
		UIKit.UIView view_loading { get; set; }

		[Outlet]
		UIKit.UIView view_register { get; set; }

		[Outlet]
		UIKit.UIView view_userEmail { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (view_userEmail != null) {
				view_userEmail.Dispose ();
				view_userEmail = null;
			}

			if (BT_back != null) {
				BT_back.Dispose ();
				BT_back = null;
			}

			if (BT_login != null) {
				BT_login.Dispose ();
				BT_login = null;
			}

			if (BT_register != null) {
				BT_register.Dispose ();
				BT_register = null;
			}

			if (txt_userName != null) {
				txt_userName.Dispose ();
				txt_userName = null;
			}

			if (view_loading != null) {
				view_loading.Dispose ();
				view_loading = null;
			}

			if (view_register != null) {
				view_register.Dispose ();
				view_register = null;
			}
		}
	}
}

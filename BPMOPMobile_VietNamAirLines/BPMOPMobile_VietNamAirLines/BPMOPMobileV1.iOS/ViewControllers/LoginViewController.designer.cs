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
	[Register ("LoginViewController")]
	partial class LoginViewController
	{
		[Outlet]
		UIKit.UIButton BT_back { get; set; }

		[Outlet]
		UIKit.UIButton BT_login { get; set; }

		[Outlet]
		UIKit.UIButton BT_register { get; set; }

		[Outlet]
		UIKit.UILabel lbl_loading { get; set; }

		[Outlet]
		UIKit.UITextField txt_loginName { get; set; }

		[Outlet]
		UIKit.UITextField txt_password { get; set; }

		[Outlet]
		UIKit.UIView view_loading { get; set; }

		[Outlet]
		UIKit.UIView view_login { get; set; }

		[Outlet]
		UIKit.UIView view_password { get; set; }

		[Outlet]
		UIKit.UIView view_userAcc { get; set; }

		[Outlet]
		UIKit.UIView view_userNamePassword { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (lbl_loading != null) {
				lbl_loading.Dispose ();
				lbl_loading = null;
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

			if (txt_loginName != null) {
				txt_loginName.Dispose ();
				txt_loginName = null;
			}

			if (txt_password != null) {
				txt_password.Dispose ();
				txt_password = null;
			}

			if (view_loading != null) {
				view_loading.Dispose ();
				view_loading = null;
			}

			if (view_login != null) {
				view_login.Dispose ();
				view_login = null;
			}

			if (view_password != null) {
				view_password.Dispose ();
				view_password = null;
			}

			if (view_userAcc != null) {
				view_userAcc.Dispose ();
				view_userAcc = null;
			}

			if (view_userNamePassword != null) {
				view_userNamePassword.Dispose ();
				view_userNamePassword = null;
			}
		}
	}
}

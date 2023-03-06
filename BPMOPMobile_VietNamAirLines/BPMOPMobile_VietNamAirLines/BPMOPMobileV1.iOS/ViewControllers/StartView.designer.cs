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
	[Register ("StartView")]
	partial class StartView
	{
		[Outlet]
		UIKit.UIButton BT_fanpage { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_login { get; set; }

		[Outlet]
		UIKit.UIButton BT_phone { get; set; }

		[Outlet]
		UIKit.UIButton BT_register { get; set; }

		[Outlet]
		UIKit.UIButton BT_webpage { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.NSLayoutConstraint constraint_topLogo { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel lbl_loading { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView loading_view { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UITextField txt_LoginName { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UITextField txt_Password { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView view_loginForm { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView view_user_pass { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BT_phone != null) {
				BT_phone.Dispose ();
				BT_phone = null;
			}

			if (BT_fanpage != null) {
				BT_fanpage.Dispose ();
				BT_fanpage = null;
			}

			if (BT_login != null) {
				BT_login.Dispose ();
				BT_login = null;
			}

			if (BT_register != null) {
				BT_register.Dispose ();
				BT_register = null;
			}

			if (BT_webpage != null) {
				BT_webpage.Dispose ();
				BT_webpage = null;
			}

			if (constraint_topLogo != null) {
				constraint_topLogo.Dispose ();
				constraint_topLogo = null;
			}

			if (lbl_loading != null) {
				lbl_loading.Dispose ();
				lbl_loading = null;
			}

			if (loading_view != null) {
				loading_view.Dispose ();
				loading_view = null;
			}

			if (txt_LoginName != null) {
				txt_LoginName.Dispose ();
				txt_LoginName = null;
			}

			if (txt_Password != null) {
				txt_Password.Dispose ();
				txt_Password = null;
			}

			if (view_loginForm != null) {
				view_loginForm.Dispose ();
				view_loginForm = null;
			}

			if (view_user_pass != null) {
				view_user_pass.Dispose ();
				view_user_pass = null;
			}
		}
	}
}

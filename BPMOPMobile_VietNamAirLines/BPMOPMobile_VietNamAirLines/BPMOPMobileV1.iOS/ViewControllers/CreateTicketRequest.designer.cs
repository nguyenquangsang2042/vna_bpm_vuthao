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
	[Register ("CreateTicketRequest")]
	partial class CreateTicketRequest
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_back { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_filter { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UICollectionView Collection_ticket { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView header_view { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint headerView_constantHeight { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BT_back != null) {
				BT_back.Dispose ();
				BT_back = null;
			}

			if (BT_filter != null) {
				BT_filter.Dispose ();
				BT_filter = null;
			}

			if (Collection_ticket != null) {
				Collection_ticket.Dispose ();
				Collection_ticket = null;
			}

			if (header_view != null) {
				header_view.Dispose ();
				header_view = null;
			}

			if (headerView_constantHeight != null) {
				headerView_constantHeight.Dispose ();
				headerView_constantHeight = null;
			}
		}
	}
}

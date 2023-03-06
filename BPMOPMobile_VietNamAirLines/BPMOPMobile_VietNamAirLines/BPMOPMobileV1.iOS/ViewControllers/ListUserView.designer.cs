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
	[Register ("ListUserView")]
	partial class ListUserView
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_accept { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_clear { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_close { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.NSLayoutConstraint collectionUser_heightConstant { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.NSLayoutConstraint Constraint_rightBTClear { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint headerView_constantHeight { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel lbl_line { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel lbl_title { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.NSLayoutConstraint selectedUserView_heightConstant { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UITableView table_content { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UITextField tf_search { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UICollectionView User_CollectionView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView view_searchUser { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BT_accept != null) {
				BT_accept.Dispose ();
				BT_accept = null;
			}

			if (BT_clear != null) {
				BT_clear.Dispose ();
				BT_clear = null;
			}

			if (BT_close != null) {
				BT_close.Dispose ();
				BT_close = null;
			}

			if (collectionUser_heightConstant != null) {
				collectionUser_heightConstant.Dispose ();
				collectionUser_heightConstant = null;
			}

			if (Constraint_rightBTClear != null) {
				Constraint_rightBTClear.Dispose ();
				Constraint_rightBTClear = null;
			}

			if (lbl_line != null) {
				lbl_line.Dispose ();
				lbl_line = null;
			}

			if (lbl_title != null) {
				lbl_title.Dispose ();
				lbl_title = null;
			}

			if (selectedUserView_heightConstant != null) {
				selectedUserView_heightConstant.Dispose ();
				selectedUserView_heightConstant = null;
			}

			if (table_content != null) {
				table_content.Dispose ();
				table_content = null;
			}

			if (tf_search != null) {
				tf_search.Dispose ();
				tf_search = null;
			}

			if (User_CollectionView != null) {
				User_CollectionView.Dispose ();
				User_CollectionView = null;
			}

			if (view_searchUser != null) {
				view_searchUser.Dispose ();
				view_searchUser = null;
			}

			if (headerView_constantHeight != null) {
				headerView_constantHeight.Dispose ();
				headerView_constantHeight = null;
			}
		}
	}
}

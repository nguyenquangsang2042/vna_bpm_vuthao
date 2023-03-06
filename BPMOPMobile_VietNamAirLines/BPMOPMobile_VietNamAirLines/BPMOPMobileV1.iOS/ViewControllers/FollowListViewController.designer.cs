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
	[Register ("FollowListViewController")]
	partial class FollowListViewController
	{
		[Outlet]
		UIKit.UIView bottom_view { get; set; }

		[Outlet]
		UIKit.UIButton BT_avatar { get; set; }

		[Outlet]
		UIKit.UIButton BT_fromMe { get; set; }

		[Outlet]
		UIKit.UIButton BT_search { get; set; }

		[Outlet]
		UIKit.UIButton BT_todo { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint constraint_heightSearch { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint contraint_heightViewNavBot { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint headerView_constantHeight { get; set; }

		[Outlet]
		UIKit.UIActivityIndicatorView indicator_loadmore { get; set; }

		[Outlet]
		UIKit.UILabel lbl_nodata { get; set; }

		[Outlet]
		UIKit.UILabel lbl_title { get; set; }

		[Outlet]
		UIKit.UISearchBar searchBar { get; set; }

		[Outlet]
		UIKit.UITableView table_content { get; set; }

		[Outlet]
		UIKit.UIView top_view { get; set; }

		[Outlet]
		UIKit.UIView view_loadmore { get; set; }

		[Outlet]
		UIKit.UIView view_search { get; set; }

		[Outlet]
		UIKit.UIView view_segment { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (bottom_view != null) {
				bottom_view.Dispose ();
				bottom_view = null;
			}

			if (BT_search != null) {
				BT_search.Dispose ();
				BT_search = null;
			}

			if (lbl_nodata != null) {
				lbl_nodata.Dispose ();
				lbl_nodata = null;
			}

			if (lbl_title != null) {
				lbl_title.Dispose ();
				lbl_title = null;
			}

			if (table_content != null) {
				table_content.Dispose ();
				table_content = null;
			}

			if (top_view != null) {
				top_view.Dispose ();
				top_view = null;
			}

			if (BT_avatar != null) {
				BT_avatar.Dispose ();
				BT_avatar = null;
			}

			if (headerView_constantHeight != null) {
				headerView_constantHeight.Dispose ();
				headerView_constantHeight = null;
			}

			if (view_loadmore != null) {
				view_loadmore.Dispose ();
				view_loadmore = null;
			}

			if (indicator_loadmore != null) {
				indicator_loadmore.Dispose ();
				indicator_loadmore = null;
			}

			if (constraint_heightSearch != null) {
				constraint_heightSearch.Dispose ();
				constraint_heightSearch = null;
			}

			if (BT_todo != null) {
				BT_todo.Dispose ();
				BT_todo = null;
			}

			if (BT_fromMe != null) {
				BT_fromMe.Dispose ();
				BT_fromMe = null;
			}

			if (contraint_heightViewNavBot != null) {
				contraint_heightViewNavBot.Dispose ();
				contraint_heightViewNavBot = null;
			}

			if (view_segment != null) {
				view_segment.Dispose ();
				view_segment = null;
			}

			if (view_search != null) {
				view_search.Dispose ();
				view_search = null;
			}

			if (searchBar != null) {
				searchBar.Dispose ();
				searchBar = null;
			}
		}
	}
}

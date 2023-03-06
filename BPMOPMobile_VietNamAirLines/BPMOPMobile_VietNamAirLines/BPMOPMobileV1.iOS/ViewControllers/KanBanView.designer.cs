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
	[Register ("KanBanView")]
	partial class KanBanView
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView bottom_view { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_all { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_apply { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_back { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_completed { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_date_0 { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_date_1 { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_date_2 { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_date_3 { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_filter_fromdate { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_filter_todate { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_filterDate { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_InProgress { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_kanBanNext { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_kanBanPrevious { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_reject { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_reset_filter { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton BT_search { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.NSLayoutConstraint constraint_heightSearch { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.NSLayoutConstraint contraint_heightViewNavBot { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint headerView_constantHeight { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UICollectionView KanbanCollection { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UITextField lbl_fromdate { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel lbl_titleWorkFlow { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UITextField lbl_todate { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UITextField tf_search_title { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView top_view { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView view_choose_kanban { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView view_content { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView view_filter { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView view_filterdate { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView view_fromdate { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView view_search { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView view_todate { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (bottom_view != null) {
				bottom_view.Dispose ();
				bottom_view = null;
			}

			if (BT_all != null) {
				BT_all.Dispose ();
				BT_all = null;
			}

			if (BT_apply != null) {
				BT_apply.Dispose ();
				BT_apply = null;
			}

			if (BT_back != null) {
				BT_back.Dispose ();
				BT_back = null;
			}

			if (BT_completed != null) {
				BT_completed.Dispose ();
				BT_completed = null;
			}

			if (BT_date_0 != null) {
				BT_date_0.Dispose ();
				BT_date_0 = null;
			}

			if (BT_date_1 != null) {
				BT_date_1.Dispose ();
				BT_date_1 = null;
			}

			if (BT_date_2 != null) {
				BT_date_2.Dispose ();
				BT_date_2 = null;
			}

			if (BT_date_3 != null) {
				BT_date_3.Dispose ();
				BT_date_3 = null;
			}

			if (BT_filter_fromdate != null) {
				BT_filter_fromdate.Dispose ();
				BT_filter_fromdate = null;
			}

			if (BT_filter_todate != null) {
				BT_filter_todate.Dispose ();
				BT_filter_todate = null;
			}

			if (BT_filterDate != null) {
				BT_filterDate.Dispose ();
				BT_filterDate = null;
			}

			if (BT_InProgress != null) {
				BT_InProgress.Dispose ();
				BT_InProgress = null;
			}

			if (BT_kanBanNext != null) {
				BT_kanBanNext.Dispose ();
				BT_kanBanNext = null;
			}

			if (BT_kanBanPrevious != null) {
				BT_kanBanPrevious.Dispose ();
				BT_kanBanPrevious = null;
			}

			if (BT_reject != null) {
				BT_reject.Dispose ();
				BT_reject = null;
			}

			if (BT_reset_filter != null) {
				BT_reset_filter.Dispose ();
				BT_reset_filter = null;
			}

			if (BT_search != null) {
				BT_search.Dispose ();
				BT_search = null;
			}

			if (constraint_heightSearch != null) {
				constraint_heightSearch.Dispose ();
				constraint_heightSearch = null;
			}

			if (contraint_heightViewNavBot != null) {
				contraint_heightViewNavBot.Dispose ();
				contraint_heightViewNavBot = null;
			}

			if (KanbanCollection != null) {
				KanbanCollection.Dispose ();
				KanbanCollection = null;
			}

			if (lbl_fromdate != null) {
				lbl_fromdate.Dispose ();
				lbl_fromdate = null;
			}

			if (lbl_titleWorkFlow != null) {
				lbl_titleWorkFlow.Dispose ();
				lbl_titleWorkFlow = null;
			}

			if (lbl_todate != null) {
				lbl_todate.Dispose ();
				lbl_todate = null;
			}

			if (tf_search_title != null) {
				tf_search_title.Dispose ();
				tf_search_title = null;
			}

			if (top_view != null) {
				top_view.Dispose ();
				top_view = null;
			}

			if (view_choose_kanban != null) {
				view_choose_kanban.Dispose ();
				view_choose_kanban = null;
			}

			if (view_content != null) {
				view_content.Dispose ();
				view_content = null;
			}

			if (view_filter != null) {
				view_filter.Dispose ();
				view_filter = null;
			}

			if (view_filterdate != null) {
				view_filterdate.Dispose ();
				view_filterdate = null;
			}

			if (view_fromdate != null) {
				view_fromdate.Dispose ();
				view_fromdate = null;
			}

			if (view_search != null) {
				view_search.Dispose ();
				view_search = null;
			}

			if (view_todate != null) {
				view_todate.Dispose ();
				view_todate = null;
			}

			if (headerView_constantHeight != null) {
				headerView_constantHeight.Dispose ();
				headerView_constantHeight = null;
			}
		}
	}
}

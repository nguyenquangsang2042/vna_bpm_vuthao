using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;

namespace BPMOPMobile.Droid.Class
{
    public class MinionAction
    {
        public static event EventHandler<ChangeViewListWorkflow> ChangeView;
        public class ChangeViewListWorkflow : EventArgs
        {
            public int IsSuccess { get; set; }
            public ChangeViewListWorkflow(int isSuccess)
            {
                IsSuccess = isSuccess;
            }
        }
        public static void ViewListWorkflow(object sender, ChangeViewListWorkflow v)
        {
            if (ChangeView != null)
            {
                ChangeView(sender, v);
            }
        }

        //------------------------------------------------------------------------------------------------------------------

        public static event EventHandler<ChangeViewEventArgs> DetailNew;
        public class ChangeViewEventArgs : EventArgs
        {
            public BeanWorkflowItem IsSuccess { get; set; }
            public ChangeViewEventArgs(BeanWorkflowItem isSuccess)
            {
                IsSuccess = isSuccess;
            }
        }

        public static event EventHandler RefreshFragmentViewPager;
        public static void OnRefreshFragmentViewPager(object sender, EventArgs e)
        {
            if (RefreshFragmentViewPager != null)
            {
                RefreshFragmentViewPager(sender, e);
            }
        }

        //------------------------------------------------------------------------------------------------------------------

        public static event EventHandler RefreshFragmentHomePage;
        public static void OnRefreshFragmentHomePage(object sender, EventArgs e)
        {
            if (RefreshFragmentHomePage != null)
            {
                RefreshFragmentHomePage(sender, e);
            }
        }
        //------------------------------------------------------------------------------------------------------------------

        public static event EventHandler RefreshFragmentParent;
        public static void OnRefreshFragmentParent(object sender, EventArgs e)
        {
            if (RefreshFragmentParent != null)
            {
                RefreshFragmentParent(sender, e);
            }
        }
        //------------------------------------------------------------------------------------------------------------------

        public static event EventHandler MoveToHeadHomePage;
        public static void OnMoveToHeadHomePage(object sender, EventArgs e)
        {
            if (MoveToHeadHomePage != null)
            {
                MoveToHeadHomePage(sender, e);
            }
        }
        //------------------------------------------------------------------------------------------------------------------

        public static event EventHandler RedirectFragmentLeftMenu; // Renew data + perform Click
        public static void OnRedirectFragmentLeftMenu(object sender, EventArgs e)
        {
            if (RedirectFragmentLeftMenu != null)
            {
                RedirectFragmentLeftMenu(sender, e);
            }
        }

        //------------------------------------------------------------------------------------------------------------------

        public static event EventHandler RenewDataAndShowFragmentLeftMenu; // Chỉ Renew data chứ không perform Click
        public static void OnRenewDataAndShowFragmentLeftMenu(object sender, EventArgs e) 
        { 
            if (RenewDataAndShowFragmentLeftMenu != null)
                RenewDataAndShowFragmentLeftMenu(sender, e);
        }

        //------------------------------------------------------------------------------------------------------------------

        public static event EventHandler RenewFragmentSingleVDT; // Renew trang VDT
        public static void OnRenewFragmentSingleVDT(object sender, EventArgs e)
        {
            if (RenewFragmentSingleVDT != null)
                RenewFragmentSingleVDT(sender, e);
        }
        //------------------------------------------------------------------------------------------------------------------

        public static event EventHandler RenewFragmentSingleFollow; // Renew trang VDT
        public static void OnRenewFragmentSingleFollow(object sender, EventArgs e)
        {
            if (RenewFragmentSingleFollow != null)
                RenewFragmentSingleFollow(sender, e);
        }

        //------------------------------------------------------------------------------------------------------------------

        public static event EventHandler RenewFragmentSingleVTBD; // Renew trang VTBD
        public static void OnRenewFragmentSingleVTBD(object sender, EventArgs e)
        {
            if (RenewFragmentSingleVTBD != null)
                RenewFragmentSingleVTBD(sender, e);
        }
        //------------------------------------------------------------------------------------------------------------------

        public static event EventHandler RenewFragmentBoardDetailGroup; // Renew trang RenewFragmentBoardDetailGroup
        public static void OnRenewFragmentBoardDetailGroup(object sender, EventArgs e)
        {
            if (RenewFragmentBoardDetailGroup != null)
                RenewFragmentBoardDetailGroup(sender, e);
        }

        //------------------------------------------------------------------------------------------------------------------

        public static void ReadDetaiNew(object sender, ChangeViewEventArgs v)
        {
            if (DetailNew != null)
            {
                DetailNew(sender, v);
            }
        }

        //------------------------------------------------------------------------------------------------------------------

        public static event EventHandler<RenewListDragDrop> RenewListDragDropEvent;
        public class RenewListDragDrop : EventArgs
        {
            public int _RecyPostionStart { get; set; }
            public int _RecyPostionEnd { get; set; }
            public BeanWorkflowItem _itemUpdate { get; set; }
            public RenewListDragDrop(int _RecyPostionStart, int _RecyPostionEnd, BeanWorkflowItem _itemUpdate)
            {
                this._RecyPostionStart = _RecyPostionStart;
                this._RecyPostionEnd = _RecyPostionEnd;
                this._itemUpdate = _itemUpdate;
            }
        }
        public static void OnRenewListDragDrop(object sender, RenewListDragDrop v)
        {
            if (RenewListDragDropEvent != null)
            {
                RenewListDragDropEvent(sender, v);
            }
        }

        //------------------------------------------------------------------------------------------------------------------

        // Event này call khi Click vào trang DetailWorkflow -> bấm Follow rồi Back ra -> renew item đã Click đó

        public static event EventHandler<RenewItem_AfterFollow> RenewItem_AfterFollowEvent;
        public class RenewItem_AfterFollow : EventArgs
        {
            public string _workflowItemID { get; set; }
            public bool _IsFollow { get; set; }
            public RenewItem_AfterFollow(string _workflowItemID, bool _IsFollow)
            {
                this._workflowItemID = _workflowItemID;
                this._IsFollow = _IsFollow;
            }
        }
        public static void OnRenewItem_AfterFollow(object sender, RenewItem_AfterFollow v)
        {
            if (RenewItem_AfterFollowEvent != null)
            {
                RenewItem_AfterFollowEvent(sender, v);
            }
        }

    }
}
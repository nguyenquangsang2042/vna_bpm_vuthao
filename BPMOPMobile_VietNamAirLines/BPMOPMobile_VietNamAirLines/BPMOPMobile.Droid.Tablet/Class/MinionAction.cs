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

namespace BPMOPMobile.Droid.Tablet.Class
{
    public class MinionAction
    {
        public static int _FlagNavigation = 1; // 1 - Home, 2 - VDT, 3- VTBD
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

        public static event EventHandler RefreshFragmentHomePage;
        public static void OnRefreshFragmentHomePage(object sender, EventArgs e)
        {
            if (RefreshFragmentHomePage != null)
            {
                RefreshFragmentHomePage(sender, e);
            }
        }
        public static event EventHandler RefreshFragmentLeftMenu;
        public static void OnRefreshFragmentLeftMenu(object sender, EventArgs e)
        {
            if (RefreshFragmentLeftMenu != null)
            {
                RefreshFragmentLeftMenu(sender, e);
            }
        }

        public static void ReadDetaiNew(object sender, ChangeViewEventArgs v)
        {
            if (DetailNew != null)
            {
                DetailNew(sender, v);
            }
        }


    }
}
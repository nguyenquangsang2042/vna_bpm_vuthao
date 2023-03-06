using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using Android.Content.PM;
using BPMOPMobile.Class;
using System.IO;
using System;
using BPMOPMobile.Bean;
using BPMOPMobile.DataProvider;
using Android.Views.InputMethods;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Support.V4.Widget;
using BPMOPMobile.Droid.Tablet.Presenter.Fragment;
using Android.Content;
using BPMOPMobile.Droid.Core.Common;
using static BPMOPMobile.Droid.Tablet.Class.MyFirebaseIidService;
using System.Threading.Tasks;
using BPMOPMobile.Droid.Tablet.Class;
using System.Threading;

namespace BPMOPMobile.Droid.Tablet
{
    [System.Obsolete]
    [Activity(Label = "BPMOP Tablet", MainLauncher = true, Icon = "@drawable/icon_logosmall", Theme = "@style/Theme.AppCompat.Light.NoActionBar.FullScreen",
        WindowSoftInputMode = SoftInput.AdjustPan, LaunchMode = LaunchMode.SingleTop, Exported = true, ScreenOrientation = ScreenOrientation.Landscape)]
    public class MainActivity : AppCompatActivity
    {
        
        private InputMethodManager _inputMethodManager;
        private FragmentStartView _startView;
        private NavigationView _lsttest;
        private DisplayMetrics _metrics;
        private FragmentLeftMenu _objLeftMenuFragment;
        public DrawerLayout MDrawerLayout;
        private Android.Support.V4.App.ActionBarDrawerToggle _mDrawerToggle;
        public static bool _openDetailPage = true; // true la` cho mo
        private long _lastClickTime = 0;
        public static string IdNotify = "";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ActivityMain);
            _inputMethodManager = (InputMethodManager)GetSystemService(Context.InputMethodService);
            MDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawerlayout_ActivityMain_Content).JavaCast<DrawerLayout>();
            MainActivity _mainAct = new MainActivity();
            if (this.Intent != null)
            {
                _mainAct.OnNewIntent(this.Intent);
            }
            if (_startView == null)
            {
                _startView = new FragmentStartView();
                ShowFragment(FragmentManager, _startView, "FragmentStartView", 1);
            }
        }
        public override void OnBackPressed()
        {
            try
            {
                if (SystemClock.ElapsedRealtime() - _lastClickTime < 1000)
                {
                    return;
                }
                _lastClickTime = SystemClock.ElapsedRealtime();
                string tam = GetCurrentFragmentTag();
                if (tam == "FragmentStartView" || tam == "FragmentHomePage")
                {
                    return;
                }
                else
                {
                    base.OnBackPressed();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - MainActivity - OnBackPressed - Error: " + ex.Message);
#endif
            }
        }

        public string GetCurrentFragmentTag()
        {
            string fragmentTag = FragmentManager.GetBackStackEntryAt(FragmentManager.BackStackEntryCount - 1).Name;
            return fragmentTag;
        }
        public void ShowFragment(FragmentManager fm, Fragment fragToShow, string fragTag, int type = 0)
        {
            string previousFragTag = "";
            Fragment previousFrag = null;
            try
            {
                InitMenuLeft(fragTag);
                if (fm.BackStackEntryCount > 0)
                {
                    previousFragTag = fm.GetBackStackEntryAt(fm.BackStackEntryCount - 1).Name;
                    previousFrag = fm.FindFragmentByTag(previousFragTag);
                    if (previousFragTag != null && previousFragTag != "FragmentLeftMenu")
                    {
                        CmmDroidFunction.RemoverRootViewMain(previousFrag.View);
                    }
                }
                if (fragTag != previousFragTag)
                {
                    FragmentTransaction fragmentTx;
                    fragmentTx = fm.BeginTransaction();
                    if (type == 0)
                    {
                        fragmentTx.SetCustomAnimations(Resource.Animation.fragment_enter, Resource.Animation.fragment_exit, Resource.Animation.fragment_pop_enter, Resource.Animation.fragment_pop_exit);
                    }
                    else
                    {
                        fragmentTx.SetCustomAnimations(Resource.Animation.fade_in, Resource.Animation.fade_out);
                    }
                    fragmentTx.Replace(Resource.Id.frame_ActivityMain_content, fragToShow, fragTag);
                    fragmentTx.AddToBackStack(fragTag);
                    fragmentTx.SetTransition(FragmentTransit.FragmentFade);
                    fragmentTx.Commit();
                    //fm.ExecutePendingTransactions();
                    FragmentManager.ExecutePendingTransactions();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - MainActivity - ShowFragment - Error: " + ex.Message);
#endif
            }
        }
        public void AddFragment(FragmentManager fm, Fragment fragToShow, string fragTag, int type = 0)
        {
            string previousFragTag = "";
            Fragment previousFrag = null;
            try
            {
                //InitMenuLeft(fragTag);
                //if (fm.BackStackEntryCount > 0)
                //{
                //    previousFragTag = fm.GetBackStackEntryAt(fm.BackStackEntryCount - 1).Name;
                //    previousFrag = fm.FindFragmentByTag(previousFragTag);
                //    if (previousFragTag != null && previousFragTag != "FragmentLeftMenu")
                //    {
                //        CmmDroidFunction.RemoverRootViewMain(previousFrag.View);
                //    }
                //}

                FragmentTransaction fragmentTx;
                fragmentTx = fm.BeginTransaction();
                if (type == 0)
                {
                    fragmentTx.SetCustomAnimations(Resource.Animation.fragment_enter, Resource.Animation.fragment_exit, Resource.Animation.fragment_pop_enter, Resource.Animation.fragment_pop_exit);
                }
                else
                {
                    fragmentTx.SetCustomAnimations(Resource.Animation.fade_in, Resource.Animation.fade_out);
                }
                fragmentTx.Add(Resource.Id.frame_ActivityMain_content, fragToShow, fragTag);
                fragmentTx.AddToBackStack(fragTag);
                fragmentTx.SetTransition(FragmentTransit.FragmentFade);
                fragmentTx.Commit();
                fm.ExecutePendingTransactions();
                //FragmentManager.ExecutePendingTransactions();

            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - MainActivity - ShowFragment - Error: " + ex.Message);
#endif
            }
        }

        public void RemoveFragmentInStack(FragmentManager fm, string fragTag)
        {
            try
            {

                Android.App.Fragment fragmentToRemove = fm.FindFragmentByTag(fragTag);
                if (fragmentToRemove != null)
                {
                    FragmentTransaction _transaction = fm.BeginTransaction();
                    _transaction.Remove(fragmentToRemove);
                    //_transaction.Replace(Resource.Id.frame_ActivityMain_content, fragmentToRemove, fragTag);
                    //_transaction.Commit();
                    //fm.PopBackStack();
                    //fm.ExecutePendingTransactions();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - MainActivity - RemoveFragmentInStack - Error: " + ex.Message);
#endif
            }
        }
        public void RemoveAllFragmentInStack(FragmentManager fm)
        {
            try
            {
                for (int i = 0; i < fm.BackStackEntryCount; i++)
                {
                    string fragTag = fm.GetBackStackEntryAt(i).Name;
                    if (fragTag.Equals("FragmentLeftMenu"))
                    {

                    }
                    else
                    {
                        //Fragment fragmentToRemove = fm.FindFragmentByTag(fragTag);
                        //FragmentTransaction _transaction = fm.BeginTransaction();
                        //_transaction.Remove(fragmentToRemove);
                        FragmentManager.PopBackStack(null, FragmentManager.PopBackStackInclusive);
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - MainActivity - RemoveAllFragmentInStack - Error: " + ex.Message);
#endif
            }
        }

        public void ReLoadCurrentFragment(FragmentManager fm, string fragTag)
        {
            try
            {
                Fragment frg = null;
                frg = FragmentManager.FindFragmentByTag(fragTag);
                if (frg != null)
                {
                    FragmentTransaction fragmentTx = FragmentManager.BeginTransaction();
                    fragmentTx.Detach(frg);
                    fragmentTx.Attach(frg);
                    fragmentTx.Commit();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - MainActivity - ShowFragment - Error: " + ex.Message);
#endif
            }
        }
        public void HideFragment(string name = null)
        {
            try
            {
                _inputMethodManager.HideSoftInputFromWindow(this.CurrentFocus?.WindowToken, HideSoftInputFlags.None);
                var tam = GetCurrentFragmentTag();
                if (tam != null && tam != "FragmentLeftMenu")
                {
                    var previousFrag1 = FragmentManager.FindFragmentByTag(tam);
                    CmmDroidFunction.RemoverRootViewMain(previousFrag1.View);
                }
                if (!string.IsNullOrEmpty(name))
                {
                    InitMenuLeft(name);
                    Fragment previousFrag = FragmentManager.FindFragmentByTag(name);
                    ShowFragment(FragmentManager, previousFrag, name, 1);
                }
                else
                {
                    if (!string.IsNullOrEmpty(tam))
                    {
                        InitMenuLeft(tam);
                    }
                    FragmentManager.PopBackStack();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - MainActivity - HideFragment - Error: " + ex.Message);
#endif
            }
        }

        public void OpenDawer()
        {
            if (MDrawerLayout != null)
            {
                MDrawerLayout.OpenDrawer(_lsttest);
            }
            else
            {
                InitMenuLeft("FragmentHomePage");
                MDrawerLayout.OpenDrawer(_lsttest);
            }
        }
        public void CloseDawer()
        {
            if (MDrawerLayout != null && MDrawerLayout.IsDrawerOpen(_lsttest))
            {
                MDrawerLayout.CloseDrawer(_lsttest);
            }
        }
        public void InitMenuLeft(string fragTag)
        {
            try
            {
                //if (_objLeftMenuFragment == null)
                //{
                if (fragTag == "FragmentHomePage")
                {
                    if (MDrawerLayout == null)
                    {
                        MDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawerlayout_ActivityMain_Content).JavaCast<DrawerLayout>();
                    }
                    MDrawerLayout.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
                    _lsttest = FindViewById<Android.Support.Design.Widget.NavigationView>(Resource.Id.navigation_ActivityMain_leftmenu);
                    _metrics = Resources.DisplayMetrics;
                    int sizeLeft = int.Parse(((_metrics.WidthPixels * 3) / 4).ToString());
                    _lsttest.SetMinimumWidth(sizeLeft);
                    FragmentTransaction fragmentTx;
                    fragmentTx = FragmentManager.BeginTransaction();
                    _objLeftMenuFragment = new FragmentLeftMenu();
                    fragmentTx.Replace(Resource.Id.navigation_ActivityMain_leftmenu, _objLeftMenuFragment, "FragmentLeftMenu");
                    fragmentTx.Commit();
                }
                else
                {
                    //if (MDrawerLayout != null)
                    //{
                    //    ThreadPool.QueueUserWorkItem(o => Close());
                    //}
                }
                //}
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - MainActivity - MenuLeft - Error: " + ex.Message);
#endif
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            try
            {
                if (intent.Action.Equals(QuickstartPreferences.PushRemaindLate))
                {
                    string idNotify = intent.Extras.GetString("IDNotify");
                    MainActivity.IdNotify = idNotify;
                    GetNotify(idNotify);

                }
                if (intent.Action.Equals(QuickstartPreferences.PushAutoLogout))
                {
                    //StartViewFragment StartView = new StartViewFragment();
                    //ShowFragment(FragmentManager, StartView, "StartView");
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - MainActivity - OnNewIntent - Error: " + ex.Message);
#endif
            }
        }
        /// <summary>
        /// cập nhật beanNotify lúc có push
        /// </summary>
        /// <param name="itemId">ID của beanNotify</param>
        private async void GetNotify(string itemId)
        {
            try
            {
                ProviderBase pBase = new ProviderBase();
                await Task.Run(() =>
                {
                    pBase.UpdateMasterData<BeanNotify>(null, true);
                    pBase.UpdateMasterData<BeanWorkflowItem>(null, true);
                });

                SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(CmmVariable.M_DataPath);
                string query = "SELECT * FROM BeanNotify WHERE ID = ? ";
                var res = conn.Query<BeanNotify>(query, itemId);
                if (res != null && res.Count > 0)
                {
                    RunOnUiThread(() =>
                    {
                        string queryNotify = string.Format("SELECT * FROM BeanWorkflowItem WHERE ItemID = {0} AND ListName = '{1}'", res[0].DocumentID, res[0].ListName);
                        var lstWorkflowItem = conn.Query<BeanWorkflowItem>(queryNotify);
                        string tagFragment = GetCurrentFragmentTag();
                        if (lstWorkflowItem != null && lstWorkflowItem.Count > 0)
                        {
                            if (tagFragment != null && tagFragment == "FragmentDetailWorkFlow")
                            {
                                MinionAction.ReadDetaiNew(null, new MinionAction.ChangeViewEventArgs(lstWorkflowItem[0]));
                            }
                            else
                            {
                                // nho bo ra
                                ////FragmentDetailWorkflow detailWorkFlow = new FragmentDetailWorkflow(lstWorkflowItem[0], null, "");
                                ////ShowFragment(FragmentManager, detailWorkFlow, "FragmentDetailWorkflow", 0);
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - MainActivity - GetNotify - Error: " + ex.Message);
#endif
            }
        }
        public bool OnTouch(View v, MotionEvent e)
        {
            HideSoftKeyboard(v);
            return true;
        }
        public void HideSoftKeyboard(View v)
        {
            InputMethodManager inputMethodManager = this.GetSystemService(Context.InputMethodService) as InputMethodManager;
            if (inputMethodManager != null) inputMethodManager.HideSoftInputFromWindow(v.WindowToken, HideSoftInputFlags.None);
        }
        void Close()
        {
            Thread.Sleep(200);
            RunOnUiThread(() =>
            {
                MDrawerLayout.CloseDrawers();
                MDrawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
                MDrawerLayout = null;
            });
        }
        /// <summary>
        /// đăng xuất tài khoản
        /// </summary>
        public void SignOut()
        {
            try
            {
                var dataFile = CmmVariable.M_DataPath;
                if (File.Exists(dataFile))
                    File.Delete(dataFile);
                var foderFile = CmmVariable.M_DataFolder;
                if (File.Exists(foderFile))
                    File.Delete(foderFile);
                var configAvatar = CmmVariable.M_Avatar;
                if (File.Exists(configAvatar))
                    File.Delete(configAvatar);
                var configFile = CmmVariable.M_settingFileName;
                if (File.Exists(configFile))
                    File.Delete(configFile);
                var configFolderAvatar = CmmVariable.M_Folder_Avatar;
                if (File.Exists(configFolderAvatar))
                    File.Delete(configFolderAvatar);

                CmmVariable.M_DataPath = "DB_sqlite_XamDocument.db3";
                CmmVariable.M_settingFileName = "config.ini";
                CmmVariable.M_Avatar = "avatar.jpg";
                CmmVariable.M_DataLangPath = "DB_Lang.db3";
                CmmVariable.M_Folder_Avatar = "Avatars";
                CmmVariable.M_DataFolder = "data";
                CmmVariable.M_AuthenticatedHttpClient = null;
                CmmVariable.SysConfig = null;

                RemoveAllFragmentInStack(FragmentManager);

                FragmentStartView startView = new FragmentStartView();
                ShowFragment(FragmentManager, startView, "FragmentStartView", 1);
            }
            catch (Exception)
            {
                // ignored
            }
        }
        
    }
}
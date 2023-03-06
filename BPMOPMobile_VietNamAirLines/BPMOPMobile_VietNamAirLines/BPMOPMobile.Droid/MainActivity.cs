using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System;
using Android.Views;
using Android.Content.PM;
using Android.Views.InputMethods;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Support.V4.Widget;
using Android.Content;
using BPMOPMobile.Droid.Core.Common;
using BPMOPMobile.Class;
using System.IO;
using BPMOPMobile.Droid.Class;
using BPMOPMobile.Bean;
using BPMOPMobile.DataProvider;
using static BPMOPMobile.Droid.Class.MyFirebaseInstanceIdService;
using System.Threading.Tasks;
using System.Threading;
using BPMOPMobile.Droid.Presenter.Fragment;
using BPMOPMobile.Droid.Core.Class;
using BPMOPMobile.Droid.Core.Controller;
using Android.Provider;
using Android.Text;
using Android.Database;
using Android;
using Android.Gms.Common;
using Android.Support.V4.Content;
using Android.Preferences;
using System.Collections.Generic;
using System.Linq;
using BPMOPMobile.Droid.Presenter.Fragment.FragmentChildApp;
using System.ComponentModel;
using Android.Content.Res;
using Firebase;

namespace BPMOPMobile.Droid
{

    [Activity(MainLauncher = true, Icon = "@drawable/logo_vna_2",                 // Set default Main Activity + Icon
        WindowSoftInputMode = SoftInput.AdjustPan,                               // Không cho resize khi show Keyboard
        LaunchMode = LaunchMode.SingleTop,                                       // Không cho instance chỉ nhiều lần
        Exported = true,                                                         // Cho giao tiếp với Service
        ScreenOrientation = ScreenOrientation.Portrait,                          // Không cho Vertical (Khóa hướng dọc)
        ConfigurationChanges = ConfigChanges.Navigation | ConfigChanges.ScreenLayout | ConfigChanges.UiMode |
                               ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.Density | ConfigChanges.LayoutDirection |
                               ConfigChanges.ColorMode | ConfigChanges.FontScale)]  // Không Instance lại khi change Darkmode | font scale

    public class MainActivity : AppCompatActivity
    {
        public static bool FlagRefreshDataFragment = false;                                                     // Flag check xem khi reopen view có cần renew data không
        public static int FlagNavigation = 1;                                                                   // Lưu state Navigation theo BPMOPMobile.Droid.Class.EnumBottomNavigationView
        public static int FlagNavigation_ChildOptional = (int)EnumBottomNavigationView.ChildAppSingleListVTBD;  // Lưu state Navigation Optional
        public static string _notificationID = "";                                                              // Để lưu ID notify lại -> xử lý OnNewIntent

        public KeyguardManager _keyguardManager = null;

        public static BeanWorkflow ChildAppWorkflow;                                                            // Để lưu currentWorkflow của child App

        private NavigationView _navigationView;
        private FragmentLeftMenu _fragmentLeftMenu;
        public DrawerLayout _drawerLayout;

        // Danh sách những Fragment được Unlock Left menu
        string[] _lstFragmentAllowLeftMenu = new string[] { typeof(FragmentHomePage).Name, typeof(FragmentSingleListVDT).Name, typeof(FragmentSingleListVTBD).Name, typeof(FragmentBoard).Name };

        // Danh sách những Fragment KHÔNG được OnBackPressed
        string[] _lstFragmentReject = new string[] {
            /* DPM */typeof(FragmentParent).Name,typeof(FragmentStartView).Name,typeof(Fragment_FirstLogin).Name, typeof(FragmentHomePage).Name, typeof(FragmentSingleListVDT).Name, typeof(FragmentSingleListVTBD).Name, typeof(FragmentBoard).Name, typeof(FragmentConfirmOTP).Name, typeof(FragmentReplyComment).Name,
            /* Child */ typeof(FragmentChildAppHomePage).Name, typeof(FragmentChildAppSingleListVDT).Name, typeof(FragmentChildAppSingleListVTBD).Name, typeof(FragmentChildAppList).Name, typeof(FragmentChildAppReport).Name, typeof(FragmentChildAppKanban).Name};

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightFollowSystem;
            base.OnCreate(savedInstanceState);

            Window.SetNavigationBarColor(Android.Graphics.Color.Black);
            SetContentView(Resource.Layout.ActivityMain);
            _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawerlayout_ActivityMain_Content).JavaCast<DrawerLayout>();

            if (_keyguardManager == null)
                _keyguardManager = (KeyguardManager)GetSystemService(KeyguardService);

            // GOOGLE PLAY SERVICES
            Firebase.FirebaseApp.InitializeApp(Application.Context);
            //FirebaseApp.InitializeApp(this.ApplicationContext) ;
            if (IsPlayServicesAvailable())
            {
               StartService(new Intent(this, typeof(MyFireBaseIntentService)));
            }

            // INTENT NOTIFICATION
            if (this.Intent != null)
            {
                _notificationID = "";
                this.OnNewIntent(this.Intent);
            }

            // INIT VIEW FOR ACTIVITY
            FragmentStartView startView = new FragmentStartView();
            ShowFragment(SupportFragmentManager, startView, "FragmentStartView", 1);


        }

        public override void OnBackPressed()
        {
            try
            {
                if (CmmDroidFunction.PreventMultipleClick(1000) == true)
                {
                    string _currentFragmentTag = GetCurrentFragmentTag();
                    if (!_lstFragmentReject.Contains(_currentFragmentTag)) // không nằm trong list reject -> back bình thường
                    {
                        // Nếu được back về thì check xem có Unlock Left Menu không
                        if (SupportFragmentManager.BackStackEntryCount > 2 && _lstFragmentAllowLeftMenu.Contains(SupportFragmentManager.GetBackStackEntryAt(SupportFragmentManager.BackStackEntryCount - 2).Name))
                            _drawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);

                        base.OnBackPressed();
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - MainActivity - OnBackPressed - Error: " + ex.Message);
#endif
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
            catch (Exception ex)
            {
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "OnRequestPermissionsResult", ex);
            }
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            // ai cho đổi config???
            base.OnConfigurationChanged(new Configuration(Resources.Configuration));
        }

        protected override void AttachBaseContext(Context @base)
        {
            base.AttachBaseContext(@base);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected async override void OnNewIntent(Intent intent)
        {
            try
            {
                if (intent.Action.Equals(QuickstartPreferences.PushNotificationBPM))
                {
                    if (!String.IsNullOrEmpty(GetCurrentFragmentTag())) // Đang mở app (background hoặc foreground)
                    {
                        ProviderBase _provider = new ProviderBase();
                        await Task.Run(() =>
                        {
                            _provider.UpdateMasterData<BeanNotify>(null, true);
                            _provider.UpdateMasterData<BeanWorkflowItem>(null, true);
                            _provider.UpdateMasterData<BeanAppBase>(null, true);
                            RunOnUiThread(() =>
                            {
                                MainActivity._notificationID = "";
                                FragmentDetailWorkflow detailWorkFlow = new FragmentDetailWorkflow(null, null, "FragmentHomePage");
                                AddFragment(SupportFragmentManager, detailWorkFlow, "FragmentHomePage", 0);
                            });
                        });
                    }
                    else // Đang kill app (đã có update masterdata bên startview -> )
                    {
                        MainActivity._notificationID = "123";
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - MainActivity - OnNewIntent - Error: " + ex.Message);
#endif
            }
        }

        public string GetCurrentFragmentTag()
        {
            string _result = "";
            try
            {
                _result = SupportFragmentManager.GetBackStackEntryAt(SupportFragmentManager.BackStackEntryCount - 1).Name;
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "GetCurrentFragmentTag", ex);
#endif
            }
            return _result;
        }

        /// <summary>
        /// Get List Fragment ra trong trường hợp nhiều thằng trùng tag
        /// </summary>
        /// <returns></returns>
        public List<Android.Support.V4.App.Fragment> FindListFragmentByName(string _nameTag)
        {
            List<Android.Support.V4.App.Fragment> _listFragment = new List<Android.Support.V4.App.Fragment>(this.SupportFragmentManager.Fragments)
                                                                                        .Where(x => x.GetType().Name == _nameTag)
                                                                                        .ToList();
            return _listFragment;
        }

        public void ShowFragment(Android.Support.V4.App.FragmentManager fm, CustomBaseFragment fragToShow, string fragTag, int type = 0)
        {
            string previousFragTag = "";
            try
            {
                InitMenuLeft(fragTag);
                if (fragTag != previousFragTag)
                {
                    Android.Support.V4.App.FragmentTransaction _fragmentTransaction;
                    _fragmentTransaction = fm.BeginTransaction();
                    if (type == 0)
                        _fragmentTransaction.SetCustomAnimations(Resource.Animation.fragment_enter, Resource.Animation.fragment_exit, Resource.Animation.fragment_pop_enter, Resource.Animation.fragment_pop_exit);
                    else
                        _fragmentTransaction.SetCustomAnimations(Resource.Animation.fade_in, Resource.Animation.fade_out);

                    _fragmentTransaction.Replace(Resource.Id.frame_ActivityMain_content, fragToShow, fragTag);
                    _fragmentTransaction.AddToBackStack(fragTag);
                    _fragmentTransaction.SetTransition((int)FragmentTransit.FragmentFade);
                    _fragmentTransaction.Commit();
                    //fm.ExecutePendingTransactions();
                    SupportFragmentManager.ExecutePendingTransactions();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - MainActivity - ShowFragment - Error: " + ex.Message);
#endif
            }
        }

        public void AddFragment(Android.Support.V4.App.FragmentManager fm, CustomBaseFragment fragToShow, string fragTag, int type = 0)
        {
            try
            {
                Android.Support.V4.App.FragmentTransaction _fragmentTransaction;
                _fragmentTransaction = fm.BeginTransaction();
                if (type == 0)
                    _fragmentTransaction.SetCustomAnimations(Resource.Animation.fragment_enter, Resource.Animation.fragment_exit, Resource.Animation.fragment_pop_enter, Resource.Animation.fragment_pop_exit);
                else
                    _fragmentTransaction.SetCustomAnimations(Resource.Animation.fade_in, Resource.Animation.fade_out);

                _fragmentTransaction.Add(Resource.Id.frame_ActivityMain_content, fragToShow, fragTag);
                _fragmentTransaction.AddToBackStack(fragTag);
                _fragmentTransaction.SetTransition((int)FragmentTransit.FragmentFade);
                _fragmentTransaction.Commit();
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

        public void HideFragment(string _fragmentName = null, int _type = 1)
        {
            try
            {
                ((InputMethodManager)GetSystemService(Context.InputMethodService)).HideSoftInputFromWindow(this.CurrentFocus?.WindowToken, HideSoftInputFlags.None);

                string _currentTag = GetCurrentFragmentTag();
                if (!String.IsNullOrEmpty(_fragmentName))
                {
                    InitMenuLeft(_fragmentName);
                    CustomBaseFragment previousFrag = (CustomBaseFragment)SupportFragmentManager.FindFragmentByTag(_fragmentName);
                    ShowFragment(SupportFragmentManager, previousFrag, _fragmentName, _type);
                }
                else
                {
                    if (!String.IsNullOrEmpty(_currentTag))
                    {
                        InitMenuLeft(_currentTag);
                    }

                    if (SupportFragmentManager.BackStackEntryCount > 2 && _lstFragmentAllowLeftMenu.Contains(SupportFragmentManager.GetBackStackEntryAt(SupportFragmentManager.BackStackEntryCount - 2).Name))
                    {
                        _drawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
                    }

                    //Fragment current = FragmentManager.FindFragmentByTag(tam);
                    //FragmentTransaction fragmentTx;
                    //fragmentTx = FragmentManager.BeginTransaction();
                    //fragmentTx.Remove(current);
                    //fragmentTx.SetTransition(FragmentTransit.FragmentFade);
                    //fragmentTx.Commit();
                    SupportFragmentManager.PopBackStack();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - MainActivity - HideFragment - Error: " + ex.Message);
#endif
            }
        }

        public void RemoveFragmentInStack(Android.Support.V4.App.FragmentManager fm, string fragTag)
        {
            try
            {
                CustomBaseFragment fragmentToRemove = (CustomBaseFragment)fm.FindFragmentByTag(fragTag);
                if (fragmentToRemove != null)
                {
                    //Android.Support.V4.App.FragmentTransaction _transaction = fm.BeginTransaction();


                    Android.Support.V4.App.FragmentTransaction _transaction = SupportFragmentManager.BeginTransaction();
                    _transaction.Remove(fragmentToRemove);
                    _transaction.Commit();
                    fm.PopBackStack();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - MainActivity - RemoveFragmentInStack - Error: " + ex.Message);
#endif
            }
        }

        public void RemoveAllFragmentInStack(Android.Support.V4.App.FragmentManager fm)
        {
            try
            {
                for (int i = 0; i < fm.BackStackEntryCount; i++)
                {
                    string fragTag = fm.GetBackStackEntryAt(i).Name;
                    if (!fragTag.Equals("FragmentLeftMenu"))
                    {
                        SupportFragmentManager.PopBackStack(null, (int)Android.App.PopBackStackFlags.Inclusive);
                        //SupportFragmentManager.PopBackStack();
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

        public void ReLoadCurrentFragment(Android.Support.V4.App.FragmentManager fm, string fragTag)
        {
            try
            {
                CustomBaseFragment frg = null;
                frg = (CustomBaseFragment)SupportFragmentManager.FindFragmentByTag(fragTag);
                if (frg != null)
                {
                    Android.Support.V4.App.FragmentTransaction fragmentTx = SupportFragmentManager.BeginTransaction();
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

        public void OpenDrawer()
        {
            if (_drawerLayout != null && _navigationView != null)
            {
                _drawerLayout.OpenDrawer(_navigationView);
            }
            else
            {
                InitMenuLeft(typeof(FragmentHomePage).Name);
                _drawerLayout.OpenDrawer(_navigationView);
            }
        }

        public void CloseDrawer()
        {
            if (_drawerLayout != null && _drawerLayout.IsDrawerOpen(_navigationView))
            {
                _drawerLayout.CloseDrawer(_navigationView);
            }
        }

        public void InitMenuLeft(string fragTag)
        {
            try
            {
                if (fragTag == typeof(FragmentParent).Name && _fragmentLeftMenu == null)
                {
                    if (_drawerLayout == null)
                    {
                        _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawerlayout_ActivityMain_Content).JavaCast<DrawerLayout>();
                    }
                    _drawerLayout.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
                    _navigationView = FindViewById<Android.Support.Design.Widget.NavigationView>(Resource.Id.navigation_ActivityMain_leftmenu);
                    _fragmentLeftMenu = new FragmentLeftMenu();

                    int sizeLeft = int.Parse(((base.Resources.DisplayMetrics.WidthPixels * 3) / 4).ToString());
                    _navigationView.SetMinimumWidth(sizeLeft);

                    Android.Support.V4.App.FragmentTransaction fragmentTx;
                    fragmentTx = SupportFragmentManager.BeginTransaction();
                    fragmentTx.SetCustomAnimations(Resource.Animation.fade_in, Resource.Animation.fade_out);
                    fragmentTx.Replace(Resource.Id.navigation_ActivityMain_leftmenu, _fragmentLeftMenu, "FragmentLeftMenu");
                    fragmentTx.Commit();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Author: khoahd - MainActivity - MenuLeft - Error: " + ex.Message);
#endif
            }
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

                var configFile = CmmVariable.M_settingFileName;
                if (File.Exists(configFile))
                    File.Delete(configFile);

                var foderFile = CmmVariable.M_DataFolder;
                if (File.Exists(foderFile))
                    File.Delete(foderFile);

                var configAvatar = CmmVariable.M_Avatar;
                if (File.Exists(configAvatar))
                    File.Delete(configAvatar);

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

                RemoveAllFragmentInStack(SupportFragmentManager);

                FragmentStartView firstLogin = new FragmentStartView();
                ShowFragment(SupportFragmentManager, firstLogin, "FragmentStartView", 1);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SignOut", ex);
#endif
            }
        }

        public void UpdateDBLanguage(string langCode)
        {
            try
            {
                ProviderUser pApp = new ProviderUser(); // Lấy lại ngôn ngữ default VN
                pApp.UpdateLangData(langCode, false, true);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "Override_OnActivityResult", ex);
#endif
            }
        }

        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {

                }
                else
                {

                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public BeanAttachFile GetAttachFileFromURI(Activity _mainAct, Context _context, Android.Net.Uri _selectedUri)
        {
            BeanAttachFile _res = new BeanAttachFile();
            try
            {
                if (_selectedUri != null)
                {
                    //Android.Net.Uri _selectedUri = data.Data;

                    string localPath = "";
                    if (_selectedUri.ToString().Contains("primary"))
                    {
                        localPath = this.GetActualPathFromFile(_selectedUri);
                    }
                    else if (!_selectedUri.ToString().Contains("primary") && _selectedUri.ToString().Contains("externalstorage"))//thẻ nhớ máy
                    {
                        localPath = "/storage/extSdCard/" + _selectedUri.Path.Split(':')[1];
                    }
                    else if (_selectedUri.ToString().ToLower().Contains("fileprovider"))
                    {
                        localPath = this.GetActualPathFromFile(_selectedUri);
                    }
                    else
                    {
                        localPath = this.GetActualPathFromFile(_selectedUri);
                    }


                    _res.ID = ""; // File mới ID = ""
                    _res.Title = CmmDroidFunction.GetDisplayNameOfURI(_context, _selectedUri) + ";#" + DateTime.Now.ToShortTimeString();
                    _res.Path = localPath; /* _selectedUri.Path; */
                    _res.CreatedBy = CmmVariable.SysConfig.UserId;
                    _res.IsAuthor = true;
                    _res.CreatedName = CmmVariable.SysConfig.DisplayName;
                    _res.CreatedPositon = CmmVariable.SysConfig.PositionTitle;
                    _res.AttachTypeId = null;
                    _res.AttachTypeName = "";
                    try
                    {
                        ParcelFileDescriptor fd = _mainAct.ContentResolver.OpenFileDescriptor(_selectedUri, "r");
                        _res.Size = fd.StatSize; // fd.StatSize là Bytes
                        fd.Close();
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "GetAttachFileFromURI", ex);
#endif
            }
            return _res;
        }

        public BeanAttachFile GetAttachFileFromURI_Camera(Activity _mainAct, Context _context, Android.Net.Uri _selectedUri)
        {
            BeanAttachFile _res = new BeanAttachFile();
            try
            {
                if (_selectedUri != null)
                {
                    string localPath = CmmVariable.M_Domain + "/" + _selectedUri.Path;

                    ParcelFileDescriptor fd = _mainAct.ContentResolver.OpenFileDescriptor(_selectedUri, "r");
                    _res.ID = ""; // File mới ID = ""
                    _res.Title = CmmDroidFunction.GetDisplayNameOfURI(_context, _selectedUri) + ";#" + DateTime.Now.ToShortTimeString();
                    _res.Path = localPath; /* _selectedUri.Path; */
                    _res.CreatedBy = CmmVariable.SysConfig.UserId;
                    _res.IsAuthor = true;
                    _res.CreatedName = CmmVariable.SysConfig.DisplayName;
                    _res.CreatedPositon = CmmVariable.SysConfig.PositionTitle;
                    _res.AttachTypeId = null;
                    _res.AttachTypeName = "";
                    _res.Size = fd.StatSize; // fd.StatSize là Bytes
                    fd.Close();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "GetAttachFileFromURI", ex);
#endif
            }
            return _res;
        }

        public string GetActualPathFromFile(Android.Net.Uri uri)
        {
            bool isKitKat = Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat;

            if (isKitKat && DocumentsContract.IsDocumentUri(this, uri))
            {
                // ExternalStorageProvider
                if (IsExternalStorageDocument(uri))
                {
                    string docId = DocumentsContract.GetDocumentId(uri);

                    char[] chars = { ':' };
                    string[] split = docId.Split(chars);
                    string type = split[0];

                    if ("primary".Equals(type, StringComparison.OrdinalIgnoreCase))
                    {
                       /* if (Convert.ToInt32(Build.VERSION.SdkInt) > 30 || Convert.ToInt32(Build.VERSION.SdkInt) == 30)
                        {
                            if (!Android.OS.Environment.IsExternalStorageManager)
                            {
                                Intent intent = new Intent(Android.Provider.Settings.ActionManageAppAllFilesAccessPermission, Android.Net.Uri.Parse("package:" + Application.Context.ApplicationInfo.PackageName));
                                StartActivity(intent);
                            }
                            else
                            {
                                return Android.OS.Environment.ExternalStorageDirectory + "/" + split[1];
                            }
                        }
                        else*/
                            return Android.OS.Environment.ExternalStorageDirectory + "/" + split[1];
                    }
                }
                // DownloadsProvider
                else if (IsDownloadsDocument(uri))
                {
                    string id = DocumentsContract.GetDocumentId(uri);

                    if (!TextUtils.IsEmpty(id))
                    {
                        if (id.StartsWith("raw:"))
                        {
                            return id.Replace("raw:", "");
                        }
                        try
                        {
                            /*Android.Net.Uri contentUri = ContentUris.WithAppendedId(
                                            Android.Net.Uri.Parse("content://downloads/public_downloads"), long.Parse(id));
                            return GetDataColumn(this, contentUri, null, null);*/
                            char[] chars = { ':' };
                            string[] split = id.Split(chars);

                            string type = split[0];
                           /* if (Convert.ToInt32(Build.VERSION.SdkInt) > 30 || Convert.ToInt32(Build.VERSION.SdkInt) == 30)
                            {
                                if (!Android.OS.Environment.IsExternalStorageManager)
                                {
                                    Intent intent = new Intent(Android.Provider.Settings.ActionManageAppAllFilesAccessPermission, Android.Net.Uri.Parse("package:" + Application.Context.ApplicationInfo.PackageName));
                                    StartActivity(intent);
                                    return "";
                                }
                            }*/

                            Android.Net.Uri contentUri = null;
                            if ("image".Equals(type))
                            {
                                contentUri = MediaStore.Images.Media.ExternalContentUri;
                            }
                            else if ("video".Equals(type))
                            {
                                contentUri = MediaStore.Video.Media.ExternalContentUri;
                            }
                            else if ("audio".Equals(type))
                            {
                                contentUri = MediaStore.Audio.Media.ExternalContentUri;
                            }
                            else if ("document".Equals(type) || "msf".Equals(type))
                            {
                                contentUri = MediaStore.Files.GetContentUri("external");
                            }

                            string selection = "_id=?";
                            string[] selectionArgs = new string[]
                            {
                    split[1]
                            };

                            return GetDataColumn(this, contentUri, selection, selectionArgs);
                        }
                        catch (Java.Lang.NumberFormatException)
                        {
                            return null;
                        }
                    }
                }
                // MediaProvider
                else if (IsMediaDocument(uri))
                {
                    string docId = DocumentsContract.GetDocumentId(uri);

                    char[] chars = { ':' };
                    string[] split = docId.Split(chars);

                    string type = split[0];
                    /*if (Convert.ToInt32(Build.VERSION.SdkInt) > 30 || Convert.ToInt32(Build.VERSION.SdkInt) == 30)
                    {
                        if (!Android.OS.Environment.IsExternalStorageManager)
                        {
                            Intent intent = new Intent(Android.Provider.Settings.ActionManageAppAllFilesAccessPermission, Android.Net.Uri.Parse("package:" + Application.Context.ApplicationInfo.PackageName));
                            StartActivity(intent);
                            return "";
                        }
                    }*/
                   
                    Android.Net.Uri contentUri = null;
                    if ("image".Equals(type))
                    {
                        contentUri = MediaStore.Images.Media.ExternalContentUri;
                    }
                    else if ("video".Equals(type))
                    {
                        contentUri = MediaStore.Video.Media.ExternalContentUri;
                    }
                    else if ("audio".Equals(type))
                    {
                        contentUri = MediaStore.Audio.Media.ExternalContentUri;
                    }
                    else if("document".Equals(type))
                    {
                        contentUri = MediaStore.Files.GetContentUri ("external");
                    }    

                    string selection = "_id=?";
                    string[] selectionArgs = new string[]
                    {
                    split[1]
                    };

                    return GetDataColumn(this, contentUri, selection, selectionArgs);
                }
            }
            // MediaStore (and general)
            else if ("content".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {

                // Return the remote address
                if (IsGooglePhotosUri(uri))
                    return uri.LastPathSegment;

                return GetDataColumn(this, uri, null, null);
            }
            // File
            else if ("file".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return uri.Path;
            }

            return null;
        }

        public static string GetDataColumn(Context context, Android.Net.Uri uri, string selection, string[] selectionArgs)
        {
            ICursor cursor = null;
            string column = "_data";
            string[] projection = { column };
            try
            {
                cursor = context.ContentResolver.Query(uri, projection, selection, selectionArgs, null);
                if (cursor != null && cursor.MoveToFirst())
                {
                    int index = cursor.GetColumnIndexOrThrow(column);
                    return cursor.GetString(index);
                }
            }
            finally
            {
                if (cursor != null)
                    cursor.Close();
            }
            return null;
        }

        public static bool IsExternalStorageDocument(Android.Net.Uri uri)
        {
            return "com.android.externalstorage.documents".Equals(uri.Authority);
        }

        //Whether the Uri authority is DownloadsProvider.
        public static bool IsDownloadsDocument(Android.Net.Uri uri)
        {
            return "com.android.providers.downloads.documents".Equals(uri.Authority);
        }

        //Whether the Uri authority is MediaProvider.
        public static bool IsMediaDocument(Android.Net.Uri uri)
        {
            return "com.android.providers.media.documents".Equals(uri.Authority);
        }

        //Whether the Uri authority is Google Photos.
        public static bool IsGooglePhotosUri(Android.Net.Uri uri)
        {
            return "com.google.android.apps.photos.content".Equals(uri.Authority);
        }

    }
}
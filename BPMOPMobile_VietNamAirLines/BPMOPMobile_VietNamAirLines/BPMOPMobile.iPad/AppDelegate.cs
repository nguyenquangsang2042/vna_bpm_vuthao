using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using BPMOPMobile.iPad.IOSClass;
using Foundation;
using SlideMenuControllerXamarin;
using UIKit;
using UserNotifications;

namespace BPMOPMobile.iPad
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {

        public MainView mainView { get; set; }
        public MenuView menu { get; set; }
        public UINavigationController NavController;
        public SlideMenuController SlideMenuController;
        public NSDictionary ReceiveInfo = null;
        public UIStoryboard storyboard { get; set; }
        public bool isOpenFromTerminal = false;
        public string pushDeviceToken;
        private string appstateFrom;
        public string isOpenByNotification;
        public NSObject app_ver = NSBundle.MainBundle.InfoDictionary["CFBundleVersion"];

        [Export("window")]
        public override UIWindow Window { get; set; }

        [System.Runtime.InteropServices.DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        public extern static void void_objc_msgSend_nfloat(IntPtr receiver, IntPtr selector, nfloat arg1);

        [Export("application:didFinishLaunchingWithOptions:")]
        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method
            storyboard = UIStoryboard.FromName("Main", null);

            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // Request notification permissions from the user
                UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert, (approved, err) =>
                {
                    // Handle approval
                    if (approved)
                        InvokeOnMainThread(() =>
                        {
                            UIApplication.SharedApplication.RegisterForRemoteNotifications();
                        });
                });

                ///IOS 15 thì thêm SectionHeaderTopPadding vào để không có khoảng trống giữa header với top
                if (UIDevice.CurrentDevice.CheckSystemVersion(15, 0))
                {
                    try
                    {
                        ///UITableView.Appearance.SectionHeaderTopPadding = 0.0f;
                        void_objc_msgSend_nfloat(UITableView.Appearance.Handle, ObjCRuntime.Selector.GetHandle("setSectionHeaderTopPadding:"), 0.0f);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("FinishedLaunching - SectionHeaderTopPadding: " + ex.ToString());
                    }
                }
            }
            else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
                                   UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                                   new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }
            else
            {
                UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
                UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
            }

            if (launchOptions != null)
            {
                if (launchOptions["UIApplicationLaunchOptionsRemoteNotificationKey"] != null)
                {
                    ReceiveInfo = launchOptions.ObjectForKey(new NSString("UIApplicationLaunchOptionsRemoteNotificationKey")) as NSDictionary;
                    isOpenFromTerminal = true;
                }
            }

            if (CmmIOSFunction.iosVersion() >= 13)
            {
                this.Window.OverrideUserInterfaceStyle = UIUserInterfaceStyle.Light;
            }

            return true;
        }

        #region remote push notification
        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            //var regID = deviceToken.Description.Replace("<", "").Replace(">", "").Replace(" ", "");
            // Get current device token
            // Get current device token
            byte[] result = new byte[deviceToken.Length];
            Marshal.Copy(deviceToken.Bytes, result, 0, (int)deviceToken.Length);
            var token = BitConverter.ToString(result).Replace("-", "");

            var DeviceToken = token.ToString();//deviceToken.Description;
            //var DeviceToken = deviceToken.Description;
            if (!string.IsNullOrWhiteSpace(DeviceToken))
            {
                DeviceToken = DeviceToken.Trim('<').Trim('>');
            }

            // Get previous device token
            var oldDeviceToken = NSUserDefaults.StandardUserDefaults.StringForKey("PushDeviceToken");

            // Has the token changed?
            if (string.IsNullOrEmpty(oldDeviceToken) || !oldDeviceToken.Equals(DeviceToken))
            {
                //TODO: Put your own logic here to notify your server that the device token has changed/been created!
            }

            // Save new device token 
            if (!string.IsNullOrEmpty(DeviceToken))
            {
                NSUserDefaults.StandardUserDefaults.SetString(DeviceToken, "PushDeviceToken");
                pushDeviceToken = DeviceToken.Replace("<", "").Replace(">", "").Replace(" ", "");
                Console.WriteLine("Registered: " + pushDeviceToken);
            }
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            Console.WriteLine("FailedToRegisterForRemoteNotifications: " + error.DebugDescription);
        }

        public async override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            try
            {
                isOpenByNotification = application.ApplicationState.ToString();
                if (userInfo != null)
                {
                    //ReceiveInfo = userInfo;
                    appstateFrom = application.ApplicationState.ToString();

                    var customInfo = userInfo.ObjectForKey(new NSString("CustomInfo")) as NSDictionary;

                    string templateID = string.Empty;
                    if (customInfo.ContainsKey(new NSString("TemplateId")))
                        templateID = customInfo.ObjectForKey(new NSString("TemplateId")).ToString();

                    var app = userInfo.ObjectForKey(new NSString("aps")) as NSDictionary;
                    string alert = app.ObjectForKey(new NSString("alert")).ToString();

                    string ID = string.Empty;
                    if (customInfo.ContainsKey(new NSString("ID")))
                        ID = customInfo.ObjectForKey(new NSString("ID")).ToString();

                    string ListName = string.Empty;
                    if (customInfo.ContainsKey(new NSString("ListName")))
                        ListName = customInfo.ObjectForKey(new NSString("ListName")).ToString();

                    string DocumentID = string.Empty;
                    if (customInfo.ContainsKey(new NSString("DocumentID")))
                        DocumentID = customInfo.ObjectForKey(new NSString("DocumentID")).ToString();

                    string TaskID = string.Empty;
                    if (customInfo.ContainsKey(new NSString("TaskID")))
                        TaskID = customInfo.ObjectForKey(new NSString("TaskID")).ToString();

                    if (templateID == "201") // đăng nhập bằng thiết bị khác, signout
                    {
                        //CmmIOSFunction.processReceivedRemoteNotification(storyboard, NavController, ID, ListName, DocumentID, TaskID, templateID, alert, appstateFrom);
                    }
                    else if (templateID == "1024")
                    {

                    }
                    else
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR - AppDelegate - ReceivedRemoteNotification - Err: " + ex.ToString());
            }
        }

        #endregion

        // UISceneSession Lifecycle

        //[Export ("application:configurationForConnectingSceneSession:options:")]
        //public UISceneConfiguration GetConfiguration (UIApplication application, UISceneSession connectingSceneSession, UISceneConnectionOptions options)
        //{
        //    // Called when a new scene session is being created.
        //    // Use this method to select a configuration to create the new scene with.
        //    return UISceneConfiguration.Create ("Default Configuration", connectingSceneSession.Role);
        //}

        //[Export ("application:didDiscardSceneSessions:")]
        //public void DidDiscardSceneSessions (UIApplication application, NSSet<UISceneSession> sceneSessions)
        //{
        //    // Called when the user discards a scene session.
        //    // If any sessions were discarded while the application was not running, this will be called shortly after `FinishedLaunching`.
        //    // Use this method to release any resources that were specific to the discarded scenes, as they will not return.
        //}
    }
}


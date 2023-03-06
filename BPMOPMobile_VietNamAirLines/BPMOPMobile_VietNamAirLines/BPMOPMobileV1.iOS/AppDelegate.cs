using BPMOPMobile.Bean;
using BPMOPMobile.DataProvider;
using BPMOPMobileV1.iOS.IOSClass;
using BPMOPMobileV1.iOS.ViewControllers;
using Foundation;
//using SidebarNavigation;
using System;
using System.Threading.Tasks;
using UIKit;
using TelerikUI;
using UserNotifications;
using SlideMenuControllerXamarin;
using System.Runtime.InteropServices;

namespace BPMOPMobileV1.iOS
{

    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate, IUIApplicationDelegate
    {
        public MainView mainView { get; set; }
        public MenuView menu { get; set; }
        public UINavigationController NavController;
        public SlideMenuController SlideMenuController;
        //public SidebarController SideBarController;
        public NSDictionary ReceiveInfo = null;
        public UIStoryboard storyboard { get; set; }
        public bool isOpenFromTerminal = false;
        public string pushDeviceToken;
        private string appstateFrom;
        public NSObject app_ver = NSBundle.MainBundle.InfoDictionary["CFBundleVersion"];
        public NSMutableDictionary beanUpdateManagement = new NSMutableDictionary();

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            storyboard = UIStoryboard.FromName("Main", null);



            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound | UNAuthorizationOptions.Sound,
                                                                        (granted, error) =>
                                                                        {
                                                                            if (granted)
                                                                                InvokeOnMainThread(UIApplication.SharedApplication.RegisterForRemoteNotifications);
                                                                        });
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

            return true;
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message)
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive.
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }

        #region remote push notification

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            //var regID = deviceToken.Description.Replace("<", "").Replace(">", "").Replace(" ", "");
            // Get current device token
            if (deviceToken != null)
            {
                byte[] result = new byte[deviceToken.Length];
                Marshal.Copy(deviceToken.Bytes, result, 0, (int)deviceToken.Length);

                var DeviceToken = BitConverter.ToString(result).Replace("-", "");//deviceToken.Description;
                if (!string.IsNullOrWhiteSpace(DeviceToken))
                {
                    DeviceToken = DeviceToken.Trim('<').Trim('>');
                }
                // Get previous device token
                var oldDeviceToken = NSUserDefaults.StandardUserDefaults.StringForKey("PushDeviceToken");

                // Has the token changed?
                if (string.IsNullOrEmpty(oldDeviceToken) || !string.Equals(oldDeviceToken, DeviceToken, StringComparison.OrdinalIgnoreCase))
                {
                    //TODO: Put your own logic here to notify your server that the device token has changed/been created!
                }

                // Save new device token 
                if (!string.IsNullOrEmpty(DeviceToken))
                {
                    NSUserDefaults.StandardUserDefaults.SetString(DeviceToken, "PushDeviceToken");
                    pushDeviceToken = DeviceToken.Replace("<", "", StringComparison.OrdinalIgnoreCase).Replace(">", "", StringComparison.OrdinalIgnoreCase).Replace(" ", "", StringComparison.OrdinalIgnoreCase);
                    Console.WriteLine("Registered: " + pushDeviceToken);
                }

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
                        CmmIOSFunction.processReceivedRemoteNotification(storyboard, NavController, ID, ListName, DocumentID, TaskID, templateID, alert, appstateFrom);
                    }
                    else
                    {
                        await Task.Run(() =>
                        {

                            //var keySchedule = CmmFunction.getAppSetting("ScheduleImportAnnounceCategoryId");

                            ProviderBase p_base = new ProviderBase();
                            p_base.UpdateMasterData<BeanNotify>(null, true);
                            p_base.UpdateMasterData<BeanWorkflowItem>(null, true);

                            //p_base.UpdateAllMasterData(true);

                            InvokeOnMainThread(() =>
                            {
                                CmmIOSFunction.processReceivedRemoteNotification(storyboard, NavController, ID, ListName, DocumentID, TaskID, templateID, alert, appstateFrom);
                            });
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR - AppDelegate - ReceivedRemoteNotification - Err: " + ex.ToString());
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Core.Common;
using Firebase.Iid;
using Newtonsoft.Json;
using static Android.Provider.Settings;

namespace BPMOPMobile.Droid.Class
{
    [Service] 
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class MyFirebaseInstanceIdService : FirebaseInstanceIdService
    {
        const string Tag = "MyFirebaseIIDService";

        public override void OnTokenRefresh()
        {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            Log.Debug(Tag, "Refreshed token: " + refreshedToken);
            SendRegistrationToServer();
            SaveRegistrationToken(refreshedToken);
        }
        public void SaveRegistrationToken(string token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            try
            {
                DeviceInfo objDevice = new DeviceInfo();
                try
                {
                    objDevice.DevicePushToken = FirebaseInstanceId.Instance.Token;
                    CmmDroidFunction.LogErrToSDCard(FirebaseInstanceId.Instance.Token);
                }
                catch (Exception ex)
                {
#if DEBUG
                    CmmDroidFunction.WriteTrackingError(this.GetType().Name, "SaveRegistrationToken", ex);
#endif
                    objDevice.DevicePushToken = "";
                }


                BluetoothAdapter _bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
                objDevice.DeviceName = _bluetoothAdapter.Name;
                objDevice.DeviceModel = Build.Model;
                objDevice.DeviceId = Secure.GetString(ContentResolver, Secure.AndroidId);
                objDevice.DeviceOS = 1;
                objDevice.DeviceOSVersion = Build.VERSION.SdkInt.ToString();
                objDevice.AppVersion = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName;

                CmmVariable.SysConfig.DeviceInfo = JsonConvert.SerializeObject(objDevice);
            }
            catch (Exception)
            {
                // ignored
            }
        }
        void SendRegistrationToServer()
        {

        }
        public static class QuickstartPreferences
        {
            public const string PushNotificationBPM = "PushNotificationBPM";

            public const string PushRemaindLate = "RemindLate";
            public const string PushAutoLogout = "AutoLogout";

            public const string SENT_TOKEN_TO_SERVER = "sentTokenToServer";
            public const string REGISTRATION_COMPLETE = "registrationComplete";
            public const string PUSH_APP = "PushApp";
            public const string PUSH_REMAIND_LATE = "RemindLate";
            public const string PUSH_AUTO_LOGOUT = "AutoLogout";
            public const string WAKE_UPAPP = "WakeUpApp";
        }
    }
}
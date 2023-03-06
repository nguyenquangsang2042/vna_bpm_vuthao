using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using Firebase.Iid;
using Newtonsoft.Json;
using static Android.Provider.Settings;

namespace BPMOPMobile.Droid.Class
{
    [Obsolete] [Service] [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class MyFirebaseIidService : FirebaseInstanceIdService
    {
        const string Tag = "MyFirebaseIIDService";

        [Obsolete]
        public override void OnTokenRefresh()
        {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            Log.Debug(Tag, "Refreshed token: " + refreshedToken);
            SendRegistrationToServer();
            SaveRegistrationToken(refreshedToken);
        }
        //server okiaf
        public void SaveRegistrationToken(string token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            try
            {
                DeviceInfo objDevice = new DeviceInfo();
                objDevice.DeviceId = Secure.GetString(ContentResolver, Secure.AndroidId);
                objDevice.DeviceOS = 1;
                token = FirebaseInstanceId.Instance.Token;
                if (!string.IsNullOrEmpty(token))
                {
                    objDevice.DevicePushToken = token;
                }
                string device = JsonConvert.SerializeObject(objDevice);
                CmmVariable.SysConfig.DeviceInfo = device;
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
            public const string PushRemaindLate = "RemindLate";
            public const string PushAutoLogout = "AutoLogout";
        }
    }
}
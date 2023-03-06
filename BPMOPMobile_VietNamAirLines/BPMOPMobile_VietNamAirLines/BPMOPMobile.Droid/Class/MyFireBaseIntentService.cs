using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Droid.Core.Common;
using Firebase.Iid;

namespace BPMOPMobile.Droid.Class
{
    [Service]
    class MyFireBaseIntentService : IntentService
    {
        const string TAG = "RegIntentService";
        static object locker = new object();
        static readonly string[] TOPICS = { "global" };
        public static string stringt = "";
        public MyFireBaseIntentService() : base(TAG)
        {

        }

        protected override void OnHandleIntent(Intent intent)
        {
            var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);
            try
            {
                lock (locker)
                {
#if DEBUG
                    //var instanceId = FirebaseInstanceId.Instance;
                    //var token = FirebaseInstanceId.Instance.Token;

                    //if (token != null)
                    //{
                    //    instanceId.DeleteToken(token, "");
                    //    instanceId.DeleteInstanceId();


                    //}
                    //token = instanceId.Token;
#endif
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "OnHandleIntent", ex);
#endif
            }
        }
    }
}
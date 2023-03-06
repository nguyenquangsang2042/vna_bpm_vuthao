using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Droid.Core.Common;
using Firebase.Messaging;
using static BPMOPMobile.Droid.Class.MyFirebaseInstanceIdService;
using static BPMOPMobile.Droid.Class.MyFireBaseIntentService;

namespace BPMOPMobile.Droid.Class
{
    [Service(Exported = true)]
    [BroadcastReceiver]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        private Android.Net.Uri _notificationSoundURI = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
        private string _groupKey = "com.vuthao.BPM.VNABPM";
        string _channelID = "BPMOPChannelID";
        string _channelName = "BPMOPChannelName";


        public override void OnCreate()
        {
            CmmDroidFunction.LogErrToSDCard("MyFirebaseMessagingService OnCreate()");
            base.OnCreate();
        }
        public override void OnMessageReceived(RemoteMessage message)
        {
            CmmDroidFunction.LogErrToSDCard("MyFirebaseMessagingService OnMessageReceived()");
            base.OnMessageReceived(message);
            try
            {
                IDictionary<string, string> _data = message.Data;

                if (_data != null)
                {
                    string _title = _data.ContainsKey("Title") ? _data["Title"].ToString() : "";
                    string _body = _data.ContainsKey("Content") ? _data["Content"].ToString() : "";
                    string _url = _data.ContainsKey("Url") ? _data["Url"].ToString() : "";

                    string _resourceID = _data.ContainsKey("ResourceId") ? _data["ResourceId"].ToString() : "";
                    string _resourceCategoryID = _data.ContainsKey("ResourceCategoryId") ? _data["ResourceCategoryId"].ToString() : "";
                    string _resourceSubCategoryID = _data.ContainsKey("ResourceSubCategoryId") ? _data["ResourceSubCategoryId"].ToString() : "";
                    
                    CreateNotification(_title, _body, _url);

                }
            }
            catch (Exception ex)
            {
                CmmDroidFunction.LogErrToSDCard("MyFirebaseMessagingService OnMessageReceived() catch");
                CreateNotification("", "", "");
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "OnMessageReceived", ex);
#endif
            }
        }
        /// <summary>
        /// Tạo noti và show lên phone
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="Title"></param>
        /// <param name="URL"></param>
        public void CreateNotification(string Message, string Title, string URL)
        {
            try
            {
                #region Builder
                Bundle valuesForActivity = new Bundle();

                // Intent
                Intent _intent = new Intent(this, typeof(MainActivity));
                _intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
                _intent.PutExtras(valuesForActivity);
                _intent.SetAction(QuickstartPreferences.PushNotificationBPM);

                // Gán Intent vào PendingIntent
                PendingIntent _pendingIntent = PendingIntent.GetActivity(this, new Random().Next(), _intent, PendingIntentFlags.UpdateCurrent);

                NotificationCompat.Builder _notiBuilder = new NotificationCompat.Builder(this, _channelID)
                                 .SetChannelId(_channelID)
                                 .SetContentIntent(_pendingIntent)
                                 .SetContentTitle(Title)
                                 .SetContentText(Message)
                                 .SetSmallIcon(Resource.Drawable.logo_ver2)
                                 .SetAutoCancel(true)
                                 .SetSound(_notificationSoundURI)
                                 .SetGroup(_groupKey)
                                 .SetVibrate(new long[] { 2000, 2000, 2000, 2000, 2000 }).SetPriority(2).SetVisibility(1)
                                 .SetColor(new Color(ContextCompat.GetColor(this, Resource.Color.clViolet))); // Màu chữ Title
                #endregion

                #region Notification
                Notification _notification = _notiBuilder.Build();
                NotificationManager _notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
                NotificationChannel _notificationChannel = _notificationManager.GetNotificationChannel(_channelID);

                if (_notificationChannel == null)
                {
                    _notificationChannel = new NotificationChannel(_channelID, _channelName, NotificationImportance.High);
                    _notificationChannel.EnableVibration(true);
                    _notificationChannel.EnableLights(true);
                    _notificationChannel.LockscreenVisibility = NotificationVisibility.Public;
                    _notificationManager.CreateNotificationChannel(_notificationChannel);
                }
                _notificationChannel?.Dispose();
                _notificationManager.Notify((int)(DateTime.Now.Ticks - (new DateTime(2010, 1, 1)).Ticks), _notification);

                #endregion

            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "CreateNotification", ex);
#endif
            }
        }

    }
}
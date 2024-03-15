using Android.App;
using Android.Content;
using Android.Gms.Common.Apis;
using Android.Graphics;
using Android.Media;
using Android.Util;
using AndroidX.Core.App;
using Firebase.Messaging;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.MyTNBService.Request;
using myTNB.AndroidApp.Src.MyTNBService.Response;
using myTNB.AndroidApp.Src.MyTNBService.ServiceImpl;
using myTNB.AndroidApp.Src.NotificationDetails.Activity;
using myTNB.AndroidApp.Src.Notifications.Activity;
using myTNB.AndroidApp.Src.OverVoltageFeedback.Activity;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;

namespace myTNB.AndroidApp.Src.Firebase.Services
{
    //[Service]
    //[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    internal class FCMNotificationMessageService : FirebaseMessagingService
    {
        CancellationTokenSource cts;
        private PendingIntent pendingIntent;
        readonly string TAG = typeof(FCMNotificationMessageService).Name;
        public bool flag = false;
        public NotificationDetails.Models.NotificationDetails UserNotificationDetail { get; set; }
        /// <summary>
        /// If App is killed , we cannot get onMessageReceived here
        /// So the notification and this one should be separate entity or have separate processing of notification
        /// </summary>
        /// <param name="message"></param>
        public override void OnMessageReceived(RemoteMessage remoteMessage)
        {
            if (!UserEntity.IsCurrentlyActive())
            {
                return;
            }
            UserEntity userEntity = UserEntity.GetActive();

            IDictionary<string, string> remoteData = remoteMessage.Data;

            string title = string.Empty;
            if (remoteMessage.GetNotification().Title != null)
            {
                title = remoteMessage.GetNotification().Title;
            }

            string message = string.Empty;
            if (remoteMessage.GetNotification().Body != null)
            {
                message = remoteMessage.GetNotification().Body;
            }

            if (remoteData.ContainsKey("EventId") && remoteData.ContainsKey("Type") && remoteData.ContainsKey("RequestTransId"))
            {
                UserSessions.SetNotification(remoteData["Type"], remoteData["EventId"], remoteData["RequestTransId"]);
            }

            // SendNotification(title, message);
            //if (remoteData.ContainsKey("Badge") && int.TryParse(remoteData["Badge"], out int count))
            try
            {
                if (remoteData.ContainsKey("Type") && remoteData.ContainsKey("EventId") && remoteData.ContainsKey("RequestTransId"))
                {
                    UserSessions.SetNotification(remoteData["Type"], remoteData["EventId"], remoteData["RequestTransId"]);
                }

                SendNotification(title, message, remoteData);
                if (remoteData.ContainsKey("Badge") && int.TryParse(remoteData["Badge"], out int count))
                {
                    if (count <= 0)
                    {
                        ME.Leolin.Shortcutbadger.ShortcutBadger.RemoveCount(this.ApplicationContext);
                    }
                    else
                    {
                        ME.Leolin.Shortcutbadger.ShortcutBadger.ApplyCount(this.ApplicationContext, count);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug("ERROR", e.Message);
            }
        }

        //private async void QueryNotifications(string title, string message)
        //{
        //}

        private void SendNotification(string title, string message, IDictionary<string, string> remoteData)
        {
            if (remoteData.ContainsKey("claimId"))
            {
                Intent intent = new Intent(this, typeof(OverVoltageFeedbackDetailActivity));               
                //intent.AddFlags(ActivityFlags.ClearTop);                
                intent.AddFlags(ActivityFlags.ClearTop);
                intent.PutExtra("ClaimId",remoteData["claimId"]);
                PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

                Android.Net.Uri defaultSoundUri = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
                NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(this)
                        .SetSmallIcon(Resource.Drawable.ic_launcher)
                        .SetLargeIcon(BitmapFactory.DecodeResource(this.Resources,
                                Resource.Drawable.ic_launcher))
                        .SetContentTitle(title)
                        .SetContentText(message)
                        .SetAutoCancel(true)
                        .SetSound(defaultSoundUri)
                        .SetContentIntent(pendingIntent);

                NotificationManager notificationManager =
                        (NotificationManager)GetSystemService(Context.NotificationService);

                notificationManager.Notify(0, notificationBuilder.Build());
            }
            else
            {
            if (UserSessions.Notification != null)
            {
                Intent intent = new Intent(this, typeof(UserNotificationDetailActivity));
                intent.PutExtra(Utils.Constants.SELECTED_FROMDASHBOARD_NOTIFICATION_DETAIL_ITEM, JsonConvert.SerializeObject(UserSessions.Notification));
                intent.AddFlags(ActivityFlags.ClearTop);
                pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);
            }
            else
            {
                flag = true;
                Intent intent = new Intent(this, typeof(NotificationActivity));
                intent.PutExtra(Utils.Constants.HAS_NOTIFICATION, true);
                intent.AddFlags(ActivityFlags.ClearTop);
                pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);
            }
            // Intent intent = new Intent(this, typeof(NotificationActivity));
            // intent.PutExtra(Utils.Constants.HAS_NOTIFICATION, true);

            // intent.AddFlags(ActivityFlags.ClearTop);
            // PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            if (UserSessions.Notification != null || flag)
            {
                Android.Net.Uri defaultSoundUri = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
                NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(this)
                        .SetSmallIcon(Resource.Drawable.ic_launcher)
                        .SetLargeIcon(BitmapFactory.DecodeResource(this.Resources,
                                Resource.Drawable.ic_launcher))
                        .SetContentTitle(title)
                        .SetContentText(message)
                        .SetAutoCancel(true)
                        .SetSound(defaultSoundUri)
                        .SetContentIntent(pendingIntent);

                NotificationManager notificationManager =
                        (NotificationManager)GetSystemService(Context.NotificationService);

                notificationManager.Notify(0, notificationBuilder.Build());
            }
            }
        }
    }
}
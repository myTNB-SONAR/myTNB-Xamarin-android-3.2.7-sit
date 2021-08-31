using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using AndroidX.Core.App;
using Firebase.Messaging;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Notifications.Activity;
using myTNB_Android.Src.OverVoltageFeedback.Activity;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;
using System.Threading;

namespace myTNB_Android.Src.Firebase.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    internal class FCMNotificationMessageService : FirebaseMessagingService
    {
        CancellationTokenSource cts;
        readonly string TAG = typeof(FCMNotificationMessageService).Name;
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
            //var x = remoteData["claimId"];
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

        private async void QueryNotifications(string title, string message)
        {
        }

        private void SendNotification(string title, string message, IDictionary<string, string> remoteData)
        {
            Intent intent;
            if (remoteData.ContainsKey("claimId"))
            {
                intent = new Intent(this, typeof(OverVoltageFeedbackDetailActivity));               
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
                intent = new Intent(this, typeof(NotificationActivity));
                intent.PutExtra(Constants.HAS_NOTIFICATION, true);
                intent.AddFlags(ActivityFlags.ClearTop);


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
                

          
        }
    }
}
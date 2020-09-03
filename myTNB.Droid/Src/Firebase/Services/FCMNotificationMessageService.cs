using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using AndroidX.Core.App;
using Firebase.Messaging;
using myTNB_Android.Src.AppLaunch.Api;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.AppLaunch.Requests;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Notifications.Activity;
using myTNB_Android.Src.Utils;
using Refit;
using System;
using System.Collections.Generic;
using System.Net.Http;
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
            //Log.Debug(TAG, "From: " + message.From);
            //Log.Debug(TAG, "Notification Message Body: " + message.GetNotification().Body);
            //Console.WriteLine("From: " + message.From);
            //Console.WriteLine("From: " + message.From);
            //// COUNT ALL THE UNREAD USER NOTIFICATIONS HE
            //ME.Leolin.Shortcutbadger.ShortcutBadger.ApplyCount(this.ApplicationContext, UserNotificationEntity.Count());

            if (!UserEntity.IsCurrentlyActive())
            {
                return;
            }
            UserEntity userEntity = UserEntity.GetActive();

            IDictionary<string, string> remoteData = remoteMessage.Data;
            //if (remoteData.ContainsKey("Email"))
            //{
                String title = "";
                if (remoteMessage.GetNotification().Title != null)
                {
                    title = remoteMessage.GetNotification().Title;
                }

                String message = "";
                if (remoteMessage.GetNotification().Body != null)
                {
                    message = remoteMessage.GetNotification().Body;
                }

                SendNotification(title, message);
            //}
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
//            cts = new CancellationTokenSource();
//#if DEBUG
//            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
//            var api = RestService.For<INotificationApi>(httpClient);
//#else
//            var api = RestService.For<INotificationApi>(Constants.SERVER_URL.END_POINT);
//#endif

//            try
//            {
//                var appNotificationChannelsResponse = await api.GetAppNotificationChannels(new NotificationRequest()
//                {
//                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID
//                }, cts.Token);

//                var appNotificationTypesResponse = await api.GetAppNotificationTypes(new NotificationRequest()
//                {
//                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID
//                }, cts.Token);



//                if (!appNotificationChannelsResponse.Data.IsError)
//                {
//                    if (!appNotificationTypesResponse.Data.IsError)
//                    {


//                        foreach (NotificationChannels notificationChannel in appNotificationChannelsResponse.Data.Data)
//                        {
//                            NotificationChannelEntity.InsertOrReplaceAsync(notificationChannel);

//                        }

//                        foreach (NotificationTypes notificationTypes in appNotificationTypesResponse.Data.Data)
//                        {
//                            NotificationTypesEntity.InsertOrReplaceAsync(notificationTypes);

//                        }

//                        if (UserEntity.IsCurrentlyActive())
//                        {
//                            NotificationApiImpl notificationAPI = new NotificationApiImpl();
//                            MyTNBService.Response.UserNotificationResponse response = await notificationAPI.GetUserNotifications<MyTNBService.Response.UserNotificationResponse>(new Base.Request.APIBaseRequest());
//                            if (response != null && response.Data != null && response.Data.ErrorCode == "7200")
//                            {
//                                if (response.Data.ResponseData != null && response.Data.ResponseData.UserNotificationList != null &&
//                                    response.Data.ResponseData.UserNotificationList.Count > 0)
//                                {
//                                    foreach (UserNotification userNotification in response.Data.ResponseData.UserNotificationList)
//                                    {
//                                        // TODO : SAVE ALL NOTIFICATIONs
//                                        int newRecord = UserNotificationEntity.InsertOrReplace(userNotification);
//                                    }
//                                }

//                                SendNotification(title, message);
//                            }
//                        }

//                    }

//                }





//            }
//            catch (ApiException apiException)
//            {

//            }
//            catch (Exception e)
//            {

//            }
        }

        private void SendNotification(String title, String message)
        {

            Intent intent = new Intent(this, typeof(NotificationActivity));
            intent.PutExtra(Constants.HAS_NOTIFICATION, true);

            intent.AddFlags(ActivityFlags.ClearTop);
            PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0 /* Request code */, intent,
                    PendingIntentFlags.OneShot);

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
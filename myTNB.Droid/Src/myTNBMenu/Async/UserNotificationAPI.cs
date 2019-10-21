using Android.OS;
using myTNB_Android.Src.AppLaunch.Api;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.AppLaunch.Requests;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.MVP;
using myTNB_Android.Src.MyTNBService.Notification;
using myTNB_Android.Src.Utils;
using Refit;
using System;
using System.Threading;

namespace myTNB_Android.Src.myTNBMenu.Async
{
    public class UserNotificationAPI : AsyncTask
    {
        CancellationTokenSource cts = new CancellationTokenSource();
#if DEBUG
        INotificationApi api = RestService.For<INotificationApi>(Constants.SERVER_URL.END_POINT);
#else
        INotificationApi api = RestService.For<INotificationApi>(Constants.SERVER_URL.END_POINT);
#endif
        private string deviceId = null;
        private DashboardHomeContract.IUserActionsListener homeListener = null;

        public UserNotificationAPI(string deviceId, DashboardHomeContract.IUserActionsListener listener)
        {
            this.deviceId = deviceId;
            this.homeListener = listener;
        }


        protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
        {
            try
            {
                Console.WriteLine("000 UserNotificationAPI started");
                GetUserNotifications();
                Console.WriteLine("000 UserNotificationAPI ended");
            }
            catch (ApiException apiException)
            {
                try
                {
                    UserNotificationEntity.RemoveAll();
                }
                catch (System.Exception ne)
                {
                    Utility.LoggingNonFatalError(ne);
                }
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Newtonsoft.Json.JsonReaderException e)
            {
                try
                {
                    UserNotificationEntity.RemoveAll();
                }
                catch (System.Exception ne)
                {
                    Utility.LoggingNonFatalError(ne);
                }
                Utility.LoggingNonFatalError(e);
            }
            catch (System.Exception e)
            {
                try
                {
                    UserNotificationEntity.RemoveAll();
                }
                catch (System.Exception ne)
                {
                    Utility.LoggingNonFatalError(ne);
                }
                Utility.LoggingNonFatalError(e);
            }
            return null;
        }

        private async void GetUserNotifications()
        {
            try
            {
                NotificationApiImpl notificationAPI = new NotificationApiImpl();
                MyTNBService.Response.UserNotificationResponse response = await notificationAPI.GetUserNotifications<MyTNBService.Response.UserNotificationResponse>(new Base.Request.APIBaseRequest());
                if (response.Data != null && response.Data.ErrorCode == "7200")
                {
                    if (response.Data.ResponseData != null && response.Data.ResponseData.UserNotificationList != null &&
                        response.Data.ResponseData.UserNotificationList.Count > 0)
                    {
                        foreach (UserNotification userNotification in response.Data.ResponseData.UserNotificationList)
                        {
                            // tODO : SAVE ALL NOTIFICATIONs
                            int newRecord = UserNotificationEntity.InsertOrReplace(userNotification);
                        }
                    }
                    else
                    {
                        try
                        {
                            UserNotificationEntity.RemoveAll();
                        }
                        catch (System.Exception ne)
                        {
                            Utility.LoggingNonFatalError(ne);
                        }
                    }
                }
                else
                {
                    try
                    {
                        UserNotificationEntity.RemoveAll();
                    }
                    catch (System.Exception ne)
                    {
                        Utility.LoggingNonFatalError(ne);
                    }
                }
            }
            catch (ApiException apiException)
            {
                try
                {
                    UserNotificationEntity.RemoveAll();
                }
                catch (System.Exception ne)
                {
                    Utility.LoggingNonFatalError(ne);
                }
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Newtonsoft.Json.JsonReaderException e)
            {
                try
                {
                    UserNotificationEntity.RemoveAll();
                }
                catch (System.Exception ne)
                {
                    Utility.LoggingNonFatalError(ne);
                }
                Utility.LoggingNonFatalError(e);
            }
            catch (System.Exception e)
            {
                try
                {
                    UserNotificationEntity.RemoveAll();
                }
                catch (System.Exception ne)
                {
                    Utility.LoggingNonFatalError(ne);
                }
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void OnPostExecute(Java.Lang.Object result)
        {
            base.OnPostExecute(result);

            if (homeListener != null)
            {
                homeListener.OnNotificationCount();
            }
        }

    }
}

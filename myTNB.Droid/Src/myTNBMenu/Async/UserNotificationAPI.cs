using Android.OS;
using myTNB_Android.Src.AppLaunch.Api;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.AppLaunch.Requests;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.MVP;
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
        private DashboardContract.IUserActionsListener listener = null;

        public UserNotificationAPI(string deviceId, DashboardContract.IUserActionsListener listener)
        {
            this.deviceId = deviceId;
            this.listener = listener;
        }


        protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
        {
            try
            {
                Console.WriteLine("000 UserNotificationAPI started");
                UserEntity loggedUser = UserEntity.GetActive();

                var userNotificationResponse = api.GetUserNotifications(new UserNotificationRequest()
                {
                    ApiKeyId = Constants.APP_CONFIG.API_KEY_ID,
                    Email = loggedUser.Email,
                    DeviceId = deviceId

                }, cts.Token);

                if (userNotificationResponse != null && userNotificationResponse.Result != null && userNotificationResponse.Result.Data.Status.ToUpper() == Constants.REFRESH_MODE)
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
                else if (userNotificationResponse.Result != null && userNotificationResponse.Result.Data != null)
                {
                    if (!userNotificationResponse.Result.Data.IsError)
                    {
                        if (userNotificationResponse.Result.Data.Data.Count > 0) 
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
                        foreach (UserNotification userNotification in userNotificationResponse.Result.Data.Data)
                        {
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

        protected override void OnPostExecute(Java.Lang.Object result)
        {
            base.OnPostExecute(result);


            listener.OnNotificationCount();
        }

    }
}

using System;
using Android.Content;
using Android.OS;
using Java.Lang;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;
using static myTNB_Android.Src.FindUs.Response.GetLocationTypesResponse;

namespace myTNB_Android.Src.AppLaunch.Async
{
    public class MasterApiDBOperation : AsyncTask
    {

        private MasterDataResponse masterDataResponse = null;
        private ISharedPreferences preferences = null;

        public MasterApiDBOperation(MasterDataResponse masterDataResponse, ISharedPreferences preferences)
        {
            this.preferences = preferences;
            this.masterDataResponse = masterDataResponse;
        }

        protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
        {

            Console.WriteLine("========= 0000 MasterApiDBOperation started");
            if (masterDataResponse != null && masterDataResponse.Data != null) {
                if (!masterDataResponse.Data.IsError && !masterDataResponse.Data.Status.Equals(Constants.MAINTENANCE_MODE)) {
                    foreach (Weblink web in masterDataResponse.Data.MasterData.WebLinks)
                    {
                        int newRecord = WeblinkEntity.InsertOrReplace(web);
                        //Log.Debug(TAG, "New Weblink Record " + newRecord);
                    }

                    //Log.Debug(TAG, "Weblink Records " + WeblinkEntity.Count());

                    FeedbackCategoryEntity.RemoveActive();
                    foreach (FeedbackCategory cat in masterDataResponse.Data.MasterData.FeedbackCategorys)
                    {
                        int newRecord = FeedbackCategoryEntity.InsertOrReplace(cat);
                    }

                    int ctr = 0;
                    FeedbackStateEntity.RemoveActive();
                    foreach (FeedbackState state in masterDataResponse.Data.MasterData.States)
                    {
                        bool isSelected = ctr == 0 ? true : false;
                        int newRecord = FeedbackStateEntity.InsertOrReplace(state, isSelected);
                        ctr++;
                    }


                    FeedbackTypeEntity.RemoveActive();
                    ctr = 0;
                    foreach (FeedbackType type in masterDataResponse.Data.MasterData.FeedbackTypes)
                    {
                        bool isSelected = ctr == 0 ? true : false;
                        int newRecord = FeedbackTypeEntity.InsertOrReplace(type, isSelected);
                        Console.WriteLine(string.Format("FeedbackType Id = {0}", newRecord));
                        ctr++;
                    }


                    foreach (NotificationChannels notificationChannel in masterDataResponse.Data.MasterData.NotificationTypeChannels)
                    {
                        int newRecord = NotificationChannelEntity.InsertOrReplace(notificationChannel);
                        //Log.Debug(TAG, "New Channel Record " + newRecord);
                    }

                    foreach (NotificationTypes notificationTypes in masterDataResponse.Data.MasterData.NotificationTypes)
                    {
                        int newRecord = NotificationTypesEntity.InsertOrReplace(notificationTypes);
                        //Log.Debug(TAG, "New Type Record " + newRecord);
                    }

                    LocationTypesEntity.InsertFristRecord();
                    foreach (LocationType loc in masterDataResponse.Data.MasterData.LocationTypes)
                    {
                        int newRecord = LocationTypesEntity.InsertOrReplace(loc);
                        //Log.Debug(TAG, "Location Types Record " + newRecord);
                    }


                    //Log.Debug(TAG, "Location Records " + LocationTypesEntity.Count());

                    DownTimeEntity.RemoveActive();
                    foreach (DownTime cat in masterDataResponse.Data.MasterData.Downtimes)
                    {
                        int newRecord = DownTimeEntity.InsertOrReplace(cat);
                    }

                    int appCurrentVersion = DeviceIdUtils.GetAppVersionCode();
                    if (UserEntity.IsCurrentlyActive()) {
                        int prevAppVersionCode = UserSessions.GetPrevAppVersionCode(preferences);
                        if (prevAppVersionCode > 0) {
                            if (prevAppVersionCode < appCurrentVersion) {
                                SMUsageHistoryEntity.RemoveAll();
                            }
                        }else{
                            SMUsageHistoryEntity.RemoveAll();
                        }
                    }
                    UserSessions.SetAppVersionCode(preferences, appCurrentVersion);



                    //Log.Debug(TAG, "DownTime Records " + DownTimeEntity.Count());
                }
            }

            Console.WriteLine("========= 0000 MasterApiDBOperation ended");

            return null;
        }
    }
}

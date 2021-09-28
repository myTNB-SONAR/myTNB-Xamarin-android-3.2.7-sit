using Android.Content;
using Android.OS;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.Utils;
using System;
using static myTNB_Android.Src.FindUs.Response.GetLocationTypesResponse;

namespace myTNB_Android.Src.AppLaunch.Async
{
    public class MasterApiDBOperation : AsyncTask
    {

        private AppLaunchMasterDataResponseAWS masterDataResponse = null;
        private ISharedPreferences preferences = null;

        public MasterApiDBOperation(AppLaunchMasterDataResponseAWS masterDataResponse, ISharedPreferences preferences)
        {
            this.preferences = preferences;
            this.masterDataResponse = masterDataResponse;
        }

        protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
        {

            Console.WriteLine("========= 0000 MasterApiDBOperation started");
            if (masterDataResponse != null && masterDataResponse.ErrorCode != null)
            {
                if (masterDataResponse.ErrorCode == "7200" && masterDataResponse.ErrorCode != "7000")
                {
                    MyTNBAccountManagement.GetInstance().SetMasterDataResponse(masterDataResponse);

                    if (masterDataResponse.Data.WebLinks != null)
                    {
                        foreach (Weblink web in masterDataResponse.Data.WebLinks)
                        {
                            int newRecord = WeblinkEntity.InsertOrReplace(web);
                        }
                    }

                    FeedbackCategoryEntity.RemoveActive();
                    if (masterDataResponse.Data.FeedbackCategorysV2 != null && masterDataResponse.Data.FeedbackCategorysV2.Count > 0)
                    {
                        foreach (FeedbackCategory cat in masterDataResponse.Data.FeedbackCategorysV2)
                        {
                            int newRecord = FeedbackCategoryEntity.InsertOrReplace(cat);
                        }
                    }
                    else
                    {
                        foreach (FeedbackCategory cat in masterDataResponse.Data.FeedbackCategorys)
                        {
                            int newRecord = FeedbackCategoryEntity.InsertOrReplace(cat);
                        }
                    }

                    int ctr = 0;
                    FeedbackStateEntity.RemoveActive();
                    foreach (FeedbackState state in masterDataResponse.Data.States)
                    {
                        bool isSelected = ctr == 0 ? true : false;
                        int newRecord = FeedbackStateEntity.InsertOrReplace(state, isSelected);
                        ctr++;
                    }


                    FeedbackTypeEntity.RemoveActive();
                    ctr = 0;
                    foreach (FeedbackType type in masterDataResponse.Data.FeedbackTypes)
                    {
                        bool isSelected = ctr == 0 ? true : false;
                        int newRecord = FeedbackTypeEntity.InsertOrReplace(type, isSelected);
                        Console.WriteLine(string.Format("FeedbackType Id = {0}", newRecord));
                        ctr++;
                    }


                    foreach (NotificationChannels notificationChannel in masterDataResponse.Data.NotificationTypeChannels)
                    {
                        int newRecord = NotificationChannelEntity.InsertOrReplace(notificationChannel);
                    }

                    foreach (NotificationTypes notificationTypes in masterDataResponse.Data.NotificationTypes)
                    {
                        int newRecord = NotificationTypesEntity.InsertOrReplace(notificationTypes);
                    }

                    LocationTypesEntity.InsertFristRecord();
                    foreach (LocationType loc in masterDataResponse.Data.LocationTypes)
                    {
                        int newRecord = LocationTypesEntity.InsertOrReplace(loc);
                    }

                    DownTimeEntity.RemoveActive();
                    foreach (DownTime cat in masterDataResponse.Data.Downtimes)
                    {
                        int newRecord = DownTimeEntity.InsertOrReplace(cat);
                    }

                    int appCurrentVersion = DeviceIdUtils.GetAppVersionCode();
                    if (UserEntity.IsCurrentlyActive())
                    {
                        int prevAppVersionCode = UserSessions.GetPrevAppVersionCode(preferences);
                        if (prevAppVersionCode > 0)
                        {
                            if (prevAppVersionCode < appCurrentVersion)
                            {
                                SMUsageHistoryEntity.RemoveAll();
                            }
                        }
                        else
                        {
                            SMUsageHistoryEntity.RemoveAll();
                        }
                    }
                    UserSessions.SetAppVersionCode(preferences, appCurrentVersion);
                }
            }

            Console.WriteLine("========= 0000 MasterApiDBOperation ended");

            return null;
        }
    }
}

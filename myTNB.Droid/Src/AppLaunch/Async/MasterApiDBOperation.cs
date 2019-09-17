﻿using Android.Content;
using Android.OS;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;
using System;
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
            if (masterDataResponse != null && masterDataResponse.Data != null)
            {
                if (masterDataResponse.Data.ErrorCode == "7200" && masterDataResponse.Data.ErrorCode != "7000")
                {
                    MyTNBAccountManagement.GetInstance().SetCurentMasterData(masterDataResponse);

                    foreach (Weblink web in masterDataResponse.Data.MasterData.WebLinks)
                    {
                        int newRecord = WeblinkEntity.InsertOrReplace(web);
                    }

                    FeedbackCategoryEntity.RemoveActive();
                    if (masterDataResponse.Data.MasterData.FeedbackCategorysV2 != null && masterDataResponse.Data.MasterData.FeedbackCategorysV2.Count > 0)
                    {
                        foreach (FeedbackCategory cat in masterDataResponse.Data.MasterData.FeedbackCategorysV2)
                        {
                            int newRecord = FeedbackCategoryEntity.InsertOrReplace(cat);
                        }
                    }
                    else
                    {
                        foreach (FeedbackCategory cat in masterDataResponse.Data.MasterData.FeedbackCategorys)
                        {
                            int newRecord = FeedbackCategoryEntity.InsertOrReplace(cat);
                        }
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
                    }

                    foreach (NotificationTypes notificationTypes in masterDataResponse.Data.MasterData.NotificationTypes)
                    {
                        int newRecord = NotificationTypesEntity.InsertOrReplace(notificationTypes);
                    }

                    LocationTypesEntity.InsertFristRecord();
                    foreach (LocationType loc in masterDataResponse.Data.MasterData.LocationTypes)
                    {
                        int newRecord = LocationTypesEntity.InsertOrReplace(loc);
                    }

                    DownTimeEntity.RemoveActive();
                    foreach (DownTime cat in masterDataResponse.Data.MasterData.Downtimes)
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

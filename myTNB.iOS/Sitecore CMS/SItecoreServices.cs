﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB.SQLite.SQLiteDataManager;

namespace myTNB.SitecoreCMS
{
    public sealed class SitecoreServices
    {
        private static readonly Lazy<SitecoreServices> lazy = new Lazy<SitecoreServices>(() => new SitecoreServices());
        public static SitecoreServices Instance { get { return lazy.Value; } }

        private async Task<NSData> GetImageFromURL(string urlString)
        {
            NSUrl url = NSUrl.FromString(urlString);
            NSData data = NSData.FromUrl(url, NSDataReadingOptions.Uncached, out NSError error);
            if (error != null) { data = null; }
            return data;
        }

        private void UpdateTimeStamp(string sitecoreTS, string key, ref bool needsUpdate)
        {
            NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
            string currentTS = sharedPreference.StringForKey(key);

            if (currentTS != null && currentTS.Equals(sitecoreTS))
            {
                needsUpdate = false;
            }
            else
            {
                sharedPreference.SetString(sitecoreTS, key);
                sharedPreference.Synchronize();
            }
        }

        public Task LoadSSMRWalkthrough()
        {
            return Task.Factory.StartNew(() =>
           {
               GetItemsService iService = new GetItemsService(TNBGlobal.OS
                   , DataManager.DataManager.SharedInstance.ImageSize
                   , TNBGlobal.SITECORE_URL
                   , TNBGlobal.DEFAULT_LANGUAGE);

               ApplySSMRTimeStampResponseModel timeStamp = iService.GetApplySSMRWalkthroughTimestampItem();
               bool needsUpdate = true;

               if (timeStamp == null || timeStamp.Data == null || timeStamp.Data.Count == 0
                    || string.IsNullOrEmpty(timeStamp.Data[0].Timestamp)
                    || string.IsNullOrWhiteSpace(timeStamp.Data[0].Timestamp))
               {
                   timeStamp = new ApplySSMRTimeStampResponseModel();
                   timeStamp.Data = new List<ApplySSMRTimeStamp> { new ApplySSMRTimeStamp { Timestamp = string.Empty } };
               }

               UpdateTimeStamp(timeStamp.Data[0].Timestamp, "SiteCoreApplySSMRWalkthroughTimeStamp", ref needsUpdate);

               if (needsUpdate)
               {
                   ApplySSMRResponseModel applySSMrWalkthroughItems = iService.GetApplySSMRWalkthroughItems();
                   if (applySSMrWalkthroughItems != null && applySSMrWalkthroughItems.Data != null && applySSMrWalkthroughItems.Data.Count > 0)
                   {
                       List<Task<NSData>> GetImagesTask = new List<Task<NSData>>();
                       for (int i = 0; i < applySSMrWalkthroughItems.Data.Count; i++)
                       {
                           ApplySSMRModel item = applySSMrWalkthroughItems.Data[i];
                           GetImagesTask.Add(GetImageFromURL(item.Image));
                       }

                       Task.WaitAll(GetImagesTask.ToArray());

                       for (int j = 0; j < GetImagesTask.Count; j++)
                       {
                           if (GetImagesTask[j] == null || GetImagesTask[j].Result == null) { continue; }
                           byte[] data = GetImagesTask[j].Result.ToByteArray();
                           applySSMrWalkthroughItems.Data[j].ImageByteArray = data;
                       }

                       ApplySSMRWalkthroughEntity wsManager = new ApplySSMRWalkthroughEntity();
                       wsManager.DeleteTable();
                       wsManager.CreateTable();
                       wsManager.InsertListOfItems(applySSMrWalkthroughItems.Data);
                   }
               }
           });
        }

        public Task LoadMeterReadSSMRWalkthrough()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS
                    , DataManager.DataManager.SharedInstance.ImageSize, TNBGlobal.SITECORE_URL, TNBGlobal.DEFAULT_LANGUAGE);
                MeterReadSSMRTimeStampResponseModel timeStamp = iService.GetMeterReadSSMRWalkthroughTimestampItem();

                bool needsUpdate = true;

                if (timeStamp == null || timeStamp.Data == null || timeStamp.Data.Count == 0
                  || string.IsNullOrEmpty(timeStamp.Data[0].Timestamp)
                  || string.IsNullOrWhiteSpace(timeStamp.Data[0].Timestamp))
                {
                    timeStamp = new MeterReadSSMRTimeStampResponseModel();
                    timeStamp.Data = new List<MeterReadSSMRTimeStamp> { new MeterReadSSMRTimeStamp { Timestamp = string.Empty } };
                }

                UpdateTimeStamp(timeStamp.Data[0].Timestamp, "SiteCoreMeterReadSSMRWalkthroughTimeStamp", ref needsUpdate);

                if (needsUpdate)
                {
                    MeterReadSSMRResponseModel meterReadSSMrWalkthroughItems = iService.GetMeterReadSSMRWalkthroughItems();
                    if (meterReadSSMrWalkthroughItems != null && meterReadSSMrWalkthroughItems.Data != null && meterReadSSMrWalkthroughItems.Data.Count > 0)
                    {
                        List<Task<NSData>> GetImagesTask = new List<Task<NSData>>();
                        for (int i = 0; i < meterReadSSMrWalkthroughItems.Data.Count; i++)
                        {
                            MeterReadSSMRModel item = meterReadSSMrWalkthroughItems.Data[i];
                            GetImagesTask.Add(GetImageFromURL(item.Image));
                        }

                        Task.WaitAll(GetImagesTask.ToArray());

                        for (int j = 0; j < GetImagesTask.Count; j++)
                        {
                            if (GetImagesTask[j] == null || GetImagesTask[j].Result == null) { continue; }
                            byte[] data = GetImagesTask[j].Result.ToByteArray();
                            meterReadSSMrWalkthroughItems.Data[j].ImageByteArray = data;
                        }

                        MeterReadSSMRWalkthroughEntity wsManager = new MeterReadSSMRWalkthroughEntity();
                        wsManager.DeleteTable();
                        wsManager.CreateTable();
                        wsManager.InsertListOfItems(meterReadSSMrWalkthroughItems.Data);
                    }
                }
            });
        }

        public Task LoadMeterReadSSMRWalkthroughV2()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS
                    , DataManager.DataManager.SharedInstance.ImageSize, TNBGlobal.SITECORE_URL, TNBGlobal.DEFAULT_LANGUAGE);
                MeterReadSSMRTimeStampResponseModel timeStamp = iService.GetMeterReadSSMRWalkthroughTimestampItemV2();

                bool needsUpdate = true;

                if (timeStamp == null || timeStamp.Data == null || timeStamp.Data.Count == 0
                  || string.IsNullOrEmpty(timeStamp.Data[0].Timestamp)
                  || string.IsNullOrWhiteSpace(timeStamp.Data[0].Timestamp))
                {
                    timeStamp = new MeterReadSSMRTimeStampResponseModel();
                    timeStamp.Data = new List<MeterReadSSMRTimeStamp> { new MeterReadSSMRTimeStamp { Timestamp = string.Empty } };
                }

                UpdateTimeStamp(timeStamp.Data[0].Timestamp, "SiteCoreMeterReadSSMRWalkthroughTimeStampV2", ref needsUpdate);

                if (needsUpdate)
                {
                    MeterReadSSMRResponseModel meterReadSSMrWalkthroughItems = iService.GetMeterReadSSMRWalkthroughItemsV2();
                    if (meterReadSSMrWalkthroughItems != null && meterReadSSMrWalkthroughItems.Data != null && meterReadSSMrWalkthroughItems.Data.Count > 0)
                    {
                        List<Task<NSData>> GetImagesTask = new List<Task<NSData>>();
                        for (int i = 0; i < meterReadSSMrWalkthroughItems.Data.Count; i++)
                        {
                            MeterReadSSMRModel item = meterReadSSMrWalkthroughItems.Data[i];
                            GetImagesTask.Add(GetImageFromURL(item.Image));
                        }

                        Task.WaitAll(GetImagesTask.ToArray());

                        for (int j = 0; j < GetImagesTask.Count; j++)
                        {
                            if (GetImagesTask[j] == null || GetImagesTask[j].Result == null) { continue; }
                            byte[] data = GetImagesTask[j].Result.ToByteArray();
                            meterReadSSMrWalkthroughItems.Data[j].ImageByteArray = data;
                        }

                        MeterReadSSMRWalkthroughEntityV2 wsManager = new MeterReadSSMRWalkthroughEntityV2();
                        wsManager.DeleteTable();
                        wsManager.CreateTable();
                        wsManager.InsertListOfItems(meterReadSSMrWalkthroughItems.Data);
                    }
                }
            });
        }

        public Task LoadBillDetailsTooltip()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS
                    , DataManager.DataManager.SharedInstance.ImageSize, TNBGlobal.SITECORE_URL, TNBGlobal.DEFAULT_LANGUAGE);
                BillDetailsTooltipTimeStampResponseModel timeStamp = iService.GetBillDetailsTooltipTimestampItem();

                bool needsUpdate = true;

                if (timeStamp == null || timeStamp.Data == null || timeStamp.Data.Count == 0
                  || string.IsNullOrEmpty(timeStamp.Data[0].Timestamp)
                  || string.IsNullOrWhiteSpace(timeStamp.Data[0].Timestamp))
                {
                    timeStamp = new BillDetailsTooltipTimeStampResponseModel();
                    timeStamp.Data = new List<BillsTooltipTimeStamp> { new BillsTooltipTimeStamp { Timestamp = string.Empty } };
                }

                UpdateTimeStamp(timeStamp.Data[0].Timestamp, "BillDetailsTooltipTimeStamp", ref needsUpdate);

                if (needsUpdate)
                {
                    BillDetailsTooltipResponseModel tooltipsItems = iService.GetBillDetailsTooltipItem();
                    if (tooltipsItems != null && tooltipsItems.Data != null && tooltipsItems.Data.Count > 0)
                    {
                        List<Task<NSData>> GetImagesTask = new List<Task<NSData>>();
                        for (int i = 0; i < tooltipsItems.Data.Count; i++)
                        {
                            BillsTooltipModelEntity item = tooltipsItems.Data[i];
                            GetImagesTask.Add(GetImageFromURL(item.Image));
                        }

                        Task.WaitAll(GetImagesTask.ToArray());

                        for (int j = 0; j < GetImagesTask.Count; j++)
                        {
                            if (GetImagesTask[j] == null || GetImagesTask[j].Result == null) { continue; }
                            byte[] data = GetImagesTask[j].Result.ToByteArray();
                            tooltipsItems.Data[j].ImageByteArray = data;
                        }

                        BillDetailsTooltipEntity wsManager = new BillDetailsTooltipEntity();
                        wsManager.DeleteTable();
                        wsManager.CreateTable();
                        wsManager.InsertListOfItems(tooltipsItems.Data);
                    }
                }
            });
        }

        public Task LoadEnergyTips()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS
                    , DataManager.DataManager.SharedInstance.ImageSize, TNBGlobal.SITECORE_URL, TNBGlobal.DEFAULT_LANGUAGE);
                EnergyTipsTimeStampResponseModel timeStamp = iService.GetEnergyTipsTimestampItem();

                bool needsUpdate = true;

                if (timeStamp == null || timeStamp.Data == null || timeStamp.Data.Count == 0
                    || string.IsNullOrEmpty(timeStamp.Data[0].Timestamp)
                    || string.IsNullOrWhiteSpace(timeStamp.Data[0].Timestamp))
                {
                    timeStamp = new EnergyTipsTimeStampResponseModel();
                    timeStamp.Data = new List<EnergyTipsTimeStamp> { new EnergyTipsTimeStamp { Timestamp = string.Empty } };
                }

                UpdateTimeStamp(timeStamp.Data[0].Timestamp, "SiteCoreEnergyTipsTimeStamp", ref needsUpdate);

                if (needsUpdate)
                {
                    EnergyTipsResponseModel energyTipsItems = iService.GetEnergyTipsItem();
                    if (energyTipsItems != null && energyTipsItems.Data != null && energyTipsItems.Data.Count > 0)
                    {
                        List<Task<NSData>> GetImagesTask = new List<Task<NSData>>();
                        for (int i = 0; i < energyTipsItems.Data.Count; i++)
                        {
                            TipsModel item = energyTipsItems.Data[i];
                            GetImagesTask.Add(GetImageFromURL(item.Image));
                        }

                        Task.WaitAll(GetImagesTask.ToArray());

                        for (int j = 0; j < GetImagesTask.Count; j++)
                        {
                            if (GetImagesTask[j] == null || GetImagesTask[j].Result == null) { continue; }
                            byte[] data = GetImagesTask[j].Result.ToByteArray();
                            energyTipsItems.Data[j].ImageByteArray = data;
                        }

                        EnergyTipsEntity wsManager = new EnergyTipsEntity();
                        wsManager.DeleteTable();
                        wsManager.CreateTable();
                        wsManager.InsertListOfItems(energyTipsItems.Data);
                    }
                }
            });
        }
    }
}
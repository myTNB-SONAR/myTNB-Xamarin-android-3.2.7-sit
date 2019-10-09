using System;
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
            NSData data = NSData.FromUrl(url, NSDataReadingOptions.MappedAlways, out NSError error);
            return data;
        }

        private void UpdateTimeStamp(string sitecoreTS, string key, ref bool needsUpdate)
        {
            NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
            string currentTS = sharedPreference.StringForKey(key);

            if (currentTS.Equals(sitecoreTS))
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
               needsUpdate = true;
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
                       var test = wsManager.GetAllItems();
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
                needsUpdate = true;
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
                        var test = wsManager.GetAllItems();
                    }
                }
            });
        }

    }
}
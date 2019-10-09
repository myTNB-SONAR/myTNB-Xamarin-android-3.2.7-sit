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

        public void OnExecute()
        {

        }

        public async Task<NSData> GetImageFromURL(string urlString)
        {
            NSUrl url = NSUrl.FromString(urlString);
            NSData data = NSData.FromUrl(url, NSDataReadingOptions.MappedAlways, out NSError error);
            return data;
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
               if (timeStamp != null && timeStamp.Data != null && timeStamp.Data.Count > 0 && timeStamp.Data[0] != null
                   && !string.IsNullOrEmpty(timeStamp.Data[0].Timestamp))
               {
                   NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                   string currentTS = sharedPreference.StringForKey("SiteCoreApplySSMRWalkthroughTimeStamp");
                   if (string.IsNullOrEmpty(currentTS) || string.IsNullOrWhiteSpace(currentTS))
                   {
                       sharedPreference.SetString(timeStamp.Data[0].Timestamp, "SiteCoreApplySSMRWalkthroughTimeStamp");
                       sharedPreference.Synchronize();
                   }
                   else
                   {
                       if (currentTS.Equals(timeStamp.Data[0].Timestamp))
                       {
                           needsUpdate = false;
                       }
                       else
                       {
                           sharedPreference.SetString(timeStamp.Data[0].Timestamp, "SiteCoreApplySSMRWalkthroughTimeStamp");
                           sharedPreference.Synchronize();
                       }
                   }
               }
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
                           if (GetImagesTask[j] == null) { continue; }
                           applySSMrWalkthroughItems.Data[j].NSDataImage = GetImagesTask[j].Result;
                       }

                       ApplySSMRWalkthroughEntity wsManager = new ApplySSMRWalkthroughEntity();
                       wsManager.DeleteTable();
                       wsManager.CreateTable();
                       wsManager.InsertListOfItems(applySSMrWalkthroughItems.Data);
                       var t = wsManager.GetAllItems();
                   }
               }
           });
        }
    }
}
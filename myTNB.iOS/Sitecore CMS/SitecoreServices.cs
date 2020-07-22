using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Foundation;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB.SQLite.SQLiteDataManager;
using Newtonsoft.Json;

namespace myTNB.SitecoreCMS
{
    public sealed class SitecoreServices
    {
        private static readonly Lazy<SitecoreServices> lazy = new Lazy<SitecoreServices>(() => new SitecoreServices());
        public static SitecoreServices Instance { get { return lazy.Value; } }

        private static bool _isForcedUpdate;

        public bool SplashHasNewTimestamp { get; set; }
        public bool NeedHelpTimeStampChanged { set; get; }
        public string NeedHelpTimeStamp { set; get; }
        public static bool IsForcedUpdate { get { return _isForcedUpdate; } }
        private bool isWhatsNewUpdating = false;

        public async Task OnExecuteSitecoreCall(bool isforcedUpdate = false)
        {
            _isForcedUpdate = isforcedUpdate;
            List<Task> taskList = new List<Task>{
                LoadMeterReadSSMRWalkthrough()
                , LoadMeterReadSSMRWalkthroughV2()
                , LoadBillDetailsTooltip()
                //, LoadSSMRWalkthrough()
                , LoadEppInfoTooltip() //Created by Syahmi ICS 05052020

                //, LoadWhereAccountInfoTooltip() //Tooltip for where O
                //, LoadRegisteredInfoTooltip()
                //, LoadOwnerConsentInfoTooltip()
                //, LoadIdentificationInfoTooltip()
                //, LoadProofConsentInfoTooltip()
             };
            if (_isForcedUpdate)
            {
                taskList.Add(LoadLanguage());
                WhatsNewCache.ClearImages();
                WhatsNewDetailCache.ClearImages();
                WhatsNewDetailDescriptionCache.ClearImages();
                WhatsNewPopupCache.ClearImages();
                RewardsCache.ClearImages();
                ClearTimeStamps();
            }
            else
            {
                taskList.Add(LoadCountry());
            }
            if (!AppLaunchMasterCache.IsEnergyTipsDisabled)
            {
                taskList.Add(LoadEnergyTips());
            }
            Task.WaitAll(taskList.ToArray());
            _isForcedUpdate = false;
        }

        private void ClearTimeStamps()
        {
            UpdateSharedPreference(string.Empty, "SiteCoreTimeStamp");
            UpdateSharedPreference(string.Empty, "SiteCoreFAQTimeStamp");
            UpdateSharedPreference(string.Empty, "SiteCoreHelpTimeStamp");
        }

        private async Task<NSData> GetImageFromURL(string urlString)
        {
            NSUrl url = NSUrl.FromString(urlString);
            NSData data = NSData.FromUrl(url, NSDataReadingOptions.Uncached, out NSError error);
            if (error != null) { data = null; }
            return data;
        }

        private string GetDataFromFile(string url)
        {
            string content = string.Empty;
            try
            {
                WebRequest webRequest = WebRequest.Create(url);
                using (WebResponse response = webRequest.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    content = reader.ReadToEnd();
                }
                //Debug.WriteLine("Content: " + content);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetDataFromFile: " + e.Message);
            }
            return content;
        }

        private void UpdateTimeStamp(string sitecoreTS, string key, ref bool needsUpdate)
        {
            NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
            string currentTS = sharedPreference.StringForKey(key);

            if (_isForcedUpdate || !currentTS.IsValid())
            {
                needsUpdate = true;
            }
            else
            {
                needsUpdate = !currentTS.Equals(sitecoreTS);
            }
        }

        private void UpdateSharedPreference(string value, string key)
        {
            try
            {
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                sharedPreference.SetString(value, key);
                sharedPreference.Synchronize();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in ClearSharedPreference: " + e.Message);
            }
        }

        //Created by Syahmi ICS 05052020
        private Task LoadEppInfoTooltip()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS
                    , DataManager.DataManager.SharedInstance.ImageSize, TNBGlobal.SITECORE_URL, TNBGlobal.APP_LANGUAGE);
                EppInfoTooltipTimeStampResponseModel timeStamp = iService.GetEppInfoTooltipTimestampItem();

                bool needsUpdate = true;

                if (timeStamp == null || timeStamp.Data == null || timeStamp.Data.Count == 0
                  || string.IsNullOrEmpty(timeStamp.Data[0].Timestamp)
                  || string.IsNullOrWhiteSpace(timeStamp.Data[0].Timestamp))
                {
                    timeStamp = new EppInfoTooltipTimeStampResponseModel();
                    timeStamp.Data = new List<EppTooltipTimeStamp> { new EppTooltipTimeStamp { Timestamp = string.Empty } };
                }

                UpdateTimeStamp(timeStamp.Data[0].Timestamp, "EppInfoTooltipTimeStamp", ref needsUpdate);

                if (needsUpdate)
                {
                    EppInfoTooltipResponseModel EpptooltipsItems = iService.GetEppInfoTooltipItem();
                    if (EpptooltipsItems != null && EpptooltipsItems.Data != null && EpptooltipsItems.Data.Count > 0)
                    {
                        List<Task<NSData>> GetImagesTask = new List<Task<NSData>>();
                        for (int i = 0; i < EpptooltipsItems.Data.Count; i++)
                        {
                            EppTooltipModelEntity item = EpptooltipsItems.Data[i];
                            GetImagesTask.Add(GetImageFromURL(item.Image));
                        }

                        Task.WaitAll(GetImagesTask.ToArray());

                        for (int j = 0; j < GetImagesTask.Count; j++)
                        {
                            if (GetImagesTask[j] == null || GetImagesTask[j].Result == null) { continue; }
                            byte[] data = GetImagesTask[j].Result.ToByteArray();
                            EpptooltipsItems.Data[j].ImageByteArray = data;
                        }

                        EppInfoTooltipEntity wsManager = new EppInfoTooltipEntity();
                        wsManager.DeleteTable();
                        wsManager.CreateTable();
                        wsManager.InsertListOfItems(EpptooltipsItems.Data);
                        UpdateSharedPreference(timeStamp.Data[0].Timestamp, "EppInfoTooltipTimeStamp");
                        Debug.WriteLine("LoadEppInfoTooltip Done");
                    }
                }
            });
        }

        /*private Task LoadSSMRWalkthrough()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS
                    , DataManager.DataManager.SharedInstance.ImageSize
                    , TNBGlobal.SITECORE_URL
                    , TNBGlobal.APP_LANGUAGE);

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
                        UpdateSharedPreference(timeStamp.Data[0].Timestamp, "SiteCoreApplySSMRWalkthroughTimeStamp");
                        Debug.WriteLine("LoadSSMRWalkthrough Done");
                    }
                }
            });
        }*/

        private Task LoadMeterReadSSMRWalkthrough()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS
                    , DataManager.DataManager.SharedInstance.ImageSize, TNBGlobal.SITECORE_URL, TNBGlobal.APP_LANGUAGE);
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
                        UpdateSharedPreference(timeStamp.Data[0].Timestamp, "SiteCoreMeterReadSSMRWalkthroughTimeStamp");
                        Debug.WriteLine("LoadMeterReadSSMRWalkthrough Done");
                    }
                }
            });
        }

        private Task LoadMeterReadSSMRWalkthroughV2()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS
                    , DataManager.DataManager.SharedInstance.ImageSize, TNBGlobal.SITECORE_URL, TNBGlobal.APP_LANGUAGE);
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
                        UpdateSharedPreference(timeStamp.Data[0].Timestamp, "SiteCoreMeterReadSSMRWalkthroughTimeStampV2");
                        Debug.WriteLine("LoadMeterReadSSMRWalkthroughV2 Done");
                    }
                }
            });
        }

        private Task LoadBillDetailsTooltip()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS
                    , DataManager.DataManager.SharedInstance.ImageSize, TNBGlobal.SITECORE_URL, TNBGlobal.APP_LANGUAGE);
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
                        UpdateSharedPreference(timeStamp.Data[0].Timestamp, "BillDetailsTooltipTimeStamp");
                        Debug.WriteLine("LoadBillDetailsTooltip Done");
                    }
                }
            });
        }

        private Task LoadEnergyTips()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS
                    , DataManager.DataManager.SharedInstance.ImageSize, TNBGlobal.SITECORE_URL, TNBGlobal.APP_LANGUAGE);
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
                        UpdateSharedPreference(timeStamp.Data[0].Timestamp, "SiteCoreEnergyTipsTimeStamp");
                        Debug.WriteLine("LoadEnergyTips Done");
                    }
                }
            });
        }

        public Task LoadTermsAndCondition()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS, DataManager.DataManager.SharedInstance.ImageSize
                    , TNBGlobal.SITECORE_URL, TNBGlobal.APP_LANGUAGE);
                TimestampResponseModel timeStamp = iService.GetTimestampItemV2();

                bool needsUpdate = false;

                if (timeStamp == null || timeStamp.Data == null || timeStamp.Data.Count == 0
                    || string.IsNullOrEmpty(timeStamp.Data[0].Timestamp)
                    || string.IsNullOrWhiteSpace(timeStamp.Data[0].Timestamp))
                {
                    timeStamp = new TimestampResponseModel();
                    timeStamp.Data = new List<TimestampModel> { new TimestampModel { Timestamp = string.Empty } };
                }

                UpdateTimeStamp(timeStamp.Data[0].Timestamp, "SiteCoreTimeStamp", ref needsUpdate);

                if (needsUpdate)
                {
                    TermsAndConditionResponseModel tncResponse = iService.GetFullRTEPagesItems();
                    if (tncResponse != null && tncResponse.Status.Equals("Success")
                        && tncResponse.Data != null && tncResponse.Data.Count > 0)
                    {
                        TermsAndConditionEntity tncEntity = new TermsAndConditionEntity();
                        tncEntity.DeleteTable();
                        tncEntity.CreateTable();
                        tncEntity.InsertListOfItems(tncResponse.Data);
                        UpdateSharedPreference(timeStamp.Data[0].Timestamp, "SiteCoreTimeStamp");
                        Debug.WriteLine("LoadTermsAndCondition Done");
                    }
                }
            });
        }

        public Task LoadFAQs()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS, DataManager.DataManager.SharedInstance.ImageSize
                    , TNBGlobal.SITECORE_URL, TNBGlobal.APP_LANGUAGE);
                FAQTimestampResponseModel timeStamp = iService.GetFAQsTimestampItem();

                bool needsUpdate = false;

                if (timeStamp == null || timeStamp.Data == null || timeStamp.Data.Count == 0
                    || string.IsNullOrEmpty(timeStamp.Data[0].Timestamp)
                    || string.IsNullOrWhiteSpace(timeStamp.Data[0].Timestamp))
                {
                    timeStamp = new FAQTimestampResponseModel();
                    timeStamp.Data = new List<FAQsParentModel> { new FAQsParentModel { Timestamp = string.Empty } };
                }

                UpdateTimeStamp(timeStamp.Data[0].Timestamp, "SiteCoreFAQTimeStamp", ref needsUpdate);

                if (needsUpdate)
                {
                    FAQEntity wsManager = new FAQEntity();
                    wsManager.DeleteTable();
                    wsManager.CreateTable();
                    FAQsResponseModel faqResponse = iService.GetFAQsItems();
                    if (faqResponse != null && faqResponse.Data != null && faqResponse.Data.Count > 0)
                    {
                        for (int i = faqResponse.Data.Count - 1; i > -1; i--)
                        {
                            if (!faqResponse.Data[i].Question.IsValid() && !faqResponse.Data[i].Answer.IsValid())
                            {
                                faqResponse.Data.RemoveAt(i);
                            }
                        }
                        wsManager.InsertListOfItems(faqResponse.Data);
                        UpdateSharedPreference(timeStamp.Data[0].Timestamp, "SiteCoreFAQTimeStamp");
                        Debug.WriteLine("LoadFAQs Done");
                    }
                }
            });
        }

        public Task LoadLanguage()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS
                    , DataManager.DataManager.SharedInstance.ImageSize
                    , TNBGlobal.SITECORE_URL
                    , TNBGlobal.APP_LANGUAGE);

                LanguageTimeStampResponseModel timeStamp = iService.GetLanguageTimestampItem();
                bool needsUpdate = true;

                if (timeStamp == null || timeStamp.Data == null || timeStamp.Data.Count == 0
                     || string.IsNullOrEmpty(timeStamp.Data[0].Timestamp)
                     || string.IsNullOrWhiteSpace(timeStamp.Data[0].Timestamp))
                {
                    timeStamp = new LanguageTimeStampResponseModel();
                    timeStamp.Data = new List<LanguageTimeStamp> { new LanguageTimeStamp { Timestamp = string.Empty } };
                }

                UpdateTimeStamp(timeStamp.Data[0].Timestamp, "LanguageTimeStamp", ref needsUpdate);

                if (timeStamp.Status != null && timeStamp.Status == "Success" && (needsUpdate || !LanguageUtility.HasSavedContent))
                {
                    LanguageResponseModel languageItems = iService.GetLanguageItems();
                    if (languageItems != null
                        && languageItems.Data != null
                        && languageItems.Data.Count > 0
                        && languageItems.Data[0] != null)
                    {
                        LanguageModel item = languageItems.Data[0];
                        if (!string.IsNullOrEmpty(item.LanguageFile) && !string.IsNullOrWhiteSpace(item.LanguageFile))
                        {
                            string content = GetDataFromFile(item.LanguageFile);
                            LanguageManager.Instance.SetLanguage(content ?? string.Empty);
                            LanguageUtility.SetLanguageGlobals();
                            LanguageUtility.SaveLanguageContent(content);
                            if (_isForcedUpdate)
                            {
                                LanguageUtility.SetLanguage(TNBGlobal.APP_LANGUAGE);
                            }
                            UpdateSharedPreference(timeStamp.Data[0].Timestamp, "LanguageTimeStamp");
                            Debug.WriteLine("LoadLanguage Done");
                        }
                    }
                }
                else
                {
                    string content = LanguageUtility.LanguageContent;
                    if (string.IsNullOrEmpty(content) || string.IsNullOrWhiteSpace(content))
                    {
                        NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                        content = sharedPreference.StringForKey("LanguageJSON");
                        if (string.IsNullOrEmpty(content) || string.IsNullOrWhiteSpace(content))
                        {
                            LanguageManager.Instance.SetLanguage(LanguageManager.Source.FILE
                            , TNBGlobal.APP_LANGUAGE == "EN" ? LanguageManager.Language.EN : LanguageManager.Language.MS);
                        }
                        else
                        {
                            LanguageManager.Instance.SetLanguage(content ?? string.Empty);
                        }
                    }
                    else
                    {
                        LanguageManager.Instance.SetLanguage(content ?? string.Empty);
                        UpdateSharedPreference(timeStamp.Data[0].Timestamp, "LanguageJSON");
                    }
                    LanguageUtility.SetLanguageGlobals();
                    LanguageUtility.SaveLanguageContent(content);
                    if (_isForcedUpdate)
                    {
                        LanguageUtility.SetLanguage(TNBGlobal.APP_LANGUAGE);
                    }
                }
            });
        }

        public Task LoadRewards()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS
                    , DataManager.DataManager.SharedInstance.ImageSize
                    , TNBGlobal.SITECORE_URL
                    , TNBGlobal.APP_LANGUAGE);

                RewardsTimestampResponseModel timeStamp = iService.GetRewardsTimestampItem();
                bool needsUpdate = true;

                if (timeStamp == null || timeStamp.Data == null || timeStamp.Data.Count == 0
                     || string.IsNullOrEmpty(timeStamp.Data[0].Timestamp)
                     || string.IsNullOrWhiteSpace(timeStamp.Data[0].Timestamp))
                {
                    timeStamp = new RewardsTimestampResponseModel();
                    timeStamp.Data = new List<RewardsTimestamp> { new RewardsTimestamp { Timestamp = string.Empty } };
                }

                UpdateTimeStamp(timeStamp.Data[0].Timestamp, "SiteCoreRewardsTimeStamp", ref needsUpdate);

                if (needsUpdate)
                {
                    RewardsEntity rewardsEntity = new RewardsEntity();
                    rewardsEntity.DeleteTable();
                    rewardsEntity.CreateTable();

                    RewardsResponseModel rewardsResponse = iService.GetRewardsItems();
                    if (rewardsResponse != null)
                    {
                        RewardsCache.RewardIsAvailable = true;
                        if (rewardsResponse.Data != null && rewardsResponse.Data.Count > 0)
                        {
                            List<RewardsModel> rewardsData = new List<RewardsModel>();
                            List<RewardsCategoryModel> categoryList = new List<RewardsCategoryModel>(rewardsResponse.Data);
                            foreach (var category in categoryList)
                            {
                                List<RewardsModel> rewardsList = new List<RewardsModel>(category.Rewards);
                                if (rewardsList.Count > 0)
                                {
                                    foreach (var reward in rewardsList)
                                    {
                                        if (!RewardsServices.RewardHasExpired(reward) && !RewardsServices.RewardHasUsedAfterXDays(reward, RewardsConstants.Int_UsedRewardLimitInDays))
                                        {
                                            reward.CategoryID = category.ID;
                                            reward.CategoryName = category.CategoryName;
                                            rewardsData.Add(reward);
                                        }
                                    }
                                }
                            }
                            rewardsEntity.InsertListOfItems(rewardsData);
                            UpdateSharedPreference(timeStamp.Data[0].Timestamp, "SiteCoreRewardsTimeStamp");
                        }
                    }
                    else
                    {
                        RewardsCache.RewardIsAvailable = false;
                    }
                }
                else
                {
                    RewardsCache.RewardIsAvailable = true;
                }
            });
        }

        public async Task<bool> WhatsNewHasUpdates()
        {
            bool needsUpdate = true;
            await Task.Run(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS
                    , DataManager.DataManager.SharedInstance.ImageSize
                    , TNBGlobal.SITECORE_URL
                    , TNBGlobal.APP_LANGUAGE);

                WhatsNewTimestampResponseModel timeStamp = iService.GetWhatsNewTimestampItem();

                if (timeStamp == null || timeStamp.Data == null || timeStamp.Data.Count == 0
                     || string.IsNullOrEmpty(timeStamp.Data[0].Timestamp)
                     || string.IsNullOrWhiteSpace(timeStamp.Data[0].Timestamp))
                {
                    timeStamp = new WhatsNewTimestampResponseModel();
                    timeStamp.Data = new List<WhatsNewTimestamp> { new WhatsNewTimestamp { Timestamp = string.Empty } };
                }

                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                string currentTS = sharedPreference.StringForKey("SiteCoreWhatsNewTimeStamp");

                if (currentTS != null && currentTS.Equals(timeStamp.Data[0].Timestamp))
                {
                    needsUpdate = false;
                }
            });

            return needsUpdate;
        }

        public bool WhatsNewHasTimeStamp()
        {
            bool hasTimeStamp = false;
            NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
            string currentTS = sharedPreference.StringForKey("SiteCoreWhatsNewTimeStamp");

            if (!string.IsNullOrEmpty(currentTS) && !isWhatsNewUpdating)
            {
                hasTimeStamp = true;
            }

            return hasTimeStamp;
        }

        public Task LoadWhatsNew(bool forceUpdate = false)
        {
            isWhatsNewUpdating = true;
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS
                    , DataManager.DataManager.SharedInstance.ImageSize
                    , TNBGlobal.SITECORE_URL
                    , TNBGlobal.APP_LANGUAGE);

                WhatsNewTimestampResponseModel timeStamp = iService.GetWhatsNewTimestampItem();
                bool needsUpdate = true;

                if (timeStamp == null || timeStamp.Data == null || timeStamp.Data.Count == 0
                     || string.IsNullOrEmpty(timeStamp.Data[0].Timestamp)
                     || string.IsNullOrWhiteSpace(timeStamp.Data[0].Timestamp))
                {
                    timeStamp = new WhatsNewTimestampResponseModel();
                    timeStamp.Data = new List<WhatsNewTimestamp> { new WhatsNewTimestamp { Timestamp = string.Empty } };
                }

                UpdateTimeStamp(timeStamp.Data[0].Timestamp, "SiteCoreWhatsNewTimeStamp", ref needsUpdate);

                if (needsUpdate || forceUpdate)
                {
                    WhatsNewResponseModel whatsNewResponse = iService.GetWhatsNewItems();
                    if (whatsNewResponse != null)
                    {
                        WhatsNewCache.IsSitecoreRefresh = whatsNewResponse.Status == "Failed" || whatsNewResponse.Status == null;

                        if (!WhatsNewCache.IsSitecoreRefresh)
                        {
                            WhatsNewEntity whatsNewEntity = new WhatsNewEntity();
                            whatsNewEntity.DeleteTable();
                            whatsNewEntity.CreateTable();

                            WhatsNewCache.WhatsNewIsAvailable = true;
                            if (whatsNewResponse.Data != null && whatsNewResponse.Data.Count > 0)
                            {
                                List<WhatsNewModel> whatsNewData = new List<WhatsNewModel>();
                                List<WhatsNewCategoryModel> categoryList = new List<WhatsNewCategoryModel>(whatsNewResponse.Data);
                                foreach (var category in categoryList)
                                {
                                    List<WhatsNewModel> whatsNewList = new List<WhatsNewModel>(category.WhatsNewItems);
                                    if (whatsNewList.Count > 0)
                                    {
                                        foreach (var whatsNew in whatsNewList)
                                        {
                                            if (!WhatsNewServices.WhatsNewHasExpired(whatsNew))
                                            {
                                                whatsNew.CategoryID = category.ID;
                                                whatsNew.CategoryName = category.CategoryName;
                                                whatsNew.IsRead = WhatsNewServices.GetIsRead(whatsNew.ID);
                                                whatsNew.ShowDateForDay = WhatsNewServices.GetWhatNewModelShowDate(whatsNew.ID);
                                                whatsNew.ShowCountForDay = WhatsNewServices.GetWhatNewModelShowCount(whatsNew.ID);
                                                whatsNew.SkipShowOnAppLaunch = WhatsNewServices.GetIsSkipAppLaunch(whatsNew.ID);
                                                whatsNewData.Add(whatsNew);
                                            }
                                        }
                                    }
                                }
                                whatsNewEntity.InsertListOfItems(whatsNewData);
                                if (!string.IsNullOrEmpty(timeStamp.Data[0].Timestamp))
                                {
                                    UpdateSharedPreference(timeStamp.Data[0].Timestamp, "SiteCoreWhatsNewTimeStamp");
                                }
                            }
                        }
                        else
                        {
                            WhatsNewCache.WhatsNewIsAvailable = false;
                        }
                    }
                    else
                    {
                        WhatsNewCache.IsSitecoreRefresh = false;
                        WhatsNewCache.WhatsNewIsAvailable = false;
                    }
                }
                else
                {
                    WhatsNewCache.IsSitecoreRefresh = false;
                    WhatsNewCache.WhatsNewIsAvailable = true;
                }

                isWhatsNewUpdating = false;
                Debug.WriteLine("LoadWhatsNew Done");
            });
        }

        public Task LoadNeedHelpTimeStamp()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS, DataManager.DataManager.SharedInstance.ImageSize
                    , TNBGlobal.SITECORE_URL, TNBGlobal.APP_LANGUAGE);
                HelpTimeStampResponseModel timeStamp = iService.GetHelpTimestampItem();

                bool needsUpdate = false;

                if (timeStamp == null || timeStamp.Data == null || timeStamp.Data.Count == 0
                    || string.IsNullOrEmpty(timeStamp.Data[0].Timestamp)
                    || string.IsNullOrWhiteSpace(timeStamp.Data[0].Timestamp))
                {
                    timeStamp = new HelpTimeStampResponseModel();
                    timeStamp.Data = new List<HelpTimeStamp> { new HelpTimeStamp { Timestamp = string.Empty, ShowNeedHelp = false } };
                }

                UpdateTimeStamp(timeStamp.Data[0].Timestamp, "SiteCoreHelpTimeStamp", ref needsUpdate);
                NeedHelpTimeStamp = timeStamp.Data[0].Timestamp;
                NeedHelpTimeStampChanged = needsUpdate;
                ShowNeedHelp = timeStamp.Data[0].ShowNeedHelp;
                Debug.WriteLine("LoadNeedHelpTimeStamp Done");
            });
        }

        public Task LoadNeedHelp()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS, DataManager.DataManager.SharedInstance.ImageSize
                    , TNBGlobal.SITECORE_URL, TNBGlobal.APP_LANGUAGE);

                HelpResponseModel needHelpResponse = iService.GetHelpItems();
                if (needHelpResponse != null && needHelpResponse.Data != null && needHelpResponse.Data.Count > 0)
                {
                    HelpEntity wsManager = new HelpEntity();
                    wsManager.DeleteTable();
                    wsManager.CreateTable();
                    wsManager.InsertListOfItems(needHelpResponse.Data);
                    UpdateSharedPreference(NeedHelpTimeStamp, "SiteCoreHelpTimeStamp");
                    Debug.WriteLine("LoadNeedHelp Done");
                }
            });
        }

        public bool ShowNeedHelp
        {
            set
            {
                try
                {
                    NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                    sharedPreference.SetBool(value, "ShowNeedHelp");
                    sharedPreference.Synchronize();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                }
            }
            get
            {
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                return sharedPreference.BoolForKey("ShowNeedHelp");
            }
        }

        public async Task<bool> LoadDynamicSplash()
        {
            bool isDone = false;
            await Task.Run(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS, DataManager.DataManager.SharedInstance.ImageSize
                    , TNBGlobal.SITECORE_URL, TNBGlobal.APP_LANGUAGE);
                AppLaunchImageTimestampResponseModel timeStamp = iService.GetAppLaunchImageTimestampItem();

                bool needsUpdate = false;

                if (timeStamp == null || timeStamp.Data == null || timeStamp.Data.Count == 0
                    || string.IsNullOrEmpty(timeStamp.Data[0].Timestamp)
                    || string.IsNullOrWhiteSpace(timeStamp.Data[0].Timestamp))
                {
                    timeStamp = new AppLaunchImageTimestampResponseModel();
                    timeStamp.Data = new List<AppLaunchImageTimestamp> { new AppLaunchImageTimestamp { Timestamp = string.Empty } };
                }

                UpdateTimeStamp(timeStamp.Data[0].Timestamp, "AppLaunchImageTimeStamp", ref needsUpdate);

                SplashHasNewTimestamp = needsUpdate;
                if (needsUpdate)
                {
                    AppLaunchImageResponseModel appLaunchImageResponse = iService.GetAppLaunchImageItem();
                    if (appLaunchImageResponse.Status.IsValid() && appLaunchImageResponse.Status.ToUpper() == "SUCCESS")
                    {
                        if (appLaunchImageResponse != null && appLaunchImageResponse.Data != null && appLaunchImageResponse.Data.Count > 0)
                        {
                            var sharedPreference = NSUserDefaults.StandardUserDefaults;
                            var jsonStr = JsonConvert.SerializeObject(appLaunchImageResponse);
                            sharedPreference.SetString(jsonStr, "AppLaunchImageData");
                            sharedPreference.Synchronize();
                            UpdateSharedPreference(timeStamp.Data[0].Timestamp, "AppLaunchImageTimeStamp");
                            Debug.WriteLine("LoadDynamicSplash Done");
                        }
                    }
                }
                isDone = true;
            });

            return isDone;
        }

        public Task LoadCountry()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS
                    , DataManager.DataManager.SharedInstance.ImageSize
                    , TNBGlobal.SITECORE_URL
                    , TNBGlobal.APP_LANGUAGE);

                CountryTimeStampResponseModel timeStamp = iService.GetCountryTimestampItem();
                bool needsUpdate = true;

                if (timeStamp == null || timeStamp.Data == null || timeStamp.Data.Count == 0
                     || string.IsNullOrEmpty(timeStamp.Data[0].Timestamp)
                     || string.IsNullOrWhiteSpace(timeStamp.Data[0].Timestamp))
                {
                    timeStamp = new CountryTimeStampResponseModel();
                    timeStamp.Data = new List<CountryTimeStamp> { new CountryTimeStamp { Timestamp = string.Empty } };
                }

                UpdateTimeStamp(timeStamp.Data[0].Timestamp, "CountryTimeStamp", ref needsUpdate);

                if (needsUpdate)
                {
                    CountryResponseModel languageItems = iService.GetCountryItems();
                    if (languageItems != null && languageItems.Data != null
                        && languageItems.Data.Count > 0 && languageItems.Data[0] != null)
                    {
                        CountryDataModel item = languageItems.Data[0];
                        if (item.CountryFile.IsValid())
                        {
                            string content = GetDataFromFile(item.CountryFile);
                            CountryManager.Instance.SetCountries(content);
                            CountryUtility.CountryContent = content;
                            UpdateSharedPreference(timeStamp.Data[0].Timestamp, "CountryTimeStamp");
                        }
                        else
                        {
                            CountryManager.Instance.SetCountries();
                            CountryUtility.CountryContent = CountryManager.Instance.CountryString;
                        }
                        Debug.WriteLine("LoadCountry Done");
                    }
                }
                else
                {
                    string content = CountryUtility.CountryContent;
                    if (content.IsValid())
                    {
                        CountryManager.Instance.SetCountries(content);
                    }
                    else
                    {
                        CountryManager.Instance.SetCountries();
                    }
                    CountryUtility.CountryContent = CountryManager.Instance.CountryString;
                }
            });
        }
    }
}
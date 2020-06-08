using myTNB.SitecoreCM.Services;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Service;
using myTNB_Android.Src.SitecoreCMS.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace myTNB.SitecoreCMS.Services
{
    public class GetItemsService
    {
        private static string OS { get; set; }
        private static string ImageSize { get; set; }
        private static string WebsiteUrl { get; set; }
        private static string Language { get; set; }

        public GetItemsService(string os, string imageSize, string websiteUrl, string language = "en")
        {
            OS = os;
            ImageSize = imageSize;
            WebsiteUrl = websiteUrl;
            Language = language;
        }

        public string GetWalkthroughScreenItems()
        {
            WalkthroughScreenService service = new WalkthroughScreenService();
            var data = service.GetWalkthroughScreens(OS, ImageSize, WebsiteUrl, Language);
            var resp = CheckData(data.ToList<object>());
            return JsonConvert.SerializeObject(resp);
        }

        public string GetPreLoginPromoItem()
        {
            PreLoginPromoService service = new PreLoginPromoService();
            var data = service.GetPreLoginPromo(OS, ImageSize, WebsiteUrl, Language);
            var listData = AddDataToList(data);
            var resp = CheckData(listData);
            return JsonConvert.SerializeObject(resp);
        }

        public string GetFullRTEPagesItems()
        {
            FullRTEPagesService service = new FullRTEPagesService();
            var data = service.GetFullRTEPages(WebsiteUrl, Language);
            var resp = CheckData(data.ToList<object>());
            return JsonConvert.SerializeObject(resp);
        }

        public string GetEnergyTipsItems()
        {
            EnergyTipsService service = new EnergyTipsService();
            var data = service.GetEnergyTips(OS, ImageSize, WebsiteUrl, Language);
            var resp = CheckData(data.ToList<object>());
            return JsonConvert.SerializeObject(resp);
        }

        public string GetLocationsItems()
        {
            LocationsService service = new LocationsService();
            var data = service.GetLocations(WebsiteUrl, Language);
            var resp = CheckData(data.ToList<object>());
            return JsonConvert.SerializeObject(resp);
        }

        public string GetFAQsItem()
        {
            FAQsService service = new FAQsService();
            var data = service.GetFAQsService(OS, ImageSize, WebsiteUrl, Language);
            var resp = CheckData(data.ToList<object>());
            return JsonConvert.SerializeObject(resp);
        }
        public string GetFAQsTimestampItem()
        {
            FAQsService service = new FAQsService();
            var data = service.GetTimestamp(WebsiteUrl, Language);
            var listData = AddDataToList(data);
            var resp = CheckData(listData);
            return JsonConvert.SerializeObject(resp);
        }

        public string GetTimestampItem()
        {
            TimestampService service = new TimestampService();
            var data = service.GetTimestamp(WebsiteUrl, Language);
            var listData = AddDataToList(data);
            var resp = CheckData(listData);
            return JsonConvert.SerializeObject(resp);
        }


        public AppLaunchResponseModel GetAppLaunchItem()
        {
            AppLaunchResponseModel respModel = new AppLaunchResponseModel();
            try
            {
                AppLaunchService service = new AppLaunchService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetItems();
                var resp = CheckData(data.ToList<object>());
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<AppLaunchResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetAppLaunchItem: " + e.Message);
            }
            return respModel;
        }
        public HelpResponseModel GetHelpItems()
        {
            HelpResponseModel respModel = new HelpResponseModel();
            try
            {
                HelpService service = new HelpService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetItems();
                var resp = CheckData(data.ToList<object>());
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<HelpResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/HelpResponseModel: " + e.Message);
            }
            return respModel;
        }

        public HelpTimeStampResponseModel GetHelpTimestampItem()
        {
            HelpTimeStampResponseModel respModel = new HelpTimeStampResponseModel();
            try
            {
                HelpService service = new HelpService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetTimeStamp();
                var listData = AddDataToList(data);
                var resp = CheckData(listData);
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<HelpTimeStampResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetFAQsTimestampItem: " + e.Message);
            }
            return respModel;
        }

        public ApplySSMRResponseModel GetApplySSMRWalkthroughItems()
        {
            ApplySSMRResponseModel respModel = new ApplySSMRResponseModel();
            try
            {
                ApplySSMRWalkthroughService service = new ApplySSMRWalkthroughService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetItems();
                var resp = CheckData(data.ToList<object>());
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<ApplySSMRResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetApplySSMRWalkthroughItems: " + e.Message);
            }
            return respModel;
        }

        public ApplySSMRTimeStampResponseModel GetApplySSMRWalkthroughTimestampItem()
        {
            ApplySSMRTimeStampResponseModel respModel = new ApplySSMRTimeStampResponseModel();
            try
            {
                ApplySSMRWalkthroughService service = new ApplySSMRWalkthroughService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetTimeStamp();
                var listData = AddDataToList(data);
                var resp = CheckData(listData);
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<ApplySSMRTimeStampResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetApplySSMRWalkthroughTimestampItem: " + e.Message);
            }
            return respModel;
        }

        public SSMRMeterReadingResponseModel GetSSMRMeterReadingOnePhaseWalkthroughItems()
        {
            SSMRMeterReadingResponseModel respModel = new SSMRMeterReadingResponseModel();
            try
            {
                SSMRMeterReadingOnePhaseWalkThroughService service = new SSMRMeterReadingOnePhaseWalkThroughService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetItems();
                var resp = CheckData(data.ToList<object>());
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<SSMRMeterReadingResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetSSMRMeterReadingOnePhaseWalkthroughItems: " + e.Message);
            }
            return respModel;
        }

        public SSMRMeterReadingTimeStampResponseModel GetSSMRMeterReadingOnePhaseWalkthroughTimestampItem()
        {
            SSMRMeterReadingTimeStampResponseModel respModel = new SSMRMeterReadingTimeStampResponseModel();
            try
            {
                SSMRMeterReadingOnePhaseWalkThroughService service = new SSMRMeterReadingOnePhaseWalkThroughService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetTimeStamp();
                var listData = AddDataToList(data);
                var resp = CheckData(listData);
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<SSMRMeterReadingTimeStampResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetSSMRMeterReadingOnePhaseWalkthroughTimestampItem: " + e.Message);
            }
            return respModel;
        }

        public SSMRMeterReadingResponseModel GetSSMRMeterReadingOnePhaseOCROffWalkthroughItems()
        {
            SSMRMeterReadingResponseModel respModel = new SSMRMeterReadingResponseModel();
            try
            {
                SSMRMeterReadingOnePhaseWalkThroughOCROffService service = new SSMRMeterReadingOnePhaseWalkThroughOCROffService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetItems();
                var resp = CheckData(data.ToList<object>());
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<SSMRMeterReadingResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetSSMRMeterReadingOnePhaseOCROffWalkthroughItems: " + e.Message);
            }
            return respModel;
        }

        public SSMRMeterReadingTimeStampResponseModel GetSSMRMeterReadingOnePhaseOCROffWalkthroughTimestampItem()
        {
            SSMRMeterReadingTimeStampResponseModel respModel = new SSMRMeterReadingTimeStampResponseModel();
            try
            {
                SSMRMeterReadingOnePhaseWalkThroughOCROffService service = new SSMRMeterReadingOnePhaseWalkThroughOCROffService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetTimeStamp();
                var listData = AddDataToList(data);
                var resp = CheckData(listData);
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<SSMRMeterReadingTimeStampResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetSSMRMeterReadingOnePhaseOCROffWalkthroughTimestampItem: " + e.Message);
            }
            return respModel;
        }

        public SSMRMeterReadingResponseModel GetSSMRMeterReadingThreePhaseWalkthroughItems()
        {
            SSMRMeterReadingResponseModel respModel = new SSMRMeterReadingResponseModel();
            try
            {
                SSMRMeterReadingThreePhaseWalkthroughService service = new SSMRMeterReadingThreePhaseWalkthroughService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetItems();
                var resp = CheckData(data.ToList<object>());
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<SSMRMeterReadingResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetSSMRMeterReadingThreePhaseWalkthroughItems: " + e.Message);
            }
            return respModel;
        }

        public SSMRMeterReadingTimeStampResponseModel GetSSMRMeterReadingThreePhaseWalkthroughTimestampItem()
        {
            SSMRMeterReadingTimeStampResponseModel respModel = new SSMRMeterReadingTimeStampResponseModel();
            try
            {
                SSMRMeterReadingThreePhaseWalkthroughService service = new SSMRMeterReadingThreePhaseWalkthroughService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetTimeStamp();
                var listData = AddDataToList(data);
                var resp = CheckData(listData);
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<SSMRMeterReadingTimeStampResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetSSMRMeterReadingThreePhaseWalkthroughTimestampItem: " + e.Message);
            }
            return respModel;
        }

        public SSMRMeterReadingResponseModel GetSSMRMeterReadingThreePhaseOCROffWalkthroughItems()
        {
            SSMRMeterReadingResponseModel respModel = new SSMRMeterReadingResponseModel();
            try
            {
                SSMRMeterReadingThreePhaseWalkthroughOCROffService service = new SSMRMeterReadingThreePhaseWalkthroughOCROffService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetItems();
                var resp = CheckData(data.ToList<object>());
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<SSMRMeterReadingResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetSSMRMeterReadingThreePhaseOCROffWalkthroughItems: " + e.Message);
            }
            return respModel;
        }

        public SSMRMeterReadingTimeStampResponseModel GetSSMRMeterReadingThreePhaseOCROffWalkthroughTimestampItem()
        {
            SSMRMeterReadingTimeStampResponseModel respModel = new SSMRMeterReadingTimeStampResponseModel();
            try
            {
                SSMRMeterReadingThreePhaseWalkthroughOCROffService service = new SSMRMeterReadingThreePhaseWalkthroughOCROffService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetTimeStamp();
                var listData = AddDataToList(data);
                var resp = CheckData(listData);
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<SSMRMeterReadingTimeStampResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetSSMRMeterReadingThreePhaseOCROffWalkthroughTimestampItem: " + e.Message);
            }
            return respModel;
        }

        public EnergySavingTipsResponseModel GetEnergySavingTipsItem()
        {
            EnergySavingTipsResponseModel respModel = new EnergySavingTipsResponseModel();
            try
            {
                EnergySavingTipsService service = new EnergySavingTipsService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetItems();
                var resp = CheckData(data.ToList<object>());
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<EnergySavingTipsResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetEnergySavingTipsItem: " + e.Message);
            }
            return respModel;
        }

        public EnergySavingTipsTimeStampResponseModel GetEnergySavingTipsTimestampItem()
        {
            EnergySavingTipsTimeStampResponseModel respModel = new EnergySavingTipsTimeStampResponseModel();
            try
            {
                EnergySavingTipsService service = new EnergySavingTipsService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetTimeStamp();
                var listData = AddDataToList(data);
                var resp = CheckData(listData);
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<EnergySavingTipsTimeStampResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetEnergySavingTipsTimestampItem: " + e.Message);
            }
            return respModel;
        }

        public BillDetailsTooltipResponseModel GetBillDetailsTooltipItem()
        {
            BillDetailsTooltipResponseModel respModel = new BillDetailsTooltipResponseModel();
            try
            {
                BillDetailsTooltipService service = new BillDetailsTooltipService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetItems();
                var resp = CheckData(data.ToList<object>());
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<BillDetailsTooltipResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetBillDetailsTooltipItem: " + e.Message);
            }
            return respModel;
        }

        public AppLaunchTimeStampResponseModel GetAppLaunchTimestampItem()
        {
            AppLaunchTimeStampResponseModel respModel = new AppLaunchTimeStampResponseModel();
            try
            {
                AppLaunchService service = new AppLaunchService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetTimeStamp();
                var listData = AddDataToList(data);
                var resp = CheckData(listData);
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<AppLaunchTimeStampResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetAppLaunchTimestampItem: " + e.Message);
            }
            return respModel;
        }
        public BillDetailsTooltipTimeStampResponseModel GetBillDetailsTooltipTimestampItem()
        {
            BillDetailsTooltipTimeStampResponseModel respModel = new BillDetailsTooltipTimeStampResponseModel();
            try
            {
                BillDetailsTooltipService service = new BillDetailsTooltipService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetTimeStamp();
                var listData = AddDataToList(data);
                var resp = CheckData(listData);
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<BillDetailsTooltipTimeStampResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetBillDetailsTooltipTimestampItem: " + e.Message);
            }
            return respModel;
        }

        public RewardsResponseModel GetRewardsItems()
        {
            RewardsResponseModel respModel = new RewardsResponseModel();
            try
            {
                RewardsService service = new RewardsService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetItems();
                var resp = CheckData(data.ToList<object>());
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<RewardsResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetRewardsItems: " + e.Message);
            }
            return respModel;
        }

        public RewardsTimeStampResponseModel GetRewardsTimestampItem()
        {
            RewardsTimeStampResponseModel respModel = new RewardsTimeStampResponseModel();
            try
            {
                RewardsService service = new RewardsService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetTimeStamp();
                var listData = AddDataToList(data);
                var resp = CheckData(listData);
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<RewardsTimeStampResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetRewardsTimestampItem: " + e.Message);
            }
            return respModel;
        }

        public WhatsNewResponseModel GetWhatsNewItems()
        {
            WhatsNewResponseModel respModel = new WhatsNewResponseModel();
            try
            {
                WhatsNewService service = new WhatsNewService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetItems();
                var resp = CheckData(data.ToList<object>());
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<WhatsNewResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetWhatsNewItems: " + e.Message);
            }
            return respModel;
        }

        public WhatsNewTimeStampResponseModel GetWhatsNewTimestampItem()
        {
            WhatsNewTimeStampResponseModel respModel = new WhatsNewTimeStampResponseModel();
            try
            {
                WhatsNewService service = new WhatsNewService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetTimeStamp();
                var listData = AddDataToList(data);
                var resp = CheckData(listData);
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<WhatsNewTimeStampResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetWhatsNewTimestampItem: " + e.Message);
            }
            return respModel;
        }

        private BaseModel CheckData(List<object> data)
        {
            BaseModel bm = new BaseModel();
            bool isAnyIdNull = true;
            foreach (var item in data)
            {
                var type = item.GetType();
                var prop = type.GetProperty("ID");
                var field = type.GetField("ID");
                var value = prop == null ? field.GetValue(item) : prop.GetValue(item);
                bool isNull = value == null;
                isAnyIdNull = isAnyIdNull && isNull;
            }

            if (!isAnyIdNull)
            {
                bm.Status = "Success";
                bm.Data = data.ToList<object>();
            }
            return bm;
        }

        private List<object> AddDataToList(object data)
        {
            List<object> listData = new List<object>();
            listData.Add(data);
            return listData;
        }

        public LanguageResponseModel GetLanguageItems()
        {
            LanguageResponseModel respModel = new LanguageResponseModel();
            try
            {
                LanguageService service = new LanguageService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetItems();
                var resp = CheckData(data.ToList<object>());
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<LanguageResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetLanguageItems: " + e.Message);
            }
            return respModel;
        }

        public LanguageTimeStampResponseModel GetLanguageTimestampItem()
        {
            LanguageTimeStampResponseModel respModel = new LanguageTimeStampResponseModel();
            try
            {
                LanguageService service = new LanguageService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetTimeStamp();
                var listData = AddDataToList(data);
                var resp = CheckData(listData);
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<LanguageTimeStampResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetLanguageTimestampItem: " + e.Message);
            }
            return respModel;
        }

        public CountryResponseModel GetCountryItems()
        {
            CountryResponseModel respModel = new CountryResponseModel();
            try
            {
                CountryService service = new CountryService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetItems();
                var resp = CheckData(data.ToList<object>());
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<CountryResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetLanguageItems: " + e.Message);
            }
            return respModel;
        }

        public CountryTimeStampResponseModel GetCountryTimestampItem()
        {
            CountryTimeStampResponseModel respModel = new CountryTimeStampResponseModel();
            try
            {
                CountryService service = new CountryService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetTimeStamp();
                var listData = AddDataToList(data);
                var resp = CheckData(listData);
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<CountryTimeStampResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetLanguageTimestampItem: " + e.Message);
            }
            return respModel;
        }

        public EppToolTipResponseModel GetEppToolTipItem()
        {
            EppToolTipResponseModel respModel = new EppToolTipResponseModel();
            try
            {
                EppToolTipService service = new EppToolTipService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetItems();
                var resp = CheckData(data.ToList<object>());
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<EppToolTipResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetEppToolTipItem: " + e.Message);
            }
            return respModel;
        }

       
        public EppToolTipTimeStampResponseModel GetEppToolTipTimeStampItem()
        {
            EppToolTipTimeStampResponseModel respModel = new EppToolTipTimeStampResponseModel();
            try
            {
                EppToolTipService service = new EppToolTipService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetTimeStamp();
                var listData = AddDataToList(data);
                var resp = CheckData(listData);
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<EppToolTipTimeStampResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetEppToolTipTimeStampItem: " + e.Message);
            }
            return respModel;
        }
    }
}

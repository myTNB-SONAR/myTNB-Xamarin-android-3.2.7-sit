using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using myTNB.SitecoreCMS.Service;
using myTNB.SitecoreCMS.Model;
using System;
using System.Diagnostics;

namespace myTNB.SitecoreCMS.Services
{
    public class GetItemsService
    {
        static string OS { get; set; }
        static string ImageSize { get; set; }
        static string WebsiteUrl { get; set; }
        static string Language { get; set; }

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

        public TermsAndConditionResponseModel GetFullRTEPagesItems()
        {
            TermsAndConditionResponseModel respModel = new TermsAndConditionResponseModel();
            try
            {
                FullRTEPagesService service = new FullRTEPagesService();
                var data = service.GetFullRTEPages(WebsiteUrl, Language);
                var resp = CheckData(data.ToList<object>());
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<TermsAndConditionResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetFullRTEPagesItems: " + e.Message);
            }
            return respModel;
        }

        public string GetPromotionsItem()
        {
            PromotionsV2Service service = new PromotionsV2Service();
            var data = service.GetPromotionsService(OS, ImageSize, WebsiteUrl, Language);
            var resp = CheckData(data.ToList<object>());
            return JsonConvert.SerializeObject(resp);
        }
        public string GetPromotionsTimestampItem()
        {
            PromotionsV2Service service = new PromotionsV2Service();
            var data = service.GetTimestamp(WebsiteUrl, Language);
            var listData = AddDataToList(data);
            var resp = CheckData(listData);
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

        public string GetAppLaunchImageItem()
        {
            AppLaunchImageService service = new AppLaunchImageService();
            var data = service.GetAppLaunchImageService(OS, ImageSize, WebsiteUrl, Language);
            var resp = CheckData(data.ToList<object>());
            return JsonConvert.SerializeObject(resp);
        }
        public string GetAppLaunchImageTimestampItem()
        {
            AppLaunchImageService service = new AppLaunchImageService();
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

        public TimestampResponseModel GetTimestampItemV2()
        {
            TimestampResponseModel responseModel = new TimestampResponseModel();
            try
            {
                TimestampService service = new TimestampService();
                TimestampModel data = service.GetTimestamp(WebsiteUrl, Language);
                List<object> listData = AddDataToList(data);
                BaseModel resp = CheckData(listData);
                string serializedObj = JsonConvert.SerializeObject(resp);
                responseModel = JsonConvert.DeserializeObject<TimestampResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetTimestampItemV2: " + e.Message);
            }
            return responseModel;
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
                Debug.WriteLine("Exception in GetItemsService/GetHelpItems: " + e.Message);
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
                Debug.WriteLine("Exception in GetItemsService/GetHelpTimestampItem: " + e.Message);
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

        public MeterReadSSMRResponseModel GetMeterReadSSMRWalkthroughItems()
        {
            MeterReadSSMRResponseModel respModel = new MeterReadSSMRResponseModel();
            try
            {
                MeterReadSSMRWalkthroughService service = new MeterReadSSMRWalkthroughService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetItems();
                var resp = CheckData(data.ToList<object>());
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<MeterReadSSMRResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetMeterReadSSMRWalkthroughItems: " + e.Message);
            }
            return respModel;
        }

        public MeterReadSSMRTimeStampResponseModel GetMeterReadSSMRWalkthroughTimestampItem()
        {
            MeterReadSSMRTimeStampResponseModel respModel = new MeterReadSSMRTimeStampResponseModel();
            try
            {
                MeterReadSSMRWalkthroughService service = new MeterReadSSMRWalkthroughService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetTimeStamp();
                var listData = AddDataToList(data);
                var resp = CheckData(listData);
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<MeterReadSSMRTimeStampResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetMeterReadSSMRWalkthroughTimestampItem: " + e.Message);
            }
            return respModel;
        }

        public MeterReadSSMRResponseModel GetMeterReadSSMRWalkthroughItemsV2()
        {
            MeterReadSSMRResponseModel respModel = new MeterReadSSMRResponseModel();
            try
            {
                MeterReadSSMRWalkthroughServiceV2 service = new MeterReadSSMRWalkthroughServiceV2(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetItems();
                var resp = CheckData(data.ToList<object>());
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<MeterReadSSMRResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetMeterReadSSMRWalkthroughItemsV2: " + e.Message);
            }
            return respModel;
        }

        public MeterReadSSMRTimeStampResponseModel GetMeterReadSSMRWalkthroughTimestampItemV2()
        {
            MeterReadSSMRTimeStampResponseModel respModel = new MeterReadSSMRTimeStampResponseModel();
            try
            {
                MeterReadSSMRWalkthroughServiceV2 service = new MeterReadSSMRWalkthroughServiceV2(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetTimeStamp();
                var listData = AddDataToList(data);
                var resp = CheckData(listData);
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<MeterReadSSMRTimeStampResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetMeterReadSSMRWalkthroughTimestampItemV2: " + e.Message);
            }
            return respModel;
        }

        public EnergyTipsResponseModel GetEnergyTipsItem()
        {
            EnergyTipsResponseModel respModel = new EnergyTipsResponseModel();
            try
            {
                EnergyTipsService service = new EnergyTipsService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetItems();
                var resp = CheckData(data.ToList<object>());
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<EnergyTipsResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetEnergyTipsItem: " + e.Message);
            }
            return respModel;
        }

        public EnergyTipsTimeStampResponseModel GetEnergyTipsTimestampItem()
        {
            EnergyTipsTimeStampResponseModel respModel = new EnergyTipsTimeStampResponseModel();
            try
            {
                EnergyTipsService service = new EnergyTipsService(OS, ImageSize, WebsiteUrl, Language);
                var data = service.GetTimeStamp();
                var listData = AddDataToList(data);
                var resp = CheckData(listData);
                string serializedObj = JsonConvert.SerializeObject(resp);
                respModel = JsonConvert.DeserializeObject<EnergyTipsTimeStampResponseModel>(serializedObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetItemsService/GetEnergyTipsTimestampItem: " + e.Message);
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
    }
}
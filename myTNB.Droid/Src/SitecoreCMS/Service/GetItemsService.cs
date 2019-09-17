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

        public string GetPromotionsItem()
        {
            PromotionsService service = new PromotionsService();
            var data = service.GetPromotionsService(OS, ImageSize, WebsiteUrl, Language);
            var resp = CheckData(data.ToList<object>());
            return JsonConvert.SerializeObject(resp);
        }

        public string GetPromotionsV2Item()
        {
            PromotionsV2Service service = new PromotionsV2Service();
            var data = service.GetPromotionsService(OS, ImageSize, WebsiteUrl, Language);
            var resp = CheckData(data.ToList<object>());
            return JsonConvert.SerializeObject(resp);
        }

        public string GetPromotionsTimestampItem()
        {
            PromotionsService service = new PromotionsService();
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

        public string GetTimestampItem()
        {
            TimestampService service = new TimestampService();
            var data = service.GetTimestamp(WebsiteUrl, Language);
            var listData = AddDataToList(data);
            var resp = CheckData(listData);
            return JsonConvert.SerializeObject(resp);
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
                Debug.WriteLine("Exception in GetItemsService/GetApplySSMRWalkthroughItems: " + e.Message);
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
                Debug.WriteLine("Exception in GetItemsService/GetApplySSMRWalkthroughTimestampItem: " + e.Message);
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
                Debug.WriteLine("Exception in GetItemsService/GetApplySSMRWalkthroughItems: " + e.Message);
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
                Debug.WriteLine("Exception in GetItemsService/GetApplySSMRWalkthroughTimestampItem: " + e.Message);
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
    }
}
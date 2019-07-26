﻿using Newtonsoft.Json;
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

        public string GetFullRTEPagesItems()
        {
            FullRTEPagesService service = new FullRTEPagesService();
            var data = service.GetFullRTEPages(WebsiteUrl, Language);
            var resp = CheckData(data.ToList<object>());
            return JsonConvert.SerializeObject(resp);
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

        public string GetTimestampItem()
        {
            TimestampService service = new TimestampService();
            var data = service.GetTimestamp(WebsiteUrl, Language);
            var listData = AddDataToList(data);
            var resp = CheckData(listData);
            return JsonConvert.SerializeObject(resp);
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
    }
}
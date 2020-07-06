using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using myTNB.SitecoreCMS.Service;
using myTNB.SitecoreCMS.Model;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

namespace myTNB.SitecoreCMS.Services
{
    public class GetItemsService
    {
        static string OS { get; set; }
        static string ImageSize { get; set; }
        static string WebsiteUrl { get; set; }
        static string Language { get; set; }

        private readonly TimeSpan timeSpan = TimeSpan.FromMilliseconds(5000);

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
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
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
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
        }

        public FAQsResponseModel GetFAQsItems()
        {
            FAQsResponseModel respModel = new FAQsResponseModel();
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
                try
                {
                    FAQsService service = new FAQsService(OS, ImageSize, WebsiteUrl, Language);
                    var data = service.GetFAQsItems();
                    var resp = CheckData(data.ToList<object>());
                    string serializedObj = JsonConvert.SerializeObject(resp);
                    respModel = JsonConvert.DeserializeObject<FAQsResponseModel>(serializedObj);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception in GetItemsService/GetFAQsItems: " + e.Message);
                }
                return respModel;
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
        }

        public FAQTimestampResponseModel GetFAQsTimestampItem()
        {
            FAQTimestampResponseModel respModel = new FAQTimestampResponseModel();
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
                try
                {
                    FAQsService service = new FAQsService(OS, ImageSize, WebsiteUrl, Language);
                    FAQsParentModel data = service.GetTimestamp();
                    List<object> listData = AddDataToList(data);
                    BaseModel resp = CheckData(listData);
                    string serializedObj = JsonConvert.SerializeObject(resp);
                    respModel = JsonConvert.DeserializeObject<FAQTimestampResponseModel>(serializedObj);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception in GetItemsService/GetFAQsTimestampItem: " + e.Message);
                }
                return respModel;
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
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
            TimestampResponseModel respModel = new TimestampResponseModel();
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
                try
                {
                    TimestampService service = new TimestampService();
                    TimestampModel data = service.GetTimestamp(WebsiteUrl, Language);
                    List<object> listData = AddDataToList(data);
                    BaseModel resp = CheckData(listData);
                    string serializedObj = JsonConvert.SerializeObject(resp);
                    respModel = JsonConvert.DeserializeObject<TimestampResponseModel>(serializedObj);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception in GetItemsService/GetTimestampItemV2: " + e.Message);
                }
                return respModel;
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
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
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
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
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
        }

        public HelpTimeStampResponseModel GetHelpTimestampItem()
        {
            HelpTimeStampResponseModel respModel = new HelpTimeStampResponseModel();
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
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
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
        }

        public AppLaunchImageTimestampResponseModel GetAppLaunchImageTimestampItem()
        {
            AppLaunchImageTimestampResponseModel respModel = new AppLaunchImageTimestampResponseModel();
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
                try
                {
                    AppLaunchImageService service = new AppLaunchImageService(OS, ImageSize, WebsiteUrl, Language);
                    var data = service.GetTimestamp();
                    var listData = AddDataToList(data);
                    var resp = CheckData(listData);
                    string serializedObj = JsonConvert.SerializeObject(resp);
                    respModel = JsonConvert.DeserializeObject<AppLaunchImageTimestampResponseModel>(serializedObj);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception in GetItemsService/GetAppLaunchImageTimestampItem: " + e.Message);
                }
                return respModel;
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
        }

        public AppLaunchImageResponseModel GetAppLaunchImageItem()
        {
            AppLaunchImageResponseModel respModel = new AppLaunchImageResponseModel();
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
                try
                {
                    AppLaunchImageService service = new AppLaunchImageService(OS, ImageSize, WebsiteUrl, Language);
                    var data = service.GetAppLaunchImageService();
                    var resp = CheckData(data.ToList<object>());
                    string serializedObj = JsonConvert.SerializeObject(resp);
                    respModel = JsonConvert.DeserializeObject<AppLaunchImageResponseModel>(serializedObj);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception in GetItemsService/GetAppLaunchImageItem: " + e.Message);
                }
                return respModel;
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
        }

        public ApplySSMRResponseModel GetApplySSMRWalkthroughItems()
        {
            ApplySSMRResponseModel respModel = new ApplySSMRResponseModel();
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
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
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
        }

        public ApplySSMRTimeStampResponseModel GetApplySSMRWalkthroughTimestampItem()
        {
            ApplySSMRTimeStampResponseModel respModel = new ApplySSMRTimeStampResponseModel();
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
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
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
        }

        public MeterReadSSMRResponseModel GetMeterReadSSMRWalkthroughItems()
        {
            MeterReadSSMRResponseModel respModel = new MeterReadSSMRResponseModel();
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
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
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
        }

        public MeterReadSSMRTimeStampResponseModel GetMeterReadSSMRWalkthroughTimestampItem()
        {
            MeterReadSSMRTimeStampResponseModel respModel = new MeterReadSSMRTimeStampResponseModel();
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
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
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
        }

        public MeterReadSSMRResponseModel GetMeterReadSSMRWalkthroughItemsV2()
        {
            MeterReadSSMRResponseModel respModel = new MeterReadSSMRResponseModel();
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
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
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
        }

        public MeterReadSSMRTimeStampResponseModel GetMeterReadSSMRWalkthroughTimestampItemV2()
        {
            MeterReadSSMRTimeStampResponseModel respModel = new MeterReadSSMRTimeStampResponseModel();
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
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
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
        }

        public EnergyTipsResponseModel GetEnergyTipsItem()
        {
            EnergyTipsResponseModel respModel = new EnergyTipsResponseModel();
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
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
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
        }

        public EnergyTipsTimeStampResponseModel GetEnergyTipsTimestampItem()
        {
            EnergyTipsTimeStampResponseModel respModel = new EnergyTipsTimeStampResponseModel();
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
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
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
        }

        public BillDetailsTooltipResponseModel GetBillDetailsTooltipItem()
        {
            BillDetailsTooltipResponseModel respModel = new BillDetailsTooltipResponseModel();
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
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
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
        }

        public BillDetailsTooltipTimeStampResponseModel GetBillDetailsTooltipTimestampItem()
        {
            BillDetailsTooltipTimeStampResponseModel respModel = new BillDetailsTooltipTimeStampResponseModel();
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
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
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
        }

        //Created by Syahmi ICS 05052020
        public EppInfoTooltipResponseModel GetEppInfoTooltipItem()
        {
            EppInfoTooltipResponseModel respModel = new EppInfoTooltipResponseModel();
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
                try
                {
                    EppInfoTooltipService service = new EppInfoTooltipService(OS, ImageSize, WebsiteUrl, Language);
                    var data = service.GetItems();
                    var resp = CheckData(data.ToList<object>());
                    string serializedObj = JsonConvert.SerializeObject(resp);
                    respModel = JsonConvert.DeserializeObject<EppInfoTooltipResponseModel>(serializedObj);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception in GetItemsService/GetEppInfoTooltipItem: " + e.Message);
                }
                return respModel;
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
        }

        public EppInfoTooltipTimeStampResponseModel GetEppInfoTooltipTimestampItem()
        {
            EppInfoTooltipTimeStampResponseModel respModel = new EppInfoTooltipTimeStampResponseModel();
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
                try
                {
                    EppInfoTooltipService service = new EppInfoTooltipService(OS, ImageSize, WebsiteUrl, Language);
                    var data = service.GetTimeStamp();
                    var listData = AddDataToList(data);
                    var resp = CheckData(listData);
                    string serializedObj = JsonConvert.SerializeObject(resp);
                    respModel = JsonConvert.DeserializeObject<EppInfoTooltipTimeStampResponseModel>(serializedObj);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception in GetItemsService/GetEppInfoTooltipTimestampItem: " + e.Message);
                }
                return respModel;
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
        }


        public LanguageResponseModel GetLanguageItems()
        {
            LanguageResponseModel respModel = new LanguageResponseModel();
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
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
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
        }

        public LanguageTimeStampResponseModel GetLanguageTimestampItem()
        {
            LanguageTimeStampResponseModel respModel = new LanguageTimeStampResponseModel();
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
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
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
        }

        public RewardsTimestampResponseModel GetRewardsTimestampItem()
        {
            RewardsTimestampResponseModel respModel = new RewardsTimestampResponseModel();
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
                try
                {
                    RewardsService service = new RewardsService(OS, ImageSize, WebsiteUrl, Language);
                    var data = service.GetTimeStamp();
                    var listData = AddDataToList(data);
                    var resp = CheckData(listData);
                    string serializedObj = JsonConvert.SerializeObject(resp);
                    respModel = JsonConvert.DeserializeObject<RewardsTimestampResponseModel>(serializedObj);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception in GetItemsService/GetRewardsTimestampItem: " + e.Message);
                }
                return respModel;
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
        }

        public RewardsResponseModel GetRewardsItems()
        {
            RewardsResponseModel respModel = new RewardsResponseModel();
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
                try
                {
                    RewardsService service = new RewardsService(OS, ImageSize, WebsiteUrl, Language);
                    var data = service.GetCategoryItems();
                    var resp = CheckData(data.ToList<object>());
                    string serializedObj = JsonConvert.SerializeObject(resp);
                    respModel = JsonConvert.DeserializeObject<RewardsResponseModel>(serializedObj);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception in GetItemsService/GetRewardsItems: " + e.Message);
                }
                return respModel;
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
        }

        public WhatsNewTimestampResponseModel GetWhatsNewTimestampItem()
        {
            WhatsNewTimestampResponseModel respModel = new WhatsNewTimestampResponseModel();
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
                try
                {
                    WhatsNewService service = new WhatsNewService(OS, ImageSize, WebsiteUrl, Language);
                    var data = service.GetTimeStamp();
                    var listData = AddDataToList(data);
                    var resp = CheckData(listData);
                    string serializedObj = JsonConvert.SerializeObject(resp);
                    respModel = JsonConvert.DeserializeObject<WhatsNewTimestampResponseModel>(serializedObj);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception in GetItemsService/GetWhatsNewTimestampItem: " + e.Message);
                }
                return respModel;
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
        }

        public WhatsNewResponseModel GetWhatsNewItems()
        {
            WhatsNewResponseModel respModel = new WhatsNewResponseModel();
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
                try
                {
                    WhatsNewService service = new WhatsNewService(OS, ImageSize, WebsiteUrl, Language);
                    var data = service.GetCategoryItems();
                    var resp = CheckData(data.ToList<object>());
                    string serializedObj = JsonConvert.SerializeObject(resp);
                    respModel = JsonConvert.DeserializeObject<WhatsNewResponseModel>(serializedObj);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception in GetItemsService/GetWhatsNewItems: " + e.Message);
                }
                return respModel;
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
        }

        public PromotionsTimestampResponseModel GetPromotionsTimestampItem()
        {
            PromotionsTimestampResponseModel respModel = new PromotionsTimestampResponseModel();
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
                try
                {
                    PromotionsService service = new PromotionsService(OS, ImageSize, WebsiteUrl, Language);
                    var data = service.GetTimestamp();
                    var listData = AddDataToList(data);
                    var resp = CheckData(listData);
                    string serializedObj = JsonConvert.SerializeObject(resp);
                    respModel = JsonConvert.DeserializeObject<PromotionsTimestampResponseModel>(serializedObj);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception in GetItemsService/GetPromotionsTimestampItem: " + e.Message);
                }
                return respModel;
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
        }

        public PromotionsResponseModel GetPromotionsItem()
        {
            PromotionsResponseModel respModel = new PromotionsResponseModel();
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
                try
                {
                    PromotionsService service = new PromotionsService(OS, ImageSize, WebsiteUrl, Language);
                    var data = service.GetPromotionsService();
                    var resp = CheckData(data.ToList<object>());
                    string serializedObj = JsonConvert.SerializeObject(resp);
                    respModel = JsonConvert.DeserializeObject<PromotionsResponseModel>(serializedObj);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception in GetItemsService/GetPromotionsTimestampItem: " + e.Message);
                }
                return respModel;
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
        }

        public CountryResponseModel GetCountryItems()
        {
            CountryResponseModel respModel = new CountryResponseModel();
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
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
                    Debug.WriteLine("Exception in GetItemsService/CountryResponseModel: " + e.Message);
                }
                return respModel;
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
        }

        public CountryTimeStampResponseModel GetCountryTimestampItem()
        {
            CountryTimeStampResponseModel respModel = new CountryTimeStampResponseModel();
            CancellationTokenSource token = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
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
                    Debug.WriteLine("Exception in GetItemsService/GetCountryTimestampItem: " + e.Message);
                }
                return respModel;
            }, token.Token);
            if (task.Wait(timeSpan))
            {
                return task.Result;
            }
            else
            {
                token.Cancel();
                return respModel;
            }
        }
    }
}
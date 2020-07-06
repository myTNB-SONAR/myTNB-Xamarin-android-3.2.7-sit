using myTNB.SitecoreCM.Services;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Service;
using myTNB_Android.Src.SitecoreCMS.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB.SitecoreCMS.Services
{
    public class GetItemsService
    {
        private static string OS { get; set; }
        private static string ImageSize { get; set; }
        private static string WebsiteUrl { get; set; }
        private static string Language { get; set; }

        private readonly TimeSpan timeSpan = TimeSpan.FromMilliseconds(5000);

        public GetItemsService(string os, string imageSize, string websiteUrl, string language = "en")
        {
            OS = os;
            ImageSize = imageSize;
            WebsiteUrl = websiteUrl;
            Language = language;
        }

        public WalkthroughScreensResponseModel GetWalkthroughScreenItems()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            WalkthroughScreensResponseModel respModel = new WalkthroughScreensResponseModel();
            var task = Task.Run(() =>
            {
                try
                {
                    WalkthroughScreenService service = new WalkthroughScreenService();
                    var data = service.GetWalkthroughScreens(OS, ImageSize, WebsiteUrl, Language);
                    var resp = CheckData(data.ToList<object>());
                    string serializedObj = JsonConvert.SerializeObject(resp);
                    respModel = JsonConvert.DeserializeObject<WalkthroughScreensResponseModel>(serializedObj);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception in GetItemsService/WalkthroughScreens: " + e.Message);
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

        public PreLoginPromoResponseModel GetPreLoginPromoItem()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            PreLoginPromoResponseModel respModel = new PreLoginPromoResponseModel();

            var task = Task.Run(() =>
            {
                try
                {
                    PreLoginPromoService service = new PreLoginPromoService();
                    var data = service.GetPreLoginPromo(OS, ImageSize, WebsiteUrl, Language);
                    var listData = AddDataToList(data);
                    var resp = CheckData(listData);
                    string serializedObj = JsonConvert.SerializeObject(resp);
                    respModel = JsonConvert.DeserializeObject<PreLoginPromoResponseModel>(serializedObj);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception in GetItemsService/PreLoginPromo: " + e.Message);
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

        public FullRTEPagesResponseModel GetFullRTEPagesItems()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            FullRTEPagesResponseModel respModel = new FullRTEPagesResponseModel();

            var task = Task.Run(() =>
            {
                try
                {
                    FullRTEPagesService service = new FullRTEPagesService();
                    var data = service.GetFullRTEPages(WebsiteUrl, Language);
                    var resp = CheckData(data.ToList<object>());
                    string serializedObj = JsonConvert.SerializeObject(resp);
                    respModel = JsonConvert.DeserializeObject<FullRTEPagesResponseModel>(serializedObj);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception in GetItemsService/FullRTEPages: " + e.Message);
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

        public FAQsResponseModel GetFAQsItem()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            FAQsResponseModel respModel = new FAQsResponseModel();

            var task = Task.Run(() =>
            {
                try
                {
                    FAQsService service = new FAQsService();
                    var data = service.GetFAQsService(OS, ImageSize, WebsiteUrl, Language);
                    var resp = CheckData(data.ToList<object>());
                    string serializedObj = JsonConvert.SerializeObject(resp);
                    respModel = JsonConvert.DeserializeObject<FAQsResponseModel>(serializedObj);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception in GetItemsService/GetFAQsItem: " + e.Message);
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
        public FAQsParentResponseModel GetFAQsTimestampItem()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            FAQsParentResponseModel respModel = new FAQsParentResponseModel();

            var task = Task.Run(() =>
            {
                try
                {
                    FAQsService service = new FAQsService();
                    var data = service.GetTimestamp(WebsiteUrl, Language);
                    var listData = AddDataToList(data);
                    var resp = CheckData(listData);
                    string serializedObj = JsonConvert.SerializeObject(resp);
                    respModel = JsonConvert.DeserializeObject<FAQsParentResponseModel>(serializedObj);
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

        public TimestampResponseModel GetTimestampItem()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            TimestampResponseModel respModel = new TimestampResponseModel();

            var task = Task.Run(() =>
            {
                try
                {
                    TimestampService service = new TimestampService();
                    var data = service.GetTimestamp(WebsiteUrl, Language);
                    var listData = AddDataToList(data);
                    var resp = CheckData(listData);
                    string serializedObj = JsonConvert.SerializeObject(resp);
                    respModel = JsonConvert.DeserializeObject<TimestampResponseModel>(serializedObj);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception in GetItemsService/GetTimestampItem: " + e.Message);
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


        public AppLaunchResponseModel GetAppLaunchItem()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            AppLaunchResponseModel respModel = new AppLaunchResponseModel();

            var task = Task.Run(() =>
            {
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
        public HelpResponseModel GetHelpItems()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            HelpResponseModel respModel = new HelpResponseModel();

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
                    Debug.WriteLine("Exception in GetItemsService/HelpResponseModel: " + e.Message);
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
            CancellationTokenSource token = new CancellationTokenSource();
            HelpTimeStampResponseModel respModel = new HelpTimeStampResponseModel();

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

        public ApplySSMRResponseModel GetApplySSMRWalkthroughItems()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            ApplySSMRResponseModel respModel = new ApplySSMRResponseModel();

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
            CancellationTokenSource token = new CancellationTokenSource();
            ApplySSMRTimeStampResponseModel respModel = new ApplySSMRTimeStampResponseModel();

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

        public SSMRMeterReadingResponseModel GetSSMRMeterReadingOnePhaseWalkthroughItems()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            SSMRMeterReadingResponseModel respModel = new SSMRMeterReadingResponseModel();

            var task = Task.Run(() =>
            {
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

        public SSMRMeterReadingTimeStampResponseModel GetSSMRMeterReadingOnePhaseWalkthroughTimestampItem()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            SSMRMeterReadingTimeStampResponseModel respModel = new SSMRMeterReadingTimeStampResponseModel();

            var task = Task.Run(() =>
            {
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

        public SSMRMeterReadingResponseModel GetSSMRMeterReadingOnePhaseOCROffWalkthroughItems()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            SSMRMeterReadingResponseModel respModel = new SSMRMeterReadingResponseModel();

            var task = Task.Run(() =>
            {
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

        public SSMRMeterReadingTimeStampResponseModel GetSSMRMeterReadingOnePhaseOCROffWalkthroughTimestampItem()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            SSMRMeterReadingTimeStampResponseModel respModel = new SSMRMeterReadingTimeStampResponseModel();

            var task = Task.Run(() =>
            {
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

        public SSMRMeterReadingResponseModel GetSSMRMeterReadingThreePhaseWalkthroughItems()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            SSMRMeterReadingResponseModel respModel = new SSMRMeterReadingResponseModel();

            var task = Task.Run(() =>
            {
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

        public SSMRMeterReadingTimeStampResponseModel GetSSMRMeterReadingThreePhaseWalkthroughTimestampItem()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            SSMRMeterReadingTimeStampResponseModel respModel = new SSMRMeterReadingTimeStampResponseModel();

            var task = Task.Run(() =>
            {
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

        public SSMRMeterReadingResponseModel GetSSMRMeterReadingThreePhaseOCROffWalkthroughItems()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            SSMRMeterReadingResponseModel respModel = new SSMRMeterReadingResponseModel();

            var task = Task.Run(() =>
            {
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

        public SSMRMeterReadingTimeStampResponseModel GetSSMRMeterReadingThreePhaseOCROffWalkthroughTimestampItem()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            SSMRMeterReadingTimeStampResponseModel respModel = new SSMRMeterReadingTimeStampResponseModel();

            var task = Task.Run(() =>
            {
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

        public EnergySavingTipsResponseModel GetEnergySavingTipsItem()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            EnergySavingTipsResponseModel respModel = new EnergySavingTipsResponseModel();

            var task = Task.Run(() =>
            {
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

        public EnergySavingTipsTimeStampResponseModel GetEnergySavingTipsTimestampItem()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            EnergySavingTipsTimeStampResponseModel respModel = new EnergySavingTipsTimeStampResponseModel();

            var task = Task.Run(() =>
            {
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
            CancellationTokenSource token = new CancellationTokenSource();
            BillDetailsTooltipResponseModel respModel = new BillDetailsTooltipResponseModel();

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

        public AppLaunchTimeStampResponseModel GetAppLaunchTimestampItem()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            AppLaunchTimeStampResponseModel respModel = new AppLaunchTimeStampResponseModel();

            var task = Task.Run(() =>
            {
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
            CancellationTokenSource token = new CancellationTokenSource();
            BillDetailsTooltipTimeStampResponseModel respModel = new BillDetailsTooltipTimeStampResponseModel();

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

        public RewardsResponseModel GetRewardsItems()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            RewardsResponseModel respModel = new RewardsResponseModel();

            var task = Task.Run(() =>
            {
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

        public RewardsTimeStampResponseModel GetRewardsTimestampItem()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            RewardsTimeStampResponseModel respModel = new RewardsTimeStampResponseModel();

            var task = Task.Run(() =>
            {
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
            CancellationTokenSource token = new CancellationTokenSource();
            WhatsNewResponseModel respModel = new WhatsNewResponseModel();

            var task = Task.Run(() =>
            {
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

        public WhatsNewTimeStampResponseModel GetWhatsNewTimestampItem()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            WhatsNewTimeStampResponseModel respModel = new WhatsNewTimeStampResponseModel();

            var task = Task.Run(() =>
            {
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

        public LanguageResponseModel GetLanguageItems()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            LanguageResponseModel respModel = new LanguageResponseModel();

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
            CancellationTokenSource token = new CancellationTokenSource();
            LanguageTimeStampResponseModel respModel = new LanguageTimeStampResponseModel();

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

        public CountryResponseModel GetCountryItems()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            CountryResponseModel respModel = new CountryResponseModel();

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

        public CountryTimeStampResponseModel GetCountryTimestampItem()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            CountryTimeStampResponseModel respModel = new CountryTimeStampResponseModel();

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

        public EppToolTipResponseModel GetEppToolTipItem()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            EppToolTipResponseModel respModel = new EppToolTipResponseModel();

            var task = Task.Run(() =>
            {
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

       
        public EppToolTipTimeStampResponseModel GetEppToolTipTimeStampItem()
        {
            CancellationTokenSource token = new CancellationTokenSource();
            EppToolTipTimeStampResponseModel respModel = new EppToolTipTimeStampResponseModel();

            var task = Task.Run(() =>
            {
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

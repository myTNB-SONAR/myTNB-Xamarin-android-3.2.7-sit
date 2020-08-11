using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Util;
using myTNB.SitecoreCMS.Extensions;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.SiteCore;
using myTNB_Android.Src.Utils;
using Sitecore.MobileSDK.API.Items;
using Sitecore.MobileSDK.API.Request.Parameters;

namespace myTNB.SitecoreCMS.Services
{
  
    public class HowDoesCopyOfIdentificationService
    {

        private string _os, _imgSize, _websiteURL, _language;

        internal HowDoesCopyOfIdentificationService(string os, string imageSize, string websiteUrl = null, string language = "en")
        {
            _os = os;
            _imgSize = imageSize;
            _websiteURL = websiteUrl;
            _language = language;
        }

        internal List<HowDoesCopyOfIdentificationModel> GetItems()
        {
            SitecoreService sitecoreService = new SitecoreService();
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.HowDoesCopyOfIdentification
                , PayloadType.Content, new List<ScopeType> { ScopeType.Children }, SiteCoreConfig.FiveSecondTimeSpan, _websiteURL, _language);
            var item = req.Result;
            var list = ParseToChildrenItems(item);
            var itemList = list.Result;
            return itemList.ToList();
        }

        internal HowDoesCopyOfIdentificationTimeStamp GetTimeStamp()
        {
            SitecoreService sitecoreService = new SitecoreService();
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.HowDoesCopyOfIdentification
                , PayloadType.Content, new List<ScopeType> { ScopeType.Self }, SiteCoreConfig.FiveSecondTimeSpan, _websiteURL, _language);
            var item = req.Result;
            var list = ParseToTimestamp(item);
            var itemList = list.Result;
            return itemList;
        }

        private async Task<IEnumerable<HowDoesCopyOfIdentificationModel>> ParseToChildrenItems(ScItemsResponse itemsResponse)
        {
            List<HowDoesCopyOfIdentificationModel> list = new List<HowDoesCopyOfIdentificationModel>();
            try
            {
                for (int i = 0; i < itemsResponse.ResultCount; i++)
                {
                    ISitecoreItem item = itemsResponse[i];
                    if (item == null)
                    {
                        continue;
                    }

                    Bitmap imageCache = null;
                    String stringCache = null;

                    stringCache = item.GetImageUrlFromMediaField(Constants.Sitecore.Fields.HowDoesCopyOfIdentification.Image, _websiteURL, false);
                    imageCache = ImageUtils.GetImageBitmapFromUrl(stringCache);

                    list.Add(new HowDoesCopyOfIdentificationModel
                    {
                        ID = item.Id,
                        PopUpTitle = item.GetValueFromField(Constants.Sitecore.Fields.HowDoesCopyOfIdentification.PopUpTitle),
                        PopUpBody = item.GetValueFromField(Constants.Sitecore.Fields.HowDoesCopyOfIdentification.PopUpBody),
                        Image = stringCache,
                        ImageBase64 = BitmapToBase64(imageCache)
                    });
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in HowDoesCopyOfIdentificationService/ParseToChildrenItems: " + e.Message);
            }
            return list;
        }

        public string BitmapToBase64(Bitmap bitmap)
        {
            string B64Output = "";
            try
            {
                MemoryStream byteArrayOutputStream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Png, 100, byteArrayOutputStream);
                byte[] byteArray = byteArrayOutputStream.ToArray();
                B64Output = Base64.EncodeToString(byteArray, Base64Flags.Default);
            }
            catch (Exception e)
            {
                B64Output = "";
                Utility.LoggingNonFatalError(e);
            }

            return B64Output;
        }

        private async Task<HowDoesCopyOfIdentificationTimeStamp> ParseToTimestamp(ScItemsResponse itemsResponse)
        {
            try
            {
                for (int i = 0; i < itemsResponse.ResultCount; i++)
                {
                    ISitecoreItem item = itemsResponse[i];
                    if (item == null)
                    {
                        continue;
                    }
                    return new HowDoesCopyOfIdentificationTimeStamp
                    {
                        Timestamp = item.GetValueFromField(Constants.Sitecore.Fields.Timestamp.TimestampField),
                        ID = item.Id
                    };
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in HowDoesCopyOfIdentificationTimeStamp/GenerateTimestamp: " + e.Message);
            }
            return new HowDoesCopyOfIdentificationTimeStamp();
        }







    }
}
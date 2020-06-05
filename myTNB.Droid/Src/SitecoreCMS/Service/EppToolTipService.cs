using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using myTNB.SitecoreCMS.Extensions;
using myTNB.SitecoreCMS.Model;
using Sitecore.MobileSDK.API.Items;
using Sitecore.MobileSDK.API.Request.Parameters;

namespace myTNB.SitecoreCMS.Services
{      

    /// <summary>
    /// syahmi sto add
    /// </summary>
    public class EppToolTipService
    {

        private string _os, _imgSize, _websiteURL, _language;

        internal EppToolTipService(string os, string imageSize, string websiteUrl = null, string language = "en")
        {
            _os = os;
            _imgSize = imageSize;
            _websiteURL = websiteUrl;
            _language = language;
        }

        internal List<EppToolTipModel> GetItems()
        {
            SitecoreService sitecoreService = new SitecoreService();
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.EppToolTip
                , PayloadType.Content, new List<ScopeType> { ScopeType.Children }, _websiteURL, _language);
            var item = req.Result;
            var list = ParseToChildrenItems(item);
            var itemList = list.Result;
            return itemList.ToList();
        }

        internal EppToolTipTimeStamp GetTimeStamp()
        {
            SitecoreService sitecoreService = new SitecoreService();
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.EppToolTip
                , PayloadType.Content, new List<ScopeType> { ScopeType.Self }, _websiteURL, _language);
            var item = req.Result;
            var list = ParseToTimestamp(item);
            var itemList = list.Result;
            return itemList;
        }

        private async Task<IEnumerable<EppToolTipModel>> ParseToChildrenItems(ScItemsResponse itemsResponse)
        {
            List<EppToolTipModel> list = new List<EppToolTipModel>();
            try
            {
                for (int i = 0; i < itemsResponse.ResultCount; i++)
                {
                    ISitecoreItem item = itemsResponse[i];
                    if (item == null)
                    {
                        continue;
                    }

                    list.Add(new EppToolTipModel
                    {   ID = item.Id,
                        Title = item.GetValueFromField(Constants.Sitecore.Fields.EppToolTip.Title),
                        PopUpTitle = item.GetValueFromField(Constants.Sitecore.Fields.EppToolTip.PopUpTitle),
                        PopUpBody = item.GetValueFromField(Constants.Sitecore.Fields.EppToolTip.PopUpBody),
                        Image = item.GetImageUrlFromItemWithSize(Constants.Sitecore.Fields.EppToolTip.Image, _os, _imgSize, _websiteURL, _language).Replace(" ", "%20"),
                       
                    });
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in EppToolTipService/ParseToChildrenItems: " + e.Message);
            }
            return list;
        }

        private async Task<EppToolTipTimeStamp> ParseToTimestamp(ScItemsResponse itemsResponse)
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
                    return new EppToolTipTimeStamp
                    {
                        Timestamp = item.GetValueFromField(Constants.Sitecore.Fields.Timestamp.TimestampField),
                        ID = item.Id
                    };
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in EPPToolTipTimeStamp/GenerateTimestamp: " + e.Message);
            }
            return new EppToolTipTimeStamp();
        }







    }
}
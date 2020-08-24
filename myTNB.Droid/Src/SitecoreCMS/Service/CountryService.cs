using System;
using System.Collections.Generic;
using myTNB.SitecoreCMS.Extensions;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using Sitecore.MobileSDK.API.Items;
using Sitecore.MobileSDK.API.Request.Parameters;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using myTNB_Android.Src.SiteCore;

namespace myTNB.SitecoreCMS.Service
{
    internal class CountryService
    {
		private string _os, _imgSize, _websiteURL, _language;
		internal CountryService(string os, string imageSize, string websiteUrl = null, string language = "en")
		{
			_os = os;
			_imgSize = imageSize;
			_websiteURL = websiteUrl;
			_language = language;
		}

		internal List<Model.CountryModel> GetItems()
		{
			SitecoreService sitecoreService = new SitecoreService();
			var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.Country,
				PayloadType.Content, new List<ScopeType> { ScopeType.Children }, SiteCoreConfig.FiveSecondTimeSpan, _websiteURL, _language);

			var item = req.Result;
			var list = ParseToChildrenItems(item);
			var itemList = list.Result;
			return itemList.ToList();
		}

		internal CountryTimeStamp GetTimeStamp()
		{
			SitecoreService sitecoreService = new SitecoreService();
			var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.Country
                , PayloadType.Content, new List<ScopeType> { ScopeType.Self }, SiteCoreConfig.FiveSecondTimeSpan, _websiteURL, _language);
			var item = req.Result;
			var list = ParseToTimestamp(item);
			var itemList = list.Result;
			return itemList;
		}

		private async Task<IEnumerable<Model.CountryModel>> ParseToChildrenItems(ScItemsResponse itemsResponse)
		{
			List<Model.CountryModel> list = new List<Model.CountryModel>();
			try
			{
				for (int i = 0; i < itemsResponse.ResultCount; i++)
				{
					ISitecoreItem item = itemsResponse[i];
					if (item == null)
					{
						continue;
					}

					list.Add(new Model.CountryModel
                    {
						CountryFile = item.GetFileURLFromFieldName(Constants.Sitecore.Fields.Language.LanguageFile, _websiteURL),
						ID = item.Id
					});
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine("Exception in CountryService/GetChildren: " + e.Message);
			}
			return list;
		}

		private int GetIntFromStringValue(string val)
		{
			int parsedValue;
			Int32.TryParse(val, out parsedValue);
			return parsedValue;
		}

		private async Task<CountryTimeStamp> ParseToTimestamp(ScItemsResponse itemsResponse)
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
					return new CountryTimeStamp
                    {
						Timestamp = item.GetValueFromField(Constants.Sitecore.Fields.Timestamp.TimestampField),
						ID = item.Id
					};
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine("Exception in LanguageService/GenerateTimestamp: " + e.Message);
			}
			return new CountryTimeStamp();
		}
	}
}

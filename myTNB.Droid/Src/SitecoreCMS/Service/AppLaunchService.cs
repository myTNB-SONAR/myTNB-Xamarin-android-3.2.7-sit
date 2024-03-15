using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using myTNB.SitecoreCMS.Extensions;
using myTNB.SitecoreCMS.Model;
using myTNB.AndroidApp.Src.SiteCore;
using Sitecore.MobileSDK.API.Items;
using Sitecore.MobileSDK.API.Request.Parameters;

namespace myTNB.SitecoreCMS.Services
{
	public class AppLaunchService
	{
		private string _os, _imgSize, _websiteURL, _language;
		internal AppLaunchService(string os, string imageSize, string websiteUrl = null, string language = "en")
		{
			_os = os;
			_imgSize = imageSize;
			_websiteURL = websiteUrl;
			_language = language;
		}

		internal List<AppLaunchModel> GetItems()
		{
			SitecoreService sitecoreService = new SitecoreService();
			var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.AppLaunch
				, PayloadType.Content, new List<ScopeType> { ScopeType.Children }, SiteCoreConfig.FiveSecondTimeSpan, _websiteURL, _language);
			var item = req.Result;
			var list = ParseToChildrenItems(item);
			var itemList = list.Result;
			return itemList.ToList();
		}

		internal AppLaunchTimeStamp GetTimeStamp()
		{
			SitecoreService sitecoreService = new SitecoreService();
			var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.AppLaunch
                , PayloadType.Content, new List<ScopeType> { ScopeType.Self }, SiteCoreConfig.FiveSecondTimeSpan, _websiteURL, _language);
			var item = req.Result;
			var list = ParseToTimestamp(item);
			var itemList = list.Result;
			return itemList;
		}

		private async Task<IEnumerable<AppLaunchModel>> ParseToChildrenItems(ScItemsResponse itemsResponse)
		{
			List<AppLaunchModel> list = new List<AppLaunchModel>();
			try
			{
				for (int i = 0; i < itemsResponse.ResultCount; i++)
				{
					ISitecoreItem item = itemsResponse[i];
					if (item == null)
					{
						continue;
					}

                    list.Add(new AppLaunchModel
                    {
						Title = item.GetValueFromField(Constants.Sitecore.Fields.AppLaunch.Title),
						Description = item.GetValueFromField(Constants.Sitecore.Fields.AppLaunch.Description),
						Image = item.GetImageUrlFromItemWithSize(Constants.Sitecore.Fields.AppLaunch.Image, _os, _imgSize, _websiteURL, _language).Replace(" ", "%20"),
                        StartDateTime = item.GetValueFromField(Constants.Sitecore.Fields.AppLaunch.StartDateTime),
                        EndDateTime = item.GetValueFromField(Constants.Sitecore.Fields.AppLaunch.EndDateTime),
                        ShowForSeconds = item.GetValueFromField(Constants.Sitecore.Fields.AppLaunch.ShowForSeconds),
						ID = item.Id
					});
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine("Exception in EnergySavingTipsService/GetChildren: " + e.Message);
			}
			return list;
		}

		private int GetIntFromStringValue(string val)
		{
			int parsedValue;
			Int32.TryParse(val, out parsedValue);
			return parsedValue;
		}

		private async Task<AppLaunchTimeStamp> ParseToTimestamp(ScItemsResponse itemsResponse)
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
					return new AppLaunchTimeStamp
                    {
						Timestamp = item.GetValueFromField(Constants.Sitecore.Fields.Timestamp.TimestampField),
						ID = item.Id
					};
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine("Exception in EnergySavingTipsService/GenerateTimestamp: " + e.Message);
			}
			return new AppLaunchTimeStamp();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using myTNB.SitecoreCMS.Extensions;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Utils;
using Sitecore.MobileSDK.API.Items;
using Sitecore.MobileSDK.API.Request.Parameters;

namespace myTNB.SitecoreCMS.Services
{
	public class WhatsNewService
	{
		private string _os, _imgSize, _websiteURL, _language;
		internal WhatsNewService(string os, string imageSize, string websiteUrl = null, string language = "en")
		{
			_os = os;
			_imgSize = imageSize;
			_websiteURL = websiteUrl;
			_language = language;
		}

		internal List<WhatsNewCategoryModel> GetItems()
		{
			SitecoreService sitecoreService = new SitecoreService();
			var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.WhatsNew
				, PayloadType.Content, new List<ScopeType> { ScopeType.Children }, _websiteURL, _language);
			var item = req.Result;
			var list = ParseItems(item);
			var itemList = list.Result;
			return itemList.ToList();
		}

		internal List<WhatsNewModel> GetChildrenItems(string path, WhatsNewCategoryModel whatsNewCategory)
		{
			try
			{
				SitecoreService sitecoreService = new SitecoreService();
				var req = sitecoreService.GetItemByPath(path
					, PayloadType.Content, new List<ScopeType> { ScopeType.Children }, _websiteURL, _language);
				var item = req.Result;
				var list = ParseToChildrenItems(item, whatsNewCategory);
				var itemList = list.Result;
				return itemList.ToList();
			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
			return new List<WhatsNewModel>();
		}

		internal WhatsNewTimeStamp GetTimeStamp()
		{
			SitecoreService sitecoreService = new SitecoreService();
			var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.WhatsNew
                , PayloadType.Content, new List<ScopeType> { ScopeType.Self }, _websiteURL, _language);
			var item = req.Result;
			var list = ParseToTimestamp(item);
			var itemList = list.Result;
			return itemList;
		}

		private async Task<IEnumerable<WhatsNewCategoryModel>> ParseItems(ScItemsResponse itemsResponse)
		{
			List<WhatsNewCategoryModel> list = new List<WhatsNewCategoryModel>();
			try
			{
				for (int i = 0; i < itemsResponse.ResultCount; i++)
				{
					ISitecoreItem item = itemsResponse[i];
					if (item == null || string.IsNullOrEmpty(item.Id) || string.IsNullOrEmpty(item.Path) || string.IsNullOrEmpty(item.DisplayName))
					{
						continue;
					}

					WhatsNewCategoryModel whatsNewCategory = new WhatsNewCategoryModel()
					{
						CategoryName = item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.WhatsNewCategory),
						ID = item.Id
					};
					List<WhatsNewModel> whatsNewList = GetChildrenItems(item.Path, whatsNewCategory);

					if (whatsNewList != null && whatsNewList.Count > 0)
					{
                        whatsNewCategory.WhatsNewList = whatsNewList;
						list.Add(whatsNewCategory);
					}
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine("Exception in WhatsNewService/GetMainChildren: " + e.Message);
			}
			return list;
		}

		private async Task<IEnumerable<WhatsNewModel>> ParseToChildrenItems(ScItemsResponse itemsResponse, WhatsNewCategoryModel whatsNewCategory)
		{
			List<WhatsNewModel> list = new List<WhatsNewModel>();
			try
			{
				for (int i = 0; i < itemsResponse.ResultCount; i++)
				{
					ISitecoreItem item = itemsResponse[i];
					if (item == null || string.IsNullOrEmpty(item.Id) || string.IsNullOrEmpty(item.DisplayName))
					{
						continue;
					}
					list.Add(new WhatsNewModel
					{
						CategoryName = whatsNewCategory.CategoryName,
						CategoryID = whatsNewCategory.ID,
						TitleOnListing = item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.TitleOnListing),
						StartDate = item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.StartDate),
						EndDate = item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.EndDate),
                        PublishDate = item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.PublishDate),
                        Title = item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.Title),
						Description = item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.Description),
						Image = item.GetImageUrlFromMediaField(Constants.Sitecore.Fields.WhatsNew.Image, _websiteURL, false),
						CTA = item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.CTA),
						ID = item.Id
					});
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine("Exception in WhatsNewService/GetChildren: " + e.Message);
			}
			return list;
		}

		private async Task<WhatsNewTimeStamp> ParseToTimestamp(ScItemsResponse itemsResponse)
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
					return new WhatsNewTimeStamp
					{
						Timestamp = item.GetValueFromField(Constants.Sitecore.Fields.Timestamp.TimestampField),
						ID = item.Id
					};
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine("Exception in WhatsNewService/GenerateTimestamp: " + e.Message);
			}
			return new WhatsNewTimeStamp();
		}
	}
}
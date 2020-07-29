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

					WhatsNewModel newItem = new WhatsNewModel();

					newItem = new WhatsNewModel
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
						Image_DetailsView = item.GetImageUrlFromMediaField(Constants.Sitecore.Fields.WhatsNew.Image_DetailsView, _websiteURL, false),
						Styles_DetailsView = item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.Styles_DetailsView),
						PortraitImage_PopUp = item.GetImageUrlFromMediaField(Constants.Sitecore.Fields.WhatsNew.PortraitImage_PopUp, _websiteURL, false),
						PopUp_HeaderImage = item.GetImageUrlFromMediaField(Constants.Sitecore.Fields.WhatsNew.PopUp_HeaderImage, _websiteURL, false),
						PopUp_Text_Content = item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.PopUp_Text_Content),
						Infographic_FullView_URL = item.GetFileURLFromFieldName(Constants.Sitecore.Fields.WhatsNew.Infographic_FullView_URL, _websiteURL),
						ID = item.Id
					};

					try
                    {
						newItem.ShowEveryCountDays_PopUp = !string.IsNullOrEmpty(item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.ShowEveryCountDays_PopUp)) ? int.Parse(item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.ShowEveryCountDays_PopUp)) : -1;
					}
					catch (Exception ex)
					{
						newItem.ShowEveryCountDays_PopUp = -1;
					}
					try
					{
						newItem.ShowForTotalCountDays_PopUp = !string.IsNullOrEmpty(item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.ShowForTotalCountDays_PopUp)) ? int.Parse(item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.ShowForTotalCountDays_PopUp)) : 0;
					}
					catch (Exception ex)
					{
						newItem.ShowForTotalCountDays_PopUp = 0;
					}
					try
					{
						newItem.ShowAtAppLaunchPopUp = (item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.ShowAtAppLaunchPopUp).ToUpper().Trim() == "1" || item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.ShowAtAppLaunchPopUp).ToUpper().Trim() == "TRUE") ? true : false;
					}
					catch (Exception ex)
					{
						newItem.ShowAtAppLaunchPopUp = false;
					}

					try
					{
						newItem.Disable_DoNotShow_Checkbox = (item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.Disable_DoNotShow_Checkbox).ToUpper().Trim() == "1" || item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.Disable_DoNotShow_Checkbox).ToUpper().Trim() == "TRUE") ? true : false;
					}
					catch (Exception ex)
					{
						newItem.Disable_DoNotShow_Checkbox = false;
					}

					try
					{
						newItem.PopUp_Text_Only = (item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.PopUp_Text_Only).ToUpper().Trim() == "1" || item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.PopUp_Text_Only).ToUpper().Trim() == "TRUE") ? true : false;
					}
					catch (Exception ex)
					{
						newItem.PopUp_Text_Only = false;
					}

					try
					{
						newItem.Donot_Show_In_WhatsNew = (item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.Donot_Show_In_WhatsNew).ToUpper().Trim() == "1" || item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.Donot_Show_In_WhatsNew).ToUpper().Trim() == "TRUE") ? true : false;
					}
					catch (Exception ex)
					{
						newItem.Donot_Show_In_WhatsNew = false;
					}

					if (newItem.Description.Contains("<img"))
                    {
						string urlRegex = @"<img[^>]*?src\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*?>";
						System.Text.RegularExpressions.MatchCollection matchesImgSrc = System.Text.RegularExpressions.Regex.Matches(newItem.Description, urlRegex, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
						foreach (System.Text.RegularExpressions.Match m in matchesImgSrc)
						{
							string href = m.Groups[1].Value;
							if (!href.Contains("http"))
                            {
								href = item.GetImageUrlFromExtractedUrl(m.Groups[1].Value, _websiteURL);
							}
							newItem.Description = newItem.Description.Replace(m.Groups[1].Value, href);
						}
					}

					list.Add(newItem);
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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Android.Content;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.myTNBMenu.Fragments.WhatsNewMenu.MVP
{
	public class WhatsNewItemPresenter : WhatsNewItemContract.IWhatsNewItemPresenter
    {
        WhatsNewItemContract.IWhatsNewItemView mView;

		private ISharedPreferences mPref;

		private WhatsNewEntity mWhatsNewEntity;

		// private RewardServiceImpl mApi;

		public WhatsNewItemPresenter(WhatsNewItemContract.IWhatsNewItemView view, ISharedPreferences pref)
		{
			this.mView = view;
			this.mPref = pref;
			// this.mApi = new RewardServiceImpl();
		}


		public List<WhatsNewModel> InitializeWhatsNewList()
		{
			List<WhatsNewModel> list = new List<WhatsNewModel>();

			list.Add(new WhatsNewModel()
			{
				TitleOnListing = "",
				Image = ""
			});

			list.Add(new WhatsNewModel()
			{
				TitleOnListing = "",
				Image = ""
			});

			list.Add(new WhatsNewModel()
			{
				TitleOnListing = "",
				Image = ""
			});

			return list;
		}

		public List<WhatsNewModel> GetActiveWhatsNewList()
		{
			List<WhatsNewModel> list = new List<WhatsNewModel>();

			if (mWhatsNewEntity == null)
			{
				mWhatsNewEntity = new WhatsNewEntity();
			}

			List<WhatsNewEntity> mList = mWhatsNewEntity.GetActiveItems();

			if (mList != null && mList.Count > 0)
			{
				foreach (WhatsNewEntity obj in mList)
				{
					WhatsNewModel item = new WhatsNewModel();
					item.ID = obj.ID;
					item.Title = obj.Title;
					item.Image = obj.Image;
					item.ImageB64 = obj.ImageB64;
					item.CategoryID = obj.CategoryID;
					item.Description = obj.Description;
					item.Read = obj.Read;
					item.TitleOnListing = obj.TitleOnListing;
					item.StartDate = obj.StartDate;
					item.EndDate = obj.EndDate;
					item.CTA = obj.CTA;
					list.Add(item);
				}
			}

			return list;
		}

		public List<WhatsNewModel> GetActiveWhatsNewList(string categoryID)
		{
			List<WhatsNewModel> list = new List<WhatsNewModel>();

			if (mWhatsNewEntity == null)
			{
				mWhatsNewEntity = new WhatsNewEntity();
			}

			List<WhatsNewEntity> mList = mWhatsNewEntity.GetActiveItemsByCategory(categoryID);

			if (mList != null && mList.Count > 0)
			{
				foreach (WhatsNewEntity obj in mList)
				{
					WhatsNewModel item = new WhatsNewModel();
					item.ID = obj.ID;
					item.Title = obj.Title;
					item.Image = obj.Image;
					item.ImageB64 = obj.ImageB64;
					item.CategoryID = obj.CategoryID;
					item.Description = obj.Description;
					item.Read = obj.Read;
					item.TitleOnListing = obj.TitleOnListing;
					item.StartDate = obj.StartDate;
					item.EndDate = obj.EndDate;
					item.CTA = obj.CTA;
					list.Add(item);
				}
			}

			return list;
		}

		public void UpdateWhatsNewRead(string itemID, bool flag)
		{
			DateTime currentDate = DateTime.UtcNow;
			WhatsNewEntity wtManager = new WhatsNewEntity();
			CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
			string formattedDate = currentDate.ToString(@"M/d/yyyy h:m:s tt", currCult);
			if (!flag)
			{
				formattedDate = "";

			}
			wtManager.UpdateReadItem(itemID, flag, formattedDate);

			_ = OnUpdateWhatsNew(itemID);
		}

		private async Task OnUpdateWhatsNew(string itemID)
		{
			try
			{
				// Update api calling
                // Whats New TODO: To do update read whats new
				WhatsNewEntity wtManager = new WhatsNewEntity();
				WhatsNewEntity currentItem = wtManager.GetItem(itemID);

				/*UserInterface currentUsrInf = new UserInterface()
				{
					eid = UserEntity.GetActive().Email,
					sspuid = UserEntity.GetActive().UserID,
					did = UserEntity.GetActive().DeviceId,
					ft = FirebaseTokenEntity.GetLatest().FBToken,
					lang = LanguageUtil.GetAppLanguage().ToUpper(),
					sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
					sec_auth_k2 = "",
					ses_param1 = "",
					ses_param2 = ""
				};

				string rewardId = currentItem.ID;
				rewardId = rewardId.Replace("{", "");
				rewardId = rewardId.Replace("}", "");

				AddUpdateRewardModel currentReward = new AddUpdateRewardModel()
				{
					Email = UserEntity.GetActive().Email,
					RewardId = rewardId,
					Read = currentItem.Read,
					ReadDate = !string.IsNullOrEmpty(currentItem.ReadDateTime) ? currentItem.ReadDateTime + " +00:00" : "",
					Favourite = currentItem.IsSaved,
					FavUpdatedDate = !string.IsNullOrEmpty(currentItem.IsSavedDateTime) ? currentItem.IsSavedDateTime + " +00:00" : "",
					Redeemed = currentItem.IsUsed,
					RedeemedDate = !string.IsNullOrEmpty(currentItem.IsUsedDateTime) ? currentItem.IsUsedDateTime + " +00:00" : ""
				};

				AddUpdateRewardRequest request = new AddUpdateRewardRequest()
				{
					usrInf = currentUsrInf,
					reward = currentReward
				};

				AddUpdateRewardResponse response = await this.mApi.AddUpdateReward(request, new System.Threading.CancellationTokenSource().Token);*/

			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
		}
	}
}

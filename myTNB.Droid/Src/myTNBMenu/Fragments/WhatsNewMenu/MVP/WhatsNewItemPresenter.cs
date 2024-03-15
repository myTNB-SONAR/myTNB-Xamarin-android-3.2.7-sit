using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Android.Content;
using myTNB.SitecoreCMS.Model;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.Utils;

namespace myTNB.AndroidApp.Src.myTNBMenu.Fragments.WhatsNewMenu.MVP
{
	public class WhatsNewItemPresenter : WhatsNewItemContract.IWhatsNewItemPresenter
    {
        WhatsNewItemContract.IWhatsNewItemView mView;

		private ISharedPreferences mPref;

		private WhatsNewEntity mWhatsNewEntity;

		public WhatsNewItemPresenter(WhatsNewItemContract.IWhatsNewItemView view, ISharedPreferences pref)
		{
			this.mView = view;
			this.mPref = pref;
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
					item.PublishDate = obj.PublishDate;
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
					item.PublishDate = obj.PublishDate;
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
		}
	}
}

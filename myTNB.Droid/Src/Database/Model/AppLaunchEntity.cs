using myTNB.SitecoreCMS.Model;
using myTNB.Android.Src.Database;
using myTNB.Android.Src.Utils;
using SQLite;
using System;
using System.Collections.Generic;

namespace myTNB.Android.Src.Database.Model
{
	[Table("AppLaunchEntity")]
	public class AppLaunchEntity
    {
		[Unique, Column("ID")]
		public string ID { get; set; }

		[Column("Title")]
		public string Title { set; get; }

		[Column("Description")]
		public string Description { set; get; }

		[Column("Image")]
		public string Image { set; get; }

        [Column("ImageB64")]
        public string ImageB64 { set; get; }

        [Column("StartDateTime")]
        public string StartDateTime { set; get; }

        [Column("EndDateTime")]
        public string EndDateTime { set; get; }

        [Column("ShowForSeconds")]
        public string ShowForSeconds { set; get; }


        public void CreateTable()
		{

			try
			{
				var db = DBHelper.GetSQLiteConnection();
				List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("AppLaunchEntity");
				db.CreateTable<AppLaunchEntity>();
			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
		}

		public void InsertItem(AppLaunchEntity item)
		{
			try
			{
				var db = DBHelper.GetSQLiteConnection();
				db.InsertOrReplace(item);
			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
		}

		public void InsertListOfItems(List<AppLaunchModel> itemList)
		{
			if (itemList != null)
			{
				foreach (AppLaunchModel obj in itemList)
				{
                    AppLaunchEntity item = new AppLaunchEntity();
					item.ID = obj.ID;
					item.Image = obj.Image.Replace(" ", "%20");
                    item.ImageB64 = string.IsNullOrEmpty(obj.ImageB64) ? "" : obj.ImageB64;
                    item.Title = obj.Title;
					item.Description = obj.Description;
                    item.StartDateTime = obj.StartDateTime;
                    item.EndDateTime = obj.EndDateTime;
                    item.ShowForSeconds = obj.ShowForSeconds;
                    InsertItem(item);
				}
			}
		}

		public List<AppLaunchEntity> GetAllItems()
		{
			List<AppLaunchEntity> itemList = new List<AppLaunchEntity>();
			try
			{
				var db = DBHelper.GetSQLiteConnection();
				itemList = db.Query<AppLaunchEntity>("select * from AppLaunchEntity");
                if (itemList == null)
                {
                    itemList = new List<AppLaunchEntity>();
                }
			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
			return itemList;
		}

		public void DeleteTable()
		{
			try
			{
				var db = DBHelper.GetSQLiteConnection();
				db.DeleteAll<AppLaunchEntity>();
			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
		}
	}
}
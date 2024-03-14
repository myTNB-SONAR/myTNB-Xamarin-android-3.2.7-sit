using System;
using System.Collections.Generic;
using myTNB.Android.Src.Utils;
using SQLite;
using myTNB.Android.Src.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Model;

namespace myTNB.Android.Src.Database.Model
{
	[Table("AppLaunchParentEntity")]
	public class AppLaunchParentEntity
	{
		[Unique, Column("ID")]
		public string ID { get; set; }

		[Column("Timestamp")]
		public string Timestamp { set; get; }

		public void CreateTable()
		{
			try
			{
				var db = DBHelper.GetSQLiteConnection();
				List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("AppLaunchParentEntity");
				db.CreateTable<AppLaunchParentEntity>();
			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}

		}

		public void InsertItem(AppLaunchParentEntity item)
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

		public void InsertListOfItems(List<AppLaunchTimeStamp> itemList)
		{
			if (itemList != null)
			{
				foreach (AppLaunchTimeStamp obj in itemList)
				{
                    AppLaunchParentEntity item = new AppLaunchParentEntity();
					item.ID = obj.ID;
					item.Timestamp = obj.Timestamp;
					InsertItem(item);
				}
			}
		}

		public List<AppLaunchParentEntity> GetAllItems()
		{
			List<AppLaunchParentEntity> itemList = new List<AppLaunchParentEntity>();
			try
			{
				var db = DBHelper.GetSQLiteConnection();
				itemList = db.Query<AppLaunchParentEntity>("select * from AppLaunchParentEntity");
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
				db.DeleteAll<AppLaunchParentEntity>();
			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
		}

		public void UpdateItem(AppLaunchParentEntity item)
		{
			try
			{
				var db = DBHelper.GetSQLiteConnection();
				db.Update(item);
			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
		}
	}
}

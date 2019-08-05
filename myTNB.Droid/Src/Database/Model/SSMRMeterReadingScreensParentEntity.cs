using System;
using System.Collections.Generic;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using SQLite;
using myTNB_Android.Src.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Model;

namespace myTNB_Android.Src.Database.Model
{
	[Table("SSMRMeterReadingScreensOnePhaseParentEntity")]
	public class SSMRMeterReadingScreensParentEntity
    {
		[Unique, Column("ID")]
		public string ID { get; set; }

		[Column("Timestamp")]
		public string Timestamp { set; get; }

		public void CreateTable()
		{
			var db = DBHelper.GetSQLiteConnection();
			List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("SSMRMeterReadingScreensOnePhaseParentEntity");
			db.CreateTable<SSMRMeterReadingScreensParentEntity>();
		}

		public void InsertItem(SSMRMeterReadingScreensParentEntity item)
		{
			try
			{
				var db = DBHelper.GetSQLiteConnection();
				int newRecord = db.InsertOrReplace(item);
			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
		}

		public void InsertListOfItems(List<SSMRMeterReadingTimeStamp> itemList)
		{
			if (itemList != null)
			{
				foreach (SSMRMeterReadingTimeStamp obj in itemList)
				{
                    SSMRMeterReadingScreensParentEntity item = new SSMRMeterReadingScreensParentEntity();
					item.ID = obj.ID;
					item.Timestamp = obj.Timestamp;
					InsertItem(item);
				}
			}
		}

		public List<SSMRMeterReadingScreensParentEntity> GetAllItems()
		{
			List<SSMRMeterReadingScreensParentEntity> itemList = new List<SSMRMeterReadingScreensParentEntity>();
			try
			{
				var db = DBHelper.GetSQLiteConnection();
				itemList = db.Query<SSMRMeterReadingScreensParentEntity>("select * from SSMRMeterReadingScreensOnePhaseParentEntity");
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
				using (var db = new SQLiteConnection(Constants.DB_PATH))
				{
					db.DeleteAll<SSMRMeterReadingScreensParentEntity>();
				}
			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
		}

		public void UpdateItem(SSMRMeterReadingScreensParentEntity item)
		{
			try
			{
				var db = DBHelper.GetSQLiteConnection();
				int newRecord = db.Update(item);
			}
			catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
				Utility.LoggingNonFatalError(e);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using myTNB.AndroidApp.Src.Utils;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using SQLite;
using myTNB.AndroidApp.Src.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Model;

namespace myTNB.AndroidApp.Src.Database.Model
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
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("SSMRMeterReadingScreensOnePhaseParentEntity");
                db.CreateTable<SSMRMeterReadingScreensParentEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

		}

		public void InsertItem(SSMRMeterReadingScreensParentEntity item)
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
                var db = DBHelper.GetSQLiteConnection();
                db.DeleteAll<SSMRMeterReadingScreensParentEntity>();
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
                db.Update(item);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
		}
	}
}

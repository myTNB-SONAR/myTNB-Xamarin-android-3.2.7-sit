using myTNB.SitecoreCMS.Model;
using myTNB.AndroidApp.Src.Database;
using myTNB.AndroidApp.Src.Utils;
using SQLite;
using System;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.Database.Model
{
    [Table("SSMRMeterReadingScreensOnePhaseEntity")]
    public class SSMRMeterReadingScreensEntity
	{
        [Unique, Column("ID")]
        public string ID { get; set; }

        [Column("Title")]
        public string Title { set; get; }

        [Column("Description")]
        public string Description { set; get; }

        [Column("Image")]
        public string Image { set; get; }


        public void CreateTable()
		{
            
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("SSMRMeterReadingScreensOnePhaseEntity");
                db.CreateTable<SSMRMeterReadingScreensEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
		}

		public void InsertItem(SSMRMeterReadingScreensEntity item)
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

		public void InsertListOfItems(List<SSMRMeterReadingModel> itemList)
		{
			if (itemList != null)
			{
				foreach (SSMRMeterReadingModel obj in itemList)
				{
                    SSMRMeterReadingScreensEntity item = new SSMRMeterReadingScreensEntity();
					item.ID = obj.ID;
					item.Image = obj.Image.Replace(" ", "%20");
					item.Title = obj.Title;
					item.Description = obj.Description;
                    InsertItem(item);
				}
			}
		}

		public List<SSMRMeterReadingScreensEntity> GetAllItems()
		{
			List<SSMRMeterReadingScreensEntity> itemList = new List<SSMRMeterReadingScreensEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<SSMRMeterReadingScreensEntity>("select * from SSMRMeterReadingScreensOnePhaseEntity");
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
                db.DeleteAll<SSMRMeterReadingScreensEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
		}
	}
}
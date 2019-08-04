using System;
using System.Collections.Generic;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using SQLite;
using myTNB_Android.Src.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Model;

namespace myTNB_Android.Src.Database.Model
{
    [Table("SSMRMeterReadingThreePhaseScreensParentEntity")]
    public class SSMRMeterReadingThreePhaseScreensParentEntity
    {
        [Unique, Column("ID")]
        public string ID { get; set; }

        [Column("Timestamp")]
        public string Timestamp { set; get; }

        public void CreateTable()
        {
            var db = DBHelper.GetSQLiteConnection();
            List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("SSMRMeterReadingThreePhaseScreensParentEntity");
            db.CreateTable<SSMRMeterReadingThreePhaseScreensParentEntity>();
        }

        public void InsertItem(SSMRMeterReadingThreePhaseScreensParentEntity item)
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
                    SSMRMeterReadingThreePhaseScreensParentEntity item = new SSMRMeterReadingThreePhaseScreensParentEntity();
                    item.ID = obj.ID;
                    item.Timestamp = obj.Timestamp;
                    InsertItem(item);
                }
            }
        }

        public List<SSMRMeterReadingThreePhaseScreensParentEntity> GetAllItems()
        {
            List<SSMRMeterReadingThreePhaseScreensParentEntity> itemList = new List<SSMRMeterReadingThreePhaseScreensParentEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<SSMRMeterReadingThreePhaseScreensParentEntity>("select * from SSMRMeterReadingThreePhaseScreensParentEntity");
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
                    db.DeleteAll<SSMRMeterReadingThreePhaseScreensParentEntity>();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void UpdateItem(SSMRMeterReadingThreePhaseScreensParentEntity item)
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

using System;
using System.Collections.Generic;
using myTNB.Android.Src.Utils;
using myTNB.Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using SQLite;
using myTNB.Android.Src.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Model;

namespace myTNB.Android.Src.Database.Model
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
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("SSMRMeterReadingThreePhaseScreensParentEntity");
                db.CreateTable<SSMRMeterReadingThreePhaseScreensParentEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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
                var db = DBHelper.GetSQLiteConnection();
                db.DeleteAll<SSMRMeterReadingThreePhaseScreensParentEntity>();
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
                db.Update(item);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}

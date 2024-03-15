using System;
using System.Collections.Generic;
using myTNB.AndroidApp.Src.Utils;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using SQLite;
using myTNB.AndroidApp.Src.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Model;

namespace myTNB.AndroidApp.Src.Database.Model
{
    [Table("SSMRMeterReadingThreePhaseScreensOCROffParentEntity")]
    public class SSMRMeterReadingThreePhaseScreensOCROffParentEntity
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
                List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("SSMRMeterReadingThreePhaseScreensOCROffParentEntity");
                db.CreateTable<SSMRMeterReadingThreePhaseScreensOCROffParentEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void InsertItem(SSMRMeterReadingThreePhaseScreensOCROffParentEntity item)
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
                    SSMRMeterReadingThreePhaseScreensOCROffParentEntity item = new SSMRMeterReadingThreePhaseScreensOCROffParentEntity();
                    item.ID = obj.ID;
                    item.Timestamp = obj.Timestamp;
                    InsertItem(item);
                }
            }
        }

        public List<SSMRMeterReadingThreePhaseScreensOCROffParentEntity> GetAllItems()
        {
            List<SSMRMeterReadingThreePhaseScreensOCROffParentEntity> itemList = new List<SSMRMeterReadingThreePhaseScreensOCROffParentEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<SSMRMeterReadingThreePhaseScreensOCROffParentEntity>("select * from SSMRMeterReadingThreePhaseScreensOCROffParentEntity");
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
                db.DeleteAll<SSMRMeterReadingThreePhaseScreensOCROffParentEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void UpdateItem(SSMRMeterReadingThreePhaseScreensOCROffParentEntity item)
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

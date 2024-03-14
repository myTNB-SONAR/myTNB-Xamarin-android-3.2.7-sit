using System;
using System.Collections.Generic;
using myTNB.Android.Src.Utils;
using myTNB.Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using SQLite;
using myTNB.Android.Src.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Model;

namespace myTNB.Android.Src.Database.Model
{
    [Table("SSMRMeterReadingScreensOnePhaseOCROffParentEntity")]
    public class SSMRMeterReadingScreensOCROffParentEntity
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
                List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("SSMRMeterReadingScreensOnePhaseOCROffParentEntity");
                db.CreateTable<SSMRMeterReadingScreensOCROffParentEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public void InsertItem(SSMRMeterReadingScreensOCROffParentEntity item)
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
                    SSMRMeterReadingScreensOCROffParentEntity item = new SSMRMeterReadingScreensOCROffParentEntity();
                    item.ID = obj.ID;
                    item.Timestamp = obj.Timestamp;
                    InsertItem(item);
                }
            }
        }

        public List<SSMRMeterReadingScreensOCROffParentEntity> GetAllItems()
        {
            List<SSMRMeterReadingScreensOCROffParentEntity> itemList = new List<SSMRMeterReadingScreensOCROffParentEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<SSMRMeterReadingScreensOCROffParentEntity>("select * from SSMRMeterReadingScreensOnePhaseOCROffParentEntity");
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
                db.DeleteAll<SSMRMeterReadingScreensOCROffParentEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void UpdateItem(SSMRMeterReadingScreensOCROffParentEntity item)
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

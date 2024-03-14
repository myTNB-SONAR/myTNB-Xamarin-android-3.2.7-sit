using myTNB.SitecoreCMS.Model;
using myTNB.Android.Src.Database;
using myTNB.Android.Src.Utils;
using SQLite;
using System;
using System.Collections.Generic;

namespace myTNB.Android.Src.Database.Model
{
    [Table("SSMRMeterReadingThreePhaseScreensEntity")]
    public class SSMRMeterReadingThreePhaseScreensEntity
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
                List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("SSMRMeterReadingThreePhaseScreensEntity");
                db.CreateTable<SSMRMeterReadingThreePhaseScreensEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void InsertItem(SSMRMeterReadingThreePhaseScreensEntity item)
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
                    SSMRMeterReadingThreePhaseScreensEntity item = new SSMRMeterReadingThreePhaseScreensEntity();
                    item.ID = obj.ID;
                    item.Image = obj.Image.Replace(" ", "%20");
                    item.Title = obj.Title;
                    item.Description = obj.Description;
                    InsertItem(item);
                }
            }
        }

        public List<SSMRMeterReadingThreePhaseScreensEntity> GetAllItems()
        {
            List<SSMRMeterReadingThreePhaseScreensEntity> itemList = new List<SSMRMeterReadingThreePhaseScreensEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<SSMRMeterReadingThreePhaseScreensEntity>("select * from SSMRMeterReadingThreePhaseScreensEntity");
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
                db.DeleteAll<SSMRMeterReadingThreePhaseScreensEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
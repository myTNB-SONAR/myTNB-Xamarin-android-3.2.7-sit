using System;
using System.Collections.Generic;
using myTNB.Android.Src.SitecoreCMS.Model;
using myTNB.Android.Src.Utils;
using SQLite;

namespace myTNB.Android.Src.Database.Model
{
    [Table("FloatingButtonMarketingParentEntity")]
    public class FloatingButtonMarketingParentEntity
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
                List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("FloatingButtonMarketingParentEntity");
                db.CreateTable<FloatingButtonMarketingParentEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public void InsertItem(FloatingButtonMarketingParentEntity item)
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

        public void InsertListOfItems(List<FloatingButtonMarketingTimeStamp> itemList)
        {
            if (itemList != null)
            {
                foreach (FloatingButtonMarketingTimeStamp obj in itemList)
                {
                    FloatingButtonMarketingParentEntity item = new FloatingButtonMarketingParentEntity();
                    item.ID = obj.ID;
                    item.Timestamp = obj.Timestamp;
                    InsertItem(item);
                }
            }
        }

        public List<FloatingButtonMarketingParentEntity> GetAllItems()
        {
            List<FloatingButtonMarketingParentEntity> itemList = new List<FloatingButtonMarketingParentEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<FloatingButtonMarketingParentEntity>("select * from FloatingButtonMarketingParentEntity");
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
                db.DeleteAll<FloatingButtonMarketingParentEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void UpdateItem(FloatingButtonMarketingParentEntity item)
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



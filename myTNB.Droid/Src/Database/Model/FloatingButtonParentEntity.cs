using System;
using System.Collections.Generic;
using myTNB.Android.Src.SitecoreCMS.Model;
using myTNB.Android.Src.Utils;
using SQLite;

namespace myTNB.Android.Src.Database.Model
{
    [Table("FloatingButtonParentEntity")]
    public class FloatingButtonParentEntity
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
                List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("FloatingButtonParentEntity");
                db.CreateTable<FloatingButtonParentEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public void InsertItem(FloatingButtonParentEntity item)
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

        public void InsertListOfItems(List<FloatingButtonTimeStamp> itemList)
        {
            if (itemList != null)
            {
                foreach (FloatingButtonTimeStamp obj in itemList)
                {
                    FloatingButtonParentEntity item = new FloatingButtonParentEntity();
                    item.ID = obj.ID;
                    item.Timestamp = obj.Timestamp;
                    InsertItem(item);
                }
            }
        }

        public List<FloatingButtonParentEntity> GetAllItems()
        {
            List<FloatingButtonParentEntity> itemList = new List<FloatingButtonParentEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<FloatingButtonParentEntity>("select * from FloatingButtonParentEntity");
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
                db.DeleteAll<FloatingButtonParentEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void UpdateItem(FloatingButtonParentEntity item)
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


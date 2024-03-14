using System;
using System.Collections.Generic;
using myTNB.Android.Src.Utils;
using myTNB.Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using SQLite;
using myTNB.Android.Src.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Model;

namespace myTNB.Android.Src.Database.Model
{
    [Table("EnergySavingTipsParentEntity")]
    public class EnergySavingTipsParentEntity
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
                List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("EnergySavingTipsParentEntity");
                db.CreateTable<EnergySavingTipsParentEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public void InsertItem(EnergySavingTipsParentEntity item)
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

        public void InsertListOfItems(List<EnergySavingTipsTimeStamp> itemList)
        {
            if (itemList != null)
            {
                foreach (EnergySavingTipsTimeStamp obj in itemList)
                {
                    EnergySavingTipsParentEntity item = new EnergySavingTipsParentEntity();
                    item.ID = obj.ID;
                    item.Timestamp = obj.Timestamp;
                    InsertItem(item);
                }
            }
        }

        public List<EnergySavingTipsParentEntity> GetAllItems()
        {
            List<EnergySavingTipsParentEntity> itemList = new List<EnergySavingTipsParentEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<EnergySavingTipsParentEntity>("select * from EnergySavingTipsParentEntity");
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
                db.DeleteAll<EnergySavingTipsParentEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void UpdateItem(EnergySavingTipsParentEntity item)
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

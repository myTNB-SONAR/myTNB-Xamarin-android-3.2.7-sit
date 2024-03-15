using System;
using System.Collections.Generic;
using myTNB.AndroidApp.Src.Utils;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using SQLite;
using myTNB.AndroidApp.Src.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Model;

namespace myTNB.AndroidApp.Src.Database.Model
{
    [Table("RewardsParentEntity")]
    public class RewardsParentEntity
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
                List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("RewardsParentEntity");
                db.CreateTable<RewardsParentEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public void InsertItem(RewardsParentEntity item)
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

        public void InsertListOfItems(List<RewardsTimeStamp> itemList)
        {
            if (itemList != null)
            {
                foreach (RewardsTimeStamp obj in itemList)
                {
                    RewardsParentEntity item = new RewardsParentEntity();
                    item.ID = obj.ID;
                    item.Timestamp = obj.Timestamp;
                    InsertItem(item);
                }
            }
        }

        public List<RewardsParentEntity> GetAllItems()
        {
            List<RewardsParentEntity> itemList = new List<RewardsParentEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<RewardsParentEntity>("select * from RewardsParentEntity");
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
                db.DeleteAll<RewardsParentEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void UpdateItem(RewardsParentEntity item)
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

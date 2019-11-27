using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Database;
using myTNB_Android.Src.Utils;
using SQLite;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.Database.Model
{
    [Table("RewardsCategoryEntity")]
    public class RewardsCategoryEntity
    {
        [Unique, Column("ID")]
        public string ID { get; set; }

        [Column("DisplayName")]
        public string DisplayName { set; get; }

        public void CreateTable()
        {

            try
            {
                var db = DBHelper.GetSQLiteConnection();
                List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("RewardsCategoryEntity");
                db.CreateTable<RewardsCategoryEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void InsertItem(RewardsCategoryEntity item)
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

        public void InsertListOfItems(List<RewardsCategoryModel> itemList)
        {
            if (itemList != null)
            {
                foreach (RewardsCategoryModel obj in itemList)
                {
                    RewardsCategoryEntity item = new RewardsCategoryEntity();
                    item.ID = obj.ID;
                    item.DisplayName = obj.DisplayName;
                    InsertItem(item);
                }
            }
        }

        public List<RewardsCategoryEntity> GetAllItems()
        {
            List<RewardsCategoryEntity> itemList = new List<RewardsCategoryEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<RewardsCategoryEntity>("select * from RewardsCategoryEntity");
                if (itemList == null)
                {
                    itemList = new List<RewardsCategoryEntity>();
                }
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
                db.DeleteAll<RewardsCategoryEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        internal static int Count()
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                var existingRecord = db.Query<RewardsCategoryEntity>("SELECT * FROM RewardsCategoryEntity");

                if (existingRecord != null && existingRecord.Count > 0)
                {
                    return existingRecord.Count;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
            return 0;
        }

        internal static bool HasItem()
        {
            return Count() > 0;
        }
    }
}
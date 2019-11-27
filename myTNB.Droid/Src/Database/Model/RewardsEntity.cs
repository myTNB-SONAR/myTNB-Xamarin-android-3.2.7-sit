using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Database;
using myTNB_Android.Src.Utils;
using SQLite;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.Database.Model
{
    [Table("RewardsEntity")]
    public class RewardsEntity
    {
        [Unique, Column("ID")]
        public string ID { get; set; }

        [Column("CategoryID")]
        public string CategoryID { set; get; }

        [Column("DisplayName")]
        public string DisplayName { set; get; }

        [Column("Description")]
        public string Description { set; get; }

        [Column("Image")]
        public string Image { set; get; }

        [Column("ImageB64")]
        public string ImageB64 { set; get; }

        [Column("Read")]
        public bool Read { set; get; }

        [Column("IsUsed")]
        public bool IsUsed { set; get; }

        public void CreateTable()
        {

            try
            {
                var db = DBHelper.GetSQLiteConnection();
                List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("RewardsEntity");
                db.CreateTable<RewardsEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void InsertItem(RewardsEntity item)
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

        public void InsertListOfItems(List<RewardsModel> itemList)
        {
            if (itemList != null)
            {
                foreach (RewardsModel obj in itemList)
                {
                    RewardsEntity item = new RewardsEntity();
                    item.ID = obj.ID;
                    item.DisplayName = obj.DisplayName;
                    item.Image = obj.Image.Replace(" ", "%20");
                    item.ImageB64 = string.IsNullOrEmpty(obj.ImageB64) ? "" : obj.ImageB64;
                    item.CategoryID = obj.CategoryID;
                    item.Description = obj.Description;
                    item.Read = obj.Read;
                    item.IsUsed = obj.IsUsed;
                    InsertItem(item);
                }
            }
        }

        public List<RewardsEntity> GetAllItems()
        {
            List<RewardsEntity> itemList = new List<RewardsEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<RewardsEntity>("select * from RewardsEntity");
                if (itemList == null)
                {
                    itemList = new List<RewardsEntity>();
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
                db.DeleteAll<RewardsEntity>();
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
                var existingRecord = db.Query<RewardsEntity>("SELECT * FROM RewardsEntity WHERE Read = ? ", false);

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

        internal static bool HasUnread()
        {
            return Count() > 0;
        }

        internal static void RemoveItem(RewardsEntity item)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("Delete from RewardsEntity WHERE ID = ?", item.ID);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
        }

        internal static void UpdateReadItem(RewardsEntity item)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("UPDATE RewardsEntity SET Read = ? WHERE ID = ?", true, item.ID);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
        }

        internal static void UpdateIsUsedItem(RewardsEntity item)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("UPDATE RewardsEntity SET IsUsed = ? WHERE ID = ?", true, item.ID);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
        }
    }
}
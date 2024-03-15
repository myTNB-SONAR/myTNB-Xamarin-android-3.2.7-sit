using myTNB.SitecoreCMS.Model;
using myTNB.AndroidApp.Src.Utils;
using SQLite;
using System;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.Database.Model
{
    [Table("WhatsNewCategoryEntityV4")]
    public class WhatsNewCategoryEntity
    {
        [Unique, Column("ID")]
        public string ID { get; set; }

        [Column("CategoryName")]
        public string CategoryName { set; get; }

        public void CreateTable()
        {

            try
            {
                var db = DBHelper.GetSQLiteConnection();
                List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("WhatsNewCategoryEntityV4");
                db.CreateTable<WhatsNewCategoryEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void InsertItem(WhatsNewCategoryEntity item)
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

        public void InsertListOfItems(List<WhatsNewCategoryModel> itemList)
        {
            if (itemList != null)
            {
                foreach (WhatsNewCategoryModel obj in itemList)
                {
                    WhatsNewCategoryEntity item = new WhatsNewCategoryEntity();
                    item.ID = obj.ID;
                    item.CategoryName = obj.CategoryName;
                    InsertItem(item);
                }
            }
        }

        public List<WhatsNewCategoryEntity> GetAllItems()
        {
            List<WhatsNewCategoryEntity> itemList = new List<WhatsNewCategoryEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<WhatsNewCategoryEntity>("select * from WhatsNewCategoryEntityV4");
                if (itemList == null)
                {
                    itemList = new List<WhatsNewCategoryEntity>();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return itemList;
        }

        public void RemoveItem(string categoryId)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("Delete from WhatsNewCategoryEntityV4 WHERE CategoryID = ?", categoryId);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
        }

        public void DeleteTable()
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.DeleteAll<WhatsNewCategoryEntity>();
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
                var existingRecord = db.Query<WhatsNewCategoryEntity>("SELECT * FROM WhatsNewCategoryEntityV4");

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
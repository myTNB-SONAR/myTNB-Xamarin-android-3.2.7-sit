using myTNB.Android.Src.Utils;
using SQLite;
using System;
using System.Collections.Generic;
using myTNB.Android.Src.SitecoreCMS.Model;

namespace myTNB.Android.Src.Database.Model
{
    [Table("FloatingButtonEntity")]
    public class FloatingButtonEntity
    {
        [Unique, Column("ID")]
        public string ID { get; set; }

        [Column("Title")]
        public string Title { set; get; }

        [Column("Description")]
        public string Description { set; get; }

        [Column("Image")]
        public string Image { set; get; }

        [Column("ImageB64")]
        public string ImageB64 { set; get; }

        [Column("StartDateTime")]
        public string StartDateTime { set; get; }

        [Column("EndDateTime")]
        public string EndDateTime { set; get; }

        [Column("ShowForSeconds")]
        public string ShowForSeconds { set; get; }


        public void CreateTable()
        {

            try
            {
                var db = DBHelper.GetSQLiteConnection();
                List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("FloatingButtonEntity");
                db.CreateTable<FloatingButtonEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void InsertItem(FloatingButtonEntity item)
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

        public void InsertListOfItems(List<FloatingButtonModel> itemList)
        {
            if (itemList != null)
            {
                foreach (FloatingButtonModel obj in itemList)
                {
                    FloatingButtonEntity item = new FloatingButtonEntity();
                    item.ID = obj.ID;
                    item.Image = obj.Image.Replace(" ", "%20");
                    item.ImageB64 = string.IsNullOrEmpty(obj.ImageB64) ? "" : obj.ImageB64;
                    item.Title = obj.Title;
                    item.Description = obj.Description;
                    item.StartDateTime = obj.StartDateTime;
                    item.EndDateTime = obj.EndDateTime;
                    item.ShowForSeconds = obj.ShowForSeconds;
                    InsertItem(item);
                }
            }
        }

        public List<FloatingButtonEntity> GetAllItems()
        {
            List<FloatingButtonEntity> itemList = new List<FloatingButtonEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<FloatingButtonEntity>("select * from FloatingButtonEntity");
                if (itemList == null)
                {
                    itemList = new List<FloatingButtonEntity>();
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
                db.DeleteAll<FloatingButtonEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}


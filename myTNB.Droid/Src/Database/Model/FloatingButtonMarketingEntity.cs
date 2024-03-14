using myTNB.Android.Src.Utils;
using SQLite;
using System;
using System.Collections.Generic;
using myTNB.Android.Src.SitecoreCMS.Model;
using System.Globalization;

namespace myTNB.Android.Src.Database.Model
{
    [Table("FloatingButtonMarketingEntity")]
    public class FloatingButtonMarketingEntity
    {
        [Unique, Column("ID")]
        public string ID { get; set; }

        [Column("Title")]
        public string Title { set; get; }

        [Column("ButtonTitle")]
        public string ButtonTitle { set; get; }

        [Column("Description")]
        public string Description { set; get; }

        [Column("Description_Images")]
        public string Description_Images { set; get; }

        [Column("Infographic_FullView_URL")]
        public string Infographic_FullView_URL { set; get; }

        [Column("Infographic_FullView_URL_ImageB64")]
        public string Infographic_FullView_URL_ImageB64 { set; get; }

        //[Column("Image")]
        //public string Image { set; get; }

        //[Column("ImageB64")]
        //public string ImageB64 { set; get; }


        public void CreateTable()
        {

            try
            {
                var db = DBHelper.GetSQLiteConnection();
                List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("FloatingButtonMarketingEntity");
                db.CreateTable<FloatingButtonMarketingEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void InsertItem(FloatingButtonMarketingEntity item)
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

        public void InsertListOfItems(List<FloatingButtonMarketingModel> itemList)
        {
            if (itemList != null)
            {
                foreach (FloatingButtonMarketingModel obj in itemList)
                {
                    FloatingButtonMarketingEntity item = new FloatingButtonMarketingEntity();
                    item.ID = obj.ID;
                    //item.Image = obj.Image.Replace(" ", "%20");
                    //item.ImageB64 = string.IsNullOrEmpty(obj.ImageB64) ? "" : obj.ImageB64;
                    item.Title = obj.Title;
                    item.Description = obj.Description;
                    item.ButtonTitle = obj.ButtonTitle;
                    item.Description_Images = obj.Description_Images;
                    item.Infographic_FullView_URL = obj.Infographic_FullView_URL;
                    item.Infographic_FullView_URL_ImageB64 = string.IsNullOrEmpty(obj.Infographic_FullView_URL_ImageB64) ? "" : obj.Infographic_FullView_URL_ImageB64;
                    InsertItem(item);
                }
            }
        }

        public List<FloatingButtonMarketingEntity> GetAllItems()
        {
            List<FloatingButtonMarketingEntity> itemList = new List<FloatingButtonMarketingEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<FloatingButtonMarketingEntity>("select * from FloatingButtonMarketingEntity");
                if (itemList == null)
                {
                    itemList = new List<FloatingButtonMarketingEntity>();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return itemList;
        }

        public FloatingButtonMarketingEntity GetItem(string itemID)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                List<FloatingButtonMarketingEntity> itemList = new List<FloatingButtonMarketingEntity>();
                itemList = db.Query<FloatingButtonMarketingEntity>("Select * FROM FloatingButtonMarketingEntity WHERE ID = ?", itemID);
                if (itemList != null && itemList.Count > 0)
                {
                   
                  return itemList[0];
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }

            return null;
        }

        public void DeleteTable()
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.DeleteAll<FloatingButtonMarketingEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void UpdateCacheDescriptionImages(string itemID, string imageJson)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("UPDATE FloatingButtonMarketingEntity SET Description_Images = ? WHERE ID = ?", imageJson, itemID);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
        }

        public void UpdateFullScreenImage(string itemID, string imageB64)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("UPDATE FloatingButtonMarketingEntity SET Infographic_FullView_URL_ImageB64 = ? WHERE ID = ?", imageB64, itemID);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
        }
    }
}


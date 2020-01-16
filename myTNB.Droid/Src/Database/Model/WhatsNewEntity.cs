using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Utils;
using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace myTNB_Android.Src.Database.Model
{
    [Table("WhatsNewEntity")]
    public class WhatsNewEntity
    {
        [Unique, Column("ID")]
        public string ID { get; set; }

        [Column("CategoryID")]
        public string CategoryID { set; get; }

        [Column("Title")]
        public string Title { set; get; }

        [Column("Description")]
        public string Description { set; get; }

        [Column("TitleOnListing")]
        public string TitleOnListing { set; get; }

        [Column("StartDate")]
        public string StartDate { set; get; }

        [Column("EndDate")]
        public string EndDate { set; get; }

        [Column("Image")]
        public string Image { set; get; }

        [Column("ImageB64")]
        public string ImageB64 { set; get; }

        [Column("Read")]
        public bool Read { set; get; }

        [Column("ReadDateTime")]
        public string ReadDateTime { set; get; }

        [Column("CTA")]
        public string CTA { set; get; }

        public void CreateTable()
        {

            try
            {
                var db = DBHelper.GetSQLiteConnection();
                List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("WhatsNewEntity");
                db.CreateTable<WhatsNewEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void InsertItem(WhatsNewEntity item)
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

        public void InsertListOfItems(List<WhatsNewModel> itemList)
        {
            if (itemList != null)
            {
                foreach (WhatsNewModel obj in itemList)
                {
                    WhatsNewEntity item = new WhatsNewEntity();
                    item.ID = obj.ID;
                    item.Title = obj.Title;
                    item.Image = obj.Image.Replace(" ", "%20");
                    item.ImageB64 = string.IsNullOrEmpty(obj.ImageB64) ? "" : obj.ImageB64;
                    item.CategoryID = obj.CategoryID;
                    item.Description = obj.Description;
                    item.Read = obj.Read;
                    item.ReadDateTime = string.IsNullOrEmpty(obj.ReadDateTime) ? "" : obj.ReadDateTime;
                    item.TitleOnListing = obj.TitleOnListing;
                    item.StartDate = obj.StartDate;
                    item.EndDate = obj.EndDate;
                    item.CTA = obj.CTA;
                    InsertItem(item);
                }
            }
        }

        public List<WhatsNewEntity> GetAllItems()
        {
            List<WhatsNewEntity> itemList = new List<WhatsNewEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<WhatsNewEntity>("select * from WhatsNewEntity");
                if (itemList == null)
                {
                    itemList = new List<WhatsNewEntity>();
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
                db.DeleteAll<WhatsNewEntity>();
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
                var existingRecord = db.Query<WhatsNewEntity>("SELECT * FROM WhatsNewEntity WHERE Read = ? ", false);

                if (existingRecord != null && existingRecord.Count > 0)
                {
                    int Count = 0;

                    List<WhatsNewEntity> matchList = existingRecord.FindAll(x =>
                    {
                        int startResult = -1;
                        int endResult = 1;
                        try
                        {
                            if (!string.IsNullOrEmpty(x.StartDate) && !string.IsNullOrEmpty(x.EndDate))
                            {
                                DateTime startDateTime = DateTime.ParseExact(x.StartDate, "yyyyMMddTHHmmss",
                                CultureInfo.InvariantCulture, DateTimeStyles.None);
                                DateTime stopDateTime = DateTime.ParseExact(x.EndDate, "yyyyMMddTHHmmss",
                                    CultureInfo.InvariantCulture, DateTimeStyles.None);
                                DateTime nowDateTime = DateTime.Now;
                                startResult = DateTime.Compare(nowDateTime, startDateTime);
                                endResult = DateTime.Compare(nowDateTime, stopDateTime);
                            }
                        }
                        catch (Exception ne)
                        {
                            Utility.LoggingNonFatalError(ne);
                        }
                        return (startResult >= 0 && endResult <= 0);
                    });

                    if (matchList != null && matchList.Count > 0)
                    {
                        Count = matchList.Count;
                    }

                    return Count;
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

        public List<WhatsNewEntity> GetActiveItems()
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                var existingRecord = db.Query<WhatsNewEntity>("SELECT * FROM WhatsNewEntity");

                if (existingRecord != null && existingRecord.Count > 0)
                {
                    List<WhatsNewEntity> matchList = existingRecord.FindAll(x =>
                    {
                        int startResult = -1;
                        int endResult = 1;
                        try
                        {
                            if (!string.IsNullOrEmpty(x.StartDate) && !string.IsNullOrEmpty(x.EndDate))
                            {
                                DateTime startDateTime = DateTime.ParseExact(x.StartDate, "yyyyMMddTHHmmss",
                                CultureInfo.InvariantCulture, DateTimeStyles.None);
                                DateTime stopDateTime = DateTime.ParseExact(x.EndDate, "yyyyMMddTHHmmss",
                                    CultureInfo.InvariantCulture, DateTimeStyles.None);
                                DateTime nowDateTime = DateTime.Now;
                                startResult = DateTime.Compare(nowDateTime, startDateTime);
                                endResult = DateTime.Compare(nowDateTime, stopDateTime);
                            }
                        }
                        catch (Exception ne)
                        {
                            Utility.LoggingNonFatalError(ne);
                        }
                        return (startResult >= 0 && endResult <= 0);
                    });

                    if (matchList != null && matchList.Count > 0)
                    {
                        return matchList;
                    }

                    return new List<WhatsNewEntity>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
            return new List<WhatsNewEntity>();
        }

        public List<WhatsNewEntity> GetActiveSavedItems()
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                var existingRecord = db.Query<WhatsNewEntity>("SELECT * FROM WhatsNewEntity WHERE IsSaved = ?", true);

                if (existingRecord != null && existingRecord.Count > 0)
                {
                    List<WhatsNewEntity> matchList = existingRecord.FindAll(x =>
                    {
                        int startResult = -1;
                        int endResult = 1;
                        try
                        {
                            if (!string.IsNullOrEmpty(x.StartDate) && !string.IsNullOrEmpty(x.EndDate))
                            {
                                DateTime startDateTime = DateTime.ParseExact(x.StartDate, "yyyyMMddTHHmmss",
                                CultureInfo.InvariantCulture, DateTimeStyles.None);
                                DateTime stopDateTime = DateTime.ParseExact(x.EndDate, "yyyyMMddTHHmmss",
                                    CultureInfo.InvariantCulture, DateTimeStyles.None);
                                DateTime nowDateTime = DateTime.Now;
                                startResult = DateTime.Compare(nowDateTime, startDateTime);
                                endResult = DateTime.Compare(nowDateTime, stopDateTime);
                            }
                        }
                        catch (Exception ne)
                        {
                            Utility.LoggingNonFatalError(ne);
                        }
                        return (startResult >= 0 && endResult <= 0);
                    });

                    if (matchList != null && matchList.Count > 0)
                    {
                        return matchList;
                    }

                    return new List<WhatsNewEntity>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
            return new List<WhatsNewEntity>();
        }

        public List<WhatsNewEntity> GetActiveItemsByCategory(string categoryId)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                var existingRecord = db.Query<WhatsNewEntity>("SELECT * FROM WhatsNewEntity WHERE CategoryID = ?", categoryId);

                if (existingRecord != null && existingRecord.Count > 0)
                {
                    List<WhatsNewEntity> matchList = existingRecord.FindAll(x =>
                    {
                        int startResult = -1;
                        int endResult = 1;
                        try
                        {
                            if (!string.IsNullOrEmpty(x.StartDate) && !string.IsNullOrEmpty(x.EndDate))
                            {
                                DateTime startDateTime = DateTime.ParseExact(x.StartDate, "yyyyMMddTHHmmss",
                                CultureInfo.InvariantCulture, DateTimeStyles.None);
                                DateTime stopDateTime = DateTime.ParseExact(x.EndDate, "yyyyMMddTHHmmss",
                                    CultureInfo.InvariantCulture, DateTimeStyles.None);
                                DateTime nowDateTime = DateTime.Now;
                                startResult = DateTime.Compare(nowDateTime, startDateTime);
                                endResult = DateTime.Compare(nowDateTime, stopDateTime);
                            }
                        }
                        catch (Exception ne)
                        {
                            Utility.LoggingNonFatalError(ne);
                        }
                        return (startResult >= 0 && endResult <= 0);
                    });

                    if (matchList != null && matchList.Count > 0)
                    {
                        return matchList;
                    }

                    return new List<WhatsNewEntity>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
            return new List<WhatsNewEntity>();
        }

        public void RemoveItem(string itemID)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("Delete from WhatsNewEntity WHERE ID = ?", itemID);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
        }

        public void RemoveItemByCategoryId(string categoryId)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("Delete from WhatsNewEntity WHERE CategoryID = ?", categoryId);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
        }

        public WhatsNewEntity GetItem(string itemID)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                List<WhatsNewEntity> itemList = new List<WhatsNewEntity>();
                itemList = db.Query<WhatsNewEntity>("Select * FROM WhatsNewEntity WHERE ID = ?", itemID);
                if (itemList != null && itemList.Count > 0)
                {
                    itemList = itemList.FindAll(x =>
                    {
                        int startResult = -1;
                        int endResult = 1;
                        try
                        {
                            if (!string.IsNullOrEmpty(x.StartDate) && !string.IsNullOrEmpty(x.EndDate))
                            {
                                DateTime startDateTime = DateTime.ParseExact(x.StartDate, "yyyyMMddTHHmmss",
                                CultureInfo.InvariantCulture, DateTimeStyles.None);
                                DateTime stopDateTime = DateTime.ParseExact(x.EndDate, "yyyyMMddTHHmmss",
                                    CultureInfo.InvariantCulture, DateTimeStyles.None);
                                DateTime nowDateTime = DateTime.Now;
                                startResult = DateTime.Compare(nowDateTime, startDateTime);
                                endResult = DateTime.Compare(nowDateTime, stopDateTime);
                            }
                        }
                        catch (Exception ne)
                        {
                            Utility.LoggingNonFatalError(ne);
                        }
                        return (startResult >= 0 && endResult <= 0);
                    });

                    if (itemList != null && itemList.Count > 0)
                    {
                        return itemList[0];
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }

            return null;
        }

        public void UpdateReadItem(string itemID, bool flag, string formattedDate)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("UPDATE WhatsNewEntity SET Read = ? WHERE ID = ?", flag, itemID);

                UpdateReadDateTimeItem(itemID, formattedDate);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
        }

        public void UpdateCacheImage(string itemID, string imageB64)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("UPDATE WhatsNewEntity SET ImageB64 = ? WHERE ID = ?", imageB64, itemID);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
        }

        private void UpdateReadDateTimeItem(string itemID, string datetime)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("UPDATE WhatsNewEntity SET ReadDateTime = ? WHERE ID = ?", datetime, itemID);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
        }

    }
}
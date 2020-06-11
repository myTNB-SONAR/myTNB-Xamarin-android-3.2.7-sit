using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Utils;
using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace myTNB_Android.Src.Database.Model
{
    [Table("WhatsNewEntityV2")]
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

        [Column("PublishDate")]
        public string PublishDate { set; get; }

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

        [Column("Image_DetailsView")]
        public string Image_DetailsView { set; get; }

        [Column("Image_DetailsViewB64")]
        public string Image_DetailsViewB64 { set; get; }

        [Column("Styles_DetailsView")]
        public string Styles_DetailsView { set; get; }

        [Column("Description_Images")]
        public string Description_Images { set; get; }

        [Column("PortraitImage_PopUp")]
        public string PortraitImage_PopUp { set; get; }

        [Column("ShowEveryCountDays_PopUp")]
        public int ShowEveryCountDays_PopUp { set; get; }

        [Column("ShowForTotalCountDays_PopUp")]
        public int ShowForTotalCountDays_PopUp { set; get; }

        [Column("ShowDayDate")]
        public string ShowDayDate { set; get; }

        [Column("ShowDayDateTotal")]
        public int ShowDayDateTotal { set; get; }

        [Column("ShowAtAppLaunchPopUp")]
        public bool ShowAtAppLaunchPopUp { set; get; }

        public void CreateTable()
        {

            try
            {
                var db = DBHelper.GetSQLiteConnection();
                List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("WhatsNewEntityV2");
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
                    item.PublishDate = obj.PublishDate;
                    item.CTA = obj.CTA;
                    item.Image_DetailsView = obj.Image_DetailsView;
                    item.Image_DetailsViewB64 = string.IsNullOrEmpty(obj.Image_DetailsViewB64) ? "" : obj.Image_DetailsViewB64;
                    item.Styles_DetailsView = obj.Styles_DetailsView;
                    item.Description_Images = obj.Description_Images;
                    item.PortraitImage_PopUp = obj.PortraitImage_PopUp;
                    item.ShowEveryCountDays_PopUp = obj.ShowEveryCountDays_PopUp;
                    item.ShowAtAppLaunchPopUp = obj.ShowAtAppLaunchPopUp;
                    item.ShowDayDate = GetCurrentDate();
                    item.ShowDayDateTotal = 0;
                    item.Description_Images = obj.Description_Images;
                    InsertItem(item);
                }
            }
        }

        private string GetCurrentDate()
        {
            DateTime currentDate = DateTime.UtcNow;
            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
            return currentDate.ToString(@"M/d/yyyy h:m:s tt", currCult);
        }

        public List<WhatsNewEntity> GetAllItems()
        {
            List<WhatsNewEntity> itemList = new List<WhatsNewEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<WhatsNewEntity>("select * from WhatsNewEntityV2");
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
                var existingRecord = db.Query<WhatsNewEntity>("SELECT * FROM WhatsNewEntityV2 WHERE Read = ? ", false);

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
                var existingRecord = db.Query<WhatsNewEntity>("SELECT * FROM WhatsNewEntityV2");

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
                var existingRecord = db.Query<WhatsNewEntity>("SELECT * FROM WhatsNewEntityV2 WHERE CategoryID = ?", categoryId);

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
                db.Execute("Delete from WhatsNewEntityV2 WHERE ID = ?", itemID);
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
                db.Execute("Delete from WhatsNewEntityV2 WHERE CategoryID = ?", categoryId);
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
                itemList = db.Query<WhatsNewEntity>("Select * FROM WhatsNewEntityV2 WHERE ID = ?", itemID);
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
                db.Execute("UPDATE WhatsNewEntityV2 SET Read = ? WHERE ID = ?", flag, itemID);

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
                db.Execute("UPDATE WhatsNewEntityV2 SET ImageB64 = ? WHERE ID = ?", imageB64, itemID);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
        }

        public void UpdateCacheDetailImage(string itemID, string imageB64)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("UPDATE WhatsNewEntityV2 SET Image_DetailsViewB64 = ? WHERE ID = ?", imageB64, itemID);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
        }

        public void UpdateCacheDescriptionImages(string itemID, string imageJson)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("UPDATE WhatsNewEntityV2 SET Description_Images = ? WHERE ID = ?", imageJson, itemID);
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
                db.Execute("UPDATE WhatsNewEntityV2 SET ReadDateTime = ? WHERE ID = ?", datetime, itemID);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
        }

    }
}
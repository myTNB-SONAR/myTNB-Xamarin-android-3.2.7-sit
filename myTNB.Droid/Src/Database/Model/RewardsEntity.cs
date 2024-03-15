using myTNB.SitecoreCMS.Model;
using myTNB.AndroidApp.Src.Utils;
using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace myTNB.AndroidApp.Src.Database.Model
{
    [Table("RewardsEntity")]
    public class RewardsEntity
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

        [Column("PeriodLabel")]
        public string PeriodLabel { set; get; }

        [Column("LocationLabel")]
        public string LocationLabel { set; get; }

        [Column("TandCLabel")]
        public string TandCLabel { set; get; }

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

        [Column("IsUsed")]
        public bool IsUsed { set; get; }

        [Column("IsUsedDateTime")]
        public string IsUsedDateTime { set; get; }

        [Column("IsSaved")]
        public bool IsSaved { set; get; }

        [Column("IsSavedDateTime")]
        public string IsSavedDateTime { set; get; }

        [Column("RewardUseWithinTime")]
        public string RewardUseWithinTime { set; get; }

        [Column("RewardUseTitle")]
        public string RewardUseTitle { set; get; }

        [Column("RewardUseDescription")]
        public string RewardUseDescription { set; get; }
        

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
                    item.Title = obj.Title;
                    item.Image = obj.Image.Replace(" ", "%20");
                    item.ImageB64 = string.IsNullOrEmpty(obj.ImageB64) ? "" : obj.ImageB64;
                    item.CategoryID = obj.CategoryID;
                    item.Description = obj.Description;
                    item.Read = obj.Read;
                    item.ReadDateTime = string.IsNullOrEmpty(obj.ReadDateTime) ? "" : obj.ReadDateTime;
                    item.IsUsed = obj.IsUsed;
                    item.IsUsedDateTime = string.IsNullOrEmpty(obj.IsUsedDateTime) ? "" : obj.IsUsedDateTime;
                    item.TitleOnListing = obj.TitleOnListing;
                    item.PeriodLabel = obj.PeriodLabel;
                    item.LocationLabel = obj.LocationLabel;
                    item.TandCLabel = obj.TandCLabel;
                    item.StartDate = obj.StartDate;
                    item.EndDate = obj.EndDate;
                    item.IsSaved = obj.IsSaved;
                    item.IsSavedDateTime = string.IsNullOrEmpty(obj.IsSavedDateTime) ? "" : obj.IsSavedDateTime;
                    item.RewardUseWithinTime = obj.RewardUseWithinTime;
                    item.RewardUseTitle = obj.RewardUseTitle;
                    item.RewardUseDescription = obj.RewardUseDescription;
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
                    int Count = 0;

                    List<RewardsEntity> matchList = existingRecord.FindAll(x =>
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

                        bool isUsedExpired = false;
                        if (x.IsUsed && !string.IsNullOrEmpty(x.IsUsedDateTime))
                        {
                            try
                            {
                                DateTime dateTimeParse = DateTime.Parse(x.IsUsedDateTime, CultureInfo.InvariantCulture);
                                DateTime utcNow = DateTime.UtcNow;
                                if ((utcNow - dateTimeParse).TotalDays > 7)
                                {
                                    isUsedExpired = true;
                                }

                            }
                            catch (Exception ex)
                            {
                                Utility.LoggingNonFatalError(ex);
                            }
                        }

                        return (startResult >= 0 && endResult <= 0 && !isUsedExpired);
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

        public List<RewardsEntity> GetActiveItems()
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                var existingRecord = db.Query<RewardsEntity>("SELECT * FROM RewardsEntity");

                if (existingRecord != null && existingRecord.Count > 0)
                {
                    List<RewardsEntity> matchList = existingRecord.FindAll(x =>
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

                        bool isUsedExpired = false;
                        if (x.IsUsed && !string.IsNullOrEmpty(x.IsUsedDateTime))
                        {
                            try
                            {
                                DateTime dateTimeParse = DateTime.Parse(x.IsUsedDateTime, CultureInfo.InvariantCulture);
                                DateTime utcNow = DateTime.UtcNow;
                                if ((utcNow - dateTimeParse).TotalDays > 7)
                                {
                                    isUsedExpired = true;
                                }
                                
                            }
                            catch (Exception ex)
                            {
                                Utility.LoggingNonFatalError(ex);
                            }
                        }

                        return (startResult >= 0 && endResult <= 0 && !isUsedExpired);
                    });

                    if (matchList != null && matchList.Count > 0)
                    {
                        try
                        {
                            matchList.Sort((x, y) => DateTime.Compare(DateTime.ParseExact(y.StartDate, "yyyyMMddTHHmmss",
                                CultureInfo.InvariantCulture, DateTimeStyles.None), DateTime.ParseExact(x.StartDate, "yyyyMMddTHHmmss",
                                CultureInfo.InvariantCulture, DateTimeStyles.None)));
                        }
                        catch (Exception ex)
                        {
                            Utility.LoggingNonFatalError(ex);
                        }
                        return matchList;
                    }

                    return new List<RewardsEntity>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
            return new List<RewardsEntity>();
        }

        public List<RewardsEntity> GetActiveSavedItems()
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                var existingRecord = db.Query<RewardsEntity>("SELECT * FROM RewardsEntity WHERE IsSaved = ?", true);

                if (existingRecord != null && existingRecord.Count > 0)
                {
                    List<RewardsEntity> matchList = existingRecord.FindAll(x =>
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

                        bool isUsedExpired = false;
                        if (x.IsUsed && !string.IsNullOrEmpty(x.IsUsedDateTime))
                        {
                            try
                            {
                                DateTime dateTimeParse = DateTime.Parse(x.IsUsedDateTime, CultureInfo.InvariantCulture);
                                DateTime utcNow = DateTime.UtcNow;
                                if ((utcNow - dateTimeParse).TotalDays > 7)
                                {
                                    isUsedExpired = true;
                                }

                            }
                            catch (Exception ex)
                            {
                                Utility.LoggingNonFatalError(ex);
                            }
                        }

                        return (startResult >= 0 && endResult <= 0 && !isUsedExpired);
                    });

                    if (matchList != null && matchList.Count > 0)
                    {
                        try
                        {
                            matchList.Sort((x, y) => DateTime.Compare(DateTime.ParseExact(y.StartDate, "yyyyMMddTHHmmss",
                                CultureInfo.InvariantCulture, DateTimeStyles.None), DateTime.ParseExact(x.StartDate, "yyyyMMddTHHmmss",
                                CultureInfo.InvariantCulture, DateTimeStyles.None)));
                        }
                        catch (Exception ex)
                        {
                            Utility.LoggingNonFatalError(ex);
                        }
                        return matchList;
                    }

                    return new List<RewardsEntity>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
            return new List<RewardsEntity>();
        }

        public List<RewardsEntity> GetActiveItemsByCategory(string categoryId)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                var existingRecord = db.Query<RewardsEntity>("SELECT * FROM RewardsEntity WHERE CategoryID = ?", categoryId);

                if (existingRecord != null && existingRecord.Count > 0)
                {
                    List<RewardsEntity> matchList = existingRecord.FindAll(x =>
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

                        bool isUsedExpired = false;
                        if (x.IsUsed && !string.IsNullOrEmpty(x.IsUsedDateTime))
                        {
                            try
                            {
                                DateTime dateTimeParse = DateTime.Parse(x.IsUsedDateTime, CultureInfo.InvariantCulture);
                                DateTime utcNow = DateTime.UtcNow;
                                if ((utcNow - dateTimeParse).TotalDays > 7)
                                {
                                    isUsedExpired = true;
                                }

                            }
                            catch (Exception ex)
                            {
                                Utility.LoggingNonFatalError(ex);
                            }
                        }

                        return (startResult >= 0 && endResult <= 0 && !isUsedExpired);
                    });

                    if (matchList != null && matchList.Count > 0)
                    {
                        try
                        {
                            matchList.Sort((x, y) => DateTime.Compare(DateTime.ParseExact(y.StartDate, "yyyyMMddTHHmmss",
                                CultureInfo.InvariantCulture, DateTimeStyles.None), DateTime.ParseExact(x.StartDate, "yyyyMMddTHHmmss",
                                CultureInfo.InvariantCulture, DateTimeStyles.None)));
                        }
                        catch (Exception ex)
                        {
                            Utility.LoggingNonFatalError(ex);
                        }
                        return matchList;
                    }

                    return new List<RewardsEntity>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
            return new List<RewardsEntity>();
        }

        public void RemoveItem(string itemID)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("Delete from RewardsEntity WHERE ID = ?", itemID);
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
                db.Execute("Delete from RewardsEntity WHERE CategoryID = ?", categoryId);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
        }

        public RewardsEntity GetItem(string itemID)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                List<RewardsEntity> itemList = new List<RewardsEntity>();
                itemList = db.Query<RewardsEntity>("Select * FROM RewardsEntity WHERE ID = ?", itemID);
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

                        bool isUsedExpired = false;
                        if (x.IsUsed && !string.IsNullOrEmpty(x.IsUsedDateTime))
                        {
                            try
                            {
                                DateTime dateTimeParse = DateTime.Parse(x.IsUsedDateTime, CultureInfo.InvariantCulture);
                                DateTime utcNow = DateTime.UtcNow;
                                if ((utcNow - dateTimeParse).TotalDays > 7)
                                {
                                    isUsedExpired = true;
                                }

                            }
                            catch (Exception ex)
                            {
                                Utility.LoggingNonFatalError(ex);
                            }
                        }

                        return (startResult >= 0 && endResult <= 0 && !isUsedExpired);
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

        public bool CheckIsExpired(string itemID)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                List<RewardsEntity> itemList = new List<RewardsEntity>();
                itemList = db.Query<RewardsEntity>("Select * FROM RewardsEntity WHERE ID = ?", itemID);
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
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }

            return true;
        }

        public void UpdateReadItem(string itemID, bool flag, string formattedDate)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("UPDATE RewardsEntity SET Read = ? WHERE ID = ?", flag, itemID);

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
                db.Execute("UPDATE RewardsEntity SET ImageB64 = ? WHERE ID = ?", imageB64, itemID);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
        }

        public void UpdateIsUsedItem(string itemID, bool flag, string formattedDate)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("UPDATE RewardsEntity SET IsUsed = ? WHERE ID = ?", flag, itemID);

                UpdateIsUsedDateTimeItem(itemID, formattedDate);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
        }

        public void UpdateIsSavedItem(string itemID, bool flag, string formattedDate)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("UPDATE RewardsEntity SET IsSaved = ? WHERE ID = ?", flag, itemID);

                UpdateIsSavedDateTimeItem(itemID, formattedDate);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
        }

        private void UpdateIsSavedDateTimeItem(string itemID, string datetime)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("UPDATE RewardsEntity SET IsSavedDateTime = ? WHERE ID = ?", datetime, itemID);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
        }

        private void UpdateIsUsedDateTimeItem(string itemID, string datetime)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("UPDATE RewardsEntity SET IsUsedDateTime = ? WHERE ID = ?", datetime, itemID);
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
                db.Execute("UPDATE RewardsEntity SET ReadDateTime = ? WHERE ID = ?", datetime, itemID);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
        }

    }
}
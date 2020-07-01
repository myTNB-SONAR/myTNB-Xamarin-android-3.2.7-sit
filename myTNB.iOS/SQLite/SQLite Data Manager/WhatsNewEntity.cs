using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using myTNB.SitecoreCMS.Model;
using SQLite;

namespace myTNB.SQLite.SQLiteDataManager
{
    public class WhatsNewEntity : WhatsNewModel
    {
        /// <summary>
        /// Creates the table.
        /// </summary>
        public void CreateTable()
        {
            try
            {
                SQLiteHelper._db.CreateTable<WhatsNewEntity>();
                List<SQLiteConnection.ColumnInfo> info = SQLiteHelper._db.GetTableInfo("WhatsNewEntity");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Create Table : {0}", e.Message);
            }
        }

        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="item">Item.</param>
        public void InsertItem(WhatsNewEntity item)
        {
            try
            {
                if (item != null)
                {
                    int newRecord = SQLiteHelper._db.InsertOrReplace(item);
#if DEBUG
                    Debug.WriteLine("Insert Record: {0}", newRecord);
#endif
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Insert Item in Table : {0}", e.Message);
            }
        }

        /// <summary>
        /// Deletes the item
        /// </summary>
        /// <param name="key"></param>
        public void DeleteItem(string key)
        {
            try
            {
                if (key.IsValid())
                {
                    SQLiteHelper._db.Delete<WhatsNewEntity>(key);
#if DEBUG
                    Debug.WriteLine("Delete Record Successful");
#endif
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Delete Item in Table : {0}", e.Message);
            }
        }

        /// <summary>
        /// Inserts the list of items.
        /// </summary>
        /// <param name="itemList">Item list.</param>
        public void InsertListOfItems(List<WhatsNewModel> itemList)
        {
            if (itemList != null)
            {
                foreach (WhatsNewModel obj in itemList)
                {
                    WhatsNewEntity item = new WhatsNewEntity();

                    item.ID = obj.ID;
                    item.CategoryID = obj.CategoryID;
                    item.CategoryName = obj.CategoryName;
                    item.Title = obj.Title;
                    item.TitleOnListing = obj.TitleOnListing;
                    item.Description = obj.Description;
                    item.Image = obj.Image;
                    item.StartDate = obj.StartDate;
                    item.EndDate = obj.EndDate;
                    item.PublishDate = obj.PublishDate;
                    item.IsRead = obj.IsRead;
                    item.Image_DetailsView = obj.Image_DetailsView;
                    item.Styles_DetailsView = obj.Styles_DetailsView;
                    item.PortraitImage_PopUp = obj.PortraitImage_PopUp;
                    item.ShowEveryCountDays_PopUp = obj.ShowEveryCountDays_PopUp;
                    item.ShowForTotalCountDays_PopUp = obj.ShowForTotalCountDays_PopUp;
                    item.ShowAtAppLaunchPopUp = obj.ShowAtAppLaunchPopUp;
                    item.PopUp_Text_Only = obj.PopUp_Text_Only;
                    item.PopUp_HeaderImage = obj.PopUp_HeaderImage;
                    item.PopUp_Text_Content = obj.PopUp_Text_Content;
                    item.Donot_Show_In_WhatsNew = obj.Donot_Show_In_WhatsNew;
                    item.Disable_DoNotShow_Checkbox = obj.Disable_DoNotShow_Checkbox;
                    item.ShowDateForDay = obj.ShowDateForDay;
                    item.ShowCountForDay = obj.ShowCountForDay;
                    item.ShowDateForDay = obj.ShowDateForDay;
                    item.ShowCountForDay = obj.ShowCountForDay;
                    item.SkipShowOnAppLaunch = obj.SkipShowOnAppLaunch;
                    InsertItem(item);
                }
            }
        }

        /// <summary>
        /// Gets all entity items.
        /// </summary>
        /// <returns>The all entity items.</returns>
        List<WhatsNewEntity> GetAllEntityItems()
        {
            List<WhatsNewEntity> itemList = new List<WhatsNewEntity>();
            try
            {
                itemList = SQLiteHelper._db.Query<WhatsNewEntity>("select * from WhatsNewEntity");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Get All Items : {0}", e.Message);
            }
            return itemList;
        }

        /// <summary>
        /// Gets all items.
        /// </summary>
        /// <returns>The all items.</returns>
        public List<WhatsNewModel> GetAllItems()
        {
            List<WhatsNewModel> itemList = new List<WhatsNewModel>();
            List<WhatsNewEntity> entityItems = GetAllEntityItems();
            WhatsNewModel rewardModel;
            if (entityItems != null && entityItems.Count > 0)
            {
                for(int index = 0; index < entityItems.Count; index++)
                {
                    entityItems[index].ShowDateForDay = WhatsNewServices.GetWhatNewModelShowDate(entityItems[index].ID);
                    entityItems[index].ShowCountForDay = WhatsNewServices.GetWhatNewModelShowCount(entityItems[index].ID);
                    entityItems[index].SkipShowOnAppLaunch = WhatsNewServices.GetIsSkipAppLaunch(entityItems[index].ID);
                }
            }

            foreach (var item in entityItems)
            {
                rewardModel = new WhatsNewModel
                {
                    CategoryID = item.CategoryID,
                    CategoryName = item.CategoryName,
                    ID = item.ID,
                    Title = item.Title,
                    TitleOnListing = item.TitleOnListing,
                    Description = item.Description,
                    Image = item.Image,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    PublishDate = item.PublishDate,
                    IsRead = item.IsRead,
                    Image_DetailsView = item.Image_DetailsView,
                    Styles_DetailsView = item.Styles_DetailsView,
                    PortraitImage_PopUp = item.PortraitImage_PopUp,
                    ShowEveryCountDays_PopUp = item.ShowEveryCountDays_PopUp,
                    ShowForTotalCountDays_PopUp = item.ShowForTotalCountDays_PopUp,
                    ShowAtAppLaunchPopUp = item.ShowAtAppLaunchPopUp,
                    PopUp_Text_Only = item.PopUp_Text_Only,
                    PopUp_HeaderImage = item.PopUp_HeaderImage,
                    PopUp_Text_Content = item.PopUp_Text_Content,
                    Donot_Show_In_WhatsNew = item.Donot_Show_In_WhatsNew,
                    Disable_DoNotShow_Checkbox = item.Disable_DoNotShow_Checkbox,
                    ShowDateForDay = item.ShowDateForDay,
                    ShowCountForDay = item.ShowCountForDay,
                    SkipShowOnAppLaunch = item.SkipShowOnAppLaunch
                };
                itemList.Add(rewardModel);
            }
            return itemList;
        }


        public List<WhatsNewModel> GetActivePopupItems()
        {
            try
            {
                List<WhatsNewEntity> entityItems = GetAllEntityItems();
                if (entityItems != null && entityItems.Count > 0)
                {
                    List<WhatsNewEntity> matchList = entityItems.FindAll(x =>
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

                            x.ShowDateForDay = WhatsNewServices.GetWhatNewModelShowDate(x.ID);
                            x.ShowCountForDay = WhatsNewServices.GetWhatNewModelShowCount(x.ID);
                            x.SkipShowOnAppLaunch = WhatsNewServices.GetIsSkipAppLaunch(x.ID);
                        }
                        catch (Exception ne)
                        {
                            Console.WriteLine("Error in GetActivePopupItems in Table : {0}", ne.Message);
                        }
                        return (startResult >= 0 && endResult <= 0 && x.ShowAtAppLaunchPopUp && !x.SkipShowOnAppLaunch && x.ShowEveryCountDays_PopUp > 0/*&& x.ShowForTotalCountDays_PopUp > 0*/);
                    });
                    if (matchList != null && matchList.Count > 0)
                    {
                        List<WhatsNewEntity> matchItemList = matchList.FindAll(x =>
                        {
                            bool isAlreadyExceedQuota = false;
                            try
                            {
                                if (!string.IsNullOrEmpty(x.StartDate))
                                {
                                    DateTime nowDateTime = DateTime.Now;
                                    DateTime showDateTime = DateTime.ParseExact(x.ShowDateForDay, "yyyyMMddTHHmmss",
                                    CultureInfo.InvariantCulture, DateTimeStyles.None);
                                    if (showDateTime.Date == nowDateTime.Date && x.ShowCountForDay >= x.ShowEveryCountDays_PopUp)
                                    {
                                        isAlreadyExceedQuota = true;
                                    }
                                }
                            }
                            catch (Exception ne)
                            {
                                Console.WriteLine("Error in GetActivePopupItems in Table : {0}", ne.Message);
                            }
                            return (!isAlreadyExceedQuota);
                        });
                        if (matchItemList != null && matchItemList.Count > 0)
                        {
                            List<WhatsNewModel> itemList = new List<WhatsNewModel>();
                            WhatsNewModel rewardModel;
                            foreach (var item in matchItemList)
                            {
                                rewardModel = new WhatsNewModel
                                {
                                    CategoryID = item.CategoryID,
                                    CategoryName = item.CategoryName,
                                    ID = item.ID,
                                    Title = item.Title,
                                    TitleOnListing = item.TitleOnListing,
                                    Description = item.Description,
                                    Image = item.Image,
                                    StartDate = item.StartDate,
                                    EndDate = item.EndDate,
                                    PublishDate = item.PublishDate,
                                    IsRead = item.IsRead,
                                    Image_DetailsView = item.Image_DetailsView,
                                    Styles_DetailsView = item.Styles_DetailsView,
                                    PortraitImage_PopUp = item.PortraitImage_PopUp,
                                    ShowEveryCountDays_PopUp = item.ShowEveryCountDays_PopUp,
                                    ShowForTotalCountDays_PopUp = item.ShowForTotalCountDays_PopUp,
                                    ShowAtAppLaunchPopUp = item.ShowAtAppLaunchPopUp,
                                    PopUp_Text_Only = item.PopUp_Text_Only,
                                    PopUp_HeaderImage = item.PopUp_HeaderImage,
                                    PopUp_Text_Content = item.PopUp_Text_Content,
                                    Donot_Show_In_WhatsNew = item.Donot_Show_In_WhatsNew,
                                    Disable_DoNotShow_Checkbox = item.Disable_DoNotShow_Checkbox,
                                    ShowDateForDay = item.ShowDateForDay,
                                    ShowCountForDay = item.ShowCountForDay,
                                };
                                itemList.Add(rewardModel);
                            }

                            return itemList;
                        }
                        return new List<WhatsNewModel>();
                    }
                    return new List<WhatsNewModel>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
            return new List<WhatsNewModel>();
        }

        /// <summary>
        /// Deletes the table.
        /// </summary>
        public void DeleteTable()
        {
            try
            {
                SQLiteHelper._db.DeleteAll<WhatsNewEntity>();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }

        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="item">Item.</param>
        public void UpdateItem(WhatsNewEntity item)
        {
            try
            {
                int newRecord = SQLiteHelper._db.Update(item);
#if DEBUG
                Debug.WriteLine("Update Record: {0}", newRecord);
#endif
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Update Item in Table : {0}", e.Message);
            }
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <returns>The item.</returns>
        /// <param name="key">Key.</param>
        public static WhatsNewEntity GetItem(string key)
        {
            WhatsNewEntity item = null;
            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    item = SQLiteHelper._db.Get<WhatsNewEntity>(key);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Reading from Table : {0}", e.Message);
            }
            return item;
        }

        /// <summary>
        /// Updates the description.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="description"></param>
        public void UpdateDescription(string key, string description)
        {
            try
            {
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(description))
                {
                    var item = SQLiteHelper._db.Get<WhatsNewEntity>(key);
                    if (item != null)
                    {
                        item.Description = description;
                        UpdateItem(item);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Reading from Table : {0}", e.Message);
            }
        }

        /// <summary>
        /// Updates the entity item.
        /// </summary>
        /// <param name="reward"></param>
        public void UpdateEntity(WhatsNewModel reward)
        {
            if (reward != null)
            {
                WhatsNewEntity item = new WhatsNewEntity
                {
                    CategoryID = reward.CategoryID,
                    CategoryName = reward.CategoryName,
                    ID = reward.ID,
                    Title = reward.Title,
                    TitleOnListing = reward.TitleOnListing,
                    Description = reward.Description,
                    Image = reward.Image,
                    StartDate = reward.StartDate,
                    EndDate = reward.EndDate,
                    PublishDate = reward.PublishDate,
                    IsRead = reward.IsRead,
                    Image_DetailsView = reward.Image_DetailsView,
                    Styles_DetailsView = reward.Styles_DetailsView,
                    PortraitImage_PopUp = reward.PortraitImage_PopUp,
                    ShowEveryCountDays_PopUp = reward.ShowEveryCountDays_PopUp,
                    ShowForTotalCountDays_PopUp = reward.ShowForTotalCountDays_PopUp,
                    ShowAtAppLaunchPopUp = reward.ShowAtAppLaunchPopUp,
                    PopUp_Text_Only = reward.PopUp_Text_Only,
                    PopUp_HeaderImage = reward.PopUp_HeaderImage,
                    PopUp_Text_Content = reward.PopUp_Text_Content,
                    Donot_Show_In_WhatsNew = reward.Donot_Show_In_WhatsNew,
                    Disable_DoNotShow_Checkbox = reward.Disable_DoNotShow_Checkbox,
                    ShowDateForDay = reward.ShowDateForDay,
                    ShowCountForDay = reward.ShowCountForDay,
                    SkipShowOnAppLaunch = reward.SkipShowOnAppLaunch
                };
                UpdateItem(item);
            }
        }
    }
}

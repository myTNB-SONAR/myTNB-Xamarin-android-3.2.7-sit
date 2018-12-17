using SQLite;
using System.Collections.Generic;
using myTNB.SitecoreCMS.Model;
using System;
using myTNB_Android.Src.Utils;
using myTNB.SitecoreCM.Models;
using System.Threading.Tasks;
using System.Threading;
using Android.Graphics;
using System.Net;

namespace myTNB.SQLite.SQLiteDataManager
{
    public class PromotionsEntityV2 : PromotionsModelV2
    {
        public void CreateTable()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("PromotionsEntityV2");
            db.CreateTable<PromotionsEntityV2>();
        }

        public void InsertItem(PromotionsEntityV2 item)
        {
            try
            {
                var db = new SQLiteConnection(Constants.DB_PATH);
                int newRecord = db.InsertOrReplace(item);
                Console.WriteLine("Insert Record: {0}", newRecord );
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Insert Item in Table : {0}", e.Message);
            }
        }

        public void InsertListOfItems(List<PromotionsModelV2> itemList)
        {
            if(itemList != null){
                foreach (PromotionsModelV2 obj in itemList)
                {
                    PromotionsEntityV2 item = new PromotionsEntityV2();
                    item.ID = obj.ID;
                    item.GeneralLinkUrl = obj.GeneralLinkUrl;
                    item.Text = obj.Text;
                    item.Title = obj.Title;
                    item.HeaderContent = obj.HeaderContent;
                    item.BodyContent = obj.BodyContent;
                    item.FooterContent = obj.FooterContent;
                    item.PortraitImage = GetImageFilePathAsync(obj.Title.Replace(" ", ""), obj.PortraitImage.Replace(" ", "%20")).Result;
                    item.LandscapeImage = GetImageFilePathAsync(obj.Title.Replace(" ", "")+"_landscape", obj.LandscapeImage.Replace(" ", "%20")).Result; //obj.LandscapeImage.Replace(" ", "%20");
                    if (!string.IsNullOrEmpty(obj.PromoStartDate))
                    {
                        item.PromoStartDate = GetFormattedStringDate(obj.PromoStartDate);
                    }

                    string PromoEndDateStr = null;
                    if (string.IsNullOrEmpty(obj.PromoEndDate) && !string.IsNullOrEmpty(obj.PromoStartDate)) {
                        DateTime PromoEndDate = DateTime.Now;
                        PromoEndDate = PromoEndDate.AddDays(90);
                        PromoEndDateStr = GetFormattedStringDate(PromoEndDate.ToString("yyyyMMdd"));
                    }
                    else
                    {
                        PromoEndDateStr = GetFormattedStringDate(obj.PromoEndDate);
                    }
                    item.PromoEndDate = PromoEndDateStr;
                    item.PublishedDate = obj.PublishedDate;
                    item.IsPromoExpired = obj.IsPromoExpired;
                    item.ShowAtAppLaunch = obj.ShowAtAppLaunch;
                    InsertItem(item);
                }
            }
        }

        public string GetFormattedStringDate(string date)
        {
            return DateTime.ParseExact(date, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
        }

        public List<PromotionsEntityV2> GetAllItems()
        {
            List<PromotionsEntityV2> itemList = new List<PromotionsEntityV2>();
            try
            {
                var db = new SQLiteConnection(Constants.DB_PATH);
                itemList = db.Query<PromotionsEntityV2>("select * from PromotionsEntityV2");
            }catch(Exception e){
                Console.WriteLine("Error in Get All Items : {0}", e.Message);
            }
            return itemList;
        }

        public PromotionsEntityV2 GetItemById(string id)
        {
            PromotionsEntityV2 entityV2 = new PromotionsEntityV2();
            try
            {
                var db = new SQLiteConnection(Constants.DB_PATH);
                List<PromotionsEntityV2> itemList = new List<PromotionsEntityV2>();
                itemList = db.Query<PromotionsEntityV2>("SELECT * FROM PromotionsEntityV2 WHERE ID = ? ", id);
                if(itemList.Count > 0)
                {
                    entityV2 = itemList[0];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Get Item : {0}", e.Message);
            }
            return entityV2;
        }

        public void DeleteTable(){
            try
            {
                var db = new SQLiteConnection(Constants.DB_PATH);
                db.DeleteAll<PromotionsEntityV2>();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Delete Table : {0}",e.Message);
            }
        }

        public void UpdateItem(PromotionsEntityV2 item)
        {
            try
            {
                var db = new SQLiteConnection(Constants.DB_PATH);
                var existingRecord = db.Query<PromotionsEntityV2>("SELECT * FROM PromotionsEntityV2 WHERE ID = ? ", item.ID);

                if (existingRecord != null && existingRecord.Count > 0)
                {
                    var promotionRecord = existingRecord[0];
                    promotionRecord.LandscapeImage = item.LandscapeImage;
                    promotionRecord.Read = item.Read;
                    db.Update(promotionRecord);
                    Console.WriteLine("Update Record: {0}", promotionRecord);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
        }

        public void UpdateItemAsShown(PromotionsEntityV2 item)
        {
            try
            {
                var db = new SQLiteConnection(Constants.DB_PATH);
                var existingRecord = db.Query<PromotionsEntityV2>("SELECT * FROM PromotionsEntityV2 WHERE ID = ? ", item.ID);

                if (existingRecord != null && existingRecord.Count > 0)
                {
                    DateTime dateTime = DateTime.Now;
                    var promotionRecord = existingRecord[0];
                    promotionRecord.PromoShownDate = GetFormattedStringDate(dateTime.ToString("yyyyMMdd"));
                    db.Update(promotionRecord);
                    Console.WriteLine("Update Record: {0}", promotionRecord);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
        }

        public void UpdateAllItemAsShown()
        {
            try
            {
                var db = new SQLiteConnection(Constants.DB_PATH);
                List<PromotionsEntityV2> itemList = new List<PromotionsEntityV2>();
                try
                {
                    itemList = db.Query<PromotionsEntityV2>("select * from PromotionsEntityV2");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in Get All Items : {0}", e.Message);
                }

                var dbUpdate = new SQLiteConnection(Constants.DB_PATH);
                foreach (PromotionsModelV2 promotionRecord in itemList)
                {
                    DateTime dateTime = DateTime.Now;
                    promotionRecord.PromoShownDate = GetFormattedStringDate(dateTime.ToString("yyyyMMdd"));
                    dbUpdate.Update(promotionRecord);
                    Console.WriteLine("Update All Record: {0}", promotionRecord);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }
        }

        internal static bool HasUnread()
        {
            try
            {
                var db = new SQLiteConnection(Constants.DB_PATH);
                var existingRecord = db.Query<PromotionsEntityV2>("SELECT * FROM PromotionsEntityV2 WHERE Read = ? ", false);

                if (existingRecord != null && existingRecord.Count > 0)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Updating Item in Table : {0}", e.Message);
            }

            return false;
        }

        public List<PromotionsEntityV2> GetAllValidPromotions()
        {
            List<PromotionsEntityV2> itemList = new List<PromotionsEntityV2>();
            List<PromotionsEntityV2> validItemList = new List<PromotionsEntityV2>();
            try
            {
                var db = new SQLiteConnection(Constants.DB_PATH);
                itemList = db.Query<PromotionsEntityV2>("select * from PromotionsEntityV2");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Get All Items : {0}", e.Message);
            }

            foreach(PromotionsModelV2 item in itemList)
            {
                
                DateTime PromoStartDate = DateTime.Now;
                if (!string.IsNullOrEmpty(item.PromoStartDate))
                {
                    PromoStartDate = DateTime.ParseExact(item.PromoStartDate, "dd/MM/yyyy", null);
                }
                DateTime PromoEndDate = DateTime.Now;
                if (!string.IsNullOrEmpty(item.PromoEndDate))
                {
                    PromoEndDate = DateTime.ParseExact(item.PromoEndDate, "dd/MM/yyyy", null);
                }

                string PromoExpired = string.IsNullOrEmpty(item.IsPromoExpired) == true ? "0" : item.IsPromoExpired;
                string ShowAtLaunch = string.IsNullOrEmpty(item.ShowAtAppLaunch) == true ? "0" : item.ShowAtAppLaunch;

                bool DaysCounterExceeded = false;
                DateTime TodayDate = DateTime.Now;
                if (!string.IsNullOrEmpty(item.PromoShownDate))
                {
                    DateTime PromoShownDate = new DateTime();
                    PromoShownDate = DateTime.ParseExact(item.PromoShownDate, "dd/MM/yyyy", null);
                    if((TodayDate - PromoShownDate).TotalDays >= Constants.PROMOTION_DAYS_COUNTER_LIMIT)
                        DaysCounterExceeded = true;
                    else
                        DaysCounterExceeded = false;
                }
                else
                {
                    DaysCounterExceeded = true;
                }

                if(TodayDate.Date <= PromoEndDate.Date && PromoExpired.Equals("0"))
                {
                    if(DaysCounterExceeded && ShowAtLaunch.Equals("1"))
                    {
                        PromotionsEntityV2 entityV2 = new PromotionsEntityV2();
                        entityV2.ID = item.ID;
                        entityV2.GeneralLinkUrl = item.GeneralLinkUrl;
                        entityV2.Text = item.Text;
                        entityV2.Title = item.Title;
                        entityV2.HeaderContent = item.HeaderContent;
                        entityV2.BodyContent = item.BodyContent;
                        entityV2.FooterContent = item.FooterContent;
                        entityV2.PortraitImage = item.PortraitImage.Replace(" ", "%20");
                        entityV2.LandscapeImage = item.LandscapeImage.Replace(" ", "%20");
                        entityV2.PromoStartDate = item.PromoStartDate;
                        entityV2.PromoEndDate = item.PromoEndDate;
                        entityV2.PublishedDate = item.PublishedDate;
                        entityV2.IsPromoExpired = item.IsPromoExpired;
                        entityV2.Read = item.Read;
                        entityV2.PromoShownDate = item.PromoShownDate;
                        entityV2.ShowAtAppLaunch = item.ShowAtAppLaunch;
                        validItemList.Add(entityV2);
                    }
                }
            }
            return validItemList;
        }

        public static void RemoveAll()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            db.Execute("DELETE FROM PromotionsEntityV2");
        }

        public async Task<string> GetImageFilePathAsync(string title, string imageUrl)
        {
            //progressBar.Visibility = ViewStates.Visible;
            string filepath = imageUrl;
            CancellationTokenSource cts = new CancellationTokenSource();
            Bitmap imageBitmap = null;
            await Task.Run(() =>
            {
                imageBitmap = GetImageBitmapFromUrl(imageUrl);
            }, cts.Token);

            if (imageBitmap != null)
            {
                filepath = await FileUtils.SaveAsync(imageBitmap, FileUtils.PROMO_IMAGE_FOLDER, string.Format("{0}.jpeg", title));
            }
            //progressBar.Visibility = ViewStates.Gone;

            return filepath;
        }


        private Android.Graphics.Bitmap GetImageBitmapFromUrl(string url)
        {
            Android.Graphics.Bitmap image = null;
            using (WebClient webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    image = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }
            return image;
        }
    }
}
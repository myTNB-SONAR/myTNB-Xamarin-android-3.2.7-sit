using myTNB.AndroidApp.Src.Utils;
using SQLite;
using System;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.Database.Model
{

    [Table("TooltipImageDirectEntity")]


    //syahmi add

    public class TooltipImageDirectEntity
    {
        [Unique, Column("ID")]
        public string ID { get; set; }

        [Column("Url")]
        public string Url { set; get; }

        [Column("ImageBase64")]
        public string ImageBase64 { set; get; }

        [Column("ImageCategory")]
        public string ImageCategory { set; get; }


        public enum IMAGE_CATEGORY
        {
            WHERE_MY_ACC,
            IC_SAMPLE,
            PROOF_OF_CONSENT,
            PERMISE_IMAGE
    

        }

        public static void CreateTable()
        {

            try
            {
                var db = DBHelper.GetSQLiteConnection();
                List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("TooltipImageDirectEntity");
                db.CreateTable<TooltipImageDirectEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        public static void InsertItem(TooltipImageDirectEntity item)
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

        public void InsertListOfItems(List<TooltipImageDirectEntity> itemList)
        {
            if (itemList != null)
            {
                foreach (TooltipImageDirectEntity obj in itemList)
                {
                    TooltipImageDirectEntity item = new TooltipImageDirectEntity();
                    item.ID = obj.ID;
                    item.Url = obj.Url;
                    item.ImageBase64 = obj.ImageBase64;
                    item.ImageCategory = obj.ImageCategory;
                    InsertItem(item);
                }
            }
        }

        public List<TooltipImageDirectEntity> GetAllItems()
        {
            List<TooltipImageDirectEntity> itemList = new List<TooltipImageDirectEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<TooltipImageDirectEntity>("select * from TooltipImageDirectEntity");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return itemList;
        }

        public static bool isUrlExist(string url)
        {
            bool isConsistUrl = false;

            try
            {

                List <TooltipImageDirectEntity> itemList;

                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<TooltipImageDirectEntity>("select * from TooltipImageDirectEntity where Url = ?", url);
                if (itemList != null && itemList.Count > 0)
                {
                    isConsistUrl = true;
                }
     
            }
            catch (Exception e)
            {
                Console.WriteLine("Error ini sUrlExist : {0}", e.Message);
            }
            return isConsistUrl;
        }

        public static bool isNeedUpdate(string url, IMAGE_CATEGORY ImageCategory)
        {
            bool isConsistUrl = true;  // default need to be true

            try
            {

                List<TooltipImageDirectEntity> itemList;

                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<TooltipImageDirectEntity>("select * from TooltipImageDirectEntity where Url  = ? AND ImageCategory  = ?", url , ImageCategory.ToString());
                if (itemList != null && itemList.Count > 0)
                {
                    isConsistUrl = false;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Error ini sUrlExist : {0}", e.Message);
            }
            return isConsistUrl;
        }

        public static void DeleteImage(IMAGE_CATEGORY ImageCategory)
        {
                try
                {
                    var db = DBHelper.GetSQLiteConnection();
                    db.Execute("DELETE FROM TooltipImageDirectEntity WHERE  ImageCategory  = ?", ImageCategory.ToString());
                }
                catch (System.Exception ne)
                {
                    Utility.LoggingNonFatalError(ne);
                }
   
        }

        public static string GetImageBase64(IMAGE_CATEGORY ImageCategory)
        {
            string data = null;

            try
            {

                List<TooltipImageDirectEntity> itemList;
                

                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<TooltipImageDirectEntity>("select * from TooltipImageDirectEntity where ImageCategory = ?", ImageCategory.ToString());
                if (itemList != null && itemList.Count > 0)
                {
                    data = itemList[0].ImageBase64;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Error ini GetImageBase64 : {0}", e.Message);
            }
            return data;
        }

        public void DeleteTable()
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.DeleteAll<TooltipImageDirectEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }


}
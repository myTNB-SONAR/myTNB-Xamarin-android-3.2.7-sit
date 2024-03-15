using System;
using System.Collections.Generic;
using SQLite;

namespace myTNB.AndroidApp.Src.Database.Model
{
    public class SitecoreCmsEntity
    {
        [PrimaryKey, Column("itemId")]
        public string itemId { get; set; }

        [Column("jsonStringData")]
        public string jsonStringData { get; set; }

        [Column("jsonTimeStampData")]
        public string jsonTimeStampData { get; set; }

        public enum SITE_CORE_ID
        {
            APPLY_SSMR_WALKTHROUGH,
            BILL_TOOLTIP,
            BILL_TOOLTIPV2,
            LANGUAGE_URL,
            LANGUAGE_EN,
            LANGUAGE_MS,
            COUNTRY,
            EPP_TOOLTIP,
            WHERE_IS_MY_ACC,
            WHO_IS_REGISTERED_OWNER,
            DO_I_NEED_OWNER_CONSENT,
            HOW_DOES_COPY_IC,
            HOW_DOES_PROOF_OF_CONSENT,
            NEW_BILL_DESIGN
        }

        public static void CreateTable()
        {
            var db = DBHelper.GetSQLiteConnection();
            List<SQLiteConnection.ColumnInfo> info = db.GetTableInfo("SitecoreCmsEntity");
            db.CreateTable<SitecoreCmsEntity>();
        }

        private static void InsertItem(SitecoreCmsEntity item)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                int newRecord = db.InsertOrReplace(item);
                Console.WriteLine("Insert Record: {0}", newRecord);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Insert Item in Table : {0}", e.Message);
            }
        }

        public static void InsertSiteCoreItem(SITE_CORE_ID itemId, string jsonItemData, string timestampData)
        {
            try
            {
                SitecoreCmsEntity item = new SitecoreCmsEntity();
                item.itemId = itemId.ToString();
                item.jsonStringData = jsonItemData;
                item.jsonTimeStampData = timestampData;

                var db = DBHelper.GetSQLiteConnection();
                int newRecord = db.InsertOrReplace(item);
                Console.WriteLine("Insert Record: {0}", newRecord);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Insert Item in Table : {0}", e.Message);
            }
        }

        public static string GetItemById(SITE_CORE_ID itemId)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                var itemList = db.Query<SitecoreCmsEntity>("SELECT jsonStringData FROM SitecoreCmsEntity WHERE itemId = ?", itemId.ToString());
                if (itemList.Count > 0)
                {
                    return itemList[0].jsonStringData;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Get All Items : {0}", e.Message);
            }
            return null;
        }

        public static string GetItemTimestampById(SITE_CORE_ID itemId)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                var itemList = db.Query<SitecoreCmsEntity>("SELECT jsonTimeStampData FROM SitecoreCmsEntity WHERE itemId = ?", itemId.ToString());
                if (itemList.Count > 0)
                {
                    return itemList[0].jsonTimeStampData;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Get All Items : {0}", e.Message);
            }
            return null;
        }

        public static bool IsNeedUpdates(SITE_CORE_ID itemId, string newTimeStampData)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                List<SitecoreCmsEntity> itemList = db.Query<SitecoreCmsEntity>("SELECT jsonTimeStampData FROM SitecoreCmsEntity WHERE itemId = ?", itemId.ToString());
                return newTimeStampData != itemList[0].jsonTimeStampData;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Get All Items : {0}", e.Message);
            }
            return true;
        }

        public static void DeleteSitecoreRecord(SITE_CORE_ID itemId)
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("DELETE FROM SitecoreCmsEntity where itemId = ?", itemId.ToString());


            }
            catch (System.Exception e)
            {
                Console.WriteLine("Error in Delete Items : {0}", e.Message);
            }
        }

        public static List<SitecoreCmsEntity> GetAllItems()
        {
            List<SitecoreCmsEntity> itemList = new List<SitecoreCmsEntity>();
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                itemList = db.Query<SitecoreCmsEntity>("select * from SitecoreCmsEntity");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Get All Items : {0}", e.Message);
            }
            return itemList;
        }

        public static void DeleteTable()
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.DeleteAll<SitecoreCmsEntity>();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Delete Table : {0}", e.Message);
            }
        }

        public static void UpdateItem(SitecoreCmsEntity item)
        {
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                int newRecord = db.Update(item);
                Console.WriteLine("Insert Record: {0}", newRecord);
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Insert Item in Table : {0}", e.Message);
            }
        }
    }
}

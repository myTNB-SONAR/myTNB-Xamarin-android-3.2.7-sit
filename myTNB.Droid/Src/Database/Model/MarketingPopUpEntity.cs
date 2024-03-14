using System;
using System.Collections.Generic;
using myTNB.Android.Src.Utils;
using SQLite;

namespace myTNB.Android.Src.Database.Model
{
    [Table("MarketingPopUpEntity")]
    public class MarketingPopUpEntity
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }

        [Unique, Column("contractAccount")]
        public string ContractAccount { get; set; }

        [Column("dbrPopUpHasShown")]
        public bool DBRPopUpHasShown { get; set; }

        public static int CreateTable()
        {
            var db = DBHelper.GetSQLiteConnection();
            return (int)db.CreateTable<MarketingPopUpEntity>();
        }

        public static void CreateTableAsync(SQLiteAsyncConnection db)
        {
            db.CreateTableAsync<MarketingPopUpEntity>();
        }

        public static int InsertOrReplace(string ca, bool dbrPopUpHasShown)
        {
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new MarketingPopUpEntity()
            {
                ContractAccount = ca,
                DBRPopUpHasShown = dbrPopUpHasShown
            };

            int newRecordId = db.InsertOrReplace(newRecord);
            if (newRecordId > 0)
            {
                return newRecord.Id ?? 0;
            }

            return 0;
        }

        public static bool GetDBRPopUpFlag(string ca)
        {
            bool hasShown = false;
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                List<MarketingPopUpEntity> marketingPopUpEntity = db.Query<MarketingPopUpEntity>("SELECT * FROM MarketingPopUpEntity WHERE ContractAccount = ?", ca);

                if (marketingPopUpEntity.Count > 0)
                {
                    hasShown = marketingPopUpEntity[0].DBRPopUpHasShown;
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return hasShown;
        }

        public static void RemoveAll()
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("DELETE FROM MarketingPopUpEntity");
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using myTNB_Android.Src.Utils;
using SQLite;

namespace myTNB_Android.Src.Database.Model
{
    [Table("MyHomeNCPopUpEntity")]
    public class MyHomeNCPopUpEntity
	{
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }

        [Unique, Column("NotificationId")]
        public string NotificationId { get; set; }

        [Column("NCPopUpHasShown")]
        public bool NCPopUpHasShown { get; set; }

        public static int CreateTable()
        {
            var db = DBHelper.GetSQLiteConnection();
            return (int)db.CreateTable<MyHomeNCPopUpEntity>();
        }

        public static void CreateTableAsync(SQLiteAsyncConnection db)
        {
            db.CreateTableAsync<MyHomeNCPopUpEntity>();
        }

        public static int InsertOrReplace(string notificationId, bool dbrPopUpHasShown)
        {
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new MyHomeNCPopUpEntity()
            {
                NotificationId = notificationId,
                NCPopUpHasShown = dbrPopUpHasShown
            };

            int newRecordId = db.InsertOrReplace(newRecord);
            if (newRecordId > 0)
            {
                return newRecord.Id ?? 0;
            }

            return 0;
        }

        public static bool GetNCPopUpFlag(string notificationId)
        {
            bool hasShown = false;
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                List<MyHomeNCPopUpEntity> myHomeNCPopUpEntity = db.Query<MyHomeNCPopUpEntity>("SELECT * FROM MyHomeNCPopUpEntity WHERE NotificationId = ?", notificationId);

                if (myHomeNCPopUpEntity.Count > 0)
                {
                    hasShown = myHomeNCPopUpEntity[0].NCPopUpHasShown;
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
                db.Execute("DELETE FROM MyHomeNCPopUpEntity");
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }
    }
}


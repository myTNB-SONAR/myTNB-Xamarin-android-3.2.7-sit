using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Utils;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace myTNB_Android.Src.Database.Model
{
    [Table("NotificationTypesEntity")]
    public class NotificationTypesEntity
    {
        //        "Id": "1000004",
        //"Title": "Billing & Payment",
        //"Code": "BP",
        //"PreferenceMode": "M",
        //"Type": "ANNOUNCEMENT",
        //"CreatedDate": "11/7/2017 4:37:57 PM",
        //"MasterId": null,
        //"IsOpted": null

        [Unique, Column("Id")]
        public string Id { get; set; }

        [Column("Title")]
        public string Title { get; set; }

        [Column("Code")]
        public string Code { get; set; }

        [Column("PreferenceMode")]
        public string PreferenceMode { get; set; }

        [Column("Type")]
        public string Type { get; set; }

        [Column("CreatedDate")]
        public string CreatedDate { get; set; }

        [Column("MasterId")]
        public string MasterId { get; set; }

        [Column("IsOpted")]
        public bool IsOpted { get; set; }

        [Column("ShowInPreference")]
        public bool ShowInPreference { get; set; }

        [Column("ShowInFilterList")]
        public bool ShowInFilterList { get; set; }

        public static int CreateTable()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return (int)db.CreateTable<NotificationTypesEntity>();
            //}
        }

        public static void CreateTableAsync(SQLiteAsyncConnection db)
        {
            db.CreateTableAsync<NotificationTypesEntity>();
        }

        public static int InsertOrReplace(NotificationTypes type)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new NotificationTypesEntity()
            {
                Id = type.Id,
                Title = type.Title,
                Code = type.Code,
                PreferenceMode = type.PreferenceMode,
                Type = type.Type,
                CreatedDate = type.CreatedDate,
                MasterId = type.MasterId,
                IsOpted = type.IsOpted,
                ShowInPreference = type.ShowInPreference,
                ShowInFilterList = type.ShowInFilterList
            };

            int rows = db.InsertOrReplace(newRecord);

            return rows;
            //}
        }

        public static void InsertOrReplaceAsync(NotificationTypes type)
        {
            //var db = new SQLiteAsyncConnection(Constants.DB_PATH);
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new NotificationTypesEntity()
            {
                Id = type.Id,
                Title = type.Title,
                Code = type.Code,
                PreferenceMode = type.PreferenceMode,
                Type = type.Type,
                CreatedDate = type.CreatedDate,
                MasterId = type.MasterId,
                IsOpted = type.IsOpted,
                ShowInPreference = type.ShowInPreference,
                ShowInFilterList = type.ShowInFilterList
            };

            //db.InsertOrReplaceAsync(newRecord);
            db.InsertOrReplace(newRecord);
        }

        public static NotificationTypesEntity GetById(string Id)
        {
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                return db.Query<NotificationTypesEntity>("SELECT * FROM NotificationTypesEntity WHERE Id = ?", Id).ToList()[0];
                //}
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
                return null;
            }



        }

        public static List<NotificationTypesEntity> List()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return db.Query<NotificationTypesEntity>("SELECT * FROM NotificationTypesEntity ").ToList();
            //}

        }

        public static void RemoveActive()
        {
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("Delete from NotificationTypesEntity");
                //}
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }
    }
}
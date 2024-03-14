using myTNB.Android.Src.AppLaunch.Models;
using myTNB.Android.Src.Utils;
using SQLite;

namespace myTNB.Android.Src.Database.Model
{
    [Table("NotificationChannelEntity")]
    public class NotificationChannelEntity
    {
        //        "Id": "1000001",
        //"Title": "Push Notifications",
        //"Code": "PUSH",
        //"PreferenceMode": "M",
        //"Type": "CHANNEL",
        //"CreatedDate": "11/7/2017 4:35:10 PM",
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


        public static int CreateTable()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return (int)db.CreateTable<NotificationChannelEntity>();
            //}
        }

        public static void CreateTableAsync(SQLiteAsyncConnection db)
        {
            db.CreateTableAsync<NotificationChannelEntity>();
        }

        public static int InsertOrReplace(NotificationChannels channels)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new NotificationChannelEntity()
            {
                Id = channels.Id,
                Title = channels.Title,
                Code = channels.Code,
                PreferenceMode = channels.PreferenceMode,
                Type = channels.Type,
                CreatedDate = channels.CreatedDate,
                MasterId = channels.MasterId,
                IsOpted = channels.IsOpted == null ? true : false
            };

            int rows = db.InsertOrReplace(newRecord);

            return rows;
            //}
        }

        public static async void InsertOrReplaceAsync(NotificationChannels channels)
        {
            //var db = new SQLiteAsyncConnection(Constants.DB_PATH);
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new NotificationChannelEntity()
            {
                Id = channels.Id,
                Title = channels.Title,
                Code = channels.Code,
                PreferenceMode = channels.PreferenceMode,
                Type = channels.Type,
                CreatedDate = channels.CreatedDate,
                MasterId = channels.MasterId,
                IsOpted = channels.IsOpted == null ? true : false
            };

            //db.InsertOrReplaceAsync(newRecord);
            db.InsertOrReplace(newRecord);

        }

        public static void RemoveActive()
        {
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("Delete from NotificationChannelEntity");
                //}
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }
    }
}
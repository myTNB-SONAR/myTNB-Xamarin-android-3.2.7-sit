using myTNB_Android.Src.Utils;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace myTNB_Android.Src.Database.Model
{
    [Table("NotificationFilterEntity")]
    public class NotificationFilterEntity
    {
        [Unique, Column("Id")]
        public string Id { get; set; }

        [Column("Title")]
        public string Title { get; set; }

        [Column("IsSelected")]
        public bool IsSelected { get; set; }

        public static int CreateTable()
        {
            var db = DBHelper.GetSQLiteConnection();
            return (int)db.CreateTable<NotificationFilterEntity>();
        }

        public static void CreateTableAsync(SQLiteAsyncConnection db)
        {
            db.CreateTableAsync<NotificationFilterEntity>();
        }

        public static int InsertOrReplace(string Id, string Title, bool isSelected)
        {
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new NotificationFilterEntity()
            {
                Id = Id,
                Title = Title,
                IsSelected = isSelected
            };

            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
        }

        public static NotificationFilterEntity GetActive()
        {
            var db = DBHelper.GetSQLiteConnection();
            //return db.Query<NotificationFilterEntity>("SELECT * FROM NotificationFilterEntity WHERE IsSelected = ?", true)[0];
            List<NotificationFilterEntity> notificationList = new List<NotificationFilterEntity>();
            notificationList = db.Query<NotificationFilterEntity>("SELECT * FROM NotificationFilterEntity WHERE IsSelected = ?", true).ToList();
            if (notificationList != null && notificationList.Count() > 0)
            {
                return notificationList[0];
            }                
            return null;
        }

        public static void RemoveAll()
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("DELETE FROM NotificationFilterEntity");
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }

        public static List<NotificationFilterEntity> List()
        {
            var db = DBHelper.GetSQLiteConnection();
            return db.Query<NotificationFilterEntity>("SELECT * FROM NotificationFilterEntity").ToList();
        }

        public static void UnSelectAll()
        {
            var db = DBHelper.GetSQLiteConnection();
            db.Execute("UPDATE NotificationFilterEntity SET IsSelected = ?", false);
        }

        public static int SelectItem(string id)
        {
            var db = DBHelper.GetSQLiteConnection();
            return db.Execute("UPDATE NotificationFilterEntity SET IsSelected = ? WHERE Id = ?", true, id);
        }
    }
}
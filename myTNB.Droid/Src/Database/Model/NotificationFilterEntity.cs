using myTNB.Android.Src.Utils;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace myTNB.Android.Src.Database.Model
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
            using (var db = new SQLiteConnection(Constants.DB_PATH))
            {
                return (int)db.CreateTable<NotificationFilterEntity>();
            }
        }

        public static void CreateTableAsync(SQLiteAsyncConnection db)
        {
            db.CreateTableAsync<NotificationFilterEntity>();
        }

        public static int InsertOrReplace(string Id, string Title, bool isSelected)
        {
            using (var db = new SQLiteConnection(Constants.DB_PATH))
            {
                var newRecord = new NotificationFilterEntity()
                {
                    Id = Id,
                    Title = Title,
                    IsSelected = isSelected
                };

                int newRecordRow = db.InsertOrReplace(newRecord);

                return newRecordRow;
            }
        }

        public static NotificationFilterEntity GetActive()
        {
            using (var db = new SQLiteConnection(Constants.DB_PATH))
            {
                //return db.Query<NotificationFilterEntity>("SELECT * FROM NotificationFilterEntity WHERE IsSelected = ?", true)[0];
                List<NotificationFilterEntity> notificationList = new List<NotificationFilterEntity>();
                notificationList = db.Query<NotificationFilterEntity>("SELECT * FROM NotificationFilterEntity WHERE IsSelected = ?", true).ToList();
                if (notificationList != null && notificationList.Count() > 0)
                {
                    return notificationList[0];
                }
                return null;
            }
        }

        public static void RemoveAll()
        {
            try
            {
                using (var db = new SQLiteConnection(Constants.DB_PATH))
                {
                    db.Execute("DELETE FROM NotificationFilterEntity");
                }
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }

        public static List<NotificationFilterEntity> List()
        {
            using (var db = new SQLiteConnection(Constants.DB_PATH))
            {
                return db.Query<NotificationFilterEntity>("SELECT * FROM NotificationFilterEntity").ToList();
            }
        }

        public static void UnSelectAll()
        {
            using (var db = new SQLiteConnection(Constants.DB_PATH))
            {
                db.Execute("UPDATE NotificationFilterEntity SET IsSelected = ?", false);
            }
        }

        public static int SelectItem(string id)
        {
            using (var db = new SQLiteConnection(Constants.DB_PATH))
            {
                return db.Execute("UPDATE NotificationFilterEntity SET IsSelected = ? WHERE Id = ?", true, id);
            }
        }
    }
}
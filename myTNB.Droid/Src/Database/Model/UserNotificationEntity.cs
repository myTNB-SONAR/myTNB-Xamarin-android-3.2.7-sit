using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Utils;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace myTNB_Android.Src.Database.Model
{
    [Table("UserNotificationEntity")]
    public class UserNotificationEntity
    {
        [PrimaryKey, Column("Id")]
        public string Id { get; set; }

        [Unique, Column("UId")]
        public string UId { get; set; }

        [Column("Email")]
        public string Email { get; set; }

        [Column("DeviceId")]
        public string DeviceId { get; set; }

        [Column("AccountNum")]
        public string AccountNum { get; set; }

        [Column("Title")]
        public string Title { get; set; }

        [Column("Message")]
        public string Message { get; set; }

        [Column("IsRead")]
        public bool IsRead { get; set; }

        [Column("IsDeleted")]
        public bool IsDeleted { get; set; }

        [Column("NotificationTypeId")]
        public string NotificationTypeId { get; set; }

        [Column("BCRMNotificationTypeId")]
        public string BCRMNotificationTypeId { get; set; }

        [Column("CreatedDate")]
        public string CreatedDate { get; set; }

        [Column("NotificationType")]
        public string NotificationType { get; set; }

        [Column("Target")]
        public string Target { get; set; }

        [Column("ODNBatchSubcategory")]
        public string ODNBatchSubcategory { get; set; }

        [Column("isForceDisplay")]
        public bool IsForceDisplay { get; set; }

        [Column("HeaderTitle")]
        public string HeaderTitle { get; set; }

        public static int CreateTable()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, true))
            //using (var db = DBHelper.GetSQLiteConnection())
            //{
            var db = DBHelper.GetSQLiteConnection();
            return (int)db.CreateTable<UserNotificationEntity>();

            //}
        }

        public static void CreateTableAsync(SQLiteAsyncConnection db)
        {
            db.CreateTableAsync<UserNotificationEntity>();
        }

        public static int InsertOrReplace(UserNotification userNotification)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, true))
            //using (var db = DBHelper.GetSQLiteConnection())
            //{
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new UserNotificationEntity()
            {
                Id = userNotification.Id,
                UId = userNotification.Id,
                Email = userNotification.Email,
                DeviceId = userNotification.DeviceId,
                AccountNum = userNotification.AccountNum,
                Title = userNotification.Title,
                Message = userNotification.Message,
                IsRead = userNotification.IsRead,
                IsDeleted = userNotification.IsDeleted,
                NotificationTypeId = userNotification.NotificationTypeId,
                BCRMNotificationTypeId = userNotification.BCRMNotificationTypeId,
                CreatedDate = userNotification.CreatedDate,
                NotificationType = userNotification.NotificationType,
                Target = userNotification.Target,
                ODNBatchSubcategory = userNotification.ODNBatchSubcategory,
                IsForceDisplay = userNotification.IsForceDisplay,
                HeaderTitle = userNotification.HeaderTitle
            };
            int rows = db.InsertOrReplace(newRecord);
            //db.Close();
            return rows;
            //}

        }

        public static void InsertOrReplaceAsync(UserNotification userNotification)
        {
            //var db = new SQLiteAsyncConnection(Constants.DB_PATH, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, true);
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new UserNotificationEntity()
            {
                Id = userNotification.Id,
                UId = userNotification.Id,
                Email = userNotification.Email,
                DeviceId = userNotification.DeviceId,
                AccountNum = userNotification.AccountNum,
                Title = userNotification.Title,
                Message = userNotification.Message,
                IsRead = userNotification.IsRead,
                IsDeleted = userNotification.IsDeleted,
                NotificationTypeId = userNotification.NotificationTypeId,
                BCRMNotificationTypeId = userNotification.BCRMNotificationTypeId,
                CreatedDate = userNotification.CreatedDate,
                NotificationType = userNotification.NotificationType,
                Target = userNotification.Target,
                ODNBatchSubcategory = userNotification.ODNBatchSubcategory,
                IsForceDisplay = userNotification.IsForceDisplay,
                HeaderTitle = userNotification.HeaderTitle
            };

            //db.InsertOrReplaceAsync(newRecord);
            db.InsertOrReplace(newRecord);

        }

        public static void UpdateIsRead(string notificationId, bool isRead)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, true))
            //using (var db = DBHelper.GetSQLiteConnection())
            //{
            var db = DBHelper.GetSQLiteConnection();
            db.Execute("UPDATE UserNotificationEntity set IsRead = ? WHERE Id = ?", isRead, notificationId);
            //db.Close();
            //}
        }

        public static void UpdateIsDeleted(string notificationId, bool isDeleted)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, true))
            //using (var db = DBHelper.GetSQLiteConnection())
            //{
            var db = DBHelper.GetSQLiteConnection();
            db.Execute("UPDATE UserNotificationEntity set IsDeleted = ? WHERE Id = ?", isDeleted, notificationId);
            //db.Close();
            //}
        }

        public static void RemoveById(string notificationId)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, true))
            //using (var db = DBHelper.GetSQLiteConnection())
            //{
            var db = DBHelper.GetSQLiteConnection();
            db.Execute("DELETE FROM UserNotificationEntity WHERE Id = ?", notificationId);
            //db.Close();
            //}
        }

        public static UserNotificationEntity GetById(string Id)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, true))
            //using (var db = DBHelper.GetSQLiteConnection())
            //{
            var db = DBHelper.GetSQLiteConnection();
            UserNotificationEntity userNotificationEntity = new UserNotificationEntity();
            userNotificationEntity = db.Query<UserNotificationEntity>("SELECT * FROM UserNotificationEntity WHERE Id = ?", Id)[0];
            //db.Close();
            return userNotificationEntity;
            //return db.Query<UserNotificationEntity>("SELECT * FROM UserNotificationEntity WHERE Id = ?", Id)[0];
            //}
        }

        public static List<UserNotificationEntity> ListAllActive()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, true))
            //using (var db = DBHelper.GetSQLiteConnection())
            //{
            var db = DBHelper.GetSQLiteConnection();
            List<UserNotificationEntity> activeList = new List<UserNotificationEntity>();
            activeList = db.Query<UserNotificationEntity>("SELECT * FROM UserNotificationEntity WHERE IsDeleted = ?", false).ToList<UserNotificationEntity>();
            //db.Close();
            return activeList;
            //return db.Query<UserNotificationEntity>("SELECT * FROM UserNotificationEntity WHERE IsDeleted = ?", false).ToList<UserNotificationEntity>();
            //}
        }

        public static List<UserNotificationEntity> ListFiltered(string notificationTypeId)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, true))
            //using (var db = DBHelper.GetSQLiteConnection())
            //{
            var db = DBHelper.GetSQLiteConnection();
            List<UserNotificationEntity> filteredList = new List<UserNotificationEntity>();
            filteredList = db.Query<UserNotificationEntity>("SELECT * FROM UserNotificationEntity WHERE IsDeleted = ? AND NotificationTypeId = ?", false, notificationTypeId).ToList<UserNotificationEntity>();
            //db.Close();
            return filteredList;
            //return db.Query<UserNotificationEntity>("SELECT * FROM UserNotificationEntity WHERE IsDeleted = ? AND NotificationTypeId = ?", false, notificationTypeId).ToList<UserNotificationEntity>();
            //}
        }

        public static List<UserNotificationEntity> ListFilteredNotificationsByBCRMType(string accNum, string bcrmNotificationTypeId)
        {
            List<UserNotificationEntity> list = new List<UserNotificationEntity>();

            try
            {
                var db = DBHelper.GetSQLiteConnection();
                List<UserNotificationEntity> filteredList = new List<UserNotificationEntity>();
                list = db.Query<UserNotificationEntity>("SELECT * FROM UserNotificationEntity WHERE AccountNum = ? AND IsDeleted = ? AND BCRMNotificationTypeId = ?", accNum, false, bcrmNotificationTypeId).ToList<UserNotificationEntity>();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return list;
        }

        public static int Count()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, true))
            //using (var db = DBHelper.GetSQLiteConnection())
            //{
            var db = DBHelper.GetSQLiteConnection();
            int count = 0;

            List<UserNotificationEntity> notificationList = db.Query<UserNotificationEntity>("SELECT * FROM UserNotificationEntity WHERE IsRead = ? AND IsDeleted = ?", false, false);
            //Added checking on notification count.
            notificationList.ForEach(item =>
            {
                if (item.NotificationType != "ODN")
                {
                    if (item.ODNBatchSubcategory == "ODNAsBATCH")
                    {
                        count++;
                    }
                    else
                    {
                        if (item.IsForceDisplay
                            || (UserEntity.GetActive().Email.Equals(item.Email)
                            && MyTNBAccountManagement.GetInstance().IsAccountNumberExist(item.AccountNum)))
                        {
                            if (item.NotificationTypeId == Constants.NOTIFICATION_TYPE_ID_SD)
                            {
                                if (MyTNBAccountManagement.GetInstance().IsSDUserVerify())
                                {
                                    count++;
                                }
                            }
                            else if (item.NotificationTypeId == Constants.NOTIFICATION_TYPE_ID_EB)
                            {
                                if (MyTNBAccountManagement.GetInstance().IsEBUserVerify())
                                {
                                    count++;
                                }
                            }
                            else
                            {
                                count++;
                            }
                        }
                    }
                }
                else
                {
                    count++;
                }
            });
            //db.Close();
            return count;
            //return db.Query<UserNotificationEntity>("SELECT * FROM UserNotificationEntity WHERE IsRead = ? AND IsDeleted = ?", false, false).Count;
            //}
        }

        public static void RemoveAll()
        {
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, true))
                //using (var db = DBHelper.GetSQLiteConnection())
                //{
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("DELETE FROM UserNotificationEntity");
                //db.Close();
                //}
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }

        public static bool HasNotifications()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            //int count = db.Query<UserNotificationEntity>("SELECT * FROM UserNotificationEntity WHERE IsRead = ? AND IsDeleted = ?", false, false).Count;
            //db.Close();
            //if (Count() > 0) {
            //    return true;
            //} else {
            //    return false;
            //}
            //}
            return (Count() > 0);
        }
    }
}
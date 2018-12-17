using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.AppLaunch.Models;

namespace myTNB_Android.Src.Database.Model
{
    [Table("UserNotificationEntity")]
    public class UserNotificationEntity
    {
        [PrimaryKey , Column("Id")]
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

        public static int CreateTable()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.CreateTable<UserNotificationEntity>();
        }

        public static void CreateTableAsync(SQLiteAsyncConnection db)
        {
            db.CreateTableAsync<UserNotificationEntity>();
        }

        public static int InsertOrReplace(UserNotification userNotification)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
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
                Target = userNotification.Target
            };
            int rows = db.InsertOrReplace(newRecord);
            return rows;


        }

        public static void InsertOrReplaceAsync(UserNotification userNotification)
        {
            var db = new SQLiteAsyncConnection(Constants.DB_PATH);
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
                Target = userNotification.Target
            };

            db.InsertOrReplaceAsync(newRecord);
        }

        public static void UpdateIsRead(string notificationId, bool isRead)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            db.Execute("UPDATE UserNotificationEntity set IsRead = ? WHERE Id = ?" , isRead , notificationId);
        }

        public static void UpdateIsDeleted(string notificationId , bool isDeleted)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            db.Execute("UPDATE UserNotificationEntity set IsDeleted = ? WHERE Id = ?", isDeleted, notificationId);
        }

        public static UserNotificationEntity GetById(string Id)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<UserNotificationEntity>("SELECT * FROM UserNotificationEntity WHERE Id = ?" , Id)[0];
        }

        public static List<UserNotificationEntity> ListAllActive()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<UserNotificationEntity>("SELECT * FROM UserNotificationEntity WHERE IsDeleted = ?" , false).ToList<UserNotificationEntity>();
        }

        public static List<UserNotificationEntity> ListFiltered(string notificationTypeId)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<UserNotificationEntity>("SELECT * FROM UserNotificationEntity WHERE IsDeleted = ? AND NotificationTypeId = ?", false , notificationTypeId).ToList<UserNotificationEntity>();
        }

        public static int Count()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<UserNotificationEntity>("SELECT * FROM UserNotificationEntity WHERE IsRead = ? AND IsDeleted = ?", false, false).Count;
        }

        public static void RemoveAll()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            db.Execute("DELETE FROM UserNotificationEntity");
        }

        public static bool HasNotifications()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<UserNotificationEntity>("SELECT * FROM UserNotificationEntity WHERE IsRead = ? AND IsDeleted = ?" , false , false ).Count > 0;
        }

    }
}
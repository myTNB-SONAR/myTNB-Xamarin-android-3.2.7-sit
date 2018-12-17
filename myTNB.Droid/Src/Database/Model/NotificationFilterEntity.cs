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

namespace myTNB_Android.Src.Database.Model
{
    [Table("NotificationFilterEntity")]
    public class NotificationFilterEntity
    {
        [Unique , Column("Id")]
        public string Id { get; set; }

        [Column("Title")]
        public string Title { get; set; }

        [Column("IsSelected")]
        public bool IsSelected { get; set; }

        public static int CreateTable()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.CreateTable<NotificationFilterEntity>();
        }

        public static void CreateTableAsync(SQLiteAsyncConnection db)
        {
            db.CreateTableAsync<NotificationFilterEntity>();
        }

        public static int InsertOrReplace(string Id , string Title , bool isSelected)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
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
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<NotificationFilterEntity>("SELECT * FROM NotificationFilterEntity WHERE IsSelected = ?" , true)[0];
        }

        public static void RemoveAll()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            db.Execute("DELETE FROM NotificationFilterEntity");
        }

        public static List<NotificationFilterEntity> List()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<NotificationFilterEntity>("SELECT * FROM NotificationFilterEntity").ToList();
        }

        public static void UnSelectAll()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            db.Execute("UPDATE NotificationFilterEntity SET IsSelected = ?" , false);
        }

        public static int SelectItem(string id)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Execute("UPDATE NotificationFilterEntity SET IsSelected = ? WHERE Id = ?", true , id);
        }
    }
}
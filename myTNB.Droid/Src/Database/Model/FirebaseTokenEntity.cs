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
    [Table("FirebaseTokenEntity")]
    internal class FirebaseTokenEntity
    {

        [Unique , Column("FBToken")]
        public string FBToken { get; set; }

        [Column("IsLatest")]
        public bool IsLatest { get; set; }

        public static int CreateTable()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.CreateTable<FirebaseTokenEntity>();
        }

        public static void CreateTableAsync(SQLiteAsyncConnection db)
        {
            db.CreateTableAsync<FirebaseTokenEntity>();
        }

        public static int InsertOrReplace(string token , bool isLatest)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            var newRecord = new FirebaseTokenEntity()
            {
                FBToken = token,
                IsLatest = isLatest
            };

            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
        }

        public static void RemoveLatest()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            db.Execute("UPDATE FirebaseTokenEntity SET IsLatest = ? where IsLatest = ?", false, true);
        }

        public static void RemoveAll()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            db.Execute("DELETE FROM FirebaseTokenEntity");
        }

        public static bool HasLatest()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<FirebaseTokenEntity>("SELECT * FROM FirebaseTokenEntity WHERE IsLatest = ?", true).Count > 0;
        }

        public static FirebaseTokenEntity GetLatest()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<FirebaseTokenEntity>("SELECT * FROM FirebaseTokenEntity WHERE IsLatest = ?" , true)[0];
        }
    }
}
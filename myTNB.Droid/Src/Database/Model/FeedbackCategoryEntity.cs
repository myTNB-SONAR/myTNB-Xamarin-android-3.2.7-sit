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
using SQLite;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.AppLaunch.Models;

namespace myTNB_Android.Src.Database.Model
{
    [Table("FeedbackCategoryEntity")]
    public class FeedbackCategoryEntity
    {
        [PrimaryKey , Column("FeedbackCategoryId")]
        public string Id { get; set; }

        [Column("FeedbackCategoryName")]
        public string Name { get; set; }


        public static int CreateTable()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.CreateTable<FeedbackCategoryEntity>();
        }

        public static int InsertOrReplace(FeedbackCategory feedback)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            var newRecord = new FeedbackCategoryEntity()
            {
                Id = feedback.FeedbackCategoryId,
                Name = feedback.FeedbackCategoryName
            };


            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
        }



        public static void RemoveActive()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            db.Execute("DELETE FROM FeedbackCategoryEntity");
        }


        public static bool HasRecords()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<FeedbackCategoryEntity>("SELECT * FROM FeedbackCategoryEntity").Count > 0;
        }


        public static List<FeedbackCategoryEntity> GetActiveList()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<FeedbackCategoryEntity>("SELECT * FROM FeedbackCategoryEntity").ToList();
        }
    }
}
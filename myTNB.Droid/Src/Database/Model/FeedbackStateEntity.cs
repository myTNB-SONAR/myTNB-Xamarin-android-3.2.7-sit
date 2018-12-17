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
    [Table("FeedbackStateEntity")]
    public class FeedbackStateEntity
    {
        [PrimaryKey, Column("StateId")]
        public string Id { get; set; }

        [Column("StateName")]
        public string Name { get; set; }

        [Column("IsSelected")]
        public bool IsSelected { get; set; }

        public static int CreateTable()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.CreateTable<FeedbackStateEntity>();
        }

        public static int InsertOrReplace(FeedbackState feedback)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            var newRecord = new FeedbackStateEntity()
            {
                Id = feedback.StateId,
                Name = feedback.StateName
            };


            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
        }

        public static int InsertOrReplace(FeedbackState feedback , bool isSelected)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            var newRecord = new FeedbackStateEntity()
            {
                Id = feedback.StateId,
                Name = feedback.StateName,
                IsSelected = isSelected
            };


            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
        }

        public static void SetSelected(string Id)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            db.Execute("UPDATE FeedbackStateEntity SET IsSelected = ? WHERE StateId = ?" , true  , Id);
        }

        public static void ResetSelected()
        {
            RemoveActive();
            if (HasRecords())
            {
                FeedbackStateEntity state = GetFirstOrSelected();
                if (state != null)
                {

                    var db = new SQLiteConnection(Constants.DB_PATH);
                    int newRecordRow = db.InsertOrReplace(state);

                }
            }
        }

        public static void Remove()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            db.Execute("DELETE FROM FeedbackStateEntity");
        }

        public static void RemoveActive()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            db.Execute("UPDATE FeedbackStateEntity SET IsSelected = ?" , false);
        }


        public static bool HasRecords()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<FeedbackStateEntity>("SELECT * FROM FeedbackStateEntity").Count > 0;
        }

        public static FeedbackStateEntity GetFirstOrSelected()
        {
            if (HasSelected())
            {
                return GetSelected();
            }
            else
            {
                return GetActiveList()[0];
            }
        }

        public static bool HasSelected()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<FeedbackStateEntity>("SELECT * FROM FeedbackStateEntity WHERE IsSelected = ?", true).Count > 0;
        }

        public static FeedbackStateEntity GetSelected()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<FeedbackStateEntity>("SELECT * FROM FeedbackStateEntity WHERE IsSelected = ?", true)[0];
        }

        public static List<FeedbackStateEntity> GetActiveList()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<FeedbackStateEntity>("SELECT * FROM FeedbackStateEntity ORDER BY StateId").ToList();
        }
    }
}
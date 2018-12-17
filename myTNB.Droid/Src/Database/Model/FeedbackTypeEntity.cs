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
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Utils;
namespace myTNB_Android.Src.Database.Model
{
    public class FeedbackTypeEntity
    {
        [PrimaryKey, Column("FeedbackTypeId")]
        public string Id { get; set; }

        [Column("FeedbackTypeName")]
        public string Name { get; set; }

        [Column("IsSelected")]
        public bool IsSelected { get; set; }

        public static int CreateTable()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.CreateTable<FeedbackTypeEntity>();
        }

        public static int InsertOrReplace(FeedbackType type)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            var newRecord = new FeedbackTypeEntity()
            {
                Id = type.FeedbackTypeId,
                Name = type.FeedbackTypeName
            };

            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
        }

        public static int InsertOrReplace(FeedbackType type , bool isSelected)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            var newRecord = new FeedbackTypeEntity()
            {
                Id = type.FeedbackTypeId,
                Name = type.FeedbackTypeName,
                IsSelected = isSelected
            };

            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
        }


        public static void SetSelected(string Id)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            db.Execute("UPDATE FeedbackTypeEntity SET IsSelected = ? WHERE FeedbackTypeId = ?", true, Id);
        }

        public static void ResetSelected()
        {
            RemoveActive();
            if (HasRecords())
            {
                FeedbackTypeEntity state = GetFirstOrSelected();
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
            db.Execute("DELETE FROM FeedbackTypeEntity");
        }

        public static void RemoveActive()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            db.Execute("UPDATE FeedbackTypeEntity SET IsSelected = ?", false);
        }


        public static bool HasRecords()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<FeedbackTypeEntity>("SELECT * FROM FeedbackTypeEntity").Count > 0;
        }

        public static FeedbackTypeEntity GetFirstOrSelected()
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
            return db.Query<FeedbackTypeEntity>("SELECT * FROM FeedbackTypeEntity WHERE IsSelected = ?", true).Count > 0;
        }

        public static FeedbackTypeEntity GetSelected()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<FeedbackTypeEntity>("SELECT * FROM FeedbackTypeEntity WHERE IsSelected = ?", true)[0];
        }

        public static List<FeedbackTypeEntity> GetActiveList()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<FeedbackTypeEntity>("SELECT * FROM FeedbackTypeEntity").ToList();
        }
    }
}
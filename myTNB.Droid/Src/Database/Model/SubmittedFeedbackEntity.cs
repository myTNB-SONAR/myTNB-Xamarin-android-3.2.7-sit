﻿using System;
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
using myTNB_Android.Src.Base.Models;

namespace myTNB_Android.Src.Database.Model
{
    [Table("SubmittedFeedbackEntity")]
    public class SubmittedFeedbackEntity
    {
        [PrimaryKey, Column("FeedbackId")]
        public string Id { get; set; }

        [Column("DateCreated")]
        public string DateCreated { get; set; }

        [Column("FeedbackMessage")]
        public string FeedbackMessage { get; set; }

        [Column("FeedbackCategoryName")]
        public string FeedbackCategoryName { get; set; }

        [Column("FeedbackCategoryId")]
        public string FeedbackCategoryId { get; set; }

        public static int CreateTable()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.CreateTable<SubmittedFeedbackEntity>();
        }



        public static int InsertOrReplace(SubmittedFeedback submittedFeedback)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            var newRecord = new SubmittedFeedbackEntity()
            {
                Id = submittedFeedback.FeedbackId,
                DateCreated = submittedFeedback.DateCreated,
                FeedbackMessage = submittedFeedback.FeedbackMessage,
                FeedbackCategoryName = submittedFeedback.FeedbackCategoryName,
                FeedbackCategoryId = submittedFeedback.FeedbackCategoryId
            };

            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
        }

        public static int InsertOrReplace(string Id, string DateCreated, string FeedbackMessage, string FeedbackCategoryName, string FeedbackCategoryId)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            var newRecord = new SubmittedFeedbackEntity()
            {
                Id = Id,
                DateCreated = DateCreated,
                FeedbackMessage = FeedbackMessage,
                FeedbackCategoryName = FeedbackCategoryName,
                FeedbackCategoryId = FeedbackCategoryId
            };

            int newRecordRow = db.InsertOrReplace(newRecord);

            return newRecordRow;
        }

        public static void Remove()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            db.Execute("DELETE FROM SubmittedFeedbackEntity");
        }


        public static bool HasRecords()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<SubmittedFeedbackEntity>("SELECT * FROM SubmittedFeedbackEntity").Count > 0;
        }

        public static int Count()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<SubmittedFeedbackEntity>("SELECT * FROM SubmittedFeedbackEntity").Count();
        }

  
        public static List<SubmittedFeedbackEntity> GetActiveList()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<SubmittedFeedbackEntity>("SELECT * FROM SubmittedFeedbackEntity").ToList();
        }
    }
}
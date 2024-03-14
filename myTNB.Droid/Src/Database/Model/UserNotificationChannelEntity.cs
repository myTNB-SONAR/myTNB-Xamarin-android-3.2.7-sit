﻿using myTNB.Android.Src.myTNBMenu.Models;
using myTNB.Android.Src.Utils;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace myTNB.Android.Src.Database.Model
{
    [Table("UserNotificationChannelEntity")]
    public class UserNotificationChannelEntity
    {
        //        "Id": "1000004",
        //"Title": "Billing & Payment",
        //"Code": "BP",
        //"PreferenceMode": "M",
        //"Type": "ANNOUNCEMENT",
        //"CreatedDate": "11/7/2017 4:37:57 PM",
        //"MasterId": null,
        //"IsOpted": null


        [Column("Id")]
        public string Id { get; set; }

        [Column("Title")]
        public string Title { get; set; }

        [Unique, Column("Code")]
        public string Code { get; set; }

        [Column("PreferenceMode")]
        public string PreferenceMode { get; set; }

        [Column("Type")]
        public string Type { get; set; }

        [Column("CreatedDate")]
        public string CreatedDate { get; set; }

        [Column("MasterId")]
        public string MasterId { get; set; }

        [Column("IsOpted")]
        public bool IsOpted { get; set; }

        [Column("ShowInPreference")]
        public bool ShowInPreference { get; set; }

        [Column("ShowInFilterList")]
        public bool ShowInFilterList { get; set; }

        public static int CreateTable()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return (int)db.CreateTable<UserNotificationChannelEntity>();
            //}
        }

        public static void CreateTableAsync(SQLiteAsyncConnection db)
        {
            db.CreateTableAsync<UserNotificationChannelEntity>();
        }

        public static int InsertOrReplace(UserNotificationChannel channel)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new UserNotificationChannelEntity()
            {
                Id = channel.Id,
                Title = channel.Title,
                Code = channel.Code,
                PreferenceMode = channel.PreferenceMode,
                Type = channel.Type,
                CreatedDate = channel.CreatedDate,
                MasterId = channel.MasterId,
                IsOpted = channel.IsOpted,
                ShowInFilterList = channel.ShowInFilterList,
                ShowInPreference = channel.ShowInPreference
            };

            int rows = db.InsertOrReplace(newRecord);

            return rows;
            //}
        }

        public static void RemoveActive()
        {
            try
            {
                //using (var db = new SQLiteConnection(Constants.DB_PATH))
                //{
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("Delete from UserNotificationChannelEntity");
                //}
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }

        public static void UpdateIsOpted(string Code, bool isOpted)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            db.Execute("Update UserNotificationChannelEntity set IsOpted = ? WHERE Code = ?", isOpted, Code);
            //}
        }

        public static void UpdateIsOpted(string Id, string Code, bool isOpted)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            db.Execute("Update UserNotificationChannelEntity set Id = ? , IsOpted = ? WHERE Code = ?", Id, isOpted, Code);
            //}
        }

        public static List<UserNotificationChannelEntity> ListAllActive()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return db.Query<UserNotificationChannelEntity>("select * from UserNotificationChannelEntity ").ToList<UserNotificationChannelEntity>();
            //}
        }

    }
}
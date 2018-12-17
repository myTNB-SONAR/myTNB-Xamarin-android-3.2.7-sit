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
using myTNB_Android.Src.myTNBMenu.Models;

namespace myTNB_Android.Src.Database.Model
{
    [Table("UserNotificationTypesEntity")]
    public class UserNotificationTypesEntity
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

        [Unique , Column("Code")]
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
        public bool ShowInPreference  { get; set; }

        [Column("ShowInFilterList")]
        public bool ShowInFilterList { get; set; }


        public static int CreateTable()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.CreateTable<UserNotificationTypesEntity>();
        }

        public static void CreateTableAsync(SQLiteAsyncConnection db)
        {
            db.CreateTableAsync<UserNotificationTypesEntity>();
        }

        public static int InsertOrReplace(UserNotificationType channel)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            var newRecord = new UserNotificationTypesEntity()
            {
                Id = channel.Id,
                Title = channel.Title,
                Code = channel.Code,
                PreferenceMode = channel.PreferenceMode,
                Type = channel.Type,
                CreatedDate = channel.CreatedDate,
                MasterId = channel.MasterId,
                IsOpted = channel.IsOpted,
                ShowInPreference = channel.ShowInPreference,
                ShowInFilterList = channel.ShowInFilterList
            };

            int rows = db.InsertOrReplace(newRecord);

            return rows;
        }

        public static int RemoveActive()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Execute("Delete from UserNotificationTypesEntity");
        }

        public static void UpdateIsOpted(string Code, bool isOpted)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            db.Execute("Update UserNotificationTypesEntity set IsOpted = ? WHERE Code = ?", isOpted, Code);
        }

        public static void UpdateIsOpted(string Id, string Code, bool isOpted)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            db.Execute("Update UserNotificationTypesEntity set Id = ? , IsOpted = ? WHERE Code = ?",Id, isOpted, Code);
        }

        public static List<UserNotificationTypesEntity> ListAllActive()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.Query<UserNotificationTypesEntity>("select * from UserNotificationTypesEntity ").ToList<UserNotificationTypesEntity>();
        }
    }
}
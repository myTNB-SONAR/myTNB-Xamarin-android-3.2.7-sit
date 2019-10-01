﻿using myTNB_Android.Src.Login.Models;
using myTNB_Android.Src.Utils;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace myTNB_Android.Src.Database.Model
{
    [Table("UserEntity")]
    public class UserEntity
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }

        [Unique, Column("userID")]
        public string UserID { get; set; }

        [Column("displayName")]
        public string DisplayName { get; set; }

        [Column("userName")]
        public string UserName { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("identificationNo")]
        public string IdentificationNo { get; set; }

        [Column("mobileNo")]
        public string MobileNo { get; set; }

        [Column("dateCreated")]
        public string DateCreated { get; set; }

        [Column("lastLoginDate")]
        public string LastLoginDate { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("deviceId")]
        public string DeviceId { get; set; }


        public static int CreateTable()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return (int)db.CreateTable<UserEntity>();
            //}
        }

        public static void CreateTableAsync(SQLiteAsyncConnection db)
        {
            db.CreateTableAsync<UserEntity>();
        }

        public static int InsertOrReplace(User user)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new UserEntity()
            {
                UserID = user.UserId,
                DisplayName = user.DisplayName,
                UserName = user.UserName,
                Email = user.Email,
                DateCreated = user.DateCreated ?? "",
                LastLoginDate = user.LastLoginDate ?? "",
                MobileNo = user.MobileNo,
                IdentificationNo = user.IdentificationNo,
                Status = Constants.ACTIVE
            };

            int newRecordId = db.InsertOrReplace(newRecord);
            if (newRecordId > 0)
            {
                return newRecord.Id ?? 0;
            }

            return 0;
            //}
        }

        public static int UpdatePhoneNumber(string newPhoneNumber)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return db.Execute("UPDATE UserEntity SET mobileNo = ?", newPhoneNumber);
            //}
        }

        public static int UpdateDeviceId(string deviceId)
        {
            var db = DBHelper.GetSQLiteConnection();
            return db.Execute("UPDATE UserEntity SET deviceId = ?", deviceId);
        }

        public static IEnumerable<UserEntity> ListAllActive()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return db.Query<UserEntity>("select * from UserEntity where status = ?", Constants.ACTIVE);
            //}
        }

        public static UserEntity GetActive()
        {
            List<UserEntity> activeList = ListAllActive().ToList<UserEntity>();
            if (activeList != null && activeList.Count() > 0)
            {
                return activeList[0];
            }
            return null;
        }


        public static Boolean IsCurrentlyActive()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            List<UserEntity> userList = db.Query<UserEntity>("select * from UserEntity where status = ?", Constants.ACTIVE);
            if (userList.Count > 0)
            {
                return true;
            }

            return false;
            //}


        }



        public static int RemoveActive()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return db.Execute("Delete from UserEntity where status = ? ", Constants.ACTIVE);
            //}
        }


    }
}
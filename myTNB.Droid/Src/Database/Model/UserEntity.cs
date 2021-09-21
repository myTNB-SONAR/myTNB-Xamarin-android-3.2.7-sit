using myTNB_Android.Src.Login.Models;
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

        [Column("selectedLanguage")]
        public string SelectedLanguage { get; set; }

        [Column("isActivated")]
        public bool IsActivated { get; set; }


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
                Status = Constants.ACTIVE,
                IsActivated = user.IsActivated
            };

            int newRecordId = db.InsertOrReplace(newRecord);
            if (newRecordId > 0)
            {
                return newRecord.Id ?? 0;
            }

            return 0;
            //}
        }

        public static int UpdateFullname(string fullname)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return db.Execute("UPDATE UserEntity SET displayName = ?", fullname);
            //}
        }

        public static int UpdateICno(string icno)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return db.Execute("UPDATE UserEntity SET identificationNo = ?", icno);
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

        public static int UpdateSelectedLanguage(string language)
        {
            var db = DBHelper.GetSQLiteConnection();
            return db.Execute("UPDATE UserEntity SET selectedLanguage = ?", language);
        }

        public static string GetSelectedLanguage()
        {
            var db = DBHelper.GetSQLiteConnection();
            List<UserEntity> userEntityList = db.Query<UserEntity>("select selectedLanguage from UserEntity");
            return userEntityList[0].SelectedLanguage;
        }

        public static void RemoveActive()
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("Delete from UserEntity where status = ? ", Constants.ACTIVE);
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }


    }

    [Table("UserLoginCountEntity")]
    public class UserLoginCountEntity
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("loginCount")]
        public int LoginCount { get; set; }

        [Column("dateCreated")]
        public string DateCreated { get; set; }

        [Column("lastLoginDate")]
        public string LastLoginDate { get; set; }


        public static int CreateTable()
        {
            var db = DBHelper.GetSQLiteConnection();
            return (int)db.CreateTable<UserLoginCountEntity>();
        }

        public static void CreateTableAsync(SQLiteAsyncConnection db)
        {
            db.CreateTableAsync<UserLoginCountEntity>();
        }

        public static int InsertOrReplace(User user, int userLoginCount)
        {
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new UserLoginCountEntity()
            {
                
                Email = user.Email,
                LoginCount = userLoginCount,
                DateCreated = user.DateCreated ?? "",
                LastLoginDate = user.LastLoginDate ?? ""
            };

            int newRecordId = db.InsertOrReplace(newRecord);
            if (newRecordId > 0)
            {
                return newRecord.Id ?? 0;
            }

            return 0;
        }

        public static int GetLoginCount(string email)
        {
            int count =0;
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                List<UserLoginCountEntity> userLoginCountEntity = db.Query<UserLoginCountEntity>("select * from UserLoginCountEntity where Email= ?", email);

                count = userLoginCountEntity.Count > 0 ? userLoginCountEntity.Count : 0;
            }
            catch(Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return count;
        }
        public static void RemoveAll()
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("DELETE FROM UserLoginCountEntity");
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }
    }
}
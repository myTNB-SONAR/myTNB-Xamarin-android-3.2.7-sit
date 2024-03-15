using myTNB.AndroidApp.Src.Utils;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace myTNB.AndroidApp.Src.Database.Model
{
    [Table("UserRegister")]
    public class UserRegister
    {
        [PrimaryKey, Column("icNo")]
        public string ICNo { get; set; }
        [Column("fullName")]
        public string FullName { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("mobileNo")]
        public string MobileNo { get; set; }
        [Column("verificationCode")]
        public string VerificationCode { get; set; }
        [Column("password")]
        public string Password { get; set; }
        [Column("status")]
        public int Status { get; set; }

        public static int CreateTable()
        {
            //var db = new SQLiteConnection(Constants.DB_PATH);
            var db = DBHelper.GetSQLiteConnection();
            return (int)db.CreateTable<UserRegister>();
        }

        /// <summary>
        /// Save a registration data 
        /// </summary>
        /// <param name="icNo"></param>
        /// <param name="fullname"></param>
        /// <param name="email"></param>
        /// <param name="mobileNo"></param>
        /// <param name="verificationCode"></param>
        /// <param name="password"></param>
        /// <returns>Number of rows affected</returns>
        public static int InsertOrReplace(string icNo, string fullname, string email, string mobileNo, string verificationCode, string password)
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            var newRecord = new UserRegister()
            {
                ICNo = icNo,
                FullName = fullname,
                Email = email,
                MobileNo = mobileNo,
                VerificationCode = verificationCode,
                Password = password,
                Status = Constants.ACTIVE
            };
            return db.InsertOrReplace(newRecord);
            //}

        }

        public static IEnumerable<UserRegister> GetListOfActive()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return db.Query<UserRegister>("select * from UserRegister where status = ?", Constants.ACTIVE);
            //}
        }

        public static Boolean HasActive()
        {
            List<UserRegister> listOfActive = GetListOfActive().ToList<UserRegister>();
            return listOfActive.Count > 0;
        }

        public static int DeActivate()
        {
            //using (var db = new SQLiteConnection(Constants.DB_PATH))
            //{
            var db = DBHelper.GetSQLiteConnection();
            return db.Execute("Update UserRegister set status = ? ", Constants.INACTIVE);
            //}
        }

        public static UserRegister GetActive()
        {
            List<UserRegister> listOfActive = GetListOfActive().ToList<UserRegister>();
            return listOfActive[0];
        }

        public static void RemoveActive()
        {
            try
            {
                var db = DBHelper.GetSQLiteConnection();
                db.Execute("Delete from UserRegister where status = ? ", Constants.ACTIVE);
            }
            catch (System.Exception ne)
            {
                Utility.LoggingNonFatalError(ne);
            }
        }
    }
}
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
    [Table("AccountTypeEntity")]
    public class AccountTypeEntity
    {


        [PrimaryKey, Column("accountType")]
        public int? AccountType { get; set; }

        [MaxLength(50), Column("type")]
        public string Type { get; set; }

        
        [Column("accountTypeName")]
        public string AccountTypeName { get; set; }

        public AccountTypeEntity()
        {
        }

        public static int CreateTable(SQLiteConnection db)
        {
            return db.CreateTable<AccountTypeEntity>();
        }

        public static int CreateTable()
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            return db.CreateTable<AccountTypeEntity>();
        }

        public static void CreateTableAsync(SQLiteAsyncConnection db)
        {
            db.CreateTableAsync<AccountTypeEntity>();
        }

        public static int InsertOrReplace(string type , int accountType, string accountTypeName)
        {
            var db = new SQLiteConnection(Constants.DB_PATH);
            var newRecord = new AccountTypeEntity()
            {
                Type = type,
                AccountType = accountType,
                AccountTypeName = accountTypeName
            };
            int newRecordId = db.InsertOrReplace(newRecord);
            if (newRecordId > 0)
            {
                return newRecord.AccountType ?? 0;
            }

            return 0;

            
        }

        public static int InsertOrReplace(SQLiteConnection db, string type, int accountType, string accountTypeName)
        {
            var newRecord = new AccountTypeEntity()
            {
                Type = type,
                AccountType = accountType,
                AccountTypeName = accountTypeName
            };
            int newRecordId =  db.InsertOrReplace(newRecord);
            if (newRecordId > 0)
            {
                return newRecord.AccountType ?? 0;
            }

            return 0;
        }

    }
}
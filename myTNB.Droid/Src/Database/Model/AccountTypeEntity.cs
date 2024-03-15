using SQLite;

namespace myTNB.AndroidApp.Src.Database.Model
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
            return (int)db.CreateTable<AccountTypeEntity>();
        }

        public static int CreateTable()
        {
            var db = DBHelper.GetSQLiteConnection();
            return (int)db.CreateTable<AccountTypeEntity>();
        }

        public static void CreateTableAsync(SQLiteAsyncConnection db)
        {
            db.CreateTableAsync<AccountTypeEntity>();
        }

        public static int InsertOrReplace(string type, int accountType, string accountTypeName)
        {
            var db = DBHelper.GetSQLiteConnection();
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
            int newRecordId = db.InsertOrReplace(newRecord);
            if (newRecordId > 0)
            {
                return newRecord.AccountType ?? 0;
            }

            return 0;
        }

    }
}
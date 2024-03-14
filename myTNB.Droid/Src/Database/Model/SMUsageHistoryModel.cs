using SQLite;
using System;

namespace myTNB.Android.Src.Database.Model
{
    public class SMUsageHistoryModel
    {
        [PrimaryKey, Column("AccountNo")]
        public String AccountNo { set; get; }

        public DateTime Timestamp { set; get; }

        public String JsonResponse { set; get; }
    }
}
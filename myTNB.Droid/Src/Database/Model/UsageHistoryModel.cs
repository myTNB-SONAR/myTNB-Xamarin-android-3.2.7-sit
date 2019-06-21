﻿using SQLite;
using System;

namespace myTNB_Android.Src.Database.Model
{
    public class UsageHistoryModel
    {
        [PrimaryKey, Column("AccountNo")]
        public String AccountNo { set; get; }
        public DateTime Timestamp { set; get; }
        public String JsonResponse { set; get; }
    }
}
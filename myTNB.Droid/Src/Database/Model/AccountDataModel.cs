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
using Java.Sql;
using SQLite;

namespace myTNB_Android.Src.Database.Model
{
    public class AccountDataModel
    {
        [PrimaryKey, Column("AccountNo")]
        public String AccountNo { set; get; }

        public DateTime Timestamp { set; get; }        

        public String JsonResponse { set; get; }
    }
}
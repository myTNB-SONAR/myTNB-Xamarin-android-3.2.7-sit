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

namespace myTNB_Android.Src.AddAccount.Models
{
    public class NewAccount
    {
        public string type { get; set; }

        public bool isOwner { get; set; }

        public string accountNumber { get; set; }

        public string accountLabel { get; set; }

        public string accountAddress { get; set; }

        public string icNum { get; set; }

        public string userAccountId { get; set; }

        public string amCurrentChg { get; set; }

        public bool isRegistered { get; set; }

        public bool isPaid { get; set; }

        public string accountCategoryId { get; set; }

        public string ownerName { get; set; }

        public string accountTypeId { get; set; }

        public string smartMeterCode { get; set; }

    }
}
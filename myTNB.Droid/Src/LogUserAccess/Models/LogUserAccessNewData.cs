﻿using System;

namespace myTNB.AndroidApp.Src.LogUserAccess.Models
{
    public class LogUserAccessNewData
    {
        public string AccountNo { get; set; }

        public string Action { get; set; }

        public string ActivityLogID { get; set; }

        public string CreateBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsApplyEBilling { get; set; }

        public bool IsHaveAccess { get; set; }

        public string UserID { get; set; }

        public string UserName { get; set; }

        public bool IsPreRegister { get; set; }

        public string CreateByName { get; set; }

        public string Email { get; set; }
    }
}

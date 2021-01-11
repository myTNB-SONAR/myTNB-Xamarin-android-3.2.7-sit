using System;

namespace myTNB_Android.Src.LogUserAccess.Models
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
    }
}

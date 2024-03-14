using System;
namespace myTNB.Android.Src.DBR.DBRApplication.MVP
{
    public class DBRAccount
    {
        public DBRAccount()
        {
        }

        public string accountNumber { get; set; }
        public string accountName { get; set; }
        public string accountAddress { get; set; }
        public bool accountSelected { get; set; }
        public string email { get; set; }
        public string mobileNumber { get; set; }
        public bool isTaggedSMR { get; set; }
        public string accountOwnerName { get; set; }
        public bool IsOwner { get; set; }
    }
}

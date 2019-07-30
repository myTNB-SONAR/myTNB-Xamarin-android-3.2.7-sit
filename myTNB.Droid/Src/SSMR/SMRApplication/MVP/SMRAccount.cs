using System;
namespace myTNB_Android.Src.SSMR.SMRApplication.MVP
{
    public class SMRAccount
    {
        public SMRAccount()
        {
        }

        public string accountNumber { get; set; }
        public string accountName { get; set; }
        public bool accountSelected { get; set; }
        public string email { get; set; }
        public string mobileNumber { get; set; }
    }
}

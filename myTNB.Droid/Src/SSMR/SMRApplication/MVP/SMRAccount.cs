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
        public string accountAddress { get; set; }
        public bool accountSelected { get; set; }
        public string email { get; set; }
        public string mobileNumber { get; set; }
        public bool isTaggedSMR { get; set; }
        public string accountOwnerName { get; set; }
        public string BudgetAmount { get; set; }
        public string InstallationType { get; set; }

    }
}

using System;

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

        public bool IsTaggedSMR { get; set; }

        public bool isOwned { get; set; }

        public string emailOwner { get; set; }

        public string mobileNoOwner { get; set; }

        public string ISDmobileNo { get; set; }

        public bool isNoDetailOwner { get; set; }

        public bool CountryCheck { get; set; }

        public string countryDetail { get; set; }

        public bool CountryCheckNoPhone { get; set; }
        
        public bool IsError { get; set; }

        public string BudgetAmount { get; set; }

        public string InstallationType { get; set; }

        public string CreatedDate { get; set; }

        public bool IsApplyEBilling { get; set; }

        public bool IsHaveAccess { get; set; }

        public string BusinessArea { get; set; }

        public string RateCategory { get; set; }

        public bool IsInManageAccessList { get; set; }

        public string CreatedBy { get; set; }
    }
}

using System;
namespace myTNB.AndroidApp.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
    public class AccountCardModel
    {
        public int Id
        {
            get; set;
        }

        public string AccountName
        {
            get; set;
        }

        public string AccountNumber
        {
            get; set;
        }

        public string BillDueAmount
        {
            get; set;
        }

        public string BillDueNote
        {
            get; set;
        }

        public int AccountType
        {
            get; set;
        }

        public int SmartMeterCode
        {
            get; set;
        }

        public bool IsTaggedSMR
        {
            get; set;
        }

        public bool IsNegativeAmount
        {
            get; set;
        }

        public bool IsZeroAmount
        {
            get; set;
        }
    }
}

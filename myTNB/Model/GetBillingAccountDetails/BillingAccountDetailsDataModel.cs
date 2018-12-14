namespace myTNB.Model
{
    public class BillingAccountDetailsDataModel
    {
        public string accNum { set; get; }
        public string accName { set; get; }
        public string accICNo { set; get; }
        public string accICNoNew { set; get; }
        public string accComNo { set; get; }
        public double amDeposit { set; get; }
        public double amCurrentChg { set; get; }
        public double amOutstandingChg { set; get; }
        public double amPayableChg { set; get; }
        public double amLastPay { set; get; }
        public string dateBill { set; get; }
        public string datePymtDue { set; get; }
        public string dateLastPay { set; get; }
        public string sttSupply { set; get; }
        public string addStreet { set; get; }
        public string addArea { set; get; }
        public string addTown { set; get; }
        public string addState { set; get; }
        public string stnName { set; get; }
        public string stnAddStreet { set; get; }
        public string stnAddArea { set; get; }
        public string stnAddTown { set; get; }
        public string stnAddState { set; get; }
        public double amCustBal { set; get; }
        public string isError { set; get; }
        public string message { set; get; }
    }
}
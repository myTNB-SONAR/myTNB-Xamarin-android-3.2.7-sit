namespace myTNB.Model
{
    public class PaymentHistoryDataModel
    {
        #region Normal Account fields
        public string DtEvent { set; get; }
        public string DtInput { set; get; }
        public string AmPaid { set; get; }
        public string CdPBranch { set; get; }
        public string NmPBranch { set; get; }
        public string isError { set; get; }
        public string message { set; get; }
        public string DocumentNumber { set; get; }
        public string MechantTransId { set; get; }

        #endregion

        #region RE Account fields
        public string Amount { set; get; }
        public string BillConsumption { set; get; }
        public string DocumentDate { set; get; }
        public string DocumentNo { set; get; }
        public string PaidDate { set; get; }
        #endregion

    }
}
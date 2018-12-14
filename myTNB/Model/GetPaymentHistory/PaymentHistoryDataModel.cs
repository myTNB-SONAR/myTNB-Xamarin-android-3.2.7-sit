namespace myTNB.Model
{
    public class PaymentHistoryDataModel
    {
        public string DtEvent { set; get; }
        public string DtInput { set; get; }
        public string AmPaid { set; get; }
        public string CdPBranch { set; get; }
        public string NmPBranch { set; get; }
        public string isError { set; get; }
        public string message { set; get; }
        public string DocumentNumber { set; get; }
        public string MechantTransId { set; get; }
    }
}
namespace myTNB.Model
{
    public class BillHistoryDataModel
    {
        public string BillingNo { set; get; }
        public string NrBill { set; get; }
        public string DtBill { set; get; }
        public string AmPayable { set; get; }
        public string QtUnits { set; get; }
        public string isError { set; get; }
        public string message { set; get; }
    }
}
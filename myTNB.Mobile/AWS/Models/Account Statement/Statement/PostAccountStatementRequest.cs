namespace myTNB.Mobile.AWS.Models.AccountStatement
{
    public class PostAccountStatementRequest
    {
        public string ReferenceNo { set; get; }
        public string CaNo { set; get; }
        public string StatementPeriod { set; get; }
        public bool IsOwnedAccount { set; get; }
    }
}
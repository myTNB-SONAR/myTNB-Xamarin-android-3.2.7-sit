using System;
namespace myTNB.Android.Src.MyTNBService.Request
{
    public class GetPaymentReceiptRequest : BaseRequest
    {
        public string contractAccount, detailedInfoNumber;
        public bool isOwnedAccount, showAllReceipts;

        public GetPaymentReceiptRequest(string contractAccount, string detailedInfoNumber, bool isOwnedAccount, bool showAllReceipts)
        {
            this.contractAccount = contractAccount;
            this.detailedInfoNumber = detailedInfoNumber;
            this.isOwnedAccount = isOwnedAccount;
            this.showAllReceipts = showAllReceipts;
        }
    }
}

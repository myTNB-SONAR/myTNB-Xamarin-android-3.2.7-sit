using System;
namespace myTNB.AndroidApp.Src.MyTNBService.Request
{
    public class GetBillHistoryRequest : BaseRequest
    {
        public string contractAccount;
        public bool isOwnedAccount;

        public GetBillHistoryRequest(string contractAccount, bool isOwnedAccount)
        {
            this.contractAccount = contractAccount;
            this.isOwnedAccount = isOwnedAccount;
        }
    }
}

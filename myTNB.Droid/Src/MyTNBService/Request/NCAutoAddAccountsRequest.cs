using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class NCAutoAddAccountsRequest : BaseRequestV4
    {
        public string IdNo;

        public NCAutoAddAccountsRequest(string IdNo)
        {
            this.IdNo = IdNo;
        }
    }
}

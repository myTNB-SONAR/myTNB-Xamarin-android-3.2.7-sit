using System;
namespace myTNB.AndroidApp.Src.MyTNBService.Request
{
    public class GetAccountAccessRight : BaseRequest
    {
        public string accountNo;

        public GetAccountAccessRight(string accNum)
        {
            this.accountNo = accNum;
        }
    }
}

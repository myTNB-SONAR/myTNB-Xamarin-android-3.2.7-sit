using System;
namespace myTNB_Android.Src.MyTNBService.Request
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

using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class RemoveAccountRequest : BaseRequest
    {
        public string accNum;

        public RemoveAccountRequest(string accNum)
        {
            this.accNum = accNum;
        }
    }
}

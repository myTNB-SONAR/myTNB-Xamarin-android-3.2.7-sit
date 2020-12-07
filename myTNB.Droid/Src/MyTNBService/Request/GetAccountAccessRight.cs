using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class GetAccountAccessRight : BaseRequest
    {
        public string accountNo, usrInf;
        //public DeviceInfoRequest deviceInf;

        public GetAccountAccessRight(string accNum, string usr)
        {
            this.accountNo = accNum;
            this.usrInf = usr;
        }
    }
}

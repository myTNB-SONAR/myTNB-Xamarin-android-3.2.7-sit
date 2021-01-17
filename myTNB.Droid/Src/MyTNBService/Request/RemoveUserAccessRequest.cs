using Java.Util;
using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class RemoveUserAccountRequest : BaseRequest
    {
        public string accNum;
        public DeviceInfoRequest deviceInf;
        public String[] accountIdList;
        //public ArrayList accountIdList = new ArrayList();


        public RemoveUserAccountRequest(String[] nameList)
        {
            deviceInf = new DeviceInfoRequest();
            this.accountIdList = nameList;
        }
    }
}

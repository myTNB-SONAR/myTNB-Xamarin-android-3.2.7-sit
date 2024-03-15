using System;
namespace myTNB.AndroidApp.Src.MyTNBService.Request
{
    public class SubmittedFeedbeckClaimIdDetailRequestModel : BaseRequest
    {

        public string serviceReqNo;
        public DeviceInfoRequest deviceInf;
        public SubmittedFeedbeckClaimIdDetailRequestModel(string serviceReqNo)
        {
            this.serviceReqNo = serviceReqNo;
            deviceInf = new DeviceInfoRequest();
        }
    }
}

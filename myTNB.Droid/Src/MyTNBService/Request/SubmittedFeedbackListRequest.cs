﻿using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class SubmittedFeedbackListRequest : BaseRequest
    {
        public DeviceInfoRequest deviceInf;
        public SubmittedFeedbackListRequest()
        {
            deviceInf = new DeviceInfoRequest();
        }
    }
}

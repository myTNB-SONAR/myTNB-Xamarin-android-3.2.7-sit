﻿using System;
namespace myTNB.AndroidApp.Src.MyTNBService.Request
{
    public class SendUpdatePhoneTokenSMSRequest : BaseRequest
    {
        public string mobileNo;
        public string oldPhoneNumber;

        public SendUpdatePhoneTokenSMSRequest(string mobileNo, string oldPhoneNumber)
        {
            this.mobileNo = mobileNo;
            this.oldPhoneNumber = oldPhoneNumber;
        }
    }
}

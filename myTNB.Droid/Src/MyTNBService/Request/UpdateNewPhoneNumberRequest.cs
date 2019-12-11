﻿using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class UpdateNewPhoneNumberRequest : BaseRequest
    {
        public string oldPhoneNumber, newPhoneNumber, token;

        public UpdateNewPhoneNumberRequest(string oldPhoneNumber, string newPhoneNumber, string token)
        {
            this.oldPhoneNumber = oldPhoneNumber;
            this.newPhoneNumber = newPhoneNumber;
            this.token = token;
        }
    }
}

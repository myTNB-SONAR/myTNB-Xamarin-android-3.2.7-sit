﻿using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class UpdateUserInfo : BaseRequest
    {
        public string name, mobileNo;

        public UpdateUserInfo(string mobile, string fname)
        {
            this.mobileNo = mobile;
            this.name = fname;
        }
    }
}

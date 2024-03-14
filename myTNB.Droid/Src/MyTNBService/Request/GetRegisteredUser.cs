﻿using System;
namespace myTNB.Android.Src.MyTNBService.Request
{
    public class GetRegisteredUser : BaseRequest
    {
        public string IdType, IdNo;

        public GetRegisteredUser(string idType, string idNo)
        {
            this.IdType = idType;
            this.IdNo = idNo;
        }
    }
}

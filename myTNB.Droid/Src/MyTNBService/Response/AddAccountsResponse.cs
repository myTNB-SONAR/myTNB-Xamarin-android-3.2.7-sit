﻿using System;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.MyTNBService.Response
{
    public class AddAccountsResponse : BaseResponse<List<AddAccount.Models.AddAccount>>
    {
        public List<AddAccount.Models.AddAccount> GetData()
        {
            return Response.Data;
        }
    }
}

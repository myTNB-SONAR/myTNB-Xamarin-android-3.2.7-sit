﻿using myTNB_Android.Src.MultipleAccountPayment.Model;
using System.Collections.Generic;

namespace myTNB_Android.Src.MultipleAccountPayment.Requests
{
    public class MPGetAccountDueAmountRequest : MPAccountDueRequest
    {

        public MPGetAccountDueAmountRequest(string apiKeyId, List<string> accounts) : base(apiKeyId, accounts)
        {
            base.apiKeyID = apiKeyID;
            base.accounts = accounts;
        }
    }
}
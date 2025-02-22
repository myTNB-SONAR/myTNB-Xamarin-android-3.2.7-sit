﻿using System;
using myTNB.AndroidApp.Src.Base.Request;

namespace myTNB.AndroidApp.Src.MyTNBService.Request
{
    public class AccountReceiptRequest : APIBaseRequest
    {
        public string contractAccount { get; set; }
        public string detailedInfoNumber { get; set; }
        public bool isOwnedAccount { get; set; }
        public bool showAllReceipts { get; set; }


        public AccountReceiptRequest(string contractAccount, string detailedInfoNumber, bool isOwnedAccount, bool showAllReceipts)
		{
            this.contractAccount = contractAccount;
            this.detailedInfoNumber = detailedInfoNumber;
            this.isOwnedAccount = isOwnedAccount;
            this.showAllReceipts = showAllReceipts;
		}
	}
}

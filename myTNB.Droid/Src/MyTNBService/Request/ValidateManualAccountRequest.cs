using System;
namespace myTNB.Android.Src.MyTNBService.Request
{
    public class ValidateManualAccountRequest : BaseRequest
    {
		public string accountNum, accountType, userIdentificationNum, suppliedMotherName, isOwner;

        public ValidateManualAccountRequest(string accountNum, string accountType, string userIdentificationNum, string suppliedMotherName, string isOwner)
        {
            this.accountNum = accountNum;
            this.accountType = accountType;
            this.userIdentificationNum = userIdentificationNum;
            this.suppliedMotherName = suppliedMotherName;
            this.isOwner = isOwner;
        }
    }
}

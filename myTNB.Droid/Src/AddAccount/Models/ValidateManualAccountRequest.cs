using Refit;

namespace myTNB.Android.Src.AddAccount.Models
{
    public class ValidateManualAccountRequest
    {
        [AliasAs("apiKeyID")]
        public string apiKeyID { get; set; }

        [AliasAs("accountNum")]
        public string accountNum { get; set; }

        [AliasAs("accountType")]
        public string accountType { get; set; }

        [AliasAs("userIdentificationNum")]
        public string userIdentificationNum { get; set; }

        [AliasAs("suppliedMotherName")]
        public string suppliedMotherName { get; set; }

        [AliasAs("isOwner")]
        public bool isOwner { get; set; }


        public ValidateManualAccountRequest(string apiKeyID, string accountNum, string accountType, string userIdentificationNum, string suppliedMotherName, bool isOwner)
        {
            this.apiKeyID = apiKeyID;
            this.accountNum = accountNum;
            this.accountType = accountType;
            this.userIdentificationNum = userIdentificationNum;
            this.suppliedMotherName = suppliedMotherName;
            this.isOwner = isOwner;
        }
    }
}